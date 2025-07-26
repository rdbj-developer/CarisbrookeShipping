using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBModificationService.Modals
{
    public class SM_DEFECT_MAINTENANCE_MODAL
    {

        public int DEFECTID { get; set; }
        public string DEFECTNO { get; set; }
        public string DEFECTTITLE { get; set; }
        public string LOCATION { get; set; }
        public int ASSISTANCE_ID { get; set; }
        public DateTime OBSERVED_DATE { get; set; }
        public DateTime OPEN_DATE { get; set; }
        public DateTime CLOSED_DATE { get; set; }
        public int TYPEID { get; set; }
        public int PRIORTYID { get; set; }
        public int EQUIPID { get; set; }
        public int REQUINID { get; set; }
        public int STATUS { get; set; }
        public int CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
        public int SITEID { get; set; }
        public int UPDATESITE { get; set; }
        public int EXPORTED { get; set; }
        public string SHIP_DESCR { get; set; }
        public string USERNAME { get; set; }
    }
    public class SM_DEFECT_MAINTENANCE_Email_MODAL
    {
        public string DEFECTNO { get; set; }
        public string DEFECTTITLE { get; set; }
        public DateTime OPEN_DATE { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string USERNAME { get; set; }
        public string SHIP_DESCR { get; set; }
    }
}
