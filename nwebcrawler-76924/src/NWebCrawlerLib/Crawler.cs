using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NWebCrawlerLib.Interface;

namespace NWebCrawlerLib
{
    /// <summary>
    /// 爬虫封装类
    /// </summary>
    public class Crawler
    {
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
            return CrawleHistroy.Count(s => s.Url == url) > 0;
        }
    }
}
