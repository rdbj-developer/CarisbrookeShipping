using System;
using System.Collections.Generic;

namespace ShipApplication.BLL.Modals
{
    public class HoldVentilationRecordFormModal
    {
        public HoldVentilationRecordForm HoldVentilationRecordForm { get; set; }
        public List<HoldVentilationRecordSheetModal> HoldVentilationRecordList { get; set; }
    }

    public class HoldVentilationRecordForm
    {
        public long? HoldVentilationRecordFormId { get; set; }

        public string ShipName { get; set; }

        public string ShipCode { get; set; }

        public string Cargo { get; set; }

        public float? Quantity { get; set; }

        public string LoadingPort { get; set; }

        public DateTime? LoadingDate { get; set; }

        public string DischargingPort { get; set; }

        public DateTime? DischargingDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsSynced { get; set; }
       
    }

    public class HoldVentilationRecordSheetModal
    {
        public long? HoldVentilationRecordId { get; set; }

        public long? HoldVentilationRecordFormId { get; set; }

        public DateTime? HVRDate { get; set; }

        public TimeSpan? HVRTime { get; set; }

        public string OUTDryBulb { get; set; }

        public string OUTWetBulb { get; set; }

        public string OUTDewPOint { get; set; }

        public string HODryBulb { get; set; }

        public string HOWetBulb { get; set; }

        public string HODewPOint { get; set; }

        public bool? IsVentilation { get; set; }

        public float? SeaTemp { get; set; }

        public string Remarks { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }

}
