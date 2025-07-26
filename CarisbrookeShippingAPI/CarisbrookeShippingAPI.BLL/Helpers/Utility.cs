using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public static class Utility
    {
        private static DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
        public static double ToDouble(object value)
        {

            try
            {
                return Convert.ToDouble(value);
            }
            catch { return 0; }
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
        public static string DateToString(object value)
        {
            try
            {
                
                return (Convert.ToDateTime(value, ukDtfi)).ToString("dd-MMM-yyyy");
            }
            catch { return string.Empty; }
        }
        public static DateTime ToDateTime(object value)
        {
            try
            {
                return Convert.ToDateTime(value, ukDtfi);
            }
            catch { return Utility.ToDateTimeUtcNow(); } //DateTime.Now; //RDBJ 10/27/2021 set UtcTime }
        }
        public static string ToString(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch { return string.Empty; }
        }
        public static string ToDateTimeStr(object date)
        {
            try
            {
                return (Convert.ToDateTime(date, ukDtfi).ToString("dd-MMM-yyyy")).ToString();
            }
            catch
            {
                return string.Empty;
            }
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
        public static Boolean isValidPONo(this string strToCheck)
        {
            string patern = @"^[a-zA-Z]{3}(-\d{4})(-\d{4})$";
            Regex sampleRegex = new Regex(patern);
            var res = sampleRegex.IsMatch(strToCheck);
            return res;
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

        // JSL 10/15/2022
        public static string Base64Encode(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return text;
            }

            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }
        // End JSL 10/15/2022

        // JSL 10/15/2022
        public static string Base64Decode(string base64)
        {
            if (String.IsNullOrEmpty(base64))
            {
                return base64;
            }

            var base64Bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
        // End JSL 10/15/2022

        // JSL 10/15/2022
        public static Dictionary<string, string> GetRequestedUserFromAuthorization(string strAuthorizationString)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            try
            {
                if (strAuthorizationString != null && strAuthorizationString.StartsWith("Basic"))
                {
                    //Extract credentials
                    string encodedUsernamePassword = strAuthorizationString.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    int seperatorIndex = usernamePassword.IndexOf(':');

                    string username = usernamePassword.Substring(0, seperatorIndex);
                    retDictMetaData["Username"] = username;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRequestedUserFromAuthorization : " + ex.Message);
            }
            return retDictMetaData;
        }
        // End JSL 10/15/2022

        // JSL 12/04/2022
        public static string ConvertIntoBase64EndCodedUploadedFile(string strFilePath)
        {
            string strReturnAttachmentBase64 = string.Empty;
            
            // JSL 01/12/2023
            if (string.IsNullOrEmpty(strFilePath))
                return strReturnAttachmentBase64;
            // End JSL 01/12/2023

            string strBasePath = System.Configuration.ConfigurationManager.AppSettings["OfficeAppPath"];
            strBasePath = strBasePath.Replace("\\", "/") + "\\/Images/";

            string strFinalFilePath = Path.Combine(strBasePath, strFilePath);
            string contentType = MimeMapping.GetMimeMapping(strFinalFilePath);
            try
            {
                //Byte[] bytes = ReadAllBytes(strFinalFilePath).Result; // JSL 12/24/2022 commented
                // JSL 12/24/2022 wrapped in if
                Byte[] bytes = null;
                if (File.Exists(strFinalFilePath))
                {
                    bytes = System.IO.File.ReadAllBytes(@strFinalFilePath);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    strReturnAttachmentBase64 = "data:" + contentType + ";base64," + base64String;
                }
                // End JSL 12/24/2022 wrapped in if
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ConvertIntoBase64EndCodedUploadedFile Error : " + ex.InnerException.ToString());
            }
            return strReturnAttachmentBase64;
        }
        // End JSL 12/04/2022

        // JSL 12/24/2022
        private const int BUFFER_SIZE = 0x4096;
        public static FileStream OpenRead(string path)
        {
            LogHelper.writelog("OpenRead Start path : " + path);
            path = path.Replace(@"\", "/");
            // Open a file stream for reading and that supports asynchronous I/O
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true);
        }
        // End JSL 12/24/2022

        // JSL 12/24/2022
        public static async Task<byte[]> ReadAllBytes(string path)
        {
            LogHelper.writelog("ReadAllBytes Start path : " + path);
            using (var fs = OpenRead(path))
            {
                var buff = new byte[fs.Length];
                await fs.ReadAsync(buff, 0, (int)fs.Length);
                return buff;
            }
        }
        // End JSL 12/24/2022

        // JSL 12/03/2022
        public static string ConvertBase64IntoFile(Dictionary<string, string> dictFileData, bool blnIsGIRPhotos = false
            , bool blnIsNeedToDeleteExistingAttachment = true // JSL 01/01/2023 set true,   // JSL 12/24/2022
            )
        {
            string strReturnFilePath = string.Empty;
            string strBasePath = System.Configuration.ConfigurationManager.AppSettings["OfficeAppPath"];
            strBasePath = strBasePath.Replace("\\", "/") + "/Images/";

            string UniqueFormID = string.Empty;
            string strReportType = string.Empty;
            string strDetailUniqueId = string.Empty;
            string strFileName = string.Empty;
            string strBase64String = string.Empty;

            // JSL 01/08/2023
            string strSubDetailType = string.Empty;
            string strSubDetailUniqueId = string.Empty;
            // End JSL 01/08/2023

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

                    // JSL 01/08/2023
                    if (dictFileData.ContainsKey("SubDetailType"))
                        strSubDetailType = dictFileData["SubDetailType"];

                    if (dictFileData.ContainsKey("SubDetailUniqueId"))
                        strSubDetailUniqueId = dictFileData["SubDetailUniqueId"];
                    // End JSL 01/08/2023
                }

                string strFileExtension = Path.GetExtension(strFileName);
                string strNewFileName = Path.GetFileNameWithoutExtension(strFileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + strFileExtension;

                string strStorePath = strReportType + @"/" + UniqueFormID;
                if (!blnIsGIRPhotos)
                {
                    strStorePath = strStorePath + @"/" + strDetailUniqueId;

                    // JSL 01/08/2023
                    if (!string.IsNullOrEmpty(strSubDetailUniqueId) && !string.IsNullOrEmpty(strSubDetailType))
                    {
                        strStorePath = strStorePath + @"/" + strSubDetailType + @"/" + strSubDetailUniqueId;
                    }
                    // End JSL 01/08/2023
                }

                // Delete existing files
                DirectoryInfo fileDirectory = new DirectoryInfo(Path.Combine(strBasePath, strStorePath));

                try
                {
                    // JSL 12/24/2022 wrapped in if
                    if (blnIsNeedToDeleteExistingAttachment)
                    {
                        // JSL 01/08/2023 wrapped in if
                        if (fileDirectory.Exists)
                        {
                            FileInfo[] taskFiles = fileDirectory.GetFiles(Path.GetFileNameWithoutExtension(strFileName) + "_*");
                            foreach (FileInfo taskFile in taskFiles)
                                taskFile.Delete();
                        }
                    }
                    // End JSL 12/24/2022 wrapped in if
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("ConvertBase64IntoFile taskFiles Error : " + ex.InnerException.ToString());
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
    }
}
