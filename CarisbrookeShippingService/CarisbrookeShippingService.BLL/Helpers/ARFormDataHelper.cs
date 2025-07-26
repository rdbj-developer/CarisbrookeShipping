using System.Collections.Generic;
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
    public class ARFormDataHelper
    {
        public void StartARSync()
        {
            try
            {
                List<ArrivalReport> UnSyncList = GetARFormsUnsyncedData();
                if (UnSyncList != null && UnSyncList.Count > 0)
                {
                    LogHelper.writelog("UnSyncList count for AR Data is about " + UnSyncList.Count + "");
                    List<long> SuccessIds = SendARDataToRemote(UnSyncList);
                    if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                    {
                        UpdateLocalARFormsStatus(SuccessIds);
                        LogHelper.writelog("AR Data Sync process done.");
                    }
                    else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                    {
                        UpdateLocalARFormsStatus(SuccessIds);
                        LogHelper.writelog("Some AR Data Synced and some remaining...");
                    }
                    else
                    {
                        LogHelper.writelog("AR Data Synced Not done.");
                    }
                }
                else
                {
                    LogHelper.writelog("AR Data Synced already done.");
                }
            }
            catch (Exception)
            {
            }
        }
        public List<ArrivalReport> GetARFormsUnsyncedData()
        {
            List<ArrivalReport> SyncList = new List<ArrivalReport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.ArrivalReports + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            SyncList = dt.ToListof<ArrivalReport>();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetARFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<long> SendARDataToRemote(List<ArrivalReport> UnSyncList)
        {
            List<long> SuccessIds = new List<long>();
            foreach (var item in UnSyncList)
            {
                long localDBARFormID = item.ARID;
                item.ARID = 0;
                APIHelper _helper = new APIHelper();
                APIResponse res = _helper.SubmitArrivalReport(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBARFormID);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalARFormsStatus(List<long> SuccessIds)
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
                        string Query = "UPDATE " + AppStatic.ArrivalReports + " SET IsSynced = 1 WHERE ARID in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalARFormsStatus : " + ex.Message);
            }
        }
    }
}
