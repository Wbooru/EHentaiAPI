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
            client.Settings.putGallerySite(EhUrl.SITE_E);
        }

        [Fact, Order(1)]
        public async void GetGalleryList()
        {
            var req = new EhRequest();
            req.setArgs("https://e-hentai.org");
            req.setMethod(EhClient.Method.METHOD_GET_GALLERY_LIST);

            var result = await client.execute<GalleryListParser.Result>(req);

            Assert.NotEmpty(result.galleryInfoList);
        }

        [Fact, Order(2)]
        public async void GetGalleryDetail()
        {
            var info = await client.GetGalleryDetail(myLove);

            Assert.Equal("2021-11-16 15:10", info.posted);
            Assert.Equal("Yes", info.visible);
            Assert.Equal("https://e-hentai.org/g/2056718/d0e47e5e3e/", info.parent);
            Assert.Equal("[Pixiv] Miwabe Sakura (4816744)", info.title);
            Assert.Equal("[Pixiv] みわべさくら (4816744)", info.titleJpn);
            Assert.Equal("miwabe sakura", info.tags.FirstOrDefault(x=>x.groupName=="artist")?.getTagAt(0));

            Assert.NotEmpty(info.comments.comments);
            Assert.Equal("Pokom", info.comments.comments.FirstOrDefault().user);
            Assert.True(info.comments.comments.FirstOrDefault().uploader);
        }

        [Fact, Order(3)]
        public async void GetTorrentList()
        {
            var info = await client.GetGalleryDetail("https://e-hentai.org/g/2062872/fb6abc76c6/");
            Assert.NotEmpty(await client.GetTorrentList(info));
        }

        [Fact, Order(3)]
        public async void GetArchiveList()
        {
            await client.SignIn(TestSettings.UserName, TestSettings.Password);
            var info = await client.GetGalleryDetail("https://e-hentai.org/g/2062872/fb6abc76c6/");
            var archiveList = await client.GetArchiveList(info);
            Assert.False(string.IsNullOrWhiteSpace(archiveList.Key));
            Assert.NotEmpty(archiveList.Value);
        }

        [Fact, Order(3)]
        public async void GetGalleryToken()
        {
            Assert.Equal("03037d8698", await client.GetGalleryToken("https://e-hentai.org/s/35142216f7/2062874-16"));
        }
    }
}
