using AWS_DB_UpdateService.Modals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AWS_DB_UpdateService.Helpers
{
    public static class Utility
    {
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
        public static string GetDBConnStr()
        {
            string connetionString = string.Empty;
            try
            {
                connetionString = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return connetionString;
        }
        public static int ToInteger(object val)
        {
            try
            {
                return Convert.ToInt32(val);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string ToSTRING(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch { return string.Empty; }
        }
        public static List<T> ToListof<T>(this DataTable dt)
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
                    properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }
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
                if (!string.IsNullOrEmpty(jsonText))
                {
                    LoginModal Modal = JsonConvert.DeserializeObject<LoginModal>(jsonText);
                    return Modal;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("LoginConfig Create Error : " + ex.Message);
                return null;
            }
        }
    }
}
