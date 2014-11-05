using System;
using System.IO;

namespace NWebCrawlerLib.Common
{
    /// <summary>
    /// 存储Web文件在本地文件系统
    /// </summary>
    public class FileSystemUtility
    {
        /// <summary>
        /// 文件保存路径
        /// </summary>
        private static string fileFolder = MemCache.FileSystemFolder;

        /// <summary>
        /// 下载网络文件
        /// 根据url进行命名
        /// 此处可以加以判断，如果是自己需要的类型就将其保存，否则不保存到本地文件系统中
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resource">下载的数据byte数组</param>
        public static void StoreWebFile(string url, byte[] resource)
        {
            FileStream fs = null;
            string filePath = null;
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
            try
            {
                string fileName = url.Replace("/", "_").Replace(":", "").Replace("?", "");
                // TODO:对文件名加以判断
                if (CheckFileName(ref fileName))
                {
                    filePath = Path.Combine(fileFolder, fileName);
                    fs = new FileStream(filePath, FileMode.Create);

                    fs.Write(resource, 0, resource.Length);
                    fs.Flush();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + e.StackTrace);
                Logger.Error(filePath);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 对文件名加以判断
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckFileName(ref string name)
        {
            //1、是否包含非法字符
            //2、长度是否合法
            //3、是否是自己需要的类型
            return true;
        }
    }
}
