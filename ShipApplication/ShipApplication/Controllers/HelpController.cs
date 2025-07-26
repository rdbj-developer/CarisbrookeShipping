using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    // RDBJ 12/30/2021 Added Controller
    [AuthorizeFilter]
    public class HelpController : Controller
    {
        HelpAndSupportHelper _HShelper; // RDBJ 12/30/2021

        // RDBJ 12/30/2021 
        public ActionResult Index()
        {
            LoadShipsList();
            return View();
        }
        // End RDBJ 12/30/2021 

        // RDBJ 12/31/2021
        [HttpPost]
        public ActionResult GetHelpAndSupportsList()
        {
            List<HelpAndSupport> resHSList = new List<HelpAndSupport>();
            List<CSShipsModal> lstCSShips = new List<CSShipsModal>();

            _HShelper = new HelpAndSupportHelper();
            resHSList = _HShelper.GetHelpAndSupportsList().Where(x=>x.ShipId == SessionManager.ShipCode).ToList();
            lstCSShips = LoadShipsList();

            var response = (from HSList in resHSList
                            join left in lstCSShips on HSList.ShipId equals left.Code into joinedList
                            from ShipList in joinedList.DefaultIfEmpty()
                            select new HelpAndSupport { 
                                Id = HSList.Id,
                                ShipId = ShipList.Name,
                                Comments = HSList.Comments,
                                StrStatus = HSList.IsStatus == 0 ? "Open" : "Closed",
                                StrPriority = HSList.Priority == 1 ? "Low" : HSList.Priority == 2 ? "Medium" : HSList.Priority == 3 ? "High" : HSList.Priority == 4 ? "Critical" : "",
                                CreatedBy = HSList.CreatedBy,
                                CreatedDateTime = HSList.CreatedDateTime,
                            }).ToList();


            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/31/2021

        // RDBJ 12/30/2021 
        [HttpPost]
        public JsonResult InsertOrUpdateHelpAndSupport(HelpAndSupport Modal)
        {
            _HShelper = new HelpAndSupportHelper();
            APIResponse response = new APIResponse();
            bool blnNeedToUpdate = false;
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
                    Modal.ModifiedDateTime = Utility.ToDateTimeUtcNow();    // RDBJ 12/31/2021
                }

                if (string.IsNullOrWhiteSpace(Modal.ShipId))
                    Modal.ShipId = SessionManager.ShipCode;

                if (string.IsNullOrWhiteSpace(Modal.CreatedBy) && blnNeedToUpdate == false)
                    Modal.CreatedBy = SessionManager.Username;

                response = _HShelper.InsertOrUpdateHelpAndSupport(Modal, blnNeedToUpdate);

                if (Modal.IsStatus == 1)
                    LogHelper.LogForPerformClosedDeleted("Help&Support".Trim(), Convert.ToString(Modal.Id), SessionManager.Username, "Closed");
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/30/2021 


        #region Common Functions
        // RDBJ 12/31/2021
        public List<CSShipsModal> LoadShipsList()
        {
            SettingsHelper _settingsHelper = new SettingsHelper();
            GIRTableHelper _GIRTableHelper = new GIRTableHelper();
            List<CSShipsModal> shipsList = null;

            shipsList = _GIRTableHelper.GetAllShips();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").DistinctBy(x => x.Code).ToList(); //RDBJ 10/07/2021 set Distinct
            shipsList = shipsList.OrderBy(x => x.Name).ToList();

            if (shipsList == null || shipsList.Count <= 0)
                shipsList = _settingsHelper.GetAllShipsFromJson();

            ViewBag.ships = new SelectList(shipsList, "Code", "Name");
            return shipsList;
        }
        // End RDBJ 12/31/2021
        #endregion
    }
}