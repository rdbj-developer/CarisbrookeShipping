using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace CarisbrookeShippingService.BLL.Helpers
{
    public class DPRFormDataHelper
    {
        public void StartDPRSync()
        {
            try
            {
                List<DailyPositionReport> UnSyncList = GetDPRFormsUnsyncedData();
                if (UnSyncList != null && UnSyncList.Count > 0)
                {
                    LogHelper.writelog("UnSyncList count for DPR Data is about " + UnSyncList.Count + "");
                    List<long> SuccessIds = SendDPRDataToRemote(UnSyncList);
                    if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                    {
                        UpdateLocalDPRFormsStatus(SuccessIds);
                        LogHelper.writelog("DPR Data Sync process done.");
                    }
                    else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                    {
                        UpdateLocalDPRFormsStatus(SuccessIds);
                        LogHelper.writelog("Some DPR Data Synced and some remaining...");
                    }
                    else
                    {
                        LogHelper.writelog("DPR Data Synced Not done.");
                    }
                }
                else
                {
                    LogHelper.writelog("DPR Data Synced already done.");
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StartDPRSync Error: " + ex.Message);
            }
        }
        public List<DailyPositionReport> GetDPRFormsUnsyncedData()
        {
            List<DailyPositionReport> SyncList = new List<DailyPositionReport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.DailyPositionReport + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            SyncList = dt.ToListof<DailyPositionReport>();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDPRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<long> SendDPRDataToRemote(List<DailyPositionReport> UnSyncList)
        {
            List<long> SuccessIds = new List<long>();
            foreach (var item in UnSyncList)
            {
                long localDBARFormID = item.DPRID;
                item.DPRID = 0;
                APIHelper _helper = new APIHelper();
                APIResponse res = _helper.SubmitDailyPositionReport(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBARFormID);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalDPRFormsStatus(List<long> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds);
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        string Query = "UPDATE " + AppStatic.DailyPositionReport + " SET IsSynced = 1 WHERE DPRID in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalDPRFormsStatus : " + ex.Message);
            }
        }
    }
}
