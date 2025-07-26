using System;

namespace OfficeApplication.BLL.Modals
{
    public class FleetInspectionDueDatesModal
    {
        public string Ship { get; set; }
        public string Notes { get; set; }
        public DateTime? LastInternalAudit { get; set; }
        public DateTime? InternalAuditDue { get; set; }
        public DateTime? LastFlagGI { get; set; }
        public DateTime? LastFlagASI { get; set; }
        public DateTime? LastFlagCICA { get; set; }
        public DateTime? LastSI { get; set; }
        public DateTime? NextSIDue { get; set; }
        public DateTime? LastGI { get; set; }
        public DateTime? LastFSTO { get; set; }
        public DateTime? SMCExpiry { get; set; }
        public DateTime? SMCVerifDue { get; set; }
        public DateTime? LastExtAudit { get; set; }
        public string SMCIsVerified { get; set; }
        public int ShipId { get; set; }
        public string FieldName { get; set; }
        public DateTime? EventDate { get; set; }
    }
}

