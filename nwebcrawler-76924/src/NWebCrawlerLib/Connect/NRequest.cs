using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NWebCrawlerLib.Connect
{
    /// <summary>
    /// 封装的Request类
    /// </summary>
    public class NRequest
    {
        //public string Method;
        //public string Header;
        //public WebHeaderCollection Headers;
        //public Uri RequestUri;
        public int Timeout;
        //public bool KeepAlive;

        private HttpWebRequest Request;// = (HttpWebRequest)WebRequest.Create(Url);

        private WebResponse response;
        public WebResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = Request.GetResponse();
                }
                return response;
            }
        }

        public NRequest(string url)
        {
            Timeout = MemCache.ConnectionTimeoutMs;
            //Method = "GET";
            //Headers["Connection"] = "Keep-Alive";
            //Headers["Host"] = new Uri(url).Host;

            Request = (HttpWebRequest)WebRequest.Create(url);
            Request.Timeout = Timeout;
        }
    }
}
