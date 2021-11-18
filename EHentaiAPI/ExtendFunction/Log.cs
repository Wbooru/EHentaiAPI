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

        internal static void D(string tag, string msg, Exception e = default) => LogImplement?.D(tag, msg, e);
        internal static void I(string tag, string msg) => LogImplement?.I(tag, msg);
        internal static void E(string tag, string msg) => LogImplement?.E(tag, msg);
        internal static void W(string tag, string msg) => LogImplement?.W(tag, msg);
    }

    internal class DefaultLogImpl : ILog
    {
        private readonly StringBuilder sb = new StringBuilder();

        private void Output(string prefix, string tag, string msg, Exception e = default)
        {
            sb.Clear();
            sb.Append('[').Append(prefix).Append(']').Append(tag).Append(':').Append(msg);
            if (e != null)
                sb.AppendLine().Append(e.StackTrace);
            Console.WriteLine(sb.ToString());
        }

        public void D(string tag, string msg, Exception e) => Output("DEBUG", tag, msg, e);

        public void E(string tag, string msg) => Output("ERROR", tag, msg);

        public void I(string tag, string msg) => Output("INFO", tag, msg);

        public void W(string tag, string msg) => Output("WARN", tag, msg);
    }
}
