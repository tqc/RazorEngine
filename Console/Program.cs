namespace Console
{
    using cs = System.Console;
    using System.Collections.Generic;


    using RazorEngine;
    using RazorEngine.Templating;

    class Program
    {
        static void Main(string[] args)
        {
            string template =
@"@using System.Collections.Generic
@helper Title(string title) {
    <title>@title</title>
}
@helper DisplayItems(IEnumerable<string> items) {
    @if(items != null) {
        @foreach(string item in items) {
            <li>@item</li>
        }
    }
}
<html>
    <head>
        @Title(Model.Title)
    </head>
    <body>
        <p>Enter your name: @Html.TextBoxFor(m => m.Name)</p>
        <ul>
            @DisplayItems(Model.Items)
        </ul>
    </body>
</html>";

            Razor.SetTemplateBaseType(typeof(HtmlTemplateBase<>));
            var model = new PageModel
                        {
                            Title = "Test",
                            Name = "Matt",
                            Items = new List<string>
                                    {
                                        "One", "Two", "Three", "Four"
                                    }

                        };

            string result = Razor.Parse(template, model);
            cs.WriteLine(result);

            cs.ReadKey();
        }
    }

    public class PageModel
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public IList<string> Items { get; set; }
    }
}