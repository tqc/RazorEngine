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
            string templatebasename = "RazorEngine.Templating.TemplateBase";
            if (args.Length > 0)
            {
                templatebasename = args[0];
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
                    var pt = fitemplate.DirectoryName.Split(new char[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
                    var ns = pt[pt.Length - 1];
                    for (var i = pt.Length - 2; i > 0; i--)
                    {
                        ns = pt[i]+"."+ns;
                        if (pt[i + 1] == "Views") break;
                    }

                    

                    string template =
                         File.ReadAllText(fitemplate.FullName);
                    Razor.SetTemplateBaseType(typeof(TemplateBase<>));

                    string baseTypeName = templatebasename;
                    if (template.StartsWith("@model"))
                    {
                        var l1 = template.IndexOf("\n");
                        var modelTypeName = template.Substring(6, l1 - 6).Trim();
                        template = template.Substring(l1).Trim();
                        baseTypeName = templatebasename+"<" + modelTypeName + ">";
                    }

                    string result = Razor.ParseToCode(template, null, cn, baseTypeName, ns);

                    File.WriteAllText(ficode.FullName, result);
                    Console.WriteLine("Updated {0}.{1}", ns, cn);
                    filecount++;
                }
            }
            Console.WriteLine("Done - updated {0} files", filecount);
        }
    }

    
}


