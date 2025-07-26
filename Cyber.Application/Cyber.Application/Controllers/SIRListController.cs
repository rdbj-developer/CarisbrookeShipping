using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    public class SIRListController : Controller
    {
        // GET: SIRList
        public ActionResult Index()
        {
            APIHelper _helper = new APIHelper();
            List<SuperintendedInspectionReport> list = new List<SuperintendedInspectionReport>();
            list = _helper.GetAllSIRForms();
            ViewBag.Listdata = list.OrderByDescending(x => x.Date);
            return View();
        }
        [HttpGet]
        public ActionResult DetailsView(int id, bool isDefectSection = false,string no="")
        {
            APIHelper _helper = new APIHelper();
            SIRModal list = new SIRModal();
            list = _helper.SIRFormDetailsView(id);
            if(no!="")
                ViewBag.No = "section"+no.Split('.')[0];
            return View(list);
        }
    }
}