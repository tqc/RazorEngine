using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace CompiledViews.SharePoint
{
    /// <summary>
    /// Clears the response and sends a text response with an optional http status code
    /// </summary>
    public class HtmlResult : ActionResult
    {
        private Control ParentControl;
        private string Message;
        private int StatusCode;

        public HtmlResult(Control parent, string message, int status)
        {
            ParentControl = parent;
            Message = message;
            StatusCode = status;
        }

        public override void Execute()
        {
            var resp = HttpContext.Current.Response;
            resp.Clear();
            resp.ContentType = "text/html";
            resp.StatusCode = StatusCode;
            resp.Write(Message);
            resp.End();
        }
    }

}
