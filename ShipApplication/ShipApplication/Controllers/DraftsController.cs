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
    [AuthorizeFilter] //RDBJ 10/17/2021
    public class DraftsController : Controller
    {
        public static string[] ClassificationSociety = { "BV - Bureau Veritas", "GL - Germanisher Lloyd", "LR - Lloyd's Register", "RI - RINA", "NKK - Nippon Kaiji Kyokai" }; // RDBJ 01/11/2022
        public static string[] FlagState = { "GER - German", "IOM - Isle of Man", "UK - United Kingdom", "MLT - Malta", "MAR - Madeira", "AB - Antigue & Barbuda", "LI - Liberia" }; // RDBJ 01/11/2022
        public static string[] PortOfRegistry = { "Leer", "Douglas", "Cowes", "Valletta", "Madeira", "St. John's", "Panama", "Liberia" }; // RDBJ 01/11/2022

        // GET: Drafts
        [AuthorizeFilter]
        public ActionResult Index()
        {
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            LoadShipsList();
            return View();
        }

        #region GIR Draft
        [AuthorizeFilter]
        public ActionResult GIRDrafts()
        {
            return View();
        }
        public ActionResult GetGIRDrafts(string ship)
        {
            //APIHelper _helper = new APIHelper();
            //List<GIRData> Modal = new List<GIRData>();
            //DraftsHelper _dHelper = new DraftsHelper();
            //Modal = _dHelper.GET_GIRDrafts_Local_DB(ship);
            //if (Modal == null)
            //{
            //    bool isInternetAvailable = Utility.CheckInternet();
            //    if (isInternetAvailable)
            //        Modal = _helper.GetGIRDrafts(ship);             
            //}             
            //return Json(Modal, JsonRequestBehavior.AllowGet);
            APIHelper _helper = new APIHelper();
            List<GIRData> Modal = new List<GIRData>();
            DraftsHelper _dHelper = new DraftsHelper();
            //bool isInternetAvailable = Utility.CheckInternet();
            //if (!isInternetAvailable)
            //{
            //    Modal = _helper.GetGIRDrafts(ship);
            //}
            //else
            //{
            //}
            Modal = _dHelper.GET_GIRDrafts_Local_DB(ship);
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GIRformData(string id, bool isDefectSection = false, string section = "")
        {
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            LoadShipsList();

            GeneralInspectionReport Modal = new GeneralInspectionReport();
            DraftsHelper _dHelper = new DraftsHelper();
            Modal = _dHelper.GIRFormDetailsView_Local_DB(id);
            Modal.CarriedOutByTheDOOW = String.IsNullOrEmpty(Modal.CarriedOutByTheDOOW) ? "false" : Modal.CarriedOutByTheDOOW;   // RDBJ 02/09/2022

            //RDBJ 10/06/2021
            GIRTableHelper _laHelper = new GIRTableHelper();
            Modal.CSShipsModal = _laHelper.GIRGetGeneralDescription(Modal.Ship);
            //End RDBJ 10/06/2021

            // RDBJ 02/11/2022 commented 
            /*
            SessionManager.ShipCode = Modal.Ship;
            SessionManager.ShipName = Modal.ShipName;
            */
            // RDBJ 02/11/2022 commented 

            // RDBJ 02/11/2022
            ViewBag.ShipCode = Modal.Ship;
            ViewBag.ShipName = Modal.ShipName;
            // End RDBJ 02/11/2022

            ViewBag.SavedAsDraft = Modal.SavedAsDraft;

            ViewBag.ClassificationSociety = bindDropDown(ClassificationSociety, Convert.ToInt32(Modal.CSShipsModal.ClassificationSocietyId)); // RDBJ 01/11/2022
            ViewBag.FlagState = bindDropDown(FlagState, Convert.ToInt32(Modal.CSShipsModal.FlagStateId)); // RDBJ 01/11/2022
            ViewBag.PortOfRegistry = bindDropDown(PortOfRegistry, Convert.ToInt32(Modal.CSShipsModal.PortOfRegistryId)); // RDBJ 01/11/2022

            return View(Modal);
        }
        public void LoadShipsList()
        {
            SettingsHelper _lhelper = new SettingsHelper();
            GIRTableHelper _laHelper = new GIRTableHelper(); //RDBJS 09/17/2021
            List<CSShipsModal> shipsList = null;

            //APIHelper _helper = new APIHelper(); //RDBJS 09/17/2021 Commented
            //bool isInternetAvailable = Utility.CheckInternet();
            //if (isInternetAvailable)
            //{
            //    shipsList = _helper.GetAllShips();
            //    shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
            //    shipsList = shipsList.OrderBy(x => x.Name).ToList();
            //}
            //else
            //{

            //}

            //RDBJS 09/17/2021
            shipsList = _laHelper.GetAllShips();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").DistinctBy(x => x.Code).ToList(); //RDBJ 10/07/2021 set Distinct
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            //End RDBJS 09/17/2021

            //RDBJS 09/17/2021 Added if
            if (shipsList == null || shipsList.Count <= 0)
                shipsList = _lhelper.GetAllShipsFromJson();

            ViewBag.ships = new SelectList(shipsList, "Code", "Name");
        }
        public ActionResult _GIR_DeficienciesSection(string id)
        {
            GeneralInspectionReport Modal = new GeneralInspectionReport();
            DraftsHelper _dHelper = new DraftsHelper();
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            Modal = _dHelper.GIRFormGetDeficiency_Local_DB(id);
            if (Modal.SavedAsDraft == null)
            {
                Modal.SavedAsDraft = true;
            }

            //RDBJ 10/19/2021
            if (Modal.IsDeficienciesSectionComplete == null)
            {
                Modal.IsDeficienciesSectionComplete = false;
            }
            //End RDBJ 10/19/2021

            ViewBag.SavedAsDraft = Modal.SavedAsDraft;  // RDBJ 03/21/2022
            return PartialView("GIR/_GIR_DeficienciesSection", Modal);
        }
        #endregion

        #region SIR Draft
        public ActionResult SIRDrafts()
        {
            return View();
        }
        public ActionResult GetSIRDrafts(string ship)
        {
            //APIHelper _helper = new APIHelper();
            List<SIRData> Modal = new List<SIRData>();
            //bool isInternetAvailable = Utility.CheckInternet();
            //if (!isInternetAvailable)
            //{
            //    Modal = _helper.GetSIRDrafts(ship);
            //}
            //else
            //{
            DraftsHelper _dHelper = new DraftsHelper();
            Modal = _dHelper.GET_SIRDrafts_Local_DB(ship);
            //}
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SIRformData(string id, bool isDefectSection = false, string section = "")
        {
            LoadShipsList();
            //APIHelper _helper = new APIHelper();
            SIRModal Modal = new SIRModal();
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            //bool isInternetAvailable = Utility.CheckInternet();
            //if (!isInternetAvailable)
            //{
            //    Modal = _helper.SIRFormDetailsView(id);
            //}
            //else
            //{
            DraftsHelper _dHelper = new DraftsHelper();
            Modal = _dHelper.SIRFormDetailsView_Local_DB(id);
            //}
            ViewBag.Inspector = ServerModal.IsInspector;
            ViewBag.SavedAsDraft = Modal.SuperintendedInspectionReport.SavedAsDraft;
            ViewBag.ShipCode = Modal.SuperintendedInspectionReport.ShipName;
            return View(Modal);
        }
        #endregion

        public ActionResult DeleteGISIIADrafts(string GISIIAFormID, string type) // RDBJ 01/31/2022 rename function //RDBJ 10/09/2021 int to string GISTFormID
        {
            APIHelper _helper = new APIHelper();
            DraftsHelper _dHelper = new DraftsHelper();
            _dHelper.DeleteGISIIADrafts_Local_DB(GISIIAFormID, type);
            return Json(new { ok = "OK" }, JsonRequestBehavior.AllowGet);
        }

        #region IAR Form
        public ActionResult IARformData(string id)
        {
            IAF list = new IAF();

            IAFHelper _lhelper = new IAFHelper();
            list = _lhelper.IAFormDetailsView_Local_DB(id);
            list.InternalAuditForm.AuditNo = "1";   // RDBJ 02/09/2022
            // RDBJ 05/02/2022
            LoadShipsList();
            ViewBag.ShipName = list.InternalAuditForm.ShipName;
            // End RDBJ 05/02/2022

            Dictionary<string, string> response = new Dictionary<string, string>();
            List<String> data = new List<string>();
            if (list != null && list.InternalAuditForm != null)
            {
                response = _lhelper.GetNumberForNotes(list.InternalAuditForm.ShipName
                    , Convert.ToString(list.InternalAuditForm.UniqueFormID)); // RDBJ 05/02/2022
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        data.Add(item.Value);
                    }
                }
            }
            ViewBag.Numbers = data;
            ViewBag.StringNumbers = String.Join(",", data.Cast<string>().ToArray()); // RDBJ 05/02/2022

            ViewBag.AuditNoteTypeList = new List<string>() { "ISM-Non Conformity", "ISPS-Non Conformity", "ISM-Observation", "ISPS-Observation", "MLC-Deficiency" }; //RDBJ 11/18/2021

            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;    // RDBJ 05/02/2022
            ViewBag.SavedAsDraft = list.InternalAuditForm.SavedAsDraft; // RDBJ 05/02/2022

            // JSL 05/22/2022
            bool IsLatestReferencesFound = false;
            var SMSdata = _lhelper.GetSMSReferenceData();
            var SSPdata = _lhelper.GetSSPReferenceData();
            var MLCdata = _lhelper.GetMLCRegulationTree();

            if ((SMSdata != null && SMSdata.Count > 0)
                || (SSPdata != null && SSPdata.Count > 0)
                || (MLCdata != null && MLCdata.Count > 0)
                )
            {
                IsLatestReferencesFound = true;
            }

            ViewBag.IsLatestReferencesFound = IsLatestReferencesFound;
            // End JSL 05/22/2022

            return View(list);
        }
        #endregion

        // RDBJ 01/31/2022
        public ActionResult GetGISIIADraftsReportsByShip(string code, string type)
        {
            DraftsHelper _dHelper = new DraftsHelper();
            List<GIRData> lstGIRDrafts;
            List<SIRData> lstSIRDrafts;
            List<AuditList> lstIARDrafts;

            if (type.ToUpper() == "GI")
            {
                lstGIRDrafts = _dHelper.GET_GIRDrafts_Local_DB(code);
                return Json(lstGIRDrafts, JsonRequestBehavior.AllowGet);
            }
            else if (type.ToUpper() == "SI")
            {
                lstSIRDrafts = _dHelper.GET_SIRDrafts_Local_DB(code);
                return Json(lstSIRDrafts, JsonRequestBehavior.AllowGet);
            }
            else
            {
                lstIARDrafts = _dHelper.GET_IARDrafts_Local_DB(code);
                return Json(lstIARDrafts, JsonRequestBehavior.AllowGet);
            }
        }
        // End RDBJ 01/31/2022

        #region common function
        // RDBJ 01/11/2022
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
        // End RDBJ 01/11/2022
        #endregion
    }
}