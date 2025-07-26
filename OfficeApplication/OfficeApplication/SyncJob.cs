using OfficeApplication.BLL.Helpers;
using Quartz;
using System;

namespace OfficeApplication
{
    public class SyncJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                APIHelper _helper = new APIHelper();
                var res = _helper.UpdateAWSUserFromSeacrew();
                if (res)
                    LogHelper.writelog("SyncJob : Successfully Sync AWS Users.");
                else
                    LogHelper.writelog("SyncJob : Failed to Sync AWS Users.");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SyncJob -> Execute -> Error : " + ex.Message);
            }
        }
    }
}