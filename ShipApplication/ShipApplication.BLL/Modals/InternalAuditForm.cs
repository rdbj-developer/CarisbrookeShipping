using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class IAF
    {
        public InternalAuditForm InternalAuditForm { get; set; }
        public List<AuditNote> AuditNote { get; set; }
    }
    public class InternalAuditForm
    {
        public long InternalAuditFormId { get; set; }

        public Nullable<System.Guid> UniqueFormID { get; set; }

        public decimal FormVersion { get; set; }
        public Nullable<long> ShipId { get; set; }
        public string ShipName { get; set; }
        public string Location { get; set; }
        public string AuditNo { get; set; }
        public Nullable<bool> AuditTypeISM { get; set; }
        public Nullable<bool> AuditTypeISPS { get; set; }
        public Nullable<bool> AuditTypeMLC { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Auditor { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsSynced { get; set; }
        public Nullable<int> isDelete { get; set; } //RDBJ 11/22/2021
        public int AuditType { get; set; } //RDBJ 11/24/2021
        public bool IsAdditional { get; set; } //RDBJ 11/24/2021
        public bool IsClosed { get; set; } //RDBJ 11/24/2021
        public bool? SavedAsDraft { get; set; } // RDBJ 01/31/2022
    }
    public  class AuditNote
    {
        public long AuditNotesId { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public Nullable<System.Guid> NotesUniqueID { get; set; }
        public Nullable<long> InternalAuditFormId { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public string BriefDescription { get; set; }
        public string Reference { get; set; }
        public string FullDescription { get; set; }
        public string CorrectiveAction { get; set; }
        public string PreventativeAction { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }    // JSL 06/13/2022
        public Nullable<System.DateTime> TimeScale { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public List<AuditNotesAttachment> AuditNotesAttachment { get; set; }
        public bool IsResolved { get; set; }
        public Nullable<System.DateTime> DateClosed { get; set; }
        public int? Priority { get; set; } //RDBJ 11/25/2021
        public Nullable<int> isDelete { get; set; } //RDBJ 11/25/2021
        public Nullable<System.Guid> AssignTo { get; set; } // RDBJ 02/04/2022
    }
    public  class AuditNotesAttachment
    {
        public long AuditNotesAttachmentId { get; set; }
        public Nullable<long> InternalAuditFormId { get; set; }
        public Nullable<Guid> NotesFileUniqueID { get; set; } //RDBJ 10/04/2021
        public Nullable<Guid> NotesUniqueID { get; set; }
        public Nullable<long> AuditNotesId { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public bool IsActive { get; set; }  // RDBJ 02/05/2022 set bool type
    }
    public class AuditDetail
    {
        public long NoteID { get; set; }
        public Nullable<Guid> NotesUniqueID { get; set; }
        public Nullable<Guid> UniqueFormID { get; set; }
        public string Type { get; set; }
        public string Deficiency { get; set; }
        public string Reference { get; set; }
        public string DueDate { get; set; }
        public bool IsResolved { get; set; }
        public long Number { get; set; }    // RDBJ 01/31/2022
        public List<AuditNotesAttachment> AuditNotesAttachment { get; set; } //RDBJ 10/04/2021
    }
    public class Audit_Note_Resolutions
    {
        public long ResolutionID { get; set; }
        public Nullable<Guid> ResolutionUniqueID { get; set; }
        public Nullable<Guid> ResolutionFileUniqueID { get; set; }
        public Nullable<Guid> NotesUniqueID { get; set; }
        public long? AuditNoteID { get; set; }
        public string UserName { get; set; }
        public string Resolution { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Audit_Note_Resolutions_Files> AuditNoteResolutionsFiles { get; set; }
        public int? isNew { get; set; } //RDBJ 10/26/2021
    }
    public class Audit_Note_Resolutions_Files
    {
        public long ResolutionFileID { get; set; }
        public Nullable<Guid> ResolutionUniqueID { get; set; }
        public Nullable<Guid> ResolutionFileUniqueID { get; set; }
        public long ResolutionID { get; set; }
        public long AuditNoteID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }

    // RDBJ 01/31/2022
    public class AuditList
    {
        public long InternalAuditFormId { get; set; }
        public string Auditor { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public System.Guid UniqueFormID { get; set; }
    }
    // End RDBJ 01/31/2022
}
