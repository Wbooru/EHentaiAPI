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
        public String apiKey;
        public int torrentCount;
        public String torrentUrl;
        public String archiveUrl;
        public String parent;
        public List<GalleryInfo> newerVersions = new();
        public String visible;
        public String language;
        public String size;
        public int favoriteCount;
        public bool isFavorited;
        public int ratingCount;
        public GalleryTagGroup[] tags;
        public GalleryCommentList comments;
        public int previewPages;
        public PreviewSet previewSet;

        public GalleryDetail()
        {
        }
    }
}
