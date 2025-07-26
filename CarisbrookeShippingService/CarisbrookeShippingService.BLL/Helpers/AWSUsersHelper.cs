using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class AWSUsersHelper
    {
        public void SyncAwsUsers()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                List<AWSUserModal> AwsUsersList = _helper.GetUsersFromAPI();
                List<AWSUserModal> LocalUsersList = GetLocalUsers();
                int i = 0;
                int j = 0;
                int k = 0;
                if (AwsUsersList != null && AwsUsersList.Count > 0)
                {
                    foreach (var item in AwsUsersList)
                    {
                        try
                        {
                            AWSUserModal localUser = LocalUsersList.Where(x => x.empnre01 == item.empnre01).FirstOrDefault();
                            if (localUser != null)
                            {
                                bool res = UpdateAWSUser(item);
                                if (res)
                                    k++;
                            }
                            else
                            {
                                // Not Exist in local machine - Insert New User
                                bool res = InsertNewAWSUser(item);
                                if (res)
                                    i++;
                                else
                                    j++;
                            }
                        }
                        catch (Exception)
                        {
                            LogHelper.writelog("SyncAwsUsers -> for each loop : Failed to Add/Update AWS Users");
                        }
                    }
                    LogHelper.writelog(i + " New Users Added in Local DB Users Table ");
                    LogHelper.writelog(j + " New Users Failed to Add in Local DB Users Table ");
                    if (k > 0)
                        LogHelper.writelog(k + " Users Updated in Local DB Users Table ");
                }
                else
                {
                    LogHelper.writelog(" No Data retrieved from API");
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        public List<AWSUserModal> GetLocalUsers()
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            List<AWSUserModal> dbUsersList = new List<AWSUserModal>();
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        string selectQuery = "SELECT  * FROM Users";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dbUsersList = dt.ToListof<AWSUserModal>();
                        }
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("Connection is not available For Local DB !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return dbUsersList;
        }
        public bool InsertNewAWSUser(AWSUserModal Modal)
        {
            bool res = false;
            try
            {
                int UID = 0;
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        DataTable dt = new DataTable();
                        string InsertQuery = @"INSERT INTO dbo.Users 
                                            (empnre01,empste01,fstnme01,surnme01,bthcnte01,fnce01,lane01,rsnewe01,sxee01,bthdate01,nate01)
                                                OUTPUT INSERTED.UID
                                                VALUES (@empnre01,@empste01,@fstnme01,@surnme01,@bthcnte01,@fnce01,@lane01,@rsnewe01,@sxee01,@bthdate01,@nate01)";

                        SqlCommand cmd = new SqlCommand(InsertQuery, conn);
                        cmd.Parameters.Add("@empnre01", SqlDbType.NVarChar).Value = Modal.empnre01 == null ? string.Empty : Modal.empnre01;
                        cmd.Parameters.Add("@empste01", SqlDbType.NVarChar).Value = Modal.empste01 == null ? string.Empty : Modal.empste01;
                        cmd.Parameters.Add("@fstnme01", SqlDbType.NVarChar).Value = Modal.fstnme01 == null ? string.Empty : Modal.fstnme01;
                        cmd.Parameters.Add("@surnme01", SqlDbType.NVarChar).Value = Modal.surnme01 == null ? string.Empty : Modal.surnme01;
                        cmd.Parameters.Add("@bthcnte01", SqlDbType.NVarChar).Value = Modal.bthcnte01 == null ? DBNull.Value : (object)Modal.bthcnte01;
                        cmd.Parameters.Add("@fnce01", SqlDbType.NVarChar).Value = Modal.fnce01 == null ? DBNull.Value : (object)Modal.fnce01;
                        cmd.Parameters.Add("@lane01", SqlDbType.NVarChar).Value = Modal.lane01 == null ? DBNull.Value : (object)Modal.lane01;
                        cmd.Parameters.Add("@rsnewe01", SqlDbType.NVarChar).Value = Modal.rsnewe01 == null ? DBNull.Value : (object)Modal.rsnewe01;
                        cmd.Parameters.Add("@sxee01", SqlDbType.NVarChar).Value = Modal.sxee01 == null ? DBNull.Value : (object)Modal.sxee01;
                        cmd.Parameters.Add("@bthdate01", SqlDbType.DateTime).Value = Modal.bthdate01 == null ? DBNull.Value : (object)Modal.bthdate01;
                        cmd.Parameters.Add("@nate01", SqlDbType.NVarChar).Value = Modal.nate01 == null ? DBNull.Value : (object)Modal.nate01;
                        conn.Open();
                        object resultObj = cmd.ExecuteScalar();
                        if (resultObj != null)
                        {
                            Int32.TryParse(resultObj.ToString(), out UID);
                        }
                        conn.Close();
                    }
                    else
                        LogHelper.writelog("SQL Connection is not available");
                    if (UID > 0)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public bool UpdateAWSUser(AWSUserModal Modal)
        {
            bool res = false;
            try
            {
                int UID = 0;
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        DataTable dt = new DataTable();
                        string InsertQuery = @"UPDATE dbo.Users 
                                               SET empste01=@empste01,fstnme01=@fstnme01,surnme01=@surnme01,bthcnte01=@bthcnte01,
                                                   fnce01=@fnce01,lane01=@lane01,rsnewe01=@rsnewe01,sxee01=@sxee01,bthdate01=@bthdate01,nate01=@nate01
                                               WHERE empnre01=@empnre01";

                        SqlCommand cmd = new SqlCommand(InsertQuery, conn);
                        cmd.Parameters.Add("@empnre01", SqlDbType.NVarChar).Value = Modal.empnre01 == null ? string.Empty : Modal.empnre01;
                        cmd.Parameters.Add("@empste01", SqlDbType.NVarChar).Value = Modal.empste01 == null ? string.Empty : Modal.empste01;
                        cmd.Parameters.Add("@fstnme01", SqlDbType.NVarChar).Value = Modal.fstnme01 == null ? string.Empty : Modal.fstnme01;
                        cmd.Parameters.Add("@surnme01", SqlDbType.NVarChar).Value = Modal.surnme01 == null ? string.Empty : Modal.surnme01;
                        cmd.Parameters.Add("@bthcnte01", SqlDbType.NVarChar).Value = Modal.bthcnte01 == null ? DBNull.Value : (object)Modal.bthcnte01;
                        cmd.Parameters.Add("@fnce01", SqlDbType.NVarChar).Value = Modal.fnce01 == null ? DBNull.Value : (object)Modal.fnce01;
                        cmd.Parameters.Add("@lane01", SqlDbType.NVarChar).Value = Modal.lane01 == null ? DBNull.Value : (object)Modal.lane01;
                        cmd.Parameters.Add("@rsnewe01", SqlDbType.NVarChar).Value = Modal.rsnewe01 == null ? DBNull.Value : (object)Modal.rsnewe01;
                        cmd.Parameters.Add("@sxee01", SqlDbType.NVarChar).Value = Modal.sxee01 == null ? DBNull.Value : (object)Modal.sxee01;
                        cmd.Parameters.Add("@bthdate01", SqlDbType.DateTime).Value = Modal.bthdate01 == null ? DBNull.Value : (object)Modal.bthdate01;
                        cmd.Parameters.Add("@nate01", SqlDbType.NVarChar).Value = Modal.nate01 == null ? DBNull.Value : (object)Modal.nate01;
                        conn.Open();
                        object resultObj = cmd.ExecuteNonQuery();
                        if (resultObj != null)
                        {
                            Int32.TryParse(resultObj.ToString(), out UID);
                        }
                        conn.Close();
                    }
                    else
                        LogHelper.writelog("SQL Connection is not available");
                    if (UID > 0)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
    }
}
