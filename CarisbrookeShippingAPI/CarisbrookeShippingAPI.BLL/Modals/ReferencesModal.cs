using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    //RDBJ 11/29/2021 Added this Modal
    public class ReferencesModal
    {
    }

    //RDBJ 11/29/2021
    public class SMSReferencesTree
    {
        public int SMSReferenceId { get; set; }
        public Nullable<int> SMSReferenceParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }

        // JSL 05/20/2022
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        // End JSL 05/20/2022
    }
    //End RDBJ 11/29/2021

    //RDBJ 11/29/2021
    public class SSPReferenceTree
    {
        public int SSPReferenceId { get; set; }
        public Nullable<int> SSPReferenceParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }

        // JSL 05/20/2022
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        // End JSL 05/20/2022
    }
    //End RDBJ 11/29/2021

    //RDBJ 11/29/2021
    public class MLCRegulationTree
    {
        public int MLCRegulationId { get; set; }
        public Nullable<int> MLCRegulationParentId { get; set; }
        public string Number { get; set; }
        public string Regulation { get; set; }

        // JSL 05/20/2022
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        // End JSL 05/20/2022
    }
    //End RDBJ 11/29/2021
}
