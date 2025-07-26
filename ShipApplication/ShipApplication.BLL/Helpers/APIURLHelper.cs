using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Helpers
{
    public class APIURLHelper
    {
        public static string GetShipDeficincyGrid = "api/Deficiencies/GetShipDeficincyGrid";
        public static string GetShipGISIReports = "api/Deficiencies/GetShipGISIReports";
        public static string GetDeficienciesData = "api/Deficiencies/GetDeficienciesData";
        public static string GetAuditShipsDeficincyGrid = "api/Deficiencies/GetAuditShipsDeficincyGrid";
        public static string GetShipAudits = "api/Deficiencies/GetShipAudits";
        public static string UpdateAuditDeficiencies = "api/Deficiencies/UpdateAuditDeficiencies";
        public static string AddAuditDeficiencyComments = "api/Deficiencies/AddAuditDeficiencyComments";
        public static string GetAuditDeficiencyComments = "api/Deficiencies/GetAuditDeficiencyComments";
        public static string GetAuditDeficiencyCommentFiles = "api/Deficiencies/GetAuditDeficiencyCommentFiles";
        public static string GetFileComment = "api/Deficiencies/GetFileComment";

        public static string GetAuditFile = "api/Deficiencies/GetAuditFile";
        public static string AddAuditNoteResolutions = "api/IAF/AddAuditNoteResolutions";
        public static string GetAuditNoteResolutions = "api/IAF/GetAuditNoteResolutions";
        public static string GetAuditNoteResolutionFiles = "api/IAF/GetAuditNoteResolutionFiles";
        public static string GetFileAuditNoteResolution = "api/IAF/GetFileAuditNoteResolution";
    }
}
