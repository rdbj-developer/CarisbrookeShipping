using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Helpers
{
    public class TableQueryGenerator
    {
        public static string SMRFormTableQuery()
        {
            string SMRtableQuery = @"CREATE TABLE [dbo].[SMRForm](
                                    [SMRFormID] [bigint] IDENTITY(1,1) PRIMARY KEY,
                                    [ShipID] [int] NULL,
                                    [ShipName] [nvarchar](max) NULL,
                                    [ReviewPeriod] [nvarchar](250) NULL,
                                    [Year] [int] NULL,
                                    [DateOfMeeting] [nvarchar](250) NULL,
                                    [Section1] [nvarchar](max) NULL, 
                                    [Section2] [nvarchar](max) NULL,
                                    [Section3] [nvarchar](max) NULL,
                                    [Section4] [nvarchar](max) NULL,
                                    [Section5] [nvarchar](max) NULL,
                                    [Section6] [nvarchar](max) NULL,
                                    [Section7] [nvarchar](max) NULL,
                                    [Section7a] [nvarchar](max) NULL,
                                    [Section7b] [nvarchar](max) NULL,
                                    [Section7c] [nvarchar](max) NULL,
	                                [Section7d] [nvarchar](max) NULL,
	                                [Section7e1] [nvarchar](max) NULL,
	                                [Section7e2] [nvarchar](max) NULL,
	                                [Section7e3] [nvarchar](max) NULL,
	                                [Section7f1] [nvarchar](max) NULL,
	                                [Section7f2] [nvarchar](max) NULL,
	                                [Section7g] [nvarchar](max) NULL,
	                                [Section7h] [nvarchar](max) NULL,
	                                [Section8a] [nvarchar](max) NULL,
	                                [Section8b] [nvarchar](max) NULL,
	                                [Section8b1] [nvarchar](max) NULL,
	                                [Section8b2] [nvarchar](max) NULL,
	                                [Section8b3] [nvarchar](max) NULL,
	                                [Section8b4] [nvarchar](max) NULL,
	                                [Section8b5] [nvarchar](max) NULL,
	                                [Section9] [nvarchar](max) NULL,
	                                [Section10] [nvarchar](max) NULL,
	                                [Section11] [nvarchar](max) NULL,
	                                [Section12a] [nvarchar](max) NULL,
	                                [Section12b] [nvarchar](max) NULL,
	                                [Section12c] [nvarchar](max) NULL,
	                                [Section12d] [nvarchar](max) NULL,
	                                [Section12e] [nvarchar](max) NULL,
	                                [Section12f] [nvarchar](max) NULL,
	                                [Section12g] [nvarchar](max) NULL,
	                                [Section12h] [nvarchar](max) NULL,
	                                [Section12i] [nvarchar](max) NULL,
	                                [Section12j] [nvarchar](max) NULL,
	                                [Section12k] [nvarchar](max) NULL,
	                                [Section13] [nvarchar](max) NULL,
                                    [IsSynced] [bit] NULL,
	                                [CreatedDate] [datetime] NULL,
	                                [UpdatedDate] [datetime] NULL )";
            return SMRtableQuery;

        }
        public static string SMRFormCrewMembersTableQuery()
        {
            string SMRtableQuery = @"CREATE TABLE [dbo].[SMRFormCrewMembers](
                                   [CrewMemberID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                               [SMRFormID] [bigint] NULL,
	                               [Rank] [nvarchar](max) NULL,
	                               [FullName] [nvarchar](max) NULL,
	                               [ElectedAsSafety] [bit] NULL,
	                               [CreatedDate] [datetime] NULL,
	                               [UpdatedDate] [datetime] NULL
                                   )";
            return SMRtableQuery;
        }
        public static string DocumentsTableQuery()
        {
            string SMRtableQuery = @"CREATE TABLE [dbo].[Documents](
	                                [DocID] [int] IDENTITY(1,1) PRIMARY KEY,
	                                [DocumentID] [uniqueidentifier] NULL,
	                                [ParentID] [uniqueidentifier] NULL,
	                                [Number] [nvarchar](250) NULL,
                                    [DocNo] [int] NULL,
	                                [Title] [nvarchar](max) NULL,
	                                [Type] [nvarchar](50) NULL,
	                                [Path] [nvarchar](max) NULL,
	                                [DownloadPath] [nvarchar](max) NULL,
	                                [IsDeleted] [bit] NULL,
	                                [UploadType] [nvarchar](50) NULL,
                                    [DocumentVersion] [float] NULL,
	                                [Version] [float] NULL,
                                    [Location] [nvarchar](250) NULL,
	                                [CreatedDate] [datetime] NULL,
	                                [UpdatedDate] [datetime] NULL,
                                    [SectionType] [nvarchar](100) NULL
                                   )";
            return SMRtableQuery;
        }
        public static string ArrivalReportsTableQuery()
        {
            string SMRtableQuery = @"CREATE TABLE [dbo].[ArrivalReports](
	                                [ARID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                                [ShipNo] [int] NULL,
	                                [ShipName] [nvarchar](250) NULL,
	                                [ReportCreated] [nvarchar](50) NULL,
	                                [VoyageNo] [int] NULL,
	                                [PortName] [nvarchar](250) NULL,
	                                [ArrivalDate] [nvarchar](50) NULL,
	                                [ArrivalTime] [nvarchar](50) NULL,
	                                [NORTenderedDate] [nvarchar](50) NULL,
	                                [NORTenderedTime] [nvarchar](50) NULL,
	                                [POBDate] [nvarchar](50) NULL,
	                                [POBTime] [nvarchar](50) NULL,
	                                [NoOfTugsUsed] [nvarchar](250) NULL,
	                                [OnAnchor] [bit] NULL,
	                                [ArrivalAlongSideDate] [nvarchar](50) NULL,
	                                [ArrivalAlongSideTime] [nvarchar](50) NULL,
	                                [AverageSpeed] [float] NULL,
	                                [DistanceMade] [float] NULL,
	                                [FuelOil] [float] NULL,
	                                [DieselOil] [float] NULL,
	                                [SulphurFuelOil] [float] NULL,
	                                [SulphurDieselOil] [float] NULL,
	                                [FreshWater] [float] NULL,
	                                [LubeOil] [float] NULL,
	                                [CargoDate] [nvarchar](50) NULL,
	                                [CargoTime] [nvarchar](50) NULL,
	                                [ETCDepartureDate] [nvarchar](50) NULL,
	                                [ETCDepartureTime] [nvarchar](50) NULL,
	                                [NextPort] [nvarchar](max) NULL,
	                                [Remarks] [nvarchar](max) NULL,
	                                [ToEmail] [nvarchar](250) NULL,
	                                [CCEmail] [nvarchar](max) NULL,
	                                [CreatedDate] [datetime] NULL,
	                                [UpdatedDate] [datetime] NULL,
                                    [IsSynced] [bit] NULL,
                                    [CreatedBy] [nvarchar](250) NULL
                                   )";
            return SMRtableQuery;
        }
        public static string DepartureReportsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[DepartureReports](
	                                [DRID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                                [ShipNo] [int] NULL,
	                                [ShipName] [nvarchar](250) NULL,
	                                [ReportCreated] [nvarchar](50) NULL,
	                                [VoyageNo] [int] NULL,
	                                [PortName] [nvarchar](250) NULL,
	                                [DateCargoOperations] [nvarchar](50) NULL,
	                                [TimeCargoOperations] [nvarchar](50) NULL,
	                                [CargoOnBoard] [nvarchar](50) NULL,
	                                [CargoLoaded] [nvarchar](50) NULL,
                                    [DraftAFT] [float] NULL,
	                                [DraftFWD] [float] NULL,
	                                [POBDate] [nvarchar](50) NULL,
	                                [POBTime] [nvarchar](50) NULL,
	                                [DepartureDate] [nvarchar](50) NULL,
	                                [DepartureTime] [nvarchar](50) NULL,
	                                [POffDate] [nvarchar](50) NULL,
	                                [POffTime] [nvarchar](50) NULL,
	                                [NoOfTugs] [nvarchar](250) NULL,
	                                [FuelOil] [float] NULL,
	                                [DieselOil] [float] NULL,
	                                [SulphurFuelOil] [float] NULL,
	                                [SulphurDieselOil] [float] NULL,
	                                [NextPort] [nvarchar](max) NULL,
	                                [ETADate] [nvarchar](50) NULL,
	                                [ETATime] [nvarchar](50) NULL,
	                                [IntendedRoute] [nvarchar](max) NULL,
	                                [Remarks] [nvarchar](max) NULL,
	                                [ToEmail] [nvarchar](250) NULL,
	                                [CCEmail] [nvarchar](max) NULL,
	                                [CreatedDate] [datetime] NULL,
	                                [UpdatedDate] [datetime] NULL,
                                    [IsSynced] [bit] NULL,
                                    [CreatedBy] [nvarchar](250) NULL
                                   )";
            return tableQuery;
        }
        public static string DailyCargoReportsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[DailyCargoReports](
	                            [DCRID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [ShipNo] [int] NULL,
	                            [ShipName] [nvarchar](250) NULL,
	                            [ReportCreated] [nvarchar](50) NULL,
	                            [VoyageNo] [int] NULL,
	                            [PortName] [nvarchar](250) NULL,
	                            [NoOfGangsEmployed] [float] NULL,
	                            [NoOfShipsCranesInUse] [float] NULL,
	                            [QuantityOfCargoLoaded] [float] NULL,
	                            [TotalCargoLoaded] [float] NULL,
	                            [CargoRemaining] [float] NULL,
	                            [FuelOil] [float] NULL,
	                            [DieselOil] [float] NULL,
	                            [SulphurFuelOil] [float] NULL,
	                            [SulphurDieselOil] [float] NULL,
	                            [Sludge] [float] NULL,
	                            [DirtyOil] [float] NULL,
	                            [ETCDate] [nvarchar](50) NULL,
	                            [ETCTime] [nvarchar](50) NULL,
	                            [NextPort] [nvarchar](max) NULL,
	                            [Remarks] [nvarchar](max) NULL,
	                            [ToEmail] [nvarchar](250) NULL,
	                            [CCEmail] [nvarchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
                                [IsSynced] [bit] NULL,
                                [CargoType] [nvarchar](250) NULL,
                                [CreatedBy] [nvarchar](250) NULL
                                   )";
            return tableQuery;
        }
        public static string DailyPositionReportTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[DailyPositionReport](
	                            [DPRID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [ShipNo] [int] NULL,
	                            [ShipCode] [nvarchar](50) NULL,
	                            [ShipName] [nvarchar](250) NULL,
	                            [ReportCreated] [nvarchar](50) NULL,
	                            [VoyageNo] [int] NULL,
	                            [Latitude] [nvarchar](50) NULL,
	                            [Longitude] [nvarchar](50) NULL,
	                            [Anchored] [bit] NULL,
	                            [AverageSpeed] [float] NULL,
	                            [DistanceMade] [float] NULL,
	                            [NextPort] [nvarchar](max) NULL,
	                            [EstimatedArrivalDateEcoSpeed] [nvarchar](50) NULL,
	                            [EstimatedArrivalTimeEcoSpeed] [nvarchar](50) NULL,
	                            [EstimatedArrivalDateFullSpeed] [nvarchar](50) NULL,
	                            [EstimatedArrivalTimeFullSpeed] [nvarchar](50) NULL,
	                            [FuelOil] [float] NULL,
	                            [DieselOil] [float] NULL,
	                            [SulphurFuelOil] [float] NULL,
	                            [SulphurDieselOil] [float] NULL,
	                            [FreshWater] [float] NULL,
	                            [LubeOil] [float] NULL,
	                            [Sludge] [float] NULL,
	                            [DirtyOil] [float] NULL,
	                            [Pitch] [float] NULL,
	                            [EngineLoad] [float] NULL,
	                            [HighCylExhTemp] [float] NULL,
	                            [ExhGasTempAftTurboChrg] [float] NULL,
	                            [OilCunsum] [float] NULL,
	                            [WindDirection] [nvarchar](max) NULL,
	                            [WindForce] [nvarchar](max) NULL,
	                            [SeaState] [nvarchar](max) NULL,
	                            [SwellDirection] [nvarchar](max) NULL,
	                            [SwellHeight] [float] NULL,
	                            [DraftAft] [float] NULL,
	                            [DraftForward] [float] NULL,
	                            [Remarks] [nvarchar](max) NULL,
	                            [ToEmail] [nvarchar](250) NULL,
	                            [CCEmail] [nvarchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
                                [IsSynced] [bit] NULL,
                                [CargoType] [nvarchar](250) NULL,
                                [CreatedBy] [nvarchar](250) NULL
                                )";
            return tableQuery;
        }
        public static string FormsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[Forms](
	                             [ID] [int] IDENTITY(1,1) NOT NULL,
	                             [FormID] [uniqueidentifier] NULL,
	                             [TemplatePath] [nvarchar](max) NULL,
	                             [Code] [nvarchar](100) NULL,
	                             [Title] [nvarchar](max) NULL,
	                             [Type] [nvarchar](50) NULL,
                                 [Issue] [int] NULL,
	                             [IssueDate] [datetime] NULL,
	                             [Amendment] [int] NULL,
	                             [AmendmentDate] [datetime] NULL,
	                             [Department] nvarchar(250) NULL,
	                             [Category] nvarchar(250) NULL,
	                             [AccessLevel] nvarchar(150) NULL,
	                             [AllowsNetworkAccess] bit NULL,
	                             [CanBeOpened] bit NULL,
	                             [HasSavedData] bit NULL,
	                             [IsURNBased] bit NULL,
	                             [URN] nvarchar(MAX) NULL,
	                             [IsDeleted] [bit] NULL,
	                             [DownloadPath] [nvarchar](max) NULL,
	                             [UploadType] [nvarchar](50) NULL,
	                             [CreatedDate] [datetime] NULL,
	                             [UpdatedDate] [datetime] NULL,
                                 [DocumentVersion] [float] NULL,
	                             [Version] [float] NULL,
                                 [FolderType] [nvarchar](50) NULL
                                   )";
            return tableQuery;
        }
        #region GIRTables
        public static string GeneralInspectionReportTableQuery()
        {
			// RDBJ 01/05/2022 Added isDelete
			string tableQuery = @"CREATE TABLE [dbo].[GeneralInspectionReport](
									[UniqueFormID] uniqueidentifier NULL,
									[FormVersion] numeric(18, 2) NULL,
	                                [GIRFormID] [bigint] IDENTITY(1,1) NOT NULL,
	                                [ShipID] [int] NULL,
	                                [ShipName] [nvarchar](max) NULL,
	                                [Ship] [nvarchar](max) NULL,
	                                [Port] [nvarchar](max) NULL,
	                                [Inspector] [nvarchar](max) NULL,
	                                [Date] [datetime] NULL,
	                                [GeneralPreamble] [nvarchar](max) NULL,
	                                [Classsociety] [nvarchar](max) NULL,
	                                [YearofBuild] [nvarchar](max) NULL,
	                                [Flag] [nvarchar](max) NULL,
	                                [Classofvessel] [nvarchar](max) NULL,
	                                [Portofregistry] [nvarchar](max) NULL,
	                                [MMSI] [nvarchar](max) NULL,
	                                [IMOnumber] [nvarchar](max) NULL,
	                                [Callsign] [nvarchar](max) NULL,
	                                [SummerDWT] [nvarchar](max) NULL,
	                                [Grosstonnage] [nvarchar](max) NULL,
	                                [Lightweight] [nvarchar](max) NULL,
	                                [Nettonnage] [nvarchar](max) NULL,
	                                [Beam] [nvarchar](max) NULL,
	                                [LOA] [nvarchar](max) NULL,
	                                [Summerdraft] [nvarchar](max) NULL,
	                                [LBP] [nvarchar](max) NULL,
	                                [Bowthruster] [nvarchar](max) NULL,
	                                [BHP] [nvarchar](max) NULL,
	                                [Noofholds] [nvarchar](max) NULL,
	                                [Nomoveablebulkheads] [nvarchar](max) NULL,
	                                [Containers] [nvarchar](max) NULL,
	                                [Cargocapacity] [nvarchar](max) NULL,
	                                [Cargohandlingequipment] [nvarchar](max) NULL,
	                                [Lastvoyageandcargo] [nvarchar](max) NULL,
	                                [CurrentPlannedvoyageandcargo] [nvarchar](max) NULL,
	                                [IsSynced] [bit] NULL,
	                                [CreatedDate] [datetime] NULL,
	                                [UpdatedDate] [datetime] NULL,
	                                [ShipboardWorkingArrangements] [nvarchar](max) NULL,
	                                [CertificationIndex] [nvarchar](max) NULL,
	                                [IsPubsAndDocsSectionComplete] [bit] NULL,
	                                [CarriedOutByTheDOOW] [nvarchar](max) NULL,
	                                [IsRegs4shipsDVD] [bit] NULL,
	                                [Regs4shipsDVD] [nvarchar](max) NULL,
	                                [IsSOPEPPoints] [bit] NULL,
	                                [SOPEPPoints] [nvarchar](max) NULL,
	                                [IsBWMP] [bit] NULL,
	                                [BWMP] [nvarchar](max) NULL,
	                                [IsBWMPSupplement] [bit] NULL,
	                                [BWMPSupplement] [nvarchar](max) NULL,
	                                [IsIntactStabilityManual] [bit] NULL,
	                                [IntactStabilityManual] [nvarchar](max) NULL,
	                                [IsStabilityComputer] [bit] NULL,
	                                [StabilityComputer] [nvarchar](max) NULL,
	                                [IsDateOfLast] [bit] NULL,
	                                [DateOfLast] [nvarchar](max) NULL,
	                                [IsCargoSecuring] [bit] NULL,
	                                [CargoSecuring] [nvarchar](max) NULL,
	                                [IsBulkCargo] [bit] NULL,
	                                [BulkCargo] [nvarchar](max) NULL,
	                                [IsSMSManual] [bit] NULL,
	                                [SMSManual] [nvarchar](max) NULL,
	                                [IsRegisterOf] [bit] NULL,
	                                [RegisterOf] [nvarchar](max) NULL,
	                                [IsFleetStandingOrder] [bit] NULL,
	                                [FleetStandingOrder] [nvarchar](max) NULL,
	                                [IsFleetMemoranda] [bit] NULL,
	                                [FleetMemoranda] [nvarchar](max) NULL,
	                                [IsShipsPlans] [bit] NULL,
	                                [ShipsPlans] [nvarchar](max) NULL,
	                                [IsCollective] [bit] NULL,
	                                [Collective] [nvarchar](max) NULL,
	                                [IsDraftAndFreeboardNotice] [bit] NULL,
	                                [DraftAndFreeboardNotice] [nvarchar](max) NULL,
	                                [IsPCSOPEP] [bit] NULL,
	                                [PCSOPEP] [nvarchar](max) NULL,
	                                [IsNTVRP] [bit] NULL,
	                                [NTVRP] [nvarchar](max) NULL,
	                                [IsVGP] [bit] NULL,
	                                [VGP] [nvarchar](max) NULL,
	                                [PubsComments] [nvarchar](max) NULL,
	                                [OfficialLogbookA] [nvarchar](max) NULL,
	                                [OfficialLogbookB] [nvarchar](max) NULL,
	                                [OfficialLogbookC] [nvarchar](max) NULL,
	                                [OfficialLogbookD] [nvarchar](max) NULL,
	                                [OfficialLogbookE] [nvarchar](max) NULL,
	                                [DeckLogbook] [nvarchar](max) NULL,
	                                [Listofcrew] [nvarchar](max) NULL,
	                                [LastHose] [nvarchar](max) NULL,
	                                [PassagePlanning] [nvarchar](max) NULL,
	                                [LoadingComputer] [nvarchar](max) NULL,
	                                [EngineLogbook] [nvarchar](max) NULL,
	                                [OilRecordBook] [nvarchar](max) NULL,
	                                [RiskAssessments] [nvarchar](max) NULL,
	                                [GMDSSLogbook] [nvarchar](max) NULL,
	                                [DeckLogbook5D] [nvarchar](max) NULL,
	                                [GarbageRecordBook] [nvarchar](max) NULL,
	                                [BallastWaterRecordBook] [nvarchar](max) NULL,
	                                [CargoRecordBook] [nvarchar](max) NULL,
	                                [EmissionsControlManual] [nvarchar](max) NULL,
	                                [LGR] [nvarchar](max) NULL,
	                                [PEER] [nvarchar](max) NULL,
	                                [RecordKeepingComments] [nvarchar](max) NULL,
	                                [LastPortStateControl] [nvarchar](max) NULL,
	                                [LiferaftsComment] [nvarchar](max) NULL,
	                                [releasesComment] [nvarchar](max) NULL,
	                                [LifeboatComment] [nvarchar](max) NULL,
	                                [LifeboatdavitComment] [nvarchar](max) NULL,
	                                [LifeboatequipmentComment] [nvarchar](max) NULL,
	                                [RescueboatComment] [nvarchar](max) NULL,
	                                [RescueboatequipmentComment] [nvarchar](max) NULL,
	                                [RescueboatoutboardmotorComment] [nvarchar](max) NULL,
	                                [RescueboatdavitComment] [nvarchar](max) NULL,
	                                [DeckComment] [nvarchar](max) NULL,
	                                [PyrotechnicsComment] [nvarchar](max) NULL,
	                                [EPIRBComment] [nvarchar](max) NULL,
	                                [SARTsComment] [nvarchar](max) NULL,
	                                [GMDSSComment] [nvarchar](max) NULL,
	                                [ManoverboardComment] [nvarchar](max) NULL,
	                                [LinethrowingapparatusComment] [nvarchar](max) NULL,
	                                [FireextinguishersComment] [nvarchar](max) NULL,
	                                [EmergencygeneratorComment] [nvarchar](max) NULL,
	                                [CO2roomComment] [nvarchar](max) NULL,
	                                [SurvivalComment] [nvarchar](max) NULL,
	                                [LifejacketComment] [nvarchar](max) NULL,
	                                [FiremansComment] [nvarchar](max) NULL,
	                                [LifebuoysComment] [nvarchar](max) NULL,
	                                [FireboxesComment] [nvarchar](max) NULL,
	                                [EmergencybellsComment] [nvarchar](max) NULL,
	                                [EmergencylightingComment] [nvarchar](max) NULL,
	                                [FireplanComment] [nvarchar](max) NULL,
	                                [DamageComment] [nvarchar](max) NULL,
	                                [EmergencyplansComment] [nvarchar](max) NULL,
	                                [MusterlistComment] [nvarchar](max) NULL,
	                                [SafetysignsComment] [nvarchar](max) NULL,
	                                [EmergencysteeringComment] [nvarchar](max) NULL,
	                                [StatutoryemergencydrillsComment] [nvarchar](max) NULL,
	                                [EEBDComment] [nvarchar](max) NULL,
	                                [OxygenComment] [nvarchar](max) NULL,
	                                [MultigasdetectorComment] [nvarchar](max) NULL,
	                                [GasdetectorComment] [nvarchar](max) NULL,
	                                [SufficientquantityComment] [nvarchar](max) NULL,
	                                [BASetsComment] [nvarchar](max) NULL,
	                                [SafetyComment] [nvarchar](max) NULL,
	                                [GangwayComment] [nvarchar](max) NULL,
	                                [RestrictedComment] [nvarchar](max) NULL,
	                                [OutsideComment] [nvarchar](max) NULL,
	                                [EntrancedoorsComment] [nvarchar](max) NULL,
	                                [AccommodationComment] [nvarchar](max) NULL,
	                                [GMDSSComment5G] [nvarchar](max) NULL,
	                                [VariousComment] [nvarchar](max) NULL,
	                                [SSOComment] [nvarchar](max) NULL,
	                                [SecuritylogbookComment] [nvarchar](max) NULL,
	                                [Listoflast10portsComment] [nvarchar](max) NULL,
	                                [PFSOComment] [nvarchar](max) NULL,
	                                [SecuritylevelComment] [nvarchar](max) NULL,
	                                [DrillsandtrainingComment] [nvarchar](max) NULL,
	                                [DOSComment] [nvarchar](max) NULL,
	                                [SSASComment] [nvarchar](max) NULL,
	                                [VisitorslogbookComment] [nvarchar](max) NULL,
	                                [KeyregisterComment] [nvarchar](max) NULL,
	                                [ShipSecurityComment] [nvarchar](max) NULL,
	                                [SecurityComment] [nvarchar](max) NULL,
	                                [NauticalchartsComment] [nvarchar](max) NULL,
	                                [NoticetomarinersComment] [nvarchar](max) NULL,
	                                [ListofradiosignalsComment] [nvarchar](max) NULL,
	                                [ListoflightsComment] [nvarchar](max) NULL,
	                                [SailingdirectionsComment] [nvarchar](max) NULL,
	                                [TidetablesComment] [nvarchar](max) NULL,
	                                [NavtexandprinterComment] [nvarchar](max) NULL,
	                                [RadarsComment] [nvarchar](max) NULL,
	                                [GPSComment] [nvarchar](max) NULL,
	                                [AISComment] [nvarchar](max) NULL,
	                                [VDRComment] [nvarchar](max) NULL,
	                                [ECDISComment] [nvarchar](max) NULL,
	                                [EchosounderComment] [nvarchar](max) NULL,
	                                [ADPbackuplaptopComment] [nvarchar](max) NULL,
	                                [ColourprinterComment] [nvarchar](max) NULL,
	                                [VHFDSCtransceiverComment] [nvarchar](max) NULL,
	                                [radioinstallationComment] [nvarchar](max) NULL,
	                                [InmarsatCComment] [nvarchar](max) NULL,
	                                [MagneticcompassComment] [nvarchar](max) NULL,
	                                [SparecompassbowlComment] [nvarchar](max) NULL,
	                                [CompassobservationbookComment] [nvarchar](max) NULL,
	                                [GyrocompassComment] [nvarchar](max) NULL,
	                                [RudderindicatorComment] [nvarchar](max) NULL,
	                                [SpeedlogComment] [nvarchar](max) NULL,
	                                [NavigationComment] [nvarchar](max) NULL,
	                                [SignalflagsComment] [nvarchar](max) NULL,
	                                [RPMComment] [nvarchar](max) NULL,
	                                [BasicmanoeuvringdataComment] [nvarchar](max) NULL,
	                                [MasterstandingordersComment] [nvarchar](max) NULL,
	                                [MasternightordersbookComment] [nvarchar](max) NULL,
	                                [SextantComment] [nvarchar](max) NULL,
	                                [AzimuthmirrorComment] [nvarchar](max) NULL,
	                                [BridgepostersComment] [nvarchar](max) NULL,
	                                [ReviewofplannedComment] [nvarchar](max) NULL,
	                                [BridgebellbookComment] [nvarchar](max) NULL,
	                                [BridgenavigationalComment] [nvarchar](max) NULL,
	                                [SecurityEquipmentComment] [nvarchar](max) NULL,
	                                [NavigationPost] [nvarchar](max) NULL,
	                                [GeneralComment] [nvarchar](max) NULL,
	                                [MedicinestorageComment] [nvarchar](max) NULL,
	                                [MedicinechestcertificateComment] [nvarchar](max) NULL,
	                                [InventoryStoresComment] [nvarchar](max) NULL,
	                                [OxygencylindersComment] [nvarchar](max) NULL,
	                                [StretcherComment] [nvarchar](max) NULL,
	                                [SalivaComment] [nvarchar](max) NULL,
	                                [AlcoholComment] [nvarchar](max) NULL,
	                                [HospitalComment] [nvarchar](max) NULL,
	                                [GeneralGalleyComment] [nvarchar](max) NULL,
	                                [HygieneComment] [nvarchar](max) NULL,
	                                [FoodstorageComment] [nvarchar](max) NULL,
	                                [FoodlabellingComment] [nvarchar](max) NULL,
	                                [GalleyriskassessmentComment] [nvarchar](max) NULL,
	                                [FridgetemperatureComment] [nvarchar](max) NULL,
	                                [FoodandProvisionsComment] [nvarchar](max) NULL,
	                                [GalleyComment] [nvarchar](max) NULL,
	                                [ConditionComment] [nvarchar](max) NULL,
	                                [PaintworkComment] [nvarchar](max) NULL,
	                                [LightingComment] [nvarchar](max) NULL,
	                                [PlatesComment] [nvarchar](max) NULL,
	                                [BilgesComment] [nvarchar](max) NULL,
	                                [PipelinesandvalvesComment] [nvarchar](max) NULL,
	                                [LeakageComment] [nvarchar](max) NULL,
	                                [EquipmentComment] [nvarchar](max) NULL,
	                                [OilywaterseparatorComment] [nvarchar](max) NULL,
	                                [FueloiltransferplanComment] [nvarchar](max) NULL,
	                                [SteeringgearComment] [nvarchar](max) NULL,
	                                [WorkshopandequipmentComment] [nvarchar](max) NULL,
	                                [SoundingpipesComment] [nvarchar](max) NULL,
	                                [EnginecontrolComment] [nvarchar](max) NULL,
	                                [ChiefEngineernightordersbookComment] [nvarchar](max) NULL,
	                                [ChiefEngineerstandingordersComment] [nvarchar](max) NULL,
	                                [PreUMSComment] [nvarchar](max) NULL,
	                                [EnginebellbookComment] [nvarchar](max) NULL,
	                                [LockoutComment] [nvarchar](max) NULL,
	                                [EngineRoomComment] [nvarchar](max) NULL,
	                                [CleanlinessandhygieneComment] [nvarchar](max) NULL,
	                                [ConditionComment5M] [nvarchar](max) NULL,
	                                [PaintworkComment5M] [nvarchar](max) NULL,
	                                [SignalmastandstaysComment] [nvarchar](max) NULL,
	                                [MonkeyislandComment] [nvarchar](max) NULL,
	                                [FireDampersComment] [nvarchar](max) NULL,
	                                [RailsBulwarksComment] [nvarchar](max) NULL,
	                                [WatertightdoorsComment] [nvarchar](max) NULL,
	                                [VentilatorsComment] [nvarchar](max) NULL,
	                                [WinchesComment] [nvarchar](max) NULL,
	                                [FairleadsComment] [nvarchar](max) NULL,
	                                [MooringLinesComment] [nvarchar](max) NULL,
	                                [EmergencyShutOffsComment] [nvarchar](max) NULL,
	                                [RadioaerialsComment] [nvarchar](max) NULL,
	                                [SOPEPlockerComment] [nvarchar](max) NULL,
	                                [ChemicallockerComment] [nvarchar](max) NULL,
	                                [AntislippaintComment] [nvarchar](max) NULL,
	                                [SuperstructureComment] [nvarchar](max) NULL,
	                                [CabinsComment] [nvarchar](max) NULL,
	                                [OfficesComment] [nvarchar](max) NULL,
	                                [MessroomsComment] [nvarchar](max) NULL,
	                                [ToiletsComment] [nvarchar](max) NULL,
	                                [LaundryroomComment] [nvarchar](max) NULL,
	                                [ChangingroomComment] [nvarchar](max) NULL,
	                                [OtherComment] [nvarchar](max) NULL,
	                                [ConditionComment5N] [nvarchar](max) NULL,
	                                [SelfclosingfiredoorsComment] [nvarchar](max) NULL,
	                                [StairwellsComment] [nvarchar](max) NULL,
	                                [SuperstructureInternalComment] [nvarchar](max) NULL,
	                                [PortablegangwayComment] [nvarchar](max) NULL,
	                                [SafetynetComment] [nvarchar](max) NULL,
	                                [AccommodationLadderComment] [nvarchar](max) NULL,
	                                [SafeaccessprovidedComment] [nvarchar](max) NULL,
	                                [PilotladdersComment] [nvarchar](max) NULL,
	                                [BoardingEquipmentComment] [nvarchar](max) NULL,
	                                [CleanlinessComment] [nvarchar](max) NULL,
	                                [PaintworkComment5P] [nvarchar](max) NULL,
	                                [ShipsiderailsComment] [nvarchar](max) NULL,
	                                [WeathertightdoorsComment] [nvarchar](max) NULL,
	                                [FirehydrantsComment] [nvarchar](max) NULL,
	                                [VentilatorsComment5P] [nvarchar](max) NULL,
	                                [ManholecoversComment] [nvarchar](max) NULL,
	                                [MainDeckAreaComment] [nvarchar](max) NULL,
	                                [ConditionComment5Q] [nvarchar](max) NULL,
	                                [PaintworkComment5Q] [nvarchar](max) NULL,
	                                [MechanicaldamageComment] [nvarchar](max) NULL,
	                                [AccessladdersComment] [nvarchar](max) NULL,
	                                [ManholecoversComment5Q] [nvarchar](max) NULL,
	                                [HoldbilgeComment] [nvarchar](max) NULL,
	                                [AccessdoorsComment] [nvarchar](max) NULL,
	                                [ConditionHatchCoversComment] [nvarchar](max) NULL,
	                                [PaintworkHatchCoversComment] [nvarchar](max) NULL,
	                                [RubbersealsComment] [nvarchar](max) NULL,
	                                [SignsofhatchesComment] [nvarchar](max) NULL,
	                                [SealingtapeComment] [nvarchar](max) NULL,
	                                [ConditionofhydraulicsComment] [nvarchar](max) NULL,
	                                [PortablebulkheadsComment] [nvarchar](max) NULL,
	                                [TweendecksComment] [nvarchar](max) NULL,
	                                [HatchcoamingComment] [nvarchar](max) NULL,
	                                [ConditionCargoCranesComment] [nvarchar](max) NULL,
	                                [GantrycranealarmComment] [nvarchar](max) NULL,
	                                [GantryconditionComment] [nvarchar](max) NULL,
	                                [CargoHoldsComment] [nvarchar](max) NULL,
	                                [CleanlinessComment5R] [nvarchar](max) NULL,
	                                [PaintworkComment5R] [nvarchar](max) NULL,
	                                [TriphazardsComment] [nvarchar](max) NULL,
	                                [WindlassComment] [nvarchar](max) NULL,
	                                [CablesComment] [nvarchar](max) NULL,
	                                [WinchesComment5R] [nvarchar](max) NULL,
	                                [FairleadsComment5R] [nvarchar](max) NULL,
	                                [MooringComment] [nvarchar](max) NULL,
	                                [HatchToforecastlespaceComment] [nvarchar](max) NULL,
	                                [VentilatorsComment5R] [nvarchar](max) NULL,
	                                [BellComment] [nvarchar](max) NULL,
	                                [ForemastComment] [nvarchar](max) NULL,
	                                [FireComment] [nvarchar](max) NULL,
	                                [RailsComment] [nvarchar](max) NULL,
	                                [AntislippaintComment5R] [nvarchar](max) NULL,

	                                [SnapBackZoneComment] [nvarchar](max) NULL,
	                                [ConditionGantryCranesComment] [nvarchar](max) NULL,
	                                [MedicalLogBookComment] [nvarchar](max) NULL,
	                                [DrugsNarcoticsComment] [nvarchar](max) NULL,
	                                [DefibrillatorComment] [nvarchar](max) NULL,
	                                [RPWaterHandbook] [nvarchar](max) NULL,
	                                [BioRPWH] [nvarchar](max) NULL,
	                                [PRE] [nvarchar](max) NULL,
	                                [NoiseVibrationFile] [nvarchar](max) NULL,
	                                [BioMPR] [nvarchar](max) NULL,
	                                [AsbestosPlan] [nvarchar](max) NULL,
	                                [ShipPublicAddrComment] [nvarchar](max) NULL,
	                                [BridgewindowswiperssprayComment] [nvarchar](max) NULL,
	                                [BridgewindowswipersComment] [nvarchar](max) NULL,
	                                [DaylightSignalsComment] [nvarchar](max) NULL,
	                                [LiferaftDavitComment] [nvarchar](max) NULL,
	                                [CylindersLockerComment] [nvarchar](max) NULL,
	                                [SnapBackZone5NComment] [nvarchar](max) NULL,
	                                [ADPPublicationsComment] [nvarchar](max) NULL,

	                                [ForecastleComment] [nvarchar](max) NULL,
	                                [CleanlinessComment5S] [nvarchar](max) NULL,
	                                [PaintworkComment5S] [nvarchar](max) NULL,
	                                [ForepeakComment] [nvarchar](max) NULL,
	                                [ChainlockerComment] [nvarchar](max) NULL,
	                                [LightingComment5S] [nvarchar](max) NULL,
	                                [AccesssafetychainComment] [nvarchar](max) NULL,
	                                [EmergencyfirepumpComment] [nvarchar](max) NULL,
	                                [BowthrusterandroomComment] [nvarchar](max) NULL,
	                                [SparemooringlinesComment] [nvarchar](max) NULL,
	                                [PaintlockerComment] [nvarchar](max) NULL,
	                                [ForecastleSpaceComment] [nvarchar](max) NULL,
	                                [BoottopComment] [nvarchar](max) NULL,
	                                [TopsidesComment] [nvarchar](max) NULL,
	                                [AntifoulingComment] [nvarchar](max) NULL,
	                                [DraftandplimsollComment] [nvarchar](max) NULL,
	                                [FoulingComment] [nvarchar](max) NULL,
	                                [MechanicalComment] [nvarchar](max) NULL,
	                                [HullComment] [nvarchar](max) NULL,
	                                [SummaryComment] [nvarchar](max) NULL,
                                    [SavedAsDraft] [bit] NULL,
									[isDelete] [int] NOT NULL DEFAULT 0,
                                    PRIMARY KEY CLUSTERED 
                                    (
	                                    [GIRFormID] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                                    ";
            return tableQuery;
        }
        public static string GIRSafeManningRequirementsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRSafeManningRequirements](
                                [SafeManningRequirementsID] [int] IDENTITY(1,1) NOT NULL,
	                            [GIRFormID] [bigint] NULL,
	                            [Rank] [nvarchar](max) NULL,
	                            [RequiredbySMD] [bit] NULL,
	                            [OnBoard] [bit] NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
	                            [Ship] [nvarchar](max) NULL,
								[UniqueFormID] uniqueidentifier NULL,
                                PRIMARY KEY CLUSTERED
                                (
                                    [SafeManningRequirementsID] ASC
                                )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
                                ) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
            return tableQuery;
        }
        public static string GIRCrewDocumentsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRCrewDocuments](
	                            [CrewDocumentsID] [int] IDENTITY(1,1) NOT NULL,
	                            [GIRFormID] [bigint] NULL,
	                            [CrewmemberName] [nvarchar](max) NULL,
	                            [CertificationDetail] [nvarchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
	                            [Ship] [nvarchar](max) NULL,
								[UniqueFormID] uniqueidentifier NULL,
                                PRIMARY KEY CLUSTERED 
                                (
	                                [CrewDocumentsID] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            return tableQuery;
        }
        public static string GIRRestandWorkHoursTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRRestandWorkHours](
	                            [RestandWorkHoursID] [int] IDENTITY(1,1) NOT NULL,
	                            [GIRFormID] [bigint] NULL,
	                            [CrewmemberName] [nvarchar](max) NULL,
	                            [RestAndWorkDetail] [nvarchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
	                            [Ship] [nvarchar](max) NULL,
								[UniqueFormID] uniqueidentifier NULL,
                                PRIMARY KEY CLUSTERED 
                                (
	                                [RestandWorkHoursID] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            return tableQuery;
        }
        public static string GIRDeficienciesTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRDeficiencies](
	                            [DeficienciesID] [int] IDENTITY(1,1) NOT NULL,
	                            [GIRFormID] [bigint] NULL,
	                            [No] [int] NOT NULL,
	                            [DateRaised] [datetime] NULL,
	                            [Deficiency] [nvarchar](max) NULL,
	                            [DateClosed] [datetime] NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
	                            [Ship] [nvarchar](max) NULL,
	                            [IsClose] [bit] NULL,
	                            [ReportType] [nvarchar](max) NULL,
	                            [FileName] [nvarchar](max) NULL,
	                            [StorePath] [nvarchar](max) NULL,
                                [SIRNo] [nvarchar](max) NULL,
								[ItemNo] [nvarchar](150) NULL,
                                [Section] [nvarchar](500) NULL,
								[DeficienciesUniqueID] uniqueidentifier NULL,
								[UniqueFormID] uniqueidentifier NULL,
								[isDelete] int,
                                PRIMARY KEY CLUSTERED 
                                (
	                                [DeficienciesID] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            return tableQuery;
        }
        public static string GIRPhotographsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRPhotographs](
	                            [PhotographsID] [int] IDENTITY(1,1) NOT NULL,
	                            [GIRFormID] [bigint] NULL,
	                            [ImagePath] [nvarchar](max) NULL,
	                            [ImageCaption] [nvarchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
	                            [Ship] [nvarchar](max) NULL,
                                [FileName] [nvarchar](max) NULL,
								[UniqueFormID] uniqueidentifier NULL,
                                PRIMARY KEY CLUSTERED 
                                (
	                                [PhotographsID] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            return tableQuery;
        }
        public static string GIRDeficienciesFileTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRDeficienciesFiles](
	                            [GIRDeficienciesFileID] [int] IDENTITY(1,1) NOT NULL,
	                            [DeficienciesID] [bigint] NULL,
	                            [FileName] [nvarchar](max) NULL,
	                            [StorePath] [nvarchar](max) NULL,   
								[DeficienciesUniqueID] uniqueidentifier NULL,
                                PRIMARY KEY CLUSTERED 
                                (
	                                [GIRDeficienciesFileID] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            return tableQuery;
        }
        public static string GIRDeficienciesNoteTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRDeficienciesNotes](
	                            [NoteID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [DeficienciesID] [bigint] NULL,
	                            [GIRFormID] [bigint] NULL,
	                            [UserName] [nvarchar](max) NULL,
	                            [Comment] [nvarchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [ModifyDate] [datetime] NULL,
								[DeficienciesUniqueID] uniqueidentifier NULL,
								[NoteUniqueID] uniqueidentifier NULL
                                )";
            return tableQuery;
        }
        public static string GIRDeficienciesCommentFileTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRDeficienciesCommentFile](
	                            [GIRCommentFileID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [NoteID] [bigint] NULL,
	                            [DeficienciesID] [bigint] NULL,
	                            [FileName] [nvarchar](max) NULL,
	                            [StorePath] [nvarchar](max) NULL,
	                            [IsUpload] [nvarchar](max) NULL,
								[NoteUniqueID] uniqueidentifier NULL, 
								[CommentFileUniqueID] uniqueidentifier NULL
                                )";
            return tableQuery;
        }
        public static string GIRDeficienciesInitialActionsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRDeficienciesInitialActions](
	                            [GIRInitialID] [bigint] IDENTITY(1,1) NOT NULL,
								[DeficienciesID] [bigint] NULL,
								[GIRFormID] [bigint] NULL,
								[Name] [nvarchar](max) NULL,
								[Description] [nvarchar](max) NULL,
								[CreatedDate] [datetime] NULL,
								[DeficienciesUniqueID] uniqueidentifier NULL,
								[IniActUniqueID] uniqueidentifier NULL
                                )";
            return tableQuery;
        }
		public static string GIRDeficienciesInitialActionsFileTableQuery()
		{
			string tableQuery = @"CREATE TABLE [dbo].[GIRDeficienciesInitialActionsFile](
								[GIRFileID] [bigint] IDENTITY(1,1) NOT NULL,
								[GIRInitialID] [bigint] NULL,
								[DeficienciesID] [bigint] NULL,
								[FileName] [nvarchar](max) NULL,
								[StorePath] [nvarchar](max) NULL,
								[IsUpload] [nvarchar](max) NULL,
								[IniActUniqueID] uniqueidentifier NULL,
								[IniActFileUniqueID] uniqueidentifier NULL
                                )";
			return tableQuery;
		}
		public static string GIRDeficienciesResolutionTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[GIRDeficienciesResolution](
								[GIRResolutionID] [bigint] IDENTITY(1,1) NOT NULL,
								[DeficienciesID] [bigint] NULL,
								[GIRFormID] [bigint] NULL,
								[Name] [nvarchar](max) NULL,
								[Resolution] [nvarchar](max) NULL,
								[CreatedDate] [datetime] NULL,
								[DeficienciesUniqueID] uniqueidentifier NULL,
								[ResolutionUniqueID] uniqueidentifier NULL
                                )";
            return tableQuery;
        }
		public static string GIRDeficienciesResolutionFileTableQuery()
		{
			string tableQuery = @"CREATE TABLE [dbo].[GIRDeficienciesResolutionFile](
								[GIRFileID] [bigint] IDENTITY(1,1) NOT NULL,
								[GIRResolutionID] [bigint] NULL,
								[DeficienciesID] [bigint] NULL,
								[FileName] [nvarchar](max) NULL,
								[StorePath] [nvarchar](max) NULL,
								[IsUpload] [nvarchar](max) NULL,
								[ResolutionUniqueID] uniqueidentifier NULL,
								[ResolutionFileUniqueID] uniqueidentifier NULL
                                )";
			return tableQuery;
		}
		#endregion

		#region SIRTable
		public static string SuperintendedInspectionReportTableQuery()
        {
			// RDBJ 02/15/2022 Added Section9_16_Condition, Section9_16_Comment, Section9_17_Condition, Section9_17_Comment, Section18_8_Condition, Section18_8_Comment, Section18_9_Condition, Section18_9_Comment // RDBJ 01/05/2022 Added isDelete
			string tableQuery = @"CREATE TABLE [dbo].[SuperintendedInspectionReport](
	                            [SIRFormID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [ShipID] [bigint] NULL,
	                            [ShipName] [nvarchar](max) NULL,
	                            [Date] [datetime] NULL,
	                            [Port] [nvarchar](max) NULL,
	                            [Master] [nvarchar](max) NULL,
	                            [Superintended] [nvarchar](max) NULL,
	                            [Section1_1_Condition] [nvarchar](max) NULL,
	                            [Section1_1_Comment] [nvarchar](max) NULL,
	                            [Section1_2_Condition] [nvarchar](max) NULL,
	                            [Section1_2_Comment] [nvarchar](max) NULL,
	                            [Section1_3_Condition] [nvarchar](max) NULL,
	                            [Section1_3_Comment] [nvarchar](max) NULL,
	                            [Section1_4_Condition] [nvarchar](max) NULL,
	                            [Section1_4_Comment] [nvarchar](max) NULL,
	                            [Section1_5_Condition] [nvarchar](max) NULL,
	                            [Section1_5_Comment] [nvarchar](max) NULL,
	                            [Section1_6_Condition] [nvarchar](max) NULL,
	                            [Section1_6_Comment] [nvarchar](max) NULL,
	                            [Section1_7_Condition] [nvarchar](max) NULL,
	                            [Section1_7_Comment] [nvarchar](max) NULL,
	                            [Section1_8_Condition] [nvarchar](max) NULL,
	                            [Section1_8_Comment] [nvarchar](max) NULL,
	                            [Section1_9_Condition] [nvarchar](max) NULL,
	                            [Section1_9_Comment] [nvarchar](max) NULL,
	                            [Section1_10_Condition] [nvarchar](max) NULL,
	                            [Section1_10_Comment] [nvarchar](max) NULL,
	                            [Section1_11_Condition] [nvarchar](max) NULL,
	                            [Section1_11_Comment] [nvarchar](max) NULL,
	                            [Section2_1_Condition] [nvarchar](max) NULL,
	                            [Section2_1_Comment] [nvarchar](max) NULL,
	                            [Section2_2_Condition] [nvarchar](max) NULL,
	                            [Section2_2_Comment] [nvarchar](max) NULL,
	                            [Section2_3_Condition] [nvarchar](max) NULL,
	                            [Section2_3_Comment] [nvarchar](max) NULL,
	                            [Section2_4_Condition] [nvarchar](max) NULL,
	                            [Section2_4_Comment] [nvarchar](max) NULL,
	                            [Section2_5_Condition] [nvarchar](max) NULL,
	                            [Section2_5_Comment] [nvarchar](max) NULL,
	                            [Section2_6_Condition] [nvarchar](max) NULL,
	                            [Section2_6_Comment] [nvarchar](max) NULL,
	                            [Section2_7_Condition] [nvarchar](max) NULL,
	                            [Section2_7_Comment] [nvarchar](max) NULL,
	                            [Section3_1_Condition] [nvarchar](max) NULL,
	                            [Section3_1_Comment] [nvarchar](max) NULL,
	                            [Section3_2_Condition] [nvarchar](max) NULL,
	                            [Section3_2_Comment] [nvarchar](max) NULL,
	                            [Section3_3_Condition] [nvarchar](max) NULL,
	                            [Section3_3_Comment] [nvarchar](max) NULL,
	                            [Section3_4_Condition] [nvarchar](max) NULL,
	                            [Section3_4_Comment] [nvarchar](max) NULL,
	                            [Section3_5_Condition] [nvarchar](max) NULL,
	                            [Section3_5_Comment] [nvarchar](max) NULL,
	                            [Section4_1_Condition] [nvarchar](max) NULL,
	                            [Section4_1_Comment] [nvarchar](max) NULL,
	                            [Section4_2_Condition] [nvarchar](max) NULL,
	                            [Section4_2_Comment] [nvarchar](max) NULL,
	                            [Section4_3_Condition] [nvarchar](max) NULL,
	                            [Section4_3_Comment] [nvarchar](max) NULL,
	                            [Section5_1_Condition] [nvarchar](max) NULL,
	                            [Section5_1_Comment] [nvarchar](max) NULL,
	                            [Section5_6_Condition] [nvarchar](max) NULL,
	                            [Section5_6_Comment] [nvarchar](max) NULL,
	                            [Section5_8_Condition] [nvarchar](max) NULL,
	                            [Section5_8_Comment] [nvarchar](max) NULL,
	                            [Section5_9_Condition] [nvarchar](max) NULL,
	                            [Section5_9_Comment] [nvarchar](max) NULL,
	                            [Section6_1_Condition] [nvarchar](max) NULL,
	                            [Section6_1_Comment] [nvarchar](max) NULL,
	                            [Section6_2_Condition] [nvarchar](max) NULL,
	                            [Section6_2_Comment] [nvarchar](max) NULL,
	                            [Section6_3_Condition] [nvarchar](max) NULL,
	                            [Section6_3_Comment] [nvarchar](max) NULL,
	                            [Section6_4_Condition] [nvarchar](max) NULL,
	                            [Section6_4_Comment] [nvarchar](max) NULL,
	                            [Section6_5_Condition] [nvarchar](max) NULL,
	                            [Section6_5_Comment] [nvarchar](max) NULL,
	                            [Section6_6_Condition] [nvarchar](max) NULL,
	                            [Section6_6_Comment] [nvarchar](max) NULL,
	                            [Section6_7_Condition] [nvarchar](max) NULL,
	                            [Section6_7_Comment] [nvarchar](max) NULL,
	                            [Section6_8_Condition] [nvarchar](max) NULL,
	                            [Section6_8_Comment] [nvarchar](max) NULL,
	                            [Section7_1_Condition] [nvarchar](max) NULL,
	                            [Section7_1_Comment] [nvarchar](max) NULL,
	                            [Section7_2_Condition] [nvarchar](max) NULL,
	                            [Section7_2_Comment] [nvarchar](max) NULL,
	                            [Section7_3_Condition] [nvarchar](max) NULL,
	                            [Section7_3_Comment] [nvarchar](max) NULL,
	                            [Section7_4_Condition] [nvarchar](max) NULL,
	                            [Section7_4_Comment] [nvarchar](max) NULL,
	                            [Section7_5_Condition] [nvarchar](max) NULL,
	                            [Section7_5_Comment] [nvarchar](max) NULL,
	                            [Section7_6_Condition] [nvarchar](max) NULL,
	                            [Section7_6_Comment] [nvarchar](max) NULL,
	                            [Section8_1_Condition] [nvarchar](max) NULL,
	                            [Section8_1_Comment] [nvarchar](max) NULL,
	                            [Section8_2_Condition] [nvarchar](max) NULL,
	                            [Section8_2_Comment] [nvarchar](max) NULL,
	                            [Section8_3_Condition] [nvarchar](max) NULL,
	                            [Section8_3_Comment] [nvarchar](max) NULL,
	                            [Section8_4_Condition] [nvarchar](max) NULL,
	                            [Section8_4_Comment] [nvarchar](max) NULL,
	                            [Section8_5_Condition] [nvarchar](max) NULL,
	                            [Section8_5_Comment] [nvarchar](max) NULL,
	                            [Section8_6_Condition] [nvarchar](max) NULL,
	                            [Section8_6_Comment] [nvarchar](max) NULL,
	                            [Section8_7_Condition] [nvarchar](max) NULL,
	                            [Section8_7_Comment] [nvarchar](max) NULL,
	                            [Section8_8_Condition] [nvarchar](max) NULL,
	                            [Section8_8_Comment] [nvarchar](max) NULL,
	                            [Section8_9_Condition] [nvarchar](max) NULL,
	                            [Section8_9_Comment] [nvarchar](max) NULL,
	                            [Section8_10_Condition] [nvarchar](max) NULL,
	                            [Section8_10_Comment] [nvarchar](max) NULL,
	                            [Section8_11_Condition] [nvarchar](max) NULL,
	                            [Section8_11_Comment] [nvarchar](max) NULL,
	                            [Section8_12_Condition] [nvarchar](max) NULL,
	                            [Section8_12_Comment] [nvarchar](max) NULL,
	                            [Section8_13_Condition] [nvarchar](max) NULL,
	                            [Section8_13_Comment] [nvarchar](max) NULL,
	                            [Section8_14_Condition] [nvarchar](max) NULL,
	                            [Section8_14_Comment] [nvarchar](max) NULL,
	                            [Section8_15_Condition] [nvarchar](max) NULL,
	                            [Section8_15_Comment] [nvarchar](max) NULL,
	                            [Section8_16_Condition] [nvarchar](max) NULL,
	                            [Section8_16_Comment] [nvarchar](max) NULL,
	                            [Section8_17_Condition] [nvarchar](max) NULL,
	                            [Section8_17_Comment] [nvarchar](max) NULL,
	                            [Section8_18_Condition] [nvarchar](max) NULL,
	                            [Section8_18_Comment] [nvarchar](max) NULL,
	                            [Section8_19_Condition] [nvarchar](max) NULL,
	                            [Section8_19_Comment] [nvarchar](max) NULL,
	                            [Section8_20_Condition] [nvarchar](max) NULL,
	                            [Section8_20_Comment] [nvarchar](max) NULL,
	                            [Section8_21_Condition] [nvarchar](max) NULL,
	                            [Section8_21_Comment] [nvarchar](max) NULL,
	                            [Section8_22_Condition] [nvarchar](max) NULL,
	                            [Section8_22_Comment] [nvarchar](max) NULL,
	                            [Section8_23_Condition] [nvarchar](max) NULL,
	                            [Section8_23_Comment] [nvarchar](max) NULL,
	                            [Section8_24_Condition] [nvarchar](max) NULL,
	                            [Section8_24_Comment] [nvarchar](max) NULL,
	                            [Section8_25_Condition] [nvarchar](max) NULL,
	                            [Section8_25_Comment] [nvarchar](max) NULL,
	                            [Section9_1_Condition] [nvarchar](max) NULL,
	                            [Section9_1_Comment] [nvarchar](max) NULL,
	                            [Section9_2_Condition] [nvarchar](max) NULL,
	                            [Section9_2_Comment] [nvarchar](max) NULL,
	                            [Section9_3_Condition] [nvarchar](max) NULL,
	                            [Section9_3_Comment] [nvarchar](max) NULL,
	                            [Section9_4_Condition] [nvarchar](max) NULL,
	                            [Section9_4_Comment] [nvarchar](max) NULL,
	                            [Section9_5_Condition] [nvarchar](max) NULL,
	                            [Section9_5_Comment] [nvarchar](max) NULL,
	                            [Section9_6_Condition] [nvarchar](max) NULL,
	                            [Section9_6_Comment] [nvarchar](max) NULL,
	                            [Section9_7_Condition] [nvarchar](max) NULL,
	                            [Section9_7_Comment] [nvarchar](max) NULL,
	                            [Section9_8_Condition] [nvarchar](max) NULL,
	                            [Section9_8_Comment] [nvarchar](max) NULL,
	                            [Section9_9_Condition] [nvarchar](max) NULL,
	                            [Section9_9_Comment] [nvarchar](max) NULL,
	                            [Section9_10_Condition] [nvarchar](max) NULL,
	                            [Section9_10_Comment] [nvarchar](max) NULL,
	                            [Section9_11_Condition] [nvarchar](max) NULL,
	                            [Section9_11_Comment] [nvarchar](max) NULL,
	                            [Section9_12_Condition] [nvarchar](max) NULL,
	                            [Section9_12_Comment] [nvarchar](max) NULL,
	                            [Section9_13_Condition] [nvarchar](max) NULL,
	                            [Section9_13_Comment] [nvarchar](max) NULL,
	                            [Section9_14_Condition] [nvarchar](max) NULL,
	                            [Section9_14_Comment] [nvarchar](max) NULL,
	                            [Section9_15_Condition] [nvarchar](max) NULL,
	                            [Section9_15_Comment] [nvarchar](max) NULL,
								[Section9_16_Condition] [nvarchar](max) NULL,
	                            [Section9_16_Comment] [nvarchar](max) NULL,
								[Section9_17_Condition] [nvarchar](max) NULL,
	                            [Section9_17_Comment] [nvarchar](max) NULL,
	                            [Section10_1_Condition] [nvarchar](max) NULL,
	                            [Section10_1_Comment] [nvarchar](max) NULL,
	                            [Section10_2_Condition] [nvarchar](max) NULL,
	                            [Section10_2_Comment] [nvarchar](max) NULL,
	                            [Section10_3_Condition] [nvarchar](max) NULL,
	                            [Section10_3_Comment] [nvarchar](max) NULL,
	                            [Section10_4_Condition] [nvarchar](max) NULL,
	                            [Section10_4_Comment] [nvarchar](max) NULL,
	                            [Section10_5_Condition] [nvarchar](max) NULL,
	                            [Section10_5_Comment] [nvarchar](max) NULL,
	                            [Section10_6_Condition] [nvarchar](max) NULL,
	                            [Section10_6_Comment] [nvarchar](max) NULL,
	                            [Section10_7_Condition] [nvarchar](max) NULL,
	                            [Section10_7_Comment] [nvarchar](max) NULL,
	                            [Section10_8_Condition] [nvarchar](max) NULL,
	                            [Section10_8_Comment] [nvarchar](max) NULL,
	                            [Section10_9_Condition] [nvarchar](max) NULL,
	                            [Section10_9_Comment] [nvarchar](max) NULL,
	                            [Section10_10_Condition] [nvarchar](max) NULL,
	                            [Section10_10_Comment] [nvarchar](max) NULL,
	                            [Section10_11_Condition] [nvarchar](max) NULL,
	                            [Section10_11_Comment] [nvarchar](max) NULL,
	                            [Section10_12_Condition] [nvarchar](max) NULL,
	                            [Section10_12_Comment] [nvarchar](max) NULL,
	                            [Section10_13_Condition] [nvarchar](max) NULL,
	                            [Section10_13_Comment] [nvarchar](max) NULL,
	                            [Section10_14_Condition] [nvarchar](max) NULL,
	                            [Section10_14_Comment] [nvarchar](max) NULL,
	                            [Section10_15_Condition] [nvarchar](max) NULL,
	                            [Section10_15_Comment] [nvarchar](max) NULL,
	                            [Section10_16_Condition] [nvarchar](max) NULL,
	                            [Section10_16_Comment] [nvarchar](max) NULL,
	                            [Section11_1_Condition] [nvarchar](max) NULL,
	                            [Section11_1_Comment] [nvarchar](max) NULL,
	                            [Section11_2_Condition] [nvarchar](max) NULL,
	                            [Section11_2_Comment] [nvarchar](max) NULL,
	                            [Section11_3_Condition] [nvarchar](max) NULL,
	                            [Section11_3_Comment] [nvarchar](max) NULL,
	                            [Section11_4_Condition] [nvarchar](max) NULL,
	                            [Section11_4_Comment] [nvarchar](max) NULL,
	                            [Section11_5_Condition] [nvarchar](max) NULL,
	                            [Section11_5_Comment] [nvarchar](max) NULL,
	                            [Section11_6_Condition] [nvarchar](max) NULL,
	                            [Section11_6_Comment] [nvarchar](max) NULL,
	                            [Section11_7_Condition] [nvarchar](max) NULL,
	                            [Section11_7_Comment] [nvarchar](max) NULL,
	                            [Section11_8_Condition] [nvarchar](max) NULL,
	                            [Section11_8_Comment] [nvarchar](max) NULL,
	                            [Section12_1_Condition] [nvarchar](max) NULL,
	                            [Section12_1_Comment] [nvarchar](max) NULL,
	                            [Section12_2_Condition] [nvarchar](max) NULL,
	                            [Section12_2_Comment] [nvarchar](max) NULL,
	                            [Section12_3_Condition] [nvarchar](max) NULL,
	                            [Section12_3_Comment] [nvarchar](max) NULL,
	                            [Section12_4_Condition] [nvarchar](max) NULL,
	                            [Section12_4_Comment] [nvarchar](max) NULL,
	                            [Section12_5_Condition] [nvarchar](max) NULL,
	                            [Section12_5_Comment] [nvarchar](max) NULL,
	                            [Section12_6_Condition] [nvarchar](max) NULL,
	                            [Section12_6_Comment] [nvarchar](max) NULL,
	                            [Section13_1_Condition] [nvarchar](max) NULL,
	                            [Section13_1_Comment] [nvarchar](max) NULL,
	                            [Section13_2_Condition] [nvarchar](max) NULL,
	                            [Section13_2_Comment] [nvarchar](max) NULL,
	                            [Section13_3_Condition] [nvarchar](max) NULL,
	                            [Section13_3_Comment] [nvarchar](max) NULL,
	                            [Section13_4_Condition] [nvarchar](max) NULL,
	                            [Section13_4_Comment] [nvarchar](max) NULL,
	                            [Section14_1_Condition] [nvarchar](max) NULL,
	                            [Section14_1_Comment] [nvarchar](max) NULL,
	                            [Section14_2_Condition] [nvarchar](max) NULL,
	                            [Section14_2_Comment] [nvarchar](max) NULL,
	                            [Section14_3_Condition] [nvarchar](max) NULL,
	                            [Section14_3_Comment] [nvarchar](max) NULL,
	                            [Section14_4_Condition] [nvarchar](max) NULL,
	                            [Section14_4_Comment] [nvarchar](max) NULL,
	                            [Section14_5_Condition] [nvarchar](max) NULL,
	                            [Section14_5_Comment] [nvarchar](max) NULL,
	                            [Section14_6_Condition] [nvarchar](max) NULL,
	                            [Section14_6_Comment] [nvarchar](max) NULL,
	                            [Section14_7_Condition] [nvarchar](max) NULL,
	                            [Section14_7_Comment] [nvarchar](max) NULL,
	                            [Section14_8_Condition] [nvarchar](max) NULL,
	                            [Section14_8_Comment] [nvarchar](max) NULL,
	                            [Section14_9_Condition] [nvarchar](max) NULL,
	                            [Section14_9_Comment] [nvarchar](max) NULL,
	                            [Section14_10_Condition] [nvarchar](max) NULL,
	                            [Section14_10_Comment] [nvarchar](max) NULL,
	                            [Section14_11_Condition] [nvarchar](max) NULL,
	                            [Section14_11_Comment] [nvarchar](max) NULL,
	                            [Section14_12_Condition] [nvarchar](max) NULL,
	                            [Section14_12_Comment] [nvarchar](max) NULL,
	                            [Section14_13_Condition] [nvarchar](max) NULL,
	                            [Section14_13_Comment] [nvarchar](max) NULL,
	                            [Section14_14_Condition] [nvarchar](max) NULL,
	                            [Section14_14_Comment] [nvarchar](max) NULL,
	                            [Section14_15_Condition] [nvarchar](max) NULL,
	                            [Section14_15_Comment] [nvarchar](max) NULL,
	                            [Section14_16_Condition] [nvarchar](max) NULL,
	                            [Section14_16_Comment] [nvarchar](max) NULL,
	                            [Section14_17_Condition] [nvarchar](max) NULL,
	                            [Section14_17_Comment] [nvarchar](max) NULL,
	                            [Section14_18_Condition] [nvarchar](max) NULL,
	                            [Section14_18_Comment] [nvarchar](max) NULL,
	                            [Section14_19_Condition] [nvarchar](max) NULL,
	                            [Section14_19_Comment] [nvarchar](max) NULL,
	                            [Section14_20_Condition] [nvarchar](max) NULL,
	                            [Section14_20_Comment] [nvarchar](max) NULL,
	                            [Section14_21_Condition] [nvarchar](max) NULL,
	                            [Section14_21_Comment] [nvarchar](max) NULL,
	                            [Section14_22_Condition] [nvarchar](max) NULL,
	                            [Section14_22_Comment] [nvarchar](max) NULL,
	                            [Section14_23_Condition] [nvarchar](max) NULL,
	                            [Section14_23_Comment] [nvarchar](max) NULL,
	                            [Section14_24_Condition] [nvarchar](max) NULL,
	                            [Section14_24_Comment] [nvarchar](max) NULL,
	                            [Section14_25_Condition] [nvarchar](max) NULL,
	                            [Section14_25_Comment] [nvarchar](max) NULL,
	                            [Section15_1_Condition] [nvarchar](max) NULL,
	                            [Section15_1_Comment] [nvarchar](max) NULL,
	                            [Section15_2_Condition] [nvarchar](max) NULL,
	                            [Section15_2_Comment] [nvarchar](max) NULL,
	                            [Section15_3_Condition] [nvarchar](max) NULL,
	                            [Section15_3_Comment] [nvarchar](max) NULL,
	                            [Section15_4_Condition] [nvarchar](max) NULL,
	                            [Section15_4_Comment] [nvarchar](max) NULL,
	                            [Section15_5_Condition] [nvarchar](max) NULL,
	                            [Section15_5_Comment] [nvarchar](max) NULL,
	                            [Section15_6_Condition] [nvarchar](max) NULL,
	                            [Section15_6_Comment] [nvarchar](max) NULL,
	                            [Section15_7_Condition] [nvarchar](max) NULL,
	                            [Section15_7_Comment] [nvarchar](max) NULL,
	                            [Section15_8_Condition] [nvarchar](max) NULL,
	                            [Section15_8_Comment] [nvarchar](max) NULL,
	                            [Section15_9_Condition] [nvarchar](max) NULL,
	                            [Section15_9_Comment] [nvarchar](max) NULL,
	                            [Section15_10_Condition] [nvarchar](max) NULL,
	                            [Section15_10_Comment] [nvarchar](max) NULL,
	                            [Section15_11_Condition] [nvarchar](max) NULL,
	                            [Section15_11_Comment] [nvarchar](max) NULL,
	                            [Section15_12_Condition] [nvarchar](max) NULL,
	                            [Section15_12_Comment] [nvarchar](max) NULL,
	                            [Section15_13_Condition] [nvarchar](max) NULL,
	                            [Section15_13_Comment] [nvarchar](max) NULL,
	                            [Section15_14_Condition] [nvarchar](max) NULL,
	                            [Section15_14_Comment] [nvarchar](max) NULL,
	                            [Section15_15_Condition] [nvarchar](max) NULL,
	                            [Section15_15_Comment] [nvarchar](max) NULL,
	                            [Section16_1_Condition] [nvarchar](max) NULL,
	                            [Section16_1_Comment] [nvarchar](max) NULL,
	                            [Section16_2_Condition] [nvarchar](max) NULL,
	                            [Section16_2_Comment] [nvarchar](max) NULL,
	                            [Section16_3_Condition] [nvarchar](max) NULL,
	                            [Section16_3_Comment] [nvarchar](max) NULL,
	                            [Section16_4_Condition] [nvarchar](max) NULL,
	                            [Section16_4_Comment] [nvarchar](max) NULL,
	                            [Section17_1_Condition] [nvarchar](max) NULL,
	                            [Section17_1_Comment] [nvarchar](max) NULL,
	                            [Section17_2_Condition] [nvarchar](max) NULL,
	                            [Section17_2_Comment] [nvarchar](max) NULL,
	                            [Section17_3_Condition] [nvarchar](max) NULL,
	                            [Section17_3_Comment] [nvarchar](max) NULL,
	                            [Section17_4_Condition] [nvarchar](max) NULL,
	                            [Section17_4_Comment] [nvarchar](max) NULL,
	                            [Section17_5_Condition] [nvarchar](max) NULL,
	                            [Section17_5_Comment] [nvarchar](max) NULL,
	                            [Section17_6_Condition] [nvarchar](max) NULL,
	                            [Section17_6_Comment] [nvarchar](max) NULL,
	                            [Section18_1_Condition] [nvarchar](max) NULL,
	                            [Section18_1_Comment] [nvarchar](max) NULL,
	                            [Section18_2_Condition] [nvarchar](max) NULL,
	                            [Section18_2_Comment] [nvarchar](max) NULL,
	                            [Section18_3_Condition] [nvarchar](max) NULL,
	                            [Section18_3_Comment] [nvarchar](max) NULL,
	                            [Section18_4_Condition] [nvarchar](max) NULL,
	                            [Section18_4_Comment] [nvarchar](max) NULL,
	                            [Section18_5_Condition] [nvarchar](max) NULL,
	                            [Section18_5_Comment] [nvarchar](max) NULL,
	                            [Section18_6_Condition] [nvarchar](max) NULL,
	                            [Section18_6_Comment] [nvarchar](max) NULL,
	                            [Section18_7_Condition] [nvarchar](max) NULL,
	                            [Section18_7_Comment] [nvarchar](max) NULL,
								[Section18_8_Condition] [nvarchar](max) NULL,
	                            [Section18_8_Comment] [nvarchar](max) NULL,
								[Section18_9_Condition] [nvarchar](max) NULL,
	                            [Section18_9_Comment] [nvarchar](max) NULL,
	                            [IsSynced] [bit] NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [ModifyDate] [datetime] NULL,
                                [SavedAsDraft] [bit] NULL,
								[UniqueFormID][uniqueidentifier] NULL,
								[FormVersion][numeric](18,2) NULL,
								[isDelete] [int] NOT NULL DEFAULT 0
                                )";
            return tableQuery;
        }
        public static string SIRNotesTableQuery()
        {
			// RDBJ 04/02/2022 Added NotesUniqueID and IsDeleted
			string tableQuery = @"CREATE TABLE [dbo].[SIRNotes](
	                            [NoteID] [bigint] IDENTITY(1,1) PRIMARY KEY,
								[NotesUniqueID][uniqueidentifier] NULL,
	                            [SIRFormID] [bigint] NULL,
	                            [Number] [nvarchar](50) NULL,
	                            [Note] [nvarchar](max) NULL,
								[UniqueFormID][uniqueidentifier] NULL,
								[IsDeleted] [int] NOT NULL DEFAULT 0
                                )";
            return tableQuery;
        }
        public static string SIRAdditionalNotesTableQuery()
        {
			// RDBJ 04/02/2022 Added NotesUniqueID and IsDeleted
			string tableQuery = @"CREATE TABLE [dbo].[SIRAdditionalNotes](
	                            [NoteID] [bigint] IDENTITY(1,1) PRIMARY KEY,
								[NotesUniqueID][uniqueidentifier] NULL,
	                            [SIRFormID] [bigint] NULL,
	                            [Number] [nvarchar](50) NULL,
	                            [Note] [nvarchar](max) NULL,
								[UniqueFormID][uniqueidentifier] NULL,
								[IsDeleted] [int] NOT NULL DEFAULT 0
                                )";

            return tableQuery;
        }
        #endregion

        #region IAF Table
        public static string IAFTableQuery()
        {
			// RDBJ 01/05/2022 Added isDelete
			string tableQuery = @"CREATE TABLE [dbo].[InternalAuditForm](
	                            [InternalAuditFormId] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [ShipId] [bigint] NULL,
	                            [ShipName] [varchar](max) NULL,
	                            [Location] [varchar](max) NULL,
	                            [AuditNo] [varchar](max) NULL,
	                            [AuditTypeISM] [bit] NULL,
	                            [AuditTypeISPS] [bit] NULL,
	                            [AuditTypeMLC] [bit] NULL,
	                            [Date] [datetime] NULL,
	                            [Auditor] [varchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
	                            [IsSynced] [bit] NULL,
								[UniqueFormID][uniqueidentifier] NULL,
								[FormVersion] [numeric](18,2) NULL,
								[isDelete] [int] NOT NULL DEFAULT 0)";
            return tableQuery;
        }
        public static string IAFNotesTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AuditNotes](
	                            [AuditNotesId] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [InternalAuditFormId] [bigint] NULL,
	                            [Number] [varchar](max) NULL,
	                            [Type] [varchar](max) NULL,
	                            [BriefDescription] [varchar](max) NULL,
	                            [Reference] [varchar](max) NULL,
	                            [FullDescription] [varchar](max) NULL,
	                            [CorrectiveAction] [varchar](max) NULL,
	                            [PreventativeAction] [varchar](max) NULL,
	                            [Rank] [varchar](max) NULL,
	                            [Name] [varchar](max) NULL,
	                            [TimeScale] [datetime] NULL,
	                            [Ship] [varchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
								[UniqueFormID][uniqueidentifier] NULL,
								[NotesUniqueID][uniqueidentifier] NULL
								)";
            return tableQuery;
        }
        public static string IAFNotesAttachTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AuditNotesAttachment](
	                            [AuditNotesAttachmentId] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [InternalAuditFormId] [bigint] NULL,
	                            [AuditNotesId] [bigint] NULL,
	                            [FileName] [varchar](max) NULL,
	                            [StorePath] [varchar](max) NULL,
								[NotesUniqueID][uniqueidentifier] NULL)";
            return tableQuery;
        }
        public static string IAFNotesCommnetsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AuditNotesComments](
	                            [CommentsID] [bigint] IDENTITY(1,1) PRIMARY KEY,
                                [AuditNoteID] [bigint] NULL,
	                            [UserName] [nvarchar](250) NULL,
	                            [Comment] [nvarchar](max) NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
								[CommentUniqueID][uniqueidentifier] NULL,
								[NotesUniqueID][uniqueidentifier] NULL
	                            )";
            return tableQuery;
        }
        public static string IAFNotesCommentsFilesTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AuditNotesCommentsFiles](
	                            [CommentFileID] [bigint] IDENTITY(1,1) PRIMARY KEY,
	                            [CommentsID] [bigint] NULL,
	                            [AuditNoteID] [bigint] NULL,
	                            [FileName] [nvarchar](max) NULL,
	                            [StorePath] [nvarchar](max) NULL,
								[CommentUniqueID][uniqueidentifier] NULL,
								[CommentFileUniqueID][uniqueidentifier] NULL
	                            )";
            return tableQuery;
        }

		public static string IAFNotesResolutionTableQuery()
		{
			string tableQuery = @"CREATE TABLE [dbo].[AuditNotesResolution](
								[ResolutionID] [bigint] IDENTITY(1,1) PRIMARY KEY,
								[AuditNoteID] [bigint] NULL,
								[UserName] [nvarchar](250) NULL,
								[Resolution] [nvarchar](max) NULL,
								[CreatedDate] [datetime] NULL,
								[UpdatedDate] [datetime] NULL,
								[ResolutionUniqueID][uniqueidentifier] NULL,
								[NotesUniqueID][uniqueidentifier] NULL)";
			return tableQuery;
		}

		public static string IAFNotesResolutionFilesTableQuery()
		{
			string tableQuery = @"CREATE TABLE [dbo].[AuditNotesResolutionFiles](
								[ResolutionFileID] [bigint] IDENTITY(1,1) PRIMARY KEY,
								[ResolutionID] [bigint] NULL,
								[AuditNoteID] [bigint] NULL,
								[FileName] [nvarchar](max) NULL,
								[StorePath] [nvarchar](max) NULL,
								[ResolutionUniqueID][uniqueidentifier] NULL,
								[ResolutionFileUniqueID][uniqueidentifier] NULL
						)";
			return tableQuery;
		}

		// JSL 05/20/2022
		public static string IAFMLCRegulationTreeTableQuery()
        {
			string tableQuery = @"CREATE TABLE [dbo].[MLCRegulationTree](
								[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
								[MLCRegulationId] [int] NULL,
								[MLCRegulationParentId] [int] NULL,
								[Number] [nvarchar](255) NULL,
								[Regulation] [nvarchar](255) NULL,
								[IsDeleted] [bit] NOT NULL DEFAULT 0,
								[CreatedDateTime] [datetime] NULL,
								[ModifiedDateTime] [datetime] NULL
							)";
			return tableQuery;
		}
		// End JSL 05/20/2022

		// JSL 05/20/2022
		public static string IAFSMSReferencesTreeTableQuery()
		{
			string tableQuery = @"CREATE TABLE [dbo].[SMSReferencesTree](
								[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
								[SMSReferenceId] [int] NULL,
								[SMSReferenceParentId] [int] NULL,
								[Number] [nvarchar](255) NULL,
								[Reference] [nvarchar](255) NULL,
								[IsDeleted] [bit] NOT NULL DEFAULT 0,
								[CreatedDateTime] [datetime] NULL,
								[ModifiedDateTime] [datetime] NULL
							)";
			return tableQuery;
		}
		// End JSL 05/20/2022

		// JSL 05/20/2022
		public static string IAFSSPReferenceTreeTableQuery()
		{
			string tableQuery = @"CREATE TABLE [dbo].[SSPReferenceTree](
								[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
								[SSPReferenceId] [int] NULL,
								[SSPReferenceParentId] [int] NULL,
								[Number] [nvarchar](255) NULL,
								[Reference] [nvarchar](255) NULL,
								[IsDeleted] [bit] NOT NULL DEFAULT 0,
								[CreatedDateTime] [datetime] NULL,
								[ModifiedDateTime] [datetime] NULL
							)";
			return tableQuery;
		}
		// End JSL 05/20/2022
		#endregion


		//RDBJ 09/16/2021
		#region CSShipsTable
		public static string CSShipsTableQuery()
		{
			string tableQuery = @"CREATE TABLE [dbo].[CSShips](
								[ShipId] [int] NOT NULL,
								[Name] [nvarchar](255) NULL,
								[Code] [nvarchar](255) NULL,
								[ShipClassId] [int] NULL,
								[BuildCountryId] [int] NULL,
								[BuildYear] [int] NULL,
								[ClassificationSocietyId] [int] NULL,
								[FlagStateId] [int] NULL,
								[IMONumber] [int] NULL,
								[CallSign] [nvarchar](255) NULL,
								[MMSI] [int] NULL,
								[GrossTonnage] [int] NULL,
								[NetTonnage] [int] NULL,
								[OfficeId] [int] NULL,
								[TechnicalManagerId] [int] NULL,
								[SuperintendentId] [int] NULL,
								[Notes] [nvarchar](max) NULL,
								[IsDelivered] [bit] NOT NULL,
								[FleetId] [int] NULL,
								[YardNo] [int] NULL,
								[OfficialNumber] [int] NULL,
								[PortOfRegistryId] [int] NULL,
								[SummerDeadweight] [decimal](8, 3) NULL,
								[Lightweight] [decimal](7, 3) NULL,
								[LOA] [decimal](6, 3) NULL,
								[LBP] [decimal](6, 3) NULL,
								[Beam] [decimal](5, 3) NULL,
								[SummerDraft] [decimal](7, 5) NULL,
								[BHP] [int] NULL,
								[BowThruster] [int] NULL,
								[BuildNumber] [int] NULL,
								[Agent] [nvarchar](max) NULL,
								[Ports] [nvarchar](max) NULL,
								[TechnicalManagerNotes] [nvarchar](max) NULL,
								[MinimumSafeManning] [int] NULL,
								[MaximumPersonsLSA] [int] NULL,
								[TotalBerths] [int] NULL,
								[RegisteredOwners] [nvarchar](255) NULL,
								[HullAndMachineryId] [int] NULL,
								[ProtectionAndIndemnityId] [int] NULL,
								[Owner] [nvarchar](255) NULL,
								[SMCIsVerified] [bit] NOT NULL,
								[MarineSoftwareNumber] [char](3) NULL,
								[Email] [nvarchar](255) NULL,
								[DataloyNumber] [char](3) NULL,
								[Phone1] [nvarchar](50) NULL,
								[Phone2] [nvarchar](50) NULL,
								[Phone3] [nvarchar](50) NULL,
								[Mobile] [nvarchar](50) NULL,
								[SatC1] [nvarchar](50) NULL,
								[SatC2] [nvarchar](50) NULL,
								[Citadel] [nvarchar](50) NULL,
								[P&I Club] [nvarchar](255) NULL)";
			return tableQuery;
		}

		public static string CSShipsTableTypeQuery()
		{
			string tableTypeQuery = @"IF NOT EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'CSShipsTableType')  
                                    CREATE TYPE CSShipsTableType AS Table 
                                    (	[ShipId] [int] NOT NULL,
										[Name] [nvarchar](255) NULL,
										[Code] [nvarchar](255) NULL,
										[ShipClassId] [int] NULL,
										[BuildCountryId] [int] NULL,
										[BuildYear] [int] NULL,
										[ClassificationSocietyId] [int] NULL,
										[FlagStateId] [int] NULL,
										[IMONumber] [int] NULL,
										[CallSign] [nvarchar](255) NULL,
										[MMSI] [int] NULL,
										[GrossTonnage] [int] NULL,
										[NetTonnage] [int] NULL,
										[OfficeId] [int] NULL,
										[TechnicalManagerId] [int] NULL,
										[SuperintendentId] [int] NULL,
										[Notes] [nvarchar](max) NULL,
										[IsDelivered] [bit] NOT NULL,
										[FleetId] [int] NULL,
										[YardNo] [int] NULL,
										[OfficialNumber] [int] NULL,
										[PortOfRegistryId] [int] NULL,
										[SummerDeadweight] [decimal](8, 3) NULL,
										[Lightweight] [decimal](7, 3) NULL,
										[LOA] [decimal](6, 3) NULL,
										[LBP] [decimal](6, 3) NULL,
										[Beam] [decimal](5, 3) NULL,
										[SummerDraft] [decimal](7, 5) NULL,
										[BHP] [int] NULL,
										[BowThruster] [int] NULL,
										[BuildNumber] [int] NULL,
										[Agent] [nvarchar](max) NULL,
										[Ports] [nvarchar](max) NULL,
										[TechnicalManagerNotes] [nvarchar](max) NULL,
										[MinimumSafeManning] [int] NULL,
										[MaximumPersonsLSA] [int] NULL,
										[TotalBerths] [int] NULL,
										[RegisteredOwners] [nvarchar](255) NULL,
										[HullAndMachineryId] [int] NULL,
										[ProtectionAndIndemnityId] [int] NULL,
										[Owner] [nvarchar](255) NULL,
										[SMCIsVerified] [bit] NOT NULL,
										[MarineSoftwareNumber] [char](3) NULL,
										[Email] [nvarchar](255) NULL,
										[DataloyNumber] [char](3) NULL,
										[Phone1] [nvarchar](50) NULL,
										[Phone2] [nvarchar](50) NULL,
										[Phone3] [nvarchar](50) NULL,
										[Mobile] [nvarchar](50) NULL,
										[SatC1] [nvarchar](50) NULL,
										[SatC2] [nvarchar](50) NULL,
										[Citadel] [nvarchar](50) NULL,
										[P&I Club] [nvarchar](255) NULL
                                    )  ";
			return tableTypeQuery;
		}

		public static string CSShipsInsertProcedureQuery()
		{
			string tableTypeQuery = @"Create Proc usp_InsertUpdateAWSCSShips
                                    	@tblCSShips CSShipsTableType READONLY
                                    AS
                                    BEGIN
									SET NOCOUNT ON;
                                    
									-- RDBJ 03/26/2022 Wrapped in MERGE
									MERGE CSShips  AS dbCSShips  
									USING @tblCSShips AS tblTypeCSShips 
									ON (dbCSShips.Code = tblTypeCSShips.Code)  
									WHEN  MATCHED THEN  
										UPDATE
										SET [ShipId] = tblTypeCSShips.[ShipId] 
										,[Name] = tblTypeCSShips.[Name]
										,[ShipClassId] = tblTypeCSShips.[ShipClassId]
										,[BuildCountryId] = tblTypeCSShips.[BuildCountryId]
										,[BuildYear] = tblTypeCSShips.[BuildYear]
										,[ClassificationSocietyId] = tblTypeCSShips.[ClassificationSocietyId]
										,[FlagStateId] = tblTypeCSShips.[FlagStateId]
										,[IMONumber] = tblTypeCSShips.[IMONumber]
										,[CallSign] = tblTypeCSShips.[CallSign]
										,[MMSI] = tblTypeCSShips.[MMSI]
										,[GrossTonnage] = tblTypeCSShips.[GrossTonnage]
										,[NetTonnage] = tblTypeCSShips.[NetTonnage]
										,[OfficeId] = tblTypeCSShips.[OfficeId]
										,[TechnicalManagerId] = tblTypeCSShips.[TechnicalManagerId]
										,[SuperintendentId] = tblTypeCSShips.[SuperintendentId]
										,[Notes] = tblTypeCSShips.[Notes]
										,[IsDelivered] = tblTypeCSShips.[IsDelivered]
										,[FleetId] = tblTypeCSShips.[FleetId]
										,[YardNo] = tblTypeCSShips.[YardNo]
										,[OfficialNumber] = tblTypeCSShips.[OfficialNumber]
										,[PortOfRegistryId] = tblTypeCSShips.[PortOfRegistryId]
										,[SummerDeadweight] = tblTypeCSShips.[SummerDeadweight]
										,[Lightweight] = tblTypeCSShips.[Lightweight]
										,[LOA] = tblTypeCSShips.[LOA]
										,[LBP] = tblTypeCSShips.[LBP]
										,[Beam] = tblTypeCSShips.[Beam]
										,[SummerDraft] = tblTypeCSShips.[SummerDraft]
										,[BHP] = tblTypeCSShips.[BHP]
										,[BowThruster] = tblTypeCSShips.[BowThruster]
										,[BuildNumber] = tblTypeCSShips.[BuildNumber]
										,[Agent] = tblTypeCSShips.[Agent]
										,[Ports] = tblTypeCSShips.[Ports]
										,[TechnicalManagerNotes] = tblTypeCSShips.[TechnicalManagerNotes]
										,[MinimumSafeManning] = tblTypeCSShips.[MinimumSafeManning]
										,[MaximumPersonsLSA] = tblTypeCSShips.[MaximumPersonsLSA]
										,[TotalBerths] = tblTypeCSShips.[TotalBerths]
										,[RegisteredOwners] = tblTypeCSShips.[RegisteredOwners]
										,[HullAndMachineryId] = tblTypeCSShips.[HullAndMachineryId]
										,[ProtectionAndIndemnityId] = tblTypeCSShips.[ProtectionAndIndemnityId]
										,[Owner] = tblTypeCSShips.[Owner]
										,[SMCIsVerified] = tblTypeCSShips.[SMCIsVerified]
										,[MarineSoftwareNumber] = tblTypeCSShips.[MarineSoftwareNumber]
										,[Email] = tblTypeCSShips.[Email]
										,[DataloyNumber] = tblTypeCSShips.[DataloyNumber]
										,[Phone1] = tblTypeCSShips.[Phone1]
										,[Phone2] = tblTypeCSShips.[Phone2]
										,[Phone3] = tblTypeCSShips.[Phone3]
										,[Mobile] = tblTypeCSShips.[Mobile]
										,[SatC1] = tblTypeCSShips.[SatC1]
										,[SatC2] = tblTypeCSShips.[SatC2]
										,[Citadel] = tblTypeCSShips.[Citadel]
										,[P&I Club] = tblTypeCSShips.[P&I Club]
  
									WHEN NOT MATCHED THEN  
										INSERT
										(
										[ShipId],[Name],[Code],[ShipClassId],[BuildCountryId],[BuildYear],[ClassificationSocietyId],[FlagStateId],[IMONumber],[CallSign],[MMSI],[GrossTonnage]
										,[NetTonnage],[OfficeId],[TechnicalManagerId],[SuperintendentId],[Notes],[IsDelivered],[FleetId],[YardNo],[OfficialNumber],[PortOfRegistryId]
										,[SummerDeadweight],[Lightweight],[LOA],[LBP],[Beam],[SummerDraft],[BHP],[BowThruster],[BuildNumber],[Agent],[Ports],[TechnicalManagerNotes]
										,[MinimumSafeManning],[MaximumPersonsLSA],[TotalBerths],[RegisteredOwners],[HullAndMachineryId],[ProtectionAndIndemnityId],[Owner],[SMCIsVerified]
										)
										VALUES( 
										tblTypeCSShips.[ShipId],tblTypeCSShips.[Name],tblTypeCSShips.[Code],tblTypeCSShips.[ShipClassId],tblTypeCSShips.[BuildCountryId],tblTypeCSShips.[BuildYear],tblTypeCSShips.[ClassificationSocietyId],tblTypeCSShips.[FlagStateId],tblTypeCSShips.[IMONumber]
										,tblTypeCSShips.[CallSign],tblTypeCSShips.[MMSI],tblTypeCSShips.[GrossTonnage],tblTypeCSShips.[NetTonnage],tblTypeCSShips.[OfficeId],tblTypeCSShips.[TechnicalManagerId],tblTypeCSShips.[SuperintendentId],tblTypeCSShips.[Notes]
										,tblTypeCSShips.[IsDelivered],tblTypeCSShips.[FleetId],tblTypeCSShips.[YardNo],tblTypeCSShips.[OfficialNumber],tblTypeCSShips.[PortOfRegistryId],tblTypeCSShips.[SummerDeadweight],tblTypeCSShips.[Lightweight],tblTypeCSShips.[LOA],tblTypeCSShips.[LBP]
										,tblTypeCSShips.[Beam],tblTypeCSShips.[SummerDraft],tblTypeCSShips.[BHP],tblTypeCSShips.[BowThruster],tblTypeCSShips.[BuildNumber],tblTypeCSShips.[Agent],tblTypeCSShips.[Ports],tblTypeCSShips.[TechnicalManagerNotes],tblTypeCSShips.[MinimumSafeManning]
										,tblTypeCSShips.[MaximumPersonsLSA],tblTypeCSShips.[TotalBerths],tblTypeCSShips.[RegisteredOwners],tblTypeCSShips.[HullAndMachineryId],tblTypeCSShips.[ProtectionAndIndemnityId],tblTypeCSShips.[Owner],tblTypeCSShips.[SMCIsVerified]
										);
									-- End RDBJ 03/26/2022
								END";
			return tableTypeQuery;
		}

		// RDBJ 03/26/2022
		public static string CSShipsUPDATEInsertUpdateAWSCSShipsProcedureQuery()
		{
			string tableTypeQuery = @"ALTER Proc [dbo].[usp_InsertUpdateAWSCSShips]
										@tblCSShips CSShipsTableType READONLY
									AS
									BEGIN
										SET NOCOUNT ON;
                                    
										-- RDBJ 03/26/2022 Wrapped in MERGE
										MERGE CSShips  AS dbCSShips  
										USING @tblCSShips AS tblTypeCSShips 
										ON (dbCSShips.Code = tblTypeCSShips.Code)  
										WHEN  MATCHED THEN  
											UPDATE
											SET [ShipId] = tblTypeCSShips.[ShipId] 
											,[Name] = tblTypeCSShips.[Name]
											,[ShipClassId] = tblTypeCSShips.[ShipClassId]
											,[BuildCountryId] = tblTypeCSShips.[BuildCountryId]
											,[BuildYear] = tblTypeCSShips.[BuildYear]
											,[ClassificationSocietyId] = tblTypeCSShips.[ClassificationSocietyId]
											,[FlagStateId] = tblTypeCSShips.[FlagStateId]
											,[IMONumber] = tblTypeCSShips.[IMONumber]
											,[CallSign] = tblTypeCSShips.[CallSign]
											,[MMSI] = tblTypeCSShips.[MMSI]
											,[GrossTonnage] = tblTypeCSShips.[GrossTonnage]
											,[NetTonnage] = tblTypeCSShips.[NetTonnage]
											,[OfficeId] = tblTypeCSShips.[OfficeId]
											,[TechnicalManagerId] = tblTypeCSShips.[TechnicalManagerId]
											,[SuperintendentId] = tblTypeCSShips.[SuperintendentId]
											,[Notes] = tblTypeCSShips.[Notes]
											,[IsDelivered] = tblTypeCSShips.[IsDelivered]
											,[FleetId] = tblTypeCSShips.[FleetId]
											,[YardNo] = tblTypeCSShips.[YardNo]
											,[OfficialNumber] = tblTypeCSShips.[OfficialNumber]
											,[PortOfRegistryId] = tblTypeCSShips.[PortOfRegistryId]
											,[SummerDeadweight] = tblTypeCSShips.[SummerDeadweight]
											,[Lightweight] = tblTypeCSShips.[Lightweight]
											,[LOA] = tblTypeCSShips.[LOA]
											,[LBP] = tblTypeCSShips.[LBP]
											,[Beam] = tblTypeCSShips.[Beam]
											,[SummerDraft] = tblTypeCSShips.[SummerDraft]
											,[BHP] = tblTypeCSShips.[BHP]
											,[BowThruster] = tblTypeCSShips.[BowThruster]
											,[BuildNumber] = tblTypeCSShips.[BuildNumber]
											,[Agent] = tblTypeCSShips.[Agent]
											,[Ports] = tblTypeCSShips.[Ports]
											,[TechnicalManagerNotes] = tblTypeCSShips.[TechnicalManagerNotes]
											,[MinimumSafeManning] = tblTypeCSShips.[MinimumSafeManning]
											,[MaximumPersonsLSA] = tblTypeCSShips.[MaximumPersonsLSA]
											,[TotalBerths] = tblTypeCSShips.[TotalBerths]
											,[RegisteredOwners] = tblTypeCSShips.[RegisteredOwners]
											,[HullAndMachineryId] = tblTypeCSShips.[HullAndMachineryId]
											,[ProtectionAndIndemnityId] = tblTypeCSShips.[ProtectionAndIndemnityId]
											,[Owner] = tblTypeCSShips.[Owner]
											,[SMCIsVerified] = tblTypeCSShips.[SMCIsVerified]
											,[MarineSoftwareNumber] = tblTypeCSShips.[MarineSoftwareNumber]
											,[Email] = tblTypeCSShips.[Email]
											,[DataloyNumber] = tblTypeCSShips.[DataloyNumber]
											,[Phone1] = tblTypeCSShips.[Phone1]
											,[Phone2] = tblTypeCSShips.[Phone2]
											,[Phone3] = tblTypeCSShips.[Phone3]
											,[Mobile] = tblTypeCSShips.[Mobile]
											,[SatC1] = tblTypeCSShips.[SatC1]
											,[SatC2] = tblTypeCSShips.[SatC2]
											,[Citadel] = tblTypeCSShips.[Citadel]
											,[P&I Club] = tblTypeCSShips.[P&I Club]
  
										WHEN NOT MATCHED THEN  
											INSERT
											(
											[ShipId],[Name],[Code],[ShipClassId],[BuildCountryId],[BuildYear],[ClassificationSocietyId],[FlagStateId],[IMONumber],[CallSign],[MMSI],[GrossTonnage]
											,[NetTonnage],[OfficeId],[TechnicalManagerId],[SuperintendentId],[Notes],[IsDelivered],[FleetId],[YardNo],[OfficialNumber],[PortOfRegistryId]
											,[SummerDeadweight],[Lightweight],[LOA],[LBP],[Beam],[SummerDraft],[BHP],[BowThruster],[BuildNumber],[Agent],[Ports],[TechnicalManagerNotes]
											,[MinimumSafeManning],[MaximumPersonsLSA],[TotalBerths],[RegisteredOwners],[HullAndMachineryId],[ProtectionAndIndemnityId],[Owner],[SMCIsVerified]
											)
											VALUES( 
											tblTypeCSShips.[ShipId],tblTypeCSShips.[Name],tblTypeCSShips.[Code],tblTypeCSShips.[ShipClassId],tblTypeCSShips.[BuildCountryId],tblTypeCSShips.[BuildYear],tblTypeCSShips.[ClassificationSocietyId],tblTypeCSShips.[FlagStateId],tblTypeCSShips.[IMONumber]
											,tblTypeCSShips.[CallSign],tblTypeCSShips.[MMSI],tblTypeCSShips.[GrossTonnage],tblTypeCSShips.[NetTonnage],tblTypeCSShips.[OfficeId],tblTypeCSShips.[TechnicalManagerId],tblTypeCSShips.[SuperintendentId],tblTypeCSShips.[Notes]
											,tblTypeCSShips.[IsDelivered],tblTypeCSShips.[FleetId],tblTypeCSShips.[YardNo],tblTypeCSShips.[OfficialNumber],tblTypeCSShips.[PortOfRegistryId],tblTypeCSShips.[SummerDeadweight],tblTypeCSShips.[Lightweight],tblTypeCSShips.[LOA],tblTypeCSShips.[LBP]
											,tblTypeCSShips.[Beam],tblTypeCSShips.[SummerDraft],tblTypeCSShips.[BHP],tblTypeCSShips.[BowThruster],tblTypeCSShips.[BuildNumber],tblTypeCSShips.[Agent],tblTypeCSShips.[Ports],tblTypeCSShips.[TechnicalManagerNotes],tblTypeCSShips.[MinimumSafeManning]
											,tblTypeCSShips.[MaximumPersonsLSA],tblTypeCSShips.[TotalBerths],tblTypeCSShips.[RegisteredOwners],tblTypeCSShips.[HullAndMachineryId],tblTypeCSShips.[ProtectionAndIndemnityId],tblTypeCSShips.[Owner],tblTypeCSShips.[SMCIsVerified]
											);
										-- End RDBJ 03/26/2022
									END";
			return tableTypeQuery;
		}
		// End RDBJ 03/26/2022

		#endregion
		//End RDBJ 09/16/2021

		// Users Table
		public static string UsersTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[Users](
	                            [UID] [int] IDENTITY(1,1) NOT NULL,
	                            [empnre01] [nvarchar](50) NULL,
	                            [empste01] [nvarchar](50) NULL,
	                            [fstnme01] [nvarchar](50) NULL,
	                            [surnme01] [nvarchar](50) NULL,
	                            [bthcnte01] [nvarchar](50) NULL,
	                            [fnce01] [nvarchar](50) NULL,
	                            [lane01] [nvarchar](50) NULL,
	                            [rsnewe01] [nvarchar](50) NULL,
	                            [sxee01] [nvarchar](50) NULL,
	                            [bthdate01] [datetime] NULL,
	                            [nate01] [nvarchar](50) NULL)";
            return tableQuery;
        }

        public static string UsersTableTypeQuery()
        {
            string tableTypeQuery = @"IF NOT EXISTS(SELECT * FROM sys.types WHERE is_table_type = 1 AND name = N'UsersTableType')  
                                    CREATE TYPE UsersTableType AS Table 
                                    (	[UID] [int],
                                    	[empnre01] [nvarchar](50) NULL,
                                    	[empste01] [nvarchar](50) NULL,
                                    	[fstnme01] [nvarchar](50) NULL,
                                    	[surnme01] [nvarchar](50) NULL,
                                    	[bthcnte01] [nvarchar](50) NULL,
                                    	[fnce01] [nvarchar](50) NULL,
                                    	[lane01] [nvarchar](50) NULL,
                                    	[rsnewe01] [nvarchar](50) NULL,
                                    	[sxee01] [nvarchar](50) NULL,
                                    	[bthdate01] [datetime] NULL,
                                    	[nate01] [nvarchar](50) NULL
                                    )  ";
            return tableTypeQuery;
        }
        public static string UsersInsertProcedureQuery()
        {
            string tableTypeQuery = @"Create Proc usp_InsertUpdateAWSUsers
                                    	@tblUsers UsersTableType READONLY
                                    AS
                                    BEGIN
                                    	SET NOCOUNT ON;
                                    
                                    	UPDATE u1
                                    	SET u1.empste01= u2.empste01
                                    	,u1.[fstnme01] = u2.[fstnme01]
                                    	,u1.[surnme01] = u2.[surnme01]
                                    	,u1.[bthcnte01]= u2.[bthcnte01]
                                    	,u1.[fnce01]   = u2.[fnce01]
                                    	,u1.[lane01]   = u2.[lane01]
                                    	,u1.[rsnewe01] = u2.[rsnewe01]
                                    	,u1.[sxee01]   = u2.[sxee01]
                                    	,u1.[bthdate01]= u2.[bthdate01]
                                    	,u1.[nate01]   = u2.[nate01]
                                    	FROM Users u1
                                    	INNER JOIN @tblUsers u2
                                    	ON u1.empnre01=u2.empnre01
                                    
                                    	INSERT INTO Users
                                    	(empnre01,empste01,[fstnme01],[surnme01],[bthcnte01],[fnce01],[lane01],[rsnewe01],[sxee01],[bthdate01],[nate01])
                                    	SELECT u2.empnre01,u2.empste01,u2.[fstnme01],u2.[surnme01],u2.[bthcnte01],u2.[fnce01],u2.[lane01],u2.[rsnewe01]
                                    	,u2.[sxee01],u2.[bthdate01],u2.[nate01]
                                    	FROM @tblUsers u2
                                    	WHERE empnre01 NOT IN (SELECT empnre01 FROM Users)
                                    END";
            return tableTypeQuery;
        }

        public static string CheckTableExistQuery(string tableName)
        {
            string tableTypeQuery = @"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = N'" + tableName + "' AND type = 'U') ";
            return tableTypeQuery;
        }

        public static string HoldVentilationRecordFormTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[HoldVentilationRecordForm](
	                            [HoldVentilationRecordFormId] [bigint] IDENTITY(1,1) NOT NULL,
	                            [ShipName] [varchar](255) NULL,
	                            [ShipCode] [varchar](255) NULL,
	                            [Cargo] [varchar](300) NULL,
	                            [Quantity] [float] NULL,
	                            [LoadingPort] [varchar](300) NULL,
	                            [LoadingDate] [datetime] NULL,
	                            [DischargingPort] [varchar](300) NULL,
	                            [DischargingDate] [datetime] NULL,
	                            [CreatedDate] [datetime] NULL,
	                            [UpdatedDate] [datetime] NULL,
	                            [IsSynced] [bit] NULL)";
            return tableQuery;
        }
        public static string HoldVentilationRecordSheetFormTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[HoldVentilationRecordSheet](
	                                [HoldVentilationRecordId] [bigint] IDENTITY(1,1) NOT NULL,
	                                [HoldVentilationRecordFormId] [bigint] NULL,
	                                [HVRDate] [date] NULL,
	                                [HVRTime] [time](7) NULL,
	                                [OUTDryBulb] [varchar](max) NULL,
	                                [OUTWetBulb] [varchar](max) NULL,
	                                [OUTDewPOint] [varchar](max) NULL,
	                                [HODryBulb] [varchar](max) NULL,
	                                [HOWetBulb] [varchar](max) NULL,
	                                [HODewPOint] [varchar](max) NULL,
	                                [IsVentilation] [bit] NULL,
	                                [SeaTemp] [float] NULL,
	                                [Remarks] [varchar](max) NULL,
	                                [CreatedDate] [datetime] NULL,
	                                [UpdatedDate] [datetime] NULL)";
            return tableQuery;
        }

        public static string ShipAppReleaseNoteTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[ShipAppReleaseNote](
                                	[AppId] [bigint] IDENTITY(1,1) NOT NULL,
                                	[AppVersion] [varchar](50) NULL,
                                	[AppDescription] [varchar](max) NULL,
                                	[AppPublishDate] [datetime] NULL,
                                	[NoOfFilesAffected] [int] NULL,
                                	[CreatedDate] [datetime] NULL,
                                	[UpdateDate] [datetime] NULL)";
            return tableQuery;
        }

        public static string FeedbackFormTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[FeedbackForm](
	                                [Id] [uniqueidentifier] NOT NULL,
	                                [ShipName] [varchar](255) NULL,
	                                [ShipCode] [varchar](255) NULL,
	                                [Title] [varchar](300) NULL,
	                                [Details] [nvarchar](max) NULL,
	                                [CreatedDate] [datetime] NULL,	
	                                [IsMailSent] [bit] NULL,	
	                                [IsSynced] [bit] NULL,
                                    [AttachmentPath] [nvarchar](max) NULL,
	                                [AttachmentFileName] [nvarchar](max) NULL,
                                    [FeedbackBy] [nvarchar](150) NULL)";
            return tableQuery;
        }

        public static string RiskAssessmentTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[RiskAssessmentForm](
	[RAFID] [bigint] IDENTITY(1,1) NOT NULL,
	[ShipName] [nvarchar](500) NULL,
	[ShipCode] [nvarchar](100) NULL,
	[Number] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[ReviewerName] [nvarchar](max) NULL,
	[ReviewerDate] [datetime] NULL,
	[ReviewerRank] [nvarchar](max) NULL,
	[ReviewerLocation] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[SavedAsDraft] [bit] NULL,
	[IsSynced] [bit] NULL,
    [Code] [nvarchar](100) NULL,
	[Issue] [int] NULL,
	[IssueDate] [datetime] NULL,
	[Amendment] [int] NULL,
	[AmendmentDate] [datetime] NULL,
	[IsConfidential] [bit] NULL,
    [DocumentID] [uniqueidentifier] NULL,
	[ParentID] [uniqueidentifier] NULL,
	[Type] [nvarchar](50) NULL,
	[SectionType] [nvarchar](100) NULL,
    [IsAmended] [bit] NULL,
    [IsApplicable] [bit] NULL)";

            return tableQuery;
        }

        public static string RiskAssessmentFormHazardTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[RiskAssessmentFormHazard](
	[Id] [uniqueidentifier] NOT NULL,
	[HazardId] [bigint] NULL,
	[RAFID] [bigint] NULL,
	[Stage1Description] [nvarchar](max) NULL,
	[Stage2Severity] [nvarchar](250) NULL,
	[Stage2Likelihood] [nvarchar](250) NULL,
	[Stage2RiskFactor] [nvarchar](250) NULL,
	[Stage3Description] [nvarchar](max) NULL,
	[Stage4Severity] [nvarchar](250) NULL,
	[Stage4Likelihood] [nvarchar](250) NULL,
	[Stage4RiskFactor] [nvarchar](250) NULL)";
            return tableQuery;
        }

        public static string RiskAssessmentFormReviewerTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[RiskAssessmentFormReviewer](
	[Id] [uniqueidentifier] NOT NULL,
	[RAFID] [bigint] NULL,
	[ReviewerName] [nvarchar](max) NULL,
	[IndexNo] [bigint] NULL)";
            return tableQuery;
        }
        public static string AssetManagmentEquipmentListTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AssetManagmentEquipmentList](
	[AMEId] [uniqueidentifier] NOT NULL,
	[ShipName] [nvarchar](500) NULL,
	[ShipCode] [nvarchar](100) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[SavedAsDraft] [bit] NULL,
	[IsSynced] [bit] NULL)";
            return tableQuery;
        }
        public static string AssetManagmentEquipmentOTListTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AssetManagmentEquipmentOTList](
	[OTId] [uniqueidentifier] NOT NULL,
	[AMEId] [uniqueidentifier] NOT NULL,
	[OTEquipment] [nvarchar](250) NULL,
	[OTLocation] [nvarchar](250) NULL,
	[OTMake] [nvarchar](250) NULL,
	[OTModel] [nvarchar](250) NULL,
	[OTType] [nvarchar](250) NULL,
	[OTSerialNo] [nvarchar](250) NULL,
	[OTWorkingCondition] [nvarchar](250) NULL,
	[OTLastServiced] [nvarchar](250) NULL,
	[OTRemark] [nvarchar](250) NULL,
	[OTHardwareId] nvarchar(500) NULL,
	[OTOwner] nvarchar(500) NULL,
	[OTPersonResponsible] nvarchar(500) NULL,
	[OTCriticality] nvarchar(500) NULL,
	[OTOperatingSystem] nvarchar(500) NULL,
	[OTOSPatchVersion] nvarchar(250) NULL)";
            return tableQuery;
        }
        public static string AssetManagmentEquipmentITListTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AssetManagmentEquipmentITList](
	[ITId] [uniqueidentifier] NOT NULL,
	[AMEId] [uniqueidentifier] NOT NULL,
	[ITEquipment] [nvarchar](250) NULL,
	[ITLocation] [nvarchar](250) NULL,
	[ITMake] [nvarchar](250) NULL,
	[ITModel] [nvarchar](250) NULL,
	[ITType] [nvarchar](250) NULL,
	[ITSerialNo] [nvarchar](250) NULL,
	[ITWorkingCondition] [nvarchar](250) NULL,
	[ITLastServiced] [nvarchar](250) NULL,
	[ITRemark] [nvarchar](250) NULL,
	[ITHardwareId] nvarchar(500) NULL,
	[ITOwner] nvarchar(500) NULL,
	[ITPersonResponsible] nvarchar(500) NULL,
	[ITCriticality] nvarchar(500) NULL,
	[ITOperatingSystem] nvarchar(500) NULL,
	[ITOSPatchVersion] nvarchar(250) NULL)";
            return tableQuery;
        }
        public static string AssetManagmentEquipmentSoftwareAssetsTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[AssetManagmentEquipmentSoftwareAssets](
	[SAId] [uniqueidentifier] NOT NULL,
	[AMEId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Manufacturer] [nvarchar](250) NULL,
	[LicenseType] [nvarchar](250) NULL,
	[Category] [nvarchar](250) NULL,
	[IsActive] [nvarchar](250) NULL,
	[SASoftwareId] nvarchar(500) NULL,
	[SAOwner] nvarchar(500) NULL,
	[SAPersonResponsible] nvarchar(500) NULL,
	[SACriticality] nvarchar(500) NULL,
	[SAOperatingSystem] nvarchar(500) NULL,
	[SAOSPatchVersion] nvarchar(250) NULL)";
            return tableQuery;
        }

        public static string CybersecurityRisksAssessmentTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[CybersecurityRisksAssessment](
	[CRAId] [uniqueidentifier] NOT NULL,
	[ShipName] [nvarchar](500) NULL,
	[ShipCode] [nvarchar](100) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,	
	[IsSynced] [bit] NULL)";
            return tableQuery;
        }

        public static string CybersecurityRisksAssessmentListTableQuery()
        {
            string tableQuery = @"CREATE TABLE [dbo].[CybersecurityRisksAssessmentList](
	[CRALId] [uniqueidentifier] NOT NULL,
	[CRAId] [uniqueidentifier] NOT NULL,
	[RiskId] [nvarchar](500) NULL,
	[RiskDescription] [nvarchar](1000) NULL,
	[InherentRiskCategoryC] [nvarchar](250) NULL,
	[InherentRiskCategoryI] [nvarchar](250) NULL,
	[InherentRiskCategoryA] [nvarchar](250) NULL,
	[InherentRiskCategoryS] [nvarchar](250) NULL,
	[InherentImpactScore] [nvarchar](500) NULL,
	[InherentLikelihoodScore] [nvarchar](500) NULL,
	[InherentRiskScore] [nvarchar](500) NULL,
	[RiskOwner] [nvarchar](500) NULL,
	[Controls] [nvarchar](500) NULL,
	[ResidualRiskCategoryC] [nvarchar](250) NULL,
	[ResidualRiskCategoryI] [nvarchar](250) NULL,
	[ResidualRiskCategoryA] [nvarchar](250) NULL,
	[ResidualRiskCategoryS] [nvarchar](250) NULL,
	[ResidualImpactScore] [nvarchar](500) NULL,
	[ResidualLikelihoodScore] [nvarchar](500) NULL,
	[ResidualRiskScore] [nvarchar](500) NULL,
	[RiskDecision] [nvarchar](500) NULL,
	[Vulnerability] [nvarchar](1000) NULL,
	[HardwareId] [nvarchar](1000) NULL)";
            return tableQuery;
        }
    }
}
