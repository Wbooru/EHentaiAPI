using EHentaiAPI.Utils;
using System;
using System.Linq;

namespace EHentaiAPI.Client
{
    public class EhUrl
    {
        public const int SITE_E = 0;
        public const int SITE_EX = 1;

        public const string DOMAIN_EX = "exhentai.org";
        public const string DOMAIN_E = "e-hentai.org";
        public const string DOMAIN_LOFI = "lofi.e-hentai.org";

        public const string HOST_EX = "https://" + DOMAIN_EX + "/";
        public const string HOST_E = "https://" + DOMAIN_E + "/";

        public const string API_SIGN_IN = "https://forums.e-hentai.org/index.php?act=Login&CODE=01";

        public const string API_E = HOST_E + "api.php";
        public const string API_EX = HOST_EX + "api.php";

        public const string URL_POPULAR_E = "https://e-hentai.org/popular";
        public const string URL_POPULAR_EX = "https://exhentai.org/popular";

        public const string URL_IMAGE_SEARCH_E = "https://upload.e-hentai.org/image_lookup.php";
        public const string URL_IMAGE_SEARCH_EX = "https://exhentai.org/upload/image_lookup.php";

        public const string URL_SIGN_IN = "https://forums.e-hentai.org/index.php?act=Login";
        public const string URL_REGISTER = "https://forums.e-hentai.org/index.php?act=Reg&CODE=00";
        public const string URL_FAVORITES_E = HOST_E + "favorites.php";
        public const string URL_FAVORITES_EX = HOST_EX + "favorites.php";
        public const string URL_FORUMS = "https://forums.e-hentai.org/";

        public const string REFERER_EX = "https://" + DOMAIN_EX;
        public const string REFERER_E = "https://" + DOMAIN_E;

        public const string ORIGIN_EX = REFERER_EX;
        public const string ORIGIN_E = REFERER_E;

        public const string URL_UCONFIG_E = HOST_E + "uconfig.php";
        public const string URL_UCONFIG_EX = HOST_EX + "uconfig.php";

        public const string URL_MY_TAGS_E = HOST_E + "mytags";
        public const string URL_MY_TAGS_EX = HOST_EX + "mytags";

        public const string URL_WATCHED_E = HOST_E + "watched";
        public const string URL_WATCHED_EX = HOST_EX + "watched";

        private const string URL_PREFIX_THUMB_E = "https://ehgt.org/";
        private const string URL_PREFIX_THUMB_EX = "https://exhentai.org/t/";
        private readonly Settings settings;

        public EhUrl(Settings settings)
        {
            this.settings = settings;
        }

        public string getGalleryDetailUrl(long gid, string token)
        {
            return getGalleryDetailUrl(gid, token, 0, false);
        }

        public string getHost()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return HOST_E;
                case SITE_EX:
                    return HOST_EX;
            }
        }

        public string getFavoritesUrl()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return URL_FAVORITES_E;
                case SITE_EX:
                    return URL_FAVORITES_EX;
            }
        }

        public string getApiUrl()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return API_E;
                case SITE_EX:
                    return API_EX;
            }
        }

        public string getReferer()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return REFERER_E;
                case SITE_EX:
                    return REFERER_EX;
            }
        }

        public string getOrigin()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return ORIGIN_E;
                case SITE_EX:
                    return ORIGIN_EX;
            }
        }

        public string getUConfigUrl()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return URL_UCONFIG_E;
                case SITE_EX:
                    return URL_UCONFIG_EX;
            }
        }

        public string getMyTagsUrl()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return URL_MY_TAGS_E;
                case SITE_EX:
                    return URL_MY_TAGS_EX;
            }
        }

        public string getGalleryDetailUrl(long gid, string token, int index, bool allComment)
        {
            UrlBuilder builder = new UrlBuilder(getHost() + "g/" + gid + '/' + token + '/');
            if (index != 0)
            {
                builder.addQuery("p", index);
            }
            if (allComment)
            {
                builder.addQuery("hc", 1);
            }
            return builder.build();
        }

        public string getGalleryMultiPageViewerUrl(long gid, string token)
        {
            UrlBuilder builder = new UrlBuilder(getHost() + "mpv/" + gid + '/' + token + '/');
            return builder.build();
        }

        public string getPageUrl(long gid, int index, string pToken)
        {
            return getHost() + "s/" + pToken + '/' + gid + '-' + (index + 1);
        }

        public string getAddFavorites(long gid, string token)
        {
            return getHost() + "gallerypopups.php?gid=" + gid + "&t=" + token + "&act=addfav";
        }

        public string getDownloadArchive(long gid, string token, string or)
        {
            return getHost() + "archiver.php?gid=" + gid + "&token=" + token + "&or=" + or;
        }

        public string getTagDefinitionUrl(string tag)
        {
            return "https://ehwiki.org/wiki/" + tag.Replace(' ', '_');
        }

        public string getPopularUrl()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return URL_POPULAR_E;
                case SITE_EX:
                    return URL_POPULAR_EX;
            }
        }

        public string getImageSearchUrl()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return URL_IMAGE_SEARCH_E;
                case SITE_EX:
                    return URL_IMAGE_SEARCH_EX;
            }
        }

        public string getWatchedUrl()
        {
            switch (settings.getGallerySite())
            {
                default:
                case SITE_E:
                    return URL_WATCHED_E;
                case SITE_EX:
                    return URL_WATCHED_EX;
            }
        }

        public string getThumbUrlPrefix()
        {
            switch (settings.getGallerySite())
            {
                default:
                    //case SITE_E:
                    return URL_PREFIX_THUMB_E;
                    //case SITE_EX:
                    //    return URL_PREFIX_THUMB_EX;
            }
        }

        public string getFixedPreviewThumbUrl(string originUrl)
        {
            var url = new Uri(originUrl);
            if (url == null) return originUrl;
            var pathSegments = url.Segments.Skip(1).Select(x => x.EndsWith("/") ? x.Substring(0, x.Length - 1) : x).ToArray();
            if (pathSegments.Length < 3) return originUrl;

            var iterator = pathSegments.Reverse().GetEnumerator();
            // The last segments, like
            // 317a1a254cd9c3269e71b2aa2671fe8d28c91097-260198-640-480-png_250.jpg
            if (!iterator.MoveNext()) return originUrl;
            string lastSegment = iterator.Current;
            // The second last segments, like
            // 7a
            if (!iterator.MoveNext()) return originUrl;
            string secondLastSegment = iterator.Current;
            // The third last segments, like
            // 31
            if (!iterator.MoveNext()) return originUrl;
            string thirdLastSegment = iterator.Current;
            // Check path segments
            if (lastSegment != null && secondLastSegment != null
                    && thirdLastSegment != null
                    && lastSegment.StartsWith(thirdLastSegment)
                    && lastSegment.StartsWith(thirdLastSegment + secondLastSegment))
            {
                return getThumbUrlPrefix() + thirdLastSegment + "/" + secondLastSegment + "/" + lastSegment;
            }
            else
            {
                return originUrl;
            }
        }

        public Settings getSettings()
        {
            return settings;
        }
    }
}
