using Newtonsoft.Json;
using spiderDemo.Handler;
using spiderDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace spiderDemo
{
    public class PullData
    {
        public static string txtStatusCode { get; set; }

        public static string txtStatusText { get; set; }



        #region 债券新闻
        public void GetBondsNewsData(string searchText, string type = "title")
        {
            var customHandler = new CustomProcessingHandler
            {
                InnerHandler = new HttpClientHandler()
            };
            //types=S888009007
            var title = searchText;  // 标题检索
            var pageIndex = "1";
            var limit = "50";

            var encodeTitle = Util.EncodeString(title);

            var url = string.Format("app.jg.eastmoney.com/NewsData/GetNewsBySearch.do?types=S888009007&{0}={1}&pageIndex={2}&limit={3}&sort=date&order=desc", type, encodeTitle, pageIndex, limit);
            var uri = "http://" + url;
            //var uri = "http://app.jg.eastmoney.com/NewsData/GetNewsBySearch.do?types=S888009007&title=%E8%AF%89%E8%AE%BC&pageIndex=1&limit=50&sort=date&order=desc";
            var client = new HttpClient(customHandler, true)
            {
                BaseAddress = new Uri(uri)
            };
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

            var count = 0;
            while (count< 300)
            {
                Thread.Sleep(1000);
                var task = client.GetAsync(client.BaseAddress);
                task.Result.EnsureSuccessStatusCode();
                HttpResponseMessage response = task.Result;

                txtStatusCode = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;

                var result = response.Content.ReadAsStringAsync();
                var txtMsg = result.Result.Replace("<br>", Environment.NewLine); // Insert new lines

                var filePath = @"D:\债券测试\TempRsult"+ count+".txt";

                //  var data = ConverToNewsList(txtMsg);
                //  var str = JsonConvert.SerializeObject(data);
                // File.WriteAllText(filePath, str, Encoding.UTF8);
                File.WriteAllText(filePath, "testing ", Encoding.UTF8);

                Console.WriteLine(filePath+ count+".....");
                count++;
            }


        }

        public List<NewsViewModel> ConverToNewsList(string txtMsg)
        {
            List<NewsViewModel> items = new List<NewsViewModel>();
            var summary = JsonConvert.DeserializeObject<Suammry>(txtMsg);
            var newsList = summary.NewsList;
            var start = txtMsg.IndexOf("records") + 9;
            var end = txtMsg.IndexOf("total") - 2;
            Console.WriteLine("start" + start);
            Console.WriteLine("end" + end);
            txtMsg = txtMsg.Substring(start, end - start);

            var model = JsonConvert.DeserializeObject(txtMsg);

            var list = JsonConvert.DeserializeObject<List<News>>(txtMsg);
            var count = list.Count;
            list = list.Take(5).ToList();
            list.ForEach(o =>
            {
                var id = o.Id;
                var item = GetNewsDetail(o.Id);
                items.Add(item);
            });
            return items;
        }

        public NewsViewModel GetNewsDetail(string id)
        {
            var url = string.Format("app.jg.eastmoney.com/NewsData/GetNewsText.do?id={0}&cid=Admin", id);
            var uri = "http://" + url;
            var customHandler = new CustomProcessingHandler
            {
                InnerHandler = new HttpClientHandler()
            };
            var client = new HttpClient(customHandler, true)
            {
                BaseAddress = new Uri(uri)
            };
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            var task = client.GetAsync(client.BaseAddress);
            task.Result.EnsureSuccessStatusCode();
            HttpResponseMessage response = task.Result;
            var result = response.Content.ReadAsStringAsync();
            var txtMsg = result.Result.Replace("<br>", Environment.NewLine);
            return JsonConvert.DeserializeObject<NewsViewModel>(txtMsg);
        }

        #endregion

        #region 上交所公告
        public async void GetNewsFromShangExchange(string keyword)
        {
            var encodeKey = Util.EncodeString(keyword);

            // query string parameters
            var search = "qwjs";
            var jsonCallBack = "jQuery111202517423818574538_1531118387371";
            var page = "1";
            var orderby = "-CRELEASETIME";
            var perpage = "10";
            var last = "1531118387379";
            var urlStr = "/search/getSearchResult.do?search=" + search + "&jsonCallBack=" + jsonCallBack + "&page=" + page + "&searchword=T_L+CTITLE+T_D+E_KEYWORDS+T_JT_E+T_L" + encodeKey + "B7T_RT_R&orderby=" + orderby + "&perpage=" + perpage + "&_=" + last;

            var url = "http://query.sse.com.cn" + urlStr;

            url = "http://query.sse.com.cn/search/getSearchResult.do?search=qwjs&jsonCallBack=" + jsonCallBack + "&page=" + page + "&searchword=T_L+CTITLE+T_D+E_KEYWORDS+T_JT_E+T_L" + encodeKey + "T_RT_R&orderby=-CRELEASETIME&perpage=" + perpage + "&_=1531119909975";



            HttpClient httpClient = new HttpClient();



            httpClient.DefaultRequestHeaders.Add("Cookie", "yfx_c_g_u_id_10000042=_ck18070722304716339381556754279; VISITED_MENU=%5B%228307%22%5D; yfx_f_l_v_t_10000042=f_t_1530973847600__r_t_1531066298213__v_t_1531066298213__r_c_1; seecookie=%u4E0A%u6D77%2C%u4E2D%u4FE1");
            httpClient.DefaultRequestHeaders.Add("Host", "query.sse.com.cn");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("http://www.sse.com.cn/home/search/?webswd=" + encodeKey);
            httpClient.DefaultRequestHeaders.Add("UserAgent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");

            Console.WriteLine("begin send request:..." + DateTime.Now);

            var count = 0;
            //var recivieResponse = true;
            while (count < 300)
            {
                try
                {
                    count += 1;
                    // 创建一个异步get请求，当请求返回时继续处理   
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string resultStr = await response.Content.ReadAsStringAsync();
                    File.WriteAllText(@"D:\上交所测试\上交所文件" + count + ".txt", "测试ing", Encoding.UTF8);
                    Console.WriteLine("save file 上交所文件" + count + " __:" + DateTime.Now);
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    //recivieResponse = false;
                    File.WriteAllText(@"D:\上交所测试\error" + count + ".txt", ex.Message, Encoding.UTF8);
                    break;
                }
            }
            Console.WriteLine("save file finished." + DateTime.Now);
        }



        #endregion

        #region 深交所 post

        public void GetResultByPost()
        {
            HttpClient httpClient = new HttpClient();
            var keyword = "加强";
            var range = "title";  // 标题
            // range = "body";  // 正文
            // range = "content";  // 标题+正文
            var time = "1";
            var orderby = "score";
            var currentPage = "1";
            var pageSize = "20";
            var random = "0.3778834245960281";
            var url = "http://www.szse.cn/api/search/content?random=" + random;

            // 设置请求头信息  
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");
            // httpClient.DefaultRequestHeaders.Add("Content-Length", "85");
            // httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            httpClient.DefaultRequestHeaders.Add("Origin", "http://www.szse.cn");
            httpClient.DefaultRequestHeaders.Add("Host", "www.szse.cn");
            httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
            var encodeKey = Util.EncodeString(keyword);
            httpClient.DefaultRequestHeaders.Add("Referer", "http://www.szse.cn/application/search/index.html?keyword=" + encodeKey);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("X-Request-Type", "ajax");
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");


            List<ShenReportItem> shenReportItems = new List<ShenReportItem>();
            List<ShenReportModel> shenModeltItems = new List<ShenReportModel>();

            var count = 0;
            while (count < 500)
            {
                Thread.Sleep(1000);
                // 构造POST参数  
                HttpContent postContent = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                   {"keyword", keyword},
                   {"range", range},
                   {"time", time},
                   {"orderby",orderby},
                   {"currentPage", currentPage},
                    {"pageSize", pageSize},
                });

                httpClient
                   .PostAsync(url, postContent)
                   .ContinueWith(
                   (postTask) =>
                   {
                       HttpResponseMessage response = postTask.Result;

                       // 确认响应成功，否则抛出异常  
                       response.EnsureSuccessStatusCode();

                       // 异步读取响应为字符串  
                       response.Content.ReadAsStringAsync().ContinueWith(
                               (readTask) =>
                               {
                                  // File.WriteAllText(@"D:\深交所查询结果集合.txt", readTask.Result, Encoding.UTF8);
                                 //  var model = JsonConvert.DeserializeObject<ShenReportList>(readTask.Result);
                                  // var size = model.totalSize;
                                 //  shenReportItems = model.data;
                                   //  Console.WriteLine("shenReportItems：" + shenReportItems.Count);
                                   var countPdf = 0;
                                   var pdfPath = @"D:\PDF\file" + countPdf + ".pdf";
                                   //shenReportItems.ForEach(o =>
                                   //{
                                   //    countPdf += 1;
                                   //    var docType = o.doctype;
                                   //    if (docType == "html")
                                   //    {
                                   //        var uri = "http://www.szse.cn" + o.docpubjsonurl;
                                   //        var vmModel = GetShenDetail(uri);
                                   //        shenModeltItems.Add(vmModel);
                                   //    }
                                   //    else
                                   //    {
                                   //        pdfPath = @"D:\PDF\file" + countPdf + ".pdf";
                                   //        var uri = o.docpubjsonurl;
                                   //        if (uri.IndexOf("www.szse.cn") < 0)
                                   //        {
                                   //            if (uri.IndexOf("http:") > -1)
                                   //            {
                                   //                uri = uri.Substring(5);
                                   //            }
                                   //            uri = "http://www.szse.cn" + uri;
                                   //        }
                                   //    //Util.DownloadPdf(uri, pdfPath);
                                   //}
                                   //});
                                   // var names = shenModeltItems.Select(o => o.data.title).ToList();

                                   // var encodeStr = JsonConvert.SerializeObject(names);

                                   File.WriteAllText(@"D:\深交所测试\深交所" + count + ".txt", "测试ing", Encoding.UTF8);
                                   Console.WriteLine("深交所" + count + ".....");
                               });

                       Console.WriteLine("响应是否成功：" + response.IsSuccessStatusCode);

                     //  Console.WriteLine("响应头信息如下：\n");
                       var headers = response.Headers;

                       //foreach (var header in headers)
                       //{
                       //    Console.WriteLine("{0}: {1}", header.Key, string.Join("", header.Value.ToList()));
                       //}

                   }
                   );
                count = count + 1;
            }
        }

        public async Task<ShenReportModel> GetShenReportDetailInfo(string uri, HttpClient httpClient)
        {
            //var uri = "http://www.szse.cn/aboutus/trends/news/t20180608_539236.html";
            if (string.IsNullOrEmpty(uri))
            {
                uri = "http://www.szse.cn" + "/json/aboutus/trends/news/t20180608_539236.json";
            }

            // 创建一个异步get请求，当请求返回时继续处理
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string resultStr = await response.Content.ReadAsStringAsync();

            File.WriteAllText(@"D:\深交所新闻.txt", resultStr, Encoding.UTF8);

            var model = JsonConvert.DeserializeObject<ShenReportModel>(resultStr);
            return model;

        }

        public ShenReportModel GetShenDetail(string uri)
        {
            var customHandler = new CustomProcessingHandler
            {
                InnerHandler = new HttpClientHandler()
            };
            var client = new HttpClient(customHandler, true)
            {
                BaseAddress = new Uri(uri)
            };
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            var task = client.GetAsync(client.BaseAddress);
            task.Result.EnsureSuccessStatusCode();
            HttpResponseMessage response = task.Result;
            var result = response.Content.ReadAsStringAsync();
            var txtMsg = result.Result.Replace("<br>", Environment.NewLine);
            return JsonConvert.DeserializeObject<ShenReportModel>(txtMsg);
        }
        #endregion

        #region 证监会 post

        public void GetSecuritiesNewsByPost()
        {
            HttpClient httpClient = new HttpClient();
            var uri = "http://www.csrc.gov.cn/wcm/websearch/zjh_simp_list.jsp";
            var schword = "中信";
            // 设置请求头信息  
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");
            httpClient.DefaultRequestHeaders.Add("Cookie", "JSESSIONID=EA937871A9B687AE18F97A0CD130176E");
            httpClient.DefaultRequestHeaders.Add("Host", "www.csrc.gov.cn");
            httpClient.DefaultRequestHeaders.Add("Origin", "http://www.csrc.gov.cn");
            httpClient.DefaultRequestHeaders.Add("Referer", "http://www.csrc.gov.cn/pub/newsite/");
            // 构造POST参数  
            HttpContent postContent = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
               {"schword", schword},
               {"searchword", "pub/newsite/"},
               {"channelid", "3858"},
               {"whereId",""}
            });

            httpClient
            .PostAsync(uri, postContent)
            .ContinueWith(
            (postTask) =>
            {
                HttpResponseMessage response = postTask.Result;
                // 确认响应成功，否则抛出异常  
                response.EnsureSuccessStatusCode();
                Console.WriteLine("EnsureSuccessStatusCode.");
                // 异步读取响应为字符串  
                response.Content.ReadAsStringAsync().ContinueWith(
                    (readTask) =>
                    {
                        File.WriteAllText(@"D:\证监会查询结果.txt", readTask.Result, Encoding.UTF8);
                        Console.WriteLine("save as txt finished.");
                    });
            });
        }


        #endregion

        // 频率限制 每小时请求返回的上限

        public async void Run(string uri)
        {
            uri = "https://blog.csdn.net/moonpure/article/details/45274263";
            Console.WriteLine("run start..." + DateTime.Now);
            HttpClient httpClient = new HttpClient();

            // 创建一个异步get请求，当请求返回时继续处理
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string resultStr = await response.Content.ReadAsStringAsync();

            Console.WriteLine("run end..." + DateTime.Now);
            File.WriteAllText(@"E:\TempResult.txt", resultStr, Encoding.UTF8);
            Console.WriteLine("finish save file." + DateTime.Now);
        }

        public void GetRequest()
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(new Uri(@"http://www.baidu.com"));
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = 100;
            webRequest.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            webRequest.Referer = @"http://www.baidu.com";
            webRequest.AllowAutoRedirect = true;

            //添加 cookie
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(@"http://www.baidu.com"), new Cookie("cookiekey", "cookievalue"));
            webRequest.CookieContainer = cookieContainer;

        }
    }



}
