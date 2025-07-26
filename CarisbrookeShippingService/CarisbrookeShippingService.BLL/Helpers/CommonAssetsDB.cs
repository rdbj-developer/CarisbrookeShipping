using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Helpers
{
    // JSL 05/20/2022 added this class
    public class CommonAssetsDB
    {
		public static string connectionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
		public static DataSet RecordDataSet(string strSQL, SqlParameter[] sqlParam, CommandType commandType = CommandType.Text, string tablename = null, DataTable dt = null)
		{
			SqlConnection myConnection = new SqlConnection(connectionString);
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
	}
}
