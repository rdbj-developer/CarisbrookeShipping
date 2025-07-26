using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CarisbrookeShippingService.Modals;
using CarisbrookeShippingService.BLL.Resources.Constant;

namespace CarisbrookeShippingService.Helpers
{
    public class APIHelper
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
        public ShipAppReleaseNoteModal GetLatestShipAppInfo()
        {
            ShipAppReleaseNoteModal response = new ShipAppReleaseNoteModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/28/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", CarisbrookeShippingService.BLL.Helpers.Utility.GenerateBasicOAuthToken());
                    // End JSL 09/28/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage resMsg = client.GetAsync("api/ShipAppReleaseNote/GetLatestShipAppInfo").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<ShipAppReleaseNoteModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLatestShipAppInfo Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse SubmitShipAppDownloadLog(ShipAppDownloadLogModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/28/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", CarisbrookeShippingService.BLL.Helpers.Utility.GenerateBasicOAuthToken());
                    // End JSL 09/28/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/ShipAppReleaseNote/AddShipAppDownloadLog", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Submit ShipAppDownloadLog Error : " + ex.Message);
            }
            return response;
        }
        public APIResponse AddMainSyncServicesEventLog(MainSyncServicesEventLogModal Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/28/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", CarisbrookeShippingService.BLL.Helpers.Utility.GenerateBasicOAuthToken());
                    // End JSL 09/28/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/ServicesInfo/AddMainSyncServicesEventLog", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddMainSyncServicesEventLog Error : " + ex.Message);
            }
            return response;
        }
        public ShipSystemModal GetShipSystemByPCId(ShipSystemModal Modal)
        {
            ShipSystemModal response = new ShipSystemModal();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/28/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", CarisbrookeShippingService.BLL.Helpers.Utility.GenerateBasicOAuthToken());
                    // End JSL 09/28/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/SystemInfo/GetShipSystemByPCId", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<ShipSystemModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipSystemByPCId Error : " + ex.Message);
            }
            return response;
        }
    }
}
