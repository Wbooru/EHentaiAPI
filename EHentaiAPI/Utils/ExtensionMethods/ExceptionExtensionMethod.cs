using EHentaiAPI.ExtendFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Utils.ExtensionMethods
{
    internal static class ExceptionExtensionMethod
    {
        public static void printStackTrace(this Exception e)
        {
            Log.e("PrintStackTrace", e.StackTrace);
        }
    }
}
