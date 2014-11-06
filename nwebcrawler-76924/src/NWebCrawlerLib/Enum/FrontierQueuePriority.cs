using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Enum
{
    /// <summary>
    /// URL队列优先级
    /// 支持5种不同的优先级
    /// </summary>
    public enum FrontierQueuePriority
    {
        Low,
        BelowNormal,
        Normal,
        AboveNormal,
        High,
    }
}
