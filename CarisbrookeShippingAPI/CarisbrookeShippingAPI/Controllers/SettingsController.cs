using CarisbrookeShippingAPI.BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    //[Authorize] // JSL 09/26/2022
    // RDBJ 02/24/2022 Added Settings Controller
    public class SettingsController : ApiController
    {
        SettingsHelper _ObjSettingsHelper = new SettingsHelper();

        // RDBJ2 02/23/2022
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _ObjSettingsHelper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End RDBJ2 02/23/2022
    }
}
