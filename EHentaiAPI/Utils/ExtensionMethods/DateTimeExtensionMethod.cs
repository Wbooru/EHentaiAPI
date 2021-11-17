using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils.ExtensionMethods
{
    internal static class DateTimeExtensionMethod
    {
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return (dateTime.Ticks - 621355968000000000) / 10000000;
        }
    }
}
