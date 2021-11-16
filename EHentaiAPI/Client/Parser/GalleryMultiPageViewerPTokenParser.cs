using EHentaiAPI.Client.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryMultiPageViewerPTokenParser
    {
        public static List<string> parse(string body)
        {
            var list = new List<string>();
            var pos = body.IndexOf("var imagelist = ") + 16;
            var ei = body.IndexOf(";", body.IndexOf("var imagelist = "));
            var imagelist = body.Substring(pos, ei - pos);
            try
            {
                var ja = new JArray(imagelist);
                for (int i = 0; i < ja.Count; i++)
                {
                    var jo = ja[(i)];
                    list.Add(jo.getString("k"));
                }
            }
            catch
            {
                //ExceptionUtils.throwIfFatal(e);
                throw new EhException(body);
            }
            return list;
        }
    }
}
