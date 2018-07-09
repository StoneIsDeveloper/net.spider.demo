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

            PullData spider = new PullData();

            // Util.DownloadPdf();

            // 债券新闻
            //spider.GetBondsNewsData("诉讼");

            // 上交所
            //spider.GetNewsFromShangExchange("上海");

            // 深交所
            spider.GetResultByPost();
            // spider.GetShenReportDetailInfo();

            // 证监会
           // spider.GetSecuritiesNewsByPost();


            Console.ReadKey();

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
