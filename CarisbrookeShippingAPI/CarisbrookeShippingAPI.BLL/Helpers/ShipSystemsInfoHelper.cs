using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class ShipSystemsInfoHelper
    {
        #region ShipSystems
        public void AddShipSystem(ShipSystemModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            ShipSystem dbShipSystem = dbContext.ShipSystems.Where(x => x.PCName == Modal.PCName && x.PCUniqueId == Modal.PCUniqueId).FirstOrDefault();
            if (dbShipSystem == null)
            {
                dbShipSystem = new ShipSystem
                {
                    PCName = Modal.PCName,
                    PCUniqueId = Modal.PCUniqueId,
                    ShipCode = Modal.ShipCode,
                    CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow() //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                };
                dbContext.ShipSystems.Add(dbShipSystem);
            }
            else if (Modal.ShipCode != dbShipSystem.ShipCode)
            {
                dbShipSystem.ShipCode = Modal.ShipCode;
                dbShipSystem.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            }
            dbContext.SaveChanges();
        }

        public ShipSystemModal GetShipSystemByPCId(ShipSystemModal Modal)
        {
            if (string.IsNullOrWhiteSpace(Modal.ShipCode))
                return GetShipSystemByPCName(Modal);

            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            var dbShipSystem = dbContext.ShipSystems.Where(x => x.ShipCode == Modal.ShipCode && x.PCName == Modal.PCName && x.PCUniqueId == Modal.PCUniqueId).FirstOrDefault();
            var objShipSystemModal = new ShipSystemModal();
            bool isNewShip = false;
            if (dbShipSystem == null)
            {
                AddShipSystem(Modal);
                isNewShip = true;
            }
            else
            {
                if (dbShipSystem.IsDeleted.HasValue && dbShipSystem.IsDeleted.Value)
                {
                    dbShipSystem.IsDeleted = false;
                    dbContext.SaveChanges();
                }
            }
            if (isNewShip)
                dbShipSystem = dbContext.ShipSystems.Where(x => x.ShipCode == Modal.ShipCode && x.PCName == Modal.PCName && x.PCUniqueId == Modal.PCUniqueId).FirstOrDefault();
            objShipSystemModal = new ShipSystemModal
            {
                CreatedDate = dbShipSystem.CreatedDate,
                Id = dbShipSystem.Id,
                PCName = dbShipSystem.PCName,
                PCUniqueId = dbShipSystem.PCUniqueId,
                ShipCode = dbShipSystem.ShipCode
            };
            return objShipSystemModal;
        }
        public ShipSystemModal GetShipSystemByPCName(ShipSystemModal Modal)
        {
        GetData:
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            var dbShipSystem = dbContext.ShipSystems.Where(x => x.PCName == Modal.PCName && x.PCUniqueId == Modal.PCUniqueId).FirstOrDefault();
            ShipSystemModal objShipSystemModal = null;
            if (dbShipSystem != null)
            {
                objShipSystemModal = new ShipSystemModal();
                objShipSystemModal = new ShipSystemModal
                {
                    CreatedDate = dbShipSystem.CreatedDate,
                    Id = dbShipSystem.Id,
                    PCName = dbShipSystem.PCName,
                    PCUniqueId = dbShipSystem.PCUniqueId,
                    ShipCode = dbShipSystem.ShipCode
                };
            }
            else
            {
                AddShipSystem(Modal);
                goto GetData;
            }
            return objShipSystemModal;
        }


        public void UploadShipSystemLogs(ShipSystemLog value)
        {

            //Ignore blocked PC entries
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            var dbShipSystems = dbContext.ShipSystems.Where(x => x.ShipCode == value.ShipCode && x.PCName == value.PCName && x.PCUniqueId == value.PCUniqueId && x.IsBlocked == true).FirstOrDefault();
            if (dbShipSystems != null)
                return;

            var basePath = Convert.ToString(ConfigurationManager.AppSettings["ServicesLogPath"]);
            string logfilePath = basePath + value.LogSourceName
                + "\\" + value.ShipCode + "_" + value.ShipName + "\\" + value.PCName + "_" + value.PCUniqueId + "\\" + value.LogFileName;
            Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
            File.WriteAllText(logfilePath, value.LogData);
        }
        #endregion

        #region ShipSystemsInfo
        public void AddShipSystemInfo(ShipSystemsInfoModal Modal)
        {
            Modal.CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            //Ignore blocked PC entries
            var dbShipSystems = dbContext.ShipSystems.Where(x => x.Id == Modal.ShipSystemId && x.IsBlocked == true).FirstOrDefault();
            if (dbShipSystems != null)
                return;

            ShipSystemsInfo dbShipSystemsInfoes = dbContext.ShipSystemsInfoes.Where(x => x.ShipSystemId == Modal.ShipSystemId && x.PropertyName == Modal.PropertyName).FirstOrDefault();
            if (dbShipSystemsInfoes != null && dbShipSystemsInfoes.CreatedDate.Value.Date != Modal.CreatedDate.Value.Date)
                dbShipSystemsInfoes = null;
            if (dbShipSystemsInfoes == null)
            {
                dbShipSystemsInfoes = new ShipSystemsInfo
                {
                    Id = Modal.Id == Guid.Empty ? Guid.NewGuid() : Modal.Id,
                    PropertyName = Modal.PropertyName,
                    PropertyValue = Modal.PropertyValue,
                    ShipSystemId = Modal.ShipSystemId,
                    GroupName = Modal.GroupName,
                    CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    DisplayOrder = Modal.DisplayOrder
                };
                dbContext.ShipSystemsInfoes.Add(dbShipSystemsInfoes);
            }
            else
            {
                dbShipSystemsInfoes.PropertyValue = Modal.PropertyValue;
                dbShipSystemsInfoes.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            }
            dbContext.SaveChanges();
        }
        public void AddShipSystemInfoBulk(List<ShipSystemsInfoModal> Modal)
        {
            if (Modal != null)
            {
                foreach (var item in Modal)
                {
                    try
                    {
                        AddShipSystemInfo(item);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

        }
        #endregion

        #region ShipSystemsEventlog
        public void AddShipSystemEventlog(ShipSystemsEventLogModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            //Ignore blocked PC entries
            var dbShipSystems = dbContext.ShipSystems.Where(x => x.Id == Modal.ShipSystemId && x.IsBlocked == true).FirstOrDefault();
            if (dbShipSystems != null)
                return;
            ShipSystemsEventLog dbShipSystemsEventLogs = dbContext.ShipSystemsEventLogs.Where(x => x.ShipSystemId == Modal.ShipSystemId && x.EventId == Modal.EventId && x.EventDate == Modal.EventDate).FirstOrDefault();
            if (dbShipSystemsEventLogs == null)
            {
                dbShipSystemsEventLogs = new ShipSystemsEventLog
                {
                    Id = Modal.Id == Guid.Empty ? Guid.NewGuid() : Modal.Id,
                    EventDate = Modal.EventDate,
                    EventDescription = Modal.EventDescription,
                    ShipSystemId = Modal.ShipSystemId,
                    EventId = Modal.EventId,
                    EventMachineName = Modal.EventMachineName,
                    EventSource = Modal.EventSource,
                    UserName = Modal.UserName,
                    CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    EventLogType = Modal.EventLogType,
                    EventType = Modal.EventType
                };
                dbContext.ShipSystemsEventLogs.Add(dbShipSystemsEventLogs);
            }
            dbContext.SaveChanges();
        }
        #endregion

        #region ShipSystemsService
        public void AddShipSystemsService(ShipSystemsServiceModal Modal)
        {
            Modal.CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

            //Ignore blocked PC entries
            var dbShipSystems = dbContext.ShipSystems.Where(x => x.Id == Modal.ShipSystemId && x.IsBlocked == true).FirstOrDefault();
            if (dbShipSystems != null)
                return;

            var dbShipSystemsServices = dbContext.ShipSystemsServices.Where(x => x.ShipSystemId == Modal.ShipSystemId && x.Name == Modal.Name).FirstOrDefault();
            if (dbShipSystemsServices != null && dbShipSystemsServices.CreatedDate.Value.Date != Modal.CreatedDate.Value.Date)
                dbShipSystemsServices = null;
            if (dbShipSystemsServices == null)
            {
                dbShipSystemsServices = new ShipSystemsService
                {
                    Id = Modal.Id == Guid.Empty ? Guid.NewGuid() : Modal.Id,
                    ShipSystemId = Modal.ShipSystemId,
                    CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Description = Modal.Description,
                    Name = Modal.Name,
                    ProcessId = Modal.ProcessId,
                    ServiceType = Modal.ServiceType,
                    StartName = Modal.StartName,
                    StartupType = Modal.StartupType,
                    Status = Modal.Status,
                    SystemName = Modal.SystemName,
                    Title = Modal.Title,
                    GroupName = Modal.GroupName
                };
                dbContext.ShipSystemsServices.Add(dbShipSystemsServices);
            }
            else
            {
                dbShipSystemsServices.Description = Modal.Description;
                dbShipSystemsServices.ProcessId = Modal.ProcessId;
                dbShipSystemsServices.ServiceType = Modal.ServiceType;
                dbShipSystemsServices.StartName = Modal.StartName;
                dbShipSystemsServices.StartupType = Modal.StartupType;
                dbShipSystemsServices.Status = Modal.Status;
                dbShipSystemsServices.SystemName = Modal.SystemName;
                dbShipSystemsServices.Title = Modal.Title;
                dbShipSystemsServices.GroupName = Modal.GroupName;
                dbShipSystemsServices.UpdatedDate = Modal.UpdatedDate.HasValue ? Modal.UpdatedDate.Value : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            }
            dbContext.SaveChanges();
        }
        #endregion

        #region ShipSystemsProcess
        public void AddShipSystemsProcess(ShipSystemsProcessModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            //Ignore blocked PC entries
            var dbShipSystems = dbContext.ShipSystems.Where(x => x.Id == Modal.ShipSystemId && x.IsBlocked == true).FirstOrDefault();
            if (dbShipSystems != null)
                return;
            Modal.CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            var dbShipSystemsProcesses = dbContext.ShipSystemsProcesses.Where(x => x.ShipSystemId == Modal.ShipSystemId && x.Name == Modal.Name && x.ProcessId == Modal.ProcessId).FirstOrDefault();
            if (dbShipSystemsProcesses != null && dbShipSystemsProcesses.CreatedDate.Value.Date != Modal.CreatedDate.Value.Date)
                dbShipSystemsProcesses = null;
            if (dbShipSystemsProcesses == null)
            {
                dbShipSystemsProcesses = new ShipSystemsProcess
                {
                    Id = Modal.Id == Guid.Empty ? Guid.NewGuid() : Modal.Id,
                    ShipSystemId = Modal.ShipSystemId,
                    CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime,
                    Description = Modal.Description,
                    Name = Modal.Name,
                    ProcessId = Modal.ProcessId,
                    Title = Modal.Title,
                    MemoryUsage = Modal.MemoryUsage,
                    Status = Modal.Status,
                    Username = Modal.Username
                };
                dbContext.ShipSystemsProcesses.Add(dbShipSystemsProcesses);
            }
            else
            {
                dbShipSystemsProcesses.Description = Modal.Description;
                dbShipSystemsProcesses.MemoryUsage = Modal.MemoryUsage;
                dbShipSystemsProcesses.Status = Modal.Status;
                dbShipSystemsProcesses.Username = Modal.Username;
                dbShipSystemsProcesses.Title = Modal.Title;
                dbShipSystemsProcesses.UpdatedDate = Modal.UpdatedDate.HasValue ? Modal.UpdatedDate.Value : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            }
            dbContext.SaveChanges();
        }
        #endregion

        #region ShipSystemsSoftware
        public void AddShipSystemsSoftware(ShipSystemsSoftwareInfoModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            //Ignore blocked PC entries
            var dbShipSystems = dbContext.ShipSystems.Where(x => x.Id == Modal.ShipSystemId && x.IsBlocked == true).FirstOrDefault();
            if (dbShipSystems != null)
                return;
            Modal.CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            var dbShipSystemsSoftware = dbContext.ShipSystemsSoftwareInfoes.Where(x => x.ShipSystemId == Modal.ShipSystemId && x.Name == Modal.Name).FirstOrDefault();
            if (dbShipSystemsSoftware != null && dbShipSystemsSoftware.CreatedDate.HasValue && dbShipSystemsSoftware.CreatedDate.Value.Date != Modal.CreatedDate.Value.Date)
                dbShipSystemsSoftware = null;
            if (dbShipSystemsSoftware == null)
            {
                dbShipSystemsSoftware = new ShipSystemsSoftwareInfo
                {
                    Id = Modal.Id == Guid.Empty ? Guid.NewGuid() : Modal.Id,
                    ShipSystemId = Modal.ShipSystemId,
                    CreatedDate = Modal.CreatedDate.HasValue ? Modal.CreatedDate.Value : Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    Name = Modal.Name,
                    InstallDate = Modal.InstallDate,
                    InstallLocation = Modal.InstallLocation,
                    ModifyPath = Modal.ModifyPath,
                    Publisher = Modal.Publisher,
                    RepairPath = Modal.RepairPath,
                    UninstallString = Modal.UninstallString,
                    Version = Modal.Version
                };
                dbContext.ShipSystemsSoftwareInfoes.Add(dbShipSystemsSoftware);
            }
            else
            {
                dbShipSystemsSoftware.InstallDate = Modal.InstallDate;
                dbShipSystemsSoftware.InstallLocation = Modal.InstallLocation;
                dbShipSystemsSoftware.ModifyPath = Modal.ModifyPath;
                dbShipSystemsSoftware.Publisher = Modal.Publisher;
                dbShipSystemsSoftware.RepairPath = Modal.RepairPath;
                dbShipSystemsSoftware.UninstallString = Modal.UninstallString;
                dbShipSystemsSoftware.Version = Modal.Version;
                dbShipSystemsSoftware.UpdatedDate = Modal.UpdatedDate.HasValue ? Modal.UpdatedDate.Value : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            }
            dbContext.SaveChanges();
        }
        public List<ShipSystemsSoftwareInfoModal> GetShipSystemsSoftwareAssets(string ShipCode)
        {
            List<ShipSystemsSoftwareInfoModal> dbResult = new List<ShipSystemsSoftwareInfoModal>();
            try
            {
                string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
                using (var conn = new SqlConnection(connetionString))
                {
                    DataTable dt = new DataTable();
                    string Query = "Select CS.Name as ShipName,CS.Code as ShipCode,SW.* from [dbo].[ShipSystemsSoftwareInfo] SW " +
                        " inner join ShipSystems SI on SI.Id = SW.ShipSystemId " +
                        " inner join CSShips CS on CS.Code = SI.ShipCode where CS.Name Not like 'XX%'  ";
                    if (!string.IsNullOrWhiteSpace(ShipCode))
                    {
                        Query += " AND SI.ShipCode='" + ShipCode + "' ";
                    }
                    
                    SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                    sqlAdp.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                        dbResult = dt.ToListof<ShipSystemsSoftwareInfoModal>();
                }
                if (dbResult == null)
                    dbResult = new List<ShipSystemsSoftwareInfoModal>();

                dbResult = dbResult.OrderBy(x => x.ShipName).ThenBy(x => x.Name).ToList();
                return dbResult;

            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipSystemsSoftwareAssets Error : " + ex.Message);
            }
            return dbResult;
        }
        #endregion
    }
}
