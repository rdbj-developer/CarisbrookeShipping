using AWS_DB_UpdateService.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;

namespace AWS_DB_UpdateService.Helpers
{
    public class TriggerLogHelper
    {
        public void ImportLatestData()
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
                        //SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM TriggerLogs WHERE IsUploaded = 0 and [TaskID] = '0'", conn);
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(";WITH CTE AS( SELECT ROW_NUMBER() OVER (PARTITION BY empnry01 ORDER BY empnry01) AS rn,* FROM  TriggerLogs   where  IsUploaded = 0 and [TaskID] = '0') " +
                            " select * from CTE Where rn=1", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            LogHelper.writelog(dt.Rows.Count + " Records Found ");
                            List<TriggerLogs> dataList = dt.ToListof<TriggerLogs>();
                            List<APIRequest> importAPIDataList = new List<APIRequest>();
                            List<APIRequest> updateAPIDataList = new List<APIRequest>();
                            string Action = "INSERTED";
                            foreach (TriggerLogs item in dataList)
                            {
                                dsisy01 dbUser = GetUserByEmpyNo(item.empnry01);
                                if (dbUser != null)
                                {
                                    //Check User existance in STA
                                    CookieContainer cookies = new CookieContainer();
                                    LoginRes lRes = APIHelper.Login(ref cookies);
                                    if (lRes != null && lRes.Response != null)
                                    {
                                        if (lRes.Response.Status == "SUCCESS")
                                        {
                                            var objUserResponse = APIHelper.GetUserLookupInterface(dbUser.empnry01, cookies);
                                            if (objUserResponse != null && objUserResponse.Response != null)
                                            {
                                                if (objUserResponse.Response.Status == "SUCCESS")
                                                    Action = "UPDATED";
                                                else
                                                    Action = "INSERTED";
                                            }
                                        }
                                    }
                                    if (Action == "INSERTED")
                                    {
                                        importAPIDataList.Add(new APIRequest
                                        {
                                            objdsisy01 = dbUser,
                                            objTriggerLogs = item
                                        });
                                        //STAWebAPIHelper.AddOrUpdatePerson_API(dbUser, item);
                                    }
                                    else if (Action == "UPDATED")
                                    {
                                        if (string.IsNullOrWhiteSpace(dbUser.iInstallationName))
                                        {
                                            importAPIDataList.Add(new APIRequest
                                            {
                                                objdsisy01 = dbUser,
                                                objTriggerLogs = item
                                            });
                                            //STAWebAPIHelper.AddOrUpdatePerson_API(dbUser, item);//APIHelper.Update_TriggerUploadStatus(item, "Request canceled because of iInstallationID not found.");
                                        }
                                        else
                                        {
                                            updateAPIDataList.Add(new APIRequest
                                            {
                                                objdsisy01 = dbUser,
                                                objTriggerLogs = item
                                            });
                                            //STAWebAPIHelper.Import_ServiceRecordRequest_API(dbUser, item);
                                        }
                                    }
                                }
                                else
                                    APIHelper.Update_TriggerUploadStatus(item, "Excluded former employees");
                            }
                            if (importAPIDataList != null && importAPIDataList.Count > 0)
                            {
                                importAPIDataList = importAPIDataList.GroupBy(s => s.objdsisy01.empnry01)
                                                                                 .Select(grp => grp.FirstOrDefault())
                                                                                 .OrderBy(s => s.objdsisy01.empnry01)
                                                                                 .ToList();
                                STAWebAPIHelper.ImportPeople_BulkAPI(importAPIDataList);
                            }
                            if (updateAPIDataList != null && updateAPIDataList.Count > 0)
                            {
                                updateAPIDataList = updateAPIDataList.GroupBy(s => s.objdsisy01.empnry01)
                                                                                     .Select(grp => grp.FirstOrDefault())
                                                                                     .OrderBy(s => s.objdsisy01.empnry01)
                                                                                     .ToList();
                                STAWebAPIHelper.Import_ServiceRecordRequest_BulkAPI(updateAPIDataList);
                            }
                        }
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
                LogHelper.writelog(ex.Message);
            }

        }
        public dsisy01 GetUserByEmpyNo(string empnry)
        {
            try
            {
                string connetionString = Utility.GetDBConnStr();
                LogHelper.writelog(connetionString);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("GetUserByEmpyNo : Connection Done");
                        conn.Open();
                        StringBuilder strQuery = new StringBuilder();
                        strQuery.AppendLine("SELECT d1.empnry01,d1.famname,d1.givenname,d2.bthdate01,d2.bthctye01,d2.sxee01,RM.uRankID,CM.uCountryID ");
                        strQuery.AppendLine(",ISNULL(E.line1a04,'') as line1a04, d1.crew_signon,d1.crew_signoff ");
                        //strQuery.AppendLine(",ISNULL(Cast(V.nmo04 as varchar),'') as iInstallationID, uVesselTypeID = 'Carrier' ");
                        strQuery.AppendLine(",ISNULL(IM.iInstallationID,0) as iInstallationID,ISNULL(Cast(V.nmo04 as varchar),'') as iInstallationName, uVesselTypeID = 'Carrier',ISNULL(d1.cont_employer,'') as iInstallationCode ");
                        strQuery.AppendLine("FROM dsisy01 d1 With(Nolock) ");
                        strQuery.AppendLine("Inner Join [dbo].[dsise01] d2 With(Nolock) ON d2.empnre01 = d1.empnry01 AND d2.empste01 <> 'E' "); //Excluded former emplyee
                        strQuery.AppendLine("Left Join [Ranks] R With(Nolock) ON R.fnccds26 = d2.fnce01 ");
                        strQuery.AppendLine("Left Join [RanksMaster] RM With(Nolock) ON RM.vRankName = R.[STA value] ");
                        strQuery.AppendLine("Left Join [Countries] C With(Nolock) ON C.cnts27 = d2.nate01 ");
                        strQuery.AppendLine("LEft Join [dbo].[CountryMaster] CM With(Nolock) ON CM.vCountryName = C.[STA value] ");
                        strQuery.AppendLine("Left Join [dbo].[dsisa04] E With(Nolock) ON E.empnra04 = d1.empnry01 AND commtypea04 = 'EMAIL' ");
                        strQuery.AppendLine("Left Join [dbo].[Vessels] V With(Nolock) ON V.cdo04 = d1.cont_employer ");
                        strQuery.AppendLine("Left Join [dbo].[InstallationMaster] IM With(Nolock) ON IM.vInstallationName = V.nmo04 ");
                        strQuery.AppendLine("WHERE d1.empnry01 = '" + empnry + "' AND RM.uRankID != '700A1C61-2A21-4F5A-9FA9-444497DBAA7C' ");
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(strQuery.ToString(), conn);
                        sqlAdp.Fill(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<dsisy01> dataList = dt.ToListof<dsisy01>();
                            dsisy01 dbObject = dataList[0];
                            return dbObject;
                        }
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("GetUserByEmpyNo : SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return null;
        }
        public void Check_Imported_Data_Status()
        {
            try
            {
                LogHelper.writelog("Checking For Last Imported Data");
                string connetionString = Utility.GetDBConnStr();
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT Distinct [TaskID] FROM TriggerLogs WHERE [TaskID] != '0' and [IsUploaded] = 0 and ISNULL(Attempt,0) < 5 ", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            LogHelper.writelog(dt.Rows.Count + " Records Found ");
                            List<TriggerLogs> dataList = dt.ToListof<TriggerLogs>();
                            dataList = dataList.Where(x => x.UploadStatus != "SUCCESS").ToList();
                            foreach (TriggerLogs item in dataList)
                            {
                                CookieContainer cookies = new CookieContainer();
                                LoginRes lRes = APIHelper.Login(ref cookies);
                                if (lRes != null && lRes.Response != null)
                                {
                                    if (lRes.Response.Status == "SUCCESS")
                                    {
                                        string resString = string.Empty;
                                        UploadFileRes reqStatusModal = APIHelper.CheckRequestStatus(Utility.ToInteger(item.TaskID), cookies);
                                        UploadFileResult uploadStatusResult = APIHelper.GetResponseDataResult(reqStatusModal);
                                        if (uploadStatusResult != null)
                                        {
                                            if (uploadStatusResult.Content.ToUpper() == "COMPLETED")
                                            {
                                                APIHelper.UpdateBulkUploadStatus("SUCCESS", Utility.ToSTRING(item.TaskID));
                                            }
                                            else if (uploadStatusResult.Content.ToUpper() == "FAILED")
                                            {
                                                UploadFileRes resModal = APIHelper.GetTicketResults(Utility.ToInteger(item.TaskID), cookies, out resString);
                                                if (resModal != null && resModal.Response != null)
                                                {
                                                    if (resModal.Response.Status == "SUCCESS")
                                                        APIHelper.UpdateBulkUploadStatus("SUCCESS", Utility.ToSTRING(item.TaskID));
                                                    else
                                                        APIHelper.UpdateBulkUploadStatus(resString, Utility.ToSTRING(item.TaskID), true);
                                                }
                                            }
                                            else
                                                APIHelper.UpdateBulkUploadStatus(uploadStatusResult.Content, Utility.ToSTRING(item.TaskID));

                                            //UploadFileRes resModal = APIHelper.GetTicketResults(Utility.ToInteger(item.TaskID), cookies, out resString);
                                            //if (resModal != null && resModal.Response != null)
                                            //{
                                            //    if (resModal.Response.Status == "SUCCESS")
                                            //        APIHelper.UpdateBulkUploadStatus("SUCCESS", item.TaskID.ToString());
                                            //    else
                                            //        APIHelper.UpdateBulkUploadStatus(resString, item.TaskID.ToString());
                                            //}
                                        }
                                    }
                                }
                                else
                                    APIHelper.Update_TriggerUploadStatus(item, "Excluded former employees");
                            }
                        }
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
                LogHelper.writelog(ex.Message);
            }
        }
        //public void Check_Imported_Data_Status()
        //{
        //    try
        //    {
        //        LogHelper.writelog("Checking For Last Imported Data");
        //        string connetionString = Utility.GetDBConnStr();
        //        using (var conn = new SqlConnection(connetionString))
        //        {
        //            if (conn.IsAvailable())
        //            {
        //                LogHelper.writelog("Connection Done");
        //                conn.Open();
        //                DataTable dt = new DataTable();
        //                SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM TriggerLogs WHERE [TaskID] != '0' and [IsUploaded] = 0 and ISNULL(Attempt,0) < 5 ", conn);
        //                sqlAdp.Fill(dt);
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    LogHelper.writelog(dt.Rows.Count + " Records Found ");
        //                    List<TriggerLogs> dataList = dt.ToListof<TriggerLogs>();
        //                    dataList = dataList.Where(x => x.UploadStatus != "SUCCESS").ToList();
        //                    foreach (TriggerLogs item in dataList)
        //                    {
        //                        dsisy01 dbUser = GetUserByEmpyNo(item.empnry01);
        //                        if (dbUser != null)
        //                        {
        //                            CookieContainer cookies = new CookieContainer();
        //                            LoginRes lRes = APIHelper.Login(ref cookies);
        //                            if (lRes != null && lRes.Response != null)
        //                            {
        //                                if (lRes.Response.Status == "SUCCESS")
        //                                {
        //                                    string resString = string.Empty;
        //                                    UploadFileRes resModal = APIHelper.GetTicketResults(Utility.ToInteger(item.TaskID), cookies, out resString);
        //                                    if (resModal != null && resModal.Response != null)
        //                                    {
        //                                        if (resModal.Response.Status == "SUCCESS")
        //                                        {
        //                                            //APIHelper.UpdateDB(dbUser, item);
        //                                            APIHelper.Update_DBUploadStatus(dbUser, "SUCCESS", item.TaskID.ToString(), item);
        //                                        }
        //                                        else
        //                                        {
        //                                            APIHelper.Update_DBUploadStatus(dbUser, resString, item.TaskID.ToString(), item);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else
        //                            APIHelper.Update_TriggerUploadStatus(item, "Excluded former employees");
        //                    }
        //                }
        //                conn.Close();
        //            }
        //            else
        //            {
        //                LogHelper.writelog("SQL Connection is not available");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.writelog(ex.Message);
        //    }

        //}
    }
}
