using Newtonsoft.Json;
using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Helpers;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    [AuthorizeFilter] //RDBJ 10/17/2021
    public class FormsController : Controller
    {
        public static string[] ClassificationSociety = { "BV - Bureau Veritas", "GL - Germanisher Lloyd", "LR - Lloyd's Register", "RI - RINA", "NKK - Nippon Kaiji Kyokai" }; // RDBJ 01/11/2022
        public static string[] FlagState = { "GER - German", "IOM - Isle of Man", "UK - United Kingdom", "MLT - Malta", "MAR - Madeira", "AB - Antigue & Barbuda", "LI - Liberia" }; // RDBJ 01/11/2022
        public static string[] PortOfRegistry = { "Leer", "Douglas", "Cowes", "Valletta", "Madeira", "St. John's", "Panama", "Liberia" }; // RDBJ 01/11/2022

        #region Forms
        // GET: Forms
        [AuthorizeFilter]
        public ActionResult Index()
        {
            try
            {
                SettingsHelper _helper = new SettingsHelper();
                SimpleObject shipModal = _helper.GetShipJson();
                if (shipModal != null)
                {
                    Session["ShipName"] = shipModal.name;
                    Session["Ship"] = shipModal.id;
                }
            }
            catch (Exception)
            { }
            string strServiceUrl = string.Empty;
            try
            {
                strServiceUrl = ConfigurationManager.AppSettings["FileServiceUrl"].ToString();
                if (strServiceUrl.Contains("http://localhost:3063/CarisbrookeFileService/"))
                {
                    strServiceUrl = "http://localhost:80/Temporary_Listen_Addresses/CarisbrookeFileService/";
                    try
                    {
                        //Helps to open the Root level web.config file.
                        Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                        //Modifying the AppKey from AppValue to AppValue1
                        webConfigApp.AppSettings.Settings["FileServiceUrl"].Value = strServiceUrl;
                        //Save the Modified settings of AppSettings.
                        webConfigApp.Save();
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception ex)
            {
                //Helps to open the Root level web.config file.
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                //Modifying the AppKey from AppValue to AppValue1
                webConfigApp.AppSettings.Settings.Add("FileServiceUrl", "http://localhost:80/Temporary_Listen_Addresses/CarisbrookeFileService/");

                //Save the Modified settings of AppSettings.
                webConfigApp.Save();

                strServiceUrl = "http://localhost:80/Temporary_Listen_Addresses/CarisbrookeFileService/";// "http://localhost:3063/CarisbrookeFileService/";
            }
            ViewBag.ServiceUrl = strServiceUrl;
            try
            {
                List<FormModal> FormList = new List<FormModal>();
                FormTableHelper _helper = new FormTableHelper();
                FormList = _helper.GetAllFormsFromLocalDB();
                if (FormList == null || FormList.Count <= 0)
                {
                    bool isInternetAvailable = Utility.CheckInternet();
                    if (isInternetAvailable)
                    {
                        // Get Data Via API
                        APIHelper _aHelper = new APIHelper();
                        FormList = _aHelper.GetAllForms();
                    }
                }
                if (FormList != null && FormList.Count > 0)
                {
                    FormList = FormList.DistinctBy(item => item.Title).ToList();
                    foreach (var item in FormList)
                    {
                        try
                        {
                            if (item.Code == "SCSI")
                                item.DownloadPath = JsonConvert.SerializeObject(string.Format(@"{0}SCSI\SCSI.xlsx", Convert.ToString(ConfigurationManager.AppSettings["SavedFormsPath"])));
                            else if (item.Code == "BSI")
                                item.DownloadPath = JsonConvert.SerializeObject(string.Format(@"{0}BSI\BSI Form.xml", Convert.ToString(ConfigurationManager.AppSettings["SavedFormsPath"])));
                            else if (item.Code == "CO")
                                item.DownloadPath = JsonConvert.SerializeObject(string.Format(@"{0}CO\Crew Overview.xml", Convert.ToString(ConfigurationManager.AppSettings["SavedFormsPath"])));
                            //else if (item.Code == "RAF")
                            //    item.DownloadPath = JsonConvert.SerializeObject(string.Format(@"{0}Risk Assessments\Risk Assessments.xlsx", Convert.ToString(ConfigurationManager.AppSettings["SavedFormsPath"])));
                            else
                                item.DownloadPath = "";
                            item.TemplatePath = JsonConvert.SerializeObject(item.TemplatePath);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    //FormList.ForEach(x => x.TemplatePath = JsonConvert.SerializeObject(x.TemplatePath));
                    FormList = FormList.Where(x => x.CanBeOpened.HasValue && x.CanBeOpened.Value).OrderBy(x => x.Title).ToList();
                }
                ViewBag.FormList = FormList;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Forms->Index " + ex.Message);
            }
            try
            {
                var isUpdateIsSynced = ConfigurationManager.AppSettings["IsUpdateIsSynced"].ToString();
            }
            catch (Exception ex)
            {
                //Helps to open the Root level web.config file.
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                //Modifying the AppKey from AppValue to AppValue1
                webConfigApp.AppSettings.Settings.Add("IsUpdateIsSynced", "False");

                //Save the Modified settings of AppSettings.
                webConfigApp.Save();
                LocalDBHelper.ExecuteQuery("Update RiskAssessmentForm SET IsSynced=0 Where IsSynced=1");
            }
            try
            {
                var isOfficeAppUrlExist = ConfigurationManager.AppSettings["OfficeAPPUrl"].ToString();
            }
            catch (Exception)
            {
                //Helps to open the Root level web.config file.
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                //Modifying the AppKey from AppValue to AppValue1
                webConfigApp.AppSettings.Settings.Add("OfficeAPPUrl", "http://52.56.98.197/OfficeApplication/");

                //Save the Modified settings of AppSettings.
                webConfigApp.Save();
            }
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
                if (apiUrl.Contains("http://52.56.98.197/CarisbrookeShipping/"))
                {
                    apiUrl = "http://ims.carisbrooke.co/CarisbrookeShipping/";
                    try
                    {
                        //Helps to open the Root level web.config file.
                        Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                        //Modifying the AppKey from AppValue to AppValue1
                        webConfigApp.AppSettings.Settings["APIUrl"].Value = apiUrl;
                        //Save the Modified settings of AppSettings.
                        webConfigApp.Save();
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception ex)
            {
                //Helps to open the Root level web.config file.
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                //Modifying the AppKey from AppValue to AppValue1
                webConfigApp.AppSettings.Settings.Add("APIUrl", "http://ims.carisbrooke.co/CarisbrookeShipping/");

                //Save the Modified settings of AppSettings.
                webConfigApp.Save();

            }
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["OfficeAPPUrl"].ToString();
                if (apiUrl.Contains("http://52.56.98.197/OfficeApplication/"))
                {
                    apiUrl = "http://ims.carisbrooke.co/OfficeApplication/";
                    try
                    {
                        //Helps to open the Root level web.config file.
                        Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                        //Modifying the AppKey from AppValue to AppValue1
                        webConfigApp.AppSettings.Settings["OfficeAPPUrl"].Value = apiUrl;
                        //Save the Modified settings of AppSettings.
                        webConfigApp.Save();
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception ex)
            {
                //Helps to open the Root level web.config file.
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                //Modifying the AppKey from AppValue to AppValue1
                webConfigApp.AppSettings.Settings.Add("OfficeAPPUrl", "http://ims.carisbrooke.co/OfficeApplication/");

                //Save the Modified settings of AppSettings.
                webConfigApp.Save();

            }
            try
            {
                AssetManagmentEquipmentListHelper _assteHelper = new AssetManagmentEquipmentListHelper();
                _assteHelper.CreateAssetTables();
            }
            catch (Exception)
            {

            }
            return View();
        }
        #endregion

        #region SMR
        [AuthorizeFilter]
        public ActionResult SMR()
        {
            SMRModal Modal = new SMRModal();
            return View();
        }
        [HttpPost]
        public ActionResult SMR(SMRModal Modal)
        {
            APIHelper _aHelper = new APIHelper();
            SMRFormTableHelper _laHelper = new SMRFormTableHelper();
            try
            {
                if (Modal != null)
                {
                    //Modal.ShipID = SessionManager.ShipCode;
                    Modal.ShipName = SessionManager.ShipName;
                    Modal.Year = Utility.ToDateTimeUtcNow().Year;
                    Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    if (Modal.SMRFormCrewMemberList != null && Modal.SMRFormCrewMemberList.Count > 0)
                    {
                        foreach (var item in Modal.SMRFormCrewMemberList)
                        {
                            item.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            item.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        }
                    }
                }
                bool isInternetAvailable = Utility.CheckInternet();
                LogHelper.writelog("Internet Available : " + isInternetAvailable.ToString());
                if (isInternetAvailable)
                {
                    APIResponse resp = _aHelper.SubmitSMR(Modal);
                    if (resp != null && resp.result == AppStatic.SUCCESS)
                    {
                        Modal.IsSynced = true;
                        bool res = _laHelper.SaveSMRFormDataInLocalDB(Modal, true);
                        if (res == true)
                            ViewBag.result = AppStatic.SUCCESS;
                        else
                            ViewBag.result = "Data submitted on server but not stored on local";
                    }
                    else
                    {
                        bool res = _laHelper.SaveSMRFormDataInLocalDB(Modal, true);
                        if (res == false)
                            ViewBag.result = AppStatic.ERROR;
                        else
                            ViewBag.result = AppStatic.SUCCESS;
                    }
                }
                else
                {
                    bool res = _laHelper.SaveSMRFormDataInLocalDB(Modal, true);
                    if (res == false)
                        ViewBag.result = AppStatic.ERROR;
                    else
                        ViewBag.result = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return View();
        }
        #endregion

        #region Arrival Report
        [AuthorizeFilter]
        public ActionResult ArrivalReport()
        {
            if (TempData["Result"] != null)
            {
                ViewBag.Result = Utility.ToString(TempData["Result"]);
            }
            SettingsHelper _helper = new SettingsHelper();
            SMTPServerModal SMTP = _helper.GetSMTPSettingsJson();
            if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
            {
                ViewBag.CCEmail = string.Join(";", SMTP.CCEmail);
            }

            return View();
        }
        #endregion

        #region Departure Report
        [AuthorizeFilter]
        public ActionResult DepartureReport()
        {
            if (TempData["Result"] != null)
            {
                ViewBag.Result = Utility.ToString(TempData["Result"]);
            }
            SettingsHelper _helper = new SettingsHelper();
            SMTPServerModal SMTP = _helper.GetSMTPSettingsJson();
            if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
            {
                ViewBag.CCEmail = string.Join(";", SMTP.CCEmail);
            }
            return View();
        }
        #endregion

        #region Internal Audit Form
        [AuthorizeFilter]
        public ActionResult InternalAuditForm()
        {
            try
            {
                ViewBag.IsLocalRequest = Utility.ToString(Request.IsLocal);
                ViewBag.Path = JsonConvert.SerializeObject(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\Repository\Forms\Technical and Safety\IAF - Internal Audit Form v2.xsn");
                string folderPath = string.Format(@"{0}IAF", Convert.ToString(ConfigurationManager.AppSettings["SavedFormsPath"]));
                ViewBag.FolderPath = JsonConvert.SerializeObject(folderPath);
                //folderPath
                DirectoryInfo d = new DirectoryInfo(folderPath);//Assuming Test is your Folder
                if (!d.Exists)
                    d.Create();
                List<FormModal> FormList = new List<FormModal>();
                FileInfo[] Files = d.GetFiles(); //Getting Text files
                foreach (FileInfo file in Files)
                {
                    FormList.Add(new FormModal
                    {
                        Title = file.Name,
                        TemplatePath = JsonConvert.SerializeObject(file.FullName)
                    });
                }
                DirectoryInfo[] getfolders = d.GetDirectories();
                foreach (DirectoryInfo folderName in getfolders)
                {
                    FormList.Add(new FormModal
                    {
                        Title = folderName.Name,
                        TemplatePath = JsonConvert.SerializeObject(folderName.FullName),
                        IsFolder = true
                    });
                    FileInfo[] folderSubFiles = folderName.GetFiles();
                    foreach (FileInfo file in folderSubFiles)
                    {
                        FormList.Add(new FormModal
                        {
                            Title = file.Name,
                            TemplatePath = JsonConvert.SerializeObject(file.FullName)
                        });
                    }
                }
                ViewBag.FormList = FormList;
                if (FormList != null && FormList.Count > 0)
                    ViewBag.OpenFolderClass = "";
                else
                    ViewBag.OpenFolderClass = "disabled";
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InternalAuditForm Error:--" + ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetAuditNote(int id)
        {
            IAF response = new IAF();
            bool isInternetAvailable = Utility.CheckInternet();
            LogHelper.writelog("Internet Available : " + isInternetAvailable.ToString());
            if (id != 0)
            {
                if (isInternetAvailable)
                {
                    APIHelper _helper = new APIHelper();
                    response = _helper.IAFormDetailsView(id);
                }
                else
                {
                    IAFHelper _laHelper = new IAFHelper();
                    response = _laHelper.IAFormDetailsView_Local_DB(id.ToString());

                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAuditNotesById(Guid? id)
        {
            AuditNote response = new AuditNote();
            if (id != null)
            {
                IAFHelper _laHelper = new IAFHelper();
                response = _laHelper.GetAuditNotesById_Local_DB(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Daily Cargo Report
        [AuthorizeFilter]
        public ActionResult DailyCargoReport()
        {
            if (TempData["Result"] != null)
            {
                ViewBag.Result = Utility.ToString(TempData["Result"]);
            }
            SettingsHelper _helper = new SettingsHelper();
            SMTPServerModal SMTP = _helper.GetSMTPSettingsJson();
            if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
            {
                ViewBag.CCEmail = string.Join(";", SMTP.CCEmail);
            }
            return View();
        }
        #endregion

        #region Daily Position Report
        [AuthorizeFilter]
        public ActionResult DailyPositionReport()
        {
            if (TempData["Result"] != null)
            {
                ViewBag.Result = Utility.ToString(TempData["Result"]);
            }
            SettingsHelper _helper = new SettingsHelper();
            SMTPServerModal SMTP = _helper.GetSMTPSettingsJson();
            if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
            {
                ViewBag.CCEmail = string.Join(";", SMTP.CCEmail);
            }
            return View();
        }
        #endregion

        #region General Inspection Report
        [AuthorizeFilter]
        public ActionResult GeneralInspectionReport()
        {
            GeneralInspectionReport Modal = new GeneralInspectionReport();
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            LoadShipsList();

            Modal.GIRPhotographs = new List<GIRPhotographs>();  // RDBJ 02/07/2022

            //RDBJ 10/06/2021
            GIRTableHelper _laHelper = new GIRTableHelper();
            Modal.Ship = SessionManager.ShipCode; //10/07/2021
            Modal.CSShipsModal = _laHelper.GIRGetGeneralDescription(SessionManager.ShipCode);
            //End RDBJ 10/06/2021

            ViewBag.ClassificationSociety = bindDropDown(ClassificationSociety, Convert.ToInt32(Modal.CSShipsModal.ClassificationSocietyId)); // RDBJ 01/11/2022
            ViewBag.FlagState = bindDropDown(FlagState, Convert.ToInt32(Modal.CSShipsModal.FlagStateId)); // RDBJ 01/11/2022
            ViewBag.PortOfRegistry = bindDropDown(PortOfRegistry, Convert.ToInt32(Modal.CSShipsModal.PortOfRegistryId)); // RDBJ 01/11/2022

            return View(Modal);
        }
        [HttpPost]
        public ActionResult GeneralInspectionReport(GeneralInspectionReport Modal)
        {
            APIHelper _aHelper = new APIHelper();
            GIRTableHelper _laHelper = new GIRTableHelper();
            SettingsHelper _helper = new SettingsHelper();
            TempData["data"] = Modal;
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    SetGIRModal(ref Modal);
                    Modal.SavedAsDraft = false;
                    Modal.IsSynced = false;
                    Modal.isDelete = 0; // RDBJ 01/05/2022
                    Modal.FormVersion = Modal.FormVersion + Convert.ToDecimal(0.01); // JSL 05/01/2022

                    bool res = _laHelper.SaveGIRDataInLocalDB(Modal, Modal.IsSynced.Value);
                    if (res == false)
                        ViewBag.result = AppStatic.ERROR;
                    else
                        ViewBag.result = AppStatic.SUCCESS;
                    List<CSShipsModal> shipsList = _helper.GetAllShipsFromJson();
                    TempData["Name"] = shipsList.Where(x => x.Code == Modal.Ship).Select(x => x.Name).FirstOrDefault();
                    return RedirectToAction("SendGIReportMail", "Mail", new
                    {
                        Modal = TempData["data"]
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit GIR Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("Index", "Deficiencies");
        }
        [AuthorizeFilter] //RDBJ 10/17/2021
        public JsonResult GIRAutoSave(GeneralInspectionReport Modal)
        {
            long resp = 0;
            string UniqueFormID = string.Empty;
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    SetGIRModal(ref Modal);
                    GIRTableHelper _helper = new GIRTableHelper();
                    if (Modal.FormVersion == 0)
                    {
                        Modal.FormVersion = 1;
                    }
                    else
                    {
                        Modal.FormVersion = Modal.FormVersion + Convert.ToDecimal(0.01);
                    }
                    Modal.isDelete = 0; // RDBJ 01/05/2022
                    UniqueFormID = _helper.AutoSaveGIRDataInLocalDB(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIR AutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(new { OffGIRFormId = resp, UniqueFormID = UniqueFormID, FormVersion = Modal.FormVersion });
        }

        //RDBJ 10/06/2021
        public JsonResult GIRShipGeneralDescriptionSave(GeneralInspectionReport Modal)
        {
            bool res = false;
            try
            {
                if (Modal.CSShipsModal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.CSShipsModal.Code))
                        Modal.CSShipsModal.Code = Modal.Ship;

                    GIRTableHelper _helper = new GIRTableHelper();
                    res = _helper.GIRShipGeneralDescriptionSave(Modal.CSShipsModal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRShipGeneralDescriptionSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/06/2021

        [HttpPost]
        public ActionResult GIRSaveFormDarf(GeneralInspectionReport Modal)
        {
            GIRTableHelper _laHelper = new GIRTableHelper();
            SettingsHelper _helper = new SettingsHelper(); //RDBJ 11/10/2021
            TempData["data"] = Modal; //RDBJ 11/10/2021
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    Modal.ShipName = SessionManager.ShipName;
                    Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    bool res;

                    Modal.FormVersion = Modal.FormVersion + Convert.ToDecimal(0.01); // JSL 05/01/2022

                    res = _laHelper.SaveDraftDataInLocalDB(Modal);
                    if (res == true)
                        ViewBag.result = AppStatic.SUCCESS;
                    else
                        ViewBag.result = "Error occured while saving data in local";

                    //RDBJ 11/10/2021
                    List<CSShipsModal> shipsList = _helper.GetAllShipsFromJson();
                    TempData["Name"] = shipsList.Where(x => x.Code == Modal.Ship).Select(x => x.Name).FirstOrDefault();
                    return RedirectToAction("SendGIReportMail", "Mail", new
                    {
                        Modal = TempData["data"]
                    });
                    //End RDBJ 11/10/2021
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIR Save Form Darf Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("Index", "Deficiencies");
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
        [HttpPost]
        public ActionResult getNextNumber(string ship, string ReportType, string UniqueFormID) //RDBJ 10/30/2021 Added ReportType
        {
            GIRTableHelper _helper = new GIRTableHelper();
            var no = _helper.DeficienciesNumber(ship, ReportType, UniqueFormID); //RDBJ 10/30/2021 Added ReportType //RDBJ 09/18/2021 Removed parameters Guid.Empty,""
            return Json(no, JsonRequestBehavior.AllowGet);
        }

        public List<CSShipsModal> LoadShipsList()
        {
            //APIHelper _helper = new APIHelper(); //RDBJS 09/17/2021 Commented

            SettingsHelper _lhelper = new SettingsHelper();
            GIRTableHelper _laHelper = new GIRTableHelper();
            List<CSShipsModal> shipsList = null;

            //RDBJS 09/17/2021 Commented
            /*
            bool isInternetAvailable = Utility.CheckInternet();
            if (isInternetAvailable)
            {
                shipsList = _helper.GetAllShips();
                shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
                shipsList = shipsList.OrderBy(x => x.Name).ToList();
            }
            */

            shipsList = _laHelper.GetAllShips();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").DistinctBy(x => x.Code).ToList(); //RDBJ 10/07/2021 set Distinct
            shipsList = shipsList.OrderBy(x => x.Name).ToList();

            if (shipsList == null || shipsList.Count <= 0)
                shipsList = _lhelper.GetAllShipsFromJson();

            ViewBag.ships = new SelectList(shipsList, "Code", "Name");
            return shipsList;
        }
        public JsonResult AddGIRDeficiencies(GIRDeficiencies Modal) // RDBJ 02/20/2022 set with JsonResult from Void
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>(); // RDBJ 02/20/2022
            try
            {
                GIRTableHelper _helper = new GIRTableHelper();
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    Modal.IsClose = false;
                    Modal.IsSynced = false;

                    if(Modal.No == 0)
                        Modal.No = _helper.DeficienciesNumber(Modal.Ship, Modal.ReportType, Convert.ToString(Modal.UniqueFormID)); //RDBJ 10/30/2021 Added ReportType //RDBJ 09/18/2021 Removed Modal.UniqueFormID,Modal.ItemNo
                    
                    //var response = _helper.AddGIRDeficienciesInLocalDB(Modal);  // RDBJ 02/20/222 commented this line
                    retDictMetaData = _helper.AddGIRDeficienciesInLocalDB(Modal); // RDBJ 02/20/222
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficiencies Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(retDictMetaData, JsonRequestBehavior.AllowGet); // RDBJ 02/20/2022
        }

        // JSL 11/12/2022
        public JsonResult AddGIRDeficienciesWithFile()
        {
            GIRDeficiencies Modal = new GIRDeficiencies();  
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>(); 
            try
            {
                GIRTableHelper _helper = new GIRTableHelper();

                NameValueCollection nvcForm = Request.Form;
                if (nvcForm != null && nvcForm.Count > 0)
                {
                    Modal.DeficienciesUniqueID = Guid.NewGuid();
                    Modal.GIRFormID = 0;
                    Modal.UniqueFormID = Guid.Parse(nvcForm["UniqueFormID"]);
                    Modal.No = Convert.ToInt32(nvcForm["No"]);
                    Modal.Section = nvcForm["Section"];
                    Modal.DateRaised = Convert.ToDateTime(nvcForm["DateRaised"]);
                    Modal.Deficiency = nvcForm["Deficiency"];

                    if (!string.IsNullOrEmpty(nvcForm["DateClosed"]))
                        Modal.DateClosed = Convert.ToDateTime(nvcForm["DateClosed"]);
                    
                    Modal.ReportType = nvcForm["ReportType"];
                    Modal.Ship = nvcForm["Ship"];
                    Modal.Priority = Convert.ToInt32(nvcForm["Priority"]);
                    Modal.Port = nvcForm["Port"];
                    Modal.GIRDeficienciesFile = new List<GIRDeficienciesFile>();
                }

                if (Request.Files.Count > 0)
                {
                    try
                    {
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                            dictMetaData["UniqueFormID"] = Convert.ToString(Modal.UniqueFormID);
                            dictMetaData["ReportType"] = Modal.ReportType;

                            Dictionary<string, string> dictFileData = Assets.UploadFileAndReturnPath(files[i], Convert.ToString(Modal.DeficienciesUniqueID), dictDefData: dictMetaData);
                            if (dictFileData != null && dictFileData.Count > 0)
                            {
                                string strFileName = dictFileData["FileName"];
                                string strAttachmentData = dictFileData["StorePath"];
                                GIRDeficienciesFile fileData = new GIRDeficienciesFile();
                                fileData.DeficienciesID = 0;
                                fileData.FileName = strFileName;
                                fileData.StorePath = strAttachmentData;
                                fileData.IsUpload = "true";

                                Modal.GIRDeficienciesFile.Add(fileData);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    Modal.IsClose = false;
                    Modal.IsSynced = false;

                    if (Modal.No == 0)
                        Modal.No = _helper.DeficienciesNumber(Modal.Ship, Modal.ReportType, Convert.ToString(Modal.UniqueFormID));

                    retDictMetaData = _helper.AddGIRDeficienciesInLocalDB(Modal); 
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficiencies Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(retDictMetaData, JsonRequestBehavior.AllowGet);
        }
        // End JSL 11/12/2022

        [HttpPost]
        public ActionResult GIRDetails(string code)
        {
            List<GIRDataList> Modal = new List<GIRDataList>();
            if (string.IsNullOrWhiteSpace(code))
                code = SessionManager.ShipCode;
            bool isInternetAvailable = Utility.CheckInternet();
            if (isInternetAvailable)
            {
                APIHelper _apihelper = new APIHelper();
                Modal = _apihelper.GetDeficienciesData(code);
            }
            else
            {
                GIRTableHelper _helper = new GIRTableHelper();
                Modal = _helper.GetDeficienciesDataInLocalDB(code);
            }
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeFilter]
        [HttpPost]
        public ActionResult GIRDeficienciesUpload(FormCollection formCollection)
        {
            Guid uniqueFormId = new Guid(formCollection["uniqueFormData"]);
            string ShipName = formCollection["ShipName"];
            if (Request.Files.Count != 1 || Request.Files[0].ContentLength == 0)
            {
                ModelState.AddModelError("uploadError", "File's length is zero, or no files found");
                return RedirectToAction("RiskAssessmentForm");
            }
            RiskAssessmentFormModal Modal = new RiskAssessmentFormModal();
            var hpf = this.Request.Files[0];
            if (hpf.ContentLength == 0)
            {
                return RedirectToAction("RiskAssessmentForm");
            }
            // check the file size (max 4 Mb)

            if (hpf.ContentLength > 1024 * 1024 * 4)
            {
                ModelState.AddModelError("uploadError", "File size can't exceed 4 MB");
                return RedirectToAction("RiskAssessmentForm");
            }
            // extract only the filename
            var fileName = Path.GetFileName(Request.Files[0].FileName);

            // store the file inside ~/App_Data/uploads folder
            //var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);

            string path = Server.MapPath("~/UploadFile/");
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            //postedFile.SaveAs(path + Path.GetFileName(postedFile.FileName));

            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                else
                {

                    bool exists = System.IO.Directory.Exists(path);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(path);
                    //Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath("~/App_Data")));
                    hpf.SaveAs(path + fileName);
                }
                GIRTableHelper _helper = new GIRTableHelper();
                Modal = _helper.GIRDeficienciesUpload(path + fileName, uniqueFormId, ShipName);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("uploadError", "Can't save file to disk");
            }
            finally
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            return RiskAssessmentForm(Modal);
        }

        //RDBJ 09/17/2021
        [HttpPost]
        public ActionResult ShipDetails(string shipCode)
        {
            GIRTableHelper _laHelper = new GIRTableHelper();
            CSShipsModal shipsList = _laHelper.GetCSShipDetails(shipCode);
            return Json(shipsList, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 09/17/2021

        // RDBJ2 03/12/2022
        [HttpPost]
        public JsonResult UploadGIRPhotosOrDefFiles()
        {
            GIRTableHelper _helper = new GIRTableHelper();
            var dictMetadata = new Dictionary<string, string>();
            bool IsUploadGIRDefFiles = Convert.ToBoolean(Convert.ToString(Request.Form["IsUploadGIRDefFiles"]));   

            string straction = string.Empty;   

            if (IsUploadGIRDefFiles)
                straction = AppStatic.API_UPLOADGISIDEFICIENCIESFILEORPHOTO;
            else
                straction = AppStatic.API_UPLOADGIRPHOTOGRAPHS;

            Dictionary<string, string> dictToReturn = new Dictionary<string, string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        // JSL 11/12/2022
                        string strUniqueFormID = Convert.ToString(Request.Form["UniqueFormID"]);
                        string strDeficienciesUniqueID = Convert.ToString(Request.Form["DeficienciesUniqueID"]);
                        string strFileName = string.Empty;
                        string strAttachmentData = string.Empty;

                        Dictionary<string, string> dictFileData = new Dictionary<string, string>();
                        if (IsUploadGIRDefFiles)
                            dictFileData = Assets.UploadFileAndReturnPath(files[i], strDeficienciesUniqueID);
                        else
                            dictFileData = Assets.UploadFileAndReturnPath(files[i], strUniqueFormID, blnIsGIRPhotos: true);

                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            strFileName = dictFileData["FileName"];
                            strAttachmentData = dictFileData["StorePath"];
                        }
                        // End JSL 11/12/2022

                        // JSL 11/12/2022 commented
                        /*
                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        string strContentType = file.ContentType;
                        string FileName = Path.GetFileName(file.FileName);
                        byte[] PictureAsBytes = new byte[file.ContentLength];
                        using (BinaryReader theReader = new BinaryReader(file.InputStream))
                        {
                            PictureAsBytes = theReader.ReadBytes(file.ContentLength);
                        }
                        string ImagePath = "data:" + strContentType + ";base64," + Convert.ToBase64String(PictureAsBytes);
                        */
                        // End JSL 11/12/2022 commented

                        dictMetadata["strAction"] = straction;
                        
                        // JSL 11/12/2022 commented
                        /*
                        dictMetadata["FileName"] = FileName;
                        dictMetadata["ImagePath"] = ImagePath;
                        */
                        // End JSL 11/12/2022 commented
                        
                        // JSL 11/12/2022
                        dictMetadata["FileName"] = strFileName;
                        dictMetadata["ImagePath"] = strAttachmentData;
                        // JSL 11/12/2022
                        
                        dictMetadata["UniqueFormID"] = Convert.ToString(Request.Form["UniqueFormID"]);
                        dictMetadata["Ship"] = Convert.ToString(Request.Form["Ship"]);
                        dictMetadata["DeficienciesUniqueID"] = Convert.ToString(Request.Form["DeficienciesUniqueID"]);
                        dictMetadata["DeficienciesFileUniqueID"] = Convert.ToString(Request.Form["DeficienciesFileUniqueID"]); // JSL 06/04/2022

                        try
                        {
                            dictToReturn = _helper.AjaxPostPerformAction(dictMetadata);
                            //dictToReturn["FileName"] = FileName;  // JSL 11/12/2022 commented
                            dictToReturn["FileName"] = strFileName;    // JSL 11/12/2022

                            if (!IsUploadGIRDefFiles)
                            {
                                //dictToReturn["ImagePath"] = ImagePath;  // JSL 11/12/2022 commented

                                // JSL 11/12/2022
                                string strFileBasePath = "../Images/";
                                dictToReturn["ImagePath"] = Path.Combine(strFileBasePath, strAttachmentData);  
                                // End JSL 11/12/2022
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("UploadGIRPhotosOrDefFiles : " + straction + " Error : " + ex.Message);
                        }
                    }
                    // Returns message that successfully uploaded  
                }
                catch (Exception ex)
                {
                    //return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                //return Json("No files selected.");
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ2 03/12/2022

        #endregion

        #region Superintended Inspection Report
        [AuthorizeFilter]
        public ActionResult SuperintendedInspectionReport()
        {
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            bool isInternetAvailable = Utility.CheckInternet();
            SIRModal Modal = new SIRModal();
            ViewBag.Inspector = ServerModal.IsInspector;
            LoadShipsList();
            return View(Modal);
        }
        [HttpPost]
        public ActionResult SuperintendedInspectionReport(SIRModal Modal)
        {
            ViewBag.data = Modal;
            TempData["data"] = Modal;
            APIHelper _aHelper = new APIHelper();
            SettingsHelper _helper = new SettingsHelper();
            SIRTableHelper _laHelper = new SIRTableHelper();
            try
            {
                if (Modal != null)
                {
                    var shiplist = LoadShipsList();
                    if (shiplist != null && shiplist.Count > 0)
                    {
                        var objShip = shiplist.Where(x => x.Code == Modal.SuperintendedInspectionReport.ShipName).FirstOrDefault();
                        if (objShip != null)
                        {
                            Modal.SuperintendedInspectionReport.ShipName = objShip.Code;
                            Modal.SuperintendedInspectionReport.ShipID = objShip.ShipId;
                        }
                    }
                    Modal.SuperintendedInspectionReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.SuperintendedInspectionReport.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                }
                Modal.SuperintendedInspectionReport.SavedAsDraft = false;
                
                Modal.SuperintendedInspectionReport.IsSynced = false;
                Modal.SuperintendedInspectionReport.isDelete = 0; // RDBJ 01/05/2022

                Modal.SuperintendedInspectionReport.FormVersion = Modal.SuperintendedInspectionReport.FormVersion + Convert.ToDecimal(0.01);    // JSL 05/01/2022

                bool res = _laHelper.SaveSIRDataInLocalDB(Modal, true);
                if (res == false)
                    ViewBag.result = AppStatic.ERROR;
                else
                    ViewBag.result = AppStatic.SUCCESS;
                List<CSShipsModal> shipsList = _helper.GetAllShipsFromJson();
                TempData["Name"] = shipsList.Where(x => x.Code == Modal.SuperintendedInspectionReport.ShipName).Select(x => x.Name).FirstOrDefault();
                return RedirectToAction("SendSIReportMail", "Mail", new
                {
                    Modal = TempData["data"]
                });
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SIR Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("SuperintendedInspectionReport");
        }
        public JsonResult SIRAutoSave(SIRModal Modal)
        {
            long resp = 0;
            long officeSIRId = 0;
            string UniqueFormID = string.Empty;
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.SuperintendedInspectionReport.ShipName))
                        Modal.SuperintendedInspectionReport.ShipName = SessionManager.ShipCode;
                    Modal.SuperintendedInspectionReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    //Modal.SuperintendedInspectionReport.SavedAsDraft = true;
                    Modal.SuperintendedInspectionReport.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime

                    bool? check = Modal.SuperintendedInspectionReport.SavedAsDraft;
                    
                    SIRTableHelper _helper = new SIRTableHelper();
                    if (Modal.SuperintendedInspectionReport.FormVersion == 0)
                    {
                        Modal.SuperintendedInspectionReport.FormVersion = 1;
                    }
                    else
                    {
                        Modal.SuperintendedInspectionReport.FormVersion = Modal.SuperintendedInspectionReport.FormVersion + Convert.ToDecimal(0.01);
                    }
                    Modal.SuperintendedInspectionReport.isDelete = 0; // RDBJ 01/05/2022
                    UniqueFormID = _helper.AutoSaveSIRDataInLocalDB(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIR AutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(new { OfficeSIRFormID = officeSIRId, UniqueFormID = UniqueFormID, FormVersion = Modal.SuperintendedInspectionReport.FormVersion, SIRFormID = resp });
        }

        //RDBJ 10/11/2021 fixed _SIR_DeficienciesSection
        public PartialViewResult _SIR_DeficienciesSection(string id)
        {
            DraftsHelper _dHelper = new DraftsHelper();
            SIRModal Modal = new SIRModal();
            Modal = _dHelper.SIRFormGetDeficiency_Local_DB(id);
            if (Modal.SuperintendedInspectionReport.SavedAsDraft == null)
            {
                Modal.SuperintendedInspectionReport.SavedAsDraft = true;
            }
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            ViewBag.SavedAsDraft = Modal.SuperintendedInspectionReport.SavedAsDraft;  // RDBJ 03/21/2022
            return PartialView("SIR/_SIR_DeficienciesSection", Modal);
        }
        #endregion

        #region RWH Forms
        [AuthorizeFilter]
        public ActionResult RestAndWorkHoursForm()
        {
            try
            {
                ViewBag.IsLocalRequest = Utility.ToString(Request.IsLocal);
                ViewBag.Path = JsonConvert.SerializeObject(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\Repository\Forms\Technical and Safety\RWH - Record of Hours of Work and Rest.xltm");
                string folderPath = string.Format(@"{0}RWH", Convert.ToString(ConfigurationManager.AppSettings["SavedFormsPath"])); //@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\Repository\Saved Forms\RWH";
                ViewBag.FolderPath = JsonConvert.SerializeObject(folderPath);
                //folderPath
                DirectoryInfo d = new DirectoryInfo(folderPath);//Assuming Test is your Folder
                if (!d.Exists)
                    d.Create();
                List<FormModal> FormList = new List<FormModal>();
                FileInfo[] Files = d.GetFiles(); //Getting Text files
                foreach (FileInfo file in Files)
                {
                    FormList.Add(new FormModal
                    {
                        Title = file.Name,
                        TemplatePath = JsonConvert.SerializeObject(file.FullName)
                    });
                }
                ViewBag.FormList = FormList;
                if (FormList != null && FormList.Count > 0)
                    ViewBag.OpenFolderClass = "";
                else
                    ViewBag.OpenFolderClass = "disabled";
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RestAndWorkHoursForm Error : " + ex.Message);
            }
            return View();
        }
        #endregion

        #region HoldVentilationRecord
        [AuthorizeFilter]
        public ActionResult HoldVentilationRecord()
        {
            LoadShipsList();
            return View();
        }
        [HttpPost]
        public ActionResult HoldVentilationRecord(HoldVentilationRecordFormModal Modal)
        {
            APIHelper _aHelper = new APIHelper();
            HoldVentilationRecordHelper _laHelper = new HoldVentilationRecordHelper();
            try
            {
                if (Modal != null)
                {
                    Modal.HoldVentilationRecordForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    var finlaList = new List<HoldVentilationRecordSheetModal>();
                    if (Modal.HoldVentilationRecordList != null && Modal.HoldVentilationRecordList.Count > 0)
                    {
                        foreach (var item in Modal.HoldVentilationRecordList)
                        {
                            var obj = item.GetType()
                                        .GetProperties() //get all properties on object
                                        .Where(pi => pi.Name != "IsVentilation")
                                        .Select(pi => pi.GetValue(item)) //get value for the propery
                                        .Any(value => value != null); // Check if one of the values is not null, if so it returns true
                            if (obj == true)
                            {
                                item.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                item.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                finlaList.Add(item);
                            }
                        }
                    }
                }
                bool isInternetAvailable = Utility.CheckInternet();
                LogHelper.writelog("Internet Available : " + isInternetAvailable.ToString());
                if (isInternetAvailable)
                {
                    APIResponse resp = _aHelper.SubmitHoldVentilationRecordForm(Modal);
                    if (resp != null && resp.result == AppStatic.SUCCESS)
                    {
                        Modal.HoldVentilationRecordForm.IsSynced = true;
                        bool res = _laHelper.SaveHVRFormDataInLocalDB(Modal, true);
                        if (res == true)
                            ViewBag.result = AppStatic.SUCCESS;
                        else
                            ViewBag.result = "Data submitted on server but not stored on local";
                    }
                    else
                    {
                        bool res = _laHelper.SaveHVRFormDataInLocalDB(Modal, true);
                        if (res == false)
                            ViewBag.result = AppStatic.ERROR;
                        else
                            ViewBag.result = AppStatic.SUCCESS;
                    }
                }
                else
                {
                    bool res = _laHelper.SaveHVRFormDataInLocalDB(Modal, true);
                    if (res == false)
                        ViewBag.result = AppStatic.ERROR;
                    else
                        ViewBag.result = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit HoldVentilationRecord Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            ModelState.Clear();
            return View();
        }
        #endregion

        #region Feedback
        [AuthorizeFilter]
        public ActionResult FeedbackForm()
        {
            if (TempData["Result"] != null)
            {
                ViewBag.Result = Utility.ToString(TempData["Result"]);
            }
            return View();
        }
        #endregion

        #region Risk Assessment Form
        [AuthorizeFilter]
        public ActionResult RiskAssessmentFormList()
        {
            return View();
        }

        public ActionResult GetRiskAssessmentFormList(string ship)
        {
            APIHelper _helper = new APIHelper();
            List<RiskAssessmentForm> Modal = new List<RiskAssessmentForm>();
            bool isInternetAvailable = Utility.CheckInternet();
            if (isInternetAvailable)
            {
                Modal = _helper.GetRiskassessmentForm(ship);
            }
            else
            {
                RiskAssessmentHelper _dHelper = new RiskAssessmentHelper();
                Modal = _dHelper.GET_RiskAssessment_Local_DB(ship);
            }
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeFilter]
        //public ActionResult RAformData(int? id = 0, bool isDefectSection = false) // JSL 11/26/2022 commented
        public ActionResult RAformData(string id, string shipCode = "", string docid = "")    // JSL 11/26/2022
        {
            LoadShipsList();
            ViewBag.result = Utility.ToString(TempData["RAFRes"]);
            ViewBag.CurrentDateString = Utility.GetCurrentDateShortString();
            APIHelper _helper = new APIHelper();
            RiskAssessmentFormModal Modal = new RiskAssessmentFormModal();
            // Get Data From Local
            RiskAssessmentHelper _dHelper = new RiskAssessmentHelper();

            // JSL 11/26/2022
            Guid guidOutput;
            bool isIdGuid = Guid.TryParse(id, out guidOutput);
            // End JSL 11/26/2022

            //if (id > 0)  // JSL 11/26/2022 commented this line
            if (isIdGuid) // JSL 11/26/2022
            {
                //Modal = _dHelper.RAFormDetailsView_Local_DB(SessionManager.ShipName, Convert.ToInt32(id));  // JSL 06/22/2022 commented this line
                //Modal = _dHelper.RAFormDetailsView_Local_DB(SessionManager.ShipCode, Convert.ToInt32(id));  // JSL 11/26/2022 commented  // JSL 06/22/2022
                Modal = _dHelper.RAFormDetailsView_Local_DB(SessionManager.ShipCode, Convert.ToString(id));  // JSL 11/26/2022  // JSL 06/22/2022
                bool isInternetAvailable = Utility.CheckInternet();

                if (Modal.RiskAssessmentForm == null)
                {
                    // Get Data Via API
                    if (isInternetAvailable)
                    {
                        Modal = _helper.RAFormDetailsView(SessionManager.ShipName, Convert.ToInt32(id));
                    }
                }
            }
            if (Modal.RiskAssessmentForm != null)
                Modal.RiskAssessmentForm.IsApplicable = Modal.RiskAssessmentForm.IsApplicable.HasValue && Modal.RiskAssessmentForm.IsApplicable.Value == false ? false : true;

            if (Modal.RiskAssessmentFormHazardList != null)
                Modal.RiskAssessmentFormHazardList = Modal.RiskAssessmentFormHazardList.OrderBy(x => x.HazardId).ToList();
            return View(Modal);
        }

        [AuthorizeFilter]
        public ActionResult RiskAssessmentForm()
        {
            ViewBag.result = Utility.ToString(TempData["RAFRes"]);
            ViewBag.CurrentDateString = Utility.GetCurrentDateShortString();
            return View();
        }

        [AuthorizeFilter]
        [HttpPost]
        public ActionResult RiskAssessmentForm(RiskAssessmentFormModal Modal)
        {
            APIHelper _aHelper = new APIHelper();
            RiskAssessmentHelper _raHelper = new RiskAssessmentHelper();
            try
            {
                if (Modal != null)
                {
                    // JSL 11/26/2022
                    if (Modal.RiskAssessmentForm.RAFUniqueID == null || Modal.RiskAssessmentForm.RAFUniqueID == Guid.Empty)
                    {
                        Modal.RiskAssessmentForm.IsSynced = false;
                        Modal.RiskAssessmentForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.RiskAssessmentForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.RiskAssessmentForm.IsSynced = false;
                        Modal.RiskAssessmentForm.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.RiskAssessmentForm.UpdatedBy = SessionManager.Username;
                    }
                    // End JSL 11/26/2022
                    
                    if (Modal.RiskAssessmentForm.IsAmended.HasValue && Modal.RiskAssessmentForm.IsAmended.Value)
                        Modal.RiskAssessmentForm.AmendmentDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    //else Modal.RiskAssessmentForm.AmendmentDate = null;
                    var finlaList = new List<RiskAssessmentFormHazard>();
                    if (Modal.RiskAssessmentFormHazardList != null && Modal.RiskAssessmentFormHazardList.Count > 0)
                    {
                        foreach (var item in Modal.RiskAssessmentFormHazardList)
                        {
                            var obj = item.GetType()
                                        .GetProperties() //get all properties on object
                                        .Where(pi => pi.Name != "IsVentilation")
                                        .Select(pi => pi.GetValue(item)) //get value for the propery
                                        .Any(value => value != null); // Check if one of the values is not null, if so it returns true
                            if (obj == true)
                            {
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(item.Stage2RiskFactor))
                                        item.Stage2RiskFactor = Convert.ToString(Utility.ToEnum<RiskFactorType>(item.Stage2RiskFactor.Replace(" ", "")).GetHashCode());
                                }
                                catch (Exception)
                                { }
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(item.Stage4RiskFactor))
                                        item.Stage4RiskFactor = Convert.ToString(Utility.ToEnum<RiskFactorType>(item.Stage4RiskFactor.Replace(" ", "")).GetHashCode());
                                }
                                catch (Exception)
                                { }
                                finlaList.Add(item);
                            }
                        }
                    }

                    var ReviewerList = new List<RiskAssessmentFormReviewer>();
                    if (Modal.RiskAssessmentFormReviewerList != null && Modal.RiskAssessmentFormReviewerList.Count > 0)
                    {
                        foreach (var item in Modal.RiskAssessmentFormReviewerList)
                        {
                            var obj = item.GetType()
                                        .GetProperties() //get all properties on object
                                        .Where(pi => pi.Name != "IsVentilation")
                                        .Select(pi => pi.GetValue(item)) //get value for the propery
                                        .Any(value => value != null); // Check if one of the values is not null, if so it returns true
                            if (obj == true)
                            {
                                ReviewerList.Add(item);
                            }
                        }
                    }

                }
                //bool isInternetAvailable = Utility.CheckInternet();
                //LogHelper.writelog("Internet Available : " + isInternetAvailable.ToString());
                //if (isInternetAvailable)
                //{
                //    APIResponse resp = _aHelper.SubmitRiskAssessmentForm(Modal);
                //    if (resp != null && resp.result == AppStatic.SUCCESS)
                //    {
                //        Modal.RiskAssessmentForm.IsSynced = true;
                //        bool res = _raHelper.SaveRAFormDataInLocalDB(Modal, true);
                //        if (res == true)
                //            ViewBag.result = AppStatic.SUCCESS;
                //        else
                //            ViewBag.result = "Data submitted on server but not stored on local";
                //    }
                //    else
                //    {
                //        bool res = _raHelper.SaveRAFormDataInLocalDB(Modal, true);
                //        if (res == false)
                //            ViewBag.result = AppStatic.ERROR;
                //        else
                //            ViewBag.result = AppStatic.SUCCESS;
                //    }
                //}
                //else
                //{
                bool res = _raHelper.SaveRAFormDataInLocalDB(Modal, true);
                if (res == false)
                {
                    ViewBag.result = AppStatic.ERROR;
                    TempData["RAFRes"] = AppStatic.ERROR;
                }
                else
                {
                    ViewBag.result = AppStatic.SUCCESS;
                    TempData["RAFRes"] = AppStatic.SUCCESS;
                }
                // }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit RiskAssessment Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
                TempData["RAFRes"] = AppStatic.ERROR;
            }
            TempData.Keep();
            if (Modal.RiskAssessmentForm.RAFID != 0)
            {
                //return RedirectToAction("RAformData", new { id = Modal.RiskAssessmentForm.RAFID }); // JSL 11/26/2022 commented
                return RedirectToAction("RAformData", new { id = Modal.RiskAssessmentForm.RAFUniqueID });   // JSL 11/26/2022
            }
            else
            {
                return RedirectToAction("RiskAssessmentForm");
            }
        }

        [AuthorizeFilter]
        [HttpPost]
        public ActionResult RiskAssessmentFormUpload()
        {
            if (Request.Files.Count != 1 || Request.Files[0].ContentLength == 0)
            {
                ModelState.AddModelError("uploadError", "File's length is zero, or no files found");
                return RedirectToAction("RiskAssessmentForm");
            }
            RiskAssessmentFormModal Modal = new RiskAssessmentFormModal();
            var hpf = this.Request.Files[0];
            if (hpf.ContentLength == 0)
            {
                return RedirectToAction("RiskAssessmentForm");
            }
            // check the file size (max 4 Mb)

            if (hpf.ContentLength > 1024 * 1024 * 4)
            {
                ModelState.AddModelError("uploadError", "File size can't exceed 4 MB");
                return RedirectToAction("RiskAssessmentForm");
            }
            // extract only the filename
            var fileName = Path.GetFileName(Request.Files[0].FileName);

            // store the file inside ~/App_Data/uploads folder
            var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
            try
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                else
                    Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath("~/App_Data")));
                hpf.SaveAs(path);
                DocumentTableHelper _helper = new DocumentTableHelper();
                Modal = _helper.GetDocumentsRiskAssesmentFileData(path);
            }
            catch (Exception)
            {
                ModelState.AddModelError("uploadError", "Can't save file to disk");
            }
            finally
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            return RiskAssessmentForm(Modal);
        }

        public string CheckRAFNumberExist(string RAFNumber)
        {
            string result = string.Empty;
            try
            {
                RiskAssessmentHelper _raHelper = new RiskAssessmentHelper();
                if (_raHelper.CheckRAFNumberExistFromLocalDB(RAFNumber))
                    result = "Number already exist!";
                else
                    result = "";
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckRAFNumberExist Error : " + ex.Message);
            }
            return result;
        }

        // Delete Local RiskAssessment Data
        [AuthorizeFilter]
        [HttpGet]
        public JsonResult DeleteLocalRiskAssessmentData()
        {
            LogHelper.writelog("DeleteLocalRiskAssessmentData ");
            RiskAssessmentHelper _raHelper = new RiskAssessmentHelper();
            var isDeleted = _raHelper.DeleteLocalRiskAssessmentData(SessionManager.ShipCode);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region common function
        // RDBJ2 04/01/2022
        public JsonResult PerformAction(string jsonmetadata, string straction)
        {
            FormTableHelper _helper = new FormTableHelper();

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
                    LogHelper.writelog("Forms PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ2 04/01/2022

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