using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class PurchaseOrder
    {
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string PONO { get; set; }
        public double? POTOTALBASECURRENCY { get; set; }
        public string POTITLE { get; set; }
        public double? POTOTAL { get; set; }
        public double? POEXCHRATE { get; set; }
        public string POSTATUS { get; set; }
        public DateTime? PORECVDATE { get; set; }
        public DateTime? FORWARDER_RECVD_DATE { get; set; }
        public DateTime PODATE { get; set; }
        public string POCurrency { get; set; }
    }
}
