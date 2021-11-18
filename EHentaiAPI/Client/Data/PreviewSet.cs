using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public abstract class PreviewSet
    {
        public abstract int Size { get; }

        public abstract int GetPosition(int index);

        public abstract string GetPageUrlAt(int index);

        public abstract GalleryPreview GetGalleryPreview(long gid, int index);

        public override string ToString() => $"{Size} images";
    }
}
