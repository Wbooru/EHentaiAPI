using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EHentaiAPI.Client.EhClient;

namespace EHentaiAPI.Client
{
    public class EhRequest
    {
        private EhTask task;
        public EhRequestCallback Callback { get; set; }
        public EhConfig EhConfig { get; set; }
        public object[] Args { get; set; }
        private bool mCancel = false;

        public Method Method { get; set; }

        public EhRequest SetMethod(Method method)
        {
            Method = method;
            return this;
        }

        public EhRequest SetArgs(params object[] args)
        {
            Args = args;
            return this;
        }

        public EhRequest SetCallback(EhRequestCallback callback)
        {
            Callback = callback;
            return this;
        }

        public EhRequest SetEhConfig(EhConfig ehConfig)
        {
            EhConfig = ehConfig;
            return this;
        }

        public EhRequest SetTask(EhTask task)
        {
            this.task = task;
            return this;
        }

        public void Cancel()
        {
            if (!mCancel)
            {
                mCancel = true;
                if (task != null)
                {
                    task.Stop();
                    task = null;
                }
            }
        }

        public bool IsCancelled => mCancel;
    }
}
