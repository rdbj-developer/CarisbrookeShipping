using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OfficeApplication.Models.SignalR
{
    // JSL 06/23/2022 added this file
    [AuthorizeFilter]
    public class NotificationsHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserHubModels> Users =
       new ConcurrentDictionary<string, UserHubModels>(StringComparer.InvariantCultureIgnoreCase);

        // JSL 06/23/2022
        public void GetNotification()
        {
            try
            {
                //string loggedUser = Context.User.Identity.Name;

                var userInfoCookie = Context.RequestCookies["LoggedUserInfo"];
                Dictionary<string, string> dictQueryString = new Dictionary<string, string>();
                string loggedUser = string.Empty;
                string strUserID = string.Empty;

                if (userInfoCookie != null)
                {
                    dictQueryString = Assets.ParseQueryString(userInfoCookie.Value);
                }

                if (dictQueryString != null && dictQueryString.Count > 0)
                {
                    strUserID = dictQueryString["UserID"];  // JSL 06/25/2022
                    loggedUser = dictQueryString["UserName"];
                }

                // JSL 06/25/2022
                //Get TotalNotification  
                string strFormsNotificationData = string.Empty;
                string strDeficienciesNotificationData = string.Empty;
                Dictionary<string, string> dictNotificationData = LoadNotificationData(strUserID
                        , dictQueryString   // JSL 09/27/2022
                    );

                if (dictNotificationData.ContainsKey("CurrentUserNotifications"))
                    strFormsNotificationData = dictNotificationData["CurrentUserNotifications"];

                if (dictNotificationData.ContainsKey("DeficienciesNotificationsList"))
                    strDeficienciesNotificationData = dictNotificationData["DeficienciesNotificationsList"];
                // End JSL 06/25/2022

                //Send To  
                UserHubModels receiver;
                if (Users.TryGetValue(loggedUser, out receiver))
                {
                    foreach (var itemConnectionId in receiver.ConnectionIds)
                    {
                        //var cid = receiver.ConnectionIds.FirstOrDefault();
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
                        context.Clients.Client(itemConnectionId).broadcaastNotificationForForms(strFormsNotificationData);   // JSL 06/25/2022
                        context.Clients.Client(itemConnectionId).broadcaastNotificationForDeficiencies(strDeficienciesNotificationData);
                        //context.Clients.All.notify(totalNotif);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("NotificationsHub GetNotification : " + ex.Message);
            }
        }
        // End JSL 06/23/2022

        // JSL 06/23/2022        //Specific User Call  
        public void SendNotification(string SentTo, string strNotificationData
                , bool blnIsFormsNotification = false   // JSL 06/25/2022
            )
        {
            try
            {
                //Send To  
                UserHubModels receiver;
                if (Users.TryGetValue(SentTo, out receiver))
                {
                    foreach (var itemConnectionId in receiver.ConnectionIds)
                    {
                        //var cid = receiver.ConnectionIds.FirstOrDefault();
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
                        if (blnIsFormsNotification)
                            context.Clients.Client(itemConnectionId).broadcaastNotificationForForms(strNotificationData);
                        else
                            context.Clients.Client(itemConnectionId).broadcaastNotificationForDeficiencies(strNotificationData);
                        //context.Clients.All.notify(totalNotif);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("NotificationsHub LoadNotificationData : " + ex.Message);
            }
        }
        // End JSL 06/23/2022

        // JSL 06/30/2022
        public void PushNotificationToUsers(string SentTo, string strCurrentUserID)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                Dictionary<string, string> retDictMetadata = new Dictionary<string, string>(); 
                Dictionary<string, string> dictQueryString = new Dictionary<string, string>();
                string loggedUser = string.Empty;
                string strRecentDeficienciesNotificationsData = string.Empty;

                //Send To  
                UserHubModels receiver;
                if (Users.TryGetValue(SentTo, out receiver))
                {
                    if (receiver.InitDateTime != null)
                    {
                        //List<Dictionary<string, string>> lstDicDeficienciesNotification = new List<Dictionary<string, string>>();
                        //lstDicDeficienciesNotification = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(strNotificationData);

                        //lstDicDeficienciesNotification = lstDicDeficienciesNotification.Where(x => Convert.ToDateTime(x["CreatedDateTime"]) > receiver.InitDateTime)
                        //    .OrderByDescending(o => Convert.ToDateTime(o["CreatedDateTime"])).ToList();

                        //strNotificationData = JsonConvert.SerializeObject(lstDicDeficienciesNotification);

                        // JSL 06/30/2022
                        try
                        {
                            Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                            dictMetaData["CurrentUserId"] = strCurrentUserID;
                            dictMetaData["InitialDateTime"] = Convert.ToString(receiver.InitDateTime);
                            dictMetaData["strAction"] = AppStatic.API_GETRECENTDEFICIENCIESNOTIFICATION;

                            // JSL 01/08/2023
                            dictMetaData["Email"] = "@p!U$er";
                            dictMetaData["Password"] = "@p!Pa$$w0rd";
                            // End JSL 01/08/2023

                            retDictMetadata = _helper.PostAsyncAPICall(APIURLHelper.APINotifications, "CommonPostAPICall", dictMetaData);

                            if (retDictMetadata != null && retDictMetadata.Count > 0)
                            {
                                if (retDictMetadata.ContainsKey("RecentDeficienciesNotificationsList"))
                                {
                                    strRecentDeficienciesNotificationsData = Convert.ToString(retDictMetadata["RecentDeficienciesNotificationsList"]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog("NotificationsHub RecentDeficienciesNotificationsList PushNotificationToUsers : " + ex.Message);
                        }
                        // End JSL 06/30/2022
                    }

                    foreach (var itemConnectionId in receiver.ConnectionIds)
                    {
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
                        context.Clients.Client(itemConnectionId).broadcaastNotificationForToster(strRecentDeficienciesNotificationsData);
                    }

                    receiver.InitDateTime = Utility.ToDateTimeUtcNow();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("NotificationsHub PushNotificationToUsers : " + ex.Message);
            }
        }
        // End JSL 06/30/2022

        // JSL 06/25/2022
        private Dictionary<string, string> LoadNotificationData(string strUserID
            , Dictionary<string, string> dictQueryString    // JSL 09/27/2022
            )
        {
            APIHelper _helper = new APIHelper();
            Dictionary<string, string> retDictMetadata = new Dictionary<string, string>();
            try
            {
                Dictionary<string, string> dictMetaData = new Dictionary<string, string>();
                dictMetaData = dictQueryString; // JSL 09/27/2022
                dictMetaData["CurrentUserId"] = strUserID;
                dictMetaData["strAction"] = AppStatic.API_GETNOTIFICATION;
                retDictMetadata = _helper.PostAsyncAPICall(APIURLHelper.APINotifications, "CommonPostAPICall", dictMetaData);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("NotificationsHub LoadNotificationData : " + ex.Message);
            }
            return retDictMetadata;
        }
        // End JSL 06/25/2022

        // JSL 06/23/2022
        public override Task OnConnected()
        {
            //var str = ((ClaimsIdentity)Context.User.Identity).FindFirst("UserID");
            //ClaimsPrincipal principal = HttpContext.Current.User as ClaimsPrincipal;
            //string userName = Context.User.Identity.Name;

            var userInfoCookie = Context.RequestCookies["LoggedUserInfo"];
            Dictionary<string, string> dictQueryString = new Dictionary<string, string>();
            string userName = string.Empty;
            string connectionId = Context.ConnectionId;

            if (userInfoCookie != null)
            {
                dictQueryString = Assets.ParseQueryString(userInfoCookie.Value);
            }

            if (dictQueryString != null && dictQueryString.Count > 0)
            {
                userName = dictQueryString["UserName"];
            }

            if (!string.IsNullOrEmpty(userName))
            {
                var user = Users.GetOrAdd(userName, _ => new UserHubModels
                {
                    UserName = userName,
                    InitDateTime = Utility.ToDateTimeUtcNow(),
                    ConnectionIds = new HashSet<string>()
                });

                if (user != null)
                {
                    lock (user.ConnectionIds)
                    {
                        user.ConnectionIds.Add(connectionId);
                        if (user.ConnectionIds.Count == 0)
                        {
                            //Clients.Others.userConnected(userName);
                        }
                    }
                }
            }

            GetNotification();
            return base.OnConnected();
        }
        // End JSL 06/23/2022

        // JSL 06/23/2022
        public override Task OnDisconnected(bool stopCalled)
        {
            var userInfoCookie = Context.RequestCookies["LoggedUserInfo"];
            Dictionary<string, string> dictQueryString = new Dictionary<string, string>();
            string userName = string.Empty;
            string connectionId = Context.ConnectionId;

            if (userInfoCookie != null)
            {
                dictQueryString = Assets.ParseQueryString(userInfoCookie.Value);
            }

            if (dictQueryString != null && dictQueryString.Count > 0)
            {
                userName = dictQueryString["UserName"];
            }

            UserHubModels user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserHubModels removedUser;
                        Users.TryRemove(userName, out removedUser);
                        //Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
        // End JSL 06/23/2022
    }
}