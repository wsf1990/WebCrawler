using System.Net;
using NWebCrawlerLib.Common;
using NWebCrawlerLib.Connect;
using NWebCrawlerLib.Enum;
using NWebCrawlerLib.Event;
using NWebCrawlerLib.Interface;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NWebCrawlerLib
{
    public delegate void CrawlerStatusChangedEventHandler(object sender, CrawlerStatusChangedEventArgs e);

    /// <summary>
    /// 爬虫是能够自动下载网页的程序.
    /// Web的重要特性:
    /// 1.网络上的信息分散在数以十亿计的网页中, 而这些网页由遍布地球各个角落的数以百万计服务器负责存储.
    /// 2.Web是一个迅速演化的动态实体.
    /// </summary>
    public class CrawlerThread
    {
        #region field
        /// <summary>
        /// 将委托定义为事件
        /// </summary>
        public event CrawlerStatusChangedEventHandler StatusChanged;
        /// <summary>
        /// 工作线程
        /// </summary>
        private Thread m_thread;
        /// <summary>
        /// 线程阻止
        /// </summary>
        private ManualResetEvent m_suspendEvent = new ManualResetEvent(true);
        /// <summary>
        /// 工作状态
        /// </summary>
        private CrawlerStatusType m_statusType;
        
        /// <summary>
        /// 爬虫的下载者
        /// </summary>
        private Downloader m_downloader;
        /// <summary>
        /// 记录工作状态 URL 发生变化
        /// </summary>
        private bool m_dirty; 
        #endregion

        #region Props

        private string m_name;
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// 爬虫状态
        /// </summary>
        public CrawlerStatusType Status
        {
            get
            {
                return m_statusType;
            }
            set
            {
                if (m_statusType != value)
                {
                    m_statusType = value;
                    this.m_dirty = true;
                }

            }
        }
        /// <summary>
        /// 响应头
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        private string m_url;

        public string Url
        {
            get
            {
                return m_url;
            }
            set
            {
                if (m_url != value)
                {
                    m_url = value;
                    this.m_dirty = true;
                }
            }
        }

        #endregion

        #region ctor

        public CrawlerThread(Downloader d)
        {
            m_thread = new Thread(DoWork);
            m_name = m_thread.ManagedThreadId.ToString();
            this.m_downloader = d;
            this.m_dirty = false;
        }

        #endregion

        #region method
        public void Start()
        {
            m_thread.Start();
        }

        public void Abort()
        {
            m_thread.Abort();
        }

        public void Suspend()
        {
            m_suspendEvent.Reset();
        }
        /// <summary>
        /// 线程完毕，可以进行下一线程
        /// </summary>
        public void Resume()
        {
            m_suspendEvent.Set();
        }

        private void Flush()
        {
            if (this.m_dirty)
                this.StatusChanged(this, null);
        }
        #endregion

        #region Work
        private void DoWork()
        {
            while (true)
            {
                //终止当前线程
                m_suspendEvent.WaitOne(Timeout.Infinite);

                if (Crawler.UrlsQueueFrontier.Count > 0)
                {
                    try
                    {
                        // 从队列中获取URL
                        Url = (string)Crawler.UrlsQueueFrontier.Dequeue();

                        // 获取页面
                        Fetch();
                        // TODO: 检测是否完成
                        //if (false) break;
                    }
                    catch (InvalidOperationException ex)
                    {
                        SleepWhenQueueIsEmpty();
                    }
                }
                else
                {
                    SleepWhenQueueIsEmpty();
                }
            }
        }

        /// <summary>
        /// 这个方法主要做三件事:
        /// 1.获取页面.
        /// 2.提取URL并加入队列.
        /// 3.保存页面(到网页库).
        /// </summary>
        private void Fetch()
        {
            try
            {
                // 获取页面.
                Status = CrawlerStatusType.Fetch;
                Flush();
                //var req = new NWebRequest(Url);
                var req = new NRequest(Url);
                var response = req.Response;
                string contentType = MimeType = response.ContentType.ToLower();

                Crawler.CrawleHistroy.Add(new CrawlHistroyEntry()
                {
                    Timestamp = DateTime.UtcNow,
                    Url = Url,
                    Size = response.ContentLength
                });

                //此处网页和需要下载的响应头应放其过去，其余阻止
                //if (!contentType.Contains("text/html") && !MemCache.AllowAllMimeTypes && !MemCache.AllowedFileTypes.Contains(contentType))
                //    return;
                if(string.IsNullOrWhiteSpace(contentType))
                    return;
                //TODO:根据contentType选择进行网页URL解析或者直接下载
                if (contentType.Contains("text/html"))//网页解析
                {
                    ParseURL(response);
                }
                else if (MemCache.IsAllowedFileTypes(contentType))//直接下载
                {
                    DownloadURL(response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        } 
        #endregion

        #region 对网页的处理
        /// <summary>
        /// 网页解析
        /// </summary>
        /// <param name="response"></param>
        private void ParseURL(WebResponse response)
        {
            string html = Utility.GetStringFromResponse(response);
            // 解析网页中URL.
            Status = CrawlerStatusType.GetUrl;
            Flush();

            string baseUri = Utility.GetBaseUri(Url);
            string[] links = Parser.ExtractLinks(baseUri, html);//解析网页中链接
            // 提取URL并加入队列.
            if (Crawler.UrlsQueueFrontier.Count < 1000)
            {
                Status = CrawlerStatusType.Parse;
                Flush();

                foreach (string link in links)
                {
                    // 避免爬虫陷阱
                    if (link.Length > 256)
                        continue;
                    // 避免出现环
                    if (Crawler.IsUrlContain(link))
                        continue;
                    // 加入队列
                    Crawler.UrlsQueueFrontier.Enqueue(link);
                }
            }

            Console.WriteLine("[{1}] Url: {0}", Url, Crawler.CrawleHistroy.Count);

            Url = string.Empty;
            Status = CrawlerStatusType.Idle;
            MimeType = string.Empty;
            Flush();
        }
        /// <summary>
        /// 下载URL内容
        /// </summary>
        private void DownloadURL(WebResponse response)
        {
            try
            {
                Status = CrawlerStatusType.Save;
                Flush();
                FileSystemUtility.StoreWebFile(Url, response);
                lock (m_downloader.TotalSizelock)
                {
                    m_downloader.TotalSize += response.ContentLength;
                    Crawler.FileCount++;
                }
            }
            #region IOException
            catch (IOException ioEx)
            {
                if (ioEx.InnerException != null)
                {

                    if (ioEx.InnerException is SocketException)
                    {
                        SocketException socketEx = (SocketException)ioEx.InnerException;
                        if (socketEx.NativeErrorCode == 10054)
                        {
                            // 远程主机强迫关闭了一个现有的连接。
                            // Logger.Error(ioEx.Message);
                        }
                    }
                    else
                    {
                        int hr = (int)ioEx.GetType().GetProperty("HResult",
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic).GetValue(ioEx, null);
                        if (hr == -2147024864)
                        {
                            // 另一个程序正在使用此文件，进程无法访问。 
                            // 束手无策 TODO: 想个办法
                            //Logger.Error(ioEx.Message);
                        }
                        else
                        {
                            //throw;
                            //Logger.Error(ioEx.Message);
                        }
                    }
                }
            }
            #endregion
            catch (NotSupportedException /*nsEx*/)
            {
                // 无法识别该 URI 前缀。
                // 束手无策 TODO: 想个办法
                //Logger.Error(nsEx.Message);
            }
        } 
        #endregion

        #region static method
        /// <summary>
        /// 考虑：会不会占用太多内存？
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static byte[] ReadInstreamIntoMemory(Stream stream)
        {
            int bufferSize = 16384;
            byte[] buffer = new byte[bufferSize];
            MemoryStream ms = new MemoryStream();
            while (true)
            {
                int numBytesRead = stream.Read(buffer, 0, bufferSize);
                if (numBytesRead <= 0) break;
                ms.Write(buffer, 0, numBytesRead);
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 为避免挤占CPU, 队列为空时睡觉. 
        /// </summary>
        private void SleepWhenQueueIsEmpty()
        {
            Status = CrawlerStatusType.Idle;
            Url = string.Empty;
            Flush();

            Thread.Sleep(MemCache.ThreadSleepTimeWhenQueueIsEmptyMs);
        } 
        #endregion

    }

}
