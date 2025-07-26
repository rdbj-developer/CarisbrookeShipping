using Newtonsoft.Json;
using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace OfficeApplication.BLL.Helpers
{
    public class APIHelper
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];

        #region SRMFORM
        public List<SMRModal> GetSMRFormsFilled(SMRFormReq Modal)
        {
            List<SMRModal> response = new List<SMRModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SMRFormsFilled", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SMRModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
            }
            return response;
        }
        public SMRModal GetSMRFormByID(long id)
        {
            SMRModal response = new SMRModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetSMRFormByID/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<SMRModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region GIRForm
        public List<GIRData> GetGIRFormByID()
        {
            List<GIRData> response = new List<GIRData>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetGIRFormsFilled").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GIRData>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }
        public List<GIRDataList> GetDeficienciesData(string id)
        {
            List<GIRDataList> response = new List<GIRDataList>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesData/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GIRDataList>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }
        public bool UpdateDeficienciesData(int id, bool isClose)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/UpdateDeficienciesData?id=" + id + "&isClose=" + isClose).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse AddDeficienciesNote(DeficienciesNote note)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(note), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/AddDeficienciesNote", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public List<DeficienciesNote> GetDeficienciesNote(int id)
        {
            List<DeficienciesNote> response = new List<DeficienciesNote>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesNote/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeficienciesNote>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }
        public List<GIRDeficienciesFile> GetDeficienciesFiles(int id)
        {
            List<GIRDeficienciesFile> response = new List<GIRDeficienciesFile>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesFiles/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GIRDeficienciesFile>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }

        public string GetFile(int id)
        {
            string response = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetFile?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFile Error : " + ex.Message);
            }
            return response;
        }
        public string GetFileComment(int id)
        {
            string response = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetFileComment?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFile Error : " + ex.Message);
            }
            return response;
        }


        public List<GeneralInspectionReport> GetAllGIRForms()
        {
            List<GeneralInspectionReport> response = new List<GeneralInspectionReport>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetAllGIRForms/").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GeneralInspectionReport>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllGIRForms Error : " + ex.Message);
            }
            return response;
        }
        public GeneralInspectionReport GIRFormDetailsView(int id)
        {
            GeneralInspectionReport response = new GeneralInspectionReport();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GIRFormDetailsView/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<GeneralInspectionReport>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllGIRForms Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region SIRForms
        public List<SuperintendedInspectionReport> GetAllSIRForms()
        {
            List<SuperintendedInspectionReport> response = new List<SuperintendedInspectionReport>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetAllSIRForms/").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SuperintendedInspectionReport>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllSIRForms Error : " + ex.Message);
            }
            return response;
        }
        public SIRModal SIRFormDetailsView(int id)
        {
            SIRModal response = new SIRModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/SIRFormDetailsView/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<SIRModal>(resStr);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllGIRForms Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region Documents
        public List<DocumentModal> GetAllDocuments(string sectionType = "")
        {
            List<DocumentModal> response = new List<DocumentModal>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAllDocumentsForOfficeApp?sectionType=" + sectionType).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocuments Error : " + ex.Message);
            }
            return response;
        }
        public List<DocumentModal> GetAllDocumentsWithRAData(string shipCode)
        {
            List<DocumentModal> response = new List<DocumentModal>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAllDocumentsWithRAData?shipCode=" + shipCode).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocuments Error : " + ex.Message);
            }
            return response;
        }
        public DocumentModal GetDocumentBYID(string id)
        {
            DocumentModal response = new DocumentModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetDocumentBYID/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<DocumentModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentBYID Error : " + ex.Message);
            }
            return response;
        }
        public List<RepositoryModal> GetAllRepositories()
        {
            List<RepositoryModal> response = new List<RepositoryModal>();
            string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAllRepositories").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RepositoryModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse AddDocument(DocumentModal Modal)
        {
            APIResponse response = new APIResponse();
            string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Documents/AddDocument", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateDocument(DocumentModal Modal)
        {
            APIResponse response = new APIResponse();
            string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Documents/UpdateDocument", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateDocumentFile(DocumentModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Documents/UpdateDocumentFile", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse DeleteDocument(string id)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/DeleteDocument/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteDocument Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateDocumentFileFolder(DocumentModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Documents/UpdateDocumentFileFolder", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDocumentFileFolder Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region CASHIP_WebGUI

        public List<SMV_BUDGET_OVERVIEW_1> Get_Reports_Data()
        {
            List<SMV_BUDGET_OVERVIEW_1> response = new List<SMV_BUDGET_OVERVIEW_1>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/Get_Reports_Data").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SMV_BUDGET_OVERVIEW_1>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public List<Invoice> Get_Reports_DataInvoice()
        {
            List<Invoice> response = new List<Invoice>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/Get_Reports_DataInvoice").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Invoice>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public List<PurchaseOrder> Get_Reports_DataPurchase()
        {
            List<PurchaseOrder> response = new List<PurchaseOrder>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/Get_Reports_DataPurchase").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PurchaseOrder>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public List<SMV_ACCOUNT_RECONCILATION_RPT> Get_Reports_DataList()
        {
            List<SMV_ACCOUNT_RECONCILATION_RPT> response = new List<SMV_ACCOUNT_RECONCILATION_RPT>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/Get_Reports_DataList").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SMV_ACCOUNT_RECONCILATION_RPT>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public List<AccountCodeData> GetAccountCodeList()
        {
            List<AccountCodeData> response = new List<AccountCodeData>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/GetAccountCodeList").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccountCodeData>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }

        public DateTime GetLastFinilizeReportDate()
        {
            DateTime response = new DateTime();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/GetLastFinilizeReportDate").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLastFinilizeReportDate Error : " + ex.Message);
            }
            return response;
        }
        public UALOpexFormula Get_Reports_UALOpexFormula(DateTime startdate, DateTime enddate)
        {
            UALOpexFormula response = new UALOpexFormula();
            try
            {
                string st = startdate.ToString("ddMMyyyy");
                string ed = enddate.ToString("ddMMyyyy");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/Get_Reports_UALOpexFormula?startdate=" + st + "&enddate=" + ed).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<UALOpexFormula>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public OpexReportFinilizeDate UpdateOpexReportDate(OpexReportFinilizeDate data)
        {
            OpexReportFinilizeDate response = new OpexReportFinilizeDate();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                    HttpResponseMessage resMsg = client.PostAsync("api/Reports/UpdateOpexReportDate", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<OpexReportFinilizeDate>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region Login
        public UserProfile Login(UserProfile User)
        {
            UserProfile response = new UserProfile();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(User), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Login/Login", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public UserRole RolePremission(UserProfile User)
        {
            UserRole response = new UserRole();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(User), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Login/RolePremission", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<UserRole>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region VIMS
        public VIMSLIST GetVIMSData(string pono)
        {
            VIMSLIST response = new VIMSLIST();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/VIMS/GetAllVIMS?id=" + pono).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<VIMSLIST>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetVIMSData Error : " + ex.Message);
            }
            return response;
        }
        public bool UpdateVIMSDate(double INVOICE_EXCHRATE, int REQNINVOICEID, DateTime INVOICE_DATE, double OldINVOICE_EXCHRATE, DateTime OldINVOICE_DATE, string user)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/VIMS/UpdateVIMSDate/?INVOICE_EXCHRATE=" + INVOICE_EXCHRATE + "&REQNINVOICEID=" + REQNINVOICEID + "&INVOICE_DATE=" + INVOICE_DATE.ToString("dd/MM/yyyy") + "&OldINVOICE_EXCHRATE=" + OldINVOICE_EXCHRATE + "&OldINVOICE_DATE=" + OldINVOICE_DATE.ToString("dd/MM/yyyy") + "&user=" + user).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateVIMSDate Error : " + ex.Message);
            }
            return response;
        }
        public bool UpdateVIMSAccountData(string PONO, string OldAccountCode, string NewAccountCode, string user, DateTime PODATE, DateTime OLDPODATE)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/VIMS/UpdateVIMSAccountData/?PONO=" + PONO + "&OldAccountCode=" + OldAccountCode + "&NewAccountCode=" + NewAccountCode + "&user=" + user + "&PODATE=" + PODATE.ToString("dd/MM/yyyy") + "&OLDPODATE=" + OLDPODATE.ToString("dd/MM/yyyy")).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateVIMSAccountData Error : " + ex.Message);
            }
            return response;
        }
        public List<SM_ACCOUNTCODE> GetAccountDetails()
        {
            List<SM_ACCOUNTCODE> response = new List<SM_ACCOUNTCODE>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/VIMS/GetAccountDetails").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SM_ACCOUNTCODE>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateVIMSAccountData Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region IAF
        public List<InternalAuditForm> GetAllInternalAuditForm()
        {
            List<InternalAuditForm> response = new List<InternalAuditForm>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/GetAllInternalAuditForm/").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InternalAuditForm>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllSIRForms Error : " + ex.Message);
            }
            return response;
        }
        public IAF IAFormDetailsView(int id)
        {
            IAF response = new IAF();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/IAFormDetailsView/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<IAF>(resStr);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllGIRForms Error : " + ex.Message);
            }
            return response;
        }
        public Dictionary<string, string> GetNumberForNotes(string ship)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/GetNumberForNotes?ship=" + ship).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNumberForNotes Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region Deficiencies

        public string GetResponse(string apiUrl)
        {
            string resStr = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync(apiUrl).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        resStr = resMsg.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipGISIReports Error : " + ex.Message);
            }
            return resStr;
        }
        public List<Deficiency_GISI_Ships> GetGISIShips()
        {
            List<Deficiency_GISI_Ships> response = new List<Deficiency_GISI_Ships>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Deficiencies/GetGISIShips").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_GISI_Ships>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipGISIReports Error : " + ex.Message);
            }
            return response;
        }
        public List<Deficiency_GISI_Report> GetShipGISIReports(string code)
        {
            List<Deficiency_GISI_Report> response = new List<Deficiency_GISI_Report>();
            string resStr = GetResponse(APIURLHelper.GetShipGISIReports + "/" + code);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_GISI_Report>>(resStr);
            }
            return response;
        }
        public List<GIRDataList> GetGISIDeficiencies(Deficiency_GISI_Report Modal)
        {
            List<GIRDataList> response = new List<GIRDataList>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Deficiencies/GetDeficienciesData", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GIRDataList>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }

        public List<Deficiency_Audit_Ships> GetAuditShips()
        {
            List<Deficiency_Audit_Ships> response = new List<Deficiency_Audit_Ships>();
            string resStr = GetResponse(APIURLHelper.GetAuditShips);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_Audit_Ships>>(resStr);
            }
            return response;
        }
        public List<Deficiency_Ship_Audits> GetShipAudits(string code)
        {
            List<Deficiency_Ship_Audits> response = new List<Deficiency_Ship_Audits>();
            string resStr = GetResponse(APIURLHelper.GetShipAudits + "/" + code);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_Ship_Audits>>(resStr);
            }
            return response;
        }
        public bool UpdateAuditDeficiencies(long id, bool isClose)
        {
            bool response = false;
            string resStr = GetResponse(APIURLHelper.UpdateAuditDeficiencies + "?id=" + id + "&isClose=" + isClose);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
            }
            return response;
        }
        public bool AddAuditDeficiencyComments(Audit_Deficiency_Comments Modal)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync(APIURLHelper.AddAuditDeficiencyComments, content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddAuditDeficiencyComments Error : " + ex.Message);
            }
            return response;
        }
        public List<Audit_Deficiency_Comments> GetAuditDeficiencyComments(long NoteID)
        {
            List<Audit_Deficiency_Comments> response = new List<Audit_Deficiency_Comments>();
            string resStr = GetResponse(APIURLHelper.GetAuditDeficiencyComments + "/" + NoteID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Audit_Deficiency_Comments>>(resStr);
            }
            return response;
        }
        public List<Audit_Deficiency_Comments_Files> GetAuditDeficiencyCommentFiles(long NoteID)
        {
            List<Audit_Deficiency_Comments_Files> response = new List<Audit_Deficiency_Comments_Files>();
            string resStr = GetResponse(APIURLHelper.GetAuditDeficiencyCommentFiles + "/" + NoteID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Audit_Deficiency_Comments_Files>>(resStr);
            }
            return response;
        }
        public string GetAuditCommentFile(long CommentFileID)
        {
            string response = string.Empty;
            string resStr = GetResponse(APIURLHelper.GetFileComment + "/" + CommentFileID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
            }
            return response;
        }
        #endregion

        #region Users

        public List<UserProfile> GetAllUsers()
        {
            List<UserProfile> response = new List<UserProfile>();
            string resStr = GetResponse(APIURLHelper.GetAllUsers);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserProfile>>(resStr);
            }
            return response;
        }
        public bool AddUser(UserProfile User)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(User), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Users/AddUser", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddUser Error : " + ex.Message);
            }
            return response;
        }

        public List<UserGroup> GetAllUserGroups()
        {
            List<UserGroup> response = new List<UserGroup>();
            string resStr = GetResponse(APIURLHelper.GetAllUserGroups);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = JsonConvert.DeserializeObject<List<UserGroup>>(resStr);
            }
            return response;
        }
        public List<Menus> GetUserGroupMenuPermission(int userGRoup)
        {
            List<Menus> response = new List<Menus>();
            string resStr = GetResponse(APIURLHelper.GetUserGroupMenuPermission + "/" + userGRoup);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = JsonConvert.DeserializeObject<List<Menus>>(resStr);
            }
            return response;
        }
        public bool UpdateAWSUserFromSeacrew()
        {
            bool response = false;
            string resStr = GetResponse(APIURLHelper.UpdateLocalDbForUsers);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = JsonConvert.DeserializeObject<bool>(resStr);
            }
            return response;
        }
        #endregion

        #region Forms
        public List<FormModal> GetAllForms()
        {
            List<FormModal> response = new List<FormModal>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetAllForms").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<FormModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllForms Error : " + ex.Message);
            }
            return response;
        }
        public FormModal GetFormBYID(string id)
        {
            FormModal response = new FormModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetFormBYID/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<FormModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFormBYID Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse AddForm(FormModal Modal)
        {
            APIResponse response = new APIResponse();
            string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/AddForm", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddForm Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateForm(FormModal Modal)
        {
            APIResponse response = new APIResponse();
            string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/UpdateForm", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateForm Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateFormFile(FormModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/UpdateFormFile", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFormFile Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse DeleteForm(string id)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/DeleteForm/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteForm Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region Reports
        public List<CSShipsModal> GetAllShipsFromJson()
        {
            List<CSShipsModal> ships = new List<CSShipsModal>();
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "Repository\\Ships.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                    if (!string.IsNullOrEmpty(jsonText))
                    {
                        ships = JsonConvert.DeserializeObject<List<CSShipsModal>>(jsonText);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllShipsFromJson Error : " + ex.Message);
            }
            return ships;
        }
        public List<CSShipsModal> GetAllShips()
        {
            List<CSShipsModal> response = new List<CSShipsModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/IMOS/GetAllShips").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<CSShipsModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllShips Error : " + ex.Message);
            }
            return response;
        }
        public List<ShipReportsAnalysisModal> GetAllShipReportsData(string shipCode, string shipName)
        {
            List<ShipReportsAnalysisModal> response = new List<ShipReportsAnalysisModal>();
            try
            {
                var Modal = new
                {
                    shipCode = shipCode,
                    shipName = shipName
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    //StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Reports/GetAllShipReportsData", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<ShipReportsAnalysisModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllShipReportsData Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region DailyCargoReport
        public DailyCargoReportModal DailyCargoFormDetailsView(int id)
        {
            DailyCargoReportModal response = new DailyCargoReportModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/DailyCargoFormDetailsView/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<DailyCargoReportModal>(resStr);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("DailyCargoFormDetailsView  Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region ArrivalReport
        public ArrivalReportModal ArrivalDetailsView(int id)
        {
            ArrivalReportModal response = new ArrivalReportModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/ArrivalDetailsView/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<ArrivalReportModal>(resStr);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("DailyCargoFormDetailsView  Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region ShipReorts 
        public List<ArrivalReportModal> GetAllArrivalReportData(string ShipName)
        {
            List<ArrivalReportModal> response = new List<ArrivalReportModal>();
            try
            {
                var Modal = new
                {
                    shipName = ShipName
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GetAllArrivalDataReports", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<ArrivalReportModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllArrivalReportData Error : " + ex.Message);
            }
            return response;
        }

        public List<DepartureReportModal> GetAllDepartureReportData(string ShipName)
        {
            List<DepartureReportModal> response = new List<DepartureReportModal>();
            try
            {
                var Modal = new
                {
                    shipName = ShipName
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GetAllDepartureDataReports", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<DepartureReportModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDepartureReportData Error : " + ex.Message);
            }
            return response;
        }

        public List<DailyCargoReportModal> GetAllDailyCargoReportData(string ShipName)
        {
            List<DailyCargoReportModal> response = new List<DailyCargoReportModal>();
            try
            {
                var Modal = new
                {
                    shipName = ShipName
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GetAllDailyCargoDataReports", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<DailyCargoReportModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDailyCargoReportData Error : " + ex.Message);
            }
            return response;
        }

        public List<DailyPositionReportModal> GetAllDailyPositionReportData(string ShipName)
        {
            List<DailyPositionReportModal> response = new List<DailyPositionReportModal>();
            try
            {
                var Modal = new
                {
                    shipName = ShipName
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GetAllDailyPositionDataReports", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<DailyPositionReportModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDailyPositionReportData Error : " + ex.Message);
            }
            return response;
        }

        #endregion

        #region SiteInfo
        public List<ShipWisePCModal> GetAllSiteInfoData()
        {
            List<ShipWisePCModal> response = new List<ShipWisePCModal>();
            try
            {
                var Modal = new
                {
                    shipName = ""
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GetAllSiteInfoDatas", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<ShipWisePCModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllSiteInfoData Error : " + ex.Message);
            }
            return response;
        }

        public APIResponse GetAllDeletePCRecordData(string ShipCode, int Id, string PCName, string PCUniqueId)
        {
            APIResponse response = new APIResponse();
            try
            {
                var Modal = new
                {
                    ShipCode = ShipCode,
                    Id = Id,
                    PCName = PCName,
                    PCUniqueId = PCUniqueId
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GetAllDeletePCRecords", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllSiteInfoData Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateBlockPCRecords(string ShipCode, int Id, string PCName, string PCUniqueId, bool IsBlocked)
        {
            APIResponse response = new APIResponse();
            try
            {
                var Modal = new
                {
                    ShipCode = ShipCode,
                    Id = Id,
                    PCName = PCName,
                    PCUniqueId = PCUniqueId,
                    IsBlocked = IsBlocked
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/UpdateBlockPCRecords", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateBlockPCRecords Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateMainPCRecords(string ShipCode, int Id, string PCName, string PCUniqueId, bool IsMainPC)
        {
            APIResponse response = new APIResponse();
            try
            {
                var Modal = new
                {
                    ShipCode = ShipCode,
                    Id = Id,
                    PCName = PCName,
                    PCUniqueId = PCUniqueId,
                    IsMainPC = IsMainPC
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/UpdateMainPCRecords", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateMainPCRecords Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region RiskAssessment

        public List<RiskAssessmentForm> GetRiskassessmentForm(string code)
        {
            List<RiskAssessmentForm> response = new List<RiskAssessmentForm>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetRiskAssessmentFormList?id=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RiskAssessmentForm>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAFormByID Error : " + ex.Message);
            }
            return response;
        }

        public RiskAssessmentFormModal RAFormDetailsView(string ShipName, int id)
        {
            RiskAssessmentFormModal response = new RiskAssessmentFormModal();
            try
            {
                var Modal = new
                {
                    ShipName = ShipName,
                    id = id
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/RAFormDetailsView", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<RiskAssessmentFormModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllGIRForms Error : " + ex.Message);
            }
            return response;
        }

        public APIResponse SubmitRiskAssessmentForm(RiskAssessmentFormModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitRiskAssessmentForm", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
            }
            return response;
        }

        public bool CheckRAFNumberExistFromData(string ShipName, string RAFNumber)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/CheckRAFNumberExistFromData/ShipName=" + ShipName + "&RAFNumber=" + RAFNumber).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckRAFNumberExistFromData Error : " + ex.Message);
            }
            return response;
        }

        public List<RiskAssessmentForm> GetAllDocumentRiskassessment(string code)
        {
            List<RiskAssessmentForm> response = new List<RiskAssessmentForm>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetAllDocumentRiskassessment?id=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RiskAssessmentForm>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentRiskassessment Error : " + ex.Message);
            }
            return response;
        }

        public bool InsertDocumentsBulkDataInRiskAssessment(List<RiskAssessmentForm> AllDocuments)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(AllDocuments), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/InsertDocumentsBulkDataInRiskAssessment", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssessment Error : " + ex.Message);
            }
            return response;
        }
        public bool InsertDocumentsBulkDataInRiskAssesmentHazared(List<RiskAssessmentFormHazard> AllDocuments)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(AllDocuments), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/InsertDocumentsBulkDataInRiskAssesmentHazared", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssesmentHazared Error : " + ex.Message);
            }
            return response;
        }
        public bool InsertDocumentsBulkDataInRiskAssessmentReviewer(List<RiskAssessmentFormReviewer> AllDocuments)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(AllDocuments), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/InsertDocumentsBulkDataInRiskAssessmentReviewer", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssessmentReviewer Error : " + ex.Message);
            }
            return response;
        }

        public List<RiskAssessmentReviewLog> GetAllRiskAssessmentReviewLog(string shipCode = "")
        {
            List<RiskAssessmentReviewLog> lstRiskAssessmentReviewLog = new List<RiskAssessmentReviewLog>();
            APIResponse response = new APIResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetRiskAssessmentReviewLog?shipCode=" + shipCode).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteDocument Error : " + ex.Message);
            }
            if (!string.IsNullOrEmpty(response.result) && response.msg == AppStatic.SUCCESS)
            {
                lstRiskAssessmentReviewLog = JsonConvert.DeserializeObject<List<RiskAssessmentReviewLog>>(response.result);
            }

            return lstRiskAssessmentReviewLog;
        }

        #endregion

        #region PurchasingDept
        public List<PurchasingDeptModel> GetAllPurchasingDeptData(string POYear, int POMonth, int? FleetId)
        {
            List<PurchasingDeptModel> response = new List<PurchasingDeptModel>();
            try
            {
                var Modal = new
                {
                    POYear = POYear,
                    POMonth = POMonth,
                    FleetId = FleetId
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/POService/GetAllPurchasingDeptDataReports", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<PurchasingDeptModel>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllPurchasingDeptData Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region AssetManagmentEquipmentList
        public APIResponse SubmitAssetManagmentEquipmentList(AssetManagmentEquipmentListModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Documents/SubmitAssetManagmentEquipmentList", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit AssetManagmentEquipmentList Form Error : " + ex.Message);
            }
            return response;
        }
        public AssetManagmentEquipmentListModal GetAssetManagmentEquipmentData(string shipCode)
        {
            AssetManagmentEquipmentListModal response = new AssetManagmentEquipmentListModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAssetManagmentEquipmentData?shipCode=" + shipCode).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<AssetManagmentEquipmentListModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentData Error : " + ex.Message);
            }
            return response;
        }
        public List<string> GetAssetManagmentHardwareId(string shipCode)
        {
            List<string> response = new List<string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAssetManagmentHardwareId?shipCode=" + shipCode).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentData Error : " + ex.Message);
            }
            return response;
        }
        #endregion
        #region CybersecurityRisksAssessment
        public APIResponse SubmitCybersecurityRisksAssessment(CybersecurityRisksAssessmentModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Documents/SubmitCybersecurityRisksAssessment", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit CybersecurityRisksAssessment Form Error : " + ex.Message);
            }
            return response;
        }
        public CybersecurityRisksAssessmentModal GetCybersecurityRisksAssessmentData(string shipCode)
        {
            CybersecurityRisksAssessmentModal response = new CybersecurityRisksAssessmentModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetCybersecurityRisksAssessmentData?shipCode=" + shipCode).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<CybersecurityRisksAssessmentModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCybersecurityRisksAssessmentData Error : " + ex.Message);
            }
            return response;
        }
        public List<string> GetCyberSecurityRiskList()
        {
            List<string> response = new List<string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetCyberSecurityRiskList").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecurityRiskList Error : " + ex.Message);
            }
            return response;
        }

        public List<string> GetCyberSecurityVulnerabilitiesList()
        {
            List<string> response = new List<string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetCyberSecurityVulnerabilitiesList").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecurityVulnerabilitiesList Error : " + ex.Message);
            }
            return response;
        }
        public List<CyberSecuritySettingsModal> GetAllCyberSecuritySettingsList()
        {
            List<CyberSecuritySettingsModal> response = new List<CyberSecuritySettingsModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAllCyberSecuritySettingsList").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<CyberSecuritySettingsModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllCyberSecuritySettingsList Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse CopyCybersecurityRisksAssessment(CyberSecurityCopyDataModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Documents/CopyCybersecurityRisksAssessment", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CopyCybersecurityRisksAssessment Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region Fleet Inspection Due Dates
        public List<FleetInspectionDueDatesModal> GetFleetInspectionDueDates()
        {
            List<FleetInspectionDueDatesModal> response = new List<FleetInspectionDueDatesModal>();
            try
            {
                //var Modal = new
                //{
                //    shipCode = shipCode,
                //    shipName = shipName
                //};
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.GetAsync("api/Reports/GetFleetInspectionDueDates").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<FleetInspectionDueDatesModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFleetInspectionDueDates Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse UpdateFleetInspectionDueDates(FleetInspectionDueDatesModal value)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Reports/UpdateFleetInspectionDueDates", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFleetInspectionDueDates Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region SoftwareAssets
        public List<ShipSystemsSoftwareInfoModal> GetShipSystemsSoftwareAssets(string code)
        {
            List<ShipSystemsSoftwareInfoModal> response = new List<ShipSystemsSoftwareInfoModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/SystemInfo/GetShipSystemsSoftwareAssets?shipCode=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShipSystemsSoftwareInfoModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipSystemsSoftwareAssets Error : " + ex.Message);
            }
            return response;
        }

        #endregion
    }
}
