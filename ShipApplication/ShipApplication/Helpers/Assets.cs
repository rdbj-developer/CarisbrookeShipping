using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace ShipApplication.Helpers
{
    public static class Assets
    {
        // JSL 11/12/2022
        public static ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
        public static string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
        // End JSL 11/12/2022

        public static Dictionary<string, string> UploadFileAndReturnBase64EndCodedURL(HttpPostedFileBase file)
        {
            Dictionary<string, string> dicReturnAttachmentBase64 = new Dictionary<string, string>();
            bool blnIsAllowFileUpload = false;
            try
            {
                if (file.ContentLength < 2000000 && Assets.CheckFileType(file.ContentType))
                {
                    blnIsAllowFileUpload = true;
                }

                if (blnIsAllowFileUpload)
                {
                    System.IO.Stream fs = file.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    dicReturnAttachmentBase64["FileName"] = file.FileName;
                    dicReturnAttachmentBase64["AttachmentBase64"] = "data:" + file.ContentType + ";base64," + base64String;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UploadFileAndReturnBase64EndCodedURL Error : " + ex.InnerException.ToString());
            }
            return dicReturnAttachmentBase64;
        }

        // JSL 11/12/2022
        public static Dictionary<string, string> UploadFileAndReturnPath(HttpPostedFileBase file, string strDetailUniqueId, bool blnIsAuditNote = false, bool blnIsGIRPhotos = false, Dictionary<string, string> dictDefData = null)
        {
            Dictionary<string, string> dicReturnFileData = new Dictionary<string, string>();
            string strFileBasePath = HttpContext.Current.Server.MapPath("~/Images/");
            bool blnIsAllowFileUpload = false;

            // JSL 01/08/2023
            string strSubDetailType = string.Empty;
            string strSubDetailUniqueId = string.Empty;

            if (dictDefData != null && dictDefData.Count > 0)
            {
                if (dictDefData.ContainsKey("SubDetailType"))
                    strSubDetailType = dictDefData["SubDetailType"];

                if (dictDefData.ContainsKey("SubDetailUniqueId"))
                    strSubDetailUniqueId = dictDefData["SubDetailUniqueId"];
            }
            // End JSL 01/08/2023

            try
            {
                if (file.ContentLength < 2000000 && Assets.CheckFileType(file.ContentType))
                {
                    blnIsAllowFileUpload = true;
                }

                if (blnIsAllowFileUpload)
                {
                    string UniqueFormID = string.Empty;
                    string reportType = "";

                    if (!blnIsGIRPhotos)
                    {
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        DataTable dt = new DataTable();

                        string strQuery = string.Empty;
                        if (!blnIsAuditNote)
                        {
                            strQuery = "SELECT UniqueFormID, ReportType FROM " + AppStatic.GIRDeficiencies + " WHERE DeficienciesUniqueID = '" + strDetailUniqueId + "'";
                        }
                        else
                        {
                            strQuery = "SELECT UniqueFormID, 'IA' as ReportType FROM " + AppStatic.AuditNotes + " WHERE NotesUniqueID = '" + strDetailUniqueId + "'";
                        }
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(strQuery, connection);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            reportType = Convert.ToString(dt.Rows[0][1]);
                            UniqueFormID = Convert.ToString(dt.Rows[0][0]);
                        }
                        else
                        {
                            if (dictDefData != null && dictDefData.Count > 0)
                            {
                                reportType = dictDefData["ReportType"];
                                UniqueFormID = dictDefData["UniqueFormID"];
                            }
                        }
                    }
                    else
                    {
                        UniqueFormID = strDetailUniqueId;
                        reportType = "GI";
                    }

                    string strFileExtension = Path.GetExtension(file.FileName);
                    string strFileName = file.FileName;
                    string strNewFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + strFileExtension;

                    string strStorePath = reportType + @"/" + UniqueFormID;

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
                    strStorePath = strStorePath + @"/" + strNewFileName;

                    string strFileStorePath = Path.Combine(strFileBasePath, strStorePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(strFileStorePath));
                    file.SaveAs(strFileStorePath);

                    dicReturnFileData["FileName"] = strFileName;
                    dicReturnFileData["StorePath"] = strStorePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UploadFileAndReturnPath Error : " + ex.InnerException.ToString());
            }
            return dicReturnFileData;
        }
        // End JSL 11/12/2022

        public static bool CheckFileType(string strContentType)
        {
            switch (strContentType.ToLower())
            {
                case "image/jpeg":
                    return true;
                case "image/png":
                    return true;
                case "image/pjpeg":
                    return true;
                case "image/x-png":
                    return true;
                case "application/pdf":
                    return true;
                case "application/xml":
                    return true;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    return true;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.template":
                    return true;
                case "application/vnd.ms-excel.addin.macroEnabled.12":
                    return true;
                case "application/vnd.ms-excel.template.macroEnabled.12":
                    return true;
                case "application/vnd.ms-excel.sheet.binary.macroEnabled.12":
                    return true;
                case "application/vnd.ms-excel":
                    return true;
                case "application/vnd.ms-excel.sheet.macroEnabled.12":
                    return true;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.template":
                    return true;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    return true;
                case "application/vnd.ms-word.document.macroEnabled.12":
                    return true;
                case "application/vnd.ms-word.template.macroEnabled.12":
                    return true;
                case "application/msword":
                    return true;
                case "application/vnd.ms-powerpoint":
                    return true;
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    return true;
                case "application/vnd.openxmlformats-officedocument.presentationml.template":
                    return true;
                case "application/vnd.openxmlformats-officedocument.presentationml.slideshow":
                    return true;
                case "application/vnd.ms-powerpoint.addin.macroEnabled.12":
                    return true;
                case "application/vnd.ms-powerpoint.presentation.macroEnabled.12":
                    return true;
                case "application/vnd.ms-powerpoint.template.macroEnabled.12":
                    return true;
                case "application/vnd.ms-powerpoint.slideshow.macroEnabled.12":
                    return true;
                default:
                    return false;
            }
        }
    }
}