using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.ExtendFunction
{
    public interface IRequest
    {
        public WebHeaderCollection Headers { get; set; }
        public CookieContainer Cookies { get; set; }
        public string Method { get; set; }
        public HttpContent Content { get; set; }

        public Task<IResponse> SendAsync();
    }
}
