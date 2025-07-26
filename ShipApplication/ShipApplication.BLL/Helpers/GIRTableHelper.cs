using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;

namespace ShipApplication.BLL.Helpers
{
    public class GIRTableHelper
    {
        public bool SaveGIRDataInLocalDB(GeneralInspectionReport Modal, bool synced)
        {
            bool res = false;
            try
            {
                if (Modal.UniqueFormID != Guid.Empty)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string UpdateQury = GETGIRUpdateQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(UpdateQury, connection);

                        Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime

                        GIRUpdateCMD(Modal, ref command);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GeneralInspectionReport);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GeneralInspectionReport); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                            string InsertQury = GeneralInspectionReportDataInsertQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);

                            Guid FormGUID = Guid.NewGuid();
                            Modal.UniqueFormID = FormGUID;
                            Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime

                            GeneralInspectionReportDataInsertCMD(Modal, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();
                        }
                    }
                }
                if (Modal.Manning_SafeMiningChanged == true)
                {
                    GIRSafeManningRequirements_Save(Modal.UniqueFormID, Modal.GIRSafeManningRequirements);
                }
                if (Modal.Manning_CrewDocsChanged == true)
                {
                    GIRCrewDocuments_Save(Modal.UniqueFormID, Modal.GIRCrewDocuments);
                }
                if (Modal.Manning_RestAndWorkChanged == true)
                {
                    GIRRestandWorkHours_Save(Modal.UniqueFormID, Modal.GIRRestandWorkHours);
                }
                if (Modal.Manning_DeficienciesChanged == true)
                {
                    GIRDeficiencies_Save(Modal.UniqueFormID, Modal.GIRDeficiencies);
                }
                if (Modal.Manning_PhotosChanged == true)
                {
                    GIRPhotos_Save(Modal.UniqueFormID, Modal.GIRPhotographs);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GeneralInspectionReport table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string GeneralInspectionReportDataInsertQuery()
        {
            // RDBJ 01/05/2022 Added isDelete
            //RDBJ 10/19/2021 Added 
            /*
            IsGeneralSectionComplete,IsManningSectionComplete,IsShipCertificationSectionComplete,IsRecordKeepingSectionComplete,
            IsSafetyEquipmentSectionComplete,IsSecuritySectionComplete,IsBridgeSectionComplete,IsMedicalSectionComplete,IsGalleySectionComplete,
            IsEngineRoomSectionComplete,IsSuperstructureSectionComplete,IsDeckSectionComplete,IsHoldsAndCoverSectionComplete,IsForeCastleSectionComplete,
            IsHullSectionComplete,IsSummarySectionComplete,IsDeficienciesSectionComplete,IsPhotographsSectionComplete
             */
            string InsertQury = @"INSERT INTO dbo.GeneralInspectionReport 
                                (UniqueFormID,FormVersion,ShipID,ShipName,Ship,Port,Inspector,Date,GeneralPreamble,Classsociety,YearofBuild,Flag,Classofvessel,
			                    Portofregistry,MMSI,IMOnumber,Callsign,SummerDWT,Grosstonnage,Lightweight,Nettonnage,Beam,LOA,Summerdraft,
			                    LBP,Bowthruster,BHP,Noofholds,Nomoveablebulkheads,Containers,Cargocapacity,Cargohandlingequipment,Lastvoyageandcargo,CurrentPlannedvoyageandcargo,
                                ShipboardWorkingArrangements,CertificationIndex,IsPubsAndDocsSectionComplete,CarriedOutByTheDOOW,IsRegs4shipsDVD,Regs4shipsDVD,IsSOPEPPoints,SOPEPPoints,IsBWMP,BWMP,IsBWMPSupplement,
                                BWMPSupplement,IsIntactStabilityManual,IntactStabilityManual,IsStabilityComputer,StabilityComputer,IsDateOfLast,DateOfLast,IsCargoSecuring,CargoSecuring,IsBulkCargo,BulkCargo,IsSMSManual,
                                SMSManual,IsRegisterOf,RegisterOf,IsFleetStandingOrder,FleetStandingOrder,IsFleetMemoranda,FleetMemoranda,IsShipsPlans,ShipsPlans,IsCollective,Collective,IsDraftAndFreeboardNotice,
                                DraftAndFreeboardNotice,IsPCSOPEP,PCSOPEP,IsNTVRP,NTVRP,IsVGP,VGP,PubsComments,OfficialLogbookA,OfficialLogbookB,OfficialLogbookC,OfficialLogbookD,OfficialLogbookE,DeckLogbook,
                                Listofcrew,LastHose,PassagePlanning,LoadingComputer,EngineLogbook,OilRecordBook,RiskAssessments,GMDSSLogbook,DeckLogbook5D,GarbageRecordBook,BallastWaterRecordBook,CargoRecordBook,EmissionsControlManual,
                                LGR,PEER,RecordKeepingComments,LastPortStateControl,LiferaftsComment,releasesComment,LifeboatComment,LifeboatdavitComment,LifeboatequipmentComment,RescueboatComment,
                                RescueboatequipmentComment,RescueboatoutboardmotorComment,RescueboatdavitComment,DeckComment,PyrotechnicsComment,EPIRBComment,SARTsComment,GMDSSComment,ManoverboardComment,
                                LinethrowingapparatusComment,FireextinguishersComment,EmergencygeneratorComment,CO2roomComment,SurvivalComment,LifejacketComment,FiremansComment,LifebuoysComment,FireboxesComment,
                                EmergencybellsComment,EmergencylightingComment,FireplanComment,DamageComment,EmergencyplansComment,MusterlistComment,SafetysignsComment,EmergencysteeringComment,
                                StatutoryemergencydrillsComment,EEBDComment,OxygenComment,MultigasdetectorComment,GasdetectorComment,SufficientquantityComment,BASetsComment,SafetyComment,GangwayComment,RestrictedComment,
                                OutsideComment,EntrancedoorsComment,AccommodationComment,GMDSSComment5G,VariousComment,SSOComment,SecuritylogbookComment,Listoflast10portsComment,PFSOComment,
                                SecuritylevelComment,DrillsandtrainingComment,DOSComment,SSASComment,VisitorslogbookComment,KeyregisterComment,ShipSecurityComment,SecurityComment,NauticalchartsComment,
                                NoticetomarinersComment,ListofradiosignalsComment,ListoflightsComment,SailingdirectionsComment,TidetablesComment,NavtexandprinterComment,RadarsComment,GPSComment,AISComment,
                                VDRComment,ECDISComment,EchosounderComment,ADPbackuplaptopComment,ColourprinterComment,VHFDSCtransceiverComment,radioinstallationComment,InmarsatCComment
                                ,MagneticcompassComment,SparecompassbowlComment,CompassobservationbookComment,GyrocompassComment,RudderindicatorComment,SpeedlogComment,NavigationComment,SignalflagsComment
                                ,RPMComment,BasicmanoeuvringdataComment,MasterstandingordersComment,MasternightordersbookComment,SextantComment,AzimuthmirrorComment,BridgepostersComment,ReviewofplannedComment
                                ,BridgebellbookComment,BridgenavigationalComment,SecurityEquipmentComment,NavigationPost,GeneralComment,MedicinestorageComment,MedicinechestcertificateComment
                                ,InventoryStoresComment,OxygencylindersComment,StretcherComment,SalivaComment,AlcoholComment,HospitalComment,GeneralGalleyComment,HygieneComment,FoodstorageComment
                                ,FoodlabellingComment,GalleyriskassessmentComment,FridgetemperatureComment,FoodandProvisionsComment,GalleyComment,ConditionComment,PaintworkComment,LightingComment,PlatesComment
                                ,BilgesComment,PipelinesandvalvesComment,LeakageComment,EquipmentComment,OilywaterseparatorComment,FueloiltransferplanComment,SteeringgearComment,WorkshopandequipmentComment
                                ,SoundingpipesComment,EnginecontrolComment,ChiefEngineernightordersbookComment,ChiefEngineerstandingordersComment,PreUMSComment,EnginebellbookComment,LockoutComment
                                ,EngineRoomComment,CleanlinessandhygieneComment,ConditionComment5M,PaintworkComment5M,SignalmastandstaysComment,MonkeyislandComment,FireDampersComment,RailsBulwarksComment
                                ,WatertightdoorsComment,VentilatorsComment,WinchesComment,FairleadsComment,MooringLinesComment,EmergencyShutOffsComment,RadioaerialsComment,SOPEPlockerComment,ChemicallockerComment
                                ,AntislippaintComment,SuperstructureComment,CabinsComment,OfficesComment,MessroomsComment,ToiletsComment,LaundryroomComment,ChangingroomComment,OtherComment,ConditionComment5N
                                ,SelfclosingfiredoorsComment,StairwellsComment,SuperstructureInternalComment,PortablegangwayComment,SafetynetComment,AccommodationLadderComment,SafeaccessprovidedComment
                                ,PilotladdersComment,BoardingEquipmentComment,CleanlinessComment,PaintworkComment5P,ShipsiderailsComment,WeathertightdoorsComment,FirehydrantsComment,VentilatorsComment5P
                                ,ManholecoversComment,MainDeckAreaComment,ConditionComment5Q,PaintworkComment5Q,MechanicaldamageComment,AccessladdersComment,ManholecoversComment5Q,HoldbilgeComment
                                ,AccessdoorsComment,ConditionHatchCoversComment,PaintworkHatchCoversComment,RubbersealsComment,SignsofhatchesComment,SealingtapeComment,ConditionofhydraulicsComment
                                ,PortablebulkheadsComment,TweendecksComment,HatchcoamingComment,ConditionCargoCranesComment,GantrycranealarmComment,GantryconditionComment,CargoHoldsComment
                                ,CleanlinessComment5R,PaintworkComment5R,TriphazardsComment,WindlassComment,CablesComment,WinchesComment5R,FairleadsComment5R,MooringComment,HatchToforecastlespaceComment
                                ,VentilatorsComment5R,BellComment,ForemastComment,FireComment,RailsComment,AntislippaintComment5R
                                ,SnapBackZoneComment,ConditionGantryCranesComment
                                ,SnapBackZone5NComment,MedicalLogBookComment,DrugsNarcoticsComment,DefibrillatorComment
                                ,RPWaterHandbook,BioRPWH,PRE,NoiseVibrationFile,BioMPR,AsbestosPlan,ShipPublicAddrComment
                                ,BridgewindowswiperssprayComment,BridgewindowswipersComment,DaylightSignalsComment
                                ,LiferaftDavitComment,CylindersLockerComment,ADPPublicationsComment
                                ,ForecastleComment,CleanlinessComment5S,PaintworkComment5S,ForepeakComment
                                ,ChainlockerComment,LightingComment5S,AccesssafetychainComment,EmergencyfirepumpComment,BowthrusterandroomComment,SparemooringlinesComment,PaintlockerComment,ForecastleSpaceComment
                                ,BoottopComment,TopsidesComment,AntifoulingComment,DraftandplimsollComment,FoulingComment,MechanicalComment,HullComment,SummaryComment,
                                IsSynced,CreatedDate,UpdatedDate,SavedAsDraft,IsGeneralSectionComplete,IsManningSectionComplete,IsShipCertificationSectionComplete,IsRecordKeepingSectionComplete,
                                IsSafetyEquipmentSectionComplete,IsSecuritySectionComplete,IsBridgeSectionComplete,IsMedicalSectionComplete,IsGalleySectionComplete,
                                IsEngineRoomSectionComplete,IsSuperstructureSectionComplete,IsDeckSectionComplete,IsHoldsAndCoverSectionComplete,IsForeCastleSectionComplete,
                                IsHullSectionComplete,IsSummarySectionComplete,IsDeficienciesSectionComplete,IsPhotographsSectionComplete, [isDelete])
                                OUTPUT INSERTED.GIRFormID
                                VALUES (@UniqueFormID,@FormVersion,@ShipID,@ShipName,@Ship,@Port,@Inspector,@Date,@GeneralPreamble,@Classsociety,@YearofBuild,@Flag,@Classofvessel,
                                @Portofregistry,@MMSI,@IMOnumber,@Callsign,@SummerDWT,@Grosstonnage,@Lightweight,@Nettonnage,@Beam,@LOA,@Summerdraft,
                                @LBP,@Bowthruster,@BHP,@Noofholds,@Nomoveablebulkheads,@Containers,@Cargocapacity,@Cargohandlingequipment,@Lastvoyageandcargo,@CurrentPlannedvoyageandcargo,
                                @ShipboardWorkingArrangements,@CertificationIndex,@IsPubsAndDocsSectionComplete,@CarriedOutByTheDOOW,@IsRegs4shipsDVD,@Regs4shipsDVD,@IsSOPEPPoints,@SOPEPPoints,@IsBWMP,@BWMP,@IsBWMPSupplement,
                                @BWMPSupplement,@IsIntactStabilityManual,@IntactStabilityManual,@IsStabilityComputer,@StabilityComputer,@IsDateOfLast,@DateOfLast,@IsCargoSecuring,@CargoSecuring,@IsBulkCargo,@BulkCargo,@IsSMSManual,
                                @SMSManual, @IsRegisterOf,@RegisterOf,@IsFleetStandingOrder,@FleetStandingOrder,@IsFleetMemoranda,@FleetMemoranda,@IsShipsPlans,@ShipsPlans,@IsCollective,@Collective,@IsDraftAndFreeboardNotice,
                                @DraftAndFreeboardNotice,@IsPCSOPEP,@PCSOPEP,@IsNTVRP,@NTVRP,@IsVGP,@VGP,@PubsComments,@OfficialLogbookA,@OfficialLogbookB,@OfficialLogbookC,@OfficialLogbookD,@OfficialLogbookE,@DeckLogbook,
                                @Listofcrew,@LastHose,@PassagePlanning,@LoadingComputer,@EngineLogbook,@OilRecordBook,@RiskAssessments,@GMDSSLogbook,@DeckLogbook5D,@GarbageRecordBook,@BallastWaterRecordBook,@CargoRecordBook,@EmissionsControlManual,
                                @LGR,@PEER,@RecordKeepingComments,@LastPortStateControl,@LiferaftsComment,@releasesComment,@LifeboatComment,@LifeboatdavitComment,@LifeboatequipmentComment,@RescueboatComment,
                                @RescueboatequipmentComment,@RescueboatoutboardmotorComment,@RescueboatdavitComment,@DeckComment,@PyrotechnicsComment,@EPIRBComment,@SARTsComment,@GMDSSComment,@ManoverboardComment,
                                @LinethrowingapparatusComment,@FireextinguishersComment,@EmergencygeneratorComment,@CO2roomComment,@SurvivalComment,@LifejacketComment,@FiremansComment,@LifebuoysComment,@FireboxesComment,
                                @EmergencybellsComment,@EmergencylightingComment,@FireplanComment,@DamageComment,@EmergencyplansComment,@MusterlistComment,@SafetysignsComment,@EmergencysteeringComment,
                                @StatutoryemergencydrillsComment,@EEBDComment,@OxygenComment,@MultigasdetectorComment,@GasdetectorComment,@SufficientquantityComment,@BASetsComment,@SafetyComment,@GangwayComment,@RestrictedComment,
                                @OutsideComment,@EntrancedoorsComment,@AccommodationComment,@GMDSSComment5G,@VariousComment,@SSOComment,@SecuritylogbookComment,@Listoflast10portsComment,@PFSOComment,
                                @SecuritylevelComment,@DrillsandtrainingComment,@DOSComment,@SSASComment,@VisitorslogbookComment,@KeyregisterComment,@ShipSecurityComment,@SecurityComment,@NauticalchartsComment,
                                @NoticetomarinersComment,@ListofradiosignalsComment,@ListoflightsComment,@SailingdirectionsComment,@TidetablesComment,@NavtexandprinterComment,@RadarsComment,@GPSComment,@AISComment,
                                @VDRComment,@ECDISComment,@EchosounderComment,@ADPbackuplaptopComment,@ColourprinterComment,@VHFDSCtransceiverComment,@radioinstallationComment,@InmarsatCComment
                                ,@MagneticcompassComment,@SparecompassbowlComment,@CompassobservationbookComment,@GyrocompassComment,@RudderindicatorComment,@SpeedlogComment,@NavigationComment,@SignalflagsComment
                                ,@RPMComment,@BasicmanoeuvringdataComment,@MasterstandingordersComment,@MasternightordersbookComment,@SextantComment,@AzimuthmirrorComment,@BridgepostersComment,@ReviewofplannedComment
                                ,@BridgebellbookComment,@BridgenavigationalComment,@SecurityEquipmentComment,@NavigationPost,@GeneralComment,@MedicinestorageComment,@MedicinechestcertificateComment
                                ,@InventoryStoresComment,@OxygencylindersComment,@StretcherComment,@SalivaComment,@AlcoholComment,@HospitalComment,@GeneralGalleyComment,@HygieneComment,@FoodstorageComment
                                ,@FoodlabellingComment,@GalleyriskassessmentComment,@FridgetemperatureComment,@FoodandProvisionsComment,@GalleyComment,@ConditionComment,@PaintworkComment,@LightingComment,@PlatesComment
                                ,@BilgesComment,@PipelinesandvalvesComment,@LeakageComment,@EquipmentComment,@OilywaterseparatorComment,@FueloiltransferplanComment,@SteeringgearComment,@WorkshopandequipmentComment
                                ,@SoundingpipesComment,@EnginecontrolComment,@ChiefEngineernightordersbookComment,@ChiefEngineerstandingordersComment,@PreUMSComment,@EnginebellbookComment,@LockoutComment
                                ,@EngineRoomComment,@CleanlinessandhygieneComment,@ConditionComment5M,@PaintworkComment5M,@SignalmastandstaysComment,@MonkeyislandComment,@FireDampersComment,@RailsBulwarksComment
                                ,@WatertightdoorsComment,@VentilatorsComment,@WinchesComment,@FairleadsComment,@MooringLinesComment,@EmergencyShutOffsComment,@RadioaerialsComment,@SOPEPlockerComment,@ChemicallockerComment
                                ,@AntislippaintComment,@SuperstructureComment,@CabinsComment,@OfficesComment,@MessroomsComment,@ToiletsComment,@LaundryroomComment,@ChangingroomComment,@OtherComment,@ConditionComment5N
                                ,@SelfclosingfiredoorsComment,@StairwellsComment,@SuperstructureInternalComment,@PortablegangwayComment,@SafetynetComment,@AccommodationLadderComment,@SafeaccessprovidedComment
                                ,@PilotladdersComment,@BoardingEquipmentComment,@CleanlinessComment,@PaintworkComment5P,@ShipsiderailsComment,@WeathertightdoorsComment,@FirehydrantsComment,@VentilatorsComment5P
                                ,@ManholecoversComment,@MainDeckAreaComment,@ConditionComment5Q,@PaintworkComment5Q,@MechanicaldamageComment,@AccessladdersComment,@ManholecoversComment5Q,@HoldbilgeComment
                                ,@AccessdoorsComment,@ConditionHatchCoversComment,@PaintworkHatchCoversComment,@RubbersealsComment,@SignsofhatchesComment,@SealingtapeComment,@ConditionofhydraulicsComment
                                ,@PortablebulkheadsComment,@TweendecksComment,@HatchcoamingComment,@ConditionCargoCranesComment,@GantrycranealarmComment,@GantryconditionComment,@CargoHoldsComment
                                ,@CleanlinessComment5R,@PaintworkComment5R,@TriphazardsComment,@WindlassComment,@CablesComment,@WinchesComment5R,@FairleadsComment5R,@MooringComment,@HatchToforecastlespaceComment
                                ,@VentilatorsComment5R,@BellComment,@ForemastComment,@FireComment,@RailsComment,@AntislippaintComment5R
                                ,@SnapBackZoneComment,@ConditionGantryCranesComment
                                ,@SnapBackZone5NComment,@MedicalLogBookComment,@DrugsNarcoticsComment,@DefibrillatorComment
                                ,@RPWaterHandbook,@BioRPWH,@PRE,@NoiseVibrationFile,@BioMPR,@AsbestosPlan,@ShipPublicAddrComment
                                ,@BridgewindowswiperssprayComment,@BridgewindowswipersComment,@DaylightSignalsComment
                                ,@LiferaftDavitComment,@CylindersLockerComment,@ADPPublicationsComment
                                ,@ForecastleComment,@CleanlinessComment5S,@PaintworkComment5S,@ForepeakComment
                                ,@ChainlockerComment,@LightingComment5S,@AccesssafetychainComment,@EmergencyfirepumpComment,@BowthrusterandroomComment,@SparemooringlinesComment,@PaintlockerComment,@ForecastleSpaceComment
                                ,@BoottopComment,@TopsidesComment,@AntifoulingComment,@DraftandplimsollComment,@FoulingComment,@MechanicalComment,@HullComment,@SummaryComment
                                ,@IsSynced,@CreatedDate,@UpdatedDate,@SavedAsDraft,@IsGeneralSectionComplete,@IsManningSectionComplete,@IsShipCertificationSectionComplete,@IsRecordKeepingSectionComplete
                                ,@IsSafetyEquipmentSectionComplete,@IsSecuritySectionComplete,@IsBridgeSectionComplete,@IsMedicalSectionComplete,@IsGalleySectionComplete,@IsEngineRoomSectionComplete
                                ,@IsSuperstructureSectionComplete,@IsDeckSectionComplete,@IsHoldsAndCoverSectionComplete,@IsForeCastleSectionComplete,@IsHullSectionComplete,@IsSummarySectionComplete
                                ,@IsDeficienciesSectionComplete,@IsPhotographsSectionComplete, @isDelete)";
            return InsertQury;
        }
        public void GeneralInspectionReportDataInsertCMD(GeneralInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.BigInt).Value = Modal.FormVersion;
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete == null ? 0 : (object)Modal.isDelete; // RDBJ 01/05/2022

            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID == null ? DBNull.Value : (object)Modal.ShipID;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName == null ? string.Empty : Modal.ShipName;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? string.Empty : Modal.Ship;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port == null ? string.Empty : Modal.Port;
            command.Parameters.Add("@Inspector", SqlDbType.NVarChar).Value = Modal.Inspector == null ? string.Empty : Modal.Inspector;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date == null ? DBNull.Value : (object)Modal.Date;
            command.Parameters.Add("@GeneralPreamble", SqlDbType.NVarChar).Value = Modal.GeneralPreamble != null ? Modal.GeneralPreamble : "";
            command.Parameters.Add("@Classsociety", SqlDbType.NVarChar).Value = Modal.Classsociety != null ? Modal.Classsociety : "";
            command.Parameters.Add("@YearofBuild", SqlDbType.NVarChar).Value = Modal.YearofBuild != null ? Modal.YearofBuild : "";
            command.Parameters.Add("@Flag", SqlDbType.NVarChar).Value = Modal.Flag != null ? Modal.Flag : "";
            command.Parameters.Add("@Classofvessel", SqlDbType.NVarChar).Value = Modal.Classofvessel != null ? Modal.Classofvessel : "";
            command.Parameters.Add("@Portofregistry", SqlDbType.NVarChar).Value = Modal.Portofregistry != null ? Modal.Portofregistry : "";
            command.Parameters.Add("@MMSI", SqlDbType.NVarChar).Value = Modal.MMSI != null ? Modal.MMSI : "";
            command.Parameters.Add("@IMOnumber", SqlDbType.NVarChar).Value = Modal.IMOnumber != null ? Modal.IMOnumber : "";
            command.Parameters.Add("@Callsign", SqlDbType.NVarChar).Value = Modal.Callsign != null ? Modal.Callsign : "";
            command.Parameters.Add("@SummerDWT", SqlDbType.NVarChar).Value = Modal.SummerDWT != null ? Modal.SummerDWT : "";
            command.Parameters.Add("@Grosstonnage", SqlDbType.NVarChar).Value = Modal.Grosstonnage != null ? Modal.Grosstonnage : "";
            command.Parameters.Add("@Lightweight", SqlDbType.NVarChar).Value = Modal.Lightweight != null ? Modal.Lightweight : "";
            command.Parameters.Add("@Nettonnage", SqlDbType.NVarChar).Value = Modal.Nettonnage != null ? Modal.Nettonnage : "";
            command.Parameters.Add("@Beam", SqlDbType.NVarChar).Value = Modal.Beam != null ? Modal.Beam : "";
            command.Parameters.Add("@LOA", SqlDbType.NVarChar).Value = Modal.LOA != null ? Modal.LOA : "";
            command.Parameters.Add("@Summerdraft", SqlDbType.NVarChar).Value = Modal.Summerdraft != null ? Modal.Summerdraft : "";
            command.Parameters.Add("@LBP", SqlDbType.NVarChar).Value = Modal.LBP != null ? Modal.LBP : "";
            command.Parameters.Add("@Bowthruster", SqlDbType.NVarChar).Value = Modal.Bowthruster != null ? Modal.Bowthruster : "";
            command.Parameters.Add("@BHP", SqlDbType.NVarChar).Value = Modal.BHP != null ? Modal.BHP : "";
            command.Parameters.Add("@Noofholds", SqlDbType.NVarChar).Value = Modal.Noofholds != null ? Modal.Noofholds : "";
            command.Parameters.Add("@Nomoveablebulkheads", SqlDbType.NVarChar).Value = Modal.Nomoveablebulkheads != null ? Modal.Nomoveablebulkheads : "";
            command.Parameters.Add("@Containers", SqlDbType.NVarChar).Value = Modal.Containers != null ? Modal.Containers : "";
            command.Parameters.Add("@Cargocapacity", SqlDbType.NVarChar).Value = Modal.Cargocapacity != null ? Modal.Cargocapacity : "";
            command.Parameters.Add("@Cargohandlingequipment", SqlDbType.NVarChar).Value = Modal.Cargohandlingequipment != null ? Modal.Cargohandlingequipment : "";
            command.Parameters.Add("@Lastvoyageandcargo", SqlDbType.NVarChar).Value = Modal.Lastvoyageandcargo != null ? Modal.Lastvoyageandcargo : "";
            command.Parameters.Add("@CurrentPlannedvoyageandcargo", SqlDbType.NVarChar).Value = Modal.CurrentPlannedvoyageandcargo != null ? Modal.CurrentPlannedvoyageandcargo : "";

            command.Parameters.Add("@ShipboardWorkingArrangements", SqlDbType.NVarChar).Value = Modal.ShipboardWorkingArrangements != null ? Modal.ShipboardWorkingArrangements : "";
            command.Parameters.Add("@CertificationIndex", SqlDbType.NVarChar).Value = Modal.CertificationIndex != null ? Modal.CertificationIndex : "";
            command.Parameters.Add("@IsPubsAndDocsSectionComplete", SqlDbType.Bit).Value = Modal.IsPubsAndDocsSectionComplete == null ? DBNull.Value : (object)Modal.IsPubsAndDocsSectionComplete;
            command.Parameters.Add("@CarriedOutByTheDOOW", SqlDbType.NVarChar).Value = Modal.CarriedOutByTheDOOW != null ? Modal.CarriedOutByTheDOOW : "";
            command.Parameters.Add("@IsSOPEPPoints", SqlDbType.Bit).Value = Modal.IsSOPEPPoints == null ? DBNull.Value : (object)Modal.IsSOPEPPoints;
            command.Parameters.Add("@SOPEPPoints", SqlDbType.NVarChar).Value = Modal.SOPEPPoints != null ? Modal.SOPEPPoints : "";
            command.Parameters.Add("@IsRegs4shipsDVD", SqlDbType.Bit).Value = Modal.IsRegs4shipsDVD == null ? DBNull.Value : (object)Modal.IsRegs4shipsDVD;
            command.Parameters.Add("@Regs4shipsDVD", SqlDbType.NVarChar).Value = Modal.Regs4shipsDVD != null ? Modal.Regs4shipsDVD : "";

            command.Parameters.Add("@IsBWMP", SqlDbType.Bit).Value = Modal.IsBWMP == null ? DBNull.Value : (object)Modal.IsBWMP;
            command.Parameters.Add("@BWMP", SqlDbType.NVarChar).Value = Modal.BWMP != null ? Modal.BWMP : "";
            command.Parameters.Add("@IsBWMPSupplement", SqlDbType.Bit).Value = Modal.IsBWMPSupplement == null ? DBNull.Value : (object)Modal.IsBWMPSupplement;
            command.Parameters.Add("@BWMPSupplement", SqlDbType.NVarChar).Value = Modal.BWMPSupplement != null ? Modal.BWMPSupplement : "";
            command.Parameters.Add("@IsIntactStabilityManual", SqlDbType.Bit).Value = Modal.IsIntactStabilityManual == null ? DBNull.Value : (object)Modal.IsIntactStabilityManual;
            command.Parameters.Add("@IntactStabilityManual", SqlDbType.NVarChar).Value = Modal.IntactStabilityManual != null ? Modal.IntactStabilityManual : "";
            command.Parameters.Add("@IsStabilityComputer", SqlDbType.Bit).Value = Modal.IsStabilityComputer == null ? DBNull.Value : (object)Modal.IsStabilityComputer;
            command.Parameters.Add("@StabilityComputer", SqlDbType.NVarChar).Value = Modal.StabilityComputer != null ? Modal.StabilityComputer : "";
            command.Parameters.Add("@IsDateOfLast", SqlDbType.Bit).Value = Modal.IsDateOfLast == null ? DBNull.Value : (object)Modal.IsDateOfLast;
            command.Parameters.Add("@DateOfLast", SqlDbType.NVarChar).Value = Modal.DateOfLast != null ? Modal.DateOfLast : "";
            command.Parameters.Add("@IsCargoSecuring", SqlDbType.Bit).Value = Modal.IsCargoSecuring == null ? DBNull.Value : (object)Modal.IsCargoSecuring;
            command.Parameters.Add("@CargoSecuring", SqlDbType.NVarChar).Value = Modal.CargoSecuring != null ? Modal.CargoSecuring : "";
            command.Parameters.Add("@IsBulkCargo", SqlDbType.Bit).Value = Modal.IsBulkCargo == null ? DBNull.Value : (object)Modal.IsBulkCargo;
            command.Parameters.Add("@BulkCargo", SqlDbType.NVarChar).Value = Modal.BulkCargo != null ? Modal.BulkCargo : "";
            command.Parameters.Add("@IsSMSManual", SqlDbType.Bit).Value = Modal.IsSMSManual == null ? DBNull.Value : (object)Modal.IsSMSManual;
            command.Parameters.Add("@SMSManual", SqlDbType.NVarChar).Value = Modal.SMSManual != null ? Modal.SMSManual : "";
            command.Parameters.Add("@IsRegisterOf", SqlDbType.Bit).Value = Modal.IsRegisterOf == null ? DBNull.Value : (object)Modal.IsRegisterOf;
            command.Parameters.Add("@RegisterOf", SqlDbType.NVarChar).Value = Modal.RegisterOf != null ? Modal.RegisterOf : "";
            command.Parameters.Add("@IsFleetStandingOrder", SqlDbType.Bit).Value = Modal.IsFleetStandingOrder == null ? DBNull.Value : (object)Modal.IsFleetStandingOrder;
            command.Parameters.Add("@FleetStandingOrder", SqlDbType.NVarChar).Value = Modal.FleetStandingOrder != null ? Modal.FleetStandingOrder : "";
            command.Parameters.Add("@IsFleetMemoranda", SqlDbType.Bit).Value = Modal.IsFleetMemoranda == null ? DBNull.Value : (object)Modal.IsFleetMemoranda;
            command.Parameters.Add("@FleetMemoranda", SqlDbType.NVarChar).Value = Modal.FleetMemoranda != null ? Modal.FleetMemoranda : "";
            command.Parameters.Add("@IsShipsPlans", SqlDbType.Bit).Value = Modal.IsShipsPlans == null ? DBNull.Value : (object)Modal.IsShipsPlans;
            command.Parameters.Add("@ShipsPlans", SqlDbType.NVarChar).Value = Modal.ShipsPlans != null ? Modal.ShipsPlans : "";
            command.Parameters.Add("@IsCollective", SqlDbType.Bit).Value = Modal.IsCollective == null ? DBNull.Value : (object)Modal.IsCollective;
            command.Parameters.Add("@Collective", SqlDbType.NVarChar).Value = Modal.Collective != null ? Modal.Collective : "";
            command.Parameters.Add("@IsDraftAndFreeboardNotice", SqlDbType.Bit).Value = Modal.IsDraftAndFreeboardNotice == null ? DBNull.Value : (object)Modal.IsDraftAndFreeboardNotice;
            command.Parameters.Add("@DraftAndFreeboardNotice", SqlDbType.NVarChar).Value = Modal.DraftAndFreeboardNotice != null ? Modal.DraftAndFreeboardNotice : "";
            command.Parameters.Add("@IsPCSOPEP", SqlDbType.Bit).Value = Modal.IsPCSOPEP == null ? DBNull.Value : (object)Modal.IsPCSOPEP;
            command.Parameters.Add("@PCSOPEP", SqlDbType.NVarChar).Value = Modal.PCSOPEP != null ? Modal.PCSOPEP : "";
            command.Parameters.Add("@IsNTVRP", SqlDbType.Bit).Value = Modal.IsNTVRP == null ? DBNull.Value : (object)Modal.IsNTVRP;
            command.Parameters.Add("@NTVRP", SqlDbType.NVarChar).Value = Modal.NTVRP != null ? Modal.NTVRP : "";
            command.Parameters.Add("@IsVGP", SqlDbType.Bit).Value = Modal.IsVGP == null ? DBNull.Value : (object)Modal.IsVGP;
            command.Parameters.Add("@VGP", SqlDbType.NVarChar).Value = Modal.VGP != null ? Modal.VGP : "";
            command.Parameters.Add("@PubsComments", SqlDbType.NVarChar).Value = Modal.PubsComments != null ? Modal.PubsComments : "";

            command.Parameters.Add("@OfficialLogbookA", SqlDbType.NVarChar).Value = Modal.OfficialLogbookA != null ? Modal.OfficialLogbookA : "";
            command.Parameters.Add("@OfficialLogbookB", SqlDbType.NVarChar).Value = Modal.OfficialLogbookB != null ? Modal.OfficialLogbookB : "";
            command.Parameters.Add("@OfficialLogbookC", SqlDbType.NVarChar).Value = Modal.OfficialLogbookC != null ? Modal.OfficialLogbookC : "";
            command.Parameters.Add("@OfficialLogbookD", SqlDbType.NVarChar).Value = Modal.OfficialLogbookD != null ? Modal.OfficialLogbookD : "";
            command.Parameters.Add("@OfficialLogbookE", SqlDbType.NVarChar).Value = Modal.OfficialLogbookE != null ? Modal.OfficialLogbookE : "";
            command.Parameters.Add("@DeckLogbook", SqlDbType.NVarChar).Value = Modal.DeckLogbook != null ? Modal.DeckLogbook : "";
            command.Parameters.Add("@Listofcrew", SqlDbType.NVarChar).Value = Modal.Listofcrew != null ? Modal.Listofcrew : "";
            command.Parameters.Add("@LastHose", SqlDbType.NVarChar).Value = Modal.LastHose != null ? Modal.LastHose : "";
            command.Parameters.Add("@PassagePlanning", SqlDbType.NVarChar).Value = Modal.PassagePlanning != null ? Modal.PassagePlanning : "";
            command.Parameters.Add("@LoadingComputer", SqlDbType.NVarChar).Value = Modal.LoadingComputer != null ? Modal.LoadingComputer : "";
            command.Parameters.Add("@EngineLogbook", SqlDbType.NVarChar).Value = Modal.EngineLogbook != null ? Modal.EngineLogbook : "";
            command.Parameters.Add("@OilRecordBook", SqlDbType.NVarChar).Value = Modal.OilRecordBook != null ? Modal.OilRecordBook : "";
            command.Parameters.Add("@RiskAssessments", SqlDbType.NVarChar).Value = Modal.RiskAssessments != null ? Modal.RiskAssessments : "";
            command.Parameters.Add("@GMDSSLogbook", SqlDbType.NVarChar).Value = Modal.GMDSSLogbook != null ? Modal.GMDSSLogbook : "";
            command.Parameters.Add("@DeckLogbook5D", SqlDbType.NVarChar).Value = Modal.DeckLogbook5D != null ? Modal.DeckLogbook5D : "";
            command.Parameters.Add("@GarbageRecordBook", SqlDbType.NVarChar).Value = Modal.GarbageRecordBook != null ? Modal.GarbageRecordBook : "";
            command.Parameters.Add("@BallastWaterRecordBook", SqlDbType.NVarChar).Value = Modal.BallastWaterRecordBook != null ? Modal.BallastWaterRecordBook : "";
            command.Parameters.Add("@CargoRecordBook", SqlDbType.NVarChar).Value = Modal.CargoRecordBook != null ? Modal.CargoRecordBook : "";
            command.Parameters.Add("@EmissionsControlManual", SqlDbType.NVarChar).Value = Modal.EmissionsControlManual != null ? Modal.EmissionsControlManual : "";
            command.Parameters.Add("@LGR", SqlDbType.NVarChar).Value = Modal.LGR != null ? Modal.LGR : "";
            command.Parameters.Add("@PEER", SqlDbType.NVarChar).Value = Modal.PEER != null ? Modal.PEER : "";
            command.Parameters.Add("@RecordKeepingComments", SqlDbType.NVarChar).Value = Modal.RecordKeepingComments != null ? Modal.RecordKeepingComments : "";

            command.Parameters.Add("@LastPortStateControl", SqlDbType.NVarChar).Value = Modal.LastPortStateControl != null ? Modal.LastPortStateControl : "";
            command.Parameters.Add("@LiferaftsComment", SqlDbType.NVarChar).Value = Modal.LiferaftsComment != null ? Modal.LiferaftsComment : "";
            command.Parameters.Add("@releasesComment", SqlDbType.NVarChar).Value = Modal.releasesComment != null ? Modal.releasesComment : "";
            command.Parameters.Add("@LifeboatComment", SqlDbType.NVarChar).Value = Modal.LifeboatComment != null ? Modal.LifeboatComment : "";
            command.Parameters.Add("@LifeboatdavitComment", SqlDbType.NVarChar).Value = Modal.LifeboatdavitComment != null ? Modal.LifeboatdavitComment : "";
            command.Parameters.Add("@LifeboatequipmentComment", SqlDbType.NVarChar).Value = Modal.LifeboatequipmentComment != null ? Modal.LifeboatequipmentComment : "";
            command.Parameters.Add("@RescueboatComment", SqlDbType.NVarChar).Value = Modal.RescueboatComment != null ? Modal.RescueboatComment : "";
            command.Parameters.Add("@RescueboatequipmentComment", SqlDbType.NVarChar).Value = Modal.RescueboatequipmentComment != null ? Modal.RescueboatequipmentComment : "";
            command.Parameters.Add("@RescueboatoutboardmotorComment", SqlDbType.NVarChar).Value = Modal.RescueboatoutboardmotorComment != null ? Modal.RescueboatoutboardmotorComment : "";
            command.Parameters.Add("@RescueboatdavitComment", SqlDbType.NVarChar).Value = Modal.RescueboatdavitComment != null ? Modal.RescueboatdavitComment : "";
            command.Parameters.Add("@DeckComment", SqlDbType.NVarChar).Value = Modal.DeckComment != null ? Modal.DeckComment : "";
            command.Parameters.Add("@PyrotechnicsComment", SqlDbType.NVarChar).Value = Modal.PyrotechnicsComment != null ? Modal.PyrotechnicsComment : "";
            command.Parameters.Add("@EPIRBComment", SqlDbType.NVarChar).Value = Modal.EPIRBComment != null ? Modal.EPIRBComment : "";
            command.Parameters.Add("@SARTsComment", SqlDbType.NVarChar).Value = Modal.SARTsComment != null ? Modal.SARTsComment : "";
            command.Parameters.Add("@GMDSSComment", SqlDbType.NVarChar).Value = Modal.GMDSSComment != null ? Modal.GMDSSComment : "";
            command.Parameters.Add("@ManoverboardComment", SqlDbType.NVarChar).Value = Modal.ManoverboardComment != null ? Modal.ManoverboardComment : "";
            command.Parameters.Add("@LinethrowingapparatusComment", SqlDbType.NVarChar).Value = Modal.LinethrowingapparatusComment != null ? Modal.LinethrowingapparatusComment : "";
            command.Parameters.Add("@FireextinguishersComment", SqlDbType.NVarChar).Value = Modal.FireextinguishersComment != null ? Modal.FireextinguishersComment : "";
            command.Parameters.Add("@EmergencygeneratorComment", SqlDbType.NVarChar).Value = Modal.EmergencygeneratorComment != null ? Modal.EmergencygeneratorComment : "";
            command.Parameters.Add("@CO2roomComment", SqlDbType.NVarChar).Value = Modal.CO2roomComment != null ? Modal.CO2roomComment : "";
            command.Parameters.Add("@SurvivalComment", SqlDbType.NVarChar).Value = Modal.SurvivalComment != null ? Modal.SurvivalComment : "";
            command.Parameters.Add("@LifejacketComment", SqlDbType.NVarChar).Value = Modal.LifejacketComment != null ? Modal.LifejacketComment : "";
            command.Parameters.Add("@FiremansComment", SqlDbType.NVarChar).Value = Modal.FiremansComment != null ? Modal.FiremansComment : "";
            command.Parameters.Add("@LifebuoysComment", SqlDbType.NVarChar).Value = Modal.LifebuoysComment != null ? Modal.LifebuoysComment : "";
            command.Parameters.Add("@FireboxesComment", SqlDbType.NVarChar).Value = Modal.FireboxesComment != null ? Modal.FireboxesComment : "";
            command.Parameters.Add("@EmergencybellsComment", SqlDbType.NVarChar).Value = Modal.EmergencybellsComment != null ? Modal.EmergencybellsComment : "";
            command.Parameters.Add("@EmergencylightingComment", SqlDbType.NVarChar).Value = Modal.EmergencylightingComment != null ? Modal.EmergencylightingComment : "";
            command.Parameters.Add("@FireplanComment", SqlDbType.NVarChar).Value = Modal.FireplanComment != null ? Modal.FireplanComment : "";
            command.Parameters.Add("@DamageComment", SqlDbType.NVarChar).Value = Modal.DamageComment != null ? Modal.DamageComment : "";
            command.Parameters.Add("@EmergencyplansComment", SqlDbType.NVarChar).Value = Modal.EmergencyplansComment != null ? Modal.EmergencyplansComment : "";
            command.Parameters.Add("@MusterlistComment", SqlDbType.NVarChar).Value = Modal.MusterlistComment != null ? Modal.MusterlistComment : "";
            command.Parameters.Add("@SafetysignsComment", SqlDbType.NVarChar).Value = Modal.SafetysignsComment != null ? Modal.SafetysignsComment : "";
            command.Parameters.Add("@EmergencysteeringComment", SqlDbType.NVarChar).Value = Modal.EmergencysteeringComment != null ? Modal.EmergencysteeringComment : "";
            command.Parameters.Add("@StatutoryemergencydrillsComment", SqlDbType.NVarChar).Value = Modal.StatutoryemergencydrillsComment != null ? Modal.StatutoryemergencydrillsComment : "";
            command.Parameters.Add("@EEBDComment", SqlDbType.NVarChar).Value = Modal.EEBDComment != null ? Modal.EEBDComment : "";
            command.Parameters.Add("@OxygenComment", SqlDbType.NVarChar).Value = Modal.OxygenComment != null ? Modal.OxygenComment : "";
            command.Parameters.Add("@MultigasdetectorComment", SqlDbType.NVarChar).Value = Modal.MultigasdetectorComment != null ? Modal.MultigasdetectorComment : "";
            command.Parameters.Add("@GasdetectorComment", SqlDbType.NVarChar).Value = Modal.GasdetectorComment != null ? Modal.GasdetectorComment : "";
            command.Parameters.Add("@SufficientquantityComment", SqlDbType.NVarChar).Value = Modal.SufficientquantityComment != null ? Modal.SufficientquantityComment : "";
            command.Parameters.Add("@BASetsComment", SqlDbType.NVarChar).Value = Modal.BASetsComment != null ? Modal.BASetsComment : "";
            command.Parameters.Add("@SafetyComment", SqlDbType.NVarChar).Value = Modal.SafetyComment != null ? Modal.SafetyComment : "";

            command.Parameters.Add("@GangwayComment", SqlDbType.NVarChar).Value = Modal.GangwayComment != null ? Modal.GangwayComment : "";
            command.Parameters.Add("@RestrictedComment", SqlDbType.NVarChar).Value = Modal.RestrictedComment != null ? Modal.RestrictedComment : "";
            command.Parameters.Add("@OutsideComment", SqlDbType.NVarChar).Value = Modal.OutsideComment != null ? Modal.OutsideComment : "";
            command.Parameters.Add("@EntrancedoorsComment", SqlDbType.NVarChar).Value = Modal.EntrancedoorsComment != null ? Modal.EntrancedoorsComment : "";
            command.Parameters.Add("@AccommodationComment", SqlDbType.NVarChar).Value = Modal.AccommodationComment != null ? Modal.AccommodationComment : "";
            command.Parameters.Add("@GMDSSComment5G", SqlDbType.NVarChar).Value = Modal.GMDSSComment5G != null ? Modal.GMDSSComment5G : "";
            command.Parameters.Add("@VariousComment", SqlDbType.NVarChar).Value = Modal.VariousComment != null ? Modal.VariousComment : "";
            command.Parameters.Add("@SSOComment", SqlDbType.NVarChar).Value = Modal.SSOComment != null ? Modal.SSOComment : "";
            command.Parameters.Add("@SecuritylogbookComment", SqlDbType.NVarChar).Value = Modal.SecuritylogbookComment != null ? Modal.SecuritylogbookComment : "";
            command.Parameters.Add("@Listoflast10portsComment", SqlDbType.NVarChar).Value = Modal.Listoflast10portsComment != null ? Modal.Listoflast10portsComment : "";
            command.Parameters.Add("@PFSOComment", SqlDbType.NVarChar).Value = Modal.PFSOComment != null ? Modal.PFSOComment : "";
            command.Parameters.Add("@SecuritylevelComment", SqlDbType.NVarChar).Value = Modal.SecuritylevelComment != null ? Modal.SecuritylevelComment : "";
            command.Parameters.Add("@DrillsandtrainingComment", SqlDbType.NVarChar).Value = Modal.DrillsandtrainingComment != null ? Modal.DrillsandtrainingComment : "";
            command.Parameters.Add("@DOSComment", SqlDbType.NVarChar).Value = Modal.DOSComment != null ? Modal.DOSComment : "";
            command.Parameters.Add("@SSASComment", SqlDbType.NVarChar).Value = Modal.SSASComment != null ? Modal.SSASComment : "";
            command.Parameters.Add("@VisitorslogbookComment", SqlDbType.NVarChar).Value = Modal.VisitorslogbookComment != null ? Modal.VisitorslogbookComment : "";
            command.Parameters.Add("@KeyregisterComment", SqlDbType.NVarChar).Value = Modal.KeyregisterComment != null ? Modal.KeyregisterComment : "";
            command.Parameters.Add("@ShipSecurityComment", SqlDbType.NVarChar).Value = Modal.ShipSecurityComment != null ? Modal.ShipSecurityComment : "";
            command.Parameters.Add("@SecurityComment", SqlDbType.NVarChar).Value = Modal.SecurityComment != null ? Modal.SecurityComment : "";

            command.Parameters.Add("@NauticalchartsComment", SqlDbType.NVarChar).Value = Modal.NauticalchartsComment != null ? Modal.NauticalchartsComment : "";
            command.Parameters.Add("@NoticetomarinersComment", SqlDbType.NVarChar).Value = Modal.NoticetomarinersComment != null ? Modal.NoticetomarinersComment : "";
            command.Parameters.Add("@ListofradiosignalsComment", SqlDbType.NVarChar).Value = Modal.ListofradiosignalsComment != null ? Modal.ListofradiosignalsComment : "";
            command.Parameters.Add("@ListoflightsComment", SqlDbType.NVarChar).Value = Modal.ListoflightsComment != null ? Modal.ListoflightsComment : "";
            command.Parameters.Add("@SailingdirectionsComment", SqlDbType.NVarChar).Value = Modal.SailingdirectionsComment != null ? Modal.SailingdirectionsComment : "";
            command.Parameters.Add("@TidetablesComment", SqlDbType.NVarChar).Value = Modal.TidetablesComment != null ? Modal.TidetablesComment : "";
            command.Parameters.Add("@NavtexandprinterComment", SqlDbType.NVarChar).Value = Modal.NavtexandprinterComment != null ? Modal.NavtexandprinterComment : "";
            command.Parameters.Add("@RadarsComment", SqlDbType.NVarChar).Value = Modal.RadarsComment != null ? Modal.RadarsComment : "";
            command.Parameters.Add("@GPSComment", SqlDbType.NVarChar).Value = Modal.GPSComment != null ? Modal.GPSComment : "";
            command.Parameters.Add("@AISComment", SqlDbType.NVarChar).Value = Modal.AISComment != null ? Modal.AISComment : "";
            command.Parameters.Add("@VDRComment", SqlDbType.NVarChar).Value = Modal.VDRComment != null ? Modal.VDRComment : "";
            command.Parameters.Add("@ECDISComment", SqlDbType.NVarChar).Value = Modal.ECDISComment != null ? Modal.ECDISComment : "";
            command.Parameters.Add("@EchosounderComment", SqlDbType.NVarChar).Value = Modal.EchosounderComment != null ? Modal.EchosounderComment : "";
            command.Parameters.Add("@ADPbackuplaptopComment", SqlDbType.NVarChar).Value = Modal.ADPbackuplaptopComment != null ? Modal.ADPbackuplaptopComment : "";
            command.Parameters.Add("@ColourprinterComment", SqlDbType.NVarChar).Value = Modal.ColourprinterComment != null ? Modal.ColourprinterComment : "";
            command.Parameters.Add("@VHFDSCtransceiverComment", SqlDbType.NVarChar).Value = Modal.VHFDSCtransceiverComment != null ? Modal.VHFDSCtransceiverComment : "";
            command.Parameters.Add("@radioinstallationComment", SqlDbType.NVarChar).Value = Modal.radioinstallationComment != null ? Modal.radioinstallationComment : "";
            command.Parameters.Add("@InmarsatCComment", SqlDbType.NVarChar).Value = Modal.InmarsatCComment != null ? Modal.InmarsatCComment : "";
            command.Parameters.Add("@MagneticcompassComment", SqlDbType.NVarChar).Value = Modal.MagneticcompassComment != null ? Modal.MagneticcompassComment : "";
            command.Parameters.Add("@SparecompassbowlComment", SqlDbType.NVarChar).Value = Modal.SparecompassbowlComment != null ? Modal.SparecompassbowlComment : "";
            command.Parameters.Add("@CompassobservationbookComment", SqlDbType.NVarChar).Value = Modal.CompassobservationbookComment != null ? Modal.CompassobservationbookComment : "";
            command.Parameters.Add("@GyrocompassComment", SqlDbType.NVarChar).Value = Modal.GyrocompassComment != null ? Modal.GyrocompassComment : "";
            command.Parameters.Add("@RudderindicatorComment", SqlDbType.NVarChar).Value = Modal.RudderindicatorComment != null ? Modal.RudderindicatorComment : "";
            command.Parameters.Add("@SpeedlogComment", SqlDbType.NVarChar).Value = Modal.SpeedlogComment != null ? Modal.SpeedlogComment : "";
            command.Parameters.Add("@NavigationComment", SqlDbType.NVarChar).Value = Modal.NavigationComment != null ? Modal.NavigationComment : "";
            command.Parameters.Add("@SignalflagsComment", SqlDbType.NVarChar).Value = Modal.SignalflagsComment != null ? Modal.SignalflagsComment : "";
            command.Parameters.Add("@RPMComment", SqlDbType.NVarChar).Value = Modal.RPMComment != null ? Modal.RPMComment : "";
            command.Parameters.Add("@BasicmanoeuvringdataComment", SqlDbType.NVarChar).Value = Modal.BasicmanoeuvringdataComment != null ? Modal.BasicmanoeuvringdataComment : "";
            command.Parameters.Add("@MasterstandingordersComment", SqlDbType.NVarChar).Value = Modal.MasterstandingordersComment != null ? Modal.MasterstandingordersComment : "";
            command.Parameters.Add("@MasternightordersbookComment", SqlDbType.NVarChar).Value = Modal.MasternightordersbookComment != null ? Modal.MasternightordersbookComment : "";
            command.Parameters.Add("@SextantComment", SqlDbType.NVarChar).Value = Modal.SextantComment != null ? Modal.SextantComment : "";
            command.Parameters.Add("@AzimuthmirrorComment", SqlDbType.NVarChar).Value = Modal.AzimuthmirrorComment != null ? Modal.AzimuthmirrorComment : "";
            command.Parameters.Add("@BridgepostersComment", SqlDbType.NVarChar).Value = Modal.BridgepostersComment != null ? Modal.BridgepostersComment : "";
            command.Parameters.Add("@ReviewofplannedComment", SqlDbType.NVarChar).Value = Modal.ReviewofplannedComment != null ? Modal.ReviewofplannedComment : "";
            command.Parameters.Add("@BridgebellbookComment", SqlDbType.NVarChar).Value = Modal.BridgebellbookComment != null ? Modal.BridgebellbookComment : "";
            command.Parameters.Add("@BridgenavigationalComment", SqlDbType.NVarChar).Value = Modal.BridgenavigationalComment != null ? Modal.BridgenavigationalComment : "";
            command.Parameters.Add("@SecurityEquipmentComment", SqlDbType.NVarChar).Value = Modal.SecurityEquipmentComment != null ? Modal.SecurityEquipmentComment : "";
            command.Parameters.Add("@NavigationPost", SqlDbType.NVarChar).Value = Modal.NavigationPost != null ? Modal.NavigationPost : "";

            command.Parameters.Add("@GeneralComment", SqlDbType.NVarChar).Value = Modal.GeneralComment != null ? Modal.GeneralComment : "";
            command.Parameters.Add("@MedicinestorageComment", SqlDbType.NVarChar).Value = Modal.MedicinestorageComment != null ? Modal.MedicinestorageComment : "";
            command.Parameters.Add("@MedicinechestcertificateComment", SqlDbType.NVarChar).Value = Modal.MedicinechestcertificateComment != null ? Modal.MedicinechestcertificateComment : "";
            command.Parameters.Add("@InventoryStoresComment", SqlDbType.NVarChar).Value = Modal.InventoryStoresComment != null ? Modal.InventoryStoresComment : "";
            command.Parameters.Add("@OxygencylindersComment", SqlDbType.NVarChar).Value = Modal.OxygencylindersComment != null ? Modal.OxygencylindersComment : "";
            command.Parameters.Add("@StretcherComment", SqlDbType.NVarChar).Value = Modal.StretcherComment != null ? Modal.StretcherComment : "";
            command.Parameters.Add("@SalivaComment", SqlDbType.NVarChar).Value = Modal.SalivaComment != null ? Modal.SalivaComment : "";
            command.Parameters.Add("@AlcoholComment", SqlDbType.NVarChar).Value = Modal.AlcoholComment != null ? Modal.AlcoholComment : "";
            command.Parameters.Add("@HospitalComment", SqlDbType.NVarChar).Value = Modal.HospitalComment != null ? Modal.HospitalComment : "";


            command.Parameters.Add("@GeneralGalleyComment", SqlDbType.NVarChar).Value = Modal.GeneralGalleyComment != null ? Modal.GeneralGalleyComment : "";
            command.Parameters.Add("@HygieneComment", SqlDbType.NVarChar).Value = Modal.HygieneComment != null ? Modal.HygieneComment : "";
            command.Parameters.Add("@FoodstorageComment", SqlDbType.NVarChar).Value = Modal.FoodstorageComment != null ? Modal.FoodstorageComment : "";
            command.Parameters.Add("@FoodlabellingComment", SqlDbType.NVarChar).Value = Modal.FoodlabellingComment != null ? Modal.FoodlabellingComment : "";
            command.Parameters.Add("@GalleyriskassessmentComment", SqlDbType.NVarChar).Value = Modal.GalleyriskassessmentComment != null ? Modal.GalleyriskassessmentComment : "";
            command.Parameters.Add("@FridgetemperatureComment", SqlDbType.NVarChar).Value = Modal.FridgetemperatureComment != null ? Modal.FridgetemperatureComment : "";
            command.Parameters.Add("@FoodandProvisionsComment", SqlDbType.NVarChar).Value = Modal.FoodandProvisionsComment != null ? Modal.FoodandProvisionsComment : "";
            command.Parameters.Add("@GalleyComment", SqlDbType.NVarChar).Value = Modal.GalleyComment != null ? Modal.GalleyComment : "";


            command.Parameters.Add("@ConditionComment", SqlDbType.NVarChar).Value = Modal.ConditionComment != null ? Modal.ConditionComment : "";
            command.Parameters.Add("@PaintworkComment", SqlDbType.NVarChar).Value = Modal.PaintworkComment != null ? Modal.PaintworkComment : "";
            command.Parameters.Add("@LightingComment", SqlDbType.NVarChar).Value = Modal.LightingComment != null ? Modal.LightingComment : "";
            command.Parameters.Add("@PlatesComment", SqlDbType.NVarChar).Value = Modal.PlatesComment != null ? Modal.PlatesComment : "";
            command.Parameters.Add("@BilgesComment", SqlDbType.NVarChar).Value = Modal.BilgesComment != null ? Modal.BilgesComment : "";
            command.Parameters.Add("@PipelinesandvalvesComment", SqlDbType.NVarChar).Value = Modal.PipelinesandvalvesComment != null ? Modal.PipelinesandvalvesComment : "";
            command.Parameters.Add("@LeakageComment", SqlDbType.NVarChar).Value = Modal.LeakageComment != null ? Modal.LeakageComment : "";
            command.Parameters.Add("@EquipmentComment", SqlDbType.NVarChar).Value = Modal.EquipmentComment != null ? Modal.EquipmentComment : "";
            command.Parameters.Add("@OilywaterseparatorComment", SqlDbType.NVarChar).Value = Modal.OilywaterseparatorComment != null ? Modal.OilywaterseparatorComment : "";
            command.Parameters.Add("@FueloiltransferplanComment", SqlDbType.NVarChar).Value = Modal.FueloiltransferplanComment != null ? Modal.FueloiltransferplanComment : "";
            command.Parameters.Add("@SteeringgearComment", SqlDbType.NVarChar).Value = Modal.SteeringgearComment != null ? Modal.SteeringgearComment : "";
            command.Parameters.Add("@WorkshopandequipmentComment", SqlDbType.NVarChar).Value = Modal.WorkshopandequipmentComment != null ? Modal.WorkshopandequipmentComment : "";
            command.Parameters.Add("@SoundingpipesComment", SqlDbType.NVarChar).Value = Modal.SoundingpipesComment != null ? Modal.SoundingpipesComment : "";
            command.Parameters.Add("@EnginecontrolComment", SqlDbType.NVarChar).Value = Modal.EnginecontrolComment != null ? Modal.EnginecontrolComment : "";
            command.Parameters.Add("@ChiefEngineernightordersbookComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineernightordersbookComment != null ? Modal.ChiefEngineernightordersbookComment : "";
            command.Parameters.Add("@ChiefEngineerstandingordersComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineerstandingordersComment != null ? Modal.ChiefEngineerstandingordersComment : "";
            command.Parameters.Add("@PreUMSComment", SqlDbType.NVarChar).Value = Modal.PreUMSComment != null ? Modal.PreUMSComment : "";
            command.Parameters.Add("@EnginebellbookComment", SqlDbType.NVarChar).Value = Modal.EnginebellbookComment != null ? Modal.EnginebellbookComment : "";
            command.Parameters.Add("@LockoutComment", SqlDbType.NVarChar).Value = Modal.LockoutComment != null ? Modal.LockoutComment : "";
            command.Parameters.Add("@EngineRoomComment", SqlDbType.NVarChar).Value = Modal.EngineRoomComment != null ? Modal.EngineRoomComment : "";


            command.Parameters.Add("@CleanlinessandhygieneComment", SqlDbType.NVarChar).Value = Modal.CleanlinessandhygieneComment != null ? Modal.CleanlinessandhygieneComment : "";
            command.Parameters.Add("@ConditionComment5M", SqlDbType.NVarChar).Value = Modal.ConditionComment5M != null ? Modal.ConditionComment5M : "";
            command.Parameters.Add("@PaintworkComment5M", SqlDbType.NVarChar).Value = Modal.PaintworkComment5M != null ? Modal.PaintworkComment5M : "";
            command.Parameters.Add("@SignalmastandstaysComment", SqlDbType.NVarChar).Value = Modal.SignalmastandstaysComment != null ? Modal.SignalmastandstaysComment : "";
            command.Parameters.Add("@MonkeyislandComment", SqlDbType.NVarChar).Value = Modal.MonkeyislandComment != null ? Modal.MonkeyislandComment : "";
            command.Parameters.Add("@FireDampersComment", SqlDbType.NVarChar).Value = Modal.FireDampersComment != null ? Modal.FireDampersComment : "";
            command.Parameters.Add("@RailsBulwarksComment", SqlDbType.NVarChar).Value = Modal.RailsBulwarksComment != null ? Modal.RailsBulwarksComment : "";
            command.Parameters.Add("@WatertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WatertightdoorsComment != null ? Modal.WatertightdoorsComment : "";
            command.Parameters.Add("@VentilatorsComment", SqlDbType.NVarChar).Value = Modal.VentilatorsComment != null ? Modal.VentilatorsComment : "";
            command.Parameters.Add("@WinchesComment", SqlDbType.NVarChar).Value = Modal.WinchesComment != null ? Modal.WinchesComment : "";
            command.Parameters.Add("@FairleadsComment", SqlDbType.NVarChar).Value = Modal.FairleadsComment != null ? Modal.FairleadsComment : "";
            command.Parameters.Add("@MooringLinesComment", SqlDbType.NVarChar).Value = Modal.MooringLinesComment != null ? Modal.MooringLinesComment : "";
            command.Parameters.Add("@EmergencyShutOffsComment", SqlDbType.NVarChar).Value = Modal.EmergencyShutOffsComment != null ? Modal.EmergencyShutOffsComment : "";
            command.Parameters.Add("@RadioaerialsComment", SqlDbType.NVarChar).Value = Modal.RadioaerialsComment != null ? Modal.RadioaerialsComment : "";
            command.Parameters.Add("@SOPEPlockerComment", SqlDbType.NVarChar).Value = Modal.SOPEPlockerComment != null ? Modal.SOPEPlockerComment : "";
            command.Parameters.Add("@ChemicallockerComment", SqlDbType.NVarChar).Value = Modal.ChemicallockerComment != null ? Modal.ChemicallockerComment : "";
            command.Parameters.Add("@AntislippaintComment", SqlDbType.NVarChar).Value = Modal.AntislippaintComment != null ? Modal.AntislippaintComment : "";
            command.Parameters.Add("@SuperstructureComment", SqlDbType.NVarChar).Value = Modal.SuperstructureComment != null ? Modal.SuperstructureComment : "";


            command.Parameters.Add("@CabinsComment", SqlDbType.NVarChar).Value = Modal.CabinsComment != null ? Modal.CabinsComment : "";
            command.Parameters.Add("@OfficesComment", SqlDbType.NVarChar).Value = Modal.OfficesComment != null ? Modal.OfficesComment : "";
            command.Parameters.Add("@MessroomsComment", SqlDbType.NVarChar).Value = Modal.MessroomsComment != null ? Modal.MessroomsComment : "";
            command.Parameters.Add("@ToiletsComment", SqlDbType.NVarChar).Value = Modal.ToiletsComment != null ? Modal.ToiletsComment : "";
            command.Parameters.Add("@LaundryroomComment", SqlDbType.NVarChar).Value = Modal.LaundryroomComment != null ? Modal.LaundryroomComment : "";
            command.Parameters.Add("@ChangingroomComment", SqlDbType.NVarChar).Value = Modal.ChangingroomComment != null ? Modal.ChangingroomComment : "";
            command.Parameters.Add("@OtherComment", SqlDbType.NVarChar).Value = Modal.OtherComment != null ? Modal.OtherComment : "";
            command.Parameters.Add("@ConditionComment5N", SqlDbType.NVarChar).Value = Modal.ConditionComment5N != null ? Modal.ConditionComment5N : "";
            command.Parameters.Add("@SelfclosingfiredoorsComment", SqlDbType.NVarChar).Value = Modal.SelfclosingfiredoorsComment != null ? Modal.SelfclosingfiredoorsComment : "";
            command.Parameters.Add("@StairwellsComment", SqlDbType.NVarChar).Value = Modal.StairwellsComment != null ? Modal.StairwellsComment : "";
            command.Parameters.Add("@SuperstructureInternalComment", SqlDbType.NVarChar).Value = Modal.SuperstructureInternalComment != null ? Modal.SuperstructureInternalComment : "";

            command.Parameters.Add("@PortablegangwayComment", SqlDbType.NVarChar).Value = Modal.PortablegangwayComment != null ? Modal.PortablegangwayComment : "";
            command.Parameters.Add("@SafetynetComment", SqlDbType.NVarChar).Value = Modal.SafetynetComment != null ? Modal.SafetynetComment : "";
            command.Parameters.Add("@AccommodationLadderComment", SqlDbType.NVarChar).Value = Modal.AccommodationLadderComment != null ? Modal.AccommodationLadderComment : "";
            command.Parameters.Add("@SafeaccessprovidedComment", SqlDbType.NVarChar).Value = Modal.SafeaccessprovidedComment != null ? Modal.SafeaccessprovidedComment : "";
            command.Parameters.Add("@PilotladdersComment", SqlDbType.NVarChar).Value = Modal.PilotladdersComment != null ? Modal.PilotladdersComment : "";
            command.Parameters.Add("@BoardingEquipmentComment", SqlDbType.NVarChar).Value = Modal.BoardingEquipmentComment != null ? Modal.BoardingEquipmentComment : "";
            command.Parameters.Add("@CleanlinessComment", SqlDbType.NVarChar).Value = Modal.CleanlinessComment != null ? Modal.CleanlinessComment : "";
            command.Parameters.Add("@PaintworkComment5P", SqlDbType.NVarChar).Value = Modal.PaintworkComment5P != null ? Modal.PaintworkComment5P : "";
            command.Parameters.Add("@ShipsiderailsComment", SqlDbType.NVarChar).Value = Modal.ShipsiderailsComment != null ? Modal.ShipsiderailsComment : "";
            command.Parameters.Add("@WeathertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WeathertightdoorsComment != null ? Modal.WeathertightdoorsComment : "";
            command.Parameters.Add("@FirehydrantsComment", SqlDbType.NVarChar).Value = Modal.FirehydrantsComment != null ? Modal.FirehydrantsComment : "";
            command.Parameters.Add("@VentilatorsComment5P", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5P != null ? Modal.VentilatorsComment5P : "";
            command.Parameters.Add("@ManholecoversComment", SqlDbType.NVarChar).Value = Modal.ManholecoversComment != null ? Modal.ManholecoversComment : "";
            command.Parameters.Add("@MainDeckAreaComment", SqlDbType.NVarChar).Value = Modal.MainDeckAreaComment != null ? Modal.MainDeckAreaComment : "";


            command.Parameters.Add("@ConditionComment5Q", SqlDbType.NVarChar).Value = Modal.ConditionComment5Q != null ? Modal.ConditionComment5Q : "";
            command.Parameters.Add("@PaintworkComment5Q", SqlDbType.NVarChar).Value = Modal.PaintworkComment5Q != null ? Modal.PaintworkComment5Q : "";
            command.Parameters.Add("@MechanicaldamageComment", SqlDbType.NVarChar).Value = Modal.MechanicaldamageComment != null ? Modal.MechanicaldamageComment : "";
            command.Parameters.Add("@AccessladdersComment", SqlDbType.NVarChar).Value = Modal.AccessladdersComment != null ? Modal.AccessladdersComment : "";
            command.Parameters.Add("@ManholecoversComment5Q", SqlDbType.NVarChar).Value = Modal.ManholecoversComment5Q != null ? Modal.ManholecoversComment5Q : "";
            command.Parameters.Add("@HoldbilgeComment", SqlDbType.NVarChar).Value = Modal.HoldbilgeComment != null ? Modal.HoldbilgeComment : "";
            command.Parameters.Add("@AccessdoorsComment", SqlDbType.NVarChar).Value = Modal.AccessdoorsComment != null ? Modal.AccessdoorsComment : "";
            command.Parameters.Add("@ConditionHatchCoversComment", SqlDbType.NVarChar).Value = Modal.ConditionHatchCoversComment != null ? Modal.ConditionHatchCoversComment : "";
            command.Parameters.Add("@PaintworkHatchCoversComment", SqlDbType.NVarChar).Value = Modal.PaintworkHatchCoversComment != null ? Modal.PaintworkHatchCoversComment : "";
            command.Parameters.Add("@RubbersealsComment", SqlDbType.NVarChar).Value = Modal.RubbersealsComment != null ? Modal.RubbersealsComment : "";
            command.Parameters.Add("@SignsofhatchesComment", SqlDbType.NVarChar).Value = Modal.SignsofhatchesComment != null ? Modal.SignsofhatchesComment : "";
            command.Parameters.Add("@SealingtapeComment", SqlDbType.NVarChar).Value = Modal.SealingtapeComment != null ? Modal.SealingtapeComment : "";
            command.Parameters.Add("@ConditionofhydraulicsComment", SqlDbType.NVarChar).Value = Modal.ConditionofhydraulicsComment != null ? Modal.ConditionofhydraulicsComment : "";
            command.Parameters.Add("@PortablebulkheadsComment", SqlDbType.NVarChar).Value = Modal.PortablebulkheadsComment != null ? Modal.PortablebulkheadsComment : "";
            command.Parameters.Add("@TweendecksComment", SqlDbType.NVarChar).Value = Modal.TweendecksComment != null ? Modal.TweendecksComment : "";
            command.Parameters.Add("@HatchcoamingComment", SqlDbType.NVarChar).Value = Modal.HatchcoamingComment != null ? Modal.HatchcoamingComment : "";
            command.Parameters.Add("@ConditionCargoCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionCargoCranesComment != null ? Modal.ConditionCargoCranesComment : "";
            command.Parameters.Add("@GantrycranealarmComment", SqlDbType.NVarChar).Value = Modal.GantrycranealarmComment != null ? Modal.GantrycranealarmComment : "";
            command.Parameters.Add("@GantryconditionComment", SqlDbType.NVarChar).Value = Modal.GantryconditionComment != null ? Modal.GantryconditionComment : "";
            command.Parameters.Add("@CargoHoldsComment", SqlDbType.NVarChar).Value = Modal.CargoHoldsComment != null ? Modal.CargoHoldsComment : "";


            command.Parameters.Add("@CleanlinessComment5R", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5R != null ? Modal.CleanlinessComment5R : "";
            command.Parameters.Add("@PaintworkComment5R", SqlDbType.NVarChar).Value = Modal.PaintworkComment5R != null ? Modal.PaintworkComment5R : "";
            command.Parameters.Add("@TriphazardsComment", SqlDbType.NVarChar).Value = Modal.TriphazardsComment != null ? Modal.TriphazardsComment : "";
            command.Parameters.Add("@WindlassComment", SqlDbType.NVarChar).Value = Modal.WindlassComment != null ? Modal.WindlassComment : "";
            command.Parameters.Add("@CablesComment", SqlDbType.NVarChar).Value = Modal.CablesComment != null ? Modal.CablesComment : "";
            command.Parameters.Add("@WinchesComment5R", SqlDbType.NVarChar).Value = Modal.WinchesComment5R != null ? Modal.WinchesComment5R : "";
            command.Parameters.Add("@FairleadsComment5R", SqlDbType.NVarChar).Value = Modal.FairleadsComment5R != null ? Modal.FairleadsComment5R : "";
            command.Parameters.Add("@MooringComment", SqlDbType.NVarChar).Value = Modal.MooringComment != null ? Modal.MooringComment : "";
            command.Parameters.Add("@HatchToforecastlespaceComment", SqlDbType.NVarChar).Value = Modal.HatchToforecastlespaceComment != null ? Modal.HatchToforecastlespaceComment : "";
            command.Parameters.Add("@VentilatorsComment5R", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5R != null ? Modal.VentilatorsComment5R : "";
            command.Parameters.Add("@BellComment", SqlDbType.NVarChar).Value = Modal.BellComment != null ? Modal.BellComment : "";
            command.Parameters.Add("@ForemastComment", SqlDbType.NVarChar).Value = Modal.ForemastComment != null ? Modal.ForemastComment : "";
            command.Parameters.Add("@FireComment", SqlDbType.NVarChar).Value = Modal.FireComment != null ? Modal.FireComment : "";
            command.Parameters.Add("@RailsComment", SqlDbType.NVarChar).Value = Modal.RailsComment != null ? Modal.RailsComment : "";
            command.Parameters.Add("@AntislippaintComment5R", SqlDbType.NVarChar).Value = Modal.AntislippaintComment5R != null ? Modal.AntislippaintComment5R : "";

            command.Parameters.Add("@ForecastleComment", SqlDbType.NVarChar).Value = Modal.ForecastleComment != null ? Modal.ForecastleComment : "";
            command.Parameters.Add("@CleanlinessComment5S", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5S != null ? Modal.CleanlinessComment5S : "";
            command.Parameters.Add("@PaintworkComment5S", SqlDbType.NVarChar).Value = Modal.PaintworkComment5S != null ? Modal.PaintworkComment5S : "";
            command.Parameters.Add("@ForepeakComment", SqlDbType.NVarChar).Value = Modal.ForepeakComment != null ? Modal.ForepeakComment : "";
            command.Parameters.Add("@ChainlockerComment", SqlDbType.NVarChar).Value = Modal.ChainlockerComment != null ? Modal.ChainlockerComment : "";
            command.Parameters.Add("@LightingComment5S", SqlDbType.NVarChar).Value = Modal.LightingComment5S != null ? Modal.LightingComment5S : "";
            command.Parameters.Add("@AccesssafetychainComment", SqlDbType.NVarChar).Value = Modal.AccesssafetychainComment != null ? Modal.AccesssafetychainComment : "";
            command.Parameters.Add("@EmergencyfirepumpComment", SqlDbType.NVarChar).Value = Modal.EmergencyfirepumpComment != null ? Modal.EmergencyfirepumpComment : "";
            command.Parameters.Add("@BowthrusterandroomComment", SqlDbType.NVarChar).Value = Modal.BowthrusterandroomComment != null ? Modal.BowthrusterandroomComment : "";
            command.Parameters.Add("@SparemooringlinesComment", SqlDbType.NVarChar).Value = Modal.SparemooringlinesComment != null ? Modal.SparemooringlinesComment : "";
            command.Parameters.Add("@PaintlockerComment", SqlDbType.NVarChar).Value = Modal.PaintlockerComment != null ? Modal.PaintlockerComment : "";
            command.Parameters.Add("@ForecastleSpaceComment", SqlDbType.NVarChar).Value = Modal.ForecastleSpaceComment != null ? Modal.ForecastleSpaceComment : "";
            command.Parameters.Add("@BoottopComment", SqlDbType.NVarChar).Value = Modal.BoottopComment != null ? Modal.BoottopComment : "";
            command.Parameters.Add("@TopsidesComment", SqlDbType.NVarChar).Value = Modal.TopsidesComment != null ? Modal.TopsidesComment : "";
            command.Parameters.Add("@AntifoulingComment", SqlDbType.NVarChar).Value = Modal.AntifoulingComment != null ? Modal.AntifoulingComment : "";
            command.Parameters.Add("@DraftandplimsollComment", SqlDbType.NVarChar).Value = Modal.DraftandplimsollComment != null ? Modal.DraftandplimsollComment : "";
            command.Parameters.Add("@FoulingComment", SqlDbType.NVarChar).Value = Modal.FoulingComment != null ? Modal.FoulingComment : "";
            command.Parameters.Add("@MechanicalComment", SqlDbType.NVarChar).Value = Modal.MechanicalComment != null ? Modal.MechanicalComment : "";
            command.Parameters.Add("@HullComment", SqlDbType.NVarChar).Value = Modal.HullComment != null ? Modal.HullComment : "";
            command.Parameters.Add("@SummaryComment", SqlDbType.NVarChar).Value = Modal.SummaryComment != null ? Modal.SummaryComment : "";

            command.Parameters.Add("@IsSynced", SqlDbType.NVarChar).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate == null ? DBNull.Value : (object)Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate == null ? DBNull.Value : (object)Modal.UpdatedDate;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft == null ? DBNull.Value : (object)Modal.SavedAsDraft;

            command.Parameters.Add("@SnapBackZoneComment", SqlDbType.NVarChar).Value = Modal.SnapBackZoneComment != null ? Modal.SnapBackZoneComment : "";
            command.Parameters.Add("@ConditionGantryCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionGantryCranesComment != null ? Modal.ConditionGantryCranesComment : "";
            command.Parameters.Add("@MedicalLogBookComment", SqlDbType.NVarChar).Value = Modal.MedicalLogBookComment != null ? Modal.MedicalLogBookComment : "";
            command.Parameters.Add("@DrugsNarcoticsComment", SqlDbType.NVarChar).Value = Modal.DrugsNarcoticsComment != null ? Modal.DrugsNarcoticsComment : "";
            command.Parameters.Add("@DefibrillatorComment", SqlDbType.NVarChar).Value = Modal.DefibrillatorComment != null ? Modal.DefibrillatorComment : "";
            command.Parameters.Add("@RPWaterHandbook", SqlDbType.NVarChar).Value = Modal.RPWaterHandbook != null ? Modal.RPWaterHandbook : "";
            command.Parameters.Add("@BioRPWH", SqlDbType.NVarChar).Value = Modal.BioRPWH != null ? Modal.BioRPWH : "";
            command.Parameters.Add("@PRE", SqlDbType.NVarChar).Value = Modal.PRE != null ? Modal.PRE : "";
            command.Parameters.Add("@NoiseVibrationFile", SqlDbType.NVarChar).Value = Modal.NoiseVibrationFile != null ? Modal.NoiseVibrationFile : "";
            command.Parameters.Add("@BioMPR", SqlDbType.NVarChar).Value = Modal.BioMPR != null ? Modal.BioMPR : "";
            command.Parameters.Add("@AsbestosPlan", SqlDbType.NVarChar).Value = Modal.AsbestosPlan != null ? Modal.AsbestosPlan : "";
            command.Parameters.Add("@ShipPublicAddrComment", SqlDbType.NVarChar).Value = Modal.ShipPublicAddrComment != null ? Modal.ShipPublicAddrComment : "";
            command.Parameters.Add("@BridgewindowswiperssprayComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswiperssprayComment != null ? Modal.BridgewindowswiperssprayComment : "";
            command.Parameters.Add("@BridgewindowswipersComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswipersComment != null ? Modal.BridgewindowswipersComment : "";
            command.Parameters.Add("@DaylightSignalsComment", SqlDbType.NVarChar).Value = Modal.DaylightSignalsComment != null ? Modal.DaylightSignalsComment : "";
            command.Parameters.Add("@LiferaftDavitComment", SqlDbType.NVarChar).Value = Modal.LiferaftDavitComment != null ? Modal.LiferaftDavitComment : "";
            command.Parameters.Add("@CylindersLockerComment", SqlDbType.NVarChar).Value = Modal.CylindersLockerComment != null ? Modal.CylindersLockerComment : "";
            command.Parameters.Add("@SnapBackZone5NComment", SqlDbType.NVarChar).Value = Modal.SnapBackZone5NComment != null ? Modal.SnapBackZone5NComment : "";
            command.Parameters.Add("@ADPPublicationsComment", SqlDbType.NVarChar).Value = Modal.ADPPublicationsComment != null ? Modal.ADPPublicationsComment : "";

            //RDBJ 10/19/2021
            command.Parameters.Add("@IsGeneralSectionComplete", SqlDbType.Bit).Value = Modal.IsGeneralSectionComplete == null ? DBNull.Value : (object)Modal.IsGeneralSectionComplete;
            command.Parameters.Add("@IsManningSectionComplete", SqlDbType.Bit).Value = Modal.IsManningSectionComplete == null ? DBNull.Value : (object)Modal.IsManningSectionComplete;
            command.Parameters.Add("@IsShipCertificationSectionComplete", SqlDbType.Bit).Value = Modal.IsShipCertificationSectionComplete == null ? DBNull.Value : (object)Modal.IsShipCertificationSectionComplete;
            command.Parameters.Add("@IsRecordKeepingSectionComplete", SqlDbType.Bit).Value = Modal.IsRecordKeepingSectionComplete == null ? DBNull.Value : (object)Modal.IsRecordKeepingSectionComplete;
            command.Parameters.Add("@IsSafetyEquipmentSectionComplete", SqlDbType.Bit).Value = Modal.IsSafetyEquipmentSectionComplete == null ? DBNull.Value : (object)Modal.IsSafetyEquipmentSectionComplete;
            command.Parameters.Add("@IsSecuritySectionComplete", SqlDbType.Bit).Value = Modal.IsSecuritySectionComplete == null ? DBNull.Value : (object)Modal.IsSecuritySectionComplete;
            command.Parameters.Add("@IsBridgeSectionComplete", SqlDbType.Bit).Value = Modal.IsBridgeSectionComplete == null ? DBNull.Value : (object)Modal.IsBridgeSectionComplete;
            command.Parameters.Add("@IsMedicalSectionComplete", SqlDbType.Bit).Value = Modal.IsMedicalSectionComplete == null ? DBNull.Value : (object)Modal.IsMedicalSectionComplete;
            command.Parameters.Add("@IsGalleySectionComplete", SqlDbType.Bit).Value = Modal.IsGalleySectionComplete == null ? DBNull.Value : (object)Modal.IsGalleySectionComplete;
            command.Parameters.Add("@IsEngineRoomSectionComplete", SqlDbType.Bit).Value = Modal.IsEngineRoomSectionComplete == null ? DBNull.Value : (object)Modal.IsEngineRoomSectionComplete;
            command.Parameters.Add("@IsSuperstructureSectionComplete", SqlDbType.Bit).Value = Modal.IsSuperstructureSectionComplete == null ? DBNull.Value : (object)Modal.IsSuperstructureSectionComplete;
            command.Parameters.Add("@IsDeckSectionComplete", SqlDbType.Bit).Value = Modal.IsDeckSectionComplete == null ? DBNull.Value : (object)Modal.IsDeckSectionComplete;
            command.Parameters.Add("@IsHoldsAndCoverSectionComplete", SqlDbType.Bit).Value = Modal.IsHoldsAndCoverSectionComplete == null ? DBNull.Value : (object)Modal.IsHoldsAndCoverSectionComplete;
            command.Parameters.Add("@IsForeCastleSectionComplete", SqlDbType.Bit).Value = Modal.IsForeCastleSectionComplete == null ? DBNull.Value : (object)Modal.IsForeCastleSectionComplete;
            command.Parameters.Add("@IsHullSectionComplete", SqlDbType.Bit).Value = Modal.IsHullSectionComplete == null ? DBNull.Value : (object)Modal.IsHullSectionComplete;
            command.Parameters.Add("@IsSummarySectionComplete", SqlDbType.Bit).Value = Modal.IsSummarySectionComplete == null ? DBNull.Value : (object)Modal.IsSummarySectionComplete;
            command.Parameters.Add("@IsDeficienciesSectionComplete", SqlDbType.Bit).Value = Modal.IsDeficienciesSectionComplete == null ? DBNull.Value : (object)Modal.IsDeficienciesSectionComplete;
            command.Parameters.Add("@IsPhotographsSectionComplete", SqlDbType.Bit).Value = Modal.IsPhotographsSectionComplete == null ? DBNull.Value : (object)Modal.IsPhotographsSectionComplete;
            //ENd RDBJ 10/19/2021
        }
        public bool SaveGlRSafeManningRequirementsDataInLocalDB(List<GlRSafeManningRequirements> GlRSafeManningRequirements, long GIRFormID)
        {
            bool res = false;
            try
            {
                if (GlRSafeManningRequirements != null && GlRSafeManningRequirements.Count > 0 && GIRFormID > 0)
                {
                    foreach (var item in GlRSafeManningRequirements)
                    {
                        item.GIRFormID = GIRFormID;
                        item.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        item.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRSafeManningRequirements);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRSafeManningRequirements); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(GlRSafeManningRequirements);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.GIRSafeManningRequirements;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveGlRCrewDocumentsDataInLocalDB(List<GIRCrewDocuments> GlRCrewDocuments, long GIRFormID)
        {
            bool res = false;
            try
            {
                if (GlRCrewDocuments != null && GlRCrewDocuments.Count > 0 && GIRFormID > 0)
                {
                    foreach (var item in GlRCrewDocuments)
                    {
                        item.CrewDocumentsID = 0;
                        item.GIRFormID = GIRFormID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRCrewDocuments);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRCrewDocuments); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(GlRCrewDocuments);
                            if (dt.Columns["Ship"].Ordinal != 6)
                                dt.Columns["Ship"].SetOrdinal(6);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.GIRCrewDocuments;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveGIRRestandWorkHoursDataInLocalDB(List<GIRRestandWorkHours> GIRRestandWorkHours, long GIRFormID)
        {
            bool res = false;
            try
            {
                if (GIRRestandWorkHours != null && GIRRestandWorkHours.Count > 0 && GIRFormID > 0)
                {
                    foreach (var item in GIRRestandWorkHours)
                    {
                        item.GIRFormID = GIRFormID;
                        item.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        item.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRRestandWorkHours);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRRestandWorkHours); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(GIRRestandWorkHours);
                            if (dt.Columns["Ship"].Ordinal != 6)
                                dt.Columns["Ship"].SetOrdinal(6);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.GIRRestandWorkHours;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveGIRDeficienciesDataInLocalDB(List<GIRDeficiencies> GIRDeficiencies, long GIRFormID)
        {
            bool res = false;
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0 && GIRFormID > 0)
                {
                    foreach (var item in GIRDeficiencies)
                    {
                        item.GIRFormID = GIRFormID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficiencies);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficiencies); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            foreach (var item in GIRDeficiencies)
                            {
                                List<GIRDeficiencies> _obj = new List<Modals.GIRDeficiencies>();
                                _obj.Add(item);
                                DataTable dt = Utility.ToDataTable(_obj);
                                if (dt.Columns.Contains("GIRDeficienciesFile"))
                                {
                                    dt.Columns.Remove("GIRDeficienciesFile");
                                    dt.AcceptChanges();
                                }
                                if (!dt.Columns.Contains("FileName"))
                                    dt.Columns.Add("FileName");
                                if (!dt.Columns.Contains("StorePath"))
                                    dt.Columns.Add("StorePath");
                                SqlConnection connection = new SqlConnection(ConnectionString);
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                                bulkCopy.DestinationTableName = AppStatic.GIRDeficiencies;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                DataTable dt1 = new DataTable();
                                SqlDataAdapter sqlAdp = new SqlDataAdapter("select top 1 * from " + AppStatic.GIRDeficiencies + " where [GIRFormID]=" + item.GIRFormID + " order by [DeficienciesID] desc", connection);
                                sqlAdp.Fill(dt1);
                                connection.Close();
                                if (dt != null && dt.Rows.Count > 0)
                                    item.DeficienciesID = Convert.ToInt32(dt1.Rows[0]["DeficienciesID"]);
                                res = true;
                                if (res == true)
                                {
                                    res = SaveGIRDeficienciesFile(item.GIRDeficienciesFile, item.DeficienciesID);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveGIRDeficienciesFile(List<GIRDeficienciesFile> GIRDeficienciesFile, long DeficienciesID)
        {
            bool res = false;
            try
            {
                if (DeficienciesID > 0 && GIRDeficienciesFile != null && GIRDeficienciesFile.Count > 0)
                {

                    foreach (var GIRFileitem in GIRDeficienciesFile)
                    {
                        GIRFileitem.DeficienciesID = DeficienciesID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesFiles);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesFiles); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(GIRDeficienciesFile);
                            if (dt.Columns.Contains("IsUpload"))
                            {
                                dt.Columns.Remove("IsUpload");
                                dt.AcceptChanges();
                            }
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                            bulkCopy.DestinationTableName = AppStatic.GIRDeficienciesFiles;
                            connection.Open();
                            bulkCopy.WriteToServer(dt);
                            connection.Close();
                            res = true;
                        }

                    }

                }

            }
            catch (Exception ex)
            {

                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveGIRPhotographsDataInLocalDB(List<GIRPhotographs> GIRPhotographs, Guid UniqueFormID)
        {
            bool res = false;
            try
            {
                if (GIRPhotographs != null && GIRPhotographs.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in GIRPhotographs)
                    {
                        //item.GIRFormID = GIRFormID;
                        item.UniqueFormID = Guid.Parse(UniqueFormID.ToString());
                        item.CreatedDate = DateTime.Parse(Utility.ToDateTimeUtcNow().ToString("dd/MM/yyyy")); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        item.UpdatedDate = DateTime.Parse(Utility.ToDateTimeUtcNow().ToString("dd/MM/yyyy")); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRPhotographs);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRPhotographs); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(GIRPhotographs);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.GIRPhotographs;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SaveGIRPhotographsDataInLocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveGIRDeficienciesFileDataInLocalDB(List<GIRDeficienciesFile> GIRDeficienciesFile, int DeficienciesID)
        {
            bool res = false;
            try
            {
                if (GIRDeficienciesFile != null && GIRDeficienciesFile.Count > 0 && DeficienciesID > 0)
                {
                    foreach (var item in GIRDeficienciesFile)
                    {
                        item.DeficienciesID = DeficienciesID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesFiles);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesFiles); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(GIRDeficienciesFile);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.GIRDeficienciesFiles;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveDeficienciesNoteDataInLocalDB(List<GIRDeficienciesNote> DeficienciesNote, long GIRFormID)
        {
            bool res = false;
            try
            {
                if (DeficienciesNote != null && DeficienciesNote.Count > 0 && GIRFormID > 0)
                {
                    foreach (var item in DeficienciesNote)
                    {
                        item.GIRFormID = GIRFormID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesNote);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesNote); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(DeficienciesNote);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.GIRDeficienciesNote;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public bool SaveGIRDeficienciesCommentFileDataInLocalDB(List<GIRDeficienciesCommentFile> GIRDeficienciesCommentFile, long DeficienciesID)
        {
            bool res = false;
            try
            {
                if (GIRDeficienciesCommentFile != null && GIRDeficienciesCommentFile.Count > 0 && DeficienciesID > 0)
                {
                    foreach (var item in GIRDeficienciesCommentFile)
                    {
                        item.DeficienciesID = DeficienciesID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesCommentFile);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesCommentFile); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(GIRDeficienciesCommentFile);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.GIRDeficienciesCommentFile;
                                connection.Open();
                                bulkCopy.WriteToServer(dt);
                                connection.Close();
                                res = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string AutoSaveGIRDataInLocalDB(GeneralInspectionReport Modal)
        {
            string UniqueFormID = string.Empty;
            try
            {
                Modal.IsSynced = false; //RDBJ 10/13/2021 Set at global
                if (Modal.UniqueFormID != Guid.Empty)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    //AlterGeneralInspectionReportTable(); //RDBJ 10/19/2021
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string UpdateQury = GETGIRUpdateQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(UpdateQury, connection);

                        Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.UniqueFormID = Modal.UniqueFormID;
                        GIRUpdateCMD(Modal, ref command);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        UniqueFormID = Modal.UniqueFormID.ToString();
                    }
                }
                else
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GeneralInspectionReport);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GeneralInspectionReport); }
                    if (isTbaleCreated)
                    {
                        //AlterGeneralInspectionReportTable(); //RDBJ 10/19/2021 wrapped in this function
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                            SqlConnection connection = new SqlConnection(connetionString);
                            string InsertQury = GeneralInspectionReportDataInsertQuery();
                            SqlCommand command = new SqlCommand(InsertQury, connection);

                            Guid FormGUID = Guid.NewGuid();
                            Modal.UniqueFormID = FormGUID;
                            Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            GeneralInspectionReportDataInsertCMD(Modal, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();
                            UniqueFormID = FormGUID.ToString();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(UniqueFormID))
                {
                    if (Modal.Manning_SafeMiningChanged == true)
                    {
                        GIRSafeManningRequirements_Save(Modal.UniqueFormID, Modal.GIRSafeManningRequirements);
                    }
                    if (Modal.Manning_CrewDocsChanged == true)
                    {
                        GIRCrewDocuments_Save(Modal.UniqueFormID, Modal.GIRCrewDocuments);
                    }
                    if (Modal.Manning_RestAndWorkChanged == true)
                    {
                        GIRRestandWorkHours_Save(Modal.UniqueFormID, Modal.GIRRestandWorkHours);
                    }
                    if (Modal.Manning_DeficienciesChanged == true)
                    {
                        GIRDeficiencies_Save(Modal.UniqueFormID, Modal.GIRDeficiencies);
                    }
                    if (Modal.Manning_PhotosChanged == true)
                    {
                        GIRPhotos_Save(Modal.UniqueFormID, Modal.GIRPhotographs);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GeneralInspectionReport table Error : " + ex.Message.ToString());
            }
            return UniqueFormID;
        }
        public string GETGIRUpdateQuery()
        {
            //RDBJ 10/19/2021 Added 
            /*
            IsGeneralSectionComplete,IsManningSectionComplete,IsShipCertificationSectionComplete,IsRecordKeepingSectionComplete,
            IsSafetyEquipmentSectionComplete,IsSecuritySectionComplete,IsBridgeSectionComplete,IsMedicalSectionComplete,IsGalleySectionComplete,
            IsEngineRoomSectionComplete,IsSuperstructureSectionComplete,IsDeckSectionComplete,IsHoldsAndCoverSectionComplete,IsForeCastleSectionComplete,
            IsHullSectionComplete,IsSummarySectionComplete,IsDeficienciesSectionComplete,IsPhotographsSectionComplete
             */
            string query = @"UPDATE dbo.GeneralInspectionReport SET ShipID = @ShipID, FormVersion = @FormVersion,
                           ShipName = @ShipName, Ship = @Ship, Port = @Port, Inspector = @Inspector, Date = @Date, GeneralPreamble = @GeneralPreamble, 
                           Classsociety = @Classsociety, YearofBuild = @YearofBuild, Flag = @Flag, Classofvessel = @Classofvessel, Portofregistry = @Portofregistry, 
                           MMSI = @MMSI, IMOnumber = @IMOnumber, Callsign = @Callsign, SummerDWT = @SummerDWT, Grosstonnage = @Grosstonnage, Lightweight = @Lightweight, 
                           Nettonnage = @Nettonnage, Beam = @Beam, LOA = @LOA, Summerdraft = @Summerdraft, LBP = @LBP, Bowthruster = @Bowthruster, BHP = @BHP, 
                           Noofholds = @Noofholds, Nomoveablebulkheads = @Nomoveablebulkheads, Containers = @Containers, Cargocapacity = @Cargocapacity, 
                           Cargohandlingequipment = @Cargohandlingequipment, Lastvoyageandcargo = @Lastvoyageandcargo, CurrentPlannedvoyageandcargo = @CurrentPlannedvoyageandcargo, 
                           ShipboardWorkingArrangements = @ShipboardWorkingArrangements, CertificationIndex = @CertificationIndex, 
                           CarriedOutByTheDOOW = @CarriedOutByTheDOOW, IsRegs4shipsDVD = @IsRegs4shipsDVD, Regs4shipsDVD = @Regs4shipsDVD, IsSOPEPPoints = @IsSOPEPPoints, 
                           SOPEPPoints = @SOPEPPoints, IsBWMP = @IsBWMP, BWMP = @BWMP, IsBWMPSupplement = @IsBWMPSupplement, BWMPSupplement = @BWMPSupplement, 
                           IsIntactStabilityManual = @IsIntactStabilityManual, IntactStabilityManual = @IntactStabilityManual, 
                           IsStabilityComputer = @IsStabilityComputer, StabilityComputer = @StabilityComputer, IsDateOfLast = @IsDateOfLast, DateOfLast = @DateOfLast, 
                           IsCargoSecuring = @IsCargoSecuring, CargoSecuring = @CargoSecuring, IsBulkCargo = @IsBulkCargo, 
                           BulkCargo = @BulkCargo, IsSMSManual = @IsSMSManual, SMSManual = @SMSManual, IsRegisterOf = @IsRegisterOf, RegisterOf = @RegisterOf, 
                           IsFleetStandingOrder = @IsFleetStandingOrder, FleetStandingOrder = @FleetStandingOrder, 
                           IsFleetMemoranda = @IsFleetMemoranda, FleetMemoranda = @FleetMemoranda, IsShipsPlans = @IsShipsPlans, ShipsPlans = @ShipsPlans, 
                           IsCollective = @IsCollective, Collective = @Collective, IsDraftAndFreeboardNotice = @IsDraftAndFreeboardNotice, 
                           DraftAndFreeboardNotice = @DraftAndFreeboardNotice, IsPCSOPEP = @IsPCSOPEP, PCSOPEP = @PCSOPEP, IsNTVRP = @IsNTVRP, NTVRP = @NTVRP, 
                           IsVGP = @IsVGP, VGP = @VGP,PubsComments = @PubsComments, IsPubsAndDocsSectionComplete = @IsPubsAndDocsSectionComplete, 
                           OfficialLogbookA = @OfficialLogbookA, OfficialLogbookB = @OfficialLogbookB, OfficialLogbookC = @OfficialLogbookC, OfficialLogbookD = @OfficialLogbookD, OfficialLogbookE = @OfficialLogbookE, 
                           DeckLogbook = @DeckLogbook, Listofcrew = @Listofcrew, LastHose = @LastHose, PassagePlanning = @PassagePlanning, 
                           LoadingComputer = @LoadingComputer, EngineLogbook = @EngineLogbook, OilRecordBook = @OilRecordBook, RiskAssessments = @RiskAssessments, 
                           GMDSSLogbook = @GMDSSLogbook, DeckLogbook5D = @DeckLogbook5D, GarbageRecordBook = @GarbageRecordBook, BallastWaterRecordBook = @BallastWaterRecordBook, 
                           CargoRecordBook = @CargoRecordBook, EmissionsControlManual = @EmissionsControlManual, LGR = @LGR, PEER = @PEER, 
                           RecordKeepingComments = @RecordKeepingComments, LastPortStateControl = @LastPortStateControl, LiferaftsComment = @LiferaftsComment, releasesComment = @releasesComment, 
                           LifeboatComment = @LifeboatComment, LifeboatdavitComment = @LifeboatdavitComment, LifeboatequipmentComment = @LifeboatequipmentComment, RescueboatComment = @RescueboatComment, 
                           RescueboatequipmentComment = @RescueboatequipmentComment, RescueboatoutboardmotorComment = @RescueboatoutboardmotorComment, 
                           RescueboatdavitComment = @RescueboatdavitComment, DeckComment = @DeckComment, PyrotechnicsComment = @PyrotechnicsComment, EPIRBComment = @EPIRBComment, 
                           SARTsComment = @SARTsComment, GMDSSComment = @GMDSSComment, ManoverboardComment = @ManoverboardComment, LinethrowingapparatusComment = @LinethrowingapparatusComment, 
                           FireextinguishersComment = @FireextinguishersComment, EmergencygeneratorComment = @EmergencygeneratorComment, CO2roomComment = @CO2roomComment, SurvivalComment = @SurvivalComment, 
                           LifejacketComment = @LifejacketComment, FiremansComment = @FiremansComment, LifebuoysComment = @LifebuoysComment, FireboxesComment = @FireboxesComment, 
                           EmergencybellsComment = @EmergencybellsComment, EmergencylightingComment = @EmergencylightingComment, FireplanComment = @FireplanComment, DamageComment = @DamageComment, 
                           EmergencyplansComment = @EmergencyplansComment, MusterlistComment = @MusterlistComment, SafetysignsComment = @SafetysignsComment, EmergencysteeringComment = @EmergencysteeringComment, 
                           StatutoryemergencydrillsComment = @StatutoryemergencydrillsComment, EEBDComment = @EEBDComment, OxygenComment = @OxygenComment, MultigasdetectorComment = @MultigasdetectorComment, 
                           GasdetectorComment = @GasdetectorComment, SufficientquantityComment = @SufficientquantityComment, BASetsComment = @BASetsComment, SafetyComment = @SafetyComment, 
                           GangwayComment = @GangwayComment, RestrictedComment = @RestrictedComment, OutsideComment = @OutsideComment, EntrancedoorsComment = @EntrancedoorsComment, 
                           AccommodationComment = @AccommodationComment, GMDSSComment5G = @GMDSSComment5G, VariousComment = @VariousComment, SSOComment = @SSOComment, 
                           SecuritylogbookComment = @SecuritylogbookComment, Listoflast10portsComment = @Listoflast10portsComment, PFSOComment = @PFSOComment, SecuritylevelComment = @SecuritylevelComment, 
                           DrillsandtrainingComment = @DrillsandtrainingComment, DOSComment = @DOSComment, SSASComment = @SSASComment, VisitorslogbookComment = @VisitorslogbookComment, 
                           KeyregisterComment = @KeyregisterComment, ShipSecurityComment = @ShipSecurityComment, SecurityComment = @SecurityComment, NauticalchartsComment = @NauticalchartsComment, 
                           NoticetomarinersComment = @NoticetomarinersComment, ListofradiosignalsComment = @ListofradiosignalsComment, ListoflightsComment = @ListoflightsComment, SailingdirectionsComment = @SailingdirectionsComment, 
                           TidetablesComment = @TidetablesComment, NavtexandprinterComment = @NavtexandprinterComment, RadarsComment = @RadarsComment, GPSComment = @GPSComment, 
                           AISComment = @AISComment, VDRComment = @VDRComment, ECDISComment = @ECDISComment, EchosounderComment = @EchosounderComment, 
                           ADPbackuplaptopComment = @ADPbackuplaptopComment, ColourprinterComment = @ColourprinterComment, VHFDSCtransceiverComment = @VHFDSCtransceiverComment, radioinstallationComment = @radioinstallationComment, 
                           InmarsatCComment = @InmarsatCComment, MagneticcompassComment = @MagneticcompassComment, SparecompassbowlComment = @SparecompassbowlComment, CompassobservationbookComment = @CompassobservationbookComment, 
                           GyrocompassComment = @GyrocompassComment, RudderindicatorComment = @RudderindicatorComment, SpeedlogComment = @SpeedlogComment, NavigationComment = @NavigationComment, 
                           SignalflagsComment = @SignalflagsComment, RPMComment = @RPMComment, BasicmanoeuvringdataComment = @BasicmanoeuvringdataComment, MasterstandingordersComment = @MasterstandingordersComment, 
                           MasternightordersbookComment = @MasternightordersbookComment, SextantComment = @SextantComment, AzimuthmirrorComment = @AzimuthmirrorComment, BridgepostersComment = @BridgepostersComment, 
                           ReviewofplannedComment = @ReviewofplannedComment, BridgebellbookComment = @BridgebellbookComment, BridgenavigationalComment = @BridgenavigationalComment, SecurityEquipmentComment = @SecurityEquipmentComment, 
                           NavigationPost = @NavigationPost, GeneralComment = @GeneralComment, MedicinestorageComment = @MedicinestorageComment, MedicinechestcertificateComment = @MedicinechestcertificateComment, 
                           InventoryStoresComment = @InventoryStoresComment, OxygencylindersComment = @OxygencylindersComment, StretcherComment = @StretcherComment, SalivaComment = @SalivaComment, 
                           AlcoholComment = @AlcoholComment, HospitalComment = @HospitalComment, GeneralGalleyComment = @GeneralGalleyComment, HygieneComment = @HygieneComment, 
                           FoodstorageComment = @FoodstorageComment, FoodlabellingComment = @FoodlabellingComment, GalleyriskassessmentComment = @GalleyriskassessmentComment, FridgetemperatureComment = @FridgetemperatureComment, 
                           FoodandProvisionsComment = @FoodandProvisionsComment, GalleyComment = @GalleyComment, 
                           ConditionComment = @ConditionComment, PaintworkComment = @PaintworkComment, LightingComment = @LightingComment, 
                           PlatesComment = @PlatesComment, BilgesComment = @BilgesComment, PipelinesandvalvesComment = @PipelinesandvalvesComment, 
                           LeakageComment = @LeakageComment, EquipmentComment = @EquipmentComment, OilywaterseparatorComment = @OilywaterseparatorComment, 
                           FueloiltransferplanComment = @FueloiltransferplanComment, SteeringgearComment = @SteeringgearComment, 
                           WorkshopandequipmentComment = @WorkshopandequipmentComment, SoundingpipesComment = @SoundingpipesComment, 
                           EnginecontrolComment = @EnginecontrolComment, ChiefEngineernightordersbookComment = @ChiefEngineernightordersbookComment, 
                           ChiefEngineerstandingordersComment = @ChiefEngineerstandingordersComment, PreUMSComment = @PreUMSComment, 
                           EnginebellbookComment = @EnginebellbookComment, LockoutComment = @LockoutComment, EngineRoomComment = @EngineRoomComment, 
                           CleanlinessandhygieneComment = @CleanlinessandhygieneComment, ConditionComment5M = @ConditionComment5M, 
                           PaintworkComment5M = @PaintworkComment5M, SignalmastandstaysComment = @SignalmastandstaysComment, 
                           MonkeyislandComment = @MonkeyislandComment, FireDampersComment = @FireDampersComment, 
                           RailsBulwarksComment = @RailsBulwarksComment, WatertightdoorsComment = @WatertightdoorsComment, 
                           VentilatorsComment = @VentilatorsComment, WinchesComment = @WinchesComment, 
                           FairleadsComment = @FairleadsComment, MooringLinesComment = @MooringLinesComment, 
                           EmergencyShutOffsComment = @EmergencyShutOffsComment, RadioaerialsComment = @RadioaerialsComment, 
                           SOPEPlockerComment = @SOPEPlockerComment, ChemicallockerComment = @ChemicallockerComment, 
                           AntislippaintComment = @AntislippaintComment, SuperstructureComment = @SuperstructureComment, 
                           CabinsComment = @CabinsComment, OfficesComment = @OfficesComment, MessroomsComment = @MessroomsComment, 
                           ToiletsComment = @ToiletsComment, LaundryroomComment = @LaundryroomComment, 
                           ChangingroomComment = @ChangingroomComment, OtherComment = @OtherComment, ConditionComment5N = @ConditionComment5N, 
                           SelfclosingfiredoorsComment = @SelfclosingfiredoorsComment, StairwellsComment = @StairwellsComment, SuperstructureInternalComment = @SuperstructureInternalComment, 
                           PortablegangwayComment = @PortablegangwayComment, SafetynetComment = @SafetynetComment, 
                           AccommodationLadderComment = @AccommodationLadderComment, SafeaccessprovidedComment = @SafeaccessprovidedComment, 
                           PilotladdersComment = @PilotladdersComment, BoardingEquipmentComment = @BoardingEquipmentComment, 
                           CleanlinessComment = @CleanlinessComment, PaintworkComment5P = @PaintworkComment5P, ShipsiderailsComment = @ShipsiderailsComment, 
                           WeathertightdoorsComment = @WeathertightdoorsComment, FirehydrantsComment = @FirehydrantsComment, 
                           VentilatorsComment5P = @VentilatorsComment5P, ManholecoversComment = @ManholecoversComment, MainDeckAreaComment = @MainDeckAreaComment, 
                           ConditionComment5Q = @ConditionComment5Q, PaintworkComment5Q = @PaintworkComment5Q, 
                           MechanicaldamageComment = @MechanicaldamageComment, AccessladdersComment = @AccessladdersComment, 
                           ManholecoversComment5Q = @ManholecoversComment5Q, HoldbilgeComment = @HoldbilgeComment, 
                           AccessdoorsComment = @AccessdoorsComment, ConditionHatchCoversComment = @ConditionHatchCoversComment, 
                           PaintworkHatchCoversComment = @PaintworkHatchCoversComment, RubbersealsComment = @RubbersealsComment, 
                           SignsofhatchesComment = @SignsofhatchesComment, SealingtapeComment = @SealingtapeComment, 
                           ConditionofhydraulicsComment = @ConditionofhydraulicsComment, PortablebulkheadsComment = @PortablebulkheadsComment, 
                           TweendecksComment = @TweendecksComment, HatchcoamingComment = @HatchcoamingComment, 
                           ConditionCargoCranesComment = @ConditionCargoCranesComment, GantrycranealarmComment = @GantrycranealarmComment, 
                           GantryconditionComment = @GantryconditionComment, CargoHoldsComment = @CargoHoldsComment, 
                           CleanlinessComment5R = @CleanlinessComment5R, PaintworkComment5R = @PaintworkComment5R, TriphazardsComment = @TriphazardsComment, 
                           WindlassComment = @WindlassComment, CablesComment = @CablesComment, WinchesComment5R = @WinchesComment5R, 
                           FairleadsComment5R = @FairleadsComment5R, MooringComment = @MooringComment, 
                           HatchToforecastlespaceComment = @HatchToforecastlespaceComment, VentilatorsComment5R = @VentilatorsComment5R, 
                           BellComment = @BellComment, ForemastComment = @ForemastComment, FireComment = @FireComment, 
                           RailsComment = @RailsComment, AntislippaintComment5R = @AntislippaintComment5R,
                           SnapBackZoneComment = @SnapBackZoneComment,
                           ConditionGantryCranesComment=@ConditionGantryCranesComment,
                           CylindersLockerComment=@CylindersLockerComment,SnapBackZone5NComment=@SnapBackZone5NComment,
                           MedicalLogBookComment=@MedicalLogBookComment, DrugsNarcoticsComment=@DrugsNarcoticsComment,
                           DefibrillatorComment=@DefibrillatorComment,RPWaterHandbook=@RPWaterHandbook,BioRPWH=@BioRPWH,
                           PRE=@PRE,NoiseVibrationFile=@NoiseVibrationFile,BioMPR=@BioMPR,
                           AsbestosPlan=@AsbestosPlan,ShipPublicAddrComment=@ShipPublicAddrComment,
                           BridgewindowswiperssprayComment=@BridgewindowswiperssprayComment,
                           BridgewindowswipersComment=@BridgewindowswipersComment,DaylightSignalsComment=@DaylightSignalsComment,
                           LiferaftDavitComment=@LiferaftDavitComment,ADPPublicationsComment=@ADPPublicationsComment,
                           ForecastleComment = @ForecastleComment, 
                           CleanlinessComment5S = @CleanlinessComment5S, PaintworkComment5S = @PaintworkComment5S, 
                           ForepeakComment = @ForepeakComment, ChainlockerComment = @ChainlockerComment, LightingComment5S = @LightingComment5S, 
                           AccesssafetychainComment = @AccesssafetychainComment, EmergencyfirepumpComment = @EmergencyfirepumpComment, 
                           BowthrusterandroomComment = @BowthrusterandroomComment, SparemooringlinesComment = @SparemooringlinesComment, 
                           PaintlockerComment = @PaintlockerComment, ForecastleSpaceComment = @ForecastleSpaceComment, 
                           BoottopComment = @BoottopComment, TopsidesComment = @TopsidesComment, AntifoulingComment = @AntifoulingComment, 
                           DraftandplimsollComment = @DraftandplimsollComment, FoulingComment = @FoulingComment, MechanicalComment = @MechanicalComment, 
                           HullComment = @HullComment, SummaryComment = @SummaryComment, 
                           IsSynced = @IsSynced, UpdatedDate = @UpdatedDate, SavedAsDraft = @SavedAsDraft , IsGeneralSectionComplete = @IsGeneralSectionComplete
                           , IsManningSectionComplete = @IsManningSectionComplete, IsShipCertificationSectionComplete = @IsShipCertificationSectionComplete, IsRecordKeepingSectionComplete = @IsRecordKeepingSectionComplete
                           , IsSafetyEquipmentSectionComplete = @IsSafetyEquipmentSectionComplete, IsSecuritySectionComplete = @IsSecuritySectionComplete, IsBridgeSectionComplete = @IsBridgeSectionComplete
                           , IsMedicalSectionComplete = @IsMedicalSectionComplete, IsGalleySectionComplete = @IsGalleySectionComplete, IsEngineRoomSectionComplete = @IsEngineRoomSectionComplete
                           , IsSuperstructureSectionComplete = @IsSuperstructureSectionComplete, IsDeckSectionComplete = @IsDeckSectionComplete, IsHoldsAndCoverSectionComplete = @IsHoldsAndCoverSectionComplete
                           , IsForeCastleSectionComplete = @IsForeCastleSectionComplete, IsHullSectionComplete = @IsHullSectionComplete, IsSummarySectionComplete = @IsSummarySectionComplete
                           , IsDeficienciesSectionComplete = @IsDeficienciesSectionComplete, IsPhotographsSectionComplete = @IsPhotographsSectionComplete
                           WHERE UniqueFormID = @UniqueFormID";
            return query;
        }
        public void GIRUpdateCMD(GeneralInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.FormVersion;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID == null ? DBNull.Value : (object)Modal.ShipID;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName == null ? string.Empty : Modal.ShipName;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? string.Empty : Modal.Ship;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port == null ? string.Empty : Modal.Port;
            command.Parameters.Add("@Inspector", SqlDbType.NVarChar).Value = Modal.Inspector == null ? string.Empty : Modal.Inspector;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date == null ? DBNull.Value : (object)Modal.Date;
            command.Parameters.Add("@GeneralPreamble", SqlDbType.NVarChar).Value = Modal.GeneralPreamble != null ? Modal.GeneralPreamble : "";
            command.Parameters.Add("@Classsociety", SqlDbType.NVarChar).Value = Modal.Classsociety != null ? Modal.Classsociety : "";
            command.Parameters.Add("@YearofBuild", SqlDbType.NVarChar).Value = Modal.YearofBuild != null ? Modal.YearofBuild : "";
            command.Parameters.Add("@Flag", SqlDbType.NVarChar).Value = Modal.Flag != null ? Modal.Flag : "";
            command.Parameters.Add("@Classofvessel", SqlDbType.NVarChar).Value = Modal.Classofvessel != null ? Modal.Classofvessel : "";
            command.Parameters.Add("@Portofregistry", SqlDbType.NVarChar).Value = Modal.Portofregistry != null ? Modal.Portofregistry : "";
            command.Parameters.Add("@MMSI", SqlDbType.NVarChar).Value = Modal.MMSI != null ? Modal.MMSI : "";
            command.Parameters.Add("@IMOnumber", SqlDbType.NVarChar).Value = Modal.IMOnumber != null ? Modal.IMOnumber : "";
            command.Parameters.Add("@Callsign", SqlDbType.NVarChar).Value = Modal.Callsign != null ? Modal.Callsign : "";
            command.Parameters.Add("@SummerDWT", SqlDbType.NVarChar).Value = Modal.SummerDWT != null ? Modal.SummerDWT : "";
            command.Parameters.Add("@Grosstonnage", SqlDbType.NVarChar).Value = Modal.Grosstonnage != null ? Modal.Grosstonnage : "";
            command.Parameters.Add("@Lightweight", SqlDbType.NVarChar).Value = Modal.Lightweight != null ? Modal.Lightweight : "";
            command.Parameters.Add("@Nettonnage", SqlDbType.NVarChar).Value = Modal.Nettonnage != null ? Modal.Nettonnage : "";
            command.Parameters.Add("@Beam", SqlDbType.NVarChar).Value = Modal.Beam != null ? Modal.Beam : "";
            command.Parameters.Add("@LOA", SqlDbType.NVarChar).Value = Modal.LOA != null ? Modal.LOA : "";
            command.Parameters.Add("@Summerdraft", SqlDbType.NVarChar).Value = Modal.Summerdraft != null ? Modal.Summerdraft : "";
            command.Parameters.Add("@LBP", SqlDbType.NVarChar).Value = Modal.LBP != null ? Modal.LBP : "";
            command.Parameters.Add("@Bowthruster", SqlDbType.NVarChar).Value = Modal.Bowthruster != null ? Modal.Bowthruster : "";
            command.Parameters.Add("@BHP", SqlDbType.NVarChar).Value = Modal.BHP != null ? Modal.BHP : "";
            command.Parameters.Add("@Noofholds", SqlDbType.NVarChar).Value = Modal.Noofholds != null ? Modal.Noofholds : "";
            command.Parameters.Add("@Nomoveablebulkheads", SqlDbType.NVarChar).Value = Modal.Nomoveablebulkheads != null ? Modal.Nomoveablebulkheads : "";
            command.Parameters.Add("@Containers", SqlDbType.NVarChar).Value = Modal.Containers != null ? Modal.Containers : "";
            command.Parameters.Add("@Cargocapacity", SqlDbType.NVarChar).Value = Modal.Cargocapacity != null ? Modal.Cargocapacity : "";
            command.Parameters.Add("@Cargohandlingequipment", SqlDbType.NVarChar).Value = Modal.Cargohandlingequipment != null ? Modal.Cargohandlingequipment : "";
            command.Parameters.Add("@Lastvoyageandcargo", SqlDbType.NVarChar).Value = Modal.Lastvoyageandcargo != null ? Modal.Lastvoyageandcargo : "";
            command.Parameters.Add("@CurrentPlannedvoyageandcargo", SqlDbType.NVarChar).Value = Modal.CurrentPlannedvoyageandcargo != null ? Modal.CurrentPlannedvoyageandcargo : "";
            command.Parameters.Add("@ShipboardWorkingArrangements", SqlDbType.NVarChar).Value = Modal.ShipboardWorkingArrangements != null ? Modal.ShipboardWorkingArrangements : "";
            command.Parameters.Add("@CertificationIndex", SqlDbType.NVarChar).Value = Modal.CertificationIndex != null ? Modal.CertificationIndex : "";
            command.Parameters.Add("@CarriedOutByTheDOOW", SqlDbType.NVarChar).Value = Modal.CarriedOutByTheDOOW != null ? Modal.CarriedOutByTheDOOW : "";
            command.Parameters.Add("@IsSOPEPPoints", SqlDbType.Bit).Value = Modal.IsSOPEPPoints == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@SOPEPPoints", SqlDbType.NVarChar).Value = Modal.SOPEPPoints != null ? Modal.SOPEPPoints : "";
            command.Parameters.Add("@IsRegs4shipsDVD", SqlDbType.Bit).Value = Modal.IsRegs4shipsDVD == null ? false : true;  //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@Regs4shipsDVD", SqlDbType.NVarChar).Value = Modal.Regs4shipsDVD != null ? Modal.Regs4shipsDVD : "";
            command.Parameters.Add("@IsBWMP", SqlDbType.Bit).Value = Modal.IsBWMP == null ? false : true;  //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@BWMP", SqlDbType.NVarChar).Value = Modal.BWMP != null ? Modal.BWMP : "";
            command.Parameters.Add("@IsBWMPSupplement", SqlDbType.Bit).Value = Modal.IsBWMPSupplement == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@BWMPSupplement", SqlDbType.NVarChar).Value = Modal.BWMPSupplement != null ? Modal.BWMPSupplement : "";
            command.Parameters.Add("@IsIntactStabilityManual", SqlDbType.Bit).Value = Modal.IsIntactStabilityManual == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@IntactStabilityManual", SqlDbType.NVarChar).Value = Modal.IntactStabilityManual != null ? Modal.IntactStabilityManual : "";
            command.Parameters.Add("@IsStabilityComputer", SqlDbType.Bit).Value = Modal.IsStabilityComputer == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@StabilityComputer", SqlDbType.NVarChar).Value = Modal.StabilityComputer != null ? Modal.StabilityComputer : "";
            command.Parameters.Add("@IsDateOfLast", SqlDbType.Bit).Value = Modal.IsDateOfLast == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@DateOfLast", SqlDbType.NVarChar).Value = Modal.DateOfLast != null ? Modal.DateOfLast : "";
            command.Parameters.Add("@IsCargoSecuring", SqlDbType.Bit).Value = Modal.IsCargoSecuring == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@CargoSecuring", SqlDbType.NVarChar).Value = Modal.CargoSecuring != null ? Modal.CargoSecuring : "";
            command.Parameters.Add("@IsBulkCargo", SqlDbType.Bit).Value = Modal.IsBulkCargo == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@BulkCargo", SqlDbType.NVarChar).Value = Modal.BulkCargo != null ? Modal.BulkCargo : "";
            command.Parameters.Add("@IsSMSManual", SqlDbType.Bit).Value = Modal.IsSMSManual == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@SMSManual", SqlDbType.NVarChar).Value = Modal.SMSManual != null ? Modal.SMSManual : "";
            command.Parameters.Add("@IsRegisterOf", SqlDbType.Bit).Value = Modal.IsRegisterOf == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@RegisterOf", SqlDbType.NVarChar).Value = Modal.RegisterOf != null ? Modal.RegisterOf : "";
            command.Parameters.Add("@IsFleetStandingOrder", SqlDbType.Bit).Value = Modal.IsFleetStandingOrder == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@FleetStandingOrder", SqlDbType.NVarChar).Value = Modal.FleetStandingOrder != null ? Modal.FleetStandingOrder : "";
            command.Parameters.Add("@IsFleetMemoranda", SqlDbType.Bit).Value = Modal.IsFleetMemoranda == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@FleetMemoranda", SqlDbType.NVarChar).Value = Modal.FleetMemoranda != null ? Modal.FleetMemoranda : "";
            command.Parameters.Add("@IsShipsPlans", SqlDbType.Bit).Value = Modal.IsShipsPlans == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@ShipsPlans", SqlDbType.NVarChar).Value = Modal.ShipsPlans != null ? Modal.ShipsPlans : "";
            command.Parameters.Add("@IsCollective", SqlDbType.Bit).Value = Modal.IsCollective == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@Collective", SqlDbType.NVarChar).Value = Modal.Collective != null ? Modal.Collective : "";
            command.Parameters.Add("@IsDraftAndFreeboardNotice", SqlDbType.Bit).Value = Modal.IsDraftAndFreeboardNotice == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@DraftAndFreeboardNotice", SqlDbType.NVarChar).Value = Modal.DraftAndFreeboardNotice != null ? Modal.DraftAndFreeboardNotice : "";
            command.Parameters.Add("@IsPCSOPEP", SqlDbType.Bit).Value = Modal.IsPCSOPEP == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@PCSOPEP", SqlDbType.NVarChar).Value = Modal.PCSOPEP != null ? Modal.PCSOPEP : "";
            command.Parameters.Add("@IsNTVRP", SqlDbType.Bit).Value = Modal.IsNTVRP == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@NTVRP", SqlDbType.NVarChar).Value = Modal.NTVRP != null ? Modal.NTVRP : "";
            command.Parameters.Add("@IsVGP", SqlDbType.Bit).Value = Modal.IsVGP == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@VGP", SqlDbType.NVarChar).Value = Modal.VGP != null ? Modal.VGP : "";
            command.Parameters.Add("@PubsComments", SqlDbType.NVarChar).Value = Modal.PubsComments != null ? Modal.PubsComments : "";
            command.Parameters.Add("@IsPubsAndDocsSectionComplete", SqlDbType.Bit).Value = Modal.IsPubsAndDocsSectionComplete == null ? false : true; //RDBJ 10/19/2021 set value based on checkbox checked
            command.Parameters.Add("@OfficialLogbookA", SqlDbType.NVarChar).Value = Modal.OfficialLogbookA != null ? Modal.OfficialLogbookA : "";
            command.Parameters.Add("@OfficialLogbookB", SqlDbType.NVarChar).Value = Modal.OfficialLogbookB != null ? Modal.OfficialLogbookB : "";
            command.Parameters.Add("@OfficialLogbookC", SqlDbType.NVarChar).Value = Modal.OfficialLogbookC != null ? Modal.OfficialLogbookC : "";
            command.Parameters.Add("@OfficialLogbookD", SqlDbType.NVarChar).Value = Modal.OfficialLogbookD != null ? Modal.OfficialLogbookD : "";
            command.Parameters.Add("@OfficialLogbookE", SqlDbType.NVarChar).Value = Modal.OfficialLogbookE != null ? Modal.OfficialLogbookE : "";
            command.Parameters.Add("@DeckLogbook", SqlDbType.NVarChar).Value = Modal.DeckLogbook != null ? Modal.DeckLogbook : "";
            command.Parameters.Add("@Listofcrew", SqlDbType.NVarChar).Value = Modal.Listofcrew != null ? Modal.Listofcrew : "";
            command.Parameters.Add("@LastHose", SqlDbType.NVarChar).Value = Modal.LastHose != null ? Modal.LastHose : "";
            command.Parameters.Add("@PassagePlanning", SqlDbType.NVarChar).Value = Modal.PassagePlanning != null ? Modal.PassagePlanning : "";
            command.Parameters.Add("@LoadingComputer", SqlDbType.NVarChar).Value = Modal.LoadingComputer != null ? Modal.LoadingComputer : "";
            command.Parameters.Add("@EngineLogbook", SqlDbType.NVarChar).Value = Modal.EngineLogbook != null ? Modal.EngineLogbook : "";
            command.Parameters.Add("@OilRecordBook", SqlDbType.NVarChar).Value = Modal.OilRecordBook != null ? Modal.OilRecordBook : "";
            command.Parameters.Add("@RiskAssessments", SqlDbType.NVarChar).Value = Modal.RiskAssessments != null ? Modal.RiskAssessments : "";
            command.Parameters.Add("@GMDSSLogbook", SqlDbType.NVarChar).Value = Modal.GMDSSLogbook != null ? Modal.GMDSSLogbook : "";
            command.Parameters.Add("@DeckLogbook5D", SqlDbType.NVarChar).Value = Modal.DeckLogbook5D != null ? Modal.DeckLogbook5D : "";
            command.Parameters.Add("@GarbageRecordBook", SqlDbType.NVarChar).Value = Modal.GarbageRecordBook != null ? Modal.GarbageRecordBook : "";
            command.Parameters.Add("@BallastWaterRecordBook", SqlDbType.NVarChar).Value = Modal.BallastWaterRecordBook != null ? Modal.BallastWaterRecordBook : "";
            command.Parameters.Add("@CargoRecordBook", SqlDbType.NVarChar).Value = Modal.CargoRecordBook != null ? Modal.CargoRecordBook : "";
            command.Parameters.Add("@EmissionsControlManual", SqlDbType.NVarChar).Value = Modal.EmissionsControlManual != null ? Modal.EmissionsControlManual : "";
            command.Parameters.Add("@LGR", SqlDbType.NVarChar).Value = Modal.LGR != null ? Modal.LGR : "";
            command.Parameters.Add("@PEER", SqlDbType.NVarChar).Value = Modal.PEER != null ? Modal.PEER : "";
            command.Parameters.Add("@RecordKeepingComments", SqlDbType.NVarChar).Value = Modal.RecordKeepingComments != null ? Modal.RecordKeepingComments : "";
            command.Parameters.Add("@LastPortStateControl", SqlDbType.NVarChar).Value = Modal.LastPortStateControl != null ? Modal.LastPortStateControl : "";

            command.Parameters.Add("@LiferaftsComment", SqlDbType.NVarChar).Value = Modal.LiferaftsComment != null ? Modal.LiferaftsComment : "";
            command.Parameters.Add("@releasesComment", SqlDbType.NVarChar).Value = Modal.releasesComment != null ? Modal.releasesComment : "";
            command.Parameters.Add("@LifeboatComment", SqlDbType.NVarChar).Value = Modal.LifeboatComment != null ? Modal.LifeboatComment : "";
            command.Parameters.Add("@LifeboatdavitComment", SqlDbType.NVarChar).Value = Modal.LifeboatdavitComment != null ? Modal.LifeboatdavitComment : "";
            command.Parameters.Add("@LifeboatequipmentComment", SqlDbType.NVarChar).Value = Modal.LifeboatequipmentComment != null ? Modal.LifeboatequipmentComment : "";
            command.Parameters.Add("@RescueboatComment", SqlDbType.NVarChar).Value = Modal.RescueboatComment != null ? Modal.RescueboatComment : "";
            command.Parameters.Add("@RescueboatequipmentComment", SqlDbType.NVarChar).Value = Modal.RescueboatequipmentComment != null ? Modal.RescueboatequipmentComment : "";
            command.Parameters.Add("@RescueboatoutboardmotorComment", SqlDbType.NVarChar).Value = Modal.RescueboatoutboardmotorComment != null ? Modal.RescueboatoutboardmotorComment : "";
            command.Parameters.Add("@RescueboatdavitComment", SqlDbType.NVarChar).Value = Modal.RescueboatdavitComment != null ? Modal.RescueboatdavitComment : "";
            command.Parameters.Add("@DeckComment", SqlDbType.NVarChar).Value = Modal.DeckComment != null ? Modal.DeckComment : "";
            command.Parameters.Add("@PyrotechnicsComment", SqlDbType.NVarChar).Value = Modal.PyrotechnicsComment != null ? Modal.PyrotechnicsComment : "";
            command.Parameters.Add("@EPIRBComment", SqlDbType.NVarChar).Value = Modal.EPIRBComment != null ? Modal.EPIRBComment : "";
            command.Parameters.Add("@SARTsComment", SqlDbType.NVarChar).Value = Modal.SARTsComment != null ? Modal.SARTsComment : "";
            command.Parameters.Add("@GMDSSComment", SqlDbType.NVarChar).Value = Modal.GMDSSComment != null ? Modal.GMDSSComment : "";
            command.Parameters.Add("@ManoverboardComment", SqlDbType.NVarChar).Value = Modal.ManoverboardComment != null ? Modal.ManoverboardComment : "";
            command.Parameters.Add("@LinethrowingapparatusComment", SqlDbType.NVarChar).Value = Modal.LinethrowingapparatusComment != null ? Modal.LinethrowingapparatusComment : "";
            command.Parameters.Add("@FireextinguishersComment", SqlDbType.NVarChar).Value = Modal.FireextinguishersComment != null ? Modal.FireextinguishersComment : "";
            command.Parameters.Add("@EmergencygeneratorComment", SqlDbType.NVarChar).Value = Modal.EmergencygeneratorComment != null ? Modal.EmergencygeneratorComment : "";
            command.Parameters.Add("@CO2roomComment", SqlDbType.NVarChar).Value = Modal.CO2roomComment != null ? Modal.CO2roomComment : "";
            command.Parameters.Add("@SurvivalComment", SqlDbType.NVarChar).Value = Modal.SurvivalComment != null ? Modal.SurvivalComment : "";
            command.Parameters.Add("@LifejacketComment", SqlDbType.NVarChar).Value = Modal.LifejacketComment != null ? Modal.LifejacketComment : "";
            command.Parameters.Add("@FiremansComment", SqlDbType.NVarChar).Value = Modal.FiremansComment != null ? Modal.FiremansComment : "";
            command.Parameters.Add("@LifebuoysComment", SqlDbType.NVarChar).Value = Modal.LifebuoysComment != null ? Modal.LifebuoysComment : "";
            command.Parameters.Add("@FireboxesComment", SqlDbType.NVarChar).Value = Modal.FireboxesComment != null ? Modal.FireboxesComment : "";
            command.Parameters.Add("@EmergencybellsComment", SqlDbType.NVarChar).Value = Modal.EmergencybellsComment != null ? Modal.EmergencybellsComment : "";
            command.Parameters.Add("@EmergencylightingComment", SqlDbType.NVarChar).Value = Modal.EmergencylightingComment != null ? Modal.EmergencylightingComment : "";
            command.Parameters.Add("@FireplanComment", SqlDbType.NVarChar).Value = Modal.FireplanComment != null ? Modal.FireplanComment : "";
            command.Parameters.Add("@DamageComment", SqlDbType.NVarChar).Value = Modal.DamageComment != null ? Modal.DamageComment : "";
            command.Parameters.Add("@EmergencyplansComment", SqlDbType.NVarChar).Value = Modal.EmergencyplansComment != null ? Modal.EmergencyplansComment : "";
            command.Parameters.Add("@MusterlistComment", SqlDbType.NVarChar).Value = Modal.MusterlistComment != null ? Modal.MusterlistComment : "";
            command.Parameters.Add("@SafetysignsComment", SqlDbType.NVarChar).Value = Modal.SafetysignsComment != null ? Modal.SafetysignsComment : "";
            command.Parameters.Add("@EmergencysteeringComment", SqlDbType.NVarChar).Value = Modal.EmergencysteeringComment != null ? Modal.EmergencysteeringComment : "";
            command.Parameters.Add("@StatutoryemergencydrillsComment", SqlDbType.NVarChar).Value = Modal.StatutoryemergencydrillsComment != null ? Modal.StatutoryemergencydrillsComment : "";
            command.Parameters.Add("@EEBDComment", SqlDbType.NVarChar).Value = Modal.EEBDComment != null ? Modal.EEBDComment : "";
            command.Parameters.Add("@OxygenComment", SqlDbType.NVarChar).Value = Modal.OxygenComment != null ? Modal.OxygenComment : "";
            command.Parameters.Add("@MultigasdetectorComment", SqlDbType.NVarChar).Value = Modal.MultigasdetectorComment != null ? Modal.MultigasdetectorComment : "";
            command.Parameters.Add("@GasdetectorComment", SqlDbType.NVarChar).Value = Modal.GasdetectorComment != null ? Modal.GasdetectorComment : "";
            command.Parameters.Add("@SufficientquantityComment", SqlDbType.NVarChar).Value = Modal.SufficientquantityComment != null ? Modal.SufficientquantityComment : "";
            command.Parameters.Add("@BASetsComment", SqlDbType.NVarChar).Value = Modal.BASetsComment != null ? Modal.BASetsComment : "";
            command.Parameters.Add("@SafetyComment", SqlDbType.NVarChar).Value = Modal.SafetyComment != null ? Modal.SafetyComment : "";

            command.Parameters.Add("@GangwayComment", SqlDbType.NVarChar).Value = Modal.GangwayComment != null ? Modal.GangwayComment : "";
            command.Parameters.Add("@RestrictedComment", SqlDbType.NVarChar).Value = Modal.RestrictedComment != null ? Modal.RestrictedComment : "";
            command.Parameters.Add("@OutsideComment", SqlDbType.NVarChar).Value = Modal.OutsideComment != null ? Modal.OutsideComment : "";
            command.Parameters.Add("@EntrancedoorsComment", SqlDbType.NVarChar).Value = Modal.EntrancedoorsComment != null ? Modal.EntrancedoorsComment : "";
            command.Parameters.Add("@AccommodationComment", SqlDbType.NVarChar).Value = Modal.AccommodationComment != null ? Modal.AccommodationComment : "";
            command.Parameters.Add("@GMDSSComment5G", SqlDbType.NVarChar).Value = Modal.GMDSSComment5G != null ? Modal.GMDSSComment5G : "";
            command.Parameters.Add("@VariousComment", SqlDbType.NVarChar).Value = Modal.VariousComment != null ? Modal.VariousComment : "";
            command.Parameters.Add("@SSOComment", SqlDbType.NVarChar).Value = Modal.SSOComment != null ? Modal.SSOComment : "";
            command.Parameters.Add("@SecuritylogbookComment", SqlDbType.NVarChar).Value = Modal.SecuritylogbookComment != null ? Modal.SecuritylogbookComment : "";
            command.Parameters.Add("@Listoflast10portsComment", SqlDbType.NVarChar).Value = Modal.Listoflast10portsComment != null ? Modal.Listoflast10portsComment : "";
            command.Parameters.Add("@PFSOComment", SqlDbType.NVarChar).Value = Modal.PFSOComment != null ? Modal.PFSOComment : "";
            command.Parameters.Add("@SecuritylevelComment", SqlDbType.NVarChar).Value = Modal.SecuritylevelComment != null ? Modal.SecuritylevelComment : "";
            command.Parameters.Add("@DrillsandtrainingComment", SqlDbType.NVarChar).Value = Modal.DrillsandtrainingComment != null ? Modal.DrillsandtrainingComment : "";
            command.Parameters.Add("@DOSComment", SqlDbType.NVarChar).Value = Modal.DOSComment != null ? Modal.DOSComment : "";
            command.Parameters.Add("@SSASComment", SqlDbType.NVarChar).Value = Modal.SSASComment != null ? Modal.SSASComment : "";
            command.Parameters.Add("@VisitorslogbookComment", SqlDbType.NVarChar).Value = Modal.VisitorslogbookComment != null ? Modal.VisitorslogbookComment : "";
            command.Parameters.Add("@KeyregisterComment", SqlDbType.NVarChar).Value = Modal.KeyregisterComment != null ? Modal.KeyregisterComment : "";
            command.Parameters.Add("@ShipSecurityComment", SqlDbType.NVarChar).Value = Modal.ShipSecurityComment != null ? Modal.ShipSecurityComment : "";
            command.Parameters.Add("@SecurityComment", SqlDbType.NVarChar).Value = Modal.SecurityComment != null ? Modal.SecurityComment : "";

            command.Parameters.Add("@NauticalchartsComment", SqlDbType.NVarChar).Value = Modal.NauticalchartsComment != null ? Modal.NauticalchartsComment : "";
            command.Parameters.Add("@NoticetomarinersComment", SqlDbType.NVarChar).Value = Modal.NoticetomarinersComment != null ? Modal.NoticetomarinersComment : "";
            command.Parameters.Add("@ListofradiosignalsComment", SqlDbType.NVarChar).Value = Modal.ListofradiosignalsComment != null ? Modal.ListofradiosignalsComment : "";
            command.Parameters.Add("@ListoflightsComment", SqlDbType.NVarChar).Value = Modal.ListoflightsComment != null ? Modal.ListoflightsComment : "";
            command.Parameters.Add("@SailingdirectionsComment", SqlDbType.NVarChar).Value = Modal.SailingdirectionsComment != null ? Modal.SailingdirectionsComment : "";
            command.Parameters.Add("@TidetablesComment", SqlDbType.NVarChar).Value = Modal.TidetablesComment != null ? Modal.TidetablesComment : "";
            command.Parameters.Add("@NavtexandprinterComment", SqlDbType.NVarChar).Value = Modal.NavtexandprinterComment != null ? Modal.NavtexandprinterComment : "";
            command.Parameters.Add("@RadarsComment", SqlDbType.NVarChar).Value = Modal.RadarsComment != null ? Modal.RadarsComment : "";
            command.Parameters.Add("@GPSComment", SqlDbType.NVarChar).Value = Modal.GPSComment != null ? Modal.GPSComment : "";
            command.Parameters.Add("@AISComment", SqlDbType.NVarChar).Value = Modal.AISComment != null ? Modal.AISComment : "";
            command.Parameters.Add("@VDRComment", SqlDbType.NVarChar).Value = Modal.VDRComment != null ? Modal.VDRComment : "";
            command.Parameters.Add("@ECDISComment", SqlDbType.NVarChar).Value = Modal.ECDISComment != null ? Modal.ECDISComment : "";
            command.Parameters.Add("@EchosounderComment", SqlDbType.NVarChar).Value = Modal.EchosounderComment != null ? Modal.EchosounderComment : "";
            command.Parameters.Add("@ADPbackuplaptopComment", SqlDbType.NVarChar).Value = Modal.ADPbackuplaptopComment != null ? Modal.ADPbackuplaptopComment : "";
            command.Parameters.Add("@ColourprinterComment", SqlDbType.NVarChar).Value = Modal.ColourprinterComment != null ? Modal.ColourprinterComment : "";
            command.Parameters.Add("@VHFDSCtransceiverComment", SqlDbType.NVarChar).Value = Modal.VHFDSCtransceiverComment != null ? Modal.VHFDSCtransceiverComment : "";
            command.Parameters.Add("@radioinstallationComment", SqlDbType.NVarChar).Value = Modal.radioinstallationComment != null ? Modal.radioinstallationComment : "";
            command.Parameters.Add("@InmarsatCComment", SqlDbType.NVarChar).Value = Modal.InmarsatCComment != null ? Modal.InmarsatCComment : "";
            command.Parameters.Add("@MagneticcompassComment", SqlDbType.NVarChar).Value = Modal.MagneticcompassComment != null ? Modal.MagneticcompassComment : "";
            command.Parameters.Add("@SparecompassbowlComment", SqlDbType.NVarChar).Value = Modal.SparecompassbowlComment != null ? Modal.SparecompassbowlComment : "";
            command.Parameters.Add("@CompassobservationbookComment", SqlDbType.NVarChar).Value = Modal.CompassobservationbookComment != null ? Modal.CompassobservationbookComment : "";
            command.Parameters.Add("@GyrocompassComment", SqlDbType.NVarChar).Value = Modal.GyrocompassComment != null ? Modal.GyrocompassComment : "";
            command.Parameters.Add("@RudderindicatorComment", SqlDbType.NVarChar).Value = Modal.RudderindicatorComment != null ? Modal.RudderindicatorComment : "";
            command.Parameters.Add("@SpeedlogComment", SqlDbType.NVarChar).Value = Modal.SpeedlogComment != null ? Modal.SpeedlogComment : "";
            command.Parameters.Add("@NavigationComment", SqlDbType.NVarChar).Value = Modal.NavigationComment != null ? Modal.NavigationComment : "";
            command.Parameters.Add("@SignalflagsComment", SqlDbType.NVarChar).Value = Modal.SignalflagsComment != null ? Modal.SignalflagsComment : "";
            command.Parameters.Add("@RPMComment", SqlDbType.NVarChar).Value = Modal.RPMComment != null ? Modal.RPMComment : "";
            command.Parameters.Add("@BasicmanoeuvringdataComment", SqlDbType.NVarChar).Value = Modal.BasicmanoeuvringdataComment != null ? Modal.BasicmanoeuvringdataComment : "";
            command.Parameters.Add("@MasterstandingordersComment", SqlDbType.NVarChar).Value = Modal.MasterstandingordersComment != null ? Modal.MasterstandingordersComment : "";
            command.Parameters.Add("@MasternightordersbookComment", SqlDbType.NVarChar).Value = Modal.MasternightordersbookComment != null ? Modal.MasternightordersbookComment : "";
            command.Parameters.Add("@SextantComment", SqlDbType.NVarChar).Value = Modal.SextantComment != null ? Modal.SextantComment : "";
            command.Parameters.Add("@AzimuthmirrorComment", SqlDbType.NVarChar).Value = Modal.AzimuthmirrorComment != null ? Modal.AzimuthmirrorComment : "";
            command.Parameters.Add("@BridgepostersComment", SqlDbType.NVarChar).Value = Modal.BridgepostersComment != null ? Modal.BridgepostersComment : "";
            command.Parameters.Add("@ReviewofplannedComment", SqlDbType.NVarChar).Value = Modal.ReviewofplannedComment != null ? Modal.ReviewofplannedComment : "";
            command.Parameters.Add("@BridgebellbookComment", SqlDbType.NVarChar).Value = Modal.BridgebellbookComment != null ? Modal.BridgebellbookComment : "";
            command.Parameters.Add("@BridgenavigationalComment", SqlDbType.NVarChar).Value = Modal.BridgenavigationalComment != null ? Modal.BridgenavigationalComment : "";
            command.Parameters.Add("@SecurityEquipmentComment", SqlDbType.NVarChar).Value = Modal.SecurityEquipmentComment != null ? Modal.SecurityEquipmentComment : "";
            command.Parameters.Add("@NavigationPost", SqlDbType.NVarChar).Value = Modal.NavigationPost != null ? Modal.NavigationPost : "";

            command.Parameters.Add("@GeneralComment", SqlDbType.NVarChar).Value = Modal.GeneralComment != null ? Modal.GeneralComment : "";
            command.Parameters.Add("@MedicinestorageComment", SqlDbType.NVarChar).Value = Modal.MedicinestorageComment != null ? Modal.MedicinestorageComment : "";
            command.Parameters.Add("@MedicinechestcertificateComment", SqlDbType.NVarChar).Value = Modal.MedicinechestcertificateComment != null ? Modal.MedicinechestcertificateComment : "";
            command.Parameters.Add("@InventoryStoresComment", SqlDbType.NVarChar).Value = Modal.InventoryStoresComment != null ? Modal.InventoryStoresComment : "";
            command.Parameters.Add("@OxygencylindersComment", SqlDbType.NVarChar).Value = Modal.OxygencylindersComment != null ? Modal.OxygencylindersComment : "";
            command.Parameters.Add("@StretcherComment", SqlDbType.NVarChar).Value = Modal.StretcherComment != null ? Modal.StretcherComment : "";
            command.Parameters.Add("@SalivaComment", SqlDbType.NVarChar).Value = Modal.SalivaComment != null ? Modal.SalivaComment : "";
            command.Parameters.Add("@AlcoholComment", SqlDbType.NVarChar).Value = Modal.AlcoholComment != null ? Modal.AlcoholComment : "";
            command.Parameters.Add("@HospitalComment", SqlDbType.NVarChar).Value = Modal.HospitalComment != null ? Modal.HospitalComment : "";

            command.Parameters.Add("@GeneralGalleyComment", SqlDbType.NVarChar).Value = Modal.GeneralGalleyComment != null ? Modal.GeneralGalleyComment : "";
            command.Parameters.Add("@HygieneComment", SqlDbType.NVarChar).Value = Modal.HygieneComment != null ? Modal.HygieneComment : "";
            command.Parameters.Add("@FoodstorageComment", SqlDbType.NVarChar).Value = Modal.FoodstorageComment != null ? Modal.FoodstorageComment : "";
            command.Parameters.Add("@FoodlabellingComment", SqlDbType.NVarChar).Value = Modal.FoodlabellingComment != null ? Modal.FoodlabellingComment : "";
            command.Parameters.Add("@GalleyriskassessmentComment", SqlDbType.NVarChar).Value = Modal.GalleyriskassessmentComment != null ? Modal.GalleyriskassessmentComment : "";
            command.Parameters.Add("@FridgetemperatureComment", SqlDbType.NVarChar).Value = Modal.FridgetemperatureComment != null ? Modal.FridgetemperatureComment : "";
            command.Parameters.Add("@FoodandProvisionsComment", SqlDbType.NVarChar).Value = Modal.FoodandProvisionsComment != null ? Modal.FoodandProvisionsComment : "";
            command.Parameters.Add("@GalleyComment", SqlDbType.NVarChar).Value = Modal.GalleyComment != null ? Modal.GalleyComment : "";

            command.Parameters.Add("@ConditionComment", SqlDbType.NVarChar).Value = Modal.ConditionComment != null ? Modal.ConditionComment : "";
            command.Parameters.Add("@PaintworkComment", SqlDbType.NVarChar).Value = Modal.PaintworkComment != null ? Modal.PaintworkComment : "";
            command.Parameters.Add("@LightingComment", SqlDbType.NVarChar).Value = Modal.LightingComment != null ? Modal.LightingComment : "";
            command.Parameters.Add("@PlatesComment", SqlDbType.NVarChar).Value = Modal.PlatesComment != null ? Modal.PlatesComment : "";
            command.Parameters.Add("@BilgesComment", SqlDbType.NVarChar).Value = Modal.BilgesComment != null ? Modal.BilgesComment : "";
            command.Parameters.Add("@PipelinesandvalvesComment", SqlDbType.NVarChar).Value = Modal.PipelinesandvalvesComment != null ? Modal.PipelinesandvalvesComment : "";
            command.Parameters.Add("@LeakageComment", SqlDbType.NVarChar).Value = Modal.LeakageComment != null ? Modal.LeakageComment : "";
            command.Parameters.Add("@EquipmentComment", SqlDbType.NVarChar).Value = Modal.EquipmentComment != null ? Modal.EquipmentComment : "";
            command.Parameters.Add("@OilywaterseparatorComment", SqlDbType.NVarChar).Value = Modal.OilywaterseparatorComment != null ? Modal.OilywaterseparatorComment : "";
            command.Parameters.Add("@FueloiltransferplanComment", SqlDbType.NVarChar).Value = Modal.FueloiltransferplanComment != null ? Modal.FueloiltransferplanComment : "";
            command.Parameters.Add("@SteeringgearComment", SqlDbType.NVarChar).Value = Modal.SteeringgearComment != null ? Modal.SteeringgearComment : "";
            command.Parameters.Add("@WorkshopandequipmentComment", SqlDbType.NVarChar).Value = Modal.WorkshopandequipmentComment != null ? Modal.WorkshopandequipmentComment : "";
            command.Parameters.Add("@SoundingpipesComment", SqlDbType.NVarChar).Value = Modal.SoundingpipesComment != null ? Modal.SoundingpipesComment : "";
            command.Parameters.Add("@EnginecontrolComment", SqlDbType.NVarChar).Value = Modal.EnginecontrolComment != null ? Modal.EnginecontrolComment : "";
            command.Parameters.Add("@ChiefEngineernightordersbookComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineernightordersbookComment != null ? Modal.ChiefEngineernightordersbookComment : "";
            command.Parameters.Add("@ChiefEngineerstandingordersComment", SqlDbType.NVarChar).Value = Modal.ChiefEngineerstandingordersComment != null ? Modal.ChiefEngineerstandingordersComment : "";
            command.Parameters.Add("@PreUMSComment", SqlDbType.NVarChar).Value = Modal.PreUMSComment != null ? Modal.PreUMSComment : "";
            command.Parameters.Add("@EnginebellbookComment", SqlDbType.NVarChar).Value = Modal.EnginebellbookComment != null ? Modal.EnginebellbookComment : "";
            command.Parameters.Add("@LockoutComment", SqlDbType.NVarChar).Value = Modal.LockoutComment != null ? Modal.LockoutComment : "";
            command.Parameters.Add("@EngineRoomComment", SqlDbType.NVarChar).Value = Modal.EngineRoomComment != null ? Modal.EngineRoomComment : "";
            command.Parameters.Add("@CleanlinessandhygieneComment", SqlDbType.NVarChar).Value = Modal.CleanlinessandhygieneComment != null ? Modal.CleanlinessandhygieneComment : "";
            command.Parameters.Add("@ConditionComment5M", SqlDbType.NVarChar).Value = Modal.ConditionComment5M != null ? Modal.ConditionComment5M : "";
            command.Parameters.Add("@PaintworkComment5M", SqlDbType.NVarChar).Value = Modal.PaintworkComment5M != null ? Modal.PaintworkComment5M : "";
            command.Parameters.Add("@SignalmastandstaysComment", SqlDbType.NVarChar).Value = Modal.SignalmastandstaysComment != null ? Modal.SignalmastandstaysComment : "";
            command.Parameters.Add("@MonkeyislandComment", SqlDbType.NVarChar).Value = Modal.MonkeyislandComment != null ? Modal.MonkeyislandComment : "";
            command.Parameters.Add("@FireDampersComment", SqlDbType.NVarChar).Value = Modal.FireDampersComment != null ? Modal.FireDampersComment : "";
            command.Parameters.Add("@RailsBulwarksComment", SqlDbType.NVarChar).Value = Modal.RailsBulwarksComment != null ? Modal.RailsBulwarksComment : "";
            command.Parameters.Add("@WatertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WatertightdoorsComment != null ? Modal.WatertightdoorsComment : "";
            command.Parameters.Add("@VentilatorsComment", SqlDbType.NVarChar).Value = Modal.VentilatorsComment != null ? Modal.VentilatorsComment : "";
            command.Parameters.Add("@WinchesComment", SqlDbType.NVarChar).Value = Modal.WinchesComment != null ? Modal.WinchesComment : "";
            command.Parameters.Add("@FairleadsComment", SqlDbType.NVarChar).Value = Modal.FairleadsComment != null ? Modal.FairleadsComment : "";
            command.Parameters.Add("@MooringLinesComment", SqlDbType.NVarChar).Value = Modal.MooringLinesComment != null ? Modal.MooringLinesComment : "";
            command.Parameters.Add("@EmergencyShutOffsComment", SqlDbType.NVarChar).Value = Modal.EmergencyShutOffsComment != null ? Modal.EmergencyShutOffsComment : "";
            command.Parameters.Add("@RadioaerialsComment", SqlDbType.NVarChar).Value = Modal.RadioaerialsComment != null ? Modal.RadioaerialsComment : "";
            command.Parameters.Add("@SOPEPlockerComment", SqlDbType.NVarChar).Value = Modal.SOPEPlockerComment != null ? Modal.SOPEPlockerComment : "";
            command.Parameters.Add("@ChemicallockerComment", SqlDbType.NVarChar).Value = Modal.ChemicallockerComment != null ? Modal.ChemicallockerComment : "";
            command.Parameters.Add("@AntislippaintComment", SqlDbType.NVarChar).Value = Modal.AntislippaintComment != null ? Modal.AntislippaintComment : "";
            command.Parameters.Add("@SuperstructureComment", SqlDbType.NVarChar).Value = Modal.SuperstructureComment != null ? Modal.SuperstructureComment : "";
            command.Parameters.Add("@CabinsComment", SqlDbType.NVarChar).Value = Modal.CabinsComment != null ? Modal.CabinsComment : "";
            command.Parameters.Add("@OfficesComment", SqlDbType.NVarChar).Value = Modal.OfficesComment != null ? Modal.OfficesComment : "";
            command.Parameters.Add("@MessroomsComment", SqlDbType.NVarChar).Value = Modal.MessroomsComment != null ? Modal.MessroomsComment : "";
            command.Parameters.Add("@ToiletsComment", SqlDbType.NVarChar).Value = Modal.ToiletsComment != null ? Modal.ToiletsComment : "";
            command.Parameters.Add("@LaundryroomComment", SqlDbType.NVarChar).Value = Modal.LaundryroomComment != null ? Modal.LaundryroomComment : "";
            command.Parameters.Add("@ChangingroomComment", SqlDbType.NVarChar).Value = Modal.ChangingroomComment != null ? Modal.ChangingroomComment : "";
            command.Parameters.Add("@OtherComment", SqlDbType.NVarChar).Value = Modal.OtherComment != null ? Modal.OtherComment : "";
            command.Parameters.Add("@ConditionComment5N", SqlDbType.NVarChar).Value = Modal.ConditionComment5N != null ? Modal.ConditionComment5N : "";
            command.Parameters.Add("@SelfclosingfiredoorsComment", SqlDbType.NVarChar).Value = Modal.SelfclosingfiredoorsComment != null ? Modal.SelfclosingfiredoorsComment : "";
            command.Parameters.Add("@StairwellsComment", SqlDbType.NVarChar).Value = Modal.StairwellsComment != null ? Modal.StairwellsComment : "";
            command.Parameters.Add("@SuperstructureInternalComment", SqlDbType.NVarChar).Value = Modal.SuperstructureInternalComment != null ? Modal.SuperstructureInternalComment : "";
            command.Parameters.Add("@PortablegangwayComment", SqlDbType.NVarChar).Value = Modal.PortablegangwayComment != null ? Modal.PortablegangwayComment : "";
            command.Parameters.Add("@SafetynetComment", SqlDbType.NVarChar).Value = Modal.SafetynetComment != null ? Modal.SafetynetComment : "";
            command.Parameters.Add("@AccommodationLadderComment", SqlDbType.NVarChar).Value = Modal.AccommodationLadderComment != null ? Modal.AccommodationLadderComment : "";
            command.Parameters.Add("@SafeaccessprovidedComment", SqlDbType.NVarChar).Value = Modal.SafeaccessprovidedComment != null ? Modal.SafeaccessprovidedComment : "";
            command.Parameters.Add("@PilotladdersComment", SqlDbType.NVarChar).Value = Modal.PilotladdersComment != null ? Modal.PilotladdersComment : "";
            command.Parameters.Add("@BoardingEquipmentComment", SqlDbType.NVarChar).Value = Modal.BoardingEquipmentComment != null ? Modal.BoardingEquipmentComment : "";
            command.Parameters.Add("@CleanlinessComment", SqlDbType.NVarChar).Value = Modal.CleanlinessComment != null ? Modal.CleanlinessComment : "";
            command.Parameters.Add("@PaintworkComment5P", SqlDbType.NVarChar).Value = Modal.PaintworkComment5P != null ? Modal.PaintworkComment5P : "";
            command.Parameters.Add("@ShipsiderailsComment", SqlDbType.NVarChar).Value = Modal.ShipsiderailsComment != null ? Modal.ShipsiderailsComment : "";
            command.Parameters.Add("@WeathertightdoorsComment", SqlDbType.NVarChar).Value = Modal.WeathertightdoorsComment != null ? Modal.WeathertightdoorsComment : "";
            command.Parameters.Add("@FirehydrantsComment", SqlDbType.NVarChar).Value = Modal.FirehydrantsComment != null ? Modal.FirehydrantsComment : "";
            command.Parameters.Add("@VentilatorsComment5P", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5P != null ? Modal.VentilatorsComment5P : "";
            command.Parameters.Add("@ManholecoversComment", SqlDbType.NVarChar).Value = Modal.ManholecoversComment != null ? Modal.ManholecoversComment : "";
            command.Parameters.Add("@MainDeckAreaComment", SqlDbType.NVarChar).Value = Modal.MainDeckAreaComment != null ? Modal.MainDeckAreaComment : "";
            command.Parameters.Add("@ConditionComment5Q", SqlDbType.NVarChar).Value = Modal.ConditionComment5Q != null ? Modal.ConditionComment5Q : "";
            command.Parameters.Add("@PaintworkComment5Q", SqlDbType.NVarChar).Value = Modal.PaintworkComment5Q != null ? Modal.PaintworkComment5Q : "";
            command.Parameters.Add("@MechanicaldamageComment", SqlDbType.NVarChar).Value = Modal.MechanicaldamageComment != null ? Modal.MechanicaldamageComment : "";
            command.Parameters.Add("@AccessladdersComment", SqlDbType.NVarChar).Value = Modal.AccessladdersComment != null ? Modal.AccessladdersComment : "";
            command.Parameters.Add("@ManholecoversComment5Q", SqlDbType.NVarChar).Value = Modal.ManholecoversComment5Q != null ? Modal.ManholecoversComment5Q : "";
            command.Parameters.Add("@HoldbilgeComment", SqlDbType.NVarChar).Value = Modal.HoldbilgeComment != null ? Modal.HoldbilgeComment : "";
            command.Parameters.Add("@AccessdoorsComment", SqlDbType.NVarChar).Value = Modal.AccessdoorsComment != null ? Modal.AccessdoorsComment : "";
            command.Parameters.Add("@ConditionHatchCoversComment", SqlDbType.NVarChar).Value = Modal.ConditionHatchCoversComment != null ? Modal.ConditionHatchCoversComment : "";
            command.Parameters.Add("@PaintworkHatchCoversComment", SqlDbType.NVarChar).Value = Modal.PaintworkHatchCoversComment != null ? Modal.PaintworkHatchCoversComment : "";
            command.Parameters.Add("@RubbersealsComment", SqlDbType.NVarChar).Value = Modal.RubbersealsComment != null ? Modal.RubbersealsComment : "";
            command.Parameters.Add("@SignsofhatchesComment", SqlDbType.NVarChar).Value = Modal.SignsofhatchesComment != null ? Modal.SignsofhatchesComment : "";
            command.Parameters.Add("@SealingtapeComment", SqlDbType.NVarChar).Value = Modal.SealingtapeComment != null ? Modal.SealingtapeComment : "";
            command.Parameters.Add("@ConditionofhydraulicsComment", SqlDbType.NVarChar).Value = Modal.ConditionofhydraulicsComment != null ? Modal.ConditionofhydraulicsComment : "";
            command.Parameters.Add("@PortablebulkheadsComment", SqlDbType.NVarChar).Value = Modal.PortablebulkheadsComment != null ? Modal.PortablebulkheadsComment : "";
            command.Parameters.Add("@TweendecksComment", SqlDbType.NVarChar).Value = Modal.TweendecksComment != null ? Modal.TweendecksComment : "";
            command.Parameters.Add("@HatchcoamingComment", SqlDbType.NVarChar).Value = Modal.HatchcoamingComment != null ? Modal.HatchcoamingComment : "";
            command.Parameters.Add("@ConditionCargoCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionCargoCranesComment != null ? Modal.ConditionCargoCranesComment : "";
            command.Parameters.Add("@GantrycranealarmComment", SqlDbType.NVarChar).Value = Modal.GantrycranealarmComment != null ? Modal.GantrycranealarmComment : "";
            command.Parameters.Add("@GantryconditionComment", SqlDbType.NVarChar).Value = Modal.GantryconditionComment != null ? Modal.GantryconditionComment : "";
            command.Parameters.Add("@CargoHoldsComment", SqlDbType.NVarChar).Value = Modal.CargoHoldsComment != null ? Modal.CargoHoldsComment : "";
            command.Parameters.Add("@CleanlinessComment5R", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5R != null ? Modal.CleanlinessComment5R : "";
            command.Parameters.Add("@PaintworkComment5R", SqlDbType.NVarChar).Value = Modal.PaintworkComment5R != null ? Modal.PaintworkComment5R : "";
            command.Parameters.Add("@TriphazardsComment", SqlDbType.NVarChar).Value = Modal.TriphazardsComment != null ? Modal.TriphazardsComment : "";
            command.Parameters.Add("@WindlassComment", SqlDbType.NVarChar).Value = Modal.WindlassComment != null ? Modal.WindlassComment : "";
            command.Parameters.Add("@CablesComment", SqlDbType.NVarChar).Value = Modal.CablesComment != null ? Modal.CablesComment : "";
            command.Parameters.Add("@WinchesComment5R", SqlDbType.NVarChar).Value = Modal.WinchesComment5R != null ? Modal.WinchesComment5R : "";
            command.Parameters.Add("@FairleadsComment5R", SqlDbType.NVarChar).Value = Modal.FairleadsComment5R != null ? Modal.FairleadsComment5R : "";
            command.Parameters.Add("@MooringComment", SqlDbType.NVarChar).Value = Modal.MooringComment != null ? Modal.MooringComment : "";
            command.Parameters.Add("@HatchToforecastlespaceComment", SqlDbType.NVarChar).Value = Modal.HatchToforecastlespaceComment != null ? Modal.HatchToforecastlespaceComment : "";
            command.Parameters.Add("@VentilatorsComment5R", SqlDbType.NVarChar).Value = Modal.VentilatorsComment5R != null ? Modal.VentilatorsComment5R : "";
            command.Parameters.Add("@BellComment", SqlDbType.NVarChar).Value = Modal.BellComment != null ? Modal.BellComment : "";
            command.Parameters.Add("@ForemastComment", SqlDbType.NVarChar).Value = Modal.ForemastComment != null ? Modal.ForemastComment : "";
            command.Parameters.Add("@FireComment", SqlDbType.NVarChar).Value = Modal.FireComment != null ? Modal.FireComment : "";
            command.Parameters.Add("@RailsComment", SqlDbType.NVarChar).Value = Modal.RailsComment != null ? Modal.RailsComment : "";
            command.Parameters.Add("@AntislippaintComment5R", SqlDbType.NVarChar).Value = Modal.AntislippaintComment5R != null ? Modal.AntislippaintComment5R : "";


            command.Parameters.Add("@SnapBackZoneComment", SqlDbType.NVarChar).Value = Modal.SnapBackZoneComment != null ? Modal.SnapBackZoneComment : "";
            command.Parameters.Add("@ConditionGantryCranesComment", SqlDbType.NVarChar).Value = Modal.ConditionGantryCranesComment != null ? Modal.ConditionGantryCranesComment : "";
            command.Parameters.Add("@MedicalLogBookComment", SqlDbType.NVarChar).Value = Modal.MedicalLogBookComment != null ? Modal.MedicalLogBookComment : "";
            command.Parameters.Add("@DrugsNarcoticsComment", SqlDbType.NVarChar).Value = Modal.DrugsNarcoticsComment != null ? Modal.DrugsNarcoticsComment : "";
            command.Parameters.Add("@DefibrillatorComment", SqlDbType.NVarChar).Value = Modal.DefibrillatorComment != null ? Modal.DefibrillatorComment : "";
            command.Parameters.Add("@RPWaterHandbook", SqlDbType.NVarChar).Value = Modal.RPWaterHandbook != null ? Modal.RPWaterHandbook : "";
            command.Parameters.Add("@BioRPWH", SqlDbType.NVarChar).Value = Modal.BioRPWH != null ? Modal.BioRPWH : "";
            command.Parameters.Add("@PRE", SqlDbType.NVarChar).Value = Modal.PRE != null ? Modal.PRE : "";
            command.Parameters.Add("@NoiseVibrationFile", SqlDbType.NVarChar).Value = Modal.NoiseVibrationFile != null ? Modal.NoiseVibrationFile : "";
            command.Parameters.Add("@BioMPR", SqlDbType.NVarChar).Value = Modal.BioMPR != null ? Modal.BioMPR : "";
            command.Parameters.Add("@AsbestosPlan", SqlDbType.NVarChar).Value = Modal.AsbestosPlan != null ? Modal.AsbestosPlan : "";
            command.Parameters.Add("@ShipPublicAddrComment", SqlDbType.NVarChar).Value = Modal.ShipPublicAddrComment != null ? Modal.ShipPublicAddrComment : "";
            command.Parameters.Add("@BridgewindowswiperssprayComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswiperssprayComment != null ? Modal.BridgewindowswiperssprayComment : "";
            command.Parameters.Add("@BridgewindowswipersComment", SqlDbType.NVarChar).Value = Modal.BridgewindowswipersComment != null ? Modal.BridgewindowswipersComment : "";
            command.Parameters.Add("@DaylightSignalsComment", SqlDbType.NVarChar).Value = Modal.DaylightSignalsComment != null ? Modal.DaylightSignalsComment : "";
            command.Parameters.Add("@LiferaftDavitComment", SqlDbType.NVarChar).Value = Modal.LiferaftDavitComment != null ? Modal.LiferaftDavitComment : "";
            command.Parameters.Add("@CylindersLockerComment", SqlDbType.NVarChar).Value = Modal.CylindersLockerComment != null ? Modal.CylindersLockerComment : "";
            command.Parameters.Add("@ADPPublicationsComment", SqlDbType.NVarChar).Value = Modal.ADPPublicationsComment != null ? Modal.ADPPublicationsComment : "";
            command.Parameters.Add("@SnapBackZone5NComment", SqlDbType.NVarChar).Value = Modal.SnapBackZone5NComment != null ? Modal.SnapBackZone5NComment : "";

            command.Parameters.Add("@ForecastleComment", SqlDbType.NVarChar).Value = Modal.ForecastleComment != null ? Modal.ForecastleComment : "";
            command.Parameters.Add("@CleanlinessComment5S", SqlDbType.NVarChar).Value = Modal.CleanlinessComment5S != null ? Modal.CleanlinessComment5S : "";
            command.Parameters.Add("@PaintworkComment5S", SqlDbType.NVarChar).Value = Modal.PaintworkComment5S != null ? Modal.PaintworkComment5S : "";
            command.Parameters.Add("@ForepeakComment", SqlDbType.NVarChar).Value = Modal.ForepeakComment != null ? Modal.ForepeakComment : "";
            command.Parameters.Add("@ChainlockerComment", SqlDbType.NVarChar).Value = Modal.ChainlockerComment != null ? Modal.ChainlockerComment : "";
            command.Parameters.Add("@LightingComment5S", SqlDbType.NVarChar).Value = Modal.LightingComment5S != null ? Modal.LightingComment5S : "";
            command.Parameters.Add("@AccesssafetychainComment", SqlDbType.NVarChar).Value = Modal.AccesssafetychainComment != null ? Modal.AccesssafetychainComment : "";
            command.Parameters.Add("@EmergencyfirepumpComment", SqlDbType.NVarChar).Value = Modal.EmergencyfirepumpComment != null ? Modal.EmergencyfirepumpComment : "";
            command.Parameters.Add("@BowthrusterandroomComment", SqlDbType.NVarChar).Value = Modal.BowthrusterandroomComment != null ? Modal.BowthrusterandroomComment : "";
            command.Parameters.Add("@SparemooringlinesComment", SqlDbType.NVarChar).Value = Modal.SparemooringlinesComment != null ? Modal.SparemooringlinesComment : "";
            command.Parameters.Add("@PaintlockerComment", SqlDbType.NVarChar).Value = Modal.PaintlockerComment != null ? Modal.PaintlockerComment : "";
            command.Parameters.Add("@ForecastleSpaceComment", SqlDbType.NVarChar).Value = Modal.ForecastleSpaceComment != null ? Modal.ForecastleSpaceComment : "";
            command.Parameters.Add("@BoottopComment", SqlDbType.NVarChar).Value = Modal.BoottopComment != null ? Modal.BoottopComment : "";
            command.Parameters.Add("@TopsidesComment", SqlDbType.NVarChar).Value = Modal.TopsidesComment != null ? Modal.TopsidesComment : "";
            command.Parameters.Add("@AntifoulingComment", SqlDbType.NVarChar).Value = Modal.AntifoulingComment != null ? Modal.AntifoulingComment : "";
            command.Parameters.Add("@DraftandplimsollComment", SqlDbType.NVarChar).Value = Modal.DraftandplimsollComment != null ? Modal.DraftandplimsollComment : "";
            command.Parameters.Add("@FoulingComment", SqlDbType.NVarChar).Value = Modal.FoulingComment != null ? Modal.FoulingComment : "";
            command.Parameters.Add("@MechanicalComment", SqlDbType.NVarChar).Value = Modal.MechanicalComment != null ? Modal.MechanicalComment : "";
            command.Parameters.Add("@HullComment", SqlDbType.NVarChar).Value = Modal.HullComment != null ? Modal.HullComment : "";
            command.Parameters.Add("@SummaryComment", SqlDbType.NVarChar).Value = Modal.SummaryComment != null ? Modal.SummaryComment : "";
            command.Parameters.Add("@IsSynced", SqlDbType.NVarChar).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            //command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate; // == null ? DBNull.Value : (object)Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime //Modal.UpdatedDate == null ? DateTime.Now : (object)Modal.UpdatedDate;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft == null ? DBNull.Value : (object)Modal.SavedAsDraft;

            //RDBJ 10/19/2021
            command.Parameters.Add("@IsGeneralSectionComplete", SqlDbType.Bit).Value = Modal.IsGeneralSectionComplete == null ? false : true;
            command.Parameters.Add("@IsManningSectionComplete", SqlDbType.Bit).Value = Modal.IsManningSectionComplete == null ? false : true;
            command.Parameters.Add("@IsShipCertificationSectionComplete", SqlDbType.Bit).Value = Modal.IsShipCertificationSectionComplete == null ? false : true;
            command.Parameters.Add("@IsRecordKeepingSectionComplete", SqlDbType.Bit).Value = Modal.IsRecordKeepingSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSafetyEquipmentSectionComplete", SqlDbType.Bit).Value = Modal.IsSafetyEquipmentSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSecuritySectionComplete", SqlDbType.Bit).Value = Modal.IsSecuritySectionComplete == null ? false : true;
            command.Parameters.Add("@IsBridgeSectionComplete", SqlDbType.Bit).Value = Modal.IsBridgeSectionComplete == null ? false : true;
            command.Parameters.Add("@IsMedicalSectionComplete", SqlDbType.Bit).Value = Modal.IsMedicalSectionComplete == null ? false : true;
            command.Parameters.Add("@IsGalleySectionComplete", SqlDbType.Bit).Value = Modal.IsGalleySectionComplete == null ? false : true;
            command.Parameters.Add("@IsEngineRoomSectionComplete", SqlDbType.Bit).Value = Modal.IsEngineRoomSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSuperstructureSectionComplete", SqlDbType.Bit).Value = Modal.IsSuperstructureSectionComplete == null ? false : true;
            command.Parameters.Add("@IsDeckSectionComplete", SqlDbType.Bit).Value = Modal.IsDeckSectionComplete == null ? false : true;
            command.Parameters.Add("@IsHoldsAndCoverSectionComplete", SqlDbType.Bit).Value = Modal.IsHoldsAndCoverSectionComplete == null ? false : true;
            command.Parameters.Add("@IsForeCastleSectionComplete", SqlDbType.Bit).Value = Modal.IsForeCastleSectionComplete == null ? false : true;
            command.Parameters.Add("@IsHullSectionComplete", SqlDbType.Bit).Value = Modal.IsHullSectionComplete == null ? false : true;
            command.Parameters.Add("@IsSummarySectionComplete", SqlDbType.Bit).Value = Modal.IsSummarySectionComplete == null ? false : true;
            command.Parameters.Add("@IsDeficienciesSectionComplete", SqlDbType.Bit).Value = Modal.IsDeficienciesSectionComplete == null ? false : true;
            command.Parameters.Add("@IsPhotographsSectionComplete", SqlDbType.Bit).Value = Modal.IsPhotographsSectionComplete == null ? false : true;
            //End RDBJ 10/19/2021
        }
        public void GIRSafeManningRequirements_Save(Guid UniqueFormID, List<GlRSafeManningRequirements> GIRSafeManningRequirements)
        {
            bool res = DeleteRecords(AppStatic.GIRSafeManningRequirements, "UniqueFormID", Convert.ToString(UniqueFormID));
            if (res == true)
            {
                try
                {
                    if (GIRSafeManningRequirements != null && GIRSafeManningRequirements.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRSafeManningRequirements);
                        bool isTbaleCreated = true;
                        if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRSafeManningRequirements); }
                        if (isTbaleCreated)
                        {
                            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                            if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                            {
                                string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                                string InsertQuery = GIRSafeManningRequirements_InsertQuery();
                                SqlConnection connection = new SqlConnection(ConnectionString);
                                connection.Open();
                                foreach (var item in GIRSafeManningRequirements)
                                {
                                    //RDBJ 10/09/2021 Wrapped in if
                                    if (!string.IsNullOrEmpty(item.Rank))
                                    {
                                        item.UniqueFormID = UniqueFormID;
                                        item.GIRFormID = 0;
                                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                                        GIRSafeManningRequirements_CMD(item, ref command);
                                        command.ExecuteScalar();
                                    }
                                }
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Add Local DB in GIRSafeManningRequirements_Save table Error : " + ex.Message.ToString());
                    res = false;
                }
            }
        }
        public void GIRCrewDocuments_Save(Guid UniqueFormID, List<GIRCrewDocuments> GIRCrewDocuments)
        {
            bool res = DeleteRecords(AppStatic.GIRCrewDocuments, "UniqueFormID", Convert.ToString(UniqueFormID));
            if (res == true)
            {
                try
                {
                    if (GIRCrewDocuments != null && GIRCrewDocuments.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRCrewDocuments);
                        bool isTbaleCreated = true;
                        if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRCrewDocuments); }
                        if (isTbaleCreated)
                        {
                            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                            if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                            {
                                string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                                string InsertQuery = GIRCrewDocuments_InsertQuery();
                                SqlConnection connection = new SqlConnection(ConnectionString);
                                connection.Open();
                                foreach (var item in GIRCrewDocuments)
                                {
                                    //RDBJ 10/09/2021 Wrapped in if
                                    if (!string.IsNullOrEmpty(item.CrewmemberName))
                                    {
                                        item.UniqueFormID = UniqueFormID;
                                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                                        GIRCrewDocuments_CMD(item, ref command);
                                        command.ExecuteScalar();
                                    }
                                }
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Add Local DB in GIRCrewDocuments_Save table Error : " + ex.Message.ToString());
                    res = false;
                }
            }
        }
        public void GIRRestandWorkHours_Save(Guid UniqueFormID, List<GIRRestandWorkHours> GIRRestandWorkHours)
        {
            bool res = DeleteRecords(AppStatic.GIRRestandWorkHours, "UniqueFormID", Convert.ToString(UniqueFormID));
            if (res == true)
            {
                try
                {
                    if (GIRRestandWorkHours != null && GIRRestandWorkHours.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRRestandWorkHours);
                        bool isTbaleCreated = true;
                        if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRRestandWorkHours); }
                        if (isTbaleCreated)
                        {
                            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                            if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                            {
                                string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                                string InsertQuery = GIRRestandWorkHours_InsertQuery();
                                SqlConnection connection = new SqlConnection(ConnectionString);
                                connection.Open();
                                foreach (var item in GIRRestandWorkHours)
                                {
                                    //RDBJ 10/09/2021 Wrapped in if
                                    if (!string.IsNullOrEmpty(item.CrewmemberName))
                                    {
                                        item.GIRFormID = 0;
                                        item.UniqueFormID = UniqueFormID;
                                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                                        GIRRestandWorkHours_CMD(item, ref command);
                                        command.ExecuteScalar();
                                    }
                                }
                                connection.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("Add Local DB in GIRCrewDocuments_Save table Error : " + ex.Message.ToString());
                    res = false;
                }
            }
        }
        public void GIRDeficiencies_Save(Guid UniqueFormID, List<GIRDeficiencies> GIRDeficiencies)
        {
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    GIRDeficiencies defDetails = new GIRDeficiencies();
                    //bool res1 = DeleteRecords(AppStatic.GIRDeficiencies, "GIRFormID", GIRid);
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficiencies);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficiencies); }
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection connection = new SqlConnection(ConnectionString);
                    if (isTbaleCreated)
                    {
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            connection.Open();
                            foreach (var item in GIRDeficiencies)
                            {
                                defDetails = GIRDefRecordsExist(AppStatic.GIRDeficiencies, UniqueFormID, item.No, item.ReportType);
                                try
                                {
                                    if (defDetails.DeficienciesID > 0)
                                    {
                                        string UpdateQuery = GIRDeficiencies_UpdateQuery();
                                        SqlCommand command = new SqlCommand(UpdateQuery, connection);
                                        item.UniqueFormID = UniqueFormID;
                                        item.DeficienciesUniqueID = defDetails.DeficienciesUniqueID; //RDBJ 11/02/2021

                                        //RDBJ 10/12/2021 wrapped in if
                                        if (item.DateClosed != null)
                                            item.IsClose = true;
                                        else
                                            item.IsClose = false;

                                        GIRDeficiencies_UpdateCMD(item, ref command);
                                        command.ExecuteNonQuery();
                                        //GIRDeficienciesFiles_Save(item.GIRDeficienciesFile, defDetails.DeficienciesUniqueID);
                                    }
                                    else
                                    {
                                        item.CreatedDate = item.CreatedDate ?? Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                        string InsertQuery = GIRDeficiencies_InsertQuery();
                                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                                        item.UniqueFormID = UniqueFormID;
                                        item.DeficienciesUniqueID = Guid.NewGuid();

                                        //RDBJ 10/26/2021 set isClose logic and wrapped in if
                                        if (item.DateClosed != null)
                                            item.IsClose = true;
                                        else
                                            item.IsClose = false;

                                        GIRDeficiencies_CMD(item, ref command);
                                        command.ExecuteScalar();
                                        //GIRDeficienciesFiles_Save(item.GIRDeficienciesFile, item.DeficienciesUniqueID);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("Failed to add Deficiencies : " + ex.Message.ToString());

                                }
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRDeficiencies_Save table Error : " + ex.Message.ToString());
            }
        }
        public void GIRPhotos_Save(Guid UniqueFormID, List<GIRPhotographs> GIRPhotographs
            , bool IsSingleUpload = false // RDBJ2 03/12/2022
            )
        {
            bool res = true;   // RDBJ2 03/12/2022

            // RDBJ2 03/12/2022 wrapped in if
            if (!IsSingleUpload)
                res = DeleteRecords(AppStatic.GIRPhotographs, "UniqueFormID", Convert.ToString(UniqueFormID));
            
            try
            {
                if (res == true)
                {
                    //SaveGIRPhotographsDataInLocalDB(GIRPhotographs, UniqueFormID);
                    if (GIRPhotographs != null && GIRPhotographs.Count > 0 && UniqueFormID != Guid.Empty)
                    {
                        bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRPhotographs);
                        bool isTbaleCreated = true;
                        if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRPhotographs); }
                        if (isTbaleCreated)
                        {
                            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                            if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                            {
                                string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                                string InsertQuery = GIRPhotographsSave_InsertQuery();
                                SqlConnection connection = new SqlConnection(ConnectionString);
                                connection.Open();
                                foreach (var item in GIRPhotographs)
                                {
                                    item.GIRFormID = 0;
                                    item.UniqueFormID = UniqueFormID;
                                    item.CreatedDate = Utility.ToDateTimeUtcNow();
                                    item.UpdatedDate = Utility.ToDateTimeUtcNow();

                                    SqlCommand command = new SqlCommand(InsertQuery, connection);
                                    GIRPhotographsSave_CMD(item, ref command);
                                    command.ExecuteScalar();
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRPhotos_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRPhotographsSave_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRPhotographs 
                                  (GIRFormID,ImagePath,ImageCaption,CreatedDate,UpdatedDate,Ship,FileName,UniqueFormID)
                                  VALUES (@GIRFormID,@ImagePath,@ImageCaption,@CreatedDate,@UpdatedDate,@Ship,@FileName,@UniqueFormID)";
            return InsertQuery;
        }
        public void GIRPhotographsSave_CMD(GIRPhotographs Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@ImagePath", SqlDbType.NVarChar).Value = Modal.ImagePath;
            command.Parameters.Add("@ImageCaption", SqlDbType.NVarChar).Value = Modal.ImageCaption == null ? "" : (object)Modal.ImageCaption;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
        }

        public void GIRDeficienciesFiles_Save(List<GIRDeficienciesFile> DefFiles, Guid defUniqueId
            , bool IsSingleUpload = false // RDBJ2 03/12/2022
            )
        {
            try
            {
                if (DefFiles != null && DefFiles.Count > 0)
                {
                    bool res = true; // RDBJ2 03/12/2022

                    // RDBJ2 03/12/2022 wrapped in if
                    //if (!IsSingleUpload)
                    //    res = DeleteRecords(AppStatic.GIRDeficienciesFiles, "DeficienciesUniqueID", Convert.ToString(defUniqueId)); //RDBJ 10/13/2021 Set DeficienciesUniqueID and defUniqueId

                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesFiles);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesFiles); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            string InsertQuery = GIRDeficienciesFiles_InsertQuery();
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            connection.Open();
                            foreach (var item in DefFiles)
                            {
                                // RDBJ 03/12/2022 wrapped in if
                                if (Convert.ToBoolean(item.IsUpload))
                                {
                                    item.DeficienciesID = 0; //DefID; //RDBJ 10/13/2021 set with 0
                                    item.DeficienciesUniqueID = defUniqueId;
                                    SqlCommand command = new SqlCommand(InsertQuery, connection);
                                    GIRDeficienciesFiles_CMD(item, ref command);
                                    command.ExecuteScalar();
                                }
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRDeficienciesFiles_Save table Error : " + ex.Message.ToString());
            }
        }
        public string GIRSafeManningRequirements_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRSafeManningRequirements 
                                  (UniqueFormID,GIRFormID,Rank,RequiredbySMD,OnBoard,CreatedDate,UpdatedDate,Ship)
                                  OUTPUT INSERTED.SafeManningRequirementsID
                                  VALUES (@UniqueFormID,@GIRFormID,@Rank,@RequiredbySMD,@OnBoard,@CreatedDate,@UpdatedDate,@Ship)";
            return InsertQuery;
        }
        public string GIRCrewDocuments_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRCrewDocuments 
                                  (UniqueFormID,GIRFormID,CrewmemberName,CertificationDetail,CreatedDate,UpdatedDate,Ship)
                                  OUTPUT INSERTED.CrewDocumentsID
                                  VALUES (@UniqueFormID,@GIRFormID,@CrewmemberName,@CertificationDetail,@CreatedDate,@UpdatedDate,@Ship)";
            return InsertQuery;
        }
        public string GIRRestandWorkHours_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRRestandWorkHours 
                                  (UniqueFormID,GIRFormID,CrewmemberName,RestAndWorkDetail,CreatedDate,UpdatedDate,Ship)
                                  OUTPUT INSERTED.RestandWorkHoursID
                                  VALUES (@UniqueFormID,@GIRFormID,@CrewmemberName,@RestAndWorkDetail,@CreatedDate,@UpdatedDate,@Ship)";
            return InsertQuery;
        }
        public string GIRDeficiencies_InsertQuery()
        {
            // RDBJ 12/08/2021 added GIRFormID   //RDBJ 10/30/2021 Added Priority
            string InsertQuery = @"INSERT INTO dbo.GIRDeficiencies 
                                  (GIRFormID,UniqueFormID,No,DateRaised,Deficiency,DateClosed,CreatedDate,UpdatedDate,Ship,IsClose,ReportType,FileName,StorePath,SIRNo,ItemNo,Section,isDelete,DeficienciesUniqueID,[Priority])
                                  OUTPUT INSERTED.DeficienciesID
                                  VALUES (@GIRFormID,@UniqueFormID,@No,@DateRaised,@Deficiency,@DateClosed,@CreatedDate,@UpdatedDate,@Ship,@IsClose,@ReportType,@FileName,@StorePath,@SIRNo,@ItemNo,@Section,@isDelete,@DeficienciesUniqueID,@Priority)";
            return InsertQuery;
        }
        public string GIRDeficiencies_UpdateQuery()
        {
            //RDBJ 10/30/2021 Added DeficienciesUniqueID //RDBJ 10/26/2021 Added IsClose = @IsClose 
            string UpdateQuery = @"UPDATE dbo.GIRDeficiencies SET
                                DateRaised = @DateRaised, Deficiency = @Deficiency, DateClosed = @DateClosed, UpdatedDate = @UpdatedDate, IsClose = @IsClose
                                WHERE No = @No and DeficienciesUniqueID = @DeficienciesUniqueID";
            return UpdateQuery;
        }
        public string GIRDeficienciesFiles_InsertQuery()
        {
            // JSL 06/04/2022 added DeficienciesFileUniqueID
            string InsertQuery = @"INSERT INTO dbo.GIRDeficienciesFiles 
                                  (DeficienciesID,FileName,StorePath,DeficienciesUniqueID, DeficienciesFileUniqueID)
                                  OUTPUT INSERTED.GIRDeficienciesFileID
                                  VALUES (@DeficienciesID,@FileName,@StorePath,@DeficienciesUniqueID,@DeficienciesFileUniqueID)";
            return InsertQuery;
        }
        public void GIRSafeManningRequirements_CMD(GlRSafeManningRequirements Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@Rank", SqlDbType.NVarChar).Value = Modal.Rank == null ? string.Empty : Modal.Rank;
            command.Parameters.Add("@RequiredbySMD", SqlDbType.Bit).Value = Modal.RequiredbySMD == null ? false : Modal.RequiredbySMD; //RDBJ 10/08/2021 Changed RequiredbySMD from OnBoard
            command.Parameters.Add("@OnBoard", SqlDbType.Bit).Value = Modal.OnBoard == null ? false : Modal.OnBoard;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
        }
        public void GIRCrewDocuments_CMD(GIRCrewDocuments Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@CrewmemberName", SqlDbType.NVarChar).Value = Modal.CrewmemberName == null ? string.Empty : Modal.CrewmemberName;
            command.Parameters.Add("@CertificationDetail", SqlDbType.NVarChar).Value = Modal.CertificationDetail == null ? string.Empty : Modal.CertificationDetail;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
        }
        public void GIRRestandWorkHours_CMD(GIRRestandWorkHours Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@GIRFormID", SqlDbType.BigInt).Value = Modal.GIRFormID;
            command.Parameters.Add("@CrewmemberName", SqlDbType.NVarChar).Value = Modal.CrewmemberName == null ? string.Empty : Modal.CrewmemberName;
            command.Parameters.Add("@RestAndWorkDetail", SqlDbType.NVarChar).Value = Modal.RestAndWorkDetail == null ? string.Empty : Modal.RestAndWorkDetail;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
        }
        public void GIRDeficiencies_CMD(GIRDeficiencies Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@GIRFormID", SqlDbType.Int).Value = 0; // RDBJ 12/08/2021
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            command.Parameters.Add("@No", SqlDbType.Int).Value = Modal.No;

            // JSL 10/15/2022 wrapped in if
            if (Modal.DateRaised != null)
                command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Modal.DateRaised;
            else
                command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            // End JSL 10/15/2022 wrapped in if

            command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency == null ? DBNull.Value : (object)Modal.Deficiency;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
            command.Parameters.Add("@IsClose", SqlDbType.Bit).Value = Modal.IsClose == null ? false : (object)Modal.IsClose;
            command.Parameters.Add("@ReportType", SqlDbType.NVarChar).Value = Modal.ReportType;//"GI"
            //command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.Ship;
            //command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.Ship
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@SIRNo", SqlDbType.NVarChar).Value = 0;
            command.Parameters.Add("@ItemNo", SqlDbType.NVarChar).Value = Modal.ItemNo == null ? DBNull.Value : (object)Modal.ItemNo;
            command.Parameters.Add("@Section", SqlDbType.NVarChar).Value = Modal.Section == null ? DBNull.Value : (object)Modal.Section;
            command.Parameters.Add("@Priority", SqlDbType.Int).Value = Modal.Priority; //RDBJ 10/30/2021
        }
        public void GIRDeficiencies_UpdateCMD(GIRDeficiencies Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID; //RDBJ 10/30/2021 Added DeficienciesUniqueID
            command.Parameters.Add("@No", SqlDbType.Int).Value = Modal.No;
            command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Modal.DateRaised == null ? Utility.ToDateTimeUtcNow() : (object)Modal.DateRaised; // Utility.ToDateTimeUtcNow(); //RDBJ 11/29/2021 Fixed Date Raised issue //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency == null ? DBNull.Value : (object)Modal.Deficiency;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@IsClose", SqlDbType.Bit).Value = Modal.IsClose; //RDBJ 10/26/2021
        }
        public void GIRDeficienciesFiles_CMD(GIRDeficienciesFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            //command.Parameters.Add("@DeficienciesFileUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesFileUniqueID; // JSL 11/12/2022 commented // JSL 06/04/2022
            command.Parameters.Add("@DeficienciesFileUniqueID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid(); // JSL 11/12/2022
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
        }
        public bool DeleteRecords(string tablename, string columnname, string RecID)
        {
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(tablename);
                bool isTableCreated = true;
                if (!isTableExist) { isTableCreated = LocalDBHelper.CreateTable(tablename); }
                if (isTableCreated)
                {
                    bool isColumnGUID = LocalDBHelper.CheckTableColumnExist(tablename, columnname);
                    if (!isColumnGUID)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE " + tablename + " ADD " + columnname + " uniqueidentifier NULL");

                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        using (SqlConnection conn = new SqlConnection(connetionString))
                        {
                            conn.Open();
                            using (SqlCommand command = new SqlCommand("DELETE FROM " + tablename + " WHERE " + columnname + " = '" + RecID + "'", conn))
                            {
                                command.ExecuteNonQuery();
                            }
                            conn.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteRecords Local DB in table Error : " + ex.Message.ToString());
                return false;
            }
        }

        public GIRDeficiencies GIRDefRecordsExist(string tablename, Guid UniqueFormID, long No, string ReportType)
        {
            GIRDeficiencies defDetails = new GIRDeficiencies();
            try
            {
                int res = 0;
                bool isTableExist = LocalDBHelper.CheckTableExist(tablename);
                bool isTableCreated = true;
                if (!isTableExist) { isTableCreated = LocalDBHelper.CreateTable(tablename); }
                if (isTableCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        connection.Open();
                        DataTable dt1 = new DataTable();
                        string isExistRecord = "select DeficienciesID, DeficienciesUniqueID from " + AppStatic.GIRDeficiencies + " where UniqueFormID = '" + UniqueFormID + "' AND [No] = " + No + " and isDelete = 0 and ReportType = '" + ReportType.ToUpper() + "'";
                        SqlDataAdapter sqlAdp1 = new SqlDataAdapter(isExistRecord, connection);
                        sqlAdp1.Fill(dt1);
                        connection.Close(); //RDBJ 10/17/2021
                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            if (dt1.Rows[0][0] == DBNull.Value)
                            {
                                //res = 0;
                                return defDetails;
                            }
                            else
                            {
                                res = Convert.ToInt32(dt1.Rows[0][0]);
                                defDetails.DeficienciesID = Convert.ToInt32(dt1.Rows[0][0]);
                                defDetails.DeficienciesUniqueID = new Guid(Convert.ToString(dt1.Rows[0][1]));
                            }
                        }
                    }
                }
                return defDetails;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeficienciesNumber :" + ex.Message);
                return defDetails;
            }
        }
        public string CreateUpdateQuery()
        {
            string test = @"[ConditionComment]
                              ,[PaintworkComment]
                              ,[LightingComment]
                              ,[PlatesComment]
                              ,[BilgesComment]
                              ,[PipelinesandvalvesComment]
                              ,[SummaryComment]";
            List<string> liststr = test.Split(',').ToList();
            string newstr = string.Empty;
            foreach (string item in liststr)
            {
                string sss = item.Replace('[', ' ');
                sss = sss.Replace(']', ' ');
                sss = sss.Replace(@"\r\n", " ");
                newstr = newstr + sss.Trim() + " = " + "@" + sss.Trim() + ", ";
            }
            return newstr;
        }
        public bool SaveDraftDataInLocalDB(GeneralInspectionReport Modal)
        {
            bool res = false;
            long GIRid = 0;
            try
            {
                if (Modal.UniqueFormID != Guid.Empty)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string UpdateQury = GETGIRUpdateQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(UpdateQury, connection);
                        Modal.SavedAsDraft = false;
                        GIRUpdateCMD(Modal, ref command);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        //GIRid = Modal.GIRFormID;
                        //if (GIRid > 0)
                        //{
                        //    if (Modal.Manning_SafeMiningChanged == true)
                        //    {
                        //        GIRSafeManningRequirements_Save(GIRid, Modal.GIRSafeManningRequirements);
                        //    }
                        //    if (Modal.Manning_CrewDocsChanged == true)
                        //    {
                        //        GIRCrewDocuments_Save(GIRid, Modal.GIRCrewDocuments);
                        //    }
                        //    if (Modal.Manning_RestAndWorkChanged == true)
                        //    {
                        //        GIRRestandWorkHours_Save(GIRid, Modal.GIRRestandWorkHours);
                        //    }
                        if (Modal.Manning_DeficienciesChanged == true)
                        {
                            GIRDeficiencies_Save(Modal.UniqueFormID, Modal.GIRDeficiencies);
                        }
                        //    if (Modal.Manning_PhotosChanged == true)
                        //    {
                        //        GIRPhotos_Save(GIRid, Modal.GIRPhotographs);
                        //    }
                        //}
                        res = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Save Draft DataInLocalDB table Error : " + ex.Message.ToString());
            }
            return res;
        }

        public int DeficienciesNumber(string ship, string ReportType, string UniqueFormID) //RDBJ 10/30/2021 Added ReportType //RDBJ 09/18/2021 Removed Guid? UniqueFormID, string ItemNo
        {
            int nextnumber = 0;
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection connection = new SqlConnection(ConnectionString);
                    connection.Open();

                    //RDBJ 09/18/2021
                    DataTable dt1 = new DataTable();
                    string numberQuery = "select MAX(No) from " + AppStatic.GIRDeficiencies + " where [Ship]='" + ship + "' and ReportType = '" + ReportType.ToUpper() + "' and isDelete = 0 and UniqueFormID = '" + UniqueFormID + "'"; //RDBJ 10/30/2021 Added ReportType = '" + ReportType + "' and isDelete = 0
                    SqlDataAdapter sqlAdp1 = new SqlDataAdapter(numberQuery, connection);
                    sqlAdp1.Fill(dt1);
                    if (dt1 != null && dt1.Rows.Count > 0)
                    {
                        if (dt1.Rows[0][0] == DBNull.Value)
                            nextnumber = 501;
                        else
                            nextnumber = Convert.ToInt32(dt1.Rows[0][0]) + 1;
                    }
                    //End RDBJ 09/18/2021

                    //RDBJ 09/18/2021 Commented
                    /*
                    DataTable dt1 = new DataTable();
                    DataTable dt2 = new DataTable();

                    string existingDef = "select No from " + AppStatic.GIRDeficiencies + " where [Ship]='" + ship + "' and UniqueFormID='" + UniqueFormID + "' and ItemNo='" + ItemNo + "'";
                    SqlDataAdapter sqlAdp2 = new SqlDataAdapter(existingDef, connection);
                    sqlAdp2.Fill(dt2);
                    if (dt2 != null && dt2.Rows.Count > 0)
                    {

                        nextnumber = Convert.ToInt32(dt2.Rows[0][0]);
                    }
                    else
                    {
                        string numberQuery = "select MAX(No) from " + AppStatic.GIRDeficiencies + " where [Ship]='" + ship + "'";
                        SqlDataAdapter sqlAdp1 = new SqlDataAdapter(numberQuery, connection);
                        sqlAdp1.Fill(dt1);
                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            if (dt1.Rows[0][0] == DBNull.Value)
                                nextnumber = 501;
                            else
                                nextnumber = Convert.ToInt32(dt1.Rows[0][0]) + 1;
                        }
                    }
                    */
                    connection.Close(); //RDBJ 10/17/2021
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeficienciesNumber :" + ex.Message);
            }
            return nextnumber;
        }

        //RDBJ 10/30/2021
        public List<int> getDeficienciesDeletedNumbers(string ship, string ReportType, string UniqueFormID)
        {
            List<int> availableNumbers = new List<int>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection connection = new SqlConnection(ConnectionString);

                    if (CommonHelpers.StoredProcedure_ExistOrNot("SP_Get_GIDeficiencies_OR_SIActionableItems_Number"))
                    {
                        connection.Open();
                        SqlCommand command;
                        command = new SqlCommand("SP_Get_GIDeficiencies_OR_SIActionableItems_Number", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Ship", SqlDbType.VarChar).Value = ship;
                        command.Parameters.Add("@ReportType", SqlDbType.VarChar).Value = ReportType;
                        command.Parameters.Add("@UniqueFormID", SqlDbType.VarChar).Value = UniqueFormID;

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            int number;
                            number = Convert.ToInt32(sdr["AvailableNo"]);
                            availableNumbers.Add(number);
                        }

                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("getDeficienciesDeletedNumbers :" + ex.Message);
            }
            return availableNumbers;
        }
        //End RDBJ 10/30/2021

        //RDBJ 10/30/2021
        public void UpdateDeficiencyPriority_Local_DB(string DeficienciesUniqueID, int PriorityWeek)
        {
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                string UniqueFormID = string.Empty;
                string ReportType = string.Empty;

                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE GIRDeficiencies SET [Priority] = " + PriorityWeek + " WHERE DeficienciesUniqueID = '" + DeficienciesUniqueID + "'", conn))
                        {
                            command.ExecuteNonQuery();
                        }

                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT UniqueFormID, ReportType FROM " + AppStatic.GIRDeficiencies + " WHERE DeficienciesUniqueID = '" + DeficienciesUniqueID + "'", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            UniqueFormID = Convert.ToString(dt.Rows[0][0]);
                            ReportType = Convert.ToString(dt.Rows[0][1]);
                        }
                        conn.Close();
                    }

                    APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();
                    _defhelper.UpdateGIRSyncStatus_Local_DB(Convert.ToString(UniqueFormID), ReportType);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficiencyPriority_Local_DB :" + ex.Message);
            }
        }
        //End RDBJ 10/30/2021

        // RDBJ 02/19/2022
        public Dictionary<string, string> CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(Dictionary<string, string> dicMetadata)
        {
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            bool blnIsDeficiencyExist = false;
            bool blnIsDeficiencyInitialActionExist = false;
            string ItemNo = string.Empty;
            string Section = string.Empty;
            string ReportType = string.Empty;
            string Ship = string.Empty;
            Guid UniqueFormID = Guid.Empty;

            if (dicMetadata.ContainsKey("ItemNo"))
                ItemNo = dicMetadata["ItemNo"].ToString().Trim();

            if (dicMetadata.ContainsKey("Section"))
                Section = dicMetadata["Section"].ToString().Trim();

            if (dicMetadata.ContainsKey("ReportType"))
                ReportType = dicMetadata["ReportType"].ToString().Trim();

            if (dicMetadata.ContainsKey("Ship"))
                Ship = dicMetadata["Ship"].ToString().Trim();

            if (dicMetadata.ContainsKey("UniqueFormID"))
                UniqueFormID = Guid.Parse(dicMetadata["UniqueFormID"].ToString().Trim());
            try
            {
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {
                        if (UniqueFormID != null && UniqueFormID != Guid.Empty)
                        {
                            string getDefQuery = string.Empty;
                            if (!string.IsNullOrEmpty(ItemNo))
                            {
                                getDefQuery = "select DeficienciesUniqueID from " + AppStatic.GIRDeficiencies + " where isDelete = 0 and [Section]='" + Section + "' and (ItemNo='" + ItemNo + "' or ItemNo is null)  and UniqueFormID = '" + UniqueFormID.ToString() + "'";
                                conn.Open();
                                DataTable dt = new DataTable();
                                SqlDataAdapter sqlAdp = new SqlDataAdapter(getDefQuery, conn);
                                sqlAdp.Fill(dt);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    blnIsDeficiencyExist = true;
                                    dicRetMetadata["DeficienciesUniqueID"] = Convert.ToString(dt.Rows[0][0]);
                                }
                                conn.Close();

                                if (blnIsDeficiencyExist)
                                {
                                    string getInitialActionQuery = string.Empty;
                                    if (!string.IsNullOrEmpty(dicRetMetadata["DeficienciesUniqueID"].ToString()))
                                    {
                                        getInitialActionQuery = "SELECT TOP 1 [Name], [Description], [CreatedDate] FROM " + AppStatic.GIRDeficienciesInitialActions + " WHERE [DeficienciesUniqueID] = '" + dicRetMetadata["DeficienciesUniqueID"].ToString() + "' ORDER BY [CreatedDate];";
                                        conn.Open();
                                        DataTable dtInitialAction = new DataTable();
                                        SqlDataAdapter sqlAdpInitialAction = new SqlDataAdapter(getInitialActionQuery, conn);
                                        sqlAdp.Fill(dtInitialAction);
                                        if (dtInitialAction != null && dtInitialAction.Rows.Count > 0)
                                        {
                                            blnIsDeficiencyInitialActionExist = true;
                                        }
                                        conn.Close();
                                    }
                                }
                            }
                        }

                    }
                }

                dicRetMetadata["IsDeficiencyExist"] = blnIsDeficiencyExist.ToString().ToLower();
                dicRetMetadata["IsDeficiencyInitialActionExist"] = blnIsDeficiencyInitialActionExist.ToString().ToLower();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu " + ex.Message + "\n" + ex.InnerException);
            }
            return dicRetMetadata;
        }
        // End RDBJ 02/19/2022

        // RDBJ 02/20/2022
        public Dictionary<string, string> UpdateCorrectiveAction(Dictionary<string, string> dicMetadata)
        {
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();

            try
            {
                Guid IniActUniqueID = Guid.Empty;
                Guid DeficienciesUniqueID = Guid.Empty;
                string Name = string.Empty;
                string Description = string.Empty;
                string ReportType = string.Empty;

                if (dicMetadata.ContainsKey("IniActUniqueID"))
                    IniActUniqueID = Guid.Parse(dicMetadata["IniActUniqueID"].ToString().Trim());

                if (dicMetadata.ContainsKey("DeficienciesUniqueID"))
                    DeficienciesUniqueID = Guid.Parse(dicMetadata["DeficienciesUniqueID"].ToString().Trim());

                if (dicMetadata.ContainsKey("Name"))
                    Name = dicMetadata["Name"].ToString().Trim();

                if (dicMetadata.ContainsKey("Description"))
                    Description = dicMetadata["Description"].ToString().Trim();

                if (IniActUniqueID != null && IniActUniqueID != Guid.Empty)
                {
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);

                        string InsertQuery = GIRDeficiencies_InsertQuery();
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        SqlCommand command;

                        connection.Open();
                        string UpdateQury = @"UPDATE " + AppStatic.GIRDeficienciesInitialActions + " SET [Description] = @Description WHERE [IniActUniqueID] = @IniActUniqueID";
                        command = new SqlCommand(UpdateQury, connection);
                        command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = Description;
                        command.Parameters.Add("@IniActUniqueID", SqlDbType.UniqueIdentifier).Value = IniActUniqueID;
                        command.ExecuteNonQuery();

                        DataTable dt2 = new DataTable();
                        string getDefQuery = "SELECT [DeficienciesUniqueID], [UniqueFormID], [ReportType] FROM " + AppStatic.GIRDeficiencies + " WHERE isDelete = 0 and DeficienciesUniqueID = '" + DeficienciesUniqueID.ToString() + "'";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(getDefQuery, connection);
                        sqlAdp.Fill(dt2);

                        dicRetMetadata["DeficienciesUniqueID"] = dt2.Rows[0][0].ToString();
                        dicRetMetadata["UniqueFormID"] = dt2.Rows[0][1].ToString();
                        dicRetMetadata["ReportType"] = dt2.Rows[0][2].ToString();

                        connection.Close();
                    }

                    // RDBJ 02/20/2022
                    APIDeficienciesHelper objDeficienciesHelper = new APIDeficienciesHelper();
                    dicRetMetadata["FormVersion"] = objDeficienciesHelper.UpdateGIRSyncStatus_Local_DB(Convert.ToString(dicRetMetadata["UniqueFormID"]), Convert.ToString(dicRetMetadata["ReportType"]));
                    objDeficienciesHelper.UpdateGISIDeficiencyOrIAFNoteUpdatedDate_Local_DB(Convert.ToString(dicRetMetadata["DeficienciesUniqueID"]), Convert.ToString(dicRetMetadata["ReportType"]));
                    // End RDBJ 02/20/2022
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCorrectiveAction " + ex.Message + "\n" + ex.InnerException);
            }
            return dicRetMetadata;
        }
        // End RDBJ 02/20/2022

        public Dictionary<string, string> AddGIRDeficienciesInLocalDB(GIRDeficiencies Modal)
        {
            //bool res = false; // RDBJ 02/20/2022 commented this line
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();  // RDBJ 02/20/2022
            try
            {
                //if (Modal != null && Modal.GIRFormID.HasValue && Modal.GIRFormID.Value > 0)
                if (Modal.UniqueFormID != Guid.Empty)
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficiencies);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficiencies); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);

                            string InsertQuery = GIRDeficiencies_InsertQuery();
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            SqlCommand command;
                            connection.Open();
                            try
                            {
                                //Update Deficiencies
                                if (Modal.Section != "")
                                {
                                    DataTable dt2 = new DataTable();

                                    string getDefQuery = string.Empty; //RDBJ 10/30/201

                                    //RDBJ 10/30/2021 wrapped in if
                                    if (!string.IsNullOrEmpty(Modal.ItemNo))
                                        getDefQuery = "select DeficienciesUniqueID from " + AppStatic.GIRDeficiencies + " where isDelete = 0 and [Section]='" + Modal.Section + "' and (ItemNo='" + Modal.ItemNo + "')  and UniqueFormID = '" + Modal.UniqueFormID + "'";   // JSL 09/28/2022 removed or ItemNo is null //RDBJ 10/25/2021 Added isDelete = 0  //RDBJ 09/18/2021 removed and [No] = " + Modal.No + "
                                    else
                                        getDefQuery = "select DeficienciesUniqueID from " + AppStatic.GIRDeficiencies + " where isDelete = 0 and [Section]='" + Modal.Section + "' and [No] = " + Modal.No + "  and UniqueFormID = '" + Modal.UniqueFormID + "'";

                                    SqlDataAdapter sqlAdp = new SqlDataAdapter(getDefQuery, connection);
                                    sqlAdp.Fill(dt2);
                                    if (dt2 != null && dt2.Rows.Count > 0)
                                    {
                                        string UpdateQury = @"UPDATE dbo.GIRDeficiencies SET Deficiency = @Deficiency WHERE DeficienciesUniqueID = @DeficienciesUniqueID";
                                        command = new SqlCommand(UpdateQury, connection);
                                        command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency;
                                        command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = dt2.Rows[0][0];
                                        command.ExecuteNonQuery();
                                        retDictMetaData["DeficienciesUniqueID"] = dt2.Rows[0][0].ToString(); // RDBJ 02/20/2022
                                    }
                                    else
                                    {
                                        // JSL 11/12/2022 wrapped in if
                                        if (Modal.DeficienciesUniqueID == null || Modal.DeficienciesUniqueID == Guid.Empty)
                                            Modal.DeficienciesUniqueID = Guid.NewGuid();

                                        //RDBJ 10/30/2021 wrapped in if
                                        if (Modal.DateClosed != null)
                                            Modal.IsClose = true;

                                        command = new SqlCommand(InsertQuery, connection);
                                        GIRDeficiencies_CMD(Modal, ref command);
                                        command.ExecuteScalar();
                                        retDictMetaData["DeficienciesUniqueID"] = Modal.DeficienciesUniqueID.ToString(); // RDBJ 02/20/2022
                                    }

                                    if (Modal.GIRDeficienciesFile.Count > 0 && Modal.GIRDeficienciesFile != null)
                                    {
                                        GIRDeficienciesFiles_Save(Modal.GIRDeficienciesFile, Modal.DeficienciesUniqueID);
                                    }

                                    // JSL 06/27/2022
                                    if (Modal.ReportType.ToUpper() == AppStatic.SIRForm)
                                    {
                                        // JSL 09/28/2022 wrapped in if
                                        if (!string.IsNullOrEmpty(Modal.Port) && Modal.Port.ToLower() == "yes")
                                        {
                                            APIDeficienciesHelper _helper = new APIDeficienciesHelper();
                                            GIRDeficienciesInitialActions initialActions = new GIRDeficienciesInitialActions();
                                            initialActions.DeficienciesUniqueID = Modal.DeficienciesUniqueID;
                                            initialActions.Name = SessionManager.Username;
                                            initialActions.Description = string.Empty;
                                            _helper.AddDeficienciesInitialActions_Local_DB(initialActions);
                                        }
                                        // End JSL 09/28/2022 wrapped in if
                                    }
                                    // JSL 06/27/2022

                                    APIDeficienciesHelper deficienciesHelper = new APIDeficienciesHelper();
                                    retDictMetaData["FormVersion"] = deficienciesHelper.UpdateGIRSyncStatus_Local_DB(Convert.ToString(Modal.UniqueFormID), Modal.ReportType);   // RDBJ 02/20/2022 set return
                                }

                            }
                            catch (Exception ex)
                            {
                                LogHelper.writelog("Failed to add Deficiencies : " + ex.Message.ToString());
                                retDictMetaData["FormVersion"] = string.Empty;  // RDBJ 02/20/2022 set ret empty
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in AddGIRDeficiencies table Error : " + ex.Message.ToString());
            }
            return retDictMetaData;
        }

        public int DeleteGIRDeficiencies_Local_DB(Guid UniqueFormID, string ReportType, string DefID) //RDBJ 10/30/2021 added DefID
        {
            int res = 0;
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE GIRDeficiencies SET isDelete = 1 WHERE UniqueFormID = '" + UniqueFormID + "' and ReportType = '" + ReportType + "' and DeficienciesUniqueID = '" + DefID + "'", conn)) //RDBJ 10/30/2021 added DefID //RDBJ 10/30/2021 Replcae with [No] = " + DefNumber
                        {
                            command.ExecuteNonQuery();
                            res = 1;
                        }
                        conn.Close();
                    }

                    ResetGIDeficienciesOrSIActionableItemsNumbersFrom501(Convert.ToString(UniqueFormID)); // RDBJ 12/08/2021

                    APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();
                    _defhelper.UpdateGIRSyncStatus_Local_DB(Convert.ToString(UniqueFormID), ReportType); //RDBJ 09/22/2021 removed false
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteGIRDeficiencies_Local_DB :" + ex.Message);
                res = 0;
            }
            return res;
        }

        // RDBJ 12/08/2021
        public bool ResetGIDeficienciesOrSIActionableItemsNumbersFrom501(string UniqueFormID)
        {
            bool blnResponse = false;
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command;
            try
            {
                if (!string.IsNullOrEmpty(UniqueFormID))
                {
                    if (CommonHelpers.StoredProcedure_ExistOrNot("SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501"))
                    {
                        connection.Open();
                        command = new SqlCommand("SP_ResetGIDeficienciesOrSIActionableItemsNumbersFrom501", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@UniqueFormID", SqlDbType.VarChar).Value = UniqueFormID;
                        command.ExecuteReader();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ResetGIDeficienciesOrSIActionableItemsNumbersFrom501 Error : " + ex.Message.ToString());
            }
            return blnResponse;
        }
        // End RDBJ 12/08/2021


        //public int DeficienciesNumber(string ship,)
        //{
        //    int nextnumber = 0;
        //    try
        //    {
        //        //nextnumber = dbContext.GIRDeficiencies.Where(x => x.Ship == ship).Max(x => x.No);
        //        DataTable dt1 = new DataTable();
        //        SqlDataAdapter sqlAdp = new SqlDataAdapter("select MAX(No) from " + AppStatic.GIRDeficiencies + " where [Ship]=" + ship + "", connection);
        //        sqlAdp.Fill(dt1);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.writelog("DeficienciesNumber :" + ex.Message);
        //    }
        //    return nextnumber = nextnumber + 1;
        //}
        public List<GIRDataList> GetDeficienciesDataInLocalDB(string code)
        {
            List<GIRDataList> list = new List<GIRDataList>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("IsAvailable");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.GIRDeficiencies + " WHERE Ship = '" + code + "' Order By CreatedDate DESC";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var DefList = dt.ToListof<GIRDeficiencies>();
                            if (DefList != null && DefList.Count > 0)
                            {
                                Query = "SELECT GF.* FROM " + AppStatic.GIRDeficienciesFiles + " GF INNER JOIN " + AppStatic.GIRDeficiencies + " D on D.DeficienciesID = GF.DeficienciesID " +
                                    " WHERE  D.Ship IS NOT NULL AND D.Ship = '" + code + "' Order By D.CreatedDate DESC";
                                sqlAdp = new SqlDataAdapter(Query, conn);
                                DataTable dtFile = new DataTable();
                                sqlAdp.Fill(dtFile);
                                var DefFileList = dtFile.ToListof<GIRDeficienciesFile>();
                                if (DefFileList == null)
                                    DefFileList = new List<GIRDeficienciesFile>();
                                foreach (var item in DefList)
                                {
                                    GIRDataList obj = new GIRDataList();
                                    obj.GIRDeficienciesFile = DefFileList.Where(x => x.DeficienciesID == item.DeficienciesID).ToList();
                                    obj.GIRFormID = item.GIRFormID;
                                    obj.DeficienciesID = item.DeficienciesID;
                                    obj.Deficiency = item.Deficiency;
                                    obj.IsClose = item.IsClose;
                                    obj.Number = item.No != 0 ? item.No.ToString() : item.SIRNo;
                                    obj.FileName = item.FileName;
                                    obj.StorePath = item.StorePath;
                                    obj.ReportType = item.ReportType;
                                    list.Add(obj);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesData : " + ex.Message);
            }
            return list;
        }
        public RiskAssessmentFormModal GIRDeficienciesUpload(string FilePath, Guid uniqueFormId, string ShipName)
        {
            RiskAssessmentFormModal ModalList = new RiskAssessmentFormModal();
            try
            {
                try
                {
                    if (System.IO.File.Exists(FilePath))
                    {
                        GIRDeficiencies gIRDeficiencies = new GIRDeficiencies();
                        gIRDeficiencies.UniqueFormID = uniqueFormId;
                        gIRDeficiencies.ReportType = "GI";
                        gIRDeficiencies.IsClose = false;
                        gIRDeficiencies.IsSynced = false;
                        XmlDocument xml = new XmlDocument();
                        xml.Load(FilePath);
                        XmlNodeList nodes = xml.DocumentElement.ChildNodes;
                        foreach (XmlNode childNodes in nodes)
                        {
                            if (childNodes.Name == "my:Deficiencies")
                            {
                                XmlNodeList defiTag = xml.GetElementsByTagName("my:Deficiency");
                                int a = xml.GetElementsByTagName("my:Deficiency").Count;
                                foreach (XmlNode item in defiTag)
                                {
                                    foreach (XmlNode ChildItem in item.ChildNodes)
                                    {
                                        if (ChildItem.Name == "my:DefectNumber")
                                        {
                                            string Number = ChildItem.InnerText.Trim();
                                            gIRDeficiencies.No = Convert.ToInt32(Number);
                                        }
                                        else if (ChildItem.Name == "my:DefectRaisedDate")
                                        {
                                            DateTime? raisedDate = Convert.ToDateTime(ChildItem.InnerText.Trim());
                                            gIRDeficiencies.DateRaised = raisedDate;
                                        }
                                        else if (ChildItem.Name == "my:DeficiencyDescription")
                                        {
                                            string defDes = ChildItem.InnerText.Trim();
                                            gIRDeficiencies.Deficiency = defDes;
                                        }
                                    }

                                    // gIRDeficiencies.Section = "IMPORT";
                                    if (string.IsNullOrEmpty(ShipName))
                                    {
                                        gIRDeficiencies.Ship = SessionManager.ShipCode;
                                    }
                                    else
                                    {
                                        gIRDeficiencies.Ship = ShipName;
                                    }
                                    AddGIRDeficienciesInLocalDB(gIRDeficiencies);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => for : " + ex.Message);
                }
            }
            catch (Exception e)
            {
                LogHelper.writelog("GetDocumentsRiskAssesmentSubCategories => outer : " + e.Message);
            }
            return ModalList;
        }

        //RDBJ 09/17/2021 GetAllShips
        public List<CSShipsModal> GetAllShips()
        {
            List<CSShipsModal> ShipModelList = new List<CSShipsModal>();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.CSShips;
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            ShipModelList = dt.ToListof<CSShipsModal>().ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllShips : " + ex.Message);
            }
            return ShipModelList;
        }
        //End RDBJ 09/17/2021

        //RDBJ 09/17/2021 
        public CSShipsModal GetCSShipDetails(string shipCode)
        {
            CSShipsModal ShipModelList = new CSShipsModal();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.CSShips + " WHERE Code = '" + shipCode + "'";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            ShipModelList = dt.ToListof<CSShipsModal>().FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCSShipDetails : " + ex.Message);
            }
            return ShipModelList;
        }
        //End RDBJ 09/17/2021

        //RDBJ 09/17/2021
        public CSShipsModal GIRGetGeneralDescription(string shipCode)
        {
            CSShipsModal ShipModelList = new CSShipsModal();
            GeneralInspectionReport girGeneralDescriptionDetails = new GeneralInspectionReport();
            try
            {
                ShipModelList = GetCSShipDetails(shipCode);

                girGeneralDescriptionDetails.MMSI = Convert.ToString(ShipModelList.MMSI);
                girGeneralDescriptionDetails.YearofBuild = Convert.ToString(ShipModelList.BuildYear);
                girGeneralDescriptionDetails.IMOnumber = Convert.ToString(ShipModelList.IMONumber);
                girGeneralDescriptionDetails.Callsign = Convert.ToString(ShipModelList.CallSign);
                girGeneralDescriptionDetails.SummerDWT = Convert.ToString(ShipModelList.SummerDeadweight);
                girGeneralDescriptionDetails.Grosstonnage = Convert.ToString(ShipModelList.GrossTonnage);
                girGeneralDescriptionDetails.Lightweight = Convert.ToString(ShipModelList.Lightweight);
                girGeneralDescriptionDetails.Nettonnage = Convert.ToString(ShipModelList.NetTonnage);
                girGeneralDescriptionDetails.Beam = Convert.ToString(ShipModelList.Beam);
                girGeneralDescriptionDetails.LOA = Convert.ToString(ShipModelList.LOA);
                girGeneralDescriptionDetails.Summerdraft = Convert.ToString(ShipModelList.SummerDraft);
                girGeneralDescriptionDetails.LBP = Convert.ToString(ShipModelList.LBP);
                girGeneralDescriptionDetails.BHP = Convert.ToString(ShipModelList.BHP);
                girGeneralDescriptionDetails.Classsociety = Convert.ToString(ShipModelList.ClassificationSocietyId);
                girGeneralDescriptionDetails.Flag = Convert.ToString(ShipModelList.FlagStateId);
                girGeneralDescriptionDetails.Portofregistry = Convert.ToString(ShipModelList.PortOfRegistryId);
                girGeneralDescriptionDetails.Bowthruster = Convert.ToString(ShipModelList.BowThruster);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRGetGeneralDescription : " + ex.Message);
            }
            return ShipModelList;
        }
        //End RDBJ 09/17/2021

        //RDBJ 10/06/2021
        public bool GIRShipGeneralDescriptionSave(CSShipsModal modal)
        {
            bool res = false;
            try
            {
                if (modal != null)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string UpdateQury = GETCSShipsUpdateQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(UpdateQury, connection);

                        GETCSShipsUpdateCMD(modal, ref command);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRShipGeneralDescriptionSave : " + ex.Message);
            }
            return res;
        }
        //End RDBJ 10/06/2021

        //RDBJ 10/06/2021
        public string GETCSShipsUpdateQuery()
        {
            string UpdateQuery = @"UPDATE CSShips SET MMSI = @MMSI, BuildYear = @BuildYear, IMONumber = @IMONumber, CallSign = @CallSign, SummerDeadweight = @SummerDeadweight,
                                    GrossTonnage = @GrossTonnage, Lightweight = @Lightweight, NetTonnage = @NetTonnage, Beam = @Beam, LOA = @LOA, SummerDraft = @SummerDraft,
                                    LBP = @LBP, BHP = @BHP, ClassificationSocietyId = @ClassificationSocietyId, FlagStateId = @FlagStateId, PortOfRegistryId = @PortOfRegistryId, BowThruster = @BowThruster
                                    WHERE Code = @Code";
            return UpdateQuery;
        }
        //End RDBJ 10/06/2021

        //RDBJ 10/06/2021
        public void GETCSShipsUpdateCMD(CSShipsModal Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@MMSI", SqlDbType.NVarChar).Value = Modal.MMSI;
            command.Parameters.Add("@BuildYear", SqlDbType.NVarChar).Value = Modal.BuildYear;
            command.Parameters.Add("@IMONumber", SqlDbType.NVarChar).Value = Modal.IMONumber;
            command.Parameters.Add("@CallSign", SqlDbType.NVarChar).Value = Modal.CallSign != null ? Modal.CallSign : (object)Modal.CallSign; // RDBJ 01/11/2022 handle null
            command.Parameters.Add("@SummerDeadweight", SqlDbType.NVarChar).Value = Modal.SummerDeadweight;
            command.Parameters.Add("@GrossTonnage", SqlDbType.NVarChar).Value = Modal.GrossTonnage;
            command.Parameters.Add("@Lightweight", SqlDbType.NVarChar).Value = Modal.Lightweight;
            command.Parameters.Add("@NetTonnage", SqlDbType.NVarChar).Value = Modal.NetTonnage;
            command.Parameters.Add("@Beam", SqlDbType.NVarChar).Value = Modal.Beam;
            command.Parameters.Add("@LOA", SqlDbType.NVarChar).Value = Modal.LOA;
            command.Parameters.Add("@SummerDraft", SqlDbType.NVarChar).Value = Modal.SummerDraft;
            command.Parameters.Add("@LBP", SqlDbType.NVarChar).Value = Modal.LBP;
            command.Parameters.Add("@BHP", SqlDbType.NVarChar).Value = Modal.BHP;
            command.Parameters.Add("@ClassificationSocietyId", SqlDbType.NVarChar).Value = Modal.ClassificationSocietyId;
            command.Parameters.Add("@FlagStateId", SqlDbType.NVarChar).Value = Modal.FlagStateId;
            command.Parameters.Add("@PortOfRegistryId", SqlDbType.NVarChar).Value = Modal.PortOfRegistryId;
            command.Parameters.Add("@BowThruster", SqlDbType.NVarChar).Value = Modal.BowThruster;
            command.Parameters.Add("@Code", SqlDbType.NVarChar).Value = Modal.Code;
            //command.Parameters.Add("@Classofvessel", SqlDbType.NVarChar).Value = Modal.Classofvessel != null ? Modal.Classofvessel : "";
        }
        //End RDBJ 10/06/2021

        // RDBJ2 03/12/2022
        public Dictionary<string, string> AjaxPostPerformAction(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();

            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                // RDBJ2 03/12/2022
                case AppStatic.API_UPLOADGISIDEFICIENCIESFILEORPHOTO:
                    {
                        try
                        {
                            List<GIRDeficienciesFile> files = new List<GIRDeficienciesFile>();
                            string strUniqueFormID = string.Empty;
                            string strReportType = string.Empty;
                            string strDeficienciesUniqueID = string.Empty;
                            string strFileName = string.Empty;
                            string strImagePath = string.Empty;

                            // JSL 06/04/2022
                            string strDeficienciesFileUniqueID = string.Empty;

                            if (dictMetaData.ContainsKey("DeficienciesFileUniqueID"))
                                strDeficienciesFileUniqueID = dictMetaData["DeficienciesFileUniqueID"].ToString();
                            // End JSL 06/04/2022

                            if (dictMetaData.ContainsKey("DeficienciesUniqueID"))
                                strDeficienciesUniqueID = dictMetaData["DeficienciesUniqueID"].ToString();

                            if (dictMetaData.ContainsKey("FileName"))
                                strFileName = dictMetaData["FileName"].ToString();

                            if (dictMetaData.ContainsKey("ImagePath"))
                                strImagePath = dictMetaData["ImagePath"].ToString();

                            GIRDeficienciesFile file = new GIRDeficienciesFile();
                            file.DeficienciesID = 0;
                            file.DeficienciesUniqueID = Guid.Parse(strDeficienciesUniqueID);
                            file.DeficienciesFileUniqueID = Guid.Parse(strDeficienciesFileUniqueID);    // JSL 06/04/2022
                            file.FileName = strFileName;
                            file.StorePath = strImagePath;
                            file.IsUpload = "true";

                            files.Add(file);

                            GIRDeficienciesFiles_Save(files, Guid.Parse(strDeficienciesUniqueID), true);

                            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                            if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                            {
                                string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                                string InsertQuery = GIRDeficiencies_InsertQuery();
                                SqlConnection connection = new SqlConnection(ConnectionString);

                                DataTable dtDeficiency = new DataTable();
                                string getDefQuery = string.Empty;
                                getDefQuery = "SELECT UniqueFormID, ReportType FROM " + AppStatic.GIRDeficiencies + " WHERE DeficienciesUniqueID = '" + strDeficienciesUniqueID + "'";

                                connection.Open();
                                SqlDataAdapter sqlAdp = new SqlDataAdapter(getDefQuery, connection);
                                sqlAdp.Fill(dtDeficiency);
                                if (dtDeficiency != null && dtDeficiency.Rows.Count > 0)
                                {
                                    strUniqueFormID = dtDeficiency.Rows[0][0].ToString();
                                    strReportType = dtDeficiency.Rows[0][1].ToString();
                                }
                                connection.Close();

                                string strFormVersion = _defhelper.UpdateGIRSyncStatus_Local_DB(strUniqueFormID, strReportType);    // JSL 06/04/2022 set with return string
                                retDictMetaData["FormVersion"] = strFormVersion; // JSL 06/04/2022
                            }

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPLOADGISIDEFICIENCIESFILEORPHOTO + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ2 03/12/2022
                // RDBJ2 03/12/2022
                case AppStatic.API_UPLOADGIRPHOTOGRAPHS:
                    {
                        try
                        {
                            List<GIRPhotographs> files = new List<GIRPhotographs>();
                            string strUniqueFormID = string.Empty;
                            string strFileName = string.Empty;
                            string strImagePath = string.Empty;
                            string strShip = string.Empty;

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                strUniqueFormID = dictMetaData["UniqueFormID"].ToString();

                            if (dictMetaData.ContainsKey("FileName"))
                                strFileName = dictMetaData["FileName"].ToString();

                            if (dictMetaData.ContainsKey("ImagePath"))
                                strImagePath = dictMetaData["ImagePath"].ToString();

                            if (dictMetaData.ContainsKey("Ship"))
                                strShip = dictMetaData["Ship"].ToString();

                            GIRPhotographs file = new GIRPhotographs();
                            file.GIRFormID = 0;
                            file.UniqueFormID = Guid.Parse(strUniqueFormID);
                            file.FileName = strFileName;
                            file.ImagePath = strImagePath;
                            file.ImageCaption = string.Empty;
                            file.Ship = strShip;
                            file.CreatedDate = Utility.ToDateTimeUtcNow();
                            file.UpdatedDate = Utility.ToDateTimeUtcNow();

                            files.Add(file);

                            GIRPhotos_Save(Guid.Parse(strUniqueFormID), files, true);

                            string strFormVersion = _defhelper.UpdateGIRSyncStatus_Local_DB(strUniqueFormID, AppStatic.GIRForm);    // JSL 06/04/2022 set with return string
                            retDictMetaData["FormVersion"] = strFormVersion; // JSL 06/04/2022

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPLOADGIRPHOTOGRAPHS + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ2 03/12/2022
                default:
                    break;
            }

            if (IsPerformSuccess)
            {
                retDictMetaData["Status"] = AppStatic.SUCCESS;
            }
            else
            {
                retDictMetaData["Status"] = AppStatic.ERROR;
            }

            return retDictMetaData;
        }
        // End RDBJ2 03/12/2022
    }
}
