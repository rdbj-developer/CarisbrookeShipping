using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class Invoice
    {
        public string INVOICENO { get; set; }
        public double? INVOICE_AMOUNT { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string SupplierName { get; set; }
        public double? POTOTALUSD { get; set; }
        public double? POTotalPOCurr { get; set; }
        public string POCurrency { get; set; }
        public string PONO { get; set; }
        public double? INVOICEAMOUNTUSD { get; set; }
        public string InvoiceSent { get; set; }
        public double? INVOICEAMOUNTInvCurr { get; set; }
        public double? NETINVOICEAMOUNT { get; set; }
        public string POSTATUS { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public DateTime? PODATE { get; set; }
        public DateTime? GoodsatForwarderRecvdDate { get; set; }
        public double? InvoiceExchrate { get; set; }
        public double? SumofInvoiceItems { get; set; }
    }
}
