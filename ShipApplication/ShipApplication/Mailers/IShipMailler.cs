using Mvc.Mailer;
using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.Mailers
{
    public interface IShipMailler
    {
        MvcMailMessage ArrivalReport(ArrivalReportModal Modal);
        MvcMailMessage DepartureReport(DepartureReportModal Modal);
        MvcMailMessage DailyCargoReport(DailyCargoReportModal Modal);
        MvcMailMessage DailyPositionReport(DailyPositionReportModal Modal);
    }
}
