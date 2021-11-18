using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Exceptions
{
    public class ParseException : EhException
    {
        private string mBody;

        public ParseException(string detailMessage, string body) : base(detailMessage)
        {
            mBody = body;
        }

        public ParseException(string detailMessage, string body, Exception cause) : base(detailMessage, cause)
        {
            mBody = body;
        }

        public string GetBody()
        {
            return mBody;
        }
    }
}
