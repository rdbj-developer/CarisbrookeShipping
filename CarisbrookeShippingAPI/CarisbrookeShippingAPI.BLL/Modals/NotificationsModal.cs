using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    //RDBJ 10/14/2021 Added this Modal
    public class NotificationsModal
    {
    }

    //RDBJ 10/14/2021
    public class Notifications
    {
        public string Ship { get; set; }
        public string ShipName { get; set; } //RDBJ 10/21/2021
        public string ReportType { get; set; } //RDBJ 10/21/2021
        public string Deficiency { get; set; }
        public string AssignTo { get; set; } // RDBJ 12/21/2021
        public Guid? DeficienciesUniqueID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long CommentsCount { get; set; }
        public long ResolutionsCount { get; set; }
        public long InitialActionsCount { get; set; }
    }
    //End RDBJ 10/14/2021

    // JSL 06/30/2022
    public class RecentNotifications
    {
        public Guid? UniqueDataId { get; set; }
        public string ShipName { get; set; } 
        public string ReportType { get; set; }
        public string Number { get; set; }
        public string Deficiency { get; set; }
        public string Priority { get; set; }
        public string DataType { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }
    // End JSL 06/30/2022
}
