using OfficeApplication.Models;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }
    }
}