using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Modals
{
    public class OpexReportFinilizeDate
    {
        public Guid FinilizeID { get; set; }
        public Nullable<System.Guid> FinilizeBy { get; set; }
        public Nullable<System.DateTime> StardDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}
