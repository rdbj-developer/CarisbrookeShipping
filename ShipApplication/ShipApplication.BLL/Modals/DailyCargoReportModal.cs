using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class DailyCargoReportModal
    {
        public int ShipNo { get; set; }
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public DateTime ReportCreated { get; set; }
        public int VoyageNo { get; set; }
        public string PortName { get; set; }
        public double NoOfGangs { get; set; }
        public double NoOfShips { get; set; }
        public double QuantityOfCargoLoaded { get; set; }
        public double TotalCargoLoaded { get; set; }
        public double CargoRemaining { get; set; }
        public double FuelOil { get; set; }
        public double DieselOil { get; set; }
        public double SulphurFuelOil { get; set; }
        public double SulphurDieselOil { get; set; }
        public double Sludge { get; set; }
        public double DirtyOil { get; set; }
        public DateTime ETCDate { get; set; }
        public string ETCTime { get; set; }
        public string NextPort { get; set; }
        public string Remarks { get; set; }
        public string ToEmail { get; set; }
        public List<string> CCEmail { get; set; }
        public bool IsSynced { get; set; }
        public string CargoType { get; set; }
        public string CreatedBy { get; set; }
    }
}
