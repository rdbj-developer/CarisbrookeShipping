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
        public static string AddAuditDeficiencyComments = "api/Deficiencies/AddAuditDeficiencyComments";
        public static string GetAuditDeficiencyComments = "api/Deficiencies/GetAuditDeficiencyComments";
        public static string GetAuditDeficiencyCommentFiles = "api/Deficiencies/GetAuditDeficiencyCommentFiles";
        public static string GetFileComment = "api/Deficiencies/GetFileComment";
        public static string UpdateLocalDbForUsers = "api/AWS/UpdateLocalDbForUsers";
        public static string GetAllUsers = "api/Users/GetAllUsers";
        public static string GetAllUserGroups = "api/Users/GetAllUserGroups";
        public static string GetUserGroupMenuPermission = "api/Users/GetUserGroupMenuPermission";
    }
}
