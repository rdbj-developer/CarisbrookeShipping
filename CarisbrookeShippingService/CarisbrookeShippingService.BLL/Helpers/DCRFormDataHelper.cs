using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class DCRFormDataHelper
    {
        public void StartDCRSync()
        {
            try
            {
                List<DailyCargoReport> UnSyncList = GetDCRFormsUnsyncedData();
                if (UnSyncList != null && UnSyncList.Count > 0)
                {
                    LogHelper.writelog("UnSyncList count for DCR Data is about " + UnSyncList.Count + "");
                    List<long> SuccessIds = SendDCRDataToRemote(UnSyncList);
                    if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                    {
                        UpdateLocalDCRFormsStatus(SuccessIds);
                        LogHelper.writelog("DCR Data Sync process done.");
                    }
                    else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                    {
                        UpdateLocalDCRFormsStatus(SuccessIds);
                        LogHelper.writelog("Some DCR Data Synced and some remaining...");
                    }
                    else
                    {
                        LogHelper.writelog("DCR Data Synced Not done.");
                    }
                }
                else
                {
                    LogHelper.writelog("DCR Data Synced already done.");
                }
            }
            catch (Exception)
            {
                
            }
        }
        public List<DailyCargoReport> GetDCRFormsUnsyncedData()
        {
            List<DailyCargoReport> SyncList = new List<DailyCargoReport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.DailyCargoReports + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            SyncList = dt.ToListof<DailyCargoReport>();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDCRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<long> SendDCRDataToRemote(List<DailyCargoReport> UnSyncList)
        {
            List<long> SuccessIds = new List<long>();
            foreach (var item in UnSyncList)
            {
                long localDBARFormID = item.DCRID;
                item.DCRID = 0;
                APIHelper _helper = new APIHelper();
                APIResponse res = _helper.SubmitDailyCargoReport(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBARFormID);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalDCRFormsStatus(List<long> SuccessIds)
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
                        string Query = "UPDATE " + AppStatic.DailyCargoReports + " SET IsSynced = 1 WHERE DCRID in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalDCRFormsStatus : " + ex.Message);
            }
        }
    }
}
