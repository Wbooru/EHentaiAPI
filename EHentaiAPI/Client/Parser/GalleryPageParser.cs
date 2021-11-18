using EHentaiAPI.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryPageParser
    {

        private readonly static Regex PATTERN_IMAGE_URL = new Regex("<img[^>]*src=\"([^\"]+)\" style");
        private readonly static Regex PATTERN_SKIP_HATH_KEY = new Regex("onclick=\"return nl\\('([^\\)]+)'\\)");
        private readonly static Regex PATTERN_ORIGIN_IMAGE_URL = new Regex("<a href=\"([^\"]+)fullimg.php([^\"]+)\">");
        // TODO Not sure about the size of show keys
        private readonly static Regex PATTERN_SHOW_KEY = new Regex("var showkey=\"([0-9a-z]+)\";");


        public static Result Parse(string body)
        {
            Match m;
            Result result = new Result();
            m = PATTERN_IMAGE_URL.Match(body);
            if (m.Success)
            {
                result.imageUrl = ParserUtils.UnescapeXml(ParserUtils.Trim(m.Groups[1].Value));
            }
            m = PATTERN_SKIP_HATH_KEY.Match(body);
            if (m.Success)
            {
                result.skipHathKey = ParserUtils.UnescapeXml(ParserUtils.Trim(m.Groups[1].Value));
            }
            m = PATTERN_ORIGIN_IMAGE_URL.Match(body);
            if (m.Success)
            {
                result.originImageUrl = ParserUtils.UnescapeXml(m.Groups[1].Value) + "fullimg.php" + ParserUtils.UnescapeXml(m.Groups[2].Value);
            }
            m = PATTERN_SHOW_KEY.Match(body);
            if (m.Success)
            {
                result.showKey = m.Groups[1].Value;
            }

            if (!string.IsNullOrWhiteSpace(result.imageUrl) && !string.IsNullOrWhiteSpace(result.showKey))
            {
                return result;
            }
            else
            {
                throw new ParseException("Parse image url and show error", body);
            }
        }

        public class Result
        {
            public string imageUrl;
            public string skipHathKey;
            public string originImageUrl;
            public string showKey;
        }
    }
}
