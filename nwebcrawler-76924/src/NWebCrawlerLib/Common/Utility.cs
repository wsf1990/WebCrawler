using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace NWebCrawlerLib
{
    /// <summary>
    /// 工具帮助类
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// url的哈希值
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Hash(string url)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] bs = Encoding.UTF8.GetBytes(url);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }

            return s.ToString();
        }

        /// <summary>
        /// URL标准化处理
        /// foamliu, 2009/12/27.
        /// 爬虫需要两个URL是否指向相同的页面这一点可以被迅速检测出来, 这就需要URL规范化.
        /// URL规范化做的主要的事情:
        /// 转换为小写
        /// 相对URL转换成绝对URL
        /// 删除默认端口号
        /// 根目录添加斜杠
        /// 猜测的目录添加尾部斜杠
        /// 删除分块
        /// 解析路径
        /// 删除缺省名字
        /// 解码禁用字符
        /// 更多信息参照RFC3986:
        /// http://tools.ietf.org/html/rfc3986
        /// </summary>
        /// <param name="strURL"></param>
        public static void Normalize(string baseUri, ref string strUri)
        {
            // 相对URL转换成绝对URL
            if (strUri.StartsWith("/"))
            {
                strUri = baseUri + strUri.Substring(1);
            }

            // 当查询字符串为空时去掉问号"?"
            if (strUri.EndsWith("?"))
                strUri = strUri.Substring(0, strUri.Length - 1);

            // 转换为小写
            strUri = strUri.ToLower();

            // 删除默认端口号
            // 解析路径
            // 解码转义字符
            Uri tempUri = new Uri(strUri);
            strUri = tempUri.ToString();

            // 根目录添加斜杠
            int posTailingSlash = strUri.IndexOf("/", 8);
            if (posTailingSlash == -1)
                strUri += '/';

            // 猜测的目录添加尾部斜杠
            if (posTailingSlash != -1 && !strUri.EndsWith("/") && strUri.IndexOf(".", posTailingSlash) == -1)
                strUri += '/';

            // 删除分块
            int posFragment = strUri.IndexOf("#");
            if (posFragment != -1)
            {
                strUri = strUri.Substring(0, posFragment);
            }

            // 删除缺省名字
            string[] DefaultDirectoryIndexes = 
            {
                "index.html",
                "default.asp",
                "default.aspx",
            };
            foreach (string index in DefaultDirectoryIndexes)
            {
                if (strUri.EndsWith(index))
                {
                    strUri = strUri.Substring(0, (strUri.Length - index.Length));
                    break;
                }
            }
        }
        /// <summary>
        /// URL标准化处理
        /// </summary>
        /// <param name="strUri"></param>
        public static void Normalize(ref string strUri)
        {
            Normalize(string.Empty, ref strUri);
        }
        /// <summary>
        /// 获取基本uri?
        /// HOST
        /// </summary>
        /// <param name="strUri"></param>
        /// <returns></returns>
        public static string GetBaseUri(string strUri)
        {
            Uri uri = new Uri(strUri);
            string port = uri.IsDefaultPort ? string.Empty : port = ":" + uri.Port;
            return uri.Scheme + "://" + uri.Host + port + "/";
        }
        /// <summary>
        /// 获取扩展类型
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static string GetExtensionByMimeType(string mimeType)
        {
            int pos;
            if ((pos = mimeType.IndexOf('/')) != -1)
            {
                return mimeType.Substring(pos + 1);
            }
            return string.Empty;
        }

        /// <summary>
        /// 执行控制台命令  如cmd等
        /// </summary>
        /// <param name="fileName">控制台 exe文件路径</param>
        /// <param name="arguments">参数</param>
        public static void ExecuteCommandSync(string fileName, string arguments)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(fileName, arguments);
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                procStartInfo.RedirectStandardOutput = false;
                procStartInfo.RedirectStandardInput = false;

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 从流中获取字符串
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetStringFromResponse(WebResponse response)
        {
            using (var sw = new StreamReader(response.GetResponseStream()))
            {
                return sw.ReadToEnd();
            }
        }

        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        public static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024 * 1024];
            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                using (Stream outStream = System.IO.File.Create(FileName))
                {
                    using (Stream inStream = response.GetResponseStream())
                    {
                        int l;
                        do
                        {
                            l = inStream.Read(buffer, 0, buffer.Length);
                            if (l > 0)
                                outStream.Write(buffer, 0, l);
                        }
                        while (l > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Value = false;
            }
            return Value;
        } 

    }
}
