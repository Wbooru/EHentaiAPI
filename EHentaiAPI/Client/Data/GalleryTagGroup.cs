using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryTagGroup
    {
        private List<string> mTagList;
        public string groupName;

        public GalleryTagGroup()
        {
            mTagList = new List<string>();
        }

        public void AddTag(string tag)
        {
            mTagList.Add(tag);
        }

        public int Size()
        {
            return mTagList.Count;
        }

        public string GetTagAt(int position)
        {
            return mTagList[(position)];
        }
    }
}
