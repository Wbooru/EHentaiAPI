using AngleSharp;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using EHentaiAPI.Utils.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public static class EventPaneParser
    {
        public static string Parse(string body)
        {
            string @event = null;
            try
            {
                Document d = Document.Parse(body);
                var eventpane = d.GetElementById("eventpane");
                if (eventpane != null)
                {
                    @event = eventpane.ToHtml();
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return @event;
        }
    }
}
