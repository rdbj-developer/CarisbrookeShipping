using CarisbrookeOpenFileService.Helper;
using CarisbrookeOpenFileService.WinForms;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CarisbrookeOpenFileService
{
    class Program
    {
        // private static Timer aTimer;
        static void Main(string[] args)
        {
            string AppVersion = ConfigurationManager.AppSettings["AppVersion"];
            try
            {
                LogHelper.writelog("Carisbrooke File Service (Version: " + AppVersion + ") running...");
                try
                {
                    foreach (Process proc in Process.GetProcesses())
                    {
                        if (proc.ProcessName.Equals(Process.GetCurrentProcess().ProcessName) && proc.Id != Process.GetCurrentProcess().Id)
                        {
                            string fileName = Path.GetFileName(proc.MainModule.FileName);
                            string oldVersionDir = Path.GetDirectoryName(proc.MainModule.FileName);
                            proc.Kill();
                            try
                            {
                                //if (Directory.Exists(oldVersionDir)) Directory.Delete(oldVersionDir, true);   // JSL 12/20/2022 commented
                            }
                            catch (Exception e)
                            { LogHelper.writelog(" Main : Failed to delete OLD service setup:--" + e.Message); }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog(" Failed to kill process:--" + ex.Message);
                    LogHelper.writelog(" Failed to kill process InnerException :--" + ex.InnerException ?? ex.InnerException.Message);
                    if (IsApplicationAlreadyRunning())
                    {
                        MessageBox.Show("Another instance of this application is running. Please close that service before starting new.");
                        Environment.Exit(0);
                    }
                }
                Form form;
                form = new frmManageService();

                Application.EnableVisualStyles();
                Application.Run(form);

                new ManualResetEvent(false).WaitOne();

            }
            catch (Exception ex)
            {
                
                var notification = new NotifyIcon()
                {
                    Visible = true,
                    Icon = System.Drawing.SystemIcons.Information,
                    Text = "Carisbrooke File Service (Version: " + AppVersion + ")",
                    BalloonTipTitle = "Carisbrooke File Service (Version: " + AppVersion + ")",
                    BalloonTipText = "Service is failed due to : " + ex.Message,
                };
                // Display for 5 seconds.
                notification.ShowBalloonTip(10000);
                LogHelper.writelog(" Error:--" + ex.Message);
            }
        }

        static bool IsApplicationAlreadyRunning()
        {
            string proc = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(proc);
            if (processes.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
