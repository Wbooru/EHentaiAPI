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

        private readonly static Regex PATTERN_NEED_HATH_CLIENT = new Regex("(You must have a H@H client assigned to your account to use this feature\\.)");

        private static void doThrowException(HttpWebResponse call, int code, WebHeaderCollection headers,
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
                string error = GalleryNotAvailableParser.parse(body);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    throw new EhException(error);
                }
            }

            if (e is ParseException z)
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

        private static void throwException(HttpWebResponse call, int code, WebHeaderCollection headers,
                                            string body, Exception e)
        {
            try
            {
                doThrowException(call, code, headers, body, e);
            }
            catch (Exception error)
            {
                Log.d("StackTrace", error.StackTrace);
                throw;
            }
        }

        public static string signIn(EhTask task, CookieContainer cookieContainer,
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
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(formBody)
                    .cookies(cookieContainer)
                    .build();

            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return SignInParser.parse(body);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static GalleryListParser.Result getGalleryList(EhTask task, CookieContainer cookieContainer,
                                                              string url)
        {
            string referer = task.mEhUrl.getReferer();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

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
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                result = GalleryListParser.parse(task.mEhUrl.getSettings(), body);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            return result;
        }

        // At least, GalleryInfo contain valid gid and token
        public static List<GalleryInfo> fillGalleryListByApi(EhTask task, CookieContainer cookieContainer,
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
                    doFillGalleryListByApi(task, cookieContainer, requestItems, referer);
                    requestItems.Clear();
                }
            }
            return galleryInfoList;
        }

        private static void doFillGalleryListByApi(EhTask task, CookieContainer cookieContainer,
                                                   List<GalleryInfo> galleryInfoList, string referer)
        {
            var json = new JObject();
            json.Add("method", "gdata");
            var ja = new JArray();
            for (int i = 0, size = galleryInfoList.Count; i < size; i++)
            {
                GalleryInfo gi = galleryInfoList.ElementAt(i);
                var g = new JArray();
                g.Add(gi.gid);
                g.Add(gi.token);
                ja.Add(g);
            }
            json.Add("gidlist", ja);
            json.Add("namespace", 1);
            string url = task.mEhUrl.getApiUrl();
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .cookies(cookieContainer)
                    .post(new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON))
                    .build();

            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                GalleryApiParser.parse(task.mEhUrl.getSettings(), body, galleryInfoList);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static GalleryDetail getGalleryDetail(EhTask task, CookieContainer cookieContainer,
                                                     string url)
        {
            string referer = task.mEhUrl.getReferer();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                string html = EventPaneParser.parse(body);
                /*
                if (html != null)
                {
                    EhApplication.getInstance().showEventPane(html);
                }
                */
                return GalleryDetailParser.parse(task.mEhUrl, body, url);
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }


        public static KeyValuePair<PreviewSet, int> getPreviewSet(
                 EhTask task, CookieContainer cookieContainer, string url)
        {
            string referer = task.mEhUrl.getReferer();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();

            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return KeyValuePair.Create(GalleryDetailParser.parsePreviewSet(task.mEhUrl, body),
                        GalleryDetailParser.parsePreviewPages(body));
            }
            catch (Exception e)
            {
                ////ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static RateGalleryParser.Result rateGallery(EhTask task, CookieContainer cookieContainer,
                                                            long apiUid, string apiKey, long gid,
                                                           string token, float rating)
        {
            var json = new JObject();
            json.Add("method", "rategallery");
            json.Add("apiuid", apiUid);
            json.Add("apikey", apiKey);
            json.Add("gid", gid);
            json.Add("token", token);
            json.Add("rating", (int)Math.Ceiling(rating * 2));
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.mEhUrl.getApiUrl();
            string referer = task.mEhUrl.getGalleryDetailUrl(gid, token);
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();

            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return RateGalleryParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static GalleryCommentList commentGallery(EhTask task, CookieContainer cookieContainer,
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
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, url, origin)
                    .post(new FormUrlEncodedContent(bodyMap))
                    .cookies(cookieContainer)
                    .build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var document = Utils.Document.parse(body);

                var elements = document.select("#chd + p");
                if (elements.Length > 0)
                {
                    throw new EhException(elements[0].Text());
                }

                return GalleryDetailParser.parseComments(document);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static string getGalleryToken(EhTask task, CookieContainer cookieContainer,
                                             long gid, string gtoken, int page)
        {
            var obj = new
            {
                method = "gtoken",
                pagelist = new[] { new object[] { gid, gtoken, page + 1 } }
            };
            var d = JsonConvert.SerializeObject(obj);

            var requestBody = new StringContent(d, Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.mEhUrl.getApiUrl();
            string referer = task.mEhUrl.getReferer();
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return GalleryTokenApiParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static FavoritesParser.Result getFavorites(EhTask task, CookieContainer cookieContainer,
                                                          string url, bool callApi)
        {
            string referer = task.mEhUrl.getReferer();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            FavoritesParser.Result result;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                result = FavoritesParser.parse(task.mEhUrl.getSettings(), body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            return result;
        }

        /**
         * @param dstCat -1 for delete, 0 - 9 for cloud favorite, others throw Exception
         * @param note   max 250 characters
         */
        public static void addFavorites(EhTask task, CookieContainer cookieContainer,
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
            var builder = new Dictionary<string, string>();
            builder.Add("favcat", catStr);
            builder.Add("favnote", note != null ? note : "");
            // submit=Add+to+Favorites is not necessary, just use submit=Apply+Changes all the time
            builder.Add("submit", "Apply Changes");
            builder.Add("update", "1");
            string url = task.mEhUrl.getAddFavorites(gid, token);
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, url, origin)
                    .post(new FormUrlEncodedContent(builder))
                    .cookies(cookieContainer)
                    .build();

            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                throwException(null, code, headers, body, null);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            return;
        }

        public static void addFavoritesRange(EhTask task, CookieContainer cookieContainer,
                                             long[] gidArray, string[] tokenArray, int dstCat)
        {
            Debug.Assert(gidArray.Length == tokenArray.Length);
            for (int i = 0, n = gidArray.Length; i < n; i++)
            {
                addFavorites(task, cookieContainer, gidArray[i], tokenArray[i], dstCat, null);
            }
            return;
        }

        public static FavoritesParser.Result modifyFavorites(EhTask task, CookieContainer cookieContainer,
                                                             string url, long[] gidArray, int dstCat, bool callApi)
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
            var builder = new Dictionary<string, string>();
            builder.Add("ddact", catStr);
            foreach (long gid in gidArray)
            {
                builder.Add("modifygids[]", gid.ToString());
            }
            builder.Add("apply", "Apply");
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, url, origin)
                    .post(new FormUrlEncodedContent(builder))
                    .cookies(cookieContainer)
                    .build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            FavoritesParser.Result result;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                result = FavoritesParser.parse(task.mEhUrl.getSettings(), body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            return result;
        }

        public static Dictionary<string, string> getTorrentList(EhTask task, CookieContainer cookieContainer,
                                                            string url, long gid, string token)
        {
            string referer = task.mEhUrl.getGalleryDetailUrl(gid, token);
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            KeyValuePair<string, string>[] result;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                result = TorrentParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            return result.ToDictionary(k => k.Key, v => v.Value);
        }

        public static KeyValuePair<string, KeyValuePair<string, string>[]> getArchiveList(EhTask task, CookieContainer cookieContainer,
                                                                          string url, long gid, string token)
        {
            string referer = task.mEhUrl.getGalleryDetailUrl(gid, token);
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            KeyValuePair<string, KeyValuePair<string, string>[]> result;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                result = ArchiveParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            return result;
        }

        public static void downloadArchive(EhTask task, CookieContainer cookieContainer,
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
            var builder = new Dictionary<string, string>();
            builder.Add("hathdl_xres", res);
            string url = task.mEhUrl.getDownloadArchive(gid, token, or);
            string referer = task.mEhUrl.getGalleryDetailUrl(gid, token);
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(new FormUrlEncodedContent(builder))
                    .cookies(cookieContainer)
                    .build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                throwException(null, code, headers, body, null);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }

            var m = PATTERN_NEED_HATH_CLIENT.Match(body);
            if (m.Success)
            {
                throw new NoHAtHClientException("No H@H client");
            }
        }

        private static ProfileParser.Result getProfileInternal(EhTask task, CookieContainer cookieContainer,
                                                                string url, string referer)
        {
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return ProfileParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static ProfileParser.Result getProfile(EhTask task, CookieContainer cookieContainer)
        {
            string url = EhUrl.URL_FORUMS;
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, null).cookies(cookieContainer).build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return getProfileInternal(task, cookieContainer, ForumsParser.parse(body), url);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static VoteCommentParser.Result voteComment(EhTask task, CookieContainer cookieContainer,
                                                           long apiUid, string apiKey, long gid, string token, long commentId, int commentVote)
        {
            var json = new JObject();
            json.Add("method", "votecomment");
            json.Add("apiuid", apiUid);
            json.Add("apikey", apiKey);
            json.Add("gid", gid);
            json.Add("token", token);
            json.Add("comment_id", commentId);
            json.Add("comment_vote", commentVote);
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.mEhUrl.getApiUrl();
            string referer = task.mEhUrl.getReferer();
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();

            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return VoteCommentParser.parse(body, commentVote);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static VoteTagParser.Result voteTag(EhTask task, CookieContainer cookieContainer,
                                                   long apiUid, string apiKey, long gid, string token, string tags, int vote)
        {
            var json = new JObject();
            json.Add("method", "taggallery");
            json.Add("apiuid", apiUid);
            json.Add("apikey", apiKey);
            json.Add("gid", gid);
            json.Add("token", token);
            json.Add("tags", tags);
            json.Add("vote", vote);
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.mEhUrl.getApiUrl();
            string referer = task.mEhUrl.getReferer();
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return VoteTagParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
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
                var response = request.GetResponse() as HttpWebResponse;

                Log.d(TAG, "" + request.RequestUri.ToString());

                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
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

        public static GalleryPageParser.Result getGalleryPage(EhTask task, CookieContainer cookieContainer,
                                                               string url, long gid, string token)
        {
            string referer = task.mEhUrl.getGalleryDetailUrl(gid, token);
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer).cookies(cookieContainer).build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return GalleryPageParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }

        public static GalleryPageApiParser.Result getGalleryPageApi(EhTask task, CookieContainer cookieContainer,
                                                                     long gid, int index, string pToken, string showKey, string previousPToken)
        {
            var json = new JObject();
            json.Add("method", "showpage");
            json.Add("gid", gid);
            json.Add("page", index + 1);
            json.Add("imgkey", pToken);
            json.Add("showkey", showKey);
            var requestBody = new StringContent(json.ToString(), Encoding.UTF8, MEDIA_TYPE_JSON);
            string url = task.mEhUrl.getApiUrl();
            string referer = null;
            if (index > 0 && previousPToken != null)
            {
                referer = task.mEhUrl.getPageUrl(gid, index - 1, previousPToken);
            }
            string origin = task.mEhUrl.getOrigin();
            Log.d(TAG, url);
            var request = new EhRequestBuilder(url, referer, origin)
                    .post(requestBody)
                    .cookies(cookieContainer)
                    .build();


            // Put call
            if (null != task)
            {
                task.setCall(request);
            }

            string body = null;
            WebHeaderCollection headers = null;
            int code = -1;
            try
            {
                var response = request.GetResponse() as HttpWebResponse;
                code = (int)response.StatusCode;
                headers = response.Headers;
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return GalleryPageApiParser.parse(body);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                throwException(null, code, headers, body, e);
                throw;
            }
        }
    }
}
