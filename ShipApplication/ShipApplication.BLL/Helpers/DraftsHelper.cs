using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ShipApplication.BLL.Helpers
{
    public class DraftsHelper
    {
        #region GIR
        public List<GIRData> GET_GIRDrafts_Local_DB(string shipCode)
        {
            List<GIRData> GIRDrafts = new List<GIRData>();
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
                            string selectQuery = "SELECT * FROM " + AppStatic.GeneralInspectionReport + " WHERE Ship ='" + shipCode + "' and SavedAsDraft = 1 and [isDelete] = 0"; // RDBJ 01/05/2022 added [isDelete]
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            dt.DefaultView.Sort = "Date DESC";
                            dt = dt.DefaultView.ToTable();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GeneralInspectionReport> SyncList = dt.ToListof<GeneralInspectionReport>();
                                foreach (var item in SyncList)
                                {
                                    GIRData Modal = new GIRData();
                                    Modal.Ship = item.Ship;
                                    Modal.ShipName = item.ShipName;
                                    Modal.GeneralPreamble = item.GeneralPreamble;
                                    Modal.Auditor = item.Inspector;
                                    Modal.Location = item.Port;
                                    Modal.Date = Utility.ToDateTimeStr(item.Date);
                                    Modal.GIRFormID = item.GIRFormID;
                                    Modal.UpdatedDate = item.UpdatedDate;
                                    Modal.UniqueFormID = item.UniqueFormID;
                                    GIRDrafts.Add(Modal);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDrafts Error : " + ex.Message);
            }
            return GIRDrafts;
        }
        public GeneralInspectionReport GIRFormDetailsView_Local_DB(string id)
        {
            GeneralInspectionReport dbModal = new GeneralInspectionReport();
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
                            string selectQuery = "SELECT * FROM " + AppStatic.GeneralInspectionReport + " WHERE UniqueFormID = '" + id + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GeneralInspectionReport> SyncList = dt.ToListof<GeneralInspectionReport>();
                                dbModal = SyncList[0];

                                DataTable dtCrewDocuments = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRCrewDocuments + " WHERE UniqueFormID = '" + id + "'", conn);
                                sqlAdp.Fill(dtCrewDocuments);
                                dbModal.GIRCrewDocuments = dtCrewDocuments.ToListof<GIRCrewDocuments>();

                                DataTable dtSafeManningRequirements = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRSafeManningRequirements + " WHERE UniqueFormID = '" + id + "'", conn);
                                sqlAdp.Fill(dtSafeManningRequirements);
                                dbModal.GIRSafeManningRequirements = dtSafeManningRequirements.ToListof<GlRSafeManningRequirements>();

                                DataTable dtRestandWorkHours = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRRestandWorkHours + " WHERE UniqueFormID = '" + id + "'", conn);
                                sqlAdp.Fill(dtRestandWorkHours);
                                dbModal.GIRRestandWorkHours = dtRestandWorkHours.ToListof<GIRRestandWorkHours>();

                                DataTable dtPhotographs = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRPhotographs + " WHERE UniqueFormID = '" + id + "'", conn);
                                sqlAdp.Fill(dtPhotographs);
                                dbModal.GIRPhotographs = dtPhotographs.ToListof<GIRPhotographs>();

                                // JSL 12/04/2022
                                if (dbModal.GIRPhotographs != null && dbModal.GIRPhotographs.Count > 0)
                                {
                                    foreach (var itemPhoto in dbModal.GIRPhotographs)
                                    {
                                        if (itemPhoto.ImagePath.StartsWith("data:"))
                                        {
                                            Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                                            dicFileMetaData["UniqueFormID"] = id;
                                            dicFileMetaData["ReportType"] = "GI";
                                            dicFileMetaData["FileName"] = itemPhoto.FileName;
                                            dicFileMetaData["Base64FileData"] = itemPhoto.ImagePath;

                                            itemPhoto.ImagePath = Utility.ConvertBase64IntoFile(dicFileMetaData, true);

                                            if (!string.IsNullOrEmpty(itemPhoto.ImagePath))
                                            {
                                                Dictionary<string, string> dicFilePathUpdateMetaData = new Dictionary<string, string>();
                                                dicFilePathUpdateMetaData["TableName"] = AppStatic.GIRPhotographs;
                                                dicFilePathUpdateMetaData["ColumnName"] = "ImagePath";
                                                dicFilePathUpdateMetaData["WhereColumnName"] = "PhotographsID";
                                                dicFilePathUpdateMetaData["WhereColumnDataID"] = Convert.ToString(itemPhoto.PhotographsID);
                                                Utility.UpdateFilePathIn_Local_DB(dicFilePathUpdateMetaData, itemPhoto.ImagePath);
                                            }
                                        }
                                    }
                                }
                                // End JSL 12/04/2022

                                DataTable dtDeficiencies = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE UniqueFormID = '" + id + "'", conn);
                                sqlAdp.Fill(dtDeficiencies);
                                dbModal.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>();

                                if (dbModal.GIRDeficiencies != null && dbModal.GIRDeficiencies.Count > 0)
                                {
                                    foreach (var def in dbModal.GIRDeficiencies)
                                    {
                                        DataTable dtDeficienciesFiles = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesFiles + " WHERE DeficienciesID = " + def.DeficienciesID , conn);
                                        sqlAdp.Fill(dtDeficienciesFiles);
                                        def.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                        if (def.GIRDeficienciesFile != null && def.GIRDeficienciesFile.Count > 0)
                                        {
                                            foreach (var deffile in def.GIRDeficienciesFile)
                                            {
                                                deffile.IsUpload = "true";
                                            }
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
                LogHelper.writelog("GIRFormDetailsView_Local_DB Error : " + ex.Message);
            }
            return dbModal;
        }

        public GeneralInspectionReport GIRFormGetDeficiency_Local_DB(string id)
        {
            GeneralInspectionReport dbModal = new GeneralInspectionReport();
            if (!string.IsNullOrWhiteSpace(id))
                dbModal.UniqueFormID = Guid.Parse(id);
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
                            string selectQuery = "SELECT SavedAsDraft, IsDeficienciesSectionComplete FROM " + AppStatic.GeneralInspectionReport + " WHERE UniqueFormID = '" + id + "'";
                            SqlDataAdapter sqlAdpDt = new SqlDataAdapter(selectQuery, conn);
                            sqlAdpDt.Fill(dt);
                            dbModal.SavedAsDraft = Convert.ToBoolean(dt.Rows[0]["SavedAsDraft"]);

                            //RDBJ 10/30/2021 Wrapped in If
                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["IsDeficienciesSectionComplete"])))
                                dbModal.IsDeficienciesSectionComplete = Convert.ToBoolean(dt.Rows[0]["IsDeficienciesSectionComplete"]); //RDBJ 10/19/2021
                            else
                                dbModal.IsDeficienciesSectionComplete = false;

                            DataTable dtDeficiencies = new DataTable();
                            var sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE UniqueFormID = '" + dbModal.UniqueFormID + "' and ReportType = 'GI' and ISNULL(isDelete, 0 ) = 0", conn);
                            sqlAdp.Fill(dtDeficiencies);
                            dbModal.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>();

                            if (dbModal.GIRDeficiencies != null && dbModal.GIRDeficiencies.Count > 0)
                            {
                                foreach (var def in dbModal.GIRDeficiencies)
                                {
                                    DataTable dtDeficienciesFiles = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT [GIRDeficienciesFileID], [DeficienciesID], [DeficienciesFileUniqueID], [FileName], [DeficienciesUniqueID] FROM " + AppStatic.GIRDeficienciesFiles + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn); //RDBJ 10/30/2021 Replace with DeficienciesUniqueID
                                    sqlAdp.Fill(dtDeficienciesFiles);
                                    def.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                    if (def.GIRDeficienciesFile != null && def.GIRDeficienciesFile.Count > 0)
                                    {
                                        foreach (var deffile in def.GIRDeficienciesFile)
                                        {
                                            deffile.IsUpload = "true";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRFormGetDeficiency_Local_DB Error : " + ex.Message);
            }
            return dbModal;

        }
        #endregion

        #region SIR
        public List<SIRData> GET_SIRDrafts_Local_DB(string shipCode)
        {
            List<SIRData> SIRDrafts = new List<SIRData>();
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
                            string selectQuery = "SELECT * FROM " + AppStatic.SuperintendedInspectionReport + " WHERE ShipName ='" + shipCode + "' and SavedAsDraft = 1 and [isDelete] = 0"; // RDBJ 01/05/2022 added [isDelete]
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            dt.DefaultView.Sort = "Date DESC";
                            dt = dt.DefaultView.ToTable();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<SuperintendedInspectionReport> SyncList = dt.ToListof<SuperintendedInspectionReport>();
                                foreach (var item in SyncList)
                                {
                                    SIRData Modal = new SIRData();
                                    Modal.SIRFormID = item.SIRFormID;
                                    Modal.ShipName = item.ShipName;
                                    Modal.Date = Utility.ToDateTimeStr(item.Date);
                                    Modal.Location = item.Port;
                                    Modal.Master = item.Master;
                                    Modal.Ship = item.ShipName;
                                    Modal.Superintended = item.Superintended;
                                    Modal.UpdatedDate = item.ModifyDate;
                                    Modal.UniqueFormID = item.UniqueFormID;
                                    SIRDrafts.Add(Modal);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GET_SIRDrafts_Local_DB Error : " + ex.Message);
            }
            return SIRDrafts;
        }
        public SIRModal SIRFormDetailsView_Local_DB(string id)
        {
            SIRModal dbModal = new SIRModal();
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
                            string selectQuery = "SELECT * FROM " + AppStatic.SuperintendedInspectionReport + " WHERE UniqueFormID ='" + id + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<SuperintendedInspectionReport> SyncList = dt.ToListof<SuperintendedInspectionReport>();
                                dbModal.SuperintendedInspectionReport = SyncList[0];
                                try
                                {
                                    DataTable dtSIRNote = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SIRNotes + " WHERE [IsDeleted] = 0 AND UniqueFormID ='" + id + "' AND NotesUniqueID IS NOT NULL", conn);   // RDBJ 04/04/2022 Added AND NotesUniqueID IS NOT NULL // RDBJ2 04/01/2022 Added [IsDeleted] = 0
                                    sqlAdp.Fill(dtSIRNote);
                                    if (dtSIRNote != null && dtSIRNote.Rows.Count > 0)
                                    {
                                        dbModal.SIRNote = dtSIRNote.ToListof<SIRNote>();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("SIRFormDetailsView_Local_DB SIRNotes Error : " + ex.Message);
                                }
                                try
                                {
                                    DataTable dtSIRAdditionalNote = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SIRAdditionalNotes + " WHERE [IsDeleted] = 0 AND UniqueFormID ='" + id + "' AND NotesUniqueID IS NOT NULL", conn); // RDBJ 04/04/2022 Added AND NotesUniqueID IS NOT NULL   // RDBJ2 04/01/2022 Added [IsDeleted] = 0
                                    sqlAdp.Fill(dtSIRAdditionalNote);
                                    dbModal.SIRAdditionalNote = dtSIRAdditionalNote.ToListof<SIRAdditionalNote>();
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("SIRFormDetailsView_Local_DB SIRAdditionalNotes Error : " + ex.Message);
                                }
                                try
                                {
                                    DataTable dtDeficiencies = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE UniqueFormID = '" + id + "' and ISNULL(isDelete, 0 ) = 0", conn);
                                    sqlAdp.Fill(dtDeficiencies);
                                    dbModal.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>();

                                    if (dbModal.GIRDeficiencies != null && dbModal.GIRDeficiencies.Count > 0)
                                    {
                                        foreach (var def in dbModal.GIRDeficiencies)
                                        {
                                            DataTable dtDeficienciesFiles = new DataTable();
                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesFiles + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                            sqlAdp.Fill(dtDeficienciesFiles);
                                            def.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                            if (def.GIRDeficienciesFile != null && def.GIRDeficienciesFile.Count > 0)
                                            {
                                                foreach (var deffile in def.GIRDeficienciesFile)
                                                {
                                                    deffile.IsUpload = "true";
                                                }
                                            }
                                        }
                                    }

                                }
                                catch (Exception)
                                {                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRFormDetailsView_Local_DB Error : " + ex.Message);
            }
            return dbModal;
        }

        public SIRModal SIRFormGetDeficiency_Local_DB(string id)
        {
            SIRModal dbModal = new SIRModal();
            if (!string.IsNullOrWhiteSpace(id) && id != "0")
                dbModal.SuperintendedInspectionReport.UniqueFormID = Guid.Parse(id);
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
                            //RDBJ 10/11/2021
                            DataTable dt = new DataTable();
                            string selectQuery = "SELECT * FROM " + AppStatic.SuperintendedInspectionReport + " WHERE UniqueFormID = '" + id + "'";
                            SqlDataAdapter sqlAdpDt = new SqlDataAdapter(selectQuery, conn);
                            sqlAdpDt.Fill(dt);
                            dbModal.SuperintendedInspectionReport.SavedAsDraft = Convert.ToBoolean(dt.Rows[0]["SavedAsDraft"]);
                            //End RDBJ 10/11/2021

                            DataTable dtDeficiencies = new DataTable();
                            var sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE UniqueFormID = '" + dbModal.SuperintendedInspectionReport.UniqueFormID + "' and ReportType = 'SI' and ISNULL(isDelete, 0 ) = 0", conn);
                            sqlAdp.Fill(dtDeficiencies);
                            dbModal.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>();

                            if (dbModal.GIRDeficiencies != null && dbModal.GIRDeficiencies.Count > 0)
                            {
                                foreach (var def in dbModal.GIRDeficiencies)
                                {
                                    DataTable dtDeficienciesFiles = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT [GIRDeficienciesFileID], [DeficienciesID], [DeficienciesFileUniqueID], [FileName], [DeficienciesUniqueID] FROM " + AppStatic.GIRDeficienciesFiles + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn); // RDBJ 01/15/2022 updated with DeficienciesUniqueID
                                    sqlAdp.Fill(dtDeficienciesFiles);
                                    def.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                    if (def.GIRDeficienciesFile != null && def.GIRDeficienciesFile.Count > 0)
                                    {
                                        foreach (var deffile in def.GIRDeficienciesFile)
                                        {
                                            deffile.IsUpload = "true";
                                        }
                                    }

                                    // RDBJ 02/20/2022
                                    DataTable dtDefInitialAction = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT TOP 1 [IniActUniqueID], [DeficienciesUniqueID], [Description] FROM " + AppStatic.GIRDeficienciesInitialActions + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "' ORDER BY [CreatedDate]", conn); // RDBJ 01/15/2022 updated with DeficienciesUniqueID
                                    sqlAdp.Fill(dtDefInitialAction);
                                    var defIniAction = dtDefInitialAction.ToListof<GIRDeficienciesInitialActions>();
                                    if (defIniAction != null && defIniAction.Count > 0)
                                    {
                                        foreach (var item in defIniAction)
                                        {
                                            GIRDeficienciesInitialActions initialActions = new GIRDeficienciesInitialActions();
                                            initialActions.IniActUniqueID = item.IniActUniqueID;
                                            initialActions.DeficienciesUniqueID = item.DeficienciesUniqueID;
                                            initialActions.Description = item.Description;

                                            def.GIRDeficienciesInitialActions.Add(initialActions);
                                        }
                                    }
                                    // End RDBJ 02/20/2022
                                    // JSL 06/27/2022 added else
                                    else
                                    {
                                        // JSL 06/27/2022 commented
                                        /*
                                        APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                                        GIRDeficienciesInitialActions initialActions = new GIRDeficienciesInitialActions();
                                        initialActions.DeficienciesUniqueID = def.DeficienciesUniqueID;
                                        initialActions.Name = SessionManager.Username;
                                        initialActions.Description = string.Empty;
                                        _helper.AddDeficienciesInitialActions_Local_DB(initialActions);

                                        DataTable dtDefInitialActionIfNotExist = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT TOP 1 [IniActUniqueID], [DeficienciesUniqueID], [Description] FROM " + AppStatic.GIRDeficienciesInitialActions + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "' ORDER BY [CreatedDate]", conn);
                                        sqlAdp.Fill(dtDefInitialActionIfNotExist);
                                        var defIniActionIfNotExist = dtDefInitialActionIfNotExist.ToListof<GIRDeficienciesInitialActions>();
                                        if (defIniActionIfNotExist != null && defIniActionIfNotExist.Count > 0)
                                        {
                                            foreach (var item in defIniActionIfNotExist)
                                            {
                                                GIRDeficienciesInitialActions initialActionsIfNotExist = new GIRDeficienciesInitialActions();
                                                initialActionsIfNotExist.IniActUniqueID = item.IniActUniqueID;
                                                initialActionsIfNotExist.DeficienciesUniqueID = item.DeficienciesUniqueID;
                                                initialActionsIfNotExist.Description = item.Description;

                                                def.GIRDeficienciesInitialActions.Add(initialActionsIfNotExist);
                                            }
                                        }
                                        */
                                        // End JSL 06/27/2022 commented
                                    }
                                    // End JSL 06/27/2022 added else
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRFormGetDeficiency_Local_DB : " + ex.Message);
            }
            return dbModal;
        }
        #endregion

        #region IAF
        // RDBJ 01/31/2022
        public List<AuditList> GET_IARDrafts_Local_DB(string shipCode)
        {
            List<AuditList> IARDrafts = new List<AuditList>();
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
                            string selectQuery = "SELECT * FROM " + AppStatic.InternalAuditForm + " WHERE ShipName ='" + shipCode + "' and SavedAsDraft = 1 and [isDelete] = 0"; // RDBJ 01/05/2022 added [isDelete]
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            dt.DefaultView.Sort = "Date DESC";
                            dt = dt.DefaultView.ToTable();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<InternalAuditForm> SyncList = dt.ToListof<InternalAuditForm>();
                                foreach (var item in SyncList)
                                {
                                    AuditList Modal = new AuditList();
                                    Modal.InternalAuditFormId = item.InternalAuditFormId;
                                    Modal.Auditor = item.Auditor;
                                    Modal.Location = item.Location;
                                    Modal.Date = Utility.ToDateTimeStr(item.Date);
                                    Modal.Type = item.AuditType == 1 ? "Internal" : "External";
                                    Modal.Ship = item.ShipName;
                                    Modal.ShipName = item.ShipName;
                                    Modal.UpdatedDate = item.UpdatedDate;
                                    Modal.UniqueFormID = (Guid)item.UniqueFormID;
                                    IARDrafts.Add(Modal);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GET_IARDrafts_Local_DB Error : " + ex.Message);
            }
            return IARDrafts;
        }
        // End RDBJ 01/31/2022
        #endregion

        #region common Functions For GISI
        public bool DeleteGISIIADrafts_Local_DB(string GISIIAFormID, string type) // RDBJ 01/31/2022 rename function //RDBJ 10/09/2021 int to string GISTFormID
        {
            try
            {
                string query = string.Empty;
                if (type.ToLower() == "gi")
                {
                    query = "UPDATE " + AppStatic.GeneralInspectionReport + " SET " +
                        //"SavedAsDraft = @SavedAsDraft " + // RDBJ 01/05/2022 commented this line
                        "[isDelete] = @isDelete " + // RDBJ 01/05/2022
                        "WHERE UniqueFormID = @UniqueFormID"; //RDBJ 10/09/2021 UniqueFormID
                }
                else if (type.ToLower() == "si") // RDBJ 01/31/2022 added if condition
                {
                    query = "UPDATE " + AppStatic.SuperintendedInspectionReport + " SET " +
                        //"SavedAsDraft = @SavedAsDraft " + // RDBJ 01/05/2022 commented this line
                        "[isDelete] = @isDelete " + // RDBJ 01/05/2022
                        "WHERE UniqueFormID = @UniqueFormID"; //RDBJ 10/09/2021 UniqueFormID
                }
                // RDBJ 01/31/2022 added else
                else
                {
                    query = "UPDATE " + AppStatic.InternalAuditForm + " SET " +
                        //"SavedAsDraft = @SavedAsDraft " +
                        "[isDelete] = @isDelete " + 
                        "WHERE UniqueFormID = @UniqueFormID";
                }
                // End RDBJ 01/31/2022 added else

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
                                //cmd.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = 0;  // RDBJ 01/05/2022 commented this line //RDBJ 10/09/2021
                                cmd.Parameters.Add("@isDelete", SqlDbType.Int).Value = 1;  // RDBJ 01/05/2022 
                                cmd.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(GISIIAFormID); //RDBJ 10/09/2021
                                conn.Open();
                                int rowsAffected = cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }

                // RDBJ 01/05/2022
                APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();
                _defhelper.UpdateGIRSyncStatus_Local_DB(Convert.ToString(GISIIAFormID), type);
                // End RDBJ 01/05/2022

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteGISIIADrafts_Local_DB : " + ex.Message);
                return false;
            }
        }
        #endregion
    }
}
