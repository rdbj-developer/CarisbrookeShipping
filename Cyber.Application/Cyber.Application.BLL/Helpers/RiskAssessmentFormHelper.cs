using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OfficeApplication.BLL.Helpers
{
    public class RiskAssessmentFormHelper
    {
        public bool InsertDocumentsInRiskAssessmentData(string FilePath, string ShipCode, string ShipName, string UserName)
        {
            bool res = false;
            try
            {
                List<RiskAssessmentForm> AllDocs = new List<RiskAssessmentForm>();
                APIHelper _helper = new APIHelper();
                AllDocs.AddRange(GetDocumentsRiskAssesmentSubCategories(FilePath, ShipCode, ShipName, UserName));
                res = _helper.InsertDocumentsBulkDataInRiskAssessment(AllDocs);
                if (res)
                {
                    var hazardList = GetDocumentsRiskAssesmentHazared(FilePath, AllDocs, ShipCode);
                    _helper.InsertDocumentsBulkDataInRiskAssesmentHazared(hazardList);
                    var reviewerList = GetDocumentsRiskAssesmentReviewer(FilePath, AllDocs, ShipCode);
                    _helper.InsertDocumentsBulkDataInRiskAssessmentReviewer(reviewerList);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsInRiskAssessmentData : " + ex.Message);
            }
            return res;
        }

        public List<RiskAssessmentForm> GetDocumentsRiskAssesmentSubCategories(string FilePath, string ShipCode, string ShipName, string UserName)
        {
            List<RiskAssessmentForm> ModalList = new List<RiskAssessmentForm>();
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(FilePath);
                XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
                var List = nodesList.Cast<XmlNode>()
                                  .Where(n => n["Path"].InnerText.Contains("Risk Assessments")).ToList();
                APIHelper _helper = new APIHelper();
                var response = _helper.GetAllDocumentRiskassessment(ShipCode);
                foreach (XmlNode node in List)
                {
                    try
                    {
                        RiskAssessmentForm Modal = new RiskAssessmentForm();
                        string DocumentID = node.Attributes["Id"].InnerText;
                        Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                        Modal.ShipName = ShipName;
                        Modal.ShipCode = ShipCode;
                        Modal.CreatedBy = UserName;
                        Modal.CreatedDate = DateTime.Now;
                        foreach (XmlNode chldNode in node.ChildNodes)
                        {
                            try
                            {
                                if (chldNode.Name == "Number")
                                {
                                    string Number = chldNode.InnerText.Trim();
                                    Modal.Number = Convert.ToString(Number).Trim();
                                }
                                if (chldNode.Name == "Title")
                                {
                                    string Title = chldNode.InnerText.Trim();
                                    Modal.Title = Convert.ToString(Title).Trim();
                                }

                                if (chldNode.Name == "Path")
                                {
                                    string Path = chldNode.InnerText.Trim();
                                    if (!System.IO.File.Exists(Path))
                                    {
                                        try
                                        {
                                            if (Path.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                                                Path = Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                                            Path = System.Web.HttpContext.Current.Server.MapPath(@"~\" + Path);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    if (System.IO.File.Exists(Path) && System.IO.Path.GetExtension(Path).ToLower() == ".xml")
                                    {
                                        try
                                        {
                                            XmlDocument xml = new XmlDocument();
                                            xml.Load(Path);
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
                                                    Modal.ReviewerName = Convert.ToString(ReviewName).Trim();
                                                    string ReviewRank = RankNodes[0].InnerText.Trim();
                                                    Modal.ReviewerRank = Convert.ToString(ReviewRank).Trim();
                                                    string ReviewDate = DateNodes[0].InnerText.Trim();
                                                    if (ReviewDate != null && ReviewDate != "")
                                                    {
                                                        Modal.ReviewerDate = Convert.ToDateTime(ReviewDate);
                                                    }
                                                    string ReviewLocation = LocationNodes[0].InnerText.Trim();
                                                    Modal.ReviewerLocation = Convert.ToString(ReviewLocation).Trim();
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
                                                    Modal.Code = Convert.ToString(Code).Trim();
                                                    string Issue = IssueNode[0].InnerText.Trim();
                                                    Modal.Issue = Convert.ToInt32(Issue);
                                                    string IssueDate = "01/" + IssueDateNode[0].InnerText.Split('/')[0] + "/20" + IssueDateNode[0].InnerText.Split('/')[1];
                                                    if (IssueDate != null && IssueDate != "")
                                                    {
                                                        Modal.IssueDate = DateTime.ParseExact(IssueDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                                    }
                                                    string Amendment = AmendmentNode[0].InnerText.Trim();
                                                    Modal.Amendment = Convert.ToInt32(Amendment);
                                                    string AmendmentDate = AmendmentDateNode[0].InnerText.Trim();
                                                    if (AmendmentDate != null && AmendmentDate != "")
                                                    {
                                                        Modal.AmendmentDate = DateTime.ParseExact(AmendmentDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                                    }
                                                    string IsConfidential = IsConfidentialNode[0].InnerText.Trim();
                                                    Modal.IsConfidential = Convert.ToBoolean(IsConfidential);
                                                }
                                            }
                                        }
                                        catch (Exception t)
                                        {
                                            LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => Inner Most for : " + t.Message + " " + t.InnerException + " Path : " + Path);
                                        }

                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => Inner for : " + e.Message + " " + e.InnerException);
                            }

                        }
                        //Modal.Title  Check this value from RiskFormTable IF exist then do not add to this list                    
                        var listdata = response.Where(x => x.Title == Modal.Title).FirstOrDefault();
                        if (listdata == null)
                        {
                            ModalList.Add(Modal);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => for : " + ex.Message + " " + ex.InnerException);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => outer : " + e.Message + " " + e.InnerException);
            }
            return ModalList;
        }

        public List<RiskAssessmentFormHazard> GetDocumentsRiskAssesmentHazared(string FilePath, List<RiskAssessmentForm> ExistDocs, string ShipCode)
        {
            if (ExistDocs == null || ExistDocs.Count <= 0)
                return new List<RiskAssessmentFormHazard>();
            XmlDocument doc = new XmlDocument();
            List<RiskAssessmentFormHazard> ModalList = new List<RiskAssessmentFormHazard>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
            var List = nodesList.Cast<XmlNode>()
                              .Where(n => n["Path"].InnerText.Contains("Risk Assessments")).ToList();
            APIHelper _helper = new APIHelper();
            var response = _helper.GetAllDocumentRiskassessment(ShipCode);
            foreach (XmlNode node in List)
            {
                string DocumentID = node.Attributes["Id"].InnerText;
                Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                var RAFID = 0;

                foreach (XmlNode chldNode in node.ChildNodes)
                {
                    if (chldNode.Name == "Title")
                    {
                        string Title = chldNode.InnerText.Trim();
                        RAFID = Convert.ToInt32(response.Where(x => x.Title == Title).Select(x => x.RAFID).SingleOrDefault());
                    }
                    if (chldNode.Name == "Path")
                    {
                        string Path = chldNode.InnerText.Trim();
                        if (!System.IO.File.Exists(Path))
                        {
                            try
                            {
                                if (Path.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                                    Path = Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                                Path = System.Web.HttpContext.Current.Server.MapPath(@"~\" + Path);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        if (System.IO.File.Exists(Path) && System.IO.Path.GetExtension(Path).ToLower() == ".xml")
                        {
                            if (RAFID == 0)
                                continue;
                            XmlDocument xml = new XmlDocument();
                            xml.Load(Path);
                            XmlNodeList nodes = xml.DocumentElement.ChildNodes;
                            foreach (XmlNode childNodes in nodes)
                            {
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
                                        RiskAssessmentFormHazard Modal = new RiskAssessmentFormHazard();
                                        Modal.Id = newGuid;
                                        Modal.RAFID = RAFID;
                                        string HazardId = HazardIdNode[i].Attributes["my:HazardId"].InnerText.Trim();
                                        Modal.HazardId = Convert.ToInt64(HazardId);
                                        string Description1 = DescriptionNode[i].InnerText.Trim();
                                        Modal.Stage1Description = Convert.ToString(Description1).Trim();
                                        string Severity2 = SeverityNode[i].InnerText.Trim();
                                        Modal.Stage2Severity = Convert.ToString(Severity2).Trim();
                                        string Likelihood2 = LikelihoodNode[i].InnerText.Trim();
                                        Modal.Stage2Likelihood = Convert.ToString(Likelihood2).Trim();
                                        string RiskFactor2 = RiskFactorNode[i].InnerText.Trim();
                                        Modal.Stage2RiskFactor = Convert.ToString(RiskFactor2).Trim();
                                        string Description3 = ControlMeasuresNode[i].InnerText.Trim();
                                        Modal.Stage3Description = Convert.ToString(Description3).Trim();
                                        string Severity4 = ReducedSeverityNode[i].InnerText.Trim();
                                        Modal.Stage4Severity = Convert.ToString(Severity4).Trim();
                                        string Likelihood4 = ReducedLikelihoodNode[i].InnerText.Trim();
                                        Modal.Stage4Likelihood = Convert.ToString(Likelihood4).Trim();
                                        string RiskFactor4 = ReducedRiskFactorNode[i].InnerText.Trim();
                                        Modal.Stage4RiskFactor = Convert.ToString(RiskFactor4).Trim();

                                        ModalList.Add(Modal);
                                    }

                                }
                            }
                        }
                    }
                }
                //ModalList.Add(Modal);
            }
            return ModalList;
        }
        public List<RiskAssessmentFormReviewer> GetDocumentsRiskAssesmentReviewer(string FilePath, List<RiskAssessmentForm> ExistDocs, string ShipCode)
        {
            if (ExistDocs == null || ExistDocs.Count <= 0)
                return new List<RiskAssessmentFormReviewer>();
            XmlDocument doc = new XmlDocument();
            List<RiskAssessmentFormReviewer> ModalList = new List<RiskAssessmentFormReviewer>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
            var List = nodesList.Cast<XmlNode>()
                              .Where(n => n["Path"].InnerText.Contains("Risk Assessments")).ToList();
            APIHelper _helper = new APIHelper();
            var response = _helper.GetAllDocumentRiskassessment(ShipCode);
            foreach (XmlNode node in List)
            {
                try
                {
                    RiskAssessmentFormReviewer Modal = new RiskAssessmentFormReviewer();
                    string DocumentID = node.Attributes["Id"].InnerText;
                    Guid newGuid = Guid.NewGuid();
                    Modal.Id = newGuid;
                    string newTitle = "";
                    foreach (XmlNode chldNode in node.ChildNodes)
                    {
                        try
                        {
                            if (chldNode.Name == "Title")
                            {
                                string Title = chldNode.InnerText.Trim();
                                newTitle = ExistDocs.Where(x => x.Title == Title).Select(x => x.Title).SingleOrDefault();
                                if (newTitle == null)
                                    continue;

                                var RAFID = response.Where(x => x.Title == Title).Select(x => x.RAFID).SingleOrDefault();
                                Modal.RAFID = RAFID;
                            }
                            if (chldNode.Name == "Path")
                            {
                                if (Modal.RAFID == 0)
                                    continue;
                                string Path = chldNode.InnerText.Trim();
                                if (!System.IO.File.Exists(Path))
                                {
                                    try
                                    {
                                        if (Path.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                                            Path = Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                                        Path = System.Web.HttpContext.Current.Server.MapPath(@"~\" + Path);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                if (System.IO.File.Exists(Path) && System.IO.Path.GetExtension(Path).ToLower() == ".xml")
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(Path);
                                    XmlNodeList nodes = xml.DocumentElement.ChildNodes;
                                    foreach (XmlNode childNodes in nodes)
                                    {
                                        if (childNodes.Name == "my:Review")
                                        {
                                            XmlNodeList MemberNodes = xml.GetElementsByTagName("my:ReviewMembers");
                                            string ReviewersName = MemberNodes[0].InnerText.Trim();
                                            Modal.ReviewerName = Convert.ToString(ReviewersName).Trim();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.writelog("GetDocumentsRiskAssesmentReviewer => Inner for : " + e.Message);
                        }
                    }
                    ModalList.Add(Modal);
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("GetDocumentsRiskAssesmentReviewer => for : " + ex.Message);
                }
            }
            return ModalList;
        }
        public RiskAssessmentFormModal GetDocumentsRiskAssesmentFileData(string FilePath)
        {
            RiskAssessmentFormModal ModalList = new RiskAssessmentFormModal();
            try
            {
                APIHelper _helper = new APIHelper();
                var response = _helper.GetAllDocumentRiskassessment(SessionManager.ShipCode);
                try
                {
                    RiskAssessmentForm Modal = new RiskAssessmentForm();
                    List<RiskAssessmentFormHazard> lstHazards = new List<RiskAssessmentFormHazard>();
                    List<RiskAssessmentFormReviewer> lstReviewers = new List<RiskAssessmentFormReviewer>();
                    Modal.ShipName = SessionManager.ShipName;
                    Modal.ShipCode = SessionManager.ShipCode;
                    Modal.CreatedBy = SessionManager.Username;
                    Modal.CreatedDate = DateTime.Now;
                    if (System.IO.File.Exists(FilePath))
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(FilePath);
                        XmlNodeList nodes = xml.DocumentElement.ChildNodes;
                        foreach (XmlNode childNodes in nodes)
                        {
                            if (childNodes.Name == "my:Details")
                            {
                                XmlNodeList NumberNode = xml.GetElementsByTagName("my:Number");
                                string Number = NumberNode[0].InnerText.Trim();
                                Modal.Number = Convert.ToString(Number).Trim();

                                XmlNodeList TitleNodes = xml.GetElementsByTagName("my:Title");
                                string Title = TitleNodes[0].InnerText.Trim();
                                Modal.Title = Convert.ToString(Title).Trim();
                            }
                            if (childNodes.Name == "my:Review")
                            {
                                XmlNodeList NameNode = xml.GetElementsByTagName("my:ReviewerName");
                                XmlNodeList RankNodes = xml.GetElementsByTagName("my:ReviewerRank");
                                XmlNodeList DateNodes = xml.GetElementsByTagName("my:ReviewDate");
                                XmlNodeList LocationNodes = xml.GetElementsByTagName("my:ReviewLocation");
                                XmlNodeList MemberNodes = xml.GetElementsByTagName("my:ReviewMembers");
                                string ReviewName = NameNode[0].InnerText.Trim();
                                Modal.ReviewerName = Convert.ToString(ReviewName).Trim();
                                string ReviewRank = RankNodes[0].InnerText.Trim();
                                Modal.ReviewerRank = Convert.ToString(ReviewRank).Trim();
                                string ReviewDate = DateNodes[0].InnerText.Trim();
                                if (ReviewDate != null && ReviewDate != "")
                                {
                                    Modal.ReviewerDate = Convert.ToDateTime(ReviewDate);
                                }
                                string ReviewLocation = LocationNodes[0].InnerText.Trim();
                                Modal.ReviewerLocation = Convert.ToString(ReviewLocation).Trim();
                                if (MemberNodes != null && MemberNodes.Count > 0)
                                {
                                    for (var i = 0; i < MemberNodes.Count; i++)
                                    {
                                        RiskAssessmentFormReviewer ModalReviewer = new RiskAssessmentFormReviewer();
                                        string Description1 = MemberNodes[i].InnerText.Trim();
                                        ModalReviewer.ReviewerName = Convert.ToString(Description1).Trim();
                                        lstReviewers.Add(ModalReviewer);
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
                                Modal.Code = Convert.ToString(Code).Trim();
                                string Issue = IssueNode[0].InnerText.Trim();
                                Modal.Issue = Convert.ToInt32(Issue);
                                string IssueDate = "01/" + IssueDateNode[0].InnerText.Split('/')[0] + "/20" + IssueDateNode[0].InnerText.Split('/')[1];
                                if (IssueDate != null && IssueDate != "")
                                {
                                    Modal.IssueDate = DateTime.ParseExact(IssueDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                string Amendment = AmendmentNode[0].InnerText.Trim();
                                Modal.Amendment = Convert.ToInt32(Amendment);
                                string AmendmentDate = AmendmentDateNode[0].InnerText.Trim();
                                if (AmendmentDate != null && AmendmentDate != "")
                                {
                                    Modal.AmendmentDate = DateTime.ParseExact(AmendmentDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                string IsConfidential = IsConfidentialNode[0].InnerText.Trim();
                                Modal.IsConfidential = Convert.ToBoolean(IsConfidential);
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
                                    lstHazards.Add(ModalHazard);
                                }
                            }
                        }
                    }
                    //Modal.Title  Check this value from RiskFormTable IF exist then do not add to this list                    
                    var listdata = response.Where(x => x.Title == Modal.Title).FirstOrDefault();
                    if (listdata == null)
                    {
                        ModalList.RiskAssessmentForm = Modal;
                        ModalList.RiskAssessmentFormHazardList = lstHazards;
                        ModalList.RiskAssessmentFormReviewerList = lstReviewers;
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.writelog("GetDocumentsRiskAssesmentFileData => for : " + ex.Message);
                }
            }
            catch (Exception e)
            {
                LogHelper.writelog("GetDocumentsRiskAssesmentFileData => outer : " + e.Message);
            }
            return ModalList;
        }
    }
}
