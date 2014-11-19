using System.Collections.Generic;

namespace NWebCrawlerLib.URLQueue
{
    /// <summary>
    /// 循环列表扩展方法类
    /// </summary>
    public static class CircularLinkedList
    {
        /// <summary>
        /// 下一个或者第一个
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static LinkedListNode<DomainUrlBucket> NextOrFirst(this LinkedListNode<DomainUrlBucket> current)
        {
            if (current.Next == null)
                return current.List.First;
            return current.Next;
        }
        /// <summary>
        /// 前一个或者最后一个
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static LinkedListNode<DomainUrlBucket> PreviousOrLast(this LinkedListNode<DomainUrlBucket> current)
        {
            if (current.Previous == null)
                return current.List.Last;
            return current.Previous;
        }
    }

}
