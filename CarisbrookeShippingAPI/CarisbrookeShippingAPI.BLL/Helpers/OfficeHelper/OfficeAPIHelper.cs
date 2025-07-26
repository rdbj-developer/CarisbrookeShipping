using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers.OfficeHelper
{
    // JSL 06/24/2022 added this file
    public static class OfficeAPIHelper
    {
        public static string OfficeAPIUrl = System.Configuration.ConfigurationManager.AppSettings["OfficeAPPUrl"];    // JSL 06/24/2022

        // JSL 06/24/2022
        public static Dictionary<string, string> PostAsyncOfficeAPICall(
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
                    client.BaseAddress = new Uri(OfficeAPIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
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
                LogHelper.writelog("PostAsyncOfficeAPICall Error : " + ex.Message);
            }
            return dicRetMetadata;
        }
        // End JSL 06/24/2022
    }
}
