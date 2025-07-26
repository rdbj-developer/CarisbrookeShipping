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
    public class CommonDBLayer
    {
		public static ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();	// RDBJ 04/18/2022
		public static string connetionString = Utility.GetLocalDBConnStr(dbConnModal);  // RDBJ 04/18/2022

		// RDBJ 04/18/2022
		public static DataSet RecordDataSet(string strSQL, SqlParameter[] sqlParams, CommandType commandType = CommandType.Text)
		{
			SqlConnection myConnection = new SqlConnection(connetionString);
			SqlDataAdapter myAdapter = new SqlDataAdapter();
			myAdapter.SelectCommand = new SqlCommand(strSQL, myConnection);
			myAdapter.SelectCommand.CommandType = commandType;
			if (sqlParams != null)
			{
                if (sqlParams.Length > 0)
                {
					for (int i = 0; i < sqlParams.Length; i++)
					{
						myAdapter.SelectCommand.Parameters.Add(sqlParams[i]);
					}
				}
			}
			DataSet ds = new DataSet();
			myAdapter.Fill(ds);
			return ds;
		}
		// End RDBJ 04/18/2022

		// RDBJ 04/18/2022
		public static DataSet RecordDataSet(string strSQL, SqlParameter[] sqlParam, CommandType commandType = CommandType.Text, string tablename = null, DataTable dt = null)
		{
			SqlConnection myConnection = new SqlConnection(connetionString);
			SqlDataAdapter myAdapter = new SqlDataAdapter();
			myAdapter.SelectCommand = new SqlCommand(strSQL, myConnection);
			myAdapter.SelectCommand.CommandType = commandType;
			myAdapter.SelectCommand.Parameters.AddRange(sqlParam);
			myAdapter.SelectCommand.CommandTimeout = 0;
			if (!string.IsNullOrEmpty(tablename))
				myAdapter.SelectCommand.Parameters.AddWithValue(tablename, dt);

			DataSet ds = new DataSet();
			myAdapter.Fill(ds);
			return ds;
		}
		// End RDBJ 04/18/2022
	}
}
