using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib
{
    /// <summary>
    /// 爬取过的URL的历史记录
    /// </summary>
    public class CrawlHistroyEntry
    {
        public string Url { get; set; }
        /// <summary>
        ///  时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }   
        public long Size { get; set; }
    }
}
