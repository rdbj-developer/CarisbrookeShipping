using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Modals
{
    // RDBJ 01/01/2022 Added this class
    public class HelpAndSupportModal
    {
    }

    // RDBJ 01/01/2022
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
        public string StrStatus { get; set; }
        public string StrPriority { get; set; }
    }

    // End RDBJ 01/01/2022
}
