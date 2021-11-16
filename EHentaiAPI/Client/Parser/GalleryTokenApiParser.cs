﻿using EHentaiAPI.Client.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryTokenApiParser
    {

        /**
         * {
         * "tokenlist": [
         * {
         * "gid":618395,
         * "token":"0439fa3666"
         * }
         * ]
         * }
         */
        public static String parse(String body)
        {
            var jo = new JObject(body).getJSONArray("tokenlist")[(0)];
            try
            {
                return jo.getString("token");
            }
            catch (Exception)
            {
                //ExceptionUtils.throwIfFatal(e);
                throw new EhException(jo.getString("error"));
            }
        }
    }
}
