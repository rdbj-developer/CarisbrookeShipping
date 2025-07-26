using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class FeedbackFormDataHelper
    {
        public void StartFeedbackFormSync()
        {
            List<FeedbackFormModal> UnSyncList = GetFeedbackFormUnsyncedData();
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for FeedbackForm Data is about " + UnSyncList.Count + "");
                List<Guid> SuccessIds = SendFeedbackFormDataToRemote(UnSyncList);
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    UpdateLocalFeedbackFormStatus(SuccessIds);
                    if (SuccessIds.Count == UnSyncList.Count)
                        LogHelper.writelog("FeedbackForm Data Sync process done.");
                    if (SuccessIds.Count < UnSyncList.Count)
                        LogHelper.writelog("Some FeedbackForm Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("FeedbackForm Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("FeedbackForm Data Synced already done.");
            }
        }
        public List<FeedbackFormModal> GetFeedbackFormUnsyncedData()
        {
            List<FeedbackFormModal> SyncList = new List<FeedbackFormModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                        conn.Open();
                    {

                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.FeedbackForm + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<FeedbackFormModal>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFeedbackFormUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<Guid> SendFeedbackFormDataToRemote(List<FeedbackFormModal> UnSyncList)
        {
            List<Guid> SuccessIds = new List<Guid>();
            Guid tempId = Guid.Empty;
            foreach (var item in UnSyncList)
            {
                if (item != null)
                {
                    tempId = item.Id;
                    APIHelper _helper = new APIHelper();
                    APIResponse res = _helper.SubmitFeedbackForm(item);
                    if (res != null && res.result == AppStatic.SUCCESS)
                        SuccessIds.Add(tempId);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalFeedbackFormStatus(List<Guid> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds.Select(x => string.Format("'{0}'", x)).ToList());
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        string Query = "UPDATE " + AppStatic.FeedbackForm + " SET IsSynced = 1 WHERE Id in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalFeedbackFormStatus : " + ex.Message);
            }
        }
    }
}
