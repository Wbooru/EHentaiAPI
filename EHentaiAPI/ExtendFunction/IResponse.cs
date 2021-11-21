using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.ExtendFunction
{
    public interface IResponse
    {
        public int StatusCode { get; }
        public WebHeaderCollection Headers { get; }

        public Task<string> GetResponseContentAsStringAsync();
    }
}
