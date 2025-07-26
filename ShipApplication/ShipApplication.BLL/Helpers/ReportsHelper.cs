using ShipApplication.BLL.Modals;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ShipApplication.BLL.Helpers
{
    public class ReportsHelper
    {
        #region ArrivalReport
        public bool SaveArrivalReportDataInLocalDB(ArrivalReportModal Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.ArrivalReports);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.ArrivalReports); }
                if (isTbaleCreated)
                {
                    try
                    {
                        bool isColumnExist = LocalDBHelper.CheckTableColumnExist(AppStatic.ArrivalReports, "CreatedBy");
                        if (!isColumnExist)
                        {
                            LocalDBHelper.ExecuteQuery("ALTER TABLE ArrivalReports ADD CreatedBy Nvarchar(250)");
                        }
                    }
                    catch (Exception)
                    {
                    }
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQury = ArrivalReportInsertQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(InsertQury, connection);
                        ARDataInsertCMD(Modal, ref command);
                        connection.Open();
                        object resultObj = command.ExecuteScalar();
                        long databaseID = 0;
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out databaseID);
                        }
                        if (databaseID > 0)
                            res = true;
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in Arrival Report table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string ArrivalReportInsertQuery()
        {
            string InsertQury = @"INSERT INTO dbo.ArrivalReports (ShipNo,ShipName,ReportCreated,VoyageNo,PortName,
                                ArrivalDate,ArrivalTime,NORTenderedDate,NORTenderedTime,POBDate,POBTime,
                                NoOfTugsUsed,OnAnchor,ArrivalAlongSideDate,ArrivalAlongSideTime,AverageSpeed,DistanceMade,
                                FuelOil,DieselOil,SulphurFuelOil,SulphurDieselOil,FreshWater,LubeOil,CargoDate,CargoTime,
                                ETCDepartureDate,ETCDepartureTime,NextPort,Remarks,ToEmail,CCEmail,CreatedDate,UpdatedDate,IsSynced,CreatedBy)
                                OUTPUT INSERTED.ARID
                                VALUES (@ShipNo,@ShipName,@ReportCreated,@VoyageNo,@PortName,
                                @ArrivalDate,@ArrivalTime,@NORTenderedDate,@NORTenderedTime,@POBDate,@POBTime,
                                @NoOfTugsUsed,@OnAnchor,@ArrivalAlongSideDate,@ArrivalAlongSideTime,@AverageSpeed,@DistanceMade,
                                @FuelOil,@DieselOil,@SulphurFuelOil,@SulphurDieselOil,@FreshWater,@LubeOil,@CargoDate,@CargoTime,
                                @ETCDepartureDate,@ETCDepartureTime,@NextPort,@Remarks,@ToEmail,@CCEmail,@CreatedDate,@UpdatedDate,@IsSynced,@CreatedBy)";
            return InsertQury;
        }
        public void ARDataInsertCMD(ArrivalReportModal Modal, ref SqlCommand command)
        {
            string CCEmail = string.Empty;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }

            command.Parameters.Add("@ShipNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.ShipNo);
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ShipName);
            command.Parameters.Add("@ReportCreated", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ReportCreated);
            command.Parameters.Add("@VoyageNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.VoyageNo);
            command.Parameters.Add("@PortName", SqlDbType.NVarChar).Value = Utility.ToString(Modal.PortName);
            command.Parameters.Add("@ArrivalDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ArrivalDate);
            command.Parameters.Add("@ArrivalTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ArrivalTime);
            command.Parameters.Add("@NORTenderedDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.TenderedDate);
            command.Parameters.Add("@NORTenderedTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.TenderedTime);
            command.Parameters.Add("@POBDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.POBDate);
            command.Parameters.Add("@POBTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.POBTime);
            command.Parameters.Add("@NoOfTugsUsed", SqlDbType.NVarChar).Value = Utility.ToString(Modal.TugsNo);
            command.Parameters.Add("@OnAnchor", SqlDbType.NVarChar).Value = Modal.chkAnchorOn;
            command.Parameters.Add("@ArrivalAlongSideDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ArrivalAlongsideDate);
            command.Parameters.Add("@ArrivalAlongSideTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ArrivalAlongsideTime);
            command.Parameters.Add("@AverageSpeed", SqlDbType.Float).Value = Utility.ToDouble(Modal.AverageSpeed);
            command.Parameters.Add("@DistanceMade", SqlDbType.Float).Value = Utility.ToDouble(Modal.Distance);
            command.Parameters.Add("@FuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.FuelOil);
            command.Parameters.Add("@DieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.DieselOil);
            command.Parameters.Add("@SulphurFuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurFuelOil);
            command.Parameters.Add("@SulphurDieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurDieselOil);
            command.Parameters.Add("@FreshWater", SqlDbType.Float).Value = Utility.ToDouble(Modal.FreshWater);
            command.Parameters.Add("@LubeOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.LubeOil);
            command.Parameters.Add("@CargoDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.CargoDate);
            command.Parameters.Add("@CargoTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.CargoTime);
            command.Parameters.Add("@ETCDepartureDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.DepartureDate);
            command.Parameters.Add("@ETCDepartureTime", SqlDbType.NVarChar).Value = Modal.DepartureTime;
            command.Parameters.Add("@NextPort", SqlDbType.NVarChar).Value = Utility.ToString(Modal.NextPort);
            command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = Utility.ToString(Modal.Remarks);
            command.Parameters.Add("@ToEmail", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ToEmail);
            command.Parameters.Add("@CCEmail", SqlDbType.NVarChar).Value = Utility.ToString(CCEmail);
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced;
            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy;
        }
        #endregion

        #region DepartureReport
        public bool SaveDepartureReportDataInLocalDB(DepartureReportModal Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.DepartureReports);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.DepartureReports); }
                if (isTbaleCreated)
                {
                    try
                    {
                        bool isColumnExist = LocalDBHelper.CheckTableColumnExist(AppStatic.DepartureReports, "CreatedBy");
                        if (!isColumnExist)
                        {
                            LocalDBHelper.ExecuteQuery("ALTER TABLE DepartureReports ADD CreatedBy Nvarchar(250)");
                        }
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        bool isColumnExist = LocalDBHelper.CheckTableColumnExist(AppStatic.DepartureReports, "DraftAFT");
                        if (!isColumnExist)
                        {
                            LocalDBHelper.ExecuteQuery("ALTER TABLE DepartureReports ADD DraftAFT float");
                        }

                        bool isColumnExist2 = LocalDBHelper.CheckTableColumnExist(AppStatic.DepartureReports, "DraftFWD");
                        if (!isColumnExist2)
                        {
                            LocalDBHelper.ExecuteQuery("ALTER TABLE DepartureReports ADD DraftFWD float");
                        }
                    }
                    catch (Exception)
                    {
                    }
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQury = DepartureReportInsertQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(InsertQury, connection);
                        DRDataInsertCMD(Modal, ref command);
                        connection.Open();
                        object resultObj = command.ExecuteScalar();
                        long databaseID = 0;
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out databaseID);
                        }
                        if (databaseID > 0)
                            res = true;
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in Departure Report table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string DepartureReportInsertQuery()
        {
            string InsertQury = @"INSERT INTO dbo.DepartureReports (ShipNo,ShipName,ReportCreated,VoyageNo,PortName,
                                DateCargoOperations,TimeCargoOperations,CargoOnBoard,CargoLoaded,DraftAFT,DraftFWD,
                                POBDate,POBTime,DepartureDate,DepartureTime,POffDate,POffTime,NoOfTugs,
                                FuelOil,DieselOil,SulphurFuelOil,SulphurDieselOil,NextPort,ETADate,ETATime,IntendedRoute,
                                Remarks,ToEmail,CCEmail,IsSynced,CreatedDate,UpdatedDate,CreatedBy)
                                OUTPUT INSERTED.DRID
                                VALUES (@ShipNo,@ShipName,@ReportCreated,@VoyageNo,@PortName,
                                @DateCargoOperations,@TimeCargoOperations,@CargoOnBoard,@CargoLoaded,@DraftAFT,@DraftFWD,
                                @POBDate,@POBTime,@DepartureDate,@DepartureTime,@POffDate,@POffTime,@NoOfTugs,
                                @FuelOil,@DieselOil,@SulphurFuelOil,@SulphurDieselOil,@NextPort,@ETADate,@ETATime,@IntendedRoute,
                                @Remarks,@ToEmail,@CCEmail,@IsSynced,@CreatedDate,@UpdatedDate,@CreatedBy)";
            return InsertQury;
        }
        public void DRDataInsertCMD(DepartureReportModal Modal, ref SqlCommand command)
        {
            string CCEmail = string.Empty;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }

            command.Parameters.Add("@ShipNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.ShipNo);
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ShipName);
            command.Parameters.Add("@ReportCreated", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ReportCreated);
            command.Parameters.Add("@VoyageNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.VoyageNo);
            command.Parameters.Add("@PortName", SqlDbType.NVarChar).Value = Utility.ToString(Modal.PortName);
            command.Parameters.Add("@DateCargoOperations", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.DateCargoOperations);
            command.Parameters.Add("@TimeCargoOperations", SqlDbType.NVarChar).Value = Utility.ToString(Modal.TimeCargoOperations);
            command.Parameters.Add("@CargoOnBoard", SqlDbType.NVarChar).Value = Utility.ToString(Modal.CargoOnBoard);
            command.Parameters.Add("@CargoLoaded", SqlDbType.NVarChar).Value = Utility.ToString(Modal.CargoLoaded);
            command.Parameters.Add("@DraftAFT", SqlDbType.Float).Value = Utility.ToDouble(Modal.DraftAFT);
            command.Parameters.Add("@DraftFWD", SqlDbType.Float).Value = Utility.ToDouble(Modal.DraftFWD);
            command.Parameters.Add("@POBDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.POBDate);
            command.Parameters.Add("@POBTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.POBTime);
            command.Parameters.Add("@DepartureDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.DepartureDate);
            command.Parameters.Add("@DepartureTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.DepartureTime);
            command.Parameters.Add("@POffDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.POffDate);
            command.Parameters.Add("@POffTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.POffTime);
            command.Parameters.Add("@NoOfTugs", SqlDbType.NVarChar).Value = Utility.ToString(Modal.NoOfTugs);
            command.Parameters.Add("@FuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.FuelOil);
            command.Parameters.Add("@DieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.DieselOil);
            command.Parameters.Add("@SulphurFuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurFuelOil);
            command.Parameters.Add("@SulphurDieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurDieselOil);
            command.Parameters.Add("@NextPort", SqlDbType.NVarChar).Value = Utility.ToString(Modal.NextPort);
            command.Parameters.Add("@ETADate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ETADate);
            command.Parameters.Add("@ETATime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ETATime);
            command.Parameters.Add("@IntendedRoute", SqlDbType.NVarChar).Value = Utility.ToString(Modal.IntendedRoute);
            command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = Utility.ToString(Modal.Remarks);
            command.Parameters.Add("@ToEmail", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ToEmail);
            command.Parameters.Add("@CCEmail", SqlDbType.NVarChar).Value = Utility.ToString(CCEmail);
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced;
            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy;
        }
        #endregion

        #region DailyCargoReport
        public bool SaveDailyCargoReportDataInLocalDB(DailyCargoReportModal Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.DailyCargoReports);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.DailyCargoReports); }
                if (isTbaleCreated)
                {
                    try
                    {
                        bool isColumnExist = LocalDBHelper.CheckTableColumnExist(AppStatic.DailyCargoReports, "CreatedBy");
                        if (!isColumnExist)
                        {
                            LocalDBHelper.ExecuteQuery("ALTER TABLE DailyCargoReports ADD CreatedBy Nvarchar(250)");
                        }
                    }
                    catch (Exception)
                    {
                    }
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQury = DailyCargoReportInsertQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(InsertQury, connection);
                        DCRDataInsertCMD(Modal, ref command);
                        connection.Open();
                        object resultObj = command.ExecuteScalar();
                        long databaseID = 0;
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out databaseID);
                        }
                        if (databaseID > 0)
                            res = true;
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in Departure Report table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string DailyCargoReportInsertQuery()
        {
            string InsertQury = @"INSERT INTO dbo.DailyCargoReports (ShipNo,ShipName,ReportCreated,VoyageNo,PortName,
                                NoOfGangsEmployed,NoOfShipsCranesInUse,QuantityOfCargoLoaded,TotalCargoLoaded,CargoRemaining,
                                FuelOil,DieselOil,SulphurFuelOil,SulphurDieselOil,Sludge,DirtyOil,
                                ETCDate,ETCTime,NextPort,
                                Remarks,ToEmail,CCEmail,IsSynced,CreatedDate,UpdatedDate,CargoType,CreatedBy)
                                OUTPUT INSERTED.DCRID
                                VALUES (@ShipNo,@ShipName,@ReportCreated,@VoyageNo,@PortName,
                                @NoOfGangsEmployed,@NoOfShipsCranesInUse,@QuantityOfCargoLoaded,@TotalCargoLoaded,@CargoRemaining,
                                @FuelOil,@DieselOil,@SulphurFuelOil,@SulphurDieselOil,@Sludge,@DirtyOil,
                                @ETCDate,@ETCTime,@NextPort,
                                @Remarks,@ToEmail,@CCEmail,@IsSynced,@CreatedDate,@UpdatedDate,@CargoType,@CreatedBy)";
            return InsertQury;
        }
        public void DCRDataInsertCMD(DailyCargoReportModal Modal, ref SqlCommand command)
        {
            string CCEmail = string.Empty;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }

            command.Parameters.Add("@ShipNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.ShipNo);
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ShipName);
            command.Parameters.Add("@ReportCreated", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ReportCreated);
            command.Parameters.Add("@VoyageNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.VoyageNo);
            command.Parameters.Add("@PortName", SqlDbType.NVarChar).Value = Utility.ToString(Modal.PortName);
            command.Parameters.Add("@NoOfGangsEmployed", SqlDbType.Float).Value = Utility.ToDouble(Modal.NoOfGangs);
            command.Parameters.Add("@NoOfShipsCranesInUse", SqlDbType.Float).Value = Utility.ToDouble(Modal.NoOfShips);
            command.Parameters.Add("@QuantityOfCargoLoaded", SqlDbType.Float).Value = Utility.ToDouble(Modal.QuantityOfCargoLoaded);
            command.Parameters.Add("@TotalCargoLoaded", SqlDbType.Float).Value = Utility.ToDouble(Modal.TotalCargoLoaded);
            command.Parameters.Add("@CargoRemaining", SqlDbType.Float).Value = Utility.ToDouble(Modal.CargoRemaining);
            command.Parameters.Add("@FuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.FuelOil);
            command.Parameters.Add("@DieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.DieselOil);
            command.Parameters.Add("@SulphurFuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurFuelOil);
            command.Parameters.Add("@SulphurDieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurDieselOil);
            command.Parameters.Add("@Sludge", SqlDbType.Float).Value = Utility.ToDouble(Modal.Sludge);
            command.Parameters.Add("@DirtyOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.DirtyOil);
            command.Parameters.Add("@ETCDate", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ETCDate);
            command.Parameters.Add("@ETCTime", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ETCTime);
            command.Parameters.Add("@NextPort", SqlDbType.NVarChar).Value = Utility.ToString(Modal.NextPort);
            command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = Utility.ToString(Modal.Remarks);
            command.Parameters.Add("@ToEmail", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ToEmail);
            command.Parameters.Add("@CCEmail", SqlDbType.NVarChar).Value = Utility.ToString(CCEmail);
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced;
            command.Parameters.Add("@CargoType", SqlDbType.NVarChar).Value = Utility.ToString(Modal.CargoType);
            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy;
        }
        #endregion

        #region DailyPositionReport
        public bool SaveDailyPositionReportDataInLocalDB(DailyPositionReportModal Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.DailyPositionReport);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.DailyPositionReport); }
                if (isTbaleCreated)
                {
                    try
                    {
                        bool isColumnExist = LocalDBHelper.CheckTableColumnExist(AppStatic.DailyPositionReport, "CreatedBy");
                        if (!isColumnExist)
                        {
                            LocalDBHelper.ExecuteQuery("ALTER TABLE DailyPositionReport ADD CreatedBy Nvarchar(250)");
                        }
                    }
                    catch (Exception)
                    {
                    }
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQury = DailyPositionReportInsertQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(InsertQury, connection);
                        DPRDataInsertCMD(Modal, ref command);
                        connection.Open();
                        object resultObj = command.ExecuteScalar();
                        long databaseID = 0;
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out databaseID);
                        }
                        if (databaseID > 0)
                            res = true;
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in Departure Position table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string DailyPositionReportInsertQuery()
        {
            string InsertQury = @"INSERT INTO dbo.DailyPositionReport (ShipNo,ShipCode,ShipName,ReportCreated,VoyageNo,
                                Latitude,Longitude,Anchored,AverageSpeed,DistanceMade,
                                NextPort,EstimatedArrivalDateEcoSpeed,EstimatedArrivalTimeEcoSpeed,EstimatedArrivalDateFullSpeed,EstimatedArrivalTimeFullSpeed,
                                FuelOil,DieselOil,SulphurFuelOil,SulphurDieselOil,FreshWater,LubeOil,Sludge,DirtyOil,
                                Pitch,EngineLoad,HighCylExhTemp,ExhGasTempAftTurboChrg,OilCunsum,
                                WindDirection,WindForce,SeaState,SwellDirection,SwellHeight,DraftAft,DraftForward,
                                Remarks,ToEmail,CCEmail,IsSynced,CreatedDate,UpdatedDate,CargoType,CreatedBy)
                                OUTPUT INSERTED.DPRID
                                VALUES (@ShipNo,@ShipCode,@ShipName,@ReportCreated,@VoyageNo,
                                @Latitude,@Longitude,@Anchored,@AverageSpeed,@DistanceMade,
                                @NextPort,@EstimatedArrivalDateEcoSpeed,@EstimatedArrivalTimeEcoSpeed,@EstimatedArrivalDateFullSpeed,@EstimatedArrivalTimeFullSpeed,
                                @FuelOil,@DieselOil,@SulphurFuelOil,@SulphurDieselOil,@FreshWater,@LubeOil,@Sludge,@DirtyOil,
                                @Pitch,@EngineLoad,@HighCylExhTemp,@ExhGasTempAftTurboChrg,@OilCunsum,
                                @WindDirection,@WindForce,@SeaState,@SwellDirection,@SwellHeight,@DraftAft,@DraftForward,
                                @Remarks,@ToEmail,@CCEmail,@IsSynced,@CreatedDate,@UpdatedDate,@CargoType,@CreatedBy)";
            return InsertQury;
        }
        public void DPRDataInsertCMD(DailyPositionReportModal Modal, ref SqlCommand command)
        {
            string CCEmail = string.Empty;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }

            command.Parameters.Add("@ShipNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.ShipNo);
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ShipName);
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ShipCode);
            command.Parameters.Add("@ReportCreated", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.ReportCreated);
            command.Parameters.Add("@VoyageNo", SqlDbType.Int).Value = Utility.ToInteger(Modal.VoyageNo);
            command.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = Utility.ToString(Modal.Latitude);
            command.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = Utility.ToString(Modal.Longitude);
            command.Parameters.Add("@Anchored", SqlDbType.Bit).Value = Modal.chkAnchored;
            command.Parameters.Add("@AverageSpeed", SqlDbType.Float).Value = Utility.ToDouble(Modal.AverageSpeed);
            command.Parameters.Add("@DistanceMade", SqlDbType.Float).Value = Utility.ToDouble(Modal.DistanceMade);
            command.Parameters.Add("@NextPort", SqlDbType.NVarChar).Value = Utility.ToString(Modal.NextPort);
            command.Parameters.Add("@EstimatedArrivalDateEcoSpeed", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.EstimatedArrivalDateEcoSpeed);
            command.Parameters.Add("@EstimatedArrivalTimeEcoSpeed", SqlDbType.NVarChar).Value = Utility.ToString(Modal.EstimatedArrivalTimeEcoSpeed);
            command.Parameters.Add("@EstimatedArrivalDateFullSpeed", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.EstimatedArrivalDateFullSpeed);
            command.Parameters.Add("@EstimatedArrivalTimeFullSpeed", SqlDbType.NVarChar).Value = Utility.ToString(Modal.EstimatedArrivalTimeFullSpeed);
            command.Parameters.Add("@FuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.FuelOil);
            command.Parameters.Add("@DieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.DieselOil);
            command.Parameters.Add("@SulphurFuelOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurFuelOil);
            command.Parameters.Add("@SulphurDieselOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.SulphurDieselOil);
            command.Parameters.Add("@FreshWater", SqlDbType.Float).Value = Utility.ToDouble(Modal.FreshWater);
            command.Parameters.Add("@LubeOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.LubeOil);
            command.Parameters.Add("@Sludge", SqlDbType.Float).Value = Utility.ToDouble(Modal.Sludge);
            command.Parameters.Add("@DirtyOil", SqlDbType.Float).Value = Utility.ToDouble(Modal.DirtyOil);
            command.Parameters.Add("@Pitch", SqlDbType.Float).Value = Utility.ToDouble(Modal.Pitch);
            command.Parameters.Add("@EngineLoad", SqlDbType.Float).Value = Utility.ToDouble(Modal.EngineLoad);
            command.Parameters.Add("@HighCylExhTemp", SqlDbType.Float).Value = Utility.ToDouble(Modal.HighCylExhTemp);
            command.Parameters.Add("@ExhGasTempAftTurboChrg", SqlDbType.Float).Value = Utility.ToDouble(Modal.ExhGasTempAftTurboChrg);
            command.Parameters.Add("@OilCunsum", SqlDbType.Float).Value = Utility.ToDouble(Modal.OilCunsum);
            command.Parameters.Add("@WindDirection", SqlDbType.NVarChar).Value = Utility.ToString(Modal.WindDirection);
            command.Parameters.Add("@WindForce", SqlDbType.NVarChar).Value = Utility.ToString(Modal.WindForce);
            command.Parameters.Add("@SeaState", SqlDbType.NVarChar).Value = Utility.ToString(Modal.SeaState);
            command.Parameters.Add("@SwellDirection", SqlDbType.NVarChar).Value = Utility.ToString(Modal.SwellDirection);
            command.Parameters.Add("@SwellHeight", SqlDbType.Float).Value = Utility.ToDouble(Modal.SwellHeight);
            command.Parameters.Add("@DraftAft", SqlDbType.Float).Value = Utility.ToDouble(Modal.DraftAft);
            command.Parameters.Add("@DraftForward", SqlDbType.Float).Value = Utility.ToDouble(Modal.DraftForward);
            command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = Utility.ToString(Modal.Remarks);
            command.Parameters.Add("@ToEmail", SqlDbType.NVarChar).Value = Utility.ToString(Modal.ToEmail);
            command.Parameters.Add("@CCEmail", SqlDbType.NVarChar).Value = Utility.ToString(CCEmail);
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced;
            command.Parameters.Add("@CargoType", SqlDbType.NVarChar).Value = Utility.ToString(Modal.CargoType);
            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy;
        }
        #endregion
    }
}
