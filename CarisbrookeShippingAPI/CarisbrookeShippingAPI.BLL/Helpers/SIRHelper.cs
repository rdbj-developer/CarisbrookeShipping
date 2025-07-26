using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class SIRHelper
    {
        public void SubmitSIR(SIRModal Modal)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.SuperintendedInspectionReport dbModal = new Entity.SuperintendedInspectionReport();
                if (Modal != null && Modal.SuperintendedInspectionReport.UniqueFormID != Guid.Empty)
                {
                    dbModal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == Modal.SuperintendedInspectionReport.UniqueFormID).FirstOrDefault();
                }

                if (dbModal == null)
                    dbModal = new Entity.SuperintendedInspectionReport();

                // JSL 12/31/2022
                if (Modal.SuperintendedInspectionReport.ShipID == null || Modal.SuperintendedInspectionReport.ShipID == 0)
                {
                    var dbships = dbContext.CSShips.Where(x => x.Code == Modal.SuperintendedInspectionReport.ShipName).FirstOrDefault();
                    if (dbships != null)
                    {
                        Modal.SuperintendedInspectionReport.ShipID = dbships.ShipId;
                    }
                }
                // End JSL 12/31/2022

                SetSIRFormDataRefEntity(ref dbModal, Modal.SuperintendedInspectionReport); //RDBJ 09/25/2021
                dbModal.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbModal.SavedAsDraft = false;
                dbModal.isDelete = 0; // RDBJ 01/05/2022

                if (dbModal != null && dbModal.UniqueFormID != null) //RDBJ 09/25/2021 condition changed if (Modal != null && dbModal.UniqueFormID != null && dbModal.UniqueFormID != Guid.Empty)
                {
                    dbModal.IsSynced = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbModal.IsSynced = false;
                    dbModal.UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID;
                    dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion;
                    dbModal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.SuperintendedInspectionReports.Add(dbModal);
                    dbContext.SaveChanges();
                }
                if (Modal.SIRNote != null && Modal.SIRNote.Count > 0)
                {
                    SIRNotes_Save(Modal.SuperintendedInspectionReport.UniqueFormID, Modal.SIRNote);
                }
                if (Modal.SIRAdditionalNote != null && Modal.SIRAdditionalNote.Count > 0)
                {
                    SIRAdditionalNote_Save(Modal.SuperintendedInspectionReport.UniqueFormID, Modal.SIRAdditionalNote);
                }
                if (Modal.GIRDeficiencies != null && Modal.GIRDeficiencies.Count > 0)
                {
                    GIRDeficiencies_Save(Modal.SuperintendedInspectionReport.UniqueFormID, Modal.GIRDeficiencies);
                }

                List<string> d = new List<string>();
                PropertyInfo[] properties = typeof(Modals.SuperintendedInspectionReport).GetProperties();
                LogHelper.writelog("SubmitSIR : SIR save SuperintendedInspectionReport :" + properties.Length);
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name.Contains("Condition"))
                    {
                        if (Convert.ToString(property.GetValue(Modal.SuperintendedInspectionReport)) == "Poor" || Convert.ToString(property.GetValue(Modal.SuperintendedInspectionReport)) == "Unsatisfactory")
                        {
                            LogHelper.writelog("SubmitSIR : SIR save for poor");
                            //AddDeficiency(property, Modal.SuperintendedInspectionReport);
                        }
                    }
                }
                //-------- 23-08 ------
                //GIRDeficiencies_Save(dbModal.SIRFormID, dbModal.ShipName, Modal.GIRDeficiencies);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitSIR : " + ex.Message + " : " + ex.InnerException);
            }
        }
        public string SIRAutoSave(SIRModal Modal, bool IsSave = false)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.SuperintendedInspectionReport dbModal = null;

                if (Modal != null && Modal.SuperintendedInspectionReport != null && Modal.SuperintendedInspectionReport.UniqueFormID != Guid.Empty)
                {
                    dbModal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == Modal.SuperintendedInspectionReport.UniqueFormID).FirstOrDefault();
                }

                if (dbModal == null)
                    dbModal = new Entity.SuperintendedInspectionReport();

                SetSIRFormDataRefEntity(ref dbModal, Modal.SuperintendedInspectionReport);
                dbModal.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbModal.isDelete = 0; // RDBJ 01/05/2022

                // JSL 12/31/2022
                if (dbModal.ShipID == null || dbModal.ShipID == 0)
                {
                    var dbships = dbContext.CSShips.Where(x => x.Code == dbModal.ShipName).FirstOrDefault();
                    if (dbships != null)
                    {
                        dbModal.ShipID = dbships.ShipId;
                    }
                }
                // End JSL 12/31/2022

                if (dbModal != null && dbModal.UniqueFormID != null)
                {
                    dbModal.IsSynced = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbModal.IsSynced = false;
                    dbModal.UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID;
                    dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion;
                    dbModal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.SuperintendedInspectionReports.Add(dbModal);
                    dbContext.SaveChanges();
                }

                if (Modal.NotesChanged)
                {
                    SIRNotes_Save(Modal.SuperintendedInspectionReport.UniqueFormID, Modal.SIRNote);
                }
                if (Modal.AdditionalNotesChanged)
                {
                    SIRAdditionalNote_Save(Modal.SuperintendedInspectionReport.UniqueFormID, Modal.SIRAdditionalNote);
                }
                if (Modal.DeficienciesChanged)
                {
                    GIRDeficiencies_Save(Modal.SuperintendedInspectionReport.UniqueFormID, Modal.GIRDeficiencies);
                }

                PropertyInfo[] properties = typeof(Modals.SuperintendedInspectionReport).GetProperties();
                LogHelper.writelog("SIRAutoSave : SIR save SuperintendedInspectionReport :" + properties.Length);
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name.Contains("Condition"))
                    {
                        if (Convert.ToString(property.GetValue(Modal.SuperintendedInspectionReport)) == "Poor" || Convert.ToString(property.GetValue(Modal.SuperintendedInspectionReport)) == "Unsatisfactory")
                        {
                            LogHelper.writelog("SIRAutoSave : SIR save for poor");
                            //AddDeficiency(property, Modal.SuperintendedInspectionReport);
                        }
                    }
                }
                return Convert.ToString(dbModal.UniqueFormID);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Auto Save SIR : " + ex.Message);
                return string.Empty;
            }
        }
        public List<SIRData> GetSIRDrafts(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<SIRData> list = new List<SIRData>();
            try
            {
                //var dbList = (from sir in dbContext.SuperintendedInspectionReports
                //              join ship in dbContext.CSShips on sir.ShipName equals ship.Code
                //              where ship.Code == shipCode && sir.SavedAsDraft == true
                //              select new { sir.SIRFormID, sir.ShipName, sir.Port, sir.Master, sir.Superintended, sir.Date, ship.Name, sir.ModifyDate })
                //             .ToList();
                var dbList = dbContext.SuperintendedInspectionReports.Where(x => x.ShipName == shipCode && x.SavedAsDraft == true
                && x.isDelete == 0  // RDBJ 01/05/2022
                ).ToList();
                list = dbList.OrderByDescending(x => x.CreatedDate).Select(x => new SIRData() // RDBJ 12/03/2021 set  with Created date
                {
                    Ship = x.ShipName,
                    ShipName = x.ShipName,
                    Master = x.Master,
                    Location = x.Port,
                    Date = Utility.ToDateTimeStr(x.Date),
                    SIRFormID = x.SIRFormID,
                    Superintended = x.Superintended,
                    UniqueFormID = x.UniqueFormID,
                    UpdatedDate = x.ModifyDate,
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSIRDrafts " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        public void AddDeficiency(PropertyInfo property, Entity.SuperintendedInspectionReport Modal)
        {
            try
            {
                Entity.GIRDeficiency deficiency = new Entity.GIRDeficiency();

                deficiency.IsClose = false;
                deficiency.ReportType = "SI";
                deficiency.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                deficiency.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                deficiency.GIRFormID = Modal.SIRFormID;
                deficiency.UniqueFormID = Modal.UniqueFormID;
                deficiency.Ship = Modal.ShipName;

                if (property.Name.Contains("Section1_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "1." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section1_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section2_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "2." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section2_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section3_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "3." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section3_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section4_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "4." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section4_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section5_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "5." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section5_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section6_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "6." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section6_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section7_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "7." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section7_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section8_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "8." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section8_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section9_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "9." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section9_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section10_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "10." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section10_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section11_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "11." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section11_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section12_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "12." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section12_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section13_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "13." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section13_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section14_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "14." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section14_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section15_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "15." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section15_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section16_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "16." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section16_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section17_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "17." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section17_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                if (property.Name.Contains("Section18_"))
                {
                    string[] list = property.Name.Split('_');
                    deficiency.SIRNo = "18." + list[1];
                    deficiency.Deficiency = Modal.GetType().GetProperty("Section18_" + list[1] + "_Comment").ToString().Replace("System.String", "");
                }
                LogHelper.writelog("AddDeficiency : SIR save for poor1");
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                LogHelper.writelog("AddDeficiency : SIR save for poor2");
                dbContext.GIRDeficiencies.Add(deficiency);
                LogHelper.writelog("AddDeficiency : SIR save for poor3");
                dbContext.SaveChanges();
                LogHelper.writelog("AddDeficiency : SIR save for poor4");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficiency : " + ex.Message);
            }

        }
        public List<Entity.SuperintendedInspectionReport> GetAllSIRForms()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Entity.SuperintendedInspectionReport> list = new List<Entity.SuperintendedInspectionReport>();
            var data = dbContext.SuperintendedInspectionReports
                         .Select(x => new { x.SIRFormID, x.ShipName, x.Date, x.Port, x.Master }).ToList();
            foreach (var item in data)
            {
                Entity.SuperintendedInspectionReport obj = new Entity.SuperintendedInspectionReport();
                obj.SIRFormID = item.SIRFormID;
                obj.ShipName = item.ShipName;
                obj.Date = item.Date;
                obj.Port = item.Port;
                obj.Master = item.Master;
                list.Add(obj);
            }
            return list;
        }
        public SIRModal SIRFormDetailsView(string id)
        {
            var UniqueFormID = new Guid(id);
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            SIRModal dbModal = new SIRModal();
            dbModal.SuperintendedInspectionReport = new Modals.SuperintendedInspectionReport();
            dbModal.SIRNote = new List<Modals.SIRNote>();
            dbModal.SIRAdditionalNote = new List<Modals.SIRAdditionalNote>();

            var Modal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
            if (Modal != null)
                SetSIRFormData(Modal, ref dbModal);

            var SIRNoteModal = dbContext.SIRNotes.Where(
                x => x.UniqueFormID == UniqueFormID
                && x.NotesUniqueID != null  // RDBJ 04/04/2022
                && x.IsDeleted == 0 // RDBJ 04/02/2022
                ).ToList();
            foreach (var itemNote in SIRNoteModal)
            {
                Modals.SIRNote sirNote = new Modals.SIRNote();

                sirNote.NoteID = itemNote.NoteID;
                sirNote.SIRFormID = itemNote.SIRFormID;
                sirNote.Number = itemNote.Number;
                sirNote.Note = itemNote.Note;
                sirNote.UniqueFormID = itemNote.UniqueFormID;
                sirNote.NotesUniqueID = (Guid)itemNote.NotesUniqueID; // RDBJ 04/02/2022

                dbModal.SIRNote.Add(sirNote);
            }

            var SIRAddNoteModal = dbContext.SIRAdditionalNotes.Where(
                x => x.UniqueFormID == UniqueFormID
                && x.NotesUniqueID != null  // RDBJ 04/04/2022
                && x.IsDeleted == 0 // RDBJ 04/02/2022
                ).ToList();
            foreach (var itemAddNote in SIRAddNoteModal)
            {
                Modals.SIRAdditionalNote sirAddNote = new Modals.SIRAdditionalNote();

                sirAddNote.NoteID = itemAddNote.NoteID;
                sirAddNote.SIRFormID = itemAddNote.SIRFormID;
                sirAddNote.Number = itemAddNote.Number;
                sirAddNote.Note = itemAddNote.Note;
                sirAddNote.UniqueFormID = itemAddNote.UniqueFormID;
                sirAddNote.NotesUniqueID = (Guid)itemAddNote.NotesUniqueID; // RDBJ 04/02/2022

                dbModal.SIRAdditionalNote.Add(sirAddNote);
            }

            return dbModal;
        }
        public List<Modals.SIRNote> SIRFormNotes(string id)
        {
            var UniqueFormID = new Guid(id);
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Modals.SIRNote> Modal = new List<Modals.SIRNote>();
            var SIRNoteModal = dbContext.SIRNotes.Where(x => x.UniqueFormID == UniqueFormID).ToList();
            Modal = (List<Modals.SIRNote>)SIRNoteModal.Select(y => new Modals.SIRNote
            {
                NoteID = y.NoteID,
                SIRFormID = y.SIRFormID,
                Number = y.Number,
                Note = y.Note,
                UniqueFormID = y.UniqueFormID
            });

            return Modal;
        }

        public Modals.SIRModal SIRFormGetDeficiency(string id)
        {
            var UniqueFormID = new Guid(id);
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.SIRModal dbModal = new Modals.SIRModal();
            List<GIRDeficiencies> sirDefis = new List<GIRDeficiencies>(); // RDBJ 01/15/2022
            dbModal.SuperintendedInspectionReport = new Modals.SuperintendedInspectionReport();
            var Modal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
            if (Modal != null)
            {
                //SIRModel.SIRFormID = Modal.SIRFormID;
                //SIRModel.ShipID = Modal.ShipID;
                //SIRModel.ShipName = Modal.ShipName;
                //SIRModel.UniqueFormID = Modal.UniqueFormID;

                //dbModal.SuperintendedInspectionReport = Modal; //RDBJ 09/25/2021 Commented
                SetSIRFormData(Modal, ref dbModal); //RDBJ 09/25/2021

                var DefModal = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UniqueFormID && x.ReportType == "SI"
                && x.isDelete == 0  // RDBJ 01/15/2022
                ).ToList();

                // RDBJ 01/15/2022
                if (DefModal != null && DefModal.Count > 0)
                {
                    foreach (var def in DefModal)
                    {
                        GIRDeficiencies sirDef = new GIRDeficiencies();
                        sirDef.DeficienciesID = def.DeficienciesID;
                        sirDef.No = def.No;
                        sirDef.DateRaised = def.DateRaised;
                        sirDef.Deficiency = def.Deficiency;
                        sirDef.DateClosed = def.DateClosed;
                        sirDef.CreatedDate = def.CreatedDate;
                        sirDef.UpdatedDate = def.UpdatedDate;
                        sirDef.Ship = def.Ship;
                        sirDef.IsClose = def.IsClose;
                        sirDef.ReportType = def.ReportType;
                        sirDef.ItemNo = def.ItemNo;
                        sirDef.Section = def.Section;
                        sirDef.UniqueFormID = def.UniqueFormID;
                        sirDef.isDelete = def.isDelete;
                        sirDef.Priority = def.Priority == null ? 12 : def.Priority; //RDBJ 11/02/2021
                        sirDef.DeficienciesUniqueID = def.DeficienciesUniqueID; //RDBJ 11/02/2021

                        var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList(); //RDBJ 11/02/2021 Added DeficienciesUniqueID
                        if (defFiles != null && defFiles.Count > 0)
                        {
                            foreach (var defFile in defFiles)
                            {
                                Modals.GIRDeficienciesFile sirDefFile = new Modals.GIRDeficienciesFile();
                                sirDefFile.FileName = defFile.FileName;
                                //sirDefFile.StorePath = defFile.StorePath;
                                sirDefFile.DeficienciesID = defFile.DeficienciesID;
                                sirDefFile.GIRDeficienciesFileID = defFile.GIRDeficienciesFileID;
                                sirDefFile.DeficienciesFileUniqueID = defFile.DeficienciesFileUniqueID;    // JSL 06/04/2022
                                sirDefFile.DeficienciesUniqueID = defFile.DeficienciesUniqueID; //RDBJ 11/02/2021
                                sirDefFile.IsUpload = "true"; //RDBJ 11/02/2021
                                sirDef.GIRDeficienciesFile.Add(sirDefFile);
                            }
                        }

                        // RDBJ 02/18/2022
                        sirDef.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>();
                        var defInitialAction = dbContext.GIRDeficienciesInitialActions
                            .Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID)
                            .OrderBy(x => x.CreatedDate)
                            .FirstOrDefault();

                        if (defInitialAction != null)
                        {
                            Modals.GIRDeficienciesInitialActions sirDefInitialAction = new Modals.GIRDeficienciesInitialActions();
                            sirDefInitialAction.IniActUniqueID = defInitialAction.IniActUniqueID;
                            sirDefInitialAction.DeficienciesUniqueID = defInitialAction.DeficienciesUniqueID;
                            sirDefInitialAction.Description = defInitialAction.Description;

                            sirDef.GIRDeficienciesInitialActions.Add(sirDefInitialAction);
                        }
                        // End RDBJ 02/18/2022

                        sirDefis.Add(sirDef);
                    }
                }
                dbModal.GIRDeficiencies = sirDefis;
                // ENd RDBJ 01/15/2022

                //var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == DefModal.FirstOrDefault().DeficienciesID).ToList();
                /*
                if (DefModal.Count > 0)
                {
                    dbModal.GIRDeficiencies = DefModal.Select(y => new GIRDeficiencies()
                    {
                        No = y.No,
                        Section = y.Section,
                        ItemNo = y.ItemNo,
                        Deficiency = y.Deficiency,
                        DateClosed = y.DateClosed,
                        DateRaised = y.DateRaised,
                        DeficienciesID = y.DeficienciesID,

                        GIRDeficienciesFile = dbContext.GIRDeficienciesFiles
                        .Where(x => x.DeficienciesUniqueID == y.DeficienciesUniqueID) // RDBJ 01/15/2022 updated with UniqueID
                        .Select(x => new Modals.GIRDeficienciesFile()
                        {
                            FileName = x.FileName,
                            StorePath = x.StorePath,
                            DeficienciesID = x.DeficienciesID,
                            GIRDeficienciesFileID = x.GIRDeficienciesFileID
                        }).ToList()

                    }).ToList();
                }
                else
                {
                    dbModal.GIRDeficiencies = DefModal.Select(y => new GIRDeficiencies()
                    {
                        No = y.No,
                        Section = y.Section,
                        ItemNo = y.ItemNo,
                        Ship = y.Ship,
                        Deficiency = y.Deficiency,
                        DateClosed = y.DateClosed,
                        DateRaised = y.DateRaised,
                        DeficienciesID = y.DeficienciesID
                    }).ToList();
                }
                */
            }
            return dbModal;
        }
        public void SetSIRFormDataRefEntity(ref Entity.SuperintendedInspectionReport dbModal, Modals.SuperintendedInspectionReport Modal)
        {
            dbModal.FormVersion = Modal.FormVersion;
            dbModal.ShipID = Modal.ShipID;
            dbModal.ShipName = Modal.ShipName;
            dbModal.Date = Modal.Date;
            dbModal.Port = Modal.Port;
            dbModal.Master = Modal.Master;
            dbModal.Superintended = Modal.Superintended;
            dbModal.FormVersion = Modal.FormVersion;
            dbModal.Section1_1_Condition = Modal.Section1_1_Condition;
            dbModal.Section1_1_Comment = Modal.Section1_1_Comment;
            dbModal.Section1_2_Condition = Modal.Section1_2_Condition;
            dbModal.Section1_2_Comment = Modal.Section1_2_Comment;
            dbModal.Section1_3_Condition = Modal.Section1_3_Condition;
            dbModal.Section1_3_Comment = Modal.Section1_3_Comment;
            dbModal.Section1_4_Condition = Modal.Section1_4_Condition;
            dbModal.Section1_4_Comment = Modal.Section1_4_Comment;
            dbModal.Section1_5_Condition = Modal.Section1_5_Condition;
            dbModal.Section1_5_Comment = Modal.Section1_5_Comment;
            dbModal.Section1_6_Condition = Modal.Section1_6_Condition;
            dbModal.Section1_6_Comment = Modal.Section1_6_Comment;
            dbModal.Section1_7_Condition = Modal.Section1_7_Condition;
            dbModal.Section1_7_Comment = Modal.Section1_7_Comment;
            dbModal.Section1_8_Condition = Modal.Section1_8_Condition;
            dbModal.Section1_8_Comment = Modal.Section1_8_Comment;
            dbModal.Section1_9_Condition = Modal.Section1_9_Condition;
            dbModal.Section1_9_Comment = Modal.Section1_9_Comment;
            dbModal.Section1_10_Condition = Modal.Section1_10_Condition;
            dbModal.Section1_10_Comment = Modal.Section1_10_Comment;
            dbModal.Section1_11_Condition = Modal.Section1_11_Condition;
            dbModal.Section1_11_Comment = Modal.Section1_11_Comment;

            dbModal.Section2_1_Condition = Modal.Section2_1_Condition;
            dbModal.Section2_1_Comment = Modal.Section2_1_Comment;
            dbModal.Section2_2_Condition = Modal.Section2_2_Condition;
            dbModal.Section2_2_Comment = Modal.Section2_2_Comment;
            dbModal.Section2_3_Condition = Modal.Section2_3_Condition;
            dbModal.Section2_3_Comment = Modal.Section2_3_Comment;
            dbModal.Section2_4_Condition = Modal.Section2_4_Condition;
            dbModal.Section2_4_Comment = Modal.Section2_4_Comment;
            dbModal.Section2_5_Condition = Modal.Section2_5_Condition;
            dbModal.Section2_5_Comment = Modal.Section2_5_Comment;
            dbModal.Section2_6_Condition = Modal.Section2_6_Condition;
            dbModal.Section2_6_Comment = Modal.Section2_6_Comment;
            dbModal.Section2_7_Condition = Modal.Section2_7_Condition;
            dbModal.Section2_7_Comment = Modal.Section2_7_Comment;

            dbModal.Section3_1_Condition = Modal.Section3_1_Condition;
            dbModal.Section3_1_Comment = Modal.Section3_1_Comment;
            dbModal.Section3_2_Condition = Modal.Section3_2_Condition;
            dbModal.Section3_2_Comment = Modal.Section3_2_Comment;
            dbModal.Section3_3_Condition = Modal.Section3_3_Condition;
            dbModal.Section3_3_Comment = Modal.Section3_3_Comment;
            dbModal.Section3_4_Condition = Modal.Section3_4_Condition;
            dbModal.Section3_4_Comment = Modal.Section3_4_Comment;
            dbModal.Section3_5_Condition = Modal.Section3_5_Condition;
            dbModal.Section3_5_Comment = Modal.Section3_5_Comment;

            dbModal.Section4_1_Condition = Modal.Section4_1_Condition;
            dbModal.Section4_1_Comment = Modal.Section4_1_Comment;
            dbModal.Section4_2_Condition = Modal.Section4_2_Condition;
            dbModal.Section4_2_Comment = Modal.Section4_2_Comment;
            dbModal.Section4_3_Condition = Modal.Section4_3_Condition;
            dbModal.Section4_3_Comment = Modal.Section4_3_Comment;

            dbModal.Section5_1_Condition = Modal.Section5_1_Condition;
            dbModal.Section5_1_Comment = Modal.Section5_1_Comment;
            dbModal.Section5_6_Condition = Modal.Section5_6_Condition;
            dbModal.Section5_6_Comment = Modal.Section5_6_Comment;
            dbModal.Section5_8_Condition = Modal.Section5_8_Condition;
            dbModal.Section5_8_Comment = Modal.Section5_8_Comment;
            dbModal.Section5_9_Condition = Modal.Section5_9_Condition;
            dbModal.Section5_9_Comment = Modal.Section5_9_Comment;

            dbModal.Section6_1_Condition = Modal.Section6_1_Condition;
            dbModal.Section6_1_Comment = Modal.Section6_1_Comment;
            dbModal.Section6_2_Condition = Modal.Section6_2_Condition;
            dbModal.Section6_2_Comment = Modal.Section6_2_Comment;
            dbModal.Section6_3_Condition = Modal.Section6_3_Condition;
            dbModal.Section6_3_Comment = Modal.Section6_3_Comment;
            dbModal.Section6_4_Condition = Modal.Section6_4_Condition;
            dbModal.Section6_4_Comment = Modal.Section6_4_Comment;
            dbModal.Section6_5_Condition = Modal.Section6_5_Condition;
            dbModal.Section6_5_Comment = Modal.Section6_5_Comment;
            dbModal.Section6_6_Condition = Modal.Section6_6_Condition;
            dbModal.Section6_6_Comment = Modal.Section6_6_Comment;
            dbModal.Section6_7_Condition = Modal.Section6_7_Condition;
            dbModal.Section6_7_Comment = Modal.Section6_7_Comment;
            dbModal.Section6_8_Condition = Modal.Section6_8_Condition;
            dbModal.Section6_8_Comment = Modal.Section6_8_Comment;

            dbModal.Section7_1_Condition = Modal.Section7_1_Condition;
            dbModal.Section7_1_Comment = Modal.Section7_1_Comment;
            dbModal.Section7_2_Condition = Modal.Section7_2_Condition;
            dbModal.Section7_2_Comment = Modal.Section7_2_Comment;
            dbModal.Section7_3_Condition = Modal.Section7_3_Condition;
            dbModal.Section7_3_Comment = Modal.Section7_3_Comment;
            dbModal.Section7_4_Condition = Modal.Section7_4_Condition;
            dbModal.Section7_4_Comment = Modal.Section7_4_Comment;
            dbModal.Section7_5_Condition = Modal.Section7_5_Condition;
            dbModal.Section7_5_Comment = Modal.Section7_5_Comment;
            dbModal.Section7_6_Condition = Modal.Section7_6_Condition;
            dbModal.Section7_6_Comment = Modal.Section7_6_Comment;

            dbModal.Section8_1_Condition = Modal.Section8_1_Condition;
            dbModal.Section8_1_Comment = Modal.Section8_1_Comment;
            dbModal.Section8_2_Condition = Modal.Section8_2_Condition;
            dbModal.Section8_2_Comment = Modal.Section8_2_Comment;
            dbModal.Section8_3_Condition = Modal.Section8_3_Condition;
            dbModal.Section8_3_Comment = Modal.Section8_3_Comment;
            dbModal.Section8_4_Condition = Modal.Section8_4_Condition;
            dbModal.Section8_4_Comment = Modal.Section8_4_Comment;
            dbModal.Section8_5_Condition = Modal.Section8_5_Condition;
            dbModal.Section8_5_Comment = Modal.Section8_5_Comment;
            dbModal.Section8_6_Condition = Modal.Section8_6_Condition;
            dbModal.Section8_6_Comment = Modal.Section8_6_Comment;
            dbModal.Section8_7_Condition = Modal.Section8_7_Condition;
            dbModal.Section8_7_Comment = Modal.Section8_7_Comment;
            dbModal.Section8_8_Condition = Modal.Section8_8_Condition;
            dbModal.Section8_8_Comment = Modal.Section8_8_Comment;
            dbModal.Section8_9_Condition = Modal.Section8_9_Condition;
            dbModal.Section8_9_Comment = Modal.Section8_9_Comment;
            dbModal.Section8_10_Condition = Modal.Section8_10_Condition;
            dbModal.Section8_10_Comment = Modal.Section8_10_Comment;
            dbModal.Section8_11_Condition = Modal.Section8_11_Condition;
            dbModal.Section8_11_Comment = Modal.Section8_11_Comment;
            dbModal.Section8_12_Condition = Modal.Section8_12_Condition;
            dbModal.Section8_12_Comment = Modal.Section8_12_Comment;
            dbModal.Section8_13_Condition = Modal.Section8_13_Condition;
            dbModal.Section8_13_Comment = Modal.Section8_13_Comment;
            dbModal.Section8_14_Condition = Modal.Section8_14_Condition;
            dbModal.Section8_14_Comment = Modal.Section8_14_Comment;
            dbModal.Section8_15_Condition = Modal.Section8_15_Condition;
            dbModal.Section8_15_Comment = Modal.Section8_15_Comment;
            dbModal.Section8_16_Condition = Modal.Section8_16_Condition;
            dbModal.Section8_16_Comment = Modal.Section8_16_Comment;
            dbModal.Section8_17_Condition = Modal.Section8_17_Condition;
            dbModal.Section8_17_Comment = Modal.Section8_17_Comment;
            dbModal.Section8_18_Condition = Modal.Section8_18_Condition;
            dbModal.Section8_18_Comment = Modal.Section8_18_Comment;
            dbModal.Section8_19_Condition = Modal.Section8_19_Condition;
            dbModal.Section8_19_Comment = Modal.Section8_19_Comment;
            dbModal.Section8_20_Condition = Modal.Section8_20_Condition;
            dbModal.Section8_20_Comment = Modal.Section8_20_Comment;
            dbModal.Section8_21_Condition = Modal.Section8_21_Condition;
            dbModal.Section8_21_Comment = Modal.Section8_21_Comment;
            dbModal.Section8_22_Condition = Modal.Section8_22_Condition;
            dbModal.Section8_22_Comment = Modal.Section8_22_Comment;
            dbModal.Section8_23_Condition = Modal.Section8_23_Condition;
            dbModal.Section8_23_Comment = Modal.Section8_23_Comment;
            dbModal.Section8_24_Condition = Modal.Section8_24_Condition;
            dbModal.Section8_24_Comment = Modal.Section8_24_Comment;
            dbModal.Section8_25_Condition = Modal.Section8_25_Condition;
            dbModal.Section8_25_Comment = Modal.Section8_25_Comment;

            dbModal.Section9_1_Condition = Modal.Section9_1_Condition;
            dbModal.Section9_1_Comment = Modal.Section9_1_Comment;
            dbModal.Section9_2_Condition = Modal.Section9_2_Condition;
            dbModal.Section9_2_Comment = Modal.Section9_2_Comment;
            dbModal.Section9_3_Condition = Modal.Section9_3_Condition;
            dbModal.Section9_3_Comment = Modal.Section9_3_Comment;
            dbModal.Section9_4_Condition = Modal.Section9_4_Condition;
            dbModal.Section9_4_Comment = Modal.Section9_4_Comment;
            dbModal.Section9_5_Condition = Modal.Section9_5_Condition;
            dbModal.Section9_5_Comment = Modal.Section9_5_Comment;
            dbModal.Section9_6_Condition = Modal.Section9_6_Condition;
            dbModal.Section9_6_Comment = Modal.Section9_6_Comment;
            dbModal.Section9_7_Condition = Modal.Section9_7_Condition;
            dbModal.Section9_7_Comment = Modal.Section9_7_Comment;
            dbModal.Section9_8_Condition = Modal.Section9_8_Condition;
            dbModal.Section9_8_Comment = Modal.Section9_8_Comment;
            dbModal.Section9_9_Condition = Modal.Section9_9_Condition;
            dbModal.Section9_9_Comment = Modal.Section9_9_Comment;
            dbModal.Section9_10_Condition = Modal.Section9_10_Condition;
            dbModal.Section9_10_Comment = Modal.Section9_10_Comment;
            dbModal.Section9_11_Condition = Modal.Section9_11_Condition;
            dbModal.Section9_11_Comment = Modal.Section9_11_Comment;
            dbModal.Section9_12_Condition = Modal.Section9_12_Condition;
            dbModal.Section9_12_Comment = Modal.Section9_12_Comment;
            dbModal.Section9_13_Condition = Modal.Section9_13_Condition;
            dbModal.Section9_13_Comment = Modal.Section9_13_Comment;
            dbModal.Section9_14_Condition = Modal.Section9_14_Condition;
            dbModal.Section9_14_Comment = Modal.Section9_14_Comment;
            dbModal.Section9_15_Condition = Modal.Section9_15_Condition;
            dbModal.Section9_15_Comment = Modal.Section9_15_Comment;

            // RDBJ 02/15/2022
            dbModal.Section9_16_Condition = Modal.Section9_16_Condition;
            dbModal.Section9_16_Comment = Modal.Section9_16_Comment;
            dbModal.Section9_17_Condition = Modal.Section9_17_Condition;
            dbModal.Section9_17_Comment = Modal.Section9_17_Comment;
            // End RDBJ 02/15/2022

            dbModal.Section10_1_Condition = Modal.Section10_1_Condition;
            dbModal.Section10_1_Comment = Modal.Section10_1_Comment;
            dbModal.Section10_2_Condition = Modal.Section10_2_Condition;
            dbModal.Section10_2_Comment = Modal.Section10_2_Comment;
            dbModal.Section10_3_Condition = Modal.Section10_3_Condition;
            dbModal.Section10_3_Comment = Modal.Section10_3_Comment;
            dbModal.Section10_4_Condition = Modal.Section10_4_Condition;
            dbModal.Section10_4_Comment = Modal.Section10_4_Comment;
            dbModal.Section10_5_Condition = Modal.Section10_5_Condition;
            dbModal.Section10_5_Comment = Modal.Section10_5_Comment;
            dbModal.Section10_6_Condition = Modal.Section10_6_Condition;
            dbModal.Section10_6_Comment = Modal.Section10_6_Comment;
            dbModal.Section10_7_Condition = Modal.Section10_7_Condition;
            dbModal.Section10_7_Comment = Modal.Section10_7_Comment;
            dbModal.Section10_8_Condition = Modal.Section10_8_Condition;
            dbModal.Section10_8_Comment = Modal.Section10_8_Comment;
            dbModal.Section10_9_Condition = Modal.Section10_9_Condition;
            dbModal.Section10_9_Comment = Modal.Section10_9_Comment;
            dbModal.Section10_10_Condition = Modal.Section10_10_Condition;
            dbModal.Section10_10_Comment = Modal.Section10_10_Comment;
            dbModal.Section10_11_Condition = Modal.Section10_11_Condition;
            dbModal.Section10_11_Comment = Modal.Section10_11_Comment;
            dbModal.Section10_12_Condition = Modal.Section10_12_Condition;
            dbModal.Section10_12_Comment = Modal.Section10_12_Comment;
            dbModal.Section10_13_Condition = Modal.Section10_13_Condition;
            dbModal.Section10_13_Comment = Modal.Section10_13_Comment;
            dbModal.Section10_14_Condition = Modal.Section10_14_Condition;
            dbModal.Section10_14_Comment = Modal.Section10_14_Comment;
            dbModal.Section10_15_Condition = Modal.Section10_15_Condition;
            dbModal.Section10_15_Comment = Modal.Section10_15_Comment;
            dbModal.Section10_16_Condition = Modal.Section10_16_Condition;
            dbModal.Section10_16_Comment = Modal.Section10_16_Comment;

            dbModal.Section11_1_Condition = Modal.Section11_1_Condition;
            dbModal.Section11_1_Comment = Modal.Section11_1_Comment;
            dbModal.Section11_2_Condition = Modal.Section11_2_Condition;
            dbModal.Section11_2_Comment = Modal.Section11_2_Comment;
            dbModal.Section11_3_Condition = Modal.Section11_3_Condition;
            dbModal.Section11_3_Comment = Modal.Section11_3_Comment;
            dbModal.Section11_4_Condition = Modal.Section11_4_Condition;
            dbModal.Section11_4_Comment = Modal.Section11_4_Comment;
            dbModal.Section11_5_Condition = Modal.Section11_5_Condition;
            dbModal.Section11_5_Comment = Modal.Section11_5_Comment;
            dbModal.Section11_6_Condition = Modal.Section11_6_Condition;
            dbModal.Section11_6_Comment = Modal.Section11_6_Comment;
            dbModal.Section11_7_Condition = Modal.Section11_7_Condition;
            dbModal.Section11_7_Comment = Modal.Section11_7_Comment;
            dbModal.Section11_8_Condition = Modal.Section11_8_Condition;
            dbModal.Section11_8_Comment = Modal.Section11_8_Comment;

            dbModal.Section12_1_Condition = Modal.Section12_1_Condition;
            dbModal.Section12_1_Comment = Modal.Section12_1_Comment;
            dbModal.Section12_2_Condition = Modal.Section12_2_Condition;
            dbModal.Section12_2_Comment = Modal.Section12_2_Comment;
            dbModal.Section12_3_Condition = Modal.Section12_3_Condition;
            dbModal.Section12_3_Comment = Modal.Section12_3_Comment;
            dbModal.Section12_4_Condition = Modal.Section12_4_Condition;
            dbModal.Section12_4_Comment = Modal.Section12_4_Comment;
            dbModal.Section12_5_Condition = Modal.Section12_5_Condition;
            dbModal.Section12_5_Comment = Modal.Section12_5_Comment;
            dbModal.Section12_6_Condition = Modal.Section12_6_Condition;
            dbModal.Section12_6_Comment = Modal.Section12_6_Comment;

            dbModal.Section13_1_Condition = Modal.Section13_1_Condition;
            dbModal.Section13_1_Comment = Modal.Section13_1_Comment;
            dbModal.Section13_2_Condition = Modal.Section13_2_Condition;
            dbModal.Section13_2_Comment = Modal.Section13_2_Comment;
            dbModal.Section13_3_Condition = Modal.Section13_3_Condition;
            dbModal.Section13_3_Comment = Modal.Section13_3_Comment;
            dbModal.Section13_4_Condition = Modal.Section13_4_Condition;
            dbModal.Section13_4_Comment = Modal.Section13_4_Comment;

            dbModal.Section14_1_Condition = Modal.Section14_1_Condition;
            dbModal.Section14_1_Comment = Modal.Section14_1_Comment;
            dbModal.Section14_2_Condition = Modal.Section14_2_Condition;
            dbModal.Section14_2_Comment = Modal.Section14_2_Comment;
            dbModal.Section14_3_Condition = Modal.Section14_3_Condition;
            dbModal.Section14_3_Comment = Modal.Section14_3_Comment;
            dbModal.Section14_4_Condition = Modal.Section14_4_Condition;
            dbModal.Section14_4_Comment = Modal.Section14_4_Comment;
            dbModal.Section14_5_Condition = Modal.Section14_5_Condition;
            dbModal.Section14_5_Comment = Modal.Section14_5_Comment;
            dbModal.Section14_6_Condition = Modal.Section14_6_Condition;
            dbModal.Section14_6_Comment = Modal.Section14_6_Comment;
            dbModal.Section14_7_Condition = Modal.Section14_7_Condition;
            dbModal.Section14_7_Comment = Modal.Section14_7_Comment;
            dbModal.Section14_8_Condition = Modal.Section14_8_Condition;
            dbModal.Section14_8_Comment = Modal.Section14_8_Comment;
            dbModal.Section14_9_Condition = Modal.Section14_9_Condition;
            dbModal.Section14_9_Comment = Modal.Section14_9_Comment;
            dbModal.Section14_10_Condition = Modal.Section14_10_Condition;
            dbModal.Section14_10_Comment = Modal.Section14_10_Comment;
            dbModal.Section14_11_Condition = Modal.Section14_11_Condition;
            dbModal.Section14_11_Comment = Modal.Section14_11_Comment;
            dbModal.Section14_12_Condition = Modal.Section14_12_Condition;
            dbModal.Section14_12_Comment = Modal.Section14_12_Comment;
            dbModal.Section14_13_Condition = Modal.Section14_13_Condition;
            dbModal.Section14_13_Comment = Modal.Section14_13_Comment;
            dbModal.Section14_14_Condition = Modal.Section14_14_Condition;
            dbModal.Section14_14_Comment = Modal.Section14_14_Comment;
            dbModal.Section14_15_Condition = Modal.Section14_15_Condition;
            dbModal.Section14_15_Comment = Modal.Section14_15_Comment;
            dbModal.Section14_16_Condition = Modal.Section14_16_Condition;
            dbModal.Section14_16_Comment = Modal.Section14_16_Comment;
            dbModal.Section14_17_Condition = Modal.Section14_17_Condition;
            dbModal.Section14_17_Comment = Modal.Section14_17_Comment;
            dbModal.Section14_18_Condition = Modal.Section14_18_Condition;
            dbModal.Section14_18_Comment = Modal.Section14_18_Comment;
            dbModal.Section14_19_Condition = Modal.Section14_19_Condition;
            dbModal.Section14_19_Comment = Modal.Section14_19_Comment;
            dbModal.Section14_20_Condition = Modal.Section14_20_Condition;
            dbModal.Section14_20_Comment = Modal.Section14_20_Comment;
            dbModal.Section14_21_Condition = Modal.Section14_21_Condition;
            dbModal.Section14_21_Comment = Modal.Section14_21_Comment;
            dbModal.Section14_22_Condition = Modal.Section14_22_Condition;
            dbModal.Section14_22_Comment = Modal.Section14_22_Comment;
            dbModal.Section14_23_Condition = Modal.Section14_23_Condition;
            dbModal.Section14_23_Comment = Modal.Section14_23_Comment;
            dbModal.Section14_24_Condition = Modal.Section14_24_Condition;
            dbModal.Section14_24_Comment = Modal.Section14_24_Comment;
            dbModal.Section14_25_Condition = Modal.Section14_25_Condition;
            dbModal.Section14_25_Comment = Modal.Section14_25_Comment;

            dbModal.Section15_1_Condition = Modal.Section15_1_Condition;
            dbModal.Section15_1_Comment = Modal.Section15_1_Comment;
            dbModal.Section15_2_Condition = Modal.Section15_2_Condition;
            dbModal.Section15_2_Comment = Modal.Section15_2_Comment;
            dbModal.Section15_3_Condition = Modal.Section15_3_Condition;
            dbModal.Section15_3_Comment = Modal.Section15_3_Comment;
            dbModal.Section15_4_Condition = Modal.Section15_4_Condition;
            dbModal.Section15_4_Comment = Modal.Section15_4_Comment;
            dbModal.Section15_5_Condition = Modal.Section15_5_Condition;
            dbModal.Section15_5_Comment = Modal.Section15_5_Comment;
            dbModal.Section15_6_Condition = Modal.Section15_6_Condition;
            dbModal.Section15_6_Comment = Modal.Section15_6_Comment;
            dbModal.Section15_7_Condition = Modal.Section15_7_Condition;
            dbModal.Section15_7_Comment = Modal.Section15_7_Comment;
            dbModal.Section15_8_Condition = Modal.Section15_8_Condition;
            dbModal.Section15_8_Comment = Modal.Section15_8_Comment;
            dbModal.Section15_9_Condition = Modal.Section15_9_Condition;
            dbModal.Section15_9_Comment = Modal.Section15_9_Comment;
            dbModal.Section15_10_Condition = Modal.Section15_10_Condition;
            dbModal.Section15_10_Comment = Modal.Section15_10_Comment;
            dbModal.Section15_11_Condition = Modal.Section15_11_Condition;
            dbModal.Section15_11_Comment = Modal.Section15_11_Comment;
            dbModal.Section15_12_Condition = Modal.Section15_12_Condition;
            dbModal.Section15_12_Comment = Modal.Section15_12_Comment;
            dbModal.Section15_13_Condition = Modal.Section15_13_Condition;
            dbModal.Section15_13_Comment = Modal.Section15_13_Comment;
            dbModal.Section15_14_Condition = Modal.Section15_14_Condition;
            dbModal.Section15_14_Comment = Modal.Section15_14_Comment;
            dbModal.Section15_15_Condition = Modal.Section15_15_Condition;
            dbModal.Section15_15_Comment = Modal.Section15_15_Comment;

            dbModal.Section16_1_Condition = Modal.Section16_1_Condition;
            dbModal.Section16_1_Comment = Modal.Section16_1_Comment;
            dbModal.Section16_2_Condition = Modal.Section16_2_Condition;
            dbModal.Section16_2_Comment = Modal.Section16_2_Comment;
            dbModal.Section16_3_Condition = Modal.Section16_3_Condition;
            dbModal.Section16_3_Comment = Modal.Section16_3_Comment;
            dbModal.Section16_4_Condition = Modal.Section16_4_Condition;
            dbModal.Section16_4_Comment = Modal.Section16_4_Comment;

            dbModal.Section17_1_Condition = Modal.Section17_1_Condition;
            dbModal.Section17_1_Comment = Modal.Section17_1_Comment;
            dbModal.Section17_2_Condition = Modal.Section17_2_Condition;
            dbModal.Section17_2_Comment = Modal.Section17_2_Comment;
            dbModal.Section17_3_Condition = Modal.Section17_3_Condition;
            dbModal.Section17_3_Comment = Modal.Section17_3_Comment;
            dbModal.Section17_4_Condition = Modal.Section17_4_Condition;
            dbModal.Section17_4_Comment = Modal.Section17_4_Comment;
            dbModal.Section17_5_Condition = Modal.Section17_5_Condition;
            dbModal.Section17_5_Comment = Modal.Section17_5_Comment;
            dbModal.Section17_6_Condition = Modal.Section17_6_Condition;
            dbModal.Section17_6_Comment = Modal.Section17_6_Comment;

            dbModal.Section18_1_Condition = Modal.Section18_1_Condition;
            dbModal.Section18_1_Comment = Modal.Section18_1_Comment;
            dbModal.Section18_2_Condition = Modal.Section18_2_Condition;
            dbModal.Section18_2_Comment = Modal.Section18_2_Comment;
            dbModal.Section18_3_Condition = Modal.Section18_3_Condition;
            dbModal.Section18_3_Comment = Modal.Section18_3_Comment;
            dbModal.Section18_4_Condition = Modal.Section18_4_Condition;
            dbModal.Section18_4_Comment = Modal.Section18_4_Comment;
            dbModal.Section18_5_Condition = Modal.Section18_5_Condition;
            dbModal.Section18_5_Comment = Modal.Section18_5_Comment;
            dbModal.Section18_6_Condition = Modal.Section18_6_Condition;
            dbModal.Section18_6_Comment = Modal.Section18_6_Comment;
            dbModal.Section18_7_Condition = Modal.Section18_7_Condition;
            dbModal.Section18_7_Comment = Modal.Section18_7_Comment;

            // RDBJ 02/15/2022
            dbModal.Section18_8_Condition = Modal.Section18_8_Condition;
            dbModal.Section18_8_Comment = Modal.Section18_8_Comment;
            dbModal.Section18_9_Condition = Modal.Section18_9_Condition;
            dbModal.Section18_9_Comment = Modal.Section18_9_Comment;
            // End RDBJ 02/15/2022

            dbModal.IsSynced = Modal.IsSynced;
            //dbModal.CreatedDate = Modal.CreatedDate; //RDBJ 09/24/2021 Commented 
            dbModal.ModifyDate = Modal.ModifyDate;
            dbModal.SavedAsDraft = Modal.SavedAsDraft;

        }
        public void SetSIRFormData(Entity.SuperintendedInspectionReport Modal, ref SIRModal dbModal)
        {
            dbModal.SuperintendedInspectionReport.UniqueFormID = Modal.UniqueFormID;
            dbModal.SuperintendedInspectionReport.ShipID = Modal.ShipID == null ? 0 : Modal.ShipID;
            dbModal.SuperintendedInspectionReport.ShipName = Modal.ShipName;
            dbModal.SuperintendedInspectionReport.Date = Modal.Date;
            dbModal.SuperintendedInspectionReport.Port = Modal.Port;
            dbModal.SuperintendedInspectionReport.Master = Modal.Master;
            dbModal.SuperintendedInspectionReport.Superintended = Modal.Superintended;
            dbModal.SuperintendedInspectionReport.FormVersion = Modal.FormVersion;
            dbModal.SuperintendedInspectionReport.Section1_1_Condition = Modal.Section1_1_Condition;
            dbModal.SuperintendedInspectionReport.Section1_1_Comment = Modal.Section1_1_Comment;
            dbModal.SuperintendedInspectionReport.Section1_2_Condition = Modal.Section1_2_Condition;
            dbModal.SuperintendedInspectionReport.Section1_2_Comment = Modal.Section1_2_Comment;
            dbModal.SuperintendedInspectionReport.Section1_3_Condition = Modal.Section1_3_Condition;
            dbModal.SuperintendedInspectionReport.Section1_3_Comment = Modal.Section1_3_Comment;
            dbModal.SuperintendedInspectionReport.Section1_4_Condition = Modal.Section1_4_Condition;
            dbModal.SuperintendedInspectionReport.Section1_4_Comment = Modal.Section1_4_Comment;
            dbModal.SuperintendedInspectionReport.Section1_5_Condition = Modal.Section1_5_Condition;
            dbModal.SuperintendedInspectionReport.Section1_5_Comment = Modal.Section1_5_Comment;
            dbModal.SuperintendedInspectionReport.Section1_6_Condition = Modal.Section1_6_Condition;
            dbModal.SuperintendedInspectionReport.Section1_6_Comment = Modal.Section1_6_Comment;
            dbModal.SuperintendedInspectionReport.Section1_7_Condition = Modal.Section1_7_Condition;
            dbModal.SuperintendedInspectionReport.Section1_7_Comment = Modal.Section1_7_Comment;
            dbModal.SuperintendedInspectionReport.Section1_8_Condition = Modal.Section1_8_Condition;
            dbModal.SuperintendedInspectionReport.Section1_8_Comment = Modal.Section1_8_Comment;
            dbModal.SuperintendedInspectionReport.Section1_9_Condition = Modal.Section1_9_Condition;
            dbModal.SuperintendedInspectionReport.Section1_9_Comment = Modal.Section1_9_Comment;
            dbModal.SuperintendedInspectionReport.Section1_10_Condition = Modal.Section1_10_Condition;
            dbModal.SuperintendedInspectionReport.Section1_10_Comment = Modal.Section1_10_Comment;
            dbModal.SuperintendedInspectionReport.Section1_11_Condition = Modal.Section1_11_Condition;
            dbModal.SuperintendedInspectionReport.Section1_11_Comment = Modal.Section1_11_Comment;

            dbModal.SuperintendedInspectionReport.Section2_1_Condition = Modal.Section2_1_Condition;
            dbModal.SuperintendedInspectionReport.Section2_1_Comment = Modal.Section2_1_Comment;
            dbModal.SuperintendedInspectionReport.Section2_2_Condition = Modal.Section2_2_Condition;
            dbModal.SuperintendedInspectionReport.Section2_2_Comment = Modal.Section2_2_Comment;
            dbModal.SuperintendedInspectionReport.Section2_3_Condition = Modal.Section2_3_Condition;
            dbModal.SuperintendedInspectionReport.Section2_3_Comment = Modal.Section2_3_Comment;
            dbModal.SuperintendedInspectionReport.Section2_4_Condition = Modal.Section2_4_Condition;
            dbModal.SuperintendedInspectionReport.Section2_4_Comment = Modal.Section2_4_Comment;
            dbModal.SuperintendedInspectionReport.Section2_5_Condition = Modal.Section2_5_Condition;
            dbModal.SuperintendedInspectionReport.Section2_5_Comment = Modal.Section2_5_Comment;
            dbModal.SuperintendedInspectionReport.Section2_6_Condition = Modal.Section2_6_Condition;
            dbModal.SuperintendedInspectionReport.Section2_6_Comment = Modal.Section2_6_Comment;
            dbModal.SuperintendedInspectionReport.Section2_7_Condition = Modal.Section2_7_Condition;
            dbModal.SuperintendedInspectionReport.Section2_7_Comment = Modal.Section2_7_Comment;

            dbModal.SuperintendedInspectionReport.Section3_1_Condition = Modal.Section3_1_Condition;
            dbModal.SuperintendedInspectionReport.Section3_1_Comment = Modal.Section3_1_Comment;
            dbModal.SuperintendedInspectionReport.Section3_2_Condition = Modal.Section3_2_Condition;
            dbModal.SuperintendedInspectionReport.Section3_2_Comment = Modal.Section3_2_Comment;
            dbModal.SuperintendedInspectionReport.Section3_3_Condition = Modal.Section3_3_Condition;
            dbModal.SuperintendedInspectionReport.Section3_3_Comment = Modal.Section3_3_Comment;
            dbModal.SuperintendedInspectionReport.Section3_4_Condition = Modal.Section3_4_Condition;
            dbModal.SuperintendedInspectionReport.Section3_4_Comment = Modal.Section3_4_Comment;
            dbModal.SuperintendedInspectionReport.Section3_5_Condition = Modal.Section3_5_Condition;
            dbModal.SuperintendedInspectionReport.Section3_5_Comment = Modal.Section3_5_Comment;

            dbModal.SuperintendedInspectionReport.Section4_1_Condition = Modal.Section4_1_Condition;
            dbModal.SuperintendedInspectionReport.Section4_1_Comment = Modal.Section4_1_Comment;
            dbModal.SuperintendedInspectionReport.Section4_2_Condition = Modal.Section4_2_Condition;
            dbModal.SuperintendedInspectionReport.Section4_2_Comment = Modal.Section4_2_Comment;
            dbModal.SuperintendedInspectionReport.Section4_3_Condition = Modal.Section4_3_Condition;
            dbModal.SuperintendedInspectionReport.Section4_3_Comment = Modal.Section4_3_Comment;

            dbModal.SuperintendedInspectionReport.Section5_1_Condition = Modal.Section5_1_Condition;
            dbModal.SuperintendedInspectionReport.Section5_1_Comment = Modal.Section5_1_Comment;
            dbModal.SuperintendedInspectionReport.Section5_6_Condition = Modal.Section5_6_Condition;
            dbModal.SuperintendedInspectionReport.Section5_6_Comment = Modal.Section5_6_Comment;
            dbModal.SuperintendedInspectionReport.Section5_8_Condition = Modal.Section5_8_Condition;
            dbModal.SuperintendedInspectionReport.Section5_8_Comment = Modal.Section5_8_Comment;
            dbModal.SuperintendedInspectionReport.Section5_9_Condition = Modal.Section5_9_Condition;
            dbModal.SuperintendedInspectionReport.Section5_9_Comment = Modal.Section5_9_Comment;

            dbModal.SuperintendedInspectionReport.Section6_1_Condition = Modal.Section6_1_Condition;
            dbModal.SuperintendedInspectionReport.Section6_1_Comment = Modal.Section6_1_Comment;
            dbModal.SuperintendedInspectionReport.Section6_2_Condition = Modal.Section6_2_Condition;
            dbModal.SuperintendedInspectionReport.Section6_2_Comment = Modal.Section6_2_Comment;
            dbModal.SuperintendedInspectionReport.Section6_3_Condition = Modal.Section6_3_Condition;
            dbModal.SuperintendedInspectionReport.Section6_3_Comment = Modal.Section6_3_Comment;
            dbModal.SuperintendedInspectionReport.Section6_4_Condition = Modal.Section6_4_Condition;
            dbModal.SuperintendedInspectionReport.Section6_4_Comment = Modal.Section6_4_Comment;
            dbModal.SuperintendedInspectionReport.Section6_5_Condition = Modal.Section6_5_Condition;
            dbModal.SuperintendedInspectionReport.Section6_5_Comment = Modal.Section6_5_Comment;
            dbModal.SuperintendedInspectionReport.Section6_6_Condition = Modal.Section6_6_Condition;
            dbModal.SuperintendedInspectionReport.Section6_6_Comment = Modal.Section6_6_Comment;
            dbModal.SuperintendedInspectionReport.Section6_7_Condition = Modal.Section6_7_Condition;
            dbModal.SuperintendedInspectionReport.Section6_7_Comment = Modal.Section6_7_Comment;
            dbModal.SuperintendedInspectionReport.Section6_8_Condition = Modal.Section6_8_Condition;
            dbModal.SuperintendedInspectionReport.Section6_8_Comment = Modal.Section6_8_Comment;

            dbModal.SuperintendedInspectionReport.Section7_1_Condition = Modal.Section7_1_Condition;
            dbModal.SuperintendedInspectionReport.Section7_1_Comment = Modal.Section7_1_Comment;
            dbModal.SuperintendedInspectionReport.Section7_2_Condition = Modal.Section7_2_Condition;
            dbModal.SuperintendedInspectionReport.Section7_2_Comment = Modal.Section7_2_Comment;
            dbModal.SuperintendedInspectionReport.Section7_3_Condition = Modal.Section7_3_Condition;
            dbModal.SuperintendedInspectionReport.Section7_3_Comment = Modal.Section7_3_Comment;
            dbModal.SuperintendedInspectionReport.Section7_4_Condition = Modal.Section7_4_Condition;
            dbModal.SuperintendedInspectionReport.Section7_4_Comment = Modal.Section7_4_Comment;
            dbModal.SuperintendedInspectionReport.Section7_5_Condition = Modal.Section7_5_Condition;
            dbModal.SuperintendedInspectionReport.Section7_5_Comment = Modal.Section7_5_Comment;
            dbModal.SuperintendedInspectionReport.Section7_6_Condition = Modal.Section7_6_Condition;
            dbModal.SuperintendedInspectionReport.Section7_6_Comment = Modal.Section7_6_Comment;

            dbModal.SuperintendedInspectionReport.Section8_1_Condition = Modal.Section8_1_Condition;
            dbModal.SuperintendedInspectionReport.Section8_1_Comment = Modal.Section8_1_Comment;
            dbModal.SuperintendedInspectionReport.Section8_2_Condition = Modal.Section8_2_Condition;
            dbModal.SuperintendedInspectionReport.Section8_2_Comment = Modal.Section8_2_Comment;
            dbModal.SuperintendedInspectionReport.Section8_3_Condition = Modal.Section8_3_Condition;
            dbModal.SuperintendedInspectionReport.Section8_3_Comment = Modal.Section8_3_Comment;
            dbModal.SuperintendedInspectionReport.Section8_4_Condition = Modal.Section8_4_Condition;
            dbModal.SuperintendedInspectionReport.Section8_4_Comment = Modal.Section8_4_Comment;
            dbModal.SuperintendedInspectionReport.Section8_5_Condition = Modal.Section8_5_Condition;
            dbModal.SuperintendedInspectionReport.Section8_5_Comment = Modal.Section8_5_Comment;
            dbModal.SuperintendedInspectionReport.Section8_6_Condition = Modal.Section8_6_Condition;
            dbModal.SuperintendedInspectionReport.Section8_6_Comment = Modal.Section8_6_Comment;
            dbModal.SuperintendedInspectionReport.Section8_7_Condition = Modal.Section8_7_Condition;
            dbModal.SuperintendedInspectionReport.Section8_7_Comment = Modal.Section8_7_Comment;
            dbModal.SuperintendedInspectionReport.Section8_8_Condition = Modal.Section8_8_Condition;
            dbModal.SuperintendedInspectionReport.Section8_8_Comment = Modal.Section8_8_Comment;
            dbModal.SuperintendedInspectionReport.Section8_9_Condition = Modal.Section8_9_Condition;
            dbModal.SuperintendedInspectionReport.Section8_9_Comment = Modal.Section8_9_Comment;
            dbModal.SuperintendedInspectionReport.Section8_10_Condition = Modal.Section8_10_Condition;
            dbModal.SuperintendedInspectionReport.Section8_10_Comment = Modal.Section8_10_Comment;
            dbModal.SuperintendedInspectionReport.Section8_11_Condition = Modal.Section8_11_Condition;
            dbModal.SuperintendedInspectionReport.Section8_11_Comment = Modal.Section8_11_Comment;
            dbModal.SuperintendedInspectionReport.Section8_12_Condition = Modal.Section8_12_Condition;
            dbModal.SuperintendedInspectionReport.Section8_12_Comment = Modal.Section8_12_Comment;
            dbModal.SuperintendedInspectionReport.Section8_13_Condition = Modal.Section8_13_Condition;
            dbModal.SuperintendedInspectionReport.Section8_13_Comment = Modal.Section8_13_Comment;
            dbModal.SuperintendedInspectionReport.Section8_14_Condition = Modal.Section8_14_Condition;
            dbModal.SuperintendedInspectionReport.Section8_14_Comment = Modal.Section8_14_Comment;
            dbModal.SuperintendedInspectionReport.Section8_15_Condition = Modal.Section8_15_Condition;
            dbModal.SuperintendedInspectionReport.Section8_15_Comment = Modal.Section8_15_Comment;
            dbModal.SuperintendedInspectionReport.Section8_16_Condition = Modal.Section8_16_Condition;
            dbModal.SuperintendedInspectionReport.Section8_16_Comment = Modal.Section8_16_Comment;
            dbModal.SuperintendedInspectionReport.Section8_17_Condition = Modal.Section8_17_Condition;
            dbModal.SuperintendedInspectionReport.Section8_17_Comment = Modal.Section8_17_Comment;
            dbModal.SuperintendedInspectionReport.Section8_18_Condition = Modal.Section8_18_Condition;
            dbModal.SuperintendedInspectionReport.Section8_18_Comment = Modal.Section8_18_Comment;
            dbModal.SuperintendedInspectionReport.Section8_19_Condition = Modal.Section8_19_Condition;
            dbModal.SuperintendedInspectionReport.Section8_19_Comment = Modal.Section8_19_Comment;
            dbModal.SuperintendedInspectionReport.Section8_20_Condition = Modal.Section8_20_Condition;
            dbModal.SuperintendedInspectionReport.Section8_20_Comment = Modal.Section8_20_Comment;
            dbModal.SuperintendedInspectionReport.Section8_21_Condition = Modal.Section8_21_Condition;
            dbModal.SuperintendedInspectionReport.Section8_21_Comment = Modal.Section8_21_Comment;
            dbModal.SuperintendedInspectionReport.Section8_22_Condition = Modal.Section8_22_Condition;
            dbModal.SuperintendedInspectionReport.Section8_22_Comment = Modal.Section8_22_Comment;
            dbModal.SuperintendedInspectionReport.Section8_23_Condition = Modal.Section8_23_Condition;
            dbModal.SuperintendedInspectionReport.Section8_23_Comment = Modal.Section8_23_Comment;
            dbModal.SuperintendedInspectionReport.Section8_24_Condition = Modal.Section8_24_Condition;
            dbModal.SuperintendedInspectionReport.Section8_24_Comment = Modal.Section8_24_Comment;
            dbModal.SuperintendedInspectionReport.Section8_25_Condition = Modal.Section8_25_Condition;
            dbModal.SuperintendedInspectionReport.Section8_25_Comment = Modal.Section8_25_Comment;

            dbModal.SuperintendedInspectionReport.Section9_1_Condition = Modal.Section9_1_Condition;
            dbModal.SuperintendedInspectionReport.Section9_1_Comment = Modal.Section9_1_Comment;
            dbModal.SuperintendedInspectionReport.Section9_2_Condition = Modal.Section9_2_Condition;
            dbModal.SuperintendedInspectionReport.Section9_2_Comment = Modal.Section9_2_Comment;
            dbModal.SuperintendedInspectionReport.Section9_3_Condition = Modal.Section9_3_Condition;
            dbModal.SuperintendedInspectionReport.Section9_3_Comment = Modal.Section9_3_Comment;
            dbModal.SuperintendedInspectionReport.Section9_4_Condition = Modal.Section9_4_Condition;
            dbModal.SuperintendedInspectionReport.Section9_4_Comment = Modal.Section9_4_Comment;
            dbModal.SuperintendedInspectionReport.Section9_5_Condition = Modal.Section9_5_Condition;
            dbModal.SuperintendedInspectionReport.Section9_5_Comment = Modal.Section9_5_Comment;
            dbModal.SuperintendedInspectionReport.Section9_6_Condition = Modal.Section9_6_Condition;
            dbModal.SuperintendedInspectionReport.Section9_6_Comment = Modal.Section9_6_Comment;
            dbModal.SuperintendedInspectionReport.Section9_7_Condition = Modal.Section9_7_Condition;
            dbModal.SuperintendedInspectionReport.Section9_7_Comment = Modal.Section9_7_Comment;
            dbModal.SuperintendedInspectionReport.Section9_8_Condition = Modal.Section9_8_Condition;
            dbModal.SuperintendedInspectionReport.Section9_8_Comment = Modal.Section9_8_Comment;
            dbModal.SuperintendedInspectionReport.Section9_9_Condition = Modal.Section9_9_Condition;
            dbModal.SuperintendedInspectionReport.Section9_9_Comment = Modal.Section9_9_Comment;
            dbModal.SuperintendedInspectionReport.Section9_10_Condition = Modal.Section9_10_Condition;
            dbModal.SuperintendedInspectionReport.Section9_10_Comment = Modal.Section9_10_Comment;
            dbModal.SuperintendedInspectionReport.Section9_11_Condition = Modal.Section9_11_Condition;
            dbModal.SuperintendedInspectionReport.Section9_11_Comment = Modal.Section9_11_Comment;
            dbModal.SuperintendedInspectionReport.Section9_12_Condition = Modal.Section9_12_Condition;
            dbModal.SuperintendedInspectionReport.Section9_12_Comment = Modal.Section9_12_Comment;
            dbModal.SuperintendedInspectionReport.Section9_13_Condition = Modal.Section9_13_Condition;
            dbModal.SuperintendedInspectionReport.Section9_13_Comment = Modal.Section9_13_Comment;
            dbModal.SuperintendedInspectionReport.Section9_14_Condition = Modal.Section9_14_Condition;
            dbModal.SuperintendedInspectionReport.Section9_14_Comment = Modal.Section9_14_Comment;
            dbModal.SuperintendedInspectionReport.Section9_15_Condition = Modal.Section9_15_Condition;
            dbModal.SuperintendedInspectionReport.Section9_15_Comment = Modal.Section9_15_Comment;

            // RDBJ 02/15/2022
            dbModal.SuperintendedInspectionReport.Section9_16_Condition = Modal.Section9_16_Condition;
            dbModal.SuperintendedInspectionReport.Section9_16_Comment = Modal.Section9_16_Comment;
            dbModal.SuperintendedInspectionReport.Section9_17_Condition = Modal.Section9_17_Condition;
            dbModal.SuperintendedInspectionReport.Section9_17_Comment = Modal.Section9_17_Comment;
            // End RDBJ 02/15/2022

            dbModal.SuperintendedInspectionReport.Section10_1_Condition = Modal.Section10_1_Condition;
            dbModal.SuperintendedInspectionReport.Section10_1_Comment = Modal.Section10_1_Comment;
            dbModal.SuperintendedInspectionReport.Section10_2_Condition = Modal.Section10_2_Condition;
            dbModal.SuperintendedInspectionReport.Section10_2_Comment = Modal.Section10_2_Comment;
            dbModal.SuperintendedInspectionReport.Section10_3_Condition = Modal.Section10_3_Condition;
            dbModal.SuperintendedInspectionReport.Section10_3_Comment = Modal.Section10_3_Comment;
            dbModal.SuperintendedInspectionReport.Section10_4_Condition = Modal.Section10_4_Condition;
            dbModal.SuperintendedInspectionReport.Section10_4_Comment = Modal.Section10_4_Comment;
            dbModal.SuperintendedInspectionReport.Section10_5_Condition = Modal.Section10_5_Condition;
            dbModal.SuperintendedInspectionReport.Section10_5_Comment = Modal.Section10_5_Comment;
            dbModal.SuperintendedInspectionReport.Section10_6_Condition = Modal.Section10_6_Condition;
            dbModal.SuperintendedInspectionReport.Section10_6_Comment = Modal.Section10_6_Comment;
            dbModal.SuperintendedInspectionReport.Section10_7_Condition = Modal.Section10_7_Condition;
            dbModal.SuperintendedInspectionReport.Section10_7_Comment = Modal.Section10_7_Comment;
            dbModal.SuperintendedInspectionReport.Section10_8_Condition = Modal.Section10_8_Condition;
            dbModal.SuperintendedInspectionReport.Section10_8_Comment = Modal.Section10_8_Comment;
            dbModal.SuperintendedInspectionReport.Section10_9_Condition = Modal.Section10_9_Condition;
            dbModal.SuperintendedInspectionReport.Section10_9_Comment = Modal.Section10_9_Comment;
            dbModal.SuperintendedInspectionReport.Section10_10_Condition = Modal.Section10_10_Condition;
            dbModal.SuperintendedInspectionReport.Section10_10_Comment = Modal.Section10_10_Comment;
            dbModal.SuperintendedInspectionReport.Section10_11_Condition = Modal.Section10_11_Condition;
            dbModal.SuperintendedInspectionReport.Section10_11_Comment = Modal.Section10_11_Comment;
            dbModal.SuperintendedInspectionReport.Section10_12_Condition = Modal.Section10_12_Condition;
            dbModal.SuperintendedInspectionReport.Section10_12_Comment = Modal.Section10_12_Comment;
            dbModal.SuperintendedInspectionReport.Section10_13_Condition = Modal.Section10_13_Condition;
            dbModal.SuperintendedInspectionReport.Section10_13_Comment = Modal.Section10_13_Comment;
            dbModal.SuperintendedInspectionReport.Section10_14_Condition = Modal.Section10_14_Condition;
            dbModal.SuperintendedInspectionReport.Section10_14_Comment = Modal.Section10_14_Comment;
            dbModal.SuperintendedInspectionReport.Section10_15_Condition = Modal.Section10_15_Condition;
            dbModal.SuperintendedInspectionReport.Section10_15_Comment = Modal.Section10_15_Comment;
            dbModal.SuperintendedInspectionReport.Section10_16_Condition = Modal.Section10_16_Condition;
            dbModal.SuperintendedInspectionReport.Section10_16_Comment = Modal.Section10_16_Comment;

            dbModal.SuperintendedInspectionReport.Section11_1_Condition = Modal.Section11_1_Condition;
            dbModal.SuperintendedInspectionReport.Section11_1_Comment = Modal.Section11_1_Comment;
            dbModal.SuperintendedInspectionReport.Section11_2_Condition = Modal.Section11_2_Condition;
            dbModal.SuperintendedInspectionReport.Section11_2_Comment = Modal.Section11_2_Comment;
            dbModal.SuperintendedInspectionReport.Section11_3_Condition = Modal.Section11_3_Condition;
            dbModal.SuperintendedInspectionReport.Section11_3_Comment = Modal.Section11_3_Comment;
            dbModal.SuperintendedInspectionReport.Section11_4_Condition = Modal.Section11_4_Condition;
            dbModal.SuperintendedInspectionReport.Section11_4_Comment = Modal.Section11_4_Comment;
            dbModal.SuperintendedInspectionReport.Section11_5_Condition = Modal.Section11_5_Condition;
            dbModal.SuperintendedInspectionReport.Section11_5_Comment = Modal.Section11_5_Comment;
            dbModal.SuperintendedInspectionReport.Section11_6_Condition = Modal.Section11_6_Condition;
            dbModal.SuperintendedInspectionReport.Section11_6_Comment = Modal.Section11_6_Comment;
            dbModal.SuperintendedInspectionReport.Section11_7_Condition = Modal.Section11_7_Condition;
            dbModal.SuperintendedInspectionReport.Section11_7_Comment = Modal.Section11_7_Comment;
            dbModal.SuperintendedInspectionReport.Section11_8_Condition = Modal.Section11_8_Condition;
            dbModal.SuperintendedInspectionReport.Section11_8_Comment = Modal.Section11_8_Comment;

            dbModal.SuperintendedInspectionReport.Section12_1_Condition = Modal.Section12_1_Condition;
            dbModal.SuperintendedInspectionReport.Section12_1_Comment = Modal.Section12_1_Comment;
            dbModal.SuperintendedInspectionReport.Section12_2_Condition = Modal.Section12_2_Condition;
            dbModal.SuperintendedInspectionReport.Section12_2_Comment = Modal.Section12_2_Comment;
            dbModal.SuperintendedInspectionReport.Section12_3_Condition = Modal.Section12_3_Condition;
            dbModal.SuperintendedInspectionReport.Section12_3_Comment = Modal.Section12_3_Comment;
            dbModal.SuperintendedInspectionReport.Section12_4_Condition = Modal.Section12_4_Condition;
            dbModal.SuperintendedInspectionReport.Section12_4_Comment = Modal.Section12_4_Comment;
            dbModal.SuperintendedInspectionReport.Section12_5_Condition = Modal.Section12_5_Condition;
            dbModal.SuperintendedInspectionReport.Section12_5_Comment = Modal.Section12_5_Comment;
            dbModal.SuperintendedInspectionReport.Section12_6_Condition = Modal.Section12_6_Condition;
            dbModal.SuperintendedInspectionReport.Section12_6_Comment = Modal.Section12_6_Comment;

            dbModal.SuperintendedInspectionReport.Section13_1_Condition = Modal.Section13_1_Condition;
            dbModal.SuperintendedInspectionReport.Section13_1_Comment = Modal.Section13_1_Comment;
            dbModal.SuperintendedInspectionReport.Section13_2_Condition = Modal.Section13_2_Condition;
            dbModal.SuperintendedInspectionReport.Section13_2_Comment = Modal.Section13_2_Comment;
            dbModal.SuperintendedInspectionReport.Section13_3_Condition = Modal.Section13_3_Condition;
            dbModal.SuperintendedInspectionReport.Section13_3_Comment = Modal.Section13_3_Comment;
            dbModal.SuperintendedInspectionReport.Section13_4_Condition = Modal.Section13_4_Condition;
            dbModal.SuperintendedInspectionReport.Section13_4_Comment = Modal.Section13_4_Comment;

            dbModal.SuperintendedInspectionReport.Section14_1_Condition = Modal.Section14_1_Condition;
            dbModal.SuperintendedInspectionReport.Section14_1_Comment = Modal.Section14_1_Comment;
            dbModal.SuperintendedInspectionReport.Section14_2_Condition = Modal.Section14_2_Condition;
            dbModal.SuperintendedInspectionReport.Section14_2_Comment = Modal.Section14_2_Comment;
            dbModal.SuperintendedInspectionReport.Section14_3_Condition = Modal.Section14_3_Condition;
            dbModal.SuperintendedInspectionReport.Section14_3_Comment = Modal.Section14_3_Comment;
            dbModal.SuperintendedInspectionReport.Section14_4_Condition = Modal.Section14_4_Condition;
            dbModal.SuperintendedInspectionReport.Section14_4_Comment = Modal.Section14_4_Comment;
            dbModal.SuperintendedInspectionReport.Section14_5_Condition = Modal.Section14_5_Condition;
            dbModal.SuperintendedInspectionReport.Section14_5_Comment = Modal.Section14_5_Comment;
            dbModal.SuperintendedInspectionReport.Section14_6_Condition = Modal.Section14_6_Condition;
            dbModal.SuperintendedInspectionReport.Section14_6_Comment = Modal.Section14_6_Comment;
            dbModal.SuperintendedInspectionReport.Section14_7_Condition = Modal.Section14_7_Condition;
            dbModal.SuperintendedInspectionReport.Section14_7_Comment = Modal.Section14_7_Comment;
            dbModal.SuperintendedInspectionReport.Section14_8_Condition = Modal.Section14_8_Condition;
            dbModal.SuperintendedInspectionReport.Section14_8_Comment = Modal.Section14_8_Comment;
            dbModal.SuperintendedInspectionReport.Section14_9_Condition = Modal.Section14_9_Condition;
            dbModal.SuperintendedInspectionReport.Section14_9_Comment = Modal.Section14_9_Comment;
            dbModal.SuperintendedInspectionReport.Section14_10_Condition = Modal.Section14_10_Condition;
            dbModal.SuperintendedInspectionReport.Section14_10_Comment = Modal.Section14_10_Comment;
            dbModal.SuperintendedInspectionReport.Section14_11_Condition = Modal.Section14_11_Condition;
            dbModal.SuperintendedInspectionReport.Section14_11_Comment = Modal.Section14_11_Comment;
            dbModal.SuperintendedInspectionReport.Section14_12_Condition = Modal.Section14_12_Condition;
            dbModal.SuperintendedInspectionReport.Section14_12_Comment = Modal.Section14_12_Comment;
            dbModal.SuperintendedInspectionReport.Section14_13_Condition = Modal.Section14_13_Condition;
            dbModal.SuperintendedInspectionReport.Section14_13_Comment = Modal.Section14_13_Comment;
            dbModal.SuperintendedInspectionReport.Section14_14_Condition = Modal.Section14_14_Condition;
            dbModal.SuperintendedInspectionReport.Section14_14_Comment = Modal.Section14_14_Comment;
            dbModal.SuperintendedInspectionReport.Section14_15_Condition = Modal.Section14_15_Condition;
            dbModal.SuperintendedInspectionReport.Section14_15_Comment = Modal.Section14_15_Comment;
            dbModal.SuperintendedInspectionReport.Section14_16_Condition = Modal.Section14_16_Condition;
            dbModal.SuperintendedInspectionReport.Section14_16_Comment = Modal.Section14_16_Comment;
            dbModal.SuperintendedInspectionReport.Section14_17_Condition = Modal.Section14_17_Condition;
            dbModal.SuperintendedInspectionReport.Section14_17_Comment = Modal.Section14_17_Comment;
            dbModal.SuperintendedInspectionReport.Section14_18_Condition = Modal.Section14_18_Condition;
            dbModal.SuperintendedInspectionReport.Section14_18_Comment = Modal.Section14_18_Comment;
            dbModal.SuperintendedInspectionReport.Section14_19_Condition = Modal.Section14_19_Condition;
            dbModal.SuperintendedInspectionReport.Section14_19_Comment = Modal.Section14_19_Comment;
            dbModal.SuperintendedInspectionReport.Section14_20_Condition = Modal.Section14_20_Condition;
            dbModal.SuperintendedInspectionReport.Section14_20_Comment = Modal.Section14_20_Comment;
            dbModal.SuperintendedInspectionReport.Section14_21_Condition = Modal.Section14_21_Condition;
            dbModal.SuperintendedInspectionReport.Section14_21_Comment = Modal.Section14_21_Comment;
            dbModal.SuperintendedInspectionReport.Section14_22_Condition = Modal.Section14_22_Condition;
            dbModal.SuperintendedInspectionReport.Section14_22_Comment = Modal.Section14_22_Comment;
            dbModal.SuperintendedInspectionReport.Section14_23_Condition = Modal.Section14_23_Condition;
            dbModal.SuperintendedInspectionReport.Section14_23_Comment = Modal.Section14_23_Comment;
            dbModal.SuperintendedInspectionReport.Section14_24_Condition = Modal.Section14_24_Condition;
            dbModal.SuperintendedInspectionReport.Section14_24_Comment = Modal.Section14_24_Comment;
            dbModal.SuperintendedInspectionReport.Section14_25_Condition = Modal.Section14_25_Condition;
            dbModal.SuperintendedInspectionReport.Section14_25_Comment = Modal.Section14_25_Comment;

            dbModal.SuperintendedInspectionReport.Section15_1_Condition = Modal.Section15_1_Condition;
            dbModal.SuperintendedInspectionReport.Section15_1_Comment = Modal.Section15_1_Comment;
            dbModal.SuperintendedInspectionReport.Section15_2_Condition = Modal.Section15_2_Condition;
            dbModal.SuperintendedInspectionReport.Section15_2_Comment = Modal.Section15_2_Comment;
            dbModal.SuperintendedInspectionReport.Section15_3_Condition = Modal.Section15_3_Condition;
            dbModal.SuperintendedInspectionReport.Section15_3_Comment = Modal.Section15_3_Comment;
            dbModal.SuperintendedInspectionReport.Section15_4_Condition = Modal.Section15_4_Condition;
            dbModal.SuperintendedInspectionReport.Section15_4_Comment = Modal.Section15_4_Comment;
            dbModal.SuperintendedInspectionReport.Section15_5_Condition = Modal.Section15_5_Condition;
            dbModal.SuperintendedInspectionReport.Section15_5_Comment = Modal.Section15_5_Comment;
            dbModal.SuperintendedInspectionReport.Section15_6_Comment = Modal.Section15_6_Comment;
            dbModal.SuperintendedInspectionReport.Section15_7_Condition = Modal.Section15_7_Condition;
            dbModal.SuperintendedInspectionReport.Section15_7_Comment = Modal.Section15_7_Comment;
            dbModal.SuperintendedInspectionReport.Section15_8_Condition = Modal.Section15_8_Condition;
            dbModal.SuperintendedInspectionReport.Section15_8_Comment = Modal.Section15_8_Comment;
            dbModal.SuperintendedInspectionReport.Section15_9_Condition = Modal.Section15_9_Condition;
            dbModal.SuperintendedInspectionReport.Section15_9_Comment = Modal.Section15_9_Comment;
            dbModal.SuperintendedInspectionReport.Section15_10_Condition = Modal.Section15_10_Condition;
            dbModal.SuperintendedInspectionReport.Section15_10_Comment = Modal.Section15_10_Comment;
            dbModal.SuperintendedInspectionReport.Section15_11_Condition = Modal.Section15_11_Condition;
            dbModal.SuperintendedInspectionReport.Section15_11_Comment = Modal.Section15_11_Comment;
            dbModal.SuperintendedInspectionReport.Section15_12_Condition = Modal.Section15_12_Condition;
            dbModal.SuperintendedInspectionReport.Section15_12_Comment = Modal.Section15_12_Comment;
            dbModal.SuperintendedInspectionReport.Section15_13_Condition = Modal.Section15_13_Condition;
            dbModal.SuperintendedInspectionReport.Section15_13_Comment = Modal.Section15_13_Comment;
            dbModal.SuperintendedInspectionReport.Section15_14_Condition = Modal.Section15_14_Condition;
            dbModal.SuperintendedInspectionReport.Section15_14_Comment = Modal.Section15_14_Comment;
            dbModal.SuperintendedInspectionReport.Section15_15_Condition = Modal.Section15_15_Condition;
            dbModal.SuperintendedInspectionReport.Section15_15_Comment = Modal.Section15_15_Comment;

            dbModal.SuperintendedInspectionReport.Section16_1_Condition = Modal.Section16_1_Condition;
            dbModal.SuperintendedInspectionReport.Section16_1_Comment = Modal.Section16_1_Comment;
            dbModal.SuperintendedInspectionReport.Section16_2_Condition = Modal.Section16_2_Condition;
            dbModal.SuperintendedInspectionReport.Section16_2_Comment = Modal.Section16_2_Comment;
            dbModal.SuperintendedInspectionReport.Section16_3_Condition = Modal.Section16_3_Condition;
            dbModal.SuperintendedInspectionReport.Section16_3_Comment = Modal.Section16_3_Comment;
            dbModal.SuperintendedInspectionReport.Section16_4_Condition = Modal.Section16_4_Condition;
            dbModal.SuperintendedInspectionReport.Section16_4_Comment = Modal.Section16_4_Comment;

            dbModal.SuperintendedInspectionReport.Section17_1_Condition = Modal.Section17_1_Condition;
            dbModal.SuperintendedInspectionReport.Section17_1_Comment = Modal.Section17_1_Comment;
            dbModal.SuperintendedInspectionReport.Section17_2_Condition = Modal.Section17_2_Condition;
            dbModal.SuperintendedInspectionReport.Section17_2_Comment = Modal.Section17_2_Comment;
            dbModal.SuperintendedInspectionReport.Section17_3_Condition = Modal.Section17_3_Condition;
            dbModal.SuperintendedInspectionReport.Section17_3_Comment = Modal.Section17_3_Comment;
            dbModal.SuperintendedInspectionReport.Section17_4_Condition = Modal.Section17_4_Condition;
            dbModal.SuperintendedInspectionReport.Section17_4_Comment = Modal.Section17_4_Comment;
            dbModal.SuperintendedInspectionReport.Section17_5_Condition = Modal.Section17_5_Condition;
            dbModal.SuperintendedInspectionReport.Section17_5_Comment = Modal.Section17_5_Comment;
            dbModal.SuperintendedInspectionReport.Section17_6_Condition = Modal.Section17_6_Condition;
            dbModal.SuperintendedInspectionReport.Section17_6_Comment = Modal.Section17_6_Comment;

            dbModal.SuperintendedInspectionReport.Section18_1_Condition = Modal.Section18_1_Condition;
            dbModal.SuperintendedInspectionReport.Section18_1_Comment = Modal.Section18_1_Comment;
            dbModal.SuperintendedInspectionReport.Section18_2_Condition = Modal.Section18_2_Condition;
            dbModal.SuperintendedInspectionReport.Section18_2_Comment = Modal.Section18_2_Comment;
            dbModal.SuperintendedInspectionReport.Section18_3_Condition = Modal.Section18_3_Condition;
            dbModal.SuperintendedInspectionReport.Section18_3_Comment = Modal.Section18_3_Comment;
            dbModal.SuperintendedInspectionReport.Section18_4_Condition = Modal.Section18_4_Condition;
            dbModal.SuperintendedInspectionReport.Section18_4_Comment = Modal.Section18_4_Comment;
            dbModal.SuperintendedInspectionReport.Section18_5_Condition = Modal.Section18_5_Condition;
            dbModal.SuperintendedInspectionReport.Section18_5_Comment = Modal.Section18_5_Comment;
            dbModal.SuperintendedInspectionReport.Section18_6_Condition = Modal.Section18_6_Condition;
            dbModal.SuperintendedInspectionReport.Section18_6_Comment = Modal.Section18_6_Comment;
            dbModal.SuperintendedInspectionReport.Section18_7_Condition = Modal.Section18_7_Condition;
            dbModal.SuperintendedInspectionReport.Section18_7_Comment = Modal.Section18_7_Comment;

            // RDBJ 02/15/2022
            dbModal.SuperintendedInspectionReport.Section18_8_Condition = Modal.Section18_8_Condition;
            dbModal.SuperintendedInspectionReport.Section18_8_Comment = Modal.Section18_8_Comment;
            dbModal.SuperintendedInspectionReport.Section18_9_Condition = Modal.Section18_9_Condition;
            dbModal.SuperintendedInspectionReport.Section18_9_Comment = Modal.Section18_9_Comment;
            // End RDBJ 02/15/2022

            dbModal.SuperintendedInspectionReport.IsSynced = Modal.IsSynced;
            dbModal.SuperintendedInspectionReport.CreatedDate = Modal.CreatedDate;
            dbModal.SuperintendedInspectionReport.ModifyDate = Modal.ModifyDate;
            dbModal.SuperintendedInspectionReport.SavedAsDraft = Modal.SavedAsDraft;
        }

        //RDBJ 09/25/2021
        public void SIRNotes_Save(Guid? UniqueFormID, List<Modals.SIRNote> SIRNotes)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                if (SIRNotes != null && SIRNotes.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    // RDBJ 04/02/2022
                    foreach (var item in SIRNotes)
                    {
                        bool IsSIRNoteExist = false;
                        Entity.SIRNote SIRNote = dbContext.SIRNotes.Where(x => x.NotesUniqueID == item.NotesUniqueID).FirstOrDefault();

                        if (SIRNote != null)
                            IsSIRNoteExist = true;
                        else
                            SIRNote = new Entity.SIRNote();

                        SIRNote.NotesUniqueID = item.NotesUniqueID;
                        SIRNote.SIRFormID = 0;
                        SIRNote.Number = item.Number;
                        SIRNote.Note = item.Note;
                        SIRNote.UniqueFormID = UniqueFormID;

                        if (!IsSIRNoteExist)
                            dbContext.SIRNotes.Add(SIRNote);

                        dbContext.SaveChanges();
                    }
                    // End RDBJ 04/02/2022


                    // RDBJ 04/02/2022 Commented 
                    /*
                    var dbSIRNotes = dbContext.SIRNotes.Where(x => x.UniqueFormID == UniqueFormID);
                    if (dbSIRNotes != null)
                    {
                        foreach (var item in dbSIRNotes)
                        {
                            dbContext.SIRNotes.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }
                    

                    foreach (var item in SIRNotes)
                    {
                        Entity.SIRNote note = new Entity.SIRNote();
                        note.Note = item.Note;
                        note.Number = item.Number;
                        note.SIRFormID = 0;
                        note.UniqueFormID = item.UniqueFormID;
                        dbContext.SIRNotes.Add(note);
                    }
                    dbContext.SaveChanges();
                    */
                    // End RDBJ 04/02/2022 Commented 
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRNotes_Save : " + ex.Message);
            }
        }
        //End RDBJ 09/25/2021

        //RDBJ 09/25/2021
        public void SIRAdditionalNote_Save(Guid? UniqueFormID, List<Modals.SIRAdditionalNote> SIRAddNotes)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                if (SIRAddNotes != null && SIRAddNotes.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    // RDBJ 04/02/2022
                    foreach (var item in SIRAddNotes)
                    {
                        bool IsSIRAddNoteExist = false;
                        Entity.SIRAdditionalNote SIRAddNote = dbContext.SIRAdditionalNotes.Where(x => x.NotesUniqueID == item.NotesUniqueID).FirstOrDefault();

                        if (SIRAddNote != null)
                            IsSIRAddNoteExist = true;
                        else
                            SIRAddNote = new Entity.SIRAdditionalNote();

                        SIRAddNote.NotesUniqueID = item.NotesUniqueID;
                        SIRAddNote.SIRFormID = 0;
                        SIRAddNote.Number = item.Number;
                        SIRAddNote.Note = item.Note;
                        SIRAddNote.UniqueFormID = UniqueFormID;

                        if (!IsSIRAddNoteExist)
                            dbContext.SIRAdditionalNotes.Add(SIRAddNote);

                        dbContext.SaveChanges();
                    }
                    // End RDBJ 04/02/2022

                    // RDBJ 04/02/2022 Commented 
                    /*
                    var dbSIRNotes = dbContext.SIRNotes.Where(x => x.UniqueFormID == UniqueFormID);
                    if (dbSIRNotes != null)
                    {
                        foreach (var item in dbSIRNotes)
                        {
                            dbContext.SIRNotes.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }

                    foreach (var item in SIRAddNotes)
                    {
                        Entity.SIRAdditionalNote note = new Entity.SIRAdditionalNote();
                        note.Note = item.Note;
                        note.Number = item.Number;
                        note.SIRFormID = 0;
                        note.UniqueFormID = item.UniqueFormID;
                        dbContext.SIRAdditionalNotes.Add(note);
                    }
                    dbContext.SaveChanges();
                    */
                    // End RDBJ 04/02/2022 Commented 
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRAdditionalNote_Save : " + ex.Message);
            }
        }
        //End RDBJ 09/25/2021

        public void GIRDeficiencies_Save(Guid? UniqueFormID, List<Modals.GIRDeficiencies> GIRDeficiencies)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0)
                {
                    foreach (var item in GIRDeficiencies)
                    {
                        //RDBJ 10/11/2021 Fixed New Adding and If exist was not update
                        Entity.GIRDeficiency member = new Entity.GIRDeficiency();
                        member = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UniqueFormID && x.No == item.No).FirstOrDefault(); //RDBJ 10/11/2021
                        int cnt = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UniqueFormID && x.No == item.No).Select(x => x.DeficienciesID).FirstOrDefault();

                        if (cnt == 0)
                        {
                            member.GIRFormID = 0;
                            member.No = item.No;
                            member.DateRaised = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.Deficiency = item.Deficiency;
                            member.DateClosed = item.DateClosed;
                            member.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            member.Ship = item.Ship;
                            member.IsClose = false;
                            member.ReportType = "SI";
                            member.ItemNo = item.ItemNo;
                            member.Section = item.Section;
                            member.UniqueFormID = UniqueFormID;
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

                        //GIRDeficienciesFile_Save(item, member.DeficienciesUniqueID);  // JSL 04/06/2022 commented this line
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRDeficiencies_Save : " + ex.Message);
            }
        }
        public void GIRDeficienciesFile_Save(GIRDeficiencies modal, Guid? DeficienciesUniqueID)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                // RDBJ 01/20/2022
                List<Entity.GIRDeficienciesFile> girDefFiles = new List<Entity.GIRDeficienciesFile>();
                girDefFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == DeficienciesUniqueID).ToList();

                foreach (var itemFile in girDefFiles)
                {
                    dbContext.GIRDeficienciesFiles.Remove(itemFile);
                    dbContext.SaveChanges();
                }
                // End RDBJ 01/20/2022

                foreach (var item in modal.GIRDeficienciesFile)
                {
                    if (item.IsUpload == "true")
                    {
                        var split = item.StorePath.Split(',');//.LastOrDefault();
                        string OrignalString = split.LastOrDefault();
                        if (!string.IsNullOrEmpty(OrignalString))
                        {
                            //RDBJ 09/25/2021 Commented
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
                            */

                            Entity.GIRDeficienciesFile file = new Entity.GIRDeficienciesFile();
                            file.FileName = item.FileName;
                            file.StorePath = item.StorePath; //RDBJ 09/25/2021 set file bite store ratherthan  "/GIRDeficiency/" + subPath + imageName;
                            file.DeficienciesID = 0; //RDBJ 09/25/2021 set 0
                            file.DeficienciesUniqueID = DeficienciesUniqueID; //RDBJ 09/25/2021
                            file.DeficienciesFileUniqueID = (Guid)item.DeficienciesFileUniqueID;  // JSL 06/04/2022

                            // RDBJ 03/12/2022 wrapped in if
                            if (Convert.ToBoolean(item.IsUpload))
                                {
                                dbContext.GIRDeficienciesFiles.Add(file);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Save GIRDeficiency Image File " + ex.Message + "\n" + ex.InnerException);
            }
        }
    }
}
