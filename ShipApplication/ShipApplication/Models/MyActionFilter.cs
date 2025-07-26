using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Models
{
    public class MyActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                SettingsHelper _helper = new SettingsHelper();
                SimpleObject shipModal = _helper.GetShipJson();
                if (shipModal != null)
                {
                    SessionManager.ShipName = shipModal.name;
                    SessionManager.ShipCode = Utility.ToString(shipModal.id);
                }
            }
            catch (Exception ex) { }
        }
    }
}