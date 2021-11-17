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
            client.Settings.putGallerySite(EhUrl.SITE_E);

            await client.SignIn(TestSettings.UserName, TestSettings.Password);

            var detail = await client.GetGalleryDetail("https://e-hentai.org/g/2062507/f4c499a82a/");

            var voteResult = await client.VoteComment(detail.apiUid, detail.apiKey, detail.gid, detail.token, detail.comments.comments[1].id, 1);

            var torrentList = await client.GetTorrentList(detail.torrentUrl, detail.gid, detail.token);
            var archiveList = await client.GetArchiveList(detail.archiveUrl, detail.gid, detail.token);

            var voteTagResult = await client.VoteTag(detail.apiUid, detail.apiKey, detail.gid, detail.token, detail.tags.First().getTagAt(0), 1);

            Console.ReadLine();
        }
    }
}
