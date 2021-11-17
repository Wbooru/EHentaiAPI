using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryDetailUrlParser
    {

        private static readonly Regex URL_STRICT_PATTERN = new Regex(
            "https?://(?:" + EhUrl.DOMAIN_EX + "|" + EhUrl.DOMAIN_E + "|" + EhUrl.DOMAIN_LOFI + ")/(?:g|mpv)/(\\d+)/([0-9a-f]{10})");

        private static readonly Regex URL_PATTERN = new Regex(
                "(\\d+)/([0-9a-f]{10})(?:[^0-9a-f]|$)");


        public static Result parse(string url)
        {
            return parse(url, true);
        }


        public static Result parse(string url, bool strict)
        {
            if (url == null)
            {
                return null;
            }

            Regex pattern = strict ? URL_STRICT_PATTERN : URL_PATTERN;
            var m = pattern.Match(url);
            if (m.Success)
            {
                var result = new Result();
                result.gid = ParserUtils.parseLong(m.Groups[(1)].Value, -1L);
                result.token = m.Groups[(2)].Value;
                if (result.gid < 0)
                {
                    return null;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public class Result
        {
            public long gid;
            public string token;
        }
    }
}
