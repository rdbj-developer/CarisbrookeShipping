using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ShipApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                string culture = "en-GB";
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);             
            }
            catch (Exception)
            {
            }
        }
        
        //RDBJ 10/23/2021
        protected void Application_EndRequest()
        {
            if (Context.Items["AjaxPermissionDenied"] is bool)
            {
                Context.Response.StatusCode = 401;
                Context.Response.End();
            }
        }
        //RDBJ 10/23/2021
    }
}
