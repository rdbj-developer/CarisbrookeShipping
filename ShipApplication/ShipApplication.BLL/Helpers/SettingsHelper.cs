using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Win32;
using Newtonsoft.Json;
using ShipApplication.BLL.Modals;
using ShipApplication.BLL.Resources.Constant;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;

namespace ShipApplication.BLL.Helpers
{
    public class SettingsHelper
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"].ToString(); //RDBJ 11/10/2021

        // JSL 11/12/2022
        public static ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();  
        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
        // End JSL 11/12/2022
        public void CreateShipsJson()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                List<CSShipsModal> ShipsList = _helper.GetAllShips();
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "Repository\\Ships.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                string jsonData = JsonConvert.SerializeObject(ShipsList, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ShipsJson Create Error : " + ex.Message);
            }
        }
        public List<CSShipsModal> GetAllShipsFromJson()
        {
           
            List<CSShipsModal> ships = new List<CSShipsModal>();
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "Repository\\Ships.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                    if (!string.IsNullOrEmpty(jsonText))
                    {
                        ships = JsonConvert.DeserializeObject<List<CSShipsModal>>(jsonText);
                        ships = ships.OrderBy(x => x.Name).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllShipsFromJson Error : " + ex.Message);
            }
            return ships;
        }
        public bool SaveShipJson(SimpleObject modal)
        {
            bool res = false;
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\Shipvalue.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                string jsonData = JsonConvert.SerializeObject(modal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveShipJson Error : " + ex.Message);
            }
            return res;
        }
        public SimpleObject GetShipJson()
        {
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\Shipvalue.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                    if (!string.IsNullOrEmpty(jsonText))
                    {
                        SimpleObject res = JsonConvert.DeserializeObject<SimpleObject>(jsonText);
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipJson Error : " + ex.Message);
            }
            return null;
        }
        public bool SaveSMTPSettingsJson(SMTPServerModal modal)
        {
            bool res = false;
            try
            {
                //C:\JsonFiles
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\SMTPServerConfig.json";
                
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                string jsonData = JsonConvert.SerializeObject(modal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveSMTPSettingsJson Error : " + ex.Message);
            }
            return res;
        }
        public SMTPServerModal GetSMTPSettingsJson()
        {
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\SMTPServerConfig.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                    if (!string.IsNullOrEmpty(jsonText))
                    {
                        SMTPServerModal res = JsonConvert.DeserializeObject<SMTPServerModal>(jsonText);
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMTPSettingsJson Error : " + ex.Message);
            }
            return null;
        }

        //RDBJ 11/10/2021
        public List<string> GetEmailFromUserProfileTableWhereTechnicalAndISMGroup(
            string ShipCode // JSL 02/24/2023
            )
        {
            List<string> response = new List<string>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/AWS/GetEmailFromUserProfileTableWhereTechnicalAndISMGroup?ShipCode=" + ShipCode).Result;  // JSL 02/25/2023 added ShipCode
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetEmailFromUserProfileTableWhereTechnicalAndISMGroup Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/10/2021

        // JSL 11/12/2022
        public bool DeleteDataFromDatabaseExceptCurrentShip(string strShipCode)
        {
            bool blnRetStatus = false;
            try
            {
                Delete_GIR_SIR_DataFromThisShipExceptCurrentShipData(strShipCode, AppStatic.GIRForm);
                Delete_GIR_SIR_DataFromThisShipExceptCurrentShipData(strShipCode, AppStatic.SIRForm);
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
        public void Delete_GIR_SIR_DataFromThisShipExceptCurrentShipData(string strShipCode, string strFormType)
        {
            APIDeficienciesHelper objDeficienciesHelper = new APIDeficienciesHelper();
            List<Deficiency_GISI_Report> GISI_Report_List = new List<Deficiency_GISI_Report>();
            GISI_Report_List = objDeficienciesHelper.GetShipGISIReports_Local_DB(strShipCode, strFormType, true);
            foreach (var itemGI_SI in GISI_Report_List)
            {
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            if (conn.State == ConnectionState.Open)
                                conn.Close();

                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "";
                            if (strFormType == "GI")
                                query = "SELECT D.*,G.Inspector,G.Port,G.CreatedDate as DateRaised,G.UniqueFormID  FROM " + AppStatic.GIRDeficiencies + "  D Inner Join GeneralInspectionReport G on G.UniqueFormID = D.UniqueFormID WHERE D.isDelete = 0 and D.Ship = '" + itemGI_SI.Ship + "' and D.UniqueFormID = '" + Convert.ToString(itemGI_SI.UniqueFormID) + "' and D.ReportType = '" + strFormType + "' ORDER BY No";
                            else if (strFormType == "SI")
                                query = "SELECT D.*,G.Superintended as Inspector,G.Port,G.CreatedDate as DateRaised,G.UniqueFormID  FROM " + AppStatic.GIRDeficiencies + "  D Inner Join SuperintendedInspectionReport G on G.UniqueFormID = D.UniqueFormID WHERE D.isDelete = 0 and D.Ship = '" + itemGI_SI.Ship + "' and D.UniqueFormID = '" + Convert.ToString(itemGI_SI.UniqueFormID) + "' and D.ReportType = '" + strFormType + "' ORDER BY No";

                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            conn.Close();
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficiencies> lstDeficiencies = dt.ToListof<GIRDeficiencies>();
                                foreach (var itemDeficiencies in lstDeficiencies)
                                {
                                    // Delete comments file and comment
                                    List<GIRDeficienciesNote> lstComments = new List<GIRDeficienciesNote>();
                                    lstComments = objDeficienciesHelper.GetDeficienciesNote_Local_DB(itemDeficiencies.DeficienciesUniqueID);
                                    foreach (var itemComment in lstComments)
                                    {
                                        // Delete files for each comment
                                        DeleteRecords(AppStatic.GIRDeficienciesCommentFile, "NoteUniqueID", Convert.ToString(itemComment.NoteUniqueID));
                                    }
                                    // Delete all comments after files deleted
                                    DeleteRecords(AppStatic.GIRDeficienciesNote, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                                    // End Delete comments file and comment

                                    // Delete resolution file and resolution
                                    List<GIRDeficienciesResolution> lstResolutions = new List<GIRDeficienciesResolution>();
                                    lstResolutions = objDeficienciesHelper.GetDeficienciesResolution_Local_DB(itemDeficiencies.DeficienciesUniqueID);
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
                                    lstIniActions = objDeficienciesHelper.GetDeficienciesInitialActions_Local_DB(itemDeficiencies.DeficienciesUniqueID);
                                    foreach (var itemIniAction in lstIniActions)
                                    {
                                        // Delete files for each comment
                                        DeleteRecords(AppStatic.GIRDeficienciesInitialActionsFile, "IniActFileUniqueID", Convert.ToString(itemIniAction.IniActUniqueID));
                                    }
                                    // Delete all comments after files deleted
                                    DeleteRecords(AppStatic.GIRDeficienciesInitialActions, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                                    // End Delete initial file and initial

                                    // Delete Def. Files
                                    DeleteRecords(AppStatic.GIRDeficienciesFiles, "DeficienciesUniqueID", Convert.ToString(itemDeficiencies.DeficienciesUniqueID));
                                    // End Delete Def. Files
                                }

                                // Delete Def.
                                DeleteRecords(AppStatic.GIRDeficiencies, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                                // End Delete Def.

                                // Delete Others table data for GI
                                if (strFormType == "GI")
                                {
                                    // Delete SafeManningRequirements
                                    DeleteRecords(AppStatic.GIRSafeManningRequirements, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                                    // End Delete SafeManningRequirements

                                    // Delete CrewDocuments
                                    DeleteRecords(AppStatic.GIRCrewDocuments, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                                    // End Delete CrewDocuments

                                    // Delete RestandWorkHours
                                    DeleteRecords(AppStatic.GIRRestandWorkHours, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                                    // End Delete RestandWorkHours

                                    // Delete GIRPhotos
                                    DeleteRecords(AppStatic.GIRPhotographs, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                                    // End Delete GIRPhotos
                                }
                                // End Delete Others table data for GI

                                // Delete Others table data for SI
                                if (strFormType == "SI")
                                {
                                    // Delete SIRNotes
                                    DeleteRecords(AppStatic.SIRNotes, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                                    // End Delete SIRNotes

                                    // Delete SIRAdditionalNotes
                                    DeleteRecords(AppStatic.SIRAdditionalNotes, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                                    // End Delete SIRAdditionalNotes
                                }
                                // End Delete Others table data for SI
                            }
                        }
                    }
                }

                // Delete GI_Form
                if (strFormType == "GI")
                {
                    DeleteRecords(AppStatic.GeneralInspectionReport, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                }
                // End Delete GI_Form

                // Delete SI_Form
                if (strFormType == "SI")
                {
                    DeleteRecords(AppStatic.SuperintendedInspectionReport, "UniqueFormID", Convert.ToString(itemGI_SI.UniqueFormID));
                }
                // End Delete SI_Form
            }
        }
        // End JSL 11/12/2022

        public void Delete_IAF_DataFromThisShipExceptCurrentShipData(string strShipCode)
        {
            APIDeficienciesHelper objDeficienciesHelper = new APIDeficienciesHelper();
            List<Deficiency_Ship_Audits> lstAuditList = new List<Deficiency_Ship_Audits>();
            lstAuditList = objDeficienciesHelper.GetShipAudits_Local_DB(strShipCode, true);
            foreach (var itemIAF in lstAuditList)
            {
                IAF modalIAF = new IAF();
                modalIAF = objDeficienciesHelper.GetAuditDetails_Local_DB(itemIAF.UniqueFormID);
                List<AuditNote> lstNotesList = modalIAF.AuditNote;
                foreach (var itemNote in lstNotesList)
                {
                    // Delete comments file and comment
                    List<Audit_Deficiency_Comments> lstComments = new List<Audit_Deficiency_Comments>();
                    lstComments = objDeficienciesHelper.GetAuditDeficiencyComments_Local_DB(itemNote.NotesUniqueID);
                    foreach (var itemComment in lstComments)
                    {
                        // Delete files for each comment
                        DeleteRecords(AppStatic.AuditNotesCommentsFiles, "CommentUniqueID", Convert.ToString(itemComment.CommentUniqueID));
                    }
                    // Delete all comments after files deleted
                    DeleteRecords(AppStatic.AuditNotesComments, "NotesUniqueID", Convert.ToString(itemNote.NotesUniqueID));
                    // End Delete comments file and comment

                    // Delete resolution file and resolution
                    List<Audit_Note_Resolutions> lstResolutions = new List<Audit_Note_Resolutions>();
                    IAFHelper objIAFhelper = new IAFHelper();
                    lstResolutions = objIAFhelper.GetAuditNoteResolutions_Local_DB(itemNote.NotesUniqueID);

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
                DeleteRecords(AppStatic.InternalAuditForm, "UniqueFormID", Convert.ToString(itemIAF.UniqueFormID));
                // End Delete IA_Form
            }
        }

        // JSL 11/12/2022
        public bool DeleteRecords(string tablename, string columnname, string RecID)
        {
            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
            SqlConnection conn = new SqlConnection(connetionString);
            try
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM " + tablename + " WHERE " + columnname + " = '" + RecID + "'", conn))
                {
                    command.ExecuteNonQuery();
                }
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteRecords Local DB in table Error : " + ex.Message.ToString());
                return false;
            }
        }
        // End JSL 11/12/2022

        // RDBJ 02/26/2022
        public Dictionary<string, string> AjaxPostPerformAction(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                // JSL 11/12/2022
                case AppStatic.API_DELETEOTHERSHIPSDATAFROMDATABASE:
                    {
                        try
                        {
                            string strCurrShipCode = string.Empty;
                            strCurrShipCode = SessionManager.ShipCode;
                            DeleteDataFromDatabaseExceptCurrentShip(strCurrShipCode);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_DELETEOTHERSHIPSDATAFROMDATABASE + " Error : " + ex.InnerException.ToString());
                        }
                        break;
                    }
                // End JSL 11/12/2022
                // RDBJ 04/13/2022
                case AppStatic.API_GETMAINSYNCSERVICEDETAILSANDSTATUS:
                    {
                        try
                        {
                            string ServiceName = "CarisbrookeShippingService";  //Convert.ToString(ConfigurationManager.AppSettings["MainSyncServiceName"]);
                            string ServiceStatus = string.Empty;
                            string strDisplayName = string.Empty;
                            string strInstalledVersion = string.Empty;
                            string strInstalledDate = string.Empty;
                            string strPublisher = string.Empty;
                            string strUnninstallCommand = string.Empty;
                            string strModifyPath = string.Empty;

                            string strLatestVersion = string.Empty;

                            bool blnIsServiceInstalled = CheckServiceInstalled(ServiceName);    // RDBJ 04/18/2022

                            // RDBJ 04/18/2022 wrapped in if
                            if (blnIsServiceInstalled)
                            {
                                ServiceController mainSyncServieController = new ServiceController(ServiceName);
                                switch (mainSyncServieController.Status)
                                {
                                    case ServiceControllerStatus.Running:
                                        ServiceStatus = "Running";
                                        break;
                                    case ServiceControllerStatus.Stopped:
                                        ServiceStatus = "Stopped";
                                        break;
                                    case ServiceControllerStatus.Paused:
                                        ServiceStatus = "Paused";
                                        break;
                                    case ServiceControllerStatus.StopPending:
                                        ServiceStatus = "Stopping";
                                        break;
                                    case ServiceControllerStatus.StartPending:
                                        ServiceStatus = "Starting";
                                        break;
                                    default:
                                        ServiceStatus = "Status Changing";
                                        break;
                                }

                                string registry_key_64 = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key_64))
                                {
                                    foreach (string name in key.GetSubKeyNames())
                                    {
                                        using (RegistryKey subkey = key.OpenSubKey(name))
                                        {
                                            string InstalledAppName = Convert.ToString(subkey.GetValue("DisplayName"));

                                            if (!string.IsNullOrEmpty(InstalledAppName) && InstalledAppName == ServiceName)
                                            {
                                                strDisplayName = (string)subkey.GetValue("DisplayName");
                                                strInstalledVersion = (string)subkey.GetValue("DisplayVersion");
                                                strInstalledDate = (string)subkey.GetValue("InstallDate");
                                                strPublisher = (string)subkey.GetValue("Publisher");
                                                strUnninstallCommand = (string)subkey.GetValue("UninstallString");
                                                strModifyPath = (string)subkey.GetValue("ModifyPath");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ServiceStatus = "Service not installed";
                            }

                            string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + @"JsonFiles\MainSyncServiceIntervalTime.json";
                            Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                            Dictionary<string, string> LocalSettings = new Dictionary<string, string>();
                            Dictionary<string, string> ServerSettings = new Dictionary<string, string>();    // JSL 06/24/2022
                            if (System.IO.File.Exists(jsonFilePath))
                            {
                                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                                if (!string.IsNullOrEmpty(jsonText))
                                {
                                    Dictionary<string, object> fileDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);
                                    LocalSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["LocalSettings"].ToString());
                                    ServerSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["ServerSettings"].ToString());   // JSL 06/24/2022

                                    // JSL 06/24/2022 commented 
                                    /*
                                    if (LocalSettings != null)
                                    {
                                        if (LocalSettings.ContainsKey("MainSyncServiceVersion"))
                                            strLatestVersion = Convert.ToString(LocalSettings["MainSyncServiceVersion"]);
                                    }
                                    */

                                    // End JSL 06/24/2022Commented

                                    // JSL 06/24/2022
                                    if (ServerSettings != null)
                                    {
                                        if (ServerSettings.ContainsKey("MainSyncServiceVersion"))
                                            strLatestVersion = Convert.ToString(ServerSettings["MainSyncServiceVersion"]);
                                    }
                                    // End JSL 06/24/2022
                                }
                            }

                            bool IsNeedToShowNotificationForLatestMainSyncService = false;
                            if (!string.IsNullOrEmpty(strLatestVersion)
                                && !string.IsNullOrEmpty(strInstalledVersion)  // RDBJ 04/18/2022
                                )
                            {
                                string strJoinInstalledVersion = strInstalledVersion.Replace(".", "");
                                strJoinInstalledVersion = strJoinInstalledVersion.Substring(0, strJoinInstalledVersion.Length - 2);

                                strLatestVersion = strLatestVersion.Replace(".", "");
                                if (strLatestVersion.Length == 4)
                                    strLatestVersion = strLatestVersion + "0";

                                long intInstalledVersion = Convert.ToInt64(strJoinInstalledVersion);
                                long intLatestVersion = Convert.ToInt64(strLatestVersion.Replace(".", ""));

                                IsNeedToShowNotificationForLatestMainSyncService = (intLatestVersion > intInstalledVersion ? true : false);
                            }
                            // RDBJ 04/18/2022 added else
                            else
                            {
                                IsNeedToShowNotificationForLatestMainSyncService = true;
                            }

                            retDictMetaData["ServiceStatus"] = ServiceStatus;
                            retDictMetaData["DisplayName"] = strDisplayName;
                            retDictMetaData["InstalledVersion"] = !string.IsNullOrEmpty(strInstalledVersion) ? strInstalledVersion.Substring(0, strInstalledVersion.Length - 2) : string.Empty; // RDBJ 04/18/2022 handle not installed error
                            retDictMetaData["LatestVersion"] = Convert.ToString(LocalSettings["MainSyncServiceVersion"]);
                            retDictMetaData["InstalledDate"] = !string.IsNullOrEmpty(strInstalledVersion) ? Convert.ToString(DateTime.ParseExact(strInstalledDate, "yyyyMMdd", CultureInfo.InvariantCulture)) : string.Empty;   // RDBJ 04/18/2022 handle not installed error
                            retDictMetaData["IsNeedToShowNotificationForLatestMainSyncService"] = IsNeedToShowNotificationForLatestMainSyncService.ToString().ToLower();
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_GETMAINSYNCSERVICEDETAILSANDSTATUS + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ 04/13/2022
                // RDBJ 02/26/2022
                case AppStatic.API_UPDATEMAINSYNCSERVICESETTINGS:
                    {
                        try
                        {
                            string IntervalTime = string.Empty;
                            string UseServerTimeInterval = string.Empty;
                            string UpdatedBy = string.Empty;
                            string MainSyncServiceVersion = string.Empty;

                            if (dictMetaData.ContainsKey("IntervalTime"))
                                IntervalTime = dictMetaData["IntervalTime"].ToString();

                            if (dictMetaData.ContainsKey("UseServerTimeInterval"))
                                UseServerTimeInterval = dictMetaData["UseServerTimeInterval"].ToString().ToLower();

                            if (dictMetaData.ContainsKey("UpdatedBy"))
                                UpdatedBy = dictMetaData["UpdatedBy"].ToString();

                            if (dictMetaData.ContainsKey("MainSyncServiceVersion"))
                                MainSyncServiceVersion = dictMetaData["MainSyncServiceVersion"].ToString();

                            string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + @"JsonFiles\MainSyncServiceIntervalTime.json";
                            Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                            if (!File.Exists(jsonFilePath))
                            {
                                File.WriteAllText(jsonFilePath, string.Empty);
                                break;  // RDBJ 02/28/2022
                            }
                            string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                            Dictionary<string, object> fileDictMetaData = new Dictionary<string, object>();

                            if (!string.IsNullOrEmpty(jsonText))
                            {
                                fileDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);
                                Dictionary<string, string> ServerSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["ServerSettings"].ToString());

                                retDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["LocalSettings"].ToString());
                                retDictMetaData["IntervalTime"] = IntervalTime;
                                retDictMetaData["UseServerTimeInterval"] = UseServerTimeInterval;
                                retDictMetaData["UpdatedBy"] = UpdatedBy;
                                retDictMetaData["UpdatedDate"] = Utility.ToDateTimeUtcNow().ToString("dd-MM-yyyy");
                                retDictMetaData["MainSyncServiceVersion"] = MainSyncServiceVersion;

                                fileDictMetaData["LocalSettings"] = retDictMetaData;
                                fileDictMetaData["ServerSettings"] = ServerSettings;

                                jsonText = JsonConvert.SerializeObject(fileDictMetaData, Formatting.Indented);
                                File.WriteAllText(jsonFilePath, jsonText);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPDATEMAINSYNCSERVICESETTINGS + " Error : " + ex.Message);
                        }
                        IsPerformSuccess = true;
                        break;
                    }
                // End RDBJ 02/26/2022
                // RDBJ 02/26/2022
                case AppStatic.API_GETMAINSYNCSERVICESETTINGS:
                    {
                        try
                        {
                            string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + @"JsonFiles\MainSyncServiceIntervalTime.json";
                            Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                            if (System.IO.File.Exists(jsonFilePath))
                            {
                                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                                if (!string.IsNullOrEmpty(jsonText))
                                {
                                    retDictMetaData["MainSyncSettings"] = jsonText;
                                }
                                else
                                {
                                    retDictMetaData["MainSyncSettings"] = string.Empty;
                                }
                            }
                            else
                            {
                                retDictMetaData["MainSyncSettings"] = string.Empty;
                            }
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            retDictMetaData["MainSyncSettings"] = string.Empty;
                            LogHelper.writelog(AppStatic.API_GETMAINSYNCSERVICESETTINGS + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ 02/26/2022
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

        // End RDBJ 02/26/2022

        // RDBJ 04/18/2022
        public static bool CheckServiceInstalled(string serviceToFind)
        {
            ServiceController[] servicelist = ServiceController.GetServices();
            foreach (ServiceController service in servicelist)
            {
                if (service.ServiceName == serviceToFind)
                    return true;
            }
            return false;
        }
        // End RDBJ 04/18/2022

        //public void createFile()
        //{
        //    Byte[] buffer = pck.GetAsByteArray();

        //    //Clear the response               
        //    Response.Clear();
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    Response.Cookies.Clear();
        //    Response.ContentType = "text/plain";
        //    Response.OutputStream.Write(buffer, 0, buffer.Length);
        //    Response.AddHeader("Content-Disposition", "attachment;filename=yourfile.txt");
        //}
    }
}
