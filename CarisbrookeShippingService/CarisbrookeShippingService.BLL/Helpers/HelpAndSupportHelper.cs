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
    // RDBJ 01/01/2022 Added this class
    public class HelpAndSupportHelper
    {
        public static string connectionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
        public static SqlConnection connection = new SqlConnection(connectionString);

        #region HelpAndSupport Local To AWSServer

        public void StartHelpAndSupportSyncLocalToAWSServer()
        {
            List<HelpAndSupport> UnSyncList = GetHelpAndSupportUnsyncedData();
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for HelpAndSupport Data is about " + UnSyncList.Count);
                List<string> SuccessIds = SendHelpAndSupportDataToRemote(UnSyncList);
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                {
                    UpdateLocalHelpAndSupportDataStatus(SuccessIds);
                    LogHelper.writelog("HelpAndSupport Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                {
                    UpdateLocalHelpAndSupportDataStatus(SuccessIds);
                    LogHelper.writelog("Some HelpAndSupport Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("HelpAndSupport Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("HelpAndSupport data Synced from local to server already done.");
            }
        }
        public List<HelpAndSupport> GetHelpAndSupportUnsyncedData()
        {
            List<HelpAndSupport> SyncList = new List<HelpAndSupport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.HelpAndSupport + " WHERE ISNULL(IsSynced, 0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<HelpAndSupport>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetHelpAndSupportUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<string> SendHelpAndSupportDataToRemote(List<HelpAndSupport> UnSyncList)
        {
            CloudLocalHelpAndSupportSynch _helper = new CloudLocalHelpAndSupportSynch();
            List<string> SuccessIds = new List<string>(); 
            foreach (var item in UnSyncList)
            {
                string localDBHelpAndSupportID = Convert.ToString(item.Id);
                
                APIResponse res = _helper.SendHelpAndSupportDataLocalToRemote(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBHelpAndSupportID);
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalHelpAndSupportDataStatus(List<string> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds.Select(x => string.Format("'{0}'", x)));
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        string Query = "UPDATE " + AppStatic.HelpAndSupport + " SET [IsSynced] = 1 WHERE Id in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalHelpAndSupportDataStatus : " + ex.Message);
            }
        }

        #endregion

        #region HelpAndSupport AWSServer To Local
        public void StartHelpAndSupportSyncAWSServerToLocal()
        {
            List<HelpAndSupport> CloudList = GetHelpAndSupportDataFromCloud();
            if (CloudList != null && CloudList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for HelpAndSupport Data is about " + CloudList.Count + "");
                List<string> SuccessIds = SendHelpAndSupportDataToLocal(CloudList);
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == CloudList.Count)
                {
                    UpdateCloudHelpAndSupportSynchStatus(SuccessIds);
                    LogHelper.writelog("HelpAndSupport Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < CloudList.Count)
                {
                    //UpdateLocalGIRFormsStatus(SuccessIds);
                    LogHelper.writelog("Some HelpAndSupport Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("HelpAndSupport Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("HelpAndSupport Data Synced already done.");
            }
        }

        public List<HelpAndSupport> GetHelpAndSupportDataFromCloud()
        {
            List<HelpAndSupport> ListFromCloud = new List<HelpAndSupport>();
            try
            {
                APIHelper _helper = new APIHelper();
                ListFromCloud = _helper.GetHelpAndSupportList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetHelpAndSupportUnsyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return ListFromCloud;
        }

        public List<HelpAndSupport> GetHelpAndSupportDataFromLocal()
        {
            List<HelpAndSupport> ListFromLocal = new List<HelpAndSupport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT [Id], [ModifiedDateTime] FROM " + AppStatic.HelpAndSupport, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            ListFromLocal = dt.ToListof<HelpAndSupport>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetHelpAndSupportDataFromLocal " + ex.Message);
            }
            return ListFromLocal;
        }

        public List<string> SendHelpAndSupportDataToLocal(List<HelpAndSupport> CloudHelpAndSupportList)
        {
            List<string> SuccessIds = new List<string>();
            List<HelpAndSupport> lstLocalHelpAndSupports = GetHelpAndSupportDataFromLocal();
            foreach (var item in CloudHelpAndSupportList)
            {
                string localDBGIRUniqueFormID = Convert.ToString(item.Id);
                APIResponse res = InsertOrUpdateHelpAndSupportInLocalDB(lstLocalHelpAndSupports, item);

                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBGIRUniqueFormID);
                }
            }
            return SuccessIds;
        }

        public APIResponse InsertOrUpdateHelpAndSupportInLocalDB(List<HelpAndSupport> LocalHelpAndSupportList, HelpAndSupport Modal)
        {
            APIResponse response = new APIResponse();
            bool NeedToUpdate = false;
            string InsertOrUpdateQuery = string.Empty;
            try
            {
                var dbModal = LocalHelpAndSupportList.Where(x => x.Id == Modal.Id).FirstOrDefault();

                if (dbModal == null)
                {
                    // Insert
                    InsertOrUpdateQuery = HelpAndSupport_InsertQuery();
                }
                else
                {
                    // RDBJ 01/01/2022 if Server copy latest then it will update
                    if ((Modal.ModifiedDateTime > dbModal.ModifiedDateTime) 
                        || dbModal.ModifiedDateTime == null
                        )
                    {
                        // Update
                        NeedToUpdate = true;
                        InsertOrUpdateQuery = HelpAndSupport_UpdateQuery();
                    }
                }

                if (!string.IsNullOrEmpty(InsertOrUpdateQuery))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(InsertOrUpdateQuery, connection);
                    HelpAndSupportInsertOrUpdate_CMD(Modal, ref command);

                    if (NeedToUpdate)
                        command.ExecuteNonQuery();
                    else
                        command.ExecuteScalar();

                    connection.Close();
                }

                response.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                response.result = AppStatic.ERROR;
                LogHelper.writelog("InsertOrUpdateHelpAndSupportInLocalDB Error : " + ex.Message.ToString());
            }
            finally
            {
                IfConnectionOpenThenCloseIt(connection);
            }
            return response;
        }

        public void UpdateCloudHelpAndSupportSynchStatus(List<string> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds);
                APIHelper _helper = new APIHelper();
                _helper.UpdateCloudHelpAndSupportSynchStatus(IdsStr);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCloudHelpAndSupportSynchStatus : " + ex.Message);
            }
        }

        // RDBJ 01/01/2022
        public string HelpAndSupport_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[HelpAndSupport]
                                  ([Id], [ShipId], [Comments], [IsStatus], [Priority], [CreatedBy], [CreatedDateTime], [ModifiedBy], [ModifiedDateTime], [IsSynced], [IsDeleted])
                                  VALUES (@Id,@ShipId,@Comments,@IsStatus,@Priority,@CreatedBy,@CreatedDateTime,@ModifiedBy,@ModifiedDateTime,@IsSynced,@IsDeleted)";
            return InsertQuery;
        }
        // End RDBJ 01/01/2022

        // RDBJ 01/01/2022
        public string HelpAndSupport_UpdateQuery()
        {
            string UpdateQuery = @"UPDATE  [dbo].[HelpAndSupport] SET
                                Comments = @Comments, IsStatus = @IsStatus, Priority = @Priority, ModifiedBy = @ModifiedBy, 
                                ModifiedDateTime = @ModifiedDateTime, IsSynced = @IsSynced, IsDeleted = @IsDeleted
                                WHERE Id = @Id";
            return UpdateQuery;
        }
        // End RDBJ 01/01/2022

        // RDBJ 01/01/2022
        public void HelpAndSupportInsertOrUpdate_CMD(HelpAndSupport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Modal.Id;
            command.Parameters.Add("@ShipId", SqlDbType.NVarChar).Value = Modal.ShipId == null ? DBNull.Value : (object)Modal.ShipId;
            command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = Modal.Comments == null ? DBNull.Value : (object)Modal.Comments;
            command.Parameters.Add("@IsStatus", SqlDbType.Int).Value = Modal.IsStatus;
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority;

            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy == null ? DBNull.Value : (object)Modal.CreatedBy;
            command.Parameters.Add("@CreatedDateTime", SqlDbType.DateTime).Value = Modal.CreatedDateTime;

            command.Parameters.Add("@ModifiedBy", SqlDbType.NVarChar).Value = Modal.ModifiedBy == null ? DBNull.Value : (object)Modal.ModifiedBy;
            command.Parameters.Add("@ModifiedDateTime", SqlDbType.DateTime).Value = Modal.ModifiedDateTime == null ? DBNull.Value : (object)Modal.ModifiedDateTime;

            command.Parameters.Add("@IsSynced", SqlDbType.Int).Value = 1;
            command.Parameters.Add("@IsDeleted", SqlDbType.Int).Value = Modal.IsDeleted;
        }
        // End RDBJ 01/01/2022



        #endregion

        #region Common Function
        // RDBJ 01/01/2022
        public static void IfConnectionOpenThenCloseIt(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        // End RDBJ 01/01/2022

        #endregion
    }
}
