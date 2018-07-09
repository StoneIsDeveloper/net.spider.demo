using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.ViewModel
{
    public class NewsViewModel
    {
        public string Id { get; set; }
        public string[] Author { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string docreader { get; set; }
        public string medianame { get; set; }
        public string newsid { get; set; }
        public string[] reletecodes { get; set; }
        public string relkey { get; set; }
        public string url { get; set; }
        public string text { get; set; }
    }

    public class Suammry
    {
        public string total { get; set; }
        public List<News> NewsList { get; set; }
    }

    public class News
    {
        public string Id { get; set; }

        public string[] Author { get; set; }

        public string Date { get; set; }

        public string From { get; set; }

        public string Title { get; set; }

        public string url { get; set; }

        public string medianame { get; set; }
    }
}
