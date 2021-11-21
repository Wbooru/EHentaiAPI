using EHentaiAPI.Client;
using System;
using Xunit;
using Xunit.Extensions.Ordering;
using Xunit.Sdk;

namespace EHentaiAPI.UnitTest
{
    public class LoginTest : IClassFixture<ShareClient>
    {
        private EhClient client;

        public LoginTest(ShareClient shareClient)
        {
            client = shareClient.Client;
        }

        private void CheckSettings()
        {
            //please fill your username/password into TestSettings.
            Assert.False(string.IsNullOrWhiteSpace(TestSettings.UserName));
            Assert.False(string.IsNullOrWhiteSpace(TestSettings.Password));
        }

        [Fact, Order(1)]
        public async void SignIn()
        {
            CheckSettings();

            var userName = await client.SignInAsync(TestSettings.UserName, TestSettings.Password);
            Assert.Equal(TestSettings.UserName, userName);
        }

        [Fact, Order(2)]
        public async void GetProfile()
        {
            var userName = await client.SignInAsync(TestSettings.UserName, TestSettings.Password);
            var profile = await client.GetProfileAsync();
            Assert.Equal(userName, profile.displayName);
        }
    }
}
