using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class GalleryComment
    {
        // 0 for uploader comment. can't vote
        public long id;
        public int score;
        public bool editable;
        public bool voteUpAble;
        public bool voteUpEd;
        public bool voteDownAble;
        public bool voteDownEd;
        public bool uploader;
        public string voteState;
        public long time;
        public string user;
        public string comment;
        public long lastEdited;

        public GalleryComment()
        {
        }


        public int describeContents()
        {
            return 0;
        }
    }
}
