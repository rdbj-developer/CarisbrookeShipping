using CarisbrookeShippingService.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace CarisbrookeShippingService.Helpers
{
    public class ShipAppUpdateHelper
    {
        int noOfFilesUpdated = 0;
        string OfficeAPPUrl = System.Configuration.ConfigurationManager.AppSettings["OfficeAPPUrl"];
        public bool IsFirstTimeCall { get; set; } //RDBJ 10/04/2021

        public ShipAppUpdateHelper()
        {
            noOfFilesUpdated = 0;
        }
        public void StartShipAppUpdateSync()
        {
            try
            {
                APIHelper _helper = new APIHelper();
                ShipAppReleaseNoteModal LatestShipAppVersion = _helper.GetLatestShipAppInfo();
                ShipAppReleaseNoteModal LocalShipAppVersion = GetLocalShipAppVersionData();
                if (LatestShipAppVersion != null && LatestShipAppVersion.AppId > 0)
                {
                    if (IsFirstTimeCall || LocalShipAppVersion == null || (LocalShipAppVersion != null && LocalShipAppVersion.AppVersion != LatestShipAppVersion.AppVersion)) //RDBJ 10/04/2021 Added IsFirstTimeCall
                    {
                        LogHelper.writelog("Ship app update found...");
                        //GetLatest Files and update it.
                        if (DownloadLatestSetup())
                        {
                            //Update Download Log
                            var objShip = Utility.GetShipValue();
                            objShip = objShip ?? new SimpleObject();
                            _helper.SubmitShipAppDownloadLog(new ShipAppDownloadLogModal
                            {
                                DownloadDate = Utility.ToDateTimeUtcNow(), //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                DownloadedAppId = LatestShipAppVersion.AppId,
                                OldAppId = LocalShipAppVersion.AppId,
                                PCName = Utility.GetPCName(),
                                PCUniqueId = Utility.GetPCUniqueId(),
                                ShipCode = objShip.id
                            });
                            //UpdateLocal Version Info
                            InsertShipAppReleaseNote(LatestShipAppVersion);
                            BLL.Helpers.Utility.GetMainSyncServiceDataAndSaveInMainSyncServiceFile(true);   // JSL 11/12/2022
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("StartShipAppUpdateSync : Ship app update failed..." + ex.Message);
            }
        }
        public ShipAppReleaseNoteModal GetLocalShipAppVersionData()
        {
            ShipAppReleaseNoteModal SyncData = new ShipAppReleaseNoteModal();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                        conn.Open();
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd = new SqlCommand(GetShipAppReleaseNoteTableQuery(), conn);
                        cmd.ExecuteNonQuery();

                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT top 1 * FROM " + AppStatic.ShipAppReleaseNote + " ORDER BY AppPublishDate DESC", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            List<ShipAppReleaseNoteModal> AppUpdateList = dt.ToListof<ShipAppReleaseNoteModal>();
                            if (AppUpdateList != null)
                                SyncData = AppUpdateList.FirstOrDefault();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetShipAppUpdateUnsyncedData " + ex.Message);
            }
            return SyncData;
        }

        private string GetShipAppReleaseNoteTableQuery()
        {
            string tableQuery = @"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = N'ShipAppReleaseNote' AND type = 'U')
                                CREATE TABLE [dbo].[ShipAppReleaseNote](
                                	[AppId] [bigint] IDENTITY(1,1) NOT NULL,
                                	[AppVersion] [varchar](50) NULL,
                                	[AppDescription] [varchar](max) NULL,
                                	[AppPublishDate] [datetime] NULL,
                                	[NoOfFilesAffected] [int] NULL,
                                	[CreatedDate] [datetime] NULL,
                                	[UpdateDate] [datetime] NULL)";
            return tableQuery;
        }

        public bool InsertShipAppReleaseNote(ShipAppReleaseNoteModal modal)
        {
            bool res = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(Utility.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string Query = @"INSERT INTO [dbo].[ShipAppReleaseNote]([AppVersion],[AppDescription],[AppPublishDate],[NoOfFilesAffected],[CreatedDate])
                                        VALUES(@AppVersion,@AppDescription,@AppPublishDate,@NoOfFilesAffected,@CreatedDate)";
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(Query, connection);
                        connection.Open();
                        command.Parameters.Add("@AppVersion", SqlDbType.VarChar).Value = modal.AppVersion ?? (object)DBNull.Value;
                        command.Parameters.Add("@AppDescription", SqlDbType.VarChar).Value = modal.AppDescription ?? (object)DBNull.Value;
                        command.Parameters.Add("@AppPublishDate", SqlDbType.DateTime).Value = modal.AppPublishDate ?? (object)DBNull.Value;
                        command.Parameters.Add("@NoOfFilesAffected", SqlDbType.Int).Value = modal.NoOfFilesAffected ?? (object)DBNull.Value;
                        command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        object resultObj = command.ExecuteScalar();
                        long databaseID = 0;
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out databaseID);
                        }
                        if (databaseID > 0)
                            res = true;
                        connection.Close();
                        LogHelper.writelog("InsertShipAppReleaseNote : Latest version data dump to ShipAppReleaseNote ");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetLocalDocs : " + ex.Message);
                res = false;
            }
            return res;
        }

        public bool DownloadLatestSetup()
        {
            bool res = false;
            try
            {
                string ShipAPPUrl = System.Configuration.ConfigurationManager.AppSettings["ShipAPPLocalPath"];
                //LogHelper.writelog("ShipApplication Publish Start....... " + DateTime.Now.ToString());
                LogHelper.writelog("ShipApplication Publish Start....... " + Utility.ToDateTimeUtcNow().ToString()); //RDBJ 10/27/2021 set UtcTime
                //Download ShipApplication
                res = DownloadAndUpdatesFiles(ShipAPPUrl, "ShipApplication.zip");
                LogHelper.writelog("ShipApplication Publish End with Status: " + Utility.ToString(res));                
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CreatePhysicalLocation " + ex.Message);
                res = false;
            }
            return res;
        }

        public bool DownloadAndUpdatesFiles(string destinationPath, string fileName)
        {
            bool res = false;
            try
            {
                string DownloadUrl = @"" + OfficeAPPUrl + "Service\\" + fileName;
                //string prefixLocalPath = Path.GetTempPath() + Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmm");
                string prefixLocalPath = Path.GetTempPath() + Guid.NewGuid().ToString() + Utility.ToDateTimeUtcNow().ToString("ddMMyyyyHHmm"); //RDBJ 10/27/2021 set UtcTime
                //string prefixLocalPath = @"C:\inetpub\wwwroot\LatestShipApplication\" + Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHHmm"); //RDBJ 10/05/2021 Commented this is use for VC if not update latest

                string downloadLocalPath = @"" + prefixLocalPath + "\\" + fileName; //RDBJ 10/05/2021 Added @"" +
                Directory.CreateDirectory(Path.GetDirectoryName(downloadLocalPath));

                //Download updates from Server to Local
                WebClient wc = new WebClient();
                wc.DownloadFile(DownloadUrl, downloadLocalPath);

                //Extract locally downloaded updates
                ZipFile.ExtractToDirectory(downloadLocalPath, prefixLocalPath);

                fileName = fileName.Replace(".zip", "");
                //Copy downloaded updates to published location
                prefixLocalPath = prefixLocalPath + "\\" + fileName;
                //Force clean up
                CloneDirectory(prefixLocalPath, destinationPath);

                LogHelper.writelog("CloneDirectory Updated no. Of Files : " + noOfFilesUpdated); //RDBJ 10/04/2021

                //Remove downloaded updates
                if (File.Exists(downloadLocalPath))
                    File.Delete(downloadLocalPath);
                Directory.Delete(prefixLocalPath, true);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DownloadAndUpdatesFiles " + ex.Message);
                res = false;
            }
            return res;
        }

        public void CloneDirectory(string root, string dest)
        {
            //RDBJ 10/04/2021 Added try and Catch 
            try
            {
                foreach (var directory in Directory.GetDirectories(root))
                {
                    string dirName = Path.GetFileName(directory);
                    if (!Directory.Exists(Path.Combine(dest, dirName)))
                    {
                        Directory.CreateDirectory(Path.Combine(dest, dirName));
                    }
                    CloneDirectory(directory, Path.Combine(dest, dirName));
                }

                foreach (var file in Directory.GetFiles(root))
                {
                    File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
                    noOfFilesUpdated++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CloneDirectory " + ex.Message); 
            }
        }
    }
}
