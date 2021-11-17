using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.ExtendFunction
{
    public interface ILog
    {
        void d(string tag, string msg, Exception e);
        void i(string tag, string msg);
        void w(string tag, string msg);
        void e(string tag, string msg);
    }
}
