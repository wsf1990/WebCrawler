using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
        public Collection<CrawlerThread> CrawlerThreads { get; set; }

        /// <summary>
        /// 爬取历史记录
        /// </summary>
        public IList<CrawlHistroyEntry> CrawleHistroy { get; set; }
    }
}
