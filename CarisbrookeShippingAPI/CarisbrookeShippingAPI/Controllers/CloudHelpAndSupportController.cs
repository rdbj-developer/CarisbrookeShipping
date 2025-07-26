using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    // RDBJ 01/01/2022 Added this class
    public class CloudHelpAndSupportController : ApiController
    {
        // RDBJ 01/01/2022
        [HttpPost]
        public APIResponse SendHelpAndSupportDataLocalToRemote([FromBody] BLL.Modals.HelpAndSupport value)
        {
            APIResponse res = new APIResponse();
            try
            {
                CloudHelpAndSupportHelper _helper = new CloudHelpAndSupportHelper();
                bool response = _helper.InsertOrUpdateHelpAndSupportSynch(value);
                if (response)
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudHelpAndSupportController : SendHelpAndSupportDataLocalToRemote Exception : \n" + ex.Message);
                LogHelper.writelog("CloudHelpAndSupportController : SendHelpAndSupportDataLocalToRemote Inner Exception : \n" + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        // End RDBJ 01/01/2022

        // RDBJ 01/01/2022
        [HttpGet]
        public List<BLL.Modals.HelpAndSupport> GetHelpAndSupportRemoteToLocal()
        {
            List<BLL.Modals.HelpAndSupport> response = new List<BLL.Modals.HelpAndSupport>();
            try
            {
                CloudHelpAndSupportHelper _helper = new CloudHelpAndSupportHelper();
                response = _helper.GetUnsynchHelpAndSupportList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudHelpAndSupportController : GetHelpAndSupportRemoteToLocal Exception : \n" + ex.Message);
                LogHelper.writelog("CloudHelpAndSupportController : GetHelpAndSupportRemoteToLocal Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End RDBJ 01/01/2022

        // RDBJ 01/01/2022
        [HttpGet]
        public bool UpdateCloudHelpAndSupportSynchStatus(string IdsStr)
        {
            bool response = false;
            try
            {
                CloudHelpAndSupportHelper _helper = new CloudHelpAndSupportHelper();
                response = _helper.UpdateCloudHelpAndSupportSynchStatus(IdsStr);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudHelpAndSupportController : UpdateCloudHelpAndSupportSynchStatus Exception : \n" + ex.Message);
                LogHelper.writelog("CloudHelpAndSupportController : UpdateCloudHelpAndSupportSynchStatus Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End RDBJ 01/01/2022
    }
}
