using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils.ExtensionMethods
{
    internal static class JsonExtensionMethod
    {
        public static string getString(this JToken o, string key) => o[key].ToString();
        public static JArray getJSONArray(this JToken o, string key) => o[key] as JArray;
    }
}
