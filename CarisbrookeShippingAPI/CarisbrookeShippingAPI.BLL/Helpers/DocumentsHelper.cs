using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class DocumentsHelper
    {
        string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
        public List<DocumentsModal> GetAllDocuments(string sectionType = "")
        {
            List<DocumentsModal> list = new List<DocumentsModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Document> dbList = new List<Document>();
            dbList = dbContext.Documents.Where(x => x.IsDeleted == false).ToList();
            if (!string.IsNullOrWhiteSpace(sectionType))
                dbList = dbList.Where(x => (Utility.ToString(x.SectionType).ToUpper() == "" && Utility.ToString(sectionType).ToUpper() == "ISM")
                    || (Utility.ToString(x.SectionType).ToUpper() == Utility.ToString(sectionType).ToUpper())).ToList();
            list = dbList.Select(x => new DocumentsModal()
            {
                DocID = x.DocID,
                DocumentID = x.DocumentID,
                ParentID = x.ParentID,
                Number = x.Number,
                Title = x.Title,
                Type = x.Type,
                Path = x.Path,
                IsDeleted = x.IsDeleted.HasValue ? x.IsDeleted.Value : false,
                DownloadPath = x.DownloadPath,
                UploadType = x.UploadType,
                DocumentVersion = x.DocumentVersion,
                Version = x.Version,
                DocNo = x.DocNo,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                SectionType = x.SectionType
            }).OrderBy(x => x.SectionType).ToList();
            return list;
        }

        public List<DocumentsModal> GetAllDocumentsForShip(string sectionType = "", string shipCode = "")
        {
            List<DocumentsModal> list = new List<DocumentsModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<DocumentsModal> dbList = new List<DocumentsModal>();
            using (var conn = new SqlConnection(connetionString))
            {
                //DataTable dt = new DataTable();
                //string Query = "SELECT Top 1 D.DocID, D.DocumentID, D.ParentID, D.Number, D.DocNo, D.Title, D.Type, D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.SectionType, R.RAFID " +
                //               " FROM Documents D outer Apply(select R.RAFID, R.Number, R.Title, R.Type, R.DocumentID, R.ParentID, R.SectionType From RiskAssessmentForm R " +
                //               " Where R.Number = D.Number AND R.Title = D.Title AND R.ShipCode='" + shipCode + "' )R Union Select D.DocID, R.DocumentID, R.ParentID, R.Number, D.DocNo, R.Title, R.Type, D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType, R.RAFID " +
                //               " FROM RiskAssessmentForm R " +
                //               "Left Join Documents D on D.Number = R.Number AND D.Title = R.Title Where D.Title IS NULL AND R.ShipCode='" + shipCode + "'";
                //SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                //sqlAdp.Fill(dt);
                //if (dt != null && dt.Rows.Count > 0)
                //    dbList = dt.ToListof<DocumentsModal>();
                using (SqlCommand cmd = new SqlCommand("GetAllDocumentsForShip", conn))
                {
                    cmd.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = shipCode;
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            dbList = dt.ToListof<DocumentsModal>();
                    }
                }
            }
            if (dbList == null)
                dbList = new List<DocumentsModal>();
            // dbList = dbContext.Documents.Where(x => x.IsDeleted == false).ToList();
            if (!string.IsNullOrWhiteSpace(sectionType))
                dbList = dbList.Where(x => (Utility.ToString(x.SectionType).ToUpper() == "" && Utility.ToString(sectionType).ToUpper() == "ISM")
                    || (Utility.ToString(x.SectionType).ToUpper() == Utility.ToString(sectionType).ToUpper())).ToList();
            list = dbList.Select(x => new DocumentsModal()
            {
                DocID = x.DocID,
                DocumentID = x.DocumentID,
                ParentID = x.ParentID,
                Number = x.Number,
                Title = x.Title,
                Type = x.Type,
                Path = x.Path,
                IsDeleted = x.IsDeleted,
                DownloadPath = x.DownloadPath,
                UploadType = x.UploadType,
                DocumentVersion = x.DocumentVersion,
                Version = x.Version,
                DocNo = x.DocNo,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                SectionType = x.SectionType,
                RAFID = x.RAFID,
                RAFId1 = x.RAFId1,
                //IsWebPage = x.IsWebPage
            }).OrderBy(x => x.SectionType).ToList();
            return list;
        }

        public List<DocumentsModal> GetAllDocumentsForOfficeApp(string sectionType = "")
        {
            List<DocumentsModal> list = new List<DocumentsModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Document> dbList = new List<Document>();
            dbList = dbContext.Documents.Where(x => x.IsDeleted == false).ToList();
            if (!string.IsNullOrWhiteSpace(sectionType))
                dbList = dbList.Where(x => (Utility.ToString(x.SectionType).ToUpper() == "" && Utility.ToString(sectionType).ToUpper() == "ISM")
                    || (Utility.ToString(x.SectionType).ToUpper() == Utility.ToString(sectionType).ToUpper())).ToList();
            list = dbList.Select(x => new DocumentsModal()
            {
                DocID = x.DocID,
                DocumentID = x.DocumentID,
                ParentID = x.ParentID,
                Number = x.Number,
                Title = x.Title,
                Type = x.Type,
                Path = x.Path,
                IsDeleted = x.IsDeleted,
                DownloadPath = x.DownloadPath,
                UploadType = x.UploadType,
                DocumentVersion = x.DocumentVersion,
                Version = x.Version,
                DocNo = x.DocNo,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                SectionType = x.SectionType
            }).OrderBy(x => x.SectionType).ToList();
            return list;
        }
        public List<DocumentsModal> GetAllDocumentsWithRAData(string shipCode)
        {
            List<DocumentsModal> list = new List<DocumentsModal>();
            using (var conn = new SqlConnection(connetionString))
            {
                DataTable dt = new DataTable();
                string Query = "SELECT D.DocID, D.DocumentID, D.ParentID, D.Number, D.DocNo, D.Title, D.Type, D.Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.SectionType, ISNULL(R.RAFID,0) as RAFID " +
                                       "FROM Documents D outer Apply(select top 1 R.RAFID, R.Number, R.Title, R.Type, R.DocumentID, R.ParentID, R.SectionType From RiskAssessmentForm R " +
                                       " Where R.Number = D.Number AND R.Title = D.Title AND (@ShipCode = '' OR R.ShipCode=@ShipCode) )R Where IsNULL(D.IsDeleted,0)=0 Union All " + 
                                       "Select D.DocID, D.DocumentID, ISNULL(R.ParentID,D.ParentID) as ParentID, R.Number" +
                                       //", D.DocNo" +  // JSL 09/06/2022 commented this line
                                       //", ISNULL(R.DocNo, LTRIM(SUBSTRING(R.Number,3,5))) AS 'DocNo'" +   // JSL 02/25/2023 commented   // JSL 09/06/2022
                                       //", ISNULL(R.DocNo, LEFT(SUBSTRING(LTRIM(SUBSTRING(R.Number,3,5)), PATINDEX('%[0-9]%', LTRIM(SUBSTRING(R.Number,3,5))), 8000), PATINDEX('%[^0-9]%', SUBSTRING(LTRIM(SUBSTRING(R.Number,3,5)), PATINDEX('%[0-9]%', LTRIM(SUBSTRING(R.Number,3,5))), 8000) + 'X') -1)) AS 'DocNo'" +  // JSL 02/25/2023 added and commented, use this if below failed
                                       ", ISNULL(R.DocNo, LEFT(SUBSTRING(R.Number, PATINDEX('%[0-9]%', R.Number), 8000), PATINDEX('%[^0-9]%', SUBSTRING(R.Number, PATINDEX('%[0-9]%', R.Number), 8000) + 'X') -1)) AS 'DocNo'" +    // JSL 02/25/2023
                                       ", R.Title, 'XML' as [Type], NULL as Path, D.DownloadPath, D.IsDeleted, D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType, ISNULL(R.RAFID,0) as RAFID " +
                                       "FROM (Select D.DocID, R.Number, D.DocNo, R.Title, R.Type, D.Path, D.DownloadPath, D.IsDeleted,D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, R.SectionType,ISNULL(R.RAFID,0) as RAFID ,R.ParentID  " +
                                       "FROM RiskAssessmentForm R Left Join Documents D on D.Number = R.Number AND D.Title = R.Title Where D.Title IS NULL AND (@ShipCode = '' OR R.ShipCode=@ShipCode) AND IsNULL(D.IsDeleted,0)=0 ) R " +
                                       "Outer Apply( Select Top 1 D.DocID,D.DocNo,  D.DocumentID, D.ParentID,D.Path, D.DownloadPath,D.UploadType, D.DocumentVersion, D.Version , D.Location,D.CreatedDate, D.UpdatedDate, D.IsDeleted   from Documents D " +
                                       //"Where SUBSTRING (LTRIM(D.Number),1,1) = SUBSTRING (LTRIM(R.Number),1,1) AND IsNULL(D.IsDeleted,0)=0  )D"  // JSL 09/06/2022 commented this line
                                       "Where (LTRIM(SUBSTRING(D.Number,3,5)) =  LTRIM(SUBSTRING(R.Number,3,5))  OR LTRIM(SUBSTRING(D.Number,1,1)) =  LTRIM(SUBSTRING(R.Number,1,1))) AND IsNULL(D.IsDeleted,0)=0  )D"    // JSL 09/06/2022
                                       + " ORDER BY ParentID, DocNo"    // JSL 09/06/2022
                                       ;

                //ISNULL(NULLIF(D.ParentID,'00000000-0000-0000-0000-000000000000'),R.ParentID)
                SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", shipCode);
                sqlAdp.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                    list = dt.ToListof<DocumentsModal>();
            }
            if (list == null)
                list = new List<DocumentsModal>();
            return list;
        }

        public List<DocumentsModal> GetAllDocumentsForService(bool isGetAllRecord = false)
        {
            List<DocumentsModal> list = new List<DocumentsModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Document> dbList = new List<Document>();
            //string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
            //using (var conn = new SqlConnection(connetionString))
            //{
            //    DataTable dt = new DataTable();
            //    string Query = "SELECT s.*, R.RAFID as RAFId FROM Documents s outer Apply(select top 1 RAFID From RiskAssessmentForm R " +
            //                   " Where R.Title = s.Title )R WHERE IsDeleted = 0";
            //    SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
            //    sqlAdp.Fill(dt);
            //    if (dt != null && dt.Rows.Count > 0)
            //    dbList = dt.ToListof<DocumentsModal>();
            //}
            //if (dbList == null)
            //    dbList = new List<DocumentsModal>();
            //dbList = dbContext.Documents.ToList(); //.Where(x => x.IsDeleted == false)
            if (isGetAllRecord)
                dbList = dbContext.Documents.ToList();
            else
                dbList = dbContext.Documents.Where(x => x.IsDeleted == false).ToList();
            list = dbList.Select(x => new DocumentsModal()
            {
                DocID = x.DocID,
                DocumentID = x.DocumentID,
                ParentID = x.ParentID,
                Number = x.Number,
                Title = x.Title,
                Type = x.Type,
                Path = x.Path,
                IsDeleted = x.IsDeleted,
                DownloadPath = x.DownloadPath,
                UploadType = x.UploadType,
                DocumentVersion = x.DocumentVersion,
                Version = x.Version,
                DocNo = x.DocNo,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                SectionType = x.SectionType,
                RAFID = x.RAFID,
                // RAFId1 = x.RAFId1,
                //  IsWebPage = x.IsWebPage
            }).ToList();
            return list;
        }
        public DocumentsModal GetDocumentByID(string id)
        {
            DocumentsModal DocModal = new DocumentsModal();
            Guid result = new Guid(id);
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Document dbDoc = dbContext.Documents.Where(x => x.DocumentID == result).FirstOrDefault();
            if (dbDoc != null)
            {
                DocModal.DocID = dbDoc.DocID;
                DocModal.DocumentID = dbDoc.DocumentID;
                DocModal.ParentID = dbDoc.ParentID;
                DocModal.Number = dbDoc.Number;
                DocModal.Title = dbDoc.Title;
                DocModal.Type = dbDoc.Type;

                DocModal.Path = dbDoc.Path;
                DocModal.IsDeleted = dbDoc.IsDeleted;
                DocModal.DownloadPath = dbDoc.DownloadPath;
                DocModal.UploadType = dbDoc.UploadType;
                DocModal.DocumentVersion = dbDoc.DocumentVersion;
                DocModal.Version = dbDoc.Version;
                DocModal.DocNo = dbDoc.DocNo;
                DocModal.CreatedDate = dbDoc.CreatedDate;
                DocModal.UpdatedDate = dbDoc.UpdatedDate;
                DocModal.SectionType = dbDoc.SectionType;
            }
            return DocModal;
        }
        public void AddDocument(DocumentsModal Modal)
        {
            Document dbDoc = new Document();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Document> dbDocList = dbContext.Documents.Where(x => x.ParentID == Modal.ParentID).ToList();

            dbDoc.DocumentID = Modal.DocumentID;
            dbDoc.ParentID = Modal.ParentID;
            dbDoc.Number = Modal.Number;
            dbDoc.Title = Modal.Title;
            dbDoc.Type = Modal.Type;
            dbDoc.Path = Modal.Path;
            dbDoc.IsDeleted = Modal.IsDeleted;
            dbDoc.DownloadPath = Modal.DownloadPath;
            dbDoc.UploadType = Modal.UploadType;
            dbDoc.DocumentVersion = Modal.DocumentVersion;
            dbDoc.Version = Modal.Version;
            dbDoc.Location = Modal.Location;
            if (Modal.DocNo == null || Modal.DocNo == 0)
            {
                // If DocNo is not mentioned
                if (dbDocList == null)
                    dbDoc.DocNo = 1;
                else
                    dbDoc.DocNo = dbDocList.Count + 1;
            }
            else if (Utility.ToInteger(Modal.DocNo) > 0)
            {
                // If DocNo is mentioned
                Document dbDocUpdate = dbDocList.Where(x => x.DocNo == Modal.DocNo).FirstOrDefault();
                if (dbDocUpdate != null)
                {
                    // If DocNo is already existing in database
                    List<Document> NextdbDocList = dbDocList.Where(x => x.DocNo >= Modal.DocNo).ToList().OrderBy(x => x.DocNo).ToList();
                    foreach (var item in NextdbDocList)
                    {
                        item.DocNo = item.DocNo + 1;
                    }
                    dbDoc.DocNo = Modal.DocNo;
                }
                else
                {
                    // If DocNo is not existing in database
                    dbDoc.DocNo = Modal.DocNo;
                }
            }
            dbDoc.CreatedDate = Modal.CreatedDate;
            dbDoc.UpdatedDate = Modal.UpdatedDate;
            dbDoc.SectionType = Modal.SectionType;
            dbContext.Documents.Add(dbDoc);
            dbContext.SaveChanges();
        }
        public void UpdateDocument(DocumentsModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Document dbDoc = dbContext.Documents.Where(x => x.DocumentID == Modal.DocumentID).FirstOrDefault();
            if (dbDoc != null)
            {
                dbDoc.Number = Modal.Number;
                dbDoc.Title = Modal.Title;
                dbDoc.UploadType = Modal.UploadType;
                dbDoc.Version = dbDoc.Version + 0.1;
                int currentDocNo = Utility.ToInteger(dbDoc.DocNo);
                int changedDocNo = Utility.ToInteger(Modal.DocNo);
                if (dbDoc.DocNo != Modal.DocNo && currentDocNo > 0 && changedDocNo > 0)
                {
                    List<Document> dbDocList = dbContext.Documents.Where(x => x.ParentID == Modal.ParentID).ToList();
                    if (currentDocNo > changedDocNo)
                    {
                        dbDocList = dbDocList.Where(x => x.DocNo <= currentDocNo && x.DocNo >= changedDocNo).ToList().OrderBy(x => x.DocNo).ToList();
                        foreach (var item1 in dbDocList)
                        {
                            if (item1.DocNo == currentDocNo)
                            {
                                item1.DocNo = changedDocNo;
                            }
                            else if (item1.DocNo < currentDocNo && item1.DocNo >= changedDocNo)
                            {
                                item1.DocNo = item1.DocNo + 1;
                            }
                        }
                    }
                    if (currentDocNo < changedDocNo)
                    {
                        dbDocList = dbDocList.Where(x => x.DocNo >= currentDocNo && x.DocNo <= changedDocNo).ToList().OrderBy(x => x.DocNo).ToList();
                        foreach (var item1 in dbDocList)
                        {
                            if (item1.DocNo == currentDocNo)
                            {
                                item1.DocNo = changedDocNo;
                            }
                            else if (item1.DocNo > currentDocNo && item1.DocNo <= changedDocNo)
                            {
                                item1.DocNo = item1.DocNo - 1;
                            }
                        }
                    }
                }
                else
                {
                    dbDoc.DocNo = Modal.DocNo;
                }
                dbDoc.SectionType = Modal.SectionType;
                dbDoc.UpdatedDate = Modal.UpdatedDate;
                dbContext.SaveChanges();
            }
        }
        public void UpdateDocumentFile(DocumentsModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Document dbDoc = dbContext.Documents.Where(x => x.DocumentID == Modal.DocumentID).FirstOrDefault();
            if (dbDoc != null)
            {
                dbDoc.Path = Modal.Path;
                dbDoc.DownloadPath = Modal.DownloadPath;
                dbDoc.Type = Modal.Type;
                dbDoc.DocumentVersion = dbDoc.DocumentVersion + 0.1;
                dbDoc.UploadType = AppStatic.UPDATED;
                dbDoc.UpdatedDate = Modal.UpdatedDate;
                dbDoc.SectionType = Modal.SectionType;
                dbContext.SaveChanges();
            }
        }
        public void DeleteDocument(string id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid docID = Guid.Parse(id);
            Document dbDoc = dbContext.Documents.Where(x => x.DocumentID == docID).FirstOrDefault();
            if (dbDoc != null)
            {
                string Type = dbDoc.Type;
                if (Type == "FOLDER" || Type == "Folder")
                {
                    List<Document> DoclIst = dbContext.Documents.Where(x => x.ParentID == dbDoc.DocumentID).ToList();
                    if (DoclIst != null)
                    {
                        foreach (var item in DoclIst)
                        {
                            item.IsDeleted = true;
                            item.UploadType = AppStatic.REMOVED;
                        }
                    }
                }

                // Change the Doc No
                int DbDocNo = Utility.ToInteger(dbDoc.DocNo);
                if (DbDocNo > 0)
                {
                    List<Document> DBDocList = dbContext.Documents.Where(x => x.ParentID == dbDoc.ParentID && x.DocNo > DbDocNo).OrderBy(x => x.DocNo).ToList();
                    if (DBDocList != null)
                    {
                        foreach (var item in DBDocList)
                        {
                            item.DocNo = item.DocNo - 1;
                        }
                    }
                    dbDoc.DocNo = null;
                }
                dbDoc.IsDeleted = true;
                dbDoc.Version = dbDoc.Version + 0.1;
                dbContext.SaveChanges();
            }
        }

        #region Sync Documnets
        public bool AddSyncDocuments(List<DocumentsModal> DocList)
        {
            bool res = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                foreach (var Modal in DocList)
                {
                    Document dbDoc = new Document();
                    dbDoc.DocumentID = Modal.DocumentID;
                    dbDoc.ParentID = Modal.ParentID;
                    dbDoc.Number = Modal.Number;
                    dbDoc.Title = Modal.Title;
                    dbDoc.Type = Modal.Type;
                    dbDoc.Path = Modal.Path;
                    dbDoc.DownloadPath = Modal.DownloadPath;
                    dbDoc.IsDeleted = false;
                    dbDoc.UploadType = AppStatic.NEW;
                    dbDoc.DocumentVersion = 1.0;
                    dbDoc.Version = 1.0;
                    dbDoc.Location = "DOCFILE";
                    dbDoc.CreatedDate = Modal.CreatedDate;
                    dbDoc.UpdatedDate = Modal.UpdatedDate;
                    dbDoc.SectionType = Modal.SectionType;
                    dbContext.Documents.Add(dbDoc);
                }
                dbContext.SaveChanges();
                dbContext.Dispose();
                res = true;
            }
            catch (Exception ex)
            {

            }
            return res;
        }
        #endregion

        #region Repositiories - Not Working
        public List<RepositoryModal> GetAllRepositories()
        {
            List<RepositoryModal> list = new List<RepositoryModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Repository> dbList = new List<Repository>();
            dbList = dbContext.Repositories.ToList();
            list = dbList.Select(x => new RepositoryModal()
            {
                RepID = x.RepID,
                DocumentID = x.DocumentID.ToString(),
                ParentID = x.ParentID.ToString(),
                Name = x.Name,
                Type = x.Type,
                Extension = x.Extension,
                Path = x.Path,
                Version = x.Version,
            }).ToList();
            return list;
        }
        public void AddNewRepository()
        {
            List<RepositoryModal> FolderList = new List<RepositoryModal>();
            try
            {
                string path = @"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\Repository";
                ListDirectories(path, Guid.Empty.ToString(), ref FolderList);

                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                foreach (var item in FolderList)
                {
                    Repository dbRepo = new Repository();
                    dbRepo.DocumentID = new Guid(item.DocumentID);
                    dbRepo.ParentID = new Guid(item.ParentID);
                    dbRepo.Name = item.Name;
                    dbRepo.Type = item.Type;
                    dbRepo.Extension = item.Extension;
                    dbRepo.Path = item.Path;
                    dbRepo.Version = item.Version;
                    dbRepo.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbRepo.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime

                    dbContext.Repositories.Add(dbRepo);
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddNewRepository :" + ex.Message);
            }
        }
        void ListDirectories(string path, string parentid, ref List<RepositoryModal> FolderList)
        {
            var directories = Directory.GetDirectories(path);
            if (directories.Any())
            {
                foreach (var directory in directories)
                {
                    string docID = Guid.NewGuid().ToString();
                    var di = new DirectoryInfo(directory);
                    FolderList.Add(new RepositoryModal { DocumentID = docID, ParentID = parentid, Name = di.Name, Type = "FOLDER", Extension = string.Empty, Version = 1.0, Path = directory });
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        string docFileID = Guid.NewGuid().ToString();
                        FolderList.Add(new RepositoryModal { DocumentID = docFileID, ParentID = docID, Name = Path.GetFileName(file), Type = "File", Extension = Path.GetExtension(file).ToUpper(), Version = 1.0, Path = file });
                    }
                    ListDirectories(directory, docID, ref FolderList);
                }
            }
        }
        #endregion

        #region AssetManagmentEquipmentList
        public void SubmitAssetManagmentEquipmentListData(AssetManagmentEquipmentListModal Modal)
        {
            try
            {
                bool isNeedToAddLog = false;
                //if (!Modal.IsFromOfficeApp)
                isNeedToAddLog = true;

                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                AssetManagmentEquipmentList dbModal = new AssetManagmentEquipmentList();
                AssetManagmentEquipmentLog dbLogModal = null;
                if (Modal != null && !string.IsNullOrWhiteSpace(Modal.AssetManagmentEquipmentListForm.ShipCode))
                {
                    dbModal = dbContext.AssetManagmentEquipmentLists.Where(x => x.ShipCode == Modal.AssetManagmentEquipmentListForm.ShipCode).FirstOrDefault();
                    if (dbModal != null)
                    {
                        if (isNeedToAddLog)
                        {
                            dbLogModal = new AssetManagmentEquipmentLog
                            {
                                AMEId = dbModal.AMEId,
                                CreatedBy = dbModal.CreatedBy,
                                CreatedDate = dbModal.CreatedDate,
                                LogDate = Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                ShipCode = dbModal.ShipCode,
                                ShipName = dbModal.ShipName,
                                UpdatedBy = dbModal.UpdatedBy,
                                UpdatedDate = dbModal.UpdatedDate,
                                Id = Guid.NewGuid()
                            };
                        }

                        Modal.AssetManagmentEquipmentListForm.AMEId = dbModal.AMEId;
                        dbModal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        dbModal.ShipName = Modal.AssetManagmentEquipmentListForm.ShipName;
                        dbModal.ShipCode = Modal.AssetManagmentEquipmentListForm.ShipCode;
                        dbModal.UpdatedBy = string.IsNullOrWhiteSpace(Modal.AssetManagmentEquipmentListForm.UpdatedBy) ? Modal.AssetManagmentEquipmentListForm.CreatedBy : Modal.AssetManagmentEquipmentListForm.UpdatedBy;
                        dbModal.UpdatedDate = Modal.AssetManagmentEquipmentListForm.UpdatedDate.HasValue ? Modal.AssetManagmentEquipmentListForm.UpdatedDate : Modal.AssetManagmentEquipmentListForm.CreatedDate;
                        dbModal.SavedAsDraft = Modal.AssetManagmentEquipmentListForm.SavedAsDraft;
                        dbModal.IsSynced = Modal.AssetManagmentEquipmentListForm.IsSynced;
                    }
                    else
                    {
                        if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                            Modal.AssetManagmentEquipmentListForm.AMEId = Guid.NewGuid();
                        dbModal = new AssetManagmentEquipmentList();
                        dbModal.AMEId = Modal.AssetManagmentEquipmentListForm.AMEId;
                        dbModal.CreatedBy = Modal.AssetManagmentEquipmentListForm.CreatedBy;
                        dbModal.CreatedDate = Modal.AssetManagmentEquipmentListForm.CreatedDate;
                        dbModal.IsSynced = Modal.AssetManagmentEquipmentListForm.IsSynced;
                        dbModal.ShipCode = Modal.AssetManagmentEquipmentListForm.ShipCode;
                        dbModal.ShipName = Modal.AssetManagmentEquipmentListForm.ShipName;
                        dbModal.UpdatedBy = Modal.AssetManagmentEquipmentListForm.UpdatedBy;
                        dbModal.UpdatedDate = Modal.AssetManagmentEquipmentListForm.UpdatedDate;
                        dbContext.AssetManagmentEquipmentLists.Add(dbModal);
                    }
                }
                else
                {
                    if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                        Modal.AssetManagmentEquipmentListForm.AMEId = Guid.NewGuid();
                    dbModal = new AssetManagmentEquipmentList();
                    dbModal.AMEId = Modal.AssetManagmentEquipmentListForm.AMEId;
                    dbModal.CreatedBy = Modal.AssetManagmentEquipmentListForm.CreatedBy;
                    dbModal.CreatedDate = Modal.AssetManagmentEquipmentListForm.CreatedDate;
                    dbModal.IsSynced = Modal.AssetManagmentEquipmentListForm.IsSynced;
                    dbModal.ShipCode = Modal.AssetManagmentEquipmentListForm.ShipCode;
                    dbModal.ShipName = Modal.AssetManagmentEquipmentListForm.ShipName;
                    dbModal.UpdatedBy = Modal.AssetManagmentEquipmentListForm.UpdatedBy;
                    dbModal.UpdatedDate = Modal.AssetManagmentEquipmentListForm.UpdatedDate;
                    dbContext.AssetManagmentEquipmentLists.Add(dbModal);
                }
                dbContext.SaveChanges();
                if (Modal.AssetManagmentEquipmentOTListModel != null && Modal.AssetManagmentEquipmentOTListModel.Count > 0)
                {
                    if (Modal != null && Modal.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                    {
                        List<AssetManagmentEquipmentOTList> dbAssetOTListModal = dbContext.AssetManagmentEquipmentOTLists.Where(x => x.AMEId == Modal.AssetManagmentEquipmentListForm.AMEId).ToList();
                        //Remove in DB
                        if (dbAssetOTListModal != null && dbAssetOTListModal.Count > 0)
                        {
                            //Log Maintain
                            if (dbLogModal != null)
                                dbLogModal.OTTableList = JsonConvert.SerializeObject(dbAssetOTListModal);

                            foreach (var item in dbAssetOTListModal)
                            {
                                dbContext.AssetManagmentEquipmentOTLists.Remove(item);
                            }
                            dbContext.SaveChanges();
                        }
                    }
                    //inser in DB
                    foreach (var item in Modal.AssetManagmentEquipmentOTListModel)
                    {
                        if (string.IsNullOrWhiteSpace(item.OTEquipment))
                            continue;
                        dbContext.AssetManagmentEquipmentOTLists.Add(new AssetManagmentEquipmentOTList
                        {
                            OTId = Guid.NewGuid(),
                            AMEId = Modal.AssetManagmentEquipmentListForm.AMEId,
                            OTEquipment = item.OTEquipment,
                            OTLastServiced = item.OTLastServiced,
                            OTLocation = item.OTLocation,
                            OTMake = item.OTMake,
                            OTModel = item.OTModel,
                            OTType = item.OTType,
                            OTSerialNo = item.OTSerialNo,
                            OTWorkingCondition = item.OTWorkingCondition,
                            OTRemark = item.OTRemark,
                            OTHardwareId = item.OTHardwareId,
                            OTOwner = item.OTOwner,
                            OTPersonResponsible = item.OTPersonResponsible,
                            OTCriticality = item.OTCriticality,
                            OTOperatingSystem = item.OTOperatingSystem,
                            OTOSPatchVersion = item.OTOSPatchVersion
                        });
                    }
                    dbContext.SaveChanges();
                }

                if (Modal.AssetManagmentEquipmentITListModel != null && Modal.AssetManagmentEquipmentITListModel.Count > 0)
                {
                    if (Modal != null && Modal.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                    {
                        List<AssetManagmentEquipmentITList> dbAssetITListModal = dbContext.AssetManagmentEquipmentITLists.Where(x => x.AMEId == Modal.AssetManagmentEquipmentListForm.AMEId).ToList();
                        //Remove in DB
                        if (dbAssetITListModal != null && dbAssetITListModal.Count > 0)
                        {
                            //Log Maintain
                            if (dbLogModal != null)
                                dbLogModal.ITTableList = JsonConvert.SerializeObject(dbAssetITListModal);

                            foreach (var item in dbAssetITListModal)
                            {
                                dbContext.AssetManagmentEquipmentITLists.Remove(item);
                            }
                            dbContext.SaveChanges();
                        }
                    }

                    //insert in DB
                    foreach (var item in Modal.AssetManagmentEquipmentITListModel)
                    {
                        if (string.IsNullOrWhiteSpace(item.ITEquipment))
                            continue;
                        dbContext.AssetManagmentEquipmentITLists.Add(new AssetManagmentEquipmentITList
                        {
                            ITId = Guid.NewGuid(),
                            AMEId = Modal.AssetManagmentEquipmentListForm.AMEId,
                            ITEquipment = item.ITEquipment,
                            ITLastServiced = item.ITLastServiced,
                            ITLocation = item.ITLocation,
                            ITMake = item.ITMake,
                            ITModel = item.ITModel,
                            ITType = item.ITType,
                            ITSerialNo = item.ITSerialNo,
                            ITWorkingCondition = item.ITWorkingCondition,
                            ITRemark = item.ITRemark,
                            ITHardwareId = item.ITHardwareId,
                            ITOwner = item.ITOwner,
                            ITPersonResponsible = item.ITPersonResponsible,
                            ITCriticality = item.ITCriticality,
                            ITOperatingSystem = item.ITOperatingSystem,
                            ITOSPatchVersion = item.ITOSPatchVersion
                        });
                    }
                    dbContext.SaveChanges();
                }

                if (Modal != null && Modal.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                {
                    List<AssetManagmentEquipmentSoftwareAsset> dbSoftwareAssetModal = dbContext.AssetManagmentEquipmentSoftwareAssets.Where(x => x.AMEId == Modal.AssetManagmentEquipmentListForm.AMEId).ToList();
                    //Remove in DB
                    if (dbSoftwareAssetModal != null && dbSoftwareAssetModal.Count > 0)
                    {
                        //Log Maintain
                        if (dbLogModal != null)
                            dbLogModal.SoftwareAssetsList = JsonConvert.SerializeObject(dbSoftwareAssetModal);
                        foreach (var item in dbSoftwareAssetModal)
                        {
                            dbContext.AssetManagmentEquipmentSoftwareAssets.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }
                }
                if (Modal.AssetManagmentEquipmentSoftwareAssetsModel != null && Modal.AssetManagmentEquipmentSoftwareAssetsModel.Count > 0)
                {
                    //insert in DB
                    foreach (var item in Modal.AssetManagmentEquipmentSoftwareAssetsModel)
                    {
                        if (string.IsNullOrWhiteSpace(item.Name))
                            continue;
                        dbContext.AssetManagmentEquipmentSoftwareAssets.Add(new AssetManagmentEquipmentSoftwareAsset
                        {
                            SAId = Guid.NewGuid(),
                            AMEId = Modal.AssetManagmentEquipmentListForm.AMEId,
                            Name = item.Name,
                            Category = item.Category,
                            IsActive = item.IsActive,
                            LicenseType = item.LicenseType,
                            Manufacturer = item.Manufacturer,
                            SASoftwareId = item.SASoftwareId,
                            SAOwner = item.SAOwner,
                            SAPersonResponsible = item.SAPersonResponsible,
                            SACriticality = item.SACriticality,
                            SAOperatingSystem = item.SAOperatingSystem,
                            SAOSPatchVersion = item.SAOSPatchVersion
                        });
                    }
                    dbContext.SaveChanges();
                }

                if (dbLogModal != null)
                {
                    dbContext.AssetManagmentEquipmentLogs.Add(dbLogModal);
                    dbContext.SaveChanges();
                }
                LogHelper.writelog("SubmitAssetManagmentEquipmentListData : AssetManagmentEquipmentListForm save");

            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitAssetManagmentEquipmentListData : " + ex.Message + " : " + ex.InnerException);
            }
        }
        public AssetManagmentEquipmentListModal GetAssetManagmentEquipmentData(string ShipCode)
        {
            AssetManagmentEquipmentListModal dbResult = new AssetManagmentEquipmentListModal();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                AssetManagmentEquipmentList dbModal = new AssetManagmentEquipmentList();
                dbModal = dbContext.AssetManagmentEquipmentLists.Where(x => x.ShipCode == ShipCode).FirstOrDefault();
                if (dbModal == null)
                    dbModal = new AssetManagmentEquipmentList();
                dbResult.AssetManagmentEquipmentListForm = new AssetManagmentEquipmentListForm
                {
                    AMEId = dbModal.AMEId,
                    CreatedBy = dbModal.CreatedBy,
                    CreatedDate = dbModal.CreatedDate,
                    IsSynced = dbModal.IsSynced,
                    SavedAsDraft = dbModal.SavedAsDraft,
                    ShipCode = dbModal.ShipCode,
                    ShipName = dbModal.ShipName,
                    UpdatedBy = dbModal.UpdatedBy,
                    UpdatedDate = dbModal.UpdatedDate
                };
                var dbOTList = dbContext.AssetManagmentEquipmentOTLists.Where(x => x.AMEId == dbModal.AMEId).ToList();
                if (dbOTList == null)
                    dbOTList = new List<AssetManagmentEquipmentOTList>();
                var dbITList = dbContext.AssetManagmentEquipmentITLists.Where(x => x.AMEId == dbModal.AMEId).ToList();
                if (dbITList == null)
                    dbITList = new List<AssetManagmentEquipmentITList>();
                var dbSoftwareList = dbContext.AssetManagmentEquipmentSoftwareAssets.Where(x => x.AMEId == dbModal.AMEId).ToList();
                if (dbSoftwareList == null)
                    dbSoftwareList = new List<AssetManagmentEquipmentSoftwareAsset>();
                foreach (var item in dbOTList)
                {
                    dbResult.AssetManagmentEquipmentOTListModel.Add(new AssetManagmentEquipmentOTListModel
                    {
                        AMEId = item.AMEId,
                        OTEquipment = item.OTEquipment,
                        OTId = item.OTId,
                        OTLastServiced = item.OTLastServiced,
                        OTLocation = item.OTLocation,
                        OTMake = item.OTMake,
                        OTModel = item.OTModel,
                        OTRemark = item.OTRemark,
                        OTSerialNo = item.OTSerialNo,
                        OTType = item.OTType,
                        OTWorkingCondition = item.OTWorkingCondition,
                        OTCriticality = item.OTCriticality,
                        OTHardwareId = item.OTHardwareId,
                        OTOwner = item.OTOwner,
                        OTPersonResponsible = item.OTPersonResponsible,
                        OTOperatingSystem = item.OTOperatingSystem,
                        OTOSPatchVersion = item.OTOSPatchVersion
                    });
                }
                foreach (var item in dbITList)
                {
                    dbResult.AssetManagmentEquipmentITListModel.Add(new AssetManagmentEquipmentITListModel
                    {
                        AMEId = item.AMEId,
                        ITEquipment = item.ITEquipment,
                        ITId = item.ITId,
                        ITLastServiced = item.ITLastServiced,
                        ITLocation = item.ITLocation,
                        ITMake = item.ITMake,
                        ITModel = item.ITModel,
                        ITRemark = item.ITRemark,
                        ITSerialNo = item.ITSerialNo,
                        ITType = item.ITType,
                        ITWorkingCondition = item.ITWorkingCondition,
                        ITPersonResponsible = item.ITPersonResponsible,
                        ITCriticality = item.ITCriticality,
                        ITHardwareId = item.ITHardwareId,
                        ITOwner = item.ITOwner,
                        ITOperatingSystem = item.ITOperatingSystem,
                        ITOSPatchVersion = item.ITOSPatchVersion
                    });
                }
                foreach (var item in dbSoftwareList)
                {
                    dbResult.AssetManagmentEquipmentSoftwareAssetsModel.Add(new AssetManagmentEquipmentSoftwareAssetsModel
                    {
                        SAId = item.SAId,
                        AMEId = item.AMEId,
                        Name = item.Name,
                        Category = item.Category,
                        IsActive = item.IsActive,
                        LicenseType = item.LicenseType,
                        Manufacturer = item.Manufacturer,
                        SACriticality = item.SACriticality,
                        SAOwner = item.SAOwner,
                        SAPersonResponsible = item.SAPersonResponsible,
                        SASoftwareId = item.SASoftwareId,
                        SAOperatingSystem = item.SAOperatingSystem,
                        SAOSPatchVersion = item.SAOSPatchVersion
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentData Error : " + ex.Message);
            }
            return dbResult;
        }
        public AssetManagmentEquipmentListModal GetAssetManagmentEquipmentUnSyncData(string ShipCode)
        {
            AssetManagmentEquipmentListModal dbResult = new AssetManagmentEquipmentListModal();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                AssetManagmentEquipmentList dbModal = new AssetManagmentEquipmentList();
                dbModal = dbContext.AssetManagmentEquipmentLists.Where(x => x.ShipCode == ShipCode && x.IsSynced == false).FirstOrDefault();
                if (dbModal == null)
                    dbModal = new AssetManagmentEquipmentList();
                dbResult.AssetManagmentEquipmentListForm = new AssetManagmentEquipmentListForm
                {
                    AMEId = dbModal.AMEId,
                    CreatedBy = dbModal.CreatedBy,
                    CreatedDate = dbModal.CreatedDate,
                    IsSynced = dbModal.IsSynced,
                    SavedAsDraft = dbModal.SavedAsDraft,
                    ShipCode = dbModal.ShipCode,
                    ShipName = dbModal.ShipName,
                    UpdatedBy = dbModal.UpdatedBy,
                    UpdatedDate = dbModal.UpdatedDate
                };
                var dbOTList = dbContext.AssetManagmentEquipmentOTLists.Where(x => x.AMEId == dbModal.AMEId).ToList();
                if (dbOTList == null)
                    dbOTList = new List<AssetManagmentEquipmentOTList>();
                var dbITList = dbContext.AssetManagmentEquipmentITLists.Where(x => x.AMEId == dbModal.AMEId).ToList();
                if (dbITList == null)
                    dbITList = new List<AssetManagmentEquipmentITList>();
                var dbSoftwareList = dbContext.AssetManagmentEquipmentSoftwareAssets.Where(x => x.AMEId == dbModal.AMEId).ToList();
                if (dbSoftwareList == null)
                    dbSoftwareList = new List<AssetManagmentEquipmentSoftwareAsset>();
                foreach (var item in dbOTList)
                {
                    dbResult.AssetManagmentEquipmentOTListModel.Add(new AssetManagmentEquipmentOTListModel
                    {
                        AMEId = item.AMEId,
                        OTEquipment = item.OTEquipment,
                        OTId = item.OTId,
                        OTLastServiced = item.OTLastServiced,
                        OTLocation = item.OTLocation,
                        OTMake = item.OTMake,
                        OTModel = item.OTModel,
                        OTRemark = item.OTRemark,
                        OTSerialNo = item.OTSerialNo,
                        OTType = item.OTType,
                        OTWorkingCondition = item.OTWorkingCondition,
                        OTCriticality = item.OTCriticality,
                        OTHardwareId = item.OTHardwareId,
                        OTOwner = item.OTOwner,
                        OTPersonResponsible = item.OTPersonResponsible,
                        OTOperatingSystem = item.OTOperatingSystem,
                        OTOSPatchVersion = item.OTOSPatchVersion
                    });
                }
                foreach (var item in dbITList)
                {
                    dbResult.AssetManagmentEquipmentITListModel.Add(new AssetManagmentEquipmentITListModel
                    {
                        AMEId = item.AMEId,
                        ITEquipment = item.ITEquipment,
                        ITId = item.ITId,
                        ITLastServiced = item.ITLastServiced,
                        ITLocation = item.ITLocation,
                        ITMake = item.ITMake,
                        ITModel = item.ITModel,
                        ITRemark = item.ITRemark,
                        ITSerialNo = item.ITSerialNo,
                        ITType = item.ITType,
                        ITWorkingCondition = item.ITWorkingCondition,
                        ITPersonResponsible = item.ITPersonResponsible,
                        ITCriticality = item.ITCriticality,
                        ITHardwareId = item.ITHardwareId,
                        ITOwner = item.ITOwner,
                        ITOperatingSystem = item.ITOperatingSystem,
                        ITOSPatchVersion = item.ITOSPatchVersion
                    });
                }
                foreach (var item in dbSoftwareList)
                {
                    dbResult.AssetManagmentEquipmentSoftwareAssetsModel.Add(new AssetManagmentEquipmentSoftwareAssetsModel
                    {
                        SAId = item.SAId,
                        AMEId = item.AMEId,
                        Name = item.Name,
                        Category = item.Category,
                        IsActive = item.IsActive,
                        LicenseType = item.LicenseType,
                        Manufacturer = item.Manufacturer,
                        SACriticality = item.SACriticality,
                        SAOwner = item.SAOwner,
                        SAPersonResponsible = item.SAPersonResponsible,
                        SASoftwareId = item.SASoftwareId,
                        SAOperatingSystem = item.SAOperatingSystem,
                        SAOSPatchVersion = item.SAOSPatchVersion
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentUnSyncData Error : " + ex.Message);
            }
            return dbResult;
        }

        public void UpdateAssetManagmentEquipmentSyncStatus(string ShipCode, bool IsSynced)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                AssetManagmentEquipmentList dbModal = new AssetManagmentEquipmentList();
                dbModal = dbContext.AssetManagmentEquipmentLists.Where(x => x.ShipCode == ShipCode).FirstOrDefault();
                if (dbModal != null)
                {
                    dbModal.IsSynced = IsSynced;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception)
            { }
        }
        public List<string> GetAssetManagmentHardwareId(string ShipCode)
        {
            List<string> dbResult = new List<string>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                AssetManagmentEquipmentList dbModal = new AssetManagmentEquipmentList();
                dbModal = dbContext.AssetManagmentEquipmentLists.Where(x => x.ShipCode == ShipCode).FirstOrDefault();
                if (dbModal == null)
                    dbModal = new AssetManagmentEquipmentList();
                DataTable dt = new DataTable();
                string Query = @"Select OTHardwareId + '|' + OTEquipment as HardwareId from AssetManagmentEquipmentOTList Where AMEId='" + dbModal.AMEId + "' AND OTHardwareId IS NOT NULL ";
                Query += " Union Select ITHardwareId + '|' + ITEquipment as HardwareId from AssetManagmentEquipmentITList Where AMEId= '" + dbModal.AMEId + "' AND ITHardwareId IS NOT NULL ";
                Query += " Union SElect SASoftwareId + '|' + Name as HardwareId from AssetManagmentEquipmentSoftwareAssets Where AMEId= '" + dbModal.AMEId + "' AND SASoftwareId IS NOT NULL ";

                using (var conn = new SqlConnection(connetionString))
                {
                    SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                    sqlAdp.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                        dbResult = (dt.AsEnumerable()
                           .Select(r => r.Field<string>("HardwareId"))
                           .OrderBy(x => x).ToList());
                }
                //var dbOTList = dbContext.AssetManagmentEquipmentOTLists.Where(x => x.AMEId == dbModal.AMEId).ToList();
                //if (dbOTList == null)
                //    dbOTList = new List<AssetManagmentEquipmentOTList>();
                //var dbITList = dbContext.AssetManagmentEquipmentITLists.Where(x => x.AMEId == dbModal.AMEId).ToList();
                //if (dbITList == null)
                //    dbITList = new List<AssetManagmentEquipmentITList>();
                //var dbSWList = dbContext.AssetManagmentEquipmentSoftwareAssets.Where(x => x.AMEId == dbModal.AMEId).ToList();
                //if (dbSWList == null)
                //    dbSWList = new List<AssetManagmentEquipmentSoftwareAsset>();
                //dbResult.AddRange(dbOTList.Select(x => x.OTHardwareId + "|" + x.OTEquipment).ToList());
                //dbResult.AddRange(dbITList.Select(x => x.ITHardwareId + "|" + x.ITEquipment).ToList());
                //dbResult.AddRange(dbSWList.Select(x => x.SASoftwareId + "|" + x.Name).ToList());
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentHardwareId Error : " + ex.Message);
            }
            return dbResult;
        }
        #endregion
        #region CybersecurityRisksAssessment
        public void SubmitCybersecurityRisksAssessment(CybersecurityRisksAssessmentModal Modal)
        {
            try
            {
                //bool isNeedToAddLog = false;
                //if (!Modal.IsFromOfficeApp)
                //    isNeedToAddLog = true;

                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                CybersecurityRisksAssessment dbModal = new CybersecurityRisksAssessment();
                CybersecurityRisksAssessmentLog dbLogModal = null;
                if (Modal != null && !string.IsNullOrWhiteSpace(Modal.CybersecurityRisksAssessmentForm.ShipCode))
                {
                    dbModal = dbContext.CybersecurityRisksAssessments.Where(x => x.ShipCode == Modal.CybersecurityRisksAssessmentForm.ShipCode).FirstOrDefault();
                    if (dbModal != null)
                    {
                        //if (isNeedToAddLog)
                        //{
                        dbLogModal = new CybersecurityRisksAssessmentLog
                        {
                            CRAId = dbModal.CRAId,
                            CreatedBy = dbModal.CreatedBy,
                            CreatedDate = dbModal.CreatedDate,
                            LogDate = Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            ShipCode = dbModal.ShipCode,
                            ShipName = dbModal.ShipName,
                            UpdatedBy = dbModal.UpdatedBy,
                            UpdatedDate = dbModal.UpdatedDate,
                            Id = Guid.NewGuid()
                        };
                        //}

                        Modal.CybersecurityRisksAssessmentForm.CRAId = dbModal.CRAId;
                        dbModal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        dbModal.ShipName = Modal.CybersecurityRisksAssessmentForm.ShipName;
                        dbModal.ShipCode = Modal.CybersecurityRisksAssessmentForm.ShipCode;
                        dbModal.UpdatedBy = string.IsNullOrWhiteSpace(Modal.CybersecurityRisksAssessmentForm.UpdatedBy) ? Modal.CybersecurityRisksAssessmentForm.CreatedBy : Modal.CybersecurityRisksAssessmentForm.UpdatedBy;
                        dbModal.UpdatedDate = Modal.CybersecurityRisksAssessmentForm.UpdatedDate.HasValue ? Modal.CybersecurityRisksAssessmentForm.UpdatedDate : Modal.CybersecurityRisksAssessmentForm.CreatedDate;
                        dbModal.IsSynced = Modal.CybersecurityRisksAssessmentForm.IsSynced;
                    }
                    else
                    {
                        if (Modal.CybersecurityRisksAssessmentForm.CRAId == Guid.Empty)
                            Modal.CybersecurityRisksAssessmentForm.CRAId = Guid.NewGuid();
                        dbModal = new CybersecurityRisksAssessment();
                        dbModal.CRAId = Modal.CybersecurityRisksAssessmentForm.CRAId;
                        dbModal.CreatedBy = Modal.CybersecurityRisksAssessmentForm.CreatedBy;
                        dbModal.CreatedDate = Modal.CybersecurityRisksAssessmentForm.CreatedDate;
                        dbModal.IsSynced = Modal.CybersecurityRisksAssessmentForm.IsSynced;
                        dbModal.ShipCode = Modal.CybersecurityRisksAssessmentForm.ShipCode;
                        dbModal.ShipName = Modal.CybersecurityRisksAssessmentForm.ShipName;
                        dbModal.UpdatedBy = Modal.CybersecurityRisksAssessmentForm.UpdatedBy;
                        dbModal.UpdatedDate = Modal.CybersecurityRisksAssessmentForm.UpdatedDate;
                        dbContext.CybersecurityRisksAssessments.Add(dbModal);
                    }
                }
                else
                {
                    if (Modal.CybersecurityRisksAssessmentForm.CRAId == Guid.Empty)
                        Modal.CybersecurityRisksAssessmentForm.CRAId = Guid.NewGuid();
                    dbModal = new CybersecurityRisksAssessment();
                    dbModal.CRAId = Modal.CybersecurityRisksAssessmentForm.CRAId;
                    dbModal.CreatedBy = Modal.CybersecurityRisksAssessmentForm.CreatedBy;
                    dbModal.CreatedDate = Modal.CybersecurityRisksAssessmentForm.CreatedDate;
                    dbModal.IsSynced = Modal.CybersecurityRisksAssessmentForm.IsSynced;
                    dbModal.ShipCode = Modal.CybersecurityRisksAssessmentForm.ShipCode;
                    dbModal.ShipName = Modal.CybersecurityRisksAssessmentForm.ShipName;
                    dbModal.UpdatedBy = Modal.CybersecurityRisksAssessmentForm.UpdatedBy;
                    dbModal.UpdatedDate = Modal.CybersecurityRisksAssessmentForm.UpdatedDate;
                    dbContext.CybersecurityRisksAssessments.Add(dbModal);
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (OptimisticConcurrencyException e)
                {
                    LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- OptimisticConcurrencyException, Error:", e.Message));
                    var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                    objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessments);
                    objContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- DbUpdateConcurrencyException, Error:", e.Message));
                    var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                    objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessments);
                    objContext.SaveChanges();
                }
                if (Modal.CybersecurityRisksAssessmentListModal != null && Modal.CybersecurityRisksAssessmentListModal.Count > 0)
                {
                    CybersecurityRisksAssessmentList objRecord = null;
                    if (Modal != null && Modal.CybersecurityRisksAssessmentForm.CRAId != Guid.Empty)
                    {
                        List<CybersecurityRisksAssessmentList> dbRiskList = dbContext.CybersecurityRisksAssessmentLists.Where(x => x.CRAId == Modal.CybersecurityRisksAssessmentForm.CRAId).ToList();
                        //Remove in DB
                        if (dbRiskList != null && dbRiskList.Count > 0)
                        {
                            //Log Maintain
                            if (dbLogModal != null)
                                dbLogModal.CybersecurityRisksAssessmentList = JsonConvert.SerializeObject(dbRiskList);
                            if (Modal.CybersecurityRisksAssessmentForm.RemovedCRALId != Guid.Empty)
                            {
                                objRecord = dbContext.CybersecurityRisksAssessmentLists.Where(x => x.CRAId == Modal.CybersecurityRisksAssessmentForm.CRAId && x.CRALId == Modal.CybersecurityRisksAssessmentForm.RemovedCRALId).FirstOrDefault();
                                if (objRecord != null)
                                {
                                    dbContext.CybersecurityRisksAssessmentLists.Remove(objRecord);
                                    try
                                    {
                                        dbContext.SaveChanges();
                                    }
                                    catch (OptimisticConcurrencyException e)
                                    {
                                        LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- OptimisticConcurrencyException, Error:", e.Message));
                                        var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                                        objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessmentLists);
                                        objContext.SaveChanges();
                                    }
                                    catch (DbUpdateConcurrencyException e)
                                    {
                                        LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- DbUpdateConcurrencyException, Error:", e.Message));
                                        var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                                        objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessmentLists);
                                        objContext.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    //insert in DB
                    bool isNewItem = false, isNeedToSaveChanges = false;

                    foreach (var item in Modal.CybersecurityRisksAssessmentListModal)
                    {
                        isNewItem = false;
                        //if (string.IsNullOrWhiteSpace(item.RiskId) && string.IsNullOrWhiteSpace(item.HardwareId))
                        //    continue;
                        try
                        {
                            if (item.CRALId == Guid.Empty)
                                isNewItem = true;
                            else
                            {
                                objRecord = dbContext.CybersecurityRisksAssessmentLists.Where(x => x.CRAId == Modal.CybersecurityRisksAssessmentForm.CRAId && x.CRALId == item.CRALId).FirstOrDefault();
                                if (objRecord != null)
                                {
                                    if (item.IsUpdated)
                                    {
                                        objRecord.Controls = item.Controls;
                                        objRecord.HardwareId = item.HardwareId;
                                        objRecord.InherentImpactScore = item.InherentImpactScore;
                                        objRecord.InherentLikelihoodScore = item.InherentLikelihoodScore;
                                        objRecord.InherentRiskCategoryA = item.InherentRiskCategoryA;
                                        objRecord.InherentRiskCategoryC = item.InherentRiskCategoryC;
                                        objRecord.InherentRiskCategoryI = item.InherentRiskCategoryI;
                                        objRecord.InherentRiskCategoryS = item.InherentRiskCategoryS;
                                        objRecord.InherentRiskScore = item.InherentRiskScore;
                                        objRecord.ResidualImpactScore = item.ResidualImpactScore;
                                        objRecord.ResidualLikelihoodScore = item.ResidualLikelihoodScore;
                                        objRecord.ResidualRiskCategoryA = item.ResidualRiskCategoryA;
                                        objRecord.ResidualRiskCategoryC = item.ResidualRiskCategoryC;
                                        objRecord.ResidualRiskCategoryI = item.ResidualRiskCategoryI;
                                        objRecord.ResidualRiskCategoryS = item.ResidualRiskCategoryS;
                                        objRecord.ResidualRiskScore = item.ResidualRiskScore;
                                        objRecord.RiskDecision = item.RiskDecision;
                                        objRecord.RiskDescription = item.RiskDescription;
                                        objRecord.RiskId = item.RiskId;
                                        objRecord.RiskOwner = item.RiskOwner;
                                        objRecord.Vulnerability = item.Vulnerability;
                                        try
                                        {
                                            dbContext.SaveChanges();
                                        }
                                        catch (OptimisticConcurrencyException e)
                                        {
                                            LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- OptimisticConcurrencyException, Error:", e.Message));
                                            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                                            objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessments);
                                            objContext.SaveChanges();
                                        }
                                        catch (DbUpdateConcurrencyException e)
                                        {
                                            LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- DbUpdateConcurrencyException, Error:", e.Message));
                                            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                                            objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessments);
                                            objContext.SaveChanges();
                                        }
                                    }
                                }
                                else
                                    isNewItem = true;

                            }
                            if (isNewItem)
                            {
                                isNeedToSaveChanges = true;
                                dbContext.CybersecurityRisksAssessmentLists.Add(new CybersecurityRisksAssessmentList
                                {
                                    CRALId = item.CRALId == Guid.Empty ? Guid.NewGuid() : item.CRALId,
                                    CRAId = Modal.CybersecurityRisksAssessmentForm.CRAId,
                                    Controls = item.Controls,
                                    HardwareId = item.HardwareId,
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
                            else
                            {

                            }
                            //dbContext.SaveChanges();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment : Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State));
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage));
                                }
                            }
                        }
                    }
                    if (isNeedToSaveChanges)
                    {
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch (OptimisticConcurrencyException e)
                        {
                            LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- OptimisticConcurrencyException, Error:", e.Message));
                            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                            objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessments);
                            objContext.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- DbUpdateConcurrencyException, Error:", e.Message));
                            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                            objContext.Refresh(RefreshMode.ClientWins, dbContext.CybersecurityRisksAssessments);
                            objContext.SaveChanges();
                        }
                    }
                }
                if (dbLogModal != null)
                {
                    WriteCybersecurityRisksAssessmentLogs("Existing Record : " + JsonConvert.SerializeObject(dbLogModal), dbLogModal.ShipCode + "_" + dbLogModal.ShipName);
                    WriteCybersecurityRisksAssessmentLogs("New Record Request : " + JsonConvert.SerializeObject(Modal), dbLogModal.ShipCode + "_" + dbLogModal.ShipName);
                    //Write Log in ShipWise File
                    //try
                    //{
                    //    dbContext.CybersecurityRisksAssessmentLogs.Add(dbLogModal);
                    //    dbContext.SaveChanges();
                    //}
                    //catch (Exception)
                    //{ }
                }
                LogHelper.writelog("SubmitCybersecurityRisksAssessment : CybersecurityRisksAssessment save");

            }
            catch (OptimisticConcurrencyException ex)
            {
                LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment : OptimisticConcurrencyException errors:", ex.Message)); ;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment : Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        LogHelper.writelog(string.Format("SubmitCybersecurityRisksAssessment :- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitCybersecurityRisksAssessment : " + ex.Message + " : " + ex.InnerException);
            }
        }
        public CybersecurityRisksAssessmentModal GetCybersecurityRisksAssessmentData(string ShipCode)
        {
            CybersecurityRisksAssessmentModal dbResult = new CybersecurityRisksAssessmentModal();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                AssetManagmentEquipmentList dbModal = new AssetManagmentEquipmentList();
                dbModal = dbContext.AssetManagmentEquipmentLists.Where(x => x.ShipCode == ShipCode).FirstOrDefault();
                if (dbModal == null)
                    dbModal = new AssetManagmentEquipmentList();

                using (var conn = new SqlConnection(connetionString))
                {
                    DataSet ds = new DataSet();
                    string Query = @"Select * from CybersecurityRisksAssessment Where ShipCode='" + ShipCode + "' ; ";
                    Query += @"Select CL.*,ISNULL(Cast(InherentImpactScore as int) * Cast(InherentLikelihoodScore as int),0) as ImpactScore, ISNULL(Cast(ResidualImpactScore as int) * Cast(ResidualLikelihoodScore as int),0) as ReseduleScore from CybersecurityRisksAssessmentList CL
                            Inner Join CybersecurityRisksAssessment C ON C.CRAID = CL.CRAID
                            Where ShipCode='" + ShipCode + "' Order By CL.ResidualRiskScore,CL.RiskId; ";
                    Query += @"Select OTHardwareId + '|' + OTEquipment as HardwareId from AssetManagmentEquipmentOTList Where AMEId='" + dbModal.AMEId + "'";
                    Query += " Union All Select ITHardwareId + '|' + ITEquipment as HardwareId from AssetManagmentEquipmentITList Where AMEId= '" + dbModal.AMEId + "'";
                    Query += " Union All SElect SASoftwareId + '|' + Name as HardwareId from AssetManagmentEquipmentSoftwareAssets Where AMEId= '" + dbModal.AMEId + "'";
                    SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                    sqlAdp.Fill(ds);
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                            dbResult.CybersecurityRisksAssessmentForm = dt.ToListof<CybersecurityRisksAssessmentForm>().FirstOrDefault();
                        dt = ds.Tables[1];
                        if (dt != null && dt.Rows.Count > 0)
                            dbResult.CybersecurityRisksAssessmentListModal = dt.ToListof<CybersecurityRisksAssessmentListModal>();
                        dt = ds.Tables[2];
                        if (dt != null && dt.Rows.Count > 0)
                            dbResult.HardwareListBoxValues = (dt.AsEnumerable()
                           .Select(r => r.Field<string>("HardwareId"))
                           .OrderBy(x => x).ToList());
                        dbResult.CybersecurityRisksAssessmentListModal = dbResult.CybersecurityRisksAssessmentListModal.OrderByDescending(x => x.RiskId == null ? 0 : 1).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCybersecurityRisksAssessmentData Error : " + ex.Message);
            }
            return dbResult;
        }
        public CybersecurityRisksAssessmentModal GetCybersecurityRisksAssessmentUnSyncData(string ShipCode)
        {
            CybersecurityRisksAssessmentModal dbResult = new CybersecurityRisksAssessmentModal();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                CybersecurityRisksAssessment dbModal = new CybersecurityRisksAssessment();
                dbModal = dbContext.CybersecurityRisksAssessments.Where(x => x.ShipCode == ShipCode && x.IsSynced == false).FirstOrDefault();
                if (dbModal == null)
                    dbModal = new CybersecurityRisksAssessment();
                dbResult.CybersecurityRisksAssessmentForm = new CybersecurityRisksAssessmentForm
                {
                    CRAId = dbModal.CRAId,
                    CreatedBy = dbModal.CreatedBy,
                    CreatedDate = dbModal.CreatedDate,
                    IsSynced = dbModal.IsSynced,
                    ShipCode = dbModal.ShipCode,
                    ShipName = dbModal.ShipName,
                    UpdatedBy = dbModal.UpdatedBy,
                    UpdatedDate = dbModal.UpdatedDate
                };
                var dbRiskList = dbContext.CybersecurityRisksAssessmentLists.Where(x => x.CRAId == dbModal.CRAId).ToList();
                if (dbRiskList == null)
                    dbRiskList = new List<CybersecurityRisksAssessmentList>();
                foreach (var item in dbRiskList)
                {
                    dbResult.CybersecurityRisksAssessmentListModal.Add(new CybersecurityRisksAssessmentListModal
                    {
                        CRAId = item.CRAId,
                        CRALId = item.CRALId,
                        Controls = item.Controls,
                        HardwareId = item.HardwareId,
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
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCybersecurityRisksAssessmentUnSyncData Error : " + ex.Message);
            }
            return dbResult;
        }
        public void UpdateCybersecurityRisksAssessmentSyncStatus(string ShipCode, bool IsSynced)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                CybersecurityRisksAssessment dbModal = new CybersecurityRisksAssessment();
                dbModal = dbContext.CybersecurityRisksAssessments.Where(x => x.ShipCode == ShipCode).FirstOrDefault();
                if (dbModal != null)
                {
                    dbModal.IsSynced = IsSynced;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception)
            { }
        }

        public List<string> GetCyberSecurityRiskList()
        {
            List<string> riskList = new List<string>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var dbRiskList = dbContext.CyberSecuritySettings.Where(x => x.Type == "RISKDESC").ToList();
                foreach (var item in dbRiskList)
                {
                    riskList.Add(item.Name);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecurityRiskList Error : " + ex.Message);
            }
            return riskList;
        }

        public List<string> GetCyberSecurityVulnerabilitiesList()
        {
            List<string> vulnerabilitiesList = new List<string>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var dbVulnerabilitiesList = dbContext.CyberSecuritySettings.Where(x => x.Type == "VULN").ToList();
                foreach (var item in dbVulnerabilitiesList)
                {
                    vulnerabilitiesList.Add(item.Name);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecurityVulnerabilitiesList Error : " + ex.Message);
            }
            return vulnerabilitiesList;
        }
        public List<CyberSecuritySettingsModal> GetCyberSecuritySettingsListByType(string type)
        {
            List<CyberSecuritySettingsModal> vulnerabilitiesList = new List<CyberSecuritySettingsModal>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var dbVulnerabilitiesList = dbContext.CyberSecuritySettings.Where(x => x.Type == type).ToList();
                if (dbVulnerabilitiesList != null)
                {
                    foreach (var item in dbVulnerabilitiesList)
                    {
                        vulnerabilitiesList.Add(new CyberSecuritySettingsModal
                        {
                            Name = item.Name,
                            Type = item.Type
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecuritySettingsListByType Error : " + ex.Message);
            }
            return vulnerabilitiesList;
        }
        public List<CyberSecuritySettingsModal> GetAllCyberSecuritySettingsList()
        {
            List<CyberSecuritySettingsModal> vulnerabilitiesList = new List<CyberSecuritySettingsModal>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var dbVulnerabilitiesList = dbContext.CyberSecuritySettings.ToList();
                if (dbVulnerabilitiesList != null)
                {
                    foreach (var item in dbVulnerabilitiesList)
                    {
                        vulnerabilitiesList.Add(new CyberSecuritySettingsModal
                        {
                            Name = item.Name,
                            Type = item.Type
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllCyberSecuritySettingsList Error : " + ex.Message);
            }
            return vulnerabilitiesList;
        }

        public void CopyCybersecurityRisksAssessment(CyberSecurityCopyDataModal Modal)
        {

            using (var conn = new SqlConnection(connetionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CopyRiskAssessment", conn))
                {
                    conn.Open();
                    cmd.Parameters.Add("@SourceShipCode", SqlDbType.NVarChar).Value = Modal.SourceShipCode;
                    cmd.Parameters.Add("@DestinationShipCode", SqlDbType.NVarChar).Value = Modal.DestinationShipCode;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public static void WriteCybersecurityRisksAssessmentLogs(string content, string shipName)
        {
            try
            {
                //string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + shipName + "\\" + DateTime.Now.ToString("ddMMMyyyy") + ".log";
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + shipName + "\\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log"; //RDBJ 10/27/2021 set UtcTime
                Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
                FileStream fs = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine("Log At " + Utility.ToDateTimeUtcNow()); //RDBJ 10/27/2021 set UtcTime);
                sw.WriteLine(content);
                sw.WriteLine("=======================================================================================================");
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        #endregion
    }
}
