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
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private Method mMethod;
        private EhRequestCallback mCallback;
        public EhUrl mEhUrl;
        private HttpWebRequest request;
        private CookieContainer mCookieContainer;

        public EhTask(Method method, EhUrl ehUrl, EhRequestCallback callback, EhConfig ehConfig, CookieContainer cookieContainer)
        {
            mMethod = method;
            mCallback = callback;
            mEhUrl = ehUrl;
            mCookieContainer = cookieContainer;
        }

        private object doInBackground(params object[] @params)
        {
            try
            {
                switch (mMethod)
                {
                    case Method.METHOD_SIGN_IN:
                        return EhEngine.signIn(this, mCookieContainer, (string)@params[0], (string)@params[1]);
                    case Method.METHOD_GET_GALLERY_LIST:
                        return EhEngine.getGalleryList(this, mCookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_GALLERY_DETAIL:
                        return EhEngine.getGalleryDetail(this, mCookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_PREVIEW_SET:
                        return EhEngine.getPreviewSet(this, mCookieContainer, (string)@params[0]);
                    case Method.METHOD_GET_RATE_GALLERY:
                        return EhEngine.rateGallery(this, mCookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (float)@params[4]);
                    case Method.METHOD_GET_COMMENT_GALLERY:
                        return EhEngine.commentGallery(this, mCookieContainer, (string)@params[0], (string)@params[1], (string)@params[2]);
                    case Method.METHOD_GET_GALLERY_TOKEN:
                        return EhEngine.getGalleryToken(this, mCookieContainer, (long)@params[0], (string)@params[1], (int)@params[2]);
                    case Method.METHOD_GET_FAVORITES:
                        return EhEngine.getFavorites(this, mCookieContainer, (string)@params[0], (bool)@params[1]);
                    case Method.METHOD_ADD_FAVORITES:
                        EhEngine.addFavorites(this, mCookieContainer, (long)@params[0], (string)@params[1], (int)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_ADD_FAVORITES_RANGE:
                        EhEngine.addFavoritesRange(this, mCookieContainer, (long[])@params[0], (string[])@params[1], (int)@params[2]);
                        break;
                    case Method.METHOD_MODIFY_FAVORITES:
                        return EhEngine.modifyFavorites(this, mCookieContainer, (string)@params[0], (long[])@params[1], (int)@params[2], (bool)@params[3]);
                    case Method.METHOD_GET_TORRENT_LIST:
                        return EhEngine.getTorrentList(this, mCookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_GET_PROFILE:
                        return EhEngine.getProfile(this, mCookieContainer);
                    case Method.METHOD_VOTE_COMMENT:
                        return EhEngine.voteComment(this, mCookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (long)@params[4], (int)@params[5]);
                    //case Method.METHOD_IMAGE_SEARCH:
                    //    return EhEngine.imageSearch(this,CookieContainer, (File)@params[0], (Boolean)@params[1], (Boolean)@params[2], (Boolean)@params[3]);
                    case Method.METHOD_ARCHIVE_LIST:
                        return EhEngine.getArchiveList(this, mCookieContainer, (string)@params[0], (long)@params[1], (string)@params[2]);
                    case Method.METHOD_DOWNLOAD_ARCHIVE:
                        EhEngine.downloadArchive(this, mCookieContainer, (long)@params[0], (string)@params[1], (string)@params[2], (string)@params[3]);
                        break;
                    case Method.METHOD_VOTE_TAG:
                        return EhEngine.voteTag(this, mCookieContainer, (long)@params[0], (string)@params[1], (long)@params[2], (string)@params[3], (string)@params[4], (int)@params[5]);
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

        public async void executeOnExecutor(TaskScheduler scheduler, object[] args)
        {
            this.task = new Task<object>(() => doInBackground(args), cancellationTokenSource.Token);
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

        public void stop()
        {
            cancellationTokenSource.Cancel();
        }

        public void setCall(HttpWebRequest request)
        {
            this.request = request;
        }

        public Task<object> getTask()
        {
            return this.task;
        }
    }

}
