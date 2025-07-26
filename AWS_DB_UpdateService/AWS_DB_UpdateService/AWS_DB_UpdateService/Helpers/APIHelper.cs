using AWS_DB_UpdateService.Modals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace AWS_DB_UpdateService.Helpers
{
    public static class APIHelper
    {
        public static LoginRes Login(ref CookieContainer cookies)
        {
            try
            {
                string company, user, password;
                var objLogin = Utility.ReadLoginConfigJson();
                if (objLogin != null)
                {
                    company = objLogin.Company;
                    user = objLogin.User;
                    password = objLogin.Password;
                }
                else
                {
                    company = Utility.ToSTRING(ConfigurationManager.AppSettings["company"]);
                    user = Utility.ToSTRING(ConfigurationManager.AppSettings["user"]);
                    password = Utility.ToSTRING(ConfigurationManager.AppSettings["password"]);
                }

                var request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/Login");
                var postData = "company=" + company;
                postData += "&user=" + user;
                postData += "&password=" + password;
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.CookieContainer = cookies;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                string responseData = responseReader.ReadToEnd();
                string res = responseData;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(res);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                string jsonTextw = JsonConvert.SerializeObject(responseReader.ReadToEnd());
                LoginRes resModal = JsonConvert.DeserializeObject<LoginRes>(jsonText);
                return resModal;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Login Error: " + ex.Message);
                return null;
            }
        }
        public static UploadFileRes Upload(CookieContainer cookies, string FileName, string APIUrl, string paramName = "Data")
        {
            string fileUrl = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + FileName;
            if (File.Exists(fileUrl))
            {
                try
                {
                    string replyFromServer = string.Empty;
                    var cookieContainer = new CookieContainer();
                    using (var handler = new HttpClientHandler() { CookieContainer = cookies })
                    using (var client = new HttpClient(handler))
                    {
                        MultipartFormDataContent form = new MultipartFormDataContent();
                        HttpContent content = new StringContent("content");
                        form.Add(content, "content");
                        var stream = new FileStream(fileUrl, FileMode.Open);
                        content = new StreamContent(stream);
                        var fileName =
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = paramName,
                            FileName = Path.GetFileName(fileUrl),
                        };
                        form.Add(content);
                        HttpResponseMessage response = null;

                        var url = new Uri(APIUrl);
                        response = (client.PostAsync(url, form)).Result;

                        replyFromServer = response.Content.ReadAsStringAsync().Result;
                    }

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(replyFromServer);
                    string jsonText = JsonConvert.SerializeXmlNode(doc);
                    UploadFileRes resModal = JsonConvert.DeserializeObject<UploadFileRes>(jsonText);
                    return resModal;
                }
                catch (Exception ex)
                {
                    LogHelper.writelog(ex.Message);
                }
            }
            else
            {
                LogHelper.writelog(fileUrl + " File not exist for upload");
            }
            return null;
        }
        public static UploadFileRes CheckRequestStatus(int TaskID, CookieContainer cookies)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/CheckRequestStatus?taskid=" + TaskID);
                request.CookieContainer = cookies;
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                string responseSTR = responseString;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseSTR);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                UploadFileRes resModal = JsonConvert.DeserializeObject<UploadFileRes>(jsonText);
                return resModal;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return null;
        }
        public static UploadFileRes GetTicketResults(int TaskID, CookieContainer cookies, out string resString)
        {
            resString = string.Empty;
            try
            {
                LogHelper.writelog("Get Ticke tResults");
                var request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/GetTicketResults?TaskID=" + TaskID);
                request.CookieContainer = cookies;
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                string res = responseString;
                resString = res;
                LogHelper.writelog(TaskID.ToString() + " " + res);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(res);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                UploadFileRes resModal = JsonConvert.DeserializeObject<UploadFileRes>(jsonText);
                return resModal;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return null;
        }
        public static UploadFileResult GetResponseDataResult(UploadFileRes ResModal)
        {
            UploadFileResult res = new UploadFileResult();
            if (ResModal != null && ResModal.Response != null)
            {
                if (ResModal.Response.Status == "SUCCESS")
                {
                    if (ResModal.Response.ResponseData != null && ResModal.Response.ResponseData.Result != null)
                    {
                        res.Type = ResModal.Response.ResponseData.Result.Type;
                        res.Format = ResModal.Response.ResponseData.Result.Format;
                        res.Content = ResModal.Response.ResponseData.Result.Content;
                        return res;
                    }
                }
            }
            return null;
        }
        public static UploadFileRes GetUserLookupInterface(string UserID, CookieContainer cookies)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/Execute/R02010?UserId=" + UserID);
                request.CookieContainer = cookies;
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                string responseSTR = responseString;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseSTR);
                string jsonText = JsonConvert.SerializeXmlNode(doc);
                UploadFileRes resModal = JsonConvert.DeserializeObject<UploadFileRes>(jsonText);
                return resModal;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return null;
        }
        public static void Process_UpdateDB(UploadFileResult uploadStatusResult, dsisy01 dbUser, int TaskID, CookieContainer cookies, TriggerLogs trgLogItem)
        {
            if (uploadStatusResult.Content == "Completed")
            {
                UpdateDB(dbUser, trgLogItem);
            }
            else
            {
                string resString = string.Empty;
                UploadFileRes resModal = APIHelper.GetTicketResults(TaskID, cookies, out resString);
                if (resModal != null && resModal.Response != null)
                {
                    if (resModal.Response.Status == "SUCCESS")
                    {
                        // UpdateDB(dbUser, trgLogItem);
                        Update_DBUploadStatus(dbUser, "SUCCESS", TaskID.ToString(), trgLogItem);
                    }
                    else
                    {
                        LogHelper.writelog("Failed To Upload on AWS");
                        Update_DBUploadStatus(dbUser, resString, TaskID.ToString(), trgLogItem);
                    }
                }
            }
        }
        public static void Process_UpdateBulkDB(UploadFileResult uploadStatusResult, int TaskID, CookieContainer cookies, List<APIRequest> lstData)
        {
            if (uploadStatusResult.Content.ToUpper() == "COMPLETED")
            {
                UpdateBulkDB(lstData, Utility.ToSTRING(TaskID));
            }
            else if (uploadStatusResult.Content.ToUpper() == "FAILED")
            {
                //UpdateBulkUploadStatus(uploadStatusResult.Content, Utility.ToSTRING(TaskID))
                string resString = string.Empty;
                UploadFileRes resModal = GetTicketResults(TaskID, cookies, out resString);
                if (resModal != null && resModal.Response != null)
                {
                    if (resModal.Response.Status.ToUpper() == "SUCCESS")
                    {
                        UpdateBulkDB(lstData, Utility.ToSTRING(TaskID));
                    }
                    else
                    {
                        APIHelper.UpdateBulkUploadTaskId(resString, Utility.ToSTRING(TaskID), lstData, true);
                        //APIHelper.UpdateBulkUploadStatus(resString, Utility.ToSTRING(TaskID), true);
                        //if (resModal.Response.ResponseData != null && resModal.Response.ResponseData.Error != null && resModal.Response.ResponseData.Error.Content != null)
                        //{
                        //    var t = resModal.Response.ResponseData.Error.Content;
                        //}
                        ////Update Failed status for rejected records....
                        //foreach (var item in lstData)
                        //{
                        //    try
                        //    {
                        //        Update_DBUploadStatus(item.objdsisy01, resString, TaskID.ToString(), item.objTriggerLogs);
                        //    }
                        //    catch (Exception) { }
                        //}
                        LogHelper.writelog("Failed To Upload on AWS");
                    }
                }
                else APIHelper.UpdateBulkUploadTaskId(resString, Utility.ToSTRING(TaskID), lstData, true); //APIHelper.UpdateBulkUploadStatus(resString, Utility.ToSTRING(TaskID), true);
            }
            else
                APIHelper.UpdateBulkUploadTaskId(uploadStatusResult.Content, Utility.ToSTRING(TaskID), lstData); // APIHelper.UpdateBulkUploadStatus(uploadStatusResult.Content, Utility.ToSTRING(TaskID));
        }
        //public static void Process_UpdateBulkDB(UploadFileResult uploadStatusResult, int TaskID, CookieContainer cookies, List<APIRequest> lstData)
        //{
        //    if (uploadStatusResult.Content == "Completed")
        //    {
        //        UpdateBulkDB(lstData, Utility.ToSTRING(TaskID));
        //    }
        //    else
        //    {
        //        //UpdateBulkUploadStatus(uploadStatusResult.Content, Utility.ToSTRING(TaskID))
        //        string resString = string.Empty;
        //        UploadFileRes resModal = GetTicketResults(TaskID, cookies, out resString);
        //        if (resModal != null && resModal.Response != null)
        //        {
        //            if (resModal.Response.Status == "SUCCESS")
        //            {
        //                UpdateBulkDB(lstData, Utility.ToSTRING(TaskID));
        //            }
        //            else
        //            {
        //                if (resModal.Response.ResponseData != null && resModal.Response.ResponseData.Error != null && resModal.Response.ResponseData.Error.Content != null)
        //                {
        //                    var t = resModal.Response.ResponseData.Error.Content;
        //                }
        //                //Update Failed status for rejected records....
        //                foreach (var item in lstData)
        //                {
        //                    try
        //                    {
        //                        Update_DBUploadStatus(item.objdsisy01, resString, TaskID.ToString(), item.objTriggerLogs);
        //                    }
        //                    catch (Exception) { }
        //                }
        //                LogHelper.writelog("Failed To Upload on AWS");
        //            }
        //        }
        //    }
        //}
        public static void Update_DBUploadStatus(dsisy01 dbUser, string resStr, string TaskID, TriggerLogs trgLogItem)
        {
            try
            {
                string strQuryIsUploaded = "";
                if (resStr == "SUCCESS")
                    strQuryIsUploaded = " , IsUploaded = 1 ";
                string connetionString = Utility.GetDBConnStr();
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "Update T SET T.Attempt=ISNULL(T.Attempt,0) + 1, UploadStatus = '" + resStr + "',TaskID = '" + TaskID + "' " + strQuryIsUploaded + " FROM [TriggerLogs] T Where T.empnry01 = '" + dbUser.empnry01 + "' and ID=" + trgLogItem.ID;
                        SqlCommand sqlcom = new SqlCommand(query, conn);
                        sqlcom.ExecuteNonQuery();
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(Utility.ToSTRING(dbUser.empnry01.Trim()) + " Record Update Failed!!!");
                LogHelper.writelog(ex.Message);
            }
        }
        public static void UpdateDB(dsisy01 dbUser, TriggerLogs trgLogItem)
        {
            try
            {
                string connetionString = Utility.GetDBConnStr();
                LogHelper.writelog(connetionString);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "Update T SET IsUploaded = 1,UploadStatus = 'SUCCESS', T.Attempt=1 FROM [TriggerLogs] T Where T.empnry01 = '" + dbUser.empnry01 + "' and ID=" + trgLogItem.ID;
                        SqlCommand sqlcom = new SqlCommand(query, conn);
                        sqlcom.ExecuteNonQuery();
                        conn.Close();
                        LogHelper.writelog(Utility.ToSTRING(dbUser.empnry01.Trim()) + " Record Updated Successfully");
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(Utility.ToSTRING(dbUser.empnry01.Trim()) + " Record Update Failed!!!");
                LogHelper.writelog(ex.Message);
            }
        }
        public static void UpdateBulkDB(List<APIRequest> lstData, string TaskID)
        {
            string dbUserIds = string.Empty;
            try
            {
                dbUserIds = string.Join(",", lstData.Select(i => Utility.ToSTRING(i.objdsisy01.empnry01).Trim()));
                string taskIds = string.Join(",", lstData.Select(i => i.objTriggerLogs.ID));

                string connetionString = Utility.GetDBConnStr();
                LogHelper.writelog(connetionString);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "Update T SET IsUploaded = 1,UploadStatus = 'SUCCESS', T.Attempt=1, T.TaskID=" + TaskID + " FROM [TriggerLogs] T Where T.empnry01 IN(" + dbUserIds + ") and ID IN(" + taskIds + ")";
                        SqlCommand sqlcom = new SqlCommand(query, conn);
                        sqlcom.ExecuteNonQuery();
                        conn.Close();
                        LogHelper.writelog(dbUserIds + " Records Updated Successfully");
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(dbUserIds + " Record Update Failed!!!");
                LogHelper.writelog(ex.Message);
            }
        }

        public static void UpdateBulkUploadTaskId(string resStr, string TaskID, List<APIRequest> lstData, bool isFinalStatus = false)
        {
            try
            {
                string dbUserIds = string.Empty;
                dbUserIds = string.Join(",", lstData.Select(i => Utility.ToSTRING(i.objdsisy01.empnry01).Trim()));
                string taskIds = string.Join(",", lstData.Select(i => i.objTriggerLogs.ID));

                string strQuryIsUploaded = "";
                if (resStr == "SUCCESS")
                    strQuryIsUploaded = " , IsUploaded = 1 ";
                else if (isFinalStatus)
                {
                    strQuryIsUploaded = " , IsUploaded = 1 ";
                }
                if (string.IsNullOrWhiteSpace(resStr))
                    resStr = "";
                string connetionString = Utility.GetDBConnStr();
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "Update T SET T.Attempt=ISNULL(T.Attempt,0) + 1, T.TaskID=" + TaskID + " , UploadStatus = '" + resStr.Replace("'", "''") + "'" + strQuryIsUploaded + " FROM [TriggerLogs] T Where T.empnry01 IN(" + dbUserIds + ") and ID IN(" + taskIds + ")";
                        SqlCommand sqlcom = new SqlCommand(query, conn);
                        sqlcom.ExecuteNonQuery();
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateBulkUploadTaskId : " + Utility.ToSTRING(TaskID) + " Record Update Failed!!!");
                LogHelper.writelog("UpdateBulkUploadTaskId : " + ex.Message);
            }
        }

        public static void UpdateBulkUploadStatus(string resStr, string TaskID, bool isFinalStatus = false)
        {
            try
            {
                string strQuryIsUploaded = "";
                if (resStr == "SUCCESS")
                    strQuryIsUploaded = " , IsUploaded = 1 ";
                else if (isFinalStatus)
                {
                    strQuryIsUploaded = " , IsUploaded = 1 ";
                }
                if (string.IsNullOrWhiteSpace(resStr))
                    resStr = "";
                string connetionString = Utility.GetDBConnStr();
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "Update T SET T.Attempt=ISNULL(T.Attempt,0) + 1, UploadStatus = '" + resStr.Replace("'", "''") + "'" + strQuryIsUploaded + " FROM [TriggerLogs] T Where T.TaskID=" + TaskID;
                        SqlCommand sqlcom = new SqlCommand(query, conn);
                        sqlcom.ExecuteNonQuery();
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateBulkUploadStatus : " + Utility.ToSTRING(TaskID) + " Record Update Failed!!!");
                LogHelper.writelog("UpdateBulkUploadStatus : " + ex.Message);
            }
        }

        public static void Update_TriggerUploadStatus(TriggerLogs trgLogItem, string status)
        {
            try
            {
                string connetionString = Utility.GetDBConnStr();
                LogHelper.writelog(connetionString);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "Update T SET IsUploaded = 1,UploadStatus = '" + status + "', T.Attempt=1 FROM [TriggerLogs] T Where T.empnry01 = '" + trgLogItem.empnry01 + "' and ID=" + trgLogItem.ID;
                        SqlCommand sqlcom = new SqlCommand(query, conn);
                        sqlcom.ExecuteNonQuery();
                        conn.Close();
                        LogHelper.writelog(Utility.ToSTRING(trgLogItem.empnry01.Trim()) + " Record Updated Successfully");
                    }
                    else
                        LogHelper.writelog("SQL Connection is not available");
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(Utility.ToSTRING(trgLogItem.empnry01.Trim()) + " Record Update Failed!!!");
                LogHelper.writelog(ex.Message);
            }
        }
        public static void PrintAPIError(string Status, object ResponseData, string content)
        {
            LogHelper.writelog(content);
            LogHelper.writelog(Status);
            var errorMessage = JsonConvert.SerializeObject(ResponseData);
            Failed_Result err = JsonConvert.DeserializeObject<Failed_Result>(errorMessage);
            LogHelper.writelog(err.Error.Code);
            LogHelper.writelog(err.Error.Description);
            LogHelper.writelog("==========================================================");
        }
    }
}
