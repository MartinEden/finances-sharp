using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FinancesSharp.Helpers
{
    public static class JavascriptHelpers
    {
        public static string AsJavascriptList<T>(this IEnumerable<T> list)
        {
            return JsonConvert.SerializeObject(list);
        }
    }
}