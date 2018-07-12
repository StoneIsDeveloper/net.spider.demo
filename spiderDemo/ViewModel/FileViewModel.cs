using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.ViewModel
{
    public class FileViewModel
    {
        public FileType FileType { get; set; }

        public string FileCode { get; set; }

        public string FileName { get; set; }

        public string Url { get; set; }

        public string Path { get; set; }

        public string MessageId { get; set; }

        public string MessageTitle { get; set; }

    }

    public enum FileType
    {
        pdf = 1,
        doc = 2,
        excel = 3,
        img = 4
    }

}
