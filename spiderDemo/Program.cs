using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace spiderDemo
{
    public class Program
    {
        private const string Uri = "http://api.worldbank.org/countries?format=json";

        public static void Main(string[] args)
        {
            //new Spider().GetRequst();
            Console.WriteLine("begin...."+ DateTime.Now);
            var uri = "http://query.sse.com.cn/search/getSearchResult.do?search=qwjs&jsonCallBack=jQuery1112054606790882445_1530973857460&page=1&searchword=T_L+CTITLE+T_D+E_KEYWORDS+T_JT_E+T_L%E4%B8%8A%E6%B5%B7T_RT_R&orderby=-CRELEASETIME&perpage=10&_=1530973857470";
            //RunSJS(uri);
            DownloadPdf();
            Console.ReadKey();

        }

        static void DownloadPdf()
        {
            var url = "http://www.sse.com.cn/disclosure/bond/announcement/asset/c/3282034567288629.pdf";

            HttpClient httpClient = new HttpClient();

            // 创建一个异步GET请求，当请求返回时继续处理  
            httpClient.GetAsync(url).ContinueWith(
                (requestTask) =>
                {
                    HttpResponseMessage response = requestTask.Result;

                    // 确认响应成功，否则抛出异常  
                    response.EnsureSuccessStatusCode();

                    // 异步读取响应为字符串  
                    response.Content.DownloadAsFileAsync(@"E:\test1.pdf", true).ContinueWith(
                        (readTask) => Console.WriteLine("文件下载完成！"));
                });

            Console.WriteLine("输入任意字符结束...");
            Console.ReadLine();
        }

      

        // 上交所
        static async void RunSJS(string uri)
        {
            Console.WriteLine("run start..." + DateTime.Now);
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Cookie", "yfx_c_g_u_id_10000042=_ck18070722304716339381556754279; VISITED_MENU=%5B%228307%22%5D; yfx_f_l_v_t_10000042=f_t_1530973847600__r_t_1531066298213__v_t_1531066298213__r_c_1; seecookie=%u4E0A%u6D77%2C%u4E2D%u4FE1");
            httpClient.DefaultRequestHeaders.Add("Host", "query.sse.com.cn");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("http://www.sse.com.cn/home/search/?webswd=%E4%B8%AD%E4%BF%A1");
            httpClient.DefaultRequestHeaders.Add("UserAgent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");


            // 创建一个异步get请求，当请求返回时继续处理
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string resultStr = await response.Content.ReadAsStringAsync();
            Console.WriteLine("run end..." + DateTime.Now);
            //Console.WriteLine(resultStr);
            File.WriteAllText(@"E:\TempResult.txt", resultStr, Encoding.UTF8);
            Console.WriteLine("finish save file." + DateTime.Now);
        }
        static async void Run(string uri)
        {
            Console.WriteLine("run start..." + DateTime.Now);
            HttpClient httpClient = new HttpClient();

            

            // 创建一个异步get请求，当请求返回时继续处理
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string resultStr = await response.Content.ReadAsStringAsync();
            Console.WriteLine("run end..." + DateTime.Now);
            //Console.WriteLine(resultStr);
            File.WriteAllText(@"E:\TempResult.txt", resultStr, Encoding.UTF8);
            Console.WriteLine("finish save file." + DateTime.Now);
        }

    }

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

    public  class Spider
    {
        private const string Uri = "http://api.worldbank.org/countries?format=json";
        public void GetRequst()
        {
            HttpClient httpClient = new HttpClient();
            // 创建一个异步Get请求,当请求返回时继续处理
            httpClient.GetAsync(Uri).ContinueWith(
                (requstTask) => {
                    HttpResponseMessage response = requstTask.Result;
                    // 确认响应成功，否则抛出异常
                    response.EnsureSuccessStatusCode();

                    // 异步读取响应为字符串
                    response.Content.ReadAsStringAsync().ContinueWith(
                        (readTask) => {
                            Console.WriteLine(readTask.Result);
                        });
                });
            Console.WriteLine("Hit enter to exit.");
            Console.ReadKey();
        }

        public void Download()
        {
            var client = new WebClient();
            // 下载为字符串
            var webString = client.DownloadString(@"http://www.baidu.com");
            // 下载为二进制数据
            var webBytes = client.DownloadData(@"http://wwww.baidu.com");
            // 异步下载并监听完成
            client.DownloadStringCompleted += (Object Sender, DownloadStringCompletedEventArgs e) =>
            {
                Console.WriteLine("下载完成了");
            };
        }

        public void TakeRequest()
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(@"http://wwww.baidu.com"));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = 100;
            request.UserAgent = @"Mozilla/5.0 (iPhone; U; CPU like Mac OS X; en)";
            request.Referer = @"http://www.baidu.com";
            request.AllowAutoRedirect = true;
        }

        public void AddCookie()
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(@"http://wwww.baidu.com"));
            var container = new CookieContainer();
            container.Add(new Uri(@"http://www.baidu.com"), new Cookie("key", "value"));
            request.CookieContainer = container;
        }

        public void UseProxy()
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(@"http://wwww.baidu.com"));
            var proxy = new WebProxy(new Uri(@"127.0.0.1:8080"));
            request.Proxy = proxy;
        }

        public void ConvertJson()
        {
            var ConfigFileName = "";
            var ConfigJSONString = File.ReadAllText(ConfigFileName, Encoding.UTF8);
            var TempConfigObject = JsonConvert.DeserializeObject(ConfigJSONString);
        }

        public void SaveAsFile(string msg)
        {
            File.WriteAllText(@"D:\TempResult.txt", msg, Encoding.UTF8);
            // File.AppendAllText(@"D:\TempResult.txt", "test", Encoding.UTF8);
        }
    }

    public class DownloadImage
    {
        private WebProxy px = new WebProxy();
        private string url = @"http://www.quanjing.com/GetImage.ashx?q=%E8%87%AA%E7%84%B6%E7%95%8C%7C%7C1%7C100%7C1%7C2%7C%7C%7C%7C&Fr=1&CEFlag=1&size=&sortFlag=&isScroll=0&_=1310026216505";
        private Regex reUrl = new Regex("lowsrc=\"(.+?)\"");
        private string path = @"D:\Image";   
    }




}
