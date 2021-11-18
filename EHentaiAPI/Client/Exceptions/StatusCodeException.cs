using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Exceptions
{
    public class StatusCodeException : Exception
    {
        public int ResponseCode { get; init; }

        public StatusCodeException(int responseCode) : base(Enum.Parse<HttpStatusCode>(responseCode.ToString()).ToString())
        {
            ResponseCode = responseCode;
        }

        public StatusCodeException(int responseCode, string message) : base(message)
        {
            ResponseCode = responseCode;
        }
    }
}
