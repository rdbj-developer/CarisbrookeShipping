using Newtonsoft.Json;
using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class ReportsController : Controller
    {
        OpexReportModalList Modal = new OpexReportModalList();
        List<MainCode> codeList = new List<MainCode>();
        List<SubCode> subcodelist = new List<SubCode>();
        List<DisplayModalList> displayList = new List<DisplayModalList>();
        List<DisplayModalList> AlldisplayList = new List<DisplayModalList>();
        List<AccountCodeData> ACCodeList = new List<AccountCodeData>();

        List<DisplayModalInvoice> invoiceList = new List<DisplayModalInvoice>();
        List<DisplayModalPurchaseOrder> orderList = new List<DisplayModalPurchaseOrder>();
        List<CodeInvoice> codelistforinvoice = new List<CodeInvoice>();
        List<SubCodeInvoice> SubCodeInvoice = new List<SubCodeInvoice>();
        List<CodePurchaseOrder> CodePurchaseOrder = new List<CodePurchaseOrder>();
        List<SubCodePurchaseOrder> SubCodePurchaseOrder = new List<SubCodePurchaseOrder>();

        UALOpexFormula UALOpexFormula = new UALOpexFormula();
        List<DisplayModalActualInvoices> ActualInvoicesList = new List<DisplayModalActualInvoices>();
        List<ActualInvoicesCode> ActualInvoicesCode = new List<ActualInvoicesCode>();
        List<ActualInvoicesSubCode> ActualInvoicesSubCode = new List<ActualInvoicesSubCode>();

        List<DisplayModalInvoicesReceivedCurrentMonth> InvoicesReceivedCurrentMonth = new List<DisplayModalInvoicesReceivedCurrentMonth>();
        List<InvoicesReceivedCurrentMonthCode> InvoicesReceivedCurrentMonthCode = new List<InvoicesReceivedCurrentMonthCode>();
        List<InvoicesReceivedCurrentMonthSubCode> InvoicesReceivedCurrentMonthSubCode = new List<InvoicesReceivedCurrentMonthSubCode>();

        List<DisplayModalPOsCurrentMonth> POsCurrentMonth = new List<DisplayModalPOsCurrentMonth>();
        List<POsCurrentMonthCode> POsCurrentMonthCode = new List<POsCurrentMonthCode>();
        List<POsCurrentMonthSubCode> POsCurrentMonthSubCode = new List<POsCurrentMonthSubCode>();

        List<DisplayModalOpenPOsPreviousMonths> OpenPOsPreviousMonths = new List<DisplayModalOpenPOsPreviousMonths>();
        List<OpenPOsPreviousMonthsCode> OpenPOsPreviousMonthsCode = new List<OpenPOsPreviousMonthsCode>();
        List<OpenPOsPreviousMonthsSubCode> OpenPOsPreviousMonthsSubCode = new List<OpenPOsPreviousMonthsSubCode>();

        List<DisplayModalPreviousMonthPObalance> PreviousMonthPObalance = new List<DisplayModalPreviousMonthPObalance>();
        List<PreviousMonthPObalanceCode> PreviousMonthPObalanceCode = new List<PreviousMonthPObalanceCode>();
        List<PreviousMonthPObalanceSubCode> PreviousMonthPObalanceSubCode = new List<PreviousMonthPObalanceSubCode>();

        List<FinalOpexCode> FinalOpexCode = new List<FinalOpexCode>();
        List<FinalOpexSubcode> FinalOpexSubcode = new List<FinalOpexSubcode>();
        List<Budget> Budget = new List<Budget>();
        [SessionExpire]
        [HttpPost]
        public ActionResult HierarchyBinding_Code(string month)
        {
            try
            {

                FinanceList(month);
                return Json(codeList.OrderBy(x => Convert.ToInt32(x.Code)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult HierarchyBinding_Details(string month)
        {
            try
            {
                FinanceList(month);
                subcodelist = displayList
                          .GroupBy(l => l.AccountCode)

                          .Select(cl => new SubCode
                          {
                              Code = cl.First().Code,
                              ACCOUNT_CODE = cl.First().AccountCode,
                              Total = cl.Sum(c => Convert.ToDouble(c.POTOTALBASECURRENCY)).ToString(),
                              ACCOUNT_DESCR = cl.First().AccountDescription,

                          }).ToList();

                return Json(subcodelist.OrderBy(x => Convert.ToInt32(x.ACCOUNT_CODE)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpPost]
        public ActionResult HierarchyBinding_DetailsList(string month)
        {
            try
            {
                FinanceList(month);
                int _month = FilterMonth(month);
                return Json(displayList.OrderBy(x => Convert.ToInt32(x.AccountCode)), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }
        [SessionExpire]
        public ActionResult Finance()
        {
            OpexReportModal Modal = new OpexReportModal();
            List<SMV_BUDGET_OVERVIEW_1> list = GetReportData();
            Modal.DispList = GetDisplayData(list, "Annual");
            Modal.PrintList = GetPrintData(list, "Annual");
            TempData["SelectedPeriod"] = "Annual";
            ViewBag.selected = "Annual";
            return View(Modal);

        }
        [HttpPost]
        [SessionExpire]
        public ActionResult Finance(FormCollection coll)
        {
            OpexReportModal Modal = new OpexReportModal();
            string selected = coll["hdnSelectedVal"];
            List<SMV_BUDGET_OVERVIEW_1> list = GetReportData();
            Modal.DispList = GetDisplayData(list, selected);
            Modal.PrintList = GetPrintData(list, selected);
            TempData["SelectedPeriod"] = selected;
            ViewBag.selected = selected;
            return View(Modal);
        }
        [SessionExpire]
        public ActionResult FinanceList(string monthname = "Annual", int year = 0)
        {
            DateTime target = new DateTime(2007, 1, 1);
            List<int> lstYears = new List<int>();
            while (target < DateTime.Today)
            {
                lstYears.Add(target.Year);
                target = target.AddYears(1);
            }
            ViewBag.yearList = lstYears.OrderByDescending(x=>x).ToList();
            TempData["SelectedPeriod"] = monthname;
            ViewBag.selected = monthname;
            if (year == 0)
                year = DateTime.Today.Year;
            ViewBag.year = year;
            int month = FilterMonth(monthname);
            ParentCodeForOpex(month, year);
            return View(Modal);
        }
        public List<SMV_BUDGET_OVERVIEW_1> GetReportData()
        {
            APIHelper _helper = new APIHelper();
            List<SMV_BUDGET_OVERVIEW_1> list = new List<SMV_BUDGET_OVERVIEW_1>();
            list = _helper.Get_Reports_Data();
            return list;
        }
        public List<SMV_ACCOUNT_RECONCILATION_RPT> GetReportDataList()
        {
            APIHelper _helper = new APIHelper();
            List<SMV_ACCOUNT_RECONCILATION_RPT> list = new List<SMV_ACCOUNT_RECONCILATION_RPT>();
            list = _helper.Get_Reports_DataList();
            ACCodeList = _helper.GetAccountCodeList();
            return list;
        }
        public List<Invoice> GetReportDataListInvoice()
        {
            APIHelper _helper = new APIHelper();
            List<Invoice> list = new List<Invoice>();
            list = _helper.Get_Reports_DataInvoice();
            ACCodeList = _helper.GetAccountCodeList();
            return list;
        }
        public List<PurchaseOrder> GetReportDataListPurchaseOrder()
        {
            APIHelper _helper = new APIHelper();
            List<PurchaseOrder> list = new List<PurchaseOrder>();
            list = _helper.Get_Reports_DataPurchase();
            ACCodeList = _helper.GetAccountCodeList();
            return list;
        }
        public UALOpexFormula GetReportDataListUALOpexFormula(DateTime startdate, DateTime enddate)
        {
            APIHelper _helper = new APIHelper();
            UALOpexFormula list = new UALOpexFormula();
            list = _helper.Get_Reports_UALOpexFormula(startdate, enddate);
            ACCodeList = _helper.GetAccountCodeList();
            return list;
        }
        public List<DisplayModal> GetDisplayData(List<SMV_BUDGET_OVERVIEW_1> list, string Period)
        {
            List<DisplayModal> DispList = new List<DisplayModal>();
            try
            {
                List<SMV_BUDGET_OVERVIEW_1> MainCatList = list.GroupBy(x => x.PARENT_CODE).Select(y => y.FirstOrDefault()).ToList();
                foreach (var item in MainCatList)
                {
                    DisplayModal MainDisp = new DisplayModal();
                    MainDisp.CODE = item.PARENT_CODE;
                    MainDisp.ACCOUNT_CODE = item.PARENT_DESCR;
                    MainDisp.ACCOUNT_DESCR = string.Empty;
                    MainDisp.TOTAL = string.Empty;
                    MainDisp.TOTALAMT = string.Empty;
                    List<SMV_BUDGET_OVERVIEW_1> SubCatList = list.Where(x => x.PARENT_CODE == item.PARENT_CODE).ToList();
                    double TotalAMTSum = GetTotalAMT(Period, SubCatList);
                    if (TotalAMTSum > 0)
                        MainDisp.TOTALAMT = TotalAMTSum.ToString();

                    DispList.Add(MainDisp);

                    // Sub Category
                    foreach (var subItem in SubCatList)
                    {
                        DisplayModal SubDisp = new DisplayModal();
                        SubDisp.CODE = string.Empty;
                        SubDisp.ACCOUNT_CODE = subItem.ACCOUNT_CODE;
                        SubDisp.ACCOUNT_DESCR = subItem.ACCOUNT_DESCR;
                        SubDisp.TOTAL = GetTotal(Period, subItem);
                        SubDisp.TOTALAMT = string.Empty;
                        DispList.Add(SubDisp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return DispList;
        }
        public List<DisplayModal> GetPrintData(List<SMV_BUDGET_OVERVIEW_1> list, string Period)
        {
            List<DisplayModal> DispList = new List<DisplayModal>();
            try
            {
                APIHelper _helper = new APIHelper();
                List<SMV_BUDGET_OVERVIEW_1> MainCatList = list.GroupBy(x => x.PARENT_CODE).Select(y => y.FirstOrDefault()).ToList();
                double Amount = 0;
                foreach (var item in MainCatList)
                {
                    DisplayModal MainDisp = new DisplayModal();
                    MainDisp.CODE = string.Empty;
                    MainDisp.ACCOUNT_CODE = string.Empty;
                    MainDisp.ACCOUNT_DESCR = item.PARENT_DESCR;
                    MainDisp.TOTAL = string.Empty;
                    MainDisp.TOTALAMT = string.Empty;
                    DispList.Add(MainDisp);

                    // Sub Category
                    List<SMV_BUDGET_OVERVIEW_1> SubCatList = list.Where(x => x.PARENT_CODE == item.PARENT_CODE).ToList();
                    double subAmount = 0;
                    foreach (var subItem in SubCatList)
                    {
                        DisplayModal SubDisp = new DisplayModal();
                        SubDisp.CODE = string.Empty;
                        SubDisp.ACCOUNT_CODE = string.Empty;
                        SubDisp.ACCOUNT_DESCR = subItem.ACCOUNT_DESCR;
                        SubDisp.TOTAL = GetTotal(Period, subItem);
                        SubDisp.TOTALAMT = string.Empty;
                        DispList.Add(SubDisp);
                        subAmount = subAmount + Utility.ToDouble(SubDisp.TOTAL);
                    }

                    double TotalAMTSum = GetTotalAMT(Period, SubCatList);
                    MainDisp = new DisplayModal();
                    MainDisp.CODE = "SubCatLast";
                    MainDisp.ACCOUNT_CODE = string.Empty;
                    MainDisp.ACCOUNT_DESCR = string.Empty;
                    MainDisp.TOTAL = TotalAMTSum.ToString();
                    MainDisp.TOTALAMT = string.Empty;
                    DispList.Add(MainDisp);

                    Amount = Amount + subAmount;
                }
                DisplayModal TotalDisp = new DisplayModal();
                TotalDisp.CODE = "Last";
                TotalDisp.ACCOUNT_CODE = string.Empty;
                TotalDisp.ACCOUNT_DESCR = "TOTAL";
                TotalDisp.TOTAL = Amount.ToString();
                TotalDisp.TOTALAMT = string.Empty;
                DispList.Add(TotalDisp);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return DispList;
        }

        public double GetTotalAMT(string Period, List<SMV_BUDGET_OVERVIEW_1> SubCatList)
        {
            double TotalSum = 0;
            foreach (var subItem in SubCatList)
            {
                switch (Period)
                {
                    case "Annual":
                        TotalSum = TotalSum + subItem.P01 + subItem.P02 + subItem.P03 + subItem.P04 + subItem.P05 + subItem.P06 + subItem.P07 + subItem.P08 + subItem.P09 + subItem.P10 + subItem.P11 + subItem.P12;
                        break;
                    case "January":
                        TotalSum = TotalSum + subItem.P01;
                        break;
                    case "February":
                        TotalSum = TotalSum + subItem.P02;
                        break;
                    case "March":
                        TotalSum = TotalSum + subItem.P03;
                        break;
                    case "April":
                        TotalSum = TotalSum + subItem.P04;
                        break;
                    case "May":
                        TotalSum = TotalSum + subItem.P05;
                        break;
                    case "June":
                        TotalSum = TotalSum + subItem.P06;
                        break;
                    case "July":
                        TotalSum = TotalSum + subItem.P07;
                        break;
                    case "August":
                        TotalSum = TotalSum + subItem.P08;
                        break;
                    case "September":
                        TotalSum = TotalSum + subItem.P09;
                        break;
                    case "October":
                        TotalSum = TotalSum + subItem.P10;
                        break;
                    case "November":
                        TotalSum = TotalSum + subItem.P11;
                        break;
                    case "December":
                        TotalSum = TotalSum + subItem.P12;
                        break;
                }
            }
            return TotalSum;
        }
        public string GetTotal(string Period, SMV_BUDGET_OVERVIEW_1 subItem)
        {
            double TotalSum = 0;
            switch (Period)
            {
                case "Annual":
                    TotalSum = subItem.P01 + subItem.P02 + subItem.P03 + subItem.P04 + subItem.P05 + subItem.P06 + subItem.P07 + subItem.P08 + subItem.P09 + subItem.P10 + subItem.P11 + subItem.P12;
                    break;
                case "January":
                    TotalSum = subItem.P01;
                    break;
                case "February":
                    TotalSum = subItem.P02;
                    break;
                case "March":
                    TotalSum = subItem.P03;
                    break;
                case "April":
                    TotalSum = subItem.P04;
                    break;
                case "May":
                    TotalSum = subItem.P05;
                    break;
                case "June":
                    TotalSum = subItem.P06;
                    break;
                case "July":
                    TotalSum = subItem.P07;
                    break;
                case "August":
                    TotalSum = subItem.P08;
                    break;
                case "September":
                    TotalSum = subItem.P09;
                    break;
                case "October":
                    TotalSum = subItem.P10;
                    break;
                case "November":
                    TotalSum = subItem.P11;
                    break;
                case "December":
                    TotalSum = subItem.P12;
                    break;
            }
            return TotalSum.ToString();
        }

        public List<DisplayModalList> GetDisplayDataList(List<SMV_ACCOUNT_RECONCILATION_RPT> list)
        {
            List<DisplayModalList> displayListNew = new List<DisplayModalList>();
            List<DisplayModalList> DispList = new List<DisplayModalList>();
            try
            {
                foreach (var subItem in list)
                {
                    DisplayModalList SubDisp = new DisplayModalList();
                    SubDisp.Code = subItem.AccountCode;
                    SubDisp.AccountCode = subItem.AccountCode;
                    SubDisp.AccountDescription = subItem.AccountDescription;
                    SubDisp.Total = subItem.POTOTAL.ToString();// GetTotalList(Period, subItem);
                    //SubDisp.TOTALAMT = string.Empty;
                    SubDisp.PONO = subItem.PONO;
                    SubDisp.POTITLE = subItem.POTITLE;
                    SubDisp.POCurrency = subItem.POCurrency;
                    SubDisp.POTOTAL = subItem.POTOTAL;
                    SubDisp.POSTATUS = subItem.POSTATUS;
                    SubDisp.POTOTALBASECURRENCY = subItem.POTOTALBASECURRENCY;
                    SubDisp.SupplierName = subItem.SupplierName;
                    SubDisp.InvoiceSent = subItem.InvoiceSent;
                    SubDisp.POEXCHRATE = subItem.POEXCHRATE;
                    SubDisp.INVOICENo = subItem.INVOICENo;

                    SubDisp.INVOICEAMOUNT = subItem.INVOICEAMOUNT;
                    SubDisp.NETINVOICEAMOUNT = subItem.NETINVOICEAMOUNT;
                    SubDisp.POSTATUS = subItem.POSTATUS;
                    SubDisp.INVOICEDATE = subItem.INVOICEDATE;
                    SubDisp.InvoiceReceivedDate = subItem.InvoiceReceivedDate;
                    SubDisp.PORECVDATE = subItem.PORECVDATE;
                    SubDisp.FORWARDER_RECVD_DATE = subItem.FORWARDER_RECVD_DATE;
                    SubDisp.PODATE = subItem.PODATE;
                    SubDisp.Code = subItem.AccountCode.Substring(0, 3);
                    SubDisp.INVOICEAMOUNTBASECURRENCY = subItem.INVOICEAMOUNTBASECURRENCY;
                    string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == subItem.AccountCode.Substring(0, 3)).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                    SubDisp.Description = desc;
                    DispList.Add(SubDisp);
                    int cnt = displayListNew.Where(x => x.PONO == subItem.PONO).ToList().Count();
                    if (cnt == 0)
                    {
                        displayListNew.Add(SubDisp);
                    }

                }
                AlldisplayList = DispList;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        public List<DisplayModalInvoice> GetDisplayDataListInvoice(List<Invoice> list)
        {
            List<DisplayModalInvoice> displayListNew = new List<DisplayModalInvoice>();
            List<DisplayModalInvoice> DispList = new List<DisplayModalInvoice>();
            try
            {
                foreach (var subItem in list)
                {
                    try
                    {
                        DisplayModalInvoice SubDisp = new DisplayModalInvoice();

                        SubDisp.AccountCode = subItem.AccountCode;
                        SubDisp.AccountDescription = subItem.AccountDescription;
                        SubDisp.Total = subItem.POTOTALUSD.ToString();
                        SubDisp.PONO = subItem.PONO;
                        SubDisp.INVOICE_AMOUNT = subItem.INVOICE_AMOUNT;
                        SubDisp.INVOICENO = subItem.INVOICENO;
                        SubDisp.SupplierName = subItem.SupplierName;
                        SubDisp.POCurrency = subItem.POCurrency;
                        SubDisp.POTOTALUSD = subItem.POTOTALUSD;
                        SubDisp.POSTATUS = subItem.POSTATUS;
                        SubDisp.POTotalPOCurr = subItem.POTotalPOCurr;

                        SubDisp.InvoiceSent = subItem.InvoiceSent;
                        SubDisp.INVOICEAMOUNTUSD = Math.Round(subItem.INVOICEAMOUNTUSD, 2);
                        SubDisp.INVOICEAMOUNTInvCurr = subItem.INVOICEAMOUNTInvCurr;

                        SubDisp.InvoiceDate = subItem.InvoiceDate;
                        SubDisp.NETINVOICEAMOUNT = subItem.NETINVOICEAMOUNT;

                        SubDisp.InvoiceReceivedDate = Utility.ConvertToDate(subItem.InvoiceReceivedDate);
                        SubDisp.InvoiceExchrate = subItem.InvoiceExchrate;
                        SubDisp.GoodsatForwarderRecvdDate = Utility.ConvertToDate(subItem.GoodsatForwarderRecvdDate);

                        if (subItem.AccountCode.Length > 3)
                            SubDisp.Code = subItem.AccountCode.Substring(0, 3);
                        else
                            SubDisp.Code = subItem.AccountCode;

                        SubDisp.PODATE = subItem.PODATE;
                        SubDisp.SumofInvoiceItems = subItem.SumofInvoiceItems;
                        string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == SubDisp.Code).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                        SubDisp.Description = desc;
                        DispList.Add(SubDisp);
                        int cnt = displayListNew.Where(x => x.PONO == subItem.PONO).ToList().Count();
                        if (cnt == 0)
                        {
                            displayListNew.Add(SubDisp);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("Error at Item :" + JsonConvert.SerializeObject(subItem) + " => " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return DispList;
        }
        public List<DisplayModalPurchaseOrder> GetDisplayDataListPurchaseOrder(List<PurchaseOrder> list)
        {
            List<DisplayModalPurchaseOrder> displayListNew = new List<DisplayModalPurchaseOrder>();
            List<DisplayModalPurchaseOrder> DispList = new List<DisplayModalPurchaseOrder>();
            try
            {
                foreach (var subItem in list)
                {
                    DisplayModalPurchaseOrder SubDisp = new DisplayModalPurchaseOrder();

                    SubDisp.AccountCode = subItem.AccountCode;
                    SubDisp.AccountDescription = subItem.AccountDescription;
                    SubDisp.Total = subItem.POTOTAL.ToString();
                    SubDisp.PONO = subItem.PONO;
                    SubDisp.PODATE = subItem.PODATE;
                    SubDisp.Code = subItem.AccountCode.Substring(0, 3);
                    SubDisp.POTOTALBASECURRENCY = subItem.POTOTALBASECURRENCY;
                    SubDisp.POTITLE = subItem.POTITLE;
                    SubDisp.POTOTAL = subItem.POTOTAL;
                    SubDisp.POEXCHRATE = subItem.POEXCHRATE;
                    SubDisp.POSTATUS = subItem.POSTATUS;
                    SubDisp.PORECVDATE = Utility.ConvertToDate(subItem.PORECVDATE);
                    SubDisp.FORWARDER_RECVD_DATE = Utility.ConvertToDate(subItem.FORWARDER_RECVD_DATE);
                    SubDisp.POCurrency = subItem.POCurrency;
                    string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == subItem.AccountCode.Substring(0, 3)).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                    SubDisp.Description = desc;
                    DispList.Add(SubDisp);
                    int cnt = displayListNew.Where(x => x.PONO == subItem.PONO).ToList().Count();
                    if (cnt == 0)
                    {
                        displayListNew.Add(SubDisp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        public List<DisplayModalActualInvoices> GetDisplayDataListUALOpex(UALOpexFormula list)
        {
            List<DisplayModalActualInvoices> displayListNew = new List<DisplayModalActualInvoices>();
            List<DisplayModalActualInvoices> DispList = new List<DisplayModalActualInvoices>();
            try
            {
                foreach (var subItem in list.ActualInvoices)
                {
                    DisplayModalActualInvoices SubDisp = new DisplayModalActualInvoices();

                    SubDisp.Account_Code = subItem.Account_Code;
                    SubDisp.Account_Description = subItem.Account_Description;
                    SubDisp.Total = subItem.PO_TOTALUSD.ToString();
                    SubDisp.PO_NO = subItem.PO_NO;
                    SubDisp.PO_DATE = Utility.ConvertToDate(subItem.PO_DATE);
                    SubDisp.Code = subItem.Account_Code.Substring(0, 3);
                    SubDisp.Supplier_Name = subItem.Supplier_Name;
                    SubDisp.PO_TOTALUSD = subItem.PO_TOTALUSD;
                    SubDisp.PO_STATUS = subItem.PO_STATUS;
                    SubDisp.INVOICENO = subItem.INVOICENO;
                    SubDisp.PO_Currency = subItem.PO_Currency;
                    SubDisp.Invoice_Date = Utility.ConvertToDate(subItem.Invoice_Date);
                    SubDisp.Invoice_Received_Date = Utility.ConvertToDate(subItem.Invoice_Received_Date);
                    SubDisp.Invoice_Exch_rate = subItem.Invoice_Exch_rate;
                    SubDisp.Invoice_Sent = subItem.Invoice_Sent;
                    SubDisp.INVOICE_AMOUNT = subItem.INVOICE_AMOUNT;
                    SubDisp.PO_Total_PO_Curr = subItem.PO_Total_PO_Curr;
                    SubDisp.INVOICE_AMOUNT_USD = subItem.INVOICE_AMOUNT_USD;
                    SubDisp.INVOICE_AMOUNT_Inv_Curr = subItem.INVOICE_AMOUNT_Inv_Curr;
                    SubDisp.NET_INVOICE_AMOUNT = subItem.NET_INVOICE_AMOUNT;
                    SubDisp.Invoice_Exch_rate = subItem.Invoice_Exch_rate;
                    SubDisp.Sum_of_Invoice_Items = subItem.Sum_of_Invoice_Items;
                    SubDisp.Goods_at_Forwarder_Recvd_Date = Utility.ConvertToDate(subItem.Goods_at_Forwarder_Recvd_Date);
                    string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == subItem.Account_Code.Substring(0, 3)).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                    SubDisp.Description = desc;
                    DispList.Add(SubDisp);
                    int cnt = displayListNew.Where(x => x.PO_NO == subItem.PO_NO).ToList().Count();
                    if (cnt == 0)
                    {
                        displayListNew.Add(SubDisp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        public List<DisplayModalInvoicesReceivedCurrentMonth> GetDisplayDataListUALOpexIRCurrentMonth(UALOpexFormula list)
        {
            List<DisplayModalInvoicesReceivedCurrentMonth> displayListNew = new List<DisplayModalInvoicesReceivedCurrentMonth>();
            List<DisplayModalInvoicesReceivedCurrentMonth> DispList = new List<DisplayModalInvoicesReceivedCurrentMonth>();
            try
            {
                foreach (var subItem in list.InvoicesReceivedCurrentMonth)
                {
                    DisplayModalInvoicesReceivedCurrentMonth SubDisp = new DisplayModalInvoicesReceivedCurrentMonth();

                    SubDisp.Account_Code = subItem.Account_Code;
                    SubDisp.Account_Description = subItem.Account_Description;
                    SubDisp.Total = subItem.PO_TOTALUSD.ToString();
                    SubDisp.PO_NO = subItem.PO_NO;
                    SubDisp.PO_DATE = Utility.ConvertToDate(subItem.PO_DATE);
                    SubDisp.Code = subItem.Account_Code.Substring(0, 3);
                    SubDisp.Supplier_Name = subItem.Supplier_Name;
                    SubDisp.PO_TOTALUSD = subItem.PO_TOTALUSD;
                    SubDisp.PO_STATUS = subItem.PO_STATUS;
                    SubDisp.INVOICENO = subItem.INVOICENO;
                    SubDisp.PO_Currency = subItem.PO_Currency;
                    SubDisp.Invoice_Date = Utility.ConvertToDate(subItem.Invoice_Date);
                    SubDisp.Invoice_Received_Date = Utility.ConvertToDate(subItem.Invoice_Received_Date);
                    SubDisp.Invoice_Exch_rate = subItem.Invoice_Exch_rate;
                    SubDisp.Invoice_Sent = subItem.Invoice_Sent;
                    SubDisp.INVOICE_AMOUNT = subItem.INVOICE_AMOUNT;
                    SubDisp.PO_Total_PO_Curr = subItem.PO_Total_PO_Curr;
                    SubDisp.INVOICE_AMOUNT_USD = subItem.INVOICE_AMOUNT_USD;
                    SubDisp.INVOICE_AMOUNT_Inv_Curr = subItem.INVOICE_AMOUNT_Inv_Curr;
                    SubDisp.NET_INVOICE_AMOUNT = subItem.NET_INVOICE_AMOUNT;
                    SubDisp.Invoice_Exch_rate = subItem.Invoice_Exch_rate;
                    SubDisp.Sum_of_Invoice_Items = subItem.Sum_of_Invoice_Items;
                    SubDisp.Goods_at_Forwarder_Recvd_Date = Utility.ConvertToDate(subItem.Goods_at_Forwarder_Recvd_Date);
                    string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == subItem.Account_Code.Substring(0, 3)).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                    SubDisp.Description = desc;
                    DispList.Add(SubDisp);
                    int cnt = displayListNew.Where(x => x.PO_NO == subItem.PO_NO).ToList().Count();
                    if (cnt == 0)
                    {
                        displayListNew.Add(SubDisp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        public List<DisplayModalPOsCurrentMonth> GetDisplayDataListUALOpexPOsCurrentMonth(UALOpexFormula list)
        {
            List<DisplayModalPOsCurrentMonth> displayListNew = new List<DisplayModalPOsCurrentMonth>();
            List<DisplayModalPOsCurrentMonth> DispList = new List<DisplayModalPOsCurrentMonth>();
            try
            {
                foreach (var subItem in list.POsCurrentMonth)
                {
                    DisplayModalPOsCurrentMonth SubDisp = new DisplayModalPOsCurrentMonth();

                    SubDisp.Account_Code = subItem.Account_Code;
                    SubDisp.Account_Description = subItem.Account_Description;
                    SubDisp.Total = subItem.PO_TOTAL.ToString();
                    SubDisp.PO_NO = subItem.PO_NO;
                    SubDisp.PO_DATE = Utility.ConvertToDate(subItem.PO_DATE);
                    SubDisp.Code = subItem.Account_Code.Substring(0, 3);
                    SubDisp.PO_TOTAL_BASE_CURRENCY = subItem.PO_TOTAL_BASE_CURRENCY;
                    SubDisp.PO_TOTAL = subItem.PO_TOTAL;
                    SubDisp.PO_STATUS = subItem.PO_STATUS;
                    SubDisp.PO_EXCH_RATE = subItem.PO_EXCH_RATE;
                    SubDisp.PO_Currency = subItem.PO_Currency;
                    SubDisp.PORECVDATE = Utility.ConvertToDate(subItem.PORECVDATE);
                    SubDisp.FORWARDER_RECVD_DATE = Utility.ConvertToDate(subItem.FORWARDER_RECVD_DATE);

                    string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == subItem.Account_Code.Substring(0, 3)).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                    SubDisp.Description = desc;
                    DispList.Add(SubDisp);
                    int cnt = displayListNew.Where(x => x.PO_NO == subItem.PO_NO).ToList().Count();
                    if (cnt == 0)
                    {
                        displayListNew.Add(SubDisp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        public List<DisplayModalOpenPOsPreviousMonths> GetDisplayDataListUALOpexPOsPreviousMonth(UALOpexFormula list)
        {
            List<DisplayModalOpenPOsPreviousMonths> displayListNew = new List<DisplayModalOpenPOsPreviousMonths>();
            List<DisplayModalOpenPOsPreviousMonths> DispList = new List<DisplayModalOpenPOsPreviousMonths>();
            try
            {
                foreach (var subItem in list.OpenPOsPreviousMonths)
                {
                    DisplayModalOpenPOsPreviousMonths SubDisp = new DisplayModalOpenPOsPreviousMonths();

                    SubDisp.Account_Code = subItem.Account_Code;
                    SubDisp.Account_Description = subItem.Account_Description;
                    SubDisp.Total = subItem.PO_TOTAL.ToString();
                    SubDisp.PO_NO = subItem.PO_NO;
                    SubDisp.PO_DATE = Utility.ConvertToDate(subItem.PO_DATE);
                    SubDisp.Code = subItem.Account_Code.Substring(0, 3);
                    SubDisp.PO_TOTAL_BASE_CURRENCY = subItem.PO_TOTAL_BASE_CURRENCY;
                    SubDisp.PO_TOTAL = subItem.PO_TOTAL;
                    SubDisp.PO_STATUS = subItem.PO_STATUS;
                    SubDisp.PO_EXCH_RATE = subItem.PO_EXCH_RATE;
                    SubDisp.PO_Currency = subItem.PO_Currency;
                    SubDisp.PORECVDATE = Utility.ConvertToDate(subItem.PORECVDATE);
                    SubDisp.FORWARDER_RECVD_DATE = Utility.ConvertToDate(subItem.FORWARDER_RECVD_DATE);
                    string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == subItem.Account_Code.Substring(0, 3)).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                    SubDisp.Description = desc;
                    DispList.Add(SubDisp);
                    int cnt = displayListNew.Where(x => x.PO_NO == subItem.PO_NO).ToList().Count();
                    if (cnt == 0)
                    {
                        displayListNew.Add(SubDisp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        public List<DisplayModalPreviousMonthPObalance> GetDisplayDataListUALOpexPOPreviousMonthPOBalance(UALOpexFormula list)
        {
            List<DisplayModalPreviousMonthPObalance> displayListNew = new List<DisplayModalPreviousMonthPObalance>();
            List<DisplayModalPreviousMonthPObalance> DispList = new List<DisplayModalPreviousMonthPObalance>();
            try
            {
                foreach (var subItem in list.PreviousMonthPObalance)
                {
                    DisplayModalPreviousMonthPObalance SubDisp = new DisplayModalPreviousMonthPObalance();

                    SubDisp.Account_Code = subItem.Account_Code;
                    SubDisp.Account_Description = subItem.Account_Description;
                    SubDisp.Total = subItem.PO_TOTAL.ToString();
                    SubDisp.PO_NO = subItem.PO_NO;
                    SubDisp.PO_DATE = Utility.ConvertToDate(subItem.PO_DATE);
                    SubDisp.Code = subItem.Account_Code.Substring(0, 3);
                    SubDisp.PO_TOTAL_BASE_CURRENCY = subItem.PO_TOTAL_BASE_CURRENCY;
                    SubDisp.PO_TOTAL = subItem.PO_TOTAL;
                    SubDisp.PO_STATUS = subItem.PO_STATUS;
                    SubDisp.PO_EXCH_RATE = subItem.PO_EXCH_RATE;
                    SubDisp.PO_Currency = subItem.PO_Currency;
                    SubDisp.PORECVDATE = Utility.ConvertToDate(subItem.PORECVDATE);
                    SubDisp.FORWARDER_RECVD_DATE = Utility.ConvertToDate(subItem.FORWARDER_RECVD_DATE);

                    string desc = ACCodeList.Where(x => x.ACCOUNT_CODE == subItem.Account_Code.Substring(0, 3)).Select(x => x.ACCOUNT_DESCR).FirstOrDefault();
                    SubDisp.Description = desc;
                    DispList.Add(SubDisp);
                    int cnt = displayListNew.Where(x => x.PO_NO == subItem.PO_NO).ToList().Count();
                    if (cnt == 0)
                    {
                        displayListNew.Add(SubDisp);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        public List<FinalOpexCode> GetDisplayDataListUALOpexReport(UALOpexFormula list)
        {
            try
            {
                foreach (var item in ActualInvoicesCode)
                {
                    FinalOpexCode obj = new FinalOpexCode();
                    obj.Code = item.Code;
                    obj.Opex = item.Total;
                    FinalOpexCode.Add(obj);
                }
                foreach (var item in InvoicesReceivedCurrentMonthCode)
                {
                    int count = FinalOpexCode.Where(x => x.Code == item.Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexCode obj = new FinalOpexCode();
                        obj.Code = item.Code;
                        obj.Opex = item.Total;
                        FinalOpexCode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexCode.Where(x => x.Code == item.Code).FirstOrDefault();
                        data.Opex = data.Opex + item.Total;
                    }

                }
                foreach (var item in POsCurrentMonthCode)
                {
                    int count = FinalOpexCode.Where(x => x.Code == item.Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexCode obj = new FinalOpexCode();
                        obj.Code = item.Code;
                        obj.Opex = item.Total;
                        FinalOpexCode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexCode.Where(x => x.Code == item.Code).FirstOrDefault();
                        data.Opex = data.Opex + item.Total;
                    }

                }
                foreach (var item in OpenPOsPreviousMonthsCode)
                {
                    int count = FinalOpexCode.Where(x => x.Code == item.Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexCode obj = new FinalOpexCode();
                        obj.Code = item.Code;
                        obj.Opex = item.Total;
                        FinalOpexCode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexCode.Where(x => x.Code == item.Code).FirstOrDefault();
                        data.Opex = data.Opex + item.Total;
                    }
                }
                foreach (var item in PreviousMonthPObalanceCode)
                {
                    int count = FinalOpexCode.Where(x => x.Code == item.Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexCode obj = new FinalOpexCode();
                        obj.Code = item.Code;
                        obj.Opex = item.Total;
                        FinalOpexCode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexCode.Where(x => x.Code == item.Code).FirstOrDefault();
                        data.Opex = data.Opex - item.Total;
                    }
                }

                foreach (var item in FinalOpexCode)
                {
                    item.ActualInvoicesCurrentMonth = ActualInvoicesCode.Where(x => x.Code == item.Code).Select(x => x.Total).FirstOrDefault();
                    item.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthCode.Where(x => x.Code == item.Code).Select(x => x.Total).FirstOrDefault();
                    item.OpenPOsCurrentMonth = POsCurrentMonthCode.Where(x => x.Code == item.Code).Select(x => x.Total).FirstOrDefault();
                    item.OpenPOsPreviousMonths = OpenPOsPreviousMonthsCode.Where(x => x.Code == item.Code).Select(x => x.Total).FirstOrDefault();
                    item.PreviousMonthPObalance = PreviousMonthPObalanceCode.Where(x => x.Code == item.Code).Select(x => x.Total).FirstOrDefault();
                }

                foreach (var item in ActualInvoicesList)
                {

                    int count = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexSubcode obj = new FinalOpexSubcode();
                        obj.Account_Code = item.Account_Code;
                        obj.Code = item.Code;
                        obj.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.Opex = obj.ActualInvoicesCurrentMonth + obj.InvoicesReceivedCurrentMonth + obj.OpenPOsCurrentMonth + obj.OpenPOsPreviousMonths - obj.PreviousMonthPObalance;
                        FinalOpexSubcode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).FirstOrDefault();
                        data.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.Opex = data.ActualInvoicesCurrentMonth + data.InvoicesReceivedCurrentMonth + data.OpenPOsCurrentMonth + data.OpenPOsPreviousMonths - data.PreviousMonthPObalance;
                    }
                }
                foreach (var item in InvoicesReceivedCurrentMonth)
                {
                    int count = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexSubcode obj = new FinalOpexSubcode();
                        obj.Account_Code = item.Account_Code;
                        obj.Code = item.Code;
                        obj.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.Opex = obj.ActualInvoicesCurrentMonth + obj.InvoicesReceivedCurrentMonth + obj.OpenPOsCurrentMonth + obj.OpenPOsPreviousMonths - obj.PreviousMonthPObalance;
                        FinalOpexSubcode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).FirstOrDefault();
                        data.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.Opex = data.ActualInvoicesCurrentMonth + data.InvoicesReceivedCurrentMonth + data.OpenPOsCurrentMonth + data.OpenPOsPreviousMonths - data.PreviousMonthPObalance;
                    }
                }
                foreach (var item in POsCurrentMonth)
                {
                    int count = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexSubcode obj = new FinalOpexSubcode();
                        obj.Account_Code = item.Account_Code;
                        obj.Code = item.Code;
                        obj.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.Opex = obj.ActualInvoicesCurrentMonth + obj.InvoicesReceivedCurrentMonth + obj.OpenPOsCurrentMonth + obj.OpenPOsPreviousMonths - obj.PreviousMonthPObalance;
                        FinalOpexSubcode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).FirstOrDefault();
                        data.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.Opex = data.ActualInvoicesCurrentMonth + data.InvoicesReceivedCurrentMonth + data.OpenPOsCurrentMonth + data.OpenPOsPreviousMonths - data.PreviousMonthPObalance;
                    }
                }
                foreach (var item in OpenPOsPreviousMonths)
                {
                    int count = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexSubcode obj = new FinalOpexSubcode();
                        obj.Account_Code = item.Account_Code;
                        obj.Code = item.Code;
                        obj.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.Opex = obj.ActualInvoicesCurrentMonth + obj.InvoicesReceivedCurrentMonth + obj.OpenPOsCurrentMonth + obj.OpenPOsPreviousMonths - obj.PreviousMonthPObalance;
                        FinalOpexSubcode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).FirstOrDefault();
                        data.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.Opex = data.ActualInvoicesCurrentMonth + data.InvoicesReceivedCurrentMonth + data.OpenPOsCurrentMonth + data.OpenPOsPreviousMonths - data.PreviousMonthPObalance;
                    }
                }
                foreach (var item in PreviousMonthPObalance)
                {
                    int count = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).Count();
                    if (count <= 0)
                    {
                        FinalOpexSubcode obj = new FinalOpexSubcode();
                        obj.Account_Code = item.Account_Code;
                        obj.Code = item.Code;
                        obj.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        obj.Opex = obj.ActualInvoicesCurrentMonth + obj.InvoicesReceivedCurrentMonth + obj.OpenPOsCurrentMonth + obj.OpenPOsPreviousMonths - obj.PreviousMonthPObalance;
                        FinalOpexSubcode.Add(obj);
                    }
                    else
                    {
                        var data = FinalOpexSubcode.Where(x => x.Account_Code == item.Account_Code).FirstOrDefault();
                        data.ActualInvoicesCurrentMonth = ActualInvoicesSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsCurrentMonth = POsCurrentMonthSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.OpenPOsPreviousMonths = OpenPOsPreviousMonthsSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.PreviousMonthPObalance = PreviousMonthPObalanceSubCode.Where(x => x.ACCOUNT_CODE == item.Account_Code).Select(x => x.Total).FirstOrDefault();
                        data.Opex = data.ActualInvoicesCurrentMonth + data.InvoicesReceivedCurrentMonth + data.OpenPOsCurrentMonth + data.OpenPOsPreviousMonths - data.PreviousMonthPObalance;
                    }
                }
                ViewBag.table6Code = FinalOpexCode.OrderBy(x => x.Code);
                ViewBag.table6SubCode = FinalOpexSubcode.OrderBy(x => x.Account_Code);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return FinalOpexCode;
        }
        public List<DisplayModalList> GetPrintDataList(List<SMV_ACCOUNT_RECONCILATION_RPT> list, string Period)
        {
            List<DisplayModalList> DispList = new List<DisplayModalList>();
            try
            {
                APIHelper _helper = new APIHelper();
                List<SMV_ACCOUNT_RECONCILATION_RPT> MainCatList = list.GroupBy(x => x.AccountCode).Select(y => y.FirstOrDefault()).ToList();
                double Amount = 0;
                foreach (var item in MainCatList)
                {
                    DisplayModalList MainDisp = new DisplayModalList();
                    MainDisp.Code = string.Empty;
                    MainDisp.AccountCode = string.Empty;
                    MainDisp.AccountDescription = item.AccountDescription;
                    MainDisp.Total = string.Empty;
                    // MainDisp.TOTALAMT = string.Empty;
                    DispList.Add(MainDisp);

                    // Sub Category
                    List<SMV_ACCOUNT_RECONCILATION_RPT> SubCatList = list.Where(x => x.AccountCode == item.AccountCode).ToList();
                    double subAmount = 0;
                    foreach (var subItem in SubCatList)
                    {
                        DisplayModalList SubDisp = new DisplayModalList();
                        SubDisp.Code = string.Empty;
                        SubDisp.AccountCode = string.Empty;
                        SubDisp.AccountDescription = subItem.AccountDescription;
                        SubDisp.Total = subItem.POTOTAL.ToString(); //GetTotalList(Period, subItem);
                        // SubDisp.TOTALAMT = string.Empty;
                        DispList.Add(SubDisp);
                        subAmount = subAmount + Utility.ToDouble(SubDisp.Total);
                    }

                    double TotalAMTSum = Convert.ToDouble(subAmount);//GetTotalAMTList(Period, SubCatList);
                    MainDisp = new DisplayModalList();
                    MainDisp.Code = "SubCatLast";
                    MainDisp.AccountCode = string.Empty;
                    MainDisp.AccountDescription = string.Empty;
                    MainDisp.Total = TotalAMTSum.ToString();
                    //MainDisp.TOTALAMT = string.Empty;
                    DispList.Add(MainDisp);

                    Amount = Amount + subAmount;
                }
                DisplayModalList TotalDisp = new DisplayModalList();
                TotalDisp.Code = "Last";
                TotalDisp.AccountCode = string.Empty;
                TotalDisp.AccountDescription = "TOTAL";
                TotalDisp.Total = Amount.ToString();
                // TotalDisp.TOTALAMT = string.Empty;
                DispList.Add(TotalDisp);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return DispList;
        }

        public double GetTotalAMTList(string Period, List<SMV_ACCOUNT_RECONCILATION_RPT> SubCatList)
        {
            double TotalSum = 0;
            foreach (var subItem in SubCatList)
            {
                // TotalSum = TotalSum + subItem.POTOTAL;
            }
            return TotalSum;
        }
        public string GetTotalList(string Period, SMV_ACCOUNT_RECONCILATION_RPT subItem)
        {
            double TotalSum = 0;
            //switch (Period)
            //{
            //    case "Annual":
            //        TotalSum = subItem.P01 + subItem.P02 + subItem.P03 + subItem.P04 + subItem.P05 + subItem.P06 + subItem.P07 + subItem.P08 + subItem.P09 + subItem.P10 + subItem.P11 + subItem.P12;
            //        break;
            //    case "January":
            //        TotalSum = subItem.P01;
            //        break;
            //    case "February":
            //        TotalSum = subItem.P02;
            //        break;
            //    case "March":
            //        TotalSum = subItem.P03;
            //        break;
            //    case "April":
            //        TotalSum = subItem.P04;
            //        break;
            //    case "May":
            //        TotalSum = subItem.P05;
            //        break;
            //    case "June":
            //        TotalSum = subItem.P06;
            //        break;
            //    case "July":
            //        TotalSum = subItem.P07;
            //        break;
            //    case "August":
            //        TotalSum = subItem.P08;
            //        break;
            //    case "September":
            //        TotalSum = subItem.P09;
            //        break;
            //    case "October":
            //        TotalSum = subItem.P10;
            //        break;
            //    case "November":
            //        TotalSum = subItem.P11;
            //        break;
            //    case "December":
            //        TotalSum = subItem.P12;
            //        break;
            //}
            return TotalSum.ToString();
        }
        public int FilterMonth(string selected)
        {
            int momth = 0;
            switch (selected)
            {
                case "Annual":
                    momth = 0;
                    break;
                case "January":
                    momth = 1;
                    break;
                case "February":
                    momth = 2;
                    break;
                case "March":
                    momth = 3;
                    break;
                case "April":
                    momth = 4;
                    break;
                case "May":
                    momth = 5;
                    break;
                case "June":
                    momth = 6;
                    break;
                case "July":
                    momth = 7;
                    break;
                case "August":
                    momth = 8;
                    break;
                case "September":
                    momth = 9;
                    break;
                case "October":
                    momth = 10;
                    break;
                case "November":
                    momth = 11;
                    break;
                case "December":
                    momth = 12;
                    break;
            }
            return momth;
        }
        public string getTwoDigitNo(string no)
        {
            if (no == null || no == "0")
                return no;
            else
            {
                no = String.Format("{0:0.00}", no);
                return no;
            }

        }
        public List<Export> getPrintData()
        {
            int month = FilterMonth(TempData["SelectedPeriod"].ToString());
            List<SMV_ACCOUNT_RECONCILATION_RPT> list = GetReportDataList();
            List<DisplayModalList> DispList = GetDisplayDataList(list);
            List<Export> data = new List<Export>();
            List<MainCode> codeListexport = new List<MainCode>();
            List<SubCode> subcodelistexport = new List<SubCode>();
            if (month > 0)
            {
                DispList = DispList.Where(x => x.PODATE.Month == month).ToList();
                AlldisplayList = AlldisplayList.Where(x => x.PODATE.Month == month).ToList();
            }
            subcodelistexport = DispList
                        .GroupBy(l => l.AccountCode)
                        .Select(cl => new SubCode
                        {
                            Code = cl.First().Code,
                            ACCOUNT_CODE = cl.First().AccountCode,
                            Total = cl.Sum(c => Convert.ToDouble(c.POTOTALBASECURRENCY)).ToString(),
                            ACCOUNT_DESCR = cl.First().AccountDescription,
                        }).ToList();

            codeListexport = DispList
                            .GroupBy(l => l.Code)
                            .Select(cl => new MainCode
                            {
                                Code = cl.First().Code,
                                Total = cl.Sum(c => Convert.ToDouble(c.POTOTALBASECURRENCY)).ToString(),
                                Description = cl.First().Description
                            }).ToList();


            ViewBag.printdata = data;
            return data;
        }

        public ActionResult Invoice(string month = "Annual", int year = 0)
        {
            DateTime target = new DateTime(2007, 1, 1);
            List<int> lstYears = new List<int>();
            while (target < DateTime.Today)
            {
                lstYears.Add(target.Year);
                target = target.AddYears(1);
            }
            ViewBag.yearList = lstYears.OrderByDescending(x => x).ToList();
            TempData["Invoice"] = month;
            ViewBag.selected = month;
            if (year == 0)
                year = DateTime.Today.Year;
            ViewBag.year = year;
            int _month = FilterMonth(month);
            ParentCodeForInvoices(_month, year);
            return View();
        }
        public ActionResult PurchaseOrder(string month = "Annual", int year = 0)
        {
            DateTime target = new DateTime(2007, 1, 1);
            List<int> lstYears = new List<int>();
            while (target < DateTime.Today)
            {
                lstYears.Add(target.Year);
                target = target.AddYears(1);
            }
            ViewBag.yearList = lstYears.OrderByDescending(x => x).ToList();
            TempData["SelectedPeriodorder"] = month;
            ViewBag.selected = month;
            if (year == 0)
                year = DateTime.Today.Year;
            ViewBag.year = year;
            int _month = FilterMonth(month);
            ParentCodeForPurchaseOrder(_month, year);
            return View();
        }
        public ActionResult UALOpexFormulaReport(string month = "Annual", int year = 0)
        {
            DateTime target = new DateTime(2007, 1, 1);
            List<int> lstYears = new List<int>();
            while (target < DateTime.Today)
            {
                lstYears.Add(target.Year);
                target = target.AddYears(1);
            }
            ViewBag.yearList = lstYears.OrderByDescending(x => x).ToList();
            UALOpexFormula list = new BLL.Modals.UALOpexFormula();
            TempData["UALOpexFormula"] = month;
            TempData["UALOpexFormulaYear"] = year;
            ViewBag.month = month;
            if (year == 0)
                year = DateTime.Today.Year;
            ViewBag.year = year;
            int _month;

            //DateTime st = new DateTime(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["year"]), _month, 1);
            //if (_month != 12)
            //{
            //    DateTime ed = new DateTime(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["year"]), _month + 1, 1);
            //    list = GetReportDataListUALOpexFormula(st, ed);
            //}
            //else
            //{
            //    DateTime ed = new DateTime(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["year"]), _month, 31);
            //    list = GetReportDataListUALOpexFormula(st, ed);
            //}

            if (month == "Annual")
            {
                DateTime st = new DateTime(year, 1, 1);
                DateTime ed = new DateTime(year, 12, 31);
                list = GetReportDataListUALOpexFormula(st, ed);
            }
            else
            {
                _month = FilterMonth(month);
                DateTime st = new DateTime(year, _month, 1);
                DateTime ed = GetEndDate(year, _month);
                list = GetReportDataListUALOpexFormula(st, ed);
            }

            GetOpexReportDate();
            ParentCodeForActualInvoices(list);
            ParentCodeForInvoicesReceivedCurrentMonthCode(list);
            ParentCodeForOpenPOsPreviousMonthsCode(list);
            ParentCodeForPOsCurrentMonthCode(list);
            ParentCodeForPreviousMonthPObalance(list);
            GetDisplayDataListUALOpexReport(list);
            return View();
        }

        public DateTime GetEndDate(int Year, int Month)
        {
            switch (Month)
            {
                case 1:
                    return new DateTime(Year, Month, 31);
                case 2:
                    return new DateTime(Year, Month, 28);
                case 3:
                    return new DateTime(Year, Month, 31);
                case 4:
                    return new DateTime(Year, Month, 30);
                case 5:
                    return new DateTime(Year, Month, 31);
                case 6:
                    return new DateTime(Year, Month, 30);
                case 7:
                    return new DateTime(Year, Month, 31);
                case 8:
                    return new DateTime(Year, Month, 31);
                case 9:
                    return new DateTime(Year, Month, 30);
                case 10:
                    return new DateTime(Year, Month, 31);
                case 11:
                    return new DateTime(Year, Month, 30);
                case 12:
                    return new DateTime(Year, Month, 31);
            }
            return Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
        }

        #region get parent code
        public List<CodeInvoice> ParentCodeForOpex(int _month, int year)
        {
            List<SMV_ACCOUNT_RECONCILATION_RPT> list = GetReportDataList();
            displayList = GetDisplayDataList(list);
            if (_month > 0)
            {
                displayList = displayList.Where(x => x.PODATE.Month == _month && x.PODATE.Year == year).ToList();
            }
            else
            {
                List<DisplayModalList> DisplayModalListNew = new List<DisplayModalList>();
                for (int i = 1; i <= 12; i++)
                {
                    DisplayModalListNew.AddRange(displayList.Where(x => x.PODATE.Month == i && x.PODATE.Year == year).ToList());
                }
                displayList = new List<DisplayModalList>();
                displayList = DisplayModalListNew;
            }
            codeList = displayList
                          .GroupBy(l => l.Code)
                          .Select(cl => new MainCode
                          {
                              Code = cl.First().Code,
                              Total = cl.Sum(c => Convert.ToDouble(c.POTOTAL)).ToString(),
                              Description = cl.First().Description
                          }).OrderBy(x => Convert.ToUInt64(x.Code)).ToList();

            subcodelist = displayList
                        .GroupBy(l => l.AccountCode)
                        .Select(cl => new SubCode
                        {
                            Code = cl.First().Code,
                            ACCOUNT_CODE = cl.First().AccountCode,
                            Total = cl.Sum(c => Convert.ToDouble(c.POTOTAL)).ToString(),
                            ACCOUNT_DESCR = cl.First().AccountDescription,
                        }).ToList();
            ViewBag.opexCode = codeList.OrderBy(x => Convert.ToUInt64(x.Code));
            ViewBag.openSubCode = subcodelist.OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE));
            ViewBag.opexdeatils = displayList.OrderBy(x => Convert.ToUInt64(x.AccountCode));
            return codelistforinvoice;
        }
        public List<CodeInvoice> ParentCodeForInvoices(int _month, int year)
        {
            List<Invoice> list = GetReportDataListInvoice();
            invoiceList = GetDisplayDataListInvoice(list);
            if (_month > 0)
            {
                invoiceList = invoiceList.Where(x => x.InvoiceDate.Month == _month && x.InvoiceDate.Year == year).ToList();
            }
            else
            {
                List<DisplayModalInvoice> invoiceListNew = new List<DisplayModalInvoice>();
                for (int i = 1; i <= 12; i++)
                {
                    invoiceListNew.AddRange(invoiceList.Where(x => x.InvoiceDate.Month == i && x.InvoiceDate.Year == year).ToList());
                }
                invoiceList = new List<DisplayModalInvoice>();
                invoiceList = invoiceListNew;
            }
            codelistforinvoice = invoiceList
                          .GroupBy(l => l.Code)
                          .Select(cl => new CodeInvoice
                          {
                              Code = cl.First().Code,
                              Total = cl.Sum(c => Convert.ToDouble(c.INVOICEAMOUNTUSD)),
                              Description = cl.First().Description
                          }).OrderBy(x => Convert.ToUInt64(x.Code)).ToList();

            SubCodeInvoice = invoiceList
                        .GroupBy(l => l.AccountCode)
                        .Select(cl => new SubCodeInvoice
                        {
                            Code = cl.First().Code,
                            ACCOUNT_CODE = cl.First().AccountCode,
                            Total = cl.Sum(c => Convert.ToDouble(c.INVOICEAMOUNTUSD)),
                            ACCOUNT_DESCR = cl.First().AccountDescription,
                        }).ToList();
            ViewBag.InvoiceCode = codelistforinvoice.OrderBy(x => Convert.ToUInt64(x.Code));
            ViewBag.InvoiceSubCode = SubCodeInvoice.OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE));
            ViewBag.Invoicedeatils = invoiceList.OrderBy(x => Convert.ToUInt64(x.AccountCode));
            return codelistforinvoice;
        }
        public List<CodePurchaseOrder> ParentCodeForPurchaseOrder(int _month, int year)
        {
            List<PurchaseOrder> list = GetReportDataListPurchaseOrder();
            orderList = GetDisplayDataListPurchaseOrder(list);
            if (_month > 0)
            {
                orderList = orderList.Where(x => x.PODATE.Month == _month && x.PODATE.Year == year).ToList();
            }
            else
            {
                //orderList = orderList.Where(x => x.PODATE.Month == _month).ToList();
                List<DisplayModalPurchaseOrder> orderListNew = new List<DisplayModalPurchaseOrder>();
                for (int i = 1; i <= 12; i++)
                {
                    orderListNew.AddRange(orderList.Where(x => x.PODATE.Month == i && x.PODATE.Year == year).ToList());
                }
                orderList = new List<DisplayModalPurchaseOrder>();
                orderList = orderListNew;
            }
            CodePurchaseOrder = orderList
                          .GroupBy(l => l.Code)
                          .Select(cl => new CodePurchaseOrder
                          {
                              Code = cl.First().Code,
                              Total = cl.Sum(c => Convert.ToDouble(c.POTOTAL)),
                              Description = cl.First().Description
                          }).OrderBy(x => Convert.ToUInt64(x.Code)).ToList();

            SubCodePurchaseOrder = orderList
                        .GroupBy(l => l.AccountCode)
                        .Select(cl => new SubCodePurchaseOrder
                        {
                            Code = cl.First().Code,
                            ACCOUNT_CODE = cl.First().AccountCode,
                            Total = cl.Sum(c => Convert.ToDouble(c.POTOTAL)),
                            ACCOUNT_DESCR = cl.First().AccountDescription,
                        }).ToList();
            ViewBag.PurchaseOrderCode = CodePurchaseOrder.OrderBy(x => Convert.ToUInt64(x.Code));
            ViewBag.PurchaseOrderSubCode = SubCodePurchaseOrder.OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE));
            ViewBag.PurchaseOrderdeatils = orderList.OrderBy(x => Convert.ToUInt64(x.AccountCode));
            return CodePurchaseOrder;
        }
        public List<ActualInvoicesCode> ParentCodeForActualInvoices(UALOpexFormula list)
        {
            ActualInvoicesList = GetDisplayDataListUALOpex(list);
            ActualInvoicesCode = ActualInvoicesList
                       .GroupBy(l => l.Code)
                       .Select(cl => new ActualInvoicesCode
                       {
                           Code = cl.First().Code,
                           Total = cl.Sum(c => c.INVOICE_AMOUNT_USD),
                           Description = cl.First().Description
                       }).OrderBy(x => x.Code).ToList();
            ActualInvoicesSubCode = ActualInvoicesList
                         .GroupBy(l => l.Account_Code)
                         .Select(cl => new ActualInvoicesSubCode
                         {
                             Code = cl.First().Code,
                             ACCOUNT_CODE = cl.First().Account_Code,
                             Total = cl.Sum(c => Convert.ToDouble(c.INVOICE_AMOUNT_USD)),
                             ACCOUNT_DESCR = cl.First().Account_Description,
                         }).ToList();
            ViewBag.table1Code = ActualInvoicesCode.OrderBy(x => Convert.ToUInt64(x.Code));
            ViewBag.table1SubCode = ActualInvoicesSubCode.OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE));
            ViewBag.table1deatils = ActualInvoicesList.OrderBy(x => Convert.ToUInt64(x.Account_Code));
            return ActualInvoicesCode;
        }
        public List<InvoicesReceivedCurrentMonthCode> ParentCodeForInvoicesReceivedCurrentMonthCode(UALOpexFormula list)
        {
            InvoicesReceivedCurrentMonth = GetDisplayDataListUALOpexIRCurrentMonth(list);
            InvoicesReceivedCurrentMonthCode = InvoicesReceivedCurrentMonth
                       .GroupBy(l => l.Code)
                       .Select(cl => new InvoicesReceivedCurrentMonthCode
                       {
                           Code = cl.First().Code,
                           Total = cl.Sum(c => c.INVOICE_AMOUNT_USD),
                           Description = cl.First().Description
                       }).OrderBy(x => x.Code).ToList();
            InvoicesReceivedCurrentMonthSubCode = InvoicesReceivedCurrentMonth
                         .GroupBy(l => l.Account_Code)

                         .Select(cl => new InvoicesReceivedCurrentMonthSubCode
                         {
                             Code = cl.First().Code,
                             ACCOUNT_CODE = cl.First().Account_Code,
                             Total = cl.Sum(c => c.INVOICE_AMOUNT_USD),
                             ACCOUNT_DESCR = cl.First().Account_Description,
                         }).ToList();
            ViewBag.table2Code = InvoicesReceivedCurrentMonthCode.OrderBy(x => x.Code);
            ViewBag.table2SubCode = InvoicesReceivedCurrentMonthSubCode.OrderBy(x => x.ACCOUNT_CODE);
            ViewBag.table2deatils = InvoicesReceivedCurrentMonth.OrderBy(x => x.Account_Code);
            return InvoicesReceivedCurrentMonthCode;
        }
        public List<POsCurrentMonthCode> ParentCodeForPOsCurrentMonthCode(UALOpexFormula list)
        {
            POsCurrentMonth = GetDisplayDataListUALOpexPOsCurrentMonth(list);
            POsCurrentMonthCode = POsCurrentMonth
                       .GroupBy(l => l.Code)
                       .Select(cl => new POsCurrentMonthCode
                       {
                           Code = cl.First().Code,
                           Total = cl.Sum(c => c.PO_TOTAL),
                           Description = cl.First().Description
                       }).OrderBy(x => x.Code).ToList();
            POsCurrentMonthSubCode = POsCurrentMonth
                          .GroupBy(l => l.Account_Code)

                          .Select(cl => new POsCurrentMonthSubCode
                          {
                              Code = cl.First().Code,
                              ACCOUNT_CODE = cl.First().Account_Code,
                              Total = cl.Sum(c => c.PO_TOTAL),
                              ACCOUNT_DESCR = cl.First().Account_Description,
                          }).ToList();
            ViewBag.table3Code = POsCurrentMonthCode.OrderBy(x => x.Code);
            ViewBag.table3SubCode = POsCurrentMonthSubCode.OrderBy(x => x.ACCOUNT_CODE);
            ViewBag.table3deatils = POsCurrentMonth.OrderBy(x => x.Account_Code);
            return POsCurrentMonthCode;
        }
        public List<OpenPOsPreviousMonthsCode> ParentCodeForOpenPOsPreviousMonthsCode(UALOpexFormula list)
        {
            OpenPOsPreviousMonths = GetDisplayDataListUALOpexPOsPreviousMonth(list);
            OpenPOsPreviousMonthsCode = OpenPOsPreviousMonths
                       .GroupBy(l => l.Code)
                       .Select(cl => new OpenPOsPreviousMonthsCode
                       {
                           Code = cl.First().Code,
                           Total = cl.Sum(c => c.PO_TOTAL),
                           Description = cl.First().Description
                       }).OrderBy(x => x.Code).ToList();
            OpenPOsPreviousMonthsSubCode = OpenPOsPreviousMonths
                             .GroupBy(l => l.Account_Code)

                             .Select(cl => new OpenPOsPreviousMonthsSubCode
                             {
                                 Code = cl.First().Code,
                                 ACCOUNT_CODE = cl.First().Account_Code,
                                 Total = cl.Sum(c => c.PO_TOTAL),
                                 ACCOUNT_DESCR = cl.First().Account_Description,
                             }).ToList();
            ViewBag.table4Code = OpenPOsPreviousMonthsCode.OrderBy(x => x.Code);
            ViewBag.table4SubCode = OpenPOsPreviousMonthsSubCode.OrderBy(x => x.ACCOUNT_CODE);
            ViewBag.table4deatils = OpenPOsPreviousMonths.OrderBy(x => x.Account_Code);
            return OpenPOsPreviousMonthsCode;
        }
        public List<PreviousMonthPObalanceCode> ParentCodeForPreviousMonthPObalance(UALOpexFormula list)
        {
            PreviousMonthPObalance = GetDisplayDataListUALOpexPOPreviousMonthPOBalance(list);
            PreviousMonthPObalanceCode = PreviousMonthPObalance
                       .GroupBy(l => l.Code)
                       .Select(cl => new PreviousMonthPObalanceCode
                       {
                           Code = cl.First().Code,
                           Total = cl.Sum(c => c.PO_TOTAL),
                           Description = cl.First().Description
                       }).OrderBy(x => x.Code).ToList();
            PreviousMonthPObalanceSubCode = PreviousMonthPObalance
                         .GroupBy(l => l.Account_Code)

                         .Select(cl => new PreviousMonthPObalanceSubCode
                         {
                             Code = cl.First().Code,
                             ACCOUNT_CODE = cl.First().Account_Code,
                             Total = cl.Sum(c => c.PO_TOTAL),
                             ACCOUNT_DESCR = cl.First().Account_Description,
                         }).ToList();
            ViewBag.table5Code = PreviousMonthPObalanceCode.OrderBy(x => x.Code);
            ViewBag.table5SubCode = PreviousMonthPObalanceSubCode.OrderBy(x => x.ACCOUNT_CODE);
            ViewBag.table5deatils = PreviousMonthPObalance.OrderBy(x => x.Account_Code);
            return PreviousMonthPObalanceCode;
        }
        #endregion

        #region Download reports in excle
        public ActionResult Download(string month, int year)
        {
            try
            {
                List<Export> export = new List<Export>();
                int _month = FilterMonth(month);
                ParentCodeForOpex(_month, year);
                List<Export> data = new List<Export>();
                foreach (var item in codeList)
                {
                    Export ex = new Export();
                    ex.AC_Code = item.Code;
                    ex.Total_USD = item.Total;
                    ex.AC_Description = item.Description;
                    data.Add(ex);

                    List<SubCode> _subcodelistexport = subcodelist.Where(x => x.Code == item.Code).ToList();
                    foreach (var item1 in _subcodelistexport)
                    {
                        Export ex1 = new Export();
                        ex1.Account_Code = item1.ACCOUNT_CODE;
                        ex1.Total = item1.Total;
                        ex1.Account_Description = item1.ACCOUNT_DESCR;
                        data.Add(ex1);
                        List<DisplayModalList> _DispList = displayList.Where(x => x.AccountCode == item1.ACCOUNT_CODE).ToList();
                        foreach (var item2 in _DispList)
                        {
                            Export ex2 = new Export();
                            ex2.Total = item2.POTOTAL.ToString();
                            ex2.Account_Description = item2.AccountDescription;
                            ex2.PO_NO = item2.PONO;
                            ex2.PO_Currency = item2.POCurrency;
                            ex2.PO_TOTAL = getTwoDigitNo(item2.POTOTAL.ToString());
                            ex2.PO_STATUS = item2.POSTATUS;
                            ex2.PO_TOTAL_SHIP_CURRENCY = getTwoDigitNo(item2.POTOTALBASECURRENCY.ToString());
                            ex2.Supplier_Name = item2.SupplierName;
                            ex2.Invoice_Sent = item2.InvoiceSent;
                            ex2.PO_EXCHRATE = getTwoDigitNo(item2.POEXCHRATE.ToString());
                            ex2.INVOICE_No = item2.INVOICENo;
                            ex2.INVOICE_AMOUNT = getTwoDigitNo(item2.INVOICEAMOUNT.ToString());
                            ex2.NET_INVOICE_AMOUNT = getTwoDigitNo(item2.NETINVOICEAMOUNT.ToString());
                            ex2.PO_STATUS = item2.POSTATUS;
                            ex2.INVOICE_DATE = Utility.ConvertToDate(item2.INVOICEDATE);
                            ex2.PO_Received_Date = Utility.ConvertToDate(item2.PORECVDATE);
                            ex2.RECEIVED_AT_FREIGHT_FORWARDER = Utility.ConvertToDate(item2.FORWARDER_RECVD_DATE);
                            ex2.PO_DATE = Utility.ConvertToDate(item2.PODATE);
                            ex2.InvoiceReceivedDate = Utility.ConvertToDate(item2.InvoiceReceivedDate);
                            ex2.INVOICE_AMOUNT_SHIP_CURRENCY = getTwoDigitNo(item2.INVOICEAMOUNTBASECURRENCY.ToString());
                            data.Add(ex2);
                        }
                    }
                }
                var fileName = "Opex_report.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(data, Response.Output);
                Response.End();
                return RedirectToAction("Finance", "Reports");


            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("FinanceList", "Reports");
            }
        }
        public ActionResult DownloadPurchaseOrder(string month, int year)
        {
            try
            {
                List<DisplayModalPurchaseOrder> export = new List<DisplayModalPurchaseOrder>();
                int _month = FilterMonth(month);
                ParentCodeForPurchaseOrder(_month, year);
                foreach (var item in CodePurchaseOrder)
                {
                    DisplayModalPurchaseOrder obj = new DisplayModalPurchaseOrder();
                    obj.Code = item.Code;
                    obj.Description = item.Description;
                    obj.TotalUSD = String.Format("{0:0.00}", item.Total);
                    export.Add(obj);
                    List<SubCodePurchaseOrder> _SubCodePurchaseOrder = SubCodePurchaseOrder.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.Code)).ToList();
                    foreach (var subcode in _SubCodePurchaseOrder)
                    {
                        DisplayModalPurchaseOrder ex1 = new DisplayModalPurchaseOrder();
                        ex1.AccountCode = subcode.ACCOUNT_CODE;
                        ex1.Total = String.Format("{0:0.00}", subcode.Total);
                        ex1.AccountDescription = subcode.ACCOUNT_DESCR;
                        export.Add(ex1);

                        List<DisplayModalPurchaseOrder> _export = orderList.Where(x => x.AccountCode == subcode.ACCOUNT_CODE).OrderBy(x => Convert.ToInt64(x.AccountCode)).ToList();
                        foreach (var item1 in _export)
                        {
                            DisplayModalPurchaseOrder obj1 = new DisplayModalPurchaseOrder();



                            obj1.PONO = item1.PONO;
                            obj1.PODATE = Utility.ConvertToDate(item1.PODATE);

                            obj1.POTOTALBASECURRENCY = item1.POTOTALBASECURRENCY;
                            obj1.POTITLE = item1.POTITLE;
                            obj1.POTOTAL = Math.Round(item1.POTOTAL, 2);
                            obj1.POEXCHRATE = Math.Round(item1.POEXCHRATE);
                            obj1.POSTATUS = item1.POSTATUS;
                            obj1.PORECVDATE = item1.PORECVDATE;
                            obj1.FORWARDER_RECVD_DATE = item1.FORWARDER_RECVD_DATE;
                            obj1.POCurrency = item1.POCurrency;

                            export.Add(obj1);
                        }
                    }
                }
                // Export = Export.OrderBy(x => x.Code).ThenBy(x => x.AccountCode).ToList();
                var fileName = "Purchase_order_report.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("Invoice", "Reports");

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("FinanceList", "Reports");
            }
        }
        public ActionResult DownloadInvoice(string month, int year)
        {
            try
            {
                List<DisplayModalInvoice> export = new List<DisplayModalInvoice>();
                int _month = FilterMonth(month);
                ParentCodeForInvoices(_month, year);
                foreach (var item in codelistforinvoice)
                {
                    DisplayModalInvoice obj = new DisplayModalInvoice();
                    obj.Code = item.Code;
                    obj.Description = item.Description;
                    obj.Total_USD = String.Format("{0:0.00}", item.Total);
                    export.Add(obj);
                    List<SubCodeInvoice> _SubCodeInvoice = SubCodeInvoice.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.Code)).ToList();
                    foreach (var subcode in _SubCodeInvoice)
                    {
                        DisplayModalInvoice ex1 = new DisplayModalInvoice();
                        ex1.AccountCode = subcode.ACCOUNT_CODE;
                        ex1.Total = String.Format("{0:0.00}", subcode.Total);
                        ex1.AccountDescription = subcode.ACCOUNT_DESCR;
                        export.Add(ex1);
                        List<DisplayModalInvoice> _export = invoiceList.Where(x => x.AccountCode == subcode.ACCOUNT_CODE).OrderBy(x => Convert.ToUInt64(x.AccountCode)).ToList();
                        foreach (var item1 in _export)
                        {
                            DisplayModalInvoice obj1 = new DisplayModalInvoice();

                            obj1.AccountDescription = item1.AccountDescription;

                            obj1.PONO = item1.PONO;
                            obj1.INVOICE_AMOUNT = item1.INVOICE_AMOUNT != null ? String.Format("{0:0.00}", item1.INVOICE_AMOUNT) : "";
                            obj1.INVOICENO = item1.INVOICENO;
                            obj1.SupplierName = item1.SupplierName;
                            obj1.POCurrency = item1.POCurrency;
                            obj1.POTOTALUSD = Math.Round(item1.POTOTALUSD);
                            obj1.POSTATUS = item1.POSTATUS;
                            obj1.POTotalPOCurr = String.Format("{0:0.00}", item1.POTotalPOCurr);

                            obj1.InvoiceSent = item1.InvoiceSent;
                            obj1.INVOICEAMOUNTUSD = item1.INVOICEAMOUNTUSD;
                            obj1.INVOICEAMOUNTInvCurr = String.Format("{0:0.00}", item1.INVOICEAMOUNTInvCurr);

                            obj1.InvoiceDate = Utility.ConvertToDate(item1.InvoiceDate);
                            obj1.NETINVOICEAMOUNT = String.Format("{0:0.00}", item1.NETINVOICEAMOUNT);

                            obj1.InvoiceReceivedDate = item1.InvoiceReceivedDate;
                            obj1.InvoiceExchrate = String.Format("{0:0.00}", item1.InvoiceExchrate);
                            obj1.GoodsatForwarderRecvdDate = item1.GoodsatForwarderRecvdDate;

                            obj1.PODATE = Utility.ConvertToDate(item1.PODATE);

                            obj1.SumofInvoiceItems = String.Format("{0:0.00}", item1.SumofInvoiceItems);
                            export.Add(obj1);
                        }
                    }
                }
                // Export = Export.OrderBy(x => x.Code).ThenBy(x => x.AccountCode).ToList();
                var fileName = "Invoice_report.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("Invoice", "Reports");

                // For MS EXCEl Plugin
                //string fullPath = Path.Combine(Server.MapPath("~/temp"), fileName);
                //Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                //bool res = Utility.ExportToExcel(dt, fullPath);
                //Thread.Sleep(2000);
                //string filename = Path.GetFileName(fullPath);
                //byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                //return File(fileBytes, "application/vnd.ms-excel", filename);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("Invoice", "Reports");
            }
        }
        public ActionResult DownloadTable1()
        {
            try
            {
                List<DisplayModalActualInvoices> export = new List<DisplayModalActualInvoices>();
                string monthname = TempData["UALOpexFormula"].ToString();
                string year = TempData["UALOpexFormulaYear"].ToString();
                int _month;

                UALOpexFormula list = new UALOpexFormula();
                if (monthname == "Annual")
                {
                    DateTime st = new DateTime(Convert.ToInt32(year), 1, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), 12, 31);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                else
                {
                    _month = FilterMonth(monthname);
                    DateTime st = new DateTime(Convert.ToInt32(year), _month, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), _month + 1, 1);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                ParentCodeForActualInvoices(list);
                foreach (var item in ActualInvoicesCode)
                {
                    DisplayModalActualInvoices ex = new DisplayModalActualInvoices();
                    ex.Code = item.Code;
                    ex.TotalUSD = String.Format("{0:0.00}", item.Total);
                    ex.Description = item.Description;
                    export.Add(ex);
                    List<ActualInvoicesSubCode> _ActualInvoicesSubCode = ActualInvoicesSubCode.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE)).ToList();
                    foreach (var item1 in _ActualInvoicesSubCode)
                    {
                        DisplayModalActualInvoices ex1 = new DisplayModalActualInvoices();
                        ex1.Account_Code = item1.ACCOUNT_CODE;
                        ex1.Total = String.Format("{0:0.00}", item1.Total);
                        ex1.Account_Description = item1.ACCOUNT_DESCR;
                        export.Add(ex1);
                        List<DisplayModalActualInvoices> _ActualInvoicesList = ActualInvoicesList.Where(x => x.Account_Code == item1.ACCOUNT_CODE).OrderBy(x => Convert.ToUInt64(x.Account_Code)).ToList();
                        foreach (var item2 in _ActualInvoicesList)
                        {
                            DisplayModalActualInvoices ex2 = new DisplayModalActualInvoices();
                            ex2.PO_NO = item2.PO_NO;
                            ex2.PO_DATE = item2.PO_DATE;

                            ex2.Supplier_Name = item2.Supplier_Name;
                            ex2.PO_TOTALUSD = item2.PO_TOTALUSD;
                            ex2.PO_STATUS = item2.PO_STATUS;
                            ex2.INVOICENO = item2.INVOICENO;
                            ex2.PO_Currency = item2.PO_Currency;
                            ex2.Invoice_Date = item2.Invoice_Date;
                            ex2.Invoice_Received_Date = item2.Invoice_Received_Date;
                            ex2.Invoice_Exch_rate = item2.Invoice_Exch_rate;
                            ex2.Invoice_Sent = item2.Invoice_Sent;
                            ex2.INVOICE_AMOUNT = item2.INVOICE_AMOUNT;
                            ex2.PO_Total_PO_Curr = item2.PO_Total_PO_Curr;
                            ex2.INVOICE_AMOUNT_USD = item2.INVOICE_AMOUNT_USD;
                            ex2.INVOICE_AMOUNT_Inv_Curr = item2.INVOICE_AMOUNT_Inv_Curr;
                            ex2.NET_INVOICE_AMOUNT = item2.NET_INVOICE_AMOUNT;
                            ex2.Invoice_Exch_rate = item2.Invoice_Exch_rate;
                            ex2.Sum_of_Invoice_Items = item2.Sum_of_Invoice_Items;
                            ex2.Goods_at_Forwarder_Recvd_Date = item2.Goods_at_Forwarder_Recvd_Date;
                            export.Add(ex2);
                        }
                    }
                }
                var fileName = "Actual_Invoices_Report.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("UALOpexFormulaReport", "Reports");

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("UALOpexFormulaReport", "Reports");
            }
        }
        public ActionResult DownloadTable2()
        {
            try
            {
                List<DisplayModalInvoicesReceivedCurrentMonth> export = new List<DisplayModalInvoicesReceivedCurrentMonth>();
                int _month;
                string monthname = TempData["UALOpexFormula"].ToString();
                string year = TempData["UALOpexFormulaYear"].ToString();

                UALOpexFormula list = new UALOpexFormula();
                if (monthname == "Annual")
                {
                    DateTime st = new DateTime(Convert.ToInt32(year), 1, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), 12, 31);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                else
                {
                    _month = FilterMonth(monthname);
                    DateTime st = new DateTime(Convert.ToInt32(year), _month, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), _month + 1, 1);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                ParentCodeForInvoicesReceivedCurrentMonthCode(list);
                foreach (var item in InvoicesReceivedCurrentMonthCode)
                {
                    DisplayModalInvoicesReceivedCurrentMonth ex = new DisplayModalInvoicesReceivedCurrentMonth();
                    ex.Code = item.Code;
                    ex.TotalUSD = String.Format("{0:0.00}", item.Total);
                    ex.Description = item.Description;
                    export.Add(ex);
                    List<InvoicesReceivedCurrentMonthSubCode> _InvoicesReceivedCurrentMonthSubCode = InvoicesReceivedCurrentMonthSubCode.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE)).ToList();
                    foreach (var item1 in _InvoicesReceivedCurrentMonthSubCode)
                    {
                        DisplayModalInvoicesReceivedCurrentMonth ex1 = new DisplayModalInvoicesReceivedCurrentMonth();
                        ex1.Account_Code = item1.ACCOUNT_CODE;
                        ex1.Total = String.Format("{0:0.00}", item1.Total);
                        ex1.Account_Description = item1.ACCOUNT_DESCR;
                        export.Add(ex1);
                        List<DisplayModalInvoicesReceivedCurrentMonth> _InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonth.Where(x => x.Account_Code == item1.ACCOUNT_CODE).OrderBy(x => Convert.ToUInt64(x.Account_Code)).ToList();
                        foreach (var item2 in _InvoicesReceivedCurrentMonth)
                        {
                            DisplayModalInvoicesReceivedCurrentMonth ex2 = new DisplayModalInvoicesReceivedCurrentMonth();
                            ex2.PO_NO = item2.PO_NO;
                            ex2.PO_DATE = item2.PO_DATE;

                            ex2.Supplier_Name = item2.Supplier_Name;
                            ex2.PO_TOTALUSD = item2.PO_TOTALUSD;
                            ex2.PO_STATUS = item2.PO_STATUS;
                            ex2.INVOICENO = item2.INVOICENO;
                            ex2.PO_Currency = item2.PO_Currency;
                            ex2.Invoice_Date = item2.Invoice_Date;
                            ex2.Invoice_Received_Date = item2.Invoice_Received_Date;
                            ex2.Invoice_Exch_rate = item2.Invoice_Exch_rate;
                            ex2.Invoice_Sent = item2.Invoice_Sent;
                            ex2.INVOICE_AMOUNT = item2.INVOICE_AMOUNT;
                            ex2.PO_Total_PO_Curr = item2.PO_Total_PO_Curr;
                            ex2.INVOICE_AMOUNT_USD = item2.INVOICE_AMOUNT_USD;
                            ex2.INVOICE_AMOUNT_Inv_Curr = item2.INVOICE_AMOUNT_Inv_Curr;
                            ex2.NET_INVOICE_AMOUNT = item2.NET_INVOICE_AMOUNT;
                            ex2.Invoice_Exch_rate = item2.Invoice_Exch_rate;
                            ex2.Sum_of_Invoice_Items = item2.Sum_of_Invoice_Items;
                            ex2.Goods_at_Forwarder_Recvd_Date = item2.Goods_at_Forwarder_Recvd_Date;
                            export.Add(ex2);
                        }
                    }
                }
                var fileName = "Invoices_Received_Current_Month_Reports.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("UALOpexFormulaReport", "Reports");

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("UALOpexFormulaReport", "Reports");
            }
        }
        public ActionResult DownloadTable3()
        {
            try
            {
                List<DisplayModalPOsCurrentMonth> export = new List<DisplayModalPOsCurrentMonth>();
                int _month;
                string monthname = TempData["UALOpexFormula"].ToString();
                string year = TempData["UALOpexFormulaYear"].ToString();

                UALOpexFormula list = new UALOpexFormula();
                if (monthname == "Annual")
                {
                    DateTime st = new DateTime(Convert.ToInt32(year), 1, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), 12, 31);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                else
                {
                    _month = FilterMonth(monthname);
                    DateTime st = new DateTime(Convert.ToInt32(year), _month, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), _month + 1, 1);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }



                ParentCodeForPOsCurrentMonthCode(list);
                foreach (var item in POsCurrentMonthCode)
                {
                    DisplayModalPOsCurrentMonth ex = new DisplayModalPOsCurrentMonth();
                    ex.Code = item.Code;
                    ex.TotalUSD = String.Format("{0:0.00}", item.Total);
                    ex.Description = item.Description;
                    export.Add(ex);
                    List<POsCurrentMonthSubCode> _POsCurrentMonthSubCode = POsCurrentMonthSubCode.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE)).ToList();
                    foreach (var item1 in _POsCurrentMonthSubCode)
                    {
                        DisplayModalPOsCurrentMonth ex1 = new DisplayModalPOsCurrentMonth();
                        ex1.Account_Code = item1.ACCOUNT_CODE;
                        ex1.Total = String.Format("{0:0.00}", item1.Total);
                        ex1.Account_Description = item1.ACCOUNT_DESCR;
                        export.Add(ex1);
                        List<DisplayModalPOsCurrentMonth> _POsCurrentMonth = POsCurrentMonth.Where(x => x.Account_Code == item1.ACCOUNT_CODE).OrderBy(x => Convert.ToUInt64(x.Account_Code)).ToList();
                        foreach (var item2 in _POsCurrentMonth)
                        {
                            DisplayModalPOsCurrentMonth ex2 = new DisplayModalPOsCurrentMonth();
                            ex2.Account_Description = item2.Account_Description;
                            ex2.PO_NO = item2.PO_NO;
                            ex2.PO_DATE = item2.PO_DATE;
                            ex2.PO_TOTAL_BASE_CURRENCY = item2.PO_TOTAL_BASE_CURRENCY;
                            ex2.PO_TOTAL = item2.PO_TOTAL;
                            ex2.PO_STATUS = item2.PO_STATUS;
                            ex2.PO_EXCH_RATE = item2.PO_EXCH_RATE;
                            ex2.PO_Currency = item2.PO_Currency;
                            ex2.PORECVDATE = item2.PORECVDATE;
                            ex2.FORWARDER_RECVD_DATE = item2.FORWARDER_RECVD_DATE;
                            export.Add(ex2);
                        }
                    }
                }
                var fileName = "Open_POs_Current_Month.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("UALOpexFormulaReport", "Reports");

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("UALOpexFormulaReport", "Reports");
            }
        }
        public ActionResult DownloadTable4()
        {
            try
            {
                List<DisplayModalOpenPOsPreviousMonths> export = new List<DisplayModalOpenPOsPreviousMonths>();
                int _month;
                string monthname = TempData["UALOpexFormula"].ToString();
                string year = TempData["UALOpexFormulaYear"].ToString();

                UALOpexFormula list = new UALOpexFormula();
                if (monthname == "Annual")
                {
                    DateTime st = new DateTime(Convert.ToInt32(year), 1, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), 12, 31);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                else
                {
                    _month = FilterMonth(monthname);
                    DateTime st = new DateTime(Convert.ToInt32(year), _month, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), _month + 1, 1);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }

                ParentCodeForOpenPOsPreviousMonthsCode(list);
                foreach (var item in OpenPOsPreviousMonthsCode)
                {
                    DisplayModalOpenPOsPreviousMonths ex = new DisplayModalOpenPOsPreviousMonths();
                    ex.Code = item.Code;
                    ex.TotalUSD = String.Format("{0:0.00}", item.Total);
                    ex.Description = item.Description;
                    export.Add(ex);
                    List<OpenPOsPreviousMonthsSubCode> _OpenPOsPreviousMonthsSubCode = OpenPOsPreviousMonthsSubCode.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE)).ToList();
                    foreach (var item1 in _OpenPOsPreviousMonthsSubCode)
                    {
                        DisplayModalOpenPOsPreviousMonths ex1 = new DisplayModalOpenPOsPreviousMonths();
                        ex1.Account_Code = item1.ACCOUNT_CODE;
                        ex1.Total = String.Format("{0:0.00}", item1.Total);
                        ex1.Account_Description = item1.ACCOUNT_DESCR;
                        export.Add(ex1);
                        List<DisplayModalOpenPOsPreviousMonths> _OpenPOsPreviousMonths = OpenPOsPreviousMonths.Where(x => x.Account_Code == item1.ACCOUNT_CODE).OrderBy(x => Convert.ToUInt64(x.Account_Code)).ToList();
                        foreach (var item2 in _OpenPOsPreviousMonths)
                        {
                            DisplayModalOpenPOsPreviousMonths ex2 = new DisplayModalOpenPOsPreviousMonths();
                            ex2.Account_Description = item2.Account_Description;
                            ex2.PO_NO = item2.PO_NO;
                            ex2.PO_DATE = item2.PO_DATE;
                            ex2.PO_TOTAL_BASE_CURRENCY = item2.PO_TOTAL_BASE_CURRENCY;
                            ex2.PO_TOTAL = item2.PO_TOTAL;
                            ex2.PO_STATUS = item2.PO_STATUS;
                            ex2.PO_EXCH_RATE = item2.PO_EXCH_RATE;
                            ex2.PO_Currency = item2.PO_Currency;
                            ex2.PORECVDATE = item2.PORECVDATE;
                            ex2.FORWARDER_RECVD_DATE = item2.FORWARDER_RECVD_DATE;
                            export.Add(ex2);
                        }
                    }
                }
                var fileName = "Open_POs_Previous_Month.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("UALOpexFormulaReport", "Reports");

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("UALOpexFormulaReport", "Reports");
            }
        }
        public ActionResult DownloadTable5()
        {
            try
            {
                List<DisplayModalPreviousMonthPObalance> export = new List<DisplayModalPreviousMonthPObalance>();
                int _month;
                string monthname = TempData["UALOpexFormula"].ToString();
                string year = TempData["UALOpexFormulaYear"].ToString();

                UALOpexFormula list = new UALOpexFormula();
                if (monthname == "Annual")
                {
                    DateTime st = new DateTime(Convert.ToInt32(year), 1, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), 12, 31);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                else
                {
                    _month = FilterMonth(monthname);
                    DateTime st = new DateTime(Convert.ToInt32(year), _month, 1);
                    DateTime ed = new DateTime(Convert.ToInt32(year), _month + 1, 1);
                    list = GetReportDataListUALOpexFormula(st, ed);
                }
                ParentCodeForPreviousMonthPObalance(list);
                foreach (var item in PreviousMonthPObalanceCode)
                {
                    DisplayModalPreviousMonthPObalance ex = new DisplayModalPreviousMonthPObalance();
                    ex.Code = item.Code;
                    ex.TotalUSD = String.Format("{0:0.00}", item.Total);
                    ex.Description = item.Description;
                    export.Add(ex);
                    List<PreviousMonthPObalanceSubCode> _PreviousMonthPObalanceSubCode = PreviousMonthPObalanceSubCode.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.ACCOUNT_CODE)).ToList();
                    foreach (var item1 in _PreviousMonthPObalanceSubCode)
                    {
                        DisplayModalPreviousMonthPObalance ex1 = new DisplayModalPreviousMonthPObalance();
                        ex1.Account_Code = item1.ACCOUNT_CODE;
                        ex1.Total = String.Format("{0:0.00}", item1.Total);
                        ex1.Account_Description = item1.ACCOUNT_DESCR;
                        export.Add(ex1);
                        List<DisplayModalPreviousMonthPObalance> _PreviousMonthPObalance = PreviousMonthPObalance.Where(x => x.Account_Code == item1.ACCOUNT_CODE).OrderBy(x => Convert.ToUInt64(x.Account_Code)).ToList();
                        foreach (var item2 in _PreviousMonthPObalance)
                        {
                            DisplayModalPreviousMonthPObalance ex2 = new DisplayModalPreviousMonthPObalance();
                            ex2.Account_Description = item2.Account_Description;
                            ex2.PO_NO = item2.PO_NO;
                            ex2.PO_DATE = item2.PO_DATE;
                            ex2.PO_TOTAL_BASE_CURRENCY = item2.PO_TOTAL_BASE_CURRENCY;
                            ex2.PO_TOTAL = item2.PO_TOTAL;
                            ex2.PO_STATUS = item2.PO_STATUS;
                            ex2.PO_EXCH_RATE = item2.PO_EXCH_RATE;
                            ex2.PO_Currency = item2.PO_Currency;
                            ex2.PORECVDATE = item2.PORECVDATE;
                            ex2.FORWARDER_RECVD_DATE = item2.FORWARDER_RECVD_DATE;
                            export.Add(ex2);
                        }
                    }
                }
                var fileName = "Previous_Month_PO_balance.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("UALOpexFormulaReport", "Reports");

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("UALOpexFormulaReport", "Reports");
            }
        }

        public ActionResult DownloadTable6(bool isConfirm, string month, int year)
        {
            try
            {
                List<FinalOpexSubcode> export = new List<FinalOpexSubcode>();
                UALOpexFormulaReport(month, year);
                if (isConfirm)
                {
                    APIHelper _helper = new APIHelper();
                    OpexReportFinilizeDate data = new OpexReportFinilizeDate();
                    data.FinilizeID = Guid.NewGuid();
                    data.EndDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    data.FinilizeBy = new Guid(Session["UserID"].ToString());
                    DateTime dt = _helper.GetLastFinilizeReportDate();
                    if (dt == null)
                        dt = new DateTime(2018, 8, 1);
                    else
                        data.StardDate = dt;
                    data.StardDate = dt;
                    _helper.UpdateOpexReportDate(data);
                }
                foreach (var item in FinalOpexCode)
                {
                    FinalOpexSubcode ex = new FinalOpexSubcode();
                    ex.Code = item.Code;
                    ex.ActualInvoicesCurrentMonth = Utility.TwoDecimal(item.ActualInvoicesCurrentMonth);
                    ex.InvoicesReceivedCurrentMonth = Utility.TwoDecimal(item.InvoicesReceivedCurrentMonth);
                    ex.OpenPOsCurrentMonth = Utility.TwoDecimal(item.OpenPOsCurrentMonth);
                    ex.OpenPOsPreviousMonths = Utility.TwoDecimal(item.OpenPOsPreviousMonths);
                    ex.PreviousMonthPObalance = Utility.TwoDecimal(item.PreviousMonthPObalance);
                    ex.Opex = item.Opex;
                    export.Add(ex);
                    List<FinalOpexSubcode> _FinalOpexSubcode = FinalOpexSubcode.Where(x => x.Code == item.Code).OrderBy(x => Convert.ToUInt64(x.Code)).ToList();
                    foreach (var item1 in _FinalOpexSubcode)
                    {
                        FinalOpexSubcode ex1 = new FinalOpexSubcode();
                        ex1.Account_Code = item1.Account_Code;
                        ex1.ActualInvoicesCurrentMonth = Utility.TwoDecimal(item1.ActualInvoicesCurrentMonth);
                        ex1.InvoicesReceivedCurrentMonth = Utility.TwoDecimal(item1.InvoicesReceivedCurrentMonth);
                        ex1.OpenPOsCurrentMonth = Utility.TwoDecimal(item1.OpenPOsCurrentMonth);
                        ex1.OpenPOsPreviousMonths = Utility.TwoDecimal(item1.OpenPOsPreviousMonths);
                        ex1.PreviousMonthPObalance = Utility.TwoDecimal(item1.PreviousMonthPObalance);
                        ex1.Opex = item1.Opex;
                        export.Add(ex1);
                    }
                }
                var fileName = "Opex_Final.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("UALOpexFormulaReport", "Reports");
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("UALOpexFormulaReport", "Reports");
            }
        }
        #endregion

        public void GetOpexReportDate()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                DateTime dt = _helper.GetLastFinilizeReportDate();
                ViewBag.OpexDate = dt.ToString("dd/MM/yyyy").Replace('-', '/');
            }
            catch (Exception)
            {

            }
        }
        [HttpPost]
        public ActionResult SaveOpexDate(string Date)
        {
            string EndDate = Date;
            APIHelper _helper = new APIHelper();
            OpexReportFinilizeDate data = new OpexReportFinilizeDate();
            data.FinilizeID = Guid.NewGuid();
            data.EndDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            data.FinilizeBy = new Guid(Session["UserID"].ToString());
            DateTime dt = _helper.GetLastFinilizeReportDate();
            if (dt == null)
                dt = new DateTime(2018, 8, 1);
            else
                data.StardDate = dt;
            data.StardDate = dt;
            _helper.UpdateOpexReportDate(data);
            return Json("yes", JsonRequestBehavior.AllowGet);
        }

        #region Ship Reports Analysis
        public ActionResult ShipReportsAnalysis()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = _helper.GetAllShips();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX")).ToList();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            ViewBag.Ships = shipsList;
            return View();
        }
        public string GetShipReportsData(string shipCode, string shipName)
        {

            List<ShipReportsAnalysisModal> result = new List<ShipReportsAnalysisModal>();
            try
            {
                APIHelper _helper = new APIHelper();
                result = _helper.GetAllShipReportsData(shipCode, shipName);
            }
            catch (Exception)
            {

            }
            return JsonConvert.SerializeObject(result);
        }
        public string GetShipReportsDetails(string reportName, string id)
        {
            string strDetail = string.Empty;
            try
            {
                APIHelper _helper = new APIHelper();
                switch (reportName.ToLower())
                {
                    case "superintended inspection":
                        var objReport = _helper.SIRFormDetailsView(id); //RDBJ 09/24/2021 removed int convert
                        if (objReport != null && objReport.SuperintendedInspectionReport != null)
                        {
                            var objResult = new
                            {
                                Port = objReport.SuperintendedInspectionReport.Port,
                                Master = objReport.SuperintendedInspectionReport.Master,
                                Superintended = objReport.SuperintendedInspectionReport.Superintended,
                                Date = objReport.SuperintendedInspectionReport.Date,
                                Section1_1_Condition = objReport.SuperintendedInspectionReport.Section1_1_Condition,
                                Section1_1_Comment = objReport.SuperintendedInspectionReport.Section1_1_Comment,
                                Section1_2_Condition = objReport.SuperintendedInspectionReport.Section1_2_Condition,
                                Section1_2_Comment = objReport.SuperintendedInspectionReport.Section1_2_Comment,
                                Section1_3_Condition = objReport.SuperintendedInspectionReport.Section1_3_Condition,
                                Section1_3_Comment = objReport.SuperintendedInspectionReport.Section1_3_Comment,
                                Section1_4_Condition = objReport.SuperintendedInspectionReport.Section1_4_Condition,
                                Section1_4_Comment = objReport.SuperintendedInspectionReport.Section1_4_Comment,
                                Section1_5_Condition = objReport.SuperintendedInspectionReport.Section1_5_Condition,
                                Section1_5_Comment = objReport.SuperintendedInspectionReport.Section1_5_Comment,
                                Section1_6_Condition = objReport.SuperintendedInspectionReport.Section1_6_Condition,
                                Section1_6_Comment = objReport.SuperintendedInspectionReport.Section1_6_Comment,
                                Section1_7_Condition = objReport.SuperintendedInspectionReport.Section1_7_Condition,
                                Section1_7_Comment = objReport.SuperintendedInspectionReport.Section1_7_Comment,
                                Section1_8_Condition = objReport.SuperintendedInspectionReport.Section1_8_Condition,
                            };
                            strDetail = JsonConvert.SerializeObject(objResult);
                        }
                        break;
                    case "daily cargo":
                        var objDCReport = _helper.DailyCargoFormDetailsView(Utility.ToInt(id));
                        if (objDCReport != null)
                        {
                            var objResult = new
                            {
                                CargoType = objDCReport.CargoType,
                                CargoRemaining = objDCReport.CargoRemaining,
                                ReportCreated = objDCReport.ReportCreated,
                                VoyageNo = objDCReport.VoyageNo,
                                PortName = objDCReport.PortName,
                                NoOfGangs = objDCReport.NoOfGangs,
                                NoOfShips = objDCReport.NoOfShips,
                                QuantityOfCargoLoaded = objDCReport.QuantityOfCargoLoaded,
                                TotalCargoLoaded = objDCReport.TotalCargoLoaded,
                                FuelOil = objDCReport.FuelOil,
                                DieselOil = objDCReport.DieselOil,
                                SulphurFuelOil = objDCReport.SulphurFuelOil,
                                SulphurDieselOil = objDCReport.SulphurDieselOil,
                                Sludge = objDCReport.Sludge,
                                DirtyOil = objDCReport.DirtyOil,
                                ETCDate = objDCReport.ETCDate,
                                ETCTime = objDCReport.ETCTime,
                                NextPort = objDCReport.NextPort,
                                Remarks = objDCReport.Remarks
                            };
                            strDetail = JsonConvert.SerializeObject(objResult);
                        }
                        break;
                    case "general inspection":
                        var objGIReport = _helper.GIRFormDetailsView(Utility.ToInt(id));
                        if (objGIReport != null)
                        {
                            var objResult = new
                            {
                                Port = objGIReport.Port,
                                Inspector = objGIReport.Inspector,
                                Date = objGIReport.Date,
                                GeneralPreamble = objGIReport.GeneralPreamble,
                                Classsociety = objGIReport.Classsociety,
                                YearofBuild = objGIReport.YearofBuild,
                                Flag = objGIReport.Flag,
                                Classofvessel = objGIReport.Classofvessel,
                                Portofregistry = objGIReport.Portofregistry,
                                MMSI = objGIReport.MMSI,
                                IMOnumber = objGIReport.IMOnumber,
                                Callsign = objGIReport.Callsign,
                                SummerDWT = objGIReport.SummerDWT,
                                Grosstonnage = objGIReport.Grosstonnage,
                                Lightweight = objGIReport.Lightweight,
                                Nettonnage = objGIReport.Nettonnage,
                                Beam = objGIReport.Beam,
                                LOA = objGIReport.LOA,
                                Summerdraft = objGIReport.Summerdraft,
                                LBP = objGIReport.LBP,
                                Bowthruster = objGIReport.Bowthruster,
                                BHP = objGIReport.BHP,
                                Noofholds = objGIReport.Noofholds,
                                Nomoveablebulkheads = objGIReport.Nomoveablebulkheads,
                                Containers = objGIReport.Containers,
                                Cargocapacity = objGIReport.Cargocapacity,
                                Cargohandlingequipment = objGIReport.Cargohandlingequipment,
                                Lastvoyageandcargo = objGIReport.Lastvoyageandcargo,
                                CurrentPlannedvoyageandcargo = objGIReport.CurrentPlannedvoyageandcargo,
                            };
                            strDetail = JsonConvert.SerializeObject(objResult);
                        }
                        break;
                    case "ship management review":
                        var objSMRReport = _helper.GetSMRFormByID(Utility.ToInt(id));
                        if (objSMRReport != null)
                        {
                            var objResult = new
                            {
                                ReviewPeriod = objSMRReport.ReviewPeriod,
                                Year = objSMRReport.Year,
                                DateOfMeeting = objSMRReport.DateOfMeeting,
                                Section1 = objSMRReport.Section1,
                                Section2 = objSMRReport.Section2,
                                Section3 = objSMRReport.Section3,
                                Section4 = objSMRReport.Section4,
                                Section5 = objSMRReport.Section5,
                                Section6 = objSMRReport.Section6,
                                Section7 = objSMRReport.Section7,
                                Section7a = objSMRReport.Section7a,
                                Section7b = objSMRReport.Section7b,
                                Section7c = objSMRReport.Section7c,
                                Section7d = objSMRReport.Section7d,
                                Section7e1 = objSMRReport.Section7e1,
                                Section7e2 = objSMRReport.Section7e2,
                                Section7e3 = objSMRReport.Section7e3,
                                Section7f1 = objSMRReport.Section7f1,
                                Section7f2 = objSMRReport.Section7f2,
                                Section7g = objSMRReport.Section7g,
                                Section7h = objSMRReport.Section7h,
                                Section8a = objSMRReport.Section8a,
                                Section8b = objSMRReport.Section8b,
                                Section8b1 = objSMRReport.Section8b1,
                                Section8b2 = objSMRReport.Section8b2,
                                Section8b3 = objSMRReport.Section8b3,
                                Section8b4 = objSMRReport.Section8b4,
                                Section8b5 = objSMRReport.Section8b5,
                                Section9 = objSMRReport.Section9,
                                Section10 = objSMRReport.Section10,
                                Section11 = objSMRReport.Section11,
                                Section12a = objSMRReport.Section12a,
                                Section12b = objSMRReport.Section12b,
                                Section12c = objSMRReport.Section12c,
                                Section12d = objSMRReport.Section12d,
                                Section12e = objSMRReport.Section12e,
                                Section12f = objSMRReport.Section12f,
                                Section12g = objSMRReport.Section12g,
                                Section12h = objSMRReport.Section12h,
                                Section12i = objSMRReport.Section12i,
                                Section12j = objSMRReport.Section12j,
                                Section12k = objSMRReport.Section12k,
                                Section13 = objSMRReport.Section13
                            };
                            strDetail = JsonConvert.SerializeObject(objResult);
                        }
                        break;
                    case "internal audit form":
                        var objIAFReport = _helper.IAFormDetailsView(new Guid(id));
                        if (objIAFReport != null)
                        {
                            var objResult = new
                            {
                                Location = objIAFReport.InternalAuditForm.Location,
                                AuditNo = objIAFReport.InternalAuditForm.AuditNo,
                                AuditTypeISM = objIAFReport.InternalAuditForm.AuditTypeISM,
                                AuditTypeISPS = objIAFReport.InternalAuditForm.AuditTypeISPS,
                                AuditTypeMLC = objIAFReport.InternalAuditForm.AuditTypeMLC,
                                Date = objIAFReport.InternalAuditForm.Date,
                                Auditor = objIAFReport.InternalAuditForm.Auditor
                            };
                            strDetail = JsonConvert.SerializeObject(objResult);
                        }
                        break;
                    case "arrival":
                        var objArReport = _helper.ArrivalDetailsView(Utility.ToInt(id));
                        if (objArReport != null)
                        {
                            var objResult = new
                            {
                                VoyageNo = objArReport.VoyageNo,
                                PortName = objArReport.PortName,
                                ArrivalDate = objArReport.ArrivalDate,
                                ArrivalTime = objArReport.ArrivalTime,
                                POBDate = objArReport.POBDate,
                                POBTime = objArReport.POBTime,
                                AverageSpeed = objArReport.AverageSpeed,
                                FuelOil = objArReport.FuelOil,
                                DieselOil = objArReport.DieselOil,
                                SulphurFuelOil = objArReport.SulphurFuelOil,
                                SulphurDieselOil = objArReport.SulphurDieselOil,
                                FreshWater = objArReport.FreshWater,
                                LubeOil = objArReport.LubeOil,
                                CargoDate = objArReport.CargoDate,
                                CargoTime = objArReport.CargoTime,
                                NextPort = objArReport.NextPort,
                                Remarks = objArReport.Remarks
                            };
                            strDetail = JsonConvert.SerializeObject(objResult);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
            return strDetail;
        }
        public ActionResult DownloadShipReportsData(string shipCode, string shipName)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                var export = _helper.GetAllShipReportsData(shipCode, shipName);

                var fileName = "ShipReportsAnalysis.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return RedirectToAction("ShipReportsAnalysis", "Reports");
        }
        #endregion

        #region Fleet Inspection Due Dates
        public ActionResult FleetInspectionDueDates()
        {
            //APIHelper _helper = new APIHelper();
            //List<CSShipsModal> shipsList = _helper.GetAllShips();
            //if (shipsList == null)
            //    shipsList = new List<CSShipsModal>();
            //shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX")).ToList();
            //shipsList = shipsList.OrderBy(x => x.Name).ToList();
            //ViewBag.Ships = shipsList;
            return View();
        }
        public JsonResult GetFleetInspectionDueDates()
        {
            APIHelper _helper = new APIHelper();
            try
            {
                var data = _helper.GetFleetInspectionDueDates();
                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFleetInspectionDueDates :" + ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UpdateFleetInspectionDueDates(FleetInspectionDueDatesModal value)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                var data = _helper.UpdateFleetInspectionDueDates(value);
                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFleetInspectionDueDates :" + ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}