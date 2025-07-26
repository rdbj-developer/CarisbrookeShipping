using Newtonsoft.Json;
using ShipApplication.BLL.Modals;
using ShipApplication.BLL.Resources.Constant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Helpers
{
    public class APIDeficienciesHelper
    {
        string APIUrl = System.Configuration.ConfigurationManager.AppSettings["APIUrl"].ToString();

        public string GetResponse(string apiUrl)
        {
            string resStr = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync(apiUrl).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        resStr = resMsg.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(apiUrl + " Error : " + ex.Message);
            }
            return resStr;
        }

        public GIRDeficiencies GetDeficienciesById(long id)
        {
            GIRDeficiencies response = new GIRDeficiencies();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // HTTP GET
                    HttpResponseMessage resMsg = client.GetAsync("api/Forms/GetDeficienciesById/" + id).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<GIRDeficiencies>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Error : " + ex.Message);
            }
            return response;
        }

        public List<Deficiency_GISI_Ships> GetShipDeficincyGrid(string code)
        {
            List<Deficiency_GISI_Ships> response = new List<Deficiency_GISI_Ships>();
            string resStr = GetResponse(APIURLHelper.GetShipDeficincyGrid + "/" + code);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_GISI_Ships>>(resStr);
            }
            return response;
        }
        public List<Deficiency_GISI_Report> GetShipGISIReports(string code, string type)
        {
            List<Deficiency_GISI_Report> response = new List<Deficiency_GISI_Report>();
            string resStr = GetResponse(APIURLHelper.GetShipGISIReports + "?id=" + code + "&type=" + type);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_GISI_Report>>(resStr);
            }
            return response;
        }
        public List<GIRDataList> GetGISIDeficiencies(Deficiency_GISI_Report Modal)
        {
            List<GIRDataList> response = new List<GIRDataList>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync(APIURLHelper.GetDeficienciesData, content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GIRDataList>>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Error : " + ex.Message);
            }
            return response;
        }
        public List<Deficiency_Audit_Ships> GetAuditShipsDeficincyGrid(string code)
        {
            List<Deficiency_Audit_Ships> response = new List<Deficiency_Audit_Ships>();
            string resStr = GetResponse(APIURLHelper.GetAuditShipsDeficincyGrid + "/" + code);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_Audit_Ships>>(resStr);
            }
            return response;
        }
        public List<Deficiency_Ship_Audits> GetShipAudits(string code)
        {
            List<Deficiency_Ship_Audits> response = new List<Deficiency_Ship_Audits>();
            string resStr = GetResponse(APIURLHelper.GetShipAudits + "/" + code);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Deficiency_Ship_Audits>>(resStr);
            }
            return response;
        }
        public IAF GetAuditDetails(int id)
        {
            IAF response = new IAF();
            string resStr = GetResponse("api/IAF/IAFormDetailsView/" + id);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<IAF>(resStr);
            }
            return response;
        }
        public bool UpdateAuditDeficiencies(long id, bool isClose)
        {
            bool response = false;
            string resStr = GetResponse(APIURLHelper.UpdateAuditDeficiencies + "?id=" + id + "&isClose=" + isClose);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
            }
            return response;
        }
        public bool AddAuditDeficiencyComments(Audit_Deficiency_Comments Modal)
        {
            bool response = false;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(APIUrl);
                    client.DefaultRequestHeaders.Accept.Clear();

                    // JSL 09/27/2022
                    client.DefaultRequestHeaders.Add(CarisbrookeShippingAPI.API_KEY_HEADER, CarisbrookeShippingAPI.API_KEY_VALUE);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Utility.GenerateBasicOAuthToken());
                    // End JSL 09/27/2022

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(Modal), Encoding.UTF8, "application/json");
                    // HTTP POST
                    HttpResponseMessage resMsg = client.PostAsync(APIURLHelper.AddAuditDeficiencyComments, content).Result;
                    if (resMsg.IsSuccessStatusCode)
                    {
                        string resStr = resMsg.Content.ReadAsStringAsync().Result;
                        response = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(resStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddAuditDeficiencyComments Error : " + ex.Message);
            }
            return response;
        }
        public List<Audit_Deficiency_Comments> GetAuditDeficiencyComments(long NoteID)
        {
            List<Audit_Deficiency_Comments> response = new List<Audit_Deficiency_Comments>();
            string resStr = GetResponse(APIURLHelper.GetAuditDeficiencyComments + "/" + NoteID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Audit_Deficiency_Comments>>(resStr);
            }
            return response;
        }
        public List<Audit_Deficiency_Comments_Files> GetAuditDeficiencyCommentFiles(long NoteID)
        {
            List<Audit_Deficiency_Comments_Files> response = new List<Audit_Deficiency_Comments_Files>();
            string resStr = GetResponse(APIURLHelper.GetAuditDeficiencyCommentFiles + "/" + NoteID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Audit_Deficiency_Comments_Files>>(resStr);
            }
            return response;
        }
        public string GetAuditCommentFile(long CommentFileID)
        {
            string response = string.Empty;
            string resStr = GetResponse(APIURLHelper.GetFileComment + "/" + CommentFileID);
            if (!string.IsNullOrEmpty(resStr))
            {
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resStr);
            }
            return response;
        }


        #region Local

        #region GI/SI
        public List<Deficiency_GISI_Ships> GetShipDeficincyGrid_Local_DB(string code)
        {
            List<Deficiency_GISI_Ships> GISI_Ships_List = new List<Deficiency_GISI_Ships>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            LogHelper.writelog("GetShipDeficincyGrid_Local_DB : SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE Ship = '" + code + "'");
                            conn.Open();
                            DataTable dt = new DataTable();
                            SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE Ship = '" + code + "'", conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficiencies> dbDefList = dt.ToListof<GIRDeficiencies>();
                                Deficiency_GISI_Ships modal = new Deficiency_GISI_Ships();
                                modal.Ship = code;
                                modal.ShipName = SessionManager.ShipName.ToString();
                                modal.TotalDeficiencies = dbDefList.Where(x => x.ReportType == "GI" || x.ReportType == "SI").Count();
                                modal.TotalOutstending = dbDefList.Where(x => x.IsClose == false || x.IsClose == null).Count();
                                modal.GIDeficiencies = dbDefList.Where(x => x.ReportType == "GI").Count();
                                modal.OpenGIDeficiencies = dbDefList.Where(x => x.ReportType == "GI" && x.IsClose == false).Count();
                                modal.SIDeficiencies = dbDefList.Where(x => x.ReportType == "SI").Count();
                                modal.OpenSIDeficiencies = dbDefList.Where(x => x.ReportType == "SI" && x.IsClose == false).Count();
                                GISI_Ships_List.Add(modal);
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipDeficincyGrid_Local_DB " + ex.Message);
            }
            return GISI_Ships_List;
        }
        public List<Deficiency_GISI_Report> GetShipGISIReports_Local_DB(string ShipCode, string type
            , bool blnIsSetWhereClauseWithNotEqual = false  // JSL 11/12/2022
            )
        {
            List<Deficiency_GISI_Report> GISI_Report_List = new List<Deficiency_GISI_Report>();
            if (type.ToLower() == "gi")
            {
                GISI_Report_List.AddRange(GetGIDeficiencies_Local_DB(ShipCode
                    , blnIsSetWhereClauseWithNotEqual: blnIsSetWhereClauseWithNotEqual    // JSL 11/12/2022
                    ));
            }
            else if (type.ToLower() == "si")
            {
                GISI_Report_List.AddRange(GetSIDeficiencies_Local_DB(ShipCode
                    , blnIsSetWhereClauseWithNotEqual: blnIsSetWhereClauseWithNotEqual    // JSL 11/12/2022
                    ));
            }

            return GISI_Report_List;
        }
        public List<Deficiency_GISI_Report> GetGIDeficiencies_Local_DB(string ShipCode
            , bool blnIsSetWhereClauseWithNotEqual = false  // JSL 11/12/2022
            )
        {
            List<Deficiency_GISI_Report> GI_Report_List = new List<Deficiency_GISI_Report>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();

                            // JSL 11/12/2022
                            string strQuery = string.Empty;
                            if (blnIsSetWhereClauseWithNotEqual)
                                strQuery = "SELECT * FROM " + AppStatic.GeneralInspectionReport + " WHERE Ship != '" + ShipCode + "'";
                            else
                                strQuery = "SELECT * FROM " + AppStatic.GeneralInspectionReport + " WHERE Ship = '" + ShipCode + "' AND SavedAsDraft = 0 AND [isDelete] = 0 Order By [GIRFormID] DESC";
                            // End JSL 11/12/2022

                            SqlDataAdapter sqlAdp = new SqlDataAdapter(strQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                LogHelper.writelog("GetGIDeficiencies_Local_DB Count: " + dt.Rows.Count);
                                List<GeneralInspectionReport> dbReportsList = dt.ToListof<GeneralInspectionReport>();
                                foreach (var item in dbReportsList)
                                {
                                    Deficiency_GISI_Report _report = new Deficiency_GISI_Report();
                                    //_report.FormID = item.GIRFormID;
                                    _report.UniqueFormID = item.UniqueFormID;
                                    _report.Auditor = item.Inspector;
                                    _report.Ship = item.Ship;
                                    _report.Type = "GI";
                                    //_report.Date = Utility.ToDateTimeStr(item.CreatedDate); // RDBJ 04/01/2022 commented this line
                                    _report.Date = item.Date; // RDBJ 04/01/2022
                                    _report.Location = item.Port;
                                    LogHelper.writelog("GetGIDeficiencies_Local_DB Port: " + item.Port);
                                    DataTable dtDeficiencies = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE ISNULL(isDelete, 0) = 0 and ReportType = 'GI' and UniqueFormID = '" + item.UniqueFormID + "' and Ship = '" + item.Ship + "'", conn); //RDBJ 10/13/2021 Added isDelete = 0
                                    sqlAdp.Fill(dtDeficiencies);
                                    List<GIRDeficiencies> defList = dtDeficiencies.ToListof<GIRDeficiencies>();
                                    if (defList != null && defList.Count > 0)
                                    {
                                        _report.Deficiencies = defList.Count;
                                        _report.OpenDeficiencies = defList.Where(x => x.IsClose == false).Count();
                                        foreach (var defItem in defList)
                                        {
                                            //int days = (Utility.ToDateTime(defItem.DateRaised).Date - DateTime.Now.Date).Days;
                                            int days = (Utility.ToDateTime(defItem.DateRaised).Date - Utility.ToDateTimeUtcNow().Date).Days;
                                            if (days < 0)
                                                days = -days;
                                            if (days > 84 && defItem.IsClose == false)
                                            {
                                                _report.isExpired = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _report.Deficiencies = 0;
                                        _report.OpenDeficiencies = 0;
                                        _report.isExpired = false;
                                    }
                                    GI_Report_List.Add(_report);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
                if (GI_Report_List != null && GI_Report_List.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIDeficiencies_Local_DB " + ex.Message);
            }
            return GI_Report_List
                .OrderByDescending(x => x.Date).ToList(); // RDBJ 04/01/2022
        }
        public List<Deficiency_GISI_Report> GetSIDeficiencies_Local_DB(string ShipCode
            , bool blnIsSetWhereClauseWithNotEqual = false  // JSL 11/12/2022
            )
        {
            List<Deficiency_GISI_Report> SI_Report_List = new List<Deficiency_GISI_Report>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();

                            // JSL 11/12/2022
                            string strQuery = string.Empty;
                            if (blnIsSetWhereClauseWithNotEqual)
                                strQuery = "SELECT * FROM " + AppStatic.SuperintendedInspectionReport + " WHERE ShipName != '" + ShipCode + "'";
                            else
                                strQuery = "SELECT * FROM " + AppStatic.SuperintendedInspectionReport + " WHERE ShipName = '" + ShipCode + "' AND SavedAsDraft = 0 AND [isDelete] = 0 Order By [SIRFormID] DESC";
                            // End JSL 11/12/2022

                            SqlDataAdapter sqlAdp = new SqlDataAdapter(strQuery, conn);

                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<SuperintendedInspectionReport> dbReportsList = dt.ToListof<SuperintendedInspectionReport>();
                                foreach (var item in dbReportsList)
                                {
                                    Deficiency_GISI_Report _report = new Deficiency_GISI_Report();
                                    //_report.FormID = item.SIRFormID;
                                    _report.Ship = item.ShipName;
                                    _report.Type = "SI";
                                    //_report.Date = Utility.ToDateTimeStr(item.CreatedDate); // RDBJ 04/01/2022 commented this line
                                    _report.Date = item.Date; // RDBJ 04/01/2022 
                                    _report.Location = item.Port;
                                    _report.UniqueFormID = item.UniqueFormID;
                                    _report.Auditor = item.Superintended; // RDBJ 01/15/2022
                                    DataTable dtDeficiencies = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE isDelete = 0 and ReportType = 'SI' and UniqueFormID = '" + item.UniqueFormID + "' and Ship = '" + item.ShipName + "'", conn); //RDBJ 10/13/2021 Added isDelete = 0
                                    sqlAdp.Fill(dtDeficiencies);
                                    List<GIRDeficiencies> defList = dtDeficiencies.ToListof<GIRDeficiencies>();
                                    if (defList != null && defList.Count > 0)
                                    {
                                        _report.Deficiencies = defList.Count;
                                        _report.OpenDeficiencies = defList.Where(x => x.IsClose == false).Count();
                                        foreach (var defItem in defList)
                                        {
                                            //int days = (Utility.ToDateTime(defItem.DateRaised).Date - DateTime.Now.Date).Days;
                                            int days = (Utility.ToDateTime(defItem.DateRaised).Date - Utility.ToDateTimeUtcNow().Date).Days;
                                            if (days < 0)
                                                days = -days;
                                            if (days > 84 && defItem.IsClose == false)
                                            {
                                                _report.isExpired = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _report.Deficiencies = 0;
                                        _report.OpenDeficiencies = 0;
                                        _report.isExpired = false;
                                    }
                                    SI_Report_List.Add(_report);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIDeficiencies_Local_DB " + ex.Message);
            }
            return SI_Report_List
                .OrderByDescending(x => x.Date).ToList(); // RDBJ 04/01/2022
        }
        public List<GIRDataList> GetDeficienciesData_Local_DB(Deficiency_GISI_Report value)
        {
            List<GIRDataList> list = new List<GIRDataList>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "";
                            if (value.Type == "GI")
                                query = "SELECT D.*,G.Inspector,G.Port,G.CreatedDate as DateRaised,G.UniqueFormID  FROM " + AppStatic.GIRDeficiencies + "  D Inner Join GeneralInspectionReport G on G.UniqueFormID = D.UniqueFormID WHERE D.isDelete = 0 and D.Ship = '" + value.Ship + "' and D.UniqueFormID = '" + value.UniqueFormID + "' and D.ReportType = '" + value.Type + "' ORDER BY No"; //RDBJ 10/13/2021 Added isDelete = 0
                            else if (value.Type == "SI")
                            {
                                query = "SELECT D.*,G.Superintended as Inspector,G.Port,G.CreatedDate as DateRaised,G.UniqueFormID  FROM " + AppStatic.GIRDeficiencies + "  D Inner Join SuperintendedInspectionReport G on G.UniqueFormID = D.UniqueFormID WHERE D.isDelete = 0 and D.Ship = '" + value.Ship + "' and D.UniqueFormID = '" + value.UniqueFormID + "' and D.ReportType = '" + value.Type + "' ORDER BY No"; //RDBJ 10/13/2021 Added isDelete = 0

                            }
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficiencies> dbDefList = dt.ToListof<GIRDeficiencies>();
                                foreach (var item in dbDefList)
                                {
                                    GIRDataList obj = new GIRDataList();

                                    DataTable dtDeficienciesFiles = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesFiles + " WHERE DeficienciesUniqueID = '" + item.DeficienciesUniqueID + "'", conn); //RDBJ 10/30/2021 DeficienciesUniqueID
                                    sqlAdp.Fill(dtDeficienciesFiles);
                                    List<GIRDeficienciesFile> defFiles = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                    foreach (var subitem in defFiles)
                                    {
                                        GIRDeficienciesFile filedata = new GIRDeficienciesFile();
                                        //filedata.DeficienciesID = subitem.DeficienciesID; // RDBJ 12/23/2021 Commented
                                        filedata.FileName = subitem.FileName;
                                        //filedata.StorePath = subitem.StorePath; // RDBJ 12/23/2021 Commented
                                        filedata.GIRDeficienciesFileID = subitem.GIRDeficienciesFileID;
                                        obj.GIRDeficienciesFile.Add(filedata);
                                    }

                                    obj.GIRFormID = item.GIRFormID;
                                    obj.DeficienciesID = item.DeficienciesID;
                                    obj.Deficiency = item.Deficiency;
                                    obj.IsClose = item.IsClose;
                                    obj.Number = item.No != 0 ? item.No.ToString() : item.SIRNo;
                                    obj.FileName = item.FileName;
                                    obj.StorePath = item.StorePath;
                                    obj.ReportType = item.ReportType;
                                    obj.DateRaised = item.DateRaised.HasValue ? item.DateRaised : item.CreatedDate;
                                    obj.DateClosed = item.DateClosed;
                                    obj.Section = item.Section;
                                    obj.Inspector = item.Inspector;
                                    obj.UniqueFormID = item.UniqueFormID;
                                    obj.Port = item.Port;
                                    obj.CreatedDate = Convert.ToDateTime(Utility.ToDateTimeStr(item.CreatedDate));
                                    obj.DeficienciesUniqueID = item.DeficienciesUniqueID;
                                    //int days = (Utility.ToDateTime(item.DateRaised).Date - DateTime.Now.Date).Days;
                                    int days = (Utility.ToDateTime(item.DateRaised).Date - Utility.ToDateTimeUtcNow().Date).Days;
                                    if (days < 0)
                                        days = -days;
                                    if (days > 84 && obj.IsClose == false)
                                        obj.isExpired = true;

                                    list.Add(obj);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesData_Local_DB : " + ex.Message);
            }
            return list;
        }
        public Dictionary<string, string> GetGIRDeficienciesFiles_Local(int id)   // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();   // RDBJ 01/27/2022 set with dictionary
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            if (conn.IsAvailable())
                            {
                                conn.Open();
                                DataTable dt = new DataTable();
                                string query = "SELECT * FROM " + AppStatic.GIRDeficienciesFiles + " WHERE GIRDeficienciesFileID = '" + id + "'";
                                SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                                sqlAdp.Fill(dt);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    List<GIRDeficienciesFile> lstDefList = dt.ToListof<GIRDeficienciesFile>();
                                    if (lstDefList != null && lstDefList.Count > 0)
                                    {
                                        retDicData["FileData"] = lstDefList[0].StorePath;   // RDBJ 01/27/2022 set with dictionary
                                        retDicData["FileName"] = lstDefList[0].FileName;   // RDBJ 01/27/2022 set with dictionary
                                    }
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesFiles_Local " + ex.Message + "\n" + ex.InnerException);

            }
            return retDicData;
        }

        public Dictionary<string, string> DownloadCommentFile_Local_DB(string CommentFileID)   // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();   // RDBJ 01/27/2022 set with dictionary
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        DataTable dtAuditNotesCommentFile = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT StorePath, FileName FROM " + AppStatic.GIRDeficienciesCommentFile + " WHERE CommentFileUniqueID = '" + CommentFileID + "'", conn); // RDBJ 01/27/2022 added FileName column
                        sqlAdp.Fill(dtAuditNotesCommentFile);
                        if (dtAuditNotesCommentFile != null && dtAuditNotesCommentFile.Rows.Count > 0)
                        {
                            retDicData["FileData"] = Convert.ToString(dtAuditNotesCommentFile.Rows[0][0]);   // RDBJ 01/27/2022 set with dictionary
                            retDicData["FileName"] = Convert.ToString(dtAuditNotesCommentFile.Rows[0][1]);   // RDBJ 01/27/2022 set with dictionary
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditNoteCommentFile_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }
        public GIRDataList GetDeficienciesDataByDeficienciesID_Local_DB(Guid? uniqueDeficienciesID)
        {
            GIRDataList obj = new GIRDataList();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            //string query = "SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE DeficienciesUniqueID = '" + uniqueDeficienciesID + "'";   // JSL 06/13/2022 commented this line
                            string query = "SELECT Def.*, Ship.Name AS ShipName FROM " + AppStatic.GIRDeficiencies + " Def LEFT JOIN " + AppStatic.CSShips + " Ship ON Ship.Code = Def.Ship WHERE DeficienciesUniqueID = '" + uniqueDeficienciesID + "'";    // JSL 06/13/2022
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficiencies> dbDefList = dt.ToListof<GIRDeficiencies>();
                                obj.GIRFormID = dbDefList[0].GIRFormID;
                                obj.DeficienciesID = dbDefList[0].DeficienciesID;
                                obj.Deficiency = dbDefList[0].Deficiency;
                                obj.DateRaised = dbDefList[0].DateRaised;
                                obj.DateClosed = dbDefList[0].DateClosed;
                                obj.Section = dbDefList[0].Section;
                                obj.Ship = dbDefList[0].Ship;
                                obj.ShipName = dbDefList[0].ShipName;   // JSL 06/13/2022
                                obj.ReportType = dbDefList[0].ReportType;
                                obj.DeficienciesUniqueID = dbDefList[0].DeficienciesUniqueID;
                                obj.No = dbDefList[0].No;
                                obj.IsClose = dbDefList[0].IsClose; //RDBJ 10/26/2021
                                obj.Priority = dbDefList[0].Priority; //RDBJ 10/30/2021
                                obj.UniqueFormID = dbDefList[0].UniqueFormID;   // JSL 12/04/2022
                                // JSL 07/18/2022
                                if (dbDefList[0].DueDate != null)
                                {
                                    obj.DueDate = dbDefList[0].DueDate; // RDBJ 03/01/2022
                                }
                                else
                                {
                                    obj.DueDate = dbDefList[0].DateRaised.Value.AddDays((double)(7 * obj.Priority));
                                }
                                // End JSL 07/18/2022

                                if (dbDefList.Count > 0)
                                {
                                    DataTable dtDeficienciesFiles = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesFiles + " WHERE DeficienciesUniqueID = '" + uniqueDeficienciesID + "'", conn);
                                    sqlAdp.Fill(dtDeficienciesFiles);
                                    obj.GIRDeficienciesFile = dtDeficienciesFiles.ToListof<GIRDeficienciesFile>();
                                    if (obj.GIRDeficienciesFile != null && obj.GIRDeficienciesFile.Count > 0)
                                    {
                                        foreach (var deffile in obj.GIRDeficienciesFile)
                                        {
                                            //deffile.StorePath = string.Empty; // JSL 12/03/2022 commented // RDBJ 12/23/2021
                                            // JSL 12/04/2022
                                            if (deffile.StorePath.StartsWith("data:"))
                                            {
                                                Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                                                dicFileMetaData["UniqueFormID"] = Convert.ToString(obj.UniqueFormID);
                                                dicFileMetaData["ReportType"] = obj.ReportType;
                                                dicFileMetaData["DetailUniqueId"] = Convert.ToString(obj.DeficienciesUniqueID);
                                                dicFileMetaData["FileName"] = deffile.FileName;
                                                dicFileMetaData["Base64FileData"] = deffile.StorePath;

                                                deffile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);

                                                if (!string.IsNullOrEmpty(deffile.StorePath))
                                                {
                                                    Dictionary<string, string> dicFilePathUpdateMetaData = new Dictionary<string, string>();
                                                    dicFilePathUpdateMetaData["TableName"] = AppStatic.GIRDeficienciesFiles;
                                                    dicFilePathUpdateMetaData["ColumnName"] = "StorePath";
                                                    dicFilePathUpdateMetaData["WhereColumnName"] = "DeficienciesFileUniqueID";
                                                    dicFilePathUpdateMetaData["WhereColumnDataID"] = Convert.ToString(deffile.DeficienciesFileUniqueID);
                                                    Utility.UpdateFilePathIn_Local_DB(dicFilePathUpdateMetaData, deffile.StorePath);
                                                }
                                            }
                                            // End JSL 12/04/2022
                                            deffile.IsUpload = "true";
                                        }
                                    }
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesDataByDeficienciesID_Local_DB : " + ex.Message);
            }
            return obj;
        }
        public List<GIRDeficienciesNote> GetDeficienciesNote_Local_DB(Guid id)
        {
            List<GIRDeficienciesNote> list = new List<GIRDeficienciesNote>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.GIRDeficienciesNote + " WHERE DeficienciesUniqueID = '" + id + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficienciesNote> dbDefNotesList = dt.ToListof<GIRDeficienciesNote>();
                                foreach (var item in dbDefNotesList)
                                {
                                    GIRDeficienciesNote obj = new GIRDeficienciesNote();
                                    obj.GIRDeficienciesCommentFile = new List<GIRDeficienciesCommentFile>();

                                    DataTable dtGIRDeficienciesCommentFiles = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesCommentFile + " WHERE NoteUniqueID = '" + item.NoteUniqueID + "'", conn);
                                    sqlAdp.Fill(dtGIRDeficienciesCommentFiles);
                                    List<GIRDeficienciesCommentFile> defCommentFiles = dtGIRDeficienciesCommentFiles.ToListof<GIRDeficienciesCommentFile>();

                                    foreach (var subitem in defCommentFiles)
                                    {
                                        GIRDeficienciesCommentFile filedata = new GIRDeficienciesCommentFile();
                                        //filedata.StorePath = subitem.StorePath; //RDBJ 10/22/2021 Commented
                                        filedata.FileName = subitem.FileName;
                                        //filedata.GIRCommentFileID = subitem.GIRCommentFileID; //RDBJ 10/22/2021 Commented
                                        filedata.CommentFileUniqueID = subitem.CommentFileUniqueID;
                                        obj.GIRDeficienciesCommentFile.Add(filedata);
                                    }
                                    obj.Comment = item.Comment;
                                    obj.UserName = item.UserName;
                                    obj.CreatedDate = item.CreatedDate;
                                    obj.isNew = item.isNew; //RDBJ 10/26/2021
                                    obj.IsResolution = false;   // JSL 07/05/2022
                                    list.Add(obj);
                                }

                                // JSL 07/05/2022
                                DataTable dtResolution = new DataTable();
                                string queryResolution = "SELECT * FROM " + AppStatic.GIRDeficienciesResolution + " WHERE DeficienciesUniqueID = '" + id + "'";
                                SqlDataAdapter sqlAdpResolution = new SqlDataAdapter(queryResolution, conn);    // JSL 07/07/2022
                                sqlAdpResolution.Fill(dtResolution);
                                if (dtResolution != null && dtResolution.Rows.Count > 0)
                                {
                                    List<GIRDeficienciesResolution> dbDefResolutionList = dtResolution.ToListof<GIRDeficienciesResolution>();
                                    foreach (var item in dbDefResolutionList)
                                    {
                                        GIRDeficienciesNote obj = new GIRDeficienciesNote();
                                        obj.GIRDeficienciesCommentFile = new List<GIRDeficienciesCommentFile>();

                                        DataTable dtGIRDeficienciesResolutionFiles = new DataTable();
                                        SqlDataAdapter sqlAdpResolutionFile = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesResolutionFile + " WHERE ResolutionUniqueID = '" + item.ResolutionUniqueID + "'", conn);
                                        sqlAdpResolutionFile.Fill(dtGIRDeficienciesResolutionFiles);
                                        List<GIRDeficienciesResolutionFile> defResolutionFiles = dtGIRDeficienciesResolutionFiles.ToListof<GIRDeficienciesResolutionFile>();

                                        foreach (var subitem in defResolutionFiles)
                                        {
                                            GIRDeficienciesCommentFile filedata = new GIRDeficienciesCommentFile();
                                            filedata.FileName = subitem.FileName;
                                            filedata.CommentFileUniqueID = subitem.ResolutionFileUniqueID;
                                            obj.GIRDeficienciesCommentFile.Add(filedata);
                                        }
                                        obj.Comment = item.Resolution;
                                        obj.UserName = item.Name;
                                        obj.CreatedDate = item.CreatedDate;
                                        obj.isNew = item.isNew;
                                        obj.IsResolution = true;
                                        list.Add(obj);
                                    }
                                }
                                // End JSL 07/05/2022

                                list = list.OrderByDescending(x => x.CreatedDate).ToList();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesNote_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public List<GIRDeficienciesFile> GetDeficienciesFiles_Local_DB(int id)
        {
            List<GIRDeficienciesFile> list = new List<GIRDeficienciesFile>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.GIRDeficienciesFiles + " WHERE DeficienciesID = " + id;
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficienciesFile> dbGIRDeficienciesFiles = dt.ToListof<GIRDeficienciesFile>();
                                foreach (var item in dbGIRDeficienciesFiles)
                                {
                                    GIRDeficienciesFile obj = new GIRDeficienciesFile();
                                    obj.StorePath = item.StorePath;
                                    obj.FileName = item.FileName;
                                    obj.DeficienciesID = item.DeficienciesID;
                                    obj.GIRDeficienciesFileID = item.GIRDeficienciesFileID;
                                    list.Add(obj);
                                }
                                list = list.OrderByDescending(x => x.DeficienciesID).ToList();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesNote_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public bool AddDeficienciesNote_Local_DB(GIRDeficienciesNote data)
        {
            bool res = false;
            try
            {
                string UniqueFormID = string.Empty;
                string reportType = "";
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesNote);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesNote); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID,ReportType FROM " + AppStatic.GIRDeficiencies + " WHERE DeficienciesUniqueID = '" + data.DeficienciesUniqueID + "'", connection);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            //data.GIRFormID = Convert.ToInt64(dt.Rows[0][0]);
                            UniqueFormID = Convert.ToString(dt.Rows[0][0]);
                            reportType = Convert.ToString(dt.Rows[0][1]);
                            //defiId = Convert.ToInt32(dt.Rows[0][2]);
                        }
                        string InsertQuery = GIRDeficienciesNote_InsertQuery();
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);

                        Guid NoteUniqueGUID = Guid.NewGuid();
                        // JSL 01/08/2023 wrapped in if
                        if (data.NoteUniqueID != null && data.NoteUniqueID != Guid.Empty)
                            NoteUniqueGUID = data.NoteUniqueID;
                        data.NoteUniqueID = NoteUniqueGUID;

                        //data.DeficienciesID = defiId;
                        GIRDeficienciesNote_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();

                        isTbaleCreated = true;
                        isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesCommentFile);
                        if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesCommentFile); }
                        if (isTbaleCreated)
                        {
                            if (data.GIRDeficienciesCommentFile != null && data.GIRDeficienciesCommentFile.Count > 0)
                            {
                                foreach (var defFile in data.GIRDeficienciesCommentFile)
                                {
                                    defFile.NoteID = 0; //RDBJ 11/10/2021 Set 0
                                    defFile.NoteUniqueID = NoteUniqueGUID;
                                    AddDeficienciesCommentFiles_Local_DB(defFile);
                                }
                            }
                        }
                        
                        UpdateGIRSyncStatus_Local_DB(UniqueFormID, reportType); //RDBJ 09/22/2021 removed false
                        UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB(Convert.ToString(data.DeficienciesUniqueID), reportType); //RDBJ 11/10/2021
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesNote_Local_DB Error : " + ex.InnerException.ToString());
                res = false;
            }
            return res;
        }
        public string GIRDeficienciesNote_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRDeficienciesNotes 
                                  (NoteUniqueID,GIRFormID,UserName,Comment,CreatedDate,ModifyDate,DeficienciesUniqueID,isNew)
                                  OUTPUT INSERTED.NoteID
                                  VALUES (@NoteUniqueID,@GIRFormID,@UserName,@Comment,@CreatedDate,@ModifyDate,@DeficienciesUniqueID,@isNew)";
            return InsertQuery;
        }
        public void GIRDeficienciesNote_CMD(GIRDeficienciesNote Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@NoteUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NoteUniqueID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID == null ? DBNull.Value : (object)Modal.GIRFormID;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = Modal.UserName == null ? string.Empty : Modal.UserName;
            command.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = Modal.Comment == null ? DBNull.Value : (object)Modal.Comment;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@ModifyDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 0; //RDBJ 10/26/2021
        }
        public bool UpdateDeficienciesData_Local_DB(string id, bool isClose)
        {
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    string UpdateQury = string.Empty;
                    if (isClose)
                        UpdateQury = "UPDATE " + AppStatic.GIRDeficiencies + " SET IsClose = 1 WHERE DeficienciesUniqueID = '" + id + "'";
                    else
                        UpdateQury = "UPDATE " + AppStatic.GIRDeficiencies + " SET IsClose = 0 WHERE DeficienciesUniqueID = '" + id + "'";
                    SqlConnection connection = new SqlConnection(connetionString);
                    SqlCommand command = new SqlCommand(UpdateQury, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficienciesData_Local_DB Error : " + ex.Message.ToString());
                return false;
            }
            return true;
        }

        //RDBJ 09/22/2021 change Logic, make common method rather than duplicates
        public string UpdateGIRSyncStatus_Local_DB(string UniqueFormID, string reportType) // RDBJ 02/20/2022 set string from bool //RDBJ 09/22/2021 removed  bool IsSynced
        {
            string strReturn = string.Empty;    // RDBJ 02/20/2022
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection connection = new SqlConnection(connetionString);

                    string UpdateQury = string.Empty;
                    string TableName = string.Empty; //RDBJ 09/22/2021
                    string FormVersion = string.Empty; //RDBJ 09/22/2021

                    if (reportType.ToUpper() == AppStatic.GIRForm)  // "GI")    // RDBJ 04/18/2022 set with Common variable rather than static
                    {
                        TableName = AppStatic.GeneralInspectionReport;
                    }
                    else if (reportType.ToUpper() == AppStatic.SIRForm)  //"SI")    // RDBJ 04/18/2022 set with Common variable rather than static
                    {
                        TableName = AppStatic.SuperintendedInspectionReport;
                    }
                    //RDBJ 10/04/2021
                    else
                    {
                        TableName = AppStatic.InternalAuditForm;
                    }

                    DataTable dt = new DataTable();
                    SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT FormVersion FROM " + TableName + " WHERE UniqueFormID = '" + UniqueFormID + "'", connection);
                    sqlAdp.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        FormVersion = Convert.ToString(Convert.ToDecimal(dt.Rows[0][0]) + Convert.ToDecimal(0.01));
                    }

                    UpdateQury = "UPDATE " + TableName + " SET IsSynced = 0, FormVersion = " + Convert.ToDecimal(FormVersion) + "  WHERE UniqueFormID = '" + UniqueFormID + "'";
                    SqlCommand command = new SqlCommand(UpdateQury, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    strReturn = FormVersion;    // RDBJ 02/20/2022
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficienciesData_Local_DB Error : " + ex.Message.ToString());
            }
            return strReturn;
        }
        //End RDBJ 09/22/2021 

        //RDBJ 11/10/2021
        public bool UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB(string UniqueID, string reportType) //RDBJ 09/22/2021 removed  bool IsSynced
        {
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection connection = new SqlConnection(connetionString);

                    string UpdateQury = string.Empty;
                    string TableName = string.Empty;
                    string ColumnName = string.Empty;

                    if (reportType.ToUpper() == "IAF")
                    {
                        TableName = AppStatic.AuditNotes;
                        ColumnName = "NotesUniqueID";
                    }
                    else
                    {
                        TableName = AppStatic.GIRDeficiencies;
                        ColumnName = "DeficienciesUniqueID";
                    }

                    UpdateQury = "UPDATE " + TableName + " SET UpdatedDate = '" + Utility.ToDateTimeUtcNow().ToString("MM/dd/yyyy hh:mm:ss") + "'  WHERE " + ColumnName + " = '" + UniqueID + "'";
                    SqlCommand command = new SqlCommand(UpdateQury, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB Error : " + ex.Message.ToString());
                return false;
            }
            return true;
        }
        //End RDBJ 11/10/2021

        public string GETGIRUpdateQuery()
        {
            string query = @"UPDATE dbo.GIRDeficiencies SET IsClose = @IsClose,
                           WHERE DeficienciesID = @DeficienciesID";
            return query;
        }
        public List<GIRDeficienciesInitialActions> GetDeficienciesInitialActions_Local_DB(Guid id)
        {
            List<GIRDeficienciesInitialActions> list = new List<GIRDeficienciesInitialActions>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.GIRDeficienciesInitialActions + " WHERE DeficienciesUniqueID = '" + id + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficienciesInitialActions> dbDefNotesList = dt.ToListof<GIRDeficienciesInitialActions>();
                                foreach (var item in dbDefNotesList)
                                {
                                    GIRDeficienciesInitialActions obj = new GIRDeficienciesInitialActions();
                                    obj.GIRDeficienciesInitialActionsFiles = new List<GIRDeficienciesInitialActionsFile>();

                                    DataTable dtGIRDeficienciesCommentFiles = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesInitialActionsFile + " WHERE IniActUniqueID ='" + item.IniActUniqueID + "'", conn);
                                    sqlAdp.Fill(dtGIRDeficienciesCommentFiles);
                                    List<GIRDeficienciesInitialActionsFile> defCommentFiles = dtGIRDeficienciesCommentFiles.ToListof<GIRDeficienciesInitialActionsFile>();

                                    foreach (var subitem in defCommentFiles)
                                    {
                                        GIRDeficienciesInitialActionsFile filedata = new GIRDeficienciesInitialActionsFile();
                                        //filedata.StorePath = subitem.StorePath; //RDBJ 10/22/2021 Commented
                                        filedata.FileName = subitem.FileName;
                                        //filedata.GIRInitialID = subitem.GIRInitialID; //RDBJ 10/22/2021 Commented
                                        //filedata.GIRFileID = subitem.GIRFileID; //RDBJ 10/22/2021 Commented
                                        filedata.IniActFileUniqueID = subitem.IniActFileUniqueID; //RDBJ 09/18/2021
                                        obj.GIRDeficienciesInitialActionsFiles.Add(filedata);
                                    }
                                    obj.Description = item.Description;
                                    obj.Name = item.Name;
                                    obj.CreatedDate = item.CreatedDate;
                                    obj.isNew = item.isNew; //RDBJ 10/26/2021
                                    list.Add(obj);
                                }
                                list = list.OrderByDescending(x => x.CreatedDate).ToList();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesInitialActions_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public string GIRDeficienciesInitialActions_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesInitialActions]([IniActUniqueID],[GIRFormID],[Name],[Description],[CreatedDate],DeficienciesUniqueID,isNew)
                                  OUTPUT INSERTED.GIRInitialID
                                  VALUES (@IniActUniqueID,@GIRFormID,@Name,@Description,@CreatedDate,@DeficienciesUniqueID,@isNew)";
            return InsertQuery;
        }
        public void GIRDeficienciesInitialActions_CMD(GIRDeficienciesInitialActions Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@IniActUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.IniActUniqueID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID == null ? DBNull.Value : (object)Modal.GIRFormID;
            command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Modal.Name == null ? string.Empty : Modal.Name;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Modal.Description == null ? DBNull.Value : (object)Modal.Description;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 0; //RDBJ 10/26/2021
        }
        public Dictionary<string, string> AddDeficienciesInitialActions_Local_DB(GIRDeficienciesInitialActions data) // RDBJ 02/20/2022 set return with Dictionary
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            try
            {
                string UniqueFormID = string.Empty;
                string defiUniqueId = string.Empty;
                string reportType = "";
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesInitialActions);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesInitialActions); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID,ReportType FROM " + AppStatic.GIRDeficiencies + " WHERE DeficienciesUniqueID = '" + data.DeficienciesUniqueID + "'", connection);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            //data.GIRFormID = Convert.ToInt64(dt.Rows[0][0]);
                            UniqueFormID = Convert.ToString(dt.Rows[0][0]);
                            reportType = Convert.ToString(dt.Rows[0][1]);
                        }
                        Guid IniActUniqueID = Guid.NewGuid();
                        // JSL 01/08/2023 wrapped in if
                        if (data.IniActUniqueID != null && data.IniActUniqueID != Guid.Empty)
                            IniActUniqueID = data.IniActUniqueID;
                        data.IniActUniqueID = IniActUniqueID;

                        string InsertQuery = GIRDeficienciesInitialActions_InsertQuery();
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                        GIRDeficienciesInitialActions_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();

                        isTbaleCreated = true;
                        isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesInitialActionsFile);
                        if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesInitialActionsFile); }
                        if (isTbaleCreated)
                        {
                            if (data.GIRDeficienciesInitialActionsFiles != null && data.GIRDeficienciesInitialActionsFiles.Count > 0)
                            {
                                foreach (var defFile in data.GIRDeficienciesInitialActionsFiles)
                                {
                                    defFile.GIRInitialID = 0; //RDBJ 11/10/2021 Set 0
                                    defFile.IniActUniqueID = IniActUniqueID;
                                    AddDeficienciesInitialActionsFiles_Local_DB(defFile);
                                }
                            }
                        }
                        retDictMetaData["FormVersion"] = UpdateGIRSyncStatus_Local_DB(UniqueFormID, reportType); //RDBJ 09/22/2021 removed false
                        UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB(Convert.ToString(data.DeficienciesUniqueID), reportType); //RDBJ 11/10/2021
                        
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesInitialActions_Local_DB Error : " + ex.InnerException.ToString());
                retDictMetaData["FormVersion"] = string.Empty;  // RDBJ 02/20/2022
            }
            return retDictMetaData; // RDBJ 02/20/2022
        }
        public bool AddDeficienciesInitialActionsFiles_Local_DB(GIRDeficienciesInitialActionsFile data)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesInitialActionsFile);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesInitialActionsFile); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        Guid IniActFileUniqueID = Guid.NewGuid();
                        data.IniActFileUniqueID = IniActFileUniqueID;
                        string InsertQuery = GIRDeficienciesInitialActionsFile_InsertQuery();
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                        GIRDeficienciesInitialActionsFile_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRDeficienciesInitialActionsFile_Local_DB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        public bool AddDeficienciesCommentFiles_Local_DB(GIRDeficienciesCommentFile data)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesCommentFile);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesCommentFile); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQuery = GIRDeficienciesCommentFile_InsertQuery();
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);

                        Guid CommentFileUniqueGUID = Guid.NewGuid();
                        data.CommentFileUniqueID = CommentFileUniqueGUID;

                        GIRDeficienciesCommentFile_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRDeficienciesInitialActionsFile_Local_DB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string GIRDeficienciesInitialActionsFile_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesInitialActionsFile]
                                ([IniActUniqueID],[IniActFileUniqueID],[GIRInitialID],[FileName],[StorePath],[IsUpload])
                                  OUTPUT INSERTED.GIRFileID
                                  VALUES (@IniActUniqueID,@IniActFileUniqueID,@GIRInitialID,@FileName,@StorePath,@IsUpload)";
            return InsertQuery;
        }
        public void GIRDeficienciesInitialActionsFile_CMD(GIRDeficienciesInitialActionsFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@IniActUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.IniActUniqueID;
            command.Parameters.Add("@IniActFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.IniActFileUniqueID;
            command.Parameters.Add("@GIRInitialID", SqlDbType.BigInt).Value = Modal.GIRInitialID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
            command.Parameters.Add("@IsUpload", SqlDbType.NVarChar).Value = Modal.IsUpload == null ? DBNull.Value : (object)Modal.IsUpload;
        }

        public string GIRDeficienciesCommentFile_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesCommentFile]
                                ([CommentFileUniqueID], [NoteUniqueID], [NoteID],[FileName],[StorePath],[IsUpload])
                                  OUTPUT INSERTED.GIRCommentFileID
                                  VALUES (@CommentFileUniqueID,@NoteUniqueID,@NoteID,@FileName,@StorePath,@IsUpload)";
            return InsertQuery;
        }
        public void GIRDeficienciesCommentFile_CMD(GIRDeficienciesCommentFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@CommentFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentFileUniqueID;
            command.Parameters.Add("@NoteUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NoteUniqueID;
            command.Parameters.Add("@NoteID", SqlDbType.BigInt).Value = Modal.NoteID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
            command.Parameters.Add("@IsUpload", SqlDbType.NVarChar).Value = Modal.IsUpload == null ? DBNull.Value : (object)Modal.IsUpload;
        }
        public List<GIRDeficienciesResolution> GetDeficienciesResolution_Local_DB(Guid id)
        {
            List<GIRDeficienciesResolution> list = new List<GIRDeficienciesResolution>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.GIRDeficienciesResolution + " WHERE DeficienciesUniqueID = '" + id + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<GIRDeficienciesResolution> dbDefNotesList = dt.ToListof<GIRDeficienciesResolution>();
                                foreach (var item in dbDefNotesList)
                                {
                                    GIRDeficienciesResolution obj = new GIRDeficienciesResolution();
                                    obj.GIRDeficienciesResolutionFiles = new List<GIRDeficienciesResolutionFile>();

                                    DataTable dtGIRDeficienciesCommentFiles = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.GIRDeficienciesResolutionFile + " WHERE ResolutionUniqueID = '" + item.ResolutionUniqueID + "'", conn);
                                    sqlAdp.Fill(dtGIRDeficienciesCommentFiles);
                                    List<GIRDeficienciesResolutionFile> defCommentFiles = dtGIRDeficienciesCommentFiles.ToListof<GIRDeficienciesResolutionFile>();

                                    foreach (var subitem in defCommentFiles)
                                    {
                                        GIRDeficienciesResolutionFile filedata = new GIRDeficienciesResolutionFile();
                                        //filedata.StorePath = subitem.StorePath; //RDBJ 10/22/2021 Commented
                                        filedata.FileName = subitem.FileName;
                                        //filedata.GIRResolutionID = subitem.GIRResolutionID; //RDBJ 10/22/2021 Commented
                                        //filedata.GIRFileID = subitem.GIRFileID; //RDBJ 10/22/2021 Commented
                                        filedata.ResolutionFileUniqueID = subitem.ResolutionFileUniqueID; //RDBJ 09/18/2021
                                        obj.GIRDeficienciesResolutionFiles.Add(filedata);
                                    }
                                    obj.Resolution = item.Resolution;
                                    obj.Name = item.Name;
                                    obj.CreatedDate = item.CreatedDate;
                                    obj.isNew = item.isNew;
                                    list.Add(obj);
                                }
                                list = list.OrderByDescending(x => x.CreatedDate).ToList();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesResolution_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public string GIRDeficienciesResolution_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesResolution]([ResolutionUniqueID],[GIRFormID],[Name],[Resolution],[CreatedDate],[DeficienciesUniqueID],[isNew])
                                  OUTPUT INSERTED.GIRResolutionID
                                  VALUES (@ResolutionUniqueID,@GIRFormID,@Name,@Resolution,@CreatedDate,@DeficienciesUniqueID,@isNew)";
            return InsertQuery;
        }
        public void GIRDeficienciesResolution_CMD(GIRDeficienciesResolution Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ResolutionUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionUniqueID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID == null ? DBNull.Value : (object)Modal.GIRFormID;
            command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Modal.Name == null ? string.Empty : Modal.Name;
            command.Parameters.Add("@Resolution", SqlDbType.NVarChar).Value = Modal.Resolution == null ? DBNull.Value : (object)Modal.Resolution;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 0; //RDBJ 10/26/2021
        }
        public bool AddDeficienciesResolution_Local_DB(GIRDeficienciesResolution data)
        {
            bool res = false;
            try
            {
                string UniqueFormID = string.Empty;
                string defiUniqueId = string.Empty;
                string reportType = "";
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesResolution);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesResolution); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID,ReportType FROM " + AppStatic.GIRDeficiencies + " WHERE DeficienciesUniqueID = '" + data.DeficienciesUniqueID + "'", connection);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            // data.GIRFormID = Convert.ToInt64(dt.Rows[0][0]);
                            UniqueFormID = Convert.ToString(dt.Rows[0][0]);
                            reportType = Convert.ToString(dt.Rows[0][1]);
                            //defiUniqueId = Convert.ToString(dt.Rows[0][2]);
                        }
                        Guid ResolutionUniqueID = Guid.NewGuid();
                        data.ResolutionUniqueID = ResolutionUniqueID;
                        //data.DeficienciesUniqueID = new Guid(defiUniqueId);

                        string InsertQuery = GIRDeficienciesResolution_InsertQuery();
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                        GIRDeficienciesResolution_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();
                        isTbaleCreated = true;
                        isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesResolutionFile);
                        if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesResolutionFile); }
                        if (isTbaleCreated)
                        {
                            if (data.GIRDeficienciesResolutionFiles != null && data.GIRDeficienciesResolutionFiles.Count > 0)
                            {
                                foreach (var defFile in data.GIRDeficienciesResolutionFiles)
                                {
                                    defFile.GIRResolutionID = 0; //RDBJ 11/10/2021 Set 0
                                    defFile.ResolutionUniqueID = ResolutionUniqueID;
                                    AddGIRDeficienciesResolutionFiles_Local_DB(defFile);
                                }
                            }
                        }
                    }
                    UpdateGIRSyncStatus_Local_DB(UniqueFormID, reportType); //RDBJ 09/22/2021 removed false
                    UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB(Convert.ToString(data.DeficienciesUniqueID), reportType); //RDBJ 11/10/2021
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesInitialActions_Local_DB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool AddGIRDeficienciesResolutionFiles_Local_DB(GIRDeficienciesResolutionFile data)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesResolutionFile);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesResolutionFile); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        Guid ResolutionFileUniqueID = Guid.NewGuid();
                        data.ResolutionFileUniqueID = ResolutionFileUniqueID;
                        string InsertQuery = GIRDeficienciesResolutionFile_InsertQuery();
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                        GIRDeficienciesResolutionFile_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRDeficienciesResolutionFile_Local_DB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string GIRDeficienciesResolutionFile_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[GIRDeficienciesResolutionFile]
                                ([ResolutionUniqueID],[ResolutionFileUniqueID],[GIRResolutionID],[FileName],[StorePath],[IsUpload])
                                  OUTPUT INSERTED.GIRFileID
                                  VALUES (@ResolutionUniqueID,@ResolutionFileUniqueID,@GIRResolutionID,@FileName,@StorePath,@IsUpload)";
            return InsertQuery;
        }
        public void GIRDeficienciesResolutionFile_CMD(GIRDeficienciesResolutionFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ResolutionUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionUniqueID;
            command.Parameters.Add("@ResolutionFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.ResolutionFileUniqueID;
            command.Parameters.Add("@GIRResolutionID", SqlDbType.BigInt).Value = Modal.GIRResolutionID;
            //command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
            command.Parameters.Add("@IsUpload", SqlDbType.NVarChar).Value = Modal.IsUpload == null ? DBNull.Value : (object)Modal.IsUpload;
        }
        public Dictionary<string, string> GetGIRDeficienciesInitialActionFile_Local(string IntActFileID)  // RDBJ 01/27/2022 set with dictionary //RDBJ 09/18/2021 change int id
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();  // RDBJ 01/27/2022 set with dictionary
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            if (conn.IsAvailable())
                            {
                                conn.Open();
                                DataTable dt = new DataTable();
                                string query = "SELECT * FROM " + AppStatic.GIRDeficienciesInitialActionsFile + " WHERE IniActFileUniqueID = '" + IntActFileID + "'"; //RDBJ 09/18/2021 change id
                                SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                                sqlAdp.Fill(dt);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    List<GIRDeficienciesInitialActionsFile> lstDefList = dt.ToListof<GIRDeficienciesInitialActionsFile>();
                                    if (lstDefList != null && lstDefList.Count > 0)
                                    {
                                        retDicData["FileData"] = lstDefList[0].StorePath;  // RDBJ 01/27/2022 set with dictionary
                                        retDicData["FileName"] = lstDefList[0].FileName;  // RDBJ 01/27/2022 set with dictionary
                                    }
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesInitialActionFile_Local " + ex.Message + "\n" + ex.InnerException);

            }
            return retDicData;
        }
        public Dictionary<string, string> GetGIRDeficienciesResolutionFile_Local(string ResolutionFileID)  // RDBJ 01/27/2022 set with dictionary //RDBJ 09/18/2021 change int id
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();  // RDBJ 01/27/2022 set with dictionary
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            if (conn.IsAvailable())
                            {
                                conn.Open();
                                DataTable dt = new DataTable();
                                string query = "SELECT * FROM " + AppStatic.GIRDeficienciesResolutionFile + " WHERE ResolutionFileUniqueID = '" + ResolutionFileID + "'"; //RDBJ 09/18/2021 change id
                                SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                                sqlAdp.Fill(dt);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    List<GIRDeficienciesResolutionFile> lstDefList = dt.ToListof<GIRDeficienciesResolutionFile>();
                                    if (lstDefList != null && lstDefList.Count > 0)
                                    {
                                        retDicData["FileData"] = lstDefList[0].StorePath;  // RDBJ 01/27/2022 set with dictionary
                                        retDicData["FileName"] = lstDefList[0].FileName;  // RDBJ 01/27/2022 set with dictionary
                                    }
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesResolutionFile_Local " + ex.Message + "\n" + ex.InnerException);

            }
            return retDicData;
        }
        #endregion

        #region Audit

        public List<Deficiency_Audit_Ships> GetAuditShipsDeficincyGrid_Local_DB(string code)
        {
            List<Deficiency_Audit_Ships> ShipsList = new List<Deficiency_Audit_Ships>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotes + " WHERE Ship = '" + code + "'", conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<AuditNote> dbNotesList = dt.ToListof<AuditNote>();
                                Deficiency_Audit_Ships modal = new Deficiency_Audit_Ships();
                                modal.IAFId = dbNotesList.FirstOrDefault().InternalAuditFormId;
                                modal.Ship = code;
                                modal.ShipName = SessionManager.ShipName;
                                modal.OpenISMOBS = dbNotesList.Where(y => y.Type == "ISM-Observation" && y.Ship == code).Count();
                                modal.OpenISMNCNs = dbNotesList.Where(y => y.Type == "ISM-Non Conformity" && y.Ship == code).Count();
                                modal.OpenISPSOBS = dbNotesList.Where(y => y.Type == "ISPS-Observation" && y.Ship == code).Count();
                                modal.OpenISPSNCN = dbNotesList.Where(y => y.Type == "ISPS-Non Conformity" && y.Ship == code).Count();
                                modal.OpenMLCOBS = 0;
                                modal.OpenMLCNCNs = 0;
                                ShipsList.Add(modal);
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipDeficincyGrid_Local_DB " + ex.Message);
            }
            return ShipsList;
        }
        public List<Deficiency_Ship_Audits> GetShipAudits_Local_DB(string Code
            , bool blnIsSetWhereClauseWithNotEqual = false  // JSL 11/12/2022
            )
        {
            List<Deficiency_Ship_Audits> data = new List<Deficiency_Ship_Audits>();
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();  // JSL 04/19/2022 set at global
            bool blnIsInspector = dbConnModal.IsInspector;  // JSL 04/19/2022
            try
            {
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();

                            // JSL 11/12/2022
                            string strQuery = string.Empty;
                            if (blnIsSetWhereClauseWithNotEqual)
                                strQuery = "SELECT * FROM " + AppStatic.InternalAuditForm + " WHERE ShipName != '" + Code + "'";
                            else
                                strQuery = "SELECT * FROM " + AppStatic.InternalAuditForm + " WHERE ShipName = '" + Code + "' AND [SavedAsDraft] = 0 AND [isDelete] = 0 ORDER BY [Date] DESC, [InternalAuditFormId] DESC";  // JSL 02/15/2023 removed AND CAST(CreatedDate as DATE) != '04-10-2022'
                            // End JSL 11/12/2022

                            SqlDataAdapter sqlAdp = new SqlDataAdapter(strQuery, conn); // JSL 04/20/2022 set Desc by Date   // JSL 04/19/2022 AND CAST(CreatedDate as DATE) != '04-10-2022' AND [isDelete] = 0 // RDBJ 01/31/2022 Added [SavedAsDraft]
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<InternalAuditForm> AuditFormsList = dt.ToListof<InternalAuditForm>();
                                foreach (InternalAuditForm item in AuditFormsList)
                                {
                                    try
                                    {
                                        List<string> strSubject = new List<string>();   // RDBJ 01/31/2022
                                        Deficiency_Ship_Audits dbAudit = new Deficiency_Ship_Audits();
                                        dbAudit.InternalAuditFormId = item.InternalAuditFormId;
                                        dbAudit.UniqueFormID = item.UniqueFormID;

                                        // RDBJ 01/31/2022
                                        if (item.AuditTypeISM == true)
                                        {
                                            strSubject.Add("ISM");
                                        }
                                        if (item.AuditTypeISPS == true)
                                        {
                                            strSubject.Add("ISPS");
                                        }
                                        if (item.AuditTypeMLC == true)
                                        {
                                            strSubject.Add("MLC");
                                        }
                                        // End RDBJ 01/31/2022

                                        dbAudit.Subject = String.Join(", ", strSubject.ToArray()); // RDBJ 01/31/2022 set all type Audit //item.AuditTypeISM == true ? "ISM" : item.AuditTypeISPS == true ? "ISPS" : item.AuditTypeMLC == true ? "MLC" : string.Empty;
                                        
                                        dbAudit.Type = Convert.ToString(item.AuditType); // RDBJ 01/31/2022 set Dynamic //"Internal";
                                        dbAudit.Extra = item.IsAdditional;  // RDBJ 01/31/2022 Set Dynamic
                                        //dbAudit.AuditDate = Utility.ToDateTimeStr(item.Date); // RDBJ 04/01/2022 commented this line
                                        dbAudit.AuditDate = item.Date; // RDBJ 04/01/2022
                                        dbAudit.Location = item.Location;
                                        dbAudit.Auditor = item.Auditor;

                                        DataTable dtAuditNotes = new DataTable();
                                        sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + item.UniqueFormID + "'", conn);
                                        sqlAdp.Fill(dtAuditNotes);
                                        List<AuditNote> AuditNotesList = dtAuditNotes.ToListof<AuditNote>();

                                        dbAudit.NCN = AuditNotesList.Where(y => y.UniqueFormID == item.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "ISM-Non Conformity" || z.Type == "ISPS-Non Conformity").ToList().Count; // RDBJ 01/31/2022 Updated with UniqueFormID
                                        dbAudit.OutstandingNCN = AuditNotesList.Where(y => y.UniqueFormID == item.UniqueFormID && y.isDelete == 0 && y.IsResolved == false).ToList().Where(z => z.Type == "ISM-Non Conformity" || z.Type == "ISPS-Non Conformity").ToList().Count; // RDBJ 01/31/2022 Updated with UniqueFormID
                                        dbAudit.OBS = AuditNotesList.Where(y => y.UniqueFormID == item.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "ISM-Observation" || z.Type == "ISPS-Observation").ToList().Count; // RDBJ 01/31/2022 Updated with UniqueFormID
                                        dbAudit.OutstandingOBS = AuditNotesList.Where(y => y.UniqueFormID == item.UniqueFormID && y.isDelete == 0 && y.IsResolved == false).ToList().Where(z => z.Type == "ISM-Observation" || z.Type == "ISPS-Observation").ToList().Count; // RDBJ 01/31/2022 Updated with UniqueFormID
                                        // RDBJ 01/31/2022
                                        dbAudit.MLC = AuditNotesList.Where(y => y.UniqueFormID == item.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "MLC-Deficiency").ToList().Count;
                                        dbAudit.OutstandingMLC = AuditNotesList.Where(y => y.UniqueFormID == item.UniqueFormID && y.isDelete == 0 && (y.IsResolved == null || y.IsResolved == false)).ToList().Where(z => z.Type == "MLC-Deficiency").ToList().Count;
                                        // End RDBJ 01/31/2022

                                        dbAudit.Closed = item.IsClosed; // RDBJ 01/31/2022 Set Dynamic
                                        data.Add(dbAudit);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.writelog(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipAudits_Local_DB Error : " + ex.Message);
            }

            // JSL 04/19/2022
            //if (!blnIsInspector)  // JSL 01/10/2023 commented
            if (false)  // JSL 01/10/2023
            {
                data = data.Where(x => x.OutstandingNCN != 0
                        || x.OutstandingOBS != 0
                        || x.OutstandingMLC != 0
                        )
                        .ToList();
            }
            // End JSL 04/19/2022

            return data
                .OrderByDescending(x => x.AuditDate).ToList(); // RDBJ 04/01/2022
        }
        public IAF GetAuditDetails_Local_DB(Guid? id)
        {
            IAF data = new IAF();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.InternalAuditForm + " WHERE UniqueFormID = '" + id + "'", conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<InternalAuditForm> AuditFormsList = dt.ToListof<InternalAuditForm>();
                                data.InternalAuditForm = AuditFormsList[0];

                                DataTable dtAuditNotes = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotes + " WHERE UniqueFormID = '" + data.InternalAuditForm.UniqueFormID + "'", conn);
                                sqlAdp.Fill(dtAuditNotes);
                                List<AuditNote> AuditNotesList = dtAuditNotes.ToListof<AuditNote>();
                                data.AuditNote = new List<AuditNote>();
                                foreach (AuditNote item in AuditNotesList)
                                {
                                    AuditNote obj = new AuditNote();
                                    obj.BriefDescription = item.BriefDescription;
                                    obj.CorrectiveAction = item.CorrectiveAction;
                                    obj.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                    obj.FullDescription = obj.FullDescription;
                                    obj.InternalAuditFormId = data.InternalAuditForm.InternalAuditFormId;
                                    obj.UniqueFormID = data.InternalAuditForm.UniqueFormID;
                                    obj.Name = item.Name;
                                    obj.Number = item.Number;
                                    obj.PreventativeAction = item.PreventativeAction;
                                    obj.Rank = item.Rank;
                                    obj.Reference = item.Reference;
                                    obj.TimeScale = item.TimeScale;
                                    obj.Type = item.Type;
                                    obj.AuditNotesId = item.AuditNotesId;
                                    obj.IsResolved = item.IsResolved == null || item.IsResolved == Convert.ToBoolean(0) ? false : item.IsResolved;
                                    obj.NotesUniqueID = item.NotesUniqueID;
                                    obj.AuditNotesAttachment = new List<AuditNotesAttachment>();

                                    DataTable dtAuditNotesAttachs = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesAttachment + " WHERE NotesUniqueID = '" + item.NotesUniqueID + "'", conn);
                                    sqlAdp.Fill(dtAuditNotesAttachs);

                                    List<AuditNotesAttachment> AuditNotesAttachmentList = dtAuditNotesAttachs.ToListof<AuditNotesAttachment>();
                                    foreach (var noteAttach in AuditNotesAttachmentList)
                                    {
                                        Modals.AuditNotesAttachment objAuditNotesAttachment = new Modals.AuditNotesAttachment();
                                        objAuditNotesAttachment.NotesFileUniqueID = noteAttach.NotesFileUniqueID;
                                        objAuditNotesAttachment.FileName = noteAttach.FileName;
                                        obj.AuditNotesAttachment.Add(objAuditNotesAttachment);
                                    }
                                    data.AuditNote.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditDetails_Local_DB Error : " + ex.Message);
            }
            return data;
        }
        public bool AddAuditDeficiencyComments_Local_DB(Audit_Deficiency_Comments data)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AuditNotesComments);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AuditNotesComments); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string UniqueFormID = string.Empty;
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID FROM " + AppStatic.AuditNotes + " WHERE NotesUniqueID = '" + data.NotesUniqueID + "'", connection);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            UniqueFormID = Convert.ToString(dt.Rows[0][0]);
                        }

                        string InsertQuery = AuditDeficiencyComments_InsertQuery();
                        connection.Open();
                        SqlCommand command = new SqlCommand(InsertQuery, connection);

                        // JSL 01/08/2023 wrapped in if
                        if (data.CommentUniqueID == null || data.CommentUniqueID == Guid.Empty)
                            data.CommentUniqueID = Guid.NewGuid();

                        AuditDeficiencyComments_CMD(data, ref command);
                        command.ExecuteScalar();
                        connection.Close();
                        if (data.AuditDeficiencyCommentsFiles != null && data.AuditDeficiencyCommentsFiles.Count > 0)
                        {
                            AddAuditDeficiencyCommentsFiles_Local_DB(data.AuditDeficiencyCommentsFiles, data.CommentUniqueID);
                        }
                        res = true;
                        UpdateGIRSyncStatus_Local_DB(UniqueFormID, "IAF"); //RDBJ 10/04/2021
                        UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB(Convert.ToString(data.NotesUniqueID), "IAF"); //RDBJ 11/10/2021
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesNote_Local_DB Error : " + ex.InnerException.ToString());
                res = false;
            }
            return res;
        }
        public bool AddAuditDeficiencyCommentsFiles_Local_DB(List<Audit_Deficiency_Comments_Files> data, Guid? CommentUniqueID)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.AuditNotesCommentsFiles);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.AuditNotesCommentsFiles); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        foreach (var item in data)
                        {
                            string InsertQuery = AuditDeficiencyCommentsFiles_InsertQuery();
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            connection.Open();
                            SqlCommand command = new SqlCommand(InsertQuery, connection);

                            item.CommentFileUniqueID = Guid.NewGuid();
                            item.CommentUniqueID = CommentUniqueID;
                            
                            AuditDeficiencyCommentsFiles_CMD(item, ref command);
                            command.ExecuteScalar();
                            connection.Close();
                        }
                        
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddAuditDeficiencyCommentsFiles_Local_DB Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public List<Audit_Deficiency_Comments> GetAuditDeficiencyComments_Local_DB(Guid? NotesUniqueID)
        {
            List<Audit_Deficiency_Comments> list = new List<Audit_Deficiency_Comments>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.AuditNotesComments + " WHERE NotesUniqueID = '" + NotesUniqueID + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<Audit_Deficiency_Comments> dbAuditNotesCommentsList = dt.ToListof<Audit_Deficiency_Comments>();
                                foreach (var item in dbAuditNotesCommentsList)
                                {
                                    Audit_Deficiency_Comments obj = new Audit_Deficiency_Comments();
                                    obj.AuditDeficiencyCommentsFiles = new List<Audit_Deficiency_Comments_Files>();

                                    DataTable dtAudit_Deficiency_Comments_Files = new DataTable();
                                    sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.AuditNotesCommentsFiles + " WHERE CommentUniqueID = '" + item.CommentUniqueID + "'", conn);
                                    sqlAdp.Fill(dtAudit_Deficiency_Comments_Files);
                                    List<Audit_Deficiency_Comments_Files> defCommentFiles = dtAudit_Deficiency_Comments_Files.ToListof<Audit_Deficiency_Comments_Files>();

                                    foreach (var subitem in defCommentFiles)
                                    {
                                        Audit_Deficiency_Comments_Files filedata = new Audit_Deficiency_Comments_Files();
                                        filedata.CommentFileUniqueID = subitem.CommentFileUniqueID;
                                        filedata.FileName = subitem.FileName;
                                        //filedata.StorePath = subitem.StorePath;   // JSL 06/16/2022 commented this line to avoid maxJson issue
                                        obj.AuditDeficiencyCommentsFiles.Add(filedata);
                                    }
                                    obj.CommentsID = item.CommentsID;
                                    obj.CommentUniqueID = item.CommentUniqueID;
                                    obj.NotesUniqueID = item.NotesUniqueID;
                                    obj.AuditNoteID = item.AuditNoteID;
                                    obj.UserName = item.UserName;
                                    obj.Comment = item.Comment;
                                    obj.CreatedDate = item.CreatedDate;
                                    obj.isNew = item.isNew; //RDBJ 10/26/2021
                                    list.Add(obj);
                                }
                                list = list.OrderByDescending(x => x.CreatedDate).ToList();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditDeficiencyComments_Local_DB " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public List<Audit_Deficiency_Comments_Files> GetAuditDeficiencyCommentFiles_Local_DB(Guid? commentUniqueID)
        {
            List<Audit_Deficiency_Comments_Files> CommentFiles = new List<Audit_Deficiency_Comments_Files>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string query = "SELECT * FROM " + AppStatic.AuditNotesCommentsFiles + " CommentUniqueID AuditNoteID = " + commentUniqueID;
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(query, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<Audit_Deficiency_Comments_Files> dbAuditNotesCommentsFilesList = dt.ToListof<Audit_Deficiency_Comments_Files>();
                                foreach (var item in dbAuditNotesCommentsFilesList)
                                {
                                    Audit_Deficiency_Comments_Files filedata = new Audit_Deficiency_Comments_Files();
                                    filedata.CommentFileID = item.CommentFileID;
                                    filedata.CommentUniqueID = item.CommentUniqueID;
                                    filedata.CommentFileUniqueID = item.CommentFileUniqueID;
                                    filedata.CommentsID = item.CommentsID;
                                    filedata.AuditNoteID = item.AuditNoteID;
                                    filedata.FileName = item.FileName;
                                    filedata.StorePath = item.StorePath;
                                    CommentFiles.Add(filedata);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditDeficiencyCommentFiles_Local_DB" + ex.Message + "\n" + ex.InnerException);
            }
            return CommentFiles;
        }

        public bool UpdateAuditDeficiencies_Local_DB(string id, bool isClose) //RDBJ 10/05/2021 Set datatype int to String
        {
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    string UpdateQury = UpdateAuditNoteStatusQuery();
                    //if (isClose)
                    //    UpdateQury = "UPDATE " + AppStatic.AuditNotes + " SET IsResolved = 1, DateClosed = " + DateTime.Now + "  WHERE AuditNotesId = " + id;
                    //else
                    //    UpdateQury = "UPDATE " + AppStatic.AuditNotes + " SET IsResolved = 0, DateClosed = " + DateTime.Now + " WHERE AuditNotesId = " + id;

                    SqlConnection connection = new SqlConnection(connetionString);
                    SqlCommand command = new SqlCommand(UpdateQury, connection);
                    UpdateAuditNoteStatusQuery_CMD(id, isClose, ref command);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateAuditDeficiencies_Local_DB Error : " + ex.Message.ToString());
                return false;
            }
            return true;
        }
        public string UpdateAuditNoteStatusQuery()
        {
            //RDBJ 10/05/2021 set NotesUniqueID from AuditNotesId
            string UpdateQuery = @"UPDATE dbo.AuditNotes SET IsResolved = @IsResolved, DateClosed = @DateClosed
                                  WHERE NotesUniqueID = @NotesUniqueID";
            return UpdateQuery;
        }
        public void UpdateAuditNoteStatusQuery_CMD(string id, bool isClose, ref SqlCommand command) //RDBJ 10/05/2021 Set int to string 
        {
            command.Parameters.Add("@IsResolved", SqlDbType.BigInt).Value = Utility.ToLong(isClose);
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(id); //RDBJ 10/05/2021 set NotesUniqueID from AuditNotesId
        }

        public string AuditDeficiencyComments_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.AuditNotesComments 
                                  (AuditNoteID,UserName,Comment,CreatedDate,UpdatedDate,NotesUniqueID,CommentUniqueID, isNew)
                                  OUTPUT INSERTED.CommentsID
                                  VALUES (@AuditNoteID,@UserName,@Comment,@CreatedDate,@UpdatedDate,@NotesUniqueID,@CommentUniqueID,@isNew)";
            return InsertQuery;
        }
        public void AuditDeficiencyComments_CMD(Audit_Deficiency_Comments Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@AuditNoteID", SqlDbType.BigInt).Value = Utility.ToLong(Modal.AuditNoteID);
            command.Parameters.Add("@CommentUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentUniqueID;
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = Modal.UserName == null ? DBNull.Value : (object)Modal.UserName;
            command.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = Modal.Comment == null ? DBNull.Value : (object)Modal.Comment;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@isNew", SqlDbType.Int).Value = 0; //RDBJ 10/26/2021
        }
        public string AuditDeficiencyCommentsFiles_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.AuditNotesCommentsFiles 
                                  (CommentsID,AuditNoteID,FileName,StorePath,CommentUniqueID,CommentFileUniqueID)
                                  OUTPUT INSERTED.CommentFileID
                                  VALUES (@CommentsID,@AuditNoteID,@FileName,@StorePath,@CommentUniqueID,@CommentFileUniqueID)";
            return InsertQuery;
        }
        public void AuditDeficiencyCommentsFiles_CMD(Audit_Deficiency_Comments_Files Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@CommentsID", SqlDbType.BigInt).Value = Modal.CommentsID;
            command.Parameters.Add("@CommentUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentUniqueID;
            command.Parameters.Add("@CommentFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.CommentFileUniqueID;
            command.Parameters.Add("@AuditNoteID", SqlDbType.BigInt).Value = Modal.AuditNoteID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
        }

        #endregion

        #endregion

        #region Common
        // RDBJ2 05/10/2022
        public Dictionary<string, string> AjaxPostPerformAction(Dictionary<string, string> dictMetaData)
        {
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            string connectionString = Utility.GetLocalDBConnStr(dbConnModal);

            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();

            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                // RDBJ2 05/10/2022
                case AppStatic.API_DELETEDEFICIENCYFILE:
                    {
                        try
                        {
                            GIRTableHelper objGIRTabHelper = new GIRTableHelper();
                            long DeficienciesFileID = 0;
                            Guid DeficienciesUniqueID = Guid.Empty;
                            string strFormType = string.Empty;
                            bool blnIsDeleted = false;

                            string strTableName = string.Empty;
                            string strTableColumnsName = string.Empty;
                            string strTableFindColumnsName = string.Empty;
                            string strUniqueFormID = string.Empty;
                            string strReportType = string.Empty;

                            // JSL 06/04/2022
                            bool IsDeleteFromSection = false;
                            Guid deficienciesFileUniqueID = new Guid(); // JSL 06/04/2022

                            if (dictMetaData.ContainsKey("DeficienciesFileUniqueID"))
                                deficienciesFileUniqueID = Guid.Parse(dictMetaData["DeficienciesFileUniqueID"].ToString());

                            if (dictMetaData.ContainsKey("IsDeleteFromSection"))
                                IsDeleteFromSection = Convert.ToBoolean(dictMetaData["IsDeleteFromSection"].ToString());
                            // End JSL 06/04/2022

                            if (dictMetaData.ContainsKey("DeficienciesFileID"))
                                DeficienciesFileID = Convert.ToInt64(dictMetaData["DeficienciesFileID"]);

                            if (dictMetaData.ContainsKey("DeficienciesUniqueID"))
                                DeficienciesUniqueID = Guid.Parse(dictMetaData["DeficienciesUniqueID"].ToString());

                            if (dictMetaData.ContainsKey("FormType"))
                                strFormType = dictMetaData["FormType"].ToString();

                            if (strFormType != AppStatic.IAFForm)
                            {
                                strTableName = AppStatic.GIRDeficiencies;
                                strTableColumnsName = "UniqueFormID, ReportType";
                                strTableFindColumnsName = "DeficienciesUniqueID";

                                // JSL 06/04/2022 wrraped in if
                                if (IsDeleteFromSection)
                                {
                                    blnIsDeleted = objGIRTabHelper.DeleteRecords(AppStatic.GIRDeficienciesFiles, "DeficienciesFileUniqueID", Convert.ToString(deficienciesFileUniqueID));
                                }
                                else
                                {
                                    blnIsDeleted = objGIRTabHelper.DeleteRecords(AppStatic.GIRDeficienciesFiles, "GIRDeficienciesFileID", Convert.ToString(DeficienciesFileID));
                                }

                            }
                            else
                            {
                                strTableName = AppStatic.AuditNotes;
                                strTableColumnsName = "UniqueFormID";
                                strTableFindColumnsName = "NotesUniqueID";
                                blnIsDeleted = objGIRTabHelper.DeleteRecords(AppStatic.AuditNotesAttachment, "AuditNotesAttachmentId", Convert.ToString(DeficienciesFileID));
                            }

                            SqlConnection connection = new SqlConnection(connectionString);
                            DataTable dtDeficiency = new DataTable();
                            string getDefQuery = string.Empty;

                            getDefQuery = "SELECT " + strTableColumnsName + " FROM " + strTableName + " WHERE " + strTableFindColumnsName + " = '" + DeficienciesUniqueID + "'";

                            connection.Open();
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(getDefQuery, connection);
                            sqlAdp.Fill(dtDeficiency);
                            if (dtDeficiency != null && dtDeficiency.Rows.Count > 0)
                            {
                                strUniqueFormID = dtDeficiency.Rows[0][0].ToString();

                                if (strFormType != AppStatic.IAFForm)
                                    strReportType = dtDeficiency.Rows[0][1].ToString();
                                else
                                    strReportType = AppStatic.IAFForm;
                            }
                            connection.Close();

                            string strFormVersion = UpdateGIRSyncStatus_Local_DB(strUniqueFormID, strReportType);
                            // JSL 06/04/2022
                            retDictMetaData["FormVersion"] = strFormVersion;
                            if (IsDeleteFromSection)
                                retDictMetaData["DeficienciesFileUniqueID"] = dictMetaData["DeficienciesFileUniqueID"].ToString();
                            else
                                retDictMetaData["DeficienciesFileID"] = dictMetaData["DeficienciesFileID"].ToString();

                            retDictMetaData["DeficienciesUniqueID"] = dictMetaData["DeficienciesUniqueID"].ToString();
                            retDictMetaData["IsDeleteFromSection"] = IsDeleteFromSection.ToString().ToLower();

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_DELETEDEFICIENCYFILE + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ2 05/10/2022
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
        // End RDBJ2 05/10/2022
        #endregion
    }
}
