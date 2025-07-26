using Newtonsoft.Json;
using ShipApplication.BLL.Modals;
using ShipApplication.BLL.Resources.Constant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ShipApplication.BLL.Helpers
{
    public class APIHelper
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"].ToString();
        public APIResponse SubmitSMR(SMRModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitSMRForm", content).Result;
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
        public APIResponse SubmitGIR(GeneralInspectionReport Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitGIRForm", content).Result;
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
        public int getNextNumber(string ship)
        {
            int response = 1;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/DeficienciesNumber?ship=" + ship).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
            }
            return response;
        }

        public GeneralInspectionReport GIRGetGeneralDescription(string shipCode)
        {
            GeneralInspectionReport response = new GeneralInspectionReport();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IMOS/GIRGetGeneralDescription?shipCode=" + shipCode).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<GeneralInspectionReport>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRGetGeneralDescription Error : " + ex.Message);
            }
            return response;
        }

        public APIResponse SubmitArrivalReport(ArrivalReportModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitArrivalReport", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit Arrival Report Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse SubmitDepartureReport(DepartureReportModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitDepartureReport", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit Departure Report Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse SubmitDailyCargoReport(DailyCargoReportModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitDailyCargoReport", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit Cargo Report Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse SubmitDailyPositionReport(DailyPositionReportModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitDailyPositionReport", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit Daily Position Report Error : " + ex.Message);
            }
            return response;
        }
        public List<DocumentModal> GetAllDocuments(string shipCode)
        {
            List<DocumentModal> response = new List<DocumentModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAllDocumentsForShip?shipCode=" + shipCode).Result;
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
        public List<DocumentModal> GetAllDocumentsForService()
        {
            List<DocumentModal> response = new List<DocumentModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Documents/GetAllDocumentsForService").Result;
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public List<CSShipsModal> GetAllShips()
        {
            List<CSShipsModal> response = new List<CSShipsModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/IMOS/GetAllShips").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CSShipsModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocuments Error : " + ex.Message);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public APIResponse AddDeficienciesNote(GIRDeficienciesNote note)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public List<GIRDeficienciesNote> GetDeficienciesNote(int id)
        {
            List<GIRDeficienciesNote> response = new List<GIRDeficienciesNote>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesNote/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<GIRDeficienciesNote>>(resStr);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesFiles/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<GIRDeficienciesFile>>(resStr);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public string GetGIRDeficienciesInitialActionFile(int id)
        {
            string response = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetGIRDeficienciesInitialActionFile?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesInitialActionFile Error : " + ex.Message);
            }
            return response;
        }
        public string GetGIRDeficienciesResolutionFile(int id)
        {
            string response = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetGIRDeficienciesResolutionFile?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesResolutionFile Error : " + ex.Message);
            }
            return response;
        }
        public long GIRAutoSave(GeneralInspectionReport Modal)
        {
            long response = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GIRAutoSave", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<long>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
            }
            return response;
        }
        public long GIRSaveFormDarf(GeneralInspectionReport Modal)
        {
            long response = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GIRSaveFormDarf", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<long>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
            }
            return response;
        }

        public List<GIRData> GetGIRDrafts(string code)
        {
            List<GIRData> response = new List<GIRData>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Drafts/GetGIRDrafts?id=" + code).Result;
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
        public GeneralInspectionReport GIRFormDetailsView(int id)
        {
            GeneralInspectionReport response = new GeneralInspectionReport();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public APIResponse AddGIRDeficiencies(GIRDeficiencies Modal)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/AddGIRDeficiencies", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficiencies: Submit GIR Deficiencies Error : " + ex.Message);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public GeneralInspectionReport GIRFormGetDeficiency(int id)
        {
            GeneralInspectionReport response = new GeneralInspectionReport();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GIRFormGetDeficiency/" + id).Result;
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
        public APIResponse AddDeficienciesInitialActions(GIRDeficienciesInitialActions model)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/AddDeficienciesInitialActions", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesInitialActions Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse AddDeficienciesResolution(GIRDeficienciesResolution model)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/AddDeficienciesResolution", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesResolution Error : " + ex.Message);
            }
            return response;
        }
        public List<GIRDeficienciesInitialActions> GetDeficienciesInitialActions(int id)
        {
            List<GIRDeficienciesInitialActions> response = new List<GIRDeficienciesInitialActions>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesInitialActions/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<GIRDeficienciesInitialActions>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesInitialActions Error : " + ex.Message);
            }
            return response;
        }
        public List<GIRDeficienciesResolution> GetDeficienciesResolution(int id)
        {
            List<GIRDeficienciesResolution> response = new List<GIRDeficienciesResolution>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesResolution/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<GIRDeficienciesResolution>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesResolution Error : " + ex.Message);
            }
            return response;
        }
        #region SIRForms
        public APIResponse SubmitSIR(SIRModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitSIRForm", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit SIR Form Error : " + ex.Message);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
                LogHelper.writelog("SIRFormDetailsView Error : " + ex.Message);
            }
            return response;
        }
        public long SIRAutoSave(SIRModal Modal)
        {
            long response = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SIRAutoSave", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<long>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRAutoSave Error : " + ex.Message);
            }
            return response;
        }
        public List<SIRData> GetSIRDrafts(string code)
        {
            List<SIRData> response = new List<SIRData>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Drafts/GetSIRDrafts?id=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SIRData>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRDrafts Error : " + ex.Message);
            }
            return response;
        }

        public APIResponse AddSIRDeficiencies(GIRDeficiencies Modal)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/AddSIRDeficiencies", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddSIRDeficiencies: Submit GIR Deficiencies Error : " + ex.Message);
            }
            return response;
        }

        public SIRModal SIRFormGetDeficiency(int id,string type)
        {
            SIRModal response = new SIRModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/SIRFormGetDeficiency/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<SIRModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRFormGetDeficiency Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region IAF
        public APIResponse SubmitIAForm(IAF Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/IAF/SubmitIAForm", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit IAForm Error : " + ex.Message);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
                LogHelper.writelog("Submit SMR Form Error : " + ex.Message);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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

        public AuditNote GetAuditNotesById(int id)
        {
            AuditNote response = new AuditNote();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/GetAuditNotesById/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<AuditNote>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }

        public List<Audit_Note_Resolutions> GetAuditNoteResolutions(long NoteID)
        {
            List<Audit_Note_Resolutions> response = new List<Audit_Note_Resolutions>();
            string resStr = GetResponse("api/IAF/GetAuditNoteResolutions/" + NoteID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Audit_Note_Resolutions>>(resStr);
            }
            return response;
        }

        public bool AddAuditNoteResolutions(Audit_Note_Resolutions Modal)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync(APIURLHelper.AddAuditNoteResolutions, content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddAuditNoteResolutions Error : " + ex.Message);
            }
            return response;
        }

        public string DownloadAuditFile(long FileID)
        {
            string response = string.Empty;
            string resStr = GetResponse(APIURLHelper.GetAuditFile + "/" + FileID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
            }
            return response;
        }

        public string GetAuditNoteResolutionFile(long ResolutionFileID)
        {
            string response = string.Empty;
            string resStr = GetResponse(APIURLHelper.GetFileAuditNoteResolution + "/" + ResolutionFileID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public List<FormModal> GetAllFormsForService()
        {
            List<FormModal> response = new List<FormModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetAllFormsForService").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<FormModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllFormsForService Error : " + ex.Message);
            }
            return response;
        }

        public bool DeleteGISIDrafts(int GISIFormID, string type)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/DeleteGISIDrafts?GISIFormID=" + GISIFormID + "&type=" + type).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllForms Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region HoldVentilationRecord
        public APIResponse SubmitHoldVentilationRecordForm(HoldVentilationRecordFormModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitHoldVentilationRecordForm", content).Result;
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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

        public RiskAssessmentFormModal RAFormDetailsView(string ShipName ,int id)
        {
            RiskAssessmentFormModal response = new RiskAssessmentFormModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/RAFormDetailsView/id=" + id + "&ShipName=" + ShipName).Result;
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

        public APIResponse SubmitRiskAssessmentForm(RiskAssessmentFormModal Modal) {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public RiskAssessmentDataList GetRiskAssessmentDataToSeupShipApp(string code)
        {
            RiskAssessmentDataList response = new RiskAssessmentDataList();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetRiskAssessmentDataToSeupShipApp?id=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<RiskAssessmentDataList>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRiskAssessmentDataToSeupShipApp Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region Feedback
        public APIResponse SubmitFeedbackForm(FeedbackFormModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SubmitFeedbackForm", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit Feedback Form Error : " + ex.Message);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
                LogHelper.writelog("Submit SubmitCybersecurityRisksAssessment Form Error : " + ex.Message);
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

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
        public List<string> GetAssetManagmentHardwareId(string shipCode)
        {
            List<string> response = new List<string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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

        public string GetResponse(string apiUrl)
        {
            string resStr = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

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
    }
}
