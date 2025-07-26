using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire] // RDBJ 01/15/2022
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
        public ActionResult DetailsView(string id) //RDBJ 09/24/2021 change int to string
        {
            APIHelper _helper = new APIHelper();
            SIRModal list = new SIRModal();
            List<CSShipsModal> shipsList = LoadShipsList();
            ViewBag.ShipDatas = shipsList;
            //ViewBag.ShipName = SessionManager.ShipName; // RDBJ 01/17/2022 commented this line
            list = _helper.SIRFormDetailsView(id);
            ViewBag.ShipCode = list.SuperintendedInspectionReport.ShipName; // RDBJ 01/17/2022

            // JSL 12/31/2022
            if (list.SuperintendedInspectionReport.ShipID == null || list.SuperintendedInspectionReport.ShipID == 0)
            {
                var shipDetailsForThisFormShip = shipsList.Where(x => x.Code == list.SuperintendedInspectionReport.ShipName).FirstOrDefault();
                if (shipDetailsForThisFormShip != null)
                {
                    list.SuperintendedInspectionReport.ShipID = shipDetailsForThisFormShip.ShipId;
                }
            }
            // End JSL 12/31/2022

            return View(list);
        }

        public List<CSShipsModal> LoadShipsList()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = _helper.GetAllShips();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();

            // JSL 01/05/2022
            if (SessionManager.UserGroup == "8") // 8 is used to avoid show some ships for Visitor Groups
            {
                shipsList = shipsList
                    .Where(x =>
                    x.Name.ToLower().StartsWith("j")
                    || x.Name.ToLower().StartsWith("c")
                    || x.Name.ToLower().StartsWith("vectis")
                    )
                    .ToList();
            }
            // End JSL 01/05/2022

            return shipsList;
        }
    }
}