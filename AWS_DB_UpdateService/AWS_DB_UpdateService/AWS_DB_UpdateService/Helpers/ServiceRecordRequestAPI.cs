//using AWS_DB_UpdateService.Modals;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
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
//    public class ServiceRecordRequestAPI
//    {
//        public void Import_ServiceRecordRequest_API(dsisy01 dbUser)
//        {
//            bool isCreated = Create_XML_ServiceRecordRequest(dbUser);
//            if (isCreated)
//            {
//                CookieContainer cookies = new CookieContainer();
//                LoginRes lRes = APIHelper.Login(dbUser, ref cookies);
//                if (lRes != null && lRes.Response != null && lRes.Response.Status == "SUCCESS")
//                {
//                    UploadFileRes reqResModal = Upload(cookies, dbUser);
//                    UploadFileResult uploadResult = GetResponseDataResult(reqResModal);
//                    if (uploadResult != null)
//                    {
//                        int TaskID = Utility.ToInteger(uploadResult.Content);
//                        if (TaskID > 0)
//                        {
//                            UploadFileRes reqStatusModal = APIHelper.CheckRequestStatus(TaskID, cookies);
//                            UploadFileResult uploadStatusResult = GetResponseDataResult(reqStatusModal);
//                            if (uploadStatusResult != null) { }
//                            //if (resModal != null && resModal.Response != null)
//                            //{
//                            //    if (resModal.Response.Status == "SUCCESS")
//                            //    {
//                            //        if (resModal.Response.ResponseData != null && resModal.Response.ResponseData.Result != null)
//                            //        {
//                            //            string Type = resModal.Response.ResponseData.Result.Type;
//                            //            string Format = resModal.Response.ResponseData.Result.Format;
//                            //            string Content = resModal.Response.ResponseData.Result.Content;
//                            //            if (Content == "Completed")
//                            //            {
//                            //                res = true;
//                            //            }
//                            //            else
//                            //            {
//                            //                //GetTicketResults(TaskID, cookies);
//                            //            }
//                            //        }
//                            //    }
//                            //}
//                        }
//                    }
//                }
//            }
//        }
//        public bool Create_XML_ServiceRecordRequest(dsisy01 dbUser)
//        {
//            try
//            {
//                XmlDocument xmlEmloyeeDoc = new XmlDocument();
//                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + Utility.ToSTRING(dbUser.empnry01.Trim()) + "_Update" + ".xml";
//                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

//                XmlDocument doc = new XmlDocument();
//                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
//                doc.AppendChild(docNode);

//                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUserServiceRecord");
//                doc.AppendChild(ESDI_tblUserNode);

//                XmlNode tblUserNode = doc.CreateElement("tblUserServiceRecord");

//                XmlNode vUserID = doc.CreateElement("vExternalID");
//                vUserID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
//                tblUserNode.AppendChild(vUserID);

//                XmlNode vFirstname = doc.CreateElement("vUserID");
//                vFirstname.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
//                tblUserNode.AppendChild(vFirstname);

//                XmlNode vLastname = doc.CreateElement("iInstallationID");
//                vLastname.InnerText = "107522";
//                tblUserNode.AppendChild(vLastname);

//                XmlNode dtBirthdate = doc.CreateElement("dtSignOn");
//                dtBirthdate.InnerText = "2019-01-01";
//                tblUserNode.AppendChild(dtBirthdate);

//                XmlNode uRankID = doc.CreateElement("dtSignOff");
//                uRankID.InnerText = "2019-03-15";
//                tblUserNode.AppendChild(uRankID);

//                XmlNode vExternalID = doc.CreateElement("uRankID");
//                vExternalID.InnerText = "7521306F-349B-41DA-99F8-A8BA03286DF1";
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
//        public UploadFileRes Upload(CookieContainer cookies, dsisy01 dbUser)
//        {
//            string fileUrl = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + Utility.ToSTRING(dbUser.empnry01.Trim()) + "_Update" + ".xml";
//            if (File.Exists(fileUrl))
//            {
//                try
//                {
//                    string replyFromServer = string.Empty;
//                    var cookieContainer = new CookieContainer();
//                    using (var handler = new HttpClientHandler() { CookieContainer = cookies })
//                    using (var client = new HttpClient(handler))
//                    {
//                        MultipartFormDataContent form = new MultipartFormDataContent();
//                        HttpContent content = new StringContent("content");
//                        form.Add(content, "content");
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

//                        var url = new Uri("https://stacdn.seagull.no/STA40WEBAPI/Execute/R00040");
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
//        public UploadFileResult GetResponseDataResult(UploadFileRes ResModal)
//        {
//            UploadFileResult res = new UploadFileResult();
//            if (ResModal != null && ResModal.Response != null)
//            {
//                if (ResModal.Response.Status == "SUCCESS")
//                {
//                    if (ResModal.Response.ResponseData != null && ResModal.Response.ResponseData.Result != null)
//                    {
//                        res.Type = ResModal.Response.ResponseData.Result.Type;
//                        res.Format = ResModal.Response.ResponseData.Result.Format;
//                        res.Content = ResModal.Response.ResponseData.Result.Content;
//                        return res;
//                    }
//                }
//            }
//            return null;
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
//    }
//}
