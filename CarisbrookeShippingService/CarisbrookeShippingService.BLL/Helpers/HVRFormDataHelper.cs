using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class HVRFormDataHelper
    {
        public void StartHVRSync()
        {
            List<HVRModal> UnSyncList = GetHVRFormsUnsyncedData();
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for HVR Data is about " + UnSyncList.Count + "");
                List<long> SuccessIds = SendHVRDataToRemote(UnSyncList);
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    UpdateLocalHVRFormsStatus(SuccessIds);

                    if (SuccessIds.Count == UnSyncList.Count)
                        LogHelper.writelog("HVR Data Sync process done.");
                    if (SuccessIds.Count < UnSyncList.Count)
                        LogHelper.writelog("Some HVR Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("HVR Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("HVR Data Synced already done.");
            }
        }
        public List<HVRModal> GetHVRFormsUnsyncedData()
        {
            List<HVRModal> SyncList = new List<HVRModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.HoldVentilationRecordForm + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<HoldVentilationRecordForm> HVRSyncList = dt.ToListof<HoldVentilationRecordForm>();
                            foreach (var item in HVRSyncList)
                            {
                                try
                                {
                                    HVRModal Modal = new HVRModal();

                                    Modal.HoldVentilationRecordForm = item;

                                    DataTable dtNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.HoldVentilationRecordSheet + " WHERE HoldVentilationRecordFormId = " + item.HoldVentilationRecordFormId + "", conn);
                                    sqlAdp.Fill(dtNotes);
                                    Modal.HoldVentilationRecordList = dtNotes.ToListof<HoldVentilationRecordSheetModal>();

                                    SyncList.Add(Modal);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("HVRFormData Error : HoldVentilationRecordFormId : " + item.HoldVentilationRecordFormId + " " + ex.Message);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetHVRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<long> SendHVRDataToRemote(List<HVRModal> UnSyncList)
        {
            List<long> SuccessIds = new List<long>();
            foreach (var item in UnSyncList)
            {
                if (item.HoldVentilationRecordForm != null && item.HoldVentilationRecordForm.HoldVentilationRecordFormId > 0)
                {
                    long localHoldVentilationRecordFormId = item.HoldVentilationRecordForm.HoldVentilationRecordFormId;
                    item.HoldVentilationRecordForm.HoldVentilationRecordFormId = 0;
                    APIHelper _helper = new APIHelper();
                    APIResponse res = _helper.SubmitHVR(item);
                    if (res != null && res.result == AppStatic.SUCCESS)
                    {
                        SuccessIds.Add(localHoldVentilationRecordFormId);
                    }
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalHVRFormsStatus(List<long> SuccessIds)
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
                        string Query = "UPDATE " + AppStatic.HoldVentilationRecordForm + " SET IsSynced = 1 WHERE HoldVentilationRecordFormId in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalHVRFormsStatus : " + ex.Message);
            }
        }
    }
}
