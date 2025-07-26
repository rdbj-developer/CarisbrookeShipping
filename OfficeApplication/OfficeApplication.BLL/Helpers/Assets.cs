using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OfficeApplication.BLL.Helpers
{
    // JSL 06/23/2022 added this file
    public static class Assets
    {
        // JSL 06/23/2022
        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var nvc = HttpUtility.ParseQueryString(queryString);
            return nvc.AllKeys.ToDictionary(k => k, k => nvc[k]);
        }
        // End JSL 06/23/2022

        // JSL 06/30/2022
        public static string CreateQueryString(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(kvp =>
               string.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value))));
        }
        // End JSL 06/30/2022
    }
}
