using Newtonsoft.Json;
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
        public static Result Parse(string body)
        {
            var result = new Result();
            var jo = JsonConvert.DeserializeObject<JObject>(body);
            if (jo.ContainsKey("error")) result.error = jo.Value<string>("error");
            return result;
        }

        public class Result
        {
            public string error;
        }
    }
}
