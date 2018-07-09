using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.Helper
{
    public static class HttpContentExtension
    {
        public static Task DownloadAsFileAsync(this HttpContent content, string fileName, bool overwrite)
        {
            string filePath = Path.GetFullPath(fileName);
            if (!overwrite && File.Exists(filePath))
            {
                throw new InvalidOperationException(string.Format("文件 {0} 已经存在！", filePath));
            }

            try
            {
                return content.ReadAsByteArrayAsync().ContinueWith(
                    (readBytestTask) =>
                    {
                        byte[] data = readBytestTask.Result;
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            fs.Write(data, 0, data.Length);
                            //清空缓冲区  
                            fs.Flush();
                            fs.Close();
                        }
                    }
                    );
            }
            catch (Exception e)
            {
                Console.WriteLine("发生异常： {0}", e.Message);
            }

            return null;
        }
    }
}
