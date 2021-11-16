using EHentaiAPI.Client;
using System;
using Xunit;
using Xunit.Sdk;

namespace EHentaiAPI.UnitTest
{
    public class LoginTest
    {
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

            var client = new EhClient();
            var req = new EhRequest();
            req.setArgs(TestSettings.UserName, TestSettings.Password);
            req.setMethod(EhClient.Method.METHOD_SIGN_IN);
            Assert.Equal(TestSettings.UserName, await client.execute<string>(req));
        }
    }
}
