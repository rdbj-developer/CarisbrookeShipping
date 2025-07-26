using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class OpenFileServicesReleaseNoteHelper
    {
        public void AddOpenFileServicesReleaseNote(OpenFileServicesReleaseNote Modal)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                OpenFileServicesReleaseNote dbModal = new OpenFileServicesReleaseNote();
                if (Modal != null)
                {
                    if (Modal.AppId > 0)
                    {
                        dbModal = dbContext.OpenFileServicesReleaseNotes.Where(x => x.AppId == Modal.AppId).FirstOrDefault();
                        dbModal.UpdateDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    else
                    {
                        if (!Modal.AppPublishDate.HasValue)
                            Modal.AppPublishDate = Utility.ToDateTimeUtcNow();
                        Modal.CreatedDate = Utility.ToDateTimeUtcNow();
                        dbContext.OpenFileServicesReleaseNotes.Add(Modal);
                    }
                    dbContext.SaveChanges();
                    LogHelper.writelog("SubmitOpenFileServicesReleaseNote : OpenFileServicesReleaseNote save");
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitOpenFileServicesReleaseNote : " + ex.Message + " : " + ex.InnerException);
            }
        }

        public OpenFileServicesReleaseNote GetLatestOpenFileServicesInfo()
        {
            var OpenFileServicesReleaseNote = new OpenFileServicesReleaseNote();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                OpenFileServicesReleaseNote = dbContext.OpenFileServicesReleaseNotes.OrderByDescending(x => x.AppPublishDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLatestOpenFileServicesInfo : " + ex.Message + " : " + ex.InnerException);
            }
            return OpenFileServicesReleaseNote;
        }
        public void AddOpenFileServicesDownloadLog(OpenFileServicesDownloadLog Modal)
        {
            try
            {
                if (Modal.Id == Guid.Empty)
                    Modal.Id = Guid.NewGuid();
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                dbContext.OpenFileServicesDownloadLogs.Add(Modal);
                dbContext.SaveChanges();
                LogHelper.writelog("SubmitOpenFileServicesDownloadLog : OpenFileServicesDownloadLog save");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitOpenFileServicesDownloadLog : " + ex.Message + " : " + ex.InnerException);
            }
        }

    }
}
