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
        public EhTask task;
        private EhRequestCallback mCallback;
        private EhConfig mEhConfig;
        private object[] mArgs;
        private Method mMethod;
        private bool mCancel = false;

        public Method getMethod() => mMethod;

        public EhRequest setMethod(Method method)
        {
            mMethod = method;
            return this;
        }

        public object[] getArgs()
        {
            return mArgs;
        }

        public EhRequest setArgs(params object[] args)
        {
            mArgs = args;
            return this;
        }

        public EhRequestCallback getCallback()
        {
            return mCallback;
        }

        public EhRequest setCallback(EhRequestCallback callback)
        {
            mCallback = callback;
            return this;
        }

        public EhConfig getEhConfig()
        {
            return mEhConfig;
        }

        public EhRequest setEhConfig(EhConfig ehConfig)
        {
            mEhConfig = ehConfig;
            return this;
        }

        public void cancel()
        {
            if (!mCancel)
            {
                mCancel = true;
                if (task != null)
                {
                    task.stop();
                    task = null;
                }
            }
        }

        public bool isCancelled()
        {
            return mCancel;
        }
    }
}
