using EHentaiAPI.Client.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class NormalPreviewSet : PreviewSet
    {
        public List<int> PositionList { get; private set; } = new();
        public List<string> ImageKeyList { get; private set; } = new();
        public List<string> ImageUrlList { get; private set; } = new();
        public List<string> PageUrlList { get; private set; } = new();

        private static string GetImageKey(string imageUrl)
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

        public void AddItem(int position, string imageUrl, string pageUrl)
        {
            PositionList.Add(position);
            ImageKeyList.Add(GetImageKey(imageUrl));
            ImageUrlList.Add(imageUrl);
            PageUrlList.Add(pageUrl);
        }

        public override int Size => PositionList.Count;

        public override int GetPosition(int index)
        {
            return PositionList[index];
        }

        public override string GetPageUrlAt(int index)
        {
            return PageUrlList[index];
        }

        public override GalleryPreview GetGalleryPreview(long gid, int index)
        {
            var galleryPreview = new GalleryPreview();
            galleryPreview.Position = PositionList[index];
            galleryPreview.ImageKey = ImageKeyList[index];
            galleryPreview.ImageUrl = ImageUrlList[index];
            galleryPreview.PageUrl = PageUrlList[index];
            galleryPreview.PToken = GalleryPageUrlParser.Parse(galleryPreview.PageUrl).pToken;

            return galleryPreview;
        }
    }
}
