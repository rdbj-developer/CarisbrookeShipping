using DBModificationService.Modals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBModificationService.Helpers
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
        public static string GetIMOSDBConnStr()
        {
            string connetionString = string.Empty;
            try
            {
                connetionString = ConfigurationManager.ConnectionStrings["ConnStrIMOS"].ConnectionString;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return connetionString;
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
        public static int ToINT(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch { return 0; }
        }
        public static string ToSTRING(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch { return string.Empty; }
        }
        public static bool ToBOOL(object value)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch { return false; }
        }
        public static SMTPServerModal GetSMTPSettings()
        {
            SMTPServerModal modal = new SMTPServerModal();
            try
            {
                modal.SMPTServerName = Utility.ToSTRING(ConfigurationManager.AppSettings["SMPTServerName"]);
                modal.SMTPPort = Utility.ToSTRING(ConfigurationManager.AppSettings["SMTPPort"]);
                modal.SMTPFromAddress = Utility.ToSTRING(ConfigurationManager.AppSettings["SMTPFromAddress"]);
                modal.SMTPUserName = Utility.ToSTRING(ConfigurationManager.AppSettings["SMTPUserName"]);
                modal.SMTPPassword = Utility.ToSTRING(ConfigurationManager.AppSettings["SMTPPassword"]);
                modal.IsAuthenticationRequired = Utility.ToBOOL(ConfigurationManager.AppSettings["IsAuthenticationRequired"]);
                modal.TOEmail = Utility.ToSTRING(ConfigurationManager.AppSettings["TOEmail"]);
                modal.CCEmail = Utility.ToSTRING(ConfigurationManager.AppSettings["CCEmail"]);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return modal;
        }
        public static string GetOfficeDBConnStr()
        {
            string connetionString = string.Empty;
            try
            {
                connetionString = ConfigurationManager.ConnectionStrings["CarisbrookingOfficeConnStr"].ConnectionString;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return connetionString;
        }
    }
}
