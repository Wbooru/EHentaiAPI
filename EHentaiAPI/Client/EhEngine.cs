using AngleSharp.Dom;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Client.Parser;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public class EhEngine
    {
        public const string MEDIA_TYPE_JSON = "application/json";
        private const string TAG = nameof(EhEngine);
        private const string SAD_PANDA_DISPOSITION = "inline; filename=\"sadpanda.jpg\"";
        private const string SAD_PANDA_TYPE = "image/gif";
        private const string SAD_PANDA_LENGTH = "9615";
        private const string KOKOMADE_URL = "https://exhentai.org/img/kokomade.jpg";
        private const string MEDIA_TYPE_JPEG = "image/jpeg";

        private static readonly Regex PATTERN_NEED_HATH_CLIENT = new Regex("(You must have a H@H client assigned to your account to use this feature\\.)");

        private static void DoThrowException(int code, WebHeaderCollection headers,
                                             string body, Exception e)
        {
            // Check sad panda
            if (headers != null && SAD_PANDA_DISPOSITION.Equals(headers.Get("Content-Disposition")) &&
                    SAD_PANDA_TYPE.Equals(headers.Get("Content-Type")) &&
                    SAD_PANDA_LENGTH.Equals(headers.Get("Content-Length")))
            {
                throw new EhException("Sad Panda");
            }

            // Check sad panda(without panda)
            if (headers != null && "text/html; charset=UTF-8".Equals(headers.Get("Content-Type")) &&
                    "0".Equals(headers.Get("Content-Length")))
            {
                throw new EhException("Sad Panda\n(without panda)");
            }

            // Check kokomade
            if (body != null && body.Contains(KOKOMADE_URL))
            {
                throw new EhException("今回はここまで\n\n"/* + GetText.getString(R.string.kokomade_tip)*/);
            }

            if (body != null && body.Contains("Gallery Not Available - "))
            {
                string error = GalleryNotAvailableParser.Parse(body);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    throw new EhException(error);
                }
            }

            if (e is ParseException)
            {
                if (body != null && !body.Contains("<"))
                {
                    throw new EhException(body);
                }
                else if (string.IsNullOrWhiteSpace(body))
                {
                    throw new EhException("Empty page");
                }
                else
                {
                    throw new EhException("Parse error");
                }
            }

            if (code >= 400)
            {
                throw new StatusCodeException(code);
            }

            if (e != null)
            {
                throw e;
            }
        }

        private static void ThrowException(int code, WebHeaderCollection headers, string body,
                                            Exception e)
        {
            try
            {
                DoThrowException(code, headers, body, e);
            }
            catch (Exception error)
            {
                Log.D("StackTrace", error.StackTrace);
                throw;
            }
        }

        public static async Task<string> SignInAsync(CookieContainer cookieContainer,
                                    string username, string password)
        {
            string referer = "https://forums.e-hentai.org/index.php?act=Login&CODE=00";
            using var formBody = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                ["referer"] = referer,
                ["b"] = "",
                ["bt"] = "",
                ["UserName"] = username,
                ["PassWord"] = password,
                ["CookieDate"] = "1"
            });
            var url = EhUrl.API_SIGN_IN;
            string origin = "https://forums.e-hentai.org";
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(formBody)
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return SignInParser.Parse(body);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<GalleryListParser.Result> GetGalleryListAsync(EhTask task, CookieContainer cookieContainer,
                                                              string url)
        {
            string referer = task.EhUrl.GetReferer();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            GalleryListParser.Result result;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                result = GalleryListParser.Parse(task.EhUrl.GetSettings(), body);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }

            return result;
        }

        // At least, GalleryInfo contain valid gid and token
        public static async Task<List<GalleryInfo>> FillGalleryListByApiAsync(EhTask task, CookieContainer cookieContainer,
                                                             List<GalleryInfo> galleryInfoList, string referer)
        {
            // We can only request 25 items one time at most
            const int MAX_REQUEST_SIZE = 25;
            List<GalleryInfo> requestItems = new(MAX_REQUEST_SIZE);
            for (int i = 0, size = galleryInfoList.Count; i < size; i++)
            {
                requestItems.Add(galleryInfoList.ElementAt(i));
                if (requestItems.Count == MAX_REQUEST_SIZE || i == size - 1)
                {
                    await DoFillGalleryListByApiAsync(task, cookieContainer, requestItems, referer);
                    requestItems.Clear();
                }
            }
            return galleryInfoList;
        }

        private static async Task DoFillGalleryListByApiAsync(EhTask task, CookieContainer cookieContainer,
                                                   List<GalleryInfo> galleryInfoList, string referer)
        {
            var json = new JObject
            {
                { "method", "gdata" }
            };
            var ja = new JArray();
            for (int i = 0, size = galleryInfoList.Count; i < size; i++)
            {
                GalleryInfo gi = galleryInfoList.ElementAt(i);
                var g = new JArray
                {
                    gi.Gid,
                    gi.Token
                };
                ja.Add(g);
            }
            json.Add("gidlist", ja);
            json.Add("namespace", 1);
            string url = task.EhUrl.GetApiUrl();
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .cookies(cookieContainer)
                    .post(new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON))
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                GalleryApiParser.Parse(task.EhUrl.GetSettings(), body, galleryInfoList);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<GalleryDetail> GetGalleryDetailAsync(EhTask task, CookieContainer cookieContainer,
                                                     string url)
        {
            string referer = task.EhUrl.GetReferer();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                string html = EventPaneParser.Parse(body);
                /*
                if (html != null)
                {
                    EhApplication.getInstance().showEventPane(html);
                }
                */
                return GalleryDetailParser.Parse(task.EhUrl, body, url);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }


        public static async Task<KeyValuePair<PreviewSet, int>> GetPreviewSetAsync(
                 EhTask task, CookieContainer cookieContainer, string url)
        {
            string referer = task.EhUrl.GetReferer();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return KeyValuePair.Create(GalleryDetailParser.ParsePreviewSet(task.EhUrl, body),
                        GalleryDetailParser.ParsePreviewPages(body));
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<RateGalleryParser.Result> RateGalleryAsync(EhTask task, CookieContainer cookieContainer,
                                                            long apiUid, string apiKey, long gid,
                                                           string token, float rating)
        {
            var json = new JObject
            {
                { "method", "rategallery" },
                { "apiuid", apiUid },
                { "apikey", apiKey },
                { "gid", gid },
                { "token", token },
                { "rating", (int)Math.Ceiling(rating * 2) }
            };
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.EhUrl.GetApiUrl();
            string referer = task.EhUrl.GetGalleryDetailUrl(gid, token);
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return RateGalleryParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<GalleryCommentList> CommentGalleryAsync(EhTask task, CookieContainer cookieContainer,
                                                         string url, string comment, long? id)
        {
            var bodyMap = new Dictionary<string, string>();
            if (id == null)
            {
                bodyMap.Add("commenttext_new", comment);
            }
            else
            {
                bodyMap.Add("commenttext_edit", comment);
                bodyMap.Add("edit_comment", id?.ToString());
            }
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, url, origin)
                    .post(new FormUrlEncodedContent(bodyMap))
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                var document = Utils.Document.Parse(body);

                var elements = document.Select("#chd + p");
                if (elements.Length > 0)
                {
                    throw new EhException(elements[0].Text());
                }

                return GalleryDetailParser.ParseComments(document);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<string> GetGalleryTokenAsync(EhTask task, CookieContainer cookieContainer,
                                             long gid, string gtoken, int page)
        {
            var obj = new
            {
                method = "gtoken",
                pagelist = new[] { new object[] { gid, gtoken, page + 1 } }
            };
            var d = JsonConvert.SerializeObject(obj);

            var requestBody = new StringContent(d, Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.EhUrl.GetApiUrl();
            string referer = task.EhUrl.GetReferer();
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return GalleryTokenApiParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<FavoritesParser.Result> GetFavoritesAsync(EhTask task, CookieContainer cookieContainer,
                                                          string url)
        {
            string referer = task.EhUrl.GetReferer();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            FavoritesParser.Result result;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                result = FavoritesParser.Parse(task.EhUrl.GetSettings(), body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }

            return result;
        }

        /**
         * @param dstCat -1 for delete, 0 - 9 for cloud favorite, others throw Exception
         * @param note   max 250 characters
         */
        public static async Task AddFavoritesAsync(EhTask task, CookieContainer cookieContainer,
                                        long gid, string token, int dstCat, string note)
        {
            string catStr;
            if (dstCat == -1)
            {
                catStr = "favdel";
            }
            else if (dstCat >= 0 && dstCat <= 9)
            {
                catStr = dstCat.ToString();
            }
            else
            {
                throw new EhException("Invalid dstCat: " + dstCat);
            }
            var builder = new Dictionary<string, string>
            {
                { "favcat", catStr },
                { "favnote", note ?? "" },
                // submit=Add+to+Favorites is not necessary, just use submit=Apply+Changes all the time
                { "submit", "Apply Changes" },
                { "update", "1" }
            };
            string url = task.EhUrl.GetAddFavorites(gid, token);
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, url, origin)
                    .post(new FormUrlEncodedContent(builder))
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                //throwException(null, code, headers, body, null);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task AddFavoritesRangeAsync(EhTask task, CookieContainer cookieContainer,
                                             long[] gidArray, string[] tokenArray, int dstCat)
        {
            Debug.Assert(gidArray.Length == tokenArray.Length);
            for (int i = 0, n = gidArray.Length; i < n; i++)
            {
                await AddFavoritesAsync(task, cookieContainer, gidArray[i], tokenArray[i], dstCat, null);
            }
        }

        public static async Task<FavoritesParser.Result> ModifyFavoritesAsync(EhTask task, CookieContainer cookieContainer,
                                                             string url, long[] gidArray, int dstCat)
        {
            string catStr;
            if (dstCat == -1)
            {
                catStr = "delete";
            }
            else if (dstCat >= 0 && dstCat <= 9)
            {
                catStr = "fav" + dstCat;
            }
            else
            {
                throw new EhException("Invalid dstCat: " + dstCat);
            }
            var builder = new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("ddact", catStr)
            };
            foreach (long gid in gidArray)
            {
                builder.Add(KeyValuePair.Create("modifygids[]", gid.ToString()));
            }
            builder.Add(KeyValuePair.Create("apply", "Apply"));
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, url, origin)
                    .post(new FormUrlEncodedContent(builder))
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            FavoritesParser.Result result;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                result = FavoritesParser.Parse(task.EhUrl.GetSettings(), body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }

            return result;
        }

        public static async Task<Dictionary<string, string>> GetTorrentListAsync(EhTask task, CookieContainer cookieContainer,
                                                            string url, long gid, string token)
        {
            string referer = task.EhUrl.GetGalleryDetailUrl(gid, token);
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            KeyValuePair<string, string>[] result;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                result = TorrentParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }

            return result.ToDictionary(k => k.Key, v => v.Value);
        }

        public static async Task<KeyValuePair<string, KeyValuePair<string, string>[]>> GetArchiveListAsync(EhTask task, CookieContainer cookieContainer,
                                                                          string url, long gid, string token)
        {
            string referer = task.EhUrl.GetGalleryDetailUrl(gid, token);
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            KeyValuePair<string, KeyValuePair<string, string>[]> result;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                result = ArchiveParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }

            return result;
        }

        public static async Task DownloadArchiveAsync(EhTask task, CookieContainer cookieContainer,
                                           long gid, string token, string or, string res)
        {
            if (or == null || or.Length == 0)
            {
                throw new EhException("Invalid form param or: " + or);
            }
            if (res == null || res.Length == 0)
            {
                throw new EhException("Invalid res: " + res);
            }
            var builder = new Dictionary<string, string>
            {
                { "hathdl_xres", res }
            };
            string url = task.EhUrl.GetDownloadArchive(gid, token, or);
            string referer = task.EhUrl.GetGalleryDetailUrl(gid, token);
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(new FormUrlEncodedContent(builder))
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                //throwException(null, code, headers, body, null);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }

            var m = PATTERN_NEED_HATH_CLIENT.Match(body);
            if (m.Success)
            {
                throw new NoHAtHClientException("No H@H client");
            }
        }

        private static async Task<ProfileParser.Result> GetProfileInternalAsync(CookieContainer cookieContainer,
                                                                string url, string referer)
        {
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return ProfileParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<ProfileParser.Result> GetProfileAsync(CookieContainer cookieContainer)
        {
            string url = EhUrl.URL_FORUMS;
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, null).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return await GetProfileInternalAsync(cookieContainer, ForumsParser.Parse(body), url);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<VoteCommentParser.Result> VoteCommentAsync(EhTask task, CookieContainer cookieContainer,
                                                           long apiUid, string apiKey, long gid, string token, long commentId, int commentVote)
        {
            var json = new JObject
            {
                { "method", "votecomment" },
                { "apiuid", apiUid },
                { "apikey", apiKey },
                { "gid", gid },
                { "token", token },
                { "comment_id", commentId },
                { "comment_vote", commentVote }
            };
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.EhUrl.GetApiUrl();
            string referer = task.EhUrl.GetReferer();
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return VoteCommentParser.Parse(body, commentVote);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<VoteTagParser.Result> VoteTagAsync(EhTask task, CookieContainer cookieContainer,
                                                   long apiUid, string apiKey, long gid, string token, string tags, int vote)
        {
            var json = new JObject
            {
                { "method", "taggallery" },
                { "apiuid", apiUid },
                { "apikey", apiKey },
                { "gid", gid },
                { "token", token },
                { "tags", tags },
                { "vote", vote }
            };
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.EhUrl.GetApiUrl();
            string referer = task.EhUrl.GetReferer();
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return VoteTagParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        /**
         * @param image Must be jpeg
         */
        /*
        public static GalleryListParser.Result imageSearch(EhTask task,CookieContainer cookieContainer,
                                                           File image, bool uss, bool osc, bool se)
        {
            MultipartBody.Builder builder = new MultipartBody.Builder();
            builder.setType(MultipartBody.FORM);
            builder.addPart(
                    WebHeaderCollection.of("Content-Disposition", "form-data; name=\"sfile\"; filename=\"a.jpg\""),
                    new StringContent(image, MEDIA_TYPE_JPEG)
            );
            if (uss)
            {
                builder.addPart(
                        WebHeaderCollection.of("Content-Disposition", "form-data; name=\"fs_similar\""),
                        new StringContent("on", null)
                );
            }
            if (osc)
            {
                builder.addPart(
                        WebHeaderCollection.of("Content-Disposition", "form-data; name=\"fs_covers\""),
                        new StringContent("on", null)
                );
            }
            if (se)
            {
                builder.addPart(
                        WebHeaderCollection.of("Content-Disposition", "form-data; name=\"fs_exp\""),
                        new StringContent("on", null)
                );
            }
            builder.addPart(
                    WebHeaderCollection.of("Content-Disposition", "form-data; name=\"f_sfile\""),
                    new StringContent("File Search", null)
            );
            string url = task.mEhUrl.getImageSearchUrl();
            string referer = task.mEhUrl.getReferer();
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(builder.build())
                    .build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            GalleryListParser.Result result;
            int code = -1;
            try
            {
                var response =  await request.SendAsync();

                Log.d(TAG, "" + request.RequestUri.ToString());

                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                result = GalleryListParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            fillGalleryList(task, result.galleryInfoList, url, true);

            return result;
        }
        */

        public static async Task<GalleryPageParser.Result> GetGalleryPageAsync(EhTask task, CookieContainer cookieContainer,
                                                               string url, long gid, string token)
        {
            string referer = task.EhUrl.GetGalleryDetailUrl(gid, token);
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return GalleryPageParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }

        public static async Task<GalleryPageApiParser.Result> GetGalleryPageApiAsync(EhTask task, CookieContainer cookieContainer,
                                                                     long gid, int index, string pToken, string showKey, string previousPToken)
        {
            var json = new JObject
            {
                { "method", "showpage" },
                { "gid", gid },
                { "page", index + 1 },
                { "imgkey", pToken },
                { "showkey", showKey }
            };
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.EhUrl.GetApiUrl();
            string referer = null;
            if (index > 0 && previousPToken != null)
            {
                referer = task.EhUrl.GetPageUrl(gid, index - 1, previousPToken);
            }
            string origin = task.EhUrl.GetOrigin();
            Log.D(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = await request.SendAsync();
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = await response.GetResponseContentAsStringAsync();
                return GalleryPageApiParser.Parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                ThrowException(code, headers, body, e);
                throw;
            }
        }
    }
}
