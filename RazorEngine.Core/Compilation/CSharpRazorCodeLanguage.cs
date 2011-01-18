namespace RazorEngine.Compilation
{
    using System.Web.Razor;
    using System.Web.Razor.Generator;

    /// <summary>
    /// Defines a razor code language that supports
    /// </summary>
    public class CSharpRazorCodeLanguage : System.Web.Razor.CSharpRazorCodeLanguage
    {
        #region Methods
        /// <summary>
        /// Creates a code generator.
        /// </summary>
        /// <param name="className">The name of the generated class.</param>
        /// <param name="rootNamespaceName">The namespace of the generated class.</param>
        /// <param name="sourceFileName">The source file filename.</param>
        /// <param name="host">The <see cref="RazorEngineHost"/> instance.</param>
        /// <returns>A new instance of <see cref="RazorCodeGenerator"/>.</returns>
        public override RazorCodeGenerator CreateCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
        {
            return new CSharpRazorCodeGenerator(className, rootNamespaceName, sourceFileName, host);
        }
        #endregion
    }
}
