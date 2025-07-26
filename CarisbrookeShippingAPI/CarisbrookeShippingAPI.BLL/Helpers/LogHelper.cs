using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class LogHelper
    {
        public static void writelog(string content)
        {
            try
            {
                //string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + DateTime.Now.ToString("ddMMMyyyy") + ".log";
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log";
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
                LogHelper.writelog(ex.Message);
            }
        }

        // JSL 10/15/2022
        public static void LogForDeficienciesClose(string strFormType, string DeficienciyID, string Username, string Status)
        {
            try
            {
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + @"Logs\LogForDeficienciesClose\" + Utility.ToDateTimeUtcNow().ToString("ddMMMyyyy") + ".log";
                Directory.CreateDirectory(Path.GetDirectoryName(logfilePath));
                FileStream fs = new FileStream(logfilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine("--------------------------------------------------------------------------------------------------------");
                sw.WriteLine("Form : " + strFormType);
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
        // End JSL 10/15/2022
    }
}
