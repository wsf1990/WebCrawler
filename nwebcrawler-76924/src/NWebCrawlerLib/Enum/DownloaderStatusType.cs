using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Enum
{
    /// <summary>
    ///下载状态
    /// </summary>
    public enum DownloaderStatusType
    {
        NotStarted,
        Running,
        Suspended,
        Stopped,
    }
}
