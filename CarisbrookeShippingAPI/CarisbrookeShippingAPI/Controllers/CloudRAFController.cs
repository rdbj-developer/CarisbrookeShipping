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
    // JSL 11/24/2022 added this controller
    [Authorize]
    public class CloudRAFController : ApiController
    {
        CloudRAFHelper _ObjCloudRAFHelper = new CloudRAFHelper();   // JSL 11/24/2022

        // JSL 11/24/2022
        [HttpPost]
        public APIResponse SubmitRiskAssessmentForm([FromBody] RiskAssessmentFormModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                _ObjCloudRAFHelper.SubmitRiskAssessmentFormData(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudRAF SubmitRiskAssessmentForm Inner Exception : " + ex.InnerException.ToString());
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        // End JSL 11/24/2022

        // JSL 11/26/2022
        [HttpGet]
        public List<BLL.Modals.RiskAssessmentFormModal> getRAFRemoteToLocal(
            string shipCode
            )
        {
            List<BLL.Modals.RiskAssessmentFormModal> response = new List<BLL.Modals.RiskAssessmentFormModal>();
            try
            {
                response = _ObjCloudRAFHelper.getUnsynchRAFList(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudRAFController : getRAFRemoteToLocal Exception : \n" + ex.Message);
                LogHelper.writelog("CloudRAFController : getRAFRemoteToLocal Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        [HttpPost]
        public bool sendSynchRAFListUFID(List<string> IdsStr)
        {
            bool response = false;
            try
            {
                response = _ObjCloudRAFHelper.sendSynchRAFListUFID(IdsStr);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudRAFController : sendSynchRAFListUFID Exception : \n" + ex.Message);
                LogHelper.writelog("CloudRAFController : sendSynchRAFListUFID Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        [HttpGet]
        public List<RiskAssessmentFormDetails> GetRAFFormsSyncedData(
            string shipCode
            )
        {
            List<RiskAssessmentFormDetails> response = new List<RiskAssessmentFormDetails>();
            try
            {
                response = _ObjCloudRAFHelper.getSynchRAFList(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudRAFController : GetRAFFormsSyncedData Exception : \n" + ex.Message);
                LogHelper.writelog("CloudRAFController : GetRAFFormsSyncedData Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        [HttpGet]
        public BLL.Modals.RiskAssessmentFormModal GetRAFSyncedData(string RAFUniqueID)
        {
            BLL.Modals.RiskAssessmentFormModal response = new BLL.Modals.RiskAssessmentFormModal();
            try
            {
                response = _ObjCloudRAFHelper.getSynchRAF(RAFUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudRAFController : GetRAFSyncedData Exception : \n" + ex.Message);
                LogHelper.writelog("CloudRAFController : GetRAFSyncedData Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End JSL 11/26/2022
    }
}
