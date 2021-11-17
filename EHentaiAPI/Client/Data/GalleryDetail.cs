using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryDetail : GalleryInfo
    {
        public long apiUid = -1L;
        public string apiKey;
        public int torrentCount;
        public string torrentUrl;
        public string archiveUrl;
        public string parent;
        public List<GalleryInfo> newerVersions = new();
        public string visible;
        public string language;
        public string size;
        public int favoriteCount;
        public bool isFavorited;
        public int ratingCount;
        public GalleryTagGroup[] tags;
        public GalleryCommentList comments;
        public int previewPages;
        public PreviewSet previewSet;
        public string url;

        public GalleryDetail()
        {
        }
    }
}
