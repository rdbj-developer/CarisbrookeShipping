using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class IMOSController : ApiController
    {

        [HttpGet]
        public List<CSShipsModal> GetAllShips()
        {
            List<CSShipsModal> ShipsList = new List<CSShipsModal>();
            try
            {
                IMOSHelper _helper = new IMOSHelper();
                ShipsList = _helper.GetAllShips();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Exception : " + ex.Message);
                LogHelper.writelog("Inner Exception : " + ex.InnerException);
            }
            return ShipsList;
        }

        [HttpGet]
        public BLL.Modals.CSShipsModal GIRGetGeneralDescription(string shipCode) //RDBJ 10/07/21021 Change CSShipsModal from GeneralInspectionReport
        {
            BLL.Modals.CSShipsModal list = new BLL.Modals.CSShipsModal(); //RDBJ 10/07/21021 Change CSShipsModal from GeneralInspectionReport
            try
            {
                IMOSHelper _helper = new IMOSHelper();
                list = _helper.GetGeneralSettingDataByCode(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRGetGeneralDescription Exception : " + ex.Message);
                LogHelper.writelog("GIRGetGeneralDescription Inner Exception : " + ex.InnerException);
            }
            return list;
        }
    }
}
