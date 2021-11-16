using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.Client
{
    public class EhRequestCallback
    {
        public Action<object> OnSuccess { get; set; }
        public Action<Exception> OnFailure { get; set; }
        public Action OnCancel { get; set; }
    }
}
