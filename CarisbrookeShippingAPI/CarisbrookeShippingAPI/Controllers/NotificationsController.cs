using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    //RDBJ 10/12/2021 Added Notifications Controller
    public class NotificationsController : ApiController
    {
        //RDBJ 10/14/2021
        [HttpGet]
        public List<Notifications> GetNotifications(
            string UserID, int UserGroup    // RDBJ 12/18/2021
             , string crntUserName // RDBJ 01/05/2022
            )
        {
            NotificationsHelper _helper = new NotificationsHelper();
            List<Notifications> list = _helper.GetNotifications(UserID, UserGroup
                , crntUserName // RDBJ 01/05/2022
                ); // RDBJ 12/18/2021 pass UserID and UserGroup //RDBJ 10/14/2021
            return list;
        }

        //RDBJ 10/16/2021
        public long GetCountOfNotifications(
            string UserID, int UserGroup    // RDBJ 12/18/2021
             , string crntUserName // RDBJ 01/05/2022
            )
        {
            long totalNotifications = 0;
            NotificationsHelper _helper = new NotificationsHelper();
            totalNotifications = _helper.GetCountOfNotifications(UserID, UserGroup
                , crntUserName // RDBJ 01/05/2022
                ); // RDBJ 12/18/2021 pass UserID and UserGroup
            return totalNotifications;
        }
        //End RDBJ 10/16/2021

        //RDBJ 10/16/2021
        public Notifications GetCountOfNotificationsById(string id, string formType
            , string crntUserName // RDBJ 01/05/2022
            )  //RDBJ 10/21/2021 Added formType
        {
            Notifications notificationDetails = new Notifications();
            NotificationsHelper _helper = new NotificationsHelper();
            notificationDetails = _helper.GetCountOfNotificationsById(id, formType
                , crntUserName // RDBJ 01/05/2022
                );  //RDBJ 10/21/2021 Added formType
            return notificationDetails;
        }
        //End RDBJ 10/16/2021

        // RDBJ 04/01/2022 commented below function
        /*
        //RDBJ 10/16/2021
        [HttpGet]
        public bool openAndSeenNotificationsUpdateStatusById(string id, string section, string formType) //RDBJ 10/21/2021 Added formType
        {
            bool blnUpdateStatus = false;
            NotificationsHelper _helper = new NotificationsHelper();
            blnUpdateStatus = _helper.openAndSeenNotificationsUpdateStatusById(id, section, formType); //RDBJ 10/21/2021 Added formType
            return blnUpdateStatus;
        }
        //End RDBJ 10/16/2021
        */

        // RDBJ 04/01/2022
        [HttpPost]
        public bool openAndSeenNotificationsUpdateStatusById(Dictionary<string, object> dicData)
        {
            bool blnUpdateStatus = false;
            NotificationsHelper _helper = new NotificationsHelper();
            blnUpdateStatus = _helper.openAndSeenNotificationsUpdateStatusById(dicData);    // JSL 06/25/2022 removed when clean all old Notification
            blnUpdateStatus = _helper.openAndSeenNotificationsUpdateStatusByDefId(dicData);    // JSL 06/25/2022 this is based on new notification
            return blnUpdateStatus;
        }
        // End RDBJ 04/01/2022

        // RDBJ 12/21/2021
        [HttpGet]
        public List<Notifications> LoadAssignedToMeGISIIADeficiencies(
            string UserID
            )
        {
            NotificationsHelper _helper = new NotificationsHelper();
            List<Notifications> list = _helper.LoadAssignedToMeGISIIADeficiencies(UserID);
            return list;
        }
        // RDBJ 12/21/2021

        #region Common Functions
        // JSL 04/30/2022
        [HttpPost]
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            NotificationsHelper _helper = new NotificationsHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End JSL 04/30/2022
        #endregion
    }
}
