using Newtonsoft.Json;
using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    [AuthorizeFilter] //RDBJ 10/17/2021
    public class DocumentsController : Controller
    {
        // GET: Documents
        [AuthorizeFilter]
        public ActionResult Index()
        {
            //APIHelper _helper = new APIHelper();
            //List<DocumentModal> DocList = _helper.GetAllDocuments();
            //return View(DocList);
            if (TempData["ErrorMsg"] != null && Utility.ToString(TempData["ErrorMsg"]) != "")
                ViewBag.ErrorMsg = TempData["ErrorMsg"];
            TempData["ErrorMsg"] = null;
            TempData.Keep();
            return View();
        }
        public ActionResult ViewPDF(string id)
        {

            DocumentModal Modal = new DocumentModal();
            string path = string.Empty;
            string mimeType = string.Empty;
            string documentTitle = string.Empty;
            try
            {
                DocumentTableHelper _helper = new DocumentTableHelper();
                Modal = _helper.GetDocumentBYID(id);
                if ((Modal == null || string.IsNullOrWhiteSpace(Modal.Path)) && Utility.CheckInternet())
                {
                    APIHelper _apihelper = new APIHelper();
                    Modal = _apihelper.GetDocumentBYID(id);
                }
                LogHelper.writelog(id);
                //DocumentModal Modal = _helper.GetDocumentBYID(id);
                if (Modal != null && !string.IsNullOrEmpty(Modal.Path))
                {
                    documentTitle = Modal.Title;
                    Modal.Path = Modal.Path.Replace("&amp;", "&");
                    path = Path.Combine(Modal.Path);
                    mimeType = MimeMapping.GetMimeMapping(Path.GetFileName(path));
                    if (!System.IO.File.Exists(path))
                    {
                        try
                        {
                            if (path.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                                path = path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                            if (path.Contains(@"C:\Carisbrooke Shipping Ltd\Saved Forms"))
                                path = path.Replace(@"C:\Carisbrooke Shipping Ltd\Saved Forms", @"\Repository\Saved Forms");

                            path = Server.MapPath(@"~\" + path);
                        }
                        catch (Exception)
                        {
                        }
                    }
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
            DocumentTableHelper _helper = new DocumentTableHelper();
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
        public ActionResult CreatePDF(string path)
        {
            DocumentTableHelper _helper = new DocumentTableHelper();
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
            return File(path, mimeType);
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
        public ActionResult AddDocument()
        {
            return View();
        }
        public JsonResult GetAllDocuments()
        {
            try
            {
                List<DocumentModal> DocList = new List<DocumentModal>();
                List<DocumentModal> retDocList = new List<DocumentModal>(); // JSL 02/11/2023
                // Get Data From Local
                DocumentTableHelper _helper = new DocumentTableHelper();
                DocList = _helper.GetAllDocumentsFromLocalDB(SessionManager.ShipCode);

                // JSL 02/25/2023 commented do not want to take data from Server if it is null
                /*if (DocList == null || DocList.Count == 0)
                {
                    // Get Data Via API
                    APIHelper _aHelper = new APIHelper();
                    DocList = _aHelper.GetAllDocuments(SessionManager.ShipCode);
                }*/
                // End JSL 02/25/2023 commented do not want to take data from Server if it is null

                if (DocList != null && DocList.Count > 0)
                {
                    foreach (var item in DocList)
                    {
                        item.Path = JsonConvert.SerializeObject(item.Path);
                    }
                    DocList = DocList.OrderBy(x => x.SectionType).ThenBy(x => Utility.ToLong(x.DocNo)).DistinctBy(x => x.DocumentID).Where(x => x.Title != "JSL").ToList(); //RDBJ 10/01/2021 //RDBJ 09/27/2021 Updated DistinctBy for avoid Duplication in SHIPS 

                    // JSL 02/11/2023 used this thing for the showing in order for the daniels wish
                    /*retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "ism").ToList());
                    retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "isps").ToList());
                    retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "mlc").ToList());
                    retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "cyber security").ToList());
                    retDocList.AddRange(DocList.Where(x => x.SectionType.ToLower() == "it").ToList());*/
                    // End JSL 02/11/2023 used this thing for the showing in order for the daniels wish
                }
                return Json(DocList, JsonRequestBehavior.AllowGet); // JSL 02/11/2023 commented
                //return Json(retDocList, JsonRequestBehavior.AllowGet);  // JSL 02/11/2023
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocuments " + ex.Message);
                return null;
            }
        }

        #region Document Forms
        [AuthorizeFilter]
        public ActionResult RiskAssessmentReviewLog()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = _helper.GetAllShipsFromJson();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList.Add(new CSShipsModal
            {
                Code = "Office",
                Name = "Office"
            });
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            ViewBag.ShipDatas = shipsList;

            // JSL 12/16/2022
            //List<RiskAssessmentReviewLog> retlistRAFLog = GetRiskAssessmentReviewLogDataByShipCode(SessionManager.ShipCode);
            //ViewBag.DocList = retlistRAFLog;
            // End JSL 12/16/2022

            return View();
        }

        // JSL 12/24/2022
        public ActionResult _RAFLogs(string shipCode)
        {
            List<RiskAssessmentReviewLog> retlistRAFLog = GetRiskAssessmentReviewLogDataByShipCode(SessionManager.ShipCode);
            ViewBag.DocList = retlistRAFLog;

            return PartialView("_RAFLogs", retlistRAFLog);
        }
        // End JSL 12/24/2022

        [HttpGet]
        public string GetRiskAssessmentReviewLogData(string shipCode)
        {
            //APIHelper _helper = new APIHelper();

            //var docList = _helper.GetAllRiskAssessmentReviewLog(shipCode);
            RiskAssessmentHelper _helper = new RiskAssessmentHelper();
            var docList = _helper.GetAllRiskAssessmentReviewLogFromLocalDB(SessionManager.ShipCode);

            // JSL 06/22/2022 this is use for remove duplication data
            if (shipCode.ToLower() == "vis")
            {
                docList = docList.Where(x => x.ShipName.ToLower() == "onego isle").ToList();
            }
            // End JSL 06/22/2022 this is use for remove duplication data

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

        // JSL 12/12/2022
        public List<RiskAssessmentReviewLog> GetRiskAssessmentReviewLogDataByShipCode(string shipCode)
        {
            List<RiskAssessmentReviewLog> retlistRAFLog = new List<RiskAssessmentReviewLog>();
            RiskAssessmentHelper _helper = new RiskAssessmentHelper();
            try
            {
                var docList = _helper.GetAllRiskAssessmentReviewLogFromLocalDB(SessionManager.ShipCode);

                if (shipCode.ToLower() == "vis")
                {
                    docList = docList.Where(x => x.ShipName.ToLower() == "onego isle").ToList();
                }

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
                    retlistRAFLog = docList;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRiskAssessmentReviewLogDataByShipCode " + ex.InnerException.ToString());
            }

            return retlistRAFLog;
        }
        // End JSL 12/12/2022
        #endregion

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

        #region AssetManagmentEquipmentList
        [AuthorizeFilter]
        public ActionResult AssetManagmentEquipmentList()
        {
            AssetManagmentEquipmentListModal Modal = new AssetManagmentEquipmentListModal();
            try
            {
                List<string> criticalityList = new List<string>();
                criticalityList.Add("Low");
                criticalityList.Add("Medium");
                criticalityList.Add("High");
                criticalityList.Add("Safety Critical");
                TempData["CriticalityList"] = criticalityList;
                if (TempData["AMERes"] != null)
                    ViewBag.result = TempData["AMERes"];
                AssetManagmentEquipmentListHelper _helper = new AssetManagmentEquipmentListHelper();
                Modal = _helper.GetAssetManagmentEquipmentData_LocalDB(SessionManager.ShipCode);
                if (Modal == null || Modal.AssetManagmentEquipmentOTListModel == null || Modal.AssetManagmentEquipmentOTListModel.Count <= 0)
                {
                    Modal.AssetManagmentEquipmentOTListModel = new List<AssetManagmentEquipmentOTListModel>();
                    Modal.AssetManagmentEquipmentOTListModel.AddRange(new List<AssetManagmentEquipmentOTListModel> {
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="SAT C 2 PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Gyro",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Gyro repeaters",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="2 WAY VHF(3PCS)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="MF/HF transceiver",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Speed log",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Echo sounder",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="GPS PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="EPIRB",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Talk back system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ECDIS BACK-UP",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Radar No2",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ECDIS MAIN",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Radar No1",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="VHF PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="VHF transceivers(Duplicate)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Public address system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="NAVTEX PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ARPA",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Course Recorder",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="GPS 1 GPS 2",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Off course alarm",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="AIS",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Weather facsimile",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Long Range Identification and Tracking systems (LRIT)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Internal Telephone system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Sound Powered telephone System",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="SSAS (SHIP SECURITY ALERT SYSTEM)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Inmarsat C system(DUPLICATE)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="SAT C  1 PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Voyage Data Recorder (VDR or SVDR)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Inmarsat C System(PRIMARY)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Auto Pilot",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="VHF transceivers(PRIMARY)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ECHO SOUNDER PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="2 SART",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Ballast Water Management System",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Auxiliary engine control/alarm systems" ,OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="CPP systems ",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Fire detection system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Oily water separator",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Alarm & monitoring system (AMS)" ,OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Auto exchange phone systems",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Bow thruster Control system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Eefting" ,OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Emergency generator Control Unit",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Besi or other valve control system" ,OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Steering control system" ,OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Citadel phone docking station" ,OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Bridge manoeuvring display" ,OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},

                    }
                    );
                }

                //else { 
                //Modal.AssetManagmentEquipmentOTListModel.Select(x=>x.OTHardwareId)
                //}
                if (Modal == null || Modal.AssetManagmentEquipmentITListModel == null || Modal.AssetManagmentEquipmentITListModel.Count <= 0)
                {

                    Modal.AssetManagmentEquipmentITListModel = new List<AssetManagmentEquipmentITListModel>();
                    Modal.AssetManagmentEquipmentITListModel.AddRange(new List<AssetManagmentEquipmentITListModel> {
            new AssetManagmentEquipmentITListModel{ITEquipment="SATTELITE PHONE 1",ITLastServiced="",ITLocation ="BRIDGE",ITMake ="CISCO",ITModel = "CP7821",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="SATTELITE PHONE 1",ITLastServiced="",ITLocation ="CITADEL",ITMake ="IRIDIUM",ITModel = "9555",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="PHONE",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="CISCO",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="MASTER CABIN",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="PRINTER",ITLastServiced="",ITLocation ="MASTER CABIN",ITMake ="HP",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="TV",ITLastServiced="",ITLocation ="MASTER CABIN",ITMake ="SAMSUNG",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="TV",ITLastServiced="",ITLocation ="CH.OFF CABIN",ITMake ="KONKA",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="CH.ENGR. CABIN",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="PRINTER",ITLastServiced="",ITLocation ="CH.ENGR. CABIN",ITMake ="CANON",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="TV",ITLastServiced="",ITLocation ="CH.ENGR. CABIN",ITMake ="KONKA",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="PHILIP",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="LG",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="PRINTER",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="HP",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="ENGINE ROOM",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment="PRINTER",ITLastServiced="",ITLocation ="ENGINE ROOM",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ITEquipment = "TV", ITLastServiced = "", ITLocation = "CREW SMOKING ROOM", ITMake = "SAMSUNG", ITModel = "", ITRemark = "SATISFACTORY", ITSerialNo = "", ITType = "", ITWorkingCondition = "GOOD" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "IMS Dashboard", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Shipmaster VIMS", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Regs4Ships", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Loading Software", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Admiralty Digital publications", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Thomas Gunn Voyager", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Seagull Training Laptop", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Skyfile Mail", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "MS Office", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Microsoft SQL Server", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "LogMeIn", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Skyfile AntiVirus", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Skyfile NOAD", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },
            new AssetManagmentEquipmentITListModel{ITEquipment = "Mobile Phone", ITLastServiced = "", ITLocation = "", ITMake = "", ITModel = "", ITRemark = "", ITSerialNo = "", ITType = "", ITWorkingCondition = "" },

                    }
                     );
                }
                Modal.AssetManagmentEquipmentOTListModel = Modal.AssetManagmentEquipmentOTListModel.OrderBy(x => x.OTCriticality).ThenBy(x => x.OTEquipment).ToList();
                Modal.AssetManagmentEquipmentITListModel = Modal.AssetManagmentEquipmentITListModel.OrderBy(x => x.ITCriticality).ThenBy(x => x.ITEquipment).ToList();
                Modal.AssetManagmentEquipmentSoftwareAssetsModel = Modal.AssetManagmentEquipmentSoftwareAssetsModel.OrderBy(x => x.SACriticality).ThenBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AssetManagmentEquipmentList " + ex.Message);
            }
            return View(Modal);
        }
        [HttpPost]
        public ActionResult AssetManagmentEquipmentList(AssetManagmentEquipmentListModal Modal)
        {
            try
            {
                if (Modal != null)
                {
                    if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                    {
                        Modal.AssetManagmentEquipmentListForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.AssetManagmentEquipmentListForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.AssetManagmentEquipmentListForm.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.AssetManagmentEquipmentListForm.UpdatedBy = SessionManager.Username;
                    }
                    Modal.AssetManagmentEquipmentListForm.IsSynced = false;
                    AssetManagmentEquipmentListHelper _helper = new AssetManagmentEquipmentListHelper();
                    bool res = _helper.SaveAssetManagmentEquipmentDataInLocalDB(Modal);
                    if (res)
                    {
                        ViewBag.result = AppStatic.SUCCESS;
                        TempData["AMERes"] = AppStatic.SUCCESS;
                        bool isInternetAvailable = Utility.CheckInternet();
                        if (isInternetAvailable)
                        {
                            var objData = _helper.GetAssetManagmentEquipmentData_LocalDB(SessionManager.ShipCode);
                            if (objData != null && objData.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                            {
                                APIHelper _aHelper = new APIHelper();
                                Modal.AssetManagmentEquipmentListForm.IsSynced = true;
                                Modal.AssetManagmentEquipmentListForm.AMEId = objData.AssetManagmentEquipmentListForm.AMEId;
                                APIResponse resp = _aHelper.SubmitAssetManagmentEquipmentList(Modal);
                                if (resp != null && resp.result == AppStatic.SUCCESS)
                                {
                                    _helper.SaveAssetManagmentEquipmentDataInLocalDB(Modal);
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.result = AppStatic.ERROR;
                        TempData["AMERes"] = AppStatic.ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.result = AppStatic.ERROR;
                TempData["AMERes"] = AppStatic.ERROR;
                LogHelper.writelog("AssetManagmentEquipmentList Save Data " + ex.Message);
            }
            return RedirectToAction("AssetManagmentEquipmentList");
        }
        public string AssetManagmentEquipmentAutoSave(AssetManagmentEquipmentListModal Modal)
        {
            string resp = "";
            try
            {
                if (Modal != null)
                {
                    if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                    {
                        Modal.AssetManagmentEquipmentListForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.AssetManagmentEquipmentListForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.AssetManagmentEquipmentListForm.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.AssetManagmentEquipmentListForm.UpdatedBy = SessionManager.Username;
                    }
                    Modal.AssetManagmentEquipmentListForm.IsSynced = false;
                    AssetManagmentEquipmentListHelper _helper = new AssetManagmentEquipmentListHelper();
                    bool res = _helper.SaveAssetManagmentEquipmentDataInLocalDB(Modal);
                    if (res)
                    {
                        ViewBag.result = AppStatic.SUCCESS;
                        if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                        {
                            var objData = _helper.GetAssetManagmentEquipmentData_LocalDB(SessionManager.ShipCode);
                            if (objData != null && objData.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                                Modal.AssetManagmentEquipmentListForm.AMEId = objData.AssetManagmentEquipmentListForm.AMEId;
                        }
                    }
                    else
                    {
                        ViewBag.result = AppStatic.ERROR;
                        TempData["AMERes"] = AppStatic.ERROR;
                    }
                    resp = Convert.ToString(Modal.AssetManagmentEquipmentListForm.AMEId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AssetManagmentEquipmentAutoSave AutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            return resp;
        }
        public ActionResult DownloadAssetManagmentEquipmentList()
        {
            try
            {
                string shipCode = SessionManager.ShipCode;
                AssetManagmentEquipmentListHelper _helper = new AssetManagmentEquipmentListHelper();
                var Modal = _helper.GetAssetManagmentEquipmentData_LocalDB(shipCode);
                if (Modal == null)
                {
                    Modal = new AssetManagmentEquipmentListModal
                    {
                        AssetManagmentEquipmentITListModel = new List<AssetManagmentEquipmentITListModel>(),
                        AssetManagmentEquipmentOTListModel = new List<AssetManagmentEquipmentOTListModel>(),
                        AssetManagmentEquipmentSoftwareAssetsModel = new List<AssetManagmentEquipmentSoftwareAssetsModel>()
                    };
                }
                List<OTITListReportModel> exportOTList = new List<OTITListReportModel>();
                if (Modal.AssetManagmentEquipmentOTListModel != null && Modal.AssetManagmentEquipmentOTListModel.Count > 0)
                {
                    foreach (var item in Modal.AssetManagmentEquipmentOTListModel)
                    {
                        exportOTList.Add(new OTITListReportModel
                        {
                            Criticality = item.OTCriticality,
                            Equipment = item.OTEquipment,
                            HardwareId = item.OTHardwareId,
                            LastServiced = item.OTLastServiced,
                            Location = item.OTLocation,
                            Make = item.OTMake,
                            Model = item.OTModel,
                            Owner = item.OTOwner,
                            PersonResponsible = item.OTPersonResponsible,
                            Remark = item.OTRemark,
                            SerialNo = item.OTSerialNo,
                            Type = item.OTType,
                            WorkingCondition = item.OTWorkingCondition
                        });
                    }
                }
                List<OTITListReportModel> exportITList = new List<OTITListReportModel>();
                if (Modal.AssetManagmentEquipmentITListModel != null && Modal.AssetManagmentEquipmentITListModel.Count > 0)
                {
                    foreach (var item in Modal.AssetManagmentEquipmentITListModel)
                    {
                        exportITList.Add(new OTITListReportModel
                        {
                            Criticality = item.ITCriticality,
                            Equipment = item.ITEquipment,
                            HardwareId = item.ITHardwareId,
                            LastServiced = item.ITLastServiced,
                            Location = item.ITLocation,
                            Make = item.ITMake,
                            Model = item.ITModel,
                            Owner = item.ITOwner,
                            PersonResponsible = item.ITPersonResponsible,
                            Remark = item.ITRemark,
                            SerialNo = item.ITSerialNo,
                            Type = item.ITType,
                            WorkingCondition = item.ITWorkingCondition
                        });
                    }
                }

                List<SoftwareAssetsReportModel> exportSWList = new List<SoftwareAssetsReportModel>();
                if (Modal.AssetManagmentEquipmentSoftwareAssetsModel != null && Modal.AssetManagmentEquipmentSoftwareAssetsModel.Count > 0)
                {
                    foreach (var item in Modal.AssetManagmentEquipmentSoftwareAssetsModel)
                    {
                        exportSWList.Add(new SoftwareAssetsReportModel
                        {
                            Category = item.Category,
                            IsActive = item.IsActive,
                            LicenseType = item.LicenseType,
                            Manufacturer = item.Manufacturer,
                            Name = item.Name,
                            Criticality = item.SACriticality,
                            Owner = item.SAOwner,
                            PersonResponsible = item.SAPersonResponsible,
                            SoftwareId = item.SASoftwareId
                        });
                    }
                }

                exportOTList = exportOTList.OrderBy(x => x.HardwareId).ToList();
                exportITList = exportITList.OrderBy(x => x.HardwareId).ToList();
                exportSWList = exportSWList.OrderBy(x => x.SoftwareId).ToList();

                DataSet ds = new DataSet();
                ds.Tables.Add(Utility.ToDataTable(exportOTList));
                ds.Tables.Add(Utility.ToDataTable(exportITList));
                ds.Tables.Add(Utility.ToDataTable(exportSWList));

                var fileName = "Assets_report.xls";
                var spreadsheetStream = Utility.CreateWorkbook(ds, new List<string>() { "OTEquipment", "ITEquipment", "SoftwareAssets" });
                return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }
        #endregion

        #region CybersecurityRisksAssessment
        public ActionResult CybersecurityRisksAssessment()
        {
            CybersecurityRisksAssessmentModal Modal = new CybersecurityRisksAssessmentModal();
            try
            {
                if (TempData["CRARes"] != null)
                    ViewBag.result = TempData["CRARes"];
                #region BindRiskList
                List<string> lstRiskList = new List<string>();
                lstRiskList.Add("accidental change of information system data");
                lstRiskList.Add("application errors");
                lstRiskList.Add("bomb attack");
                lstRiskList.Add("bomb threat");
                lstRiskList.Add("breach of contractual relations");
                lstRiskList.Add("breach of legislation");
                lstRiskList.Add("breakdown of communication links");
                lstRiskList.Add("concealing user identity");
                lstRiskList.Add("damage caused by third-party activities");
                lstRiskList.Add("damage incurred during penetration testing");
                lstRiskList.Add("destruction of records");
                lstRiskList.Add("deterioration of media");
                lstRiskList.Add("disclosure of passwords");
                lstRiskList.Add("eavesdropping");
                lstRiskList.Add("embezzlement");
                lstRiskList.Add("equipment failure");
                lstRiskList.Add("falsification of records");
                lstRiskList.Add("fire");
                lstRiskList.Add("flood");
                lstRiskList.Add("fraud");
                lstRiskList.Add("industrial espionage");
                lstRiskList.Add("information interception");
                lstRiskList.Add("interruption of power supply");
                lstRiskList.Add("leakage/disclosure of information");
                lstRiskList.Add("loss of support services");
                lstRiskList.Add("maintenance errors");
                lstRiskList.Add("malicious code");
                lstRiskList.Add("misuse of audit tools");
                lstRiskList.Add("misuse of information systems");
                lstRiskList.Add("other disasters (man-made)");
                lstRiskList.Add("other disasters (natural)");
                lstRiskList.Add("pollution");
                lstRiskList.Add("social engineering");
                lstRiskList.Add("strike");
                lstRiskList.Add("lightning strike");
                lstRiskList.Add("terrorist attacks");
                lstRiskList.Add("theft");
                lstRiskList.Add("unauthorized access to the information system");
                lstRiskList.Add("unauthorized change of records");
                lstRiskList.Add("unauthorized installation of software");
                lstRiskList.Add("unauthorized network access");
                lstRiskList.Add("unauthorized physical access");
                lstRiskList.Add("unauthorized use of licensed materials");
                lstRiskList.Add("unauthorized use of software");
                lstRiskList.Add("use of unauthorized or untested code");
                lstRiskList.Add("user error");
                lstRiskList.Add("vandalism");
                lstRiskList.Add("risk mismanagement");
                lstRiskList.Add("loss of data and availability");
                lstRiskList.Add("malware");
                lstRiskList.Add("Security breach");
                ViewBag.RiskList = lstRiskList;
                #endregion

                #region Bind Vulnerabilities
                List<string> lstVulnerabilities = new List<string>();
                lstVulnerabilities.Add("active sessions after working hours");
                lstVulnerabilities.Add("cable placing");
                lstVulnerabilities.Add("complicated user interface");
                lstVulnerabilities.Add("cryptographic keys accessible to unauthorized persons");
                lstVulnerabilities.Add("disposal of storage media without erasing data");
                lstVulnerabilities.Add("extensive powers");
                lstVulnerabilities.Add("inadequate capacity management");
                lstVulnerabilities.Add("inadequate change control");
                lstVulnerabilities.Add("inadequate level of knowledge and/or awareness of employees");
                lstVulnerabilities.Add("inadequate maintenance");
                lstVulnerabilities.Add("inadequate network management");
                lstVulnerabilities.Add("inadequate segregation of duties");
                lstVulnerabilities.Add("inadequate supervision of external suppliers");
                lstVulnerabilities.Add("inadequate supervision of the work of employees");
                lstVulnerabilities.Add("inadequate user rights");
                lstVulnerabilities.Add("information available to unauthorized persons");
                lstVulnerabilities.Add("lack of evidence of sent or received messages");
                lstVulnerabilities.Add("lack of input and output data control");
                lstVulnerabilities.Add("lack of or poor internal audit implementation");
                lstVulnerabilities.Add("lack of validation for processed data");
                lstVulnerabilities.Add("location sensitive to natural disasters");
                lstVulnerabilities.Add("location sensitive to water leakage");
                lstVulnerabilities.Add("mobile equipment subject to theft");
                lstVulnerabilities.Add("networks accessible to unauthorized persons");
                lstVulnerabilities.Add("no deactivation of user accounts after termination of employment");
                lstVulnerabilities.Add("no separation of test and operational environment");
                lstVulnerabilities.Add("out-of-date databases for protection against malicious code");
                lstVulnerabilities.Add("over-dependence on one device/system");
                lstVulnerabilities.Add("poor selection of test data");
                lstVulnerabilities.Add("sensitivity of equipment to humidity and pollution");
                lstVulnerabilities.Add("sensitivity of equipment to temperature");
                lstVulnerabilities.Add("sensitivity of equipment to voltage change");
                lstVulnerabilities.Add("single copy, only one copy of information");
                lstVulnerabilities.Add("system-generated user accounts and passwords remain unchanged");
                lstVulnerabilities.Add("systems unprotected from unauthorized access");
                lstVulnerabilities.Add("unauthorized access to facilities allowed");
                lstVulnerabilities.Add("unclearly defined confidentiality level");
                lstVulnerabilities.Add("unclearly defined cryptographic rules");
                lstVulnerabilities.Add("unclearly defined organizational rules");
                lstVulnerabilities.Add("unclearly defined requirements for software development");
                lstVulnerabilities.Add("unclearly defined rules for access control");
                lstVulnerabilities.Add("unclearly defined rules for working off-premises");
                lstVulnerabilities.Add("uncontrolled copying");
                lstVulnerabilities.Add("uncontrolled Internet downloads");
                lstVulnerabilities.Add("uncontrolled use of information systems");
                lstVulnerabilities.Add("undocumented software");
                lstVulnerabilities.Add("unmotivated or disgruntled employees");
                lstVulnerabilities.Add("unprotected public network connections");
                lstVulnerabilities.Add("use of old equipment");
                lstVulnerabilities.Add("weak passwords");
                lstVulnerabilities.Add("lack of two-factor authentication");
                lstVulnerabilities.Add("lack of Company wide risk management");
                lstVulnerabilities.Add("lack of incident management training");
                lstVulnerabilities.Add("lack of sufficient monitoring");
                lstVulnerabilities.Add("lack of removable media policy");
                lstVulnerabilities.Add("lack of Home and Mobile working policy");
                lstVulnerabilities.Add("hardware and software patches not applied within 14 days");
                lstVulnerabilities.Add("lack of formal risk management process");
                lstVulnerabilities.Add("lack of asset management");
                lstVulnerabilities.Add("lack of regular vulnerability scans");
                lstVulnerabilities.Add("lack of user activity monitoring");
                lstVulnerabilities.Add("lack of AUP");
                lstVulnerabilities.Add("lack of web filtering");
                lstVulnerabilities.Add("lack of automatic alerting - security monitoring");
                lstVulnerabilities.Add("Outdated software");
                ViewBag.VulnerabilityList = lstVulnerabilities;
                #endregion

                CybersecurityRisksAssessmentHelper _helper = new CybersecurityRisksAssessmentHelper();
                Modal = _helper.GetCybersecurityRisksAssessmentData_LocalDB(SessionManager.ShipCode);
                if (Modal == null || Modal.CybersecurityRisksAssessmentListModal == null || Modal.CybersecurityRisksAssessmentListModal.Count <= 0)
                {
                    if (Utility.CheckInternet())
                    {
                        APIHelper _apihelper = new APIHelper();
                        Modal = _apihelper.GetCybersecurityRisksAssessmentData(SessionManager.ShipCode);
                        ViewBag.HardwareList = _apihelper.GetAssetManagmentHardwareId(SessionManager.ShipCode);
                    }
                }
                else
                {
                    AssetManagmentEquipmentListHelper _assetHElper = new AssetManagmentEquipmentListHelper();
                    ViewBag.HardwareList = _assetHElper.GetAssetManagmentHardwareId_LocalDB(SessionManager.ShipCode);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CybersecurityRisksAssessment " + ex.Message);
            }
            return View(Modal);
        }
        [HttpPost]
        public ActionResult CybersecurityRisksAssessment(CybersecurityRisksAssessmentModal Modal)
        {
            try
            {
                if (Modal != null)
                {
                    if (Modal.CybersecurityRisksAssessmentForm.CRAId == Guid.Empty)
                    {
                        Modal.CybersecurityRisksAssessmentForm.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.CybersecurityRisksAssessmentForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.CybersecurityRisksAssessmentForm.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.CybersecurityRisksAssessmentForm.UpdatedBy = SessionManager.Username;
                    }
                    Modal.CybersecurityRisksAssessmentForm.IsSynced = false;
                    CybersecurityRisksAssessmentHelper _helper = new CybersecurityRisksAssessmentHelper();
                    bool res = _helper.SaveCybersecurityRisksAssessmentDataInLocalDB(Modal);
                    if (res)
                    {
                        ViewBag.result = AppStatic.SUCCESS;
                        TempData["CRARes"] = AppStatic.SUCCESS;
                        bool isInternetAvailable = Utility.CheckInternet();
                        if (isInternetAvailable)
                        {
                            var objData = _helper.GetCybersecurityRisksAssessmentData_LocalDB(SessionManager.ShipCode);
                            if (objData != null && objData.CybersecurityRisksAssessmentForm.CRAId != Guid.Empty)
                            {
                                APIHelper _aHelper = new APIHelper();
                                Modal.CybersecurityRisksAssessmentForm.IsSynced = true;
                                Modal.CybersecurityRisksAssessmentForm.CRAId = objData.CybersecurityRisksAssessmentForm.CRAId;
                                APIResponse resp = _aHelper.SubmitCybersecurityRisksAssessment(Modal);
                                if (resp != null && resp.result == AppStatic.SUCCESS)
                                {
                                    _helper.SaveCybersecurityRisksAssessmentDataInLocalDB(Modal);
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.result = AppStatic.ERROR;
                        TempData["CRARes"] = AppStatic.ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.result = AppStatic.ERROR;
                TempData["CRARes"] = AppStatic.ERROR;
                LogHelper.writelog("CybersecurityRisksAssessment Save Data " + ex.Message);
            }
            return RedirectToAction("CybersecurityRisksAssessment");
        }
        public ActionResult DownloadCybersecurityRiskAssessmentList()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                var Modal = _helper.GetCybersecurityRisksAssessmentData(SessionManager.ShipCode);
                if (Modal != null && Modal.CybersecurityRisksAssessmentListModal != null && Modal.CybersecurityRisksAssessmentListModal.Count > 0)
                {
                    foreach (var item in Modal.CybersecurityRisksAssessmentListModal)
                    {
                        if (!string.IsNullOrWhiteSpace(item.HardwareId))
                            item.HardwareIdList = item.HardwareId.Split(',').ToList();
                        else
                            item.HardwareIdList = new List<string>();
                    }
                }
                else
                {
                    Modal = new CybersecurityRisksAssessmentModal
                    {
                        CybersecurityRisksAssessmentListModal = new List<CybersecurityRisksAssessmentListModal>()
                    };
                }
                List<CybersecurityRisksAssessmentReportModal> exportList = new List<CybersecurityRisksAssessmentReportModal>();
                foreach (var item in Modal.CybersecurityRisksAssessmentListModal)
                {
                    exportList.Add(new CybersecurityRisksAssessmentReportModal
                    {
                        HardwareId = item.HardwareId,
                        Controls = item.Controls,
                        InherentImpactScore = item.InherentImpactScore,
                        InherentLikelihoodScore = item.InherentLikelihoodScore,
                        InherentRiskCategoryA = item.InherentRiskCategoryA,
                        InherentRiskCategoryC = item.InherentRiskCategoryC,
                        InherentRiskCategoryI = item.InherentRiskCategoryI,
                        InherentRiskCategoryS = item.InherentRiskCategoryS,
                        InherentRiskScore = item.InherentRiskScore,
                        ResidualImpactScore = item.ResidualImpactScore,
                        ResidualLikelihoodScore = item.ResidualLikelihoodScore,
                        ResidualRiskCategoryA = item.ResidualRiskCategoryA,
                        ResidualRiskCategoryC = item.ResidualRiskCategoryC,
                        ResidualRiskCategoryI = item.ResidualRiskCategoryI,
                        ResidualRiskCategoryS = item.ResidualRiskCategoryS,
                        ResidualRiskScore = item.ResidualRiskScore,
                        RiskDecision = item.RiskDecision,
                        RiskDescription = item.RiskDescription,
                        RiskId = item.RiskId,
                        RiskOwner = item.RiskOwner,
                        Vulnerability = item.Vulnerability
                    });
                }
                // Export = Export.OrderBy(x => x.Code).ThenBy(x => x.AccountCode).ToList();
                var fileName = "Cybersecurity_report.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(exportList, Response.Output);
                Response.End();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return RedirectToAction("CybersecurityRisksAssessment", "Documents");
        }
        #endregion
    }
}