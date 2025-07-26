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
    public class IAFHelper
    {
        public bool SaveIAFDataInLocalDB(IAF Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.InternalAuditForm);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.InternalAuditForm); }
                if (isTbaleCreated)
                {
                    Modal.InternalAuditForm.IsSynced = false; // RDBJ 02/05/2022
                    Modal.InternalAuditForm.SavedAsDraft = false; // RDBJ 02/05/2022

                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(connetionString);
                        // RDBJ 02/05/2022 wrapped in if
                        if (Modal.InternalAuditForm.UniqueFormID == null || Modal.InternalAuditForm.UniqueFormID == Guid.Empty)
                        {
                            string InsertQury = IAFDataInsertQuery();

                            SqlCommand command = new SqlCommand(InsertQury, connection);

                            Guid FormGUID = Guid.NewGuid();
                            Modal.InternalAuditForm.UniqueFormID = FormGUID;
                            Modal.InternalAuditForm.CreatedDate = Utility.ToDateTimeUtcNow(); // RDBJ 02/05/2022
                            Modal.InternalAuditForm.UpdatedDate = Utility.ToDateTimeUtcNow(); // RDBJ 02/05/2022
                            Modal.InternalAuditForm.AuditType = 1; // RDBJ 02/05/2022
                            Modal.InternalAuditForm.isDelete = 0; // RDBJ 02/05/2022
                            IAFDataInsertCMD(Modal.InternalAuditForm, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();
                        }
                        // RDBJ 02/05/2022 added else
                        else
                        {
                            string UpdateQury = GETIAFUpdateQuery();
                            SqlCommand command = new SqlCommand(UpdateQury, connection);

                            Modal.InternalAuditForm.UpdatedDate = Utility.ToDateTimeUtcNow();
                            IAFUpdateCMD(Modal, ref command);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }

                        // RDBJ 02/05/2022 wrapped in if
                        if (Modal.AuditNote != null && Modal.AuditNote.Count > 0)
                        {
                            IAFAuditNotesDataInsert(Modal.AuditNote, Modal.InternalAuditForm.ShipName, Modal.InternalAuditForm.UniqueFormID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveIAFDataInLocalDB : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        // RDBJ 02/04/2022
        public string IAFAutoSave(IAF Modal)
        {
            string retStrUniqueFormID = string.Empty;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.InternalAuditForm);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.InternalAuditForm); }
                if (isTbaleCreated)
                {
                    Modal.InternalAuditForm.IsSynced = false; // RDBJ 02/04/2022
                    
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(connetionString);

                        if (Modal.InternalAuditForm.UniqueFormID == null || Modal.InternalAuditForm.UniqueFormID == Guid.Empty)
                        { 
                            string InsertQury = IAFDataInsertQuery();
                            
                            SqlCommand command = new SqlCommand(InsertQury, connection);

                            Guid FormGUID = Guid.NewGuid();
                            Modal.InternalAuditForm.UniqueFormID = FormGUID;
                            Modal.InternalAuditForm.CreatedDate = Utility.ToDateTimeUtcNow();
                            Modal.InternalAuditForm.UpdatedDate = Utility.ToDateTimeUtcNow();
                            Modal.InternalAuditForm.AuditType = 1;
                            Modal.InternalAuditForm.isDelete = 0;
                            IAFDataInsertCMD(Modal.InternalAuditForm, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();
                        }
                        else
                        {
                            string UpdateQury = GETIAFUpdateQuery();
                            SqlCommand command = new SqlCommand(UpdateQury, connection);

                            Modal.InternalAuditForm.UpdatedDate = Utility.ToDateTimeUtcNow();
                            IAFUpdateCMD(Modal, ref command);

                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }

                        // RDBJ 02/05/2022 wrapped in if
                        if (Modal.AuditNote != null && Modal.AuditNote.Count > 0)
                        {
                            IAFAuditNotesDataInsert(Modal.AuditNote, Modal.InternalAuditForm.ShipName, Modal.InternalAuditForm.UniqueFormID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAutoSave : " + ex.Message.ToString());
                retStrUniqueFormID = string.Empty;
            }
            return retStrUniqueFormID = Convert.ToString(Modal.InternalAuditForm.UniqueFormID);
        }
        // End RDBJ 02/04/2022

        public bool IAFAuditNotesDataInsert(List<AuditNote> AuditNotes, string Ship, Guid? UniqueFormID)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AuditNotes);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AuditNotes); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(connetionString);
                        foreach (var AuditNoteItem in AuditNotes)
                        {
                            string auditNotesUniqueID = IAFNoteCommentResolutionRecordsExist(AppStatic.AuditNotes, "NotesUniqueID", AuditNoteItem.NotesUniqueID); // RDBJ 02/04/2022
                            // RDBJ 02/04/2022 wrapped in if and added if
                            if (!string.IsNullOrEmpty(auditNotesUniqueID))
                            {
                                string UpdateQuery = IAFNotes_UpdateQuery();
                                SqlCommand command = new SqlCommand(UpdateQuery, connection);
                                AuditNoteItem.UpdatedDate = Utility.ToDateTimeUtcNow();
                                IAFNotes_UpdateCMD(AuditNoteItem, ref command);
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                            else
                            {
                                //AuditNoteItem.NotesUniqueID = Guid.NewGuid();
                                AuditNoteItem.InternalAuditFormId = 0;
                                AuditNoteItem.UniqueFormID = UniqueFormID;

                                AuditNoteItem.Priority = 12;
                                AuditNoteItem.isDelete = 0;
                                AuditNoteItem.UpdatedDate = Utility.ToDateTimeUtcNow(); 
                                AuditNoteItem.CreatedDate = Utility.ToDateTimeUtcNow();
                                string InsertQury = IAFAuditNotesDataInsertQuery();
                                SqlCommand command = new SqlCommand(InsertQury, connection);
                                IAFAuditNotesDataInsertCMD(AuditNoteItem, Ship, ref command);
                                connection.Open();
                                command.ExecuteScalar();
                                connection.Close();
                            }

                            if (AuditNoteItem.AuditNotesAttachment != null && AuditNoteItem.AuditNotesAttachment.Count > 0)
                            {
                                IAFAuditNotesAttachmentsDataInsert(AuditNoteItem.AuditNotesAttachment, AuditNoteItem.NotesUniqueID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAuditNotesDataInsert : " + ex.Message.ToString());
            }
            return res;
        }
        public bool IAFAuditNotesAttachmentsDataInsert(List<AuditNotesAttachment> AttachList, Guid? NotesUniqueID)
        {
            bool res = DeleteRecords(AppStatic.AuditNotesAttachment, "NotesUniqueID", Convert.ToString(NotesUniqueID)); // RDBJ 02/04/2022

            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AuditNotesAttachment);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AuditNotesAttachment); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(connetionString);
                        
                        foreach (var item in AttachList)
                        {
                            // RDBJ 02/04/2022 wrapped in if
                            if (item.IsActive)
                            {
                                string InsertQury = IAFAuditNotesAttachmentsDataInsertQuery();
                                SqlCommand command = new SqlCommand(InsertQury, connection);

                                item.NotesFileUniqueID = Guid.NewGuid();
                                item.NotesUniqueID = NotesUniqueID;
                                item.InternalAuditFormId = 0;
                                item.AuditNotesId = 0;

                                IAFAuditNotesAttachmentsDataInsertCMD(item, ref command);
                                connection.Open(); // RDBJ 02/04/2022
                                command.ExecuteScalar();
                                connection.Close(); // RDBJ 02/04/2022
                            }
                        }
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAuditNotesAttachmentsDataInsert : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string IAFDataInsertQuery()
        {
            // RDBJ 02/04/2022 added SavedAsDraft //RDBJ 11/25/2021 added isDelete,AuditType,IsClosed,IsAdditional
            string InsertQury = @"INSERT INTO dbo.InternalAuditForm (ShipId,ShipName,Location,AuditNo,AuditTypeISM,
                                AuditTypeISPS,AuditTypeMLC,Date,Auditor,CreatedDate,UpdatedDate,IsSynced,FormVersion,UniqueFormID,
                                isDelete,AuditType,IsClosed,IsAdditional,SavedAsDraft)
                                OUTPUT INSERTED.InternalAuditFormId
                                VALUES (@ShipId,@ShipName,@Location,@AuditNo,@AuditTypeISM,
                                @AuditTypeISPS,@AuditTypeMLC,@Date,@Auditor,@CreatedDate,@UpdatedDate,@IsSynced,@FormVersion,@UniqueFormID,
                                @isDelete,@AuditType,@IsClosed,@IsAdditional,@SavedAsDraft)";
            return InsertQury;
        }
        public void IAFDataInsertCMD(InternalAuditForm Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ShipId", SqlDbType.BigInt).Value = Modal.ShipId ?? (object)DBNull.Value;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.BigInt).Value = Modal.FormVersion;
            command.Parameters.Add("@ShipName", SqlDbType.VarChar).Value = Modal.ShipName;
            command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Modal.Location;
            command.Parameters.Add("@AuditNo", SqlDbType.VarChar).Value = Modal.AuditNo;
            command.Parameters.Add("@AuditTypeISM", SqlDbType.Bit).Value = Modal.AuditTypeISM == null ? false : true;
            command.Parameters.Add("@AuditTypeISPS", SqlDbType.Bit).Value = Modal.AuditTypeISPS == null ? false : true;
            command.Parameters.Add("@AuditTypeMLC", SqlDbType.Bit).Value = Modal.AuditTypeMLC == null ? false : true;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date;
            command.Parameters.Add("@Auditor", SqlDbType.VarChar).Value = Modal.Auditor;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = false;
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete; //RDBJ 11/25/2021
            command.Parameters.Add("@AuditType", SqlDbType.Int).Value = Modal.AuditType; //RDBJ 11/25/2021
            command.Parameters.Add("@IsClosed", SqlDbType.Bit).Value = Modal.IsClosed; //RDBJ 11/25/2021
            command.Parameters.Add("@IsAdditional", SqlDbType.Bit).Value = Modal.IsAdditional; //RDBJ 11/25/2021
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft;   // RDBJ 02/04/2022
        }

        // RDBJ 02/04/2022
        public string GETIAFUpdateQuery()
        {
            string query = @"UPDATE dbo.InternalAuditForm SET ShipId = @ShipId, ShipName = @ShipName, Location = @Location, 
                            AuditNo = @AuditNo, AuditTypeISM = @AuditTypeISM, AuditTypeISPS = @AuditTypeISPS, AuditTypeMLC = @AuditTypeMLC, 
                            Date = @Date, Auditor = @Auditor, UpdatedDate = @UpdatedDate, IsSynced = @IsSynced, 
                            FormVersion = @FormVersion, AuditType = @AuditType, IsClosed = @IsClosed, IsAdditional = @IsAdditional,
                            SavedAsDraft = @SavedAsDraft
                            WHERE UniqueFormID = @UniqueFormID";
            return query;
        }
        // End RDBJ 02/04/2022

        // RDBJ 02/04/2022
        public void IAFUpdateCMD(IAF Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.InternalAuditForm.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.InternalAuditForm.FormVersion;
            command.Parameters.Add("@ShipId", SqlDbType.Int).Value = Modal.InternalAuditForm.ShipId == null ? DBNull.Value : (object)Modal.InternalAuditForm.ShipId;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.ShipName == null ? string.Empty : Modal.InternalAuditForm.ShipName;
            command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.Location == null ? string.Empty : Modal.InternalAuditForm.Location;
            command.Parameters.Add("@AuditNo", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.AuditNo == null ? string.Empty : Modal.InternalAuditForm.AuditNo;
            command.Parameters.Add("@AuditTypeISM", SqlDbType.Bit).Value = Modal.InternalAuditForm.AuditTypeISM == null ? DBNull.Value : (object)Modal.InternalAuditForm.AuditTypeISM;
            command.Parameters.Add("@AuditTypeISPS", SqlDbType.Bit).Value = Modal.InternalAuditForm.AuditTypeISPS == null ? DBNull.Value : (object)Modal.InternalAuditForm.AuditTypeISPS;
            command.Parameters.Add("@AuditTypeMLC", SqlDbType.Bit).Value = Modal.InternalAuditForm.AuditTypeMLC == null ? DBNull.Value : (object)Modal.InternalAuditForm.AuditTypeMLC;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.InternalAuditForm.Date;
            command.Parameters.Add("@Auditor", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.Auditor != null ? Modal.InternalAuditForm.Auditor : "";
            command.Parameters.Add("@IsSynced", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.IsSynced == null ? DBNull.Value : (object)Modal.InternalAuditForm.IsSynced;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.InternalAuditForm.UpdatedDate == null ? Utility.ToDateTimeUtcNow() : (object)Modal.InternalAuditForm.UpdatedDate;
            command.Parameters.Add("@AuditType", SqlDbType.Int).Value = Modal.InternalAuditForm.AuditType;
            command.Parameters.Add("@IsClosed", SqlDbType.Bit).Value = Modal.InternalAuditForm.IsClosed;
            command.Parameters.Add("@IsAdditional", SqlDbType.Bit).Value = Modal.InternalAuditForm.IsAdditional;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.InternalAuditForm.SavedAsDraft;
        }
        // End RDBJ 02/04/2022

        public string IAFAuditNotesDataInsertQuery()
        {
            // RDBJ 02/04/2022 added AssignTo //RDBJ 11/25/2021 added Priority and isDelete
            string InsertQury = @"INSERT INTO dbo.AuditNotes (InternalAuditFormId,Number,Type,BriefDescription,Reference,
                                FullDescription,CorrectiveAction,PreventativeAction,Rank,Name,Ship,TimeScale,CreatedDate,
                                UpdatedDate,IsResolved,UniqueFormId,NotesUniqueID,Priority,isDelete,AssignTo)
                                OUTPUT INSERTED.AuditNotesId
                                VALUES (@InternalAuditFormId,@Number,@Type,@BriefDescription,@Reference,
                                @FullDescription,@CorrectiveAction,@PreventativeAction,@Rank,@Name,@Ship,
                                @TimeScale,@CreatedDate,@UpdatedDate,@IsResolved,@UniqueFormId,@NotesUniqueID,@Priority,@isDelete,@AssignTo)";
            return InsertQury;
        }
        public void IAFAuditNotesDataInsertCMD(AuditNote Modal, string Ship, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormId", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@InternalAuditFormId", SqlDbType.BigInt).Value = Modal.InternalAuditFormId;
            command.Parameters.Add("@Number", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.Number) ? string.Empty : Modal.Number;
            command.Parameters.Add("@Type", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.Type) ? string.Empty : Modal.Type;
            command.Parameters.Add("@BriefDescription", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.BriefDescription) ? string.Empty : Modal.BriefDescription;
            command.Parameters.Add("@Reference", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.Reference) ? string.Empty : Modal.Reference;
            command.Parameters.Add("@FullDescription", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.FullDescription) ? string.Empty : Modal.FullDescription;
            command.Parameters.Add("@CorrectiveAction", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.CorrectiveAction) ? string.Empty : Modal.CorrectiveAction;
            command.Parameters.Add("@PreventativeAction", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.PreventativeAction) ? string.Empty : Modal.PreventativeAction;
            command.Parameters.Add("@Rank", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.Rank) ? string.Empty : Modal.Rank;
            command.Parameters.Add("@Name", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Modal.Name) ? string.Empty : Modal.Name;
            command.Parameters.Add("@Ship", SqlDbType.VarChar).Value = string.IsNullOrEmpty(Ship) ? string.Empty : Ship;
            command.Parameters.Add("@TimeScale", SqlDbType.DateTime).Value = Modal.TimeScale == null ? (object)DBNull.Value : Modal.TimeScale;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
            command.Parameters.Add("@IsResolved", SqlDbType.Bit).Value = false;
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority; //RDBJ 11/25/2021
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete; //RDBJ 11/25/2021
            command.Parameters.Add("@AssignTo", SqlDbType.UniqueIdentifier).Value = Modal.AssignTo == null ? DBNull.Value : (object)Modal.AssignTo;    // RDBJ 02/04/2022
        }

        // RDBJ 02/04/2022
        public string IAFNotes_UpdateQuery(
            bool IsUpdateFromIADetailPage = false   // RDBJ2 03/31/2022
            )
        {
            string UpdateQuery = string.Empty;  // RDBJ2 03/31/2022

            // RDBJ2 03/31/2022 wrapped in if and added if part
            if (IsUpdateFromIADetailPage)
            {
                UpdateQuery = @"UPDATE dbo.AuditNotes SET
                                CorrectiveAction=@CorrectiveAction,PreventativeAction=@PreventativeAction,
                                Rank=@Rank,Name=@Name,TimeScale=@TimeScale,
                                UpdatedDate=@UpdatedDate
                                WHERE NotesUniqueID = @NotesUniqueID";
            }
            else
            {
                UpdateQuery = @"UPDATE dbo.AuditNotes SET
                                Number = @Number, Type = @Type, BriefDescription = @BriefDescription, Reference = @Reference,
                                FullDescription = @FullDescription,CorrectiveAction=@CorrectiveAction,PreventativeAction=@PreventativeAction,
                                Rank=@Rank,Name=@Name,TimeScale=@TimeScale,
                                UpdatedDate=@UpdatedDate
                                WHERE NotesUniqueID = @NotesUniqueID";
            }
            return UpdateQuery;
        }
        public void IAFNotes_UpdateCMD(AuditNote Modal, ref SqlCommand command
            , bool IsUpdateFromIADetailPage = false   // RDBJ2 03/31/2022
            )
        {
            if (!IsUpdateFromIADetailPage)
            {
                command.Parameters.Add("@Number", SqlDbType.VarChar).Value = Modal.Number;
                command.Parameters.Add("@Type", SqlDbType.VarChar).Value = Modal.Type;
                command.Parameters.Add("@BriefDescription", SqlDbType.VarChar).Value = Modal.BriefDescription == null ? DBNull.Value : (object)Modal.BriefDescription;
                command.Parameters.Add("@Reference", SqlDbType.VarChar).Value = Modal.Reference == null ? DBNull.Value : (object)Modal.Reference;
                command.Parameters.Add("@FullDescription", SqlDbType.VarChar).Value = Modal.FullDescription == null ? DBNull.Value : (object)Modal.FullDescription;
            }

            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@CorrectiveAction", SqlDbType.VarChar).Value = Modal.CorrectiveAction == null ? DBNull.Value : (object)Modal.CorrectiveAction;
            command.Parameters.Add("@PreventativeAction", SqlDbType.VarChar).Value = Modal.PreventativeAction == null ? DBNull.Value : (object)Modal.PreventativeAction;
            command.Parameters.Add("@Rank", SqlDbType.VarChar).Value = Modal.Rank == null ? DBNull.Value : (object)Modal.Rank;
            command.Parameters.Add("@Name", SqlDbType.VarChar).Value = Modal.Name == null ? DBNull.Value : (object)Modal.Name;
            command.Parameters.Add("@TimeScale", SqlDbType.DateTime).Value = Modal.TimeScale == null ? DBNull.Value : (object)Modal.TimeScale;

            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
        }
        // End RDBJ 02/04/2022

        public string IAFAuditNotesAttachmentsDataInsertQuery()
        {
            //RDBJ 10/04/2021 Added NotesFileUniqueID,NotesUniqueID
            string InsertQury = @"INSERT INTO dbo.AuditNotesAttachment (NotesFileUniqueID,NotesUniqueID,InternalAuditFormId,AuditNotesId,FileName,StorePath)
                                OUTPUT INSERTED.AuditNotesAttachmentId
                                VALUES (@NotesFileUniqueID,@NotesUniqueID,@InternalAuditFormId,@AuditNotesId,@FileName,@StorePath)";
            return InsertQury;
        }
        public void IAFAuditNotesAttachmentsDataInsertCMD(AuditNotesAttachment Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@NotesFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesFileUniqueID; //RDBJ 10/04/2021
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID; //RDBJ 10/04/2021
            command.Parameters.Add("@InternalAuditFormId", SqlDbType.BigInt).Value = Modal.InternalAuditFormId;
            command.Parameters.Add("@AuditNotesId", SqlDbType.BigInt).Value = Modal.AuditNotesId;
            command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.VarChar).Value = Modal.StorePath;
        }

        public Dictionary<string, string> GetNumberForNotes(string ship
            , string UniqueFormID)  // RDBJ 02/02/2022
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        SqlDataAdapter sqlAdp;
                        DataTable dt;
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            int intNumberStartFrom = 0; // RDBJ 02/02/2022
                            dt = new DataTable();

                            // RDBJ2 03/14/2022
                            string number = "SELECT count(*) as number FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + UniqueFormID + "' AND Ship = '" + ship + "' AND [isDelete] = 0";
                            sqlAdp = new SqlDataAdapter(number, conn);
                            sqlAdp.Fill(dt);
                            //intNumberStartFrom = 500 + Convert.ToInt32(dt.Rows[0][0]);  // JSL 05/27/2022 commented this line
                            intNumberStartFrom = Convert.ToInt32(dt.Rows[0][0]);  // JSL 05/27/2022
                            data.Add("ISM-Non Conformity", Convert.ToString(intNumberStartFrom));
                            data.Add("ISPS-Non Conformity", Convert.ToString(intNumberStartFrom));
                            data.Add("ISM-Observation", Convert.ToString(intNumberStartFrom));
                            data.Add("ISPS-Observation", Convert.ToString(intNumberStartFrom));
                            data.Add("MLC-Deficiency", Convert.ToString(intNumberStartFrom));
                            // End RDBJ2 03/14/2022

                            // RDBJ2 03/14/2022 Commented this section due to avoid individual
                            /*
                            string number = "SELECT count(*) as number FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + UniqueFormID + "' AND Ship = '" + ship + "' AND Type = 'ISM-Non Conformity'"; // RDBJ 02/02/2022 Added UniqueFormID
                            sqlAdp = new SqlDataAdapter(number, conn);
                            sqlAdp.Fill(dt);
                            intNumberStartFrom = 500 + Convert.ToInt32(dt.Rows[0][0]);   // RDBJ 02/02/2022
                            data.Add("ISM-Non Conformity", Convert.ToString(intNumberStartFrom)); // RDBJ 02/02/2022 added variable intNumberStartFrom

                            dt = new DataTable();
                            number = "SELECT count(*) as number FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + UniqueFormID + "' AND Ship = '" + ship + "' AND Type = 'ISPS-Non Conformity'"; // RDBJ 02/02/2022 Added UniqueFormID
                            sqlAdp = new SqlDataAdapter(number, conn);
                            sqlAdp.Fill(dt);
                            intNumberStartFrom = 500 + Convert.ToInt32(dt.Rows[0][0]);   // RDBJ 02/02/2022
                            data.Add("ISPS-Non Conformity", Convert.ToString(intNumberStartFrom)); // RDBJ 02/02/2022 added variable intNumberStartFrom

                            dt = new DataTable();
                            number = "SELECT count(*) as number FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + UniqueFormID + "' AND Ship = '" + ship + "' AND Type = 'ISM-Observation'"; // RDBJ 02/02/2022 Added UniqueFormID
                            sqlAdp = new SqlDataAdapter(number, conn);
                            sqlAdp.Fill(dt);
                            intNumberStartFrom = 500 + Convert.ToInt32(dt.Rows[0][0]);   // RDBJ 02/02/2022
                            data.Add("ISM-Observation", Convert.ToString(intNumberStartFrom)); // RDBJ 02/02/2022 added variable intNumberStartFrom

                            dt = new DataTable();
                            number = "SELECT count(*) as number FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + UniqueFormID + "' AND Ship = '" + ship + "' AND Type = 'ISPS-Observation'"; // RDBJ 02/02/2022 Added UniqueFormID
                            sqlAdp = new SqlDataAdapter(number, conn);
                            sqlAdp.Fill(dt);
                            intNumberStartFrom = 500 + Convert.ToInt32(dt.Rows[0][0]);   // RDBJ 02/02/2022
                            data.Add("ISPS-Observation", Convert.ToString(intNumberStartFrom)); // RDBJ 02/02/2022 added variable intNumberStartFrom

                            dt = new DataTable();
                            number = "SELECT count(*) as number FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + UniqueFormID + "' AND Ship = '" + ship + "' AND Type = 'MLC-Deficiency'"; // RDBJ 02/02/2022 Added UniqueFormID
                            sqlAdp = new SqlDataAdapter(number, conn);
                            sqlAdp.Fill(dt);
                            intNumberStartFrom = 500 + Convert.ToInt32(dt.Rows[0][0]);   // RDBJ 02/02/2022
                            data.Add("MLC-Deficiency", Convert.ToString(intNumberStartFrom)); // RDBJ 02/02/2022 added variable intNumberStartFrom
                            */
                            // End RDBJ2 03/14/2022 Commented this section due to avoid individual
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNumberForNotes : " + ex.Message + " : " + ex.InnerException);
                return null;
            }
        }

        public IAF IAFormDetailsView_Local_DB(string IAFormID)
        {
            IAF data = new IAF();
            Guid UniqueFormID = Guid.Parse(IAFormID);
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.InternalAuditForm + " WHERE UniqueFormID = '" + UniqueFormID + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<InternalAuditForm> dbIAF = dt.ToListof<InternalAuditForm>();
                                data.InternalAuditForm = dbIAF[0];
                                if (dbIAF.Count > 0)
                                {
                                    DataTable dtAuditNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + UniqueFormID + "' AND [isDelete] = 0", conn); // RDBJ2 03/14/2022 set isDelete = 0
                                    sqlAdp.Fill(dtAuditNotes);
                                    if (dtAuditNotes != null && dtAuditNotes.Rows.Count > 0)
                                    {
                                        List<AuditNote> dbIAFNotes = dtAuditNotes.ToListof<AuditNote>();
                                        data.AuditNote = dbIAFNotes;

                                        for (int i = 0; i < data.AuditNote.Count; i++)
                                        {
                                            DataTable dtAuditNotesAttachment = new DataTable();
                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesAttachment + " WHERE NotesUniqueID = '" + data.AuditNote[i].NotesUniqueID + "'", conn);
                                            sqlAdp.Fill(dtAuditNotesAttachment);
                                            List<AuditNotesAttachment> resFile = dtAuditNotesAttachment.ToListof<AuditNotesAttachment>();
                                            data.AuditNote[i].AuditNotesAttachment = resFile;
                                        }
                                    }
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFormDetailsView_Local_DB : " + ex.Message);
            }
            return data;
        }
        public AuditNote GetAuditNotesById_Local_DB(Guid? NotesUniqueID)
        {
            Modals.AuditNote obj = new Modals.AuditNote();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dtAuditNotes = new DataTable();
                            //SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotes + " WHERE NotesUniqueID = '" + NotesUniqueID + "'", conn);   // JSL 06/13/2022 Commented this line
                            string strQuery = "SELECT ANote.*, Ship.Name AS ShipName FROM " + AppStatic.AuditNotes + " ANote LEFT JOIN " + AppStatic.CSShips + " Ship ON Ship.Code = ANote.Ship WHERE NotesUniqueID = '" + NotesUniqueID + "'"; // JSL 06/23/2022
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(strQuery, conn);   // JSL 06/13/2022
                            sqlAdp.Fill(dtAuditNotes);
                            if (dtAuditNotes != null && dtAuditNotes.Rows.Count > 0)
                            {
                                List<AuditNote> dbIAFNotes = dtAuditNotes.ToListof<AuditNote>();
                                obj = dbIAFNotes[0];
                                
                                DataTable dtAuditNotesAttachment = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesAttachment + " WHERE NotesUniqueID = '" + obj.NotesUniqueID + "'", conn);
                                sqlAdp.Fill(dtAuditNotesAttachment);
                                List<AuditNotesAttachment> resFile = dtAuditNotesAttachment.ToListof<AuditNotesAttachment>();
                                obj.AuditNotesAttachment = resFile;

                                // JSL 12/04/2022
                                if (obj.AuditNotesAttachment != null && obj.AuditNotesAttachment.Count > 0)
                                {
                                    foreach (var deffile in obj.AuditNotesAttachment)
                                    {
                                        if (deffile.StorePath.StartsWith("data:"))
                                        {
                                            Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                                            dicFileMetaData["UniqueFormID"] = Convert.ToString(obj.UniqueFormID);
                                            dicFileMetaData["ReportType"] = "IA";
                                            dicFileMetaData["DetailUniqueId"] = Convert.ToString(obj.NotesUniqueID);
                                            dicFileMetaData["FileName"] = deffile.FileName;
                                            dicFileMetaData["Base64FileData"] = deffile.StorePath;

                                            deffile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);

                                            if (!string.IsNullOrEmpty(deffile.StorePath))
                                            {
                                                Dictionary<string, string> dicFilePathUpdateMetaData = new Dictionary<string, string>();
                                                dicFilePathUpdateMetaData["TableName"] = AppStatic.AuditNotesAttachment;
                                                dicFilePathUpdateMetaData["ColumnName"] = "StorePath";
                                                dicFilePathUpdateMetaData["WhereColumnName"] = "NotesFileUniqueID";
                                                dicFilePathUpdateMetaData["WhereColumnDataID"] = Convert.ToString(deffile.NotesFileUniqueID);
                                                Utility.UpdateFilePathIn_Local_DB(dicFilePathUpdateMetaData, deffile.StorePath);
                                            }
                                        }
                                    }
                                }
                                // End JSL 12/04/2022
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNotesById " + ex.Message + "\n" + ex.InnerException);
            }
            return obj;
        }

        public bool AddAuditNoteResolutions_Local_DB(Audit_Note_Resolutions data)
        {
            APIDeficienciesHelper deficienciesHelper = new APIDeficienciesHelper();
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AuditNotesResolution);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AuditNotesResolution); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string UniqueFormID = string.Empty;
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID FROM " + AppStatic.AuditNotes + " WHERE NotesUniqueID = '" + data.NotesUniqueID + "'", connection);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            UniqueFormID = Convert.ToString(dt.Rows[0][0]);
                        }

                        string InsertQuery = AuditNoteResolution_InsertQuery();
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                        data.ResolutionUniqueID = Guid.NewGuid();
                        AuditNoteResolution_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();

                        if (data.AuditNoteResolutionsFiles != null && data.AuditNoteResolutionsFiles.Count > 0)
                            AddAuditNoteResolutionFiles_Local_DB(data.AuditNoteResolutionsFiles, data.ResolutionUniqueID);
                        
                        res = true;

                        deficienciesHelper.UpdateGIRSyncStatus_Local_DB(UniqueFormID, "IAF"); //RDBJ 10/04/2021
                        deficienciesHelper.UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB(Convert.ToString(data.NotesUniqueID), "IAF"); //RDBJ 11/10/2021
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddAuditNoteResolutions_Local_DB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool AddAuditNoteResolutionFiles_Local_DB(List<Audit_Note_Resolutions_Files> data, Guid? ResolutionUniqueID)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AuditNotesResolutionFiles);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AuditNotesResolutionFiles); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        foreach (var item in data)
                        {
                            string InsertQuery = AuditNoteResolutionFiles_InsertQuery();
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            connection.Open();
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            
                            item.ResolutionFileUniqueID = Guid.NewGuid();
                            item.ResolutionUniqueID = ResolutionUniqueID;

                            AuditNoteResolutionFiles_CMD(item, ref command);
                            command.ExecuteScalar();
                            connection.Close();
                        }
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddAuditNoteResolutionFiles_Local_DB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string AuditNoteResolution_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.AuditNotesResolution 
                                  (AuditNoteID,UserName,Resolution,CreatedDate,UpdatedDate,NotesUniqueID,ResolutionUniqueID,isNew)
                                  OUTPUT INSERTED.ResolutionID
                                  VALUES (@AuditNoteID,@UserName,@Resolution,@CreatedDate,@UpdatedDate,@NotesUniqueID,@ResolutionUniqueID,@isNew)";
            return InsertQuery;
        }
        public void AuditNoteResolution_CMD(Audit_Note_Resolutions Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@AuditNoteID", SqlDbType.BigInt).Value = Utility.ToLong(Modal.AuditNoteID);
            command.Parameters.Add("@ResolutionUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionUniqueID;
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = Modal.UserName == null ? DBNull.Value : (object)Modal.UserName;
            command.Parameters.Add("@Resolution", SqlDbType.NVarChar).Value = Modal.Resolution == null ? DBNull.Value : (object)Modal.Resolution;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 0; //RDBJ 10/26/2021
        }
        public string AuditNoteResolutionFiles_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.AuditNotesResolutionFiles 
                                  (ResolutionID,AuditNoteID,FileName,StorePath,ResolutionUniqueID,ResolutionFileUniqueID)
                                  OUTPUT INSERTED.ResolutionFileID
                                  VALUES (@ResolutionID,@AuditNoteID,@FileName,@StorePath,@ResolutionUniqueID,@ResolutionFileUniqueID)";
            return InsertQuery;
        }
        public void AuditNoteResolutionFiles_CMD(Audit_Note_Resolutions_Files Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ResolutionID", SqlDbType.BigInt).Value = Modal.ResolutionID;
            command.Parameters.Add("@ResolutionUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionUniqueID;
            command.Parameters.Add("@ResolutionFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionFileUniqueID;
            command.Parameters.Add("@AuditNoteID", SqlDbType.BigInt).Value = Modal.AuditNoteID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
        }

        public List<Audit_Note_Resolutions> GetAuditNoteResolutions_Local_DB(Guid? NotesUniqueID)
        {
            List<Audit_Note_Resolutions> list = new List<Audit_Note_Resolutions>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.AuditNotesResolution + " WHERE NotesUniqueID = '" + NotesUniqueID + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<Audit_Note_Resolutions> dbAuditNotesCommentsList = dt.ToListof<Audit_Note_Resolutions>();
                                foreach (var item in dbAuditNotesCommentsList)
                                {
                                    Audit_Note_Resolutions obj = new Audit_Note_Resolutions();
                                    obj.AuditNoteResolutionsFiles = new List<Audit_Note_Resolutions_Files>();

                                    DataTable dtAudit_Deficiency_Comments_Files = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesResolutionFiles + " WHERE ResolutionUniqueID = '" + item.ResolutionUniqueID + "'", conn);
                                    sqlAdp.Fill(dtAudit_Deficiency_Comments_Files);
                                    List<Audit_Note_Resolutions_Files> defCommentFiles = dtAudit_Deficiency_Comments_Files.ToListof<Audit_Note_Resolutions_Files>();

                                    foreach (var subitem in defCommentFiles)
                                    {
                                        Audit_Note_Resolutions_Files filedata = new Audit_Note_Resolutions_Files();
                                        //filedata.ResolutionFileID = subitem.ResolutionFileID;
                                        filedata.ResolutionFileUniqueID = subitem.ResolutionFileUniqueID;
                                        //filedata.ResolutionID = subitem.ResolutionID;
                                        //filedata.AuditNoteID = subitem.AuditNoteID;
                                        //filedata.ResolutionUniqueID = subitem.ResolutionUniqueID;
                                        filedata.FileName = subitem.FileName;
                                        //filedata.StorePath = subitem.StorePath;
                                        obj.AuditNoteResolutionsFiles.Add(filedata);
                                    }
                                    obj.ResolutionID = item.ResolutionID;
                                    obj.ResolutionUniqueID = item.ResolutionUniqueID;
                                    obj.NotesUniqueID = item.NotesUniqueID;
                                    obj.AuditNoteID = item.AuditNoteID;
                                    obj.UserName = item.UserName;
                                    obj.Resolution = item.Resolution;
                                    obj.CreatedDate = item.CreatedDate;
                                    obj.isNew = item.isNew; //RDBJ 10/26/2021
                                    list.Add(obj);
                                }
                                list = list.OrderByDescending(x => x.CreatedDate).ToList();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNoteResolutions_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public Dictionary<string, string> GetAuditNoteCommentFile_Local_DB(int CommentFileID)   // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();  // RDBJ 01/27/2022 set with dictionary
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        DataTable dtAuditNotesCommentFile = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT StorePath, FileName FROM " + AppStatic.AuditNotesCommentsFiles + " WHERE CommentFileID = " + CommentFileID, conn);   // RDBJ 01/27/2022 added FileName column
                        sqlAdp.Fill(dtAuditNotesCommentFile);
                        if (dtAuditNotesCommentFile != null && dtAuditNotesCommentFile.Rows.Count > 0)
                        {
                            retDicData["FileData"] = Convert.ToString(dtAuditNotesCommentFile.Rows[0][0]);  // RDBJ 01/27/2022 set with dictionary
                            retDicData["FileName"] = Convert.ToString(dtAuditNotesCommentFile.Rows[0][1]);  // RDBJ 01/27/2022 set with dictionary
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNoteCommentFile_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }

        // RDBJ 01/27/2022
        public Dictionary<string, string> GetAuditNoteCommentFile_Local_DB(string CommentFileID)
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        DataTable dtAuditNotesResolutionFile = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT StorePath, FileName FROM " + AppStatic.AuditNotesCommentsFiles + " WHERE CommentFileUniqueID = '" + CommentFileID + "'", conn);   // JSL 06/16/2022 Change CommentFileUniqueID from ResolutionFileUniqueID
                        sqlAdp.Fill(dtAuditNotesResolutionFile);
                        if (dtAuditNotesResolutionFile != null && dtAuditNotesResolutionFile.Rows.Count > 0)
                        {
                            retDicData["FileData"] = Convert.ToString(dtAuditNotesResolutionFile.Rows[0][0]);
                            retDicData["FileName"] = Convert.ToString(dtAuditNotesResolutionFile.Rows[0][1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNoteResolutionFile_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }
        // End RDBJ 01/27/2022

        public Dictionary<string, string> GetAuditNoteResolutionFile_Local_DB(string ResolutionFileID)  // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();  // RDBJ 01/27/2022 set with dictionary
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        DataTable dtAuditNotesResolutionFile = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT StorePath, FileName FROM " + AppStatic.AuditNotesResolutionFiles + " WHERE ResolutionFileUniqueID = '" + ResolutionFileID + "'", conn);   // RDBJ 01/27/2022 added FileName column
                        sqlAdp.Fill(dtAuditNotesResolutionFile);
                        if (dtAuditNotesResolutionFile != null && dtAuditNotesResolutionFile.Rows.Count > 0)
                        {
                            retDicData["FileData"] = Convert.ToString(dtAuditNotesResolutionFile.Rows[0][0]);  // RDBJ 01/27/2022 set with dictionary
                            retDicData["FileName"] = Convert.ToString(dtAuditNotesResolutionFile.Rows[0][1]);  // RDBJ 01/27/2022 set with dictionary
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNoteResolutionFile_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }
        public Dictionary<string, string> DownloadAuditFile_Local_DB(string IAuditFileID)  // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();  // RDBJ 01/27/2022 set with dictionary
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        DataTable dtAuditNotesFile = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT StorePath, FileName FROM " + AppStatic.AuditNotesAttachment + " WHERE NotesFileUniqueID = '" + IAuditFileID +"'", conn);    // RDBJ 01/27/2022 added FileName column
                        sqlAdp.Fill(dtAuditNotesFile);
                        if (dtAuditNotesFile != null && dtAuditNotesFile.Rows.Count > 0)
                        {
                            retDicData["FileData"] = Convert.ToString(dtAuditNotesFile.Rows[0][0]);  // RDBJ 01/27/2022 set with dictionary
                            retDicData["FileName"] = Convert.ToString(dtAuditNotesFile.Rows[0][1]);  // RDBJ 01/27/2022 set with dictionary
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DownloadAuditFile_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }

        // RDBJ 02/02/2022
        public bool RemoveAuditsOrAuditNotes(string ID
            , string UniqueFormID   // RDBJ 02/04/2022
            , bool IsAudit) 
        {
            try
            {
                string query = string.Empty;
                if (IsAudit)
                {
                    query = "UPDATE " + AppStatic.InternalAuditForm + " SET " +
                        "[isDelete] = @isDelete " +
                        "WHERE UniqueFormID = @ID";
                }
                else
                {
                    query = "UPDATE " + AppStatic.AuditNotes + " SET " +
                        "[isDelete] = @isDelete " +
                        "WHERE NotesUniqueID = @ID";
                }

                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Add("@isDelete", SqlDbType.Int).Value = 1;
                                cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(ID);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }

                // RDBJ 02/04/2022
                if (!IsAudit)
                    ID = UniqueFormID;  

                APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();
                _defhelper.UpdateGIRSyncStatus_Local_DB(Convert.ToString(ID), "IA");

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RemoveAuditsOrAuditNotes : " + ex.Message);
                return false;
            }
        }
        // End RDBJ 02/02/2022

        // RDBJ 02/04/2022
        public string IAFNoteCommentResolutionRecordsExist(string tableName, string columnName, Guid? UniqueID)
        {
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                SqlConnection connection = new SqlConnection(connetionString);
                string res = string.Empty;
                connection.Open();
                DataTable dt1 = new DataTable();
                string isExistRecord = "select " + columnName + " from " + tableName + " where " + columnName + " = '" + UniqueID + "'";
                SqlDataAdapter sqlAdp1 = new SqlDataAdapter(isExistRecord, connection);
                sqlAdp1.Fill(dt1);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0][0] == DBNull.Value)
                        res = string.Empty;
                    else
                        res = Convert.ToString(dt1.Rows[0][0]);
                }
                connection.Close(); //RDBJ 10/27/2021
                return res;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFNoteCommentResolutionRecordsExist :" + ex.Message);
                return string.Empty;
            }
        }
        // End RDBJ 02/04/2022

        // RDBJ 02/04/2022
        public bool DeleteRecords(string tablename, string columnname, string RecID)
        {
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM " + tablename + " WHERE " + columnname + " = '" + RecID + "'", connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteRecords Local DB in table Error : " + ex.Message.ToString());
                return false;
            }
        }
        // End RDBJ 02/04/2022

        // RDBJ2 03/31/2022
        public Dictionary<string, string> AjaxPostPerformAction(Dictionary<string, string> dictMetaData)
        {
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
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

                            if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                            {
                                string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                                SqlConnection connection = new SqlConnection(connetionString);

                                string auditNotesUniqueID = IAFNoteCommentResolutionRecordsExist(AppStatic.AuditNotes, "NotesUniqueID", NotesUniqueID);
                                if (!string.IsNullOrEmpty(auditNotesUniqueID))
                                {
                                    string UpdateQuery = IAFNotes_UpdateQuery(true);
                                    SqlCommand command = new SqlCommand(UpdateQuery, connection);

                                    AuditNote AuditNoteItem = new AuditNote();
                                    AuditNoteItem.NotesUniqueID = NotesUniqueID;
                                    AuditNoteItem.CorrectiveAction = strCorrectiveActions;
                                    AuditNoteItem.PreventativeAction = strPreventativeActions;
                                    AuditNoteItem.Rank = strRank;
                                    AuditNoteItem.Name = strName;
                                    AuditNoteItem.TimeScale = Convert.ToDateTime(strTimeScale);
                                    AuditNoteItem.UpdatedDate = Utility.ToDateTimeUtcNow();

                                    IAFNotes_UpdateCMD(AuditNoteItem, ref command, true);
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    connection.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPDATEAUDITNOTEDETAILS + " Error : " + ex.Message);
                        }
                        IsPerformSuccess = true;
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
        // End RDBJ2 03/31/2022

        // JSL 05/22/2022
        public List<SMSReferencesTree> GetSMSReferenceData()
        {
            List<SMSReferencesTree> lstSMSReference = new List<SMSReferencesTree>();
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM [dbo].[SMSReferencesTree] WHERE IsDeleted = 0";
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
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMSReferenceData Error : " + ex.Message.ToString());
                lstSMSReference = null;
            }
            return lstSMSReference;
        }
        // End JSL 05/22/2022

        // JSL 05/22/2022
        public List<SSPReferenceTree> GetSSPReferenceData()
        {
            List<SSPReferenceTree> lstSSPReference = new List<SSPReferenceTree>();
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM [dbo].[SSPReferenceTree] WHERE IsDeleted = 0";
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
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSSPReferenceData Error : " + ex.Message.ToString());
                lstSSPReference = null;
            }
            return lstSSPReference;
        }
        // End JSL 05/22/2022

        // JSL 05/22/2022
        public List<MLCRegulationTree> GetMLCRegulationTree()
        {
            List<MLCRegulationTree> lstMLCReference = new List<MLCRegulationTree>();
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM [dbo].[MLCRegulationTree] WHERE IsDeleted = 0";
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
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetMLCRegulationTree Error : " + ex.Message.ToString());
                lstMLCReference = null;
            }
            return lstMLCReference;
        }
        // End JSL 05/22/2022
    }
}
