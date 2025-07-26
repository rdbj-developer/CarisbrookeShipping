using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
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
        public string ShipName { get; set; }
        public string Location { get; set; }
        public Nullable<int> AuditNo { get; set; }
        public Nullable<bool> AuditTypeISM { get; set; }
        public Nullable<bool> AuditTypeISPS { get; set; }
        public Nullable<bool> AuditTypeMLC { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Auditor { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsSynced { get; set; }
    }
    public class AuditNote
    {
        public long AuditNotesId { get; set; }
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
        public Nullable<System.DateTime> TimeScale { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public List<AuditNotesAttachment> AuditNotesAttachment { get; set; }
        public bool isResolved { get; set; }
    }
    public class AuditNotesAttachment
    {
        public long AuditNotesAttachmentId { get; set; }
        public Nullable<long> InternalAuditFormId { get; set; }
        public Nullable<long> AuditNotesId { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
    }

    public class AuditList
    {
        public long InternalAuditFormId { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public bool Extra { get; set; }
        public string AuditDate { get; set; }
        public string Location { get; set; }
        public string Auditor { get; set; }
        public int NCN { get; set; }
        public int OBS { get; set; }
        public bool Closed { get; set; }
    }
    public class AuditDetail
    {
        public long NoteID { get; set; }
        public string Type { get; set; }
        public string Deficiency { get; set; }
        public string Reference { get; set; }
        public string DueDate { get; set; }
        public bool IsResolved { get; set; }
    }
}
