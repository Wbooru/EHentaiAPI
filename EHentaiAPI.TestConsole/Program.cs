using EHentaiAPI.Client;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Parser;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.UnitTest;
using EHentaiAPI.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EHentaiAPI.TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new EhClient();
            client.Cookies.Add(new System.Net.Cookie("sl", "dm_1","/", "e-hentai.org"));

            client.Settings = new Settings()
            {
                SharedPreferences = new FileSharedPreferences()
            };
            client.Settings.putGallerySite(EhUrl.SITE_E);

            var req = new EhRequest();
            req.setArgs("https://e-hentai.org");
            req.setMethod(EhClient.Method.METHOD_GET_GALLERY_LIST);

            var result = await client.execute<GalleryListParser.Result>(req);

            Console.ReadLine();
        }
    }
}
