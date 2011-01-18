namespace RazorEngine.Templating
{
    using System;
    using System.Collections.Generic;
    using System.Web.Razor.Parser;

    using Compilation;

    /// <summary>
    /// Provides template services.
    /// </summary>
    public class TemplateService
    {
        #region Fields
        private readonly IDictionary<string, ITemplate> cache;
        private RazorCompiler compiler;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="TemplateService"/>.
        /// </summary>
        /// <param name="provider">[Optional] The language provider to use.</param>
        /// <param name="templateBaseType">[Optional] The base template type.</param>
        /// <param name="parser">[Optional] The markup parser to use.</param>
        public TemplateService(ILanguageProvider provider = null, Type templateBaseType = null, MarkupParser parser = null)
        {
            cache = new Dictionary<string, ITemplate>();

            provider = provider ?? new CSharpLanguageProvider();
            compiler = new RazorCompiler(provider, templateBaseType, parser);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of namespaces.
        /// </summary>
        public IList<string> Namespaces
        {
            get { return compiler.Namespaces; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Pre-compiles the specified template and caches it using the specified name.
        /// </summary>
        /// <param name="template">The template to precompile.</param>
        /// <param name="name">The cache name for the template.</param>
        public void Compile(string template, string name)
        {
            Compile(template, null, name);
        }

        /// <summary>
        /// Pre-compiles the specified template and caches it using the specified name.
        /// </summary>
        /// <param name="template">The template to precompile.</param>
        /// <param name="modelType">The type of model used in the template.</param>
        /// <param name="name">The cache name for the template.</param>
        public void Compile(string template, Type modelType, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Pre-compiled templates must have a name", "name");

            GetTemplate(template, modelType, name);
        }

        /// <summary>
        /// Pre-compiles the specified template and caches it using the specified name.
        /// This method should be used when an anonymous model is used in the template.
        /// </summary>
        /// <param name="template">The template to precompile.</param>
        /// <param name="name">The cache name for the template.</param>
        public void CompileWithAnonymous(string template, string name)
        {
            Compile(template, new { }.GetType(), name);
        }

        /// <summary>
        /// Gets an <see cref="ITemplate"/> for the specified template.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="modelType">[Optional] The model to use in the template.</param>
        /// <param name="name">[Optional] The name of the template.</param>
        /// <returns></returns>
        internal ITemplate GetTemplate(string template, Type modelType = null, string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (cache.ContainsKey(name))
                    return cache[name];
            }

            var instance = compiler.CreateTemplate(template, modelType);

            if (!string.IsNullOrEmpty(name))
            {
                if (!cache.ContainsKey(name))
                    cache.Add(name, instance);
            }

            return instance;
        }

        /// <summary>
        /// Parses the specified template.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="name">[Optional] The name of the template.</param>
        /// <returns>The parsed template.</returns>
        public string Parse(string template, string name = null)
        {
            var instance = GetTemplate(template, null, name);
            instance.Execute();

            return instance.Result;
        }

        /// <summary>
        /// Parses the specified template.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="model">The model to merge with the template.</param>
        /// <param name="name">[Optional] The name of the template.</param>
        /// <returns>The parsed template.</returns>
        public string Parse<T>(string template, T model, string name = null)
        {
            var instance = GetTemplate(template, typeof(T), name);

            var dynamicInstance = instance as ITemplate<dynamic>;
            if (dynamicInstance != null)
                dynamicInstance.Model = model;

            var typedInstance = instance as ITemplate<T>;
            if (typedInstance != null)
                typedInstance.Model = model;

            instance.Execute();
            return instance.Result;
        }

        /// <summary>
        /// Convert a cshtml template to cs code
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <param name="model">[Optional] Type of the model for the template.</param>
        /// <param name="name">[Optional] The name of the template.</param>
        /// <param name="baseTypeName">[Optional] Base type for the template. To be used if the model type is
        /// in an unreferenced assembly.</param>
        /// <param name="outputNamespace">[Optional] Namespace for the generated class. Default is RazorEngine.Dynamic</param>
        /// <returns>The parsed template in code form.</returns>
        public string ParseToCode(string template, Type modelType = null, string name = null, string baseTypeName = null, string outputNamespace = null)
        {
            return compiler.GetCode(name, template, modelType, baseTypeName, outputNamespace);
        }


        /// <summary>
        /// Runs the cached template with the specified name.
        /// </summary>
        /// <param name="name">The name of the cached template.</param>
        /// <returns>The parsed template.</returns>
        public string Run(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The name of the cached template is required.");

            if (!cache.ContainsKey(name))
                throw new ArgumentException(
                    string.Format("No cached template exists with the name '{0}'", name));

            var instance = cache[name];
            instance.Execute();

            return instance.Result;
        }

        /// <summary>
        /// Runs the cached template with the specified name.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="name">The name of the cached template.</param>
        /// <returns>The parsed template.</returns>
        public string Run<T>(T model, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The name of the cached template is required.");

            if (!cache.ContainsKey(name))
                throw new ArgumentException(
                    string.Format("No cached template exists with the name '{0}'", name));

            var instance = cache[name];

            var dynamicInstance = instance as ITemplate<dynamic>;
            if (dynamicInstance != null)
                dynamicInstance.Model = model;

            var typedInstance = instance as ITemplate<T>;
            if (typedInstance != null)
                typedInstance.Model = model;

            instance.Execute();
            return instance.Result;
        }
        #endregion
    }
}