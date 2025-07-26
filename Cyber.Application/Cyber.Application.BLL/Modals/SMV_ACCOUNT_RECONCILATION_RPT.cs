using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class SMV_ACCOUNT_RECONCILATION_RPT
    {
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string PONO { get; set; }
        public string POTITLE { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceSent { get; set; }
        public double? POTOTALBASECURRENCY { get; set; }
        public double? POTOTAL { get; set; }
        public double? POEXCHRATE { get; set; }
        public string INVOICENo { get; set; }
        public double? INVOICEAMOUNTBASECURRENCY { get; set; }
        public double? INVOICEAMOUNT { get; set; }
        public double? NETINVOICEAMOUNT { get; set; }
        public string POSTATUS { get; set; }
        public DateTime? PORECVDATE { get; set; }
        public DateTime? FORWARDER_RECVD_DATE { get; set; }
        public DateTime PODATE { get; set; }
        public string POCurrency { get; set; }
        public DateTime? INVOICEDATE { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
    }
    public class OpexReportModalList
    {
        public List<MainCode> DispList { get; set; }
    }
    public class DisplayModalList
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Total { get; set; }

        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string PONO { get; set; }
        public string POTITLE { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceSent { get; set; }
        public double? POTOTALBASECURRENCY { get; set; }
        public double? POTOTAL { get; set; }
        public double? POEXCHRATE { get; set; }
        public string INVOICENo { get; set; }
        public double? INVOICEAMOUNTBASECURRENCY { get; set; }
        public double? INVOICEAMOUNT { get; set; }
        public double? NETINVOICEAMOUNT { get; set; }
        public string POSTATUS { get; set; }
        public DateTime? PORECVDATE { get; set; }
        public DateTime? FORWARDER_RECVD_DATE { get; set; }
        public DateTime PODATE { get; set; }
        public string POCurrency { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public DateTime? INVOICEDATE { get; set; }
    }
    public class MainCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Total { get; set; }
    }
    public class SubCode
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
        public string Budget { get; set; }
    }
    public class Export
    {

        public string AC_Code { get; set; }
        public string AC_Description { get; set; }
        public string Total_USD { get; set; }

        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Total { get; set; }
        public string PO_NO { get; set; }
       
        public string Supplier_Name { get; set; }
        public string Invoice_Sent { get; set; }
        public string  PO_TOTAL_SHIP_CURRENCY { get; set; }
        public string PO_TOTAL { get; set; }
        public string PO_EXCHRATE { get; set; }
        public string INVOICE_No { get; set; }
        public string INVOICE_AMOUNT_SHIP_CURRENCY { get; set; }
        public string INVOICE_AMOUNT { get; set; }
        public string InvoiceReceivedDate { get; set; }
        public string NET_INVOICE_AMOUNT { get; set; }
        public string PO_STATUS { get; set; }
        public string PO_Received_Date { get; set; }
        public string RECEIVED_AT_FREIGHT_FORWARDER { get; set; }
        public dynamic PO_DATE { get; set; }
        public string PO_Currency { get; set; }
        public string INVOICE_DATE { get; set; }
    }
}
