using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CompiledViews.SharePoint
{
    /// <summary>
    /// Clears the response and sends a json response with an optional http status code
    /// </summary>
    public class JsonResult : ActionResult
    {
        private Control ParentControl;
        private string Message;
        private int StatusCode;

        public JsonResult(Control parent, string message, int status)
        {
            ParentControl = parent;
            Message = message;
            StatusCode = status;
        }

        public override void Execute()
        {
            var resp = ParentControl.Page.Response;
            resp.Clear();
            resp.ContentType = "text/json";
            resp.StatusCode = StatusCode;
            resp.Write(Message);
            resp.End();
        }
    }

}
