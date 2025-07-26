using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class FormsHelper
    {
        string OfficeAPPUrl = System.Configuration.ConfigurationManager.AppSettings["OfficeAPPUrl"];
        public void StartFormsync()
        {
            APIHelper _helper = new APIHelper();
            List<SyncedForm> SyncedFormList = new List<SyncedForm>();
            List<FormModal> APIFormsList = _helper.GetAllFormsForService();
            List<FormModal> LocalFormsList = GetLocalForms();

            if (APIFormsList != null && APIFormsList.Count > 0)
            {
                foreach (var ApiDoc in APIFormsList)
                {
                    FormModal LocalDoc = LocalFormsList.Where(x => x.FormID == ApiDoc.FormID).FirstOrDefault();
                    if (LocalDoc != null)
                    {
                        // Exist in local machine - check for update
                        UpdateFormProcess(ApiDoc, LocalDoc, ref SyncedFormList);
                    }
                    else
                    {
                        // Not Exist in local machine - Insert New Form
                        InsertFormProcess(ApiDoc, ref SyncedFormList);
                    }
                }
            }
            PrintSyncList(SyncedFormList);
        }
        public void InsertFormProcess(FormModal ApiDoc, ref List<SyncedForm> SyncedFormList)
        {
            if (ApiDoc.IsDeleted == false)
            {
                bool isInsertedinLocalDB = InsertForm(ApiDoc);
                if (isInsertedinLocalDB == true)
                {
                    bool isCreatedOnLocal = CreatePhysicalLocation(ApiDoc);
                    if (isInsertedinLocalDB == true && isCreatedOnLocal == true)
                    {
                        AddSyncList(ApiDoc, ref SyncedFormList, AppStatic.INSERTED);
                    }
                }
            }
        }
        public void UpdateFormProcess(FormModal ApiDoc, FormModal LocalDoc, ref List<SyncedForm> SyncedFormList)
        {
            try
            {
                if (LocalDoc.Version != ApiDoc.Version)
                {
                    LocalDoc.Title = ApiDoc.Title;
                    LocalDoc.Version = ApiDoc.Version;
                    LocalDoc.Type = ApiDoc.Type;
                    LocalDoc.Code = ApiDoc.Code;
                    LocalDoc.AccessLevel = ApiDoc.AccessLevel;
                    LocalDoc.AllowsNetworkAccess = ApiDoc.AllowsNetworkAccess;
                    LocalDoc.Amendment = ApiDoc.Amendment;
                    LocalDoc.AmendmentDate = ApiDoc.AmendmentDate;
                    LocalDoc.CanBeOpened = ApiDoc.CanBeOpened;
                    LocalDoc.Category = ApiDoc.Category;
                    LocalDoc.Department = ApiDoc.Department;
                    LocalDoc.HasSavedData = ApiDoc.HasSavedData;
                    LocalDoc.Issue = ApiDoc.Issue;
                    LocalDoc.IssueDate = ApiDoc.IssueDate;
                    LocalDoc.IsURNBased = ApiDoc.IsURNBased;
                    LocalDoc.URN = ApiDoc.URN;
                    LocalDoc.FolderType = ApiDoc.FolderType;
                    bool isUpdatedDoc = UpdateForm(LocalDoc);
                    if (isUpdatedDoc)
                    {
                        AddSyncList(ApiDoc, ref SyncedFormList, AppStatic.UPDATED);
                    }
                }
                var isNeedToDownload = false;
                if (!string.IsNullOrWhiteSpace(LocalDoc.TemplatePath))
                {
                    if (ApiDoc.Type == "FOLDER")
                    {
                        if (!Directory.Exists(LocalDoc.TemplatePath))
                            isNeedToDownload = true;
                    }
                    else
                    {
                        if (!System.IO.File.Exists(LocalDoc.TemplatePath))
                            isNeedToDownload = true;
                    }
                }
                if (LocalDoc.DocumentVersion != ApiDoc.DocumentVersion || isNeedToDownload == true)
                {
                    string oldFilePath = LocalDoc.TemplatePath;
                    string oldFileDownloadPath = LocalDoc.DownloadPath;
                    bool res = DownloadUpdatedFormDoc(LocalDoc, ApiDoc);
                    if (res)
                    {
                        LocalDoc.TemplatePath = ApiDoc.TemplatePath;
                        LocalDoc.DownloadPath = ApiDoc.DownloadPath;
                        LocalDoc.UploadType = ApiDoc.UploadType;
                        LocalDoc.DocumentVersion = ApiDoc.DocumentVersion;
                        bool isUpdatedDoc = UpdateFormFile(LocalDoc);
                        if (res == true && isUpdatedDoc == true)
                        {
                            //DeleteOldFile(oldFilePath, oldFileDownloadPath);
                            AddSyncList(ApiDoc, ref SyncedFormList, AppStatic.UPDATED);
                        }
                    }
                }
                if (LocalDoc.IsDeleted != ApiDoc.IsDeleted)
                    LocalDoc.IsDeleted = ApiDoc.IsDeleted;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFormProcess : " + ex.Message);
            }

        }
        public List<FormModal> GetLocalForms()
        {
            List<FormModal> LocalFormsList = new List<FormModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.Forms, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            LocalFormsList = dt.ToListof<FormModal>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLocalForms : " + ex.Message);
            }
            return LocalFormsList;
        }
        public bool InsertForm(FormModal modal)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "INSERT INTO " + AppStatic.Forms +
                              " ([FormID],[TemplatePath],[Code],[Title],[Type],[Issue],[IssueDate],[Amendment],[AmendmentDate],[Department],[Category],[AccessLevel],[AllowsNetworkAccess],[CanBeOpened],[HasSavedData],[IsURNBased],[URN],[IsDeleted],[DownloadPath],[UploadType],[CreatedDate],[UpdatedDate],[DocumentVersion],[Version],[FolderType]) OUTPUT INSERTED.ID " +
                                "VALUES(@FormID,@TemplatePath,@Code,@Title,@Type,@Issue,@IssueDate,@Amendment,@AmendmentDate,@Department,@Category,@AccessLevel,@AllowsNetworkAccess,@CanBeOpened,@HasSavedData,@IsURNBased,@URN,@IsDeleted,@DownloadPath,@UploadType,@CreatedDate,@UpdatedDate,@DocumentVersion,@Version,@FolderType)";

                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@FormID", SqlDbType.UniqueIdentifier).Value = modal.FormID;
                        command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = modal.Code == null ? string.Empty : modal.Code;
                        command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = modal.Title == null ? string.Empty : modal.Title;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = modal.Type == null ? string.Empty : modal.Type;
                        command.Parameters.Add("@TemplatePath", SqlDbType.NVarChar).Value = modal.TemplatePath == null ? string.Empty : modal.TemplatePath;
                        command.Parameters.Add("@DownloadPath", SqlDbType.NVarChar).Value = modal.DownloadPath == null ? string.Empty : modal.DownloadPath;
                        command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = modal.IsDeleted;
                        command.Parameters.Add("@UploadType", SqlDbType.NVarChar).Value = modal.UploadType == null ? string.Empty : modal.UploadType;
                        command.Parameters.Add("@Version", SqlDbType.Float).Value = modal.Version == null ? 1.0 : modal.Version;
                        command.Parameters.Add("@DocumentVersion", SqlDbType.Float).Value = modal.DocumentVersion == null ? 1.0 : modal.DocumentVersion;
                        command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@Issue", SqlDbType.Int).Value = modal.Issue ?? (object)DBNull.Value;
                        command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = modal.IssueDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@Amendment", SqlDbType.Int).Value = modal.Amendment ?? (object)DBNull.Value;
                        command.Parameters.Add("@AmendmentDate", SqlDbType.DateTime).Value = modal.AmendmentDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@Department", SqlDbType.NVarChar).Value = modal.Department ?? (object)DBNull.Value;
                        command.Parameters.Add("@Category", SqlDbType.NVarChar).Value = modal.Category ?? (object)DBNull.Value;
                        command.Parameters.Add("@AccessLevel", SqlDbType.NVarChar).Value = modal.AccessLevel ?? (object)DBNull.Value;
                        command.Parameters.Add("@AllowsNetworkAccess", SqlDbType.Bit).Value = modal.AllowsNetworkAccess ?? (object)DBNull.Value;
                        command.Parameters.Add("@CanBeOpened", SqlDbType.Bit).Value = modal.CanBeOpened ?? (object)DBNull.Value;
                        command.Parameters.Add("@HasSavedData", SqlDbType.Bit).Value = modal.HasSavedData ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsURNBased", SqlDbType.Bit).Value = modal.IsURNBased ?? (object)DBNull.Value;
                        command.Parameters.Add("@URN", SqlDbType.NVarChar).Value = modal.URN ?? (object)DBNull.Value;
                        command.Parameters.Add("@FolderType", SqlDbType.NVarChar).Value = modal.FolderType ?? (object)DBNull.Value;

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
                LogHelper.writelog("InsertForm : " + ex.Message);
                res = false;
            }
            return res;
        }
        public bool UpdateForm(FormModal modal)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "UPDATE " + AppStatic.Forms +
                              " SET [FormID] = @FormID,[TemplatePath] = @TemplatePath,[Code] = @Code,[Title] = @Title,[Type] = @Type,[Issue] = @Issue,[IssueDate] = @IssueDate,[Amendment] = @Amendment,[AmendmentDate] = @AmendmentDate,[Department] = @Department,[Category] = @Category,[AccessLevel] = @AccessLevel,[AllowsNetworkAccess] = @AllowsNetworkAccess,[CanBeOpened] = @CanBeOpened,[HasSavedData] = @HasSavedData,[IsURNBased] = @IsURNBased,[URN] = @URN,[IsDeleted] = @IsDeleted,[DownloadPath] = @DownloadPath,[UploadType] = @UploadType,[CreatedDate] = @CreatedDate,[UpdatedDate] = @UpdatedDate,[DocumentVersion] = @DocumentVersion,[Version] = @Version,[FolderType] = @FolderType WHERE ID = @ID";
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@FormID", SqlDbType.UniqueIdentifier).Value = modal.FormID;
                        command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = modal.Code == null ? string.Empty : modal.Code;
                        command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = modal.Title == null ? string.Empty : modal.Title;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = modal.Type == null ? string.Empty : modal.Type;
                        command.Parameters.Add("@TemplatePath", SqlDbType.NVarChar).Value = modal.TemplatePath == null ? string.Empty : modal.TemplatePath;
                        command.Parameters.Add("@DownloadPath", SqlDbType.NVarChar).Value = modal.DownloadPath == null ? string.Empty : modal.DownloadPath;
                        command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = modal.IsDeleted ?? (object)DBNull.Value;
                        command.Parameters.Add("@UploadType", SqlDbType.NVarChar).Value = modal.UploadType == null ? string.Empty : modal.UploadType;
                        command.Parameters.Add("@Version", SqlDbType.Float).Value = modal.Version == null ? 1.0 : modal.Version;
                        command.Parameters.Add("@DocumentVersion", SqlDbType.Float).Value = modal.DocumentVersion == null ? 1.0 : modal.DocumentVersion;
                        command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@Issue", SqlDbType.Int).Value = modal.Issue ?? (object)DBNull.Value;
                        command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = modal.IssueDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@Amendment", SqlDbType.Int).Value = modal.Amendment ?? (object)DBNull.Value;
                        command.Parameters.Add("@AmendmentDate", SqlDbType.DateTime).Value = modal.AmendmentDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@Department", SqlDbType.NVarChar).Value = modal.Department ?? (object)DBNull.Value;
                        command.Parameters.Add("@Category", SqlDbType.NVarChar).Value = modal.Category ?? (object)DBNull.Value;
                        command.Parameters.Add("@AccessLevel", SqlDbType.NVarChar).Value = modal.AccessLevel ?? (object)DBNull.Value;
                        command.Parameters.Add("@AllowsNetworkAccess", SqlDbType.Bit).Value = modal.AllowsNetworkAccess ?? (object)DBNull.Value;
                        command.Parameters.Add("@CanBeOpened", SqlDbType.Bit).Value = modal.CanBeOpened ?? (object)DBNull.Value;
                        command.Parameters.Add("@HasSavedData", SqlDbType.Bit).Value = modal.HasSavedData ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsURNBased", SqlDbType.Bit).Value = modal.IsURNBased ?? (object)DBNull.Value;
                        command.Parameters.Add("@URN", SqlDbType.NVarChar).Value = modal.URN ?? (object)DBNull.Value;
                        command.Parameters.Add("@ID", SqlDbType.Int).Value = modal.ID;
                        command.Parameters.Add("@FolderType", SqlDbType.NVarChar).Value = modal.FolderType ?? (object)DBNull.Value;
                        command.ExecuteNonQuery();
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                res = false;
                LogHelper.writelog("UpdateForm : " + ex.Message);
            }
            return res;
        }
        public bool UpdateFormFile(FormModal LocalDoc)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "UPDATE " + AppStatic.Forms +
                              " SET [TemplatePath] = @TemplatePath, [DownloadPath] = @DownloadPath, [DocumentVersion] = @DocumentVersion, [UpdatedDate] = @UpdatedDate WHERE [ID] = @ID";
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@TemplatePath", SqlDbType.NVarChar).Value = LocalDoc.TemplatePath ?? (object)DBNull.Value;
                        command.Parameters.Add("@DownloadPath", SqlDbType.NVarChar).Value = LocalDoc.DownloadPath ?? (object)DBNull.Value;
                        command.Parameters.Add("@DocumentVersion", SqlDbType.Float).Value = LocalDoc.DocumentVersion ?? (object)DBNull.Value;
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        command.Parameters.Add("@ID", SqlDbType.Int).Value = LocalDoc.ID;
                        command.ExecuteNonQuery();
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                res = false;
                LogHelper.writelog("UpdateFormFile : " + ex.Message);
            }
            return res;
        }
        public bool CreatePhysicalLocation(FormModal modal)
        {
            bool res = false;
            try
            {
                if (string.IsNullOrEmpty(modal.DownloadPath))
                {
                    modal.DownloadPath = modal.TemplatePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                }

                string DownloadPath = @"" + OfficeAPPUrl + modal.DownloadPath;
                DownloadPath = DownloadPath.Replace("\\", "/");
                string prefixLocalPath = @"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\";
                //string prefixServerPath = @"C:\inetpub\wwwroot\ShipApplication\";
                string downloadLocalPath = prefixLocalPath + modal.DownloadPath;
                //string downloadServerPath = prefixServerPath + modal.DownloadPath;

                if (modal.DownloadPath != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                    //Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                    WebClient wc = new WebClient();
                    wc.DownloadFile(DownloadPath, downloadLocalPath);
                    // wc.DownloadFile(DownloadPath, downloadServerPath);
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
        public bool DownloadUpdatedFormDoc(FormModal LocalDoc, FormModal ApiDoc)
        {
            bool res = false;
            try
            {
                string oldFilePath = LocalDoc.TemplatePath;
                string oldFileName = Path.GetFileName(oldFilePath);
                string oldFileNameOrig = Path.GetFileNameWithoutExtension(oldFilePath);
                string oldFileExtension = Path.GetExtension(oldFilePath);

                try
                {
                    // Delete Old Files
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        string newFilePath = oldFilePath.Replace(oldFileName, oldFileNameOrig + "_" + LocalDoc.DocumentVersion + oldFileExtension);
                        System.IO.File.Move(oldFilePath, newFilePath);

                    }
                }
                catch (Exception)
                {
                }

                string oldFileDownloadPath = LocalDoc.DownloadPath;
                //string prefixServerPath = @"C:\inetpub\wwwroot\ShipApplication\";
                //string ServerDeletePath = prefixServerPath + oldFileDownloadPath;
                //if (!string.IsNullOrWhiteSpace(oldFileDownloadPath))
                //{
                //    if (System.IO.File.Exists(ServerDeletePath))
                //    {
                //        string newFilePath = ServerDeletePath.Replace(oldFileName, oldFileNameOrig + "_" + LocalDoc.DocumentVersion + oldFileExtension);
                //        System.IO.File.Move(ServerDeletePath, newFilePath);
                //        //System.IO.File.Delete(ServerDeletePath);
                //    }
                //}
                // Create New Files

                if (string.IsNullOrWhiteSpace(ApiDoc.DownloadPath))
                {
                    var pathIndex = ApiDoc.TemplatePath.IndexOf("Repository");
                    ApiDoc.DownloadPath = ApiDoc.TemplatePath.Substring(pathIndex);
                    //Repository
                }

                string DownloadPath = @"" + OfficeAPPUrl + ApiDoc.DownloadPath;
                //string downloadServerPath = prefixServerPath + ApiDoc.DownloadPath;
                string downloadLocalPath = ApiDoc.TemplatePath;
                //Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                WebClient wc = new WebClient();
                //wc.DownloadFile(DownloadPath, downloadServerPath);
                wc.DownloadFile(DownloadPath, downloadLocalPath);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DownloadUpdatedFormDoc :" + ex.Message);
            }
            return res;
        }
        public bool DeleteOldFile(string FilePath, string DownloadPath)
        {
            bool res = false;
            try
            {
                if (System.IO.File.Exists(FilePath))
                {
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
        public void AddSyncList(FormModal modal, ref List<SyncedForm> SyncDocList, string SyncedType)
        {
            try
            {
                SyncedForm SyncedDoc = new SyncedForm();
                SyncedDoc.FormID = modal.FormID.ToString();
                SyncedDoc.Title = modal.Title;
                SyncedDoc.SyncType = SyncedType;
                SyncedDoc.SyncDateTime = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                SyncDocList.Add(SyncedDoc);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddSyncList : " + ex.Message);
            }
        }
        public void PrintSyncList(List<SyncedForm> SyncDocList)
        {
            if (SyncDocList != null && SyncDocList.Count > 0)
            {
                LogHelper.writelog("Total Synced Forms " + SyncDocList.Count + " At " + Utility.ToDateTimeUtcNow()); //RDBJ 10/27/2021 set UtcTime
                foreach (var syncDoc in SyncDocList)
                {
                    try
                    {
                        LogHelper.writelog(syncDoc.FormID + " " + syncDoc.Title + " " + syncDoc.SyncType + " At " + Utility.ToDateTimeUtcNow()); //RDBJ 10/27/2021 set UtcTime
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("SyncFormList" + ex.Message);
                    }
                }
            }
        }
    }
}
