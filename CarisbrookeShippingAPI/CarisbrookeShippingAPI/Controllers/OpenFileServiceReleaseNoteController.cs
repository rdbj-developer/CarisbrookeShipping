using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class OpenFileServiceReleaseNoteController : ApiController
    {
        [HttpPost]
        public APIResponse AddOpenFileServicesReleaseNote([FromBody] OpenFileServicesReleaseNote value)
        {
            APIResponse res = new APIResponse();
            try
            {
                OpenFileServicesReleaseNoteHelper _helper = new OpenFileServicesReleaseNoteHelper();
                _helper.AddOpenFileServicesReleaseNote(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddOpenFileServicesReleaseNote Exception : " + ex.Message);
                LogHelper.writelog("AddOpenFileServicesReleaseNote Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpPost]
        public APIResponse AddOpenFileServicesDownloadLog([FromBody] OpenFileServicesDownloadLog value)
        {
            APIResponse res = new APIResponse();
            try
            {
                OpenFileServicesReleaseNoteHelper _helper = new OpenFileServicesReleaseNoteHelper();
                _helper.AddOpenFileServicesDownloadLog(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddOpenFileServicesDownloadLog Exception : " + ex.Message);
                LogHelper.writelog("AddOpenFileServicesDownloadLog Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpGet]
        public OpenFileServicesReleaseNote GetLatestOpenFileServicesInfo()
        {
            OpenFileServicesReleaseNote res = new OpenFileServicesReleaseNote();
            try
            {
                OpenFileServicesReleaseNoteHelper _helper = new OpenFileServicesReleaseNoteHelper();
                res = _helper.GetLatestOpenFileServicesInfo();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLatestOpenFileServicesInfo Exception : " + ex.Message);
                LogHelper.writelog("GetLatestOpenFileServicesInfo Inner Exception : " + ex.InnerException);
            }
            return res;
        }
    }
}
