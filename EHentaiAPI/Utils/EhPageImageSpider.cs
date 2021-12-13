using EHentaiAPI.Client;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils
{
    public class EhPageImageSpider
    {
        private static readonly string[] URL_509_SUFFIX_ARRAY = {
            "/509.gif",
            "/509s.gif"
        };

        internal class SpiderInfo
        {
            public string ImageUrl { get; set; }
            public string SkipHathKey { get; set; }
            public string OriginImageUrl { get; set; }
            public string ShowKey { get; set; }

            public override string ToString() => $"ShowKey:{ShowKey} SkipHathKey:{SkipHathKey} OriginImageUrl:{OriginImageUrl}";
        }

        public class DownloadReporter
        {
            private readonly SpiderTask task;

            public DownloadReporter(SpiderTask task) => this.task = task;

            public long CurrentDownloadingLength
            {
                get { return task.CurrentDownloadingLength; }
                set
                {
                    task.CurrentDownloadingLength = value;
                }
            }

            public long TotalDownloadLength
            {
                get { return task.TotalDownloadLength; }
                set
                {
                    task.TotalDownloadLength = value;
                }
            }

            public override string ToString() => $"({1.0 * CurrentDownloadingLength / TotalDownloadLength * 100:F2}%) CurrentDownloadingLength:{CurrentDownloadingLength} TotalDownloadLength:{TotalDownloadLength}";
        }

        public class SpiderTask : INotifyPropertyChanged
        {
            public enum TaskStatus
            {
                NotStart,
                FetechingInfo,
                DownloadingImage,
                Finish,
                Error
            }

            public TaskStatus CurrentStatus
            {
                get
                {
                    var previewTaskStatus = PreviewTask.Status;
                    var downloadTaskStatus = PreviewTask.Status;

                    if (previewTaskStatus == System.Threading.Tasks.TaskStatus.Faulted ||
                        previewTaskStatus == System.Threading.Tasks.TaskStatus.Canceled ||
                        downloadTaskStatus == System.Threading.Tasks.TaskStatus.Faulted ||
                        downloadTaskStatus == System.Threading.Tasks.TaskStatus.Canceled)
                    {
                        return TaskStatus.Error;
                    }

                    if (previewTaskStatus != System.Threading.Tasks.TaskStatus.RanToCompletion)
                    {
                        return TaskStatus.FetechingInfo;
                    }

                    if (previewTaskStatus == System.Threading.Tasks.TaskStatus.RanToCompletion &&
                        downloadTaskStatus != System.Threading.Tasks.TaskStatus.RanToCompletion)
                    {
                        return TaskStatus.DownloadingImage;
                    }

                    if (previewTaskStatus == System.Threading.Tasks.TaskStatus.RanToCompletion &&
                        downloadTaskStatus == System.Threading.Tasks.TaskStatus.RanToCompletion)
                    {
                        return TaskStatus.Finish;
                    }

                    return TaskStatus.NotStart;
                }
            }

            internal TaskCompletionSource<SpiderInfo> PreviewTaskSource { get; set; } = new();
            internal TaskCompletionSource<object> DownloadTaskSouce { get; set; } = new();

            internal Task<SpiderInfo> PreviewTask => PreviewTaskSource.Task;
            public Task<object> DownloadTask => DownloadTaskSouce.Task;

            public GalleryPreview Preview { get; set; }

            private long currentDownloadingLength;
            public long CurrentDownloadingLength
            {
                get { return currentDownloadingLength; }
                set
                {
                    currentDownloadingLength = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDownloadingLength)));
                }
            }

            private long totalDownloadLength;
            public long TotalDownloadLength
            {
                get { return totalDownloadLength; }
                set
                {
                    totalDownloadLength = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalDownloadLength)));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public override string ToString() => $"{CurrentStatus} ({1.0 * CurrentDownloadingLength / TotalDownloadLength * 100:F2}%) PreviewPosition:{Preview?.Position}";
        }

        public EhPageImageSpider(EhClient client, GalleryDetail detail, Func<string, DownloadReporter, Task<object>> imageDownloadFunc)
        {
            this.client = client;
            this.detail = detail;
            this.imageDownloadFunc = imageDownloadFunc;
            this.detail = detail;
            previews = new(client, detail);
        }

        private Dictionary<int, SpiderTask> tasks = new();
        private readonly EhClient client;
        private readonly GalleryDetail detail;
        private readonly Func<string, DownloadReporter, Task<object>> imageDownloadFunc;
        private Dictionary<int, string> skipHathKeys = new();
        private FullPreviewSetCollection previews;
        private TaskCompletionSource<string> showKey = new TaskCompletionSource<string>();

        public SpiderTask RequestPage(int index)
        {
            if (tasks.TryGetValue(index, out var task))
                return task;

            var backCount = client.Settings.SpiderBackPreloadCount;
            var frontCount = client.Settings.SpiderFrontPreloadCount;

            for (int i = backCount; i >= 0; i--)
                PreloadPage(index - i);
            PreloadPage(index);
            for (int i = 1; i <= frontCount; i++)
                PreloadPage(index + i);

            return tasks[index];
        }

        private string GetPageUrl(long gid, int index, string pToken,
                                  string oldPageUrl, string skipHathKey)
        {
            string pageUrl;
            if (oldPageUrl != null)
            {
                pageUrl = oldPageUrl;
            }
            else
            {
                pageUrl = client.EhUrl.GetPageUrl(gid, index, pToken);
            }
            // Add skipHathKey
            if (skipHathKey != null)
            {
                if (pageUrl.Contains("?"))
                {
                    pageUrl += "&nl=" + skipHathKey;
                }
                else
                {
                    pageUrl += "?nl=" + skipHathKey;
                }
            }
            return pageUrl;
        }

        private string PickGoodSkipHashKey(int index)
        {
            return skipHathKeys.LastOrDefault(x => x.Key <= index).Value;
        }

        private async void PreloadPage(int index)
        {
            Action<string> log = (msg) => Log.D($"EhPageImageSpider:{GetHashCode()}", $"{index}:{msg}");

            if (index < 0)
                return;
            if (index >= detail.Pages - 1)
                return;
            if (tasks.ContainsKey(index))
                return;

            var task = new SpiderTask();
            tasks[index] = task;
            log("Started");

            var prevTask = tasks.TryGetValue(index - 1, out var d) ? d : default;
            var curPreview = await previews.GetAsync(index);
            task.Preview = curPreview;

            if (prevTask is null)
            {
                var skipHashKey = PickGoodSkipHashKey(index);
                var imageUrl = GetPageUrl(detail.Gid, index, curPreview.PToken, curPreview.PageUrl, skipHashKey);
                log($"call GetGalleryPageAsync() , PToken:{curPreview.PToken} , SkipHashKey:{skipHashKey} , PageUrl:{curPreview.PageUrl}");
                var pageResult = await client.GetGalleryPageAsync(detail, curPreview);
                if (URL_509_SUFFIX_ARRAY.Any(x => pageResult.imageUrl.EndsWith(x)))
                {
                    task.DownloadTaskSouce.SetException(new EhException("509"));
                }
                else
                {
                    var curSpiderInfo = new SpiderInfo()
                    {
                        ImageUrl = pageResult.imageUrl,
                        OriginImageUrl = pageResult.originImageUrl,
                        ShowKey = pageResult.showKey,
                        SkipHathKey = pageResult.skipHathKey
                    };
                    if (!string.IsNullOrWhiteSpace(pageResult.skipHathKey))
                        skipHathKeys[curPreview.Position] = pageResult.skipHathKey;
                    task.PreviewTaskSource.SetResult(curSpiderInfo);
                    if (!showKey.Task.IsCompleted)
                        showKey.SetResult(curSpiderInfo.ShowKey);
                    var obj = await imageDownloadFunc(curSpiderInfo.ImageUrl, new DownloadReporter(task));
                    task.DownloadTaskSouce.SetResult(obj);
                }
            }
            else
            {
                var prevInfo = prevTask is null ? null : await prevTask.PreviewTask;
                var currentShowKey = prevInfo.ShowKey ?? await showKey.Task;
                log($"call GetGalleryPageApiAsync() , PToken:{curPreview.PToken} , showKey:{currentShowKey} , PreviousPToken:{prevTask.Preview.PToken}");
                var pageResult = await client.GetGalleryPageApiAsync(detail.Gid, curPreview.Position, curPreview.PToken, currentShowKey, prevTask.Preview.PToken);
                if (URL_509_SUFFIX_ARRAY.Any(x => pageResult.imageUrl.EndsWith(x)))
                {
                    task.DownloadTaskSouce.SetException(new EhException("509"));
                    return;
                }
                var curSpiderInfo = new SpiderInfo()
                {
                    ImageUrl = pageResult.imageUrl,
                    OriginImageUrl = pageResult.originImageUrl,
                    ShowKey = currentShowKey,
                    SkipHathKey = pageResult.skipHathKey
                };
                if (!string.IsNullOrWhiteSpace(pageResult.skipHathKey))
                    skipHathKeys[curPreview.Position] = pageResult.skipHathKey;
                task.PreviewTaskSource.SetResult(curSpiderInfo);
                var obj = await imageDownloadFunc(curSpiderInfo.ImageUrl, new DownloadReporter(task));
                task.DownloadTaskSouce.SetResult(obj);
            }

            log("Finished");
        }
    }
}
