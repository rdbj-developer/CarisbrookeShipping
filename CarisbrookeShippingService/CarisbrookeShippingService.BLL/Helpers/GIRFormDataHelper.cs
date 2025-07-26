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
    public class GIRFormDataHelper
    {
        public void StartGIRSync()
        {
            //List<GeneralInspectionReport> UnSyncList = GetGIRFormsUnsyncedData();   // JSL 07/16/2022 commented this ilne
            List<GIRModal> UnSyncList = GetGIRFormsUnsyncedData();  // JSL 07/16/2022
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for GIR Data is about " + UnSyncList.Count + "");
                List<string> SuccessIds = SendGIRDataToRemote(UnSyncList); //RDBJ 09/25/2021 Change long to string
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                {
                    UpdateLocalGIRFormsStatus(SuccessIds);
                    LogHelper.writelog("GIR Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                {
                    UpdateLocalGIRFormsStatus(SuccessIds);
                    LogHelper.writelog("Some GIR Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("GIR Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("GIR Data Synced from local to server already done.");
            }

            //RDBJ 09/25/2021 Commented This logic get AWS Copy if throw error and updated in LocalDB and Lost the last LocalDB form copy
            /*
            LogHelper.writelog("GIR Data Synced start from server to local.");
            StartGIRSyncCloudTOLocal();
            LogHelper.writelog("GIR Data Synced done from server to local.");
            */
        }
        //public List<GeneralInspectionReport> GetGIRFormsUnsyncedData()    // JSL 07/16/2022 commented this line
        public List<GIRModal> GetGIRFormsUnsyncedData()  // JSL 07/16/2022
        {
            //List<GeneralInspectionReport> SyncList = new List<GeneralInspectionReport>();   // JSL 07/16/2022 commented this line
            List<GIRModal> SyncList = new List<GIRModal>(); // JSL 07/16/20222
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GeneralInspectionReport + " WHERE ISNULL(IsSynced,0) = 0 AND [UniqueFormID] IS NOT NULL", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            //SyncList = dt.ToListof<GeneralInspectionReport>();  // JSL 07/16/2022 commented this line
                            List<GeneralInspectionReport> GIRSyncList = dt.ToListof<GeneralInspectionReport>();  // JSL 07/16/2022
                            //foreach (var item in SyncList)  // JSL 07/16/2022 commented this line
                            foreach (var item in GIRSyncList)   // JSL 07/16/2022
                            {
                                // RDBJ 01/05/2022 wrapped in if
                                if (item.isDelete == 0)
                                {
                                    try
                                    {
                                        // JSL 07/16/2022
                                        GIRModal Modal = new GIRModal();
                                        Modal.GeneralInspectionReport = item;
                                        // End  JSL 07/16/2022

                                        DataTable dtCrewDocuments = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GlRCrewDocuments + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                        sqlAdp.Fill(dtCrewDocuments);
                                        //item.GIRCrewDocuments = dtCrewDocuments.ToListof<GlRCrewDocuments>();   // JSL 07/16/2022 commented this line
                                        Modal.GeneralInspectionReport.GIRCrewDocuments = dtCrewDocuments.ToListof<GlRCrewDocuments>();  // JSL 07/16/2022

                                        DataTable dtSafeManningRequirements = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GlRSafeManningRequirements + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                        sqlAdp.Fill(dtSafeManningRequirements);
                                        //item.GIRSafeManningRequirements = dtSafeManningRequirements.ToListof<GlRSafeManningRequirements>(); // JSL 07/16/2022 commented this line
                                        Modal.GeneralInspectionReport.GIRSafeManningRequirements = dtSafeManningRequirements.ToListof<GlRSafeManningRequirements>();    // JSL 07/16/2022

                                        DataTable dtRestandWorkHours = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRRestandWorkHours + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                        sqlAdp.Fill(dtRestandWorkHours);
                                        //item.GIRRestandWorkHours = dtRestandWorkHours.ToListof<GIRRestandWorkHours>();  // JSL 07/16/2022 commented this line
                                        Modal.GeneralInspectionReport.GIRRestandWorkHours = dtRestandWorkHours.ToListof<GIRRestandWorkHours>(); // JSL 07/16/2022

                                        DataTable dtPhotographs = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRPhotographs + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                        sqlAdp.Fill(dtPhotographs);
                                        //item.GIRPhotographs = dtPhotographs.ToListof<GIRPhotographs>(); // JSL 07/16/2022 commented this line
                                        Modal.GeneralInspectionReport.GIRPhotographs = dtPhotographs.ToListof<GIRPhotographs>();    // JSL 07/16/2022
                                        // JSL 11/12/2022
                                        if (Modal.GeneralInspectionReport.GIRPhotographs != null && Modal.GeneralInspectionReport.GIRPhotographs.Count > 0)
                                        {
                                            foreach (var itemFile in Modal.GeneralInspectionReport.GIRPhotographs)
                                            {
                                                if (!itemFile.ImagePath.StartsWith("data:"))
                                                {
                                                    itemFile.ImagePath = Utility.ConvertIntoBase64EndCodedUploadedFile(itemFile.ImagePath);
                                                }
                                            }
                                        }
                                        // End JSL 11/12/2022

                                        DataTable dtDeficiencies = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE UniqueFormID = '" + item.UniqueFormID + "' AND ReportType = 'GI'", conn);
                                        sqlAdp.Fill(dtDeficiencies);
                                        //item.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>();  // JSL 07/16/2022 commented this line
                                        Modal.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>(); // JSL 07/16/2022

                                        //if (item.GIRDeficiencies != null && item.GIRDeficiencies.Count > 0) // JSL 07/16/2022 commented this line
                                        if (Modal.GIRDeficiencies != null && Modal.GIRDeficiencies.Count > 0)   // JSL 07/16/2022
                                        {
                                            //foreach (var def in item.GIRDeficiencies)   // JSL 07/16/2022 commented this line
                                            foreach (var def in Modal.GIRDeficiencies)   // JSL 07/16/2022
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
                                                            // JSL 11/12/2022
                                                            if (!deffile.StorePath.StartsWith("data:"))
                                                            {
                                                                deffile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(deffile.StorePath);
                                                            }
                                                            // End JSL 11/12/2022
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
                                                            // JSL 11/12/2022
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
                                                            // End JSL 11/12/2022
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
                                                            // JSL 11/12/2022
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
                                                            // End JSL 11/12/2022
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
                                        SyncList.Add(Modal);    // JSL 07/16/2022
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.writelog("GIRFormsData Error : GIRFormID : " + item.GIRFormID + " " + ex.Message);
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
                LogHelper.writelog("GetGIRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        //public List<string> SendGIRDataToRemote(List<GeneralInspectionReport> UnSyncList)   // JSL 07/16/2022 commented this line //RDBJ 09/25/2021 Change long to string return
        public List<string> SendGIRDataToRemote(List<GIRModal> UnSyncList)   // JSL 07/16/2022 //RDBJ 09/25/2021 Change long to string return
        {
            // JSL 07/16/2022
            APIResponse res = new APIResponse();
            APIHelper _apiHelper = new APIHelper();
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            // End JSL 07/16/2022

            List<string> SuccessIds = new List<string>(); //RDBJ 09/25/2021 Change long to string
            foreach (var item in UnSyncList)
            {
                // JSL 07/16/2022 commented this line
                /*
                string localDBGIRUniqueFormID = Convert.ToString(item.UniqueFormID); //RDBJ 09/25/2021
                item.GIRFormID = 0;
                CloudLocalGIRSynch _helper = new CloudLocalGIRSynch();
                res = _helper.sendGIRLocalToRemote(item);
                */
                // End JSL 07/16/2022 commented this line
                

                // JSL 07/18/2022 commented
                // JSL 07/16/2022
                if (item.GeneralInspectionReport != null && item.GeneralInspectionReport.UniqueFormID != null)
                {
                    item.GeneralInspectionReport.GIRFormID = 0;
                    CloudLocalGIRSynch _helper = new CloudLocalGIRSynch();
                    bool IsAllowToSendDataForServer = false;
                    Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                    dictMetaData["FormType"] = AppStatic.GIRForm;
                    dictMetaData["FormUniqueID"] = item.GeneralInspectionReport.UniqueFormID.ToString();
                    dictMetaData["FormVersion"] = item.GeneralInspectionReport.FormVersion.ToString();
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
                                GIRModal modalGIRToSendServer = new GIRModal();
                                modalGIRToSendServer.GeneralInspectionReport = item.GeneralInspectionReport;
                                //modalGIRToSendServer.GIRDeficiencies = new List<GIRDeficiencies>(); // JSL 09/12/2022 commented 
                                modalGIRToSendServer.GeneralInspectionReport.GIRDeficiencies = new List<GIRDeficiencies>(); // JSL 09/12/2022

                                // JSL 10/28/2022 reorder conditions
                                res = new APIResponse();
                                foreach (var itemDeficiencies in item.GIRDeficiencies)
                                {
                                    dictMetaData["ShipCode"] = itemDeficiencies.Ship;
                                    dictMetaData["DeficienciesData"] = JsonConvert.SerializeObject(itemDeficiencies);
                                    dictMetaData["strAction"] = AppStatic.API_METHOD_InsertOrUpdateDeficienciesData;

                                    retDictMetaData = _apiHelper.PostAsyncAPICall(AppStatic.APICloudGIR, AppStatic.API_CommonPostAPICall, dictMetaData);
                                    if (retDictMetaData != null)
                                    {
                                        if (retDictMetaData["Status"] == AppStatic.ERROR)
                                        {
                                            LogHelper.writelog("GIR Data Synced Error for Deficienciy : " + itemDeficiencies.DeficienciesUniqueID.ToString());
                                        }
                                    }
                                    else
                                    {
                                        retDictMetaData["Status"] = AppStatic.ERROR;
                                    }
                                    dictMetaData["DeficienciesData"] = "";  // JSL 01/03/2023
                                }
                                res.result = retDictMetaData["Status"];

                                // JSL 01/03/2023
                                if (res != null && res.result == AppStatic.SUCCESS)
                                {
                                    // Delete GIRPhotographs first
                                    dictMetaData["strAction"] = AppStatic.API_METHOD_DeleteGIRPhotographsData;
                                    retDictMetaData = _apiHelper.PostAsyncAPICall(AppStatic.APICloudGIR, AppStatic.API_CommonPostAPICall, dictMetaData);
                                    if (retDictMetaData != null)
                                    {
                                        if (retDictMetaData["Status"] == AppStatic.ERROR)
                                        {
                                            LogHelper.writelog("GIR Photograph Delete Data Error : " + item.GeneralInspectionReport.UniqueFormID.ToString());
                                        }
                                    }
                                    else
                                    {
                                        retDictMetaData["Status"] = AppStatic.ERROR;
                                    }
                                }
                                res.result = retDictMetaData["Status"];


                                if (res != null && res.result == AppStatic.SUCCESS)
                                {
                                    foreach (var itemGIRPhotographs in item.GeneralInspectionReport.GIRPhotographs)
                                    {
                                        // Upload GIRPhotographs
                                        dictMetaData["ShipCode"] = itemGIRPhotographs.Ship;
                                        dictMetaData["PhotographData"] = JsonConvert.SerializeObject(itemGIRPhotographs);
                                        dictMetaData["strAction"] = AppStatic.API_METHOD_InsertOrUpdateGIRPhotographsData;

                                        retDictMetaData = _apiHelper.PostAsyncAPICall(AppStatic.APICloudGIR, AppStatic.API_CommonPostAPICall, dictMetaData);
                                        if (retDictMetaData != null)
                                        {
                                            if (retDictMetaData["Status"] == AppStatic.ERROR)
                                            {
                                                LogHelper.writelog("GIR Photograph Data Synced Error : " + itemGIRPhotographs.PhotographsID.ToString());
                                            }
                                        }
                                        else
                                        {
                                            retDictMetaData["Status"] = AppStatic.ERROR;
                                        }
                                        dictMetaData["PhotographData"] = "";  // JSL 01/03/2023
                                    }
                                }
                                // End JSL 01/03/2023

                                res.result = retDictMetaData["Status"];
                                if (res != null && res.result == AppStatic.SUCCESS)
                                {
                                    modalGIRToSendServer.GeneralInspectionReport.GIRPhotographs = new List<GIRPhotographs>(); // JSL 01/03/2023
                                    res = new APIResponse();
                                    res = _helper.sendGIRLocalToRemote(modalGIRToSendServer.GeneralInspectionReport);
                                }
                                // End JSL 10/28/2022 reorder conditions
                            }
                        }
                    }
                }
                // End JSL 07/16/2022
                // JSL 07/18/2022 commented

                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    //SuccessIds.Add(localDBGIRUniqueFormID);     // JSL 07/16/2022 commented this line //RDBJ 09/25/2021 changed localDBGIRFormID
                    SuccessIds.Add(Convert.ToString(item.GeneralInspectionReport.UniqueFormID)); // JSL 07/16/2022
                }
            }
            return SuccessIds;
        }
        public void UpdateLocalGIRFormsStatus(List<string> SuccessIds) //RDBJ 09/25/2021 Change List<long> to List<string>
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
                        string Query = "UPDATE " + AppStatic.GeneralInspectionReport + " SET IsSynced = 1 WHERE UniqueFormID in (" + IdsStr + ")"; //RDBJ 09/25/2021 Updated UniqueFormID from GIRFormId
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalGIRFormsStatus : " + ex.Message);
            }
        }

        #region getGIRCloudData
        public void StartGIRSyncCloudTOLocal(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<GeneralInspectionReport> UnSyncList = GetGIRFormsUnsyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine  // JSL 11/12/2022
                );
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for GIR Data is about " + UnSyncList.Count + "");
                List<string> SuccessIds = SendGIRDataToLocal(UnSyncList);
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                {
                    UpdateCloudGIRFormsStatus(SuccessIds);
                    LogHelper.writelog("GIR Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                {
                    //UpdateLocalGIRFormsStatus(SuccessIds);
                    LogHelper.writelog("Some GIR Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("GIR Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("GIR Data Synced already done.");
            }
        }
        public List<GeneralInspectionReport> GetGIRFormsUnsyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<GeneralInspectionReport> UnSyncListFromCloud = new List<GeneralInspectionReport>();
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
                UnSyncListFromCloud = _helper.GetGIRGeneralDescription(strShipCode);    // JSL 11/12/2022 added strShipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRFormsUnsyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return UnSyncListFromCloud;
        }
        public List<string> SendGIRDataToLocal(List<GeneralInspectionReport> UnSyncList)
        {
            List<string> SuccessIds = new List<string>();
            foreach (var item in UnSyncList)
            {
                string localDBGIRUniqueFormID = Convert.ToString(item.UniqueFormID);
                item.GIRFormID = 0;
                APIResponse res = SaveGIRDataInLocalDB(item);

                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBGIRUniqueFormID);
                }
            }
            return SuccessIds;
        }
        public APIResponse SaveGIRDataInLocalDB(GeneralInspectionReport Modal)
        {
            APIResponse res = new APIResponse();
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);

            string UniqueFormID = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID FROM " + AppStatic.GeneralInspectionReport + " WHERE UniqueFormID = '" + Modal.UniqueFormID + "'", connection);
                sqlAdp.Fill(dt);

                Modal.IsSynced = true;
                UniqueFormID = Convert.ToString(Modal.UniqueFormID);
                if (dt.Rows.Count > 0)
                {
                    var girLocalList = dt.ToListof<GeneralInspectionReport>().FirstOrDefault();
                    if (!girLocalList.FormVersion.HasValue || Modal.FormVersion > girLocalList.FormVersion)
                    {
                        string UpdateQury = GETGIRUpdateQuery();
                        SqlCommand command = new SqlCommand(UpdateQury, connection);

                        GIRUpdateCMD(Modal, ref command);
                        SetGIRModal(ref Modal); //RDBJ 09/20/2021 

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    string InsertQury = GeneralInspectionReportDataInsertQuery();
                    SqlCommand command = new SqlCommand(InsertQury, connection);

                    GeneralInspectionReportDataInsertCMD(Modal, ref command);
                    SetGIRModal(ref Modal); //RDBJ 09/20/2021 

                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }
                //RDBJ 09/20/2021 Added below If
                if (!string.IsNullOrEmpty(UniqueFormID))
                {
                    GIRDeficiencies_Save(Guid.Parse(UniqueFormID), Modal.GIRDeficiencies
                        , (bool)Modal.SavedAsDraft  // JSL 05/23/2022
                        );

                    //RDBJ 09/29/2021
                    GIRSafeManningRequirements_Save(Guid.Parse(UniqueFormID), Modal.GIRSafeManningRequirements);
                    GIRCrewDocuments_Save(Guid.Parse(UniqueFormID), Modal.GIRCrewDocuments);
                    GIRRestandWorkHours_Save(Guid.Parse(UniqueFormID), Modal.GIRRestandWorkHours);
                    GIRPhotos_Save(Guid.Parse(UniqueFormID), Modal.GIRPhotographs);
                    //End RDBJ 09/29/2021
                }

                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GeneralInspectionReport table Error : " + ex.Message.ToString());
                LogHelper.writelog("Add Local DB in GeneralInspectionReport table : " + UniqueFormID + "  Inner Error : " + ex.InnerException.ToString()); // RDBJ 01/21/2022
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

        //RDBJ 09/20/2021 
        public void SetGIRModal(ref GeneralInspectionReport Modal)
        {
            if (Modal.GIRSafeManningRequirements != null && Modal.GIRSafeManningRequirements.Count > 0)
            {
                foreach (var item in Modal.GIRSafeManningRequirements)
                {
                    item.Ship = item.Ship;
                    item.OnBoard = Convert.ToBoolean(item.OnBoard);
                    item.RequiredbySMD = Convert.ToBoolean(item.RequiredbySMD);
                }
            }
            if (Modal.GIRCrewDocuments != null && Modal.GIRCrewDocuments.Count > 0)
            {
                foreach (var item in Modal.GIRCrewDocuments)
                {
                    item.Ship = item.Ship;
                    item.CreatedDate = item.CreatedDate; //DateTime.Now;
                    item.UpdatedDate = item.UpdatedDate; //DateTime.Now;
                }
            }
            if (Modal.GIRRestandWorkHours != null && Modal.GIRRestandWorkHours.Count > 0)
            {
                foreach (var item in Modal.GIRRestandWorkHours)
                {
                    item.Ship = item.Ship;
                }
            }
            if (Modal.GIRDeficiencies != null && Modal.GIRDeficiencies.Count > 0)
            {
                foreach (var item in Modal.GIRDeficiencies)
                {
                    item.Ship = item.Ship;
                    item.IsClose = Convert.ToBoolean(item.IsClose);
                    item.ReportType = item.ReportType;
                }
            }
            if (Modal.GIRPhotographs != null && Modal.GIRPhotographs.Count > 0)
            {
                foreach (var item in Modal.GIRPhotographs)
                {
                    item.Ship = item.Ship;
                }
            }
        }
        //End RDBJ 09/20/2021 

        #region GIR Insert/Update
        public string GeneralInspectionReportDataInsertQuery()
        {
            // RDBJ 01/05/2022 Added isDelete
            //RDBJ 10/20/2021 Added 
            /*
            IsGeneralSectionComplete,IsManningSectionComplete,IsShipCertificationSectionComplete,IsRecordKeepingSectionComplete,
            IsSafetyEquipmentSectionComplete,IsSecuritySectionComplete,IsBridgeSectionComplete,IsMedicalSectionComplete,IsGalleySectionComplete,
            IsEngineRoomSectionComplete,IsSuperstructureSectionComplete,IsDeckSectionComplete,IsHoldsAndCoverSectionComplete,IsForeCastleSectionComplete,
            IsHullSectionComplete,IsSummarySectionComplete,IsDeficienciesSectionComplete,IsPhotographsSectionComplete
             */
            string InsertQury = @"INSERT INTO dbo.GeneralInspectionReport 
                                (UniqueFormID,FormVersion,ShipID,ShipName,Ship,Port,Inspector,Date,GeneralPreamble,Classsociety,YearofBuild,Flag,Classofvessel,
			                    Portofregistry,MMSI,IMOnumber,Callsign,SummerDWT,Grosstonnage,Lightweight,Nettonnage,Beam,LOA,Summerdraft,
			                    LBP,Bowthruster,BHP,Noofholds,Nomoveablebulkheads,Containers,Cargocapacity,Cargohandlingequipment,Lastvoyageandcargo,CurrentPlannedvoyageandcargo,
                                ShipboardWorkingArrangements,CertificationIndex,IsPubsAndDocsSectionComplete,CarriedOutByTheDOOW,IsRegs4shipsDVD,Regs4shipsDVD,IsSOPEPPoints,SOPEPPoints,IsBWMP,BWMP,IsBWMPSupplement,
                                BWMPSupplement,IsIntactStabilityManual,IntactStabilityManual,IsStabilityComputer,StabilityComputer,IsDateOfLast,DateOfLast,IsCargoSecuring,CargoSecuring,IsBulkCargo,BulkCargo,IsSMSManual,
                                SMSManual,IsRegisterOf,RegisterOf,IsFleetStandingOrder,FleetStandingOrder,IsFleetMemoranda,FleetMemoranda,IsShipsPlans,ShipsPlans,IsCollective,Collective,IsDraftAndFreeboardNotice,
                                DraftAndFreeboardNotice,IsPCSOPEP,PCSOPEP,IsNTVRP,NTVRP,IsVGP,VGP,PubsComments,OfficialLogbookA,OfficialLogbookB,OfficialLogbookC,OfficialLogbookD,OfficialLogbookE,DeckLogbook,
                                Listofcrew,LastHose,PassagePlanning,LoadingComputer,EngineLogbook,OilRecordBook,RiskAssessments,GMDSSLogbook,DeckLogbook5D,GarbageRecordBook,BallastWaterRecordBook,CargoRecordBook,EmissionsControlManual,
                                LGR,PEER,RecordKeepingComments,LastPortStateControl,LiferaftsComment,releasesComment,LifeboatComment,LifeboatdavitComment,LifeboatequipmentComment,RescueboatComment,
                                RescueboatequipmentComment,RescueboatoutboardmotorComment,RescueboatdavitComment,DeckComment,PyrotechnicsComment,EPIRBComment,SARTsComment,GMDSSComment,ManoverboardComment,
                                LinethrowingapparatusComment,FireextinguishersComment,EmergencygeneratorComment,CO2roomComment,SurvivalComment,LifejacketComment,FiremansComment,LifebuoysComment,FireboxesComment,
                                EmergencybellsComment,EmergencylightingComment,FireplanComment,DamageComment,EmergencyplansComment,MusterlistComment,SafetysignsComment,EmergencysteeringComment,
                                StatutoryemergencydrillsComment,EEBDComment,OxygenComment,MultigasdetectorComment,GasdetectorComment,SufficientquantityComment,BASetsComment,SafetyComment,GangwayComment,RestrictedComment,
                                OutsideComment,EntrancedoorsComment,AccommodationComment,GMDSSComment5G,VariousComment,SSOComment,SecuritylogbookComment,Listoflast10portsComment,PFSOComment,
                                SecuritylevelComment,DrillsandtrainingComment,DOSComment,SSASComment,VisitorslogbookComment,KeyregisterComment,ShipSecurityComment,SecurityComment,NauticalchartsComment,
                                NoticetomarinersComment,ListofradiosignalsComment,ListoflightsComment,SailingdirectionsComment,TidetablesComment,NavtexandprinterComment,RadarsComment,GPSComment,AISComment,
                                VDRComment,ECDISComment,EchosounderComment,ADPbackuplaptopComment,ColourprinterComment,VHFDSCtransceiverComment,radioinstallationComment,InmarsatCComment
                                ,MagneticcompassComment,SparecompassbowlComment,CompassobservationbookComment,GyrocompassComment,RudderindicatorComment,SpeedlogComment,NavigationComment,SignalflagsComment
                                ,RPMComment,BasicmanoeuvringdataComment,MasterstandingordersComment,MasternightordersbookComment,SextantComment,AzimuthmirrorComment,BridgepostersComment,ReviewofplannedComment
                                ,BridgebellbookComment,BridgenavigationalComment,SecurityEquipmentComment,NavigationPost,GeneralComment,MedicinestorageComment,MedicinechestcertificateComment
                                ,InventoryStoresComment,OxygencylindersComment,StretcherComment,SalivaComment,AlcoholComment,HospitalComment,GeneralGalleyComment,HygieneComment,FoodstorageComment
                                ,FoodlabellingComment,GalleyriskassessmentComment,FridgetemperatureComment,FoodandProvisionsComment,GalleyComment,ConditionComment,PaintworkComment,LightingComment,PlatesComment
                                ,BilgesComment,PipelinesandvalvesComment,LeakageComment,EquipmentComment,OilywaterseparatorComment,FueloiltransferplanComment,SteeringgearComment,WorkshopandequipmentComment
                                ,SoundingpipesComment,EnginecontrolComment,ChiefEngineernightordersbookComment,ChiefEngineerstandingordersComment,PreUMSComment,EnginebellbookComment,LockoutComment
                                ,EngineRoomComment,CleanlinessandhygieneComment,ConditionComment5M,PaintworkComment5M,SignalmastandstaysComment,MonkeyislandComment,FireDampersComment,RailsBulwarksComment
                                ,WatertightdoorsComment,VentilatorsComment,WinchesComment,FairleadsComment,MooringLinesComment,EmergencyShutOffsComment,RadioaerialsComment,SOPEPlockerComment,ChemicallockerComment
                                ,AntislippaintComment,SuperstructureComment,CabinsComment,OfficesComment,MessroomsComment,ToiletsComment,LaundryroomComment,ChangingroomComment,OtherComment,ConditionComment5N
                                ,SelfclosingfiredoorsComment,StairwellsComment,SuperstructureInternalComment,PortablegangwayComment,SafetynetComment,AccommodationLadderComment,SafeaccessprovidedComment
                                ,PilotladdersComment,BoardingEquipmentComment,CleanlinessComment,PaintworkComment5P,ShipsiderailsComment,WeathertightdoorsComment,FirehydrantsComment,VentilatorsComment5P
                                ,ManholecoversComment,MainDeckAreaComment,ConditionComment5Q,PaintworkComment5Q,MechanicaldamageComment,AccessladdersComment,ManholecoversComment5Q,HoldbilgeComment
                                ,AccessdoorsComment,ConditionHatchCoversComment,PaintworkHatchCoversComment,RubbersealsComment,SignsofhatchesComment,SealingtapeComment,ConditionofhydraulicsComment
                                ,PortablebulkheadsComment,TweendecksComment,HatchcoamingComment,ConditionCargoCranesComment,GantrycranealarmComment,GantryconditionComment,CargoHoldsComment
                                ,CleanlinessComment5R,PaintworkComment5R,TriphazardsComment,WindlassComment,CablesComment,WinchesComment5R,FairleadsComment5R,MooringComment,HatchToforecastlespaceComment
                                ,VentilatorsComment5R,BellComment,ForemastComment,FireComment,RailsComment,AntislippaintComment5R
                                ,SnapBackZoneComment,ConditionGantryCranesComment
                                ,SnapBackZone5NComment,MedicalLogBookComment,DrugsNarcoticsComment,DefibrillatorComment
                                ,RPWaterHandbook,BioRPWH,PRE,NoiseVibrationFile,BioMPR,AsbestosPlan,ShipPublicAddrComment
                                ,BridgewindowswiperssprayComment,BridgewindowswipersComment,DaylightSignalsComment
                                ,LiferaftDavitComment,CylindersLockerComment,ADPPublicationsComment
                                ,ForecastleComment,CleanlinessComment5S,PaintworkComment5S,ForepeakComment
                                ,ChainlockerComment,LightingComment5S,AccesssafetychainComment,EmergencyfirepumpComment,BowthrusterandroomComment,SparemooringlinesComment,PaintlockerComment,ForecastleSpaceComment
                                ,BoottopComment,TopsidesComment,AntifoulingComment,DraftandplimsollComment,FoulingComment,MechanicalComment,HullComment,SummaryComment,
                                IsSynced,CreatedDate,UpdatedDate,SavedAsDraft,IsGeneralSectionComplete,IsManningSectionComplete,IsShipCertificationSectionComplete,IsRecordKeepingSectionComplete,
                                IsSafetyEquipmentSectionComplete,IsSecuritySectionComplete,IsBridgeSectionComplete,IsMedicalSectionComplete,IsGalleySectionComplete,
                                IsEngineRoomSectionComplete,IsSuperstructureSectionComplete,IsDeckSectionComplete,IsHoldsAndCoverSectionComplete,IsForeCastleSectionComplete,
                                IsHullSectionComplete,IsSummarySectionComplete,IsDeficienciesSectionComplete,IsPhotographsSectionComplete, isDelete)
                                OUTPUT INSERTED.GIRFormID
                                VALUES (@UniqueFormID,@FormVersion,@ShipID,@ShipName,@Ship,@Port,@Inspector,@Date,@GeneralPreamble,@Classsociety,@YearofBuild,@Flag,@Classofvessel,
                                @Portofregistry,@MMSI,@IMOnumber,@Callsign,@SummerDWT,@Grosstonnage,@Lightweight,@Nettonnage,@Beam,@LOA,@Summerdraft,
                                @LBP,@Bowthruster,@BHP,@Noofholds,@Nomoveablebulkheads,@Containers,@Cargocapacity,@Cargohandlingequipment,@Lastvoyageandcargo,@CurrentPlannedvoyageandcargo,
                                @ShipboardWorkingArrangements,@CertificationIndex,@IsPubsAndDocsSectionComplete,@CarriedOutByTheDOOW,@IsRegs4shipsDVD,@Regs4shipsDVD,@IsSOPEPPoints,@SOPEPPoints,@IsBWMP,@BWMP,@IsBWMPSupplement,
                                @BWMPSupplement,@IsIntactStabilityManual,@IntactStabilityManual,@IsStabilityComputer,@StabilityComputer,@IsDateOfLast,@DateOfLast,@IsCargoSecuring,@CargoSecuring,@IsBulkCargo,@BulkCargo,@IsSMSManual,
                                @SMSManual, @IsRegisterOf,@RegisterOf,@IsFleetStandingOrder,@FleetStandingOrder,@IsFleetMemoranda,@FleetMemoranda,@IsShipsPlans,@ShipsPlans,@IsCollective,@Collective,@IsDraftAndFreeboardNotice,
                                @DraftAndFreeboardNotice,@IsPCSOPEP,@PCSOPEP,@IsNTVRP,@NTVRP,@IsVGP,@VGP,@PubsComments,@OfficialLogbookA,@OfficialLogbookB,@OfficialLogbookC,@OfficialLogbookD,@OfficialLogbookE,@DeckLogbook,
                                @Listofcrew,@LastHose,@PassagePlanning,@LoadingComputer,@EngineLogbook,@OilRecordBook,@RiskAssessments,@GMDSSLogbook,@DeckLogbook5D,@GarbageRecordBook,@BallastWaterRecordBook,@CargoRecordBook,@EmissionsControlManual,
                                @LGR,@PEER,@RecordKeepingComments,@LastPortStateControl,@LiferaftsComment,@releasesComment,@LifeboatComment,@LifeboatdavitComment,@LifeboatequipmentComment,@RescueboatComment,
                                @RescueboatequipmentComment,@RescueboatoutboardmotorComment,@RescueboatdavitComment,@DeckComment,@PyrotechnicsComment,@EPIRBComment,@SARTsComment,@GMDSSComment,@ManoverboardComment,
                                @LinethrowingapparatusComment,@FireextinguishersComment,@EmergencygeneratorComment,@CO2roomComment,@SurvivalComment,@LifejacketComment,@FiremansComment,@LifebuoysComment,@FireboxesComment,
                                @EmergencybellsComment,@EmergencylightingComment,@FireplanComment,@DamageComment,@EmergencyplansComment,@MusterlistComment,@SafetysignsComment,@EmergencysteeringComment,
                                @StatutoryemergencydrillsComment,@EEBDComment,@OxygenComment,@MultigasdetectorComment,@GasdetectorComment,@SufficientquantityComment,@BASetsComment,@SafetyComment,@GangwayComment,@RestrictedComment,
                                @OutsideComment,@EntrancedoorsComment,@AccommodationComment,@GMDSSComment5G,@VariousComment,@SSOComment,@SecuritylogbookComment,@Listoflast10portsComment,@PFSOComment,
                                @SecuritylevelComment,@DrillsandtrainingComment,@DOSComment,@SSASComment,@VisitorslogbookComment,@KeyregisterComment,@ShipSecurityComment,@SecurityComment,@NauticalchartsComment,
                                @NoticetomarinersComment,@ListofradiosignalsComment,@ListoflightsComment,@SailingdirectionsComment,@TidetablesComment,@NavtexandprinterComment,@RadarsComment,@GPSComment,@AISComment,
                                @VDRComment,@ECDISComment,@EchosounderComment,@ADPbackuplaptopComment,@ColourprinterComment,@VHFDSCtransceiverComment,@radioinstallationComment,@InmarsatCComment
                                ,@MagneticcompassComment,@SparecompassbowlComment,@CompassobservationbookComment,@GyrocompassComment,@RudderindicatorComment,@SpeedlogComment,@NavigationComment,@SignalflagsComment
                                ,@RPMComment,@BasicmanoeuvringdataComment,@MasterstandingordersComment,@MasternightordersbookComment,@SextantComment,@AzimuthmirrorComment,@BridgepostersComment,@ReviewofplannedComment
                                ,@BridgebellbookComment,@BridgenavigationalComment,@SecurityEquipmentComment,@NavigationPost,@GeneralComment,@MedicinestorageComment,@MedicinechestcertificateComment
                                ,@InventoryStoresComment,@OxygencylindersComment,@StretcherComment,@SalivaComment,@AlcoholComment,@HospitalComment,@GeneralGalleyComment,@HygieneComment,@FoodstorageComment
                                ,@FoodlabellingComment,@GalleyriskassessmentComment,@FridgetemperatureComment,@FoodandProvisionsComment,@GalleyComment,@ConditionComment,@PaintworkComment,@LightingComment,@PlatesComment
                                ,@BilgesComment,@PipelinesandvalvesComment,@LeakageComment,@EquipmentComment,@OilywaterseparatorComment,@FueloiltransferplanComment,@SteeringgearComment,@WorkshopandequipmentComment
                                ,@SoundingpipesComment,@EnginecontrolComment,@ChiefEngineernightordersbookComment,@ChiefEngineerstandingordersComment,@PreUMSComment,@EnginebellbookComment,@LockoutComment
                                ,@EngineRoomComment,@CleanlinessandhygieneComment,@ConditionComment5M,@PaintworkComment5M,@SignalmastandstaysComment,@MonkeyislandComment,@FireDampersComment,@RailsBulwarksComment
                                ,@WatertightdoorsComment,@VentilatorsComment,@WinchesComment,@FairleadsComment,@MooringLinesComment,@EmergencyShutOffsComment,@RadioaerialsComment,@SOPEPlockerComment,@ChemicallockerComment
                                ,@AntislippaintComment,@SuperstructureComment,@CabinsComment,@OfficesComment,@MessroomsComment,@ToiletsComment,@LaundryroomComment,@ChangingroomComment,@OtherComment,@ConditionComment5N
                                ,@SelfclosingfiredoorsComment,@StairwellsComment,@SuperstructureInternalComment,@PortablegangwayComment,@SafetynetComment,@AccommodationLadderComment,@SafeaccessprovidedComment
                                ,@PilotladdersComment,@BoardingEquipmentComment,@CleanlinessComment,@PaintworkComment5P,@ShipsiderailsComment,@WeathertightdoorsComment,@FirehydrantsComment,@VentilatorsComment5P
                                ,@ManholecoversComment,@MainDeckAreaComment,@ConditionComment5Q,@PaintworkComment5Q,@MechanicaldamageComment,@AccessladdersComment,@ManholecoversComment5Q,@HoldbilgeComment
                                ,@AccessdoorsComment,@ConditionHatchCoversComment,@PaintworkHatchCoversComment,@RubbersealsComment,@SignsofhatchesComment,@SealingtapeComment,@ConditionofhydraulicsComment
                                ,@PortablebulkheadsComment,@TweendecksComment,@HatchcoamingComment,@ConditionCargoCranesComment,@GantrycranealarmComment,@GantryconditionComment,@CargoHoldsComment
                                ,@CleanlinessComment5R,@PaintworkComment5R,@TriphazardsComment,@WindlassComment,@CablesComment,@WinchesComment5R,@FairleadsComment5R,@MooringComment,@HatchToforecastlespaceComment
                                ,@VentilatorsComment5R,@BellComment,@ForemastComment,@FireComment,@RailsComment,@AntislippaintComment5R
                                ,@SnapBackZoneComment,@ConditionGantryCranesComment
                                ,@SnapBackZone5NComment,@MedicalLogBookComment,@DrugsNarcoticsComment,@DefibrillatorComment
                                ,@RPWaterHandbook,@BioRPWH,@PRE,@NoiseVibrationFile,@BioMPR,@AsbestosPlan,@ShipPublicAddrComment
                                ,@BridgewindowswiperssprayComment,@BridgewindowswipersComment,@DaylightSignalsComment
                                ,@LiferaftDavitComment,@CylindersLockerComment,@ADPPublicationsComment
                                ,@ForecastleComment,@CleanlinessComment5S,@PaintworkComment5S,@ForepeakComment
                                ,@ChainlockerComment,@LightingComment5S,@AccesssafetychainComment,@EmergencyfirepumpComment,@BowthrusterandroomComment,@SparemooringlinesComment,@PaintlockerComment,@ForecastleSpaceComment
                                ,@BoottopComment,@TopsidesComment,@AntifoulingComment,@DraftandplimsollComment,@FoulingComment,@MechanicalComment,@HullComment,@SummaryComment,
                                @IsSynced,@CreatedDate,@UpdatedDate,@SavedAsDraft,@IsGeneralSectionComplete,@IsManningSectionComplete,@IsShipCertificationSectionComplete,@IsRecordKeepingSectionComplete
                                ,@IsSafetyEquipmentSectionComplete,@IsSecuritySectionComplete,@IsBridgeSectionComplete,@IsMedicalSectionComplete,@IsGalleySectionComplete,@IsEngineRoomSectionComplete
                                ,@IsSuperstructureSectionComplete,@IsDeckSectionComplete,@IsHoldsAndCoverSectionComplete,@IsForeCastleSectionComplete,@IsHullSectionComplete,@IsSummarySectionComplete
                                ,@IsDeficienciesSectionComplete,@IsPhotographsSectionComplete, @isDelete)";
            return InsertQury;
        }
        public void GeneralInspectionReportDataInsertCMD(GeneralInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.FormVersion; //RDBJ 09/29/2021 It was get only Integer value rather than Decimal too (set SqlDbType.Decimal)
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete == null ? DBNull.Value : (object)Modal.isDelete; // RDBJ 01/05/2022

            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID == null ? DBNull.Value : (object)Modal.ShipID;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName == null ? string.Empty : Modal.ShipName;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? string.Empty : Modal.Ship;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port == null ? string.Empty : Modal.Port;
            command.Parameters.Add("@Inspector", SqlDbType.NVarChar).Value = Modal.Inspector == null ? string.Empty : Modal.Inspector;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date == null ? DBNull.Value : (object)Modal.Date;
            command.Parameters.Add("@GeneralPreamble", SqlDbType.NVarChar).Value = Modal.GeneralPreamble != null ? Modal.GeneralPreamble : "";
            command.Parameters.Add("@Classsociety", SqlDbType.NVarChar).Value = Modal.Classsociety != null ? Modal.Classsociety : "";
            command.Parameters.Add("@YearofBuild", SqlDbType.NVarChar).Value = Modal.YearofBuild != null ? Modal.YearofBuild : "";
            command.Parameters.Add("@Flag", SqlDbType.NVarChar).Value = Modal.Flag != null ? Modal.Flag : "";
            command.Parameters.Add("@Classofvessel", SqlDbType.NVarChar).Value = Modal.Classofvessel != null ? Modal.Classofvessel : "";
            command.Parameters.Add("@Portofregistry", SqlDbType.NVarChar).Value = Modal.Portofregistry != null ? Modal.Portofregistry : "";
            command.Parameters.Add("@MMSI", SqlDbType.NVarChar).Value = Modal.MMSI != null ? Modal.MMSI : "";
            command.Parameters.Add("@IMOnumber", SqlDbType.NVarChar).Value = Modal.IMOnumber != null ? Modal.IMOnumber : "";
            command.Parameters.Add("@Callsign", SqlDbType.NVarChar).Value = Modal.Callsign != null ? Modal.Callsign : "";
            command.Parameters.Add("@SummerDWT", SqlDbType.NVarChar).Value = Modal.SummerDWT != null ? Modal.SummerDWT : "";
            command.Parameters.Add("@Grosstonnage", SqlDbType.NVarChar).Value = Modal.Grosstonnage != null ? Modal.Grosstonnage : "";
            command.Parameters.Add("@Lightweight", SqlDbType.NVarChar).Value = Modal.Lightweight != null ? Modal.Lightweight : "";
            command.Parameters.Add("@Nettonnage", SqlDbType.NVarChar).Value = Modal.Nettonnage != null ? Modal.Nettonnage : "";
            command.Parameters.Add("@Beam", SqlDbType.NVarChar).Value = Modal.Beam != null ? Modal.Beam : "";
            command.Parameters.Add("@LOA", SqlDbType.NVarChar).Value = Modal.LOA != null ? Modal.LOA : "";
            command.Parameters.Add("@Summerdraft", SqlDbType.NVarChar).Value = Modal.Summerdraft != null ? Modal.Summerdraft : "";
            command.Parameters.Add("@LBP", SqlDbType.NVarChar).Value = Modal.LBP != null ? Modal.LBP : "";
            command.Parameters.Add("@Bowthruster", SqlDbType.NVarChar).Value = Modal.Bowthruster != null ? Modal.Bowthruster : "";
            command.Parameters.Add("@BHP", SqlDbType.NVarChar).Value = Modal.BHP != null ? Modal.BHP : "";
            command.Parameters.Add("@Noofholds", SqlDbType.NVarChar).Value = Modal.Noofholds != null ? Modal.Noofholds : "";
            command.Parameters.Add("@Nomoveablebulkheads", SqlDbType.NVarChar).Value = Modal.Nomoveablebulkheads != null ? Modal.Nomoveablebulkheads : "";
            command.Parameters.Add("@Containers", SqlDbType.NVarChar).Value = Modal.Containers != null ? Modal.Containers : "";
            command.Parameters.Add("@Cargocapacity", SqlDbType.NVarChar).Value = Modal.Cargocapacity != null ? Modal.Cargocapacity : "";
            command.Parameters.Add("@Cargohandlingequipment", SqlDbType.NVarChar).Value = Modal.Cargohandlingequipment != null ? Modal.Cargohandlingequipment : "";
            command.Parameters.Add("@Lastvoyageandcargo", SqlDbType.NVarChar).Value = Modal.Lastvoyageandcargo != null ? Modal.Lastvoyageandcargo : "";
            command.Parameters.Add("@CurrentPlannedvoyageandcargo", SqlDbType.NVarChar).Value = Modal.CurrentPlannedvoyageandcargo != null ? Modal.CurrentPlannedvoyageandcargo : "";

            command.Parameters.Add("@ShipboardWorkingArrangements", SqlDbType.NVarChar).Value = Modal.ShipboardWorkingArrangements != null ? Modal.ShipboardWorkingArrangements : "";
            command.Parameters.Add("@CertificationIndex", SqlDbType.NVarChar).Value = Modal.CertificationIndex != null ? Modal.CertificationIndex : "";
            command.Parameters.Add("@IsPubsAndDocsSectionComplete", SqlDbType.Bit).Value = Modal.IsPubsAndDocsSectionComplete == null ? DBNull.Value : (object)Modal.IsPubsAndDocsSectionComplete;
            command.Parameters.Add("@CarriedOutByTheDOOW", SqlDbType.NVarChar).Value = Modal.CarriedOutByTheDOOW != null ? Modal.CarriedOutByTheDOOW : "";
            command.Parameters.Add("@IsSOPEPPoints", SqlDbType.Bit).Value = Modal.IsSOPEPPoints == null ? DBNull.Value : (object)Modal.IsSOPEPPoints;
            command.Parameters.Add("@SOPEPPoints", SqlDbType.NVarChar).Value = Modal.SOPEPPoints != null ? Modal.SOPEPPoints : "";
            command.Parameters.Add("@IsRegs4shipsDVD", SqlDbType.Bit).Value = Modal.IsRegs4shipsDVD == null ? DBNull.Value : (object)Modal.IsRegs4shipsDVD;
            command.Parameters.Add("@Regs4shipsDVD", SqlDbType.NVarChar).Value = Modal.Regs4shipsDVD != null ? Modal.Regs4shipsDVD : "";

            command.Parameters.Add("@IsBWMP", SqlDbType.Bit).Value = Modal.IsBWMP == null ? DBNull.Value : (object)Modal.IsBWMP;
            command.Parameters.Add("@BWMP", SqlDbType.NVarChar).Value = Modal.BWMP != null ? Modal.BWMP : "";
            command.Parameters.Add("@IsBWMPSupplement", SqlDbType.Bit).Value = Modal.IsBWMPSupplement == null ? DBNull.Value : (object)Modal.IsBWMPSupplement;
            command.Parameters.Add("@BWMPSupplement", SqlDbType.NVarChar).Value = Modal.BWMPSupplement != null ? Modal.BWMPSupplement : "";
            command.Parameters.Add("@IsIntactStabilityManual", SqlDbType.Bit).Value = Modal.IsIntactStabilityManual == null ? DBNull.Value : (object)Modal.IsIntactStabilityManual;
            command.Parameters.Add("@IntactStabilityManual", SqlDbType.NVarChar).Value = Modal.IntactStabilityManual != null ? Modal.IntactStabilityManual : "";
            command.Parameters.Add("@IsStabilityComputer", SqlDbType.Bit).Value = Modal.IsStabilityComputer == null ? DBNull.Value : (object)Modal.IsStabilityComputer;
            command.Parameters.Add("@StabilityComputer", SqlDbType.NVarChar).Value = Modal.StabilityComputer != null ? Modal.StabilityComputer : "";
            command.Parameters.Add("@IsDateOfLast", SqlDbType.Bit).Value = Modal.IsDateOfLast == null ? DBNull.Value : (object)Modal.IsDateOfLast;
            command.Parameters.Add("@DateOfLast", SqlDbType.NVarChar).Value = Modal.DateOfLast != null ? Modal.DateOfLast : "";
            command.Parameters.Add("@IsCargoSecuring", SqlDbType.Bit).Value = Modal.IsCargoSecuring == null ? DBNull.Value : (object)Modal.IsCargoSecuring;
            command.Parameters.Add("@CargoSecuring", SqlDbType.NVarChar).Value = Modal.CargoSecuring != null ? Modal.CargoSecuring : "";
            command.Parameters.Add("@IsBulkCargo", SqlDbType.Bit).Value = Modal.IsBulkCargo == null ? DBNull.Value : (object)Modal.IsBulkCargo;
            command.Parameters.Add("@BulkCargo", SqlDbType.NVarChar).Value = Modal.BulkCargo != null ? Modal.BulkCargo : "";
            command.Parameters.Add("@IsSMSManual", SqlDbType.Bit).Value = Modal.IsSMSManual == null ? DBNull.Value : (object)Modal.IsSMSManual;
            command.Parameters.Add("@SMSManual", SqlDbType.NVarChar).Value = Modal.SMSManual != null ? Modal.SMSManual : "";
            command.Parameters.Add("@IsRegisterOf", SqlDbType.Bit).Value = Modal.IsRegisterOf == null ? DBNull.Value : (object)Modal.IsRegisterOf;
            command.Parameters.Add("@RegisterOf", SqlDbType.NVarChar).Value = Modal.RegisterOf != null ? Modal.RegisterOf : "";
            command.Parameters.Add("@IsFleetStandingOrder", SqlDbType.Bit).Value = Modal.IsFleetStandingOrder == null ? DBNull.Value : (object)Modal.IsFleetStandingOrder;
            command.Parameters.Add("@FleetStandingOrder", SqlDbType.NVarChar).Value = Modal.FleetStandingOrder != null ? Modal.FleetStandingOrder : "";
            command.Parameters.Add("@IsFleetMemoranda", SqlDbType.Bit).Value = Modal.IsFleetMemoranda == null ? DBNull.Value : (object)Modal.IsFleetMemoranda;
            command.Parameters.Add("@FleetMemoranda", SqlDbType.NVarChar).Value = Modal.FleetMemoranda != null ? Modal.FleetMemoranda : "";
            command.Parameters.Add("@IsShipsPlans", SqlDbType.Bit).Value = Modal.IsShipsPlans == null ? DBNull.Value : (object)Modal.IsShipsPlans;
            command.Parameters.Add("@ShipsPlans", SqlDbType.NVarChar).Value = Modal.ShipsPlans != null ? Modal.ShipsPlans : "";
            command.Parameters.Add("@IsCollective", SqlDbType.Bit).Value = Modal.IsCollective == null ? DBNull.Value : (object)Modal.IsCollective;
            command.Parameters.Add("@Collective", SqlDbType.NVarChar).Value = Modal.Collective != null ? Modal.Collective : "";
            command.Parameters.Add("@IsDraftAndFreeboardNotice", SqlDbType.Bit).Value = Modal.IsDraftAndFreeboardNotice == null ? DBNull.Value : (object)Modal.IsDraftAndFreeboardNotice;
            command.Parameters.Add("@DraftAndFreeboardNotice", SqlDbType.NVarChar).Value = Modal.DraftAndFreeboardNotice != null ? Modal.DraftAndFreeboardNotice : "";
            command.Parameters.Add("@IsPCSOPEP", SqlDbType.Bit).Value = Modal.IsPCSOPEP == null ? DBNull.Value : (object)Modal.IsPCSOPEP;
            command.Parameters.Add("@PCSOPEP", SqlDbType.NVarChar).Value = Modal.PCSOPEP != null ? Modal.PCSOPEP : "";
            command.Parameters.Add("@IsNTVRP", SqlDbType.Bit).Value = Modal.IsNTVRP == null ? DBNull.Value : (object)Modal.IsNTVRP;
            command.Parameters.Add("@NTVRP", SqlDbType.NVarChar).Value = Modal.NTVRP != null ? Modal.NTVRP : "";
            command.Parameters.Add("@IsVGP", SqlDbType.Bit).Value = Modal.IsVGP == null ? DBNull.Value : (object)Modal.IsVGP;
            command.Parameters.Add("@VGP", SqlDbType.NVarChar).Value = Modal.VGP != null ? Modal.VGP : "";
            command.Parameters.Add("@PubsComments", SqlDbType.NVarChar).Value = Modal.PubsComments != null ? Modal.PubsComments : "";

            command.Parameters.Add("@OfficialLogbookA", SqlDbType.NVarChar).Value = Modal.OfficialLogbookA != null ? Modal.OfficialLogbookA : "";
            command.Parameters.Add("@OfficialLogbookB", SqlDbType.NVarChar).Value = Modal.OfficialLogbookB != null ? Modal.OfficialLogbookB : "";
            command.Parameters.Add("@OfficialLogbookC", SqlDbType.NVarChar).Value = Modal.OfficialLogbookC != null ? Modal.OfficialLogbookC : "";
            command.Parameters.Add("@OfficialLogbookD", SqlDbType.NVarChar).Value = Modal.OfficialLogbookD != null ? Modal.OfficialLogbookD : "";
            command.Parameters.Add("@OfficialLogbookE", SqlDbType.NVarChar).Value = Modal.OfficialLogbookE != null ? Modal.OfficialLogbookE : "";
            command.Parameters.Add("@DeckLogbook", SqlDbType.NVarChar).Value = Modal.DeckLogbook != null ? Modal.DeckLogbook : "";
            command.Parameters.Add("@Listofcrew", SqlDbType.NVarChar).Value = Modal.Listofcrew != null ? Modal.Listofcrew : "";
            command.Parameters.Add("@LastHose", SqlDbType.NVarChar).Value = Modal.LastHose != null ? Modal.LastHose : "";
            command.Parameters.Add("@PassagePlanning", SqlDbType.NVarChar).Value = Modal.PassagePlanning != null ? Modal.PassagePlanning : "";
            command.Parameters.Add("@LoadingComputer", SqlDbType.NVarChar).Value = Modal.LoadingComputer != null ? Modal.LoadingComputer : "";
            command.Parameters.Add("@EngineLogbook", SqlDbType.NVarChar).Value = Modal.EngineLogbook != null ? Modal.EngineLogbook : "";
            command.Parameters.Add("@OilRecordBook", SqlDbType.NVarChar).Value = Modal.OilRecordBook != null ? Modal.OilRecordBook : "";
            command.Parameters.Add("@RiskAssessments", SqlDbType.NVarChar).Value = Modal.RiskAssessments != null ? Modal.RiskAssessments : "";
            command.Parameters.Add("@GMDSSLogbook", SqlDbType.NVarChar).Value = Modal.GMDSSLogbook != null ? Modal.GMDSSLogbook : "";
            command.Parameters.Add("@DeckLogbook5D", SqlDbType.NVarChar).Value = Modal.DeckLogbook5D != null ? Modal.DeckLogbook5D : "";
            command.Parameters.Add("@GarbageRecordBook", SqlDbType.NVarChar).Value = Modal.GarbageRecordBook != null ? Modal.GarbageRecordBook : "";
            command.Parameters.Add("@BallastWaterRecordBook", SqlDbType.NVarChar).Value = Modal.BallastWaterRecordBook != null ? Modal.BallastWaterRecordBook : "";
            command.Parameters.Add("@CargoRecordBook", SqlDbType.NVarChar).Value = Modal.CargoRecordBook != null ? Modal.CargoRecordBook : "";
            command.Parameters.Add("@EmissionsControlManual", SqlDbType.NVarChar).Value = Modal.EmissionsControlManual != null ? Modal.EmissionsControlManual : "";
            command.Parameters.Add("@LGR", SqlDbType.NVarChar).Value = Modal.LGR != null ? Modal.LGR : "";
            command.Parameters.Add("@PEER", SqlDbType.NVarChar).Value = Modal.PEER != null ? Modal.PEER : "";
            command.Parameters.Add("@RecordKeepingComments", SqlDbType.NVarChar).Value = Modal.RecordKeepingComments != null ? Modal.RecordKeepingComments : "";

            command.Parameters.Add("@LastPortStateControl", SqlDbType.NVarChar).Value = Modal.LastPortStateControl != null ? Modal.LastPortStateControl : "";
            command.Parameters.Add("@LiferaftsComment", SqlDbType.NVarChar).Value = Modal.LiferaftsComment != null ? Modal.LiferaftsComment : "";
            command.Parameters.Add("@releasesComment", SqlDbType.NVarChar).Value = Modal.releasesComment != null ? Modal.releasesComment : "";
            command.Parameters.Add("@LifeboatComment", SqlDbType.NVarChar).Value = Modal.LifeboatComment != null ? Modal.LifeboatComment : "";
            command.Parameters.Add("@LifeboatdavitComment", SqlDbType.NVarChar).Value = Modal.LifeboatdavitComment != null ? Modal.LifeboatdavitComment : "";
            command.Parameters.Add("@LifeboatequipmentComment", SqlDbType.NVarChar).Value = Modal.LifeboatequipmentComment != null ? Modal.LifeboatequipmentComment : "";
            command.Parameters.Add("@RescueboatComment", SqlDbType.NVarChar).Value = Modal.RescueboatComment != null ? Modal.RescueboatComment : "";
            command.Parameters.Add("@RescueboatequipmentComment", SqlDbType.NVarChar).Value = Modal.RescueboatequipmentComment != null ? Modal.RescueboatequipmentComment : "";
            command.Parameters.Add("@RescueboatoutboardmotorComment", SqlDbType.NVarChar).Value = Modal.RescueboatoutboardmotorComment != null ? Modal.RescueboatoutboardmotorComment : "";
            command.Parameters.Add("@RescueboatdavitComment", SqlDbType.NVarChar).Value = Modal.RescueboatdavitComment != null ? Modal.RescueboatdavitComment : "";
            command.Parameters.Add("@DeckComment", SqlDbType.NVarChar).Value = Modal.DeckComment != null ? Modal.DeckComment : "";
            command.Parameters.Add("@PyrotechnicsComment", SqlDbType.NVarChar).Value = Modal.PyrotechnicsComment != null ? Modal.PyrotechnicsComment : "";
            command.Parameters.Add("@EPIRBComment", SqlDbType.NVarChar).Value = Modal.EPIRBComment != null ? Modal.EPIRBComment : "";
            command.Parameters.Add("@SARTsComment", SqlDbType.NVarChar).Value = Modal.SARTsComment != null ? Modal.SARTsComment : "";
            command.Parameters.Add("@GMDSSComment", SqlDbType.NVarChar).Value = Modal.GMDSSComment != null ? Modal.GMDSSComment : "";
            command.Parameters.Add("@ManoverboardComment", SqlDbType.NVarChar).Value = Modal.ManoverboardComment != null ? Modal.ManoverboardComment : "";
            command.Parameters.Add("@LinethrowingapparatusComment", SqlDbType.NVarChar).Value = Modal.LinethrowingapparatusComment != null ? Modal.LinethrowingapparatusComment : "";
            command.Parameters.Add("@FireextinguishersComment", SqlDbType.NVarChar).Value = Modal.FireextinguishersComment != null ? Modal.FireextinguishersComment : "";
            command.Parameters.Add("@EmergencygeneratorComment", SqlDbType.NVarChar).Value = Modal.EmergencygeneratorComment != null ? Modal.EmergencygeneratorComment : "";
            command.Parameters.Add("@CO2roomComment", SqlDbType.NVarChar).Value = Modal.CO2roomComment != null ? Modal.CO2roomComment : "";
            command.Parameters.Add("@SurvivalComment", SqlDbType.NVarChar).Value = Modal.SurvivalComment != null ? Modal.SurvivalComment : "";
            command.Parameters.Add("@LifejacketComment", SqlDbType.NVarChar).Value = Modal.LifejacketComment != null ? Modal.LifejacketComment : "";
            command.Parameters.Add("@FiremansComment", SqlDbType.NVarChar).Value = Modal.FiremansComment != null ? Modal.FiremansComment : "";
            command.Parameters.Add("@LifebuoysComment", SqlDbType.NVarChar).Value = Modal.LifebuoysComment != null ? Modal.LifebuoysComment : "";
            command.Parameters.Add("@FireboxesComment", SqlDbType.NVarChar).Value = Modal.FireboxesComment != null ? Modal.FireboxesComment : "";
            command.Parameters.Add("@EmergencybellsComment", SqlDbType.NVarChar).Value = Modal.EmergencybellsComment != null ? Modal.EmergencybellsComment : "";
            command.Parameters.Add("@EmergencylightingComment", SqlDbType.NVarChar).Value = Modal.EmergencylightingComment != null ? Modal.EmergencylightingComment : "";
            command.Parameters.Add("@FireplanComment", SqlDbType.NVarChar).Value = Modal.FireplanComment != null ? Modal.FireplanComment : "";
            command.Parameters.Add("@DamageComment", SqlDbType.NVarChar).Value = Modal.DamageComment != null ? Modal.DamageComment : "";
            command.Parameters.Add("@EmergencyplansComment", SqlDbType.NVarChar).Value = Modal.EmergencyplansComment != null ? Modal.EmergencyplansComment : "";
            command.Parameters.Add("@MusterlistComment", SqlDbType.NVarChar).Value = Modal.MusterlistComment != null ? Modal.MusterlistComment : "";
            command.Parameters.Add("@SafetysignsComment", SqlDbType.NVarChar).Value = Modal.SafetysignsComment != null ? Modal.SafetysignsComment : "";
            command.Parameters.Add("@EmergencysteeringComment", SqlDbType.NVarChar).Value = Modal.EmergencysteeringComment != null ? Modal.EmergencysteeringComment : "";
            command.Parameters.Add("@StatutoryemergencydrillsComment", SqlDbType.NVarChar).Value = Modal.StatutoryemergencydrillsComment != null ? Modal.StatutoryemergencydrillsComment : "";
            command.Parameters.Add("@EEBDComment", SqlDbType.NVarChar).Value = Modal.EEBDComment != null ? Modal.EEBDComment : "";
            command.Parameters.Add("@OxygenComment", SqlDbType.NVarChar).Value = Modal.OxygenComment != null ? Modal.OxygenComment : "";
            command.Parameters.Add("@MultigasdetectorComment", SqlDbType.NVarChar).Value = Modal.MultigasdetectorComment != null ? Modal.MultigasdetectorComment : "";
            command.Parameters.Add("@GasdetectorComment", SqlDbType.NVarChar).Value = Modal.GasdetectorComment != null ? Modal.GasdetectorComment : "";
            command.Parameters.Add("@SufficientquantityComment", SqlDbType.NVarChar).Value = Modal.SufficientquantityComment != null ? Modal.SufficientquantityComment : "";
            command.Parameters.Add("@BASetsComment", SqlDbType.NVarChar).Value = Modal.BASetsComment != null ? Modal.BASetsComment : "";
            command.Parameters.Add("@SafetyComment", SqlDbType.NVarChar).Value = Modal.SafetyComment != null ? Modal.SafetyComment : "";

            command.Parameters.Add("@GangwayComment", SqlDbType.NVarChar).Value = Modal.GangwayComment != null ? Modal.GangwayComment : "";
            command.Parameters.Add("@RestrictedComment", SqlDbType.NVarChar).Value = Modal.RestrictedComment != null ? Modal.RestrictedComment : "";
            command.Parameters.Add("@OutsideComment", SqlDbType.NVarChar).Value = Modal.OutsideComment != null ? Modal.OutsideComment : "";
            command.Parameters.Add("@EntrancedoorsComment", SqlDbType.NVarChar).Value = Modal.EntrancedoorsComment != null ? Modal.EntrancedoorsComment : "";
            command.Parameters.Add("@AccommodationComment", SqlDbType.NVarChar).Value = Modal.AccommodationComment != null ? Modal.AccommodationComment : "";
            command.Parameters.Add("@GMDSSComment5G", SqlDbType.NVarChar).Value = Modal.GMDSSComment5G != null ? Modal.GMDSSComment5G : "";
            command.Parameters.Add("@VariousComment", SqlDbType.NVarChar).Value = Modal.VariousComment != null ? Modal.VariousComment : "";
            command.Parameters.Add("@SSOComment", SqlDbType.NVarChar).Value = Modal.SSOComment != null ? Modal.SSOComment : "";
            command.Parameters.Add("@SecuritylogbookComment", SqlDbType.NVarChar).Value = Modal.SecuritylogbookComment != null ? Modal.SecuritylogbookComment : "";
            command.Parameters.Add("@Listoflast10portsComment", SqlDbType.NVarChar).Value = Modal.Listoflast10portsComment != null ? Modal.Listoflast10portsComment : "";
            command.Parameters.Add("@PFSOComment", SqlDbType.NVarChar).Value = Modal.PFSOComment != null ? Modal.PFSOComment : "";
            command.Parameters.Add("@SecuritylevelComment", SqlDbType.NVarChar).Value = Modal.SecuritylevelComment != null ? Modal.SecuritylevelComment : "";
            command.Parameters.Add("@DrillsandtrainingComment", SqlDbType.NVarChar).Value = Modal.DrillsandtrainingComment != null ? Modal.DrillsandtrainingComment : "";
            command.Parameters.Add("@DOSComment", SqlDbType.NVarChar).Value = Modal.DOSComment != null ? Modal.DOSComment : "";
            command.Parameters.Add("@SSASComment", SqlDbType.NVarChar).Value = Modal.SSASComment != null ? Modal.SSASComment : "";
            command.Parameters.Add("@VisitorslogbookComment", SqlDbType.NVarChar).Value = Modal.VisitorslogbookComment != null ? Modal.VisitorslogbookComment : "";
            command.Parameters.Add("@KeyregisterComment", SqlDbType.NVarChar).Value = Modal.KeyregisterComment != null ? Modal.KeyregisterComment : "";
            command.Parameters.Add("@ShipSecurityComment", SqlDbType.NVarChar).Value = Modal.ShipSecurityComment != null ? Modal.ShipSecurityComment : "";
            command.Parameters.Add("@SecurityComment", SqlDbType.NVarChar).Value = Modal.SecurityComment != null ? Modal.SecurityComment : "";

            command.Parameters.Add("@NauticalchartsComment", SqlDbType.NVarChar).Value = Modal.NauticalchartsComment != null ? Modal.NauticalchartsComment : "";
            command.Parameters.Add("@NoticetomarinersComment", SqlDbType.NVarChar).Value = Modal.NoticetomarinersComment != null ? Modal.NoticetomarinersComment : "";
            command.Parameters.Add("@ListofradiosignalsComment", SqlDbType.NVarChar).Value = Modal.ListofradiosignalsComment != null ? Modal.ListofradiosignalsComment : "";
            command.Parameters.Add("@ListoflightsComment", SqlDbType.NVarChar).Value = Modal.ListoflightsComment != null ? Modal.ListoflightsComment : "";
            command.Parameters.Add("@SailingdirectionsComment", SqlDbType.NVarChar).Value = Modal.SailingdirectionsComment != null ? Modal.SailingdirectionsComment : "";
            command.Parameters.Add("@TidetablesComment", SqlDbType.NVarChar).Value = Modal.TidetablesComment != null ? Modal.TidetablesComment : "";
            command.Parameters.Add("@NavtexandprinterComment", SqlDbType.NVarChar).Value = Modal.NavtexandprinterComment != null ? Modal.NavtexandprinterComment : "";
            command.Parameters.Add("@RadarsComment", SqlDbType.NVarChar).Value = Modal.RadarsComment != null ? Modal.RadarsComment : "";
            command.Parameters.Add("@GPSComment", SqlDbType.NVarChar).Value = Modal.GPSComment != null ? Modal.GPSComment : "";
            command.Parameters.Add("@AISComment", SqlDbType.NVarChar).Value = Modal.AISComment != null ? Modal.AISComment : "";
            command.Parameters.Add("@VDRComment", SqlDbType.NVarChar).Value = Modal.VDRComment != null ? Modal.VDRComment : "";
            command.Parameters.Add("@ECDISComment", SqlDbType.NVarChar).Value = Modal.ECDISComment != null ? Modal.ECDISComment : "";
            command.Parameters.Add("@EchosounderComment", SqlDbType.NVarChar).Value = Modal.EchosounderComment != null ? Modal.EchosounderComment : "";
            command.Parameters.Add("@ADPbackuplaptopComment", SqlDbType.NVarChar).Value = Modal.ADPbackuplaptopComment != null ? Modal.ADPbackuplaptopComment : "";
            command.Parameters.Add("@ColourprinterComment", SqlDbType.NVarChar).Value = Modal.ColourprinterComment != null ? Modal.ColourprinterComment : "";
            command.Parameters.Add("@VHFDSCtransceiverComment", SqlDbType.NVarChar).Value = Modal.VHFDSCtransceiverComment != null ? Modal.VHFDSCtransceiverComment : "";
            command.Parameters.Add("@radioinstallationComment", SqlDbType.NVarChar).Value = Modal.radioinstallationComment != null ? Modal.radioinstallationComment : "";
            command.Parameters.Add("@InmarsatCComment", SqlDbType.NVarChar).Value = Modal.InmarsatCComment != null ? Modal.InmarsatCComment : "";
            command.Parameters.Add("@MagneticcompassComment", SqlDbType.NVarChar).Value = Modal.MagneticcompassComment != null ? Modal.MagneticcompassComment : "";
            command.Parameters.Add("@SparecompassbowlComment", SqlDbType.NVarChar).Value = Modal.SparecompassbowlComment != null ? Modal.SparecompassbowlComment : "";
            command.Parameters.Add("@CompassobservationbookComment", SqlDbType.NVarChar).Value = Modal.CompassobservationbookComment != null ? Modal.CompassobservationbookComment : "";
            command.Parameters.Add("@GyrocompassComment", SqlDbType.NVarChar).Value = Modal.GyrocompassComment != null ? Modal.GyrocompassComment : "";
            command.Parameters.Add("@RudderindicatorComment", SqlDbType.NVarChar).Value = Modal.RudderindicatorComment != null ? Modal.RudderindicatorComment : "";
            command.Parameters.Add("@SpeedlogComment", SqlDbType.NVarChar).Value = Modal.SpeedlogComment != null ? Modal.SpeedlogComment : "";
            command.Parameters.Add("@NavigationComment", SqlDbType.NVarChar).Value = Modal.NavigationComment != null ? Modal.NavigationComment : "";
            command.Parameters.Add("@SignalflagsComment", SqlDbType.NVarChar).Value = Modal.SignalflagsComment != null ? Modal.SignalflagsComment : "";
            command.Parameters.Add("@RPMComment", SqlDbType.NVarChar).Value = Modal.RPMComment != null ? Modal.RPMComment : "";
            command.Parameters.Add("@BasicmanoeuvringdataComment", SqlDbType.NVarChar).Value = Modal.BasicmanoeuvringdataComment != null ? Modal.BasicmanoeuvringdataComment : "";
            command.Parameters.Add("@MasterstandingordersComment", SqlDbType.NVarChar).Value = Modal.MasterstandingordersComment != null ? Modal.MasterstandingordersComment : "";
            command.Parameters.Add("@MasternightordersbookComment", SqlDbType.NVarChar).Value = Modal.MasternightordersbookComment != null ? Modal.MasternightordersbookComment : "";
            command.Parameters.Add("@SextantComment", SqlDbType.NVarChar).Value = Modal.SextantComment != null ? Modal.SextantComment : "";
            command.Parameters.Add("@AzimuthmirrorComment", SqlDbType.NVarChar).Value = Modal.AzimuthmirrorComment != null ? Modal.AzimuthmirrorComment : "";
            command.Parameters.Add("@BridgepostersComment", SqlDbType.NVarChar).Value = Modal.BridgepostersComment != null ? Modal.BridgepostersComment : "";
            command.Parameters.Add("@ReviewofplannedComment", SqlDbType.NVarChar).Value = Modal.ReviewofplannedComment != null ? Modal.ReviewofplannedComment : "";
            command.Parameters.Add("@BridgebellbookComment", SqlDbType.NVarChar).Value = Modal.BridgebellbookComment != null ? Modal.BridgebellbookComment : "";
            command.Parameters.Add("@BridgenavigationalComment", SqlDbType.NVarChar).Value = Modal.BridgenavigationalComment != null ? Modal.BridgenavigationalComment : "";
            command.Parameters.Add("@SecurityEquipmentComment", SqlDbType.NVarChar).Value = Modal.SecurityEquipmentComment != null ? Modal.SecurityEquipmentComment : "";
            command.Parameters.Add("@NavigationPost", SqlDbType.NVarChar).Value = Modal.NavigationPost != null ? Modal.NavigationPost : "";

            command.Parameters.Add("@GeneralComment", SqlDbType.NVarChar).Value = Modal.GeneralComment != null ? Modal.GeneralComment : "";
            command.Parameters.Add("@MedicinestorageComment", SqlDbType.NVarChar).Value = Modal.MedicinestorageComment != null ? Modal.MedicinestorageComment : "";
            command.Parameters.Add("@MedicinechestcertificateComment", SqlDbType.NVarChar).Value = Modal.MedicinechestcertificateComment != null ? Modal.MedicinechestcertificateComment : "";
            command.Parameters.Add("@InventoryStoresComment", SqlDbType.NVarChar).Value = Modal.InventoryStoresComment != null ? Modal.InventoryStoresComment : "";
            command.Parameters.Add("@OxygencylindersComment", SqlDbType.NVarChar).Value = Modal.OxygencylindersComment != null ? Modal.OxygencylindersComment : "";
            command.Parameters.Add("@StretcherComment", SqlDbType.NVarChar).Value = Modal.StretcherComment != null ? Modal.StretcherComment : "";
            command.Parameters.Add("@SalivaComment", SqlDbType.NVarChar).Value = Modal.SalivaComment != null ? Modal.SalivaComment : "";
            command.Parameters.Add("@AlcoholComment", SqlDbType.NVarChar).Value = Modal.AlcoholComment != null ? Modal.AlcoholComment : "";
            command.Parameters.Add("@HospitalComment", SqlDbType.NVarChar).Value = Modal.HospitalComment != null ? Modal.HospitalComment : "";


            command.Parameters.Add("@GeneralGalleyComment", SqlDbType.NVarChar).Value = Modal.GeneralGalleyComment != null ? Modal.GeneralGalleyComment : "";
            command.Parameters.Add("@HygieneComment", SqlDbType.NVarChar).Value = Modal.HygieneComment != null ? Modal.HygieneComment : "";
            command.Parameters.Add("@FoodstorageComment", SqlDbType.NVarChar).Value = Modal.FoodstorageComment != null ? Modal.FoodstorageComment : "";
            command.Parameters.Add("@FoodlabellingComment", SqlDbType.NVarChar).Value = Modal.FoodlabellingComment != null ? Modal.FoodlabellingComment : "";
            command.Parameters.Add("@GalleyriskassessmentComment", SqlDbType.NVarChar).Value = Modal.GalleyriskassessmentComment != null ? Modal.GalleyriskassessmentComment : "";
            command.Parameters.Add("@FridgetemperatureComment", SqlDbType.NVarChar).Value = Modal.FridgetemperatureComment != null ? Modal.FridgetemperatureComment : "";
            command.Parameters.Add("@FoodandProvisionsComment", SqlDbType.NVarChar).Value = Modal.FoodandProvisionsComment != null ? Modal.FoodandProvisionsComment : "";
            command.Parameters.Add("@GalleyComment", SqlDbType.NVarChar).Value = Modal.GalleyComment != null ? Modal.GalleyComment : "";


            command.Parameters.Add("@ConditionComment", SqlDbType.NVarChar).Value = Modal.ConditionComment != null ? Modal.ConditionComment : "";
            command.Parameters.Add("@PaintworkComment", SqlDbType.NVarChar).Value = Modal.PaintworkComment != null ? Modal.PaintworkComment : "";
            command.Parameters.Add("@LightingComment", SqlDbType.NVarChar).Value = Modal.LightingComment != null ? Modal.LightingComment : "";
            command.Parameters.Add("@PlatesComment", SqlDbType.NVarChar).Value = Modal.PlatesComment != null ? Modal.PlatesComment : "";
            command.Parameters.Add("@BilgesComment", SqlDbType.NVarChar).Value = Modal.BilgesComment != null ? Modal.BilgesComment : "";
            command.Parameters.Add("@PipelinesandvalvesComment", SqlDbType.NVarChar).Value = Modal.PipelinesandvalvesComment != null ? Modal.PipelinesandvalvesComment : "";
            command.Parameters.Add("@LeakageComment", SqlDbType.NVarChar).Value = Modal.LeakageComment != null ? Modal.LeakageComment : "";
            command.Parameters.Add("@EquipmentComment", SqlDbType.NVarChar).Value = Modal.EquipmentComment != null ? Modal.EquipmentComment : "";
            command.Parameters.Add("@OilywaterseparatorComment", SqlDbType.NVarChar).Value = Modal.OilywaterseparatorComment != null ? Modal.OilywaterseparatorComment : "";
            command.Parameters.Add("@FueloiltransferplanComment", SqlDbType.NVarChar).Value = Modal.FueloiltransferplanComment != null ? Modal.FueloiltransferplanComment : "";
            command.Parameters.Add("@SteeringgearComment", SqlDbType.NVarChar).Value = Modal.SteeringgearComment != null ? Modal.SteeringgearComment : "";
            command.Parameters.Add("@WorkshopandequipmentComment", SqlDbType.NVarChar).Value = Modal.WorkshopandequipmentComment != null ? Modal.WorkshopandequipmentComment : "";
            command.Parameters.Add("@SoundingpipesComment", SqlDbType.NVarChar).Value = Modal.SoundingpipesComment != null ? Modal.SoundingpipesComment : "";
            command.Parameters.Add("@EnginecontrolComment", SqlDbType.NVarChar).Value = Modal.EnginecontrolComment != null ? Modal.EnginecontrolComment : "";
            command.Parameters.Add("@ChiefEngineernightordersbookComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineernightordersbookComment != null ? Modal.ChiefEngineernightordersbookComment : "";
            command.Parameters.Add("@ChiefEngineerstandingordersComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineerstandingordersComment != null ? Modal.ChiefEngineerstandingordersComment : "";
            command.Parameters.Add("@PreUMSComment", SqlDbType.NVarChar).Value = Modal.PreUMSComment != null ? Modal.PreUMSComment : "";
            command.Parameters.Add("@EnginebellbookComment", SqlDbType.NVarChar).Value = Modal.EnginebellbookComment != null ? Modal.EnginebellbookComment : "";
            command.Parameters.Add("@LockoutComment", SqlDbType.NVarChar).Value = Modal.LockoutComment != null ? Modal.LockoutComment : "";
            command.Parameters.Add("@EngineRoomComment", SqlDbType.NVarChar).Value = Modal.EngineRoomComment != null ? Modal.EngineRoomComment : "";


            command.Parameters.Add("@CleanlinessandhygieneComment", SqlDbType.NVarChar).Value = Modal.CleanlinessandhygieneComment != null ? Modal.CleanlinessandhygieneComment : "";
            command.Parameters.Add("@ConditionComment5M", SqlDbType.NVarChar).Value = Modal.ConditionComment5M != null ? Modal.ConditionComment5M : "";
            command.Parameters.Add("@PaintworkComment5M", SqlDbType.NVarChar).Value = Modal.PaintworkComment5M != null ? Modal.PaintworkComment5M : "";
            command.Parameters.Add("@SignalmastandstaysComment", SqlDbType.NVarChar).Value = Modal.SignalmastandstaysComment != null ? Modal.SignalmastandstaysComment : "";
            command.Parameters.Add("@MonkeyislandComment", SqlDbType.NVarChar).Value = Modal.MonkeyislandComment != null ? Modal.MonkeyislandComment : "";
            command.Parameters.Add("@FireDampersComment", SqlDbType.NVarChar).Value = Modal.FireDampersComment != null ? Modal.FireDampersComment : "";
            command.Parameters.Add("@RailsBulwarksComment", SqlDbType.NVarChar).Value = Modal.RailsBulwarksComment != null ? Modal.RailsBulwarksComment : "";
            command.Parameters.Add("@WatertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WatertightdoorsComment != null ? Modal.WatertightdoorsComment : "";
            command.Parameters.Add("@VentilatorsComment", SqlDbType.NVarChar).Value = Modal.VentilatorsComment != null ? Modal.VentilatorsComment : "";
            command.Parameters.Add("@WinchesComment", SqlDbType.NVarChar).Value = Modal.WinchesComment != null ? Modal.WinchesComment : "";
            command.Parameters.Add("@FairleadsComment", SqlDbType.NVarChar).Value = Modal.FairleadsComment != null ? Modal.FairleadsComment : "";
            command.Parameters.Add("@MooringLinesComment", SqlDbType.NVarChar).Value = Modal.MooringLinesComment != null ? Modal.MooringLinesComment : "";
            command.Parameters.Add("@EmergencyShutOffsComment", SqlDbType.NVarChar).Value = Modal.EmergencyShutOffsComment != null ? Modal.EmergencyShutOffsComment : "";
            command.Parameters.Add("@RadioaerialsComment", SqlDbType.NVarChar).Value = Modal.RadioaerialsComment != null ? Modal.RadioaerialsComment : "";
            command.Parameters.Add("@SOPEPlockerComment", SqlDbType.NVarChar).Value = Modal.SOPEPlockerComment != null ? Modal.SOPEPlockerComment : "";
            command.Parameters.Add("@ChemicallockerComment", SqlDbType.NVarChar).Value = Modal.ChemicallockerComment != null ? Modal.ChemicallockerComment : "";
            command.Parameters.Add("@AntislippaintComment", SqlDbType.NVarChar).Value = Modal.AntislippaintComment != null ? Modal.AntislippaintComment : "";
            command.Parameters.Add("@SuperstructureComment", SqlDbType.NVarChar).Value = Modal.SuperstructureComment != null ? Modal.SuperstructureComment : "";


            command.Parameters.Add("@CabinsComment", SqlDbType.NVarChar).Value = Modal.CabinsComment != null ? Modal.CabinsComment : "";
            command.Parameters.Add("@OfficesComment", SqlDbType.NVarChar).Value = Modal.OfficesComment != null ? Modal.OfficesComment : "";
            command.Parameters.Add("@MessroomsComment", SqlDbType.NVarChar).Value = Modal.MessroomsComment != null ? Modal.MessroomsComment : "";
            command.Parameters.Add("@ToiletsComment", SqlDbType.NVarChar).Value = Modal.ToiletsComment != null ? Modal.ToiletsComment : "";
            command.Parameters.Add("@LaundryroomComment", SqlDbType.NVarChar).Value = Modal.LaundryroomComment != null ? Modal.LaundryroomComment : "";
            command.Parameters.Add("@ChangingroomComment", SqlDbType.NVarChar).Value = Modal.ChangingroomComment != null ? Modal.ChangingroomComment : "";
            command.Parameters.Add("@OtherComment", SqlDbType.NVarChar).Value = Modal.OtherComment != null ? Modal.OtherComment : "";
            command.Parameters.Add("@ConditionComment5N", SqlDbType.NVarChar).Value = Modal.ConditionComment5N != null ? Modal.ConditionComment5N : "";
            command.Parameters.Add("@SelfclosingfiredoorsComment", SqlDbType.NVarChar).Value = Modal.SelfclosingfiredoorsComment != null ? Modal.SelfclosingfiredoorsComment : "";
            command.Parameters.Add("@StairwellsComment", SqlDbType.NVarChar).Value = Modal.StairwellsComment != null ? Modal.StairwellsComment : "";
            command.Parameters.Add("@SuperstructureInternalComment", SqlDbType.NVarChar).Value = Modal.SuperstructureInternalComment != null ? Modal.SuperstructureInternalComment : "";

            command.Parameters.Add("@PortablegangwayComment", SqlDbType.NVarChar).Value = Modal.PortablegangwayComment != null ? Modal.PortablegangwayComment : "";
            command.Parameters.Add("@SafetynetComment", SqlDbType.NVarChar).Value = Modal.SafetynetComment != null ? Modal.SafetynetComment : "";
            command.Parameters.Add("@AccommodationLadderComment", SqlDbType.NVarChar).Value = Modal.AccommodationLadderComment != null ? Modal.AccommodationLadderComment : "";
            command.Parameters.Add("@SafeaccessprovidedComment", SqlDbType.NVarChar).Value = Modal.SafeaccessprovidedComment != null ? Modal.SafeaccessprovidedComment : "";
            command.Parameters.Add("@PilotladdersComment", SqlDbType.NVarChar).Value = Modal.PilotladdersComment != null ? Modal.PilotladdersComment : "";
            command.Parameters.Add("@BoardingEquipmentComment", SqlDbType.NVarChar).Value = Modal.BoardingEquipmentComment != null ? Modal.BoardingEquipmentComment : "";
            command.Parameters.Add("@CleanlinessComment", SqlDbType.NVarChar).Value = Modal.CleanlinessComment != null ? Modal.CleanlinessComment : "";
            command.Parameters.Add("@PaintworkComment5P", SqlDbType.NVarChar).Value = Modal.PaintworkComment5P != null ? Modal.PaintworkComment5P : "";
            command.Parameters.Add("@ShipsiderailsComment", SqlDbType.NVarChar).Value = Modal.ShipsiderailsComment != null ? Modal.ShipsiderailsComment : "";
            command.Parameters.Add("@WeathertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WeathertightdoorsComment != null ? Modal.WeathertightdoorsComment : "";
            command.Parameters.Add("@FirehydrantsComment", SqlDbType.NVarChar).Value = Modal.FirehydrantsComment != null ? Modal.FirehydrantsComment : "";
            command.Parameters.Add("@VentilatorsComment5P", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5P != null ? Modal.VentilatorsComment5P : "";
            command.Parameters.Add("@ManholecoversComment", SqlDbType.NVarChar).Value = Modal.ManholecoversComment != null ? Modal.ManholecoversComment : "";
            command.Parameters.Add("@MainDeckAreaComment", SqlDbType.NVarChar).Value = Modal.MainDeckAreaComment != null ? Modal.MainDeckAreaComment : "";


            command.Parameters.Add("@ConditionComment5Q", SqlDbType.NVarChar).Value = Modal.ConditionComment5Q != null ? Modal.ConditionComment5Q : "";
            command.Parameters.Add("@PaintworkComment5Q", SqlDbType.NVarChar).Value = Modal.PaintworkComment5Q != null ? Modal.PaintworkComment5Q : "";
            command.Parameters.Add("@MechanicaldamageComment", SqlDbType.NVarChar).Value = Modal.MechanicaldamageComment != null ? Modal.MechanicaldamageComment : "";
            command.Parameters.Add("@AccessladdersComment", SqlDbType.NVarChar).Value = Modal.AccessladdersComment != null ? Modal.AccessladdersComment : "";
            command.Parameters.Add("@ManholecoversComment5Q", SqlDbType.NVarChar).Value = Modal.ManholecoversComment5Q != null ? Modal.ManholecoversComment5Q : "";
            command.Parameters.Add("@HoldbilgeComment", SqlDbType.NVarChar).Value = Modal.HoldbilgeComment != null ? Modal.HoldbilgeComment : "";
            command.Parameters.Add("@AccessdoorsComment", SqlDbType.NVarChar).Value = Modal.AccessdoorsComment != null ? Modal.AccessdoorsComment : "";
            command.Parameters.Add("@ConditionHatchCoversComment", SqlDbType.NVarChar).Value = Modal.ConditionHatchCoversComment != null ? Modal.ConditionHatchCoversComment : "";
            command.Parameters.Add("@PaintworkHatchCoversComment", SqlDbType.NVarChar).Value = Modal.PaintworkHatchCoversComment != null ? Modal.PaintworkHatchCoversComment : "";
            command.Parameters.Add("@RubbersealsComment", SqlDbType.NVarChar).Value = Modal.RubbersealsComment != null ? Modal.RubbersealsComment : "";
            command.Parameters.Add("@SignsofhatchesComment", SqlDbType.NVarChar).Value = Modal.SignsofhatchesComment != null ? Modal.SignsofhatchesComment : "";
            command.Parameters.Add("@SealingtapeComment", SqlDbType.NVarChar).Value = Modal.SealingtapeComment != null ? Modal.SealingtapeComment : "";
            command.Parameters.Add("@ConditionofhydraulicsComment", SqlDbType.NVarChar).Value = Modal.ConditionofhydraulicsComment != null ? Modal.ConditionofhydraulicsComment : "";
            command.Parameters.Add("@PortablebulkheadsComment", SqlDbType.NVarChar).Value = Modal.PortablebulkheadsComment != null ? Modal.PortablebulkheadsComment : "";
            command.Parameters.Add("@TweendecksComment", SqlDbType.NVarChar).Value = Modal.TweendecksComment != null ? Modal.TweendecksComment : "";
            command.Parameters.Add("@HatchcoamingComment", SqlDbType.NVarChar).Value = Modal.HatchcoamingComment != null ? Modal.HatchcoamingComment : "";
            command.Parameters.Add("@ConditionCargoCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionCargoCranesComment != null ? Modal.ConditionCargoCranesComment : "";
            command.Parameters.Add("@GantrycranealarmComment", SqlDbType.NVarChar).Value = Modal.GantrycranealarmComment != null ? Modal.GantrycranealarmComment : "";
            command.Parameters.Add("@GantryconditionComment", SqlDbType.NVarChar).Value = Modal.GantryconditionComment != null ? Modal.GantryconditionComment : "";
            command.Parameters.Add("@CargoHoldsComment", SqlDbType.NVarChar).Value = Modal.CargoHoldsComment != null ? Modal.CargoHoldsComment : "";


            command.Parameters.Add("@CleanlinessComment5R", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5R != null ? Modal.CleanlinessComment5R : "";
            command.Parameters.Add("@PaintworkComment5R", SqlDbType.NVarChar).Value = Modal.PaintworkComment5R != null ? Modal.PaintworkComment5R : "";
            command.Parameters.Add("@TriphazardsComment", SqlDbType.NVarChar).Value = Modal.TriphazardsComment != null ? Modal.TriphazardsComment : "";
            command.Parameters.Add("@WindlassComment", SqlDbType.NVarChar).Value = Modal.WindlassComment != null ? Modal.WindlassComment : "";
            command.Parameters.Add("@CablesComment", SqlDbType.NVarChar).Value = Modal.CablesComment != null ? Modal.CablesComment : "";
            command.Parameters.Add("@WinchesComment5R", SqlDbType.NVarChar).Value = Modal.WinchesComment5R != null ? Modal.WinchesComment5R : "";
            command.Parameters.Add("@FairleadsComment5R", SqlDbType.NVarChar).Value = Modal.FairleadsComment5R != null ? Modal.FairleadsComment5R : "";
            command.Parameters.Add("@MooringComment", SqlDbType.NVarChar).Value = Modal.MooringComment != null ? Modal.MooringComment : "";
            command.Parameters.Add("@HatchToforecastlespaceComment", SqlDbType.NVarChar).Value = Modal.HatchToforecastlespaceComment != null ? Modal.HatchToforecastlespaceComment : "";
            command.Parameters.Add("@VentilatorsComment5R", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5R != null ? Modal.VentilatorsComment5R : "";
            command.Parameters.Add("@BellComment", SqlDbType.NVarChar).Value = Modal.BellComment != null ? Modal.BellComment : "";
            command.Parameters.Add("@ForemastComment", SqlDbType.NVarChar).Value = Modal.ForemastComment != null ? Modal.ForemastComment : "";
            command.Parameters.Add("@FireComment", SqlDbType.NVarChar).Value = Modal.FireComment != null ? Modal.FireComment : "";
            command.Parameters.Add("@RailsComment", SqlDbType.NVarChar).Value = Modal.RailsComment != null ? Modal.RailsComment : "";
            command.Parameters.Add("@AntislippaintComment5R", SqlDbType.NVarChar).Value = Modal.AntislippaintComment5R != null ? Modal.AntislippaintComment5R : "";

            command.Parameters.Add("@ForecastleComment", SqlDbType.NVarChar).Value = Modal.ForecastleComment != null ? Modal.ForecastleComment : "";
            command.Parameters.Add("@CleanlinessComment5S", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5S != null ? Modal.CleanlinessComment5S : "";
            command.Parameters.Add("@PaintworkComment5S", SqlDbType.NVarChar).Value = Modal.PaintworkComment5S != null ? Modal.PaintworkComment5S : "";
            command.Parameters.Add("@ForepeakComment", SqlDbType.NVarChar).Value = Modal.ForepeakComment != null ? Modal.ForepeakComment : "";
            command.Parameters.Add("@ChainlockerComment", SqlDbType.NVarChar).Value = Modal.ChainlockerComment != null ? Modal.ChainlockerComment : "";
            command.Parameters.Add("@LightingComment5S", SqlDbType.NVarChar).Value = Modal.LightingComment5S != null ? Modal.LightingComment5S : "";
            command.Parameters.Add("@AccesssafetychainComment", SqlDbType.NVarChar).Value = Modal.AccesssafetychainComment != null ? Modal.AccesssafetychainComment : "";
            command.Parameters.Add("@EmergencyfirepumpComment", SqlDbType.NVarChar).Value = Modal.EmergencyfirepumpComment != null ? Modal.EmergencyfirepumpComment : "";
            command.Parameters.Add("@BowthrusterandroomComment", SqlDbType.NVarChar).Value = Modal.BowthrusterandroomComment != null ? Modal.BowthrusterandroomComment : "";
            command.Parameters.Add("@SparemooringlinesComment", SqlDbType.NVarChar).Value = Modal.SparemooringlinesComment != null ? Modal.SparemooringlinesComment : "";
            command.Parameters.Add("@PaintlockerComment", SqlDbType.NVarChar).Value = Modal.PaintlockerComment != null ? Modal.PaintlockerComment : "";
            command.Parameters.Add("@ForecastleSpaceComment", SqlDbType.NVarChar).Value = Modal.ForecastleSpaceComment != null ? Modal.ForecastleSpaceComment : "";
            command.Parameters.Add("@BoottopComment", SqlDbType.NVarChar).Value = Modal.BoottopComment != null ? Modal.BoottopComment : "";
            command.Parameters.Add("@TopsidesComment", SqlDbType.NVarChar).Value = Modal.TopsidesComment != null ? Modal.TopsidesComment : "";
            command.Parameters.Add("@AntifoulingComment", SqlDbType.NVarChar).Value = Modal.AntifoulingComment != null ? Modal.AntifoulingComment : "";
            command.Parameters.Add("@DraftandplimsollComment", SqlDbType.NVarChar).Value = Modal.DraftandplimsollComment != null ? Modal.DraftandplimsollComment : "";
            command.Parameters.Add("@FoulingComment", SqlDbType.NVarChar).Value = Modal.FoulingComment != null ? Modal.FoulingComment : "";
            command.Parameters.Add("@MechanicalComment", SqlDbType.NVarChar).Value = Modal.MechanicalComment != null ? Modal.MechanicalComment : "";
            command.Parameters.Add("@HullComment", SqlDbType.NVarChar).Value = Modal.HullComment != null ? Modal.HullComment : "";
            command.Parameters.Add("@SummaryComment", SqlDbType.NVarChar).Value = Modal.SummaryComment != null ? Modal.SummaryComment : "";

            command.Parameters.Add("@IsSynced", SqlDbType.NVarChar).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft == null ? DBNull.Value : (object)Modal.SavedAsDraft;

            command.Parameters.Add("@SnapBackZoneComment", SqlDbType.NVarChar).Value = Modal.SnapBackZoneComment != null ? Modal.SnapBackZoneComment : "";
            command.Parameters.Add("@ConditionGantryCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionGantryCranesComment != null ? Modal.ConditionGantryCranesComment : "";
            command.Parameters.Add("@MedicalLogBookComment", SqlDbType.NVarChar).Value = Modal.MedicalLogBookComment != null ? Modal.MedicalLogBookComment : "";
            command.Parameters.Add("@DrugsNarcoticsComment", SqlDbType.NVarChar).Value = Modal.DrugsNarcoticsComment != null ? Modal.DrugsNarcoticsComment : "";
            command.Parameters.Add("@DefibrillatorComment", SqlDbType.NVarChar).Value = Modal.DefibrillatorComment != null ? Modal.DefibrillatorComment : "";
            command.Parameters.Add("@RPWaterHandbook", SqlDbType.NVarChar).Value = Modal.RPWaterHandbook != null ? Modal.RPWaterHandbook : "";
            command.Parameters.Add("@BioRPWH", SqlDbType.NVarChar).Value = Modal.BioRPWH != null ? Modal.BioRPWH : "";
            command.Parameters.Add("@PRE", SqlDbType.NVarChar).Value = Modal.PRE != null ? Modal.PRE : "";
            command.Parameters.Add("@NoiseVibrationFile", SqlDbType.NVarChar).Value = Modal.NoiseVibrationFile != null ? Modal.NoiseVibrationFile : "";
            command.Parameters.Add("@BioMPR", SqlDbType.NVarChar).Value = Modal.BioMPR != null ? Modal.BioMPR : "";
            command.Parameters.Add("@AsbestosPlan", SqlDbType.NVarChar).Value = Modal.AsbestosPlan != null ? Modal.AsbestosPlan : "";
            command.Parameters.Add("@ShipPublicAddrComment", SqlDbType.NVarChar).Value = Modal.ShipPublicAddrComment != null ? Modal.ShipPublicAddrComment : "";
            command.Parameters.Add("@BridgewindowswiperssprayComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswiperssprayComment != null ? Modal.BridgewindowswiperssprayComment : "";
            command.Parameters.Add("@BridgewindowswipersComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswipersComment != null ? Modal.BridgewindowswipersComment : "";
            command.Parameters.Add("@DaylightSignalsComment", SqlDbType.NVarChar).Value = Modal.DaylightSignalsComment != null ? Modal.DaylightSignalsComment : "";
            command.Parameters.Add("@LiferaftDavitComment", SqlDbType.NVarChar).Value = Modal.LiferaftDavitComment != null ? Modal.LiferaftDavitComment : "";
            command.Parameters.Add("@CylindersLockerComment", SqlDbType.NVarChar).Value = Modal.CylindersLockerComment != null ? Modal.CylindersLockerComment : "";
            command.Parameters.Add("@SnapBackZone5NComment", SqlDbType.NVarChar).Value = Modal.SnapBackZone5NComment != null ? Modal.SnapBackZone5NComment : "";
            command.Parameters.Add("@ADPPublicationsComment", SqlDbType.NVarChar).Value = Modal.ADPPublicationsComment != null ? Modal.ADPPublicationsComment : "";

            //RDBJ 10/20/2021
            command.Parameters.Add("@IsGeneralSectionComplete", SqlDbType.Bit).Value = Modal.IsGeneralSectionComplete == null ? DBNull.Value : (object)Modal.IsGeneralSectionComplete;
            command.Parameters.Add("@IsManningSectionComplete", SqlDbType.Bit).Value = Modal.IsManningSectionComplete == null ? DBNull.Value : (object)Modal.IsManningSectionComplete;
            command.Parameters.Add("@IsShipCertificationSectionComplete", SqlDbType.Bit).Value = Modal.IsShipCertificationSectionComplete == null ? DBNull.Value : (object)Modal.IsShipCertificationSectionComplete;
            command.Parameters.Add("@IsRecordKeepingSectionComplete", SqlDbType.Bit).Value = Modal.IsRecordKeepingSectionComplete == null ? DBNull.Value : (object)Modal.IsRecordKeepingSectionComplete;
            command.Parameters.Add("@IsSafetyEquipmentSectionComplete", SqlDbType.Bit).Value = Modal.IsSafetyEquipmentSectionComplete == null ? DBNull.Value : (object)Modal.IsSafetyEquipmentSectionComplete;
            command.Parameters.Add("@IsSecuritySectionComplete", SqlDbType.Bit).Value = Modal.IsSecuritySectionComplete == null ? DBNull.Value : (object)Modal.IsSecuritySectionComplete;
            command.Parameters.Add("@IsBridgeSectionComplete", SqlDbType.Bit).Value = Modal.IsBridgeSectionComplete == null ? DBNull.Value : (object)Modal.IsBridgeSectionComplete;
            command.Parameters.Add("@IsMedicalSectionComplete", SqlDbType.Bit).Value = Modal.IsMedicalSectionComplete == null ? DBNull.Value : (object)Modal.IsMedicalSectionComplete;
            command.Parameters.Add("@IsGalleySectionComplete", SqlDbType.Bit).Value = Modal.IsGalleySectionComplete == null ? DBNull.Value : (object)Modal.IsGalleySectionComplete;
            command.Parameters.Add("@IsEngineRoomSectionComplete", SqlDbType.Bit).Value = Modal.IsEngineRoomSectionComplete == null ? DBNull.Value : (object)Modal.IsEngineRoomSectionComplete;
            command.Parameters.Add("@IsSuperstructureSectionComplete", SqlDbType.Bit).Value = Modal.IsSuperstructureSectionComplete == null ? DBNull.Value : (object)Modal.IsSuperstructureSectionComplete;
            command.Parameters.Add("@IsDeckSectionComplete", SqlDbType.Bit).Value = Modal.IsDeckSectionComplete == null ? DBNull.Value : (object)Modal.IsDeckSectionComplete;
            command.Parameters.Add("@IsHoldsAndCoverSectionComplete", SqlDbType.Bit).Value = Modal.IsHoldsAndCoverSectionComplete == null ? DBNull.Value : (object)Modal.IsHoldsAndCoverSectionComplete;
            command.Parameters.Add("@IsForeCastleSectionComplete", SqlDbType.Bit).Value = Modal.IsForeCastleSectionComplete == null ? DBNull.Value : (object)Modal.IsForeCastleSectionComplete;
            command.Parameters.Add("@IsHullSectionComplete", SqlDbType.Bit).Value = Modal.IsHullSectionComplete == null ? DBNull.Value : (object)Modal.IsHullSectionComplete;
            command.Parameters.Add("@IsSummarySectionComplete", SqlDbType.Bit).Value = Modal.IsSummarySectionComplete == null ? DBNull.Value : (object)Modal.IsSummarySectionComplete;
            command.Parameters.Add("@IsDeficienciesSectionComplete", SqlDbType.Bit).Value = Modal.IsDeficienciesSectionComplete == null ? DBNull.Value : (object)Modal.IsDeficienciesSectionComplete;
            command.Parameters.Add("@IsPhotographsSectionComplete", SqlDbType.Bit).Value = Modal.IsPhotographsSectionComplete == null ? DBNull.Value : (object)Modal.IsPhotographsSectionComplete;
            //ENd RDBJ 10/20/2021
        }
        public string GETGIRUpdateQuery()
        {
            // RDBJ 01/05/2022 Added isDelete
            //RDBJ 10/20/2021 Added 
            /*
            IsGeneralSectionComplete,IsManningSectionComplete,IsShipCertificationSectionComplete,IsRecordKeepingSectionComplete,
            IsSafetyEquipmentSectionComplete,IsSecuritySectionComplete,IsBridgeSectionComplete,IsMedicalSectionComplete,IsGalleySectionComplete,
            IsEngineRoomSectionComplete,IsSuperstructureSectionComplete,IsDeckSectionComplete,IsHoldsAndCoverSectionComplete,IsForeCastleSectionComplete,
            IsHullSectionComplete,IsSummarySectionComplete,IsDeficienciesSectionComplete,IsPhotographsSectionComplete
             */
            string query = @"UPDATE dbo.GeneralInspectionReport SET ShipID = @ShipID, FormVersion = @FormVersion, isDelete = @isDelete,
                           ShipName = @ShipName, Ship = @Ship, Port = @Port, Inspector = @Inspector, Date = @Date, GeneralPreamble = @GeneralPreamble, 
                           Classsociety = @Classsociety, YearofBuild = @YearofBuild, Flag = @Flag, Classofvessel = @Classofvessel, Portofregistry = @Portofregistry, 
                           MMSI = @MMSI, IMOnumber = @IMOnumber, Callsign = @Callsign, SummerDWT = @SummerDWT, Grosstonnage = @Grosstonnage, Lightweight = @Lightweight, 
                           Nettonnage = @Nettonnage, Beam = @Beam, LOA = @LOA, Summerdraft = @Summerdraft, LBP = @LBP, Bowthruster = @Bowthruster, BHP = @BHP, 
                           Noofholds = @Noofholds, Nomoveablebulkheads = @Nomoveablebulkheads, Containers = @Containers, Cargocapacity = @Cargocapacity, 
                           Cargohandlingequipment = @Cargohandlingequipment, Lastvoyageandcargo = @Lastvoyageandcargo, CurrentPlannedvoyageandcargo = @CurrentPlannedvoyageandcargo, 
                           ShipboardWorkingArrangements = @ShipboardWorkingArrangements, CertificationIndex = @CertificationIndex, 
                           CarriedOutByTheDOOW = @CarriedOutByTheDOOW, IsRegs4shipsDVD = @IsRegs4shipsDVD, Regs4shipsDVD = @Regs4shipsDVD, IsSOPEPPoints = @IsSOPEPPoints, 
                           SOPEPPoints = @SOPEPPoints, IsBWMP = @IsBWMP, BWMP = @BWMP, IsBWMPSupplement = @IsBWMPSupplement, BWMPSupplement = @BWMPSupplement, 
                           IsIntactStabilityManual = @IsIntactStabilityManual, IntactStabilityManual = @IntactStabilityManual, 
                           IsStabilityComputer = @IsStabilityComputer, StabilityComputer = @StabilityComputer, IsDateOfLast = @IsDateOfLast, DateOfLast = @DateOfLast, 
                           IsCargoSecuring = @IsCargoSecuring, CargoSecuring = @CargoSecuring, IsBulkCargo = @IsBulkCargo, 
                           BulkCargo = @BulkCargo, IsSMSManual = @IsSMSManual, SMSManual = @SMSManual, IsRegisterOf = @IsRegisterOf, RegisterOf = @RegisterOf, 
                           IsFleetStandingOrder = @IsFleetStandingOrder, FleetStandingOrder = @FleetStandingOrder, 
                           IsFleetMemoranda = @IsFleetMemoranda, FleetMemoranda = @FleetMemoranda, IsShipsPlans = @IsShipsPlans, ShipsPlans = @ShipsPlans, 
                           IsCollective = @IsCollective, Collective = @Collective, IsDraftAndFreeboardNotice = @IsDraftAndFreeboardNotice, 
                           DraftAndFreeboardNotice = @DraftAndFreeboardNotice, IsPCSOPEP = @IsPCSOPEP, PCSOPEP = @PCSOPEP, IsNTVRP = @IsNTVRP, NTVRP = @NTVRP, 
                           IsVGP = @IsVGP, VGP = @VGP,PubsComments = @PubsComments, IsPubsAndDocsSectionComplete = @IsPubsAndDocsSectionComplete, 
                           OfficialLogbookA = @OfficialLogbookA, OfficialLogbookB = @OfficialLogbookB, OfficialLogbookC = @OfficialLogbookC, OfficialLogbookD = @OfficialLogbookD, OfficialLogbookE = @OfficialLogbookE, 
                           DeckLogbook = @DeckLogbook, Listofcrew = @Listofcrew, LastHose = @LastHose, PassagePlanning = @PassagePlanning, 
                           LoadingComputer = @LoadingComputer, EngineLogbook = @EngineLogbook, OilRecordBook = @OilRecordBook, RiskAssessments = @RiskAssessments, 
                           GMDSSLogbook = @GMDSSLogbook, DeckLogbook5D = @DeckLogbook5D, GarbageRecordBook = @GarbageRecordBook, BallastWaterRecordBook = @BallastWaterRecordBook, 
                           CargoRecordBook = @CargoRecordBook, EmissionsControlManual = @EmissionsControlManual, LGR = @LGR, PEER = @PEER, 
                           RecordKeepingComments = @RecordKeepingComments, LastPortStateControl = @LastPortStateControl, LiferaftsComment = @LiferaftsComment, releasesComment = @releasesComment, 
                           LifeboatComment = @LifeboatComment, LifeboatdavitComment = @LifeboatdavitComment, LifeboatequipmentComment = @LifeboatequipmentComment, RescueboatComment = @RescueboatComment, 
                           RescueboatequipmentComment = @RescueboatequipmentComment, RescueboatoutboardmotorComment = @RescueboatoutboardmotorComment, 
                           RescueboatdavitComment = @RescueboatdavitComment, DeckComment = @DeckComment, PyrotechnicsComment = @PyrotechnicsComment, EPIRBComment = @EPIRBComment, 
                           SARTsComment = @SARTsComment, GMDSSComment = @GMDSSComment, ManoverboardComment = @ManoverboardComment, LinethrowingapparatusComment = @LinethrowingapparatusComment, 
                           FireextinguishersComment = @FireextinguishersComment, EmergencygeneratorComment = @EmergencygeneratorComment, CO2roomComment = @CO2roomComment, SurvivalComment = @SurvivalComment, 
                           LifejacketComment = @LifejacketComment, FiremansComment = @FiremansComment, LifebuoysComment = @LifebuoysComment, FireboxesComment = @FireboxesComment, 
                           EmergencybellsComment = @EmergencybellsComment, EmergencylightingComment = @EmergencylightingComment, FireplanComment = @FireplanComment, DamageComment = @DamageComment, 
                           EmergencyplansComment = @EmergencyplansComment, MusterlistComment = @MusterlistComment, SafetysignsComment = @SafetysignsComment, EmergencysteeringComment = @EmergencysteeringComment, 
                           StatutoryemergencydrillsComment = @StatutoryemergencydrillsComment, EEBDComment = @EEBDComment, OxygenComment = @OxygenComment, MultigasdetectorComment = @MultigasdetectorComment, 
                           GasdetectorComment = @GasdetectorComment, SufficientquantityComment = @SufficientquantityComment, BASetsComment = @BASetsComment, SafetyComment = @SafetyComment, 
                           GangwayComment = @GangwayComment, RestrictedComment = @RestrictedComment, OutsideComment = @OutsideComment, EntrancedoorsComment = @EntrancedoorsComment, 
                           AccommodationComment = @AccommodationComment, GMDSSComment5G = @GMDSSComment5G, VariousComment = @VariousComment, SSOComment = @SSOComment, 
                           SecuritylogbookComment = @SecuritylogbookComment, Listoflast10portsComment = @Listoflast10portsComment, PFSOComment = @PFSOComment, SecuritylevelComment = @SecuritylevelComment, 
                           DrillsandtrainingComment = @DrillsandtrainingComment, DOSComment = @DOSComment, SSASComment = @SSASComment, VisitorslogbookComment = @VisitorslogbookComment, 
                           KeyregisterComment = @KeyregisterComment, ShipSecurityComment = @ShipSecurityComment, SecurityComment = @SecurityComment, NauticalchartsComment = @NauticalchartsComment, 
                           NoticetomarinersComment = @NoticetomarinersComment, ListofradiosignalsComment = @ListofradiosignalsComment, ListoflightsComment = @ListoflightsComment, SailingdirectionsComment = @SailingdirectionsComment, 
                           TidetablesComment = @TidetablesComment, NavtexandprinterComment = @NavtexandprinterComment, RadarsComment = @RadarsComment, GPSComment = @GPSComment, 
                           AISComment = @AISComment, VDRComment = @VDRComment, ECDISComment = @ECDISComment, EchosounderComment = @EchosounderComment, 
                           ADPbackuplaptopComment = @ADPbackuplaptopComment, ColourprinterComment = @ColourprinterComment, VHFDSCtransceiverComment = @VHFDSCtransceiverComment, radioinstallationComment = @radioinstallationComment, 
                           InmarsatCComment = @InmarsatCComment, MagneticcompassComment = @MagneticcompassComment, SparecompassbowlComment = @SparecompassbowlComment, CompassobservationbookComment = @CompassobservationbookComment, 
                           GyrocompassComment = @GyrocompassComment, RudderindicatorComment = @RudderindicatorComment, SpeedlogComment = @SpeedlogComment, NavigationComment = @NavigationComment, 
                           SignalflagsComment = @SignalflagsComment, RPMComment = @RPMComment, BasicmanoeuvringdataComment = @BasicmanoeuvringdataComment, MasterstandingordersComment = @MasterstandingordersComment, 
                           MasternightordersbookComment = @MasternightordersbookComment, SextantComment = @SextantComment, AzimuthmirrorComment = @AzimuthmirrorComment, BridgepostersComment = @BridgepostersComment, 
                           ReviewofplannedComment = @ReviewofplannedComment, BridgebellbookComment = @BridgebellbookComment, BridgenavigationalComment = @BridgenavigationalComment, SecurityEquipmentComment = @SecurityEquipmentComment, 
                           NavigationPost = @NavigationPost, GeneralComment = @GeneralComment, MedicinestorageComment = @MedicinestorageComment, MedicinechestcertificateComment = @MedicinechestcertificateComment, 
                           InventoryStoresComment = @InventoryStoresComment, OxygencylindersComment = @OxygencylindersComment, StretcherComment = @StretcherComment, SalivaComment = @SalivaComment, 
                           AlcoholComment = @AlcoholComment, HospitalComment = @HospitalComment, GeneralGalleyComment = @GeneralGalleyComment, HygieneComment = @HygieneComment, 
                           FoodstorageComment = @FoodstorageComment, FoodlabellingComment = @FoodlabellingComment, GalleyriskassessmentComment = @GalleyriskassessmentComment, FridgetemperatureComment = @FridgetemperatureComment, 
                           FoodandProvisionsComment = @FoodandProvisionsComment, GalleyComment = @GalleyComment, 
                           ConditionComment = @ConditionComment, PaintworkComment = @PaintworkComment, LightingComment = @LightingComment, 
                           PlatesComment = @PlatesComment, BilgesComment = @BilgesComment, PipelinesandvalvesComment = @PipelinesandvalvesComment, 
                           LeakageComment = @LeakageComment, EquipmentComment = @EquipmentComment, OilywaterseparatorComment = @OilywaterseparatorComment, 
                           FueloiltransferplanComment = @FueloiltransferplanComment, SteeringgearComment = @SteeringgearComment, 
                           WorkshopandequipmentComment = @WorkshopandequipmentComment, SoundingpipesComment = @SoundingpipesComment, 
                           EnginecontrolComment = @EnginecontrolComment, ChiefEngineernightordersbookComment = @ChiefEngineernightordersbookComment, 
                           ChiefEngineerstandingordersComment = @ChiefEngineerstandingordersComment, PreUMSComment = @PreUMSComment, 
                           EnginebellbookComment = @EnginebellbookComment, LockoutComment = @LockoutComment, EngineRoomComment = @EngineRoomComment, 
                           CleanlinessandhygieneComment = @CleanlinessandhygieneComment, ConditionComment5M = @ConditionComment5M, 
                           PaintworkComment5M = @PaintworkComment5M, SignalmastandstaysComment = @SignalmastandstaysComment, 
                           MonkeyislandComment = @MonkeyislandComment, FireDampersComment = @FireDampersComment, 
                           RailsBulwarksComment = @RailsBulwarksComment, WatertightdoorsComment = @WatertightdoorsComment, 
                           VentilatorsComment = @VentilatorsComment, WinchesComment = @WinchesComment, 
                           FairleadsComment = @FairleadsComment, MooringLinesComment = @MooringLinesComment, 
                           EmergencyShutOffsComment = @EmergencyShutOffsComment, RadioaerialsComment = @RadioaerialsComment, 
                           SOPEPlockerComment = @SOPEPlockerComment, ChemicallockerComment = @ChemicallockerComment, 
                           AntislippaintComment = @AntislippaintComment, SuperstructureComment = @SuperstructureComment, 
                           CabinsComment = @CabinsComment, OfficesComment = @OfficesComment, MessroomsComment = @MessroomsComment, 
                           ToiletsComment = @ToiletsComment, LaundryroomComment = @LaundryroomComment, 
                           ChangingroomComment = @ChangingroomComment, OtherComment = @OtherComment, ConditionComment5N = @ConditionComment5N, 
                           SelfclosingfiredoorsComment = @SelfclosingfiredoorsComment, StairwellsComment = @StairwellsComment, SuperstructureInternalComment = @SuperstructureInternalComment, 
                           PortablegangwayComment = @PortablegangwayComment, SafetynetComment = @SafetynetComment, 
                           AccommodationLadderComment = @AccommodationLadderComment, SafeaccessprovidedComment = @SafeaccessprovidedComment, 
                           PilotladdersComment = @PilotladdersComment, BoardingEquipmentComment = @BoardingEquipmentComment, 
                           CleanlinessComment = @CleanlinessComment, PaintworkComment5P = @PaintworkComment5P, ShipsiderailsComment = @ShipsiderailsComment, 
                           WeathertightdoorsComment = @WeathertightdoorsComment, FirehydrantsComment = @FirehydrantsComment, 
                           VentilatorsComment5P = @VentilatorsComment5P, ManholecoversComment = @ManholecoversComment, MainDeckAreaComment = @MainDeckAreaComment, 
                           ConditionComment5Q = @ConditionComment5Q, PaintworkComment5Q = @PaintworkComment5Q, 
                           MechanicaldamageComment = @MechanicaldamageComment, AccessladdersComment = @AccessladdersComment, 
                           ManholecoversComment5Q = @ManholecoversComment5Q, HoldbilgeComment = @HoldbilgeComment, 
                           AccessdoorsComment = @AccessdoorsComment, ConditionHatchCoversComment = @ConditionHatchCoversComment, 
                           PaintworkHatchCoversComment = @PaintworkHatchCoversComment, RubbersealsComment = @RubbersealsComment, 
                           SignsofhatchesComment = @SignsofhatchesComment, SealingtapeComment = @SealingtapeComment, 
                           ConditionofhydraulicsComment = @ConditionofhydraulicsComment, PortablebulkheadsComment = @PortablebulkheadsComment, 
                           TweendecksComment = @TweendecksComment, HatchcoamingComment = @HatchcoamingComment, 
                           ConditionCargoCranesComment = @ConditionCargoCranesComment, GantrycranealarmComment = @GantrycranealarmComment, 
                           GantryconditionComment = @GantryconditionComment, CargoHoldsComment = @CargoHoldsComment, 
                           CleanlinessComment5R = @CleanlinessComment5R, PaintworkComment5R = @PaintworkComment5R, TriphazardsComment = @TriphazardsComment, 
                           WindlassComment = @WindlassComment, CablesComment = @CablesComment, WinchesComment5R = @WinchesComment5R, 
                           FairleadsComment5R = @FairleadsComment5R, MooringComment = @MooringComment, 
                           HatchToforecastlespaceComment = @HatchToforecastlespaceComment, VentilatorsComment5R = @VentilatorsComment5R, 
                           BellComment = @BellComment, ForemastComment = @ForemastComment, FireComment = @FireComment, 
                           RailsComment = @RailsComment, AntislippaintComment5R = @AntislippaintComment5R,
                           SnapBackZoneComment = @SnapBackZoneComment,
                           ConditionGantryCranesComment=@ConditionGantryCranesComment,
                           CylindersLockerComment=@CylindersLockerComment,SnapBackZone5NComment=@SnapBackZone5NComment,
                           MedicalLogBookComment=@MedicalLogBookComment, DrugsNarcoticsComment=@DrugsNarcoticsComment,
                           DefibrillatorComment=@DefibrillatorComment,RPWaterHandbook=@RPWaterHandbook,BioRPWH=@BioRPWH,
                           PRE=@PRE,NoiseVibrationFile=@NoiseVibrationFile,BioMPR=@BioMPR,
                           AsbestosPlan=@AsbestosPlan,ShipPublicAddrComment=@ShipPublicAddrComment,
                           BridgewindowswiperssprayComment=@BridgewindowswiperssprayComment,
                           BridgewindowswipersComment=@BridgewindowswipersComment,DaylightSignalsComment=@DaylightSignalsComment,
                           LiferaftDavitComment=@LiferaftDavitComment,ADPPublicationsComment=@ADPPublicationsComment,
                           ForecastleComment = @ForecastleComment, 
                           CleanlinessComment5S = @CleanlinessComment5S, PaintworkComment5S = @PaintworkComment5S, 
                           ForepeakComment = @ForepeakComment, ChainlockerComment = @ChainlockerComment, LightingComment5S = @LightingComment5S, 
                           AccesssafetychainComment = @AccesssafetychainComment, EmergencyfirepumpComment = @EmergencyfirepumpComment, 
                           BowthrusterandroomComment = @BowthrusterandroomComment, SparemooringlinesComment = @SparemooringlinesComment, 
                           PaintlockerComment = @PaintlockerComment, ForecastleSpaceComment = @ForecastleSpaceComment, 
                           BoottopComment = @BoottopComment, TopsidesComment = @TopsidesComment, AntifoulingComment = @AntifoulingComment, 
                           DraftandplimsollComment = @DraftandplimsollComment, FoulingComment = @FoulingComment, MechanicalComment = @MechanicalComment, 
                           HullComment = @HullComment, SummaryComment = @SummaryComment, 
                           IsSynced = @IsSynced, UpdatedDate = @UpdatedDate, SavedAsDraft = @SavedAsDraft, CreatedDate = @CreatedDate, IsGeneralSectionComplete = @IsGeneralSectionComplete
                           , IsManningSectionComplete = @IsManningSectionComplete, IsShipCertificationSectionComplete = @IsShipCertificationSectionComplete, IsRecordKeepingSectionComplete = @IsRecordKeepingSectionComplete
                           , IsSafetyEquipmentSectionComplete = @IsSafetyEquipmentSectionComplete, IsSecuritySectionComplete = @IsSecuritySectionComplete, IsBridgeSectionComplete = @IsBridgeSectionComplete
                           , IsMedicalSectionComplete = @IsMedicalSectionComplete, IsGalleySectionComplete = @IsGalleySectionComplete, IsEngineRoomSectionComplete = @IsEngineRoomSectionComplete
                           , IsSuperstructureSectionComplete = @IsSuperstructureSectionComplete, IsDeckSectionComplete = @IsDeckSectionComplete, IsHoldsAndCoverSectionComplete = @IsHoldsAndCoverSectionComplete
                           , IsForeCastleSectionComplete = @IsForeCastleSectionComplete, IsHullSectionComplete = @IsHullSectionComplete, IsSummarySectionComplete = @IsSummarySectionComplete
                           , IsDeficienciesSectionComplete = @IsDeficienciesSectionComplete, IsPhotographsSectionComplete = @IsPhotographsSectionComplete  
                           WHERE UniqueFormID = @UniqueFormID";
            return query;
        }
        public void GIRUpdateCMD(GeneralInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.FormVersion;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete == null ? DBNull.Value : (object)Modal.isDelete; // RDBJ 01/05/2022
            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID == null ? DBNull.Value : (object)Modal.ShipID;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName == null ? string.Empty : Modal.ShipName;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? string.Empty : Modal.Ship;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port == null ? string.Empty : Modal.Port;
            command.Parameters.Add("@Inspector", SqlDbType.NVarChar).Value = Modal.Inspector == null ? string.Empty : Modal.Inspector;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date == null ? DBNull.Value : (object)Modal.Date;
            command.Parameters.Add("@GeneralPreamble", SqlDbType.NVarChar).Value = Modal.GeneralPreamble != null ? Modal.GeneralPreamble : "";
            command.Parameters.Add("@Classsociety", SqlDbType.NVarChar).Value = Modal.Classsociety != null ? Modal.Classsociety : "";
            command.Parameters.Add("@YearofBuild", SqlDbType.NVarChar).Value = Modal.YearofBuild != null ? Modal.YearofBuild : "";
            command.Parameters.Add("@Flag", SqlDbType.NVarChar).Value = Modal.Flag != null ? Modal.Flag : "";
            command.Parameters.Add("@Classofvessel", SqlDbType.NVarChar).Value = Modal.Classofvessel != null ? Modal.Classofvessel : "";
            command.Parameters.Add("@Portofregistry", SqlDbType.NVarChar).Value = Modal.Portofregistry != null ? Modal.Portofregistry : "";
            command.Parameters.Add("@MMSI", SqlDbType.NVarChar).Value = Modal.MMSI != null ? Modal.MMSI : "";
            command.Parameters.Add("@IMOnumber", SqlDbType.NVarChar).Value = Modal.IMOnumber != null ? Modal.IMOnumber : "";
            command.Parameters.Add("@Callsign", SqlDbType.NVarChar).Value = Modal.Callsign != null ? Modal.Callsign : "";
            command.Parameters.Add("@SummerDWT", SqlDbType.NVarChar).Value = Modal.SummerDWT != null ? Modal.SummerDWT : "";
            command.Parameters.Add("@Grosstonnage", SqlDbType.NVarChar).Value = Modal.Grosstonnage != null ? Modal.Grosstonnage : "";
            command.Parameters.Add("@Lightweight", SqlDbType.NVarChar).Value = Modal.Lightweight != null ? Modal.Lightweight : "";
            command.Parameters.Add("@Nettonnage", SqlDbType.NVarChar).Value = Modal.Nettonnage != null ? Modal.Nettonnage : "";
            command.Parameters.Add("@Beam", SqlDbType.NVarChar).Value = Modal.Beam != null ? Modal.Beam : "";
            command.Parameters.Add("@LOA", SqlDbType.NVarChar).Value = Modal.LOA != null ? Modal.LOA : "";
            command.Parameters.Add("@Summerdraft", SqlDbType.NVarChar).Value = Modal.Summerdraft != null ? Modal.Summerdraft : "";
            command.Parameters.Add("@LBP", SqlDbType.NVarChar).Value = Modal.LBP != null ? Modal.LBP : "";
            command.Parameters.Add("@Bowthruster", SqlDbType.NVarChar).Value = Modal.Bowthruster != null ? Modal.Bowthruster : "";
            command.Parameters.Add("@BHP", SqlDbType.NVarChar).Value = Modal.BHP != null ? Modal.BHP : "";
            command.Parameters.Add("@Noofholds", SqlDbType.NVarChar).Value = Modal.Noofholds != null ? Modal.Noofholds : "";
            command.Parameters.Add("@Nomoveablebulkheads", SqlDbType.NVarChar).Value = Modal.Nomoveablebulkheads != null ? Modal.Nomoveablebulkheads : "";
            command.Parameters.Add("@Containers", SqlDbType.NVarChar).Value = Modal.Containers != null ? Modal.Containers : "";
            command.Parameters.Add("@Cargocapacity", SqlDbType.NVarChar).Value = Modal.Cargocapacity != null ? Modal.Cargocapacity : "";
            command.Parameters.Add("@Cargohandlingequipment", SqlDbType.NVarChar).Value = Modal.Cargohandlingequipment != null ? Modal.Cargohandlingequipment : "";
            command.Parameters.Add("@Lastvoyageandcargo", SqlDbType.NVarChar).Value = Modal.Lastvoyageandcargo != null ? Modal.Lastvoyageandcargo : "";
            command.Parameters.Add("@CurrentPlannedvoyageandcargo", SqlDbType.NVarChar).Value = Modal.CurrentPlannedvoyageandcargo != null ? Modal.CurrentPlannedvoyageandcargo : "";
            command.Parameters.Add("@ShipboardWorkingArrangements", SqlDbType.NVarChar).Value = Modal.ShipboardWorkingArrangements != null ? Modal.ShipboardWorkingArrangements : "";
            command.Parameters.Add("@CertificationIndex", SqlDbType.NVarChar).Value = Modal.CertificationIndex != null ? Modal.CertificationIndex : "";
            command.Parameters.Add("@CarriedOutByTheDOOW", SqlDbType.NVarChar).Value = Modal.CarriedOutByTheDOOW != null ? Modal.CarriedOutByTheDOOW : "";
            command.Parameters.Add("@IsSOPEPPoints", SqlDbType.Bit).Value = Modal.IsSOPEPPoints == null ? DBNull.Value : (object)Modal.IsSOPEPPoints;
            command.Parameters.Add("@SOPEPPoints", SqlDbType.NVarChar).Value = Modal.SOPEPPoints != null ? Modal.SOPEPPoints : "";
            command.Parameters.Add("@IsRegs4shipsDVD", SqlDbType.Bit).Value = Modal.IsRegs4shipsDVD == null ? DBNull.Value : (object)Modal.IsRegs4shipsDVD;
            command.Parameters.Add("@Regs4shipsDVD", SqlDbType.NVarChar).Value = Modal.Regs4shipsDVD != null ? Modal.Regs4shipsDVD : "";
            command.Parameters.Add("@IsBWMP", SqlDbType.Bit).Value = Modal.IsBWMP == null ? DBNull.Value : (object)Modal.IsBWMP;
            command.Parameters.Add("@BWMP", SqlDbType.NVarChar).Value = Modal.BWMP != null ? Modal.BWMP : "";
            command.Parameters.Add("@IsBWMPSupplement", SqlDbType.Bit).Value = Modal.IsBWMPSupplement == null ? DBNull.Value : (object)Modal.IsBWMPSupplement;
            command.Parameters.Add("@BWMPSupplement", SqlDbType.NVarChar).Value = Modal.BWMPSupplement != null ? Modal.BWMPSupplement : "";
            command.Parameters.Add("@IsIntactStabilityManual", SqlDbType.Bit).Value = Modal.IsIntactStabilityManual == null ? DBNull.Value : (object)Modal.IsIntactStabilityManual;
            command.Parameters.Add("@IntactStabilityManual", SqlDbType.NVarChar).Value = Modal.IntactStabilityManual != null ? Modal.IntactStabilityManual : "";
            command.Parameters.Add("@IsStabilityComputer", SqlDbType.Bit).Value = Modal.IsStabilityComputer == null ? DBNull.Value : (object)Modal.IsStabilityComputer;
            command.Parameters.Add("@StabilityComputer", SqlDbType.NVarChar).Value = Modal.StabilityComputer != null ? Modal.StabilityComputer : "";
            command.Parameters.Add("@IsDateOfLast", SqlDbType.Bit).Value = Modal.IsDateOfLast == null ? DBNull.Value : (object)Modal.IsDateOfLast;
            command.Parameters.Add("@DateOfLast", SqlDbType.NVarChar).Value = Modal.DateOfLast != null ? Modal.DateOfLast : "";
            command.Parameters.Add("@IsCargoSecuring", SqlDbType.Bit).Value = Modal.IsCargoSecuring == null ? DBNull.Value : (object)Modal.IsCargoSecuring;
            command.Parameters.Add("@CargoSecuring", SqlDbType.NVarChar).Value = Modal.CargoSecuring != null ? Modal.CargoSecuring : "";
            command.Parameters.Add("@IsBulkCargo", SqlDbType.Bit).Value = Modal.IsBulkCargo == null ? DBNull.Value : (object)Modal.IsBulkCargo;
            command.Parameters.Add("@BulkCargo", SqlDbType.NVarChar).Value = Modal.BulkCargo != null ? Modal.BulkCargo : "";
            command.Parameters.Add("@IsSMSManual", SqlDbType.Bit).Value = Modal.IsSMSManual == null ? DBNull.Value : (object)Modal.IsSMSManual;
            command.Parameters.Add("@SMSManual", SqlDbType.NVarChar).Value = Modal.SMSManual != null ? Modal.SMSManual : "";
            command.Parameters.Add("@IsRegisterOf", SqlDbType.Bit).Value = Modal.IsRegisterOf == null ? DBNull.Value : (object)Modal.IsRegisterOf;
            command.Parameters.Add("@RegisterOf", SqlDbType.NVarChar).Value = Modal.RegisterOf != null ? Modal.RegisterOf : "";
            command.Parameters.Add("@IsFleetStandingOrder", SqlDbType.Bit).Value = Modal.IsFleetStandingOrder == null ? DBNull.Value : (object)Modal.IsFleetStandingOrder;
            command.Parameters.Add("@FleetStandingOrder", SqlDbType.NVarChar).Value = Modal.FleetStandingOrder != null ? Modal.FleetStandingOrder : "";
            command.Parameters.Add("@IsFleetMemoranda", SqlDbType.Bit).Value = Modal.IsFleetMemoranda == null ? DBNull.Value : (object)Modal.IsFleetMemoranda;
            command.Parameters.Add("@FleetMemoranda", SqlDbType.NVarChar).Value = Modal.FleetMemoranda != null ? Modal.FleetMemoranda : "";
            command.Parameters.Add("@IsShipsPlans", SqlDbType.Bit).Value = Modal.IsShipsPlans == null ? DBNull.Value : (object)Modal.IsShipsPlans;
            command.Parameters.Add("@ShipsPlans", SqlDbType.NVarChar).Value = Modal.ShipsPlans != null ? Modal.ShipsPlans : "";
            command.Parameters.Add("@IsCollective", SqlDbType.Bit).Value = Modal.IsCollective == null ? DBNull.Value : (object)Modal.IsCollective;
            command.Parameters.Add("@Collective", SqlDbType.NVarChar).Value = Modal.Collective != null ? Modal.Collective : "";
            command.Parameters.Add("@IsDraftAndFreeboardNotice", SqlDbType.Bit).Value = Modal.IsDraftAndFreeboardNotice == null ? DBNull.Value : (object)Modal.IsDraftAndFreeboardNotice;
            command.Parameters.Add("@DraftAndFreeboardNotice", SqlDbType.NVarChar).Value = Modal.DraftAndFreeboardNotice != null ? Modal.DraftAndFreeboardNotice : "";
            command.Parameters.Add("@IsPCSOPEP", SqlDbType.Bit).Value = Modal.IsPCSOPEP == null ? DBNull.Value : (object)Modal.IsPCSOPEP;
            command.Parameters.Add("@PCSOPEP", SqlDbType.NVarChar).Value = Modal.PCSOPEP != null ? Modal.PCSOPEP : "";
            command.Parameters.Add("@IsNTVRP", SqlDbType.Bit).Value = Modal.IsNTVRP == null ? DBNull.Value : (object)Modal.IsNTVRP;
            command.Parameters.Add("@NTVRP", SqlDbType.NVarChar).Value = Modal.NTVRP != null ? Modal.NTVRP : "";
            command.Parameters.Add("@IsVGP", SqlDbType.Bit).Value = Modal.IsVGP == null ? DBNull.Value : (object)Modal.IsVGP;
            command.Parameters.Add("@VGP", SqlDbType.NVarChar).Value = Modal.VGP != null ? Modal.VGP : "";
            command.Parameters.Add("@PubsComments", SqlDbType.NVarChar).Value = Modal.PubsComments != null ? Modal.PubsComments : "";
            command.Parameters.Add("@IsPubsAndDocsSectionComplete", SqlDbType.Bit).Value = Modal.IsPubsAndDocsSectionComplete == null ? DBNull.Value : (object)Modal.IsPubsAndDocsSectionComplete;
            command.Parameters.Add("@OfficialLogbookA", SqlDbType.NVarChar).Value = Modal.OfficialLogbookA != null ? Modal.OfficialLogbookA : "";
            command.Parameters.Add("@OfficialLogbookB", SqlDbType.NVarChar).Value = Modal.OfficialLogbookB != null ? Modal.OfficialLogbookB : "";
            command.Parameters.Add("@OfficialLogbookC", SqlDbType.NVarChar).Value = Modal.OfficialLogbookC != null ? Modal.OfficialLogbookC : "";
            command.Parameters.Add("@OfficialLogbookD", SqlDbType.NVarChar).Value = Modal.OfficialLogbookD != null ? Modal.OfficialLogbookD : "";
            command.Parameters.Add("@OfficialLogbookE", SqlDbType.NVarChar).Value = Modal.OfficialLogbookE != null ? Modal.OfficialLogbookE : "";
            command.Parameters.Add("@DeckLogbook", SqlDbType.NVarChar).Value = Modal.DeckLogbook != null ? Modal.DeckLogbook : "";
            command.Parameters.Add("@Listofcrew", SqlDbType.NVarChar).Value = Modal.Listofcrew != null ? Modal.Listofcrew : "";
            command.Parameters.Add("@LastHose", SqlDbType.NVarChar).Value = Modal.LastHose != null ? Modal.LastHose : "";
            command.Parameters.Add("@PassagePlanning", SqlDbType.NVarChar).Value = Modal.PassagePlanning != null ? Modal.PassagePlanning : "";
            command.Parameters.Add("@LoadingComputer", SqlDbType.NVarChar).Value = Modal.LoadingComputer != null ? Modal.LoadingComputer : "";
            command.Parameters.Add("@EngineLogbook", SqlDbType.NVarChar).Value = Modal.EngineLogbook != null ? Modal.EngineLogbook : "";
            command.Parameters.Add("@OilRecordBook", SqlDbType.NVarChar).Value = Modal.OilRecordBook != null ? Modal.OilRecordBook : "";
            command.Parameters.Add("@RiskAssessments", SqlDbType.NVarChar).Value = Modal.RiskAssessments != null ? Modal.RiskAssessments : "";
            command.Parameters.Add("@GMDSSLogbook", SqlDbType.NVarChar).Value = Modal.GMDSSLogbook != null ? Modal.GMDSSLogbook : "";
            command.Parameters.Add("@DeckLogbook5D", SqlDbType.NVarChar).Value = Modal.DeckLogbook5D != null ? Modal.DeckLogbook5D : "";
            command.Parameters.Add("@GarbageRecordBook", SqlDbType.NVarChar).Value = Modal.GarbageRecordBook != null ? Modal.GarbageRecordBook : "";
            command.Parameters.Add("@BallastWaterRecordBook", SqlDbType.NVarChar).Value = Modal.BallastWaterRecordBook != null ? Modal.BallastWaterRecordBook : "";
            command.Parameters.Add("@CargoRecordBook", SqlDbType.NVarChar).Value = Modal.CargoRecordBook != null ? Modal.CargoRecordBook : "";
            command.Parameters.Add("@EmissionsControlManual", SqlDbType.NVarChar).Value = Modal.EmissionsControlManual != null ? Modal.EmissionsControlManual : "";
            command.Parameters.Add("@LGR", SqlDbType.NVarChar).Value = Modal.LGR != null ? Modal.LGR : "";
            command.Parameters.Add("@PEER", SqlDbType.NVarChar).Value = Modal.PEER != null ? Modal.PEER : "";
            command.Parameters.Add("@RecordKeepingComments", SqlDbType.NVarChar).Value = Modal.RecordKeepingComments != null ? Modal.RecordKeepingComments : "";
            command.Parameters.Add("@LastPortStateControl", SqlDbType.NVarChar).Value = Modal.LastPortStateControl != null ? Modal.LastPortStateControl : "";

            command.Parameters.Add("@LiferaftsComment", SqlDbType.NVarChar).Value = Modal.LiferaftsComment != null ? Modal.LiferaftsComment : "";
            command.Parameters.Add("@releasesComment", SqlDbType.NVarChar).Value = Modal.releasesComment != null ? Modal.releasesComment : "";
            command.Parameters.Add("@LifeboatComment", SqlDbType.NVarChar).Value = Modal.LifeboatComment != null ? Modal.LifeboatComment : "";
            command.Parameters.Add("@LifeboatdavitComment", SqlDbType.NVarChar).Value = Modal.LifeboatdavitComment != null ? Modal.LifeboatdavitComment : "";
            command.Parameters.Add("@LifeboatequipmentComment", SqlDbType.NVarChar).Value = Modal.LifeboatequipmentComment != null ? Modal.LifeboatequipmentComment : "";
            command.Parameters.Add("@RescueboatComment", SqlDbType.NVarChar).Value = Modal.RescueboatComment != null ? Modal.RescueboatComment : "";
            command.Parameters.Add("@RescueboatequipmentComment", SqlDbType.NVarChar).Value = Modal.RescueboatequipmentComment != null ? Modal.RescueboatequipmentComment : "";
            command.Parameters.Add("@RescueboatoutboardmotorComment", SqlDbType.NVarChar).Value = Modal.RescueboatoutboardmotorComment != null ? Modal.RescueboatoutboardmotorComment : "";
            command.Parameters.Add("@RescueboatdavitComment", SqlDbType.NVarChar).Value = Modal.RescueboatdavitComment != null ? Modal.RescueboatdavitComment : "";
            command.Parameters.Add("@DeckComment", SqlDbType.NVarChar).Value = Modal.DeckComment != null ? Modal.DeckComment : "";
            command.Parameters.Add("@PyrotechnicsComment", SqlDbType.NVarChar).Value = Modal.PyrotechnicsComment != null ? Modal.PyrotechnicsComment : "";
            command.Parameters.Add("@EPIRBComment", SqlDbType.NVarChar).Value = Modal.EPIRBComment != null ? Modal.EPIRBComment : "";
            command.Parameters.Add("@SARTsComment", SqlDbType.NVarChar).Value = Modal.SARTsComment != null ? Modal.SARTsComment : "";
            command.Parameters.Add("@GMDSSComment", SqlDbType.NVarChar).Value = Modal.GMDSSComment != null ? Modal.GMDSSComment : "";
            command.Parameters.Add("@ManoverboardComment", SqlDbType.NVarChar).Value = Modal.ManoverboardComment != null ? Modal.ManoverboardComment : "";
            command.Parameters.Add("@LinethrowingapparatusComment", SqlDbType.NVarChar).Value = Modal.LinethrowingapparatusComment != null ? Modal.LinethrowingapparatusComment : "";
            command.Parameters.Add("@FireextinguishersComment", SqlDbType.NVarChar).Value = Modal.FireextinguishersComment != null ? Modal.FireextinguishersComment : "";
            command.Parameters.Add("@EmergencygeneratorComment", SqlDbType.NVarChar).Value = Modal.EmergencygeneratorComment != null ? Modal.EmergencygeneratorComment : "";
            command.Parameters.Add("@CO2roomComment", SqlDbType.NVarChar).Value = Modal.CO2roomComment != null ? Modal.CO2roomComment : "";
            command.Parameters.Add("@SurvivalComment", SqlDbType.NVarChar).Value = Modal.SurvivalComment != null ? Modal.SurvivalComment : "";
            command.Parameters.Add("@LifejacketComment", SqlDbType.NVarChar).Value = Modal.LifejacketComment != null ? Modal.LifejacketComment : "";
            command.Parameters.Add("@FiremansComment", SqlDbType.NVarChar).Value = Modal.FiremansComment != null ? Modal.FiremansComment : "";
            command.Parameters.Add("@LifebuoysComment", SqlDbType.NVarChar).Value = Modal.LifebuoysComment != null ? Modal.LifebuoysComment : "";
            command.Parameters.Add("@FireboxesComment", SqlDbType.NVarChar).Value = Modal.FireboxesComment != null ? Modal.FireboxesComment : "";
            command.Parameters.Add("@EmergencybellsComment", SqlDbType.NVarChar).Value = Modal.EmergencybellsComment != null ? Modal.EmergencybellsComment : "";
            command.Parameters.Add("@EmergencylightingComment", SqlDbType.NVarChar).Value = Modal.EmergencylightingComment != null ? Modal.EmergencylightingComment : "";
            command.Parameters.Add("@FireplanComment", SqlDbType.NVarChar).Value = Modal.FireplanComment != null ? Modal.FireplanComment : "";
            command.Parameters.Add("@DamageComment", SqlDbType.NVarChar).Value = Modal.DamageComment != null ? Modal.DamageComment : "";
            command.Parameters.Add("@EmergencyplansComment", SqlDbType.NVarChar).Value = Modal.EmergencyplansComment != null ? Modal.EmergencyplansComment : "";
            command.Parameters.Add("@MusterlistComment", SqlDbType.NVarChar).Value = Modal.MusterlistComment != null ? Modal.MusterlistComment : "";
            command.Parameters.Add("@SafetysignsComment", SqlDbType.NVarChar).Value = Modal.SafetysignsComment != null ? Modal.SafetysignsComment : "";
            command.Parameters.Add("@EmergencysteeringComment", SqlDbType.NVarChar).Value = Modal.EmergencysteeringComment != null ? Modal.EmergencysteeringComment : "";
            command.Parameters.Add("@StatutoryemergencydrillsComment", SqlDbType.NVarChar).Value = Modal.StatutoryemergencydrillsComment != null ? Modal.StatutoryemergencydrillsComment : "";
            command.Parameters.Add("@EEBDComment", SqlDbType.NVarChar).Value = Modal.EEBDComment != null ? Modal.EEBDComment : "";
            command.Parameters.Add("@OxygenComment", SqlDbType.NVarChar).Value = Modal.OxygenComment != null ? Modal.OxygenComment : "";
            command.Parameters.Add("@MultigasdetectorComment", SqlDbType.NVarChar).Value = Modal.MultigasdetectorComment != null ? Modal.MultigasdetectorComment : "";
            command.Parameters.Add("@GasdetectorComment", SqlDbType.NVarChar).Value = Modal.GasdetectorComment != null ? Modal.GasdetectorComment : "";
            command.Parameters.Add("@SufficientquantityComment", SqlDbType.NVarChar).Value = Modal.SufficientquantityComment != null ? Modal.SufficientquantityComment : "";
            command.Parameters.Add("@BASetsComment", SqlDbType.NVarChar).Value = Modal.BASetsComment != null ? Modal.BASetsComment : "";
            command.Parameters.Add("@SafetyComment", SqlDbType.NVarChar).Value = Modal.SafetyComment != null ? Modal.SafetyComment : "";

            command.Parameters.Add("@GangwayComment", SqlDbType.NVarChar).Value = Modal.GangwayComment != null ? Modal.GangwayComment : "";
            command.Parameters.Add("@RestrictedComment", SqlDbType.NVarChar).Value = Modal.RestrictedComment != null ? Modal.RestrictedComment : "";
            command.Parameters.Add("@OutsideComment", SqlDbType.NVarChar).Value = Modal.OutsideComment != null ? Modal.OutsideComment : "";
            command.Parameters.Add("@EntrancedoorsComment", SqlDbType.NVarChar).Value = Modal.EntrancedoorsComment != null ? Modal.EntrancedoorsComment : "";
            command.Parameters.Add("@AccommodationComment", SqlDbType.NVarChar).Value = Modal.AccommodationComment != null ? Modal.AccommodationComment : "";
            command.Parameters.Add("@GMDSSComment5G", SqlDbType.NVarChar).Value = Modal.GMDSSComment5G != null ? Modal.GMDSSComment5G : "";
            command.Parameters.Add("@VariousComment", SqlDbType.NVarChar).Value = Modal.VariousComment != null ? Modal.VariousComment : "";
            command.Parameters.Add("@SSOComment", SqlDbType.NVarChar).Value = Modal.SSOComment != null ? Modal.SSOComment : "";
            command.Parameters.Add("@SecuritylogbookComment", SqlDbType.NVarChar).Value = Modal.SecuritylogbookComment != null ? Modal.SecuritylogbookComment : "";
            command.Parameters.Add("@Listoflast10portsComment", SqlDbType.NVarChar).Value = Modal.Listoflast10portsComment != null ? Modal.Listoflast10portsComment : "";
            command.Parameters.Add("@PFSOComment", SqlDbType.NVarChar).Value = Modal.PFSOComment != null ? Modal.PFSOComment : "";
            command.Parameters.Add("@SecuritylevelComment", SqlDbType.NVarChar).Value = Modal.SecuritylevelComment != null ? Modal.SecuritylevelComment : "";
            command.Parameters.Add("@DrillsandtrainingComment", SqlDbType.NVarChar).Value = Modal.DrillsandtrainingComment != null ? Modal.DrillsandtrainingComment : "";
            command.Parameters.Add("@DOSComment", SqlDbType.NVarChar).Value = Modal.DOSComment != null ? Modal.DOSComment : "";
            command.Parameters.Add("@SSASComment", SqlDbType.NVarChar).Value = Modal.SSASComment != null ? Modal.SSASComment : "";
            command.Parameters.Add("@VisitorslogbookComment", SqlDbType.NVarChar).Value = Modal.VisitorslogbookComment != null ? Modal.VisitorslogbookComment : "";
            command.Parameters.Add("@KeyregisterComment", SqlDbType.NVarChar).Value = Modal.KeyregisterComment != null ? Modal.KeyregisterComment : "";
            command.Parameters.Add("@ShipSecurityComment", SqlDbType.NVarChar).Value = Modal.ShipSecurityComment != null ? Modal.ShipSecurityComment : "";
            command.Parameters.Add("@SecurityComment", SqlDbType.NVarChar).Value = Modal.SecurityComment != null ? Modal.SecurityComment : "";

            command.Parameters.Add("@NauticalchartsComment", SqlDbType.NVarChar).Value = Modal.NauticalchartsComment != null ? Modal.NauticalchartsComment : "";
            command.Parameters.Add("@NoticetomarinersComment", SqlDbType.NVarChar).Value = Modal.NoticetomarinersComment != null ? Modal.NoticetomarinersComment : "";
            command.Parameters.Add("@ListofradiosignalsComment", SqlDbType.NVarChar).Value = Modal.ListofradiosignalsComment != null ? Modal.ListofradiosignalsComment : "";
            command.Parameters.Add("@ListoflightsComment", SqlDbType.NVarChar).Value = Modal.ListoflightsComment != null ? Modal.ListoflightsComment : "";
            command.Parameters.Add("@SailingdirectionsComment", SqlDbType.NVarChar).Value = Modal.SailingdirectionsComment != null ? Modal.SailingdirectionsComment : "";
            command.Parameters.Add("@TidetablesComment", SqlDbType.NVarChar).Value = Modal.TidetablesComment != null ? Modal.TidetablesComment : "";
            command.Parameters.Add("@NavtexandprinterComment", SqlDbType.NVarChar).Value = Modal.NavtexandprinterComment != null ? Modal.NavtexandprinterComment : "";
            command.Parameters.Add("@RadarsComment", SqlDbType.NVarChar).Value = Modal.RadarsComment != null ? Modal.RadarsComment : "";
            command.Parameters.Add("@GPSComment", SqlDbType.NVarChar).Value = Modal.GPSComment != null ? Modal.GPSComment : "";
            command.Parameters.Add("@AISComment", SqlDbType.NVarChar).Value = Modal.AISComment != null ? Modal.AISComment : "";
            command.Parameters.Add("@VDRComment", SqlDbType.NVarChar).Value = Modal.VDRComment != null ? Modal.VDRComment : "";
            command.Parameters.Add("@ECDISComment", SqlDbType.NVarChar).Value = Modal.ECDISComment != null ? Modal.ECDISComment : "";
            command.Parameters.Add("@EchosounderComment", SqlDbType.NVarChar).Value = Modal.EchosounderComment != null ? Modal.EchosounderComment : "";
            command.Parameters.Add("@ADPbackuplaptopComment", SqlDbType.NVarChar).Value = Modal.ADPbackuplaptopComment != null ? Modal.ADPbackuplaptopComment : "";
            command.Parameters.Add("@ColourprinterComment", SqlDbType.NVarChar).Value = Modal.ColourprinterComment != null ? Modal.ColourprinterComment : "";
            command.Parameters.Add("@VHFDSCtransceiverComment", SqlDbType.NVarChar).Value = Modal.VHFDSCtransceiverComment != null ? Modal.VHFDSCtransceiverComment : "";
            command.Parameters.Add("@radioinstallationComment", SqlDbType.NVarChar).Value = Modal.radioinstallationComment != null ? Modal.radioinstallationComment : "";
            command.Parameters.Add("@InmarsatCComment", SqlDbType.NVarChar).Value = Modal.InmarsatCComment != null ? Modal.InmarsatCComment : "";
            command.Parameters.Add("@MagneticcompassComment", SqlDbType.NVarChar).Value = Modal.MagneticcompassComment != null ? Modal.MagneticcompassComment : "";
            command.Parameters.Add("@SparecompassbowlComment", SqlDbType.NVarChar).Value = Modal.SparecompassbowlComment != null ? Modal.SparecompassbowlComment : "";
            command.Parameters.Add("@CompassobservationbookComment", SqlDbType.NVarChar).Value = Modal.CompassobservationbookComment != null ? Modal.CompassobservationbookComment : "";
            command.Parameters.Add("@GyrocompassComment", SqlDbType.NVarChar).Value = Modal.GyrocompassComment != null ? Modal.GyrocompassComment : "";
            command.Parameters.Add("@RudderindicatorComment", SqlDbType.NVarChar).Value = Modal.RudderindicatorComment != null ? Modal.RudderindicatorComment : "";
            command.Parameters.Add("@SpeedlogComment", SqlDbType.NVarChar).Value = Modal.SpeedlogComment != null ? Modal.SpeedlogComment : "";
            command.Parameters.Add("@NavigationComment", SqlDbType.NVarChar).Value = Modal.NavigationComment != null ? Modal.NavigationComment : "";
            command.Parameters.Add("@SignalflagsComment", SqlDbType.NVarChar).Value = Modal.SignalflagsComment != null ? Modal.SignalflagsComment : "";
            command.Parameters.Add("@RPMComment", SqlDbType.NVarChar).Value = Modal.RPMComment != null ? Modal.RPMComment : "";
            command.Parameters.Add("@BasicmanoeuvringdataComment", SqlDbType.NVarChar).Value = Modal.BasicmanoeuvringdataComment != null ? Modal.BasicmanoeuvringdataComment : "";
            command.Parameters.Add("@MasterstandingordersComment", SqlDbType.NVarChar).Value = Modal.MasterstandingordersComment != null ? Modal.MasterstandingordersComment : "";
            command.Parameters.Add("@MasternightordersbookComment", SqlDbType.NVarChar).Value = Modal.MasternightordersbookComment != null ? Modal.MasternightordersbookComment : "";
            command.Parameters.Add("@SextantComment", SqlDbType.NVarChar).Value = Modal.SextantComment != null ? Modal.SextantComment : "";
            command.Parameters.Add("@AzimuthmirrorComment", SqlDbType.NVarChar).Value = Modal.AzimuthmirrorComment != null ? Modal.AzimuthmirrorComment : "";
            command.Parameters.Add("@BridgepostersComment", SqlDbType.NVarChar).Value = Modal.BridgepostersComment != null ? Modal.BridgepostersComment : "";
            command.Parameters.Add("@ReviewofplannedComment", SqlDbType.NVarChar).Value = Modal.ReviewofplannedComment != null ? Modal.ReviewofplannedComment : "";
            command.Parameters.Add("@BridgebellbookComment", SqlDbType.NVarChar).Value = Modal.BridgebellbookComment != null ? Modal.BridgebellbookComment : "";
            command.Parameters.Add("@BridgenavigationalComment", SqlDbType.NVarChar).Value = Modal.BridgenavigationalComment != null ? Modal.BridgenavigationalComment : "";
            command.Parameters.Add("@SecurityEquipmentComment", SqlDbType.NVarChar).Value = Modal.SecurityEquipmentComment != null ? Modal.SecurityEquipmentComment : "";
            command.Parameters.Add("@NavigationPost", SqlDbType.NVarChar).Value = Modal.NavigationPost != null ? Modal.NavigationPost : "";

            command.Parameters.Add("@GeneralComment", SqlDbType.NVarChar).Value = Modal.GeneralComment != null ? Modal.GeneralComment : "";
            command.Parameters.Add("@MedicinestorageComment", SqlDbType.NVarChar).Value = Modal.MedicinestorageComment != null ? Modal.MedicinestorageComment : "";
            command.Parameters.Add("@MedicinechestcertificateComment", SqlDbType.NVarChar).Value = Modal.MedicinechestcertificateComment != null ? Modal.MedicinechestcertificateComment : "";
            command.Parameters.Add("@InventoryStoresComment", SqlDbType.NVarChar).Value = Modal.InventoryStoresComment != null ? Modal.InventoryStoresComment : "";
            command.Parameters.Add("@OxygencylindersComment", SqlDbType.NVarChar).Value = Modal.OxygencylindersComment != null ? Modal.OxygencylindersComment : "";
            command.Parameters.Add("@StretcherComment", SqlDbType.NVarChar).Value = Modal.StretcherComment != null ? Modal.StretcherComment : "";
            command.Parameters.Add("@SalivaComment", SqlDbType.NVarChar).Value = Modal.SalivaComment != null ? Modal.SalivaComment : "";
            command.Parameters.Add("@AlcoholComment", SqlDbType.NVarChar).Value = Modal.AlcoholComment != null ? Modal.AlcoholComment : "";
            command.Parameters.Add("@HospitalComment", SqlDbType.NVarChar).Value = Modal.HospitalComment != null ? Modal.HospitalComment : "";

            command.Parameters.Add("@GeneralGalleyComment", SqlDbType.NVarChar).Value = Modal.GeneralGalleyComment != null ? Modal.GeneralGalleyComment : "";
            command.Parameters.Add("@HygieneComment", SqlDbType.NVarChar).Value = Modal.HygieneComment != null ? Modal.HygieneComment : "";
            command.Parameters.Add("@FoodstorageComment", SqlDbType.NVarChar).Value = Modal.FoodstorageComment != null ? Modal.FoodstorageComment : "";
            command.Parameters.Add("@FoodlabellingComment", SqlDbType.NVarChar).Value = Modal.FoodlabellingComment != null ? Modal.FoodlabellingComment : "";
            command.Parameters.Add("@GalleyriskassessmentComment", SqlDbType.NVarChar).Value = Modal.GalleyriskassessmentComment != null ? Modal.GalleyriskassessmentComment : "";
            command.Parameters.Add("@FridgetemperatureComment", SqlDbType.NVarChar).Value = Modal.FridgetemperatureComment != null ? Modal.FridgetemperatureComment : "";
            command.Parameters.Add("@FoodandProvisionsComment", SqlDbType.NVarChar).Value = Modal.FoodandProvisionsComment != null ? Modal.FoodandProvisionsComment : "";
            command.Parameters.Add("@GalleyComment", SqlDbType.NVarChar).Value = Modal.GalleyComment != null ? Modal.GalleyComment : "";

            command.Parameters.Add("@ConditionComment", SqlDbType.NVarChar).Value = Modal.ConditionComment != null ? Modal.ConditionComment : "";
            command.Parameters.Add("@PaintworkComment", SqlDbType.NVarChar).Value = Modal.PaintworkComment != null ? Modal.PaintworkComment : "";
            command.Parameters.Add("@LightingComment", SqlDbType.NVarChar).Value = Modal.LightingComment != null ? Modal.LightingComment : "";
            command.Parameters.Add("@PlatesComment", SqlDbType.NVarChar).Value = Modal.PlatesComment != null ? Modal.PlatesComment : "";
            command.Parameters.Add("@BilgesComment", SqlDbType.NVarChar).Value = Modal.BilgesComment != null ? Modal.BilgesComment : "";
            command.Parameters.Add("@PipelinesandvalvesComment", SqlDbType.NVarChar).Value = Modal.PipelinesandvalvesComment != null ? Modal.PipelinesandvalvesComment : "";
            command.Parameters.Add("@LeakageComment", SqlDbType.NVarChar).Value = Modal.LeakageComment != null ? Modal.LeakageComment : "";
            command.Parameters.Add("@EquipmentComment", SqlDbType.NVarChar).Value = Modal.EquipmentComment != null ? Modal.EquipmentComment : "";
            command.Parameters.Add("@OilywaterseparatorComment", SqlDbType.NVarChar).Value = Modal.OilywaterseparatorComment != null ? Modal.OilywaterseparatorComment : "";
            command.Parameters.Add("@FueloiltransferplanComment", SqlDbType.NVarChar).Value = Modal.FueloiltransferplanComment != null ? Modal.FueloiltransferplanComment : "";
            command.Parameters.Add("@SteeringgearComment", SqlDbType.NVarChar).Value = Modal.SteeringgearComment != null ? Modal.SteeringgearComment : "";
            command.Parameters.Add("@WorkshopandequipmentComment", SqlDbType.NVarChar).Value = Modal.WorkshopandequipmentComment != null ? Modal.WorkshopandequipmentComment : "";
            command.Parameters.Add("@SoundingpipesComment", SqlDbType.NVarChar).Value = Modal.SoundingpipesComment != null ? Modal.SoundingpipesComment : "";
            command.Parameters.Add("@EnginecontrolComment", SqlDbType.NVarChar).Value = Modal.EnginecontrolComment != null ? Modal.EnginecontrolComment : "";
            command.Parameters.Add("@ChiefEngineernightordersbookComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineernightordersbookComment != null ? Modal.ChiefEngineernightordersbookComment : "";
            command.Parameters.Add("@ChiefEngineerstandingordersComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineerstandingordersComment != null ? Modal.ChiefEngineerstandingordersComment : "";
            command.Parameters.Add("@PreUMSComment", SqlDbType.NVarChar).Value = Modal.PreUMSComment != null ? Modal.PreUMSComment : "";
            command.Parameters.Add("@EnginebellbookComment", SqlDbType.NVarChar).Value = Modal.EnginebellbookComment != null ? Modal.EnginebellbookComment : "";
            command.Parameters.Add("@LockoutComment", SqlDbType.NVarChar).Value = Modal.LockoutComment != null ? Modal.LockoutComment : "";
            command.Parameters.Add("@EngineRoomComment", SqlDbType.NVarChar).Value = Modal.EngineRoomComment != null ? Modal.EngineRoomComment : "";
            command.Parameters.Add("@CleanlinessandhygieneComment", SqlDbType.NVarChar).Value = Modal.CleanlinessandhygieneComment != null ? Modal.CleanlinessandhygieneComment : "";
            command.Parameters.Add("@ConditionComment5M", SqlDbType.NVarChar).Value = Modal.ConditionComment5M != null ? Modal.ConditionComment5M : "";
            command.Parameters.Add("@PaintworkComment5M", SqlDbType.NVarChar).Value = Modal.PaintworkComment5M != null ? Modal.PaintworkComment5M : "";
            command.Parameters.Add("@SignalmastandstaysComment", SqlDbType.NVarChar).Value = Modal.SignalmastandstaysComment != null ? Modal.SignalmastandstaysComment : "";
            command.Parameters.Add("@MonkeyislandComment", SqlDbType.NVarChar).Value = Modal.MonkeyislandComment != null ? Modal.MonkeyislandComment : "";
            command.Parameters.Add("@FireDampersComment", SqlDbType.NVarChar).Value = Modal.FireDampersComment != null ? Modal.FireDampersComment : "";
            command.Parameters.Add("@RailsBulwarksComment", SqlDbType.NVarChar).Value = Modal.RailsBulwarksComment != null ? Modal.RailsBulwarksComment : "";
            command.Parameters.Add("@WatertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WatertightdoorsComment != null ? Modal.WatertightdoorsComment : "";
            command.Parameters.Add("@VentilatorsComment", SqlDbType.NVarChar).Value = Modal.VentilatorsComment != null ? Modal.VentilatorsComment : "";
            command.Parameters.Add("@WinchesComment", SqlDbType.NVarChar).Value = Modal.WinchesComment != null ? Modal.WinchesComment : "";
            command.Parameters.Add("@FairleadsComment", SqlDbType.NVarChar).Value = Modal.FairleadsComment != null ? Modal.FairleadsComment : "";
            command.Parameters.Add("@MooringLinesComment", SqlDbType.NVarChar).Value = Modal.MooringLinesComment != null ? Modal.MooringLinesComment : "";
            command.Parameters.Add("@EmergencyShutOffsComment", SqlDbType.NVarChar).Value = Modal.EmergencyShutOffsComment != null ? Modal.EmergencyShutOffsComment : "";
            command.Parameters.Add("@RadioaerialsComment", SqlDbType.NVarChar).Value = Modal.RadioaerialsComment != null ? Modal.RadioaerialsComment : "";
            command.Parameters.Add("@SOPEPlockerComment", SqlDbType.NVarChar).Value = Modal.SOPEPlockerComment != null ? Modal.SOPEPlockerComment : "";
            command.Parameters.Add("@ChemicallockerComment", SqlDbType.NVarChar).Value = Modal.ChemicallockerComment != null ? Modal.ChemicallockerComment : "";
            command.Parameters.Add("@AntislippaintComment", SqlDbType.NVarChar).Value = Modal.AntislippaintComment != null ? Modal.AntislippaintComment : "";
            command.Parameters.Add("@SuperstructureComment", SqlDbType.NVarChar).Value = Modal.SuperstructureComment != null ? Modal.SuperstructureComment : "";
            command.Parameters.Add("@CabinsComment", SqlDbType.NVarChar).Value = Modal.CabinsComment != null ? Modal.CabinsComment : "";
            command.Parameters.Add("@OfficesComment", SqlDbType.NVarChar).Value = Modal.OfficesComment != null ? Modal.OfficesComment : "";
            command.Parameters.Add("@MessroomsComment", SqlDbType.NVarChar).Value = Modal.MessroomsComment != null ? Modal.MessroomsComment : "";
            command.Parameters.Add("@ToiletsComment", SqlDbType.NVarChar).Value = Modal.ToiletsComment != null ? Modal.ToiletsComment : "";
            command.Parameters.Add("@LaundryroomComment", SqlDbType.NVarChar).Value = Modal.LaundryroomComment != null ? Modal.LaundryroomComment : "";
            command.Parameters.Add("@ChangingroomComment", SqlDbType.NVarChar).Value = Modal.ChangingroomComment != null ? Modal.ChangingroomComment : "";
            command.Parameters.Add("@OtherComment", SqlDbType.NVarChar).Value = Modal.OtherComment != null ? Modal.OtherComment : "";
            command.Parameters.Add("@ConditionComment5N", SqlDbType.NVarChar).Value = Modal.ConditionComment5N != null ? Modal.ConditionComment5N : "";
            command.Parameters.Add("@SelfclosingfiredoorsComment", SqlDbType.NVarChar).Value = Modal.SelfclosingfiredoorsComment != null ? Modal.SelfclosingfiredoorsComment : "";
            command.Parameters.Add("@StairwellsComment", SqlDbType.NVarChar).Value = Modal.StairwellsComment != null ? Modal.StairwellsComment : "";
            command.Parameters.Add("@SuperstructureInternalComment", SqlDbType.NVarChar).Value = Modal.SuperstructureInternalComment != null ? Modal.SuperstructureInternalComment : "";
            command.Parameters.Add("@PortablegangwayComment", SqlDbType.NVarChar).Value = Modal.PortablegangwayComment != null ? Modal.PortablegangwayComment : "";
            command.Parameters.Add("@SafetynetComment", SqlDbType.NVarChar).Value = Modal.SafetynetComment != null ? Modal.SafetynetComment : "";
            command.Parameters.Add("@AccommodationLadderComment", SqlDbType.NVarChar).Value = Modal.AccommodationLadderComment != null ? Modal.AccommodationLadderComment : "";
            command.Parameters.Add("@SafeaccessprovidedComment", SqlDbType.NVarChar).Value = Modal.SafeaccessprovidedComment != null ? Modal.SafeaccessprovidedComment : "";
            command.Parameters.Add("@PilotladdersComment", SqlDbType.NVarChar).Value = Modal.PilotladdersComment != null ? Modal.PilotladdersComment : "";
            command.Parameters.Add("@BoardingEquipmentComment", SqlDbType.NVarChar).Value = Modal.BoardingEquipmentComment != null ? Modal.BoardingEquipmentComment : "";
            command.Parameters.Add("@CleanlinessComment", SqlDbType.NVarChar).Value = Modal.CleanlinessComment != null ? Modal.CleanlinessComment : "";
            command.Parameters.Add("@PaintworkComment5P", SqlDbType.NVarChar).Value = Modal.PaintworkComment5P != null ? Modal.PaintworkComment5P : "";
            command.Parameters.Add("@ShipsiderailsComment", SqlDbType.NVarChar).Value = Modal.ShipsiderailsComment != null ? Modal.ShipsiderailsComment : "";
            command.Parameters.Add("@WeathertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WeathertightdoorsComment != null ? Modal.WeathertightdoorsComment : "";
            command.Parameters.Add("@FirehydrantsComment", SqlDbType.NVarChar).Value = Modal.FirehydrantsComment != null ? Modal.FirehydrantsComment : "";
            command.Parameters.Add("@VentilatorsComment5P", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5P != null ? Modal.VentilatorsComment5P : "";
            command.Parameters.Add("@ManholecoversComment", SqlDbType.NVarChar).Value = Modal.ManholecoversComment != null ? Modal.ManholecoversComment : "";
            command.Parameters.Add("@MainDeckAreaComment", SqlDbType.NVarChar).Value = Modal.MainDeckAreaComment != null ? Modal.MainDeckAreaComment : "";
            command.Parameters.Add("@ConditionComment5Q", SqlDbType.NVarChar).Value = Modal.ConditionComment5Q != null ? Modal.ConditionComment5Q : "";
            command.Parameters.Add("@PaintworkComment5Q", SqlDbType.NVarChar).Value = Modal.PaintworkComment5Q != null ? Modal.PaintworkComment5Q : "";
            command.Parameters.Add("@MechanicaldamageComment", SqlDbType.NVarChar).Value = Modal.MechanicaldamageComment != null ? Modal.MechanicaldamageComment : "";
            command.Parameters.Add("@AccessladdersComment", SqlDbType.NVarChar).Value = Modal.AccessladdersComment != null ? Modal.AccessladdersComment : "";
            command.Parameters.Add("@ManholecoversComment5Q", SqlDbType.NVarChar).Value = Modal.ManholecoversComment5Q != null ? Modal.ManholecoversComment5Q : "";
            command.Parameters.Add("@HoldbilgeComment", SqlDbType.NVarChar).Value = Modal.HoldbilgeComment != null ? Modal.HoldbilgeComment : "";
            command.Parameters.Add("@AccessdoorsComment", SqlDbType.NVarChar).Value = Modal.AccessdoorsComment != null ? Modal.AccessdoorsComment : "";
            command.Parameters.Add("@ConditionHatchCoversComment", SqlDbType.NVarChar).Value = Modal.ConditionHatchCoversComment != null ? Modal.ConditionHatchCoversComment : "";
            command.Parameters.Add("@PaintworkHatchCoversComment", SqlDbType.NVarChar).Value = Modal.PaintworkHatchCoversComment != null ? Modal.PaintworkHatchCoversComment : "";
            command.Parameters.Add("@RubbersealsComment", SqlDbType.NVarChar).Value = Modal.RubbersealsComment != null ? Modal.RubbersealsComment : "";
            command.Parameters.Add("@SignsofhatchesComment", SqlDbType.NVarChar).Value = Modal.SignsofhatchesComment != null ? Modal.SignsofhatchesComment : "";
            command.Parameters.Add("@SealingtapeComment", SqlDbType.NVarChar).Value = Modal.SealingtapeComment != null ? Modal.SealingtapeComment : "";
            command.Parameters.Add("@ConditionofhydraulicsComment", SqlDbType.NVarChar).Value = Modal.ConditionofhydraulicsComment != null ? Modal.ConditionofhydraulicsComment : "";
            command.Parameters.Add("@PortablebulkheadsComment", SqlDbType.NVarChar).Value = Modal.PortablebulkheadsComment != null ? Modal.PortablebulkheadsComment : "";
            command.Parameters.Add("@TweendecksComment", SqlDbType.NVarChar).Value = Modal.TweendecksComment != null ? Modal.TweendecksComment : "";
            command.Parameters.Add("@HatchcoamingComment", SqlDbType.NVarChar).Value = Modal.HatchcoamingComment != null ? Modal.HatchcoamingComment : "";
            command.Parameters.Add("@ConditionCargoCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionCargoCranesComment != null ? Modal.ConditionCargoCranesComment : "";
            command.Parameters.Add("@GantrycranealarmComment", SqlDbType.NVarChar).Value = Modal.GantrycranealarmComment != null ? Modal.GantrycranealarmComment : "";
            command.Parameters.Add("@GantryconditionComment", SqlDbType.NVarChar).Value = Modal.GantryconditionComment != null ? Modal.GantryconditionComment : "";
            command.Parameters.Add("@CargoHoldsComment", SqlDbType.NVarChar).Value = Modal.CargoHoldsComment != null ? Modal.CargoHoldsComment : "";
            command.Parameters.Add("@CleanlinessComment5R", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5R != null ? Modal.CleanlinessComment5R : "";
            command.Parameters.Add("@PaintworkComment5R", SqlDbType.NVarChar).Value = Modal.PaintworkComment5R != null ? Modal.PaintworkComment5R : "";
            command.Parameters.Add("@TriphazardsComment", SqlDbType.NVarChar).Value = Modal.TriphazardsComment != null ? Modal.TriphazardsComment : "";
            command.Parameters.Add("@WindlassComment", SqlDbType.NVarChar).Value = Modal.WindlassComment != null ? Modal.WindlassComment : "";
            command.Parameters.Add("@CablesComment", SqlDbType.NVarChar).Value = Modal.CablesComment != null ? Modal.CablesComment : "";
            command.Parameters.Add("@WinchesComment5R", SqlDbType.NVarChar).Value = Modal.WinchesComment5R != null ? Modal.WinchesComment5R : "";
            command.Parameters.Add("@FairleadsComment5R", SqlDbType.NVarChar).Value = Modal.FairleadsComment5R != null ? Modal.FairleadsComment5R : "";
            command.Parameters.Add("@MooringComment", SqlDbType.NVarChar).Value = Modal.MooringComment != null ? Modal.MooringComment : "";
            command.Parameters.Add("@HatchToforecastlespaceComment", SqlDbType.NVarChar).Value = Modal.HatchToforecastlespaceComment != null ? Modal.HatchToforecastlespaceComment : "";
            command.Parameters.Add("@VentilatorsComment5R", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5R != null ? Modal.VentilatorsComment5R : "";
            command.Parameters.Add("@BellComment", SqlDbType.NVarChar).Value = Modal.BellComment != null ? Modal.BellComment : "";
            command.Parameters.Add("@ForemastComment", SqlDbType.NVarChar).Value = Modal.ForemastComment != null ? Modal.ForemastComment : "";
            command.Parameters.Add("@FireComment", SqlDbType.NVarChar).Value = Modal.FireComment != null ? Modal.FireComment : "";
            command.Parameters.Add("@RailsComment", SqlDbType.NVarChar).Value = Modal.RailsComment != null ? Modal.RailsComment : "";
            command.Parameters.Add("@AntislippaintComment5R", SqlDbType.NVarChar).Value = Modal.AntislippaintComment5R != null ? Modal.AntislippaintComment5R : "";


            command.Parameters.Add("@SnapBackZoneComment", SqlDbType.NVarChar).Value = Modal.SnapBackZoneComment != null ? Modal.SnapBackZoneComment : "";
            command.Parameters.Add("@ConditionGantryCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionGantryCranesComment != null ? Modal.ConditionGantryCranesComment : "";
            command.Parameters.Add("@MedicalLogBookComment", SqlDbType.NVarChar).Value = Modal.MedicalLogBookComment != null ? Modal.MedicalLogBookComment : "";
            command.Parameters.Add("@DrugsNarcoticsComment", SqlDbType.NVarChar).Value = Modal.DrugsNarcoticsComment != null ? Modal.DrugsNarcoticsComment : "";
            command.Parameters.Add("@DefibrillatorComment", SqlDbType.NVarChar).Value = Modal.DefibrillatorComment != null ? Modal.DefibrillatorComment : "";
            command.Parameters.Add("@RPWaterHandbook", SqlDbType.NVarChar).Value = Modal.RPWaterHandbook != null ? Modal.RPWaterHandbook : "";
            command.Parameters.Add("@BioRPWH", SqlDbType.NVarChar).Value = Modal.BioRPWH != null ? Modal.BioRPWH : "";
            command.Parameters.Add("@PRE", SqlDbType.NVarChar).Value = Modal.PRE != null ? Modal.PRE : "";
            command.Parameters.Add("@NoiseVibrationFile", SqlDbType.NVarChar).Value = Modal.NoiseVibrationFile != null ? Modal.NoiseVibrationFile : "";
            command.Parameters.Add("@BioMPR", SqlDbType.NVarChar).Value = Modal.BioMPR != null ? Modal.BioMPR : "";
            command.Parameters.Add("@AsbestosPlan", SqlDbType.NVarChar).Value = Modal.AsbestosPlan != null ? Modal.AsbestosPlan : "";
            command.Parameters.Add("@ShipPublicAddrComment", SqlDbType.NVarChar).Value = Modal.ShipPublicAddrComment != null ? Modal.ShipPublicAddrComment : "";
            command.Parameters.Add("@BridgewindowswiperssprayComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswiperssprayComment != null ? Modal.BridgewindowswiperssprayComment : "";
            command.Parameters.Add("@BridgewindowswipersComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswipersComment != null ? Modal.BridgewindowswipersComment : "";
            command.Parameters.Add("@DaylightSignalsComment", SqlDbType.NVarChar).Value = Modal.DaylightSignalsComment != null ? Modal.DaylightSignalsComment : "";
            command.Parameters.Add("@LiferaftDavitComment", SqlDbType.NVarChar).Value = Modal.LiferaftDavitComment != null ? Modal.LiferaftDavitComment : "";
            command.Parameters.Add("@CylindersLockerComment", SqlDbType.NVarChar).Value = Modal.CylindersLockerComment != null ? Modal.CylindersLockerComment : "";
            command.Parameters.Add("@ADPPublicationsComment", SqlDbType.NVarChar).Value = Modal.ADPPublicationsComment != null ? Modal.ADPPublicationsComment : "";
            command.Parameters.Add("@SnapBackZone5NComment", SqlDbType.NVarChar).Value = Modal.SnapBackZone5NComment != null ? Modal.SnapBackZone5NComment : "";

            command.Parameters.Add("@ForecastleComment", SqlDbType.NVarChar).Value = Modal.ForecastleComment != null ? Modal.ForecastleComment : "";
            command.Parameters.Add("@CleanlinessComment5S", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5S != null ? Modal.CleanlinessComment5S : "";
            command.Parameters.Add("@PaintworkComment5S", SqlDbType.NVarChar).Value = Modal.PaintworkComment5S != null ? Modal.PaintworkComment5S : "";
            command.Parameters.Add("@ForepeakComment", SqlDbType.NVarChar).Value = Modal.ForepeakComment != null ? Modal.ForepeakComment : "";
            command.Parameters.Add("@ChainlockerComment", SqlDbType.NVarChar).Value = Modal.ChainlockerComment != null ? Modal.ChainlockerComment : "";
            command.Parameters.Add("@LightingComment5S", SqlDbType.NVarChar).Value = Modal.LightingComment5S != null ? Modal.LightingComment5S : "";
            command.Parameters.Add("@AccesssafetychainComment", SqlDbType.NVarChar).Value = Modal.AccesssafetychainComment != null ? Modal.AccesssafetychainComment : "";
            command.Parameters.Add("@EmergencyfirepumpComment", SqlDbType.NVarChar).Value = Modal.EmergencyfirepumpComment != null ? Modal.EmergencyfirepumpComment : "";
            command.Parameters.Add("@BowthrusterandroomComment", SqlDbType.NVarChar).Value = Modal.BowthrusterandroomComment != null ? Modal.BowthrusterandroomComment : "";
            command.Parameters.Add("@SparemooringlinesComment", SqlDbType.NVarChar).Value = Modal.SparemooringlinesComment != null ? Modal.SparemooringlinesComment : "";
            command.Parameters.Add("@PaintlockerComment", SqlDbType.NVarChar).Value = Modal.PaintlockerComment != null ? Modal.PaintlockerComment : "";
            command.Parameters.Add("@ForecastleSpaceComment", SqlDbType.NVarChar).Value = Modal.ForecastleSpaceComment != null ? Modal.ForecastleSpaceComment : "";
            command.Parameters.Add("@BoottopComment", SqlDbType.NVarChar).Value = Modal.BoottopComment != null ? Modal.BoottopComment : "";
            command.Parameters.Add("@TopsidesComment", SqlDbType.NVarChar).Value = Modal.TopsidesComment != null ? Modal.TopsidesComment : "";
            command.Parameters.Add("@AntifoulingComment", SqlDbType.NVarChar).Value = Modal.AntifoulingComment != null ? Modal.AntifoulingComment : "";
            command.Parameters.Add("@DraftandplimsollComment", SqlDbType.NVarChar).Value = Modal.DraftandplimsollComment != null ? Modal.DraftandplimsollComment : "";
            command.Parameters.Add("@FoulingComment", SqlDbType.NVarChar).Value = Modal.FoulingComment != null ? Modal.FoulingComment : "";
            command.Parameters.Add("@MechanicalComment", SqlDbType.NVarChar).Value = Modal.MechanicalComment != null ? Modal.MechanicalComment : "";
            command.Parameters.Add("@HullComment", SqlDbType.NVarChar).Value = Modal.HullComment != null ? Modal.HullComment : "";
            command.Parameters.Add("@SummaryComment", SqlDbType.NVarChar).Value = Modal.SummaryComment != null ? Modal.SummaryComment : "";
            command.Parameters.Add("@IsSynced", SqlDbType.NVarChar).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? Utility.ToDateTimeUtcNow() : (object)Modal.UpdatedDate; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft == null ? DBNull.Value : (object)Modal.SavedAsDraft;

            //RDBJ 10/20/2021
            command.Parameters.Add("@IsGeneralSectionComplete", SqlDbType.Bit).Value = Modal.IsGeneralSectionComplete == null ? false : true;
            command.Parameters.Add("@IsManningSectionComplete", SqlDbType.Bit).Value = Modal.IsManningSectionComplete == null ? false : true;
            command.Parameters.Add("@IsShipCertificationSectionComplete", SqlDbType.Bit).Value = Modal.IsShipCertificationSectionComplete == null ? false : true;
            command.Parameters.Add("@IsRecordKeepingSectionComplete", SqlDbType.Bit).Value = Modal.IsRecordKeepingSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSafetyEquipmentSectionComplete", SqlDbType.Bit).Value = Modal.IsSafetyEquipmentSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSecuritySectionComplete", SqlDbType.Bit).Value = Modal.IsSecuritySectionComplete == null ? false : true;
            command.Parameters.Add("@IsBridgeSectionComplete", SqlDbType.Bit).Value = Modal.IsBridgeSectionComplete == null ? false : true;
            command.Parameters.Add("@IsMedicalSectionComplete", SqlDbType.Bit).Value = Modal.IsMedicalSectionComplete == null ? false : true;
            command.Parameters.Add("@IsGalleySectionComplete", SqlDbType.Bit).Value = Modal.IsGalleySectionComplete == null ? false : true;
            command.Parameters.Add("@IsEngineRoomSectionComplete", SqlDbType.Bit).Value = Modal.IsEngineRoomSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSuperstructureSectionComplete", SqlDbType.Bit).Value = Modal.IsSuperstructureSectionComplete == null ? false : true;
            command.Parameters.Add("@IsDeckSectionComplete", SqlDbType.Bit).Value = Modal.IsDeckSectionComplete == null ? false : true;
            command.Parameters.Add("@IsHoldsAndCoverSectionComplete", SqlDbType.Bit).Value = Modal.IsHoldsAndCoverSectionComplete == null ? false : true;
            command.Parameters.Add("@IsForeCastleSectionComplete", SqlDbType.Bit).Value = Modal.IsForeCastleSectionComplete == null ? false : true;
            command.Parameters.Add("@IsHullSectionComplete", SqlDbType.Bit).Value = Modal.IsHullSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSummarySectionComplete", SqlDbType.Bit).Value = Modal.IsSummarySectionComplete == null ? false : true;
            command.Parameters.Add("@IsDeficienciesSectionComplete", SqlDbType.Bit).Value = Modal.IsDeficienciesSectionComplete == null ? false : true;
            command.Parameters.Add("@IsPhotographsSectionComplete", SqlDbType.Bit).Value = Modal.IsPhotographsSectionComplete == null ? false : true;
            //End RDBJ 10/20/2021
        }
        #endregion

        //RDBJ 09/28/2021
        #region Insert/Update [GIRSafeManningRequirements], [GIRCrewDocuments], [GIRRestandWorkHours], [GIRPhotographs]
        public void GIRSafeManningRequirements_Save(Guid UniqueFormID, List<GlRSafeManningRequirements> GIRSafeManningRequirements)
        {
            bool res = DeleteRecords(AppStatic.GlRSafeManningRequirements, "UniqueFormID", Convert.ToString(UniqueFormID));
            if (res == true)
            {
                try
                {
                    if (GIRSafeManningRequirements != null && GIRSafeManningRequirements.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                        SqlConnection connection = new SqlConnection(connetionString);
                        string InsertQuery = GIRSafeManningRequirements_InsertQuery();
                        connection.Open();
                        foreach (var item in GIRSafeManningRequirements)
                        {
                            item.UniqueFormID = UniqueFormID;
                            item.GIRFormID = 0;
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            GIRSafeManningRequirements_CMD(item, ref command);
                            object resultObj = command.ExecuteScalar();
                            long databaseID = 0;
                            if (resultObj != null)
                            {
                                long.TryParse(resultObj.ToString(), out databaseID);
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Add Local DB in GIRSafeManningRequirements_Save table Error : " + ex.Message.ToString());
                    res = false;
                }
            }
        }
        public string GIRSafeManningRequirements_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRSafeManningRequirements 
                                  (UniqueFormID,GIRFormID,Rank,RequiredbySMD,OnBoard,CreatedDate,UpdatedDate,Ship)
                                  OUTPUT INSERTED.SafeManningRequirementsID
                                  VALUES (@UniqueFormID,@GIRFormID,@Rank,@RequiredbySMD,@OnBoard,@CreatedDate,@UpdatedDate,@Ship)";
            return InsertQuery;
        }
        public void GIRSafeManningRequirements_CMD(GlRSafeManningRequirements Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@Rank", SqlDbType.NVarChar).Value = Modal.Rank == null ? string.Empty : Modal.Rank;
            command.Parameters.Add("@RequiredbySMD", SqlDbType.Bit).Value = Modal.OnBoard == null ? false : Modal.OnBoard;
            command.Parameters.Add("@OnBoard", SqlDbType.Bit).Value = Modal.OnBoard == null ? false : Modal.OnBoard;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate;// DateTime.Now;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate; // DateTime.Now;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
        }

        public void GIRCrewDocuments_Save(Guid UniqueFormID, List<GlRCrewDocuments> GIRCrewDocuments)
        {
            bool res = DeleteRecords(AppStatic.GlRCrewDocuments, "UniqueFormID", Convert.ToString(UniqueFormID));
            if (res == true)
            {
                try
                {
                    if (GIRCrewDocuments != null && GIRCrewDocuments.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                        SqlConnection connection = new SqlConnection(connetionString);
                        string InsertQuery = GIRCrewDocuments_InsertQuery();
                        connection.Open();
                        foreach (var item in GIRCrewDocuments)
                        {
                            item.UniqueFormID = UniqueFormID;
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            GIRCrewDocuments_CMD(item, ref command);
                            object resultObj = command.ExecuteScalar();
                            long databaseID = 0;
                            if (resultObj != null)
                            {
                                long.TryParse(resultObj.ToString(), out databaseID);
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Add Local DB in GIRCrewDocuments_Save table Error : " + ex.Message.ToString());
                    res = false;
                }
            }
        }
        public string GIRCrewDocuments_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRCrewDocuments 
                                  (UniqueFormID,GIRFormID,CrewmemberName,CertificationDetail,CreatedDate,UpdatedDate,Ship)
                                  OUTPUT INSERTED.CrewDocumentsID
                                  VALUES (@UniqueFormID,@GIRFormID,@CrewmemberName,@CertificationDetail,@CreatedDate,@UpdatedDate,@Ship)";
            return InsertQuery;
        }
        public void GIRCrewDocuments_CMD(GlRCrewDocuments Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@CrewmemberName", SqlDbType.NVarChar).Value = Modal.CrewmemberName == null ? string.Empty : Modal.CrewmemberName;
            command.Parameters.Add("@CertificationDetail", SqlDbType.NVarChar).Value = Modal.CertificationDetail == null ? string.Empty : Modal.CertificationDetail;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate; //DateTime.Now;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate; //DateTime.Now;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
        }

        public void GIRRestandWorkHours_Save(Guid UniqueFormID, List<GIRRestandWorkHours> GIRRestandWorkHours)
        {
            bool res = DeleteRecords(AppStatic.GIRRestandWorkHours, "UniqueFormID", Convert.ToString(UniqueFormID));
            if (res == true)
            {
                try
                {
                    if (GIRRestandWorkHours != null && GIRRestandWorkHours.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                        SqlConnection connection = new SqlConnection(connetionString);
                        string InsertQuery = GIRRestandWorkHours_InsertQuery();
                        connection.Open();
                        foreach (var item in GIRRestandWorkHours)
                        {
                            item.GIRFormID = 0;
                            item.UniqueFormID = UniqueFormID;
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            GIRRestandWorkHours_CMD(item, ref command);
                            object resultObj = command.ExecuteScalar();
                            long databaseID = 0;
                            if (resultObj != null)
                            {
                                long.TryParse(resultObj.ToString(), out databaseID);
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Add Local DB in GIRCrewDocuments_Save table Error : " + ex.Message.ToString());
                    res = false;
                }
            }
        }
        public string GIRRestandWorkHours_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRRestandWorkHours 
                                  (UniqueFormID,GIRFormID,CrewmemberName,RestAndWorkDetail,CreatedDate,UpdatedDate,Ship)
                                  OUTPUT INSERTED.RestandWorkHoursID
                                  VALUES (@UniqueFormID,@GIRFormID,@CrewmemberName,@RestAndWorkDetail,@CreatedDate,@UpdatedDate,@Ship)";
            return InsertQuery;
        }
        public void GIRRestandWorkHours_CMD(GIRRestandWorkHours Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@CrewmemberName", SqlDbType.NVarChar).Value = Modal.CrewmemberName == null ? string.Empty : Modal.CrewmemberName;
            command.Parameters.Add("@RestAndWorkDetail", SqlDbType.NVarChar).Value = Modal.RestAndWorkDetail == null ? string.Empty : Modal.RestAndWorkDetail;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate; //DateTime.Now;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate; //DateTime.Now;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
        }

        public void GIRPhotos_Save(Guid UniqueFormID, List<GIRPhotographs> GIRPhotographs)
        {
            bool res = DeleteRecords(AppStatic.GIRPhotographs, "UniqueFormID", Convert.ToString(UniqueFormID));
            try
            {
                if (res == true)
                {
                    //SaveGIRPhotographsDataInLocalDB(GIRPhotographs, UniqueFormID);
                    if (GIRPhotographs != null && GIRPhotographs.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        ServerConnectModal dbConnModal = Utility.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            string InsertQuery = GIRPhotographsSave_InsertQuery();
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            connection.Open();
                            foreach (var item in GIRPhotographs)
                            {
                                item.GIRFormID = 0;
                                item.UniqueFormID = UniqueFormID;
                                item.CreatedDate = Utility.ToDateTimeUtcNow();
                                item.UpdatedDate = Utility.ToDateTimeUtcNow();

                                // JSL 11/13/2022
                                if (item.ImagePath.StartsWith("data:"))
                                {
                                    Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                                    dicFileMetaData["UniqueFormID"] = Convert.ToString(UniqueFormID);
                                    dicFileMetaData["ReportType"] = "GI";
                                    dicFileMetaData["FileName"] = item.FileName;
                                    dicFileMetaData["Base64FileData"] = item.ImagePath;

                                    item.ImagePath = Utility.ConvertBase64IntoFile(dicFileMetaData, true);
                                }
                                // End JSL 11/13/2022

                                SqlCommand command = new SqlCommand(InsertQuery, connection);
                                GIRPhotographsSave_CMD(item, ref command);
                                command.ExecuteScalar();
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRPhotos_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRPhotographsSave_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRPhotographs 
                                  (GIRFormID,ImagePath,ImageCaption,CreatedDate,UpdatedDate,Ship,FileName,UniqueFormID)
                                  VALUES (@GIRFormID,@ImagePath,@ImageCaption,@CreatedDate,@UpdatedDate,@Ship,@FileName,@UniqueFormID)";
            return InsertQuery;
        }
        public void GIRPhotographsSave_CMD(GIRPhotographs Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@ImagePath", SqlDbType.NVarChar).Value = Modal.ImagePath;
            command.Parameters.Add("@ImageCaption", SqlDbType.NVarChar).Value = Modal.ImageCaption == null ? "" : (object)Modal.ImageCaption;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
        }
        public bool SaveGIRPhotographsDataInLocalDB(List<GIRPhotographs> GIRPhotographs, Guid UniqueFormID)
        {
            bool res = false;
            try
            {
                if (GIRPhotographs != null && GIRPhotographs.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in GIRPhotographs)
                    {
                        //item.GIRFormID = GIRFormID;
                        item.UniqueFormID = UniqueFormID;
                        item.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        item.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    DataTable dt = Utility.ToDataTable(GIRPhotographs);
                    using (SqlConnection connection = new SqlConnection(connetionString))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                        bulkCopy.DestinationTableName = AppStatic.GIRPhotographs;
                        connection.Open();
                        bulkCopy.WriteToServer(dt);
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SaveGIRPhotographsDataInLocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        #endregion
        //End RDBJ 09/28/2021

        #region GIRDeficiencies Insert/Update
        public string GIRDefRecordsExist(Guid? DeficienciesUniqueID) //RDBJ O9/21/2021 Removed another parameter
        {
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                SqlConnection connection = new SqlConnection(connetionString);
                string res = string.Empty; //RDBJ 09/21/2021
                connection.Open();
                DataTable dt1 = new DataTable();
                //string isExistRecord = "select DeficienciesID from " + AppStatic.GIRDeficiencies + " where UniqueFormID = '" + UniqueFormID + "' AND [No] = " + No + ""; //RDBJ 09/21/2021 Commented
                string isExistRecord = "select DeficienciesUniqueID from " + AppStatic.GIRDeficiencies + " where DeficienciesUniqueID = '" + DeficienciesUniqueID + "'"; //RDBJ 09/21/2021
                SqlDataAdapter sqlAdp1 = new SqlDataAdapter(isExistRecord, connection);
                sqlAdp1.Fill(dt1);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0][0] == DBNull.Value)
                        res = string.Empty; //RDBJ 09/21/2021 Changed 0
                    else
                        res = Convert.ToString(dt1.Rows[0][0]); //RDBJ 09/20/2021 Convert toString from int
                }
                connection.Close(); //RDBJ 10/27/2021
                return res;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeficienciesNumber :" + ex.Message);
                return string.Empty;
            }
        }
        public void GIRDeficiencies_Save(Guid UniqueFormID, List<GIRDeficiencies> GIRDeficiencies
            , bool FormIsDraft = false  // JSL 05/23/2022
            )
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in GIRDeficiencies)
                    {
                        string resDeficienciesUniqueID = GIRDefRecordsExist(item.DeficienciesUniqueID); //RDBJ 09/21/2021 Removed second parameter and changed DeficienciesUniqueID from UniqueFormID
                        try
                        {
                            if (!string.IsNullOrEmpty(resDeficienciesUniqueID)) //RDBJ 09/21/2021 changed condition (res > 0)
                            {
                                string UpdateQuery = GIRDeficiencies_UpdateQuery();
                                SqlCommand command = new SqlCommand(UpdateQuery, connection);
                                item.UniqueFormID = UniqueFormID;
                                GIRDeficiencies_UpdateCMD(item, ref command);
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                            else
                            {
                                item.CreatedDate = item.CreatedDate ?? Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                string InsertQuery = GIRDeficiencies_InsertQuery();
                                SqlCommand command = new SqlCommand(InsertQuery, connection);
                                item.UniqueFormID = UniqueFormID;
                                GIRDeficiencies_CMD(item, ref command);
                                connection.Open();
                                command.ExecuteScalar();
                                connection.Close();
                            }

                            // JSL 11/13/2022
                            Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                            dicFileMetaData["UniqueFormID"] = Convert.ToString(UniqueFormID);
                            dicFileMetaData["ReportType"] = item.ReportType;
                            dicFileMetaData["DetailUniqueId"] = Convert.ToString(item.DeficienciesUniqueID);
                            // End JSL 11/13/2022

                            GIRDeficienciesFiles_Save(item.DeficienciesUniqueID, item.GIRDeficienciesFile   //RDBJ 09/21/2021
                                , dicFileMetaData   // JSL 11/13/2022
                                );
                            GIRDeficienciesComments_Save(item.GIRDeficienciesComments   //RDBJ 09/21/2021
                                , dicFileMetaData   // JSL 11/13/2022
                                );
                            GIRDeficienciesResolution_Save(item.GIRDeficienciesResolution   //RDBJ 09/21/2021
                                , dicFileMetaData   // JSL 11/13/2022
                                );
                            GIRDeficienciesInitialActions_Save(item.GIRDeficienciesInitialActions   //RDBJ 09/21/2021
                                , dicFileMetaData   // JSL 11/13/2022
                                , FormIsDraft   // JSL 05/23/2022
                                );

                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("Failed to add Deficiencies : " + ex.Message.ToString());

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRDeficiencies_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRDeficiencies_InsertQuery()
        {
            // RDBJ 03/01/2022 Added DueDate // RDBJ 12/18/2021 Added AssignTo // RDBJ 11/01/2021 Added Priority
            string InsertQuery = @"INSERT INTO dbo.GIRDeficiencies 
                                  (DeficienciesUniqueID,UniqueFormID,No,DateRaised,Deficiency,DateClosed,CreatedDate,UpdatedDate,Ship,IsClose,ReportType,FileName,StorePath,SIRNo,ItemNo,Section,isDelete,Priority,AssignTo,DueDate)
                                  OUTPUT INSERTED.DeficienciesID
                                  VALUES (@DeficienciesUniqueID,@UniqueFormID,@No,@DateRaised,@Deficiency,@DateClosed,@CreatedDate,@UpdatedDate,@Ship,@IsClose,@ReportType,@FileName,@StorePath,@SIRNo,@ItemNo,@Section,@isDelete,@Priority,@AssignTo,@DueDate)"; //RDBJ 09/22/2021 Added DeficienciesUniqueID
            return InsertQuery;
        }
        public void GIRDeficiencies_CMD(GIRDeficiencies Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID; //RDBJ 09/22/2021
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority; // RDBJ 11/01/2021
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            //command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@No", SqlDbType.Int).Value = Modal.No;
            command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Modal.DateRaised == null ? DBNull.Value : (object)Modal.DateRaised; //DateTime.Now;
            command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency == null ? DBNull.Value : (object)Modal.Deficiency;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //DateTime.Now;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate; //DateTime.Now;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
            command.Parameters.Add("@IsClose", SqlDbType.Bit).Value = Modal.IsClose == null ? false : (object)Modal.IsClose;
            command.Parameters.Add("@ReportType", SqlDbType.NVarChar).Value = Modal.ReportType;//"GI"
            //command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.Ship;
            //command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.Ship
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@SIRNo", SqlDbType.NVarChar).Value = 0;
            command.Parameters.Add("@ItemNo", SqlDbType.NVarChar).Value = Modal.ItemNo == null ? DBNull.Value : (object)Modal.ItemNo;
            command.Parameters.Add("@Section", SqlDbType.NVarChar).Value = Modal.Section == null ? DBNull.Value : (object)Modal.Section;
            command.Parameters.Add("@AssignTo", SqlDbType.UniqueIdentifier).Value = Modal.AssignTo == null ? DBNull.Value : (object)Modal.AssignTo; // RDBJ 12/18/2021
            command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = Modal.DueDate == null ? DBNull.Value : (object)Modal.DueDate;    // RDBJ 03/01/2022
        }
        public string GIRDeficiencies_UpdateQuery()
        {
            // RDBJ 03/28/2022 Added Ship // RDBJ 03/01/2022 Added DueDate // RDBJ 12/18/2021 Added AssignTo // RDBJ 11/01/2021 Added Priority //RDBJ 10/27/2021 set Section = @Section //RDBJ 10/25/2021 set No = @No and DeficienciesUniqueID = @DeficienciesUniqueID
            string UpdateQuery = @"UPDATE dbo.GIRDeficiencies SET No = @No, Section = @Section, IsClose = @IsClose, Priority = @Priority, AssignTo = @AssignTo, Ship = @Ship,
                                DateRaised = @DateRaised, Deficiency = @Deficiency, DateClosed = @DateClosed, UpdatedDate = @UpdatedDate, isDelete = @isDelete, DueDate = @DueDate
                                WHERE DeficienciesUniqueID = @DeficienciesUniqueID";
            return UpdateQuery;
        }
        public void GIRDeficiencies_UpdateCMD(GIRDeficiencies Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID; //RDBJ 10/25/2021 set DeficienciesUniqueID rather than UniqueFormID
            command.Parameters.Add("@No", SqlDbType.Int).Value = Modal.No;
            command.Parameters.Add("@Section", SqlDbType.NVarChar).Value = Modal.Section == null ? DBNull.Value : (object)Modal.Section; //RDBJ 10/27/2021 
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship; // RDBJ 03/28/2022
            command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Modal.DateRaised == null ? DBNull.Value : (object)Modal.DateRaised;  //DateTime.Now;
            command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency == null ? DBNull.Value : (object)Modal.Deficiency;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate;  //DateTime.Now;
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete;
            command.Parameters.Add("@IsClose", SqlDbType.Bit).Value = Modal.IsClose == null ? false : (object)Modal.IsClose; //RDBJ 10/27/2021
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority; //RDBJ 11/01/2021
            command.Parameters.Add("@AssignTo", SqlDbType.UniqueIdentifier).Value = Modal.AssignTo == null ? DBNull.Value : (object)Modal.AssignTo; // RDBJ 12/18/2021
            command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = Modal.DueDate == null ? DBNull.Value : (object)Modal.DueDate;    // RDBJ 03/01/2022
        }
        #endregion

        //RDBJ 09/21/2021
        public bool DeleteRecords(string tablename, string columnname, string RecID)
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
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
        //End RDBJ 09/21/2021

        #region GIRDeficienciesFiles
        public void GIRDeficienciesFiles_Save(Guid? DefID, List<GIRDeficienciesFile> DefFiles
            , Dictionary<string, string> dicFileMetaData   // JSL 11/13/2022
            )
        {
            bool res = DeleteRecords(AppStatic.GIRDeficienciesFile, "DeficienciesUniqueID", Convert.ToString(DefID)); //RDBJ 09/21/2021

            try
            {
                if (res)
                {
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    SqlConnection connection = new SqlConnection(connetionString);

                    if (DefFiles != null && DefFiles.Count > 0) //RDBJ 09/21/2021 Removed && DefID > 0
                    {
                        string InsertQuery = GIRDeficienciesFiles_InsertQuery();
                        connection.Open();
                        foreach (var item in DefFiles)
                        {
                            // JSL 11/13/2022
                            if (item.StorePath.StartsWith("data:"))
                            {
                                dicFileMetaData["FileName"] = item.FileName;
                                dicFileMetaData["Base64FileData"] = item.StorePath;

                                item.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                            }
                            // End JSL 11/13/2022

                            //item.DeficienciesID = DefID; //RDBJ 09/21/2021 Commented
                            item.DeficienciesUniqueID = DefID; //RDBJ 09/21/2021
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            GIRDeficienciesFiles_CMD(item, ref command);
                            object resultObj = command.ExecuteScalar();
                            long databaseID = 0;
                            if (resultObj != null)
                            {
                                long.TryParse(resultObj.ToString(), out databaseID);
                            }
                        }
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRDeficienciesFiles_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRDeficienciesFiles_InsertQuery()
        {
            // JSL 06/07/2022 added DeficienciesFileUniqueID
            string InsertQuery = @"INSERT INTO dbo.GIRDeficienciesFiles 
                                  (DeficienciesUniqueID,DeficienciesID,FileName,StorePath, DeficienciesFileUniqueID)
                                  OUTPUT INSERTED.GIRDeficienciesFileID
                                  VALUES (@DeficienciesUniqueID,@DeficienciesID,@FileName,@StorePath, @DeficienciesFileUniqueID)"; //RDBJ 09/21/2021 Added DeficienciesUniqueID
            return InsertQuery;
        }
        public void GIRDeficienciesFiles_CMD(GIRDeficienciesFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID; //RDBJ 09/21/2021
            command.Parameters.Add("@DeficienciesFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesFileUniqueID; // JSL 06/07/2022
            command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID; // RDBJ 01/15/2022 set 0 to avoid null error
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
        }
        #endregion

        #region Check Comments, InitialAction, Resolution and Files Exist or not
        public string GIRDefCommentIntActResolutionRecordsExist(string tableName, string columnName, Guid UniqueID)
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
                LogHelper.writelog("GIRDefCommentIntActResolutionRecordsExist :" + ex.Message);
                return string.Empty;
            }
        }
        public string GIRDefCommentIntActResolutionFileRecordsExist(string tableName, string columnName, Guid FileUniqueID)
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
                LogHelper.writelog("GIRDefCommentIntActResolutionFileRecordsExist :" + ex.Message);
                return string.Empty;
            }
        }
        #endregion

        #region GIRDeficiencies Comments and Files
        public void GIRDeficienciesComments_Save(List<GIRDeficienciesNote> modalDefNotes //RDBJ 09/21/2021 Removed Guid? DefID,
            , Dictionary<string, string> dicFileMetaData   // JSL 11/13/2022
            )
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (modalDefNotes != null && modalDefNotes.Count > 0) //RDBJ 09/21/2021 Removed && DefID > 0
                {
                    foreach (var itemComments in modalDefNotes)
                    {
                        string noteUID = GIRDefCommentIntActResolutionRecordsExist(AppStatic.DeficienciesNote, "NoteUniqueID", itemComments.NoteUniqueID); //RDBJ 09/20/2021 update here
                        if (string.IsNullOrEmpty(noteUID))
                        {
                            connection.Open();
                            string InsertQuery = GIRDeficienciesNote_InsertQuery();
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            GIRDeficienciesNote_CMD(itemComments, ref command);
                            command.ExecuteScalar();
                            connection.Close(); //RDBJ 10/27/2021
                            if (itemComments.GIRDeficienciesCommentFile != null && itemComments.GIRDeficienciesCommentFile.Count > 0)
                            {
                                foreach (var itemCommentsFile in itemComments.GIRDeficienciesCommentFile)
                                {
                                    string commentFileUID = GIRDefCommentIntActResolutionFileRecordsExist(AppStatic.GIRDeficienciesCommentFile, "CommentFileUniqueID", itemCommentsFile.CommentFileUniqueID); //RDBJ 09/20/2021 update here
                                    if (string.IsNullOrEmpty(commentFileUID))
                                    {
                                        // JSL 11/13/2022
                                        if (itemCommentsFile.StorePath.StartsWith("data:"))
                                        {
                                            dicFileMetaData["FileName"] = itemCommentsFile.FileName;
                                            dicFileMetaData["Base64FileData"] = itemCommentsFile.StorePath;

                                            // JSL 01/08/2023
                                            dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeComment;
                                            dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentsFile.NoteUniqueID);
                                            // End JSL 01/08/2023

                                            itemCommentsFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                        }
                                        // End JSL 11/13/2022
                                        connection.Open();
                                        string InsertComFileQuery = GIRDeficienciesCommentFile_InsertQuery();
                                        command = new SqlCommand(InsertComFileQuery, connection);
                                        GIRDeficienciesCommentFile_CMD(itemCommentsFile, ref command);
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
                LogHelper.writelog("Add Local DB in GIRDeficienciesComments_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRDeficienciesNote_InsertQuery()
        {
            //RDBJ 10/25/2021 Added isNew Column
            string InsertQuery = @"INSERT INTO dbo.GIRDeficienciesNotes 
                                  (NoteUniqueID,GIRFormID,DeficienciesUniqueID,UserName,Comment,CreatedDate,ModifyDate,isNew)
                                  OUTPUT INSERTED.NoteID
                                  VALUES (@NoteUniqueID,@GIRFormID,@DeficienciesUniqueID,@UserName,@Comment,@CreatedDate,@ModifyDate,@isNew)";
            return InsertQuery;
        }
        public void GIRDeficienciesNote_CMD(GIRDeficienciesNote Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@NoteUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NoteUniqueID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID == null ? DBNull.Value : (object)Modal.GIRFormID;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = Modal.UserName == null ? string.Empty : Modal.UserName;
            command.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = Modal.Comment == null ? DBNull.Value : (object)Modal.Comment;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //DateTime.Now;
            command.Parameters.Add("@ModifyDate", SqlDbType.DateTime).Value = Modal.ModifyDate == null ? DBNull.Value : (object)Modal.ModifyDate; //DateTime.Now;
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 1; //RDBJ 10/25/2021
        }
        public string GIRDeficienciesCommentFile_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesCommentFile]
                                ([CommentFileUniqueID], [NoteUniqueID], [NoteID],[FileName],[StorePath],[IsUpload])
                                  OUTPUT INSERTED.GIRCommentFileID
                                  VALUES (@CommentFileUniqueID,@NoteUniqueID,@NoteID,@FileName,@StorePath,@IsUpload)";
            return InsertQuery;
        }
        public void GIRDeficienciesCommentFile_CMD(GIRDeficienciesCommentFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@CommentFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentFileUniqueID;
            command.Parameters.Add("@NoteUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NoteUniqueID;
            command.Parameters.Add("@NoteID", SqlDbType.BigInt).Value = Modal.NoteID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
            command.Parameters.Add("@IsUpload", SqlDbType.NVarChar).Value = Modal.IsUpload == null ? DBNull.Value : (object)Modal.IsUpload;
        }
        #endregion

        #region GIRDeficiencies Initial Actions and File
        public void GIRDeficienciesInitialActions_Save(List<GIRDeficienciesInitialActions> modalDefIntActs //RDBJ 09/21/2021 Removed Guid? DefID,
            , Dictionary<string, string> dicFileMetaData   // JSL 11/13/2022
            , bool FormIsDraft = false  // JSL 05/23/2022
            )
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (modalDefIntActs != null && modalDefIntActs.Count > 0) //RDBJ 09/21/2021 Removed && DefID > 0
                {
                    foreach (var itemIntAct in modalDefIntActs)
                    {
                        string intActionUID = GIRDefCommentIntActResolutionRecordsExist(AppStatic.GIRDeficienciesInitialActions, "IniActUniqueID", itemIntAct.IniActUniqueID); //RDBJ 09/20/2021 update here
                        if (string.IsNullOrEmpty(intActionUID))
                        {
                            connection.Open();
                            string InsertQuery = GIRDeficienciesInitialActions_InsertQuery();
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            GIRDeficienciesInitialActions_CMD(itemIntAct, ref command
                                , FormIsDraft   // JSL 05/23/2022
                                );
                            object resultObj = command.ExecuteScalar();
                            connection.Close();
                            long databaseID = 0;
                            if (resultObj != null)
                            {
                                long.TryParse(resultObj.ToString(), out databaseID);
                            }

                            if (itemIntAct.GIRDeficienciesInitialActionsFiles != null && itemIntAct.GIRDeficienciesInitialActionsFiles.Count > 0)
                            {
                                foreach (var itemIntActFile in itemIntAct.GIRDeficienciesInitialActionsFiles)
                                {
                                    string IntActFileUID = GIRDefCommentIntActResolutionFileRecordsExist(AppStatic.GIRDeficienciesInitialActionsFile, "IniActFileUniqueID", itemIntActFile.IniActFileUniqueID); //RDBJ 09/20/2021 update here
                                    if (string.IsNullOrEmpty(IntActFileUID))
                                    {
                                        // JSL 11/13/2022
                                        if (itemIntActFile.StorePath.StartsWith("data:"))
                                        {
                                            dicFileMetaData["FileName"] = itemIntActFile.FileName;
                                            dicFileMetaData["Base64FileData"] = itemIntActFile.StorePath;

                                            // JSL 01/08/2023
                                            dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeInitialAction;
                                            dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemIntActFile.IniActUniqueID);
                                            // End JSL 01/08/2023

                                            itemIntActFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                        }
                                        // End JSL 11/13/2022

                                        connection.Open();
                                        string InsertComFileQuery = GIRDeficienciesInitialActionsFile_InsertQuery();
                                        command = new SqlCommand(InsertComFileQuery, connection);
                                        GIRDeficienciesInitialActionsFile_CMD(itemIntActFile, ref command);
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
                LogHelper.writelog("Add Local DB in GIRDeficienciesInitialActions_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRDeficienciesInitialActions_InsertQuery()
        {
            //RDBJ 10/25/2021 Added isNew Column
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesInitialActions]([IniActUniqueID],[DeficienciesUniqueID],[GIRFormID],[Name],[Description],[CreatedDate],[isNew])
                                  OUTPUT INSERTED.GIRInitialID
                                  VALUES (@IniActUniqueID,@DeficienciesUniqueID,@GIRFormID,@Name,@Description,@CreatedDate,@isNew)";
            return InsertQuery;
        }
        public void GIRDeficienciesInitialActions_CMD(GIRDeficienciesInitialActions Modal
            , ref SqlCommand command
            , bool FormIsDraft = false  // JSL 05/23/2022
            )
        {
            command.Parameters.Add("@IniActUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.IniActUniqueID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID == null ? DBNull.Value : (object)Modal.GIRFormID;
            command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Modal.Name == null ? string.Empty : Modal.Name;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Modal.Description == null ? DBNull.Value : (object)Modal.Description;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //DateTime.Now; //RDBJ 10/25/2021 Set Actual Reponsed date rather than now

            // JSL 05/23/2022
            if (FormIsDraft)
                command.Parameters.Add("@isNew", SqlDbType.Int).Value = 0; //RDBJ 10/25/2021
            else
                command.Parameters.Add("@isNew", SqlDbType.Int).Value = 1;
            // End JSL 05/23/2022
        }
        public string GIRDeficienciesInitialActionsFile_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesInitialActionsFile]
                                ([IniActUniqueID],[IniActFileUniqueID],[GIRInitialID],[FileName],[StorePath],[IsUpload])
                                  OUTPUT INSERTED.GIRFileID
                                  VALUES (@IniActUniqueID,@IniActFileUniqueID,@GIRInitialID,@FileName,@StorePath,@IsUpload)";
            return InsertQuery;
        }
        public void GIRDeficienciesInitialActionsFile_CMD(GIRDeficienciesInitialActionsFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@IniActUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.IniActUniqueID;
            command.Parameters.Add("@IniActFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.IniActFileUniqueID;
            command.Parameters.Add("@GIRInitialID", SqlDbType.BigInt).Value = Modal.GIRInitialID == null ? DBNull.Value : (object)Modal.GIRInitialID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
            command.Parameters.Add("@IsUpload", SqlDbType.NVarChar).Value = Modal.IsUpload == null ? DBNull.Value : (object)Modal.IsUpload;
        }
        #endregion

        #region GIRDeficiencies Resolution and File
        public void GIRDeficienciesResolution_Save(List<GIRDeficienciesResolution> modalDefResolutions //RDBJ 09/21/2021 Removed Guid? DefID,
            , Dictionary<string, string> dicFileMetaData   // JSL 11/13/2022
            )
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);
            try
            {
                if (modalDefResolutions != null && modalDefResolutions.Count > 0) //RDBJ 09/21/2021 Removed && DefID > 0
                {
                    foreach (var itemResolution in modalDefResolutions)
                    {
                        string intResolutionUID = GIRDefCommentIntActResolutionRecordsExist(AppStatic.GIRDeficienciesResolution, "ResolutionUniqueID", itemResolution.ResolutionUniqueID); //RDBJ 09/20/2021 update here
                        if (string.IsNullOrEmpty(intResolutionUID))
                        {
                            connection.Open();
                            string InsertQuery = GIRDeficienciesResolution_InsertQuery();
                            SqlCommand command = new SqlCommand(InsertQuery, connection);
                            GIRDeficienciesResolution_CMD(itemResolution, ref command);
                            object resultObj = command.ExecuteScalar();
                            connection.Close();
                            long databaseID = 0;
                            if (resultObj != null)
                            {
                                long.TryParse(resultObj.ToString(), out databaseID);
                            }

                            if (itemResolution.GIRDeficienciesResolutionFiles != null && itemResolution.GIRDeficienciesResolutionFiles.Count > 0)
                            {
                                foreach (var itemRes in itemResolution.GIRDeficienciesResolutionFiles)
                                {
                                    string IntActFileUID = GIRDefCommentIntActResolutionFileRecordsExist(AppStatic.GIRDeficienciesResolutionFile, "ResolutionFileUniqueID", itemRes.ResolutionFileUniqueID); //RDBJ 09/20/2021 update here
                                    if (string.IsNullOrEmpty(IntActFileUID))
                                    {
                                        // JSL 11/13/2022
                                        if (itemRes.StorePath.StartsWith("data:"))
                                        {
                                            dicFileMetaData["FileName"] = itemRes.FileName;
                                            dicFileMetaData["Base64FileData"] = itemRes.StorePath;

                                            // JSL 01/08/2023
                                            dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeResolution;
                                            dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemRes.ResolutionUniqueID);
                                            // End JSL 01/08/2023

                                            itemRes.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                        }
                                        // End JSL 11/13/2022
                                        connection.Open();
                                        string InsertComFileQuery = GIRDeficienciesResolutionFile_InsertQuery();
                                        command = new SqlCommand(InsertComFileQuery, connection);
                                        GIRDeficienciesResolutionFile_CMD(itemRes, ref command);
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
                LogHelper.writelog("Add Local DB in GIRDeficienciesResolution_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRDeficienciesResolution_InsertQuery()
        {
            //RDBJ 10/25/2021 Added isNew Column
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesResolution]([ResolutionUniqueID],[DeficienciesUniqueID],[GIRFormID],[Name],[Resolution],[CreatedDate],[isNew])
                                  OUTPUT INSERTED.GIRResolutionID
                                  VALUES (@ResolutionUniqueID,@DeficienciesUniqueID,@GIRFormID,@Name,@Resolution,@CreatedDate,@isNew)";
            return InsertQuery;
        }
        public void GIRDeficienciesResolution_CMD(GIRDeficienciesResolution Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ResolutionUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionUniqueID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID == null ? DBNull.Value : (object)Modal.GIRFormID;
            command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Modal.Name == null ? string.Empty : Modal.Name;
            command.Parameters.Add("@Resolution", SqlDbType.NVarChar).Value = Modal.Resolution == null ? DBNull.Value : (object)Modal.Resolution;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate; //DateTime.Now; //RDBJ 10/25/2021 Set Actual Reponsed date rather than now
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 1; //RDBJ 10/25/2021
        }
        public string GIRDeficienciesResolutionFile_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesResolutionFile]
                                ([ResolutionUniqueID],[ResolutionFileUniqueID],[GIRResolutionID],[FileName],[StorePath],[IsUpload])
                                  OUTPUT INSERTED.GIRFileID
                                  VALUES (@ResolutionUniqueID,@ResolutionFileUniqueID,@GIRResolutionID,@FileName,@StorePath,@IsUpload)";
            return InsertQuery;
        }
        public void GIRDeficienciesResolutionFile_CMD(GIRDeficienciesResolutionFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ResolutionUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionUniqueID;
            command.Parameters.Add("@ResolutionFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionFileUniqueID;
            command.Parameters.Add("@GIRResolutionID", SqlDbType.BigInt).Value = Modal.GIRResolutionID == null ? DBNull.Value : (object)Modal.GIRResolutionID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID == null ? DBNull.Value : (object)Modal.DeficienciesID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
            command.Parameters.Add("@IsUpload", SqlDbType.NVarChar).Value = Modal.IsUpload == null ? DBNull.Value : (object)Modal.IsUpload;
        }
        #endregion

        public void UpdateCloudGIRFormsStatus(List<string> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds);
                APIHelper _helper = new APIHelper();
                _helper.sendSynchGIRListUFID(SuccessIds); // RDBJ 01/19/2022 set with List
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCloudGIRFormsStatus : " + ex.Message);
            }
        }
        #endregion

        #region GIRSynch Based on Latest Version

        //RDBJ 09/22/2021
        public void GETGIRLatestData(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<GeneralInspectionReport> CloudSyncList = GetGIRFormsSyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine  // JSL 11/12/2022
                );
            List<GeneralInspectionReport> LocalSyncList = GetGIRFormsSyncedDataFromLocal();

            if (CloudSyncList != null && CloudSyncList.Count > 0)
            {
                LogHelper.writelog("GETGIRLatestData : SyncList count for GIR Data is about " + CloudSyncList.Count + "");
                foreach (var CloudGir in CloudSyncList)
                {
                    try
                    {
                        GeneralInspectionReport LocalGIR = LocalSyncList.Where(x => x.UniqueFormID == CloudGir.UniqueFormID && x.Ship == CloudGir.Ship).FirstOrDefault();
                        if (LocalGIR != null)
                        {
                            InsertUpdateGIRProcess(CloudGir, LocalGIR);
                        }
                        else
                        {
                            GeneralInspectionReport girFormData = GetGIRSyncedDataFromCloud(Convert.ToString(CloudGir.UniqueFormID));
                            SaveGIRDataInLocalDB(girFormData);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("GETGIRLatestData : GIR Data Synced Not done. Error : " + ex.Message);
                    }
                }
            }
            else
            {
                LogHelper.writelog("GETGIRLatestData : GIR Data Synced already done.");
            }
        }
        //End RDBJ 09/22/2021

        //RDBJ 09/22/2021
        public List<GeneralInspectionReport> GetGIRFormsSyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false   // JSL 11/12/2022
            )
        {
            List<GeneralInspectionReport> SyncListFromCloud = new List<GeneralInspectionReport>();
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
                SyncListFromCloud = _helper.GetGIRFormsSyncedDataFromCloud(strShipCode);    // JSL 11/12/2022 added strShipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRFormsUnsyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncListFromCloud;
        }
        //End RDBJ 09/22/2021

        //RDBJ 09/22/2021
        public List<GeneralInspectionReport> GetGIRFormsSyncedDataFromLocal()
        {
            List<GeneralInspectionReport> SyncList = new List<GeneralInspectionReport>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID, FormVersion, Ship, IsSynced FROM " + AppStatic.GeneralInspectionReport + " WHERE [UniqueFormID] IS NOT NULL", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<GeneralInspectionReport>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRFormsSyncedDataFromLocal " + ex.Message);
            }
            return SyncList;
        }
        //End RDBJ 09/22/2021

        //RDBJ 09/22/2021
        public void InsertUpdateGIRProcess(GeneralInspectionReport CloudGIR, GeneralInspectionReport LocalGIR)
        {
            GeneralInspectionReport girFormData = new GeneralInspectionReport();
            try
            {
                if (CloudGIR.FormVersion > LocalGIR.FormVersion)
                {
                    girFormData = GetGIRSyncedDataFromCloud(Convert.ToString(CloudGIR.UniqueFormID));

                    // RDBJ 12/22/2021 later use
                    /*
                    girFormData.GIRDeficiencies = GetGIRDeficienciesFromCloud(Convert.ToString(CloudGIR.UniqueFormID));
                    foreach (var item in girFormData.GIRDeficiencies)
                    {
                        item.GIRDeficienciesComments = GetGIRDeficienciesCommentsFromCloud(Convert.ToString(item.DeficienciesUniqueID));
                        item.GIRDeficienciesInitialActions = GetGIRDeficienciesInitialActionsFromCloud(Convert.ToString(item.DeficienciesUniqueID));
                        item.GIRDeficienciesResolution = GetGIRDeficienciesResolutionsFromCloud(Convert.ToString(item.DeficienciesUniqueID));
                    }
                    */
                    SaveGIRDataInLocalDB(girFormData);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertUpdateGIRProcess : " + ex.Message);
            }
        }
        //End RDBJ 09/22/2021

        //RDBJ 09/22/2021
        public GeneralInspectionReport GetGIRSyncedDataFromCloud(string UniqueFormID)
        {
            GeneralInspectionReport SyncGIRFromCloud = new GeneralInspectionReport();
            try
            {
                APIHelper _helper = new APIHelper();
                SyncGIRFromCloud = _helper.GetGIRSyncedDataFromCloud(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRFormsUnsyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncGIRFromCloud;
        }
        //End RDBJ 09/22/2021


        // RDBJ 12/22/2021 Below Code later use
        #region // RDBJ 12/22/2021 Below Code later use
        // RDBJ 12/22/2021
        public List<GIRDeficiencies> GetGIRDeficienciesFromCloud(string UniqueFormID)
        {
            List<GIRDeficiencies> GIRDefFromCloud = new List<GIRDeficiencies>();
            try
            {
                APIHelper _helper = new APIHelper();
                GIRDefFromCloud = _helper.GetGIRDeficienciesFromCloud(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesFromCloud Error : " + ex.Message.ToString());
            }
            return GIRDefFromCloud;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        public List<GIRDeficienciesNote> GetGIRDeficienciesCommentsFromCloud(string DeficienciesUniqueID)
        {
            List<GIRDeficienciesNote> GIRDefCommentsFromCloud = new List<GIRDeficienciesNote>();
            try
            {
                APIHelper _helper = new APIHelper();
                GIRDefCommentsFromCloud = _helper.GetGIRDeficienciesCommentsFromCloud(DeficienciesUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesCommentsFromCloud Error : " + ex.Message.ToString());
            }
            return GIRDefCommentsFromCloud;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        public List<GIRDeficienciesInitialActions> GetGIRDeficienciesInitialActionsFromCloud(string DeficienciesUniqueID)
        {
            List<GIRDeficienciesInitialActions> GIRDefInitialActionsFromCloud = new List<GIRDeficienciesInitialActions>();
            try
            {
                APIHelper _helper = new APIHelper();
                GIRDefInitialActionsFromCloud = _helper.GetGIRDeficienciesInitialActionsFromCloud(DeficienciesUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesInitialActionsFromCloud Error : " + ex.Message.ToString());
            }
            return GIRDefInitialActionsFromCloud;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        public List<GIRDeficienciesResolution> GetGIRDeficienciesResolutionsFromCloud(string DeficienciesUniqueID)
        {
            List<GIRDeficienciesResolution> GIRDefResolutionFromCloud = new List<GIRDeficienciesResolution>();
            try
            {
                APIHelper _helper = new APIHelper();
                GIRDefResolutionFromCloud = _helper.GetGIRDeficienciesResolutionsFromCloud(DeficienciesUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesResolutionsFromCloud Error : " + ex.Message.ToString());
            }
            return GIRDefResolutionFromCloud;
        }
        // End RDBJ 12/22/2021
        #endregion

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
