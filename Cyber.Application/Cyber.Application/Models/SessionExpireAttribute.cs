using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Models
{
    public class SessionExpireAttribute: ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("~/Login/Login");
                return;
            }
            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    if (filterContext.HttpContext.Request.IsAjaxRequest())
            //    {
            //        filterContext.HttpContext.Response.StatusCode = 403;
            //        filterContext.HttpContext.Response.Write("Invalid session -- please login!");
            //        filterContext.HttpContext.Response.End();
            //    }
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}