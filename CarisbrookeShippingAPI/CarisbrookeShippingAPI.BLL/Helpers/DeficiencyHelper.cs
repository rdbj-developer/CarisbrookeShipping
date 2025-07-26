using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class DeficiencyHelper
    {
        public List<Deficiency_GISI_Ships> GetGISIShips()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Deficiency_GISI_Ships> ShipsList = new List<Deficiency_GISI_Ships>();
            try
            {
                ShipsList = (from G in dbContext.GIRDeficiencies
                             group G by G.Ship into pg
                             join s in dbContext.CSShips on pg.FirstOrDefault().Ship equals s.Code
                             where s.Code != null
                             select new Deficiency_GISI_Ships
                             {
                                 Ship = s.Code,
                                 ShipName = s.Name,
                                 TotalDeficiencies = pg.Where(x => x.ReportType == "GI" || x.ReportType == "SI").Count(),
                                 TotalOutstending = pg.Where(x => x.IsClose == false || x.IsClose == null).Count(),
                                 GIDeficiencies = pg.Where(x => x.ReportType == "GI").Count(),
                                 OpenGIDeficiencies = pg.Where(x => x.ReportType == "GI" && x.IsClose == false).Count(),
                                 SIDeficiencies = pg.Where(x => x.ReportType == "SI").Count(),
                                 OpenSIDeficiencies = pg.Where(x => x.ReportType == "SI" && x.IsClose == false).Count()
                             }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGISIShips " + ex.Message + "\n" + ex.InnerException);
            }
            return ShipsList;
        }
        public List<Deficiency_GISI_Report> GetShipGISIReports(string ShipCode, string type)
        {
            List<Deficiency_GISI_Report> GISIData = new List<Deficiency_GISI_Report>();
            try
            {
                if (type.ToLower() == "gi")
                    GISIData.AddRange(GetGIDeficiencies(ShipCode));
                else if (type.ToLower() == "si")
                    GISIData.AddRange(GetSIDeficiencies(ShipCode));
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipGISIReports : " + ex.Message);
            }
            return GISIData;
        }
        public List<Deficiency_GISI_Report> GetGIDeficiencies(string ShipCode)
        {
            List<Deficiency_GISI_Report> GIData = new List<Deficiency_GISI_Report>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Entity.GeneralInspectionReport> dbGIData = dbContext.GeneralInspectionReports.Where(x => x.Ship == ShipCode && x.SavedAsDraft == false
            && x.isDelete == 0  // RDBJ 01/05/2022
            ).ToList();
            List<Deficiency_GISI_Report> GIList = dbGIData
                //.OrderByDescending(x => x.GIRFormID)  // RDBJ 01/04/2022 commented this line
                //.OrderByDescending(x => x.Date) // RDBJ 01/04/2022
                .Select(x => new Deficiency_GISI_Report()
                {
                    FormID = x.GIRFormID,
                    UniqueFormID = x.UniqueFormID,
                    Ship = x.Ship,
                    Type = "GI",
                    //Date = Utility.ToDateTimeStr(x.CreatedDate), // RDBJ 01/04/2022 commented this line
                    Date = x.Date, // RDBJ 01/04/2022
                    Location = x.Port,
                    Auditor = x.Inspector,
                    GIRDeficiences = dbContext.GIRDeficiencies.Where(z => z.isDelete == 0 && z.ReportType == "GI" && z.Ship == x.Ship && (((z.UniqueFormID == null) && z.GIRFormID == x.GIRFormID) || z.UniqueFormID == x.UniqueFormID)).ToList() //RDBJ 10/13/2021 Added isDelete == 0
                }).ToList();
            foreach (var item in GIList)
            {
                List<GIRDeficiency> dbDeficiencies = item.GIRDeficiences.ToList();
                if (dbDeficiencies != null && dbDeficiencies.Count > 0)
                {
                    item.Deficiencies = dbDeficiencies.Count;
                    item.OpenDeficiencies = dbDeficiencies.Where(x => x.IsClose == false).Count();
                    foreach (var defItem in dbDeficiencies)
                    {
                        //int days = (Utility.ToDateTime(defItem.DateRaised).Date - DateTime.Now.Date).Days;
                        int days = (Utility.ToDateTime(defItem.DateRaised).Date - Utility.ToDateTimeUtcNow().Date).Days;
                        if (days < 0)
                            days = -days;
                        if (days > 84 && defItem.IsClose == false)
                        {
                            item.isExpired = true;
                            break;
                        }
                    }
                }
                else
                {
                    item.Deficiencies = 0;
                    item.OpenDeficiencies = 0;
                    item.isExpired = false;
                }
                item.GIRDeficiences = null;
                GIData.Add(item);
            }
            return GIData
                .OrderByDescending(x => x.Date).ToList(); // RDBJ 04/01/2022
        }
        public List<Deficiency_GISI_Report> GetSIDeficiencies(string ShipCode)
        {
            List<Deficiency_GISI_Report> SIData = new List<Deficiency_GISI_Report>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Entity.SuperintendedInspectionReport> dbSIData = dbContext.SuperintendedInspectionReports.Where(x => x.ShipName == ShipCode && x.SavedAsDraft == false
            && x.isDelete == 0  // RDBJ 01/05/2022
            ).ToList();
            List<Deficiency_GISI_Report> SIList = dbSIData
                //.OrderByDescending(x => x.SIRFormID) // RDBJ 01/04/2022 commented this line
                //.OrderByDescending(x => x.Date) // RDBJ 04/01/2022
                .Select(x => new Deficiency_GISI_Report()
                {
                    //FormID = x.SIRFormID,
                    UniqueFormID = x.UniqueFormID,
                    Ship = x.ShipName,
                    Type = "SI",
                    //Date = Utility.ToDateTimeStr(x.CreatedDate), // RDBJ 01/04/2022 commented this line
                    Date = x.Date, // RDBJ 01/04/2022
                    Location = x.Port,
                    Auditor = x.Superintended,
                    GIRDeficiences = dbContext.GIRDeficiencies.Where(z => z.isDelete == 0 && z.ReportType == "SI" && z.Ship == x.ShipName && z.UniqueFormID == x.UniqueFormID).ToList() //RDBJ 10/13/2021 Added isDelete == 0
                }).ToList();
            foreach (var item in SIList)
            {
                List<GIRDeficiency> dbDeficiencies = item.GIRDeficiences.ToList();
                if (dbDeficiencies != null && dbDeficiencies.Count > 0)
                {
                    item.Deficiencies = dbDeficiencies.Count;
                    item.OpenDeficiencies = dbDeficiencies.Where(x => x.IsClose == false).Count();
                    foreach (var defItem in dbDeficiencies)
                    {
                        //int days = (Utility.ToDateTime(defItem.DateRaised).Date - DateTime.Now.Date).Days;
                        int days = (Utility.ToDateTime(defItem.DateRaised).Date - Utility.ToDateTimeUtcNow().Date).Days;
                        if (days < 0)
                            days = -days;
                        if (days > 84 && defItem.IsClose == false)
                        {
                            item.isExpired = true;
                            break;
                        }
                    }
                }
                else
                {
                    item.Deficiencies = 0;
                    item.OpenDeficiencies = 0;
                    item.isExpired = false;
                }
                item.GIRDeficiences = null;
                SIData.Add(item);
            }
            return SIData
                .OrderByDescending(x => x.Date).ToList(); // RDBJ 04/01/2022
        }
        public List<GIRDataList> GetDeficienciesData(Deficiency_GISI_Report value)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<GIRDataList> list = new List<GIRDataList>();
            try
            {
                var data = dbContext.GIRDeficiencies
                    .Where(x => x.Ship == value.Ship && x.UniqueFormID == value.UniqueFormID && x.ReportType == value.Type && x.isDelete == 0)
                    //.OrderBy(x => x.No) // RDBJ 12/14/2021 Commented this line
                    .OrderByDescending(x => x.UpdatedDate) // RDBJ 12/14/2021
                    .ToList(); //RDBJ 10/13/2021 Added isDelete == 0

                //var query = from GD in dbContext.GIRDeficiencies
                //            join UP in dbContext.UserProfile on UP.User equals GD.Ass into gj
                //            select new
                //            {
                //                UsergroupID = u.UsergroupID,
                //                UsergroupName = u.UsergroupName,
                //                Price = (x == null ? String.Empty : x.Price)
                //            };

                string Port = "", Inspector = "";

                if (value.Type.ToLower() == "gi")
                {
                    var giObj = dbContext.GeneralInspectionReports.Where(x => x.Ship == value.Ship && x.UniqueFormID == value.UniqueFormID).FirstOrDefault();
                    if (giObj == null)
                        giObj = new Entity.GeneralInspectionReport();
                    Port = giObj.Port;
                    Inspector = giObj.Inspector;
                }
                else
                {
                    var siObj = dbContext.SuperintendedInspectionReports.Where(x => x.ShipName == value.Ship && x.SIRFormID == value.FormID).FirstOrDefault();
                    if (siObj == null)
                        siObj = new Entity.SuperintendedInspectionReport();
                    Port = siObj.Port;
                    Inspector = siObj.Superintended;
                }

                foreach (var item in data)
                {
                    GIRDataList obj = new GIRDataList();
                    //var files = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesID == item.DeficienciesID).ToList(); //RDBJ 09/18/2021 Commented
                    var files = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == item.DeficienciesUniqueID).ToList(); //RDBJ 09/18/2021 
                    foreach (var subitem in files)
                    {
                        Modals.GIRDeficienciesFile filedata = new Modals.GIRDeficienciesFile();
                        //filedata.DeficienciesID = subitem.DeficienciesID; // RDBJ 12/15/2021 commented this line
                        filedata.FileName = subitem.FileName;
                        //filedata.StorePath = subitem.StorePath; // RDBJ 12/15/2021 commented this line
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
                    obj.UniqueFormID = item.UniqueFormID;
                    obj.ReportType = item.ReportType;
                    obj.DateRaised = item.DateRaised.HasValue ? item.DateRaised : item.CreatedDate;
                    obj.DateClosed = item.DateClosed;
                    obj.Section = item.Section;
                    obj.Inspector = Inspector;
                    obj.Port = Port;
                    obj.CreatedDate = item.CreatedDate;
                    obj.DeficienciesUniqueID = item.DeficienciesUniqueID;
                    obj.UpdatedDate = item.UpdatedDate; // RDBJ 12/14/2021
                    obj.AssignTo = item.AssignTo != null ? dbContext.UserProfiles.Where(x => x.UserID == item.AssignTo).Select(y => y.UserName).FirstOrDefault() : string.Empty; // RDBJ 12/21/2021

                    //int days = (Utility.ToDateTime(item.DateRaised).Date - DateTime.Now.Date).Days;
                    int days = (Utility.ToDateTime(item.DateRaised).Date - Utility.ToDateTimeUtcNow().Date).Days;
                    if (days < 0)
                        days = -days;
                    if (days > 84 && obj.IsColse == false)
                        obj.isExpired = true;

                    list.Add(obj);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesData : " + ex.Message);
            }
            return list.OrderByDescending(x => x.UpdatedDate).ToList();
        }

        public List<Deficiency_Audit_Ships> GetAuditShips(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Deficiency_Audit_Ships> ShipsList = new List<Deficiency_Audit_Ships>();
            try
            {
                ShipsList = (from G in dbContext.AuditNotes
                             group G by G.Ship into pg
                             join s in dbContext.CSShips on pg.FirstOrDefault().Ship equals s.Code
                             where s.Code != null && s.Code == shipCode
                             select new Deficiency_Audit_Ships
                             {
                                 IAFId = pg.FirstOrDefault().InternalAuditFormId,
                                 Ship = s.Code,
                                 ShipName = s.Name,
                                 OpenISMOBS = pg.Where(y => y.Type == "ISM-Observation" && y.Ship == s.Code).Count(),
                                 OpenISMNCNs = pg.Where(y => y.Type == "ISM-Non Conformity" && y.Ship == s.Code).Count(),
                                 OpenISPSOBS = pg.Where(y => y.Type == "ISPS-Observation" && y.Ship == s.Code).Count(),
                                 OpenISPSNCN = pg.Where(y => y.Type == "ISPS-Non Conformity" && y.Ship == s.Code).Count(),
                                 OpenMLCOBS = 0,
                                 OpenMLCNCNs = 0,
                             }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditShips " + ex.Message + "\n" + ex.InnerException);
            }
            return ShipsList;
        }
        public List<Deficiency_Ship_Audits> GetShipAudits(string Code
            , bool blnIsAddedNewAudit   // JSL 04/20/2022
            )
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Deficiency_Ship_Audits> data = new List<Deficiency_Ship_Audits>();
            try
            {
                List<Entity.InternalAuditForm> dbAuditForms = dbContext.InternalAuditForms
                    .Where(
                    x => x.ShipName == Code && x.isDelete == 0 //RDBJ 11/10/2021 Added x.isDelete == 0
                    && x.SavedAsDraft == false  // RDBJ 01/23/2022
                ).ToList();

                // RDBJ 01/24/2022 commented code
                /*
                data = dbAuditForms.OrderByDescending(x => x.Date).Select(x => new Deficiency_Ship_Audits() // RDBJ 04/01/2022 set OrderByDesc Date //RDBJ 11/25/2021 Set OrderBYDesc CreatedDate //RDBJ 11/13/2021 Set UniqueFormID
                {
                    InternalAuditFormId = x.InternalAuditFormId,
                    UniqueFormID = x.UniqueFormID,
                    Subject = x.AuditTypeISM == true ? "ISM" : x.AuditTypeISPS == true ? "ISPS" : x.AuditTypeMLC == true ? "MLC" : string.Empty,
                    Type = x.AuditType, // "Internal", //RDBJ 11/24/2021 set dynamics
                    Extra = x.IsAdditional, // false, //RDBJ 11/24/2021 set dynamics
                    //AuditDate = Utility.ToDateTimeStr(x.Date), // RDBJ 04/01/2022 commented this line
                    AuditDate = x.Date, // RDBJ 04/01/2022
                    Location = x.Location,
                    Auditor = x.Auditor,
                    NCN = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "ISM-Non Conformity" || z.Type == "ISPS-Non Conformity").ToList().Count,  //RDBJ 11/22/2021 && y.isDelete == 0 //RDBJ 11/13/2021 Set UniqueFormID
                    OutstandingNCN = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0 && (y.IsResolved == null || y.IsResolved == false)).ToList().Where(z => z.Type == "ISM-Non Conformity" || z.Type == "ISPS-Non Conformity").ToList().Count, //RDBJ 11/22/2021 && y.isDelete == 0 //RDBJ 11/13/2021 Set UniqueFormID and handle isResolved null
                    OBS = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "ISM-Observation" || z.Type == "ISPS-Observation").ToList().Count, //RDBJ 11/22/2021 && y.isDelete == 0 //RDBJ 11/13/2021 Set UniqueFormID
                    OutstandingOBS = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0 && (y.IsResolved == null || y.IsResolved == false)).ToList().Where(z => z.Type == "ISM-Observation" || z.Type == "ISPS-Observation").ToList().Count, //RDBJ 11/22/2021 && y.isDelete == 0 //RDBJ 11/13/2021 Set UniqueFormID and handle isResolved null
                    MLC = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "MLC-Deficiency").ToList().Count, // RDBJ 01/24/2022
                    OutstandingMLC = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0 && (y.IsResolved == null || y.IsResolved == false)).ToList().Where(z => z.Type == "MLC-Deficiency").ToList().Count, // RDBJ 01/24/2022
                    Closed = x.IsClosed, // true, //RDBJ 11/24/2021 set dynamics
                    AuditTypeISM = x.AuditTypeISM, //RDBJ 11/23/2021
                    AuditTypeISPS = x.AuditTypeISPS, //RDBJ 11/23/2021
                    AuditTypeMLC = x.AuditTypeMLC, //RDBJ 11/23/2021
                }).ToList();
                */
                // End RDBJ 01/24/2022 commented code

                // JSL 04/20/2022
                if (blnIsAddedNewAudit)
                {
                    dbAuditForms = dbAuditForms
                    .OrderByDescending(x => x.CreatedDate)
                    .OrderByDescending(x => x.InternalAuditFormId)
                    .ToList();
                }
                else
                {
                    dbAuditForms = dbAuditForms
                    .OrderByDescending(x => x.Date)
                    .ToList();
                }
                // End JSL 04/20/2022

                // RDBJ 01/24/2022
                foreach (var x in dbAuditForms
                    //.OrderByDescending(x => x.Date)
                    //.OrderByDescending(x => x.CreatedDate)    // RDBJ 04/20/2022 commented this condition  // RDBJ 01/28/2022
                    )
                {
                    List<string> strSubject = new List<string>();
                    Deficiency_Ship_Audits defShipAudit = new Deficiency_Ship_Audits();
                    defShipAudit.InternalAuditFormId = x.InternalAuditFormId;
                    defShipAudit.UniqueFormID = x.UniqueFormID;

                    if (x.AuditTypeISM == true)
                    {
                        strSubject.Add("ISM");
                    }
                    if (x.AuditTypeISPS == true)
                    {
                        strSubject.Add("ISPS");
                    }
                    if (x.AuditTypeMLC == true)
                    {
                        strSubject.Add("MLC");
                    }

                    defShipAudit.Subject = String.Join(", ", strSubject.ToArray());
                    
                    defShipAudit.Type = x.AuditType;
                    defShipAudit.Extra = x.IsAdditional;
                    defShipAudit.AuditDate = x.Date;
                    defShipAudit.Location = x.Location;
                    defShipAudit.Auditor = x.Auditor;
                    defShipAudit.NCN = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "ISM-Non Conformity" || z.Type == "ISPS-Non Conformity").ToList().Count; 
                    defShipAudit.OutstandingNCN = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0 && (y.IsResolved == null || y.IsResolved == false)).ToList().Where(z => z.Type == "ISM-Non Conformity" || z.Type == "ISPS-Non Conformity").ToList().Count;
                    defShipAudit.OBS = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "ISM-Observation" || z.Type == "ISPS-Observation").ToList().Count;
                    defShipAudit.OutstandingOBS = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0 && (y.IsResolved == null || y.IsResolved == false)).ToList().Where(z => z.Type == "ISM-Observation" || z.Type == "ISPS-Observation").ToList().Count; 
                    defShipAudit.MLC = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0).ToList().Where(z => z.Type == "MLC-Deficiency").ToList().Count;
                    defShipAudit.OutstandingMLC = dbContext.AuditNotes.Where(y => y.UniqueFormID == x.UniqueFormID && y.isDelete == 0 && (y.IsResolved == null || y.IsResolved == false)).ToList().Where(z => z.Type == "MLC-Deficiency").ToList().Count;
                    defShipAudit.Closed = x.IsClosed;
                    defShipAudit.AuditTypeISM = x.AuditTypeISM;
                    defShipAudit.AuditTypeISPS = x.AuditTypeISPS;
                    defShipAudit.AuditTypeMLC = x.AuditTypeMLC;

                    data.Add(defShipAudit);
                }
                // End RDBJ 01/24/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                LogHelper.writelog(ex.InnerException.Message);
            }
            return data;
                //.OrderByDescending(x => x.AuditDate).ToList(); // RDBJ 04/01/2022
        }

        // JSL 02/11/2023
        public List<Modals.FSTOInspection> GetFSTOAuditDataByShipCode(string Code)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Modals.FSTOInspection> retLstFSTOInspection = new List<Modals.FSTOInspection>();
            try
            {
                var entityData = dbContext.FSTOInspections
                    .Where(x => x.ShipCode == Code && x.IsDeleted == false
                    )
                    .OrderByDescending(x => x.CreatedDateTime)  // JSL 02/18/2023
                    .ToList();


                foreach (var itemFSTO in entityData)
                {
                    string strCompletedDays = "";
                    Modals.FSTOInspection modalFSTOInspection = new Modals.FSTOInspection();

                    modalFSTOInspection.UniqueFormID = itemFSTO.UniqueFormID;
                    modalFSTOInspection.FormVersion = itemFSTO.FormVersion;

                    modalFSTOInspection.TravelStartedOn = itemFSTO.TravelStartedOn;
                    modalFSTOInspection.EmbarkedOn = itemFSTO.EmbarkedOn;
                    modalFSTOInspection.DisembarkedOn = itemFSTO.DisembarkedOn;
                    modalFSTOInspection.TravelEndedOn = itemFSTO.TravelEndedOn;

                    modalFSTOInspection.Location = itemFSTO.Location;

                    modalFSTOInspection.UserGUID = itemFSTO.UserGUID;
                    modalFSTOInspection.UserName = itemFSTO.UserGUID != null ? dbContext.UserProfiles.Where(x => x.UserID == itemFSTO.UserGUID).Select(y => y.UserName).FirstOrDefault() : string.Empty; ;

                    modalFSTOInspection.ShipCode = itemFSTO.ShipCode;
                    modalFSTOInspection.ShipName = !(string.IsNullOrEmpty(itemFSTO.ShipCode)) ? dbContext.CSShips.Where(x => x.Code == itemFSTO.ShipCode).Select(y => y.Name).FirstOrDefault() : string.Empty; ;

                    if (modalFSTOInspection.TravelStartedOn != null && modalFSTOInspection.TravelEndedOn != null)
                    {
                        TimeSpan timeSpan = ((TimeSpan)(modalFSTOInspection.TravelEndedOn - modalFSTOInspection.TravelStartedOn));
                        strCompletedDays = Convert.ToString(timeSpan.TotalDays);
                    }

                    modalFSTOInspection.CompletedDays = strCompletedDays;
                    modalFSTOInspection.PurposeOfVisit = itemFSTO.PurposeOfVisit;

                    modalFSTOInspection.CreatedBy = itemFSTO.CreatedBy;
                    modalFSTOInspection.CreatedDateTime = itemFSTO.CreatedDateTime;
                    modalFSTOInspection.ModifiedBy = itemFSTO.ModifiedBy;
                    modalFSTOInspection.ModifiedDateTime = itemFSTO.ModifiedDateTime;

                    // JSL 02/17/2023
                    modalFSTOInspection.FSTOInspectionAttachments = new List<Modals.FSTOInspectionAttachment>();
                    var fstoFiles = dbContext.FSTOInspectionAttachments.Where(x => x.UniqueFormID == itemFSTO.UniqueFormID && x.IsDeleted == false).ToList();
                    foreach (var fileItem in fstoFiles)
                    {
                        Modals.FSTOInspectionAttachment filedata = new Modals.FSTOInspectionAttachment();
                        filedata.UniqueID = fileItem.UniqueID;
                        filedata.AttachmentName = fileItem.AttachmentName;
                        modalFSTOInspection.FSTOInspectionAttachments.Add(filedata);
                    }
                    // End JSL 02/17/2023

                    retLstFSTOInspection.Add(modalFSTOInspection);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.ToString());
            }
            return retLstFSTOInspection;
        }
        // End JSL 02/11/2023

        // JSL 02/17/2023
        public Modals.FSTOInspection GetFSTOAuditDataById(string id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Modals.FSTOInspection retLstFSTOInspection = new Modals.FSTOInspection();
            Guid FSTOFormUniqueId = Guid.Parse(id);
            try
            {
                var itemFSTO = dbContext.FSTOInspections
                    .Where(x => x.UniqueFormID == FSTOFormUniqueId
                    ).FirstOrDefault();


                if (itemFSTO != null)
                {
                    string strCompletedDays = "";
                    Modals.FSTOInspection modalFSTOInspection = new Modals.FSTOInspection();

                    modalFSTOInspection.UniqueFormID = itemFSTO.UniqueFormID;
                    modalFSTOInspection.FormVersion = itemFSTO.FormVersion;

                    modalFSTOInspection.TravelStartedOn = itemFSTO.TravelStartedOn;
                    modalFSTOInspection.EmbarkedOn = itemFSTO.EmbarkedOn;
                    modalFSTOInspection.DisembarkedOn = itemFSTO.DisembarkedOn;
                    modalFSTOInspection.TravelEndedOn = itemFSTO.TravelEndedOn;

                    modalFSTOInspection.Location = itemFSTO.Location;

                    modalFSTOInspection.UserGUID = itemFSTO.UserGUID;
                    modalFSTOInspection.UserName = itemFSTO.UserGUID != null ? dbContext.UserProfiles.Where(x => x.UserID == itemFSTO.UserGUID).Select(y => y.UserName).FirstOrDefault() : string.Empty; ;

                    modalFSTOInspection.ShipCode = itemFSTO.ShipCode;
                    modalFSTOInspection.ShipName = !(string.IsNullOrEmpty(itemFSTO.ShipCode)) ? dbContext.CSShips.Where(x => x.Code == itemFSTO.ShipCode).Select(y => y.Name).FirstOrDefault() : string.Empty; ;

                    if (modalFSTOInspection.TravelStartedOn != null && modalFSTOInspection.TravelEndedOn != null)
                    {
                        TimeSpan timeSpan = ((TimeSpan)(modalFSTOInspection.TravelEndedOn - modalFSTOInspection.TravelStartedOn));
                        strCompletedDays = Convert.ToString(timeSpan.TotalDays);
                    }

                    modalFSTOInspection.CompletedDays = strCompletedDays;
                    modalFSTOInspection.PurposeOfVisit = itemFSTO.PurposeOfVisit;

                    modalFSTOInspection.CreatedBy = itemFSTO.CreatedBy;
                    modalFSTOInspection.CreatedDateTime = itemFSTO.CreatedDateTime;
                    modalFSTOInspection.ModifiedBy = itemFSTO.ModifiedBy;
                    modalFSTOInspection.ModifiedDateTime = itemFSTO.ModifiedDateTime;

                    modalFSTOInspection.FSTOInspectionAttachments = new List<Modals.FSTOInspectionAttachment>();
                    var fstoFiles = dbContext.FSTOInspectionAttachments.Where(x => x.UniqueFormID == itemFSTO.UniqueFormID && x.IsDeleted == false).ToList();
                    foreach (var fileItem in fstoFiles)
                    {
                        Modals.FSTOInspectionAttachment filedata = new Modals.FSTOInspectionAttachment();
                        filedata.UniqueID = fileItem.UniqueID;
                        filedata.AttachmentName = fileItem.AttachmentName;
                        modalFSTOInspection.FSTOInspectionAttachments.Add(filedata);
                    }

                    retLstFSTOInspection = modalFSTOInspection;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.ToString());
            }
            return retLstFSTOInspection;
        }
        // End JSL 02/17/2023

        // JSL 02/17/2023
        public Dictionary<string, string> GetFSTOFile(string fileId)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid FileUniqueID = Guid.Parse(fileId);
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            try
            {
                var data = dbContext.FSTOInspectionAttachments.Where(x => x.UniqueID == FileUniqueID).FirstOrDefault();
                if (data != null)
                {
                    retDicData["FileName"] = data.AttachmentName;
                    retDicData["FileData"] = data.AttachmentPath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFSTOFile " + ex.ToString());
            }
            return retDicData;
        }
        // End JSL 02/17/2023

        // JSL 02/18/2023
        public bool InsertOrUpdateFSTO(Dictionary<string, string> dictMetaData)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            bool retblnResult = false;
            try
            {
                Guid FSTOFormUniqueId = Guid.Empty;
                string UserGUID = string.Empty;
                string ShipCode = string.Empty;
                string TravelStartedOn = string.Empty;
                string EmbarkedOn = string.Empty;
                string DisembarkedOn = string.Empty;
                string TravelEndedOn = string.Empty;
                string Location = string.Empty;
                string PurposeOfVisit = string.Empty;
                string CurrentUserID = string.Empty;

                if (dictMetaData.ContainsKey("UniqueFormID"))
                    FSTOFormUniqueId = Guid.Parse(dictMetaData["UniqueFormID"].ToString());

                if (dictMetaData.ContainsKey("UserGUID"))
                    UserGUID = dictMetaData["UserGUID"].ToString();

                if (dictMetaData.ContainsKey("ShipCode"))
                    ShipCode = dictMetaData["ShipCode"].ToString();

                if (dictMetaData.ContainsKey("TravelStartedOn"))
                    TravelStartedOn = dictMetaData["TravelStartedOn"].ToString();

                if (dictMetaData.ContainsKey("EmbarkedOn"))
                    EmbarkedOn = dictMetaData["EmbarkedOn"].ToString();

                if (dictMetaData.ContainsKey("DisembarkedOn"))
                    DisembarkedOn = dictMetaData["DisembarkedOn"].ToString();

                if (dictMetaData.ContainsKey("TravelEndedOn"))
                    TravelEndedOn = dictMetaData["TravelEndedOn"].ToString();

                if (dictMetaData.ContainsKey("Location"))
                    Location = dictMetaData["Location"].ToString();

                if (dictMetaData.ContainsKey("PurposeOfVisit"))
                    PurposeOfVisit = dictMetaData["PurposeOfVisit"].ToString();

                if (dictMetaData.ContainsKey("CurrentUserID"))
                    CurrentUserID = dictMetaData["CurrentUserID"].ToString();

                bool IsNeedToUpdate = false;

                var entityFSTOModal = dbContext.FSTOInspections.Where(x => x.UniqueFormID == FSTOFormUniqueId).FirstOrDefault();
                if (entityFSTOModal != null && (entityFSTOModal.UniqueFormID != null || entityFSTOModal.UniqueFormID != Guid.Empty))
                {
                    IsNeedToUpdate = true;
                    entityFSTOModal.FormVersion = entityFSTOModal.FormVersion + Convert.ToDecimal(0.01);

                    entityFSTOModal.ModifiedBy = CurrentUserID;
                    entityFSTOModal.ModifiedDateTime = Utility.ToDateTimeUtcNow();
                }
                else
                {
                    entityFSTOModal = new Entity.FSTOInspection();
                    entityFSTOModal.UniqueFormID = FSTOFormUniqueId;
                    entityFSTOModal.FormVersion = Convert.ToDecimal(1.00);

                    entityFSTOModal.CreatedBy = CurrentUserID;
                    entityFSTOModal.CreatedDateTime = Utility.ToDateTimeUtcNow();

                    entityFSTOModal.IsDeleted = false;
                }

                entityFSTOModal.IsSynced = false;
                entityFSTOModal.UserGUID = Guid.Parse(UserGUID);
                entityFSTOModal.ShipCode = ShipCode;

                if (!string.IsNullOrEmpty(TravelStartedOn))
                    entityFSTOModal.TravelStartedOn = Convert.ToDateTime(TravelStartedOn);

                if (!string.IsNullOrEmpty(EmbarkedOn))
                    entityFSTOModal.EmbarkedOn = Convert.ToDateTime(EmbarkedOn);

                if (!string.IsNullOrEmpty(DisembarkedOn))
                    entityFSTOModal.DisembarkedOn = Convert.ToDateTime(DisembarkedOn);

                if (!string.IsNullOrEmpty(TravelEndedOn))
                    entityFSTOModal.TravelEndedOn = Convert.ToDateTime(TravelEndedOn);

                entityFSTOModal.Location = Location;
                entityFSTOModal.PurposeOfVisit = PurposeOfVisit;

                if (!IsNeedToUpdate)
                    dbContext.FSTOInspections.Add(entityFSTOModal);

                dbContext.SaveChanges();
                retblnResult = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Insert Or Update FSTO Error : " + ex.ToString());
            }
            return retblnResult;
        }
        // End JSL 02/18/2023

        // JSL 02/18/2023
        public bool InsertOrUpdateFSTOFile(Dictionary<string, string> dictMetaData)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            bool retblnResult = false;

            try
            {
                Guid FileUniqueId = Guid.Empty;
                Guid FSTOFormUniqueId = Guid.Empty;
                string AttachmentName = string.Empty;
                string AttachmentPath = string.Empty;
                string CurrentUserID = string.Empty;

                if (dictMetaData != null)
                {
                    if (dictMetaData.ContainsKey("UniqueID"))
                        FileUniqueId = Guid.Parse(dictMetaData["UniqueID"].ToString());
                    
                    if (dictMetaData.ContainsKey("UniqueFormID"))
                        FSTOFormUniqueId = Guid.Parse(dictMetaData["UniqueFormID"].ToString());

                    if (dictMetaData.ContainsKey("AttachmentName"))
                        AttachmentName = dictMetaData["AttachmentName"].ToString();
                    
                    if (dictMetaData.ContainsKey("AttachmentPath"))
                        AttachmentPath = dictMetaData["AttachmentPath"].ToString();

                    if (dictMetaData.ContainsKey("CurrentUserID"))
                        CurrentUserID = dictMetaData["CurrentUserID"].ToString();

                    var entityModalFSTOAttachment = new Entity.FSTOInspectionAttachment();
                    entityModalFSTOAttachment.UniqueID = FileUniqueId;
                    entityModalFSTOAttachment.UniqueFormID = FSTOFormUniqueId;
                    entityModalFSTOAttachment.AttachmentName = AttachmentName;
                    entityModalFSTOAttachment.AttachmentPath = AttachmentPath;
                    entityModalFSTOAttachment.CreatedBy = Guid.Parse(CurrentUserID);
                    entityModalFSTOAttachment.CreatedDateTime = Utility.ToDateTimeUtcNow();
                    entityModalFSTOAttachment.IsDeleted = false;

                    dbContext.FSTOInspectionAttachments.Add(entityModalFSTOAttachment);
                    dbContext.SaveChanges();

                    GIRHelper.UpdateFormVersion(string.Empty, AppStatic.FSTOForm, Convert.ToString(FSTOFormUniqueId), false);
                }
                retblnResult = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertOrUpdateFSTOFile Error : " + ex.ToString());
            }

            return retblnResult;
        }
        // End JSL 02/18/2023

        public bool UpdateAuditDeficiencies(string id, bool isClose)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid NoteUniqueID = Guid.Parse(id);
            try
            {
                var data = dbContext.AuditNotes.Where(x => x.NotesUniqueID == NoteUniqueID).FirstOrDefault();
                if (data != null)
                {
                    // JSL 06/28/2022 wrapped in if
                    if (isClose)
                    {
                        data.DateClosed = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        data.IsResolved = isClose;
                    }
                    // End JSL 06/28/2022 wrapped in if
                    // JSL 06/28/2022 added else
                    else
                    {
                        data.DateClosed = null;
                        data.IsResolved = isClose;
                    }
                    // End JSL 06/28/2022 added else
                }
                dbContext.SaveChanges();

                // JSL 10/15/2022
                var lstAuditData = dbContext.AuditNotes.Where(x => x.UniqueFormID == data.UniqueFormID && x.isDelete == 0).ToList();
                bool blnIsAuditSetClose = false;
                lstAuditData = lstAuditData.Where(x => x.IsResolved == false).ToList();

                if (lstAuditData != null && lstAuditData.Count > 0)
                    blnIsAuditSetClose = false;
                else
                    blnIsAuditSetClose = true;

                var IAForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == data.UniqueFormID).FirstOrDefault();
                IAForm.IsClosed = blnIsAuditSetClose;
                dbContext.SaveChanges();
                // End JSL 10/15/2022

                // JSL 06/28/2022
                if (isClose)
                {
                    dbContext.Database.ExecuteSqlCommand("UPDATE [AuditNotesComments] SET [isNew] = 0 WHERE [NotesUniqueID] = {0}", NoteUniqueID);
                    dbContext.Database.ExecuteSqlCommand("UPDATE [AuditNotesResolution] SET [isNew] = 0 WHERE [NotesUniqueID] = {0}", NoteUniqueID);

                    dbContext.Database.ExecuteSqlCommand("UPDATE [Notification] SET [ReadDateTime] = '" + Utility.ToDateTimeUtcNow() + "', [IsRead] = {0} WHERE [UniqueDataId] = {1}", 1, NoteUniqueID);
                }
                
                GIRHelper.UpdateFormVersion(Convert.ToString(NoteUniqueID), AppStatic.IAFForm); 
                // JSL 06/28/2022
                
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateAuditDeficiencies : " + ex.Message);
                return false;
            }
        }
        public bool Add_Audit_Deficiency_Comments(Audit_Deficiency_Comments data)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                AuditNotesComment dbComment = new AuditNotesComment();
                dbComment.AuditNoteID = 0; //data.AuditNoteID; //RDBJ 10/05/2021 Commented
                // JSL 01/08/2023 wrapped in if
                if (dbComment.CommentUniqueID == null || dbComment.CommentUniqueID == Guid.Empty)
                    dbComment.CommentUniqueID = Guid.NewGuid();
                dbComment.NotesUniqueID = data.NotesUniqueID;
                dbComment.UserName = data.UserName;
                dbComment.Comment = data.Comment;
                dbComment.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbComment.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbComment.isNew = 0; //RDBJ 10/21/2021
                dbContext.AuditNotesComments.Add(dbComment);
                dbContext.SaveChanges();
                if (data.AuditDeficiencyCommentsFiles != null)
                {
                    SaveAuditDeficiencyCommentFiles(data.AuditDeficiencyCommentsFiles, dbComment.CommentsID, dbComment.CommentUniqueID);
                }
                //RDBJ 10/05/2021
                Entity.AuditNote AudNoteDetails = dbContext.AuditNotes.Where(x => x.NotesUniqueID == data.NotesUniqueID).FirstOrDefault();
                if (AudNoteDetails != null)
                {
                    Entity.InternalAuditForm IAFForm = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == AudNoteDetails.UniqueFormID).FirstOrDefault();
                    IAFForm.FormVersion = IAFForm.FormVersion + Convert.ToDecimal(0.01);
                    IAFForm.IsSynced = false;
                    dbContext.SaveChanges();
                }
                //End RDBJ 10/05/2021

                AudNoteDetails.UpdatedDate = Utility.ToDateTimeUtcNow(); //RDBJ 10/23/2021
                dbContext.SaveChanges(); //RDBJ 10/23/2021
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add_Audit_Deficiency_Comments " + ex.Message + "\n" + ex.InnerException);
                return false;
            }
        }
        public List<Audit_Deficiency_Comments> GetAuditDeficiencyComments(Guid? NoteID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Audit_Deficiency_Comments> Comments = new List<Audit_Deficiency_Comments>();
            try
            {
                List<AuditNotesComment> dbComments = dbContext.AuditNotesComments.Where(x => x.NotesUniqueID == NoteID).ToList();
                if (dbComments != null && dbComments.Count > 0)
                {
                    Comments = dbComments.OrderByDescending(x => x.CreatedDate).Select(x => new Audit_Deficiency_Comments()
                    {
                        CommentsID = x.CommentsID,
                        AuditNoteID = x.AuditNoteID,
                        NotesUniqueID = x.NotesUniqueID,
                        CommentUniqueID = x.CommentUniqueID,
                        UserName = x.UserName,
                        Comment = x.Comment,
                        CreatedDate = x.CreatedDate,
                        isNew = x.isNew, //RDBJ 10/21/2021
                    }).ToList();
                    foreach (var item in Comments)
                    {
                        item.AuditDeficiencyCommentsFiles = GetAuditDeficiencyCommentFilesByCommentID(item.CommentUniqueID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditDeficiencyComments " + ex.Message + "\n" + ex.InnerException);
            }
            return Comments;
        }
        public string SaveAuditDeficiencyCommentFiles(List<Audit_Deficiency_Comments_Files> modal, long commentID, Guid? commentUniqueID)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                foreach (var item in modal)
                {
                    if (item.IsUpload == "true")
                    {
                        // JSL 12/03/2022
                        AuditNotesCommentsFile file = new AuditNotesCommentsFile();
                        file.FileName = item.FileName;
                        file.StorePath = item.StorePath;
                        file.CommentsID = 0;
                        file.AuditNoteID = 0;
                        file.CommentUniqueID = commentUniqueID;
                        file.CommentFileUniqueID = Guid.NewGuid();
                        dbContext.AuditNotesCommentsFiles.Add(file);
                        dbContext.SaveChanges();
                        // End JSL 12/03/2022

                        // JSL 12/03/2022 commented
                        /*
                        var split = item.StorePath.Split(',');
                        string OrignalString = split.LastOrDefault();
                        if (!string.IsNullOrEmpty(OrignalString))
                        {
                            byte[] imageBytes = Convert.FromBase64String(OrignalString);
                            string rootpath = HttpContext.Current.Server.MapPath("~/Deficiencies/AuditDeficiencyComments/");

                            string subPath = item.AuditNoteID.ToString() + "/" + commentID.ToString() + "/";
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

                            AuditNotesCommentsFile file = new AuditNotesCommentsFile();
                            file.FileName = item.FileName;
                            file.StorePath = item.StorePath; //"/Deficiencies/AuditDeficiencyComments/" + subPath + imageName; //RDBJ 10/05/2021 Changes
                            file.CommentsID = 0; //commentID; //RDBJ 10/05/2021 Set 0
                            file.AuditNoteID = 0; //item.AuditNoteID; //RDBJ 10/05/2021 Set 0
                            file.CommentUniqueID = commentUniqueID;
                            file.CommentFileUniqueID = Guid.NewGuid();
                            dbContext.AuditNotesCommentsFiles.Add(file);
                            dbContext.SaveChanges();
                        }
                        */
                        // End JSL 12/03/2022 commented
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SaveImageFileForComments " + ex.Message + "\n" + ex.InnerException);
            }
            return "";
        }
        public List<Audit_Deficiency_Comments_Files> GetAuditDeficiencyCommentFiles(long NoteID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Audit_Deficiency_Comments_Files> CommentFiles = new List<Audit_Deficiency_Comments_Files>();
            try
            {
                List<AuditNotesCommentsFile> dbComments = dbContext.AuditNotesCommentsFiles.Where(x => x.AuditNoteID == NoteID).ToList();
                if (dbComments != null && dbComments.Count > 0)
                {
                    CommentFiles = dbComments.Select(x => new Audit_Deficiency_Comments_Files()
                    {
                        CommentFileID = x.CommentFileID,
                        CommentsID = x.CommentsID,
                        AuditNoteID = x.AuditNoteID,
                        FileName = x.FileName,
                        StorePath = x.StorePath,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditDeficiencyCommentFiles " + ex.Message + "\n" + ex.InnerException);
            }
            return CommentFiles;
        }
        public List<Audit_Deficiency_Comments_Files> GetAuditDeficiencyCommentFilesByCommentID(Guid? CommentUniqueID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Audit_Deficiency_Comments_Files> CommentFiles = new List<Audit_Deficiency_Comments_Files>();
            try
            {
                List<AuditNotesCommentsFile> dbComments = dbContext.AuditNotesCommentsFiles.Where(x => x.CommentUniqueID == CommentUniqueID).ToList();
                if (dbComments != null && dbComments.Count > 0)
                {
                    CommentFiles = dbComments.Select(x => new Audit_Deficiency_Comments_Files()
                    {
                        //CommentFileID = x.CommentFileID,  // JSL 05/17/2022 commented this line
                        //CommentUniqueID = x.CommentUniqueID,  // JSL 05/17/2022 commented this line
                        CommentFileUniqueID = x.CommentFileUniqueID,
                        //CommentsID = x.CommentsID, 
                        //AuditNoteID = x.AuditNoteID,  // JSL 05/17/2022 commented this line
                        FileName = x.FileName,
                        //StorePath = x.StorePath,  // JSL 05/17/2022 commented this line
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditDeficiencyCommentFiles " + ex.Message + "\n" + ex.InnerException);
            }
            return CommentFiles;
        }

        public Dictionary<string, string> GetAuditFile(string FileID) // RDBJ 01/27/2022 set with dictionary
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid FileUniqueID = Guid.Parse(FileID);
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                var data = dbContext.AuditNotesAttachments.Where(x => x.NotesFileUniqueID == FileUniqueID).FirstOrDefault();
                if (data != null)
                {
                    retDicData["FileData"] = data.StorePath; // RDBJ 01/27/2022 set with dictionary
                    retDicData["FileName"] = data.FileName; // RDBJ 01/27/2022 set with dictionary
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditFile " + ex.Message + "\n" + ex.InnerException);
            }
            return retDicData;
        }

        public Dictionary<string, string> GetFileComment(string CommentFileID) // RDBJ 01/27/2022 set with dictionary
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid FileUniqueID = Guid.Parse(CommentFileID);
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                var data = dbContext.AuditNotesCommentsFiles.Where(x => x.CommentFileUniqueID == FileUniqueID).FirstOrDefault();
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

        public int DeleteGIRDeficiencies(string UniqueFormID, string ReportType, string defID) //RDBJ 11/02/2021 Added defID
        {
            int res = 0;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                GIRDeficiency girDef = new GIRDeficiency();

                Guid UFID = new Guid();
                //UFID = Guid.Parse(UniqueFormID); //RDBJ 11/02/2021 commented this line
                UFID = Guid.Parse(defID); //RDBJ 11/02/2021

                //girDef = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UFID && x.ReportType == ReportType && x.No == DefNumber).FirstOrDefault(); //RDBJ 11/02/2021 commented this line
                girDef = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == UFID).FirstOrDefault(); //RDBJ 11/02/2021
                if (girDef != null)
                {
                    girDef.isDelete = 1;
                    dbContext.SaveChanges();

                    ResetGIDeficienciesOrSIActionableItemsNumbersFrom501(Convert.ToString(girDef.UniqueFormID)); // RDBJ 12/13/2021

                    //RDBJ 10/13/2021 wrapped in if
                    if (ReportType.ToUpper() == "GI")
                    {
                        Entity.GeneralInspectionReport girData = new Entity.GeneralInspectionReport();
                        girData = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == girDef.UniqueFormID).FirstOrDefault();
                        girData.FormVersion = girData.FormVersion + Convert.ToDecimal(0.01); //RDBJ 10/13/2021
                        girData.IsSynced = false;
                    }
                    //RDBJ 10/13/2021
                    else
                    {
                        Entity.SuperintendedInspectionReport sirData = new Entity.SuperintendedInspectionReport();
                        sirData = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == girDef.UniqueFormID).FirstOrDefault();
                        sirData.FormVersion = sirData.FormVersion + Convert.ToDecimal(0.01);
                        sirData.IsSynced = false;
                    }
                    //End RDBJ 10/13/2021
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteGIRDeficiencies " + ex.Message + "\n" + ex.InnerException);
            }
            return res;
        }

        //RDBJ 11/02/2021
        public List<int> getDeficienciesDeletedNumbers(string ship, string reportType, string UniqueFormID)
        {
            List<int> availableNumbers = new List<int>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var availableNumbersList = dbContext.SP_Get_GIDeficiencies_OR_SIActionableItems_Number(ship, reportType, 501, 0, UniqueFormID);
                foreach (var item in availableNumbersList)
                {
                    int number;
                    number = Convert.ToInt32(item.AvailableNo);
                    availableNumbers.Add(number);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("getDeficienciesDeletedNumbers :" + ex.Message);
            }
            return availableNumbers;
        }
        //End RDBJ 11/02/2021

        //RDBJ 11/02/2021
        public bool UpdateDeficiencyPriority(string DeficienciesUniqueID, int PriorityWeek
            , string DueDate    // RDBJ 02/28/2022
            )
        {
            bool response = false;
            try
            {
                if (!string.IsNullOrEmpty(DeficienciesUniqueID))
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    Guid UFID = Guid.Parse(DeficienciesUniqueID);

                    GIRDeficiency girDef = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == UFID).FirstOrDefault();
                    if (girDef != null)
                    {
                        girDef.Priority = PriorityWeek;
                        // JSL 07/09/2022 wrapped in if
                        if (!string.IsNullOrEmpty(DueDate))
                            girDef.DueDate = DateTime.ParseExact(DueDate, "dd/MM/yyyy", null); // JSL 07/09/2022 // Convert.ToDateTime(DueDate);   // RDBJ 02/28/2022
                        // End JSL 07/09/2022 wrapped in if
                        dbContext.SaveChanges();

                        if (girDef.ReportType.ToUpper() == "GI")
                        {
                            Entity.GeneralInspectionReport girData = new Entity.GeneralInspectionReport();
                            girData = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == girDef.UniqueFormID).FirstOrDefault();
                            girData.FormVersion = girData.FormVersion + Convert.ToDecimal(0.01); //RDBJ 10/13/2021
                            girData.IsSynced = false;
                        }
                        else
                        {
                            Entity.SuperintendedInspectionReport sirData = new Entity.SuperintendedInspectionReport();
                            sirData = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == girDef.UniqueFormID).FirstOrDefault();
                            sirData.FormVersion = sirData.FormVersion + Convert.ToDecimal(0.01);
                            sirData.IsSynced = false;
                        }
                        dbContext.SaveChanges();
                    }
                    response = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficiencyPriority " + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 11/02/2021

        // RDBJ 12/13/2021
        public void ResetGIDeficienciesOrSIActionableItemsNumbersFrom501(string UniqueFormID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            var defnumbersUpdate = dbContext.SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501(UniqueFormID);
        }
        // End RDBJ 12/13/2021

        //RDBJ 12/17/2021
        public bool UpdateDeficiencyAssignToUser(string DeficienciesUniqueID, string AssignTo
            , bool blnIsIAF   // RDBJ 12/21/2021
            , bool blnIsNeedToDelete   // JSL 07/02/2022
            )
        {
            bool response = false;
            try
            {
                if (!string.IsNullOrEmpty(DeficienciesUniqueID))
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    Guid UFID = Guid.Parse(DeficienciesUniqueID);

                    // RDBJ 12/21/2021 wrapped in if and added if
                    if (blnIsIAF)
                    {
                        Entity.AuditNote auditNote = dbContext.AuditNotes.Where(x => x.NotesUniqueID == UFID).FirstOrDefault();
                        if (auditNote != null)
                        {
                            // JSL 07/02/2022 wrapped in if
                            if (blnIsNeedToDelete)
                            {
                                auditNote.AssignTo = null;
                            }
                            // End JSL 07/02/2022 wrapped in if
                            else
                                auditNote.AssignTo = Guid.Parse(AssignTo);
                            dbContext.SaveChanges();

                            Entity.InternalAuditForm iafData = new Entity.InternalAuditForm();
                            iafData = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == auditNote.UniqueFormID).FirstOrDefault();
                            iafData.FormVersion = iafData.FormVersion + Convert.ToDecimal(0.01);
                            iafData.IsSynced = false;

                            dbContext.SaveChanges();
                        }
                        response = true;
                    }
                    else
                    {
                        GIRDeficiency girDef = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == UFID).FirstOrDefault();
                        if (girDef != null)
                        {
                            // JSL 07/02/2022 wrapped in if
                            if (blnIsNeedToDelete)
                            {
                                girDef.AssignTo = null;
                            }
                            // End JSL 07/02/2022 wrapped in if
                            else
                                girDef.AssignTo = Guid.Parse(AssignTo);
                            dbContext.SaveChanges();

                            if (girDef.ReportType.ToUpper() == "GI")
                            {
                                Entity.GeneralInspectionReport girData = new Entity.GeneralInspectionReport();
                                girData = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == girDef.UniqueFormID).FirstOrDefault();
                                girData.FormVersion = girData.FormVersion + Convert.ToDecimal(0.01); //RDBJ 10/13/2021
                                girData.IsSynced = false;
                            }
                            else
                            {
                                Entity.SuperintendedInspectionReport sirData = new Entity.SuperintendedInspectionReport();
                                sirData = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == girDef.UniqueFormID).FirstOrDefault();
                                sirData.FormVersion = sirData.FormVersion + Convert.ToDecimal(0.01);
                                sirData.IsSynced = false;
                            }
                            dbContext.SaveChanges();
                        }
                        response = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficiencyAssignToUser " + ex.Message + "\n" + ex.InnerException);
            }
            return response;
        }
        //End RDBJ 12/17/2021

        // RDBJ 02/17/2022
        public Dictionary<string, string> CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            GIRDeficiency member = null;
            try
            {
                bool blnIsDeficiencyExist = false;
                bool blnIsDeficiencyInitialActionExist = false;
                string ItemNo = string.Empty;
                string Section = string.Empty;
                string ReportType = string.Empty;
                string Ship = string.Empty;
                Guid UniqueFormID = Guid.Empty;

                if (dicMetadata.ContainsKey("ItemNo"))
                    ItemNo = dicMetadata["ItemNo"].ToString().Trim();

                if (dicMetadata.ContainsKey("Section"))
                    Section = dicMetadata["Section"].ToString().Trim();

                if (dicMetadata.ContainsKey("ReportType"))
                    ReportType = dicMetadata["ReportType"].ToString().Trim();

                if (dicMetadata.ContainsKey("Ship"))
                    Ship = dicMetadata["Ship"].ToString().Trim();

                if (dicMetadata.ContainsKey("UniqueFormID"))
                    UniqueFormID = Guid.Parse(dicMetadata["UniqueFormID"].ToString().Trim());

                if (UniqueFormID != null && UniqueFormID != Guid.Empty)
                {
                    if (!string.IsNullOrEmpty(ItemNo))
                        member = dbContext.GIRDeficiencies.Where(x => x.Section == Section && (x.ItemNo == ItemNo || x.ItemNo == null) && x.UniqueFormID == UniqueFormID && x.isDelete == 0).FirstOrDefault();

                    if (member != null)
                    {
                        blnIsDeficiencyExist = true;
                        dicRetMetadata["DeficienciesUniqueID"] = member.DeficienciesUniqueID.ToString().ToLower();  // RDBJ 02/18/2022

                        GIRDeficienciesInitialAction GIRInitialAction = null;
                        GIRInitialAction = dbContext.GIRDeficienciesInitialActions
                            .Where(x => x.DeficienciesUniqueID == member.DeficienciesUniqueID)
                            .OrderBy(x => x.CreatedDate).FirstOrDefault();

                        if (GIRInitialAction != null)
                            blnIsDeficiencyInitialActionExist = true;
                    }
                }

                dicRetMetadata["IsDeficiencyExist"] = blnIsDeficiencyExist.ToString().ToLower();
                dicRetMetadata["IsDeficiencyInitialActionExist"] = blnIsDeficiencyInitialActionExist.ToString().ToLower();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu " + ex.Message + "\n" + ex.InnerException);
            }
            return dicRetMetadata;
        }
        // End RDBJ 02/17/2022

        // RDBJ 02/18/2022
        public Dictionary<string, string> UpdateCorrectiveAction(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            GIRDeficienciesInitialAction member = null;
            try
            {
                Guid IniActUniqueID = Guid.Empty;
                Guid DeficienciesUniqueID = Guid.Empty;
                string Name = string.Empty;
                string Description = string.Empty;

                if (dicMetadata.ContainsKey("IniActUniqueID"))
                    IniActUniqueID = Guid.Parse(dicMetadata["IniActUniqueID"].ToString().Trim());

                if (dicMetadata.ContainsKey("DeficienciesUniqueID"))
                    DeficienciesUniqueID = Guid.Parse(dicMetadata["DeficienciesUniqueID"].ToString().Trim());

                if (dicMetadata.ContainsKey("Name"))
                    Name = dicMetadata["Name"].ToString().Trim();
                
                if (dicMetadata.ContainsKey("Description"))
                    Description = dicMetadata["Description"].ToString().Trim();

                if (IniActUniqueID != null && IniActUniqueID != Guid.Empty)
                {
                    member = dbContext.GIRDeficienciesInitialActions.Where(x => x.IniActUniqueID == IniActUniqueID).FirstOrDefault();
                    if (member != null)
                    {
                        member.Name = Name;
                        member.Description = Description;
                        member.isNew = 1;
                        dbContext.SaveChanges();

                        GIRDeficiency defDetails = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == member.DeficienciesUniqueID).FirstOrDefault();
                        if (defDetails.ReportType.ToUpper() == "GI")
                        {
                            Entity.GeneralInspectionReport girForm = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                            girForm.FormVersion = girForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                            girForm.IsSynced = false;
                            dbContext.SaveChanges();
                            dicRetMetadata["FormVersion"] = girForm.FormVersion.ToString(); // RDBJ 02/19/2022
                        }
                        else
                        {
                            Entity.SuperintendedInspectionReport sirForm = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == defDetails.UniqueFormID).FirstOrDefault();
                            sirForm.FormVersion = sirForm.FormVersion + Convert.ToDecimal(0.01); //RDBJ 09/22/2021
                            sirForm.IsSynced = false;
                            dbContext.SaveChanges();
                            dicRetMetadata["FormVersion"] = sirForm.FormVersion.ToString(); // RDBJ 02/19/2022
                        }

                        defDetails.UpdatedDate = Utility.ToDateTimeUtcNow();
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCorrectiveAction " + ex.Message + "\n" + ex.InnerException);
            }
            return dicRetMetadata;
        }
        // End RDBJ 02/18/2022

        // JSL 05/10/2022
        #region Common Functions
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
                // JSL 02/18/2023
                case AppStatic.API_INSERTORUPDATEFSTO:
                    {
                        try
                        {
                            bool blnIsActionForUpdate = false;
                            var blnResult = InsertOrUpdateFSTO(dictMetaData);

                            if (dictMetaData.ContainsKey("IsActionForUpdate"))
                                blnIsActionForUpdate = Convert.ToBoolean(dictMetaData["IsActionForUpdate"]);

                            retDictMetaData["IsActionForUpdate"] = Convert.ToString(blnIsActionForUpdate).ToLower();
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_INSERTORUPDATEFSTO + " Error : " + ex.ToString());
                        }
                        break;
                    }
                // End JSL 02/18/2023
                // JSL 02/18/2023
                case AppStatic.API_UPLOADFSTOFILES:
                    {
                        try
                        {
                            var blnResult = InsertOrUpdateFSTOFile(dictMetaData);
                            retDictMetaData = dictMetaData;
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPLOADFSTOFILES + " Error : " + ex.ToString());
                        }
                        break;
                    }
                // End JSL 02/18/2023
                // JSL 02/17/2023
                case AppStatic.API_GETFSTODETAILSBYID:
                    {
                        try
                        {
                            string FSTOFormUniqueId = string.Empty;
                            string strFormType = string.Empty;

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                FSTOFormUniqueId = dictMetaData["UniqueFormID"].ToString();

                            if (dictMetaData.ContainsKey("FormType"))
                                strFormType = dictMetaData["FormType"].ToString();

                            var modalFSTODetails = GetFSTOAuditDataById(FSTOFormUniqueId);
                            if (modalFSTODetails != null)
                            {
                                retDictMetaData["FSTOData"] = JsonConvert.SerializeObject(modalFSTODetails);
                            }

                            retDictMetaData["FormType"] = strFormType;
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_GETFSTODETAILSBYID + " Error : " + ex.ToString());
                        }
                        break;
                    }
                // End JSL 02/17/2023
                case AppStatic.API_DELETEFSTO:
                    {
                        try
                        {
                            Guid UniqueFormID = Guid.Empty;
                            string strCurrentUserID = string.Empty;
                            string strFormType = string.Empty;

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                UniqueFormID = Guid.Parse(dictMetaData["UniqueFormID"].ToString());

                            if (dictMetaData.ContainsKey("CurrentUserID"))
                                strCurrentUserID = dictMetaData["CurrentUserID"].ToString();

                            if (dictMetaData.ContainsKey("FormType"))
                                strFormType = dictMetaData["FormType"].ToString();

                            var fstoForm = dbContext.FSTOInspections.Where(x => x.UniqueFormID == UniqueFormID).FirstOrDefault();
                            if (fstoForm != null)
                            {
                                fstoForm.IsDeleted = true;
                                fstoForm.ModifiedBy = strCurrentUserID;
                                fstoForm.ModifiedDateTime = Utility.ToDateTimeUtcNow();
                                dbContext.SaveChanges();
                            }

                            retDictMetaData = GIRHelper.UpdateFormVersion(string.Empty, AppStatic.FSTOForm, Convert.ToString(UniqueFormID), false);
                            retDictMetaData["UniqueFormID"] = Convert.ToString(UniqueFormID);
                            retDictMetaData["FormType"] = strFormType;
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_DELETEFSTO + " Error : " + ex.ToString());
                        }
                        break;
                    }
                // End JSL 02/17/2023
                // JSL 02/17/2023
                case AppStatic.API_DELETEFSTOFILE:
                    {
                        try
                        {
                            Guid FileUniqueID = Guid.Empty;
                            Guid guidCurrentUserID = new Guid();
                            string strFormType = string.Empty;

                            if (dictMetaData.ContainsKey("FileUniqueID"))
                                FileUniqueID = Guid.Parse(dictMetaData["FileUniqueID"].ToString());

                            if (dictMetaData.ContainsKey("CurrentUserID"))
                                guidCurrentUserID = Guid.Parse(dictMetaData["CurrentUserID"].ToString());

                            if (dictMetaData.ContainsKey("FormType"))
                                strFormType = dictMetaData["FormType"].ToString();

                            var fstoFile = dbContext.FSTOInspectionAttachments.Where(x => x.UniqueID == FileUniqueID).FirstOrDefault();
                            if (fstoFile != null)
                            {
                                fstoFile.IsDeleted = true;
                                fstoFile.ModifiedBy = guidCurrentUserID;
                                fstoFile.ModifiedDateTime = Utility.ToDateTimeUtcNow();
                                dbContext.SaveChanges();
                                
                                retDictMetaData = GIRHelper.UpdateFormVersion(string.Empty, AppStatic.FSTOForm, Convert.ToString(fstoFile.UniqueFormID), false);
                            }

                            retDictMetaData["FileUniqueID"] = Convert.ToString(FileUniqueID);
                            retDictMetaData["FormType"] = strFormType;
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_DELETEFSTOFILE + " Error : " + ex.ToString());
                        }
                        break;
                    }
                // End JSL 02/17/2023
                // JSL 05/10/2022
                case AppStatic.API_DELETEDEFICIENCYFILE:
                    {
                        try
                        {
                            long DeficienciesFileID = 0;
                            Guid DeficienciesUniqueID = Guid.Empty;
                            string strFormType = string.Empty;

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
                                Entity.GIRDeficienciesFile entityDefFile = new Entity.GIRDeficienciesFile();

                                // JSL 06/04/2022 wrraped in if
                                if (IsDeleteFromSection)
                                {
                                    entityDefFile = dbContext.GIRDeficienciesFiles
                                    .Where(x => x.DeficienciesFileUniqueID == deficienciesFileUniqueID).FirstOrDefault();
                                }
                                else
                                {
                                    entityDefFile = dbContext.GIRDeficienciesFiles
                                    .Where(x => x.GIRDeficienciesFileID == DeficienciesFileID).FirstOrDefault();
                                }

                                dbContext.GIRDeficienciesFiles.Remove(entityDefFile);
                                dbContext.SaveChanges();

                                retDictMetaData = GIRHelper.UpdateFormVersion(Convert.ToString(DeficienciesUniqueID));
                            }
                            else
                            {
                                Entity.AuditNotesAttachment entityDefFile = dbContext.AuditNotesAttachments
                                    .Where(x => x.AuditNotesAttachmentId == DeficienciesFileID).FirstOrDefault();

                                dbContext.AuditNotesAttachments.Remove(entityDefFile);
                                dbContext.SaveChanges();

                                Entity.AuditNote entityAuditNote = dbContext.AuditNotes
                                    .Where(x => x.NotesUniqueID == entityDefFile.NotesUniqueID).FirstOrDefault();

                                retDictMetaData = GIRHelper.UpdateFormVersion(string.Empty, AppStatic.IAFForm, Convert.ToString(entityAuditNote.UniqueFormID), false);
                            }

                            // JSL 06/04/2022
                            if (IsDeleteFromSection)
                                retDictMetaData["DeficienciesFileUniqueID"] = dictMetaData["DeficienciesFileUniqueID"].ToString();
                            else
                                retDictMetaData["DeficienciesFileID"] = dictMetaData["DeficienciesFileID"].ToString();

                            retDictMetaData["DeficienciesUniqueID"] = dictMetaData["DeficienciesUniqueID"].ToString();
                            retDictMetaData["IsDeleteFromSection"] = IsDeleteFromSection.ToString().ToLower();
                            // End JSL 06/04/2022

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_DELETEDEFICIENCYFILE + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End JSL 05/10/2022
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
        #endregion
        // End JSL 05/10/2022

        #region ShipApplication
        public List<Deficiency_GISI_Ships> GetShipDeficincyGrid(string code)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Deficiency_GISI_Ships> ShipsList = new List<Deficiency_GISI_Ships>();
            try
            {
                ShipsList = (from G in dbContext.GIRDeficiencies
                             group G by G.Ship into pg
                             join s in dbContext.CSShips on pg.FirstOrDefault().Ship equals s.Code
                             where s.Code != null && s.Code == code
                             select new Deficiency_GISI_Ships
                             {
                                 Ship = s.Code,
                                 ShipName = s.Name,
                                 TotalDeficiencies = pg.Where(x => x.ReportType == "GI" || x.ReportType == "SI").Count(),
                                 TotalOutstending = pg.Where(x => x.IsClose == false || x.IsClose == null).Count(),
                                 GIDeficiencies = pg.Where(x => x.ReportType == "GI").Count(),
                                 OpenGIDeficiencies = pg.Where(x => x.ReportType == "GI" && x.IsClose == false).Count(),
                                 SIDeficiencies = pg.Where(x => x.ReportType == "SI").Count(),
                                 OpenSIDeficiencies = pg.Where(x => x.ReportType == "SI" && x.IsClose == false).Count()
                             }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGISIShips " + ex.Message + "\n" + ex.InnerException);
            }
            return ShipsList;
        }
        public List<Deficiency_Audit_Ships> GetAuditShipsDeficincyGrid(string code)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Deficiency_Audit_Ships> ShipsList = new List<Deficiency_Audit_Ships>();
            try
            {
                ShipsList = (from G in dbContext.AuditNotes
                             group G by G.Ship into pg
                             join s in dbContext.CSShips on pg.FirstOrDefault().Ship equals s.Code
                             where s.Code != null && s.Code == code
                             select new Deficiency_Audit_Ships
                             {
                                 IAFId = pg.FirstOrDefault().InternalAuditFormId,
                                 Ship = s.Code,
                                 ShipName = s.Name,
                                 OpenISMOBS = pg.Where(y => y.Type == "ISM-Observation" && y.Ship == s.Code).Count(),
                                 OpenISMNCNs = pg.Where(y => y.Type == "ISM-Non Conformity" && y.Ship == s.Code).Count(),
                                 OpenISPSOBS = pg.Where(y => y.Type == "ISPS-Observation" && y.Ship == s.Code).Count(),
                                 OpenISPSNCN = pg.Where(y => y.Type == "ISPS-Non Conformity" && y.Ship == s.Code).Count(),
                                 OpenMLCOBS = 0,
                                 OpenMLCNCNs = 0,
                             }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAuditShips " + ex.Message + "\n" + ex.InnerException);
            }
            return ShipsList;
        }
        #endregion
    }
}
