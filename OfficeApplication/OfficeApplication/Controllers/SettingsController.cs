using Newtonsoft.Json;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }

        #region Commons
        // RDBJ 02/24/2022
        public JsonResult PerformAction(string jsonmetadata, string straction)
        {
            APIHelper _helper = new APIHelper();
            //Dictionary<string, object> dictToReturn = new Dictionary<string, object>();

            Dictionary<string, string> dictToReturn = new Dictionary<string, string>();
            dictToReturn.Add("jsonMetaData", jsonmetadata);
            dictToReturn.Add("straction", straction);

            var dictMetadata = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(jsonmetadata))
            {
                dictMetadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonmetadata);
                dictMetadata["strAction"] = straction;

                try
                {
                    dictToReturn = _helper.PostAsyncAPICall(APIURLHelper.APISettings, "CommonPostAPICall", dictMetadata);
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Settings PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 02/24/2022
        #endregion
    }
}