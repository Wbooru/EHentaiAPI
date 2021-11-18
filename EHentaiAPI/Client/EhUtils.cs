using EHentaiAPI.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public class EhUtils
    {
        public const int NONE = -1; // Use it for homepage
        public const int UNKNOWN = 0x400;

        public const int ALL_CATEGORY = EhUtils.UNKNOWN - 1;
        //DOUJINSHI|MANGA|ARTIST_CG|GAME_CG|WESTERN|NON_H|IMAGE_SET|COSPLAY|ASIAN_PORN|MISC;

        public const uint BG_COLOR_DOUJINSHI = 0xfff44336;
        public const uint BG_COLOR_MANGA = 0xffff9800;
        public const uint BG_COLOR_ARTIST_CG = 0xfffbc02d;
        public const uint BG_COLOR_GAME_CG = 0xff4caf50;
        public const uint BG_COLOR_WESTERN = 0xff8bc34a;
        public const uint BG_COLOR_NON_H = 0xff2196f3;
        public const uint BG_COLOR_IMAGE_SET = 0xff3f51b5;
        public const uint BG_COLOR_COSPLAY = 0xff9c27b0;
        public const uint BG_COLOR_ASIAN_PORN = 0xff9575cd;
        public const uint BG_COLOR_MISC = 0xfff06292;
        public const uint BG_COLOR_UNKNOWN = 0x00000000;

        // Remove [XXX], (XXX), {XXX}, ~XXX~ stuff
        public static readonly Regex PATTERN_TITLE_PREFIX = new Regex(
            "^(?:(?:\\([^\\)]*\\))|(?:\\[[^\\]]*\\])|(?:\\{[^\\}]*\\})|(?:~[^~]*~)|\\s+)*");
        // Remove [XXX], (XXX), {XXX}, ~XXX~ stuff and something like ch. 1-23
        public static readonly Regex PATTERN_TITLE_SUFFIX = new Regex(
                "(?:\\s+ch.[\\s\\d-]+)?(?:(?:\\([^\\)]*\\))|(?:\\[[^\\]]*\\])|(?:\\{[^\\}]*\\})|(?:~[^~]*~)|\\s+)*$",
                 RegexOptions.IgnoreCase);

        private static readonly int[] CATEGORY_VALUES = {
            EhConfig.MISC,
            EhConfig.DOUJINSHI,
            EhConfig.MANGA,
            EhConfig.ARTIST_CG,
            EhConfig.GAME_CG,
            EhConfig.IMAGE_SET,
            EhConfig.COSPLAY,
            EhConfig.ASIAN_PORN,
            EhConfig.NON_H,
            EhConfig.WESTERN,
            UNKNOWN};

        private static readonly string[][] CATEGORY_STRINGS = {
            new string[]{"misc"},
            new string[]{"doujinshi"},
            new string[] { "manga" },
            new string[] { "artistcg", "Artist CG Sets", "Artist CG" },
            new string[] { "gamecg", "Game CG Sets", "Game CG" },
            new string[] { "imageset", "Image Sets", "Image Set" },
            new string[] { "cosplay" },
            new string[] { "asianporn", "Asian Porn" },
            new string[] { "non-h" },
            new string[] { "western" },
            new string[] { "unknown" }
    };

        public static int GetCategory(string type)
        {
            int i;
            for (i = 0; i < CATEGORY_STRINGS.Length - 1; i++)
            {
                foreach (string str in CATEGORY_STRINGS[i])
                    if (str.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                        return CATEGORY_VALUES[i];
            }

            return CATEGORY_VALUES[i];
        }

        public static string GetCategory(int type)
        {
            int i;
            for (i = 0; i < CATEGORY_VALUES.Length - 1; i++)
            {
                if (CATEGORY_VALUES[i] == type)
                    break;
            }
            return CATEGORY_STRINGS[i][0];
        }

        public static uint GetCategoryColor(int category)
        {
            switch (category)
            {
                case EhConfig.DOUJINSHI:
                    return BG_COLOR_DOUJINSHI;
                case EhConfig.MANGA:
                    return BG_COLOR_MANGA;
                case EhConfig.ARTIST_CG:
                    return BG_COLOR_ARTIST_CG;
                case EhConfig.GAME_CG:
                    return BG_COLOR_GAME_CG;
                case EhConfig.WESTERN:
                    return BG_COLOR_WESTERN;
                case EhConfig.NON_H:
                    return BG_COLOR_NON_H;
                case EhConfig.IMAGE_SET:
                    return BG_COLOR_IMAGE_SET;
                case EhConfig.COSPLAY:
                    return BG_COLOR_COSPLAY;
                case EhConfig.ASIAN_PORN:
                    return BG_COLOR_ASIAN_PORN;
                case EhConfig.MISC:
                    return BG_COLOR_MISC;
                default:
                    return BG_COLOR_UNKNOWN;
            }
        }

        public static string ExtractTitle(string title)
        {
            if (null == title)
            {
                return null;
            }
            title = PATTERN_TITLE_PREFIX.Replace(title, "", 1);
            title = PATTERN_TITLE_SUFFIX.Replace(title, "", 1);
            // Sometimes title is combined by romaji and english translation.
            // Only need romaji.
            // TODO But not sure every '|' means that
            int index = title.IndexOf('|');
            if (index >= 0)
            {
                title = title.Substring(0, index - 0);
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }
            else
            {
                return title;
            }
        }

        public static string HandleThumbUrlResolution(Settings settings, string url)
        {
            if (null == url)
            {
                return null;
            }

            string resolution;
            switch (settings.GetThumbResolution())
            {
                default:
                case 0: // Auto
                    return url;
                case 1: // 250
                    resolution = "250";
                    break;
                case 2: // 300
                    resolution = "300";
                    break;
            }

            int index1 = url.LastIndexOf('_');
            int index2 = url.LastIndexOf('.');
            if (index1 >= 0 && index2 >= 0 && index1 < index2)
            {
                return url.Substring(0, index1 + 1) + resolution + url.Substring(index2);
            }
            else
            {
                return url;
            }
        }
    }
}
