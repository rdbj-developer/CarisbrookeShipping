using System;
using System.IO;

namespace AWS_PO_UpdateService.Helper
{
    public class LogHelper
    {
        public static void writelog(string content)
        {
            try
            {
                string logfilePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + DateTime.Now.ToString("ddMMMyyyy") + ".log";
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
        public static string GetEndLine()
        {
            string EndLine = "==============================================================================";
            return EndLine;
        }
    }
}
