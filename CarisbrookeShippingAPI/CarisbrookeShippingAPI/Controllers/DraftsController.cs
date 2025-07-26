using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class DraftsController : ApiController
    {
        [HttpGet]
        public List<GIRData> GetGIRDrafts(string id)
        {
            GIRHelper _helper = new GIRHelper();
            List<GIRData> DraftsList = _helper.GetGIRDrafts(id);
            return DraftsList;
        }
        [HttpGet]
        public List<SIRData> GetSIRDrafts(string id)
        {
            SIRHelper _helper = new SIRHelper();
            List<SIRData> DraftsList = _helper.GetSIRDrafts(id);
            return DraftsList;
        }

        // RDBJ 01/23/2022
        [HttpGet]
        public List<AuditList> GetIARDrafts(string id)
        {
            IAFHelper _helper = new IAFHelper();
            List<AuditList> DraftsList = _helper.GetIARDrafts(id);
            return DraftsList;
        }
        // End RDBJ 01/23/2022

        // RDBJ 12/03/2021
        [HttpGet]
        public bool DeleteGISIIADrafts(string GISIFormID, string type)
        {
            bool response = false;
            try
            {
                FormsHelper _helper = new FormsHelper();
                response = _helper.DeleteGISIIADrafts(GISIFormID, type);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteGISIIADrafts Exception : " + ex.Message);
                LogHelper.writelog("DeleteGISIIADrafts Inner Exception : " + ex.InnerException);
                response = false;
            }
            return response;
        }
        // End RDBJ 12/03/2021
    }
}
