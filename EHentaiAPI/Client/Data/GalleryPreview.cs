using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryPreview
    {
        public string imageKey;
        public string imageUrl;
        public string pageUrl;
        public int position;

        public GalleryPreview()
        {

        }

        public int GetPosition()
        {
            return position;
        }
    }
}
