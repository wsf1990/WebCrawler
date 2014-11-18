﻿
namespace NWebCrawlerLib
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NWebCrawlerLib.Common;

    /// <summary>
    /// 页面解析.
    /// HTML代码不需要像程序语言那样经过严格的语法检查, 而且有为数众多的非专业网页编辑人员的存在, 
    /// 因此爬虫的页面解析必须足够宽容, 不能因为小小错误而把许多重要的网页丢弃.
    /// 
    /// 下面的简单实现只提取页面的链接信息.
    /// 它会去:
    /// 1.寻找相应标签并获取其href属性值.
    /// 
    /// 它不会去:
    /// 1.区分静态和动态页面, 更加不会自主构造查询URL.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// 取出html中的链接
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string[] ExtractLinks(string baseUri, string html)
        {
            var urls = new Collection<string>();

            try
            {
                string strRef = @"(href|HREF|src|SRC)[ ]*=[ ]*[""'][^""'#>]+[""']";
                MatchCollection matches = new Regex(strRef).Matches(html);
                
                foreach (Match match in matches)
                {
                    strRef = match.Value.Substring(match.Value.IndexOf('=') + 1).Trim('"', '\'', '#', ' ', '>', '{', '}');
                    try
                    {
                        if (IsGoodUri(strRef))
                        {
                            Utility.Normalize(baseUri, ref strRef);
                            urls.Add(strRef);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Logger.Info("Found: " + urls.Count + " ref(s)\r\n");

            return urls.ToArray();
        }

        /// <summary>
        /// 判断是否是javascript:
        /// </summary>
        /// <param name="strUri"></param>
        /// <returns></returns>
        static bool IsGoodUri(string strUri)
        {
            if (strUri.ToLower().StartsWith("javascript:"))
                return false;
            return true;
        }
       
    }
}
