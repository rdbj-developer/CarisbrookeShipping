using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 02/11/2023 added this class file
namespace OfficeApplication.BLL.Modals
{
    public class FSTO
    {
        public FSTO() { }
        public FSTOInspection FSTOInspection { get; set; }
    }

    // JSL 02/11/2023
    public class FSTOInspection
    {
        // JSL 02/17/2023
        public FSTOInspection()
        {
            FSTOInspectionAttachments = new List<FSTOInspectionAttachment>();
        }
        // End JSL 02/17/2023
        public long FSTOInspectionId { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public Nullable<decimal> FormVersion { get; set; }
        public Nullable<System.DateTime> TravelStartedOn { get; set; }
        public Nullable<System.DateTime> EmbarkedOn { get; set; }
        public Nullable<System.DateTime> DisembarkedOn { get; set; }
        public Nullable<System.DateTime> TravelEndedOn { get; set; }
        public string Location { get; set; }
        public string ShipCode { get; set; }
        public string ShipName { get; set; }
        public string CompletedDays { get; set; }
        public string PurposeOfVisit { get; set; }
        public Nullable<System.Guid> UserGUID { get; set; }
        public string UserName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public Nullable<bool> IsSynced { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public List<FSTOInspectionAttachment> FSTOInspectionAttachments { get; set; } = new List<FSTOInspectionAttachment>(); // JSL 02/17/2023
    }
    // End JSL 02/11/2023

    // JSL 02/17/2023
    public class FSTOInspectionAttachment
    {
        public System.Guid UniqueID { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
    }
    // End JSL 02/17/2023
}
