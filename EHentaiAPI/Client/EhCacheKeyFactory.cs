using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public static class EhCacheKeyFactory
    {
        public static String getThumbKey(long gid)
        {
            return "preview:large:" + gid + ":" + 0; // "thumb:" + gid;
        }

        public static String getNormalPreviewKey(long gid, int index)
        {
            return "preview:normal:" + gid + ":" + index;
        }

        public static String getLargePreviewKey(long gid, int index)
        {
            return "preview:large:" + gid + ":" + index;
        }

        public static String getLargePreviewSetKey(long gid, int index)
        {
            return "large_preview_set:" + gid + ":" + index;
        }

        public static String getImageKey(long gid, int index)
        {
            return "image:" + gid + ":" + index;
        }
    }
}
