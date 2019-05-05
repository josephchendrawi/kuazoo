using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.kuazoo.helper
{
    public abstract class ReplaceSymbol
    {
        public static string Replace(string name)
        {
            return name.Replace(" ", "-").Replace("!", "").Replace("+", "").Replace(":", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("*", "").Replace("&", "").Replace("@", "").Replace("=", "").Replace("%", "").Replace("/", "");

        }
    }
}
