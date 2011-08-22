using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using RazorEngine.Templating;

namespace CompiledViews.SharePoint
{
    
    /// <summary>
    /// Adds a string to the parent as the content of a LiteralControl
    /// </summary>
    public class ContentResult : ActionResult
    {
        private string Content;
        private MvcWebPart ParentControl;

        protected ContentResult() { }


        public ContentResult(MvcWebPart parent, string content)
        {
            ParentControl = parent;
            Content = content;
        }

        public override void Execute()
        {
            ParentControl.ViewContent.Text=Content;
        }
    }
}
