//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarisbrookeShippingAPI.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class ArrivalReport
    {
        public long ARID { get; set; }
        public Nullable<int> ShipNo { get; set; }
        public string ShipName { get; set; }
        public string ReportCreated { get; set; }
        public Nullable<int> VoyageNo { get; set; }
        public string PortName { get; set; }
        public string ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string NORTenderedDate { get; set; }
        public string NORTenderedTime { get; set; }
        public string POBDate { get; set; }
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
        public string CargoDate { get; set; }
        public string CargoTime { get; set; }
        public string ETCDepartureDate { get; set; }
        public string ETCDepartureTime { get; set; }
        public string NextPort { get; set; }
        public string Remarks { get; set; }
        public string ToEmail { get; set; }
        public string CCEmail { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsSynced { get; set; }
        public string CreatedBy { get; set; }
    }
}
