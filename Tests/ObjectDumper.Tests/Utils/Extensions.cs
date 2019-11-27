using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectDumping.Tests.Utils
{
    public static class Extensions
    {
        public static HtmlDocument ToHtmlDocument(this string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            if (htmlDoc.ParseErrors.Count() > 0)
            {
                var ex = new FormatException("html string must not contain parse erros");
                ex.Data.Add("ParseErrors", htmlDoc.ParseErrors);
                throw ex;
            }
            return htmlDoc;
        }
    }
}
