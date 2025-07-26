using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class SystemInfoController : ApiController
    {
        [HttpPost]
        public APIResponse AddShipSystem([FromBody]ShipSystemModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.AddShipSystem(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipSystem Exception : " + ex.Message);
                LogHelper.writelog("AddShipSystem Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public ShipSystemModal GetShipSystemByPCId([FromBody]ShipSystemModal value)
        {
            ShipSystemModal res = new ShipSystemModal();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                res = _helper.GetShipSystemByPCId(value);                
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipSystemByPCId Exception : " + ex.Message);
                LogHelper.writelog("GetShipSystemByPCId Inner Exception : " + ex.InnerException);                
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddShipSystemInfo([FromBody]ShipSystemsInfoModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.AddShipSystemInfo(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipSystemInfo Exception : " + ex.Message);
                LogHelper.writelog("AddShipSystemInfo Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddShipSystemInfoBulk([FromBody]List<ShipSystemsInfoModal> value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.AddShipSystemInfoBulk(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipSystemInfoBulk Exception : " + ex.Message);
                LogHelper.writelog("AddShipSystemInfoBulk Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddShipSystemEventlog([FromBody]ShipSystemsEventLogModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.AddShipSystemEventlog(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipSystemEventlog Exception : " + ex.Message);
                LogHelper.writelog("AddShipSystemEventlog Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddShipSystemsService([FromBody]ShipSystemsServiceModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.AddShipSystemsService(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipSystemsService Exception : " + ex.Message);
                LogHelper.writelog("AddShipSystemsService Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddShipSystemsProcess([FromBody]ShipSystemsProcessModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.AddShipSystemsProcess(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipSystemsProcess Exception : " + ex.Message);
                LogHelper.writelog("AddShipSystemsProcess Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpPost]
        public APIResponse UploadShipSystemLogs([FromBody]ShipSystemLog value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.UploadShipSystemLogs(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UploadShipSystemLogs Exception : " + ex.Message);
                LogHelper.writelog("UploadShipSystemLogs Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse AddShipSystemsSoftware([FromBody] ShipSystemsSoftwareInfoModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                _helper.AddShipSystemsSoftware(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddShipSystemsSoftware Exception : " + ex.Message);
                LogHelper.writelog("AddShipSystemsSoftware Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public List<ShipSystemsSoftwareInfoModal> GetShipSystemsSoftwareAssets(string shipCode)
        {
            List<ShipSystemsSoftwareInfoModal> res = new List<ShipSystemsSoftwareInfoModal>();
            try
            {
                ShipSystemsInfoHelper _helper = new ShipSystemsInfoHelper();
                res = _helper.GetShipSystemsSoftwareAssets(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipSystemsSoftwareAssets Exception : " + ex.Message);
                LogHelper.writelog("GetShipSystemsSoftwareAssets Inner Exception : " + ex.InnerException);
            }
            return res;
        }
    }
}
