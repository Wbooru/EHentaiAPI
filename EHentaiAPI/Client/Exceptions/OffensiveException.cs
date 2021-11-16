using EHentaiAPI.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Exceptions
{
    public class OffensiveException : EhException
    {
        public OffensiveException() : base("OFFENSIVE")
        {

        }
    }
}
