using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGUFMerge.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string GetSubstringBeforeLastSymbol(this string source, char symbol)
        {
            int lastIndex = source.LastIndexOf(symbol);

            return (lastIndex != -1) ? source.Substring(0, lastIndex) : source;
        }
    }
}
