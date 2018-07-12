using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.ViewModel
{

    public class ShangReports
    {
        public string count { get; set; }

        public string countPage { get; set; }

        public List<ShangReportViewModel> data { get; set; }
    }

    public class ShangReportViewModel
    {
        [Description("标题")]
        public string CTITLE_TXT { get; set; }

        public string CCHANNELCODE { get; set; }

        [Description("内容摘要")]
        public string CONTENT { get; set; }

        [Description("发布日期 yyyy-mm-dd")]
        public string CRELEASETIME { get; set; }

        [Description("发布时间 HH：mm：ss ")]
        public string CRELEASETIME2 { get; set; }

        public string CSITECODE { get; set; }

        public string CURL { get; set; }

        public string DOCID { get; set; }

        public string FILESIZE { get; set; }

        public string KEYWORD { get; set; }

        [Description("消息类型")]
        public string MIMETYPE { get; set; }

    }
}
