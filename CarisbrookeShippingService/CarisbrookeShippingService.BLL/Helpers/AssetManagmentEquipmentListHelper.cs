using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class AssetManagmentEquipmentListHelper
    {
        public void StartAMESync()
        {
            try
            {
                List<AssetManagmentEquipmentListModal> UnSyncList = GetAMEListUnsyncedLocalData();
                if (UnSyncList != null && UnSyncList.Count > 0)
                {
                    LogHelper.writelog("UnSyncList count for AssetManagmentEquipmentList Data is about " + UnSyncList.Count + "");
                    List<Guid> SuccessIds = SendAMEDataToRemote(UnSyncList);
                    if (SuccessIds != null && SuccessIds.Count > 0)
                    {
                        UpdateLocalAMEDataStatus(SuccessIds);
                        if (SuccessIds.Count == UnSyncList.Count)
                            LogHelper.writelog("AssetManagmentEquipmentList Data Sync process done.");
                        if (SuccessIds.Count < UnSyncList.Count)
                            LogHelper.writelog("Some AssetManagmentEquipmentList Data Synced and some remaining...");
                    }
                    else
                        LogHelper.writelog("AssetManagmentEquipmentList Data Synced Not done.");
                }
                else
                {
                    List<Guid> SuccessIds = SyncRemoteDataToLocal();
                    if (SuccessIds != null && SuccessIds.Count > 0)
                    {
                        LogHelper.writelog("AssetManagmentEquipmentList Data Sync from Remote process done.");
                    }                    
                }
                //Check to Sync Remote Changes                
                LogHelper.writelog("AssetManagmentEquipmentList Data Synced already done..");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StartAMESync " + ex.Message);
            }
        }
        public List<AssetManagmentEquipmentListModal> GetAMEListUnsyncedLocalData(bool isGetAllData = false)
        {
            List<AssetManagmentEquipmentListModal> SyncList = new List<AssetManagmentEquipmentListModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        string query = "SELECT * FROM " + AppStatic.AssetManagmentEquipmentList;
                        if (!isGetAllData)
                            query += " WHERE ISNULL(IsSynced,0) = 0";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<AssetManagmentEquipmentListForm> HVRSyncList = dt.ToListof<AssetManagmentEquipmentListForm>();
                            foreach (var item in HVRSyncList)
                            {
                                try
                                {
                                    AssetManagmentEquipmentListModal Modal = new AssetManagmentEquipmentListModal();
                                    Modal.AssetManagmentEquipmentListForm = item;

                                    DataTable dtNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AssetManagmentEquipmentOTList + " WHERE AMEId = '" + item.AMEId + "'", conn);
                                    sqlAdp.Fill(dtNotes);
                                    Modal.AssetManagmentEquipmentOTListModel = dtNotes.ToListof<AssetManagmentEquipmentOTListModel>();

                                    dtNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AssetManagmentEquipmentITList + " WHERE AMEId = '" + item.AMEId + "'", conn);
                                    sqlAdp.Fill(dtNotes);
                                    Modal.AssetManagmentEquipmentITListModel = dtNotes.ToListof<AssetManagmentEquipmentITListModel>();
                                    
                                    dtNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AssetManagmentEquipmentSoftwareAssets + " WHERE AMEId = '" + item.AMEId + "'", conn);
                                    sqlAdp.Fill(dtNotes);
                                    Modal.AssetManagmentEquipmentSoftwareAssetsModel = dtNotes.ToListof<AssetManagmentEquipmentSoftwareAssetsModel>();
                                    SyncList.Add(Modal);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("GetAMEListUnsyncedData Error in loop: AMEId : " + item.AMEId + " " + ex.Message);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAMEListUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<Guid> SendAMEDataToRemote(List<AssetManagmentEquipmentListModal> UnSyncList)
        {
            List<Guid> SuccessIds = new List<Guid>();
            foreach (var item in UnSyncList)
            {
                try
                {
                    if (item.AssetManagmentEquipmentListForm != null && item.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                    {
                        item.AssetManagmentEquipmentListForm.IsSynced = true;
                        APIHelper _helper = new APIHelper();
                        APIResponse res = _helper.SubmitAssetManagmentEquipmentList(item);
                        if (res != null && res.result == AppStatic.SUCCESS)
                        {
                            SuccessIds.Add(item.AssetManagmentEquipmentListForm.AMEId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("SendAMEDataToRemote Error in loop : " + ex.Message);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalAMEDataStatus(List<Guid> SuccessIds)
        {
            try
            {
                string IdsStr = "'" + String.Join("','", SuccessIds) + "'";
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        string Query = "UPDATE " + AppStatic.AssetManagmentEquipmentList + " SET IsSynced = 1 WHERE AMEId in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalAMEDataStatus : " + ex.Message);
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
                    var unSyncRemotChanges = _helper.GetAssetManagmentEquipmentUnSyncData(objShip.id);
                    if (unSyncRemotChanges != null && unSyncRemotChanges.AssetManagmentEquipmentListForm != null && unSyncRemotChanges.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                    {
                        unSyncRemotChanges.AssetManagmentEquipmentListForm.IsSynced = true;
                        //Check if Entry Exist? If Exist then make entry or else update it
                        var lstLocalData = GetAMEListUnsyncedLocalData(true);
                        if (lstLocalData != null && lstLocalData.Count > 0)
                        {

                            var matchingData = lstLocalData.Where(x => x.AssetManagmentEquipmentListForm.ShipCode == unSyncRemotChanges.AssetManagmentEquipmentListForm.ShipCode).FirstOrDefault();
                            if (matchingData != null)
                            {
                                unSyncRemotChanges.AssetManagmentEquipmentListForm.AMEId = matchingData.AssetManagmentEquipmentListForm.AMEId;
                                //Update Local Entry
                                isSuccess = UpdateAssetManagmentEquipmentDataInLocalDB(unSyncRemotChanges);
                                LogHelper.writelog("Updated from Office DB");
                            }
                            else
                            {
                                //New Entry
                                isSuccess = InsertAssetManagmentEquipmentDataInLocalDB(unSyncRemotChanges);
                                LogHelper.writelog("Inserted from Office DB");
                            }
                        }
                        else
                        {
                            //New Entry
                            isSuccess = InsertAssetManagmentEquipmentDataInLocalDB(unSyncRemotChanges);
                            LogHelper.writelog("Inserted from Office DB");
                        }
                        if (isSuccess)
                        {
                            _helper.UpdateAssetManagmentEquipmentSyncStatus(objShip.id, true);
                            SuccessIds.Add(unSyncRemotChanges.AssetManagmentEquipmentListForm.AMEId);
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
        #region Insert/Update
        public bool InsertAssetManagmentEquipmentDataInLocalDB(AssetManagmentEquipmentListModal Modal)
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
                        string InsertQury = AMEDataInsertQuery();
                        SqlCommand command = new SqlCommand(InsertQury, conn);
                        AMEDataInsertCMD(Modal, ref command);
                        conn.Open();
                        command.ExecuteScalar();
                        conn.Close();
                        SaveOTDataInLocalDB(Modal.AssetManagmentEquipmentOTListModel, Modal.AssetManagmentEquipmentListForm.AMEId);
                        SaveITDataInLocalDB(Modal.AssetManagmentEquipmentITListModel, Modal.AssetManagmentEquipmentListForm.AMEId);
                        SaveSoftwareAssetsDataInLocalDB(Modal.AssetManagmentEquipmentSoftwareAssetsModel, Modal.AssetManagmentEquipmentListForm.AMEId);
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertAssetManagmentEquipmentDataInLocalDB: Add Local DB in AssetManagmentEquipmentList table Error : " + ex.Message.ToString());
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
        public bool UpdateAssetManagmentEquipmentDataInLocalDB(AssetManagmentEquipmentListModal Modal)
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
                        string InsertQury = AMEDataUpdateQuery();
                        SqlCommand command = new SqlCommand(InsertQury, conn);
                        AMEDataUpdateCMD(Modal, ref command);
                        conn.Open();
                        command.ExecuteScalar();
                        conn.Close();

                        string UpdateQury1 = string.Empty;
                        UpdateQury1 = "Delete from " + AppStatic.AssetManagmentEquipmentOTList + " WHERE AMEId = '" + Modal.AssetManagmentEquipmentListForm.AMEId + "'";
                        SqlConnection connection1 = new SqlConnection(connetionString);
                        SqlCommand command1 = new SqlCommand(UpdateQury1, connection1);
                        connection1.Open();
                        command1.ExecuteNonQuery();
                        connection1.Close();

                        string UpdateQury2 = string.Empty;
                        UpdateQury2 = "Delete from " + AppStatic.AssetManagmentEquipmentITList + " WHERE AMEId = '" + Modal.AssetManagmentEquipmentListForm.AMEId + "'";
                        SqlConnection connection2 = new SqlConnection(connetionString);
                        SqlCommand command2 = new SqlCommand(UpdateQury2, connection2);
                        connection2.Open();
                        command2.ExecuteNonQuery();
                        connection2.Close();

                        string UpdateQury3 = string.Empty;
                        UpdateQury3 = "Delete from " + AppStatic.AssetManagmentEquipmentSoftwareAssets + " WHERE AMEId = '" + Modal.AssetManagmentEquipmentListForm.AMEId + "'";
                        SqlConnection connection3 = new SqlConnection(connetionString);
                        SqlCommand command3 = new SqlCommand(UpdateQury3, connection3);
                        connection3.Open();
                        command3.ExecuteNonQuery();
                        connection3.Close();

                        SaveOTDataInLocalDB(Modal.AssetManagmentEquipmentOTListModel, Modal.AssetManagmentEquipmentListForm.AMEId);
                        SaveITDataInLocalDB(Modal.AssetManagmentEquipmentITListModel, Modal.AssetManagmentEquipmentListForm.AMEId);
                        SaveSoftwareAssetsDataInLocalDB(Modal.AssetManagmentEquipmentSoftwareAssetsModel, Modal.AssetManagmentEquipmentListForm.AMEId);
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertAssetManagmentEquipmentDataInLocalDB: Add Local DB in AssetManagmentEquipmentList table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
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
                    bool isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTHardwareId");
                    if (!isColumnHarwareIdExist)
                        Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTHardwareId nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTOwner");
                    if (!isColumnHarwareIdExist)
                        Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTOwner nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTPersonResponsible");
                    if (!isColumnHarwareIdExist)
                        Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTPersonResponsible nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTCriticality");
                    if (!isColumnHarwareIdExist)
                        Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTCriticality nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTOperatingSystem");
                    if (!isColumnHarwareIdExist)
                        Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTOperatingSystem nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentOTList, "OTOSPatchVersion");
                    if (!isColumnHarwareIdExist)
                        Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentOTList ADD OTOSPatchVersion nvarchar(250) ");

                    Records = Records.Where(x => !string.IsNullOrWhiteSpace(x.OTEquipment)).ToList();

                    foreach (var item in Records)
                    {
                        item.AMEId = AMEId;
                        item.OTId = Guid.NewGuid();
                    }
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    DataTable dt = Utility.ToDataTable(Records);
                    using (SqlConnection connection = new SqlConnection(connetionString))
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
            catch (Exception ex)
            {
                LogHelper.writelog("SaveOTDataInLocalDB: Add Local DB in AssetManagmentEquipmentOTList table Error : " + ex.Message.ToString());
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
                    bool isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITHardwareId");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITHardwareId nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITOwner");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITOwner nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITPersonResponsible");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITPersonResponsible nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITCriticality");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITCriticality nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITOperatingSystem");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITOperatingSystem nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentITList, "ITOSPatchVersion");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentITList ADD ITOSPatchVersion nvarchar(250) ");
                    Records = Records.Where(x => !string.IsNullOrWhiteSpace(x.ITEquipment)).ToList();
                    foreach (var item in Records)
                    {
                        item.AMEId = AMEId;
                        item.ITId = Guid.NewGuid();
                    }
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    DataTable dt = Utility.ToDataTable(Records);
                    using (SqlConnection connection = new SqlConnection(connetionString))
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
            catch (Exception ex)
            {
                LogHelper.writelog("SaveITDataInLocalDB: Add Local DB in AssetManagmentEquipmentITList table Error : " + ex.Message.ToString());
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
                    bool isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SASoftwareId");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SASoftwareId nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAOwner");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAOwner nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAPersonResponsible");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAPersonResponsible nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SACriticality");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SACriticality nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAOperatingSystem");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAOperatingSystem nvarchar(500) ");

                    isColumnHarwareIdExist = Utility.CheckTableColumnExist(AppStatic.AssetManagmentEquipmentSoftwareAssets, "SAOSPatchVersion");
                    if (!isColumnHarwareIdExist) Utility.ExecuteQuery("ALTER TABLE AssetManagmentEquipmentSoftwareAssets ADD SAOSPatchVersion nvarchar(250) ");
                    Records = Records.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
                    foreach (var item in Records)
                    {
                        item.AMEId = AMEId;
                        item.SAId = Guid.NewGuid();
                    }
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    DataTable dt = Utility.ToDataTable(Records);
                    using (SqlConnection connection = new SqlConnection(connetionString))
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
            catch (Exception ex)
            {
                LogHelper.writelog("SaveSoftwareAssetsDataInLocalDB: Add Local DB in AssetManagmentEquipmentITList table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        #endregion
    }
}
