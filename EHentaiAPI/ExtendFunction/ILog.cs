using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.ExtendFunction
{
    public interface ILog
    {
        void d(String tag, String msg, Exception e);
        void i(String tag, String msg);
        void w(String tag, String msg);
        void e(String tag, String msg);
    }
}
