using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AuditNote = CarisbrookeShippingAPI.BLL.Modals.AuditNote;
using AuditNotesAttachment = CarisbrookeShippingAPI.BLL.Modals.AuditNotesAttachment;
using IAF = CarisbrookeShippingAPI.BLL.Modals.IAF;
namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class CloudIAFHelper
    {
        #region LocalToCloud
        public bool IAFSynch(Modals.IAF Modal)
        {
            bool res = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.InternalAuditForm dbModal = new Entity.InternalAuditForm();
                bool IsNeedToUpdateAuditNotes = false;  // JSL 04/20/2022
                bool blnSendNotificationToUserForForm = false;    // JSL 05/01/2022
                bool IsNeedToSendNotification = false;  // JSL 06/27/2022 this is send notification to all users

                if (Modal != null && Modal.InternalAuditForm.UniqueFormID != null)
                {
                    // RDBJ 12/17/2021 wrapped in if
                    if (Modal.InternalAuditForm.UniqueFormID != null)
                    {
                        dbModal = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == Modal.InternalAuditForm.UniqueFormID).FirstOrDefault();

                        if (dbModal == null)
                            dbModal = new Entity.InternalAuditForm();

                        // JSL 12/31/2022
                        if (Modal.InternalAuditForm.ShipId == null || Modal.InternalAuditForm.ShipId == 0)
                        {
                            var dbships = dbContext.CSShips.Where(x => x.Code == Modal.InternalAuditForm.ShipName).FirstOrDefault();
                            if (dbships != null)
                            {
                                Modal.InternalAuditForm.ShipId = dbships.ShipId;
                            }
                        }
                        // End JSL 12/31/2022

                        if (dbModal != null && dbModal.UniqueFormID != null)
                        {
                            int IsLocalFormVersionLatest = Decimal.Compare((decimal)Modal.InternalAuditForm.FormVersion, (decimal)dbModal.FormVersion); // JSL 04/20/2022
                            // RDBJ 03/30/2022 Commented if
                            //if (Modal.InternalAuditForm.FormVersion > dbModal.FormVersion) 
                            //{
                                //dbModal.IsSynced = true;
                                //dbContext.SaveChanges();
                            //}

                            // JSL 04/20/2022
                            if (IsLocalFormVersionLatest == 1)
                            {
                                // JSL 05/01/2022
                                if (Modal.InternalAuditForm.SavedAsDraft == false && dbModal.SavedAsDraft == true)
                                {
                                    blnSendNotificationToUserForForm = true;
                                }
                                // End JSL 05/01/2022

                                SetIAFormData(ref dbModal, Modal);
                                dbModal.IsSynced = true;
                                dbContext.SaveChanges();
                                IsNeedToUpdateAuditNotes = true;
                            }
                            // End JSL 04/20/2022
                        }
                        else
                        {
                            SetIAFormData(ref dbModal, Modal);
                            dbModal.IsSynced = true;
                            dbModal.UniqueFormID = Modal.InternalAuditForm.UniqueFormID;
                            dbModal.FormVersion = Modal.InternalAuditForm.FormVersion;
                            dbContext.InternalAuditForms.Add(dbModal);
                            dbContext.SaveChanges();
                            IsNeedToUpdateAuditNotes = true;
                            //blnSendNotificationToUser = true;   // JSL 05/01/2022

                            // JSL 06/27/2022
                            if (dbModal.SavedAsDraft == false)
                            {
                                blnSendNotificationToUserForForm = true;
                            }
                            // End JSL 06/27/2022
                        }

                        // RDBJ 04/20/2022
                        if (IsNeedToUpdateAuditNotes)
                        {
                            // RDBJ 03/08/2022 wrapped in if
                            if (Modal.AuditNote != null && Modal.AuditNote.Count > 0)
                                IAFNotes_Save(Modal.AuditNote //RDBJ 10/05/2021 Removed Convert.ToString(dbModal.UniqueFormID) 
                                    , ref IsNeedToSendNotification  // JSL 06/27/2022
                                    );

                        }
                        // End RDBJ 04/20/2022

                        // JSL 05/01/2022
                        if (blnSendNotificationToUserForForm)
                        {
                            // JSL 06/24/2022
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "5";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(dbModal.UniqueFormID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeForm;
                            dictNotificationData["IsDraft"] = Convert.ToString(dbModal.SavedAsDraft);
                            dictNotificationData["Title"] = AppStatic.IAFFormName;

                            string strDetailsURL = string.Empty;
                            if ((bool)dbModal.SavedAsDraft)
                            {
                                strDetailsURL = "IAFList/DetailsView?id=" + Convert.ToString(dbModal.UniqueFormID);
                            }
                            else
                            {
                                strDetailsURL = "Forms/GeneralInspectionReport";
                            }
                            dictNotificationData["DetailsURL"] = strDetailsURL;

                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData);
                            // End JSL 06/24/2022

                            // JSL 06/24/2022 commentd
                            /*
                            var UsersList = dbContext.UserProfiles
                                .Where(x => x.UserGroup == 1    // 1 for Admin group
                                || x.UserGroup == 5 // 5 for ISM group
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
                                entityModelNotification.Title = AppStatic.IAFFormName;  // JSL 05/13/2022 Updated with IAF

                                if (entityModelNotification.IsDraft)
                                {
                                    entityModelNotification.DetailsURL = "IAFList/DetailsView?id=" + entityModelNotification.UniqueDataId.ToString();
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
                            // End JSL 06/24/2022 commentd
                        }
                        // End JSL 05/01/2022

                        // JSL 06/27/2022
                        if (blnSendNotificationToUserForForm || IsNeedToSendNotification)
                        {
                            SendSignalRNotificationCallForTheOffice(blnSendNotificationToUserForForm: blnSendNotificationToUserForForm);
                        }
                        // End JSL 06/27/2022

                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud IAFSynch CreatedDate : " + Modal.InternalAuditForm.CreatedDate + " & UpdatedDate : " + Modal.InternalAuditForm.UpdatedDate.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch Ship : " + Modal.InternalAuditForm.ShipName + " & ShipId : " + Modal.InternalAuditForm.ShipId.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch Location : " + Modal.InternalAuditForm.Location + " & AuditNo : " + Modal.InternalAuditForm.AuditNo.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch AuditTypeISM : " + Modal.InternalAuditForm.AuditTypeISM + " & AuditTypeISPS : " + Modal.InternalAuditForm.AuditTypeISPS.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch AuditTypeMLC : " + Modal.InternalAuditForm.AuditTypeMLC + " & Date : " + Modal.InternalAuditForm.Date.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch Auditor : " + Modal.InternalAuditForm.Auditor + " & IsSynced : " + Modal.InternalAuditForm.IsSynced.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch FormVersion : " + Modal.InternalAuditForm.FormVersion + " & AuditType : " + Modal.InternalAuditForm.AuditType.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch IsClosed : " + Modal.InternalAuditForm.IsClosed + " & IsAdditional : " + Modal.InternalAuditForm.IsAdditional.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch isDelete : " + Modal.InternalAuditForm.isDelete + " & SavedAsDraft : " + Modal.InternalAuditForm.SavedAsDraft.ToString());    // RDBJ 03/08/2022
                LogHelper.writelog("Cloud IAFSynch : " + ex.Message + " InnerException : " + ex.InnerException.ToString());
            }
            return res;
        }
        public void IAFNotes_Save(List<Modals.AuditNote> auditNotes //RDBJ 10/05/2021 Removed string UniqueFormID
            , ref bool IsNeedToSendNotification // JSL 06/27/2022
            )
        {
            bool blnIfExist = false;
            try
            {
                if (auditNotes != null && auditNotes.Count > 0) //RDBJ 10/05/2021  Removed && Guid.Parse(UniqueFormID) != Guid.Empty
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    foreach (var item in auditNotes)
                    {
                        Entity.AuditNote member = new Entity.AuditNote();
                        member = dbContext.AuditNotes.Where(x => x.NotesUniqueID == item.NotesUniqueID).FirstOrDefault(); //RDBJ 10/05/2021 Replace UniqueFormID to NotesUniqueID
                        
                        if (member != null)
                            blnIfExist = true;
                        else
                            member = new Entity.AuditNote();

                        member.Number = item.Number;
                        member.Type = item.Type;
                        member.BriefDescription = item.BriefDescription;
                        member.DateClosed = item.DateClosed;
                        member.CreatedDate = item.CreatedDate;
                        member.UpdatedDate = item.UpdatedDate;
                        member.Ship = item.Ship;
                        member.Reference = item.Reference;
                        member.FullDescription = item.FullDescription;
                        member.CorrectiveAction = item.CorrectiveAction;
                        member.PreventativeAction = item.PreventativeAction;
                        member.UniqueFormID = item.UniqueFormID;
                        member.Rank = item.Rank;
                        member.Name = item.Name;
                        member.TimeScale = item.TimeScale;
                        member.Name = item.Name;
                        member.Ship = item.Ship;
                        member.IsResolved = item.isResolved;
                        member.DateClosed = item.DateClosed;
                        member.Ship = item.Ship;
                        member.NotesUniqueID = item.NotesUniqueID;
                        member.InternalAuditFormId = item.InternalAuditFormId;

                        member.Priority = item.Priority; //RDBJ 11/25/2021
                        member.isDelete = Convert.ToInt32(item.isDelete); //RDBJ 11/25/2021
                        member.AssignTo = item.AssignTo; // RDBJ 12/21/2021

                        if (blnIfExist) //RDBJ 10/05/2021 If Not Exist
                        {
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            dbContext.AuditNotes.Add(member);
                            dbContext.SaveChanges();
                        }

                        // JSL 12/03/2022
                        Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                        dicFileMetaData["UniqueFormID"] = Convert.ToString(item.UniqueFormID);
                        dicFileMetaData["ReportType"] = "IA";
                        dicFileMetaData["DetailUniqueId"] = Convert.ToString(item.NotesUniqueID);
                        // End JSL 12/03/2022

                        if (item.AuditNotesAttachment != null && item.AuditNotesAttachment.Count > 0)
                        {
                            IAFAuditNotesFile_Save(item.AuditNotesAttachment //RDBJ 10/05/2021 Replaced Function name IAFAuditNotesFile_Save Removed Ship, item.NotesUniqueID parameters and pass AuditNotesAttachment
                                    , dicFileMetaData   // JSL 12/03/2022
                                );
                        }
                        if (item.AuditNotesComment != null && item.AuditNotesComment.Count > 0)
                        {
                            AuditNotesComment_Save(item.AuditNotesComment //RDBJ 10/05/2021 Removed item.NotesUniqueID
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/27/2022
                                );
                        }
                        if (item.AuditNotesResolution != null && item.AuditNotesResolution.Count > 0)
                        {
                            AuditResolution_Save(item.AuditNotesResolution //RDBJ 10/05/2021 Removed item.NotesUniqueID
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/27/2022   
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud IAFNotes_Save : " + ex.Message);
            }
        }
        public void IAFAuditNotesFile_Save(List<Modals.AuditNotesAttachment> modal //RDBJ 10/05/2021 Removed Guid? NotesUniqueID
            , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                //RDBJ 10/05/2021
                foreach (var item in modal)
                {
                    Entity.AuditNotesAttachment file = new Entity.AuditNotesAttachment();
                    file = dbContext.AuditNotesAttachments.Where(x => x.NotesFileUniqueID == item.NotesFileUniqueID).FirstOrDefault();
                    if (file == null)
                    {
                        file = new Entity.AuditNotesAttachment();
                        file.NotesFileUniqueID = item.NotesFileUniqueID;
                        file.NotesUniqueID = item.NotesUniqueID;
                        file.InternalAuditFormId = item.InternalAuditFormId;
                        file.AuditNotesId = item.AuditNotesId;
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

                        dbContext.AuditNotesAttachments.Add(file);
                        dbContext.SaveChanges();
                    }
                }
                //End RDBJ 10/05/2021
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud IAFAuditNotesFile_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }

        public void AuditNotesComment_Save(List<Modals.Audit_Deficiency_Comments> modalDefNotes //RDBJ 10/05/2021 Removed Guid? NotesUniqueID
                , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
                , ref bool IsNeedToSendNotification // JSL 06/27/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modalDefNotes != null && modalDefNotes.Count > 0)
                {
                    foreach (var itemDefNotes in modalDefNotes)
                    {
                        Entity.AuditNotesComment dbModal = new Entity.AuditNotesComment();
                        dbModal = dbContext.AuditNotesComments.Where(x => x.CommentUniqueID == itemDefNotes.CommentUniqueID).FirstOrDefault(); //RDBJ 10/05/2021 Changed NotesUniqueID to CommentUniqueID

                        if (dbModal == null)
                        {
                            Entity.AuditNotesComment defNotes = new Entity.AuditNotesComment();
                            defNotes.CommentUniqueID = itemDefNotes.CommentUniqueID;
                            defNotes.NotesUniqueID = itemDefNotes.NotesUniqueID;
                            defNotes.AuditNoteID = itemDefNotes.AuditNoteID;
                            defNotes.UserName = itemDefNotes.UserName;
                            defNotes.Comment = itemDefNotes.Comment;
                            defNotes.CreatedDate = itemDefNotes.CreatedDate;
                            defNotes.UpdatedDate = itemDefNotes.UpdatedDate;
                            //defNotes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/23/2021
                            defNotes.isNew = 0; // JSL 06/27/2022
                            dbContext.AuditNotesComments.Add(defNotes);
                            dbContext.SaveChanges();

                            // JSL 06/27/2022
                            IsNeedToSendNotification = true;
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "5";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefNotes.NotesUniqueID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeComment;
                            dictNotificationData["IsDraft"] = Convert.ToString(true);
                            dictNotificationData["Title"] = AppStatic.IAFFormName;
                            dictNotificationData["DetailsURL"] = "GIRList/InternalAuditDetails?id=" + Convert.ToString(itemDefNotes.NotesUniqueID);
                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData);
                            // End JSL 06/27/2022
                        }

                        if (itemDefNotes.AuditDeficiencyCommentsFiles != null && itemDefNotes.AuditDeficiencyCommentsFiles.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefNotes.AuditDeficiencyCommentsFiles)
                            {
                                Entity.AuditNotesCommentsFile commentFile = new Entity.AuditNotesCommentsFile();
                                commentFile = dbContext.AuditNotesCommentsFiles.Where(x => x.CommentFileUniqueID == itemCommentFiles.CommentFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.AuditNotesCommentsFile defNotesFile = new Entity.AuditNotesCommentsFile();
                                    defNotesFile.CommentFileUniqueID = itemCommentFiles.CommentFileUniqueID;
                                    defNotesFile.CommentUniqueID = itemCommentFiles.CommentUniqueID;
                                    defNotesFile.CommentsID = itemCommentFiles.CommentsID;
                                    defNotesFile.AuditNoteID = itemCommentFiles.AuditNoteID;
                                    defNotesFile.FileName = itemCommentFiles.FileName;
                                    defNotesFile.StorePath = itemCommentFiles.StorePath;

                                    // JSL 12/03/2022
                                    if (defNotesFile.StorePath.StartsWith("data:"))
                                    {
                                        dicFileMetaData["FileName"] = defNotesFile.FileName;
                                        dicFileMetaData["Base64FileData"] = defNotesFile.StorePath;

                                        // JSL 01/08/2023
                                        dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeComment;
                                        dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentFiles.CommentUniqueID);
                                        // End JSL 01/08/2023

                                        defNotesFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                    }
                                    // End JSL 12/03/2022

                                    dbContext.AuditNotesCommentsFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud AuditNotesComment_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void AuditResolution_Save(List<Modals.Audit_Note_Resolutions> modalDefResolution //RDBJ 10/05/2021 Removed Guid? NotesUniqueID
                , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
                , ref bool IsNeedToSendNotification // JSL 06/27/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modalDefResolution != null && modalDefResolution.Count > 0)
                {
                    foreach (var itemDefRes in modalDefResolution)
                    {
                        Entity.AuditNotesResolution dbModal = new Entity.AuditNotesResolution();
                        dbModal = dbContext.AuditNotesResolutions.Where(x => x.ResolutionUniqueID == itemDefRes.ResolutionUniqueID).FirstOrDefault(); //RDBJ 10/05/2021 Changed NotesUniqueID to ResolutionUniqueID

                        if (dbModal == null)
                        {
                            Entity.AuditNotesResolution defRes = new Entity.AuditNotesResolution();
                            defRes.ResolutionUniqueID = itemDefRes.ResolutionUniqueID;
                            defRes.NotesUniqueID = itemDefRes.NotesUniqueID;
                            defRes.AuditNoteID = itemDefRes.AuditNoteID;
                            defRes.UserName = itemDefRes.UserName;
                            defRes.Resolution = itemDefRes.Resolution;
                            defRes.CreatedDate = itemDefRes.CreatedDate;
                            defRes.UpdatedDate = itemDefRes.UpdatedDate;
                            //defRes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/23/2021
                            defRes.isNew = 0; // JSL 06/27/2022

                            dbContext.AuditNotesResolutions.Add(defRes);
                            dbContext.SaveChanges();

                            // JSL 06/27/2022
                            IsNeedToSendNotification = true;
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "5";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefRes.NotesUniqueID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeResolution;
                            dictNotificationData["IsDraft"] = Convert.ToString(true);
                            dictNotificationData["Title"] = AppStatic.IAFFormName;
                            dictNotificationData["DetailsURL"] = "GIRList/InternalAuditDetails?id=" + Convert.ToString(itemDefRes.NotesUniqueID);
                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData);
                            // End JSL 06/27/2022
                        }

                        if (itemDefRes.AuditNoteResolutionsFiles != null && itemDefRes.AuditNoteResolutionsFiles.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefRes.AuditNoteResolutionsFiles)
                            {
                                Entity.AuditNotesResolutionFile commentFile = new Entity.AuditNotesResolutionFile();
                                commentFile = dbContext.AuditNotesResolutionFiles.Where(x => x.ResolutionFileUniqueID == itemCommentFiles.ResolutionFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.AuditNotesResolutionFile defNotesFile = new Entity.AuditNotesResolutionFile();
                                    defNotesFile.ResolutionFileUniqueID = itemCommentFiles.ResolutionFileUniqueID;
                                    defNotesFile.ResolutionUniqueID = itemCommentFiles.ResolutionUniqueID;
                                    defNotesFile.ResolutionID = itemCommentFiles.ResolutionID;
                                    defNotesFile.AuditNoteID = itemCommentFiles.AuditNoteID;
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

                                    dbContext.AuditNotesResolutionFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud AuditResolution_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        #endregion

        #region Cloud to Local
        public List<IAF> getUnsynchIAFList(
            string strShipCode  // JSL 11/12/2022
            )
        {
            List<IAF> unSyncList = new List<IAF>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                // RDBJ 01/31/2022 Commented due to avoid old IAs
                /*
                var iafList = dbContext.InternalAuditForms
                    .Where(x => x.IsSynced == false
                    && x.UniqueFormID != null   // RDBJ 12/18/2021
                    ).ToList();
                */

                var iafList = dbContext.Database
                    .SqlQuery<Entity.InternalAuditForm>("SELECT * FROM [dbo].[InternalAuditForm] WHERE IsSynced = 0 AND UniqueFormID IS NOT NULL AND CAST(CreatedDate as DATE) != '2022-01-19'").ToList();  // RDBJ 04/19/2022 Remove CAST(CreatedDate as DATE) != '04-03-2022' AND CAST(CreatedDate as DATE) != '04-10-2022' // RDBJ 04/10/2022 AND CAST(CreatedDate as DATE) != '04-10-2022'  // RDBJ 04/03/2022 Added AND CAST(CreatedDate as DATE) != '04-03-2022' // RDBJ 01/31/2022

                if (iafList != null && iafList.Count > 0)
                {
                    // JSL 11/12/2022
                    if (!string.IsNullOrEmpty(strShipCode))
                    {
                        iafList = iafList.Where(x => x.ShipName == strShipCode).ToList();
                    }
                    // End JSL 11/12/2022

                    foreach (var item in iafList)
                    {
                        IAF dbModal = new IAF();
                        dbModal.InternalAuditForm = new Modals.InternalAuditForm();
                        dbModal.AuditNote = new List<AuditNote>();
                        GetIAFFormData(item, ref dbModal);

                        var auditNotesList = dbContext.AuditNotes.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                        if (auditNotesList != null && auditNotesList.Count > 0)
                        {
                            foreach (var def in auditNotesList)
                            {
                                AuditNote auditNote = new AuditNote();
                                auditNote.AuditNotesAttachment = new List<AuditNotesAttachment>();
                                auditNote.AuditNotesComment = new List<Audit_Deficiency_Comments>(); //RDBJ 10/05/2021
                                auditNote.AuditNotesResolution = new List<Audit_Note_Resolutions>(); //RDBJ 10/05/2021

                                auditNote.NotesUniqueID = def.NotesUniqueID;
                                auditNote.UniqueFormID = def.UniqueFormID;
                                auditNote.InternalAuditFormId = def.InternalAuditFormId; //RDBJ 10/05/2021
                                auditNote.Number = def.Number;
                                auditNote.Type = def.Type;
                                auditNote.BriefDescription = def.BriefDescription;
                                auditNote.Reference = def.Reference;
                                auditNote.FullDescription = def.FullDescription;
                                auditNote.CorrectiveAction = def.CorrectiveAction;
                                auditNote.PreventativeAction = def.PreventativeAction;
                                auditNote.Rank = def.Rank;
                                auditNote.Name = def.Name;
                                auditNote.TimeScale = def.TimeScale;
                                auditNote.CreatedDate = def.CreatedDate;
                                auditNote.UpdatedDate = def.UpdatedDate;
                                auditNote.Ship = def.Ship;
                                auditNote.isResolved = def.IsResolved;
                                auditNote.DateClosed = def.DateClosed;
                                auditNote.Priority = def.Priority; //RDBJ 11/25/2021
                                auditNote.isDelete = def.isDelete; //RDBJ 11/25/2021
                                auditNote.AssignTo = def.AssignTo; // RDBJ 12/21/2021

                                // RDBJ 12/23/2021 wrapped in if
                                if (def.isDelete == 0)
                                {
                                    var auditNoteFiles = dbContext.AuditNotesAttachments.Where(x => x.NotesUniqueID == auditNote.NotesUniqueID).ToList();
                                    if (auditNoteFiles != null && auditNoteFiles.Count > 0)
                                    {
                                        foreach (var auditNotesFile in auditNoteFiles)
                                        {
                                            Modals.AuditNotesAttachment audNoteFile = new Modals.AuditNotesAttachment();
                                            audNoteFile.NotesFileUniqueID = auditNotesFile.NotesFileUniqueID;
                                            audNoteFile.NotesUniqueID = auditNotesFile.NotesUniqueID;
                                            audNoteFile.InternalAuditFormId = auditNotesFile.InternalAuditFormId;
                                            audNoteFile.AuditNotesId = auditNotesFile.AuditNotesId;
                                            audNoteFile.FileName = auditNotesFile.FileName;
                                            audNoteFile.StorePath = auditNotesFile.StorePath;

                                            // JSL 12/04/2022
                                            if (!audNoteFile.StorePath.StartsWith("data:"))
                                            {
                                                audNoteFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(audNoteFile.StorePath);
                                            }
                                            // End JSL 12/04/2022

                                            auditNote.AuditNotesAttachment.Add(audNoteFile);
                                        }
                                    }

                                    var auditNoteComments = dbContext.AuditNotesComments.Where(x => x.NotesUniqueID == def.NotesUniqueID).ToList();
                                    if (auditNoteComments != null && auditNoteComments.Count > 0)
                                    {
                                        foreach (var auditNoteComment in auditNoteComments)
                                        {
                                            Audit_Deficiency_Comments audNoteComment = new Audit_Deficiency_Comments();
                                            audNoteComment.CommentUniqueID = auditNoteComment.CommentUniqueID;
                                            audNoteComment.NotesUniqueID = auditNoteComment.NotesUniqueID;
                                            audNoteComment.AuditNoteID = auditNoteComment.AuditNoteID;
                                            audNoteComment.UserName = auditNoteComment.UserName;
                                            audNoteComment.Comment = auditNoteComment.Comment;
                                            audNoteComment.CreatedDate = auditNoteComment.CreatedDate;
                                            audNoteComment.UpdatedDate = auditNoteComment.UpdatedDate;

                                            var auditNoteCommentFiles = dbContext.AuditNotesCommentsFiles.Where(x => x.CommentUniqueID == audNoteComment.CommentUniqueID).ToList();
                                            if (auditNoteCommentFiles != null && auditNoteCommentFiles.Count > 0)
                                            {
                                                foreach (var auditNoteCommmentFile in auditNoteCommentFiles)
                                                {
                                                    Modals.Audit_Deficiency_Comments_Files audComFile = new Modals.Audit_Deficiency_Comments_Files();
                                                    audComFile.CommentFileUniqueID = auditNoteCommmentFile.CommentFileUniqueID;
                                                    audComFile.CommentUniqueID = auditNoteCommmentFile.CommentUniqueID;
                                                    audComFile.CommentsID = auditNoteCommmentFile.CommentsID;
                                                    audComFile.AuditNoteID = auditNoteCommmentFile.AuditNoteID;
                                                    audComFile.FileName = auditNoteCommmentFile.FileName;
                                                    audComFile.StorePath = auditNoteCommmentFile.StorePath;

                                                    // JSL 12/04/2022
                                                    if (!audComFile.StorePath.StartsWith("data:"))
                                                    {
                                                        audComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(audComFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    audNoteComment.AuditDeficiencyCommentsFiles.Add(audComFile);
                                                }
                                            }
                                            auditNote.AuditNotesComment.Add(audNoteComment);
                                        }
                                    }

                                    var auditNoteResolutions = dbContext.AuditNotesResolutions.Where(x => x.NotesUniqueID == def.NotesUniqueID).ToList();
                                    if (auditNoteResolutions != null && auditNoteResolutions.Count > 0)
                                    {
                                        foreach (var auditNoteResolution in auditNoteResolutions)
                                        {
                                            Audit_Note_Resolutions audNoteResolution = new Audit_Note_Resolutions();
                                            audNoteResolution.ResolutionUniqueID = auditNoteResolution.ResolutionUniqueID;
                                            audNoteResolution.NotesUniqueID = auditNoteResolution.NotesUniqueID;
                                            audNoteResolution.AuditNoteID = auditNoteResolution.AuditNoteID;
                                            audNoteResolution.UserName = auditNoteResolution.UserName;
                                            audNoteResolution.Resolution = auditNoteResolution.Resolution;
                                            audNoteResolution.CreatedDate = auditNoteResolution.CreatedDate;
                                            audNoteResolution.UpdatedDate = auditNoteResolution.UpdatedDate;

                                            var auditNoteResolutionFiles = dbContext.AuditNotesResolutionFiles.Where(x => x.ResolutionUniqueID == audNoteResolution.ResolutionUniqueID).ToList();
                                            if (auditNoteResolutionFiles != null && auditNoteResolutionFiles.Count > 0)
                                            {
                                                foreach (var auditNoteResolutionFile in auditNoteResolutionFiles)
                                                {
                                                    Modals.Audit_Note_Resolutions_Files audNoteResolutionFile = new Modals.Audit_Note_Resolutions_Files();
                                                    audNoteResolutionFile.ResolutionFileUniqueID = auditNoteResolutionFile.ResolutionFileUniqueID;
                                                    audNoteResolutionFile.ResolutionUniqueID = auditNoteResolutionFile.ResolutionUniqueID;
                                                    audNoteResolutionFile.ResolutionID = auditNoteResolutionFile.ResolutionID;
                                                    audNoteResolutionFile.AuditNoteID = auditNoteResolutionFile.AuditNoteID;
                                                    audNoteResolutionFile.FileName = auditNoteResolutionFile.FileName;
                                                    audNoteResolutionFile.StorePath = auditNoteResolutionFile.StorePath;

                                                    // JSL 12/04/2022
                                                    if (!audNoteResolutionFile.StorePath.StartsWith("data:"))
                                                    {
                                                        audNoteResolutionFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(audNoteResolutionFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    audNoteResolution.AuditNoteResolutionsFiles.Add(audNoteResolutionFile);
                                                }
                                            }
                                            auditNote.AuditNotesResolution.Add(audNoteResolution);
                                        }
                                    }
                                }

                                dbModal.AuditNote.Add(auditNote);
                            }
                        }
                        unSyncList.Add(dbModal);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getUnsynchIAFList " + ex.Message + "\n" + ex.InnerException);
                unSyncList = null;
            }
            return unSyncList;
        }
        public void SetIAFormData(ref Entity.InternalAuditForm dbModal, Modals.IAF Modal)
        {
            dbModal.CreatedDate = Modal.InternalAuditForm.CreatedDate;
            dbModal.UpdatedDate = Modal.InternalAuditForm.UpdatedDate;
            dbModal.ShipId = string.IsNullOrEmpty(Convert.ToString(Modal.InternalAuditForm.ShipId)) ? null : Modal.InternalAuditForm.ShipId; // RDBJ 03/08/2022 Handle Null  // RDBJ 02/27/2022 Handle null values
            dbModal.ShipName = Modal.InternalAuditForm.ShipName;
            dbModal.Location = Modal.InternalAuditForm.Location;
            dbModal.AuditNo = string.IsNullOrEmpty(Modal.InternalAuditForm.AuditNo) ? "1" : Modal.InternalAuditForm.AuditNo;    // RDBJ 02/23/2022 Avoid Null error
            dbModal.AuditTypeISM = Modal.InternalAuditForm.AuditTypeISM == null ? false : Modal.InternalAuditForm.AuditTypeISM; // RDBJ 02/23/2022 Avoid Null error
            dbModal.AuditTypeISPS = Modal.InternalAuditForm.AuditTypeISPS == null ? false : Modal.InternalAuditForm.AuditTypeISPS;  // RDBJ 02/23/2022 Avoid Null error
            dbModal.AuditTypeMLC = Modal.InternalAuditForm.AuditTypeMLC == null ? false : Modal.InternalAuditForm.AuditTypeMLC; // RDBJ 02/23/2022 Avoid Null error
            dbModal.Date = Modal.InternalAuditForm.Date;
            dbModal.Auditor = Modal.InternalAuditForm.Auditor;
            dbModal.IsSynced = Modal.InternalAuditForm.IsSynced;
            //dbModal.UniqueFormID = Modal.InternalAuditForm.UniqueFormID;
            dbModal.FormVersion = Modal.InternalAuditForm.FormVersion;

            //dbModal.AuditType = string.IsNullOrEmpty(Convert.ToString(Modal.InternalAuditForm.AuditType)) ? 1 : Modal.InternalAuditForm.AuditType; // RDBJ 03/08/2022 Handle Null //RDBJ 11/25/2021
            dbModal.AuditType = Modal.InternalAuditForm.AuditType == 0 ? 1 : Modal.InternalAuditForm.AuditType;  // JSL 04/20/2022
            dbModal.IsClosed = Modal.InternalAuditForm.IsClosed; //RDBJ 11/25/2021
            dbModal.IsAdditional = Modal.InternalAuditForm.IsAdditional; //RDBJ 11/25/2021
            dbModal.isDelete = string.IsNullOrEmpty(Convert.ToString(Modal.InternalAuditForm.isDelete)) ? 0 : Modal.InternalAuditForm.isDelete; // RDBJ 03/08/2022 Handle Null  //RDBJ 11/25/2021
            dbModal.SavedAsDraft = Convert.ToBoolean(Modal.InternalAuditForm.SavedAsDraft); // RDBJ 03/08/2022 Handle Null   // RDBJ 02/06/2022
        }
        public void GetIAFFormData(Entity.InternalAuditForm Modal, ref Modals.IAF dbModal)
        {
            dbModal.InternalAuditForm.UniqueFormID = Modal.UniqueFormID;
            dbModal.InternalAuditForm.FormVersion = Modal.FormVersion;
            dbModal.InternalAuditForm.ShipId = Modal.ShipId;
            dbModal.InternalAuditForm.ShipName = Modal.ShipName;
            dbModal.InternalAuditForm.Location = Modal.Location;
            dbModal.InternalAuditForm.AuditNo = Modal.AuditNo;
            dbModal.InternalAuditForm.AuditTypeISM = Modal.AuditTypeISM;
            dbModal.InternalAuditForm.AuditTypeISPS = Modal.AuditTypeISPS;
            dbModal.InternalAuditForm.AuditTypeMLC = Modal.AuditTypeMLC;
            dbModal.InternalAuditForm.Date = Modal.Date;
            dbModal.InternalAuditForm.Auditor = Modal.Auditor;
            dbModal.InternalAuditForm.CreatedDate = Modal.CreatedDate;
            dbModal.InternalAuditForm.UpdatedDate = Modal.UpdatedDate;
            dbModal.InternalAuditForm.IsSynced = Modal.IsSynced;
            dbModal.InternalAuditForm.Auditor = Modal.Auditor;

            dbModal.InternalAuditForm.AuditType = Modal.AuditType; //RDBJ 11/25/2021
            dbModal.InternalAuditForm.IsClosed = Modal.IsClosed; //RDBJ 11/25/2021
            dbModal.InternalAuditForm.IsAdditional = Modal.IsAdditional; //RDBJ 11/25/2021
            dbModal.InternalAuditForm.isDelete = Modal.isDelete; //RDBJ 11/25/2021
            dbModal.InternalAuditForm.SavedAsDraft = Modal.SavedAsDraft; // RDBJ 02/06/2022
        }
        public bool sendSynchIAFListUFID(List<string> IdsStr) // RDBJ 01/19/2022 set List<string>
        {
            bool response = false;
            //string[] FormUID = IdsStr.Split(',');
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                for (int i = 0; i < IdsStr.Count; i++)  // RDBJ 01/19/2022 set with List
                {
                    Guid UFID = Guid.Parse(IdsStr[i]);  // RDBJ 01/19/2022 set with List
                    Entity.InternalAuditForm girForms = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == UFID).FirstOrDefault();
                    girForms.IsSynced = true;
                }
                dbContext.SaveChanges();
                response = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud sendSynchIAFListUFID " + ex.Message + "\n" + ex.InnerException);
                response = false;
            }
            return response;
        }
        #endregion Cloud to Local

        //RDBJ 10/05/2021
        public List<Modals.InternalAuditForm> getSynchIAFList(
            string strShipCode  // JSL 11/12/2022
            )
        {
            List<Modals.InternalAuditForm> SyncList = new List<Modals.InternalAuditForm>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                // RDBJ 01/31/2022 Commented due to avoid old IAs
                /*
                SyncList = dbContext.InternalAuditForms
                    .Where(x => x.UniqueFormID != null) // RDBJ 12/17/2021
                    .Select(y => new Modals.InternalAuditForm()
                {
                    UniqueFormID = y.UniqueFormID,
                    FormVersion = y.FormVersion,
                    ShipName = y.ShipName,
                    IsSynced = y.IsSynced,
                }).ToList();
                */

                SyncList = dbContext.Database
                   .SqlQuery<Modals.InternalAuditForm>("SELECT [UniqueFormID] ,[FormVersion] ,[InternalAuditFormId] ,[ShipId] ,[ShipName] ,[Location] ,[AuditNo] ,[AuditTypeISM] ,[AuditTypeISPS] ,[AuditTypeMLC] ,[Date] ,[Auditor] ,[CreatedDate] ,[UpdatedDate] ,[IsSynced] ,[isDelete] ,[AuditType] ,[IsAdditional] ,[IsClosed] ,[SavedAsDraft] FROM [dbo].[InternalAuditForm] WHERE UniqueFormID IS NOT NULL AND CAST(CreatedDate as DATE) != '2022-01-19'").ToList(); // RDBJ 04/19/2022 Remove CAST(CreatedDate as DATE) != '04-03-2022' AND CAST(CreatedDate as DATE) != '04-10-2022' // RDBJ 04/10/2022 AND CAST(CreatedDate as DATE) != '04-10-2022'  // RDBJ 04/03/2022 Added AND CAST(CreatedDate as DATE) != '04-03-2022'   // RDBJ 01/31/2022

                // JSL 11/12/2022
                if (!string.IsNullOrEmpty(strShipCode))
                {
                    SyncList = SyncList.Where(x => x.ShipName == strShipCode).ToList();
                }
                // End JSL 11/12/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchIAFList " + ex.Message + "\n" + ex.InnerException);
                SyncList = null;
            }
            return SyncList;
        }
        //End RDBJ 10/05/2021

        //RDBJ 10/05/2021
        public IAF getSynchIAF(string UniqueFormID)
        {
            IAF dbModal = new IAF();
            Guid UFormId = Guid.Parse(UniqueFormID);
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var IAFForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == UFormId).FirstOrDefault();
                if (IAFForm != null)
                {
                    dbModal.InternalAuditForm = new Modals.InternalAuditForm();
                    dbModal.AuditNote = new List<AuditNote>();
                    GetIAFFormData(IAFForm, ref dbModal);

                    var auditNotesList = dbContext.AuditNotes.Where(x => x.UniqueFormID == IAFForm.UniqueFormID).ToList();
                    if (auditNotesList != null && auditNotesList.Count > 0)
                    {
                        foreach (var def in auditNotesList)
                        {
                            AuditNote auditNote = new AuditNote();
                            auditNote.AuditNotesAttachment = new List<AuditNotesAttachment>();
                            auditNote.AuditNotesComment = new List<Audit_Deficiency_Comments>(); //RDBJ 10/05/2021
                            auditNote.AuditNotesResolution = new List<Audit_Note_Resolutions>(); //RDBJ 10/05/2021

                            auditNote.Number = def.Number;
                            auditNote.Type = def.Type;
                            auditNote.BriefDescription = def.BriefDescription;
                            auditNote.DateClosed = def.DateClosed;
                            auditNote.CreatedDate = def.CreatedDate;
                            auditNote.UpdatedDate = def.UpdatedDate;
                            auditNote.Reference = def.Reference;
                            auditNote.FullDescription = def.FullDescription;
                            auditNote.CorrectiveAction = def.CorrectiveAction;
                            auditNote.PreventativeAction = def.PreventativeAction;
                            auditNote.Rank = def.Rank;
                            auditNote.Name = def.Name;
                            auditNote.TimeScale = def.TimeScale;
                            auditNote.UniqueFormID = def.UniqueFormID;
                            auditNote.UpdatedDate = def.UpdatedDate;
                            auditNote.Ship = def.Ship;
                            auditNote.isResolved = def.IsResolved;
                            auditNote.NotesUniqueID = def.NotesUniqueID;
                            auditNote.InternalAuditFormId = def.InternalAuditFormId; //RDBJ 10/05/2021
                            auditNote.Priority = def.Priority; //RDBJ 11/25/2021
                            auditNote.isDelete = def.isDelete; //RDBJ 11/25/2021
                            auditNote.AssignTo = def.AssignTo; // RDBJ 12/21/2021

                            // RDBJ 12/23/2021 wrapped in if
                            if (def.isDelete == 0)
                            {
                                var auditNoteFiles = dbContext.AuditNotesAttachments.Where(x => x.NotesUniqueID == auditNote.NotesUniqueID).ToList();
                                if (auditNoteFiles != null && auditNoteFiles.Count > 0)
                                {
                                    foreach (var auditNotesFile in auditNoteFiles)
                                    {
                                        Modals.AuditNotesAttachment audNoteFile = new Modals.AuditNotesAttachment();
                                        audNoteFile.NotesFileUniqueID = auditNotesFile.NotesFileUniqueID;
                                        audNoteFile.NotesUniqueID = auditNotesFile.NotesUniqueID;
                                        audNoteFile.InternalAuditFormId = auditNotesFile.InternalAuditFormId;
                                        audNoteFile.AuditNotesId = auditNotesFile.AuditNotesId;
                                        audNoteFile.FileName = auditNotesFile.FileName;
                                        audNoteFile.StorePath = auditNotesFile.StorePath;

                                        // JSL 12/04/2022
                                        if (!audNoteFile.StorePath.StartsWith("data:"))
                                        {
                                            audNoteFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(audNoteFile.StorePath);
                                        }
                                        // End JSL 12/04/2022

                                        auditNote.AuditNotesAttachment.Add(audNoteFile);
                                    }
                                }

                                var auditNoteComments = dbContext.AuditNotesComments.Where(x => x.NotesUniqueID == def.NotesUniqueID).ToList();
                                if (auditNoteComments != null && auditNoteComments.Count > 0)
                                {
                                    foreach (var auditNoteComment in auditNoteComments)
                                    {
                                        Audit_Deficiency_Comments audNoteComment = new Audit_Deficiency_Comments();
                                        audNoteComment.CommentUniqueID = auditNoteComment.CommentUniqueID;
                                        audNoteComment.NotesUniqueID = auditNoteComment.NotesUniqueID;
                                        audNoteComment.AuditNoteID = auditNoteComment.AuditNoteID;
                                        audNoteComment.UserName = auditNoteComment.UserName;
                                        audNoteComment.Comment = auditNoteComment.Comment;
                                        audNoteComment.CreatedDate = auditNoteComment.CreatedDate;
                                        audNoteComment.UpdatedDate = auditNoteComment.UpdatedDate;

                                        var auditNoteCommentFiles = dbContext.AuditNotesCommentsFiles.Where(x => x.CommentUniqueID == audNoteComment.CommentUniqueID).ToList();
                                        if (auditNoteCommentFiles != null && auditNoteCommentFiles.Count > 0)
                                        {
                                            foreach (var auditNoteCommmentFile in auditNoteCommentFiles)
                                            {
                                                Modals.Audit_Deficiency_Comments_Files audComFile = new Modals.Audit_Deficiency_Comments_Files();
                                                audComFile.CommentFileUniqueID = auditNoteCommmentFile.CommentFileUniqueID;
                                                audComFile.CommentUniqueID = auditNoteCommmentFile.CommentUniqueID;
                                                audComFile.CommentsID = auditNoteCommmentFile.CommentsID;
                                                audComFile.AuditNoteID = auditNoteCommmentFile.AuditNoteID;
                                                audComFile.FileName = auditNoteCommmentFile.FileName;
                                                audComFile.StorePath = auditNoteCommmentFile.StorePath;

                                                // JSL 12/04/2022
                                                if (!audComFile.StorePath.StartsWith("data:"))
                                                {
                                                    audComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(audComFile.StorePath);
                                                }
                                                // End JSL 12/04/2022

                                                audNoteComment.AuditDeficiencyCommentsFiles.Add(audComFile);
                                            }
                                        }
                                        auditNote.AuditNotesComment.Add(audNoteComment);
                                    }
                                }

                                var auditNoteResolutions = dbContext.AuditNotesResolutions.Where(x => x.NotesUniqueID == def.NotesUniqueID).ToList();
                                if (auditNoteResolutions != null && auditNoteResolutions.Count > 0)
                                {
                                    foreach (var auditNoteResolution in auditNoteResolutions)
                                    {
                                        Audit_Note_Resolutions audNoteResolution = new Audit_Note_Resolutions();
                                        audNoteResolution.ResolutionUniqueID = auditNoteResolution.ResolutionUniqueID;
                                        audNoteResolution.NotesUniqueID = auditNoteResolution.NotesUniqueID;
                                        audNoteResolution.AuditNoteID = auditNoteResolution.AuditNoteID;
                                        audNoteResolution.UserName = auditNoteResolution.UserName;
                                        audNoteResolution.Resolution = auditNoteResolution.Resolution;
                                        audNoteResolution.CreatedDate = auditNoteResolution.CreatedDate;
                                        audNoteResolution.UpdatedDate = auditNoteResolution.UpdatedDate;

                                        var auditNoteResolutionFiles = dbContext.AuditNotesResolutionFiles.Where(x => x.ResolutionUniqueID == audNoteResolution.ResolutionUniqueID).ToList();
                                        if (auditNoteResolutionFiles != null && auditNoteResolutionFiles.Count > 0)
                                        {
                                            foreach (var auditNoteResolutionFile in auditNoteResolutionFiles)
                                            {
                                                Modals.Audit_Note_Resolutions_Files audNoteResolutionFile = new Modals.Audit_Note_Resolutions_Files();
                                                audNoteResolutionFile.ResolutionFileUniqueID = auditNoteResolutionFile.ResolutionFileUniqueID;
                                                audNoteResolutionFile.ResolutionUniqueID = auditNoteResolutionFile.ResolutionUniqueID;
                                                audNoteResolutionFile.ResolutionID = auditNoteResolutionFile.ResolutionID;
                                                audNoteResolutionFile.AuditNoteID = auditNoteResolutionFile.AuditNoteID;
                                                audNoteResolutionFile.FileName = auditNoteResolutionFile.FileName;
                                                audNoteResolutionFile.StorePath = auditNoteResolutionFile.StorePath;

                                                // JSL 12/04/2022
                                                if (!audNoteResolutionFile.StorePath.StartsWith("data:"))
                                                {
                                                    audNoteResolutionFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(audNoteResolutionFile.StorePath);
                                                }
                                                // End JSL 12/04/2022

                                                audNoteResolution.AuditNoteResolutionsFiles.Add(audNoteResolutionFile);
                                            }
                                        }
                                        auditNote.AuditNotesResolution.Add(audNoteResolution);
                                    }
                                }
                            }

                            dbModal.AuditNote.Add(auditNote);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchIAF " + ex.Message + "\n" + ex.InnerException);
                dbModal = null;
            }
            return dbModal;
        }
        //End RDBJ 10/05/2021

        #region MLC, SMR, SSP References syncing
        // JSL 05/20/2022
        public List<Dictionary<string, object>> GetReferencesDataList(string strTableName)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Dictionary<string, object>> dictReferencesList = new List<Dictionary<string, object>>();
            string strSQLQuery = string.Empty;

            try
            {
                switch (strTableName.ToLower())
                {
                    case "mlc":
                        try
                        {
                            strSQLQuery = "SELECT * FROM [dbo].[MLCRegulationTree]";
                            dictReferencesList = GetReferencesDataListBySQLQuery(strSQLQuery);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("Cloud GetReferencesDataList " + strTableName + " : " + ex.Message + "\n" + ex.InnerException);
                        }
                        break;
                    case "sms":
                        try
                        {
                            strSQLQuery = "SELECT * FROM [dbo].[SMSReferencesTree]";
                            dictReferencesList = GetReferencesDataListBySQLQuery(strSQLQuery);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("Cloud GetReferencesDataList " + strTableName + " : " + ex.Message + "\n" + ex.InnerException);
                        }
                        break;
                    case "ssp":
                        try
                        {
                            strSQLQuery = "SELECT * FROM [dbo].[SSPReferenceTree]";
                            dictReferencesList = GetReferencesDataListBySQLQuery(strSQLQuery);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("Cloud GetReferencesDataList " + strTableName + " : " + ex.Message + "\n" + ex.InnerException);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GetReferencesDataList " + ex.Message + "\n" + ex.InnerException);
                return null;
            }
            return dictReferencesList;
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public static List<Dictionary<string, object>> GetReferencesDataListBySQLQuery(string strSQLQuery)
        {
            List<Dictionary<string, object>> lstDict = new List<Dictionary<string, object>>();
            try
            {

                string sql = strSQLQuery;
                SqlParameter[] sqlParam = new SqlParameter[] {};
                DataSet ds = CommonAssetsDB.RecordDataSet(sql, sqlParam, CommandType.Text);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dict = Enumerable.Range(0, ds.Tables[0].Columns.Count).ToDictionary(j => ds.Tables[0].Columns[j].ColumnName, j => ds.Tables[0].Rows[i].ItemArray[j]);
                        lstDict.Add(dict);
                    }

                }
                else
                    lstDict = null;
            }
            catch (Exception ex)
            {
                lstDict = null;
            }
            return lstDict;
        }
        // End JSL 05/20/2022
        #endregion

        // JSL 07/16/2022
        public void SendSignalRNotificationCallForTheOffice(string shipCode = "", bool blnSendNotificationToUserForForm = false)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            var lstUsers = dbContext.UserProfiles.Where(x => x.UserGroup == 1 || x.UserGroup == 5).ToList();
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
                        List<Modals.AuditNote> modalDeficiencies = new List<AuditNote>();
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
                                Modals.AuditNote modalDeficiency = new AuditNote();
                                modalDeficiency = JsonConvert.DeserializeObject<AuditNote>(strDeficiencyData);
                                if (modalDeficiency != null)
                                {
                                    modalDeficiencies.Add(modalDeficiency);
                                }
                            }

                            IAFNotes_Save(modalDeficiencies
                                    , ref IsNeedToSendNotification
                                    );

                            if (IsNeedToSendNotification)
                            {
                                SendSignalRNotificationCallForTheOffice();
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
