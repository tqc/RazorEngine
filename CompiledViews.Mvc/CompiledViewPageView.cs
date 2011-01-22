using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CompiledViews.Mvc
{
    public class CompiledViewPageView : IView
    {
        private WebViewPage page;
        // Methods

        internal CompiledViewPageView(ControllerContext controllerContext,
            string viewPath,
            WebViewPage viewpage)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(viewPath))
            {
                throw new ArgumentException("viewPath is null or empty");
            }

            this._controllerContext = controllerContext;
            this.ViewPath = viewPath;
            this.page = viewpage;
        }

        public string ViewPath { get; set; }

        private ControllerContext _controllerContext;

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }
            Type compiledType = page.GetType();

            if (page == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "View could not be created - {0}", new object[] { this.ViewPath }));
            }
            this.RenderView(viewContext, writer, page);
        }

        protected void RenderView(ViewContext viewContext, TextWriter writer, object instance)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WebViewPage page = instance as WebViewPage;
            if (page == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Wrong view base - {0}", new object[] { ViewPath }));
            }
            // page.OverridenLayoutPath = "";// this.LayoutPath;
            page.VirtualPath = ViewPath;
            page.ViewContext = viewContext;
            page.ViewData = viewContext.ViewData;
            page.InitHelpers();
            HttpContextBase httpContext = viewContext.HttpContext;
            WebPageRenderingBase base4 = null;
            object model = null;
            ((ICompiledViewPage)page).ExecutePage(new WebPageContext(httpContext, base4, model), writer);

        }
    }
}
