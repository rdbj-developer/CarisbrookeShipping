using OfficeApplication.BLL.Helpers;
using OfficeApplication.BLL.Modals;
using OfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeApplication.Controllers
{
    [SessionExpire]
    public class RepositoryController : Controller
    {
        // GET: Repository
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllRepositories()
        {
            List<RepositoryModal> FolderList = new List<RepositoryModal>();
            try
            {
                APIHelper _helper = new APIHelper();
                FolderList = _helper.GetAllRepositories();
                if (FolderList != null && FolderList.Count > 0)
                {
                    FolderList = FolderList.OrderBy(x => x.Name).ToList();
                }
                //string path = @"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\Repository";
                //ListDirectories(path, Guid.Empty.ToString(), ref FolderList);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return Json(FolderList, JsonRequestBehavior.AllowGet);
        }
        void ListDirectories(string path, string parentid, ref List<DocumentFolders> FolderList)
        {
            var directories = Directory.GetDirectories(path);
            if (directories.Any())
            {
                foreach (var directory in directories)
                {
                    string docID = Guid.NewGuid().ToString();
                    var di = new DirectoryInfo(directory);
                    FolderList.Add(new DocumentFolders { DocumentID = docID, ParentID = parentid, Name = di.Name, Type = "FOLDER", Extension = string.Empty, Version = 1.0, Path = directory });
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        string docFileID = Guid.NewGuid().ToString();
                        FolderList.Add(new DocumentFolders { DocumentID = docFileID, ParentID = docID, Name = Path.GetFileName(file), Type = "File", Extension = Path.GetExtension(file).ToUpper(), Version = 1.0, Path = file });
                    }
                    ListDirectories(directory, docID, ref FolderList);
                }
            }
        }
        [HttpPost]
        public ActionResult AddNewFile(List<HttpPostedFileBase> postedFiles, FormCollection coll)
        {
            try
            {
                foreach (HttpPostedFileBase postedFile in postedFiles)
                {
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(postedFile.FileName);
                        string Repositorypath = coll["hdnDocPath"];
                        string path = Path.Combine(@"C:\ProgramData\Carisbrooke Shipping Ltd\ISM Dashboard\", Repositorypath);
                        postedFile.SaveAs(path + _FileName);
                    }
                }
                ViewBag.Message = "File Added Successfully!!";
                return View();
            }
            catch
            {
                ViewBag.Message = "File adding failed!!";
                return View();
            }
        }
    }

}