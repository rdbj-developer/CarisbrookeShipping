using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class VIMSHelper
    {
        public VIMSLIST GetAllVIMS(string id)
        {
            VIMSLIST list = new VIMSLIST();

            using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
            {
                list.VIMS = dbContext.Database.SqlQuery<VIMS>("spGetVIMSList @PONO, @result", new SqlParameter("PONO", id), new SqlParameter("result", "VIMS")).ToList();
                list.AccountList = dbContext.Database.SqlQuery<AccountList>("spGetVIMSList @PONO, @result", new SqlParameter("PONO", id), new SqlParameter("result", "Account")).ToList();
            }
            return list;
        }
        public bool UpdateVIMSDate(float INVOICE_EXCHRATE,int REQNINVOICEID,DateTime INVOICE_DATE,float OldINVOICE_EXCHRATE,DateTime OldINVOICE_DATE,string user)
        {
            try
            {
                List<VIMS> list = new List<VIMS>();

                using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
                {
                    list = dbContext.Database.SqlQuery<VIMS>("spUpdateVIMS @INVOICE_EXCHRATE, @INVOICE_DATE, @REQNINVOICEID,@VRID,@ACCOUNTID,@TYPE, @PODATE", 
                    new SqlParameter("INVOICE_EXCHRATE", INVOICE_EXCHRATE),
                    new SqlParameter("INVOICE_DATE",INVOICE_DATE),
                    new SqlParameter("VRID", ""),
                    new SqlParameter("ACCOUNTID", ""),
                    new SqlParameter("TYPE", "VIMS"),
                    new SqlParameter("PODATE",""),
                    new SqlParameter("REQNINVOICEID", REQNINVOICEID)).ToList();
                }
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    VIMSLogInfomation obj = new VIMSLogInfomation();
                    obj.ModifyBy = new Guid(user);
                    obj.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    obj.OldINVOICE_DATE = OldINVOICE_DATE;
                    obj.OldINVOICE_EXCHRATE = OldINVOICE_EXCHRATE;
                    obj.NewINVOICE_DATE = INVOICE_DATE;
                    obj.NewINVOICE_EXCHRATE = INVOICE_EXCHRATE;
                    obj.VIMSID = Guid.NewGuid();
                    obj.REQNINVOICEID = REQNINVOICEID;
                    dbContext.VIMSLogInfomations.Add(obj);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
           
        }
        
        public bool UpdateVIMSAccountData(string PONO, string OldAccountCode, string NewAccountCode, string user,DateTime PODATE,DateTime OLDPODATE)
        {
            try
            {
                List<VIMS> list = new List<VIMS>();

                using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
                {
                   
                    list = dbContext.Database.SqlQuery<VIMS>("spUpdateVIMS @INVOICE_EXCHRATE, @INVOICE_DATE, @REQNINVOICEID,@VRID,@ACCOUNTID,@TYPE,@PODATE",
                       new SqlParameter("INVOICE_EXCHRATE", ""),
                       new SqlParameter("INVOICE_DATE", ""),
                         new SqlParameter("VRID", PONO),
                         new SqlParameter("ACCOUNTID", NewAccountCode),
                         new SqlParameter("TYPE", "Account"),
                            new SqlParameter("PODATE", PODATE),
                        new SqlParameter("REQNINVOICEID", "")).ToList();
                }
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    VIMSLogInfomation obj = new VIMSLogInfomation();
                    obj.ModifyBy = new Guid(user);
                    obj.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    obj.NewAccountID = NewAccountCode;
                    obj.OLDAccountID = OldAccountCode;
                    obj.VRID = Utility.ToInteger(PONO);
                    obj.VIMSID = Guid.NewGuid();
                    obj.NewPoDate = PODATE;
                    obj.OldPoDate = OLDPODATE;
                    dbContext.VIMSLogInfomations.Add(obj);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public List<SM_ACCOUNTCODE> GetAccountDetails()
        {
            try
            {
                List<SM_ACCOUNTCODE> list = new List<SM_ACCOUNTCODE>();

                using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
                {

                    list = dbContext.SM_ACCOUNTCODE.ToList();
                }
                return list;
            }
            catch (Exception ex)
            {

                return null;
            }

        }
    }
}


