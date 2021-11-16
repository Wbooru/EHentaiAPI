using EHentaiAPI.ExtendFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI
{
    internal static class ExceptionExtendMethod
    {
        public static void printStackTrace(this Exception e)
        {
            Log.e("PrintStackTrace", e.StackTrace);
        }
    }
}
