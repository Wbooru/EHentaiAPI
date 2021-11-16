using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public abstract class PreviewSet
    {
        public abstract int size();

        public abstract int getPosition(int index);

        public abstract String getPageUrlAt(int index);

        public abstract GalleryPreview getGalleryPreview(long gid, int index);
    }
}
