using Mvc.Mailer;
using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ShipApplication.Mailers
{
    public class ShipMailer : MailerBase, IShipMailler
    {
        public virtual MvcMailMessage ArrivalReport(ArrivalReportModal Modal)
        {
            try
            {
                ViewBag.ArrivalReport = Modal;
                return Populate(x =>
                {
                    x.Subject = "Arrival Report - " + Modal.ShipName + " - " + Modal.PortName;
                    x.ViewName = "ArrivalReportView";
                    x.To.Add(Modal.ToEmail);
                });
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return null;
            }
        }
        public virtual MvcMailMessage DepartureReport(DepartureReportModal Modal)
        {
            try
            {
                ViewBag.DepartureReport = Modal;
                return Populate(x =>
                {
                    x.Subject = "Departure Report - " + Modal.ShipName + " - " + Modal.PortName;
                    x.ViewName = "DepartureReportView";
                    x.To.Add(Modal.ToEmail);
                });
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return null;
            }
        }
        public virtual MvcMailMessage DailyCargoReport(DailyCargoReportModal Modal)
        {
            try
            {
                ViewBag.DailyCargoReport = Modal;
                return Populate(x =>
                {
                    x.Subject = "Daily Cargo Report - " + Modal.ShipName + " - " + Modal.PortName;
                    x.ViewName = "DailyCargoReportView";
                    x.To.Add(Modal.ToEmail);
                });
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return null;
            }
        }
        public virtual MvcMailMessage DailyPositionReport(DailyPositionReportModal Modal)
        {
            try
            {
                string[] formats = { "dd/MM/yyyy" };
                var dtEstimatedArrivalDateEcoSpeed = DateTime.ParseExact(Modal.EstimatedArrivalDateEcoSpeed, formats, new CultureInfo("en-GB"), DateTimeStyles.None);
                var dtEstimatedArrivalDateFullSpeed = DateTime.ParseExact(Modal.EstimatedArrivalDateFullSpeed, formats, new CultureInfo("en-GB"), DateTimeStyles.None);
                Modal.EstimatedArrivalDateEcoSpeed = dtEstimatedArrivalDateEcoSpeed.ToString("dd/MMM/yyyy");
                Modal.EstimatedArrivalDateFullSpeed = dtEstimatedArrivalDateFullSpeed.ToString("dd/MMM/yyyy");
                ViewBag.DailyPositionReport = Modal;
                return Populate(x =>
                {
                    x.Subject = "Daily Position Report - " + Modal.ShipName;
                    x.ViewName = "DailyPositionReportView";
                    x.To.Add(Modal.ToEmail);
                });
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return null;
            }
        }
    }
}