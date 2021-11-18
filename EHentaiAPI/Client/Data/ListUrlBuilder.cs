using EHentaiAPI.Utils;
using System;
using System.Net;
using System.Text;

namespace EHentaiAPI.Client.Data
{
    public class ListUrlBuilder
    {
        public static class AdvanceSearchTable
        {
            public const int SNAME = 0x1;
            public const int STAGS = 0x2;
            public const int SDESC = 0x4;
            public const int STORR = 0x8;
            public const int STO = 0x10;
            public const int SDT1 = 0x20;
            public const int SDT2 = 0x40;
            public const int SH = 0x80;
            public const int SFL = 0x100;
            public const int SFU = 0x200;
            public const int SFT = 0x400;
        }

        public enum ListMode
        {
            Normal = 0x0,
            Uploader = 0x1,
            Tag = 0x2,
            WhatsHot = 0x3,
            ImageSearch = 0x4,
            SubScription = 0x5,
            TopList = 0x6
        }

        private readonly EhUrl ehUrl;

        public ListMode Mode { get; set; } = ListMode.Normal;
        public int PageIndex { get; set; } = 0;
        public int Category { get; set; } = EhUtils.NONE;

        private string keyword;
        public string Keyword
        {
            get
            {
                return ListMode.Uploader == Mode ? "uploader:" + keyword : keyword;
            }
            set
            {
                keyword = value;
            }
        }

        public string SHash { get; set; } = null;

        public int AdvanceSearch { get; set; } = -1;
        public int MinRating { get; set; } = -1;
        public int PageFrom { get; set; } = -1;
        public int PageTo { get; set; } = -1;

        public string ImagePath { get; set; }
        public bool UseSimilarityScan { get; set; }
        public bool OnlySearchCovers { get; set; }
        public bool ShowExpunged { get; set; }

        public ListUrlBuilder(EhUrl ehUrl)
        {
            this.ehUrl = ehUrl;
        }

        /**
         * Make this ListUrlBuilder point to homepage
         */
        public void Reset()
        {
            Mode = ListMode.Normal;
            PageIndex = 0;
            Category = EhUtils.NONE;
            Keyword = null;
            AdvanceSearch = -1;
            MinRating = -1;
            PageFrom = -1;
            PageTo = -1;
            ImagePath = null;
            UseSimilarityScan = false;
            OnlySearchCovers = false;
            ShowExpunged = false;
            SHash = null;
        }

        /**
         * @param query xxx=yyy&mmm=nnn
         */
        // TODO page
        public void SetQuery(string query)
        {
            Reset();

            if (string.IsNullOrWhiteSpace(query))
            {
                return;
            }
            var querys = query.Split('&');
            int category = 0;
            string keyword = null;
            bool enableAdvanceSearch = false;
            int advanceSearch = 0;
            bool enableMinRating = false;
            int minRating = -1;
            bool enablePage = false;
            int pageFrom = -1;
            int pageTo = -1;
            foreach (string str in querys)
            {
                int index = str.IndexOf('=');
                if (index < 0)
                    continue;
                var key = str.Substring(0, index - 0);
                var value = str.Substring(index + 1);
                switch (key)
                {
                    case "f_cats":
                        var cats = int.TryParse(value, out var d) ? d : EhConfig.ALL_CATEGORY;
                        category |= (~cats) & EhConfig.ALL_CATEGORY;
                        break;
                    case "f_doujinshi":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.DOUJINSHI;
                        }
                        break;
                    case "f_manga":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.MANGA;
                        }
                        break;
                    case "f_artistcg":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.ARTIST_CG;
                        }
                        break;
                    case "f_gamecg":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.GAME_CG;
                        }
                        break;
                    case "f_western":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.WESTERN;
                        }
                        break;
                    case "f_non-h":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.NON_H;
                        }
                        break;
                    case "f_imageset":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.IMAGE_SET;
                        }
                        break;
                    case "f_cosplay":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.COSPLAY;
                        }
                        break;
                    case "f_asianporn":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.ASIAN_PORN;
                        }
                        break;
                    case "f_misc":
                        if ("1".Equals(value))
                        {
                            category |= EhConfig.MISC;
                        }
                        break;
                    case "f_search":
                        keyword = WebUtility.UrlDecode(value);

                        break;
                    case "advsearch":
                        if ("1".Equals(value))
                        {
                            enableAdvanceSearch = true;
                        }
                        break;
                    case "f_sname":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SNAME;
                        }
                        break;
                    case "f_stags":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.STAGS;
                        }
                        break;
                    case "f_sdesc":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SDESC;
                        }
                        break;
                    case "f_storr":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.STORR;
                        }
                        break;
                    case "f_sto":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.STO;
                        }
                        break;
                    case "f_sdt1":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SDT1;
                        }
                        break;
                    case "f_sdt2":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SDT2;
                        }
                        break;
                    case "f_sh":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SH;
                        }
                        break;
                    case "f_sfl":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SFL;
                        }
                        break;
                    case "f_sfu":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SFU;
                        }
                        break;
                    case "f_sft":
                        if ("on".Equals(value))
                        {
                            advanceSearch |= AdvanceSearchTable.SFT;
                        }
                        break;
                    case "f_sr":
                        if ("on".Equals(value))
                        {
                            enableMinRating = true;
                        }
                        break;
                    case "f_srdd":
                        minRating = int.TryParse(value, out d) ? d : -1;
                        break;
                    case "f_sp":
                        if ("on".Equals(value))
                        {
                            enablePage = true;
                        }
                        break;
                    case "f_spf":
                        pageFrom = int.TryParse(value, out d) ? d : -1;
                        break;
                    case "f_spt":
                        pageTo = int.TryParse(value, out d) ? d : -1;
                        break;
                    case "f_shash":
                        SHash = value;
                        break;
                }
            }

            Category = category;
            Keyword = keyword;
            if (enableAdvanceSearch)
            {
                AdvanceSearch = advanceSearch;
                if (enableMinRating)
                {
                    MinRating = minRating;
                }
                else
                {
                    MinRating = -1;
                }
                if (enablePage)
                {
                    PageFrom = pageFrom;
                    PageTo = pageTo;
                }
                else
                {
                    PageFrom = -1;
                    PageTo = -1;
                }
            }
            else
            {
                AdvanceSearch = -1;
            }
        }

        public string Build()
        {
            StringBuilder sb;

            switch (Mode)
            {
                default:
                case ListMode.Normal:
                case ListMode.SubScription:
                    {
                        string url;
                        if (Mode == ListMode.Normal)
                        {
                            url = ehUrl.GetHost();
                        }
                        else
                        {
                            url = ehUrl.GetWatchedUrl();
                        }

                        var ub = new UrlBuilder(url);
                        if (Category != EhUtils.NONE)
                        {
                            ub.AddQuery("f_cats", (~Category) & EhConfig.ALL_CATEGORY);
                        }
                        // Search key
                        if (Keyword != null)
                        {
                            string keyword = Keyword.Trim();
                            if (!string.IsNullOrWhiteSpace(keyword))
                            {
                                try
                                {
                                    ub.AddQuery("f_search", WebUtility.UrlEncode(Keyword));
                                }
                                catch
                                {
                                    // Empty
                                }
                            }
                        }
                        if (SHash != null)
                        {
                            ub.AddQuery("f_shash", SHash);
                        }
                        // Page index
                        if (PageIndex != 0)
                        {
                            ub.AddQuery("page", PageIndex);
                        }
                        // Advance search
                        if (AdvanceSearch != -1)
                        {
                            ub.AddQuery("advsearch", "1");
                            if ((AdvanceSearch & AdvanceSearchTable.SNAME) != 0)
                                ub.AddQuery("f_sname", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.STAGS) != 0)
                                ub.AddQuery("f_stags", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.SDESC) != 0)
                                ub.AddQuery("f_sdesc", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.STORR) != 0)
                                ub.AddQuery("f_storr", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.STO) != 0) ub.AddQuery("f_sto", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.SDT1) != 0)
                                ub.AddQuery("f_sdt1", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.SDT2) != 0)
                                ub.AddQuery("f_sdt2", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.SH) != 0) ub.AddQuery("f_sh", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.SFL) != 0) ub.AddQuery("f_sfl", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.SFU) != 0) ub.AddQuery("f_sfu", "on");
                            if ((AdvanceSearch & AdvanceSearchTable.SFT) != 0) ub.AddQuery("f_sft", "on");
                            // Set min star
                            if (MinRating != -1)
                            {
                                ub.AddQuery("f_sr", "on");
                                ub.AddQuery("f_srdd", MinRating);
                            }
                            // Pages
                            if (PageFrom != -1 || PageTo != -1)
                            {
                                ub.AddQuery("f_sp", "on");
                                ub.AddQuery("f_spf", PageFrom != -1 ? PageFrom : "");
                                ub.AddQuery("f_spt", PageTo != -1 ? PageTo : "");
                            }
                        }
                        return ub.Build();
                    }
                case ListMode.Uploader:
                    {
                        sb = new StringBuilder(ehUrl.GetHost());
                        sb.Append("uploader/");
                        try
                        {
                            sb.Append(WebUtility.UrlEncode(Keyword));
                        }
                        catch
                        {
                            // Empty
                        }
                        if (PageIndex != 0)
                        {
                            sb.Append('/').Append(PageIndex);
                        }
                        return sb.ToString();
                    }
                case ListMode.Tag:
                    {
                        sb = new StringBuilder(ehUrl.GetHost());
                        sb.Append("tag/");
                        try
                        {
                            sb.Append(WebUtility.UrlEncode(Keyword));
                        }
                        catch
                        {
                            // Empty
                        }
                        if (PageIndex != 0)
                        {
                            sb.Append('/').Append(PageIndex);
                        }
                        return sb.ToString();
                    }
                case ListMode.WhatsHot:
                    return ehUrl.GetPopularUrl();
                case ListMode.ImageSearch:
                    return ehUrl.GetImageSearchUrl();
                case ListMode.TopList:
                    sb = new StringBuilder(EhUrl.HOST_E);
                    sb.Append("toplist.php?tl=");
                    try
                    {
                        sb.Append(WebUtility.UrlEncode(Keyword));
                    }
                    catch
                    {
                        // Empty
                    }
                    if (PageIndex != 0)
                    {
                        sb.Append("&p=").Append(PageIndex);
                    }
                    return sb.ToString();
            }
        }
    }
}
