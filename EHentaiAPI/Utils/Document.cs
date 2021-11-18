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

        public IElement GetElementById(string v)
        {
            return document.GetElementById(v);
        }

        public static Document Parse(string html) => new Document(html);

        public IElement GetElementByClass(string v)
        {
            return GetElementsByClass(v).FirstOrDefault();
        }

        public IHtmlCollection<IElement> GetElementsByClass(string v)
        {
            return document.GetElementsByClassName(v);
        }

        public IHtmlCollection<IElement> Select(string cssQuery)
        {
            return document.QuerySelectorAll(cssQuery);
        }
    }
}
