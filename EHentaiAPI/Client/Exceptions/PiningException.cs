using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Exceptions
{
    public class PiningException : EhException
    {
        public PiningException() : base("pining for the fjords")
        {

        }
    }
}
