using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class VoteTagParser
    {
        // {"error":"The tag \"neko\" is not allowed. Use character:neko or artist:neko"}
        public static Result parse(String body)
        {
            var result = new Result();
            var jo = new JObject(body);
            if (jo.ContainsKey("error")) result.error = jo.Value<string>("error");
            return result;
        }

        public class Result
        {
            public string error;
        }
    }
}
