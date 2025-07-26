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
    public class SMRFormDataHelper
    {
        public void StartSMRSync()
        {
            try
            {
                List<SMRModal> UnSyncList = GetSMRFormsUnsyncedData();
                if (UnSyncList != null && UnSyncList.Count > 0)
                {
                    LogHelper.writelog("UnSyncList count for SMR Data is about " + UnSyncList.Count + "");
                    List<long> SuccessIds = SendSMRDataToRemote(UnSyncList);
                    if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                    {
                        UpdateLocalSMRFormsStatus(SuccessIds);
                        LogHelper.writelog("SMR Data Sync process done.");
                    }
                    else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                    {
                        UpdateLocalSMRFormsStatus(SuccessIds);
                        LogHelper.writelog("Some SMR Data Synced and some remaining...");
                    }
                    else
                    {
                        LogHelper.writelog("SMR Data Synced Not done.");
                    }
                }
                else
                {
                    LogHelper.writelog("SMR Data Synced already done.");
                }
            }
            catch (Exception)
            {
                
            }
        }
        public List<long> SendSMRDataToRemote(List<SMRModal> UnSyncList)
        {
            List<long> SuccessIds = new List<long>();
            foreach (var item in UnSyncList)
            {
                APIHelper _helper = new APIHelper();
                APIResponse res = _helper.SubmitSMR(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(item.SMRFormID);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalSMRFormsStatus(List<long> SuccessIds)
        {
            try
            {
                if (SuccessIds.Count > 0)
                {
                    string IdsStr = string.Join(",", SuccessIds);
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            string Query = "UPDATE " + AppStatic.SMRForm + " SET IsSynced = 1 WHERE SMRFormID in (" + IdsStr + ")";
                            SqlCommand cmd = new SqlCommand(Query, conn);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalSMRFormsStatus : " + ex.Message);
            }
        }
        public List<SMRModal> GetSMRFormsUnsyncedData()
        {
            List<SMRModal> SyncList = new List<SMRModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SMRForm + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<SMRModal>();
                            foreach (var item in SyncList)
                            {
                                DataTable dtMembers = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SMRFormCrewMembers + " WHERE SMRFormID = " + item.SMRFormID + "", conn);
                                sqlAdp.Fill(dtMembers);
                                item.SMRFormCrewMemberList = dtMembers.ToListof<SMRFormCrewMemberModal>();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
    }
}
