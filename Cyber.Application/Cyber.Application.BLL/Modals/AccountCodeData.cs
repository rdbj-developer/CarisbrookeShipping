using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class AccountCodeData
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
