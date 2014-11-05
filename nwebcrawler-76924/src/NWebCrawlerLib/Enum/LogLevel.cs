using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Enum
{
    /// <summary>
    /// Log级别
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        None = 0,
        Fatal = 1,
        Error = 2,
        Warn = 4,
        Info = 8,
        Debug = 16,
        Trace = 32,
    }
}
