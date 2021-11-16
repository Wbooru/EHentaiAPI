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

        public ISharedPreferences SharedPreferences { get; set; } = new DefaultTemporaryMemorySharedPreferences();

        public bool getBoolean(string key, bool defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.d(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void putBoolean(string key, bool value)
        {
            SharedPreferences.setValue(key, value);
        }

        public int getInt(string key, int defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.d(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void putInt(string key, int value)
        {
            SharedPreferences.setValue(key, value);
        }

        public long getLong(string key, long defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.d(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void putLong(string key, long value)
        {
            SharedPreferences.setValue(key, value);
        }

        public float getFloat(string key, float defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.d(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void putFloat(string key, float value)
        {
            SharedPreferences.setValue(key, value);
        }

        public string getstring(string key, string defValue)
        {
            try
            {
                return SharedPreferences.getValue(key, defValue);
            }
            catch (InvalidCastException e)
            {
                Log.d(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void putstring(string key, string value)
        {
            SharedPreferences.setValue(key, value);
        }

        public int getIntFromStr(string key, int defValue)
        {
            try
            {
                return int.TryParse(SharedPreferences.getValue(key, defValue.ToString()), out var d) ? d : defValue;
            }
            catch (InvalidCastException e)
            {
                Log.d(TAG, "Get ClassCastException when get " + key + " value", e);
                return defValue;
            }
        }

        public void putIntToStr(string key, int value)
        {
            SharedPreferences.setValue(key, value.ToString());
        }

        public int getVersionCode()
        {
            return getInt(KEY_VERSION_CODE, DEFAULT_VERSION_CODE);
        }

        public void putVersionCode(int value)
        {
            putInt(KEY_VERSION_CODE, value);
        }

        public string getDisplayName()
        {
            return getstring(KEY_DISPLAY_NAME, DEFAULT_DISPLAY_NAME);
        }

        public void putDisplayName(string value)
        {
            putstring(KEY_DISPLAY_NAME, value);
        }

        public string getAvatar()
        {
            return getstring(KEY_AVATAR, DEFAULT_AVATAR);
        }

        public void putAvatar(string value)
        {
            putstring(KEY_AVATAR, value);
        }

        public bool getShowWarning()
        {
            return getBoolean(KEY_SHOW_WARNING, DEFAULT_SHOW_WARNING);
        }

        public void putShowWarning(bool value)
        {
            putBoolean(KEY_SHOW_WARNING, value);
        }

        public bool getRemoveImageFiles()
        {
            return getBoolean(KEY_REMOVE_IMAGE_FILES, DEFAULT_REMOVE_IMAGE_FILES);
        }

        public void putRemoveImageFiles(bool value)
        {
            putBoolean(KEY_REMOVE_IMAGE_FILES, value);
        }

        public bool getNeedSignIn()
        {
            return getBoolean(KEY_NEED_SIGN_IN, DEFAULT_NEED_SIGN_IN);
        }

        public void putNeedSignIn(bool value)
        {
            putBoolean(KEY_NEED_SIGN_IN, value);
        }

        public bool getSelectSite()
        {
            return getBoolean(KEY_SELECT_SITE, DEFAULT_SELECT_SITE);
        }

        public void putSelectSite(bool value)
        {
            putBoolean(KEY_SELECT_SITE, value);
        }

        public bool getQuickSearchTip()
        {
            return getBoolean(KEY_QUICK_SEARCH_TIP, DEFAULT_QUICK_SEARCH_TIP);
        }

        public void putQuickSearchTip(bool value)
        {
            putBoolean(KEY_QUICK_SEARCH_TIP, value);
        }

        public int getTheme()
        {
            return getIntFromStr(KEY_THEME, DEFAULT_THEME);
        }

        public void putTheme(int theme)
        {
            putIntToStr(KEY_THEME, theme);
        }

        public bool getApplyNavBarThemeColor()
        {
            return getBoolean(KEY_APPLY_NAV_BAR_THEME_COLOR, DEFAULT_APPLY_NAV_BAR_THEME_COLOR);
        }

        public int getGallerySite()
        {
            return getIntFromStr(KEY_GALLERY_SITE, DEFAULT_GALLERY_SITE);
        }

        public void putGallerySite(int value)
        {
            putIntToStr(KEY_GALLERY_SITE, value);
        }


        public int getListMode()
        {
            return getIntFromStr(KEY_LIST_MODE, DEFAULT_LIST_MODE);
        }

        public int getDetailSize()
        {
            return getIntFromStr(KEY_DETAIL_SIZE, DEFAULT_DETAIL_SIZE);
        }


        public int getThumbSize()
        {
            return getIntFromStr(KEY_THUMB_SIZE, DEFAULT_THUMB_SIZE);
        }

        public int getThumbResolution()
        {
            return getIntFromStr(KEY_THUMB_RESOLUTION, DEFAULT_THUMB_RESOLUTION);
        }

        public bool getFixThumbUrl()
        {
            return getBoolean(KEY_FIX_THUMB_URL, DEFAULT_FIX_THUMB_URL);
        }

        public bool getRequestNews()
        {
            return getBoolean(KEY_REQUEST_NEWS, DEFAULT_REQUEST_NEWS);
        }

        public bool getShowJpnTitle()
        {
            return getBoolean(KEY_SHOW_JPN_TITLE, DEFAULT_SHOW_JPN_TITLE);
        }

        public bool getShowGalleryPages()
        {
            return getBoolean(KEY_SHOW_GALLERY_PAGES, DEFAULT_SHOW_GALLERY_PAGES);
        }

        public bool getShowTagTranslations()
        {
            return getBoolean(KEY_SHOW_TAG_TRANSLATIONS, DEFAULT_SHOW_TAG_TRANSLATIONS);
        }

        public void putShowTagTranslations(bool value)
        {
            putBoolean(KEY_SHOW_TAG_TRANSLATIONS, value);
        }

        public int getDefaultCategories()
        {
            return getInt(KEY_DEFAULT_CATEGORIES, DEFAULT_DEFAULT_CATEGORIES);
        }

        public int getExcludedTagNamespaces()
        {
            return getInt(KEY_EXCLUDED_TAG_NAMESPACES, DEFAULT_EXCLUDED_TAG_NAMESPACES);
        }

        public string getExcludedLanguages()
        {
            return getstring(KEY_EXCLUDED_LANGUAGES, DEFAULT_EXCLUDED_LANGUAGES);
        }

        public void putEInkMode(bool value)
        {
            putBoolean(KEY_E_INK_MODE, value);
        }

        public bool getEInkMode()
        {
            return getBoolean(KEY_E_INK_MODE, DEFAULT_E_INK_MODE);
        }

        public bool getMeteredNetworkWarning()
        {
            return getBoolean(KEY_METERED_NETWORK_WARNING, DEFAULT_METERED_NETWORK_WARNING);
        }

        public int getScreenRotation()
        {
            return getIntFromStr(KEY_SCREEN_ROTATION, DEFAULT_SCREEN_ROTATION);
        }

        public void putScreenRotation(int value)
        {
            putIntToStr(KEY_SCREEN_ROTATION, value);
        }

        public void putReadingDirection(int value)
        {
            putIntToStr(KEY_READING_DIRECTION, value);
        }

        public void putPageScaling(int value)
        {
            putIntToStr(KEY_PAGE_SCALING, value);
        }


        public void putStartPosition(int value)
        {
            putIntToStr(KEY_START_POSITION, value);
        }

        public bool getKeepScreenOn()
        {
            return getBoolean(KEY_KEEP_SCREEN_ON, DEFAULT_KEEP_SCREEN_ON);
        }

        public void putKeepScreenOn(bool value)
        {
            putBoolean(KEY_KEEP_SCREEN_ON, value);
        }

        public bool getShowClock()
        {
            return getBoolean(KEY_SHOW_CLOCK, DEFAULT_SHOW_CLOCK);
        }

        public void putShowClock(bool value)
        {
            putBoolean(KEY_SHOW_CLOCK, value);
        }

        public bool getShowProgress()
        {
            return getBoolean(KEY_SHOW_PROGRESS, DEFAULT_SHOW_PROGRESS);
        }

        public void putShowProgress(bool value)
        {
            putBoolean(KEY_SHOW_PROGRESS, value);
        }

        public bool getShowBattery()
        {
            return getBoolean(KEY_SHOW_BATTERY, DEFAULT_SHOW_BATTERY);
        }

        public void putShowBattery(bool value)
        {
            putBoolean(KEY_SHOW_BATTERY, value);
        }

        public bool getShowPageInterval()
        {
            return getBoolean(KEY_SHOW_PAGE_INTERVAL, DEFAULT_SHOW_PAGE_INTERVAL);
        }

        public void putShowPageInterval(bool value)
        {
            putBoolean(KEY_SHOW_PAGE_INTERVAL, value);
        }

        public bool getVolumePage()
        {
            return getBoolean(KEY_VOLUME_PAGE, DEFAULT_VOLUME_PAGE);
        }

        public void putVolumePage(bool value)
        {
            putBoolean(KEY_VOLUME_PAGE, value);
        }

        public bool getReverseVolumePage()
        {
            return getBoolean(KEY_REVERSE_VOLUME_PAGE, DEFAULT_REVERSE_VOLUME_PAGE);
        }

        public void putReverseVolumePage(bool value)
        {
            putBoolean(KEY_REVERSE_VOLUME_PAGE, value);
        }

        public bool getReadingFullscreen()
        {
            return getBoolean(KEY_READING_FULLSCREEN, VALUE_READING_FULLSCREEN);
        }

        public void putReadingFullscreen(bool value)
        {
            putBoolean(KEY_READING_FULLSCREEN, value);
        }

        public bool getCustomScreenLightness()
        {
            return getBoolean(KEY_CUSTOM_SCREEN_LIGHTNESS, DEFAULT_CUSTOM_SCREEN_LIGHTNESS);
        }

        public void putCustomScreenLightness(bool value)
        {
            putBoolean(KEY_CUSTOM_SCREEN_LIGHTNESS, value);
        }

        public int getScreenLightness()
        {
            return getInt(KEY_SCREEN_LIGHTNESS, DEFAULT_SCREEN_LIGHTNESS);
        }

        public void putScreenLightness(int value)
        {
            putInt(KEY_SCREEN_LIGHTNESS, value);
        }

        public int getReadTheme()
        {
            return getIntFromStr(KEY_READ_THEME, DEFAULT_READ_THEME);
        }

        public void putReadTheme(int value)
        {
            putIntToStr(KEY_READ_THEME, value);
        }

        public bool getEnabledSecurity()
        {
            return getBoolean(KEY_SEC_SECURITY, VALUE_SEC_SECURITY);
        }

        public void putEnabledSecurity(bool value)
        {
            putBoolean(KEY_READING_FULLSCREEN, value);
        }

        public bool getMediaScan()
        {
            return getBoolean(KEY_MEDIA_SCAN, DEFAULT_MEDIA_SCAN);
        }

        public string getRecentDownloadLabel()
        {
            return getstring(KEY_RECENT_DOWNLOAD_LABEL, DEFAULT_RECENT_DOWNLOAD_LABEL);
        }

        public void putRecentDownloadLabel(string value)
        {
            putstring(KEY_RECENT_DOWNLOAD_LABEL, value);
        }

        public bool getHasDefaultDownloadLabel()
        {
            return getBoolean(KEY_HAS_DEFAULT_DOWNLOAD_LABEL, DEFAULT_HAS_DOWNLOAD_LABEL);
        }

        public void putHasDefaultDownloadLabel(bool hasDefaultDownloadLabel)
        {
            putBoolean(KEY_HAS_DEFAULT_DOWNLOAD_LABEL, hasDefaultDownloadLabel);
        }

        public string getDefaultDownloadLabel()
        {
            return getstring(KEY_DEFAULT_DOWNLOAD_LABEL, DEFAULT_DOWNLOAD_LABEL);
        }

        public void putDefaultDownloadLabel(string value)
        {
            putstring(KEY_DEFAULT_DOWNLOAD_LABEL, value);
        }

        public int getMultiThreadDownload()
        {
            return getIntFromStr(KEY_MULTI_THREAD_DOWNLOAD, DEFAULT_MULTI_THREAD_DOWNLOAD);
        }

        public void putMultiThreadDownload(int value)
        {
            putIntToStr(KEY_MULTI_THREAD_DOWNLOAD, value);
        }

        public int getDownloadDelay()
        {
            return getIntFromStr(KEY_DOWNLOAD_DELAY, DEFAULT_DOWNLOAD_DELAY);
        }

        public void putDownloadDelay(int value)
        {
            putIntToStr(KEY_DOWNLOAD_DELAY, value);
        }

        public int getPreloadImage()
        {
            return getIntFromStr(KEY_PRELOAD_IMAGE, DEFAULT_PRELOAD_IMAGE);
        }

        public void putPreloadImage(int value)
        {
            putIntToStr(KEY_PRELOAD_IMAGE, value);
        }

        public string getImageResolution()
        {
            return getstring(KEY_IMAGE_RESOLUTION, DEFAULT_IMAGE_RESOLUTION);
        }

        public bool getDownloadOriginImage()
        {
            return getBoolean(KEY_DOWNLOAD_ORIGIN_IMAGE, DEFAULT_DOWNLOAD_ORIGIN_IMAGE);
        }

        public void putDownloadOriginImage(bool value)
        {
            putBoolean(KEY_DOWNLOAD_ORIGIN_IMAGE, value);
        }

        public string[] getFavCat()
        {
            string[] favCat = new string[10];
            favCat[0] = SharedPreferences.getValue(KEY_FAV_CAT_0, DEFAULT_FAV_CAT_0);
            favCat[1] = SharedPreferences.getValue(KEY_FAV_CAT_1, DEFAULT_FAV_CAT_1);
            favCat[2] = SharedPreferences.getValue(KEY_FAV_CAT_2, DEFAULT_FAV_CAT_2);
            favCat[3] = SharedPreferences.getValue(KEY_FAV_CAT_3, DEFAULT_FAV_CAT_3);
            favCat[4] = SharedPreferences.getValue(KEY_FAV_CAT_4, DEFAULT_FAV_CAT_4);
            favCat[5] = SharedPreferences.getValue(KEY_FAV_CAT_5, DEFAULT_FAV_CAT_5);
            favCat[6] = SharedPreferences.getValue(KEY_FAV_CAT_6, DEFAULT_FAV_CAT_6);
            favCat[7] = SharedPreferences.getValue(KEY_FAV_CAT_7, DEFAULT_FAV_CAT_7);
            favCat[8] = SharedPreferences.getValue(KEY_FAV_CAT_8, DEFAULT_FAV_CAT_8);
            favCat[9] = SharedPreferences.getValue(KEY_FAV_CAT_9, DEFAULT_FAV_CAT_9);
            return favCat;
        }

        public void putFavCat(string[] value)
        {
            Debug.Assert(10 == value.Length);
            SharedPreferences
                    .setValue(KEY_FAV_CAT_0, value[0])
                    .setValue(KEY_FAV_CAT_1, value[1])
                    .setValue(KEY_FAV_CAT_2, value[2])
                    .setValue(KEY_FAV_CAT_3, value[3])
                    .setValue(KEY_FAV_CAT_4, value[4])
                    .setValue(KEY_FAV_CAT_5, value[5])
                    .setValue(KEY_FAV_CAT_6, value[6])
                    .setValue(KEY_FAV_CAT_7, value[7])
                    .setValue(KEY_FAV_CAT_8, value[8])
                    .setValue(KEY_FAV_CAT_9, value[9]);
        }

        public int[] getFavCount()
        {
            int[] favCount = new int[10];
            favCount[0] = SharedPreferences.getValue(KEY_FAV_COUNT_0, DEFAULT_FAV_COUNT);
            favCount[1] = SharedPreferences.getValue(KEY_FAV_COUNT_1, DEFAULT_FAV_COUNT);
            favCount[2] = SharedPreferences.getValue(KEY_FAV_COUNT_2, DEFAULT_FAV_COUNT);
            favCount[3] = SharedPreferences.getValue(KEY_FAV_COUNT_3, DEFAULT_FAV_COUNT);
            favCount[4] = SharedPreferences.getValue(KEY_FAV_COUNT_4, DEFAULT_FAV_COUNT);
            favCount[5] = SharedPreferences.getValue(KEY_FAV_COUNT_5, DEFAULT_FAV_COUNT);
            favCount[6] = SharedPreferences.getValue(KEY_FAV_COUNT_6, DEFAULT_FAV_COUNT);
            favCount[7] = SharedPreferences.getValue(KEY_FAV_COUNT_7, DEFAULT_FAV_COUNT);
            favCount[8] = SharedPreferences.getValue(KEY_FAV_COUNT_8, DEFAULT_FAV_COUNT);
            favCount[9] = SharedPreferences.getValue(KEY_FAV_COUNT_9, DEFAULT_FAV_COUNT);
            return favCount;
        }

        public void putFavCount(int[] count)
        {
            Debug.Assert(10 == count.Length);
            SharedPreferences
                    .setValue(KEY_FAV_COUNT_0, count[0])
                    .setValue(KEY_FAV_COUNT_1, count[1])
                    .setValue(KEY_FAV_COUNT_2, count[2])
                    .setValue(KEY_FAV_COUNT_3, count[3])
                    .setValue(KEY_FAV_COUNT_4, count[4])
                    .setValue(KEY_FAV_COUNT_5, count[5])
                    .setValue(KEY_FAV_COUNT_6, count[6])
                    .setValue(KEY_FAV_COUNT_7, count[7])
                    .setValue(KEY_FAV_COUNT_8, count[8])
                    .setValue(KEY_FAV_COUNT_9, count[9]);
        }

        public int getFavLocalCount()
        {
            return SharedPreferences.getValue(KEY_FAV_LOCAL, DEFAULT_FAV_COUNT);
        }

        public void putFavLocalCount(int count)
        {
            SharedPreferences.setValue(KEY_FAV_LOCAL, count);
        }

        public int getFavCloudCount()
        {
            return SharedPreferences.getValue(KEY_FAV_CLOUD, DEFAULT_FAV_COUNT);
        }

        public void putFavCloudCount(int count)
        {
            SharedPreferences.setValue(KEY_FAV_CLOUD, count);
        }

        public void putRecentFavCat(int value)
        {
            putInt(KEY_RECENT_FAV_CAT, value);
        }

        public int getDefaultFavSlot()
        {
            return getInt(KEY_DEFAULT_FAV_SLOT, DEFAULT_DEFAULT_FAV_SLOT);
        }

        public void putDefaultFavSlot(int value)
        {
            putInt(KEY_DEFAULT_FAV_SLOT, value);
        }

        public bool getAskAnalytics()
        {
            return getBoolean(KEY_ASK_ANALYTICS, DEFAULT_ASK_ANALYTICS);
        }

        public void putAskAnalytics(bool value)
        {
            putBoolean(KEY_ASK_ANALYTICS, value);
        }

        public bool getEnableAnalytics()
        {
            return getBoolean(KEY_ENABLE_ANALYTICS, DEFAULT_ENABLE_ANALYTICS);
        }

        public void putEnableAnalytics(bool value)
        {
            putBoolean(KEY_ENABLE_ANALYTICS, value);
        }

        public string getUserID()
        {
            string userID = getstring(KEY_USER_ID, null);
            return userID;
        }

        public bool getBetaUpdateChannel()
        {
            return getBoolean(KEY_BETA_UPDATE_CHANNEL, DEFAULT_BETA_UPDATE_CHANNEL);
        }

        public void putBetaUpdateChannel(bool value)
        {
            putBoolean(KEY_BETA_UPDATE_CHANNEL, value);
        }

        public int getSkipUpdateVersion()
        {
            return getInt(KEY_SKIP_UPDATE_VERSION, DEFAULT_SKIP_UPDATE_VERSION);
        }

        public void putSkipUpdateVersion(int value)
        {
            putInt(KEY_SKIP_UPDATE_VERSION, value);
        }

        public bool getSaveParseErrorBody()
        {
            return getBoolean(KEY_SAVE_PARSE_ERROR_BODY, DEFAULT_SAVE_PARSE_ERROR_BODY);
        }

        public void putSaveParseErrorBody(bool value)
        {
            putBoolean(KEY_SAVE_PARSE_ERROR_BODY, value);
        }

        public bool getSaveCrashLog()
        {
            return getBoolean(KEY_SAVE_CRASH_LOG, DEFAULT_SAVE_CRASH_LOG);
        }

        public string getSecurity()
        {
            return getstring(KEY_SECURITY, DEFAULT_SECURITY);
        }

        public void putSecurity(string value)
        {
            putstring(KEY_SECURITY, value);
        }

        public bool getEnableFingerprint()
        {
            return getBoolean(KEY_ENABLE_FINGERPRINT, true);
        }

        public void putEnableFingerprint(bool value)
        {
            putBoolean(KEY_ENABLE_FINGERPRINT, value);
        }

        public int getReadCacheSize()
        {
            return getIntFromStr(KEY_READ_CACHE_SIZE, DEFAULT_READ_CACHE_SIZE);
        }

        public bool getBuiltInHosts()
        {
            return getBoolean(KEY_BUILT_IN_HOSTS, DEFAULT_BUILT_IN_HOSTS);
        }

        public void putBuiltInHosts(bool value)
        {
            putBoolean(KEY_BUILT_IN_HOSTS, value);
        }

        public bool getDF()
        {
            return getBoolean(KEY_DOMAIN_FRONTING, DEFAULT_FRONTING);
        }

        public void putDF(bool value)
        {
            putBoolean(KEY_DOMAIN_FRONTING, value);
        }

        public string getAppLanguage()
        {
            return getstring(KEY_APP_LANGUAGE, DEFAULT_APP_LANGUAGE);
        }

        public void putAppLanguage(string value)
        {
            putstring(KEY_APP_LANGUAGE, value);
        }

        public int getProxyType()
        {
            return getInt(KEY_PROXY_TYPE, DEFAULT_PROXY_TYPE);
        }

        public void putProxyType(int value)
        {
            putInt(KEY_PROXY_TYPE, value);
        }

        public string getProxyIp()
        {
            return getstring(KEY_PROXY_IP, DEFAULT_PROXY_IP);
        }

        public void putProxyIp(string value)
        {
            putstring(KEY_PROXY_IP, value);
        }

        public int getProxyPort()
        {
            return getInt(KEY_PROXY_PORT, DEFAULT_PROXY_PORT);
        }

        public void putProxyPort(int value)
        {
            putInt(KEY_PROXY_PORT, value);
        }

        public bool getGuideQuickSearch()
        {
            return getBoolean(KEY_GUIDE_QUICK_SEARCH, DEFAULT_GUIDE_QUICK_SEARCH);
        }

        public void putGuideQuickSearch(bool value)
        {
            putBoolean(KEY_GUIDE_QUICK_SEARCH, value);
        }

        public bool getGuideCollections()
        {
            return getBoolean(KEY_GUIDE_COLLECTIONS, DEFAULT_GUIDE_COLLECTIONS);
        }

        public void putGuideCollections(bool value)
        {
            putBoolean(KEY_GUIDE_COLLECTIONS, value);
        }

        public bool getGuideDownloadThumb()
        {
            return getBoolean(KEY_GUIDE_DOWNLOAD_THUMB, DEFAULT_GUIDE_DOWNLOAD_THUMB);
        }

        public void putGuideDownloadThumb(bool value)
        {
            putBoolean(KEY_GUIDE_DOWNLOAD_THUMB, value);
        }

        public bool getGuideDownloadLabels()
        {
            return getBoolean(KEY_GUIDE_DOWNLOAD_LABELS, DEFAULT_GUIDE_DOWNLOAD_LABELS);
        }

        public void puttGuideDownloadLabels(bool value)
        {
            putBoolean(KEY_GUIDE_DOWNLOAD_LABELS, value);
        }

        public bool getGuideGallery()
        {
            return getBoolean(KEY_GUIDE_GALLERY, DEFAULT_GUIDE_GALLERY);
        }

        public void putGuideGallery(bool value)
        {
            putBoolean(KEY_GUIDE_GALLERY, value);
        }

        public int getClipboardTextHashCode()
        {
            return getInt(KEY_CLIPBOARD_TEXT_HASH_CODE, DEFAULT_CLIPBOARD_TEXT_HASH_CODE);
        }

        public void putClipboardTextHashCode(int value)
        {
            putInt(KEY_CLIPBOARD_TEXT_HASH_CODE, value);
        }
    }
}
