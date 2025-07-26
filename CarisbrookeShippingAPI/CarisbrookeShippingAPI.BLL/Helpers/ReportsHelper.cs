using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class ReportsHelper
    {
        public List<SMV_BUDGET_OVERVIEW_1> Get_Reports_Data()
        {
            List<SMV_BUDGET_OVERVIEW_1> list = new List<SMV_BUDGET_OVERVIEW_1>();

            using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
            {
                // list = dbContext.Database.SqlQuery<SMV_ACCOUNT_RECONCILATION_RPT>("sp_GetReportsDataList").ToList();
                list = dbContext.Database.SqlQuery<SMV_BUDGET_OVERVIEW_1>("sp_GetReportsData").ToList();
            }
            return list;
        }
        public List<SMV_ACCOUNT_RECONCILATION_RPT> Get_Reports_DataList()
        {
            List<SMV_ACCOUNT_RECONCILATION_RPT> list = new List<SMV_ACCOUNT_RECONCILATION_RPT>();
            using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
            {
                // list = dbContext.Database.SqlQuery<SMV_ACCOUNT_RECONCILATION_RPT>("sp_GetReportsDataList").ToList();
                list = dbContext.Database.SqlQuery<SMV_ACCOUNT_RECONCILATION_RPT>("sp_GetReportsData").ToList();
            }
            return list;
        }
        public List<Invoice> Get_Reports_DataInvoice()
        {
            List<Invoice> list = new List<Invoice>();
            using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
            {
                list = dbContext.Database.SqlQuery<Invoice>("GetInvoiceReportData").ToList();
            }
            return list;
        }
        public List<PurchaseOrder> Get_Reports_DataPurchase()
        {
            List<PurchaseOrder> list = new List<PurchaseOrder>();
            using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
            {
                list = dbContext.Database.SqlQuery<PurchaseOrder>("spGetPurchaseOrderData").ToList();
            }
            return list;
        }

        public UALOpexFormula Get_Reports_UALOpexFormula(DateTime startdate, DateTime enddate)
        {
            UALOpexFormula list = new UALOpexFormula();
            List<ActualInvoices> ActualInvoices = new List<ActualInvoices>();
            List<InvoicesReceivedCurrentMonth> InvoicesReceivedCurrentMonth = new List<InvoicesReceivedCurrentMonth>();
            List<POsCurrentMonth> POsCurrentMonth = new List<POsCurrentMonth>();
            List<OpenPOsPreviousMonths> OpenPOsPreviousMonths = new List<OpenPOsPreviousMonths>();
            List<PreviousMonthPObalance> PreviousMonthPObalance = new List<PreviousMonthPObalance>();
            using (CASHIP_WebGUIEntities dbContext = new CASHIP_WebGUIEntities())
            {
                //sp_GetUALOpexFormula_ActualInvoices
                //    sp_GetUALOpexFormula_InvoicesReceivedCurrentMonth
                //    sp_GetUALOpexFormula_OpenPOsCurrentMontn
                //    sp_GetUALOpexFormula_OpenPOsPreviousMonths
                //    sp_GetUALOpexFormula_PreviousMonthPObalance

                ActualInvoices = dbContext.Database.SqlQuery<ActualInvoices>("sp_GetUALOpexFormula_ActualInvoices @StartDate, @EndDate", new SqlParameter("StartDate", startdate), new SqlParameter("EndDate", enddate)).ToList();
                InvoicesReceivedCurrentMonth = dbContext.Database.SqlQuery<InvoicesReceivedCurrentMonth>("sp_GetUALOpexFormula_InvoicesReceivedCurrentMonth @StartDate, @EndDate", new SqlParameter("StartDate", startdate), new SqlParameter("EndDate", enddate)).ToList();
                POsCurrentMonth = dbContext.Database.SqlQuery<POsCurrentMonth>("sp_GetUALOpexFormula_OpenPOsCurrentMontn @StartDate, @EndDate", new SqlParameter("StartDate", startdate), new SqlParameter("EndDate", enddate)).ToList();
                OpenPOsPreviousMonths = dbContext.Database.SqlQuery<OpenPOsPreviousMonths>("sp_GetUALOpexFormula_OpenPOsPreviousMonths @StartDate, @EndDate", new SqlParameter("StartDate", startdate), new SqlParameter("EndDate", enddate)).ToList();

                int Year = startdate.Year;
                int Month = startdate.Month;
                if (Month == 1)
                {
                    Year = Year - 1;
                    Month = 12;
                }
                else
                {
                    Month = Month - 1;
                    Year = Year;
                }

                DateTime prevMonthStartDate = GetStartDate(Year, Month);
                DateTime prevMonthEndDate = GetEndDate(Year, Month);

                PreviousMonthPObalance = dbContext.Database.SqlQuery<PreviousMonthPObalance>("sp_GetUALOpexFormula_PreviousMonthPObalance @StartDate, @EndDate", new SqlParameter("StartDate", prevMonthStartDate), new SqlParameter("EndDate", prevMonthEndDate)).ToList();
            }
            list.ActualInvoices = ActualInvoices;
            list.InvoicesReceivedCurrentMonth = InvoicesReceivedCurrentMonth;
            list.POsCurrentMonth = POsCurrentMonth;
            list.OpenPOsPreviousMonths = OpenPOsPreviousMonths;
            list.PreviousMonthPObalance = PreviousMonthPObalance;
            return list;
        }
        public DateTime GetStartDate(int Year, int Month)
        {
            return new DateTime(Year, Month, 1);
        }
        public DateTime GetEndDate(int Year, int Month)
        {
            switch (Month)
            {
                case 1:
                    return new DateTime(Year, Month, 31);
                case 2:
                    return new DateTime(Year, Month, 28);
                case 3:
                    return new DateTime(Year, Month, 31);
                case 4:
                    return new DateTime(Year, Month, 30);
                case 5:
                    return new DateTime(Year, Month, 31);
                case 6:
                    return new DateTime(Year, Month, 30);
                case 7:
                    return new DateTime(Year, Month, 31);
                case 8:
                    return new DateTime(Year, Month, 31);
                case 9:
                    return new DateTime(Year, Month, 30);
                case 10:
                    return new DateTime(Year, Month, 31);
                case 11:
                    return new DateTime(Year, Month, 30);
                case 12:
                    return new DateTime(Year, Month, 31);
            }
            return Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
        }
        public OpexReportFinilizeDate UpdateOpexReportDate(OpexReportFinilizeDate date)
        {
            OpexReportFinilizeDate data = new OpexReportFinilizeDate();
            using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
            {
                date.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbContext.OpexReportFinilizeDates.Add(date);
                dbContext.SaveChanges();
                return date;
            }
        }
        public DateTime? GetLastFinilizeReportDate()
        {
            OpexReportFinilizeDate data = new OpexReportFinilizeDate();
            using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
            {
                DateTime? dt = dbContext.OpexReportFinilizeDates.OrderByDescending(x => x.CreatedDate).Select(x => x.EndDate).FirstOrDefault();
                if (dt == null)
                    dt = new DateTime(2018, 8, 1);

                return dt;
            }

        }
        public List<AccountCodeData> GetAccountCodeList()
        {
            List<AccountCodeData> list = new List<AccountCodeData>();
            using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
            {
                var data = dbContext.ACCOUNTCODEs.ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        AccountCodeData obj = new AccountCodeData();
                        obj.ACCOUNT_CODE = item.ACCOUNT_CODE;
                        obj.ACCOUNT_DESCR = item.ACCOUNT_DESCR;
                        list.Add(obj);
                    }
                }
            }
            return list;
        }
        public List<ShipReportsAnalysisModal> GetAllShipReportsData(ShipModal Modal)
        {
            List<ShipReportsAnalysisModal> list = new List<ShipReportsAnalysisModal>();
            using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
            {
                string sqlQuery = "Select * FROM( " +
                " Select ARID as ID,ShipName,'Arrival' as ReportName,VoyageNo,PortName,ReportCreated as CreatedDate ,'' as Inspector,'' as Superintended from[dbo].[ArrivalReports]  " +
                " union  " +
                 " select DCRID as ID,ShipName,'Daily Cargo' as ReportName,VoyageNo,PortName,ReportCreated as CreatedDate  ,'' as Inspector,'' as Superintended from[dbo].[DailyCargoReports]  " +
                 " union  " +
                 " Select DRID as ID,ShipName,'Departure' as ReportName,VoyageNo,PortName,ReportCreated as CreatedDate  ,'' as Inspector,'' as Superintended from[dbo].[DepartureReports]  " +
                 " union  " +
                 " select DPRID as ID,ShipName,'Daily Position' as ReportName,VoyageNo,'' as PortName,ReportCreated as CreatedDate  ,'' as Inspector,'' as Superintended from[dbo].[DailyPositionReport]  " +
                 //" union  " +
                 //" Select GIRFormID as ID,ShipName,'General Inspection' as ReportName,'' as VoyageNo,'' as PortName,CreatedDate,Inspector ,'' as Superintended from[dbo].[GeneralInspectionReport]  S  " +
                 //" union  " +
                 //" Select InternalAuditFormId as ID,ShipName,'Internal Audit Form' as ReportName,'' as VoyageNo,'' as PortName,CreatedDate  ,'' as Inspector,'' as Superintended from[dbo].[InternalAuditForm]  S  " +
                 //" union  " +
                 //" Select SMRFormID as ID,ShipName,'Ship Management Review' as ReportName,'' as VoyageNo,'' as PortName,CreatedDate  ,'' as Inspector,'' as Superintended from[dbo].[SMRForm] S  " +
                 //" union  " +
                 //" Select SIRFormID as ID,ShipName,'Superintended Inspection' as ReportName,'' as VoyageNo,'' as PortName,Isnull(CreatedDate,Date) as CreatedDate  ,'' as Inspector,ISNULL(Superintended, '') as Superintended from[dbo].[SuperintendedInspectionReport]  " +
                 " ) X ";
                if (!string.IsNullOrWhiteSpace(Modal.shipCode))
                    sqlQuery += " Where (ShipName = '" + Modal.shipName + "' OR ShipName = '" + Modal.shipCode + "')";
                sqlQuery += " order by CreatedDate DESC  ";

                var data = dbContext.Database.SqlQuery<ShipReportsAnalysisModal>(sqlQuery);
                if (data != null)
                    return data.ToList();
            }
            return list;
        }
        public List<FleetInspectionDueDatesModal> GetFleetInspectionDueDates()
        {
            //string connetionString = Convert.ToString(ConfigurationManager.AppSettings["CarisbrookeLtdConnectionString"]);    // RDBJ 03/18/2022 commented this line
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);    // RDBJ 03/18/2022
            List<FleetInspectionDueDatesModal> codeList = new List<FleetInspectionDueDatesModal>();
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    //if (conn.IsAvailable())
                    //{
                    //using (SqlCommand cmd = new SqlCommand("[ISM].[spFleetInspectionDueDates]", conn))    // RDBJ 03/18/2022 commented this line
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spFleetInspectionDueDates]", conn))  // RDBJ 03/18/2022
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                codeList = dt.ToListof<FleetInspectionDueDatesModal>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFleetInspectionDueDates : " + ex.Message);
            }

            return codeList;
        }
        public bool UpdateFleetInspectionDueDates(FleetInspectionDueDatesModal Modal)
        {
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["CarisbrookeLtdConnectionString"]);
            bool flag = false;
            try
            {
                if (Modal == null || string.IsNullOrWhiteSpace(Modal.FieldName))
                    return false;
                string query = "";
                FleetInspectionEvents objData = new FleetInspectionEvents();
                objData.ShipId = Modal.ShipId;
                objData.EventDate = Modal.EventDate;

                // RDBJ 12/24/2021
                string strTableName = string.Empty;
                bool blnIsExist = false;
                using (var conn = new SqlConnection(connetionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[ISM].[SP_CheckIsExistFleetInspectionDueDates]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ShipId", SqlDbType.Int).Value = objData.ShipId;
                        cmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = Modal.FieldName.ToLower();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                blnIsExist = Convert.ToInt32(dt.Rows[0]["IsExist"].ToString()) == 1 ? true : false;
                            }
                        }
                    }
                }
                // End RDBJ 12/24/2021

                switch (Modal.FieldName.ToLower())
                {
                    case "lastflaggi":
                        strTableName = "Flag_GI_Events";    // RDBJ 12/24/2021
                        // RDBJ 12/24/2021 commented below query
                        //query = "INSERT INTO [ISM].[Flag_GI_Events]([ShipId],[EventDate],[Location],[HasDefects],[IsClosed],[FlagStateId]) " +
                        //        " VALUES(@ShipId,@EventDate,@Location,@HasDefects,@IsClosed,@FlagStateId)";
                        break;
                    case "lastflagasi":
                        strTableName = "Flag_ASI_Events";   // RDBJ 12/24/2021
                        // RDBJ 12/24/2021 commented below query
                        //query = "INSERT INTO [ISM].[Flag_ASI_Events]([ShipId],[EventDate],[Location],[HasDefects],[IsClosed],[FlagStateId]) " +
                        //       " VALUES(@ShipId,@EventDate,@Location,@HasDefects,@IsClosed,@FlagStateId)";                       
                        break;
                    case "lastflagcica":
                        strTableName = "Flag_CICA_Events";  // RDBJ 12/24/2021
                        // RDBJ 12/24/2021 commented below query
                        //query = "INSERT INTO [ISM].[Flag_CICA_Events]([ShipId],[EventDate],[Location],[HasDefects],[IsClosed],[FlagStateId]) " +
                        //       " VALUES(@ShipId,@EventDate,@Location,@HasDefects,@IsClosed,@FlagStateId)";                       
                        break;
                    default:
                        break;
                }

                // RDBJ 12/24/2021
                if (blnIsExist)
                {
                    query = "UPDATE [ISM].[" + strTableName + "] SET [EventDate] = @EventDate " +
                            " WHERE [ShipId] = @ShipId";
                }
                else
                {
                    query = "INSERT INTO [ISM].[" + strTableName + "]([ShipId],[EventDate],[Location],[HasDefects],[IsClosed],[FlagStateId]) " +
                           " VALUES(@ShipId,@EventDate,@Location,@HasDefects,@IsClosed,@FlagStateId)";
                }
                // End RDBJ 12/24/2021

                if (string.IsNullOrWhiteSpace(query))
                    return false;

                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.Add("@ShipId", SqlDbType.Int).Value = objData.ShipId;
                            command.Parameters.Add("@EventDate", SqlDbType.DateTime).Value = objData.EventDate ?? (object)DBNull.Value;
                            command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = objData.Location ?? (object)DBNull.Value;
                            command.Parameters.Add("@HasDefects", SqlDbType.Bit).Value = objData.HasDefects ?? (object)DBNull.Value;
                            command.Parameters.Add("@IsClosed", SqlDbType.Bit).Value = objData.IsClosed ?? (object)DBNull.Value;
                            command.Parameters.Add("@FlagStateId", SqlDbType.Int).Value = objData.FlagStateId ?? (object)DBNull.Value;
                            conn.Open();
                            command.ExecuteScalar();
                            conn.Close();
                            flag = true;
                        }                        
                    }
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFleetInspectionDueDates : " + ex.Message);
            }
            return flag;
        }
    }
}
