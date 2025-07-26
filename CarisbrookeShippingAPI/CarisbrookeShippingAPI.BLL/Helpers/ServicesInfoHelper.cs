using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class ServicesInfoHelper
    {
        #region GeneralSettings
        public void AddGeneralSettings(GeneralSettingsModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            GeneralSetting dbGeneralSetting = dbContext.GeneralSettings.Where(x => x.Code == Modal.Code).FirstOrDefault();
            if (dbGeneralSetting == null)
            {
                dbGeneralSetting = new GeneralSetting
                {
                    Code = Modal.Code,
                    Description = Modal.Description,
                    IsActive = Modal.IsActive,
                    Text = Modal.Text,
                    Value = Modal.Value
                };
                dbContext.GeneralSettings.Add(dbGeneralSetting);
            }
            dbContext.SaveChanges();
        }
        #endregion
        #region MainSyncServicesEventLog
        public void AddMainSyncServicesEventLog(MainSyncServicesEventLogModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            //Ignore blocked PC entries
            //var dbShipSystems = dbContext.ShipSystems.Where(x => x.Id == Modal.ShipSystemId && x.IsBlocked == true).FirstOrDefault();
            //if (dbShipSystems != null)
            //    return;
            MainSyncServicesEventLog dbMainSyncServicesEventLog = new MainSyncServicesEventLog
            {
                Id = Guid.NewGuid(),
                IsActive = Modal.IsActive,
                LastUpdateDate = Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Latitude = Modal.Latitude,
                Longitude = Modal.Longitude,
                RunningVersion = Modal.RunningVersion,
                ShipSystemId = Modal.ShipSystemId
            };
            dbContext.MainSyncServicesEventLogs.Add(dbMainSyncServicesEventLog);
            dbContext.SaveChanges();
        }
        #endregion
        #region OpenFileServicesEventLog
        public void AddOpenFileServicesEventLog(OpenFileServicesEventLogModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            //Ignore blocked PC entries
            //var dbShipSystems = dbContext.ShipSystems.Where(x => x.Id == Modal.ShipSystemId && x.IsBlocked == true).FirstOrDefault();
            //if (dbShipSystems != null)
            //    return;
            OpenFileServicesEventLog dbOpenFileServicesEventLog = new OpenFileServicesEventLog
            {
                Id = Guid.NewGuid(),
                IsActive = Modal.IsActive,
                LastUpdateDate = Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Latitude = Modal.Latitude,
                Longitude = Modal.Longitude,
                RunningVersion = Modal.RunningVersion,
                ShipSystemId = Modal.ShipSystemId
            };
            dbContext.OpenFileServicesEventLogs.Add(dbOpenFileServicesEventLog);
            dbContext.SaveChanges();
        }
        #endregion
    }
}
