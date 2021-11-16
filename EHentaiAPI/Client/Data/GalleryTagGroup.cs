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

        public void addTag(string tag)
        {
            mTagList.Add(tag);
        }

        public int size()
        {
            return mTagList.Count;
        }

        public string getTagAt(int position)
        {
            return mTagList[(position)];
        }

        public int describeContents()
        {
            return 0;
        }

    }
}
