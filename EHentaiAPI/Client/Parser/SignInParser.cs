using EHentaiAPI.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class SignInParser
    {

        private static readonly Regex NAME_PATTERN = new Regex("<p>You are now logged in as: (.+?)<");
        private static readonly Regex ERROR_PATTERN = new Regex(
                "(?:<h4>The error returned was:</h4>\\s*<p>(.+?)</p>)"
                        + "|(?:<span class=\"postcolor\">(.+?)</span>)");

        public static string parse(string body)
        {
            var m = NAME_PATTERN.Match(body);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                m = ERROR_PATTERN.Match(body);
                if (m.Success)
                {
                    throw new EhException(m.Groups[1].Value == null ? m.Groups[2].Value : m.Groups[1].Value);
                }
                else
                {
                    throw new ParseException("Can't parse sign in", body);
                }
            }
        }
    }
}
