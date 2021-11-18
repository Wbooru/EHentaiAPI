using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class LargePreviewSet : PreviewSet
    {
        private List<int> mPositionList;
        private List<string> mImageUrlList;
        private List<string> mPageUrlList;

        public LargePreviewSet()
        {
            mPositionList = new();
            mImageUrlList = new();
            mPageUrlList = new();
        }

        public void AddItem(int index, string imageUrl, string pageUrl)
        {
            mPositionList.Add(index);
            mImageUrlList.Add(imageUrl);
            mPageUrlList.Add(pageUrl);
        }

        public override int Size()
        {
            return mImageUrlList.Count;
        }

        public override int GetPosition(int index)
        {
            return mPositionList[(index)];
        }

        public override string GetPageUrlAt(int index)
        {
            return mPageUrlList[(index)];
        }

        public override GalleryPreview GetGalleryPreview(long gid, int index)
        {
            GalleryPreview galleryPreview = new GalleryPreview();
            galleryPreview.position = mPositionList[(index)];
            galleryPreview.imageKey = EhCacheKeyFactory.GetLargePreviewKey(gid, galleryPreview.position);
            galleryPreview.imageUrl = mImageUrlList[(index)];
            galleryPreview.pageUrl = mPageUrlList[(index)];
            return galleryPreview;
        }
    }
}
