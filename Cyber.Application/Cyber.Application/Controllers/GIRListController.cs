using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class GIRListController : Controller
    {
        // GET: GIRList
        [HttpGet]
        public ActionResult Index(int id,bool isDefectSection=false)
        {
            APIHelper _helper = new APIHelper();
            GeneralInspectionReport list = new GeneralInspectionReport();
            list = _helper.GIRFormDetailsView(id);
            return View(list);
        }
        public ActionResult ListForms()
        {
            APIHelper _helper = new APIHelper();
            List<GeneralInspectionReport> list = new List<GeneralInspectionReport>();
            list=_helper.GetAllGIRForms();
            ViewBag.Listdata = list.OrderByDescending(x=>x.Date);
            return View();
        }
        
    }
}