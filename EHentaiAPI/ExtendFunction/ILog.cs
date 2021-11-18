using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.ExtendFunction
{
    public interface ILog
    {
        void D(string tag, string msg, Exception e);
        void I(string tag, string msg);
        void W(string tag, string msg);
        void E(string tag, string msg);
    }
}
