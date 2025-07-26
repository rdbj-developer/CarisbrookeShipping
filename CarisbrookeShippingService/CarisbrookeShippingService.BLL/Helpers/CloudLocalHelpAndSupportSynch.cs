using CarisbrookeShippingService.BLL.Modals;
using CarisbrookeShippingService.BLL.Resources.Constant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingService.BLL.Helpers
{
    // RDBJ 01/01/2021 Added this class
    public class CloudLocalHelpAndSupportSynch
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"]; // RDBJ 01/01/2021

        // RDBJ 01/01/2021
        public APIResponse SendHelpAndSupportDataLocalToRemote(HelpAndSupport Modal)
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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/28/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync("api/CloudHelpAndSupport/SendHelpAndSupportDataLocalToRemote", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<APIResponse>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SendHelpAndSupportDataLocalToRemote Error : " + ex.Message);
            }
            return response;
        }
        // End RDBJ 01/01/2021
    }
}
