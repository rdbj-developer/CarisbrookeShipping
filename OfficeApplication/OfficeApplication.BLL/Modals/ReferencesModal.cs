using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
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
    }
    //End RDBJ 11/29/2021

    //RDBJ 11/29/2021
    public class SSPReferenceTree
    {
        public int SSPReferenceId { get; set; }
        public Nullable<int> SSPReferenceParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }
    }
    //End RDBJ 11/29/2021

    //RDBJ 11/29/2021
    public class MLCRegulationTree
    {
        public int MLCRegulationId { get; set; }
        public Nullable<int> MLCRegulationParentId { get; set; }
        public string Number { get; set; }
        public string Regulation { get; set; }
    }
    //End RDBJ 11/29/2021

    //RDBJ 11/30/2021
    public class TreeObject
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Number { get; set; }
        public string Reference { get; set; }
        public TreeObject[] Nodes { get; set; } //RDBJ 12/01/2021 set it array
    }
    //End RDBJ 11/30/2021
}
