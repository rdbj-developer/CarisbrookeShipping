using Newtonsoft.Json;
using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OfficeApplication.BLL.Helpers
{
    public class DraftsHelper
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"];
        public GeneralInspectionReport GIRDeficienciesView(int id)
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
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GIRDeficienciesView/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = JsonConvert.DeserializeObject<GeneralInspectionReport>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesInitialActions Error : " + ex.Message);
            }
            return response;
        }
    }
}
