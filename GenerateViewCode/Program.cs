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

            string baseTypeName = null;
            if (template.StartsWith("@model"))
            {
                var l1 = template.IndexOf("\n");
                var modelTypeName = template.Substring(6, l1-6).Trim();
                template = template.Substring(l1).Trim();
                baseTypeName = "RazorEngine.Templating.TemplateBase<" + modelTypeName + ">";

            }

            string result = Razor.ParseToCode(template, null, "DocView", baseTypeName);

            File.WriteAllText(@"views\shared\DocView.cshtml.cs", result);

        }
    }

    
}


