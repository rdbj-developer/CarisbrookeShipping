using System;

namespace AWS_PO_UpdateService.Modals
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
}
