using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class Deficiency_GISI_Ships
    {
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public long? TotalDeficiencies { get; set; }
        public long? TotalOutstending { get; set; }
        public int? GIDeficiencies { get; set; }
        public int? OpenGIDeficiencies { get; set; }
        public int? SIDeficiencies { get; set; }
        public int? OpenSIDeficiencies { get; set; }
    }
    public class Deficiency_GISI_Report
    {
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public long FormID { get; set; }
        public string Ship { get; set; }
        public string Type { get; set; }
        //public string Date { get; set; } // RDBJ 04/01/2022 commented this line
        public Nullable<DateTime> Date { get; set; } // RDBJ 04/01/2022
        public string Location { get; set; }
        public string Auditor { get; set; }
        public int Deficiencies { get; set; }
        public int OpenDeficiencies { get; set; }
        public bool isExpired { get; set; }
        public List<GIRDeficiency> GIRDeficiences { get; set; }
    }

    public class Deficiency_Audit_Ships
    {
        public long? IAFId { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public string Type { get; set; }
        public int? OpenISMOBS { get; set; }
        public int? OpenISMNCNs { get; set; }
        public int? OpenISPSOBS { get; set; }
        public int? OpenISPSNCN { get; set; }
        public int? OpenMLCOBS { get; set; }
        public int? OpenMLCNCNs { get; set; }
    }
    public class Deficiency_Ship_Audits
    {
        public long InternalAuditFormId { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public string Subject { get; set; }
        public int Type { get; set; } //RDBJ 11/24/2021 Set int from string
        public bool Extra { get; set; }
        //public string AuditDate { get; set; } // RDBJ 04/01/2022 commented this line
        public Nullable<DateTime> AuditDate { get; set; }
        public string Location { get; set; }
        public string Auditor { get; set; }
        public int NCN { get; set; }
        public int OBS { get; set; }
        public int OutstandingNCN { get; set; }
        public int OutstandingOBS { get; set; }
        public bool Closed { get; set; }
        public Nullable<bool> AuditTypeISM { get; set; } //RDBJ 11/23/2021
        public Nullable<bool> AuditTypeISPS { get; set; } //RDBJ 11/23/2021
        public Nullable<bool> AuditTypeMLC { get; set; } //RDBJ 11/23/2021
        public int MLC { get; set; } // RDBJ 01/24/2022
        public int OutstandingMLC { get; set; } // RDBJ 01/24/2022
    }
    public class Audit_Deficiency_Comments
    {
        public Audit_Deficiency_Comments()
        {
            AuditDeficiencyCommentsFiles = new List<Audit_Deficiency_Comments_Files>();
        }
        public long CommentsID { get; set; }
        public Nullable<Guid> NotesUniqueID { get; set; }
        public Nullable<Guid> CommentUniqueID { get; set; }
        public long? AuditNoteID { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public List<Audit_Deficiency_Comments_Files> AuditDeficiencyCommentsFiles { get; set; }
        public int? isNew { get; set; } //RDBJ 10/21/2021
    }
    public class Audit_Deficiency_Comments_Files
    {
        public long CommentFileID { get; set; }
        public long? CommentsID { get; set; }
        public long? AuditNoteID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }

        public Nullable<System.Guid> CommentUniqueID { get; set; }
        public Nullable<System.Guid> CommentFileUniqueID { get; set; }
    }
}
