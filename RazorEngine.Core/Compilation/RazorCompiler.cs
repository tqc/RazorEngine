namespace RazorEngine.Compilation
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    using Templating;

    /// <summary>
    /// Provides compilation services for razor templates.
    /// </summary>
    internal class RazorCompiler
    {
        #region Fields
        private readonly ILanguageProvider languageProvider;
        private readonly Type templateBaseType;
        private readonly MarkupParser markupParser;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="RazorCompiler"/>
        /// </summary>
        /// <param name="provider">The language provider used to create language services.</param>
        /// <param name="baseType">[Optional] The template base type.</param>
        /// <param name="parser">[Optional] The markup parser.</param>
        public RazorCompiler(ILanguageProvider provider, Type baseType = null, MarkupParser parser = null)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (baseType != null && !typeof(ITemplate).IsAssignableFrom(baseType))
                throw new ArgumentException(
                    string.Format("{0} is not a valid template base.  Templates must inherit from ITemplate.",
                                  baseType.FullName));

            // Need to initialise this type to ensure assemblies are loaded for referencing. Eugh....
            var temp1 = Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None;

            languageProvider = provider;
            templateBaseType = baseType;

            markupParser = parser ?? new HtmlMarkupParser();

            Namespaces = new List<string> { "System", "System.Collections.Generic", "System.Linq" };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of namespaces.
        /// </summary>
        public IList<string> Namespaces { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds any required namespace imports to the generated 
        /// </summary>
        /// <param name="generator">The <see cref="CodeNamespace"/> for the generated code unit.</param>
        private void AddNamespaceImports(CodeNamespace generator)
        {
            generator.Imports.AddRange(Namespaces.Select(n => new CodeNamespaceImport(n)).ToArray());

            if (templateBaseType == null)
                return;

            templateBaseType.GetCustomAttributes(typeof(RequireNamespacesAttribute), true)
                .OfType<RequireNamespacesAttribute>()
                .ToList()
                .ForEach(a => generator.Imports.AddRange(
                    a.Namespaces.Select(n => new CodeNamespaceImport(n))
                    .ToArray()));
        }

        /// <summary>
        /// Adds any required references to the compiler parameters.
        /// </summary>
        /// <param name="parameters">The compiler parameters.</param>
        private void AddReferences(CompilerParameters parameters)
        {
            var list = new List<string>();
            foreach (string location in GetCoreReferences())
            {
                list.Add(location.ToLowerInvariant());
            }

            foreach (string location in GetBaseTypeReferencedAssemblies())
            {
                list.Add(location.ToLowerInvariant());
            }

            foreach (string location in list)
                System.Diagnostics.Debug.Print(location);

            parameters.ReferencedAssemblies.AddRange(list.Distinct().ToArray());
        }

        /// <summary>
        /// Builds the full declarative name of the specified type with a dynamic type argument.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The declarative name for the type.</returns>
        private static string BuildDynamicName(Type type)
        {
            return type.Namespace + "." + type.Name.Substring(0, type.Name.IndexOf('`')) + "<dynamic>";
        }

        /// <summary>
        /// Builds the full declarative name of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The declarative name for the type.</returns>
        private static string BuildName(ILanguageProvider provider, Type type)
        {
            return provider.GenerateClassName(type);
        }

        /// <summary>
        /// Compiles the string template into a <see cref="ITemplate"/>.
        /// </summary>
        /// <param name="className">The name of the compiled class.</param>
        /// <param name="template">The template to compile.</param>
        /// <param name="modelType">[Optional] The model type.</param>
        /// <returns>The results of compilation.</returns>
        public CompilerResults CompileTemplate(string className, string template, Type modelType = null)
        {
            var service = languageProvider.CreateLanguageService();
            var codeDom = languageProvider.CreateCodeDomProvider();
            var host = new RazorEngineHost(service);
            host.GeneratedClassContext = new GeneratedClassContext("Execute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo", "RazorEngine.Templating.TemplateWriter");

            var generator = service.CreateCodeGenerator(className, "RazorEngine.Dynamic", null, host);
            var codeParser = service.CreateCodeParser();

            var parser = new RazorParser(codeParser, markupParser);

            AddNamespaceImports(generator.GeneratedNamespace);

            string baseType = GetBaseTypeDeclaration(languageProvider, modelType, templateBaseType);
            generator.GeneratedClass.BaseTypes.Add(baseType);

            if ((modelType != null) && IsAnonymousType(modelType))
                generator.GeneratedClass.CustomAttributes.Add(
                    new CodeAttributeDeclaration(
                        new CodeTypeReference(typeof(HasDynamicModelAttribute))));

            ParseTemplate(template, parser, generator);

            var builder = new StringBuilder();
            GenerateCode(codeDom, generator, builder);

            var parameters = new CompilerParameters();
            AddReferences(parameters);

            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            parameters.IncludeDebugInformation = false;
            parameters.CompilerOptions = "/target:library /optimize";

            return codeDom.CompileAssemblyFromSource(parameters, new[] { builder.ToString() });
        }

        /// <summary>
        /// Creates a <see cref="ITemplate"/> from the specified string template.
        /// </summary>
        /// <param name="template">The string template to create a <see cref="ITemplate"/> for.</param>
        /// <param name="modelType">[Optional] The model type.</param>
        /// <returns>An instance of <see cref="ITemplate"/>.</returns>
        public ITemplate CreateTemplate(string template, Type modelType = null)
        {
            string className = Regex.Replace(Guid.NewGuid().ToString("N"), @"[^A-Za-z]*", "");

            var result = CompileTemplate(className, template, modelType);

            if (result.Errors != null && result.Errors.Count > 0)
                throw new TemplateCompilationException(result.Errors);

            try
            {
                ITemplate instance = (ITemplate)result.CompiledAssembly.CreateInstance("RazorEngine.Dynamic." + className);

                return instance;
            }
            catch (TargetInvocationException tex)
            {
                throw tex.InnerException;
            }
        }

        /// <summary>
        /// Generates the required code using the specified compile unit.
        /// </summary>
        /// <param name="codeDom">The code DOM provider.</param>
        /// <param name="codeGenerator">The code generator.</param>
        /// <param name="builder">The string builder used to write the code.</param>
        private static void GenerateCode(CodeDomProvider codeDom, RazorCodeGenerator codeGenerator, StringBuilder builder)
        {
            var statement = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Clear");
            codeGenerator.GeneratedExecuteMethod.Statements.Insert(0, new CodeExpressionStatement(statement));

            using (var writer = new StringWriter(builder))
            {
                codeDom.GenerateCodeFromCompileUnit(codeGenerator.GeneratedCode, writer, new CodeGeneratorOptions());
            }
        }

        /// <summary>
        /// Gets the base type declaration for the template.
        /// </summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="templateBaseType">The template base type.</param>
        /// <returns>The base type declaration.</returns>
        private static string GetBaseTypeDeclaration(ILanguageProvider languageProvider, Type modelType, Type templateBaseType = null)
        {
            if (templateBaseType == null)
            {
                if (modelType == null)
                    return BuildName(languageProvider, typeof(TemplateBase));

                if (IsAnonymousType(modelType))
                    return BuildDynamicName(typeof(TemplateBase<>));

                return BuildName(languageProvider, typeof(TemplateBase<>).MakeGenericType(modelType));
            }

            if (modelType == null)
                return BuildName(languageProvider, templateBaseType);

            if (IsAnonymousType(modelType))
                return BuildDynamicName(templateBaseType);

            return BuildName(languageProvider, templateBaseType.MakeGenericType(modelType));
        }

        /// <summary>
        /// Gets the locations of assemblies referenced by a custom base template type.
        /// </summary>
        /// <returns>An enumerable of reference assembly locations.</returns>
        private IEnumerable<string> GetBaseTypeReferencedAssemblies()
        {
            if (templateBaseType == null)
                return new string[0];

            return templateBaseType.Assembly
                .GetReferencedAssemblies()
                .Select(n => Assembly.ReflectionOnlyLoad(n.FullName).Location);
        }

        /// <summary>
        /// Gets the locations of all core referenced assemblies.
        /// </summary>
        /// <returns>An enumerable of reference assembly locations.</returns>
        private static IEnumerable<string> GetCoreReferences()
        {
            var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => a.Location);

            return refs.Concat(
                typeof(RazorCompiler)
                    .Assembly
                    .GetReferencedAssemblies()
                    .Select(n => Assembly.ReflectionOnlyLoad(n.FullName).Location));
        }

        /// <summary>
        /// Determines if the specified type represents an anonymous type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the specified type represents an anonymous type, otherwise false.</returns>
        internal static bool IsAnonymousType(Type type)
        {
            return (type.IsClass
                    && type.IsSealed
                    && type.BaseType == typeof(object)
                    && type.Name.StartsWith("<>")
                    && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length > 0);
        }

        /// <summary>
        /// Parses the specified template.
        /// </summary>
        /// <param name="template">The string template to parse.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="visitor">The parser visitor.</param>
        private static void ParseTemplate(string template, RazorParser parser, ParserVisitor visitor)
        {
            using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(template))))
            {
                parser.Parse(reader, visitor);
            }
        }
        #endregion
    }
}