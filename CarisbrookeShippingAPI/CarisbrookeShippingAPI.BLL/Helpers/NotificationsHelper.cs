using CarisbrookeShippingAPI.Entity;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;
using CarisbrookeShippingAPI.BLL.Helpers.OfficeHelper;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    //RDBJ 10/12/2021 Added this class
    public class NotificationsHelper
    {
        //RDBJ 10/14/2021
        public List<Notifications> GetNotifications(
            string UserID, int UserGroup    // RDBJ 12/18/2021
            , string crntUserName // RDBJ 01/05/2022
            )
        {
            List<Notifications> notificationsList = new List<Notifications>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var defNotificationLists = dbContext.SP_GetAllNotifications(
                    UserID, UserGroup   // RDBJ 12/18/2021
                    , crntUserName // RDBJ 01/05/2022
                    );
                foreach (var item in defNotificationLists)
                {
                    Notifications notifications = new Notifications();
                    //notifications.Ship = item.Ship; //RDNJ 10/21/2021 commented 
                    notifications.ShipName = item.Name; //RDBJ 10/21/2021
                    notifications.ReportType = item.ReportType; //RDBJ 10/21/2021
                    notifications.Deficiency = item.Deficiency;
                    notifications.AssignTo = item.UserName; // RDBJ 12/21/2021
                    notifications.DeficienciesUniqueID = item.DeficienciesUniqueID;
                    notifications.UpdatedDate = item.UpdatedDate;
                    notifications.CommentsCount = Convert.ToInt64(item.CommentsCount);
                    notifications.ResolutionsCount = Convert.ToInt64(item.ResolutionsCount);
                    notifications.InitialActionsCount = Convert.ToInt64(item.InitialActionsCount);

                    notificationsList.Add(notifications);
                }

                // JSL 06/16/2022
                if (UserGroup == 7)
                    notificationsList = notificationsList.Where(x => x.ReportType == "SI").ToList();
                else if (UserGroup != 1)
                    notificationsList = notificationsList.Where(x => x.ReportType != "SI").ToList();
                // End JSL 06/16/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNotifications : " + ex.Message);
            }
            return notificationsList;
        }
        //End RDBJ 10/14/2021

        //RDBJ 10/16/2021
        public long GetCountOfNotifications(
            string UserID, int UserGroup    // RDBJ 12/18/2021
            , string crntUserName // RDBJ 01/05/2022
            )
        {
            List<Notifications> notificationsList = new List<Notifications>();  // JSL 06/16/2022
            long totalNotifications = 0;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var defNotificationLists = dbContext.SP_GetAllNotifications(
                    UserID, UserGroup   // RDBJ 12/18/2021
                    , crntUserName // RDBJ 01/05/2022
                    );

                // JSL 06/16/2022 commented
                /*
                if (defNotificationLists != null)
                    totalNotifications = defNotificationLists.Count();
                else
                    totalNotifications = 0;
                */
                // End JSL 06/16/2022 commented

                // JSL 06/16/2022
                foreach (var item in defNotificationLists)
                {
                    Notifications notifications = new Notifications();
                    //notifications.Ship = item.Ship; //RDNJ 10/21/2021 commented 
                    notifications.ShipName = item.Name; //RDBJ 10/21/2021
                    notifications.ReportType = item.ReportType; //RDBJ 10/21/2021
                    notifications.Deficiency = item.Deficiency;
                    notifications.AssignTo = item.UserName; // RDBJ 12/21/2021
                    notifications.DeficienciesUniqueID = item.DeficienciesUniqueID;
                    notifications.UpdatedDate = item.UpdatedDate;
                    notifications.CommentsCount = Convert.ToInt64(item.CommentsCount);
                    notifications.ResolutionsCount = Convert.ToInt64(item.ResolutionsCount);
                    notifications.InitialActionsCount = Convert.ToInt64(item.InitialActionsCount);

                    notificationsList.Add(notifications);
                }

                if (UserGroup == 7)
                    notificationsList = notificationsList.Where(x => x.ReportType == "SI").ToList();
                else if (UserGroup != 1)
                    notificationsList = notificationsList.Where(x => x.ReportType != "SI").ToList();


                if (notificationsList != null)
                    totalNotifications = notificationsList.Count();
                else
                    totalNotifications = 0;
                // End JSL 06/16/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCountOfNotifications : " + ex.Message);
            }
            return totalNotifications;
        }
        //End RDBJ 10/16/2021

        //RDBJ 10/16/2021
        public Notifications GetCountOfNotificationsById(string id, string formType
            , string crntUserName // RDBJ 01/05/2022
            ) //RDBJ 10/21/2021 Added formType
        {
            Notifications notifications = new Notifications();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var defNotificationDetails = dbContext.SP_GetNotificationDetailsById(id, formType
                    , crntUserName // RDBJ 01/05/2022
                    ).FirstOrDefault(); //RDBJ 10/21/2021 Added formType
                if (defNotificationDetails != null)
                {

                    notifications.Ship = ""; //defNotificationDetails.Ship; //RDBJ 10/21/2021 Commented and set null
                    notifications.Deficiency = ""; //defNotificationDetails.Deficiency; //RDBJ 10/21/2021 Commented and set null
                    notifications.DeficienciesUniqueID = Guid.Empty; // defNotificationDetails.DeficienciesUniqueID; //RDBJ 10/21/2021 Commented and set null
                    notifications.UpdatedDate = null; //defNotificationDetails.UpdatedDate; //RDBJ 10/21/2021 Commented and set null
                    notifications.CommentsCount = Convert.ToInt64(defNotificationDetails.CommentsCount);
                    notifications.ResolutionsCount = Convert.ToInt64(defNotificationDetails.ResolutionsCount);
                    notifications.InitialActionsCount = Convert.ToInt64(defNotificationDetails.InitialActionsCount);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCountOfNotificationsById : " + ex.Message);
            }
            return notifications;
        }
        //End RDBJ 10/16/2021

        // RDBJ 04/01/2022 commented below function
        /*
        //RDBJ 10/16/2021
        public bool openAndSeenNotificationsUpdateStatusById(string id, string section, string formType) //RDBJ 10/21/2021 Added formType
        {
            bool notificationsUpdateStatusSeen = false;
            try
            {
                string tableName = string.Empty;
                string columnName = string.Empty; //RDBJ 10/21/2021

                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                switch (section.ToLower())
                {
                    case "comments":
                        //RDBJ 10/21/2021 wrapped in if and added logic for the IAF
                        if (formType.ToLower() == "iaf")
                        {
                            tableName = "AuditNotesComments";
                            columnName = "NotesUniqueID";
                        }
                        else
                        {
                            tableName = "GIRDeficienciesNotes";
                            columnName = "DeficienciesUniqueID"; //RDBJ 10/21/2021
                        }
                        break;
                    case "initialactions":
                        tableName = "GIRDeficienciesInitialActions";
                        columnName = "DeficienciesUniqueID"; //RDBJ 10/21/2021
                        break;
                    case "resolution":
                        //RDBJ 10/21/2021 wrapped in if and added logic for the IAF
                        if (formType.ToLower() == "iaf")
                        {
                            tableName = "AuditNotesResolution";
                            columnName = "NotesUniqueID";
                        }
                        else
                        {
                            tableName = "GIRDeficienciesResolution";
                            columnName = "DeficienciesUniqueID"; //RDBJ 10/21/2021
                        }
                        break;
                    default:
                        break;
                }
                
                dbContext.Database.ExecuteSqlCommand("UPDATE " + tableName + " SET isNew = {0} WHERE " + columnName + " = {1}", 0, id); //RDBJ 10/21/2021 added columnName
                notificationsUpdateStatusSeen = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("openAndSeenNotificationsUpdateStatusById : " + ex.Message);
            }
            return notificationsUpdateStatusSeen;
        }
        //End RDBJ 10/16/2021
        */

        // RDBJ 04/01/2022
        public bool openAndSeenNotificationsUpdateStatusById(Dictionary<string, object> dicData) 
        {
            bool notificationsUpdateStatusSeen = false;
            try
            {
                string parentTableName = string.Empty;
                string tableName = string.Empty;
                string columnName = string.Empty;
                string section = string.Empty;
                string formType = string.Empty;

                bool blnAllowToUpdateSeenStatus = false;

                Guid guidCurrentUserID = Guid.Empty;
                Guid guidDeforNoteID = Guid.Empty;
                int strAssignedUserGroupID = 0;
                int strCurrentUserGroupID = 0;

                int intTechnicalOrISMGroupID = 0; // RDBJ 01/17/2022

                if (dicData.ContainsKey("CurrentUserID"))
                {
                    guidCurrentUserID = Guid.Parse(Convert.ToString(dicData["CurrentUserID"]));
                }

                if (dicData.ContainsKey("DefOrNoteID"))
                {
                    guidDeforNoteID = Guid.Parse(Convert.ToString(dicData["DefOrNoteID"]));
                }

                if (dicData.ContainsKey("Section"))
                {
                    section = Convert.ToString(dicData["Section"]);
                }

                if (dicData.ContainsKey("FormType"))
                {
                    formType = Convert.ToString(dicData["FormType"]);
                }

                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                switch (section.ToLower())
                {
                    case "comments":
                        if (formType.ToLower() == "iaf")
                        {
                            parentTableName = "AuditNotes";
                            tableName = "AuditNotesComments";
                            columnName = "NotesUniqueID";
                        }
                        else
                        {
                            parentTableName = "GIRDeficiencies";
                            tableName = "GIRDeficienciesNotes";
                            columnName = "DeficienciesUniqueID";
                        }
                        break;
                    case "initialactions":
                        parentTableName = "GIRDeficiencies";    // RDBJ 01/07/2022
                        tableName = "GIRDeficienciesInitialActions";
                        columnName = "DeficienciesUniqueID";
                        break;
                    case "resolution":
                        if (formType.ToLower() == "iaf")
                        {
                            parentTableName = "AuditNotes";
                            tableName = "AuditNotesResolution";
                            columnName = "NotesUniqueID";
                        }
                        else
                        {
                            parentTableName = "GIRDeficiencies";
                            tableName = "GIRDeficienciesResolution";
                            columnName = "DeficienciesUniqueID";
                        }
                        break;
                    default:
                        break;
                }

                string guidAssignedUserID = dbContext.Database
                    .SqlQuery<string>("Select Cast([AssignTo] as nvarchar(255)) from [" + parentTableName + "] where [" + columnName + "] = @id" // RDBJ 01/07/2022 Added cast
                    , new SqlParameter("@id", guidDeforNoteID))
                    .FirstOrDefault();

                // RDBJ 01/17/2022 added if
                if (formType.ToLower() == "si")
                {
                    intTechnicalOrISMGroupID = 7; // Number 7 is for Technical Users
                }
                else
                {
                    intTechnicalOrISMGroupID = 5; // Number 5 is for ISM Users
                }


                if (!string.IsNullOrEmpty(Convert.ToString(guidAssignedUserID))
                    //&& guidAssignedUserID != Guid.Empty
                    )
                {
                    if (guidCurrentUserID.ToString().ToLower() == guidAssignedUserID.ToLower())
                    {
                        blnAllowToUpdateSeenStatus = true;
                    }
                    else
                    {
                        strCurrentUserGroupID = dbContext.Database.SqlQuery<int>("Select [UserGroup] from [UserProfile] where [UserID] = @id", new SqlParameter("@id", guidCurrentUserID))
                            .FirstOrDefault();

                        if (strCurrentUserGroupID == intTechnicalOrISMGroupID //5 // RDBJ 01/17/2022 used dynamic group value
                            || strCurrentUserGroupID == 1)
                        {
                            strAssignedUserGroupID = dbContext.Database.SqlQuery<int>("Select [UserGroup] from [UserProfile] where [UserID] = @id", new SqlParameter("@id", guidAssignedUserID))
                            .FirstOrDefault();

                            if (strAssignedUserGroupID == intTechnicalOrISMGroupID //5 // RDBJ 01/17/2022 used dynamic group value 
                                || strAssignedUserGroupID == 1)
                            {
                                blnAllowToUpdateSeenStatus = true;
                            }
                        }
                    }
                }
                else
                {
                    blnAllowToUpdateSeenStatus = false;
                    
                    // RDBJ 01/05/2022
                    strCurrentUserGroupID = dbContext.Database.SqlQuery<int>("Select [UserGroup] from [UserProfile] where [UserID] = @id", new SqlParameter("@id", guidCurrentUserID))
                            .FirstOrDefault();

                    if (strCurrentUserGroupID == intTechnicalOrISMGroupID //5 // RDBJ 01/17/2022 used dynamic group value 
                        || strCurrentUserGroupID == 1)
                    {
                        blnAllowToUpdateSeenStatus = true;
                    }
                    // End RDBJ 01/05/2022
                }

                //if (blnAllowToUpdateSeenStatus)   // JSL 07/04/2022 commented this line
                if (true)   // JSL 07/04/2022
                {
                    dbContext.Database.ExecuteSqlCommand("UPDATE " + tableName + " SET isNew = {0} WHERE " + columnName + " = {1}", 0, guidDeforNoteID);
                }
                
                notificationsUpdateStatusSeen = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("openAndSeenNotificationsUpdateStatusById : " + ex.Message);
            }
            return notificationsUpdateStatusSeen;
        }
        //End RDBJ 04/01/2022

        // JSL 06/25/2022
        public bool openAndSeenNotificationsUpdateStatusByDefId(Dictionary<string, object> dicData)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            bool notificationsUpdateStatusSeen = false;
            try
            {
                string tableName = string.Empty;
                string columnName = string.Empty;
                string section = string.Empty;
                string strDataType = string.Empty;

                // JSL 07/04/2022
                string parentTableName = string.Empty;
                string formType = string.Empty;

                bool blnAllowToUpdateSeenStatusForSelf = false;
                bool blnAllowToUpdateSeenStatusForAllExceptAssignedUser = false;    
                bool blnAllowToUpdateSeenStatusForAllUserBasedOnShipFleetIdGroup = false;    
                bool blnAllowToUpdateSeenStatusForAllUserBasedOnShipFLeetIdExceptWhohaveFleetId = false;    
                bool blnAllowToUpdateSeenStatusForAllExceptAssignedOne = false; 
                bool blnAllowToUpdateSeenStatusForAllISM = false; // JSL 07/07/2022
                bool blnAllowToUpdateSeenStatusForAllAdmin = false; // JSL 07/07/2022

                int intAssignedUserGroupID = 0;
                int intCurrentUserGroupID = 0;
                int intTechnicalOrISMGroupID = 0;
                int intSheepFleetId = 0;
                // End JSL 07/04/2022

                Guid guidCurrentUserID = Guid.Empty;
                Guid guidDeforNoteID = Guid.Empty;

                if (dicData.ContainsKey("CurrentUserID"))
                    guidCurrentUserID = Guid.Parse(Convert.ToString(dicData["CurrentUserID"]));

                if (dicData.ContainsKey("DefOrNoteID"))
                    guidDeforNoteID = Guid.Parse(Convert.ToString(dicData["DefOrNoteID"]));

                if (dicData.ContainsKey("Section"))
                    section = Convert.ToString(dicData["Section"]);

                // JSL 07/04/2022
                string strGuidAssignedUserID = string.Empty;
                string strShipCode = string.Empty;
                if (dicData.ContainsKey("FormType"))
                {
                    formType = Convert.ToString(dicData["FormType"]);
                }

                if (formType.ToLower() == "iaf")
                {
                    var entityModalAuditOrDefData = dbContext.AuditNotes.Where(x => x.NotesUniqueID == guidDeforNoteID).FirstOrDefault();
                    strGuidAssignedUserID = Convert.ToString(entityModalAuditOrDefData.AssignTo);
                }
                else
                {
                    var entityModalAuditOrDefData = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == guidDeforNoteID).FirstOrDefault();
                    strGuidAssignedUserID = Convert.ToString(entityModalAuditOrDefData.AssignTo);

                    if (formType.ToLower() == "si")
                    {
                        intTechnicalOrISMGroupID = 7; // Number 7 is for Technical Users
                        strShipCode = Convert.ToString(entityModalAuditOrDefData.Ship);
                    }
                    else
                    {
                        intTechnicalOrISMGroupID = 5; // Number 5 is for ISM Users
                    }
                }

                intCurrentUserGroupID = dbContext.Database.SqlQuery<int>("Select [UserGroup] from [UserProfile] where [UserID] = @id", new SqlParameter("@id", guidCurrentUserID))
                            .FirstOrDefault();

                if (!string.IsNullOrEmpty(strGuidAssignedUserID)
                    )
                {
                    if (guidCurrentUserID.ToString().ToLower() == strGuidAssignedUserID.ToLower())
                    {
                        blnAllowToUpdateSeenStatusForSelf = true;
                    }
                    else
                    {
                        blnAllowToUpdateSeenStatusForAllExceptAssignedOne = true;
                    }
                }
                else
                {
                    if (intCurrentUserGroupID == intTechnicalOrISMGroupID
                        || intCurrentUserGroupID == 1)
                    {
                        if (intCurrentUserGroupID == 7)
                        {
                            if (!string.IsNullOrEmpty(strShipCode))
                            {
                                var entityShipData = dbContext.CSShips.Where(x => x.Code == strShipCode).FirstOrDefault();
                                intSheepFleetId = (int)entityShipData.FleetId;

                                var currentViewerUserDetails = dbContext.UserProfiles.Where(x => x.UserID == guidCurrentUserID).FirstOrDefault();
                                if (currentViewerUserDetails.ShipFleetID == intSheepFleetId)
                                {
                                    // clean base on SheepFleetId group users
                                    blnAllowToUpdateSeenStatusForAllUserBasedOnShipFleetIdGroup = true;
                                }
                                else
                                {
                                    if (currentViewerUserDetails.ShipFleetID == 0)
                                    {
                                        // clean except sheepfleetid of all
                                        blnAllowToUpdateSeenStatusForAllUserBasedOnShipFLeetIdExceptWhohaveFleetId = true;
                                    }
                                    else
                                    {
                                        blnAllowToUpdateSeenStatusForSelf = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // JSL 07/07/2022
                            if (intCurrentUserGroupID == 5)
                            {
                                blnAllowToUpdateSeenStatusForAllISM = true;
                            }
                            else if (intCurrentUserGroupID == 1)
                            {
                                blnAllowToUpdateSeenStatusForAllAdmin = true;
                            }
                            // End JSL 07/07/2022
                            //blnAllowToUpdateSeenStatusForAll = true;
                        }
                    }
                }
                // End JSL 07/04/2022

                tableName = "Notification";
                columnName = "UniqueDataId";

                // JSL 07/06/2022
                List<string> lstDataType = new List<string>();
                lstDataType.Add("Comment");
                lstDataType.Add("InitialAction");
                lstDataType.Add("Resolution");
                // End JSL 07/06/2022

                switch (section.ToLower())
                {
                    case "comments":
                        //strDataType = "Comment";    // JSL 07/06/2022 commented
                        lstDataType = lstDataType.Where(x => x != "InitialAction").ToList();    // JSL 07/06/2022
                        break;
                    case "initialactions":
                        //strDataType = "InitialAction";  // JSL 07/06/2022 commented
                        lstDataType = lstDataType.Where(x => x == "InitialAction").ToList();    // JSL 07/06/2022
                        break;
                    case "resolution":
                        //strDataType = "Resolution"; // JSL 07/06/2022 commented
                        lstDataType = lstDataType.Where(x => x != "InitialAction").ToList();    // JSL 07/06/2022
                        break;
                    default:
                        break;
                }

                // JSL 07/06/2022
                if (lstDataType != null && lstDataType.Count > 0)
                {
                    strDataType = string.Join(",", lstDataType.Select(x => string.Format("'{0}'", x)));
                }
                // End JSL 07/06/2022

                // JSL 07/04/2022
                List<Guid> lstUsers = new List<Guid>();
                
                if (blnAllowToUpdateSeenStatusForAllExceptAssignedOne)
                {
                    Guid guidAssignedUserID = Guid.Parse(strGuidAssignedUserID);
                    lstUsers = dbContext.UserProfiles.Where(x => x.UserGroup == intCurrentUserGroupID && x.UserID != guidAssignedUserID).Select(x=> x.UserID).ToList();    // JSL 07/07/2022 removed x.UserGroup == 1 || 
                }

                if (blnAllowToUpdateSeenStatusForSelf)
                {
                    lstUsers = dbContext.UserProfiles.Where(x => (x.UserGroup == intCurrentUserGroupID) && x.UserID == guidCurrentUserID).Select(x => x.UserID).ToList();    // JSL 07/07/2022 removed x.UserGroup == 1 || 
                }

                if (blnAllowToUpdateSeenStatusForAllUserBasedOnShipFleetIdGroup)
                {
                    lstUsers = dbContext.UserProfiles.Where(x => (x.UserGroup == intCurrentUserGroupID) && x.ShipFleetID == intSheepFleetId).Select(x => x.UserID).ToList(); // JSL 07/07/2022 removed x.UserGroup == 1 || 
                }

                if (blnAllowToUpdateSeenStatusForAllUserBasedOnShipFLeetIdExceptWhohaveFleetId)
                {
                    lstUsers = dbContext.UserProfiles.Where(x => (x.UserGroup == intCurrentUserGroupID) && x.ShipFleetID == 0).Select(x => x.UserID).ToList();   // JSL 07/07/2022 removed x.UserGroup == 1 || 
                }

                // JSL 07/07/2022
                if (blnAllowToUpdateSeenStatusForAllISM)
                {
                    lstUsers = dbContext.UserProfiles.Where(x => x.UserGroup == 5).Select(x => x.UserID).ToList();
                }
                if (blnAllowToUpdateSeenStatusForAllAdmin)
                {
                    lstUsers = dbContext.UserProfiles.Where(x => x.UserGroup == 1).Select(x => x.UserID).ToList();
                }
                // End JSL 07/07/2022

                string strUserIdListForClearNotification = string.Empty;
                if (lstUsers != null && lstUsers.Count > 0)
                {
                    strUserIdListForClearNotification = string.Join(",", lstUsers.Select(x => string.Format("'{0}'", x)));
                }
                // End JSL 07/04/2022

                if (!string.IsNullOrEmpty(strUserIdListForClearNotification))
                    //dbContext.Database.ExecuteSqlCommand("UPDATE [" + tableName + "] SET [ReadDateTime] = '" + Utility.ToDateTimeUtcNow() + "', [IsRead] = {0} WHERE [" + columnName + "] = {1} AND [DataType] = {2} AND [SentToUserId] IN (" + strUserIdListForClearNotification + ")", 1, guidDeforNoteID, strDataType);  // JSL 07/06/2022 commented 
                    dbContext.Database.ExecuteSqlCommand("UPDATE [" + tableName + "] SET [ReadDateTime] = '" + Utility.ToDateTimeUtcNow() + "', [ReadByUserId] = '" + guidCurrentUserID + "', [IsRead] = {0} WHERE [" + columnName + "] = {1} AND [DataType] IN (" + strDataType + ") AND [SentToUserId] IN (" + strUserIdListForClearNotification + ")", 1, guidDeforNoteID);  // JSL 07/07/2022 added [ReadByUserId] = '" + guidCurrentUserID + "'  // JSL 07/06/2022

                SendNotificationsForSignalR(Convert.ToString(guidCurrentUserID));
                notificationsUpdateStatusSeen = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("openAndSeenNotificationsUpdateStatusByDefId : " + ex.Message);
            }
            return notificationsUpdateStatusSeen;
        }
        // End JSL 06/25/2022

        //RDBJ 12/21/2021
        public List<Notifications> LoadAssignedToMeGISIIADeficiencies(
            string UserID
            )
        {
            List<Notifications> notificationsList = new List<Notifications>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var defNotificationLists = dbContext.SP_GetAssignedToMeGISIIADeficiencies(
                    UserID
                    );
                foreach (var item in defNotificationLists)
                {
                    Notifications notifications = new Notifications();
                    notifications.ShipName = item.Name;
                    notifications.ReportType = item.ReportType;
                    notifications.Deficiency = item.Deficiency;
                    notifications.DeficienciesUniqueID = item.DeficienciesUniqueID;
                    notifications.UpdatedDate = item.UpdatedDate;

                    notificationsList.Add(notifications);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("LoadAssignedToMeGISIIADeficiencies : " + ex.Message);
            }
            return notificationsList;
        }
        //End RDBJ 12/21/2021

        // JSL 06/25/2022
        public static Dictionary<string, object> GetNotificationsByUser(string UserID)
        {
            Dictionary<string, object> dictReturn = new Dictionary<string, object>();
            List<Notifications> notificationsList = new List<Notifications>();
            long totalNotifications = 0;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var defNotificationLists = dbContext.SP_GetAllNotificationsByUser(UserID);
                
                foreach (var item in defNotificationLists.ToList())
                {
                    Notifications notifications = new Notifications();
                    //notifications.Ship = item.Ship; 
                    notifications.ShipName = item.Name; 
                    notifications.ReportType = item.ReportType;
                    notifications.Deficiency = item.Deficiency;
                    notifications.AssignTo = item.UserName;
                    notifications.DeficienciesUniqueID = item.DeficienciesUniqueID;
                    notifications.UpdatedDate = item.UpdatedDate;
                    notifications.CommentsCount = Convert.ToInt64(item.Comment);
                    notifications.ResolutionsCount = Convert.ToInt64(item.Resolution);
                    notifications.InitialActionsCount = Convert.ToInt64(item.InitialAction);

                    notificationsList.Add(notifications);
                }

                if (defNotificationLists != null)
                    totalNotifications = Convert.ToInt32(notificationsList.Count());
                else
                    totalNotifications = 0;

                dictReturn["NumberOfDeficienciesNotifications"] = totalNotifications;
                dictReturn["DeficienciesNotificationsList"] = notificationsList;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNotificationsByUser : " + ex.Message);
            }
            return dictReturn;
        }
        // End JSL 06/25/2022

        // JSL 06/30/2022
        public static Dictionary<string, object> GetRecentNotificationsByUser(string UserID, string FromDateTime)
        {
            Dictionary<string, object> dictReturn = new Dictionary<string, object>();
            List<RecentNotifications> notificationsList = new List<RecentNotifications>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                DateTime afterThisdateTime = DateTime.ParseExact(FromDateTime, "dd/MM/yyyy HH:mm:ss", null);
                var defNotificationLists = dbContext.CB_proc_GetRecentNotifications(UserID, afterThisdateTime);

                foreach (var item in defNotificationLists.ToList())
                {
                    RecentNotifications notifications = new RecentNotifications();
                    //notifications.Ship = item.Ship; 
                    notifications.UniqueDataId = item.UniqueDataId;
                    notifications.ShipName = item.Name;
                    notifications.ReportType = item.ReportType;
                    notifications.Number = Convert.ToString(item.No);
                    notifications.Deficiency = item.Deficiency;
                    notifications.Priority = Convert.ToString(item.Priority);
                    notifications.DataType = item.DataType;
                    notifications.CreatedDateTime = item.CreatedDateTime;
                    notificationsList.Add(notifications);
                }

                dictReturn["RecentNotificationsList"] = notificationsList;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRecentNotificationsByUser : " + ex.Message);
            }
            return dictReturn;
        }
        // End JSL 06/30/2022

        #region Common Functions
        // JSL 04/30/2022
        public Dictionary<string, string> PerformPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                case AppStatic.API_OPENANDSEENNOTIFICATION:
                    {
                        try
                        {
                            string strNotificationID = string.Empty;
                            string strNotificationDetailsURL = string.Empty;
                            string strCurrentUserID = string.Empty; // JSL 07/07/2022

                            if (dictMetaData.ContainsKey("NotDataId"))
                                strNotificationID = dictMetaData["NotDataId"].ToString();

                            if (dictMetaData.ContainsKey("NotDetailsURL"))
                                strNotificationDetailsURL = dictMetaData["NotDetailsURL"].ToString();

                            // JSL 07/07/2022
                            if (dictMetaData.ContainsKey("CurrentUserID"))
                                strCurrentUserID = dictMetaData["CurrentUserID"].ToString();
                            // End JSL 07/07/2022

                            Guid NotificationGUID = Guid.Parse(strNotificationID);
                            Guid GuidCurrentUserID = Guid.Parse(strCurrentUserID);  // JSL 07/07/2022

                            Entity.Notification entityNotification = dbContext.Notifications.Where(x => x.UniqueId == NotificationGUID).FirstOrDefault();
                            entityNotification.IsRead = true;
                            entityNotification.ReadByUserId = GuidCurrentUserID; // JSL 07/07/2022
                            entityNotification.ReadDateTime = Utility.ToDateTimeUtcNow();
                            dbContext.SaveChanges();

                            retDictMetaData["NotDetailsURL"] = strNotificationDetailsURL;
                            // JSL 07/12/2022
                            retDictMetaData["NotDataId"] = strNotificationID;  
                            SendNotificationsForSignalR(strCurrentUserID, blnSendNotificationToUserForForm: true);
                            // End JSL 07/12/2022
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_OPENANDSEENNOTIFICATION + " Error : " + ex.Message);
                        }
                        break;
                    }
                case AppStatic.API_GETNOTIFICATION:
                    {
                        try
                        {
                            string strUserID = string.Empty;

                            if (dictMetaData.ContainsKey("CurrentUserId"))
                                strUserID = dictMetaData["CurrentUserId"].ToString();

                            Guid CurrUserGUID = Guid.Parse(strUserID);

                            /*
                            var lstNotifications = dbContext.Notifications
                                .Where(x => x.SentToUserId == CurrUserGUID
                                && x.IsRead == false
                                )
                                .OrderByDescending(x => x.CreatedDateTime)
                                .ToList();
                            */

                            List<Dictionary<string, string>> dictLstNotifications = new List<Dictionary<string, string>>();

                            var dbLstNotifications = dbContext.SP_GetNotificationsForTheISMDashboard(strUserID);
                            foreach (var item in dbLstNotifications)
                            {
                                Dictionary<string, string> dictNotification = new Dictionary<string, string>();
                                dictNotification["UniqueId"] = item.UniqueId.ToString();
                                dictNotification["UniqueDataId"] = item.UniqueDataId.ToString();
                                dictNotification["DataType"] = item.DataType;
                                dictNotification["IsDraft"] = item.IsDraft.ToString().ToLower();
                                dictNotification["Title"] = item.Title;
                                dictNotification["DetailsURL"] = item.DetailsURL;
                                dictNotification["CreatedDateTime"] = item.CreatedDateTime.ToString();
                                dictNotification["ReadDateTime"] = item.ReadDateTime.ToString();
                                dictNotification["IsRead"] = item.IsRead.ToString().ToLower();
                                dictNotification["ShipName"] = item.ShipName;
                                dictNotification["CreatedByPerson"] = item.CreatedByPerson;
                                dictNotification["DataDate"] = item.DataDate.ToString();
                                dictNotification["ShipCode"] = item.ShipCode;   // JSL 05/25/2022


                                dictLstNotifications.Add(dictNotification);
                            }

                            retDictMetaData["CurrentUserNotifications"] = JsonConvert.SerializeObject(dictLstNotifications);

                            // JSL 06/25/2022
                            Dictionary<string, object> dictDeficienciesNotificationData = new Dictionary<string, object>();
                            dictDeficienciesNotificationData = NotificationsHelper.GetNotificationsByUser(strUserID);
                            retDictMetaData["DeficienciesNotificationsList"] = JsonConvert.SerializeObject(dictDeficienciesNotificationData["DeficienciesNotificationsList"]);
                            // End JSL 06/25/2022

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_GETNOTIFICATION + " Error : " + ex.Message);
                        }
                        break;
                    }
                // JSL 07/04/2022
                case AppStatic.API_GETNOTIFICATIONFROMPAGE:
                {
                    try
                    {
                        string strUserID = string.Empty;

                        if (dictMetaData.ContainsKey("CurrentUserId"))
                            strUserID = dictMetaData["CurrentUserId"].ToString();

                        Guid CurrUserGUID = Guid.Parse(strUserID);

                        List<Dictionary<string, string>> dictLstNotifications = new List<Dictionary<string, string>>();

                        var dbLstNotifications = dbContext.SP_GetNotificationsForTheISMDashboard(strUserID);
                        foreach (var item in dbLstNotifications)
                        {
                            Dictionary<string, string> dictNotification = new Dictionary<string, string>();
                            dictNotification["UniqueId"] = item.UniqueId.ToString();
                            dictNotification["UniqueDataId"] = item.UniqueDataId.ToString();
                            dictNotification["DataType"] = item.DataType;
                            dictNotification["IsDraft"] = item.IsDraft.ToString().ToLower();
                            dictNotification["Title"] = item.Title;
                            dictNotification["DetailsURL"] = item.DetailsURL;
                            dictNotification["CreatedDateTime"] = item.CreatedDateTime.ToString();
                            dictNotification["ReadDateTime"] = item.ReadDateTime.ToString();
                            dictNotification["IsRead"] = item.IsRead.ToString().ToLower();
                            dictNotification["ShipName"] = item.ShipName;
                            dictNotification["CreatedByPerson"] = item.CreatedByPerson;
                            dictNotification["DataDate"] = item.DataDate.ToString();
                            dictNotification["ShipCode"] = item.ShipCode;   


                            dictLstNotifications.Add(dictNotification);
                        }

                        retDictMetaData["CurrentUserNotifications"] = JsonConvert.SerializeObject(dictLstNotifications);

                        Dictionary<string, object> dictDeficienciesNotificationData = new Dictionary<string, object>();
                        dictDeficienciesNotificationData = NotificationsHelper.GetNotificationsByUser(strUserID);
                        retDictMetaData["DeficienciesNotificationsList"] = JsonConvert.SerializeObject(dictDeficienciesNotificationData["DeficienciesNotificationsList"]);

                        IsPerformSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog(AppStatic.API_GETNOTIFICATIONFROMPAGE + " Error : " + ex.Message);
                    }
                    break;
                }
                // End JSL 07/04/2022
                // JSL 06/25/2022
                case AppStatic.API_GETDEFICIENCIESNOTIFICATION:
                    {
                        try
                        {
                            string strUserID = string.Empty;

                            if (dictMetaData.ContainsKey("CurrentUserId"))
                                strUserID = dictMetaData["CurrentUserId"].ToString();

                            Dictionary<string, object> dictDeficienciesNotificationData = new Dictionary<string, object>();
                            dictDeficienciesNotificationData = NotificationsHelper.GetNotificationsByUser(strUserID);
                            retDictMetaData["DeficienciesNotificationsList"] = JsonConvert.SerializeObject(dictDeficienciesNotificationData["DeficienciesNotificationsList"]);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_GETDEFICIENCIESNOTIFICATION + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End JSL 06/25/2022
                // JSL 06/30/2022
                case AppStatic.API_GETRECENTDEFICIENCIESNOTIFICATION:
                    {
                        try
                        {
                            string strUserID = string.Empty;
                            string strInitialDateTime = string.Empty;

                            if (dictMetaData.ContainsKey("CurrentUserId"))
                                strUserID = dictMetaData["CurrentUserId"].ToString();
                            
                            if (dictMetaData.ContainsKey("InitialDateTime"))
                                strInitialDateTime = dictMetaData["InitialDateTime"].ToString();

                            Dictionary<string, object> dictRecentDeficienciesNotificationData = new Dictionary<string, object>();
                            dictRecentDeficienciesNotificationData = NotificationsHelper.GetRecentNotificationsByUser(strUserID, strInitialDateTime);
                            retDictMetaData["RecentDeficienciesNotificationsList"] = JsonConvert.SerializeObject(dictRecentDeficienciesNotificationData["RecentNotificationsList"]);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_GETRECENTDEFICIENCIESNOTIFICATION + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End JSL 06/30/2022
                default:
                    break;
            }

            if (IsPerformSuccess)
            {
                retDictMetaData["Status"] = AppStatic.SUCCESS;
            }
            else
            {
                retDictMetaData["Status"] = AppStatic.ERROR;
            }

            return retDictMetaData;
        }
        // End JSL 04/30/2022

        // JSL 06/24/2022
        public static void SaveNotificationsDataForEachUser(Dictionary<string, string> dictNotificationData
            , string strFormType = ""   // JSL 06/27/2022
            , string strShipCode = ""   // JSL 06/27/2022
            , bool IsNeedToSendNotificationBaseOnShipFleetId = false    // JSL 06/27/2022
            )
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            int intUserGroup = 0;
            Guid guidUniqueDataId = Guid.Empty;
            string strDataType = string.Empty;
            bool blnIsDraft = false;
            string strTitle = string.Empty;
            string strDetailsURL = string.Empty;

            if (dictNotificationData != null && dictNotificationData.Count > 0)
            {
                if (dictNotificationData.ContainsKey("UserGroup"))
                    intUserGroup = Convert.ToInt32(dictNotificationData["UserGroup"]);

                if (dictNotificationData.ContainsKey("UniqueDataId"))
                    guidUniqueDataId = Guid.Parse(dictNotificationData["UniqueDataId"]);

                if (dictNotificationData.ContainsKey("DataType"))
                    strDataType = dictNotificationData["DataType"];

                if (dictNotificationData.ContainsKey("IsDraft"))
                    blnIsDraft = Convert.ToBoolean(dictNotificationData["IsDraft"]);

                if (dictNotificationData.ContainsKey("Title"))
                    strTitle = dictNotificationData["Title"];

                if (dictNotificationData.ContainsKey("DetailsURL"))
                    strDetailsURL = dictNotificationData["DetailsURL"];

                var UsersList = dbContext.UserProfiles
                                .Where(x => (x.UserGroup == 1    // 1 for Admin group
                                //|| x.UserGroup == 5 // 5 for ISM group    // JSL 06/27/2022 commented this line
                                || x.UserGroup == intUserGroup)
                                && x.ShipFleetID == 0   // JSL 07/01/2022
                                )
                                .ToList();

                // JSL 06/27/2022
                if (IsNeedToSendNotificationBaseOnShipFleetId)
                {
                    if (!string.IsNullOrEmpty(strFormType))
                    {
                        if (strFormType.ToUpper() == AppStatic.SIRForm)
                        {
                            if (!string.IsNullOrEmpty(strShipCode))
                            {
                                int ShipFleetID = (int)dbContext.CSShips.Where(x => x.Code == strShipCode).Select(y => y.FleetId).FirstOrDefault();
                                //UsersList = UsersList.Where(x => (x.UserGroup == intUserGroup && x.ShipFleetID == ShipFleetID) || x.UserGroup == 1).ToList(); // JSL 07/01/2022 commented this line
                                // JSL 07/01/2022
                                UsersList = dbContext.UserProfiles.Where(x => (x.UserGroup == intUserGroup || x.UserGroup == 1) && x.ShipFleetID == 0).ToList();
                                UsersList.AddRange(dbContext.UserProfiles.Where(x => (x.UserGroup == intUserGroup) && x.ShipFleetID == ShipFleetID).ToList());
                                // End JSL 07/01/2022
                            }
                        }
                    }
                }
                // End JSL 06/27/2022

                // send to user
                foreach (var item in UsersList)
                {
                    Entity.Notification entityModelNotification = new Entity.Notification();
                    entityModelNotification.UniqueId = Guid.NewGuid();
                    entityModelNotification.UniqueDataId = guidUniqueDataId;
                    entityModelNotification.DataType = strDataType;
                    entityModelNotification.IsDraft = blnIsDraft;
                    entityModelNotification.Title = strTitle;
                    entityModelNotification.DetailsURL = strDetailsURL;
                    entityModelNotification.SentToUserId = item.UserID;
                    entityModelNotification.UserGroup = item.UserGroup;
                    entityModelNotification.CreatedDateTime = Utility.ToDateTimeUtcNow();
                    dbContext.Notifications.Add(entityModelNotification);
                }
                dbContext.SaveChanges();
            }
        }
        // End JSL 06/24/2022

        // JSL 06/25/2022
        public static void SendNotificationsForSignalR(
                string strUserID
                , string strEmail = ""
                , bool blnSendNotificationToUserForForm = false
            )
        {
            if (string.IsNullOrEmpty(strEmail))
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Guid UserGUID = Guid.Parse(strUserID);
                strEmail = dbContext.UserProfiles.Where(x => x.UserID == UserGUID).Select(y => y.Email).FirstOrDefault();
            }

            Dictionary<string, string> dicMetadata = new Dictionary<string, string>();
            if (blnSendNotificationToUserForForm)
            {
                Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                dictMetaData["CurrentUserId"] = strUserID;
                dictMetaData["strAction"] = AppStatic.API_GETNOTIFICATION;

                Dictionary<string, string> dictFormsNotificationData = new Dictionary<string, string>();
                NotificationsHelper _helper = new NotificationsHelper();
                dictFormsNotificationData = _helper.PerformPostAPICall(dictMetaData);

                //dicMetadata["NumberOfFormsNotifications"] = dictFormsNotificationData["NumberOfFormsNotifications"].ToString();
                dicMetadata["FormsNotificationsList"] = dictFormsNotificationData["CurrentUserNotifications"];
            }

            Dictionary<string, object> dictDeficienciesNotificationData = new Dictionary<string, object>();
            dictDeficienciesNotificationData = GetNotificationsByUser(strUserID);
            dicMetadata["NumberOfDeficienciesNotifications"] = dictDeficienciesNotificationData["NumberOfDeficienciesNotifications"].ToString();
            dicMetadata["DeficienciesNotificationsList"] = JsonConvert.SerializeObject(dictDeficienciesNotificationData["DeficienciesNotificationsList"]);

            dicMetadata["SendNotificationToUserID"] = strUserID;   // JSL 06/30/2022
            dicMetadata["SendNotificationToUser"] = strEmail;
            dicMetadata["SendNotificationFor"] = AppStatic.NotificationType;
            OfficeAPIHelper.PostAsyncOfficeAPICall(AppStatic.API_SIGNALRNOTIFICATION_CONTROLLER, AppStatic.API_SIGNALRNOTIFICATION_ACTION, dicMetadata);
        }
        // End JSL 06/25/2022
        #endregion
    }
}
