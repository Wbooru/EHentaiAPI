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

            var request = new EhRequest()
                .setMethod(EhClient.Method.METHOD_SIGN_IN)
                .setArgs("DarkProjector", TestSettings.Password);
            var userName = await client.execute<string>(request);

            request = new EhRequest()
                .setMethod(EhClient.Method.METHOD_GET_GALLERY_DETAIL)
                .setArgs("https://e-hentai.org/g/2062095/080e7efd64/");

            var galleryList = await client.execute<GalleryDetail>(request);
            
            Console.ReadLine();
        }
    }
}
