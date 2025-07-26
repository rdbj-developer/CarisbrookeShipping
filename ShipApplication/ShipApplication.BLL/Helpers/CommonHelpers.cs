using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Helpers
{
    //RDBJ 10/30/2021 Created
    public static class CommonHelpers
    {
        public static string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
        public static SqlConnection connection = new SqlConnection(connetionString);
        public static SqlCommand command;
        public static bool StoredProcedure_ExistOrNot(string sp_Name)
        {
            bool blnIsExist = false;
            try
            {
                if (!string.IsNullOrEmpty(connetionString))
                {
                    string IsSPExistQuery = "select * from sysobjects where type='P' and name='" + sp_Name + "'";
                    connection.Open();
                    command = new SqlCommand(IsSPExistQuery, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            blnIsExist = true;
                            break;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StoredProcedure_ExistOrNot : " + ex.Message);
            }
            //This is use for close the connection
            finally
            {
                connection.Close();
            }
            return blnIsExist;
        }
    }
}
