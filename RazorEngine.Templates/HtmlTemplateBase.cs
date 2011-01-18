namespace RazorEngine.Templating
{
    using System.IO;
    using System.Web.Mvc;

    using Compilation;

    /// <summary>
    /// Provides a base implementation of a template base that supports <see cref="HtmlHelper{T}"/>s.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    [RequireNamespaces("System.Web.Mvc.Html")]
    public abstract class HtmlTemplateBase<T> : TemplateBase<T>
    {
        #region Fields
        private readonly HtmlHelperFactory factory = new HtmlHelperFactory();
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="HtmlTemplateBase{T}"/>.
        /// </summary>
        protected HtmlTemplateBase()
        {
            CreateHelper(Model);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="HtmlHelper{T}"/> for this template.
        /// </summary>
        public HtmlHelper<T> Html { get; private set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public override T Model
        {
            get { return base.Model; }
            set
            {
                base.Model = value;
                CreateHelper(value);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the required html helper.
        /// </summary>
        private void CreateHelper(T model)
        {
            Html = factory.CreateHtmlHelper(model, new StringWriter(StringBuilder));
        }
        #endregion
    }
}