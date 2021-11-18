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
        private int mResponseCode;
        private string mMessage;

        public StatusCodeException(int responseCode)
        {
            mResponseCode = responseCode;
            mMessage = Enum.Parse<HttpStatusCode>(responseCode.ToString()).ToString();
        }

        public StatusCodeException(int responseCode, string message)
        {
            mResponseCode = responseCode;
            mMessage = message;
        }

        public int GetResponseCode()
        {
            return mResponseCode;
        }

        public string GetMessage()
        {
            return mMessage;
        }
    }
}
