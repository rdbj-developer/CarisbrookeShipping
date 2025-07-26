using DBModificationService.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBModificationService.Helpers
{
    public class AWSUsersHelper
    {
        public void SyncAwsUsers()
        {
            try
            {
                List<AWSUserModal> AwsUsersList = GetAWSUsers();
                List<AWSUserModal> OfficeUsersList = GetOfficeUsers();
                int i = 0;
                int j = 0;
                if (AwsUsersList != null && AwsUsersList.Count > 0)
                {
                    foreach (var item in AwsUsersList)
                    {
                        AWSUserModal localUser = OfficeUsersList.Where(x => x.empnre01 == item.empnre01).FirstOrDefault();
                        if (localUser != null)
                        {

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
                    LogHelper.writelog(i + " New Users Added in Office DB Users Table ");
                    LogHelper.writelog(j + " New Users Failed to Add in Office DB Users Table ");
                }
                else
                {
                    LogHelper.writelog(" No Data retrieved from AWS");
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        public List<AWSUserModal> GetAWSUsers()
        {
            string connetionString = "Data Source=185.175.112.35;Initial Catalog=scw52;User ID= scw52;Password=scw52";
            List<AWSUserModal> dbUsersList = new List<AWSUserModal>();
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT  [empnre01],[empste01],[fstnme01],[surnme01],[bthcnte01],[fnce01],[lane01],[rsnewe01],[sxee01],[bthdate01],[nate01] FROM dsise01 where empste01 = 'p'";
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
                        LogHelper.writelog("AWS Connection is not available !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return dbUsersList;
        }
        public List<AWSUserModal> GetOfficeUsers()
        {
            string connetionString = Utility.GetOfficeDBConnStr();
            List<AWSUserModal> dbUsersList = new List<AWSUserModal>();
            LogHelper.writelog(connetionString);
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done For Office Application");
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
                        LogHelper.writelog("Connection is not available For Office Application !!!");
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
                string connetionString = Utility.GetOfficeDBConnStr();
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
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
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



        //public List<AWSUserModal> GetOfficeUsersForTest()
        //{
        //    string connetionString = Utility.GetOfficeDBConnStr();
        //    List<AWSUserModal> dbUsersList = new List<AWSUserModal>();
        //    LogHelper.writelog(connetionString);
        //    try
        //    {
        //        using (var conn = new SqlConnection(connetionString))
        //        {
        //            if (conn.IsAvailable())
        //            {
        //                LogHelper.writelog("Connection Done For Office Application");
        //                string selectQuery = "SELECT  * FROM UsersNew";
        //                conn.Open();
        //                DataTable dt = new DataTable();
        //                SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
        //                sqlAdp.Fill(dt);
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    dbUsersList = dt.ToListof<AWSUserModal>();
        //                }
        //                conn.Close();
        //            }
        //            else
        //            {
        //                LogHelper.writelog("Connection is not available For Office Application !!!");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.writelog(ex.Message);
        //    }
        //    return dbUsersList;
        //}
    }
}
