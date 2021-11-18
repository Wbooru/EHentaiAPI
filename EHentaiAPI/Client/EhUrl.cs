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
        //private const string URL_PREFIX_THUMB_EX = "https://exhentai.org/t/";

        private readonly Settings settings;

        public EhUrl(Settings settings)
        {
            this.settings = settings;
        }

        public string GetGalleryDetailUrl(long gid, string token)
        {
            return GetGalleryDetailUrl(gid, token, 0, false);
        }

        public string GetHost()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => HOST_EX,
                _ => HOST_E,
            };
        }

        public string GetFavoritesUrl()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => URL_FAVORITES_EX,
                _ => URL_FAVORITES_E,
            };
        }

        public string GetApiUrl()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => API_EX,
                _ => API_E,
            };
        }

        public string GetReferer()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => REFERER_EX,
                _ => REFERER_E,
            };
        }

        public string GetOrigin()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => ORIGIN_EX,
                _ => ORIGIN_E,
            };
        }

        public string GetUConfigUrl()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => URL_UCONFIG_EX,
                _ => URL_UCONFIG_E,
            };
        }

        public string GetMyTagsUrl()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => URL_MY_TAGS_EX,
                _ => URL_MY_TAGS_E,
            };
        }

        public string GetGalleryDetailUrl(long gid, string token, int index, bool allComment)
        {
            UrlBuilder builder = new UrlBuilder(GetHost() + "g/" + gid + '/' + token + '/');
            if (index != 0)
            {
                builder.AddQuery("p", index);
            }
            if (allComment)
            {
                builder.AddQuery("hc", 1);
            }
            return builder.Build();
        }

        public string GetGalleryMultiPageViewerUrl(long gid, string token)
        {
            UrlBuilder builder = new UrlBuilder(GetHost() + "mpv/" + gid + '/' + token + '/');
            return builder.Build();
        }

        public string GetPageUrl(long gid, int index, string pToken)
        {
            return GetHost() + "s/" + pToken + '/' + gid + '-' + (index + 1);
        }

        public string GetAddFavorites(long gid, string token)
        {
            return GetHost() + "gallerypopups.php?gid=" + gid + "&t=" + token + "&act=addfav";
        }

        public string GetDownloadArchive(long gid, string token, string or)
        {
            return GetHost() + "archiver.php?gid=" + gid + "&token=" + token + "&or=" + or;
        }

        public static string GetTagDefinitionUrl(string tag)
        {
            return "https://ehwiki.org/wiki/" + tag.Replace(' ', '_');
        }

        public string GetPopularUrl()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => URL_POPULAR_EX,
                _ => URL_POPULAR_E,
            };
        }

        public string GetImageSearchUrl()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => URL_IMAGE_SEARCH_EX,
                _ => URL_IMAGE_SEARCH_E,
            };
        }

        public string GetWatchedUrl()
        {
            return settings.GetGallerySite() switch
            {
                SITE_EX => URL_WATCHED_EX,
                _ => URL_WATCHED_E,
            };
        }

        public string GetThumbUrlPrefix()
        {
            return settings.GetGallerySite() switch
            {
                _ => URL_PREFIX_THUMB_E,//case SITE_E:
            };
        }

        public string GetFixedPreviewThumbUrl(string originUrl)
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
                return GetThumbUrlPrefix() + thirdLastSegment + "/" + secondLastSegment + "/" + lastSegment;
            }
            else
            {
                return originUrl;
            }
        }

        public Settings GetSettings()
        {
            return settings;
        }
    }
}
