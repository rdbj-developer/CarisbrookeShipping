using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.BLL.Helpers;
using System.Xml;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class CustomController : Controller
    {
        // GET: Custom
        public ActionResult Index()
        {
            return View();
        }


        #region Sync Documnets
        public ActionResult SyncDocuments()
        {
            string DocFilePath = Server.MapPath("~/Repository/DocumentsIndex.xml");
            List<DocumentsModal> AllDocs = new List<DocumentsModal>();
            DocumentsHelper _helper = new DocumentsHelper();
            AllDocs.AddRange(GetDocumentsMainCategories(DocFilePath));
            AllDocs.AddRange(GetDocumentsSubCategories(DocFilePath));
            bool res = _helper.AddSyncDocuments(AllDocs);
            if (res == true)
                return RedirectToAction("Index", "Custom");
            else
                return View();
        }
        public List<DocumentsModal> GetDocumentsMainCategories(string FilePath)
        {
            XmlDocument doc = new XmlDocument();
            List<DocumentsModal> ModalList = new List<DocumentsModal>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("DocumentFolder");
            foreach (XmlNode node in nodesList)
            {
                DocumentsModal Modal = new DocumentsModal();
                string DocumentID = node.Attributes["Id"].InnerText;
                Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                Modal.DocumentID = newGuid;
                Modal.Type = "FOLDER";
                Modal.IsDeleted = false;
                Modal.Version = 1.0;
                Modal.DocumentVersion = 1.0;
                Modal.UploadType = AppStatic.NEW;
                Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                foreach (XmlNode chldNode in node.ChildNodes)
                {
                    if (chldNode.Name == "ParentId")
                    {
                        string ParentId = chldNode.InnerText.Trim();
                        newGuid = Guid.Parse(Convert.ToString(ParentId));
                        Modal.ParentID = newGuid;
                    }
                    if (chldNode.Name == "Number")
                    {
                        string Number = chldNode.InnerText.Trim();
                        Modal.Number = Convert.ToString(Number).Trim();
                    }
                    if (chldNode.Name == "Title")
                    {
                        string Title = chldNode.InnerText.Trim();
                        Modal.Title = Convert.ToString(Title).Trim();
                    }
                    if (chldNode.Name == "Path")
                    {
                        string Path = chldNode.InnerText.Trim();
                        Modal.Path = Convert.ToString(Path).Trim();
                        string DownloadPath = Modal.Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                        Modal.DownloadPath = DownloadPath;
                    }
                }
                ModalList.Add(Modal);
            }
            return ModalList;
        }
        public List<DocumentsModal> GetDocumentsSubCategories(string FilePath)
        {
            XmlDocument doc = new XmlDocument();
            List<DocumentsModal> ModalList = new List<DocumentsModal>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Document");
            foreach (XmlNode node in nodesList)
            {
                DocumentsModal Modal = new DocumentsModal();
                string DocumentID = node.Attributes["Id"].InnerText;
                Guid newGuid = Guid.Parse(Convert.ToString(DocumentID));
                Modal.DocumentID = newGuid;
                Modal.IsDeleted = false;
                Modal.Version = 1.0;
                Modal.DocumentVersion = 1.0;
                Modal.UploadType = AppStatic.NEW;
                Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                foreach (XmlNode chldNode in node.ChildNodes)
                {
                    if (chldNode.Name == "ParentId")
                    {
                        string ParentId = chldNode.InnerText.Trim();
                        newGuid = Guid.Parse(Convert.ToString(ParentId));
                        Modal.ParentID = newGuid;
                    }
                    if (chldNode.Name == "Number")
                    {
                        string Number = chldNode.InnerText.Trim();
                        Modal.Number = Convert.ToString(Number).Trim();
                    }
                    if (chldNode.Name == "Title")
                    {
                        string Title = chldNode.InnerText.Trim();
                        Modal.Title = Convert.ToString(Title).Trim();
                    }
                    if (chldNode.Name == "Path")
                    {
                        string Path = chldNode.InnerText.Trim();
                        Modal.Path = Convert.ToString(Path).Trim();
                        string DownloadPath = Modal.Path.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
                        Modal.DownloadPath = DownloadPath;
                    }
                }
                string res = System.IO.Path.GetExtension(Modal.Path);
                res = res.Replace(".", "").ToUpper();
                Modal.Type = res;
                ModalList.Add(Modal);
            }
            return ModalList;
        }
        #endregion
    }
}