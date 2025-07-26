using Newtonsoft.Json;
using ShipApplication.BLL.Modals;
using ShipApplication.BLL.Resources.Constant;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ShipApplication.BLL.Helpers
{
    public class APIUsersHelper
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"].ToString();
        public UserModal LoginUser(ShipUserReq Modal)
        {
            UserModal response = new UserModal();
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
                    HttpResponseMessage resMsg = client.PostAsync("api/Users/LoginUser", content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<UserModal>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("LoginUser Error : " + ex.Message);
            }
            return response;
        }
        public List<UserModalAWS> GetAWSUsers()
        {
            List<UserModalAWS> response = new List<UserModalAWS>();
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
                    HttpResponseMessage resMsg = client.GetAsync("api/AWS/GetAWSUsersFromOfficeDBForSync").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<UserModalAWS>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAWSUsers Error : " + ex.Message);
            }
            return response;
        }

        //RDBJ 09/16/2021
        public List<CSShipsModalAWS> GetAWSShipsDetails()
        {
            List<CSShipsModalAWS> response = new List<CSShipsModalAWS>();
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
                    HttpResponseMessage resMsg = client.GetAsync("api/AWS/GetAWSCSShipsFromOfficeDBForSync").Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<List<CSShipsModalAWS>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAWSShipsDetails Error : " + ex.InnerException);  // JSL 11/03/2022
                LogHelper.writelog("GetAWSShipsDetails Error : " + ex.Message);
            }
            return response;
        }
        //End RDBJ 09/16/2021
    }
}
