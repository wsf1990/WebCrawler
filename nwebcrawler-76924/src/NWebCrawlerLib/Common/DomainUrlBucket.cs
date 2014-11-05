using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Common
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
