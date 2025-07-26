using System;
using System.Collections.Generic;

namespace CarisbrookeShippingService.BLL.Modals
{
    public class IAF
    {
        public InternalAuditForm InternalAuditForm { get; set; }
        public List<AuditNote> AuditNote { get; set; }
    }
    public class InternalAuditForm
    {
        public long InternalAuditFormId { get; set; }
        public Nullable<long> ShipId { get; set; }
        public Nullable<Guid> UniqueFormID { get; set; }
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
        public Nullable<decimal> FormVersion { get; set; }
        public Nullable<int> isDelete { get; set; } //RDBJ 11/25/2021
        public int AuditType { get; set; } //RDBJ 11/25/2021
        public bool IsAdditional { get; set; } //RDBJ 11/25/2021
        public bool IsClosed { get; set; } //RDBJ 11/25/2021
        public bool? SavedAsDraft { get; set; } // RDBJ 01/31/2022
    }
    public class AuditNote
    {
        public AuditNote()
        {
            AuditNotesAttachment = new List<AuditNotesAttachment>();
            AuditNotesComment = new List<Audit_Deficiency_Comments>();
            AuditNotesResolution = new List<Audit_Note_Resolutions>();
        }
        public long AuditNotesId { get; set; }
        public Nullable<long> InternalAuditFormId { get; set; }
        public Nullable<Guid> NotesUniqueID { get; set; }
        public Nullable<Guid> UniqueFormID { get; set; }
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
        public Nullable<System.DateTime> TimeScale { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool? IsResolved { get; set; } // RDBJ 01/19/2022 make case
        public int? Priority { get; set; } //RDBJ 11/25/2021
        public Nullable<int> isDelete { get; set; } //RDBJ 11/25/2021
        public Nullable<System.DateTime> DateClosed { get; set; }
        public List<AuditNotesAttachment> AuditNotesAttachment { get; set; }
        public List<Audit_Deficiency_Comments> AuditNotesComment { get; set; }
        public List<Audit_Note_Resolutions> AuditNotesResolution { get; set; }
        public Nullable<System.Guid> AssignTo { get; set; } // RDBJ 12/21/2021
    }
    public class AuditNotesAttachment
    {
        public Nullable<Guid> NotesFileUniqueID { get; set; } //RDBJ 10/05/2021
        public Nullable<Guid> NotesUniqueID { get; set; } //RDBJ 10/05/2021
        public long AuditNotesAttachmentId { get; set; }
        public Nullable<long> InternalAuditFormId { get; set; }
        public Nullable<long> AuditNotesId { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public bool IsActive { get; set; }
    }

    public class Audit_Note_Resolutions
    {
        public long ResolutionID { get; set; }
        public Nullable<Guid> NotesUniqueID { get; set; }
        public Nullable<long> AuditNoteID { get; set; }
        public Nullable<Guid> ResolutionUniqueID { get; set; }
        public string UserName { get; set; }
        public string Resolution { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public List<Audit_Note_Resolutions_Files> AuditNoteResolutionsFiles { get; set; }
    }
    public class Audit_Note_Resolutions_Files
    {
        public long ResolutionFileID { get; set; }
        public Nullable<Guid> ResolutionUniqueID { get; set; }
        public Nullable<Guid> ResolutionFileUniqueID { get; set; }
        public Nullable<long> ResolutionID { get; set; }
        public Nullable<long> AuditNoteID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }

    public class Audit_Deficiency_Comments
    {
        public long CommentsID { get; set; }
        public Nullable<Guid> NotesUniqueID { get; set; }
        public Nullable<Guid> CommentUniqueID { get; set; }
        public long? AuditNoteID { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public List<Audit_Deficiency_Comments_Files> AuditDeficiencyCommentsFiles { get; set; }
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
