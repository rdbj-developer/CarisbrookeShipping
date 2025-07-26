using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    // JSL 05/22/2022 Added this Modal
    public class ReferencesModal
    {
    }

    // JSL 05/22/2022
    public class SMSReferencesTree
    {
        public int SMSReferenceId { get; set; }
        public Nullable<int> SMSReferenceParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }
    }
    // End JSL 05/22/2022

    // JSL 05/22/2022
    public class SSPReferenceTree
    {
        public int SSPReferenceId { get; set; }
        public Nullable<int> SSPReferenceParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }
    }
    // End JSL 05/22/2022

    // JSL 05/22/2022
    public class MLCRegulationTree
    {
        public int MLCRegulationId { get; set; }
        public Nullable<int> MLCRegulationParentId { get; set; }
        public string Number { get; set; }
        public string Regulation { get; set; }
    }
    // End JSL 05/22/2022

    // JSL 05/22/2022
    public class TreeObject
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }
        public TreeObject[] Nodes { get; set; }
    }
    // End JSL 05/22/2022
}
