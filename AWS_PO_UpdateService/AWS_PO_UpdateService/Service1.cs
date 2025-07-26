using AWS_PO_UpdateService.Helper;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;

namespace AWS_PO_UpdateService
{
    public partial class Service1 : ServiceBase
    {
        Timer _timer = new Timer();
        static string _ScheduledRunningTime = "12:00 PM";
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
                //string _CurrentTime = String.Format("{0:t}", DateTime.Now);
                //if (_CurrentTime == _ScheduledRunningTime || isFirstTime)
                //{
                isFirstTime = false;
                LogHelper.writelog("Inteval Start " + DateTime.Now.ToString());
                _timer.Enabled = false;
                StartSync();
                setTimeInterval(120);
                LogHelper.writelog("Inteval End " + DateTime.Now.ToString());
                LogHelper.writelog(LogHelper.GetEndLine());
                //}
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
            LogHelper.writelog("Start Sync...");
            POUpdateHelper _helper = new POUpdateHelper();
            _helper.StartPOUpdate();
            LogHelper.writelog("Sync Done...");
        }
        public void setTimeInterval(Double Minutes)
        {
            _timer.Enabled = false;
            _timer.Interval = Minutes * 60000;
            _timer.Enabled = true;
        }
    }
}
