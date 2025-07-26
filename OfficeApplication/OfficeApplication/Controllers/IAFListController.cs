using Newtonsoft.Json;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire] //RDBJ 11/13/2021
    public class IAFListController : Controller
    {
        // GET: IAFList
        public ActionResult Index()
        {
            APIHelper _helper = new APIHelper();

            // RDBJ 01/07/2022 Commented below code
            /*
            List<InternalAuditForm> list = new List<InternalAuditForm>();
            list = _helper.GetAllInternalAuditForm();
            ViewBag.Listdata = list.OrderByDescending(x => x.Date);
            */

            // RDBJ 01/07/2022
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            ViewBag.ShipCode = SessionManager.ShipCode;

            // RDBJ 01/22/2022 Commented below code
            /*
            List<string> data = GetAuditNotesNumberForNotes(SessionManager.ShipCode);
            ViewBag.Numbers = data;
            ViewBag.StringNumbers = String.Join(",", data.Cast<string>().ToArray());

            ViewBag.AuditNoteTypeList = new List<string>() { "ISM-Non Conformity", "ISPS-Non Conformity", "ISM-Observation", "ISPS-Observation", "MLC-Deficiency" }; //RDBJ 11/18/2021
            */
            // End RDBJ 01/08/2022

            ViewBag.AuditorsList = GetFormsPersonList(1);   // JSL 07/23/2022

            return View();
        }

        // RDBJ 01/08/2022
        [HttpPost]
        public ActionResult Index(IAF Modal)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                if (Modal != null)
                {
                    Modal.InternalAuditForm.AuditType = 1; // 1 = Internal/ 2 = External

                    if (Modal.InternalAuditForm.UniqueFormID == null)
                        Modal.InternalAuditForm.UniqueFormID = Guid.NewGuid();

                    APIResponse res = _helper.SubmitIAForm(Modal);
                    if (Convert.ToBoolean(res.result) == false)
                        ViewBag.result = AppStatic.ERROR;
                    else
                        ViewBag.result = AppStatic.SUCCESS;

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit IA Form Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return RedirectToAction("GeneralInspectionReport", "Forms");
        }
        // End RDBJ 01/08/2022

        // RDBJ 01/08/2022
        public ActionResult Details(string id, string count)
        {
            ViewBag.id = id;
            ViewBag.count = count;
            return PartialView("IAFStep2");
        }
        // End RDBJ 01/08/2022

        public ActionResult DetailsView(Guid? id)
        {
            APIHelper _helper = new APIHelper();
            IAF list = new IAF();
            list = _helper.IAFormDetailsView(id);
            
            // RDBJ 04/03/2022 Wrapped in if
            if(string.IsNullOrEmpty(list.InternalAuditForm.AuditNo))
                list.InternalAuditForm.AuditNo = "1"; // RDBJ 02/09/2022

            ViewBag.Auditor = list.InternalAuditForm.Auditor;   // RDBJ 02/09/2022

            ViewBag.AuditorsList = GetFormsPersonList(1);   // JSL 07/23/2022

            //RDBJ 11/16/2021
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            ViewBag.ShipName = list.InternalAuditForm.ShipName;
            //End RDBJ 11/16/2021

            Dictionary<string, string> response = new Dictionary<string, string>();
            List<String> data = new List<string>();
            if (list != null && list.InternalAuditForm != null)
            {
                response = _helper.GetNumberForNotes(list.InternalAuditForm.ShipName
                    , Convert.ToString(list.InternalAuditForm.UniqueFormID)); // RDBJ 01/22/2022
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        data.Add(item.Value);
                    }
                }
            }
            ViewBag.Numbers = data; // RDBJ 11/17/2021
            ViewBag.StringNumbers = String.Join(",", data.Cast<string>().ToArray()); // RDBJ 11/16/2021

            ViewBag.AuditNoteTypeList = new List<string>() { "ISM-Non Conformity", "ISPS-Non Conformity", "ISM-Observation", "ISPS-Observation", "MLC-Deficiency" }; //RDBJ 11/18/2021

            // JSL 12/31/2022
            if (list.InternalAuditForm.ShipId == null || list.InternalAuditForm.ShipId == 0)
            {
                var shipDetailsForThisFormShip = shipsList.Where(x => x.Code == list.InternalAuditForm.ShipName).FirstOrDefault();
                if (shipDetailsForThisFormShip != null)
                {
                    list.InternalAuditForm.ShipId = shipDetailsForThisFormShip.ShipId;
                }
            }
            // End JSL 12/31/2022

            return View(list);
        }

        // RDBJ 01/27/2022
        public ActionResult EditStep2Details(string id, string count)
        {
            ViewBag.id = id;
            ViewBag.count = count;
            return PartialView("IAFStep2Edit");
        }
        // End RDBJ 01/27/2022


        public ActionResult Download(string file, string name)
        {
            byte[] fileBytes = null;
            APIHelper _helper = new APIHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.DownloadAuditFile(file);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            GetFile = regex.Replace(GetFile, string.Empty);
            fileBytes = Convert.FromBase64String(GetFile);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }

        [HttpPost]
        public JsonResult AddAuditNoteResolutions(Audit_Note_Resolutions actions)
        {
            long res = 0;
            if (actions != null)
            {
                APIHelper _helper = new APIHelper();
                _helper.AddAuditNoteResolutions(actions);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAuditNoteResolution(Guid id)
        {
            List<Audit_Note_Resolutions> response = new List<Audit_Note_Resolutions>();
            if (id != null)
            {
                APIHelper _apihelper = new APIHelper();
                response = _apihelper.GetAuditNoteResolutions(id);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadAuditNoteResolutionFile(string ResolutionFileID, string name)
        {
            byte[] fileBytes = null;
            APIHelper _helper = new APIHelper();

            // RDBJ 01/27/2022 set with dictionary
            Dictionary<string, string> retDicData = _helper.GetAuditNoteResolutionFile(ResolutionFileID);
            string GetFile = Convert.ToString(retDicData["FileData"]);
            name = Convert.ToString(retDicData["FileName"]);
            // End RDBJ 01/27/2022 set with dictionary

            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            GetFile = regex.Replace(GetFile, string.Empty);
            fileBytes = Convert.FromBase64String(GetFile);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);

        }

        //RDBJ 11/13/2021
        [HttpPost]
        public JsonResult UpdateIAFAuditNotePriority(string NotesUniqueID, int PriorityWeek)
        {
            if (!string.IsNullOrEmpty(NotesUniqueID)
                && SessionManager.UserGroup != "8"  // JSL 12/21/2022
                )
            {
                APIHelper _helper = new APIHelper();
                _helper.UpdateIAFAuditNotePriority(NotesUniqueID, PriorityWeek);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/13/2021

        //RDBJ 11/17/2021
        [HttpPost]
        public void AddIAFAuditNote(AuditNote Modal)
        {
            APIHelper _aHelper = new APIHelper();
            try
            {
                if (Modal != null)
                {
                    Guid NotesUniqueID = Guid.NewGuid();

                    if (string.IsNullOrWhiteSpace(Modal.Ship))
                        Modal.Ship = SessionManager.ShipCode;

                    //Modal.isResolved = false;
                    Modal.NotesUniqueID = NotesUniqueID;
                    Modal.Priority = 12;

                    _aHelper.AddIAFAuditNote(Modal);
                    ViewBag.result = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddIAFAuditNote Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
        }
        //End RDBJ 11/17/2021

        //RDBJ 11/19/2021
        [HttpPost]
        public JsonResult IAFAutoSave(IAF Modal)
        {
            APIHelper _aHelper = new APIHelper();
            IAF IAFModal = new IAF();
            try
            {
                if (Modal != null
                    && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                    )
                {
                    //RDBJ 11/24/2021 Wrapped in if
                    IAFModal = Modal;

                    if (IAFModal.InternalAuditForm.UniqueFormID == null)
                    {
                        IAFModal.InternalAuditForm.UniqueFormID = Guid.NewGuid();
                        IAFModal.InternalAuditForm.FormVersion = 1;
                    }

                    if (IAFModal.AuditNote == null)
                        IAFModal.AuditNote = new List<AuditNote>();

                    _aHelper.IAFAutoSave(IAFModal);
                    ViewBag.result = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return Json(new { UniqueFormID = IAFModal.InternalAuditForm.UniqueFormID});
        }
        //End RDBJ 11/19/2021

        //RDBJ 11/23/2021
        public JsonResult GetNumberForNotes(string Ship
            , string UniqueFormID // RDBJ 01/22/2022
            )
        {
            APIHelper _helper = new APIHelper();

            // RDBJ 01/08/2022 commented below code
            /*
            Dictionary<string, string> response = new Dictionary<string, string>();
            List<String> data = new List<string>();
            if (!string.IsNullOrEmpty(Ship))
            {
                response = _helper.GetNumberForNotes(Ship);
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        data.Add(item.Value);
                    }
                }
            }
            */

            // JSL 12/19/2022 commented
            /*
            List<string> data = GetAuditNotesNumberForNotes(Ship
               , UniqueFormID); // RDBJ 01/22/2022
            */
            // End JSL 12/19/2022 commented

            // JSL 12/19/2022
            List<string> data = new List<string>();
            if (SessionManager.UserGroup != "8")
            {
                data = GetAuditNotesNumberForNotes(Ship
                , UniqueFormID); // RDBJ 01/22/2022
            }
            // End JSL 12/19/2022
           
            return Json(new { Numbers = data });
        }
        //End RDBJ 11/23/2021

        //RDBJ 11/23/2021
        public void SubmitIAForm(InternalAuditForm Modal)
        {
            APIHelper _aHelper = new APIHelper();
            IAF IAFModal = new IAF();
            IAFModal.InternalAuditForm = Modal;
            IAFModal.AuditNote = new List<AuditNote>();

            try
            {
                if (Modal != null)
                {
                    Guid UniqueFormID = Guid.NewGuid();
                    IAFModal.InternalAuditForm.UniqueFormID = UniqueFormID;
                    IAFModal.InternalAuditForm.FormVersion = 1;

                    if (string.IsNullOrWhiteSpace(Modal.ShipName))
                        Modal.ShipName = SessionManager.ShipCode;

                    _aHelper.SubmitIAForm(IAFModal);
                    ViewBag.result = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitIAForm Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
        }
        //End RDBJ 11/23/2021

        //RDBJ 11/24/2021
        [HttpPost]
        public ActionResult UpdateAdditionalAndCloseStatus(string id, string IsAdditionalAndClosedStatus, string IsAdditionalAndClosed)
        {
            bool res = false;
            if (id != "" 
                && SessionManager.UserGroup != "8"  // JSL 12/19/2022
                )
            {
                APIHelper _helper = new APIHelper();
                res = _helper.UpdateAdditionalAndCloseStatus(id, Convert.ToBoolean(IsAdditionalAndClosedStatus), Convert.ToBoolean(IsAdditionalAndClosed));
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/24/2021


        //RDBJ 11/24/2021
        public ActionResult GetAuditDetails(Guid? id)
        {
            APIHelper _helper = new APIHelper();
            IAF Modal = _helper.IAFormDetailsView(id);
            Modal.InternalAuditForm.AuditNo = "1";    // RDBJ 02/09/2022
            return Json(Modal.InternalAuditForm, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/24/2021

        //RDBJ 11/24/2021
        [HttpPost]
        public ActionResult RemoveAuditsOrAuditNotes(string id
            , string IsAudit //RDBJ 11/25/2021
            )
        {
            bool res = false;
            if (id != "")
            {
                APIHelper _helper = new APIHelper();
                res = _helper.RemoveAuditsOrAuditNotes(id, Convert.ToBoolean(IsAudit)); //RDBJ 11/25/2021 added IsAudit
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/24/2021

        #region Common Functions

        // RDBJ2 02/23/2022
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
                    dictToReturn = _helper.PostAsyncAPICall(APIURLHelper.APIIAF, "CommonPostAPICall", dictMetadata);
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("IAFList PerformAction : " + straction + " Error : " + ex.Message);
                }
            }
            
            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ2 02/23/2022

        // JSL 07/23/2022
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
        // End JSL 07/23/2022

        //RDBJ 11/16/2021
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
        //End RDBJ 11/16/2021

        // RDBJ 01/08/2022
        public List<string> GetAuditNotesNumberForNotes(string shipCode
            , string UniqueFormID   // RDBJ 01/22/2022
            )
        {
            APIHelper _helper = new APIHelper();
            Dictionary<string, string> response = new Dictionary<string, string>();
            List<string> data = new List<string>();
            if (!string.IsNullOrEmpty(shipCode))
            {
                response = _helper.GetNumberForNotes(shipCode
                    , UniqueFormID  // RDBJ 01/22/2022
                    );
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        data.Add(item.Value);
                    }
                }
            }
            return data;
        }
        // End RDBJ 01/08/2022

        //RDBJ 11/29/2021
        public JsonResult GetSMSReferenceData()
        {
            List<TreeObject> response = new List<TreeObject>();
            APIHelper _helper = new APIHelper();

            //RDBJ 11/30/2021
            var data = _helper.GetSMSReferenceData();
            response = data.Select(x => new TreeObject()
            {
                Id = x.SMSReferenceId,
                ParentId = x.SMSReferenceParentId == null ? 0 : x.SMSReferenceParentId,
                Number = x.Number,
                Reference = x.Reference,
            }).ToList();

            var treelist = FlatToHierarchy(response, 0); // RDBJ 12/01/2021 return with array node
            //End RDBJ 11/30/2021

            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/29/2021

        //RDBJ 11/29/2021
        public JsonResult GetSSPReferenceData()
        {
            List<TreeObject> response = new List<TreeObject>();
            APIHelper _helper = new APIHelper();

            //RDBJ 11/30/2021
            var data = _helper.GetSSPReferenceData();
            response = data.Select(x => new TreeObject()
            {
                Id = x.SSPReferenceId,
                ParentId = x.SSPReferenceParentId == null ? 0 : x.SSPReferenceParentId,
                Number = x.Number,
                Reference = x.Reference,
            }).ToList();

            var treelist = FlatToHierarchy(response, 0);  // RDBJ 12/01/2021 return with array node
            //End RDBJ 11/30/2021

            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/29/2021

        //RDBJ 11/29/2021
        public JsonResult GetMLCRegulationTree()
        {
            List<TreeObject> response = new List<TreeObject>();
            APIHelper _helper = new APIHelper();

            //RDBJ 11/30/2021
            var data = _helper.GetMLCRegulationTree();
            response = data.Select(x => new TreeObject()
            {
                Id = x.MLCRegulationId,
                ParentId = x.MLCRegulationParentId == null ? 0 : x.MLCRegulationParentId,
                Number = x.Number,
                Reference = x.Regulation,
            }).ToList();

            var treelist = FlatToHierarchy(response, 0);  // RDBJ 12/01/2021 return with array node
            //End RDBJ 11/30/2021

            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 11/29/2021

        // RDBJ 12/01/2021 return with array node // RDBJ 11/30/2021 
        public TreeObject[] FlatToHierarchy(List<TreeObject> list, int parentId = 0)
        {

            return (from i in list
                    where i.ParentId == parentId
                    select new TreeObject
                    {
                        Id = i.Id,
                        ParentId = i.ParentId,
                        Number = i.Number,
                        Reference = i.Number + " " + i.Reference, // RDBJ 12/01/2021 merger number and reference
                        Nodes = FlatToHierarchy(list, i.Id).ToArray()
                    }).ToArray();
        }
        //End RDBJ 11/30/2021


        //RDBJ 12/01/2021 Commented use for later
        /*
        //Recursion method for recursively get all child nodes
        public void GetTreeview(List<TreeObject> list, TreeObject current, ref List<TreeObject> returnList)
        {
            //get child of current item
            var childs = list.Where(a => a.ParentId == current.Id).ToList();
            //current.Children = new List<TreeObject>();
            //current.Children.AddRange(childs);
            foreach (var i in childs)
            {
                GetTreeview(list, i, ref returnList);
            }
        }

        public List<TreeObject> BuildTree(List<TreeObject> list)
        {
            List<TreeObject> returnList = new List<TreeObject>();
            //find top levels items
            var topLevels = list.Where(a => a.ParentId == list.OrderBy(b => b.ParentId).FirstOrDefault().ParentId);
            returnList.AddRange(topLevels);
            foreach (var i in topLevels)
            {
                GetTreeview(list, i, ref returnList);
            }
            return returnList;
        }
        */
        #endregion
    }
}