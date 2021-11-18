using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryDetail : GalleryInfo
    {
        public long ApiUid { get; set; } = -1L;
        public string ApiKey { get; set; }
        public int TorrentCount { get; set; }
        public string TorrentUrl { get; set; }
        public string ArchiveUrl { get; set; }
        public string Parent { get; set; }
        public List<GalleryInfo> NewerVersions { get; set; } = new();
        public string Visible { get; set; }
        public string Language { get; set; }
        public string Size { get; set; }
        public int FavoriteCount { get; set; }
        public bool IsFavorited { get; set; }
        public int RatingCount { get; set; }
        public GalleryTagGroup[] Tags { get; set; }
        public GalleryCommentList Comments { get; set; }
        public int PreviewPages { get; set; }
        public PreviewSet PreviewSet { get; set; }
        public string Url { get; set; }
    }
}
