using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace OfficeApplication.Helpers
{
    // JSL 12/02/2022 added this class
    public static class Assets
    {
        // JSL 12/02/2022
        public static Dictionary<string, string> UploadFileAndReturnPath(HttpPostedFileBase file, string strDetailUniqueId, bool blnIsAuditNote = false, bool blnIsGIRPhotos = false, Dictionary<string, string> dictDefData = null
            , bool blnIsFSTOAttachment = false
            )
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

                    if (!blnIsGIRPhotos
                        && !blnIsFSTOAttachment // JSL 02/18/2023
                        )
                    {
                        UniqueFormID = dictDefData["UniqueFormID"];
                        reportType = dictDefData["ReportType"];
                    }
                    else
                    {
                        UniqueFormID = strDetailUniqueId;
                        // JSL 02/18/2023 wrapped in if
                        if (blnIsGIRPhotos)
                            reportType = "GI";

                        if (blnIsFSTOAttachment)
                            reportType = "FSTO";
                        // End JSL 02/18/2023 wrapped in if
                    }

                    string strFileExtension = Path.GetExtension(file.FileName);
                    string strFileName = file.FileName;
                    string strNewFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + strFileExtension;

                    string strStorePath = reportType + @"/" + UniqueFormID;

                    if (!blnIsGIRPhotos
                        && !blnIsFSTOAttachment // JSL 02/18/2023
                        )
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
        // End JSL 12/02/2022

        // JSL 12/02/2022
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
        // End JSL 12/02/2022

        // JSL 02/14/2023
        public static List<UserProfile> GetAllUsersProfileList()
        {
            APIHelper _helper = new APIHelper();
            List<UserProfile> retModal = new List<UserProfile>();

            try
            {
                retModal = _helper.GetAllUsers()
                .OrderBy(x => x.UserName)
                .ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Assets GetAllUsersProfileList Error : " + ex.ToString());
            }

            return retModal;
        }
        // End JSL 02/14/2023

        // JSL 02/14/2023
        public static List<CSShipsModal> LoadShipsList()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = new List<CSShipsModal>();

            try
            {
                shipsList = _helper.GetAllShips();
                if (shipsList != null && shipsList.Count > 0)
                {
                    shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
                    shipsList = shipsList.OrderBy(x => x.Name).ToList();

                    if (SessionManager.UserGroup == "8") // 8 is used to avoid show some ships for Visitor Groups
                    {
                        shipsList = shipsList
                            .Where(x =>
                            x.Name.ToLower().StartsWith("j")
                            || x.Name.ToLower().StartsWith("c")
                            || x.Name.ToLower().StartsWith("vectis")
                            )
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Assets LoadShipsList Error : " + ex.ToString());
            }

            return shipsList;
        }
        // End JSL 02/14/2023
    }
}