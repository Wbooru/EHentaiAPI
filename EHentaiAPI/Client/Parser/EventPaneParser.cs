using AngleSharp;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public static class EventPaneParser
    {
        public static string parse(string body)
        {
            string @event = null;
            try
            {
                Document d = Document.parse(body);
                var eventpane = d.getElementById("eventpane");
                if (eventpane != null)
                {
                    @event = eventpane.ToHtml();
                }
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
            return @event;
        }
    }
}
