using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    //RDBJ 10/25/2021 Added Notifications Controller
    [AuthorizeFilter] //RDBJ 10/25/2021
    public class NotificationsController : Controller
    {
        //RDBJ 10/25/2021
        public ActionResult Index()
        {
            NotificationsModal modal = new NotificationsModal();
            NotificationsHelper _localHelper = new NotificationsHelper();
            modal.GISINotifications = _localHelper.GetNotifications();
            ViewBag.Notification = modal.GISINotifications;

            return View(modal);
        }

        //RDBJ 10/26/2021 this method is use for auto refresh
        public JsonResult GetNotifications()
        {
            NotificationsModal modal = new NotificationsModal();
            NotificationsHelper _localHelper = new NotificationsHelper();
            modal.GISINotifications = _localHelper.GetNotifications();
            ViewBag.Notification = modal.GISINotifications;
            return Json(modal);
        }
        //End RDBJ 10/26/2021

        //RDBJ 10/26/2021
        public JsonResult GetCountOfNotifications()
        {
            long res = 0;
            NotificationsHelper _localHelper = new NotificationsHelper();
            res = _localHelper.GetCountOfNotifications();
            return Json(new { totalNotification = res });
        }
        //End RDBJ 10/26/2021

        //RDBJ 10/26/2021
        public JsonResult GetCountOfNotificationsById(string id, string formType)
        {
            Notifications notificationsCounts = new Notifications();
            NotificationsHelper _localHelper = new NotificationsHelper();
            notificationsCounts = _localHelper.GetCountOfNotificationsById(id, formType);

            return Json(new
            {
                totalNewComments = notificationsCounts.CommentsCount,
                totalNewInitialActions = notificationsCounts.InitialActionsCount,
                totalNewResolutions = notificationsCounts.ResolutionsCount
            }, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/26/2021

        //RDBJ 10/26/2021
        public JsonResult openAndSeenNotificationsUpdateStatusById(string id, string section, string formType)
        {
            bool res = false;
            NotificationsHelper _localHelper = new NotificationsHelper();
            res = _localHelper.openAndSeenNotificationsUpdateStatusById(id, section, formType);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/26/2021
    }
}