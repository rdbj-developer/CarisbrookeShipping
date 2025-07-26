using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBModificationService.Modals
{
    public class SM_DEFECT_REMARKS_MODAL
    {
        public string DEFECTNO { get; set; }
        public string DEFECTTITLE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string REMARK { get; set; }
        public DateTime REMARK_UPDATE_DATE { get; set; }
        public string SHIP_DESCR { get; set; }
        public string USERNAME { get; set; }
    }
    public class SM_DEFECT_REMARKS_EMAIL_MODAL
    {
        public string SHIP_DESCR { get; set; }
        public string DEFECTNO { get; set; }
        public string DEFECTTITLE { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string USERNAME { get; set; }
        public string REMARK { get; set; }
        public string EmailAddress { get; set; }
        public string CCEmailAddress { get; set; }
        public string CCEmail_Manager { get; set; }
    }
}
