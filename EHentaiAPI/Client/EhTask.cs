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
        private Task<object> task;
        public Task<object> Task => task;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private Method mMethod;
        private EhRequestCallback mCallback;
        public EhUrl mEhUrl;
        private CookieContainer mCookieContainer;

        public EhTask(Method method, EhUrl ehUrl, EhRequestCallback callback, CookieContainer cookieContainer)
        {
            mMethod = method;
            mCallback = callback;
            mEhUrl = ehUrl;
            mCookieContainer = cookieContainer;
        }

        private object DoInBackground(params object[] @params)
        {
            try
            {
                switch (mMethod)
                {
                    case Method.METHOD_SIGN_IN:
                        return EhEngine.SignIn(this, mCookieContainer, (string)@params[0], (string)@params[1]);
                    case Method.METHOD_GET_GALLERY_LIST:
                        return EhEngine.GetGalleryList(this, mCookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_GALLERY_DETAIL:
                        return EhEngine.GetGalleryDetail(this, mCookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_PREVIEW_SET:
                        return EhEngine.GetPreviewSet(this, mCookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_RATE_GALLERY:
                        return EhEngine.RateGallery(this, mCookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (float)@params[4]);
                    case Method.METHOD_GET_COMMENT_GALLERY:
                        return EhEngine.CommentGallery(this, mCookieContainer, (string)@params[0], (string)@params[1], (long?)@params[2]);
                    case Method.METHOD_GET_GALLERY_TOKEN:
                        return EhEngine.GetGalleryToken(this, mCookieContainer, (long)@params[0], (string)@params[1], (int)@params[2]);
                    case Method.METHOD_GET_FAVORITES:
                        return EhEngine.GetFavorites(this, mCookieContainer, (string)@params[0]);
                    case Method.METHOD_ADD_FAVORITES:
                        EhEngine.AddFavorites(this, mCookieContainer, (long)@params[0], (string)@params[1], (int)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_ADD_FAVORITES_RANGE:
                        EhEngine.AddFavoritesRange(this, mCookieContainer, (long[])@params[0], (string[])@params[1], (int)@params[2]);
                        break;
                    case Method.METHOD_MODIFY_FAVORITES:
                        return EhEngine.ModifyFavorites(this, mCookieContainer, (string)@params[0], (long[])@params[1], (int)@params[2]);
                    case Method.METHOD_GET_TORRENT_LIST:
                        return EhEngine.GetTorrentList(this, mCookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_GET_PROFILE:
                        return EhEngine.GetProfile(this, mCookieContainer);
                    case Method.METHOD_VOTE_COMMENT:
                        return EhEngine.VoteComment(this, mCookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (long)@params[4], (int)@params[5]);
                    //case Method.METHOD_IMAGE_SEARCH:
                    //    return EhEngine.imageSearch(this,CookieContainer, (File)@params[0], (Boolean)@params[1], (Boolean)@params[2], (Boolean)@params[3]);
                    case Method.METHOD_ARCHIVE_LIST:
                        return EhEngine.GetArchiveList(this, mCookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_DOWNLOAD_ARCHIVE:
                        EhEngine.DownloadArchive(this, mCookieContainer, (long)@params[0], (string)@params[1], (string)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_VOTE_TAG:
                        return EhEngine.VoteTag(this, mCookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (string)@params[4], (int)@params[5]);
                    default:
                        throw new InvalidOperationException("Can't detect method :" + mMethod);
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
            this.task = new Task<object>(() => DoInBackground(args), cancellationTokenSource.Token);
            task.Start(scheduler);
            var r = await task;
            if (cancellationTokenSource.IsCancellationRequested)
            {
                mCallback?.OnCancel?.Invoke();
            }
            else
            {
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        mCallback?.OnSuccess?.Invoke(r);
                        break;
                    case TaskStatus.Canceled:
                        mCallback?.OnCancel?.Invoke();
                        break;
                    case TaskStatus.Faulted:
                        mCallback?.OnFailure?.Invoke(r as Exception);
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
