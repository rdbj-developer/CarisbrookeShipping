using CarisbrookeShippingAPI.BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    // JSL 07/16/2022 added this controller
    public class CommonMethodsController : ApiController
    {
        [HttpPost]
        // JSL 07/16/2022
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            CommonMethodsForSyncing _helper = new CommonMethodsForSyncing();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End JSL 07/16/2022
    }
}
