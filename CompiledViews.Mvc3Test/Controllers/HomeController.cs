using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompiledViews.Mvc3Test.Models;

namespace CompiledViews.Mvc3Test.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var m = new HomeModel()
            {
                TestProperty = "Test Value"
            };
            return View(m);
        }
        public ActionResult Index2()
        {
            var m = new HomeModel()
            {
                TestProperty = "Test Value"
            };
            return View(m);
        }

    }
}
