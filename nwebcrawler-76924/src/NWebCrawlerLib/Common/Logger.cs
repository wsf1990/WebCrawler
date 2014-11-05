using NWebCrawlerLib.Enum;
using NWebCrawlerLib.Event;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace NWebCrawlerLib.Common
{
    public delegate void NewLogEventHandler(object sender, NewLogEventArgs e);

    public delegate void NewErrorEventHandler(object sender, NewErrorEventArgs e);
    
    /// <summary>
    /// 日志记录，此处也可使用Log4net
    /// </summary>
    public class Logger
    {
        const string BaseFileName = "NWebCrawler";
        /// <summary>
        /// 日志所记录的类型
        /// </summary>
        private string DeclaringType;

        public static event NewLogEventHandler NewLogEvent;
        public static event NewErrorEventHandler NewErrorEvent;

        public Logger(Type type)
        {
            if (type != null)
                DeclaringType = type.FullName;
        }
        /// <summary>
        /// 记录日志路径
        /// </summary>
        public static string LogPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BaseFileName + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Year + ".log");
            }
        }
        /// <summary>
        /// 生成日志信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public static void LogMessage(string message, LogLevel logLevel)
        {
            using(StreamWriter writer = File.AppendText(LogPath))
            {
                FormatMessage(ref message);
                writer.WriteLine(message);
            }
        }
        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="message"></param>
        private static void FormatMessage(ref string message)
        {
            int CurrentProcessId = Process.GetCurrentProcess().Id;
            int CurrentThreadId = Thread.CurrentThread.ManagedThreadId;
            string format = "yyyy-MM-dd HH:mm:ss.fff";
            message = String.Format(CultureInfo.InvariantCulture, "{0} [pid:{1}] [tid:{2}] - {3}",
                DateTime.Now.ToString(format, CultureInfo.InvariantCulture), CurrentProcessId, CurrentThreadId, message);
        }

        public static void Info(string format, params object[] args)
        {
            Logger.Info(string.Format(format, args));
        }

        public static void Info(string message)
        {
            FormatMessage(ref message);
            if (NewLogEvent != null)
                NewLogEvent(null, new NewLogEventArgs() { LogMessage = message });
        }

        public static void Error(string format, params object[] args)
        {
            Logger.Error(string.Format(format, args));
        }

        public static void Error(string message)
        {
            FormatMessage(ref message);
            if (NewErrorEvent != null)
            {
                NewErrorEvent(null, new NewErrorEventArgs() { ErrorMessage = message });
            }
        }
    }
}
