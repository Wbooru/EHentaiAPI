using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    internal class ParserUtils
    {
        private static readonly string[] ESCAPE_CHARATER_LIST = {
            "&amp;",
            "&lt;",
            "&gt;",
            "&quot;",
            "&#039;",
            "&times;",
            "&nbsp;"
        };

        private static readonly string[] UNESCAPE_CHARATER_LIST = {
            "&",
            "<",
            ">",
            "\"",
            "'",
            "×",
            " "
        };

        public static string UnescapeXml(string str)
        {
            for (int i = 0; i < ESCAPE_CHARATER_LIST.Length; i++)
            {
                str = str.Replace(ESCAPE_CHARATER_LIST[i], UNESCAPE_CHARATER_LIST[i]);
            }
            return str;
        }

        public static string Trim(string str)
        {
            // Avoid null
            if (str == null)
            {
                str = "";
            }
            return UnescapeXml(str).Trim();
        }

        public static int ParseInt(string p, int v)
        {
            return int.TryParse(p.Trim(), out var d) ? d : v;
        }

        public static string FormatDate(long v)
        {
            //todo
            return "miaomiao";
        }

        public static long ParseLong(string p, long v)
        {
            return long.TryParse(p.Trim(), out var d) ? d : v;
        }

        public static float ParseFloat(string p, float v)
        {
            return float.TryParse(p.Trim(), out var d) ? d : v;
        }
    }
}
