using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class SMV_BUDGET_OVERVIEW_1
    {
        public string PARENT_CODE { get; set; }
        public string PARENT_DESCR { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string ACCOUNT_DESCR { get; set; }
        public int BUDGETYEAR { get; set; }
        public Int16 IS_ACTIVE { get; set; }
        public double BUDGET_AMT { get; set; }
        public double MONTHLY_BUDGET { get; set; }
        public int USERID_BUDGETBY { get; set; }
        public int BUDGETID { get; set; }
        public int PARENTID { get; set; }
        public string BASECURRENCY { get; set; }
        public double P01 { get; set; }
        public double P02 { get; set; }
        public double P03 { get; set; }
        public double P04 { get; set; }
        public double P05 { get; set; }
        public double P06 { get; set; }
        public double P07 { get; set; }
        public double P08 { get; set; }
        public double P09 { get; set; }
        public double P10 { get; set; }
        public double P11 { get; set; }
        public double P12 { get; set; }
        public int SITEID { get; set; }
    }
    public class OpexReportModal
    {
        public List<DisplayModal> DispList { get; set; }
        public List<DisplayModal> PrintList { get; set; }

    }
    public class DisplayModal
    {
        public string CODE { get; set; }
        public string ACCOUNT_CODE { get; set; }
        public string ACCOUNT_DESCR { get; set; }
        public string TOTAL { get; set; }
        public string TOTALAMT { get; set; }

    }
}
