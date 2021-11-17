using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Client.Parser;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public class EhClient
    {
        public TaskScheduler Scheduler { get; set; } = TaskScheduler.Default;
        public CookieContainer Cookies { get; set; } = new CookieContainer();

        private EhUrl ehUrl;
        public EhUrl EhUrl
        {
            get
            {
                if (ehUrl is null)
                {
                    throw new EhException("Please setup EhClient::Settings before using EhClient::EhUrl");
                }
                return ehUrl;
            }
        }

        private Settings setting;
        public Settings Settings
        {
            get
            {
                return setting;
            }
            set
            {
                setting = value;
                ehUrl = new EhUrl(value);
            }
        }

        public EhClient(Settings setting = default)
        {
            Settings = setting ?? new Settings();
        }

        public enum Method
        {
            METHOD_SIGN_IN,
            METHOD_GET_GALLERY_LIST,
            METHOD_GET_GALLERY_DETAIL,
            METHOD_GET_PREVIEW_SET,
            METHOD_GET_RATE_GALLERY,
            METHOD_GET_COMMENT_GALLERY,
            METHOD_GET_GALLERY_TOKEN,
            METHOD_GET_FAVORITES,
            METHOD_ADD_FAVORITES,
            METHOD_ADD_FAVORITES_RANGE,
            METHOD_MODIFY_FAVORITES,
            METHOD_GET_TORRENT_LIST,
            METHOD_GET_PROFILE,
            METHOD_VOTE_COMMENT,
            METHOD_IMAGE_SEARCH,
            METHOD_ARCHIVE_LIST,
            METHOD_DOWNLOAD_ARCHIVE,
            METHOD_VOTE_TAG,
        }

        public Task execute(EhRequest request) => execute<object>(request);

        public async Task<T> execute<T>(EhRequest request)
        {
            if (!request.isCancelled())
            {
                var task = new EhTask(request.getMethod(), EhUrl, request.getCallback(), request.getEhConfig(), Cookies);
                task.executeOnExecutor(Scheduler, request.getArgs());
                request.task = task;
                var r = await task.getTask();
                if (r is Exception e)
                    throw e;
                return (T)r;
            }
            else
            {
                request.getCallback().OnCancel();
                return default;
            }
        }

        #region New API Usages

        public Task<string> SignIn(string username, string password)
            => execute<string>(new EhRequest()
                .setArgs(username, password)
                .setMethod(Method.METHOD_SIGN_IN));

        public Task<GalleryListParser.Result> GetGalleryList(string url)
            => execute<GalleryListParser.Result>(new EhRequest()
                .setArgs(url)
                .setMethod(Method.METHOD_GET_GALLERY_LIST));

        public Task<GalleryDetail> GetGalleryDetail(string detailUrl)
            => execute<GalleryDetail>(new EhRequest()
                .setArgs(detailUrl)
                .setMethod(Method.METHOD_GET_GALLERY_DETAIL));

        public Task<VoteCommentParser.Result> VoteComment(GalleryDetail detail, GalleryComment comment, int vote)
            => VoteComment(detail.apiUid, detail.apiKey, detail.gid, detail.token, comment.id, vote);

        public Task<VoteCommentParser.Result> VoteComment(long apiUid, string apiKey, long gid, string token, long commentId, int commentVote)
            => execute<VoteCommentParser.Result>(new EhRequest()
                .setArgs(apiUid, apiKey, gid, token, commentId, commentVote)
                .setMethod(Method.METHOD_VOTE_COMMENT));

        public Task<Dictionary<string, string>> GetTorrentList(GalleryDetail detail)
            => GetTorrentList(detail.torrentUrl, detail.gid, detail.token);

        public Task<Dictionary<string, string>> GetTorrentList(string torrentUrl, long gid, string token)
            => execute<Dictionary<string, string>>(new EhRequest()
                .setArgs(torrentUrl, gid, token)
                .setMethod(Method.METHOD_GET_TORRENT_LIST));

        public Task<KeyValuePair<string, KeyValuePair<string, string>[]>> GetArchiveList(GalleryDetail detail)
            => GetArchiveList(detail.archiveUrl, detail.gid, detail.token);

        public Task<KeyValuePair<string, KeyValuePair<string, string>[]>> GetArchiveList(string archiveUrl, long gid, string token)
            => execute<KeyValuePair<string, KeyValuePair<string, string>[]>>(new EhRequest()
                .setArgs(archiveUrl, gid, token)
                .setMethod(Method.METHOD_ARCHIVE_LIST));

        public Task<VoteTagParser.Result> VoteTag(GalleryDetail detail, string tags, int vote)
            => VoteTag(detail.apiUid, detail.apiKey, detail.gid, detail.token, tags, vote);

        public Task<VoteTagParser.Result> VoteTag(long apiUid, string apiKey, long gid, string token, string tags, int vote)
            => execute<VoteTagParser.Result>(new EhRequest()
                .setArgs(apiUid, apiKey, gid, token, tags, vote)
                .setMethod(Method.METHOD_VOTE_TAG));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="rating">1~5 , also 3.5,etc.</param>
        /// <returns></returns>
        public Task<RateGalleryParser.Result> RateGallery(GalleryDetail detail, float rating)
            => RateGallery(detail.apiUid, detail.apiKey, detail.gid, detail.token, rating);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="rating">1~5 , also 3.5,etc.</param>
        /// <returns></returns>
        public Task<RateGalleryParser.Result> RateGallery(long apiUid, string apiKey, long gid, string token, float rating)
            => execute<RateGalleryParser.Result>(new EhRequest()
                .setArgs(apiUid, apiKey, gid, token, rating)
                .setMethod(Method.METHOD_GET_RATE_GALLERY));

        public Task<GalleryCommentList> CommentNewGallery(GalleryDetail detail, string commentContent)
            => CommentNewGallery(detail.url, commentContent);

        public Task<GalleryCommentList> CommentNewGallery(string url, string commentContent)
            => execute<GalleryCommentList>(new EhRequest()
                .setArgs(url, commentContent, null)
                .setMethod(Method.METHOD_GET_COMMENT_GALLERY));

        public Task<GalleryCommentList> ModifyCommentGallery(GalleryDetail detail, string newCommentContent, GalleryComment comment)
            => ModifyCommentGallery(detail.url, newCommentContent, comment.id);

        public Task<GalleryCommentList> ModifyCommentGallery(string url, string commentContent, long commendId)
            => execute<GalleryCommentList>(new EhRequest()
                .setArgs(url, commentContent, commendId)
                .setMethod(Method.METHOD_GET_COMMENT_GALLERY));

        /// <summary>
        /// 通过具体页面url获取画廊的token,比如
        /// https://e-hentai.org/s/7b13386d6b/2062872-10
        /// </summary>
        /// <param name="url">具体页面url</param>
        /// <returns></returns>
        public Task<string> GetGalleryToken(string url)
        {
            var result2 = GalleryPageUrlParser.parse(url, false);
            return GetGalleryToken(result2.gid, result2.pToken, result2.page);
        }

        /// <summary>
        /// 通过具体页面url获取画廊的token,比如
        /// https://e-hentai.org/s/7b13386d6b/2062872-10
        /// </summary>
        /// <param name="gid">2062872</param>
        /// <param name="gtoken">7b13386d6b</param>
        /// <param name="page">10</param>
        /// <returns>token fb6abc76c6</returns>
        public Task<string> GetGalleryToken(long gid, string gtoken, int page)
            => execute<string>(new EhRequest()
                .setArgs(gid, gtoken, page)
                .setMethod(Method.METHOD_GET_GALLERY_TOKEN));

        /// <summary>
        /// 获取指定的收藏列表
        /// </summary>
        /// <param name="url">获取列表的url地址,比如https://e-hentai.org/favorites.php?favcat=all&page=2 ,可通过<class>FavListUrlBuilder</class>快速构造</param>
        /// <returns></returns>
        public Task<FavoritesParser.Result> GetFavorites(string url)
            => execute<FavoritesParser.Result>(new EhRequest()
                .setArgs(url)
                .setMethod(Method.METHOD_GET_FAVORITES));

        public Task<FavoritesParser.Result> GetFavorites(FavListUrlBuilder urlBuilder)
            => GetFavorites(urlBuilder.build());

        public Task AddFavorite(GalleryDetail detail, int dstCat, string note)
            => AddFavorite(detail.gid, detail.token, dstCat, note);

        public Task AddFavorite(long gid, string token, int dstCat, string note)
            => execute(new EhRequest()
                .setArgs(gid, token, dstCat, note)
                .setMethod(Method.METHOD_ADD_FAVORITES));

        public Task AddFavoritesRange(long[] gidArray, string[] tokenArray, int dstCat)
            => execute(new EhRequest()
                .setArgs(gidArray, tokenArray, dstCat)
                .setMethod(Method.METHOD_ADD_FAVORITES_RANGE));

        public Task ModifyFavorites(FavListUrlBuilder urlBuilder, long[] gidArray, int dstCat)
            => ModifyFavorites(urlBuilder.build(), gidArray, dstCat);

        public Task ModifyFavorites(string url, long[] gidArray, int dstCat)
            => execute(new EhRequest()
                .setArgs(url, gidArray, dstCat)
                .setMethod(Method.METHOD_MODIFY_FAVORITES));

        #endregion
    }
}
