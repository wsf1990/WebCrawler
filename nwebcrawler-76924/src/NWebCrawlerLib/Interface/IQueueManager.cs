using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWebCrawlerLib.Interface
{
    public interface IQueueManager
    {
        int Count { get; }
        void Enqueue(string url);
        string Dequeue();
        void Clear();
        /// <summary>
        /// 判断url是否已经加入队列
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool IsContain(string url);
    }
}
