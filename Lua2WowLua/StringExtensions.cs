using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lua2WowLua
{
    public static class StringExtensions
    {
        public static string Repeat(this string str, int count)
        {
            if (count == 0)
                return "";

            StringBuilder sb = new StringBuilder(str.Length * count);

            for(int i = 0; i < count; i++)
                sb.Append(str);

            return sb.ToString();
        }
    }
}
