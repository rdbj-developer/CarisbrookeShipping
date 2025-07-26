using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class AdminController : Controller
    {
        #region Properties
        List<DocumentModal> docList = new List<DocumentModal>();
        List<FormModal> formList = new List<FormModal>();
        #endregion

        #region Index/TabView

        public ActionResult Index()
        {
            try
            {
                ViewBag.IsPageInit = TempData["isPageInit"];
            }
            catch (Exception)
            {
            }
            return View();
        }
        #endregion

        #region Documents

        public ActionResult ManageDocuments()
        {
            APIHelper _helper = new APIHelper();
            try
            {
                docList = _helper.GetAllDocuments("ISM");
                docList = docList.OrderBy(x => x.SectionType).ThenBy(x => Utility.ToLong(x.Number)).ToList();
                var sb = new StringBuilder();
                ListDirectories(Guid.Empty, sb);
                string res = sb.ToString();
                ViewBag.TreeView = res;
                if (TempData["Result"] != null && Utility.ToString(TempData["Result"]) != "")
                {
                    TempData["Result"] = TempData["Result"];
                    TempData["ClickedType"] = TempData["ClickedType"];
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return View();
        }
        public string DocumentsTreeView(string sectionType = "ISM")
        {
            string res = "";
            APIHelper _helper = new APIHelper();
            try
            {
                docList = _helper.GetAllDocuments(sectionType);
                docList = docList.OrderBy(x => x.SectionType).ThenBy(x => Utility.ToLong(x.Number)).ToList();
                var sb = new StringBuilder();
                ListDirectories(Guid.Empty, sb);
                res = sb.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        void ListDirectories(Guid? path, StringBuilder sb)
        {
            var parentList = docList.Where(x => x.ParentID == path).ToList();
            if (parentList.Any())
            {
                parentList = parentList.OrderBy(x => x.DocNo).ToList();
                sb.AppendLine("<ul>");
                foreach (var item in parentList)
                {
                    sb.AppendFormat("<li class='liText' data-DocumentID='" + item.DocumentID + "' data-ParentID='" + item.ParentID +
                                    "' data-Path='" + item.Path + "' data-Number='" + item.Number + "' data-DocNo='" + item.DocNo +
                                    "' data-SectionType='" + item.SectionType +
                                    "' data-Title='" + item.Title + "' data-Type='" + item.Type + "'>{0}", item.Number);
                    sb.AppendLine();
                    ListDirectories(item.DocumentID, sb);
                }
                sb.AppendLine("</ul>");
            }
        }

        [HttpPost]
        public ActionResult ManageDocuments(HttpPostedFileBase postedFiles, DocumentModal Modal, FormCollection coll, HttpPostedFileBase updatedFiles)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                string ClickedType = coll["hdnClickedType"];
                TempData["ClickedType"] = ClickedType;
                if (ClickedType == "AddRoot")
                {
                    AddRootFolder(Modal);
                }
                if (ClickedType == "AddFolder")
                {
                    AddFolder(Modal, "FOLDER");
                }
                if (ClickedType == "AddWindowsFolder")
                {
                    AddFolder(Modal, "WINDOWSFOLDER");
                }
                if (ClickedType == "AddFile")
                {
                    AddFile(postedFiles, Modal);
                }
                if (ClickedType == "RemoveObj")
                {
                    RemoveFileOrFolder(Modal, coll);
                }
                if (ClickedType == "EditDocument")
                {
                    EditDocument(Modal);
                }
                if (ClickedType == "UpdateDocument")
                {
                    UpdateDocumentFile(updatedFiles, Modal);
                }
                if (ClickedType == "UpdateFileFolder")
                {
                    UpdateDocumentFolder(Modal);
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            try
            {
                docList = _helper.GetAllDocuments();
                docList = docList.OrderBy(x => x.SectionType).ThenBy(x => Utility.ToLong(x.Number)).ToList();
                var sb = new StringBuilder();
                ListDirectories(Guid.Empty, sb);
                string res = sb.ToString();
                ViewBag.TreeView = res;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }
        public void AddRootFolder(DocumentModal Modal)
        {
            APIHelper _helper = new APIHelper();
            string dbPath = Modal.Path + @"\" + Modal.Title;
            string DownloadPath = dbPath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");
            DocumentModal dbModal = new DocumentModal();
            dbModal.DocumentID = Guid.NewGuid();
            dbModal.ParentID = Guid.Empty;
            dbModal.Number = Modal.Number;
            dbModal.Title = Modal.Title;
            dbModal.Path = dbPath;
            dbModal.Type = "FOLDER";
            dbModal.IsDeleted = false;
            dbModal.DownloadPath = DownloadPath;
            dbModal.UploadType = AppStatic.NEW;
            dbModal.Version = 1.0;
            dbModal.DocumentVersion = 1.0;
            dbModal.DocNo = Modal.DocNo;
            dbModal.Location = AppStatic.MANAGESECTION;
            dbModal.CreatedDate = DateTime.Now;
            dbModal.UpdatedDate = DateTime.Now;
            dbModal.SectionType = Modal.SectionType;
            APIResponse res = _helper.AddDocument(dbModal);
            if (res.result == AppStatic.SUCCESS)
            {
                TempData["Result"] = AppStatic.SUCCESS;
                string FolderPath = dbPath + @"\";
                Directory.CreateDirectory(Path.GetDirectoryName(FolderPath));
                string ServerFolderCreatePath = Server.MapPath("~/" + DownloadPath + @"\");
                Directory.CreateDirectory(Path.GetDirectoryName(ServerFolderCreatePath));
            }
            else
                TempData["Result"] = AppStatic.ERROR;
            TempData.Keep();
        }
        public void AddFolder(DocumentModal Modal, string type)
        {
            APIHelper _helper = new APIHelper();
            string dbPath = Modal.Path + @"\" + Modal.Title;
            string DownloadPath = dbPath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");

            DocumentModal dbModal = new DocumentModal();
            dbModal.DocumentID = Guid.NewGuid();
            dbModal.ParentID = Modal.DocumentID;
            dbModal.Number = Modal.Number;
            dbModal.Title = Modal.Title;
            dbModal.Path = dbPath;
            dbModal.Type = type;
            dbModal.IsDeleted = false;
            dbModal.DownloadPath = DownloadPath;
            dbModal.UploadType = AppStatic.NEW;
            dbModal.Version = 1.0;
            dbModal.DocumentVersion = 1.0;
            dbModal.DocNo = Modal.DocNo;
            dbModal.Location = AppStatic.MANAGESECTION;
            dbModal.CreatedDate = DateTime.Now;
            dbModal.UpdatedDate = DateTime.Now;
            dbModal.SectionType = Modal.SectionType;
            APIResponse res = _helper.AddDocument(dbModal);
            if (res.result == AppStatic.SUCCESS)
            {
                TempData["Result"] = AppStatic.SUCCESS;
                string FolderPath = dbPath + @"\";
                Directory.CreateDirectory(Path.GetDirectoryName(FolderPath));
                string ServerFolderCreatePath = Server.MapPath("~/" + DownloadPath + @"\");
                Directory.CreateDirectory(Path.GetDirectoryName(ServerFolderCreatePath));
            }
            else
                TempData["Result"] = AppStatic.ERROR;
            TempData.Keep();
        }
        public void AddFile(HttpPostedFileBase postedFiles, DocumentModal Modal)
        {
            APIHelper _helper = new APIHelper();
            if (postedFiles != null && postedFiles.ContentLength > 0)
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string dbPath = Modal.Path + @"\" + Modal.Title + Path.GetExtension(postedFiles.FileName);
                string DownloadPath = dbPath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");

                DocumentModal dbModal = new DocumentModal();
                dbModal.DocumentID = Guid.NewGuid();
                dbModal.ParentID = Modal.DocumentID;
                dbModal.Number = Modal.Number;
                dbModal.Title = Modal.Title;
                dbModal.Path = dbPath;
                dbModal.Type = Path.GetExtension(postedFiles.FileName).ToUpper().Replace(".", "");
                dbModal.IsDeleted = false;
                dbModal.DownloadPath = DownloadPath;
                dbModal.UploadType = AppStatic.NEW;
                dbModal.Version = 1.0;
                dbModal.DocumentVersion = 1.0;
                dbModal.DocNo = Modal.DocNo;
                dbModal.Location = AppStatic.MANAGESECTION;
                dbModal.CreatedDate = DateTime.Now;
                dbModal.UpdatedDate = DateTime.Now;
                dbModal.SectionType = Modal.SectionType;
                APIResponse res = _helper.AddDocument(dbModal);
                if (res.result == AppStatic.SUCCESS)
                {
                    TempData["Result"] = AppStatic.SUCCESS;
                    Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                    postedFiles.SaveAs(dbPath);
                    string ServerCreatePath = Server.MapPath("~/" + DownloadPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(ServerCreatePath));
                    postedFiles.SaveAs(ServerCreatePath);
                }
                else
                    TempData["Result"] = AppStatic.ERROR;
            }
            else
            {
                TempData["Result"] = AppStatic.ERROR;
            }
            TempData.Keep();
        }
        public void RemoveFileOrFolder(DocumentModal Modal, FormCollection coll)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                string selectedType = coll["hdnSelectedType"];
                string filePath = Modal.Path;
                string serverPath = filePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");
                string docID = Modal.DocumentID.ToString();
                APIResponse res = _helper.DeleteDocument(docID);
                if (res.result == AppStatic.SUCCESS)
                {
                    TempData["Result"] = AppStatic.SUCCESS;
                    if (selectedType == "FOLDER" || selectedType == "WINDOWSFOLDER")
                    {
                        Directory.Delete(filePath, true);
                        string serverDeletePath = Server.MapPath("~" + serverPath);
                        Directory.Delete(serverDeletePath, true);
                    }
                    else
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        string serverDeletePath = Server.MapPath("~" + serverPath);
                        if (System.IO.File.Exists(serverDeletePath))
                        {
                            System.IO.File.Delete(serverDeletePath);
                        }
                    }
                }
                else
                    TempData["Result"] = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                TempData["Result"] = AppStatic.ERROR;
                LogHelper.writelog("RemoveFileOrFolder " + ex.Message);
            }
            TempData.Keep();
        }
        public void EditDocument(DocumentModal Modal)
        {
            APIHelper _helper = new APIHelper();
            APIResponse res = _helper.UpdateDocument(Modal);
            if (res.result == AppStatic.SUCCESS)
                TempData["Result"] = AppStatic.SUCCESS;
            else
                TempData["Result"] = AppStatic.ERROR;
            TempData.Keep();
        }
        public void UpdateDocumentFile(HttpPostedFileBase postedFiles, DocumentModal Modal)
        {
            APIHelper _helper = new APIHelper();
            if (postedFiles != null && postedFiles.ContentLength > 0)
            {
                string filePath = Modal.Path;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                string serverPath = filePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");
                string serverDeletePath = Server.MapPath("~" + serverPath);
                if (System.IO.File.Exists(serverDeletePath))
                {
                    System.IO.File.Delete(serverDeletePath);
                }
                string oldFileName = Path.GetFileName(filePath);
                string folderPath = filePath.Replace(oldFileName, "");
                string newFileName = Path.GetFileName(postedFiles.FileName);
                string savePath = folderPath + newFileName;
                string DownloadPath = savePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");

                Modal.Path = savePath;
                Modal.DownloadPath = DownloadPath;
                Modal.Type = Path.GetExtension(postedFiles.FileName).ToUpper().Replace(".", "");
                APIResponse res = _helper.UpdateDocumentFile(Modal);
                if (res.result == AppStatic.SUCCESS)
                {
                    TempData["Result"] = AppStatic.SUCCESS;
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                    postedFiles.SaveAs(savePath);
                    string ServerCreatePath = Server.MapPath("~/" + DownloadPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(ServerCreatePath));
                    postedFiles.SaveAs(ServerCreatePath);
                }
                else
                    TempData["Result"] = AppStatic.ERROR;
                TempData.Keep();
            }
        }
        public void UpdateDocumentFolder(DocumentModal Modal)
        {
            APIHelper _helper = new APIHelper();
            if (Modal.Path != null && Modal.Path != "")
            {
                string filePath = Modal.Path;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                Modal.Type = "WINDOWSFOLDER";
                APIResponse res = _helper.UpdateDocumentFileFolder(Modal);
                if (res.result == AppStatic.SUCCESS)
                {
                    TempData["Result"] = AppStatic.SUCCESS;
                }
                else
                    TempData["Result"] = AppStatic.ERROR;
                TempData.Keep();
            }
        }
        #endregion

        #region Users

        public ActionResult Users()
        {

            if (TempData["UserAdded"] != null)
            {
                ViewBag.UserAdded = Utility.ToString(TempData["UserAdded"]);
            }
            APIHelper _helper = new APIHelper();
            List<UserGroup> lstUserGroups = _helper.GetAllUserGroups();
            ViewBag.UserGroups = lstUserGroups;
            return View();
        }

        public ActionResult GetAllUsers()
        {
            APIHelper _helper = new APIHelper();
            List<UserProfile> Modal = _helper.GetAllUsers();
            return Json(Modal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUser(FormCollection frmColl)
        {
            UserProfile Modal = new UserProfile();
            Modal.Email = Utility.ToString(frmColl["Email"]);
            Modal.Password = Utility.ToString(frmColl["Password"]);
            Modal.UserRole = Utility.ToInt(frmColl["UserRole"]);
            Modal.UserName = Utility.ToString(frmColl["UserName"]);
            Modal.UserGroup = Utility.ToInt(frmColl["UserGroup"]);
            Modal.RoleOrder = 1;
            APIHelper _helper = new APIHelper();
            bool res = _helper.AddUser(Modal);
            if (res)
            {
                TempData["UserAdded"] = "Success";
            }
            else
            {
                TempData["UserAdded"] = "Error";
            }
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }
        #endregion

        #region VIMS

        public ActionResult ViewVIMS()
        {
            ViewBag.PONO = TempData["PONO"];
            ViewBag.data = TempData["data"];
            ViewBag.AccountData = TempData["AccountData"];
            return View();
        }

        [HttpPost]
        public ActionResult ViewVIMSList(string pono)
        {
            ViewBag.PONO = pono;
            APIHelper helper = new APIHelper();
            ViewBag.data = helper.GetVIMSData(pono);
            ViewBag.AccountData = helper.GetAccountDetails();
            return PartialView();
        }

        [HttpPost]
        public ActionResult UpdateVIMS(FormCollection form)
        {
            string INVOICE_EXCHRATE = Request.Form["INVOICE_EXCHRATE"];
            string REQNINVOICEID = Request.Form["REQNINVOICEID"];
            string INVOICE_DATE = Request.Form["INVOICE_DATE"];
            string OldINVOICE_EXCHRATE = Request.Form["OldINVOICE_EXCHRATE"];
            string OldINVOICE_DATE = Request.Form["OldINVOICE_DATE"];
            string user = Session["UserID"].ToString();
            ViewBag.PONO = Request.Form["PONO"];
            APIHelper helper = new APIHelper();
            DateTime myDatenew = DateTime.ParseExact(INVOICE_DATE, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            DateTime myDateold = DateTime.ParseExact(OldINVOICE_DATE, "dd/MM/yyyy",
                                      System.Globalization.CultureInfo.InvariantCulture);
            helper.UpdateVIMSDate(Convert.ToDouble(INVOICE_EXCHRATE), Convert.ToInt32(REQNINVOICEID), Convert.ToDateTime(myDatenew), Convert.ToDouble(OldINVOICE_EXCHRATE), Convert.ToDateTime(myDateold), user);
            return RedirectToAction("ReloadGrid", new
            {
                pono = ViewBag.PONO
            });
        }

        [HttpPost]
        public ActionResult UpdateAccount(FormCollection form)
        {
            string VRID = Request.Form["VRID"];
            string ACCOUNTID = Request.Form["ACCOUNT_ID"];
            string OLdACCOUNT_ID = Request.Form["OLdACCOUNT_ID"];
            string PODATE = Request.Form["PODATE"];
            string OLDPODATE = Request.Form["OLDPODATE"];
            string user = Session["UserID"].ToString();
            ViewBag.PONO = Request.Form["PONO"];
            APIHelper helper = new APIHelper();
            DateTime myDatenew = DateTime.ParseExact(PODATE, "dd/MM/yyyy",
                                      System.Globalization.CultureInfo.InvariantCulture);
            DateTime myDateold = DateTime.ParseExact(OLDPODATE, "dd/MM/yyyy",
                                      System.Globalization.CultureInfo.InvariantCulture);
            helper.UpdateVIMSAccountData(VRID, OLdACCOUNT_ID, ACCOUNTID, user, myDatenew, myDateold);
            return RedirectToAction("ReloadGrid", new
            {
                pono = ViewBag.PONO
            });
        }

        public ActionResult ReloadGrid(string pono)
        {
            ViewBag.PONO = pono;
            APIHelper helper = new APIHelper();
            ViewBag.data = helper.GetVIMSData(pono);
            ViewBag.AccountData = helper.GetAccountDetails();
            //return View("ViewVIMS");
            TempData["PONO"] = pono;
            TempData["data"] = ViewBag.data;
            TempData["AccountData"] = ViewBag.AccountData;
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }
        #endregion

        #region Forms

        public ActionResult ManageForms()
        {
            APIHelper _helper = new APIHelper();
            try
            {
                formList = _helper.GetAllForms();
                formList = formList.OrderBy(x => x.Title).ToList(); //.OrderBy(x => x.Code).
                var sb = new StringBuilder();
                ListFormDirectories(Guid.Empty, sb);
                string res = sb.ToString();
                ViewBag.TreeView = res;
                if (TempData["Result"] != null && Utility.ToString(TempData["Result"]) != "")
                {
                    TempData["Result"] = TempData["Result"];
                    TempData["ClickedType"] = TempData["ClickedType"];
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return View();
        }
        public string FormsTreeView()
        {
            string res = "";
            APIHelper _helper = new APIHelper();
            try
            {
                formList = _helper.GetAllForms();
                formList = formList.OrderBy(x => x.Title).ToList();
                var sb = new StringBuilder();
                ListFormDirectories(Guid.Empty, sb);
                res = sb.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        void ListFormDirectories(Guid? path, StringBuilder sb)
        {
            if (formList.Any())
            {
                sb.AppendLine("<ul>");
                foreach (var item in formList)
                {
                    sb.AppendFormat("<li class='liText' data-FormID='" + item.FormID + "' data-Path='" + item.TemplatePath
                        + "' data-Title='" + item.Title + "' data-Code='" + item.Code + "' data-Issue='" + item.Issue + "' data-Amendment='" + item.Amendment
                        + "' data-IssueDate='" + item.IssueDate + "' data-AmendmentDate='" + item.AmendmentDate + "' data-AllowsNetworkAccess='" + item.AllowsNetworkAccess + "' data-CanBeOpened='" + item.CanBeOpened
                        + "' data-Department='" + item.Department + "' data-Category='" + item.Category + "' data-AccessLevel='" + item.AccessLevel + "' data-URN='" + item.URN
                        + "' data-DownloadPath='" + item.DownloadPath + "' data-UploadType='" + item.UploadType + "' data-CreatedDate='" + item.CreatedDate + "' data-DocType='" + item.Type
                        + "' data-HasSavedData='" + item.HasSavedData + "' data-IsURNBased='" + item.IsURNBased
                        + "'>");
                    sb.AppendLine();
                }
                sb.AppendLine("</ul>");
            }
        }

        [HttpPost]
        public ActionResult ManageForms(HttpPostedFileBase postedFiles, FormModal Modal, FormCollection coll, HttpPostedFileBase updatedFiles)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                string ClickedType = coll["hdnFormClickedType"];
                TempData["ClickedType"] = ClickedType;
                if (ClickedType == "AddFormFile")
                {
                    AddFormFile(postedFiles, Modal);
                }
                if (ClickedType == "RemoveObj")
                {
                    RemoveFormFileOrFolder(Modal, coll);
                }
                if (ClickedType == "EditForm")
                {
                    EditForm(Modal);
                }
                if (ClickedType == "UpdateForm")
                {
                    UpdateFormFile(updatedFiles, Modal);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            try
            {
                formList = _helper.GetAllForms();
                formList = formList.OrderBy(x => x.Code).ToList();
                var sb = new StringBuilder();
                ListFormDirectories(Guid.Empty, sb);
                string res = sb.ToString();
                ViewBag.TreeView = res;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }
        public void AddFormFile(HttpPostedFileBase postedFiles, FormModal Modal)
        {
            APIHelper _helper = new APIHelper();
            if (postedFiles != null && postedFiles.ContentLength > 0)
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string dbPath = Modal.TemplatePath + @"\" + Modal.Title + Path.GetExtension(postedFiles.FileName);
                string DownloadPath = dbPath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");

                FormModal dbModal = new FormModal()
                {
                    FormID = Guid.NewGuid(),
                    Title = Modal.Title,
                    TemplatePath = dbPath,
                    Type = Path.GetExtension(postedFiles.FileName).ToUpper().Replace(".", ""),
                    IsDeleted = false,
                    DownloadPath = DownloadPath,
                    UploadType = AppStatic.NEW,
                    CreatedDate = DateTime.Now,
                    Category = Modal.Category,
                    AccessLevel = Modal.AccessLevel,
                    AllowsNetworkAccess = Modal.AllowsNetworkAccess,
                    Amendment = Modal.Amendment,
                    AmendmentDate = Modal.AmendmentDate,
                    CanBeOpened = Modal.CanBeOpened,
                    Code = Modal.Code,
                    Department = Modal.Department,
                    HasSavedData = Modal.HasSavedData,
                    Issue = Modal.Issue,
                    IssueDate = Modal.IssueDate,
                    IsURNBased = Modal.IsURNBased,
                    URN = Modal.URN,
                    Version = 1.0,
                    DocumentVersion = 1.0
                };

                APIResponse res = _helper.AddForm(dbModal);
                if (res.result == AppStatic.SUCCESS)
                {
                    TempData["Result"] = AppStatic.SUCCESS;
                    Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
                    postedFiles.SaveAs(dbPath);
                    string ServerCreatePath = Server.MapPath("~/" + DownloadPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(ServerCreatePath));
                    postedFiles.SaveAs(ServerCreatePath);
                }
                else
                    TempData["Result"] = AppStatic.ERROR;
            }
            else
            {
                TempData["Result"] = AppStatic.ERROR;
            }
            TempData.Keep();
        }
        public void RemoveFormFileOrFolder(FormModal Modal, FormCollection coll)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                string selectedType = coll["hdnSelectedType"];
                string filePath = Modal.TemplatePath;
                string serverPath = filePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");
                string docID = Modal.FormID.ToString();
                APIResponse res = _helper.DeleteForm(docID);
                if (res.result == AppStatic.SUCCESS)
                {
                    TempData["Result"] = AppStatic.SUCCESS;
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    string serverDeletePath = Server.MapPath("~" + serverPath);
                    if (System.IO.File.Exists(serverDeletePath))
                        System.IO.File.Delete(serverDeletePath);
                }
                else
                    TempData["Result"] = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                TempData["Result"] = AppStatic.ERROR;
                LogHelper.writelog("RemoveFormFileOrFolder " + ex.Message);
            }
            TempData.Keep();
        }
        public void EditForm(FormModal Modal)
        {
            APIHelper _helper = new APIHelper();
            APIResponse res = _helper.UpdateForm(Modal);
            if (res.result == AppStatic.SUCCESS)
                TempData["Result"] = AppStatic.SUCCESS;
            else
                TempData["Result"] = AppStatic.ERROR;
            TempData.Keep();
        }
        public void UpdateFormFile(HttpPostedFileBase postedFiles, FormModal Modal)
        {
            APIHelper _helper = new APIHelper();
            if (postedFiles != null && postedFiles.ContentLength > 0)
            {
                string filePath = Modal.TemplatePath;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                string serverPath = filePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard", "");
                string serverDeletePath = Server.MapPath("~" + serverPath);
                if (System.IO.File.Exists(serverDeletePath))
                {
                    System.IO.File.Delete(serverDeletePath);
                }
                string oldFileName = Path.GetFileName(filePath);
                string folderPath = filePath.Replace(oldFileName, "");
                string newFileName = Path.GetFileName(postedFiles.FileName);
                string savePath = folderPath + newFileName;
                string DownloadPath = savePath.Replace(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", "");

                Modal.TemplatePath = savePath;
                Modal.DownloadPath = DownloadPath;
                Modal.Type = Path.GetExtension(postedFiles.FileName).ToUpper().Replace(".", "");
                APIResponse res = _helper.UpdateFormFile(Modal);
                if (res.result == AppStatic.SUCCESS)
                {
                    TempData["Result"] = AppStatic.SUCCESS;
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                    postedFiles.SaveAs(savePath);
                    string ServerCreatePath = Server.MapPath("~/" + DownloadPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(ServerCreatePath));
                    postedFiles.SaveAs(ServerCreatePath);
                }
                else
                    TempData["Result"] = AppStatic.ERROR;
                TempData.Keep();
            }
        }
        #endregion

        #region ShipReports

        public ActionResult ManageShipReports()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                List<CSShipsModal> shipsList = _helper.GetAllShips();
                if (shipsList == null)
                    shipsList = new List<CSShipsModal>();
                shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX")).ToList();
                shipsList = shipsList.OrderBy(x => x.Name).ToList();
                ViewBag.ShipDatas = shipsList;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }

            return View();
        }

        public JsonResult GetAllArrivalReport(string ShipName)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                if (ShipName == "All")
                    ShipName = "";
                List<ArrivalReportModal> ArrivalData = new List<ArrivalReportModal>();
                ArrivalData = _helper.GetAllArrivalReportData(ShipName);
                var jsonResult = Json(ArrivalData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllDepartureReport(string ShipName)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                if (ShipName == "All")
                    ShipName = "";
                List<DepartureReportModal> DepartureData = new List<DepartureReportModal>();
                DepartureData = _helper.GetAllDepartureReportData(ShipName);
                var jsonResult = Json(DepartureData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllDailyCargoReport(string ShipName)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                if (ShipName == "All")
                    ShipName = "";
                List<DailyCargoReportModal> DailyCargoData = new List<DailyCargoReportModal>();
                DailyCargoData = _helper.GetAllDailyCargoReportData(ShipName);
                var jsonResult = Json(DailyCargoData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllDailyPositionReport(string ShipName)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                if (ShipName == "All")
                    ShipName = "";
                List<DailyPositionReportModal> DailyPositionData = new List<DailyPositionReportModal>();
                DailyPositionData = _helper.GetAllDailyPositionReportData(ShipName);
                ViewBag.DailyPositionData = DailyPositionData;
                var jsonResult = Json(DailyPositionData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region SiteInfo

        public ActionResult ManageSiteInfo()
        {
            //APIHelper _helper = new APIHelper();
            //List<ShipWisePCModal> ShipData = new List<ShipWisePCModal>();
            //try
            //{
            //    ShipData = _helper.GetAllSiteInfoData();
            //    ViewBag.ShipPC = ShipData;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.writelog(ex.Message);
            //}

            return View();
        }

        public JsonResult GetAllSiteInfo(string ShipName)
        {
            APIHelper _helper = new APIHelper();
            List<ShipWisePCModal> ShipData = new List<ShipWisePCModal>();
            List<ShipWisePCModal> codeList = new List<ShipWisePCModal>();
            try
            {
                ShipData = _helper.GetAllSiteInfoData();
                ViewBag.ShipPC = ShipData;
                return Json(ShipData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllDeletePCRecord(string ShipCode, int Id, string PCName, string PCUniqueId)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                APIResponse res = new APIResponse();
                res = _helper.GetAllDeletePCRecordData(ShipCode, Id, PCName, PCUniqueId);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateBlockPCRecords(string ShipCode, int Id, string PCName, string PCUniqueId, bool IsBlocked)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                APIResponse res = new APIResponse();
                res = _helper.UpdateBlockPCRecords(ShipCode, Id, PCName, PCUniqueId, IsBlocked);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateMainPCRecords(string ShipCode, int Id, string PCName, string PCUniqueId, bool IsMainPC)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                APIResponse res = new APIResponse();
                res = _helper.UpdateMainPCRecords(ShipCode, Id, PCName, PCUniqueId, IsMainPC);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region XMLs

        public ActionResult ManageXmls()
        {
            APIHelper _helper = new APIHelper();
            try
            {
                formList = _helper.GetAllForms();
                formList = formList.Where(x => Utility.ToString(x.FolderType).ToLower() == "xml").OrderBy(x => x.Title).ToList();
                var sb = new StringBuilder();
                ListFormDirectories(Guid.Empty, sb);
                string res = sb.ToString();
                ViewBag.TreeView = res;
                if (TempData["Result"] != null && Utility.ToString(TempData["Result"]) != "")
                {
                    TempData["Result"] = TempData["Result"];
                    TempData["ClickedType"] = TempData["ClickedType"];
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return View();
        }
        #endregion

        #region Document ISM

        public ActionResult ManageISMDocuments()
        {
            APIHelper _helper = new APIHelper();
            try
            {
                docList = _helper.GetAllDocuments("ISM");
                docList = docList.OrderBy(x => x.SectionType).ThenBy(x => Utility.ToLong(x.Number)).ToList();
                var sb = new StringBuilder();
                ListDirectories(Guid.Empty, sb);
                string res = sb.ToString();
                ViewBag.TreeView = res;
                if (TempData["Result"] != null && Utility.ToString(TempData["Result"]) != "")
                {
                    TempData["Result"] = TempData["Result"];
                    TempData["ClickedType"] = TempData["ClickedType"];
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return View();
        }
        #endregion

        #region AssetManagmentEquipmentList   

        public ActionResult ManageAssetManagmentEquipmentList()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                List<CSShipsModal> shipsList = _helper.GetAllShips();
                if (shipsList == null)
                    shipsList = new List<CSShipsModal>();
                shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX")).ToList();
                shipsList = shipsList.OrderBy(x => x.Name).ToList();
                ViewBag.ShipDatas = shipsList;
                TempData["ShipDatas"] = shipsList;

                List<string> criticalityList = new List<string>();
                criticalityList.Add("Low");
                criticalityList.Add("Medium");
                criticalityList.Add("High");
                criticalityList.Add("Safety Critical");
                TempData["CriticalityList"] = criticalityList;
                if (TempData["AMERes"] != null)
                {
                    ViewBag.result = TempData["AMERes"];
                    TempData["AMERes"] = null;
                }
                TempData.Keep();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ManageAssetManagmentEquipmentList " + ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ManageAssetManagmentEquipmentList(AssetManagmentEquipmentListModal Modal)
        {
            try
            {
                if (Modal != null)
                {
                    string shipName = Modal.AssetManagmentEquipmentListForm.ShipName;
                    if (TempData["ShipDatas"] != null)
                    {
                        var shipList = (List<CSShipsModal>)TempData["ShipDatas"];
                        if (shipList != null && shipList.Count > 0)
                        {
                            var ship = shipList.Where(x => x.Code == Modal.AssetManagmentEquipmentListForm.ShipCode).FirstOrDefault();
                            if (ship != null && !string.IsNullOrWhiteSpace(ship.Name))
                                Modal.AssetManagmentEquipmentListForm.ShipName = ship.Name;
                        }
                    }
                    if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                    {
                        Modal.AssetManagmentEquipmentListForm.CreatedDate = DateTime.Now;
                        Modal.AssetManagmentEquipmentListForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.AssetManagmentEquipmentListForm.UpdatedDate = DateTime.Now;
                        Modal.AssetManagmentEquipmentListForm.UpdatedBy = SessionManager.Username;
                    }
                    Modal.AssetManagmentEquipmentListForm.IsSynced = false;
                    Modal.IsFromOfficeApp = true;
                    APIHelper _aHelper = new APIHelper();
                    APIResponse res = _aHelper.SubmitAssetManagmentEquipmentList(Modal);
                    if (res.result == null || res.result == AppStatic.ERROR)
                    {
                        ViewBag.result = AppStatic.ERROR;
                        TempData["AMERes"] = AppStatic.ERROR;
                    }
                    else
                    {
                        ViewBag.result = AppStatic.SUCCESS;
                        TempData["AMERes"] = AppStatic.SUCCESS;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.result = AppStatic.ERROR;
                TempData["AMERes"] = AppStatic.ERROR;
                LogHelper.writelog("ManageAssetManagmentEquipmentList Save Data " + ex.Message);
            }
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public string ManageAssetManagmentEquipmentAutoSave(AssetManagmentEquipmentListModal Modal)
        {
            string resp = "";
            try
            {
                if (Modal != null)
                {
                    string shipName = Modal.AssetManagmentEquipmentListForm.ShipName;
                    if (TempData["ShipDatas"] != null)
                    {
                        var shipList = (List<CSShipsModal>)TempData["ShipDatas"];
                        if (shipList != null && shipList.Count > 0)
                        {
                            var ship = shipList.Where(x => x.Code == Modal.AssetManagmentEquipmentListForm.ShipCode).FirstOrDefault();
                            if (ship != null && !string.IsNullOrWhiteSpace(ship.Name))
                                Modal.AssetManagmentEquipmentListForm.ShipName = ship.Name;
                        }
                    }
                    if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                    {
                        Modal.AssetManagmentEquipmentListForm.CreatedDate = DateTime.Now;
                        Modal.AssetManagmentEquipmentListForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.AssetManagmentEquipmentListForm.UpdatedDate = DateTime.Now;
                        Modal.AssetManagmentEquipmentListForm.UpdatedBy = SessionManager.Username;
                    }
                    Modal.AssetManagmentEquipmentListForm.IsSynced = false;
                    Modal.IsFromOfficeApp = true;
                    APIHelper _aHelper = new APIHelper();
                    APIResponse res = _aHelper.SubmitAssetManagmentEquipmentList(Modal);
                    if (res.result == null || res.result == AppStatic.ERROR)
                    {
                        ViewBag.result = AppStatic.ERROR;

                    }
                    else
                    {
                        if (Modal.AssetManagmentEquipmentListForm.AMEId == Guid.Empty)
                        {
                            var result = _aHelper.GetAssetManagmentEquipmentData(Modal.AssetManagmentEquipmentListForm.ShipCode);
                            if (result != null && result.AssetManagmentEquipmentListForm != null && result.AssetManagmentEquipmentListForm.AMEId != Guid.Empty)
                                Modal.AssetManagmentEquipmentListForm.AMEId = result.AssetManagmentEquipmentListForm.AMEId;
                        }
                        ViewBag.result = AppStatic.SUCCESS;
                    }
                    resp = Convert.ToString(Modal.AssetManagmentEquipmentListForm.AMEId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ManageAssetManagmentEquipmentAutoSave AutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            TempData.Keep();
            return resp;
        }

        [HttpGet]
        public string GetAssetManagmentEquipmentList(string shipCode)
        {
            AssetManagmentEquipmentListModal Modal = new AssetManagmentEquipmentListModal();
            try
            {
                APIHelper _helper = new APIHelper();
                Modal = _helper.GetAssetManagmentEquipmentData(shipCode);
                if (Modal == null || Modal.AssetManagmentEquipmentITListModel == null || Modal.AssetManagmentEquipmentITListModel.Count <= 0)
                {
                    Modal.AssetManagmentEquipmentOTListModel = new List<AssetManagmentEquipmentOTListModel>();
                    Modal.AssetManagmentEquipmentOTListModel.AddRange(new List<AssetManagmentEquipmentOTListModel> {
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="SAT C 2 PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Gyro",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Gyro repeaters",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="2 WAY VHF(3PCS)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="MF/HF transceiver",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Speed log",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Echo sounder",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="GPS PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="EPIRB",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Talk back system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ECDIS BACK-UP",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Radar No2",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ECDIS MAIN",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Radar No1",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="VHF PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="VHF transceivers(Duplicate)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Public address system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="NAVTEX PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ARPA",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Course Recorder",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="GPS 1 GPS 2",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Off course alarm",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="AIS",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Weather facsimile",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Long Range Identification and Tracking systems (LRIT)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Internal Telephone system",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Sound Powered telephone System",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="SSAS (SHIP SECURITY ALERT SYSTEM)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Inmarsat C system(DUPLICATE)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="SAT C  1 PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Voyage Data Recorder (VDR or SVDR)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Inmarsat C System(PRIMARY)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="Auto Pilot",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="VHF transceivers(PRIMARY)",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="ECHO SOUNDER PRINTER",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""},
                        new AssetManagmentEquipmentOTListModel{ OTEquipment="2 SART",OTLastServiced="",OTLocation ="",OTMake ="",OTModel = "",OTRemark = "",OTSerialNo = "",OTType = "",OTWorkingCondition = ""}
                }
                );
                    Modal.AssetManagmentEquipmentITListModel = new List<AssetManagmentEquipmentITListModel>();
                    Modal.AssetManagmentEquipmentITListModel.AddRange(new List<AssetManagmentEquipmentITListModel> {
            new AssetManagmentEquipmentITListModel{ ITEquipment="SATTELITE PHONE 1",ITLastServiced="",ITLocation ="BRIDGE",ITMake ="CISCO",ITModel = "CP7821",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="SATTELITE PHONE 1",ITLastServiced="",ITLocation ="CITADEL",ITMake ="IRIDIUM",ITModel = "9555",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="PHONE",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="CISCO",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="MASTER CABIN",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="PRINTER",ITLastServiced="",ITLocation ="MASTER CABIN",ITMake ="HP",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="TV",ITLastServiced="",ITLocation ="MASTER CABIN",ITMake ="SAMSUNG",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="TV",ITLastServiced="",ITLocation ="CH.OFF CABIN",ITMake ="KONKA",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="CH.ENGR. CABIN",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="PRINTER",ITLastServiced="",ITLocation ="CH.ENGR. CABIN",ITMake ="CANON",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="TV",ITLastServiced="",ITLocation ="CH.ENGR. CABIN",ITMake ="KONKA",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="PHILIP",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="LG",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="PRINTER",ITLastServiced="",ITLocation ="BALLAST ROOM",ITMake ="HP",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="DESKTOP PC",ITLastServiced="",ITLocation ="ENGINE ROOM",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel{ ITEquipment="PRINTER",ITLastServiced="",ITLocation ="ENGINE ROOM",ITMake ="DELL",ITModel = "",ITRemark = "SATISFACTORY",ITSerialNo = "",ITType = "",ITWorkingCondition = "GOOD"},
            new AssetManagmentEquipmentITListModel { ITEquipment = "TV", ITLastServiced = "", ITLocation = "CREW SMOKING ROOM", ITMake = "SAMSUNG", ITModel = "", ITRemark = "SATISFACTORY", ITSerialNo = "", ITType = "", ITWorkingCondition = "GOOD" } }
                     );
                }
                Modal.AssetManagmentEquipmentOTListModel = Modal.AssetManagmentEquipmentOTListModel.OrderBy(x => x.OTCriticality).ThenBy(x => x.OTEquipment).ToList();
                Modal.AssetManagmentEquipmentITListModel = Modal.AssetManagmentEquipmentITListModel.OrderBy(x => x.ITCriticality).ThenBy(x => x.ITEquipment).ToList();
                Modal.AssetManagmentEquipmentSoftwareAssetsModel = Modal.AssetManagmentEquipmentSoftwareAssetsModel.OrderBy(x => x.SACriticality).ThenBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentList " + ex.Message);
            }
            TempData.Keep();
            return ConvertViewToString("_OTITEquipmentList", Modal);
        }

        public ActionResult DownloadAssetManagmentEquipmentList(string shipCode)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                var Modal = _helper.GetAssetManagmentEquipmentData(shipCode);
                if (Modal == null)
                {
                    Modal = new AssetManagmentEquipmentListModal
                    {
                        AssetManagmentEquipmentITListModel = new List<AssetManagmentEquipmentITListModel>(),
                        AssetManagmentEquipmentOTListModel = new List<AssetManagmentEquipmentOTListModel>(),
                        AssetManagmentEquipmentSoftwareAssetsModel = new List<AssetManagmentEquipmentSoftwareAssetsModel>()
                    };
                }
                List<OTITListReportModel> exportOTList = new List<OTITListReportModel>();
                if (Modal.AssetManagmentEquipmentOTListModel != null && Modal.AssetManagmentEquipmentOTListModel.Count > 0)
                {
                    foreach (var item in Modal.AssetManagmentEquipmentOTListModel)
                    {
                        exportOTList.Add(new OTITListReportModel
                        {
                            Criticality = item.OTCriticality,
                            Equipment = item.OTEquipment,
                            HardwareId = item.OTHardwareId,
                            LastServiced = item.OTLastServiced,
                            Location = item.OTLocation,
                            Make = item.OTMake,
                            Model = item.OTModel,
                            Owner = item.OTOwner,
                            PersonResponsible = item.OTPersonResponsible,
                            Remark = item.OTRemark,
                            SerialNo = item.OTSerialNo,
                            Type = item.OTType,
                            WorkingCondition = item.OTWorkingCondition,
                            OperatingSystem = item.OTOperatingSystem,
                            OSPatchVersion = item.OTOSPatchVersion
                        });
                    }
                }
                List<OTITListReportModel> exportITList = new List<OTITListReportModel>();
                if (Modal.AssetManagmentEquipmentITListModel != null && Modal.AssetManagmentEquipmentITListModel.Count > 0)
                {
                    foreach (var item in Modal.AssetManagmentEquipmentITListModel)
                    {
                        exportITList.Add(new OTITListReportModel
                        {
                            Criticality = item.ITCriticality,
                            Equipment = item.ITEquipment,
                            HardwareId = item.ITHardwareId,
                            LastServiced = item.ITLastServiced,
                            Location = item.ITLocation,
                            Make = item.ITMake,
                            Model = item.ITModel,
                            Owner = item.ITOwner,
                            PersonResponsible = item.ITPersonResponsible,
                            Remark = item.ITRemark,
                            SerialNo = item.ITSerialNo,
                            Type = item.ITType,
                            WorkingCondition = item.ITWorkingCondition,
                            OperatingSystem = item.ITOperatingSystem,
                            OSPatchVersion = item.ITOSPatchVersion
                        });
                    }
                }

                List<SoftwareAssetsReportModel> exportSWList = new List<SoftwareAssetsReportModel>();
                if (Modal.AssetManagmentEquipmentSoftwareAssetsModel != null && Modal.AssetManagmentEquipmentSoftwareAssetsModel.Count > 0)
                {
                    foreach (var item in Modal.AssetManagmentEquipmentSoftwareAssetsModel)
                    {
                        exportSWList.Add(new SoftwareAssetsReportModel
                        {
                            Category = item.Category,
                            IsActive = item.IsActive,
                            LicenseType = item.LicenseType,
                            Manufacturer = item.Manufacturer,
                            Name = item.Name,
                            Criticality = item.SACriticality,
                            Owner = item.SAOwner,
                            PersonResponsible = item.SAPersonResponsible,
                            SoftwareId = item.SASoftwareId,
                            OperatingSystem = item.SAOperatingSystem,
                            OSPatchVersion = item.SAOSPatchVersion
                        });
                    }
                }

                exportOTList = exportOTList.OrderBy(x => x.HardwareId).ToList();
                exportITList = exportITList.OrderBy(x => x.HardwareId).ToList();
                exportSWList = exportSWList.OrderBy(x => x.SoftwareId).ToList();

                DataSet ds = new DataSet();
                ds.Tables.Add(Utility.ToDataTable(exportOTList));
                ds.Tables.Add(Utility.ToDataTable(exportITList));
                ds.Tables.Add(Utility.ToDataTable(exportSWList));

                var fileName = "Assets_report.xls";
                var spreadsheetStream = Utility.CreateWorkbook(ds, new List<string>() { "OTEquipment", "ITEquipment", "SoftwareAssets" });
                return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            finally
            {
                TempData["isPageInit"] = false;
                TempData.Keep();
            }
            return RedirectToAction("Index", "Admin");
        }
        #endregion

        #region Convert Partials As Html
        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, TempData, writer);
                ViewBag.DocList = model;
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
        #endregion

        #region ManageCybersecurity

        public ActionResult ManageCybersecurity()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                List<CSShipsModal> shipsList = _helper.GetAllShips();
                if (shipsList == null)
                    shipsList = new List<CSShipsModal>();
                shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList();
                shipsList = shipsList.OrderBy(x => x.Name).ToList();
                ViewBag.ShipDatas = shipsList;
                TempData["ShipDatas"] = shipsList;
                if (TempData["CRARes"] != null)
                {
                    ViewBag.result = TempData["CRARes"];
                    TempData["CRARes"] = null;
                }
                #region BindDropdownList
                List<string> lstRiskList = new List<string>();
                List<string> lstVulnerabilities = new List<string>();
                List<string> lstControls = new List<string>();
                List<string> lstRiskDecision = new List<string>();
                var lstCyberSecuritySettings = _helper.GetAllCyberSecuritySettingsList();
                if (lstCyberSecuritySettings != null)
                {
                    lstRiskList = lstCyberSecuritySettings.Where(x => x.Type.Trim() == "RISKDESC").Select(x => x.Name).ToList();
                    lstVulnerabilities = lstCyberSecuritySettings.Where(x => x.Type.Trim() == "VULN").Select(x => x.Name).ToList();
                    lstControls = lstCyberSecuritySettings.Where(x => x.Type == "CONTROLS").Select(x => x.Name).ToList();
                    lstRiskDecision = lstCyberSecuritySettings.Where(x => x.Type == "RISKDECISION").Select(x => x.Name).ToList();
                }
                TempData["RiskList"] = lstRiskList;
                TempData["VulnerabilityList"] = lstVulnerabilities;
                TempData["ControlsList"] = lstControls;
                TempData["RiskDecisionList"] = lstRiskDecision;
                #endregion

                TempData.Keep();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ManageCybersecurity : " + ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ManageCybersecurity(CybersecurityRisksAssessmentModal Modal)
        {
            try
            {
                if (Modal != null)
                {
                    if (Modal.CybersecurityRisksAssessmentForm.CRAId == Guid.Empty)
                    {
                        Modal.CybersecurityRisksAssessmentForm.CreatedDate = DateTime.Now;
                        Modal.CybersecurityRisksAssessmentForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.CybersecurityRisksAssessmentForm.UpdatedDate = DateTime.Now;
                        Modal.CybersecurityRisksAssessmentForm.UpdatedBy = SessionManager.Username;
                    }
                    Modal.CybersecurityRisksAssessmentForm.IsSynced = false;
                    if (Modal.CybersecurityRisksAssessmentListModal != null && Modal.CybersecurityRisksAssessmentListModal.Count > 0)
                    {
                        foreach (var item in Modal.CybersecurityRisksAssessmentListModal)
                        {
                            if (item.HardwareIdList != null && item.HardwareIdList.Count > 0)
                                item.HardwareId = string.Join(",", item.HardwareIdList.Select(x => x.Split('|')[0]).ToList());

                            if (item.ControlsList != null && item.ControlsList.Count > 0)
                                item.Controls = string.Join(",", item.ControlsList);
                        }
                    }
                    APIHelper _helper = new APIHelper();
                    APIResponse res = _helper.SubmitCybersecurityRisksAssessment(Modal);
                    if (res.result == null || res.result == AppStatic.ERROR)
                    {
                        ViewBag.result = AppStatic.ERROR;
                        TempData["CRARes"] = AppStatic.ERROR;
                    }
                    else
                    {
                        ViewBag.result = AppStatic.SUCCESS;
                        TempData["CRARes"] = AppStatic.SUCCESS;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.result = AppStatic.ERROR;
                TempData["CRARes"] = AppStatic.ERROR;
                LogHelper.writelog("CybersecurityRisksAssessment Save Data " + ex.Message);
            }
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public string ManageCybersecurityAutoSave(CybersecurityRisksAssessmentModal Modal)
        {
            string resp = "";
            try
            {
                if (Modal != null && Modal.CybersecurityRisksAssessmentForm != null)
                {
                    string shipName = Modal.CybersecurityRisksAssessmentForm.ShipName;
                    if (TempData["ShipDatas"] != null)
                    {
                        var shipList = (List<CSShipsModal>)TempData["ShipDatas"];
                        if (shipList != null && shipList.Count > 0)
                        {
                            var ship = shipList.Where(x => x.Code == Modal.CybersecurityRisksAssessmentForm.ShipCode).FirstOrDefault();
                            if (ship != null && !string.IsNullOrWhiteSpace(ship.Name))
                                Modal.CybersecurityRisksAssessmentForm.ShipName = ship.Name;
                        }
                    }
                    if (Modal.CybersecurityRisksAssessmentForm.CRAId == Guid.Empty)
                    {
                        Modal.CybersecurityRisksAssessmentForm.CreatedDate = DateTime.Now;
                        Modal.CybersecurityRisksAssessmentForm.CreatedBy = SessionManager.Username;
                    }
                    else
                    {
                        Modal.CybersecurityRisksAssessmentForm.UpdatedDate = DateTime.Now;
                        Modal.CybersecurityRisksAssessmentForm.UpdatedBy = SessionManager.Username;
                    }
                    if (Modal.CybersecurityRisksAssessmentListModal != null && Modal.CybersecurityRisksAssessmentListModal.Count > 0)
                    {
                        foreach (var item in Modal.CybersecurityRisksAssessmentListModal)
                        {
                            if (item.HardwareIdList != null && item.HardwareIdList.Count > 0)
                                item.HardwareId = string.Join(",", item.HardwareIdList);
                            //item.HardwareId = string.Join(",", item.HardwareIdList.Select(x => x.Split('|')[0]).ToList());
                            if (item.ControlsList != null && item.ControlsList.Count > 0)
                                item.Controls = string.Join(",", item.ControlsList);
                        }
                    }
                    Modal.CybersecurityRisksAssessmentForm.IsSynced = false;
                    Modal.IsFromOfficeApp = true;
                    APIHelper _aHelper = new APIHelper();
                    APIResponse res = _aHelper.SubmitCybersecurityRisksAssessment(Modal);
                    if (res.result == null || res.result == AppStatic.ERROR)
                    {
                        ViewBag.result = AppStatic.ERROR;
                    }
                    else
                    {
                        if (Modal.CybersecurityRisksAssessmentForm.CRAId == Guid.Empty)
                        {
                            var result = _aHelper.GetCybersecurityRisksAssessmentData(Modal.CybersecurityRisksAssessmentForm.ShipCode);
                            if (result != null && result.CybersecurityRisksAssessmentForm != null && result.CybersecurityRisksAssessmentForm.CRAId != Guid.Empty)
                                Modal.CybersecurityRisksAssessmentForm.CRAId = result.CybersecurityRisksAssessmentForm.CRAId;
                        }
                        ViewBag.result = AppStatic.SUCCESS;
                    }
                    resp = Convert.ToString(Modal.CybersecurityRisksAssessmentForm.CRAId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ManageCybersecurityAutoSave AutoSave Error : " + ex.Message);
                ViewBag.result = AppStatic.ERROR;
            }
            TempData.Keep();
            return resp;
        }


        //[HttpGet]
        //public ActionResult _CybersecurityRisksAssessmentList(string shipCode)
        //{
        //    CybersecurityRisksAssessmentModal Modal = new CybersecurityRisksAssessmentModal();
        //    try
        //    {
        //        APIHelper _helper = new APIHelper();
        //        Modal = _helper.GetCybersecurityRisksAssessmentData(shipCode);
        //        //if (Modal != null && Modal.CybersecurityRisksAssessmentListModal != null && Modal.CybersecurityRisksAssessmentListModal.Count > 0)
        //        //{
        //        //    //int impactScore, reseduleScore;
        //        //    //foreach (var item in Modal.CybersecurityRisksAssessmentListModal)
        //        //    //{
        //        //    //    //if (string.IsNullOrWhiteSpace(item.HardwareId))
        //        //    //    //    item.HardwareId = "";
        //        //    //    //if (!string.IsNullOrWhiteSpace(item.HardwareId))
        //        //    //    //    item.HardwareIdList = item.HardwareId.Split(',').ToList();
        //        //    //    //else
        //        //    //    //{
        //        //    //    //    item.HardwareIdList = new List<string>();
        //        //    //    //    item.HardwareId = "";
        //        //    //    //}
        //        //    //    //if (!string.IsNullOrWhiteSpace(item.Controls))
        //        //    //    //    item.ControlsList = item.Controls.Split(',').ToList();
        //        //    //    //else
        //        //    //    //{
        //        //    //    //    item.Controls = "";
        //        //    //    //    item.ControlsList = new List<string>();
        //        //    //    //}
        //        //    //    //if (string.IsNullOrWhiteSpace(item.Controls))
        //        //    //    //    item.Controls = "";

        //        //    //    //impactScore = (Convert.ToInt32(item.InherentImpactScore) * Convert.ToInt32(item.InherentLikelihoodScore));
        //        //    //    //if (impactScore <= 3)
        //        //    //    //{
        //        //    //    //    item.InherentRiskScore = "Low";
        //        //    //    //    item.InherentClass = "bg-green-active";
        //        //    //    //}
        //        //    //    //else if (impactScore > 3 && impactScore <= 6)
        //        //    //    //{
        //        //    //    //    item.InherentRiskScore = "Medium";
        //        //    //    //    item.InherentClass = "bg-yellowtext";
        //        //    //    //}
        //        //    //    //else if (impactScore > 6 && impactScore <= 9)
        //        //    //    //{
        //        //    //    //    item.InherentRiskScore = "High";
        //        //    //    //    item.InherentClass = "bg-yellow-active";
        //        //    //    //}
        //        //    //    //else if (impactScore > 9 && impactScore <= 16)
        //        //    //    //{
        //        //    //    //    item.InherentRiskScore = "Very high";
        //        //    //    //    item.InherentClass = "bg-red";
        //        //    //    //}
        //        //    //    //reseduleScore = (Convert.ToInt32(item.ResidualImpactScore) * Convert.ToInt32(item.ResidualLikelihoodScore));
        //        //    //    //if (reseduleScore <= 3)
        //        //    //    //{
        //        //    //    //    item.ResidualRiskScore = "Low";
        //        //    //    //    item.ResidualClass = "bg-green-active";
        //        //    //    //}
        //        //    //    //else if (reseduleScore > 3 && reseduleScore <= 6)
        //        //    //    //{
        //        //    //    //    item.ResidualRiskScore = "Medium";
        //        //    //    //    item.ResidualClass = "bg-yellowtext";
        //        //    //    //}
        //        //    //    //else if (reseduleScore > 6 && reseduleScore <= 9)
        //        //    //    //{
        //        //    //    //    item.ResidualRiskScore = "High";
        //        //    //    //    item.ResidualClass = "bg-yellow-active";
        //        //    //    //}
        //        //    //    //else if (reseduleScore > 9 && reseduleScore <= 16)
        //        //    //    //{
        //        //    //    //    item.ResidualRiskScore = "Very high";
        //        //    //    //    item.ResidualClass = "bg-red";
        //        //    //    //}
        //        //    //}
        //        //}
        //        var hardwareList = _helper.GetAssetManagmentHardwareId(shipCode);
        //        if (hardwareList != null && hardwareList.Count > 0)
        //            hardwareList = hardwareList.OrderBy(q => q).ToList();
        //        TempData["HardwareList"] = hardwareList;
        //        Modal.HardwareListBoxValues = hardwareList;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.writelog("_CybersecurityRisksAssessmentList " + ex.Message);
        //    }
        //    TempData["shipCode"] = shipCode;
        //    TempData.Keep();
        //    return PartialView("_CybersecurityRisksAssessmentList", Modal);
        //}

        [HttpGet]
        public ActionResult GetCybersecurityRiskAssessmentList(string shipCode)
        {
            CybersecurityRisksAssessmentModal Modal = new CybersecurityRisksAssessmentModal();
            try
            {
                APIHelper _helper = new APIHelper();
                Modal = _helper.GetCybersecurityRisksAssessmentData(shipCode);
                TempData["HardwareList"] = Modal.HardwareListBoxValues;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCybersecurityRiskAssessmentList " + ex.Message);
            }
            TempData["shipCode"] = shipCode;
            TempData.Keep();
            return PartialView("_CybersecurityRisksAssessmentList", Modal);
        }

        public ActionResult DownloadCybersecurityRiskAssessmentList(string shipCode)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                var Modal = _helper.GetCybersecurityRisksAssessmentData(shipCode);
                if (Modal == null || Modal.CybersecurityRisksAssessmentListModal == null)
                {
                    Modal = new CybersecurityRisksAssessmentModal
                    {
                        CybersecurityRisksAssessmentListModal = new List<CybersecurityRisksAssessmentListModal>()
                    };
                }
                List<CybersecurityRisksAssessmentReportModal> exportList = new List<CybersecurityRisksAssessmentReportModal>();
                foreach (var item in Modal.CybersecurityRisksAssessmentListModal)
                {
                    exportList.Add(new CybersecurityRisksAssessmentReportModal
                    {
                        HardwareId = item.HardwareId,
                        Controls = item.Controls,
                        InherentImpactScore = item.InherentImpactScore,
                        InherentLikelihoodScore = item.InherentLikelihoodScore,
                        InherentRiskCategoryA = item.InherentRiskCategoryA,
                        InherentRiskCategoryC = item.InherentRiskCategoryC,
                        InherentRiskCategoryI = item.InherentRiskCategoryI,
                        InherentRiskCategoryS = item.InherentRiskCategoryS,
                        InherentRiskScore = item.InherentRiskScore,
                        ResidualImpactScore = item.ResidualImpactScore,
                        ResidualLikelihoodScore = item.ResidualLikelihoodScore,
                        ResidualRiskCategoryA = item.ResidualRiskCategoryA,
                        ResidualRiskCategoryC = item.ResidualRiskCategoryC,
                        ResidualRiskCategoryI = item.ResidualRiskCategoryI,
                        ResidualRiskCategoryS = item.ResidualRiskCategoryS,
                        ResidualRiskScore = item.ResidualRiskScore,
                        RiskDecision = item.RiskDecision,
                        RiskDescription = item.RiskDescription,
                        RiskId = item.RiskId,
                        RiskOwner = item.RiskOwner,
                        Vulnerability = item.Vulnerability
                    });
                }
                var fileName = "Cybersecurity_report.xls";
                //Response.ClearContent();
                //Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                //Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                //Utility.WriteToXLS(exportList, Response.Output);
                //Response.End();
                DataSet ds = new DataSet();
                ds.Tables.Add(Utility.ToDataTable(exportList));
                var spreadsheetStream = Utility.CreateWorkbook(ds, new List<string>() { "Sheet1" });
                return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            TempData["isPageInit"] = false;
            TempData.Keep();
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public JsonResult CopyCybersecurityRisksAssessment(CyberSecurityCopyDataModal Modal)
        {
            string resp = "";
            try
            {
                APIHelper _helper = new APIHelper();
                APIResponse res = _helper.CopyCybersecurityRisksAssessment(Modal);
                if (res.result == AppStatic.SUCCESS)
                {
                    resp = AppStatic.SUCCESS;
                }
                else
                    resp = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CopyCybersecurityRisksAssessment Error : " + ex.Message);
                resp = AppStatic.ERROR;
            }
            TempData.Keep();
            var jsonResult = Json(new { result = resp }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region ManageSoftwareAssets

        public ActionResult ManageSoftwareAssets()
        {
            APIHelper _helper = new APIHelper();
            List<CSShipsModal> shipsList = _helper.GetAllShips();
            if (shipsList == null)
                shipsList = new List<CSShipsModal>();
            shipsList = shipsList.Where(x => !x.Name.ToUpper().StartsWith("XX")).ToList();
            shipsList = shipsList.OrderBy(x => x.Name).ToList();
            ViewBag.ShipDatas = shipsList;
            TempData["ShipDatas"] = shipsList;
            return View();
        }

        public JsonResult GetShipSystemsSoftwareAssets(string ShipCode)
        {
            APIHelper _helper = new APIHelper();
            try
            {
                if (ShipCode == "All")
                    ShipCode = "";
                var data = _helper.GetShipSystemsSoftwareAssets(ShipCode);
                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipSystemsSoftwareAssets : " + ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ManageDownloads
        public ActionResult ManageDownloads()
        {
            return View();
        }
        public FileResult DownloadMainSyncServiceSetup()
        {
            string path = Server.MapPath(@"~\Service\CarisbrookeShippingService.exe");
            string mimeType = string.Empty;
            try
            {
                string filename = Path.GetFileName(path);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, "application/force-download", filename);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return File(path, mimeType);
        }
        #endregion
    }
}