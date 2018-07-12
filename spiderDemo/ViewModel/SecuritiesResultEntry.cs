using spiderDemo.Extension;
using spiderDemo.Extension.Model;
using spiderDemo.Extension.Model.Attibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.ViewModel
{

    public class SecuritiesResultEntry
    {
        public string Id { get; set; }
        public string Keyword { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string ReleaseDate { get; set; }

        public string SourceUri { get; set; }

        public string FileUri { get; set; }

        public int FileCount { get; set; }

        public string Content { get; set; }

        public string SourceWebsite { get; set; }

    }
}
