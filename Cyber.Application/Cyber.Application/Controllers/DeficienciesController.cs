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
        public ActionResult GetShipGISIReports(string code)
        {
            APIHelper _helper = new APIHelper();
            List<Deficiency_GISI_Report> Modal = _helper.GetShipGISIReports(code);
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGISIReportsDeficiencies(string ship, string FormID, string Type)
        {
            APIHelper _helper = new APIHelper();
            Deficiency_GISI_Report req = new Deficiency_GISI_Report();
            req.Ship = ship;
            req.FormID = Utility.ToLong(FormID);
            req.Type = Type;
            List<GIRDataList> Modal = _helper.GetGISIDeficiencies(req);
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateDeficiencyPriority(int DefID, int PriorityWeek)
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Audit Degiciencies
        public ActionResult GetAuditShips()
        {
            APIHelper _helper = new APIHelper();
            List<Deficiency_Audit_Ships> AuditList = _helper.GetAuditShips();
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AuditDeficiencies()
        {
            return View();
        }
        public ActionResult GetShipAudits(string code)
        {
            APIHelper _helper = new APIHelper();
            List<Deficiency_Ship_Audits> AuditList = _helper.GetShipAudits(code);
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAuditDetails(int id)
        {
            APIHelper _helper = new APIHelper();
            IAF Modal = _helper.IAFormDetailsView(id);
            List<AuditDetail> AuditList = new List<AuditDetail>();
            if (Modal != null && Modal.AuditNote != null)
            {
                List<AuditNote> NotesList = Modal.AuditNote.Where(x => x.Type == "ISM-Non Conformity" || x.Type == "ISPS-Non Conformity" || x.Type == "ISM-Observation" || x.Type == "ISPS-Observation").ToList();
                AuditList = NotesList.Select(x => new AuditDetail()
                {
                    NoteID = x.AuditNotesId,
                    Type = x.Type == "ISM-Non Conformity" || x.Type == "ISPS-Non Conformity" ? "NCN" : x.Type == "ISM-Observation" || x.Type == "ISPS-Observation" ? "OBS" : string.Empty,
                    Deficiency = x.BriefDescription,
                    Reference = x.Reference,
                    DueDate = Utility.ToDateTimeStr(x.TimeScale),
                    IsResolved = x.isResolved,
                }).ToList();
            }
            return Json(AuditList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateAuditDeficiencies(string id, string isClose)
        {
            bool res = false;
            if (id != "")
            {
                APIHelper _helper = new APIHelper();
                res = _helper.UpdateAuditDeficiencies(Utility.ToLong(id), Convert.ToBoolean(isClose));
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddAuditDeficiencyComments(Audit_Deficiency_Comments Modal)
        {
            APIHelper _helper = new APIHelper();
            bool res = _helper.AddAuditDeficiencyComments(Modal);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAuditDeficiencyComments(long NoteID)
        {
            APIHelper _helper = new APIHelper();
            List<Audit_Deficiency_Comments> Comments = _helper.GetAuditDeficiencyComments(NoteID);
            return Json(Comments, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAuditDeficiencyCommentFiles(long id)
        {
            APIHelper _helper = new APIHelper();
            List<Audit_Deficiency_Comments_Files> response = _helper.GetAuditDeficiencyCommentFiles(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadCommentFile(int CommentFileID, string name)
        {
            APIHelper _helper = new APIHelper();
            string GetFile = _helper.GetAuditCommentFile(CommentFileID);
            string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
            LogHelper.writelog("---------" + filepath + "----------");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        #endregion
    }
}