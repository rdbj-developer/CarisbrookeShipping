using CarisbrookeShippingService.BLL.Modals;
using CarisbrookeShippingService.Helpers;
using CarisbrookeShippingService.Modals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Device.Location;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using bllContext = CarisbrookeShippingService.BLL.Helpers;


namespace CarisbrookeShippingService
{
    public partial class Service1 : ServiceBase
    {
        public int _TimeInterval = 5;   // RDBJ 02/26/2022 set default 5mins
        Timer _timer = new Timer();
        public bool IsFirstTimeCall { get; set; }
        public bool IsInspectorInThisMachine { get; set; }  // JSL 11/12/2022
        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            // JSL 11/12/2022
            IsInspectorInThisMachine = false;
            ServerConnectModal dbConnModal = Utility.ReadDBConfigJson();
            IsInspectorInThisMachine = dbConnModal.IsInspector;


            // End JSL 11/12/2022

            // RDBJ 02/26/2022 swapped line
            IsFirstTimeCall = true; 
            SyncServiceStatus(true);
            // End RDBJ 02/26/2022 swapped line

            //LogHelper.writelog("---------CarisbrookShippingService Service Started At " + DateTime.Now.ToString() + " -----------");
            LogHelper.writelog("---------CarisbrookShippingService Service Started At " + Utility.ToDateTimeUtcNow().ToString() + " -----------"); //RDBJ 10/27/2021 set UtcTime
            _timer.Elapsed += _timer_Elapsed;
            setTimeInterval(2);
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //LogHelper.writelog("CarisbrookShippingService -> Inteval Start " + DateTime.Now.ToString());
                LogHelper.writelog("CarisbrookShippingService -> Interval Start " + Utility.ToDateTimeUtcNow().ToString()); //RDBJ 10/27/2021 set UtcTime
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    _timer.Enabled = false;
                    StartSync();

                    //SetTimeInterval();  // JSL 06/27/2022 commeted this line  // RDBJ 02/26/2022

                    // JSL 06/27/2022 wrapped in try..catch
                    try
                    {
                        SetTimeInterval();  // RDBJ 02/26/2022
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("Something went wrong and set default 8 mins time interval....");
                        _TimeInterval = 8;
                    }
                    // End JSL 06/27/2022 wrapped in try..catch

                    LogHelper.writelog("Internet Available and start syncing proces....");

                    IsFirstTimeCall = false;
                    LogHelper.writelog("TimeInterval : " + _TimeInterval);  // RDBJ 02/26/2022

                    //setTimeInterval(5);   // RDBJ 02/26/2022 Commented this line
                    setTimeInterval(_TimeInterval); // RDBJ 02/26/2022

                }
                else
                {
                    LogHelper.writelog("Internet is not available....");    // JSL 06/27/2022
                }
                //LogHelper.writelog("Interval End " + DateTime.Now.ToString());
                LogHelper.writelog("Interval End " + Utility.ToDateTimeUtcNow().ToString()); //RDBJ 10/27/2021 set utcTime
                LogHelper.writelog(LogHelper.GetEndLine());
            }
            catch (Exception ex)
            {
                //LogHelper.writelog("_timer_Elapsed Error : " + DateTime.Now.ToString() + " " + ex.Message);
                LogHelper.writelog("_timer_Elapsed Error : " + Utility.ToDateTimeUtcNow().ToString() + " " + ex.Message); //RDBJ 10/27/2021 set UtcTime
            }
        }
        protected override void OnStop()
        {
            try
            {
                SyncServiceStatus(false);
                LogHelper.writelog("CarisbrookShippingService OnStop called");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CarisbrookShippingService OnStop error - " + ex);
            }
        }
        public void StartSync()
        {
            string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
            if (!string.IsNullOrEmpty(connetionString))
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Start Sync...");
                        ShipAppUpdateHelper _ShipAppUpdatehelper = new ShipAppUpdateHelper();
                        _ShipAppUpdatehelper.IsFirstTimeCall = IsFirstTimeCall; //RDBJ 10/04/2021
                        _ShipAppUpdatehelper.StartShipAppUpdateSync();
                        
                        bllContext.ServiceHelper _ServiceHelper = new bllContext.ServiceHelper();
                        _ServiceHelper.IsFirstTimeCall = IsFirstTimeCall;
                        _ServiceHelper.IsInspectorInThisMachine = IsInspectorInThisMachine; // JSL 11/12/2022
                        _ServiceHelper.StartSync();
                        LogHelper.writelog("Sync Done...");
                    }
                    else
                        LogHelper.writelog("Local Sql Connection Not working...");
                }
            }
            else
                LogHelper.writelog("Connectionstring Not Found...");
        }
        public void setTimeInterval(Double Minutes)
        {
            _timer.Enabled = false;
            _timer.Interval = Minutes * 60000;
            _timer.Enabled = true;
        }

        public void SyncServiceStatus(bool isStart)
        {
            try
            {
                var objShip = Utility.GetShipValue();
                objShip = objShip ?? new SimpleObject();
                string PCUniqueId = Utility.GetPCUniqueId();
                var objShipSystem = new ShipSystemModal
                {
                    CreatedDate = Utility.ToDateTimeUtcNow(), //RDBJ 10/27/2021 set UtcTime
                    PCName = Utility.GetPCName(),
                    PCUniqueId = PCUniqueId,
                    ShipCode = objShip.Code
                };
                APIHelper _apihelper = new APIHelper();
                var res = _apihelper.GetShipSystemByPCId(objShipSystem);

                GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                watcher.Start();
                var whereat = watcher.Position.Location;
                string Lat = "0", Lon = "0";
                if (watcher.TryStart(false, TimeSpan.FromSeconds(3)))
                {
                    if (!whereat.IsUnknown)
                    {
                        Lat = whereat.Latitude.ToString("0.000000");
                        Lon = whereat.Longitude.ToString("0.000000");
                    }
                }
                string ver = ConfigurationManager.AppSettings["AppVersion"];
                var objServiceInfo = new MainSyncServicesEventLogModal
                {
                    IsActive = isStart,
                    RunningVersion = ver,
                    ShipSystemId = res.Id,
                    Latitude = Convert.ToDecimal(Lat),
                    Longitude = Convert.ToDecimal(Lon)
                };
                var result = _apihelper.AddMainSyncServicesEventLog(objServiceInfo);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SyncServiceStatus function call- " + ex.Message + " ...");
            }
        }

        // RDBJ 02/26/2022
        public void SetTimeInterval()
        {
            if (IsFirstTimeCall)
            {
                BLL.Helpers.Utility.GetMainSyncServiceDataAndSaveInMainSyncServiceFile();   // RDBJ 02/26/2022
            }

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
        // End RDBJ 02/26/2022
    }
}
