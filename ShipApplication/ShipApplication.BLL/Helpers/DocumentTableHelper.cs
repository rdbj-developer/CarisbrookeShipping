using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace ShipApplication.BLL.Helpers
{
    public class DocumentTableHelper
    {
        #region CreateDatabase
        public bool InsertDocumentsDataInLocalDB(List<DocumentModal> AllDocuments)
        {
            bool res = false;
            try
            {
                List<DocumentModal> Documents = FilterDocuments(AllDocuments);
                if (Documents != null && Documents.Count > 0)
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.Documents);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.Documents); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(Documents, new List<string> { "RAFID", "RAFId1" });
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.Documents;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Insert Documents Data In LocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public List<DocumentModal> GetDocumentsMainCategories(string FilePath)
        {
            XmlDocument doc = new XmlDocument();
            List<DocumentModal> ModalList = new List<DocumentModal>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("DocumentFolder");
            foreach (XmlNode node in nodesList)
            {
                DocumentModal Modal = new DocumentModal();
                string DocumentID = node.Attributes["Id"].InnerText;
                Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                Modal.DocumentID = newGuid;
                Modal.Type = "FOLDER";
                Modal.IsDeleted = false;
                Modal.Version = 1.0;
                Modal.DocumentVersion = 1.0;
                Modal.UploadType = AppStatic.NEW;
                Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.SectionType = node.Attributes["SectionType"] != null ? node.Attributes["SectionType"].InnerText : "ISM";
                foreach (XmlNode chldNode in node.ChildNodes)
                {
                    if (chldNode.Name == "ParentId")
                    {
                        string ParentId = chldNode.InnerText.Trim();
                        newGuid = Guid.Parse(Convert.ToString(ParentId));
                        Modal.ParentID = newGuid;
                    }
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
                        Modal.Path = Convert.ToString(Path).Trim();
                    }
                }
                ModalList.Add(Modal);
            }
            return ModalList;
        }
        public List<DocumentModal> GetDocumentsSubCategories(string FilePath)
        {
            XmlDocument doc = new XmlDocument();
            List<DocumentModal> ModalList = new List<DocumentModal>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
            foreach (XmlNode node in nodesList)
            {
                DocumentModal Modal = new DocumentModal();
                string DocumentID = node.Attributes["Id"].InnerText;
                Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                Modal.DocumentID = newGuid;
                Modal.IsDeleted = false;
                Modal.Version = 1.0;
                Modal.DocumentVersion = 1.0;
                Modal.UploadType = AppStatic.NEW;
                Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.SectionType = node.Attributes["SectionType"] != null ? node.Attributes["SectionType"].InnerText : "ISM";
                foreach (XmlNode chldNode in node.ChildNodes)
                {
                    if (chldNode.Name == "ParentId")
                    {
                        string ParentId = chldNode.InnerText.Trim();
                        newGuid = Guid.Parse(Convert.ToString(ParentId));
                        Modal.ParentID = newGuid;
                    }
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
                        Modal.Path = Convert.ToString(Path).Trim();
                    }
                }
                string res = System.IO.Path.GetExtension(Modal.Path);
                res = res.Replace(".", "").ToUpper();
                Modal.Type = res;
                ModalList.Add(Modal);
            }
            return ModalList;
        }
        public bool InsertDocumentsDataInRiskAssessmentLocalDB(List<RiskAssessmentForm> AllDocuments)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentForm);
                bool isTbaleCreated = true;
                if (!isTableExist)
                {
                    isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentForm);
                }
                if (isTbaleCreated)
                {
                    bool isColumnIsApplicableExist = LocalDBHelper.CheckTableColumnExist(AppStatic.RiskAssessmentForm, "IsApplicable");
                    if (!isColumnIsApplicableExist)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE RiskAssessmentForm ADD IsApplicable bit default 1");
                }
                List<RiskAssessmentForm> RiskAssessments = FilterRiskAssessment(AllDocuments);
                if (RiskAssessments != null && RiskAssessments.Count > 0)
                {
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(RiskAssessments);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.RiskAssessmentForm;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Insert Documents Data In LocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool InsertDocumentsDataInRiskAssesmentHazaredLocalDB(List<RiskAssessmentFormHazard> AllDocuments)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentFormHazard);
                bool isTbaleCreated = true;
                if (!isTableExist)
                {
                    isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentFormHazard);
                }
                List<RiskAssessmentFormHazard> RiskAssessments = FilterRiskAssessmentHazard(AllDocuments);
                if (RiskAssessments != null && RiskAssessments.Count > 0)
                {
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(RiskAssessments);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.RiskAssessmentFormHazard;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Insert Documents Data In Risk Assessment Reviewer LocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool InsertDocumentsDataInRiskAssessmentReviewerLocalDB(List<RiskAssessmentFormReviewer> AllDocuments)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentFormReviewer);
                bool isTbaleCreated = true;
                if (!isTableExist)
                {
                    isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentFormReviewer);
                }
                List<RiskAssessmentFormReviewer> RiskAssessments = FilterRiskAssessmentReviewer(AllDocuments);
                if (RiskAssessments != null && RiskAssessments.Count > 0)
                {

                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(RiskAssessments);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.RiskAssessmentFormReviewer;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Insert Documents Data In Risk Assessment Reviewer LocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        public List<RiskAssessmentForm> GetDocumentsRiskAssesmentSubCategories(string FilePath)
        {
            List<RiskAssessmentForm> ModalList = new List<RiskAssessmentForm>();
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(FilePath);
                XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
                var List = nodesList.Cast<XmlNode>()
                                  .Where(n => n["Path"].InnerText.Contains("Risk Assessments")).ToList();
                var response = GetAllDocumentRiskassessment(SessionManager.ShipCode);
                foreach (XmlNode node in List)
                {
                    try
                    {
                        RiskAssessmentForm Modal = new RiskAssessmentForm();
                        string DocumentID = node.Attributes["Id"].InnerText;
                        Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                        Modal.ShipName = SessionManager.ShipName;
                        Modal.ShipCode = SessionManager.ShipCode;
                        Modal.CreatedBy = SessionManager.Username;
                        Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        foreach (XmlNode chldNode in node.ChildNodes)
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
                                //if (!System.IO.File.Exists(Path))
                                //{
                                //    try
                                //    {
                                //        if (Path.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                                //            Path = Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                                //        Path = System.Web.HttpContext.Current.Server.MapPath(@"~\" + Path);
                                //    }
                                //    catch (Exception)
                                //    {
                                //    }
                                //}
                                if (System.IO.File.Exists(Path))
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
                        LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => for : " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => outer : " + e.Message);
            }
            return ModalList;
        }

        public RiskAssessmentFormModal GetDocumentsRiskAssesmentFileData(string FilePath)
        {
            RiskAssessmentFormModal ModalList = new RiskAssessmentFormModal();
            try
            {
                var response = GetAllDocumentRiskassessment(SessionManager.ShipCode);
                try
                {
                    RiskAssessmentForm Modal = new RiskAssessmentForm();
                    List<RiskAssessmentFormHazard> lstHazards = new List<RiskAssessmentFormHazard>();
                    List<RiskAssessmentFormReviewer> lstReviewers = new List<RiskAssessmentFormReviewer>();
                    Modal.ShipName = SessionManager.ShipName;
                    Modal.ShipCode = SessionManager.ShipCode;
                    Modal.CreatedBy = SessionManager.Username;
                    Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
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
                    LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => for : " + ex.Message);
                }
            }
            catch (Exception e)
            {
                LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => outer : " + e.Message);
            }
            return ModalList;
        }

        public List<RiskAssessmentFormReviewer> GetDocumentsRiskAssesmentReviewer(string FilePath, List<RiskAssessmentForm> ExistDocs)
        {
            if (ExistDocs == null || ExistDocs.Count <= 0)
                return new List<RiskAssessmentFormReviewer>();
            XmlDocument doc = new XmlDocument();
            List<RiskAssessmentFormReviewer> ModalList = new List<RiskAssessmentFormReviewer>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
            var List = nodesList.Cast<XmlNode>()
                              .Where(n => n["Path"].InnerText.Contains("Risk Assessments")).ToList();
            var response = GetAllDocumentRiskassessment(SessionManager.ShipCode);
            foreach (XmlNode node in List)
            {
                try
                {
                    RiskAssessmentFormReviewer Modal = new RiskAssessmentFormReviewer();
                    string DocumentID = node.Attributes["Id"].InnerText;
                    Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                    Modal.Id = newGuid;
                    string newTitle = "";
                    foreach (XmlNode chldNode in node.ChildNodes)
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
                            if (System.IO.File.Exists(Path))
                            {
                                XmlDocument xml = new XmlDocument();
                                xml.Load(Path);
                                XmlNodeList nodes = xml.DocumentElement.ChildNodes;
                                foreach (XmlNode childNodes in nodes)
                                {
                                    if (childNodes.Name == "my:Review")
                                    {
                                        XmlNodeList MemberNodes = xml.GetElementsByTagName("my:ReviewMembers");
                                        //string ReviewersName = MemberNodes[0].InnerText.Trim();
                                        //Modal.ReviewerName = Convert.ToString(ReviewersName).Trim();
                                        string ReviewersName = "";
                                        if (MemberNodes != null && MemberNodes.Count > 0)
                                        {
                                            foreach (XmlNode item in MemberNodes)
                                            {
                                                try
                                                {
                                                    try
                                                    {
                                                        ReviewersName = item.InnerText;
                                                        if (!string.IsNullOrWhiteSpace(ReviewersName))
                                                        {
                                                            ModalList.Add(new RiskAssessmentFormReviewer
                                                            {
                                                                Id = Modal.Id,
                                                                IndexNo = Modal.IndexNo,
                                                                RAFID = Modal.RAFID,
                                                                ReviewerName = ReviewersName.Trim()
                                                            });
                                                        }
                                                    }
                                                    catch (Exception)
                                                    {

                                                    }

                                                }
                                                catch (Exception)
                                                {
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    LogHelper.writelog("GetDocumentsRiskAssesmentReviewer => for : " + ex.Message);
                }
            }
            return ModalList;
        }

        public List<RiskAssessmentFormHazard> GetDocumentsRiskAssesmentHazared(string FilePath, List<RiskAssessmentForm> ExistDocs)
        {
            if (ExistDocs == null || ExistDocs.Count <= 0)
                return new List<RiskAssessmentFormHazard>();
            XmlDocument doc = new XmlDocument();
            List<RiskAssessmentFormHazard> ModalList = new List<RiskAssessmentFormHazard>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
            var List = nodesList.Cast<XmlNode>()
                              .Where(n => n["Path"].InnerText.Contains("Risk Assessments")).ToList();
            var response = GetAllDocumentRiskassessment(SessionManager.ShipCode);
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
                        //if (!System.IO.File.Exists(Path))
                        //{
                        //    try
                        //    {
                        //        if (Path.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                        //            Path = Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                        //        Path = System.Web.HttpContext.Current.Server.MapPath(@"~\" + Path);
                        //    }
                        //    catch (Exception)
                        //    {
                        //    }
                        //}
                        if (System.IO.File.Exists(Path))
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

        public List<RiskAssessmentFormModal> GetDocumentsRiskAssesmentSubCategoriesFromAWS(List<DocumentModal> AllDocList)
        {
            List<RiskAssessmentFormModal> ModalList = new List<RiskAssessmentFormModal>();
            try
            {
                string OfficeAPPUrl = System.Configuration.ConfigurationManager.AppSettings["OfficeAPPUrl"];
                var response = GetAllDocumentRiskassessment(SessionManager.ShipCode);
                RiskAssessmentFormModal objMasterModel = new RiskAssessmentFormModal();
                RiskAssessmentForm Modal = new RiskAssessmentForm();
                foreach (var node in AllDocList)
                {
                    try
                    {
                        var title = Convert.ToString(node.Title).Trim();
                        var listdata = response.Where(x => x.Title == title).FirstOrDefault();
                        if (listdata != null)
                            continue;
                        objMasterModel = new RiskAssessmentFormModal();
                        Modal = new RiskAssessmentForm();
                        Guid newGuid = node.DocumentID.Value;
                        Modal.ShipName = SessionManager.ShipName;
                        Modal.ShipCode = SessionManager.ShipCode;
                        Modal.CreatedBy = SessionManager.Username;
                        Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.Number = Convert.ToString(node.Number).Trim();
                        Modal.Title = title;
                        if (!string.IsNullOrWhiteSpace(node.Path))
                        {
                            if (!File.Exists(node.Path))
                            {
                                try
                                {
                                    var localPath = node.Path;
                                    //Download from AWS
                                    if (localPath.Contains(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard"))
                                        localPath = localPath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");

                                    string DownloadPath = @"" + OfficeAPPUrl + localPath;
                                    DownloadPath = DownloadPath.Replace("\\", "/");
                                    if (node.Type != "FOLDER" && node.Type != "Folder" && node.DownloadPath != null && node.Type.ToLower() != "windowsfolder")
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(node.Path));
                                        WebClient wc = new WebClient();
                                        wc.DownloadFile(DownloadPath, node.Path);
                                    }
                                    else if (node.Type == "FOLDER" || node.Type == "Folder" || node.Type.ToLower() == "windowsfolder")
                                        Directory.CreateDirectory(Path.GetDirectoryName(node.Path));
                                }
                                catch (Exception e)
                                {
                                }
                            }
                            if (File.Exists(node.Path))
                            {
                                XmlDocument xml = new XmlDocument();
                                xml.Load(node.Path);
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
                                        if (MemberNodes != null && MemberNodes.Count > 0)
                                        {
                                            foreach (XmlNode item in MemberNodes)
                                            {
                                                try
                                                {
                                                    ReviewName = item.InnerText.Trim();
                                                    if (!string.IsNullOrWhiteSpace(ReviewName))
                                                    {
                                                        objMasterModel.RiskAssessmentFormReviewerList.Add(new RiskAssessmentFormReviewer
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
                                            objMasterModel.RiskAssessmentFormHazardList.Add(ModalHazard);
                                        }
                                    }
                                }
                            }
                        }
                        objMasterModel.RiskAssessmentForm = Modal;
                        //Modal.Title  Check this value from RiskFormTable IF exist then do not add to this list                    
                        ModalList.Add(objMasterModel);

                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => for : " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => outer : " + e.Message);
            }
            return ModalList;
        }

        #endregion

        #region Application Use
        public List<DocumentModal> GetAllDocumentsFromLocalDB(string shipCode = "")
        {
            List<DocumentModal> DocList = new List<DocumentModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("IsAvailable");
                        conn.Open();
                        DataTable dt = new DataTable();
                        //string Query = "SELECT D.DocID, D.DocumentID, D.ParentID, D.Number, D.DocNo, D.Title, D.Type, D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.SectionType, ISNULL(R.RAFID,0) as RAFID " +
                        //               "FROM " + AppStatic.Documents + " D outer Apply(select top 1 R.RAFID, R.Number, R.Title, R.Type, R.DocumentID, R.ParentID, R.SectionType From RiskAssessmentForm R " +
                        //               " Where R.Number = D.Number AND R.Title = D.Title AND (@ShipCode = '' OR R.ShipCode=@ShipCode) )R Where IsNULL(D.IsDeleted,0)=0 Union All Select D.DocID, R.DocumentID, R.ParentID, R.Number, D.DocNo, R.Title, R.Type, D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType, ISNULL(R.RAFID,0) as RAFID " +
                        //               "FROM RiskAssessmentForm R " +
                        //               "Left Join Documents D on D.Number = R.Number AND D.Title = R.Title Where D.Title IS NULL AND (@ShipCode = '' OR R.ShipCode=@ShipCode) AND IsNULL(D.IsDeleted,0)=0";

                        // JSL 01/13/2023 commented
                        /*string Query = "SELECT D.DocID, D.DocumentID, D.ParentID, D.Number, D.DocNo, D.Title, D.Type, D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.SectionType, ISNULL(R.RAFID,0) as RAFID, R.RAFUniqueID " +   // JSL 12/27/2022 added , R.RAFUniqueID
                                       "FROM " + AppStatic.Documents + " D outer Apply(select top 1 R.RAFID, R.RAFUniqueID, R.Number, R.Title, R.Type, R.DocumentID, R.ParentID, R.SectionType From RiskAssessmentForm R " +    // JSL 12/27/2022 added , R.RAFUniqueID
                                       " Where R.Number = D.Number AND R.Title = D.Title AND (@ShipCode = '' OR R.ShipCode=@ShipCode) )R Where IsNULL(D.IsDeleted,0)=0 Union All Select D.DocID, D.DocumentID, ISNULL(R.ParentID,D.ParentID) as ParentID, R.Number, D.DocNo, R.Title, 'XML' as [Type], D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType, ISNULL(R.RAFID,0) as RAFID , R.RAFUniqueID " +  // JSL 12/27/2022 added , R.RAFUniqueID
                                       "FROM (Select D.DocID, R.Number, D.DocNo, R.Title, R.Type, D.Path, D.DownloadPath, D.IsDeleted,D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType,ISNULL(R.RAFID,0) as RAFID, R.RAFUniqueID,R.ParentID  " + // JSL 12/27/2022 added , R.RAFUniqueID
                                       "FROM RiskAssessmentForm R Left Join Documents D on D.Number = R.Number AND D.Title = R.Title Where D.Title IS NULL AND (@ShipCode = '' OR R.ShipCode=@ShipCode) AND IsNULL(D.IsDeleted,0)=0 ) R " +
                                       "Outer Apply( Select Top 1 D.DocID,D.DocNo,  D.DocumentID, D.ParentID,D.Path, D.DownloadPath,D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.IsDeleted   from Documents D " +
                                       "Where SUBSTRING (LTRIM(D.Number),1,1) = SUBSTRING (LTRIM(R.Number),1,1) )D";*/
                        // End JSL 01/13/2023 commented

                        // JSL 01/13/2023 
                        string Query = "SELECT D.DocID, D.DocumentID, D.ParentID, D.Number, D.DocNo, D.Title, D.Type, D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.SectionType, ISNULL(R.RAFID,0) as RAFID, R.RAFUniqueID  " +
                                       "FROM " + AppStatic.Documents + " D outer Apply(select top 1 R.RAFID, R.RAFUniqueID, R.Number, R.Title, R.Type, R.DocumentID, R.ParentID, R.SectionType From RiskAssessmentForm R " +
                                       " Where R.Number = D.Number AND R.Title = D.Title AND (@ShipCode = '' OR R.ShipCode=@ShipCode) AND R.RAFUniqueID IS NOT NULL)R Where IsNULL(D.IsDeleted,0)=0 Union All " +   // JSL 01/24/2023 added R.RAFUniqueID is not null
                                       "Select D.DocID, D.DocumentID, ISNULL(R.ParentID,D.ParentID) as ParentID, R.Number" +
                                       //", D.DocNo" +  // JSL 09/06/2022 commented this line
                                       //", ISNULL(R.DocNo, LTRIM(SUBSTRING(R.Number,3,5))) AS 'DocNo'" +   // JSL 02/25/2023 commented   // JSL 09/06/2022
                                       //", ISNULL(R.DocNo, LEFT(SUBSTRING(LTRIM(SUBSTRING(R.Number,3,5)), PATINDEX('%[0-9]%', LTRIM(SUBSTRING(R.Number,3,5))), 8000), PATINDEX('%[^0-9]%', SUBSTRING(LTRIM(SUBSTRING(R.Number,3,5)), PATINDEX('%[0-9]%', LTRIM(SUBSTRING(R.Number,3,5))), 8000) + 'X') -1)) AS 'DocNo'" +  // JSL 02/25/2023 added and commented, use this if below failed
                                       ", ISNULL(R.DocNo, LEFT(SUBSTRING(R.Number, PATINDEX('%[0-9]%', R.Number), 8000), PATINDEX('%[^0-9]%', SUBSTRING(R.Number, PATINDEX('%[0-9]%', R.Number), 8000) + 'X') -1)) AS 'DocNo'" +    // JSL 02/25/2023
                                       ", R.Title, 'XML' as [Type], " +
                                       //"NULL as Path, " +
                                       "D.Path, " +
                                       "D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType, ISNULL(R.RAFID,0) as RAFID, R.RAFUniqueID " +
                                       "FROM (Select D.DocID, R.Number, D.DocNo, R.Title, R.Type, D.Path, D.DownloadPath, D.IsDeleted,D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType,ISNULL(R.RAFID,0) as RAFID, R.RAFUniqueID ,R.ParentID  " +
                                       "FROM RiskAssessmentForm R Left Join Documents D on D.Number = R.Number AND D.Title = R.Title Where D.Title IS NULL AND (@ShipCode = '' OR R.ShipCode=@ShipCode) AND R.RAFUniqueID IS NOT NULL AND IsNULL(D.IsDeleted,0)=0 ) R " +   // JSL 01/24/2023 added R.RAFUniqueID is not null
                                       "Outer Apply( Select Top 1 D.DocID,D.DocNo,  D.DocumentID, D.ParentID,D.Path, D.DownloadPath,D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.IsDeleted   from Documents D " +
                                       //"Where SUBSTRING (LTRIM(D.Number),1,1) = SUBSTRING (LTRIM(R.Number),1,1) AND IsNULL(D.IsDeleted,0)=0  )D"  // JSL 09/06/2022 commented this line
                                       //"Where (LTRIM(SUBSTRING(D.Number,3,5)) =  LTRIM(SUBSTRING(R.Number,3,5))  OR LTRIM(SUBSTRING(D.Number,1,1)) =  LTRIM(SUBSTRING(R.Number,1,1))) AND IsNULL(D.IsDeleted,0)=0  )D"    // JSL 09/06/2022
                                       "Where SUBSTRING (LTRIM(D.Number),1,1) = SUBSTRING (LTRIM(R.Number),1,1) AND IsNULL(D.IsDeleted,0)=0  )D"
                                       + " ORDER BY ParentID, DocNo"    // JSL 09/06/2022
                                       ;
                        // End JSL 01/13/2023 

                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", shipCode);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DocList = dt.ToListof<DocumentModal>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentsFromLocalDB " + ex.Message);
            }
            return DocList;
        }
        public DocumentModal GetDocumentBYID(string id)
        {
            DocumentModal response = new DocumentModal();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {

                        DataTable dt = new DataTable();
                        //string Query = "WITH Docs AS " +
                        //               "(" +
                        //               "   SELECT DocID, DocumentID,Path,Type,Title" +
                        //               "   FROM Documents D" +
                        //               "     WHERE DocumentID = '0FC3BDB8-0D98-4614-81D3-7918D01BFFF2'" +
                        //               "     UNION ALL" +
                        //               "     SELECT ehw.DocID, ehw.DocumentID,ehw.Path,ehw.Type,ehw.Title" +
                        //               "     FROM Docs AS p" +
                        //               "     INNER JOIN Documents AS ehw ON ehw.ParentID = p.DocumentID" +
                        //               " )" +
                        //               " Select D.*FROM Docs D where D.DocumentID =  @DocumentID";
                        string Query = "SELECT * FROM " + AppStatic.Documents + " WHERE DocumentID = @DocumentID";
                        SqlCommand command = new SqlCommand(Query, conn);
                        conn.Open();
                        command.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(id);
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        conn.Close();
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            DataRow dr = dt.Rows[0];
                            response.Path = Utility.ToString(dr["Path"]);
                            response.Type = Utility.ToString(dr["Type"]);
                            response.DocID = Utility.ToInteger(dr["DocID"].ToString());
                            response.Title = Utility.ToString(dr["Title"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentBYID Error : " + ex.Message);
            }
            return response;
        }
        public RiskAssessmentForm GetDocumentRiskassessmentBYID(string id)
        {
            RiskAssessmentForm response = new RiskAssessmentForm();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {

                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " WHERE RAFID = @RAFID";
                        SqlCommand command = new SqlCommand(Query, conn);
                        conn.Open();
                        command.Parameters.Add("@RAFID", SqlDbType.BigInt).Value = Convert.ToInt64(id);
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        conn.Close();
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            DataRow dr = dt.Rows[0];
                            response.RAFID = Utility.ToInteger(dr["RAFID"].ToString());
                            response.Title = Utility.ToString(dr["Title"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentRiskassessmentBYID Error : " + ex.Message);
            }
            return response;
        }

        public RiskAssessmentFormReviewer GetDocumentRiskassessmentReviewerBYID(string id)
        {
            RiskAssessmentFormReviewer response = new RiskAssessmentFormReviewer();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {

                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.RiskAssessmentFormReviewer + " WHERE RAFID = @RAFID";
                        SqlCommand command = new SqlCommand(Query, conn);
                        conn.Open();
                        command.Parameters.Add("@RAFID", SqlDbType.BigInt).Value = Convert.ToInt64(id);
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        conn.Close();
                        if (dt != null && dt.Rows.Count >= 1)
                        {
                            DataRow dr = dt.Rows[0];
                            response.RAFID = Utility.ToInteger(dr["RAFID"].ToString());
                            response.ReviewerName = Utility.ToString(dr["ReviewerName"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentRiskassessmentReviewerBYID Error : " + ex.Message);
            }
            return response;
        }

        public RiskAssessmentFormHazard GetDocumentRiskAssessmentFormHazardBYID(string id)
        {
            RiskAssessmentFormHazard response = new RiskAssessmentFormHazard();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.RiskAssessmentFormHazard + " WHERE RAFID = @RAFID";
                        SqlCommand command = new SqlCommand(Query, conn);
                        conn.Open();
                        command.Parameters.Add("@RAFID", SqlDbType.BigInt).Value = Convert.ToInt64(id);
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        conn.Close();

                        if (dt != null && dt.Rows.Count >= 1)
                        {
                            DataRow dr = dt.Rows[0];
                            response.RAFID = Utility.ToInteger(dr["RAFID"].ToString());
                            response.Stage1Description = Utility.ToString(dr["Stage1Description"]);
                            response.Stage2Severity = Utility.ToString(dr["Stage2Severity"]);
                            response.Stage2Likelihood = Utility.ToString(dr["Stage2Likelihood"]);
                            response.Stage2RiskFactor = Utility.ToString(dr["Stage2RiskFactor"]);
                            response.Stage3Description = Utility.ToString(dr["Stage3Description"]);
                            response.Stage4Severity = Utility.ToString(dr["Stage4Severity"]);
                            response.Stage4Likelihood = Utility.ToString(dr["Stage4Likelihood"]);
                            response.Stage4RiskFactor = Utility.ToString(dr["Stage4RiskFactor"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentRiskAssessmentFormHazardBYID Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        public List<DocumentModal> FilterDocuments(List<DocumentModal> Documents)
        {
            try
            {
                List<DocumentModal> FilteredDocuments = new List<DocumentModal>();
                foreach (var item in Documents)
                {
                    DocumentModal dbDoc = GetDocumentBYID(Utility.ToString(item.DocumentID));
                    if (dbDoc != null && dbDoc.DocID > 0)
                    {

                    }
                    else
                    {
                        FilteredDocuments.Add(item);
                    }
                }
                return FilteredDocuments;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterDocuments Error : " + ex.Message);
                return null;
            }
        }

        public List<RiskAssessmentForm> FilterRiskAssessment(List<RiskAssessmentForm> Documents)
        {
            try
            {
                List<RiskAssessmentForm> FilteredDocuments = new List<RiskAssessmentForm>();
                foreach (var item in Documents)
                {
                    RiskAssessmentForm dbDoc = GetDocumentRiskassessmentBYID(Utility.ToString(item.RAFID));
                    if (dbDoc != null && dbDoc.RAFID > 0)
                    {

                    }
                    else
                    {
                        FilteredDocuments.Add(item);
                    }
                }
                return FilteredDocuments;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterDocuments Error : " + ex.Message);
                return null;
            }
        }

        public List<RiskAssessmentFormReviewer> FilterRiskAssessmentReviewer(List<RiskAssessmentFormReviewer> Documents)
        {
            try
            {
                List<RiskAssessmentFormReviewer> FilteredDocuments = new List<RiskAssessmentFormReviewer>();
                foreach (var item in Documents)
                {
                    RiskAssessmentFormReviewer dbDoc = GetDocumentRiskassessmentReviewerBYID(Utility.ToString(item.RAFID));
                    if (dbDoc != null && dbDoc.RAFID > 0)
                    {
                        FilteredDocuments.Add(item);
                    }
                    else
                    {
                        FilteredDocuments.Add(item);
                    }
                }
                return FilteredDocuments;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterDocuments Error : " + ex.Message);
                return null;
            }
        }

        public List<RiskAssessmentFormHazard> FilterRiskAssessmentHazard(List<RiskAssessmentFormHazard> Documents)
        {
            try
            {
                List<RiskAssessmentFormHazard> FilteredDocuments = new List<RiskAssessmentFormHazard>();
                foreach (var item in Documents)
                {
                    RiskAssessmentFormHazard dbDoc = GetDocumentRiskAssessmentFormHazardBYID(Utility.ToString(item.RAFID));
                    if (dbDoc != null && dbDoc.RAFID > 0)
                    {
                        FilteredDocuments.Add(item);
                    }
                    else
                    {
                        FilteredDocuments.Add(item);
                    }
                }
                return FilteredDocuments;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterDocuments Error : " + ex.Message);
                return null;
            }
        }

        public List<RiskAssessmentForm> GetAllDocumentRiskassessment(string shipCode)
        {
            List<RiskAssessmentForm> response = new List<RiskAssessmentForm>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " Where ShipCode=@ShipCode";
                        SqlDataAdapter da = new SqlDataAdapter(Query, conn);
                        da.SelectCommand.Parameters.AddWithValue("@ShipCode", shipCode);
                        da.Fill(dt);
                        conn.Close();
                        if (dt != null && dt.Rows.Count > 1)
                        {
                            response = dt.ToListof<RiskAssessmentForm>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentRiskassessment Error : " + ex.Message);
            }
            return response;
        }
    }
}
