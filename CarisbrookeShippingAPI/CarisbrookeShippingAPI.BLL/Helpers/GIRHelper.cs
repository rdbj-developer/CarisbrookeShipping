using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class GIRHelper
    {
        const string Zero = "";

        public bool SubmitGIR(Modals.GeneralInspectionReport Modal)
        {
            bool res = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.GeneralInspectionReport dbModal = new Entity.GeneralInspectionReport();

                if (Modal != null && Modal.UniqueFormID != null)
                {
                    if (Modal.UniqueFormID != null)
                    {
                        dbModal = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == Modal.UniqueFormID).FirstOrDefault();
                    }
                }
                if (dbModal == null)
                    dbModal = new Entity.GeneralInspectionReport();

                // JSL 12/31/2022
                if (Modal.ShipID == null || Modal.ShipID == 0)
                {
                    var dbships = dbContext.CSShips.Where(x => x.Code == Modal.Ship).FirstOrDefault();
                    if (dbships != null)
                    {
                        Modal.ShipID = dbships.ShipId;
                        Modal.ShipName = dbships.Name;
                    }
                }
                // End JSL 12/31/2022

                SetGIRFormData(ref dbModal, Modal);

                dbModal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbModal.SavedAsDraft = false;
                dbModal.isDelete = 0; // RDBJ 01/05/2022

                if (dbModal != null && dbModal.UniqueFormID != null)
                {
                    dbModal.IsSynced = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbModal.IsSynced = false;
                    dbModal.UniqueFormID = Modal.UniqueFormID;
                    dbModal.FormVersion = Modal.FormVersion;
                    dbModal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.GeneralInspectionReports.Add(dbModal);
                    dbContext.SaveChanges();
                }

                GIRSafeManningRequirements_Save(dbModal.UniqueFormID, Modal.GIRSafeManningRequirements);
                GIRCrewDocuments_Save(dbModal.UniqueFormID, Modal.GIRCrewDocuments);
                GIRRestandWorkHours_Save(dbModal.UniqueFormID, Modal.GIRRestandWorkHours);
                GIRDeficiencies_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRDeficiencies);
                GIRPhotos_Save(dbModal.UniqueFormID, Modal.GIRPhotographs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitGIR : " + ex.Message);
            }
            return res;
        }

        public List<GIRData> GetGIRFormsFilled(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<GIRData> list = new List<GIRData>();
            try
            {
                dynamic data = null;
                if (shipCode == "")
                {
                    data = from G in dbContext.GIRDeficiencies
                           group G by G.Ship into pg
                           join s in dbContext.CSShips on pg.FirstOrDefault().Ship equals s.Code
                           where s.Code != null
                           select new GIRData
                           {
                               Location = s.Ports,
                               Outstending = pg.Where(x => x.IsClose == false || x.IsClose == null).Count(),
                               NoDefects = pg.Count(),
                               Ship = s.Code,
                               ShipName = s.Name,
                               ReportType = pg.FirstOrDefault().ReportType
                           };
                }
                else
                {
                    data = from G in dbContext.GIRDeficiencies
                           group G by G.Ship into pg
                           join s in dbContext.CSShips on pg.FirstOrDefault().Ship equals s.Code
                           where s.Code != null && s.Code == shipCode
                           select new GIRData
                           {
                               Location = s.Ports,
                               Outstending = pg.Where(x => x.IsClose == false || x.IsClose == null).Count(),
                               NoDefects = pg.Count(),
                               Ship = s.Code,
                               ShipName = s.Name,
                               ReportType = pg.FirstOrDefault().ReportType
                           };
                }
                foreach (var item in data)
                {
                    GIRData obj = new GIRData();
                    //   obj.GIRFormID = item.GIRFormID;
                    obj.Location = item.Location;
                    obj.Auditor = "";
                    //if (item.Date.HasValue)
                    //    obj.Date = item.Date.Value.ToString("dd-MMM-yyyy");
                    //else
                    obj.Date = "";
                    obj.NoDefects = item.NoDefects;
                    obj.Outstending = item.Outstending;
                    obj.Ship = item.Ship;
                    obj.ShipName = item.ShipName;
                    obj.ReportType = item.ReportType;
                    list.Add(obj);
                }
                list = list.OrderByDescending(x => x.Date).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRFormsFilled " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public List<GIRDataList> GetDeficienciesData(string code)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<GIRDataList> list = new List<GIRDataList>();
            try
            {
                var data = dbContext.GIRDeficiencies.Where(x => x.Ship == code).ToList().OrderByDescending(x => x.CreatedDate);

                foreach (var item in data)
                {
                    GIRDataList obj = new GIRDataList();
                    var files = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == item.DeficienciesID).ToList();
                    foreach (var subitem in files)
                    {
                        Modals.GIRDeficienciesFile filedata = new Modals.GIRDeficienciesFile();
                        filedata.DeficienciesID = subitem.DeficienciesID;
                        filedata.FileName = subitem.FileName;
                        filedata.StorePath = subitem.StorePath;
                        filedata.GIRDeficienciesFileID = subitem.GIRDeficienciesFileID;
                        obj.GIRDeficienciesFile.Add(filedata);
                    }
                    obj.GIRFormID = item.GIRFormID;
                    obj.DeficienciesID = item.DeficienciesID;
                    obj.Deficiency = item.Deficiency;
                    obj.IsColse = item.IsClose;
                    obj.Number = item.No != 0 ? item.No.ToString() : item.SIRNo;
                    obj.FileName = item.FileName;
                    obj.StorePath = item.StorePath;
                    obj.ReportType = item.ReportType;
                    list.Add(obj);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesData : " + ex.Message);
            }
            return list;
        }
        public bool UpdateDeficienciesData(string id, bool isClose)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid DefUniqueID = Guid.Parse(id);
            try
            {
                var data = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == DefUniqueID).FirstOrDefault();
                if (data != null)
                {
                    // JSL 06/28/2022 wrapped in if
                    if (isClose)
                    {
                        data.IsClose = isClose;
                        data.DateClosed = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    // End JSL 06/28/2022 wrapped in if
                    // JSL 06/28/2022 added else
                    else
                    {
                        data.IsClose = isClose;
                        data.DateClosed = null;
                    }
                    // End JSL 06/28/2022 added else
                }
                dbContext.SaveChanges();

                // JSL 06/28/2022
                if (isClose)
                {
                    dbContext.Database.ExecuteSqlCommand("UPDATE [GIRDeficienciesNotes] SET [isNew] = 0 WHERE [DeficienciesUniqueID] = {0}", DefUniqueID);
                    dbContext.Database.ExecuteSqlCommand("UPDATE [GIRDeficienciesInitialActions] SET [isNew] = 0 WHERE [DeficienciesUniqueID] = {0}", DefUniqueID);
                    dbContext.Database.ExecuteSqlCommand("UPDATE [GIRDeficienciesResolution] SET [isNew] = 0 WHERE [DeficienciesUniqueID] = {0}", DefUniqueID);

                    dbContext.Database.ExecuteSqlCommand("UPDATE [Notification] SET [ReadDateTime] = '" + Utility.ToDateTimeUtcNow() + "', [IsRead] = {0} WHERE [UniqueDataId] = {1}", 1, DefUniqueID);
                }
                // End JSL 06/28/2022

                UpdateFormVersion(Convert.ToString(DefUniqueID));   // JSL 06/28/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficienciesData : " + ex.Message);
                return false;
            }
            return true;
        }

        public bool UpdateDeficienciesById(int id, string Deficiency)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                var data = dbContext.GIRDeficiencies.Where(x => x.DeficienciesID == id).FirstOrDefault();
                if (data != null)
                {
                    data.Deficiency = Deficiency;
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficienciesById : " + ex.Message);
                return false;
            }
            return true;
        }

        public int GetExistsDeficienciesBySection(string Section, string ItemNo, Guid? UFormId)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            int deficienciesId = 0;
            try
            {
                var data = dbContext.GIRDeficiencies.Where(x => x.Section == Section && (x.ItemNo == ItemNo || x.ItemNo == null) && x.UniqueFormID == UFormId).FirstOrDefault();
                if (data != null)
                {
                    deficienciesId = data.DeficienciesID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficienciesBySection : " + ex.Message);
            }
            return deficienciesId;
        }
        public int DeficienciesNumber(string ship, string reportType, string UniqueFormID) //RDBJ 11/02/2021 Added UniqueFormID //RDBJ 09/24/2021 Added reportType //RDBJ 09/18/2021 removed 2 parameters Guid? UniqueFormID, string ItemNo
        {
            int nextnumber = 0;
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                //RDBJ 09/18/2021 Commented
                /*
                int existsDefi = dbContext.GIRDeficiencies.Where(x => x.Ship == ship && x.UniqueFormID == UniqueFormID && x.ItemNo == ItemNo).Select(x => x.No).FirstOrDefault();
                if(existsDefi > 0)
                {
                    nextnumber = existsDefi;
                }
                else
                {
                    nextnumber = dbContext.GIRDeficiencies.Where(x => x.Ship == ship).Select(x => x.No).DefaultIfEmpty(0).Max();
                    if (nextnumber == 0)
                    {
                        nextnumber = nextnumber + 501;
                    }
                    else
                    {
                        nextnumber = nextnumber + 1;
                    }
                }
                */

                //RDBJ 11/02/2021
                Guid UFID = new Guid();
                UFID = Guid.Parse(UniqueFormID);
                //End RDBJ 11/02/2021

                nextnumber = dbContext.GIRDeficiencies.Where(x => x.Ship == ship && x.ReportType == reportType.ToUpper() && x.UniqueFormID == UFID).Select(x => x.No).DefaultIfEmpty(0).Max(); //RDBJ 11/02/2021 Added UniqueFormID //RDBJ 09/24/2021 Added x.ReportType == reportType.ToUpper()
                if (nextnumber == 0)
                {
                    nextnumber = nextnumber + 501;
                }
                else
                {
                    nextnumber = nextnumber + 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeficienciesNumber :" + ex.Message);
            }
            return nextnumber;
        }
        public List<DeficienciesNote> GetDeficienciesNote(Guid id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<DeficienciesNote> list = new List<DeficienciesNote>();
            try
            {
                var data = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesUniqueID == id).ToList();
                foreach (var item in data)
                {
                    DeficienciesNote obj = new DeficienciesNote();
                    var notefile = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteUniqueID == item.NoteUniqueID).ToList();
                    foreach (var subitem in notefile)
                    {
                        Modals.GIRDeficienciesCommentFile filedata = new Modals.GIRDeficienciesCommentFile();
                        //filedata.StorePath = subitem.StorePath; //RDBJ 10/14/2021 commented due to avoid maxJsonLength even not required
                        filedata.FileName = subitem.FileName;
                        //filedata.GIRCommentFileID = subitem.GIRCommentFileID; //RDBJ 10/14/2021 commented not required
                        filedata.CommentFileUniqueID = subitem.CommentFileUniqueID;
                        obj.GIRDeficienciesCommentFile.Add(filedata);
                    }
                    obj.Comment = item.Comment;
                    obj.UserName = item.UserName;
                    obj.CreatedDate = item.CreatedDate;
                    obj.isNew = item.isNew; //RDBJ 10/16/2021
                    obj.IsResolution = false;   // JSL 07/05/2022
                    list.Add(obj);
                }

                // JSL 07/05/2022
                var dataResolution = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesUniqueID == id).ToList();
                foreach (var itemRes in dataResolution)
                {
                    DeficienciesNote obj = new DeficienciesNote();
                    var resfile = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == itemRes.ResolutionUniqueID).ToList();
                    foreach (var subitem in resfile)
                    {
                        Modals.GIRDeficienciesCommentFile filedata = new Modals.GIRDeficienciesCommentFile();
                        filedata.FileName = subitem.FileName;
                        filedata.CommentFileUniqueID = subitem.ResolutionFileUniqueID;
                        obj.GIRDeficienciesCommentFile.Add(filedata);
                    }

                    obj.Comment = itemRes.Resolution;
                    obj.UserName = itemRes.Name;
                    obj.CreatedDate = itemRes.CreatedDate;
                    obj.IsResolution = true;
                    obj.isNew = itemRes.isNew;
                    list.Add(obj);
                }
                // End JSL 07/05/2022

                list = list.OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesNote " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public List<Modals.GIRDeficienciesFile> GetDeficienciesFiles(int id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Modals.GIRDeficienciesFile> list = new List<Modals.GIRDeficienciesFile>();
            try
            {
                var data = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == id).ToList();
                foreach (var item in data)
                {
                    Modals.GIRDeficienciesFile obj = new Modals.GIRDeficienciesFile();
                    obj.StorePath = item.StorePath;
                    obj.FileName = item.FileName;
                    obj.DeficienciesID = item.DeficienciesID;
                    obj.GIRDeficienciesFileID = item.GIRDeficienciesFileID;
                    list.Add(obj);
                }
                list = list.OrderByDescending(x => x.DeficienciesID).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesFiles " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }

        public Modals.GIRDeficiencies GetDeficienciesById(Guid id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.GIRDeficiencies list = new Modals.GIRDeficiencies();
            try
            {
                var data = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == id).FirstOrDefault();
                if (data != null)
                {
                    list.UniqueFormID = data.UniqueFormID;  // JSL 12/03/2022
                    list.Section = data.Section;
                    list.Deficiency = data.Deficiency;
                    list.DateRaised = data.DateRaised;
                    list.DateClosed = data.DateClosed;
                    list.No = data.No;
                    list.DeficienciesID = data.DeficienciesID;
                    list.DeficienciesUniqueID = data.DeficienciesUniqueID;
                    list.ReportType = data.ReportType;
                    list.Ship = data.Ship;
                    list.ShipName = dbContext.CSShips.Where(x => x.Code == data.Ship).Select(x => x.Name).FirstOrDefault(); // JSL 06/13/2022
                    list.IsClose = data.IsClose;
                    list.Priority = data.Priority == null ? 12 : data.Priority; //RDBJ 11/02/2021
                    list.AssignTo = data.AssignTo; // RDBJ 12/17/2021

                    // JSL 07/11/2022 wrapped in if
                    if (data.DueDate != null)
                    {
                        list.DueDate = data.DueDate;    // RDBJ 02/28/2022
                    }
                    // End JSL 07/11/2022 wrapped in if
                    // JSL 07/11/2022
                    else
                    {
                        list.DueDate = data.DateRaised.Value.AddDays((double)(7 * list.Priority));
                    }
                    // End JSL 07/11/2022

                    // JSL 12/03/2022
                    Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                    dicFileMetaData["UniqueFormID"] = Convert.ToString(list.UniqueFormID);
                    dicFileMetaData["ReportType"] = list.ReportType;
                    dicFileMetaData["DetailUniqueId"] = Convert.ToString(list.DeficienciesUniqueID);

                    List<Entity.GIRDeficienciesFile> dbDefFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == id).ToList();
                    if (dbDefFiles != null && dbDefFiles.Count > 0)
                    {
                        foreach (var itemDefFile in dbDefFiles)
                        {
                            if (itemDefFile.StorePath.StartsWith("data:"))
                            {
                                dicFileMetaData["FileName"] = itemDefFile.FileName;
                                dicFileMetaData["Base64FileData"] = itemDefFile.StorePath;

                                itemDefFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    // End JSL 12/03/2022

                    list.GIRDeficienciesFile = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == id).Select(x => new Modals.GIRDeficienciesFile()
                    {
                        FileName = x.FileName,
                        StorePath = x.StorePath,    // JSL 12/03/2022 uncommented
                        //DeficienciesID = x.DeficienciesID,
                        GIRDeficienciesFileID = x.GIRDeficienciesFileID
                        , DeficienciesUniqueID = x.DeficienciesUniqueID // JSL 05/10/2022
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesById " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        //RDBJ 09/20/2021 Modified this Update Logic and so on in this Function
        public void AddDeficienciesNote(DeficienciesNote data)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                Guid NoteUID = Guid.NewGuid();

                // JSL 01/08/2023
                if (data.NoteUniqueID != null && data.NoteUniqueID != Guid.Empty)
                    NoteUID = (Guid)data.NoteUniqueID;
                // End JSL 01/08/2023

                GIRDeficienciesNote note = new GIRDeficienciesNote();
                note.Comment = data.Comment;
                note.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                note.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                note.UserName = data.UserName;
                note.DeficienciesID = 0; //data.DeficienciesID; //RDBJ 10/14/2021 Set 0
                note.DeficienciesUniqueID = data.DeficienciesUniqueID;
                note.GIRFormID = 0; // dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == data.DeficienciesUniqueID).Select(x => x.GIRFormID).FirstOrDefault(); //RDBJ 10/14/2021 set 0
                note.NoteUniqueID = NoteUID;
                //note.isNew = 1;   // JSL 07/4/2022 commented this line // RDBJ 01/05/2022 set 1 //RDBJ 10/14/2021
                note.isNew = 0; // JSL 07/04/2022
                dbContext.GIRDeficienciesNotes.Add(note);
                dbContext.SaveChanges();
                SaveImageFileForComments(data.GIRDeficienciesCommentFile, NoteUID);

                //RDBJ 09/20/2021
                Entity.GIRDeficiency defDetails = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == data.DeficienciesUniqueID).FirstOrDefault(); //RDBJ 10/14/2021 set var to Entity.GIRDeficiency
                if (defDetails.ReportType.ToUpper() == "GI")
                {
                    Entity.GeneralInspectionReport girForm = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                    girForm.FormVersion = girForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                    girForm.IsSynced = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    Entity.SuperintendedInspectionReport sirForm = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                    sirForm.FormVersion = sirForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                    sirForm.IsSynced = false;
                    dbContext.SaveChanges();
                }

                defDetails.UpdatedDate = Utility.ToDateTimeUtcNow(); //RDBJ 10/14/2021
                dbContext.SaveChanges(); //RDBJ 10/14/2021
                //End RDBJ 09/20/2021
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesNote " + ex.Message + "\n" + ex.InnerException);
            }
        }
        //End RDBJ 09/20/2021 Modified this Function

        private static string SaveImageFileForComments(List<Modals.GIRDeficienciesCommentFile> modal, Guid noteid)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modal != null)
                {
                    foreach (var item in modal)
                    {
                        if (item.IsUpload == "true")
                        {
                            // JSL 12/03/2022
                            Entity.GIRDeficienciesCommentFile file = new Entity.GIRDeficienciesCommentFile();
                            file.FileName = item.FileName;
                            file.StorePath = item.StorePath;
                            file.IsUpload = item.IsUpload;
                            file.DeficienciesID = 0;
                            file.NoteUniqueID = noteid;
                            file.CommentFileUniqueID = Guid.NewGuid();
                            dbContext.GIRDeficienciesCommentFiles.Add(file);
                            dbContext.SaveChanges();
                            // End JSL 12/03/2022

                            // JSL 12/03/2022 commented
                            /*
                            var split = item.StorePath.Split(',');
                            string OrignalString = split.LastOrDefault();
                            if (!string.IsNullOrEmpty(OrignalString))
                            {
                                
                                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                                OrignalString = regex.Replace(OrignalString, string.Empty);
                                byte[] imageBytes = Convert.FromBase64String(OrignalString);
                                string rootpath = HttpContext.Current.Server.MapPath("~/GIRComments/");

                                string subPath = item.DeficienciesID.ToString() + "/" + noteid.ToString() + "/";
                                bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                                if (!exists)
                                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                                string CompleteFolderPath = Path.Combine(rootpath + subPath);
                                if (!Directory.Exists(CompleteFolderPath))
                                {
                                    Directory.CreateDirectory(CompleteFolderPath);
                                }
                                var imageName = item.GIRCommentFileID + "_" + DateTime.Now.ToString("MMdddyyyhhmmss") + "_" + item.FileName;
                                File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);

                                Entity.GIRDeficienciesCommentFile file = new Entity.GIRDeficienciesCommentFile();
                                file.FileName = item.FileName;
                                file.StorePath = item.StorePath; //"/GIRComments/" + subPath + imageName;
                                //file.NoteID = noteid;
                                file.IsUpload = item.IsUpload;
                                file.DeficienciesID = 0; //item.DeficienciesID; //RDBJ 10/14/2021 Set 0
                                file.NoteUniqueID = noteid;
                                file.CommentFileUniqueID = Guid.NewGuid();
                                dbContext.GIRDeficienciesCommentFiles.Add(file);
                                dbContext.SaveChanges();
                            }
                            */
                            // End JSL 12/03/2022 commented
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveImageFileForComments " + ex.Message + "\n" + ex.InnerException);
            }
            return "";
        }
        private static string SaveImageFileForPhoto(List<Modals.GIRPhotographs> modal, long GIRID)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                foreach (var item in modal)
                {

                    var split = item.ImagePath.Split(',');
                    string OrignalString = split.LastOrDefault();
                    if (!string.IsNullOrEmpty(OrignalString))
                    {
                        Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                        OrignalString = regex.Replace(OrignalString, string.Empty);
                        byte[] imageBytes = Convert.FromBase64String(OrignalString);
                        string rootpath = HttpContext.Current.Server.MapPath("~/GIRPhotos/");

                        string subPath = GIRID.ToString() + "/";
                        bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                        string CompleteFolderPath = Path.Combine(rootpath + subPath);
                        if (!Directory.Exists(CompleteFolderPath))
                        {
                            Directory.CreateDirectory(CompleteFolderPath);
                        }
                        var imageName = GIRID.ToString() + "_" + DateTime.Now.ToString("MMdddyyyhhmmss") + "_" + item.FileName;
                        File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);

                        Entity.GIRPhotograph file = new Entity.GIRPhotograph();
                        file.FileName = "/GIRPhotos/" + subPath + imageName;
                        file.ImagePath = item.ImagePath;
                        file.GIRFormID = GIRID;
                        file.ImageCaption = item.ImageCaption;
                        dbContext.GIRPhotographs.Add(file);
                        dbContext.SaveChanges();
                    }


                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveImageFileForPhoto " + ex.Message + "\n" + ex.InnerException);
            }
            return "";
        }
        public Dictionary<string, string> GetFile(int id) // RDBJ 01/27/2022 set with dictionary
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                var data = dbContext.GIRDeficienciesFiles.Where(x => x.GIRDeficienciesFileID == id).FirstOrDefault();
                if (data != null)
                {
                    retDicData["FileData"] = data.StorePath; // RDBJ 01/27/2022 set with dictionary
                    retDicData["FileName"] = data.FileName; // RDBJ 01/27/2022 set with dictionary
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFile " + ex.Message + "\n" + ex.InnerException);

            }
            return retDicData;
        }
        public string GetFileComment(int id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            string str = "";
            try
            {
                var data = dbContext.AuditNotesCommentsFiles.Where(x => x.CommentFileID == id).FirstOrDefault();
                if (data != null)
                {
                    str = data.StorePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFileComment " + ex.Message + "\n" + ex.InnerException);
            }
            return str;
        }

        public Dictionary<string, string> GetCommentFile(string id) // RDBJ 01/27/2022 set with dictionary
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                Guid CFUID = Guid.Parse(id);
                var data = dbContext.GIRDeficienciesCommentFiles.Where(x => x.CommentFileUniqueID == CFUID).FirstOrDefault();
                if (data != null)
                {
                    retDicData["FileData"] = data.StorePath; // RDBJ 01/27/2022 set with dictionary
                    retDicData["FileName"] = data.FileName; // RDBJ 01/27/2022 set with dictionary
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFileComment " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }
        public Dictionary<string, string> GetGIRDeficienciesInitialActionFile(string id) // RDBJ 01/27/2022 set with dictionary //RDBJ 11/10/2021 changed datatype from int to string
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                Guid IAFUID = Guid.Parse(id);  //RDNJ 11/10/2021
                var data = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActFileUniqueID == IAFUID).FirstOrDefault(); //RDBJ 11/10/2021 Updated with IniActFileUniqueID
                if (data != null)
                {
                    retDicData["FileData"] = data.StorePath; // RDBJ 01/27/2022 set with dictionary
                    retDicData["FileName"] = data.FileName; // RDBJ 01/27/2022 set with dictionary
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesInitialActionFile " + ex.Message + "\n" + ex.InnerException);

            }
            return retDicData;
        }
        public Dictionary<string, string> GetGIRDeficienciesResolutionFile(string id) // RDBJ 01/27/2022 set with dictionary //RDBJ 11/10/2021 changed datatype from int to string
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                Guid RFUID = Guid.Parse(id);
                var data = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionFileUniqueID == RFUID).FirstOrDefault(); //RDBJ 11/10/2021 Updated with ResolutionFileUniqueID
                if (data != null)
                {
                    retDicData["FileData"] = data.StorePath; // RDBJ 01/27/2022 set with dictionary
                    retDicData["FileName"] = data.FileName; // RDBJ 01/27/2022 set with dictionary
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesResolutionFile " + ex.Message + "\n" + ex.InnerException);

            }
            return retDicData;
        }
        public List<Entity.GeneralInspectionReport> GetAllGIRForms()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Entity.GeneralInspectionReport> list = new List<Entity.GeneralInspectionReport>();
            var data = dbContext.GeneralInspectionReports
                         .Select(x => new { x.GIRFormID, x.ShipName, x.Date, x.Port, x.Inspector }).ToList();
            foreach (var item in data)
            {
                Entity.GeneralInspectionReport obj = new Entity.GeneralInspectionReport();
                obj.GIRFormID = item.GIRFormID;
                obj.ShipName = item.ShipName;
                obj.Date = item.Date;
                obj.Port = item.Port;
                obj.Inspector = item.Inspector;
                list.Add(obj);
            }
            return list;
        }
        public Modals.GeneralInspectionReport GIRFormDetailsView(int id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.GeneralInspectionReport dbModal = new Modals.GeneralInspectionReport();
            var Modal = dbContext.GeneralInspectionReports.Where(x => x.GIRFormID == id).FirstOrDefault();
            if (Modal != null)
            {
                dbModal.GIRFormID = Modal.GIRFormID;
                dbModal.ShipID = Modal.ShipID;
                dbModal.ShipName = Modal.ShipName;
                dbModal.Ship = Modal.Ship;
                dbModal.Port = Modal.Port;
                dbModal.Inspector = Modal.Inspector;
                dbModal.Date = Modal.Date;
                dbModal.GeneralPreamble = Modal.GeneralPreamble;
                dbModal.Classsociety = Modal.Classsociety;
                dbModal.YearofBuild = Modal.YearofBuild;
                dbModal.Flag = Modal.Flag;
                dbModal.Classofvessel = Modal.Classofvessel;
                dbModal.Portofregistry = Modal.Portofregistry;
                dbModal.MMSI = Modal.MMSI;
                dbModal.IMOnumber = Modal.IMOnumber;
                dbModal.Callsign = Modal.Callsign;
                dbModal.SummerDWT = Modal.SummerDWT;
                dbModal.Grosstonnage = Modal.Grosstonnage;
                dbModal.Lightweight = Modal.Lightweight;
                dbModal.Nettonnage = Modal.Nettonnage;
                dbModal.Beam = Modal.Beam;
                dbModal.LOA = Modal.LOA;
                dbModal.Summerdraft = Modal.Summerdraft;
                dbModal.LBP = Modal.LBP;
                dbModal.Bowthruster = Modal.Bowthruster;
                dbModal.BHP = Modal.BHP;
                dbModal.Noofholds = Modal.Noofholds;
                dbModal.Nomoveablebulkheads = Modal.Nomoveablebulkheads;
                dbModal.Containers = Modal.Containers;
                dbModal.Cargocapacity = Modal.Cargocapacity;
                dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
                dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
                dbModal.Lastvoyageandcargo = Modal.Lastvoyageandcargo;
                dbModal.CurrentPlannedvoyageandcargo = Modal.CurrentPlannedvoyageandcargo;

                dbModal.ShipboardWorkingArrangements = Modal.ShipboardWorkingArrangements;
                dbModal.CertificationIndex = Modal.CertificationIndex;
                dbModal.IsPubsAndDocsSectionComplete = Modal.IsPubsAndDocsSectionComplete;
                dbModal.CarriedOutByTheDOOW = Modal.CarriedOutByTheDOOW;
                dbModal.IsRegs4shipsDVD = Modal.IsRegs4shipsDVD;
                dbModal.Regs4shipsDVD = Modal.Regs4shipsDVD;
                dbModal.IsSOPEPPoints = Modal.IsSOPEPPoints;
                dbModal.SOPEPPoints = Modal.SOPEPPoints;
                dbModal.IsBWMP = Modal.IsBWMP;
                dbModal.BWMP = Modal.BWMP;
                dbModal.IsBWMPSupplement = Modal.IsBWMPSupplement;
                dbModal.BWMPSupplement = Modal.BWMPSupplement;
                dbModal.IsIntactStabilityManual = Modal.IsIntactStabilityManual;
                dbModal.IntactStabilityManual = Modal.IntactStabilityManual;
                dbModal.IsStabilityComputer = Modal.IsStabilityComputer;
                dbModal.StabilityComputer = Modal.StabilityComputer;
                dbModal.IsDateOfLast = Modal.IsDateOfLast;
                dbModal.DateOfLast = Modal.DateOfLast;
                dbModal.IsCargoSecuring = Modal.IsCargoSecuring;
                dbModal.CargoSecuring = Modal.CargoSecuring;
                dbModal.BulkCargo = Modal.BulkCargo;
                dbModal.BulkCargo = Modal.BulkCargo;
                dbModal.IsSMSManual = Modal.IsSMSManual;
                dbModal.SMSManual = Modal.SMSManual;
                dbModal.IsRegisterOf = Modal.IsRegisterOf;
                dbModal.RegisterOf = Modal.RegisterOf;
                dbModal.IsFleetStandingOrder = Modal.IsFleetStandingOrder;
                dbModal.FleetStandingOrder = Modal.FleetStandingOrder;
                dbModal.IsFleetMemoranda = Modal.IsFleetMemoranda;
                dbModal.FleetMemoranda = Modal.FleetMemoranda;
                dbModal.IsShipsPlans = Modal.IsShipsPlans;
                dbModal.ShipsPlans = Modal.ShipsPlans;
                dbModal.IsCollective = Modal.IsCollective;
                dbModal.Collective = Modal.Collective;
                dbModal.IsDraftAndFreeboardNotice = Modal.IsDraftAndFreeboardNotice;
                dbModal.DraftAndFreeboardNotice = Modal.DraftAndFreeboardNotice;
                dbModal.IsPCSOPEP = Modal.IsPCSOPEP;
                dbModal.PCSOPEP = Modal.PCOPEP;
                dbModal.IsNTVRP = Modal.INTVRP;
                dbModal.NTVRP = Modal.NTVRP;
                dbModal.IsVGP = Modal.IVGP;
                dbModal.VGP = Modal.VGP;
                dbModal.PubsComments = Modal.PubsComments;
                dbModal.OfficialLogbookA = Modal.OfficialLogbookA;
                dbModal.OfficialLogbookB = Modal.OfficialLogbookB;
                dbModal.OfficialLogbookC = Modal.OfficialLogbookC;
                dbModal.OfficialLogbookD = Modal.OfficialLogbookD;
                dbModal.OfficialLogbookE = Modal.OfficialLogbookE;
                dbModal.DeckLogbook = Modal.DeckLogbook;
                dbModal.Listofcrew = Modal.Listofcrew;
                dbModal.LastHose = Modal.LastHose;
                dbModal.PassagePlanning = Modal.PassagePlanning;
                dbModal.LoadingComputer = Modal.LoadingComputer;
                dbModal.EngineLogbook = Modal.EngineLogbook;
                dbModal.OilRecordBook = Modal.OilRecordBook;
                dbModal.RiskAssessments = Modal.RiskAssessments;
                dbModal.GMDSSLogbook = Modal.GMDSSLogbook;
                dbModal.DeckLogbook5D = Modal.DeckLogbook5D;
                dbModal.GarbageRecordBook = Modal.GarbageRecordBook;
                dbModal.BallastWaterRecordBook = Modal.BallastWaterRecordBook;
                dbModal.CargoRecordBook = Modal.CargoRecordBook;
                dbModal.EmissionsControlManual = Modal.EmissionsControlManual;
                dbModal.LGR = Modal.LGR;
                dbModal.PEER = Modal.PEER;
                dbModal.RecordKeepingComments = Modal.RecordKeepingComments;
                dbModal.LastPortStateControl = Modal.LastPortStateControl;
                dbModal.LiferaftsComment = Modal.LiferaftsComment;
                dbModal.releasesComment = Modal.releasesComment;
                dbModal.LifeboatComment = Modal.LifeboatComment;
                dbModal.LifeboatdavitComment = Modal.LifeboatdavitComment;
                dbModal.LifeboatequipmentComment = Modal.LifeboatequipmentComment;
                dbModal.RescueboatComment = Modal.RescueboatComment;
                dbModal.RescueboatequipmentComment = Modal.RescueboatequipmentComment;
                dbModal.RescueboatoutboardmotorComment = Modal.RescueboatoutboardmotorComment;
                dbModal.RescueboatdavitComment = Modal.RescueboatdavitComment;
                dbModal.DeckComment = Modal.DeckComment;
                dbModal.PyrotechnicsComment = Modal.PyrotechnicsComment;
                dbModal.EPIRBComment = Modal.EPIRBComment;
                dbModal.SARTsComment = Modal.SARTsComment;
                dbModal.GMDSSComment = Modal.GMDSSComment;
                dbModal.ManoverboardComment = Modal.ManoverboardComment;
                dbModal.LinethrowingapparatusComment = Modal.LinethrowingapparatusComment;
                dbModal.FireextinguishersComment = Modal.FireextinguishersComment;
                dbModal.EmergencygeneratorComment = Modal.EmergencygeneratorComment;
                dbModal.CO2roomComment = Modal.CO2roomComment;
                dbModal.SurvivalComment = Modal.SurvivalComment;
                dbModal.LifejacketComment = Modal.LifejacketComment;
                dbModal.FiremansComment = Modal.FiremansComment;
                dbModal.LifebuoysComment = Modal.LifebuoysComment;
                dbModal.FireboxesComment = Modal.FireboxesComment;
                dbModal.EmergencybellsComment = Modal.EmergencybellsComment;
                dbModal.EmergencylightingComment = Modal.EmergencylightingComment;
                dbModal.FireplanComment = Modal.FireplanComment;
                dbModal.DamageComment = Modal.DamageComment;
                dbModal.EmergencyplansComment = Modal.EmergencyplansComment;
                dbModal.MusterlistComment = Modal.MusterlistComment;
                dbModal.SafetysignsComment = Modal.SafetysignsComment;
                dbModal.EmergencysteeringComment = Modal.EmergencysteeringComment;
                dbModal.StatutoryemergencydrillsComment = Modal.StatutoryemergencydrillsComment;
                dbModal.EEBDComment = Modal.EEBDComment;
                dbModal.OxygenComment = Modal.OxygenComment;
                dbModal.MultigasdetectorComment = Modal.MultigasdetectorComment;
                dbModal.GasdetectorComment = Modal.GasdetectorComment;
                dbModal.SufficientquantityComment = Modal.SufficientquantityComment;
                dbModal.BASetsComment = Modal.BASetsComment;
                dbModal.SafetyComment = Modal.SafetyComment;
                dbModal.GangwayComment = Modal.GangwayComment;
                dbModal.RestrictedComment = Modal.RestrictedComment;
                dbModal.OutsideComment = Modal.OutsideComment;
                dbModal.EntrancedoorsComment = Modal.EntrancedoorsComment;
                dbModal.AccommodationComment = Modal.AccommodationComment;
                dbModal.GMDSSComment5G = Modal.GMDSSComment5G;
                dbModal.VariousComment = Modal.VariousComment;
                dbModal.SSOComment = Modal.SSOComment;
                dbModal.SecuritylogbookComment = Modal.SecuritylogbookComment;
                dbModal.Listoflast10portsComment = Modal.Listoflast10portsComment;
                dbModal.PFSOComment = Modal.PFSOComment;
                dbModal.SecuritylevelComment = Modal.SecuritylevelComment;
                dbModal.DrillsandtrainingComment = Modal.DrillsandtrainingComment;
                dbModal.DOSComment = Modal.DOSComment;
                dbModal.SSASComment = Modal.SSASComment;
                dbModal.VisitorslogbookComment = Modal.VisitorslogbookComment;
                dbModal.KeyregisterComment = Modal.KeyregisterComment;
                dbModal.ShipSecurityComment = Modal.ShipSecurityComment;
                dbModal.SecurityComment = Modal.SecurityComment;
                dbModal.NauticalchartsComment = Modal.NauticalchartsComment;
                dbModal.NoticetomarinersComment = Modal.NoticetomarinersComment;
                dbModal.ListofradiosignalsComment = Modal.ListofradiosignalsComment;
                dbModal.ListoflightsComment = Modal.ListoflightsComment;
                dbModal.SailingdirectionsComment = Modal.SailingdirectionsComment;
                dbModal.TidetablesComment = Modal.TidetablesComment;
                dbModal.NavtexandprinterComment = Modal.NavtexandprinterComment;
                dbModal.RadarsComment = Modal.RadarsComment;
                dbModal.GPSComment = Modal.GPSComment;
                dbModal.AISComment = Modal.AISComment;
                dbModal.VDRComment = Modal.VDRComment;
                dbModal.ECDISComment = Modal.ECDISComment;
                dbModal.EchosounderComment = Modal.EchosounderComment;
                dbModal.ADPbackuplaptopComment = Modal.ADPbackuplaptopComment;
                dbModal.ColourprinterComment = Modal.ColourprinterComment;
                dbModal.VHFDSCtransceiverComment = Modal.VHFDSCtransceiverComment;
                dbModal.radioinstallationComment = Modal.radioinstallationComment;
                dbModal.InmarsatCComment = Modal.InmarsatCComment;
                dbModal.MagneticcompassComment = Modal.MagneticcompassComment;
                dbModal.SparecompassbowlComment = Modal.SparecompassbowlComment;
                dbModal.CompassobservationbookComment = Modal.CompassobservationbookComment;
                dbModal.GyrocompassComment = Modal.GyrocompassComment;
                dbModal.RudderindicatorComment = Modal.RudderindicatorComment;
                dbModal.SpeedlogComment = Modal.SpeedlogComment;
                dbModal.NavigationComment = Modal.NavigationComment;
                dbModal.SignalflagsComment = Modal.SignalflagsComment;
                dbModal.RPMComment = Modal.RPMComment;
                dbModal.BasicmanoeuvringdataComment = Modal.BasicmanoeuvringdataComment;
                dbModal.MasterstandingordersComment = Modal.MasterstandingordersComment;
                dbModal.MasternightordersbookComment = Modal.MasternightordersbookComment;
                dbModal.SextantComment = Modal.SextantComment;
                dbModal.AzimuthmirrorComment = Modal.AzimuthmirrorComment;
                dbModal.BridgepostersComment = Modal.BridgepostersComment;
                dbModal.ReviewofplannedComment = Modal.ReviewofplannedComment;
                dbModal.BridgebellbookComment = Modal.BridgebellbookComment;
                dbModal.BridgenavigationalComment = Modal.BridgenavigationalComment;
                dbModal.SecurityEquipmentComment = Modal.SecurityEquipmentComment;
                dbModal.NavigationPost = Modal.NavigationPost;
                dbModal.GeneralComment = Modal.GeneralComment;
                dbModal.MedicinestorageComment = Modal.MedicinestorageComment;
                dbModal.MedicinechestcertificateComment = Modal.MedicinechestcertificateComment;
                dbModal.InventoryStoresComment = Modal.InventoryStoresComment;
                dbModal.OxygencylindersComment = Modal.OxygencylindersComment;
                dbModal.StretcherComment = Modal.StretcherComment;
                dbModal.SalivaComment = Modal.SalivaComment;
                dbModal.AlcoholComment = Modal.AlcoholComment;
                dbModal.HospitalComment = Modal.HospitalComment;
                dbModal.GeneralGalleyComment = Modal.GeneralGalleyComment;
                dbModal.HygieneComment = Modal.HygieneComment;
                dbModal.FoodstorageComment = Modal.FoodstorageComment;
                dbModal.FoodlabellingComment = Modal.FoodlabellingComment;
                dbModal.GalleyriskassessmentComment = Modal.GalleyriskassessmentComment;
                dbModal.FridgetemperatureComment = Modal.FridgetemperatureComment;
                dbModal.FoodandProvisionsComment = Modal.FoodandProvisionsComment;
                dbModal.GalleyComment = Modal.GalleyComment;
                dbModal.ConditionComment = Modal.ConditionComment;
                dbModal.PaintworkComment = Modal.PaintworkComment;
                dbModal.LightingComment = Modal.LightingComment;
                dbModal.PlatesComment = Modal.PlatesComment;
                dbModal.BilgesComment = Modal.BilgesComment;
                dbModal.PipelinesandvalvesComment = Modal.PipelinesandvalvesComment;
                dbModal.LeakageComment = Modal.LeakageComment;
                dbModal.EquipmentComment = Modal.EquipmentComment;
                dbModal.OilywaterseparatorComment = Modal.OilywaterseparatorComment;
                dbModal.FueloiltransferplanComment = Modal.FueloiltransferplanComment;
                dbModal.SteeringgearComment = Modal.SteeringgearComment;
                dbModal.WorkshopandequipmentComment = Modal.WorkshopandequipmentComment;
                dbModal.SoundingpipesComment = Modal.SoundingpipesComment;
                dbModal.EnginecontrolComment = Modal.EnginecontrolComment;
                dbModal.ChiefEngineernightordersbookComment = Modal.ChiefEngineernightordersbookComment;
                dbModal.ChiefEngineerstandingordersComment = Modal.ChiefEngineerstandingordersComment;
                dbModal.PreUMSComment = Modal.PreUMSComment;
                dbModal.EnginebellbookComment = Modal.EnginebellbookComment;
                dbModal.LockoutComment = Modal.LockoutComment;
                dbModal.EngineRoomComment = Modal.EngineRoomComment;
                dbModal.CleanlinessandhygieneComment = Modal.CleanlinessandhygieneComment;
                dbModal.ConditionComment5M = Modal.ConditionComment5M;
                dbModal.PaintworkComment5M = Modal.PaintworkComment5M;
                dbModal.SignalmastandstaysComment = Modal.SignalmastandstaysComment;
                dbModal.MonkeyislandComment = Modal.MonkeyislandComment;
                dbModal.FireDampersComment = Modal.FireDampersComment;
                dbModal.RailsBulwarksComment = Modal.RailsBulwarksComment;
                dbModal.WatertightdoorsComment = Modal.WatertightdoorsComment;
                dbModal.VentilatorsComment = Modal.VentilatorsComment;
                dbModal.WinchesComment = Modal.WinchesComment;
                dbModal.FairleadsComment = Modal.FairleadsComment;
                dbModal.MooringLinesComment = Modal.MooringLinesComment;
                dbModal.EmergencyShutOffsComment = Modal.EmergencyShutOffsComment;
                dbModal.RadioaerialsComment = Modal.RadioaerialsComment;
                dbModal.SOPEPlockerComment = Modal.SOPEPlockerComment;
                dbModal.ChemicallockerComment = Modal.ChemicallockerComment;
                dbModal.AntislippaintComment = Modal.AntislippaintComment;
                dbModal.SuperstructureComment = Modal.SuperstructureComment;
                dbModal.CabinsComment = Modal.CabinsComment;
                dbModal.OfficesComment = Modal.OfficesComment;
                dbModal.MessroomsComment = Modal.MessroomsComment;
                dbModal.ToiletsComment = Modal.ToiletsComment;
                dbModal.LaundryroomComment = Modal.LaundryroomComment;
                dbModal.ChangingroomComment = Modal.ChangingroomComment;
                dbModal.OtherComment = Modal.OtherComment;
                dbModal.ConditionComment5N = Modal.ConditionComment5N;
                dbModal.SelfclosingfiredoorsComment = Modal.SelfclosingfiredoorsComment;
                dbModal.StairwellsComment = Modal.StairwellsComment;
                dbModal.SuperstructureInternalComment = Modal.SuperstructureInternalComment;
                dbModal.PortablegangwayComment = Modal.PortablegangwayComment;
                dbModal.SafetynetComment = Modal.SafetynetComment;
                dbModal.AccommodationLadderComment = Modal.AccommodationLadderComment;
                dbModal.SafeaccessprovidedComment = Modal.SafeaccessprovidedComment;
                dbModal.PilotladdersComment = Modal.PilotladdersComment;
                dbModal.BoardingEquipmentComment = Modal.BoardingEquipmentComment;
                dbModal.CleanlinessComment = Modal.CleanlinessComment;
                dbModal.PaintworkComment5P = Modal.PaintworkComment5P;
                dbModal.ShipsiderailsComment = Modal.ShipsiderailsComment;
                dbModal.WeathertightdoorsComment = Modal.WeathertightdoorsComment;
                dbModal.FirehydrantsComment = Modal.FirehydrantsComment;
                dbModal.VentilatorsComment5P = Modal.VentilatorsComment5P;
                dbModal.ManholecoversComment = Modal.ManholecoversComment;
                dbModal.MainDeckAreaComment = Modal.MainDeckAreaComment;
                dbModal.ConditionComment5Q = Modal.ConditionComment5Q;
                dbModal.PaintworkComment5Q = Modal.PaintworkComment5Q;
                dbModal.MechanicaldamageComment = Modal.MechanicaldamageComment;
                dbModal.AccessladdersComment = Modal.AccessladdersComment;
                dbModal.ManholecoversComment5Q = Modal.ManholecoversComment5Q;
                dbModal.HoldbilgeComment = Modal.HoldbilgeComment;
                dbModal.AccessdoorsComment = Modal.AccessdoorsComment;
                dbModal.ConditionHatchCoversComment = Modal.ConditionHatchCoversComment;
                dbModal.PaintworkHatchCoversComment = Modal.PaintworkHatchCoversComment;
                dbModal.RubbersealsComment = Modal.RubbersealsComment;
                dbModal.SignsofhatchesComment = Modal.SignsofhatchesComment;
                dbModal.SealingtapeComment = Modal.SealingtapeComment;
                dbModal.ConditionofhydraulicsComment = Modal.ConditionofhydraulicsComment;
                dbModal.PortablebulkheadsComment = Modal.PortablebulkheadsComment;
                dbModal.TweendecksComment = Modal.TweendecksComment;
                dbModal.HatchcoamingComment = Modal.HatchcoamingComment;
                dbModal.ConditionCargoCranesComment = Modal.ConditionCargoCranesComment;
                dbModal.GantrycranealarmComment = Modal.GantrycranealarmComment;
                dbModal.GantryconditionComment = Modal.GantryconditionComment;
                dbModal.CargoHoldsComment = Modal.CargoHoldsComment;
                dbModal.CleanlinessComment5R = Modal.CleanlinessComment5R;
                dbModal.PaintworkComment5R = Modal.PaintworkComment5R;
                dbModal.TriphazardsComment = Modal.TriphazardsComment;
                dbModal.WindlassComment = Modal.WindlassComment;
                dbModal.CablesComment = Modal.CablesComment;
                dbModal.WinchesComment5R = Modal.WinchesComment;
                dbModal.FairleadsComment5R = Modal.FairleadsComment;
                dbModal.MooringComment = Modal.MooringComment;
                dbModal.HatchToforecastlespaceComment = Modal.HatchToforecastlespaceComment;
                dbModal.VentilatorsComment5R = Modal.VentilatorsComment5R;
                dbModal.BellComment = Modal.BellComment;
                dbModal.ForemastComment = Modal.ForemastComment;
                dbModal.FireComment = Modal.FireComment;
                dbModal.RailsComment = Modal.RailsComment;
                dbModal.AntislippaintComment5R = Modal.AntislippaintComment5R;
                dbModal.ForecastleComment = Modal.ForecastleComment;
                dbModal.CleanlinessComment5S = Modal.CleanlinessComment5S;
                dbModal.PaintworkComment5S = Modal.PaintworkComment5S;
                dbModal.ForepeakComment = Modal.ForepeakComment;
                dbModal.ChainlockerComment = Modal.ChainlockerComment;
                dbModal.LightingComment5S = Modal.LightingComment5S;
                dbModal.AccesssafetychainComment = Modal.AccessdoorsComment;
                dbModal.EmergencyfirepumpComment = Modal.EmergencyfirepumpComment;
                dbModal.BowthrusterandroomComment = Modal.BowthrusterandroomComment;
                dbModal.SparemooringlinesComment = Modal.SparemooringlinesComment;
                dbModal.PaintlockerComment = Modal.PaintlockerComment;
                dbModal.ForecastleSpaceComment = Modal.ForecastleSpaceComment;
                dbModal.BoottopComment = Modal.BoottopComment;
                dbModal.TopsidesComment = Modal.TopsidesComment;
                dbModal.AntifoulingComment = Modal.AntifoulingComment;
                dbModal.DraftandplimsollComment = Modal.DraftandplimsollComment;
                dbModal.FoulingComment = Modal.FoulingComment;
                dbModal.MechanicalComment = Modal.MechanicalComment;
                dbModal.HullComment = Modal.HullComment;
                dbModal.SummaryComment = Modal.SummaryComment;

                dbModal.SnapBackZoneComment = Modal.SnapBackZoneComment;
                dbModal.ConditionGantryCranesComment = Modal.ConditionGantryCranesComment;
                dbModal.CylindersLockerComment = Modal.CylindersLockerComment;
                dbModal.MedicalLogBookComment = Modal.MedicalLogBookComment;
                dbModal.DrugsNarcoticsComment = Modal.DrugsNarcoticsComment;
                dbModal.DefibrillatorComment = Modal.DefibrillatorComment;
                dbModal.RPWaterHandbook = Modal.RPWaterHandbook;
                dbModal.BioRPWH = Modal.BioRPWH;
                dbModal.PRE = Modal.PRE;
                dbModal.NoiseVibrationFile = Modal.NoiseVibrationFile;
                dbModal.BioMPR = Modal.BioMPR;
                dbModal.AsbestosPlan = Modal.AsbestosPlan;
                dbModal.ShipPublicAddrComment = Modal.ShipPublicAddrComment;
                dbModal.BridgewindowswiperssprayComment = Modal.BridgewindowswiperssprayComment;
                dbModal.BridgewindowswipersComment = Modal.BridgewindowswipersComment;
                dbModal.DaylightSignalsComment = Modal.DaylightSignalsComment;
                dbModal.LiferaftDavitComment = Modal.LiferaftDavitComment;
                dbModal.SnapBackZone5NComment = Modal.SnapBackZone5NComment;
                dbModal.ADPPublicationsComment = Modal.ADPPublicationsComment;

                //RDBJ 10/20/2021
                dbModal.IsGeneralSectionComplete = Modal.IsGeneralSectionComplete;
                dbModal.IsManningSectionComplete = Modal.IsManningSectionComplete;
                dbModal.IsShipCertificationSectionComplete = Modal.IsShipCertificationSectionComplete;
                dbModal.IsRecordKeepingSectionComplete = Modal.IsRecordKeepingSectionComplete;
                dbModal.IsSafetyEquipmentSectionComplete = Modal.IsSafetyEquipmentSectionComplete;
                dbModal.IsSecuritySectionComplete = Modal.IsSecuritySectionComplete;
                dbModal.IsBridgeSectionComplete = Modal.IsBridgeSectionComplete;
                dbModal.IsMedicalSectionComplete = Modal.IsMedicalSectionComplete;
                dbModal.IsGalleySectionComplete = Modal.IsGalleySectionComplete;
                dbModal.IsEngineRoomSectionComplete = Modal.IsEngineRoomSectionComplete;
                dbModal.IsSuperstructureSectionComplete = Modal.IsSuperstructureSectionComplete;
                dbModal.IsDeckSectionComplete = Modal.IsDeckSectionComplete;
                dbModal.IsHoldsAndCoverSectionComplete = Modal.IsHoldsAndCoverSectionComplete;
                dbModal.IsForeCastleSectionComplete = Modal.IsForeCastleSectionComplete;
                dbModal.IsHullSectionComplete = Modal.IsHullSectionComplete;
                dbModal.IsSummarySectionComplete = Modal.IsSummarySectionComplete;
                dbModal.IsDeficienciesSectionComplete = Modal.IsDeficienciesSectionComplete;
                dbModal.IsPhotographsSectionComplete = Modal.IsPhotographsSectionComplete;
                //End RDBJ 10/20/2021

                dbModal.IsSynced = Modal.IsSynced;
                dbModal.CreatedDate = Modal.CreatedDate;
                dbModal.UpdatedDate = Modal.UpdatedDate;
                dbModal.SavedAsDraft = Modal.SavedAsDraft;
                dbModal.GIRDeficiencies = Modal.GIRDeficiencies.Select(y => new GIRDeficiencies()
                {
                    No = y.No,
                    Section = y.Section,
                    Deficiency = y.Deficiency,
                    DateClosed = y.DateClosed,
                    DateRaised = y.DateRaised,
                    DeficienciesID = y.DeficienciesID,
                    GIRDeficienciesFile = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == y.DeficienciesID).Select(x => new Modals.GIRDeficienciesFile()
                    {
                        FileName = x.FileName,
                        StorePath = x.StorePath,
                        DeficienciesID = x.DeficienciesID,
                        GIRDeficienciesFileID = x.GIRDeficienciesFileID
                    }).ToList()
                }).ToList();
                dbModal.GIRPhotographs = Modal.GIRPhotographs.Select(y => new GIRPhotographs()
                {
                    GIRFormID = y.GIRFormID,
                    ImagePath = y.ImagePath,
                    PhotographsID = y.PhotographsID,
                    ImageCaption = y.ImageCaption,
                    FileName = !string.IsNullOrEmpty(y.FileName) && y.FileName.IndexOf('_') > 0 ? y.FileName.Substring(y.FileName.IndexOf('_') + 1) : y.FileName,
                }).ToList();
                dbModal.GIRSafeManningRequirements = Modal.GlRSafeManningRequirements.Select(y => new GlRSafeManningRequirements()
                {
                    GIRFormID = y.GIRFormID,
                    Rank = y.Rank,
                    OnBoard = y.OnBoard,
                    RequiredbySMD = y.RequiredbySMD,
                    SafeManningRequirementsID = y.SafeManningRequirementsID

                }).ToList();
                dbModal.GIRCrewDocuments = Modal.GlRCrewDocuments.Select(y => new GlRCrewDocuments()
                {
                    GIRFormID = y.GIRFormID,
                    CrewmemberName = y.CrewmemberName,
                    CertificationDetail = y.CertificationDetail,
                    CrewDocumentsID = y.CrewDocumentsID

                }).ToList();
                dbModal.GIRRestandWorkHours = Modal.GIRRestandWorkHours.Select(y => new GIRRestandWorkHours()
                {
                    GIRFormID = y.GIRFormID,
                    CrewmemberName = y.CrewmemberName,
                    RestAndWorkDetail = y.RestAndWorkDetail,
                    RestandWorkHoursID = y.RestandWorkHoursID

                }).ToList();
            }
            return dbModal;
        }
        public Modals.GeneralInspectionReport GIRFormDetailsViewByGUID(string id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.GeneralInspectionReport dbModal = new Modals.GeneralInspectionReport();
            var UniqueFormID = new Guid(id);
            var Modal = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
            var modalSafeManning = dbContext.GlRSafeManningRequirements.Where(x => x.UniqueFormID == UniqueFormID).ToList();
            var modalCrewDocuments = dbContext.GlRCrewDocuments.Where(x => x.UniqueFormID == UniqueFormID).ToList();
            var modalRestandWorks = dbContext.GIRRestandWorkHours.Where(x => x.UniqueFormID == UniqueFormID).ToList();
            var modalGIRPhotographs = dbContext.GIRPhotographs.Where(x => x.UniqueFormID == UniqueFormID).ToList(); // RDBJ 12/01/2021

            // JSL 12/03/2022
            if (modalGIRPhotographs != null && modalGIRPhotographs.Count > 0)
            {
                foreach (var itemGIRPhoto in modalGIRPhotographs)
                {
                    if (itemGIRPhoto.ImagePath.StartsWith("data:"))
                    {
                        Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                        dicFileMetaData["UniqueFormID"] = Convert.ToString(UniqueFormID);
                        dicFileMetaData["ReportType"] = "GI";
                        dicFileMetaData["FileName"] = itemGIRPhoto.FileName;
                        dicFileMetaData["Base64FileData"] = itemGIRPhoto.ImagePath;

                        itemGIRPhoto.ImagePath = Utility.ConvertBase64IntoFile(dicFileMetaData, true);
                        dbContext.SaveChanges();
                    }
                }
            }
            // End JSL 12/03/2022

            if (Modal != null)
            {
                dbModal.UniqueFormID = Modal.UniqueFormID;
                dbModal.FormVersion = Modal.FormVersion;

                dbModal.GIRFormID = Modal.GIRFormID;
                dbModal.ShipID = Modal.ShipID;
                dbModal.ShipName = Modal.ShipName;
                dbModal.Ship = Modal.Ship;
                dbModal.Port = Modal.Port;
                dbModal.Inspector = Modal.Inspector;
                dbModal.Date = Modal.Date;
                dbModal.GeneralPreamble = Modal.GeneralPreamble;
                dbModal.Classsociety = Modal.Classsociety;
                dbModal.YearofBuild = Modal.YearofBuild;
                dbModal.Flag = Modal.Flag;
                dbModal.Classofvessel = Modal.Classofvessel;
                dbModal.Portofregistry = Modal.Portofregistry;
                dbModal.MMSI = Modal.MMSI;
                dbModal.IMOnumber = Modal.IMOnumber;
                dbModal.Callsign = Modal.Callsign;
                dbModal.SummerDWT = Modal.SummerDWT;
                dbModal.Grosstonnage = Modal.Grosstonnage;
                dbModal.Lightweight = Modal.Lightweight;
                dbModal.Nettonnage = Modal.Nettonnage;
                dbModal.Beam = Modal.Beam;
                dbModal.LOA = Modal.LOA;
                dbModal.Summerdraft = Modal.Summerdraft;
                dbModal.LBP = Modal.LBP;
                dbModal.Bowthruster = Modal.Bowthruster;
                dbModal.BHP = Modal.BHP;
                dbModal.Noofholds = Modal.Noofholds;
                dbModal.Nomoveablebulkheads = Modal.Nomoveablebulkheads;
                dbModal.Containers = Modal.Containers;
                dbModal.Cargocapacity = Modal.Cargocapacity;
                dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
                dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
                dbModal.Lastvoyageandcargo = Modal.Lastvoyageandcargo;
                dbModal.CurrentPlannedvoyageandcargo = Modal.CurrentPlannedvoyageandcargo;

                dbModal.ShipboardWorkingArrangements = Modal.ShipboardWorkingArrangements;
                dbModal.CertificationIndex = Modal.CertificationIndex;
                dbModal.IsPubsAndDocsSectionComplete = Modal.IsPubsAndDocsSectionComplete;
                dbModal.CarriedOutByTheDOOW = Modal.CarriedOutByTheDOOW;
                dbModal.IsRegs4shipsDVD = Modal.IsRegs4shipsDVD;
                dbModal.Regs4shipsDVD = Modal.Regs4shipsDVD;
                dbModal.IsSOPEPPoints = Modal.IsSOPEPPoints;
                dbModal.SOPEPPoints = Modal.SOPEPPoints;
                dbModal.IsBWMP = Modal.IsBWMP;
                dbModal.BWMP = Modal.BWMP;
                dbModal.IsBWMPSupplement = Modal.IsBWMPSupplement;
                dbModal.BWMPSupplement = Modal.BWMPSupplement;
                dbModal.IsIntactStabilityManual = Modal.IsIntactStabilityManual;
                dbModal.IntactStabilityManual = Modal.IntactStabilityManual;
                dbModal.IsStabilityComputer = Modal.IsStabilityComputer;
                dbModal.StabilityComputer = Modal.StabilityComputer;
                dbModal.IsDateOfLast = Modal.IsDateOfLast;
                dbModal.DateOfLast = Modal.DateOfLast;
                dbModal.IsCargoSecuring = Modal.IsCargoSecuring;
                dbModal.CargoSecuring = Modal.CargoSecuring;
                dbModal.BulkCargo = Modal.BulkCargo;
                dbModal.BulkCargo = Modal.BulkCargo;
                dbModal.IsSMSManual = Modal.IsSMSManual;
                dbModal.SMSManual = Modal.SMSManual;
                dbModal.IsRegisterOf = Modal.IsRegisterOf;
                dbModal.RegisterOf = Modal.RegisterOf;
                dbModal.IsFleetStandingOrder = Modal.IsFleetStandingOrder;
                dbModal.FleetStandingOrder = Modal.FleetStandingOrder;
                dbModal.IsFleetMemoranda = Modal.IsFleetMemoranda;
                dbModal.FleetMemoranda = Modal.FleetMemoranda;
                dbModal.IsShipsPlans = Modal.IsShipsPlans;
                dbModal.ShipsPlans = Modal.ShipsPlans;
                dbModal.IsCollective = Modal.IsCollective;
                dbModal.Collective = Modal.Collective;
                dbModal.IsDraftAndFreeboardNotice = Modal.IsDraftAndFreeboardNotice;
                dbModal.DraftAndFreeboardNotice = Modal.DraftAndFreeboardNotice;
                dbModal.IsPCSOPEP = Modal.IsPCSOPEP;
                dbModal.PCSOPEP = Modal.PCOPEP;
                dbModal.IsNTVRP = Modal.INTVRP;
                dbModal.NTVRP = Modal.NTVRP;
                dbModal.IsVGP = Modal.IVGP;
                dbModal.VGP = Modal.VGP;
                dbModal.PubsComments = Modal.PubsComments;
                dbModal.OfficialLogbookA = Modal.OfficialLogbookA;
                dbModal.OfficialLogbookB = Modal.OfficialLogbookB;
                dbModal.OfficialLogbookC = Modal.OfficialLogbookC;
                dbModal.OfficialLogbookD = Modal.OfficialLogbookD;
                dbModal.OfficialLogbookE = Modal.OfficialLogbookE;
                dbModal.DeckLogbook = Modal.DeckLogbook;
                dbModal.Listofcrew = Modal.Listofcrew;
                dbModal.LastHose = Modal.LastHose;
                dbModal.PassagePlanning = Modal.PassagePlanning;
                dbModal.LoadingComputer = Modal.LoadingComputer;
                dbModal.EngineLogbook = Modal.EngineLogbook;
                dbModal.OilRecordBook = Modal.OilRecordBook;
                dbModal.RiskAssessments = Modal.RiskAssessments;
                dbModal.GMDSSLogbook = Modal.GMDSSLogbook;
                dbModal.DeckLogbook5D = Modal.DeckLogbook5D;
                dbModal.GarbageRecordBook = Modal.GarbageRecordBook;
                dbModal.BallastWaterRecordBook = Modal.BallastWaterRecordBook;
                dbModal.CargoRecordBook = Modal.CargoRecordBook;
                dbModal.EmissionsControlManual = Modal.EmissionsControlManual;
                dbModal.LGR = Modal.LGR;
                dbModal.PEER = Modal.PEER;
                dbModal.RecordKeepingComments = Modal.RecordKeepingComments;
                dbModal.LastPortStateControl = Modal.LastPortStateControl;
                dbModal.LiferaftsComment = Modal.LiferaftsComment;
                dbModal.releasesComment = Modal.releasesComment;
                dbModal.LifeboatComment = Modal.LifeboatComment;
                dbModal.LifeboatdavitComment = Modal.LifeboatdavitComment;
                dbModal.LifeboatequipmentComment = Modal.LifeboatequipmentComment;
                dbModal.RescueboatComment = Modal.RescueboatComment;
                dbModal.RescueboatequipmentComment = Modal.RescueboatequipmentComment;
                dbModal.RescueboatoutboardmotorComment = Modal.RescueboatoutboardmotorComment;
                dbModal.RescueboatdavitComment = Modal.RescueboatdavitComment;
                dbModal.DeckComment = Modal.DeckComment;
                dbModal.PyrotechnicsComment = Modal.PyrotechnicsComment;
                dbModal.EPIRBComment = Modal.EPIRBComment;
                dbModal.SARTsComment = Modal.SARTsComment;
                dbModal.GMDSSComment = Modal.GMDSSComment;
                dbModal.ManoverboardComment = Modal.ManoverboardComment;
                dbModal.LinethrowingapparatusComment = Modal.LinethrowingapparatusComment;
                dbModal.FireextinguishersComment = Modal.FireextinguishersComment;
                dbModal.EmergencygeneratorComment = Modal.EmergencygeneratorComment;
                dbModal.CO2roomComment = Modal.CO2roomComment;
                dbModal.SurvivalComment = Modal.SurvivalComment;
                dbModal.LifejacketComment = Modal.LifejacketComment;
                dbModal.FiremansComment = Modal.FiremansComment;
                dbModal.LifebuoysComment = Modal.LifebuoysComment;
                dbModal.FireboxesComment = Modal.FireboxesComment;
                dbModal.EmergencybellsComment = Modal.EmergencybellsComment;
                dbModal.EmergencylightingComment = Modal.EmergencylightingComment;
                dbModal.FireplanComment = Modal.FireplanComment;
                dbModal.DamageComment = Modal.DamageComment;
                dbModal.EmergencyplansComment = Modal.EmergencyplansComment;
                dbModal.MusterlistComment = Modal.MusterlistComment;
                dbModal.SafetysignsComment = Modal.SafetysignsComment;
                dbModal.EmergencysteeringComment = Modal.EmergencysteeringComment;
                dbModal.StatutoryemergencydrillsComment = Modal.StatutoryemergencydrillsComment;
                dbModal.EEBDComment = Modal.EEBDComment;
                dbModal.OxygenComment = Modal.OxygenComment;
                dbModal.MultigasdetectorComment = Modal.MultigasdetectorComment;
                dbModal.GasdetectorComment = Modal.GasdetectorComment;
                dbModal.SufficientquantityComment = Modal.SufficientquantityComment;
                dbModal.BASetsComment = Modal.BASetsComment;
                dbModal.SafetyComment = Modal.SafetyComment;
                dbModal.GangwayComment = Modal.GangwayComment;
                dbModal.RestrictedComment = Modal.RestrictedComment;
                dbModal.OutsideComment = Modal.OutsideComment;
                dbModal.EntrancedoorsComment = Modal.EntrancedoorsComment;
                dbModal.AccommodationComment = Modal.AccommodationComment;
                dbModal.GMDSSComment5G = Modal.GMDSSComment5G;
                dbModal.VariousComment = Modal.VariousComment;
                dbModal.SSOComment = Modal.SSOComment;
                dbModal.SecuritylogbookComment = Modal.SecuritylogbookComment;
                dbModal.Listoflast10portsComment = Modal.Listoflast10portsComment;
                dbModal.PFSOComment = Modal.PFSOComment;
                dbModal.SecuritylevelComment = Modal.SecuritylevelComment;
                dbModal.DrillsandtrainingComment = Modal.DrillsandtrainingComment;
                dbModal.DOSComment = Modal.DOSComment;
                dbModal.SSASComment = Modal.SSASComment;
                dbModal.VisitorslogbookComment = Modal.VisitorslogbookComment;
                dbModal.KeyregisterComment = Modal.KeyregisterComment;
                dbModal.ShipSecurityComment = Modal.ShipSecurityComment;
                dbModal.SecurityComment = Modal.SecurityComment;
                dbModal.NauticalchartsComment = Modal.NauticalchartsComment;
                dbModal.NoticetomarinersComment = Modal.NoticetomarinersComment;
                dbModal.ListofradiosignalsComment = Modal.ListofradiosignalsComment;
                dbModal.ListoflightsComment = Modal.ListoflightsComment;
                dbModal.SailingdirectionsComment = Modal.SailingdirectionsComment;
                dbModal.TidetablesComment = Modal.TidetablesComment;
                dbModal.NavtexandprinterComment = Modal.NavtexandprinterComment;
                dbModal.RadarsComment = Modal.RadarsComment;
                dbModal.GPSComment = Modal.GPSComment;
                dbModal.AISComment = Modal.AISComment;
                dbModal.VDRComment = Modal.VDRComment;
                dbModal.ECDISComment = Modal.ECDISComment;
                dbModal.EchosounderComment = Modal.EchosounderComment;
                dbModal.ADPbackuplaptopComment = Modal.ADPbackuplaptopComment;
                dbModal.ColourprinterComment = Modal.ColourprinterComment;
                dbModal.VHFDSCtransceiverComment = Modal.VHFDSCtransceiverComment;
                dbModal.radioinstallationComment = Modal.radioinstallationComment;
                dbModal.InmarsatCComment = Modal.InmarsatCComment;
                dbModal.MagneticcompassComment = Modal.MagneticcompassComment;
                dbModal.SparecompassbowlComment = Modal.SparecompassbowlComment;
                dbModal.CompassobservationbookComment = Modal.CompassobservationbookComment;
                dbModal.GyrocompassComment = Modal.GyrocompassComment;
                dbModal.RudderindicatorComment = Modal.RudderindicatorComment;
                dbModal.SpeedlogComment = Modal.SpeedlogComment;
                dbModal.NavigationComment = Modal.NavigationComment;
                dbModal.SignalflagsComment = Modal.SignalflagsComment;
                dbModal.RPMComment = Modal.RPMComment;
                dbModal.BasicmanoeuvringdataComment = Modal.BasicmanoeuvringdataComment;
                dbModal.MasterstandingordersComment = Modal.MasterstandingordersComment;
                dbModal.MasternightordersbookComment = Modal.MasternightordersbookComment;
                dbModal.SextantComment = Modal.SextantComment;
                dbModal.AzimuthmirrorComment = Modal.AzimuthmirrorComment;
                dbModal.BridgepostersComment = Modal.BridgepostersComment;
                dbModal.ReviewofplannedComment = Modal.ReviewofplannedComment;
                dbModal.BridgebellbookComment = Modal.BridgebellbookComment;
                dbModal.BridgenavigationalComment = Modal.BridgenavigationalComment;
                dbModal.SecurityEquipmentComment = Modal.SecurityEquipmentComment;
                dbModal.NavigationPost = Modal.NavigationPost;
                dbModal.GeneralComment = Modal.GeneralComment;
                dbModal.MedicinestorageComment = Modal.MedicinestorageComment;
                dbModal.MedicinechestcertificateComment = Modal.MedicinechestcertificateComment;
                dbModal.InventoryStoresComment = Modal.InventoryStoresComment;
                dbModal.OxygencylindersComment = Modal.OxygencylindersComment;
                dbModal.StretcherComment = Modal.StretcherComment;
                dbModal.SalivaComment = Modal.SalivaComment;
                dbModal.AlcoholComment = Modal.AlcoholComment;
                dbModal.HospitalComment = Modal.HospitalComment;
                dbModal.GeneralGalleyComment = Modal.GeneralGalleyComment;
                dbModal.HygieneComment = Modal.HygieneComment;
                dbModal.FoodstorageComment = Modal.FoodstorageComment;
                dbModal.FoodlabellingComment = Modal.FoodlabellingComment;
                dbModal.GalleyriskassessmentComment = Modal.GalleyriskassessmentComment;
                dbModal.FridgetemperatureComment = Modal.FridgetemperatureComment;
                dbModal.FoodandProvisionsComment = Modal.FoodandProvisionsComment;
                dbModal.GalleyComment = Modal.GalleyComment;
                dbModal.ConditionComment = Modal.ConditionComment;
                dbModal.PaintworkComment = Modal.PaintworkComment;
                dbModal.LightingComment = Modal.LightingComment;
                dbModal.PlatesComment = Modal.PlatesComment;
                dbModal.BilgesComment = Modal.BilgesComment;
                dbModal.PipelinesandvalvesComment = Modal.PipelinesandvalvesComment;
                dbModal.LeakageComment = Modal.LeakageComment;
                dbModal.EquipmentComment = Modal.EquipmentComment;
                dbModal.OilywaterseparatorComment = Modal.OilywaterseparatorComment;
                dbModal.FueloiltransferplanComment = Modal.FueloiltransferplanComment;
                dbModal.SteeringgearComment = Modal.SteeringgearComment;
                dbModal.WorkshopandequipmentComment = Modal.WorkshopandequipmentComment;
                dbModal.SoundingpipesComment = Modal.SoundingpipesComment;
                dbModal.EnginecontrolComment = Modal.EnginecontrolComment;
                dbModal.ChiefEngineernightordersbookComment = Modal.ChiefEngineernightordersbookComment;
                dbModal.ChiefEngineerstandingordersComment = Modal.ChiefEngineerstandingordersComment;
                dbModal.PreUMSComment = Modal.PreUMSComment;
                dbModal.EnginebellbookComment = Modal.EnginebellbookComment;
                dbModal.LockoutComment = Modal.LockoutComment;
                dbModal.EngineRoomComment = Modal.EngineRoomComment;
                dbModal.CleanlinessandhygieneComment = Modal.CleanlinessandhygieneComment;
                dbModal.ConditionComment5M = Modal.ConditionComment5M;
                dbModal.PaintworkComment5M = Modal.PaintworkComment5M;
                dbModal.SignalmastandstaysComment = Modal.SignalmastandstaysComment;
                dbModal.MonkeyislandComment = Modal.MonkeyislandComment;
                dbModal.FireDampersComment = Modal.FireDampersComment;
                dbModal.RailsBulwarksComment = Modal.RailsBulwarksComment;
                dbModal.WatertightdoorsComment = Modal.WatertightdoorsComment;
                dbModal.VentilatorsComment = Modal.VentilatorsComment;
                dbModal.WinchesComment = Modal.WinchesComment;
                dbModal.FairleadsComment = Modal.FairleadsComment;
                dbModal.MooringLinesComment = Modal.MooringLinesComment;
                dbModal.EmergencyShutOffsComment = Modal.EmergencyShutOffsComment;
                dbModal.RadioaerialsComment = Modal.RadioaerialsComment;
                dbModal.SOPEPlockerComment = Modal.SOPEPlockerComment;
                dbModal.ChemicallockerComment = Modal.ChemicallockerComment;
                dbModal.AntislippaintComment = Modal.AntislippaintComment;
                dbModal.SuperstructureComment = Modal.SuperstructureComment;
                dbModal.CabinsComment = Modal.CabinsComment;
                dbModal.OfficesComment = Modal.OfficesComment;
                dbModal.MessroomsComment = Modal.MessroomsComment;
                dbModal.ToiletsComment = Modal.ToiletsComment;
                dbModal.LaundryroomComment = Modal.LaundryroomComment;
                dbModal.ChangingroomComment = Modal.ChangingroomComment;
                dbModal.OtherComment = Modal.OtherComment;
                dbModal.ConditionComment5N = Modal.ConditionComment5N;
                dbModal.SelfclosingfiredoorsComment = Modal.SelfclosingfiredoorsComment;
                dbModal.StairwellsComment = Modal.StairwellsComment;
                dbModal.SuperstructureInternalComment = Modal.SuperstructureInternalComment;
                dbModal.PortablegangwayComment = Modal.PortablegangwayComment;
                dbModal.SafetynetComment = Modal.SafetynetComment;
                dbModal.AccommodationLadderComment = Modal.AccommodationLadderComment;
                dbModal.SafeaccessprovidedComment = Modal.SafeaccessprovidedComment;
                dbModal.PilotladdersComment = Modal.PilotladdersComment;
                dbModal.BoardingEquipmentComment = Modal.BoardingEquipmentComment;
                dbModal.CleanlinessComment = Modal.CleanlinessComment;
                dbModal.PaintworkComment5P = Modal.PaintworkComment5P;
                dbModal.ShipsiderailsComment = Modal.ShipsiderailsComment;
                dbModal.WeathertightdoorsComment = Modal.WeathertightdoorsComment;
                dbModal.FirehydrantsComment = Modal.FirehydrantsComment;
                dbModal.VentilatorsComment5P = Modal.VentilatorsComment5P;
                dbModal.ManholecoversComment = Modal.ManholecoversComment;
                dbModal.MainDeckAreaComment = Modal.MainDeckAreaComment;
                dbModal.ConditionComment5Q = Modal.ConditionComment5Q;
                dbModal.PaintworkComment5Q = Modal.PaintworkComment5Q;
                dbModal.MechanicaldamageComment = Modal.MechanicaldamageComment;
                dbModal.AccessladdersComment = Modal.AccessladdersComment;
                dbModal.ManholecoversComment5Q = Modal.ManholecoversComment5Q;
                dbModal.HoldbilgeComment = Modal.HoldbilgeComment;
                dbModal.AccessdoorsComment = Modal.AccessdoorsComment;
                dbModal.ConditionHatchCoversComment = Modal.ConditionHatchCoversComment;
                dbModal.PaintworkHatchCoversComment = Modal.PaintworkHatchCoversComment;
                dbModal.RubbersealsComment = Modal.RubbersealsComment;
                dbModal.SignsofhatchesComment = Modal.SignsofhatchesComment;
                dbModal.SealingtapeComment = Modal.SealingtapeComment;
                dbModal.ConditionofhydraulicsComment = Modal.ConditionofhydraulicsComment;
                dbModal.PortablebulkheadsComment = Modal.PortablebulkheadsComment;
                dbModal.TweendecksComment = Modal.TweendecksComment;
                dbModal.HatchcoamingComment = Modal.HatchcoamingComment;
                dbModal.ConditionCargoCranesComment = Modal.ConditionCargoCranesComment;
                dbModal.GantrycranealarmComment = Modal.GantrycranealarmComment;
                dbModal.GantryconditionComment = Modal.GantryconditionComment;
                dbModal.CargoHoldsComment = Modal.CargoHoldsComment;
                dbModal.CleanlinessComment5R = Modal.CleanlinessComment5R;
                dbModal.PaintworkComment5R = Modal.PaintworkComment5R;
                dbModal.TriphazardsComment = Modal.TriphazardsComment;
                dbModal.WindlassComment = Modal.WindlassComment;
                dbModal.CablesComment = Modal.CablesComment;
                dbModal.WinchesComment5R = Modal.WinchesComment;
                dbModal.FairleadsComment5R = Modal.FairleadsComment;
                dbModal.MooringComment = Modal.MooringComment;
                dbModal.HatchToforecastlespaceComment = Modal.HatchToforecastlespaceComment;
                dbModal.VentilatorsComment5R = Modal.VentilatorsComment5R;
                dbModal.BellComment = Modal.BellComment;
                dbModal.ForemastComment = Modal.ForemastComment;
                dbModal.FireComment = Modal.FireComment;
                dbModal.RailsComment = Modal.RailsComment;
                dbModal.AntislippaintComment5R = Modal.AntislippaintComment5R;
                dbModal.ForecastleComment = Modal.ForecastleComment;
                dbModal.CleanlinessComment5S = Modal.CleanlinessComment5S;
                dbModal.PaintworkComment5S = Modal.PaintworkComment5S;
                dbModal.ForepeakComment = Modal.ForepeakComment;
                dbModal.ChainlockerComment = Modal.ChainlockerComment;
                dbModal.LightingComment5S = Modal.LightingComment5S;
                dbModal.AccesssafetychainComment = Modal.AccessdoorsComment;
                dbModal.EmergencyfirepumpComment = Modal.EmergencyfirepumpComment;
                dbModal.BowthrusterandroomComment = Modal.BowthrusterandroomComment;
                dbModal.SparemooringlinesComment = Modal.SparemooringlinesComment;
                dbModal.PaintlockerComment = Modal.PaintlockerComment;
                dbModal.ForecastleSpaceComment = Modal.ForecastleSpaceComment;
                dbModal.BoottopComment = Modal.BoottopComment;
                dbModal.TopsidesComment = Modal.TopsidesComment;
                dbModal.AntifoulingComment = Modal.AntifoulingComment;
                dbModal.DraftandplimsollComment = Modal.DraftandplimsollComment;
                dbModal.FoulingComment = Modal.FoulingComment;
                dbModal.MechanicalComment = Modal.MechanicalComment;
                dbModal.HullComment = Modal.HullComment;
                dbModal.SummaryComment = Modal.SummaryComment;

                dbModal.SnapBackZoneComment = Modal.SnapBackZoneComment;
                dbModal.ConditionGantryCranesComment = Modal.ConditionGantryCranesComment;
                dbModal.CylindersLockerComment = Modal.CylindersLockerComment;
                dbModal.MedicalLogBookComment = Modal.MedicalLogBookComment;
                dbModal.DrugsNarcoticsComment = Modal.DrugsNarcoticsComment;
                dbModal.DefibrillatorComment = Modal.DefibrillatorComment;
                dbModal.RPWaterHandbook = Modal.RPWaterHandbook;
                dbModal.BioRPWH = Modal.BioRPWH;
                dbModal.PRE = Modal.PRE;
                dbModal.NoiseVibrationFile = Modal.NoiseVibrationFile;
                dbModal.BioMPR = Modal.BioMPR;
                dbModal.AsbestosPlan = Modal.AsbestosPlan;
                dbModal.ShipPublicAddrComment = Modal.ShipPublicAddrComment;
                dbModal.BridgewindowswiperssprayComment = Modal.BridgewindowswiperssprayComment;
                dbModal.BridgewindowswipersComment = Modal.BridgewindowswipersComment;
                dbModal.DaylightSignalsComment = Modal.DaylightSignalsComment;
                dbModal.LiferaftDavitComment = Modal.LiferaftDavitComment;
                dbModal.SnapBackZone5NComment = Modal.SnapBackZone5NComment;
                dbModal.ADPPublicationsComment = Modal.ADPPublicationsComment;

                //RDBJ 10/20/2021
                dbModal.IsGeneralSectionComplete = Modal.IsGeneralSectionComplete;
                dbModal.IsManningSectionComplete = Modal.IsManningSectionComplete;
                dbModal.IsShipCertificationSectionComplete = Modal.IsShipCertificationSectionComplete;
                dbModal.IsRecordKeepingSectionComplete = Modal.IsRecordKeepingSectionComplete;
                dbModal.IsSafetyEquipmentSectionComplete = Modal.IsSafetyEquipmentSectionComplete;
                dbModal.IsSecuritySectionComplete = Modal.IsSecuritySectionComplete;
                dbModal.IsBridgeSectionComplete = Modal.IsBridgeSectionComplete;
                dbModal.IsMedicalSectionComplete = Modal.IsMedicalSectionComplete;
                dbModal.IsGalleySectionComplete = Modal.IsGalleySectionComplete;
                dbModal.IsEngineRoomSectionComplete = Modal.IsEngineRoomSectionComplete;
                dbModal.IsSuperstructureSectionComplete = Modal.IsSuperstructureSectionComplete;
                dbModal.IsDeckSectionComplete = Modal.IsDeckSectionComplete;
                dbModal.IsHoldsAndCoverSectionComplete = Modal.IsHoldsAndCoverSectionComplete;
                dbModal.IsForeCastleSectionComplete = Modal.IsForeCastleSectionComplete;
                dbModal.IsHullSectionComplete = Modal.IsHullSectionComplete;
                dbModal.IsSummarySectionComplete = Modal.IsSummarySectionComplete;
                dbModal.IsDeficienciesSectionComplete = Modal.IsDeficienciesSectionComplete;
                dbModal.IsPhotographsSectionComplete = Modal.IsPhotographsSectionComplete;
                //End RDBJ 10/20/2021

                dbModal.IsSynced = Modal.IsSynced;
                dbModal.CreatedDate = Modal.CreatedDate;
                dbModal.UpdatedDate = Modal.UpdatedDate;
                dbModal.SavedAsDraft = Modal.SavedAsDraft;
                dbModal.GIRDeficiencies = Modal.GIRDeficiencies.Select(y => new GIRDeficiencies()
                {
                    No = y.No,
                    Section = y.Section,
                    Deficiency = y.Deficiency,
                    DateClosed = y.DateClosed,
                    DateRaised = y.DateRaised,
                    DeficienciesID = y.DeficienciesID,
                    GIRDeficienciesFile = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == y.DeficienciesID).Select(x => new Modals.GIRDeficienciesFile()
                    {
                        FileName = x.FileName,
                        StorePath = x.StorePath,
                        DeficienciesID = x.DeficienciesID,
                        GIRDeficienciesFileID = x.GIRDeficienciesFileID
                    }).ToList()
                }).ToList();
                dbModal.GIRPhotographs = modalGIRPhotographs.Select(y => new GIRPhotographs() // RDBJ 12/01/2021 replace with modalGIRPhotographs
                {
                    GIRFormID = y.GIRFormID,
                    ImagePath = y.ImagePath,
                    PhotographsID = y.PhotographsID,
                    ImageCaption = y.ImageCaption == null ? string.Empty : y.ImageCaption, // RDBJ 12/01/2021 Set/handle Null value
                    FileName = !string.IsNullOrEmpty(y.FileName) && y.FileName.IndexOf('_') > 0 ? y.FileName.Substring(y.FileName.IndexOf('_') + 1) : y.FileName,
                }).ToList();
                dbModal.GIRSafeManningRequirements = modalSafeManning.Select(y => new GlRSafeManningRequirements()
                {
                    GIRFormID = y.GIRFormID,
                    UniqueFormID = y.UniqueFormID,
                    Rank = y.Rank,
                    OnBoard = y.OnBoard,
                    RequiredbySMD = y.RequiredbySMD,
                    SafeManningRequirementsID = y.SafeManningRequirementsID

                }).ToList();
                dbModal.GIRCrewDocuments = modalCrewDocuments.Select(y => new GlRCrewDocuments()
                {
                    GIRFormID = y.GIRFormID,
                    UniqueFormID = y.UniqueFormID,
                    CrewmemberName = y.CrewmemberName,
                    CertificationDetail = y.CertificationDetail,
                    CrewDocumentsID = y.CrewDocumentsID

                }).ToList();
                dbModal.GIRRestandWorkHours = modalRestandWorks.Select(y => new GIRRestandWorkHours()
                {
                    GIRFormID = y.GIRFormID,
                    UniqueFormID = y.UniqueFormID,
                    CrewmemberName = y.CrewmemberName,
                    RestAndWorkDetail = y.RestAndWorkDetail,
                    RestandWorkHoursID = y.RestandWorkHoursID

                }).ToList();
            }
            return dbModal;
        }
        public Modals.GeneralInspectionReport GIRGetGeneralDescription(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.GeneralInspectionReport dbModal = new Modals.GeneralInspectionReport();
            var Modal = dbContext.CSShips.Where(x => x.Code == shipCode).FirstOrDefault();
            if (Modal != null)
            {
                //dbModal.ShipId = Modal.ShipId;
                //dbModal.ShipClassId = Modal.ShipClassId;
                //dbModal.Name = Modal.Name;
                //dbModal.Code = Modal.Code;
                //dbModal.BuildCountryId = Modal.BuildCountryId;
                //dbModal.BuildYear = Modal.BuildYear;
                //dbModal.ClassificationSocietyId = Modal.ClassificationSocietyId;
                //dbModal.FlagStateId = Modal.FlagStateId;
                //dbModal.IMONumber = Modal.IMONumber;
                //dbModal.CallSign = Modal.CallSign;
                dbModal.MMSI = Convert.ToString(Modal.MMSI);
                //dbModal.GrossTonnage = Modal.GrossTonnage;
                //dbModal.NetTonnage = Modal.NetTonnage;
                //dbModal.YardNo = Modal.YardNo;
                //dbModal.OfficialNumber = Modal.OfficialNumber;
                //dbModal.Lightweight = Modal.Lightweight;
                //dbModal.SummerDeadweight = Modal.SummerDeadweight;
                //dbModal.Lightweight = Modal.Lightweight;
                dbModal.Beam = Convert.ToString(Modal.Beam.ToString());
                dbModal.LOA = Convert.ToString(Modal.LOA.ToString());
                dbModal.LBP = Convert.ToString(Modal.LBP.ToString());
                //dbModal.SummerDraft = Modal.SummerDraft;
                //dbModal.BHP = Modal.BHP;
                //dbModal.BowThruster = Modal.BowThruster;
                //dbModal.Agent = Modal.Agent;
                //dbModal.Ports = Modal.Ports;
                //dbModal.TechnicalManagerNotes = Modal.TechnicalManagerNotes;
            }
            return dbModal;
        }
        public Modals.GeneralInspectionReport GIRDeficienciesView(int id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.GeneralInspectionReport dbModal = new Modals.GeneralInspectionReport();
            var Modal = dbContext.GeneralInspectionReports.Where(x => x.GIRFormID == id).FirstOrDefault();
            dbModal.GIRDeficiencies = Modal.GIRDeficiencies.Select(y => new GIRDeficiencies()
            {
                No = y.No,
                Section = y.Section,
                ItemNo = y.ItemNo,
                Deficiency = y.Deficiency,
                DateClosed = y.DateClosed,
                DateRaised = y.DateRaised,
                DeficienciesID = y.DeficienciesID,
                GIRDeficienciesFile = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == y.DeficienciesID).Select(x => new Modals.GIRDeficienciesFile()
                {
                    FileName = x.FileName,
                    StorePath = x.StorePath,
                    DeficienciesID = x.DeficienciesID,
                    GIRDeficienciesFileID = x.GIRDeficienciesFileID
                }).ToList()
            }).ToList();
            return dbModal;
        }
        public string GIRAutoSave(Modals.GeneralInspectionReport Modal, bool IsSave = false)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.GeneralInspectionReport dbModal = null;

                if (Modal != null && Modal.UniqueFormID != null) //RDBJ 09/17/2021 changed Modal.UniqueFormID !=  string.empty
                {
                    if (Modal.UniqueFormID != null)
                        dbModal = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == Modal.UniqueFormID).FirstOrDefault();
                }

                if (dbModal == null)
                    dbModal = new Entity.GeneralInspectionReport();

                SetGIRFormData(ref dbModal, Modal);
                dbModal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbModal.isDelete = 0; // RDBJ 01/05/2022

                // JSL 12/31/2022
                if (dbModal.ShipID == null || dbModal.ShipID == 0)
                {
                    var dbships = dbContext.CSShips.Where(x => x.Code == dbModal.Ship).FirstOrDefault();
                    if (dbships != null)
                    {
                        dbModal.ShipID = dbships.ShipId;
                        dbModal.ShipName = dbships.Name;
                    }
                }
                // End JSL 12/31/2022

                if (dbModal != null && dbModal.UniqueFormID != null) //RDBJ 09/17/2021 changed dbModal.UniqueFormID !=  string.empty
                {
                    dbModal.IsSynced = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbModal.IsSynced = false;
                    dbModal.UniqueFormID = Modal.UniqueFormID;
                    dbModal.FormVersion = Modal.FormVersion;
                    dbModal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.GeneralInspectionReports.Add(dbModal);
                    dbContext.SaveChanges();
                }

                if (Modal.Manning_SafeMiningChanged == true)
                {
                    GIRSafeManningRequirements_Save(dbModal.UniqueFormID, Modal.GIRSafeManningRequirements);
                }
                if (Modal.Manning_CrewDocsChanged == true)
                {
                    GIRCrewDocuments_Save(dbModal.UniqueFormID, Modal.GIRCrewDocuments);
                }
                if (Modal.Manning_RestAndWorkChanged == true)
                {
                    GIRRestandWorkHours_Save(dbModal.UniqueFormID, Modal.GIRRestandWorkHours);
                }
                if (Modal.Manning_DeficienciesChanged == true)
                {
                    GIRDeficiencies_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRDeficiencies);
                }
                if (Modal.Manning_PhotosChanged == true)
                {
                    GIRPhotos_Save(dbModal.UniqueFormID, Modal.GIRPhotographs);
                }
                return Convert.ToString(dbModal.UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Auto Save GIR : " + ex.Message);
                return string.Empty;
            }
        }
        public void SetGIRFormData(ref Entity.GeneralInspectionReport dbModal, Modals.GeneralInspectionReport Modal)
        {
            dbModal.FormVersion = Modal.FormVersion;
            dbModal.ShipID = Modal.ShipID;
            dbModal.ShipName = Modal.ShipName;
            dbModal.Ship = Modal.Ship;
            dbModal.Port = Modal.Port;
            dbModal.Inspector = Modal.Inspector;
            dbModal.Date = Modal.Date;
            dbModal.GeneralPreamble = Modal.GeneralPreamble;
            dbModal.Classsociety = Modal.Classsociety;
            dbModal.YearofBuild = Modal.YearofBuild;
            dbModal.Flag = Modal.Flag;
            dbModal.Classofvessel = Modal.Classofvessel;
            dbModal.Portofregistry = Modal.Portofregistry;
            dbModal.MMSI = Modal.MMSI;
            dbModal.IMOnumber = Modal.IMOnumber;
            dbModal.Callsign = Modal.Callsign;
            dbModal.SummerDWT = Modal.SummerDWT;
            dbModal.Grosstonnage = Modal.Grosstonnage;
            dbModal.Lightweight = Modal.Lightweight;
            dbModal.Nettonnage = Modal.Nettonnage;
            dbModal.Beam = Modal.Beam;
            dbModal.LOA = Modal.LOA;
            dbModal.Summerdraft = Modal.Summerdraft;
            dbModal.LBP = Modal.LBP;
            dbModal.Bowthruster = Modal.Bowthruster;
            dbModal.BHP = Modal.BHP;
            dbModal.Noofholds = Modal.Noofholds;
            dbModal.Nomoveablebulkheads = Modal.Nomoveablebulkheads;
            dbModal.Containers = Modal.Containers;
            dbModal.Cargocapacity = Modal.Cargocapacity;
            dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
            dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
            dbModal.Lastvoyageandcargo = Modal.Lastvoyageandcargo;
            dbModal.CurrentPlannedvoyageandcargo = Modal.CurrentPlannedvoyageandcargo;
            dbModal.ShipboardWorkingArrangements = Modal.ShipboardWorkingArrangements;
            dbModal.CertificationIndex = Modal.CertificationIndex;
            dbModal.IsPubsAndDocsSectionComplete = Modal.IsPubsAndDocsSectionComplete == null ? false : true; //RDBJ 10/20/2021
            dbModal.CarriedOutByTheDOOW = Modal.CarriedOutByTheDOOW;
            dbModal.IsRegs4shipsDVD = Modal.IsRegs4shipsDVD == null ? false : true; //RDBJ 10/20/2021
            dbModal.Regs4shipsDVD = Modal.Regs4shipsDVD;
            dbModal.IsSOPEPPoints = Modal.IsSOPEPPoints == null ? false : true; //RDBJ 10/20/2021
            dbModal.SOPEPPoints = Modal.SOPEPPoints;
            dbModal.IsBWMP = Modal.IsBWMP == null ? false : true; //RDBJ 10/20/2021
            dbModal.BWMP = Modal.BWMP;
            dbModal.IsBWMPSupplement = Modal.IsBWMPSupplement == null ? false : true; //RDBJ 10/20/2021
            dbModal.BWMPSupplement = Modal.BWMPSupplement;
            dbModal.IsIntactStabilityManual = Modal.IsIntactStabilityManual == null ? false : true; //RDBJ 10/20/2021
            dbModal.IntactStabilityManual = Modal.IntactStabilityManual;
            dbModal.IsStabilityComputer = Modal.IsStabilityComputer == null ? false : true; //RDBJ 10/20/2021
            dbModal.StabilityComputer = Modal.StabilityComputer;
            dbModal.IsDateOfLast = Modal.IsDateOfLast == null ? false : true; //RDBJ 10/20/2021
            dbModal.DateOfLast = Modal.DateOfLast;
            dbModal.IsCargoSecuring = Modal.IsCargoSecuring == null ? false : true; //RDBJ 10/20/2021
            dbModal.CargoSecuring = Modal.CargoSecuring;
            dbModal.BulkCargo = Modal.BulkCargo;
            dbModal.IBulkCargo = Modal.IsBulkCargo == null ? false : true; //RDBJ 10/20/2021
            dbModal.IsSMSManual = Modal.IsSMSManual == null ? false : true; //RDBJ 10/20/2021
            dbModal.SMSManual = Modal.SMSManual;
            dbModal.IsRegisterOf = Modal.IsRegisterOf == null ? false : true; //RDBJ 10/20/2021
            dbModal.RegisterOf = Modal.RegisterOf;
            dbModal.IsFleetStandingOrder = Modal.IsFleetStandingOrder == null ? false : true; //RDBJ 10/20/2021
            dbModal.FleetStandingOrder = Modal.FleetStandingOrder;
            dbModal.IsFleetMemoranda = Modal.IsFleetMemoranda == null ? false : true; //RDBJ 10/20/2021
            dbModal.FleetMemoranda = Modal.FleetMemoranda;
            dbModal.IsShipsPlans = Modal.IsShipsPlans == null ? false : true; //RDBJ 10/20/2021
            dbModal.ShipsPlans = Modal.ShipsPlans;
            dbModal.IsCollective = Modal.IsCollective == null ? false : true; //RDBJ 10/20/2021
            dbModal.Collective = Modal.Collective;
            dbModal.IsDraftAndFreeboardNotice = Modal.IsDraftAndFreeboardNotice == null ? false : true; //RDBJ 10/20/2021
            dbModal.DraftAndFreeboardNotice = Modal.DraftAndFreeboardNotice;
            dbModal.IsPCSOPEP = Modal.IsPCSOPEP == null ? false : true; //RDBJ 10/20/2021
            dbModal.PCOPEP = Modal.PCSOPEP;
            dbModal.INTVRP = Modal.IsNTVRP == null ? false : true; //RDBJ 10/20/2021
            dbModal.NTVRP = Modal.NTVRP;
            dbModal.IVGP = Modal.IsVGP == null ? false : true; //RDBJ 10/20/2021
            dbModal.VGP = Modal.VGP;
            dbModal.PubsComments = Modal.PubsComments;
            dbModal.OfficialLogbookA = Modal.OfficialLogbookA;
            dbModal.OfficialLogbookB = Modal.OfficialLogbookB;
            dbModal.OfficialLogbookC = Modal.OfficialLogbookC;
            dbModal.OfficialLogbookD = Modal.OfficialLogbookD;
            dbModal.OfficialLogbookE = Modal.OfficialLogbookE;
            dbModal.DeckLogbook = Modal.DeckLogbook;
            dbModal.Listofcrew = Modal.Listofcrew;
            dbModal.LastHose = Modal.LastHose;
            dbModal.PassagePlanning = Modal.PassagePlanning;
            dbModal.LoadingComputer = Modal.LoadingComputer;
            dbModal.EngineLogbook = Modal.EngineLogbook;
            dbModal.OilRecordBook = Modal.OilRecordBook;
            dbModal.RiskAssessments = Modal.RiskAssessments;
            dbModal.GMDSSLogbook = Modal.GMDSSLogbook;
            dbModal.DeckLogbook5D = Modal.DeckLogbook5D;
            dbModal.GarbageRecordBook = Modal.GarbageRecordBook;
            dbModal.BallastWaterRecordBook = Modal.BallastWaterRecordBook;
            dbModal.CargoRecordBook = Modal.CargoRecordBook;
            dbModal.EmissionsControlManual = Modal.EmissionsControlManual;
            dbModal.LGR = Modal.LGR;
            dbModal.PEER = Modal.PEER;
            dbModal.RecordKeepingComments = Modal.RecordKeepingComments;
            dbModal.LastPortStateControl = Modal.LastPortStateControl;

            dbModal.LiferaftsComment = Modal.LiferaftsComment;
            dbModal.releasesComment = Modal.releasesComment;
            dbModal.LifeboatComment = Modal.LifeboatComment;
            dbModal.LifeboatdavitComment = Modal.LifeboatdavitComment;
            dbModal.LifeboatequipmentComment = Modal.LifeboatequipmentComment;
            dbModal.RescueboatComment = Modal.RescueboatComment;
            dbModal.RescueboatequipmentComment = Modal.RescueboatequipmentComment;
            dbModal.RescueboatoutboardmotorComment = Modal.RescueboatoutboardmotorComment;
            dbModal.RescueboatdavitComment = Modal.RescueboatdavitComment;
            dbModal.DeckComment = Modal.DeckComment;
            dbModal.PyrotechnicsComment = Modal.PyrotechnicsComment;
            dbModal.EPIRBComment = Modal.EPIRBComment;
            dbModal.SARTsComment = Modal.SARTsComment;
            dbModal.GMDSSComment = Modal.GMDSSComment;
            dbModal.ManoverboardComment = Modal.ManoverboardComment;
            dbModal.LinethrowingapparatusComment = Modal.LinethrowingapparatusComment;
            dbModal.FireextinguishersComment = Modal.FireextinguishersComment;
            dbModal.EmergencygeneratorComment = Modal.EmergencygeneratorComment;
            dbModal.CO2roomComment = Modal.CO2roomComment;
            dbModal.SurvivalComment = Modal.SurvivalComment;
            dbModal.LifejacketComment = Modal.LifejacketComment;
            dbModal.FiremansComment = Modal.FiremansComment;
            dbModal.LifebuoysComment = Modal.LifebuoysComment;
            dbModal.FireboxesComment = Modal.FireboxesComment;
            dbModal.EmergencybellsComment = Modal.EmergencybellsComment;
            dbModal.EmergencylightingComment = Modal.EmergencylightingComment;
            dbModal.FireplanComment = Modal.FireplanComment;
            dbModal.DamageComment = Modal.DamageComment;
            dbModal.EmergencyplansComment = Modal.EmergencyplansComment;
            dbModal.MusterlistComment = Modal.MusterlistComment;
            dbModal.SafetysignsComment = Modal.SafetysignsComment;
            dbModal.EmergencysteeringComment = Modal.EmergencysteeringComment;
            dbModal.StatutoryemergencydrillsComment = Modal.StatutoryemergencydrillsComment;
            dbModal.EEBDComment = Modal.EEBDComment;
            dbModal.OxygenComment = Modal.OxygenComment;
            dbModal.MultigasdetectorComment = Modal.MultigasdetectorComment;
            dbModal.GasdetectorComment = Modal.GasdetectorComment;
            dbModal.SufficientquantityComment = Modal.SufficientquantityComment;
            dbModal.BASetsComment = Modal.BASetsComment;
            dbModal.SafetyComment = Modal.SafetyComment;

            dbModal.GangwayComment = Modal.GangwayComment;
            dbModal.RestrictedComment = Modal.RestrictedComment;
            dbModal.OutsideComment = Modal.OutsideComment;
            dbModal.EntrancedoorsComment = Modal.EntrancedoorsComment;
            dbModal.AccommodationComment = Modal.AccommodationComment;
            dbModal.GMDSSComment5G = Modal.GMDSSComment5G;
            dbModal.VariousComment = Modal.VariousComment;
            dbModal.SSOComment = Modal.SSOComment;
            dbModal.SecuritylogbookComment = Modal.SecuritylogbookComment;
            dbModal.Listoflast10portsComment = Modal.Listoflast10portsComment;
            dbModal.PFSOComment = Modal.PFSOComment;
            dbModal.SecuritylevelComment = Modal.SecuritylevelComment;
            dbModal.DrillsandtrainingComment = Modal.DrillsandtrainingComment;
            dbModal.DOSComment = Modal.DOSComment;
            dbModal.SSASComment = Modal.SSASComment;
            dbModal.VisitorslogbookComment = Modal.VisitorslogbookComment;
            dbModal.KeyregisterComment = Modal.KeyregisterComment;
            dbModal.ShipSecurityComment = Modal.ShipSecurityComment;
            dbModal.SecurityComment = Modal.SecurityComment;

            dbModal.NauticalchartsComment = Modal.NauticalchartsComment;
            dbModal.NoticetomarinersComment = Modal.NoticetomarinersComment;
            dbModal.ListofradiosignalsComment = Modal.ListofradiosignalsComment;
            dbModal.ListoflightsComment = Modal.ListoflightsComment;
            dbModal.SailingdirectionsComment = Modal.SailingdirectionsComment;
            dbModal.TidetablesComment = Modal.TidetablesComment;
            dbModal.NavtexandprinterComment = Modal.NavtexandprinterComment;
            dbModal.RadarsComment = Modal.RadarsComment;
            dbModal.GPSComment = Modal.GPSComment;
            dbModal.AISComment = Modal.AISComment;
            dbModal.VDRComment = Modal.VDRComment;
            dbModal.ECDISComment = Modal.ECDISComment;
            dbModal.EchosounderComment = Modal.EchosounderComment;
            dbModal.ADPbackuplaptopComment = Modal.ADPbackuplaptopComment;
            dbModal.ColourprinterComment = Modal.ColourprinterComment;
            dbModal.VHFDSCtransceiverComment = Modal.VHFDSCtransceiverComment;
            dbModal.radioinstallationComment = Modal.radioinstallationComment;
            dbModal.InmarsatCComment = Modal.InmarsatCComment;
            dbModal.MagneticcompassComment = Modal.MagneticcompassComment;
            dbModal.SparecompassbowlComment = Modal.SparecompassbowlComment;
            dbModal.CompassobservationbookComment = Modal.CompassobservationbookComment;
            dbModal.GyrocompassComment = Modal.GyrocompassComment;
            dbModal.RudderindicatorComment = Modal.RudderindicatorComment;
            dbModal.SpeedlogComment = Modal.SpeedlogComment;
            dbModal.NavigationComment = Modal.NavigationComment;
            dbModal.SignalflagsComment = Modal.SignalflagsComment;
            dbModal.RPMComment = Modal.RPMComment;
            dbModal.BasicmanoeuvringdataComment = Modal.BasicmanoeuvringdataComment;
            dbModal.MasterstandingordersComment = Modal.MasterstandingordersComment;
            dbModal.MasternightordersbookComment = Modal.MasternightordersbookComment;
            dbModal.SextantComment = Modal.SextantComment;
            dbModal.AzimuthmirrorComment = Modal.AzimuthmirrorComment;
            dbModal.BridgepostersComment = Modal.BridgepostersComment;
            dbModal.ReviewofplannedComment = Modal.ReviewofplannedComment;
            dbModal.BridgebellbookComment = Modal.BridgebellbookComment;
            dbModal.BridgenavigationalComment = Modal.BridgenavigationalComment;
            dbModal.SecurityEquipmentComment = Modal.SecurityEquipmentComment;
            dbModal.NavigationPost = Modal.NavigationPost;

            dbModal.GeneralComment = Modal.GeneralComment;
            dbModal.MedicinestorageComment = Modal.MedicinestorageComment;
            dbModal.MedicinechestcertificateComment = Modal.MedicinechestcertificateComment;
            dbModal.InventoryStoresComment = Modal.InventoryStoresComment;
            dbModal.OxygencylindersComment = Modal.OxygencylindersComment;
            dbModal.StretcherComment = Modal.StretcherComment;
            dbModal.SalivaComment = Modal.SalivaComment;
            dbModal.AlcoholComment = Modal.AlcoholComment;
            dbModal.HospitalComment = Modal.HospitalComment;

            dbModal.GeneralGalleyComment = Modal.GeneralGalleyComment;
            dbModal.HygieneComment = Modal.HygieneComment;
            dbModal.FoodstorageComment = Modal.FoodstorageComment;
            dbModal.FoodlabellingComment = Modal.FoodlabellingComment;
            dbModal.GalleyriskassessmentComment = Modal.GalleyriskassessmentComment;
            dbModal.FridgetemperatureComment = Modal.FridgetemperatureComment;
            dbModal.FoodandProvisionsComment = Modal.FoodandProvisionsComment;
            dbModal.GalleyComment = Modal.GalleyComment;

            dbModal.ConditionComment = Modal.ConditionComment;
            dbModal.PaintworkComment = Modal.PaintworkComment;
            dbModal.LightingComment = Modal.LightingComment;
            dbModal.PlatesComment = Modal.PlatesComment;
            dbModal.BilgesComment = Modal.BilgesComment;
            dbModal.PipelinesandvalvesComment = Modal.PipelinesandvalvesComment;
            dbModal.LeakageComment = Modal.LeakageComment;
            dbModal.EquipmentComment = Modal.EquipmentComment;
            dbModal.OilywaterseparatorComment = Modal.OilywaterseparatorComment;
            dbModal.FueloiltransferplanComment = Modal.FueloiltransferplanComment;
            dbModal.SteeringgearComment = Modal.SteeringgearComment;
            dbModal.WorkshopandequipmentComment = Modal.WorkshopandequipmentComment;
            dbModal.SoundingpipesComment = Modal.SoundingpipesComment;
            dbModal.EnginecontrolComment = Modal.EnginecontrolComment;
            dbModal.ChiefEngineernightordersbookComment = Modal.ChiefEngineernightordersbookComment;
            dbModal.ChiefEngineerstandingordersComment = Modal.ChiefEngineerstandingordersComment;
            dbModal.PreUMSComment = Modal.PreUMSComment;
            dbModal.EnginebellbookComment = Modal.EnginebellbookComment;
            dbModal.LockoutComment = Modal.LockoutComment;
            dbModal.EngineRoomComment = Modal.EngineRoomComment;

            dbModal.CleanlinessandhygieneComment = Modal.CleanlinessandhygieneComment;
            dbModal.ConditionComment5M = Modal.ConditionComment5M;
            dbModal.PaintworkComment5M = Modal.PaintworkComment5M;
            dbModal.SignalmastandstaysComment = Modal.SignalmastandstaysComment;
            dbModal.MonkeyislandComment = Modal.MonkeyislandComment;
            dbModal.FireDampersComment = Modal.FireDampersComment;
            dbModal.RailsBulwarksComment = Modal.RailsBulwarksComment;
            dbModal.WatertightdoorsComment = Modal.WatertightdoorsComment;
            dbModal.VentilatorsComment = Modal.VentilatorsComment;
            dbModal.WinchesComment = Modal.WinchesComment;
            dbModal.FairleadsComment = Modal.FairleadsComment;
            dbModal.MooringLinesComment = Modal.MooringLinesComment;
            dbModal.EmergencyShutOffsComment = Modal.EmergencyShutOffsComment;
            dbModal.RadioaerialsComment = Modal.RadioaerialsComment;
            dbModal.SOPEPlockerComment = Modal.SOPEPlockerComment;
            dbModal.ChemicallockerComment = Modal.ChemicallockerComment;
            dbModal.AntislippaintComment = Modal.AntislippaintComment;
            dbModal.SuperstructureComment = Modal.SuperstructureComment;
            dbModal.CabinsComment = Modal.CabinsComment;
            dbModal.OfficesComment = Modal.OfficesComment;
            dbModal.MessroomsComment = Modal.MessroomsComment;
            dbModal.ToiletsComment = Modal.ToiletsComment;
            dbModal.LaundryroomComment = Modal.LaundryroomComment;
            dbModal.ChangingroomComment = Modal.ChangingroomComment;
            dbModal.OtherComment = Modal.OtherComment;
            dbModal.ConditionComment5N = Modal.ConditionComment5N;
            dbModal.SelfclosingfiredoorsComment = Modal.SelfclosingfiredoorsComment;
            dbModal.StairwellsComment = Modal.StairwellsComment;
            dbModal.SuperstructureInternalComment = Modal.SuperstructureInternalComment;

            dbModal.PortablegangwayComment = Modal.PortablegangwayComment;
            dbModal.SafetynetComment = Modal.SafetynetComment;
            dbModal.AccommodationLadderComment = Modal.AccommodationLadderComment;
            dbModal.SafeaccessprovidedComment = Modal.SafeaccessprovidedComment;
            dbModal.PilotladdersComment = Modal.PilotladdersComment;
            dbModal.BoardingEquipmentComment = Modal.BoardingEquipmentComment;
            dbModal.CleanlinessComment = Modal.CleanlinessComment;
            dbModal.PaintworkComment5P = Modal.PaintworkComment5P;
            dbModal.ShipsiderailsComment = Modal.ShipsiderailsComment;
            dbModal.WeathertightdoorsComment = Modal.WeathertightdoorsComment;
            dbModal.FirehydrantsComment = Modal.FirehydrantsComment;
            dbModal.VentilatorsComment5P = Modal.VentilatorsComment5P;
            dbModal.ManholecoversComment = Modal.ManholecoversComment;
            dbModal.MainDeckAreaComment = Modal.MainDeckAreaComment;

            dbModal.ConditionComment5Q = Modal.ConditionComment5Q;
            dbModal.PaintworkComment5Q = Modal.PaintworkComment5Q;
            dbModal.MechanicaldamageComment = Modal.MechanicaldamageComment;
            dbModal.AccessladdersComment = Modal.AccessladdersComment;
            dbModal.ManholecoversComment5Q = Modal.ManholecoversComment5Q;
            dbModal.HoldbilgeComment = Modal.HoldbilgeComment;
            dbModal.AccessdoorsComment = Modal.AccessdoorsComment;
            dbModal.ConditionHatchCoversComment = Modal.ConditionHatchCoversComment;
            dbModal.PaintworkHatchCoversComment = Modal.PaintworkHatchCoversComment;
            dbModal.RubbersealsComment = Modal.RubbersealsComment;
            dbModal.SignsofhatchesComment = Modal.SignsofhatchesComment;
            dbModal.SealingtapeComment = Modal.SealingtapeComment;
            dbModal.ConditionofhydraulicsComment = Modal.ConditionofhydraulicsComment;
            dbModal.PortablebulkheadsComment = Modal.PortablebulkheadsComment;
            dbModal.TweendecksComment = Modal.TweendecksComment;
            dbModal.HatchcoamingComment = Modal.HatchcoamingComment;
            dbModal.ConditionCargoCranesComment = Modal.ConditionCargoCranesComment;
            dbModal.GantrycranealarmComment = Modal.GantrycranealarmComment;
            dbModal.GantryconditionComment = Modal.GantryconditionComment;
            dbModal.CargoHoldsComment = Modal.CargoHoldsComment;

            dbModal.CleanlinessComment5R = Modal.CleanlinessComment5R;
            dbModal.PaintworkComment5R = Modal.PaintworkComment5R;
            dbModal.TriphazardsComment = Modal.TriphazardsComment;
            dbModal.WindlassComment = Modal.WindlassComment;
            dbModal.CablesComment = Modal.CablesComment;
            dbModal.WinchesComment5R = Modal.WinchesComment5R;
            dbModal.FairleadsComment5R = Modal.FairleadsComment5R;
            dbModal.MooringComment = Modal.MooringComment;
            dbModal.HatchToforecastlespaceComment = Modal.HatchToforecastlespaceComment;
            dbModal.VentilatorsComment5R = Modal.VentilatorsComment5R;
            dbModal.BellComment = Modal.BellComment;
            dbModal.ForemastComment = Modal.ForemastComment;
            dbModal.FireComment = Modal.FireComment;
            dbModal.RailsComment = Modal.RailsComment;
            dbModal.AntislippaintComment5R = Modal.AntislippaintComment5R;
            dbModal.ForecastleComment = Modal.ForecastleComment;
            dbModal.CleanlinessComment5S = Modal.CleanlinessComment5S;
            dbModal.PaintworkComment5S = Modal.PaintworkComment5S;
            dbModal.ForepeakComment = Modal.ForepeakComment;
            dbModal.ChainlockerComment = Modal.ChainlockerComment;
            dbModal.LightingComment5S = Modal.LightingComment5S;
            dbModal.AccesssafetychainComment = Modal.AccesssafetychainComment;
            dbModal.EmergencyfirepumpComment = Modal.EmergencyfirepumpComment;
            dbModal.BowthrusterandroomComment = Modal.BowthrusterandroomComment;
            dbModal.SparemooringlinesComment = Modal.SparemooringlinesComment;
            dbModal.PaintlockerComment = Modal.PaintlockerComment;
            dbModal.ForecastleSpaceComment = Modal.ForecastleSpaceComment;

            dbModal.BoottopComment = Modal.BoottopComment;
            dbModal.TopsidesComment = Modal.TopsidesComment;
            dbModal.AntifoulingComment = Modal.AntifoulingComment;
            dbModal.DraftandplimsollComment = Modal.DraftandplimsollComment;
            dbModal.FoulingComment = Modal.FoulingComment;
            dbModal.MechanicalComment = Modal.MechanicalComment;
            dbModal.HullComment = Modal.HullComment;
            dbModal.SummaryComment = Modal.SummaryComment;
            dbModal.IsSynced = Modal.IsSynced;
            dbModal.SavedAsDraft = Modal.SavedAsDraft;

            dbModal.SnapBackZoneComment = Modal.SnapBackZoneComment;
            dbModal.ConditionGantryCranesComment = Modal.ConditionGantryCranesComment;
            dbModal.CylindersLockerComment = Modal.CylindersLockerComment;
            dbModal.MedicalLogBookComment = Modal.MedicalLogBookComment;
            dbModal.DrugsNarcoticsComment = Modal.DrugsNarcoticsComment;
            dbModal.DefibrillatorComment = Modal.DefibrillatorComment;
            dbModal.RPWaterHandbook = Modal.RPWaterHandbook;
            dbModal.BioRPWH = Modal.BioRPWH;
            dbModal.PRE = Modal.PRE;
            dbModal.NoiseVibrationFile = Modal.NoiseVibrationFile;
            dbModal.BioMPR = Modal.BioMPR;
            dbModal.AsbestosPlan = Modal.AsbestosPlan;
            dbModal.ShipPublicAddrComment = Modal.ShipPublicAddrComment;
            dbModal.BridgewindowswiperssprayComment = Modal.BridgewindowswiperssprayComment;
            dbModal.BridgewindowswipersComment = Modal.BridgewindowswipersComment;
            dbModal.DaylightSignalsComment = Modal.DaylightSignalsComment;
            dbModal.LiferaftDavitComment = Modal.LiferaftDavitComment;
            dbModal.SnapBackZone5NComment = Modal.SnapBackZone5NComment;
            dbModal.ADPPublicationsComment = Modal.ADPPublicationsComment;

            //RDBJ 10/20/2021
            dbModal.IsGeneralSectionComplete = Modal.IsGeneralSectionComplete == null ? false : true;
            dbModal.IsManningSectionComplete = Modal.IsManningSectionComplete == null ? false : true;
            dbModal.IsShipCertificationSectionComplete = Modal.IsShipCertificationSectionComplete == null ? false : true;
            dbModal.IsRecordKeepingSectionComplete = Modal.IsRecordKeepingSectionComplete == null ? false : true;
            dbModal.IsSafetyEquipmentSectionComplete = Modal.IsSafetyEquipmentSectionComplete == null ? false : true;
            dbModal.IsSecuritySectionComplete = Modal.IsSecuritySectionComplete == null ? false : true;
            dbModal.IsBridgeSectionComplete = Modal.IsBridgeSectionComplete == null ? false : true;
            dbModal.IsMedicalSectionComplete = Modal.IsMedicalSectionComplete == null ? false : true;
            dbModal.IsGalleySectionComplete = Modal.IsGalleySectionComplete == null ? false : true;
            dbModal.IsEngineRoomSectionComplete = Modal.IsEngineRoomSectionComplete == null ? false : true;
            dbModal.IsSuperstructureSectionComplete = Modal.IsSuperstructureSectionComplete == null ? false : true;
            dbModal.IsDeckSectionComplete = Modal.IsDeckSectionComplete == null ? false : true;
            dbModal.IsHoldsAndCoverSectionComplete = Modal.IsHoldsAndCoverSectionComplete == null ? false : true;
            dbModal.IsForeCastleSectionComplete = Modal.IsForeCastleSectionComplete == null ? false : true;
            dbModal.IsHullSectionComplete = Modal.IsHullSectionComplete == null ? false : true;
            dbModal.IsSummarySectionComplete = Modal.IsSummarySectionComplete == null ? false : true;
            dbModal.IsDeficienciesSectionComplete = Modal.IsDeficienciesSectionComplete == null ? false : true;
            dbModal.IsPhotographsSectionComplete = Modal.IsPhotographsSectionComplete == null ? false : true;
            //End RDBJ 10/20/2021
        }
        public void GIRSafeManningRequirements_Save(Guid? UniqueFormID, List<Modals.GlRSafeManningRequirements> GIRSafeManningRequirements)
        {
            try
            {
                if (GIRSafeManningRequirements != null && GIRSafeManningRequirements.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    List<GlRSafeManningRequirement> dbGlRSafeManningRequirements = dbContext.GlRSafeManningRequirements.Where(x => x.UniqueFormID == UniqueFormID).ToList();
                    if (dbGlRSafeManningRequirements != null && dbGlRSafeManningRequirements.Count > 0)
                    {
                        foreach (var item in dbGlRSafeManningRequirements)
                        {
                            dbContext.GlRSafeManningRequirements.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }
                    // Insert Into DB
                    foreach (var item in GIRSafeManningRequirements)
                    {
                        //RDBJ 10/09/2021 Wrapped in if
                        if (!string.IsNullOrEmpty(item.Rank))
                        {
                            Entity.GlRSafeManningRequirement member = new Entity.GlRSafeManningRequirement();
                            member.GIRFormID = 0; // GIRFormID;
                            member.UniqueFormID = UniqueFormID;
                            member.Ship = item.Ship;
                            member.Rank = item.Rank;
                            member.RequiredbySMD = item.RequiredbySMD;
                            member.OnBoard = item.OnBoard;
                            member.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            dbContext.GlRSafeManningRequirements.Add(member);
                        }
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRSafeManningRequirements_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRCrewDocuments_Save(Guid? UniqueFormID, List<Modals.GlRCrewDocuments> GIRCrewDocuments)
        {
            try
            {
                if (GIRCrewDocuments != null && GIRCrewDocuments.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    if (GIRCrewDocuments != null && GIRCrewDocuments.Count > 0)
                    {
                        List<GlRCrewDocument> dbGlRCrewDocuments = dbContext.GlRCrewDocuments.Where(x => x.UniqueFormID == UniqueFormID).ToList();
                        if (dbGlRCrewDocuments != null && dbGlRCrewDocuments.Count > 0)
                        {
                            foreach (var item in dbGlRCrewDocuments)
                            {
                                dbContext.GlRCrewDocuments.Remove(item);
                            }
                            dbContext.SaveChanges();
                        }
                        foreach (var item in GIRCrewDocuments)
                        {
                            //RDBJ 10/09/2021 Wrapped in if
                            if (!string.IsNullOrEmpty(item.CrewmemberName))
                            {
                                Entity.GlRCrewDocument member = new Entity.GlRCrewDocument();
                                member.GIRFormID = 0; //GIRFormID;
                                member.UniqueFormID = UniqueFormID;
                                member.Ship = item.Ship;
                                member.CrewmemberName = item.CrewmemberName;
                                member.CertificationDetail = item.CertificationDetail;
                                member.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                dbContext.GlRCrewDocuments.Add(member);
                            }
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRCrewDocuments_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRRestandWorkHours_Save(Guid? UniqueFormID, List<Modals.GIRRestandWorkHours> GIRRestandWorkHours)
        {
            try
            {
                if (GIRRestandWorkHours != null && GIRRestandWorkHours.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    List<GIRRestandWorkHour> dbGIRRestandWorkHours = dbContext.GIRRestandWorkHours.Where(x => x.UniqueFormID == UniqueFormID).ToList();
                    if (dbGIRRestandWorkHours != null && dbGIRRestandWorkHours.Count > 0)
                    {
                        foreach (var item in dbGIRRestandWorkHours)
                        {
                            dbContext.GIRRestandWorkHours.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }
                    foreach (var item in GIRRestandWorkHours)
                    {
                        //RDBJ 10/09/2021 Wrapped in if
                        if (!string.IsNullOrEmpty(item.CrewmemberName))
                        {
                            Entity.GIRRestandWorkHour member = new Entity.GIRRestandWorkHour();
                            member.GIRFormID = 0; // GIRFormID;
                            member.UniqueFormID = UniqueFormID;
                            member.Ship = item.Ship;
                            member.CrewmemberName = item.CrewmemberName;
                            member.RestAndWorkDetail = item.RestAndWorkDetail;
                            member.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            dbContext.GIRRestandWorkHours.Add(member);
                        }
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRRestandWorkHours_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRDeficiencies_Save(string UniqueFormID, List<Modals.GIRDeficiencies> GIRDeficiencies)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid UFormId = Guid.Parse(UniqueFormID);
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0 && UFormId != Guid.Empty)
                {
                    foreach (var item in GIRDeficiencies)
                    {
                        Entity.GIRDeficiency member = new Entity.GIRDeficiency();
                        member = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UFormId && x.No == item.No).FirstOrDefault();
                        int cnt = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UFormId && x.No == item.No).Select(x => x.DeficienciesID).FirstOrDefault();

                        if (cnt == 0)
                        {
                            member.GIRFormID = 0; //item.GIRFormID; //RDBJ 10/12/2021 set 0
                            member.No = item.No;
                            member.DateRaised = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.Deficiency = item.Deficiency;
                            member.DateClosed = item.DateClosed;
                            member.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.Ship = item.Ship;
                            member.IsClose = false;
                            member.ReportType = "GI";
                            member.ItemNo = item.ItemNo;
                            member.Section = item.Section;
                            member.UniqueFormID = UFormId;
                            member.isDelete = 0;
                            dbContext.GIRDeficiencies.Add(member);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            member.DateRaised = item.DateRaised;
                            member.Deficiency = item.Deficiency;
                            member.DateClosed = item.DateClosed;

                            //RDBJ 10/12/2021 wrapped in if
                            if (item.DateClosed != null)
                                member.IsClose = true;
                            else
                                member.IsClose = false;

                            member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.Section = item.Section;
                            dbContext.SaveChanges();
                        }

                        // GIRDeficienciesFile_Save(item, item.Ship, member.DeficienciesUniqueID);  // JSL 06/04/2022 commented this line
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRDeficiencies_Save : " + ex.Message);
            }
        }
        public void GIRDeficienciesFile_Save(GIRDeficiencies modal, string Ship, Guid? DeficienciesUniqueID) //RDBJ 09/18/2021 int DeficienciesID
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                // JSL 06/04/2022 commented
                /*
                List<Entity.GIRDeficienciesFile> girDefFiles = new List<Entity.GIRDeficienciesFile>();
                girDefFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == DeficienciesUniqueID).ToList(); //RDBJ 09/18/2021

                foreach (var itemFile in girDefFiles)
                {
                    dbContext.GIRDeficienciesFiles.Remove(itemFile);
                    dbContext.SaveChanges();
                }
                */
                // End JSL 06/04/2022 commented

                foreach (var item in modal.GIRDeficienciesFile)
                {
                    // JSL 12/03/2022
                    if (Convert.ToBoolean(item.IsUpload))
                    {
                        Entity.GIRDeficienciesFile file = new Entity.GIRDeficienciesFile();
                        file.FileName = item.FileName;
                        file.StorePath = item.StorePath;
                        file.DeficienciesID = 0;
                        file.DeficienciesUniqueID = DeficienciesUniqueID;
                        file.DeficienciesFileUniqueID = (Guid)item.DeficienciesFileUniqueID;
                        dbContext.GIRDeficienciesFiles.Add(file);
                        dbContext.SaveChanges();
                    }
                    // End JSL 12/03/2022

                    // JSL 12/03/2022 commented
                    /*
                    var split = item.StorePath.Split(',');//.LastOrDefault();
                    string OrignalString = split.LastOrDefault();
                    if (!string.IsNullOrEmpty(OrignalString))
                    {
                        //RDBJ 09/18/2021 Commented below code
                        /*
                        byte[] imageBytes = Convert.FromBase64String(OrignalString);
                        string rootpath = HttpContext.Current.Server.MapPath("~/GIRDeficiency/");

                        string subPath = Ship + "/" + DeficienciesID.ToString() + "/";
                        bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                        string CompleteFolderPath = Path.Combine(rootpath + subPath);
                        if (!Directory.Exists(CompleteFolderPath))
                        {
                            Directory.CreateDirectory(CompleteFolderPath);
                        }
                        var imageName = Ship + "_" + DateTime.Now.ToString("MMdddyyyhhmmss") + "_" + item.FileName;
                        File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);
                        // End RDBJ 09/18/2021 Commented below code
                        /*
                        Entity.GIRDeficienciesFile file = new Entity.GIRDeficienciesFile();
                        file.FileName = item.FileName;
                        file.StorePath = item.StorePath; //"/GIRDeficiency/" + subPath + imageName;
                        file.DeficienciesID = 0; //RDBJ 09/18/2021 DeficienciesID;
                        file.DeficienciesUniqueID = DeficienciesUniqueID; //RDBJ 09/18/2021
                        file.DeficienciesFileUniqueID = (Guid)item.DeficienciesFileUniqueID;  // JSL 06/04/2022

                        // RDBJ 03/12/2022 wrapped in if
                        if (Convert.ToBoolean(item.IsUpload))
                        {
                            dbContext.GIRDeficienciesFiles.Add(file);
                            dbContext.SaveChanges();
                        }
                    }
                    */
                    // End JSL 12/03/2022 commented
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Save GIRDeficiency Image File " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRPhotos_Save(Guid? UniqueFormID, List<Modals.GIRPhotographs> modal)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                List<GIRPhotograph> GIRPhotographs = dbContext.GIRPhotographs.Where(x => x.UniqueFormID == UniqueFormID).ToList();
                if (GIRPhotographs != null && GIRPhotographs.Count > 0)
                {
                    foreach (var item in GIRPhotographs)
                    {
                        string rootpath = HttpContext.Current.Server.MapPath("~/");
                        rootpath = rootpath + item.FileName;
                        if (File.Exists(rootpath))
                        {
                            File.Delete(rootpath);
                        }
                        dbContext.GIRPhotographs.Remove(item);
                    }
                    dbContext.SaveChanges();
                }
                if (modal != null && modal.Count > 0)
                {
                    foreach (var item in modal)
                    {
                        var split = item.ImagePath.Split(',');
                        string OrignalString = split.LastOrDefault();
                        if (!string.IsNullOrEmpty(OrignalString))
                        {
                            byte[] imageBytes = Convert.FromBase64String(OrignalString);
                            string rootpath = HttpContext.Current.Server.MapPath("~/GIRPhotos/");

                            string subPath = UniqueFormID.ToString() + "/";
                            bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                            string CompleteFolderPath = Path.Combine(rootpath + subPath);
                            if (!Directory.Exists(CompleteFolderPath))
                            {
                                Directory.CreateDirectory(CompleteFolderPath);
                            }
                            var imageName = Guid.NewGuid().ToString() + "_" + item.FileName;
                            File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);

                            Entity.GIRPhotograph file = new Entity.GIRPhotograph();
                            file.GIRFormID = 0; // GIRID;
                            file.UniqueFormID = UniqueFormID;
                            file.FileName = item.FileName; // RDBJ 02/07/2022 set only filename rather than with guid //imageName; // "/GIRPhotos/" + subPath + imageName;
                            file.ImagePath = item.ImagePath;
                            file.ImageCaption = item.ImageCaption;
                            file.Ship = item.Ship;
                            file.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            file.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            dbContext.GIRPhotographs.Add(file);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRPhotos_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public List<GIRData> GetGIRDrafts(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<GIRData> list = new List<GIRData>();
            try
            {
                List<Entity.GeneralInspectionReport> dbList = dbContext.GeneralInspectionReports.Where(x => x.Ship == shipCode && x.SavedAsDraft == true
                && x.isDelete == 0  // RDBJ 01/05/2022
                ).ToList();
                list = dbList.OrderByDescending(x => x.CreatedDate).Select(x => new GIRData() // RDBJ 12/03/2021 set with created date
                {
                    UniqueFormID = x.UniqueFormID, // RDBJ 12/03/2021
                    Ship = x.Ship,
                    ShipName = x.ShipName,
                    Auditor = x.Inspector,
                    Location = x.Port,
                    Date = Utility.ToDateTimeStr(x.Date),
                    GIRFormID = x.GIRFormID,
                    GeneralPreamble = x.GeneralPreamble,
                    UpdatedDate = x.UpdatedDate,
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDrafts " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public APIResponse AddGIRDeficiencies(GIRDeficiencies Modal)
        {
            APIResponse retAPIresponse = new APIResponse();  // RDBJ 02/17/2022
            Dictionary<string, string> dictRetMetaData = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            GIRDeficiency member = null; //RDBJ 09/18/2021 new GIRDeficiency();
            try
            {
                if (Modal.UniqueFormID != null && Modal.UniqueFormID != Guid.Empty)
                {
                    //RDBJ 11/02/2021 Update if logic //RDBJ 09/18/2021 Update Logic
                    if (!string.IsNullOrEmpty(Modal.ItemNo))
                        member = dbContext.GIRDeficiencies.Where(x => x.Section == Modal.Section && (x.ItemNo == Modal.ItemNo || x.ItemNo == null) && x.UniqueFormID == Modal.UniqueFormID && x.isDelete == 0).FirstOrDefault(); //RDBJ 10/25/2021 Added x.isDelete == 0
                    //RDBJ 10/11/2021 added else
                    else
                        member = dbContext.GIRDeficiencies.Where(x => x.Section == Modal.Section && x.No == Modal.No && x.UniqueFormID == Modal.UniqueFormID && x.isDelete == 0).FirstOrDefault();

                    if (member == null)
                    {
                        member = new GIRDeficiency();
                        member.GIRFormID = 0; // RDBJ 12/14/2021
                        member.DeficienciesUniqueID = Modal.DeficienciesUniqueID; //RDBJ 09/18/2021
                        member.No = Modal.No;

                        // JSL 10/15/2022 wrapped in if
                        if (Modal.DateRaised != null)
                            member.DateRaised = Modal.DateRaised;
                        else
                            member.DateRaised = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime //Modal.DateRaised;
                        // End JSL 10/15/2022 wrapped in if
                        member.Deficiency = Modal.Deficiency;
                        member.DateClosed = Modal.DateClosed;
                        member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        member.Ship = Modal.Ship;

                        //RDBJ 11/02/2021 wrapped in if
                        if (Modal.DateClosed != null)
                            member.IsClose = true;
                        else
                            member.IsClose = false;

                        member.ReportType = Modal.ReportType;
                        member.ItemNo = Modal.ItemNo;
                        member.Section = Modal.Section;
                        member.UniqueFormID = Modal.UniqueFormID;
                        member.isDelete = 0;
                        member.Priority = Modal.Priority; //RDBJ 11/02/2021

                        member.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        dbContext.GIRDeficiencies.Add(member);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        member.DateRaised = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime //Modal.DateRaised;
                        member.Deficiency = Modal.Deficiency;
                        member.DateClosed = Modal.DateClosed;
                        member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime

                        dbContext.SaveChanges();
                    }
                    //End RDBJ 09/18/2021 Update Logic

                    GIRDeficienciesFile_Save(Modal, Modal.Ship, member.DeficienciesUniqueID); //RDBJ 11/02/2021

                    // JSL 06/27/2022
                    if (Modal.ReportType == "SI")
                    {
                        if (Modal.GIRDeficienciesInitialActions != null && Modal.GIRDeficienciesInitialActions.Count > 0)
                        {
                            foreach (var item in Modal.GIRDeficienciesInitialActions)
                            {
                                AddDeficienciesInitialActions(item);
                            }
                        }
                    }
                    // End JSL 06/27/2022

                    if (Modal.ReportType == "GI")
                    {
                        Entity.GeneralInspectionReport girForm = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == Modal.UniqueFormID).FirstOrDefault();
                        girForm.FormVersion = girForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/24/2021
                        girForm.IsSynced = false;
                        dbContext.SaveChanges();
                        dictRetMetaData["FormVersion"] = girForm.FormVersion.ToString();    // RDBJ 02/19/2022
                    }
                    else
                    {
                        Entity.SuperintendedInspectionReport sirForm = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == Modal.UniqueFormID).FirstOrDefault();
                        sirForm.FormVersion = sirForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/24/2021
                        sirForm.IsSynced = false;
                        dbContext.SaveChanges();
                        dictRetMetaData["FormVersion"] = sirForm.FormVersion.ToString();    // RDBJ 02/19/2022
                    }
                }
                dictRetMetaData["DeficienciesUniqueID"] = Convert.ToString(member.DeficienciesUniqueID);

                retAPIresponse.msg = JsonConvert.SerializeObject(dictRetMetaData);
                retAPIresponse.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficiencies " + ex.Message + "\n" + ex.InnerException);
            }
            return retAPIresponse;  // RDBJ 02/17/2022
        }
        public Modals.GeneralInspectionReport GIRFormGetDeficiency(string id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.GeneralInspectionReport dbModal = new Modals.GeneralInspectionReport();
            try
            {
                Guid UFormID = Guid.Parse(id);
                var Modal = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == UFormID).FirstOrDefault();
                if (Modal != null)
                {
                    dbModal.GIRFormID = Modal.GIRFormID;
                    dbModal.ShipID = Modal.ShipID;
                    dbModal.ShipName = Modal.ShipName;
                    dbModal.Ship = Modal.Ship;
                    dbModal.SavedAsDraft = Modal.SavedAsDraft;
                    dbModal.UniqueFormID = Modal.UniqueFormID;
                    dbModal.FormVersion = Modal.FormVersion;
                    dbModal.IsDeficienciesSectionComplete = Modal.IsDeficienciesSectionComplete; //RDBJ 10/20/2021
                    List<GIRDeficiencies> girDefis = new List<GIRDeficiencies>();

                    var girDefs = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UFormID && x.ReportType == "GI" && x.isDelete == 0).ToList();
                    if (girDefs != null && girDefs.Count > 0)
                    {
                        foreach (var def in girDefs)
                        {
                            GIRDeficiencies girDef = new GIRDeficiencies();
                            girDef.DeficienciesID = def.DeficienciesID;
                            girDef.No = def.No;
                            girDef.DateRaised = def.DateRaised;
                            girDef.Deficiency = def.Deficiency;
                            girDef.DateClosed = def.DateClosed;
                            girDef.CreatedDate = def.CreatedDate;
                            girDef.UpdatedDate = def.UpdatedDate;
                            girDef.Ship = def.Ship;
                            girDef.IsClose = def.IsClose;
                            girDef.ReportType = def.ReportType;
                            girDef.ItemNo = def.ItemNo;
                            girDef.Section = def.Section;
                            girDef.UniqueFormID = def.UniqueFormID;
                            girDef.isDelete = def.isDelete;
                            girDef.Priority = def.Priority == null ? 12 : def.Priority; //RDBJ 11/02/2021
                            girDef.DeficienciesUniqueID = def.DeficienciesUniqueID; //RDBJ 11/02/2021

                            var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList(); //RDBJ 11/02/2021 Added DeficienciesUniqueID
                            if (defFiles != null && defFiles.Count > 0)
                            {
                                foreach (var defFile in defFiles)
                                {
                                    Modals.GIRDeficienciesFile girDefFile = new Modals.GIRDeficienciesFile();
                                    girDefFile.FileName = defFile.FileName;
                                    //girDefFile.StorePath = defFile.StorePath;
                                    girDefFile.DeficienciesID = defFile.DeficienciesID;
                                    girDefFile.DeficienciesFileUniqueID = defFile.DeficienciesFileUniqueID;    // JSL 06/04/2022
                                    girDefFile.GIRDeficienciesFileID = defFile.GIRDeficienciesFileID;
                                    girDefFile.DeficienciesUniqueID = defFile.DeficienciesUniqueID; //RDBJ 11/02/2021
                                    girDefFile.IsUpload = "true"; //RDBJ 11/02/2021
                                    girDef.GIRDeficienciesFile.Add(girDefFile);
                                }
                            }
                            girDefis.Add(girDef);
                        }
                    }
                    dbModal.GIRDeficiencies = girDefis;
                    //dbModal.GIRDeficiencies = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UFormID && x.ReportType == "GI" && x.isDelete == 0).Select(y => new Modals.GIRDeficiencies()
                    //{
                    //    No = y.No,
                    //    Section = y.Section,
                    //    ItemNo = y.ItemNo,
                    //    Deficiency = y.Deficiency,
                    //    DateClosed = y.DateClosed,
                    //    DateRaised = y.DateRaised,
                    //    DeficienciesID = y.DeficienciesID,
                    //    GIRDeficienciesFile = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == y.DeficienciesID).Select(x => new Modals.GIRDeficienciesFile()
                    //    {
                    //        FileName = x.FileName,
                    //        StorePath = x.StorePath,
                    //        DeficienciesID = x.DeficienciesID,
                    //        GIRDeficienciesFileID = x.GIRDeficienciesFileID
                    //    }).ToList()
                    //}).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRFormGetDeficiency " + ex.Message + "\n" + ex.InnerException);
            }
            return dbModal;
        }

        //RDBJ 09/20/2021 Modified this Update Logic and so on in this Function
        public APIResponse AddDeficienciesInitialActions(GIRDeficienciesInitialActions data) // RDBJ 02/19/2022 set Return APIResponse //RDBJ 09/22/2021 Updateed Modal Name used
        {
            APIResponse aPIResponse = new APIResponse(); // RDBJ 02/19/2022 
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                var initUniqueID = Guid.NewGuid();

                // JSL 01/08/2023
                if (data.IniActUniqueID != null && data.IniActUniqueID != Guid.Empty)
                    initUniqueID = (Guid)data.IniActUniqueID;
                // End JSL 01/08/2023

                GIRDeficienciesInitialAction dbModel = new GIRDeficienciesInitialAction
                {
                    DeficienciesID = 0, //data.DeficienciesID, //RDBJ 10/14/2021 set 0
                    CreatedDate = data.CreatedDate.HasValue ? data.CreatedDate.Value : Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime,
                    Description = data.Description,
                    DeficienciesUniqueID = data.DeficienciesUniqueID,
                    GIRFormID = 0, //dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == data.DeficienciesUniqueID).Select(x => x.GIRFormID).FirstOrDefault(), //RDBJ 10/14/2021 set 0
                    Name = data.Name,
                    IniActUniqueID = initUniqueID,
                    //isNew = 1 // JSL 07/08/2022 commented this // RDBJ 01/05/2022 set 1 //RDBJ 10/14/2021
                    isNew = 0 // JSL 07/08/2022
                };
                dbContext.GIRDeficienciesInitialActions.Add(dbModel);
                dbContext.SaveChanges();
                SaveImageFileForInitialActions(data.GIRDeficienciesInitialActionsFiles, dbModel.GIRInitialID, initUniqueID);

                //var reportType = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == data.DeficienciesUniqueID).Select(x => x.ReportType).FirstOrDefault(); //RDBJ 09/21/2021 Commented

                //RDBJ 09/20/2021
                GIRDeficiency defDetails = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == data.DeficienciesUniqueID).FirstOrDefault(); //RDBJ 10/14/2021 set var to Entity.GIRDeficiency
                if (defDetails.ReportType.ToUpper() == "GI")
                {
                    Entity.GeneralInspectionReport girForm = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                    girForm.FormVersion = girForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                    girForm.IsSynced = false;
                    dbContext.SaveChanges();
                    aPIResponse.msg = girForm.FormVersion.ToString(); // RDBJ 02/19/2022
                }
                else
                {
                    Entity.SuperintendedInspectionReport sirForm = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                    sirForm.FormVersion = sirForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                    sirForm.IsSynced = false;
                    dbContext.SaveChanges();
                    aPIResponse.msg = sirForm.FormVersion.ToString(); // RDBJ 02/19/2022
                }
                //End RDBJ 09/20/2021

                defDetails.UpdatedDate = Utility.ToDateTimeUtcNow(); //RDBJ 10/14/2021
                dbContext.SaveChanges(); //RDBJ 10/14/2021
                aPIResponse.result = AppStatic.SUCCESS; // RDBJ 02/19/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesInitialActions " + ex.Message + "\n" + ex.InnerException);
                aPIResponse.result = AppStatic.ERROR; // RDBJ 02/19/2022
            }
            return aPIResponse; // RDBJ 02/19/2022
        }
        //End RDBJ 09/20/2021 Modified this Update Logic and so on in this Function

        private static string SaveImageFileForInitialActions(List<Modals.GIRDeficienciesInitialActionsFile> modal, long GIRInitialID, Guid initUniqueID)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modal != null)
                {
                    foreach (var item in modal)
                    {
                        if (item.IsUpload == "true")
                        {
                            // JSL 12/03/2022
                            Entity.GIRDeficienciesInitialActionsFile file = new Entity.GIRDeficienciesInitialActionsFile();
                            file.FileName = item.FileName;
                            file.StorePath = item.StorePath;
                            file.GIRInitialID = 0; 
                            file.DeficienciesID = 0;
                            file.IsUpload = item.IsUpload;
                            file.IniActUniqueID = initUniqueID;
                            file.IniActFileUniqueID = Guid.NewGuid();
                            dbContext.GIRDeficienciesInitialActionsFiles.Add(file);
                            dbContext.SaveChanges();
                            // End JSL 12/03/2022

                            // JSL 12/03/2022 commented
                            /*
                            var split = item.StorePath.Split(',');
                            string OrignalString = split.LastOrDefault();
                            if (!string.IsNullOrEmpty(OrignalString))
                            {
                                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                                OrignalString = regex.Replace(OrignalString, string.Empty);
                                byte[] imageBytes = Convert.FromBase64String(OrignalString);
                                string rootpath = HttpContext.Current.Server.MapPath("~/GIRInitialActionsFile/");

                                string subPath = item.DeficienciesID.ToString() + "/" + GIRInitialID.ToString() + "/";
                                bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                                if (!exists)
                                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                                string CompleteFolderPath = Path.Combine(rootpath + subPath);
                                if (!Directory.Exists(CompleteFolderPath))
                                {
                                    Directory.CreateDirectory(CompleteFolderPath);
                                }
                                var imageName = item.GIRFileID + "_" + DateTime.Now.ToString("MMdddyyyhhmmss") + "_" + item.FileName;
                                File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);

                                Entity.GIRDeficienciesInitialActionsFile file = new Entity.GIRDeficienciesInitialActionsFile();
                                file.FileName = item.FileName;
                                file.StorePath = item.StorePath;// "/GIRInitialActionsFile/" + subPath + imageName;
                                file.GIRInitialID = 0; //GIRInitialID; //RDBJ 10/14/2021 set 0
                                file.DeficienciesID = 0;//item.DeficienciesID; //RDBJ 10/14/2021 set 0
                                file.IsUpload = item.IsUpload;
                                file.IniActUniqueID = initUniqueID;
                                file.IniActFileUniqueID = Guid.NewGuid();
                                dbContext.GIRDeficienciesInitialActionsFiles.Add(file);
                                dbContext.SaveChanges();
                            }
                            */
                            // End JSL 12/03/2022 commented
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveImageFileForComments " + ex.Message + "\n" + ex.InnerException);
            }
            return "";
        }

        //RDBJ 09/20/2021 Modified this Update Logic and so on in this Function
        public void AddDeficienciesResolution(Modals.GIRDeficienciesResolution data) //RDBJ 09/22/2021 Updateed Modal Name used
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                var ResolutionUniqueID = Guid.NewGuid();
                Entity.GIRDeficienciesResolution dbModel = new Entity.GIRDeficienciesResolution //RDBJ 09/22/2021 Use Entity
                {
                    DeficienciesID = 0, //data.DeficienciesID, //RDBJ 10/14/2021 set 0
                    CreatedDate = data.CreatedDate.HasValue ? data.CreatedDate.Value : Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Resolution = data.Resolution,
                    DeficienciesUniqueID = data.DeficienciesUniqueID,
                    GIRFormID = 0, //dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == data.DeficienciesUniqueID).Select(x => x.GIRFormID).FirstOrDefault(), // RDBJ 10/14/2021 set 0
                    Name = data.Name,
                    ResolutionUniqueID = ResolutionUniqueID,
                    //isNew = 1 // JSL 07/08/2022 commented this // RDBJ 01/05/2022 set 1 //RDBJ 10/14/2021
                    isNew = 0 // JSL 07/08/2022
                };
                dbContext.GIRDeficienciesResolutions.Add(dbModel);
                dbContext.SaveChanges();
                SaveImageFileForResolution(data.GIRDeficienciesResolutionFiles, dbModel.GIRResolutionID, ResolutionUniqueID);

                //RDBJ 09/20/2021
                GIRDeficiency defDetails = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == data.DeficienciesUniqueID).FirstOrDefault(); //RDBJ 10/14/2021 set var to Entity.GIRDeficiency
                if (defDetails.ReportType.ToUpper() == "GI")
                {
                    Entity.GeneralInspectionReport girForm = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                    girForm.FormVersion = girForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                    girForm.IsSynced = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    Entity.SuperintendedInspectionReport sirForm = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                    sirForm.FormVersion = sirForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                    sirForm.IsSynced = false;
                    dbContext.SaveChanges();
                }
                //End RDBJ 09/20/2021

                defDetails.UpdatedDate = Utility.ToDateTimeUtcNow(); //RDBJ 10/14/2021
                dbContext.SaveChanges(); //RDBJ 10/14/2021
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesResolution " + ex.Message + "\n" + ex.InnerException);

            }
        }
        //End RDBJ 09/20/2021 Modified this Update Logic and so on in this Function

        private static string SaveImageFileForResolution(List<Modals.GIRDeficienciesResolutionFile> modal, long GIRResolutionID, Guid ResolutionUniqueID)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modal != null)
                {
                    foreach (var item in modal)
                    {
                        if (item.IsUpload == "true")
                        {
                            var split = item.StorePath.Split(',');
                            string OrignalString = split.LastOrDefault();
                            if (!string.IsNullOrEmpty(OrignalString))
                            {
                                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                                OrignalString = regex.Replace(OrignalString, string.Empty);
                                byte[] imageBytes = Convert.FromBase64String(OrignalString);
                                string rootpath = HttpContext.Current.Server.MapPath("~/GIRResolutionFile/");

                                string subPath = item.DeficienciesID.ToString() + "/" + GIRResolutionID.ToString() + "/";
                                bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
                                if (!exists)
                                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
                                string CompleteFolderPath = Path.Combine(rootpath + subPath);
                                if (!Directory.Exists(CompleteFolderPath))
                                {
                                    Directory.CreateDirectory(CompleteFolderPath);
                                }
                                var imageName = item.GIRFileID + "_" + DateTime.Now.ToString("MMdddyyyhhmmss") + "_" + item.FileName;
                                File.WriteAllBytes(Path.Combine(CompleteFolderPath + imageName), imageBytes);

                                Entity.GIRDeficienciesResolutionFile file = new Entity.GIRDeficienciesResolutionFile();
                                file.FileName = item.FileName;
                                file.StorePath = item.StorePath;// "/GIRResolutionFile/" + subPath + imageName;
                                file.GIRResolutionID = 0; //GIRResolutionID; //RDBJ 10/14/2021 set 0
                                file.DeficienciesID = 0; //item.DeficienciesID; //RDBJ 10/14/2021 set 0
                                file.IsUpload = item.IsUpload;
                                file.ResolutionUniqueID = ResolutionUniqueID;
                                file.ResolutionFileUniqueID = Guid.NewGuid();
                                dbContext.GIRDeficienciesResolutionFiles.Add(file);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveImageFileForResolution " + ex.Message + "\n" + ex.InnerException);
            }
            return "";
        }
        public List<GIRDeficienciesInitialActions> GetDeficienciesInitialActions(Guid id) //RDBJ 09/22/2021 Updateed Modal Name used
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<GIRDeficienciesInitialActions> list = new List<GIRDeficienciesInitialActions>(); //RDBJ 09/22/2021 Updateed Modal Name used
            try
            {
                var data = dbContext.GIRDeficienciesInitialActions.Where(x => x.DeficienciesUniqueID == id).ToList();
                foreach (var item in data)
                {
                    GIRDeficienciesInitialActions obj = new GIRDeficienciesInitialActions(); //RDBJ 09/22/2021 Updateed Modal Name used
                    var notefile = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActUniqueID == item.IniActUniqueID).ToList();
                    if (notefile != null && notefile.Count > 0)
                    {
                        foreach (var itemFile in notefile)
                        {
                            obj.GIRDeficienciesInitialActionsFiles.Add(new Modals.GIRDeficienciesInitialActionsFile
                            {
                                //DeficienciesID = itemFile.DeficienciesID, //RDBJ 10/14/2021 commented not required
                                FileName = itemFile.FileName,
                                //GIRFileID = itemFile.GIRFileID, //RDBJ 10/14/2021 commented not required
                                //GIRInitialID = itemFile.GIRInitialID, //RDBJ 10/14/2021 commented not required
                                IniActFileUniqueID = itemFile.IniActFileUniqueID,
                                //IniActUniqueID = itemFile.IniActUniqueID, //RDBJ 10/14/2021 commented not required
                                //IsUpload = itemFile.IsUpload, //RDBJ 10/14/2021 commented not required
                                //StorePath = itemFile.StorePath //RDBJ 10/14/2021 commented not required
                            });
                        }
                    }

                    obj.Description = item.Description;
                    obj.Name = item.Name;
                    obj.CreatedDate = item.CreatedDate;
                    obj.isNew = item.isNew; //RDBJ 10/16/2021
                    list.Add(obj);
                }
                list = list.OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesInitialActions " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public List<Modals.GIRDeficienciesResolution> GetDeficienciesResolution(Guid id) //RDBJ 09/22/2021 Updateed Modal Name used
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Modals.GIRDeficienciesResolution> list = new List<Modals.GIRDeficienciesResolution>(); //RDBJ 09/22/2021 Updateed Modal Name used
            try
            {
                var data = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesUniqueID == id).ToList();
                foreach (var item in data)
                {
                    Modals.GIRDeficienciesResolution obj = new Modals.GIRDeficienciesResolution(); //RDBJ 09/22/2021 Updateed Modal Name used
                    //obj.GIRDeficienciesResolutionFiles = new List<GIRDeficienciesResolutionFile>();
                    var notefile = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == item.ResolutionUniqueID).ToList();
                    if (notefile != null && notefile.Count > 0)
                        foreach (var itemFile in notefile)
                        {
                            obj.GIRDeficienciesResolutionFiles.Add(new Modals.GIRDeficienciesResolutionFile
                            {
                                //StorePath = itemFile.StorePath, //RDBJ 10/14/2021 commented not required
                                //DeficienciesID = itemFile.DeficienciesID, //RDBJ 10/14/2021 commented not required
                                //IsUpload = itemFile.IsUpload, //RDBJ 10/14/2021 commented not required
                                FileName = itemFile.FileName,
                                //GIRFileID = itemFile.GIRFileID, //RDBJ 10/14/2021 commented not required
                                //GIRResolutionID = itemFile.GIRResolutionID, //RDBJ 10/14/2021 commented not required
                                ResolutionFileUniqueID = itemFile.ResolutionFileUniqueID,
                                //ResolutionUniqueID = itemFile.ResolutionUniqueID //RDBJ 10/14/2021 commented not required
                            });
                        }

                    obj.Resolution = item.Resolution;
                    obj.Name = item.Name;
                    obj.CreatedDate = item.CreatedDate;
                    obj.isNew = item.isNew; //RDBJ 10/16/2021
                    list.Add(obj);
                }
                list = list.OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesResolution " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }

        //RDBJ 10/07/2021
        public bool GIRShipGeneralDescriptionSave(CSShipsModal modal)
        {
            bool res = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                CSShip shipsModal = new CSShip();
                shipsModal = dbContext.CSShips.Where(x => x.Code == modal.Code).FirstOrDefault();
                if (shipsModal != null)
                {
                    shipsModal.MMSI = modal.MMSI;
                    shipsModal.BuildYear = modal.BuildYear;
                    shipsModal.IMONumber = modal.IMONumber;
                    shipsModal.CallSign = modal.CallSign;
                    shipsModal.SummerDeadweight = modal.SummerDeadweight;
                    shipsModal.GrossTonnage = modal.GrossTonnage;
                    shipsModal.Lightweight = modal.Lightweight;
                    shipsModal.NetTonnage = modal.NetTonnage;
                    shipsModal.Beam = modal.Beam;
                    shipsModal.LOA = modal.LOA;
                    shipsModal.SummerDraft = modal.SummerDraft;
                    shipsModal.LBP = modal.LBP;
                    shipsModal.BHP = modal.BHP;
                    shipsModal.ClassificationSocietyId = modal.ClassificationSocietyId;
                    shipsModal.FlagStateId = modal.FlagStateId;
                    shipsModal.PortOfRegistryId = modal.PortOfRegistryId;
                    shipsModal.BowThruster = modal.BowThruster;

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRShipGeneralDescriptionSave " + ex.Message + "\n" + ex.InnerException);
            }
            return res;
        }
        //End RDBJ 10/07/2021

        #region Common Functions
        // RDBJ 03/05/2022
        public Dictionary<string, string> PerformPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                // RDBJ 04/02/2022
                case AppStatic.API_DELETESIRNOTEORSIRADDITIONALNOTE:
                    {
                        try
                        {
                            Guid NotesUniqueID = Guid.Empty;
                            Guid UniqueFormID = Guid.Empty;
                            bool IsSIRAdditionalNote = false;

                            if (dictMetaData.ContainsKey("NotesUniqueID"))
                                NotesUniqueID = Guid.Parse(dictMetaData["NotesUniqueID"].ToString());

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                UniqueFormID = Guid.Parse(dictMetaData["UniqueFormID"].ToString());

                            if (dictMetaData.ContainsKey("IsSIRAdditionalNote"))
                                IsSIRAdditionalNote = Convert.ToBoolean(dictMetaData["IsSIRAdditionalNote"].ToString());

                            if (IsSIRAdditionalNote)
                            {
                                Entity.SIRAdditionalNote objSIRAddNote = dbContext.SIRAdditionalNotes.Where(x => x.NotesUniqueID == NotesUniqueID).FirstOrDefault();
                                objSIRAddNote.IsDeleted = 1;
                            }
                            else
                            {
                                Entity.SIRNote objSIRNote = dbContext.SIRNotes.Where(x => x.NotesUniqueID == NotesUniqueID).FirstOrDefault();
                                objSIRNote.IsDeleted = 1;
                            }

                            dbContext.SaveChanges();

                            retDictMetaData = UpdateFormVersion(string.Empty, AppStatic.SIRForm, UniqueFormID.ToString(), false);
                            retDictMetaData["NotesUniqueID"] = NotesUniqueID.ToString();
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_DELETESIRNOTEORSIRADDITIONALNOTE + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ 04/02/2022
                // RDBJ 03/19/2022
                case AppStatic.API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS:
                    {
                        try
                        {
                            string strUniqueFormID = string.Empty;
                            string strShip = string.Empty;
                            string strFormType = string.Empty;

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                strUniqueFormID = dictMetaData["UniqueFormID"].ToString();

                            if (dictMetaData.ContainsKey("Ship"))
                                strShip = dictMetaData["Ship"].ToString();

                            if (dictMetaData.ContainsKey("FormType"))
                                strFormType = dictMetaData["FormType"].ToString();

                            UpdateDeficienciesShipWhenChangeShipInForms(strUniqueFormID, strFormType, strShip, ref retDictMetaData);

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ 03/19/2022
                // RDBJ 03/12/2022
                case AppStatic.API_UPLOADGISIDEFICIENCIESFILEORPHOTO:
                    {
                        try
                        {
                            string strDeficienciesUniqueID = string.Empty;
                            string strFileName = string.Empty;
                            string strImagePath = string.Empty;

                            // JSL 06/04/2022
                            string strDeficienciesFileUniqueID = string.Empty;

                            if (dictMetaData.ContainsKey("DeficienciesFileUniqueID"))
                                strDeficienciesFileUniqueID = dictMetaData["DeficienciesFileUniqueID"].ToString();
                            // End JSL 06/04/2022

                            if (dictMetaData.ContainsKey("DeficienciesUniqueID"))
                                strDeficienciesUniqueID = dictMetaData["DeficienciesUniqueID"].ToString();

                            if (dictMetaData.ContainsKey("FileName"))
                                strFileName = dictMetaData["FileName"].ToString();

                            if (dictMetaData.ContainsKey("ImagePath"))
                                strImagePath = dictMetaData["ImagePath"].ToString();

                            Entity.GIRDeficienciesFile file = new Entity.GIRDeficienciesFile();
                            file.DeficienciesID = 0;
                            file.DeficienciesUniqueID = Guid.Parse(strDeficienciesUniqueID);
                            file.DeficienciesFileUniqueID = Guid.Parse(strDeficienciesFileUniqueID);    // JSL 06/04/2022
                            file.FileName = strFileName;
                            file.StorePath = strImagePath;

                            dbContext.GIRDeficienciesFiles.Add(file);
                            dbContext.SaveChanges();

                            var dictMetadataReturn = UpdateFormVersion(strDeficienciesUniqueID);    // JSL 06/04/2022 set with return data

                            retDictMetaData["FormVersion"] = dictMetadataReturn["FormVersion"]; // JSL 06/04/2022

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPLOADGISIDEFICIENCIESFILEORPHOTO + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ 03/12/2022
                case AppStatic.API_UPLOADGIRPHOTOGRAPHS:
                    {
                        try
                        {
                            string strUniqueFormID = string.Empty;
                            string strFileName = string.Empty;
                            string strImagePath = string.Empty;
                            string strShip = string.Empty;

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                strUniqueFormID = dictMetaData["UniqueFormID"].ToString();

                            if (dictMetaData.ContainsKey("FileName"))
                                strFileName = dictMetaData["FileName"].ToString();

                            if (dictMetaData.ContainsKey("ImagePath"))
                                strImagePath = dictMetaData["ImagePath"].ToString();

                            if (dictMetaData.ContainsKey("Ship"))
                                strShip = dictMetaData["Ship"].ToString();

                            Entity.GIRPhotograph file = new Entity.GIRPhotograph();
                            file.GIRFormID = 0;
                            file.UniqueFormID = Guid.Parse(strUniqueFormID);
                            file.FileName = strFileName; 
                            file.ImagePath = strImagePath;
                            file.ImageCaption = string.Empty;
                            file.Ship = strShip;
                            file.CreatedDate = Utility.ToDateTimeUtcNow(); 
                            file.UpdatedDate = Utility.ToDateTimeUtcNow(); 

                            dbContext.GIRPhotographs.Add(file);
                            dbContext.SaveChanges();

                            // RDBJ 03/12/2022
                            var dictMetadataReturn = UpdateFormVersion(string.Empty, AppStatic.GIRForm, strUniqueFormID, false);    // JSL 06/04/2022 set with return data
                            // End RDBJ 03/12/2022

                            retDictMetaData["FormVersion"] = dictMetadataReturn["FormVersion"]; // JSL 06/04/2022

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPLOADGIRPHOTOGRAPHS + " Error : " + ex.Message);
                        }
                        break;
                    }
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
        // End RDBJ 03/05/2022

        // RDBJ 03/19/2022 set return type with Dictionery // RDBJ 03/12/2022
        public static Dictionary<string, string> UpdateFormVersion(string strDeficienciesOrAuditUniqueID
            , string strReportType = ""
            , string strUniqueFormID = ""
            , bool IsUploadFormVersionByDeficiency = true
            )
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();  // RDBJ 03/19/2022
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid DeficienciesOrAuditUniqueID = Guid.Empty;
            Guid UniqueFormID = Guid.Empty;

            if (!string.IsNullOrEmpty(strDeficienciesOrAuditUniqueID))
                DeficienciesOrAuditUniqueID = Guid.Parse(strDeficienciesOrAuditUniqueID);

            if (!string.IsNullOrEmpty(strUniqueFormID))
                UniqueFormID = Guid.Parse(strUniqueFormID);

            try
            {
                if (IsUploadFormVersionByDeficiency
                    && DeficienciesOrAuditUniqueID != Guid.Empty)
                {
                    // JSL 06/28/2022 wrapped in if
                    if (strReportType.ToUpper() == AppStatic.IAFForm)
                    {
                        Entity.AuditNote entityAuditNote = dbContext.AuditNotes.Where(x => x.NotesUniqueID == DeficienciesOrAuditUniqueID).FirstOrDefault();
                        strReportType = AppStatic.IAFForm;
                        UniqueFormID = (Guid)entityAuditNote.UniqueFormID;
                    }
                    // End JSL 06/28/2022 wrapped in if
                    else
                    {
                        Entity.GIRDeficiency defDetails = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == DeficienciesOrAuditUniqueID).FirstOrDefault();
                        strReportType = defDetails.ReportType.ToUpper();
                        UniqueFormID = (Guid)defDetails.UniqueFormID;
                    }
                }

                if (strReportType.ToUpper() == AppStatic.GIRForm
                    && UniqueFormID != Guid.Empty)
                {
                    Entity.GeneralInspectionReport girForm = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
                    girForm.FormVersion = girForm.FormVersion + Convert.ToDecimal(0.01);
                    girForm.IsSynced = false;
                    retDictMetaData["FormVersion"] = Convert.ToString(girForm.FormVersion); // RDBJ 03/19/2022
                }
                else if (strReportType.ToUpper() == AppStatic.SIRForm
                    && UniqueFormID != Guid.Empty)
                {
                    Entity.SuperintendedInspectionReport sirForm = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
                    sirForm.FormVersion = sirForm.FormVersion + Convert.ToDecimal(0.01);
                    sirForm.IsSynced = false;
                    retDictMetaData["FormVersion"] = Convert.ToString(sirForm.FormVersion); // RDBJ 03/19/2022
                }
                // RDBJ 03/19/2022
                else if (strReportType.ToUpper() == AppStatic.IAFForm
                    && UniqueFormID != Guid.Empty)
                {
                    Entity.InternalAuditForm iarForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
                    iarForm.FormVersion = iarForm.FormVersion + Convert.ToDecimal(0.01);
                    iarForm.IsSynced = false;
                    retDictMetaData["FormVersion"] = Convert.ToString(iarForm.FormVersion); // RDBJ 03/19/2022
                }
                // End RDBJ 03/19/2022
                // JSL 02/17/2023
                else if (strReportType.ToUpper() == AppStatic.FSTOForm
                    && UniqueFormID != Guid.Empty)
                {
                    Entity.FSTOInspection fstoForm = dbContext.FSTOInspections.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
                    fstoForm.FormVersion = fstoForm.FormVersion + Convert.ToDecimal(0.01);
                    fstoForm.IsSynced = false;
                    retDictMetaData["FormVersion"] = Convert.ToString(fstoForm.FormVersion);
                }
                // End JSL 02/17/2023

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                retDictMetaData = new Dictionary<string, string>(); // RDBJ 03/19/2022
                LogHelper.writelog(strReportType + " UpdateFormVersion Error : " + ex.Message);
            }
            return retDictMetaData; // RDBJ 03/19/2022
        }
        // End RDBJ 03/12/2022

        // RDBJ 03/19/2022
        public static void UpdateDeficienciesShipWhenChangeShipInForms(string strUniqueFormID, string strFormType, string strShip
            , ref Dictionary<string, string> retDictMetaData)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                string strTableName = string.Empty;

                if (strFormType.ToUpper() == AppStatic.IAFForm)
                {
                    strTableName = "AuditNotes";
                }
                else if (strFormType.ToUpper() == AppStatic.GIRForm
                    || strFormType.ToUpper() == AppStatic.SIRForm
                    )
                {
                    strTableName = "GIRDeficiencies";
                }

                var query = "UPDATE [" + strTableName + "] SET [Ship] = '" + strShip + "' WHERE [UniqueFormID] = '" + strUniqueFormID + "'";
                dbContext.Database.ExecuteSqlCommand(query);

                retDictMetaData = UpdateFormVersion(string.Empty, strFormType, strUniqueFormID, false);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(strFormType + " UpdateDeficienciesShipWhenChangeShipInForms Error : " + ex.Message);
            }
        }
        // End RDBJ 03/19/2022

        #endregion
    }
}
