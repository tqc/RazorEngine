namespace RazorEngine.Compilation
{
    using System.Web.Razor;
    using System.Web.Razor.Parser.SyntaxTree;

    using Templating;

    /// <summary>
    /// Defines a code generator that supports VB syntax.
    /// </summary>
    public class VBRazorCodeGenerator : System.Web.Razor.Generator.VBRazorCodeGenerator
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="VBRazorCodeGenerator" />.
        /// </summary>
        /// <param name="className">The name of the generated class.</param>
        /// <param name="rootNamespaceName">The namespace of the generated class.</param>
        /// <param name="sourceFileName">The source file filename.</param>
        /// <param name="host">The <see cref="RazorEngineHost"/> instance.</param>
        public VBRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
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
