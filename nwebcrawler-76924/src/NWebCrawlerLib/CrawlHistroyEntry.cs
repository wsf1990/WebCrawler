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
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///  时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is CrawlHistroyEntry)
            {
                var che = (CrawlHistroyEntry) obj;
                return che.Url == this.Url;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
