using CarisbrookeShippingService.BLL.Modals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class SIRFormDataHelper
    {
        #region Local to Cloud Server        
        public void StartSIRSync()
        {
            List<SIRModal> UnSyncList = GetSIRFormsUnsyncedData();
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for SIR Data is about " + UnSyncList.Count + "");
                List<string> SuccessIds = SendSIRDataToRemote(UnSyncList);  //RDBJ 09/25/2021 Change long to string
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    UpdateLocalSIRFormsStatus(SuccessIds);

                    if (SuccessIds.Count == UnSyncList.Count)
                        LogHelper.writelog("SIR Data Sync process done.");
                    if (SuccessIds.Count < UnSyncList.Count)
                        LogHelper.writelog("Some SIR Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("SIR Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("SIR Data Synced already done.");
            }

            //RDBJ 09/25/2021 Commented This logic get AWS Copy if throw error and updated in LocalDB and Lost the last LocalDB form copy
            /*
            LogHelper.writelog("SIR Data Synced start from server to local.");
            StartSIRSyncCloudTOLocal();
            LogHelper.writelog("SIR Data Synced done from server to local.");
            */
        }
        public List<SIRModal> GetSIRFormsUnsyncedData()
        {
            List<SIRModal> SyncList = new List<SIRModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SuperintendedInspectionReport + " WHERE ISNULL(IsSynced,0) = 0 AND [UniqueFormID] IS NOT NULL", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<SuperintendedInspectionReport> SIRSyncList = dt.ToListof<SuperintendedInspectionReport>();
                            foreach (var item in SIRSyncList)
                            {
                                // RDBJ 01/05/2022 wrapped in if
                                if (item.isDelete == 0)
                                {
                                    try
                                    {
                                        SIRModal Modal = new SIRModal();

                                        Modal.SuperintendedInspectionReport = item;

                                        //RDBJ 09/28/2021
                                        DataTable dtNotes = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SIRNotes + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                        sqlAdp.Fill(dtNotes);
                                        Modal.SIRNote = dtNotes.ToListof<SIRNote>();
                                        //End RDBJ 09/28/2021

                                        DataTable dtAdditionalNotes = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SIRAdditionalNotes + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                        sqlAdp.Fill(dtAdditionalNotes);
                                        Modal.SIRAdditionalNote = dtAdditionalNotes.ToListof<SIRAdditionalNote>();

                                        DataTable dtDeficiencies = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE UniqueFormID = '" + item.UniqueFormID + "' AND ReportType = 'SI'", conn);
                                        sqlAdp.Fill(dtDeficiencies);
                                        Modal.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>();

                                        if (Modal.GIRDeficiencies != null && Modal.GIRDeficiencies.Count > 0)
                                        {
                                            foreach (var def in Modal.GIRDeficiencies)
                                            {
                                                // RDBJ 12/23/2021 wrapped in if
                                                if (def.isDelete == 0)
                                                {
                                                    DataTable dtDeficienciesFiles = new DataTable();
                                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesFile + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                                    sqlAdp.Fill(dtDeficienciesFiles);
                                                    def.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                                    if (def.GIRDeficienciesFile != null && def.GIRDeficienciesFile.Count > 0)
                                                    {
                                                        foreach (var deffile in def.GIRDeficienciesFile)
                                                        {
                                                            // JSL 11/13/2022
                                                            if (!deffile.StorePath.StartsWith("data:"))
                                                            {
                                                                deffile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(deffile.StorePath);
                                                            }
                                                            // End JSL 11/13/2022
                                                            deffile.IsUpload = "true";
                                                        }
                                                    }
                                                    DataTable dtDeficienciesComments = new DataTable();
                                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.DeficienciesNote + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                                    sqlAdp.Fill(dtDeficienciesComments);
                                                    def.GIRDeficienciesComments = dtDeficienciesComments.ToListof<GIRDeficienciesNote>();
                                                    if (def.GIRDeficienciesComments != null && def.GIRDeficienciesComments.Count > 0)
                                                    {
                                                        foreach (var defComment in def.GIRDeficienciesComments)
                                                        {
                                                            DataTable dtDeficienciesCommentsFiles = new DataTable();
                                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesCommentFile + " WHERE NoteUniqueID = '" + defComment.NoteUniqueID + "'", conn);
                                                            sqlAdp.Fill(dtDeficienciesCommentsFiles);
                                                            defComment.GIRDeficienciesCommentFile = dtDeficienciesCommentsFiles.ToListof<GIRDeficienciesCommentFile>();
                                                            // JSL 11/13/2022
                                                            if (defComment.GIRDeficienciesCommentFile != null && defComment.GIRDeficienciesCommentFile.Count > 0)
                                                            {
                                                                foreach (var itemFile in defComment.GIRDeficienciesCommentFile)
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

                                                    DataTable dtDeficienciesInitialAction = new DataTable();
                                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesInitialActions + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                                    sqlAdp.Fill(dtDeficienciesInitialAction);
                                                    def.GIRDeficienciesInitialActions = dtDeficienciesInitialAction.ToListof<GIRDeficienciesInitialActions>();
                                                    if (def.GIRDeficienciesInitialActions != null && def.GIRDeficienciesInitialActions.Count > 0)
                                                    {
                                                        foreach (var defInitial in def.GIRDeficienciesInitialActions)
                                                        {
                                                            DataTable dtDeficienciesInitActionFiles = new DataTable();
                                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesInitialActionsFile + " WHERE IniActUniqueID = '" + defInitial.IniActUniqueID + "'", conn);
                                                            sqlAdp.Fill(dtDeficienciesInitActionFiles);
                                                            defInitial.GIRDeficienciesInitialActionsFiles = dtDeficienciesInitActionFiles.ToListof<GIRDeficienciesInitialActionsFile>();
                                                            // JSL 11/13/2022
                                                            if (defInitial.GIRDeficienciesInitialActionsFiles != null && defInitial.GIRDeficienciesInitialActionsFiles.Count > 0)
                                                            {
                                                                foreach (var itemFile in defInitial.GIRDeficienciesInitialActionsFiles)
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

                                                    DataTable dtDeficienciesResolution = new DataTable();
                                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesResolution + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                                    sqlAdp.Fill(dtDeficienciesResolution);
                                                    def.GIRDeficienciesResolution = dtDeficienciesResolution.ToListof<GIRDeficienciesResolution>();
                                                    if (def.GIRDeficienciesResolution != null && def.GIRDeficienciesResolution.Count > 0)
                                                    {
                                                        foreach (var defResolution in def.GIRDeficienciesResolution)
                                                        {
                                                            DataTable dtDeficienciesResolutionFile = new DataTable();
                                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesResolutionFile + " WHERE ResolutionUniqueID = '" + defResolution.ResolutionUniqueID + "'", conn);
                                                            sqlAdp.Fill(dtDeficienciesResolutionFile);
                                                            defResolution.GIRDeficienciesResolutionFiles = dtDeficienciesResolutionFile.ToListof<GIRDeficienciesResolutionFile>();
                                                            // JSL 11/12/2022
                                                            if (defResolution.GIRDeficienciesResolutionFiles != null && defResolution.GIRDeficienciesResolutionFiles.Count > 0)
                                                            {
                                                                foreach (var itemFile in defResolution.GIRDeficienciesResolutionFiles)
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
                                            }
                                        }
                                        SyncList.Add(Modal);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.writelog("GetSIRFormsUnsyncedData Error : UniqueFormID : " + item.UniqueFormID + " " + ex.Message);
                                    }
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<string> SendSIRDataToRemote(List<SIRModal> UnSyncList) //RDBJ 09/25/2021 Change long to string return and removed unwanted code from here
        {
            // JSL 07/16/2022
            APIResponse res = new APIResponse();
            APIHelper _apiHelper = new APIHelper();
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            // End JSL 07/16/2022

            List<string> SuccessIds = new List<string>();
            foreach (var item in UnSyncList)
            {
                if (item.SuperintendedInspectionReport != null && item.SuperintendedInspectionReport.UniqueFormID != null)
                {
                    item.SuperintendedInspectionReport.SIRFormID = 0;
                    CloudLocalSIRSynch _helper = new CloudLocalSIRSynch(); //RDBJ 09/25/2021 Use CloudLocalSIRSynch rather than APIHelper
                    //res = _helper.sendSIRLocalToRemote(item);   //RDBJ 09/25/2021 Use sendSIRLocalToRemote rather than SubmitSIR

                    // JSL 07/18/2022 commented 
                    // JSL 07/16/2022
                    bool IsAllowToSendDataForServer = false;
                    Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                    dictMetaData["FormType"] = AppStatic.SIRForm;
                    dictMetaData["FormUniqueID"] = item.SuperintendedInspectionReport.UniqueFormID.ToString();
                    dictMetaData["FormVersion"] = item.SuperintendedInspectionReport.FormVersion.ToString();
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
                                SIRModal modalSIRToSendServer = new SIRModal();
                                modalSIRToSendServer.SuperintendedInspectionReport = item.SuperintendedInspectionReport;
                                modalSIRToSendServer.SIRNote = item.SIRNote;
                                modalSIRToSendServer.SIRAdditionalNote = item.SIRAdditionalNote;
                                modalSIRToSendServer.GIRDeficiencies = new List<GIRDeficiencies>();
                                
                                // JSL 10/28/2022 reorder conditions
                                res = new APIResponse();
                                foreach (var itemDeficiencies in item.GIRDeficiencies)
                                {
                                    dictMetaData["ShipCode"] = itemDeficiencies.Ship;
                                    dictMetaData["DeficienciesData"] = JsonConvert.SerializeObject(itemDeficiencies);
                                    dictMetaData["strAction"] = AppStatic.API_METHOD_InsertOrUpdateDeficienciesData;

                                    retDictMetaData = _apiHelper.PostAsyncAPICall(AppStatic.APICloudSIR, AppStatic.API_CommonPostAPICall, dictMetaData);
                                    if (retDictMetaData != null)
                                    {
                                        if (retDictMetaData["Status"] == AppStatic.ERROR)
                                        {
                                            LogHelper.writelog("SIR Data Synced Error for Deficienciy : " + itemDeficiencies.DeficienciesUniqueID.ToString());
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
                                    res = _helper.sendSIRLocalToRemote(modalSIRToSendServer);
                                }
                                // End JSL 10/28/2022 reorder conditions
                            }
                        }
                    }
                    // End JSL 07/16/2022
                    // JSL 07/18/2022 commented 

                    if (res != null && res.result == AppStatic.SUCCESS)
                    {
                        SuccessIds.Add(Convert.ToString(item.SuperintendedInspectionReport.UniqueFormID));
                    }
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalSIRFormsStatus(List<string> SuccessIds) //RDBJ 09/25/2021 Change List<long> to List<string>
        {
            try
            {
                // JSL 07/16/2022 wrapped in if
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    string IdsStr = string.Join(",", SuccessIds.Select(x => string.Format("'{0}'", x)));
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            string Query = "UPDATE " + AppStatic.SuperintendedInspectionReport + " SET IsSynced = 1 WHERE UniqueFormID in (" + IdsStr + ")"; //RDBJ 09/25/2021 changed UniqueFormID from SIRFormID
                            SqlCommand cmd = new SqlCommand(Query, conn);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                // End JSL 07/16/2022 wrapped in if
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalSIRFormsStatus : " + ex.Message);
            }
        } //RDBJ 09/25/2021 Change List<long> to List<string>
        #endregion

        #region Cloud Server to Local
        public void StartSIRSyncCloudTOLocal(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<SIRModal> UnSyncList = GetSIRFormsUnsyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine  // JSL 11/12/2022
                );
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for SIR Data is about " + UnSyncList.Count + "");
                List<string> SuccessIds = SendSIRDataToLocal(UnSyncList);
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                {
                    UpdateCloudSIRFormsStatus(SuccessIds);
                    LogHelper.writelog("SIR Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                {
                    //UpdateLocalGIRFormsStatus(SuccessIds);
                    LogHelper.writelog("Some SIR Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("SIR Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("SIR Data Synced already done.");
            }
        }
        public List<SIRModal> GetSIRFormsUnsyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<SIRModal> UnSyncListFromCloud = new List<SIRModal>();
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
                //UnSyncListFromCloud = _helper.GetSIRFormsUnsyncedDataFromCloud();
                UnSyncListFromCloud = _helper.GetSIRGeneralDescription(strShipCode);    // JSL 11/12/2022 added strShipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRFormsUnsyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return UnSyncListFromCloud;
        }
        public List<string> SendSIRDataToLocal(List<SIRModal> UnSyncList)
        {
            List<string> SuccessIds = new List<string>();
            foreach (var item in UnSyncList)
            {
                string localDBGIRUniqueFormID = Convert.ToString(item.SuperintendedInspectionReport.UniqueFormID);
                item.SuperintendedInspectionReport.SIRFormID = 0;
                APIResponse res = SaveSIRDataInLocalDB(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBGIRUniqueFormID);
                }
            }
            return SuccessIds;
        }
        public APIResponse SaveSIRDataInLocalDB(SIRModal ModalSIR)
        {
            APIResponse res = new APIResponse();
            string connectionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connectionString);
            var Modal = ModalSIR.SuperintendedInspectionReport;
            string UniqueFormID = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID FROM " + AppStatic.SuperintendedInspectionReport + " WHERE UniqueFormID = '" + Modal.UniqueFormID + "'", connection);
                sqlAdp.Fill(dt);

                Modal.IsSynced = true;
                UniqueFormID = Convert.ToString(Modal.UniqueFormID);
                if (dt.Rows.Count > 0)
                {
                    var girLocalList = dt.ToListof<SuperintendedInspectionReport>().FirstOrDefault();
                    if (!girLocalList.FormVersion.HasValue || Modal.FormVersion > girLocalList.FormVersion)
                    {
                        string UpdateQury = GetSIRUpdateQuery();
                        SqlCommand command = new SqlCommand(UpdateQury, connection);

                        SIRUpdateCMD(Modal, ref command);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    string InsertQury = SIRDataInsertQuery();
                    SqlCommand command = new SqlCommand(InsertQury, connection);

                    SIRDataInsertCMD(Modal, ref command);

                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }

                // RDBJ 12/18/2021 set common 
                if (!string.IsNullOrEmpty(UniqueFormID))
                {
                    GIRFormDataHelper _girHElper = new GIRFormDataHelper();
                    _girHElper.GIRDeficiencies_Save(Guid.Parse(UniqueFormID), ModalSIR.GIRDeficiencies
                        , (bool)Modal.SavedAsDraft  // JSL 05/23/2022
                        );

                    // RDBJ 04/02/2022 commented
                    /*
                    SaveSIRNotesDataInLocalDB(ModalSIR.SIRNote, Guid.Parse(UniqueFormID)); // RDBJ 12/18/2021
                    SaveSIRAdditionalNotesDataInLocalDB(ModalSIR.SIRAdditionalNote, Guid.Parse(UniqueFormID)); // RDBJ 12/18/2021
                    */
                    // End RDBJ 04/02/2022 commented

                    SIRNotesInsertOrUpdate(ModalSIR.SIRNote, Guid.Parse(UniqueFormID)); // RDBJ 04/02/2022
                    SIRAdditionalNotesInsertOrUpdate(ModalSIR.SIRAdditionalNote, Guid.Parse(UniqueFormID)); // RDBJ 04/02/2022
                }
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SuperintendedInspectionReport table : " + UniqueFormID + "  Inner Error : " + ex.InnerException.ToString()); // RDBJ 01/21/2022
                LogHelper.writelog("Add Local DB in SuperintendedInspectionReport table Error : " + ex.Message.ToString());
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

        #region SIR Insert/Update
        public string SIRDataInsertQuery()
        {
            // RDBJ 02/15/2022 Added Section9_16_Condition, Section9_16_Comment, Section9_17_Condition, Section9_17_Comment, Section18_8_Condition, Section18_8_Comment, Section18_9_Condition, Section18_9_Comment 
            // RDBJ 01/05/2022 Added isDelete
            string InsertQury = @"INSERT INTO [dbo].[SuperintendedInspectionReport]
                                ([UniqueFormID],[FormVersion],[ShipID],[ShipName],[Date],[Port],[Master],[Superintended],[Section1_1_Condition],[Section1_1_Comment],[Section1_2_Condition]
                                ,[Section1_2_Comment],[Section1_3_Condition],[Section1_3_Comment],[Section1_4_Condition],[Section1_4_Comment],[Section1_5_Condition]
                                ,[Section1_5_Comment],[Section1_6_Condition],[Section1_6_Comment],[Section1_7_Condition],[Section1_7_Comment],[Section1_8_Condition]
                                ,[Section1_8_Comment],[Section1_9_Condition],[Section1_9_Comment],[Section1_10_Condition],[Section1_10_Comment],[Section1_11_Condition]
                                ,[Section1_11_Comment],[Section2_1_Condition],[Section2_1_Comment],[Section2_2_Condition],[Section2_2_Comment],[Section2_3_Condition]
                                ,[Section2_3_Comment],[Section2_4_Condition],[Section2_4_Comment],[Section2_5_Condition],[Section2_5_Comment],[Section2_6_Condition]
                                ,[Section2_6_Comment],[Section2_7_Condition],[Section2_7_Comment],[Section3_1_Condition],[Section3_1_Comment],[Section3_2_Condition]
                                ,[Section3_2_Comment],[Section3_3_Condition],[Section3_3_Comment],[Section3_4_Condition],[Section3_4_Comment],[Section3_5_Condition]
                                ,[Section3_5_Comment],[Section4_1_Condition],[Section4_1_Comment],[Section4_2_Condition],[Section4_2_Comment],[Section4_3_Condition]
                                ,[Section4_3_Comment],[Section5_1_Condition],[Section5_1_Comment],[Section5_6_Condition],[Section5_6_Comment],[Section5_8_Condition]
                                ,[Section5_8_Comment],[Section5_9_Condition],[Section5_9_Comment],[Section6_1_Condition],[Section6_1_Comment],[Section6_2_Condition]
                                ,[Section6_2_Comment],[Section6_3_Condition],[Section6_3_Comment],[Section6_4_Condition],[Section6_4_Comment],[Section6_5_Condition]
                                ,[Section6_5_Comment],[Section6_6_Condition],[Section6_6_Comment],[Section6_7_Condition],[Section6_7_Comment],[Section6_8_Condition]
                                ,[Section6_8_Comment],[Section7_1_Condition],[Section7_1_Comment],[Section7_2_Condition],[Section7_2_Comment],[Section7_3_Condition]
                                ,[Section7_3_Comment],[Section7_4_Condition],[Section7_4_Comment],[Section7_5_Condition],[Section7_5_Comment],[Section7_6_Condition]
                                ,[Section7_6_Comment],[Section8_1_Condition],[Section8_1_Comment],[Section8_2_Condition],[Section8_2_Comment],[Section8_3_Condition]
                                ,[Section8_3_Comment],[Section8_4_Condition],[Section8_4_Comment],[Section8_5_Condition],[Section8_5_Comment],[Section8_6_Condition]
                                ,[Section8_6_Comment],[Section8_7_Condition],[Section8_7_Comment],[Section8_8_Condition],[Section8_8_Comment],[Section8_9_Condition]
                                ,[Section8_9_Comment],[Section8_10_Condition],[Section8_10_Comment],[Section8_11_Condition],[Section8_11_Comment],[Section8_12_Condition]
                                ,[Section8_12_Comment],[Section8_13_Condition],[Section8_13_Comment],[Section8_14_Condition],[Section8_14_Comment],[Section8_15_Condition]
                                ,[Section8_15_Comment],[Section8_16_Condition],[Section8_16_Comment],[Section8_17_Condition],[Section8_17_Comment],[Section8_18_Condition]
                                ,[Section8_18_Comment],[Section8_19_Condition],[Section8_19_Comment],[Section8_20_Condition],[Section8_20_Comment],[Section8_21_Condition]
                                ,[Section8_21_Comment],[Section8_22_Condition],[Section8_22_Comment],[Section8_23_Condition],[Section8_23_Comment],[Section8_24_Condition]
                                ,[Section8_24_Comment],[Section8_25_Condition],[Section8_25_Comment],[Section9_1_Condition],[Section9_1_Comment],[Section9_2_Condition]
                                ,[Section9_2_Comment],[Section9_3_Condition],[Section9_3_Comment],[Section9_4_Condition],[Section9_4_Comment],[Section9_5_Condition]
                                ,[Section9_5_Comment],[Section9_6_Condition],[Section9_6_Comment],[Section9_7_Condition],[Section9_7_Comment],[Section9_8_Condition]
                                ,[Section9_8_Comment],[Section9_9_Condition],[Section9_9_Comment],[Section9_10_Condition],[Section9_10_Comment],[Section9_11_Condition]
                                ,[Section9_11_Comment],[Section9_12_Condition],[Section9_12_Comment],[Section9_13_Condition],[Section9_13_Comment],[Section9_14_Condition]
                                ,[Section9_14_Comment],[Section9_15_Condition],[Section9_15_Comment]
                                ,[Section9_16_Condition],[Section9_16_Comment],[Section9_17_Condition],[Section9_17_Comment]
                                ,[Section10_1_Condition],[Section10_1_Comment],[Section10_2_Condition]
                                ,[Section10_2_Comment],[Section10_3_Condition],[Section10_3_Comment],[Section10_4_Condition],[Section10_4_Comment],[Section10_5_Condition]
                                ,[Section10_5_Comment],[Section10_6_Condition],[Section10_6_Comment],[Section10_7_Condition],[Section10_7_Comment],[Section10_8_Condition]
                                ,[Section10_8_Comment],[Section10_9_Condition],[Section10_9_Comment],[Section10_10_Condition],[Section10_10_Comment],[Section10_11_Condition]
                                ,[Section10_11_Comment],[Section10_12_Condition],[Section10_12_Comment],[Section10_13_Condition],[Section10_13_Comment],[Section10_14_Condition]
                                ,[Section10_14_Comment],[Section10_15_Condition],[Section10_15_Comment],[Section10_16_Condition],[Section10_16_Comment],[Section11_1_Condition]
                                ,[Section11_1_Comment],[Section11_2_Condition],[Section11_2_Comment],[Section11_3_Condition],[Section11_3_Comment],[Section11_4_Condition]
                                ,[Section11_4_Comment],[Section11_5_Condition],[Section11_5_Comment],[Section11_6_Condition],[Section11_6_Comment],[Section11_7_Condition]
                                ,[Section11_7_Comment],[Section11_8_Condition],[Section11_8_Comment],[Section12_1_Condition],[Section12_1_Comment],[Section12_2_Condition]
                                ,[Section12_2_Comment],[Section12_3_Condition],[Section12_3_Comment],[Section12_4_Condition],[Section12_4_Comment],[Section12_5_Condition]
                                ,[Section12_5_Comment],[Section12_6_Condition],[Section12_6_Comment],[Section13_1_Condition],[Section13_1_Comment],[Section13_2_Condition]
                                ,[Section13_2_Comment],[Section13_3_Condition],[Section13_3_Comment],[Section13_4_Condition],[Section13_4_Comment],[Section14_1_Condition]
                                ,[Section14_1_Comment],[Section14_2_Condition],[Section14_2_Comment],[Section14_3_Condition],[Section14_3_Comment],[Section14_4_Condition]
                                ,[Section14_4_Comment],[Section14_5_Condition],[Section14_5_Comment],[Section14_6_Condition],[Section14_6_Comment],[Section14_7_Condition]
                                ,[Section14_7_Comment],[Section14_8_Condition],[Section14_8_Comment],[Section14_9_Condition],[Section14_9_Comment],[Section14_10_Condition]
                                ,[Section14_10_Comment],[Section14_11_Condition],[Section14_11_Comment],[Section14_12_Condition],[Section14_12_Comment],[Section14_13_Condition]
                                ,[Section14_13_Comment],[Section14_14_Condition],[Section14_14_Comment],[Section14_15_Condition],[Section14_15_Comment],[Section14_16_Condition]
                                ,[Section14_16_Comment],[Section14_17_Condition],[Section14_17_Comment],[Section14_18_Condition],[Section14_18_Comment],[Section14_19_Condition]
                                ,[Section14_19_Comment],[Section14_20_Condition],[Section14_20_Comment],[Section14_21_Condition],[Section14_21_Comment],[Section14_22_Condition]
                                ,[Section14_22_Comment],[Section14_23_Condition],[Section14_23_Comment],[Section14_24_Condition],[Section14_24_Comment],[Section14_25_Condition]
                                ,[Section14_25_Comment],[Section15_1_Condition],[Section15_1_Comment],[Section15_2_Condition],[Section15_2_Comment],[Section15_3_Condition],[Section15_3_Comment]
                                ,[Section15_4_Condition],[Section15_4_Comment],[Section15_5_Condition],[Section15_5_Comment],[Section15_6_Condition],[Section15_6_Comment],[Section15_7_Condition]
                                ,[Section15_7_Comment],[Section15_8_Condition],[Section15_8_Comment],[Section15_9_Condition],[Section15_9_Comment],[Section15_10_Condition],[Section15_10_Comment]
                                ,[Section15_11_Condition],[Section15_11_Comment],[Section15_12_Condition],[Section15_12_Comment],[Section15_13_Condition],[Section15_13_Comment],[Section15_14_Condition]
                                ,[Section15_14_Comment],[Section15_15_Condition],[Section15_15_Comment],[Section16_1_Condition],[Section16_1_Comment],[Section16_2_Condition],[Section16_2_Comment]
                                ,[Section16_3_Condition],[Section16_3_Comment],[Section16_4_Condition],[Section16_4_Comment],[Section17_1_Condition],[Section17_1_Comment],[Section17_2_Condition]
                                ,[Section17_2_Comment],[Section17_3_Condition],[Section17_3_Comment],[Section17_4_Condition],[Section17_4_Comment],[Section17_5_Condition],[Section17_5_Comment]
                                ,[Section17_6_Condition],[Section17_6_Comment],[Section18_1_Condition],[Section18_1_Comment],[Section18_2_Condition],[Section18_2_Comment],[Section18_3_Condition]
                                ,[Section18_4_Comment],[Section18_5_Condition],[Section18_5_Comment],[Section18_6_Condition],[Section18_6_Comment],[Section18_7_Condition],[Section18_7_Comment]
                                ,[Section18_8_Condition],[Section18_8_Comment],[Section18_9_Condition],[Section18_9_Comment]      
                                ,[IsSynced],[CreatedDate],[ModifyDate],[SavedAsDraft], [isDelete])  OUTPUT INSERTED.SIRFormID VALUES
                                (@UniqueFormID,@FormVersion,@ShipID,@ShipName,@Date,@Port,@Master,@Superintended,@Section1_1_Condition,@Section1_1_Comment,@Section1_2_Condition
                                ,@Section1_2_Comment,@Section1_3_Condition,@Section1_3_Comment,@Section1_4_Condition,@Section1_4_Comment,@Section1_5_Condition
                                ,@Section1_5_Comment,@Section1_6_Condition,@Section1_6_Comment,@Section1_7_Condition,@Section1_7_Comment,@Section1_8_Condition
                                ,@Section1_8_Comment,@Section1_9_Condition,@Section1_9_Comment,@Section1_10_Condition,@Section1_10_Comment,@Section1_11_Condition
                                ,@Section1_11_Comment,@Section2_1_Condition,@Section2_1_Comment,@Section2_2_Condition,@Section2_2_Comment,@Section2_3_Condition
                                ,@Section2_3_Comment,@Section2_4_Condition,@Section2_4_Comment,@Section2_5_Condition,@Section2_5_Comment,@Section2_6_Condition
                                ,@Section2_6_Comment,@Section2_7_Condition,@Section2_7_Comment,@Section3_1_Condition,@Section3_1_Comment,@Section3_2_Condition
                                ,@Section3_2_Comment,@Section3_3_Condition,@Section3_3_Comment,@Section3_4_Condition,@Section3_4_Comment,@Section3_5_Condition
                                ,@Section3_5_Comment,@Section4_1_Condition,@Section4_1_Comment,@Section4_2_Condition,@Section4_2_Comment,@Section4_3_Condition
                                ,@Section4_3_Comment,@Section5_1_Condition,@Section5_1_Comment,@Section5_6_Condition,@Section5_6_Comment,@Section5_8_Condition
                                ,@Section5_8_Comment,@Section5_9_Condition,@Section5_9_Comment,@Section6_1_Condition,@Section6_1_Comment,@Section6_2_Condition
                                ,@Section6_2_Comment,@Section6_3_Condition,@Section6_3_Comment,@Section6_4_Condition,@Section6_4_Comment,@Section6_5_Condition
                                ,@Section6_5_Comment,@Section6_6_Condition,@Section6_6_Comment,@Section6_7_Condition,@Section6_7_Comment,@Section6_8_Condition
                                ,@Section6_8_Comment,@Section7_1_Condition,@Section7_1_Comment,@Section7_2_Condition,@Section7_2_Comment,@Section7_3_Condition
                                ,@Section7_3_Comment,@Section7_4_Condition,@Section7_4_Comment,@Section7_5_Condition,@Section7_5_Comment,@Section7_6_Condition
                                ,@Section7_6_Comment,@Section8_1_Condition,@Section8_1_Comment,@Section8_2_Condition,@Section8_2_Comment,@Section8_3_Condition
                                ,@Section8_3_Comment,@Section8_4_Condition,@Section8_4_Comment,@Section8_5_Condition,@Section8_5_Comment,@Section8_6_Condition
                                ,@Section8_6_Comment,@Section8_7_Condition,@Section8_7_Comment,@Section8_8_Condition,@Section8_8_Comment,@Section8_9_Condition
                                ,@Section8_9_Comment,@Section8_10_Condition,@Section8_10_Comment,@Section8_11_Condition,@Section8_11_Comment,@Section8_12_Condition
                                ,@Section8_12_Comment,@Section8_13_Condition,@Section8_13_Comment,@Section8_14_Condition,@Section8_14_Comment,@Section8_15_Condition
                                ,@Section8_15_Comment,@Section8_16_Condition,@Section8_16_Comment,@Section8_17_Condition,@Section8_17_Comment,@Section8_18_Condition
                                ,@Section8_18_Comment,@Section8_19_Condition,@Section8_19_Comment,@Section8_20_Condition,@Section8_20_Comment,@Section8_21_Condition
                                ,@Section8_21_Comment,@Section8_22_Condition,@Section8_22_Comment,@Section8_23_Condition,@Section8_23_Comment,@Section8_24_Condition
                                ,@Section8_24_Comment,@Section8_25_Condition,@Section8_25_Comment,@Section9_1_Condition,@Section9_1_Comment,@Section9_2_Condition
                                ,@Section9_2_Comment,@Section9_3_Condition,@Section9_3_Comment,@Section9_4_Condition,@Section9_4_Comment,@Section9_5_Condition
                                ,@Section9_5_Comment,@Section9_6_Condition,@Section9_6_Comment,@Section9_7_Condition,@Section9_7_Comment,@Section9_8_Condition
                                ,@Section9_8_Comment,@Section9_9_Condition,@Section9_9_Comment,@Section9_10_Condition,@Section9_10_Comment,@Section9_11_Condition
                                ,@Section9_11_Comment,@Section9_12_Condition,@Section9_12_Comment,@Section9_13_Condition,@Section9_13_Comment,@Section9_14_Condition
                                ,@Section9_14_Comment,@Section9_15_Condition,@Section9_15_Comment
                                ,@Section9_16_Condition,@Section9_16_Comment,@Section9_17_Condition,@Section9_17_Comment
                                ,@Section10_1_Condition,@Section10_1_Comment,@Section10_2_Condition
                                ,@Section10_2_Comment,@Section10_3_Condition,@Section10_3_Comment,@Section10_4_Condition,@Section10_4_Comment,@Section10_5_Condition
                                ,@Section10_5_Comment,@Section10_6_Condition,@Section10_6_Comment,@Section10_7_Condition,@Section10_7_Comment,@Section10_8_Condition
                                ,@Section10_8_Comment,@Section10_9_Condition,@Section10_9_Comment,@Section10_10_Condition,@Section10_10_Comment,@Section10_11_Condition
                                ,@Section10_11_Comment,@Section10_12_Condition,@Section10_12_Comment,@Section10_13_Condition,@Section10_13_Comment,@Section10_14_Condition
                                ,@Section10_14_Comment,@Section10_15_Condition,@Section10_15_Comment,@Section10_16_Condition,@Section10_16_Comment,@Section11_1_Condition
                                ,@Section11_1_Comment,@Section11_2_Condition,@Section11_2_Comment,@Section11_3_Condition,@Section11_3_Comment,@Section11_4_Condition
                                ,@Section11_4_Comment,@Section11_5_Condition,@Section11_5_Comment,@Section11_6_Condition,@Section11_6_Comment,@Section11_7_Condition
                                ,@Section11_7_Comment,@Section11_8_Condition,@Section11_8_Comment,@Section12_1_Condition,@Section12_1_Comment,@Section12_2_Condition
                                ,@Section12_2_Comment,@Section12_3_Condition,@Section12_3_Comment,@Section12_4_Condition,@Section12_4_Comment,@Section12_5_Condition
                                ,@Section12_5_Comment,@Section12_6_Condition,@Section12_6_Comment,@Section13_1_Condition,@Section13_1_Comment,@Section13_2_Condition
                                ,@Section13_2_Comment,@Section13_3_Condition,@Section13_3_Comment,@Section13_4_Condition,@Section13_4_Comment,@Section14_1_Condition
                                ,@Section14_1_Comment,@Section14_2_Condition,@Section14_2_Comment,@Section14_3_Condition,@Section14_3_Comment,@Section14_4_Condition
                                ,@Section14_4_Comment,@Section14_5_Condition,@Section14_5_Comment,@Section14_6_Condition,@Section14_6_Comment,@Section14_7_Condition
                                ,@Section14_7_Comment,@Section14_8_Condition,@Section14_8_Comment,@Section14_9_Condition,@Section14_9_Comment,@Section14_10_Condition
                                ,@Section14_10_Comment,@Section14_11_Condition,@Section14_11_Comment,@Section14_12_Condition,@Section14_12_Comment,@Section14_13_Condition
                                ,@Section14_13_Comment,@Section14_14_Condition,@Section14_14_Comment,@Section14_15_Condition,@Section14_15_Comment,@Section14_16_Condition
                                ,@Section14_16_Comment,@Section14_17_Condition,@Section14_17_Comment,@Section14_18_Condition,@Section14_18_Comment,@Section14_19_Condition
                                ,@Section14_19_Comment,@Section14_20_Condition,@Section14_20_Comment,@Section14_21_Condition,@Section14_21_Comment,@Section14_22_Condition
                                ,@Section14_22_Comment,@Section14_23_Condition,@Section14_23_Comment,@Section14_24_Condition,@Section14_24_Comment,@Section14_25_Condition
                                ,@Section14_25_Comment,@Section15_1_Condition,@Section15_1_Comment,@Section15_2_Condition,@Section15_2_Comment,@Section15_3_Condition,@Section15_3_Comment
                                ,@Section15_4_Condition,@Section15_4_Comment,@Section15_5_Condition,@Section15_5_Comment,@Section15_6_Condition,@Section15_6_Comment,@Section15_7_Condition
                                ,@Section15_7_Comment,@Section15_8_Condition,@Section15_8_Comment,@Section15_9_Condition,@Section15_9_Comment,@Section15_10_Condition,@Section15_10_Comment
                                ,@Section15_11_Condition,@Section15_11_Comment,@Section15_12_Condition,@Section15_12_Comment,@Section15_13_Condition,@Section15_13_Comment,@Section15_14_Condition
                                ,@Section15_14_Comment,@Section15_15_Condition,@Section15_15_Comment,@Section16_1_Condition,@Section16_1_Comment,@Section16_2_Condition,@Section16_2_Comment
                                ,@Section16_3_Condition,@Section16_3_Comment,@Section16_4_Condition,@Section16_4_Comment,@Section17_1_Condition,@Section17_1_Comment,@Section17_2_Condition
                                ,@Section17_2_Comment,@Section17_3_Condition,@Section17_3_Comment,@Section17_4_Condition,@Section17_4_Comment,@Section17_5_Condition,@Section17_5_Comment
                                ,@Section17_6_Condition,@Section17_6_Comment,@Section18_1_Condition,@Section18_1_Comment,@Section18_2_Condition,@Section18_2_Comment,@Section18_3_Condition
                                ,@Section18_4_Comment,@Section18_5_Condition,@Section18_5_Comment,@Section18_6_Condition,@Section18_6_Comment,@Section18_7_Condition,@Section18_7_Comment
                                ,@Section18_8_Condition,@Section18_8_Comment,@Section18_9_Condition,@Section18_9_Comment                                
                                ,@IsSynced,@CreatedDate,@ModifyDate,@SavedAsDraft,@isDelete)";
            return InsertQury;
        }
        public void SIRDataInsertCMD(SuperintendedInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.FormVersion; // RDBJ 01/21/2022 Change data type bigint to decimal
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete == null ? DBNull.Value : (object)Modal.isDelete;  // RDBJ 01/05/2022 set default 0 // RDBJ 01/05/2022

            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date != null ? Modal.Date : null;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port == null ? string.Empty : Modal.Port; // RDBJ 01/21/2022
            command.Parameters.Add("@Master", SqlDbType.NVarChar).Value = Modal.Master != null ? Modal.Master : "";
            command.Parameters.Add("@Superintended", SqlDbType.NVarChar).Value = Modal.Superintended != null ? Modal.Superintended : "";
            command.Parameters.Add("@Section1_1_Condition", SqlDbType.NVarChar).Value = Modal.Section1_1_Condition != null ? Modal.Section1_1_Condition : "";
            command.Parameters.Add("@Section1_1_Comment", SqlDbType.NVarChar).Value = Modal.Section1_1_Comment != null ? Modal.Section1_1_Comment : "";
            command.Parameters.Add("@Section1_2_Condition", SqlDbType.NVarChar).Value = Modal.Section1_2_Condition != null ? Modal.Section1_2_Condition : "";
            command.Parameters.Add("@Section1_2_Comment", SqlDbType.NVarChar).Value = Modal.Section1_2_Comment != null ? Modal.Section1_2_Comment : "";
            command.Parameters.Add("@Section1_3_Condition", SqlDbType.NVarChar).Value = Modal.Section1_3_Condition != null ? Modal.Section1_3_Condition : "";
            command.Parameters.Add("@Section1_3_Comment", SqlDbType.NVarChar).Value = Modal.Section1_3_Comment != null ? Modal.Section1_3_Comment : "";
            command.Parameters.Add("@Section1_4_Condition", SqlDbType.NVarChar).Value = Modal.Section1_4_Condition != null ? Modal.Section1_4_Condition : "";
            command.Parameters.Add("@Section1_4_Comment", SqlDbType.NVarChar).Value = Modal.Section1_4_Comment != null ? Modal.Section1_4_Comment : "";
            command.Parameters.Add("@Section1_5_Condition", SqlDbType.NVarChar).Value = Modal.Section1_5_Condition != null ? Modal.Section1_5_Condition : "";
            command.Parameters.Add("@Section1_5_Comment", SqlDbType.NVarChar).Value = Modal.Section1_5_Comment != null ? Modal.Section1_5_Comment : "";
            command.Parameters.Add("@Section1_6_Condition", SqlDbType.NVarChar).Value = Modal.Section1_6_Condition != null ? Modal.Section1_6_Condition : "";
            command.Parameters.Add("@Section1_6_Comment", SqlDbType.NVarChar).Value = Modal.Section1_6_Comment != null ? Modal.Section1_6_Comment : "";
            command.Parameters.Add("@Section1_7_Condition", SqlDbType.NVarChar).Value = Modal.Section1_7_Condition != null ? Modal.Section1_7_Condition : "";
            command.Parameters.Add("@Section1_7_Comment", SqlDbType.NVarChar).Value = Modal.Section1_7_Comment != null ? Modal.Section1_7_Comment : "";
            command.Parameters.Add("@Section1_8_Condition", SqlDbType.NVarChar).Value = Modal.Section1_8_Condition != null ? Modal.Section1_8_Condition : "";
            command.Parameters.Add("@Section1_8_Comment", SqlDbType.NVarChar).Value = Modal.Section1_8_Comment != null ? Modal.Section1_8_Comment : "";
            command.Parameters.Add("@Section1_9_Condition", SqlDbType.NVarChar).Value = Modal.Section1_9_Condition != null ? Modal.Section1_9_Condition : "";
            command.Parameters.Add("@Section1_9_Comment", SqlDbType.NVarChar).Value = Modal.Section1_9_Comment != null ? Modal.Section1_9_Comment : "";
            command.Parameters.Add("@Section1_10_Condition", SqlDbType.NVarChar).Value = Modal.Section1_10_Condition != null ? Modal.Section1_10_Condition : "";
            command.Parameters.Add("@Section1_10_Comment", SqlDbType.NVarChar).Value = Modal.Section1_10_Comment != null ? Modal.Section1_10_Comment : "";
            command.Parameters.Add("@Section1_11_Condition", SqlDbType.NVarChar).Value = Modal.Section1_11_Condition != null ? Modal.Section1_11_Condition : "";
            command.Parameters.Add("@Section1_11_Comment", SqlDbType.NVarChar).Value = Modal.Section1_11_Comment != null ? Modal.Section1_11_Comment : "";
            command.Parameters.Add("@Section2_1_Condition", SqlDbType.NVarChar).Value = Modal.Section2_1_Condition != null ? Modal.Section2_1_Condition : "";
            command.Parameters.Add("@Section2_1_Comment", SqlDbType.NVarChar).Value = Modal.Section2_1_Comment != null ? Modal.Section2_1_Comment : "";
            command.Parameters.Add("@Section2_2_Condition", SqlDbType.NVarChar).Value = Modal.Section2_2_Condition != null ? Modal.Section2_2_Condition : "";
            command.Parameters.Add("@Section2_2_Comment", SqlDbType.NVarChar).Value = Modal.Section2_2_Comment != null ? Modal.Section2_2_Comment : "";
            command.Parameters.Add("@Section2_3_Condition", SqlDbType.NVarChar).Value = Modal.Section2_3_Condition != null ? Modal.Section2_3_Condition : "";
            command.Parameters.Add("@Section2_3_Comment", SqlDbType.NVarChar).Value = Modal.Section2_3_Comment != null ? Modal.Section2_3_Comment : "";
            command.Parameters.Add("@Section2_4_Condition", SqlDbType.NVarChar).Value = Modal.Section2_4_Condition != null ? Modal.Section2_4_Condition : "";
            command.Parameters.Add("@Section2_4_Comment", SqlDbType.NVarChar).Value = Modal.Section2_4_Comment != null ? Modal.Section2_4_Comment : "";
            command.Parameters.Add("@Section2_5_Condition", SqlDbType.NVarChar).Value = Modal.Section2_5_Condition != null ? Modal.Section2_5_Condition : "";
            command.Parameters.Add("@Section2_5_Comment", SqlDbType.NVarChar).Value = Modal.Section2_5_Comment != null ? Modal.Section2_5_Comment : "";
            command.Parameters.Add("@Section2_6_Condition", SqlDbType.NVarChar).Value = Modal.Section2_6_Condition != null ? Modal.Section2_6_Condition : "";
            command.Parameters.Add("@Section2_6_Comment", SqlDbType.NVarChar).Value = Modal.Section2_6_Comment != null ? Modal.Section2_6_Comment : "";
            command.Parameters.Add("@Section2_7_Condition", SqlDbType.NVarChar).Value = Modal.Section2_7_Condition != null ? Modal.Section2_7_Condition : "";
            command.Parameters.Add("@Section2_7_Comment", SqlDbType.NVarChar).Value = Modal.Section2_7_Comment != null ? Modal.Section2_7_Comment : "";
            command.Parameters.Add("@Section3_1_Condition", SqlDbType.NVarChar).Value = Modal.Section3_1_Condition != null ? Modal.Section3_1_Condition : "";
            command.Parameters.Add("@Section3_1_Comment", SqlDbType.NVarChar).Value = Modal.Section3_1_Comment != null ? Modal.Section3_1_Comment : "";
            command.Parameters.Add("@Section3_2_Condition", SqlDbType.NVarChar).Value = Modal.Section3_2_Condition != null ? Modal.Section3_2_Condition : "";
            command.Parameters.Add("@Section3_2_Comment", SqlDbType.NVarChar).Value = Modal.Section3_2_Comment != null ? Modal.Section3_2_Comment : "";
            command.Parameters.Add("@Section3_3_Condition", SqlDbType.NVarChar).Value = Modal.Section3_3_Condition != null ? Modal.Section3_3_Condition : "";
            command.Parameters.Add("@Section3_3_Comment", SqlDbType.NVarChar).Value = Modal.Section3_3_Comment != null ? Modal.Section3_3_Comment : "";
            command.Parameters.Add("@Section3_4_Condition", SqlDbType.NVarChar).Value = Modal.Section3_4_Condition != null ? Modal.Section3_4_Condition : "";
            command.Parameters.Add("@Section3_4_Comment", SqlDbType.NVarChar).Value = Modal.Section3_4_Comment != null ? Modal.Section3_4_Comment : "";
            command.Parameters.Add("@Section3_5_Condition", SqlDbType.NVarChar).Value = Modal.Section3_5_Condition != null ? Modal.Section3_5_Condition : "";
            command.Parameters.Add("@Section3_5_Comment", SqlDbType.NVarChar).Value = Modal.Section3_5_Comment != null ? Modal.Section3_5_Comment : "";
            command.Parameters.Add("@Section4_1_Condition", SqlDbType.NVarChar).Value = Modal.Section4_1_Condition != null ? Modal.Section4_1_Condition : "";
            command.Parameters.Add("@Section4_1_Comment", SqlDbType.NVarChar).Value = Modal.Section4_1_Comment != null ? Modal.Section4_1_Comment : "";
            command.Parameters.Add("@Section4_2_Condition", SqlDbType.NVarChar).Value = Modal.Section4_2_Condition != null ? Modal.Section4_2_Condition : "";
            command.Parameters.Add("@Section4_2_Comment", SqlDbType.NVarChar).Value = Modal.Section4_2_Comment != null ? Modal.Section4_2_Comment : "";
            command.Parameters.Add("@Section4_3_Condition", SqlDbType.NVarChar).Value = Modal.Section4_3_Condition != null ? Modal.Section4_3_Condition : "";
            command.Parameters.Add("@Section4_3_Comment", SqlDbType.NVarChar).Value = Modal.Section4_3_Comment != null ? Modal.Section4_3_Comment : "";
            command.Parameters.Add("@Section5_1_Condition", SqlDbType.NVarChar).Value = Modal.Section5_1_Condition != null ? Modal.Section5_1_Condition : "";
            command.Parameters.Add("@Section5_1_Comment", SqlDbType.NVarChar).Value = Modal.Section5_1_Comment != null ? Modal.Section5_1_Comment : "";
            command.Parameters.Add("@Section5_6_Condition", SqlDbType.NVarChar).Value = Modal.Section5_6_Condition != null ? Modal.Section5_6_Condition : "";
            command.Parameters.Add("@Section5_6_Comment", SqlDbType.NVarChar).Value = Modal.Section5_6_Comment != null ? Modal.Section5_6_Comment : "";
            command.Parameters.Add("@Section5_8_Condition", SqlDbType.NVarChar).Value = Modal.Section5_8_Condition != null ? Modal.Section5_8_Condition : "";
            command.Parameters.Add("@Section5_8_Comment", SqlDbType.NVarChar).Value = Modal.Section5_8_Comment != null ? Modal.Section5_8_Comment : "";
            command.Parameters.Add("@Section5_9_Condition", SqlDbType.NVarChar).Value = Modal.Section5_9_Condition != null ? Modal.Section5_9_Condition : "";
            command.Parameters.Add("@Section5_9_Comment", SqlDbType.NVarChar).Value = Modal.Section5_9_Comment != null ? Modal.Section5_9_Comment : "";
            command.Parameters.Add("@Section6_1_Condition", SqlDbType.NVarChar).Value = Modal.Section6_1_Condition != null ? Modal.Section6_1_Condition : "";
            command.Parameters.Add("@Section6_1_Comment", SqlDbType.NVarChar).Value = Modal.Section6_1_Comment != null ? Modal.Section6_1_Comment : "";
            command.Parameters.Add("@Section6_2_Condition", SqlDbType.NVarChar).Value = Modal.Section6_2_Condition != null ? Modal.Section6_2_Condition : "";
            command.Parameters.Add("@Section6_2_Comment", SqlDbType.NVarChar).Value = Modal.Section6_2_Comment != null ? Modal.Section6_2_Comment : "";
            command.Parameters.Add("@Section6_3_Condition", SqlDbType.NVarChar).Value = Modal.Section6_3_Condition != null ? Modal.Section6_3_Condition : "";
            command.Parameters.Add("@Section6_3_Comment", SqlDbType.NVarChar).Value = Modal.Section6_3_Comment != null ? Modal.Section6_3_Comment : "";
            command.Parameters.Add("@Section6_4_Condition", SqlDbType.NVarChar).Value = Modal.Section6_4_Condition != null ? Modal.Section6_4_Condition : "";
            command.Parameters.Add("@Section6_4_Comment", SqlDbType.NVarChar).Value = Modal.Section6_4_Comment != null ? Modal.Section6_4_Comment : "";
            command.Parameters.Add("@Section6_5_Condition", SqlDbType.NVarChar).Value = Modal.Section6_5_Condition != null ? Modal.Section6_5_Condition : "";
            command.Parameters.Add("@Section6_5_Comment", SqlDbType.NVarChar).Value = Modal.Section6_5_Comment != null ? Modal.Section6_5_Comment : "";
            command.Parameters.Add("@Section6_6_Condition", SqlDbType.NVarChar).Value = Modal.Section6_6_Condition != null ? Modal.Section6_6_Condition : "";
            command.Parameters.Add("@Section6_6_Comment", SqlDbType.NVarChar).Value = Modal.Section6_6_Comment != null ? Modal.Section6_6_Comment : "";
            command.Parameters.Add("@Section6_7_Condition", SqlDbType.NVarChar).Value = Modal.Section6_7_Condition != null ? Modal.Section6_7_Condition : "";
            command.Parameters.Add("@Section6_7_Comment", SqlDbType.NVarChar).Value = Modal.Section6_7_Comment != null ? Modal.Section6_7_Comment : "";
            command.Parameters.Add("@Section6_8_Condition", SqlDbType.NVarChar).Value = Modal.Section6_8_Condition != null ? Modal.Section6_8_Condition : "";
            command.Parameters.Add("@Section6_8_Comment", SqlDbType.NVarChar).Value = Modal.Section6_8_Comment != null ? Modal.Section6_8_Comment : "";
            command.Parameters.Add("@Section7_1_Condition", SqlDbType.NVarChar).Value = Modal.Section7_1_Condition != null ? Modal.Section7_1_Condition : "";
            command.Parameters.Add("@Section7_1_Comment", SqlDbType.NVarChar).Value = Modal.Section7_1_Comment != null ? Modal.Section7_1_Comment : "";
            command.Parameters.Add("@Section7_2_Condition", SqlDbType.NVarChar).Value = Modal.Section7_2_Condition != null ? Modal.Section7_2_Condition : "";
            command.Parameters.Add("@Section7_2_Comment", SqlDbType.NVarChar).Value = Modal.Section7_2_Comment != null ? Modal.Section7_2_Comment : "";
            command.Parameters.Add("@Section7_3_Condition", SqlDbType.NVarChar).Value = Modal.Section7_3_Condition != null ? Modal.Section7_3_Condition : "";
            command.Parameters.Add("@Section7_3_Comment", SqlDbType.NVarChar).Value = Modal.Section7_3_Comment != null ? Modal.Section7_3_Comment : "";
            command.Parameters.Add("@Section7_4_Condition", SqlDbType.NVarChar).Value = Modal.Section7_4_Condition != null ? Modal.Section7_4_Condition : "";
            command.Parameters.Add("@Section7_4_Comment", SqlDbType.NVarChar).Value = Modal.Section7_4_Comment != null ? Modal.Section7_4_Comment : "";
            command.Parameters.Add("@Section7_5_Condition", SqlDbType.NVarChar).Value = Modal.Section7_5_Condition != null ? Modal.Section7_5_Condition : "";
            command.Parameters.Add("@Section7_5_Comment", SqlDbType.NVarChar).Value = Modal.Section7_5_Comment != null ? Modal.Section7_5_Comment : "";
            command.Parameters.Add("@Section7_6_Condition", SqlDbType.NVarChar).Value = Modal.Section7_6_Condition != null ? Modal.Section7_6_Condition : "";
            command.Parameters.Add("@Section7_6_Comment", SqlDbType.NVarChar).Value = Modal.Section7_6_Comment != null ? Modal.Section7_6_Comment : "";
            command.Parameters.Add("@Section8_1_Condition", SqlDbType.NVarChar).Value = Modal.Section8_1_Condition != null ? Modal.Section8_1_Condition : "";
            command.Parameters.Add("@Section8_1_Comment", SqlDbType.NVarChar).Value = Modal.Section8_1_Comment != null ? Modal.Section8_1_Comment : "";
            command.Parameters.Add("@Section8_2_Condition", SqlDbType.NVarChar).Value = Modal.Section8_2_Condition != null ? Modal.Section8_2_Condition : "";
            command.Parameters.Add("@Section8_2_Comment", SqlDbType.NVarChar).Value = Modal.Section8_2_Comment != null ? Modal.Section8_2_Comment : "";
            command.Parameters.Add("@Section8_3_Condition", SqlDbType.NVarChar).Value = Modal.Section8_3_Condition != null ? Modal.Section8_3_Condition : "";
            command.Parameters.Add("@Section8_3_Comment", SqlDbType.NVarChar).Value = Modal.Section8_3_Comment != null ? Modal.Section8_3_Comment : "";
            command.Parameters.Add("@Section8_4_Condition", SqlDbType.NVarChar).Value = Modal.Section8_4_Condition != null ? Modal.Section8_4_Condition : "";
            command.Parameters.Add("@Section8_4_Comment", SqlDbType.NVarChar).Value = Modal.Section8_4_Comment != null ? Modal.Section8_4_Comment : "";
            command.Parameters.Add("@Section8_5_Condition", SqlDbType.NVarChar).Value = Modal.Section8_5_Condition != null ? Modal.Section8_5_Condition : "";
            command.Parameters.Add("@Section8_5_Comment", SqlDbType.NVarChar).Value = Modal.Section8_5_Comment != null ? Modal.Section8_5_Comment : "";
            command.Parameters.Add("@Section8_6_Condition", SqlDbType.NVarChar).Value = Modal.Section8_6_Condition != null ? Modal.Section8_6_Condition : "";
            command.Parameters.Add("@Section8_6_Comment", SqlDbType.NVarChar).Value = Modal.Section8_6_Comment != null ? Modal.Section8_6_Comment : "";
            command.Parameters.Add("@Section8_7_Condition", SqlDbType.NVarChar).Value = Modal.Section8_7_Condition != null ? Modal.Section8_7_Condition : "";
            command.Parameters.Add("@Section8_7_Comment", SqlDbType.NVarChar).Value = Modal.Section8_7_Comment != null ? Modal.Section8_7_Comment : "";
            command.Parameters.Add("@Section8_8_Condition", SqlDbType.NVarChar).Value = Modal.Section8_8_Condition != null ? Modal.Section8_8_Condition : "";
            command.Parameters.Add("@Section8_8_Comment", SqlDbType.NVarChar).Value = Modal.Section8_8_Comment != null ? Modal.Section8_8_Comment : "";
            command.Parameters.Add("@Section8_9_Condition", SqlDbType.NVarChar).Value = Modal.Section8_9_Condition != null ? Modal.Section8_9_Condition : "";
            command.Parameters.Add("@Section8_9_Comment", SqlDbType.NVarChar).Value = Modal.Section8_9_Comment != null ? Modal.Section8_9_Comment : "";
            command.Parameters.Add("@Section8_10_Condition", SqlDbType.NVarChar).Value = Modal.Section8_10_Condition != null ? Modal.Section8_10_Condition : "";
            command.Parameters.Add("@Section8_10_Comment", SqlDbType.NVarChar).Value = Modal.Section8_10_Comment != null ? Modal.Section8_10_Comment : "";
            command.Parameters.Add("@Section8_11_Condition", SqlDbType.NVarChar).Value = Modal.Section8_11_Condition != null ? Modal.Section8_11_Condition : "";
            command.Parameters.Add("@Section8_11_Comment", SqlDbType.NVarChar).Value = Modal.Section8_11_Comment != null ? Modal.Section8_11_Comment : "";
            command.Parameters.Add("@Section8_12_Condition", SqlDbType.NVarChar).Value = Modal.Section8_12_Condition != null ? Modal.Section8_12_Condition : "";
            command.Parameters.Add("@Section8_12_Comment", SqlDbType.NVarChar).Value = Modal.Section8_12_Comment != null ? Modal.Section8_12_Comment : "";
            command.Parameters.Add("@Section8_13_Condition", SqlDbType.NVarChar).Value = Modal.Section8_13_Condition != null ? Modal.Section8_13_Condition : "";
            command.Parameters.Add("@Section8_13_Comment", SqlDbType.NVarChar).Value = Modal.Section8_13_Comment != null ? Modal.Section8_13_Comment : "";
            command.Parameters.Add("@Section8_14_Condition", SqlDbType.NVarChar).Value = Modal.Section8_14_Condition != null ? Modal.Section8_14_Condition : "";
            command.Parameters.Add("@Section8_14_Comment", SqlDbType.NVarChar).Value = Modal.Section8_14_Comment != null ? Modal.Section8_14_Comment : "";
            command.Parameters.Add("@Section8_15_Condition", SqlDbType.NVarChar).Value = Modal.Section8_15_Condition != null ? Modal.Section8_15_Condition : "";
            command.Parameters.Add("@Section8_15_Comment", SqlDbType.NVarChar).Value = Modal.Section8_15_Comment != null ? Modal.Section8_15_Comment : "";
            command.Parameters.Add("@Section8_16_Condition", SqlDbType.NVarChar).Value = Modal.Section8_16_Condition != null ? Modal.Section8_16_Condition : "";
            command.Parameters.Add("@Section8_16_Comment", SqlDbType.NVarChar).Value = Modal.Section8_16_Comment != null ? Modal.Section8_16_Comment : "";
            command.Parameters.Add("@Section8_17_Condition", SqlDbType.NVarChar).Value = Modal.Section8_17_Condition != null ? Modal.Section8_17_Condition : "";
            command.Parameters.Add("@Section8_17_Comment", SqlDbType.NVarChar).Value = Modal.Section8_17_Comment != null ? Modal.Section8_17_Comment : "";
            command.Parameters.Add("@Section8_18_Condition", SqlDbType.NVarChar).Value = Modal.Section8_18_Condition != null ? Modal.Section8_18_Condition : "";
            command.Parameters.Add("@Section8_18_Comment", SqlDbType.NVarChar).Value = Modal.Section8_18_Comment != null ? Modal.Section8_18_Comment : "";
            command.Parameters.Add("@Section8_19_Condition", SqlDbType.NVarChar).Value = Modal.Section8_19_Condition != null ? Modal.Section8_19_Condition : "";
            command.Parameters.Add("@Section8_19_Comment", SqlDbType.NVarChar).Value = Modal.Section8_19_Comment != null ? Modal.Section8_19_Comment : "";
            command.Parameters.Add("@Section8_20_Condition", SqlDbType.NVarChar).Value = Modal.Section8_20_Condition != null ? Modal.Section8_20_Condition : "";
            command.Parameters.Add("@Section8_20_Comment", SqlDbType.NVarChar).Value = Modal.Section8_20_Comment != null ? Modal.Section8_20_Comment : "";
            command.Parameters.Add("@Section8_21_Condition", SqlDbType.NVarChar).Value = Modal.Section8_21_Condition != null ? Modal.Section8_21_Condition : "";
            command.Parameters.Add("@Section8_21_Comment", SqlDbType.NVarChar).Value = Modal.Section8_21_Comment != null ? Modal.Section8_21_Comment : "";
            command.Parameters.Add("@Section8_22_Condition", SqlDbType.NVarChar).Value = Modal.Section8_22_Condition != null ? Modal.Section8_22_Condition : "";
            command.Parameters.Add("@Section8_22_Comment", SqlDbType.NVarChar).Value = Modal.Section8_22_Comment != null ? Modal.Section8_22_Comment : "";
            command.Parameters.Add("@Section8_23_Condition", SqlDbType.NVarChar).Value = Modal.Section8_23_Condition != null ? Modal.Section8_23_Condition : "";
            command.Parameters.Add("@Section8_23_Comment", SqlDbType.NVarChar).Value = Modal.Section8_23_Comment != null ? Modal.Section8_23_Comment : "";
            command.Parameters.Add("@Section8_24_Condition", SqlDbType.NVarChar).Value = Modal.Section8_24_Condition != null ? Modal.Section8_24_Condition : "";
            command.Parameters.Add("@Section8_24_Comment", SqlDbType.NVarChar).Value = Modal.Section8_24_Comment != null ? Modal.Section8_24_Comment : "";
            command.Parameters.Add("@Section8_25_Condition", SqlDbType.NVarChar).Value = Modal.Section8_25_Condition != null ? Modal.Section8_25_Condition : "";
            command.Parameters.Add("@Section8_25_Comment", SqlDbType.NVarChar).Value = Modal.Section8_25_Comment != null ? Modal.Section8_25_Comment : "";
            command.Parameters.Add("@Section9_1_Condition", SqlDbType.NVarChar).Value = Modal.Section9_1_Condition != null ? Modal.Section9_1_Condition : "";
            command.Parameters.Add("@Section9_1_Comment", SqlDbType.NVarChar).Value = Modal.Section9_1_Comment != null ? Modal.Section9_1_Comment : "";
            command.Parameters.Add("@Section9_2_Condition", SqlDbType.NVarChar).Value = Modal.Section9_2_Condition != null ? Modal.Section9_2_Condition : "";
            command.Parameters.Add("@Section9_2_Comment", SqlDbType.NVarChar).Value = Modal.Section9_2_Comment != null ? Modal.Section9_2_Comment : "";
            command.Parameters.Add("@Section9_3_Condition", SqlDbType.NVarChar).Value = Modal.Section9_3_Condition != null ? Modal.Section9_3_Condition : "";
            command.Parameters.Add("@Section9_3_Comment", SqlDbType.NVarChar).Value = Modal.Section9_3_Comment != null ? Modal.Section9_3_Comment : "";
            command.Parameters.Add("@Section9_4_Condition", SqlDbType.NVarChar).Value = Modal.Section9_4_Condition != null ? Modal.Section9_4_Condition : "";
            command.Parameters.Add("@Section9_4_Comment", SqlDbType.NVarChar).Value = Modal.Section9_4_Comment != null ? Modal.Section9_4_Comment : "";
            command.Parameters.Add("@Section9_5_Condition", SqlDbType.NVarChar).Value = Modal.Section9_5_Condition != null ? Modal.Section9_5_Condition : "";
            command.Parameters.Add("@Section9_5_Comment", SqlDbType.NVarChar).Value = Modal.Section9_5_Comment != null ? Modal.Section9_5_Comment : "";
            command.Parameters.Add("@Section9_6_Condition", SqlDbType.NVarChar).Value = Modal.Section9_6_Condition != null ? Modal.Section9_6_Condition : "";
            command.Parameters.Add("@Section9_6_Comment", SqlDbType.NVarChar).Value = Modal.Section9_6_Comment != null ? Modal.Section9_6_Comment : "";
            command.Parameters.Add("@Section9_7_Condition", SqlDbType.NVarChar).Value = Modal.Section9_7_Condition != null ? Modal.Section9_7_Condition : "";
            command.Parameters.Add("@Section9_7_Comment", SqlDbType.NVarChar).Value = Modal.Section9_7_Comment != null ? Modal.Section9_7_Comment : "";
            command.Parameters.Add("@Section9_8_Condition", SqlDbType.NVarChar).Value = Modal.Section9_8_Condition != null ? Modal.Section9_8_Condition : "";
            command.Parameters.Add("@Section9_8_Comment", SqlDbType.NVarChar).Value = Modal.Section9_8_Comment != null ? Modal.Section9_8_Comment : "";
            command.Parameters.Add("@Section9_9_Condition", SqlDbType.NVarChar).Value = Modal.Section9_9_Condition != null ? Modal.Section9_9_Condition : "";
            command.Parameters.Add("@Section9_9_Comment", SqlDbType.NVarChar).Value = Modal.Section9_9_Comment != null ? Modal.Section9_9_Comment : "";
            command.Parameters.Add("@Section9_10_Condition", SqlDbType.NVarChar).Value = Modal.Section9_10_Condition != null ? Modal.Section9_10_Condition : "";
            command.Parameters.Add("@Section9_10_Comment", SqlDbType.NVarChar).Value = Modal.Section9_10_Comment != null ? Modal.Section9_10_Comment : "";
            command.Parameters.Add("@Section9_11_Condition", SqlDbType.NVarChar).Value = Modal.Section9_11_Condition != null ? Modal.Section9_11_Condition : "";
            command.Parameters.Add("@Section9_11_Comment", SqlDbType.NVarChar).Value = Modal.Section9_11_Comment != null ? Modal.Section9_11_Comment : "";
            command.Parameters.Add("@Section9_12_Condition", SqlDbType.NVarChar).Value = Modal.Section9_12_Condition != null ? Modal.Section9_12_Condition : "";
            command.Parameters.Add("@Section9_12_Comment", SqlDbType.NVarChar).Value = Modal.Section9_12_Comment != null ? Modal.Section9_12_Comment : "";
            command.Parameters.Add("@Section9_13_Condition", SqlDbType.NVarChar).Value = Modal.Section9_13_Condition != null ? Modal.Section9_13_Condition : "";
            command.Parameters.Add("@Section9_13_Comment", SqlDbType.NVarChar).Value = Modal.Section9_13_Comment != null ? Modal.Section9_13_Comment : "";
            command.Parameters.Add("@Section9_14_Condition", SqlDbType.NVarChar).Value = Modal.Section9_14_Condition != null ? Modal.Section9_14_Condition : "";
            command.Parameters.Add("@Section9_14_Comment", SqlDbType.NVarChar).Value = Modal.Section9_14_Comment != null ? Modal.Section9_14_Comment : "";
            command.Parameters.Add("@Section9_15_Condition", SqlDbType.NVarChar).Value = Modal.Section9_15_Condition != null ? Modal.Section9_15_Condition : "";
            command.Parameters.Add("@Section9_15_Comment", SqlDbType.NVarChar).Value = Modal.Section9_15_Comment != null ? Modal.Section9_15_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section9_16_Condition", SqlDbType.NVarChar).Value = Modal.Section9_16_Condition != null ? Modal.Section9_16_Condition : "";
            command.Parameters.Add("@Section9_16_Comment", SqlDbType.NVarChar).Value = Modal.Section9_16_Comment != null ? Modal.Section9_16_Comment : "";
            command.Parameters.Add("@Section9_17_Condition", SqlDbType.NVarChar).Value = Modal.Section9_17_Condition != null ? Modal.Section9_17_Condition : "";
            command.Parameters.Add("@Section9_17_Comment", SqlDbType.NVarChar).Value = Modal.Section9_17_Comment != null ? Modal.Section9_17_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@Section10_1_Condition", SqlDbType.NVarChar).Value = Modal.Section10_1_Condition != null ? Modal.Section10_1_Condition : "";
            command.Parameters.Add("@Section10_1_Comment", SqlDbType.NVarChar).Value = Modal.Section10_1_Comment != null ? Modal.Section10_1_Comment : "";
            command.Parameters.Add("@Section10_2_Condition", SqlDbType.NVarChar).Value = Modal.Section10_2_Condition != null ? Modal.Section10_2_Condition : "";
            command.Parameters.Add("@Section10_2_Comment", SqlDbType.NVarChar).Value = Modal.Section10_2_Comment != null ? Modal.Section10_2_Comment : "";
            command.Parameters.Add("@Section10_3_Condition", SqlDbType.NVarChar).Value = Modal.Section10_3_Condition != null ? Modal.Section10_3_Condition : "";
            command.Parameters.Add("@Section10_3_Comment", SqlDbType.NVarChar).Value = Modal.Section10_3_Comment != null ? Modal.Section10_3_Comment : "";
            command.Parameters.Add("@Section10_4_Condition", SqlDbType.NVarChar).Value = Modal.Section10_4_Condition != null ? Modal.Section10_4_Condition : "";
            command.Parameters.Add("@Section10_4_Comment", SqlDbType.NVarChar).Value = Modal.Section10_4_Comment != null ? Modal.Section10_4_Comment : "";
            command.Parameters.Add("@Section10_5_Condition", SqlDbType.NVarChar).Value = Modal.Section10_5_Condition != null ? Modal.Section10_5_Condition : "";
            command.Parameters.Add("@Section10_5_Comment", SqlDbType.NVarChar).Value = Modal.Section10_5_Comment != null ? Modal.Section10_5_Comment : "";
            command.Parameters.Add("@Section10_6_Condition", SqlDbType.NVarChar).Value = Modal.Section10_6_Condition != null ? Modal.Section10_6_Condition : "";
            command.Parameters.Add("@Section10_6_Comment", SqlDbType.NVarChar).Value = Modal.Section10_6_Comment != null ? Modal.Section10_6_Comment : "";
            command.Parameters.Add("@Section10_7_Condition", SqlDbType.NVarChar).Value = Modal.Section10_7_Condition != null ? Modal.Section10_7_Condition : "";
            command.Parameters.Add("@Section10_7_Comment", SqlDbType.NVarChar).Value = Modal.Section10_7_Comment != null ? Modal.Section10_7_Comment : "";
            command.Parameters.Add("@Section10_8_Condition", SqlDbType.NVarChar).Value = Modal.Section10_8_Condition != null ? Modal.Section10_8_Condition : "";
            command.Parameters.Add("@Section10_8_Comment", SqlDbType.NVarChar).Value = Modal.Section10_8_Comment != null ? Modal.Section10_8_Comment : "";
            command.Parameters.Add("@Section10_9_Condition", SqlDbType.NVarChar).Value = Modal.Section10_9_Condition != null ? Modal.Section10_9_Condition : "";
            command.Parameters.Add("@Section10_9_Comment", SqlDbType.NVarChar).Value = Modal.Section10_9_Comment != null ? Modal.Section10_9_Comment : "";
            command.Parameters.Add("@Section10_10_Condition", SqlDbType.NVarChar).Value = Modal.Section10_10_Condition != null ? Modal.Section10_10_Condition : "";
            command.Parameters.Add("@Section10_10_Comment", SqlDbType.NVarChar).Value = Modal.Section10_10_Comment != null ? Modal.Section10_10_Comment : "";
            command.Parameters.Add("@Section10_11_Condition", SqlDbType.NVarChar).Value = Modal.Section10_11_Condition != null ? Modal.Section10_11_Condition : "";
            command.Parameters.Add("@Section10_11_Comment", SqlDbType.NVarChar).Value = Modal.Section10_11_Comment != null ? Modal.Section10_11_Comment : "";
            command.Parameters.Add("@Section10_12_Condition", SqlDbType.NVarChar).Value = Modal.Section10_12_Condition != null ? Modal.Section10_12_Condition : "";
            command.Parameters.Add("@Section10_12_Comment", SqlDbType.NVarChar).Value = Modal.Section10_12_Comment != null ? Modal.Section10_12_Comment : "";
            command.Parameters.Add("@Section10_13_Condition", SqlDbType.NVarChar).Value = Modal.Section10_13_Condition != null ? Modal.Section10_13_Condition : "";
            command.Parameters.Add("@Section10_13_Comment", SqlDbType.NVarChar).Value = Modal.Section10_13_Comment != null ? Modal.Section10_13_Comment : "";
            command.Parameters.Add("@Section10_14_Condition", SqlDbType.NVarChar).Value = Modal.Section10_14_Condition != null ? Modal.Section10_14_Condition : "";
            command.Parameters.Add("@Section10_14_Comment", SqlDbType.NVarChar).Value = Modal.Section10_14_Comment != null ? Modal.Section10_14_Comment : "";
            command.Parameters.Add("@Section10_15_Condition", SqlDbType.NVarChar).Value = Modal.Section10_15_Condition != null ? Modal.Section10_15_Condition : "";
            command.Parameters.Add("@Section10_15_Comment", SqlDbType.NVarChar).Value = Modal.Section10_15_Comment != null ? Modal.Section10_15_Comment : "";
            command.Parameters.Add("@Section10_16_Condition", SqlDbType.NVarChar).Value = Modal.Section10_16_Condition != null ? Modal.Section10_16_Condition : "";
            command.Parameters.Add("@Section10_16_Comment", SqlDbType.NVarChar).Value = Modal.Section10_16_Comment != null ? Modal.Section10_16_Comment : "";
            command.Parameters.Add("@Section11_1_Condition", SqlDbType.NVarChar).Value = Modal.Section11_1_Condition != null ? Modal.Section11_1_Condition : "";
            command.Parameters.Add("@Section11_1_Comment", SqlDbType.NVarChar).Value = Modal.Section11_1_Comment != null ? Modal.Section11_1_Comment : "";
            command.Parameters.Add("@Section11_2_Condition", SqlDbType.NVarChar).Value = Modal.Section11_2_Condition != null ? Modal.Section11_2_Condition : "";
            command.Parameters.Add("@Section11_2_Comment", SqlDbType.NVarChar).Value = Modal.Section11_2_Comment != null ? Modal.Section11_2_Comment : "";
            command.Parameters.Add("@Section11_3_Condition", SqlDbType.NVarChar).Value = Modal.Section11_3_Condition != null ? Modal.Section11_3_Condition : "";
            command.Parameters.Add("@Section11_3_Comment", SqlDbType.NVarChar).Value = Modal.Section11_3_Comment != null ? Modal.Section11_3_Comment : "";
            command.Parameters.Add("@Section11_4_Condition", SqlDbType.NVarChar).Value = Modal.Section11_4_Condition != null ? Modal.Section11_4_Condition : "";
            command.Parameters.Add("@Section11_4_Comment", SqlDbType.NVarChar).Value = Modal.Section11_4_Comment != null ? Modal.Section11_4_Comment : "";
            command.Parameters.Add("@Section11_5_Condition", SqlDbType.NVarChar).Value = Modal.Section11_5_Condition != null ? Modal.Section11_5_Condition : "";
            command.Parameters.Add("@Section11_5_Comment", SqlDbType.NVarChar).Value = Modal.Section11_5_Comment != null ? Modal.Section11_5_Comment : "";
            command.Parameters.Add("@Section11_6_Condition", SqlDbType.NVarChar).Value = Modal.Section11_6_Condition != null ? Modal.Section11_6_Condition : "";
            command.Parameters.Add("@Section11_6_Comment", SqlDbType.NVarChar).Value = Modal.Section11_6_Comment != null ? Modal.Section11_6_Comment : "";
            command.Parameters.Add("@Section11_7_Condition", SqlDbType.NVarChar).Value = Modal.Section11_7_Condition != null ? Modal.Section11_7_Condition : "";
            command.Parameters.Add("@Section11_7_Comment", SqlDbType.NVarChar).Value = Modal.Section11_7_Comment != null ? Modal.Section11_7_Comment : "";
            command.Parameters.Add("@Section11_8_Condition", SqlDbType.NVarChar).Value = Modal.Section11_8_Condition != null ? Modal.Section11_8_Condition : "";
            command.Parameters.Add("@Section11_8_Comment", SqlDbType.NVarChar).Value = Modal.Section11_8_Comment != null ? Modal.Section11_8_Comment : "";
            command.Parameters.Add("@Section12_1_Condition", SqlDbType.NVarChar).Value = Modal.Section12_1_Condition != null ? Modal.Section12_1_Condition : "";
            command.Parameters.Add("@Section12_1_Comment", SqlDbType.NVarChar).Value = Modal.Section12_1_Comment != null ? Modal.Section12_1_Comment : "";
            command.Parameters.Add("@Section12_2_Condition", SqlDbType.NVarChar).Value = Modal.Section12_2_Condition != null ? Modal.Section12_2_Condition : "";
            command.Parameters.Add("@Section12_2_Comment", SqlDbType.NVarChar).Value = Modal.Section12_2_Comment != null ? Modal.Section12_2_Comment : "";
            command.Parameters.Add("@Section12_3_Condition", SqlDbType.NVarChar).Value = Modal.Section12_3_Condition != null ? Modal.Section12_3_Condition : "";
            command.Parameters.Add("@Section12_3_Comment", SqlDbType.NVarChar).Value = Modal.Section12_3_Comment != null ? Modal.Section12_3_Comment : "";
            command.Parameters.Add("@Section12_4_Condition", SqlDbType.NVarChar).Value = Modal.Section12_4_Condition != null ? Modal.Section12_4_Condition : "";
            command.Parameters.Add("@Section12_4_Comment", SqlDbType.NVarChar).Value = Modal.Section12_4_Comment != null ? Modal.Section12_4_Comment : "";
            command.Parameters.Add("@Section12_5_Condition", SqlDbType.NVarChar).Value = Modal.Section12_5_Condition != null ? Modal.Section12_5_Condition : "";
            command.Parameters.Add("@Section12_5_Comment", SqlDbType.NVarChar).Value = Modal.Section12_5_Comment != null ? Modal.Section12_5_Comment : "";
            command.Parameters.Add("@Section12_6_Condition", SqlDbType.NVarChar).Value = Modal.Section12_6_Condition != null ? Modal.Section12_6_Condition : "";
            command.Parameters.Add("@Section12_6_Comment", SqlDbType.NVarChar).Value = Modal.Section12_6_Comment != null ? Modal.Section12_6_Comment : "";
            command.Parameters.Add("@Section13_1_Condition", SqlDbType.NVarChar).Value = Modal.Section13_1_Condition != null ? Modal.Section13_1_Condition : "";
            command.Parameters.Add("@Section13_1_Comment", SqlDbType.NVarChar).Value = Modal.Section13_1_Comment != null ? Modal.Section13_1_Comment : "";
            command.Parameters.Add("@Section13_2_Condition", SqlDbType.NVarChar).Value = Modal.Section13_2_Condition != null ? Modal.Section13_2_Condition : "";
            command.Parameters.Add("@Section13_2_Comment", SqlDbType.NVarChar).Value = Modal.Section13_2_Comment != null ? Modal.Section13_2_Comment : "";
            command.Parameters.Add("@Section13_3_Condition", SqlDbType.NVarChar).Value = Modal.Section13_3_Condition != null ? Modal.Section13_3_Condition : "";
            command.Parameters.Add("@Section13_3_Comment", SqlDbType.NVarChar).Value = Modal.Section13_3_Comment != null ? Modal.Section13_3_Comment : "";
            command.Parameters.Add("@Section13_4_Condition", SqlDbType.NVarChar).Value = Modal.Section13_4_Condition != null ? Modal.Section13_4_Condition : "";
            command.Parameters.Add("@Section13_4_Comment", SqlDbType.NVarChar).Value = Modal.Section13_4_Comment != null ? Modal.Section13_4_Comment : "";
            command.Parameters.Add("@Section14_1_Condition", SqlDbType.NVarChar).Value = Modal.Section14_1_Condition != null ? Modal.Section14_1_Condition : "";
            command.Parameters.Add("@Section14_1_Comment", SqlDbType.NVarChar).Value = Modal.Section14_1_Comment != null ? Modal.Section14_1_Comment : "";
            command.Parameters.Add("@Section14_2_Condition", SqlDbType.NVarChar).Value = Modal.Section14_2_Condition != null ? Modal.Section14_2_Condition : "";
            command.Parameters.Add("@Section14_2_Comment", SqlDbType.NVarChar).Value = Modal.Section14_2_Comment != null ? Modal.Section14_2_Comment : "";
            command.Parameters.Add("@Section14_3_Condition", SqlDbType.NVarChar).Value = Modal.Section14_3_Condition != null ? Modal.Section14_3_Condition : "";
            command.Parameters.Add("@Section14_3_Comment", SqlDbType.NVarChar).Value = Modal.Section14_3_Comment != null ? Modal.Section14_3_Comment : "";
            command.Parameters.Add("@Section14_4_Condition", SqlDbType.NVarChar).Value = Modal.Section14_4_Condition != null ? Modal.Section14_4_Condition : "";
            command.Parameters.Add("@Section14_4_Comment", SqlDbType.NVarChar).Value = Modal.Section14_4_Comment != null ? Modal.Section14_4_Comment : "";
            command.Parameters.Add("@Section14_5_Condition", SqlDbType.NVarChar).Value = Modal.Section14_5_Condition != null ? Modal.Section14_5_Condition : "";
            command.Parameters.Add("@Section14_5_Comment", SqlDbType.NVarChar).Value = Modal.Section14_5_Comment != null ? Modal.Section14_5_Comment : "";
            command.Parameters.Add("@Section14_6_Condition", SqlDbType.NVarChar).Value = Modal.Section14_6_Condition != null ? Modal.Section14_6_Condition : "";
            command.Parameters.Add("@Section14_6_Comment", SqlDbType.NVarChar).Value = Modal.Section14_6_Comment != null ? Modal.Section14_6_Comment : "";
            command.Parameters.Add("@Section14_7_Condition", SqlDbType.NVarChar).Value = Modal.Section14_7_Condition != null ? Modal.Section14_7_Condition : "";
            command.Parameters.Add("@Section14_7_Comment", SqlDbType.NVarChar).Value = Modal.Section14_7_Comment != null ? Modal.Section14_7_Comment : "";
            command.Parameters.Add("@Section14_8_Condition", SqlDbType.NVarChar).Value = Modal.Section14_8_Condition != null ? Modal.Section14_8_Condition : "";
            command.Parameters.Add("@Section14_8_Comment", SqlDbType.NVarChar).Value = Modal.Section14_8_Comment != null ? Modal.Section14_8_Comment : "";
            command.Parameters.Add("@Section14_9_Condition", SqlDbType.NVarChar).Value = Modal.Section14_9_Condition != null ? Modal.Section14_9_Condition : "";
            command.Parameters.Add("@Section14_9_Comment", SqlDbType.NVarChar).Value = Modal.Section14_9_Comment != null ? Modal.Section14_9_Comment : "";
            command.Parameters.Add("@Section14_10_Condition", SqlDbType.NVarChar).Value = Modal.Section14_10_Condition != null ? Modal.Section14_10_Condition : "";
            command.Parameters.Add("@Section14_10_Comment", SqlDbType.NVarChar).Value = Modal.Section14_10_Comment != null ? Modal.Section14_10_Comment : "";
            command.Parameters.Add("@Section14_11_Condition", SqlDbType.NVarChar).Value = Modal.Section14_11_Condition != null ? Modal.Section14_11_Condition : "";
            command.Parameters.Add("@Section14_11_Comment", SqlDbType.NVarChar).Value = Modal.Section14_11_Comment != null ? Modal.Section14_11_Comment : "";
            command.Parameters.Add("@Section14_12_Condition", SqlDbType.NVarChar).Value = Modal.Section14_12_Condition != null ? Modal.Section14_12_Condition : "";
            command.Parameters.Add("@Section14_12_Comment", SqlDbType.NVarChar).Value = Modal.Section14_12_Comment != null ? Modal.Section14_12_Comment : "";
            command.Parameters.Add("@Section14_13_Condition", SqlDbType.NVarChar).Value = Modal.Section14_13_Condition != null ? Modal.Section14_13_Condition : "";
            command.Parameters.Add("@Section14_13_Comment", SqlDbType.NVarChar).Value = Modal.Section14_13_Comment != null ? Modal.Section14_13_Comment : "";
            command.Parameters.Add("@Section14_14_Condition", SqlDbType.NVarChar).Value = Modal.Section14_14_Condition != null ? Modal.Section14_14_Condition : "";
            command.Parameters.Add("@Section14_14_Comment", SqlDbType.NVarChar).Value = Modal.Section14_14_Comment != null ? Modal.Section14_14_Comment : "";
            command.Parameters.Add("@Section14_15_Condition", SqlDbType.NVarChar).Value = Modal.Section14_15_Condition != null ? Modal.Section14_15_Condition : "";
            command.Parameters.Add("@Section14_15_Comment", SqlDbType.NVarChar).Value = Modal.Section14_15_Comment != null ? Modal.Section14_15_Comment : "";
            command.Parameters.Add("@Section14_16_Condition", SqlDbType.NVarChar).Value = Modal.Section14_16_Condition != null ? Modal.Section14_16_Condition : "";
            command.Parameters.Add("@Section14_16_Comment", SqlDbType.NVarChar).Value = Modal.Section14_16_Comment != null ? Modal.Section14_16_Comment : "";
            command.Parameters.Add("@Section14_17_Condition", SqlDbType.NVarChar).Value = Modal.Section14_17_Condition != null ? Modal.Section14_17_Condition : "";
            command.Parameters.Add("@Section14_17_Comment", SqlDbType.NVarChar).Value = Modal.Section14_17_Comment != null ? Modal.Section14_17_Comment : "";
            command.Parameters.Add("@Section14_18_Condition", SqlDbType.NVarChar).Value = Modal.Section14_18_Condition != null ? Modal.Section14_18_Condition : "";
            command.Parameters.Add("@Section14_18_Comment", SqlDbType.NVarChar).Value = Modal.Section14_18_Comment != null ? Modal.Section14_18_Comment : "";
            command.Parameters.Add("@Section14_19_Condition", SqlDbType.NVarChar).Value = Modal.Section14_19_Condition != null ? Modal.Section14_19_Condition : "";
            command.Parameters.Add("@Section14_19_Comment", SqlDbType.NVarChar).Value = Modal.Section14_19_Comment != null ? Modal.Section14_19_Comment : "";
            command.Parameters.Add("@Section14_20_Condition", SqlDbType.NVarChar).Value = Modal.Section14_20_Condition != null ? Modal.Section14_20_Condition : "";
            command.Parameters.Add("@Section14_20_Comment", SqlDbType.NVarChar).Value = Modal.Section14_20_Comment != null ? Modal.Section14_20_Comment : "";
            command.Parameters.Add("@Section14_21_Condition", SqlDbType.NVarChar).Value = Modal.Section14_21_Condition != null ? Modal.Section14_21_Condition : "";
            command.Parameters.Add("@Section14_21_Comment", SqlDbType.NVarChar).Value = Modal.Section14_21_Comment != null ? Modal.Section14_21_Comment : "";
            command.Parameters.Add("@Section14_22_Condition", SqlDbType.NVarChar).Value = Modal.Section14_22_Condition != null ? Modal.Section14_22_Condition : "";
            command.Parameters.Add("@Section14_22_Comment", SqlDbType.NVarChar).Value = Modal.Section14_22_Comment != null ? Modal.Section14_22_Comment : "";
            command.Parameters.Add("@Section14_23_Condition", SqlDbType.NVarChar).Value = Modal.Section14_23_Condition != null ? Modal.Section14_23_Condition : "";
            command.Parameters.Add("@Section14_23_Comment", SqlDbType.NVarChar).Value = Modal.Section14_23_Comment != null ? Modal.Section14_23_Comment : "";
            command.Parameters.Add("@Section14_24_Condition", SqlDbType.NVarChar).Value = Modal.Section14_24_Condition != null ? Modal.Section14_24_Condition : "";
            command.Parameters.Add("@Section14_24_Comment", SqlDbType.NVarChar).Value = Modal.Section14_24_Comment != null ? Modal.Section14_24_Comment : "";
            command.Parameters.Add("@Section14_25_Condition", SqlDbType.NVarChar).Value = Modal.Section14_25_Condition != null ? Modal.Section14_25_Condition : "";
            command.Parameters.Add("@Section14_25_Comment", SqlDbType.NVarChar).Value = Modal.Section14_25_Comment != null ? Modal.Section14_25_Comment : "";
            command.Parameters.Add("@Section15_1_Condition", SqlDbType.NVarChar).Value = Modal.Section15_1_Condition != null ? Modal.Section15_1_Condition : "";
            command.Parameters.Add("@Section15_1_Comment", SqlDbType.NVarChar).Value = Modal.Section15_1_Comment != null ? Modal.Section15_1_Comment : "";
            command.Parameters.Add("@Section15_2_Condition", SqlDbType.NVarChar).Value = Modal.Section15_2_Condition != null ? Modal.Section15_2_Condition : "";
            command.Parameters.Add("@Section15_2_Comment", SqlDbType.NVarChar).Value = Modal.Section15_2_Comment != null ? Modal.Section15_2_Comment : "";
            command.Parameters.Add("@Section15_3_Condition", SqlDbType.NVarChar).Value = Modal.Section15_3_Condition != null ? Modal.Section15_3_Condition : "";
            command.Parameters.Add("@Section15_3_Comment", SqlDbType.NVarChar).Value = Modal.Section15_3_Comment != null ? Modal.Section15_3_Comment : "";
            command.Parameters.Add("@Section15_4_Condition", SqlDbType.NVarChar).Value = Modal.Section15_4_Condition != null ? Modal.Section15_4_Condition : "";
            command.Parameters.Add("@Section15_4_Comment", SqlDbType.NVarChar).Value = Modal.Section15_4_Comment != null ? Modal.Section15_4_Comment : "";
            command.Parameters.Add("@Section15_5_Condition", SqlDbType.NVarChar).Value = Modal.Section15_5_Condition != null ? Modal.Section15_5_Condition : "";
            command.Parameters.Add("@Section15_5_Comment", SqlDbType.NVarChar).Value = Modal.Section15_5_Comment != null ? Modal.Section15_5_Comment : "";
            command.Parameters.Add("@Section15_6_Condition", SqlDbType.NVarChar).Value = Modal.Section15_6_Condition != null ? Modal.Section15_6_Condition : "";
            command.Parameters.Add("@Section15_6_Comment", SqlDbType.NVarChar).Value = Modal.Section15_6_Comment != null ? Modal.Section15_6_Comment : "";
            command.Parameters.Add("@Section15_7_Condition", SqlDbType.NVarChar).Value = Modal.Section15_7_Condition != null ? Modal.Section15_7_Condition : "";
            command.Parameters.Add("@Section15_7_Comment", SqlDbType.NVarChar).Value = Modal.Section15_7_Comment != null ? Modal.Section15_7_Comment : "";
            command.Parameters.Add("@Section15_8_Condition", SqlDbType.NVarChar).Value = Modal.Section15_8_Condition != null ? Modal.Section15_8_Condition : "";
            command.Parameters.Add("@Section15_8_Comment", SqlDbType.NVarChar).Value = Modal.Section15_8_Comment != null ? Modal.Section15_8_Comment : "";
            command.Parameters.Add("@Section15_9_Condition", SqlDbType.NVarChar).Value = Modal.Section15_9_Condition != null ? Modal.Section15_9_Condition : "";
            command.Parameters.Add("@Section15_9_Comment", SqlDbType.NVarChar).Value = Modal.Section15_9_Comment != null ? Modal.Section15_9_Comment : "";
            command.Parameters.Add("@Section15_10_Condition", SqlDbType.NVarChar).Value = Modal.Section15_10_Condition != null ? Modal.Section15_10_Condition : "";
            command.Parameters.Add("@Section15_10_Comment", SqlDbType.NVarChar).Value = Modal.Section15_10_Comment != null ? Modal.Section15_10_Comment : "";
            command.Parameters.Add("@Section15_11_Condition", SqlDbType.NVarChar).Value = Modal.Section15_11_Condition != null ? Modal.Section15_11_Condition : "";
            command.Parameters.Add("@Section15_11_Comment", SqlDbType.NVarChar).Value = Modal.Section15_11_Comment != null ? Modal.Section15_11_Comment : "";
            command.Parameters.Add("@Section15_12_Condition", SqlDbType.NVarChar).Value = Modal.Section15_12_Condition != null ? Modal.Section15_12_Condition : "";
            command.Parameters.Add("@Section15_12_Comment", SqlDbType.NVarChar).Value = Modal.Section15_12_Comment != null ? Modal.Section15_12_Comment : "";
            command.Parameters.Add("@Section15_13_Condition", SqlDbType.NVarChar).Value = Modal.Section15_13_Condition != null ? Modal.Section15_13_Condition : "";
            command.Parameters.Add("@Section15_13_Comment", SqlDbType.NVarChar).Value = Modal.Section15_13_Comment != null ? Modal.Section15_13_Comment : "";
            command.Parameters.Add("@Section15_14_Condition", SqlDbType.NVarChar).Value = Modal.Section15_14_Condition != null ? Modal.Section15_14_Condition : "";
            command.Parameters.Add("@Section15_14_Comment", SqlDbType.NVarChar).Value = Modal.Section15_14_Comment != null ? Modal.Section15_14_Comment : "";
            command.Parameters.Add("@Section15_15_Condition", SqlDbType.NVarChar).Value = Modal.Section15_15_Condition != null ? Modal.Section15_15_Condition : "";
            command.Parameters.Add("@Section15_15_Comment", SqlDbType.NVarChar).Value = Modal.Section15_15_Comment != null ? Modal.Section15_15_Comment : "";
            command.Parameters.Add("@Section16_1_Condition", SqlDbType.NVarChar).Value = Modal.Section16_1_Condition != null ? Modal.Section16_1_Condition : "";
            command.Parameters.Add("@Section16_1_Comment", SqlDbType.NVarChar).Value = Modal.Section16_1_Comment != null ? Modal.Section16_1_Comment : "";
            command.Parameters.Add("@Section16_2_Condition", SqlDbType.NVarChar).Value = Modal.Section16_2_Condition != null ? Modal.Section16_2_Condition : "";
            command.Parameters.Add("@Section16_2_Comment", SqlDbType.NVarChar).Value = Modal.Section16_2_Comment != null ? Modal.Section16_2_Comment : "";
            command.Parameters.Add("@Section16_3_Condition", SqlDbType.NVarChar).Value = Modal.Section16_3_Condition != null ? Modal.Section16_3_Condition : "";
            command.Parameters.Add("@Section16_3_Comment", SqlDbType.NVarChar).Value = Modal.Section16_3_Comment != null ? Modal.Section16_3_Comment : "";
            command.Parameters.Add("@Section16_4_Condition", SqlDbType.NVarChar).Value = Modal.Section16_4_Condition != null ? Modal.Section16_4_Condition : "";
            command.Parameters.Add("@Section16_4_Comment", SqlDbType.NVarChar).Value = Modal.Section16_4_Comment != null ? Modal.Section16_4_Comment : "";
            command.Parameters.Add("@Section17_1_Condition", SqlDbType.NVarChar).Value = Modal.Section17_1_Condition != null ? Modal.Section17_1_Condition : "";
            command.Parameters.Add("@Section17_1_Comment", SqlDbType.NVarChar).Value = Modal.Section17_1_Comment != null ? Modal.Section17_1_Comment : "";
            command.Parameters.Add("@Section17_2_Condition", SqlDbType.NVarChar).Value = Modal.Section17_2_Condition != null ? Modal.Section17_2_Condition : "";
            command.Parameters.Add("@Section17_2_Comment", SqlDbType.NVarChar).Value = Modal.Section17_2_Comment != null ? Modal.Section17_2_Comment : "";
            command.Parameters.Add("@Section17_3_Condition", SqlDbType.NVarChar).Value = Modal.Section17_3_Condition != null ? Modal.Section17_3_Condition : "";
            command.Parameters.Add("@Section17_3_Comment", SqlDbType.NVarChar).Value = Modal.Section17_3_Comment != null ? Modal.Section17_3_Comment : "";
            command.Parameters.Add("@Section17_4_Condition", SqlDbType.NVarChar).Value = Modal.Section17_4_Condition != null ? Modal.Section17_4_Condition : "";
            command.Parameters.Add("@Section17_4_Comment", SqlDbType.NVarChar).Value = Modal.Section17_4_Comment != null ? Modal.Section17_4_Comment : "";
            command.Parameters.Add("@Section17_5_Condition", SqlDbType.NVarChar).Value = Modal.Section17_5_Condition != null ? Modal.Section17_5_Condition : "";
            command.Parameters.Add("@Section17_5_Comment", SqlDbType.NVarChar).Value = Modal.Section17_5_Comment != null ? Modal.Section17_5_Comment : "";
            command.Parameters.Add("@Section17_6_Condition", SqlDbType.NVarChar).Value = Modal.Section17_6_Condition != null ? Modal.Section17_6_Condition : "";
            command.Parameters.Add("@Section17_6_Comment", SqlDbType.NVarChar).Value = Modal.Section17_6_Comment != null ? Modal.Section17_6_Comment : "";
            command.Parameters.Add("@Section18_1_Condition", SqlDbType.NVarChar).Value = Modal.Section18_1_Condition != null ? Modal.Section18_1_Condition : "";
            command.Parameters.Add("@Section18_1_Comment", SqlDbType.NVarChar).Value = Modal.Section18_1_Comment != null ? Modal.Section18_1_Comment : "";
            command.Parameters.Add("@Section18_2_Condition", SqlDbType.NVarChar).Value = Modal.Section18_2_Condition != null ? Modal.Section18_2_Condition : "";
            command.Parameters.Add("@Section18_2_Comment", SqlDbType.NVarChar).Value = Modal.Section18_2_Comment != null ? Modal.Section18_2_Comment : "";
            command.Parameters.Add("@Section18_3_Condition", SqlDbType.NVarChar).Value = Modal.Section18_3_Condition != null ? Modal.Section18_3_Condition : "";
            command.Parameters.Add("@Section18_3_Comment", SqlDbType.NVarChar).Value = Modal.Section18_3_Comment != null ? Modal.Section18_3_Comment : "";
            command.Parameters.Add("@Section18_4_Condition", SqlDbType.NVarChar).Value = Modal.Section18_4_Condition != null ? Modal.Section18_4_Condition : "";
            command.Parameters.Add("@Section18_4_Comment", SqlDbType.NVarChar).Value = Modal.Section18_4_Comment != null ? Modal.Section18_4_Comment : "";
            command.Parameters.Add("@Section18_5_Condition", SqlDbType.NVarChar).Value = Modal.Section18_5_Condition != null ? Modal.Section18_5_Condition : "";
            command.Parameters.Add("@Section18_5_Comment", SqlDbType.NVarChar).Value = Modal.Section18_5_Comment != null ? Modal.Section18_5_Comment : "";
            command.Parameters.Add("@Section18_6_Condition", SqlDbType.NVarChar).Value = Modal.Section18_6_Condition != null ? Modal.Section18_6_Condition : "";
            command.Parameters.Add("@Section18_6_Comment", SqlDbType.NVarChar).Value = Modal.Section18_6_Comment != null ? Modal.Section18_6_Comment : "";
            command.Parameters.Add("@Section18_7_Condition", SqlDbType.NVarChar).Value = Modal.Section18_7_Condition != null ? Modal.Section18_7_Condition : "";
            command.Parameters.Add("@Section18_7_Comment", SqlDbType.NVarChar).Value = Modal.Section18_7_Comment != null ? Modal.Section18_7_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section18_8_Condition", SqlDbType.NVarChar).Value = Modal.Section18_8_Condition != null ? Modal.Section18_8_Condition : "";
            command.Parameters.Add("@Section18_8_Comment", SqlDbType.NVarChar).Value = Modal.Section18_8_Comment != null ? Modal.Section18_8_Comment : "";
            command.Parameters.Add("@Section18_9_Condition", SqlDbType.NVarChar).Value = Modal.Section18_9_Condition != null ? Modal.Section18_9_Condition : "";
            command.Parameters.Add("@Section18_9_Comment", SqlDbType.NVarChar).Value = Modal.Section18_9_Comment != null ? Modal.Section18_9_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate != null ? Modal.CreatedDate : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@ModifyDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime //Modal.ModifyDate != null ? Modal.ModifyDate : null;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft ?? (object)DBNull.Value;
        }
        public string GetSIRUpdateQuery()
        {
            // RDBJ 01/05/2022 Added isDelete
            string query = @"UPDATE dbo.SuperintendedInspectionReport SET ShipID = @ShipID, FormVersion = @FormVersion,
                           ShipName = @ShipName,Date=@Date,Port=@Port,
                           Master=@Master,Superintended=@Superintended,
                            Section1_1_Condition=@Section1_1_Condition,Section1_1_Comment=@Section1_1_Comment,
                            Section1_2_Condition=@Section1_2_Condition,Section1_2_Comment=@Section1_2_Comment,Section1_3_Condition=@Section1_3_Condition,
                            Section1_3_Comment=@Section1_3_Comment,Section1_4_Condition=@Section1_4_Condition,
                            Section1_4_Comment=@Section1_4_Comment,Section1_5_Condition=@Section1_5_Condition,
                            Section1_5_Comment=@Section1_5_Comment,Section1_6_Condition=@Section1_6_Condition,Section1_6_Comment=@Section1_6_Comment,
                            Section1_7_Condition=@Section1_7_Condition,Section1_7_Comment=@Section1_7_Comment,Section1_8_Condition=@Section1_8_Condition,
                            Section1_8_Comment=@Section1_8_Comment,Section1_9_Condition=@Section1_9_Condition,Section1_9_Comment=@Section1_9_Comment,
                            Section1_10_Condition=@Section1_10_Condition,Section1_10_Comment=@Section1_10_Comment,Section1_11_Condition=@Section1_11_Condition,
                            Section1_11_Comment=@Section1_11_Comment,Section2_1_Condition=@Section2_1_Condition,Section2_1_Comment=@Section2_1_Comment,
                            Section2_2_Condition=@Section2_2_Condition,Section2_2_Comment=@Section2_2_Comment,Section2_3_Condition=@Section2_3_Condition,
                            Section2_3_Comment=@Section2_3_Comment,Section2_4_Condition=@Section2_4_Condition,Section2_4_Comment=@Section2_4_Comment,
                            Section2_5_Condition=@Section2_5_Condition,Section2_5_Comment=@Section2_5_Comment,Section2_6_Condition=@Section2_6_Condition,
                            Section2_6_Comment=@Section2_6_Comment,Section2_7_Condition=@Section2_7_Condition,Section2_7_Comment=@Section2_7_Comment,
                            Section3_1_Condition=@Section3_1_Condition,Section3_1_Comment=@Section3_1_Comment,Section3_2_Condition=@Section3_2_Condition,
                            Section3_2_Comment=@Section3_2_Comment,Section3_3_Condition=@Section3_3_Condition,Section3_3_Comment=@Section3_3_Comment,
                            Section3_4_Condition=@Section3_4_Condition,Section3_4_Comment=@Section3_4_Comment,Section3_5_Condition=@Section3_5_Condition,
                            Section3_5_Comment=@Section3_5_Comment,Section4_1_Condition=@Section4_1_Condition,Section4_1_Comment=@Section4_1_Comment,
                            Section4_2_Condition=@Section4_2_Condition,Section4_2_Comment=@Section4_2_Comment,Section4_3_Condition=@Section4_3_Condition,
                            Section4_3_Comment=@Section4_3_Comment,Section5_1_Condition=@Section5_1_Condition,Section5_1_Comment=@Section5_1_Comment,
                            Section5_6_Condition=@Section5_6_Condition,Section5_6_Comment=@Section5_6_Comment,Section5_8_Condition=@Section5_8_Condition,
                            Section5_8_Comment=@Section5_8_Comment,Section5_9_Condition=@Section5_9_Condition,Section5_9_Comment=@Section5_9_Comment,
                            Section6_1_Condition=@Section6_1_Condition,Section6_1_Comment=@Section6_1_Comment,Section6_2_Condition=@Section6_2_Condition,
                            Section6_2_Comment=@Section6_2_Comment,Section6_3_Condition=@Section6_3_Condition,Section6_3_Comment=@Section6_3_Comment,
                            Section6_4_Condition=@Section6_4_Condition,Section6_4_Comment=@Section6_4_Comment,Section6_5_Condition=@Section6_5_Condition,
                            Section6_5_Comment=@Section6_5_Comment,Section6_6_Condition=@Section6_6_Condition,Section6_6_Comment=@Section6_6_Comment,
                            Section6_7_Condition=@Section6_7_Condition,Section6_7_Comment=@Section6_7_Comment,Section6_8_Condition=@Section6_8_Condition,
                            Section6_8_Comment=@Section6_8_Comment,Section7_1_Condition=@Section7_1_Condition,Section7_1_Comment=@Section7_1_Comment,
                            Section7_2_Condition=@Section7_2_Condition,Section7_2_Comment=@Section7_2_Comment,Section7_3_Condition=@Section7_3_Condition,
                            Section7_3_Comment=@Section7_3_Comment,Section7_4_Condition=@Section7_4_Condition,Section7_4_Comment=@Section7_4_Comment,
                            Section7_5_Condition=@Section7_5_Condition,Section7_5_Comment=@Section7_5_Comment,Section7_6_Condition=@Section7_6_Condition,
                            Section7_6_Comment=@Section7_6_Comment,Section8_1_Condition=@Section8_1_Condition,Section8_1_Comment=@Section8_1_Comment,
                            Section8_2_Condition=@Section8_2_Condition,Section8_2_Comment=@Section8_2_Comment,Section8_3_Condition=@Section8_3_Condition,
                            Section8_3_Comment=@Section8_3_Comment,Section8_4_Condition=@Section8_4_Condition,Section8_4_Comment=@Section8_4_Comment,
                            Section8_5_Condition=@Section8_5_Condition,Section8_5_Comment=@Section8_5_Comment,Section8_6_Condition=@Section8_6_Condition,
                            Section8_6_Comment=@Section8_6_Comment,Section8_7_Condition=@Section8_7_Condition,Section8_7_Comment=@Section8_7_Comment,
                            Section8_8_Condition=@Section8_8_Condition,Section8_8_Comment=@Section8_8_Comment,Section8_9_Condition=@Section8_9_Condition,
                            Section8_9_Comment=@Section8_9_Comment,Section8_10_Condition=@Section8_10_Condition,Section8_10_Comment=@Section8_10_Comment,
                            Section8_11_Condition=@Section8_11_Condition,Section8_11_Comment=@Section8_11_Comment,Section8_12_Condition=@Section8_12_Condition,
                            Section8_12_Comment=@Section8_12_Comment,Section8_13_Condition=@Section8_13_Condition,Section8_13_Comment=@Section8_13_Comment,
                            Section8_14_Condition=@Section8_14_Condition,Section8_14_Comment=@Section8_14_Comment,Section8_15_Condition=@Section8_15_Condition,
                            Section8_15_Comment=@Section8_15_Comment,Section8_16_Condition=@Section8_16_Condition,Section8_16_Comment=@Section8_16_Comment,
                            Section8_17_Condition=@Section8_17_Condition,Section8_17_Comment=@Section8_17_Comment,Section8_18_Condition=@Section8_18_Condition,
                            Section8_18_Comment=@Section8_18_Comment,Section8_19_Condition=@Section8_19_Condition,Section8_19_Comment=@Section8_19_Comment,
                            Section8_20_Condition=@Section8_20_Condition,Section8_20_Comment=@Section8_20_Comment,Section8_21_Condition=@Section8_21_Condition,
                            Section8_21_Comment=@Section8_21_Comment,Section8_22_Condition=@Section8_22_Condition,Section8_22_Comment=@Section8_22_Comment,
                            Section8_23_Condition=@Section8_23_Condition,Section8_23_Comment=@Section8_23_Comment,Section8_24_Condition=@Section8_24_Condition,
                            Section8_24_Comment=@Section8_24_Comment,Section8_25_Condition=@Section8_25_Condition,Section8_25_Comment=@Section8_25_Comment,
                            Section9_1_Condition=@Section9_1_Condition,Section9_1_Comment=@Section9_1_Comment,Section9_2_Condition=@Section9_2_Condition,
                            Section9_2_Comment=@Section9_2_Comment,Section9_3_Condition=@Section9_3_Condition,Section9_3_Comment=@Section9_3_Comment,
                            Section9_4_Condition=@Section9_4_Condition,Section9_4_Comment=@Section9_4_Comment,Section9_5_Condition=@Section9_5_Condition,
                            Section9_5_Comment=@Section9_5_Comment,Section9_6_Condition=@Section9_6_Condition,Section9_6_Comment=@Section9_6_Comment,
                            Section9_7_Condition=@Section9_7_Condition,Section9_7_Comment=@Section9_7_Comment,Section9_8_Condition=@Section9_8_Condition,
                            Section9_8_Comment=@Section9_8_Comment,Section9_9_Condition=@Section9_9_Condition,Section9_9_Comment=@Section9_9_Comment,
                            Section9_10_Condition=@Section9_10_Condition,Section9_10_Comment=@Section9_10_Comment,Section9_11_Condition=@Section9_11_Condition,
                            Section9_11_Comment=@Section9_11_Comment,Section9_12_Condition=@Section9_12_Condition,Section9_12_Comment=@Section9_12_Comment,
                            Section9_13_Condition=@Section9_13_Condition,Section9_13_Comment=@Section9_13_Comment,Section9_14_Condition=@Section9_14_Condition,
                            Section9_14_Comment=@Section9_14_Comment,Section9_15_Condition=@Section9_15_Condition,Section9_15_Comment=@Section9_15_Comment,
                            Section9_16_Condition=@Section9_16_Condition,Section9_16_Comment=@Section9_16_Comment,
                            Section9_17_Condition=@Section9_17_Condition,Section9_17_Comment=@Section9_17_Comment,                            
                            Section10_1_Condition=@Section10_1_Condition,Section10_1_Comment=@Section10_1_Comment,Section10_2_Condition=@Section10_2_Condition,
                            Section10_2_Comment=@Section10_2_Comment,Section10_3_Condition=@Section10_3_Condition,Section10_3_Comment=@Section10_3_Comment,
                            Section10_4_Condition=@Section10_4_Condition,Section10_4_Comment=@Section10_4_Comment,Section10_5_Condition=@Section10_5_Condition,
                            Section10_5_Comment=@Section10_5_Comment,Section10_6_Condition=@Section10_6_Condition,Section10_6_Comment=@Section10_6_Comment,
                            Section10_7_Condition=@Section10_7_Condition,Section10_7_Comment=@Section10_7_Comment,Section10_8_Condition=@Section10_8_Condition,
                            Section10_8_Comment=@Section10_8_Comment,Section10_9_Condition=@Section10_9_Condition,Section10_9_Comment=@Section10_9_Comment,
                            Section10_10_Condition=@Section10_10_Condition,Section10_10_Comment=@Section10_10_Comment,Section10_11_Condition=@Section10_11_Condition,
                            Section10_11_Comment=@Section10_11_Comment,Section10_12_Condition=@Section10_12_Condition,Section10_12_Comment=@Section10_12_Comment,
                            Section10_13_Condition=@Section10_13_Condition,Section10_13_Comment=@Section10_13_Comment,Section10_14_Condition=@Section10_14_Condition,
                            Section10_14_Comment=@Section10_14_Comment,Section10_15_Condition=@Section10_15_Condition,Section10_15_Comment=@Section10_15_Comment,
                            Section10_16_Condition=@Section10_16_Condition,Section10_16_Comment=@Section10_16_Comment,Section11_1_Condition=@Section11_1_Condition,
                            Section11_1_Comment=@Section11_1_Comment,Section11_2_Condition=@Section11_2_Condition,Section11_2_Comment=@Section11_2_Comment,
                            Section11_3_Condition=@Section11_3_Condition,Section11_3_Comment=@Section11_3_Comment,Section11_4_Condition=@Section11_4_Condition,
                            Section11_4_Comment=@Section11_4_Comment,Section11_5_Condition=@Section11_5_Condition,Section11_5_Comment=@Section11_5_Comment,
                            Section11_6_Condition=@Section11_6_Condition,Section11_6_Comment=@Section11_6_Comment,Section11_7_Condition=@Section11_7_Condition,
                            Section11_7_Comment=@Section11_7_Comment,Section11_8_Condition=@Section11_8_Condition,Section11_8_Comment=@Section11_8_Comment,
                            Section12_1_Condition=@Section12_1_Condition,Section12_1_Comment=@Section12_1_Comment,Section12_2_Condition=@Section12_2_Condition,
                            Section12_2_Comment=@Section12_2_Comment,Section12_3_Condition=@Section12_3_Condition,Section12_3_Comment=@Section12_3_Comment,
                            Section12_4_Condition=@Section12_4_Condition,Section12_4_Comment=@Section12_4_Comment,Section12_5_Condition=@Section12_5_Condition,
                            Section12_5_Comment=@Section12_5_Comment,Section12_6_Condition=@Section12_6_Condition,Section12_6_Comment=@Section12_6_Comment,
                            Section13_1_Condition=@Section13_1_Condition,Section13_1_Comment=@Section13_1_Comment,Section13_2_Condition=@Section13_2_Condition,
                            Section13_2_Comment=@Section13_2_Comment,Section13_3_Condition=@Section13_3_Condition,Section13_3_Comment=@Section13_3_Comment,
                            Section13_4_Condition=@Section13_4_Condition,Section13_4_Comment=@Section13_4_Comment,Section14_1_Condition=@Section14_1_Condition,
                            Section14_1_Comment=@Section14_1_Comment,Section14_2_Condition=@Section14_2_Condition,Section14_2_Comment=@Section14_2_Comment,
                            Section14_3_Condition=@Section14_3_Condition,Section14_3_Comment=@Section14_3_Comment,Section14_4_Condition=@Section14_4_Condition,
                            Section14_4_Comment=@Section14_4_Comment,Section14_5_Condition=@Section14_5_Condition,Section14_5_Comment=@Section14_5_Comment,
                            Section14_6_Condition=@Section14_6_Condition,Section14_6_Comment=@Section14_6_Comment,Section14_7_Condition=@Section14_7_Condition,
                            Section14_7_Comment=@Section14_7_Comment,Section14_8_Condition=@Section14_8_Condition,Section14_8_Comment=@Section14_8_Comment,
                            Section14_9_Condition=@Section14_9_Condition,Section14_9_Comment=@Section14_9_Comment,Section14_10_Condition=@Section14_10_Condition,
                            Section14_10_Comment=@Section14_10_Comment,Section14_11_Condition=@Section14_11_Condition,Section14_11_Comment=@Section14_11_Comment,
                            Section14_12_Condition=@Section14_12_Condition,Section14_12_Comment=@Section14_12_Comment,Section14_13_Condition=@Section14_13_Condition,
                            Section14_13_Comment=@Section14_13_Comment,Section14_14_Condition=@Section14_14_Condition,Section14_14_Comment=@Section14_14_Comment,
                            Section14_15_Condition=@Section14_15_Condition,Section14_15_Comment=@Section14_15_Comment,Section14_16_Condition=@Section14_16_Condition,
                            Section14_16_Comment=@Section14_16_Comment,Section14_17_Condition=@Section14_17_Condition,Section14_17_Comment=@Section14_17_Comment,
                            Section14_18_Condition=@Section14_18_Condition,Section14_18_Comment=@Section14_18_Comment,Section14_19_Condition=@Section14_19_Condition,
                            Section14_19_Comment=@Section14_19_Comment,Section14_20_Condition=@Section14_20_Condition,Section14_20_Comment=@Section14_20_Comment,
                            Section14_21_Condition=@Section14_21_Condition,Section14_21_Comment=@Section14_21_Comment,Section14_22_Condition=@Section14_22_Condition,
                            Section14_22_Comment=@Section14_22_Comment,Section14_23_Condition=@Section14_23_Condition,Section14_23_Comment=@Section14_23_Comment,
                            Section14_24_Condition=@Section14_24_Condition,Section14_24_Comment=@Section14_24_Comment,Section14_25_Condition=@Section14_25_Condition,
                            Section14_25_Comment=@Section14_25_Comment,Section15_1_Condition=@Section15_1_Condition,Section15_1_Comment=@Section15_1_Comment,
                            Section15_2_Condition=@Section15_2_Condition,Section15_2_Comment=@Section15_2_Comment,Section15_3_Condition=@Section15_3_Condition,
                            Section15_3_Comment=@Section15_3_Comment,Section15_4_Condition=@Section15_4_Condition,Section15_4_Comment=@Section15_4_Comment,
                            Section15_5_Condition=@Section15_5_Condition,Section15_5_Comment=@Section15_5_Comment,Section15_6_Condition=@Section15_6_Condition,
                            Section15_6_Comment=@Section15_6_Comment,Section15_7_Condition=@Section15_7_Condition,Section15_7_Comment=@Section15_7_Comment,
                            Section15_8_Condition=@Section15_8_Condition,Section15_8_Comment=@Section15_8_Comment,Section15_9_Condition=@Section15_9_Condition,
                            Section15_9_Comment=@Section15_9_Comment,Section15_10_Condition=@Section15_10_Condition,Section15_10_Comment=@Section15_10_Comment,
                            Section15_11_Condition=@Section15_11_Condition,Section15_11_Comment=@Section15_11_Comment,Section15_12_Condition=@Section15_12_Condition,
                            Section15_12_Comment=@Section15_12_Comment,Section15_13_Condition=@Section15_13_Condition,Section15_13_Comment=@Section15_13_Comment,
                            Section15_14_Condition=@Section15_14_Condition,Section15_14_Comment=@Section15_14_Comment,Section15_15_Condition=@Section15_15_Condition,
                            Section15_15_Comment=@Section15_15_Comment,Section16_1_Condition=@Section16_1_Condition,Section16_1_Comment=@Section16_1_Comment,
                            Section16_2_Condition=@Section16_2_Condition,Section16_2_Comment=@Section16_2_Comment,Section16_3_Condition=@Section16_3_Condition,
                            Section16_3_Comment=@Section16_3_Comment,Section16_4_Condition=@Section16_4_Condition,Section16_4_Comment=@Section16_4_Comment,
                            Section17_1_Condition=@Section17_1_Condition,Section17_1_Comment=@Section17_1_Comment,Section17_2_Condition=@Section17_2_Condition,
                            Section17_2_Comment=@Section17_2_Comment,Section17_3_Condition=@Section17_3_Condition,Section17_3_Comment=@Section17_3_Comment,
                            Section17_4_Condition=@Section17_4_Condition,Section17_4_Comment=@Section17_4_Comment,Section17_5_Condition=@Section17_5_Condition,
                            Section17_5_Comment=@Section17_5_Comment,Section17_6_Condition=@Section17_6_Condition,Section17_6_Comment=@Section17_6_Comment,
                            Section18_1_Condition=@Section18_1_Condition,Section18_1_Comment=@Section18_1_Comment,Section18_2_Condition=@Section18_2_Condition,
                            Section18_2_Comment=@Section18_2_Comment,Section18_3_Condition=@Section18_3_Condition,Section18_4_Comment=@Section18_4_Comment,
                            Section18_5_Condition=@Section18_5_Condition,Section18_5_Comment=@Section18_5_Comment,Section18_6_Condition=@Section18_6_Condition,
                            Section18_6_Comment=@Section18_6_Comment,Section18_7_Condition=@Section18_7_Condition,Section18_7_Comment=@Section18_7_Comment,
                            Section18_8_Condition=@Section18_8_Condition,Section18_8_Comment=@Section18_8_Comment,
                            Section18_9_Condition=@Section18_9_Condition,Section18_9_Comment=@Section18_9_Comment,                            
                            IsSynced=@IsSynced,ModifyDate=@ModifyDate,SavedAsDraft=@SavedAsDraft, isDelete = @isDelete WHERE UniqueFormID = @UniqueFormID";
            return query;
        }
        public void SIRUpdateCMD(SuperintendedInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete ?? (object)DBNull.Value; // RDBJ 01/05/2022
            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date != null ? Modal.Date : null;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port == null ? string.Empty : Modal.Port; // RDBJ 01/21/2022
            command.Parameters.Add("@Master", SqlDbType.NVarChar).Value = Modal.Master != null ? Modal.Master : "";
            command.Parameters.Add("@Superintended", SqlDbType.NVarChar).Value = Modal.Superintended != null ? Modal.Superintended : "";
            command.Parameters.Add("@Section1_1_Condition", SqlDbType.NVarChar).Value = Modal.Section1_1_Condition != null ? Modal.Section1_1_Condition : "";
            command.Parameters.Add("@Section1_1_Comment", SqlDbType.NVarChar).Value = Modal.Section1_1_Comment != null ? Modal.Section1_1_Comment : "";
            command.Parameters.Add("@Section1_2_Condition", SqlDbType.NVarChar).Value = Modal.Section1_2_Condition != null ? Modal.Section1_2_Condition : "";
            command.Parameters.Add("@Section1_2_Comment", SqlDbType.NVarChar).Value = Modal.Section1_2_Comment != null ? Modal.Section1_2_Comment : "";
            command.Parameters.Add("@Section1_3_Condition", SqlDbType.NVarChar).Value = Modal.Section1_3_Condition != null ? Modal.Section1_3_Condition : "";
            command.Parameters.Add("@Section1_3_Comment", SqlDbType.NVarChar).Value = Modal.Section1_3_Comment != null ? Modal.Section1_3_Comment : "";
            command.Parameters.Add("@Section1_4_Condition", SqlDbType.NVarChar).Value = Modal.Section1_4_Condition != null ? Modal.Section1_4_Condition : "";
            command.Parameters.Add("@Section1_4_Comment", SqlDbType.NVarChar).Value = Modal.Section1_4_Comment != null ? Modal.Section1_4_Comment : "";
            command.Parameters.Add("@Section1_5_Condition", SqlDbType.NVarChar).Value = Modal.Section1_5_Condition != null ? Modal.Section1_5_Condition : "";
            command.Parameters.Add("@Section1_5_Comment", SqlDbType.NVarChar).Value = Modal.Section1_5_Comment != null ? Modal.Section1_5_Comment : "";
            command.Parameters.Add("@Section1_6_Condition", SqlDbType.NVarChar).Value = Modal.Section1_6_Condition != null ? Modal.Section1_6_Condition : "";
            command.Parameters.Add("@Section1_6_Comment", SqlDbType.NVarChar).Value = Modal.Section1_6_Comment != null ? Modal.Section1_6_Comment : "";
            command.Parameters.Add("@Section1_7_Condition", SqlDbType.NVarChar).Value = Modal.Section1_7_Condition != null ? Modal.Section1_7_Condition : "";
            command.Parameters.Add("@Section1_7_Comment", SqlDbType.NVarChar).Value = Modal.Section1_7_Comment != null ? Modal.Section1_7_Comment : "";
            command.Parameters.Add("@Section1_8_Condition", SqlDbType.NVarChar).Value = Modal.Section1_8_Condition != null ? Modal.Section1_8_Condition : "";
            command.Parameters.Add("@Section1_8_Comment", SqlDbType.NVarChar).Value = Modal.Section1_8_Comment != null ? Modal.Section1_8_Comment : "";
            command.Parameters.Add("@Section1_9_Condition", SqlDbType.NVarChar).Value = Modal.Section1_9_Condition != null ? Modal.Section1_9_Condition : "";
            command.Parameters.Add("@Section1_9_Comment", SqlDbType.NVarChar).Value = Modal.Section1_9_Comment != null ? Modal.Section1_9_Comment : "";
            command.Parameters.Add("@Section1_10_Condition", SqlDbType.NVarChar).Value = Modal.Section1_10_Condition != null ? Modal.Section1_10_Condition : "";
            command.Parameters.Add("@Section1_10_Comment", SqlDbType.NVarChar).Value = Modal.Section1_10_Comment != null ? Modal.Section1_10_Comment : "";
            command.Parameters.Add("@Section1_11_Condition", SqlDbType.NVarChar).Value = Modal.Section1_11_Condition != null ? Modal.Section1_11_Condition : "";
            command.Parameters.Add("@Section1_11_Comment", SqlDbType.NVarChar).Value = Modal.Section1_11_Comment != null ? Modal.Section1_11_Comment : "";
            command.Parameters.Add("@Section2_1_Condition", SqlDbType.NVarChar).Value = Modal.Section2_1_Condition != null ? Modal.Section2_1_Condition : "";
            command.Parameters.Add("@Section2_1_Comment", SqlDbType.NVarChar).Value = Modal.Section2_1_Comment != null ? Modal.Section2_1_Comment : "";
            command.Parameters.Add("@Section2_2_Condition", SqlDbType.NVarChar).Value = Modal.Section2_2_Condition != null ? Modal.Section2_2_Condition : "";
            command.Parameters.Add("@Section2_2_Comment", SqlDbType.NVarChar).Value = Modal.Section2_2_Comment != null ? Modal.Section2_2_Comment : "";
            command.Parameters.Add("@Section2_3_Condition", SqlDbType.NVarChar).Value = Modal.Section2_3_Condition != null ? Modal.Section2_3_Condition : "";
            command.Parameters.Add("@Section2_3_Comment", SqlDbType.NVarChar).Value = Modal.Section2_3_Comment != null ? Modal.Section2_3_Comment : "";
            command.Parameters.Add("@Section2_4_Condition", SqlDbType.NVarChar).Value = Modal.Section2_4_Condition != null ? Modal.Section2_4_Condition : "";
            command.Parameters.Add("@Section2_4_Comment", SqlDbType.NVarChar).Value = Modal.Section2_4_Comment != null ? Modal.Section2_4_Comment : "";
            command.Parameters.Add("@Section2_5_Condition", SqlDbType.NVarChar).Value = Modal.Section2_5_Condition != null ? Modal.Section2_5_Condition : "";
            command.Parameters.Add("@Section2_5_Comment", SqlDbType.NVarChar).Value = Modal.Section2_5_Comment != null ? Modal.Section2_5_Comment : "";
            command.Parameters.Add("@Section2_6_Condition", SqlDbType.NVarChar).Value = Modal.Section2_6_Condition != null ? Modal.Section2_6_Condition : "";
            command.Parameters.Add("@Section2_6_Comment", SqlDbType.NVarChar).Value = Modal.Section2_6_Comment != null ? Modal.Section2_6_Comment : "";
            command.Parameters.Add("@Section2_7_Condition", SqlDbType.NVarChar).Value = Modal.Section2_7_Condition != null ? Modal.Section2_7_Condition : "";
            command.Parameters.Add("@Section2_7_Comment", SqlDbType.NVarChar).Value = Modal.Section2_7_Comment != null ? Modal.Section2_7_Comment : "";
            command.Parameters.Add("@Section3_1_Condition", SqlDbType.NVarChar).Value = Modal.Section3_1_Condition != null ? Modal.Section3_1_Condition : "";
            command.Parameters.Add("@Section3_1_Comment", SqlDbType.NVarChar).Value = Modal.Section3_1_Comment != null ? Modal.Section3_1_Comment : "";
            command.Parameters.Add("@Section3_2_Condition", SqlDbType.NVarChar).Value = Modal.Section3_2_Condition != null ? Modal.Section3_2_Condition : "";
            command.Parameters.Add("@Section3_2_Comment", SqlDbType.NVarChar).Value = Modal.Section3_2_Comment != null ? Modal.Section3_2_Comment : "";
            command.Parameters.Add("@Section3_3_Condition", SqlDbType.NVarChar).Value = Modal.Section3_3_Condition != null ? Modal.Section3_3_Condition : "";
            command.Parameters.Add("@Section3_3_Comment", SqlDbType.NVarChar).Value = Modal.Section3_3_Comment != null ? Modal.Section3_3_Comment : "";
            command.Parameters.Add("@Section3_4_Condition", SqlDbType.NVarChar).Value = Modal.Section3_4_Condition != null ? Modal.Section3_4_Condition : "";
            command.Parameters.Add("@Section3_4_Comment", SqlDbType.NVarChar).Value = Modal.Section3_4_Comment != null ? Modal.Section3_4_Comment : "";
            command.Parameters.Add("@Section3_5_Condition", SqlDbType.NVarChar).Value = Modal.Section3_5_Condition != null ? Modal.Section3_5_Condition : "";
            command.Parameters.Add("@Section3_5_Comment", SqlDbType.NVarChar).Value = Modal.Section3_5_Comment != null ? Modal.Section3_5_Comment : "";
            command.Parameters.Add("@Section4_1_Condition", SqlDbType.NVarChar).Value = Modal.Section4_1_Condition != null ? Modal.Section4_1_Condition : "";
            command.Parameters.Add("@Section4_1_Comment", SqlDbType.NVarChar).Value = Modal.Section4_1_Comment != null ? Modal.Section4_1_Comment : "";
            command.Parameters.Add("@Section4_2_Condition", SqlDbType.NVarChar).Value = Modal.Section4_2_Condition != null ? Modal.Section4_2_Condition : "";
            command.Parameters.Add("@Section4_2_Comment", SqlDbType.NVarChar).Value = Modal.Section4_2_Comment != null ? Modal.Section4_2_Comment : "";
            command.Parameters.Add("@Section4_3_Condition", SqlDbType.NVarChar).Value = Modal.Section4_3_Condition != null ? Modal.Section4_3_Condition : "";
            command.Parameters.Add("@Section4_3_Comment", SqlDbType.NVarChar).Value = Modal.Section4_3_Comment != null ? Modal.Section4_3_Comment : "";
            command.Parameters.Add("@Section5_1_Condition", SqlDbType.NVarChar).Value = Modal.Section5_1_Condition != null ? Modal.Section5_1_Condition : "";
            command.Parameters.Add("@Section5_1_Comment", SqlDbType.NVarChar).Value = Modal.Section5_1_Comment != null ? Modal.Section5_1_Comment : "";
            command.Parameters.Add("@Section5_6_Condition", SqlDbType.NVarChar).Value = Modal.Section5_6_Condition != null ? Modal.Section5_6_Condition : "";
            command.Parameters.Add("@Section5_6_Comment", SqlDbType.NVarChar).Value = Modal.Section5_6_Comment != null ? Modal.Section5_6_Comment : "";
            command.Parameters.Add("@Section5_8_Condition", SqlDbType.NVarChar).Value = Modal.Section5_8_Condition != null ? Modal.Section5_8_Condition : "";
            command.Parameters.Add("@Section5_8_Comment", SqlDbType.NVarChar).Value = Modal.Section5_8_Comment != null ? Modal.Section5_8_Comment : "";
            command.Parameters.Add("@Section5_9_Condition", SqlDbType.NVarChar).Value = Modal.Section5_9_Condition != null ? Modal.Section5_9_Condition : "";
            command.Parameters.Add("@Section5_9_Comment", SqlDbType.NVarChar).Value = Modal.Section5_9_Comment != null ? Modal.Section5_9_Comment : "";
            command.Parameters.Add("@Section6_1_Condition", SqlDbType.NVarChar).Value = Modal.Section6_1_Condition != null ? Modal.Section6_1_Condition : "";
            command.Parameters.Add("@Section6_1_Comment", SqlDbType.NVarChar).Value = Modal.Section6_1_Comment != null ? Modal.Section6_1_Comment : "";
            command.Parameters.Add("@Section6_2_Condition", SqlDbType.NVarChar).Value = Modal.Section6_2_Condition != null ? Modal.Section6_2_Condition : "";
            command.Parameters.Add("@Section6_2_Comment", SqlDbType.NVarChar).Value = Modal.Section6_2_Comment != null ? Modal.Section6_2_Comment : "";
            command.Parameters.Add("@Section6_3_Condition", SqlDbType.NVarChar).Value = Modal.Section6_3_Condition != null ? Modal.Section6_3_Condition : "";
            command.Parameters.Add("@Section6_3_Comment", SqlDbType.NVarChar).Value = Modal.Section6_3_Comment != null ? Modal.Section6_3_Comment : "";
            command.Parameters.Add("@Section6_4_Condition", SqlDbType.NVarChar).Value = Modal.Section6_4_Condition != null ? Modal.Section6_4_Condition : "";
            command.Parameters.Add("@Section6_4_Comment", SqlDbType.NVarChar).Value = Modal.Section6_4_Comment != null ? Modal.Section6_4_Comment : "";
            command.Parameters.Add("@Section6_5_Condition", SqlDbType.NVarChar).Value = Modal.Section6_5_Condition != null ? Modal.Section6_5_Condition : "";
            command.Parameters.Add("@Section6_5_Comment", SqlDbType.NVarChar).Value = Modal.Section6_5_Comment != null ? Modal.Section6_5_Comment : "";
            command.Parameters.Add("@Section6_6_Condition", SqlDbType.NVarChar).Value = Modal.Section6_6_Condition != null ? Modal.Section6_6_Condition : "";
            command.Parameters.Add("@Section6_6_Comment", SqlDbType.NVarChar).Value = Modal.Section6_6_Comment != null ? Modal.Section6_6_Comment : "";
            command.Parameters.Add("@Section6_7_Condition", SqlDbType.NVarChar).Value = Modal.Section6_7_Condition != null ? Modal.Section6_7_Condition : "";
            command.Parameters.Add("@Section6_7_Comment", SqlDbType.NVarChar).Value = Modal.Section6_7_Comment != null ? Modal.Section6_7_Comment : "";
            command.Parameters.Add("@Section6_8_Condition", SqlDbType.NVarChar).Value = Modal.Section6_8_Condition != null ? Modal.Section6_8_Condition : "";
            command.Parameters.Add("@Section6_8_Comment", SqlDbType.NVarChar).Value = Modal.Section6_8_Comment != null ? Modal.Section6_8_Comment : "";
            command.Parameters.Add("@Section7_1_Condition", SqlDbType.NVarChar).Value = Modal.Section7_1_Condition != null ? Modal.Section7_1_Condition : "";
            command.Parameters.Add("@Section7_1_Comment", SqlDbType.NVarChar).Value = Modal.Section7_1_Comment != null ? Modal.Section7_1_Comment : "";
            command.Parameters.Add("@Section7_2_Condition", SqlDbType.NVarChar).Value = Modal.Section7_2_Condition != null ? Modal.Section7_2_Condition : "";
            command.Parameters.Add("@Section7_2_Comment", SqlDbType.NVarChar).Value = Modal.Section7_2_Comment != null ? Modal.Section7_2_Comment : "";
            command.Parameters.Add("@Section7_3_Condition", SqlDbType.NVarChar).Value = Modal.Section7_3_Condition != null ? Modal.Section7_3_Condition : "";
            command.Parameters.Add("@Section7_3_Comment", SqlDbType.NVarChar).Value = Modal.Section7_3_Comment != null ? Modal.Section7_3_Comment : "";
            command.Parameters.Add("@Section7_4_Condition", SqlDbType.NVarChar).Value = Modal.Section7_4_Condition != null ? Modal.Section7_4_Condition : "";
            command.Parameters.Add("@Section7_4_Comment", SqlDbType.NVarChar).Value = Modal.Section7_4_Comment != null ? Modal.Section7_4_Comment : "";
            command.Parameters.Add("@Section7_5_Condition", SqlDbType.NVarChar).Value = Modal.Section7_5_Condition != null ? Modal.Section7_5_Condition : "";
            command.Parameters.Add("@Section7_5_Comment", SqlDbType.NVarChar).Value = Modal.Section7_5_Comment != null ? Modal.Section7_5_Comment : "";
            command.Parameters.Add("@Section7_6_Condition", SqlDbType.NVarChar).Value = Modal.Section7_6_Condition != null ? Modal.Section7_6_Condition : "";
            command.Parameters.Add("@Section7_6_Comment", SqlDbType.NVarChar).Value = Modal.Section7_6_Comment != null ? Modal.Section7_6_Comment : "";
            command.Parameters.Add("@Section8_1_Condition", SqlDbType.NVarChar).Value = Modal.Section8_1_Condition != null ? Modal.Section8_1_Condition : "";
            command.Parameters.Add("@Section8_1_Comment", SqlDbType.NVarChar).Value = Modal.Section8_1_Comment != null ? Modal.Section8_1_Comment : "";
            command.Parameters.Add("@Section8_2_Condition", SqlDbType.NVarChar).Value = Modal.Section8_2_Condition != null ? Modal.Section8_2_Condition : "";
            command.Parameters.Add("@Section8_2_Comment", SqlDbType.NVarChar).Value = Modal.Section8_2_Comment != null ? Modal.Section8_2_Comment : "";
            command.Parameters.Add("@Section8_3_Condition", SqlDbType.NVarChar).Value = Modal.Section8_3_Condition != null ? Modal.Section8_3_Condition : "";
            command.Parameters.Add("@Section8_3_Comment", SqlDbType.NVarChar).Value = Modal.Section8_3_Comment != null ? Modal.Section8_3_Comment : "";
            command.Parameters.Add("@Section8_4_Condition", SqlDbType.NVarChar).Value = Modal.Section8_4_Condition != null ? Modal.Section8_4_Condition : "";
            command.Parameters.Add("@Section8_4_Comment", SqlDbType.NVarChar).Value = Modal.Section8_4_Comment != null ? Modal.Section8_4_Comment : "";
            command.Parameters.Add("@Section8_5_Condition", SqlDbType.NVarChar).Value = Modal.Section8_5_Condition != null ? Modal.Section8_5_Condition : "";
            command.Parameters.Add("@Section8_5_Comment", SqlDbType.NVarChar).Value = Modal.Section8_5_Comment != null ? Modal.Section8_5_Comment : "";
            command.Parameters.Add("@Section8_6_Condition", SqlDbType.NVarChar).Value = Modal.Section8_6_Condition != null ? Modal.Section8_6_Condition : "";
            command.Parameters.Add("@Section8_6_Comment", SqlDbType.NVarChar).Value = Modal.Section8_6_Comment != null ? Modal.Section8_6_Comment : "";
            command.Parameters.Add("@Section8_7_Condition", SqlDbType.NVarChar).Value = Modal.Section8_7_Condition != null ? Modal.Section8_7_Condition : "";
            command.Parameters.Add("@Section8_7_Comment", SqlDbType.NVarChar).Value = Modal.Section8_7_Comment != null ? Modal.Section8_7_Comment : "";
            command.Parameters.Add("@Section8_8_Condition", SqlDbType.NVarChar).Value = Modal.Section8_8_Condition != null ? Modal.Section8_8_Condition : "";
            command.Parameters.Add("@Section8_8_Comment", SqlDbType.NVarChar).Value = Modal.Section8_8_Comment != null ? Modal.Section8_8_Comment : "";
            command.Parameters.Add("@Section8_9_Condition", SqlDbType.NVarChar).Value = Modal.Section8_9_Condition != null ? Modal.Section8_9_Condition : "";
            command.Parameters.Add("@Section8_9_Comment", SqlDbType.NVarChar).Value = Modal.Section8_9_Comment != null ? Modal.Section8_9_Comment : "";
            command.Parameters.Add("@Section8_10_Condition", SqlDbType.NVarChar).Value = Modal.Section8_10_Condition != null ? Modal.Section8_10_Condition : "";
            command.Parameters.Add("@Section8_10_Comment", SqlDbType.NVarChar).Value = Modal.Section8_10_Comment != null ? Modal.Section8_10_Comment : "";
            command.Parameters.Add("@Section8_11_Condition", SqlDbType.NVarChar).Value = Modal.Section8_11_Condition != null ? Modal.Section8_11_Condition : "";
            command.Parameters.Add("@Section8_11_Comment", SqlDbType.NVarChar).Value = Modal.Section8_11_Comment != null ? Modal.Section8_11_Comment : "";
            command.Parameters.Add("@Section8_12_Condition", SqlDbType.NVarChar).Value = Modal.Section8_12_Condition != null ? Modal.Section8_12_Condition : "";
            command.Parameters.Add("@Section8_12_Comment", SqlDbType.NVarChar).Value = Modal.Section8_12_Comment != null ? Modal.Section8_12_Comment : "";
            command.Parameters.Add("@Section8_13_Condition", SqlDbType.NVarChar).Value = Modal.Section8_13_Condition != null ? Modal.Section8_13_Condition : "";
            command.Parameters.Add("@Section8_13_Comment", SqlDbType.NVarChar).Value = Modal.Section8_13_Comment != null ? Modal.Section8_13_Comment : "";
            command.Parameters.Add("@Section8_14_Condition", SqlDbType.NVarChar).Value = Modal.Section8_14_Condition != null ? Modal.Section8_14_Condition : "";
            command.Parameters.Add("@Section8_14_Comment", SqlDbType.NVarChar).Value = Modal.Section8_14_Comment != null ? Modal.Section8_14_Comment : "";
            command.Parameters.Add("@Section8_15_Condition", SqlDbType.NVarChar).Value = Modal.Section8_15_Condition != null ? Modal.Section8_15_Condition : "";
            command.Parameters.Add("@Section8_15_Comment", SqlDbType.NVarChar).Value = Modal.Section8_15_Comment != null ? Modal.Section8_15_Comment : "";
            command.Parameters.Add("@Section8_16_Condition", SqlDbType.NVarChar).Value = Modal.Section8_16_Condition != null ? Modal.Section8_16_Condition : "";
            command.Parameters.Add("@Section8_16_Comment", SqlDbType.NVarChar).Value = Modal.Section8_16_Comment != null ? Modal.Section8_16_Comment : "";
            command.Parameters.Add("@Section8_17_Condition", SqlDbType.NVarChar).Value = Modal.Section8_17_Condition != null ? Modal.Section8_17_Condition : "";
            command.Parameters.Add("@Section8_17_Comment", SqlDbType.NVarChar).Value = Modal.Section8_17_Comment != null ? Modal.Section8_17_Comment : "";
            command.Parameters.Add("@Section8_18_Condition", SqlDbType.NVarChar).Value = Modal.Section8_18_Condition != null ? Modal.Section8_18_Condition : "";
            command.Parameters.Add("@Section8_18_Comment", SqlDbType.NVarChar).Value = Modal.Section8_18_Comment != null ? Modal.Section8_18_Comment : "";
            command.Parameters.Add("@Section8_19_Condition", SqlDbType.NVarChar).Value = Modal.Section8_19_Condition != null ? Modal.Section8_19_Condition : "";
            command.Parameters.Add("@Section8_19_Comment", SqlDbType.NVarChar).Value = Modal.Section8_19_Comment != null ? Modal.Section8_19_Comment : "";
            command.Parameters.Add("@Section8_20_Condition", SqlDbType.NVarChar).Value = Modal.Section8_20_Condition != null ? Modal.Section8_20_Condition : "";
            command.Parameters.Add("@Section8_20_Comment", SqlDbType.NVarChar).Value = Modal.Section8_20_Comment != null ? Modal.Section8_20_Comment : "";
            command.Parameters.Add("@Section8_21_Condition", SqlDbType.NVarChar).Value = Modal.Section8_21_Condition != null ? Modal.Section8_21_Condition : "";
            command.Parameters.Add("@Section8_21_Comment", SqlDbType.NVarChar).Value = Modal.Section8_21_Comment != null ? Modal.Section8_21_Comment : "";
            command.Parameters.Add("@Section8_22_Condition", SqlDbType.NVarChar).Value = Modal.Section8_22_Condition != null ? Modal.Section8_22_Condition : "";
            command.Parameters.Add("@Section8_22_Comment", SqlDbType.NVarChar).Value = Modal.Section8_22_Comment != null ? Modal.Section8_22_Comment : "";
            command.Parameters.Add("@Section8_23_Condition", SqlDbType.NVarChar).Value = Modal.Section8_23_Condition != null ? Modal.Section8_23_Condition : "";
            command.Parameters.Add("@Section8_23_Comment", SqlDbType.NVarChar).Value = Modal.Section8_23_Comment != null ? Modal.Section8_23_Comment : "";
            command.Parameters.Add("@Section8_24_Condition", SqlDbType.NVarChar).Value = Modal.Section8_24_Condition != null ? Modal.Section8_24_Condition : "";
            command.Parameters.Add("@Section8_24_Comment", SqlDbType.NVarChar).Value = Modal.Section8_24_Comment != null ? Modal.Section8_24_Comment : "";
            command.Parameters.Add("@Section8_25_Condition", SqlDbType.NVarChar).Value = Modal.Section8_25_Condition != null ? Modal.Section8_25_Condition : "";
            command.Parameters.Add("@Section8_25_Comment", SqlDbType.NVarChar).Value = Modal.Section8_25_Comment != null ? Modal.Section8_25_Comment : "";
            command.Parameters.Add("@Section9_1_Condition", SqlDbType.NVarChar).Value = Modal.Section9_1_Condition != null ? Modal.Section9_1_Condition : "";
            command.Parameters.Add("@Section9_1_Comment", SqlDbType.NVarChar).Value = Modal.Section9_1_Comment != null ? Modal.Section9_1_Comment : "";
            command.Parameters.Add("@Section9_2_Condition", SqlDbType.NVarChar).Value = Modal.Section9_2_Condition != null ? Modal.Section9_2_Condition : "";
            command.Parameters.Add("@Section9_2_Comment", SqlDbType.NVarChar).Value = Modal.Section9_2_Comment != null ? Modal.Section9_2_Comment : "";
            command.Parameters.Add("@Section9_3_Condition", SqlDbType.NVarChar).Value = Modal.Section9_3_Condition != null ? Modal.Section9_3_Condition : "";
            command.Parameters.Add("@Section9_3_Comment", SqlDbType.NVarChar).Value = Modal.Section9_3_Comment != null ? Modal.Section9_3_Comment : "";
            command.Parameters.Add("@Section9_4_Condition", SqlDbType.NVarChar).Value = Modal.Section9_4_Condition != null ? Modal.Section9_4_Condition : "";
            command.Parameters.Add("@Section9_4_Comment", SqlDbType.NVarChar).Value = Modal.Section9_4_Comment != null ? Modal.Section9_4_Comment : "";
            command.Parameters.Add("@Section9_5_Condition", SqlDbType.NVarChar).Value = Modal.Section9_5_Condition != null ? Modal.Section9_5_Condition : "";
            command.Parameters.Add("@Section9_5_Comment", SqlDbType.NVarChar).Value = Modal.Section9_5_Comment != null ? Modal.Section9_5_Comment : "";
            command.Parameters.Add("@Section9_6_Condition", SqlDbType.NVarChar).Value = Modal.Section9_6_Condition != null ? Modal.Section9_6_Condition : "";
            command.Parameters.Add("@Section9_6_Comment", SqlDbType.NVarChar).Value = Modal.Section9_6_Comment != null ? Modal.Section9_6_Comment : "";
            command.Parameters.Add("@Section9_7_Condition", SqlDbType.NVarChar).Value = Modal.Section9_7_Condition != null ? Modal.Section9_7_Condition : "";
            command.Parameters.Add("@Section9_7_Comment", SqlDbType.NVarChar).Value = Modal.Section9_7_Comment != null ? Modal.Section9_7_Comment : "";
            command.Parameters.Add("@Section9_8_Condition", SqlDbType.NVarChar).Value = Modal.Section9_8_Condition != null ? Modal.Section9_8_Condition : "";
            command.Parameters.Add("@Section9_8_Comment", SqlDbType.NVarChar).Value = Modal.Section9_8_Comment != null ? Modal.Section9_8_Comment : "";
            command.Parameters.Add("@Section9_9_Condition", SqlDbType.NVarChar).Value = Modal.Section9_9_Condition != null ? Modal.Section9_9_Condition : "";
            command.Parameters.Add("@Section9_9_Comment", SqlDbType.NVarChar).Value = Modal.Section9_9_Comment != null ? Modal.Section9_9_Comment : "";
            command.Parameters.Add("@Section9_10_Condition", SqlDbType.NVarChar).Value = Modal.Section9_10_Condition != null ? Modal.Section9_10_Condition : "";
            command.Parameters.Add("@Section9_10_Comment", SqlDbType.NVarChar).Value = Modal.Section9_10_Comment != null ? Modal.Section9_10_Comment : "";
            command.Parameters.Add("@Section9_11_Condition", SqlDbType.NVarChar).Value = Modal.Section9_11_Condition != null ? Modal.Section9_11_Condition : "";
            command.Parameters.Add("@Section9_11_Comment", SqlDbType.NVarChar).Value = Modal.Section9_11_Comment != null ? Modal.Section9_11_Comment : "";
            command.Parameters.Add("@Section9_12_Condition", SqlDbType.NVarChar).Value = Modal.Section9_12_Condition != null ? Modal.Section9_12_Condition : "";
            command.Parameters.Add("@Section9_12_Comment", SqlDbType.NVarChar).Value = Modal.Section9_12_Comment != null ? Modal.Section9_12_Comment : "";
            command.Parameters.Add("@Section9_13_Condition", SqlDbType.NVarChar).Value = Modal.Section9_13_Condition != null ? Modal.Section9_13_Condition : "";
            command.Parameters.Add("@Section9_13_Comment", SqlDbType.NVarChar).Value = Modal.Section9_13_Comment != null ? Modal.Section9_13_Comment : "";
            command.Parameters.Add("@Section9_14_Condition", SqlDbType.NVarChar).Value = Modal.Section9_14_Condition != null ? Modal.Section9_14_Condition : "";
            command.Parameters.Add("@Section9_14_Comment", SqlDbType.NVarChar).Value = Modal.Section9_14_Comment != null ? Modal.Section9_14_Comment : "";
            command.Parameters.Add("@Section9_15_Condition", SqlDbType.NVarChar).Value = Modal.Section9_15_Condition != null ? Modal.Section9_15_Condition : "";
            command.Parameters.Add("@Section9_15_Comment", SqlDbType.NVarChar).Value = Modal.Section9_15_Comment != null ? Modal.Section9_15_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section9_16_Condition", SqlDbType.NVarChar).Value = Modal.Section9_16_Condition != null ? Modal.Section9_16_Condition : "";
            command.Parameters.Add("@Section9_16_Comment", SqlDbType.NVarChar).Value = Modal.Section9_16_Comment != null ? Modal.Section9_16_Comment : "";
            command.Parameters.Add("@Section9_17_Condition", SqlDbType.NVarChar).Value = Modal.Section9_17_Condition != null ? Modal.Section9_17_Condition : "";
            command.Parameters.Add("@Section9_17_Comment", SqlDbType.NVarChar).Value = Modal.Section9_17_Comment != null ? Modal.Section9_17_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@Section10_1_Condition", SqlDbType.NVarChar).Value = Modal.Section10_1_Condition != null ? Modal.Section10_1_Condition : "";
            command.Parameters.Add("@Section10_1_Comment", SqlDbType.NVarChar).Value = Modal.Section10_1_Comment != null ? Modal.Section10_1_Comment : "";
            command.Parameters.Add("@Section10_2_Condition", SqlDbType.NVarChar).Value = Modal.Section10_2_Condition != null ? Modal.Section10_2_Condition : "";
            command.Parameters.Add("@Section10_2_Comment", SqlDbType.NVarChar).Value = Modal.Section10_2_Comment != null ? Modal.Section10_2_Comment : "";
            command.Parameters.Add("@Section10_3_Condition", SqlDbType.NVarChar).Value = Modal.Section10_3_Condition != null ? Modal.Section10_3_Condition : "";
            command.Parameters.Add("@Section10_3_Comment", SqlDbType.NVarChar).Value = Modal.Section10_3_Comment != null ? Modal.Section10_3_Comment : "";
            command.Parameters.Add("@Section10_4_Condition", SqlDbType.NVarChar).Value = Modal.Section10_4_Condition != null ? Modal.Section10_4_Condition : "";
            command.Parameters.Add("@Section10_4_Comment", SqlDbType.NVarChar).Value = Modal.Section10_4_Comment != null ? Modal.Section10_4_Comment : "";
            command.Parameters.Add("@Section10_5_Condition", SqlDbType.NVarChar).Value = Modal.Section10_5_Condition != null ? Modal.Section10_5_Condition : "";
            command.Parameters.Add("@Section10_5_Comment", SqlDbType.NVarChar).Value = Modal.Section10_5_Comment != null ? Modal.Section10_5_Comment : "";
            command.Parameters.Add("@Section10_6_Condition", SqlDbType.NVarChar).Value = Modal.Section10_6_Condition != null ? Modal.Section10_6_Condition : "";
            command.Parameters.Add("@Section10_6_Comment", SqlDbType.NVarChar).Value = Modal.Section10_6_Comment != null ? Modal.Section10_6_Comment : "";
            command.Parameters.Add("@Section10_7_Condition", SqlDbType.NVarChar).Value = Modal.Section10_7_Condition != null ? Modal.Section10_7_Condition : "";
            command.Parameters.Add("@Section10_7_Comment", SqlDbType.NVarChar).Value = Modal.Section10_7_Comment != null ? Modal.Section10_7_Comment : "";
            command.Parameters.Add("@Section10_8_Condition", SqlDbType.NVarChar).Value = Modal.Section10_8_Condition != null ? Modal.Section10_8_Condition : "";
            command.Parameters.Add("@Section10_8_Comment", SqlDbType.NVarChar).Value = Modal.Section10_8_Comment != null ? Modal.Section10_8_Comment : "";
            command.Parameters.Add("@Section10_9_Condition", SqlDbType.NVarChar).Value = Modal.Section10_9_Condition != null ? Modal.Section10_9_Condition : "";
            command.Parameters.Add("@Section10_9_Comment", SqlDbType.NVarChar).Value = Modal.Section10_9_Comment != null ? Modal.Section10_9_Comment : "";
            command.Parameters.Add("@Section10_10_Condition", SqlDbType.NVarChar).Value = Modal.Section10_10_Condition != null ? Modal.Section10_10_Condition : "";
            command.Parameters.Add("@Section10_10_Comment", SqlDbType.NVarChar).Value = Modal.Section10_10_Comment != null ? Modal.Section10_10_Comment : "";
            command.Parameters.Add("@Section10_11_Condition", SqlDbType.NVarChar).Value = Modal.Section10_11_Condition != null ? Modal.Section10_11_Condition : "";
            command.Parameters.Add("@Section10_11_Comment", SqlDbType.NVarChar).Value = Modal.Section10_11_Comment != null ? Modal.Section10_11_Comment : "";
            command.Parameters.Add("@Section10_12_Condition", SqlDbType.NVarChar).Value = Modal.Section10_12_Condition != null ? Modal.Section10_12_Condition : "";
            command.Parameters.Add("@Section10_12_Comment", SqlDbType.NVarChar).Value = Modal.Section10_12_Comment != null ? Modal.Section10_12_Comment : "";
            command.Parameters.Add("@Section10_13_Condition", SqlDbType.NVarChar).Value = Modal.Section10_13_Condition != null ? Modal.Section10_13_Condition : "";
            command.Parameters.Add("@Section10_13_Comment", SqlDbType.NVarChar).Value = Modal.Section10_13_Comment != null ? Modal.Section10_13_Comment : "";
            command.Parameters.Add("@Section10_14_Condition", SqlDbType.NVarChar).Value = Modal.Section10_14_Condition != null ? Modal.Section10_14_Condition : "";
            command.Parameters.Add("@Section10_14_Comment", SqlDbType.NVarChar).Value = Modal.Section10_14_Comment != null ? Modal.Section10_14_Comment : "";
            command.Parameters.Add("@Section10_15_Condition", SqlDbType.NVarChar).Value = Modal.Section10_15_Condition != null ? Modal.Section10_15_Condition : "";
            command.Parameters.Add("@Section10_15_Comment", SqlDbType.NVarChar).Value = Modal.Section10_15_Comment != null ? Modal.Section10_15_Comment : "";
            command.Parameters.Add("@Section10_16_Condition", SqlDbType.NVarChar).Value = Modal.Section10_16_Condition != null ? Modal.Section10_16_Condition : "";
            command.Parameters.Add("@Section10_16_Comment", SqlDbType.NVarChar).Value = Modal.Section10_16_Comment != null ? Modal.Section10_16_Comment : "";
            command.Parameters.Add("@Section11_1_Condition", SqlDbType.NVarChar).Value = Modal.Section11_1_Condition != null ? Modal.Section11_1_Condition : "";
            command.Parameters.Add("@Section11_1_Comment", SqlDbType.NVarChar).Value = Modal.Section11_1_Comment != null ? Modal.Section11_1_Comment : "";
            command.Parameters.Add("@Section11_2_Condition", SqlDbType.NVarChar).Value = Modal.Section11_2_Condition != null ? Modal.Section11_2_Condition : "";
            command.Parameters.Add("@Section11_2_Comment", SqlDbType.NVarChar).Value = Modal.Section11_2_Comment != null ? Modal.Section11_2_Comment : "";
            command.Parameters.Add("@Section11_3_Condition", SqlDbType.NVarChar).Value = Modal.Section11_3_Condition != null ? Modal.Section11_3_Condition : "";
            command.Parameters.Add("@Section11_3_Comment", SqlDbType.NVarChar).Value = Modal.Section11_3_Comment != null ? Modal.Section11_3_Comment : "";
            command.Parameters.Add("@Section11_4_Condition", SqlDbType.NVarChar).Value = Modal.Section11_4_Condition != null ? Modal.Section11_4_Condition : "";
            command.Parameters.Add("@Section11_4_Comment", SqlDbType.NVarChar).Value = Modal.Section11_4_Comment != null ? Modal.Section11_4_Comment : "";
            command.Parameters.Add("@Section11_5_Condition", SqlDbType.NVarChar).Value = Modal.Section11_5_Condition != null ? Modal.Section11_5_Condition : "";
            command.Parameters.Add("@Section11_5_Comment", SqlDbType.NVarChar).Value = Modal.Section11_5_Comment != null ? Modal.Section11_5_Comment : "";
            command.Parameters.Add("@Section11_6_Condition", SqlDbType.NVarChar).Value = Modal.Section11_6_Condition != null ? Modal.Section11_6_Condition : "";
            command.Parameters.Add("@Section11_6_Comment", SqlDbType.NVarChar).Value = Modal.Section11_6_Comment != null ? Modal.Section11_6_Comment : "";
            command.Parameters.Add("@Section11_7_Condition", SqlDbType.NVarChar).Value = Modal.Section11_7_Condition != null ? Modal.Section11_7_Condition : "";
            command.Parameters.Add("@Section11_7_Comment", SqlDbType.NVarChar).Value = Modal.Section11_7_Comment != null ? Modal.Section11_7_Comment : "";
            command.Parameters.Add("@Section11_8_Condition", SqlDbType.NVarChar).Value = Modal.Section11_8_Condition != null ? Modal.Section11_8_Condition : "";
            command.Parameters.Add("@Section11_8_Comment", SqlDbType.NVarChar).Value = Modal.Section11_8_Comment != null ? Modal.Section11_8_Comment : "";
            command.Parameters.Add("@Section12_1_Condition", SqlDbType.NVarChar).Value = Modal.Section12_1_Condition != null ? Modal.Section12_1_Condition : "";
            command.Parameters.Add("@Section12_1_Comment", SqlDbType.NVarChar).Value = Modal.Section12_1_Comment != null ? Modal.Section12_1_Comment : "";
            command.Parameters.Add("@Section12_2_Condition", SqlDbType.NVarChar).Value = Modal.Section12_2_Condition != null ? Modal.Section12_2_Condition : "";
            command.Parameters.Add("@Section12_2_Comment", SqlDbType.NVarChar).Value = Modal.Section12_2_Comment != null ? Modal.Section12_2_Comment : "";
            command.Parameters.Add("@Section12_3_Condition", SqlDbType.NVarChar).Value = Modal.Section12_3_Condition != null ? Modal.Section12_3_Condition : "";
            command.Parameters.Add("@Section12_3_Comment", SqlDbType.NVarChar).Value = Modal.Section12_3_Comment != null ? Modal.Section12_3_Comment : "";
            command.Parameters.Add("@Section12_4_Condition", SqlDbType.NVarChar).Value = Modal.Section12_4_Condition != null ? Modal.Section12_4_Condition : "";
            command.Parameters.Add("@Section12_4_Comment", SqlDbType.NVarChar).Value = Modal.Section12_4_Comment != null ? Modal.Section12_4_Comment : "";
            command.Parameters.Add("@Section12_5_Condition", SqlDbType.NVarChar).Value = Modal.Section12_5_Condition != null ? Modal.Section12_5_Condition : "";
            command.Parameters.Add("@Section12_5_Comment", SqlDbType.NVarChar).Value = Modal.Section12_5_Comment != null ? Modal.Section12_5_Comment : "";
            command.Parameters.Add("@Section12_6_Condition", SqlDbType.NVarChar).Value = Modal.Section12_6_Condition != null ? Modal.Section12_6_Condition : "";
            command.Parameters.Add("@Section12_6_Comment", SqlDbType.NVarChar).Value = Modal.Section12_6_Comment != null ? Modal.Section12_6_Comment : "";
            command.Parameters.Add("@Section13_1_Condition", SqlDbType.NVarChar).Value = Modal.Section13_1_Condition != null ? Modal.Section13_1_Condition : "";
            command.Parameters.Add("@Section13_1_Comment", SqlDbType.NVarChar).Value = Modal.Section13_1_Comment != null ? Modal.Section13_1_Comment : "";
            command.Parameters.Add("@Section13_2_Condition", SqlDbType.NVarChar).Value = Modal.Section13_2_Condition != null ? Modal.Section13_2_Condition : "";
            command.Parameters.Add("@Section13_2_Comment", SqlDbType.NVarChar).Value = Modal.Section13_2_Comment != null ? Modal.Section13_2_Comment : "";
            command.Parameters.Add("@Section13_3_Condition", SqlDbType.NVarChar).Value = Modal.Section13_3_Condition != null ? Modal.Section13_3_Condition : "";
            command.Parameters.Add("@Section13_3_Comment", SqlDbType.NVarChar).Value = Modal.Section13_3_Comment != null ? Modal.Section13_3_Comment : "";
            command.Parameters.Add("@Section13_4_Condition", SqlDbType.NVarChar).Value = Modal.Section13_4_Condition != null ? Modal.Section13_4_Condition : "";
            command.Parameters.Add("@Section13_4_Comment", SqlDbType.NVarChar).Value = Modal.Section13_4_Comment != null ? Modal.Section13_4_Comment : "";
            command.Parameters.Add("@Section14_1_Condition", SqlDbType.NVarChar).Value = Modal.Section14_1_Condition != null ? Modal.Section14_1_Condition : "";
            command.Parameters.Add("@Section14_1_Comment", SqlDbType.NVarChar).Value = Modal.Section14_1_Comment != null ? Modal.Section14_1_Comment : "";
            command.Parameters.Add("@Section14_2_Condition", SqlDbType.NVarChar).Value = Modal.Section14_2_Condition != null ? Modal.Section14_2_Condition : "";
            command.Parameters.Add("@Section14_2_Comment", SqlDbType.NVarChar).Value = Modal.Section14_2_Comment != null ? Modal.Section14_2_Comment : "";
            command.Parameters.Add("@Section14_3_Condition", SqlDbType.NVarChar).Value = Modal.Section14_3_Condition != null ? Modal.Section14_3_Condition : "";
            command.Parameters.Add("@Section14_3_Comment", SqlDbType.NVarChar).Value = Modal.Section14_3_Comment != null ? Modal.Section14_3_Comment : "";
            command.Parameters.Add("@Section14_4_Condition", SqlDbType.NVarChar).Value = Modal.Section14_4_Condition != null ? Modal.Section14_4_Condition : "";
            command.Parameters.Add("@Section14_4_Comment", SqlDbType.NVarChar).Value = Modal.Section14_4_Comment != null ? Modal.Section14_4_Comment : "";
            command.Parameters.Add("@Section14_5_Condition", SqlDbType.NVarChar).Value = Modal.Section14_5_Condition != null ? Modal.Section14_5_Condition : "";
            command.Parameters.Add("@Section14_5_Comment", SqlDbType.NVarChar).Value = Modal.Section14_5_Comment != null ? Modal.Section14_5_Comment : "";
            command.Parameters.Add("@Section14_6_Condition", SqlDbType.NVarChar).Value = Modal.Section14_6_Condition != null ? Modal.Section14_6_Condition : "";
            command.Parameters.Add("@Section14_6_Comment", SqlDbType.NVarChar).Value = Modal.Section14_6_Comment != null ? Modal.Section14_6_Comment : "";
            command.Parameters.Add("@Section14_7_Condition", SqlDbType.NVarChar).Value = Modal.Section14_7_Condition != null ? Modal.Section14_7_Condition : "";
            command.Parameters.Add("@Section14_7_Comment", SqlDbType.NVarChar).Value = Modal.Section14_7_Comment != null ? Modal.Section14_7_Comment : "";
            command.Parameters.Add("@Section14_8_Condition", SqlDbType.NVarChar).Value = Modal.Section14_8_Condition != null ? Modal.Section14_8_Condition : "";
            command.Parameters.Add("@Section14_8_Comment", SqlDbType.NVarChar).Value = Modal.Section14_8_Comment != null ? Modal.Section14_8_Comment : "";
            command.Parameters.Add("@Section14_9_Condition", SqlDbType.NVarChar).Value = Modal.Section14_9_Condition != null ? Modal.Section14_9_Condition : "";
            command.Parameters.Add("@Section14_9_Comment", SqlDbType.NVarChar).Value = Modal.Section14_9_Comment != null ? Modal.Section14_9_Comment : "";
            command.Parameters.Add("@Section14_10_Condition", SqlDbType.NVarChar).Value = Modal.Section14_10_Condition != null ? Modal.Section14_10_Condition : "";
            command.Parameters.Add("@Section14_10_Comment", SqlDbType.NVarChar).Value = Modal.Section14_10_Comment != null ? Modal.Section14_10_Comment : "";
            command.Parameters.Add("@Section14_11_Condition", SqlDbType.NVarChar).Value = Modal.Section14_11_Condition != null ? Modal.Section14_11_Condition : "";
            command.Parameters.Add("@Section14_11_Comment", SqlDbType.NVarChar).Value = Modal.Section14_11_Comment != null ? Modal.Section14_11_Comment : "";
            command.Parameters.Add("@Section14_12_Condition", SqlDbType.NVarChar).Value = Modal.Section14_12_Condition != null ? Modal.Section14_12_Condition : "";
            command.Parameters.Add("@Section14_12_Comment", SqlDbType.NVarChar).Value = Modal.Section14_12_Comment != null ? Modal.Section14_12_Comment : "";
            command.Parameters.Add("@Section14_13_Condition", SqlDbType.NVarChar).Value = Modal.Section14_13_Condition != null ? Modal.Section14_13_Condition : "";
            command.Parameters.Add("@Section14_13_Comment", SqlDbType.NVarChar).Value = Modal.Section14_13_Comment != null ? Modal.Section14_13_Comment : "";
            command.Parameters.Add("@Section14_14_Condition", SqlDbType.NVarChar).Value = Modal.Section14_14_Condition != null ? Modal.Section14_14_Condition : "";
            command.Parameters.Add("@Section14_14_Comment", SqlDbType.NVarChar).Value = Modal.Section14_14_Comment != null ? Modal.Section14_14_Comment : "";
            command.Parameters.Add("@Section14_15_Condition", SqlDbType.NVarChar).Value = Modal.Section14_15_Condition != null ? Modal.Section14_15_Condition : "";
            command.Parameters.Add("@Section14_15_Comment", SqlDbType.NVarChar).Value = Modal.Section14_15_Comment != null ? Modal.Section14_15_Comment : "";
            command.Parameters.Add("@Section14_16_Condition", SqlDbType.NVarChar).Value = Modal.Section14_16_Condition != null ? Modal.Section14_16_Condition : "";
            command.Parameters.Add("@Section14_16_Comment", SqlDbType.NVarChar).Value = Modal.Section14_16_Comment != null ? Modal.Section14_16_Comment : "";
            command.Parameters.Add("@Section14_17_Condition", SqlDbType.NVarChar).Value = Modal.Section14_17_Condition != null ? Modal.Section14_17_Condition : "";
            command.Parameters.Add("@Section14_17_Comment", SqlDbType.NVarChar).Value = Modal.Section14_17_Comment != null ? Modal.Section14_17_Comment : "";
            command.Parameters.Add("@Section14_18_Condition", SqlDbType.NVarChar).Value = Modal.Section14_18_Condition != null ? Modal.Section14_18_Condition : "";
            command.Parameters.Add("@Section14_18_Comment", SqlDbType.NVarChar).Value = Modal.Section14_18_Comment != null ? Modal.Section14_18_Comment : "";
            command.Parameters.Add("@Section14_19_Condition", SqlDbType.NVarChar).Value = Modal.Section14_19_Condition != null ? Modal.Section14_19_Condition : "";
            command.Parameters.Add("@Section14_19_Comment", SqlDbType.NVarChar).Value = Modal.Section14_19_Comment != null ? Modal.Section14_19_Comment : "";
            command.Parameters.Add("@Section14_20_Condition", SqlDbType.NVarChar).Value = Modal.Section14_20_Condition != null ? Modal.Section14_20_Condition : "";
            command.Parameters.Add("@Section14_20_Comment", SqlDbType.NVarChar).Value = Modal.Section14_20_Comment != null ? Modal.Section14_20_Comment : "";
            command.Parameters.Add("@Section14_21_Condition", SqlDbType.NVarChar).Value = Modal.Section14_21_Condition != null ? Modal.Section14_21_Condition : "";
            command.Parameters.Add("@Section14_21_Comment", SqlDbType.NVarChar).Value = Modal.Section14_21_Comment != null ? Modal.Section14_21_Comment : "";
            command.Parameters.Add("@Section14_22_Condition", SqlDbType.NVarChar).Value = Modal.Section14_22_Condition != null ? Modal.Section14_22_Condition : "";
            command.Parameters.Add("@Section14_22_Comment", SqlDbType.NVarChar).Value = Modal.Section14_22_Comment != null ? Modal.Section14_22_Comment : "";
            command.Parameters.Add("@Section14_23_Condition", SqlDbType.NVarChar).Value = Modal.Section14_23_Condition != null ? Modal.Section14_23_Condition : "";
            command.Parameters.Add("@Section14_23_Comment", SqlDbType.NVarChar).Value = Modal.Section14_23_Comment != null ? Modal.Section14_23_Comment : "";
            command.Parameters.Add("@Section14_24_Condition", SqlDbType.NVarChar).Value = Modal.Section14_24_Condition != null ? Modal.Section14_24_Condition : "";
            command.Parameters.Add("@Section14_24_Comment", SqlDbType.NVarChar).Value = Modal.Section14_24_Comment != null ? Modal.Section14_24_Comment : "";
            command.Parameters.Add("@Section14_25_Condition", SqlDbType.NVarChar).Value = Modal.Section14_25_Condition != null ? Modal.Section14_25_Condition : "";
            command.Parameters.Add("@Section14_25_Comment", SqlDbType.NVarChar).Value = Modal.Section14_25_Comment != null ? Modal.Section14_25_Comment : "";
            command.Parameters.Add("@Section15_1_Condition", SqlDbType.NVarChar).Value = Modal.Section15_1_Condition != null ? Modal.Section15_1_Condition : "";
            command.Parameters.Add("@Section15_1_Comment", SqlDbType.NVarChar).Value = Modal.Section15_1_Comment != null ? Modal.Section15_1_Comment : "";
            command.Parameters.Add("@Section15_2_Condition", SqlDbType.NVarChar).Value = Modal.Section15_2_Condition != null ? Modal.Section15_2_Condition : "";
            command.Parameters.Add("@Section15_2_Comment", SqlDbType.NVarChar).Value = Modal.Section15_2_Comment != null ? Modal.Section15_2_Comment : "";
            command.Parameters.Add("@Section15_3_Condition", SqlDbType.NVarChar).Value = Modal.Section15_3_Condition != null ? Modal.Section15_3_Condition : "";
            command.Parameters.Add("@Section15_3_Comment", SqlDbType.NVarChar).Value = Modal.Section15_3_Comment != null ? Modal.Section15_3_Comment : "";
            command.Parameters.Add("@Section15_4_Condition", SqlDbType.NVarChar).Value = Modal.Section15_4_Condition != null ? Modal.Section15_4_Condition : "";
            command.Parameters.Add("@Section15_4_Comment", SqlDbType.NVarChar).Value = Modal.Section15_4_Comment != null ? Modal.Section15_4_Comment : "";
            command.Parameters.Add("@Section15_5_Condition", SqlDbType.NVarChar).Value = Modal.Section15_5_Condition != null ? Modal.Section15_5_Condition : "";
            command.Parameters.Add("@Section15_5_Comment", SqlDbType.NVarChar).Value = Modal.Section15_5_Comment != null ? Modal.Section15_5_Comment : "";
            command.Parameters.Add("@Section15_6_Condition", SqlDbType.NVarChar).Value = Modal.Section15_6_Condition != null ? Modal.Section15_6_Condition : "";
            command.Parameters.Add("@Section15_6_Comment", SqlDbType.NVarChar).Value = Modal.Section15_6_Comment != null ? Modal.Section15_6_Comment : "";
            command.Parameters.Add("@Section15_7_Condition", SqlDbType.NVarChar).Value = Modal.Section15_7_Condition != null ? Modal.Section15_7_Condition : "";
            command.Parameters.Add("@Section15_7_Comment", SqlDbType.NVarChar).Value = Modal.Section15_7_Comment != null ? Modal.Section15_7_Comment : "";
            command.Parameters.Add("@Section15_8_Condition", SqlDbType.NVarChar).Value = Modal.Section15_8_Condition != null ? Modal.Section15_8_Condition : "";
            command.Parameters.Add("@Section15_8_Comment", SqlDbType.NVarChar).Value = Modal.Section15_8_Comment != null ? Modal.Section15_8_Comment : "";
            command.Parameters.Add("@Section15_9_Condition", SqlDbType.NVarChar).Value = Modal.Section15_9_Condition != null ? Modal.Section15_9_Condition : "";
            command.Parameters.Add("@Section15_9_Comment", SqlDbType.NVarChar).Value = Modal.Section15_9_Comment != null ? Modal.Section15_9_Comment : "";
            command.Parameters.Add("@Section15_10_Condition", SqlDbType.NVarChar).Value = Modal.Section15_10_Condition != null ? Modal.Section15_10_Condition : "";
            command.Parameters.Add("@Section15_10_Comment", SqlDbType.NVarChar).Value = Modal.Section15_10_Comment != null ? Modal.Section15_10_Comment : "";
            command.Parameters.Add("@Section15_11_Condition", SqlDbType.NVarChar).Value = Modal.Section15_11_Condition != null ? Modal.Section15_11_Condition : "";
            command.Parameters.Add("@Section15_11_Comment", SqlDbType.NVarChar).Value = Modal.Section15_11_Comment != null ? Modal.Section15_11_Comment : "";
            command.Parameters.Add("@Section15_12_Condition", SqlDbType.NVarChar).Value = Modal.Section15_12_Condition != null ? Modal.Section15_12_Condition : "";
            command.Parameters.Add("@Section15_12_Comment", SqlDbType.NVarChar).Value = Modal.Section15_12_Comment != null ? Modal.Section15_12_Comment : "";
            command.Parameters.Add("@Section15_13_Condition", SqlDbType.NVarChar).Value = Modal.Section15_13_Condition != null ? Modal.Section15_13_Condition : "";
            command.Parameters.Add("@Section15_13_Comment", SqlDbType.NVarChar).Value = Modal.Section15_13_Comment != null ? Modal.Section15_13_Comment : "";
            command.Parameters.Add("@Section15_14_Condition", SqlDbType.NVarChar).Value = Modal.Section15_14_Condition != null ? Modal.Section15_14_Condition : "";
            command.Parameters.Add("@Section15_14_Comment", SqlDbType.NVarChar).Value = Modal.Section15_14_Comment != null ? Modal.Section15_14_Comment : "";
            command.Parameters.Add("@Section15_15_Condition", SqlDbType.NVarChar).Value = Modal.Section15_15_Condition != null ? Modal.Section15_15_Condition : "";
            command.Parameters.Add("@Section15_15_Comment", SqlDbType.NVarChar).Value = Modal.Section15_15_Comment != null ? Modal.Section15_15_Comment : "";
            command.Parameters.Add("@Section16_1_Condition", SqlDbType.NVarChar).Value = Modal.Section16_1_Condition != null ? Modal.Section16_1_Condition : "";
            command.Parameters.Add("@Section16_1_Comment", SqlDbType.NVarChar).Value = Modal.Section16_1_Comment != null ? Modal.Section16_1_Comment : "";
            command.Parameters.Add("@Section16_2_Condition", SqlDbType.NVarChar).Value = Modal.Section16_2_Condition != null ? Modal.Section16_2_Condition : "";
            command.Parameters.Add("@Section16_2_Comment", SqlDbType.NVarChar).Value = Modal.Section16_2_Comment != null ? Modal.Section16_2_Comment : "";
            command.Parameters.Add("@Section16_3_Condition", SqlDbType.NVarChar).Value = Modal.Section16_3_Condition != null ? Modal.Section16_3_Condition : "";
            command.Parameters.Add("@Section16_3_Comment", SqlDbType.NVarChar).Value = Modal.Section16_3_Comment != null ? Modal.Section16_3_Comment : "";
            command.Parameters.Add("@Section16_4_Condition", SqlDbType.NVarChar).Value = Modal.Section16_4_Condition != null ? Modal.Section16_4_Condition : "";
            command.Parameters.Add("@Section16_4_Comment", SqlDbType.NVarChar).Value = Modal.Section16_4_Comment != null ? Modal.Section16_4_Comment : "";
            command.Parameters.Add("@Section17_1_Condition", SqlDbType.NVarChar).Value = Modal.Section17_1_Condition != null ? Modal.Section17_1_Condition : "";
            command.Parameters.Add("@Section17_1_Comment", SqlDbType.NVarChar).Value = Modal.Section17_1_Comment != null ? Modal.Section17_1_Comment : "";
            command.Parameters.Add("@Section17_2_Condition", SqlDbType.NVarChar).Value = Modal.Section17_2_Condition != null ? Modal.Section17_2_Condition : "";
            command.Parameters.Add("@Section17_2_Comment", SqlDbType.NVarChar).Value = Modal.Section17_2_Comment != null ? Modal.Section17_2_Comment : "";
            command.Parameters.Add("@Section17_3_Condition", SqlDbType.NVarChar).Value = Modal.Section17_3_Condition != null ? Modal.Section17_3_Condition : "";
            command.Parameters.Add("@Section17_3_Comment", SqlDbType.NVarChar).Value = Modal.Section17_3_Comment != null ? Modal.Section17_3_Comment : "";
            command.Parameters.Add("@Section17_4_Condition", SqlDbType.NVarChar).Value = Modal.Section17_4_Condition != null ? Modal.Section17_4_Condition : "";
            command.Parameters.Add("@Section17_4_Comment", SqlDbType.NVarChar).Value = Modal.Section17_4_Comment != null ? Modal.Section17_4_Comment : "";
            command.Parameters.Add("@Section17_5_Condition", SqlDbType.NVarChar).Value = Modal.Section17_5_Condition != null ? Modal.Section17_5_Condition : "";
            command.Parameters.Add("@Section17_5_Comment", SqlDbType.NVarChar).Value = Modal.Section17_5_Comment != null ? Modal.Section17_5_Comment : "";
            command.Parameters.Add("@Section17_6_Condition", SqlDbType.NVarChar).Value = Modal.Section17_6_Condition != null ? Modal.Section17_6_Condition : "";
            command.Parameters.Add("@Section17_6_Comment", SqlDbType.NVarChar).Value = Modal.Section17_6_Comment != null ? Modal.Section17_6_Comment : "";
            command.Parameters.Add("@Section18_1_Condition", SqlDbType.NVarChar).Value = Modal.Section18_1_Condition != null ? Modal.Section18_1_Condition : "";
            command.Parameters.Add("@Section18_1_Comment", SqlDbType.NVarChar).Value = Modal.Section18_1_Comment != null ? Modal.Section18_1_Comment : "";
            command.Parameters.Add("@Section18_2_Condition", SqlDbType.NVarChar).Value = Modal.Section18_2_Condition != null ? Modal.Section18_2_Condition : "";
            command.Parameters.Add("@Section18_2_Comment", SqlDbType.NVarChar).Value = Modal.Section18_2_Comment != null ? Modal.Section18_2_Comment : "";
            command.Parameters.Add("@Section18_3_Condition", SqlDbType.NVarChar).Value = Modal.Section18_3_Condition != null ? Modal.Section18_3_Condition : "";
            command.Parameters.Add("@Section18_3_Comment", SqlDbType.NVarChar).Value = Modal.Section18_3_Comment != null ? Modal.Section18_3_Comment : "";
            command.Parameters.Add("@Section18_4_Condition", SqlDbType.NVarChar).Value = Modal.Section18_4_Condition != null ? Modal.Section18_4_Condition : "";
            command.Parameters.Add("@Section18_4_Comment", SqlDbType.NVarChar).Value = Modal.Section18_4_Comment != null ? Modal.Section18_4_Comment : "";
            command.Parameters.Add("@Section18_5_Condition", SqlDbType.NVarChar).Value = Modal.Section18_5_Condition != null ? Modal.Section18_5_Condition : "";
            command.Parameters.Add("@Section18_5_Comment", SqlDbType.NVarChar).Value = Modal.Section18_5_Comment != null ? Modal.Section18_5_Comment : "";
            command.Parameters.Add("@Section18_6_Condition", SqlDbType.NVarChar).Value = Modal.Section18_6_Condition != null ? Modal.Section18_6_Condition : "";
            command.Parameters.Add("@Section18_6_Comment", SqlDbType.NVarChar).Value = Modal.Section18_6_Comment != null ? Modal.Section18_6_Comment : "";
            command.Parameters.Add("@Section18_7_Condition", SqlDbType.NVarChar).Value = Modal.Section18_7_Condition != null ? Modal.Section18_7_Condition : "";
            command.Parameters.Add("@Section18_7_Comment", SqlDbType.NVarChar).Value = Modal.Section18_7_Comment != null ? Modal.Section18_7_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section18_8_Condition", SqlDbType.NVarChar).Value = Modal.Section18_8_Condition != null ? Modal.Section18_8_Condition : "";
            command.Parameters.Add("@Section18_8_Comment", SqlDbType.NVarChar).Value = Modal.Section18_8_Comment != null ? Modal.Section18_8_Comment : "";
            command.Parameters.Add("@Section18_9_Condition", SqlDbType.NVarChar).Value = Modal.Section18_9_Condition != null ? Modal.Section18_9_Condition : "";
            command.Parameters.Add("@Section18_9_Comment", SqlDbType.NVarChar).Value = Modal.Section18_9_Comment != null ? Modal.Section18_9_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            command.Parameters.Add("@ModifyDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime //Modal.ModifyDate != null ? Modal.ModifyDate : null;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft ?? (object)DBNull.Value;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.FormVersion;
        }
        #endregion

        #region SIR Notes and Additional Notes
        // RDBJ 04/02/2022
        public string CheckRecordsExistOrNot(string tableName, string columnName, Guid UniqueID)
        {
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                SqlConnection connection = new SqlConnection(connetionString);
                string res = string.Empty;
                connection.Open();
                DataTable dt1 = new DataTable();
                string isExistRecord = "SELECT " + columnName + " FROM " + tableName + " WHERE " + columnName + " = '" + UniqueID + "'";
                SqlDataAdapter sqlAdp1 = new SqlDataAdapter(isExistRecord, connection);
                sqlAdp1.Fill(dt1);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0][0] == DBNull.Value)
                        res = string.Empty;
                    else
                        res = Convert.ToString(dt1.Rows[0][0]);
                }
                connection.Close();
                return res;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckRecordsExistOrNot :" + ex.Message);
                return string.Empty;
            }
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public string SIRNote_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[SIRNotes] ([SIRFormID], [Number], [Note], [UniqueFormID], [NotesUniqueID], [IsDeleted])
                                VALUES (@SIRFormID, @Number, @Note, @UniqueFormID, @NotesUniqueID, @IsDeleted)";
            return InsertQuery;
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public string SIRNote_UpdateQuery()
        {
            string UpdateQuery = @"UPDATE [dbo].[SIRNotes] 
                                SET [UniqueFormID] = @UniqueFormID, [Number] = @Number, [Note] = @Note, [IsDeleted] = @IsDeleted
                                WHERE [NotesUniqueID] = @NotesUniqueID";
            return UpdateQuery;
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public void SIRNoteInsertOrUpdate_CMD(SIRNote Modal, ref SqlCommand command
            , bool IsNeedToUpdate = false
            )
        {
            if (!IsNeedToUpdate)
            {
                command.Parameters.Add("@SIRFormID", SqlDbType.BigInt).Value = 0;
            }
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = Modal.Number == null ? DBNull.Value : (object)Modal.Number;
            command.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Modal.Note == null ? DBNull.Value : (object)Modal.Note;
            command.Parameters.Add("@IsDeleted", SqlDbType.Int).Value = Modal.IsDeleted;
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public bool SIRNotesInsertOrUpdate(List<SIRNote> SIRNote, Guid UniqueFormID)
        {
            bool retBlnRes = false;
            string ConnectionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                if (SIRNote != null && SIRNote.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in SIRNote)
                    {
                        string strId = CheckRecordsExistOrNot(AppStatic.SIRNotes, "NotesUniqueID", item.NotesUniqueID);
                        string strQuery = string.Empty;
                        bool blnNeedToUpdate = !string.IsNullOrEmpty(strId);

                        if (blnNeedToUpdate)
                        {
                            strQuery = SIRNote_UpdateQuery();
                        }
                        else
                        {
                            strQuery = SIRNote_InsertQuery();
                        }
                        item.UniqueFormID = UniqueFormID;
                        SqlCommand command = new SqlCommand(strQuery, connection);
                        SIRNoteInsertOrUpdate_CMD(item, ref command, blnNeedToUpdate);
                        connection.Open();

                        if (blnNeedToUpdate)
                            command.ExecuteNonQuery();
                        else
                            command.ExecuteScalar();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRNotesInsertOrUpdate Error : " + ex.Message.ToString());
            }

            return retBlnRes;
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public string SIRAdditionalNote_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[SIRAdditionalNotes] ([SIRFormID], [Number], [Note], [UniqueFormID], [NotesUniqueID])
                                VALUES (@SIRFormID, @Number, @Note, @UniqueFormID, @NotesUniqueID)";
            return InsertQuery;
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public string SIRAdditionalNote_UpdateQuery()
        {
            string UpdateQuery = @"UPDATE [dbo].[SIRAdditionalNotes] 
                                SET [UniqueFormID] = @UniqueFormID, [Number] = @Number, [Note] = @Note, [IsDeleted] = @IsDeleted
                                WHERE [NotesUniqueID] = @NotesUniqueID";
            return UpdateQuery;
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public void SIRAdditionalNoteInsertOrUpdate_CMD(SIRAdditionalNote Modal, ref SqlCommand command
            , bool IsNeedToUpdate = false
            )
        {
            if (!IsNeedToUpdate)
            {
                command.Parameters.Add("@SIRFormID", SqlDbType.BigInt).Value = 0;
            }
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = Modal.Number == null ? DBNull.Value : (object)Modal.Number;
            command.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Modal.Note == null ? DBNull.Value : (object)Modal.Note;
            command.Parameters.Add("@IsDeleted", SqlDbType.Int).Value = Modal.IsDeleted;
        }
        // End RDBJ 04/02/2022

        // RDBJ 04/02/2022
        public bool SIRAdditionalNotesInsertOrUpdate(List<SIRAdditionalNote> SIRAdditionalNote, Guid UniqueFormID)
        {
            bool retBlnRes = false;
            string ConnectionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                if (SIRAdditionalNote != null && SIRAdditionalNote.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in SIRAdditionalNote)
                    {
                        string strId = CheckRecordsExistOrNot(AppStatic.SIRAdditionalNotes, "NotesUniqueID", item.NotesUniqueID);
                        string strQuery = string.Empty;
                        bool blnNeedToUpdate = !string.IsNullOrEmpty(strId);

                        if (blnNeedToUpdate)
                        {
                            strQuery = SIRAdditionalNote_UpdateQuery();
                        }
                        else
                        {
                            strQuery = SIRAdditionalNote_InsertQuery();
                        }
                        item.UniqueFormID = UniqueFormID;
                        SqlCommand command = new SqlCommand(strQuery, connection);
                        SIRAdditionalNoteInsertOrUpdate_CMD(item, ref command, blnNeedToUpdate);

                        connection.Open();

                        if (blnNeedToUpdate)
                            command.ExecuteNonQuery();
                        else
                            command.ExecuteScalar();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRAdditionalNotesInsertOrUpdate Error : " + ex.Message.ToString());
            }

            return retBlnRes;
        }
        // End RDBJ 04/02/2022

        // RDBJ 12/18/2021
        public bool SaveSIRNotesDataInLocalDB(List<SIRNote> SIRNote, Guid UniqueFormID)
        {
            GIRFormDataHelper _girHElper = new GIRFormDataHelper();
            bool res = false;
            try
            {
                if (SIRNote != null && SIRNote.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in SIRNote)
                    {
                        item.SIRFormID = 0; //SIRFormID; //RDBJ 10/13/2021 set with 0
                        item.UniqueFormID = UniqueFormID;
                    }

                    bool resp = _girHElper.DeleteRecords(AppStatic.SIRNotes, "UniqueFormID", Convert.ToString(UniqueFormID));

                    string connectionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    DataTable dt = Utility.ToDataTable(SIRNote);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                        bulkCopy.DestinationTableName = AppStatic.SIRNotes;
                        connection.Open();
                        bulkCopy.WriteToServer(dt);
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveSIRNotesDataInLocalDB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        // End RDBJ 12/18/2021

        // RDBJ 12/18/2021
        public bool SaveSIRAdditionalNotesDataInLocalDB(List<SIRAdditionalNote> SIRAdditionalNote, Guid UniqueFormID)
        {
            GIRFormDataHelper _girHElper = new GIRFormDataHelper();
            bool res = false;
            try
            {
                if (SIRAdditionalNote != null && SIRAdditionalNote.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in SIRAdditionalNote)
                    {
                        item.SIRFormID = 0; // SIRFormID; //RDBJ 10/13/2021 set with 0
                        item.UniqueFormID = UniqueFormID;
                    }
                    bool resp = _girHElper.DeleteRecords(AppStatic.SIRAdditionalNotes, "UniqueFormID", Convert.ToString(UniqueFormID));

                    string connectionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    DataTable dt = Utility.ToDataTable(SIRAdditionalNote);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                        bulkCopy.DestinationTableName = AppStatic.SIRAdditionalNotes;
                        connection.Open();
                        bulkCopy.WriteToServer(dt);
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveSIRAdditionalNotesDataInLocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        // End RDBJ 12/18/2021
        #endregion

        #region SIRSynch Based on Latest Version

        //RDBJ 09/27/2021
        public void GETSIRLatestData(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<SuperintendedInspectionReport> CloudSyncList = GetSIRFormsSyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine  // JSL 11/12/2022
                );
            List<SuperintendedInspectionReport> LocalSyncList = GetSIRFormsSyncedDataFromLocal();

            if (CloudSyncList != null && CloudSyncList.Count > 0)
            {
                LogHelper.writelog("GETSIRLatestData : SyncList count for SIR Data is about " + CloudSyncList.Count + "");
                foreach (var CloudSir in CloudSyncList)
                {
                    try
                    {
                        SuperintendedInspectionReport LocalSIR = LocalSyncList.Where(x => x.UniqueFormID == CloudSir.UniqueFormID && x.ShipName == CloudSir.ShipName).FirstOrDefault();
                        if (LocalSIR != null)
                        {
                            InsertUpdateSIRProcess(CloudSir, LocalSIR);
                        }
                        else
                        {
                            SIRModal sirFormData = GetSIRSyncedDataFromCloud(Convert.ToString(CloudSir.UniqueFormID));
                            SaveSIRDataInLocalDB(sirFormData);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("GETSIRLatestData : SIR Data Synced Not done. Error : " + ex.Message);
                    }
                }
            }
            else
            {
                LogHelper.writelog("GETSIRLatestData : SIR Data Synced already done.");
            }
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        public List<SuperintendedInspectionReport> GetSIRFormsSyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<SuperintendedInspectionReport> SyncListFromCloud = new List<SuperintendedInspectionReport>();
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
                SyncListFromCloud = _helper.GetSIRFormsSyncedDataFromCloud(strShipCode);    // JSL 11/12/2022 added strShipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRFormsSyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncListFromCloud;
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        public List<SuperintendedInspectionReport> GetSIRFormsSyncedDataFromLocal()
        {
            List<SuperintendedInspectionReport> SyncList = new List<SuperintendedInspectionReport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID, FormVersion, ShipName, IsSynced FROM " + AppStatic.SuperintendedInspectionReport + " WHERE [UniqueFormID] IS NOT NULL", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<SuperintendedInspectionReport>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRFormsSyncedDataFromLocal " + ex.Message);
            }
            return SyncList;
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        public void InsertUpdateSIRProcess(SuperintendedInspectionReport CloudSIR, SuperintendedInspectionReport LocalSIR)
        {
            SIRModal sirFormData = new SIRModal();
            try
            {
                if (CloudSIR.FormVersion > LocalSIR.FormVersion)
                {
                    sirFormData = GetSIRSyncedDataFromCloud(Convert.ToString(CloudSIR.UniqueFormID));
                    SaveSIRDataInLocalDB(sirFormData);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertUpdateSIRProcess : " + ex.Message);
            }
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        public SIRModal GetSIRSyncedDataFromCloud(string UniqueFormID)
        {
            SIRModal SyncGIRFromCloud = new SIRModal();
            try
            {
                APIHelper _helper = new APIHelper();
                SyncGIRFromCloud = _helper.GetSIRSyncedDataFromCloud(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRSyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncGIRFromCloud;
        }
        //End RDBJ 09/27/2021
        #endregion

        public void UpdateCloudSIRFormsStatus(List<string> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds);
                APIHelper _helper = new APIHelper();
                _helper.sendSynchSIRListUFID(SuccessIds); // RDBJ 01/19/2022 set with List
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCloudGIRFormsStatus : " + ex.Message);
            }
        }
        #endregion

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
