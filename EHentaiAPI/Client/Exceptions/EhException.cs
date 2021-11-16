using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Exceptions
{
    public class EhException : Exception
    {
        public EhException(string message) : base(message) { }
        public EhException(string message, Exception cause) : base(message, cause) { }
    }
}
