//using AWS_DB_UpdateService.Modals;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace AWS_DB_UpdateService.Helpers
//{
//    public class ImportPeopleAPI
//    {
//        public void ImportPeople_API(dsisy01 dbUser)
//        {
//            bool isCreated = Create_XML_ImportPeople(dbUser);
//            if (isCreated)
//            {
//                CookieContainer cookies = new CookieContainer();
//                LoginRes lRes = Login(dbUser, ref cookies);
//                if (lRes != null && lRes.Response != null && lRes.Response.Status == "SUCCESS")
//                {
//                    UploadFileRes resModal = Upload(cookies, dbUser);
//                    if (resModal != null && resModal.Response != null)
//                    {
//                        if (resModal.Response.Status == "SUCCESS")
//                        {
//                            if (resModal.Response.ResponseData != null && resModal.Response.ResponseData.Result != null)
//                            {
//                                string Type = resModal.Response.ResponseData.Result.Type;
//                                string Format = resModal.Response.ResponseData.Result.Format;
//                                string Content = resModal.Response.ResponseData.Result.Content;

//                                int TaskID = Utility.ToInteger(Content);
//                                if (TaskID > 0)
//                                {
//                                    CheckRequestStatus(TaskID, cookies);
//                                }
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    LogHelper.writelog("Login Failed");
//                }
//            }
//        }
//        public LoginRes Login(dsisy01 dbUser, ref  CookieContainer cookies)
//        {
//            try
//            {
//                string company = Utility.ToSTRING(ConfigurationManager.AppSettings["company"]);
//                string user = Utility.ToSTRING(ConfigurationManager.AppSettings["user"]);
//                string password = Utility.ToSTRING(ConfigurationManager.AppSettings["password"]);

//                var request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/Login");
//                var postData = "company=" + company;
//                postData += "&user=" + user;
//                postData += "&password=" + password;
//                var data = Encoding.ASCII.GetBytes(postData);

//                request.Method = "POST";
//                request.ContentType = "application/x-www-form-urlencoded";
//                request.ContentLength = data.Length;
//                request.CookieContainer = cookies;

//                using (var stream = request.GetRequestStream())
//                {
//                    stream.Write(data, 0, data.Length);
//                }

//                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

//                StreamReader responseReader = new StreamReader(response.GetResponseStream());
//                string responseData = responseReader.ReadToEnd();
//                string res = responseData;

//                // To convert an XML node contained in string xml into a JSON string   
//                XmlDocument doc = new XmlDocument();
//                doc.LoadXml(res);
//                string jsonText = JsonConvert.SerializeXmlNode(doc);
//                LoginRes resModal = JsonConvert.DeserializeObject<LoginRes>(jsonText);
//                return resModal;
//            }
//            catch (Exception ex)
//            {
//                LogHelper.writelog("Login Error: " + ex.Message);
//                return null;
//            }
//        }
//        public UploadFileRes Upload(CookieContainer cookies, dsisy01 dbUser)
//        {
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/Execute/R00010");
//            string fileUrl = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + Utility.ToSTRING(dbUser.empnry01.Trim()) + ".xml";
//            if (File.Exists(fileUrl))
//            {
//                try
//                {
//                    string replyFromServer = string.Empty;
//                    var baseAddress = new Uri("https://stacdn.seagull.no/STA40WEBAPI/Execute/R00010");
//                    var cookieContainer = new CookieContainer();
//                    using (var handler = new HttpClientHandler() { CookieContainer = cookies })
//                    using (var client = new HttpClient(handler))
//                    {
//                        MultipartFormDataContent form = new MultipartFormDataContent();
//                        HttpContent content = new StringContent("cont");
//                        form.Add(content, "cont");
//                        var stream = new FileStream(fileUrl, FileMode.Open);
//                        content = new StreamContent(stream);
//                        var fileName =
//                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
//                        {
//                            Name = "Data",
//                            FileName = Path.GetFileName(fileUrl),
//                        };
//                        form.Add(content);
//                        HttpResponseMessage response = null;

//                        var url = new Uri("https://stacdn.seagull.no/STA40WEBAPI/Execute/R00010");
//                        response = (client.PostAsync(url, form)).Result;

//                        replyFromServer = response.Content.ReadAsStringAsync().Result;
//                    }

//                    XmlDocument doc = new XmlDocument();
//                    doc.LoadXml(replyFromServer);
//                    string jsonText = JsonConvert.SerializeXmlNode(doc);
//                    UploadFileRes resModal = JsonConvert.DeserializeObject<UploadFileRes>(jsonText);
//                    return resModal;
//                }
//                catch (Exception ex)
//                {
//                    LogHelper.writelog(ex.Message);
//                }
//            }
//            else
//            {
//                LogHelper.writelog(fileUrl + " File not exist for upload");
//            }
//            return null;
//        }
//        public bool CheckRequestStatus(int TaskID, CookieContainer cookies)
//        {
//            bool res = false;
//            try
//            {
//                var request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/CheckRequestStatus?taskid=" + TaskID);
//                request.CookieContainer = cookies;
//                var response = (HttpWebResponse)request.GetResponse();
//                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
//                string responseSTR = responseString;
//                XmlDocument doc = new XmlDocument();
//                doc.LoadXml(responseSTR);
//                string jsonText = JsonConvert.SerializeXmlNode(doc);
//                UploadFileRes resModal = JsonConvert.DeserializeObject<UploadFileRes>(jsonText);
//                if (resModal != null && resModal.Response != null)
//                {
//                    if (resModal.Response.Status == "SUCCESS")
//                    {
//                        if (resModal.Response.ResponseData != null && resModal.Response.ResponseData.Result != null)
//                        {
//                            string Type = resModal.Response.ResponseData.Result.Type;
//                            string Format = resModal.Response.ResponseData.Result.Format;
//                            string Content = resModal.Response.ResponseData.Result.Content;
//                            if (Content == "Completed")
//                            {
//                                res = true;
//                            }
//                            else
//                            {
//                                GetTicketResults(TaskID, cookies);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogHelper.writelog(ex.Message);
//            }
//            return res;
//        }
//        public void GetTicketResults(int TaskID, CookieContainer cookies)
//        {
//            try
//            {
//                var request = (HttpWebRequest)WebRequest.Create("https://stacdn.seagull.no/STA40WEBAPI/GetTicketResults?TaskID=" + TaskID);
//                request.CookieContainer = cookies;
//                var response = (HttpWebResponse)request.GetResponse();
//                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
//                string res = responseString;
//                XmlDocument doc = new XmlDocument();
//                doc.LoadXml(res);
//                string jsonText = JsonConvert.SerializeXmlNode(doc);
//                UploadFileRes resModal = JsonConvert.DeserializeObject<UploadFileRes>(jsonText);
//                if (resModal != null && resModal.Response != null)
//                {
//                    if (resModal.Response.Status == "SUCCESS")
//                    {
//                        //UpdateDB(dbUser);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogHelper.writelog(ex.Message);
//            }
//        }
//        public bool Create_XML_ImportPeople(dsisy01 dbUser)
//        {
//            try
//            {
//                XmlDocument xmlEmloyeeDoc = new XmlDocument();
//                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + Utility.ToSTRING(dbUser.empnry01.Trim()) + ".xml";
//                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

//                XmlDocument doc = new XmlDocument();
//                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
//                doc.AppendChild(docNode);

//                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUser");
//                doc.AppendChild(ESDI_tblUserNode);

//                XmlNode tblUserNode = doc.CreateElement("tblUser");

//                XmlNode vUserID = doc.CreateElement("vUserID");
//                vUserID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
//                tblUserNode.AppendChild(vUserID);

//                XmlNode vFirstname = doc.CreateElement("vFirstname");
//                vFirstname.InnerText = dbUser.famname;
//                tblUserNode.AppendChild(vFirstname);

//                XmlNode vLastname = doc.CreateElement("vLastname");
//                vLastname.InnerText = dbUser.givenname;
//                tblUserNode.AppendChild(vLastname);

//                XmlNode dtBirthdate = doc.CreateElement("dtBirthdate");
//                dtBirthdate.InnerText = "2006-05-07";
//                tblUserNode.AppendChild(dtBirthdate);

//                XmlNode uRankID = doc.CreateElement("uRankID");
//                uRankID.InnerText = "7521306F-349B-41DA-99F8-A8BA03286DF1";
//                tblUserNode.AppendChild(uRankID);

//                XmlNode vSex = doc.CreateElement("vSex");
//                vSex.InnerText = "M";
//                tblUserNode.AppendChild(vSex);

//                XmlNode vPlaceOfBirth = doc.CreateElement("vPlaceOfBirth");
//                vPlaceOfBirth.InnerText = "Aden";
//                tblUserNode.AppendChild(vPlaceOfBirth);

//                XmlNode uCountryID = doc.CreateElement("uCountryID");
//                uCountryID.InnerText = "8D9D911A-C538-42CD-A123-2F35C7133F3E";
//                tblUserNode.AppendChild(uCountryID);

//                XmlNode vDocumentNo = doc.CreateElement("vDocumentNo");
//                vDocumentNo.InnerText = "4221234";
//                tblUserNode.AppendChild(vDocumentNo);

//                XmlNode vEmail = doc.CreateElement("vEmail");
//                vEmail.InnerText = "adam.smith.sample@gmail.com";
//                tblUserNode.AppendChild(vEmail);

//                XmlNode bActive = doc.CreateElement("bActive");
//                bActive.InnerText = "True";
//                tblUserNode.AppendChild(bActive);

//                XmlNode vExternalID = doc.CreateElement("vExternalID");
//                vExternalID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
//                tblUserNode.AppendChild(vExternalID);

//                ESDI_tblUserNode.AppendChild(tblUserNode);
//                doc.Save(xmlfilePath);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                LogHelper.writelog(ex.Message);
//                return false;
//            }
//        }
//        public void UpdateDB(dsisy01 dbUser)
//        {
//            try
//            {
//                string connetionString = Utility.GetDBConnStr();
//                LogHelper.writelog(connetionString);
//                using (var conn = new SqlConnection(connetionString))
//                {
//                    if (conn.IsAvailable())
//                    {
//                        LogHelper.writelog("Connection Done");
//                        conn.Open();
//                        DataTable dt = new DataTable();
//                        string query = "UPDATE dsisy01 WHERE empnry01 = '" + dbUser.empnry01 + "'";
//                        SqlCommand sqlcom = new SqlCommand(query, conn);
//                        sqlcom.ExecuteNonQuery();
//                        conn.Close();
//                        LogHelper.writelog(Utility.ToSTRING(dbUser.empnry01.Trim()) + " Record Inserted Successfully");
//                    }
//                    else
//                    {
//                        LogHelper.writelog("SQL Connection is not available");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogHelper.writelog(Utility.ToSTRING(dbUser.empnry01.Trim()) + " Record Inserted Failed!!!");
//                LogHelper.writelog(ex.Message);
//            }
//        }
//    }
//}
