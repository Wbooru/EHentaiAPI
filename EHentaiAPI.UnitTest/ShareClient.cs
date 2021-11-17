using EHentaiAPI.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.UnitTest
{
    public class ShareClient : IDisposable
    {
        public EhClient Client { get; set; } = new EhClient();

        public void Dispose()
        {

        }
    }
}
