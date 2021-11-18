using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Utils.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryPageApiParser
    {
        private static readonly Regex PATTERN_IMAGE_URL = new Regex("<img[^>]*src=\"([^\"]+)\" style");
        private static readonly Regex PATTERN_SKIP_HATH_KEY = new Regex("onclick=\"return nl\\('([^\\)]+)'\\)");
        private static readonly Regex PATTERN_ORIGIN_IMAGE_URL = new Regex("<a href=\"([^\"]+)fullimg.php([^\"]+)\">");


        public static Result Parse(string body)
        {
            try
            {
                Match m;
                Result result = new Result();

                var jo = JsonConvert.DeserializeObject<JObject>(body);
                if (jo.ContainsKey("error"))
                {
                    throw new ParseException(jo.GetString("error"), body);
                }

                string i3 = jo.GetString("i3");
                m = PATTERN_IMAGE_URL.Match(i3);
                if (m.Success)
                {
                    result.imageUrl = ParserUtils.UnescapeXml(ParserUtils.Trim(m.Groups[1].Value));
                }
                string i6 = jo.GetString("i6");
                m = PATTERN_SKIP_HATH_KEY.Match(i6);
                if (m.Success)
                {
                    result.skipHathKey = ParserUtils.UnescapeXml(ParserUtils.Trim(m.Groups[1].Value));
                }
                string i7 = jo.GetString("i7");
                m = PATTERN_ORIGIN_IMAGE_URL.Match(i7);
                if (m.Success)
                {
                    result.originImageUrl = ParserUtils.UnescapeXml(m.Groups[1].Value) + "fullimg.php" + ParserUtils.UnescapeXml(m.Groups[2].Value);
                }

                if (!string.IsNullOrWhiteSpace(result.imageUrl))
                {
                    return result;
                }
                else
                {
                    throw new ParseException("Parse image url and skip hath key error", body);
                }
            }
            catch (Exception e)
            {
                throw new ParseException("Can't parse json", body, e);
            }
        }

        public class Result
        {
            public string imageUrl;
            public string skipHathKey;
            public string originImageUrl;
        }
    }
}
