using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Event
{
    public class NewLogEventArgs : EventArgs
    {
        public string LogMessage { get; set; }
    }
}
