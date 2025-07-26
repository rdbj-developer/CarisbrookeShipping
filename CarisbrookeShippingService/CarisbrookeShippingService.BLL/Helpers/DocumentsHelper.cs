using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class DocumentsHelper
    {
        public bool IsFirstTimeCall = false;
        List<RiskAssessmentForm> LocalRiskAssList = new List<RiskAssessmentForm>();
        SimpleObject objShip = new SimpleObject();
        public void StartDocSync()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                List<SyncedDocument> SyncedDocList = new List<SyncedDocument>();
                List<DocumentModal> APIDocsList = _helper.GetAllDocumentsForService();
                List<DocumentModal> LocalDocsList = GetLocalDocs();
                objShip = Utility.GetShipValue();
                objShip = objShip ?? new SimpleObject();
                if (APIDocsList != null && APIDocsList.Count > 0)
                {
                    RiskAssessmentFormHelper _riskHelper = new RiskAssessmentFormHelper();
                    LocalRiskAssList = _riskHelper.GetAllDocumentRiskassessment();
                    foreach (var ApiDoc in APIDocsList)
                    {
                        try
                        {
                            DocumentModal LocalDoc = LocalDocsList.Where(x => x.DocumentID == ApiDoc.DocumentID).FirstOrDefault();
                            if (LocalDoc != null)
                            {
                                // Exist in local machine - check for update
                                UpdateDocumentProcess(ApiDoc, LocalDoc, ref SyncedDocList);
                            }
                            else
                            {
                                // Not Exist in local machine - Insert New Document 
                                InsertDocumentProcess(ApiDoc, ref SyncedDocList);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("StartDocSync : Failed to Sync " + ApiDoc.Title + " file. Error : " + ex.Message);
                        }
                    }
                }
                //PrintSyncList(SyncedDocList); // RDBJ 01/21/2022 commented
                LogHelper.writelog("Document Sync Completed.");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StartDocSync : " + ex.Message);
            }
        }
        public void InsertDocumentProcess(DocumentModal ApiDoc, ref List<SyncedDocument> SyncedDocList)
        {
            try
            {
                if (ApiDoc.IsDeleted == false)
                {
                    bool isInsertedinLocalDB = InsertDocument(ApiDoc);
                    if (isInsertedinLocalDB == true)
                    {
                        bool isCreatedOnLocal = CreatePhysicalLocation(ApiDoc);
                        if (isInsertedinLocalDB == true && isCreatedOnLocal == true)
                        {
                            AddSyncList(ApiDoc, ref SyncedDocList, AppStatic.INSERTED);
                            //Add RiskAssessment Items
                            if (Convert.ToString(ApiDoc.Type).ToLower() == "xml" && Convert.ToString(ApiDoc.Path).ToLower().Contains("risk assessments"))
                            {
                                AddRiskAssessmentData(ApiDoc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentProcess : " + ex.Message);
            }
        }
        public void UpdateDocumentProcess(DocumentModal ApiDoc, DocumentModal LocalDoc, ref List<SyncedDocument> SyncedDocList)
        {
            try
            {
                if (LocalDoc.Version != ApiDoc.Version || IsFirstTimeCall == true)//|| IsFirstTimeCall == true //RDBJ 09/30/2021 Commented  || LocalDoc.Version <= ApiDoc.Version below if due to not upodate documents on local
                {
                    LocalDoc.Number = ApiDoc.Number;
                    LocalDoc.Title = ApiDoc.Title;
                    LocalDoc.Version = ApiDoc.Version;
                    LocalDoc.SectionType = ApiDoc.SectionType;
                    LocalDoc.DocNo = ApiDoc.DocNo;
                    LocalDoc.IsDeleted = ApiDoc.IsDeleted;
                    LocalDoc.ParentID = ApiDoc.ParentID;
                    bool isUpdatedDoc = UpdateDocument(LocalDoc);
                    if (isUpdatedDoc)
                    {
                        AddSyncList(ApiDoc, ref SyncedDocList, AppStatic.UPDATED);
                    }
                }
                var isNeedToDownload = false;
                if (!string.IsNullOrWhiteSpace(LocalDoc.Path))
                {
                    if (ApiDoc.Type == "FOLDER" || ApiDoc.Type == "WINDOWSFOLDER")
                    {
                        if (!Directory.Exists(LocalDoc.Path))
                            isNeedToDownload = true;
                    }
                    else
                    {
                        if (!System.IO.File.Exists(LocalDoc.Path))
                            isNeedToDownload = true;
                    }
                }
                if (LocalDoc.DocumentVersion != ApiDoc.DocumentVersion || isNeedToDownload == true || IsFirstTimeCall == true)//|| IsFirstTimeCall == true //|| (!string.IsNullOrWhiteSpace(LocalDoc.Path) && !System.IO.File.Exists(LocalDoc.Path))
                {
                    string oldFilePath = LocalDoc.Path;
                    string oldFileDownloadPath = LocalDoc.DownloadPath;
                    var objResponse = DownloadUpdatedDoc(LocalDoc, ApiDoc);
                    if (objResponse != null && objResponse.IsSuccess)
                    {
                        LocalDoc.Path = ApiDoc.Path;
                        LocalDoc.DownloadPath = ApiDoc.DownloadPath;
                        LocalDoc.UploadType = ApiDoc.UploadType;
                        LocalDoc.DocumentVersion = ApiDoc.DocumentVersion;
                        if (LocalDoc.IsDeleted != ApiDoc.IsDeleted)
                            LocalDoc.IsDeleted = ApiDoc.IsDeleted;
                        bool isUpdatedDoc = UpdateDocumentFile(LocalDoc);
                        if (objResponse.IsSuccess == true && isUpdatedDoc == true)
                        {
                            //DeleteOldFile(oldFilePath, oldFileDownloadPath);
                            DeleteOldFile(objResponse.OldFilePathToDelete, objResponse.OldServerFilePathToDelete);
                            AddSyncList(ApiDoc, ref SyncedDocList, AppStatic.UPDATED);
                            //Add RiskAssessment Items
                            if (Convert.ToString(ApiDoc.Type).ToLower() == "xml" && Convert.ToString(ApiDoc.Path).ToLower().Contains("risk assessments")) {
                                AddRiskAssessmentData(ApiDoc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDocumentProcess : " + ex.Message);
            }

        }
        public List<DocumentModal> GetLocalDocs()
        {
            List<DocumentModal> LocalDocsList = new List<DocumentModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.Documents, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            LocalDocsList = dt.ToListof<DocumentModal>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLocalDocs : " + ex.Message);
            }
            return LocalDocsList;
        }
        public bool InsertDocument(DocumentModal modal)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "INSERT INTO " + AppStatic.Documents +
                              " (DocumentID,ParentID,Number,Title,Type,Path,DownloadPath,IsDeleted,UploadType,DocumentVersion,Version,CreatedDate,UpdatedDate,SectionType,DocNo) OUTPUT INSERTED.DocID " +
                              "VALUES(@DocumentID,@ParentID,@Number,@Title,@Type,@Path,@DownloadPath,@IsDeleted,@UploadType,@DocumentVersion,@Version,@CreatedDate,@UpdatedDate,@SectionType,@DocNo)";

                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = modal.DocumentID ?? (object)DBNull.Value;
                        command.Parameters.Add("@ParentID", SqlDbType.UniqueIdentifier).Value = modal.ParentID ?? (object)DBNull.Value;
                        command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = modal.Number ?? (object)DBNull.Value;
                        command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = modal.Title ?? (object)DBNull.Value;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = modal.Type ?? (object)DBNull.Value;
                        command.Parameters.Add("@Path", SqlDbType.NVarChar).Value = modal.Path ?? (object)DBNull.Value;
                        command.Parameters.Add("@DownloadPath", SqlDbType.NVarChar).Value = modal.DownloadPath ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = modal.IsDeleted;
                        command.Parameters.Add("@UploadType", SqlDbType.NVarChar).Value = modal.UploadType ?? (object)DBNull.Value;
                        command.Parameters.Add("@Version", SqlDbType.Float).Value = modal.Version == null ? 1.0 : modal.Version;
                        command.Parameters.Add("@DocumentVersion", SqlDbType.Float).Value = modal.DocumentVersion == null ? 1.0 : modal.DocumentVersion;
                        command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@SectionType", SqlDbType.NVarChar).Value = modal.SectionType ?? (object)DBNull.Value;
                        command.Parameters.Add("@DocNo", SqlDbType.Int).Value = modal.DocNo ?? (object)DBNull.Value;
                        object resultObj = command.ExecuteScalar();
                        int databaseID = 0;
                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseID);
                        }
                        if (databaseID > 0)
                            res = true;
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocument : " + ex.Message);
                res = false;
            }
            return res;
        }
        public bool UpdateDocument(DocumentModal LocalDoc)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "UPDATE " + AppStatic.Documents +
                              " SET [Number] = @Number, [Title] = @Title, [Version] = @Version, [UpdatedDate] = @UpdatedDate,[SectionType]=@SectionType,[DocNo]=@DocNo,[IsDeleted]=@IsDeleted,[ParentID]=@ParentID WHERE [DocID] = @DocID";
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = LocalDoc.Number ?? (object)DBNull.Value;
                        command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = LocalDoc.Title ?? (object)DBNull.Value;
                        command.Parameters.Add("@Version", SqlDbType.Float).Value = LocalDoc.Version ?? (object)DBNull.Value;
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@DocID", SqlDbType.Int).Value = LocalDoc.DocID;
                        command.Parameters.Add("@SectionType", SqlDbType.NVarChar).Value = LocalDoc.SectionType ?? (object)DBNull.Value;
                        command.Parameters.Add("@DocNo", SqlDbType.Int).Value = LocalDoc.DocNo ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = LocalDoc.IsDeleted;
                        command.Parameters.Add("@ParentID", SqlDbType.UniqueIdentifier).Value = LocalDoc.ParentID ?? (object)DBNull.Value;;
                        command.ExecuteNonQuery();
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                res = false;
                LogHelper.writelog("UpdateDocument : " + ex.Message);
            }
            return res;
        }
        public bool UpdateDocumentFile(DocumentModal LocalDoc)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "UPDATE " + AppStatic.Documents +
                              " SET [Path] = @Path, [DownloadPath] = @DownloadPath, [DocumentVersion] = @DocumentVersion, [UpdatedDate] = @UpdatedDate WHERE [DocID] = @DocID";
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@Path", SqlDbType.NVarChar).Value = LocalDoc.Path ?? (object)DBNull.Value;
                        command.Parameters.Add("@DownloadPath", SqlDbType.NVarChar).Value = LocalDoc.DownloadPath ?? (object)DBNull.Value;
                        command.Parameters.Add("@DocumentVersion", SqlDbType.Float).Value = LocalDoc.DocumentVersion ?? (object)DBNull.Value;
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@DocID", SqlDbType.Int).Value = LocalDoc.DocID;
                        command.ExecuteNonQuery();
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                res = false;
                LogHelper.writelog("UpdateDocumentFile : " + ex.Message);
            }
            return res;
        }
        public bool CreatePhysicalLocation(DocumentModal modal)
        {
            bool res = false;
            try
            {
                string OfficeAPPUrl = System.Configuration.ConfigurationManager.AppSettings["OfficeAPPUrl"];
                if (string.IsNullOrEmpty(modal.DownloadPath))
                {
                    modal.DownloadPath = modal.Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                }

                string DownloadPath = @"" + OfficeAPPUrl + modal.DownloadPath;
                DownloadPath = DownloadPath.Replace("\\", "/");
                string prefixLocalPath = @"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\";
                string prefixServerPath = @"C:\inetpub\wwwroot\ShipApplication\";
                string downloadLocalPath = prefixLocalPath + modal.DownloadPath;
                string downloadServerPath = prefixServerPath + modal.DownloadPath;

                if (modal.Type != "FOLDER" && modal.Type != "folder" && modal.DownloadPath != null && modal.Type.ToLower() != "windowsfolder")
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                    //Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                    WebClient wc = new WebClient();
                    wc.DownloadFile(DownloadPath, downloadLocalPath);
                    //wc.DownloadFile(DownloadPath, downloadServerPath);
                    res = true;
                }
                else if (modal.Type == "FOLDER" || modal.Type == "Folder" || modal.Type.ToLower() == "windowsfolder")
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                    //Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CreatePhysicalLocation " + ex.Message);
                res = false;
            }
            return res;
        }
        public DocumentDownloadResponse DownloadUpdatedDoc(DocumentModal LocalDoc, DocumentModal ApiDoc)
        {
            DocumentDownloadResponse objResponse = new DocumentDownloadResponse();
            bool res = false;
            try
            {
                string oldFilePath = LocalDoc.Path;
                string oldFileName = Path.GetFileName(oldFilePath);
                string oldFileNameOrig = Path.GetFileNameWithoutExtension(oldFilePath);
                string oldFileExtension = Path.GetExtension(oldFilePath);
                string newFilePath = "";
                try
                {
                    // Delete Old Files
                    newFilePath = oldFilePath.Replace(oldFileName, oldFileNameOrig + "_" + LocalDoc.DocumentVersion + oldFileExtension);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Move(oldFilePath, newFilePath);
                }
                catch (Exception)
                { }
                objResponse.OldFilePathToDelete = newFilePath;
                string oldFileDownloadPath = LocalDoc.DownloadPath;
                string prefixServerPath = @"C:\inetpub\wwwroot\ShipApplication\";
                string ServerDeletePath = prefixServerPath + oldFileDownloadPath;
                try
                {

                    newFilePath = ServerDeletePath.Replace(oldFileName, oldFileNameOrig + "_" + LocalDoc.DocumentVersion + oldFileExtension);
                    if (System.IO.File.Exists(ServerDeletePath))
                        System.IO.File.Move(ServerDeletePath, newFilePath);

                    //System.IO.File.Delete(ServerDeletePath);
                }
                catch (Exception)
                { }
                objResponse.OldFilePathToDelete = newFilePath;
                string OfficeAPPUrl = System.Configuration.ConfigurationManager.AppSettings["OfficeAPPUrl"];
                string DownloadPath = @"" + OfficeAPPUrl + ApiDoc.DownloadPath;
                string downloadServerPath = prefixServerPath + ApiDoc.DownloadPath;
                string downloadLocalPath = ApiDoc.Path;
                if (ApiDoc.Type != "FOLDER" && ApiDoc.Type != "Folder" && ApiDoc.DownloadPath != null && ApiDoc.Type.ToLower() != "windowsfolder")
                {
                    // Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                    WebClient wc = new WebClient();
                    //wc.DownloadFile(DownloadPath, downloadServerPath);
                    wc.DownloadFile(DownloadPath, downloadLocalPath);
                    res = true;
                }
                else if (ApiDoc.Type == "FOLDER" || ApiDoc.Type == "Folder" || ApiDoc.Type.ToLower() == "windowsfolder")
                {
                    // Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                    res = true;
                }
                // Create New Files                

            }
            catch (Exception ex)
            {
                LogHelper.writelog("DownloadUpdatedDoc : " + ex.Message);
                LogHelper.writelog("DownloadUpdatedDoc Failed to download : " + ApiDoc.Path);
            }
            objResponse.IsSuccess = res;
            return objResponse;
        }
        public bool DeleteOldFile(string FilePath, string DownloadPath)
        {
            bool res = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(FilePath))
                {
                    if (System.IO.File.Exists(FilePath))
                        System.IO.File.Delete(FilePath);
                }

                if (!string.IsNullOrWhiteSpace(DownloadPath))
                {
                    string prefixServerPath = @"C:\inetpub\wwwroot\ShipApplication\";
                    string ServerDeletePath = prefixServerPath + DownloadPath;
                    if (System.IO.File.Exists(ServerDeletePath))
                        System.IO.File.Delete(ServerDeletePath);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteOldFile :" + ex.Message);
            }
            return res;
        }
        public void AddSyncList(DocumentModal modal, ref List<SyncedDocument> SyncDocList, string SyncedType)
        {
            try
            {
                SyncedDocument SyncedDoc = new SyncedDocument();
                SyncedDoc.DocumentID = modal.DocumentID.ToString();
                SyncedDoc.ParentID = modal.ParentID.ToString();
                SyncedDoc.Title = modal.Title;
                SyncedDoc.DocType = modal.Type;
                SyncedDoc.SyncType = SyncedType;
                SyncedDoc.SyncDateTime = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                SyncDocList.Add(SyncedDoc);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SyncList : " + ex.Message);
            }
        }
        public void PrintSyncList(List<SyncedDocument> SyncDocList)
        {
            if (SyncDocList != null && SyncDocList.Count > 0)
            {
                //LogHelper.writelog("Total Synced Documents " + SyncDocList.Count + " At " + DateTime.Now);
                LogHelper.writelog("Total Synced Documents " + SyncDocList.Count + " At " + Utility.ToDateTimeUtcNow()); //RDBJ 10/27/2021 set UtcTime
                foreach (var syncDoc in SyncDocList)
                {
                    try
                    {
                        LogHelper.writelog(syncDoc.DocumentID + " " + syncDoc.Title + " " + syncDoc.SyncType + " At " + Utility.ToDateTimeUtcNow()); //RDBJ 10/27/2021 set UtcTime
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("PrintSyncList" + ex.Message);
                    }
                }
            }
        }

        private void AddRiskAssessmentData(DocumentModal ApiDoc) {
            try
            {
                RAFormModal objRiskModel = new RAFormModal();
                var riskdata = LocalRiskAssList.Where(x => x.Title == ApiDoc.Title).FirstOrDefault();
                if (riskdata != null)
                    return;
                List<RiskAssessmentFormReviewer> lstReviewer = new List<RiskAssessmentFormReviewer>();
                List<RiskAssessmentFormHazard> lstHazards = new List<RiskAssessmentFormHazard>();
                var Modal = new RiskAssessmentForm();
                Guid newGuid = ApiDoc.DocumentID.Value;
                Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.Number = Convert.ToString(ApiDoc.Number).Trim();
                Modal.Title = ApiDoc.Title;
                Modal.ShipName = objShip.name;
                Modal.ShipCode = objShip.id;
                Modal.CreatedBy = "CarisbrookShippingService";
                if (File.Exists(ApiDoc.Path))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(ApiDoc.Path);
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
                                for (var i = 0; i < MemberNodes.Count; i++)
                                {
                                    try
                                    {
                                        RiskAssessmentFormReviewer ModalReviewer = new RiskAssessmentFormReviewer();
                                        string Description1 = MemberNodes[i].InnerText.Trim();
                                        ModalReviewer.ReviewerName = Convert.ToString(Description1).Trim();
                                        lstReviewer.Add(ModalReviewer);
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
                                lstHazards.Add(ModalHazard);
                            }
                        }
                    }
                    objRiskModel.RiskAssessmentForm = Modal;
                    objRiskModel.RiskAssessmentFormHazardList = lstHazards;
                    objRiskModel.RiskAssessmentFormReviewerList = lstReviewer;
                    RiskAssessmentFormHelper _riskHelper = new RiskAssessmentFormHelper();
                    _riskHelper.InsertRiskAssessmentAllData(objRiskModel);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddRiskAssessmentData : " + ex.Message);
            }
        }
        
    }
}
