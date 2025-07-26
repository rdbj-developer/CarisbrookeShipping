using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System;
using System.Configuration;
using System.Linq;
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
                    Session["Email"] = _user.Email;
                    Session["UserRole"] = _user.UserRole;
                    Session["UserID"] = _user.UserID;
                    SessionManager.ShipCode = ShipCode;
                    SessionManager.ShipName = ShipName;
                    if (!_user.UserGroup.HasValue)
                        _user.UserGroup = 1;
                    Session["UserGroup"] = _user.UserGroup.Value;
                    if (user.RememberMe)
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["UserName"].Value = user.Email;
                        Response.Cookies["Password"].Value = user.Password;
                    }
                    else
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                    }
                    Session["Name"] = _user.UserName;
                    SessionManager.Username = _user.UserName;
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