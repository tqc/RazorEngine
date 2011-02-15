using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CompiledViews.SharePoint
{
    /// <summary>
    /// Clears the response and sends a text response with an optional http status code
    /// </summary>
    public class TextResult : ActionResult
    {
        private Control ParentControl;
        private string Message;
        private int StatusCode;

        public TextResult(Control parent, string message, int status)
        {
            ParentControl = parent;
            Message = message;
            StatusCode = status;
        }

        public override void Execute()
        {
            var resp = ParentControl.Page.Response;
            resp.Clear();
            resp.ContentType = "text/plain";
            resp.StatusCode = StatusCode;
            resp.Write(Message);
            resp.End();
        }
    }

}
