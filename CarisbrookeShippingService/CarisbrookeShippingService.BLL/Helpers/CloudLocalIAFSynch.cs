using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CarisbrookeShippingService.BLL.Modals;
using CarisbrookeShippingService.BLL.Resources.Constant;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class CloudLocalIAFSynch
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
        public APIResponse sendIAFLocalToRemote(IAF Modal)
        {
            APIResponse response = new APIResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(60);  // JSL 07/16/2022 set 60 mins from 30mins // RDBJ 01/21/2022 set timeout
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    
                    // JSL 09/28/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/28/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/CloudIAF/sendIAFLocalToRemote", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("sendIAFLocalToRemote IAF Form Error : " + ex.Message);
            }
            return response;
        }
    }
}
