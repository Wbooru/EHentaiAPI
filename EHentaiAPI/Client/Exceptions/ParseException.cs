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

        public ParseException(String detailMessage, String body) : base(detailMessage)
        {
            mBody = body;
        }

        public ParseException(String detailMessage, String body, Exception cause) : base(detailMessage, cause)
        {
            mBody = body;
        }

        public string getBody()
        {
            return mBody;
        }
    }
}
