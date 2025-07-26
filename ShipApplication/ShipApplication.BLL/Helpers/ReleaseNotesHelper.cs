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
    //RDBJ 10/27/2021 Added this Modal
    public class ReleaseNotesHelper
    {
        public static string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
        public static SqlConnection connection = new SqlConnection(connetionString);
        public static SqlCommand command;
        public static DataTable dt;
        public static SqlDataAdapter sqlAdp;

        public List<ReleaseNotes> GetReleaseNotes()
        {
            List<ReleaseNotes> appReleaseList = new List<ReleaseNotes>();
            try
            {
                if (!string.IsNullOrEmpty(connetionString))
                {
                    if (CommonHelpers.StoredProcedure_ExistOrNot("SP_GetAllReleaseNotes"))
                    {
                        connection.Open();
                        command = new SqlCommand("SP_GetAllReleaseNotes", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        SqlDataReader sdr = command.ExecuteReader();

                        while (sdr.Read())
                        {
                            ReleaseNotes relNote = new ReleaseNotes();
                            relNote.AppVersion = Convert.ToString(sdr["AppVersion"]);
                            relNote.AppDescription = Convert.ToString(sdr["AppDescription"]).Replace("The latest version contains bug fixes. ", "");
                            relNote.AppPublishDate = Convert.ToDateTime(Convert.ToString(sdr["AppPublishDate"]));
                            relNote.NoOfFilesAffected = Convert.ToInt32(sdr["NoOfFilesAffected"]);
                            relNote.CreatedDate = Convert.ToDateTime(Convert.ToString(sdr["CreatedDate"]));
                            //relNote.UpdateDate = Convert.ToDateTime(Convert.ToString(sdr["UpdateDate"])); // RDBJ 12/08/2021 Commented this line
                            appReleaseList.Add(relNote);
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetReleaseNotes : " + ex.Message);
            }
            //This is use for close the connection
            finally
            {
                connection.Close();
            }
            return appReleaseList.DistinctBy(x=>x.AppVersion).ToList();
        }
    }
}
