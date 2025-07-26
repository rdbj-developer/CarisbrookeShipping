using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class AppStatic
    {
        public static string SUCCESS = "Success";
        public static string ERROR = "Error";
        public static string DATABASENAME = ConfigurationManager.AppSettings["DBName"];//"CarisbrookeShipping_New";

        #region FormTypes
        public static string GIRForm = "GI";    // RDBJ2 03/12/2022
        public static string SIRForm = "SI";    // RDBJ2 03/12/2022
        public static string IAFForm = "IA";    // RDBJ2 03/12/2022
        #endregion

        #region Common
        public const string API_DELETEDEFICIENCYFILE = "DeleteDeficiencyFile"; // RDBJ 05/10/2022

        // JSL 01/08/2023
        public const string NotificationTypeComment = "Comment";
        public const string NotificationTypeInitialAction = "InitialAction";
        public const string NotificationTypeResolution = "Resolution";
        // End JSL 01/08/2023
        #endregion

        #region GISIForm Related
        public const string API_UPLOADGIRPHOTOGRAPHS = "UploadGIRPhotographs"; // RDBJ2 03/12/2022
        public const string API_UPLOADGISIDEFICIENCIESFILEORPHOTO = "UploadGISIDeficienciesFileOrPhoto"; // RDBJ2 03/12/2022
        public const string API_DELETESIRNOTEORSIRADDITIONALNOTE = "DeleteSIRNoteOrSIRAdditionalNote"; // RDBJ2 04/01/2022
        public const string API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS = "UpdateDeficienciesShipWhenChangeShipInForms"; // RDBJ 04/18/2022
        #endregion

        #region IAF Related
        public const string API_UPDATEAUDITNOTEDETAILS = "UpdateAuditNoteDetails"; // RDBJ2 03/31/2022
        #endregion

        #region Settings Related
        public const string API_GETMAINSYNCSERVICESETTINGS = "GetMainSyncServiceSettings"; // RDBJ 02/24/2022
        public const string API_UPDATEMAINSYNCSERVICESETTINGS = "UpdateMainSyncServiceSettings"; // RDBJ 02/24/2022
        public const string API_GETMAINSYNCSERVICEDETAILSANDSTATUS = "GetMainSyncServiceDetailsAndStatus"; // RDBJ 04/13/2022
        public const string API_DELETEOTHERSHIPSDATAFROMDATABASE = "DeleteOtherShipsDataFromDatabase"; // JSL 11/12/2022
        #endregion

        #region TableNames

        public static string SMRForm = "SMRForm";
        public static string SMRFormCrewMembers = "SMRFormCrewMembers";
        public static string Documents = "Documents";
        public static string ArrivalReports = "ArrivalReports";
        public static string DepartureReports = "DepartureReports";
        public static string DailyCargoReports = "DailyCargoReports";
        public static string DailyPositionReport = "DailyPositionReport";

        public static string GeneralInspectionReport = "GeneralInspectionReport";
        public static string GIRSafeManningRequirements = "GIRSafeManningRequirements";
        public static string GIRCrewDocuments = "GIRCrewDocuments";
        public static string GIRRestandWorkHours = "GIRRestandWorkHours";
        public static string GIRPhotographs = "GIRPhotographs";
        public static string GIRDeficiencies = "GIRDeficiencies";
        public static string GIRDeficienciesFiles = "GIRDeficienciesFiles";
        public static string GIRDeficienciesNote = "GIRDeficienciesNotes";
        public static string GIRDeficienciesCommentFile = "GIRDeficienciesCommentFile";
        public static string GIRDeficienciesInitialActions = "GIRDeficienciesInitialActions";
        public static string GIRDeficienciesInitialActionsFile = "GIRDeficienciesInitialActionsFile";
        public static string GIRDeficienciesResolution = "GIRDeficienciesResolution";
        public static string GIRDeficienciesResolutionFile = "GIRDeficienciesResolutionFile";

        public static string SuperintendedInspectionReport = "SuperintendedInspectionReport";
        public static string SIRNotes = "SIRNotes";
        public static string SIRAdditionalNotes = "SIRAdditionalNotes";
        public static string SIRDeficiencies = "SIRDeficiencies";
        public static string SIRDeficienciesFiles = "SIRDeficienciesFiles";

        public static string InternalAuditForm = "InternalAuditForm";
        public static string AuditNotes = "AuditNotes";
        public static string AuditNotesAttachment = "AuditNotesAttachment";
        public static string AuditNotesComments = "AuditNotesComments";
        public static string AuditNotesCommentsFiles = "AuditNotesCommentsFiles";
        public static string AuditNotesResolution = "AuditNotesResolution";
        public static string AuditNotesResolutionFiles = "AuditNotesResolutionFiles";
        public static string MLCRegulationTree = "MLCRegulationTree";   // JSL 05/20/2022
        public static string SMSReferencesTree = "SMSReferencesTree";   // JSL 05/20/2022
        public static string SSPReferenceTree = "SSPReferenceTree"; // JSL 05/20/2022


        public static string HoldVentilationRecordForm = "HoldVentilationRecordForm";
        public static string HoldVentilationRecordSheet = "HoldVentilationRecordSheet";
        public static string ShipAppReleaseNote = "ShipAppReleaseNote";
        public static string Users = "Users";
        public static string Forms = "Forms";
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

        public static string CSShips = "CSShips"; //RDBJ 09/16/2021
        public static string HelpAndSupport = "HelpAndSupport"; //RDBJ 12/30/2021
        #endregion

        #region DocumentsUploadType

        public static string NEW = "NEW";
        public static string UPDATED = "UPDATED";
        public static string REMOVED = "REMOVED";
        public static string COMPLETED = "COMPLETED";

        #endregion
    }

    public enum RiskFactorType
    {
        VeryLowRisk = 1,
        LowRisk = 2,
        MediumRisk = 3,
        HighRisk = 4,
        VeryHighRisk = 5
    };
}
