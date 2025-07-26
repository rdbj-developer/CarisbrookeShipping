using CarisbrookeOpenFileService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

namespace CarisbrookeOpenFileService.Helper
{
    public class DocumentsHelper
    {
        public void StartDocSync()
        {
            APIHelper _helper = new APIHelper();
            List<SyncedDocument> SyncedDocList = new List<SyncedDocument>();
            List<DocumentModal> APIDocsList = _helper.GetAllDocumentsForService();
            if (APIDocsList != null && APIDocsList.Count > 0)
            {
                List<DocumentModal> LocalDocsList = GetLocalDocs(APIDocsList, false);
                if (LocalDocsList == null || LocalDocsList.Count <= 0)
                {
                    foreach (var ApiDoc in APIDocsList)
                    {
                        bool isCreatedOnLocal = CreatePhysicalLocation(ApiDoc);
                    }
                }
                else
                {
                    foreach (var ApiDoc in APIDocsList)
                    {
                        DocumentModal LocalDoc = LocalDocsList.Where(x => x.DocumentID == ApiDoc.DocumentID).FirstOrDefault();
                        if (LocalDoc != null)
                        {
                            // Exist in local machine - check for update
                            UpdateDocumentProcess(ApiDoc, LocalDoc, ref SyncedDocList);
                        }
                        else
                        {
                            // Not Exist in local machine - Insert New Form
                            InsertDocumentProcess(ApiDoc, ref SyncedDocList);
                        }
                        //bool isCreatedOnLocal = CreatePhysicalLocation(ApiDoc);
                    }
                    if (SyncedDocList != null && SyncedDocList.Count > 0)
                        GetLocalDocs(APIDocsList, true);
                }
            }
            PrintSyncList(SyncedDocList);
        }
        public void InsertDocumentProcess(DocumentModal ApiDoc, ref List<SyncedDocument> SyncedDocList)
        {
            if (ApiDoc.IsDeleted == false)
            {
                bool isCreatedOnLocal = CreatePhysicalLocation(ApiDoc);
                if (isCreatedOnLocal == true)
                    AddSyncList(ApiDoc, ref SyncedDocList, AppStatic.INSERTED);
            }
        }
        public void UpdateDocumentProcess(DocumentModal ApiDoc, DocumentModal LocalDoc, ref List<SyncedDocument> SyncedDocList)
        {
            if (LocalDoc.DocumentVersion != ApiDoc.DocumentVersion || (!string.IsNullOrWhiteSpace(LocalDoc.Path) && !System.IO.File.Exists(LocalDoc.Path)))
            {
                bool res = DownloadUpdatedDoc(LocalDoc, ApiDoc);
                if (res)
                {
                    LocalDoc.Path = ApiDoc.Path;
                    LocalDoc.DownloadPath = ApiDoc.DownloadPath;
                    LocalDoc.UploadType = ApiDoc.UploadType;
                    LocalDoc.DocumentVersion = ApiDoc.DocumentVersion;
                    if (res == true)
                    {
                        AddSyncList(ApiDoc, ref SyncedDocList, AppStatic.UPDATED);
                    }
                }
            }
            if (LocalDoc.IsDeleted != ApiDoc.IsDeleted)
                LocalDoc.IsDeleted = ApiDoc.IsDeleted;
        }
        public List<DocumentModal> GetLocalDocs(List<DocumentModal> APIDocList, bool isUpdate)
        {
            List<DocumentModal> LocalDocsList = new List<DocumentModal>();
            try
            {
                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["DocumentsListPath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (File.Exists(jsonFilePath))
                {
                    jsonText = File.ReadAllText(jsonFilePath);
                }
                if (!string.IsNullOrEmpty(jsonText) && !isUpdate)
                {
                    List<DocumentModal> Modal = JsonConvert.DeserializeObject<List<DocumentModal>>(jsonText);
                    return Modal;
                }
                else
                    isUpdate = true;
                if (isUpdate)
                {
                    jsonText = JsonConvert.SerializeObject(APIDocList);
                    File.WriteAllText(jsonFilePath, jsonText);
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLocalDocs : " + ex.Message);
            }
            return LocalDocsList;
        }

        public bool CreatePhysicalLocation(DocumentModal modal)
        {
            bool res = false;
            try
            {
                string OfficeAPPUrl = ConfigurationManager.AppSettings["OfficeAPPUrl"];
                if (string.IsNullOrEmpty(modal.DownloadPath))
                {
                    modal.DownloadPath = modal.Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                }

                string DownloadPath = @"" + OfficeAPPUrl + modal.DownloadPath;
                DownloadPath = DownloadPath.Replace("\\", "/");
                string prefixLocalPath = @"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\";
                string prefixServerPath = string.Format(@"{0}", ConfigurationManager.AppSettings["PrefixServerPath"]);
                string downloadLocalPath = prefixLocalPath + modal.DownloadPath;
                string downloadServerPath = prefixServerPath + modal.DownloadPath;
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
        public bool DownloadUpdatedDoc(DocumentModal LocalDoc, DocumentModal ApiDoc)
        {
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
                    if (File.Exists(oldFilePath))
                    {
                        newFilePath = oldFilePath.Replace(oldFileName, oldFileNameOrig + "_" + LocalDoc.DocumentVersion + oldFileExtension);
                        File.Move(oldFilePath, newFilePath);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.writelog("DownloadUpdatedDoc :- Delete Old Files :" + e.Message);
                }
                if (string.IsNullOrEmpty(LocalDoc.DownloadPath))
                {
                    LocalDoc.DownloadPath = LocalDoc.Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                }
                if (LocalDoc.DownloadPath.Contains(@"C:\Carisbrooke Shipping Ltd\Saved Forms"))
                {
                    LocalDoc.DownloadPath = LocalDoc.Path.Replace(@"C:\Carisbrooke Shipping Ltd\Saved Forms", @"\Repository\Saved Forms");
                }
                if (string.IsNullOrEmpty(ApiDoc.DownloadPath))
                    ApiDoc.DownloadPath = ApiDoc.Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                if (ApiDoc.DownloadPath.Contains(@"C:\Carisbrooke Shipping Ltd\Saved Forms"))
                {
                    ApiDoc.DownloadPath = LocalDoc.Path.Replace(@"C:\Carisbrooke Shipping Ltd\Saved Forms", @"\Repository\Saved Forms");
                }

                string oldFileDownloadPath = LocalDoc.DownloadPath;
                string prefixServerPath = string.Format(@"{0}", ConfigurationManager.AppSettings["PrefixServerPath"]);
                string ServerDeletePath = prefixServerPath + oldFileDownloadPath;

                // Create New Files
                string OfficeAPPUrl = ConfigurationManager.AppSettings["OfficeAPPUrl"];
                string DownloadPath = @"" + OfficeAPPUrl + ApiDoc.DownloadPath;
                string downloadServerPath = prefixServerPath + ApiDoc.DownloadPath;
                string downloadLocalPath = ApiDoc.Path;
                //Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                //Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));                

                if (LocalDoc.Type == "FOLDER" || Convert.ToString(LocalDoc.Type).ToLower() == "windowsfolder")
                {
                    if (Directory.Exists(downloadLocalPath))
                        return false;
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                    //Directory.CreateDirectory(Path.GetDirectoryName(downloadServerPath));
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));
                    }
                    catch (Exception)
                    {

                    }
                    WebClient wc = new WebClient();
                    //wc.DownloadFile(DownloadPath, downloadServerPath);
                    wc.DownloadFile(DownloadPath, downloadLocalPath);
                    if (File.Exists(newFilePath)) //Delete Renamed FilePath
                        File.Delete(newFilePath);                    
                }

                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message + " : Path: " + LocalDoc.Path);
            }
            return res;
        }
        public bool DeleteOldFile(string FilePath, string DownloadPath)
        {
            bool res = false;
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
                string prefixServerPath = string.Format(@"{0}", ConfigurationManager.AppSettings["PrefixServerPath"]);
                string ServerDeletePath = prefixServerPath + DownloadPath;
                if (File.Exists(ServerDeletePath))
                {
                    File.Delete(ServerDeletePath);
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
                SyncedDoc.SyncDateTime = DateTime.Now;
                SyncedDoc.DocumentVersion = modal.DocumentVersion;
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
                LogHelper.writelog("Total Synced Documents " + SyncDocList.Count + " At " + DateTime.Now);
                foreach (var syncDoc in SyncDocList)
                {
                    try
                    {
                        LogHelper.writelog(syncDoc.DocumentID + " " + syncDoc.Title + " " + syncDoc.SyncType + " At " + DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("SyncDocList" + ex.Message);
                    }
                }
            }
        }
    }
}
