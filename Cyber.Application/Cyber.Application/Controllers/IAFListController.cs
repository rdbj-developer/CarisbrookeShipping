using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    public class IAFListController : Controller
    {
        // GET: IAFList
        public ActionResult Index()
        {
            APIHelper _helper = new APIHelper();
            List<InternalAuditForm> list = new List<InternalAuditForm>();
            list = _helper.GetAllInternalAuditForm();
            ViewBag.Listdata = list.OrderByDescending(x => x.Date);
            return View();
        }
        public ActionResult DetailsView(string id)
        {
            APIHelper _helper = new APIHelper();
            IAF list = new IAF();
            list = _helper.IAFormDetailsView(Convert.ToInt32( id));
            Dictionary<string, string> response = new Dictionary<string, string>();
            response = _helper.GetNumberForNotes(list.InternalAuditForm.ShipName);
            List<String> data = new List<string>();
            foreach (var item in response)
            {
                data.Add( item.Value);
            }
            ViewBag.Numbers = data;
            return View(list);
        }
        public ActionResult Download(string file, string name)
        {
            APIHelper _helper = new APIHelper();
            
            string filepath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString()  + file;
            LogHelper.writelog("---------" + filepath + "----------");
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, name);
        }
       
    }
}