using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class DRFormDataHelper
    {
        public void StartDRSync()
        {
            try
            {
                List<DepartureReport> UnSyncList = GetDRFormsUnsyncedData();
                if (UnSyncList != null && UnSyncList.Count > 0)
                {
                    LogHelper.writelog("UnSyncList count for DR Data is about " + UnSyncList.Count + "");
                    List<long> SuccessIds = SendDRDataToRemote(UnSyncList);
                    if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                    {
                        UpdateLocalDRFormsStatus(SuccessIds);
                        LogHelper.writelog("DR Data Sync process done.");
                    }
                    else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                    {
                        UpdateLocalDRFormsStatus(SuccessIds);
                        LogHelper.writelog("Some DR Data Synced and some remaining...");
                    }
                    else
                    {
                        LogHelper.writelog("DR Data Synced Not done.");
                    }
                }
                else
                {
                    LogHelper.writelog("DR Data Synced already done.");
                }
            }
            catch (Exception)
            {
            }
        }
        public List<DepartureReport> GetDRFormsUnsyncedData()
        {
            List<DepartureReport> SyncList = new List<DepartureReport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.DepartureReports + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            SyncList = dt.ToListof<DepartureReport>();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<long> SendDRDataToRemote(List<DepartureReport> UnSyncList)
        {
            List<long> SuccessIds = new List<long>();
            foreach (var item in UnSyncList)
            {
                long localDBDRFormID = item.DRID;
                item.DRID = 0;
                APIHelper _helper = new APIHelper();
                APIResponse res = _helper.SubmitDepartureReport(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBDRFormID);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalDRFormsStatus(List<long> SuccessIds)
        {
            try
            {
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    string IdsStr = string.Join(",", SuccessIds);
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            string Query = "UPDATE " + AppStatic.DepartureReports + " SET IsSynced = 1 WHERE DRID in (" + IdsStr + ")";
                            SqlCommand cmd = new SqlCommand(Query, conn);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalDRFormsStatus : " + ex.Message);
            }
        }
    }
}
