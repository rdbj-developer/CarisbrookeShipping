using CarisbrookeOpenFileService.Models;
using System;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.ServiceProcess;
using System.IO.Compression;
using CarisbrookeOpenFileService.Resources.Constant;

namespace CarisbrookeOpenFileService.Helper
{
    public static class Utility
    {
        static string OfficeAPPUrl = ConfigurationManager.AppSettings["OfficeAPPUrl"];

        // JSL 10/28/2022
        public static string Base64Encode(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return text;
            }

            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }
        // End JSL 10/28/2022

        // JSL 10/28/2022
        public static string Base64Decode(string base64)
        {
            if (String.IsNullOrEmpty(base64))
            {
                return base64;
            }

            var base64Bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
        // End JSL 10/28/2022

        // JSL 10/28/2022
        public static string GenerateBasicOAuthToken(string strUserName = "", string strPassword = "")
        {
            string strOAuthToken = string.Empty;

            strUserName = CarisbrookeShippingAPI.USERNAME_VALUE;
            strPassword = CarisbrookeShippingAPI.PASSWORD_VALUE;

            strOAuthToken = Base64Encode(strUserName + ":" + strPassword);
            return strOAuthToken;
        }
        // End JSL 10/28/2022

        public static ServiceConnectModal ReadWriteServiceConfigJson(ServiceConnectModal objSettings)
        {
            try
            {
                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["ServiceConfigPath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                //if (File.Exists(jsonFilePath))
                //    jsonText = File.ReadAllText(jsonFilePath);
                //else {                    
                //    jsonText = JsonConvert.SerializeObject(objSettings);
                //    File.WriteAllText(jsonFilePath, jsonText);
                //}
                //if (!string.IsNullOrEmpty(jsonText))
                //    return JsonConvert.DeserializeObject<ServiceConnectModal>(jsonText);                    
                //else
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ReadServiceConfigJson Error : " + ex.Message);
                return null;
            }
        }

        public static bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetLocalDBConnStr(ServerConnectModal dbConnModal)
        {
            string connetionString = string.Empty;
            try
            {
                if (dbConnModal != null)
                {
                    connetionString = "Data Source=" + dbConnModal.ServerName + ";Initial Catalog=" + dbConnModal.DatabaseName + ";User ID=" + dbConnModal.UserName + ";Password=" + dbConnModal.Password + "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return connetionString;
        }

        public static bool IsMainServiceRunning()
        {
            bool flag = false;
            try
            {
                //var objDbConfig = ReadDBConfigJson();
                //if (objDbConfig != null && objDbConfig.IsDBCreated)
                //    flag = true;
                ServiceController[] services = ServiceController.GetServices();
                if (services != null && services.Count() > 0)
                {
                    var mainService = services.Where(x => x.ServiceName == "CarisbrookeShippingService").FirstOrDefault();
                    if (mainService != null)
                        flag = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IsMainServiceRunning Create Error : " + ex.Message);
            }
            return flag;
        }

        public static ServerConnectModal ReadDBConfigJson()
        {
            try
            {
                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["DBConfigPath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (System.IO.File.Exists(jsonFilePath))
                {
                    jsonText = System.IO.File.ReadAllText(jsonFilePath);
                }
                if (!string.IsNullOrEmpty(jsonText))
                {
                    ServerConnectModal Modal = JsonConvert.DeserializeObject<ServerConnectModal>(jsonText);
                    return Modal;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DBConfig Create Error : " + ex.Message);
                return null;
            }
        }

        public static string ToString(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch { return string.Empty; }
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            DataTable table = new DataTable();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public static List<T> ToListof<T>(this DataTable dt) //, List<string> excludedColumnNames=null
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var objectProperties = typeof(T).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    //if (excludedColumnNames != null && excludedColumnNames.IndexOf(properties.Name) >= 0)
                    //    continue;
                    //else
                    //{
                    var convertedValue = GetValueByDataType(properties.PropertyType, dataRow[properties.Name]);
                    //properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                    properties.SetValue(instanceOfT, convertedValue, null);
                    // }                    
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }

        private static object GetValueByDataType(Type propertyType, object o)
        {
            if (o.ToString() == "null")
            {
                return null;
            }
            if (propertyType == (typeof(Guid)) || propertyType == typeof(Guid?))
            {
                return Guid.Parse(o.ToString());
            }
            else if (propertyType == typeof(int) || propertyType == typeof(int?) || propertyType.IsEnum)
            {
                return Convert.ToInt32(o);
            }
            else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                return Convert.ToDecimal(o);
            }
            else if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                return Convert.ToDouble(o);
            }
            else if (propertyType == typeof(long) || propertyType == typeof(long?))
            {
                return Convert.ToInt64(o);
            }
            else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return Convert.ToBoolean(o);
            }
            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                return Convert.ToDateTime(o);
            }
            return o.ToString();
        }

        /// <summary>
        /// Method that converts bytes to its human readable value
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string BytesToReadableValue(long number)
        {
            List<string> suffixes = new List<string> { " B", " KB", " MB", " GB", " TB", " PB" };

            for (int i = 0; i < suffixes.Count; i++)
            {
                long temp = number / (int)Math.Pow(1024, i + 1);

                if (temp == 0)
                {
                    return (number / (int)Math.Pow(1024, i)) + suffixes[i];
                }
            }

            return number.ToString();
        }

        public static bool DownloadAndUpdatesFiles(string destinationPath, string fileName)
        {
            bool res = false;
            try
            {
                string DownloadUrl = @"" + OfficeAPPUrl + "Service\\" + fileName;
                string prefixLocalPath = Path.GetTempPath() + Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmm");
                string downloadLocalPath = string.Format(@"" + prefixLocalPath + @"\" + fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));

                //Download updates from Server to Local
                WebClient wc = new WebClient();
                wc.DownloadFile(DownloadUrl, downloadLocalPath);

                //Extract locally downloaded updates
                ZipFile.ExtractToDirectory(downloadLocalPath, prefixLocalPath);

                fileName = fileName.Replace(".zip", "");
                //Copy downloaded updates to published location
                prefixLocalPath = prefixLocalPath + @"\" + fileName;
                //Force clean up
                CloneFiles(prefixLocalPath, destinationPath);

                //Remove downloaded updates
                if (File.Exists(downloadLocalPath))
                    File.Delete(downloadLocalPath);
               // Directory.Delete(prefixLocalPath, true);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DownloadAndUpdatesFiles " + ex.Message);
                res = false;
            }
            return res;
        }
        public static void CloneFiles(string root, string dest)
        {
            foreach (var file in Directory.GetFiles(root))
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);               
            }
        }
    }
}
