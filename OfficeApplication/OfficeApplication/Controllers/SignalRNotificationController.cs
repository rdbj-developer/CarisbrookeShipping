using Newtonsoft.Json;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using OfficeApplication.Models.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OfficeApplication.Controllers
{
    // JSL 06/24/2022 added this file
    [SessionExpire]
    public class SignalRNotificationController : ApiController
    {
        // JSL 06/24/2022
        [HttpPost]
        public APIResponse SendNotification(Dictionary<string, string> dicMetadata)
        {
            APIResponse response = new APIResponse();
            NotificationsHub objNotifHub = new NotificationsHub();
            string strSendNotificationToUser = string.Empty;
            string strSendNotificationForUserID = string.Empty;   // JSL 06/30/2022
            string strSendNotificationFor = string.Empty;
            string strFormsNotificationsList = string.Empty;
            string strDeficienciesNotificationsList = string.Empty;
            
            try
            {
                if (dicMetadata != null && dicMetadata.Count > 0)
                {
                    if (dicMetadata.ContainsKey("SendNotificationToUser"))
                        strSendNotificationToUser = dicMetadata["SendNotificationToUser"];
                    
                    if (dicMetadata.ContainsKey("SendNotificationFor"))
                        strSendNotificationFor = dicMetadata["SendNotificationFor"];

                    // JSL 06/30/2022
                    if (dicMetadata.ContainsKey("SendNotificationToUserID"))
                        strSendNotificationForUserID = dicMetadata["SendNotificationToUserID"];
                    // End JSL 06/30/2022

                    switch (strSendNotificationFor)
                    {
                        case AppStatic.NotificationType:
                            {
                                if (dicMetadata.ContainsKey("FormsNotificationsList"))
                                {
                                    strFormsNotificationsList = dicMetadata["FormsNotificationsList"];
                                    objNotifHub.SendNotification(strSendNotificationToUser, strFormsNotificationsList, true);
                                }

                                if (dicMetadata.ContainsKey("DeficienciesNotificationsList"))
                                {
                                    strDeficienciesNotificationsList = dicMetadata["DeficienciesNotificationsList"];
                                    objNotifHub.SendNotification(strSendNotificationToUser, strDeficienciesNotificationsList);
                                    objNotifHub.PushNotificationToUsers(strSendNotificationToUser, strSendNotificationForUserID);   // JSL 06/30/2022
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SignalR SendNotification : " + ex.Message);
            }

            return response;
        }
        // End JSL 06/24/2022

        // JSL 06/24/2022
        [HttpGet]
        public APIResponse Get()
        {
            APIResponse response = new APIResponse();
            return response;
        }
        // End JSL 06/24/2022
    }
}
