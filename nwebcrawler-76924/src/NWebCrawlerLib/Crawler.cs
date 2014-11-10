using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NWebCrawlerLib.Interface;
using NWebCrawlerLib.Enum;

namespace NWebCrawlerLib
{
    /// <summary>
    /// 爬虫封装类
    /// </summary>
    public class Crawler
    {
        /// <summary>
        /// DOWNLOADER对象
        /// </summary>
        public static Downloader DL = new Downloader();
        /// <summary>
        /// 爬虫线程集合
        /// </summary>
        public static Collection<CrawlerThread> CrawlerThreads { get; set; }

        /// <summary>
        /// 爬取历史记录
        /// </summary>
        public static IList<CrawlHistroyEntry> CrawleHistroy { get; set; }

        /// <summary>
        /// URL队列
        /// 尚未访问的URL列表, 使用先进先出 （First-in-first-out, FIFO) 的队列
        /// 对应的爬虫就是宽度优先爬虫 (Breadth-first crawler).
        /// </summary>
        public static IQueueManager UrlsQueueFrontier { get; set; }

        /// <summary>
        /// 判断是否已经爬取过
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsUrlContain(string url)
        {
            return CrawleHistroy.Contains(new CrawlHistroyEntry(){Url = url});
        }
        /// <summary>
        /// 启动工作
        /// </summary>
        public static void Start()
        {
            // 如果已经启动则退出
            if (null != CrawlerThreads)
                return;
            CrawlerThreads = new Collection<CrawlerThread>();

            for (int i = 0; i < MemCache.ThreadCount; i++)
            {
                CrawlerThread crawler = new CrawlerThread(DL);
                crawler.StatusChanged += DL.CrawlerStatusChanged;
                crawler.Start();

                CrawlerThreads.Add(crawler);
            }
            DL.Status = DownloaderStatusType.Running;
        }
        /// <summary>
        /// 挂起
        /// </summary>
        public static void Suspend()
        {
            if (null == CrawlerThreads)
                return;

            foreach (CrawlerThread crawler in CrawlerThreads)
            {
                crawler.Suspend();
            }

            DL.Status = DownloaderStatusType.Suspended;

        }
        /// <summary>
        /// 恢复
        /// </summary>
        public static void Resume()
        {
            if (null == CrawlerThreads)
                return;

            foreach (CrawlerThread crawler in CrawlerThreads)
            {
                crawler.Resume();
            }

            DL.Status = DownloaderStatusType.Running;
        }
        /// <summary>
        /// 中断
        /// </summary>
        public static void Abort()
        {
            if (null == CrawlerThreads)
                return;

            foreach (CrawlerThread crawler in CrawlerThreads)
            {
                crawler.Abort();
            }
        }
    }
}
