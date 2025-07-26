using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    // RDBJ 02/24/2022 added this class
    public class SettingsHelper
    {
        // RDBJ 02/24/2022
        public Dictionary<string, string> PerformPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                case AppStatic.API_UPDATEMAINSYNCSERVICESETTINGS:
                    {
                        try
                        {
                            string IntervalTime = string.Empty;
                            string UseServerTimeInterval = string.Empty;
                            string UpdatedBy = string.Empty;
                            string MainSyncServiceVersion = string.Empty;

                            if (dictMetaData.ContainsKey("IntervalTime"))
                                IntervalTime = dictMetaData["IntervalTime"].ToString();

                            if (dictMetaData.ContainsKey("UseServerTimeInterval"))
                                UseServerTimeInterval = dictMetaData["UseServerTimeInterval"].ToString().ToLower();

                            if (dictMetaData.ContainsKey("UpdatedBy"))
                                UpdatedBy = dictMetaData["UpdatedBy"].ToString();

                            if (dictMetaData.ContainsKey("MainSyncServiceVersion"))
                                MainSyncServiceVersion = dictMetaData["MainSyncServiceVersion"].ToString();

                            string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + @"JsonFiles\MainSyncServiceIntervalTime.json";
                            Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                            if (!File.Exists(jsonFilePath))
                            {
                                File.WriteAllText(jsonFilePath, string.Empty);
                            }
                            string jsonText = File.ReadAllText(jsonFilePath);
                            if (!string.IsNullOrEmpty(jsonText))
                            {
                                retDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
                                retDictMetaData["IntervalTime"] = IntervalTime;
                                retDictMetaData["UseServerTimeInterval"] = UseServerTimeInterval;
                                retDictMetaData["UpdatedBy"] = UpdatedBy;
                                retDictMetaData["UpdatedDate"] = Utility.ToDateTimeUtcNow().ToString("dd/MM/yyyy");
                                retDictMetaData["MainSyncServiceVersion"] = MainSyncServiceVersion;

                                jsonText = JsonConvert.SerializeObject(retDictMetaData, Formatting.Indented);
                                File.WriteAllText(jsonFilePath, jsonText);
                            }

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPDATEMAINSYNCSERVICESETTINGS + " Error : " + ex.Message);
                        }
                        break;
                    }
                case AppStatic.API_GETMAINSYNCSERVICESETTINGS:
                    {
                        try
                        {
                            string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + @"JsonFiles\MainSyncServiceIntervalTime.json";
                            Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                            if (System.IO.File.Exists(jsonFilePath))
                            {
                                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                                if (!string.IsNullOrEmpty(jsonText))
                                {
                                    retDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
                                }
                            }
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_GETMAINSYNCSERVICESETTINGS + " Error : " + ex.Message);
                        }
                        break;
                    }
                default:
                    break;
            }

            if (IsPerformSuccess)
            {
                retDictMetaData["Status"] = AppStatic.SUCCESS;
            }
            else
            {
                retDictMetaData["Status"] = AppStatic.ERROR;
            }

            return retDictMetaData;
        }
        // End RDBJ 02/24/2022
    }
}
