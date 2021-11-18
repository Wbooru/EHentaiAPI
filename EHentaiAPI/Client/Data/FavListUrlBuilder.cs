using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Data
{
    public class FavListUrlBuilder
    {
        public const int FAV_CAT_ALL = -1;

        private const string TAG = nameof(FavListUrlBuilder);
        private readonly EhUrl ehUrl;
        public int Index { get; set; }

        public string Keyword { get; set; }
        public int FavCat { get; set; } = FAV_CAT_ALL;

        public FavListUrlBuilder(EhUrl ehUrl)
        {
            this.ehUrl = ehUrl;
        }

        public static bool IsValidFavCat(int favCat)
        {
            return favCat >= 0 && favCat <= 9;
        }

        public string Build()
        {
            var ub = new UrlBuilder(ehUrl.GetFavoritesUrl());
            if (IsValidFavCat(FavCat))
            {
                ub.AddQuery("favcat", FavCat.ToString());
            }
            else if (FavCat == FAV_CAT_ALL)
            {
                ub.AddQuery("favcat", "all");
            }
            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                try
                {
                    ub.AddQuery("f_search", WebUtility.UrlEncode(Keyword));
                    // Name
                    ub.AddQuery("sn", "on");
                    // Tags
                    ub.AddQuery("st", "on");
                    // Note
                    ub.AddQuery("sf", "on");
                }
                catch (Exception)
                {
                    Log.E(TAG, $"Can't URLEncoder.encode {Keyword} or can't add queries.");
                }
            }
            if (Index > 0)
            {
                ub.AddQuery("page", Index);
            }
            return ub.Build();
        }
    }
}
