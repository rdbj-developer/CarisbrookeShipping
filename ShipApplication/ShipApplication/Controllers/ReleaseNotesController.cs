using ShipApplication.BLL.Modals;
using ShipApplication.BLL.Helpers;
using ShipApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    //RDBJ 10/27/2021 Added Notifications Controller
    public class ReleaseNotesController : Controller
    {
        [AuthorizeFilter] //RDBJ 10/27/2021
        public ActionResult Index()
        {
            ReleaseNotesHelper objReleaseNotesHelper = new ReleaseNotesHelper(); // RDBJ 12/08/2021
            ViewBag.appReleaseList =  objReleaseNotesHelper.GetReleaseNotes(); // RDBJ 12/08/2021
            return View();
        }
    }
}