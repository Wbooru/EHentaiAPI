using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHentaiAPI.ExtendFunction
{
    public static class Log
    {
        public static ILog LogImplement { get; set; } = new DefaultLogImpl();

        internal static void d(string tag, string msg, Exception e = default) => LogImplement?.d(tag, msg, e);
        internal static void i(string tag, string msg) => LogImplement?.i(tag, msg);
        internal static void e(string tag, string msg) => LogImplement?.e(tag, msg);
        internal static void w(string tag, string msg) => LogImplement?.w(tag, msg);
    }

    internal class DefaultLogImpl : ILog
    {
        private StringBuilder sb = new StringBuilder();

        private void Output(string prefix, string tag, string msg, Exception e = default)
        {
            sb.Clear();
            sb.Append("[").Append("prefix").Append("]").Append(tag).Append(":").Append(msg);
            if (e != null)
                sb.AppendLine().Append(e.StackTrace);
            Console.WriteLine(sb.ToString());
        }

        public void d(string tag, string msg, Exception e) => Output("DEBUG", tag, msg, e);

        public void e(string tag, string msg) => Output("ERROR", tag, msg);

        public void i(string tag, string msg) => Output("INFO", tag, msg);

        public void w(string tag, string msg) => Output("WARN", tag, msg);
    }
}
