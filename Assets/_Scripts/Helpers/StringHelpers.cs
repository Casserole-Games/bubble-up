using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assets._Scripts.Helpers
{
    public static class StringHelpers
    {
        public static string RemoveUGSSuffix(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return Regex.Replace(str, @"#\d{4}$", "");
        }
    }
}
