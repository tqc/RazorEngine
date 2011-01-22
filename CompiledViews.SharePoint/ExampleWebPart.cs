using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestViewLibrary.Model;
using TestViewLibrary.Views.Shared;

namespace CompiledViews.SharePoint
{
    public class ExampleWebPart : MvcWebPart
    {
        protected override ActionResult Get()
        {
            var m = new DocSection()
            {
                Title = "Test Model"
            };
            return View(new DocView(), m);
        }
    }

}
