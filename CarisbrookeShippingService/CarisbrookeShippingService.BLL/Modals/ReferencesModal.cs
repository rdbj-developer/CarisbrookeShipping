using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Modals
{
    // JSL 05/20/2022 Added this class
    public class ReferencesModal
    {
    }

    // JSL 05/20/2022
    public class SMSReferencesTree
    {
        public int SMSReferenceId { get; set; }
        public Nullable<int> SMSReferenceParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
    }
    // End JSL 05/20/2022

    // JSL 05/20/2022
    public class SSPReferenceTree
    {
        public int SSPReferenceId { get; set; }
        public Nullable<int> SSPReferenceParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
    }
    // End JSL 05/20/2022

    // JSL 05/20/2022
    public class MLCRegulationTree
    {
        public int MLCRegulationId { get; set; }
        public Nullable<int> MLCRegulationParentId { get; set; }
        public string Number { get; set; }
        public string Regulation { get; set; }
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
    }
    // End JSL 05/20/2022
}
