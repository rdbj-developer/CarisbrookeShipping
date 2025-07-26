using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class ReportsController : ApiController
    {
        [HttpGet]
        public List<SMV_BUDGET_OVERVIEW_1> Get_Reports_Data()
        {
            List<SMV_BUDGET_OVERVIEW_1> Invoice = new List<SMV_BUDGET_OVERVIEW_1>();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                Invoice = _helper.Get_Reports_Data();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Get_Reports_Data Exception : " + ex.Message);
                LogHelper.writelog("Get_Reports_Data Inner Exception : " + ex.InnerException);
            }
            return Invoice;
        }
        [HttpGet]
        public List<Invoice> Get_Reports_DataInvoice()
        {
            List<Invoice> Invoice = new List<Invoice>();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                Invoice = _helper.Get_Reports_DataInvoice();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Get_Reports_DataInvoice Exception : " + ex.Message);
                LogHelper.writelog("Get_Reports_DataInvoice Inner Exception : " + ex.InnerException);
            }
            return Invoice;
        }
        [HttpGet]
        public List<PurchaseOrder> Get_Reports_DataPurchase()
        {
            List<PurchaseOrder> PurchaseOrder = new List<PurchaseOrder>();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                PurchaseOrder = _helper.Get_Reports_DataPurchase();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Get_Reports_DataPurchase Exception : " + ex.Message);
                LogHelper.writelog("Get_Reports_DataPurchase Inner Exception : " + ex.InnerException);
            }
            return PurchaseOrder;
        }
        [HttpGet]
        public List<SMV_ACCOUNT_RECONCILATION_RPT> Get_Reports_DataList()
        {
            List<SMV_ACCOUNT_RECONCILATION_RPT> Invoice = new List<SMV_ACCOUNT_RECONCILATION_RPT>();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                Invoice = _helper.Get_Reports_DataList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Get_Reports_DataList Exception : " + ex.Message);
                LogHelper.writelog("Get_Reports_DataList Inner Exception : " + ex.InnerException);
            }
            return Invoice;
        }
        [HttpGet]
        public UALOpexFormula Get_Reports_UALOpexFormula(string startdate, string enddate)
        {
            UALOpexFormula list = new UALOpexFormula();
            try
            {
             
                DateTime stDate =
                    DateTime.ParseExact(startdate, "ddMMyyyy", CultureInfo.InvariantCulture);
                DateTime edDate =
                   DateTime.ParseExact(enddate, "ddMMyyyy", CultureInfo.InvariantCulture);
                ReportsHelper _helper = new ReportsHelper();
                list = _helper.Get_Reports_UALOpexFormula(stDate, edDate);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Get_Reports_DataList Exception : " + ex.Message);
                LogHelper.writelog("Get_Reports_DataList Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public List<AccountCodeData> GetAccountCodeList()
        {
            List<AccountCodeData> codelist = new List<AccountCodeData>();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                codelist = _helper.GetAccountCodeList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAccountCodeList Exception : " + ex.Message);
                LogHelper.writelog("GetAccountCodeList Inner Exception : " + ex.InnerException);
            }
            return codelist;
        }
        [HttpPost]
        public OpexReportFinilizeDate UpdateOpexReportDate(OpexReportFinilizeDate data)
        {
            OpexReportFinilizeDate _data = new OpexReportFinilizeDate();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                _data = _helper.UpdateOpexReportDate(data);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateOpexReportDate Exception : " + ex.Message);
                LogHelper.writelog("UpdateOpexReportDate Inner Exception : " + ex.InnerException);
            }
            return _data;
        }
        [HttpGet]
        public DateTime? GetLastFinilizeReportDate()
        {
            DateTime? dt = new DateTime();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                dt = _helper.GetLastFinilizeReportDate();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLastFinilizeReportDate Exception : " + ex.Message);
                LogHelper.writelog("GetLastFinilizeReportDate Inner Exception : " + ex.InnerException);
            }
            return dt;
        }
        [HttpPost]
        public List<ShipReportsAnalysisModal> GetAllShipReportsData(ShipModal Modal)
        {
            List<ShipReportsAnalysisModal> data = new List<ShipReportsAnalysisModal>();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                data = _helper.GetAllShipReportsData(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Get_Reports_DataList Exception : " + ex.Message);
                LogHelper.writelog("Get_Reports_DataList Inner Exception : " + ex.InnerException);
            }
            return data;
        }
       
        [HttpGet]
        public List<FleetInspectionDueDatesModal> GetFleetInspectionDueDates()
        {
            List<FleetInspectionDueDatesModal> codelist = new List<FleetInspectionDueDatesModal>();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                codelist = _helper.GetFleetInspectionDueDates();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFleetInspectionDueDates Exception : " + ex.Message);
                LogHelper.writelog("GetFleetInspectionDueDates Inner Exception : " + ex.InnerException);
            }
            return codelist;
        }
        [HttpPost]
        public APIResponse UpdateFleetInspectionDueDates(FleetInspectionDueDatesModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                ReportsHelper _helper = new ReportsHelper();
                if (_helper.UpdateFleetInspectionDueDates(value))
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFleetInspectionDueDates Exception : " + ex.Message);
                LogHelper.writelog("UpdateFleetInspectionDueDates Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
    }
}
