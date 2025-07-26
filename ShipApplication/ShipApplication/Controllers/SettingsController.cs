using Newtonsoft.Json;
using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    [AuthorizeFilter] //RDBJ 10/17/2021
    public class SettingsController : Controller
    {
        // GET: Settings
        [AuthorizeFilter]
        public ActionResult Index()
        {
            SettingsHelper _helper = new SettingsHelper();
            ServerConnectModal Modal = LocalDBHelper.ReadDBConfigJson();
            SMTPServerModal SMTPServerDetails = _helper.GetSMTPSettingsJson();
            if (Modal != null)
            {
                ViewBag.serverconnection = Modal;
            }
            if (SMTPServerDetails != null)
            {
                ViewBag.SMTPServerDetails = SMTPServerDetails;
            }

            if (TempData["ServerConnRes"] != null)
            {
                ViewBag.result = Utility.ToString(TempData["ServerConnRes"]);

            }
            if (TempData["DBCreate"] != null)
            {
                ViewBag.dbcreate = Utility.ToString(TempData["DBCreate"]);

            }
            if (TempData["SMTPResult"] != null)
            {
                ViewBag.SMTPResult = Utility.ToString(TempData["SMTPResult"]);
            }

            LoadShipsList();
            return View();
        }
        [HttpPost]
        public ActionResult Index(ServerConnectModal Modal)
        {
            CreateLocalDatabase(Modal);
            LoadShipsList();
            return RedirectToAction("Index", "Settings");
        }
        [HttpPost]
        public ActionResult SaveSmtpCredential(SMTPServerModal Modal)
        {
            SettingsHelper _helper = new SettingsHelper();
            bool res = _helper.SaveSMTPSettingsJson(Modal);
            if (res == true)
                TempData["SMTPResult"] = AppStatic.SUCCESS;
            else
                TempData["SMTPResult"] = AppStatic.ERROR;
            return RedirectToAction("Index", "Settings");
        }

        public void CreateLocalDatabase(ServerConnectModal Modal)
        {
            try
            {
                LocalDBHelper _helper = new LocalDBHelper();
                string connetionString = "Data Source=" + Modal.ServerName + ";Initial Catalog=master;User ID=" + Modal.UserName + ";Password=" + Modal.Password + "";
                using (var connection = new SqlConnection(connetionString))
                {
                    if (connection.IsAvailable())
                    {
                        Modal.IsConnection = true;
                        TempData["ServerConnRes"] = AppStatic.SUCCESS;
                        bool isDbExist = _helper.CheckDatabaseExists(connetionString, AppStatic.DATABASENAME);
                        if (!isDbExist)
                        {
                            bool isDbCreated = _helper.CreateDatabase(connetionString, AppStatic.DATABASENAME);
                            if (!isDbCreated)
                            {
                                TempData["DBCreate"] = AppStatic.ERROR;
                                Modal.DatabaseName = string.Empty;
                                Modal.IsDBCreated = false;
                            }
                            else
                                isDbExist = true;
                        }
                        if (isDbExist) {
                            Modal.DatabaseName = AppStatic.DATABASENAME;
                            Modal.IsDBCreated = true;
                            _helper.CreateDBConfigJson(Modal);
                            bool res = LocalDBHelper.CreateAllTable();
                            if (res == true)
                            {
                                var isInternetAvilable = Utility.CheckInternet();
                                if (isInternetAvilable)
                                {
                                    //Add these entries from AWS                                        
                                    APIHelper _apihelper = new APIHelper();
                                    var lstDocumentsData = _apihelper.GetAllDocumentsForService();
                                    if (lstDocumentsData != null && lstDocumentsData.Count > 0)
                                    {
                                        LocalDBHelper.InsertDocumentsDataFromAWS(lstDocumentsData);
                                    }
                                    var lstFormData = _apihelper.GetAllFormsForService();
                                    if (lstFormData != null && lstFormData.Count > 0)
                                    {
                                        lstFormData = lstFormData.Where(x => x.IsDeleted.HasValue == false || x.IsDeleted.Value == false).ToList();
                                        LocalDBHelper.InsertFormsDataFromAWS(lstFormData);
                                        LocalDBHelper.InsertDocumentsInRiskAssessmentDataFromAWS(lstDocumentsData);
                                    }
                                }
                                else
                                { 
                                    LocalDBHelper.InsertDocumentsData(Server.MapPath("~/Repository/DocumentsIndex.xml"));
                                    LocalDBHelper.InsertFormsData(Server.MapPath("~/Repository/FormsIndex.xml"));
                                    LocalDBHelper.InsertXmlsData(Server.MapPath("~/Repository/XmlsIndex.xml"));
                                    LocalDBHelper.InsertDocumentsInRiskAssessmentData(Server.MapPath("~/Repository/DocumentsIndex.xml"));
                                }
                                InsertLocalDataHelper.InsertUserDataFromAWS();
                            }
                            else
                            {
                                TempData["DBTablesCreate"] = AppStatic.ERROR;
                            }
                        }
                        ServerConnectModal ModalNew = LocalDBHelper.ReadDBConfigJson();
                        if (Modal != null)
                        {
                            ViewBag.serverconnection = ModalNew;
                        }
                    }
                    else
                    {
                        TempData["ServerConnRes"] = AppStatic.ERROR;
                    }
                };
            }
            catch (Exception ex)
            {
                TempData["ServerConnRes"] = AppStatic.ERROR;
            }
        }
        //RDBJ 09/16/2021
        public JsonResult GetUpdateShipsDetails()
        {
            bool res = false;
            try
            {
                LocalDBHelper.CreateCSShipsTableAndGetShipsData(); //RDBJ 10/06/2021
                res = InsertLocalDataHelper.InsertUpdateShipsDataFromAWS();
            }
            catch (Exception ex)
            {
                res = false;
                LogHelper.writelog(ex.Message);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 09/16/2021
        public JsonResult SaveShip(string id, string name)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
            {
                SimpleObject modal = new SimpleObject();
                Session["Ship"] = id;
                Session["ShipName"] = name;
                SessionManager.ShipCode = id;
                modal.id = id;
                modal.name = name;
                SettingsHelper _helper = new SettingsHelper();
                res = _helper.SaveShipJson(modal);
                try
                {
                    LocalDBHelper.InsertDocumentsInRiskAssessmentData(Server.MapPath("~/Repository/DocumentsIndex.xml"));
                }
                catch (Exception)
                {

                }
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ChangeInspector(string isInspector)
        {
            bool res = false;
            try
            {
                res = LocalDBHelper.WriteDBConfigJson(Convert.ToBoolean(isInspector));
            }
            catch (Exception ex)
            {
                res = false;
                LogHelper.writelog(ex.Message);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public void LoadShipsList()
        {
            SettingsHelper _lhelper = new SettingsHelper();
            GIRTableHelper _laHelper = new GIRTableHelper(); //RDBJS 09/17/2021
            List<CSShipsModal> shipsList = null;

            //RDBJS 09/17/2021 Commented
            /*
            APIHelper _helper = new APIHelper();
            bool isInternetAvailable = Utility.CheckInternet();
            if (isInternetAvailable)
            {
                shipsList = _helper.GetAllShips();
                shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
                shipsList = shipsList.OrderBy(x => x.Name).ToList();
            }
            else
            {
                shipsList = _lhelper.GetAllShipsFromJson();
            }
            */

            //RDBJS 09/17/2021
            shipsList = _laHelper.GetAllShips();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").DistinctBy(x => x.Code).ToList(); //RDBJ 10/07/2021 set Distinct
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            //End RDBJS 09/17/2021

            if (shipsList == null || shipsList.Count <= 0)
                shipsList = _lhelper.GetAllShipsFromJson();

            ViewBag.ships = new SelectList(shipsList, "Code", "Name");
            SimpleObject shipModal = _lhelper.GetShipJson();
            if (shipModal != null)
            {
                ViewBag.selectedShip = shipModal;
                Session["Ship"] = shipModal.id;
                Session["ShipName"] = shipModal.name;
            }
        }

        // Sync Fo Manual Testing
        [HttpGet]
        public JsonResult SyncData()
        {
            LogHelper.writelog("SyncData ");
            SMRFormTableHelper _helper = new SMRFormTableHelper();
            _helper.SyncSMRFormsLocalData();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public FileResult DownloadMainSyncServiceSetup()
        {
            string path = Server.MapPath(@"~\Service\CarisbrookeShippingService.exe");
            string mimeType = string.Empty;
            try
            {
                string filename = Path.GetFileName(path);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, "application/force-download", filename);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return File(path, mimeType);
        }

        // RDBJ 02/26/2022
        public JsonResult PerformAction(string jsonmetadata, string straction)
        {
            SettingsHelper _helper = new SettingsHelper();

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
                    dictToReturn = _helper.AjaxPostPerformAction(dictMetadata);
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Settings PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 02/26/2022
    }
}