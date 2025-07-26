using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            //LocalDBHelper.CreateDBUsersJson();
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection frmColl)
        {
            ShipUserReq req = new ShipUserReq();
            req.UserName = Utility.ToString(frmColl["Username"]);
            req.Password = Utility.ToString(frmColl["Password"]);

            var identity = (ClaimsIdentity)User.Identity;   // JSL 06/20/2022

            if (Utility.ToString(req.UserName).ToLower() == "install")
            {
                if (req.Password == "install")
                {
                    SessionManager.UserID = 1;
                    SessionManager.Username = "install";
                    SessionManager.UserFullName = "install";
                    SessionManager.Rank = "";
                    CreateClaimIdentity("1", "install");
                    return RedirectToAction("Index", "Forms");
                }
                ViewBag.Message = "Error";
            }
            else
            {
                UserModal response = LocalDBHelper.LoginUser_LocalDB(req);
                if (response != null && response.UID > 0)
                {
                    SessionManager.EmployeeID = response.EmployeeID;
                    SessionManager.UserID = response.UID;
                    SessionManager.Username = response.UserName;
                    SessionManager.UserSession = response;
                    SessionManager.UserFullName = response.FirstName + " " + response.SurName;
                    SessionManager.Rank = response.Rank;
                    CreateClaimIdentity(response.UID.ToString(), response.UserName);
                    return RedirectToAction("Index", "Forms");
                }
                ViewBag.Message = "Error";
            }
            return View();
        }

        // JSL 06/20/2022
        public void CreateClaimIdentity(
            //string strEmail
            string strUserID, string strUserName)
        {
            /*
            var identity = (ClaimsIdentity)User.Identity;
            identity.AddClaims(new[] {
                        //new Claim("Email", strEmail),
                        new Claim("UserID", strUserID),
                        new Claim("UserName", strUserName),
                    });

            var Check = HttpContext.User.Identity;

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                        new Claim("UserID", strUserID),
                        new Claim("UserName", strUserName),
                    }, "Basic"));
            var isAuthenticated = principal.Identity.IsAuthenticated; // true
            */
            //var claimName = ((ClaimsIdentity)User.Identity).FindFirst("UserID");

            HttpCookie userInfo = new HttpCookie("userInfo");
            userInfo["UserName"] = strUserName;
            userInfo["UserID"] = strUserID;
            userInfo["ShipCode"] = SessionManager.ShipCode; // JSL 06/28/2022
            Response.Cookies.Add(userInfo);
        }
        // End JSL 06/20/2022

        public ActionResult LogOut()
        {
            SessionManager.UserID = 0;
            SessionManager.Username = string.Empty;
            SessionManager.UserSession = null;
            SessionManager.UserFullName = string.Empty;
            SessionManager.Rank = string.Empty;
            return RedirectToAction("Login", "Account");
        }
    }
}