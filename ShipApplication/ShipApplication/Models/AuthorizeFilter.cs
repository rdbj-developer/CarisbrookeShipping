using ShipApplication.BLL.Helpers;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Models
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

        //RDBJ 10/23/2021
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check if session is supported  
            if (ctx.Session != null)
            {
                // check if a new session id was generated  
                if (SessionManager.UserID > 0 && !string.IsNullOrEmpty(SessionManager.Username))
                {
                    //
                }
                else
                {
                    //Check is Ajax request  
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.ClearContent();
                        filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
                    }
                    // check if a new session id was generated  
                    else
                    {
                        filterContext.Result = new RedirectResult("~/Account/Login");
                    }
                }
            }
            base.HandleUnauthorizedRequest(filterContext);
        }
        //End RDBJ 10/23/2021
    }
}