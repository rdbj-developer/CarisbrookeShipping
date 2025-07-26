using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class ArrivalReportModal
    {
        public int ShipNo { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public DateTime ReportCreated { get; set; }
        public int VoyageNo { get; set; }
        public string PortName { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public DateTime? TenderedDate { get; set; }
        public string TenderedTime { get; set; }
        public DateTime? POBDate { get; set; }
        public string POBTime { get; set; }
        public int TugsNo { get; set; }
        public bool chkAnchorOn { get; set; }
        public DateTime? ArrivalAlongsideDate { get; set; }
        public string ArrivalAlongsideTime { get; set; }
        public double AverageSpeed { get; set; }
        public double Distance { get; set; }
        public double FuelOil { get; set; }
        public double DieselOil { get; set; }
        public double SulphurFuelOil { get; set; }
        public double SulphurDieselOil { get; set; }
        public double FreshWater { get; set; }
        public double LubeOil { get; set; }
        public DateTime? CargoDate { get; set; }
        public string CargoTime { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string NextPort { get; set; }
        public string Remarks { get; set; }
        public string ToEmail { get; set; }
        public List<string> CCEmail { get; set; }
        public bool IsSynced { get; set; }
        public string CreatedBy { get; set; }
    }

}
