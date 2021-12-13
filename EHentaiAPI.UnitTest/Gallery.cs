using EHentaiAPI.Client;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions.Ordering;

[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
namespace EHentaiAPI.UnitTest
{
    public class Gallery : IClassFixture<ShareClient>
    {
        private readonly EhClient client;
        private const string myLove = "https://e-hentai.org/g/2062067/588c82702b/";

        public Gallery(ShareClient shareClient)
        {
            client = shareClient.Client;

            client.Cookies.Add(new System.Net.Cookie("sl", "dm_1", "/", "e-hentai.org"));
            client.Settings.GallerySite = Settings.GallerySites.SITE_E;
        }

        [Fact, Order(1)]
        public async void GetGalleryList()
        {
            var req = new EhRequest();
            req.SetArgs("https://e-hentai.org");
            req.SetMethod(EhClient.Method.METHOD_GET_GALLERY_LIST);

            var result = await client.Execute<GalleryListParser.Result>(req);

            Assert.NotEmpty(result.galleryInfoList);
        }

        [Fact, Order(2)]
        public async void GetGalleryDetail()
        {
            var info = await client.GetGalleryDetailAsync(myLove);

            Assert.Equal("2021-11-16 15:10", info.Posted);
            Assert.Equal("Yes", info.Visible);
            Assert.Equal("https://e-hentai.org/g/2056718/d0e47e5e3e/", info.Parent);
            Assert.Equal("[Pixiv] Miwabe Sakura (4816744)", info.Title);
            Assert.Equal("[Pixiv] みわべさくら (4816744)", info.TitleJpn);
            Assert.Equal("miwabe sakura", info.Tags.FirstOrDefault(x => x.TagGroupName == "artist")?.GetTagAt(0));

            Assert.NotEmpty(info.Comments.Comments);
            Assert.Equal("Pokom", info.Comments.Comments.FirstOrDefault()?.User);
            Assert.True(info.Comments.Comments.FirstOrDefault()?.Uploader);
            Assert.True(info.Comments.Comments.FirstOrDefault()?.Comment.Length > 0);
        }

        [Fact, Order(3)]
        public async void GetTorrentList()
        {
            var info = await client.GetGalleryDetailAsync("https://e-hentai.org/g/2062872/fb6abc76c6/");
            Assert.NotEmpty(await client.GetTorrentListAsync(info));
        }

        [Fact, Order(3)]
        public async void GetArchiveList()
        {
            await client.SignInAsync(TestSettings.UserName, TestSettings.Password);
            var info = await client.GetGalleryDetailAsync("https://e-hentai.org/g/2062872/fb6abc76c6/");
            var archiveList = await client.GetArchiveListAsync(info);
            Assert.False(string.IsNullOrWhiteSpace(archiveList.Key));
            Assert.NotEmpty(archiveList.Value);
        }

        [Fact, Order(3)]
        public async void GetGalleryToken()
        {
            Assert.Equal("03037d8698", await client.GetGalleryTokenAsync("https://e-hentai.org/s/35142216f7/2062874-16"));
        }

        [Fact, Order(3)]
        public async void GetGalleryPreviewSet()
        {
            var info = await client.GetGalleryDetailAsync("https://e-hentai.org/g/2062872/fb6abc76c6/");
            Assert.True((await client.GetPreviewSetAsync(info, 0)).Key.Size > 0);
        }
    }
}
