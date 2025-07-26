using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class ShipAppReleaseNoteHelper
    {
        public APIResponse AddShipAppReleaseNote(ShipAppReleaseNote Modal) // RDBJ 02/12/2022 set with APIResponse
        {
            APIResponse response = new APIResponse();    // RDBJ 02/12/2022
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                ShipAppReleaseNote dbModal = new ShipAppReleaseNote();
                if (Modal != null)
                {
                    if (Modal.AppId > 0)
                    {
                        dbModal = dbContext.ShipAppReleaseNotes.Where(x => x.AppId == Modal.AppId).FirstOrDefault();
                        dbModal.UpdateDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        response.result = AppStatic.SUCCESS; // RDBJ 02/12/2022
                        response.msg = "Updated Ship Released Note!"; // RDBJ 02/12/2022
                    }
                    else
                    {
                        dbModal = dbContext.ShipAppReleaseNotes.Where(x => x.AppVersion == Modal.AppVersion).FirstOrDefault(); // RDBJ 02/12/2022

                        // RDBJ 02/12/2022 wrapped in if
                        if (dbModal == null)
                        {
                            if (!Modal.AppPublishDate.HasValue)
                                Modal.AppPublishDate = Utility.ToDateTimeUtcNow();

                            Modal.CreatedDate = Utility.ToDateTimeUtcNow();
                            dbContext.ShipAppReleaseNotes.Add(Modal);

                            response.result = AppStatic.SUCCESS; // RDBJ 02/12/2022
                            response.msg = "Added Ship Released Note!"; // RDBJ 02/12/2022
                        }
                        // RDBJ 02/12/2022 added else
                        else
                        {
                            // RDBJ 02/12/2022
                            int intLatestAppVersion = 0;
                            string strLastAppVersion = dbContext.ShipAppReleaseNotes
                                .OrderByDescending(x => x.CreatedDate)
                                .Select(x => x.AppVersion).FirstOrDefault().Replace(".", "");
                            intLatestAppVersion = Convert.ToInt32(strLastAppVersion) + 1;

                            strLastAppVersion = intLatestAppVersion.ToString();

                            var list = Enumerable
                           .Range(0, strLastAppVersion.Length / 1)
                           .Select(i => strLastAppVersion.Substring(i * 1, 1));
                            var res = string.Join(".", list);
                            // End RDBJ 02/12/2022

                            response.result = AppStatic.ERROR; // RDBJ 02/12/2022
                            response.msg = "AppVersion : " + Modal.AppVersion.ToString() + " Already Exist. Please Use Higher Version of Exist! The Next AppVersion is : " + res.ToString() + " Thank You!"; // RDBJ 02/12/2022
                        }
                        
                    }
                    dbContext.SaveChanges();
                    LogHelper.writelog("SubmitShipAppReleaseNote : ShipAppReleaseNote save");
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitShipAppReleaseNote : " + ex.Message + " : " + ex.InnerException);
                response.result = AppStatic.ERROR; // RDBJ 02/12/2022
                response.msg = ex.Message.ToString(); // RDBJ 02/12/2022
            }
            return response;    // RDBJ 02/12/2022
        }

        public ShipAppReleaseNote GetLatestShipAppInfo()
        {
            var ShipAppReleaseNote = new ShipAppReleaseNote();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                ShipAppReleaseNote = dbContext.ShipAppReleaseNotes.OrderByDescending(x => x.AppPublishDate).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLatestShipAppInfo : " + ex.Message + " : " + ex.InnerException);
            }
            return ShipAppReleaseNote;
        }
        public void AddShipAppDownloadLog(ShipAppDownloadLog Modal)
        {
            try
            {
                if (Modal.Id == Guid.Empty)
                    Modal.Id = Guid.NewGuid();
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                dbContext.ShipAppDownloadLogs.Add(Modal);
                dbContext.SaveChanges();
                LogHelper.writelog("SubmitShipAppDownloadLog : ShipAppDownloadLog save");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitShipAppDownloadLog : " + ex.Message + " : " + ex.InnerException);
            }
        }

    }
}
