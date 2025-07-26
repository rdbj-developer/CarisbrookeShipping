using Newtonsoft.Json;
using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace ShipApplication.BLL.Helpers
{
    public class LocalDBHelper
    {
        public bool CheckDatabaseExists(string connectionString, string databaseName)
        {
            string sqlCreateDBQuery;
            bool result = false;
            try
            {
                SqlConnection tmpConn = new SqlConnection(connectionString);
                sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);
                using (tmpConn)
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        tmpConn.Open();
                        object resultObj = sqlCmd.ExecuteScalar();
                        int databaseID = 0;
                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseID);
                        }
                        tmpConn.Close();
                        result = (databaseID > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.writelog(ex.Message);
            }
            return result;
        }
        public bool CreateDatabase(string connectionString, string databaseName)
        {
            bool result = false;
            try
            {
                SqlConnection tmpConn = new SqlConnection(connectionString);
                string query = "CREATE DATABASE " + databaseName + ";";
                using (tmpConn)
                {
                    using (SqlCommand sqlCmd = new SqlCommand(query, tmpConn))
                    {
                        tmpConn.Open();
                        sqlCmd.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                LogHelper.writelog(ex.Message);
            }
            return result;
        }
        public void CreateDBConfigJson(ServerConnectModal Modal)
        {
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\DBConfig.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                string jsonData = JsonConvert.SerializeObject(Modal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
                CreateDBConfigJsonForService(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DBConfig Create Error : " + ex.Message);
            }
        }
        public void CreateDBConfigJsonForService(ServerConnectModal Modal)
        {
            try
            {
                string jsonFilePath = ConfigurationManager.AppSettings["DBConfigPath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                string jsonData = JsonConvert.SerializeObject(Modal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DBConfigJsonForService Create Error : " + ex.Message);
            }
        }
        public static ServerConnectModal ReadDBConfigJson()
        {
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\DBConfig.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!File.Exists(jsonFilePath))
                {
                    File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = File.ReadAllText(jsonFilePath);
                if (!string.IsNullOrEmpty(jsonText))
                {
                    ServerConnectModal Modal = JsonConvert.DeserializeObject<ServerConnectModal>(jsonText);
                    return Modal;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DBConfig Create Error : " + ex.Message);
                return null;
            }
        }
        public static bool WriteDBConfigJson(bool isInspector)
        {
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\DBConfig.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!File.Exists(jsonFilePath))
                {
                    File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = File.ReadAllText(jsonFilePath);
                if (!string.IsNullOrEmpty(jsonText))
                {
                    ServerConnectModal Modal = JsonConvert.DeserializeObject<ServerConnectModal>(jsonText);
                    Modal.IsInspector = isInspector;
                    jsonText = JsonConvert.SerializeObject(Modal, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, jsonText);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DBConfig Create Error : " + ex.Message);
                return false;
            }
        }
        public static bool CheckTableExist(string TableName)
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection conn = new SqlConnection(connetionString);
                    SqlCommand cmd = new SqlCommand(@"IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table)SELECT 1 ELSE SELECT 0", conn);
                    conn.Open();
                    cmd.Parameters.Add("@table", SqlDbType.NVarChar).Value = TableName;
                    int exists = (int)cmd.ExecuteScalar();
                    if (exists == 1)
                    {
                        //table exists
                        res = true;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }

        public static bool CheckTableColumnExist(string TableName, string ColumnName)
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection conn = new SqlConnection(connetionString);
                    SqlCommand cmd = new SqlCommand(@"IF EXISTS(SELECT * FROM sys.columns WHERE NAME = N'" + ColumnName + "' AND [object_id] = OBJECT_ID(N'" + TableName + "')) SELECT 1 ELSE SELECT 0", conn);
                    conn.Open();
                    int exists = int.Parse(cmd.ExecuteScalar().ToString());
                    if (exists == 1)
                    {
                        //Column exists
                        res = true;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }

        public static bool CreateTable(string TableName)
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    // string connetionString = "Data Source=" + dbConnModal.ServerName + ";Initial Catalog=" + dbConnModal.DatabaseName + ";User ID=" + dbConnModal.UserName + ";Password=" + dbConnModal.Password + "";
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection conn = new SqlConnection(connetionString);
                    conn.Open();
                    string SMRQuery = string.Empty;

                    if (TableName == AppStatic.ArrivalReports)
                        SMRQuery = TableQueryGenerator.ArrivalReportsTableQuery();
                    else if (TableName == AppStatic.DepartureReports)
                        SMRQuery = TableQueryGenerator.DepartureReportsTableQuery();
                    else if (TableName == AppStatic.DailyCargoReports)
                        SMRQuery = TableQueryGenerator.DailyCargoReportsTableQuery();
                    else if (TableName == AppStatic.DailyPositionReport)
                        SMRQuery = TableQueryGenerator.DailyPositionReportTableQuery();
                    else if (TableName == AppStatic.GeneralInspectionReport)
                        SMRQuery = TableQueryGenerator.GeneralInspectionReportTableQuery();
                    else if (TableName == AppStatic.GIRSafeManningRequirements)
                        SMRQuery = TableQueryGenerator.GIRSafeManningRequirementsTableQuery();
                    else if (TableName == AppStatic.GIRCrewDocuments)
                        SMRQuery = TableQueryGenerator.GIRCrewDocumentsTableQuery();
                    else if (TableName == AppStatic.GIRRestandWorkHours)
                        SMRQuery = TableQueryGenerator.GIRRestandWorkHoursTableQuery();
                    else if (TableName == AppStatic.GIRDeficiencies)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesTableQuery();
                    else if (TableName == AppStatic.GIRPhotographs)
                        SMRQuery = TableQueryGenerator.GIRPhotographsTableQuery();
                    else if (TableName == AppStatic.GIRDeficienciesFiles)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesFileTableQuery();
                    else if (TableName == AppStatic.GIRDeficienciesNote)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesNoteTableQuery();
                    else if (TableName == AppStatic.GIRDeficienciesCommentFile)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesCommentFileTableQuery();
                    else if (TableName == AppStatic.SuperintendedInspectionReport)
                        SMRQuery = TableQueryGenerator.SuperintendedInspectionReportTableQuery();
                    else if (TableName == AppStatic.SIRAdditionalNotes)
                        SMRQuery = TableQueryGenerator.SIRAdditionalNotesTableQuery();
                    else if (TableName == AppStatic.SIRNotes)
                        SMRQuery = TableQueryGenerator.SIRNotesTableQuery();
                    else if (TableName == AppStatic.InternalAuditForm)
                        SMRQuery = TableQueryGenerator.IAFTableQuery();
                    else if (TableName == AppStatic.AuditNotes)
                        SMRQuery = TableQueryGenerator.IAFNotesTableQuery();
                    else if (TableName == AppStatic.AuditNotesAttachment)
                        SMRQuery = TableQueryGenerator.IAFNotesAttachTableQuery();
                    else if (TableName == AppStatic.Documents)
                        SMRQuery = TableQueryGenerator.DocumentsTableQuery();
                    else if (TableName == AppStatic.Forms)
                        SMRQuery = TableQueryGenerator.FormsTableQuery();
                    else if (TableName == AppStatic.Users)
                        SMRQuery = TableQueryGenerator.UsersTableQuery();
                    else if (TableName == AppStatic.HoldVentilationRecordForm)
                        SMRQuery = TableQueryGenerator.HoldVentilationRecordFormTableQuery();
                    else if (TableName == AppStatic.HoldVentilationRecordSheet)
                        SMRQuery = TableQueryGenerator.HoldVentilationRecordSheetFormTableQuery();
                    else if (TableName == AppStatic.ShipAppReleaseNote)
                        SMRQuery = TableQueryGenerator.ShipAppReleaseNoteTableQuery();
                    else if (TableName == AppStatic.FeedbackForm)
                        SMRQuery = TableQueryGenerator.FeedbackFormTableQuery();
                    else if (TableName == AppStatic.RiskAssessmentForm)
                        SMRQuery = TableQueryGenerator.RiskAssessmentTableQuery();
                    else if (TableName == AppStatic.RiskAssessmentFormHazard)
                        SMRQuery = TableQueryGenerator.RiskAssessmentFormHazardTableQuery();
                    else if (TableName == AppStatic.RiskAssessmentFormReviewer)
                        SMRQuery = TableQueryGenerator.RiskAssessmentFormReviewerTableQuery();
                    else if (TableName == AppStatic.AssetManagmentEquipmentList)
                        SMRQuery = TableQueryGenerator.AssetManagmentEquipmentListTableQuery();
                    else if (TableName == AppStatic.AssetManagmentEquipmentOTList)
                        SMRQuery = TableQueryGenerator.AssetManagmentEquipmentOTListTableQuery();
                    else if (TableName == AppStatic.AssetManagmentEquipmentITList)
                        SMRQuery = TableQueryGenerator.AssetManagmentEquipmentITListTableQuery();
                    else if (TableName == AppStatic.AssetManagmentEquipmentSoftwareAssets)
                        SMRQuery = TableQueryGenerator.AssetManagmentEquipmentSoftwareAssetsTableQuery();
                    else if (TableName == AppStatic.CybersecurityRisksAssessment)
                        SMRQuery = TableQueryGenerator.CybersecurityRisksAssessmentTableQuery();
                    else if (TableName == AppStatic.CybersecurityRisksAssessmentList)
                        SMRQuery = TableQueryGenerator.CybersecurityRisksAssessmentListTableQuery();
                    else if (TableName == AppStatic.GIRDeficienciesInitialActions)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesInitialActionsTableQuery();
                    else if (TableName == AppStatic.GIRDeficienciesInitialActionsFile)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesInitialActionsFileTableQuery();
                    else if (TableName == AppStatic.GIRDeficienciesResolution)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesResolutionTableQuery();
                    else if (TableName == AppStatic.GIRDeficienciesResolutionFile)
                        SMRQuery = TableQueryGenerator.GIRDeficienciesResolutionFileTableQuery();
                    //RDBJ 09/16/2021 else if Added
                    else if (TableName == AppStatic.CSShips)
                        SMRQuery = TableQueryGenerator.CSShipsTableQuery();

                    SqlCommand cmd = new SqlCommand(SMRQuery, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public static bool CreateAllTable()
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = "Data Source=" + dbConnModal.ServerName + ";Initial Catalog=" + dbConnModal.DatabaseName + ";User ID=" + dbConnModal.UserName + ";Password=" + dbConnModal.Password + "";
                    SqlConnection conn = new SqlConnection(connetionString);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SMRForm) + TableQueryGenerator.SMRFormTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SMRFormCrewMembers) + TableQueryGenerator.SMRFormCrewMembersTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.Documents) + TableQueryGenerator.DocumentsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.ArrivalReports) + TableQueryGenerator.ArrivalReportsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.DepartureReports) + TableQueryGenerator.DepartureReportsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.DailyCargoReports) + TableQueryGenerator.DailyCargoReportsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.DailyPositionReport) + TableQueryGenerator.DailyPositionReportTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    // GIR
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GeneralInspectionReport) + TableQueryGenerator.GeneralInspectionReportTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRSafeManningRequirements) + TableQueryGenerator.GIRSafeManningRequirementsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRRestandWorkHours) + TableQueryGenerator.GIRRestandWorkHoursTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRCrewDocuments) + TableQueryGenerator.GIRCrewDocumentsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRPhotographs) + TableQueryGenerator.GIRPhotographsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficiencies) + TableQueryGenerator.GIRDeficienciesTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesFiles) + TableQueryGenerator.GIRDeficienciesFileTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesNote) + TableQueryGenerator.GIRDeficienciesNoteTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesCommentFile) + TableQueryGenerator.GIRDeficienciesCommentFileTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesInitialActions) + TableQueryGenerator.GIRDeficienciesInitialActionsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesInitialActionsFile) + TableQueryGenerator.GIRDeficienciesInitialActionsFileTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesResolution) + TableQueryGenerator.GIRDeficienciesResolutionTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesResolutionFile) + TableQueryGenerator.GIRDeficienciesResolutionFileTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    // SIR
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SuperintendedInspectionReport) + TableQueryGenerator.SuperintendedInspectionReportTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SIRAdditionalNotes) + TableQueryGenerator.SIRAdditionalNotesTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SIRNotes) + TableQueryGenerator.SIRNotesTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    // IAF
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.InternalAuditForm) + TableQueryGenerator.IAFTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotes) + TableQueryGenerator.IAFNotesTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesAttachment) + TableQueryGenerator.IAFNotesAttachTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesComments) + TableQueryGenerator.IAFNotesCommnetsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesCommentsFiles) + TableQueryGenerator.IAFNotesCommentsFilesTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesResolution) + TableQueryGenerator.IAFNotesResolutionTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesResolutionFiles) + TableQueryGenerator.IAFNotesResolutionFilesTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    // JSL 05/20/2022
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.MLCRegulationTree) + TableQueryGenerator.IAFMLCRegulationTreeTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SMSReferencesTree) + TableQueryGenerator.IAFSMSReferencesTreeTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SSPReferenceTree) + TableQueryGenerator.IAFSSPReferenceTreeTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    // End JSL 05/20/2022

                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.Forms) + TableQueryGenerator.FormsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.Users) + TableQueryGenerator.UsersTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.UsersTableTypeQuery(), conn);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.HoldVentilationRecordForm) + TableQueryGenerator.HoldVentilationRecordFormTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.HoldVentilationRecordSheet) + TableQueryGenerator.HoldVentilationRecordSheetFormTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.ShipAppReleaseNote) + TableQueryGenerator.ShipAppReleaseNoteTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.FeedbackForm) + TableQueryGenerator.FeedbackFormTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    //RAF
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.RiskAssessmentForm) + TableQueryGenerator.RiskAssessmentTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.RiskAssessmentFormHazard) + TableQueryGenerator.RiskAssessmentFormHazardTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.RiskAssessmentFormReviewer) + TableQueryGenerator.RiskAssessmentFormReviewerTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    //AssesMenagement OT-IT Equipment
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentList) + TableQueryGenerator.AssetManagmentEquipmentListTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentOTList) + TableQueryGenerator.AssetManagmentEquipmentOTListTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentITList) + TableQueryGenerator.AssetManagmentEquipmentITListTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentSoftwareAssets) + TableQueryGenerator.AssetManagmentEquipmentSoftwareAssetsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.CybersecurityRisksAssessment) + TableQueryGenerator.CybersecurityRisksAssessmentTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.CybersecurityRisksAssessmentList) + TableQueryGenerator.CybersecurityRisksAssessmentListTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    //RDBJ 09/16/2021
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.CSShips) + TableQueryGenerator.CSShipsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CSShipsTableTypeQuery(), conn);
                    cmd.ExecuteNonQuery();
                    //End RDBJ 09/16/2021

                    try
                    {
                        cmd = new SqlCommand(TableQueryGenerator.UsersInsertProcedureQuery(), conn);
                        cmd.ExecuteNonQuery();

                        //RDBJ 09/16/2021
                        cmd = new SqlCommand(TableQueryGenerator.CSShipsInsertProcedureQuery(), conn);
                        cmd.ExecuteNonQuery();
                        //End RDBJ 09/16/2021
                    }
                    catch { }
                    conn.Close();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public static void CreateExistingTableTable(string TableName)
        {
            bool isTableExist = LocalDBHelper.CheckTableExist(TableName);
            bool isTbaleCreated = true;
            if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(TableName); }
        }
        public static bool InsertDocumentsData(string FilePath)
        {
            bool res = false;
            try
            {
                List<DocumentModal> AllDocs = new List<DocumentModal>();
                DocumentTableHelper _helper = new DocumentTableHelper();
                AllDocs.AddRange(_helper.GetDocumentsMainCategories(FilePath));
                AllDocs.AddRange(_helper.GetDocumentsSubCategories(FilePath));
                _helper.InsertDocumentsDataInLocalDB(AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsData : " + ex.Message);
            }
            return res;
        }
        public static bool InsertDocumentsDataFromAWS(List<DocumentModal> AllDocs)
        {
            bool res = false;
            try
            {
                DocumentTableHelper _helper = new DocumentTableHelper();
                _helper.InsertDocumentsDataInLocalDB(AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsDataFromAWS : " + ex.Message);
            }
            return res;
        }
        public static bool InsertFormsData(string FilePath)
        {
            bool res = false;
            try
            {
                List<FormModal> AllDocs = new List<FormModal>();
                FormTableHelper _helper = new FormTableHelper();
                AllDocs.AddRange(_helper.GetFormsCategories(FilePath, "Forms"));
                AllDocs.AddRange(_helper.GetInfoPathFormCategories(FilePath, "Forms"));
                _helper.InsertFormsDataInLocalDB(AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertFormsData : " + ex.Message);
            }
            return res;
        }
        public static bool InsertFormsDataFromAWS(List<FormModal> AllDocs)
        {
            bool res = false;
            try
            {
                FormTableHelper _helper = new FormTableHelper();
                _helper.InsertFormsDataInLocalDB(AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertFormsDataFromAWS : " + ex.Message);
            }
            return res;
        }
        public static bool InsertXmlsData(string FilePath)
        {
            bool res = false;
            try
            {
                List<FormModal> AllDocs = new List<FormModal>();
                FormTableHelper _helper = new FormTableHelper();
                AllDocs.AddRange(_helper.GetFormsCategories(FilePath, "Xml"));
                _helper.InsertFormsDataInLocalDB(AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertXmlsData : " + ex.Message);
            }
            return res;
        }
        public static bool InsertDocumentsInRiskAssessmentData(string FilePath)
        {
            bool res = false;
            try
            {
                List<RiskAssessmentForm> AllDocs = new List<RiskAssessmentForm>();
                DocumentTableHelper _helper = new DocumentTableHelper();
                AllDocs.AddRange(_helper.GetDocumentsRiskAssesmentSubCategories(FilePath));
                _helper.InsertDocumentsDataInRiskAssessmentLocalDB(AllDocs);
                InsertDocumentsInRiskAssesmentHazaredData(FilePath, AllDocs);
                InsertDocumentsInRiskAssessmentReviewerData(FilePath, AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsInRiskAssessmentData : " + ex.Message);
            }
            return res;
        }

        public static bool InsertDocumentsInRiskAssessmentDataFromAWS(List<DocumentModal> AllDocumentList)
        {
            bool res = false;
            try
            {
                List<RiskAssessmentFormHazard> HazardList = new List<RiskAssessmentFormHazard>();
                List<RiskAssessmentFormReviewer> ReviewerList = new List<RiskAssessmentFormReviewer>();
                List<RiskAssessmentForm> AllDocs = new List<RiskAssessmentForm>();
                DocumentTableHelper _helper = new DocumentTableHelper();
                APIHelper _apiHelper = new APIHelper();
                var RAFID = 0;
                var awsRAFList = _apiHelper.GetRiskAssessmentDataToSeupShipApp(SessionManager.ShipCode);
                if (awsRAFList != null && awsRAFList.RiskAssessmentList != null && awsRAFList.RiskAssessmentList.Count > 0)
                {
                    AllDocs.AddRange(awsRAFList.RiskAssessmentList);
                    HazardList.AddRange(awsRAFList.RiskAssessmentFormHazardList);
                    ReviewerList.AddRange(awsRAFList.RiskAssessmentFormReviewerList);
                    _helper.InsertDocumentsDataInRiskAssessmentLocalDB(AllDocs);
                    //var response = _helper.GetAllDocumentRiskassessment(SessionManager.ShipCode);
                    //foreach (var item in AllDocs)
                    //{
                    //    RAFID = Convert.ToInt32(response.Where(x => x.Title == item.Title).Select(x => x.RAFID).SingleOrDefault());
                    //    if (RAFID == 0)
                    //        continue
                    //    var hList = awsRAFList.RiskAssessmentFormHazardList.Where(x => x.RAFID == item.RAFID).ToList();
                    //    if (hList != null && hList.Count > 0)
                    //    {
                    //        foreach (var itemH in hList)
                    //        {
                    //            itemH.RAFID = RAFID;
                    //        }
                    //        HazardList.AddRange(hList);
                    //    }
                    //    var reviewList = awsRAFList.RiskAssessmentFormReviewerList.Where(x => x.RAFID == item.RAFID).ToList();
                    //    if (reviewList != null && reviewList.Count > 0)
                    //    {
                    //        foreach (var itemR in reviewList)
                    //        {
                    //            itemR.RAFID = RAFID;
                    //        }
                    //        ReviewerList.AddRange(reviewList);
                    //    }
                    //}
                }
                else
                {
                    AllDocumentList = AllDocumentList.Where(x => Convert.ToString(x.Path).ToLower().Contains("risk assessments") && Convert.ToString(x.Type).ToLower() == "xml").ToList();
                    if (AllDocumentList != null && AllDocumentList.Count > 0)
                    {
                        var resultList = _helper.GetDocumentsRiskAssesmentSubCategoriesFromAWS(AllDocumentList);
                        if (resultList != null && resultList.Count > 0)
                        {
                            AllDocs = resultList.Select(x => x.RiskAssessmentForm).ToList();
                            _helper.InsertDocumentsDataInRiskAssessmentLocalDB(AllDocs);
                            var response = _helper.GetAllDocumentRiskassessment(SessionManager.ShipCode);

                            foreach (var item in resultList)
                            {
                                RAFID = Convert.ToInt32(response.Where(x => x.Title == item.RiskAssessmentForm.Title).Select(x => x.RAFID).SingleOrDefault());
                                if (RAFID == 0)
                                    continue;
                                if (item.RiskAssessmentFormHazardList != null && item.RiskAssessmentFormHazardList.Count > 0)
                                {
                                    foreach (var itemH in item.RiskAssessmentFormHazardList)
                                    {
                                        itemH.RAFID = RAFID;
                                    }
                                    HazardList.AddRange(item.RiskAssessmentFormHazardList);
                                }
                                if (item.RiskAssessmentFormReviewerList != null && item.RiskAssessmentFormReviewerList.Count > 0)
                                {
                                    foreach (var itemR in item.RiskAssessmentFormReviewerList)
                                    {
                                        itemR.RAFID = RAFID;
                                    }
                                    ReviewerList.AddRange(item.RiskAssessmentFormReviewerList);
                                }
                            }
                        }
                    }
                }
                if (HazardList != null && HazardList.Count > 0)
                    _helper.InsertDocumentsDataInRiskAssesmentHazaredLocalDB(HazardList);

                if (ReviewerList != null && ReviewerList.Count > 0)
                    _helper.InsertDocumentsDataInRiskAssessmentReviewerLocalDB(ReviewerList);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsInRiskAssessmentData : " + ex.Message);
            }
            return res;
        }

        public static bool InsertDocumentsInRiskAssessmentReviewerData(string FilePath, List<RiskAssessmentForm> ExistDocs)
        {
            bool res = false;
            try
            {
                List<RiskAssessmentFormReviewer> AllDocs = new List<RiskAssessmentFormReviewer>();
                DocumentTableHelper _helper = new DocumentTableHelper();
                AllDocs.AddRange(_helper.GetDocumentsRiskAssesmentReviewer(FilePath, ExistDocs));
                _helper.InsertDocumentsDataInRiskAssessmentReviewerLocalDB(AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertReviewerDocumentsData : " + ex.Message);
            }
            return res;
        }
        public static bool InsertDocumentsInRiskAssesmentHazaredData(string FilePath, List<RiskAssessmentForm> ExistDocs)
        {
            bool res = false;
            try
            {
                List<RiskAssessmentFormHazard> AllDocs = new List<RiskAssessmentFormHazard>();
                DocumentTableHelper _helper = new DocumentTableHelper();
                AllDocs.AddRange(_helper.GetDocumentsRiskAssesmentHazared(FilePath, ExistDocs));
                _helper.InsertDocumentsDataInRiskAssesmentHazaredLocalDB(AllDocs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsInRiskAssesmentHazaredData : " + ex.Message);
            }
            return res;
        }

        public static bool CreateRiskAssessmentsTable()
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = "Data Source=" + dbConnModal.ServerName + ";Initial Catalog=" + dbConnModal.DatabaseName + ";User ID=" + dbConnModal.UserName + ";Password=" + dbConnModal.Password + "";
                    SqlConnection conn = new SqlConnection(connetionString);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.RiskAssessmentForm) + TableQueryGenerator.RiskAssessmentTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.RiskAssessmentFormHazard) + TableQueryGenerator.RiskAssessmentFormHazardTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.RiskAssessmentFormReviewer) + TableQueryGenerator.RiskAssessmentFormReviewerTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }

        public static void CreateDBUsersJson()
        {
            try
            {
                APIUsersHelper _helper = new APIUsersHelper();
                List<UserModalAWS> UsersList = _helper.GetAWSUsers();

                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\UsersList.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                string jsonData = JsonConvert.SerializeObject(UsersList, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CreateDBUsersJson Create Error : " + ex.Message);
            }
        }
        public static List<UserModalAWS> ReadDBUsersJson()
        {
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\UsersList.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                if (!string.IsNullOrEmpty(jsonText))
                {
                    List<UserModalAWS> UsersList = JsonConvert.DeserializeObject<List<UserModalAWS>>(jsonText);
                    return UsersList;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ReadDBUsersJson Create Error : " + ex.Message);
                return null;
            }
        }
        public static bool DeleteUsersJson()
        {
            bool res = false;
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\UsersList.json";
                if (File.Exists(jsonFilePath))
                {
                    File.Delete(jsonFilePath);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ReadDBUsersJson Create Error : " + ex.Message);
            }
            return res;
        }
        public static UserModal LoginUser_LocalDB(ShipUserReq Modal)
        {
            UserModal user = new UserModal();
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string selectQuery = "SELECT * FROM " + AppStatic.Users + " WHERE empnre01 ='" + Modal.Password + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                var dbUser = dt.ToListof<UserModalAWS>().FirstOrDefault();
                                if (dbUser != null)
                                {
                                    string fname = dbUser.fstnme01.ToLower();
                                    string surname = dbUser.surnme01.ToLower();
                                    string username = string.Empty;
                                    if (fname.Contains(" "))
                                    {
                                        string[] fArray = fname.Split(' ');
                                        fname = fArray[0];
                                    }
                                    if (surname.Contains(" "))
                                    {
                                        string[] sArray = surname.Split(' ');
                                        surname = sArray[0];
                                    }
                                    username = fname + "." + surname;
                                    if (Modal.UserName.ToLower() == username && Modal.Password == dbUser.empnre01)
                                    {
                                        user.UID = dbUser.UID;
                                        user.EmployeeID = dbUser.empnre01;
                                        user.FirstName = dbUser.fstnme01;
                                        user.SurName = dbUser.surnme01;
                                        user.Rank = dbUser.fnce01;
                                        user.UserName = Modal.UserName;
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
                LogHelper.writelog("LoginUser_LocalDB Error : " + ex.Message);
            }
            return user;
        }
        public static bool CheckProcedureExist(string ProcedureName)
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection conn = new SqlConnection(connetionString);
                    SqlCommand cmd = new SqlCommand(@"IF EXISTS(SELECT 1 FROM sysobjects.TABLES where type='P' and name=@table)SELECT 1 ELSE SELECT 0", conn);
                    conn.Open();
                    cmd.Parameters.Add("@table", SqlDbType.NVarChar).Value = ProcedureName;
                    int exists = (int)cmd.ExecuteScalar();
                    if (exists == 1)
                    {
                        //table exists
                        res = true;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public static bool ExecuteQuery(string Query)
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    // string connetionString = "Data Source=" + dbConnModal.ServerName + ";Initial Catalog=" + dbConnModal.DatabaseName + ";User ID=" + dbConnModal.UserName + ";Password=" + dbConnModal.Password + "";
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection conn = new SqlConnection(connetionString);
                    conn.Open();
                    string SMRQuery = Query;
                    SqlCommand cmd = new SqlCommand(SMRQuery, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        //RDBJ 10/06/2021
        public static bool CreateCSShipsTableAndGetShipsData()
        {
            bool res = false;
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = "Data Source=" + dbConnModal.ServerName + ";Initial Catalog=" + dbConnModal.DatabaseName + ";User ID=" + dbConnModal.UserName + ";Password=" + dbConnModal.Password + "";
                    SqlConnection conn = new SqlConnection(connetionString);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();

                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.CSShips) + TableQueryGenerator.CSShipsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CSShipsTableTypeQuery(), conn);
                    cmd.ExecuteNonQuery();

                    bool IsSPExist = false;
                    string IsSPExistQuery = "select * from sysobjects where type='P' and name='usp_InsertUpdateAWSCSShips'";
                    cmd = new SqlCommand(IsSPExistQuery, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            IsSPExist = true;
                            break;
                        }
                    }

                    if (!IsSPExist)
                    {
                        cmd = new SqlCommand(TableQueryGenerator.CSShipsInsertProcedureQuery(), conn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd = new SqlCommand(TableQueryGenerator.CSShipsUPDATEInsertUpdateAWSCSShipsProcedureQuery(), conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CreateCSShipsTableAndGetShipsData : \n" + ex.Message);
            }
            return res;
        }
        //End RDBJ 10/06/2021
    }
}
