using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    public class PurchasingDeptController : Controller
    {

        List<PurchasingDeptModel> PurchasingDept = new List<PurchasingDeptModel>();
        List<DisplayModalPurchasingDept> orderDeptList = new List<DisplayModalPurchasingDept>();
        // GET: PurchasingDept
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllPurchasingDeptData(string POYear, string POMonth, int? FleetId)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                if (POYear == "All")
                    POYear = "";
                List<PurchasingDeptModel> PurchasingDeptData = new List<PurchasingDeptModel>();
                int _month = FilterMonth(POMonth);
                PurchasingDeptData = _helper.GetAllPurchasingDeptData(POYear, _month, FleetId);
                ViewBag.PurchasingDeptData = PurchasingDeptData;
                var jsonResult = Json(PurchasingDeptData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllPurchasingDeptData :" + ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Download Report in Excel
        public ActionResult DownloadPurchasingDept(string month, string year, int? FleetId)
        {
            try
            {
                List<DisplayModalPurchasingDept> export = new List<DisplayModalPurchasingDept>();
                int _month = FilterMonth(month);
                ParentCodeForPurchasingDept(_month, year, FleetId);
                foreach (var item in PurchasingDept)
                {
                    DisplayModalPurchasingDept obj = new DisplayModalPurchasingDept();
                    obj.ShipName = item.SiteName;
                    obj.PONumber = item.PONO;
                    obj.CompanyCode = item.CmpCode;//String.Format("{0:0.00}", item.Total)
                    obj.VendorName = item.Vendor_Addr_Name;
                    obj.Account_Code = item.Account_Code;
                    obj.Account_Descr = item.Account_Descr;
                    obj.EquipmentName = item.Equip_Name;
                    obj.CodaDocument = item.DocCode;
                    obj.CodaDocumentNumber = item.DocNum;
                    obj.ModDate = item.ModDate;
                    obj.El1 = item.El1;
                    obj.Value = item.ValueDoc;
                    obj.POTotal = item.POTotal;
                    export.Add(obj);
                }

                var fileName = "Purchasing_Dept_report.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Utility.WriteToXLS(export, Response.Output);
                Response.End();
                return RedirectToAction("Index", "PurchasingDept");

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return RedirectToAction("Index", "PurchasingDept");
            }
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

        public List<PurchasingDeptModel> ParentCodeForPurchasingDept(int _month, string year, int? FleetId)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                int POYear = Convert.ToInt32(year);
                List<PurchasingDeptModel> list = _helper.GetAllPurchasingDeptData(year, _month, FleetId);
                orderDeptList = GetDisplayDataListPurchasingDept(list);
                PurchasingDept = orderDeptList
                              .Select(cl => new PurchasingDeptModel
                              {
                                  SiteName = cl.ShipName,
                                  PONO = cl.PONumber,
                                  CmpCode = cl.CompanyCode,
                                  Vendor_Addr_Name = cl.VendorName,
                                  Account_Code = cl.Account_Code,
                                  Account_Descr = cl.Account_Descr,
                                  Equip_Name = cl.EquipmentName,
                                  DocCode = cl.CodaDocument,
                                  DocNum = cl.CodaDocumentNumber,
                                  ModDate = cl.ModDate,
                                  El1 = cl.El1,
                                  ValueDoc = cl.Value,
                                  POTotal = cl.POTotal
                              }).OrderBy(x => x.PODate).ToList();

                ViewBag.PurchasingDeptCode = PurchasingDept.OrderBy(x => x.PODate);
            }
            catch (Exception ex) {
                LogHelper.writelog(ex.Message);
            }
            return PurchasingDept;
        }

        public List<DisplayModalPurchasingDept> GetDisplayDataListPurchasingDept(List<PurchasingDeptModel> list)
        {
            List<DisplayModalPurchasingDept> displayListNew = new List<DisplayModalPurchasingDept>();
            List<DisplayModalPurchasingDept> DispList = new List<DisplayModalPurchasingDept>();
            try
            {
                foreach (var subItem in list)
                {
                    DisplayModalPurchasingDept SubDisp = new DisplayModalPurchasingDept();
                    
                    
                    SubDisp.ShipName = subItem.SiteName;
                    SubDisp.PONumber = subItem.PONO;
                    SubDisp.CompanyCode = subItem.CmpCode;
                    SubDisp.VendorName = subItem.Vendor_Addr_Name;
                    SubDisp.Account_Code = subItem.Account_Code;
                    SubDisp.Account_Descr = subItem.Account_Descr;
                    SubDisp.EquipmentName = subItem.Equip_Name;
                    SubDisp.CodaDocument = subItem.DocCode;
                    SubDisp.CodaDocumentNumber = subItem.DocNum;
                    SubDisp.ModDate = subItem.ModDate;
                    SubDisp.El1 = subItem.El1;
                    SubDisp.Value = subItem.ValueDoc;
                    SubDisp.POTotal = subItem.POTotal;
                   
                    DispList.Add(SubDisp);
                    //int cnt = displayListNew.Where(x => x.PONumber == subItem.PONO).ToList().Count();
                    //if (cnt == 0)
                    //{
                        displayListNew.Add(SubDisp);
                    //}
                    //else
                    //    continue;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return displayListNew;
        }
        #endregion
    }
}