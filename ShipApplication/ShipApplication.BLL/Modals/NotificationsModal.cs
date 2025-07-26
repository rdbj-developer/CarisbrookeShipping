using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    //RDBJ 10/25/2021 Added this Modal
    public class NotificationsModal
    {
        public List<Notifications> GISINotifications { get; set; }
    }

    //RDBJ 10/25/2021
    public class Notifications
    {
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public string ReportType { get; set; }
        public string Deficiency { get; set; }
        public Guid? DeficienciesUniqueID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long CommentsCount { get; set; }
        public long ResolutionsCount { get; set; }
        public long InitialActionsCount { get; set; }
    }
    //End RDBJ 10/25/2021
}
