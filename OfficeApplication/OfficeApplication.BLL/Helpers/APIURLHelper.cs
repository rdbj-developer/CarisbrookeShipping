using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Helpers
{
    public class APIURLHelper
    {
        public static string GetShipGISIReports = "api/Deficiencies/GetShipGISIReports";
        public static string GetAuditShips = "api/Deficiencies/GetAuditShips";
        public static string GetShipAudits = "api/Deficiencies/GetShipAudits";
        public static string UpdateAuditDeficiencies = "api/Deficiencies/UpdateAuditDeficiencies";
        public static string GetFSTOAuditDataByShipCode = "api/Deficiencies/GetFSTOAuditDataByShipCode";    // JSL 02/11/2023
        public static string GetFSTOFile = "api/Deficiencies/GetFSTOFile";   // JSL 02/17/2023

        public static string AddAuditDeficiencyComments = "api/Deficiencies/AddAuditDeficiencyComments";
        public static string GetAuditDeficiencyComments = "api/Deficiencies/GetAuditDeficiencyComments";
        public static string GetAuditDeficiencyCommentFiles = "api/Deficiencies/GetAuditDeficiencyCommentFiles";

        public static string AddAuditNoteResolutions = "api/IAF/AddAuditNoteResolutions";
        public static string GetAuditNoteResolutions = "api/IAF/GetAuditNoteResolutions";
        public static string GetAuditNoteResolutionFiles = "api/IAF/GetAuditNoteResolutionFiles";
        public static string GetFileAuditNoteResolution = "api/IAF/GetFileAuditNoteResolution";

        public static string GetAuditFile = "api/Deficiencies/GetAuditFile";
        public static string GetFileComment = "api/Deficiencies/GetFileComment";
        public static string UpdateLocalDbForUsers = "api/AWS/UpdateLocalDbForUsers";
        public static string GetAllUsers = "api/Users/GetAllUsers";
        public static string GetAllUserGroups = "api/Users/GetAllUserGroups";
        public static string GetUserGroupMenuPermission = "api/Users/GetUserGroupMenuPermission";


        public static string APIDeficiencies = "api/Deficiencies";    // RDBJ 02/17/2022
        public static string APIIAF = "api/IAF";    // RDBJ2 02/23/2022
        public static string APISettings = "api/Settings";    // RDBJ 02/24/2022
        public static string APIForms = "api/Forms";    // RDBJ 03/05/2022
        public static string APINotifications = "api/Notifications";    // RDBJ 04/30/2022
        public static string APIUsers = "api/Users";    // JSL 07/23/2022
    }
}
