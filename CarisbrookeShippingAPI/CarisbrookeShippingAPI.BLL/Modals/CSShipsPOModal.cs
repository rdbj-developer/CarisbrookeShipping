using System;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class CSShipsPOModal
    {
        public string SITENAME { get; set; }
        public string PONO { get; set; }
        public string VENDOR_ADDR_NAME { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string ACCOUNT_DESCR { get; set; }
        public Nullable<System.DateTime> PORECVDATE { get; set; }
        public Nullable<double> POTOTAL { get; set; }
        public Nullable<double> POEXCHRATE { get; set; }
        public Nullable<double> POTOTAL_BASE { get; set; }
        public Nullable<System.DateTime> FORWARDER_RECVD_DATE { get; set; }
        public string INVOICENO { get; set; }
        public string DEPT_CODE { get; set; }
        public string CURR_CODE { get; set; }
        public string EQUIP_NAME { get; set; }
        public Nullable<System.DateTime> PODATE { get; set; }
        public string INVOICE_PRESENT { get; set; }
    }
public class PurchasingDeptModel
    {
        public System.Guid Id { get; set; }
        public string CmpCode { get; set; }
        public string DocCode { get; set; }
        public string DocNum { get; set; }
        public Nullable<System.DateTime> ModDate { get; set; }
        public string El1 { get; set; }
        public Nullable<decimal> ValueDoc { get; set; }
        public string Descr { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Invoice { get; set; }
        public string SiteName { get; set; }
        public string PONO { get; set; }
        public string Vendor_Addr_Name { get; set; }
        public string Account_Code { get; set; }
        public string Account_Descr { get; set; }
        public Nullable<System.DateTime> PORecVDate { get; set; }
        public Nullable<double> POTotal { get; set; }
        public Nullable<double> POExchRate { get; set; }
        public Nullable<double> POTotal_Base { get; set; }
        public Nullable<System.DateTime> Forwarder_Recvd_Date { get; set; }
        public string InvoiceNo { get; set; }
        public string Dept_Code { get; set; }
        public string Curr_Code { get; set; }
        public string Equip_Name { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public Nullable<int> FleetId { get; set; }
        public string ShipName { get; set; }
        public string POYear { get; set; }
        public int POMonth { get; set; }
    }
}
