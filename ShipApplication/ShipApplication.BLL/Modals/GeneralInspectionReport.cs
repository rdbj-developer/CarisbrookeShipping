using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    public class GeneralInspectionReport
    {
        public GeneralInspectionReport()
        {
            CSShipsModal = new CSShipsModal();
        }
        public string appMode { get; set; }
        public long GIRFormID { get; set; }
        public long OfficeGIRFormID { get; set; }
        public int? ShipID { get; set; }
        public string ShipName { get; set; }
        public System.Guid UniqueFormID { get; set; }
        public decimal FormVersion { get; set; }
        public Nullable<int> isDelete { get; set; } // RDBJ 01/05/2022

        #region General Details
        public string Ship { get; set; }
        public string Port { get; set; }
        public string Inspector { get; set; }
        public DateTime? Date { get; set; }
        #endregion

        #region General Preamble
        public string GeneralPreamble { get; set; }
        #endregion

        #region General Description
        public CSShipsModal CSShipsModal { get; set; } //RDBJ 10/06/2021
        public string Classsociety { get; set; }
        public string YearofBuild { get; set; }
        public string Flag { get; set; }
        public string Classofvessel { get; set; }
        public string Portofregistry { get; set; }
        public string MMSI { get; set; }
        public string IMOnumber { get; set; }
        public string Callsign { get; set; }
        public string SummerDWT { get; set; }
        public string Grosstonnage { get; set; }
        public string Lightweight { get; set; }
        public string Nettonnage { get; set; }
        public string Beam { get; set; }
        public string LOA { get; set; }
        public string Summerdraft { get; set; }
        public string LBP { get; set; }
        public string Bowthruster { get; set; }
        public string BHP { get; set; }
        public string Noofholds { get; set; }
        public string Nomoveablebulkheads { get; set; }
        public string Containers { get; set; }
        public string Cargocapacity { get; set; }
        public string Cargohandlingequipment { get; set; }
        public string Lastvoyageandcargo { get; set; }
        public string CurrentPlannedvoyageandcargo { get; set; }
        #endregion

        #region Safe Manning Requirements
        public List<GlRSafeManningRequirements> GIRSafeManningRequirements { get; set; }
        public List<GIRCrewDocuments> GIRCrewDocuments { get; set; }
        public List<GIRRestandWorkHours> GIRRestandWorkHours { get; set; }
        #endregion

        #region Shipboard Working Arrangements
        public string ShipboardWorkingArrangements { get; set; }
        #endregion

        #region 5A-Certification
        public string CertificationIndex { get; set; }
        #endregion

        #region List of Publications Held On-board
        public bool? IsPubsAndDocsSectionComplete { get; set; }
        #endregion

        #region  Section 5B
        public string CarriedOutByTheDOOW { get; set; }
        #endregion

        #region Section 5C -List of Documents
        public bool? IsRegs4shipsDVD { get; set; }
        public string Regs4shipsDVD { get; set; }
        public bool? IsSOPEPPoints { get; set; }
        public string SOPEPPoints { get; set; }
        public bool? IsBWMP { get; set; }
        public string BWMP { get; set; }
        public bool? IsBWMPSupplement { get; set; }
        public string BWMPSupplement { get; set; }
        public bool? IsIntactStabilityManual { get; set; }
        public string IntactStabilityManual { get; set; }
        public bool? IsStabilityComputer { get; set; }
        public string StabilityComputer { get; set; }
        public bool? IsDateOfLast { get; set; }
        public string DateOfLast { get; set; }
        public bool? IsCargoSecuring { get; set; }
        public string CargoSecuring { get; set; }
        public bool? IsBulkCargo { get; set; }
        public string BulkCargo { get; set; }
        public bool? IsSMSManual { get; set; }
        public string SMSManual { get; set; }
        public bool? IsRegisterOf { get; set; }
        public string RegisterOf { get; set; }
        public bool? IsFleetStandingOrder { get; set; }
        public string FleetStandingOrder { get; set; }
        public bool? IsFleetMemoranda { get; set; }
        public string FleetMemoranda { get; set; }
        public bool? IsShipsPlans { get; set; }
        public string ShipsPlans { get; set; }
        public bool? IsCollective { get; set; }
        public string Collective { get; set; }
        public bool? IsDraftAndFreeboardNotice { get; set; }
        public string DraftAndFreeboardNotice { get; set; }
        public bool? IsPCSOPEP { get; set; }
        public string PCSOPEP { get; set; }
        public bool? IsNTVRP { get; set; }
        public string NTVRP { get; set; }
        public bool? IsVGP { get; set; }
        public string VGP { get; set; }
        public string PubsComments { get; set; }
        #endregion

        #region Section 5D-Record keeping
        public string OfficialLogbookA { get; set; }
        public string OfficialLogbookB { get; set; }
        public string OfficialLogbookC { get; set; }
        public string OfficialLogbookD { get; set; }
        public string OfficialLogbookE { get; set; }
        public string DeckLogbook { get; set; }
        public string Listofcrew { get; set; }
        public string LastHose { get; set; }
        public string PassagePlanning { get; set; }
        public string LoadingComputer { get; set; }
        public string EngineLogbook { get; set; }
        public string OilRecordBook { get; set; }
        public string RiskAssessments { get; set; }
        public string GMDSSLogbook { get; set; }
        public string DeckLogbook5D { get; set; }
        public string GarbageRecordBook { get; set; }
        public string BallastWaterRecordBook { get; set; }
        public string CargoRecordBook { get; set; }
        public string EmissionsControlManual { get; set; }
        public string LGR { get; set; }
        public string PEER { get; set; }
        public string RecordKeepingComments { get; set; }
        #endregion

        #region Section 5E
        public string LastPortStateControl { get; set; }
        #endregion

        #region Section 5F - Safety Equipment
        public string LiferaftsComment { get; set; }
        public string releasesComment { get; set; }
        public string LifeboatComment { get; set; }
        public string LifeboatdavitComment { get; set; }
        public string LifeboatequipmentComment { get; set; }
        public string RescueboatComment { get; set; }
        public string RescueboatequipmentComment { get; set; }
        public string RescueboatoutboardmotorComment { get; set; }
        public string RescueboatdavitComment { get; set; }
        public string DeckComment { get; set; }
        public string PyrotechnicsComment { get; set; }
        public string EPIRBComment { get; set; }
        public string SARTsComment { get; set; }
        public string GMDSSComment { get; set; }
        public string ManoverboardComment { get; set; }
        public string LinethrowingapparatusComment { get; set; }
        public string FireextinguishersComment { get; set; }
        public string EmergencygeneratorComment { get; set; }
        public string CO2roomComment { get; set; }
        public string SurvivalComment { get; set; }
        public string LifejacketComment { get; set; }
        public string FiremansComment { get; set; }
        public string LifebuoysComment { get; set; }
        public string FireboxesComment { get; set; }
        public string EmergencybellsComment { get; set; }
        public string EmergencylightingComment { get; set; }
        public string FireplanComment { get; set; }
        public string DamageComment { get; set; }
        public string EmergencyplansComment { get; set; }
        public string MusterlistComment { get; set; }
        public string SafetysignsComment { get; set; }
        public string EmergencysteeringComment { get; set; }
        public string StatutoryemergencydrillsComment { get; set; }
        public string EEBDComment { get; set; }
        public string OxygenComment { get; set; }
        public string MultigasdetectorComment { get; set; }
        public string GasdetectorComment { get; set; }
        public string SufficientquantityComment { get; set; }
        public string BASetsComment { get; set; }
        public string SafetyComment { get; set; }
        public string LiferaftDavitComment { get; set; }

        #endregion

        #region Section 5G - Security Equipment and Records
        public string GangwayComment { get; set; }
        public string RestrictedComment { get; set; }
        public string OutsideComment { get; set; }
        public string EntrancedoorsComment { get; set; }
        public string AccommodationComment { get; set; }
        public string GMDSSComment5G { get; set; }
        public string VariousComment { get; set; }
        public string SSOComment { get; set; }
        public string SecuritylogbookComment { get; set; }
        public string Listoflast10portsComment { get; set; }
        public string PFSOComment { get; set; }
        public string SecuritylevelComment { get; set; }
        public string DrillsandtrainingComment { get; set; }
        public string DOSComment { get; set; }
        public string SSASComment { get; set; }
        public string VisitorslogbookComment { get; set; }
        public string KeyregisterComment { get; set; }
        public string ShipSecurityComment { get; set; }
        public string SecurityComment { get; set; }
        #endregion

        #region Section 5H - Security Equipment and Records
        public string NauticalchartsComment { get; set; }
        public string NoticetomarinersComment { get; set; }
        public string ListofradiosignalsComment { get; set; }
        public string ListoflightsComment { get; set; }
        public string SailingdirectionsComment { get; set; }
        public string TidetablesComment { get; set; }
        public string NavtexandprinterComment { get; set; }
        public string RadarsComment { get; set; }
        public string GPSComment { get; set; }
        public string AISComment { get; set; }
        public string VDRComment { get; set; }
        public string ECDISComment { get; set; }
        public string EchosounderComment { get; set; }
        public string ADPbackuplaptopComment { get; set; }
        public string ColourprinterComment { get; set; }
        public string VHFDSCtransceiverComment { get; set; }
        public string radioinstallationComment { get; set; }
        public string InmarsatCComment { get; set; }
        public string MagneticcompassComment { get; set; }
        public string SparecompassbowlComment { get; set; }
        public string CompassobservationbookComment { get; set; }
        public string GyrocompassComment { get; set; }
        public string RudderindicatorComment { get; set; }
        public string SpeedlogComment { get; set; }
        public string NavigationComment { get; set; }
        public string SignalflagsComment { get; set; }
        public string RPMComment { get; set; }
        public string BasicmanoeuvringdataComment { get; set; }
        public string MasterstandingordersComment { get; set; }
        public string MasternightordersbookComment { get; set; }
        public string SextantComment { get; set; }
        public string AzimuthmirrorComment { get; set; }
        public string BridgepostersComment { get; set; }
        public string ReviewofplannedComment { get; set; }
        public string BridgebellbookComment { get; set; }
        public string BridgenavigationalComment { get; set; }
        public string SecurityEquipmentComment { get; set; }
        public string ADPPublicationsComment { get; set; }
        #endregion

        #region Section 5I - Navigation
        public string NavigationPost { get; set; }
        #endregion

        #region Section 5J - Hospital and Medicine Locker
        public string GeneralComment { get; set; }
        public string MedicinestorageComment { get; set; }
        public string MedicinechestcertificateComment { get; set; }
        public string InventoryStoresComment { get; set; }
        public string OxygencylindersComment { get; set; }
        public string StretcherComment { get; set; }
        public string SalivaComment { get; set; }
        public string AlcoholComment { get; set; }
        public string HospitalComment { get; set; }
        public string MedicalLogBookComment { get; set; }
        public string DrugsNarcoticsComment { get; set; }
        public string DefibrillatorComment { get; set; }
        public string RPWaterHandbook { get; set; }
        public string BioRPWH { get; set; }
        public string PRE { get; set; }
        public string NoiseVibrationFile { get; set; }
        public string BioMPR { get; set; }
        public string AsbestosPlan { get; set; }
        public string ShipPublicAddrComment { get; set; }
        public string BridgewindowswiperssprayComment { get; set; }
        public string BridgewindowswipersComment { get; set; }
        public string DaylightSignalsComment { get; set; }

        #endregion

        #region Section 5K - Galley
        public string GeneralGalleyComment { get; set; }
        public string HygieneComment { get; set; }
        public string FoodstorageComment { get; set; }
        public string FoodlabellingComment { get; set; }
        public string GalleyriskassessmentComment { get; set; }
        public string FridgetemperatureComment { get; set; }
        public string FoodandProvisionsComment { get; set; }
        public string GalleyComment { get; set; }
        #endregion

        #region Section 5L - Engine Room
        public string ConditionComment { get; set; }
        public string PaintworkComment { get; set; }
        public string LightingComment { get; set; }
        public string PlatesComment { get; set; }
        public string BilgesComment { get; set; }
        public string PipelinesandvalvesComment { get; set; }
        public string LeakageComment { get; set; }
        public string EquipmentComment { get; set; }
        public string OilywaterseparatorComment { get; set; }
        public string FueloiltransferplanComment { get; set; }
        public string SteeringgearComment { get; set; }
        public string WorkshopandequipmentComment { get; set; }
        public string SoundingpipesComment { get; set; }
        public string EnginecontrolComment { get; set; }
        public string ChiefEngineernightordersbookComment { get; set; }
        public string ChiefEngineerstandingordersComment { get; set; }
        public string PreUMSComment { get; set; }
        public string EnginebellbookComment { get; set; }
        public string LockoutComment { get; set; }
        public string EngineRoomComment { get; set; }
        #endregion

        #region Section 5M - Superstructure and Poop Deck - External Standards
        public string CleanlinessandhygieneComment { get; set; }
        public string ConditionComment5M { get; set; }
        public string PaintworkComment5M { get; set; }
        public string SignalmastandstaysComment { get; set; }
        public string MonkeyislandComment { get; set; }
        public string FireDampersComment { get; set; }
        public string RailsBulwarksComment { get; set; }
        public string WatertightdoorsComment { get; set; }
        public string VentilatorsComment { get; set; }
        public string WinchesComment { get; set; }
        public string FairleadsComment { get; set; }
        public string MooringLinesComment { get; set; }
        public string EmergencyShutOffsComment { get; set; }
        public string RadioaerialsComment { get; set; }
        public string SOPEPlockerComment { get; set; }
        public string ChemicallockerComment { get; set; }
        public string AntislippaintComment { get; set; }
        public string SuperstructureComment { get; set; }
        public string CylindersLockerComment { get; set; }
        public string SnapBackZone5NComment { get; set; }
        #endregion

        #region Section 5N - Superstructure - Internal Standards
        public string CabinsComment { get; set; }
        public string OfficesComment { get; set; }
        public string MessroomsComment { get; set; }
        public string ToiletsComment { get; set; }
        public string LaundryroomComment { get; set; }
        public string ChangingroomComment { get; set; }
        public string OtherComment { get; set; }
        public string ConditionComment5N { get; set; }
        public string SelfclosingfiredoorsComment { get; set; }
        public string StairwellsComment { get; set; }
        public string SuperstructureInternalComment { get; set; }
        #endregion

        #region Section 5O - Boarding Equipment
        public string PortablegangwayComment { get; set; }
        public string SafetynetComment { get; set; }
        public string AccommodationLadderComment { get; set; }
        public string SafeaccessprovidedComment { get; set; }
        public string PilotladdersComment { get; set; }
        public string BoardingEquipmentComment { get; set; }
        #endregion

        #region Section 5P - Main Deck Area
        public string CleanlinessComment { get; set; }
        public string PaintworkComment5P { get; set; }
        public string ShipsiderailsComment { get; set; }
        public string WeathertightdoorsComment { get; set; }
        public string FirehydrantsComment { get; set; }
        public string VentilatorsComment5P { get; set; }
        public string ManholecoversComment { get; set; }
        public string MainDeckAreaComment { get; set; }
        #endregion

        #region Section 5Q - Cargo Holds and Hatch Covers
        public string ConditionComment5Q { get; set; }
        public string PaintworkComment5Q { get; set; }
        public string MechanicaldamageComment { get; set; }
        public string AccessladdersComment { get; set; }
        public string ManholecoversComment5Q { get; set; }
        public string HoldbilgeComment { get; set; }
        public string AccessdoorsComment { get; set; }
        public string ConditionHatchCoversComment { get; set; }
        public string PaintworkHatchCoversComment { get; set; }
        public string RubbersealsComment { get; set; }
        public string SignsofhatchesComment { get; set; }
        public string SealingtapeComment { get; set; }
        public string ConditionofhydraulicsComment { get; set; }
        public string PortablebulkheadsComment { get; set; }
        public string TweendecksComment { get; set; }
        public string HatchcoamingComment { get; set; }
        public string ConditionCargoCranesComment { get; set; }
        public string GantrycranealarmComment { get; set; }
        public string GantryconditionComment { get; set; }
        public string CargoHoldsComment { get; set; }
        public string ConditionGantryCranesComment { get; set; }
        #endregion

        #region Section 5R - Forecastle Head
        public string CleanlinessComment5R { get; set; }
        public string PaintworkComment5R { get; set; }
        public string TriphazardsComment { get; set; }
        public string WindlassComment { get; set; }
        public string CablesComment { get; set; }
        public string WinchesComment5R { get; set; }
        public string FairleadsComment5R { get; set; }
        public string MooringComment { get; set; }
        public string HatchToforecastlespaceComment { get; set; }
        public string VentilatorsComment5R { get; set; }
        public string BellComment { get; set; }
        public string ForemastComment { get; set; }
        public string FireComment { get; set; }
        public string RailsComment { get; set; }
        public string AntislippaintComment5R { get; set; }
        public string ForecastleComment { get; set; }
        public string SnapBackZoneComment { get; set; }

        #endregion

        #region Section 5S - Forecastle Space
        public string CleanlinessComment5S { get; set; }
        public string PaintworkComment5S { get; set; }
        public string ForepeakComment { get; set; }
        public string ChainlockerComment { get; set; }
        public string LightingComment5S { get; set; }
        public string AccesssafetychainComment { get; set; }
        public string EmergencyfirepumpComment { get; set; }
        public string BowthrusterandroomComment { get; set; }
        public string SparemooringlinesComment { get; set; }
        public string PaintlockerComment { get; set; }
        public string ForecastleSpaceComment { get; set; }
        #endregion

        #region Section 5T - Hull, Including Paintwork

        public string BoottopComment { get; set; }
        public string TopsidesComment { get; set; }
        public string AntifoulingComment { get; set; }
        public string DraftandplimsollComment { get; set; }
        public string FoulingComment { get; set; }
        public string MechanicalComment { get; set; }
        public string HullComment { get; set; }
        #endregion

        #region Section 6 - Summary and Recommendations
        public string SummaryComment { get; set; }
        #endregion

        #region Section 7 - Deficiencies
        public List<GIRDeficiencies> GIRDeficiencies { get; set; }
        #endregion

        #region Photographs
        public List<GIRPhotographs> GIRPhotographs { get; set; }
        #endregion
        public bool? IsSynced { get; set; }
        public bool? SavedAsDraft { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Manning_SafeMiningChanged { get; set; }
        public bool Manning_CrewDocsChanged { get; set; }
        public bool Manning_RestAndWorkChanged { get; set; }
        public bool Manning_DeficienciesChanged { get; set; }
        public bool Manning_PhotosChanged { get; set; }

        //RDBJ 10/19/2021
        public bool? IsGeneralSectionComplete { get; set; }
        public bool? IsManningSectionComplete { get; set; }
        public bool? IsShipCertificationSectionComplete { get; set; }
        public bool? IsRecordKeepingSectionComplete { get; set; }
        public bool? IsSafetyEquipmentSectionComplete { get; set; }
        public bool? IsSecuritySectionComplete { get; set; }
        public bool? IsBridgeSectionComplete { get; set; }
        public bool? IsMedicalSectionComplete { get; set; }
        public bool? IsGalleySectionComplete { get; set; }
        public bool? IsEngineRoomSectionComplete { get; set; }
        public bool? IsSuperstructureSectionComplete { get; set; }
        public bool? IsDeckSectionComplete { get; set; }
        public bool? IsHoldsAndCoverSectionComplete { get; set; }
        public bool? IsForeCastleSectionComplete { get; set; }
        public bool? IsHullSectionComplete { get; set; }
        public bool? IsSummarySectionComplete { get; set; }
        public bool? IsDeficienciesSectionComplete { get; set; }
        public bool? IsPhotographsSectionComplete { get; set; }
        //End RDBJ 10/19/2021
    }
    public class GlRSafeManningRequirements
    {
        public System.Guid UniqueFormID { get; set; }
        public long SafeManningRequirementsID { get; set; }
        public long? GIRFormID { get; set; }
        public string Rank { get; set; }
        public bool? RequiredbySMD { get; set; }
        public bool? OnBoard { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Ship { get; set; }
    }
    public class GIRCrewDocuments
    {
        public System.Guid UniqueFormID { get; set; }
        public long CrewDocumentsID { get; set; }
        public long GIRFormID { get; set; }
        public string CrewmemberName { get; set; }
        public string Ship { get; set; }
        public string CertificationDetail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    public class GIRRestandWorkHours
    {
        public System.Guid UniqueFormID { get; set; }
        public long RestandWorkHoursID { get; set; }
        public long? GIRFormID { get; set; }
        public string CrewmemberName { get; set; }
        public string RestAndWorkDetail { get; set; }
        public string Ship { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    public class GIRDeficiencies
    {
        public System.Guid UniqueFormID { get; set; }
        public int isDelete { get; set; }
        public GIRDeficiencies()
        {
            GIRDeficienciesFile = new List<GIRDeficienciesFile>();
            GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>();  // RDBJ 02/20/2022
        }
        public int DeficienciesID { get; set; }
        public Nullable<long> GIRFormID { get; set; }
        public Nullable<long> OfficeGIRFormID { get; set; }
        public int No { get; set; }
        public Nullable<System.DateTime> DateRaised { get; set; }
        public string Deficiency { get; set; }
        public Nullable<System.DateTime> DateClosed { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }    // JSL 06/23/2022
        public Nullable<bool> IsClose { get; set; }
        public string ReportType { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string SIRNo { get; set; }
        public List<GIRDeficienciesFile> GIRDeficienciesFile { get; set; }
        public string ItemNo { get; set; }
        public string Section { get; set; }
        public bool IsSynced { get; set; }
        public string Inspector { get; set; }
        public string Port { get; set; }
        public System.Guid DeficienciesUniqueID { get; set; }
        public int Priority { get; set; } //RDBJ 11/01/2021
        public Nullable<System.DateTime> DueDate { get; set; }   // RDBJ 02/28/2022
        public List<GIRDeficienciesInitialActions> GIRDeficienciesInitialActions { get; set; } // RDBJ 02/20/2022
    }
    public class GIRPhotographs
    {
        public System.Guid UniqueFormID { get; set; }
        public long PhotographsID { get; set; }
        public long? GIRFormID { get; set; }
        public string ImagePath { get; set; }
        public string ImageCaption { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Ship { get; set; }
        public string FileName { get; set; }
    }

    public class GIRDeficienciesFile
    {
        public int GIRDeficienciesFileID { get; set; }
        public System.Guid DeficienciesUniqueID { get; set; }
        public System.Guid? DeficienciesFileUniqueID { get; set; } // JSL 06/04/2022
        public long? DeficienciesID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }
}
