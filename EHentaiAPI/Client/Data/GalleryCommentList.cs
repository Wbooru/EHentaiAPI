using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryCommentList
    {
        public GalleryComment[] comments;
        public bool hasMore;

        public GalleryCommentList(GalleryComment[] comments, bool hasMore)
        {
            this.comments = comments;
            this.hasMore = hasMore;
        }
    }
}
