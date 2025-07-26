using CarisbrookeShippingService.BLL.Modals;
using CarisbrookeShippingService.BLL.Resources.Constant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public static class Utility
    {
        // JSL 09/28/2022
        public static string Base64Encode(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return text;
            }

            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }
        // End JSL 09/28/2022

        // JSL 09/28/2022
        public static string Base64Decode(string base64)
        {
            if (String.IsNullOrEmpty(base64))
            {
                return base64;
            }

            var base64Bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
        // End JSL 09/28/2022

        // JSL 09/28/2022
        public static string GenerateBasicOAuthToken(string strUserName = "", string strPassword = "")
        {
            string strOAuthToken = string.Empty;

            strUserName = CarisbrookeShippingAPI.USERNAME_VALUE;
            strPassword = CarisbrookeShippingAPI.PASSWORD_VALUE;

            strOAuthToken = Base64Encode(strUserName + ":" + strPassword);
            return strOAuthToken;
        }
        // End JSL 09/28/2022

        //RDBJ 10/28/2021
        public static DateTime ToDateTimeUtcNow()
        {
            try
            {
                return DateTime.UtcNow;
            }
            catch (Exception ex)
            {

                return DateTime.UtcNow;
            }

        }
        //End RDBJ 10/28/2021
        public static string ToString(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch { return string.Empty; }
        }
        public static bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string GetLocalDBConnStr(ServerConnectModal dbConnModal)
        {
            string connetionString = string.Empty;
            try
            {
                if (dbConnModal != null)
                {
                    connetionString = "Data Source=" + dbConnModal.ServerName + ";Initial Catalog=" + dbConnModal.DatabaseName + ";User ID=" + dbConnModal.UserName + ";Password=" + dbConnModal.Password + "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return connetionString;
        }
        public static ServerConnectModal ReadDBConfigJson()
        {
            try
            {
                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["DBConfigPath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (System.IO.File.Exists(jsonFilePath))
                {
                    jsonText = System.IO.File.ReadAllText(jsonFilePath);
                }
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
        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            DataTable table = new DataTable();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public static List<T> ToListof<T>(this DataTable dt) //, List<string> excludedColumnNames=null
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var objectProperties = typeof(T).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    var convertedValue = GetValueByDataType(properties.PropertyType, dataRow[properties.Name]);
                    properties.SetValue(instanceOfT, convertedValue, null);
                }
                if (instanceOfT == null)
                {
                    instanceOfT = Activator.CreateInstance<T>();
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }
        private static object GetValueByDataType(Type propertyType, object o)
        {
            if (o.ToString() == "null")
            {
                return null;
            }
            if (propertyType == (typeof(Guid)) || propertyType == typeof(Guid?))
            {
                return Guid.Parse(o.ToString());
            }
            else if (propertyType == typeof(int) || propertyType == typeof(int?) || propertyType.IsEnum)
            {
                return Convert.ToInt32(o);
            }
            else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                return Convert.ToDecimal(o);
            }
            else if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                return Convert.ToDouble(o);
            }
            else if (propertyType == typeof(long) || propertyType == typeof(long?))
            {
                return Convert.ToInt64(o);
            }
            else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return Convert.ToBoolean(o);
            }
            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                return Convert.ToDateTime(o);
            }
            return o.ToString();
        }
        public static string GetPCUniqueId()
        {
            string uniqueId = string.Empty;
            try
            {
                string cpuInfo = string.Empty;
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();
                try
                {
                    foreach (ManagementObject mo in moc)
                    {
                        cpuInfo = mo.Properties["processorID"].Value.ToString();
                        break;
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    string drive = "C";
                    ManagementObject dsk = new ManagementObject(
                        @"win32_logicaldisk.deviceid=""" + drive + @":""");
                    dsk.Get();
                    string volumeSerial = dsk["VolumeSerialNumber"].ToString();
                    uniqueId = cpuInfo + volumeSerial;
                }
                catch (Exception)
                {
                    uniqueId = cpuInfo + Environment.MachineName;
                }
            }
            catch (Exception ex)
            {
                uniqueId = Environment.MachineName;
                LogHelper.writelog("GetPCUniqueId : " + ex.Message);
            }
            return uniqueId;
        }
        public static SimpleObject GetShipValue()
        {
            SimpleObject res = new SimpleObject();
            try
            {
                string ShipAPPUrl = System.Configuration.ConfigurationManager.AppSettings["ShipAPPLocalPath"];
                string jsonFilePath = Path.Combine(ShipAPPUrl, @"JsonFiles\Shipvalue.json"); //rdbj 09/30/2021 removed twise backslash
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                res = JsonConvert.DeserializeObject<SimpleObject>(jsonText);
            }
            catch (Exception)
            {

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
                    //AssesMenagement OT-IT Equipment
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentList) + TableQueryGenerator.AssetManagmentEquipmentListTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentOTList) + TableQueryGenerator.AssetManagmentEquipmentOTListTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentITList) + TableQueryGenerator.AssetManagmentEquipmentITListTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AssetManagmentEquipmentSoftwareAssets) + TableQueryGenerator.AssetManagmentEquipmentSoftwareAssetsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    //Cybersecurity
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.CybersecurityRisksAssessment) + TableQueryGenerator.CybersecurityRisksAssessmentTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.CybersecurityRisksAssessmentList) + TableQueryGenerator.CybersecurityRisksAssessmentListTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    //RDBJ 10/05/2021
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.DeficienciesNote ) + TableQueryGenerator.GIRDeficienciesNoteTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesCommentFile) + TableQueryGenerator.GIRDeficienciesCommentFileTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    //End RDBJ 10/05/2021

                    //RDBJ 10/02/2021
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesInitialActions) + TableQueryGenerator.GIRDeficienciesInitialActionsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesInitialActionsFile) + TableQueryGenerator.GIRDeficienciesInitialActionsFileTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesResolution) + TableQueryGenerator.GIRDeficienciesResolutionTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.GIRDeficienciesResolutionFile) + TableQueryGenerator.GIRDeficienciesResolutionFileTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesResolution) + TableQueryGenerator.IAFNotesResolutionTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesResolutionFiles) + TableQueryGenerator.IAFNotesResolutionFilesTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    //End RDBJ 10/02/2021

                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesComment) + TableQueryGenerator.IAFNotesCommnetsTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.AuditNotesCommentFile) + TableQueryGenerator.IAFNotesCommentsFilesTableQuery(), conn);
                    cmd.ExecuteNonQuery();

                    // JSL 05/20/2022
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.MLCRegulationTree) + TableQueryGenerator.IAFMLCRegulationTreeTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SMSReferencesTree) + TableQueryGenerator.IAFSMSReferencesTreeTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.SSPReferenceTree) + TableQueryGenerator.IAFSSPReferenceTreeTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    // End JSL 05/20/2022

                    // RDBJ 12/30/2021
                    cmd = new SqlCommand(TableQueryGenerator.CheckTableExistQuery(AppStatic.HelpAndSupport) + TableQueryGenerator.HelpAndSupportTableQuery(), conn);
                    cmd.ExecuteNonQuery();
                    // End RDBJ 12/30/2021

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

        //RDBJ 10/02/2021
        public static bool DatabaseModification()
        {
            bool res = false;
            try
            {
                #region AlterTable Query Scripts

                //RDBJ 10/20/2021
                string[] ColumnsNameForIsSectionComplete =
                {
                    "IsGeneralSectionComplete",
                    "IsManningSectionComplete",
                    "IsShipCertificationSectionComplete",
                    "IsPubsAndDocsSectionComplete",
                    "IsRecordKeepingSectionComplete",
                    "IsSafetyEquipmentSectionComplete",
                    "IsSecuritySectionComplete",
                    "IsBridgeSectionComplete",
                    "IsMedicalSectionComplete",
                    "IsGalleySectionComplete",
                    "IsEngineRoomSectionComplete",
                    "IsSuperstructureSectionComplete",
                    "IsDeckSectionComplete",
                    "IsHoldsAndCoverSectionComplete",
                    "IsForeCastleSectionComplete",
                    "IsHullSectionComplete",
                    "IsSummarySectionComplete",
                    "IsDeficienciesSectionComplete",
                    "IsPhotographsSectionComplete"
                };
                for (int i = 0; i < ColumnsNameForIsSectionComplete.Length; i++)
                {
                    bool isColumnExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, ColumnsNameForIsSectionComplete[i]);
                    if (!isColumnExist)
                        Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD " + ColumnsNameForIsSectionComplete[i] + " BIT DEFAULT 0");
                }
                //End RDBJ 10/20/2021

                string[] TablesNameForUNIQUEFORMID =
                {
                    AppStatic.GeneralInspectionReport,
                    AppStatic.GlRSafeManningRequirements,
                    AppStatic.GlRCrewDocuments,
                    AppStatic.GIRRestandWorkHours,
                    AppStatic.GIRPhotographs, //RDBJ 11/01/2021
                    AppStatic.SuperintendedInspectionReport,
                    AppStatic.SIRNotes,
                    AppStatic.SIRAdditionalNotes,
                    AppStatic.InternalAuditForm,
                    AppStatic.AuditNotes,
                    AppStatic.GIRDeficiencies
                };
                for (int i = 0; i < TablesNameForUNIQUEFORMID.Length; i++)
                {
                    bool isColumnUNIQUEFORMID = Utility.CheckTableColumnExist(TablesNameForUNIQUEFORMID[i], AppStatic.UNIQUEFORMID);
                    if (!isColumnUNIQUEFORMID)
                        Utility.ExecuteQuery("ALTER TABLE " + TablesNameForUNIQUEFORMID[i] + " ADD " + AppStatic.UNIQUEFORMID + " uniqueidentifier");
                }

                //RDBJ 11/01/2021
                string[] TablesNameForDEFICIENCIESUNIQUEID =
                {
                    AppStatic.GIRDeficiencies,
                    AppStatic.GIRDeficienciesFile,
                    AppStatic.DeficienciesNote,
                    AppStatic.GIRDeficienciesInitialActions,
                    AppStatic.GIRDeficienciesResolution
                };
                for (int i = 0; i < TablesNameForDEFICIENCIESUNIQUEID.Length; i++)
                {
                    bool isColumnDEFICIENCIESUNIQUEID = Utility.CheckTableColumnExist(TablesNameForDEFICIENCIESUNIQUEID[i], AppStatic.DEFICIENCIESUNIQUEID);
                    if (!isColumnDEFICIENCIESUNIQUEID)
                        Utility.ExecuteQuery("ALTER TABLE " + TablesNameForDEFICIENCIESUNIQUEID[i] + " ADD " + AppStatic.DEFICIENCIESUNIQUEID + " uniqueidentifier");
                }
                //End RDBJ 11/01/2021

                string[] TablesNameForFORMVERSION =
                {
                    AppStatic.GeneralInspectionReport,
                    AppStatic.SuperintendedInspectionReport,
                    AppStatic.InternalAuditForm
                };
                for (int i = 0; i < TablesNameForFORMVERSION.Length; i++)
                {
                    bool isColumnFORMVERSION = Utility.CheckTableColumnExist(TablesNameForFORMVERSION[i], AppStatic.FORMVERSION);
                    if (!isColumnFORMVERSION)
                        Utility.ExecuteQuery("ALTER TABLE " + TablesNameForFORMVERSION[i] + " ADD " + AppStatic.FORMVERSION + " numeric(18,2)");
                }

                TableGeneralInspectionReportModification(); //RDBJ 10/31/2021
                TableGIRDeficienciesAndFilesModification(); //RDBJ 10/31/2021
                TableGIRDeficienciesNotesAndFilesModification();
                TableGIRDeficienciesInitialActionsAndFilesModification();
                TableGIRGIRDeficienciesResolutionAndFilesModification();

                TableSuperintendedInspectionReportModification(); // RDBJ 01/05/2022
                TableSIRNotesModification();  // RDBJ 04/01/2022
                TableSIRAdditionalNotesModification();  // RDBJ 04/01/2022

                string[] TablesNameForNOTEUNIQUEID =
                {
                    AppStatic.SIRNotes,
                    AppStatic.SIRAdditionalNotes,
                    AppStatic.AuditNotes,
                    AppStatic.AuditNotesAttachment,
                    AppStatic.AuditNotesComment,
                    AppStatic.AuditNotesResolution //RDBJ 10/05/2021
                };
                for (int i = 0; i < TablesNameForNOTEUNIQUEID.Length; i++)
                {
                    bool isColumnNOTEUNIQUEID = Utility.CheckTableColumnExist(TablesNameForNOTEUNIQUEID[i], AppStatic.NOTEUNIQUEID);
                    if (!isColumnNOTEUNIQUEID)
                        Utility.ExecuteQuery("ALTER TABLE " + TablesNameForNOTEUNIQUEID[i] + " ADD " + AppStatic.NOTEUNIQUEID + " uniqueidentifier");
                    //RDBJ 10/05/2021 Added else
                    else
                    {
                        // RDBJ 04/01/2022 wrapped in if
                        if (TablesNameForNOTEUNIQUEID[i] == AppStatic.SIRNotes
                            || TablesNameForNOTEUNIQUEID[i] == AppStatic.SIRAdditionalNotes
                            )
                        {
                            Utility.ExecuteQuery("ALTER TABLE " + TablesNameForNOTEUNIQUEID[i] + " ALTER COLUMN " + AppStatic.NOTEUNIQUEID + " NVARCHAR");
                            Utility.ExecuteQuery("ALTER TABLE " + TablesNameForNOTEUNIQUEID[i] + " ALTER COLUMN " + AppStatic.NOTEUNIQUEID + " uniqueidentifier");
                        }
                        // End RDBJ 04/01/2022 wrapped in if
                    }
                }

                TableInternalAuditFormModification(); //RDBJ 11/25/2021
                TableIAFAuditNotesAndFileModification(); //RDBJ 10/31/2021
                TableIAFNotesCommnetsAndFileModification();
                TableIAFNotesResolutionsAndFileModification(); //RDBJ 10/05/2021

                //RDBJ 10/25/2021
                string[] isNewColumsTableList =
                {
                    AppStatic.DeficienciesNote,
                    AppStatic.GIRDeficienciesInitialActions,
                    AppStatic.GIRDeficienciesResolution,
                    AppStatic.AuditNotesComment,
                    AppStatic.AuditNotesResolution
                };
                for (int i = 0; i < isNewColumsTableList.Length; i++)
                {
                    bool isColumnisNew = Utility.CheckTableColumnExist(isNewColumsTableList[i], "isNew");
                    if (!isColumnisNew)
                        Utility.ExecuteQuery("ALTER TABLE " + isNewColumsTableList[i] + " ADD isNew INT NOT NULL DEFAULT(0)");
                }

                // JSL 11/24/2022
                string[] RAFUniqueIDColumnsTableList =
                {
                    AppStatic.RiskAssessmentForm,
                    AppStatic.RiskAssessmentFormHazard,
                    AppStatic.RiskAssessmentFormReviewer
                };

                for (int i = 0; i < RAFUniqueIDColumnsTableList.Length; i++)
                {
                    bool isColumnRAFUNIQUEID = Utility.CheckTableColumnExist(RAFUniqueIDColumnsTableList[i], AppStatic.RAFUNIQUEID);
                    if (!isColumnRAFUNIQUEID)
                        Utility.ExecuteQuery("ALTER TABLE " + RAFUniqueIDColumnsTableList[i] + " ADD " + AppStatic.RAFUNIQUEID + " uniqueidentifier");
                }
                // End JSL 11/24/2022 RAFUniqueID

                string[] StoredProcedureList =
                {
                    AppStatic.SP_GETALLNOTIFICATIONS,
                    AppStatic.SP_GETNOTIFICATIONDETAILSBYID,
                    AppStatic.SP_GET_GIDEFICIENCIES_OR_SIACTIONABLEITEMS_NUMBER,
                    AppStatic.SP_RESETGIDEFICIENCIESORSIACTIONABLEITEMSNUMBERSFROM501, // RDBJ 12/08/2021
                    AppStatic.SP_GETALLRELEASENOTES // RDBJ 12/08/2021
                    , AppStatic.SP_CB_PROC_SMSREFERENCESTREE_INSERTORUPDATE // JSL 05/20/2022
                    , AppStatic.SP_CB_PROC_SSPREFERENCETREE_INSERTORUPDATE // JSL 05/20/2022
                    , AppStatic.SP_CB_PROC_MLCREGULATIONTREE_INSERTORUPDATE // JSL 05/20/2022
                };
                for (int i = 0; i < StoredProcedureList.Length; i++)
                {
                    if (!StoredProcedure_ExistOrNot(StoredProcedureList[i]))
                    {
                        switch (StoredProcedureList[i])
                        {
                            // JSL 05/20/2022
                            case "CB_proc_SMSReferencesTree_InsertOrUpdate":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.CB_proc_SMSReferencesTree_InsertOrUpdate());
                                break;
                            case "CB_proc_SSPReferenceTree_InsertOrUpdate":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.CB_proc_SSPReferenceTree_InsertOrUpdate());
                                break;
                            case "CB_proc_MLCRegulationTree_InsertOrUpdate":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.CB_proc_MLCRegulationTree_InsertOrUpdate());
                                break;
                            // End JSL 05/20/2022
                            case "SP_GetAllNotifications":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.SP_GetAllNotifications());
                                break;
                            case "SP_GetNotificationDetailsById":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.SP_GetNotificationDetailsById());
                                break;
                            //RDBJ 10/31/2021
                            case "SP_Get_GIDeficiencies_OR_SIActionableItems_Number":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.SP_Get_GIDeficiencies_OR_SIActionableItems_Number());
                                break;
                            //RDBJ 12/08/2021
                            case "SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501());
                                break;
                            //RDBJ 12/08/2021
                            case "SP_GetAllReleaseNotes":
                                CreateIfNotExistStoredProcedure(TableQueryGenerator.SP_GetAllReleaseNotes());
                                break;
                            default:
                                break;
                        }
                    }
                }
                //End RDBJ 10/25/2021
                #endregion
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DatabaseModification : \n" + ex.Message);
            }
            return res;
        }
        //End RDBJ 10/02/2021

        //RDBJ 10/31/2021
        public static void TableGeneralInspectionReportModification()
        {
            try
            {
                //Table GeneralInspectionReport
                bool isColumnSnapBackZoneCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "SnapBackZoneComment");
                if (!isColumnSnapBackZoneCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD SnapBackZoneComment nvarchar(max)");

                bool isColumnConditionGantryCranesCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "ConditionGantryCranesComment");
                if (!isColumnConditionGantryCranesCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD ConditionGantryCranesComment nvarchar(max)");

                bool isColumnCylindersLockerCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "CylindersLockerComment");
                if (!isColumnCylindersLockerCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD CylindersLockerComment nvarchar(max)");

                bool isColumnSnapBackZone5NCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "SnapBackZone5NComment");
                if (!isColumnSnapBackZone5NCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD SnapBackZone5NComment nvarchar(max)");

                bool isColumnMedicalLogBookCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "MedicalLogBookComment");
                if (!isColumnMedicalLogBookCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD MedicalLogBookComment nvarchar(max)");

                bool isColumnDrugsNarcoticsCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "DrugsNarcoticsComment");
                if (!isColumnDrugsNarcoticsCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD DrugsNarcoticsComment nvarchar(max)");

                bool isColumnDefibrillatorCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "DefibrillatorComment");
                if (!isColumnDefibrillatorCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD DefibrillatorComment nvarchar(max)");

                bool isColumnADPPublicationsCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "ADPPublicationsComment");
                if (!isColumnADPPublicationsCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD ADPPublicationsComment nvarchar(max)");

                bool isColumnLiferaftDavitCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "LiferaftDavitComment");
                if (!isColumnLiferaftDavitCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD LiferaftDavitComment nvarchar(max)");

                bool isColumnDaylightSignalsCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "DaylightSignalsComment");
                if (!isColumnDaylightSignalsCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD DaylightSignalsComment nvarchar(max)");

                bool isColumnBridgewindowswipersCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "BridgewindowswipersComment");
                if (!isColumnBridgewindowswipersCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD BridgewindowswipersComment nvarchar(max)");

                bool isColumnBridgewindowswiperssprayCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "BridgewindowswiperssprayComment");
                if (!isColumnBridgewindowswiperssprayCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD BridgewindowswiperssprayComment nvarchar(max)");

                bool isColumnShipPublicAddrCommentExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "ShipPublicAddrComment");
                if (!isColumnShipPublicAddrCommentExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD ShipPublicAddrComment nvarchar(max)");

                bool isColumnAsbestosPlanExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "AsbestosPlan");
                if (!isColumnAsbestosPlanExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD AsbestosPlan nvarchar(max)");

                bool isColumnBioMPRExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "BioMPR");
                if (!isColumnBioMPRExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD BioMPR nvarchar(max)");

                bool isColumnNoiseVibrationFileExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "NoiseVibrationFile");
                if (!isColumnNoiseVibrationFileExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD NoiseVibrationFile nvarchar(max)");

                bool isColumnPREExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "PRE");
                if (!isColumnPREExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD PRE nvarchar(max)");

                bool isColumnBioRPWHExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "BioRPWH");
                if (!isColumnBioRPWHExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD BioRPWH nvarchar(max)");

                bool isColumnRPWaterHandbookExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "RPWaterHandbook");
                if (!isColumnRPWaterHandbookExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD RPWaterHandbook nvarchar(max)");
                
                //RDBJ 11/02/2021
                bool isColumnPCOPEPExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "PCOPEP");
                if (!isColumnPCOPEPExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD PCOPEP nvarchar(max)");

                bool isColumnINTVRPExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "INTVRP");
                if (!isColumnINTVRPExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD INTVRP bit");

                bool isColumnIBulkCargoExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "IBulkCargo");
                if (!isColumnIBulkCargoExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD IBulkCargo bit");

                bool isColumnIVGPExist = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "IVGP");
                if (!isColumnIVGPExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD IVGP bit");
                //End RDBJ 11/02/2021

                // RDBJ 01/05/2022
                bool isColumnisDelete = Utility.CheckTableColumnExist(AppStatic.GeneralInspectionReport, "isDelete");
                if (!isColumnisDelete)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GeneralInspectionReport + " ADD [isDelete] int DEFAULT 0 NOT NULL");
                // End RDBJ 01/05/2022

                //End Table GeneralInspectionReport
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableGeneralInspectionReportModification : \n" + ex.Message);
            }
        }
        //End //RDBJ 10/31/2021
        //RDBJ //RDBJ 10/31/2021
        public static void TableGIRDeficienciesAndFilesModification()
        {
            try
            {
                //Table GIRDeficiencies
                bool isColumnDelete = Utility.CheckTableColumnExist(AppStatic.GIRDeficiencies, "isDelete");
                if (!isColumnDelete)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficiencies + " ADD isDelete int");

                bool isColumnPriority = Utility.CheckTableColumnExist(AppStatic.GIRDeficiencies, "Priority");
                if (!isColumnPriority)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficiencies + " ADD Priority int");

                bool isColumnItemNoExist = Utility.CheckTableColumnExist(AppStatic.GIRDeficiencies, "ItemNo");
                if (!isColumnItemNoExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficiencies + " ADD ItemNo nvarchar(150)");

                bool isColumnSectionExist = Utility.CheckTableColumnExist(AppStatic.GIRDeficiencies, "Section");
                if (!isColumnSectionExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficiencies + " ADD Section nvarchar(500)");

                // RDBJ 12/18/2021
                bool isColumnAssignTo = Utility.CheckTableColumnExist(AppStatic.GIRDeficiencies, "AssignTo");
                if (!isColumnAssignTo)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficiencies + " ADD AssignTo uniqueidentifier");
                // End RDBJ 12/18/2021

                // RDBJ 03/01/2022
                bool isColumnDueDate = Utility.CheckTableColumnExist(AppStatic.GIRDeficiencies, "DueDate");
                if (!isColumnDueDate)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficiencies + " ADD DueDate datetime");
                // End RDBJ 03/01/2022

                // JSL 06/04/2022
                bool isColumnDeficienciesFileUniqueID = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesFile, "DeficienciesFileUniqueID");
                if (!isColumnDeficienciesFileUniqueID)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesFile + " ADD DeficienciesFileUniqueID uniqueidentifier NOT NULL DEFAULT NEWID() with values");
                // End JSL 06/04/2022

                //Table GIRDeficiencies
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableGIRDeficienciesAndFilesModification : \n" + ex.Message);
            }
        }
        //End RDBJ //RDBJ 10/31/2021

        //RDBJ 10/02/2021
        public static void TableGIRDeficienciesNotesAndFilesModification()
        {
            try
            {
                //Table GIRDeficienciesNotes
                bool isColumnNoteUniqueID = Utility.CheckTableColumnExist(AppStatic.DeficienciesNote, "NoteUniqueID");
                if (!isColumnNoteUniqueID)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.DeficienciesNote + " ADD NoteUniqueID uniqueidentifier");
                //End Table GIRDeficienciesNotes

                //Table GIRDeficienciesCommentFile
                bool isColumnNoteUniqueIDInGIRDeficienciesCommentFile = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesCommentFile, "NoteUniqueID");
                if (!isColumnNoteUniqueIDInGIRDeficienciesCommentFile)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesCommentFile + " ADD NoteUniqueID uniqueidentifier");

                bool isColumnFileUniqueIDInGIRDeficienciesCommentFile = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesCommentFile, "CommentFileUniqueID");
                if (!isColumnFileUniqueIDInGIRDeficienciesCommentFile)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesCommentFile + " ADD CommentFileUniqueID uniqueidentifier");
                //End Table GIRDeficienciesCommentFile
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableGIRDeficienciesNotesModification : \n" + ex.Message);
            }
        }
        //End RDBJ 10/02/2021

        //RDBJ 10/02/2021
        public static void TableGIRDeficienciesInitialActionsAndFilesModification()
        {
            try
            {
                //Table GIRDeficienciesInitialActions
                bool isColumnIniActUniqueID = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesInitialActions, "IniActUniqueID");
                if (!isColumnIniActUniqueID)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesInitialActions + " ADD IniActUniqueID uniqueidentifier");
                //End Table GIRDeficienciesInitialActions

                //Table GIRDeficienciesInitialActionsFile
                bool isColumnIniActFileUniqueIDInGIRDeficienciesInitialActionsFile = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesInitialActionsFile, "IniActUniqueID");
                if (!isColumnIniActFileUniqueIDInGIRDeficienciesInitialActionsFile)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesInitialActionsFile + " ADD IniActUniqueID uniqueidentifier");

                bool isColumnFileUniqueIDInGIRDeficienciesInitialActionsFile = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesInitialActionsFile, "IniActFileUniqueID");
                if (!isColumnFileUniqueIDInGIRDeficienciesInitialActionsFile)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesInitialActionsFile + " ADD IniActFileUniqueID uniqueidentifier");
                //End Table GIRDeficienciesInitialActionsFile
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableGIRDeficienciesInitialActionsAndFilesModification : \n" + ex.Message);
            }
        }
        //End RDBJ 10/02/2021

        //RDBJ 10/02/2021
        public static void TableGIRGIRDeficienciesResolutionAndFilesModification()
        {
            try
            {
                //Table GIRDeficienciesResolution
                bool isColumnResolutionUniqueID = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesResolution, "ResolutionUniqueID");
                if (!isColumnResolutionUniqueID)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesResolution + " ADD ResolutionUniqueID uniqueidentifier");
                //End Table GIRDeficienciesResolution

                //Table GIRDeficienciesResolutionFile
                bool isColumnResolutionUniqueIDInGIRDeficienciesResolutionFile = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesResolutionFile, "ResolutionUniqueID");
                if (!isColumnResolutionUniqueIDInGIRDeficienciesResolutionFile)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesResolutionFile + " ADD ResolutionUniqueID uniqueidentifier");

                bool isColumnFileUniqueIDInGIRDeficienciesResolutionFile = Utility.CheckTableColumnExist(AppStatic.GIRDeficienciesResolutionFile, "ResolutionFileUniqueID");
                if (!isColumnFileUniqueIDInGIRDeficienciesResolutionFile)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.GIRDeficienciesResolutionFile + " ADD ResolutionFileUniqueID uniqueidentifier");
                //End Table GIRDeficienciesResolutionFile
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableGIRGIRDeficienciesResolutionAndFilesModification : \n" + ex.Message);
            }
        }
        //End RDBJ 10/02/2021

        // RDBJ 01/05/2022
        public static void TableSuperintendedInspectionReportModification()
        {
            // Table SuperintendedInspectionReport

            // RDBJ 02/15/2022
            bool isColumnSection9_16_Condition = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section9_16_Condition");
            if (!isColumnSection9_16_Condition)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section9_16_Condition NVARCHAR(MAX) NULL");

            bool isColumnSection9_16_Comment = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section9_16_Comment");
            if (!isColumnSection9_16_Comment)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section9_16_Comment NVARCHAR(MAX) NULL");

            bool isColumnSection9_17_Condition = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section9_17_Condition");
            if (!isColumnSection9_17_Condition)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section9_17_Condition NVARCHAR(MAX) NULL");

            bool isColumnSection9_17_Comment = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section9_17_Comment");
            if (!isColumnSection9_17_Comment)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section9_17_Comment NVARCHAR(MAX) NULL");

            bool isColumnSection18_8_Condition = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section18_8_Condition");
            if (!isColumnSection18_8_Condition)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section18_8_Condition NVARCHAR(MAX) NULL");

            bool isColumnSection18_8_Comment = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section18_8_Comment");
            if (!isColumnSection18_8_Comment)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section18_8_Comment NVARCHAR(MAX) NULL");

            bool isColumnSection18_9_Condition = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section18_9_Condition");
            if (!isColumnSection18_9_Condition)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section18_9_Condition NVARCHAR(MAX) NULL");

            bool isColumnSection18_9_Comment = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "Section18_9_Comment");
            if (!isColumnSection18_9_Comment)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD Section18_9_Comment NVARCHAR(MAX) NULL");
            // End RDBJ 02/15/2022

            // RDBJ 01/05/2022
            bool isColumnisDelete = Utility.CheckTableColumnExist(AppStatic.SuperintendedInspectionReport, "isDelete");
            if (!isColumnisDelete)
                Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SuperintendedInspectionReport + " ADD [isDelete] int DEFAULT 0 NOT NULL");
            // End RDBJ 01/05/2022

            //End Table SuperintendedInspectionReport
        }
        // End RDBJ 01/05/2022

        // RDBJ 04/01/2022
        public static void TableSIRNotesModification()
        {
            try
            {
                bool isColumnIsDeletedExist = Utility.CheckTableColumnExist(AppStatic.SIRNotes, "IsDeleted");   // JSL 05/18/2022 Added SIRNotes table name
                if (!isColumnIsDeletedExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SIRNotes + " ADD IsDeleted int NOT NULL DEFAULT(0)");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableSIRNotesModification : \n" + ex.Message);
            }
            
        }
        // End RDBJ 04/01/2022

        // RDBJ 04/01/2022
        public static void TableSIRAdditionalNotesModification()
        {
            try
            {
                bool isColumnIsDeletedExist = Utility.CheckTableColumnExist(AppStatic.SIRAdditionalNotes, "IsDeleted"); // JSL 05/18/2022 Added SIRAdditionalNotes table name
                if (!isColumnIsDeletedExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.SIRAdditionalNotes + " ADD IsDeleted int NOT NULL DEFAULT(0)");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableSIRNotesModification : \n" + ex.Message);
            }
        }
        // End RDBJ 04/01/2022

        //RDBJ 11/25/2021
        public static void TableInternalAuditFormModification()
        {
            try
            {
                //Table InternalAuditForm
                bool isColumnAuditTypeExist = Utility.CheckTableColumnExist(AppStatic.InternalAuditForm, "AuditType");
                if (!isColumnAuditTypeExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.InternalAuditForm + " ADD AuditType int NOT NULL DEFAULT(1)");

                bool isColumnIsClosedExist = Utility.CheckTableColumnExist(AppStatic.InternalAuditForm, "IsClosed");
                if (!isColumnIsClosedExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.InternalAuditForm + " ADD IsClosed bit DEFAULT 0 NOT NULL");

                bool isColumnIsAdditionalExist = Utility.CheckTableColumnExist(AppStatic.InternalAuditForm, "IsAdditional");
                if (!isColumnIsAdditionalExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.InternalAuditForm + " ADD IsAdditional bit DEFAULT 0 NOT NULL");

                bool isColumnisDeleteExist = Utility.CheckTableColumnExist(AppStatic.InternalAuditForm, "isDelete");
                if (!isColumnisDeleteExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.InternalAuditForm + " ADD isDelete int NOT NULL DEFAULT(0)");

                // RDBJ 01/31/2022
                bool isColumnSavedAsDraftExist = Utility.CheckTableColumnExist(AppStatic.InternalAuditForm, "SavedAsDraft");
                if (!isColumnSavedAsDraftExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.InternalAuditForm + " ADD SavedAsDraft BIT NOT NULL DEFAULT 0;");
                // End RDBJ 01/31/2022

                //End Table InternalAuditForm
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableInternalAuditFormModification : \n" + ex.Message);
            }
        }
        //End RDBJ 11/25/2021

        //RDBJ 10/31/2021
        public static void TableIAFAuditNotesAndFileModification()
        {
            try
            {
                //Table AuditNotes
                bool isColumnDateClosedExist = Utility.CheckTableColumnExist(AppStatic.AuditNotes, "DateClosed");
                if (!isColumnDateClosedExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotes + " ADD DateClosed datetime");

                bool isColumnIsResolvedExist = Utility.CheckTableColumnExist(AppStatic.AuditNotes, "IsResolved");
                if (!isColumnIsResolvedExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotes + " ADD IsResolved bit");

                bool isColumnPriorityExist = Utility.CheckTableColumnExist(AppStatic.AuditNotes, "Priority");
                if (!isColumnPriorityExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotes + " ADD Priority int NOT NULL DEFAULT(12)");

                bool isColumnisDeleteExist = Utility.CheckTableColumnExist(AppStatic.AuditNotes, "isDelete");
                if (!isColumnisDeleteExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotes + " ADD isDelete int NOT NULL DEFAULT(0)");

                // RDBJ 12/21/2021
                bool isColumnAssignTo = Utility.CheckTableColumnExist(AppStatic.AuditNotes, "AssignTo");
                if (!isColumnAssignTo)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotes + " ADD AssignTo uniqueidentifier");
                // End RDBJ 12/21/2021

                //End Table AuditNotes

                //Table AuditNotesAttachment
                bool isColumnNotesFileUniqueIDFileExist = Utility.CheckTableColumnExist(AppStatic.AuditNotesAttachment, "NotesFileUniqueID");
                if (!isColumnNotesFileUniqueIDFileExist)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotesAttachment + " ADD NotesFileUniqueID uniqueidentifier");
                //Table AuditNotesAttachment
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableIAFNotesCommnetsAndFileModification : \n" + ex.Message);
            }
        }
        //End RDBJ //RDBJ 10/31/2021

        //RDBJ 10/02/2021
        public static void TableIAFNotesCommnetsAndFileModification()
        {
            try
            {
                //Table AuditNotesComment
                bool isColumnCommentUniqueIDInAuditNoteComment = Utility.CheckTableColumnExist(AppStatic.AuditNotesComment, "CommentUniqueID");
                if (!isColumnCommentUniqueIDInAuditNoteComment)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotesComment + " ADD CommentUniqueID uniqueidentifier");
                //End Table AuditNotesComment

                //Table AuditNotesCommentFile
                bool isColumnCommentUniqueIDInAuditNotesCommentFile = Utility.CheckTableColumnExist(AppStatic.AuditNotesCommentFile, "CommentUniqueID");
                if (!isColumnCommentUniqueIDInAuditNotesCommentFile)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotesCommentFile + " ADD CommentUniqueID uniqueidentifier");

                bool isColumnCommentUniqueIDInAuditNotesCommentFileUniqueID = Utility.CheckTableColumnExist(AppStatic.AuditNotesCommentFile, "CommentFileUniqueID");
                if (!isColumnCommentUniqueIDInAuditNotesCommentFileUniqueID)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotesCommentFile + " ADD CommentFileUniqueID uniqueidentifier");
                //End Table AuditNotesCommentFile
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableIAFNotesCommnetsAndFileModification : \n" + ex.Message);
            }
        }
        //End RDBJ 10/02/2021

        //RDBJ 10/05/2021
        public static void TableIAFNotesResolutionsAndFileModification()
        {
            try
            {
                //Table [AuditNotesResolution]
                bool isColumnResolutionUniqueIDInAuditNotesResolution = Utility.CheckTableColumnExist(AppStatic.AuditNotesResolution, "ResolutionUniqueID");
                if (!isColumnResolutionUniqueIDInAuditNotesResolution)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotesResolution + " ADD ResolutionUniqueID uniqueidentifier");
                //End Table [AuditNotesResolution]

                //Table [AuditNotesResolutionFiles]
                bool isColumnResolutionUniqueIDInAuditNotesResolutionFiles = Utility.CheckTableColumnExist(AppStatic.AuditNotesResolutionFiles, "ResolutionUniqueID");
                if (!isColumnResolutionUniqueIDInAuditNotesResolutionFiles)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotesResolutionFiles + " ADD ResolutionUniqueID uniqueidentifier");

                bool isColumnResolutionFileUniqueIDInAuditNotesResolutionFiles = Utility.CheckTableColumnExist(AppStatic.AuditNotesResolutionFiles, "ResolutionFileUniqueID");
                if (!isColumnResolutionFileUniqueIDInAuditNotesResolutionFiles)
                    Utility.ExecuteQuery("ALTER TABLE " + AppStatic.AuditNotesResolutionFiles + " ADD ResolutionFileUniqueID uniqueidentifier");
                //End Table [AuditNotesResolutionFiles]
            }
            catch (Exception ex)
            {
                LogHelper.writelog("TableIAFNotesCommnetsAndFileModification : \n" + ex.Message);
            }
        }
        //End RDBJ 10/05/2021

        //RDBJ 10/25/2021
        public static bool StoredProcedure_ExistOrNot(string sp_Name)
        {
            bool blnIsExist = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string IsSPExistQuery = "select * from sysobjects where type='P' and name='" + sp_Name + "'";
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection conn = new SqlConnection(connetionString);
                    SqlCommand command = new SqlCommand(IsSPExistQuery, conn);
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            blnIsExist = true;
                            break;
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StoredProcedure_ExistOrNot : " + ex.Message);
            }
            return blnIsExist;
        }
        //End RDBJ 10/25/2021

        //RDBJ 10/25/2021
        public static bool CreateIfNotExistStoredProcedure(string sp_query)
        {
            bool blnResponse = false;
            try
            {
                ServerConnectModal dbConnModal = ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection conn = new SqlConnection(connetionString);
                    SqlCommand command = new SqlCommand(sp_query, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CreateIfNotExistStoredProcedure : " + ex.Message);
            }
            return blnResponse;
        }
        //End RDBJ 10/25/2021

        //RDBJ 11/08/2021
        public static void GetUserProfileDataAndSaveInSMTPServerConfigFile()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                List<string> emails = _helper.GetEmailFromUserProfileTableWhereTechnicalAndISMGroup();
                WriteSMTPServerConfigJson(emails);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetUserProfileDataAndSaveInSMTPServerConfigFile : \n" + ex.Message);
            }
        }
        //End RDBJ 11/08/2021

        //RDBJ 11/08/2021
        public static void WriteSMTPServerConfigJson(List<string> CCEmails)
        {
            try
            {
                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["TechnicalAndISMGroupEmailsList"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }

                jsonText = System.IO.File.ReadAllText(jsonFilePath);
                if (!string.IsNullOrEmpty(jsonText))
                {
                    SMTPServerModal Modal = JsonConvert.DeserializeObject<SMTPServerModal>(jsonText);

                    foreach (var newEmails in CCEmails)
                    {
                        if (!Modal.CCEmail.Contains(newEmails))
                        {
                            Modal.CCEmail.Add(newEmails);
                        }
                    }

                    jsonText = JsonConvert.SerializeObject(Modal, Formatting.Indented);
                    System.IO.File.WriteAllText(jsonFilePath, jsonText);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ReadAndWriteSMTPServerConfigJson Error : " + ex.Message);
            }
        }
        //End RDBJ 11/08/2021

        // RDBJ 02/24/2022
        public static void GetMainSyncServiceDataAndSaveInMainSyncServiceFile(
                bool blnUpdateServiceVersion = false  // JSL 11/12/2022
            )
        {
            APIHelper _helper = new APIHelper();
            Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            dictMetaData["strAction"] = AppStatic.API_GETMAINSYNCSERVICESETTINGS;

            retDictMetaData = _helper.PostAsyncAPICall(AppStatic.APISettings, AppStatic.API_CommonPostAPICall, dictMetaData);

            // JSL 06/27/2022 wrapped in if
            if (retDictMetaData != null && retDictMetaData.Count > 0)
            {
                WriteMainSyncServiceIntervalTimeConfigJson(retDictMetaData
                    , blnUpdateServiceVersion   // JSL 11/12/2022
                    );
            }

            //return retDictMetaData; // RDBJ 02/26/2022
        }
        // End RDBJ 02/24/2022

        // RDBJ 02/24/2022
        public static void WriteMainSyncServiceIntervalTimeConfigJson(Dictionary<string, string> dictMetaData
            , bool blnUpdateServiceVersion = false  // JSL 11/12/2022
            )
        {
            try
            {
                string IntervalTime = string.Empty;
                string UseServerTimeInterval = string.Empty;
                string UpdatedBy = string.Empty;
                string UpdatedDate = string.Empty;
                string MainSyncServiceVersion = string.Empty;

                if (dictMetaData.ContainsKey("IntervalTime"))
                    IntervalTime = dictMetaData["IntervalTime"].ToString();

                if (dictMetaData.ContainsKey("UseServerTimeInterval"))
                    UseServerTimeInterval = dictMetaData["UseServerTimeInterval"].ToString().ToLower();

                if (dictMetaData.ContainsKey("UpdatedBy"))
                    UpdatedBy = dictMetaData["UpdatedBy"].ToString();

                if (dictMetaData.ContainsKey("UpdatedDate"))
                    UpdatedDate = dictMetaData["UpdatedDate"].ToString();

                if (dictMetaData.ContainsKey("MainSyncServiceVersion"))
                    MainSyncServiceVersion = dictMetaData["MainSyncServiceVersion"].ToString();

                dictMetaData.Remove("Status");

                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["MainSyncServiceIntervalTime"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }

                jsonText = System.IO.File.ReadAllText(jsonFilePath);
                Dictionary<string, object> fileDictMetaData = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(jsonText))
                {
                    fileDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);

                    Dictionary<string, string> LocalSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["LocalSettings"].ToString());
                    Dictionary<string, string> ServerSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["ServerSettings"].ToString());

                    // JSL 06/27/2022 wrapped in if
                    if (!string.IsNullOrEmpty(IntervalTime))
                        ServerSettings["IntervalTime"] = IntervalTime;

                    // JSL 06/27/2022 wrapped in if
                    if (!string.IsNullOrEmpty(UseServerTimeInterval))
                        ServerSettings["UseServerTimeInterval"] = UseServerTimeInterval;

                    // JSL 06/27/2022 wrapped in if
                    if (!string.IsNullOrEmpty(UpdatedBy))
                        ServerSettings["UpdatedBy"] = UpdatedBy;

                    // JSL 06/27/2022 wrapped in if
                    if (!string.IsNullOrEmpty(UpdatedDate))
                        ServerSettings["UpdatedDate"] = UpdatedDate;

                    // JSL 11/12/2022 wrapped in if
                    if (blnUpdateServiceVersion)
                    {
                        // JSL 06/27/2022 wrapped in if
                        if (!string.IsNullOrEmpty(MainSyncServiceVersion))
                            ServerSettings["MainSyncServiceVersion"] = MainSyncServiceVersion;

                        // JSL 06/27/2022 wrapped in if
                        if (!string.IsNullOrEmpty(MainSyncServiceVersion))
                            LocalSettings["MainSyncServiceVersion"] = MainSyncServiceVersion;  // JSL 06/24/2022
                    }
                    else
                    {
                        ServerSettings["MainSyncServiceVersion"] = ServerSettings["MainSyncServiceVersion"];
                        LocalSettings["MainSyncServiceVersion"] = LocalSettings["MainSyncServiceVersion"];  
                    }
                    // End JSL 11/12/2022 wrapped in if

                    fileDictMetaData["LocalSettings"] = LocalSettings;
                    fileDictMetaData["ServerSettings"] = ServerSettings;
                }
                else
                {
                    fileDictMetaData["LocalSettings"] = dictMetaData;
                    fileDictMetaData["ServerSettings"] = dictMetaData;
                }

                jsonText = JsonConvert.SerializeObject(fileDictMetaData, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonText);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("WriteMainSyncServiceIntervalTimeConfigJson Error : " + ex.Message);
            }
        }
        // End RDBJ 02/24/2022

        // JSL 11/12/2022
        public static bool DeleteDataFromDatabaseExceptCurrentShip(string strShipCode)
        {
            bool blnRetStatus = false;
            try
            {
                Delete_GIR_DataFromThisShipExceptCurrentShipData(strShipCode, AppStatic.GIRForm);
                Delete_SIR_DataFromThisShipExceptCurrentShipData(strShipCode, AppStatic.SIRForm);
                Delete_IAF_DataFromThisShipExceptCurrentShipData(strShipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteDataFromDatabaseExceptCurrentShip " + strShipCode + " Error : " + ex.InnerException.ToString());
            }
            return blnRetStatus;
        }
        // End JSL 11/12/2022

        // JSL 11/12/2022
        public static void Delete_GIR_DataFromThisShipExceptCurrentShipData(string strShipCode, string strFormType)
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            List<GIRModal> lstGISIForms = new List<GIRModal>();
            if (strFormType == "GI")
            {
                lstGISIForms = new List<GIRModal>();
                lstGISIForms = GetGIRFormsExceptCurrentShip(strShipCode);
            }
            
            foreach (var itemGI_SI in lstGISIForms)
            {
                foreach (var itemDeficiencies in itemGI_SI.GIRDeficiencies)
                {
                    // Delete comments file and comment
                    List<GIRDeficienciesNote> lstComments = new List<GIRDeficienciesNote>();
                    lstComments = itemDeficiencies.GIRDeficienciesComments;
                    foreach (var itemComment in lstComments)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.GIRDeficienciesCommentFile, "NoteUniqueID", Convert.ToString(itemComment.NoteUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.DeficienciesNote, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete comments file and comment

                    // Delete resolution file and resolution
                    List<GIRDeficienciesResolution> lstResolutions = new List<GIRDeficienciesResolution>();
                    lstResolutions = itemDeficiencies.GIRDeficienciesResolution;
                    foreach (var itemResolution in lstResolutions)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.GIRDeficienciesResolutionFile, "ResolutionUniqueID", Convert.ToString(itemResolution.ResolutionUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.GIRDeficienciesResolution, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete resolution file and resolution

                    // Delete initial file and initial
                    List<GIRDeficienciesInitialActions> lstIniActions = new List<GIRDeficienciesInitialActions>();
                    lstIniActions = itemDeficiencies.GIRDeficienciesInitialActions;
                    foreach (var itemIniAction in lstIniActions)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.GIRDeficienciesInitialActionsFile, "IniActFileUniqueID", Convert.ToString(itemIniAction.IniActUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.GIRDeficienciesInitialActions, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete initial file and initial

                    // Delete Def. Files
                    DeleteRecords(AppStatic.GIRDeficienciesFile, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete Def. Files
                }

                // Delete Def.
                DeleteRecords(AppStatic.GIRDeficiencies, "UniqueFormID", Convert.ToString(itemGI_SI.GeneralInspectionReport.UniqueFormID));
                // End Delete Def.

                // Delete Others table data for GI
                if (strFormType == "GI")
                {
                    // Delete SafeManningRequirements
                    DeleteRecords(AppStatic.GlRSafeManningRequirements, "UniqueFormID", Convert.ToString(itemGI_SI.GeneralInspectionReport.UniqueFormID));
                    // End Delete SafeManningRequirements

                    // Delete CrewDocuments
                    DeleteRecords(AppStatic.GlRCrewDocuments, "UniqueFormID", Convert.ToString(itemGI_SI.GeneralInspectionReport.UniqueFormID));
                    // End Delete CrewDocuments

                    // Delete RestandWorkHours
                    DeleteRecords(AppStatic.GIRRestandWorkHours, "UniqueFormID", Convert.ToString(itemGI_SI.GeneralInspectionReport.UniqueFormID));
                    // End Delete RestandWorkHours

                    // Delete GIRPhotos
                    DeleteRecords(AppStatic.GIRPhotographs, "UniqueFormID", Convert.ToString(itemGI_SI.GeneralInspectionReport.UniqueFormID));
                    // End Delete GIRPhotos
                }
                // End Delete Others table data for GI

                // Delete GI_Form
                if (strFormType == "GI")
                {
                    DeleteRecords(AppStatic.GeneralInspectionReport, "UniqueFormID", Convert.ToString(itemGI_SI.GeneralInspectionReport.UniqueFormID));
                }
                // End Delete GI_Form
            }
        }
        // End JSL 11/12/2022

        public static void Delete_SIR_DataFromThisShipExceptCurrentShipData(string strShipCode, string strFormType)
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            List<SIRModal> lstGISIForms = new List<SIRModal>();
            if (strFormType == "SI")
            {
                lstGISIForms = new List<SIRModal>();
                lstGISIForms = GetSIRFormsExceptCurrentShip(strShipCode);
            }

            foreach (var itemGI_SI in lstGISIForms)
            {
                foreach (var itemDeficiencies in itemGI_SI.GIRDeficiencies)
                {
                    // Delete comments file and comment
                    List<GIRDeficienciesNote> lstComments = new List<GIRDeficienciesNote>();
                    lstComments = itemDeficiencies.GIRDeficienciesComments;
                    foreach (var itemComment in lstComments)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.GIRDeficienciesCommentFile, "NoteUniqueID", Convert.ToString(itemComment.NoteUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.DeficienciesNote, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete comments file and comment

                    // Delete resolution file and resolution
                    List<GIRDeficienciesResolution> lstResolutions = new List<GIRDeficienciesResolution>();
                    lstResolutions = itemDeficiencies.GIRDeficienciesResolution;
                    foreach (var itemResolution in lstResolutions)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.GIRDeficienciesResolutionFile, "ResolutionUniqueID", Convert.ToString(itemResolution.ResolutionUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.GIRDeficienciesResolution, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete resolution file and resolution

                    // Delete initial file and initial
                    List<GIRDeficienciesInitialActions> lstIniActions = new List<GIRDeficienciesInitialActions>();
                    lstIniActions = itemDeficiencies.GIRDeficienciesInitialActions;
                    foreach (var itemIniAction in lstIniActions)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.GIRDeficienciesInitialActionsFile, "IniActFileUniqueID", Convert.ToString(itemIniAction.IniActUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.GIRDeficienciesInitialActions, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete initial file and initial

                    // Delete Def. Files
                    DeleteRecords(AppStatic.GIRDeficienciesFile, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                    // End Delete Def. Files
                }

                // Delete Def.
                DeleteRecords(AppStatic.GIRDeficiencies, "UniqueFormID", Convert.ToString(itemGI_SI.SuperintendedInspectionReport.UniqueFormID));
                // End Delete Def.

                // Delete Others table data for SI
                if (strFormType == "SI")
                {
                    // Delete SIRNotes
                    DeleteRecords(AppStatic.SIRNotes, "UniqueFormID", Convert.ToString(itemGI_SI.SuperintendedInspectionReport.UniqueFormID));
                    // End Delete SIRNotes

                    // Delete SIRAdditionalNotes
                    DeleteRecords(AppStatic.SIRAdditionalNotes, "UniqueFormID", Convert.ToString(itemGI_SI.SuperintendedInspectionReport.UniqueFormID));
                    // End Delete SIRAdditionalNotes
                }
                // End Delete Others table data for SI

                // Delete SI_Form
                if (strFormType == "SI")
                {
                    DeleteRecords(AppStatic.SuperintendedInspectionReport, "UniqueFormID", Convert.ToString(itemGI_SI.SuperintendedInspectionReport.UniqueFormID));
                }
                // End Delete SI_Form
            }
        }

        // JSL 11/12/2022
        public static void Delete_IAF_DataFromThisShipExceptCurrentShipData(string strShipCode)
        {
            List<IAF> lstAuditList = GetIAFFormsExceptCurrentShip(strShipCode);
            foreach (var modalIAF in lstAuditList)
            {
                foreach (var itemNote in modalIAF.AuditNote)
                {
                    // Delete comments file and comment
                    List<Audit_Deficiency_Comments> lstComments = new List<Audit_Deficiency_Comments>();
                    lstComments = itemNote.AuditNotesComment;
                    foreach (var itemComment in lstComments)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.AuditNotesCommentFile, "CommentUniqueID", Convert.ToString(itemComment.CommentUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.AuditNotesComment, "NotesUniqueID", Convert.ToString(itemNote.NotesUniqueID));
                    // End Delete comments file and comment

                    // Delete resolution file and resolution
                    List<Audit_Note_Resolutions> lstResolutions = new List<Audit_Note_Resolutions>();
                    lstResolutions = itemNote.AuditNotesResolution;

                    foreach (var itemResolution in lstResolutions)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.AuditNotesResolutionFiles, "ResolutionUniqueID", Convert.ToString(itemResolution.ResolutionUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.AuditNotesResolution, "NotesUniqueID", Convert.ToString(itemNote.NotesUniqueID));
                    // End Delete resolution file and resolution

                    // Delete AuditNote. Files
                    DeleteRecords(AppStatic.AuditNotesAttachment, "NotesUniqueID", Convert.ToString(itemNote.NotesUniqueID));
                    // End Delete AuditNote. Files
                }

                // Delete AuditNote.
                DeleteRecords(AppStatic.AuditNotes, "UniqueFormID", Convert.ToString(modalIAF.InternalAuditForm.UniqueFormID));
                // End Delete AuditNote.

                // Delete IA_Form
                DeleteRecords(AppStatic.InternalAuditForm, "UniqueFormID", Convert.ToString(modalIAF.InternalAuditForm.UniqueFormID));
                // End Delete IA_Form
            }
        }
        // End JSL 11/12/2022

        // JSL 11/12/2022
        public static List<GIRModal> GetGIRFormsExceptCurrentShip(string strShipCode)  
        {
            List<GIRModal> SyncList = new List<GIRModal>(); 
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GeneralInspectionReport + " WHERE Ship != '" + strShipCode + "' AND UniqueFormID IS NOT NULL", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<GeneralInspectionReport> GIRSyncList = dt.ToListof<GeneralInspectionReport>();  
                            foreach (var item in GIRSyncList)   
                            {
                                try
                                {
                                    GIRModal Modal = new GIRModal();
                                    Modal.GeneralInspectionReport = item;

                                    DataTable dtCrewDocuments = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GlRCrewDocuments + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                    sqlAdp.Fill(dtCrewDocuments);
                                    Modal.GeneralInspectionReport.GIRCrewDocuments = dtCrewDocuments.ToListof<GlRCrewDocuments>();

                                    DataTable dtSafeManningRequirements = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GlRSafeManningRequirements + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                    sqlAdp.Fill(dtSafeManningRequirements);
                                    Modal.GeneralInspectionReport.GIRSafeManningRequirements = dtSafeManningRequirements.ToListof<GlRSafeManningRequirements>();

                                    DataTable dtRestandWorkHours = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRRestandWorkHours + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                    sqlAdp.Fill(dtRestandWorkHours);
                                    Modal.GeneralInspectionReport.GIRRestandWorkHours = dtRestandWorkHours.ToListof<GIRRestandWorkHours>();

                                    DataTable dtPhotographs = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRPhotographs + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                    sqlAdp.Fill(dtPhotographs);
                                    Modal.GeneralInspectionReport.GIRPhotographs = dtPhotographs.ToListof<GIRPhotographs>();

                                    DataTable dtDeficiencies = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE UniqueFormID = '" + item.UniqueFormID + "' AND ReportType = 'GI'", conn);
                                    sqlAdp.Fill(dtDeficiencies);
                                    Modal.GIRDeficiencies = dtDeficiencies.ToListof<GIRDeficiencies>();

                                    if (Modal.GIRDeficiencies != null && Modal.GIRDeficiencies.Count > 0)
                                    {
                                        foreach (var def in Modal.GIRDeficiencies)
                                        {
                                            DataTable dtDeficienciesFiles = new DataTable();
                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesFile + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                            sqlAdp.Fill(dtDeficienciesFiles);
                                            def.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                            if (def.GIRDeficienciesFile != null && def.GIRDeficienciesFile.Count > 0)
                                            {
                                                foreach (var deffile in def.GIRDeficienciesFile)
                                                {
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
                                                }
                                            }
                                        }
                                    }
                                    SyncList.Add(Modal);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("GetGIRFormsExceptCurrentShip Error : " + item.GIRFormID + " " + ex.Message);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRFormsExceptCurrentShip " + ex.Message);
            }
            return SyncList;
        }
        // End JSL 11/12/2022

        // JSL 11/12/2022
        public static List<SIRModal> GetSIRFormsExceptCurrentShip(string strShipCode)
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
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SuperintendedInspectionReport + " WHERE ShipName != '" + strShipCode + "' AND UniqueFormID IS NOT NULL", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<SuperintendedInspectionReport> SIRSyncList = dt.ToListof<SuperintendedInspectionReport>();
                            foreach (var item in SIRSyncList)
                            {
                                try
                                {
                                    SIRModal Modal = new SIRModal();
                                    Modal.SuperintendedInspectionReport = item;

                                    DataTable dtNotes = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SIRNotes + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                    sqlAdp.Fill(dtNotes);
                                    Modal.SIRNote = dtNotes.ToListof<SIRNote>();

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
                                            DataTable dtDeficienciesFiles = new DataTable();
                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesFile + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                            sqlAdp.Fill(dtDeficienciesFiles);
                                            def.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                            if (def.GIRDeficienciesFile != null && def.GIRDeficienciesFile.Count > 0)
                                            {
                                                foreach (var deffile in def.GIRDeficienciesFile)
                                                {
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
                                                }
                                            }

                                            DataTable dtDeficienciesInitialAction = new DataTable();
                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesInitialActions + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                            sqlAdp.Fill(dtDeficienciesInitialAction);
                                            def.GIRDeficienciesInitialActions = dtDeficienciesInitialAction.ToListof<GIRDeficienciesInitialActions>();
                                            if (def.GIRDeficienciesInitialActions != null && def.GIRDeficienciesInitialActions.Count > 0)
                                            {
                                                foreach (var defComment in def.GIRDeficienciesInitialActions)
                                                {
                                                    DataTable dtDeficienciesInitActionFiles = new DataTable();
                                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesInitialActionsFile + " WHERE IniActUniqueID = '" + defComment.IniActUniqueID + "'", conn);
                                                    sqlAdp.Fill(dtDeficienciesInitActionFiles);
                                                    defComment.GIRDeficienciesInitialActionsFiles = dtDeficienciesInitActionFiles.ToListof<GIRDeficienciesInitialActionsFile>();
                                                }
                                            }

                                            DataTable dtDeficienciesResolution = new DataTable();
                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesResolution + " WHERE DeficienciesUniqueID = '" + def.DeficienciesUniqueID + "'", conn);
                                            sqlAdp.Fill(dtDeficienciesResolution);
                                            def.GIRDeficienciesResolution = dtDeficienciesResolution.ToListof<GIRDeficienciesResolution>();
                                            if (def.GIRDeficienciesResolution != null && def.GIRDeficienciesResolution.Count > 0)
                                            {
                                                foreach (var defComment in def.GIRDeficienciesResolution)
                                                {
                                                    DataTable dtDeficienciesResolutionFile = new DataTable();
                                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesResolutionFile + " WHERE ResolutionUniqueID = '" + defComment.ResolutionUniqueID + "'", conn);
                                                    sqlAdp.Fill(dtDeficienciesResolutionFile);
                                                    defComment.GIRDeficienciesResolutionFiles = dtDeficienciesResolutionFile.ToListof<GIRDeficienciesResolutionFile>();
                                                }
                                            }
                                        }
                                    }
                                    SyncList.Add(Modal);
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("GetSIRFormsExceptCurrentShip Error : " + item.UniqueFormID + " " + ex.Message);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRFormsExceptCurrentShip " + ex.Message);
            }
            return SyncList;
        }
        // End JSL 11/12/2022

        // JSL 11/12/2022
        public static List<IAF> GetIAFFormsExceptCurrentShip(string strShipCode)
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
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.InternalAuditForm + " WHERE ShipName != '" + strShipCode + "' AND UniqueFormID IS NOT NULL", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            try
                            {
                                AuditFormsList = dt.ToListof<InternalAuditForm>();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.writelog("GetIAFFormsExceptCurrentShip List " + ex.Message);
                            }
                            foreach (InternalAuditForm item in AuditFormsList)
                            {
                                IAF iafModal = new IAF();   
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
                                            note.AuditNotesAttachment = new List<AuditNotesAttachment>();
                                            DataTable dtAuditNotesAttachs = new DataTable();
                                            sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesAttachment + " WHERE NotesUniqueID = '" + note.NotesUniqueID + "'", conn);
                                            sqlAdp.Fill(dtAuditNotesAttachs);
                                            note.AuditNotesAttachment = dtAuditNotesAttachs.ToListof<AuditNotesAttachment>();

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
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("GetIAFFormsExceptCurrentShip " + ex.Message);
                                }
                                IAFList.Add(iafModal);  
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIAFFormsExceptCurrentShip " + ex.Message);
                return null;
            }
            return IAFList;
        }
        // End JSL 11/12/2022

        // JSL 11/12/2022
        public static bool DeleteRecords(string tablename, string columnname, string RecID)
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
        // End JSL 11/12/2022

        // JSL 11/12/2022
        public static string ConvertIntoBase64EndCodedUploadedFile(string strFilePath)
        {
            string strReturnAttachmentBase64 = string.Empty;
            string strBasePath = System.Configuration.ConfigurationManager.AppSettings["ShipAPPLocalPath"];
            strBasePath = strBasePath.Replace("\\", "/") + "/Images/";

            string strFinalFilePath = Path.Combine(strBasePath, strFilePath);
            string contentType = MimeMapping.GetMimeMapping(strFinalFilePath);
            try
            {
                //Byte[] bytes = ReadAllBytes(strFinalFilePath).Result; // JSL 12/24/2022 commented
                // JSL 12/24/2022 wrapped in if
                Byte[] bytes = null;
                if (File.Exists(strFinalFilePath))
                {
                    bytes = System.IO.File.ReadAllBytes(@strFinalFilePath);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    strReturnAttachmentBase64 = "data:" + contentType + ";base64," + base64String;
                }
                // End JSL 12/24/2022 wrapped in if
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ConvertIntoBase64EndCodedUploadedFile Error : " + ex.InnerException.ToString());
            }
            return strReturnAttachmentBase64;
        }
        // End JSL 11/12/2022

        // JSL 11/12/2022
        public static string ConvertBase64IntoFile(Dictionary<string, string> dictFileData, bool blnIsGIRPhotos = false
            , bool blnIsNeedToDeleteExistingAttachment = true   // JSL 01/08/2023 set true   // JSL 12/24/2022
            )
        {
            string strReturnFilePath = string.Empty;
            string strBasePath = System.Configuration.ConfigurationManager.AppSettings["ShipAPPLocalPath"];
            strBasePath = strBasePath.Replace("\\", "/") + "/Images/";

            string UniqueFormID = string.Empty;
            string strReportType = string.Empty;
            string strDetailUniqueId = string.Empty;
            string strFileName = string.Empty;
            string strBase64String = string.Empty;

            // JSL 01/08/2023
            string strSubDetailType = string.Empty;
            string strSubDetailUniqueId = string.Empty;
            // End JSL 01/08/2023

            try
            {
                if (dictFileData != null && dictFileData.Count > 0)
                {
                    if (dictFileData.ContainsKey("UniqueFormID"))
                        UniqueFormID = dictFileData["UniqueFormID"];

                    if (dictFileData.ContainsKey("ReportType"))
                        strReportType = dictFileData["ReportType"];

                    if (dictFileData.ContainsKey("DetailUniqueId"))
                        strDetailUniqueId = dictFileData["DetailUniqueId"];

                    if (dictFileData.ContainsKey("FileName"))
                        strFileName = dictFileData["FileName"];

                    if (dictFileData.ContainsKey("Base64FileData"))
                        strBase64String = dictFileData["Base64FileData"];

                    // JSL 01/08/2023
                    if (dictFileData.ContainsKey("SubDetailType"))
                        strSubDetailType = dictFileData["SubDetailType"];

                    if (dictFileData.ContainsKey("SubDetailUniqueId"))
                        strSubDetailUniqueId = dictFileData["SubDetailUniqueId"];
                    // End JSL 01/08/2023
                }

                string strFileExtension = Path.GetExtension(strFileName);
                string strNewFileName = Path.GetFileNameWithoutExtension(strFileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + strFileExtension;

                string strStorePath = strReportType + @"/" + UniqueFormID;
                if (!blnIsGIRPhotos)
                {
                    strStorePath = strStorePath + @"/" + strDetailUniqueId;

                    // JSL 01/08/2023
                    if (!string.IsNullOrEmpty(strSubDetailUniqueId) && !string.IsNullOrEmpty(strSubDetailType))
                    {
                        strStorePath = strStorePath + @"/" + strSubDetailType + @"/" + strSubDetailUniqueId;
                    }
                    // End JSL 01/08/2023
                }

                // Delete existing files
                DirectoryInfo fileDirectory = new DirectoryInfo(Path.Combine(strBasePath, strStorePath));

                try
                {
                    // JSL 12/24/2022 wrapped in if
                    if (blnIsNeedToDeleteExistingAttachment)
                    {
                        // JSL 01/08/2023 wrapped in if
                        if (fileDirectory.Exists)
                        {
                            FileInfo[] taskFiles = fileDirectory.GetFiles(Path.GetFileNameWithoutExtension(strFileName) + "_*");
                            foreach (FileInfo taskFile in taskFiles)
                                taskFile.Delete();
                        }
                    }
                    // End JSL 12/24/2022 wrapped in if
                }
                catch (Exception ex)
                {
                }
               
                strStorePath = strStorePath + @"/" + strNewFileName;
                string strFileStorePath = Path.Combine(strBasePath, strStorePath);
                Directory.CreateDirectory(Path.GetDirectoryName(strFileStorePath));

                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                strBase64String = regex.Replace(strBase64String, string.Empty);
                Byte[] bytes = Convert.FromBase64String(strBase64String);
                System.IO.File.WriteAllBytes(strFileStorePath, bytes);

                strReturnFilePath = strStorePath;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ConvertBase64IntoFile Error : " + ex.InnerException.ToString());
                strReturnFilePath = strBase64String;
            }
            return strReturnFilePath;
        }
        // End JSL 11/12/2022
    }

    public class Folders
    {
        public string Source { get; private set; }
        public string Target { get; private set; }

        public Folders(string source, string target)
        {
            Source = source;
            Target = target;
        }
    }
    public class SimpleObject
    {
        public string id { get; set; }
        public string name { get; set; }
        public int ShipId { get; set; }
        public string Code { get; set; }
        public string PCUniqueId { get; set; }
        public long ShipSystemsId { get; set; }
    }
}
