using System;

namespace CarisbrookeShippingService.BLL.Modals
{
    public class DailyPositionReport
    {
        public long DPRID { get; set; }
        public int ShipNo { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public DateTime ReportCreated { get; set; }
        public int VoyageNo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Latitudedd { get; set; }
        public string Latitudemm { get; set; }
        public string Longitudeddd { get; set; }
        public string Longitudemm { get; set; }
        public bool chkAnchored { get; set; }
        public double AverageSpeed { get; set; }
        public double DistanceMade { get; set; }
        public string NextPort { get; set; }
        public DateTime EstimatedArrivalDateEcoSpeed { get; set; }
        public string EstimatedArrivalTimeEcoSpeed { get; set; }
        public DateTime EstimatedArrivalDateFullSpeed { get; set; }
        public string EstimatedArrivalTimeFullSpeed { get; set; }
        public double FuelOil { get; set; }
        public double DieselOil { get; set; }
        public double SulphurFuelOil { get; set; }
        public double SulphurDieselOil { get; set; }
        public double FreshWater { get; set; }
        public double LubeOil { get; set; }
        public double Sludge { get; set; }
        public double DirtyOil { get; set; }
        public double Pitch { get; set; }
        public double EngineLoad { get; set; }
        public double HighCylExhTemp { get; set; }
        public double ExhGasTempAftTurboChrg { get; set; }
        public double OilCunsum { get; set; }
        public string WindDirection { get; set; }
        public string WindForce { get; set; }
        public string SeaState { get; set; }
        public string SwellDirection { get; set; }
        public double SwellHeight { get; set; }
        public double DraftAft { get; set; }
        public double DraftForward { get; set; }
        public string Remarks { get; set; }
        public string ToEmail { get; set; }
        public string CCEmail { get; set; }
        public bool IsSynced { get; set; }
        public string CargoType { get; set; }
        public string CreatedBy { get; set; }
    }
}
