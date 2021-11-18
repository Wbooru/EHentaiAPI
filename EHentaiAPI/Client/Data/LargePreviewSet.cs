using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class LargePreviewSet : PreviewSet
    {
        public List<int> PositionList { get; private set; }
        public List<string> ImageUrlList { get; private set; }
        public List<string> PageUrlList { get; private set; }

        public LargePreviewSet()
        {
            PositionList = new();
            ImageUrlList = new();
            PageUrlList = new();
        }

        public void AddItem(int index, string imageUrl, string pageUrl)
        {
            PositionList.Add(index);
            ImageUrlList.Add(imageUrl);
            PageUrlList.Add(pageUrl);
        }

        public override int Size => ImageUrlList.Count;

        public override int GetPosition(int index)
        {
            return PositionList[(index)];
        }

        public override string GetPageUrlAt(int index)
        {
            return PageUrlList[(index)];
        }

        public override GalleryPreview GetGalleryPreview(long gid, int index)
        {
            var galleryPreview = new GalleryPreview();
            galleryPreview.position = PositionList[(index)];
            galleryPreview.imageKey = EhCacheKeyFactory.GetLargePreviewKey(gid, galleryPreview.position);
            galleryPreview.imageUrl = ImageUrlList[(index)];
            galleryPreview.pageUrl = PageUrlList[(index)];
            return galleryPreview;
        }
    }
}
