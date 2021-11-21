using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EHentaiAPI.Client.EhClient;

namespace EHentaiAPI.Client
{
    public class EhTask
    {
        public Task<object> Task { get; private set; }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CookieContainer cookieContainer;

        public Method Method { get; private set; }
        public EhRequestCallback Callback { get; private set; }
        public EhUrl EhUrl { get; private set; }

        public EhTask(Method method, EhUrl ehUrl, EhRequestCallback callback, CookieContainer cookieContainer)
        {
            Method = method;
            Callback = callback;
            EhUrl = ehUrl;
            this.cookieContainer = cookieContainer;
        }

        private async Task<object> DoInBackground(params object[] @params)
        {
            try
            {
                switch (Method)
                {
                    case Method.METHOD_SIGN_IN:
                        return await EhEngine.SignInAsync(cookieContainer, (string)@params[0], (string)@params[1]);
                    case Method.METHOD_GET_GALLERY_LIST:
                        return await EhEngine.GetGalleryListAsync(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_GALLERY_DETAIL:
                        return await EhEngine.GetGalleryDetailAsync(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_PREVIEW_SET:
                        return await EhEngine.GetPreviewSetAsync(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_RATE_GALLERY:
                        return await EhEngine.RateGalleryAsync(this, cookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (float)@params[4]);
                    case Method.METHOD_GET_COMMENT_GALLERY:
                        return await EhEngine.CommentGalleryAsync(this, cookieContainer, (string)@params[0], (string)@params[1], (long?)@params[2]);
                    case Method.METHOD_GET_GALLERY_TOKEN:
                        return await EhEngine.GetGalleryTokenAsync(this, cookieContainer, (long)@params[0], (string)@params[1], (int)@params[2]);
                    case Method.METHOD_GET_FAVORITES:
                        return await EhEngine.GetFavoritesAsync(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_ADD_FAVORITES:
                        await EhEngine.AddFavoritesAsync(this, cookieContainer, (long)@params[0], (string)@params[1], (int)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_ADD_FAVORITES_RANGE:
                        await EhEngine.AddFavoritesRangeAsync(this, cookieContainer, (long[])@params[0], (string[])@params[1], (int)@params[2]);
                        break;
                    case Method.METHOD_MODIFY_FAVORITES:
                        return await EhEngine.ModifyFavoritesAsync(this, cookieContainer, (string)@params[0], (long[])@params[1], (int)@params[2]);
                    case Method.METHOD_GET_TORRENT_LIST:
                        return await EhEngine.GetTorrentListAsync(this, cookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_GET_PROFILE:
                        return await EhEngine.GetProfileAsync(cookieContainer);
                    case Method.METHOD_VOTE_COMMENT:
                        return await EhEngine.VoteCommentAsync(this, cookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (long)@params[4], (int)@params[5]);
                    //case Method.METHOD_IMAGE_SEARCH:
                    //    return EhEngine.imageSearch(this,CookieContainer, (File)@params[0], (Boolean)@params[1], (Boolean)@params[2], (Boolean)@params[3]);
                    case Method.METHOD_ARCHIVE_LIST:
                        return await EhEngine.GetArchiveListAsync(this, cookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_DOWNLOAD_ARCHIVE:
                        await EhEngine.DownloadArchiveAsync(this, cookieContainer, (long)@params[0], (string)@params[1], (string)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_VOTE_TAG:
                        return await EhEngine.VoteTagAsync(this, cookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (string)@params[4], (int)@params[5]);
                    default:
                        throw new InvalidOperationException("Can't detect method :" + Method);
                }
            }
            catch (Exception e)
            {
                return e;
            }

            return default;
        }

        public async void ExecuteOnExecutor(object[] args)
        {
            Task = DoInBackground(args);
            var r = await Task;
            if (cancellationTokenSource.IsCancellationRequested)
            {
                Callback?.OnCancel?.Invoke();
            }
            else
            {
                switch (Task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        Callback?.OnSuccess?.Invoke(r);
                        break;
                    case TaskStatus.Canceled:
                        Callback?.OnCancel?.Invoke();
                        break;
                    case TaskStatus.Faulted:
                        Callback?.OnFailure?.Invoke(r as Exception);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
