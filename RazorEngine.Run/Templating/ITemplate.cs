namespace RazorEngine.Templating
{
    /// <summary>
    /// Defines the required contract for implementing a template.
    /// </summary>
    public interface ITemplate
    {
        #region Properties
        /// <summary>
        /// Gets the result of the template.
        /// </summary>
        string Result { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Clears the template.
        /// </summary>
        void Clear();

        /// <summary>
        /// Executes the compiled template.
        /// </summary>
        void Execute();
        
        /// <summary>
        /// Writes the specified object to the template result.
        /// </summary>
        /// <param name="obj">The object to write to the template result.</param>
        void Write(object obj);

        /// <summary>
        /// Writes the specified literal to the template result.
        /// </summary>
        /// <param name="literal">The literal to write to the template result.</param>
        void WriteLiteral(string literal);
        #endregion
    }
}