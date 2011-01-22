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
    public abstract class MvcWebPart : WebPart
    {
        private bool _error = false;

        protected MvcWebPart()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        protected abstract class ActionResult
        {
            public abstract void Execute();
        }

        protected class ViewResult<T> : ActionResult
        {
            private TemplateBase<T> View;
            private T Model;
            private Control ParentControl;

            public ViewResult(Control parent, TemplateBase<T> view, T model)
            {
                ParentControl = parent;
                View = view;
                Model = model;
            }

            public override void Execute()
            {
                View.Model = Model;
                View.Execute();
                ParentControl.Controls.Add(new LiteralControl(View.Result));
            }
        }


        protected ViewResult<T> View<T>(TemplateBase<T> view, T model)
        {
            return new ViewResult<T>(this, view, model);
        }


        protected abstract ActionResult Get();

        /// <summary>        
        /// Create all your controls here for rendering.
        /// Try to avoid using the RenderWebPart() method.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!_error)
            {
                try
                {
                    base.CreateChildControls();

                    // TODO: handle post/other actions also
                    var r = Get();
                    r.Execute();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }


        /// <summary>
        /// Ensures that the CreateChildControls() is called before events.
        /// Use CreateChildControls() to create your controls.
        /// </summary>
        /// <param name="e"></param>
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

