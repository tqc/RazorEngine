using System.IO;
using System.Web.Mvc;

namespace RazorEngine.Templating
{
    /// <summary>
    /// Provides factory methods for creating <see cref="HtmlHelper{T}"/> instances.
    /// </summary>
    internal class HtmlHelperFactory
    {
        #region Methods
        /// <summary>
        /// Creates a <see cref="HtmlHelper{T}"/> for the specified model.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="model">The model to create a helper for.</param>
        /// <param name="writer">The writer used to output html.</param>
        /// <returns>An instance of <see cref="HtmlHelper{T}"/>.</returns>
        public HtmlHelper<T> CreateHtmlHelper<T>(T model, TextWriter writer)
        {
            var container = new InternalViewDataContainer<T>(model);
            var context = new ViewContext(
                new ControllerContext(),
                new InternalView(),
                container.ViewData,
                new TempDataDictionary(),
                writer);

            return new HtmlHelper<T>(context, container);
        }
        #endregion

        #region Types
        /// <summary>
        /// Defines an internal view.
        /// </summary>
        private class InternalView : IView
        {
            #region Methods
            /// <summary>
            /// Renders the contents of the view to the specified writer.
            /// </summary>
            /// <param name="context">The current View context.</param>
            /// <param name="writer">The writer used to generate the output.</param>
            public void Render(ViewContext context, TextWriter writer) { }
            #endregion
        }

        /// <summary>
        /// Defines an internal view data container.
        /// </summary>
        private class InternalViewDataContainer<T> : IViewDataContainer
        {
            #region Methods
            /// <summary>
            /// Initialises a new instance of <see cref="InternalViewDataContainer{T}"/>.
            /// </summary>
            public InternalViewDataContainer(T model)
            {
                ViewData = new ViewDataDictionary<T>(model);
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the view data dictionary.
            /// </summary>
            public ViewDataDictionary ViewData { get; set; }
            #endregion
        }
        #endregion
    }
}
