using CarisbrookeShippingService.BLL.Modals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class IAFHelper
    {
        #region Local to AWS
        public void StartIAFSync()
        {
            List<IAF> UnSyncIAFList = GetIAFUnsyncedData();
            if (UnSyncIAFList != null && UnSyncIAFList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for IAF Data is about " + UnSyncIAFList.Count + "");
                List<string> SuccessIds = SendIAFDataToRemote(UnSyncIAFList); // RDBJ 01/19/2022 change List<string>
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncIAFList.Count)
                {
                    UpdateIAFStatus(SuccessIds);
                    LogHelper.writelog("IAF Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < UnSyncIAFList.Count)
                {
                    UpdateIAFStatus(SuccessIds);
                    LogHelper.writelog("Some IAF Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("IAF Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("IAF Data Synced from local to server already done.");
            }
            //StartIAFSyncCloudTOLocal(); //RDBJ 10/05/2021 Commented
        }
        public List<IAF> GetIAFUnsyncedData()
        {
            List<IAF> IAFList = new List<IAF>();
            List<InternalAuditForm> AuditFormsList = new List<InternalAuditForm>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.InternalAuditForm + " WHERE ISNULL(IsSynced,0) = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            try
                            {
                                AuditFormsList = dt.ToListof<InternalAuditForm>();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.writelog("Get InternalAuditForm List " + ex.Message);
                            }
                            foreach (InternalAuditForm item in AuditFormsList)
                            {
                                IAF iafModal = new IAF();   // RDBJ 04/10/2022 set global
                                try
                                {
                                    iafModal.InternalAuditForm = item;
                                    iafModal.AuditNote = new List<AuditNote>();
                                    DataTable dtAuditNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                    sqlAdp.Fill(dtAuditNotes);
                                    iafModal.AuditNote = dtAuditNotes.ToListof<AuditNote>();

                                    if (iafModal.AuditNote != null && iafModal.AuditNote.Count > 0)
                                    {
                                        foreach (AuditNote note in iafModal.AuditNote)
                                        {
                                            if (note.isDelete == 0)
                                            {
                                                note.AuditNotesAttachment = new List<AuditNotesAttachment>();
                                                DataTable dtAuditNotesAttachs = new DataTable();
                                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesAttachment + " WHERE NotesUniqueID = '" + note.NotesUniqueID + "'", conn);
                                                sqlAdp.Fill(dtAuditNotesAttachs);
                                                note.AuditNotesAttachment = dtAuditNotesAttachs.ToListof<AuditNotesAttachment>();
                                                // JSL 11/13/2022
                                                if (note.AuditNotesAttachment != null && note.AuditNotesAttachment.Count > 0)
                                                {
                                                    foreach (var noteFile in note.AuditNotesAttachment)
                                                    {
                                                        if (!noteFile.StorePath.StartsWith("data:"))
                                                        {
                                                            noteFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(noteFile.StorePath);
                                                            noteFile.IsActive = true;
                                                        }
                                                    }
                                                }
                                                // End JSL 11/13/2022

                                                DataTable dtAuditNotesComments = new DataTable();
                                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesComment + " WHERE NotesUniqueID = '" + note.NotesUniqueID + "'", conn);
                                                sqlAdp.Fill(dtAuditNotesComments);
                                                note.AuditNotesComment = dtAuditNotesComments.ToListof<Audit_Deficiency_Comments>();
                                                if (note.AuditNotesComment != null && note.AuditNotesComment.Count > 0)
                                                {
                                                    foreach (var defComment in note.AuditNotesComment)
                                                    {
                                                        DataTable dtDeficienciesCommentsFiles = new DataTable();
                                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesCommentFile + " WHERE CommentUniqueID = '" + defComment.CommentUniqueID + "'", conn);
                                                        sqlAdp.Fill(dtDeficienciesCommentsFiles);
                                                        defComment.AuditDeficiencyCommentsFiles = dtDeficienciesCommentsFiles.ToListof<Audit_Deficiency_Comments_Files>();
                                                        // JSL 11/13/2022
                                                        if (defComment.AuditDeficiencyCommentsFiles != null && defComment.AuditDeficiencyCommentsFiles.Count > 0)
                                                        {
                                                            foreach (var itemFile in defComment.AuditDeficiencyCommentsFiles)
                                                            {
                                                                if (!itemFile.StorePath.StartsWith("data:"))
                                                                {
                                                                    itemFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(itemFile.StorePath);
                                                                }
                                                            }
                                                        }
                                                        // End JSL 11/13/2022
                                                    }
                                                }

                                                DataTable dtAuditNotesResolution = new DataTable();
                                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesResolution + " WHERE NotesUniqueID = '" + note.NotesUniqueID + "'", conn);
                                                sqlAdp.Fill(dtAuditNotesResolution);
                                                note.AuditNotesResolution = dtAuditNotesResolution.ToListof<Audit_Note_Resolutions>();
                                                if (note.AuditNotesResolution != null && note.AuditNotesResolution.Count > 0)
                                                {
                                                    foreach (var defRes in note.AuditNotesResolution)
                                                    {
                                                        DataTable dtDeficienciesResolutionFiles = new DataTable();
                                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesResolutionFiles + " WHERE ResolutionUniqueID = '" + defRes.ResolutionUniqueID + "'", conn);
                                                        sqlAdp.Fill(dtDeficienciesResolutionFiles);
                                                        defRes.AuditNoteResolutionsFiles = dtDeficienciesResolutionFiles.ToListof<Audit_Note_Resolutions_Files>();
                                                        // JSL 11/12/2022
                                                        if (defRes.AuditNoteResolutionsFiles != null && defRes.AuditNoteResolutionsFiles.Count > 0)
                                                        {
                                                            foreach (var itemFile in defRes.AuditNoteResolutionsFiles)
                                                            {
                                                                if (!itemFile.StorePath.StartsWith("data:"))
                                                                {
                                                                    itemFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(itemFile.StorePath);
                                                                }
                                                            }
                                                        }
                                                        // End JSL 11/12/2022
                                                    }
                                                }
                                            }
                                            //IAFList.Add(iafModal);
                                        }
                                    }
                                    else
                                    {
                                        //IAFList.Add(iafModal);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("Get AuditNotes List " + ex.Message);
                                }
                                IAFList.Add(iafModal);  // RDBJ 04/10/2022
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIAFUnsyncedData " + ex.Message);
                return null;
            }
            return IAFList;
        }
        public List<string> SendIAFDataToRemote(List<IAF> UnSyncList) // RDBJ 01/19/2022 change List<string>
        {
            //List<long> SuccessIds = new List<long>(); // RDBJ 01/19/2022 commented this line
            List<string> SuccessIds = new List<string>();
            // JSL 07/16/2022
            APIResponse res = new APIResponse();    
            APIHelper _apiHelper = new APIHelper();
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            // End JSL 07/16/2022

            foreach (var item in UnSyncList)
            {
                //long localDBIAFFormID = item.InternalAuditForm.InternalAuditFormId; // RDBJ 01/19/2022 commented this line
                string localDBIAFFormID = Convert.ToString(item.InternalAuditForm.UniqueFormID); // RDBJ 01/19/2022
                item.InternalAuditForm.InternalAuditFormId = 0;

                CloudLocalIAFSynch _helper = new CloudLocalIAFSynch();
                //res = _helper.sendIAFLocalToRemote(item);
                // JSL 07/18/2022 commented 
                // JSL 07/16/2022
                bool IsAllowToSendDataForServer = false;
                Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                dictMetaData["FormType"] = AppStatic.IAFForm;
                dictMetaData["FormUniqueID"] = item.InternalAuditForm.UniqueFormID.ToString();
                dictMetaData["FormVersion"] = item.InternalAuditForm.FormVersion.ToString();
                dictMetaData["strAction"] = AppStatic.API_METHOD_CheckFormVersion;

                retDictMetaData = _apiHelper.PostAsyncAPICall(AppStatic.APICommonMethods, AppStatic.API_CommonPostAPICall, dictMetaData);
                if (retDictMetaData != null
                    && retDictMetaData.Count > 0    // JSL 10/01/2022
                    )
                {
                    if (retDictMetaData["Status"] == AppStatic.SUCCESS)
                    {
                        IsAllowToSendDataForServer = Convert.ToBoolean(retDictMetaData["IsAllowToInsertOrUpdateData"]);
                        if (IsAllowToSendDataForServer)
                        {
                            IAF modalIAFToSendServer = new IAF();
                            modalIAFToSendServer.InternalAuditForm = item.InternalAuditForm;
                            modalIAFToSendServer.AuditNote = new List<AuditNote>();
                            
                            // JSL 10/28/2022 reorder conditions
                            res = new APIResponse();
                            foreach (var itemDeficiencies in item.AuditNote)
                            {
                                dictMetaData["ShipCode"] = itemDeficiencies.Ship;
                                dictMetaData["DeficienciesData"] = JsonConvert.SerializeObject(itemDeficiencies);
                                dictMetaData["strAction"] = AppStatic.API_METHOD_InsertOrUpdateDeficienciesData;

                                retDictMetaData = _apiHelper.PostAsyncAPICall(AppStatic.APICloudIAF, AppStatic.API_CommonPostAPICall, dictMetaData);
                                if (retDictMetaData != null)
                                {
                                    if (retDictMetaData["Status"] == AppStatic.ERROR)
                                    {
                                        LogHelper.writelog("IAF Data Synced Error for Deficienciy : " + itemDeficiencies.NotesUniqueID.ToString());
                                    }
                                }
                                else
                                {
                                    retDictMetaData["Status"] = AppStatic.ERROR;
                                }
                                dictMetaData["DeficienciesData"] = "";  // JSL 01/03/2023
                            }
                            res.result = retDictMetaData["Status"];

                            if (res != null && res.result == AppStatic.SUCCESS)
                            {
                                res = new APIResponse();
                                res = _helper.sendIAFLocalToRemote(modalIAFToSendServer);
                            }
                            // End JSL 10/28/2022 reorder conditions
                        }
                    }
                }
                // End JSL 07/16/2022
                // JSL 07/18/2022 commented 

                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBIAFFormID);
                }
            }
            return SuccessIds;
        }
        public void UpdateIAFStatus(List<string> SuccessIds) // RDBJ 01/19/2022 change List<string>
        {
            try
            {
                // RDBJ 01/21/2022
                if (SuccessIds == null
                    || SuccessIds.Count == 0)   // JSL 09/09/2022
                    return;

                string IdsStr = string.Join(",", SuccessIds.Select(x => string.Format("'{0}'", x)));
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        string Query = "UPDATE " + AppStatic.InternalAuditForm + " SET IsSynced = 1 WHERE UniqueFormID in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateIAFStatus : " + ex.Message);
            }
        }
        #endregion

        #region AWS to Local
        public void StartIAFSyncCloudTOLocal(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<IAF> UnSyncList = GetIAFUnsyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine  // JSL 11/12/2022
                );
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for IAF Data is about " + UnSyncList.Count + "");
                List<string> SuccessIds = SendIAFDataToLocal(UnSyncList);
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                {
                    UpdateCloudIAFFormsStatus(SuccessIds);
                    LogHelper.writelog("IAF Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                {
                    UpdateCloudIAFFormsStatus(SuccessIds);
                    LogHelper.writelog("Some IAF Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("IAF Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("IAF Data Synced already done.");
            }
        }
        public List<IAF> GetIAFUnsyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<IAF> UnSyncListFromCloud = new List<IAF>();
            try
            {
                // JSL 11/12/2022
                string strShipCode = string.Empty;
                if (!IsInspectorInThisMachine)
                {
                    strShipCode = shipCode;
                }
                // End JSL 11/12/2022

                APIHelper _helper = new APIHelper();
                UnSyncListFromCloud = _helper.GetIAFGeneralDescription(strShipCode);    // JSL 11/12/2022 added strShipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIAFUnsyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return UnSyncListFromCloud;
        }
        public List<string> SendIAFDataToLocal(List<IAF> UnSyncList)
        {
            List<string> SuccessIds = new List<string>();
            foreach (var item in UnSyncList)
            {
                string localDBIAFUniqueFormID = Convert.ToString(item.InternalAuditForm.UniqueFormID);
                item.InternalAuditForm.InternalAuditFormId = 0;
                APIResponse res = SaveIAFDataInLocalDB(item);

                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBIAFUniqueFormID);
                }
            }
            return SuccessIds;
        }
        public APIResponse SaveIAFDataInLocalDB(IAF Modal)
        {
            APIResponse res = new APIResponse();
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);

            //string UniqueFormID = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID FROM " + AppStatic.InternalAuditForm + " WHERE UniqueFormID = '" + Modal.InternalAuditForm.UniqueFormID + "'", connection);
                sqlAdp.Fill(dt);

                Modal.InternalAuditForm.IsSynced = true;
                //UniqueFormID = Convert.ToString(Modal.InternalAuditForm.UniqueFormID);
                if (dt.Rows.Count > 0)
                {
                    var girLocalList = dt.ToListof<InternalAuditForm>().FirstOrDefault(); //RDBJ 10/05/2021 Replace with InternalAuditForm from GeneralInspectionReport
                    if (!girLocalList.FormVersion.HasValue || Modal.InternalAuditForm.FormVersion > girLocalList.FormVersion)
                    {
                        string UpdateQury = GETIAFUpdateQuery();
                        SqlCommand command = new SqlCommand(UpdateQury, connection);

                        IAFUpdateCMD(Modal, ref command);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    string InsertQury = IAFDataInsertQuery();
                    SqlCommand command = new SqlCommand(InsertQury, connection);

                    IAFDataInsertCMD(Modal.InternalAuditForm, ref command);

                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }
                IAFNotes_Save(Modal.AuditNote);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add InternalAuditForm In Local DB Error : " + ex.Message.ToString()); // RDBJ 01/21/2022
                LogHelper.writelog("Add InternalAuditForm In Local DB : " + Modal.InternalAuditForm.UniqueFormID + "  Inner Error : " + ex.InnerException.ToString()); // RDBJ 01/21/2022

                res.result = AppStatic.ERROR;
                res.msg = ex.Message.ToString();
            }
            // RDBJ 01/21/2022 added finally
            finally
            {
                IfConnectionOpenThenCloseIt(connection);
            }
            return res;
        }
        public string IAFDataInsertQuery()
        {
            // RDBJ 02/06/2022 added SavedAsDraft //RDBJ 11/25/2021 Added isDelete,AuditType,IsClosed,IsAdditional
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
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.FormVersion; //RDBJ 11/25/2021 set Decimal dataType
            command.Parameters.Add("@ShipName", SqlDbType.VarChar).Value = Modal.ShipName;
            command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Modal.Location == null ? string.Empty : Modal.Location; // RDBJ 02/09/2022 Handle null
            command.Parameters.Add("@AuditNo", SqlDbType.VarChar).Value = Modal.AuditNo == null ? "1" : Modal.AuditNo;  // RDBJ 02/08/2022 Handle Null
            command.Parameters.Add("@AuditTypeISM", SqlDbType.Bit).Value = Modal.AuditTypeISM == null ? false : Modal.AuditTypeISM; // RDBJ 01/19/2022 set Actual value
            command.Parameters.Add("@AuditTypeISPS", SqlDbType.Bit).Value = Modal.AuditTypeISPS == null ? false : Modal.AuditTypeISPS; // RDBJ 01/19/2022 set Actual value
            command.Parameters.Add("@AuditTypeMLC", SqlDbType.Bit).Value = Modal.AuditTypeMLC == null ? false : Modal.AuditTypeMLC; // RDBJ 01/19/2022 set Actual value
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date;
            command.Parameters.Add("@Auditor", SqlDbType.VarChar).Value = Modal.Auditor != null ? Modal.Auditor : "";   // RDBJ 02/09/2022 Handle null
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced; //Updated dynamic rather than false RDBJ 11/25/2021
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete; //RDBJ 11/25/2021
            command.Parameters.Add("@AuditType", SqlDbType.Int).Value = Modal.AuditType; //RDBJ 11/25/2021
            command.Parameters.Add("@IsClosed", SqlDbType.Bit).Value = Modal.IsClosed; //RDBJ 11/25/2021
            command.Parameters.Add("@IsAdditional", SqlDbType.Bit).Value = Modal.IsAdditional; //RDBJ 11/25/2021
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft; // RDBJ 02/06/2022
        }
        public string GETIAFUpdateQuery()
        {
            // RDBJ 02/06/2022 added SavedAsDraft //RDBJ 11/25/2021 Added isDelete,AuditType,IsClosed,IsAdditional
            string query = @"UPDATE dbo.InternalAuditForm SET ShipId = @ShipId, ShipName = @ShipName, Location = @Location, 
                            AuditNo = @AuditNo, AuditTypeISM = @AuditTypeISM, AuditTypeISPS = @AuditTypeISPS, AuditTypeMLC = @AuditTypeMLC, 
                            Date = @Date, Auditor = @Auditor, CreatedDate = @CreatedDate, UpdatedDate = @UpdatedDate, IsSynced = @IsSynced, 
                            FormVersion = @FormVersion, isDelete = @isDelete, AuditType = @AuditType, IsClosed = @IsClosed, IsAdditional = @IsAdditional, SavedAsDraft = @SavedAsDraft
                            WHERE UniqueFormID = @UniqueFormID";
            return query;
        }
        public void IAFUpdateCMD(IAF Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.InternalAuditForm.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.InternalAuditForm.FormVersion;
            //command.Parameters.Add("@InternalAuditFormId", SqlDbType.BigInt).Value = Modal.InternalAuditForm.InternalAuditFormId;
            command.Parameters.Add("@ShipId", SqlDbType.Int).Value = Modal.InternalAuditForm.ShipId == null ? DBNull.Value : (object)Modal.InternalAuditForm.ShipId;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.ShipName == null ? string.Empty : Modal.InternalAuditForm.ShipName;
            command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.Location == null ? string.Empty : Modal.InternalAuditForm.Location;
            command.Parameters.Add("@AuditNo", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.AuditNo == null ? "1" : Modal.InternalAuditForm.AuditNo;
            command.Parameters.Add("@AuditTypeISM", SqlDbType.Bit).Value = Modal.InternalAuditForm.AuditTypeISM == null ? false : (object)Modal.InternalAuditForm.AuditTypeISM;  // RDBJ 02/06/2022 set type cast and handle null
            command.Parameters.Add("@AuditTypeISPS", SqlDbType.Bit).Value = Modal.InternalAuditForm.AuditTypeISPS == null ? false : (object)Modal.InternalAuditForm.AuditTypeISPS; // RDBJ 02/06/2022 set type cast and handle null
            command.Parameters.Add("@AuditTypeMLC", SqlDbType.Bit).Value = Modal.InternalAuditForm.AuditTypeMLC == null ? false : (object)Modal.InternalAuditForm.AuditTypeMLC; // RDBJ 02/06/2022 set type cast and handle null
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.InternalAuditForm.Date; //RDBJ 10/05/2021 Replace type with DateTime from NVarChar
            command.Parameters.Add("@Auditor", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.Auditor != null ? Modal.InternalAuditForm.Auditor : "";
            command.Parameters.Add("@IsSynced", SqlDbType.NVarChar).Value = Modal.InternalAuditForm.IsSynced == null ? DBNull.Value : (object)Modal.InternalAuditForm.IsSynced;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.InternalAuditForm.CreatedDate == null ? DBNull.Value : (object)Modal.InternalAuditForm.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.InternalAuditForm.UpdatedDate == null ? Utility.ToDateTimeUtcNow() : (object)Modal.InternalAuditForm.UpdatedDate; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.InternalAuditForm.isDelete; //RDBJ 11/25/2021
            command.Parameters.Add("@AuditType", SqlDbType.Int).Value = Modal.InternalAuditForm.AuditType; //RDBJ 11/25/2021
            command.Parameters.Add("@IsClosed", SqlDbType.Bit).Value = Modal.InternalAuditForm.IsClosed; //RDBJ 11/25/2021
            command.Parameters.Add("@IsAdditional", SqlDbType.Bit).Value = Modal.InternalAuditForm.IsAdditional; //RDBJ 11/25/2021
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.InternalAuditForm.SavedAsDraft; // RDBJ 02/06/2022
        }

        //RDBJ 10/05/2021
        #region Check AuditNotes, Comments, Resolution and Files Exist or not
        public string IAFNoteCommentResolutionRecordsExist(string tableName, string columnName, Guid? UniqueID)
        {
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
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
        public string IAFNoteCommentResolutionFileRecordsExist(string tableName, string columnName, Guid? FileUniqueID)
        {
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                SqlConnection connection = new SqlConnection(connetionString);
                string res = string.Empty;
                connection.Open();
                DataTable dt1 = new DataTable();
                string isExistRecord = "select " + columnName + " from " + tableName + " where " + columnName + " = '" + FileUniqueID + "'";
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
                LogHelper.writelog("IAFNoteCommentResolutionFileRecordsExist :" + ex.Message);
                return string.Empty;
            }
        }
        #endregion
        //End RDBJ 10/05/2021
        public void IAFNotes_Save(List<AuditNote> auditNotes) //RDBJ 10/05/2021 Removed Guid UniqueFormID
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (auditNotes != null && auditNotes.Count > 0) //RDBJ 10/05/2021 Removed && UniqueFormID != Guid.Empty
                {
                    foreach (var item in auditNotes)
                    {
                        string auditNotesUniqueID = IAFNoteCommentResolutionRecordsExist(AppStatic.AuditNotes, "NotesUniqueID", item.NotesUniqueID);
                        try
                        {
                            if (!string.IsNullOrEmpty(auditNotesUniqueID))
                            {
                                string UpdateQuery = IAFNotes_UpdateQuery();
                                SqlCommand command = new SqlCommand(UpdateQuery, connection);
                                IAFNotes_UpdateCMD(item, ref command);
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close(); //RDBJ 10/27/2021
                            }
                            else
                            {
                                item.CreatedDate = item.CreatedDate ?? Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                string InsertQuery = IAFNotes_InsertQuery();
                                SqlCommand command = new SqlCommand(InsertQuery, connection);
                                IAFNotes_CMD(item, ref command);
                                connection.Open();
                                command.ExecuteScalar();
                                connection.Close(); //RDBJ 10/27/2021
                            }

                            // JSL 11/13/2022
                            Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                            dicFileMetaData["UniqueFormID"] = Convert.ToString(item.UniqueFormID);
                            dicFileMetaData["ReportType"] = "IA";
                            dicFileMetaData["DetailUniqueId"] = Convert.ToString(item.NotesUniqueID);
                            // End JSL 11/13/2022

                            IAFNotesFiles_Save(item.AuditNotesAttachment
                                , item.NotesUniqueID  // RDBJ 02/06/2022
                                , dicFileMetaData   // JSL 11/13/2022
                                );
                            AuditNotesComment_Save(item.AuditNotesComment
                                , dicFileMetaData   // JSL 11/13/2022
                                );
                            AuditNotesResolution_Save(item.AuditNotesResolution //RDBJ 10/05/2021
                                , dicFileMetaData   // JSL 11/13/2022
                                );
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("IAFNotes_Save : Failed to add Audit Note : " + ex.Message.ToString());

                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRDeficiencies_Save table Error : " + ex.Message.ToString());
            }
        }
        public string IAFNotes_InsertQuery()
        {
            // RDBJ 12/21/2021 Added AssignTo //RDBJ 11/25/2021 Added Priority,isDelete,InternalAuditFormId,CreatedDate
            string InsertQuery = @"INSERT INTO dbo.AuditNotes 
                                  (NotesUniqueID,UniqueFormID,Number,Type,BriefDescription,Reference,FullDescription,CorrectiveAction,AssignTo,
                                    PreventativeAction,Rank,Name,TimeScale,Ship,UpdatedDate,DateClosed,IsResolved,Priority,isDelete,InternalAuditFormId,CreatedDate)
                                  VALUES (@NotesUniqueID,@UniqueFormID,@Number,@Type,@BriefDescription,@Reference,@FullDescription,@CorrectiveAction,@AssignTo,
                                @PreventativeAction,@Rank,@Name,@TimeScale,@Ship,@UpdatedDate,@DateClosed,@IsResolved,@Priority,@isDelete,@InternalAuditFormId,@CreatedDate)";
            return InsertQuery;
        }
        public void IAFNotes_CMD(AuditNote Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            //RDBJ 11/25/2021 modified DataType int to varchar
            command.Parameters.Add("@InternalAuditFormId", SqlDbType.BigInt).Value = Modal.InternalAuditFormId;
            command.Parameters.Add("@Number", SqlDbType.VarChar).Value = Modal.Number;
            command.Parameters.Add("@Type", SqlDbType.VarChar).Value = Modal.Type;
            command.Parameters.Add("@BriefDescription", SqlDbType.VarChar).Value = Modal.BriefDescription;
            command.Parameters.Add("@Reference", SqlDbType.VarChar).Value = (object)Modal.Reference ?? DBNull.Value;    // JSL 02/25/2023 handle null
            command.Parameters.Add("@FullDescription", SqlDbType.VarChar).Value = Modal.FullDescription == null ? DBNull.Value : (object)Modal.FullDescription;
            command.Parameters.Add("@CorrectiveAction", SqlDbType.VarChar).Value = Modal.CorrectiveAction == null ? DBNull.Value : (object)Modal.CorrectiveAction;
            command.Parameters.Add("@PreventativeAction", SqlDbType.VarChar).Value = Modal.PreventativeAction == null ? DBNull.Value : (object)Modal.PreventativeAction;
            command.Parameters.Add("@Rank", SqlDbType.VarChar).Value = Modal.Rank == null ? DBNull.Value : (object)Modal.Rank;
            command.Parameters.Add("@Name", SqlDbType.VarChar).Value = Modal.Name == null ? DBNull.Value : (object)Modal.Name;
            command.Parameters.Add("@TimeScale", SqlDbType.DateTime).Value = Modal.TimeScale == null ? DBNull.Value : (object)Modal.TimeScale;
            command.Parameters.Add("@Ship", SqlDbType.VarChar).Value = Modal.Ship;
            //End RDBJ 11/25/2021 modified DataType int to varchar
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //DateTime.Now;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate; //DateTime.Now;
            command.Parameters.Add("@IsResolved", SqlDbType.Bit).Value = Modal.IsResolved == null ? false : (object)Modal.IsResolved; // RDBJ 01/19/2022 make case
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority; //RDBJ 11/25/2021
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete; //RDBJ 11/25/2021
            command.Parameters.Add("@AssignTo", SqlDbType.UniqueIdentifier).Value = Modal.AssignTo == null ? DBNull.Value : (object)Modal.AssignTo; // RDBJ 12/21/2021
        }
        public string IAFNotes_UpdateQuery()
        {
            // RDBJ 12/21/2021 Added AssignTo //RDBJ 11/25/2021 Added Priority,isDelete,CreatedDate
            string UpdateQuery = @"UPDATE dbo.AuditNotes SET
                                UniqueFormID= @UniqueFormID, Number = @Number, Type = @Type, BriefDescription = @BriefDescription, Reference = @Reference,
                                FullDescription = @FullDescription,CorrectiveAction=@CorrectiveAction,PreventativeAction=@PreventativeAction,
                                Rank=@Rank,Name=@Name,TimeScale=@TimeScale,Ship=@Ship, CreatedDate = @CreatedDate, AssignTo = @AssignTo,
                                UpdatedDate=@UpdatedDate,DateClosed=@DateClosed,IsResolved=@IsResolved, Priority = @Priority, isDelete = @isDelete
                                WHERE NotesUniqueID = @NotesUniqueID";
            return UpdateQuery;
        }
        public void IAFNotes_UpdateCMD(AuditNote Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID; //RDBJ 10/05/2021
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@Number", SqlDbType.VarChar).Value = Modal.Number;
            command.Parameters.Add("@Type", SqlDbType.VarChar).Value = Modal.Type;
            command.Parameters.Add("@BriefDescription", SqlDbType.VarChar).Value = Modal.BriefDescription;
            command.Parameters.Add("@Reference", SqlDbType.VarChar).Value = (object)Modal.Reference ?? DBNull.Value;    // JSL 05/22/2022 allow Null add reference
            
            //RDBJ 11/25/2021 handle Null value 
            command.Parameters.Add("@FullDescription", SqlDbType.VarChar).Value = Modal.FullDescription == null ? DBNull.Value : (object)Modal.FullDescription;
            command.Parameters.Add("@CorrectiveAction", SqlDbType.VarChar).Value = Modal.CorrectiveAction == null ? DBNull.Value : (object)Modal.CorrectiveAction;
            command.Parameters.Add("@PreventativeAction", SqlDbType.VarChar).Value = Modal.PreventativeAction == null ? DBNull.Value : (object)Modal.PreventativeAction;
            command.Parameters.Add("@Rank", SqlDbType.VarChar).Value = Modal.Rank == null ? DBNull.Value : (object)Modal.Rank;
            command.Parameters.Add("@Name", SqlDbType.VarChar).Value = Modal.Name == null ? DBNull.Value : (object)Modal.Name;
            command.Parameters.Add("@TimeScale", SqlDbType.DateTime).Value = Modal.TimeScale == null ? DBNull.Value : (object)Modal.TimeScale;
            //End RDBJ 11/25/2021 handle Null value 

            command.Parameters.Add("@Ship", SqlDbType.VarChar).Value = Modal.Ship;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed; //RDBJ 11/25/2021 handle Null value 
            command.Parameters.Add("@IsResolved", SqlDbType.Bit).Value = Modal.IsResolved == null ? false : (object)Modal.IsResolved; // RDBJ 01/19/2022 make case
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority; //RDBJ 11/25/2021
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete; //RDBJ 11/25/2021
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //RDBJ 11/25/2021

            command.Parameters.Add("@AssignTo", SqlDbType.UniqueIdentifier).Value = Modal.AssignTo == null ? DBNull.Value : (object)Modal.AssignTo; // RDBJ 12/21/2021
            //command.Parameters.Add("@IsResolved", SqlDbType.Bit).Value = Modal.isResolved;
            //command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
            //command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            //command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate;  //DateTime.Now;DateTime.Now;
        }
        public void IAFNotesFiles_Save(List<AuditNotesAttachment> AuditNoteFiles //RDBJ 10/05/2021 Removed long AuditNotesId
            , Guid? NotesUniqueID  // RDBJ 02/06/2022 added NotesUniqueID
            , Dictionary<string, string> dicFileMetaData   // JSL 11/13/2022
            )
        {
            GIRFormDataHelper _girHElper = new GIRFormDataHelper(); // RDBJ 02/06/2022
            bool res = _girHElper.DeleteRecords(AppStatic.AuditNotesAttachment, "NotesUniqueID", Convert.ToString(NotesUniqueID)); // RDBJ 02/06/2022

            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (AuditNoteFiles != null && AuditNoteFiles.Count > 0) //RDBJ 10/05/2021 Removed && AuditNotesId > 0
                {
                    foreach (var item in AuditNoteFiles)
                    {
                        string auditNoteFileUniqueID = IAFNoteCommentResolutionFileRecordsExist(AppStatic.AuditNotesAttachment, "NotesFileUniqueID", item.NotesFileUniqueID);
                        if (string.IsNullOrEmpty(auditNoteFileUniqueID))
                        {
                            // JSL 11/13/2022
                            if (item.StorePath.StartsWith("data:"))
                            {
                                dicFileMetaData["FileName"] = item.FileName;
                                dicFileMetaData["Base64FileData"] = item.StorePath;

                                item.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                            }
                            // End JSL 11/13/2022
                            item.NotesUniqueID = NotesUniqueID; // RDBJ 02/06/2022
                            string InsertQury = IAFAuditNotesAttachmentsDataInsertQuery();
                            connection.Open();
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            IAFAuditNotesAttachmentsDataInsertCMD(item, ref command);
                            command.ExecuteScalar();
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFNotesFiles_Save Add in Local DB table Error : " + ex.Message.ToString());
            }
        }
        public string IAFAuditNotesAttachmentsDataInsertQuery()
        {
            string InsertQury = @"INSERT INTO dbo.AuditNotesAttachment (NotesFileUniqueID,NotesUniqueID,InternalAuditFormId,AuditNotesId,FileName,StorePath)
                                OUTPUT INSERTED.AuditNotesAttachmentId
                                VALUES (@NotesFileUniqueID,@NotesUniqueID,@InternalAuditFormId,@AuditNotesId,@FileName,@StorePath)";
            return InsertQury;
        }
        public void IAFAuditNotesAttachmentsDataInsertCMD(AuditNotesAttachment Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@NotesFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesFileUniqueID; //RDBJ 10/05/2021
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID; //RDBJ 10/05/2021
            command.Parameters.Add("@InternalAuditFormId", SqlDbType.BigInt).Value = Modal.InternalAuditFormId;
            command.Parameters.Add("@AuditNotesId", SqlDbType.BigInt).Value = Modal.AuditNotesId;
            command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.VarChar).Value = Modal.StorePath;
        }

        public void AuditNotesComment_Save(List<Audit_Deficiency_Comments> modalAuditNotesComments
            , Dictionary<string, string> dicFileMetaData   // JSL 11/13/2022
            )
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (modalAuditNotesComments != null && modalAuditNotesComments.Count > 0) //RDBJ 10/05/2021 Reoved && DefID > 0
                {
                    foreach (var itemComments in modalAuditNotesComments)
                    {
                        string CommentUniqueID = IAFNoteCommentResolutionRecordsExist(AppStatic.AuditNotesComment, "CommentUniqueID", itemComments.CommentUniqueID);
                        if (string.IsNullOrEmpty(CommentUniqueID))
                        {
                            connection.Open();
                            string InsertQuery = AuditDeficiencyComments_InsertQuery();
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            AuditDeficiencyComments_CMD(itemComments, ref command);
                            command.ExecuteScalar();
                            connection.Close();
                            
                            if (itemComments.AuditDeficiencyCommentsFiles != null && itemComments.AuditDeficiencyCommentsFiles.Count > 0)
                            {
                                foreach (var itemCommentsFile in itemComments.AuditDeficiencyCommentsFiles)
                                {
                                    string commentFileUID = IAFNoteCommentResolutionFileRecordsExist(AppStatic.AuditNotesCommentFile, "CommentFileUniqueID", itemCommentsFile.CommentFileUniqueID);
                                    if (string.IsNullOrEmpty(commentFileUID))
                                    {
                                        // JSL 11/13/2022
                                        if (itemCommentsFile.StorePath.StartsWith("data:"))
                                        {
                                            dicFileMetaData["FileName"] = itemCommentsFile.FileName;
                                            dicFileMetaData["Base64FileData"] = itemCommentsFile.StorePath;

                                            // JSL 01/08/2023
                                            dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeComment;
                                            dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentsFile.CommentUniqueID);
                                            // End JSL 01/08/2023

                                            itemCommentsFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                        }
                                        // End JSL 11/13/2022
                                        connection.Open();
                                        string commentFileInsertQuery = AuditDeficiencyCommentsFiles_InsertQuery();
                                        command = new SqlCommand(commentFileInsertQuery, connection);
                                        AuditDeficiencyCommentsFiles_CMD(itemCommentsFile, ref command);
                                        command.ExecuteScalar();
                                        connection.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AuditNotesComment_Save : Add in Local DB table Error : " + ex.Message.ToString());
            }
        }
        public string AuditDeficiencyComments_InsertQuery()
        {
            //RDBJ 10/25/2021 Added isNew Column
            string InsertQuery = @"INSERT INTO dbo.AuditNotesComments 
                                  (AuditNoteID,UserName,Comment,CreatedDate,UpdatedDate,NotesUniqueID,CommentUniqueID,isNew)
                                  OUTPUT INSERTED.CommentsID
                                  VALUES (@AuditNoteID,@UserName,@Comment,@CreatedDate,@UpdatedDate,@NotesUniqueID,@CommentUniqueID,@isNew)";
            return InsertQuery;
        }
        public void AuditDeficiencyComments_CMD(Audit_Deficiency_Comments Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@CommentUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentUniqueID;
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@AuditNoteID", SqlDbType.BigInt).Value = Modal.AuditNoteID == null ? DBNull.Value : (object)Modal.AuditNoteID;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = Modal.UserName == null ? DBNull.Value : (object)Modal.UserName;
            command.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = Modal.Comment == null ? DBNull.Value : (object)Modal.Comment;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //DateTime.Now; //RDBJ 10/25/2021 Set Actual Reponsed date rather than now
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate; //DateTime.Now; //RDBJ 10/25/2021 Set Actual Reponsed date rather than now
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 1; //RDBJ 10/25/2021
        }
        public string AuditDeficiencyCommentsFiles_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.AuditNotesCommentsFiles 
                                  (CommentsID,AuditNoteID,FileName,StorePath,CommentUniqueID,CommentFileUniqueID)
                                  OUTPUT INSERTED.CommentFileID
                                  VALUES (@CommentsID,@AuditNoteID,@FileName,@StorePath,@CommentUniqueID,@CommentFileUniqueID)";
            return InsertQuery;
        }
        public void AuditDeficiencyCommentsFiles_CMD(Audit_Deficiency_Comments_Files Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@CommentFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentFileUniqueID;
            command.Parameters.Add("@CommentUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentUniqueID;
            command.Parameters.Add("@CommentsID", SqlDbType.BigInt).Value = Modal.CommentsID;
            command.Parameters.Add("@AuditNoteID", SqlDbType.BigInt).Value = Modal.AuditNoteID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
        }

        //RDBJ 10/05/2021
        public void AuditNotesResolution_Save(List<Audit_Note_Resolutions> modalAuditNotesResolutions
            , Dictionary<string, string> dicFileMetaData   // JSL 11/13/2022
            )
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (modalAuditNotesResolutions != null && modalAuditNotesResolutions.Count > 0) //RDBJ 10/05/2021 Reoved && DefID > 0
                {
                    foreach (var itemResolution in modalAuditNotesResolutions)
                    {
                        string ResolutionUniqueID = IAFNoteCommentResolutionRecordsExist(AppStatic.AuditNotesResolution, "ResolutionUniqueID", itemResolution.ResolutionUniqueID);
                        if (string.IsNullOrEmpty(ResolutionUniqueID))
                        {
                            connection.Open();
                            string InsertQuery = AuditNoteResolution_InsertQuery();
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            AuditNoteResolution_CMD(itemResolution, ref command);
                            command.ExecuteScalar();
                            connection.Close();

                            if (itemResolution.AuditNoteResolutionsFiles != null && itemResolution.AuditNoteResolutionsFiles.Count > 0)
                            {
                                foreach (var itemResolutionFile in itemResolution.AuditNoteResolutionsFiles)
                                {
                                    string ResolutionFileUniqueID = IAFNoteCommentResolutionFileRecordsExist(AppStatic.AuditNotesResolutionFiles, "ResolutionFileUniqueID", itemResolutionFile.ResolutionFileUniqueID);
                                    if (string.IsNullOrEmpty(ResolutionFileUniqueID))
                                    {
                                        // JSL 11/13/2022
                                        if (itemResolutionFile.StorePath.StartsWith("data:"))
                                        {
                                            dicFileMetaData["FileName"] = itemResolutionFile.FileName;
                                            dicFileMetaData["Base64FileData"] = itemResolutionFile.StorePath;

                                            // JSL 01/08/2023
                                            dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeResolution;
                                            dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemResolutionFile.ResolutionUniqueID);
                                            // End JSL 01/08/2023

                                            itemResolutionFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                        }
                                        // End JSL 11/13/2022
                                        connection.Open();
                                        string commentFileInsertQuery = AuditNoteResolutionFiles_InsertQuery();
                                        command = new SqlCommand(commentFileInsertQuery, connection);
                                        AuditNoteResolutionFiles_CMD(itemResolutionFile, ref command);
                                        command.ExecuteScalar();
                                        connection.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AuditNotesResolution_Save : Add in Local DB table Error : " + ex.Message.ToString());
            }
        }
        public string AuditNoteResolution_InsertQuery()
        {
            //RDBJ 10/25/2021 Added isNew Column
            string InsertQuery = @"INSERT INTO dbo.AuditNotesResolution 
                                  (AuditNoteID,UserName,Resolution,CreatedDate,UpdatedDate,NotesUniqueID,ResolutionUniqueID,isNew)
                                  OUTPUT INSERTED.ResolutionID
                                  VALUES (@AuditNoteID,@UserName,@Resolution,@CreatedDate,@UpdatedDate,@NotesUniqueID,@ResolutionUniqueID,@isNew)";
            return InsertQuery;
        }
        public void AuditNoteResolution_CMD(Audit_Note_Resolutions Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@AuditNoteID", SqlDbType.BigInt).Value = Modal.AuditNoteID;
            command.Parameters.Add("@ResolutionUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionUniqueID;
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = Modal.UserName == null ? DBNull.Value : (object)Modal.UserName;
            command.Parameters.Add("@Resolution", SqlDbType.NVarChar).Value = Modal.Resolution == null ? DBNull.Value : (object)Modal.Resolution;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //DateTime.Now; //RDBJ 10/25/2021 Set Actual Reponsed date rather than now
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate; //DateTime.Now; //RDBJ 10/25/2021 Set Actual Reponsed date rather than now
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 1; //RDBJ 10/25/2021

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
        public void UpdateCloudIAFFormsStatus(List<string> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds);
                APIHelper _helper = new APIHelper();
                _helper.sendSynchIAFListUFID(SuccessIds); // RDBJ 01/19/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCloudIAFFormsStatus : " + ex.Message);
            }
        }
        //End RDBJ 10/05/2021
        #endregion

        //RDBJ 10/05/2021
        #region IAFSynch Based on Latest Version
        public void GETIAFLatestData(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<InternalAuditForm> CloudSyncList = GetIAFFormsSyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine  // JSL 11/12/2022
                );
            List<InternalAuditForm> LocalSyncList = GetIAFFormsSyncedDataFromLocal();

            if (CloudSyncList != null && CloudSyncList.Count > 0)
            {
                LogHelper.writelog("GETIAFLatestData : SyncList count for IAF Data is about " + CloudSyncList.Count + "");
                foreach (var CloudIAF in CloudSyncList)
                {
                    try
                    {
                        InternalAuditForm LocalIAF = LocalSyncList.Where(x => x.UniqueFormID == CloudIAF.UniqueFormID && x.ShipName == CloudIAF.ShipName).FirstOrDefault();
                        if (LocalIAF != null)
                        {
                            InsertUpdateIAFProcess(CloudIAF, LocalIAF);
                        }
                        else
                        {
                            IAF IAFFormData = GetIAFSyncedDataFromCloud(Convert.ToString(CloudIAF.UniqueFormID));
                            SaveIAFDataInLocalDB(IAFFormData);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("GETIAFLatestData : IAF Data Synced Not done. Error : " + ex.Message);
                    }
                }
            }
            else
            {
                LogHelper.writelog("GETIAFLatestData : IAF Data Synced already done.");
            }
        }
        public List<InternalAuditForm> GetIAFFormsSyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<InternalAuditForm> SyncListFromCloud = new List<InternalAuditForm>();
            try
            {
                // JSL 11/12/2022
                string strShipCode = string.Empty;
                if (!IsInspectorInThisMachine)
                {
                    strShipCode = shipCode;
                }
                // End JSL 11/12/2022
                APIHelper _helper = new APIHelper();
                SyncListFromCloud = _helper.GetIAFFormsSyncedDataFromCloud(strShipCode);    // JSL 11/12/2022 added strShipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIAFFormsSyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncListFromCloud;
        }
        public List<InternalAuditForm> GetIAFFormsSyncedDataFromLocal()
        {
            List<InternalAuditForm> SyncList = new List<InternalAuditForm>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID, FormVersion, ShipName, IsSynced FROM " + AppStatic.InternalAuditForm, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<InternalAuditForm>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIAFFormsSyncedDataFromLocal " + ex.Message);
            }
            return SyncList;
        }
        public void InsertUpdateIAFProcess(InternalAuditForm CloudIAF, InternalAuditForm LocalIAF)
        {
            IAF IAFFormData = new IAF();
            try
            {
                if (CloudIAF.FormVersion > LocalIAF.FormVersion)
                {
                    IAFFormData = GetIAFSyncedDataFromCloud(Convert.ToString(CloudIAF.UniqueFormID));
                    SaveIAFDataInLocalDB(IAFFormData);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertUpdateIAFProcess : " + ex.Message);
            }
        }
        public IAF GetIAFSyncedDataFromCloud(string UniqueFormID)
        {
            IAF SyncIAFFromCloud = new IAF();
            try
            {
                APIHelper _helper = new APIHelper();
                SyncIAFFromCloud = _helper.GetIAFSyncedDataFromCloud(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIAFSyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncIAFFromCloud;
        }
        #endregion
        //End RDBJ 10/05/2021

        // JSL 05/20/2022
        #region SMS, SSP, MLC References Tables
        // JSL 05/20/2022
        public void StartSMSReferencesSyncAWSServerToLocal()
        {
            string strGetDataForTheTable = "SMS";
            List<Dictionary<string, object>> dictCloudMLCReferencesList = GetReferencesDataFromCloud(strGetDataForTheTable);
            if (dictCloudMLCReferencesList != null && dictCloudMLCReferencesList.Count > 0)
            {
                SendReferencesDataToLocal(dictCloudMLCReferencesList, strGetDataForTheTable);
                LogHelper.writelog("Synced SMSReferences Data is about " + dictCloudMLCReferencesList.Count);
            }
            else
            {
                LogHelper.writelog("SMSReferences Data Synced already done.");
            }
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public void StartSSPReferencesSyncAWSServerToLocal()
        {
            string strGetDataForTheTable = "SSP";
            List<Dictionary<string, object>> dictCloudMLCReferencesList = GetReferencesDataFromCloud(strGetDataForTheTable);
            if (dictCloudMLCReferencesList != null && dictCloudMLCReferencesList.Count > 0)
            {
                SendReferencesDataToLocal(dictCloudMLCReferencesList, strGetDataForTheTable);
                LogHelper.writelog("Synced count for SSPReferences Data is about " + dictCloudMLCReferencesList.Count);
            }
            else
            {
                LogHelper.writelog("SSPReferences Data Synced already done.");
            }
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public void StartMLCReferencesSyncAWSServerToLocal()
        {
            string strGetDataForTheTable = "MLC";
            List<Dictionary<string, object>> dictCloudMLCReferencesList = GetReferencesDataFromCloud(strGetDataForTheTable);
            if (dictCloudMLCReferencesList != null && dictCloudMLCReferencesList.Count > 0)
            {
                SendReferencesDataToLocal(dictCloudMLCReferencesList, strGetDataForTheTable);
                LogHelper.writelog("Synced count for MLCReferences Data is about " + dictCloudMLCReferencesList.Count);
            }
            else
            {
                LogHelper.writelog("MLCReferences Data Synced already done.");
            }
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public List<Dictionary<string, object>> GetReferencesDataFromCloud(string strTableName)
        {
            APIHelper _helper = new APIHelper();
            List<Dictionary<string, object>> dictMLCReferencesList = new List<Dictionary<string, object>>();
            Dictionary<string, string> dicMetadata = new Dictionary<string, string>();
            try
            {
                dicMetadata["TableName"] = strTableName;
                dictMLCReferencesList = _helper.PostAsyncListAPICall(AppStatic.APICloudIAF, AppStatic.API_Action_GetReferencesDataFromCloud, dicMetadata);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetReferencesDataFromCloud " + ex.Message);
                return null;
            }

            return dictMLCReferencesList;
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public List<Dictionary<string, object>> GetReferencesDataFromLocal(string strTableName)
        {
            List<Dictionary<string, object>> ListFromLocal = new List<Dictionary<string, object>>();
            try
            {
                string strSQLQuery = "SELECT [Id], [ModifiedDateTime] FROM ";
                switch (strTableName.ToLower())
                {
                    case "mlc":
                        strSQLQuery += AppStatic.MLCRegulationTree;
                        break;
                    case "sms":
                        strSQLQuery += AppStatic.SMSReferencesTree;
                        break;
                    case "ssp":
                        strSQLQuery += AppStatic.SSPReferenceTree;
                        break;
                    default:
                        break;
                }

                ListFromLocal = GetReferencesDataListBySQLQuery(strSQLQuery);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetReferencesDataFromLocal " + ex.Message);
            }
            return ListFromLocal;
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public void SendReferencesDataToLocal(List<Dictionary<string, object>> dictCloudReferencesList, string strTableName)
        {
            List<Dictionary<string, object>> lstLocalReferencesData = GetReferencesDataFromLocal(strTableName);
            foreach (var item in dictCloudReferencesList)
            {
                APIResponse res = InsertOrUpdateReferencesDataInLocalDB(lstLocalReferencesData, item, strTableName);
            }
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public APIResponse InsertOrUpdateReferencesDataInLocalDB(List<Dictionary<string, object>> dictLocalReferencesList, Dictionary<string, object> dictCloudItem, string strTableName)
        {
            APIResponse response = new APIResponse();
            bool IsExistingReference = false;
            bool IsNeedToPerformInsertOrUpdate = false;
            string strId = string.Empty;
            DateTime dtLocalItemModifiedDateTime = new DateTime();
            DateTime dtCloudItemModifiedDateTime = new DateTime();
            try
            {
                if (dictCloudItem.ContainsKey("Id"))
                    strId = Convert.ToString(dictCloudItem["Id"]);

                if (dictCloudItem.ContainsKey("ModifiedDateTime"))
                    dtCloudItemModifiedDateTime = Convert.ToDateTime(dictCloudItem["ModifiedDateTime"]);

                if (dictLocalReferencesList != null)
                {
                    foreach (var item in dictLocalReferencesList)
                    {
                        if (Convert.ToString(item["Id"]) == strId)
                        {
                            dtLocalItemModifiedDateTime = Convert.ToDateTime(item["ModifiedDateTime"]);
                            IsExistingReference = true;
                            break;
                        }
                    }
                }

                if (IsExistingReference)
                {
                    // if Server copy latest then it will update
                    if ((dtCloudItemModifiedDateTime > dtLocalItemModifiedDateTime))
                    {
                        // Update
                        IsNeedToPerformInsertOrUpdate = true;
                    }
                }
                else
                {
                    // Insert
                    IsNeedToPerformInsertOrUpdate = true;
                }

                if (IsNeedToPerformInsertOrUpdate)
                {
                    switch (strTableName.ToLower())
                    {
                        case "mlc":
                            InsertOrUpdateMLCRegulationTree(dictCloudItem);
                            break;
                        case "sms":
                            InsertOrUpdateSMSReferencesTree(dictCloudItem);
                            break;
                        case "ssp":
                            InsertOrUpdateSSPReferencesTree(dictCloudItem);
                            break;
                        default:
                            break;
                    }
                }

                response.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                response.result = AppStatic.ERROR;
                LogHelper.writelog("InsertOrUpdateReferencesDataInLocalDB Error : " + ex.Message.ToString());
            }
            return response;
        }
        // End JSL 05/20/2022

        // JSL 05/20/2022
        public static List<Dictionary<string, object>> GetReferencesDataListBySQLQuery(string strSQLQuery)
        {
            List<Dictionary<string, object>> lstDict = new List<Dictionary<string, object>>();
            try
            {

                string sql = strSQLQuery;
                SqlParameter[] sqlParam = new SqlParameter[] { };
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

        // JSL 05/20/2022 
        public static List<Dictionary<string, object>> InsertOrUpdateSMSReferencesTree(Dictionary<string, object> dictMaster)
        {
            List<Dictionary<string, object>> lstDict = new List<Dictionary<string, object>>();
            try
            {
                string sql = "CB_proc_SMSReferencesTree_InsertOrUpdate";
                SqlParameter[] sqlParam = new SqlParameter[] {
                    new SqlParameter("@Id", dictMaster["Id"]),
                    new SqlParameter("@SMSReferenceId", dictMaster["SMSReferenceId"]),
                    new SqlParameter("@SMSReferenceParentId", dictMaster["SMSReferenceParentId"] ?? DBNull.Value),
                    new SqlParameter("@Number", dictMaster["Number"]),
                    new SqlParameter("@Reference", dictMaster["Reference"]),
                    new SqlParameter("@IsDeleted", dictMaster["IsDeleted"]),
                    new SqlParameter("@CreatedDateTime", dictMaster["CreatedDateTime"]),
                    new SqlParameter("@ModifiedDateTime", dictMaster["ModifiedDateTime"])
                };
                DataSet ds = CommonAssetsDB.RecordDataSet(sql, sqlParam, CommandType.StoredProcedure);
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

        // JSL 05/20/2022 
        public static List<Dictionary<string, object>> InsertOrUpdateSSPReferencesTree(Dictionary<string, object> dictMaster)
        {
            List<Dictionary<string, object>> lstDict = new List<Dictionary<string, object>>();
            try
            {
                string sql = "CB_proc_SSPReferenceTree_InsertOrUpdate";
                SqlParameter[] sqlParam = new SqlParameter[] {
                    new SqlParameter("@Id", dictMaster["Id"]),
                    new SqlParameter("@SSPReferenceId", dictMaster["SSPReferenceId"]),
                    new SqlParameter("@SSPReferenceParentId", dictMaster["SSPReferenceParentId"] ?? DBNull.Value),
                    new SqlParameter("@Number", dictMaster["Number"]),
                    new SqlParameter("@Reference", dictMaster["Reference"]),
                    new SqlParameter("@IsDeleted", dictMaster["IsDeleted"]),
                    new SqlParameter("@CreatedDateTime", dictMaster["CreatedDateTime"]),
                    new SqlParameter("@ModifiedDateTime", dictMaster["ModifiedDateTime"])
                };
                DataSet ds = CommonAssetsDB.RecordDataSet(sql, sqlParam, CommandType.StoredProcedure);
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

        // JSL 05/20/2022
        public static List<Dictionary<string, object>> InsertOrUpdateMLCRegulationTree(Dictionary<string, object> dictMaster)
        {
            List<Dictionary<string, object>> lstDict = new List<Dictionary<string, object>>();
            try
            {
                string sql = "CB_proc_MLCRegulationTree_InsertOrUpdate";
                SqlParameter[] sqlParam = new SqlParameter[] {
                    new SqlParameter("@Id", dictMaster["Id"]),
                    new SqlParameter("@MLCRegulationId", dictMaster["MLCRegulationId"]),
                    new SqlParameter("@MLCRegulationParentId", dictMaster["MLCRegulationParentId"] ?? DBNull.Value),
                    new SqlParameter("@Number", dictMaster["Number"]),
                    new SqlParameter("@Regulation", dictMaster["Regulation"]),
                    new SqlParameter("@IsDeleted", dictMaster["IsDeleted"]),
                    new SqlParameter("@CreatedDateTime", dictMaster["CreatedDateTime"]),
                    new SqlParameter("@ModifiedDateTime", dictMaster["ModifiedDateTime"])
                };
                DataSet ds = CommonAssetsDB.RecordDataSet(sql, sqlParam, CommandType.StoredProcedure);
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
        // End JSL 05/20/2022

        #region Common Function
        // RDBJ 01/21/2022
        public static void IfConnectionOpenThenCloseIt(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        // End RDBJ 01/21/2022

        #endregion
    }
}
