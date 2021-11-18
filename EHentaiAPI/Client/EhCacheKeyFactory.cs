using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public static class EhCacheKeyFactory
    {
        public static string GetThumbKey(long gid)
        {
            return "preview:large:" + gid + ":" + 0; // "thumb:" + gid;
        }

        public static string GetNormalPreviewKey(long gid, int index)
        {
            return "preview:normal:" + gid + ":" + index;
        }

        public static string GetLargePreviewKey(long gid, int index)
        {
            return "preview:large:" + gid + ":" + index;
        }

        public static string GetLargePreviewSetKey(long gid, int index)
        {
            return "large_preview_set:" + gid + ":" + index;
        }

        public static string GetImageKey(long gid, int index)
        {
            return "image:" + gid + ":" + index;
        }
    }
}
