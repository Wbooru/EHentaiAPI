using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI
{
    internal static class AngleHtmlExtendMethod
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
    }
}
