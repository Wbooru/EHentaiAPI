using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Exceptions
{
    public class NoHAtHClientException : EhException
    {
        public NoHAtHClientException(string message) : base(message)
        {
        }
    }
}
