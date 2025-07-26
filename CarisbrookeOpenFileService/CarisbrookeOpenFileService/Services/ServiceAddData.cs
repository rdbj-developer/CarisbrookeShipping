using CarisbrookeOpenFileService.Helper;
using CarisbrookeOpenFileService.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Timers;

namespace CarisbrookeOpenFileService.Services
{
    public class ServiceAddData
    {
        public int _TimeInterval = 5;   // RDBJ 03/02/2022 set default 5mins
        Timer _timer = new Timer(100);
        frmManageService objForm;
        private volatile bool _executing;
        public ServiceAddData() {

        }
        public ServiceAddData(frmManageService tempForm)
        {
            objForm = tempForm;
        }
        public void OnStart()
        {
            LogHelper.writelog("---------CarisbrookeOpenFileService Sync Service Started At " + DateTime.Now.ToString() + " -----------");
            _timer.Elapsed += _timer_Elapsed;            
            _timer.Start();
        }
        public void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_executing)
                return;

            _executing = true;
            try
            {
                LogHelper.writelog("Sync Service Inteval Start " + DateTime.Now.ToString());
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    LogHelper.writelog("Internet Available and start syncing proccess....");
                    _timer.Enabled = false;

                    // JSL 10/28/2022 wrapped in try..catch
                    try
                    {
                        SetTimeInterval();  // RDBJ 03/02/2022
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("Something went wrong and set default 8 mins time interval....");
                        _TimeInterval = 8;
                    }
                    // End JSL 10/28/2022 wrapped in try..catch

                   

                    if (StartSync())
                        objForm.NotifyMe();              
                    
                    //setTimeInterval(5); // RDBJ 03/02/2022 commented this line
                    setTimeInterval(_TimeInterval); // RDBJ 03/02/2022
                }
                LogHelper.writelog("Sync Service Inteval End " + DateTime.Now.ToString());
                LogHelper.writelog(LogHelper.GetEndLine());
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Sync Service _timer_Elapsed Error : " + DateTime.Now.ToString() + " " + ex.Message);
            }
            finally
            {
                _executing = false;
            }
        }
        public void OnStop()
        {
            try
            {
                _timer.Enabled = false;
                LogHelper.writelog("Sync Service OnStop called");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Sync Service OnStop error - " + ex);
            }
        }

        public bool StartSync()
        {
            bool isUpdated = false;
            FormsHelper _FormHelper = new FormsHelper();
            isUpdated = _FormHelper.StartFormsync();

            DocumentsHelper _DocumentHelper = new DocumentsHelper();
            _DocumentHelper.StartDocSync();
            LogHelper.writelog("Forms and Documents Sync Done");
            return isUpdated;
        }

        public void setTimeInterval(Double Minutes)
        {
            _timer.Enabled = false;
            _timer.Interval = Minutes * 60000;
            _timer.Enabled = true;
        }

        // RDBJ 03/02/2022
        public void SetTimeInterval()
        {
            string jsonText = string.Empty;
            string jsonFilePath = ConfigurationManager.AppSettings["MainSyncServiceIntervalTime"];
            Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
            if (!System.IO.File.Exists(jsonFilePath))
            {
                File.WriteAllText(jsonFilePath, string.Empty);
            }

            jsonText = System.IO.File.ReadAllText(jsonFilePath);
            Dictionary<string, object> fileDictMetaData = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(jsonText))
            {
                fileDictMetaData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);
                Dictionary<string, string> ServerSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["ServerSettings"].ToString());
                Dictionary<string, string> LocalSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileDictMetaData["LocalSettings"].ToString());

                if (LocalSettings != null && LocalSettings.Count > 0)
                {
                    bool IsNeedToUseLocalSettings = Convert.ToBoolean(LocalSettings["UseServerTimeInterval"].ToString());
                    if (IsNeedToUseLocalSettings)
                        _TimeInterval = Convert.ToInt32(LocalSettings["IntervalTime"].ToString());
                    else
                        _TimeInterval = Convert.ToInt32(ServerSettings["IntervalTime"].ToString());
                }
            }
        }
        // End RDBJ 03/02/2022
    }
}
