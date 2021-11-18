using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Exceptions
{
    public class ParseException : EhException
    {
        public string Body { get; init; }

        public ParseException(string detailMessage, string body) : base(detailMessage)
        {
            this.Body = body;
        }

        public ParseException(string detailMessage, string body, Exception cause) : base(detailMessage, cause)
        {
            this.Body = body;
        }
    }
}
