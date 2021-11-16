﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI
{
    internal static class JsonExtendMethod
    {
        public static string getString(this JToken o, string key) => o[key].ToString();
        public static JArray getJSONArray(this JToken o, string key) => o[key] as JArray;
    }
}
