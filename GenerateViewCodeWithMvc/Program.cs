﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor;
using System.IO;
using System.Web.WebPages.Razor;
using Microsoft.CSharp;

namespace GenerateViewCodeWithMvc
{
    class Program
    {
        static void Main(string[] args)
        {
            string templatebasename = "RazorEngine.Templating.TemplateBase";
            var namespaces = new List<string>();
            if (args.Length > 0)
            {
                templatebasename = args[0];
            }
            for (int i = 1; i < args.Length; i++ )
            {
                namespaces.Add(args[i]);
            }

            var filecount = 0;

            // for each .cshtml file under the working directory, generate a .cs file if it has changed.
            foreach (var templatepath in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.cshtml", SearchOption.AllDirectories))
            {
                FileInfo fitemplate = new FileInfo(templatepath);
                FileInfo ficode = new FileInfo(templatepath + ".cs");
                if (!ficode.Exists || ficode.LastWriteTimeUtc < fitemplate.LastWriteTimeUtc)
                {
                    // get classname from path
                    var cn = fitemplate.Name.Substring(0, fitemplate.Name.IndexOf('.'));
                    var pt = fitemplate.DirectoryName.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                    var ns = pt[pt.Length - 1];
                    for (var i = pt.Length - 2; i > 0; i--)
                    {
                        ns = pt[i] + "." + ns;
                        if (pt[i + 1] == "Views") break;
                    }

                    var prp = fitemplate.FullName.Substring(fitemplate.FullName.IndexOf(@"\Views\")).Replace('\\', '/');

                    string template =
                         File.ReadAllText(fitemplate.FullName);
                    var host =
                        new WebPageRazorHost("~" + prp, fitemplate.FullName);
                    //new RazorEngineHost(new CSharpRazorCodeLanguage());
                    //new WebCodeRazorHost("~"+prp, fitemplate.FullName);
                    var rte = new RazorTemplateEngine(host);


                    //Razor.SetTemplateBaseType(typeof(TemplateBase<>));

                    string baseTypeName = templatebasename;

                    if (template.StartsWith("@model"))
                    {
                        var l1 = template.IndexOf("\n");
                        var modelTypeName = template.Substring(6, l1 - 6).Trim();
                        template = template.Substring(l1).Trim();
                        baseTypeName = templatebasename + "<" + modelTypeName + ">";
                    }
                    //else if (cn == "_ViewStart")
                    //{
                    //    baseTypeName = "System.Web.WebPages.StartPage";
                    //}
                    else
                    {
                        baseTypeName = templatebasename + "<dynamic>";
                    }

                    //host.DefaultNamespace = "";

                    host.DefaultPageBaseClass = baseTypeName;

                    host.NamespaceImports.Add("System.Web.Mvc");
                    host.NamespaceImports.Add("System.Web.Mvc.Html");

                    //string result = 
                    //Razor.ParseToCode(template, null, cn, baseTypeName, ns);


                    GeneratorResults results = null;
                    using (var reader = new StringReader(template))
                    {
                        results = rte.GenerateCode(reader, cn, ns, null);
                    }
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine("using System.Web.Mvc;");
                    builder.AppendLine("using System.Web.Mvc.Html;");
                    builder.AppendLine("using System.Web.Mvc.Ajax;");

                    foreach (var v in namespaces)
                    {
                        builder.AppendLine("using "+v+";");
                    }



                    using (var writer = new StringWriter(builder))
                    {
                        new CSharpCodeProvider().GenerateCodeFromCompileUnit(results.GeneratedCode, writer, null);
                    }
                    builder.Replace("#line hidden", "#line 1 \"" + fitemplate.Name + "\"");
                    File.WriteAllText(ficode.FullName, builder.ToString());
                    Console.WriteLine("Updated {0}.{1}", ns, cn);
                    filecount++;
                }
            }
            Console.WriteLine("Done - updated {0} files", filecount);

            Console.ReadLine();
        }


    }
}
