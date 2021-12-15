using EHentaiAPI.Client;
using EHentaiAPI.Client.Data;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI
{
    public class Settings
    {

        /********************
         ****** Eh
         ********************/

        public const string KEY_THEME = "theme";
        public const string KEY_BLACK_DARK_THEME = "black_dark_theme";
        public const int THEME_LIGHT = 1;
        public const int THEME_SYSTEM = -1;
        public const int THEME_BLACK = 2;
        public const string KEY_APPLY_NAV_BAR_THEME_COLOR = "apply_nav_bar_theme_color";
        public const string KEY_GALLERY_SITE = "gallery_site";
        public const string KEY_LIST_MODE = "list_mode";
        public const string KEY_DETAIL_SIZE = "detail_size";
        public const string KEY_THUMB_SIZE = "thumb_size";
        public const string KEY_THUMB_RESOLUTION = "thumb_resolution";
        public const string KEY_SHOW_TAG_TRANSLATIONS = "show_tag_translations";
        public const string KEY_DEFAULT_CATEGORIES = "default_categories";
        public const int DEFAULT_DEFAULT_CATEGORIES = EhUtils.ALL_CATEGORY;
        public const string KEY_EXCLUDED_TAG_NAMESPACES = "excluded_tag_namespaces";
        public const string KEY_EXCLUDED_LANGUAGES = "excluded_languages";
        /********************
         ****** Privacy and Security
         ********************/
        public const string KEY_SEC_SECURITY = "enable_secure";
        public const bool VALUE_SEC_SECURITY = false;
        /********************
         ****** Download
         ********************/
        public const string KEY_DOWNLOAD_SAVE_SCHEME = "image_scheme";
        public const string KEY_DOWNLOAD_SAVE_AUTHORITY = "image_authority";
        public const string KEY_DOWNLOAD_SAVE_PATH = "image_path";
        public const string KEY_DOWNLOAD_SAVE_QUERY = "image_query";
        public const string KEY_DOWNLOAD_SAVE_FRAGMENT = "image_fragment";
        public const string KEY_MEDIA_SCAN = "media_scan";
        public const string KEY_IMAGE_RESOLUTION = "image_size";
        public const string DEFAULT_IMAGE_RESOLUTION = EhConfig.IMAGE_SIZE_AUTO;
        public const int INVALID_DEFAULT_FAV_SLOT = -2;
        public const string KEY_ENABLE_ANALYTICS = "enable_analytics";
        /********************
         ****** Advanced
         ********************/
        public const string KEY_SAVE_PARSE_ERROR_BODY = "save_parse_error_body";
        public const string KEY_SECURITY = "security";
        public const string DEFAULT_SECURITY = "";
        public const string KEY_ENABLE_FINGERPRINT = "enable_fingerprint";
        public const string KEY_READ_CACHE_SIZE = "read_cache_size";
        public const int DEFAULT_READ_CACHE_SIZE = 320;
        public const string KEY_BUILT_IN_HOSTS = "built_in_hosts_2";
        public const string KEY_DOMAIN_FRONTING = "domain_fronting";
        public const string KEY_APP_LANGUAGE = "app_language";
        private const string TAG = nameof(Settings);
        private const string KEY_VERSION_CODE = "version_code";
        private const int DEFAULT_VERSION_CODE = 0;
        private const string KEY_DISPLAY_NAME = "display_name";
        private const string DEFAULT_DISPLAY_NAME = null;
        private const string KEY_AVATAR = "avatar";
        private const string DEFAULT_AVATAR = null;
        private const string KEY_SHOW_WARNING = "show_warning";
        private const bool DEFAULT_SHOW_WARNING = true;
        private const string KEY_REMOVE_IMAGE_FILES = "include_pic";
        private const bool DEFAULT_REMOVE_IMAGE_FILES = true;
        private const string KEY_NEED_SIGN_IN = "need_sign_in";
        private const bool DEFAULT_NEED_SIGN_IN = true;
        private const string KEY_SELECT_SITE = "select_site";
        private const bool DEFAULT_SELECT_SITE = true;
        private const string KEY_QUICK_SEARCH_TIP = "quick_search_tip";
        private const bool DEFAULT_QUICK_SEARCH_TIP = true;
        private const int DEFAULT_THEME = THEME_SYSTEM;
        private const bool DEFAULT_APPLY_NAV_BAR_THEME_COLOR = true;
        private const int DEFAULT_GALLERY_SITE = 1;
        private const string KEY_LAUNCH_PAGE = "launch_page";
        private const int DEFAULT_LAUNCH_PAGE = 0;
        private const int DEFAULT_LIST_MODE = 0;
        private const int DEFAULT_DETAIL_SIZE = 0;
        private const int DEFAULT_THUMB_SIZE = 1;
        private const int DEFAULT_THUMB_RESOLUTION = 0;
        private const string KEY_FIX_THUMB_URL = "fix_thumb_url";
        private const bool DEFAULT_FIX_THUMB_URL = false;
        private const string KEY_SHOW_JPN_TITLE = "show_jpn_title";
        private const bool DEFAULT_SHOW_JPN_TITLE = false;
        private const string KEY_SHOW_GALLERY_PAGES = "show_gallery_pages";
        private const bool DEFAULT_SHOW_GALLERY_PAGES = false;
        private const bool DEFAULT_SHOW_TAG_TRANSLATIONS = false;
        private const int DEFAULT_EXCLUDED_TAG_NAMESPACES = 0;
        private const string DEFAULT_EXCLUDED_LANGUAGES = null;
        private const string KEY_METERED_NETWORK_WARNING = "cellular_network_warning";
        private const bool DEFAULT_METERED_NETWORK_WARNING = false;
        private const string KEY_NIGHT_MODE = "night_mode";
        private const string DEFAULT_NIGHT_MODE = "-1";
        private const string KEY_E_INK_MODE = "e_ink_mode_2";
        private const bool DEFAULT_E_INK_MODE = false;
        /********************
         ****** Read
         ********************/
        private const string KEY_SCREEN_ROTATION = "screen_rotation";
        private const int DEFAULT_SCREEN_ROTATION = 0;
        private const string KEY_READING_DIRECTION = "reading_direction";
        //private const int DEFAULT_READING_DIRECTION = GalleryView.LAYOUT_RIGHT_TO_LEFT;
        private const string KEY_PAGE_SCALING = "page_scaling";
        //private const int DEFAULT_PAGE_SCALING = GalleryView.SCALE_FIT;
        private const string KEY_START_POSITION = "start_position";
        //private const int DEFAULT_START_POSITION = GalleryView.START_POSITION_TOP_RIGHT;
        private const string KEY_KEEP_SCREEN_ON = "keep_screen_on";
        private const bool DEFAULT_KEEP_SCREEN_ON = false;
        private const string KEY_SHOW_CLOCK = "gallery_show_clock";
        private const bool DEFAULT_SHOW_CLOCK = true;
        private const string KEY_SHOW_PROGRESS = "gallery_show_progress";
        private const bool DEFAULT_SHOW_PROGRESS = true;
        private const string KEY_SHOW_BATTERY = "gallery_show_battery";
        private const bool DEFAULT_SHOW_BATTERY = true;
        private const string KEY_SHOW_PAGE_INTERVAL = "gallery_show_page_interval";
        private const bool DEFAULT_SHOW_PAGE_INTERVAL = true;
        private const string KEY_VOLUME_PAGE = "volume_page";
        private const bool DEFAULT_VOLUME_PAGE = false;
        private const string KEY_REVERSE_VOLUME_PAGE = "reserve_volume_page";
        private const bool DEFAULT_REVERSE_VOLUME_PAGE = false;
        private const string KEY_READING_FULLSCREEN = "reading_fullscreen";
        private const bool VALUE_READING_FULLSCREEN = true;
        private const string KEY_CUSTOM_SCREEN_LIGHTNESS = "custom_screen_lightness";
        private const bool DEFAULT_CUSTOM_SCREEN_LIGHTNESS = false;
        private const string KEY_SCREEN_LIGHTNESS = "screen_lightness";
        private const int DEFAULT_SCREEN_LIGHTNESS = 50;
        private const bool DEFAULT_MEDIA_SCAN = false;
        private const string KEY_RECENT_DOWNLOAD_LABEL = "recent_download_label";
        private const string DEFAULT_RECENT_DOWNLOAD_LABEL = null;
        private const string KEY_HAS_DEFAULT_DOWNLOAD_LABEL = "has_default_download_label";
        private const bool DEFAULT_HAS_DOWNLOAD_LABEL = false;
        private const string KEY_DEFAULT_DOWNLOAD_LABEL = "default_download_label";
        private const string DEFAULT_DOWNLOAD_LABEL = null;
        private const string KEY_MULTI_THREAD_DOWNLOAD = "download_thread";
        private const int DEFAULT_MULTI_THREAD_DOWNLOAD = 3;
        private const string KEY_PRELOAD_IMAGE = "preload_image";
        private const int DEFAULT_PRELOAD_IMAGE = 5;
        private const string KEY_DOWNLOAD_ORIGIN_IMAGE = "download_origin_image";
        private const bool DEFAULT_DOWNLOAD_ORIGIN_IMAGE = false;
        private const string KEY_READ_THEME = "read_theme";
        private const int DEFAULT_READ_THEME = 1;
        /********************
         ****** Favorites
         ********************/
        private const string KEY_FAV_CAT_0 = "fav_cat_0";
        private const string KEY_FAV_CAT_1 = "fav_cat_1";
        private const string KEY_FAV_CAT_2 = "fav_cat_2";
        private const string KEY_FAV_CAT_3 = "fav_cat_3";
        private const string KEY_FAV_CAT_4 = "fav_cat_4";
        private const string KEY_FAV_CAT_5 = "fav_cat_5";
        private const string KEY_FAV_CAT_6 = "fav_cat_6";
        private const string KEY_FAV_CAT_7 = "fav_cat_7";
        private const string KEY_FAV_CAT_8 = "fav_cat_8";
        private const string KEY_FAV_CAT_9 = "fav_cat_9";
        private const string DEFAULT_FAV_CAT_0 = "Favorites 0";
        private const string DEFAULT_FAV_CAT_1 = "Favorites 1";
        private const string DEFAULT_FAV_CAT_2 = "Favorites 2";
        private const string DEFAULT_FAV_CAT_3 = "Favorites 3";
        private const string DEFAULT_FAV_CAT_4 = "Favorites 4";
        private const string DEFAULT_FAV_CAT_5 = "Favorites 5";
        private const string DEFAULT_FAV_CAT_6 = "Favorites 6";
        private const string DEFAULT_FAV_CAT_7 = "Favorites 7";
        private const string DEFAULT_FAV_CAT_8 = "Favorites 8";
        private const string DEFAULT_FAV_CAT_9 = "Favorites 9";
        private const string KEY_FAV_COUNT_0 = "fav_count_0";
        private const string KEY_FAV_COUNT_1 = "fav_count_1";
        private const string KEY_FAV_COUNT_2 = "fav_count_2";
        private const string KEY_FAV_COUNT_3 = "fav_count_3";
        private const string KEY_FAV_COUNT_4 = "fav_count_4";
        private const string KEY_FAV_COUNT_5 = "fav_count_5";
        private const string KEY_FAV_COUNT_6 = "fav_count_6";
        private const string KEY_FAV_COUNT_7 = "fav_count_7";
        private const string KEY_FAV_COUNT_8 = "fav_count_8";
        private const string KEY_FAV_COUNT_9 = "fav_count_9";
        private const string KEY_FAV_LOCAL = "fav_local";
        private const string KEY_FAV_CLOUD = "fav_cloud";
        private const int DEFAULT_FAV_COUNT = 0;
        private const string KEY_RECENT_FAV_CAT = "recent_fav_cat";
        // -1 for local, 0 - 9 for cloud favorite, other for no default fav slot
        private const string KEY_DEFAULT_FAV_SLOT = "default_favorite_2";
        private const int DEFAULT_DEFAULT_FAV_SLOT = INVALID_DEFAULT_FAV_SLOT;
        /********************
         ****** Analytics
         ********************/
        private const string KEY_ASK_ANALYTICS = "ask_analytics";
        private const bool DEFAULT_ASK_ANALYTICS = true;
        private const bool DEFAULT_ENABLE_ANALYTICS = false;
        private const string KEY_USER_ID = "user_id";
        private const string FILENAME_USER_ID = ".user_id";
        private const int LENGTH_USER_ID = 32;
        /********************
         ****** Update
         ********************/
        private const string KEY_BETA_UPDATE_CHANNEL = "beta_update_channel";
        private const bool DEFAULT_BETA_UPDATE_CHANNEL = false;
        private const string KEY_SKIP_UPDATE_VERSION = "skip_update_version";
        private const int DEFAULT_SKIP_UPDATE_VERSION = 0;
        private const bool DEFAULT_SAVE_PARSE_ERROR_BODY = true;
        private const string KEY_SAVE_CRASH_LOG = "save_crash_log";
        private const bool DEFAULT_SAVE_CRASH_LOG = true;
        private const bool DEFAULT_BUILT_IN_HOSTS = false;
        private const bool DEFAULT_FRONTING = false;
        private const string DEFAULT_APP_LANGUAGE = "system";
        private const string KEY_PROXY_TYPE = "proxy_type";
        private const int DEFAULT_PROXY_TYPE = 1;
        private const string KEY_PROXY_IP = "proxy_ip";
        private const string DEFAULT_PROXY_IP = null;
        private const string KEY_PROXY_PORT = "proxy_port";
        private const int DEFAULT_PROXY_PORT = -1;
        /********************
         ****** Guide
         ********************/
        private const string KEY_GUIDE_QUICK_SEARCH = "guide_quick_search";
        private const bool DEFAULT_GUIDE_QUICK_SEARCH = true;
        private const string KEY_GUIDE_COLLECTIONS = "guide_collections";
        private const bool DEFAULT_GUIDE_COLLECTIONS = true;
        private const string KEY_GUIDE_DOWNLOAD_THUMB = "guide_download_thumb";
        private const bool DEFAULT_GUIDE_DOWNLOAD_THUMB = true;
        private const string KEY_GUIDE_DOWNLOAD_LABELS = "guide_download_labels";
        private const bool DEFAULT_GUIDE_DOWNLOAD_LABELS = true;
        private const string KEY_GUIDE_GALLERY = "guide_gallery";
        private const bool DEFAULT_GUIDE_GALLERY = true;
        private const string KEY_CLIPBOARD_TEXT_HASH_CODE = "clipboard_text_hash_code";
        private const int DEFAULT_CLIPBOARD_TEXT_HASH_CODE = 0;
        private const string KEY_DOWNLOAD_DELAY = "download_delay";
        private const int DEFAULT_DOWNLOAD_DELAY = 0;
        private const string KEY_REQUEST_NEWS = "request_news";
        private const bool DEFAULT_REQUEST_NEWS = true;

        //Added by EHentaiAPI
        public const string KEY_SPIDER_FRONT_PRELOAD_COUNT = "spider_front_preload_count";
        public const string KEY_SPIDER_BACK_PRELOAD_COUNT = "spider_back_preload_count";
        public const int DEFAULT_SPIDER_FRONT_PRELOAD_COUNT = 5;
        public const int DEFAULT_SPIDER_BACK_PRELOAD_COUNT = 1;

        public ISharedPreferences SharedPreferences { get; set; } = new DefaultTemporaryMemorySharedPreferences();

        public bool GetBoolean(string key, bool defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.D(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void SetBoolean(string key, bool value)
        {
            SharedPreferences.setValue(key, value);
        }

        public int GetInt(string key, int defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.D(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void SetInt(string key, int value)
        {
            SharedPreferences.setValue(key, value);
        }

        public long GetLong(string key, long defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.D(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void SetLong(string key, long value)
        {
            SharedPreferences.setValue(key, value);
        }

        public float GetFloat(string key, float defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.D(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void SetFloat(string key, float value)
        {
            SharedPreferences.setValue(key, value);
        }

        public string GetString(string key, string defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.D(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void SetString(string key, string value)
        {
            SharedPreferences.setValue(key, value);
        }

        public void PutIntToStr(string key, int value)
        {
            SharedPreferences.setValue(key, value.ToString());
        }

        public enum GallerySites
        {
            SITE_E = 0,
            SITE_EX = 1,
        }

        public GallerySites GallerySite
        {
            get { return (GallerySites)GetInt(KEY_GALLERY_SITE, DEFAULT_GALLERY_SITE); }
            set { SetInt(KEY_GALLERY_SITE, (int)value); }
        }

        public int SpiderFrontPreloadCount
        {
            get { return GetInt(KEY_SPIDER_FRONT_PRELOAD_COUNT, DEFAULT_SPIDER_FRONT_PRELOAD_COUNT); }
            set { SetInt(KEY_SPIDER_FRONT_PRELOAD_COUNT, value); }
        }

        public int SpiderBackPreloadCount
        {
            get { return GetInt(KEY_SPIDER_BACK_PRELOAD_COUNT, DEFAULT_SPIDER_BACK_PRELOAD_COUNT); }
            set { SetInt(KEY_SPIDER_BACK_PRELOAD_COUNT, value); }
        }

        public int ThumbResolution
        {
            get { return GetInt(KEY_THUMB_RESOLUTION, DEFAULT_THUMB_RESOLUTION); }
            set { SetInt(KEY_THUMB_RESOLUTION, value); }
        }

        public bool FixThumbUrl
        {
            get { return GetBoolean(KEY_FIX_THUMB_URL, DEFAULT_FIX_THUMB_URL); }
            set { SetBoolean(KEY_FIX_THUMB_URL, value); }
        }
    }
}
