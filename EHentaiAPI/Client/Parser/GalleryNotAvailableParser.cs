using AngleSharp.Dom;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryNotAvailableParser
    {
        public static String parse(String body)
        {
            String error = null;
            try
            {
                var document = Utils.Document.parse(body);
                var d = document.getElementByClass("d");
                //noinspection ConstantConditions
                error = d.Children[(0)].Html();
                error = error.Replace("<br>", "\n");
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.printStackTrace();
            }
            return error;
        }
    }
}
