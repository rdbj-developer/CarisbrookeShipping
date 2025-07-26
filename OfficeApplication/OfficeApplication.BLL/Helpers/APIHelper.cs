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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public bool UpdateDeficienciesData(string id, bool isClose)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("UpdateDeficienciesData Error : " + ex.Message);
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public APIResponse AddDeficienciesInitialActions(GIRDeficienciesInitialActions model)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public List<GIRDeficienciesInitialActions> GetDeficienciesInitialActions(Guid id)
        {
            List<GIRDeficienciesInitialActions> response = new List<GIRDeficienciesInitialActions>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public List<GIRDeficienciesResolution> GetDeficienciesResolution(Guid id)
        {
            List<GIRDeficienciesResolution> response = new List<GIRDeficienciesResolution>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public List<DeficienciesNote> GetDeficienciesNote(Guid id)
        {
            List<DeficienciesNote> response = new List<DeficienciesNote>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public GIRDeficiencies GetDeficienciesById(Guid id)
        {
            GIRDeficiencies response = new GIRDeficiencies();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesById/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<GIRDeficiencies>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }

        public Dictionary<string, string> GetFile(int id) // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> response = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetFile?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr); // RDBJ 01/27/2022 set with dictionary
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public Dictionary<string, string> GetCommentFile(string id) // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> response = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetCommentFile?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr); // RDBJ 01/27/2022 set with dictionary
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCommentFile Error : " + ex.Message);
            }
            return response;
        }

        public Dictionary<string, string> GetGIRDeficienciesInitialActionFile(string id) // RDBJ 01/27/2022 set with dictionary //RDBJ 11/10/2021 changed datatype from int to string
        {
            Dictionary<string, string> response = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetGIRDeficienciesInitialActionFile?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr); // RDBJ 01/27/2022 set with dictionary
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesInitialActionFile Error : " + ex.Message);
            }
            return response;
        }
        public Dictionary<string, string> GetGIRDeficienciesResolutionFile(string id) // RDBJ 01/27/2022 set with dictionary //RDBJ 11/10/2021 changed datatype from int to string
        {
            Dictionary<string, string> response = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetGIRDeficienciesResolutionFile?id=" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr); // RDBJ 01/27/2022 set with dictionary
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesResolutionFile Error : " + ex.Message);
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public GeneralInspectionReport GIRFormDetailsViewByGUID(string id)
        {
            GeneralInspectionReport response = new GeneralInspectionReport();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GIRFormDetailsViewByGUID/" + id).Result;
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

        public CSShipsModal GIRGetGeneralDescription(string shipCode) //RDBJ 10/07/2021 set CSShipsModal from GeneralInspectionReport
        {
            CSShipsModal response = new CSShipsModal(); //RDBJ 10/07/2021 set CSShipsModal from GeneralInspectionReport
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IMOS/GIRGetGeneralDescription?shipCode=" + shipCode).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<CSShipsModal>(resStr); //RDBJ 10/07/2021 set CSShipsModal from GeneralInspectionReport
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRGetGeneralDescription Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region GIR
        public APIResponse SubmitGIR(GeneralInspectionReport Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("SubmitGIR Form Error : " + ex.Message);
            }
            return response;
        }
        public string GIRAutoSave(GeneralInspectionReport Modal)
        {
            string response = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GIRAutoSave", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRAutoSave Error : " + ex.Message);
            }
            return response;
        }

        //RDBJ 10/07/2021
        public bool GIRShipGeneralDescriptionSave(CSShipsModal Modal)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/GIRShipGeneralDescriptionSave", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRShipGeneralDescriptionSave Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 10/07/2021
        public long GIRSaveFormDarf(GeneralInspectionReport Modal)
        {
            long response = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("GIRSaveFormDarf Error : " + ex.Message);
            }
            return response;
        }

        #endregion

        #region Drafts
        public GeneralInspectionReport GIRFormGetDeficiency(string id)
        {
            GeneralInspectionReport response = new GeneralInspectionReport();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("GIRFormGetDeficiency Error : " + ex.Message);
            }
            return response;
        }

        // RDBJ 12/03/2021
        public List<GIRData> GetGIRDrafts(string code)
        {
            List<GIRData> response = new List<GIRData>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Drafts/GetGIRDrafts/?id=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GIRData>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDrafts Error : " + ex.Message);
            }
            return response;
        }
        // End RDBJ 12/03/2021

        // RDBJ 12/03/2021
        public List<SIRData> GetSIRDrafts(string code)
        {
            List<SIRData> response = new List<SIRData>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Drafts/GetSIRDrafts/?id=" + code).Result;
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
        // End RDBJ 12/03/2021

        // RDBJ 01/23/2022
        public List<AuditList> GetIARDrafts(string code)
        {
            List<AuditList> response = new List<AuditList>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Drafts/GetIARDrafts/?id=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AuditList>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetIARDrafts Error : " + ex.Message);
            }
            return response;
        }
        // End RDBJ 01/23/2022

        // RDBJ 12/03/2021
        public bool DeleteGISIIADrafts(string GISIFormID, string type)
        {
            bool response = false;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Drafts/DeleteGISIIADrafts/?GISIFormID=" + GISIFormID + "&type=" + type).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteGISIIADrafts Error : " + ex.Message);
            }
            return response;
        }
        // End RDBJ 12/03/2021
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public SIRModal SIRFormDetailsView(string uniqueFormid) //RDBJ 09/24/2021 change int id to string uniqueFormid
        {
            SIRModal response = new SIRModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/SIRFormDetailsView/" + uniqueFormid).Result; //RDBJ 09/24/2021 change id to uniqueFormid
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
        public APIResponse SubmitSIR(SIRModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public string SIRAutoSave(SIRModal Modal)
        {
            string response = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Forms/SIRAutoSave", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRAutoSave Error : " + ex.Message);
            }
            return response;
        }

        public SuperintendedInspectionReport SIRFormGetDeficiency(string id) //RDBJ 09/24/2021 change int to string
        {
            SuperintendedInspectionReport response = new SuperintendedInspectionReport();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/SIRFormGetDeficiency/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<SuperintendedInspectionReport>(resStr);
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                    
                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        //RDBJ 11/19/2021
        public APIResponse IAFAutoSave(IAF Modal)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/IAF/IAFAutoSave", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAutoSave Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/19/2021

        //RDBJ 11/23/2021
        public APIResponse SubmitIAForm(IAF Modal)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/IAF/SubmitIAForm", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitIAForm Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/23/2021

        public List<InternalAuditForm> GetAllInternalAuditForm()
        {
            List<InternalAuditForm> response = new List<InternalAuditForm>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("GetAllInternalAuditForm Error : " + ex.Message);
            }
            return response;
        }
        public IAF IAFormDetailsView(Guid? id)
        {
            IAF response = new IAF();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("IAFormDetailsView Error : " + ex.Message);
            }
            return response;
        }

        public AuditNote GetAuditNotesById(Guid? id)
        {
            AuditNote response = new AuditNote();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("GetAuditNotesById Error : " + ex.Message);
            }
            return response;
        }

        public Dictionary<string, string> GetNumberForNotes(string ship
            , string UniqueFormID   // RDBJ 01/22/2022
            )
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/GetNumberForNotes?ship=" + ship + "&UniqueFormID=" + UniqueFormID).Result; // RDBJ 01/22/2022 added UniqueFormID
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
        
        //RDBJ 11/13/2021
        public void UpdateIAFAuditNotePriority(string NotesUniqueID, int PriorityWeek)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/UpdateIAFAuditNotePriority?NotesUniqueID=" + NotesUniqueID + "&PriorityWeek=" + PriorityWeek).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateIAFAuditNotePriority Error : " + ex.Message);
            }
        }
        //End RDBJ 11/13/2021

        //RDBJ 11/17/2021
        public APIResponse AddIAFAuditNote(AuditNote Modal)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/IAF/AddIAFAuditNote", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddIAFAuditNote Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/17/2021

        //RDBJ 11/24/2021
        public bool UpdateAdditionalAndCloseStatus(string id, bool IsAdditionalAndClosedStatus, bool IsAdditionalAndClosed)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/UpdateAdditionalAndCloseStatus?id=" + id + "&IsAdditionalAndClosedStatus=" + IsAdditionalAndClosedStatus + "&IsAdditionalAndClosed=" + IsAdditionalAndClosed).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateAdditionalAndCloseStatus Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/24/2021

        //RDBJ 11/24/2021
        public bool RemoveAuditsOrAuditNotes(string id
            , bool IsAudit  //RDBJ 11/25/2021
            )
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/RemoveAuditsOrAuditNotes?id=" + id + "&IsAudit=" + IsAudit).Result; //RDBJ 11/25/2021 Added IsAudit
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RemoveAuditsOrAuditNotes Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/24/2021

        //RDBJ 11/29/2021
        public List<SMSReferencesTree> GetSMSReferenceData()
        {
            List<SMSReferencesTree> response = new List<SMSReferencesTree>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/GetSMSReferenceData").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SMSReferencesTree>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMSReferenceData Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/29/2021

        //RDBJ 11/29/2021
        public List<SSPReferenceTree> GetSSPReferenceData()
        {
            List<SSPReferenceTree> response = new List<SSPReferenceTree>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/GetSSPReferenceData").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SSPReferenceTree>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSSPReferenceData Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/29/2021

        //RDBJ 11/29/2021
        public List<MLCRegulationTree> GetMLCRegulationTree()
        {
            List<MLCRegulationTree> response = new List<MLCRegulationTree>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/IAF/GetMLCRegulationTree").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MLCRegulationTree>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetMLCRegulationTree Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 11/29/2021
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public List<Deficiency_GISI_Report> GetShipGISIReports(string code, string type)
        {
            List<Deficiency_GISI_Report> response = new List<Deficiency_GISI_Report>();
            string resStr = GetResponse(APIURLHelper.GetShipGISIReports + "?id=" + code + "&type=" + type);
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public int DeleteGIRDeficiencies(string UniqueFormID, string ReportType, string defID) //RDBJ 11/02/2021 Added defID
        {
            int response = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP POST
                    HttpResponseMessage resMsg = client.GetAsync("api/Deficiencies/DeleteGIRDeficiencies?UniqueFormID=" + UniqueFormID + "&ReportType=" + ReportType + "&defID=" + defID).Result; //RDBJ 11/02/2021 Added defID
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public List<Deficiency_Audit_Ships> GetAuditShips(string shipCode)
        {
            List<Deficiency_Audit_Ships> response = new List<Deficiency_Audit_Ships>();
            string resStr = GetResponse(APIURLHelper.GetAuditShips + "/" + shipCode);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_Audit_Ships>>(resStr);
            }
            return response;
        }
        public List<Deficiency_Ship_Audits> GetShipAudits(string code
            , bool blnIsAddedNewAudit   // JSL 04/20/2022
            )
        {
            List<Deficiency_Ship_Audits> response = new List<Deficiency_Ship_Audits>();
            string resStr = GetResponse(APIURLHelper.GetShipAudits + "?code=" + code + "&blnIsAddedNewAudit=" + blnIsAddedNewAudit);    // JSL 04/20/2022 Added blnIsAddedNewAudit
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_Ship_Audits>>(resStr);
            }
            return response;
        }

        // JSL 02/11/2023
        public List<FSTOInspection> GetFSTOAuditDataByShipCode(string code)
        {
            List<FSTOInspection> response = new List<FSTOInspection>();
            string resStr = GetResponse(APIURLHelper.GetFSTOAuditDataByShipCode + "?code=" + code);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FSTOInspection>>(resStr);
            }
            return response;
        }
        // End JSL 02/11/2023

        // JSL 02/17/2023
        public Dictionary<string, string> GetFSTOFile(string fileId)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            string resStr = GetResponse(APIURLHelper.GetFSTOFile + "/" + fileId);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr);
            }
            return response;
        }
        // End JSL 02/17/2023

        public bool UpdateAuditDeficiencies(string id, bool isClose)
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public List<Audit_Deficiency_Comments> GetAuditDeficiencyComments(Guid? NotesUniqueID)
        {
            List<Audit_Deficiency_Comments> response = new List<Audit_Deficiency_Comments>();
            string resStr = GetResponse(APIURLHelper.GetAuditDeficiencyComments + "/" + NotesUniqueID);
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

        public Dictionary<string, string> DownloadAuditFile(string FileID) // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> response = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            string resStr = GetResponse(APIURLHelper.GetAuditFile + "/" + FileID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr); // RDBJ 01/27/2022 set with dictionary
            }
            return response;
        }

        public Dictionary<string, string> GetAuditCommentFile(string CommentFileID) // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> response = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            string resStr = GetResponse(APIURLHelper.GetFileComment + "/" + CommentFileID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr); // RDBJ 01/27/2022 set with dictionary
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public List<Audit_Note_Resolutions> GetAuditNoteResolutions(Guid NoteUniqueID)
        {
            List<Audit_Note_Resolutions> response = new List<Audit_Note_Resolutions>();
            string resStr = GetResponse(APIURLHelper.GetAuditNoteResolutions + "/" + NoteUniqueID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Audit_Note_Resolutions>>(resStr);
            }
            return response;
        }

        public Dictionary<string, string> GetAuditNoteResolutionFile(string ResolutionFileID) // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> response = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            string resStr = GetResponse(APIURLHelper.GetFileAuditNoteResolution + "/" + ResolutionFileID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr); // RDBJ 01/27/2022 set with dictionary
            }
            return response;
        }

        public int getNextNumber(string ship, string reportType, string UniqueFormID) //RDBJ 11/02/2021 Added UniqueFormID //RDBJ 09/24/2021 Added reportType //RDBJ 09/18/2021 removed 2 parameters Guid? UniqueFormID, string Section
        {
            int response = 1;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //HttpResponseMessage resMsg = client.GetAsync("api/Forms/DeficienciesNumber?ship=" + ship + "&UniqueFormID="+ UniqueFormID + "&Section="+ Section).Result; //RDBJ 09/18/2021 Commented
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/DeficienciesNumber?ship=" + ship + "&reportType=" + reportType + "&UniqueFormID=" + UniqueFormID).Result; //RDBJ 11/02/2021 Added UniqueFormID
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("getNextNumber Error : " + ex.Message); // RDBJ 09/18/2021 update from this Submit SMR Form to getNextNumber
            }
            return response;
        }

        //RDBJ 11/02/2021
        public List<int> getDeficienciesDeletedNumbers(string ship, string reportType, string UniqueFormID) //RDBJ 09/24/2021 Added reportType //RDBJ 09/18/2021 removed 2 parameters Guid? UniqueFormID, string Section
        {
            List<int> response = new List<int>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Deficiencies/getDeficienciesDeletedNumbers?ship=" + ship + "&reportType=" + reportType + "&UniqueFormID=" + UniqueFormID).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("getDeficienciesDeletedNumbers Error : " + ex.Message); // RDBJ 09/18/2021 update from this Submit SMR Form to getNextNumber
            }
            return response;
        }
        // End RDBJ 11/02/2021

        //RDBJ 11/02/2021
        public void UpdateDeficiencyPriority(string DeficienciesUniqueID, int PriorityWeek
            , string DueDate    // Ankit 02/28/2022
            )
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Deficiencies/UpdateDeficiencyPriority?DeficienciesUniqueID=" + DeficienciesUniqueID + "&PriorityWeek=" + PriorityWeek + "&DueDate=" + DueDate).Result;    // RDBJ 02/28/2022 Added DueDate
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficiencyPriority Error : " + ex.Message);
            }
        }
        //End RDBJ 11/02/2021

        // RDBJ 12/17/2021
        public void UpdateDeficiencyAssignToUser(string DeficienciesUniqueID, string AssignTo
            , string blnIsIAF   // RDBJ 12/21/2021
            , string blnIsNeedToDelete   // JSL 07/02/2022
            )
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/Deficiencies/UpdateDeficiencyAssignToUser?DeficienciesUniqueID=" + DeficienciesUniqueID + "&AssignTo=" + AssignTo + "&blnIsIAF=" + blnIsIAF + "&blnIsNeedToDelete=" + blnIsNeedToDelete).Result;  // JSL 07/02/2022 added blnIsNeedToDelete // RDBJ 12/21/2021 Append parameter blnIsIAF
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficiencyAssignToUser Error : " + ex.Message);
            }
        }
        //End RDBJ 12/17/2021

        // RDBJ 02/17/2022
        public Dictionary<string, string> CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(dicMetadata), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync(APIURLHelper.APIDeficiencies + "/CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        dicRetMetadata = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu Error : " + ex.Message);
            }
            return dicRetMetadata;
        }
        // End RDBJ 02/17/2022

        // RDBJ 02/18/2022
        public Dictionary<string, string> UpdateCorrectiveAction(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(dicMetadata), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync(APIURLHelper.APIDeficiencies + "/UpdateCorrectiveAction", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        dicRetMetadata = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCorrectiveAction Error : " + ex.Message);
            }
            return dicRetMetadata;
        }
        // End RDBJ 02/18/2022
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());

                    // End JSL 09/26/2022
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
        public APIResponse AddGIRDeficiencies(GIRDeficiencies Modal)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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
                LogHelper.writelog("GetAllDocuments Error : " + ex.Message);
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        //public RiskAssessmentFormModal RAFormDetailsView(string ShipName, int id) // JSL 11/20/2022 commented this line
        public RiskAssessmentFormModal RAFormDetailsView(string ShipName, string id)    // JSL 11/20/2022
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        public bool CheckRAFNumberExistFromData(string RAFNumber, string ShipName)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/CheckRAFNumberExistFromData?ShipName=" + ShipName + "&RAFNumber=" + RAFNumber).Result;
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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

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

        #region SoftwareAssetsJSL
        public List<ShipSystemsSoftwareInfoModal> GetShipSystemsSoftwareAssetsJSL(string code)
        {
            List<ShipSystemsSoftwareInfoModal> response = new List<ShipSystemsSoftwareInfoModal>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/SystemInfo/GetShipSystemsSoftwareAssetsJSL?shipCode=" + code).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShipSystemsSoftwareInfoModal>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipSystemsSoftwareAssetsJSL : " + ex.Message);
            }
            return response;
        }
        #endregion

        #region Notifications
        //RDBJ 10/12/2021
        public List<Notifications> GetNotifications(
            string UserID, int UserGroup    // RDBJ 12/18/2021
            , string crntUserName // RDBJ 01/05/2022
            )
        {
            List<Notifications> response = new List<Notifications>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Notifications/GetNotifications?UserID=" + UserID + "&UserGroup=" + UserGroup + "&crntUserName=" + crntUserName).Result;  // RDBJ 01/05/2022 Added current username // RDBJ 12/18/2021 pass UserID and UserGroup
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Notifications>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNotifications Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 10/12/2021

        //RDBJ 10/16/2021
        public long GetCountOfNotifications(
            string UserID, int UserGroup    // RDBJ 12/18/2021
            , string crntUserName // RDBJ 01/05/2022
            )
        {
            long response = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Notifications/GetCountOfNotifications?UserID=" + UserID + "&UserGroup=" + UserGroup + "&crntUserName=" + crntUserName).Result; // RDBJ 01/05/2022 Added current username // RDBJ 12/18/2021 pass UserID and UserGroup
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<long>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCountOfNotifications Error : " + ex.Message);
            }
            return response;
        }
        //RDBJ 10/16/2021


        //RDBJ 10/16/2021
        public Notifications GetCountOfNotificationsById(string id, string formType
            , string crntUserName // RDBJ 01/05/2022
            ) //RDBJ 10/21/2021 Added formType
        {
            Notifications response = new Notifications();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Notifications/GetCountOfNotificationsById?id=" + id + "&formType=" + formType + "&crntUserName=" + crntUserName).Result; // RDBJ 01/05/2022 Added current username //RDBJ 10/21/2021 Added formType
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<Notifications>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCountOfNotificationsById Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 10/16/2021

        // RDBJ 04/01/2022 commented below function
        /*
        //RDBJ 10/16/2021
        public bool openAndSeenNotificationsUpdateStatusById(string id, string section, string formType) //RDBJ 10/21/2021 Added formType
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Notifications/openAndSeenNotificationsUpdateStatusById?id=" + id + "&section=" + section + "&formType=" + formType).Result; //RDBJ 10/21/2021 Added formType
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("openAndSeenNotificationsUpdateStatusById Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 10/16/2021
        */

        // RDBJ 04/01/2022
        public bool openAndSeenNotificationsUpdateStatusById(Dictionary<string, object> dicData)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(dicData), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Notifications/openAndSeenNotificationsUpdateStatusById", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("openAndSeenNotificationsUpdateStatusById Error : " + ex.Message);
            }
            return response;
        }
        // RDBJ 04/01/2022


        //RDBJ 12/21/2021
        public List<Notifications> LoadAssignedToMeGISIIADeficiencies(
            string UserID
            )
        {
            List<Notifications> response = new List<Notifications>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Notifications/LoadAssignedToMeGISIIADeficiencies?UserID=" + UserID).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Notifications>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("LoadAssignedToMeGISIIADeficiencies Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 12/21/2021
        #endregion Notifications

        #region HelpAndSupports
        // RDBJ 12/28/2021
        public List<HelpAndSupport> GetHelpAndSupportsList()
        {
            List<HelpAndSupport> response = new List<HelpAndSupport>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Help/GetHelpAndSupportsList").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HelpAndSupport>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetHelpAndSupportsList Error : " + ex.Message);
            }
            return response;
        }
        // End RDBJ 12/28/2021

        // RDBJ 12/28/2021
        public APIResponse InsertOrUpdateHelpAndSupport(HelpAndSupport Modal)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/Help/SubmitHelpAndSupport", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertOrUpdateHelpAndSupport Error : " + ex.Message);
            }
            return response;
        }
        // End RDBJ 12/28/2021

        // RDBJ 12/29/2021
        public APIResponse DeleteHelpAndSupport(string ID)
        {
            APIResponse response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Help/DeleteHelpAndSupport?ID=" + ID + "&ModifiedBy=" + SessionManager.Username).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteHelpAndSupport Error : " + ex.Message);
            }
            return response;
        }
        // End RDBJ 12/29/2021
        #endregion

        #region CommonAssets
        // RDBJ2 02/23/2022
        public Dictionary<string, string> PostAsyncAPICall(
            string APIControllerName,
            string APIActionName,
            Dictionary<string, string> dicMetadata
            )
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/26/2022
                    client.DefaultRequestHeaders.Add(AppStatic.API_KEY_HEADER, AppStatic.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken(
                            dictQueryString: dicMetadata    // JSL 09/27/2022
                        ));
                    // End JSL 09/26/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(dicMetadata), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync(APIControllerName + "/" + APIActionName, content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        dicRetMetadata = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("PostAsyncAPICall Error : " + ex.Message);
            }
            return dicRetMetadata;
        }
        // End RDBJ2 02/23/2022
        #endregion
    }
}
