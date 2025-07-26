using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class ShipReportsAnalysisModal
    {
        public long ID { get; set; }
        public string ShipName { get; set; }
        public string ReportName { get; set; }
        public int VoyageNo { get; set; }
        public string PortName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string Inspector { get; set; }
        public string Superintended { get; set; }        
    }

    public class ShipModal {
        public string shipCode { get; set; }
        public string shipName { get; set; }        
    }
}
