using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryPageUrlParser
    {

        private static readonly Regex URL_STRICT_PATTERN = new Regex(
            "https?://(?:" + EhUrl.DOMAIN_EX + "|" + EhUrl.DOMAIN_E + "|" + EhUrl.DOMAIN_LOFI + ")/s/([0-9a-f]{10})/(\\d+)-(\\d+)");

        private static readonly Regex URL_PATTERN = new Regex(
                "([0-9a-f]{10})/(\\d+)-(\\d+)");

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

            var pattern = strict ? URL_STRICT_PATTERN : URL_PATTERN;
            Match m = pattern.Match(url);
            if (m.Success)
            {
                Result result = new Result();
                result.gid = ParserUtils.parseLong(m.Groups[2].Value, -1L);
                result.pToken = m.Groups[1].Value;
                result.page = ParserUtils.parseInt(m.Groups[3].Value, 0) - 1;
                if (result.gid < 0 || result.page < 0)
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
            public string pToken;
            public int page;
        }
    }
}
