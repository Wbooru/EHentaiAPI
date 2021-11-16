using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryPreview
    {
        public String imageKey;
        public String imageUrl;
        public String pageUrl;
        public int position;

        public GalleryPreview()
        {

        }

        public int getPosition()
        {
            return position;
        }

        public int describeContents()
        {
            return 0;
        }
    }
}
