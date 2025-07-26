using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    //[Authorize] // JSL 09/26/2022
    public class ShipAppReleaseNoteController : ApiController
    {
        [HttpPost]
        public APIResponse AddShipAppReleaseNote([FromBody]ShipAppReleaseNote value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipAppReleaseNoteHelper _helper = new ShipAppReleaseNoteHelper();
                res = _helper.AddShipAppReleaseNote(value); // RDBJ 02/12/2022 set with actual response 
                //res.result = AppStatic.SUCCESS; // RDBJ 02/12/2022 commented this line
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipAppReleaseNote Exception : " + ex.Message);
                LogHelper.writelog("AddShipAppReleaseNote Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpPost]
        public APIResponse AddShipAppDownloadLog([FromBody]ShipAppDownloadLog value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipAppReleaseNoteHelper _helper = new ShipAppReleaseNoteHelper();
                _helper.AddShipAppDownloadLog(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipAppDownloadLog Exception : " + ex.Message);
                LogHelper.writelog("AddShipAppDownloadLog Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpGet]
        public ShipAppReleaseNote GetLatestShipAppInfo()
        {
            ShipAppReleaseNote res = new ShipAppReleaseNote();
            try
            {
                ShipAppReleaseNoteHelper _helper = new ShipAppReleaseNoteHelper();
                res = _helper.GetLatestShipAppInfo();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLatestShipAppInfo Exception : " + ex.Message);
                LogHelper.writelog("GetLatestShipAppInfo Inner Exception : " + ex.InnerException);
            }
            return res;
        }
    }
}
