using System;
using System.Collections.Generic;
using System.Text;

namespace Taron
{
    public static class StringUtils
    {
        private static IDictionary<char, string> escapeMap = new Dictionary<char, string>()
        {
            { '\n', @"\n" },
            { '\r', @"\r" },
            { '\t', @"\t" },
            //{ ' ', @"\s" },
            { '\0', @"\0" }
        };

        public static string Escape(char c)
        {
            string result;
            if (!escapeMap.TryGetValue(c, out result))
                return c.ToString();
            else
                return result;
        }

        public static string Escape(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
                sb.Append(Escape(s[i]));
            return sb.ToString();
        }

    }
}
