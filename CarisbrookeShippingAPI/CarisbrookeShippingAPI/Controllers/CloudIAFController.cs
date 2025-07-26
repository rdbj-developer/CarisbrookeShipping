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
    public class CloudIAFController : ApiController
    {
        #region 2 way syncing
        [HttpPost]
        public APIResponse sendIAFLocalToRemote([FromBody] BLL.Modals.IAF value)
        {
            APIResponse res = new APIResponse();
            try
            {
                CloudIAFHelper _helper = new CloudIAFHelper();
                bool response = _helper.IAFSynch(value);
                if (response)
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudIAFController : sendIAFLocalToRemote Exception : \n" + ex.Message);
                LogHelper.writelog("CloudIAFController : sendIAFLocalToRemote Inner Exception : \n" + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet] // RDBJ 12/22/2021
        public List<BLL.Modals.IAF> getIAFRemoteToLocal(
            string shipCode
            )
        {
            List<BLL.Modals.IAF> response = new List<BLL.Modals.IAF>();
            try
            {
                CloudIAFHelper _helper = new CloudIAFHelper();
                response = _helper.getUnsynchIAFList(shipCode);  // JSL 11/12/2022 added shipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudIAFController : sendIAFRemoteToLocal Full " + shipCode + " : \n" + ex.ToString());
                LogHelper.writelog("CloudIAFController : sendIAFRemoteToLocal Exception " + shipCode + " : \n" + ex.Message);
                LogHelper.writelog("CloudIAFController : sendIAFRemoteToLocal Inner Exception " + shipCode + " : \n" + ex.InnerException);
            }
            return response;
        }

        //RDBJ 10/05/2021
        [HttpGet] // RDBJ 12/22/2021
        public List<BLL.Modals.InternalAuditForm> GetIAFFormsSyncedData(
            string shipCode
            )
        {
            List<BLL.Modals.InternalAuditForm> response = new List<BLL.Modals.InternalAuditForm>();
            try
            {
                CloudIAFHelper _helper = new CloudIAFHelper();
                response = _helper.getSynchIAFList(shipCode);  // JSL 11/12/2022 added shipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudIAFController : GetIAFFormsSyncedData Exception : \n" + ex.Message);
                LogHelper.writelog("CloudIAFController : GetIAFFormsSyncedData Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 10/05/2021

        //RDBJ 10/05/2021
        [HttpGet] // RDBJ 12/22/2021
        public BLL.Modals.IAF GetIAFSyncedData(string UniqueFormID)
        {
            BLL.Modals.IAF response = new BLL.Modals.IAF();
            try
            {
                CloudIAFHelper _helper = new CloudIAFHelper();
                response = _helper.getSynchIAF(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudIAFController : GetIAFSyncedData Full " + UniqueFormID + " : \n" + ex.ToString());
                LogHelper.writelog("CloudIAFController : GetIAFSyncedData Exception " + UniqueFormID + " : \n" + ex.Message);
                LogHelper.writelog("CloudIAFController : GetIAFSyncedData Inner Exception " + UniqueFormID + " : \n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 10/05/2021

        [HttpPost] // RDBJ 01/19/2022 set with post //[HttpGet]
        public bool sendSynchIAFListUFID(List<string> IdsStr) // RDBJ 01/19/2022 set List<string>
        {
            bool response = false;
            try
            {
                CloudIAFHelper _helper = new CloudIAFHelper();
                response = _helper.sendSynchIAFListUFID(IdsStr);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudIAFController : sendSynchIAFListUFID Exception : \n" + ex.Message);
                LogHelper.writelog("CloudIAFController : sendSynchIAFListUFID Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        #endregion

        #region MLC, SRM, SSP References
        // JSL 05/20/2022
        [HttpPost]
        public List<Dictionary<string, object>> GetReferencesDataFromCloud(Dictionary<string, string> dictMetaData)
        {
            CloudIAFHelper _helper = new CloudIAFHelper();
            List<Dictionary<string, object>> response = new List<Dictionary<string, object>>();
            string strTableName = string.Empty;
            try
            {
                if (dictMetaData.ContainsKey("TableName"))
                    strTableName = dictMetaData["TableName"].ToString();

                if (!string.IsNullOrEmpty(strTableName))
                    response = _helper.GetReferencesDataList(strTableName);
                else
                    response = null;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudIAFController : GetReferencesDataFromCloud Exception : \n" + ex.Message);
                LogHelper.writelog("CloudIAFController : GetReferencesDataFromCloud Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End JSL 05/20/2022
        #endregion

        // JSL 07/16/2022
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            CloudIAFHelper _helper = new CloudIAFHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End JSL 07/16/2022
    }
}