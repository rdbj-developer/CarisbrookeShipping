using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
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
        public long FormID { get; set; }
        public string Ship { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public int Deficiencies { get; set; }
        public int OpenDeficiencies { get; set; }
        public bool isExpired { get; set; }
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
    public class Audit_Deficiency_Comments
    {
        public long CommentsID { get; set; }
        public string AuditNoteID { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Audit_Deficiency_Comments_Files> AuditDeficiencyCommentsFiles { get; set; }
    }
    public class Audit_Deficiency_Comments_Files
    {
        public long CommentFileID { get; set; }
        public long CommentsID { get; set; }
        public long AuditNoteID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }
}
