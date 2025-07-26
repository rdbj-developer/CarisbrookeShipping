using ShipApplication.Models;
using System.Web.Mvc;

namespace ShipApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MyActionFilter());  
        }

    }
}
