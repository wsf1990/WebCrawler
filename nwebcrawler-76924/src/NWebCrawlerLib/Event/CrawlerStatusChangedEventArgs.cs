using NWebCrawlerLib.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Event
{
    /// <summary>
    /// 工作状态改变事件
    /// </summary>
    public class CrawlerStatusChangedEventArgs : EventArgs
    {
        public CrawlerStatusChangedEventType EventType
        {
            get;
            set;
        }
    }
}
