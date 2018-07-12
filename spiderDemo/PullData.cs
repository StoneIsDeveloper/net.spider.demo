using Newtonsoft.Json;
using spiderDemo.Handler;
using spiderDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

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
            while (count < 300)
            {
                Thread.Sleep(1000);
                var task = client.GetAsync(client.BaseAddress);
                task.Result.EnsureSuccessStatusCode();
                HttpResponseMessage response = task.Result;

                txtStatusCode = response.StatusCode + " " + response.ReasonPhrase + Environment.NewLine;

                var result = response.Content.ReadAsStringAsync();
                var txtMsg = result.Result.Replace("<br>", Environment.NewLine); // Insert new lines

                var filePath = @"D:\债券测试\TempRsult" + count + ".txt";

                //  var data = ConverToNewsList(txtMsg);
                //  var str = JsonConvert.SerializeObject(data);
                // File.WriteAllText(filePath, str, Encoding.UTF8);
                File.WriteAllText(filePath, "testing ", Encoding.UTF8);

                Console.WriteLine(filePath + count + ".....");
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

        #region 上交所公告 pdf
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


            var url = "http://query.sse.com.cn/search/getSearchResult.do?search=qwjs&jsonCallBack=" + jsonCallBack + "&page=" + page + "&searchword=T_L+CTITLE+T_D+E_KEYWORDS+T_JT_E+T_L" + encodeKey + "T_RT_R&orderby=-CRELEASETIME&perpage=" + perpage + "&_=1531119909975";

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Cookie", "yfx_c_g_u_id_10000042=_ck18070722304716339381556754279; VISITED_MENU=%5B%228307%22%5D; yfx_f_l_v_t_10000042=f_t_1530973847600__r_t_1531066298213__v_t_1531066298213__r_c_1; seecookie=%u4E0A%u6D77%2C%u4E2D%u4FE1");
            httpClient.DefaultRequestHeaders.Add("Host", "query.sse.com.cn");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("http://www.sse.com.cn/home/search/?webswd=" + encodeKey);
            httpClient.DefaultRequestHeaders.Add("UserAgent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");

            Console.WriteLine("begin send request:..." + DateTime.Now);

            // 创建一个异步get请求，当请求返回时继续处理   
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string resultStr = await response.Content.ReadAsStringAsync();

            var start = resultStr.IndexOf("(") + 1;
            var lastN = resultStr.LastIndexOf(")");

            resultStr = resultStr.Substring(start, lastN - start);

            File.WriteAllText(@"D:\上交所测试\上交所文件-" + keyword + ".txt", resultStr, Encoding.UTF8);

            var reports = JsonConvert.DeserializeObject<ShangReports>(resultStr);
            var host = "http://www.sse.com.cn/";
            var items = reports.data;
            items.ForEach(o =>
            {
                var type = o.MIMETYPE;
                if ("pdf".Equals(o.MIMETYPE, StringComparison.OrdinalIgnoreCase))
                {
                    var pdfUrl = host + o.CURL;
                    var index = o.CURL.LastIndexOf("/");
                    var end = o.CURL.LastIndexOf(".");
                    var docId = o.CURL.Substring(index, end - index);
                    var savePath = @"D:\上交所测试\" + docId + ".pdf";
                    Util.DownloadPdf(httpClient, pdfUrl, savePath);
                }
            });

            Console.WriteLine("save file finished." + DateTime.Now);
        }



        #endregion

        #region 深交所 html,pdf

        public void GetSZNewsByPost(string keyword)
        {
            HttpClient httpClient = new HttpClient();
            // var keyword = "加强";
            var range = "title";  // 标题
                                  // range = "body";  // 正文
            range = "content";  // 标题+正文
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
            // html 格式新闻
            List<ShenReportModel> shenModeltItems = new List<ShenReportModel>();

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
                           var content = readTask.Result;
                           content = content.Replace("},", Environment.NewLine);
                           File.WriteAllText(@"D:\深交所查询结果集合.txt", content, Encoding.UTF8);
                           var model = JsonConvert.DeserializeObject<ShenReportList>(readTask.Result);
                           var size = model.totalSize;
                           shenReportItems = model.data;
                           Console.WriteLine("shenReportItems：" + shenReportItems.Count);
                           var countPdf = 0;
                           shenReportItems.ForEach(o =>
                           {
                               countPdf += 1;
                               var docType = o.doctype;

                               if ("html".Equals(docType, StringComparison.OrdinalIgnoreCase))
                               {
                                   var uri = "http://www.szse.cn" + o.docpubjsonurl;
                                   var vmModel = GetShenDetail(uri);
                                   shenModeltItems.Add(vmModel);
                               }
                               if ("pdf".Equals(docType, StringComparison.OrdinalIgnoreCase))
                               {
                                   var pdfPath = @"D:\深交所测试\" + o.id + ".pdf";
                                   var pdfUri = o.docpuburl;
                                   Util.DownloadPdf(httpClient, pdfUri, pdfPath);
                               }
                           });
                           var names = shenModeltItems.Select(o => o.data.title).ToList();

                           var encodeStr = JsonConvert.SerializeObject(shenReportItems);
                           encodeStr = encodeStr.Replace("},", Environment.NewLine);
                           File.WriteAllText(@"D:\深交所查询格式化.txt", encodeStr, Encoding.UTF8);

                           var htmlStr = JsonConvert.SerializeObject(shenModeltItems);
                           htmlStr = htmlStr.Replace("},", Environment.NewLine);
                           File.WriteAllText(@"D:\深交所htm.txt", htmlStr, Encoding.UTF8);
                       });

                   Console.WriteLine("响应是否成功：" + response.IsSuccessStatusCode);

                   var headers = response.Headers;
               }
               );


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

        #region 证监会 

        public void GetSecuritiesNewsByPost(string schword)
        {
            var baseAddress = new Uri("http://www.csrc.gov.cn/pub/newsite/");

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("JSESSIONID", "5935DAA8FFDC9B282326FEE9B7A0C600", "/", "www.csrc.gov.cn"));
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };

            HttpClient httpClient = new HttpClient(handler)
            {
                BaseAddress = baseAddress
            };

            var uriPath = "http://www.csrc.gov.cn/pub/newsite/";
            var responseM = httpClient.GetAsync(uriPath).Result;

            var uri = "http://www.csrc.gov.cn/wcm/websearch/zjh_simp_list.jsp";

            #region 发送post，获取reponse
            // var schword = "中信";
            // 设置请求头信息  
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Origin", "http://www.csrc.gov.cn");
            httpClient.DefaultRequestHeaders.Add("Referer", "http://www.csrc.gov.cn/pub/newsite/");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

            var req = HttpWebRequest.Create(uri) as HttpWebRequest;
            SetHeaderValue(req.Headers, "Connection", "Keep-Alive");
            SetHeaderValue(req.Headers, "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            SetHeaderValue(req.Headers, "Accept-Encoding", "gzip, deflate");
            SetHeaderValue(req.Headers, "Connection", "keep-alive");
            SetHeaderValue(req.Headers, "Origin", "http://www.csrc.gov.cn");
            SetHeaderValue(req.Headers, "Referer", "http://www.csrc.gov.cn/pub/newsite/");
            SetHeaderValue(req.Headers, "User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36");
            SetHeaderValue(req.Headers, "Cache-Control", "no-cache");
            SetHeaderValue(req.Headers, "Accept-Language", "zh-CN,zh;q=0.9");
            SetHeaderValue(req.Headers, "Upgrade-Insecure-Requests", "1");

            req.ProtocolVersion = HttpVersion.Version10;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            var ordianryQuery = true;
            if (ordianryQuery) //普通查询
            {
                parameters.Add("schword", schword);
                parameters.Add("searchword", "pub/newsite/");
                parameters.Add("channelid", "3858");
                parameters.Add("whereId", "");
            }
            else  // 高级查询
            {
                parameters.Add("channelid", "1");
                parameters.Add("filetypeid", "pdf");
                parameters.Add("templet", "demo_result.jsp");
                parameters.Add("searchdate", "");
                parameters.Add("searchword", "(DOCTITLE=证监会 OR DOCCONTENT=证监会 OR APPFILE=证监会) and (fileext=pdf)");
                parameters.Add("searchword1", "证监会");
                parameters.Add("searchword2", "");
                parameters.Add("searchword3", "");
                parameters.Add("searchword4", "");
                parameters.Add("filetype", "pdf");
                parameters.Add("channel", "1");
                parameters.Add("idStartDate", "");
                parameters.Add("idEndDate", "");
                parameters.Add("prepage", "20");
                parameters.Add("getcontent", "on");
                parameters.Add("submit", "开始检索");
            }

            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                }
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
            req.ContentLength = data.Length;

            Stream requestStream = req.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            string htmlCode = string.Empty;
            try
            {
                using (Stream streamReceive = req.GetResponse().GetResponseStream())
                {
                    using (var zipStream = new System.IO.Compression.GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress))
                    {

                        using (StreamReader sr = new StreamReader(zipStream, Encoding.UTF8))
                        {
                            htmlCode = sr.ReadToEnd();
                        }
                    }
                }
                var index = htmlCode.IndexOf("in_main");

                if (index > -1)
                {
                    htmlCode = htmlCode.Insert(index + 8, " id=\"newsList\"");
                }
                htmlCode = htmlCode.Replace("<font color=red>", "");
                htmlCode = htmlCode.Replace("</font>", "");

                var resultList = ConvertToModelList(htmlCode);
                Console.WriteLine("save as txt finished.");
            }
            catch(Exception ex)
            {
                Console.WriteLine("eerror:>> req.GetResponse()  failed.............");
                Console.WriteLine(ex.Message);
            }
            #endregion          
        }

        public List<SecuritiesResultEntry> ConvertToModelList(string path)
        {
            List<SecuritiesResultEntry> list = new List<SecuritiesResultEntry>();
            var content = "";
            var host = @"http://www.csrc.gov.cn";
            if (string.IsNullOrEmpty(path))
            {
                path = @"D:\证监会查询结果.txt";
                FileStream file = File.OpenRead(path);
                StreamReader strRead = new StreamReader(path);
                content = strRead.ReadToEnd();
            }
            else
            {
                content = path;
            }

            HtmlDocument doc = new HtmlDocument();
            // doc.Load(path);
            doc.LoadHtml(content);
            var htmlNode = doc.DocumentNode;

            var node = doc.GetElementbyId("newsList");
            string titleValue = node.GetAttributeValue("id", "");
            foreach (HtmlAttribute attr in node.Attributes)
            {
                Console.WriteLine("{0}={1}", attr.Name, attr.Value);
            }
            var childs = node.ChildNodes;
            var allMenumbers = node.ChildNodes.ToList();

            var news = allMenumbers[3].ChildNodes.Where(o => o.Name == "div").ToList();
            news.ForEach(item =>
            {
                var h1 = item.ChildNodes.Where(o => o.Name == "div").FirstOrDefault();
                if (h1 != null)
                {
                    var aNode = h1.FirstChild;
                    var title = aNode.InnerHtml;
                    var href = aNode.GetAttributeValue("href", "");

                    var contentDiv = item.ChildNodes.Where(o => o.Name == "div").Last();
                    var modules = contentDiv.ChildNodes.Where(o => o.Name == "div").ToList();
                    var txtNode = modules.FirstOrDefault();
                    var contentText = txtNode != null ? txtNode.InnerHtml : "";
                    contentText = contentText.Replace("\r", "");
                    contentText = contentText.Replace("\n", "");
                    contentText = contentText.Replace("\t", "");
                    contentText.Trim();

                    var fileNode = modules.LastOrDefault();
                    var spans = fileNode.ChildNodes.Where(o => o.Name == "span").ToList();
                    var timeNode = spans.FirstOrDefault();
                    var timeText = timeNode != null ? timeNode.InnerHtml : "";
                    var sourceNode = spans.LastOrDefault();
                    var sourceText = sourceNode != null ? sourceNode.InnerHtml : "";

                    SecuritiesResultEntry entry = new SecuritiesResultEntry
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = title,
                        SourceUri = host + href,
                        Summary = contentText,
                        ReleaseDate = timeText,
                        SourceWebsite = sourceText
                    };
                    list.Add(entry);
                }
            });
          
            List<FileViewModel> fileList = new List<FileViewModel>();
            list.ForEach(item =>
            {
                var files = GetSecuritiesDetail( ref item);
                fileList.AddRange(files);
            });

            var countFile = fileList.Count;
            Console.WriteLine("countFile: "+ countFile);
            //下载 pdf
            HttpClient httpClient = new HttpClient();
            fileList.ForEach(o =>
            {
                var fileName = o.FileName;
                var filePath = @"D:\证监会测试\" + fileName;
              //  Util.DownloadPdf(httpClient, o.Url, filePath);
            });

            var encodeStr = JsonConvert.SerializeObject(list);
            encodeStr = encodeStr.Replace(",", "\r\n\r\n");
            File.WriteAllText(@"D:\证监会查询结果-encode.txt", encodeStr, Encoding.UTF8);
            Console.WriteLine("save as 证监会查询结果-encode.txt finished. ");

            var report = list.FirstOrDefault();
            return list;
        }

        public List<FileViewModel> GetSecuritiesDetail(ref SecuritiesResultEntry detailModel)
        {
            // SecuritiesResultEntry detailModel = new SecuritiesResultEntry();

            HttpClient httpClient = new HttpClient();
            List<FileViewModel> fileList = new List<FileViewModel>();
            var isWebNews = true;
            var megId = detailModel.Id;
            //zjhpublic,newsite
            var sourceUri = detailModel.SourceUri;
            if (sourceUri.IndexOf("zjhpublic") > -1)
            {
                isWebNews = false;
            }

            var index1 = sourceUri.LastIndexOf("/");
            var partUri = sourceUri.Substring(0, index1);

            //var path = @"/pub/newsite/zjhxwfb/xwdd/201706/t20170609_318109.html";
            //var host = @"http://www.csrc.gov.cn";
            // 新闻
            var fileCont = 0;
            var responseSuccess = true;
            var msgContent = "";
            if (isWebNews)
            {         
                var uri = new Uri(sourceUri);
                var task = httpClient.GetAsync(uri);
                try
                {
                    task.Result.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(sourceUri);
                    responseSuccess = false;
                }
                if (responseSuccess)
                {
                    HttpResponseMessage response = task.Result;
                    var result = response.Content.ReadAsStringAsync();
                    var htmlCode = result.Result.Replace("<br>", Environment.NewLine);

                    var index = htmlCode.IndexOf("in_main");
                    if (index > -1)
                    {
                        htmlCode = htmlCode.Insert(index + 8, " id=\"in_main\"");
                    }
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlCode);
                    var node = doc.GetElementbyId("in_main");
                 
                    if (node != null)
                    {
                        var content = node.ChildNodes.Where(o => o.Name == "div").FirstOrDefault();
                        if (content != null)
                        {
                            #region 正文
                            msgContent = content.InnerHtml;
                            var indexStart = msgContent.IndexOf("<script");
                            var indexEnd = msgContent.LastIndexOf("</script>");
                            if(indexStart>0 && indexEnd>0 && indexStart < indexEnd)
                            {
                                msgContent = msgContent.Remove(indexStart, indexEnd - indexStart);
                            }
                            var contentChildren = content.ChildNodes.Where(o => o.Name == "div").ToList();
                            if (contentChildren.Count >= 3)
                            {
                                var newsCon = contentChildren[2];
                                var newsDiv = newsCon.ChildNodes.Where(o => o.Name == "div").LastOrDefault();
                                if (newsDiv != null)
                                {
                                    msgContent = newsDiv.InnerHtml;
                                }
                            }
                            #endregion

                            #region 附件
                            var scriptNode = content.ChildNodes.Where(o => o.Name == "script").FirstOrDefault();
                            var scriptContent = scriptNode.InnerHtml;
                            var start = scriptContent.IndexOf("<a");
                            var end = scriptContent.IndexOf("file_appendix!=") - 5;
                            if (start > 0 && end > 0 && end > start)
                            {
                                scriptContent = scriptContent.Substring(start, end - start);
                                scriptContent = "<div id=pdf>" + scriptContent + "</div>";
                                HtmlDocument pdfContent = new HtmlDocument();
                                pdfContent.LoadHtml(scriptContent);
                                var pdfNode = pdfContent.GetElementbyId("pdf");
                                var pdfChildren = pdfNode.ChildNodes.Where(o => o.Name == "a").ToList();

                                pdfChildren.ForEach(item =>
                                {
                                    var fileName = item.InnerHtml;
                                    var fileUri = item.GetAttributeValue("href", "");                                 
                                    var firstDot = fileUri.IndexOf(".");
                                    var lastDot = fileUri.LastIndexOf(".");
                                    var typeName = fileUri.Substring(lastDot);
                                    var fileType = FileType.pdf;
                                    var parseSucceed = Enum.TryParse<FileType>(typeName, out fileType);                                  
                                    var fileId = fileUri.Substring(firstDot + 1);
                                    fileUri = partUri + fileUri.Substring(1);
                                    FileViewModel file = new FileViewModel
                                    {
                                        FileCode = fileId,
                                        FileType = fileType,
                                        FileName = fileName,
                                        Url = fileUri,
                                        MessageId = megId
                                    };
                                    fileList.Add(file);
                                    fileCont++;
                                });
                            }
                            #endregion
                        }
                    }
                    detailModel.FileCount = fileCont;
                    detailModel.Content = msgContent;
                    detailModel.SourceWebsite = "证监会";
                }
            }
            else // 公开信息
            {
                var uri = new Uri(sourceUri);
                var task = httpClient.GetAsync(uri);
                try
                {
                    task.Result.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(sourceUri);
                    responseSuccess = false;
                }
                if (responseSuccess)
                {
                    HttpResponseMessage response = task.Result;
                    var result = response.Content.ReadAsStringAsync();
                    var htmlCode = result.Result.Replace("<br>", Environment.NewLine);
                    HtmlDocument doc = new HtmlDocument();
                    var index = htmlCode.IndexOf("mainContainer");
                    if (index > -1)
                    {
                        htmlCode = htmlCode.Insert(index + 15, " id=\"mainContainer\"");
                    }

                    doc.LoadHtml(htmlCode);

                    #region 正文的头部信息
                    var headContainer = doc.GetElementbyId("headContainer");

                    var titleNode = doc.GetElementbyId("lTitle");
                    var date = "";
                    var title = titleNode.InnerHtml;
                    var tbody = headContainer.ChildNodes.Where(o => o.Name == "tbody").FirstOrDefault();
                    if (tbody != null)
                    {
                        var trList = tbody.ChildNodes.Where(o => o.Name == "tr").ToList();
                        var count = trList.Count;

                        var tr2 = trList[1];
                        var td = tr2.ChildNodes.Where(o => o.Name == "td").FirstOrDefault();
                        var innerTable = td.ChildNodes.Where(o => o.Name == "table").FirstOrDefault();
                        var innerTr = innerTable.ChildNodes.Where(o => o.Name == "tr").FirstOrDefault();
                        var innerTd = innerTr.ChildNodes.Where(o => o.Name == "td").LastOrDefault();
                        var innerSpan = innerTd.ChildNodes.Where(o => o.Name == "span").FirstOrDefault();
                        date = innerSpan.InnerHtml;
                    }
                    #endregion

                    #region 附件
                    var mainContainer = doc.GetElementbyId("mainContainer");
                    if (mainContainer != null)
                    {
                        var mainDivs = mainContainer.ChildNodes.Where(o => o.Name == "div").ToList();               
                        var pdfContent = mainDivs[2];
                        var pdfNodes = pdfContent.ChildNodes.Where(o => o.Name == "a").ToList();
                        pdfNodes.ForEach(item =>
                        {
                            var fileName = item.InnerHtml;
                            var fileUri = item.GetAttributeValue("href", "");
                            var firstDot = fileUri.IndexOf(".");
                            var lastDot = fileUri.LastIndexOf(".");
                            var typeName = fileUri.Substring(lastDot);
                            var fileType = FileType.pdf;
                            var parseSucceed = Enum.TryParse<FileType>(typeName, out fileType);
                            var fileId = fileUri.Substring(firstDot + 1);
                            fileUri = partUri + fileUri.Substring(1);
                            FileViewModel file = new FileViewModel
                            {
                                FileCode = fileId,
                                FileType = fileType,
                                FileName = fileName,
                                Url = fileUri,
                                MessageId = megId
                            };
                            fileList.Add(file);
                            fileCont++;
                        });  
                    }
                    #endregion

                    #region 正文
                    var contentRegion = doc.GetElementbyId("ContentRegion");
                    if (contentRegion != null)
                    {
                        var content = contentRegion.ChildNodes.Where(o => o.Name == "div").FirstOrDefault();
                        if (content != null)
                        {
                            msgContent = content.InnerHtml;
                            var divInner = content.ChildNodes.Where(o => o.Name == "div").FirstOrDefault();
                            if (divInner != null)
                            {
                                var conDiv = divInner.ChildNodes.Where(o => o.Name == "div").FirstOrDefault();
                                if (conDiv != null)
                                {
                                    var spanNode = conDiv.ChildNodes.Where(o => o.Name == "span").FirstOrDefault();
                                    if (spanNode != null)
                                    {
                                        msgContent = spanNode.InnerHtml;
                                        msgContent = msgContent.Replace("&nbsp;", "");
                                        msgContent = msgContent.Trim();
                                        Console.WriteLine(msgContent);
                                    }

                                }

                            }
                        }
                    }
                    #endregion

                    detailModel.FileCount = fileCont;
                    detailModel.Content = msgContent;
                    detailModel.SourceWebsite = "证监会";
                }
            }
          
            return fileList;

        }

        #endregion

        #region  中国执行信息公开网（失信人信息查询） /验证码
        public void GetCourtPersonInfo()
        {
            var uri = "http://zxgk.court.gov.cn/shixin/new_index.html";
            HttpClient httpClient = new HttpClient();
            var task = httpClient.GetAsync(uri);
            task.Result.EnsureSuccessStatusCode();
            var result = task.Result.Content.ReadAsStringAsync();
            var txtMsg = result.Result;
            File.WriteAllText(@"D:\全国法院查询结果.txt", txtMsg, Encoding.UTF8);
            Console.WriteLine(@"save file finished: D:\全国法院查询结果.txt");
        }

        #endregion

        #region 百度新闻
        public void GetBaiduNews()
        {
            var host = "http://news.baidu.com/?tn=news";
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

        public void TestRequest()
        {
            var uri = new Uri("http://www.csrc.gov.cn/pub/newsite/");
            uri = new Uri("http://ysp.www.gov.cn/013582404bd78ad3c016b8fffefe6a9a/allmobilize.min.js");
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Method = "Get";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            // webRequest.ContentLength = 100;
            webRequest.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            webRequest.Referer = @"http://www.baidu.com";
            webRequest.AllowAutoRedirect = true;

            //添加 cookie
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(@"http://www.baidu.com"), new Cookie("cookiekey", "cookievalue"));
            webRequest.CookieContainer = cookieContainer;

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Console.WriteLine(response);
        }

        private static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
    }



}
