using EHentaiAPI.Client;
using System;
using Xunit;
using Xunit.Sdk;

namespace EHentaiAPI.UnitTest
{
    public class LoginTest : IClassFixture<ShareClient>
    {
        public ShareClient ShareClient { get; }

        public LoginTest(ShareClient shareClient)
        {
            ShareClient = shareClient;
        }

        private void CheckSettings()
        {
            //please fill your username/password into TestSettings.
            Assert.False(string.IsNullOrWhiteSpace(TestSettings.UserName));
            Assert.False(string.IsNullOrWhiteSpace(TestSettings.Password));
        }

        [Fact]
        public async void SignIn()
        {
            CheckSettings();

            var userName = await ShareClient.Client.SignIn(TestSettings.UserName, TestSettings.Password);
            Assert.Equal(TestSettings.UserName, userName);
        }
    }
}
