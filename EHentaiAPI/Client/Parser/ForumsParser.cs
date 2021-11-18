using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Utils;
using EHentaiAPI.Utils.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class ForumsParser
    {
        public static string Parse(string body)
        {
            try
            {
                Document d = Document.Parse(body);
                var userlinks = d.GetElementById("userlinks");
                var child = userlinks.Children[0].Children[0].Children[0];
                return child.GetAttributeEx("href");
            }
            catch (Exception)
            {
                throw new ParseException("Parse forums error", body);
            }
        }
    }
}
