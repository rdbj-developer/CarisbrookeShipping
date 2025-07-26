using Newtonsoft.Json;
using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Helpers;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    [AuthorizeFilter] //RDBJ 10/17/2021
    public class DeficienciesController : Controller
    {
        // GET: Deficiencies

        [AuthorizeFilter]
        public ActionResult Index()
        {
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            LoadShipsList();
            return View();
        }

        public List<CSShipsModal> LoadShipsList()
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
            if (shipsList == null || shipsList.Count <= 0)
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
            return shipsList;
        }

        [HttpGet]
        public ActionResult DeficienciesDetails(Guid? id)
        {
            //APIHelper _helper = new APIHelper();
            //GeneralInspectionReport list = new GeneralInspectionReport();
            //list = _helper.GIRFormDetailsView(id);

            // RDBJ2 05/10/2022
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            // End RDBJ2 05/10/2022

            return View();
        }

        #region GISI
        [AuthorizeFilter]
        public ActionResult GISIDeficiencies()
        {
            return View();
        }
        public ActionResult GetShipDeficincyGrid()
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            List<Deficiency_GISI_Ships> ModalList = new List<Deficiency_GISI_Ships>();
            ModalList = _helper.GetShipDeficincyGrid_Local_DB(SessionManager.ShipCode.ToString());
            if (ModalList == null)
            {
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                    ModalList = _helper.GetShipDeficincyGrid(SessionManager.ShipCode.ToString());
            }
            return Json(ModalList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShipGISIReports(string code, string type)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            List<Deficiency_GISI_Report> Modal = new List<Deficiency_GISI_Report>();
            Modal = _helper.GetShipGISIReports_Local_DB(code, type);
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGISIReportsDeficiencies(string ship, string UniqueFormID, string Type)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            Deficiency_GISI_Report req = new Deficiency_GISI_Report();
            req.Ship = ship;
            //req.FormID = Utility.ToLong(FormID);
            req.UniqueFormID = Guid.Parse(UniqueFormID);
            req.Type = Type;
            List<GIRDataList> Modal = new List<GIRDataList>();
            Modal = _helper.GetDeficienciesData_Local_DB(req);

            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson(); //RDBJ 10/26/2021
            
            return Json(new { deficiencies = Modal, isInspector = ServerModal.IsInspector }, JsonRequestBehavior.AllowGet); //RDBJ 10/26/2021 Added isInspector
        }

        public ActionResult GetDeficienciesByDeficienciesID(Guid? id)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            GIRDataList Modal = new GIRDataList();

            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson(); //RDBJ 10/26/2021

            Modal = _helper.GetDeficienciesDataByDeficienciesID_Local_DB(id);
            return Json(new { details = Modal, isInspector = ServerModal.IsInspector }, JsonRequestBehavior.AllowGet); //RDBJ 10/26/2021 Added isInspector
        }
        [HttpPost]
        public ActionResult GetDeficienciesNote(Guid id)
        {
            List<GIRDeficienciesNote> response = new List<GIRDeficienciesNote>();
            if (id != null)
            {
                APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                response = _helper.GetDeficienciesNote_Local_DB(id);
            }
            //var jsonResult = Json(response, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetDeficienciesFiles(int id)
        {
            List<GIRDeficienciesFile> response = new List<GIRDeficienciesFile>();
            if (id != 0)
            {
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    APIHelper _helper = new APIHelper();
                    response = _helper.GetDeficienciesFiles(id);
                }
                else
                {
                    APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                    response = _helper.GetDeficienciesFiles_Local_DB(id);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Download(int file, string name)
        {
            byte[] fileBytes = null;
            try
            {
                APIDeficienciesHelper _helperLocal = new APIDeficienciesHelper();

                // RDBJ 01/27/2022 set with dictionary
                Dictionary<string, string> retDicData = _helperLocal.GetGIRDeficienciesFiles_Local(file);
                string GetFile = Convert.ToString(retDicData["FileData"]);
                name = Convert.ToString(retDicData["FileName"]);
                // End RDBJ 01/27/2022 set with dictionary

                //if (string.IsNullOrWhiteSpace(GetFile))   // JSL 11/12/2022 commented
                if (!GetFile.StartsWith("data:"))    // JSL 11/12/2022
                {
                    // JSL 11/12/2022
                    string strFileBasePath = Server.MapPath("~/Images/");
                    string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                    return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
                    // End JSL 11/12/2022

                    // JSL 11/12/2022 commented
                    /*
                    APIHelper _helper = new APIHelper();
                    GetFile = _helper.GetFile(file);
                    LogHelper.writelog("---------" + DowloadFilePath(GetFile) + "----------");
                    fileBytes = System.IO.File.ReadAllBytes(DowloadFilePath(GetFile));
                    */
                    // JSL 11/12/2022 commented
                }
                else
                {
                    Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                    GetFile = regex.Replace(GetFile, string.Empty);
                    fileBytes = Convert.FromBase64String(GetFile);
                }
            }
            catch (Exception ex)
            {
            }
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        public ActionResult DownloadCommentFile(string CommentFileID, string name)
        {
            byte[] fileBytes = null;
            APIDeficienciesHelper _helperLocal = new APIDeficienciesHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helperLocal.DownloadCommentFile_Local_DB(CommentFileID);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            // JSL 11/12/2022 wrapped in if
            if (GetFile.StartsWith("data:"))
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
            }
            else
            {
                string strFileBasePath = Server.MapPath("~/Images/");
                string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
            }
            // End JSL 11/12/2022 wrapped in if
        }
        public ActionResult DownloadAuditCommentFile(int CommentFileID, string name)
        {
            byte[] fileBytes = null;
            IAFHelper _lhelper = new IAFHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _lhelper.GetAuditNoteCommentFile_Local_DB(CommentFileID);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            // JSL 11/12/2022 wrapped in if
            if (GetFile.StartsWith("data:"))
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
            }
            else
            {
                string strFileBasePath = Server.MapPath("~/Images/");
                string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
            }
            // End JSL 11/12/2022 wrapped in if
        }
        [HttpPost]
        //public JsonResult AddDeficienciesNote(GIRDeficienciesNote notes)    // JSL 11/08/2022 commented this line
        public JsonResult AddDeficienciesNote()    // JSL 11/08/2022
        {
            long res = 0;
            APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();

            // JSL 11/08/2022
            GIRDeficienciesNote notes = new GIRDeficienciesNote();
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
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        // JSL 01/08/2023
                        Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                        dictMetaData["SubDetailType"] = AppStatic.NotificationTypeComment;
                        dictMetaData["SubDetailUniqueId"] = Convert.ToString(notes.NoteUniqueID);
                        // End JSL 01/08/2023

                        //Dictionary<string, string> dictFileData = Assets.UploadFileAndReturnBase64EndCodedURL(files[i]);    // JSL 11/12/2022 commented
                        Dictionary<string, string> dictFileData = Assets.UploadFileAndReturnPath(files[i], Convert.ToString(notes.DeficienciesUniqueID) // JSL 11/12/2022
                            , dictDefData: dictMetaData // JSL 01/08/2023
                            ); 
                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            string strFileName = dictFileData["FileName"];
                            //string strAttachmentData = dictFileData["AttachmentBase64"];    // JSL 11/12/2022 commented
                            string strAttachmentData = dictFileData["StorePath"];   // JSL 11/12/2022
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
            // End JSL 11/08/2022

            if (notes != null)
            {
                _defhelper.AddDeficienciesNote_Local_DB(notes);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // RDBJ 12/21/2021 this is for GI and SI
        [HttpPost]
        public ActionResult UpdateDeficienciesData(string id, string isClose)
        {
            bool res = false;
            if (id != "")
            {
                APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();
                res = _defhelper.UpdateDeficienciesData_Local_DB(id, Convert.ToBoolean(isClose));
                LogHelper.LogForDeficienciesClose(id, SessionManager.Username, isClose); // RDBJ 12/21/2021
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetDeficienciesInitialActions(Guid id)
        {
            List<GIRDeficienciesInitialActions> response = new List<GIRDeficienciesInitialActions>();
            if (id != null)
            {
                APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                response = _helper.GetDeficienciesInitialActions_Local_DB(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public JsonResult AddDeficienciesInitialActions(GIRDeficienciesInitialActions actions)  // JSL 11/08/2022 commented this line
        public JsonResult AddDeficienciesInitialActions()
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();  // RDBJ 02/20/2022
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();

            // JSL 11/08/2022
            GIRDeficienciesInitialActions actions = new GIRDeficienciesInitialActions();
            NameValueCollection nvcForm = Request.Form;
            if (nvcForm != null && nvcForm.Count > 0)
            {
                actions.Name = nvcForm["Name"];
                actions.Description= nvcForm["Description"];
                actions.DeficienciesUniqueID = Guid.Parse(nvcForm["DeficienciesUniqueID"]);
                actions.IniActUniqueID = Guid.NewGuid();    // JSL 01/08/2023
                actions.GIRDeficienciesInitialActionsFiles = new List<GIRDeficienciesInitialActionsFile>();
            }

            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        // JSL 01/08/2023
                        Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                        dictMetaData["SubDetailType"] = AppStatic.NotificationTypeInitialAction;
                        dictMetaData["SubDetailUniqueId"] = Convert.ToString(actions.IniActUniqueID);
                        // End JSL 01/08/2023

                        //Dictionary<string, string> dictFileData = Assets.UploadFileAndReturnBase64EndCodedURL(files[i]);    // JSL 11/12/2022 commented
                        Dictionary<string, string> dictFileData = Assets.UploadFileAndReturnPath(files[i], Convert.ToString(actions.DeficienciesUniqueID) // JSL 11/12/2022
                            , dictDefData: dictMetaData // JSL 01/08/2023
                            );
                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            string strFileName = dictFileData["FileName"];
                            //string strAttachmentData = dictFileData["AttachmentBase64"];    // JSL 11/12/2022 commented
                            string strAttachmentData = dictFileData["StorePath"];   // JSL 11/12/2022
                            GIRDeficienciesInitialActionsFile fileData = new GIRDeficienciesInitialActionsFile();
                            fileData.GIRInitialID = 0;
                            fileData.GIRFileID = 0;
                            fileData.DeficienciesID = 0;
                            fileData.FileName = strFileName;
                            fileData.StorePath = strAttachmentData;
                            fileData.IsUpload = "true";

                            actions.GIRDeficienciesInitialActionsFiles.Add(fileData);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            // End JSL 11/08/2022

            if (actions != null)
            {
                // RDBJ 02/20/2022 added if
                if (string.IsNullOrEmpty(actions.Name))
                    actions.Name = SessionManager.Username;

                retDictMetaData = _helper.AddDeficienciesInitialActions_Local_DB(actions); // RDBJ 02/20/2022 set return with Dictionary
            }
            return Json(retDictMetaData, JsonRequestBehavior.AllowGet); // RDBJ 02/20/2022 added retDictMetaData
        }
        public ActionResult DownloadInitialActionFile(string IntActFileID, string name) //RDBJ 09/18/2021 changed parameter name fileId
        {
            byte[] fileBytes = null;
            try
            {
                APIDeficienciesHelper _helperLocal = new APIDeficienciesHelper();

                // RDBJ 01/27/2022 set with dictionary
                Dictionary<string, string> retDicData = _helperLocal.GetGIRDeficienciesInitialActionFile_Local(IntActFileID); //RDBJ 09/18/2021 changed parameter name fileId
                string GetFile = Convert.ToString(retDicData["FileData"]);
                name = Convert.ToString(retDicData["FileName"]);
                // End RDBJ 01/27/2022 set with dictionary

                //RDBJ 09/28/2021 Added Logic
                if (GetFile.Contains(";base64"))
                {
                    Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                    GetFile = regex.Replace(GetFile, string.Empty);
                    fileBytes = Convert.FromBase64String(GetFile);
                }
                else
                {
                    // JSL 11/12/2022
                    string strFileBasePath = Server.MapPath("~/Images/");
                    string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                    return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
                    // End JSL 11/12/2022

                    // JSL 11/12/2022 commented
                    /*
                    string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
                    LogHelper.writelog("---------" + filepath + "----------");
                    fileBytes = System.IO.File.ReadAllBytes(filepath);
                    */
                    // End JSL 11/12/2022 commented
                }
                //End RDBJ 09/28/2021 Added Logic

                //RDBJ 09/18/2021 Commented Below
                /*
                if (string.IsNullOrWhiteSpace(GetFile))
                {
                    APIHelper _helper = new APIHelper();
                    GetFile = _helper.GetGIRDeficienciesInitialActionFile(IntActFileID); //RDBJ 09/18/2021 changed parameter name fileId
                    LogHelper.writelog("---------" + DowloadFilePath(GetFile) + "----------");
                    fileBytes = System.IO.File.ReadAllBytes(DowloadFilePath(GetFile));
                }
                else
                {
                    Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                    GetFile = regex.Replace(GetFile, string.Empty);
                    fileBytes = Convert.FromBase64String(GetFile);
                }
                */
            }
            catch (Exception ex)
            {
            }
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        [HttpPost]
        public ActionResult GetDeficienciesResolution(Guid id)
        {
            List<GIRDeficienciesResolution> response = new List<GIRDeficienciesResolution>();
            if (id != null)
            {
                APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                response = _helper.GetDeficienciesResolution_Local_DB(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddDeficienciesResolution(GIRDeficienciesResolution actions)
        {
            long res = 0;
            if (actions != null)
            {
                APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                _helper.AddDeficienciesResolution_Local_DB(actions);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadResolutionFile(string ResolutionFileID, string name) //RDBJ 09/18/2021 changed parameter name fileId
        {
            byte[] fileBytes = null;
            APIDeficienciesHelper _helperLocal = new APIDeficienciesHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helperLocal.GetGIRDeficienciesResolutionFile_Local(ResolutionFileID); //RDBJ 09/18/2021 changed parameter name fileId
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            //RDBJ 09/28/2021 Added Logic
            if (GetFile.Contains(";base64"))
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
            }
            else
            {
                // JSL 11/12/2022
                string strFileBasePath = Server.MapPath("~/Images/");
                string strFileDownloadPath = Path.Combine(strFileBasePath, GetFile);
                return File(strFileDownloadPath, System.Net.Mime.MediaTypeNames.Application.Octet, name);
                // End JSL 11/12/2022

                // JSL 11/12/2022 commented
                /*
                string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
                LogHelper.writelog("---------" + filepath + "----------");
                fileBytes = System.IO.File.ReadAllBytes(filepath);
                */
                // End JSL 11/12/2022 commented
            }
            //End RDBJ 09/28/2021 Added Logic

            //RDBJ 09/18/2021 Commented Below
            /*
            if (string.IsNullOrWhiteSpace(GetFile))
            {
                APIHelper _helper = new APIHelper();
                GetFile = _helper.GetGIRDeficienciesResolutionFile(ResolutionFileID); //RDBJ 09/18/2021 changed parameter name fileId
                LogHelper.writelog("---------" + DowloadFilePath(GetFile) + "----------");
                fileBytes = System.IO.File.ReadAllBytes(DowloadFilePath(GetFile));
            }
            else
            {
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                GetFile = regex.Replace(GetFile, string.Empty);
                fileBytes = Convert.FromBase64String(GetFile);
            }
            */
        }
        [HttpPost]
        public JsonResult DeleteDefectFromDB(string UniqueFormID, string ReportType, string DefID)
        {
            if (!string.IsNullOrEmpty(DefID))
            {
                GIRTableHelper _helper = new GIRTableHelper();
                _helper.DeleteGIRDeficiencies_Local_DB(Guid.Parse(UniqueFormID), ReportType, DefID);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //RDBJ 10/30/2021
        [HttpGet]
        public ActionResult getDeficienciesNumbers(string ship, string ReportType, string UniqueFormID)
        {
            List<int> Numbers = new List<int>();
            GIRTableHelper _helper = new GIRTableHelper();
            Numbers = _helper.getDeficienciesDeletedNumbers(ship, ReportType, UniqueFormID);
            return Json(Numbers, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/30/2021

        //RDBJ 10/30/2021
        [HttpPost]
        public JsonResult UpdateDeficiencyPriority(string DeficienciesUniqueID, int PriorityWeek)
        {
            if (!string.IsNullOrEmpty(DeficienciesUniqueID))
            {
                GIRTableHelper _helper = new GIRTableHelper();
                _helper.UpdateDeficiencyPriority_Local_DB(DeficienciesUniqueID, PriorityWeek);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/30/2021

        // RDBJ 02/19/2022
        [HttpPost]
        public JsonResult CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            if (dicMetadata != null)
            {
                GIRTableHelper _helper = new GIRTableHelper();
                dicRetMetadata = _helper.CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(dicMetadata);
            }
            return Json(new { dicRetMetadata = dicRetMetadata }, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 02/19/2022

        // RDBJ 02/20/2022
        [HttpPost]
        public JsonResult UpdateCorrectiveAction(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            if (dicMetadata != null)
            {
                dicMetadata.Add("Name", SessionManager.Username);

                GIRTableHelper _helper = new GIRTableHelper();
                dicRetMetadata = _helper.UpdateCorrectiveAction(dicMetadata);
            }
            return Json(dicRetMetadata, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 02/20/2022
        #endregion

        #region Audit
        [AuthorizeFilter]
        public ActionResult AuditDeficiencies()
        {
            return View();
        }
        public ActionResult GetAuditShipsDeficincyGrid()
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            List<Deficiency_Audit_Ships> AuditList = new List<Deficiency_Audit_Ships>();
            AuditList = _helper.GetAuditShipsDeficincyGrid_Local_DB(SessionManager.ShipCode.ToString());
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShipAudits(string code)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            List<Deficiency_Ship_Audits> AuditList = new List<Deficiency_Ship_Audits>();
            AuditList = _helper.GetShipAudits_Local_DB(code);
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAuditDetails(Guid? id)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            IAF Modal = new IAF();
            List<AuditDetail> AuditList = new List<AuditDetail>();
            Modal = _helper.GetAuditDetails_Local_DB(id);
            if (Modal != null && Modal.AuditNote != null)
            {
                List<AuditNote> NotesList = Modal.AuditNote; // RDBJ 01/31/2022 Removed Conditions //.Where(x => x.Type == "ISM-Non Conformity" || x.Type == "ISPS-Non Conformity" || x.Type == "ISM-Observation" || x.Type == "ISPS-Observation").ToList();
                AuditList = NotesList.Select(x => new AuditDetail()
                {
                    NoteID = x.AuditNotesId,
                    UniqueFormID = x.UniqueFormID,
                    NotesUniqueID = x.NotesUniqueID,
                    Number = Convert.ToInt64(x.Number),  // RDBJ 01/31/2022
                    //Type = x.Type == "ISM-Non Conformity" || x.Type == "ISPS-Non Conformity" ? "NCN" : x.Type == "ISM-Observation" || x.Type == "ISPS-Observation" ? "OBS" : string.Empty, // RDBJ 01/20/2022 Commented this line
                    Type = x.Type == "ISM-Non Conformity" ? "ISM-NCN" : x.Type == "ISPS-Non Conformity" ? "ISPS-NCN" : x.Type == "ISM-Observation" ? "ISM-OBS" : x.Type == "ISPS-Observation" ? "ISPS-OBS" : x.Type == "MLC-Deficiency" ? "MLC" : string.Empty, // RDBJ 01/31/2022 Added MLC // RDBJ 01/20/2022
                    Deficiency = x.BriefDescription,
                    Reference = x.Reference,
                    DueDate = Utility.ToDateTimeStr(x.TimeScale),
                    IsResolved = x.IsResolved,
                    AuditNotesAttachment = x.AuditNotesAttachment
                }).ToList();
            }
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }

        // RDBJ 12/21/2021 This is for Audit 
        [HttpPost]
        public ActionResult UpdateAuditDeficiencies(string id, string isClose)
        {
            bool res = false;
            if (id != "")
            {
                APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                res = _helper.UpdateAuditDeficiencies_Local_DB(id, Convert.ToBoolean(isClose));
                LogHelper.LogForDeficienciesClose(id, SessionManager.Username, isClose); // RDBJ 12/21/2021
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public ActionResult AddAuditDeficiencyComments(Audit_Deficiency_Comments Modal)   // JSL 11/08/2022 commented this line
        public ActionResult AddAuditDeficiencyComments()    // JSL 11/08/2022
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();

            // JSL 11/08/2022
            Audit_Deficiency_Comments Modal = new Audit_Deficiency_Comments();
            NameValueCollection nvcForm = Request.Form;
            if (nvcForm != null && nvcForm.Count > 0)
            {
                Modal.UserName = nvcForm["UserName"];
                Modal.Comment = nvcForm["Comment"];
                Modal.NotesUniqueID = Guid.Parse(nvcForm["NotesUniqueID"]);
                Modal.CommentUniqueID = Guid.NewGuid();    // JSL 01/08/2023
                Modal.AuditDeficiencyCommentsFiles = new List<Audit_Deficiency_Comments_Files>();
            }

            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        // JSL 01/08/2023
                        Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                        dictMetaData["SubDetailType"] = AppStatic.NotificationTypeComment;
                        dictMetaData["SubDetailUniqueId"] = Convert.ToString(Modal.CommentUniqueID);
                        // End JSL 01/08/2023

                        //Dictionary<string, string> dictFileData = Assets.UploadFileAndReturnBase64EndCodedURL(files[i]);    // JSL 11/12/2022 commented
                        Dictionary<string, string> dictFileData = Assets.UploadFileAndReturnPath(files[i], Convert.ToString(Modal.NotesUniqueID), true // JSL 11/12/2022
                             , dictDefData: dictMetaData // JSL 01/08/2023
                            );
                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            string strFileName = dictFileData["FileName"];
                            //string strAttachmentData = dictFileData["AttachmentBase64"];    // JSL 11/12/2022 commented
                            string strAttachmentData = dictFileData["StorePath"];   // JSL 11/12/2022
                            Audit_Deficiency_Comments_Files fileData = new Audit_Deficiency_Comments_Files();
                            fileData.CommentFileID = 0;
                            fileData.CommentsID = 0;
                            fileData.AuditNoteID = 0;
                            fileData.FileName = strFileName;
                            fileData.StorePath = strAttachmentData;
                            fileData.IsUpload = "true";

                            Modal.AuditDeficiencyCommentsFiles.Add(fileData);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                // End JSL 11/08/2022
            }

            bool res = _helper.AddAuditDeficiencyComments_Local_DB(Modal);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAuditDeficiencyComments(Guid? NotesUniqueID)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            List<Audit_Deficiency_Comments> Comments = new List<Audit_Deficiency_Comments>();
            Comments = _helper.GetAuditDeficiencyComments_Local_DB(NotesUniqueID);
            return Json(Comments, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAuditDeficiencyCommentFiles(Guid? commentUniqueID)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
            List<Audit_Deficiency_Comments_Files> response = new List<Audit_Deficiency_Comments_Files>();
            response = _helper.GetAuditDeficiencyCommentFiles_Local_DB(commentUniqueID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadAuditFile(int file, string name)
        {
            APIHelper _helper = new APIHelper();
            string GetFile = _helper.DownloadAuditFile(file);
            string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
            LogHelper.writelog("---------" + filepath + "----------");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        [HttpGet]
        public ActionResult InternalAuditDetails(Guid? id)
        {
            // RDBJ2 03/31/2022
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            // RDBJ2 03/31/2022

            return View();
        }
        #endregion

        public string DowloadFilePath(string GetFile)
        {
            string filepath = "";
            try
            {
                string CurrentURl = Request.Url.AbsoluteUri;
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\\", @"\");

                if (CurrentURl.Contains("localhost"))
                {
                    filepath = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\") + @"CarisbrookeShippingAPI\CarisbrookeShippingAPI") + GetFile.Replace(@"/", @"\");
                }
                else
                {
                    filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return filepath;
        }

        #region common function
        // RDBJ2 05/10/2022
        public JsonResult PerformAction(string jsonmetadata, string straction)
        {
            APIDeficienciesHelper _helper = new APIDeficienciesHelper();

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
                    LogHelper.writelog("Deficiencies PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ2 05/10/2022
        #endregion
    }
}