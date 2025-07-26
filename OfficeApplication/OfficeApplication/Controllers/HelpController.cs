using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class HelpController : Controller
    {
        // GET: Help
        public ActionResult Index()
        {
            // RDBJ 12/28/2021
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            ViewBag.ShipCode = SessionManager.ShipCode;
            // End RDBJ 12/28/2021
            return View();
        }

        // RDBJ 12/28/2021
        [HttpPost]
        public ActionResult GetHelpAndSupportsList()
        {
            List<HelpAndSupport> response = new List<HelpAndSupport>();
            APIHelper _helper = new APIHelper();
            response = _helper.GetHelpAndSupportsList();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/28/2021

        // RDBJ 12/27/2021
        [HttpPost]
        public JsonResult InsertOrUpdateHelpAndSupport(HelpAndSupport Modal)
        {
            APIResponse response = new APIResponse();
            APIHelper _aHelper = new APIHelper();
            bool blnNeedToUpdate = false;
            try
            {
                if (Modal != null)
                {
                    if (Modal.Id == null
                        || Modal.Id == Guid.Empty)
                    {
                        Guid HSGuid = Guid.NewGuid();
                        Modal.Id = HSGuid;
                    }
                    else
                    {
                        blnNeedToUpdate = true;
                        Modal.ModifiedBy = SessionManager.Username;
                    }

                    if (string.IsNullOrWhiteSpace(Modal.ShipId))
                        Modal.ShipId = SessionManager.ShipCode;

                    if (string.IsNullOrWhiteSpace(Modal.CreatedBy) && blnNeedToUpdate == false)
                        Modal.CreatedBy = SessionManager.Username;

                    response = _aHelper.InsertOrUpdateHelpAndSupport(Modal);

                    // RDBJ 12/30/2021 added if to set log who will delete it
                    if(Modal.IsStatus == 1)
                        LogHelper.LogForHelpAndSupportClose(Convert.ToString(Modal.Id), SessionManager.Username, "Closed");
                }
            }
            catch (Exception ex)
            {
                response.result = AppStatic.ERROR;
                LogHelper.writelog("InsertOrUpdateHelpAndSupport Error : " + ex.Message);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/27/2021

        // RDBJ 12/29/2021
        [HttpPost]
        public JsonResult DeleteHelpAndSupport(string ID)
        {
            APIResponse response = new APIResponse();
            APIHelper _helper = new APIHelper();
            response = _helper.DeleteHelpAndSupport(ID);
            LogHelper.LogForHelpAndSupportClose(ID, SessionManager.Username, "Deleted");
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/29/2021

        #region Common Functions
        // RDBJ 12/28/2021
        public List<CSShipsModal> LoadShipsList()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = _helper.GetAllShips();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            return shipsList;
        }
        // End RDBJ 12/28/2021
        #endregion
    }
}