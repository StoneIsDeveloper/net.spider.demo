using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.ViewModel
{

    public class ShenReportList
    {
        public string totalSize { get; set; } 
        public List<ShenReportItem> data { get; set; }
    }

    public class ShenReportItem
    {
        public string id { get; set; }
        public string doccontent { get; set; }
        public string docpuburl { get; set; }
        public string docpubjsonurl { get; set; }
        public string docpubtime { get; set; }
        public string doctype { get; set; }
        public string chnlcode { get; set; }
        public string index { get; set; }
    }

    public class ShenReportModel
    {
        public string code { get; set; }
        public string message { get; set; }
        public ShenReportDetailModel data { get; set; }
    }

    public class ShenReportDetailModel
    {
        public string docId { get; set; }
        public string title { get; set; }
        public string channelId { get; set; }
        public string domain { get; set; }
        public string[] imgList { get; set; }
        public string jsonPath { get; set; }
        public string content { get; set; }
    }
}
