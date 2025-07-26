using AWS_DB_UpdateService.Helpers;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;

namespace AWS_DB_UpdateService
{
    public partial class Service1 : ServiceBase
    {
        Timer _timer = new Timer();
        string dayOfWeek = Convert.ToString(ConfigurationManager.AppSettings["DayOfWeek"]);
        int hourTime = Convert.ToInt32(ConfigurationManager.AppSettings["Hour"]);
        bool isFirstTime = false;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogHelper.writelog("--------- Service Started At " + DateTime.Now.ToString() + " -----------");
            _timer.Elapsed += _timer_Elapsed;
            isFirstTime = true;
            setTimeInterval(1);
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Check for Time Occur
                var dt = DateTime.Now;
                if ((dt.DayOfWeek.ToString().ToUpper() == dayOfWeek && dt.Hour == hourTime && dt.Minute == 1) || isFirstTime)
                {
                    LogHelper.writelog("Inteval Start " + DateTime.Now.ToString());
                    bool isInternetAvailable = Utility.CheckInternet();
                    if (isInternetAvailable)
                    {
                        LogHelper.writelog("Internet Available and start syncing process....");
                        _timer.Enabled = false;
                        StartSync();
                        isFirstTime = false;
                        setTimeInterval(1);
                    }
                    else
                        LogHelper.writelog("Internet was not Available for syncing process....");
                    LogHelper.writelog("Inteval End " + DateTime.Now.ToString());
                    LogHelper.writelog(LogHelper.GetEndLine());
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("_timer_Elapsed Error : " + DateTime.Now.ToString() + " " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                LogHelper.writelog("Service OnStop called");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Service OnStop error - " + ex);
            }
        }

        public void StartSync()
        {
            TriggerLogHelper _helper = new TriggerLogHelper();
            LogHelper.writelog("Check status started....");
            _helper.Check_Imported_Data_Status();
            LogHelper.writelog("Check status completed....");
            LogHelper.writelog("Sync Started....");
            _helper.ImportLatestData();
            LogHelper.writelog("Sync Completed....");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
            LogHelper.writelog("Check status started after 30 seconds....");
            _helper.Check_Imported_Data_Status();
            LogHelper.writelog("Check status completed....");
        }

        public void setTimeInterval(Double Minutes)
        {
            _timer.Enabled = false;
            _timer.Interval = Minutes * 60000;
            _timer.Enabled = true;
        }
    }
}
