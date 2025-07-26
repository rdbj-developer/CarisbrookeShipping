using DBModificationService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DBModificationService
{
    public partial class Service1 : ServiceBase
    {
        Timer _timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogHelper.writelog("--------- Service Started At " + DateTime.Now.ToString() + " -----------");
            _timer.Elapsed += _timer_Elapsed;
            setTimeInterval(10);
        }

        protected override void OnStop()
        {
            try
            {
                LogHelper.writelog("Service OnStop called");
                _timer.Enabled = false;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Service OnStop error - " + ex);
            }
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                LogHelper.writelog("New Interval Start " + DateTime.Now.ToString());
                StartSync();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("_timer_Elapsed Error : " + DateTime.Now.ToString() + " " + ex.Message);
            }
        }
        public void StartSync()
        {
            TableHelper _helper = new TableHelper();
            _helper.GetLatestData();
            AWSUsersHelper _ahelper = new AWSUsersHelper();
            _ahelper.SyncAwsUsers();
            LogHelper.writelog("DB Sync End " + DateTime.Now.ToString());
            LogHelper.writelog(LogHelper.GetEndLine());
        }
        public void setTimeInterval(Double Minutes)
        {
            _timer.Enabled = false;
            _timer.Interval = Minutes * 60000;
            _timer.Enabled = true;
        }
    }
}
