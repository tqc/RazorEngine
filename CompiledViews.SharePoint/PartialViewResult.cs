using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using RazorEngine.Templating;

namespace CompiledViews.SharePoint
{
    
    /// <summary>
    /// Renders a compiled Razor view and adds it to the parent as the content of a LiteralControl
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PartialViewResult<T> : ActionResult
    {
        private TemplateBase<T> View;
        private T Model;
        private MvcWebPart ParentControl;

        public PartialViewResult(MvcWebPart parent, TemplateBase<T> view, T model)
        {
            ParentControl = parent;
            View = view;
            Model = model;
        }

        public override void Execute()
        {

            View.Model = Model;
            View.Execute();

            var resp = ParentControl.Page.Response;
            resp.Clear();
            resp.ContentType = "text/html";
            resp.StatusCode = 200;
            resp.Write(View.Result);
            resp.End();
        }
    }
}
