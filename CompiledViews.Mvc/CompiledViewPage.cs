using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.IO;
using System.Text;
using System.Globalization;
using System.Web.Mvc;

namespace CompiledViews.Mvc
{
    public interface ICompiledViewPage
    {
        void ExecutePage(WebPageContext pageContext, TextWriter writer);
        void ExecuteLayout(ICompiledViewPage page, TextWriter writer);
        void Execute();
        string Layout { get; set; }
        UrlHelper Url { get; set; }
        ViewContext ViewContext { get; set; }
         Dictionary<string, Action> Sections { get; }
         string Body {get;}

         StringBuilder builder { get; }
    }
//    public interface ICompiledViewPage<T> : ICompiledViewPage
//    {
//        HtmlHelper<T> Html { get; set; }
//    }


    public abstract class CompiledViewPage<T> : System.Web.Mvc.WebViewPage<T>, ICompiledViewPage
    {
        public Dictionary<string, Action> Sections { get; set; }
        public string Body { get; set; }

        public StringBuilder builder { get; set; }

        public CompiledViewPage()
        {
            builder = new StringBuilder();
            Sections = new Dictionary<string, Action>();
        }

        public new string Layout { get; set; }

        /// <summary>
        /// Clears the template.
        /// </summary>
        public void Clear()
        {
            builder.Clear();
        }

        
        /// <summary>
        /// Writes the specified object to the template result.
        /// </summary>
        /// <param name="obj">The object to write to the template result.</param>
        public void Write(object obj)
        {
            if (obj == null)
                return;

            builder.Append(obj);
        }

        /// <summary>
        /// Writes the specified literal to the template result.
        /// </summary>
        /// <param name="literal">The literal to write to the template result.</param>
        public void WriteLiteral(string literal)
        {
            if (literal == null)
                return;

            builder.Append(literal);
        }



        public void DefineSection(string s, Action a)
        {
            Sections.Add(s, a);
        }

        public new string RenderBody()
        {
            return Body;
        }

        public new string RenderSection(string name)
        {
            return RenderSection(name, true);
        }
        public new string RenderSection(string name, bool required)
        {
            if (!Sections.ContainsKey(name))
            {
                if (required) throw new Exception(string.Format("Section {0} is required", name));
                else return "";
            }
            else
            {
                
                var tmp = builder.ToString();
                builder.Clear();
                Sections[name]();
                var result = builder.ToString();
                builder.Clear();
                builder.Append(tmp);
                return result;
            }
        }


        public void ExecuteLayout(ICompiledViewPage page, TextWriter writer)
        {
            Sections = page.Sections;
            Body = page.Body;
            this.builder = page.builder;
            Html = new HtmlHelper<T>(ViewContext, this);

            Execute();
            writer.Write(builder.ToString());
        }

        private Type GetViewStartType()
        {      
            var vp = this.VirtualPath;
            while (vp.Contains("/Views/"))
            {
                vp = vp.Substring(0, vp.LastIndexOf("/"));
                var vs = CompiledRazorViewEngine.GetPageType(vp + "/_ViewStart.cshtml");
                if (vs != null) return vs;
            }
            return null;
        }

        private ICompiledViewPage GetViewStart()
        {
            var t = GetViewStartType();
            if (t != null) return (ICompiledViewPage)t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            else return null;
        }

        private dynamic GetLayout()
        {
            if (string.IsNullOrEmpty(Layout)) return null;
            var t = CompiledRazorViewEngine.GetPageType(Layout);
            if (t != null) return t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            else return null;
        }


        public void ExecutePage(WebPageContext pageContext, TextWriter writer)
        {
            Sections = new Dictionary<string, Action>();
            builder = new StringBuilder();

            var sw = new StringWriter(builder);
            ViewContext.Writer = sw;
            Html = new HtmlHelper<T>(ViewContext, this);


            // execute viewstart and copy property values
            var viewstart = GetViewStart();
            if (viewstart != null)
            {
                viewstart.Execute();
                Layout = viewstart.Layout;
            }

            // run execute to populate sections and body
            Execute();            
            // builder now contains the body

            Body = builder.ToString();
            builder.Clear();

            var layout = GetLayout();
            if (layout != null && (object)layout != this)
            {
                layout.Url = Url;
                //layout.ViewBag = ViewBag;
                layout.ViewContext = ViewContext;
                // layout = null;

                // if we have a layout page, execute it, passing sections, body and the writer
                layout.ExecuteLayout(this, writer);
            }
            else
            {
                // otherwise, this is the start page so just write the body
                writer.Write(Body);
            }

        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
 


    }
}