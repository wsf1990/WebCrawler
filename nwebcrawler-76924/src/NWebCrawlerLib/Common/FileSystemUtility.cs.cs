using System;
using System.IO;
using System.Net;

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
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        public static void StoreWebFile(string url)
        {
            string filePath = null;
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
            try
            {
                string fileName = url;
                // TODO:对文件名加以判断
                if (CheckFileName(ref fileName))
                {
                    filePath = Path.Combine(fileFolder, fileName);
                    var wc = new WebClient();
                    wc.DownloadFile(url, filePath);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + e.StackTrace);
                Logger.Error(filePath);
            }
        }

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
                string fileName = url;
                // TODO:对文件名加以判断
                if (CheckFileName(ref fileName))
                {
                    filePath = Path.Combine(fileFolder, fileName);
                    fs = new FileStream(filePath, FileMode.Create);
                    //此处若responseStream有问题，可以使用url重新下载之
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
        /// 对文件名加以判断并返回正确文件名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckFileName(ref string name)
        {
            //1、允许文件或者文件夹名称不得超过255个字符。
            //2、文件名除了开头之外任何地方都可以使用空格。
            //3、文件名中不能有下列符号：  :?/\*"<>|
            //TODO:1、长度是否合法，若超过只取前255个
            if (name.Length >= 255)
                name = name.Substring(0, 254);
            //TODO:2、将非法字符转化为_
            string replace = "_";
            name = name.Replace(":", replace)
                       .Replace("/", replace)
                       .Replace("?", replace)
                       .Replace(@"\", replace)
                       .Replace("*", replace)
                       .Replace("\"", replace)
                       .Replace("<", replace)
                       .Replace(">", replace)
                       .Replace("|", replace)
                       .TrimStart();//去除开头无效字符
            //TODO:3、是否是自己需要下载的类型
            //if (name.EndsWith(".js", StringComparison.CurrentCultureIgnoreCase))
            return MemCache.IsAllowedExtensions(name);// true;// IsAllowExt(name);
        }
        ///// <summary>
        ///// 判断是否是允许下载的类型
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //static bool IsAllowExt(string name)
        //{
        //    bool isAllow = false;
        //    MemCache.AllowedExtensions.ForEach(s => 
        //    {
        //        if(name.EndsWith(s, StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            isAllow = true;
        //        }
        //    });
        //    return isAllow;
        //}
    }
}
