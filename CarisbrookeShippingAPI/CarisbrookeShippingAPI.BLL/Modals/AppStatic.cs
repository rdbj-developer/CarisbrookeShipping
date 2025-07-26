using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class AppStatic
    {
        public static string SUCCESS = "Success";
        public static string ERROR = "Error";
        public static string DATABASENAME = "CarisbrookeShipping1234";

        #region FormTypes
        public static string GIRForm = "GI";    // RDBJ 03/12/2022
        public static string SIRForm = "SI";    // RDBJ 03/12/2022
        public static string IAFForm = "IA";    // RDBJ 03/12/2022
        public static string FSTOForm = "FSTO";    // JSL 02/17/2023
        
        // JSL 05/01/2022
        public static string NotificationTypeForm = "Form";
        public static string NotificationType = "SignalRNotification"; // JSL 06/24/2022

        public const string NotificationTypeComment = "Comment";   // JSL 06/24/2022
        public const string NotificationTypeInitialAction = "InitialAction";   // JSL 06/24/2022
        public const string NotificationTypeResolution = "Resolution"; // JSL 06/24/2022

        public static string GIRFormName = "General Inspection Report";
        public static string SIRFormName = "Superintended Inspection Report";
        public static string IAFFormName = "Internal Audit Form";

        // End JSL 05/01/2022
        #endregion

        #region SIR Local To Server
        public const string API_METHOD_CheckFormVersion = "CheckFormVersion";  // JSL 07/16/2022
        public const string API_METHOD_InsertOrUpdateDeficienciesData = "InsertOrUpdateDeficienciesData";  // JSL 07/16/2022
        public const string API_METHOD_InsertOrUpdateGIRPhotographsData = "InsertOrUpdateGIRPhotographsData";  // JSL 01/03/2023
        public const string API_METHOD_DeleteGIRPhotographsData = "DeleteGIRPhotographsData";  // JSL 01/03/2023
        #endregion

        #region OfficeAPI
        public const string API_SIGNALRNOTIFICATION_CONTROLLER = "SignalRNotification";  // JSL 06/24/2022
        public const string API_SIGNALRNOTIFICATION_ACTION = "SendNotification";  // JSL 06/24/2022
        #endregion

        #region TableNames

        public static string SMRForm = "SMRForm";
        public static string SMRFormCrewMembers = "SMRFormCrewMembers";



        #endregion

        #region DocumentsUploadType

        public static string NEW = "NEW";
        public static string UPDATED = "UPDATED";
        public static string REMOVED = "REMOVED";
        public static string COMPLETED = "COMPLETED";

        #endregion

        #region Actions

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
        public const string API_GETNOTIFICATIONFROMPAGE = "GetNotificationForPage"; // JSL 07/04/2022
        #endregion

        #region FormsPersonList related
        public const string API_GETFORMSPERSONLIST = "GetFormsPersonList"; // JSL 07/23/2022
        public const string API_ADDNEW_UPDATE_DELETE_FORMSPERSON = "AddNewUpdateDeleteFormsPerson"; // JSL 07/23/2022
        #endregion

        #region Common
        public const string API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS = "UpdateDeficienciesShipWhenChangeShipInForms"; // RDBJ 03/19/2022
        public const string API_DELETEDEFICIENCYFILE = "DeleteDeficiencyFile"; // JSL 05/10/2022
        #endregion

        #endregion
    }
}
