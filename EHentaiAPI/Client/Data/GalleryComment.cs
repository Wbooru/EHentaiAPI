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
        public long Id { get; set; }
        public int Score { get; set; }
        public bool Editable { get; set; }
        public bool VoteUpAble { get; set; }
        public bool VoteUpEd { get; set; }
        public bool VoteDownAble { get; set; }
        public bool VoteDownEd { get; set; }
        public bool Uploader { get; set; }
        public string VoteState { get; set; }
        public long Time { get; set; }
        public string User { get; set; }
        public string Comment { get; set; }
        public long LastEdited { get; set; }

        public override string ToString() => $"[{Score}]{User}:{Comment}";
    }
}
