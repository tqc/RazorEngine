namespace RazorEngine.Compilation
{
    using System.Web.Razor;
    using System.Web.Razor.Parser.SyntaxTree;

    using Templating;

    /// <summary>
    /// Defines a code generator that supports C# syntax.
    /// </summary>
    public class CSharpRazorCodeGenerator : System.Web.Razor.Generator.CSharpRazorCodeGenerator
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="CSharpRazorCodeGenerator" />.
        /// </summary>
        /// <param name="className">The name of the generated class.</param>
        /// <param name="rootNamespaceName">The namespace of the generated class.</param>
        /// <param name="sourceFileName">The source file filename.</param>
        /// <param name="host">The <see cref="RazorEngineHost"/> instance.</param>
        public CSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host) { }
        #endregion

        #region Methods
        /// <summary>
        /// Visits an error generated through parsing.
        /// </summary>
        /// <param name="err">The error that was generated.</param>
        public override void VisitError(RazorError err)
        {
            throw new TemplateParsingException(err);
        }
        #endregion
    }
}