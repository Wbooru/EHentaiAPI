using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryPreview
    {
        public string ImageKey { get; set; }
        public string ImageUrl { get; set; }
        public string PageUrl { get; set; }
        public int Position { get; set; }

        //Added by EHentaiAPI
        public string PToken { get; set; }

        public override string ToString() => $"[{Position}]{ImageUrl}";
    }
}
