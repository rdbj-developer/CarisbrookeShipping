namespace OfficeApplication.BLL.Modals
{
    public class AppStatic
    {
        public static string SUCCESS = "Success";
        public static string ERROR = "Error";
        public static string MANAGESECTION = "MANAGESECTION";

        #region GISIForms Related
        public const string API_UPLOADGIRPHOTOGRAPHS = "UploadGIRPhotographs"; // RDBJ 03/05/2022
        public const string API_UPLOADGISIDEFICIENCIESFILEORPHOTO = "UploadGISIDeficienciesFileOrPhoto"; // RDBJ 03/12/2022
        public const string API_DELETESIRNOTEORSIRADDITIONALNOTE = "DeleteSIRNoteOrSIRAdditionalNote"; // RDBJ 04/02/2022
        #endregion

        #region IAF Related
        public const string API_UPDATEAUDITNOTEDETAILS = "UpdateAuditNoteDetails"; // RDBJ2 02/23/2022
        #endregion

        #region FSTO Related
        // JSL 02/17/2023
        public const string API_INSERTORUPDATEFSTO = "InsertOrUpdateFSTO";
        public const string API_UPLOADFSTOFILES = "UploadFSTOFiles";    // JSL 02/18/2023
        public const string API_GETFSTODETAILSBYID = "GetFSTODetailsById";
        public const string API_DELETEFSTO = "DeleteFSTO";
        public const string API_DELETEFSTOFILE = "DeleteFSTOFile";
        // End JSL 02/17/2023
        #endregion

        #region Settings Related
        public const string API_GETMAINSYNCSERVICESETTINGS = "GetMainSyncServiceSettings"; // RDBJ 02/24/2022
        public const string API_UPDATEMAINSYNCSERVICESETTINGS = "UpdateMainSyncServiceSettings"; // RDBJ 02/24/2022
        #endregion

        #region Notification Related
        public const string API_GETNOTIFICATION = "GetNotification"; // JSL 04/30/2022
        public const string API_GETDEFICIENCIESNOTIFICATION = "GetDeficienciesNotification"; // JSL 06/25/2022
        public const string API_GETRECENTDEFICIENCIESNOTIFICATION = "GetRecentDeficienciesNotification"; // JSL 06/30/2022
        public const string API_OPENANDSEENNOTIFICATION = "OpenAndSeenNotification"; // JSL 05/01/2022
        public const string NotificationType = "SignalRNotification"; // JSL 06/25/2022
        public const string API_GETNOTIFICATIONFROMPAGE = "GetNotificationForPage"; // JSL 07/04/2022
        #endregion

        #region FormsPersonList related
        public const string API_GETFORMSPERSONLIST = "GetFormsPersonList"; // JSL 07/23/2022
        public const string API_ADDNEW_UPDATE_DELETE_FORMSPERSON = "AddNewUpdateDeleteFormsPerson"; // JSL 07/23/2022
        #endregion

        #region Common
        public const string API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS = "UpdateDeficienciesShipWhenChangeShipInForms"; // RDBJ 03/19/2022
        public const string API_DELETEDEFICIENCYFILE = "DeleteDeficiencyFile"; // JSL 05/10/2022

        // JSL 01/08/2023
        public const string NotificationTypeComment = "Comment";
        public const string NotificationTypeInitialAction = "InitialAction";
        public const string NotificationTypeResolution = "Resolution";
        // End JSL 01/08/2023
        #endregion

        #region DocumentsUploadType

        public static string NEW = "NEW";
        public static string UPDATED = "UPDATED";
        public static string REMOVED = "REMOVED";
        public static string COMPLETED = "COMPLETED";

        #endregion
        public enum RiskFactorType
        {
            VeryLowRisk = 1,
            LowRisk = 2,
            MediumRisk = 3,
            HighRisk = 4,
            VeryHighRisk = 5
        };

        #region SecurityHeader
        // JSL 09/26/2022
        public const string API_KEY_HEADER = "X-ApiKey";
        public const string API_KEY_VALUE = "CarisbrookeShipping";
        public const string JSON_CONTENT_TYPE = "application/json";
        public const string USERNAME_VALUE = "@p!U$er";
        public const string PASSWORD_VALUE = "@p!Pa$$w0rd";
        // End JSL 09/26/2022
        #endregion
    }
}
