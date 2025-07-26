using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    public class DraftsController : Controller
    {
        // GET: Drafts
        public ActionResult _GIR_DeficienciesSection(string id)
        {
            APIHelper _helper = new APIHelper();
            GeneralInspectionReport modal = new GeneralInspectionReport();
            modal = _helper.GIRFormGetDeficiency(id);
            return PartialView("GIR/_GIR_DeficienciesSection", modal);
        }

        // RDBJ 12/03/2021
        public ActionResult DeleteGISIIADrafts(string GISIFormID, string type)
        {
            APIHelper _helper = new APIHelper();
            _helper.DeleteGISIIADrafts(GISIFormID, type);
            return Json(new { ok = "OK" }, JsonRequestBehavior.AllowGet);
        }
        // End RDBJ 12/03/2021
    }
}