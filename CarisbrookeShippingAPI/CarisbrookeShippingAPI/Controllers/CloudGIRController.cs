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
    public class CloudGIRController : ApiController
    {
        // GET: CloudGIR
        [HttpPost]
        public APIResponse sendGIRLocalToRemote([FromBody] BLL.Modals.GeneralInspectionReport value)
        {
            APIResponse res = new APIResponse();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                res = _helper.GIRSynch(value);  // JSL 09/10/2022
                // JSL 09/10/2022 commented
                /*
                bool response = _helper.GIRSynch(value);
                if (res.result)
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
                */
                // End JSL 09/10/2022 commented
            }
            catch (Exception ex)
            {
                LogHelper.writelog("\nCloudGIRController : GIR Form : " + value.Ship + " - " + Convert.ToString(value.UniqueFormID) + "");
                LogHelper.writelog("\nCloudGIRController : sendGIRLocalToRemote Exception : \n" + ex.Message);
                LogHelper.writelog("\nCloudGIRController : sendGIRLocalToRemote Inner Exception : \n" + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpGet] // RDBJ 12/22/2021
        public List<BLL.Modals.GeneralInspectionReport> getGIRRemoteToLocal(
            string shipCode  // JSL 11/12/2022
            )
        {
            List<BLL.Modals.GeneralInspectionReport> response = new List<BLL.Modals.GeneralInspectionReport>();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.getUnsynchGIRList(shipCode);  // JSL 11/12/2022 added shipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : sendGIRRemoteToLocal Full " + shipCode + " : \n" + ex.ToString());
                LogHelper.writelog("CloudGIRController : sendGIRRemoteToLocal Exception " + shipCode + " : \n" + ex.Message.ToString());
                LogHelper.writelog("CloudGIRController : sendGIRRemoteToLocal Inner Exception " + shipCode + " : \n" + ex.InnerException.Message.ToString());
            }
            return response;
        }

        //RDBJ 09/22/2021
        [HttpGet] // RDBJ 12/22/2021
        public List<BLL.Modals.GeneralInspectionReport> GetGIRFormsSyncedData(
            string shipCode  // JSL 11/12/2022
            )
        {
            List<BLL.Modals.GeneralInspectionReport> response = new List<BLL.Modals.GeneralInspectionReport>();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.getSynchGIRList(shipCode);   // JSL 11/12/2022 added shipCode
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : GetGIRFormsSyncedData Exception " + shipCode + " : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : GetGIRFormsSyncedData Inner Exception " + shipCode + " : \n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 09/22/2021

        //RDBJ 09/22/2021
        [HttpGet] // RDBJ 12/22/2021
        public BLL.Modals.GeneralInspectionReport GetGIRSyncedData(string UniqueFormID)
        {
            BLL.Modals.GeneralInspectionReport response = new BLL.Modals.GeneralInspectionReport();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.getSynchGIR(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : GetGIRSyncedData Full " + UniqueFormID + " : \n" + ex.ToString());
                LogHelper.writelog("CloudGIRController : GetGIRSyncedData Exception " + UniqueFormID + " : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : GetGIRSyncedData Inner Exception " + UniqueFormID + " : \n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 09/22/2021

        #region // RDBJ 12/22/2021 later use
        // RDBJ 12/22/2021
        [HttpGet] 
        public List<BLL.Modals.GIRDeficiencies> GetGIRDeficienciesFromCloud(string UniqueFormID)
        {
            List<BLL.Modals.GIRDeficiencies> response = new List<BLL.Modals.GIRDeficiencies>();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.GetGIRDeficiencies(UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesFromCloud Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesFromCloud Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        [HttpGet]
        public List<BLL.Modals.DeficienciesNote> GetGIRDeficienciesCommentsFromCloud(string DeficienciesUniqueID)
        {
            List<BLL.Modals.DeficienciesNote> response = new List<BLL.Modals.DeficienciesNote>();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.GetGIRDeficienciesComments(DeficienciesUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesCommentsFromCloud Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesCommentsFromCloud Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        [HttpGet]
        public List<BLL.Modals.GIRDeficienciesInitialActions> GetGIRDeficienciesInitialActionsFromCloud(string DeficienciesUniqueID)
        {
            List<BLL.Modals.GIRDeficienciesInitialActions> response = new List<BLL.Modals.GIRDeficienciesInitialActions>();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.GetGIRDeficienciesInitialActions(DeficienciesUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesInitialActionsFromCloud Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesInitialActionsFromCloud Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        [HttpGet]
        public List<BLL.Modals.GIRDeficienciesResolution> GetGIRDeficienciesResolutionsFromCloud(string DeficienciesUniqueID)
        {
            List<BLL.Modals.GIRDeficienciesResolution> response = new List<BLL.Modals.GIRDeficienciesResolution>();
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.GetGIRDeficienciesResolutions(DeficienciesUniqueID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesResolutionsFromCloud Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : GetGIRDeficienciesResolutionsFromCloud Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        // End RDBJ 12/22/2021
        #endregion

        [HttpPost] // RDBJ 01/19/2022 set with post //[HttpGet]
        public bool sendSynchGIRListUFID(List<string> IdsStr) // RDBJ 01/19/2022 set List<string>
        {
            bool response = false;
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.sendSynchGIRListUFID(IdsStr);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : sendSynchGIRListUFID Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : sendSynchGIRListUFID Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }

        #region SIR
        [HttpGet]
        public bool sendSynchSIRListUFID(string IdsStr)
        {
            bool response = false;
            try
            {
                CloudGIRHelper _helper = new CloudGIRHelper();
                response = _helper.sendSynchSIRListUFID(IdsStr);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : sendSynchSIRListUFID Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : sendSynchSIRListUFID Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        public List<SIRModal> GetSIRFormsUnsyncedDataFromCloud()
        {
            List<SIRModal> response = new List<SIRModal>();
            try
            {
                CloudSIRHelper _helper = new CloudSIRHelper();
                response = _helper.GetSIRFormsUnsyncedDataFromCloud();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloudGIRController : GetSIRFormsUnsyncedDataFromCloud Exception : \n" + ex.Message);
                LogHelper.writelog("CloudGIRController : GetSIRFormsUnsyncedDataFromCloud Inner Exception : \n" + ex.InnerException);
            }
            return response;
        }
        #endregion

        // JSL 07/16/2022
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            CloudGIRHelper _helper = new CloudGIRHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End JSL 07/16/2022
    }
}