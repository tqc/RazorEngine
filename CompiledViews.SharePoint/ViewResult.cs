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
    public class ViewResult<T> : ActionResult
    {
        private TemplateBase<T> View;
        private T Model;
        private MvcWebPart ParentControl;

        protected ViewResult() { }


        public ViewResult(MvcWebPart parent, TemplateBase<T> view, T model)
        {
            ParentControl = parent;
            View = view;
            Model = model;
        }

        public override void Execute()
        {
            View.Model = Model;
            View.Execute();
            ParentControl.ViewContent.Text=View.Result;
        }
    }
}
