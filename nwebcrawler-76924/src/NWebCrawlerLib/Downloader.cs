using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using NWebCrawlerLib.Common;
using NWebCrawlerLib.Interface;
using NWebCrawlerLib.Event;
using NWebCrawlerLib.Enum;
using NWebCrawlerLib.URLQueue;

namespace NWebCrawlerLib
{
    /// <summary>
    /// 下载状态变化委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DownloaderStatusChangedEventHandler(object sender, DownloaderStatusChangedEventArgs e);
    
    /// <summary>
    /// 下载者
    /// </summary>
    public class Downloader : IDownloader
    {
        #region Privates
        /// <summary>
        /// 下载状态变化事件
        /// </summary>
        public event DownloaderStatusChangedEventHandler StatusChanged;

        public long TotalSize { get; set; }
        public object TotalSizelock = new object();

        public long Errors = 0L;

        #endregion

        #region Properties

        private DownloaderStatusType m_status;
        /// <summary>
        /// 下载状态
        /// </summary>
        public DownloaderStatusType Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
                if (null != StatusChanged)
                {
                    StatusChanged(this, null);
                }
            }
        }

        private bool m_dirty;
        /// <summary>
        /// 记录变化
        /// </summary>
        public bool Dirty
        {
            get { return m_dirty; }
            set { m_dirty = value; }
        }

        #endregion

        #region ctor
        public Downloader()
        {
            m_status = DownloaderStatusType.NotStarted;
            Crawler.UrlsQueueFrontier = new RoundRobinQueueManager();
            Crawler.CrawleHistroy = new List<CrawlHistroyEntry>();

            m_dirty = false;
        }
        #endregion

        /// <summary>
        /// 通常爬虫是从一系列种子(Seed)网页开始,然后使用这些网页中的链接去获取其他页面.
        /// </summary>
        /// <param name="seeds">
        /// </param>
        public void InitSeeds(string[] seeds)
        {
            Crawler.UrlsQueueFrontier.Clear();
            // 使用种子URL进行队列初始化
            foreach (string s in seeds)
                Crawler.UrlsQueueFrontier.Enqueue(s);
        }
        
        /// <summary>
        /// 状体变化时 m_dirty 置为true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CrawlerStatusChanged(object sender, CrawlerStatusChangedEventArgs e)
        {
            this.m_dirty = true;
        }

        /// <summary>
        /// 获取下载速度
        /// </summary>
        /// <returns></returns>
        public double GetDownloadSpeed()
        {
            long totalSize = 0;
            DateTime now = DateTime.UtcNow;
            int window = 15; // seconds
            lock (Crawler.CrawleHistroy)
            {
                for (int i = Crawler.CrawleHistroy.Count - 1; i >= 0; i--)
                {
                    CrawlHistroyEntry entry = Crawler.CrawleHistroy[i];
                    if ((now - entry.Timestamp) <= TimeSpan.FromSeconds(window))
                        totalSize += entry.Size;
                    else
                        break;
                }
            }
            double speed = 1.0 * totalSize / window / 1024;
            return speed;
        }

        public void Dump(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            using (StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.CreateNew)))
            {
                while (Crawler.UrlsQueueFrontier.Count > 0)
                {
                    string url = Crawler.UrlsQueueFrontier.Dequeue();
                    writer.WriteLine(url);
                }
            }
        }
    }
}
