using Newtonsoft.Json;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class DeficienciesController : Controller
    {
        // GET: Deficiencies
        public ActionResult Index()
        {
            return View();
        }

        #region GI/SI Deficiencies
        public ActionResult GetGISIShips()
        {
            APIHelper _helper = new APIHelper();
            List<Deficiency_GISI_Ships> Modal = _helper.GetGISIShips();
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShipGISIReports(string code, string type)
        {
            APIHelper _helper = new APIHelper();
            List<Deficiency_GISI_Report> Modal = _helper.GetShipGISIReports(code, type);
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGISIReportsDeficiencies(string ship, string UniqueFormID, string Type)
        {
            APIHelper _helper = new APIHelper();
            Deficiency_GISI_Report req = new Deficiency_GISI_Report();
            req.Ship = ship;
            req.UniqueFormID = Guid.Parse(UniqueFormID);
            //req.FormID = Utility.ToLong(FormID);
            req.Type = Type;
            List<GIRDataList> Modal = _helper.GetGISIDeficiencies(req);
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }

        //RDBJ 11/02/2021
        [HttpGet]
        public ActionResult getDeficienciesNumbers(string ship, string ReportType, string UniqueFormID)
        {
            List<int> Numbers = new List<int>();
            APIHelper _helper = new APIHelper();
            Numbers = _helper.getDeficienciesDeletedNumbers(ship, ReportType, UniqueFormID);
            return Json(Numbers, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/02/2021

        //RDBJ 11/02/2021
        [HttpPost]
        public JsonResult UpdateDeficiencyPriority(string DeficienciesUniqueID, int PriorityWeek
            , string DueDate    // RDBJ 02/28/2022
            )
        {
            if (!string.IsNullOrEmpty(DeficienciesUniqueID)
                && SessionManager.UserGroup != "8"  // JSL 12/21/2022
                )
            {
                APIHelper _helper = new APIHelper();
                _helper.UpdateDeficiencyPriority(DeficienciesUniqueID, PriorityWeek
                    , DueDate   // RDBJ 02/28/2022
                    );
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/02/2021

        public JsonResult DeleteDefectFromDB(string UniqueFormID, string ReportType, string defID) //RDBJ 11/02/2021 updated with defID
        {
            APIHelper _helper = new APIHelper();
            _helper.DeleteGIRDeficiencies(UniqueFormID, ReportType, defID); //RDBJ 11/02/2021 updated with defID
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // RDBJ 12/16/2021
        public ActionResult GetAllUsersProfileList()
        {
            APIHelper _helper = new APIHelper();
            List<UserProfile> Modal = _helper.GetAllUsers()
                //.Where(x => x.UserGroup != 5) // RDBJ 12/17/2021 commented this // RDBJ 12/17/2021 set 5 for avoid ISM Dept Group members
                .OrderBy(x => x.UserName) // RDBJ 12/17/2021
                .ToList();
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/16/2021

        //RDBJ 12/17/2021
        [HttpPost]
        public JsonResult UpdateDeficiencyAssignToUser(string DeficienciesUniqueID, string AssignTo
            , string blnIsIAF   // RDBJ 12/21/2021
            , string blnIsNeedToDelete   // JSL 07/02/2022
            )
        {
            if (!string.IsNullOrEmpty(DeficienciesUniqueID)
                && SessionManager.UserGroup != "8"  // JSL 12/21/2022
                )
            {
                APIHelper _helper = new APIHelper();
                _helper.UpdateDeficiencyAssignToUser(DeficienciesUniqueID, AssignTo
                    , blnIsIAF  // RDBJ 12/21/2021
                    , blnIsNeedToDelete  // JSL 07/02/2022
                    );
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 12/17/2021

        // RDBJ 02/17/2022
        [HttpPost]
        public JsonResult CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            if (dicMetadata != null)
            {
                APIHelper _helper = new APIHelper();
                dicRetMetadata = _helper.CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(dicMetadata);
            }
            return Json( new { dicRetMetadata = dicRetMetadata }, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 02/17/2022

        // RDBJ 02/18/2022
        [HttpPost]
        public JsonResult UpdateCorrectiveAction(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            if (dicMetadata != null)
            {
                dicMetadata.Add("Name", SessionManager.Username);

                APIHelper _helper = new APIHelper();
                dicRetMetadata = _helper.UpdateCorrectiveAction(dicMetadata);
            }
            return Json(dicRetMetadata, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 02/18/2022
        #endregion

        #region Audit Degiciencies
        public ActionResult GetAuditShips(string code)
        {
            APIHelper _helper = new APIHelper();
            List<Deficiency_Audit_Ships> AuditList = _helper.GetAuditShips(code);
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AuditDeficiencies()
        {
            return View();
        }
        public ActionResult GetShipAudits(string code
            , bool blnIsAddedNewAudit   // JSL 04/20/2022
            )
        {
            APIHelper _helper = new APIHelper();
            List<Deficiency_Ship_Audits> AuditList = _helper.GetShipAudits(code
                    , blnIsAddedNewAudit    // JSL 04/20/2022
                );
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAuditDetails(Guid? id)
        {
            APIHelper _helper = new APIHelper();
            IAF Modal = _helper.IAFormDetailsView(id);
            List<AuditDetail> AuditList = new List<AuditDetail>();
            if (Modal != null && Modal.AuditNote != null)
            {
                //List<AuditNote> NotesList = Modal.AuditNote.Where(x => x.Type == "ISM-Non Conformity" || x.Type == "ISPS-Non Conformity" || x.Type == "ISM-Observation" || x.Type == "ISPS-Observation").ToList(); //RDBJ 11/25/2021 commented this line
                List<AuditNote> NotesList = Modal.AuditNote.ToList(); //RDBJ 11/25/2021 Removed where Condition 
                AuditList = NotesList.Select(x => new AuditDetail()
                {
                    NoteID = x.AuditNotesId,
                    NotesUniqueID = x.NotesUniqueID,
                    InternalAuditFormId = x.InternalAuditFormId,
                    Number = Convert.ToInt64(x.Number),  // RDBJ 01/24/2022
                    //Type = x.Type == "ISM-Non Conformity" || x.Type == "ISPS-Non Conformity" ? "NCN" : x.Type == "ISM-Observation" || x.Type == "ISPS-Observation" ? "OBS" : x.Type == "MLC-Deficiency" ? "MLC" : string.Empty, // RDBJ 01/20/2022 Commented this line  //RDBJ 11/25/2021 added x.Type == "MLC-Deficiency" ? "MLC" 
                    Type = x.Type == "ISM-Non Conformity" ? "ISM-NCN" : x.Type == "ISPS-Non Conformity" ? "ISPS-NCN" : x.Type == "ISM-Observation" ? "ISM-OBS" : x.Type == "ISPS-Observation" ? "ISPS-OBS" : x.Type == "MLC-Deficiency" ? "MLC" : string.Empty, // RDBJ 01/20/2022
                    Deficiency = x.BriefDescription,
                    Reference = x.Reference,
                    DueDate = x.TimeScale != null ? Utility.ToDateTimeStr(x.TimeScale) : "", // RDBJ 03/31/2022 Handle Null
                    IsResolved = x.isResolved,
                    UpdatedDate = x.UpdatedDate, // RDBJ 12/16/2021
                }).ToList();
            }
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateAuditDeficiencies(string id, string isClose)
        {
            bool res = false;
            if (id != ""
                && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                )
            {
                APIHelper _helper = new APIHelper();
                res = _helper.UpdateAuditDeficiencies(id, Convert.ToBoolean(isClose));
                LogHelper.LogForDeficienciesClose(id, SessionManager.Username, isClose); // RDBJ 12/21/2021
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public ActionResult AddAuditDeficiencyComments(Audit_Deficiency_Comments Modal)   // JSL 12/03/2022 commented
        public ActionResult AddAuditDeficiencyComments() // JSL 12/03/2022
        {
            APIHelper _helper = new APIHelper();
            Audit_Deficiency_Comments Modal = new Audit_Deficiency_Comments();

            NameValueCollection nvcForm = Request.Form;
            if (nvcForm != null && nvcForm.Count > 0)
            {
                Modal.UserName = nvcForm["UserName"];
                Modal.Comment = nvcForm["Comment"];
                Modal.NotesUniqueID = Guid.Parse(nvcForm["NotesUniqueID"]);
                Modal.CommentUniqueID = Guid.NewGuid(); // JSL 01/08/2023
                Modal.AuditDeficiencyCommentsFiles = new List<Audit_Deficiency_Comments_Files>();
            }
            if (Request.Files.Count > 0)
            {
                try
                {
                    string strUniqueFormID = Convert.ToString(Request.Form["UniqueFormID"]);
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        Dictionary<string, string> dictFileData = new Dictionary<string, string>();
                        dictFileData["UniqueFormID"] = strUniqueFormID;
                        dictFileData["ReportType"] = "IA";

                        // JSL 01/08/2023
                        dictFileData["SubDetailType"] = AppStatic.NotificationTypeComment;
                        dictFileData["SubDetailUniqueId"] = Convert.ToString(Modal.CommentUniqueID);
                        // End JSL 01/08/2023

                        dictFileData = Helpers.Assets.UploadFileAndReturnPath(files[i], Convert.ToString(Modal.NotesUniqueID), true, dictDefData: dictFileData); 

                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            string strFileName = dictFileData["FileName"];
                            string strAttachmentData = dictFileData["StorePath"];   
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

            bool res = _helper.AddAuditDeficiencyComments(Modal);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAuditDeficiencyComments(Guid? NotesUniqueID)
        {
            APIHelper _helper = new APIHelper();
            List<Audit_Deficiency_Comments> Comments = _helper.GetAuditDeficiencyComments(NotesUniqueID);
            return Json(Comments, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAuditDeficiencyCommentFiles(long id)
        {
            APIHelper _helper = new APIHelper();
            List<Audit_Deficiency_Comments_Files> response = _helper.GetAuditDeficiencyCommentFiles(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadAuditFile(string file, string name)
        {
            byte[] fileBytes = null;
            APIHelper _helper = new APIHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.DownloadAuditFile(file);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            // JSL 12/03/2022 wrapped in if and added else
            if (GetFile.Contains(";base64"))
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
            // End JSL 12/03/2022 wrapped in if and added else
        }

        public ActionResult DownloadCommentFile(string CommentFileID, string name)
        {
            byte[] fileBytes = null;
            APIHelper _helper = new APIHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.GetAuditCommentFile(CommentFileID);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            // JSL 12/03/2022 wrapped in if and added else
            if (GetFile.Contains(";base64"))
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
            // End JSL 12/03/2022 wrapped in if and added else

        }
        #endregion

        #region FSTO
        // JSL 02/11/2023
        public ActionResult GetFSTOAuditData(string code)
        {
            APIHelper _helper = new APIHelper();
            List<FSTOInspection> AuditList = _helper.GetFSTOAuditDataByShipCode(code);
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        // End JSL 02/11/2023

        public ActionResult DownloadFSTOFile(string fileId)
        {
            byte[] fileBytes = null;
            APIHelper _helper = new APIHelper();

            Dictionary<string, string> retDicData = _helper.GetFSTOFile(fileId);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            string name = Convert.ToString(retDicData["FileName"]);

            if (GetFile.Contains(";base64"))
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
        }

        // JSL 02/18/2023
        [HttpPost]
        public JsonResult UploadFSTOFiles()
        {
            APIHelper _helper = new APIHelper();
            var dictMetadata = new Dictionary<string, string>();
            string straction = string.Empty;    
            straction = AppStatic.API_UPLOADFSTOFILES;

            Dictionary<string, string> dictToReturn = new Dictionary<string, string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string strUniqueID = Convert.ToString(Request.Form["UniqueID"]);
                        string strUniqueFormID = Convert.ToString(Request.Form["UniqueFormID"]);
                        string strReportType = Convert.ToString(Request.Form["ReportType"]);
                        string strFileName = string.Empty;
                        string strAttachmentData = string.Empty;

                        Dictionary<string, string> dictFileData = new Dictionary<string, string>();
                        dictFileData["UniqueFormID"] = strUniqueFormID;
                        dictFileData["ReportType"] = strReportType;

                        dictFileData = Helpers.Assets.UploadFileAndReturnPath(files[i], strUniqueFormID, blnIsFSTOAttachment: true);

                        if (dictFileData != null && dictFileData.Count > 0)
                        {
                            strFileName = dictFileData["FileName"];
                            strAttachmentData = dictFileData["StorePath"];
                        }

                        dictMetadata["strAction"] = straction;

                        dictMetadata["UniqueID"] = strUniqueID;
                        dictMetadata["UniqueFormID"] = strUniqueFormID;
                        dictMetadata["AttachmentName"] = strFileName;
                        dictMetadata["AttachmentPath"] = strAttachmentData;
                        dictMetadata["CurrentUserID"] = SessionManager.UserGUID;

                        try
                        {
                            dictToReturn = _helper.PostAsyncAPICall(APIURLHelper.APIDeficiencies, "CommonPostAPICall", dictMetadata);

                            string strFileBasePath = "../Images/";
                            dictToReturn["FileName"] = strFileName;
                            dictToReturn["ImagePath"] = Path.Combine(strFileBasePath, strAttachmentData);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("UploadFSTOFiles : " + straction + " Error : " + ex.Message);
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
        // End JSL 02/18/2023
        #endregion

        #region Common Functions
        // JSL 05/10/2022
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
                    dictToReturn = _helper.PostAsyncAPICall(APIURLHelper.APIDeficiencies, "CommonPostAPICall", dictMetadata);
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Deficiencies PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End JSL 05/10/2022
        #endregion
    }
}