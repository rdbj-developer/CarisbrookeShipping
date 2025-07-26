using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace ShipApplication.BLL.Helpers
{
    public class HoldVentilationRecordHelper
    {
        public bool SaveHVRFormDataInLocalDB(HoldVentilationRecordFormModal Modal, bool synced)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.HoldVentilationRecordForm);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.HoldVentilationRecordForm); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQury = HVRDataInsertQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(InsertQury, connection);
                        HVRDataInsertCMD(Modal, ref command);
                        connection.Open();
                        object resultObj = command.ExecuteScalar();
                        long databaseID = 0;
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out databaseID);
                        }
                        connection.Close();
                        if (databaseID > 0)
                        {
                            SaveHVRSheetDataInLocalDB(Modal.HoldVentilationRecordList, databaseID);
                            res = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in HoldVentilationRecordForm table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string HVRDataInsertQuery()
        {
            string InsertQury = @"INSERT INTO [dbo].[HoldVentilationRecordForm]
                                ([ShipName],[ShipCode],[Cargo],[Quantity],[LoadingPort],[LoadingDate],[DischargingPort],[DischargingDate],[CreatedDate],[IsSynced])
		                        OUTPUT INSERTED.HoldVentilationRecordFormId
                                VALUES(@ShipName,@ShipCode,@Cargo,@Quantity,@LoadingPort,@LoadingDate,@DischargingPort,@DischargingDate,@CreatedDate,@IsSynced)";
            return InsertQury;
        }
        public void HVRDataInsertCMD(HoldVentilationRecordFormModal ParentModal, ref SqlCommand command)
        {
            var Modal = ParentModal.HoldVentilationRecordForm;
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Modal.ShipCode ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName ?? (object)DBNull.Value;
            command.Parameters.Add("@Cargo", SqlDbType.NVarChar).Value = Modal.Cargo ?? (object)DBNull.Value;
            command.Parameters.Add("@Quantity", SqlDbType.Float).Value = Modal.Quantity ?? (object)DBNull.Value;
            command.Parameters.Add("@LoadingPort", SqlDbType.NVarChar).Value = Modal.LoadingPort ?? (object)DBNull.Value;
            command.Parameters.Add("@LoadingDate", SqlDbType.Date).Value = Modal.LoadingDate ?? (object)DBNull.Value;
            command.Parameters.Add("@DischargingPort", SqlDbType.NVarChar).Value = Modal.DischargingPort ?? (object)DBNull.Value;
            command.Parameters.Add("@DischargingDate", SqlDbType.NVarChar).Value = Modal.DischargingDate ?? (object)DBNull.Value;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate ?? (object)DBNull.Value;            
        }

        public bool SaveHVRSheetDataInLocalDB(List<HoldVentilationRecordSheetModal> Records, long HoldVentilationRecordFormId)
        {
            bool res = false;
            try
            {
                if (Records != null && Records.Count > 0 && HoldVentilationRecordFormId > 0)
                {
                    foreach (var item in Records)
                    {
                        item.HoldVentilationRecordFormId = HoldVentilationRecordFormId;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.HoldVentilationRecordSheet);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.HoldVentilationRecordSheet); }
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

                                bulkCopy.DestinationTableName = AppStatic.HoldVentilationRecordSheet;
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
                LogHelper.writelog("Add Local DB in HoldVentilationRecordSheet table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
    }
}
