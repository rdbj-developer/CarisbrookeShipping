using CarisbrookeShippingService.BLL.Modals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;

namespace CarisbrookeShippingService.Helpers
{
    public static class Utility
    {
        //RDBJ 10/28/2021
        public static DateTime ToDateTimeUtcNow()
        {
            try
            {
                return DateTime.UtcNow;
            }
            catch (Exception ex)
            {

                return DateTime.UtcNow;
            }

        }
        //End RDBJ 10/28/2021
        public static string ToString(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch { return string.Empty; }
        }
        public static bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];

                // RDBJ 03/30/2022 set note : timeout 1000 = 1 sec
                int timeout = 10000;    // RDBJ 03/30/2022 Set 10000 from 1000
                
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
        public static string GetPCUniqueId()
        {
            string uniqueId = string.Empty;
            try
            {
                string cpuInfo = string.Empty;
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();
                try
                {
                    foreach (ManagementObject mo in moc)
                    {
                        cpuInfo = mo.Properties["processorID"].Value.ToString();
                        break;
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    string drive = "C";
                    ManagementObject dsk = new ManagementObject(
                        @"win32_logicaldisk.deviceid=""" + drive + @":""");
                    dsk.Get();
                    string volumeSerial = dsk["VolumeSerialNumber"].ToString();
                    uniqueId = cpuInfo + volumeSerial;
                }
                catch (Exception)
                {
                    uniqueId = cpuInfo + Environment.MachineName;
                }
            }
            catch (Exception ex)
            {
                uniqueId = Environment.MachineName;
                LogHelper.writelog("GetPCUniqueId : " + ex.Message);
            }
            return uniqueId;
        }

        public static string GetPCName()
        {
            string pcName = string.Empty;
            try
            {
                ManagementClass ManagementClass1 = new ManagementClass("Win32_OperatingSystem");
                //Create a ManagementObjectCollection to loop through
                ManagementObjectCollection ManagemenobjCol = ManagementClass1.GetInstances();
                foreach (ManagementObject obj in ManagemenobjCol)
                {
                    try
                    {
                        pcName = Utility.ToString(obj.Properties["CSName"].Value);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("GetPCName foreach loop : " + ex.Message);
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetPCName Exception : " + ex.Message);
            }
            if (string.IsNullOrWhiteSpace(pcName))
                pcName = string.IsNullOrWhiteSpace(Environment.MachineName) ? Environment.UserDomainName : Environment.MachineName;

            return pcName;
        }

        public static SimpleObject GetShipValue()
        {
            SimpleObject res = new SimpleObject();
            try
            {
                string ShipAPPUrl = System.Configuration.ConfigurationManager.AppSettings["ShipAPPLocalPath"];  // JSL 11/12/2022 uncommented
                //string jsonFilePath = @"C:\JsonFiles\Shipvalue.json"; // JSL 11/12/2022 commented
                string jsonFilePath = Path.Combine(ShipAPPUrl, @"JsonFiles\Shipvalue.json");    // JSL 11/12/2022
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                res = JsonConvert.DeserializeObject<SimpleObject>(jsonText);
            }
            catch (Exception)
            {
            }
            if (res == null || res.Code == null)
            {
                var objResult = BLL.Helpers.Utility.GetShipValue();
                res = new SimpleObject
                {
                    Code = objResult.id,
                    Name = objResult.name
                };
            }
            return res;
        }
    }
    public class Folders
    {
        public string Source { get; private set; }
        public string Target { get; private set; }

        public Folders(string source, string target)
        {
            Source = source;
            Target = target;
        }
    }

    public class SimpleObject
    {
        public string id { get; set; }
        public int ShipId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PCUniqueId { get; set; }
        public long ShipSystemsId { get; set; }
    }
}
