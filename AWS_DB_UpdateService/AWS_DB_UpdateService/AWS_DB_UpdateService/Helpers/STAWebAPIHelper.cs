using AWS_DB_UpdateService.Modals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace AWS_DB_UpdateService.Helpers
{
    public class STAWebAPIHelper
    {
        public static void ImportPeople_API(dsisy01 dbUser, TriggerLogs trgLogItem)
        {
            string filename = string.Empty;
            bool isCreated = XMLHelper.Create_XML_ImportPeople(dbUser, out filename);
            if (isCreated)
            {
                string APIUrl = "https://stacdn.seagull.no/STA40WEBAPI/Execute/R00010";
                ProcessAPI(dbUser, filename, APIUrl, trgLogItem);
            }
        }
        public static void ImportPeople_BulkAPI(List<APIRequest> lstData)
        {
            string filename = string.Empty;
            var dsisy01List = lstData.Select(x => x.objdsisy01).ToList();
            bool isCreated = XMLHelper.Create_XML_ImportPeopleBulk(dsisy01List, out filename);
            if (isCreated)
            {
                string APIUrl = "https://stacdn.seagull.no/STA40WEBAPI/Execute/R00010";
                ProcessBulkAPI(filename, APIUrl, lstData);
            }
        }
        public static void AddOrUpdatePerson_API(dsisy01 dbUser, TriggerLogs trgLogItem)
        {
            string filename = string.Empty;
            bool isCreated = XMLHelper.Create_XML_AddOrUpdatePerson(dbUser, out filename);
            if (isCreated)
            {
                string APIUrl = "https://stacdn.seagull.no/STA40WEBAPI/Execute/R01040";
                ProcessAPI(dbUser, filename, APIUrl, trgLogItem, "User");
            }
        }
        public static void AddOrUpdatePerson_BulkAPI(List<APIRequest> lstData)
        {
            string filename = string.Empty;
            var dsisy01List = lstData.Select(x => x.objdsisy01).ToList();
            bool isCreated = XMLHelper.Create_XML_AddOrUpdatePersonBulk(dsisy01List, out filename);
            if (isCreated)
            {
                string APIUrl = "https://stacdn.seagull.no/STA40WEBAPI/Execute/R01040";
                ProcessBulkAPI(filename, APIUrl, lstData, "User");
            }
        }
        public static void Import_ServiceRecordRequest_API(dsisy01 dbUser, TriggerLogs trgLogItem)
        {
            string filename = string.Empty;
            bool isCreated = XMLHelper.Create_XML_ServiceRecordRequest(dbUser, out filename);
            if (isCreated)
            {
                string APIUrl = "https://stacdn.seagull.no/STA40WEBAPI/Execute/R00040";
                ProcessAPI(dbUser, filename, APIUrl, trgLogItem);
            }
        }
        public static void Import_ServiceRecordRequest_BulkAPI(List<APIRequest> lstData)
        {
            string filename = string.Empty;
            var dsisy01List = lstData.Select(x => x.objdsisy01).ToList();
            bool isCreated = XMLHelper.Create_XML_ServiceRecordRequestBulk(dsisy01List, out filename);
            if (isCreated)
            {
                string APIUrl = "https://stacdn.seagull.no/STA40WEBAPI/Execute/R00040";
                ProcessBulkAPI(filename, APIUrl, lstData);
            }
        }
        public static void ProcessAPI(dsisy01 dbUser, string filename, string APIUrl, TriggerLogs trgLogItem, string paramName = "Data")
        {
            CookieContainer cookies = new CookieContainer();
            LoginRes lRes = APIHelper.Login(ref cookies);
            if (lRes != null && lRes.Response != null)
            {
                if (lRes.Response.Status == "SUCCESS")
                {
                    UploadFileRes reqResModal = APIHelper.Upload(cookies, filename, APIUrl, paramName);
                    UploadFileResult uploadResult = APIHelper.GetResponseDataResult(reqResModal);
                    if (uploadResult != null)
                    {
                        int TaskID = Utility.ToInteger(uploadResult.Content);
                        if (TaskID > 0)
                        {
                            UploadFileRes reqStatusModal = APIHelper.CheckRequestStatus(TaskID, cookies);
                            UploadFileResult uploadStatusResult = APIHelper.GetResponseDataResult(reqStatusModal);
                            if (uploadStatusResult != null)
                            {
                                APIHelper.Process_UpdateDB(uploadStatusResult, dbUser, TaskID, cookies, trgLogItem);
                            }
                        }
                    }
                    if (Convert.ToString(paramName).ToUpper() == "USER")
                        APIHelper.Update_TriggerUploadStatus(trgLogItem, "Success");
                }
                else
                {
                    APIHelper.PrintAPIError(lRes.Response.Status, lRes.Response.ResponseData, "Login Error : " + Utility.ToSTRING(dbUser.empnry01.Trim()));
                }
            }
            string fileUrl = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
            File.Delete(fileUrl);
        }
        public static void ProcessBulkAPI(string filename, string APIUrl, List<APIRequest> lstData, string paramName = "Data")
        {
            CookieContainer cookies = new CookieContainer();
            LoginRes lRes = APIHelper.Login(ref cookies);
            if (lRes != null && lRes.Response != null)
            {
                if (lRes.Response.Status == "SUCCESS")
                {
                    UploadFileRes reqResModal = APIHelper.Upload(cookies, filename, APIUrl, paramName);
                    UploadFileResult uploadResult = APIHelper.GetResponseDataResult(reqResModal);
                    if (uploadResult != null)
                    {
                        int TaskID = Utility.ToInteger(uploadResult.Content);
                        if (TaskID > 0)
                        {
                            UploadFileRes reqStatusModal = APIHelper.CheckRequestStatus(TaskID, cookies);
                            UploadFileResult uploadStatusResult = APIHelper.GetResponseDataResult(reqStatusModal);
                            if (uploadStatusResult != null)
                            {
                                APIHelper.Process_UpdateBulkDB(uploadStatusResult, TaskID, cookies, lstData);
                            }
                        }
                    }
                    //if (Convert.ToString(paramName).ToUpper() == "USER")
                    //    APIHelper.Update_TriggerUploadStatus(trgLogItem, "Success");
                }
                else
                {
                    foreach (var item in lstData)
                    {
                        try
                        {
                            APIHelper.PrintAPIError(lRes.Response.Status, lRes.Response.ResponseData, "ProcessBulkAPI Error : " + Utility.ToSTRING(item.objdsisy01.empnry01.Trim()));
                        }
                        catch (Exception) { }
                    }
                }
            }
            string fileUrl = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
            File.Delete(fileUrl);
        }
    }
}
