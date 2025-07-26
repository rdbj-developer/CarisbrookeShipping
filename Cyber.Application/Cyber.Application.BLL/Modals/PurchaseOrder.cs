using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class PurchaseOrder
    {
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string PONO { get; set; }
        public double? POTOTALBASECURRENCY { get; set; }
        public string POTITLE { get; set; }
        public double POTOTAL { get; set; }
        public double POEXCHRATE { get; set; }
        public string POSTATUS { get; set; }
        public DateTime? PORECVDATE { get; set; }
        public DateTime? FORWARDER_RECVD_DATE { get; set; }
        public DateTime PODATE { get; set; }
        public string POCurrency { get; set; }
    }
    public class CodePurchaseOrder
    {
        public string Code { get; set; }
        public double Total { get; set; }
        public string Description { get; set; }

    }
    public class SubCodePurchaseOrder
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public double Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
    }
    public class DisplayModalPurchaseOrder
    {
        public string Code { get; set; }

        public string Description { get; set; }
        public string TotalUSD { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string Total { get; set; }
        public string PONO { get; set; }
        public double? POTOTALBASECURRENCY { get; set; }
        public string POTITLE { get; set; }
        public double POTOTAL { get; set; }
        public double POEXCHRATE { get; set; }
        public string POSTATUS { get; set; }
        public dynamic PORECVDATE { get; set; }
        public dynamic FORWARDER_RECVD_DATE { get; set; }
        public dynamic PODATE { get; set; }
        public string POCurrency { get; set; }
    }
}
