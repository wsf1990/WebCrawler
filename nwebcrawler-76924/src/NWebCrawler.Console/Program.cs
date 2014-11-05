using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NWebCrawlerLib;

namespace NWebCrawler.Console
{
    /// <summary>
    /// 控制台程序，用于启动下载者
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Downloader downloader = new Downloader();
            downloader.InitSeeds(new string[] { "http://www.sohu.com" });
            downloader.Start();
        }
    }
}
