using CarisbrookeOpenFileService.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace CarisbrookeOpenFileService.Helper
{
    public class UpdateServiceHelper
    {
        public void StartUpdate()
        {
            try
            {
                //Check if Update avilable?
                APIHelper _helper = new APIHelper();
                OpenFileServicesReleaseNoteModal LatestAppVersion = _helper.GetLatestOpenFileServicesInfo();
                OpenFileServicesReleaseNoteModal LocalAppVersion = GetLocalOpenFileServicesVersionData();
                if (LatestAppVersion != null && LatestAppVersion.AppId > 0)
                {
                    if (LocalAppVersion == null || (LocalAppVersion != null && LocalAppVersion.AppVersion != LatestAppVersion.AppVersion))
                    {
                        LogHelper.writelog("UpdateServiceHelper : New update avilable");
                        try
                        {
                            foreach (Process proc in Process.GetProcesses())
                            {
                                if (proc.ProcessName.Equals(Process.GetCurrentProcess().ProcessName) && proc.Id == Process.GetCurrentProcess().Id)
                                {
                                    string fileName = Path.GetFileName(proc.MainModule.FileName);
                                    string newVersionDir = Path.GetDirectoryName(proc.MainModule.FileName) + "_" + LatestAppVersion.AppVersion;
                                    string newFileName = Path.Combine(newVersionDir, Path.GetFileName(proc.MainModule.FileName));
                                    if (!Directory.Exists(newVersionDir))
                                        Directory.CreateDirectory(newVersionDir);
                                    if (Utility.DownloadAndUpdatesFiles(newVersionDir, "CarisbrookeOpenFileService.zip"))
                                    {
                                        if (File.Exists(newFileName))
                                        {
                                            try
                                            {
                                                SystemInfoHelper _shiphelper = new SystemInfoHelper();
                                                var objShip = _shiphelper.GetShipJson();
                                                _helper.SubmitOpenFileServicesDownloadLog(new OpenFileServicesDownloadLogModal
                                                {
                                                    DownloadDate = DateTime.Now,
                                                    DownloadedAppId = LatestAppVersion.AppId,
                                                    OldAppId = LatestAppVersion.AppId,
                                                    PCName = objShip.PCName,
                                                    PCUniqueId = objShip.PCUniqueId,
                                                    ShipCode = objShip.Code
                                                });
                                            }
                                            catch (Exception e)
                                            {
                                                LogHelper.writelog("SubmitOpenFileServicesDownloadLog : Failed - " + e.Message);
                                            }
                                            if (InsertOpenFileServicesReleaseNote(LatestAppVersion))
                                            {
                                                Process process = Process.Start(newFileName);
                                            }
                                        }
                                    }    
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateServiceHelper : StartUpdate - " + ex.Message);
            }
        }

        private OpenFileServicesReleaseNoteModal GetLocalOpenFileServicesVersionData()
        {
            OpenFileServicesReleaseNoteModal result = new OpenFileServicesReleaseNoteModal();
            try
            {
                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["OpenfileServiceAppUpdatePath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (File.Exists(jsonFilePath))
                    jsonText = File.ReadAllText(jsonFilePath);
                if (!string.IsNullOrEmpty(jsonText))
                    result = JsonConvert.DeserializeObject<OpenFileServicesReleaseNoteModal>(jsonText);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLocalOpenFileServicesVersionData : StartUpdate - " + ex.Message);
            }
            return result;
        }
        public bool InsertOpenFileServicesReleaseNote(OpenFileServicesReleaseNoteModal modal)
        {
            bool res = false;
            try
            {
                string jsonText = string.Empty;
                string jsonFilePath = ConfigurationManager.AppSettings["OpenfileServiceAppUpdatePath"];
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (modal != null)
                {
                    jsonText = JsonConvert.SerializeObject(modal);
                    File.WriteAllText(jsonFilePath, jsonText);
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertOpenFileServicesReleaseNote : " + ex.Message);
                res = false;
            }
            return res;
        }
    }
}
