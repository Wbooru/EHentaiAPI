using EHentaiAPI.Client.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class RateGalleryParser
    {

        public static Result parse(string body)
        {
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<JObject>(body);
                Result result = new Result();
                result.rating = jsonObject.Value<float>("rating_avg");
                result.ratingCount = jsonObject.Value<int>("rating_cnt");
                return result;
            }
            catch (Exception e)
            {
                throw new ParseException("Can't parse rate gallery", body, e);
            }
        }

        public class Result
        {
            public float rating;
            public int ratingCount;
        }
    }
}
