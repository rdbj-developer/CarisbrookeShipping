using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
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
        public int? FleetId { get; set; }
    }

    public class DisplayModalPurchasingDept
    {
        public string ShipName { get; set; }
        public string PONumber { get; set; }
        public string El1 { get; set; }
        public Nullable<decimal> Value { get; set; }
        public Nullable<double> POTotal { get; set; }
        public string CompanyCode { get; set; }
        public string VendorName { get; set; }
        public string Account_Code { get; set; }
        public string Account_Descr { get; set; }
        public string EquipmentName { get; set; }
        public string CodaDocument { get; set; }
        public string CodaDocumentNumber { get; set; }
        public Nullable<System.DateTime> ModDate { get; set; }
        //public int FleetId { get; set; }
        //public string POYear { get; set; }
        //public int POMonth { get; set; }
    }
}
