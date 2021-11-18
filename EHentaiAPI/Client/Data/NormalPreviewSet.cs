using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class NormalPreviewSet : PreviewSet
    {
        private List<int> mPositionList = new List<int>();
        private List<string> mImageKeyList = new();
        private List<string> mImageUrlList = new();
        private List<int> mOffsetXList = new List<int>();
        private List<int> mOffsetYList = new List<int>();
        private List<int> mClipWidthList = new List<int>();
        private List<int> mClipHeightList = new List<int>();
        private List<string> mPageUrlList = new();

        public NormalPreviewSet()
        {
        }
        private string GetImageKey(string imageUrl)
        {
            int index = imageUrl.IndexOf('/');
            if (index >= 0)
            {
                return imageUrl.Substring(index + 1);
            }
            else
            {
                return imageUrl;
            }
        }

        public void AddItem(int position, string imageUrl, int xOffset, int yOffset, int width,
                            int height, string pageUrl)
        {
            mPositionList.Add(position);
            mImageKeyList.Add(GetImageKey(imageUrl));
            mImageUrlList.Add(imageUrl);
            mOffsetXList.Add(xOffset);
            mOffsetYList.Add(yOffset);
            mClipWidthList.Add(width);
            mClipHeightList.Add(height);
            mPageUrlList.Add(pageUrl);
        }


        public override int Size()
        {
            return mPositionList.Count;
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
            galleryPreview.imageKey = mImageKeyList[(index)];
            galleryPreview.imageUrl = mImageUrlList[(index)];
            galleryPreview.pageUrl = mPageUrlList[(index)];
            return galleryPreview;
        }
    }
}
