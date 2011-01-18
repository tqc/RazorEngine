namespace RazorEngine.Templating
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Defines an exception that occurs during compilation of a template.
    /// </summary>
    public class TemplateException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initialises a new instance of <see cref="TemplateException"/>
        /// </summary>
        /// <param name="errors">The collection of compilation errors.</param>
        internal TemplateException(CompilerErrorCollection errors)
            : base("Unable to compile template.")
        {
            var list = errors.Cast<CompilerError>().ToList();
            Errors = new ReadOnlyCollection<CompilerError>(list);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of compiler errors.
        /// </summary>
        public ReadOnlyCollection<CompilerError> Errors { get; private set; }
        #endregion
    }
}