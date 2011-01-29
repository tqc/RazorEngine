using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.CSharp;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio.OLE.Interop;
using System.Web.Razor;
using System.Web.WebPages.Razor;
using System.Runtime.InteropServices;

namespace ViewCodeSingleFileGenerator
{
    [Guid("1111B3C2-C6FA-4325-A773-DE53801DBFE5")]
    public class ViewCodeFileGenerator : IVsSingleFileGenerator
    {

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cs";
            return pbstrDefaultExtension.Length;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            string templatebasename = "RazorEngine.Templating.TemplateBase";
            FileInfo fitemplate = new FileInfo(wszInputFilePath);
            FileInfo ficode = new FileInfo(wszInputFilePath.Replace(".cshtml", ".cs"));

            if (!ficode.Exists || ficode.LastWriteTimeUtc < fitemplate.LastWriteTimeUtc)
            {
                // get classname from path
                var cn = fitemplate.Name.Substring(0, fitemplate.Name.IndexOf('.'));
                // var pt = fitemplate.DirectoryName.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                var ns = wszDefaultNamespace;

                string template = File.ReadAllText(fitemplate.FullName);
                var host = new WebPageRazorHost(wszInputFilePath);
                //new WebPageRazorHost("~" + , fitemplate.FullName);
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
                //builder.AppendLine("using System.Web.Mvc;");
                //builder.AppendLine("using System.Web.Mvc.Html;");


                using (var writer = new StringWriter(builder))
                {
                    new CSharpCodeProvider().GenerateCodeFromCompileUnit(results.GeneratedCode, writer, null);
                }
                builder.Replace("#line hidden", "#line 1 \"" + fitemplate.Name + "\"");
                File.WriteAllText(ficode.FullName, builder.ToString());
                Console.WriteLine("Updated {0}.{1}", ns, cn);

                byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
                int length = bytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);

                pcbOutput = (uint)length;
                return VSConstants.S_OK;
            }
            else
            {
                rgbOutputFileContents = new IntPtr[] { };
                pcbOutput = 0;
                return 0;
            }
        }
    }
}