namespace RazorEngine.Templating
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// Provides a base implementation of a template.
    /// </summary>
    public abstract class TemplateBase : ITemplate
    {
        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="TemplateBase"/>.
        /// </summary>
        protected TemplateBase()
        {
            StringBuilder = new StringBuilder();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result of the template.
        /// </summary>
        public string Result { get { return StringBuilder.ToString(); } }

        /// <summary>
        /// Gets the string builder used to write the result of the template.
        /// </summary>
        protected StringBuilder StringBuilder { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Clears the template.
        /// </summary>
        public void Clear()
        {
            StringBuilder.Clear();
        }

        /// <summary>
        /// Executes the compiled template.
        /// </summary>
        public virtual void Execute() { }

        /// <summary>
        /// Writes the specified object to the template result.
        /// </summary>
        /// <param name="obj">The object to write to the template result.</param>
        public void Write(object obj)
        {
            if (obj == null) 
                return;

            StringBuilder.Append(obj);
        }

        /// <summary>
        /// Writes the specified literal to the template result.
        /// </summary>
        /// <param name="literal">The literal to write to the template result.</param>
        public void WriteLiteral(string literal)
        {
            if (literal == null)
                return;

            StringBuilder.Append(literal);
        }

        /// <summary>
        /// Writes a string literal to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="literal">The literal to be written.</param>
        public static void WriteLiteralTo(TextWriter writer, string literal)
        {
            if (literal == null)
                return;

            writer.Write(literal);
        }

        /// <summary>
        /// Writes the specified object to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="obj">The object to be written.</param>
        public static void WriteTo(TextWriter writer, object obj)
        {
            if (obj == null)
                return;

            writer.Write(obj);
        }
        #endregion
    }
}
