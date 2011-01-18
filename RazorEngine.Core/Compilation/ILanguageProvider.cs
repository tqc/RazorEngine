namespace RazorEngine.Compilation
{
    using System;
    using System.CodeDom.Compiler;
    using System.Web.Razor;

    /// <summary>
    /// Defines the required contract for implementing a language provider.
    /// </summary>
    public interface ILanguageProvider
    {
        #region Methods
        /// <summary>
        /// Creates a code language service.
        /// </summary>
        /// <returns>Creates a language service.</returns>
        RazorCodeLanguage CreateLanguageService();

        /// <summary>
        /// Creates a <see cref="CodeDomProvider"/>.
        /// </summary>
        /// <returns>The code DOM provider.</returns>
        CodeDomProvider CreateCodeDomProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GenerateClassName(Type type);
        #endregion
    }
}