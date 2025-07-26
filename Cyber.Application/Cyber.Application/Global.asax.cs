using System.Web.Mvc;
using System.Web.Routing;
namespace OfficeApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //try
            //{
            //    //This Scheduler will Run on Every day at specified time to Sync Seacrew db users to AWS
            //    var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //    scheduler.Start();
            //    IJobDetail job = JobBuilder.Create<SyncJob>().Build();
            //    ITrigger trigger = TriggerBuilder.Create()
            //     .WithIdentity("SyncJob", "Sync")
            //       .WithCronSchedule(Convert.ToString(ConfigurationManager.AppSettings["AWSSyncScheduleTime"]))
            //       .Build();
            //    scheduler.ScheduleJob(job, trigger);
            //    scheduler.Start();
            //}
            //catch (Exception)
            //{ }
        }
    }
}
