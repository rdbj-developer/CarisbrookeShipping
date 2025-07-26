using System;
using System.IO;

namespace OfficeApplication.BLL.Helpers
{
    public class LogHelper
    {
        public static void writelog(string content)
        {
            try
            {
                //string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + DateTime.Now.ToString("ddMMMyyyy") + ".log";
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log"; //RDBJ 10/27/2021 set utcTime
                Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
                FileStream fs = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(content);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
            }
        }

        // RDBJ 12/21/2021
        public static void LogForDeficienciesClose(string DeficienciyID, string Username, string Status)
        {
            try
            {
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + @"Logs\LogForDeficienciesClose\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log";
                Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
                FileStream fs = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine("--------------------------------------------------------------------------------------------------------");
                sw.WriteLine("Deficiency ID : " + DeficienciyID);
                sw.WriteLine("Username : " + Username);
                sw.WriteLine("Status Changed : " + Status);
                sw.WriteLine("Status Changed DateTime : " + Utility.ToDateTimeUtcNow().ToString("dd-MMM-yyyy HH:mm:ss")); // RDBJ 12/29/2021 set 24 hours clock
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        // End RDBJ 12/21/2021

        // RDBJ 12/29/2021
        public static void LogForHelpAndSupportClose(string ID, string Username, string Status)
        {
            try
            {
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + @"Logs\LogForHelpAndSupportClose\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log";
                Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
                FileStream fs = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine("--------------------------------------------------------------------------------------------------------");
                sw.WriteLine("Help&Support ID : " + ID);
                sw.WriteLine("Username : " + Username);
                sw.WriteLine("Status Changed : " + Status);
                sw.WriteLine("Status Changed DateTime : " + Utility.ToDateTimeUtcNow().ToString("dd-MMM-yyyy HH:mm:ss")); // RDBJ 12/29/2021 set 24 hours clock
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        // End RDBJ 12/29/2021
    }
}
