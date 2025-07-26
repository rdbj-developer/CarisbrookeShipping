using Newtonsoft.Json;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static OfficeApplication.BLL.Modals.AppStatic;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class DocumentsController : Controller
    {
        // GET: Documents
        public ActionResult Index()
        {
            if (TempData["ErrorMsg"] != null && Utility.ToString(TempData["ErrorMsg"]) != "")
                ViewBag.ErrorMsg = TempData["ErrorMsg"];
            TempData["ErrorMsg"] = null;
            TempData.Keep();
            return View();
        }
        public ActionResult ViewPDF(string id)
        {
            APIHelper _helper = new APIHelper();
            string path = string.Empty;
            string mimeType = string.Empty;
            string documentTitle = string.Empty;
            try
            {
                DocumentModal Modal = _helper.GetDocumentBYID(id);
                if (Modal != null && !string.IsNullOrEmpty(Modal.Path))
                {
                    documentTitle = Modal.Title;
                    Modal.Path = Modal.Path.Replace("&amp;", "&");
                    path = Path.Combine(Modal.Path);
                    mimeType = MimeMapping.GetMimeMapping(Path.GetFileName(path));
                    LogHelper.writelog("path1" + path + "----" + mimeType);
                    if (!System.IO.File.Exists(path))
                    {
                        if (path.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                            path = path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                        if (path.Contains(@"C:\Carisbrooke Shipping Ltd\Saved Forms"))
                            path = path.Replace(@"C:\Carisbrooke Shipping Ltd\Saved Forms", @"\Repository\Saved Forms");
                        path = Server.MapPath(@"~\" + path);
                        LogHelper.writelog("path2" + path + "----" + mimeType);
                    }
                    //System.Diagnostics.Process.Start(path);
                    if (Modal.Type != "PDF")
                    {
                        if (System.IO.File.Exists(path))
                        {
                            string filename = Path.GetFileName(path);
                            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                            return File(fileBytes, mimeType, filename);
                        }
                    }
                }
                LogHelper.writelog("path" + path + "----" + mimeType);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (System.IO.File.Exists(path))
                    {
                        mimeType = string.IsNullOrWhiteSpace(mimeType) ? "application/octet-stream" : mimeType;
                        return File(path, mimeType);
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            TempData["ErrorMsg"] = documentTitle + " document is not exist.";
            TempData.Keep();
            return RedirectToAction("Index", "Documents");
        }
        public JsonResult ViewPDFPath(string id)
        {
            APIHelper _helper = new APIHelper();
            string path = string.Empty;
            string mimeType = string.Empty;
            try
            {
                DocumentModal Modal = _helper.GetDocumentBYID(id);
                if (Modal != null && !string.IsNullOrEmpty(Modal.Path))
                {
                    Modal.Path = Modal.Path.Replace("&amp;", "&");
                    path = Path.Combine(Modal.Path);
                    mimeType = MimeMapping.GetMimeMapping(Path.GetFileName(path));
                    if (!System.IO.File.Exists(path))
                    {
                        path = path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");
                        path = Server.MapPath(@"~\" + path);
                    }
                    if (Modal.Type != "PDF")
                    {
                        return Json(path, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return Json(path, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllDocuments()
        {
            APIHelper _helper = new APIHelper();
            List<DocumentModal> retDocList = new List<DocumentModal>();
            List<DocumentModal> DocList = _helper.GetAllDocumentsWithRAData(SessionManager.ShipCode);
            if (DocList != null && DocList.Count > 0)
            {
                foreach (var item in DocList)
                {
                    item.Path = JsonConvert.SerializeObject(item.Path);
                }

                DocList = DocList.OrderBy(x => x.SectionType).ThenBy(x => x.DocNo).ToList();

                // JSL 02/08/2023 used this thing for the showing in order for the daniels wish
                retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "ism").ToList());
                retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "isps").ToList());
                retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "mlc").ToList());
                retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "cyber security").ToList());
                retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "it").ToList());
                // End JSL 02/08/2023 used this thing for the showing in order for the daniels wish
            }
            //return Json(DocList, JsonRequestBehavior.AllowGet);   // JSL 02/08/2023 commented
            return Json(retDocList, JsonRequestBehavior.AllowGet);  // JSL 02/08/2023
        }
        public FileResult DownloadServiceSetup()
        {
            string path = Server.MapPath(@"~\Service\CarisbrookeOpenFileService.zip");
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
        public ActionResult RiskAssessmentReviewLog()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = _helper.GetAllShips();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX")).ToList();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            ViewBag.ShipDatas = shipsList;
            return View();
        }

        [HttpGet]
        public string GetRiskAssessmentReviewLogData(string shipCode)
        {
            APIHelper _helper = new APIHelper();

            var docList = _helper.GetAllRiskAssessmentReviewLog(shipCode);

            if (docList != null)
            {
                foreach (var item in docList)
                {
                    if (!string.IsNullOrWhiteSpace(item.Stage4RiskFactor))
                    {
                        switch (item.Stage4RiskFactor)
                        {
                            case "1":
                            case "Very Low Risk":
                                item.RiskFactorColour = "bg-green-active";
                                break;
                            case "2":
                            case "Low Risk":
                                item.RiskFactorColour = "bg-green-active";// "bg-lime";
                                break;
                            case "3":
                            case "Medium Risk":
                                item.RiskFactorColour = "bg-yellowtext";
                                break;
                            case "4":
                            case "High Risk":
                                item.RiskFactorColour = "bg-red";// "bg-yellow-active";
                                break;
                            case "5":
                            case "Very High Risk":
                                item.RiskFactorColour = "bg-red";
                                break;
                            default:
                                break;
                        }
                        item.Stage4RiskFactor = Utility.SplitCamelCase(Convert.ToString(Utility.ToEnum<RiskFactorType>(item.Stage4RiskFactor.Replace(" ", ""))));
                    }
                }
            }
            return ConvertViewToString("_ReviewlogDocList", docList);
        }

        #region Convert Partials As Html
        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                ViewBag.DocList = model;
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
        #endregion

        #region Cybersecurity view
        public ActionResult CybersecurityRisksAssessmentView()
        {
            CybersecurityRisksAssessmentModal Modal = new CybersecurityRisksAssessmentModal();
            try
            {

                APIHelper _apihelper = new APIHelper();
                Modal = _apihelper.GetCybersecurityRisksAssessmentData(SessionManager.ShipCode);
                ViewBag.HardwareList = _apihelper.GetAssetManagmentHardwareId(SessionManager.ShipCode);

            }
            catch (Exception ex)
            {
                LogHelper.writelog("CybersecurityRisksAssessmentView " + ex.Message);
            }
            return View(Modal);
        }       
        #endregion
    }
}