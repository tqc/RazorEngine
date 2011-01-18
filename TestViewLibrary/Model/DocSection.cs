using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TW.PDF
{
    public class DocSection
    {
        public string SectionNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<DocSection> SubSections { get; set; }
    }
}
