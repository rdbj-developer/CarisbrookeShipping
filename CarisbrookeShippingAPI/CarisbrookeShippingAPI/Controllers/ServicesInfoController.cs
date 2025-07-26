using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class ServicesInfoController : ApiController
    {
        [HttpPost]
        public APIResponse AddGeneralSettings([FromBody]GeneralSettingsModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ServicesInfoHelper _helper = new ServicesInfoHelper();
                _helper.AddGeneralSettings(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGeneralSettings Exception : " + ex.Message);
                LogHelper.writelog("AddGeneralSettings Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddMainSyncServicesEventLog([FromBody]MainSyncServicesEventLogModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ServicesInfoHelper _helper = new ServicesInfoHelper();
                _helper.AddMainSyncServicesEventLog(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddMainSyncServicesEventLog Exception : " + ex.Message);
                LogHelper.writelog("AddMainSyncServicesEventLog Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddOpenFileServicesEventLog([FromBody]OpenFileServicesEventLogModal value)
        {     
            APIResponse res = new APIResponse();
            try
            {
                ServicesInfoHelper _helper = new ServicesInfoHelper();
                _helper.AddOpenFileServicesEventLog(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddOpenFileServicesEventLog Exception : " + ex.Message);
                LogHelper.writelog("AddOpenFileServicesEventLog Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
    }
}
