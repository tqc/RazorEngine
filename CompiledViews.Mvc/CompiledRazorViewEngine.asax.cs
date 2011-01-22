using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CompiledViews.Mvc
{
	/// <summary>
	/// Allow to embed precompiled razorviews (or any other webviewpage)
	/// Things that would be nice to have in Microsoft's code:
	///     - public access to VirtualPathFactoryManager.Instance
	///     - public access to VirtualPathFactoryManager.PageExists
	///		- let either store IVirtualPathFactory just the Type to construct instead of a constructor,
	///			or: let DictionaryBasedVirtualPathFactory use IoC if it's registered
	///		- allow to override BuildManager implementations, 
	///			that way we could hook into the framework on that level, instead of creating an extra viewengine
	/// </summary>
	public class CompiledRazorViewEngine : VirtualPathProviderViewEngine
	{
		internal static readonly string ViewStartFileName = "_ViewStart";

        private static Dictionary<string, Type> PageTypes { get; set; }

        static CompiledRazorViewEngine()
        {
            PageTypes = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Find all views in the assembly and map them to virtual paths. Views with the same virtual path
        /// will overwrite existing views to allow an individual project to override views from a common library.
        /// </summary>
        /// <param name="a"></param>
        public static void RegisterViewAssembly(Assembly a)
        {
            foreach (var t in a.GetTypes())
            {
                if (typeof(ICompiledViewPage).IsAssignableFrom(t) && t.FullName.Contains(".Views."))
                {
                    var vp = "~" + t.FullName.Substring(t.FullName.IndexOf(".Views.")).Replace(".","/") + ".cshtml";
                    PageTypes[vp] = t;
                }
            }
        }

        /// <summary>
        /// Get the type of the viewpage class for a virtual path. Returns null if the view page does not exist
        /// </summary>
        /// <param name="VirtualPath"></param>
        /// <returns></returns>
        public static Type GetPageType(string virtualPath) {
            if (!PageTypes.ContainsKey(virtualPath)) return null;
            return PageTypes[virtualPath];
        }

		public CompiledRazorViewEngine()
		{
			AreaViewLocationFormats = new[] {
			                                	"~/Areas/{2}/Views/{1}/{0}.cshtml",
			                                	"~/Areas/{2}/Views/{1}/{0}.vbhtml",
			                                	"~/Areas/{2}/Views/Shared/{0}.cshtml",
			                                	"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			                                };
			AreaMasterLocationFormats = new[] {
			                                  	"~/Areas/{2}/Views/{1}/{0}.cshtml",
			                                  	"~/Areas/{2}/Views/{1}/{0}.vbhtml",
			                                  	"~/Areas/{2}/Views/Shared/{0}.cshtml",
			                                  	"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			                                  };
			AreaPartialViewLocationFormats = new[] {
			                                       	"~/Areas/{2}/Views/{1}/{0}.cshtml",
			                                       	"~/Areas/{2}/Views/{1}/{0}.vbhtml",
			                                       	"~/Areas/{2}/Views/Shared/{0}.cshtml",
			                                       	"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			                                       };

			ViewLocationFormats = new[] {
			                            	"~/Views/{1}/{0}.cshtml",
			                            	"~/Views/{1}/{0}.vbhtml",
			                            	"~/Views/Shared/{0}.cshtml",
			                            	"~/Views/Shared/{0}.vbhtml"
			                            };
			MasterLocationFormats = new[] {
			                              	"~/Views/{1}/{0}.cshtml",
			                              	"~/Views/{1}/{0}.vbhtml",
			                              	"~/Views/Shared/{0}.cshtml",
			                              	"~/Views/Shared/{0}.vbhtml"
			                              };
			PartialViewLocationFormats = new[] {
			                                   	"~/Views/{1}/{0}.cshtml",
			                                   	"~/Views/{1}/{0}.vbhtml",
			                                   	"~/Views/Shared/{0}.cshtml",
			                                   	"~/Views/Shared/{0}.vbhtml"
			                                   };

		}

		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			return CreateView(controllerContext, partialPath, null);
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
            if (!PageTypes.ContainsKey(viewPath))
            {
                throw new Exception("Could not find type for " + viewPath);
            }
            var page = (WebViewPage)PageTypes[viewPath].GetConstructor(new Type[] { }).Invoke(new object[] { });        
            return new CompiledViewPageView(controllerContext, viewPath, page);
        }

		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
            return PageTypes.ContainsKey(virtualPath) || base.FileExists(controllerContext, virtualPath);
		}

	}
}