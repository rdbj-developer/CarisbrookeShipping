using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class Invoice
    {
        public string INVOICENO { get; set; }
        public double? INVOICE_AMOUNT { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string SupplierName { get; set; }
        public double POTOTALUSD { get; set; }
        public double? POTotalPOCurr { get; set; }
        public string POCurrency { get; set; }
        public string PONO { get; set; }
        public double INVOICEAMOUNTUSD { get; set; }
        public string InvoiceSent { get; set; }
        public double? INVOICEAMOUNTInvCurr { get; set; }
        public double? NETINVOICEAMOUNT { get; set; }
        public string POSTATUS { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public DateTime PODATE { get; set; }
        public DateTime? GoodsatForwarderRecvdDate { get; set; }
        public double? InvoiceExchrate { get; set; }
        public double? SumofInvoiceItems { get; set; }
    }
    public class CodeInvoice
    {
        public string Code { get; set; }
        public double Total { get; set; }
        public string Description { get; set; }
       
    }
    public class SubCodeInvoice
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public double Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
    }
    public class DisplayModalInvoice
    {
        public string Code { get; set; }
    
        public string Description { get; set; }
        public string Total_USD { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string Total { get; set; }
        public string SupplierName { get; set; }
        public string INVOICENO { get; set; }
        public dynamic INVOICE_AMOUNT { get; set; }
      
  
        public double POTOTALUSD { get; set; }
        public dynamic POTotalPOCurr { get; set; }
        public string POCurrency { get; set; }
        public string PONO { get; set; }
        public double INVOICEAMOUNTUSD { get; set; }
        public string InvoiceSent { get; set; }
        public dynamic INVOICEAMOUNTInvCurr { get; set; }
        public dynamic NETINVOICEAMOUNT { get; set; }
        public string POSTATUS { get; set; }
        public dynamic InvoiceDate { get; set; }
        public string InvoiceReceivedDate { get; set; }
        public dynamic PODATE { get; set; }
        public string GoodsatForwarderRecvdDate { get; set; }
        public dynamic InvoiceExchrate { get; set; }
        public dynamic SumofInvoiceItems { get; set; }
    }

    public class Budget
    {
     
        public string AccountCode { get; set; }
        public string Amount { get; set; }
        public string Group { get; set; }
    }
}
