using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Event
{
    public class NewErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
    }
}
