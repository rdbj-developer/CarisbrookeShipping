using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class VIMSController : ApiController
    {
        [HttpGet]
       
        public VIMSLIST GetAllVIMS(string id)
        {
            VIMSLIST VIMSLIST = new VIMSLIST();
            try
            {
                VIMSHelper _helper = new VIMSHelper();
                VIMSLIST = _helper.GetAllVIMS(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllVIMS Exception : " + ex.Message);
                LogHelper.writelog("GetAllVIMS Inner Exception : " + ex.InnerException);
            }
            return VIMSLIST;
        }
        [HttpGet]
        public bool UpdateVIMSDate(float INVOICE_EXCHRATE, int REQNINVOICEID, string INVOICE_DATE, float OldINVOICE_EXCHRATE, string OldINVOICE_DATE, string user)
        {
            try
            {
                DateTime new_invoice_Date = DateTime.ParseExact(INVOICE_DATE, "dd/MM/yyyy",
                                    System.Globalization.CultureInfo.InvariantCulture);
                DateTime New_OldINVOICE_DATE = DateTime.ParseExact(OldINVOICE_DATE, "dd/MM/yyyy",
                                          System.Globalization.CultureInfo.InvariantCulture);
                VIMSHelper _helper = new VIMSHelper();
                _helper.UpdateVIMSDate(INVOICE_EXCHRATE, REQNINVOICEID, new_invoice_Date, OldINVOICE_EXCHRATE, New_OldINVOICE_DATE, user);
                return true;
            }
            catch (Exception)
            {

                return false; ;
            }
           
        }
        [HttpGet]
        public bool UpdateVIMSAccountData(string PONO,string OldAccountCode,string NewAccountCode ,string user, string PODATE, string OLDPODATE)
        {
            try
            {
                DateTime myDatenew = DateTime.ParseExact(PODATE, "dd/MM/yyyy",
                                     System.Globalization.CultureInfo.InvariantCulture);
                DateTime myDateold = DateTime.ParseExact(OLDPODATE, "dd/MM/yyyy",
                                          System.Globalization.CultureInfo.InvariantCulture);
                VIMSHelper _helper = new VIMSHelper();
                _helper.UpdateVIMSAccountData( PONO,  OldAccountCode,  NewAccountCode,  user, myDatenew, myDateold);
                return true;
            }
            catch (Exception)
            {

                return false; ;
            }
        }
        [HttpGet]
        public List<SM_ACCOUNTCODE> GetAccountDetails()
        {
            try
            {
                VIMSHelper _helper = new VIMSHelper();
                List<SM_ACCOUNTCODE> list = new List<SM_ACCOUNTCODE>();
                list = _helper.GetAccountDetails();
                return list;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
    }
}
