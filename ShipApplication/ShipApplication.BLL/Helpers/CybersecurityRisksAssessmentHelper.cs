using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ShipApplication.BLL.Helpers
{
    public class CybersecurityRisksAssessmentHelper
    {
        #region Insert Data in DB
        public bool SaveCybersecurityRisksAssessmentDataInLocalDB(CybersecurityRisksAssessmentModal Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.CybersecurityRisksAssessment);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.CybersecurityRisksAssessment); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        //Insert Data to Form
                        if (Modal.CybersecurityRisksAssessmentForm.CRAId == Guid.Empty)
                        {
                            Guid databaseID = Guid.NewGuid();
                            Modal.CybersecurityRisksAssessmentForm.CRAId = databaseID;
                            string InsertQury = CRADataInsertQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            CRADataInsertCMD(Modal, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();
                            if (databaseID != Guid.Empty)
                            {
                                SaveRiskListDataInLocalDB(Modal.CybersecurityRisksAssessmentListModal, databaseID);                                
                                res = true;
                            }
                        }
                        else //Update Data to Form
                        {
                            Guid databaseID = Modal.CybersecurityRisksAssessmentForm.CRAId;
                            string InsertQury = CRADataUpdateQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            CRADataUpdateCMD(Modal, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();

                            string UpdateQury1 = string.Empty;
                            UpdateQury1 = "Delete from " + AppStatic.CybersecurityRisksAssessmentList + " WHERE CRAId = '" + databaseID + "'";
                            SqlConnection connection1 = new SqlConnection(connetionString);
                            SqlCommand command1 = new SqlCommand(UpdateQury1, connection1);
                            connection1.Open();
                            command1.ExecuteNonQuery();
                            connection1.Close();

                            if (databaseID != Guid.Empty)
                            {
                                SaveRiskListDataInLocalDB(Modal.CybersecurityRisksAssessmentListModal, databaseID);                               
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in CybersecurityRisksAssessment table Error : " + ex.Message.ToString());
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
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.CybersecurityRisksAssessmentList);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.CybersecurityRisksAssessmentList); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
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

        #region Get Data From DB
        public CybersecurityRisksAssessmentModal GetCybersecurityRisksAssessmentData_LocalDB(string ShipCode)
        {
            CybersecurityRisksAssessmentModal dbModal = new CybersecurityRisksAssessmentModal();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string selectQuery = "SELECT * FROM " + AppStatic.CybersecurityRisksAssessment + " WHERE ShipCode ='" + ShipCode + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                var SyncList = dt.ToListof<CybersecurityRisksAssessmentForm>();
                                dbModal.CybersecurityRisksAssessmentForm = SyncList.FirstOrDefault();

                                DataTable dtHazaedList = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.CybersecurityRisksAssessmentList + " WHERE CRAId = '" + dbModal.CybersecurityRisksAssessmentForm.CRAId + "'  order by RiskId ", conn);
                                sqlAdp.Fill(dtHazaedList);
                                dbModal.CybersecurityRisksAssessmentListModal = dtHazaedList.ToListof<CybersecurityRisksAssessmentListModal>();
                                dbModal.CybersecurityRisksAssessmentListModal = dbModal.CybersecurityRisksAssessmentListModal
                                    . Where(x => !string.IsNullOrEmpty(x.InherentImpactScore) && !string.IsNullOrEmpty(x.ResidualRiskScore))    // JSL 06/07/2022
                                    .OrderByDescending(x => x.RiskId == null ? 0 : 1).ToList();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCybersecurityRisksAssessmentData_LocalDB Error : " + ex.Message);
            }
            return dbModal;
        }
        #endregion
    }
}
