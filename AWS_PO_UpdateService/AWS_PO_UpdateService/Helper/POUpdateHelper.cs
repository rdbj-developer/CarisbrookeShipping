using AWS_PO_UpdateService.Modals;
using System;
using System.Linq;

namespace AWS_PO_UpdateService.Helper
{
    public class POUpdateHelper
    {
        public void StartPOUpdate()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                //delete
                var DeletecsshipPOList = _helper.DeleteCSShipsPOData();
                var csshipPOList = _helper.GetCSShipsPOData(); //Query1
                var codaFinPOList = _helper.GetCodaFinPOData(); //Query2
                if (codaFinPOList != null && codaFinPOList.Count > 0 && csshipPOList != null)
                {
                    string searchTerm = string.Empty;
                    foreach (var item in codaFinPOList)
                    {
                        searchTerm = GetPONo(item.descr);
                        var objCashipPO = csshipPOList.Where(x => x.PONO == GetPONo(item.descr)).FirstOrDefault();
                        if (objCashipPO == null)
                        {
                            //Check and Remove Existing enrty for this PONo                          
                            _helper.RemoveCodaPurchaseOrder(new CodaPurchaseOrderModal
                            {
                                PONO = searchTerm,
                                Descr = item.descr
                            });
                        }
                        else
                        {
                            ////New entry to table
                            _helper.AddCodaPurchaseOrder(new CodaPurchaseOrderModal
                            {
                                CmpCode = item.cmpcode,
                                Descr = item.descr,
                                DocCode = item.doccode,
                                DocNum = item.docnum,
                                El1 = item.el1,
                                Invoice = item.invoice,
                                ModDate = item.moddate,
                                ValueDoc = item.valuedoc,
                                Account_Code = objCashipPO.ACCOUNT_CODE,
                                Account_Descr = objCashipPO.ACCOUNT_DESCR,
                                Curr_Code = objCashipPO.CURR_CODE,
                                Dept_Code = objCashipPO.DEPT_CODE,
                                Equip_Name = objCashipPO.EQUIP_NAME,
                                Forwarder_Recvd_Date = objCashipPO.FORWARDER_RECVD_DATE,
                                InvoiceNo = objCashipPO.INVOICE_PRESENT,
                                PODate = objCashipPO.PODATE,
                                POExchRate = objCashipPO.POEXCHRATE,
                                PONO = objCashipPO.PONO,//objCashipPO.PONO,
                                PORecVDate = objCashipPO.PORECVDATE,
                                POTotal = objCashipPO.POTOTAL,
                                POTotal_Base = objCashipPO.POTOTAL_BASE,
                                SiteName = objCashipPO.SITENAME,
                                Vendor_Addr_Name = objCashipPO.VENDOR_ADDR_NAME
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StartPOUpdate Error : " + ex.Message);
            }
        }

        public string GetPONo(string searchTerm)
        {
            bool isValidPONoToCheck = false;
            try
            {
                if (searchTerm.Contains("/"))
                {
                    var arr = searchTerm.Split(new[] { '/' });
                    if (arr != null && arr.Length > 0)
                    {
                        for (int i = 0; i < arr.Length; i++)
                        {
                            searchTerm = arr[i];
                            if (searchTerm.isValidPONo())
                            {
                                isValidPONoToCheck = true;
                                break;
                            }
                        }
                    }
                }
                else if (searchTerm.isValidPONo())
                    isValidPONoToCheck = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetPONo Error : " + ex.Message);
            }
            if (isValidPONoToCheck)
                return searchTerm;
            else
                return "";
        }
    }
}
