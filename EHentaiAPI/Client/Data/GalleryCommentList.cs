using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryCommentList
    {
        public GalleryComment[] Comments { get; set; }
        public bool HasMore { get; set; }

        public GalleryCommentList(GalleryComment[] comments, bool hasMore)
        {
            this.Comments = comments;
            this.HasMore = hasMore;
        }
    }
}
