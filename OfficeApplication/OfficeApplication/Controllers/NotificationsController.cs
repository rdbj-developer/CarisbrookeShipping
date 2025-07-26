using OfficeApplication.BLL.Modals;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace OfficeApplication.Controllers
{
    //RDBJ 10/12/2021 Added Notifications Controller
    [SessionExpire]
    public class NotificationsController : Controller
    {
        
        //RDBJ 10/12/2021 Implement start
        public ActionResult Index()
        {
            //RDBJ 10/14/2021 change class modal and return modal
            NotificationsModal modal = new NotificationsModal();
            APIHelper _apiHelper = new APIHelper();

            string strSessionUserID = Session["UserID"].ToString(); // RDBJ 12/18/2021
            int intSessionUserGroup = Convert.ToInt32(Session["UserGroup"]); // RDBJ 12/18/2021

            modal.GISINotifications = _apiHelper.GetNotifications(
                strSessionUserID, intSessionUserGroup   // RDBJ 12/18/2021
                , SessionManager.Username // RDBJ 01/05/2022
                );

            ViewBag.Notification = modal.GISINotifications;

            return View(modal);
        }

        // JSL 06/29/2022
        public ActionResult NewNotification()
        {
            return View();
        }
        // End JSL 06/29/2022

        // JSL 06/30/2022
        public JsonResult GetNewNotifications()
        {
            NotificationsModal modal = new NotificationsModal();
            APIHelper _apiHelper = new APIHelper();
            Dictionary<string, string> retDictMetadata = new Dictionary<string, string>();
            string strDeficienciesNotificationData = string.Empty;
            string strSessionUserID = Session["UserID"].ToString();

            Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
            dictMetaData["CurrentUserId"] = strSessionUserID;
            dictMetaData["strAction"] = AppStatic.API_GETNOTIFICATION;
            retDictMetadata = _apiHelper.PostAsyncAPICall(APIURLHelper.APINotifications, "CommonPostAPICall", dictMetaData);

            if (retDictMetadata.ContainsKey("DeficienciesNotificationsList"))
                strDeficienciesNotificationData = retDictMetadata["DeficienciesNotificationsList"];

            return Json(new { NotificationsList = strDeficienciesNotificationData });
        }
        // End JSL 06/30/2022

        //RDBJ 10/18/2021 this method is use for auto refresh
        public JsonResult GetNotifications()
        {
            NotificationsModal modal = new NotificationsModal();
            APIHelper _apiHelper = new APIHelper();

            string strSessionUserID = Session["UserID"].ToString(); // RDBJ 12/18/2021
            int intSessionUserGroup = Convert.ToInt32(Session["UserGroup"]); // RDBJ 12/18/2021

            modal.GISINotifications = _apiHelper.GetNotifications(
                strSessionUserID, intSessionUserGroup   // RDBJ 12/18/2021
                , SessionManager.Username // RDBJ 01/05/2022
                );

            ViewBag.Notification = modal.GISINotifications;
            return Json(modal);
        }
        //End RDBJ 10/18/2021

        //RDBJ 10/16/2021
        public JsonResult GetCountOfNotifications()
        {
            long res = 0;
            APIHelper _helper = new APIHelper();

            string strSessionUserID = Session["UserID"].ToString(); // RDBJ 12/18/2021
            int intSessionUserGroup = Convert.ToInt32(Session["UserGroup"]); // RDBJ 12/18/2021

            res = _helper.GetCountOfNotifications(
                strSessionUserID, intSessionUserGroup   // RDBJ 12/18/2021
                , SessionManager.Username // RDBJ 01/05/2022
                );

            return Json(new { totalNotification = res});
        }
        //End RDBJ 10/16/2021

        //RDBJ 10/16/2021
        public JsonResult GetCountOfNotificationsById(string id, string formType) //RDBJ 10/21/2021 Added formType
        {
            Notifications notificationsCounts = new Notifications();
            APIHelper _helper = new APIHelper();
            notificationsCounts = _helper.GetCountOfNotificationsById(id, formType
                , SessionManager.Username // RDBJ 01/05/2022
                ); //RDBJ 10/21/2021 Added formType

            return Json(new { 
                totalNewComments = notificationsCounts.CommentsCount,
                totalNewInitialActions = notificationsCounts.InitialActionsCount,
                totalNewResolutions = notificationsCounts.ResolutionsCount
            }, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/16/2021

        //RDBJ 10/16/2021
        public JsonResult openAndSeenNotificationsUpdateStatusById(string id, string section, string formType) //RDBJ 10/21/2021 Added formType
        {
            bool res = false;
            APIHelper _helper = new APIHelper();

            // RDBJ 04/01/2022
            Dictionary<string, object> dicData = new Dictionary<string, object>();
            dicData["CurrentUserID"] = Session["UserID"].ToString();
            dicData["DefOrNoteID"] = id;
            dicData["Section"] = section;
            dicData["FormType"] = formType;
            // End RDBJ 04/01/2022

            // JSL 12/21/2022 wrapped in if
            if (SessionManager.UserGroup != "8")
            {
                //res = _helper.openAndSeenNotificationsUpdateStatusById(id, section, formType); //RDBJ 10/21/2021 Added formType
                res = _helper.openAndSeenNotificationsUpdateStatusById(dicData); // RDBJ 04/01/2022
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //End RDBJ 10/16/2021

        // RDBJ 12/21/2021
        public JsonResult LoadAssignedToMeGISIIADeficiencies()
        {
            NotificationsModal modal = new NotificationsModal();
            APIHelper _apiHelper = new APIHelper();

            string strSessionUserID = Session["UserID"].ToString();
            modal.GISINotifications = _apiHelper.LoadAssignedToMeGISIIADeficiencies(
                strSessionUserID
                );
            return Json(modal.GISINotifications, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/21/2021

        // JSL 04/30/2022
        public JsonResult PerformAction(string jsonmetadata, string straction)
        {
            APIHelper _helper = new APIHelper();
            //Dictionary<string, object> dictToReturn = new Dictionary<string, object>();

            Dictionary<string, string> dictToReturn = new Dictionary<string, string>();
            dictToReturn.Add("jsonMetaData", jsonmetadata);
            dictToReturn.Add("straction", straction);

            var dictMetadata = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(jsonmetadata))
            {
                dictMetadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonmetadata);
                dictMetadata["strAction"] = straction;

                try
                {
                    dictToReturn = _helper.PostAsyncAPICall(APIURLHelper.APINotifications, "CommonPostAPICall", dictMetadata);
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Forms PerformAction : " + straction + " Error : " + ex.Message);
                }
            }

            return Json(dictToReturn, JsonRequestBehavior.AllowGet);
        }
        // End JSL 04/30/2022
    }
}