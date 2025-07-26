using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Helpers
{
    // RDBJ 12/31/2021 Added this class
    public class HelpAndSupportHelper
    {
        public static string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
        public static SqlConnection connection = new SqlConnection(connetionString);
        public static SqlCommand command;
        public static DataTable dt;
        public static SqlDataAdapter sqlAdp;
        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();

        // RDBJ 12/31/2021
        public List<HelpAndSupport> GetHelpAndSupportsList()
        {
            List<HelpAndSupport> lstHelpAndSupports = new List<HelpAndSupport>();
            try
            {
                if (!string.IsNullOrEmpty(connetionString))
                {
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string strQuery = "SELECT * FROM " + AppStatic.HelpAndSupport + " WHERE [IsDeleted] = 0 ORDER BY [CreatedDateTime] DESC";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(strQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                lstHelpAndSupports = dt.ToListof<HelpAndSupport>().ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lstHelpAndSupports = null;
                LogHelper.writelog("GetHelpAndSupportsList Error : " + ex.Message);
            }
            finally
            {
                IfConnectionOpenThenCloseIt(connection);
            }
            return lstHelpAndSupports;
        }
        // End RDBJ 12/31/2021

        // RDBJ 12/30/2021
        public APIResponse InsertOrUpdateHelpAndSupport(HelpAndSupport Modal, bool NeedToUpdate)
        {
            APIResponse response = new APIResponse();
            try
            {
                if (Modal != null)
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.HelpAndSupport);
                    if (isTableExist)
                    {
                        string InsertOrUpdateQuery = string.Empty;
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            if (NeedToUpdate)
                                InsertOrUpdateQuery = HelpAndSupport_UpdateQuery();
                            else
                                InsertOrUpdateQuery = HelpAndSupport_InsertQuery();

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
                    else
                    {
                        response.result = AppStatic.ERROR;
                        response.msg = "Help & Support table not found in Database, Please wait for sometime untill the Service created try again after sometime! Thank You!";
                    }
                }

            }
            catch (Exception ex)
            {
                response.result = AppStatic.ERROR;
                LogHelper.writelog("InsertOrUpdateHelpAndSupport Error : " + ex.Message);
            }
            finally
            {
                IfConnectionOpenThenCloseIt(connection);
            }
            return response;
        }
        // End RDBJ 12/30/2021

        // RDBJ 12/31/2021
        public string HelpAndSupport_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[HelpAndSupport]
                                  ([Id], [ShipId], [Comments], [IsStatus], [Priority], [CreatedBy], [CreatedDateTime], [ModifiedBy], [ModifiedDateTime], [IsSynced], [IsDeleted])
                                  VALUES (@Id,@ShipId,@Comments,@IsStatus,@Priority,@CreatedBy,@CreatedDateTime,@ModifiedBy,@ModifiedDateTime,@IsSynced,@IsDeleted)";
            return InsertQuery;
        }
        // End RDBJ 12/31/2021

        // RDBJ 12/31/2021
        public string HelpAndSupport_UpdateQuery()
        {
            string UpdateQuery = @"UPDATE  [dbo].[HelpAndSupport] SET
                                Comments = @Comments, IsStatus = @IsStatus, Priority = @Priority, ModifiedBy = @ModifiedBy, 
                                ModifiedDateTime = @ModifiedDateTime, IsSynced = @IsSynced, IsDeleted = @IsDeleted
                                WHERE Id = @Id";
            return UpdateQuery;
        }
        // End RDBJ 12/31/2021

        // RDBJ 12/31/2021
        public void HelpAndSupportInsertOrUpdate_CMD(HelpAndSupport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Modal.Id;
            command.Parameters.Add("@ShipId", SqlDbType.NVarChar).Value = Modal.ShipId == null ? DBNull.Value : (object)Modal.ShipId;
            command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = Modal.Comments == null ? DBNull.Value : (object)Modal.Comments;
            command.Parameters.Add("@IsStatus", SqlDbType.Int).Value = Modal.IsStatus; 
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority;

            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy == null ? DBNull.Value : (object)Modal.CreatedBy;
            command.Parameters.Add("@CreatedDateTime", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow();

            command.Parameters.Add("@ModifiedBy", SqlDbType.NVarChar).Value = Modal.ModifiedBy == null ? DBNull.Value : (object)Modal.ModifiedBy;
            command.Parameters.Add("@ModifiedDateTime", SqlDbType.DateTime).Value = Modal.ModifiedDateTime == null ? DBNull.Value : (object)Modal.ModifiedDateTime;

            command.Parameters.Add("@IsSynced", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@IsDeleted", SqlDbType.Int).Value = 0;
        }
        // End RDBJ 12/31/2021

        #region Common Function
        // RDBJ 12/31/2021
        public static void IfConnectionOpenThenCloseIt(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        // End RDBJ 12/31/2021
        #endregion
    }
}
