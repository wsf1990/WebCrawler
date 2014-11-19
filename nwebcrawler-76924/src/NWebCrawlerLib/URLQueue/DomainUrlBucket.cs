using System.Collections.Generic;

namespace NWebCrawlerLib.URLQueue
{
    /// <summary>
    /// URL队列类
    /// </summary>
    public class DomainUrlBucket
    {
        /// <summary>
        /// URL队列
        /// </summary>
        public Queue<string> UrlQueue = new Queue<string>();
    }
}
