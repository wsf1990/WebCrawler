using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Enum
{
    /// <summary>
    /// Crawler 爬虫
    /// 爬虫工作状态
    /// </summary>
    public enum CrawlerStatusType
    {
        Idle,
        Fetch,  // FetchWebContent
        Parse,  // ParseWebPage
        GetUrl, // 解析网页中的URL
        Save,   // SaveToRepository
    }
}
