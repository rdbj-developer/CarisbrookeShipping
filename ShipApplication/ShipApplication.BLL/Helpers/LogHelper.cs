using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Helpers
{
    public class LogHelper
    {
        public static void writelog(string content)
        {
            try
            {
                //string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + DateTime.Now.ToString("ddMMMyyyy") + ".log";
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log"; //RDBJ 10/27/2021 set UtcTime
                Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
                FileStream fs = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(content + " At " + Utility.ToDateTimeUtcNow()); //RDBJ 10/27/2021 set UtcTime
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
                sw.WriteLine("Status Changed DateTime : " + Utility.ToDateTimeUtcNow().ToString("dd-MMM-yyyy HH:mm:ss"));
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        // End RDBJ 12/21/2021

        // RDBJ 12/30/2021
        public static void LogForPerformClosedDeleted(string ActionaName, string ID, string Username, string Status)
        {
            try
            {
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + @"Logs\LogFor" + ActionaName + @"PerformClosedDeleted\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log";
                Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
                FileStream fs = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine("--------------------------------------------------------------------------------------------------------");
                sw.WriteLine(ActionaName + " ID : " + ID);
                sw.WriteLine("Username : " + Username);
                sw.WriteLine("Status Changed : " + Status);
                sw.WriteLine("Status Changed DateTime : " + Utility.ToDateTimeUtcNow().ToString("dd-MMM-yyyy HH:mm:ss"));
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        // End RDBJ 12/30/2021
    }
}
