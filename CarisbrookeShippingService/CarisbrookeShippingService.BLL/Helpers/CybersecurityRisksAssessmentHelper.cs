using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace CarisbrookeShippingService.BLL.Helpers
{
    public class CybersecurityRisksAssessmentHelper
    {
        public void StartCRASync()
        {
            try
            {
                List<Guid> SuccessIds = SyncRemoteDataToLocal();
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    LogHelper.writelog("CybersecurityRisksAssessment Data Sync from Remote process done.");
                }
                //Check to Sync Remote Changes                
                LogHelper.writelog("CybersecurityRisksAssessment Data Synced already done..");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StartCRASync " + ex.Message);
            }
        }
        public List<Guid> SyncRemoteDataToLocal()
        {
            List<Guid> SuccessIds = new List<Guid>();
            try
            {
                bool isSuccess = false;
                var objShip = Utility.GetShipValue();
                if (objShip != null && !string.IsNullOrWhiteSpace(objShip.id))
                {
                    APIHelper _helper = new APIHelper();
                    var unSyncRemotChanges = _helper.GetCybersecurityRisksAssessmentUnSyncData(objShip.id);
                    if (unSyncRemotChanges != null && unSyncRemotChanges.CybersecurityRisksAssessmentForm != null && unSyncRemotChanges.CybersecurityRisksAssessmentForm.CRAId != Guid.Empty)
                    {
                        unSyncRemotChanges.CybersecurityRisksAssessmentForm.IsSynced = true;
                        //Check if Entry Exist? If Exist then make entry or else update it
                        var lstLocalData = GetCRAListUnsyncedLocalData(true);
                        if (lstLocalData != null && lstLocalData.Count > 0)
                        {
                            var matchingData = lstLocalData.Where(x => x.CybersecurityRisksAssessmentForm.ShipCode == unSyncRemotChanges.CybersecurityRisksAssessmentForm.ShipCode).FirstOrDefault();
                            if (matchingData != null)
                            {
                                unSyncRemotChanges.CybersecurityRisksAssessmentForm.CRAId = matchingData.CybersecurityRisksAssessmentForm.CRAId;
                                //Update Local Entry
                                isSuccess = UpdateCybersecurityRisksAssessmentDataInLocalDB(unSyncRemotChanges);
                                LogHelper.writelog("Updated from Office DB");
                            }
                            else
                            {
                                //New Entry
                                isSuccess = InsertCybersecurityRisksAssessmentDataInLocalDB(unSyncRemotChanges);
                                LogHelper.writelog("Inserted from Office DB");
                            }
                        }
                        else
                        {
                            //New Entry
                            isSuccess = InsertCybersecurityRisksAssessmentDataInLocalDB(unSyncRemotChanges);
                            LogHelper.writelog("Inserted from Office DB");
                        }
                        if (isSuccess)
                        {
                            _helper.UpdateAssetManagmentEquipmentSyncStatus(objShip.id, true);
                            SuccessIds.Add(unSyncRemotChanges.CybersecurityRisksAssessmentForm.CRAId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SyncRemoteDataToLocal : " + ex.Message);
            }
            return SuccessIds;
        }
        public List<CybersecurityRisksAssessmentModal> GetCRAListUnsyncedLocalData(bool isGetAllData = false)
        {
            List<CybersecurityRisksAssessmentModal> SyncList = new List<CybersecurityRisksAssessmentModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "SELECT * FROM " + AppStatic.CybersecurityRisksAssessment;
                        if (!isGetAllData)
                            query += " WHERE ISNULL(IsSynced,0) = 0";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<CybersecurityRisksAssessmentForm> HVRSyncList = dt.ToListof<CybersecurityRisksAssessmentForm>();
                            foreach (var item in HVRSyncList)
                            {
                                try
                                {
                                    CybersecurityRisksAssessmentModal Modal = new CybersecurityRisksAssessmentModal();
                                    Modal.CybersecurityRisksAssessmentForm = item;

                                    DataTable dtNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.CybersecurityRisksAssessmentList + " WHERE CRAId = '" + item.CRAId + "'", conn);
                                    sqlAdp.Fill(dtNotes);
                                    Modal.CybersecurityRisksAssessmentListModal = dtNotes.ToListof<CybersecurityRisksAssessmentListModal>();

                                    SyncList.Add(Modal);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("GetCRAListUnsyncedLocalData Error in loop: CRAId : " + item.CRAId + " " + ex.Message);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCRAListUnsyncedLocalData " + ex.Message);
            }
            return SyncList;
        }
        #region Insert/Update
        public bool InsertCybersecurityRisksAssessmentDataInLocalDB(CybersecurityRisksAssessmentModal Modal)
        {
            bool res = false;
            try
            {
                //Insert Data to Form
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string InsertQury = CRADataInsertQuery();
                        SqlCommand command = new SqlCommand(InsertQury, conn);
                        CRADataInsertCMD(Modal, ref command);
                        conn.Open();
                        command.ExecuteScalar();
                        conn.Close();
                        SaveRiskListDataInLocalDB(Modal.CybersecurityRisksAssessmentListModal, Modal.CybersecurityRisksAssessmentForm.CRAId);                        
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertCybersecurityRisksAssessmentDataInLocalDB: Add Local DB in CybersecurityRisksAssessment table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        private string CRADataInsertQuery()
        {
            string InsertQury = @"INSERT INTO [dbo].[CybersecurityRisksAssessment]
                                ([CRAId],[ShipName],[ShipCode],[CreatedBy],[CreatedDate],[IsSynced])		                       
                                VALUES(@CRAId,@ShipName,@ShipCode,@CreatedBy,@CreatedDate,@IsSynced)";
            return InsertQury;
        }
        private void CRADataInsertCMD(CybersecurityRisksAssessmentModal ParentModal, ref SqlCommand command)
        {
            var Modal = ParentModal.CybersecurityRisksAssessmentForm;
            command.Parameters.Add("@CRAId", SqlDbType.UniqueIdentifier).Value = Modal.CRAId;
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Modal.ShipCode ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy ?? (object)DBNull.Value;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate ?? (object)DBNull.Value;
        }
        public bool UpdateCybersecurityRisksAssessmentDataInLocalDB(CybersecurityRisksAssessmentModal Modal)
        {
            bool res = false;
            try
            {
                //Insert Data to Form
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string InsertQury = CRADataUpdateQuery();
                        SqlCommand command = new SqlCommand(InsertQury, conn);
                        CRADataUpdateCMD(Modal, ref command);
                        conn.Open();
                        command.ExecuteScalar();
                        conn.Close();
                                              
                        string UpdateQury1 = string.Empty;
                        UpdateQury1 = "Delete from " + AppStatic.CybersecurityRisksAssessmentList + " WHERE CRAId = '" + Modal.CybersecurityRisksAssessmentForm.CRAId + "'";
                        SqlConnection connection1 = new SqlConnection(connetionString);
                        SqlCommand command1 = new SqlCommand(UpdateQury1, connection1);
                        connection1.Open();
                        command1.ExecuteNonQuery();
                        connection1.Close();

                        if (Modal.CybersecurityRisksAssessmentForm.CRAId != Guid.Empty)
                        {
                            SaveRiskListDataInLocalDB(Modal.CybersecurityRisksAssessmentListModal, Modal.CybersecurityRisksAssessmentForm.CRAId);
                            res = true;
                        }
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCybersecurityRisksAssessmentDataInLocalDB: Add Local DB in CybersecurityRisksAssessment table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        private string CRADataUpdateQuery()
        {
            string UpdateQury = @"Update [CybersecurityRisksAssessment]
                                SET [IsSynced]=@IsSynced,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate 		                       
                                Where CRAId=@CRAId AND ShipCode=@ShipCode;";
            return UpdateQury;
        }
        private void CRADataUpdateCMD(CybersecurityRisksAssessmentModal ParentModal, ref SqlCommand command)
        {
            var Modal = ParentModal.CybersecurityRisksAssessmentForm;
            command.Parameters.Add("@CRAId", SqlDbType.UniqueIdentifier).Value = Modal.CRAId;
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Modal.ShipCode ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName ?? (object)DBNull.Value;
            command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = Modal.UpdatedBy ?? (object)DBNull.Value;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced ?? (object)DBNull.Value;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate ?? (object)DBNull.Value;
        }
        private bool SaveRiskListDataInLocalDB(List<CybersecurityRisksAssessmentListModal> Records, Guid CRAId)
        {
            bool res = false;
            try
            {
                if (Records != null && Records.Count > 0 && CRAId != Guid.Empty)
                {
                    foreach (var item in Records)
                    {
                        item.CRAId = CRAId;
                        item.CRALId = Guid.NewGuid();
                    }
                    bool isTableExist = Utility.CheckTableExist(AppStatic.CybersecurityRisksAssessmentList);
                    if (isTableExist)
                    {
                        ServerConnectModal dbConnModal = Utility.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(Records);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.CybersecurityRisksAssessmentList;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in CybersecurityRisksAssessmentList table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        #endregion
    }
}
