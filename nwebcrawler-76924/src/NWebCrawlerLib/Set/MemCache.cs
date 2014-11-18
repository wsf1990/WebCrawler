using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace NWebCrawlerLib
{
    /// <summary>
    /// 内存缓存配置项信息
    /// </summary>
    static class MemCache
    {
        public static List<string> AllowedFileTypes;
        public static int ConnectionTimeoutMs;
        public static string SQLiteDBFolder;
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public static string FileSystemFolder;
        public static bool AllowAllMimeTypes;
        public static int ThreadCount;
        public static int ThreadSleepTimeWhenQueueIsEmptyMs;
        /// <summary>
        /// 允许下载的文件或者网页后缀
        /// </summary>
        public static List<string> AllowedExtensions;

        /// <summary>
        /// 在创建第一个实例或引用任何静态成员之前，将自动调用静态构造函数。
        /// 将ini中的配置项信息读取到内存中进行缓存
        /// </summary>
        static MemCache()
        {
            AllowedFileTypes = new List<string>(Settings.FileMatches.Split(','));
            ConnectionTimeoutMs = Settings.ConnectionTimeout * 1000;
            SQLiteDBFolder = Settings.SQLiteDBFolder;
            FileSystemFolder = Settings.FileSystemFolder;
            AllowAllMimeTypes = Settings.AllowAllMimeTypes;
            ThreadCount = Settings.ThreadCount;
            ThreadSleepTimeWhenQueueIsEmptyMs = Settings.ThreadSleepTimeWhenQueueIsEmpty * 1000;
            AllowedExtensions = Settings.AllowExtension.Split(',').ToList();
        }
        /// <summary>
        /// 是否是允许下载的响应头
        /// 除去text/html
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns> 
        public static bool IsAllowedFileTypes(string contentType)
        {
            return AllowedFileTypes.Count(s => s.ToLower().Contains(contentType.ToLower())) > 0;
        }
        /// <summary>
        /// 是否是允许的下载类型
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsAllowedExtensions(string filename)
        {
            return AllowedExtensions.Count(s => filename.ToLower().Contains(s.ToLower())) > 0;
        }
    }
}
