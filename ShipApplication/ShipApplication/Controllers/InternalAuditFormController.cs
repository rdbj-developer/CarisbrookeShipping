using Newtonsoft.Json;
using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    [AuthorizeFilter] //RDBJ 10/17/2021
    public class InternalAuditFormController : Controller
    {
        // GET: InternalAuditForm
        public ActionResult Index()
        {
            IAFHelper _helper = new IAFHelper();    // JSL 05/22/2022
            ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson();
            ViewBag.Inspector = ServerModal.IsInspector;
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            //ViewBag.ShipName = SessionManager.ShipName; // RDBJ 02/02/2022 commented this line
            ViewBag.ShipCode = SessionManager.ShipCode; // RDBJ 02/02/2022

            // JSL 05/22/2022
            bool IsLatestReferencesFound = false;
            var SMSdata = _helper.GetSMSReferenceData();
            var SSPdata = _helper.GetSSPReferenceData();
            var MLCdata = _helper.GetMLCRegulationTree();

            if ((SMSdata != null && SMSdata.Count > 0)
                || (SSPdata != null && SSPdata.Count > 0)
                || (MLCdata != null && MLCdata.Count > 0)
                )
            {
                IsLatestReferencesFound = true;
            }

            ViewBag.IsLatestReferencesFound = IsLatestReferencesFound;
            // End JSL 05/22/2022

            return View();
        }
        [HttpPost]
        public ActionResult Index(IAF Modal)
        {
            IAFHelper _helper = new IAFHelper();
            //var shiplist = LoadShipsList();   // RDBJ 04/02/2022 commented this line
            try
            {
                if (Modal != null)
                {
                    // RDBJ 04/02/2022 commented below code
                    /*
                    if (shiplist != null && shiplist.Count > 0)
                    {
                        var objShip = shiplist.Where(x => x.Name == Modal.InternalAuditForm.ShipName).FirstOrDefault();
                        if (objShip != null)
                        {
                            Modal.InternalAuditForm.ShipName = objShip.Code;
                            Modal.InternalAuditForm.ShipId = objShip.ShipId;
                        }
                    }
                    Modal.InternalAuditForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.InternalAuditForm.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Modal.InternalAuditForm.IsSynced = false;
                    Modal.InternalAuditForm.FormVersion = 1;
                    Modal.InternalAuditForm.AuditType = 1; //RDBJ 11/25/2021
                    Modal.InternalAuditForm.IsAdditional = false; //RDBJ 11/25/2021
                    Modal.InternalAuditForm.IsClosed = true; //RDBJ 11/25/2021
                    Modal.InternalAuditForm.isDelete = 0; //RDBJ 11/25/2021
                    */
                    // RDBJ 04/02/2022  commented below code

                    Modal.InternalAuditForm.FormVersion = Modal.InternalAuditForm.FormVersion + Convert.ToDecimal(0.01);    // JSL 05/01/2022

                    bool res = _helper.SaveIAFDataInLocalDB(Modal);
                    if (res == false)
                        ViewBag.result = AppStatic.ERROR;
                    else
                        ViewBag.result = AppStatic.SUCCESS;

                    //RDBJ 11/08/2021
                    TempData["data"] = Modal;
                    List<CSShipsModal> shipsList = LoadShipsList();
                    TempData["Name"] = shipsList.Where(x => x.Code == Modal.InternalAuditForm.ShipName).Select(x => x.Name).FirstOrDefault();
                    return RedirectToAction("SendIAFReportMail", "Mail", new
                    {
                        Modal = TempData["data"]
                    });
                    //End RDBJ 11/08/2021
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit IA Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("Index", "Deficiencies");
        }
        public ActionResult Details(string id, string count)
        {
            ViewBag.id = id;
            ViewBag.count = count;
            return PartialView("IAFStep2");
        }

        // RDBJ 05/02/2022
        public ActionResult EditStep2Details(string id, string count)
        {
            ViewBag.id = id;
            ViewBag.count = count;
            return PartialView("IAFStep2Edit");
        }
        // End RDBJ 05/02/2022

        // RDBJ 04/02/2022
        [HttpPost]
        public JsonResult IAFAutoSave(IAF Modal)
        {
            IAFHelper _helper = new IAFHelper();
            IAF IAFModal = new IAF();
            string strUniqueFormID = string.Empty; // RDBJ 04/02/2022
            try
            {
                if (Modal != null)
                {
                    IAFModal = Modal;
                    if (IAFModal.InternalAuditForm.FormVersion == 0)
                    {
                        IAFModal.InternalAuditForm.FormVersion = 1;
                    }
                    else
                    {
                        Modal.InternalAuditForm.FormVersion = Modal.InternalAuditForm.FormVersion + Convert.ToDecimal(0.01);
                    }

                    if (IAFModal.AuditNote == null)
                        IAFModal.AuditNote = new List<AuditNote>();

                    strUniqueFormID = _helper.IAFAutoSave(IAFModal);
                    ViewBag.result = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(new { UniqueFormID = strUniqueFormID, FormVersion = Modal.InternalAuditForm.FormVersion });
        }
        // End RDBJ 04/02/2022

        public List<CSShipsModal> LoadShipsList()
        {
            SettingsHelper _lhelper = new SettingsHelper();
            GIRTableHelper _laHelper = new GIRTableHelper(); //RDBJS 09/17/2021
            List<CSShipsModal> shipsList = null;

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
        [HttpPost]
        public ActionResult GetNumberForNotes(string Ship
            ,string UniqueFormID)    // RDBJ 02/02/2022
        {
            List<String> data = new List<string>();
            Dictionary<string, string> response = new Dictionary<string, string>();
            IAFHelper _lhelper = new IAFHelper();
            response = _lhelper.GetNumberForNotes(Ship
                , UniqueFormID);    // RDBJ 02/02/2022
            foreach (var item in response)
            {
                data.Add(item.Key + ":" + item.Value);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAuditNoteResolution(Guid? id)
        {
            List<Audit_Note_Resolutions> response = new List<Audit_Note_Resolutions>();
            if (id != null)
            {
                IAFHelper _lhelper = new IAFHelper();
                response = _lhelper.GetAuditNoteResolutions_Local_DB(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddAuditNoteResolutions(Audit_Note_Resolutions actions)
        {
            long res = 0;
            if (actions != null)
            {
                IAFHelper _lhelper = new IAFHelper();
                _lhelper.AddAuditNoteResolutions_Local_DB(actions);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // RDBJ 01/27/2022
        public ActionResult DownloadAuditNoteCommentFile(string CommentFileID, string name)
        {
            byte[] fileBytes = null;
            IAFHelper _lhelper = new IAFHelper();
            Dictionary<string, string> retDicData = _lhelper.GetAuditNoteCommentFile_Local_DB(CommentFileID);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);

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
        // End RDBJ 01/27/2022

        public ActionResult DownloadAuditNoteResolutionFile(string ResolutionFileID, string name)
        {
            byte[] fileBytes = null;
            IAFHelper _lhelper = new IAFHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _lhelper.GetAuditNoteResolutionFile_Local_DB(ResolutionFileID);
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

        public ActionResult Download(string file, string name)
        {
            byte[] fileBytes = null;

            IAFHelper _lhelper = new IAFHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _lhelper.DownloadAuditFile_Local_DB(file);
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

        // RDBJ 02/02/2022
        [HttpPost]
        public ActionResult RemoveAuditsOrAuditNotes(string id
            , string UniqueFormID   // RDBJ 04/02/2022
            , string IsAudit //RDBJ 11/25/2021
            )
        {
            bool res = false;
            if (id != "")
            {
                IAFHelper _lhelper = new IAFHelper();
                res = _lhelper.RemoveAuditsOrAuditNotes(id, UniqueFormID, Convert.ToBoolean(IsAudit)); //RDBJ 11/25/2021 added IsAudit
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 02/02/2022

        // RDBJ2 03/31/2022
        public JsonResult PerformAction(string jsonmetadata, string straction)
        {
            IAFHelper _helper = new IAFHelper();

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
                    LogHelper.writelog("Internal Audit PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ2 03/31/2022

        #region Reference TreeData
        // JSL 05/22/2022
        [HttpGet]
        public JsonResult GetSMSReferenceData()
        {
            List<TreeObject> response = new List<TreeObject>();
            IAFHelper _helper = new IAFHelper();

            var data = _helper.GetSMSReferenceData();
            response = data.Select(x => new TreeObject()
            {
                Id = x.SMSReferenceId,
                ParentId = x.SMSReferenceParentId == null ? 0 : x.SMSReferenceParentId,
                Number = x.Number,
                Reference = x.Reference,
            }).ToList();

            var treelist = FlatToHierarchy(response, 0); 

            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        // End JSL 05/22/2022

        // JSL 05/22/2022
        [HttpGet]
        public JsonResult GetSSPReferenceData()
        {
            List<TreeObject> response = new List<TreeObject>();
            IAFHelper _helper = new IAFHelper();

            var data = _helper.GetSSPReferenceData();
            response = data.Select(x => new TreeObject()
            {
                Id = x.SSPReferenceId,
                ParentId = x.SSPReferenceParentId == null ? 0 : x.SSPReferenceParentId,
                Number = x.Number,
                Reference = x.Reference,
            }).ToList();

            var treelist = FlatToHierarchy(response, 0);  

            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        // End JSL 05/22/2022

        // JSL 05/22/2022
        [HttpGet]
        public JsonResult GetMLCRegulationTree()
        {
            List<TreeObject> response = new List<TreeObject>();
            IAFHelper _helper = new IAFHelper();

            var data = _helper.GetMLCRegulationTree();
            response = data.Select(x => new TreeObject()
            {
                Id = x.MLCRegulationId,
                ParentId = x.MLCRegulationParentId == null ? 0 : x.MLCRegulationParentId,
                Number = x.Number,
                Reference = x.Regulation,
            }).ToList();

            var treelist = FlatToHierarchy(response, 0); 

            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        // End JSL 05/22/2022

        // JSL 05/22/2022
        public TreeObject[] FlatToHierarchy(List<TreeObject> list, int parentId = 0)
        {

            return (from i in list
                    where i.ParentId == parentId
                    select new TreeObject
                    {
                        Id = i.Id,
                        ParentId = i.ParentId,
                        Number = i.Number,
                        Reference = i.Number + " " + i.Reference,
                        Nodes = FlatToHierarchy(list, i.Id).ToArray()
                    }).ToArray();
        }
        // End JSL 05/22/2022
        #endregion

    }
}