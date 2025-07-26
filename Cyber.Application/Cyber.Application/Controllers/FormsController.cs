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
            return View(ModalList);
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
            if (id != "")
            {
                APIHelper _helper = new APIHelper();
                res = _helper.UpdateDeficienciesData(Convert.ToInt32(id), Convert.ToBoolean(isClose));
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddDeficienciesNote(DeficienciesNote notes)
        {
            long res = 0;
            if (notes != null)
            {
                APIHelper _helper = new APIHelper();
                _helper.AddDeficienciesNote(notes);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDeficienciesNote(int id)
        {
            List<DeficienciesNote> response = new List<DeficienciesNote>();
            if (id != 0)
            {
                APIHelper _helper = new APIHelper();
                response = _helper.GetDeficienciesNote(id);
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

        public ActionResult Download(int file, string name)
        {
            APIHelper _helper = new APIHelper();
            string GetFile = _helper.GetFile(file);
            string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
            LogHelper.writelog("---------" + filepath + "----------");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
        public ActionResult DownloadCommentFile(int file, string name)
        {
            APIHelper _helper = new APIHelper();
            string GetFile = _helper.GetFileComment(file);
            string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString() + "/" + GetFile;
            LogHelper.writelog("---------" + filepath + "----------");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
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

        public ActionResult RAformData(int? id = 0, string shipCode = "", string docid = "")
        {
            RiskAssessmentFormModal Modal = new RiskAssessmentFormModal();
            ViewBag.result = Utility.ToString(TempData["RAFRes"]);
            ViewBag.CurrentDateString = Utility.GetCurrentDateShortString();
            APIHelper _helper = new APIHelper();
            try
            {
                if (string.IsNullOrWhiteSpace(shipCode))
                    shipCode = SessionManager.ShipCode;


                if (id > 0)
                {
                    Modal = _helper.RAFormDetailsView(shipCode, Convert.ToInt32(id));
                }
                else if (!string.IsNullOrWhiteSpace(docid))
                {
                    DocumentModal docModal = _helper.GetDocumentBYID(docid);
                    if (docModal != null && !string.IsNullOrEmpty(docModal.Path))
                    {
                        Guid newGuid = docModal.DocumentID.Value;
                        Modal.RiskAssessmentForm.CreatedDate = DateTime.Now;
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
                    Modal.RiskAssessmentForm.CreatedDate = DateTime.Now;
                    Modal.RiskAssessmentForm.CreatedBy = SessionManager.Username;
                    Modal.RiskAssessmentForm.ShipCode = Modal.RiskAssessmentForm.ShipCode ?? SessionManager.ShipCode;
                    Modal.RiskAssessmentForm.ShipName = Modal.RiskAssessmentForm.ShipName ?? SessionManager.ShipName;
                    if (Modal.RiskAssessmentForm.IsAmended.HasValue && Modal.RiskAssessmentForm.IsAmended.Value)
                        Modal.RiskAssessmentForm.AmendmentDate = DateTime.Now;
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
                return RedirectToAction("RAformData", new { id = Modal.RiskAssessmentForm.RAFID, shipCode = Modal.RiskAssessmentForm.ShipCode });
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
    }
}