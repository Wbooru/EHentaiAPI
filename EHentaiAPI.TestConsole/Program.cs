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
using System.Linq;
using System.Threading.Tasks;

namespace EHentaiAPI.TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new EhClient();

            client.Settings = new Settings()
            {
                SharedPreferences = new FileSharedPreferences()
            };
            client.Settings.PutGallerySite(EhUrl.SITE_E);
            client.Cookies.Add(new System.Net.Cookie("sl", "dm_1", "/", "e-hentai.org"));

            await client.SignInAsync(TestSettings.UserName, TestSettings.Password);

            var detail = await client.GetGalleryDetailAsync("https://e-hentai.org/g/2043816/abfa078bb5/");

            /*
            var voteResult = await client.VoteComment(detail.apiUid, detail.apiKey, detail.gid, detail.token, detail.comments.comments[1].id, 1);

            var torrentList = await client.GetTorrentList(detail.torrentUrl, detail.gid, detail.token);
            
            var archiveList = await client.GetArchiveList(detail.archiveUrl, detail.gid, detail.token);

            var voteTagResult = await client.VoteTag(detail.apiUid, detail.apiKey, detail.gid, detail.token, detail.tags.First().getTagAt(0), 1);

            var rateResult = await client.RateGallery(detail, 3);
            
            //var newCommentList = await client.CommentNewGallery("https://e-hentai.org/g/2062874/03037d8698/","谢谢好兄弟:D");
           
            //var newCommentList = await client.ModifyCommentGallery("https://e-hentai.org/g/2062874/03037d8698/", "谢谢好兄弟:D 233", detail.comments.comments.FirstOrDefault(x => x.user.Equals(TestSettings.UserName, StringComparison.InvariantCultureIgnoreCase)).id.ToString());
            
            var galleryToken = await client.GetGalleryToken("https://e-hentai.org/s/35142216f7/2062874-16");
            
            var favUrlBuilder = new FavListUrlBuilder(client.EhUrl);
            favUrlBuilder.setFavCat(FavListUrlBuilder.FAV_CAT_ALL);
            favUrlBuilder.setIndex(1);
            var favReqUrl = favUrlBuilder.build();
            var getFavorites = await client.GetFavorites(favReqUrl);

            //await client.AddFavorites(detail.gid,detail.token,1,"Haha");

            favUrlBuilder = new FavListUrlBuilder(client.EhUrl);
            favUrlBuilder.setFavCat(5);
            getFavorites = await client.GetFavorites(favUrlBuilder);
            await client.ModifyFavorites(favUrlBuilder, getFavorites.galleryInfoList.Select(x => x.gid).ToArray(), 3);
            
            var profile = await client.GetProfile();

            var previewSet = await client.GetPreviewSet(detail, 0);
            */

            Console.ReadLine();
        }
    }
}
