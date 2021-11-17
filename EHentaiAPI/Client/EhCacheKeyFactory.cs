using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public static class EhCacheKeyFactory
    {
        public static string getThumbKey(long gid)
        {
            return "preview:large:" + gid + ":" + 0; // "thumb:" + gid;
        }

        public static string getNormalPreviewKey(long gid, int index)
        {
            return "preview:normal:" + gid + ":" + index;
        }

        public static string getLargePreviewKey(long gid, int index)
        {
            return "preview:large:" + gid + ":" + index;
        }

        public static string getLargePreviewSetKey(long gid, int index)
        {
            return "large_preview_set:" + gid + ":" + index;
        }

        public static string getImageKey(long gid, int index)
        {
            return "image:" + gid + ":" + index;
        }
    }
}
