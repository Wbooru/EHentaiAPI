using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils.ExtensionMethods
{
    internal static class AngleHtmlExtensionMethod
    {
        public static IElement GetElementByIdRecursive(this IElement d, string id)
        {
            var queue = new Queue<IElement>();
            queue.Enqueue(d);

            while (queue.Count != 0)
            {
                d = queue.Dequeue();

                if (d.Id == id)
                    return d;

                foreach (var child in d.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return default;
        }

        public static string GetAttributeEx(this IElement d, string attr)
        {
            //jousp 那边如果没有此attr则返回空字符串,兼容一下
            return d.GetAttribute(attr) ?? string.Empty;
        }
    }
}
