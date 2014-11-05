using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NWebCrawlerLib.Interface;
using System.Collections;
using System.Threading;

namespace NWebCrawlerLib.Common
{
    /// <summary>
    /// 使用LinkedList实现URL列表类
    /// </summary>
    public class RoundRobinQueueManager : IQueueManager
    {
        /// <summary>
        /// url作为键值，LinkedListNode作为值
        /// </summary>
        private Dictionary<string, LinkedListNode<DomainUrlBucket>> m_Hashtable = new Dictionary<string, LinkedListNode<DomainUrlBucket>>();
        /// <summary>
        /// 循环列表
        /// </summary>
        private LinkedList<DomainUrlBucket> m_CircularLinkedList = new LinkedList<DomainUrlBucket>();
        /// <summary>
        /// 当前节点
        /// </summary>
        private LinkedListNode<DomainUrlBucket> m_CurrentNode;
        /// <summary>
        /// 用于lock操作
        /// </summary>
        private object lockObject = new object();

        private int m_Count = 0;
        /// <summary>
        /// 节点中的数量
        /// </summary>
        public int Count
        {
            get
            {
                return m_Count;
            }
        }

        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="url"></param>
        public void Enqueue(string url)
        {
            lock (lockObject)
            {
                SynchronizedEnqueue(url);
                Interlocked.Increment(ref m_Count);
            }
        }

        /// <summary>
        /// 同步入队列
        /// </summary>
        /// <param name="url"></param>
        private void SynchronizedEnqueue(string url)
        {
            string baseUri = Utility.GetBaseUri(url);

            if (m_Hashtable.ContainsKey(baseUri))
            {
                LinkedListNode<DomainUrlBucket> chain = m_Hashtable[baseUri];
                chain.Value.UrlQueue.Enqueue(url);
            }
            else
            {
                LinkedListNode<DomainUrlBucket> newNode = new LinkedListNode<DomainUrlBucket>(new DomainUrlBucket());
                m_Hashtable[baseUri] = newNode;
                newNode.Value.UrlQueue.Enqueue(url);

                if (m_CurrentNode == null)
                {
                    m_CircularLinkedList.AddFirst(newNode);
                    m_CurrentNode = newNode;
                }
                else
                {
                    m_CircularLinkedList.AddBefore(m_CurrentNode, newNode);
                }
            }
        }
        /// <summary>
        /// 出队列
        /// </summary>
        /// <returns></returns>
        public string Dequeue()
        {
            lock (lockObject)
            {
                var result = SynchronizedDequeue();
                Interlocked.Decrement(ref m_Count);
                return result;
            }
        }
        /// <summary>
        /// 同步出队列
        /// </summary>
        /// <returns></returns>
        private string SynchronizedDequeue()
        {
            if (m_CurrentNode == null)
            {
                if (m_CircularLinkedList.Count > 0)
                {
                    m_CurrentNode = m_CircularLinkedList.First;
                }
                else
                {
                    throw new InvalidOperationException("Dequeue from an empty url queue.");
                }
            }

            while (m_CircularLinkedList.Count > 0 && m_CurrentNode.Value.UrlQueue.Count == 0)
            {
                var temp = m_CurrentNode.NextOrFirst();
                m_CircularLinkedList.Remove(m_CurrentNode);
                m_CurrentNode = temp;
            }

            if (m_CurrentNode.Value.UrlQueue.Count == 0)
            {
                m_CurrentNode = null;
                throw new InvalidOperationException("Dequeue from an empty url queue.");
            }
            var result = m_CurrentNode.Value.UrlQueue.Dequeue();

            if (m_CurrentNode.Value.UrlQueue.Count == 0)
            {
                m_Hashtable.Remove(Utility.GetBaseUri(result));
            }
            m_CurrentNode = m_CurrentNode.NextOrFirst();

            return result;
        }
        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            m_Hashtable.Clear();
            m_CircularLinkedList.Clear();
            m_CurrentNode = null;
            m_Count = 0;
        }
    }
}
