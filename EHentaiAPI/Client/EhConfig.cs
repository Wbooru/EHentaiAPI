using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public class EhConfig
    {
        /**
         * The Cookie key of uconfig
         */
        public const string KEY_UCONFIG = "uconfig";
        /**
         * The Cookie key of lofi resolution
         */
        public const string KEY_LOFI_RESOLUTION = "xres";
        /**
         * The Cookie key of show warning
         */
        public const string KEY_CONTENT_WARNING = "nw";
        /**
         * load images through the Hentai@Home Network
         */
        public const string LOAD_FROM_HAH_YES = "y";
        /**
         * do not load images through the Hentai@Home Network
         */
        public const string LOAD_FROM_HAH_NO = "n";
        /**
         * Image Size Auto
         */
        public const string IMAGE_SIZE_AUTO = "a";
        /**
         * Image Size 780x
         */
        public const string IMAGE_SIZE_780X = "780";
        /**
         * Image Size 980x
         */
        public const string IMAGE_SIZE_980X = "980";
        /**
         * Image Size 1280x
         */
        public const string IMAGE_SIZE_1280X = "1280";
        /**
         * Image Size 1600x
         */
        public const string IMAGE_SIZE_1600X = "1600";
        /**
         * Image Size 2400x
         */
        public const string IMAGE_SIZE_2400X = "2400";
        /**
         * Manual Accept, Manual Start
         */
        public const string ARCHIVER_DOWNLOAD_MAMS = "0";
        /**
         * >Manual Accept, Auto Start
         */
        public const string ARCHIVER_DOWNLOAD_AAMS = "1";
        /**
         * Auto Accept, Manual Start
         */
        public const string ARCHIVER_DOWNLOAD_MAAS = "2";
        /**
         * Auto Accept, Auto Start
         */
        public const string ARCHIVER_DOWNLOAD_AAAS = "3";
        /**
         * List View on the front and search pages
         */
        public const string LAYOUT_MODE_LIST = "l";
        /**
         * Thumbnail View on the front and search pages
         */
        public const string LAYOUT_MODE_THUMB = "t";
        public const int MISC = 0x1;
        public const int DOUJINSHI = 0x2;
        public const int MANGA = 0x4;
        public const int ARTIST_CG = 0x8;
        public const int GAME_CG = 0x10;
        public const int IMAGE_SET = 0x20;
        public const int COSPLAY = 0x40;
        public const int ASIAN_PORN = 0x80;
        public const int NON_H = 0x100;
        public const int WESTERN = 0x200;
        public const int ALL_CATEGORY = 0x3ff;
        public const int NAMESPACES_RECLASS = 0x1;
        public const int NAMESPACES_LANGUAGE = 0x2;
        public const int NAMESPACES_PARODY = 0x4;
        public const int NAMESPACES_CHARACTER = 0x8;
        public const int NAMESPACES_GROUP = 0x10;
        public const int NAMESPACES_ARTIST = 0x20;
        public const int NAMESPACES_MALE = 0x40;
        public const int NAMESPACES_FEMALE = 0x80;
        public const string JAPANESE_ORIGINAL = "0";
        public const string JAPANESE_TRANSLATED = "1024";
        public const string JAPANESE_REWRITE = "2048";
        public const string ENGLISH_ORIGINAL = "1";
        public const string ENGLISH_TRANSLATED = "1025";
        public const string ENGLISH_REWRITE = "2049";
        public const string CHINESE_ORIGINAL = "10";
        public const string CHINESE_TRANSLATED = "1034";
        public const string CHINESE_REWRITE = "2058";
        public const string DUTCH_ORIGINAL = "20";
        public const string DUTCH_TRANSLATED = "1044";
        public const string DUTCH_REWRITE = "2068";
        public const string FRENCH_ORIGINAL = "30";
        public const string FRENCH_TRANSLATED = "1054";
        public const string FRENCH_REWRITE = "2078";
        public const string GERMAN_ORIGINAL = "40";
        public const string GERMAN_TRANSLATED = "1064";
        public const string GERMAN_REWRITE = "2088";
        public const string HUNGARIAN_ORIGINAL = "50";
        public const string HUNGARIAN_TRANSLATED = "1074";
        public const string HUNGARIAN_REWRITE = "2098";
        public const string ITALIAN_ORIGINAL = "60";
        public const string ITALIAN_TRANSLATED = "1084";
        public const string ITALIAN_REWRITE = "2108";
        public const string KOREAN_ORIGINAL = "70";
        public const string KOREAN_TRANSLATED = "1094";
        public const string KOREAN_REWRITE = "2118";
        public const string POLISH_ORIGINAL = "80";
        public const string POLISH_TRANSLATED = "1104";
        public const string POLISH_REWRITE = "2128";
        public const string PORTUGUESE_ORIGINAL = "90";
        public const string PORTUGUESE_TRANSLATED = "1114";
        public const string PORTUGUESE_REWRITE = "2138";
        public const string RUSSIAN_ORIGINAL = "100";
        public const string RUSSIAN_TRANSLATED = "1124";
        public const string RUSSIAN_REWRITE = "2148";
        public const string SPANISH_ORIGINAL = "110";
        public const string SPANISH_TRANSLATED = "1134";
        public const string SPANISH_REWRITE = "2158";
        public const string THAI_ORIGINAL = "120";
        public const string THAI_TRANSLATED = "1144";
        public const string THAI_REWRITE = "2168";
        public const string VIETNAMESE_ORIGINAL = "130";
        public const string VIETNAMESE_TRANSLATED = "1154";
        public const string VIETNAMESE_REWRITE = "2178";
        public const string NA_ORIGINAL = "254";
        public const string NA_TRANSLATED = "1278";
        public const string NA_REWRITE = "2302";
        public const string OTHER_ORIGINAL = "255";
        public const string OTHER_TRANSLATED = "1279";
        public const string OTHER_REWRITE = "2303";
        /**
         * 25 results per page for the index/search page and torrent search pages
         */
        public const string RESULT_COUNT_25 = "0";
        /**
         * 50 results per page for the index/search page and torrent search pages
         */
        public const string RESULT_COUNT_50 = "1";
        /**
         * 100 results per page for the index/search page and torrent search pages
         */
        public const string RESULT_COUNT_100 = "2";
        /**
         * 200 results per page for the index/search page and torrent search pages
         */
        public const string RESULT_COUNT_200 = "3";
        /**
         * On mouse-over
         */
        public const string MOUSE_OVER_YES = "m";
        /**
         * On page load
         */
        public const string MOUSE_OVER_NO = "p";
        /**
         * Preview normal size
         */
        public const string PREVIEW_SIZE_NORMAL = "m";
        /**
         * Preview large size
         */
        public const string PREVIEW_SIZE_LARGE = "l";
        /**
         * 4 row preview per page
         */
        public const string PREVIEW_ROW_4 = "2";
        /**
         * 10 row preview per page
         */
        public const string PREVIEW_ROW_10 = "5";
        /**
         * 20 row preview per page
         */
        public const string PREVIEW_ROW_20 = "10";
        /**
         * 40 row preview per page
         */
        public const string PREVIEW_ROW_40 = "20";
        /**
         * Oldest comments first
         */
        public const string COMMENTS_SORT_OLDEST_FIRST = "a";
        /**
         * Recent comments first
         */
        public const string COMMENTS_SORT_RECENT_FIRST = "d";
        /**
         * By highest score
         */
        public const string COMMENTS_SORT_HIGHEST_SCORE_FIRST = "s";
        /**
         * Show gallery comment votes On score hover or click
         */
        public const string COMMENTS_VOTES_POP = "0";
        /**
         * Always show gallery comment votes
         */
        public const string COMMENTS_VOTES_ALWAYS = "1";
        /**
         * Sort order for gallery tags alphabetically
         */
        public const string TAGS_SORT_ALPHABETICAL = "a";
        /**
         * Sort order for gallery tags by tag power
         */
        public const string TAGS_SORT_POWER = "p";
        /**
         * Show gallery page numbers
         */
        public const string SHOW_GALLERY_INDEX_YES = "1";
        /**
         * Do not show gallery page numbers
         */
        public const string SHOW_GALLERY_INDEX_NO = "0";
        /**
         * Enable Tag Flagging
         */
        public const string ENABLE_TAG_FLAGGING_YES = "y";
        /**
         * Do not enable Tag Flagging
         */
        public const string ENABLE_TAG_FLAGGING_NO = "n";
        /**
         * Always display the original images
         */
        public const string ALWAYS_ORIGINAL_YES = "y";
        /**
         * Do not Always display the original images
         */
        public const string ALWAYS_ORIGINAL_NO = "n";
        /**
         * Enable the Multi-Page Viewe
         */
        public const string MULTI_PAGE_YES = "y";
        /**
         * Do not enable the Multi-Page Viewe
         */
        public const string MULTI_PAGE_NO = "n";
        /**
         * Align left, only scale if image is larger than browser width
         */
        public const string MULTI_PAGE_STYLE_N = "n";
        /**
         * Align center, only scale if image is larger than browser width
         */
        public const string MULTI_PAGE_STYLE_C = "c";
        /**
         * Align center, Always scale images to fit browser width
         */
        public const string MULTI_PAGE_STYLE_Y = "y";
        /**
         * Show Multi-Page Viewer Thumbnail Pane
         */
        public const string MULTI_PAGE_THUMB_SHOW = "n";
        /**
         * Hide Multi-Page Viewer Thumbnail Pane
         */
        public const string MULTI_PAGE_THUMB_HIDE = "y";
        /**
         * 460x for lofi resolution
         */
        public const string LOFI_RESOLUTION_460X = "1";
        /**
         * 780X for lofi resolution
         */
        public const string LOFI_RESOLUTION_780X = "2";
        /**
         * 980X for lofi resolution
         */
        public const string LOFI_RESOLUTION_980X = "3";
        /**
         * show warning
         */
        public const string CONTENT_WARNING_SHOW = "0";
        /**
         * not show warning
         */
        public const string CONTENT_WARNING_NOT_SHOW = "1";
        /**
         * Default gallery title
         */
        private const string GALLERY_TITLE_DEFAULT = "r";
        /**
         * Show popular
         */
        private const string POPULAR_YES = "y";
        /**
         * Sort favorites by last gallery update time
         */
        private const string FAVORITES_SORT_FAVORITED_TIME = "f";
        /**
         * Load images through the Hentai@Home Network<br/>
         * key: {@link #KEY_LOAD_FROM_HAH}<br/>
         * value: {@link #LOAD_FROM_HAH_YES}, {@link #LOAD_FROM_HAH_NO}
         */
        public string loadFromHAH = LOAD_FROM_HAH_YES;

        /**
         * Image Size<br/>
         * key: {@link #KEY_IMAGE_SIZE}<br/>
         * value: {@link #IMAGE_SIZE_AUTO}, {@link #IMAGE_SIZE_780X}, {@link #IMAGE_SIZE_980X},
         * {@link #IMAGE_SIZE_1280X}, {@link #IMAGE_SIZE_1600X}, {@link #IMAGE_SIZE_2400X}
         */
        public string imageSize = IMAGE_SIZE_AUTO;

        /**
         * Scale width<br/>
         * key: {@link #KEY_SCALE_WIDTH}<br/>
         * value: 0 for no limit
         */
        public int scaleWidth = 0;

        /**
         * Scale height<br/>
         * key: {@link #KEY_SCALE_HEIGHT}<br/>
         * value: 0 for no limit
         */
        public int scaleHeight = 0;

        /**
         * Gallery title<br/>
         * key: {@link #KEY_GALLERY_TITLE}<br/>
         * value: {@link #GALLERY_TITLE_DEFAULT}, {@link #GALLERY_TITLE_JAPANESE}
         */
        public string galleryTitle = GALLERY_TITLE_DEFAULT;

        /**
         * The default behavior for downloading an archiver<br/>
         * key: {@link #KEY_ARCHIVER_DOWNLOAD}<br/>
         * value: {@link #ARCHIVER_DOWNLOAD_MAMS}, {@link #ARCHIVER_DOWNLOAD_AAMS},
         * {@link #ARCHIVER_DOWNLOAD_MAAS}, {@link #ARCHIVER_DOWNLOAD_AAAS}
         */
        public string archiverDownload = ARCHIVER_DOWNLOAD_MAMS;

        /**
         * Display mode used on the front and search pages<br/>
         * false for list, true for thumb<br/>
         * key: {@link #KEY_LAYOUT_MODE}<br/>
         * value: {@link #LAYOUT_MODE_LIST}, {@link #LAYOUT_MODE_THUMB}
         */
        public string layoutMode = LAYOUT_MODE_LIST;

        /**
         * Show popular or not<br/>
         * key: {@link #KEY_POPULAR}<br/>
         * value: {@link #POPULAR_YES}, {@link #POPULAR_NO}
         */
        public string popular = POPULAR_YES;

        /**
         * Default categories on the front page<br/>
         * key: {@link #KEY_DEFAULT_CATEGORIES}<br/>
         * value: the value of categories, for multiple use & operation,
         * 0 for none
         */
        public int defaultCategories = 0;

        /**
         * <br/>
         * key: {@link #KEY_FAVORITES_SORT}<br/>
         * value: {@link #FAVORITES_SORT_GALLERY_UPDATE_TIME}, {@link #FAVORITES_SORT_FAVORITED_TIME}
         */
        public string favoritesSort = FAVORITES_SORT_FAVORITED_TIME;

        /**
         * Certain namespaces excluded from a default tag search<br/>
         * key: {@link #KEY_EXCLUDED_NAMESPACES}<br/>
         * value: the value of namespaces, for multiple use & operation,
         * 0 for none
         */
        public int excludedNamespaces = 0;

        /**
         * Certain languages excluded from list and searches<br/>
         * key: {@link #KEY_EXCLUDED_LANGUAGES}<br/>
         * value: {@link #JAPANESE_TRANSLATED}, {@link #JAPANESE_REWRITE} ...
         * For multiple languages, use <code>x<code/> to combine them, like 1x1024x2048
         */
        public string excludedLanguages = "";

        /**
         * How many results would you like per page for the index/search page
         * and torrent search pages<br/>
         * key: {@link #KEY_RESULT_COUNT}<br/>
         * value: {@link #RESULT_COUNT_25}, {@link #RESULT_COUNT_50},
         * {@link #RESULT_COUNT_100}, {@link #RESULT_COUNT_200}<br/>
         * Require <code>Hath Perk:Paging Enlargement</code>
         */
        public string resultCount = RESULT_COUNT_25;

        /**
         * mouse-over thumb<br/>
         * key: {@link #KEY_MOUSE_OVER}<br/>
         * value: {@link #MOUSE_OVER_YES}, {@link #MOUSE_OVER_NO}
         */
        public string mouseOver = MOUSE_OVER_YES;

        /**
         * Default preview mode<br/>
         * key: {@link #KEY_PREVIEW_SIZE}<br/>
         * value: {@link #PREVIEW_SIZE_NORMAL}, {@link #PREVIEW_SIZE_LARGE}
         */
        public string previewSize = PREVIEW_SIZE_LARGE;

        /**
         * Preview row<br/>
         * key: {@link #KEY_PREVIEW_ROW}<br/>
         * value: {@link #PREVIEW_ROW_4}, {@link #PREVIEW_ROW_10},
         * {@link #PREVIEW_ROW_20}, {@link #PREVIEW_ROW_40}
         */
        public string previewRow = PREVIEW_ROW_4;

        /**
         * Sort order for gallery comments<br/>
         * key: {@link #KEY_COMMENTS_SORT}<br/>
         * value: {@link #COMMENTS_SORT_OLDEST_FIRST}, {@link #COMMENTS_SORT_RECENT_FIRST},
         * {@link #COMMENTS_SORT_HIGHEST_SCORE_FIRST}
         */
        public string commentSort = COMMENTS_SORT_OLDEST_FIRST;

        /**
         * Show gallery comment votes mode<br/>
         * key: {@link #KEY_COMMENTS_VOTES}<br/>
         * value: {@link #COMMENTS_VOTES_POP}, {@link #COMMENTS_VOTES_ALWAYS}
         */
        public string commentVotes = COMMENTS_VOTES_POP;


        /**
         * Sort order for gallery tags<br/>
         * key: {@link #KEY_TAGS_SORT}<br/>
         * value: {@link #TAGS_SORT_ALPHABETICAL}, {@link #TAGS_SORT_POWER}
         */
        public string tagSort = TAGS_SORT_ALPHABETICAL;

        /**
         * Show gallery page numbers<br/>
         * key: {@link #KEY_SHOW_GALLERY_INDEX}<br/>
         * value: {@link #SHOW_GALLERY_INDEX_YES}, {@link #SHOW_GALLERY_INDEX_NO}
         */
        public string showGalleryIndex = SHOW_GALLERY_INDEX_YES;

        /**
         * The IP of a proxy-enabled Hentai@Home Client
         * to load all images<br/>
         * key: {@link #KEY_HAH_CLIENT_IP_PORT}<br/>
         */
        public string hahClientIp = "";

        /**
         * The PORT of a proxy-enabled Hentai@Home Client
         * to load all images<br/>
         * key: {@link #KEY_HAH_CLIENT_IP_PORT}<br/>
         */
        public int hahClientPort = -1;

        /**
         * The passkey of a proxy-enabled Hentai@Home Client
         * to load all images<br/>
         * key: {@link #KEY_HAH_CLIENT_PASSKEY}<br/>
         */
        public string hahClientPasskey = "";

        /**
         * Enable tag flagging
         * key: {@link #KEY_ENABLE_TAG_FLAGGING}<br/>
         * value: {@link #ENABLE_TAG_FLAGGING_YES}, {@link #ENABLE_TAG_FLAGGING_NO}<br/>
         * <code>Bronze Star</code> or <code>Hath Perk: Tag Flagging</code> Required
         */
        public string enableTagFlagging = ENABLE_TAG_FLAGGING_NO;

        /**
         * Always display the original images instead of the resampled versions<br/>
         * key: {@link #KEY_ALWAYS_ORIGINAL}<br/>
         * value: {@link #ALWAYS_ORIGINAL_YES}, {@link #ALWAYS_ORIGINAL_NO}<br/>
         * <code>Silver Star</code> or <code>Hath Perk: Source Nexus</code> Required
         */
        public string alwaysOriginal = ALWAYS_ORIGINAL_NO;

        /**
         * Enable the multi-Page Viewer<br/>
         * key: {@link #KEY_MULTI_PAGE}<br/>
         * value: {@link #MULTI_PAGE_YES}, {@link #MULTI_PAGE_NO}<br/>
         * <code>Gold Star</code> or <code>Hath Perk: Multi-Page Viewer</code> Required
         */
        public string multiPage = MULTI_PAGE_NO;

        /**
         * Multi-Page Viewer Display Style<br/>
         * key: {@link #KEY_MULTI_PAGE_STYLE}<br/>
         * value: {@link #MULTI_PAGE_STYLE_N}, {@link #MULTI_PAGE_STYLE_C},
         * {@link #MULTI_PAGE_STYLE_Y}<br/>
         * <code>Gold Star</code> or <code>Hath Perk: Multi-Page Viewer</code> Required
         */
        public string multiPageStyle = MULTI_PAGE_STYLE_N;

        /**
         * Multi-Page Viewer Thumbnail Pane<br/>
         * key: {@link #KEY_MULTI_PAGE_THUMB}<br/>
         * value: {@link #MULTI_PAGE_THUMB_SHOW}, {@link #MULTI_PAGE_THUMB_HIDE}<br/>
         * <code>Gold Star</code> or <code>Hath Perk: Multi-Page Viewer</code> Required
         */
        public string multiPageThumb = MULTI_PAGE_THUMB_SHOW;

        /**
         * Lofi resolution
         * key: {@link #KEY_LOFI_RESOLUTION}<br/>
         * value: {@link #LOFI_RESOLUTION_460X}, {@link #LOFI_RESOLUTION_780X},
         * {@link #LOFI_RESOLUTION_980X}
         */
        public string lofiResolution = LOFI_RESOLUTION_980X;

        /**
         * Show content warning
         * key: {@link #KEY_CONTENT_WARNING}<br/>
         * value: {@link #CONTENT_WARNING_SHOW}, {@link #CONTENT_WARNING_NOT_SHOW}
         */
        public string contentWarning = CONTENT_WARNING_NOT_SHOW;
    }
}
