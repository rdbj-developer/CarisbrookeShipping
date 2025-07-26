using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        APIHelper _helper = new APIHelper();
        string ShipCode = Convert.ToString(ConfigurationManager.AppSettings["OfficeShipCode"]);
        string ShipName = Convert.ToString(ConfigurationManager.AppSettings["OfficeShipName"]);
        public ActionResult Login()
        {
            if (Request.Cookies["UserName"] != null && Request.Cookies["Password"] != null)
            {
                ViewBag.UserName = Request.Cookies["UserName"].Value;
                ViewBag.Password = Request.Cookies["Password"].Value;
                SessionManager.Username = ViewBag.UserName;
                SessionManager.ShipCode = ShipCode;
                SessionManager.ShipName = ShipName;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserProfile user)
        {
            // JSL 09/26/2022
            Session["Email"] = user.Email;
            Session["Password"] = user.Password;
            // End JSL 09/26/2022

            UserProfile _user = new UserProfile();
            _user = _helper.Login(user);
            if (_user != null)
            {
                if (_user.Email == null)
                {
                    ViewBag.Error = "Enter valid Email and Password";
                    return View();
                }
                else
                {
                    // JSL 06/18/2022
                    var identity = (ClaimsIdentity)User.Identity;
                    identity.AddClaims(new[] {
                        new Claim("Email", _user.Email),
                        new Claim("UserRole", Convert.ToString(_user.UserRole)),
                        new Claim("UserID", Convert.ToString(_user.UserID)),
                        new Claim("ShipCode", ShipCode),
                        new Claim("ShipName", ShipName),
                        new Claim("UserGroup", Convert.ToString(_user.UserGroup.Value)),
                        new Claim("UserName", _user.UserName),
                    });
                    var claimName = ((ClaimsIdentity)User.Identity).FindFirst("UserID");
                    // End JSL 06/18/2022

                    // JSL 06/23/2022
                    HttpCookie LoggedUserInfo = new HttpCookie("LoggedUserInfo");
                    LoggedUserInfo["UserName"] = _user.Email;
                    // JSL 09/27/2022
                    LoggedUserInfo["Email"] = user.Email;
                    LoggedUserInfo["Password"] = user.Password;
                    // End JSL 09/27/2022
                    LoggedUserInfo["UserID"] = Convert.ToString(_user.UserID);
                    LoggedUserInfo["UserRole"] = Convert.ToString(_user.UserRole);
                    LoggedUserInfo["ShipCode"] = ShipCode;
                    LoggedUserInfo["ShipName"] = ShipName;
                    LoggedUserInfo["UserGroup"] = Convert.ToString(_user.UserGroup.Value);
                    //LoggedUserInfo.Expires.Add(new TimeSpan(0, 1, 0));
                    Response.Cookies.Add(LoggedUserInfo);
                    // End JSL 06/23/2022


                    Session["Email"] = _user.Email;
                    Session["Password"] = user.Password;    // JSL 09/26/2022
                    Session["UserRole"] = _user.UserRole;
                    Session["UserID"] = _user.UserID;
                    Session["UserGroup"] = _user.UserGroup.Value;
                    Session["Name"] = _user.UserName;

                    SessionManager.Username = _user.UserName;
                    SessionManager.Email = _user.Email; // JSL 06/24/2022
                    SessionManager.ShipCode = ShipCode;
                    SessionManager.ShipName = ShipName;
                    SessionManager.UserGUID = _user.UserID.ToString(); // JSL 05/01/2022
                    SessionManager.UserGroup = Convert.ToString(_user.UserGroup.Value); // JSL 12/19/2022

                    if (!_user.UserGroup.HasValue)
                        _user.UserGroup = 1;
                    
                    if (user.RememberMe)
                    {
                        Response.Cookies["UserName"].Expires = Utility.ToDateTimeUtcNow().AddDays(30); //RDBJ 10/27/2021 set utcTime
                        Response.Cookies["Password"].Expires = Utility.ToDateTimeUtcNow().AddDays(30); //RDBJ 10/27/2021 set utcTime
                        Response.Cookies["UserName"].Value = user.Email;
                        Response.Cookies["Password"].Value = user.Password;
                    }
                    else
                    {
                        Response.Cookies["UserName"].Expires = Utility.ToDateTimeUtcNow().AddDays(-1); //RDBJ 10/27/2021 set utcTime
                        Response.Cookies["Password"].Expires = Utility.ToDateTimeUtcNow().AddDays(-1); //RDBJ 10/27/2021 set utcTime
                    }
                    
                    var userGroupMenuPermissionList = _helper.GetUserGroupMenuPermission(_user.UserGroup.Value);
                    Session["UserGroupMenuPermission"] = userGroupMenuPermissionList;
                    if (userGroupMenuPermissionList != null && userGroupMenuPermissionList.Count > 0)
                    {
                        var parentMenus = userGroupMenuPermissionList.Where(x => x.ParentId == 0).OrderBy(x => x.DisplayOrder).ToList();
                        if (parentMenus != null)
                        {
                            var defaultMenu = parentMenus.Where(x => x.IsDefaultMenu == true).FirstOrDefault();
                            if (defaultMenu == null)
                                defaultMenu = parentMenus.FirstOrDefault();
                            return RedirectToAction(defaultMenu.ActionName, defaultMenu.ControllerName);
                        }
                    }                    
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["Email"] = null;
            Session.RemoveAll();
            return RedirectToAction("Login");
        }
    }
}