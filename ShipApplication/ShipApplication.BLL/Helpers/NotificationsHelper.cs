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
    //RDBJ 10/25/2021 Added this Modal
    public class NotificationsHelper
    {
        public static string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
        public static SqlConnection connection = new SqlConnection(connetionString);
        public static SqlCommand command;
        public static DataTable dt;
        public static SqlDataAdapter sqlAdp;
        ServerConnectModal ServerModal = LocalDBHelper.ReadDBConfigJson(); //RDBJ 11/11/2021
        
        public List<Notifications> GetNotifications()
        {
            List<Notifications> notificationsList = new List<Notifications>();
            try
            {
                if (!string.IsNullOrEmpty(connetionString))
                {

                    if (CommonHelpers.StoredProcedure_ExistOrNot("SP_GetAllNotifications"))
                    {
                        connection.Open();
                        command = new SqlCommand("SP_GetAllNotifications", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        SqlDataReader sdr = command.ExecuteReader();
                        
                        while (sdr.Read())
                        {
                            Notifications notifications = new Notifications();
                            //notifications.Ship = item.Ship;
                            notifications.ShipName = Convert.ToString(sdr["Name"]);
                            notifications.ReportType = Convert.ToString(sdr["ReportType"]);
                            notifications.Deficiency = Convert.ToString(sdr["Deficiency"]);
                            notifications.DeficienciesUniqueID = Guid.Parse(Convert.ToString(sdr["DeficienciesUniqueID"]));
                            notifications.UpdatedDate = Convert.ToDateTime(Convert.ToString(sdr["UpdatedDate"]));
                            notifications.CommentsCount = Convert.ToInt64(sdr["CommentsCount"]);
                            notifications.ResolutionsCount = Convert.ToInt64(sdr["ResolutionsCount"]);
                            notifications.InitialActionsCount = Convert.ToInt64(sdr["InitialActionsCount"]);

                            notificationsList.Add(notifications);
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNotifications : " + ex.Message);
            }
            //This is use for close the connection
            finally
            {
                connection.Close();
            }

            //RDBJ 11/11/2021 wrapped in if
            if(!ServerModal.IsInspector)
                notificationsList = notificationsList.Where(x => x.ShipName == SessionManager.ShipName).ToList(); //RDBJ 11/08/2021

            return notificationsList;
        }

        //RDBJ 10/26/2021
        public long GetCountOfNotifications()
        {
            long totalNotifications = 0;
            try
            {
                if (!string.IsNullOrEmpty(connetionString))
                {
                    if (CommonHelpers.StoredProcedure_ExistOrNot("SP_GetAllNotifications"))
                    {
                        // JSL 11/03/2022
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                        // End JSL 11/03/2022
                        connection.Open();
                        command = new SqlCommand("SP_GetAllNotifications", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader sdr = command.ExecuteReader();

                        //RDBJ 11/08/2021 commented below
                        /*
                        if (sdr.HasRows)
                            totalNotifications = sdr.Cast<object>().Count();
                        else
                            totalNotifications = 0;
                        */

                        var shipWiseNotifictions = (dynamic)null;

                        //RDBJ 11/11/2021 wrapped in if
                        if (!ServerModal.IsInspector)
                        {
                            //RDBJ 11/08/2021
                            shipWiseNotifictions = (from IDataRecord r in sdr
                                                    select new
                                                    {
                                                        ShipName = r["Name"].ToString()
                                                    }).Where(a => a.ShipName == SessionManager.ShipName).ToList();
                        }
                        //RDBJ 11/11/2021 added else
                        else
                        {
                            shipWiseNotifictions = (from IDataRecord r in sdr
                                                    select new
                                                    {
                                                        ShipName = r["Name"].ToString()
                                                    }).ToList();
                        }

                        if(shipWiseNotifictions != null && shipWiseNotifictions.Count > 0)
                            totalNotifications = shipWiseNotifictions.Count;
                        else
                            totalNotifications = 0;
                        //End RDBJ 11/08/2021

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCountOfNotifications : " + ex.Message);
            }
            return totalNotifications;
        }
        //End RDBJ 10/26/2021

        //RDBJ 10/26/2021
        public Notifications GetCountOfNotificationsById(string id, string formType) //RDBJ 10/21/2021 Added formType
        {
            Notifications notifications = new Notifications();
            try
            {
                if (!string.IsNullOrEmpty(connetionString))
                {
                    if (CommonHelpers.StoredProcedure_ExistOrNot("SP_GetNotificationDetailsById"))
                    {
                        // JSL 11/03/2022
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                        // End JSL 11/03/2022
                        connection.Open();
                        command = new SqlCommand("SP_GetNotificationDetailsById", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.VarChar).Value = id;
                        command.Parameters.Add("@formType", SqlDbType.VarChar).Value = formType;

                        SqlDataReader sdr = command.ExecuteReader();
                        
                        while (sdr.Read())
                        {
                            //notifications.Ship = item.Ship;
                            notifications.ShipName = ""; //Convert.ToString(sdr["Name"]);
                            notifications.ReportType = ""; //Convert.ToString(sdr["ReportType"]);
                            notifications.Deficiency = ""; //Convert.ToString(sdr["Deficiency"]);
                            notifications.DeficienciesUniqueID = Guid.Empty; //Guid.Parse(Convert.ToString(sdr["DeficienciesUniqueID"]));
                            notifications.UpdatedDate = null; //Convert.ToDateTime(Convert.ToString(sdr["UpdatedDate"]));
                            notifications.CommentsCount = Convert.ToInt64(sdr["CommentsCount"]);
                            notifications.ResolutionsCount = Convert.ToInt64(sdr["ResolutionsCount"]);
                            notifications.InitialActionsCount = Convert.ToInt64(sdr["InitialActionsCount"]);

                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCountOfNotificationsById : " + ex.Message);
            }
            return notifications;
        }
        //End RDBJ 10/26/2021

        //RDBJ 10/26/2021
        public bool openAndSeenNotificationsUpdateStatusById(string id, string section, string formType)
        {
            bool notificationsUpdateStatusSeen = false;
            try
            {
                string tableName = string.Empty;
                string columnName = string.Empty;
                bool IsNeedToClearResolution = false;   // JSL 07/09/2022

                switch (section.ToLower())
                {
                    case "comments":
                        if (formType.ToLower() == "iaf")
                        {
                            tableName = "AuditNotesComments";
                            columnName = "NotesUniqueID";
                        }
                        else
                        {
                            tableName = "GIRDeficienciesNotes";
                            columnName = "DeficienciesUniqueID";
                        }
                        IsNeedToClearResolution = true; // JSL 07/09/2022
                        break;
                    case "initialactions":
                        tableName = "GIRDeficienciesInitialActions";
                        columnName = "DeficienciesUniqueID";
                        break;
                    case "resolution":
                        if (formType.ToLower() == "iaf")
                        {
                            tableName = "AuditNotesResolution";
                            columnName = "NotesUniqueID";
                        }
                        else
                        {
                            tableName = "GIRDeficienciesResolution";
                            columnName = "DeficienciesUniqueID";
                        }
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(connetionString))
                {
                    connection.Open();
                    command = new SqlCommand("UPDATE " + tableName + " SET isNew = 0 WHERE " + columnName + " = '" + id + "'", connection);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                // JSL 07/09/2022
                if (IsNeedToClearResolution)
                {
                    if (formType.ToLower() == "iaf")
                    {
                        tableName = "AuditNotesResolution";
                        columnName = "NotesUniqueID";
                    }
                    else
                    {
                        tableName = "GIRDeficienciesResolution";
                        columnName = "DeficienciesUniqueID";
                    }

                    if (!string.IsNullOrEmpty(connetionString))
                    {
                        connection.Open();
                        command = new SqlCommand("UPDATE " + tableName + " SET isNew = 0 WHERE " + columnName + " = '" + id + "'", connection);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                // End JSL 07/09/2022
                notificationsUpdateStatusSeen = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("openAndSeenNotificationsUpdateStatusById : " + ex.Message);
            }
            return notificationsUpdateStatusSeen;
        }
        //End RDBJ 10/26/2021
    }
}
