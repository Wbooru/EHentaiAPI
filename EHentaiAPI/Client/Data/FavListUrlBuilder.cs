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
        public const int FAV_CAT_LOCAL = -2;

        private const String TAG = nameof(FavListUrlBuilder);
        private readonly EhUrl ehUrl;
        private int mIndex;
        private String mKeyword;
        private int mFavCat = FAV_CAT_ALL;

        public FavListUrlBuilder(EhUrl ehUrl)
        {
            this.ehUrl = ehUrl;
        }

        public static bool isValidFavCat(int favCat)
        {
            return favCat >= 0 && favCat <= 9;
        }

        public void setIndex(int index)
        {
            mIndex = index;
        }

        public String getKeyword()
        {
            return mKeyword;
        }

        public void setKeyword(String keyword)
        {
            mKeyword = keyword;
        }

        public int getFavCat()
        {
            return mFavCat;
        }

        public void setFavCat(int favCat)
        {
            mFavCat = favCat;
        }

        public bool isLocalFavCat()
        {
            return mFavCat == FAV_CAT_LOCAL;
        }

        public String build()
        {
            UrlBuilder ub = new UrlBuilder(ehUrl.getFavoritesUrl());
            if (isValidFavCat(mFavCat))
            {
                ub.addQuery("favcat", mFavCat.ToString());
            }
            else if (mFavCat == FAV_CAT_ALL)
            {
                ub.addQuery("favcat", "all");
            }
            if (!string.IsNullOrWhiteSpace(mKeyword))
            {
                try
                {
                    ub.addQuery("f_search", WebUtility.UrlEncode(mKeyword));
                    // Name
                    ub.addQuery("sn", "on");
                    // Tags
                    ub.addQuery("st", "on");
                    // Note
                    ub.addQuery("sf", "on");
                }
                catch (Exception)
                {
                    Log.e(TAG, $"Can't URLEncoder.encode {mKeyword} or can't add queries.");
                }
            }
            if (mIndex > 0)
            {
                ub.addQuery("page", mIndex);
            }
            return ub.build();
        }
    }
}
