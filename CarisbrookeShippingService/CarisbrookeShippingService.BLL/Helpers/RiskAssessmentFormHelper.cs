using CarisbrookeShippingService.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class RiskAssessmentFormHelper
    {
        #region Ship to Server
        public void StartRAFSync()
        {
            List<RAFormModal> UnSyncList = GetRAFormsUnsyncedData();
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("UnSyncList count for RAF Data is about " + UnSyncList.Count + "");
                List<string> SuccessIds = SendRAFormDataToRemote(UnSyncList);   // JSL 11/24/2022 change long to string
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    UpdateLocalRAFormsStatus(SuccessIds);

                    if (SuccessIds.Count == UnSyncList.Count)
                        LogHelper.writelog("RAF Data Sync process done.");
                    if (SuccessIds.Count < UnSyncList.Count)
                        LogHelper.writelog("Some RAF Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("RAF Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("RAF Data Synced already done.");
            }
        }
        public List<RAFormModal> GetRAFormsUnsyncedData()
        {
            List<RAFormModal> SyncList = new List<RAFormModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                        conn.Open();
                    {
                        DataTable dt = new DataTable();
                        //SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT top 1 * FROM " + AppStatic.RiskAssessmentForm + " WHERE ISNULL(IsSynced,0) = 0", conn);    // JSL 06/23/2022 commented this line
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentForm + " WHERE ISNULL(IsSynced,0) = 0 AND [RAFUniqueID] IS NOT NULL", conn); // JSL 11/24/2022 added  // JSL 06/23/2022
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<RiskAssessmentForm> RAFSyncList = dt.ToListof<RiskAssessmentForm>();
                            foreach (var item in RAFSyncList)
                            {
                                try
                                {
                                    RAFormModal Modal = new RAFormModal();

                                    Modal.RiskAssessmentForm = item;

                                    DataTable dtNotes = new DataTable();
                                    //sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormHazard + " WHERE RAFID = " + item.RAFID + "", conn); // JSL 06/23/2022 commented this line
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormHazard + " WHERE RAFUniqueID = '" + item.RAFUniqueID + "'", conn);   // JSL 06/23/2022 
                                    sqlAdp.Fill(dtNotes);
                                    Modal.RiskAssessmentFormHazardList = dtNotes.ToListof<RiskAssessmentFormHazard>();

                                    DataTable dtRAFNotes = new DataTable();
                                    //sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormReviewer + " WHERE RAFID = " + item.RAFID + "", conn);   // JSL 06/23/2022 commented this line
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormReviewer + " WHERE RAFUniqueID = '" + item.RAFUniqueID + "'", conn); // JSL 06/23/2022 
                                    sqlAdp.Fill(dtRAFNotes);
                                    Modal.RiskAssessmentFormReviewerList = dtRAFNotes.ToListof<RiskAssessmentFormReviewer>();

                                    SyncList.Add(Modal);
                                }
                                catch (Exception ex)
                                {
                                    //LogHelper.writelog("RAFormData Error : RAFID : " + item.RAFID + " " + ex.Message);    // JSL 06/23/2022 commented this line
                                    LogHelper.writelog("RAFormData Error : RAFUniqueID : " + item.RAFUniqueID + " " + ex.Message);  // JSL 06/23/2022 
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public List<string> SendRAFormDataToRemote(List<RAFormModal> UnSyncList)  // JSL 11/24/2022 change long to string
        {
            // JSL 11/24/2022 commented
            /*
            List<string> SuccessIds = new List<string>();
            foreach (var item in UnSyncList)
            {
                if (item.RiskAssessmentForm != null && item.RiskAssessmentForm.RAFID > 0)
                {
                    long localRAFID = item.RiskAssessmentForm.RAFID;
                    item.RiskAssessmentForm.RAFID = 0;
                    APIHelper _helper = new APIHelper();
                    APIResponse res = _helper.SubmitRAF(item);
                    if (res != null && res.result == AppStatic.SUCCESS)
                    {
                        SuccessIds.Add(localRAFID);
                    }
                }
            }
            */
            // End JSL 11/24/2022 commented

            // JSL 11/24/2022
            List<string> SuccessIds = new List<string>();
            foreach (var item in UnSyncList)
            {
                if (item.RiskAssessmentForm != null && item.RiskAssessmentForm.RAFUniqueID != null)
                {
                    item.RiskAssessmentForm.RAFID = 0;
                    APIHelper _helper = new APIHelper();
                    APIResponse res = _helper.SubmitRAF(item);
                    if (res != null && res.result == AppStatic.SUCCESS)
                    {
                        SuccessIds.Add(Convert.ToString(item.RiskAssessmentForm.RAFUniqueID));
                    }
                }
            }
            // End JSL 11/24/2022

            return SuccessIds;
        }
        public void UpdateLocalRAFormsStatus(List<string> SuccessIds)   // JSL 11/24/2022 change <long> to <string>
        {
            try
            {
                // JSL 11/24/2022 wrapped in if
                if (SuccessIds != null && SuccessIds.Count > 0)
                {
                    //string IdsStr = string.Join(",", SuccessIds); // JSL 11/24/2022 commented
                    string IdsStr = string.Join(",", SuccessIds.Select(x => string.Format("'{0}'", x)));    // JSL 11/24/2022
                    string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            string Query = "UPDATE " + AppStatic.RiskAssessmentForm + " SET IsSynced = 1 WHERE RAFUniqueID in (" + IdsStr + ")";  // JSL 11/24/2022 change with RAFUniqueID
                            SqlCommand cmd = new SqlCommand(Query, conn);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                
                // End JSL 11/24/2022 wrapped in if
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalRAFormsStatus : " + ex.Message);
            }
        }
        #endregion


        #region Server to Ship
        // JSL 11/26/2022
        public void StartRAFSyncCloudTOLocal(
            string shipCode, bool IsInspectorInThisMachine = false 
            )
        {
            List<RAFormModal> UnSyncList = GetRAFFormsUnsyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine 
                );
            if (UnSyncList != null && UnSyncList.Count > 0)
            {
                LogHelper.writelog("StartRAFSyncCloudTOLocal UnSyncList count for RAF Data is about " + UnSyncList.Count + "");
                List<string> SuccessIds = SendRAFDataToLocal(UnSyncList);
                if (SuccessIds != null && SuccessIds.Count > 0 && SuccessIds.Count == UnSyncList.Count)
                {
                    UpdateCloudRAFFormsStatus(SuccessIds);
                    LogHelper.writelog("StartRAFSyncCloudTOLocal RAF Data Sync process done.");
                }
                else if (SuccessIds != null && SuccessIds.Count < UnSyncList.Count)
                {
                    LogHelper.writelog("StartRAFSyncCloudTOLocal RAF GIR Data Synced and some remaining...");
                }
                else
                {
                    LogHelper.writelog("StartRAFSyncCloudTOLocal RAF Data Synced Not done.");
                }
            }
            else
            {
                LogHelper.writelog("StartRAFSyncCloudTOLocal RAF Data Synced already done.");
            }
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public List<RAFormModal> GetRAFFormsUnsyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false
            )
        {
            List<RAFormModal> UnSyncListFromCloud = new List<RAFormModal>();
            try
            {
                string strShipCode = string.Empty;
                if (!IsInspectorInThisMachine)
                {
                    strShipCode = shipCode;
                }

                APIHelper _helper = new APIHelper();
                UnSyncListFromCloud = _helper.GetRAFGeneralDescription(strShipCode);    
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAFFormsUnsyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return UnSyncListFromCloud;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public List<string> SendRAFDataToLocal(List<RAFormModal> UnSyncList)
        {
            List<string> SuccessIds = new List<string>();
            foreach (var item in UnSyncList)
            {
                string localDBRAFUniqueID = Convert.ToString(item.RiskAssessmentForm.RAFUniqueID);
                item.RiskAssessmentForm.RAFID = 0;
                APIResponse res = SaveRAFDataInLocalDB(item);

                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(localDBRAFUniqueID);
                }
            }
            return SuccessIds;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public APIResponse SaveRAFDataInLocalDB(RAFormModal Modal)
        {
            APIResponse res = new APIResponse();
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            SqlConnection connection = new SqlConnection(connetionString);

            string UniqueFormID = string.Empty;

            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT RAFUniqueID, RAFID, UpdatedDate FROM " + AppStatic.RiskAssessmentForm + " WHERE RAFUniqueID = '" + Modal.RiskAssessmentForm.RAFUniqueID + "'", connection);
                sqlAdp.Fill(dt);

                Modal.RiskAssessmentForm.IsSynced = true;
                UniqueFormID = Convert.ToString(Modal.RiskAssessmentForm.RAFUniqueID);

                if (dt.Rows.Count > 0)
                {
                    long RAFID = Convert.ToInt64(dt.Rows[0]["RAFID"]);
                    DateTime? dbModalUpdatedDateTime = null;
                    if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["UpdatedDate"])))
                    {
                        dbModalUpdatedDateTime = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]);
                    }

                    if ((Modal.RiskAssessmentForm.UpdatedDate > dbModalUpdatedDateTime)
                        || dbModalUpdatedDateTime == null
                        )
                    {
                        UpdateRiskAssessmentData(Modal, RAFID);
                    }
                }
                else
                {
                    InsertRiskAssessmentData(Modal);
                }

                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveRAFDataInLocalDB : " + UniqueFormID + "  Inner Error : " + ex.InnerException.ToString());
                res.result = AppStatic.ERROR;
                res.msg = ex.Message.ToString();
            }
            finally
            {
                IfConnectionOpenThenCloseIt(connection);
            }

            return res;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public void UpdateCloudRAFFormsStatus(List<string> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds);
                APIHelper _helper = new APIHelper();
                _helper.sendSynchRAFListUFID(SuccessIds);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCloudRAFFormsStatus : " + ex.Message);
            }
        }
        // End JSL 11/26/2022
        #endregion


        #region GET RAF Latest

        // JSL 11/26/2022
        public void GETRAFLatestData(
            string shipCode, bool IsInspectorInThisMachine = false
            )
        {
            List<RiskAssessmentForm> CloudSyncList = GetRAFFormsSyncedDataFromCloud(
                shipCode, IsInspectorInThisMachine
                );
            List<RiskAssessmentForm> LocalSyncList = GetRAFFormsSyncedDataFromLocal();

            if (CloudSyncList != null && CloudSyncList.Count > 0)
            {
                LogHelper.writelog("GETRAFLatestData : SyncList count for RAF Data is about " + CloudSyncList.Count + "");
                foreach (var CloudRAF in CloudSyncList)
                {
                    try
                    {
                        RiskAssessmentForm LocalRAF = LocalSyncList.Where(x => x.RAFUniqueID == CloudRAF.RAFUniqueID && x.ShipCode == CloudRAF.ShipCode).FirstOrDefault();
                        if (LocalRAF != null)
                        {
                            InsertUpdateRAFProcess(CloudRAF, LocalRAF);
                        }
                        else
                        {
                            RAFormModal RAFFormData = GetRAFSyncedDataFromCloud(Convert.ToString(CloudRAF.RAFUniqueID));
                            SaveRAFDataInLocalDB(RAFFormData);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("GETRAFLatestData : RAF Data Synced Not done. Error : " + ex.Message);
                    }
                }
            }
            else
            {
                LogHelper.writelog("GETRAFLatestData : RAF Data Synced already done.");
            }
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public List<RiskAssessmentForm> GetRAFFormsSyncedDataFromCloud(
            string shipCode, bool IsInspectorInThisMachine = false  
            )
        {
            List<RiskAssessmentForm> SyncListFromCloud = new List<RiskAssessmentForm>();
            try
            {
                string strShipCode = string.Empty;
                if (!IsInspectorInThisMachine)
                {
                    strShipCode = shipCode;
                }

                APIHelper _helper = new APIHelper();
                SyncListFromCloud = _helper.GetRAFFormsSyncedDataFromCloud(strShipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAFFormsSyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncListFromCloud;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public List<RiskAssessmentForm> GetRAFFormsSyncedDataFromLocal()
        {
            List<RiskAssessmentForm> SyncList = new List<RiskAssessmentForm>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT RAFUniqueID, UpdatedDate, ShipCode, IsSynced FROM " + AppStatic.RiskAssessmentForm, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<RiskAssessmentForm>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAFFormsSyncedDataFromLocal " + ex.Message);
            }
            return SyncList;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public void InsertUpdateRAFProcess(RiskAssessmentForm CloudRAF, RiskAssessmentForm LocalRAF)
        {
            RAFormModal RAFFormData = new RAFormModal();
            try
            {
                if ((CloudRAF.UpdatedDate > LocalRAF.UpdatedDate)
                    || LocalRAF.UpdatedDate == null
                    )
                {
                    RAFFormData = GetRAFSyncedDataFromCloud(Convert.ToString(CloudRAF.RAFUniqueID));
                    SaveRAFDataInLocalDB(RAFFormData);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertUpdateRAFProcess : " + ex.Message);
            }
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public RAFormModal GetRAFSyncedDataFromCloud(string RAFUniqueID)
        {
            RAFormModal SyncRAFFromCloud = new RAFormModal();
            try
            {
                APIHelper _helper = new APIHelper();
                SyncRAFFromCloud = _helper.GetRAFSyncedDataFromCloud(RAFUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAFSyncedDataFromCloud Error : " + ex.Message.ToString());
            }
            return SyncRAFFromCloud;
        }
        // End JSL 11/26/2022

        #endregion

        public List<RiskAssessmentForm> GetAllDocumentRiskassessment()
        {
            List<RiskAssessmentForm> response = new List<RiskAssessmentForm>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " ";
                        SqlDataAdapter da = new SqlDataAdapter(Query, conn);
                        da.Fill(dt);
                        conn.Close();
                        if (dt != null && dt.Rows.Count > 1)
                        {
                            response = dt.ToListof<RiskAssessmentForm>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentRiskassessment Error : " + ex.Message);
            }
            return response;
        }

        public void InsertRiskAssessmentAllData(RAFormModal objRiskAssement)
        {
            try
            {
                // JSL 11/26/2022
                if (objRiskAssement.RiskAssessmentForm.RAFUniqueID == null || objRiskAssement.RiskAssessmentForm.RAFUniqueID == Guid.Empty)
                {
                    objRiskAssement.RiskAssessmentForm.RAFUniqueID = Guid.NewGuid();
                }
                // End JSL 11/26/2022

                var RAFID = InsertRiskAssessmentForm(objRiskAssement.RiskAssessmentForm);
                if (RAFID > 0)
                {
                    InsertOrUpdateRAFHazardsAndReviewers(objRiskAssement, RAFID);  // JSL 11/26/2022 wrapped in function
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertRiskAssessmentAllData : " + ex.Message);
            }
        }

        // JSL 11/26/2022
        public void InsertRiskAssessmentData(RAFormModal objRiskAssement)
        {
            try
            {
                var RAFID = InsertRiskAssessmentForm(objRiskAssement.RiskAssessmentForm);
                if (RAFID > 0)
                {
                    InsertOrUpdateRAFHazardsAndReviewers(objRiskAssement, RAFID);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertRiskAssessmentData : " + ex.Message);
            }
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public void UpdateRiskAssessmentData(RAFormModal objRiskAssement, long RAFID)
        {
            try
            {
                UpdateRiskAssessmentForm(objRiskAssement.RiskAssessmentForm);
                InsertOrUpdateRAFHazardsAndReviewers(objRiskAssement, RAFID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateRiskAssessmentData : " + ex.Message);
            }
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        private long UpdateRiskAssessmentForm(RiskAssessmentForm modal)
        {
            long rafId = 0;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "UPDATE " + AppStatic.RiskAssessmentForm +
                            " SET [ShipName] = @ShipName, [ShipCode] = @ShipCode, [Number] = @Number, [Title] = @Title, [ReviewerName] = @ReviewerName, [ReviewerDate] = @ReviewerDate," +
                            " [ReviewerRank] = @ReviewerRank, [ReviewerLocation] = @ReviewerLocation, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Code] = @Code, [Issue] = @Issue," +
                            " [IssueDate] = @IssueDate, [Amendment] = @Amendment, [AmendmentDate] = @AmendmentDate, [IsConfidential] = @IsConfidential, [IsAmended] = @IsAmended, [IsApplicable] = @IsApplicable, [IsSynced] = @IsSynced" +
                            " [SavedAsDraft] = @SavedAsDraft, [DocumentID] = @DocumentID, [ParentID] = @ParentID, [Type] = @Type, [SectionType] = @SectionType" + // JSL 02/25/2023
                            " WHERE [RAFUniqueID] = @RAFUniqueID";

                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@RAFUniqueID", SqlDbType.UniqueIdentifier).Value = modal.RAFUniqueID ?? (object)DBNull.Value;
                        command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = modal.ShipName ?? (object)DBNull.Value;
                        command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = modal.ShipCode ?? (object)DBNull.Value;
                        command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = modal.Number ?? (object)DBNull.Value;
                        command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = modal.Title ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerName", SqlDbType.NVarChar).Value = modal.ReviewerName ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerDate", SqlDbType.DateTime).Value = modal.ReviewerDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerRank", SqlDbType.NVarChar).Value = modal.ReviewerRank ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerLocation", SqlDbType.NVarChar).Value = modal.ReviewerLocation ?? (object)DBNull.Value;
                        command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = modal.UpdatedBy ?? (object)DBNull.Value;
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = modal.UpdatedDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = modal.Code ?? (object)DBNull.Value;
                        command.Parameters.Add("@Issue", SqlDbType.Int).Value = modal.Issue ?? (object)DBNull.Value;
                        command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = modal.IssueDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@Amendment", SqlDbType.NVarChar).Value = modal.Amendment ?? (object)DBNull.Value;
                        command.Parameters.Add("@AmendmentDate", SqlDbType.DateTime).Value = modal.AmendmentDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsConfidential", SqlDbType.Bit).Value = modal.IsConfidential ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsAmended", SqlDbType.Bit).Value = modal.IsAmended ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsApplicable", SqlDbType.Bit).Value = modal.IsApplicable ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = modal.IsSynced ?? (object)DBNull.Value;

                        // JSL 02/25/2023
                        command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = modal.SavedAsDraft ?? (object)DBNull.Value;
                        command.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = modal.DocumentID ?? (object)DBNull.Value;
                        command.Parameters.Add("@ParentID", SqlDbType.UniqueIdentifier).Value = modal.ParentID ?? (object)DBNull.Value;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = modal.Type ?? (object)DBNull.Value;
                        command.Parameters.Add("@SectionType", SqlDbType.NVarChar).Value = modal.SectionType ?? (object)DBNull.Value;
                        // End JSL 02/25/2023

                        command.ExecuteScalar();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateRiskAssessmentForm : " + ex.Message);
                rafId = 0;
            }
            return rafId;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public void InsertOrUpdateRAFHazardsAndReviewers(RAFormModal objRiskAssement, long RAFID)
        {
            if (objRiskAssement.RiskAssessmentFormHazardList != null && objRiskAssement.RiskAssessmentFormHazardList.Count > 0)
            {
                // Delete existing
                DeleteRecords(AppStatic.RiskAssessmentFormHazard, "RAFUniqueID", Convert.ToString(objRiskAssement.RiskAssessmentForm.RAFUniqueID));
                foreach (var itemH in objRiskAssement.RiskAssessmentFormHazardList)
                {
                    itemH.RAFID = RAFID;
                    itemH.RAFUniqueID = objRiskAssement.RiskAssessmentForm.RAFUniqueID;
                }
                InsertDocumentsDataInRiskAssesmentHazared(objRiskAssement.RiskAssessmentFormHazardList);
            }
            if (objRiskAssement.RiskAssessmentFormReviewerList != null && objRiskAssement.RiskAssessmentFormReviewerList.Count > 0)
            {
                // Delete existing
                DeleteRecords(AppStatic.RiskAssessmentFormReviewer, "RAFUniqueID", Convert.ToString(objRiskAssement.RiskAssessmentForm.RAFUniqueID));
                foreach (var itemR in objRiskAssement.RiskAssessmentFormReviewerList)
                {
                    itemR.RAFID = RAFID;
                    itemR.RAFUniqueID = objRiskAssement.RiskAssessmentForm.RAFUniqueID;
                }
                InsertDocumentsDataInRiskAssessmentReviewer(objRiskAssement.RiskAssessmentFormReviewerList);
            }
        }
        // End JSL 11/26/2022

        private long InsertRiskAssessmentForm(RiskAssessmentForm modal)
        {
            long rafId = 0;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = "INSERT INTO " + AppStatic.RiskAssessmentForm +
                              " ([RAFUniqueID],[IsSynced],[ShipName],[ShipCode],[Number],[Title],[ReviewerName],[ReviewerDate],[ReviewerRank],[ReviewerLocation],[CreatedBy],[UpdatedBy],[UpdatedDate] " +  // JSL 11/26/2022 added RAFUniqueID, IsSynced, UpdatedBy, UpdatedDate
                              ",[CreatedDate],[Code],[Issue],[IssueDate],[Amendment],[AmendmentDate],[IsConfidential],[IsAmended],[IsApplicable]" +
                              ",[SavedAsDraft],[DocumentID],[ParentID],[Type],[SectionType]" +  // JSL 02/25/2023
                              ") OUTPUT INSERTED.RAFID " +
                              " VALUES(@RAFUniqueID,@IsSynced,@ShipName,@ShipCode,@Number,@Title,@ReviewerName,@ReviewerDate,@ReviewerRank,@ReviewerLocation,@CreatedBy,@UpdatedBy,@UpdatedDate, " +  // JSL 11/26/2022 added @RAFUniqueID, IsSynced, UpdatedBy, UpdatedDate
                              " @CreatedDate,@Code,@Issue,@IssueDate,@Amendment,@AmendmentDate,@IsConfidential,@IsAmended,@IsApplicable" +
                              ", @SavedAsDraft, @DocumentID, @ParentID, @Type, @SectionType" +  // JSL 02/25/2023
                              ")";

                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@RAFUniqueID", SqlDbType.UniqueIdentifier).Value = modal.RAFUniqueID ?? (object)DBNull.Value; // JSL 11/26/2022
                        command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = modal.ShipName ?? (object)DBNull.Value;
                        command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = modal.ShipCode ?? (object)DBNull.Value;
                        command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = modal.Number ?? (object)DBNull.Value;
                        command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = modal.Title ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerName", SqlDbType.NVarChar).Value = modal.ReviewerName ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerDate", SqlDbType.DateTime).Value = modal.ReviewerDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerRank", SqlDbType.NVarChar).Value = modal.ReviewerRank ?? (object)DBNull.Value;
                        command.Parameters.Add("@ReviewerLocation", SqlDbType.NVarChar).Value = modal.ReviewerLocation ?? (object)DBNull.Value;
                        command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = modal.CreatedBy ?? (object)DBNull.Value;
                        command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = modal.CreatedDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = modal.UpdatedBy ?? (object)DBNull.Value;   // JSL 11/26/2022
                        command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = modal.UpdatedDate ?? (object)DBNull.Value;   // JSL 11/26/2022
                        command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = modal.Code ?? (object)DBNull.Value;
                        command.Parameters.Add("@Issue", SqlDbType.Int).Value = modal.Issue ?? (object)DBNull.Value;
                        command.Parameters.Add("@IssueDate", SqlDbType.DateTime).Value = modal.IssueDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@Amendment", SqlDbType.NVarChar).Value = modal.Amendment ?? (object)DBNull.Value;
                        command.Parameters.Add("@AmendmentDate", SqlDbType.DateTime).Value = modal.AmendmentDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsConfidential", SqlDbType.Bit).Value = modal.IsConfidential ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsAmended", SqlDbType.Bit).Value = modal.IsAmended ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsApplicable", SqlDbType.Bit).Value = modal.IsApplicable ?? (object)DBNull.Value;
                        command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = modal.IsSynced ?? (object)DBNull.Value;  // JSL 11/26/2022

                        // JSL 02/25/2023
                        command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = modal.SavedAsDraft ?? (object)DBNull.Value;
                        command.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = modal.DocumentID ?? (object)DBNull.Value;
                        command.Parameters.Add("@ParentID", SqlDbType.UniqueIdentifier).Value = modal.ParentID ?? (object)DBNull.Value;
                        command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = modal.Type ?? (object)DBNull.Value;
                        command.Parameters.Add("@SectionType", SqlDbType.NVarChar).Value = modal.SectionType ?? (object)DBNull.Value;
                        // End JSL 02/25/2023

                        object resultObj = command.ExecuteScalar();
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out rafId);
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocument : " + ex.Message);
                rafId = 0;
            }
            return rafId;
        }
        private bool InsertDocumentsDataInRiskAssesmentHazared(List<RiskAssessmentFormHazard> AllDocuments)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                DataTable dt = Utility.ToDataTable(AllDocuments);
                using (SqlConnection connection = new SqlConnection(connetionString))
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                        SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                    bulkCopy.DestinationTableName = AppStatic.RiskAssessmentFormHazard;
                    connection.Open();
                    bulkCopy.WriteToServer(dt);
                    connection.Close();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsDataInRiskAssesmentHazared Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        private bool InsertDocumentsDataInRiskAssessmentReviewer(List<RiskAssessmentFormReviewer> AllDocuments)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                DataTable dt = Utility.ToDataTable(AllDocuments);
                using (SqlConnection connection = new SqlConnection(connetionString))
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                        SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                    bulkCopy.DestinationTableName = AppStatic.RiskAssessmentFormReviewer;
                    connection.Open();
                    bulkCopy.WriteToServer(dt);
                    connection.Close();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsDataInRiskAssessmentReviewer Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        // JSL 11/26/2022
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
                LogHelper.writelog("RAF DeleteRecords Local DB in table Error : " + ex.Message.ToString());
                return false;
            }
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public static void IfConnectionOpenThenCloseIt(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        // End JSL 11/26/2022
    }
}
