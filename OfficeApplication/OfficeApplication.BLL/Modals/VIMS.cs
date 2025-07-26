using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class VIMSLIST
    {
        public List<VIMS> VIMS { get; set; }
        public List<AccountList> AccountList { get; set; }
    }
    public class VIMS
    {
        public int REQNINVOICEID { get; set; }
        public double INVOICE_AMOUNT { get; set; }
        public double INVOICE_EXCHRATE { get; set; }
        public DateTime INVOICE_DATE { get; set; }
        public string INVOICENO { get; set; }
        public int VRID { get; set; }
        public double? NET_INVOICE_AMOUNT { get; set; }
        public string CURR_CODE { get; set; }
        public string PONO { get; set; }
    }
    public class AccountList
    {
        public string PONO { get; set; }
        public string POTITLE { get; set; }
        public string ACCOUNT_DESCR { get; set; }
        public double POTOTAL { get; set; }
        public DateTime PODATE { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public int VRID { get; set; }
        public int ACCOUNTID { get; set; }
    }
    public partial class SM_ACCOUNTCODE
    {
        public int ACCOUNTID { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string ACCOUNT_DESCR { get; set; }
        public string ACCOUNT_REMARKS { get; set; }
        public Nullable<int> EXPORTED { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public Nullable<int> UPDATE_SITE { get; set; }
        public Nullable<int> SITEID { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<int> UPDATE_BY { get; set; }
        public Nullable<int> CREATED_BY { get; set; }
        public Nullable<int> PARENTID { get; set; }
    }
}
