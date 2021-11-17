using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Client.Parser;
using System;
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

        public Task<VoteCommentParser.Result> VoteComment(long apiUid, string apiKey, long gid, string token, long commentId, int commentVote) 
            => execute<VoteCommentParser.Result>(new EhRequest()
                .setArgs(apiUid, apiKey, gid, token, commentId, commentVote)
                .setMethod(Method.METHOD_VOTE_COMMENT));

        #endregion
    }
}
