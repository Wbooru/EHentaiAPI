using AngleSharp.Dom;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class ProfileParser
    {
        private const string TAG = nameof(ProfileParser);

        public static Result parse(string body)
        {
            try
            {
                var result = new Result();
                var d = Utils.Document.parse(body);
                var profilename = d.getElementById("profilename");
                result.displayName = profilename.Children[(0)].Text();
                try
                {
                    result.avatar = profilename.NextElementSibling.NextElementSibling.Children[(0)].GetAttributeEx("src");
                    if (string.IsNullOrWhiteSpace(result.avatar))
                    {
                        result.avatar = null;
                    }
                    else if (!result.avatar.StartsWith("http"))
                    {
                        result.avatar = EhUrl.URL_FORUMS + result.avatar;
                    }
                }
                catch (Exception)
                {
                    //ExceptionUtils.throwIfFatal(e);
                    Log.i(TAG, "No avatar");
                }
                return result;
            }
            catch (Exception)
            {
                //ExceptionUtils.throwIfFatal(e);
                throw new ParseException("Parse forums error", body);
            }
        }

        public class Result
        {
            public string displayName;
            public string avatar;
        }
    }
}
