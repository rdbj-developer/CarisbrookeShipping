using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class ArrivalReportModal
    {
        public long ARID { get; set; }
        public Nullable<int> ShipNo { get; set; }
        public string ShipName { get; set; }
        public DateTime ReportCreated { get; set; }
        public Nullable<int> VoyageNo { get; set; }
        public string PortName { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string NORTenderedDate { get; set; }
        public string NORTenderedTime { get; set; }
        public DateTime? POBDate { get; set; }
        public string POBTime { get; set; }
        public string NoOfTugsUsed { get; set; }
        public Nullable<bool> OnAnchor { get; set; }
        public string ArrivalAlongSideDate { get; set; }
        public string ArrivalAlongSideTime { get; set; }
        public Nullable<double> AverageSpeed { get; set; }
        public Nullable<double> DistanceMade { get; set; }
        public Nullable<double> FuelOil { get; set; }
        public Nullable<double> DieselOil { get; set; }
        public Nullable<double> SulphurFuelOil { get; set; }
        public Nullable<double> SulphurDieselOil { get; set; }
        public Nullable<double> FreshWater { get; set; }
        public Nullable<double> LubeOil { get; set; }
        public DateTime? CargoDate { get; set; }
        public string CargoTime { get; set; }
        public string ETCDepartureDate { get; set; }
        public string ETCDepartureTime { get; set; }
        public string NextPort { get; set; }
        public string Remarks { get; set; }
        public string ToEmail { get; set; }
        public List<string> CCEmail { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsSynced { get; set; }
        public DateTime? TenderedDate { get; set; }
        public string TenderedTime { get; set; }
        public int TugsNo { get; set; }
        public bool chkAnchorOn { get; set; }
        public DateTime? ArrivalAlongsideDate { get; set; }
        public string ArrivalAlongsideTime { get; set; }
        public int Distance { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
