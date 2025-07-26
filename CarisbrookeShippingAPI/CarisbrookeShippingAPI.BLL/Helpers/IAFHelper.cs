using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class IAFHelper
    {
        // JSL 10/15/2022
        public static HttpContext httpContext = HttpContext.Current;  
        public static string authHeader = httpContext.Request.Headers["Authorization"];
        public static Dictionary<string, string> _requestedUserDetails = Utility.GetRequestedUserFromAuthorization(authHeader);
        public string _Username = _requestedUserDetails["Username"];
        // JSL 10/15/2022

        //RDBJ 11/19/2021
        public void IAFAutoSave(IAF Modal)
        {
            try
            {
                IAFSubmitOrAutoSave(Modal);

                // RDBJ 02/09/2022 Wrapped in if
                if (Modal.AuditNote != null)
                    Save_IAFAuditNote(Modal.AuditNote, Modal.InternalAuditForm.UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAutoSave : " + ex.Message + " : " + ex.InnerException);
            }
        }
        //End RDBJ 11/19/2021

        public void SubmitIAForm(IAF Modal)
        {
            try
            {
                IAFSubmitOrAutoSave(Modal, true);

                // RDBJ 02/09/2022 Wrapped in if
                if (Modal.AuditNote != null)
                    Save_IAFAuditNote(Modal.AuditNote, Modal.InternalAuditForm.UniqueFormID);

                /*
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.InternalAuditForm dbModal = new Entity.InternalAuditForm();
                
                SetIAFFormDataRefEntity(ref dbModal, Modal.InternalAuditForm);

                dbModal.FormVersion = 1; //RDBJ 11/25/2021
                dbModal.ShipId = 0; //RDBJ 11/23/2021
                dbModal.CreatedDate = Utility.ToDateTimeUtcNow(); //RDBJ 11/23/2021
                dbModal.IsSynced = false; //RDBJ 11/23/2021
                dbModal.UpdatedDate = Utility.ToDateTimeUtcNow(); //RDBJ 11/23/2021
                dbModal.isDelete = 0; //RDBJ 11/23/2021

                dbContext.InternalAuditForms.Add(dbModal);
                dbContext.SaveChanges();
                

                foreach (var item in Modal.AuditNote)
                {
                    Entity.AuditNote obj = new Entity.AuditNote();
                    obj.NotesUniqueID = Guid.NewGuid();
                    obj.UniqueFormID = Modal.InternalAuditForm.UniqueFormID;
                    obj.InternalAuditFormId = Modal.InternalAuditForm.InternalAuditFormId;
                    obj.Number = item.Number;
                    obj.Type = item.Type;
                    obj.BriefDescription = item.BriefDescription;
                    obj.Reference = item.Reference;
                    obj.FullDescription = item.FullDescription;
                    obj.CorrectiveAction = item.CorrectiveAction;
                    obj.PreventativeAction = item.PreventativeAction;
                    obj.Rank = item.Rank;
                    obj.Name = item.Name;
                    obj.TimeScale = item.TimeScale;
                    obj.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    obj.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    obj.Ship = Modal.InternalAuditForm.ShipName;
                    obj.IsResolved = false;
                    obj.Priority = 12; //RDBJ 11/13/2021
                    obj.isDelete = 0; //RDBJ 11/22/2021

                    dbContext.AuditNotes.Add(obj);
                    dbContext.SaveChanges();
                    if (item.AuditNotesAttachment != null)
                    {
                        //SaveImageFileForAuditNotes(item.AuditNotesAttachment, obj.AuditNotesId, obj.InternalAuditFormId);
                        Save_IAFAuditNote_Attachment(item.AuditNotesAttachment, obj.NotesUniqueID);
                    }
                }
                */
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitIAForm : " + ex.Message + " : " + ex.InnerException);
            }
        }

        // RDBJ 01/22/2022
        public void IAFSubmitOrAutoSave(IAF Modal, bool blnIsSubmit = false)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Entity.InternalAuditForm dbModal = new Entity.InternalAuditForm();
            bool blnIsIAFExist = false;
            if (Modal.InternalAuditForm.UniqueFormID != null && Modal.InternalAuditForm.UniqueFormID != Guid.Empty)
            {
                dbModal = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == Modal.InternalAuditForm.UniqueFormID).FirstOrDefault();
            }

            if (dbModal == null)
                dbModal = new Entity.InternalAuditForm();
            else
                blnIsIAFExist = true;

            SetIAFFormDataRefEntity(ref dbModal, Modal.InternalAuditForm
                , blnIsSubmit   // RDBJ 01/23/2022
                );

            dbModal.UpdatedDate = Utility.ToDateTimeUtcNow();
            dbModal.IsSynced = false;

            // JSL 12/31/2022
            if (dbModal.ShipId == null || dbModal.ShipId == 0)
            {
                var dbships = dbContext.CSShips.Where(x => x.Code == dbModal.ShipName).FirstOrDefault();
                if (dbships != null)
                {
                    dbModal.ShipId = dbships.ShipId;
                }
            }
            // End JSL 12/31/2022

            if (blnIsIAFExist
                && dbModal != null && dbModal.UniqueFormID != null)
            {
                dbModal.FormVersion = dbModal.FormVersion + Convert.ToDecimal(0.01);
                dbContext.SaveChanges();
            }
            else
            {
                dbModal.FormVersion = 1;
                dbModal.CreatedDate = Utility.ToDateTimeUtcNow();
                dbModal.isDelete = 0;

                dbContext.InternalAuditForms.Add(dbModal);
                dbContext.SaveChanges();
            }
        }
        // End RDBJ 01/22/2022

        // RDBJ 01/22/2022
        public void Save_IAFAuditNote(List<Modals.AuditNote> auditNotes, Guid? UniqueFormID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            foreach (var item in auditNotes)
            {
                Entity.AuditNote dbAuditNote = new Entity.AuditNote();
                if (item.NotesUniqueID != null && item.NotesUniqueID != Guid.Empty)
                {
                    dbAuditNote = dbContext.AuditNotes.Where(x => x.NotesUniqueID == item.NotesUniqueID).FirstOrDefault();
                }

                // RDBJ 01/22/2022
                bool blnIsAuditNoteExist = false;
                if (dbAuditNote != null)
                {
                    blnIsAuditNoteExist = true;
                }
                else
                    dbAuditNote = new Entity.AuditNote();

                // End RDBJ 01/22/2022

                if (item.NotesUniqueID == null)
                    item.NotesUniqueID = Guid.NewGuid();

                dbAuditNote.NotesUniqueID = item.NotesUniqueID;
                dbAuditNote.UniqueFormID = UniqueFormID;
                dbAuditNote.InternalAuditFormId = 0;
                dbAuditNote.Number = item.Number;
                dbAuditNote.Type = item.Type;
                dbAuditNote.BriefDescription = item.BriefDescription;
                dbAuditNote.Reference = item.Reference;
                dbAuditNote.FullDescription = item.FullDescription;
                dbAuditNote.CorrectiveAction = item.CorrectiveAction;
                dbAuditNote.PreventativeAction = item.PreventativeAction;
                dbAuditNote.Rank = item.Rank;
                dbAuditNote.Name = item.Name;
                dbAuditNote.TimeScale = item.TimeScale;
                dbAuditNote.Ship = item.Ship;
                dbAuditNote.UpdatedDate = Utility.ToDateTimeUtcNow();

                // RDBJ 01/22/2022
                if (!blnIsAuditNoteExist)
                {
                    dbAuditNote.isDelete = 0;
                    dbAuditNote.IsResolved = false;
                    dbAuditNote.Priority = 12;
                    dbAuditNote.CreatedDate = Utility.ToDateTimeUtcNow();

                    dbContext.AuditNotes.Add(dbAuditNote);
                    dbContext.SaveChanges();
                }
                else
                    dbContext.SaveChanges();

                if (item.AuditNotesAttachment != null)
                {
                    Save_IAFAuditNote_Attachment(item.AuditNotesAttachment, item.NotesUniqueID);
                }
                // End RDBJ 01/22/2022
            }
        }
        // End RDBJ 01/22/2022

        public List<Entity.InternalAuditForm> GetAllInternalAuditForm()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Entity.InternalAuditForm> list = new List<Entity.InternalAuditForm>();
            list = dbContext.InternalAuditForms.ToList();
            return list;
        }
        public Dictionary<string, string> GetNumberForNotes(string ship
             , string UniqueFormID   // RDBJ 01/22/2022
            )
        {
            try
            {
                Guid UFID = Guid.Parse(UniqueFormID); // RDBJ 01/22/2022

                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Dictionary<string, string> data = new Dictionary<string, string>();

                // RDBJ 03/14/2022
                string number = (dbContext.AuditNotes.Where(x => x.Ship == ship
                && x.UniqueFormID == UFID 
                && x.isDelete == 0
                ).Count())
                //+ 500)    // JSL 05/27/2022
                .ToString();
                data.Add("ISM-Non Conformity", number);
                data.Add("ISPS-Non Conformity", number);
                data.Add("ISM-Observation", number);
                data.Add("ISPS-Observation", number);
                data.Add("MLC-Deficiency", number);
                // End RDBJ 03/14/2022

                // RDBJ 03/14/2022 Commented this section due to avoid individual
                /*
                string number = (dbContext.AuditNotes.Where(x => x.Ship == ship
                && x.UniqueFormID == UFID   // RDBJ 01/22/2022
                && x.isDelete == 0 //RDBJ 11/22/2021 && x.isDelete == 0
                && x.Type == "ISM-Non Conformity"
                ).Count() //RDBJ 11/22/2021 && x.isDelete == 0
                + 500) // RDBJ 01/24/2022 sum with 500
                .ToString(); 
                data.Add("ISM-Non Conformity", number);

                number = (dbContext.AuditNotes.Where(x => x.Ship == ship
                && x.UniqueFormID == UFID   // RDBJ 01/22/2022
                && x.isDelete == 0 //RDBJ 11/22/2021 && x.isDelete == 0
                && x.Type == "ISPS-Non Conformity"
                ).Count() //RDBJ 11/22/2021 && x.isDelete == 0
                + 500) // RDBJ 01/24/2022 sum with 500
                .ToString(); 
                data.Add("ISPS-Non Conformity", number);

                number = (dbContext.AuditNotes.Where(x => x.Ship == ship
                && x.UniqueFormID == UFID   // RDBJ 01/22/2022
                && x.isDelete == 0 //RDBJ 11/22/2021 && x.isDelete == 0
                && x.Type == "ISM-Observation"
                ).Count() //RDBJ 11/22/2021 && x.isDelete == 0
                + 500) // RDBJ 01/24/2022 sum with 500
                .ToString(); 
                data.Add("ISM-Observation", number);

                number = (dbContext.AuditNotes.Where(x => x.Ship == ship
                && x.UniqueFormID == UFID   // RDBJ 01/22/2022
                && x.isDelete == 0  //RDBJ 11/22/2021 && x.isDelete == 0
                && x.Type == "ISPS-Observation"
                ).Count() //RDBJ 11/22/2021 && x.isDelete == 0
                + 500) // RDBJ 01/24/2022 sum with 500
                .ToString(); 
                data.Add("ISPS-Observation", number);

                number = (dbContext.AuditNotes.Where(x => x.Ship == ship
                && x.UniqueFormID == UFID   // RDBJ 01/22/2022
                && x.isDelete == 0  //RDBJ 11/22/2021 && x.isDelete == 0
                && x.Type == "MLC-Deficiency"
                ).Count() 
                + 500) // RDBJ 01/24/2022 sum with 500
                .ToString(); 
                data.Add("MLC-Deficiency", number);
                */
                // End RDBJ 03/14/2022 Commented this section due to avoid individual

                return data;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNumberForNotes : " + ex.Message + " : " + ex.InnerException);
                return null;
            }
        }
        private static string SaveImageFileForAuditNotes(List<Modals.AuditNotesAttachment> modal, long noteid, long? IAFID)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                foreach (var item in modal)
                {
                    var split = item.StorePath.Split(',');
                    string OrignalString = split.LastOrDefault();
                    if (!string.IsNullOrEmpty(OrignalString))
                    {
                        byte[] imageBytes = Convert.FromBase64String(OrignalString);
                        string rootpath = HttpContext.Current.Server.MapPath("~/IAFNotes/");

                        string subPath = IAFID + "/" + noteid.ToString() + "/";
                        bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                        string CompleteFolderPath = Path.Combine(rootpath + subPath);
                        if (!Directory.Exists(CompleteFolderPath))
                        {
                            Directory.CreateDirectory(CompleteFolderPath);
                        }
                        var imageName = noteid + "_" + DateTime.Now.ToString("MMdddyyyhhmmss") + "_" + item.FileName;
                        File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);

                        Entity.AuditNotesAttachment objAuditNotesAttachment = new Entity.AuditNotesAttachment();
                        objAuditNotesAttachment.AuditNotesId = noteid;
                        objAuditNotesAttachment.InternalAuditFormId = IAFID;
                        objAuditNotesAttachment.FileName = item.FileName;
                        objAuditNotesAttachment.StorePath = "/IAFNotes/" + subPath + imageName;
                        dbContext.AuditNotesAttachments.Add(objAuditNotesAttachment);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveImageFileForAuditNotes " + ex.Message + "\n" + ex.InnerException);
            }
            return "";
        }
        public IAF IAFormDetailsView(Guid? id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            IAF data = new IAF();
            data.InternalAuditForm = new Modals.InternalAuditForm();

            var dbModal = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == id && x.isDelete == 0).FirstOrDefault(); //RDBJ 11/22/2021 && x.isDelete == 0
            SetIAFFormDataRefModal(dbModal, ref data);

            var res = dbContext.AuditNotes.Where(x => x.UniqueFormID == id && x.isDelete == 0).ToList(); //RDBJ 11/22/2021 && x.isDelete == 0
            foreach (var item in res)
            {
                Modals.AuditNote obj = new Modals.AuditNote();
                obj.Ship = item.Ship;   // RDBJ 01/27/2022
                obj.BriefDescription = string.IsNullOrEmpty(item.BriefDescription) ? "" : item.BriefDescription;
                obj.CorrectiveAction = string.IsNullOrEmpty(item.CorrectiveAction) ? "" : item.CorrectiveAction;
                obj.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                obj.FullDescription = string.IsNullOrEmpty(item.FullDescription) ? "" : item.FullDescription;
                obj.InternalAuditFormId = data.InternalAuditForm.InternalAuditFormId;
                obj.UniqueFormID = data.InternalAuditForm.UniqueFormID;
                obj.Name = item.Name;
                obj.Number = item.Number;
                obj.PreventativeAction = string.IsNullOrEmpty(item.PreventativeAction) ? "" : item.PreventativeAction;
                obj.Rank = item.Rank;
                obj.Reference = item.Reference;
                obj.TimeScale = item.TimeScale;
                obj.Type = item.Type;
                obj.AuditNotesId = item.AuditNotesId;
                obj.NotesUniqueID = item.NotesUniqueID;
                obj.isResolved = item.IsResolved == null ? false : item.IsResolved;
                obj.isDelete = item.isDelete; //RDBJ 11/22/2021
                obj.UpdatedDate = item.UpdatedDate; // RDBJ 12/16/2021
                obj.AssignTo = item.AssignTo; // RDBJ 12/21/2021
                obj.Username = item.AssignTo != null ? dbContext.UserProfiles.Where(x => x.UserID == item.AssignTo).Select(y => y.UserName).FirstOrDefault() : string.Empty; // RDBJ 12/21/2021

                var resFile = dbContext.AuditNotesAttachments.Where(x => x.NotesUniqueID == item.NotesUniqueID).ToList(); //RDBJ 10/05/2021 Changed NotesUniqueID from AuditNotesId
                foreach (var list in resFile)
                {
                    Modals.AuditNotesAttachment objAuditNotesAttachment = new Modals.AuditNotesAttachment();
                    objAuditNotesAttachment.NotesFileUniqueID = list.NotesFileUniqueID; //RDBJ 10/05/2021
                    objAuditNotesAttachment.FileName = list.FileName;
                    objAuditNotesAttachment.StorePath = list.StorePath; // RDBJ 01/28/2022

                    //RDBJ 11/13/2021 Commented below not required
                    /*
                    objAuditNotesAttachment.AuditNotesId = list.AuditNotesId;
                    objAuditNotesAttachment.InternalAuditFormId = list.InternalAuditFormId;
                    objAuditNotesAttachment.StorePath = list.StorePath;
                    objAuditNotesAttachment.NotesUniqueID = list.NotesUniqueID;
                    */
                obj.AuditNotesAttachment.Add(objAuditNotesAttachment);
                }
                data.AuditNote.Add(obj);
            }
            return data;
        }

        public void SetIAFFormDataRefEntity(ref Entity.InternalAuditForm dbModal, Modals.InternalAuditForm Modal
            , bool blnIsSubmit = false)
        {
            dbModal.UniqueFormID = Modal.UniqueFormID;
            //dbModal.FormVersion = Modal.FormVersion; //RDBJ 11/24/2021 Commented this line
            //dbModal.InternalAuditFormId = Modal.InternalAuditFormId; //RDBJ 11/19/2021 Commented this line
            //dbModal.ShipId = Modal.ShipId; //RDBJ 11/19/2021 Commented this line
            dbModal.ShipName = Modal.ShipName;
            dbModal.Location = Modal.Location;
            dbModal.AuditNo = Modal.AuditNo;
            dbModal.AuditTypeISM = Modal.AuditTypeISM == null ? false : Modal.AuditTypeISM; //RDBJ 11/19/2021 Handle null value with ternary
            dbModal.AuditTypeISPS = Modal.AuditTypeISPS == null ? false : Modal.AuditTypeISPS; //RDBJ 11/19/2021 Handle null value with ternary
            dbModal.AuditTypeMLC = Modal.AuditTypeMLC == null ? false : Modal.AuditTypeMLC; //RDBJ 11/19/2021 Handle null value with ternary
            dbModal.Date = Modal.Date;
            dbModal.Auditor = Modal.Auditor;
            //dbModal.CreatedDate = Modal.CreatedDate; //RDBJ 11/19/2021 Commented this line
            dbModal.UpdatedDate = Modal.UpdatedDate;
            dbModal.IsSynced = Modal.IsSynced;

            // RDBJ 01/22/2022
            int intAuditType = (int)(object)Modal.AuditType;
            if (intAuditType == 0)  // RDBJ 04/05/2022 Set 0 
            {
                Modal.AuditType = 1;
            }
            // End RDBJ 01/22/2022

            dbModal.AuditType = Modal.AuditType; //RDBJ 11/24/2021
            dbModal.IsAdditional = Modal.IsAdditional; //RDBJ 11/24/2021
            dbModal.IsClosed = Modal.IsClosed; //RDBJ 11/24/2021

            // RDBJ 01/23/2022
            if (Modal.SavedAsDraft == null)
            {
                dbModal.SavedAsDraft = !blnIsSubmit;
            }
            else
            {
                dbModal.SavedAsDraft = Modal.SavedAsDraft == null ? false : (bool)Modal.SavedAsDraft;
            }
            // End RDBJ 01/23/2022

            if (blnIsSubmit)
                dbModal.SavedAsDraft = !blnIsSubmit;

            //dbModal.SavedAsDraft = Modal.SavedAsDraft == null ? false : !blnIsSubmit;   // RDBJ 01/28/2022
        }
        public void SetIAFFormDataRefModal(Entity.InternalAuditForm dbModal, ref Modals.IAF Modal)
        {
            Modal.InternalAuditForm.UniqueFormID = dbModal.UniqueFormID;
            Modal.InternalAuditForm.FormVersion = dbModal.FormVersion;
            Modal.InternalAuditForm.InternalAuditFormId = dbModal.InternalAuditFormId;
            Modal.InternalAuditForm.ShipId = dbModal.ShipId;
            Modal.InternalAuditForm.ShipName = dbModal.ShipName;
            Modal.InternalAuditForm.Location = dbModal.Location;
            Modal.InternalAuditForm.AuditNo = dbModal.AuditNo;
            Modal.InternalAuditForm.AuditTypeISM = dbModal.AuditTypeISM;
            Modal.InternalAuditForm.AuditTypeISPS = dbModal.AuditTypeISPS;
            Modal.InternalAuditForm.AuditTypeMLC = dbModal.AuditTypeMLC;
            Modal.InternalAuditForm.Date = dbModal.Date;
            Modal.InternalAuditForm.Auditor = dbModal.Auditor;
            Modal.InternalAuditForm.CreatedDate = dbModal.CreatedDate;
            Modal.InternalAuditForm.UpdatedDate = dbModal.UpdatedDate;
            Modal.InternalAuditForm.IsSynced = dbModal.IsSynced;
            Modal.InternalAuditForm.AuditType = dbModal.AuditType; //RDBJ 11/24/2021
            Modal.InternalAuditForm.IsAdditional = dbModal.IsAdditional; //RDBJ 11/24/2021
            Modal.InternalAuditForm.IsClosed = dbModal.IsClosed; //RDBJ 11/24/2021
            Modal.InternalAuditForm.SavedAsDraft = dbModal.SavedAsDraft; // RDBJ 01/23/2022
        }
        public Modals.AuditNote GetAuditNotesById(Guid id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.AuditNote obj = new Modals.AuditNote();
            try
            {
                var item = dbContext.AuditNotes.Where(x => x.NotesUniqueID == id && x.isDelete == 0).FirstOrDefault(); //RDBJ 11/22/2021 && x.isDelete == 0
                obj.NotesUniqueID = item.NotesUniqueID; // JSL 12/03/2022
                obj.BriefDescription = item.BriefDescription;
                obj.CorrectiveAction = item.CorrectiveAction;
                obj.CreatedDate = item.CreatedDate;
                obj.UpdatedDate = item.UpdatedDate;
                obj.FullDescription = string.IsNullOrEmpty(item.FullDescription) ? "" : item.FullDescription;
                obj.InternalAuditFormId = item.InternalAuditFormId;
                obj.UniqueFormID = item.UniqueFormID;
                obj.Name = item.Name;
                obj.Number = item.Number;
                obj.PreventativeAction = item.PreventativeAction;
                obj.Rank = item.Rank;
                obj.Reference = item.Reference;
                obj.TimeScale = item.TimeScale;
                obj.Type = item.Type;
                obj.AuditNotesId = item.AuditNotesId;
                obj.Ship = item.Ship;
                obj.ShipName = dbContext.CSShips.Where(x => x.Code == item.Ship).Select(x => x.Name).FirstOrDefault();  // JSL 06/13/2022
                obj.isResolved = item.IsResolved == null ? false : item.IsResolved;
                obj.DateClosed = item.DateClosed;
                obj.Priority = item.Priority == null ? 12 : item.Priority; //RDBJ 11/13/2021
                obj.isDelete = item.isDelete; //RDBJ 11/22/2021
                obj.AssignTo = item.AssignTo; // RDBJ 12/21/2021

                // JSL 12/03/2022
                List<Entity.AuditNotesAttachment> dbNotFiles = dbContext.AuditNotesAttachments.Where(x => x.NotesUniqueID == id).ToList();
                if (dbNotFiles != null && dbNotFiles.Count > 0)
                {
                    foreach (var itemNotFile in dbNotFiles)
                    {
                        Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                        dicFileMetaData["UniqueFormID"] = Convert.ToString(obj.UniqueFormID);
                        dicFileMetaData["ReportType"] = "IA";
                        dicFileMetaData["DetailUniqueId"] = Convert.ToString(obj.NotesUniqueID);

                        if (itemNotFile.StorePath.StartsWith("data:"))
                        {
                            dicFileMetaData["FileName"] = itemNotFile.FileName;
                            dicFileMetaData["Base64FileData"] = itemNotFile.StorePath;

                            itemNotFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                            dbContext.SaveChanges();
                        }
                    }
                }
                // End JSL 12/03/2022

                obj.AuditNotesAttachment = dbContext.AuditNotesAttachments.Where(x => x.NotesUniqueID == id).Select(x => new Modals.AuditNotesAttachment()
                {
                    FileName = x.FileName,
                    NotesFileUniqueID = x.NotesFileUniqueID, //RDBJ 10/05/2021
                    AuditNotesAttachmentId = x.AuditNotesAttachmentId,   // JSL 05/10/2022
                    StorePath = x.StorePath,    // JSL 12/03/2022 uncommented
                    //RDBJ 11/13/2021 Commented below not required
                    /*
                    InternalAuditFormId = x.InternalAuditFormId,
                    AuditNotesId = x.AuditNotesId,
                    AuditNotesAttachmentId = x.AuditNotesAttachmentId,
                    NotesUniqueID = x.NotesUniqueID,
                    */
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNotesById " + ex.Message + "\n" + ex.InnerException);
            }
            return obj;
        }
        public bool AddAuditNoteResolutions(Audit_Note_Resolutions data)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                AuditNotesResolution dbResolution = new AuditNotesResolution();
                dbResolution.AuditNoteID = 0; //data.AuditNoteID; //RDBJ 10/05/2021 Set 0
                dbResolution.UserName = data.UserName;
                dbResolution.Resolution = data.Resolution;
                dbResolution.NotesUniqueID = data.NotesUniqueID;
                dbResolution.ResolutionUniqueID = Guid.NewGuid();
                dbResolution.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbResolution.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbResolution.isNew = 0; //RDBJ 10/21/2021
                dbContext.AuditNotesResolutions.Add(dbResolution);
                dbContext.SaveChanges();

                if (data.AuditNoteResolutionsFiles != null)
                {
                    SaveAuditNoteResolutionsFiles(data.AuditNoteResolutionsFiles, dbResolution.ResolutionID, dbResolution.ResolutionUniqueID);
                }

                //RDBJ 10/05/2021
                Entity.AuditNote AudNoteDetails = dbContext.AuditNotes.Where(x => x.NotesUniqueID == data.NotesUniqueID).FirstOrDefault();
                if (AudNoteDetails != null)
                {
                    Entity.InternalAuditForm IAFForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == AudNoteDetails.UniqueFormID).FirstOrDefault();
                    IAFForm.FormVersion = IAFForm.FormVersion + Convert.ToDecimal(0.01);
                    IAFForm.IsSynced = false;
                    dbContext.SaveChanges();
                }
                //End RDBJ 10/05/2021

                AudNoteDetails.UpdatedDate = Utility.ToDateTimeUtcNow(); //RDBJ 10/23/2021
                dbContext.SaveChanges(); //RDBJ 10/23/2021

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesInitialActions " + ex.Message + "\n" + ex.InnerException);
                return false;
            }
        }
        public string SaveAuditNoteResolutionsFiles(List<Audit_Note_Resolutions_Files> modal, long ResolutionID, Guid? resolutionUniqueID)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                foreach (var item in modal)
                {
                    if (item.IsUpload == "true")
                    {
                        var split = item.StorePath.Split(',');
                        string OrignalString = split.LastOrDefault();
                        if (!string.IsNullOrEmpty(OrignalString))
                        {
                            byte[] imageBytes = Convert.FromBase64String(OrignalString);
                            string rootpath = HttpContext.Current.Server.MapPath("~/InternalAudit/AuditNoteResolutions/");

                            string subPath = item.AuditNoteID.ToString() + "/" + ResolutionID.ToString() + "/";
                            bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                            string CompleteFolderPath = Path.Combine(rootpath + subPath);
                            if (!Directory.Exists(CompleteFolderPath))
                            {
                                Directory.CreateDirectory(CompleteFolderPath);
                            }
                            var imageName = Guid.NewGuid().ToString() + "_" + item.FileName;
                            File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);

                            AuditNotesResolutionFile file = new AuditNotesResolutionFile();
                            file.FileName = item.FileName;
                            file.StorePath = item.StorePath; //RDBJ 10/05/2021 store bytes //"/InternalAudit/AuditNoteResolutions/" + subPath + imageName;
                            file.ResolutionID = 0; //ResolutionID; //RDBJ 10/05/2021 Set 0
                            file.ResolutionUniqueID = resolutionUniqueID;
                            file.ResolutionFileUniqueID = Guid.NewGuid();
                            file.AuditNoteID = 0; //item.AuditNoteID; //RDBJ 10/05/2021 Set 0
                            dbContext.AuditNotesResolutionFiles.Add(file);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveAuditNoteResolutionsFiles " + ex.Message + "\n" + ex.InnerException);
            }
            return "";
        }

        public List<Audit_Note_Resolutions> GetAuditNoteResolutions(Guid NoteUniqueID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Audit_Note_Resolutions> Resolutions = new List<Audit_Note_Resolutions>();
            try
            {
                List<AuditNotesResolution> dbResolutions = dbContext.AuditNotesResolutions.Where(x => x.NotesUniqueID == NoteUniqueID).ToList();
                if (dbResolutions != null && dbResolutions.Count > 0)
                {
                    Resolutions = dbResolutions.OrderByDescending(x => x.CreatedDate).Select(x => new Audit_Note_Resolutions()
                    {
                        ResolutionID = x.ResolutionID,
                        ResolutionUniqueID = x.ResolutionUniqueID,
                        NotesUniqueID = x.NotesUniqueID,
                        AuditNoteID = x.AuditNoteID,
                        UserName = x.UserName,
                        Resolution = x.Resolution,
                        CreatedDate = x.CreatedDate,
                        isNew = x.isNew, //RDBJ 10/21/2021
                    }).ToList();
                    foreach (var item in Resolutions)
                    {
                        item.AuditNoteResolutionsFiles = GetAuditNoteResolutionFilesByResolutionID(item.ResolutionUniqueID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditDeficiencyComments " + ex.Message + "\n" + ex.InnerException);
            }
            return Resolutions;
        }
        public List<Audit_Note_Resolutions_Files> GetAuditNoteResolutionFilesByResolutionID(Guid? ResolutionUniqueID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Audit_Note_Resolutions_Files> ResolutionFiles = new List<Audit_Note_Resolutions_Files>();
            try
            {
                List<AuditNotesResolutionFile> dbResolutionsFiles = dbContext.AuditNotesResolutionFiles.Where(x => x.ResolutionUniqueID == ResolutionUniqueID).ToList();
                if (dbResolutionsFiles != null && dbResolutionsFiles.Count > 0)
                {
                    ResolutionFiles = dbResolutionsFiles.Select(x => new Audit_Note_Resolutions_Files()
                    {
                        ResolutionFileUniqueID = x.ResolutionFileUniqueID,
                        FileName = x.FileName

                        //RDBJ 11/13/2021 Commented below not required
                        /*
                        ResolutionFileID = x.ResolutionFileID,
                        ResolutionUniqueID = x.ResolutionUniqueID,
                        ResolutionID = x.ResolutionID,
                        AuditNoteID = x.AuditNoteID,
                        StorePath = x.StorePath,
                        */
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNoteResolutionFilesByResolutionID " + ex.Message + "\n" + ex.InnerException);
            }
            return ResolutionFiles;
        }
        public List<Audit_Note_Resolutions_Files> GetAuditNoteResolutionFiles(long NoteID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Audit_Note_Resolutions_Files> ResolutionFiles = new List<Audit_Note_Resolutions_Files>();
            try
            {
                List<AuditNotesResolutionFile> dbResolution = dbContext.AuditNotesResolutionFiles.Where(x => x.AuditNoteID == NoteID).ToList();
                if (dbResolution != null && dbResolution.Count > 0)
                {
                    ResolutionFiles = dbResolution.Select(x => new Audit_Note_Resolutions_Files()
                    {
                        ResolutionFileID = x.ResolutionFileID,
                        ResolutionID = x.ResolutionID,
                        AuditNoteID = x.AuditNoteID,
                        FileName = x.FileName,
                        StorePath = x.StorePath,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNoteResolutionFiles " + ex.Message + "\n" + ex.InnerException);
            }
            return ResolutionFiles;
        }

        public Dictionary<string, string> GetFileAuditNoteResolution(string ResolutionFileID) // RDBJ 01/27/2022 set with dictionary
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid FileUniqueID = Guid.Parse(ResolutionFileID);
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                var data = dbContext.AuditNotesResolutionFiles.Where(x => x.ResolutionFileUniqueID == FileUniqueID).FirstOrDefault();
                if (data != null)
                {
                    retDicData["FileData"] = data.StorePath; // RDBJ 01/27/2022 set with dictionary
                    retDicData["FileName"] = data.FileName; // RDBJ 01/27/2022 set with dictionary
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFileAuditNoteResolution " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }

        //RDBJ 11/13/2021
        public bool UpdateIAFAuditNotePriority(string NotesUniqueID, int PriorityWeek)
        {
            bool response = false;
            try
            {
                if (!string.IsNullOrEmpty(NotesUniqueID))
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    Guid UFID = Guid.Parse(NotesUniqueID);

                    Entity.AuditNote IAFAuditNote = dbContext.AuditNotes.Where(x => x.NotesUniqueID == UFID).FirstOrDefault();
                    if (IAFAuditNote != null)
                    {
                        IAFAuditNote.Priority = PriorityWeek;
                        dbContext.SaveChanges();

                        Entity.InternalAuditForm IAFData = new Entity.InternalAuditForm();
                        IAFData = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == IAFAuditNote.UniqueFormID).FirstOrDefault();
                        IAFData.FormVersion = IAFData.FormVersion + Convert.ToDecimal(0.01);
                        IAFData.IsSynced = false;
                        dbContext.SaveChanges();
                    }
                    response = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateIAFAuditNotePriority " + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 11/13/2021

        // RDBJ 11/17/2021
        public void AddIAFAuditNote(Modals.AuditNote Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

            try
            {
                if (Modal.UniqueFormID != null && Modal.UniqueFormID != Guid.Empty)
                {
                    Entity.AuditNote member = new Entity.AuditNote();
                    member.NotesUniqueID = Modal.NotesUniqueID;
                    member.UniqueFormID = Modal.UniqueFormID;
                    member.InternalAuditFormId = 0;
                    member.Number = Modal.Number;
                    member.Type = Modal.Type;
                    member.BriefDescription = Modal.BriefDescription;
                    member.Reference = Modal.Reference;
                    member.FullDescription = Modal.FullDescription;
                    member.CorrectiveAction = Modal.CorrectiveAction;
                    member.PreventativeAction = Modal.PreventativeAction;
                    member.Rank = Modal.Rank;
                    member.Name = Modal.Name;
                    member.TimeScale = Modal.TimeScale;
                    member.CreatedDate = Utility.ToDateTimeUtcNow();
                    member.UpdatedDate = Utility.ToDateTimeUtcNow();
                    member.Ship = Modal.Ship;
                    member.IsResolved = Modal.isResolved;

                    //RDBJ 11/25/2021 Wrapped in If
                    if (Convert.ToBoolean(Modal.isResolved))
                        member.DateClosed = Utility.ToDateTimeUtcNow();
                    else
                        member.DateClosed = Modal.DateClosed;

                    member.Priority = Modal.Priority;
                    member.isDelete = 0;// RDBJ 11/22/2021

                    dbContext.AuditNotes.Add(member);
                    dbContext.SaveChanges();
                }

                Save_IAFAuditNote_Attachment(Modal.AuditNotesAttachment, Modal.NotesUniqueID);

                Entity.InternalAuditForm IAFForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == Modal.UniqueFormID).FirstOrDefault();
                IAFForm.FormVersion = IAFForm.FormVersion + Convert.ToDecimal(0.01);
                IAFForm.IsSynced = false;
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddIAFAuditNote " + ex.Message + "\n" + ex.InnerException);
            }
        }
        // End RDBJ 11/17/2021

        //RDBJ 11/17/2021
        public void Save_IAFAuditNote_Attachment(List<Modals.AuditNotesAttachment> Modal, Guid? NotesUniqueID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                if (Modal != null && Modal.Count > 0)
                {
                    var AuditNoteAttachment = dbContext.AuditNotesAttachments.Where(x => x.NotesUniqueID == NotesUniqueID);
                    foreach (var itemANFile in AuditNoteAttachment)
                    {
                        dbContext.AuditNotesAttachments.Remove(itemANFile);
                    }
                    dbContext.SaveChanges();

                    foreach (var item in Modal)
                    {
                        // RDBJ 01/27/2022 wrapped in if
                        if (item.IsActive)
                        {
                            Entity.AuditNotesAttachment file = new Entity.AuditNotesAttachment();
                            file.NotesFileUniqueID = Guid.NewGuid();
                            file.NotesUniqueID = NotesUniqueID;
                            file.InternalAuditFormId = 0;
                            file.AuditNotesId = 0;
                            file.FileName = item.FileName;
                            file.StorePath = item.StorePath;

                            dbContext.AuditNotesAttachments.Add(file);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Save_IAFAuditNote_Attachment " + ex.Message + "\n" + ex.InnerException);
            }
        }
        //End RDBJ 11/17/2021

        //RDBJ 11/24/2021
        public bool UpdateAdditionalAndCloseStatus(string id, bool IsAdditionalAndClosedStatus, bool IsAdditionalAndClosed)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid IAFUniqueID = Guid.Parse(id);
            try
            {
                var data = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == IAFUniqueID).FirstOrDefault();
                if (data != null)
                {
                    if (IsAdditionalAndClosed)
                        data.IsAdditional = IsAdditionalAndClosedStatus;
                    else
                    {
                        data.IsClosed = IsAdditionalAndClosedStatus;
                        // JSL 10/15/2022
                        if (data.IsClosed)
                        {
                            var lstIAFDeficiencies = dbContext.AuditNotes.Where(x => x.UniqueFormID == IAFUniqueID && x.IsResolved == false && x.isDelete == 0).ToList();
                            lstIAFDeficiencies.ForEach(x => { 
                                x.IsResolved = true; 
                                x.DateClosed = Utility.ToDateTimeUtcNow(); 
                                x.UpdatedDate = Utility.ToDateTimeUtcNow(); 
                            });
                            
                            LogHelper.LogForDeficienciesClose(AppStatic.IAFForm, String.Join(",", lstIAFDeficiencies.Select(y => y.NotesUniqueID).ToList()), _Username, Convert.ToString(data.IsClosed));
                        }
                        // End JSL 10/15/2022
                    }

                    data.FormVersion = data.FormVersion + Convert.ToDecimal(0.01);
                    data.IsSynced = false;
                    data.UpdatedDate = Utility.ToDateTimeUtcNow();
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateAdditionalAndCloseStatus : " + ex.Message);
                return false;
            }
            return true;
        }
        //End RDBJ 11/24/2021

        //RDBJ 11/24/2021
        public bool RemoveAuditsOrAuditNotes(string id
            , bool IsAudit //RDBJ 11/25/2021
            )
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid IAFUniqueID = Guid.Parse(id);
            Entity.InternalAuditForm IAFForm = new Entity.InternalAuditForm();

            try
            {
                //RDBJ 11/25/2021 Wrapped in if and set logic to delete Audits either AuditNotes
                if (IsAudit)
                {
                    IAFForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == IAFUniqueID).FirstOrDefault();
                    IAFForm.isDelete = 1;
                }
                else
                {
                    Entity.AuditNote IAFAuditNoteForm = new Entity.AuditNote();
                    IAFAuditNoteForm = dbContext.AuditNotes.Where(x => x.NotesUniqueID == IAFUniqueID).FirstOrDefault();
                    IAFAuditNoteForm.isDelete = 1;
                    IAFAuditNoteForm.UpdatedDate = Utility.ToDateTimeUtcNow();

                    IAFForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == IAFAuditNoteForm.UniqueFormID).FirstOrDefault();
                }

                if (IAFForm != null)
                {
                    IAFForm.FormVersion = IAFForm.FormVersion + Convert.ToDecimal(0.01);
                    IAFForm.IsSynced = false;
                    IAFForm.UpdatedDate = Utility.ToDateTimeUtcNow();
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RemoveAuditsOrAuditNotes : " + ex.Message);
                return false;
            }
            return true;
        }
        //End RDBJ 11/24/2021

        //RDBJ 11/29/2021
        public List<BLL.Modals.SMSReferencesTree> GetSMSReferenceData()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();  // JSL 05/20/2022 uncommented 
            List<BLL.Modals.SMSReferencesTree> lstSMSReference = new List<BLL.Modals.SMSReferencesTree>();
            try
            {
                // JSL 05/20/2022 commented
                //RDBJ 11/30/2021
                /*
                string connetionString = Convert.ToString(ConfigurationManager.AppSettings["CarisbrookeLtdConnectionString"]);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM [ISM].[SMSReferencesTree]";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            lstSMSReference = dt.ToListof<BLL.Modals.SMSReferencesTree>();
                        }
                    }
                }
                */
                //End RDBJ 11/30/2021
                // End JSL 05/20/2022 commented

                // JSL 05/20/2022 uncommented 
                //RDBJ 11/30/2021 commented below section
                List<Entity.SMSReferencesTree> lstDBSMSReference = dbContext.SMSReferencesTrees
                    .Where(x => x.IsDeleted == false)   // JSL 05/20/2022
                    .ToList();

                if (lstDBSMSReference != null && lstDBSMSReference.Count > 0)
                {
                    lstSMSReference = lstDBSMSReference.Select(x => new Modals.SMSReferencesTree()
                    {
                        SMSReferenceId = x.SMSReferenceId,
                        SMSReferenceParentId = x.SMSReferenceParentId,
                        Number = x.Number,
                        Reference = x.Reference,
                    }).ToList();
                }
                // End JSL 05/20/2022 uncommented 
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMSReferenceData Exception : " + ex.Message);
                LogHelper.writelog("GetSMSReferenceData Inner Exception : " + ex.InnerException);
            }
            return lstSMSReference;
        }
        //End RDBJ 11/29/2021

        //RDBJ 11/29/2021
        public List<BLL.Modals.SSPReferenceTree> GetSSPReferenceData()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();   // JSL 05/20/2022 uncommented  
            List<BLL.Modals.SSPReferenceTree> lstSSPReference = new List<BLL.Modals.SSPReferenceTree>();
            try
            {
                // JSL 05/20/2022 commented
                //RDBJ 11/30/2021
                /*
                string connetionString = Convert.ToString(ConfigurationManager.AppSettings["CarisbrookeLtdConnectionString"]);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM [ISM].[SSPReferenceTree]";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            lstSSPReference = dt.ToListof<BLL.Modals.SSPReferenceTree>();
                        }
                    }
                }
                */
                //RDBJ 11/30/2021
                // End JSL 05/20/2022 commented

                // JSL 05/20/2022 uncommented 
                //RDBJ 11/30/2021 commented below section
                List<Entity.SSPReferenceTree> lstDBSSPReference = dbContext.SSPReferenceTrees
                    .Where(x => x.IsDeleted == false)   // JSL 05/20/2022
                    .ToList();
                if (lstDBSSPReference != null && lstDBSSPReference.Count > 0)
                {
                    lstSSPReference = lstDBSSPReference.Select(x => new Modals.SSPReferenceTree()
                    {
                        SSPReferenceId = x.SSPReferenceId,
                        SSPReferenceParentId = x.SSPReferenceParentId,
                        Number = x.Number,
                        Reference = x.Reference,
                    }).ToList();
                }
                // End JSL 05/20/2022 uncommented 
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSSPReferenceData Exception : " + ex.Message);
                LogHelper.writelog("GetSSPReferenceData Inner Exception : " + ex.InnerException);
            }
            return lstSSPReference;
        }
        //End RDBJ 11/29/2021

        //RDBJ 11/29/2021
        public List<BLL.Modals.MLCRegulationTree> GetMLCRegulationTree()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();    // JSL 05/20/2022 uncommented 
            List<BLL.Modals.MLCRegulationTree> lstMLCReference = new List<BLL.Modals.MLCRegulationTree>();
            try
            {
                // JSL 05/20/2022 commented
                /*
                //RDBJ 11/30/2021
                string connetionString = Convert.ToString(ConfigurationManager.AppSettings["CarisbrookeLtdConnectionString"]);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM [ISM].[MLCRegulationTree]";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            lstMLCReference = dt.ToListof<BLL.Modals.MLCRegulationTree>();
                        }
                    }
                }
                */
                //RDBJ 11/30/2021
                // End JSL 05/20/2022 commented

                // JSL 05/20/2022 uncommented 
                //RDBJ 11/30/2021 commented below section
                List<Entity.MLCRegulationTree> lstDBMLCReference = dbContext.MLCRegulationTrees
                    .Where(x => x.IsDeleted == false)   // JSL 05/20/2022
                    .ToList();
                if (lstDBMLCReference != null && lstDBMLCReference.Count > 0)
                {
                    lstMLCReference = lstDBMLCReference.Select(x => new Modals.MLCRegulationTree()
                    {
                        MLCRegulationId = x.MLCRegulationId,
                        MLCRegulationParentId = x.MLCRegulationParentId,
                        Number = x.Number,
                        Regulation = x.Regulation,
                    }).ToList();
                }
                // End JSL 05/20/2022 uncommented 
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetMLCRegulationTree Exception : " + ex.Message);
                LogHelper.writelog("GetMLCRegulationTree Inner Exception : " + ex.InnerException);
            }
            return lstMLCReference;
        }
        //End RDBJ 11/29/2021

        // RDBJ 01/23/2022
        public List<AuditList> GetIARDrafts(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<AuditList> list = new List<AuditList>();
            try
            {
                var dbList = dbContext.InternalAuditForms.Where(x => x.ShipName == shipCode && x.SavedAsDraft == true
                && x.isDelete == 0
                ).ToList();
                list = dbList.OrderByDescending(x => x.CreatedDate).Select(x => new AuditList()
                {
                    InternalAuditFormId = x.InternalAuditFormId,
                    Auditor = x.Auditor,
                    Location = x.Location,
                    Date = Utility.ToDateTimeStr(x.Date),
                    Type = x.AuditType == 1 ? "Internal" : "External",
                    Ship = x.ShipName,
                    ShipName = dbContext.CSShips.Where(y => y.Code == x.ShipName).Select(y => y.Name).FirstOrDefault(),
                    UpdatedDate = x.UpdatedDate,
                    UniqueFormID = (Guid)x.UniqueFormID,
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIARDrafts " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        // End RDBJ 01/23/2022

        // RDBJ2 02/23/2022
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
                case AppStatic.API_UPDATEAUDITNOTEDETAILS:
                    {
                        try
                        {
                            Guid NotesUniqueID = Guid.Empty;
                            string strCorrectiveActions = string.Empty;
                            string strPreventativeActions = string.Empty;
                            string strRank = string.Empty;
                            string strName = string.Empty;
                            string strTimeScale = string.Empty;
                            //string strPriority = string.Empty;

                            if (dictMetaData.ContainsKey("NotesUniqueID"))
                                NotesUniqueID = Guid.Parse(dictMetaData["NotesUniqueID"].ToString());

                            if (dictMetaData.ContainsKey("CorrectiveActions"))
                                strCorrectiveActions = dictMetaData["CorrectiveActions"].ToString();

                            if (dictMetaData.ContainsKey("PreventativeActions"))
                                strPreventativeActions = dictMetaData["PreventativeActions"].ToString();

                            if (dictMetaData.ContainsKey("Rank"))
                                strRank = dictMetaData["Rank"].ToString();

                            if (dictMetaData.ContainsKey("Name"))
                                strName = dictMetaData["Name"].ToString();

                            if (dictMetaData.ContainsKey("TimeScale"))
                                strTimeScale = dictMetaData["TimeScale"].ToString();

                            Entity.AuditNote auditNote = dbContext.AuditNotes
                                .Where(x => x.NotesUniqueID == NotesUniqueID).FirstOrDefault();

                            if (auditNote != null)
                            {
                                auditNote.CorrectiveAction = strCorrectiveActions;
                                auditNote.PreventativeAction = strPreventativeActions;
                                auditNote.Rank = strRank;
                                auditNote.Name = strName;

                                // JSL 05/10/2022 wrapped in if
                                if (!string.IsNullOrEmpty(strTimeScale))
                                    auditNote.TimeScale = DateTime.ParseExact(strTimeScale, "dd/MM/yyyy", null); // Convert.ToDateTime(strTimeScale);
                                
                                auditNote.UpdatedDate = Utility.ToDateTimeUtcNow();

                                dbContext.SaveChanges();
                                UpdateSyncedFormVersion(Convert.ToString(auditNote.UniqueFormID));
                            }

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPDATEAUDITNOTEDETAILS + " Error : " + ex.Message);
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
        // End RDBJ2 02/23/2022

        // RDBJ2 02/23/2022
        public static void UpdateSyncedFormVersion(string UniqueFormID)
        {
            Guid GuidUFID = Guid.Parse(UniqueFormID);

            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Entity.InternalAuditForm IAFData = new Entity.InternalAuditForm();
            IAFData = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == GuidUFID).FirstOrDefault();
            IAFData.FormVersion = IAFData.FormVersion + Convert.ToDecimal(0.01);
            IAFData.IsSynced = false;
            dbContext.SaveChanges();
        }
        // End RDBJ2 02/23/2022
    }
}
