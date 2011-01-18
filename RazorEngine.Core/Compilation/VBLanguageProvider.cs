namespace RazorEngine.Compilation
{
    using System.CodeDom.Compiler;
    using System.Linq;
    using System.Web.Razor;

    using Microsoft.VisualBasic;

    /// <summary>
    /// Defines a language provider that supports Visual Basic syntax.
    /// </summary>
    public class VBLanguageProvider : ILanguageProvider
    {
        #region Methods
        /// <summary>
        /// Creates a code language service.
        /// </summary>
        /// <returns>Creates a language service.</returns>
        public RazorCodeLanguage CreateLanguageService()
        {
            return new VBRazorCodeLanguage();
        }

        /// <summary>
        /// Creates a <see cref="CodeDomProvider"/>.
        /// </summary>
        /// <returns>The code DOM provider.</returns>
        public CodeDomProvider CreateCodeDomProvider()
        {
            return new VBCodeProvider();
        }

        /// <summary>
        /// Generates a class name for the Template base
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>This is probably not the right location to put this but it seemed the most logical choice</remarks>
        public string GenerateClassName(System.Type type) {
            if (!type.IsGenericType)
                return type.Namespace + "." + type.Name;

            return type.Namespace
                   + "."
                   + type.Name.Substring(0, type.Name.IndexOf('`'))
                   + "(Of "
                   + string.Join(", ", type.GetGenericArguments()
                                           .Select(GenerateClassName))
                   + ")";
        }

        #endregion
    }
}