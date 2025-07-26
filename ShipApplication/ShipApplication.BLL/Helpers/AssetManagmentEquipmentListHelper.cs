using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ShipApplication.BLL.Helpers
{
    public class AssetManagmentEquipmentListHelper
    {
        #region Check Schema
        public void CreateAssetTables()
        {
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentList);
                if (!isTableExist) { LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentList); }

                isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentOTList);
                if (!isTableExist) { LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentOTList); }
                 
                isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentITList);
                if (!isTableExist) { LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentITList); }

                isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentSoftwareAssets);
                if (!isTableExist) { LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentSoftwareAssets); }



            }
            catch (Exception ex)
            {
                LogHelper.writelog("CreateAssetTables : " + ex.Message);
            }
        }
        #endregion

        #region Insert Data in DB
        public bool SaveAssetManagmentEquipmentDataInLocalDB(AssetManagmentEquipmentListModal Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentList);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentList); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        //Insert Data to Form
                        if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                        {
                            Guid databaseID = Guid.NewGuid();
                            Modal.AssetManagmentEquipmentListForm.AMEId = databaseID;
                            string InsertQury = AMEDataInsertQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            AMEDataInsertCMD(Modal, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();
                            if (databaseID != Guid.Empty)
                            {
                                SaveOTDataInLocalDB(Modal.AssetManagmentEquipmentOTListModel, databaseID);
                                SaveITDataInLocalDB(Modal.AssetManagmentEquipmentITListModel, databaseID);
                                SaveSoftwareAssetsDataInLocalDB(Modal.AssetManagmentEquipmentSoftwareAssetsModel, databaseID);
                                res = true;
                            }
                        }
                        else //Update Data to Form
                        {
                            Guid databaseID = Modal.AssetManagmentEquipmentListForm.AMEId;
                            //Modal.AssetManagmentEquipmentListForm.AMEId = databaseID;
                            string InsertQury = AMEDataUpdateQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            AMEDataUpdateCMD(Modal, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();

                            string UpdateQury1 = string.Empty;
                            UpdateQury1 = "Delete from " + AppStatic.AssetManagmentEquipmentOTList + " WHERE AMEId = '" + databaseID + "'";
                            SqlConnection connection1 = new SqlConnection(connetionString);
                            SqlCommand command1 = new SqlCommand(UpdateQury1, connection1);
                            connection1.Open();
                            command1.ExecuteNonQuery();
                            connection1.Close();

                            string UpdateQury2 = string.Empty;
                            UpdateQury2 = "Delete from " + AppStatic.AssetManagmentEquipmentITList + " WHERE AMEId = '" + databaseID + "'";
                            SqlConnection connection2 = new SqlConnection(connetionString);
                            SqlCommand command2 = new SqlCommand(UpdateQury2, connection2);
                            connection2.Open();
                            command2.ExecuteNonQuery();
                            connection2.Close();


                            try
                            {
                                UpdateQury2 = "Delete from " + AppStatic.AssetManagmentEquipmentSoftwareAssets + " WHERE AMEId = '" + databaseID + "'";
                                command2 = new SqlCommand(UpdateQury2, connection2);
                                connection2.Open();
                                command2.ExecuteNonQuery();
                                connection2.Close();
                            }
                            catch (Exception)
                            {

                            }


                            if (databaseID != Guid.Empty)
                            {
                                SaveOTDataInLocalDB(Modal.AssetManagmentEquipmentOTListModel, databaseID);
                                SaveITDataInLocalDB(Modal.AssetManagmentEquipmentITListModel, databaseID);
                                SaveSoftwareAssetsDataInLocalDB(Modal.AssetManagmentEquipmentSoftwareAssetsModel, databaseID);
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in AssetManagmentEquipmentList table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        private string AMEDataInsertQuery()
        {
            string InsertQury = @"INSERT INTO [dbo].[AssetManagmentEquipmentList]
                                ([AMEId],[ShipName],[ShipCode],[CreatedBy],[CreatedDate],[IsSynced])		                       
                                VALUES(@AMEId,@ShipName,@ShipCode,@CreatedBy,@CreatedDate,@IsSynced)";
            return InsertQury;
        }
        private void AMEDataInsertCMD(AssetManagmentEquipmentListModal ParentModal, ref SqlCommand command)
        {
            var Modal = ParentModal.AssetManagmentEquipmentListForm;
            command.Parameters.Add("@AMEId", SqlDbType.UniqueIdentifier).Value = Modal.AMEId;
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Modal.ShipCode ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy ?? (object)DBNull.Value;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate ?? (object)DBNull.Value;
        }
        private string AMEDataUpdateQuery()
        {
            string UpdateQury = @"Update [AssetManagmentEquipmentList]
                                SET [IsSynced]=@IsSynced,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate 		                       
                                Where AMEId=@AMEId AND ShipCode=@ShipCode;";
            return UpdateQury;
        }
        private void AMEDataUpdateCMD(AssetManagmentEquipmentListModal ParentModal, ref SqlCommand command)
        {
            var Modal = ParentModal.AssetManagmentEquipmentListForm;
            command.Parameters.Add("@AMEId", SqlDbType.UniqueIdentifier).Value = Modal.AMEId;
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Modal.ShipCode ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName ?? (object)DBNull.Value;
            command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = Modal.UpdatedBy ?? (object)DBNull.Value;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced ?? (object)DBNull.Value;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate ?? (object)DBNull.Value;
        }
        private bool SaveOTDataInLocalDB(List<AssetManagmentEquipmentOTListModel> Records, Guid AMEId)
        {
            bool res = false;
            try
            {
                if (Records != null && Records.Count > 0 && AMEId != Guid.Empty)
                {
                    bool isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTHardwareId");
                    if (!isColumnHarwareIdExist)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTHardwareId nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTOwner");
                    if (!isColumnHarwareIdExist)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTOwner nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTPersonResponsible");
                    if (!isColumnHarwareIdExist)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTPersonResponsible nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTCriticality");
                    if (!isColumnHarwareIdExist)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTCriticality nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTOperatingSystem");
                    if (!isColumnHarwareIdExist)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTOperatingSystem nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTOSPatchVersion");
                    if (!isColumnHarwareIdExist)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTOSPatchVersion nvarchar(250) ");

                    Records = Records.Where(x => !string.IsNullOrWhiteSpace(x.OTEquipment)).ToList();

                    foreach (var item in Records)
                    {
                        item.AMEId = AMEId;
                        item.OTId = Guid.NewGuid();
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentOTList);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentOTList); }
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

                                bulkCopy.DestinationTableName = AppStatic.AssetManagmentEquipmentOTList;
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
                LogHelper.writelog("Add Local DB in AssetManagmentEquipmentOTList table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        private bool SaveITDataInLocalDB(List<AssetManagmentEquipmentITListModel> Records, Guid AMEId)
        {
            bool res = false;
            try
            {
                if (Records != null && Records.Count > 0 && AMEId != Guid.Empty)
                {
                    bool isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITHardwareId");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITHardwareId nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITOwner");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITOwner nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITPersonResponsible");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITPersonResponsible nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITCriticality");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITCriticality nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITOperatingSystem");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITOperatingSystem nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITOSPatchVersion");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITOSPatchVersion nvarchar(250) ");

                    Records = Records.Where(x => !string.IsNullOrWhiteSpace(x.ITEquipment)).ToList();
                    foreach (var item in Records)
                    {
                        item.AMEId = AMEId;
                        item.ITId = Guid.NewGuid();
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentITList);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentITList); }
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

                                bulkCopy.DestinationTableName = AppStatic.AssetManagmentEquipmentITList;
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
                LogHelper.writelog("Add Local DB in AssetManagmentEquipmentITList table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        private bool SaveSoftwareAssetsDataInLocalDB(List<AssetManagmentEquipmentSoftwareAssetsModel> Records, Guid AMEId)
        {
            bool res = false;
            try
            {
                if (Records != null && Records.Count > 0 && AMEId != Guid.Empty)
                {
                    bool isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SASoftwareId");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SASoftwareId nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAOwner");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAOwner nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAPersonResponsible");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAPersonResponsible nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SACriticality");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SACriticality nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAOperatingSystem");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAOperatingSystem nvarchar(500) ");

                    isColumnHarwareIdExist = LocalDBHelper.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAOSPatchVersion");
                    if (!isColumnHarwareIdExist) LocalDBHelper.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAOSPatchVersion nvarchar(250) ");

                    Records = Records.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
                    foreach (var item in Records)
                    {
                        item.AMEId = AMEId;
                        item.SAId = Guid.NewGuid();
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AssetManagmentEquipmentSoftwareAssets);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AssetManagmentEquipmentSoftwareAssets); }
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

                                bulkCopy.DestinationTableName = AppStatic.AssetManagmentEquipmentSoftwareAssets;
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
                LogHelper.writelog("Add Local DB in AssetManagmentEquipmentSoftwareAssets table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        #endregion

        #region Get Data From DB
        public AssetManagmentEquipmentListModal GetAssetManagmentEquipmentData_LocalDB(string ShipCode)
        {
            AssetManagmentEquipmentListModal dbModal = new AssetManagmentEquipmentListModal();
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
                            string selectQuery = "SELECT * FROM " + AppStatic.AssetManagmentEquipmentList + " WHERE ShipCode ='" + ShipCode + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                var SyncList = dt.ToListof<AssetManagmentEquipmentListForm>();
                                dbModal.AssetManagmentEquipmentListForm = SyncList.FirstOrDefault();

                                DataTable dtHazaedList = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AssetManagmentEquipmentOTList + " WHERE AMEID = '" + dbModal.AssetManagmentEquipmentListForm.AMEId + "'", conn);
                                sqlAdp.Fill(dtHazaedList);
                                dbModal.AssetManagmentEquipmentOTListModel = dtHazaedList.ToListof<AssetManagmentEquipmentOTListModel>();

                                DataTable dtReviewerList = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AssetManagmentEquipmentITList + " WHERE AMEID = '" + dbModal.AssetManagmentEquipmentListForm.AMEId + "'", conn);
                                sqlAdp.Fill(dtReviewerList);
                                dbModal.AssetManagmentEquipmentITListModel = dtReviewerList.ToListof<AssetManagmentEquipmentITListModel>();

                                DataTable dtSoftwarerList = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AssetManagmentEquipmentSoftwareAssets + " WHERE AMEID = '" + dbModal.AssetManagmentEquipmentListForm.AMEId + "'", conn);
                                sqlAdp.Fill(dtSoftwarerList);
                                dbModal.AssetManagmentEquipmentSoftwareAssetsModel = dtSoftwarerList.ToListof<AssetManagmentEquipmentSoftwareAssetsModel>();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentData_LocalDB Error : " + ex.Message);
            }
            return dbModal;
        }
        public List<string> GetAssetManagmentHardwareId_LocalDB(string ShipCode)
        {
            List<string> dbModal = new List<string>();
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
                            string selectQuery = "SELECT * FROM " + AppStatic.AssetManagmentEquipmentList + " WHERE ShipCode ='" + ShipCode + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                var asset = dt.ToListof<AssetManagmentEquipmentListForm>().FirstOrDefault();
                                if (asset != null)
                                {
                                    DataTable dtHardwareList = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT  OTHardwareId + '|' + OTEquipment as HardwareId  FROM " + AppStatic.AssetManagmentEquipmentOTList + " WHERE AMEID = '" + asset.AMEId + "' AND OTHardwareId IS NOT NULL " +
                                        " UNION SELECT  ITHardwareId + '|' + ITEquipment as HardwareId  FROM " + AppStatic.AssetManagmentEquipmentITList + " WHERE AMEID = '" + asset.AMEId + "' AND ITHardwareId IS NOT NULL " +
                                        " UNION SELECT SASoftwareId + '|' + Name as HardwareId FROM " + AppStatic.AssetManagmentEquipmentSoftwareAssets + " WHERE AMEID = '" + asset.AMEId + "' AND SASoftwareId IS NOT NULL", conn);
                                    sqlAdp.Fill(dtHardwareList);
                                    dbModal  = dtHardwareList.AsEnumerable().Select(r => r.Field<string>("HardwareId")).ToList();
                                }                                
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentHardwareId_LocalDB Error : " + ex.Message);
            }
            return dbModal;
        }
        #endregion
    }
}

