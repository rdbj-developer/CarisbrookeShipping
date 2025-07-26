using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeApplication.BLL.Modals;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.Models;
using System.IO;
using Newtonsoft.Json;
using static OfficeApplication.BLL.Modals.AppStatic;
using System.Xml;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.AspNet.SignalR.Hosting;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class FormsController : Controller
    {
        #region Forms
        // GET: Forms
        public ActionResult Index()
        {
            try
            {
                List<FormModal> FormList = new List<FormModal>();

                // Get Data Via API
                APIHelper _aHelper = new APIHelper();
                FormList = _aHelper.GetAllForms();

                if (FormList != null && FormList.Count > 0)
                {
                    //string path;
                    foreach (var item in FormList)
                    {
                        try
                        {
                            //if (item.Code == "RWHS")
                            //{
                            //    item.TemplatePath = JsonConvert.SerializeObject(item.TemplatePath);
                            //}
                            //else
                            //{
                            //path = item.TemplatePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                            //path = Server.MapPath(path);
                            item.TemplatePath = JsonConvert.SerializeObject(item.TemplatePath);
                            //}
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("Forms->Index " + ex.Message);
                        }
                    }
                    //FormList.ForEach(x => x.TemplatePath = JsonConvert.SerializeObject(x.TemplatePath));

                    FormList = FormList.Where(x => x.CanBeOpened.HasValue && x.CanBeOpened.Value).OrderBy(x => x.Title).ToList();
                }
                ViewBag.FormList = FormList;
                //try
                //{
                //    LogHelper.writelog("Forms->Index Start right"  + Server.MapPath("~/Repository/DocumentsIndex.xml"));
                //    RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                //    _helper.InsertDocumentsInRiskAssessmentData(Server.MapPath("~/Repository/DocumentsIndex.xml"), SessionManager.ShipCode, SessionManager.ShipName, SessionManager.Username);
                //}
                //catch (Exception e)
                //{
                //    LogHelper.writelog("Forms->Index : Error creating an entries" + e.Message);
                //}
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Forms->Index " + ex.Message);
            }
            return View();
        }
        public JsonResult GetAllForms()
        {
            APIHelper _helper = new APIHelper();
            List<FormModal> DocList = _helper.GetAllForms();
            if (DocList != null && DocList.Count > 0)
            {
                DocList = DocList.OrderBy(x => x.Category).ToList();
            }
            return Json(DocList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreatePDF(string path)
        {
            // DocumentTableHelper _helper = new DocumentTableHelper();
            // string path = string.Empty;
            string mimeType = string.Empty;
            mimeType = MimeMapping.GetMimeMapping(Path.GetFileName(path));
            try
            {
                if (mimeType != "PDF")
                {
                    string filename = Path.GetFileName(path);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    return File(fileBytes, "application/force-download", filename);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            if (!string.IsNullOrWhiteSpace(path))
                return File(path, mimeType);
            TempData["ErrorMsg"] = "Document is not exist.";
            TempData.Keep();
            return RedirectToAction("Index", "Documents");
        }
        #endregion

        #region SRM
        public ActionResult SMR()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SMR(SMRFormReq Modal)
        {
            APIHelper _helper = new APIHelper();
            List<SMRModal> ModalList = _helper.GetSMRFormsFilled(Modal);
            ViewBag.Searched = Modal;
            return View(ModalList);
        }
        public ActionResult SMRDetails(string id)
        {
            SMRModal Modal = new SMRModal();
            long SMRID = Utility.ToLong(id);
            if (SMRID > 0)
            {
                APIHelper _helper = new APIHelper();
                Modal = _helper.GetSMRFormByID(SMRID);
            }
            else
            {
                Modal = null;
            }
            return View(Modal);
        }
        #endregion

        #region GIR
        public ActionResult GeneralInspectionReport()
        {
            APIHelper _helper = new APIHelper();
            List<GIRData> ModalList = _helper.GetGIRFormByID();
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            ViewBag.ShipName = SessionManager.ShipName;
            ViewBag.UserGroup = Session["UserGroup"]; // RDBJ 01/05/2022

            ViewBag.AuditorsList = GetFormsPersonList(1);   // JSL 07/24/2022 1 is used for Auditors

            // JSL 02/14/2023
            ViewBag.Users = Helpers.Assets.GetAllUsersProfileList();
            // End JSL 02/14/2023

            return View(ModalList);
        }
        [HttpPost]
        public ActionResult GeneralInspectionReport(GeneralInspectionReport Modal)
        {
            APIHelper _aHelper = new APIHelper();
            try
            {
                if (Modal != null 
                    && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                    )
                {
                    Modal.FormVersion = Modal.FormVersion + Convert.ToDecimal(0.01);    // JSL 05/01/2022
                    //if (string.IsNullOrWhiteSpace(Modal.Ship))
                    //    Modal.Ship = SessionManager.ShipCode;
                    SetGIRModal(ref Modal);
                    APIResponse resp = _aHelper.SubmitGIR(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit GIR Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("GeneralInspectionReport");
        }
        [HttpPost]
        public ActionResult GeneralInspectionReportList()
        {
            APIHelper _helper = new APIHelper();
            List<GIRData> ModalList = _helper.GetGIRFormByID();
            return Json(ModalList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GIRDetails(string code)
        {
            List<GIRDataList> Modal = new List<GIRDataList>();
            if (code != "")
            {
                APIHelper _helper = new APIHelper();
                Modal = _helper.GetDeficienciesData(code);
            }
            else
            {
                Modal = null;
            }
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateDeficienciesData(string id, string isClose)
        {
            bool res = false;
            if (id != ""
                && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                )
            {
                APIHelper _helper = new APIHelper();
                res = _helper.UpdateDeficienciesData(id, Convert.ToBoolean(isClose));
                LogHelper.LogForDeficienciesClose(id, SessionManager.Username, isClose); // RDBJ 12/21/2021
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public JsonResult AddDeficienciesNote(DeficienciesNote notes) // JSL 12/03/2022 commented
        public JsonResult AddDeficienciesNote() // JSL 12/03/2022
        {
            APIHelper _helper = new APIHelper();
            long res = 0;
            // JSL 12/03/2022
            DeficienciesNote notes = new DeficienciesNote();
            NameValueCollection nvcForm = Request.Form;
            if (nvcForm != null && nvcForm.Count > 0)
            {
                notes.UserName = nvcForm["UserName"];
                notes.Comment = nvcForm["Comment"];
                notes.DeficienciesUniqueID = Guid.Parse(nvcForm["DeficienciesUniqueID"]);
                notes.NoteUniqueID = Guid.NewGuid();    // JSL 01/08/2023
                notes.GIRDeficienciesCommentFile = new List<GIRDeficienciesCommentFile>();
            }

            if (Request.Files.Count > 0)
            {
                try
                {
                    string strUniqueFormID = Convert.ToString(Request.Form["UniqueFormID"]);
                    string strReportType = Convert.ToString(Request.Form["ReportType"]);
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        Dictionary<string, string> dictFileData = new Dictionary<string, string>();
                        dictFileData["UniqueFormID"] = strUniqueFormID;
                        dictFileData["ReportType"] = strReportType;

                        // JSL 01/08/2023
                        dictFileData["SubDetailType"] = AppStatic.NotificationTypeComment;
                        dictFileData["SubDetailUniqueId"] = Convert.ToString(notes.NoteUniqueID);
                        // End JSL 01/08/2023

                        dictFileData = Helpers.Assets.UploadFileAndReturnPath(files[i], Convert.ToString(notes.DeficienciesUniqueID), dictDefData: dictFileData); 
                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            string strFileName = dictFileData["FileName"];
                            string strAttachmentData = dictFileData["StorePath"];
                            GIRDeficienciesCommentFile fileData = new GIRDeficienciesCommentFile();
                            fileData.GIRCommentFileID = 0;
                            fileData.NoteID = 0;
                            fileData.DeficienciesID = 0;
                            fileData.FileName = strFileName;
                            fileData.StorePath = strAttachmentData;
                            fileData.IsUpload = "true";

                            notes.GIRDeficienciesCommentFile.Add(fileData);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            // End JSL 12/03/2022
            if (notes != null)
            {
                _helper.AddDeficienciesNote(notes);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //public JsonResult AddDeficienciesInitialActions(GIRDeficienciesInitialActions notes)  // JSL 12/03/2022 commented
        public JsonResult AddDeficienciesInitialActions()   // JSL 12/03/2022
        {
            GIRDeficienciesInitialActions notes = new GIRDeficienciesInitialActions();
            APIResponse res = new APIResponse(); // RDBJ 02/19/2022 set APIResponse

            // JSL 12/03/2022
            NameValueCollection nvcForm = Request.Form;
            if (nvcForm != null && nvcForm.Count > 0)
            {
                notes.Name = nvcForm["Name"];
                notes.Description = nvcForm["Description"];
                notes.DeficienciesUniqueID = Guid.Parse(nvcForm["DeficienciesUniqueID"]);
                notes.IniActUniqueID = Guid.NewGuid();  // JSL 01/08/2023
                notes.GIRDeficienciesInitialActionsFiles = new List<GIRDeficienciesInitialActionsFile>();
            }

            if (Request.Files.Count > 0)
            {
                try
                {
                    string strUniqueFormID = Convert.ToString(Request.Form["UniqueFormID"]);
                    string strReportType = Convert.ToString(Request.Form["ReportType"]);
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        Dictionary<string, string> dictFileData = new Dictionary<string, string>();
                        dictFileData["UniqueFormID"] = strUniqueFormID;
                        dictFileData["ReportType"] = strReportType;

                        // JSL 01/08/2023
                        dictFileData["SubDetailType"] = AppStatic.NotificationTypeInitialAction;
                        dictFileData["SubDetailUniqueId"] = Convert.ToString(notes.IniActUniqueID);
                        // End JSL 01/08/2023

                        dictFileData = Helpers.Assets.UploadFileAndReturnPath(files[i], Convert.ToString(notes.DeficienciesUniqueID), dictDefData: dictFileData); 
                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            string strFileName = dictFileData["FileName"];
                            string strAttachmentData = dictFileData["StorePath"];
                            GIRDeficienciesInitialActionsFile fileData = new GIRDeficienciesInitialActionsFile();
                            fileData.GIRInitialID = 0;
                            fileData.GIRFileID = 0;
                            fileData.DeficienciesID = 0;
                            fileData.FileName = strFileName;
                            fileData.StorePath = strAttachmentData;
                            fileData.IsUpload = "true";

                            notes.GIRDeficienciesInitialActionsFiles.Add(fileData);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            // End JSL 12/03/2022

            if (notes != null)
            {
                // RDBJ 02/18/2022 added if
                if (string.IsNullOrEmpty(notes.Name))
                    notes.Name = SessionManager.Username;

                APIHelper _helper = new APIHelper();
                res = _helper.AddDeficienciesInitialActions(notes); // RDBJ 02/19/2022 set return Response
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddDeficienciesResolution(GIRDeficienciesResolution actions)
        {
            long res = 0;
            if (actions != null)
            {
                APIHelper _helper = new APIHelper();
                _helper.AddDeficienciesResolution(actions);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDeficienciesNote(Guid id)
        {
            List<DeficienciesNote> response = new List<DeficienciesNote>();
            if (id != null)
            {
                APIHelper _helper = new APIHelper();
                response = _helper.GetDeficienciesNote(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDeficienciesInitialActions(Guid id)
        {
            List<GIRDeficienciesInitialActions> response = new List<GIRDeficienciesInitialActions>();
            if (id != null)
            {

                APIHelper _apihelper = new APIHelper();
                response = _apihelper.GetDeficienciesInitialActions(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDeficienciesResolution(Guid id)
        {
            List<GIRDeficienciesResolution> response = new List<GIRDeficienciesResolution>();
            if (id != null)
            {
                APIHelper _apihelper = new APIHelper();
                response = _apihelper.GetDeficienciesResolution(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDeficienciesFiles(int id)
        {
            List<GIRDeficienciesFile> response = new List<GIRDeficienciesFile>();
            if (id != 0)
            {
                APIHelper _helper = new APIHelper();
                response = _helper.GetDeficienciesFiles(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetDeficienciesById(Guid id)
        {
            GIRDeficiencies response = new GIRDeficiencies();
            if (id != null)
            {
                APIHelper _helper = new APIHelper();
                response = _helper.GetDeficienciesById(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult getNextNumber(string ship, string reportType, string UniqueFormID) //RDBJ 11/02/2021 Added uniqueformId //RDBJ 09/24/2021 Added reportType 
        {
            APIHelper _aHelper = new APIHelper();
            int no = _aHelper.getNextNumber(ship, reportType, UniqueFormID); //RDBJ 11/02/2021 Added UniqueFormID //RDBJ 09/24/2021 Added reportType  //RDBJ 09/18/2021 removed ,null,""
            return Json(no, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Download(int file, string name)
        {
            APIHelper _helper = new APIHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.GetFile(file); 
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            //RDBJ 09/18/2021
            byte[] fileBytes = null;
            if (GetFile.Contains(";base64"))
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
            }
            else
            {
                // JSL 12/03/2022
                string strFileBasePath = Server.MapPath("~/Images/");
                string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
                // End JSL 12/03/2022

                // JSL 12/03/2022 commented
                /*
                string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
                LogHelper.writelog("---------" + filepath + "----------");
                fileBytes = System.IO.File.ReadAllBytes(filepath);
                */
                // End JSL 12/03/2022 commented
            }
            //End RDBJ 09/18/2021

            //RDBJ 09/18/2021 Commented Below
            /*
            string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
            LogHelper.writelog("---------" + filepath + "----------");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            */
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        public ActionResult DownloadCommentFile(string CommentFileID, string name)
        {
            APIHelper _helper = new APIHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.GetCommentFile(CommentFileID);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            byte[] fileBytes = null;
            if (GetFile.Contains(";base64"))
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
            }
            else
            {
                // JSL 12/03/2022
                string strFileBasePath = Server.MapPath("~/Images/");
                string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
                // End JSL 12/03/2022

                // JSL 12/03/2022 commented
                /*
                string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
                LogHelper.writelog("---------" + filepath + "----------");
                fileBytes = System.IO.File.ReadAllBytes(filepath);
                */
                // End JSL 12/03/2022 commented
            }
            // End RDBJ 01/27/2022 set with dictionary

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        public ActionResult DownloadInitialActionFile(string fileId, string name) //RDBJ 11/10/2021 changed datatype from int to string
        {
            APIHelper _helper = new APIHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.GetGIRDeficienciesInitialActionFile(fileId);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            byte[] fileBytes = null;
            if (GetFile.Contains(";base64"))
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
            }
            else
            {
                // JSL 12/03/2022
                string strFileBasePath = Server.MapPath("~/Images/");
                string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
                // End JSL 12/03/2022

                // JSL 12/03/2022 commented
                /*
                string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
                LogHelper.writelog("---------" + filepath + "----------");
                fileBytes = System.IO.File.ReadAllBytes(filepath);
                */
                // End JSL 12/03/2022 commented
            }
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        public ActionResult DownloadResolutionFile(string fileId, string name) //RDBJ 11/10/2021 changed datatype from int to string
        {
            APIHelper _helper = new APIHelper();
            
            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.GetGIRDeficienciesResolutionFile(fileId);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            byte[] fileBytes = null;
            if (GetFile.Contains(";base64"))
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
            }
            else {
                string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
                LogHelper.writelog("---------" + filepath + "----------");
                fileBytes = System.IO.File.ReadAllBytes(filepath);
            }
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);

        }
        public JsonResult AddGIRDeficiencies(GIRDeficiencies Modal) // JSL 04/06/2022 set JsonResult instead of void
        {
            APIResponse response = new APIResponse();
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;
                    Modal.IsClose = false;
                    Modal.IsSynced = false;
                    APIHelper _aHelper = new APIHelper();

                    //RDBJ 09/18/2021
                    Guid DeficienciesGUID = Guid.NewGuid();
                    Modal.DeficienciesUniqueID = DeficienciesGUID;

                    //RDBJ 11/02/2021 wrapped in if
                    if (Modal.No == 0)
                        Modal.No = _aHelper.getNextNumber(Modal.Ship, Modal.ReportType, Convert.ToString(Modal.UniqueFormID));

                    // JSL 06/27/2022
                    Modal.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>();
                    if (Modal.ReportType.ToUpper() == "SI")
                    {
                        GIRDeficienciesInitialActions defInitialActionForSI = new GIRDeficienciesInitialActions();
                        defInitialActionForSI.DeficienciesUniqueID = Modal.DeficienciesUniqueID;
                        defInitialActionForSI.Name = SessionManager.Username;
                        defInitialActionForSI.Description = string.Empty;

                        Modal.GIRDeficienciesInitialActions.Add(defInitialActionForSI);
                    }
                    // End JSL 06/27/2022

                    response = _aHelper.AddGIRDeficiencies(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficiencies Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // JSL 12/02/2022
        public JsonResult AddGIRDeficienciesWithFile()
        {
            APIResponse response = new APIResponse();
            APIHelper _aHelper = new APIHelper();

            GIRDeficiencies Modal = new GIRDeficiencies();
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            try
            {
                NameValueCollection nvcForm = Request.Form;
                if (nvcForm != null && nvcForm.Count > 0)
                {
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
                    Modal.GIRDeficienciesFile = new List<GIRDeficienciesFile>();

                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;

                    Modal.IsClose = false;
                    Modal.IsSynced = false;

                    Guid DeficienciesGUID = Guid.NewGuid();
                    Modal.DeficienciesUniqueID = DeficienciesGUID;

                    if (Modal.No == 0)
                        Modal.No = _aHelper.getNextNumber(Modal.Ship, Modal.ReportType, Convert.ToString(Modal.UniqueFormID));
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

                            Dictionary<string, string> dictFileData = Helpers.Assets.UploadFileAndReturnPath(files[i], Convert.ToString(Modal.DeficienciesUniqueID), dictDefData: dictMetaData);
                            if (dictFileData != null && dictFileData.Count > 0)
                            {
                                string strFileName = dictFileData["FileName"];
                                string strAttachmentData = dictFileData["StorePath"];
                                GIRDeficienciesFile fileData = new GIRDeficienciesFile();
                                fileData.DeficienciesFileUniqueID = Guid.NewGuid();
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

                if (Modal != null 
                    && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                    )
                {
                    Modal.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>();
                    if (Modal.ReportType.ToUpper() == "SI")
                    {
                        GIRDeficienciesInitialActions defInitialActionForSI = new GIRDeficienciesInitialActions();
                        defInitialActionForSI.DeficienciesUniqueID = Modal.DeficienciesUniqueID;
                        defInitialActionForSI.Name = SessionManager.Username;
                        defInitialActionForSI.Description = string.Empty;

                        Modal.GIRDeficienciesInitialActions.Add(defInitialActionForSI);
                    }

                    response = _aHelper.AddGIRDeficiencies(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficienciesWithFile Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        // End JSL 12/02/2022

        // RDBJ 03/05/2022
        [HttpPost]
        public JsonResult UploadGIRPhotosOrDefFiles()
        {
            APIHelper _helper = new APIHelper();
            var dictMetadata = new Dictionary<string, string>();
            bool IsUploadGIRDefFiles = Convert.ToBoolean(Convert.ToString(Request.Form["IsUploadGIRDefFiles"]));      // RDBJ 03/12/2022

            string straction = string.Empty;    // RDBJ 03/12/2022 set global

            // RDBJ 03/12/2022 wrapped in if
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
                        // JSL 12/02/2022
                        string strUniqueFormID = Convert.ToString(Request.Form["UniqueFormID"]);
                        string strReportType = Convert.ToString(Request.Form["ReportType"]);
                        string strDeficienciesUniqueID = Convert.ToString(Request.Form["DeficienciesUniqueID"]);
                        string strFileName = string.Empty;
                        string strAttachmentData = string.Empty;

                        Dictionary<string, string> dictFileData = new Dictionary<string, string>();
                        dictFileData["UniqueFormID"] = strUniqueFormID;
                        dictFileData["ReportType"] = strReportType;

                        if (IsUploadGIRDefFiles)
                            dictFileData = Helpers.Assets.UploadFileAndReturnPath(files[i], strDeficienciesUniqueID, dictDefData: dictFileData);
                        else
                            dictFileData = Helpers.Assets.UploadFileAndReturnPath(files[i], strUniqueFormID, blnIsGIRPhotos: true);

                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            strFileName = dictFileData["FileName"];
                            strAttachmentData = dictFileData["StorePath"];
                        }
                        // End JSL 12/02/2022

                        // JSL 12/02/2022 commented
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
                        string ImagePath = "data:" + strContentType + ";base64," +  Convert.ToBase64String(PictureAsBytes);  // RDBJ 03/12/2022 set contentType with base64
                        */
                        // End JSL 12/02/2022 commented

                        dictMetadata["strAction"] = straction;

                        // JSL 12/02/2022 commented
                        /*
                        dictMetadata["FileName"] = FileName;
                        dictMetadata["ImagePath"] = ImagePath;
                        */
                        // End JSL 12/02/2022 commented

                        // JSL 12/02/2022
                        dictMetadata["FileName"] = strFileName;
                        dictMetadata["ImagePath"] = strAttachmentData;
                        // End JSL 12/02/2022

                        dictMetadata["UniqueFormID"] = Convert.ToString(Request.Form["UniqueFormID"]);
                        dictMetadata["Ship"] = Convert.ToString(Request.Form["Ship"]);
                        dictMetadata["DeficienciesUniqueID"] = Convert.ToString(Request.Form["DeficienciesUniqueID"]); // RDBJ 03/12/2022
                        dictMetadata["DeficienciesFileUniqueID"] = Convert.ToString(Request.Form["DeficienciesFileUniqueID"]); // JSL 06/04/2022

                        try
                        {
                            dictToReturn = _helper.PostAsyncAPICall(APIURLHelper.APIForms, "CommonPostAPICall", dictMetadata);
                            //dictToReturn["FileName"] = FileName;  // JSL 12/02/2022 commented
                            dictToReturn["FileName"] = strFileName; // JSL 12/02/2022

                            if (!IsUploadGIRDefFiles)
                            {
                                //dictToReturn["ImagePath"] = ImagePath;    // JSL 12/02/2022 commented

                                // JSL 12/02/2022
                                string strFileBasePath = "../Images/";
                                dictToReturn["ImagePath"] = Path.Combine(strFileBasePath, strAttachmentData);
                                // End JSL 12/02/2022
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
        // End RDBJ 03/05/2022

        #endregion

        #region Risk Assessment Form
        public ActionResult RiskAssessmentFormList()
        {
            return View();
        }

        public ActionResult GetRiskAssessmentFormList(string ship)
        {
            APIHelper _helper = new APIHelper();
            List<RiskAssessmentForm> Modal = new List<RiskAssessmentForm>();
            Modal = _helper.GetRiskassessmentForm(ship);

            return Json(Modal, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult RAformData(int? id = 0, string shipCode = "", string docid = "")    // JSL 11/20/2022 commented
        public ActionResult RAformData(string id, string shipCode = "", string docid = "")    // JSL 11/20/2022
        {
            RiskAssessmentFormModal Modal = new RiskAssessmentFormModal();
            ViewBag.result = Utility.ToString(TempData["RAFRes"]);
            ViewBag.CurrentDateString = Utility.GetCurrentDateShortString();
            APIHelper _helper = new APIHelper();
            try
            {
                if (string.IsNullOrWhiteSpace(shipCode))
                    shipCode = SessionManager.ShipCode;

                // JSL 11/20/2022
                Guid guidOutput;
                bool isIdGuid = Guid.TryParse(id, out guidOutput);
                // End JSL 11/20/2022

                //if (id > 0)  // JSL 11/20/2022 commented this line
                if (isIdGuid) // JSL 11/20/2022
                {
                    //Modal = _helper.RAFormDetailsView(shipCode, Convert.ToInt32(id)); // JSL 11/20/2022 commented this line
                    Modal = _helper.RAFormDetailsView(shipCode, Convert.ToString(id));   // JSL 11/20/2022
                }
                else if (!string.IsNullOrWhiteSpace(docid))
                {
                    DocumentModal docModal = _helper.GetDocumentBYID(docid);
                    if (docModal != null && !string.IsNullOrEmpty(docModal.Path))
                    {
                        Guid newGuid = docModal.DocumentID.Value;
                        Modal.RiskAssessmentForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.RiskAssessmentForm.Number = Convert.ToString(docModal.Number).Trim();
                        Modal.RiskAssessmentForm.Title = docModal.Title;
                        Modal.RiskAssessmentForm.ShipName = SessionManager.ShipName;
                        Modal.RiskAssessmentForm.ShipCode = shipCode;
                        if (System.IO.File.Exists(docModal.Path))
                        {
                            XmlDocument xml = new XmlDocument();
                            xml.Load(docModal.Path);
                            XmlNodeList nodes = xml.DocumentElement.ChildNodes;
                            foreach (XmlNode childNodes in nodes)
                            {
                                if (childNodes.Name == "my:Review")
                                {
                                    XmlNodeList NameNode = xml.GetElementsByTagName("my:ReviewerName");
                                    XmlNodeList RankNodes = xml.GetElementsByTagName("my:ReviewerRank");
                                    XmlNodeList DateNodes = xml.GetElementsByTagName("my:ReviewDate");
                                    XmlNodeList LocationNodes = xml.GetElementsByTagName("my:ReviewLocation");
                                    XmlNodeList MemberNodes = xml.GetElementsByTagName("my:ReviewMembers");
                                    string ReviewName = NameNode[0].InnerText.Trim();
                                    Modal.RiskAssessmentForm.ReviewerName = Convert.ToString(ReviewName).Trim();
                                    string ReviewRank = RankNodes[0].InnerText.Trim();
                                    Modal.RiskAssessmentForm.ReviewerRank = Convert.ToString(ReviewRank).Trim();
                                    string ReviewDate = DateNodes[0].InnerText.Trim();
                                    if (ReviewDate != null && ReviewDate != "")
                                    {
                                        Modal.RiskAssessmentForm.ReviewerDate = Convert.ToDateTime(ReviewDate);
                                    }
                                    string ReviewLocation = LocationNodes[0].InnerText.Trim();
                                    Modal.RiskAssessmentForm.ReviewerLocation = Convert.ToString(ReviewLocation).Trim();

                                    if (!string.IsNullOrWhiteSpace(ReviewName))
                                    {
                                        Modal.RiskAssessmentFormReviewerList.Add(new RiskAssessmentFormReviewer
                                        {
                                            ReviewerName = ReviewName
                                        });
                                    }
                                    if (MemberNodes != null && MemberNodes.Count > 0)
                                    {
                                        foreach (XmlNode item in MemberNodes)
                                        {
                                            try
                                            {
                                                ReviewName = item.InnerText.Trim();
                                                if (!string.IsNullOrWhiteSpace(ReviewName))
                                                {
                                                    Modal.RiskAssessmentFormReviewerList.Add(new RiskAssessmentFormReviewer
                                                    {
                                                        ReviewerName = ReviewName
                                                    });
                                                }
                                            }
                                            catch (Exception)
                                            {
                                            }

                                        }
                                    }
                                }
                                if (childNodes.Name == "my:Header")
                                {
                                    XmlNodeList CodeNode = xml.GetElementsByTagName("my:HeaderCode");
                                    XmlNodeList IssueNode = xml.GetElementsByTagName("my:HeaderIssueNumber");
                                    XmlNodeList IssueDateNode = xml.GetElementsByTagName("my:HeaderIssueDate");
                                    XmlNodeList AmendmentNode = xml.GetElementsByTagName("my:HeaderAmmendmentNumber");
                                    XmlNodeList AmendmentDateNode = xml.GetElementsByTagName("my:HeaderAmmendmentDate");
                                    XmlNodeList IsConfidentialNode = xml.GetElementsByTagName("my:HeaderIsConfidential");

                                    string Code = CodeNode[0].InnerText.Trim();
                                    Modal.RiskAssessmentForm.Code = Convert.ToString(Code).Trim();
                                    string Issue = IssueNode[0].InnerText.Trim();
                                    Modal.RiskAssessmentForm.Issue = Convert.ToInt32(Issue);
                                    string IssueDate = "01/" + IssueDateNode[0].InnerText.Split('/')[0] + "/20" + IssueDateNode[0].InnerText.Split('/')[1];
                                    if (IssueDate != null && IssueDate != "")
                                    {
                                        Modal.RiskAssessmentForm.IssueDate = DateTime.ParseExact(IssueDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    string Amendment = AmendmentNode[0].InnerText.Trim();
                                    Modal.RiskAssessmentForm.Amendment = Convert.ToInt32(Amendment);
                                    string AmendmentDate = AmendmentDateNode[0].InnerText.Trim();
                                    if (AmendmentDate != null && AmendmentDate != "")
                                    {
                                        Modal.RiskAssessmentForm.AmendmentDate = DateTime.ParseExact(AmendmentDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    string IsConfidential = IsConfidentialNode[0].InnerText.Trim();
                                    Modal.RiskAssessmentForm.IsConfidential = Convert.ToBoolean(IsConfidential);
                                }
                                if (childNodes.Name == "my:Hazards")
                                {

                                    //string Document = xml.Attributes["Id"].InnerText;
                                    XmlNodeList HazardIdNode = xml.GetElementsByTagName("my:Hazard");
                                    XmlNodeList DescriptionNode = xml.GetElementsByTagName("my:Description");
                                    XmlNodeList SeverityNode = xml.GetElementsByTagName("my:Severity");
                                    XmlNodeList LikelihoodNode = xml.GetElementsByTagName("my:Likelihood");
                                    XmlNodeList RiskFactorNode = xml.GetElementsByTagName("my:RiskFactor");
                                    XmlNodeList ControlMeasuresNode = xml.GetElementsByTagName("my:ControlMeasures");
                                    XmlNodeList ReducedSeverityNode = xml.GetElementsByTagName("my:ReducedSeverity");
                                    XmlNodeList ReducedLikelihoodNode = xml.GetElementsByTagName("my:ReducedLikelihood");
                                    XmlNodeList ReducedRiskFactorNode = xml.GetElementsByTagName("my:ReducedRiskFactor");
                                    for (var i = 0; i < HazardIdNode.Count; i++)
                                    {
                                        RiskAssessmentFormHazard ModalHazard = new RiskAssessmentFormHazard();
                                        ModalHazard.Id = newGuid;
                                        string HazardId = HazardIdNode[i].Attributes["my:HazardId"].InnerText.Trim();
                                        ModalHazard.HazardId = Convert.ToInt64(HazardId);
                                        string Description1 = DescriptionNode[i].InnerText.Trim();
                                        ModalHazard.Stage1Description = Convert.ToString(Description1).Trim();
                                        string Severity2 = SeverityNode[i].InnerText.Trim();
                                        ModalHazard.Stage2Severity = Convert.ToString(Severity2).Trim();
                                        string Likelihood2 = LikelihoodNode[i].InnerText.Trim();
                                        ModalHazard.Stage2Likelihood = Convert.ToString(Likelihood2).Trim();
                                        string RiskFactor2 = RiskFactorNode[i].InnerText.Trim();
                                        ModalHazard.Stage2RiskFactor = Convert.ToString(RiskFactor2).Trim();
                                        string Description3 = ControlMeasuresNode[i].InnerText.Trim();
                                        ModalHazard.Stage3Description = Convert.ToString(Description3).Trim();
                                        string Severity4 = ReducedSeverityNode[i].InnerText.Trim();
                                        ModalHazard.Stage4Severity = Convert.ToString(Severity4).Trim();
                                        string Likelihood4 = ReducedLikelihoodNode[i].InnerText.Trim();
                                        ModalHazard.Stage4Likelihood = Convert.ToString(Likelihood4).Trim();
                                        string RiskFactor4 = ReducedRiskFactorNode[i].InnerText.Trim();
                                        ModalHazard.Stage4RiskFactor = Convert.ToString(RiskFactor4).Trim();
                                        Modal.RiskAssessmentFormHazardList.Add(ModalHazard);
                                    }
                                }
                            }
                        }
                    }
                }
                if (Modal.RiskAssessmentForm != null)
                    Modal.RiskAssessmentForm.IsApplicable = Modal.RiskAssessmentForm.IsApplicable.HasValue && Modal.RiskAssessmentForm.IsApplicable.Value == false ? false : true;
            }
            catch (Exception)
            {
            }
            return View(Modal);
        }

        public ActionResult RiskAssessmentForm()
        {
            ViewBag.result = Utility.ToString(TempData["RAFRes"]);
            ViewBag.CurrentDateString = Utility.GetCurrentDateShortString();
            return View();
        }

        [HttpPost]
        public ActionResult RiskAssessmentForm(RiskAssessmentFormModal Modal)
        {
            APIHelper _aHelper = new APIHelper();
            try
            {
                if (Modal != null)
                {
                    Modal.RiskAssessmentForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.RiskAssessmentForm.CreatedBy = SessionManager.Username;

                    // JSL 11/26/2022
                    Modal.RiskAssessmentForm.IsSynced = false;
                    Modal.RiskAssessmentForm.UpdatedDate = Utility.ToDateTimeUtcNow();
                    Modal.RiskAssessmentForm.UpdatedBy = SessionManager.Username;
                    // End JSL 11/26/2022

                    Modal.RiskAssessmentForm.ShipCode = Modal.RiskAssessmentForm.ShipCode ?? SessionManager.ShipCode;
                    Modal.RiskAssessmentForm.ShipName = Modal.RiskAssessmentForm.ShipName ?? SessionManager.ShipName;
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
                APIResponse resp = _aHelper.SubmitRiskAssessmentForm(Modal);
                if (resp == null || resp.result == ERROR)
                {
                    ViewBag.result = ERROR;
                    TempData["RAFRes"] = ERROR;
                }
                else
                {
                    ViewBag.result = SUCCESS;
                    TempData["RAFRes"] = SUCCESS;
                }
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
                //return RedirectToAction("RAformData", new { id = Modal.RiskAssessmentForm.RAFID, shipCode = Modal.RiskAssessmentForm.ShipCode }); // JSL 11/22/2022 commented 
                return RedirectToAction("RAformData", new { id = Modal.RiskAssessmentForm.RAFUniqueID, shipCode = Modal.RiskAssessmentForm.ShipCode }); // JSL 11/22/2022
            }
            else
            {
                return RedirectToAction("RiskAssessmentForm");
            }
        }


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
                RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
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

        public string CheckRAFNumberExist(string RAFNumber, string ShipName)
        {
            string result = string.Empty;
            try
            {
                APIHelper _raHelper = new APIHelper();
                if (_raHelper.CheckRAFNumberExistFromData(RAFNumber, ShipName))
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
        #endregion

        #region Superintended Inspection Report
        public ActionResult SuperintendedInspectionReport()
        {
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            //ViewBag.ShipName = SessionManager.ShipName; // RDBJ 01/17/2022 commented this line
            ViewBag.ShipCode = SessionManager.ShipCode; // RDBJ 01/17/2022
            SIRModal Modal = new SIRModal();
            return View(Modal);
        }
        [HttpPost]
        public ActionResult SuperintendedInspectionReport(SIRModal Modal)
        {
            ViewBag.data = Modal;
            TempData["data"] = Modal;
            APIHelper _aHelper = new APIHelper();
            try
            {
                if (Modal != null)
                {
                    if (string.IsNullOrWhiteSpace(Modal.SuperintendedInspectionReport.ShipName))
                    {
                        //Modal.Ship = SessionManager.ShipCode;
                    }
                    Modal.SuperintendedInspectionReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.SuperintendedInspectionReport.SavedAsDraft = false;
                    Modal.SuperintendedInspectionReport.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.SuperintendedInspectionReport.FormVersion = Modal.SuperintendedInspectionReport.FormVersion + Convert.ToDecimal(0.01);    // JSL 05/01/2022
                    APIResponse resp = _aHelper.SubmitSIR(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("GeneralInspectionReport");
        }
        public JsonResult SIRAutoSave(SIRModal Modal)
        {
            string UniqueFormID = string.Empty;
            try
            {
                if (Modal != null 
                    && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                    )
                {
                    if (Modal.SuperintendedInspectionReport.FormVersion == 0)
                    {
                        Modal.SuperintendedInspectionReport.FormVersion = 1;
                        Guid FormGUID = Guid.NewGuid();
                        Modal.SuperintendedInspectionReport.UniqueFormID = FormGUID;
                        Modal.SuperintendedInspectionReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    else
                    {
                        Modal.SuperintendedInspectionReport.FormVersion = Modal.SuperintendedInspectionReport.FormVersion + Convert.ToDecimal(0.01);
                    }
                    APIHelper _aHelper = new APIHelper();
                    UniqueFormID = _aHelper.SIRAutoSave(Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIR AutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(new { SIRFormID = 0, UniqueFormID = UniqueFormID, FormVersion = Modal.SuperintendedInspectionReport.FormVersion });
        }
        public ActionResult _SIR_DeficienciesSection(string id) //RDBJ 09/24/2021 change int to string
        {
            APIHelper _helper = new APIHelper();
            SuperintendedInspectionReport Modal = new SuperintendedInspectionReport();
            Modal = _helper.SIRFormGetDeficiency(id);
            return PartialView("SIR/_SIR_DeficienciesSection", Modal);
        }
        #endregion

        public ActionResult _GIR_DeficienciesSection(int id)
        {
            GeneralInspectionReport response = new GeneralInspectionReport();
            DraftsHelper _dHelper = new DraftsHelper();
            response = _dHelper.GIRDeficienciesView(id);
            return PartialView(response);
        }

        public List<CSShipsModal> LoadShipsList()
        {
            APIHelper _helper = new APIHelper();
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

            return shipsList;
        }



        // JSL 07/24/2022
        public List<CB_FormsPersonMasterModal> GetFormsPersonList(int intPersonType)
        {
            APIHelper _helper = new APIHelper();
            List<CB_FormsPersonMasterModal> lstPersons = new List<CB_FormsPersonMasterModal>();
            Dictionary<string, string> retDictMetadata = new Dictionary<string, string>();
            try
            {
                Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                dictMetaData["PersonType"] = intPersonType.ToString();
                dictMetaData["strAction"] = AppStatic.API_GETFORMSPERSONLIST;
                retDictMetadata = _helper.PostAsyncAPICall(APIURLHelper.APIUsers, "CommonPostAPICall", dictMetaData);
                lstPersons = JsonConvert.DeserializeObject<List<CB_FormsPersonMasterModal>>(retDictMetadata["FormsPersonList"]);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFormsPersonList : " + ex.Message);
            }
            return lstPersons;
        }
        // End JSL 07/24/2022

        #region IAF
        [HttpPost]
        public ActionResult GetAuditNote(Guid id)
        {
            IAF response = new IAF();
            if (id != null)
            {
                APIHelper _helper = new APIHelper();
                response = _helper.IAFormDetailsView(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAuditNotesById(Guid? id)
        {
            AuditNote response = new AuditNote();
            if (id != null)
            {
                APIHelper _helper = new APIHelper();
                response = _helper.GetAuditNotesById(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GI-SI Drafts
        // RDBJ 12/03/2021
        public ActionResult Drafts()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            ViewBag.ShipName = SessionManager.ShipName;
            return View();
        }
        // End RDBJ 12/03/2021

        // RDBJ 12/03/2021
        public ActionResult GetGISIDraftsReportsByShip(string code, string type)
        {
            APIHelper _helper = new APIHelper();
            List<GIRData> lstGIRDrafts;
            List<SIRData> lstSIRDrafts;
            List<AuditList> lstIARDrafts;   // RDBJ 01/23/2022

            if (type.ToUpper() == "GI")
            {
                lstGIRDrafts = _helper.GetGIRDrafts(code);
                return Json(lstGIRDrafts, JsonRequestBehavior.AllowGet);
            }
            else if (type.ToUpper() == "SI")    // RDBJ 01/23/2022 added if
            {
                lstSIRDrafts = _helper.GetSIRDrafts(code);
                return Json(lstSIRDrafts, JsonRequestBehavior.AllowGet);
            }
            // RDBJ 01/23/2022 added else
            else
            {
                lstIARDrafts = _helper.GetIARDrafts(code);
                return Json(lstIARDrafts, JsonRequestBehavior.AllowGet);
            }
        }
        // End RDBJ 12/03/2021
        #endregion

        #region Common Functions
        // RDBJ 03/05/2022
        public JsonResult PerformAction(string jsonmetadata, string straction)
        {
            APIHelper _helper = new APIHelper();
            //Dictionary<string, object> dictToReturn = new Dictionary<string, object>();

            Dictionary<string, string> dictToReturn = new Dictionary<string, string>();
            dictToReturn.Add("jsonMetaData", jsonmetadata);
            dictToReturn.Add("straction", straction);

            var dictMetadata = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(jsonmetadata)
                && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                )
            {
                dictMetadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonmetadata);
                dictMetadata["strAction"] = straction;

                try
                {
                    dictToReturn = _helper.PostAsyncAPICall(APIURLHelper.APIForms, "CommonPostAPICall", dictMetadata);
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Forms PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 03/05/2022
        #endregion
    }
}