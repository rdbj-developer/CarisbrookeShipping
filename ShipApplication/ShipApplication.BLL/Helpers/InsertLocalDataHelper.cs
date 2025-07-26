using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ShipApplication.BLL.Helpers
{
    public static class InsertLocalDataHelper
    {
        public static bool InsertUsersData()
        {
            bool res = false;
            try
            {
                List<UserModalAWS> UsersList = LocalDBHelper.ReadDBUsersJson();
                if (UsersList != null && UsersList.Count > 0)
                {
                    foreach (var item in UsersList)
                    {
                        item.UID = 0;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.Users);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.Users); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(UsersList);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.Users;
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
                LogHelper.writelog("InsertUsersData : " + ex.Message);
            }
            return res;
        }

        public static bool InsertUserDataFromAWS()
        {
            bool res = false;
            try
            {
                APIUsersHelper _helper = new APIUsersHelper();
                List<UserModalAWS> UsersList = null;
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                    UsersList = _helper.GetAWSUsers();
                else
                    UsersList = LocalDBHelper.ReadDBUsersJson();
                if (UsersList != null && UsersList.Count > 0)
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.Users);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.Users); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(UsersList);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                using (SqlCommand cmd = new SqlCommand("usp_InsertUpdateAWSUsers"))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.AddWithValue("@tblUsers", dt);
                                    connection.Open();
                                    cmd.ExecuteNonQuery();
                                    connection.Close();
                                    res = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (res)
                    LocalDBHelper.DeleteUsersJson();
            }
            return res;
        }
        public static void DeleteUsersData()
        {


        }

        //RDBJ 09/16/2021
        public static bool InsertUpdateShipsDataFromAWS()
        {
            bool res = false;
            try
            {
                APIUsersHelper _helper = new APIUsersHelper();
                List<CSShipsModalAWS> CSShipsList = null;
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                    CSShipsList = _helper.GetAWSShipsDetails();
                //else
                    //UsersList = LocalDBHelper.ReadDBUsersJson();
                if (CSShipsList != null && CSShipsList.Count > 0)
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.CSShips);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.CSShips); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                           

                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(CSShipsList);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                // RDBJ 03/26/2022
                                SqlCommand command = new SqlCommand();
                                command = new SqlCommand(TableQueryGenerator.CSShipsTableTypeQuery(), connection);
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();

                                using (SqlCommand cmd = new SqlCommand("WITH cte AS (SELECT Code, Name, ROW_NUMBER() OVER(PARTITION BY Code, Name ORDER BY Code, Name) row_num FROM CSShips) DELETE FROM cte WHERE row_num > 1; "))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Connection = connection;
                                    connection.Open();
                                    cmd.ExecuteNonQuery();
                                    connection.Close();
                                    res = true;
                                }
                                // End RDBJ 03/26/2022

                                using (SqlCommand cmd = new SqlCommand("usp_InsertUpdateAWSCSShips"))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.AddWithValue("@tblCSShips", dt);
                                    connection.Open();
                                    cmd.ExecuteNonQuery();
                                    connection.Close();
                                    res = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertUpdateShipsDataFromAWS : " + ex.Message);
            }
            finally
            {
                
            }
            return res;
        }
        //End RDBJ 09/16/2021
    }
}
