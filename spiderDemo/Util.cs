using spiderDemo.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo
{
    public static class Util
    {
        public static string EncodeString( string text)
        {
            var getStr = "";
            byte[] bs = Encoding.UTF8.GetBytes(text);
            foreach (byte b in bs)
            {
                getStr = getStr + string.Format("%{0:X}", b);
            }
            return getStr;
        }

        public static void DownloadPdf(HttpClient httpClient, string url,string filePath)
        {
            if (string.IsNullOrEmpty(url))
            {
                url = "http://www.sse.com.cn/disclosure/bond/announcement/asset/c/3282034567288629.pdf";
            }



            // 创建一个异步GET请求，当请求返回时继续处理  
            httpClient.GetAsync(url).ContinueWith(
                (requestTask) =>
                {
                    HttpResponseMessage response = requestTask.Result;

                    // 确认响应成功，否则抛出异常  
                    response.EnsureSuccessStatusCode();

                    var stream = response.Content.ReadAsStreamAsync();
                    // 异步读取响应为字符串  
                    response.Content.DownloadAsFileAsync(filePath, true).ContinueWith(
                        (readTask) => Console.WriteLine("文件下载完成！"));
                                 

                });
        }

        public static string GetImgCode()
        {
            var code = string.Empty;


            return code;
        }
    }
}
