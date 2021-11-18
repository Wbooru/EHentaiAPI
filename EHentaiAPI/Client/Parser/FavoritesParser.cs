using AngleSharp.Dom;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Utils;
using EHentaiAPI.Utils.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class FavoritesParser
    {
        public static Result Parse(Settings settings, string body)
        {
            if (body.Contains("This page requires you to log on.</p>"))
            {
                throw new EhException("Signing in is required");
            }
            string[] catArray = new string[10];
            int[] countArray = new int[10];

            try
            {
                var d = Utils.Document.Parse(body);
                var ido = d.GetElementByClass("ido");
                //noinspection ConstantConditions
                var fps = ido.GetElementsByClassName("fp").ToArray();
                // Last one is "fp fps"
                Debug.Assert(11 == fps.Length);

                for (int i = 0; i < 10; i++)
                {
                    var fp = fps[(i)];
                    countArray[i] = ParserUtils.ParseInt(fp.Children[0].Text(), 0);
                    catArray[i] = ParserUtils.Trim(fp.Children[2].Text());
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
                throw new ParseException("Parse favorites error", body);
            }

            GalleryListParser.Result result = GalleryListParser.Parse(settings, body);

            Result re = new Result();
            re.catArray = catArray;
            re.countArray = countArray;
            re.pages = result.pages;
            re.nextPage = result.nextPage;
            re.galleryInfoList = result.galleryInfoList;

            return re;
        }

        public class Result
        {
            public string[] catArray; // Size 10
            public int[] countArray; // Size 10
            public int pages;
            public int nextPage;
            public List<GalleryInfo> galleryInfoList;
        }
    }
}
