using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils
{
    public class UrlBuilder
    {

        public string mRootUrl;
        public Dictionary<string, Object> mQueryMap = new();

        public UrlBuilder(string rootUrl)
        {
            mRootUrl = rootUrl;
        }

        public void AddQuery(string key, Object value)
        {
            mQueryMap.Add(key, value);
        }

        public string Build()
        {
            if (mQueryMap.Count == 0)
            {
                return mRootUrl;
            }
            else
            {
                StringBuilder sb = new StringBuilder(mRootUrl);
                sb.Append("?");

                var c = 0;
                foreach (var pair in mQueryMap)
                {
                    if (c != 0)
                        sb.Append("&");
                    sb.Append(pair.Key).Append("=").Append(pair.Value);
                    c++;
                }
                return sb.ToString();
            }
        }
    }
}
