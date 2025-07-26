using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class FormsHelper
    {
        #region Reports
        public void SubmitArrivalReport(ArrivalReportModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            ArrivalReport dbReport = new ArrivalReport();
            dbReport.ShipNo = Modal.ShipNo;
            dbReport.ShipName = Modal.ShipName;
            dbReport.ReportCreated = Utility.DateToString(Modal.ReportCreated);
            dbReport.VoyageNo = Modal.VoyageNo;
            dbReport.PortName = Modal.PortName;
            dbReport.ArrivalDate = Utility.DateToString(Modal.ArrivalDate);
            dbReport.ArrivalTime = Modal.ArrivalTime;
            dbReport.NORTenderedDate = Modal.TenderedDate.HasValue ? Utility.DateToString(Modal.TenderedDate) : null;
            dbReport.NORTenderedTime = Modal.TenderedTime;
            dbReport.POBDate = Modal.POBDate.HasValue ? Utility.DateToString(Modal.POBDate) : null;
            dbReport.POBTime = Modal.POBTime;
            dbReport.NoOfTugsUsed = Utility.ToString(Modal.TugsNo);
            dbReport.OnAnchor = Modal.chkAnchorOn;
            dbReport.ArrivalAlongSideDate = Modal.ArrivalAlongsideDate.HasValue ? Utility.DateToString(Modal.ArrivalAlongsideDate) : null;
            dbReport.ArrivalAlongSideTime = Modal.ArrivalAlongsideTime;
            dbReport.AverageSpeed = Modal.AverageSpeed;
            dbReport.DistanceMade = Modal.Distance;
            dbReport.FuelOil = Modal.FuelOil;
            dbReport.DieselOil = Modal.DieselOil;
            dbReport.SulphurFuelOil = Modal.SulphurFuelOil;
            dbReport.SulphurDieselOil = Modal.SulphurDieselOil;
            dbReport.FreshWater = Modal.FreshWater;
            dbReport.LubeOil = Modal.LubeOil;
            dbReport.CargoDate = Modal.CargoDate.HasValue ? Utility.DateToString(Modal.CargoDate) : null;
            dbReport.CargoTime = Modal.CargoTime;
            dbReport.ETCDepartureDate = Utility.DateToString(Modal.DepartureDate);
            dbReport.ETCDepartureTime = Modal.DepartureTime;
            dbReport.NextPort = Modal.NextPort;
            dbReport.Remarks = Modal.Remarks;
            dbReport.ToEmail = Modal.ToEmail;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                dbReport.CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }
            else
                dbReport.CCEmail = string.Empty;
            dbReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.IsSynced = Modal.IsSynced;
            dbReport.CreatedBy = Modal.CreatedBy;
            dbContext.ArrivalReports.Add(dbReport);
            dbContext.SaveChanges();
        }
        public void SubmitDepartureReport(DepartureReportModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            DepartureReport dbReport = new DepartureReport();
            dbReport.ShipNo = Modal.ShipNo;
            dbReport.ShipName = Modal.ShipName;
            dbReport.ReportCreated = Utility.DateToString(Modal.ReportCreated);
            dbReport.VoyageNo = Modal.VoyageNo;
            dbReport.PortName = Modal.PortName;
            dbReport.DateCargoOperations = Modal.DateCargoOperations.HasValue ? Utility.DateToString(Modal.DateCargoOperations) : null;
            dbReport.TimeCargoOperations = Modal.TimeCargoOperations;
            dbReport.CargoOnBoard = Modal.CargoOnBoard;
            dbReport.CargoLoaded = Modal.CargoLoaded;
            dbReport.DraftAFT = Modal.DraftAFT;
            dbReport.DraftFWD = Modal.DraftFWD;
            dbReport.POBDate = Modal.POBDate.HasValue ? Utility.DateToString(Modal.POBDate) : null;
            dbReport.POBTime = Modal.POBTime;
            dbReport.DepartureDate = Utility.DateToString(Modal.DepartureDate);
            dbReport.DepartureTime = Modal.DepartureTime;
            dbReport.POffDate = Modal.POffDate.HasValue ? Utility.DateToString(Modal.POffDate) : null;
            dbReport.POffTime = Modal.POffTime;
            dbReport.NoOfTugs = Modal.NoOfTugs;
            dbReport.FuelOil = Modal.FuelOil;
            dbReport.DieselOil = Modal.DieselOil;
            dbReport.SulphurFuelOil = Modal.SulphurFuelOil;
            dbReport.SulphurDieselOil = Modal.SulphurDieselOil;
            dbReport.NextPort = Modal.NextPort;
            dbReport.ETADate = Utility.DateToString(Modal.ETADate);
            dbReport.ETATime = Modal.ETATime;
            dbReport.IntendedRoute = Modal.IntendedRoute;
            dbReport.Remarks = Modal.Remarks;
            dbReport.ToEmail = Modal.ToEmail;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                dbReport.CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }
            else
                dbReport.CCEmail = string.Empty;
            dbReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.IsSynced = Modal.IsSynced;
            dbReport.CreatedBy = Modal.CreatedBy;
            dbContext.DepartureReports.Add(dbReport);
            dbContext.SaveChanges();
        }
        public void SubmitDailyCargoReport(DailyCargoReportModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            DailyCargoReport dbReport = new DailyCargoReport();
            dbReport.ShipName = Modal.ShipName;
            dbReport.ReportCreated = Utility.DateToString(Modal.ReportCreated);
            dbReport.VoyageNo = Modal.VoyageNo;
            dbReport.PortName = Modal.PortName;
            dbReport.NoOfGangsEmployed = Modal.NoOfGangs;
            dbReport.NoOfShipsCranesInUse = Modal.NoOfShips;
            dbReport.QuantityOfCargoLoaded = Modal.QuantityOfCargoLoaded;
            dbReport.TotalCargoLoaded = Modal.TotalCargoLoaded;
            dbReport.CargoRemaining = Modal.CargoRemaining;
            dbReport.FuelOil = Modal.FuelOil;
            dbReport.DieselOil = Modal.DieselOil;
            dbReport.SulphurFuelOil = Modal.SulphurFuelOil;
            dbReport.SulphurDieselOil = Modal.SulphurDieselOil;
            dbReport.Sludge = Modal.Sludge;
            dbReport.DirtyOil = Modal.DirtyOil;
            dbReport.ETCDate = Utility.DateToString(Modal.ETCDate);
            dbReport.ETCTime = Modal.ETCTime;
            dbReport.NextPort = Modal.NextPort;
            dbReport.Remarks = Modal.Remarks;
            dbReport.ToEmail = Modal.ToEmail;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                dbReport.CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }
            else
                dbReport.CCEmail = string.Empty;
            dbReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.CargoType = Modal.CargoType;
            dbReport.IsSynced = Modal.IsSynced;
            dbReport.CreatedBy = Modal.CreatedBy;
            dbContext.DailyCargoReports.Add(dbReport);
            dbContext.SaveChanges();
        }

        public DailyCargoReportModal DailyCargoFormDetailsView(long id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            DailyCargoReportModal data = new DailyCargoReportModal();
            var objDC = dbContext.DailyCargoReports.Where(x => x.DCRID == id).FirstOrDefault();
            if (objDC != null)
            {
                data = new DailyCargoReportModal
                {
                    CargoRemaining = objDC.CargoRemaining ?? 0,
                    CargoType = objDC.CargoType,
                    DieselOil = objDC.DieselOil ?? 0,
                    DirtyOil = objDC.DirtyOil ?? 0,
                    ETCDate = Utility.ToDateTime(objDC.ETCDate),
                    ETCTime = objDC.ETCTime,
                    FuelOil = objDC.FuelOil ?? 0,
                    NextPort = objDC.NextPort,
                    NoOfGangs = objDC.NoOfGangsEmployed ?? 0,
                    NoOfShips = objDC.NoOfShipsCranesInUse ?? 0,
                    PortName = objDC.PortName,
                    QuantityOfCargoLoaded = objDC.QuantityOfCargoLoaded ?? 0,
                    Remarks = objDC.Remarks,
                    ShipName = objDC.ShipName,
                    Sludge = objDC.Sludge ?? 0,
                    SulphurDieselOil = objDC.SulphurDieselOil ?? 0,
                    SulphurFuelOil = objDC.SulphurFuelOil ?? 0,
                    TotalCargoLoaded = objDC.TotalCargoLoaded ?? 0,
                    VoyageNo = objDC.VoyageNo ?? 0

                };
            }

            return data;
        }

        public ArrivalReportModal ArrivalDetailsView(long id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            ArrivalReportModal data = new ArrivalReportModal();
            var objDC = dbContext.ArrivalReports.Where(x => x.ARID == id).FirstOrDefault();
            if (objDC != null)
            {
                data = new ArrivalReportModal
                {
                    ShipNo = objDC.ShipNo ?? 0,
                    ShipName = objDC.ShipName,
                    VoyageNo = objDC.VoyageNo ?? 0,
                    PortName = objDC.PortName,
                    ArrivalDate = Utility.ToDateTime(objDC.ArrivalDate),
                    ArrivalTime = objDC.ArrivalTime,
                    POBDate = Utility.ToDateTime(objDC.POBDate),
                    POBTime = objDC.POBTime,
                    AverageSpeed = Utility.ToInteger(objDC.AverageSpeed),
                    FuelOil = Utility.ToInteger(objDC.FuelOil),
                    DieselOil = Utility.ToInteger(objDC.DieselOil),
                    SulphurFuelOil = Utility.ToInteger(objDC.SulphurFuelOil),
                    SulphurDieselOil = Utility.ToInteger(objDC.SulphurDieselOil),
                    FreshWater = Utility.ToInteger(objDC.FreshWater),
                    LubeOil = Utility.ToInteger(objDC.LubeOil),
                    CargoDate = Utility.ToDateTime(objDC.CargoDate),
                    CargoTime = objDC.CargoTime,
                    NextPort = objDC.NextPort,
                    Remarks = objDC.Remarks
                };
            }
            return data;
        }

        public DailyPositionReportModal DailyPositionDetailsView(long id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            DailyPositionReportModal data = new DailyPositionReportModal();
            var objDC = dbContext.DailyPositionReports.Where(x => x.DPRID == id).FirstOrDefault();
            if (objDC != null)
            {
                data = new DailyPositionReportModal
                {
                    ShipNo = objDC.ShipNo ?? 0,
                    ShipName = objDC.ShipName,
                    VoyageNo = objDC.VoyageNo ?? 0,

                };
            }
            return data;
        }

        public void SubmitDailyPositionReport(DailyPositionReportModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            DailyPositionReport dbReport = new DailyPositionReport();
            dbReport.ShipNo = Modal.ShipNo;
            dbReport.ShipName = Modal.ShipName;
            dbReport.ShipCode = Modal.ShipCode;
            dbReport.ReportCreated = Utility.DateToString(Modal.ReportCreated);
            dbReport.VoyageNo = Modal.VoyageNo;
            dbReport.Latitude = Modal.Latitude;
            dbReport.Longitude = Modal.Longitude;
            dbReport.Anchored = Modal.chkAnchored;
            dbReport.AverageSpeed = Modal.AverageSpeed;
            dbReport.DistanceMade = Modal.DistanceMade;
            dbReport.NextPort = Modal.NextPort;
            dbReport.EstimatedArrivalDateEcoSpeed = Utility.DateToString(Modal.EstimatedArrivalDateEcoSpeed);
            dbReport.EstimatedArrivalTimeEcoSpeed = Modal.EstimatedArrivalTimeEcoSpeed;
            dbReport.EstimatedArrivalDateFullSpeed = Utility.DateToString(Modal.EstimatedArrivalDateFullSpeed);
            dbReport.EstimatedArrivalTimeFullSpeed = Modal.EstimatedArrivalTimeFullSpeed;
            dbReport.FuelOil = Modal.FuelOil;
            dbReport.DieselOil = Modal.DieselOil;
            dbReport.SulphurFuelOil = Modal.SulphurFuelOil;
            dbReport.SulphurDieselOil = Modal.SulphurDieselOil;
            dbReport.FreshWater = Modal.FreshWater;
            dbReport.LubeOil = Modal.LubeOil;
            dbReport.Sludge = Modal.Sludge;
            dbReport.DirtyOil = Modal.DirtyOil;
            dbReport.Pitch = Modal.Pitch;
            dbReport.EngineLoad = Modal.EngineLoad;
            dbReport.HighCylExhTemp = Modal.HighCylExhTemp;
            dbReport.ExhGasTempAftTurboChrg = Modal.ExhGasTempAftTurboChrg;
            dbReport.OilCunsum = Modal.OilCunsum;
            dbReport.WindDirection = Modal.WindDirection;
            dbReport.WindForce = Modal.WindForce;
            dbReport.SeaState = Modal.SeaState;
            dbReport.SwellDirection = Modal.SwellDirection;
            dbReport.SwellHeight = Modal.SwellHeight;
            dbReport.DraftAft = Modal.DraftAft;
            dbReport.DraftForward = Modal.DraftForward;
            dbReport.Remarks = Modal.Remarks;
            dbReport.ToEmail = Modal.ToEmail;
            if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
            {
                dbReport.CCEmail = string.Join(",", Modal.CCEmail.ToArray());
            }
            else
                dbReport.CCEmail = string.Empty;
            dbReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            dbReport.IsSynced = Modal.IsSynced;
            dbReport.CargoType = Modal.CargoType;
            dbReport.CreatedBy = Modal.CreatedBy;
            dbContext.DailyPositionReports.Add(dbReport);
            dbContext.SaveChanges();
        }

        public List<ArrivalReportModal> GetAllArrivalDataReports(ShipModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<ArrivalReportModal> listData = new List<ArrivalReportModal>();
            ArrivalReportModal data = new ArrivalReportModal();
            List<ArrivalReport> objDC = null;
            if (string.IsNullOrWhiteSpace(Modal.shipName))
                objDC = dbContext.ArrivalReports.ToList();
            else
                objDC = dbContext.ArrivalReports.Where(x => x.ShipName == Modal.shipName).ToList();

            if (objDC != null)
            {
                foreach (var item in objDC)
                {
                    data = new ArrivalReportModal
                    {
                        ShipNo = item.ShipNo ?? 0,
                        ShipName = item.ShipName,
                        ReportCreated = Utility.ToDateTime(item.ReportCreated),
                        VoyageNo = item.VoyageNo ?? 0,
                        PortName = item.PortName,
                        ArrivalDate = Utility.ToDateTime(item.ArrivalDate),
                        ArrivalTime = item.ArrivalTime,
                        TenderedDate = Utility.ToDateTime(item.NORTenderedDate),
                        POBDate = Utility.ToDateTime(item.POBDate),
                        POBTime = item.POBTime,
                        TugsNo = Convert.ToInt32(item.NoOfTugsUsed),
                        chkAnchorOn = Convert.ToBoolean(item.OnAnchor),
                        ArrivalAlongsideDate = Utility.ToDateTime(item.ArrivalAlongSideDate),
                        AverageSpeed = Utility.ToInteger(item.AverageSpeed),
                        Distance = Convert.ToInt32(item.DistanceMade),
                        FuelOil = Utility.ToInteger(item.FuelOil),
                        DieselOil = Utility.ToInteger(item.DieselOil),
                        SulphurFuelOil = Utility.ToInteger(item.SulphurFuelOil),
                        SulphurDieselOil = Utility.ToInteger(item.SulphurDieselOil),
                        FreshWater = Utility.ToInteger(item.FreshWater),
                        LubeOil = Utility.ToInteger(item.LubeOil),
                        CargoDate = Utility.ToDateTime(item.CargoDate),
                        CargoTime = item.CargoTime,
                        DepartureDate = Utility.ToDateTime(item.ETCDepartureDate),
                        NextPort = item.NextPort,
                        Remarks = item.Remarks,
                        ToEmail = item.ToEmail,
                        CreatedBy = item.CreatedBy
                    };
                    listData.Add(data);
                }
            }
            return listData.OrderByDescending(x => x.ReportCreated).ToList();
        }

        public List<DepartureReportModal> GetAllDepartureDataReports(ShipModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<DepartureReportModal> listData = new List<DepartureReportModal>();
            DepartureReportModal data = new DepartureReportModal();
            List<DepartureReport> objDC = null;
            if (string.IsNullOrWhiteSpace(Modal.shipName))
                objDC = dbContext.DepartureReports.ToList();
            else
                objDC = dbContext.DepartureReports.Where(x => x.ShipName == Modal.shipName).ToList();

            if (objDC != null)
            {
                foreach (var item in objDC)
                {
                    data = new DepartureReportModal
                    {
                        ShipNo = item.ShipNo ?? 0,
                        ShipName = item.ShipName,
                        ReportCreated = Utility.ToDateTime(item.ReportCreated),
                        VoyageNo = item.VoyageNo ?? 0,
                        PortName = item.PortName,
                        DateCargoOperations = Utility.ToDateTime(item.DateCargoOperations),
                        TimeCargoOperations = item.TimeCargoOperations,
                        CargoOnBoard = item.CargoOnBoard,
                        CargoLoaded = item.CargoLoaded,
                        POBDate = Utility.ToDateTime(item.POBDate),
                        POBTime = item.POBTime,
                        DepartureDate = Utility.ToDateTime(item.DepartureDate),
                        DepartureTime = item.DepartureTime,
                        POffDate = Utility.ToDateTime(item.POffDate),
                        POffTime = item.POffTime,
                        NoOfTugs = item.NoOfTugs,
                        FuelOil = item.FuelOil ?? 0,
                        DieselOil = item.DieselOil ?? 0,
                        SulphurFuelOil = item.SulphurFuelOil ?? 0,
                        SulphurDieselOil = item.SulphurDieselOil ?? 0,
                        NextPort = item.NextPort,
                        ETADate = Utility.ToDateTime(item.ETADate),
                        ETATime = item.ETATime,
                        IntendedRoute = item.IntendedRoute,
                        Remarks = item.Remarks,
                        ToEmail = item.ToEmail,
                        CreatedBy = item.CreatedBy
                    };
                    listData.Add(data);
                }
            }
            return listData.OrderByDescending(x => x.ReportCreated).ToList();
        }

        public List<DailyCargoReportModal> GetAllDailyCargoDataReports(ShipModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<DailyCargoReportModal> listData = new List<DailyCargoReportModal>();
            DailyCargoReportModal data = new DailyCargoReportModal();
            List<DailyCargoReport> objDC = null;
            if (string.IsNullOrWhiteSpace(Modal.shipName))
                objDC = dbContext.DailyCargoReports.ToList();
            else
                objDC = dbContext.DailyCargoReports.Where(x => x.ShipName == Modal.shipName).ToList();

            if (objDC != null)
            {
                foreach (var item in objDC)
                {
                    data = new DailyCargoReportModal
                    {
                        CargoRemaining = item.CargoRemaining ?? 0,
                        CargoType = item.CargoType,
                        DieselOil = item.DieselOil ?? 0,
                        DirtyOil = item.DirtyOil ?? 0,
                        ETCDate = Utility.ToDateTime(item.ETCDate),
                        ETCTime = item.ETCTime,
                        FuelOil = item.FuelOil ?? 0,
                        NextPort = item.NextPort,
                        NoOfGangs = item.NoOfGangsEmployed ?? 0,
                        NoOfShips = item.NoOfShipsCranesInUse ?? 0,
                        PortName = item.PortName,
                        QuantityOfCargoLoaded = item.QuantityOfCargoLoaded ?? 0,
                        Remarks = item.Remarks,
                        ShipName = item.ShipName,
                        Sludge = item.Sludge ?? 0,
                        SulphurDieselOil = item.SulphurDieselOil ?? 0,
                        SulphurFuelOil = item.SulphurFuelOil ?? 0,
                        TotalCargoLoaded = item.TotalCargoLoaded ?? 0,
                        VoyageNo = item.VoyageNo ?? 0,
                        ReportCreated = Utility.ToDateTime(item.ReportCreated),
                        ToEmail = item.ToEmail,
                        CreatedBy = item.CreatedBy
                    };
                    listData.Add(data);
                }
            }

            return listData.OrderByDescending(x => x.ReportCreated).ToList();
        }

        public List<DailyPositionReportModal> GetAllDailyPositionDataReports(ShipModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<DailyPositionReportModal> listData = new List<DailyPositionReportModal>();
            DailyPositionReportModal data = new DailyPositionReportModal();
            List<DailyPositionReport> objDC = null;
            if (string.IsNullOrWhiteSpace(Modal.shipName))
                objDC = dbContext.DailyPositionReports.ToList();
            else
                objDC = dbContext.DailyPositionReports.Where(x => x.ShipName == Modal.shipName).ToList();

            if (objDC != null)
            {
                foreach (var item in objDC)
                {
                    data = new DailyPositionReportModal
                    {
                        ShipNo = item.ShipNo ?? 0,
                        ShipName = item.ShipName,
                        ReportCreated = Utility.ToDateTime(item.ReportCreated),
                        VoyageNo = item.VoyageNo ?? 0,
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        chkAnchored = item.Anchored ?? false,
                        AverageSpeed = item.AverageSpeed ?? 0,
                        DistanceMade = item.DistanceMade ?? 0,
                        NextPort = item.NextPort,
                        EstimatedArrivalDateEcoSpeed = item.EstimatedArrivalDateEcoSpeed, //Utility.ToDateTime(item.EstimatedArrivalDateEcoSpeed),
                        EstimatedArrivalTimeEcoSpeed = item.EstimatedArrivalTimeEcoSpeed,
                        EstimatedArrivalDateFullSpeed = item.EstimatedArrivalDateFullSpeed,// Utility.ToDateTime(item.EstimatedArrivalDateFullSpeed),
                        EstimatedArrivalTimeFullSpeed = item.EstimatedArrivalTimeFullSpeed,
                        FuelOil = item.FuelOil ?? 0,
                        DieselOil = item.DieselOil ?? 0,
                        SulphurFuelOil = item.SulphurFuelOil ?? 0,
                        SulphurDieselOil = item.SulphurDieselOil ?? 0,
                        FreshWater = item.FreshWater ?? 0,
                        LubeOil = item.LubeOil ?? 0,
                        Sludge = item.Sludge ?? 0,
                        DirtyOil = item.DirtyOil ?? 0,
                        Pitch = item.Pitch ?? 0,
                        EngineLoad = item.EngineLoad ?? 0,
                        HighCylExhTemp = item.HighCylExhTemp ?? 0,
                        ExhGasTempAftTurboChrg = item.ExhGasTempAftTurboChrg ?? 0,
                        OilCunsum = item.OilCunsum ?? 0,
                        WindDirection = item.WindDirection,
                        WindForce = item.WindForce,
                        SeaState = item.SeaState,
                        SwellDirection = item.SwellDirection,
                        SwellHeight = item.SwellHeight ?? 0,
                        DraftAft = item.DraftAft ?? 0,
                        DraftForward = item.DraftForward ?? 0,
                        Remarks = item.Remarks,
                        ToEmail = item.ToEmail,
                        CargoType = item.CargoType,
                        CreatedBy = item.CreatedBy
                    };
                    listData.Add(data);
                }
            }
            return listData.OrderByDescending(x => x.ReportCreated).ToList();
        }
        #endregion

        #region Forms
        public List<FormsModal> GetAllForms()
        {
            List<FormsModal> list = new List<FormsModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Form> dbList = new List<Form>();
            dbList = dbContext.Forms.Where(x => x.IsDeleted == false).ToList();
            list = dbList.Select(x => new FormsModal()
            {
                ID = x.ID,
                FormID = x.FormID,
                Title = x.Title,
                Type = x.Type,
                TemplatePath = x.TemplatePath,
                IsDeleted = x.IsDeleted,
                DownloadPath = x.DownloadPath,
                UploadType = x.UploadType,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                Code = x.Code,
                AccessLevel = x.AccessLevel,
                AllowsNetworkAccess = x.AllowsNetworkAccess,
                Amendment = x.Amendment,
                AmendmentDate = x.AmendmentDate,
                CanBeOpened = x.CanBeOpened,
                Category = x.Category,
                Department = x.Department,
                HasSavedData = x.HasSavedData,
                Issue = x.Issue,
                IssueDate = x.IssueDate,
                IsURNBased = x.IsURNBased,
                URN = x.URN,
                DocumentVersion = x.DocumentVersion,
                Version = x.Version,
                FolderType = x.FolderType

            }).OrderBy(x => x.Category).ToList();
            return list;
        }
        public List<FormsModal> GetAllFormsForService()
        {
            List<FormsModal> list = new List<FormsModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Form> dbList = new List<Form>();
            dbList = dbContext.Forms.ToList();
            list = dbList.Select(x => new FormsModal()
            {
                ID = x.ID,
                FormID = x.FormID,
                Title = x.Title,
                Type = x.Type,
                TemplatePath = x.TemplatePath,
                IsDeleted = x.IsDeleted,
                DownloadPath = x.DownloadPath,
                UploadType = x.UploadType,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                Code = x.Code,
                AccessLevel = x.AccessLevel,
                AllowsNetworkAccess = x.AllowsNetworkAccess,
                Amendment = x.Amendment,
                AmendmentDate = x.AmendmentDate,
                CanBeOpened = x.CanBeOpened,
                Category = x.Category,
                Department = x.Department,
                HasSavedData = x.HasSavedData,
                Issue = x.Issue,
                IssueDate = x.IssueDate,
                IsURNBased = x.IsURNBased,
                URN = x.URN,
                DocumentVersion = x.DocumentVersion,
                Version = x.Version,
                FolderType = x.FolderType
            }).ToList();
            return list;
        }
        public FormsModal GetFormByID(string id)
        {
            FormsModal DocModal = new FormsModal();
            Guid result = new Guid(id);
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Form dbDoc = dbContext.Forms.Where(x => x.FormID == result).FirstOrDefault();
            if (dbDoc != null)
            {
                DocModal = new FormsModal()
                {
                    ID = dbDoc.ID,
                    FormID = dbDoc.FormID,
                    Title = dbDoc.Title,
                    Type = dbDoc.Type,
                    TemplatePath = dbDoc.TemplatePath,
                    IsDeleted = dbDoc.IsDeleted,
                    DownloadPath = dbDoc.DownloadPath,
                    UploadType = dbDoc.UploadType,
                    CreatedDate = dbDoc.CreatedDate,
                    UpdatedDate = dbDoc.UpdatedDate,
                    Code = dbDoc.Code,
                    AccessLevel = dbDoc.AccessLevel,
                    AllowsNetworkAccess = dbDoc.AllowsNetworkAccess,
                    Amendment = dbDoc.Amendment,
                    AmendmentDate = dbDoc.AmendmentDate,
                    CanBeOpened = dbDoc.CanBeOpened,
                    Category = dbDoc.Category,
                    Department = dbDoc.Department,
                    HasSavedData = dbDoc.HasSavedData,
                    Issue = dbDoc.Issue,
                    IssueDate = dbDoc.IssueDate,
                    IsURNBased = dbDoc.IsURNBased,
                    URN = dbDoc.URN,
                    DocumentVersion = dbDoc.DocumentVersion,
                    Version = dbDoc.Version,
                    FolderType = dbDoc.FolderType
                };
            }
            return DocModal;
        }
        public void AddForm(FormsModal Modal)
        {
            Form dbDoc = new Form();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            dbDoc = new Form()
            {
                FormID = Modal.FormID,
                Title = Modal.Title,
                Type = Modal.Type,
                TemplatePath = Modal.TemplatePath,
                IsDeleted = Modal.IsDeleted,
                DownloadPath = Modal.DownloadPath,
                UploadType = Modal.UploadType,
                CreatedDate = Modal.CreatedDate,
                UpdatedDate = Modal.UpdatedDate,
                Code = Modal.Code,
                AccessLevel = Modal.AccessLevel,
                AllowsNetworkAccess = Modal.AllowsNetworkAccess,
                Amendment = Modal.Amendment,
                AmendmentDate = Modal.AmendmentDate,
                CanBeOpened = Modal.CanBeOpened,
                Category = Modal.Category,
                Department = Modal.Department,
                HasSavedData = Modal.HasSavedData,
                Issue = Modal.Issue,
                IssueDate = Modal.IssueDate,
                IsURNBased = Modal.IsURNBased,
                URN = Modal.URN,
                DocumentVersion = Modal.DocumentVersion.HasValue ? Modal.DocumentVersion.Value : 1.0,
                Version = Modal.Version.HasValue ? Modal.Version.Value : 1.0,
                FolderType = Modal.FolderType
            };
            dbContext.Forms.Add(dbDoc);
            dbContext.SaveChanges();
        }
        public void UpdateForm(FormsModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Form dbDoc = dbContext.Forms.Where(x => x.FormID == Modal.FormID).FirstOrDefault();
            if (dbDoc != null)
            {
                dbDoc.Title = Modal.Title;
                dbDoc.Type = Modal.Type;
                dbDoc.TemplatePath = Modal.TemplatePath;
                dbDoc.IsDeleted = Modal.IsDeleted.HasValue ? Modal.IsDeleted : false;
                dbDoc.DownloadPath = Modal.DownloadPath;
                dbDoc.UploadType = Modal.UploadType;
                dbDoc.CreatedDate = Modal.CreatedDate;
                dbDoc.UpdatedDate = Modal.UpdatedDate;
                dbDoc.Code = Modal.Code;
                dbDoc.AccessLevel = Modal.AccessLevel;
                dbDoc.AllowsNetworkAccess = Modal.AllowsNetworkAccess;
                dbDoc.Amendment = Modal.Amendment;
                dbDoc.AmendmentDate = Modal.AmendmentDate;
                dbDoc.CanBeOpened = Modal.CanBeOpened;
                dbDoc.Category = Modal.Category;
                dbDoc.Department = Modal.Department;
                dbDoc.HasSavedData = Modal.HasSavedData;
                dbDoc.Issue = Modal.Issue;
                dbDoc.IssueDate = Modal.IssueDate;
                dbDoc.IsURNBased = Modal.IsURNBased;
                dbDoc.URN = Modal.URN;
                dbDoc.Version = dbDoc.Version.HasValue ? dbDoc.Version.Value + 0.1 : 1.1;
                dbDoc.UpdatedDate = Modal.UpdatedDate.HasValue ? Modal.UpdatedDate : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbDoc.FolderType = Modal.FolderType;
                dbContext.SaveChanges();
            }
        }
        public void UpdateFormFile(FormsModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Form dbDoc = dbContext.Forms.Where(x => x.FormID == Modal.FormID).FirstOrDefault();
            if (dbDoc != null)
            {
                dbDoc.TemplatePath = Modal.TemplatePath;
                dbDoc.DownloadPath = Modal.DownloadPath;
                dbDoc.Type = Modal.Type;
                dbDoc.UploadType = AppStatic.UPDATED;
                dbDoc.DocumentVersion = dbDoc.DocumentVersion.HasValue ? dbDoc.DocumentVersion.Value + 0.1 : 1.1;
                dbDoc.UpdatedDate = Modal.UpdatedDate.HasValue ? Modal.UpdatedDate : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbContext.SaveChanges();
            }
        }
        public void DeleteForm(string id)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid ID = Guid.Parse(id);
            Form dbDoc = dbContext.Forms.Where(x => x.FormID == ID).FirstOrDefault();
            if (dbDoc != null)
            {
                dbDoc.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                dbDoc.IsDeleted = true;
                dbContext.SaveChanges();
            }
        }

        // RDBJ 12/03/2021 Re-Generate with logic set FormVersion and IsSynced
        public bool DeleteGISIIADrafts(string GISIFormID, string type)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            Guid FormUID = Guid.Parse(GISIFormID);
            try
            {
                if (type.ToLower() == "gi")
                {
                    Entity.GeneralInspectionReport GIR = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == FormUID).First();
                    GIR.FormVersion = GIR.FormVersion + Convert.ToDecimal(0.01);
                    //GIR.SavedAsDraft = false; // RDBJ 01/05/2022 commented this line
                    GIR.isDelete = 1; // RDBJ 01/05/2022
                    GIR.IsSynced = false;
                    //dbContext.GeneralInspectionReports.Remove(GIR);
                    dbContext.SaveChanges();
                }
                else if (type.ToLower() == "si") // RDBJ 01/23/2022 added if condition
                {
                    Entity.SuperintendedInspectionReport SIR = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == FormUID).First();
                    SIR.FormVersion = SIR.FormVersion + Convert.ToDecimal(0.01);
                    //SIR.SavedAsDraft = false; // RDBJ 01/05/2022 commented this line
                    SIR.isDelete = 1; // RDBJ 01/05/2022
                    SIR.IsSynced = false;
                    //dbContext.SuperintendedInspectionReports.Remove(SIR);
                    dbContext.SaveChanges();
                }
                else // RDBJ 01/23/2022 added else
                {
                    Entity.InternalAuditForm IAR = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == FormUID).First();
                    IAR.FormVersion = IAR.FormVersion + Convert.ToDecimal(0.01);
                    //IAR.SavedAsDraft = false;
                    IAR.isDelete = 1;
                    IAR.IsSynced = false;
                    //dbContext.InternalAuditForms.Remove(IAR);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteGISIIADrafts : " + ex.Message);
                return false;
            }
        }
        #region Sync Documnets
        public bool AddSyncForms(List<FormsModal> DocList)
        {
            bool res = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                foreach (var Modal in DocList)
                {
                    Form dbDoc = new Form()
                    {
                        FormID = Modal.FormID,
                        Title = Modal.Title,
                        Type = Modal.Type,
                        TemplatePath = Modal.TemplatePath,
                        IsDeleted = Modal.IsDeleted,
                        DownloadPath = Modal.DownloadPath,
                        UploadType = Modal.UploadType,
                        CreatedDate = Modal.CreatedDate,
                        UpdatedDate = Modal.UpdatedDate,
                        Code = Modal.Code,
                        AccessLevel = Modal.AccessLevel,
                        AllowsNetworkAccess = Modal.AllowsNetworkAccess,
                        Amendment = Modal.Amendment,
                        AmendmentDate = Modal.AmendmentDate,
                        CanBeOpened = Modal.CanBeOpened,
                        Category = Modal.Category,
                        Department = Modal.Department,
                        HasSavedData = Modal.HasSavedData,
                        Issue = Modal.Issue,
                        IssueDate = Modal.IssueDate,
                        IsURNBased = Modal.IsURNBased,
                        URN = Modal.URN,
                        DocumentVersion = 1.0,
                        Version = 1.0,
                        FolderType = Modal.FolderType
                    };
                    dbContext.Forms.Add(dbDoc);
                }
                dbContext.SaveChanges();
                dbContext.Dispose();
                res = true;
            }
            catch (Exception)
            { }
            return res;
        }
        #endregion
        #endregion

        #region Siteinfo
        public List<ShipWisePCModal> GetAllSiteInfoDatas(ShipModal Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            //  List<ShipSystemModal> listData = new List<ShipSystemModal>();

            //  ShipSystemModal data = new ShipSystemModal();

            //var objDC = dbContext.ShipSystems.GroupBy(x=> x.ShipCode).ToList();
            //var query = (from s in dbContext.ShipSystems
            //             join se in dbContext.ShipSystemsEventLogs on s.Id equals se.ShipSystemId
            //             join sd in dbContext.ShipAppDownloadLogs on s.ShipCode equals sd.ShipCode
            //             select new ShipWisePCModal()
            //             {
            //                 ShipCode = s.ShipCode,
            //                 PCName = s.PCName,
            //                 EventDate = se.EventDate,
            //                 DownloadedDate = sd.DownloadDate
            //             });
            //codeList = query.ToList();

            //if (objDC != null)
            //{
            //    foreach (var item in objDC)
            //    {
            //        data = new ShipSystemModal
            //        {
            //            Id = item.Id,
            //            ShipCode = item.ShipCode,
            //            PCName = item.PCName,
            //        };
            //        listData.Add(data);
            //    }
            //}
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
            List<ShipWisePCModal> codeList = new List<ShipWisePCModal>();

            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open(); //RDBJ 11/11/2021
                        using (SqlCommand cmd = new SqlCommand("usp_Get_SiteInfo_Data", conn))
                        {
                            cmd.CommandTimeout = 120; // RDBJ 02/28/2022 set timeout
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    codeList = dt.ToListof<ShipWisePCModal>();
                                }
                            }
                        }
                        conn.Close(); //RDBJ 11/11/2021
                        //string selectQuery = "SELECT s.ShipCode , cs.Name ,s.PCName,SD.DownloadDate,SE.EventDate, s.PCUniqueId, s.Id " +
                        //                 "FROM ShipSystems s Outer APPLY(" +
                        //                  "  Select top 1 DownloadDate From ShipAppDownloadLog SD" +
                        //                   " Where SD.ShipCode = s.ShipCode AND SD.PCUniqueId = s.PCUniqueId Order By DownloadDate DESC" +
                        //                 ")SD Outer APPLY(" +
                        //                   " Select top 1 CreatedDate as EventDate From ShipSystemsEventLog SE" +
                        //                    " Where SE.ShipSystemId = s.Id Order By CreatedDate DESC)SE " +
                        //                    " left join CSShips cs on s.ShipCode = cs.code  where ISNULL(s.IsDeleted,0) = 0  "; //and cs.IsDelivered = 1
                        //conn.Open();
                        //DataTable dt = new DataTable();
                        //SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        //sqlAdp.Fill(dt);
                        //if (dt != null && dt.Rows.Count > 0)
                        //{
                        //    codeList = dt.ToListof<ShipWisePCModal>();
                        //}
                        //conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("CSShip Connection is not available !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllSiteInfoDatas : " + ex.Message);
            }

            return codeList;
        }

        public APIResponse DeleteAllPCRecordData(ShipWisePCModal shipData)
        {
            //  List<ShipWisePCModal> res = new List<ShipWisePCModal>();
            APIResponse res = new APIResponse();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                ShipSystem dbDoc = dbContext.ShipSystems.Where(x => x.ShipCode == shipData.ShipCode && x.PCName == shipData.PCName && x.PCUniqueId == shipData.PCUniqueId).FirstOrDefault();
                if (dbDoc != null)
                {
                    dbDoc.IsDeleted = true;
                    dbDoc.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.SaveChanges();
                }

                List<ShipAppDownloadLog> dbDownload = dbContext.ShipAppDownloadLogs.Where(x => x.ShipCode == shipData.ShipCode && x.PCUniqueId == shipData.PCUniqueId && x.PCName == shipData.PCName).ToList();
                if (dbDownload != null && dbDownload.Count > 0)
                {
                    foreach (ShipAppDownloadLog item in dbDownload)
                    {
                        dbContext.ShipAppDownloadLogs.Remove(item);
                        dbContext.SaveChanges();
                    }
                }

                List<ShipSystemsEventLog> dbEvent = dbContext.ShipSystemsEventLogs.Where(x => x.ShipSystemId == shipData.Id && x.EventMachineName == shipData.PCName).ToList();
                if (dbEvent != null && dbEvent.Count > 0)
                {
                    foreach (ShipSystemsEventLog eventitem in dbEvent)
                    {
                        dbContext.ShipSystemsEventLogs.Remove(eventitem);
                        dbContext.SaveChanges();
                    }
                }
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        public APIResponse UpdateBlockPCRecords(ShipWisePCModal shipData)
        {
            //  List<ShipWisePCModal> res = new List<ShipWisePCModal>();
            APIResponse res = new APIResponse();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                ShipSystem dbDoc = dbContext.ShipSystems.Where(x => x.ShipCode == shipData.ShipCode && x.PCName == shipData.PCName && x.PCUniqueId == shipData.PCUniqueId).FirstOrDefault();
                if (dbDoc != null)
                {
                    dbDoc.IsBlocked = shipData.IsBlocked;
                    dbDoc.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.SaveChanges();
                }
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        public APIResponse UpdateMainPCRecords(ShipWisePCModal shipData)
        {
            APIResponse res = new APIResponse();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                //ShipSystem dbDoc = dbContext.ShipSystems.Where(x => x.ShipCode == shipData.ShipCode && x.PCName == shipData.PCName && x.PCUniqueId == shipData.PCUniqueId).FirstOrDefault();
                ShipSystem dbDoc = dbContext.ShipSystems.Where(x => x.Id == shipData.Id && x.PCUniqueId == shipData.PCUniqueId).FirstOrDefault();
                if (dbDoc != null)
                {
                    dbDoc.IsMainPC = shipData.IsMainPC;
                    dbDoc.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.SaveChanges();
                }
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        #endregion
    }
}
