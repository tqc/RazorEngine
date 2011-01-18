using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System.Reflection;

namespace GenerateViewCode
{
    class Program
    {
        static void Main(string[] args)
        {

            string template =
                 File.ReadAllText(@"views\shared\DocView.cshtml");
            Razor.SetTemplateBaseType(typeof(TemplateBase<>));
            

            string result = Razor.ParseToCode(template, typeof(TestViewLibrary.Model.DocSection), "DocView");

            File.WriteAllText(@"views\shared\DocView.cshtml.cs", result);

        }
    }

    
}

namespace TestViewLibrary.Model
{
    class DocSection { }
}

