using CarisbrookeShippingAPI.BLL.Helpers.OfficeHelper;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class CloudSIRHelper
    {
        public bool SIRSynch(Modals.SIRModal Modal)
        {
            bool res = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.SuperintendedInspectionReport dbModal = new Entity.SuperintendedInspectionReport();
                bool IsNeedToUpdateSubTableData = false;    // JSL 04/20/2022
                bool blnSendNotificationToUserForForm = false;    // JSL 05/01/2022
                bool IsNeedToSendNotification = false;  // JSL 06/25/2022 this is send notification to all users

                if (Modal != null && Modal.SuperintendedInspectionReport.UniqueFormID != null)
                {
                    // RDBJ 12/17/2021 wrapped in if
                    if (Modal.SuperintendedInspectionReport.UniqueFormID != null)
                    {
                        dbModal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == Modal.SuperintendedInspectionReport.UniqueFormID).FirstOrDefault();

                        if (dbModal == null)
                            dbModal = new Entity.SuperintendedInspectionReport();

                        // JSL 04/20/2022 commented
                        /*
                        SetSIRFormData(ref dbModal, Modal);
                        dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion; // RDBJ 01/05/2022 set it before if from outer if
                        dbModal.IsSynced = true; // RDBJ 01/05/2022 set it before if from outer if
                        dbModal.isDelete = (int)(Modal.SuperintendedInspectionReport.isDelete); // RDBJ 01/05/2022
                        */
                        // End JSL 04/20/2022 commented

                        // JSL 12/31/2022
                        if (Modal.SuperintendedInspectionReport.ShipID == null || Modal.SuperintendedInspectionReport.ShipID == 0)
                        {
                            var dbships = dbContext.CSShips.Where(x => x.Code == Modal.SuperintendedInspectionReport.ShipName).FirstOrDefault();
                            if (dbships != null)
                            {
                                Modal.SuperintendedInspectionReport.ShipID = dbships.ShipId;
                            }
                        }
                        // End JSL 12/31/2022

                        if (dbModal != null && dbModal.UniqueFormID != null)
                        {
                            //dbContext.SaveChanges();  // JSL 04/20/2022 commented this line

                            // JSL 04/20/2022
                            int IsLocalFormVersionLatest = Decimal.Compare((decimal)Modal.SuperintendedInspectionReport.FormVersion, (decimal)dbModal.FormVersion);
                            if (IsLocalFormVersionLatest == 1)
                            {
                                // JSL 05/01/2022
                                if (Modal.SuperintendedInspectionReport.SavedAsDraft == false && dbModal.SavedAsDraft == true)
                                {
                                    blnSendNotificationToUserForForm = true;
                                }
                                // End JSL 05/01/2022

                                SetSIRFormData(ref dbModal, Modal);
                                dbModal.IsSynced = true;
                                dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion;
                                dbModal.isDelete = (int)(Modal.SuperintendedInspectionReport.isDelete);
                                dbContext.SaveChanges();
                                IsNeedToUpdateSubTableData = true;
                            }
                            // End JSL 04/20/2022
                        }
                        else
                        {
                            // JSL 04/20/2022
                            SetSIRFormData(ref dbModal, Modal);
                            dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion;
                            dbModal.isDelete = (int)(Modal.SuperintendedInspectionReport.isDelete);
                            // End JSL 04/20/2022

                            dbModal.UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID;
                            dbContext.SuperintendedInspectionReports.Add(dbModal);
                            dbContext.SaveChanges();
                            IsNeedToUpdateSubTableData = true;
                            //blnSendNotificationToUser = true;   // JSL 05/01/2022

                            // JSL 06/27/2022
                            if (dbModal.SavedAsDraft == false)
                            {
                                blnSendNotificationToUserForForm = true;
                            }
                            // End JSL 06/27/2022
                        }

                        // JSL 04/20/2022 wrapped in if
                        if (IsNeedToUpdateSubTableData)
                        {
                            //RDBJ 09/28/2021
                            SIRNote_Save(Convert.ToString(dbModal.UniqueFormID), Modal.SIRNote);
                            SIRAdditionalNote_Save(Convert.ToString(dbModal.UniqueFormID), Modal.SIRAdditionalNote);
                            //End RDBJ 09/28/2021

                            SIRDeficiencies_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRDeficiencies
                                , ref IsNeedToSendNotification  // JSL 06/25/2022
                                );
                        }

                        // JSL 05/01/2022
                        if (blnSendNotificationToUserForForm)
                        {

                            // JSL 06/24/2022
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "7";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(dbModal.UniqueFormID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeForm;
                            dictNotificationData["IsDraft"] = Convert.ToString(dbModal.SavedAsDraft);
                            dictNotificationData["Title"] = AppStatic.SIRFormName;

                            string strDetailsURL = string.Empty;
                            if ((bool)dbModal.SavedAsDraft)
                            {
                                strDetailsURL = "SIRList/DetailsView?id=" + Convert.ToString(dbModal.UniqueFormID);
                            }
                            else
                            {
                                strDetailsURL = "Forms/GeneralInspectionReport";
                            }
                            dictNotificationData["DetailsURL"] = strDetailsURL;

                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData
                                , AppStatic.SIRForm // JSL 06/27/2022
                                , dbModal.ShipName  // JSL 06/27/2022
                                , true  // JSL 06/27/2022
                                );
                            // End JSL 06/24/2022

                            // JSL 06/24/2022 commented
                            /*
                            var UsersList = dbContext.UserProfiles
                                .Where(x => x.UserGroup == 1    // 1 for Admin group
                                || x.UserGroup == 5 // 5 for ISM group
                                || x.UserGroup == 7 // 7 for Technical group
                                )
                                .ToList();

                            // send to user
                            foreach (var item in UsersList)
                            {
                                Entity.Notification entityModelNotification = new Entity.Notification();
                                entityModelNotification.UniqueId = Guid.NewGuid();
                                entityModelNotification.UniqueDataId = dbModal.UniqueFormID;
                                entityModelNotification.DataType = AppStatic.NotificationTypeForm;
                                entityModelNotification.IsDraft = (bool)dbModal.SavedAsDraft;
                                entityModelNotification.Title = AppStatic.SIRFormName;

                                if (entityModelNotification.IsDraft)
                                {
                                    entityModelNotification.DetailsURL = "SIRList/DetailsView?id=" + entityModelNotification.UniqueDataId.ToString();
                                }
                                else
                                {
                                    entityModelNotification.DetailsURL = "Forms/GeneralInspectionReport";
                                }

                                entityModelNotification.SentToUserId = item.UserID;
                                entityModelNotification.UserGroup = item.UserGroup;
                                entityModelNotification.CreatedDateTime = Utility.ToDateTimeUtcNow();
                                dbContext.Notifications.Add(entityModelNotification);
                                dbContext.SaveChanges();
                            }
                            */
                            // End JSL 06/24/2022 commented
                        }
                        // End JSL 05/01/2022

                        // JSL 06/27/2022
                        if (blnSendNotificationToUserForForm    // JSL 07/16/2022
                            || IsNeedToSendNotification)
                        {
                            SendSignalRNotificationCallForTheOffice(dbModal.ShipName, blnSendNotificationToUserForForm);
                        }
                        // End JSL 06/27/2022

                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud SubmitSIR : " + ex.Message);
            }
            return res;
        }

        public void SetSIRFormData(ref Entity.SuperintendedInspectionReport dbModal, Modals.SIRModal Modal)
        {
            dbModal.CreatedDate = Modal.SuperintendedInspectionReport.CreatedDate;
            //dbModal.UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID; // RDBJ 01/15/2022 commented //RDBJ 09/24/2021
            dbModal.ShipID = Modal.SuperintendedInspectionReport.ShipID;
            dbModal.ShipName = Modal.SuperintendedInspectionReport.ShipName;
            dbModal.Date = Modal.SuperintendedInspectionReport.Date;
            dbModal.Port = Modal.SuperintendedInspectionReport.Port;
            dbModal.Master = Modal.SuperintendedInspectionReport.Master;
            dbModal.Superintended = Modal.SuperintendedInspectionReport.Superintended;
            dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion;
            dbModal.Section1_1_Condition = Modal.SuperintendedInspectionReport.Section1_1_Condition;
            dbModal.Section1_1_Comment = Modal.SuperintendedInspectionReport.Section1_1_Comment;
            dbModal.Section1_2_Condition = Modal.SuperintendedInspectionReport.Section1_2_Condition;
            dbModal.Section1_2_Comment = Modal.SuperintendedInspectionReport.Section1_2_Comment;
            dbModal.Section1_3_Condition = Modal.SuperintendedInspectionReport.Section1_3_Condition;
            dbModal.Section1_3_Comment = Modal.SuperintendedInspectionReport.Section1_3_Comment;
            dbModal.Section1_4_Condition = Modal.SuperintendedInspectionReport.Section1_4_Condition;
            dbModal.Section1_4_Comment = Modal.SuperintendedInspectionReport.Section1_4_Comment;
            dbModal.Section1_5_Condition = Modal.SuperintendedInspectionReport.Section1_5_Condition;
            dbModal.Section1_5_Comment = Modal.SuperintendedInspectionReport.Section1_5_Comment;
            dbModal.Section1_6_Condition = Modal.SuperintendedInspectionReport.Section1_6_Condition;
            dbModal.Section1_6_Comment = Modal.SuperintendedInspectionReport.Section1_6_Comment;
            dbModal.Section1_7_Condition = Modal.SuperintendedInspectionReport.Section1_7_Condition;
            dbModal.Section1_7_Comment = Modal.SuperintendedInspectionReport.Section1_7_Comment;
            dbModal.Section1_8_Condition = Modal.SuperintendedInspectionReport.Section1_8_Condition;
            dbModal.Section1_8_Comment = Modal.SuperintendedInspectionReport.Section1_8_Comment;
            dbModal.Section1_9_Condition = Modal.SuperintendedInspectionReport.Section1_9_Condition;
            dbModal.Section1_9_Comment = Modal.SuperintendedInspectionReport.Section1_9_Comment;
            dbModal.Section1_10_Condition = Modal.SuperintendedInspectionReport.Section1_10_Condition;
            dbModal.Section1_10_Comment = Modal.SuperintendedInspectionReport.Section1_10_Comment;
            dbModal.Section1_11_Condition = Modal.SuperintendedInspectionReport.Section1_11_Condition;
            dbModal.Section1_11_Comment = Modal.SuperintendedInspectionReport.Section1_11_Comment;

            dbModal.Section2_1_Condition = Modal.SuperintendedInspectionReport.Section2_1_Condition;
            dbModal.Section2_1_Comment = Modal.SuperintendedInspectionReport.Section2_1_Comment;
            dbModal.Section2_2_Condition = Modal.SuperintendedInspectionReport.Section2_2_Condition;
            dbModal.Section2_2_Comment = Modal.SuperintendedInspectionReport.Section2_2_Comment;
            dbModal.Section2_3_Condition = Modal.SuperintendedInspectionReport.Section2_3_Condition;
            dbModal.Section2_3_Comment = Modal.SuperintendedInspectionReport.Section2_3_Comment;
            dbModal.Section2_4_Condition = Modal.SuperintendedInspectionReport.Section2_4_Condition;
            dbModal.Section2_4_Comment = Modal.SuperintendedInspectionReport.Section2_4_Comment;
            dbModal.Section2_5_Condition = Modal.SuperintendedInspectionReport.Section2_5_Condition;
            dbModal.Section2_5_Comment = Modal.SuperintendedInspectionReport.Section2_5_Comment;
            dbModal.Section2_6_Condition = Modal.SuperintendedInspectionReport.Section2_6_Condition;
            dbModal.Section2_6_Comment = Modal.SuperintendedInspectionReport.Section2_6_Comment;
            dbModal.Section2_7_Condition = Modal.SuperintendedInspectionReport.Section2_7_Condition;
            dbModal.Section2_7_Comment = Modal.SuperintendedInspectionReport.Section2_7_Comment;

            dbModal.Section3_1_Condition = Modal.SuperintendedInspectionReport.Section3_1_Condition;
            dbModal.Section3_1_Comment = Modal.SuperintendedInspectionReport.Section3_1_Comment;
            dbModal.Section3_2_Condition = Modal.SuperintendedInspectionReport.Section3_2_Condition;
            dbModal.Section3_2_Comment = Modal.SuperintendedInspectionReport.Section3_2_Comment;
            dbModal.Section3_3_Condition = Modal.SuperintendedInspectionReport.Section3_3_Condition;
            dbModal.Section3_3_Comment = Modal.SuperintendedInspectionReport.Section3_3_Comment;
            dbModal.Section3_4_Condition = Modal.SuperintendedInspectionReport.Section3_4_Condition;
            dbModal.Section3_4_Comment = Modal.SuperintendedInspectionReport.Section3_4_Comment;
            dbModal.Section3_5_Condition = Modal.SuperintendedInspectionReport.Section3_5_Condition;
            dbModal.Section3_5_Comment = Modal.SuperintendedInspectionReport.Section3_5_Comment;

            dbModal.Section4_1_Condition = Modal.SuperintendedInspectionReport.Section4_1_Condition;
            dbModal.Section4_1_Comment = Modal.SuperintendedInspectionReport.Section4_1_Comment;
            dbModal.Section4_2_Condition = Modal.SuperintendedInspectionReport.Section4_2_Condition;
            dbModal.Section4_2_Comment = Modal.SuperintendedInspectionReport.Section4_2_Comment;
            dbModal.Section4_3_Condition = Modal.SuperintendedInspectionReport.Section4_3_Condition;
            dbModal.Section4_3_Comment = Modal.SuperintendedInspectionReport.Section4_3_Comment;

            dbModal.Section5_1_Condition = Modal.SuperintendedInspectionReport.Section5_1_Condition;
            dbModal.Section5_1_Comment = Modal.SuperintendedInspectionReport.Section5_1_Comment;
            dbModal.Section5_6_Condition = Modal.SuperintendedInspectionReport.Section5_6_Condition;
            dbModal.Section5_6_Comment = Modal.SuperintendedInspectionReport.Section5_6_Comment;
            dbModal.Section5_8_Condition = Modal.SuperintendedInspectionReport.Section5_8_Condition;
            dbModal.Section5_8_Comment = Modal.SuperintendedInspectionReport.Section5_8_Comment;
            dbModal.Section5_9_Condition = Modal.SuperintendedInspectionReport.Section5_9_Condition;
            dbModal.Section5_9_Comment = Modal.SuperintendedInspectionReport.Section5_9_Comment;

            dbModal.Section6_1_Condition = Modal.SuperintendedInspectionReport.Section6_1_Condition;
            dbModal.Section6_1_Comment = Modal.SuperintendedInspectionReport.Section6_1_Comment;
            dbModal.Section6_2_Condition = Modal.SuperintendedInspectionReport.Section6_2_Condition;
            dbModal.Section6_2_Comment = Modal.SuperintendedInspectionReport.Section6_2_Comment;
            dbModal.Section6_3_Condition = Modal.SuperintendedInspectionReport.Section6_3_Condition;
            dbModal.Section6_3_Comment = Modal.SuperintendedInspectionReport.Section6_3_Comment;
            dbModal.Section6_4_Condition = Modal.SuperintendedInspectionReport.Section6_4_Condition;
            dbModal.Section6_4_Comment = Modal.SuperintendedInspectionReport.Section6_4_Comment;
            dbModal.Section6_5_Condition = Modal.SuperintendedInspectionReport.Section6_5_Condition;
            dbModal.Section6_5_Comment = Modal.SuperintendedInspectionReport.Section6_5_Comment;
            dbModal.Section6_6_Condition = Modal.SuperintendedInspectionReport.Section6_6_Condition;
            dbModal.Section6_6_Comment = Modal.SuperintendedInspectionReport.Section6_6_Comment;
            dbModal.Section6_7_Condition = Modal.SuperintendedInspectionReport.Section6_7_Condition;
            dbModal.Section6_7_Comment = Modal.SuperintendedInspectionReport.Section6_7_Comment;
            dbModal.Section6_8_Condition = Modal.SuperintendedInspectionReport.Section6_8_Condition;
            dbModal.Section6_8_Comment = Modal.SuperintendedInspectionReport.Section6_8_Comment;

            dbModal.Section7_1_Condition = Modal.SuperintendedInspectionReport.Section7_1_Condition;
            dbModal.Section7_1_Comment = Modal.SuperintendedInspectionReport.Section7_1_Comment;
            dbModal.Section7_2_Condition = Modal.SuperintendedInspectionReport.Section7_2_Condition;
            dbModal.Section7_2_Comment = Modal.SuperintendedInspectionReport.Section7_2_Comment;
            dbModal.Section7_3_Condition = Modal.SuperintendedInspectionReport.Section7_3_Condition;
            dbModal.Section7_3_Comment = Modal.SuperintendedInspectionReport.Section7_3_Comment;
            dbModal.Section7_4_Condition = Modal.SuperintendedInspectionReport.Section7_4_Condition;
            dbModal.Section7_4_Comment = Modal.SuperintendedInspectionReport.Section7_4_Comment;
            dbModal.Section7_5_Condition = Modal.SuperintendedInspectionReport.Section7_5_Condition;
            dbModal.Section7_5_Comment = Modal.SuperintendedInspectionReport.Section7_5_Comment;
            dbModal.Section7_6_Condition = Modal.SuperintendedInspectionReport.Section7_6_Condition;
            dbModal.Section7_6_Comment = Modal.SuperintendedInspectionReport.Section7_6_Comment;

            dbModal.Section8_1_Condition = Modal.SuperintendedInspectionReport.Section8_1_Condition;
            dbModal.Section8_1_Comment = Modal.SuperintendedInspectionReport.Section8_1_Comment;
            dbModal.Section8_2_Condition = Modal.SuperintendedInspectionReport.Section8_2_Condition;
            dbModal.Section8_2_Comment = Modal.SuperintendedInspectionReport.Section8_2_Comment;
            dbModal.Section8_3_Condition = Modal.SuperintendedInspectionReport.Section8_3_Condition;
            dbModal.Section8_3_Comment = Modal.SuperintendedInspectionReport.Section8_3_Comment;
            dbModal.Section8_4_Condition = Modal.SuperintendedInspectionReport.Section8_4_Condition;
            dbModal.Section8_4_Comment = Modal.SuperintendedInspectionReport.Section8_4_Comment;
            dbModal.Section8_5_Condition = Modal.SuperintendedInspectionReport.Section8_5_Condition;
            dbModal.Section8_5_Comment = Modal.SuperintendedInspectionReport.Section8_5_Comment;
            dbModal.Section8_6_Condition = Modal.SuperintendedInspectionReport.Section8_6_Condition;
            dbModal.Section8_6_Comment = Modal.SuperintendedInspectionReport.Section8_6_Comment;
            dbModal.Section8_7_Condition = Modal.SuperintendedInspectionReport.Section8_7_Condition;
            dbModal.Section8_7_Comment = Modal.SuperintendedInspectionReport.Section8_7_Comment;
            dbModal.Section8_8_Condition = Modal.SuperintendedInspectionReport.Section8_8_Condition;
            dbModal.Section8_8_Comment = Modal.SuperintendedInspectionReport.Section8_8_Comment;
            dbModal.Section8_9_Condition = Modal.SuperintendedInspectionReport.Section8_9_Condition;
            dbModal.Section8_9_Comment = Modal.SuperintendedInspectionReport.Section8_9_Comment;
            dbModal.Section8_10_Condition = Modal.SuperintendedInspectionReport.Section8_10_Condition;
            dbModal.Section8_10_Comment = Modal.SuperintendedInspectionReport.Section8_10_Comment;
            dbModal.Section8_11_Condition = Modal.SuperintendedInspectionReport.Section8_11_Condition;
            dbModal.Section8_11_Comment = Modal.SuperintendedInspectionReport.Section8_11_Comment;
            dbModal.Section8_12_Condition = Modal.SuperintendedInspectionReport.Section8_12_Condition;
            dbModal.Section8_12_Comment = Modal.SuperintendedInspectionReport.Section8_12_Comment;
            dbModal.Section8_13_Condition = Modal.SuperintendedInspectionReport.Section8_13_Condition;
            dbModal.Section8_13_Comment = Modal.SuperintendedInspectionReport.Section8_13_Comment;
            dbModal.Section8_14_Condition = Modal.SuperintendedInspectionReport.Section8_14_Condition;
            dbModal.Section8_14_Comment = Modal.SuperintendedInspectionReport.Section8_14_Comment;
            dbModal.Section8_15_Condition = Modal.SuperintendedInspectionReport.Section8_15_Condition;
            dbModal.Section8_15_Comment = Modal.SuperintendedInspectionReport.Section8_15_Comment;
            dbModal.Section8_16_Condition = Modal.SuperintendedInspectionReport.Section8_16_Condition;
            dbModal.Section8_16_Comment = Modal.SuperintendedInspectionReport.Section8_16_Comment;
            dbModal.Section8_17_Condition = Modal.SuperintendedInspectionReport.Section8_17_Condition;
            dbModal.Section8_17_Comment = Modal.SuperintendedInspectionReport.Section8_17_Comment;
            dbModal.Section8_18_Condition = Modal.SuperintendedInspectionReport.Section8_18_Condition;
            dbModal.Section8_18_Comment = Modal.SuperintendedInspectionReport.Section8_18_Comment;
            dbModal.Section8_19_Condition = Modal.SuperintendedInspectionReport.Section8_19_Condition;
            dbModal.Section8_19_Comment = Modal.SuperintendedInspectionReport.Section8_19_Comment;
            dbModal.Section8_20_Condition = Modal.SuperintendedInspectionReport.Section8_20_Condition;
            dbModal.Section8_20_Comment = Modal.SuperintendedInspectionReport.Section8_20_Comment;
            dbModal.Section8_21_Condition = Modal.SuperintendedInspectionReport.Section8_21_Condition;
            dbModal.Section8_21_Comment = Modal.SuperintendedInspectionReport.Section8_21_Comment;
            dbModal.Section8_22_Condition = Modal.SuperintendedInspectionReport.Section8_22_Condition;
            dbModal.Section8_22_Comment = Modal.SuperintendedInspectionReport.Section8_22_Comment;
            dbModal.Section8_23_Condition = Modal.SuperintendedInspectionReport.Section8_23_Condition;
            dbModal.Section8_23_Comment = Modal.SuperintendedInspectionReport.Section8_23_Comment;
            dbModal.Section8_24_Condition = Modal.SuperintendedInspectionReport.Section8_24_Condition;
            dbModal.Section8_24_Comment = Modal.SuperintendedInspectionReport.Section8_24_Comment;
            dbModal.Section8_25_Condition = Modal.SuperintendedInspectionReport.Section8_25_Condition;
            dbModal.Section8_25_Comment = Modal.SuperintendedInspectionReport.Section8_25_Comment;

            dbModal.Section9_1_Condition = Modal.SuperintendedInspectionReport.Section9_1_Condition;
            dbModal.Section9_1_Comment = Modal.SuperintendedInspectionReport.Section9_1_Comment;
            dbModal.Section9_2_Condition = Modal.SuperintendedInspectionReport.Section9_2_Condition;
            dbModal.Section9_2_Comment = Modal.SuperintendedInspectionReport.Section9_2_Comment;
            dbModal.Section9_3_Condition = Modal.SuperintendedInspectionReport.Section9_3_Condition;
            dbModal.Section9_3_Comment = Modal.SuperintendedInspectionReport.Section9_3_Comment;
            dbModal.Section9_4_Condition = Modal.SuperintendedInspectionReport.Section9_4_Condition;
            dbModal.Section9_4_Comment = Modal.SuperintendedInspectionReport.Section9_4_Comment;
            dbModal.Section9_5_Condition = Modal.SuperintendedInspectionReport.Section9_5_Condition;
            dbModal.Section9_5_Comment = Modal.SuperintendedInspectionReport.Section9_5_Comment;
            dbModal.Section9_6_Condition = Modal.SuperintendedInspectionReport.Section9_6_Condition;
            dbModal.Section9_6_Comment = Modal.SuperintendedInspectionReport.Section9_6_Comment;
            dbModal.Section9_7_Condition = Modal.SuperintendedInspectionReport.Section9_7_Condition;
            dbModal.Section9_7_Comment = Modal.SuperintendedInspectionReport.Section9_7_Comment;
            dbModal.Section9_8_Condition = Modal.SuperintendedInspectionReport.Section9_8_Condition;
            dbModal.Section9_8_Comment = Modal.SuperintendedInspectionReport.Section9_8_Comment;
            dbModal.Section9_9_Condition = Modal.SuperintendedInspectionReport.Section9_9_Condition;
            dbModal.Section9_9_Comment = Modal.SuperintendedInspectionReport.Section9_9_Comment;
            dbModal.Section9_10_Condition = Modal.SuperintendedInspectionReport.Section9_10_Condition;
            dbModal.Section9_10_Comment = Modal.SuperintendedInspectionReport.Section9_10_Comment;
            dbModal.Section9_11_Condition = Modal.SuperintendedInspectionReport.Section9_11_Condition;
            dbModal.Section9_11_Comment = Modal.SuperintendedInspectionReport.Section9_11_Comment;
            dbModal.Section9_12_Condition = Modal.SuperintendedInspectionReport.Section9_12_Condition;
            dbModal.Section9_12_Comment = Modal.SuperintendedInspectionReport.Section9_12_Comment;
            dbModal.Section9_13_Condition = Modal.SuperintendedInspectionReport.Section9_13_Condition;
            dbModal.Section9_13_Comment = Modal.SuperintendedInspectionReport.Section9_13_Comment;
            dbModal.Section9_14_Condition = Modal.SuperintendedInspectionReport.Section9_14_Condition;
            dbModal.Section9_14_Comment = Modal.SuperintendedInspectionReport.Section9_14_Comment;
            dbModal.Section9_15_Condition = Modal.SuperintendedInspectionReport.Section9_15_Condition;
            dbModal.Section9_15_Comment = Modal.SuperintendedInspectionReport.Section9_15_Comment;

            // RDBJ 02/15/2022
            dbModal.Section9_16_Condition = Modal.SuperintendedInspectionReport.Section9_16_Condition;
            dbModal.Section9_16_Comment = Modal.SuperintendedInspectionReport.Section9_16_Comment;
            dbModal.Section9_17_Condition = Modal.SuperintendedInspectionReport.Section9_17_Condition;
            dbModal.Section9_17_Comment = Modal.SuperintendedInspectionReport.Section9_17_Comment;
            // End RDBJ 02/15/2022

            dbModal.Section10_1_Condition = Modal.SuperintendedInspectionReport.Section10_1_Condition;
            dbModal.Section10_1_Comment = Modal.SuperintendedInspectionReport.Section10_1_Comment;
            dbModal.Section10_2_Condition = Modal.SuperintendedInspectionReport.Section10_2_Condition;
            dbModal.Section10_2_Comment = Modal.SuperintendedInspectionReport.Section10_2_Comment;
            dbModal.Section10_3_Condition = Modal.SuperintendedInspectionReport.Section10_3_Condition;
            dbModal.Section10_3_Comment = Modal.SuperintendedInspectionReport.Section10_3_Comment;
            dbModal.Section10_4_Condition = Modal.SuperintendedInspectionReport.Section10_4_Condition;
            dbModal.Section10_4_Comment = Modal.SuperintendedInspectionReport.Section10_4_Comment;
            dbModal.Section10_5_Condition = Modal.SuperintendedInspectionReport.Section10_5_Condition;
            dbModal.Section10_5_Comment = Modal.SuperintendedInspectionReport.Section10_5_Comment;
            dbModal.Section10_6_Condition = Modal.SuperintendedInspectionReport.Section10_6_Condition;
            dbModal.Section10_6_Comment = Modal.SuperintendedInspectionReport.Section10_6_Comment;
            dbModal.Section10_7_Condition = Modal.SuperintendedInspectionReport.Section10_7_Condition;
            dbModal.Section10_7_Comment = Modal.SuperintendedInspectionReport.Section10_7_Comment;
            dbModal.Section10_8_Condition = Modal.SuperintendedInspectionReport.Section10_8_Condition;
            dbModal.Section10_8_Comment = Modal.SuperintendedInspectionReport.Section10_8_Comment;
            dbModal.Section10_9_Condition = Modal.SuperintendedInspectionReport.Section10_9_Condition;
            dbModal.Section10_9_Comment = Modal.SuperintendedInspectionReport.Section10_9_Comment;
            dbModal.Section10_10_Condition = Modal.SuperintendedInspectionReport.Section10_10_Condition;
            dbModal.Section10_10_Comment = Modal.SuperintendedInspectionReport.Section10_10_Comment;
            dbModal.Section10_11_Condition = Modal.SuperintendedInspectionReport.Section10_11_Condition;
            dbModal.Section10_11_Comment = Modal.SuperintendedInspectionReport.Section10_11_Comment;
            dbModal.Section10_12_Condition = Modal.SuperintendedInspectionReport.Section10_12_Condition;
            dbModal.Section10_12_Comment = Modal.SuperintendedInspectionReport.Section10_12_Comment;
            dbModal.Section10_13_Condition = Modal.SuperintendedInspectionReport.Section10_13_Condition;
            dbModal.Section10_13_Comment = Modal.SuperintendedInspectionReport.Section10_13_Comment;
            dbModal.Section10_14_Condition = Modal.SuperintendedInspectionReport.Section10_14_Condition;
            dbModal.Section10_14_Comment = Modal.SuperintendedInspectionReport.Section10_14_Comment;
            dbModal.Section10_15_Condition = Modal.SuperintendedInspectionReport.Section10_15_Condition;
            dbModal.Section10_15_Comment = Modal.SuperintendedInspectionReport.Section10_15_Comment;
            dbModal.Section10_16_Condition = Modal.SuperintendedInspectionReport.Section10_16_Condition;
            dbModal.Section10_16_Comment = Modal.SuperintendedInspectionReport.Section10_16_Comment;

            dbModal.Section11_1_Condition = Modal.SuperintendedInspectionReport.Section11_1_Condition;
            dbModal.Section11_1_Comment = Modal.SuperintendedInspectionReport.Section11_1_Comment;
            dbModal.Section11_2_Condition = Modal.SuperintendedInspectionReport.Section11_2_Condition;
            dbModal.Section11_2_Comment = Modal.SuperintendedInspectionReport.Section11_2_Comment;
            dbModal.Section11_3_Condition = Modal.SuperintendedInspectionReport.Section11_3_Condition;
            dbModal.Section11_3_Comment = Modal.SuperintendedInspectionReport.Section11_3_Comment;
            dbModal.Section11_4_Condition = Modal.SuperintendedInspectionReport.Section11_4_Condition;
            dbModal.Section11_4_Comment = Modal.SuperintendedInspectionReport.Section11_4_Comment;
            dbModal.Section11_5_Condition = Modal.SuperintendedInspectionReport.Section11_5_Condition;
            dbModal.Section11_5_Comment = Modal.SuperintendedInspectionReport.Section11_5_Comment;
            dbModal.Section11_6_Condition = Modal.SuperintendedInspectionReport.Section11_6_Condition;
            dbModal.Section11_6_Comment = Modal.SuperintendedInspectionReport.Section11_6_Comment;
            dbModal.Section11_7_Condition = Modal.SuperintendedInspectionReport.Section11_7_Condition;
            dbModal.Section11_7_Comment = Modal.SuperintendedInspectionReport.Section11_7_Comment;
            dbModal.Section11_8_Condition = Modal.SuperintendedInspectionReport.Section11_8_Condition;
            dbModal.Section11_8_Comment = Modal.SuperintendedInspectionReport.Section11_8_Comment;

            dbModal.Section12_1_Condition = Modal.SuperintendedInspectionReport.Section12_1_Condition;
            dbModal.Section12_1_Comment = Modal.SuperintendedInspectionReport.Section12_1_Comment;
            dbModal.Section12_2_Condition = Modal.SuperintendedInspectionReport.Section12_2_Condition;
            dbModal.Section12_2_Comment = Modal.SuperintendedInspectionReport.Section12_2_Comment;
            dbModal.Section12_3_Condition = Modal.SuperintendedInspectionReport.Section12_3_Condition;
            dbModal.Section12_3_Comment = Modal.SuperintendedInspectionReport.Section12_3_Comment;
            dbModal.Section12_4_Condition = Modal.SuperintendedInspectionReport.Section12_4_Condition;
            dbModal.Section12_4_Comment = Modal.SuperintendedInspectionReport.Section12_4_Comment;
            dbModal.Section12_5_Condition = Modal.SuperintendedInspectionReport.Section12_5_Condition;
            dbModal.Section12_5_Comment = Modal.SuperintendedInspectionReport.Section12_5_Comment;
            dbModal.Section12_6_Condition = Modal.SuperintendedInspectionReport.Section12_6_Condition;
            dbModal.Section12_6_Comment = Modal.SuperintendedInspectionReport.Section12_6_Comment;

            dbModal.Section13_1_Condition = Modal.SuperintendedInspectionReport.Section13_1_Condition;
            dbModal.Section13_1_Comment = Modal.SuperintendedInspectionReport.Section13_1_Comment;
            dbModal.Section13_2_Condition = Modal.SuperintendedInspectionReport.Section13_2_Condition;
            dbModal.Section13_2_Comment = Modal.SuperintendedInspectionReport.Section13_2_Comment;
            dbModal.Section13_3_Condition = Modal.SuperintendedInspectionReport.Section13_3_Condition;
            dbModal.Section13_3_Comment = Modal.SuperintendedInspectionReport.Section13_3_Comment;
            dbModal.Section13_4_Condition = Modal.SuperintendedInspectionReport.Section13_4_Condition;
            dbModal.Section13_4_Comment = Modal.SuperintendedInspectionReport.Section13_4_Comment;

            dbModal.Section14_1_Condition = Modal.SuperintendedInspectionReport.Section14_1_Condition;
            dbModal.Section14_1_Comment = Modal.SuperintendedInspectionReport.Section14_1_Comment;
            dbModal.Section14_2_Condition = Modal.SuperintendedInspectionReport.Section14_2_Condition;
            dbModal.Section14_2_Comment = Modal.SuperintendedInspectionReport.Section14_2_Comment;
            dbModal.Section14_3_Condition = Modal.SuperintendedInspectionReport.Section14_3_Condition;
            dbModal.Section14_3_Comment = Modal.SuperintendedInspectionReport.Section14_3_Comment;
            dbModal.Section14_4_Condition = Modal.SuperintendedInspectionReport.Section14_4_Condition;
            dbModal.Section14_4_Comment = Modal.SuperintendedInspectionReport.Section14_4_Comment;
            dbModal.Section14_5_Condition = Modal.SuperintendedInspectionReport.Section14_5_Condition;
            dbModal.Section14_5_Comment = Modal.SuperintendedInspectionReport.Section14_5_Comment;
            dbModal.Section14_6_Condition = Modal.SuperintendedInspectionReport.Section14_6_Condition;
            dbModal.Section14_6_Comment = Modal.SuperintendedInspectionReport.Section14_6_Comment;
            dbModal.Section14_7_Condition = Modal.SuperintendedInspectionReport.Section14_7_Condition;
            dbModal.Section14_7_Comment = Modal.SuperintendedInspectionReport.Section14_7_Comment;
            dbModal.Section14_8_Condition = Modal.SuperintendedInspectionReport.Section14_8_Condition;
            dbModal.Section14_8_Comment = Modal.SuperintendedInspectionReport.Section14_8_Comment;
            dbModal.Section14_9_Condition = Modal.SuperintendedInspectionReport.Section14_9_Condition;
            dbModal.Section14_9_Comment = Modal.SuperintendedInspectionReport.Section14_9_Comment;
            dbModal.Section14_10_Condition = Modal.SuperintendedInspectionReport.Section14_10_Condition;
            dbModal.Section14_10_Comment = Modal.SuperintendedInspectionReport.Section14_10_Comment;
            dbModal.Section14_11_Condition = Modal.SuperintendedInspectionReport.Section14_11_Condition;
            dbModal.Section14_11_Comment = Modal.SuperintendedInspectionReport.Section14_11_Comment;
            dbModal.Section14_12_Condition = Modal.SuperintendedInspectionReport.Section14_12_Condition;
            dbModal.Section14_12_Comment = Modal.SuperintendedInspectionReport.Section14_12_Comment;
            dbModal.Section14_13_Condition = Modal.SuperintendedInspectionReport.Section14_13_Condition;
            dbModal.Section14_13_Comment = Modal.SuperintendedInspectionReport.Section14_13_Comment;
            dbModal.Section14_14_Condition = Modal.SuperintendedInspectionReport.Section14_14_Condition;
            dbModal.Section14_14_Comment = Modal.SuperintendedInspectionReport.Section14_14_Comment;
            dbModal.Section14_15_Condition = Modal.SuperintendedInspectionReport.Section14_15_Condition;
            dbModal.Section14_15_Comment = Modal.SuperintendedInspectionReport.Section14_15_Comment;
            dbModal.Section14_16_Condition = Modal.SuperintendedInspectionReport.Section14_16_Condition;
            dbModal.Section14_16_Comment = Modal.SuperintendedInspectionReport.Section14_16_Comment;
            dbModal.Section14_17_Condition = Modal.SuperintendedInspectionReport.Section14_17_Condition;
            dbModal.Section14_17_Comment = Modal.SuperintendedInspectionReport.Section14_17_Comment;
            dbModal.Section14_18_Condition = Modal.SuperintendedInspectionReport.Section14_18_Condition;
            dbModal.Section14_18_Comment = Modal.SuperintendedInspectionReport.Section14_18_Comment;
            dbModal.Section14_19_Condition = Modal.SuperintendedInspectionReport.Section14_19_Condition;
            dbModal.Section14_19_Comment = Modal.SuperintendedInspectionReport.Section14_19_Comment;
            dbModal.Section14_20_Condition = Modal.SuperintendedInspectionReport.Section14_20_Condition;
            dbModal.Section14_20_Comment = Modal.SuperintendedInspectionReport.Section14_20_Comment;
            dbModal.Section14_21_Condition = Modal.SuperintendedInspectionReport.Section14_21_Condition;
            dbModal.Section14_21_Comment = Modal.SuperintendedInspectionReport.Section14_21_Comment;
            dbModal.Section14_22_Condition = Modal.SuperintendedInspectionReport.Section14_22_Condition;
            dbModal.Section14_22_Comment = Modal.SuperintendedInspectionReport.Section14_22_Comment;
            dbModal.Section14_23_Condition = Modal.SuperintendedInspectionReport.Section14_23_Condition;
            dbModal.Section14_23_Comment = Modal.SuperintendedInspectionReport.Section14_23_Comment;
            dbModal.Section14_24_Condition = Modal.SuperintendedInspectionReport.Section14_24_Condition;
            dbModal.Section14_24_Comment = Modal.SuperintendedInspectionReport.Section14_24_Comment;
            dbModal.Section14_25_Condition = Modal.SuperintendedInspectionReport.Section14_25_Condition;
            dbModal.Section14_25_Comment = Modal.SuperintendedInspectionReport.Section14_25_Comment;

            dbModal.Section15_1_Condition = Modal.SuperintendedInspectionReport.Section15_1_Condition;
            dbModal.Section15_1_Comment = Modal.SuperintendedInspectionReport.Section15_1_Comment;
            dbModal.Section15_2_Condition = Modal.SuperintendedInspectionReport.Section15_2_Condition;
            dbModal.Section15_2_Comment = Modal.SuperintendedInspectionReport.Section15_2_Comment;
            dbModal.Section15_3_Condition = Modal.SuperintendedInspectionReport.Section15_3_Condition;
            dbModal.Section15_3_Comment = Modal.SuperintendedInspectionReport.Section15_3_Comment;
            dbModal.Section15_4_Condition = Modal.SuperintendedInspectionReport.Section15_4_Condition;
            dbModal.Section15_4_Comment = Modal.SuperintendedInspectionReport.Section15_4_Comment;
            dbModal.Section15_5_Condition = Modal.SuperintendedInspectionReport.Section15_5_Condition;
            dbModal.Section15_5_Comment = Modal.SuperintendedInspectionReport.Section15_5_Comment;
            dbModal.Section15_6_Condition = Modal.SuperintendedInspectionReport.Section15_6_Condition;
            dbModal.Section15_6_Comment = Modal.SuperintendedInspectionReport.Section15_6_Comment;
            dbModal.Section15_7_Condition = Modal.SuperintendedInspectionReport.Section15_7_Condition;
            dbModal.Section15_7_Comment = Modal.SuperintendedInspectionReport.Section15_7_Comment;
            dbModal.Section15_8_Condition = Modal.SuperintendedInspectionReport.Section15_8_Condition;
            dbModal.Section15_8_Comment = Modal.SuperintendedInspectionReport.Section15_8_Comment;
            dbModal.Section15_9_Condition = Modal.SuperintendedInspectionReport.Section15_9_Condition;
            dbModal.Section15_9_Comment = Modal.SuperintendedInspectionReport.Section15_9_Comment;
            dbModal.Section15_10_Condition = Modal.SuperintendedInspectionReport.Section15_10_Condition;
            dbModal.Section15_10_Comment = Modal.SuperintendedInspectionReport.Section15_10_Comment;
            dbModal.Section15_11_Condition = Modal.SuperintendedInspectionReport.Section15_11_Condition;
            dbModal.Section15_11_Comment = Modal.SuperintendedInspectionReport.Section15_11_Comment;
            dbModal.Section15_12_Condition = Modal.SuperintendedInspectionReport.Section15_12_Condition;
            dbModal.Section15_12_Comment = Modal.SuperintendedInspectionReport.Section15_12_Comment;
            dbModal.Section15_13_Condition = Modal.SuperintendedInspectionReport.Section15_13_Condition;
            dbModal.Section15_13_Comment = Modal.SuperintendedInspectionReport.Section15_13_Comment;
            dbModal.Section15_14_Condition = Modal.SuperintendedInspectionReport.Section15_14_Condition;
            dbModal.Section15_14_Comment = Modal.SuperintendedInspectionReport.Section15_14_Comment;
            dbModal.Section15_15_Condition = Modal.SuperintendedInspectionReport.Section15_15_Condition;
            dbModal.Section15_15_Comment = Modal.SuperintendedInspectionReport.Section15_15_Comment;

            dbModal.Section16_1_Condition = Modal.SuperintendedInspectionReport.Section16_1_Condition;
            dbModal.Section16_1_Comment = Modal.SuperintendedInspectionReport.Section16_1_Comment;
            dbModal.Section16_2_Condition = Modal.SuperintendedInspectionReport.Section16_2_Condition;
            dbModal.Section16_2_Comment = Modal.SuperintendedInspectionReport.Section16_2_Comment;
            dbModal.Section16_3_Condition = Modal.SuperintendedInspectionReport.Section16_3_Condition;
            dbModal.Section16_3_Comment = Modal.SuperintendedInspectionReport.Section16_3_Comment;
            dbModal.Section16_4_Condition = Modal.SuperintendedInspectionReport.Section16_4_Condition;
            dbModal.Section16_4_Comment = Modal.SuperintendedInspectionReport.Section16_4_Comment;

            dbModal.Section17_1_Condition = Modal.SuperintendedInspectionReport.Section17_1_Condition;
            dbModal.Section17_1_Comment = Modal.SuperintendedInspectionReport.Section17_1_Comment;
            dbModal.Section17_2_Condition = Modal.SuperintendedInspectionReport.Section17_2_Condition;
            dbModal.Section17_2_Comment = Modal.SuperintendedInspectionReport.Section17_2_Comment;
            dbModal.Section17_3_Condition = Modal.SuperintendedInspectionReport.Section17_3_Condition;
            dbModal.Section17_3_Comment = Modal.SuperintendedInspectionReport.Section17_3_Comment;
            dbModal.Section17_4_Condition = Modal.SuperintendedInspectionReport.Section17_4_Condition;
            dbModal.Section17_4_Comment = Modal.SuperintendedInspectionReport.Section17_4_Comment;
            dbModal.Section17_5_Condition = Modal.SuperintendedInspectionReport.Section17_5_Condition;
            dbModal.Section17_5_Comment = Modal.SuperintendedInspectionReport.Section17_5_Comment;
            dbModal.Section17_6_Condition = Modal.SuperintendedInspectionReport.Section17_6_Condition;
            dbModal.Section17_6_Comment = Modal.SuperintendedInspectionReport.Section17_6_Comment;

            dbModal.Section18_1_Condition = Modal.SuperintendedInspectionReport.Section18_1_Condition;
            dbModal.Section18_1_Comment = Modal.SuperintendedInspectionReport.Section18_1_Comment;
            dbModal.Section18_2_Condition = Modal.SuperintendedInspectionReport.Section18_2_Condition;
            dbModal.Section18_2_Comment = Modal.SuperintendedInspectionReport.Section18_2_Comment;
            dbModal.Section18_3_Condition = Modal.SuperintendedInspectionReport.Section18_3_Condition;
            dbModal.Section18_3_Comment = Modal.SuperintendedInspectionReport.Section18_3_Comment;
            dbModal.Section18_4_Condition = Modal.SuperintendedInspectionReport.Section18_4_Condition;
            dbModal.Section18_4_Comment = Modal.SuperintendedInspectionReport.Section18_4_Comment;
            dbModal.Section18_5_Condition = Modal.SuperintendedInspectionReport.Section18_5_Condition;
            dbModal.Section18_5_Comment = Modal.SuperintendedInspectionReport.Section18_5_Comment;
            dbModal.Section18_6_Condition = Modal.SuperintendedInspectionReport.Section18_6_Condition;
            dbModal.Section18_6_Comment = Modal.SuperintendedInspectionReport.Section18_6_Comment;
            dbModal.Section18_7_Condition = Modal.SuperintendedInspectionReport.Section18_7_Condition;
            dbModal.Section18_7_Comment = Modal.SuperintendedInspectionReport.Section18_7_Comment;

            // RDBJ 02/15/2022
            dbModal.Section18_8_Condition = Modal.SuperintendedInspectionReport.Section18_8_Condition;
            dbModal.Section18_8_Comment = Modal.SuperintendedInspectionReport.Section18_8_Comment;
            dbModal.Section18_9_Condition = Modal.SuperintendedInspectionReport.Section18_9_Condition;
            dbModal.Section18_9_Comment = Modal.SuperintendedInspectionReport.Section18_9_Comment;
            // End RDBJ 02/15/2022

            dbModal.IsSynced = Modal.SuperintendedInspectionReport.IsSynced;
            dbModal.CreatedDate = Modal.SuperintendedInspectionReport.CreatedDate;
            dbModal.ModifyDate = Modal.SuperintendedInspectionReport.ModifyDate;
            dbModal.SavedAsDraft = Modal.SuperintendedInspectionReport.SavedAsDraft;
        }

        //RDBJ 09/25/2021
        public void SIRDeficiencies_Save(string UniqueFormID, List<Modals.GIRDeficiencies> GIRDeficiencies
            , ref bool IsNeedToSendNotification // JSL 06/25/2022
            )
        {
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0 && Guid.Parse(UniqueFormID) != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    foreach (var item in GIRDeficiencies)
                    {
                        Entity.GIRDeficiency member = new Entity.GIRDeficiency();
                        member = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == item.DeficienciesUniqueID && (string.IsNullOrEmpty(x.ReportType) || x.ReportType == item.ReportType)).FirstOrDefault(); 

                        if (member == null || member.UniqueFormID == Guid.Empty)
                        {
                            member = new Entity.GIRDeficiency();

                            member.No = item.No;
                            member.DateRaised = item.DateRaised;
                            member.Deficiency = item.Deficiency;
                            member.DateClosed = item.DateClosed;
                            member.CreatedDate = item.CreatedDate;
                            member.UpdatedDate = item.UpdatedDate;
                            member.Ship = item.Ship;
                            member.IsClose = item.IsClose;
                            member.ReportType = item.ReportType;
                            member.ItemNo = item.ItemNo;
                            member.Section = item.Section;
                            member.UniqueFormID = item.UniqueFormID;
                            member.isDelete = item.isDelete;
                            member.DeficienciesUniqueID = item.DeficienciesUniqueID;
                            member.Priority = item.Priority == null ? 12 : item.Priority; //RDBJ 12/18/2021
                            member.AssignTo = item.AssignTo; // RDBJ 12/18/2021
                            member.DueDate = item.DueDate;  // RDBJ 03/01/2022

                            dbContext.GIRDeficiencies.Add(member);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            // JSL 09/09/2022
                            member.Ship = item.Ship;
                            member.ReportType = item.ReportType;
                            member.ItemNo = item.ItemNo;
                            member.Section = item.Section;
                            member.UniqueFormID = item.UniqueFormID;
                            // End JSL 09/09/2022

                            member.No = item.No;    // JSL 08/31/2022
                            member.DateRaised = item.DateRaised;
                            member.Deficiency = item.Deficiency;
                            member.DateClosed = item.DateClosed;
                            member.UpdatedDate = item.UpdatedDate;
                            member.IsClose = item.IsClose;
                            member.isDelete = item.isDelete;
                            member.DeficienciesUniqueID = item.DeficienciesUniqueID;
                            member.Priority = item.Priority == null ? 12 : item.Priority; //RDBJ 12/18/2021
                            member.AssignTo = item.AssignTo; // RDBJ 12/18/2021
                            member.DueDate = item.DueDate;  // RDBJ 03/01/2022

                            dbContext.SaveChanges();
                        }

                        // JSL 12/03/2022
                        Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                        dicFileMetaData["UniqueFormID"] = Convert.ToString(UniqueFormID);
                        dicFileMetaData["ReportType"] = item.ReportType;
                        dicFileMetaData["DetailUniqueId"] = Convert.ToString(item.DeficienciesUniqueID);
                        // End JSL 12/03/2022

                        if (item.GIRDeficienciesFile != null && item.GIRDeficienciesFile.Count > 0)
                        {
                            SIRDeficienciesFile_Save(item, item.Ship, item.DeficienciesUniqueID
                                , dicFileMetaData   // JSL 12/03/2022
                                );
                        }
                        if (item.GIRDeficienciesComments != null && item.GIRDeficienciesComments.Count > 0)
                        {
                            SIRDeficienciesComments_Save(item.GIRDeficienciesComments, item.DeficienciesUniqueID
                                    , item.Ship // JSL 06/27/2022
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/25/2022
                                );
                        }
                        if (item.GIRDeficienciesInitialActions != null && item.GIRDeficienciesInitialActions.Count > 0)
                        {
                            SIRDeficienciesInitialActions_Save(item.GIRDeficienciesInitialActions, item.DeficienciesUniqueID, item.UniqueFormID
                                    , item.Ship // JSL 06/27/2022
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/25/2022
                                );
                        }
                        if (item.GIRDeficienciesResolution != null && item.GIRDeficienciesResolution.Count > 0)
                        {
                            SIRDeficienciesResolution_Save(item.GIRDeficienciesResolution, item.DeficienciesUniqueID
                                    , item.Ship // JSL 06/27/2022
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/25/2022
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GIRDeficiencies_Save : " + ex.Message);
            }
        }
        //End RDBJ 09/25/2021

        //RDBJ 09/25/2021
        public void SIRDeficienciesFile_Save(GIRDeficiencies modal, string Ship, Guid? DeficienciesUniqueID
            , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                List<Entity.GIRDeficienciesFile> dbdeficienciesFile = new List<Entity.GIRDeficienciesFile>();
                dbdeficienciesFile = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == DeficienciesUniqueID).ToList();

                if (dbdeficienciesFile != null && dbdeficienciesFile.Count > 0)
                {
                    foreach (var item in dbdeficienciesFile)
                    {
                        if (DeficienciesUniqueID != null)
                        {
                            dbContext.GIRDeficienciesFiles.Remove(item);
                            dbContext.SaveChanges();
                        }
                    }
                }

                foreach (var item in modal.GIRDeficienciesFile)
                {
                    Entity.GIRDeficienciesFile file = new Entity.GIRDeficienciesFile();
                    file.DeficienciesFileUniqueID = (Guid)item.DeficienciesFileUniqueID;  // JSL 06/07/2022
                    file.FileName = item.FileName;
                    file.StorePath = item.StorePath;

                    // JSL 12/03/2022
                    if (file.StorePath.StartsWith("data:"))
                    {
                        dicFileMetaData["FileName"] = file.FileName;
                        dicFileMetaData["Base64FileData"] = file.StorePath;

                        file.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                    }
                    // End JSL 12/03/2022

                    file.DeficienciesUniqueID = DeficienciesUniqueID;
                    dbContext.GIRDeficienciesFiles.Add(file);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud SIRDeficienciesFile_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void SIRDeficienciesComments_Save(List<Modals.DeficienciesNote> modalDefNotes, Guid? DeficienciesID
                , string strShipCode    // JSL 06/27/2022
                , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
                , ref bool IsNeedToSendNotification // JSL 06/25/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modalDefNotes != null && modalDefNotes.Count > 0)
                {
                    foreach (var itemDefNotes in modalDefNotes)
                    {
                        Entity.GIRDeficienciesNote dbModal = new Entity.GIRDeficienciesNote();
                        dbModal = dbContext.GIRDeficienciesNotes.Where(x => x.NoteUniqueID == itemDefNotes.NoteUniqueID).FirstOrDefault();

                        if (dbModal == null)
                        {
                            Entity.GIRDeficienciesNote defNotes = new Entity.GIRDeficienciesNote();
                            defNotes.DeficienciesID = itemDefNotes.DeficienciesID;
                            defNotes.UserName = itemDefNotes.UserName;
                            defNotes.Comment = itemDefNotes.Comment;
                            defNotes.CreatedDate = itemDefNotes.CreatedDate;
                            defNotes.ModifyDate = itemDefNotes.ModifyDate;
                            defNotes.NoteUniqueID = itemDefNotes.NoteUniqueID;
                            defNotes.DeficienciesUniqueID = itemDefNotes.DeficienciesUniqueID;
                            //defNotes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/14/2021
                            defNotes.isNew = 0; // JSL 06/27/2022 //RDBJ 10/14/2021

                            dbContext.GIRDeficienciesNotes.Add(defNotes);
                            dbContext.SaveChanges();

                            // JSL 06/24/2022
                            IsNeedToSendNotification = true;
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "7";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeComment;
                            dictNotificationData["IsDraft"] = Convert.ToString(true);
                            dictNotificationData["Title"] = AppStatic.SIRFormName;
                            dictNotificationData["DetailsURL"] = "GIRList/DeficienciesDetails?id=" + Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData
                                , AppStatic.SIRForm // JSL 06/27/2022
                                , strShipCode  // JSL 06/27/2022
                                , true  // JSL 06/27/2022
                                );
                            // End JSL 06/24/2022
                        }

                        if (itemDefNotes.GIRDeficienciesCommentFile != null && itemDefNotes.GIRDeficienciesCommentFile.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefNotes.GIRDeficienciesCommentFile)
                            {
                                Entity.GIRDeficienciesCommentFile commentFile = new Entity.GIRDeficienciesCommentFile();
                                commentFile = dbContext.GIRDeficienciesCommentFiles.Where(x => x.CommentFileUniqueID == itemCommentFiles.CommentFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.GIRDeficienciesCommentFile defNotesFile = new Entity.GIRDeficienciesCommentFile();
                                    defNotesFile.DeficienciesID = itemCommentFiles.DeficienciesID;
                                    defNotesFile.FileName = itemCommentFiles.FileName;
                                    defNotesFile.StorePath = itemCommentFiles.StorePath;

                                    // JSL 12/03/2022
                                    if (defNotesFile.StorePath.StartsWith("data:"))
                                    {
                                        dicFileMetaData["FileName"] = defNotesFile.FileName;
                                        dicFileMetaData["Base64FileData"] = defNotesFile.StorePath;

                                        // JSL 01/08/2023
                                        dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeComment;
                                        dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentFiles.NoteUniqueID);
                                        // End JSL 01/08/2023

                                        defNotesFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                    }
                                    // End JSL 12/03/2022

                                    defNotesFile.IsUpload = itemCommentFiles.IsUpload;
                                    defNotesFile.NoteUniqueID = itemCommentFiles.NoteUniqueID;
                                    defNotesFile.CommentFileUniqueID = itemCommentFiles.CommentFileUniqueID;
                                    dbContext.GIRDeficienciesCommentFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud SIRDeficienciesComments_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void SIRDeficienciesInitialActions_Save(List<Modals.GIRDeficienciesInitialActions> modalDefNotes, Guid? DeficienciesID, Guid? UniqueFormId //RDBJ 09/22/2021 Updateed Modal Name used
            , string strShipCode    // JSL 06/27/2022    
            , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
            , ref bool IsNeedToSendNotification // JSL 06/25/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                // JSL 05/23/2022
                bool IsFormDraft = false;
                Entity.SuperintendedInspectionReport SIRModal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == UniqueFormId).FirstOrDefault();
                IsFormDraft = SIRModal.SavedAsDraft ?? false;
                // End JSL 05/23/2022

                if (modalDefNotes != null && modalDefNotes.Count > 0)
                {
                    foreach (var itemDefNotes in modalDefNotes)
                    {
                        Entity.GIRDeficienciesInitialAction dbModal = new Entity.GIRDeficienciesInitialAction();
                        dbModal = dbContext.GIRDeficienciesInitialActions.Where(x => x.IniActUniqueID == itemDefNotes.IniActUniqueID).FirstOrDefault();

                        if (dbModal == null)
                        {
                            Entity.GIRDeficienciesInitialAction defNotes = new Entity.GIRDeficienciesInitialAction();
                            defNotes.DeficienciesID = itemDefNotes.DeficienciesID;
                            defNotes.CreatedDate = itemDefNotes.CreatedDate;
                            defNotes.Description = itemDefNotes.Description;
                            defNotes.IniActUniqueID = itemDefNotes.IniActUniqueID;
                            defNotes.GIRFormID = itemDefNotes.GIRFormID;
                            defNotes.Name = itemDefNotes.Name;
                            defNotes.DeficienciesUniqueID = itemDefNotes.DeficienciesUniqueID;

                            if (IsFormDraft)
                                defNotes.isNew = 0; //RDBJ 10/14/2021
                            else
                            {
                                //defNotes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/14/2021
                                defNotes.isNew = 0; // JSL 06/27/2022 //RDBJ 10/14/2021
                            }

                            dbContext.GIRDeficienciesInitialActions.Add(defNotes);
                            dbContext.SaveChanges();

                            if (!IsFormDraft)
                            {
                                // JSL 06/24/2022
                                IsNeedToSendNotification = true;
                                Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                                dictNotificationData["UserGroup"] = "7";
                                dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                                dictNotificationData["DataType"] = AppStatic.NotificationTypeInitialAction;
                                dictNotificationData["IsDraft"] = Convert.ToString(true);
                                dictNotificationData["Title"] = AppStatic.SIRFormName;
                                dictNotificationData["DetailsURL"] = "GIRList/DeficienciesDetails?id=" + Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                                NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData
                                    , AppStatic.SIRForm // JSL 06/27/2022
                                    , strShipCode  // JSL 06/27/2022
                                    , true  // JSL 06/27/2022
                                    );
                                // End JSL 06/24/2022
                            }
                        }
                        if (itemDefNotes.GIRDeficienciesInitialActionsFiles != null && itemDefNotes.GIRDeficienciesInitialActionsFiles.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefNotes.GIRDeficienciesInitialActionsFiles)
                            {
                                Entity.GIRDeficienciesInitialActionsFile commentFile = new Entity.GIRDeficienciesInitialActionsFile();
                                commentFile = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActFileUniqueID == itemCommentFiles.IniActFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.GIRDeficienciesInitialActionsFile defNotesFile = new Entity.GIRDeficienciesInitialActionsFile();
                                    defNotesFile.DeficienciesID = itemCommentFiles.DeficienciesID;
                                    defNotesFile.FileName = itemCommentFiles.FileName;
                                    defNotesFile.StorePath = itemCommentFiles.StorePath;

                                    // JSL 12/03/2022
                                    if (defNotesFile.StorePath.StartsWith("data:"))
                                    {
                                        dicFileMetaData["FileName"] = defNotesFile.FileName;
                                        dicFileMetaData["Base64FileData"] = defNotesFile.StorePath;

                                        // JSL 01/08/2023
                                        dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeInitialAction;
                                        dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentFiles.IniActUniqueID);
                                        // End JSL 01/08/2023

                                        defNotesFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                    }
                                    // End JSL 12/03/2022

                                    defNotesFile.IsUpload = itemCommentFiles.IsUpload;
                                    defNotesFile.IniActUniqueID = itemCommentFiles.IniActUniqueID;
                                    defNotesFile.IniActFileUniqueID = itemCommentFiles.IniActFileUniqueID;
                                    dbContext.GIRDeficienciesInitialActionsFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud SIRDeficienciesInitialActions_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void SIRDeficienciesResolution_Save(List<Modals.GIRDeficienciesResolution> modalDefNotes, Guid? DeficienciesID   //RDBJ 09/22/2021 Updateed Modal Name used
            , string strShipCode    // JSL 06/27/2022
            , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
            , ref bool IsNeedToSendNotification // JSL 06/24/2022
            ) 
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modalDefNotes != null && modalDefNotes.Count > 0)
                {
                    foreach (var itemDefNotes in modalDefNotes)
                    {
                        Entity.GIRDeficienciesResolution dbModal = new Entity.GIRDeficienciesResolution();
                        dbModal = dbContext.GIRDeficienciesResolutions.Where(x => x.ResolutionUniqueID == itemDefNotes.ResolutionUniqueID).FirstOrDefault();

                        if (dbModal == null)
                        {
                            Entity.GIRDeficienciesResolution defNotes = new Entity.GIRDeficienciesResolution();
                            defNotes.DeficienciesID = itemDefNotes.DeficienciesID;
                            defNotes.CreatedDate = itemDefNotes.CreatedDate;
                            defNotes.Resolution = itemDefNotes.Resolution;
                            defNotes.ResolutionUniqueID = itemDefNotes.ResolutionUniqueID;
                            defNotes.GIRFormID = itemDefNotes.GIRFormID;
                            defNotes.DeficienciesUniqueID = itemDefNotes.DeficienciesUniqueID;
                            defNotes.Name = itemDefNotes.Name;
                            //defNotes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/14/2021
                            defNotes.isNew = 0; // JSL 06/27/2022 //RDBJ 10/14/2021

                            dbContext.GIRDeficienciesResolutions.Add(defNotes);
                            dbContext.SaveChanges();

                            // JSL 06/24/2022
                            IsNeedToSendNotification = true;
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "7";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeResolution;    // JSL 07/06/2022 
                            //dictNotificationData["DataType"] = AppStatic.NotificationTypeComment;    // JSL 07/06/2022 commented
                            dictNotificationData["IsDraft"] = Convert.ToString(true);
                            dictNotificationData["Title"] = AppStatic.SIRFormName;
                            dictNotificationData["DetailsURL"] = "GIRList/DeficienciesDetails?id=" + Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData
                                , AppStatic.SIRForm // JSL 06/27/2022
                                , strShipCode  // JSL 06/27/2022
                                , true  // JSL 06/27/2022
                                );
                            // End JSL 06/24/2022
                        }
                        if (itemDefNotes.GIRDeficienciesResolutionFiles != null && itemDefNotes.GIRDeficienciesResolutionFiles.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefNotes.GIRDeficienciesResolutionFiles)
                            {
                                Entity.GIRDeficienciesResolutionFile commentFile = new Entity.GIRDeficienciesResolutionFile();
                                commentFile = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionFileUniqueID == itemCommentFiles.ResolutionFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.GIRDeficienciesResolutionFile defNotesFile = new Entity.GIRDeficienciesResolutionFile();
                                    defNotesFile.DeficienciesID = itemCommentFiles.DeficienciesID;
                                    defNotesFile.FileName = itemCommentFiles.FileName;
                                    defNotesFile.StorePath = itemCommentFiles.StorePath;

                                    // JSL 12/03/2022
                                    if (defNotesFile.StorePath.StartsWith("data:"))
                                    {
                                        dicFileMetaData["FileName"] = defNotesFile.FileName;
                                        dicFileMetaData["Base64FileData"] = defNotesFile.StorePath;

                                        // JSL 01/08/2023
                                        dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeResolution;
                                        dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentFiles.ResolutionUniqueID);
                                        // End JSL 01/08/2023

                                        defNotesFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                    }
                                    // End JSL 12/03/2022

                                    defNotesFile.IsUpload = itemCommentFiles.IsUpload;
                                    defNotesFile.ResolutionUniqueID = itemCommentFiles.ResolutionUniqueID;
                                    defNotesFile.ResolutionFileUniqueID = itemCommentFiles.ResolutionFileUniqueID;
                                    dbContext.GIRDeficienciesResolutionFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud SIRDeficienciesResolution_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        //End RDBJ 09/25/2021

        //RDBJ 09/28/2021
        public void SIRNote_Save(string UniqueFormID, List<Modals.SIRNote> SIRNotes)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Guid UFormID = Guid.Parse(UniqueFormID);
                if (SIRNotes != null && SIRNotes.Count > 0)
                {
                    // RDBJ 04/02/2022
                    foreach (var item in SIRNotes)
                    {
                        bool IsSIRNoteExist = false;
                        Entity.SIRNote SIRNote = dbContext.SIRNotes.Where(x => x.NotesUniqueID == item.NotesUniqueID).FirstOrDefault();

                        if (SIRNote != null)
                            IsSIRNoteExist = true;
                        else
                            SIRNote = new Entity.SIRNote();

                        SIRNote.NotesUniqueID = item.NotesUniqueID;
                        SIRNote.SIRFormID = 0;
                        SIRNote.Number = item.Number;
                        SIRNote.Note = item.Note;
                        SIRNote.UniqueFormID = UFormID;
                        SIRNote.IsDeleted = (int)item.IsDeleted;

                        if (!IsSIRNoteExist)
                            dbContext.SIRNotes.Add(SIRNote);

                        dbContext.SaveChanges();
                    }
                    // End RDBJ 04/02/2022

                    // RDBJ 04/02/2022 Commented
                    /*
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    var dbSIRNote = dbContext.SIRNotes.Where(x => x.UniqueFormID == UFormID).ToList();
                    if (dbSIRNote != null && dbSIRNote.Count > 0)
                    {
                        foreach (var item in dbSIRNote)
                        {
                            dbContext.SIRNotes.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }

                    // Insert Into DB
                    foreach (var item in SIRNote)
                    {
                        Entity.SIRNote member = new Entity.SIRNote();
                        //member.NoteID = item.NoteID;  //RDBJ 09/28/2021 Do not uncomment this
                        member.SIRFormID = 0;
                        member.UniqueFormID = UFormID;
                        member.Number = item.Number;
                        member.Note = item.Note;
                        
                        dbContext.SIRNotes.Add(member);
                    }
                    dbContext.SaveChanges();
                    */
                    // End RDBJ 04/02/2022 Commented
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud SIRNote_Save : " + ex.Message);
            }
        }

        public void SIRAdditionalNote_Save(string UniqueFormID, List<Modals.SIRAdditionalNote> SIRAdditionalNote)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid UFormID = Guid.Parse(UniqueFormID);
            if (SIRAdditionalNote != null && SIRAdditionalNote.Count > 0)
            {
                // RDBJ 04/02/2022
                foreach (var item in SIRAdditionalNote)
                {
                    bool IsSIRAddNoteExist = false;
                    Entity.SIRAdditionalNote SIRAddNote = dbContext.SIRAdditionalNotes.Where(x => x.NotesUniqueID == item.NotesUniqueID).FirstOrDefault();

                    if (SIRAddNote != null)
                        IsSIRAddNoteExist = true;
                    else
                        SIRAddNote = new Entity.SIRAdditionalNote();

                    SIRAddNote.NotesUniqueID = item.NotesUniqueID;
                    SIRAddNote.SIRFormID = 0;
                    SIRAddNote.Number = item.Number;
                    SIRAddNote.Note = item.Note;
                    SIRAddNote.UniqueFormID = UFormID;
                    SIRAddNote.IsDeleted = (int)item.IsDeleted;

                    if (!IsSIRAddNoteExist)
                        dbContext.SIRAdditionalNotes.Add(SIRAddNote);

                    dbContext.SaveChanges();
                }
                // End RDBJ 04/02/2022

                // RDBJ 04/02/2022 Commented
                /*
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var dbSIRNote = dbContext.SIRAdditionalNotes.Where(x => x.UniqueFormID == UFormID).ToList();
                if (dbSIRNote != null && dbSIRNote.Count > 0)
                {
                    foreach (var item in dbSIRNote)
                    {
                        dbContext.SIRAdditionalNotes.Remove(item);
                    }
                    dbContext.SaveChanges();
                }

                // Insert Into DB
                foreach (var item in SIRAdditionalNote)
                {
                    Entity.SIRAdditionalNote member = new Entity.SIRAdditionalNote();
                    //member.NoteID = item.NoteID;  //RDBJ 09/28/2021 Do not uncomment this
                    member.SIRFormID = 0;
                    member.UniqueFormID = UFormID;
                    member.Number = item.Number;
                    member.Note = item.Note;

                    dbContext.SIRAdditionalNotes.Add(member);
                }
                dbContext.SaveChanges();
                */
                // End RDBJ 04/02/2022 Commented
            }
        }
        //End RDBJ 09/28/2021

        public List<SIRModal> GetSIRFormsUnsyncedDataFromCloud()
        {
            List<SIRModal> unSyncList = new List<SIRModal>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var girList = dbContext.SuperintendedInspectionReports.Where(x => x.IsSynced == false).ToList();
                if (girList != null && girList.Count > 0)
                {
                    foreach (var item in girList)
                    {
                        SIRModal dbModal = new SIRModal();
                        Modals.SuperintendedInspectionReport sirModal = new Modals.SuperintendedInspectionReport();
                       
                        GetSIRFormData(item, ref dbModal);
                        dbModal.SuperintendedInspectionReport = sirModal;

                        dbModal.SIRNote = new List<Modals.SIRNote>();  //RDBJ 09/25/2021 changed dbContext.SIRNotes.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                        dbModal.SIRAdditionalNote = new List<Modals.SIRAdditionalNote>();  //RDBJ 09/25/2021 changed dbContext.SIRAdditionalNotes.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                        dbModal.GIRDeficiencies = new List<GIRDeficiencies>();

                        //RDBJ 09/25/2021 
                        var sirNoteList = dbContext.SIRNotes.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                        if (sirNoteList != null && sirNoteList.Count > 0)
                        {
                            foreach (var sirNote in sirNoteList)
                            {
                                Modals.SIRNote sirNoteModal = new Modals.SIRNote();
                                sirNoteModal.Note = sirNote.Note;
                                sirNoteModal.Number = sirNote.Number;
                                sirNoteModal.SIRFormID = 0;
                                sirNoteModal.UniqueFormID = sirNote.UniqueFormID;

                                dbModal.SIRNote.Add(sirNoteModal);
                            }
                        }
                        //RDBJ 09/25/2021 

                        //End RDBJ 09/25/2021 
                        var sirAddNoteList = dbContext.SIRAdditionalNotes.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                        if (sirAddNoteList != null && sirAddNoteList.Count > 0)
                        {
                            foreach (var sirNote in sirNoteList)
                            {
                                Modals.SIRAdditionalNote sirAddNoteModal = new Modals.SIRAdditionalNote();
                                sirAddNoteModal.Note = sirNote.Note;
                                sirAddNoteModal.Number = sirNote.Number;
                                sirAddNoteModal.SIRFormID = 0;
                                sirAddNoteModal.UniqueFormID = sirNote.UniqueFormID;

                                dbModal.SIRAdditionalNote.Add(sirAddNoteModal);
                            }
                        }
                        //End RDBJ 09/25/2021 

                        var defList = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                        if (defList != null && defList.Count > 0)
                        {
                            foreach (var def in defList)
                            {
                                GIRDeficiencies girDef = new GIRDeficiencies();
                                girDef.GIRDeficienciesComments = new List<DeficienciesNote>();
                                girDef.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>(); //RDBJ 09/24/2021
                                girDef.GIRDeficienciesResolution = new List<Modals.GIRDeficienciesResolution>(); //RDBJ 09/24/2021

                                girDef.No = def.No;
                                girDef.DateRaised = def.DateRaised;
                                girDef.Deficiency = def.Deficiency;
                                girDef.DateClosed = def.DateClosed;
                                girDef.CreatedDate = def.CreatedDate;
                                girDef.UpdatedDate = def.UpdatedDate;
                                girDef.Ship = def.Ship;
                                girDef.IsClose = def.IsClose;
                                girDef.ReportType = def.ReportType;
                                girDef.ItemNo = def.ItemNo;
                                girDef.Section = def.Section;
                                girDef.UniqueFormID = def.UniqueFormID;
                                girDef.isDelete = def.isDelete;

                                var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == girDef.DeficienciesID).ToList();
                                if (defFiles != null && defFiles.Count > 0)
                                {
                                    foreach (var girDefFile in defFiles)
                                    {
                                        Modals.GIRDeficienciesFile defFile = new Modals.GIRDeficienciesFile();
                                        defFile.DeficienciesID = girDefFile.DeficienciesID;
                                        defFile.FileName = girDefFile.FileName;
                                        defFile.StorePath = girDefFile.StorePath;
                                        girDef.GIRDeficienciesFile.Add(defFile);
                                    }
                                }

                                var defComments = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesID == def.DeficienciesID).ToList();
                                if (defComments != null && defComments.Count > 0)
                                {
                                    foreach (var defComment in defComments)
                                    {
                                        DeficienciesNote defNote = new DeficienciesNote();
                                        defNote.DeficienciesID = defComment.DeficienciesID;
                                        defNote.UserName = defComment.UserName;
                                        defNote.Comment = defComment.Comment;
                                        defNote.CreatedDate = defComment.CreatedDate;
                                        defNote.ModifyDate = defComment.ModifyDate;
                                        defNote.NoteUniqueID = defComment.NoteUniqueID;

                                        var defCommentFiles = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteUniqueID == defNote.NoteUniqueID).ToList();
                                        if (defCommentFiles != null && defCommentFiles.Count > 0)
                                        {
                                            foreach (var defCommmentFile in defCommentFiles)
                                            {
                                                Modals.GIRDeficienciesCommentFile defComFile = new Modals.GIRDeficienciesCommentFile();
                                                defComFile.DeficienciesID = defCommmentFile.DeficienciesID;
                                                defComFile.FileName = defCommmentFile.FileName;
                                                defComFile.StorePath = defCommmentFile.StorePath;
                                                defComFile.IsUpload = defCommmentFile.IsUpload;
                                                defComFile.NoteUniqueID = defCommmentFile.NoteUniqueID;
                                                defComFile.CommentFileUniqueID = defCommmentFile.CommentFileUniqueID;

                                                defNote.GIRDeficienciesCommentFile.Add(defComFile);
                                            }
                                        }
                                        girDef.GIRDeficienciesComments.Add(defNote);
                                    }
                                }

                                var defIntialActions = dbContext.GIRDeficienciesInitialActions.Where(x => x.DeficienciesID == def.DeficienciesID).ToList();
                                if (defIntialActions != null && defIntialActions.Count > 0)
                                {
                                    foreach (var defInitial in defIntialActions)
                                    {
                                        GIRDeficienciesInitialActions defIntialAction = new GIRDeficienciesInitialActions(); //RDBJ 09/22/2021 Updateed Modal Name used
                                        defIntialAction.DeficienciesID = defInitial.DeficienciesID;
                                        defIntialAction.Name = defInitial.Name;
                                        defIntialAction.Description = defInitial.Description;
                                        defIntialAction.CreatedDate = defInitial.CreatedDate;
                                        defIntialAction.IniActUniqueID = defInitial.IniActUniqueID;

                                        var defIntialActionFiles = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActUniqueID == defInitial.IniActUniqueID).ToList();
                                        if (defIntialActionFiles != null && defIntialActionFiles.Count > 0)
                                        {
                                            foreach (var defIntialActionFile in defIntialActionFiles)
                                            {
                                                Modals.GIRDeficienciesInitialActionsFile defComFile = new Modals.GIRDeficienciesInitialActionsFile();
                                                defComFile.DeficienciesID = defIntialActionFile.DeficienciesID;
                                                defComFile.FileName = defIntialActionFile.FileName;
                                                defComFile.StorePath = defIntialActionFile.StorePath;
                                                defComFile.IsUpload = defIntialActionFile.IsUpload;
                                                defComFile.IniActUniqueID = defIntialActionFile.IniActUniqueID;
                                                defComFile.IniActFileUniqueID = defIntialActionFile.IniActFileUniqueID;

                                                defIntialAction.GIRDeficienciesInitialActionsFiles.Add(defComFile);
                                            }
                                        }
                                        girDef.GIRDeficienciesInitialActions.Add(defIntialAction); //RDBJ 09/22/2021 Updateed Modal Name used
                                    }
                                }

                                var defResolutions = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesID == def.DeficienciesID).ToList();
                                if (defResolutions != null && defResolutions.Count > 0)
                                {
                                    foreach (var defResolution in defResolutions)
                                    {
                                        Modals.GIRDeficienciesResolution defResolutionModal = new Modals.GIRDeficienciesResolution(); //RDBJ 09/22/2021 Updateed Modal Name used
                                        defResolutionModal.DeficienciesID = defResolution.DeficienciesID;
                                        defResolutionModal.Name = defResolution.Name;
                                        defResolutionModal.Resolution = defResolution.Resolution;
                                        defResolutionModal.CreatedDate = defResolution.CreatedDate;
                                        defResolutionModal.ResolutionUniqueID = defResolution.ResolutionUniqueID;

                                        var defResolutionFiles = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == defResolution.ResolutionUniqueID).ToList();
                                        if (defResolutionFiles != null && defResolutionFiles.Count > 0)
                                        {
                                            foreach (var defResolutionFile in defResolutionFiles)
                                            {
                                                Modals.GIRDeficienciesResolutionFile defresFile = new Modals.GIRDeficienciesResolutionFile();
                                                defresFile.DeficienciesID = defResolutionFile.DeficienciesID;
                                                defresFile.FileName = defResolutionFile.FileName;
                                                defresFile.StorePath = defResolutionFile.StorePath;
                                                defresFile.IsUpload = defResolutionFile.IsUpload;
                                                defresFile.ResolutionUniqueID = defResolutionFile.ResolutionUniqueID;
                                                defresFile.ResolutionFileUniqueID = defResolutionFile.ResolutionFileUniqueID;

                                                defResolutionModal.GIRDeficienciesResolutionFiles.Add(defresFile);
                                            }
                                        }
                                        girDef.GIRDeficienciesResolution.Add(defResolutionModal); //RDBJ 09/22/2021 Updateed Modal Name used
                                    }
                                }
                                dbModal.GIRDeficiencies.Add(girDef);
                            }
                        }
                        unSyncList.Add(dbModal);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GetSIRFormsUnsyncedDataFromCloud " + ex.Message + "\n" + ex.InnerException);
                unSyncList = null;
            }
            return unSyncList;
        }
        public List<SIRModal> getUnsynchSIRList(
            string strShipCode  // JSL 11/12/2022
            )
        {
            List<SIRModal> unSyncList = new List<SIRModal>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var girList = dbContext.SuperintendedInspectionReports
                    .Where(x => x.IsSynced == false
                    && x.UniqueFormID != null   // RDBJ 12/18/2021
                    ).ToList();

                if (girList != null && girList.Count > 0)
                {
                    // JSL 11/12/2022
                    if (!string.IsNullOrEmpty(strShipCode))
                    {
                        girList = girList.Where(x => x.ShipName == strShipCode).ToList();
                    }
                    // End JSL 11/12/2022

                    foreach (var item in girList)
                    {
                        SIRModal dbModal = new SIRModal();
                        dbModal.SuperintendedInspectionReport = new Modals.SuperintendedInspectionReport();
                        dbModal.GIRDeficiencies = new List<GIRDeficiencies>();
                        dbModal.SIRNote = new List<Modals.SIRNote>();  // RDBJ 12/18/2021
                        dbModal.SIRAdditionalNote = new List<Modals.SIRAdditionalNote>();  // RDBJ 12/18/2021

                        GetSIRFormData(item, ref dbModal);

                        // RDBJ 01/05/2022 wrapped in if
                        if (dbModal.SuperintendedInspectionReport.isDelete == 0)
                        {
                            // RDBJ 12/18/2021
                            var sirNoteList = dbContext.SIRNotes.Where(x => x.UniqueFormID == item.UniqueFormID
                            && x.NotesUniqueID != null  // RDBJ 04/04/2022
                            ).ToList();
                            if (sirNoteList != null && sirNoteList.Count > 0)
                            {
                                foreach (var sirNote in sirNoteList)
                                {
                                    Modals.SIRNote sirNoteModal = new Modals.SIRNote();
                                    sirNoteModal.Note = sirNote.Note;
                                    sirNoteModal.Number = sirNote.Number;
                                    sirNoteModal.SIRFormID = 0;
                                    sirNoteModal.UniqueFormID = sirNote.UniqueFormID;
                                    sirNoteModal.NotesUniqueID = (Guid)sirNote.NotesUniqueID; // RDBJ 04/02/2022
                                    sirNoteModal.IsDeleted = sirNote.IsDeleted; // RDBJ 04/02/2022

                                    dbModal.SIRNote.Add(sirNoteModal);
                                }
                            }
                            // End RDBJ 12/18/2021

                            // RDBJ 12/18/2021
                            var sirAddNoteList = dbContext.SIRAdditionalNotes.Where(x => x.UniqueFormID == item.UniqueFormID
                            && x.NotesUniqueID != null  // RDBJ 04/04/2022
                            ).ToList();
                            if (sirAddNoteList != null && sirAddNoteList.Count > 0)
                            {
                                foreach (var sirAddNote in sirAddNoteList)
                                {
                                    Modals.SIRAdditionalNote sirAddNoteModal = new Modals.SIRAdditionalNote();
                                    sirAddNoteModal.Note = sirAddNote.Note;
                                    sirAddNoteModal.Number = sirAddNote.Number;
                                    sirAddNoteModal.SIRFormID = 0;
                                    sirAddNoteModal.UniqueFormID = sirAddNote.UniqueFormID;
                                    sirAddNoteModal.NotesUniqueID = (Guid)sirAddNote.NotesUniqueID; // RDBJ 04/02/2022
                                    sirAddNoteModal.IsDeleted = sirAddNote.IsDeleted; // RDBJ 04/02/2022

                                    dbModal.SIRAdditionalNote.Add(sirAddNoteModal);
                                }
                            }
                            // End RDBJ 12/18/2021

                            var defList = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                            if (defList != null && defList.Count > 0)
                            {
                                foreach (var def in defList)
                                {
                                    GIRDeficiencies girDef = new GIRDeficiencies();
                                    girDef.GIRDeficienciesComments = new List<DeficienciesNote>();
                                    girDef.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>(); //RDBJ 09/24/2021
                                    girDef.GIRDeficienciesResolution = new List<Modals.GIRDeficienciesResolution>(); //RDBJ 09/24/2021

                                    girDef.No = def.No;
                                    girDef.DateRaised = def.DateRaised;
                                    girDef.Deficiency = def.Deficiency;
                                    girDef.DateClosed = def.DateClosed;
                                    girDef.CreatedDate = def.CreatedDate;
                                    girDef.UpdatedDate = def.UpdatedDate;
                                    girDef.Ship = def.Ship;
                                    girDef.IsClose = def.IsClose;
                                    girDef.ReportType = def.ReportType;
                                    girDef.ItemNo = def.ItemNo;
                                    girDef.Section = def.Section;
                                    girDef.UniqueFormID = def.UniqueFormID;
                                    girDef.isDelete = def.isDelete;
                                    girDef.DeficienciesUniqueID = def.DeficienciesUniqueID;
                                    girDef.Priority = def.Priority == null ? 12 : def.Priority; //RDBJ 12/18/2021
                                    girDef.AssignTo = def.AssignTo; // RDBJ 12/18/2021
                                    girDef.DueDate = def.DueDate;   // RDBJ 03/01/2022

                                    // RDBJ 12/23/2021 wrapped in if
                                    if (def.isDelete == 0)
                                    {
                                        var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == girDef.DeficienciesUniqueID).ToList();
                                        if (defFiles != null && defFiles.Count > 0)
                                        {
                                            foreach (var girDefFile in defFiles)
                                            {
                                                Modals.GIRDeficienciesFile defFile = new Modals.GIRDeficienciesFile();
                                                defFile.DeficienciesFileUniqueID = girDefFile.DeficienciesUniqueID; // JSL 06/07/2022
                                                defFile.DeficienciesID = girDefFile.DeficienciesID != null ? girDefFile.DeficienciesID : 0; // RDBJ 01/15/2022 set avoid null error
                                                defFile.DeficienciesUniqueID = girDefFile.DeficienciesUniqueID;
                                                defFile.FileName = girDefFile.FileName;
                                                defFile.StorePath = girDefFile.StorePath;

                                                // JSL 12/04/2022
                                                if (!defFile.StorePath.StartsWith("data:"))
                                                {
                                                    defFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defFile.StorePath);
                                                }
                                                // End JSL 12/04/2022

                                                girDef.GIRDeficienciesFile.Add(defFile);
                                            }
                                        }

                                        var defComments = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                        if (defComments != null && defComments.Count > 0)
                                        {
                                            foreach (var defComment in defComments)
                                            {
                                                DeficienciesNote defNote = new DeficienciesNote();
                                                defNote.DeficienciesUniqueID = defComment.DeficienciesUniqueID;
                                                defNote.UserName = defComment.UserName;
                                                defNote.Comment = defComment.Comment;
                                                defNote.CreatedDate = defComment.CreatedDate;
                                                defNote.ModifyDate = defComment.ModifyDate;
                                                defNote.NoteUniqueID = defComment.NoteUniqueID;

                                                var defCommentFiles = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteUniqueID == defNote.NoteUniqueID).ToList();
                                                if (defCommentFiles != null && defCommentFiles.Count > 0)
                                                {
                                                    foreach (var defCommmentFile in defCommentFiles)
                                                    {
                                                        Modals.GIRDeficienciesCommentFile defComFile = new Modals.GIRDeficienciesCommentFile();
                                                        defComFile.FileName = defCommmentFile.FileName;
                                                        defComFile.StorePath = defCommmentFile.StorePath;
                                                        defComFile.IsUpload = defCommmentFile.IsUpload;
                                                        defComFile.NoteUniqueID = defCommmentFile.NoteUniqueID;
                                                        defComFile.CommentFileUniqueID = defCommmentFile.CommentFileUniqueID;

                                                        // JSL 12/04/2022
                                                        if (!defComFile.StorePath.StartsWith("data:"))
                                                        {
                                                            defComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defComFile.StorePath);
                                                        }
                                                        // End JSL 12/04/2022

                                                        defNote.GIRDeficienciesCommentFile.Add(defComFile);
                                                    }
                                                }
                                                girDef.GIRDeficienciesComments.Add(defNote);
                                            }
                                        }

                                        var defIntialActions = dbContext.GIRDeficienciesInitialActions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                        if (defIntialActions != null && defIntialActions.Count > 0)
                                        {
                                            foreach (var defInitial in defIntialActions)
                                            {
                                                GIRDeficienciesInitialActions defIntialAction = new GIRDeficienciesInitialActions(); //RDBJ 09/22/2021 Updateed Modal Name used
                                                defIntialAction.DeficienciesUniqueID = defInitial.DeficienciesUniqueID;
                                                defIntialAction.Name = defInitial.Name;
                                                defIntialAction.Description = defInitial.Description;
                                                defIntialAction.CreatedDate = defInitial.CreatedDate;
                                                defIntialAction.IniActUniqueID = defInitial.IniActUniqueID;

                                                var defIntialActionFiles = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActUniqueID == defInitial.IniActUniqueID).ToList();
                                                if (defIntialActionFiles != null && defIntialActionFiles.Count > 0)
                                                {
                                                    foreach (var defIntialActionFile in defIntialActionFiles)
                                                    {
                                                        Modals.GIRDeficienciesInitialActionsFile defComFile = new Modals.GIRDeficienciesInitialActionsFile();
                                                        defComFile.FileName = defIntialActionFile.FileName;
                                                        defComFile.StorePath = defIntialActionFile.StorePath;
                                                        defComFile.IsUpload = defIntialActionFile.IsUpload;
                                                        defComFile.IniActUniqueID = defIntialActionFile.IniActUniqueID;
                                                        defComFile.IniActFileUniqueID = defIntialActionFile.IniActFileUniqueID;

                                                        // JSL 12/04/2022
                                                        if (!defComFile.StorePath.StartsWith("data:"))
                                                        {
                                                            defComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defComFile.StorePath);
                                                        }
                                                        // End JSL 12/04/2022

                                                        defIntialAction.GIRDeficienciesInitialActionsFiles.Add(defComFile);
                                                    }
                                                }
                                                girDef.GIRDeficienciesInitialActions.Add(defIntialAction); //RDBJ 09/22/2021 Updateed Modal Name used
                                            }
                                        }

                                        var defResolutions = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                        if (defResolutions != null && defResolutions.Count > 0)
                                        {
                                            foreach (var defResolution in defResolutions)
                                            {
                                                Modals.GIRDeficienciesResolution defResolutionModal = new Modals.GIRDeficienciesResolution();
                                                defResolutionModal.DeficienciesUniqueID = defResolution.DeficienciesUniqueID;
                                                defResolutionModal.Name = defResolution.Name;
                                                defResolutionModal.Resolution = defResolution.Resolution;
                                                defResolutionModal.CreatedDate = defResolution.CreatedDate;
                                                defResolutionModal.ResolutionUniqueID = defResolution.ResolutionUniqueID;

                                                var defResolutionFiles = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == defResolution.ResolutionUniqueID).ToList();
                                                if (defResolutionFiles != null && defResolutionFiles.Count > 0)
                                                {
                                                    foreach (var defResolutionFile in defResolutionFiles)
                                                    {
                                                        Modals.GIRDeficienciesResolutionFile defresFile = new Modals.GIRDeficienciesResolutionFile();
                                                        defresFile.DeficienciesID = defResolutionFile.DeficienciesID;
                                                        defresFile.FileName = defResolutionFile.FileName;
                                                        defresFile.StorePath = defResolutionFile.StorePath;
                                                        defresFile.IsUpload = defResolutionFile.IsUpload;
                                                        defresFile.ResolutionUniqueID = defResolutionFile.ResolutionUniqueID;
                                                        defresFile.ResolutionFileUniqueID = defResolutionFile.ResolutionFileUniqueID;

                                                        // JSL 12/04/2022
                                                        if (!defresFile.StorePath.StartsWith("data:"))
                                                        {
                                                            defresFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defresFile.StorePath);
                                                        }
                                                        // End JSL 12/04/2022

                                                        defResolutionModal.GIRDeficienciesResolutionFiles.Add(defresFile);
                                                    }
                                                }
                                                girDef.GIRDeficienciesResolution.Add(defResolutionModal); //RDBJ 09/22/2021 Updateed Modal Name used
                                            }
                                        }
                                    }

                                    dbModal.GIRDeficiencies.Add(girDef);
                                }
                            }
                        }

                        unSyncList.Add(dbModal);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getUnsynchSIRList " + ex.Message + "\n" + ex.InnerException);
                unSyncList = null;
            }
            return unSyncList;
        }
        public void GetSIRFormData(Entity.SuperintendedInspectionReport Modal, ref  SIRModal dbModal)
        {
            dbModal.SuperintendedInspectionReport.UniqueFormID = Modal.UniqueFormID;
            dbModal.SuperintendedInspectionReport.ShipID = Modal.ShipID;
            dbModal.SuperintendedInspectionReport.isDelete = Modal.isDelete; // RDBJ 01/05/2022
            dbModal.SuperintendedInspectionReport.ShipName = Modal.ShipName;
            dbModal.SuperintendedInspectionReport.Date = Modal.Date;
            dbModal.SuperintendedInspectionReport.Port = Modal.Port;
            dbModal.SuperintendedInspectionReport.Master = Modal.Master;
            dbModal.SuperintendedInspectionReport.Superintended = Modal.Superintended;
            dbModal.SuperintendedInspectionReport.FormVersion = Modal.FormVersion;
            dbModal.SuperintendedInspectionReport.Section1_1_Condition = Modal.Section1_1_Condition;
            dbModal.SuperintendedInspectionReport.Section1_1_Comment = Modal.Section1_1_Comment;
            dbModal.SuperintendedInspectionReport.Section1_2_Condition = Modal.Section1_2_Condition;
            dbModal.SuperintendedInspectionReport.Section1_2_Comment = Modal.Section1_2_Comment;
            dbModal.SuperintendedInspectionReport.Section1_3_Condition = Modal.Section1_3_Condition;
            dbModal.SuperintendedInspectionReport.Section1_3_Comment = Modal.Section1_3_Comment;
            dbModal.SuperintendedInspectionReport.Section1_4_Condition = Modal.Section1_4_Condition;
            dbModal.SuperintendedInspectionReport.Section1_4_Comment = Modal.Section1_4_Comment;
            dbModal.SuperintendedInspectionReport.Section1_5_Condition = Modal.Section1_5_Condition;
            dbModal.SuperintendedInspectionReport.Section1_5_Comment = Modal.Section1_5_Comment;
            dbModal.SuperintendedInspectionReport.Section1_6_Condition = Modal.Section1_6_Condition;
            dbModal.SuperintendedInspectionReport.Section1_6_Comment = Modal.Section1_6_Comment;
            dbModal.SuperintendedInspectionReport.Section1_7_Condition = Modal.Section1_7_Condition;
            dbModal.SuperintendedInspectionReport.Section1_7_Comment = Modal.Section1_7_Comment;
            dbModal.SuperintendedInspectionReport.Section1_8_Condition = Modal.Section1_8_Condition;
            dbModal.SuperintendedInspectionReport.Section1_8_Comment = Modal.Section1_8_Comment;
            dbModal.SuperintendedInspectionReport.Section1_9_Condition = Modal.Section1_9_Condition;
            dbModal.SuperintendedInspectionReport.Section1_9_Comment = Modal.Section1_9_Comment;
            dbModal.SuperintendedInspectionReport.Section1_10_Condition = Modal.Section1_10_Condition;
            dbModal.SuperintendedInspectionReport.Section1_10_Comment = Modal.Section1_10_Comment;
            dbModal.SuperintendedInspectionReport.Section1_11_Condition = Modal.Section1_11_Condition;
            dbModal.SuperintendedInspectionReport.Section1_11_Comment = Modal.Section1_11_Comment;

            dbModal.SuperintendedInspectionReport.Section2_1_Condition = Modal.Section2_1_Condition;
            dbModal.SuperintendedInspectionReport.Section2_1_Comment = Modal.Section2_1_Comment;
            dbModal.SuperintendedInspectionReport.Section2_2_Condition = Modal.Section2_2_Condition;
            dbModal.SuperintendedInspectionReport.Section2_2_Comment = Modal.Section2_2_Comment;
            dbModal.SuperintendedInspectionReport.Section2_3_Condition = Modal.Section2_3_Condition;
            dbModal.SuperintendedInspectionReport.Section2_3_Comment = Modal.Section2_3_Comment;
            dbModal.SuperintendedInspectionReport.Section2_4_Condition = Modal.Section2_4_Condition;
            dbModal.SuperintendedInspectionReport.Section2_4_Comment = Modal.Section2_4_Comment;
            dbModal.SuperintendedInspectionReport.Section2_5_Condition = Modal.Section2_5_Condition;
            dbModal.SuperintendedInspectionReport.Section2_5_Comment = Modal.Section2_5_Comment;
            dbModal.SuperintendedInspectionReport.Section2_6_Condition = Modal.Section2_6_Condition;
            dbModal.SuperintendedInspectionReport.Section2_6_Comment = Modal.Section2_6_Comment;
            dbModal.SuperintendedInspectionReport.Section2_7_Condition = Modal.Section2_7_Condition;
            dbModal.SuperintendedInspectionReport.Section2_7_Comment = Modal.Section2_7_Comment;

            dbModal.SuperintendedInspectionReport.Section3_1_Condition = Modal.Section3_1_Condition;
            dbModal.SuperintendedInspectionReport.Section3_1_Comment = Modal.Section3_1_Comment;
            dbModal.SuperintendedInspectionReport.Section3_2_Condition = Modal.Section3_2_Condition;
            dbModal.SuperintendedInspectionReport.Section3_2_Comment = Modal.Section3_2_Comment;
            dbModal.SuperintendedInspectionReport.Section3_3_Condition = Modal.Section3_3_Condition;
            dbModal.SuperintendedInspectionReport.Section3_3_Comment = Modal.Section3_3_Comment;
            dbModal.SuperintendedInspectionReport.Section3_4_Condition = Modal.Section3_4_Condition;
            dbModal.SuperintendedInspectionReport.Section3_4_Comment = Modal.Section3_4_Comment;
            dbModal.SuperintendedInspectionReport.Section3_5_Condition = Modal.Section3_5_Condition;
            dbModal.SuperintendedInspectionReport.Section3_5_Comment = Modal.Section3_5_Comment;

            dbModal.SuperintendedInspectionReport.Section4_1_Condition = Modal.Section4_1_Condition;
            dbModal.SuperintendedInspectionReport.Section4_1_Comment = Modal.Section4_1_Comment;
            dbModal.SuperintendedInspectionReport.Section4_2_Condition = Modal.Section4_2_Condition;
            dbModal.SuperintendedInspectionReport.Section4_2_Comment = Modal.Section4_2_Comment;
            dbModal.SuperintendedInspectionReport.Section4_3_Condition = Modal.Section4_3_Condition;
            dbModal.SuperintendedInspectionReport.Section4_3_Comment = Modal.Section4_3_Comment;

            dbModal.SuperintendedInspectionReport.Section5_1_Condition = Modal.Section5_1_Condition;
            dbModal.SuperintendedInspectionReport.Section5_1_Comment = Modal.Section5_1_Comment;
            dbModal.SuperintendedInspectionReport.Section5_6_Condition = Modal.Section5_6_Condition;
            dbModal.SuperintendedInspectionReport.Section5_6_Comment = Modal.Section5_6_Comment;
            dbModal.SuperintendedInspectionReport.Section5_8_Condition = Modal.Section5_8_Condition;
            dbModal.SuperintendedInspectionReport.Section5_8_Comment = Modal.Section5_8_Comment;
            dbModal.SuperintendedInspectionReport.Section5_9_Condition = Modal.Section5_9_Condition;
            dbModal.SuperintendedInspectionReport.Section5_9_Comment = Modal.Section5_9_Comment;

            dbModal.SuperintendedInspectionReport.Section6_1_Condition = Modal.Section6_1_Condition;
            dbModal.SuperintendedInspectionReport.Section6_1_Comment = Modal.Section6_1_Comment;
            dbModal.SuperintendedInspectionReport.Section6_2_Condition = Modal.Section6_2_Condition;
            dbModal.SuperintendedInspectionReport.Section6_2_Comment = Modal.Section6_2_Comment;
            dbModal.SuperintendedInspectionReport.Section6_3_Condition = Modal.Section6_3_Condition;
            dbModal.SuperintendedInspectionReport.Section6_3_Comment = Modal.Section6_3_Comment;
            dbModal.SuperintendedInspectionReport.Section6_4_Condition = Modal.Section6_4_Condition;
            dbModal.SuperintendedInspectionReport.Section6_4_Comment = Modal.Section6_4_Comment;
            dbModal.SuperintendedInspectionReport.Section6_5_Condition = Modal.Section6_5_Condition;
            dbModal.SuperintendedInspectionReport.Section6_5_Comment = Modal.Section6_5_Comment;
            dbModal.SuperintendedInspectionReport.Section6_6_Condition = Modal.Section6_6_Condition;
            dbModal.SuperintendedInspectionReport.Section6_6_Comment = Modal.Section6_6_Comment;
            dbModal.SuperintendedInspectionReport.Section6_7_Condition = Modal.Section6_7_Condition;
            dbModal.SuperintendedInspectionReport.Section6_7_Comment = Modal.Section6_7_Comment;
            dbModal.SuperintendedInspectionReport.Section6_8_Condition = Modal.Section6_8_Condition;
            dbModal.SuperintendedInspectionReport.Section6_8_Comment = Modal.Section6_8_Comment;

            dbModal.SuperintendedInspectionReport.Section7_1_Condition = Modal.Section7_1_Condition;
            dbModal.SuperintendedInspectionReport.Section7_1_Comment = Modal.Section7_1_Comment;
            dbModal.SuperintendedInspectionReport.Section7_2_Condition = Modal.Section7_2_Condition;
            dbModal.SuperintendedInspectionReport.Section7_2_Comment = Modal.Section7_2_Comment;
            dbModal.SuperintendedInspectionReport.Section7_3_Condition = Modal.Section7_3_Condition;
            dbModal.SuperintendedInspectionReport.Section7_3_Comment = Modal.Section7_3_Comment;
            dbModal.SuperintendedInspectionReport.Section7_4_Condition = Modal.Section7_4_Condition;
            dbModal.SuperintendedInspectionReport.Section7_4_Comment = Modal.Section7_4_Comment;
            dbModal.SuperintendedInspectionReport.Section7_5_Condition = Modal.Section7_5_Condition;
            dbModal.SuperintendedInspectionReport.Section7_5_Comment = Modal.Section7_5_Comment;
            dbModal.SuperintendedInspectionReport.Section7_6_Condition = Modal.Section7_6_Condition;
            dbModal.SuperintendedInspectionReport.Section7_6_Comment = Modal.Section7_6_Comment;

            dbModal.SuperintendedInspectionReport.Section8_1_Condition = Modal.Section8_1_Condition;
            dbModal.SuperintendedInspectionReport.Section8_1_Comment = Modal.Section8_1_Comment;
            dbModal.SuperintendedInspectionReport.Section8_2_Condition = Modal.Section8_2_Condition;
            dbModal.SuperintendedInspectionReport.Section8_2_Comment = Modal.Section8_2_Comment;
            dbModal.SuperintendedInspectionReport.Section8_3_Condition = Modal.Section8_3_Condition;
            dbModal.SuperintendedInspectionReport.Section8_3_Comment = Modal.Section8_3_Comment;
            dbModal.SuperintendedInspectionReport.Section8_4_Condition = Modal.Section8_4_Condition;
            dbModal.SuperintendedInspectionReport.Section8_4_Comment = Modal.Section8_4_Comment;
            dbModal.SuperintendedInspectionReport.Section8_5_Condition = Modal.Section8_5_Condition;
            dbModal.SuperintendedInspectionReport.Section8_5_Comment = Modal.Section8_5_Comment;
            dbModal.SuperintendedInspectionReport.Section8_6_Condition = Modal.Section8_6_Condition;
            dbModal.SuperintendedInspectionReport.Section8_6_Comment = Modal.Section8_6_Comment;
            dbModal.SuperintendedInspectionReport.Section8_7_Condition = Modal.Section8_7_Condition;
            dbModal.SuperintendedInspectionReport.Section8_7_Comment = Modal.Section8_7_Comment;
            dbModal.SuperintendedInspectionReport.Section8_8_Condition = Modal.Section8_8_Condition;
            dbModal.SuperintendedInspectionReport.Section8_8_Comment = Modal.Section8_8_Comment;
            dbModal.SuperintendedInspectionReport.Section8_9_Condition = Modal.Section8_9_Condition;
            dbModal.SuperintendedInspectionReport.Section8_9_Comment = Modal.Section8_9_Comment;
            dbModal.SuperintendedInspectionReport.Section8_10_Condition = Modal.Section8_10_Condition;
            dbModal.SuperintendedInspectionReport.Section8_10_Comment = Modal.Section8_10_Comment;
            dbModal.SuperintendedInspectionReport.Section8_11_Condition = Modal.Section8_11_Condition;
            dbModal.SuperintendedInspectionReport.Section8_11_Comment = Modal.Section8_11_Comment;
            dbModal.SuperintendedInspectionReport.Section8_12_Condition = Modal.Section8_12_Condition;
            dbModal.SuperintendedInspectionReport.Section8_12_Comment = Modal.Section8_12_Comment;
            dbModal.SuperintendedInspectionReport.Section8_13_Condition = Modal.Section8_13_Condition;
            dbModal.SuperintendedInspectionReport.Section8_13_Comment = Modal.Section8_13_Comment;
            dbModal.SuperintendedInspectionReport.Section8_14_Condition = Modal.Section8_14_Condition;
            dbModal.SuperintendedInspectionReport.Section8_14_Comment = Modal.Section8_14_Comment;
            dbModal.SuperintendedInspectionReport.Section8_15_Condition = Modal.Section8_15_Condition;
            dbModal.SuperintendedInspectionReport.Section8_15_Comment = Modal.Section8_15_Comment;
            dbModal.SuperintendedInspectionReport.Section8_16_Condition = Modal.Section8_16_Condition;
            dbModal.SuperintendedInspectionReport.Section8_16_Comment = Modal.Section8_16_Comment;
            dbModal.SuperintendedInspectionReport.Section8_17_Condition = Modal.Section8_17_Condition;
            dbModal.SuperintendedInspectionReport.Section8_17_Comment = Modal.Section8_17_Comment;
            dbModal.SuperintendedInspectionReport.Section8_18_Condition = Modal.Section8_18_Condition;
            dbModal.SuperintendedInspectionReport.Section8_18_Comment = Modal.Section8_18_Comment;
            dbModal.SuperintendedInspectionReport.Section8_19_Condition = Modal.Section8_19_Condition;
            dbModal.SuperintendedInspectionReport.Section8_19_Comment = Modal.Section8_19_Comment;
            dbModal.SuperintendedInspectionReport.Section8_20_Condition = Modal.Section8_20_Condition;
            dbModal.SuperintendedInspectionReport.Section8_20_Comment = Modal.Section8_20_Comment;
            dbModal.SuperintendedInspectionReport.Section8_21_Condition = Modal.Section8_21_Condition;
            dbModal.SuperintendedInspectionReport.Section8_21_Comment = Modal.Section8_21_Comment;
            dbModal.SuperintendedInspectionReport.Section8_22_Condition = Modal.Section8_22_Condition;
            dbModal.SuperintendedInspectionReport.Section8_22_Comment = Modal.Section8_22_Comment;
            dbModal.SuperintendedInspectionReport.Section8_23_Condition = Modal.Section8_23_Condition;
            dbModal.SuperintendedInspectionReport.Section8_23_Comment = Modal.Section8_23_Comment;
            dbModal.SuperintendedInspectionReport.Section8_24_Condition = Modal.Section8_24_Condition;
            dbModal.SuperintendedInspectionReport.Section8_24_Comment = Modal.Section8_24_Comment;
            dbModal.SuperintendedInspectionReport.Section8_25_Condition = Modal.Section8_25_Condition;
            dbModal.SuperintendedInspectionReport.Section8_25_Comment = Modal.Section8_25_Comment;

            dbModal.SuperintendedInspectionReport.Section9_1_Condition = Modal.Section9_1_Condition;
            dbModal.SuperintendedInspectionReport.Section9_1_Comment = Modal.Section9_1_Comment;
            dbModal.SuperintendedInspectionReport.Section9_2_Condition = Modal.Section9_2_Condition;
            dbModal.SuperintendedInspectionReport.Section9_2_Comment = Modal.Section9_2_Comment;
            dbModal.SuperintendedInspectionReport.Section9_3_Condition = Modal.Section9_3_Condition;
            dbModal.SuperintendedInspectionReport.Section9_3_Comment = Modal.Section9_3_Comment;
            dbModal.SuperintendedInspectionReport.Section9_4_Condition = Modal.Section9_4_Condition;
            dbModal.SuperintendedInspectionReport.Section9_4_Comment = Modal.Section9_4_Comment;
            dbModal.SuperintendedInspectionReport.Section9_5_Condition = Modal.Section9_5_Condition;
            dbModal.SuperintendedInspectionReport.Section9_5_Comment = Modal.Section9_5_Comment;
            dbModal.SuperintendedInspectionReport.Section9_6_Condition = Modal.Section9_6_Condition;
            dbModal.SuperintendedInspectionReport.Section9_6_Comment = Modal.Section9_6_Comment;
            dbModal.SuperintendedInspectionReport.Section9_7_Condition = Modal.Section9_7_Condition;
            dbModal.SuperintendedInspectionReport.Section9_7_Comment = Modal.Section9_7_Comment;
            dbModal.SuperintendedInspectionReport.Section9_8_Condition = Modal.Section9_8_Condition;
            dbModal.SuperintendedInspectionReport.Section9_8_Comment = Modal.Section9_8_Comment;
            dbModal.SuperintendedInspectionReport.Section9_9_Condition = Modal.Section9_9_Condition;
            dbModal.SuperintendedInspectionReport.Section9_9_Comment = Modal.Section9_9_Comment;
            dbModal.SuperintendedInspectionReport.Section9_10_Condition = Modal.Section9_10_Condition;
            dbModal.SuperintendedInspectionReport.Section9_10_Comment = Modal.Section9_10_Comment;
            dbModal.SuperintendedInspectionReport.Section9_11_Condition = Modal.Section9_11_Condition;
            dbModal.SuperintendedInspectionReport.Section9_11_Comment = Modal.Section9_11_Comment;
            dbModal.SuperintendedInspectionReport.Section9_12_Condition = Modal.Section9_12_Condition;
            dbModal.SuperintendedInspectionReport.Section9_12_Comment = Modal.Section9_12_Comment;
            dbModal.SuperintendedInspectionReport.Section9_13_Condition = Modal.Section9_13_Condition;
            dbModal.SuperintendedInspectionReport.Section9_13_Comment = Modal.Section9_13_Comment;
            dbModal.SuperintendedInspectionReport.Section9_14_Condition = Modal.Section9_14_Condition;
            dbModal.SuperintendedInspectionReport.Section9_14_Comment = Modal.Section9_14_Comment;
            dbModal.SuperintendedInspectionReport.Section9_15_Condition = Modal.Section9_15_Condition;
            dbModal.SuperintendedInspectionReport.Section9_15_Comment = Modal.Section9_15_Comment;

            // RDBJ 02/15/2022
            dbModal.SuperintendedInspectionReport.Section9_16_Condition = Modal.Section9_16_Condition;
            dbModal.SuperintendedInspectionReport.Section9_16_Comment = Modal.Section9_16_Comment;
            dbModal.SuperintendedInspectionReport.Section9_17_Condition = Modal.Section9_17_Condition;
            dbModal.SuperintendedInspectionReport.Section9_17_Comment = Modal.Section9_17_Comment;
            // End RDBJ 02/15/2022

            dbModal.SuperintendedInspectionReport.Section10_1_Condition = Modal.Section10_1_Condition;
            dbModal.SuperintendedInspectionReport.Section10_1_Comment = Modal.Section10_1_Comment;
            dbModal.SuperintendedInspectionReport.Section10_2_Condition = Modal.Section10_2_Condition;
            dbModal.SuperintendedInspectionReport.Section10_2_Comment = Modal.Section10_2_Comment;
            dbModal.SuperintendedInspectionReport.Section10_3_Condition = Modal.Section10_3_Condition;
            dbModal.SuperintendedInspectionReport.Section10_3_Comment = Modal.Section10_3_Comment;
            dbModal.SuperintendedInspectionReport.Section10_4_Condition = Modal.Section10_4_Condition;
            dbModal.SuperintendedInspectionReport.Section10_4_Comment = Modal.Section10_4_Comment;
            dbModal.SuperintendedInspectionReport.Section10_5_Condition = Modal.Section10_5_Condition;
            dbModal.SuperintendedInspectionReport.Section10_5_Comment = Modal.Section10_5_Comment;
            dbModal.SuperintendedInspectionReport.Section10_6_Condition = Modal.Section10_6_Condition;
            dbModal.SuperintendedInspectionReport.Section10_6_Comment = Modal.Section10_6_Comment;
            dbModal.SuperintendedInspectionReport.Section10_7_Condition = Modal.Section10_7_Condition;
            dbModal.SuperintendedInspectionReport.Section10_7_Comment = Modal.Section10_7_Comment;
            dbModal.SuperintendedInspectionReport.Section10_8_Condition = Modal.Section10_8_Condition;
            dbModal.SuperintendedInspectionReport.Section10_8_Comment = Modal.Section10_8_Comment;
            dbModal.SuperintendedInspectionReport.Section10_9_Condition = Modal.Section10_9_Condition;
            dbModal.SuperintendedInspectionReport.Section10_9_Comment = Modal.Section10_9_Comment;
            dbModal.SuperintendedInspectionReport.Section10_10_Condition = Modal.Section10_10_Condition;
            dbModal.SuperintendedInspectionReport.Section10_10_Comment = Modal.Section10_10_Comment;
            dbModal.SuperintendedInspectionReport.Section10_11_Condition = Modal.Section10_11_Condition;
            dbModal.SuperintendedInspectionReport.Section10_11_Comment = Modal.Section10_11_Comment;
            dbModal.SuperintendedInspectionReport.Section10_12_Condition = Modal.Section10_12_Condition;
            dbModal.SuperintendedInspectionReport.Section10_12_Comment = Modal.Section10_12_Comment;
            dbModal.SuperintendedInspectionReport.Section10_13_Condition = Modal.Section10_13_Condition;
            dbModal.SuperintendedInspectionReport.Section10_13_Comment = Modal.Section10_13_Comment;
            dbModal.SuperintendedInspectionReport.Section10_14_Condition = Modal.Section10_14_Condition;
            dbModal.SuperintendedInspectionReport.Section10_14_Comment = Modal.Section10_14_Comment;
            dbModal.SuperintendedInspectionReport.Section10_15_Condition = Modal.Section10_15_Condition;
            dbModal.SuperintendedInspectionReport.Section10_15_Comment = Modal.Section10_15_Comment;
            dbModal.SuperintendedInspectionReport.Section10_16_Condition = Modal.Section10_16_Condition;
            dbModal.SuperintendedInspectionReport.Section10_16_Comment = Modal.Section10_16_Comment;

            dbModal.SuperintendedInspectionReport.Section11_1_Condition = Modal.Section11_1_Condition;
            dbModal.SuperintendedInspectionReport.Section11_1_Comment = Modal.Section11_1_Comment;
            dbModal.SuperintendedInspectionReport.Section11_2_Condition = Modal.Section11_2_Condition;
            dbModal.SuperintendedInspectionReport.Section11_2_Comment = Modal.Section11_2_Comment;
            dbModal.SuperintendedInspectionReport.Section11_3_Condition = Modal.Section11_3_Condition;
            dbModal.SuperintendedInspectionReport.Section11_3_Comment = Modal.Section11_3_Comment;
            dbModal.SuperintendedInspectionReport.Section11_4_Condition = Modal.Section11_4_Condition;
            dbModal.SuperintendedInspectionReport.Section11_4_Comment = Modal.Section11_4_Comment;
            dbModal.SuperintendedInspectionReport.Section11_5_Condition = Modal.Section11_5_Condition;
            dbModal.SuperintendedInspectionReport.Section11_5_Comment = Modal.Section11_5_Comment;
            dbModal.SuperintendedInspectionReport.Section11_6_Condition = Modal.Section11_6_Condition;
            dbModal.SuperintendedInspectionReport.Section11_6_Comment = Modal.Section11_6_Comment;
            dbModal.SuperintendedInspectionReport.Section11_7_Condition = Modal.Section11_7_Condition;
            dbModal.SuperintendedInspectionReport.Section11_7_Comment = Modal.Section11_7_Comment;
            dbModal.SuperintendedInspectionReport.Section11_8_Condition = Modal.Section11_8_Condition;
            dbModal.SuperintendedInspectionReport.Section11_8_Comment = Modal.Section11_8_Comment;

            dbModal.SuperintendedInspectionReport.Section12_1_Condition = Modal.Section12_1_Condition;
            dbModal.SuperintendedInspectionReport.Section12_1_Comment = Modal.Section12_1_Comment;
            dbModal.SuperintendedInspectionReport.Section12_2_Condition = Modal.Section12_2_Condition;
            dbModal.SuperintendedInspectionReport.Section12_2_Comment = Modal.Section12_2_Comment;
            dbModal.SuperintendedInspectionReport.Section12_3_Condition = Modal.Section12_3_Condition;
            dbModal.SuperintendedInspectionReport.Section12_3_Comment = Modal.Section12_3_Comment;
            dbModal.SuperintendedInspectionReport.Section12_4_Condition = Modal.Section12_4_Condition;
            dbModal.SuperintendedInspectionReport.Section12_4_Comment = Modal.Section12_4_Comment;
            dbModal.SuperintendedInspectionReport.Section12_5_Condition = Modal.Section12_5_Condition;
            dbModal.SuperintendedInspectionReport.Section12_5_Comment = Modal.Section12_5_Comment;
            dbModal.SuperintendedInspectionReport.Section12_6_Condition = Modal.Section12_6_Condition;
            dbModal.SuperintendedInspectionReport.Section12_6_Comment = Modal.Section12_6_Comment;

            dbModal.SuperintendedInspectionReport.Section13_1_Condition = Modal.Section13_1_Condition;
            dbModal.SuperintendedInspectionReport.Section13_1_Comment = Modal.Section13_1_Comment;
            dbModal.SuperintendedInspectionReport.Section13_2_Condition = Modal.Section13_2_Condition;
            dbModal.SuperintendedInspectionReport.Section13_2_Comment = Modal.Section13_2_Comment;
            dbModal.SuperintendedInspectionReport.Section13_3_Condition = Modal.Section13_3_Condition;
            dbModal.SuperintendedInspectionReport.Section13_3_Comment = Modal.Section13_3_Comment;
            dbModal.SuperintendedInspectionReport.Section13_4_Condition = Modal.Section13_4_Condition;
            dbModal.SuperintendedInspectionReport.Section13_4_Comment = Modal.Section13_4_Comment;

            dbModal.SuperintendedInspectionReport.Section14_1_Condition = Modal.Section14_1_Condition;
            dbModal.SuperintendedInspectionReport.Section14_1_Comment = Modal.Section14_1_Comment;
            dbModal.SuperintendedInspectionReport.Section14_2_Condition = Modal.Section14_2_Condition;
            dbModal.SuperintendedInspectionReport.Section14_2_Comment = Modal.Section14_2_Comment;
            dbModal.SuperintendedInspectionReport.Section14_3_Condition = Modal.Section14_3_Condition;
            dbModal.SuperintendedInspectionReport.Section14_3_Comment = Modal.Section14_3_Comment;
            dbModal.SuperintendedInspectionReport.Section14_4_Condition = Modal.Section14_4_Condition;
            dbModal.SuperintendedInspectionReport.Section14_4_Comment = Modal.Section14_4_Comment;
            dbModal.SuperintendedInspectionReport.Section14_5_Condition = Modal.Section14_5_Condition;
            dbModal.SuperintendedInspectionReport.Section14_5_Comment = Modal.Section14_5_Comment;
            dbModal.SuperintendedInspectionReport.Section14_6_Condition = Modal.Section14_6_Condition;
            dbModal.SuperintendedInspectionReport.Section14_6_Comment = Modal.Section14_6_Comment;
            dbModal.SuperintendedInspectionReport.Section14_7_Condition = Modal.Section14_7_Condition;
            dbModal.SuperintendedInspectionReport.Section14_7_Comment = Modal.Section14_7_Comment;
            dbModal.SuperintendedInspectionReport.Section14_8_Condition = Modal.Section14_8_Condition;
            dbModal.SuperintendedInspectionReport.Section14_8_Comment = Modal.Section14_8_Comment;
            dbModal.SuperintendedInspectionReport.Section14_9_Condition = Modal.Section14_9_Condition;
            dbModal.SuperintendedInspectionReport.Section14_9_Comment = Modal.Section14_9_Comment;
            dbModal.SuperintendedInspectionReport.Section14_10_Condition = Modal.Section14_10_Condition;
            dbModal.SuperintendedInspectionReport.Section14_10_Comment = Modal.Section14_10_Comment;
            dbModal.SuperintendedInspectionReport.Section14_11_Condition = Modal.Section14_11_Condition;
            dbModal.SuperintendedInspectionReport.Section14_11_Comment = Modal.Section14_11_Comment;
            dbModal.SuperintendedInspectionReport.Section14_12_Condition = Modal.Section14_12_Condition;
            dbModal.SuperintendedInspectionReport.Section14_12_Comment = Modal.Section14_12_Comment;
            dbModal.SuperintendedInspectionReport.Section14_13_Condition = Modal.Section14_13_Condition;
            dbModal.SuperintendedInspectionReport.Section14_13_Comment = Modal.Section14_13_Comment;
            dbModal.SuperintendedInspectionReport.Section14_14_Condition = Modal.Section14_14_Condition;
            dbModal.SuperintendedInspectionReport.Section14_14_Comment = Modal.Section14_14_Comment;
            dbModal.SuperintendedInspectionReport.Section14_15_Condition = Modal.Section14_15_Condition;
            dbModal.SuperintendedInspectionReport.Section14_15_Comment = Modal.Section14_15_Comment;
            dbModal.SuperintendedInspectionReport.Section14_16_Condition = Modal.Section14_16_Condition;
            dbModal.SuperintendedInspectionReport.Section14_16_Comment = Modal.Section14_16_Comment;
            dbModal.SuperintendedInspectionReport.Section14_17_Condition = Modal.Section14_17_Condition;
            dbModal.SuperintendedInspectionReport.Section14_17_Comment = Modal.Section14_17_Comment;
            dbModal.SuperintendedInspectionReport.Section14_18_Condition = Modal.Section14_18_Condition;
            dbModal.SuperintendedInspectionReport.Section14_18_Comment = Modal.Section14_18_Comment;
            dbModal.SuperintendedInspectionReport.Section14_19_Condition = Modal.Section14_19_Condition;
            dbModal.SuperintendedInspectionReport.Section14_19_Comment = Modal.Section14_19_Comment;
            dbModal.SuperintendedInspectionReport.Section14_20_Condition = Modal.Section14_20_Condition;
            dbModal.SuperintendedInspectionReport.Section14_20_Comment = Modal.Section14_20_Comment;
            dbModal.SuperintendedInspectionReport.Section14_21_Condition = Modal.Section14_21_Condition;
            dbModal.SuperintendedInspectionReport.Section14_21_Comment = Modal.Section14_21_Comment;
            dbModal.SuperintendedInspectionReport.Section14_22_Condition = Modal.Section14_22_Condition;
            dbModal.SuperintendedInspectionReport.Section14_22_Comment = Modal.Section14_22_Comment;
            dbModal.SuperintendedInspectionReport.Section14_23_Condition = Modal.Section14_23_Condition;
            dbModal.SuperintendedInspectionReport.Section14_23_Comment = Modal.Section14_23_Comment;
            dbModal.SuperintendedInspectionReport.Section14_24_Condition = Modal.Section14_24_Condition;
            dbModal.SuperintendedInspectionReport.Section14_24_Comment = Modal.Section14_24_Comment;
            dbModal.SuperintendedInspectionReport.Section14_25_Condition = Modal.Section14_25_Condition;
            dbModal.SuperintendedInspectionReport.Section14_25_Comment = Modal.Section14_25_Comment;

            dbModal.SuperintendedInspectionReport.Section15_1_Condition = Modal.Section15_1_Condition;
            dbModal.SuperintendedInspectionReport.Section15_1_Comment = Modal.Section15_1_Comment;
            dbModal.SuperintendedInspectionReport.Section15_2_Condition = Modal.Section15_2_Condition;
            dbModal.SuperintendedInspectionReport.Section15_2_Comment = Modal.Section15_2_Comment;
            dbModal.SuperintendedInspectionReport.Section15_3_Condition = Modal.Section15_3_Condition;
            dbModal.SuperintendedInspectionReport.Section15_3_Comment = Modal.Section15_3_Comment;
            dbModal.SuperintendedInspectionReport.Section15_4_Condition = Modal.Section15_4_Condition;
            dbModal.SuperintendedInspectionReport.Section15_4_Comment = Modal.Section15_4_Comment;
            dbModal.SuperintendedInspectionReport.Section15_5_Condition = Modal.Section15_5_Condition;
            dbModal.SuperintendedInspectionReport.Section15_5_Comment = Modal.Section15_5_Comment;
            dbModal.SuperintendedInspectionReport.Section15_6_Comment = Modal.Section15_6_Comment;
            dbModal.SuperintendedInspectionReport.Section15_7_Condition = Modal.Section15_7_Condition;
            dbModal.SuperintendedInspectionReport.Section15_7_Comment = Modal.Section15_7_Comment;
            dbModal.SuperintendedInspectionReport.Section15_8_Condition = Modal.Section15_8_Condition;
            dbModal.SuperintendedInspectionReport.Section15_8_Comment = Modal.Section15_8_Comment;
            dbModal.SuperintendedInspectionReport.Section15_9_Condition = Modal.Section15_9_Condition;
            dbModal.SuperintendedInspectionReport.Section15_9_Comment = Modal.Section15_9_Comment;
            dbModal.SuperintendedInspectionReport.Section15_10_Condition = Modal.Section15_10_Condition;
            dbModal.SuperintendedInspectionReport.Section15_10_Comment = Modal.Section15_10_Comment;
            dbModal.SuperintendedInspectionReport.Section15_11_Condition = Modal.Section15_11_Condition;
            dbModal.SuperintendedInspectionReport.Section15_11_Comment = Modal.Section15_11_Comment;
            dbModal.SuperintendedInspectionReport.Section15_12_Condition = Modal.Section15_12_Condition;
            dbModal.SuperintendedInspectionReport.Section15_12_Comment = Modal.Section15_12_Comment;
            dbModal.SuperintendedInspectionReport.Section15_13_Condition = Modal.Section15_13_Condition;
            dbModal.SuperintendedInspectionReport.Section15_13_Comment = Modal.Section15_13_Comment;
            dbModal.SuperintendedInspectionReport.Section15_14_Condition = Modal.Section15_14_Condition;
            dbModal.SuperintendedInspectionReport.Section15_14_Comment = Modal.Section15_14_Comment;
            dbModal.SuperintendedInspectionReport.Section15_15_Condition = Modal.Section15_15_Condition;
            dbModal.SuperintendedInspectionReport.Section15_15_Comment = Modal.Section15_15_Comment;

            dbModal.SuperintendedInspectionReport.Section16_1_Condition = Modal.Section16_1_Condition;
            dbModal.SuperintendedInspectionReport.Section16_1_Comment = Modal.Section16_1_Comment;
            dbModal.SuperintendedInspectionReport.Section16_2_Condition = Modal.Section16_2_Condition;
            dbModal.SuperintendedInspectionReport.Section16_2_Comment = Modal.Section16_2_Comment;
            dbModal.SuperintendedInspectionReport.Section16_3_Condition = Modal.Section16_3_Condition;
            dbModal.SuperintendedInspectionReport.Section16_3_Comment = Modal.Section16_3_Comment;
            dbModal.SuperintendedInspectionReport.Section16_4_Condition = Modal.Section16_4_Condition;
            dbModal.SuperintendedInspectionReport.Section16_4_Comment = Modal.Section16_4_Comment;

            dbModal.SuperintendedInspectionReport.Section17_1_Condition = Modal.Section17_1_Condition;
            dbModal.SuperintendedInspectionReport.Section17_1_Comment = Modal.Section17_1_Comment;
            dbModal.SuperintendedInspectionReport.Section17_2_Condition = Modal.Section17_2_Condition;
            dbModal.SuperintendedInspectionReport.Section17_2_Comment = Modal.Section17_2_Comment;
            dbModal.SuperintendedInspectionReport.Section17_3_Condition = Modal.Section17_3_Condition;
            dbModal.SuperintendedInspectionReport.Section17_3_Comment = Modal.Section17_3_Comment;
            dbModal.SuperintendedInspectionReport.Section17_4_Condition = Modal.Section17_4_Condition;
            dbModal.SuperintendedInspectionReport.Section17_4_Comment = Modal.Section17_4_Comment;
            dbModal.SuperintendedInspectionReport.Section17_5_Condition = Modal.Section17_5_Condition;
            dbModal.SuperintendedInspectionReport.Section17_5_Comment = Modal.Section17_5_Comment;
            dbModal.SuperintendedInspectionReport.Section17_6_Condition = Modal.Section17_6_Condition;
            dbModal.SuperintendedInspectionReport.Section17_6_Comment = Modal.Section17_6_Comment;

            dbModal.SuperintendedInspectionReport.Section18_1_Condition = Modal.Section18_1_Condition;
            dbModal.SuperintendedInspectionReport.Section18_1_Comment = Modal.Section18_1_Comment;
            dbModal.SuperintendedInspectionReport.Section18_2_Condition = Modal.Section18_2_Condition;
            dbModal.SuperintendedInspectionReport.Section18_2_Comment = Modal.Section18_2_Comment;
            dbModal.SuperintendedInspectionReport.Section18_3_Condition = Modal.Section18_3_Condition;
            dbModal.SuperintendedInspectionReport.Section18_3_Comment = Modal.Section18_3_Comment;
            dbModal.SuperintendedInspectionReport.Section18_4_Condition = Modal.Section18_4_Condition;
            dbModal.SuperintendedInspectionReport.Section18_4_Comment = Modal.Section18_4_Comment;
            dbModal.SuperintendedInspectionReport.Section18_5_Condition = Modal.Section18_5_Condition;
            dbModal.SuperintendedInspectionReport.Section18_5_Comment = Modal.Section18_5_Comment;
            dbModal.SuperintendedInspectionReport.Section18_6_Condition = Modal.Section18_6_Condition;
            dbModal.SuperintendedInspectionReport.Section18_6_Comment = Modal.Section18_6_Comment;
            dbModal.SuperintendedInspectionReport.Section18_7_Condition = Modal.Section18_7_Condition;
            dbModal.SuperintendedInspectionReport.Section18_7_Comment = Modal.Section18_7_Comment;

            // RDBJ 02/15/2022
            dbModal.SuperintendedInspectionReport.Section18_8_Condition = Modal.Section18_8_Condition;
            dbModal.SuperintendedInspectionReport.Section18_8_Comment = Modal.Section18_8_Comment;
            dbModal.SuperintendedInspectionReport.Section18_9_Condition = Modal.Section18_9_Condition;
            dbModal.SuperintendedInspectionReport.Section18_9_Comment = Modal.Section18_9_Comment;
            // End RDBJ 02/15/2022

            dbModal.SuperintendedInspectionReport.IsSynced = Modal.IsSynced;
            dbModal.SuperintendedInspectionReport.CreatedDate = Modal.CreatedDate;
            dbModal.SuperintendedInspectionReport.ModifyDate = Modal.ModifyDate;
            dbModal.SuperintendedInspectionReport.SavedAsDraft = Modal.SavedAsDraft;
        }


        //RDBJ 09/27/2021
        public List<Modals.SuperintendedInspectionReport> getSynchSIRList(
            string strShipCode  // JSL 11/12/2022
            )
        {
            List<Modals.SuperintendedInspectionReport> SyncList = new List<Modals.SuperintendedInspectionReport>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                SyncList = dbContext.SuperintendedInspectionReports
                    .Where(x => x.UniqueFormID != null) // RDBJ 12/17/2021
                    .Select(y => new Modals.SuperintendedInspectionReport()
                {
                    UniqueFormID = y.UniqueFormID,
                    FormVersion = y.FormVersion,
                    ShipName = y.ShipName,
                    IsSynced = y.IsSynced,
                }).ToList();

                // JSL 11/12/2022
                if (!string.IsNullOrEmpty(strShipCode))
                {
                    SyncList = SyncList.Where(x => x.ShipName == strShipCode).ToList();
                }
                // End JSL 11/12/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchSIRList " + ex.Message + "\n" + ex.InnerException);
                SyncList = null;
            }
            return SyncList;
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        public SIRModal getSynchSIR(string UniqueFormID)
        {
            SIRModal dbModal = new SIRModal();
            Guid UFormId = Guid.Parse(UniqueFormID);
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var sirForm = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == UFormId).FirstOrDefault();
                if (sirForm != null)
                {
                    dbModal.SuperintendedInspectionReport = new Modals.SuperintendedInspectionReport();
                    dbModal.GIRDeficiencies = new List<GIRDeficiencies>();
                    dbModal.SIRNote = new List<Modals.SIRNote>();  // RDBJ 12/18/2021
                    dbModal.SIRAdditionalNote = new List<Modals.SIRAdditionalNote>();  // RDBJ 12/18/2021

                    GetSIRFormData(sirForm, ref dbModal);

                    // RDBJ 01/05/2022 wrapped in if
                    if (dbModal.SuperintendedInspectionReport.isDelete == 0)
                    {
                        // RDBJ 12/18/2021
                        var sirNoteList = dbContext.SIRNotes.Where(x => x.UniqueFormID == sirForm.UniqueFormID
                        && x.NotesUniqueID != null  // RDBJ 04/04/2022
                        ).ToList();
                        if (sirNoteList != null && sirNoteList.Count > 0)
                        {
                            foreach (var sirNote in sirNoteList)
                            {
                                Modals.SIRNote sirNoteModal = new Modals.SIRNote();
                                sirNoteModal.Note = sirNote.Note;
                                sirNoteModal.Number = sirNote.Number;
                                sirNoteModal.SIRFormID = 0;
                                sirNoteModal.UniqueFormID = sirNote.UniqueFormID;

                                dbModal.SIRNote.Add(sirNoteModal);
                            }
                        }
                        // End RDBJ 12/18/2021

                        // RDBJ 12/18/2021
                        var sirAddNoteList = dbContext.SIRAdditionalNotes.Where(x => x.UniqueFormID == sirForm.UniqueFormID
                        && x.NotesUniqueID != null  // RDBJ 04/04/2022
                        ).ToList();
                        if (sirAddNoteList != null && sirAddNoteList.Count > 0)
                        {
                            foreach (var sirNote in sirNoteList)
                            {
                                Modals.SIRAdditionalNote sirAddNoteModal = new Modals.SIRAdditionalNote();
                                sirAddNoteModal.Note = sirNote.Note;
                                sirAddNoteModal.Number = sirNote.Number;
                                sirAddNoteModal.SIRFormID = 0;
                                sirAddNoteModal.UniqueFormID = sirNote.UniqueFormID;

                                dbModal.SIRAdditionalNote.Add(sirAddNoteModal);
                            }
                        }
                        // End RDBJ 12/18/2021

                        var defList = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == sirForm.UniqueFormID).ToList();
                        if (defList != null && defList.Count > 0)
                        {
                            foreach (var def in defList)
                            {
                                GIRDeficiencies girDef = new GIRDeficiencies();
                                girDef.GIRDeficienciesComments = new List<DeficienciesNote>();
                                girDef.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>(); //RDBJ 09/24/2021
                                girDef.GIRDeficienciesResolution = new List<Modals.GIRDeficienciesResolution>(); //RDBJ 09/24/2021

                                girDef.No = def.No;
                                girDef.DateRaised = def.DateRaised;
                                girDef.Deficiency = def.Deficiency;
                                girDef.DateClosed = def.DateClosed;
                                girDef.CreatedDate = def.CreatedDate;
                                girDef.UpdatedDate = def.UpdatedDate;
                                girDef.Ship = def.Ship;
                                girDef.IsClose = def.IsClose;
                                girDef.ReportType = def.ReportType;
                                girDef.ItemNo = def.ItemNo;
                                girDef.Section = def.Section;
                                girDef.UniqueFormID = def.UniqueFormID;
                                girDef.isDelete = def.isDelete;
                                girDef.DeficienciesUniqueID = def.DeficienciesUniqueID;
                                girDef.Priority = def.Priority == null ? 12 : def.Priority; //RDBJ 12/18/2021
                                girDef.AssignTo = def.AssignTo; // RDBJ 12/18/2021
                                girDef.DueDate = def.DueDate;   // RDBJ 03/01/2022

                                // RDBJ 12/23/2021 wrapped in if
                                if (def.isDelete == 0)
                                {
                                    var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == girDef.DeficienciesUniqueID).ToList();
                                    if (defFiles != null && defFiles.Count > 0)
                                    {
                                        foreach (var girDefFile in defFiles)
                                        {
                                            Modals.GIRDeficienciesFile defFile = new Modals.GIRDeficienciesFile();
                                            defFile.DeficienciesFileUniqueID = girDefFile.DeficienciesUniqueID; // JSL 06/07/2022
                                            defFile.DeficienciesID = girDefFile.DeficienciesID != null ? girDefFile.DeficienciesID : 0; // RDBJ 01/15/2022 set avoid null error
                                            defFile.DeficienciesUniqueID = girDefFile.DeficienciesUniqueID;
                                            defFile.FileName = girDefFile.FileName;
                                            defFile.StorePath = girDefFile.StorePath;

                                            // JSL 12/04/2022
                                            if (!defFile.StorePath.StartsWith("data:"))
                                            {
                                                defFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defFile.StorePath);
                                            }
                                            // End JSL 12/04/2022

                                            girDef.GIRDeficienciesFile.Add(defFile);
                                        }
                                    }

                                    var defComments = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                    if (defComments != null && defComments.Count > 0)
                                    {
                                        foreach (var defComment in defComments)
                                        {
                                            DeficienciesNote defNote = new DeficienciesNote();
                                            defNote.DeficienciesUniqueID = defComment.DeficienciesUniqueID;
                                            defNote.UserName = defComment.UserName;
                                            defNote.Comment = defComment.Comment;
                                            defNote.CreatedDate = defComment.CreatedDate;
                                            defNote.ModifyDate = defComment.ModifyDate;
                                            defNote.NoteUniqueID = defComment.NoteUniqueID;

                                            var defCommentFiles = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteUniqueID == defNote.NoteUniqueID).ToList();
                                            if (defCommentFiles != null && defCommentFiles.Count > 0)
                                            {
                                                foreach (var defCommmentFile in defCommentFiles)
                                                {
                                                    Modals.GIRDeficienciesCommentFile defComFile = new Modals.GIRDeficienciesCommentFile();
                                                    defComFile.FileName = defCommmentFile.FileName;
                                                    defComFile.StorePath = defCommmentFile.StorePath;
                                                    defComFile.IsUpload = defCommmentFile.IsUpload;
                                                    defComFile.NoteUniqueID = defCommmentFile.NoteUniqueID;
                                                    defComFile.CommentFileUniqueID = defCommmentFile.CommentFileUniqueID;

                                                    // JSL 12/04/2022
                                                    if (!defComFile.StorePath.StartsWith("data:"))
                                                    {
                                                        defComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defComFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    defNote.GIRDeficienciesCommentFile.Add(defComFile);
                                                }
                                            }
                                            girDef.GIRDeficienciesComments.Add(defNote);
                                        }
                                    }

                                    var defIntialActions = dbContext.GIRDeficienciesInitialActions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                    if (defIntialActions != null && defIntialActions.Count > 0)
                                    {
                                        foreach (var defInitial in defIntialActions)
                                        {
                                            GIRDeficienciesInitialActions defIntialAction = new GIRDeficienciesInitialActions(); //RDBJ 09/22/2021 Updateed Modal Name used
                                            defIntialAction.DeficienciesUniqueID = defInitial.DeficienciesUniqueID;
                                            defIntialAction.Name = defInitial.Name;
                                            defIntialAction.Description = defInitial.Description;
                                            defIntialAction.CreatedDate = defInitial.CreatedDate;
                                            defIntialAction.IniActUniqueID = defInitial.IniActUniqueID;

                                            var defIntialActionFiles = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActUniqueID == defInitial.IniActUniqueID).ToList();
                                            if (defIntialActionFiles != null && defIntialActionFiles.Count > 0)
                                            {
                                                foreach (var defIntialActionFile in defIntialActionFiles)
                                                {
                                                    Modals.GIRDeficienciesInitialActionsFile defComFile = new Modals.GIRDeficienciesInitialActionsFile();
                                                    defComFile.FileName = defIntialActionFile.FileName;
                                                    defComFile.StorePath = defIntialActionFile.StorePath;
                                                    defComFile.IsUpload = defIntialActionFile.IsUpload;
                                                    defComFile.IniActUniqueID = defIntialActionFile.IniActUniqueID;
                                                    defComFile.IniActFileUniqueID = defIntialActionFile.IniActFileUniqueID;

                                                    // JSL 12/04/2022
                                                    if (!defComFile.StorePath.StartsWith("data:"))
                                                    {
                                                        defComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defComFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    defIntialAction.GIRDeficienciesInitialActionsFiles.Add(defComFile);
                                                }
                                            }
                                            girDef.GIRDeficienciesInitialActions.Add(defIntialAction); //RDBJ 09/22/2021 Updateed Modal Name used
                                        }
                                    }

                                    var defResolutions = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                    if (defResolutions != null && defResolutions.Count > 0)
                                    {
                                        foreach (var defResolution in defResolutions)
                                        {
                                            Modals.GIRDeficienciesResolution defResolutionModal = new Modals.GIRDeficienciesResolution();
                                            defResolutionModal.DeficienciesUniqueID = defResolution.DeficienciesUniqueID;
                                            defResolutionModal.Name = defResolution.Name;
                                            defResolutionModal.Resolution = defResolution.Resolution;
                                            defResolutionModal.CreatedDate = defResolution.CreatedDate;
                                            defResolutionModal.ResolutionUniqueID = defResolution.ResolutionUniqueID;

                                            var defResolutionFiles = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == defResolution.ResolutionUniqueID).ToList();
                                            if (defResolutionFiles != null && defResolutionFiles.Count > 0)
                                            {
                                                foreach (var defResolutionFile in defResolutionFiles)
                                                {
                                                    Modals.GIRDeficienciesResolutionFile defresFile = new Modals.GIRDeficienciesResolutionFile();
                                                    defresFile.DeficienciesID = defResolutionFile.DeficienciesID;
                                                    defresFile.FileName = defResolutionFile.FileName;
                                                    defresFile.StorePath = defResolutionFile.StorePath;
                                                    defresFile.IsUpload = defResolutionFile.IsUpload;
                                                    defresFile.ResolutionUniqueID = defResolutionFile.ResolutionUniqueID;
                                                    defresFile.ResolutionFileUniqueID = defResolutionFile.ResolutionFileUniqueID;

                                                    // JSL 12/04/2022
                                                    if (!defresFile.StorePath.StartsWith("data:"))
                                                    {
                                                        defresFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defresFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    defResolutionModal.GIRDeficienciesResolutionFiles.Add(defresFile);
                                                }
                                            }
                                            girDef.GIRDeficienciesResolution.Add(defResolutionModal); //RDBJ 09/22/2021 Updateed Modal Name used
                                        }
                                    }
                                }
                            
                                dbModal.GIRDeficiencies.Add(girDef);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchSIR " + ex.Message + "\n" + ex.InnerException);
                dbModal = null;
            }
            return dbModal;
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        public bool sendSynchSIRListUFID(List<string> IdsStr) // RDBJ 01/19/2022 set List<string>
        {
            bool response = false;
            //string[] FormUID = IdsStr.Split(',');
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                for (int i = 0; i < IdsStr.Count; i++) // RDBJ 01/19/2022 set with List
                {
                    Guid UFID = Guid.Parse(IdsStr[i]); // RDBJ 01/19/2022 set with List
                    Entity.SuperintendedInspectionReport girForms = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == UFID).FirstOrDefault();
                    girForms.IsSynced = true;
                }
                dbContext.SaveChanges();
                response = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud sendSynchSIRListUFID " + ex.Message + "\n" + ex.InnerException);
                response = false;
            }
            return response;
        }
        //End RDBJ 09/27/2021

        // JSL 07/16/2022
        public void SendSignalRNotificationCallForTheOffice(string shipCode, bool blnSendNotificationToUserForForm = false)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Dictionary<string, string> dicMetadata = new Dictionary<string, string>();

            int ShipFleetID = (int)dbContext.CSShips.Where(x => x.Code == shipCode).Select(y => y.FleetId).FirstOrDefault();

            //var lstUsers = dbContext.UserProfiles.Where(x => (x.UserGroup == 7 && x.ShipFleetID == ShipFleetID) || x.UserGroup == 1).ToList();  // JSL 07/01/2022 commented this line
            // JSL 07/01/2022
            var lstUsers = dbContext.UserProfiles.Where(x => (x.UserGroup == 7 || x.UserGroup == 1) && x.ShipFleetID == 0).ToList();
            lstUsers.AddRange(dbContext.UserProfiles.Where(x => (x.UserGroup == 7) && x.ShipFleetID == ShipFleetID).ToList());
            // End JSL 07/01/2022

            if (lstUsers != null && lstUsers.Count > 0)
            {
                foreach (var itemUser in lstUsers)
                {
                    NotificationsHelper.SendNotificationsForSignalR(Convert.ToString(itemUser.UserID), itemUser.Email, blnSendNotificationToUserForForm);
                }
            }
        }
        // End JSL 07/16/2022

        // JSL 07/16/2022
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
                case AppStatic.API_METHOD_InsertOrUpdateDeficienciesData:
                    {
                        string strFormType = string.Empty;
                        string strFormUniqueID = string.Empty;
                        string strShipCode = string.Empty;
                        string strDeficiencyData = string.Empty;

                        Guid guidFormUniqueID = Guid.Empty;
                        bool IsNeedToSendNotification = false;
                        List<Modals.GIRDeficiencies> modalDeficiencies = new List<GIRDeficiencies>();
                        try
                        {
                            if (dictMetaData.ContainsKey("FormUniqueID"))
                                strFormUniqueID = dictMetaData["FormUniqueID"];

                            if (dictMetaData.ContainsKey("ShipCode"))
                                strShipCode = dictMetaData["ShipCode"];

                            if (dictMetaData.ContainsKey("DeficienciesData"))
                                strDeficiencyData = dictMetaData["DeficienciesData"];

                            if (!string.IsNullOrEmpty(strDeficiencyData))
                            {
                                Modals.GIRDeficiencies modalDeficiency = new GIRDeficiencies();
                                modalDeficiency = JsonConvert.DeserializeObject<GIRDeficiencies>(strDeficiencyData);
                                if (modalDeficiency != null)
                                {
                                    modalDeficiencies.Add(modalDeficiency);
                                }
                            }
                            
                            SIRDeficiencies_Save(strFormUniqueID, modalDeficiencies
                                , ref IsNeedToSendNotification  // JSL 06/25/2022
                                );

                            if (IsNeedToSendNotification)
                            {
                                SendSignalRNotificationCallForTheOffice(strShipCode);
                            }

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(strFormType + " - " + strFormUniqueID + " : " + AppStatic.API_METHOD_InsertOrUpdateDeficienciesData + " Error : " + ex.Message);
                        }
                        break;
                    }
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
        // End JSL 07/16/2022
    }
}
