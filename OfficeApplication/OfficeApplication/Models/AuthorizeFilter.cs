using OfficeApplication.BLL.Helpers;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Models
{
    public class AuthorizeFilter : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (SessionManager.UserID > 0 && !string.IsNullOrEmpty(SessionManager.Username))
                return true;
            else
                return false;
        }
    }
}