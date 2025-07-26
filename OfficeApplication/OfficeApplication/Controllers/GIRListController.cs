using Newtonsoft.Json;
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
    public class GIRListController : Controller
    {
        public static string[] ClassificationSociety = { "BV - Bureau Veritas", "GL - Germanisher Lloyd", "LR - Lloyd's Register", "RI - RINA", "NKK - Nippon Kaiji Kyokai" }; // RDBJ 01/10/2022
        public static string[] FlagState = { "GER - German", "IOM - Isle of Man", "UK - United Kingdom", "MLT - Malta", "MAR - Madeira", "AB - Antigue & Barbuda", "LI - Liberia" }; // RDBJ 01/10/2022
        public static string[] PortOfRegistry = { "Leer", "Douglas", "Cowes", "Valletta", "Madeira", "St. John's", "Panama", "Liberia" }; // RDBJ 01/10/2022
        
        // GET: GIRList
        [HttpGet]
        public ActionResult Index(string id,bool isDefectSection=false, string section = "")
        {
            APIHelper _helper = new APIHelper();
            GeneralInspectionReport list = new GeneralInspectionReport();
            list = _helper.GIRFormDetailsViewByGUID(id);

            List<CSShipsModal> shipsList = _helper.GetAllShips();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();

            // JSL 01/05/2022
            if (SessionManager.UserGroup == "8") // 8 is used to avoid show some ships for Visitor Groups
            {
                shipsList = shipsList
                    .Where(x =>
                    x.Name.ToLower().StartsWith("j")
                    || x.Name.ToLower().StartsWith("c")
                    || x.Name.ToLower().StartsWith("vectis")
                    )
                    .ToList();
            }
            // End JSL 01/05/2022

            ViewBag.ShipDatas = shipsList;
            //ViewBag.ShipName = list.ShipName; // RDBJ 02/11/2022 commented this line
            ViewBag.ShipCode = list.Ship;   // RDBJ 02/11/2022

            list.CSShipsModal = _helper.GIRGetGeneralDescription(list.Ship); //RDBJ 10/07/2021

            ViewBag.ClassificationSociety = bindDropDown(ClassificationSociety, Convert.ToInt32(list.CSShipsModal.ClassificationSocietyId)); // RDBJ 01/10/2022
            ViewBag.FlagState = bindDropDown(FlagState, Convert.ToInt32(list.CSShipsModal.FlagStateId)); // RDBJ 01/10/2022
            ViewBag.PortOfRegistry = bindDropDown(PortOfRegistry, Convert.ToInt32(list.CSShipsModal.PortOfRegistryId)); // RDBJ 01/10/2022

            // JSL 12/31/2022
            if (list.CSShipsModal != null)
            {
                if (list.ShipID == null || list.ShipID == 0)
                {
                    list.ShipID = list.CSShipsModal.ShipId;
                    list.ShipName = list.CSShipsModal.Name;
                }
            }
            // End JSL 12/31/2022

            return View(list);
        }

        [HttpGet]
        public ActionResult DeficienciesDetails(Guid? id)
        {
            //APIHelper _helper = new APIHelper();
            //GeneralInspectionReport list = new GeneralInspectionReport();
            //list = _helper.GIRFormDetailsView(id);
            ViewBag.UserGroup = Convert.ToInt32(Session["UserGroup"]); // RDBJ 12/18/2021
            return View();
        }
        public ActionResult ListForms()
        {
            APIHelper _helper = new APIHelper();
            List<GeneralInspectionReport> list = new List<GeneralInspectionReport>();
            list=_helper.GetAllGIRForms();
            ViewBag.Listdata = list.OrderByDescending(x=>x.Date);
            return View();
        }

        #region GIR
        [HttpGet]
        public ActionResult GeneralInspectionReport()
        {
            GeneralInspectionReport Modal = new GeneralInspectionReport();
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = _helper.GetAllShips();

            Modal.GIRPhotographs = new List<GIRPhotographs>();  // RDBJ 02/07/2022

            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            
            ViewBag.ShipDatas = shipsList;
            ViewBag.ShipName = SessionManager.ShipName;
            Modal.Ship = SessionManager.ShipCode; //RDBJ 10/07/2021
            Modal.CSShipsModal = _helper.GIRGetGeneralDescription(SessionManager.ShipCode); //RDBJ 10/07/2021

            ViewBag.ClassificationSociety = bindDropDown(ClassificationSociety, Convert.ToInt32(Modal.CSShipsModal.ClassificationSocietyId)); // RDBJ 01/10/2022
            ViewBag.FlagState = bindDropDown(FlagState, Convert.ToInt32(Modal.CSShipsModal.FlagStateId)); // RDBJ 01/10/2022
            ViewBag.PortOfRegistry = bindDropDown(PortOfRegistry, Convert.ToInt32(Modal.CSShipsModal.PortOfRegistryId)); // RDBJ 01/10/2022

            return View(Modal);
        }

        [HttpPost]
        public JsonResult GIRGetGeneralDescriptionByShip(string Shipvalue)
        {
            CSShipsModal Modal = new CSShipsModal(); //RDBJ 10/07/2021 Change CSShipsModal from GeneeralInspectionReport
            APIHelper _apihelper = new APIHelper();
            Modal = _apihelper.GIRGetGeneralDescription(Shipvalue);
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GeneralInspectionReport(GeneralInspectionReport Modal)
        {
            APIHelper _aHelper = new APIHelper();
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    SetGIRModal(ref Modal);
                    APIResponse resp = _aHelper.SubmitGIR(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("GeneralInspectionReport");
        }

        public JsonResult GIRAutoSave(GeneralInspectionReport Modal)
        {
            string UniqueFormID = string.Empty;
            try
            {
                if (Modal != null
                    && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                    )
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    SetGIRModal(ref Modal);
                    APIHelper _aHelper = new APIHelper();

                    if (Modal.FormVersion == 0)
                    {
                        Modal.FormVersion = 1;
                        Guid FormGUID = Guid.NewGuid();
                        Modal.UniqueFormID = FormGUID;
                        Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    else
                    {
                        Modal.FormVersion = Modal.FormVersion + Convert.ToDecimal(0.01);
                    }
                    UniqueFormID = _aHelper.GIRAutoSave(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIR AutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(new { GIRFormID = 0, UniqueFormID = UniqueFormID, FormVersion = Modal.FormVersion });
        }

        //RDBJ 10/07/2021
        public JsonResult GIRShipGeneralDescriptionSave(GeneralInspectionReport Modal)
        {
            bool res = false;
            try
            {
                if (Modal.CSShipsModal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.CSShipsModal.Code))
                        Modal.CSShipsModal.Code = Modal.Ship;

                    APIHelper _aHelper = new APIHelper();
                    res = _aHelper.GIRShipGeneralDescriptionSave(Modal.CSShipsModal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRShipGeneralDescriptionSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/07/2021


        [HttpPost]
        public ActionResult GIRSaveFormDarf(GeneralInspectionReport Modal)
        {
            APIHelper _aHelper = new APIHelper();
            long resp = 0;
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    Modal.ShipName = SessionManager.ShipName;
                    Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.IsSynced = true;
                    resp = _aHelper.GIRSaveFormDarf(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIR Save Form Darf Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("Index", "Drafts");
        }
        public void SetGIRModal(ref GeneralInspectionReport Modal)
        {
            //Modal.ShipName = SessionManager.ShipName;

            if (Modal.GIRSafeManningRequirements != null && Modal.GIRSafeManningRequirements.Count > 0)
            {
                foreach (var item in Modal.GIRSafeManningRequirements)
                {
                    item.Ship = Modal.Ship;
                    item.OnBoard = Convert.ToBoolean(item.OnBoard);
                    item.RequiredbySMD = Convert.ToBoolean(item.RequiredbySMD);
                }
            }
            if (Modal.GIRCrewDocuments != null && Modal.GIRCrewDocuments.Count > 0)
            {
                foreach (var item in Modal.GIRCrewDocuments)
                {
                    item.Ship = Modal.Ship;
                    item.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    item.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                }
            }
            if (Modal.GIRRestandWorkHours != null && Modal.GIRRestandWorkHours.Count > 0)
            {
                foreach (var item in Modal.GIRRestandWorkHours)
                {
                    item.Ship = Modal.Ship;
                }
            }
            if (Modal.GIRDeficiencies != null && Modal.GIRDeficiencies.Count > 0)
            {
                foreach (var item in Modal.GIRDeficiencies)
                {
                    item.Ship = Modal.Ship;
                    item.IsClose = false;
                    item.ReportType = "GI";
                }
            }
            if (Modal.GIRPhotographs != null && Modal.GIRPhotographs.Count > 0)
            {
                foreach (var item in Modal.GIRPhotographs)
                {
                    item.Ship = Modal.Ship;
                }
            }
        }
        
        public JsonResult AddGIRDeficiencies(GIRDeficiencies Modal) // RDBJ 02/17/2022 set with JsonResult from Void
        {
            //string strRetDeficienciesUniqueID = string.Empty; // RDBJ 02/19/2022 commented this line // RDBJ 02/17/2022
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>(); // RDBJ 02/19/2022
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;

                    //RDBJ 09/18/2021
                    Guid DeficienciesGUID = Guid.NewGuid();
                    Modal.DeficienciesUniqueID = DeficienciesGUID;
                    //End RDBJ 09/18/2021

                    Modal.IsClose = false;
                    //Modal.ReportType = "GI"; // RDBJ 01/15/2022 commented due to pass from front-end
                    Modal.IsSynced = false;
                    APIHelper _aHelper = new APIHelper();
                    var no = _aHelper.getNextNumber(Modal.Ship, Modal.ReportType, Convert.ToString(Modal.UniqueFormID)); //RDBJ 11/02/2021 added UniqueFormID //RDBJ 09/18/2021 removed ,Modal.UniqueFormID,Modal.Section
                    Modal.No = no;
                    var resp = _aHelper.AddGIRDeficiencies(Modal);

                    // RDBJ 02/17/2022 wrapped in if
                    if (resp.result == AppStatic.SUCCESS)
                    {
                        //strRetDeficienciesUniqueID = resp.msg;  //res.msg return DefUniId // RDBJ 02/19/2022 commented this line
                        retDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.msg); // RDBJ 02/19/2022
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficiencies Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(retDictMetaData, JsonRequestBehavior.AllowGet); // RDBJ 02/17/2022
        }
        [HttpPost]
        public ActionResult GIRDetails(string code)
        {
            List<GIRDataList> Modal = new List<GIRDataList>();
            if (string.IsNullOrWhiteSpace(code))
                code = SessionManager.ShipCode;
            if (Modal == null || Modal.Count <= 0)
            {
                APIHelper _apihelper = new APIHelper();
                Modal = _apihelper.GetDeficienciesData(code);
            }
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult getNextNumber(string ship, string reportType, string UniqueFormID) //RDBJ 11/02/2021 Added UniqueFormID //RDBJ 09/24/2021 Added reportType 
        {
            APIHelper _aHelper = new APIHelper();
            int no = _aHelper.getNextNumber(ship, reportType, UniqueFormID); //RDBJ 11/02/2021 UniqueFormID //RDBJ 09/24/2021 Added reportType  //RDBJ 09/18/2021 removed ,null,""
            return Json(no, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region IAF
        [HttpGet]
        public ActionResult InternalAuditDetails(Guid id)
        {
            ViewBag.UserGroup = Convert.ToInt32(Session["UserGroup"]); // RDBJ 12/21/2021
            return View();
        }
        #endregion

        #region common functions

        // RDBJ 01/10/2022
        public List<Dictionary<string, object>> bindDropDown(string[] ddlistItems, int selectedId)
        {
            List<Dictionary<string, object>> lstDic = new List<Dictionary<string, object>>();
            for (int i = 0; i < ddlistItems.Length; i++)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>();
                keyValues["text"] = ddlistItems[i];
                if (i == selectedId)
                {
                    keyValues["selected"] = true;
                }
                else
                {
                    keyValues["selected"] = false;
                }
                lstDic.Add(keyValues);
            }
            return lstDic;
        }
        // End RDBJ 01/10/2022
        #endregion

    }
}