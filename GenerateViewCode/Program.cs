using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace GenerateViewCode
{
    class Program
    {
        static void Main(string[] args)
        {

            string template =
                 File.ReadAllText(@"views\shared\DocView.cshtml");
            Razor.SetTemplateBaseType(typeof(TemplateBase<>));
            string result = Razor.ParseToCs(template, typeof(TW.PDF.DocSection), "DocView");

            File.WriteAllText(@"views\shared\DocView.cshtml.cs", result);

        }
    }
}
