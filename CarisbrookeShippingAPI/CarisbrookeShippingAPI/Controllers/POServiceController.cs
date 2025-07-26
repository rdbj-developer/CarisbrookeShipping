using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    //[Authorize]   // JSL 10/03/2022 // JSL 09/26/2022
    public class POServiceController : ApiController
    {
        [HttpGet]
        public List<CSShipsPOModal> GetCSShipsPOData()
        {
            List<CSShipsPOModal> data = new List<CSShipsPOModal>();
            try
            {
                POServiceHelper _helper = new POServiceHelper();
                data = _helper.GetCSShipsPOData();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCSShipsPOData Exception : " + ex.Message);
                LogHelper.writelog("GetCSShipsPOData Inner Exception : " + ex.InnerException);
            }
            return data;
        }

        [HttpGet]
        public List<CodafinPOModal> GetCodaFinPOData()
        {
            List<CodafinPOModal> data = new List<CodafinPOModal>();
            try
            {
                POServiceHelper _helper = new POServiceHelper();
                data = _helper.GetCodaFinPOData();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCodaFinPOData Exception : " + ex.Message);
                LogHelper.writelog("GetCodaFinPOData Inner Exception : " + ex.InnerException);
            }
            return data;
        }

        [HttpPost]
        public APIResponse AddCodaPurchaseOrder([FromBody] CodaPurchaseOrder value)
        {
            APIResponse res = new APIResponse();
            try
            {
                POServiceHelper _helper = new POServiceHelper();
                var result = _helper.AddCodaPurchaseOrder(value);
                res.result = result ? AppStatic.SUCCESS : AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddCodaPurchaseOrder Exception : " + ex.Message);
                LogHelper.writelog("AddCodaPurchaseOrder Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        public APIResponse RemoveCodaPurchaseOrder([FromBody] CodaPurchaseOrder value)
        {
            APIResponse res = new APIResponse();
            try
            {
                POServiceHelper _helper = new POServiceHelper();
                var result = _helper.RemoveCodaPurchaseOrder(value);
                res.result = result ? AppStatic.SUCCESS : AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RemoveCodaPurchaseOrder Exception : " + ex.Message);
                LogHelper.writelog("RemoveCodaPurchaseOrder Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public APIResponse DeleteCSShipsPOData()
        {
            APIResponse res = new APIResponse();
            try
            {
                POServiceHelper _helper = new POServiceHelper();
                _helper.DeleteCSShippsPOData();
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeletCSShippsPOData Exception : " + ex.Message);
                LogHelper.writelog("DeletCSShippsPOData Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #region PurchasingDept report
        [HttpPost]
        public List<PurchasingDeptModel> GetAllPurchasingDeptDataReports(PurchasingDeptModel Modal)
        {
            List<PurchasingDeptModel> res = new List<PurchasingDeptModel>();
            try
            {
                POServiceHelper _helper = new POServiceHelper();
                res = _helper.GetAllPurchasingDeptDataReports(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllPurchasingDeptDataReports Exception : " + ex.Message);
                LogHelper.writelog("GetAllPurchasingDeptDataReports Exception : " + ex.InnerException);
            }
            return res;
        }
        #endregion
        #region PurchaseOrderSearch Service
        [HttpGet]
        [Route("PurchaseOrderSearch/{poNumber}/{appendCharacter?}")]
        public HttpResponseMessage PurchaseOrderSearch(string poNumber, string appendCharacter = null)
        {
            try
            {
                string PONumber = string.Empty;
                
                if (!string.IsNullOrWhiteSpace(appendCharacter))
                {
                    PONumber = poNumber + "_" + appendCharacter;                   
                }
                else
                {
                    PONumber = poNumber;
                }

                if (!string.IsNullOrWhiteSpace(poNumber))
                {
                    DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Convert.ToString(ConfigurationManager.AppSettings["PurchaseOrderPDFPath"]));
                    //FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + PONumber + "*.*")   // JSL 09/12/2022 commented this line

                    // JSL 09/12/2022
                    FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + PONumber + "*.*")
                        .OrderByDescending(fi => fi.CreationTime).ToArray();
                    // End JSL 09/12/2022

                    if (filesInDir != null && filesInDir.Length > 0)
                    {
                        string fullName = "";
                        string fileName = "";
                        string name = "";
                        bool isPDF = false;
                        //FileInfo objFile;
                        foreach (FileInfo foundFile in filesInDir)
                        {
                            var test = GetPONo(foundFile.Name.Split('.')[0].Replace("_", "/"));
                            if (test == PONumber)
                            {
                                var objFile = foundFile;
                                fullName = foundFile.FullName;
                                fileName = objFile.FullName;
                                name = objFile.Name;
                                if (objFile.Extension.ToLower().Contains("pdf"))
                                    isPDF = true;
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(fullName))
                        {
                            var objFile = filesInDir[0];
                            fileName = objFile.FullName;
                            name = objFile.Name;
                            if (objFile.Extension.ToLower().Contains("pdf"))
                                isPDF = true;
                        }

                        //var objFile = filesInDir[0];
                        //fileName = objFile.FullName;
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        FileStream fileStream = File.OpenRead(fileName);
                        response.Content = new StreamContent(fileStream);
                        if (isPDF)
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                        else
                        {
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/force-download");
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = name
                            };
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("PurchaseOrderSearch Exception : " + ex.Message);
                LogHelper.writelog("PurchaseOrderSearch Inner Exception : " + ex.InnerException);
            }
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
            responseMessage.Content = new StringContent("The file for which you requested is not found.");
            return responseMessage;
        }
        public string GetPONo(string searchTerm)
        {
            string poNumber = "";
            int j = 0;
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
                                j++;
                                isValidPONoToCheck = true;
                                poNumber = arr[i];
                                if (arr[i + 1].Length == 1 && arr[i+1].ToUpper() != "P")
                                {
                                    poNumber = poNumber + "_" + arr[i + 1];
                                }
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
                return poNumber;
            else
                return "";
        }

        #endregion
    }
}
