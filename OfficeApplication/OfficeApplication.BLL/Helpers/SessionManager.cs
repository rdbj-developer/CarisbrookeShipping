using System.Web;

namespace OfficeApplication.BLL.Helpers
{
    public class SessionManager
    {
        // JSL 06/24/2022
        public static string Email
        {
            get
            {
                // JSL 09/27/2022 wrapped in try..catch
                string strRetEmail = string.Empty;
                try
                {
                    if (HttpContext.Current.Session != null)
                        strRetEmail = (HttpContext.Current.Session["Email"] == null ? string.Empty : HttpContext.Current.Session["Email"].ToString());
                }
                catch (System.Exception ex)
                {
                    strRetEmail = string.Empty;
                }
                return strRetEmail;
                // End JSL 09/27/2022 wrapped in try..catch
            }
            set
            {
                HttpContext.Current.Session["Email"] = value;
            }
        }
        // End JSL 06/24/2022

        // JSL 09/26/2022
        public static string Password
        {
            get
            {
                // JSL 09/27/2022 wrapped in try..catch
                string strRetPassword = string.Empty;
                try
                {
                    if (HttpContext.Current.Session != null)
                        strRetPassword = (HttpContext.Current.Session["Password"] == null ? string.Empty : HttpContext.Current.Session["Password"].ToString());
                }
                catch (System.Exception ex)
                {
                    strRetPassword = string.Empty;
                }
                return strRetPassword;
                // End JSL 09/27/2022 wrapped in try..catch
            }
            set
            {
                HttpContext.Current.Session["Password"] = value;
            }
        }
        // End JSL 09/26/2022

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
                return (HttpContext.Current.Session["UserID"] == null ? 0 : Utility.ToInt(HttpContext.Current.Session["UserID"]));
            }
            set
            {
                HttpContext.Current.Session["UserID"] = value;
            }
        }
        public static string UserGUID
        {
            get
            {
                return (HttpContext.Current.Session["UserGUID"] == null ? string.Empty : Utility.ToString(HttpContext.Current.Session["UserGUID"]));
            }
            set
            {
                HttpContext.Current.Session["UserGUID"] = value;
            }
        }

        // JSL 12/19/2022
        public static string UserGroup
        {
            get
            {
                return (HttpContext.Current.Session["UserGroup"] == null ? string.Empty : Utility.ToString(HttpContext.Current.Session["UserGroup"]));
            }
            set
            {
                HttpContext.Current.Session["UserGroup"] = value;
            }
        }
        // End JSL 12/19/2022
    }
}
