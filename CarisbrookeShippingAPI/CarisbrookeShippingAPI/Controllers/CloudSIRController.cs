using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class CloudSIRController : ApiController
    {
        [HttpPost]
        public APIResponse sendSIRLocalToRemote([FromBody] BLL.Modals.SIRModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                CloudSIRHelper _helper = new CloudSIRHelper(); //RDBJ Use CloudSIRHelper rather then CloudGIRHelper
                bool response = _helper.SIRSynch(value);
                if (response)
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : sendSIRLocalToRemote Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : sendSIRLocalToRemote Inner Exception : \n" + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpGet] // RDBJ 12/22/2021
        public List<BLL.Modals.SIRModal> getSIRRemoteToLocal(
            string shipCode  // JSL 11/12/2022
            )
        {
            List<BLL.Modals.SIRModal> response = new List<BLL.Modals.SIRModal>();
            try
            {
                CloudSIRHelper _helper = new CloudSIRHelper();
                response = _helper.getUnsynchSIRList(shipCode); // JSL 11/12/2022 added shipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudSIRController : sendSIRRemoteToLocal Exception " + shipCode + " : \n" + ex.Message);
                LogHelper.writelog("CloudSIRController : sendSIRRemoteToLocal Inner Exception " + shipCode +  " : \n" + ex.InnerException);
            }
            return response;
        }


        //RDBJ 09/27/2021
        [HttpGet] // RDBJ 12/22/2021
        public List<BLL.Modals.SuperintendedInspectionReport> GetSIRFormsSyncedData(
            string shipCode  // JSL 11/12/2022
            )
        {
            List<BLL.Modals.SuperintendedInspectionReport> response = new List<BLL.Modals.SuperintendedInspectionReport>();
            try
            {
                CloudSIRHelper _helper = new CloudSIRHelper();
                response = _helper.getSynchSIRList(shipCode); // JSL 11/12/2022 added shipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudSIRController : GetSIRFormsSyncedData Exception : \n" + ex.Message);
                LogHelper.writelog("CloudSIRController : GetSIRFormsSyncedData Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        [HttpGet] // RDBJ 12/22/2021
        public BLL.Modals.SIRModal GetSIRSyncedData(string UniqueFormID)
        {
            BLL.Modals.SIRModal response = new BLL.Modals.SIRModal();
            try
            {
                CloudSIRHelper _helper = new CloudSIRHelper();
                response = _helper.getSynchSIR(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudSIRController : GetSIRFormsSyncedData Exception : \n" + ex.Message);
                LogHelper.writelog("CloudSIRController : GetSIRFormsSyncedData Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 09/27/2021

        //RDBJ 09/27/2021
        [HttpPost] // RDBJ 01/19/2022 set Post //[HttpGet]
        public bool sendSynchSIRListUFID(List<string> IdsStr) // RDBJ 01/19/2022 set List<string>
        {
            bool response = false;
            try
            {
                CloudSIRHelper _helper = new CloudSIRHelper();
                response = _helper.sendSynchSIRListUFID(IdsStr);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudSIRController : sendSynchSIRListUFID Exception : \n" + ex.Message);
                LogHelper.writelog("CloudSIRController : sendSynchSIRListUFID Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 09/27/2021

        // JSL 07/16/2022
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            CloudSIRHelper _helper = new CloudSIRHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End JSL 07/16/2022
    }
}
