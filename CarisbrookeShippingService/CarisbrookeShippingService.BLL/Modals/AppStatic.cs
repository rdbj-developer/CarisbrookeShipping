
namespace CarisbrookeShippingService.BLL.Modals
{
    public class AppStatic
    {
        public static string SUCCESS = "Success";
        public static string ERROR = "Error";
        public static string DATABASENAME = "CarisbrookeShipping1234";
        public static string INSERTED = "INSERTED";

        #region FormTypes
        public const string GIRForm = "GI";    // JSL 07/16/2022
        public const string SIRForm = "SI";    // JSL 07/16/2022
        public const string IAFForm = "IA";    // JSL 07/16/2022
        #endregion

        #region Actions
        public static string API_CommonPostAPICall = "CommonPostAPICall";    // RDBJ 02/26/2022
        public static string API_Action_GetReferencesDataFromCloud = "GetReferencesDataFromCloud";    // JSL 05/20/2022
        public const string API_METHOD_CheckFormVersion = "CheckFormVersion";  // JSL 07/16/2022

        public static string APICommonMethods = "api/CommonMethods";  // JSL 07/16/2022
        public const string API_METHOD_InsertOrUpdateDeficienciesData = "InsertOrUpdateDeficienciesData";  // JSL 07/16/2022
        public const string API_METHOD_InsertOrUpdateGIRPhotographsData = "InsertOrUpdateGIRPhotographsData";  // JSL 01/03/2023
        public const string API_METHOD_DeleteGIRPhotographsData = "DeleteGIRPhotographsData";  // JSL 01/03/2023

        // JSL 01/08/2023
        public const string NotificationTypeComment = "Comment";   
        public const string NotificationTypeInitialAction = "InitialAction";   
        public const string NotificationTypeResolution = "Resolution";
        // End JSL 01/08/2023

        #region GIR Related
        public static string APICloudGIR = "api/CloudGIR";  // JSL 07/16/2022
        #endregion

        #region SIR Related
        public static string APICloudSIR = "api/CloudSIR";  // JSL 07/16/2022
        #endregion

        #region IAF Related
        public static string APICloudIAF = "api/CloudIAF";  // JSL 05/20/2022
        #endregion

        #region Setting Related
        public static string APISettings = "api/Settings";    // RDBJ 02/24/2022
        public const string API_GETMAINSYNCSERVICESETTINGS = "GetMainSyncServiceSettings"; // RDBJ 02/24/2022
        #endregion

        #endregion

        #region TableNames

        public static string SMRForm = "SMRForm";
        public static string SMRFormCrewMembers = "SMRFormCrewMembers";
        public static string Documents = "Documents";
        public static string Forms = "Forms";

        public static string GeneralInspectionReport = "GeneralInspectionReport";
        public static string GlRCrewDocuments = "GIRCrewDocuments";
        public static string GlRSafeManningRequirements = "GIRSafeManningRequirements";
        public static string GIRRestandWorkHours = "GIRRestandWorkHours";

        public static string GIRDeficiencies = "GIRDeficiencies";
        public static string GIRPhotographs = "GIRPhotographs";
        public static string GIRDeficienciesFile = "GIRDeficienciesFiles";
        public static string DeficienciesNote = "GIRDeficienciesNotes";
        public static string GIRDeficienciesCommentFile = "GIRDeficienciesCommentFile";
        public static string GIRDeficienciesResolution = "GIRDeficienciesResolution";
        public static string GIRDeficienciesResolutionFile = "GIRDeficienciesResolutionFile";
        public static string GIRDeficienciesInitialActions = "GIRDeficienciesInitialActions";
        public static string GIRDeficienciesInitialActionsFile = "GIRDeficienciesInitialActionsFile";

        public static string SuperintendedInspectionReport = "SuperintendedInspectionReport";
        public static string SIRNotes = "SIRNotes";
        public static string SIRAdditionalNotes = "SIRAdditionalNotes";

        public static string InternalAuditForm = "InternalAuditForm";
        public static string AuditNotes = "AuditNotes";
        public static string AuditNotesAttachment = "AuditNotesAttachment";
        public static string AuditNotesComment = "AuditNotesComments";
        public static string AuditNotesCommentFile = "AuditNotesCommentsFiles";

        public static string AuditNotesResolution = "AuditNotesResolution";
        public static string AuditNotesResolutionFiles = "AuditNotesResolutionFiles";

        public static string MLCRegulationTree = "MLCRegulationTree";   // JSL 05/20/2022
        public static string SMSReferencesTree = "SMSReferencesTree";   // JSL 05/20/2022
        public static string SSPReferenceTree = "SSPReferenceTree"; // JSL 05/20/2022

        public static string ArrivalReports = "ArrivalReports";
        public static string DepartureReports = "DepartureReports";
        public static string DailyCargoReports = "DailyCargoReports";
        public static string DailyPositionReport = "DailyPositionReport";
        public static string HoldVentilationRecordForm = "HoldVentilationRecordForm";
        public static string HoldVentilationRecordSheet = "HoldVentilationRecordSheet";
        public static string ShipAppReleaseNote = "ShipAppReleaseNote";
        public static string FeedbackForm = "FeedbackForm";
        public static string RiskAssessmentForm = "RiskAssessmentForm";
        public static string RiskAssessmentFormHazard = "RiskAssessmentFormHazard";
        public static string RiskAssessmentFormReviewer = "RiskAssessmentFormReviewer";
        public static string AssetManagmentEquipmentList = "AssetManagmentEquipmentList";
        public static string AssetManagmentEquipmentOTList = "AssetManagmentEquipmentOTList";
        public static string AssetManagmentEquipmentITList = "AssetManagmentEquipmentITList";
        public static string AssetManagmentEquipmentSoftwareAssets = "AssetManagmentEquipmentSoftwareAssets";
        public static string CybersecurityRisksAssessment = "CybersecurityRisksAssessment";
        public static string CybersecurityRisksAssessmentList = "CybersecurityRisksAssessmentList";

        public static string HelpAndSupport = "HelpAndSupport"; // RDBJ 12/30/2021
        #endregion

        #region DocumentsUploadType

        public static string NEW = "NEW";
        public static string UPDATED = "UPDATED";
        public static string REMOVED = "REMOVED";

        #endregion

        //RDBJ 10/02/2021
        #region ColumnsName
        public static string UNIQUEFORMID = "UniqueFormID";
        public static string DEFICIENCIESUNIQUEID = "DeficienciesUniqueID"; //RDBJ 10/31/2021
        public static string FORMVERSION = "FormVersion";
        public static string NOTEUNIQUEID = "NotesUniqueID";
        public static string RAFUNIQUEID = "RAFUniqueID";   // JSL 11/24/2022
        #endregion
        //End RDBJ 10/02/2021

        //RDBJ 10/25/2021
        #region SP_Names
        public static string SP_GETALLNOTIFICATIONS = "SP_GetAllNotifications";
        public static string SP_GETNOTIFICATIONDETAILSBYID = "SP_GetNotificationDetailsById";
        public static string SP_GET_GIDEFICIENCIES_OR_SIACTIONABLEITEMS_NUMBER = "SP_Get_GIDeficiencies_OR_SIActionableItems_Number";
        public static string SP_RESETGIDEFICIENCIESORSIACTIONABLEITEMSNUMBERSFROM501 = "SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501"; // RDBJ 12/08/2021
        public static string SP_GETALLRELEASENOTES = "SP_GetAllReleaseNotes"; // RDBJ 12/08/2021
        public static string SP_CB_PROC_SMSREFERENCESTREE_INSERTORUPDATE = "CB_proc_SMSReferencesTree_InsertOrUpdate"; // JSL 05/20/2022
        public static string SP_CB_PROC_SSPREFERENCETREE_INSERTORUPDATE = "CB_proc_SSPReferenceTree_InsertOrUpdate"; // JSL 05/20/2022
        public static string SP_CB_PROC_MLCREGULATIONTREE_INSERTORUPDATE = "CB_proc_MLCRegulationTree_InsertOrUpdate"; // JSL 05/20/2022
        #endregion
        //End RDBJ 10/25/2021
    }
}
