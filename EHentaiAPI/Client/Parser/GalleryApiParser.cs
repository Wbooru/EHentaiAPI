using EHentaiAPI.Client.Data;
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

        public static void parse(Settings settings, String body, List<GalleryInfo> galleryInfoList)
        {
            var jo = new JObject(body);
            var ja = jo.GetValue("gmetadata") as JArray;

            for (int i = 0, length = ja.Count; i < length; i++)
            {
                var g = ja[(i)] as JObject;
                long gid = g[("gid")].ToObject<long>();
                GalleryInfo gi = getGalleryInfoByGid(galleryInfoList, gid);
                if (gi == null)
                    continue;
                gi.title = ParserUtils.trim(g.getString("title"));
                gi.titleJpn = ParserUtils.trim(g.getString("title_jpn"));
                gi.category = EhUtils.getCategory(g.getString("category"));
                gi.thumb = EhUtils.handleThumbUrlResolution(settings,g.getString("thumb"));
                gi.uploader = g.getString("uploader");
                gi.posted = ParserUtils.formatDate(ParserUtils.parseLong(g.getString("posted"), 0) * 1000);
                gi.rating = ParserUtils.parseFloat(g.getString("rating"), 0.0f);
                // tags
                var tagJa = g.getJSONArray("tags");
                int tagLength = tagJa.Count;
                String[] tags = new String[tagLength];
                for (int j = 0; j < tagLength; j++)
                {
                    tags[j] = tagJa.getString(j.ToString());
                }
                gi.simpleTags = tags;
                gi.pages = ParserUtils.parseInt(g.getString("filecount"), 0);
                gi.generateSLang();
            }
        }

        private static GalleryInfo getGalleryInfoByGid(List<GalleryInfo> galleryInfoList, long gid)
        {
            for (int i = 0, size = galleryInfoList.Count; i < size; i++)
            {
                GalleryInfo gi = galleryInfoList[(i)];
                if (gi.gid == gid)
                {
                    return gi;
                }
            }
            return null;
        }
    }
}
