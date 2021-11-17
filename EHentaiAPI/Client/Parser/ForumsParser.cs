using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class ForumsParser
    {
        public static String parse(String body)
        {
            try
            {
                Document d = Document.parse(body, EhUrl.URL_FORUMS);
                var userlinks = d.getElementById("userlinks");
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
