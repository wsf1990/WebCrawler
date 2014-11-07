using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NWebCrawlerLib.Common
{
    /// <summary>
    /// 请求url的  NWebRequest
    /// </summary>
    public class NWebRequest
    {
        public string Method;
        public string Header;
        public WebHeaderCollection Headers;
        public Uri RequestUri;
        public int Timeout;
        public bool KeepAlive;
        public NWebResponse response;

        public NWebRequest(Uri uri, bool bKeepAlive)
        {
            Headers = new WebHeaderCollection();
            RequestUri = uri;
            Headers["Host"] = uri.Host;
            KeepAlive = bKeepAlive;
            if (KeepAlive)
                Headers["Connection"] = "Keep-Alive";
            //请求方式：GET
            Method = "GET";
            // 设置超时以避免耗费不必要的时间等待响应缓慢的服务器或尺寸过大的网页.
            Timeout = MemCache.ConnectionTimeoutMs;
        }
        /// <summary>
        /// 返回响应
        /// </summary>
        /// <returns></returns>
        public NWebResponse GetResponse()
        {
            if (response == null || response.socket == null || response.socket.Connected == false)
            {
                response = new NWebResponse();
                response.Connect(this);
                response.SetTimeout(Timeout);
            }
            response.SendRequest(this);
            response.ReceiveHeader();
            return response;
        }
    }
}
