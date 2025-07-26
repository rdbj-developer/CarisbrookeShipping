using ClosedXML.Excel;
using ShipApplication.BLL.Modals;
using ShipApplication.BLL.Resources.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace ShipApplication.BLL.Helpers
{
    public static class Utility
    {
        private static DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;

        // JSL 09/27/2022
        public static string Base64Encode(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return text;
            }

            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }
        // End JSL 09/27/2022

        // JSL 09/27/2022
        public static string Base64Decode(string base64)
        {
            if (String.IsNullOrEmpty(base64))
            {
                return base64;
            }

            var base64Bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
        // End JSL 09/27/2022

        // JSL 09/27/2022
        public static string GenerateBasicOAuthToken(string strUserName = "", string strPassword = "")
        {
            string strOAuthToken = string.Empty;

            strUserName = CarisbrookeShippingAPI.USERNAME_VALUE;
            strPassword = CarisbrookeShippingAPI.PASSWORD_VALUE;

            strOAuthToken = Base64Encode(strUserName + ":" + strPassword);
            return strOAuthToken;
        }
        // End JSL 09/27/2022

        public static string ToString(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch { return string.Empty; }
        }
        public static int ToInteger(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch { return 0; }
        }
        public static long ToLong(object value)
        {
            try
            {
                return Convert.ToInt64(value);
            }
            catch { return 0; }
        }
        public static double ToDouble(object value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch { return 0; }
        }
        public static string DateToString(object value)
        {
            try
            {
                return (Convert.ToDateTime(value)).ToString("dd-MMM-yyyy");
            }
            catch { return string.Empty; }
        }
        public static string GetCurrentDateString()
        {
            return DateTime.Now.ToString(new System.Globalization.CultureInfo("en-GB"));
        }

        public static string GetCurrentDateShortString()
        {
            return DateTime.Now.ToString(new System.Globalization.CultureInfo("en-GB")).Substring(0, 10);
        }

        public static bool ToBoolean(object value)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch { return false; }
        }
        public static T ToEnum<T>(this string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }

        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];

                // RDBJ 03/31/2022 set note : timeout 1000 = 1 sec
                int timeout = 10000;    // RDBJ 03/31/2022 Set 10000 from 1000

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
                    LogHelper.writelog("Data  " + connetionString);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return connetionString;
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> data, List<string> exculeColumns = null)
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
            if (exculeColumns != null && exculeColumns.Count > 0)
            {
                try
                {
                    foreach (var item in exculeColumns)
                    {
                        table.Columns.Remove(item);
                    }
                }
                catch (Exception)
                {
                }

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
        public static string ToDateTimeStr(object date, string format = "dd-MM-yyyy")
        {
            try
            {
                if (date != null)
                    return (Convert.ToDateTime(date).ToString(format)).ToString();
            }
            catch
            {                
            }
            return string.Empty;
        }
        public static DateTime ToDateTime(object value)
        {
            try
            {
                return Convert.ToDateTime(value);
            }
            catch { return Utility.ToDateTimeUtcNow(); } //RDBJ 10/27/2021 set UtcTime
        }        
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
        {
            return enumerable.GroupBy(keySelector).Select(grp => grp.First());
        }
        public static void WriteToXLS<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }
        public static Stream CreateWorkbook(DataSet ds, List<string> sheetNames)
        {
            try
            {
                XLWorkbook wbook = new XLWorkbook();
                string sheetName = "";
                for (int k = 0; k < ds.Tables.Count; k++)
                {
                    if (sheetNames == null)
                        sheetName = "Sheet" + (k + 1);
                    else
                        sheetName = sheetNames[k];
                    DataTable dt = ds.Tables[k];
                    IXLWorksheet Sheet = wbook.Worksheets.Add(sheetName);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        Sheet.Cell(1, (i + 1)).Value = dt.Columns[i].ColumnName;
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            Sheet.Cell((i + 2), (j + 1)).Value = dt.Rows[i][j].ToString();
                        }
                    }
                }

                Stream spreadsheetStream = new MemoryStream();
                wbook.SaveAs(spreadsheetStream);
                spreadsheetStream.Position = 0;
                return spreadsheetStream;
            }
            catch (Exception ex)
            {

            }
            return null;
            //return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
        }

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

        // JSL 12/03/2022
        public static string ConvertBase64IntoFile(Dictionary<string, string> dictFileData, bool blnIsGIRPhotos = false)
        {
            string strReturnFilePath = string.Empty;
            string strBasePath = HttpContext.Current.Server.MapPath("~/Images/");

            string UniqueFormID = string.Empty;
            string strReportType = string.Empty;
            string strDetailUniqueId = string.Empty;
            string strFileName = string.Empty;
            string strBase64String = string.Empty;

            try
            {
                if (dictFileData != null && dictFileData.Count > 0)
                {
                    if (dictFileData.ContainsKey("UniqueFormID"))
                        UniqueFormID = dictFileData["UniqueFormID"];

                    if (dictFileData.ContainsKey("ReportType"))
                        strReportType = dictFileData["ReportType"];

                    if (dictFileData.ContainsKey("DetailUniqueId"))
                        strDetailUniqueId = dictFileData["DetailUniqueId"];

                    if (dictFileData.ContainsKey("FileName"))
                        strFileName = dictFileData["FileName"];

                    if (dictFileData.ContainsKey("Base64FileData"))
                        strBase64String = dictFileData["Base64FileData"];
                }

                string strFileExtension = Path.GetExtension(strFileName);
                string strNewFileName = Path.GetFileNameWithoutExtension(strFileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + strFileExtension;

                string strStorePath = strReportType + @"/" + UniqueFormID;
                if (!blnIsGIRPhotos)
                {
                    strStorePath = strStorePath + @"/" + strDetailUniqueId;
                }

                // Delete existing files
                DirectoryInfo fileDirectory = new DirectoryInfo(Path.Combine(strBasePath, strStorePath));

                try
                {
                    FileInfo[] taskFiles = fileDirectory.GetFiles(Path.GetFileNameWithoutExtension(strFileName) + "_*");
                    foreach (FileInfo taskFile in taskFiles)
                        taskFile.Delete();
                }
                catch (Exception ex)
                {
                }

                strStorePath = strStorePath + @"/" + strNewFileName;
                string strFileStorePath = Path.Combine(strBasePath, strStorePath);
                Directory.CreateDirectory(Path.GetDirectoryName(strFileStorePath));

                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                strBase64String = regex.Replace(strBase64String, string.Empty);
                Byte[] bytes = Convert.FromBase64String(strBase64String);
                System.IO.File.WriteAllBytes(strFileStorePath, bytes);

                strReturnFilePath = strStorePath;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ConvertBase64IntoFile Error : " + ex.InnerException.ToString());
                strReturnFilePath = strBase64String;
            }
            return strReturnFilePath;
        }
        // End JSL 12/03/2022

        // JSL 12/04/2022
        public static string UpdateFilePathIn_Local_DB(Dictionary<string, string> dictMetaData, string strFilePath)
        {
            string strReturn = string.Empty;
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection connection = new SqlConnection(connetionString);

                    string strTableName = string.Empty;
                    string strColumnName = string.Empty;
                    string strWhereColumn = string.Empty;
                    string strWhereColumnDataID = string.Empty;

                    if (dictMetaData != null && dictMetaData.Count > 0)
                    {
                        if (dictMetaData.ContainsKey("TableName"))
                            strTableName = dictMetaData["TableName"];

                        if (dictMetaData.ContainsKey("ColumnName"))
                            strColumnName = dictMetaData["ColumnName"];

                        if (dictMetaData.ContainsKey("WhereColumnName"))
                            strWhereColumn = dictMetaData["WhereColumnName"];

                        if (dictMetaData.ContainsKey("WhereColumnDataID"))
                            strWhereColumnDataID = dictMetaData["WhereColumnDataID"];

                        string UpdateQury = string.Empty;
                        UpdateQury = "UPDATE " + strTableName + " SET " + strColumnName + " = '" + strFilePath + "'  WHERE " + strWhereColumn + " = '" + strWhereColumnDataID + "'";
                        SqlCommand command = new SqlCommand(UpdateQury, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFilePathIn_Local_DB Error : " + ex.Message.ToString());
            }
            return strReturn;
        }
        // End JSL 12/04/2022
    }

    public struct DateTimeSpan
    {
        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int Milliseconds { get; }

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            int officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                            if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }
    }
}
