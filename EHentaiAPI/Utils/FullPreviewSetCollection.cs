using EHentaiAPI.Client;
using EHentaiAPI.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils
{
    public class FullPreviewSetCollection : IAsyncEnumerable<GalleryPreview>
    {
        private readonly EhClient client;
        private readonly GalleryDetail detail;
        private Dictionary<int, GalleryPreview> cachedPreviews = new();

        public FullPreviewSetCollection(EhClient client, GalleryDetail detail)
        {
            this.client = client;
            this.detail = detail;
        }


        public async ValueTask<GalleryPreview> GetAsync(int index, CancellationToken cancellationToken = default)
        {
            var d = GetAsyncEnumerator(cancellationToken);
            for (int i = 0; i <= index; i++)
                if (!await d.MoveNextAsync())
                    return default;
            return d.Current;
        }

        public async IAsyncEnumerator<GalleryPreview> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var i = 0;
            var page = 0;

            while (i < detail.Pages)
            {
                if (cachedPreviews.TryGetValue(i, out var d))
                {
                    yield return d;
                    i++;
                }
                else
                {
                    var item = await client.GetPreviewSetAsync(detail, page);
                    if (item.Value == 0 || cancellationToken.IsCancellationRequested)
                    {
                        yield break;
                    }
                    else
                    {
                        page++;

                        for (int w = 0; w < item.Key.Size; w++)
                        {
                            var preview = item.Key.GetGalleryPreview(detail.Gid, w);
                            cachedPreviews[preview.Position] = preview;
                        }

                        if (cachedPreviews.TryGetValue(i, out d))
                        {
                            yield return d;
                            i++;
                        }
                    }
                }
            }
        }
    }
}
