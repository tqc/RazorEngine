using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using RazorEngine.Templating;



namespace CompiledViews.SharePoint
{
    /// <summary>
    /// Base class for web parts that display a Razor view
    /// </summary>
    public abstract class MvcWebPart : WebPart
    {
        private bool _error = false;


        /// <summary>
        /// If true, only render child controls instead of standard web part rendering.
        /// </summary>
        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [System.ComponentModel.Category("My Property Group")]
        [WebDisplayName("Use Simple Rendering")]
        [WebDescription("If true, the web part chrome will not render. Intended for when the webpart is used as a control")]
        public bool UseSimpleRendering { get; set; }


        protected MvcWebPart()
        {
            this.ExportMode = WebPartExportMode.All;
        }



        /// <summary>
        /// Get a view result object
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="view">Instance of the compiled view class</param>
        /// <param name="model">Model object</param>
        /// <returns></returns>
        protected ViewResult<T> View<T>(TemplateBase<T> view, T model)
        {
            return new ViewResult<T>(this, view, model);
        }

        /// <summary>
        /// Get a text result with a specific http status code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statuscode"></param>
        /// <returns></returns>
        protected TextResult Text(string message, int statuscode)
        {
            return new TextResult(this, message, statuscode);
        }

        /// <summary>
        /// Get a text result with http status OK
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected TextResult Text(string message)
        {
            return new TextResult(this, message, 200);
        }

        /// <summary>
        /// Get a Json result with a specific http status code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statuscode"></param>
        /// <returns></returns>
        protected JsonResult Json(string message, int statuscode)
        {
            return new JsonResult(this, message, statuscode);
        }

        /// <summary>
        /// Get a Json result with http status OK
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected JsonResult Json(string message)
        {
            return new JsonResult(this, message, 200);
        }



        /// <summary>
        /// This method is called on a get request
        /// </summary>
        /// <returns></returns>
        protected abstract ActionResult Get();

        /// <summary>
        /// This method is called on a post request. If not overridden, it calls Get(). 
        /// Note that any implementation will need to handle the possibility that it is called because
        /// another component on the page is posting data and should not actually try to update anything.
        /// </summary>
        /// <returns></returns>
        protected virtual ActionResult Post()
        {
            return Get();
        }


        protected override void CreateChildControls()
        {
            if (!_error)
            {
                try
                {
                    base.CreateChildControls();
                    var r = Page.Request.HttpMethod == "POST" ? Post() : Get();
                    r.Execute();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!_error)
            {
                try
                {
                    base.OnLoad(e);
                    this.EnsureChildControls();

                    // Your code here...

                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }


        public override void RenderControl(HtmlTextWriter writer)
        {
            if (UseSimpleRendering)
            {
                foreach (Control c in Controls)
                {
                    c.RenderControl(writer);
                }
            }
            else
            {
                base.RenderControl(writer);
            }
        }


        /// <summary>
        /// Clear all child controls and add an error message for display.
        /// </summary>
        /// <param name="ex"></param>
        private void HandleException(Exception ex)
        {
            this._error = true;
            this.Controls.Clear();
            this.Controls.Add(new LiteralControl(ex.Message));
        }

    }

  
  

}

