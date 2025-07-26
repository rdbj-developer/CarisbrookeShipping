using System;
using System.Collections.Generic;

namespace ShipApplication.BLL.Modals
{
    public class DepartureReportModal
    {
        public int ShipNo { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public DateTime ReportCreated { get; set; }
        public int VoyageNo { get; set; }
        public string PortName { get; set; }
        public DateTime? DateCargoOperations { get; set; }
        public string TimeCargoOperations { get; set; }
        public string CargoOnBoard { get; set; }
        public string CargoLoaded { get; set; }
        public double DraftAFT { get; set; }
        public double DraftFWD { get; set; }
        public DateTime? POBDate { get; set; }
        public string POBTime { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public DateTime? POffDate { get; set; }
        public string POffTime { get; set; }
        public string NoOfTugs { get; set; }
        public double FuelOil { get; set; }
        public double DieselOil { get; set; }
        public double SulphurFuelOil { get; set; }
        public double SulphurDieselOil { get; set; }
        public string NextPort { get; set; }
        public DateTime ETADate { get; set; }
        public string ETATime { get; set; }
        public string IntendedRoute { get; set; }
        public string Remarks { get; set; }
        public string ToEmail { get; set; }
        public List<string> CCEmail { get; set; }
        public bool IsSynced { get; set; }
        public string CreatedBy { get; set; }
    }
}
