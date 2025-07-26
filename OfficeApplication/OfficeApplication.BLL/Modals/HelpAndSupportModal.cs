using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    // RDBJ 12/27/2021 Added this Modal
    public class HelpAndSupportModal
    {
    }

    // RDBJ 12/27/2021
    public class HelpAndSupport
    {
        public System.Guid? Id { get; set; }
        public string ShipId { get; set; }
        public string Comments { get; set; }
        public Nullable<int> IsStatus { get; set; }
        public Nullable<int> Priority { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public Nullable<int> IsSynced { get; set; }
        public Nullable<int> IsDeleted { get; set; }
        public string StrStatus { get; set; } // RDBJ 12/29/2021
        public string StrPriority { get; set; } // RDBJ 12/29/2021
    }
    // End RDBJ 12/27/2021
}
