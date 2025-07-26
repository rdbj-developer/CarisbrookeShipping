using System;
using System.Text.RegularExpressions;

namespace AWS_PO_UpdateService.Helper
{
    public static class Utility
    {
        public static Boolean isValidPONo(this string strToCheck)
        {
            string patern = @"^[a-zA-Z]{3}(-\d{4})(-\d{4})$";
            Regex sampleRegex = new Regex(patern);
            var res = sampleRegex.IsMatch(strToCheck);
            return res;
        }
    }
}
