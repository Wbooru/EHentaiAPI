using EHentaiAPI.Client;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EHentaiAPI.UnitTest
{
    public class Gallery
    {
        [Fact]
        public async void GetGalleryList()
        {
            var client = new EhClient();
            client.Cookies.Add(new System.Net.Cookie("sl", "dm_1", "/", "e-hentai.org"));
            client.Settings.putGallerySite(EhUrl.SITE_E);

            var req = new EhRequest();
            req.setArgs("https://e-hentai.org");
            req.setMethod(EhClient.Method.METHOD_GET_GALLERY_LIST);

            var result = await client.execute<GalleryListParser.Result>(req);

            Assert.NotEmpty(result.galleryInfoList);
        }

        [Fact]
        public async void GetGalleryDetail()
        {
            const string myLove = "https://e-hentai.org/g/2062067/588c82702b/";

            var client = new EhClient();
            client.Cookies.Add(new System.Net.Cookie("sl", "dm_1", "/", "e-hentai.org"));
            client.Settings.putGallerySite(EhUrl.SITE_E);

            var req = new EhRequest();
            req.setArgs(myLove);
            req.setMethod(EhClient.Method.METHOD_GET_GALLERY_DETAIL);

            var info = await client.execute<GalleryDetail>(req);

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
    }
}
