using EHentaiAPI.Client.Data;
using EHentaiAPI.Utils.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryApiParser
    {

        public static void Parse(Settings settings, string body, List<GalleryInfo> galleryInfoList)
        {
            var jo = JsonConvert.DeserializeObject<JObject>(body);
            var ja = jo.GetValue("gmetadata") as JArray;

            for (int i = 0, length = ja.Count; i < length; i++)
            {
                var g = ja[i] as JObject;
                long gid = g[("gid")].ToObject<long>();
                GalleryInfo gi = GetGalleryInfoByGid(galleryInfoList, gid);
                if (gi == null)
                    continue;
                gi.Title = ParserUtils.Trim(g.GetString("title"));
                gi.TitleJpn = ParserUtils.Trim(g.GetString("title_jpn"));
                gi.Category = EhUtils.GetCategory(g.GetString("category"));
                gi.Thumb = EhUtils.HandleThumbUrlResolution(settings,g.GetString("thumb"));
                gi.Uploader = g.GetString("uploader");
                gi.Posted = ParserUtils.FormatDate(ParserUtils.ParseLong(g.GetString("posted"), 0) * 1000);
                gi.Rating = ParserUtils.ParseFloat(g.GetString("rating"), 0.0f);
                // tags
                var tagJa = g.GetJSONArray("tags");
                int tagLength = tagJa.Count;
                string[] tags = new string[tagLength];
                for (int j = 0; j < tagLength; j++)
                {
                    tags[j] = tagJa.GetString(j.ToString());
                }
                gi.SimpleTags = tags;
                gi.Pages = ParserUtils.ParseInt(g.GetString("filecount"), 0);
                gi.GenerateSLang();
            }
        }

        private static GalleryInfo GetGalleryInfoByGid(List<GalleryInfo> galleryInfoList, long gid)
        {
            for (int i = 0, size = galleryInfoList.Count; i < size; i++)
            {
                GalleryInfo gi = galleryInfoList[i];
                if (gi.Gid == gid)
                {
                    return gi;
                }
            }
            return null;
        }
    }
}
