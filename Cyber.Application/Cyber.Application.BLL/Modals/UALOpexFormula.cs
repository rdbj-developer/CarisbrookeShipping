using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class UALOpexFormula
    {
        public List<ActualInvoices> ActualInvoices { set; get; }
        public List<InvoicesReceivedCurrentMonth> InvoicesReceivedCurrentMonth { set; get; }
        public List<POsCurrentMonth> POsCurrentMonth { set; get; }
        public List<OpenPOsPreviousMonths> OpenPOsPreviousMonths { get; set; }
        public List<PreviousMonthPObalance> PreviousMonthPObalance { get; set; }

    }

    public class ActualInvoices
    {
        public string INVOICENO { get; set; }
        public double? INVOICE_AMOUNT { get; set; }
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Supplier_Name { get; set; }
        public double PO_TOTALUSD { get; set; }
        public double PO_Total_PO_Curr { get; set; }
        public string PO_Currency { get; set; }
        public string PO_NO { get; set; }
        public double INVOICE_AMOUNT_USD { get; set; }
        public string Invoice_Sent { get; set; }
        public double INVOICE_AMOUNT_Inv_Curr { get; set; }
        public double NET_INVOICE_AMOUNT { get; set; }
        public string PO_STATUS { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public DateTime? Invoice_Received_Date { get; set; }
        public DateTime PO_DATE { get; set; }
        public DateTime? Goods_at_Forwarder_Recvd_Date { get; set; }
        public double Invoice_Exch_rate { get; set; }
        public double Sum_of_Invoice_Items { get; set; }

    }
    public class ActualInvoicesCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Total { get; set; }
    }
    public class ActualInvoicesSubCode
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public double Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
    }
    public class DisplayModalActualInvoices
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string TotalUSD { get; set; }
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Total { get; set; }
        public string INVOICENO { get; set; }
        public double? INVOICE_AMOUNT { get; set; }
        public string Supplier_Name { get; set; }
        public double PO_TOTALUSD { get; set; }
        public double PO_Total_PO_Curr { get; set; }
        public string PO_Currency { get; set; }
        public string PO_NO { get; set; }
        public double INVOICE_AMOUNT_USD { get; set; }
        public string Invoice_Sent { get; set; }
        public double INVOICE_AMOUNT_Inv_Curr { get; set; }
        public double NET_INVOICE_AMOUNT { get; set; }
        public string PO_STATUS { get; set; }
        public dynamic Invoice_Date { get; set; }
        public dynamic Invoice_Received_Date { get; set; }
        public dynamic PO_DATE { get; set; }
        public dynamic Goods_at_Forwarder_Recvd_Date { get; set; }
        public double Invoice_Exch_rate { get; set; }
        public double Sum_of_Invoice_Items { get; set; }

    }

    public class InvoicesReceivedCurrentMonth
    {
        public string INVOICENO { get; set; }
        public double? INVOICE_AMOUNT { get; set; }
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Supplier_Name { get; set; }
        public double PO_TOTALUSD { get; set; }
        public double PO_Total_PO_Curr { get; set; }
        public string PO_Currency { get; set; }
        public string PO_NO { get; set; }
        public double INVOICE_AMOUNT_USD { get; set; }
        public string Invoice_Sent { get; set; }
        public double INVOICE_AMOUNT_Inv_Curr { get; set; }
        public double NET_INVOICE_AMOUNT { get; set; }
        public string PO_STATUS { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public DateTime? Invoice_Received_Date { get; set; }
        public DateTime PO_DATE { get; set; }
        public DateTime? Goods_at_Forwarder_Recvd_Date { get; set; }
        public double Invoice_Exch_rate { get; set; }
        public double Sum_of_Invoice_Items { get; set; }
    }
    public class InvoicesReceivedCurrentMonthCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Total { get; set; }
    }
    public class InvoicesReceivedCurrentMonthSubCode
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public double Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
    }
    public class DisplayModalInvoicesReceivedCurrentMonth
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string TotalUSD { get; set; }
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Total { get; set; }
        public string INVOICENO { get; set; }
        public double? INVOICE_AMOUNT { get; set; }
        public string Supplier_Name { get; set; }
        public double PO_TOTALUSD { get; set; }
        public double PO_Total_PO_Curr { get; set; }
        public string PO_Currency { get; set; }
        public string PO_NO { get; set; }
        public double INVOICE_AMOUNT_USD { get; set; }
        public string Invoice_Sent { get; set; }
        public double INVOICE_AMOUNT_Inv_Curr { get; set; }
        public double NET_INVOICE_AMOUNT { get; set; }
        public string PO_STATUS { get; set; }
        public dynamic Invoice_Date { get; set; }
        public dynamic Invoice_Received_Date { get; set; }
        public dynamic PO_DATE { get; set; }
        public dynamic Goods_at_Forwarder_Recvd_Date { get; set; }
        public double Invoice_Exch_rate { get; set; }
        public double Sum_of_Invoice_Items { get; set; }

    }
    

    public class POsCurrentMonth
    {
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string PO_NO { get; set; }
        public double PO_TOTAL_BASE_CURRENCY { get; set; }
        public double PO_TOTAL { get; set; }
        public double PO_EXCH_RATE { get; set; }
        public string PO_STATUS { get; set; }
        public DateTime? PORECVDATE { get; set; }
        public DateTime? FORWARDER_RECVD_DATE { get; set; }
        public DateTime PO_DATE { get; set; }
        public string PO_Currency { get; set; }
    }
    public class POsCurrentMonthCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Total { get; set; }
    }
    public class POsCurrentMonthSubCode
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public double Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
    }
    public class DisplayModalPOsCurrentMonth
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string TotalUSD { get; set; }
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Total { get; set; }
        public string PO_NO { get; set; }
        public double PO_TOTAL_BASE_CURRENCY { get; set; }
        public double PO_TOTAL { get; set; }
        public double PO_EXCH_RATE { get; set; }
        public string PO_STATUS { get; set; }
        public dynamic PORECVDATE { get; set; }
        public dynamic  FORWARDER_RECVD_DATE { get; set; }
        public dynamic PO_DATE { get; set; }
        public string PO_Currency { get; set; }
    }

    public class OpenPOsPreviousMonths
    {
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string PO_NO { get; set; }
        public double PO_TOTAL_BASE_CURRENCY { get; set; }
        public double PO_TOTAL { get; set; }
        public double PO_EXCH_RATE { get; set; }
        public string PO_STATUS { get; set; }
        public DateTime? PORECVDATE { get; set; }
        public DateTime? FORWARDER_RECVD_DATE { get; set; }
        public DateTime PO_DATE { get; set; }
        public string PO_Currency { get; set; }
    }
    public class OpenPOsPreviousMonthsCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Total { get; set; }
    }
    public class OpenPOsPreviousMonthsSubCode
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public double Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
    }
    public class DisplayModalOpenPOsPreviousMonths
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string TotalUSD { get; set; }
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Total { get; set; }
        public string PO_NO { get; set; }
        public double PO_TOTAL_BASE_CURRENCY { get; set; }
        public double PO_TOTAL { get; set; }
        public double PO_EXCH_RATE { get; set; }
        public string PO_STATUS { get; set; }
        public dynamic PORECVDATE { get; set; }
        public dynamic FORWARDER_RECVD_DATE { get; set; }
        public dynamic PO_DATE { get; set; }
        public string PO_Currency { get; set; }
    }


    public class PreviousMonthPObalance
    {
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string PO_NO { get; set; }
        public double PO_TOTAL_BASE_CURRENCY { get; set; }
        public double PO_TOTAL { get; set; }
        public double PO_EXCH_RATE { get; set; }
        public string PO_STATUS { get; set; }
        public DateTime? PORECVDATE { get; set; }
        public DateTime? FORWARDER_RECVD_DATE { get; set; }
        public DateTime PO_DATE { get; set; }
        public string PO_Currency { get; set; }
    }
    public class PreviousMonthPObalanceCode
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Total { get; set; }
    }
    public class PreviousMonthPObalanceSubCode
    {
        public string Code { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public double Total { get; set; }
        public string ACCOUNT_DESCR { get; set; }
    }
    public class DisplayModalPreviousMonthPObalance
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string TotalUSD { get; set; }
        public string Account_Code { get; set; }
        public string Account_Description { get; set; }
        public string Total { get; set; }
        public string PO_NO { get; set; }
        public double PO_TOTAL_BASE_CURRENCY { get; set; }
        public double PO_TOTAL { get; set; }
        public double PO_EXCH_RATE { get; set; }
        public string PO_STATUS { get; set; }
        public dynamic PORECVDATE { get; set; }
        public dynamic FORWARDER_RECVD_DATE { get; set; }
        public dynamic PO_DATE { get; set; }
        public string PO_Currency { get; set; }
    }



    public class FinalOpexCode
    {
        public string Code { get; set; }
        
        public double ActualInvoicesCurrentMonth { get; set; }
        public double InvoicesReceivedCurrentMonth { get; set; }
        public double OpenPOsCurrentMonth { get; set; }
        public double OpenPOsPreviousMonths { get; set; }
        public double PreviousMonthPObalance { get; set; }
        public double Opex { get; set; }
    }
    public class FinalOpexSubcode
    {
        public string Code { get; set; }
        public string Account_Code { get; set; }
     
        public double ActualInvoicesCurrentMonth { get; set; }
        public double InvoicesReceivedCurrentMonth { get; set; }
        public double OpenPOsCurrentMonth { get; set; }
        public double OpenPOsPreviousMonths { get; set; }
        public double PreviousMonthPObalance { get; set; }
        public double Opex { get; set; }
    }
}
