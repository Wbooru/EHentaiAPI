using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class TorrentParser
    {
        private static readonly Regex PATTERN_TORRENT = new Regex("<td colspan=\"5\"> &nbsp; <a href=\".*\" onclick=\"document.location='([^\"]+)'[^<]+>([^<]+)</a></td>");

        public static KeyValuePair<String, String>[] parse(String body)
        {
            List<KeyValuePair<String, String>> torrentList = new();
            var m = PATTERN_TORRENT.Match(body);
            while (m.Success)
            {
                String url = ParserUtils.trim(m.Groups[(1)].Value);
                String name = ParserUtils.trim(m.Groups[(2)].Value);
                var item = KeyValuePair.Create(url, name);
                torrentList.Add(item);
                m = m.NextMatch();
            }
            return torrentList.ToArray();
        }
    }
}
