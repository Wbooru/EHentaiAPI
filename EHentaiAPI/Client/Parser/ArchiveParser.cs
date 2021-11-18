using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public static class ArchiveParser
    {
        private readonly static Regex PATTERN_FORM = new Regex("<form id=\"hathdl_form\" action=\"[^\"]*?or=([^=\"]*?)\" method=\"post\">");
        private readonly static Regex PATTERN_ARCHIVE = new Regex("<a href=\"[^\"]*\" onclick=\"return do_hathdl\\('([0-9]+|org)'\\)\">([^<]+)</a>");

        public static KeyValuePair<string, KeyValuePair<string, string>[]> Parse(string body)
        {
            var m = PATTERN_FORM.Match(body);
            if (!m.Success)
            {
                return new ("", Array.Empty<KeyValuePair<string, string>>());
            }
            string paramOr = m.Groups[1].Value;
            List<KeyValuePair<string, string>> archiveList = new();
            m = PATTERN_ARCHIVE.Match(body);
            while (m.Success)
            {
                string res = ParserUtils.Trim(m.Groups[1].Value);
                string name = ParserUtils.Trim(m.Groups[2].Value);
                var item = KeyValuePair.Create(res, name);
                archiveList.Add(item);

                m = m.NextMatch();
            }
            return KeyValuePair.Create(paramOr, archiveList.ToArray());
        }
    }
}
