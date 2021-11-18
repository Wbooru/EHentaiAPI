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

        private object DoInBackground(params object[] @params)
        {
            try
            {
                switch (Method)
                {
                    case Method.METHOD_SIGN_IN:
                        return EhEngine.SignIn(this, cookieContainer, (string)@params[0], (string)@params[1]);
                    case Method.METHOD_GET_GALLERY_LIST:
                        return EhEngine.GetGalleryList(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_GALLERY_DETAIL:
                        return EhEngine.GetGalleryDetail(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_PREVIEW_SET:
                        return EhEngine.GetPreviewSet(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_RATE_GALLERY:
                        return EhEngine.RateGallery(this, cookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (float)@params[4]);
                    case Method.METHOD_GET_COMMENT_GALLERY:
                        return EhEngine.CommentGallery(this, cookieContainer, (string)@params[0], (string)@params[1], (long?)@params[2]);
                    case Method.METHOD_GET_GALLERY_TOKEN:
                        return EhEngine.GetGalleryToken(this, cookieContainer, (long)@params[0], (string)@params[1], (int)@params[2]);
                    case Method.METHOD_GET_FAVORITES:
                        return EhEngine.GetFavorites(this, cookieContainer, (string)@params[0]);
                    case Method.METHOD_ADD_FAVORITES:
                        EhEngine.AddFavorites(this, cookieContainer, (long)@params[0], (string)@params[1], (int)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_ADD_FAVORITES_RANGE:
                        EhEngine.AddFavoritesRange(this, cookieContainer, (long[])@params[0], (string[])@params[1], (int)@params[2]);
                        break;
                    case Method.METHOD_MODIFY_FAVORITES:
                        return EhEngine.ModifyFavorites(this, cookieContainer, (string)@params[0], (long[])@params[1], (int)@params[2]);
                    case Method.METHOD_GET_TORRENT_LIST:
                        return EhEngine.GetTorrentList(this, cookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_GET_PROFILE:
                        return EhEngine.GetProfile(this, cookieContainer);
                    case Method.METHOD_VOTE_COMMENT:
                        return EhEngine.VoteComment(this, cookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (long)@params[4], (int)@params[5]);
                    //case Method.METHOD_IMAGE_SEARCH:
                    //    return EhEngine.imageSearch(this,CookieContainer, (File)@params[0], (Boolean)@params[1], (Boolean)@params[2], (Boolean)@params[3]);
                    case Method.METHOD_ARCHIVE_LIST:
                        return EhEngine.GetArchiveList(this, cookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_DOWNLOAD_ARCHIVE:
                        EhEngine.DownloadArchive(this, cookieContainer, (long)@params[0], (string)@params[1], (string)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_VOTE_TAG:
                        return EhEngine.VoteTag(this, cookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (string)@params[4], (int)@params[5]);
                    default:
                        throw new InvalidOperationException("Can't detect method :" + Method);
                }
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }

        public async void ExecuteOnExecutor(TaskScheduler scheduler, object[] args)
        {
            Task = new Task<object>(() => DoInBackground(args), cancellationTokenSource.Token);
            Task.Start(scheduler);
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
