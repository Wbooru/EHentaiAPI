using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace EHentaiAPI.Utils
{
    public class Document
    {
        public IHtmlDocument document;

        public Document(string html)
        {
            var d = new HtmlParser();
            document = d.ParseDocument(html);
        }

        public IElement getElementById(string v)
        {
            return document.GetElementById(v);
        }

        public static Document parse(string html, string baseUrl = default, string dasd = default) => new Document(html);

        public IElement getElementByClass(string v)
        {
            return getElementsByClass(v).FirstOrDefault();
        }

        public IHtmlCollection<IElement> getElementsByClass(string v)
        {
            return document.GetElementsByClassName(v);
        }

        public IHtmlCollection<IElement> select(string cssQuery)
        {
            return document.QuerySelectorAll(cssQuery);
        }
    }
}
