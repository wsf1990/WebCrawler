
namespace NWebCrawlerLib
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    /// <summary>
    /// 设置项类
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// 使用ini作为配置文件
        /// </summary>
        private static string ConfigurationFilePath;
        /// <summary>
        /// 当前工作文件夹
        /// </summary>
        private static string folder;
        /// <summary>
        /// 应用名称
        /// </summary>
        private static string AppName = "Crawler";

        static Settings()
        {
            folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ConfigurationFilePath = Path.Combine(folder, "config.ini");
        }

        /// <summary>
        /// 线程数
        /// </summary>
        public static int ThreadCount
        {
            get
            {
                return Convert.ToInt32(GetValue("ThreadCount", 50));
            }
            set
            {
                SetValue("ThreadCount", value);
            }
        }

        /// <summary>
        /// 若队列为空,线程睡觉的时间(秒)
        /// </summary>
        public static int ThreadSleepTimeWhenQueueIsEmpty
        {
            get
            {
                return Convert.ToInt32(GetValue("ThreadSleepTimeWhenQueueIsEmpty", 2));
            }
            set
            {
                SetValue("ThreadSleepTimeWhenQueueIsEmpty", value);
            }
        }

        /// <summary>
        /// 连接超时时间
        /// </summary>
        public static int ConnectionTimeout
        {
            get
            {
                return Convert.ToInt32(GetValue("ConnectionTimeout", 20));
            }
            set
            {
                SetValue("ConnectionTimeout", value);
            }
        }

        /// <summary>
        /// 默认存储方式 (0:文件系统；1:SQLite.)
        /// </summary>
        public static string DataStoreMode
        {
            get
            {
                return Convert.ToString(GetValue("DataStoreMode", "0"));
            }
            set
            {
                SetValue("DataStoreMode", value);
            }            
        }

        /// <summary>
        /// SQLite文件名
        /// </summary>
        public static string SQLiteDBFolder
        {
            get
            {
                return Convert.ToString(GetValue("SQLiteDBFolder", "crawlerdb.s3db"));
            }
            set
            {
                SetValue("SQLiteDBFolder", value);
            }
        }

        /// <summary>
        /// 下载文件夹
        /// </summary>
        public static string FileSystemFolder
        {
            get
            {
                return Convert.ToString(GetValue("FileSystemFolder", folder));
            }
            set
            {
                SetValue("FileSystemFolder", value);
            }
        }

        /// <summary>
        /// 允许所有html类型 html/*
        /// </summary>
        public static bool AllowAllMimeTypes
        {
            get
            {
                return Convert.ToBoolean(GetValue("AllowAllMimeTypes", true));
            }
            set
            {
                SetValue("AllowAllMimeTypes", value);
            }
        }

        /// <summary>
        /// 会下载的MIME类型,默认只支持网页. 如选多种，用逗号 "," 分隔.
        /// </summary>
        public static string FileMatches
        {
            get
            {
                return Convert.ToString(GetValue("FileMatches", ""));
            }
            set
            {
                SetValue("FileMatches", value);
            }
        }

        /// <summary>
        /// 未使用
        /// </summary>
        public static string HighPriority
        {
            get
            {
                return Convert.ToString(GetValue("HighPriority", ""));
            }
            set
            {
                SetValue("HighPriority", value);
            }
        }

        /// <summary>
        /// 语言
        /// </summary>
        public static string Language
        {
            get
            {
                return Convert.ToString(GetValue("Language", ""));
            }
            set
            {
                SetValue("Language", value);
            }
        }
        /// <summary>
        /// 允许下载的文件或者网页后缀
        /// </summary>
        public static string AllowExtension
        {
            get
            {
                return GetValue("AllowExtension", "").ToString();
            }
            set
            {
                SetValue("AllowExtension", value);
            }
        }
        /// <summary>
        /// 设置ini项
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        static void SetValue(string keyName, object value)
        {
            NativeMethods.WritePrivateProfileString(AppName, keyName, value.ToString(), ConfigurationFilePath);
        }
        /// <summary>
        /// 读取ini项
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static object GetValue(string keyName, object defaultValue)
        {
            StringBuilder retVal = new StringBuilder(1024);
            NativeMethods.GetPrivateProfileString(AppName, keyName, defaultValue.ToString(), retVal, 1024, ConfigurationFilePath);
            return retVal.ToString();
        }
    }
    /// <summary>
    /// 调用kernel32中的函数进行ini读写操作
    /// </summary>
    class NativeMethods
    {
        [DllImport("kernel32")]
        internal static extern long WritePrivateProfileString(
            string appName,
            string keyName, 
            string value, 
            string fileName);
        
        [DllImport("kernel32")]
        internal static extern int GetPrivateProfileString(
            string appName,
            string keyName, 
            string _default, 
            StringBuilder returnedValue,
            int size, 
            string fileName);       

    }
}
