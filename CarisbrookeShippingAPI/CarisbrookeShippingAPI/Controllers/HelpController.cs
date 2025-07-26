using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    // RDBJ 12/27/2021 Created
    public class HelpController : ApiController
    {
        // RDBJ 12/28/2021
        [HttpGet]
        public List<BLL.Modals.HelpAndSupport> GetHelpAndSupportsList()
        {
            List<BLL.Modals.HelpAndSupport> list = new List<BLL.Modals.HelpAndSupport>();
            try
            {
                HelpAndSupportHelper _helper = new HelpAndSupportHelper();
                list = _helper.GetHelpAndSupportsList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetHelpAndSupportsList Exception : " + ex.Message);
                LogHelper.writelog("GetHelpAndSupportsList Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        // End RDBJ 12/28/2021

        // RDBJ 12/28/2021
        [HttpPost]
        public APIResponse SubmitHelpAndSupport([FromBody] BLL.Modals.HelpAndSupport value)
        {
            APIResponse res = new APIResponse();
            try
            {
                HelpAndSupportHelper _helper = new HelpAndSupportHelper();
                bool response = _helper.InsertOrUpdateHelpAndSupport(value);
                if (response)
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitHelpAndSupport Exception : " + ex.Message);
                LogHelper.writelog("SubmitHelpAndSupport Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        // End RDBJ 12/28/2021

        // RDBJ 12/29/2021
        [HttpGet]
        public APIResponse DeleteHelpAndSupport(string ID, string ModifiedBy)
        {
            APIResponse res = new APIResponse();
            try
            {
                HelpAndSupportHelper _helper = new HelpAndSupportHelper();
                bool response = _helper.DeleteHelpAndSupport(ID, ModifiedBy);
                if (response)
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteHelpAndSupport Exception : " + ex.Message);
                LogHelper.writelog("DeleteHelpAndSupport Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        // End RDBJ 12/29/2021
    }
}
