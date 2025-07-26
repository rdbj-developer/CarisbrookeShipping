using System.Data.SqlClient;

namespace CarisbrookeOpenFileService.Helper
{
    public static class SqlExtensions
    {
        public static bool IsAvailable(this SqlConnection conn)
        {
            try
            {
                conn.Open();
                conn.Close();
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }
    }
}
