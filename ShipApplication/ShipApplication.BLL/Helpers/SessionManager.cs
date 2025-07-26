using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ShipApplication.BLL.Helpers
{
    public class SessionManager
    {
        public static string ShipName
        {
            get
            {
                return (HttpContext.Current.Session["ShipName"] == null ? string.Empty : HttpContext.Current.Session["ShipName"].ToString());
            }
            set
            {
                HttpContext.Current.Session["ShipName"] = value;
            }
        }
        public static string ShipCode
        {
            get
            {
                return (HttpContext.Current.Session["ShipCode"] == null ? string.Empty : Utility.ToString(HttpContext.Current.Session["ShipCode"]));
            }
            set
            {
                HttpContext.Current.Session["ShipCode"] = value;
            }
        }
        public static UserModal UserSession
        {
            get
            {
                return (HttpContext.Current.Session["UserSession"] == null ? null : (UserModal)HttpContext.Current.Session["UserSession"]);
            }
            set
            {
                HttpContext.Current.Session["UserSession"] = value;
            }
        }
        public static string Username
        {
            get
            {
                return (HttpContext.Current.Session["Username"] == null ? string.Empty : Utility.ToString(HttpContext.Current.Session["Username"]));
            }
            set
            {
                HttpContext.Current.Session["Username"] = value;
            }
        }

        public static int UserID
        {
            get
            {
                return (HttpContext.Current.Session["UserID"] == null ? 0 : Utility.ToInteger(HttpContext.Current.Session["UserID"]));
            }
            set
            {
                HttpContext.Current.Session["UserID"] = value;
            }
        }
        public static string EmployeeID
        {
            get
            {
                return (HttpContext.Current.Session["EmployeeID"] == null ? string.Empty : Utility.ToString(HttpContext.Current.Session["EmployeeID"]));
            }
            set
            {
                HttpContext.Current.Session["EmployeeID"] = value;
            }
        }
        public static string UserFullName
        {
            get
            {
                return (HttpContext.Current.Session["UserFullName"] == null ? string.Empty : Utility.ToString(HttpContext.Current.Session["UserFullName"]));
            }
            set
            {
                HttpContext.Current.Session["UserFullName"] = value;
            }
        }
        public static string Rank
        {
            get
            {
                return (HttpContext.Current.Session["Rank"] == null ? string.Empty : Utility.ToString(HttpContext.Current.Session["Rank"]));
            }
            set
            {
                HttpContext.Current.Session["Rank"] = value;
            }
        }
    }
}
