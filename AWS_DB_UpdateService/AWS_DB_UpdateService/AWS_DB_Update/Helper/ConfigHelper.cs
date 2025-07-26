using AWS_DB_Update.Modals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_DB_Update.Helper
{
    public class ConfigHelper
    {
        public static LoginModal ReadLoginConfigJson()
        {
            try
            {
                string jsonFilePath = ConfigurationManager.AppSettings["LoginConfigPath"]; 
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!File.Exists(jsonFilePath))
                {
                    File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = File.ReadAllText(jsonFilePath);
                LoginModal Modal = null;
                if (!string.IsNullOrEmpty(jsonText))
                    Modal = JsonConvert.DeserializeObject<LoginModal>(jsonText);
                else
                {
                    Modal = new LoginModal
                    {
                        Company = Convert.ToString(ConfigurationManager.AppSettings["company"]),
                        User = Convert.ToString(ConfigurationManager.AppSettings["user"]),
                        Password = Convert.ToString(ConfigurationManager.AppSettings["password"])
                    };
                }
                return Modal;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ReadLoginConfigJson Error: " + ex.Message);
                return null;
            }
        }

        public static bool WriteLoginConfigJson(LoginModal objUser)
        {
            bool flag = true;
            try
            {
                string jsonFilePath = ConfigurationManager.AppSettings["LoginConfigPath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!File.Exists(jsonFilePath))
                {
                    File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                string jsonData = JsonConvert.SerializeObject(objUser, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("WriteLoginConfigJson Error: " + ex.Message);
                flag = false;
            }
            return flag;
        }
    }
}
