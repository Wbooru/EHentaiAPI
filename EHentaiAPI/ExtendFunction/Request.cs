using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.ExtendFunction
{
    public static class Request
    {
        public static Func<string, IRequest> RequestFactory { get; set; } = (url) => new DefaultRequestImpl(url);

        internal static IRequest Create(string url)
        {
            return RequestFactory?.Invoke(url);
        }
    }

    internal class DefaultRequestImpl : IRequest
    {
        private HttpWebRequest request;

        public WebHeaderCollection Headers
        {
            get
            {
                return request.Headers;
            }
            set
            {
                request.Headers = value;
            }
        }

        public CookieContainer Cookies
        {
            get
            {
                return request.CookieContainer;
            }

            set
            {
                request.CookieContainer = value;
            }
        }

        public string Method
        {
            get
            {

                return request.Method;
            }
            set
            {
                request.Method = value?.ToUpper();
            }
        }

        public HttpContent Content { get; set; }

        public DefaultRequestImpl(string url)
        {
            request = WebRequest.Create(url) as HttpWebRequest;
        }

        public async Task<IResponse> SendAsync()
        {
            if (Content != null)
            {
                await Content.CopyToAsync(await request.GetRequestStreamAsync());
                request.ContentType = Content.Headers.ContentType.ToString();
                request.ContentLength = Content.Headers.ContentLength ?? 0;
            }

            var response = await request.GetResponseAsync();
            return new DefaultResponseImpl(response as HttpWebResponse);
        }
    }


    public class DefaultResponseImpl : IResponse
    {
        private HttpWebResponse response;

        public DefaultResponseImpl(HttpWebResponse response)
        {
            this.response = response;
        }

        public int StatusCode => (int)response.StatusCode;

        public WebHeaderCollection Headers => response.Headers;

        public async Task<string> GetResponseContentAsStringAsync()
        {
            using var reader = new StreamReader(response.GetResponseStream());
            var content = await reader.ReadToEndAsync();
            return content;
        }
    }
}
