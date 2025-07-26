using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace OfficeApplication.BLL.Helpers
{
    public static class Utility
    {
        public static long ToLong(object value)
        {
            try
            {
                return Convert.ToInt64(value);
            }
            catch { return 0; }
        }
        public static double ToDouble(object value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch { return 0; }
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            DataTable table = new DataTable();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public static void WriteToXLS<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }
        public static bool ExportToExcel(this System.Data.DataTable DataTable, string ExcelFilePath = null)
        {
            bool res = false;
            try
            {
                int ColumnsCount;

                if (DataTable == null || (ColumnsCount = DataTable.Columns.Count) == 0)
                    throw new Exception("ExportToExcel: Null or empty input table!\n");

                // load excel, and create a new workbook
                Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
                Excel.Workbooks.Add();

                // single worksheet
                Microsoft.Office.Interop.Excel._Worksheet Worksheet = Excel.ActiveSheet;

                object[] Header = new object[ColumnsCount];

                // column headings               
                for (int i = 0; i < ColumnsCount; i++)
                    Header[i] = DataTable.Columns[i].ColumnName;

                Microsoft.Office.Interop.Excel.Range HeaderRange = Worksheet.get_Range((Microsoft.Office.Interop.Excel.Range)(Worksheet.Cells[1, 1]), (Microsoft.Office.Interop.Excel.Range)(Worksheet.Cells[1, ColumnsCount]));
                HeaderRange.Value = Header;
                HeaderRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                HeaderRange.Font.Bold = true;

                // DataCells
                int RowsCount = DataTable.Rows.Count;
                object[,] Cells = new object[RowsCount, ColumnsCount];

                for (int j = 0; j < RowsCount; j++)
                    for (int i = 0; i < ColumnsCount; i++)
                        Cells[j, i] = DataTable.Rows[j][i];

                Worksheet.get_Range((Microsoft.Office.Interop.Excel.Range)(Worksheet.Cells[2, 1]), (Microsoft.Office.Interop.Excel.Range)(Worksheet.Cells[RowsCount + 1, ColumnsCount])).Value = Cells;

                // check fielpath
                if (ExcelFilePath != null && ExcelFilePath != "")
                {
                    try
                    {
                        Worksheet.SaveAs(ExcelFilePath);
                        Excel.Quit();
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                else
                {
                    Excel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return res;
        }
        public static string ConvertToDate(dynamic date)
        {
            if (date == null)
                return "";
            else
            {
                return date == null ? "" : ((DateTime)date).ToString("dd/MM/yyyy HH:mm");
            }
        }
        public static int Year()
        {
            return 2018;
        }
        public static double TwoDecimal(object value)
        {
            try
            {
                return Convert.ToDouble((Convert.ToDouble(value)).ToString("F"));
            }
            catch
            {
                return 0.00;
            }
        }
        public static string ToDateTimeStr(object date)
        {
            try
            {
                return (Convert.ToDateTime(date).ToString("dd-MMM-yyyy")).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string ToString(object date)
        {
            try
            {
                return Convert.ToString(date);
            }
            catch
            {
                return string.Empty;
            }
        }
        public static int ToInt(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch { return 0; }
        }
        public static string GetCurrentDateShortString()
        {
            return DateTime.Now.ToString(new System.Globalization.CultureInfo("en-GB")).Substring(0, 10);
        }
        public static T ToEnum<T>(this string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static string ToDateTimeStr(object date, string format = "dd-MMM-yyyy")
        {
            try
            {
                if (date != null)
                    return (Convert.ToDateTime(date).ToString(format)).ToString();
            }
            catch
            {
            }
            return string.Empty;
        }
        public static Stream CreateWorkbook(DataSet ds, List<string> sheetNames)
        {
            try
            {
                XLWorkbook wbook = new XLWorkbook();
                string sheetName = "";
                for (int k = 0; k < ds.Tables.Count; k++)
                {
                    if (sheetNames == null)
                        sheetName = "Sheet" + (k + 1);
                    else
                        sheetName = sheetNames[k];
                    DataTable dt = ds.Tables[k];
                    IXLWorksheet Sheet = wbook.Worksheets.Add(sheetName);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        Sheet.Cell(1, (i + 1)).Value = dt.Columns[i].ColumnName;
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            Sheet.Cell((i + 2), (j + 1)).Value = dt.Rows[i][j].ToString();
                        }
                    }
                }

                Stream spreadsheetStream = new MemoryStream();
                wbook.SaveAs(spreadsheetStream);
                spreadsheetStream.Position = 0;
                return spreadsheetStream;
            }
            catch (Exception ex)
            {

            }
            return null;
            //return new FileStreamResult(spreadsheetStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = fileName };
        }
    }
    public struct DateTimeSpan
    {
        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int Milliseconds { get; }

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            int officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                            if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }
    }
}
