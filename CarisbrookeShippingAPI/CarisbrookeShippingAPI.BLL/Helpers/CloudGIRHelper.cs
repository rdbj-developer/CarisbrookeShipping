using CarisbrookeShippingAPI.BLL.Helpers.OfficeHelper;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using GeneralInspectionReport = CarisbrookeShippingAPI.BLL.Modals.GeneralInspectionReport;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class CloudGIRHelper
    {

        //public bool GIRSynch(Modals.GeneralInspectionReport Modal)    // JSL 09/10/2022 commented this line
        public APIResponse GIRSynch(Modals.GeneralInspectionReport Modal)   // JSL 09/10/2022
        {
            //bool res = false; // JSL 09/10/2022 commented this line
            APIResponse res = new APIResponse();    // JSL 09/10/2022
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.GeneralInspectionReport dbModal = new Entity.GeneralInspectionReport();
                bool IsNeedToUpdateSubTableData = false;    // JSL 04/20/2022
                bool blnSendNotificationToUserForForm = false;    // JSL 05/01/2022
                bool IsNeedToSendNotification = false;  // JSL 06/24/2022 this is send notification to all users

                if (Modal != null && Modal.UniqueFormID != null)
                {
                    // RDBJ 12/17/2021 wrapped in if
                    if (Modal.UniqueFormID != null)
                    {
                        dbModal = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == Modal.UniqueFormID).FirstOrDefault();

                        if (dbModal == null)
                            dbModal = new Entity.GeneralInspectionReport();

                        // JSL 04/20/2022 commented
                        /*
                        SetGIRFormData(ref dbModal, Modal);
                        dbModal.IsSynced = true; //RDBJ 11/02/2021
                        dbModal.FormVersion = Modal.FormVersion; //RDBJ 11/02/2021
                        dbModal.isDelete = (int)(Modal.isDelete); // RDBJ 01/05/2022
                        */
                        // End JSL 04/20/2022 commented

                        // JSL 12/31/2022
                        if (Modal.ShipID == null || Modal.ShipID == 0)
                        {
                            var dbships = dbContext.CSShips.Where(x => x.Code == Modal.Ship).FirstOrDefault();
                            if (dbships != null)
                            {
                                Modal.ShipID = dbships.ShipId;
                                Modal.ShipName = dbships.Name;
                            }
                        }
                        // End JSL 12/31/2022

                        if (dbModal != null && dbModal.UniqueFormID != null)
                        {
                            //dbContext.SaveChanges();  // JSL 04/20/2022 commented this line

                            // JSL 04/20/2022
                            int IsLocalFormVersionLatest = Decimal.Compare((decimal)Modal.FormVersion, (decimal)dbModal.FormVersion);
                            if (IsLocalFormVersionLatest == 1)
                            {
                                // JSL 05/01/2022
                                if (Modal.SavedAsDraft == false && dbModal.SavedAsDraft == true)
                                {
                                    blnSendNotificationToUserForForm = true;
                                }
                                // End JSL 05/01/2022

                                SetGIRFormData(ref dbModal, Modal);
                                dbModal.IsSynced = true;
                                dbModal.FormVersion = Modal.FormVersion;
                                dbModal.isDelete = (int)(Modal.isDelete); 
                                dbContext.SaveChanges();
                                IsNeedToUpdateSubTableData = true;
                            }
                            // End JSL 04/20/2022
                        }
                        else
                        {
                            // JSL 04/20/2022
                            SetGIRFormData(ref dbModal, Modal);
                            dbModal.FormVersion = Modal.FormVersion; 
                            dbModal.isDelete = (int)(Modal.isDelete);
                            // End JSL 04/20/2022

                            dbModal.UniqueFormID = Modal.UniqueFormID;
                            dbContext.GeneralInspectionReports.Add(dbModal);
                            dbContext.SaveChanges();
                            IsNeedToUpdateSubTableData = true;
                            //blnSendNotificationToUser = true;   // JSL 05/01/2022

                            // JSL 06/27/2022
                            if (dbModal.SavedAsDraft == false)
                            {
                                blnSendNotificationToUserForForm = true;
                            }
                            // End JSL 06/27/2022
                        }

                        // JSL 04/20/2022 wrapped in if
                        if (IsNeedToUpdateSubTableData)
                        {
                            GIRSafeManningRequirements_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRSafeManningRequirements);
                            GIRCrewDocuments_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRCrewDocuments);
                            GIRRestandWorkHours_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRRestandWorkHours);
                            GIRDeficiencies_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRDeficiencies
                                , ref IsNeedToSendNotification  // JSL 06/24/2022
                                );
                            //GIRPhotos_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRPhotographs); // JSL 01/03/2023 commented
                        }
                        // End JSL 04/20/2022 wrapped in if

                        // JSL 05/01/2022
                        if (blnSendNotificationToUserForForm)
                        {
                            // JSL 06/24/2022
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "5";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(dbModal.UniqueFormID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeForm;
                            dictNotificationData["IsDraft"] = Convert.ToString(dbModal.SavedAsDraft);
                            dictNotificationData["Title"] = AppStatic.GIRFormName;

                            string strDetailsURL = string.Empty;
                            if ((bool)dbModal.SavedAsDraft)
                            {
                                strDetailsURL = "GIRList/Index?id=" + Convert.ToString(dbModal.UniqueFormID);
                            }
                            else
                            {
                                strDetailsURL = "Forms/GeneralInspectionReport";
                            }
                            dictNotificationData["DetailsURL"] = strDetailsURL;

                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData);
                            // End JSL 06/24/2022

                            // JSL 06/24/2022 commented
                            /*
                            var UsersList = dbContext.UserProfiles
                                .Where(x => x.UserGroup == 1    // 1 for Admin group
                                || x.UserGroup == 5 // 5 for ISM group
                                )
                                .ToList();

                            // send to user
                            foreach (var item in UsersList)
                            {
                                Entity.Notification entityModelNotification = new Entity.Notification();
                                entityModelNotification.UniqueId = Guid.NewGuid();
                                entityModelNotification.UniqueDataId = dbModal.UniqueFormID;
                                entityModelNotification.DataType = AppStatic.NotificationTypeForm;
                                entityModelNotification.IsDraft = (bool)dbModal.SavedAsDraft;
                                entityModelNotification.Title = AppStatic.GIRFormName;

                                if (entityModelNotification.IsDraft)
                                {
                                    entityModelNotification.DetailsURL = "GIRList/Index?id=" + entityModelNotification.UniqueDataId.ToString();
                                }
                                else
                                {
                                    entityModelNotification.DetailsURL = "Forms/GeneralInspectionReport";
                                }

                                entityModelNotification.SentToUserId = item.UserID;
                                entityModelNotification.UserGroup = item.UserGroup;
                                entityModelNotification.CreatedDateTime = Utility.ToDateTimeUtcNow();
                                dbContext.Notifications.Add(entityModelNotification);
                                dbContext.SaveChanges();
                            }
                            */
                            // End JSL 06/24/2022 commented
                        }
                        // End JSL 05/01/2022

                        // JSL 06/24/2022
                        if (blnSendNotificationToUserForForm || IsNeedToSendNotification)
                        {
                            SendSignalRNotificationCallForTheOffice(blnSendNotificationToUserForForm: blnSendNotificationToUserForForm);
                        }
                        // End JSL 06/24/2022

                        //res = true;   // JSL 09/10/2022 commented this line
                        //res.result = "true";  // JSL 09/12/2022 commented this line    // JSL 09/10/2022
                        res.result = AppStatic.SUCCESS;    // JSL 09/12/2022
                    }
                }
            }
            catch (Exception ex)
            {
                //res.result = "false";   // JSL 09/12/2022 commented this line
                res.result = AppStatic.ERROR;   // JSL 09/12/2022
                res.msg = ex.Message;
                LogHelper.writelog("Cloud SubmitGIR : " + ex.Message);
            }
            return res;
        }
        public void SetGIRFormData(ref Entity.GeneralInspectionReport dbModal, Modals.GeneralInspectionReport Modal)
        {
            dbModal.CreatedDate = Modal.CreatedDate;
            dbModal.UpdatedDate = Modal.UpdatedDate;

            dbModal.ShipID = Modal.ShipID;
            dbModal.ShipName = Modal.ShipName;
            dbModal.Ship = Modal.Ship;
            dbModal.Port = Modal.Port;
            dbModal.Inspector = Modal.Inspector;
            dbModal.Date = Modal.Date;
            dbModal.GeneralPreamble = Modal.GeneralPreamble;
            dbModal.Classsociety = Modal.Classsociety;
            dbModal.YearofBuild = Modal.YearofBuild;
            dbModal.Flag = Modal.Flag;
            dbModal.Classofvessel = Modal.Classofvessel;
            dbModal.Portofregistry = Modal.Portofregistry;
            dbModal.MMSI = Modal.MMSI;
            dbModal.IMOnumber = Modal.IMOnumber;
            dbModal.Callsign = Modal.Callsign;
            dbModal.SummerDWT = Modal.SummerDWT;
            dbModal.Grosstonnage = Modal.Grosstonnage;
            dbModal.Lightweight = Modal.Lightweight;
            dbModal.Nettonnage = Modal.Nettonnage;
            dbModal.Beam = Modal.Beam;
            dbModal.LOA = Modal.LOA;
            dbModal.Summerdraft = Modal.Summerdraft;
            dbModal.LBP = Modal.LBP;
            dbModal.Bowthruster = Modal.Bowthruster;
            dbModal.BHP = Modal.BHP;
            dbModal.Noofholds = Modal.Noofholds;
            dbModal.Nomoveablebulkheads = Modal.Nomoveablebulkheads;
            dbModal.Containers = Modal.Containers;
            dbModal.Cargocapacity = Modal.Cargocapacity;
            dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
            dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
            dbModal.Lastvoyageandcargo = Modal.Lastvoyageandcargo;
            dbModal.CurrentPlannedvoyageandcargo = Modal.CurrentPlannedvoyageandcargo;
            dbModal.ShipboardWorkingArrangements = Modal.ShipboardWorkingArrangements;
            dbModal.CertificationIndex = Modal.CertificationIndex;
            dbModal.IsPubsAndDocsSectionComplete = Modal.IsPubsAndDocsSectionComplete;
            dbModal.CarriedOutByTheDOOW = Modal.CarriedOutByTheDOOW;
            dbModal.IsRegs4shipsDVD = Modal.IsRegs4shipsDVD;
            dbModal.Regs4shipsDVD = Modal.Regs4shipsDVD;
            dbModal.IsSOPEPPoints = Modal.IsSOPEPPoints;
            dbModal.SOPEPPoints = Modal.SOPEPPoints;
            dbModal.IsBWMP = Modal.IsBWMP;
            dbModal.BWMP = Modal.BWMP;
            dbModal.IsBWMPSupplement = Modal.IsBWMPSupplement;
            dbModal.BWMPSupplement = Modal.BWMPSupplement;
            dbModal.IsIntactStabilityManual = Modal.IsIntactStabilityManual;
            dbModal.IntactStabilityManual = Modal.IntactStabilityManual;
            dbModal.IsStabilityComputer = Modal.IsStabilityComputer;
            dbModal.StabilityComputer = Modal.StabilityComputer;
            dbModal.IsDateOfLast = Modal.IsDateOfLast;
            dbModal.DateOfLast = Modal.DateOfLast;
            dbModal.IsCargoSecuring = Modal.IsCargoSecuring;
            dbModal.CargoSecuring = Modal.CargoSecuring;
            dbModal.BulkCargo = Modal.BulkCargo;
            dbModal.IBulkCargo = Modal.IsBulkCargo;
            dbModal.IsSMSManual = Modal.IsSMSManual;
            dbModal.SMSManual = Modal.SMSManual;
            dbModal.IsRegisterOf = Modal.IsRegisterOf;
            dbModal.RegisterOf = Modal.RegisterOf;
            dbModal.IsFleetStandingOrder = Modal.IsFleetStandingOrder;
            dbModal.FleetStandingOrder = Modal.FleetStandingOrder;
            dbModal.IsFleetMemoranda = Modal.IsFleetMemoranda;
            dbModal.FleetMemoranda = Modal.FleetMemoranda;
            dbModal.IsShipsPlans = Modal.IsShipsPlans;
            dbModal.ShipsPlans = Modal.ShipsPlans;
            dbModal.IsCollective = Modal.IsCollective;
            dbModal.Collective = Modal.Collective;
            dbModal.IsDraftAndFreeboardNotice = Modal.IsDraftAndFreeboardNotice;
            dbModal.DraftAndFreeboardNotice = Modal.DraftAndFreeboardNotice;
            dbModal.IsPCSOPEP = Modal.IsPCSOPEP;
            dbModal.PCOPEP = Modal.PCSOPEP;
            dbModal.INTVRP = Modal.IsNTVRP;
            dbModal.NTVRP = Modal.NTVRP;
            dbModal.IVGP = Modal.IsVGP;
            dbModal.VGP = Modal.VGP;
            dbModal.PubsComments = Modal.PubsComments;
            dbModal.OfficialLogbookA = Modal.OfficialLogbookA;
            dbModal.OfficialLogbookB = Modal.OfficialLogbookB;
            dbModal.OfficialLogbookC = Modal.OfficialLogbookC;
            dbModal.OfficialLogbookD = Modal.OfficialLogbookD;
            dbModal.OfficialLogbookE = Modal.OfficialLogbookE;
            dbModal.DeckLogbook = Modal.DeckLogbook;
            dbModal.Listofcrew = Modal.Listofcrew;
            dbModal.LastHose = Modal.LastHose;
            dbModal.PassagePlanning = Modal.PassagePlanning;
            dbModal.LoadingComputer = Modal.LoadingComputer;
            dbModal.EngineLogbook = Modal.EngineLogbook;
            dbModal.OilRecordBook = Modal.OilRecordBook;
            dbModal.RiskAssessments = Modal.RiskAssessments;
            dbModal.GMDSSLogbook = Modal.GMDSSLogbook;
            dbModal.DeckLogbook5D = Modal.DeckLogbook5D;
            dbModal.GarbageRecordBook = Modal.GarbageRecordBook;
            dbModal.BallastWaterRecordBook = Modal.BallastWaterRecordBook;
            dbModal.CargoRecordBook = Modal.CargoRecordBook;
            dbModal.EmissionsControlManual = Modal.EmissionsControlManual;
            dbModal.LGR = Modal.LGR;
            dbModal.PEER = Modal.PEER;
            dbModal.RecordKeepingComments = Modal.RecordKeepingComments;
            dbModal.LastPortStateControl = Modal.LastPortStateControl;

            dbModal.LiferaftsComment = Modal.LiferaftsComment;
            dbModal.releasesComment = Modal.releasesComment;
            dbModal.LifeboatComment = Modal.LifeboatComment;
            dbModal.LifeboatdavitComment = Modal.LifeboatdavitComment;
            dbModal.LifeboatequipmentComment = Modal.LifeboatequipmentComment;
            dbModal.RescueboatComment = Modal.RescueboatComment;
            dbModal.RescueboatequipmentComment = Modal.RescueboatequipmentComment;
            dbModal.RescueboatoutboardmotorComment = Modal.RescueboatoutboardmotorComment;
            dbModal.RescueboatdavitComment = Modal.RescueboatdavitComment;
            dbModal.DeckComment = Modal.DeckComment;
            dbModal.PyrotechnicsComment = Modal.PyrotechnicsComment;
            dbModal.EPIRBComment = Modal.EPIRBComment;
            dbModal.SARTsComment = Modal.SARTsComment;
            dbModal.GMDSSComment = Modal.GMDSSComment;
            dbModal.ManoverboardComment = Modal.ManoverboardComment;
            dbModal.LinethrowingapparatusComment = Modal.LinethrowingapparatusComment;
            dbModal.FireextinguishersComment = Modal.FireextinguishersComment;
            dbModal.EmergencygeneratorComment = Modal.EmergencygeneratorComment;
            dbModal.CO2roomComment = Modal.CO2roomComment;
            dbModal.SurvivalComment = Modal.SurvivalComment;
            dbModal.LifejacketComment = Modal.LifejacketComment;
            dbModal.FiremansComment = Modal.FiremansComment;
            dbModal.LifebuoysComment = Modal.LifebuoysComment;
            dbModal.FireboxesComment = Modal.FireboxesComment;
            dbModal.EmergencybellsComment = Modal.EmergencybellsComment;
            dbModal.EmergencylightingComment = Modal.EmergencylightingComment;
            dbModal.FireplanComment = Modal.FireplanComment;
            dbModal.DamageComment = Modal.DamageComment;
            dbModal.EmergencyplansComment = Modal.EmergencyplansComment;
            dbModal.MusterlistComment = Modal.MusterlistComment;
            dbModal.SafetysignsComment = Modal.SafetysignsComment;
            dbModal.EmergencysteeringComment = Modal.EmergencysteeringComment;
            dbModal.StatutoryemergencydrillsComment = Modal.StatutoryemergencydrillsComment;
            dbModal.EEBDComment = Modal.EEBDComment;
            dbModal.OxygenComment = Modal.OxygenComment;
            dbModal.MultigasdetectorComment = Modal.MultigasdetectorComment;
            dbModal.GasdetectorComment = Modal.GasdetectorComment;
            dbModal.SufficientquantityComment = Modal.SufficientquantityComment;
            dbModal.BASetsComment = Modal.BASetsComment;
            dbModal.SafetyComment = Modal.SafetyComment;

            dbModal.GangwayComment = Modal.GangwayComment;
            dbModal.RestrictedComment = Modal.RestrictedComment;
            dbModal.OutsideComment = Modal.OutsideComment;
            dbModal.EntrancedoorsComment = Modal.EntrancedoorsComment;
            dbModal.AccommodationComment = Modal.AccommodationComment;
            dbModal.GMDSSComment5G = Modal.GMDSSComment5G;
            dbModal.VariousComment = Modal.VariousComment;
            dbModal.SSOComment = Modal.SSOComment;
            dbModal.SecuritylogbookComment = Modal.SecuritylogbookComment;
            dbModal.Listoflast10portsComment = Modal.Listoflast10portsComment;
            dbModal.PFSOComment = Modal.PFSOComment;
            dbModal.SecuritylevelComment = Modal.SecuritylevelComment;
            dbModal.DrillsandtrainingComment = Modal.DrillsandtrainingComment;
            dbModal.DOSComment = Modal.DOSComment;
            dbModal.SSASComment = Modal.SSASComment;
            dbModal.VisitorslogbookComment = Modal.VisitorslogbookComment;
            dbModal.KeyregisterComment = Modal.KeyregisterComment;
            dbModal.ShipSecurityComment = Modal.ShipSecurityComment;
            dbModal.SecurityComment = Modal.SecurityComment;

            dbModal.NauticalchartsComment = Modal.NauticalchartsComment;
            dbModal.NoticetomarinersComment = Modal.NoticetomarinersComment;
            dbModal.ListofradiosignalsComment = Modal.ListofradiosignalsComment;
            dbModal.ListoflightsComment = Modal.ListoflightsComment;
            dbModal.SailingdirectionsComment = Modal.SailingdirectionsComment;
            dbModal.TidetablesComment = Modal.TidetablesComment;
            dbModal.NavtexandprinterComment = Modal.NavtexandprinterComment;
            dbModal.RadarsComment = Modal.RadarsComment;
            dbModal.GPSComment = Modal.GPSComment;
            dbModal.AISComment = Modal.AISComment;
            dbModal.VDRComment = Modal.VDRComment;
            dbModal.ECDISComment = Modal.ECDISComment;
            dbModal.EchosounderComment = Modal.EchosounderComment;
            dbModal.ADPbackuplaptopComment = Modal.ADPbackuplaptopComment;
            dbModal.ColourprinterComment = Modal.ColourprinterComment;
            dbModal.VHFDSCtransceiverComment = Modal.VHFDSCtransceiverComment;
            dbModal.radioinstallationComment = Modal.radioinstallationComment;
            dbModal.InmarsatCComment = Modal.InmarsatCComment;
            dbModal.MagneticcompassComment = Modal.MagneticcompassComment;
            dbModal.SparecompassbowlComment = Modal.SparecompassbowlComment;
            dbModal.CompassobservationbookComment = Modal.CompassobservationbookComment;
            dbModal.GyrocompassComment = Modal.GyrocompassComment;
            dbModal.RudderindicatorComment = Modal.RudderindicatorComment;
            dbModal.SpeedlogComment = Modal.SpeedlogComment;
            dbModal.NavigationComment = Modal.NavigationComment;
            dbModal.SignalflagsComment = Modal.SignalflagsComment;
            dbModal.RPMComment = Modal.RPMComment;
            dbModal.BasicmanoeuvringdataComment = Modal.BasicmanoeuvringdataComment;
            dbModal.MasterstandingordersComment = Modal.MasterstandingordersComment;
            dbModal.MasternightordersbookComment = Modal.MasternightordersbookComment;
            dbModal.SextantComment = Modal.SextantComment;
            dbModal.AzimuthmirrorComment = Modal.AzimuthmirrorComment;
            dbModal.BridgepostersComment = Modal.BridgepostersComment;
            dbModal.ReviewofplannedComment = Modal.ReviewofplannedComment;
            dbModal.BridgebellbookComment = Modal.BridgebellbookComment;
            dbModal.BridgenavigationalComment = Modal.BridgenavigationalComment;
            dbModal.SecurityEquipmentComment = Modal.SecurityEquipmentComment;
            dbModal.NavigationPost = Modal.NavigationPost;

            dbModal.GeneralComment = Modal.GeneralComment;
            dbModal.MedicinestorageComment = Modal.MedicinestorageComment;
            dbModal.MedicinechestcertificateComment = Modal.MedicinechestcertificateComment;
            dbModal.InventoryStoresComment = Modal.InventoryStoresComment;
            dbModal.OxygencylindersComment = Modal.OxygencylindersComment;
            dbModal.StretcherComment = Modal.StretcherComment;
            dbModal.SalivaComment = Modal.SalivaComment;
            dbModal.AlcoholComment = Modal.AlcoholComment;
            dbModal.HospitalComment = Modal.HospitalComment;

            dbModal.GeneralGalleyComment = Modal.GeneralGalleyComment;
            dbModal.HygieneComment = Modal.HygieneComment;
            dbModal.FoodstorageComment = Modal.FoodstorageComment;
            dbModal.FoodlabellingComment = Modal.FoodlabellingComment;
            dbModal.GalleyriskassessmentComment = Modal.GalleyriskassessmentComment;
            dbModal.FridgetemperatureComment = Modal.FridgetemperatureComment;
            dbModal.FoodandProvisionsComment = Modal.FoodandProvisionsComment;
            dbModal.GalleyComment = Modal.GalleyComment;

            dbModal.ConditionComment = Modal.ConditionComment;
            dbModal.PaintworkComment = Modal.PaintworkComment;
            dbModal.LightingComment = Modal.LightingComment;
            dbModal.PlatesComment = Modal.PlatesComment;
            dbModal.BilgesComment = Modal.BilgesComment;
            dbModal.PipelinesandvalvesComment = Modal.PipelinesandvalvesComment;
            dbModal.LeakageComment = Modal.LeakageComment;
            dbModal.EquipmentComment = Modal.EquipmentComment;
            dbModal.OilywaterseparatorComment = Modal.OilywaterseparatorComment;
            dbModal.FueloiltransferplanComment = Modal.FueloiltransferplanComment;
            dbModal.SteeringgearComment = Modal.SteeringgearComment;
            dbModal.WorkshopandequipmentComment = Modal.WorkshopandequipmentComment;
            dbModal.SoundingpipesComment = Modal.SoundingpipesComment;
            dbModal.EnginecontrolComment = Modal.EnginecontrolComment;
            dbModal.ChiefEngineernightordersbookComment = Modal.ChiefEngineernightordersbookComment;
            dbModal.ChiefEngineerstandingordersComment = Modal.ChiefEngineerstandingordersComment;
            dbModal.PreUMSComment = Modal.PreUMSComment;
            dbModal.EnginebellbookComment = Modal.EnginebellbookComment;
            dbModal.LockoutComment = Modal.LockoutComment;
            dbModal.EngineRoomComment = Modal.EngineRoomComment;

            dbModal.CleanlinessandhygieneComment = Modal.CleanlinessandhygieneComment;
            dbModal.ConditionComment5M = Modal.ConditionComment5M;
            dbModal.PaintworkComment5M = Modal.PaintworkComment5M;
            dbModal.SignalmastandstaysComment = Modal.SignalmastandstaysComment;
            dbModal.MonkeyislandComment = Modal.MonkeyislandComment;
            dbModal.FireDampersComment = Modal.FireDampersComment;
            dbModal.RailsBulwarksComment = Modal.RailsBulwarksComment;
            dbModal.WatertightdoorsComment = Modal.WatertightdoorsComment;
            dbModal.VentilatorsComment = Modal.VentilatorsComment;
            dbModal.WinchesComment = Modal.WinchesComment;
            dbModal.FairleadsComment = Modal.FairleadsComment;
            dbModal.MooringLinesComment = Modal.MooringLinesComment;
            dbModal.EmergencyShutOffsComment = Modal.EmergencyShutOffsComment;
            dbModal.RadioaerialsComment = Modal.RadioaerialsComment;
            dbModal.SOPEPlockerComment = Modal.SOPEPlockerComment;
            dbModal.ChemicallockerComment = Modal.ChemicallockerComment;
            dbModal.AntislippaintComment = Modal.AntislippaintComment;
            dbModal.SuperstructureComment = Modal.SuperstructureComment;
            dbModal.CabinsComment = Modal.CabinsComment;
            dbModal.OfficesComment = Modal.OfficesComment;
            dbModal.MessroomsComment = Modal.MessroomsComment;
            dbModal.ToiletsComment = Modal.ToiletsComment;
            dbModal.LaundryroomComment = Modal.LaundryroomComment;
            dbModal.ChangingroomComment = Modal.ChangingroomComment;
            dbModal.OtherComment = Modal.OtherComment;
            dbModal.ConditionComment5N = Modal.ConditionComment5N;
            dbModal.SelfclosingfiredoorsComment = Modal.SelfclosingfiredoorsComment;
            dbModal.StairwellsComment = Modal.StairwellsComment;
            dbModal.SuperstructureInternalComment = Modal.SuperstructureInternalComment;

            dbModal.PortablegangwayComment = Modal.PortablegangwayComment;
            dbModal.SafetynetComment = Modal.SafetynetComment;
            dbModal.AccommodationLadderComment = Modal.AccommodationLadderComment;
            dbModal.SafeaccessprovidedComment = Modal.SafeaccessprovidedComment;
            dbModal.PilotladdersComment = Modal.PilotladdersComment;
            dbModal.BoardingEquipmentComment = Modal.BoardingEquipmentComment;
            dbModal.CleanlinessComment = Modal.CleanlinessComment;
            dbModal.PaintworkComment5P = Modal.PaintworkComment5P;
            dbModal.ShipsiderailsComment = Modal.ShipsiderailsComment;
            dbModal.WeathertightdoorsComment = Modal.WeathertightdoorsComment;
            dbModal.FirehydrantsComment = Modal.FirehydrantsComment;
            dbModal.VentilatorsComment5P = Modal.VentilatorsComment5P;
            dbModal.ManholecoversComment = Modal.ManholecoversComment;
            dbModal.MainDeckAreaComment = Modal.MainDeckAreaComment;

            dbModal.ConditionComment5Q = Modal.ConditionComment5Q;
            dbModal.PaintworkComment5Q = Modal.PaintworkComment5Q;
            dbModal.MechanicaldamageComment = Modal.MechanicaldamageComment;
            dbModal.AccessladdersComment = Modal.AccessladdersComment;
            dbModal.ManholecoversComment5Q = Modal.ManholecoversComment5Q;
            dbModal.HoldbilgeComment = Modal.HoldbilgeComment;
            dbModal.AccessdoorsComment = Modal.AccessdoorsComment;
            dbModal.ConditionHatchCoversComment = Modal.ConditionHatchCoversComment;
            dbModal.PaintworkHatchCoversComment = Modal.PaintworkHatchCoversComment;
            dbModal.RubbersealsComment = Modal.RubbersealsComment;
            dbModal.SignsofhatchesComment = Modal.SignsofhatchesComment;
            dbModal.SealingtapeComment = Modal.SealingtapeComment;
            dbModal.ConditionofhydraulicsComment = Modal.ConditionofhydraulicsComment;
            dbModal.PortablebulkheadsComment = Modal.PortablebulkheadsComment;
            dbModal.TweendecksComment = Modal.TweendecksComment;
            dbModal.HatchcoamingComment = Modal.HatchcoamingComment;
            dbModal.ConditionCargoCranesComment = Modal.ConditionCargoCranesComment;
            dbModal.GantrycranealarmComment = Modal.GantrycranealarmComment;
            dbModal.GantryconditionComment = Modal.GantryconditionComment;
            dbModal.CargoHoldsComment = Modal.CargoHoldsComment;

            dbModal.CleanlinessComment5R = Modal.CleanlinessComment5R;
            dbModal.PaintworkComment5R = Modal.PaintworkComment5R;
            dbModal.TriphazardsComment = Modal.TriphazardsComment;
            dbModal.WindlassComment = Modal.WindlassComment;
            dbModal.CablesComment = Modal.CablesComment;
            dbModal.WinchesComment5R = Modal.WinchesComment5R;
            dbModal.FairleadsComment5R = Modal.FairleadsComment5R;
            dbModal.MooringComment = Modal.MooringComment;
            dbModal.HatchToforecastlespaceComment = Modal.HatchToforecastlespaceComment;
            dbModal.VentilatorsComment5R = Modal.VentilatorsComment5R;
            dbModal.BellComment = Modal.BellComment;
            dbModal.ForemastComment = Modal.ForemastComment;
            dbModal.FireComment = Modal.FireComment;
            dbModal.RailsComment = Modal.RailsComment;
            dbModal.AntislippaintComment5R = Modal.AntislippaintComment5R;
            dbModal.ForecastleComment = Modal.ForecastleComment;
            dbModal.CleanlinessComment5S = Modal.CleanlinessComment5S;
            dbModal.PaintworkComment5S = Modal.PaintworkComment5S;
            dbModal.ForepeakComment = Modal.ForepeakComment;
            dbModal.ChainlockerComment = Modal.ChainlockerComment;
            dbModal.LightingComment5S = Modal.LightingComment5S;
            dbModal.AccesssafetychainComment = Modal.AccesssafetychainComment;
            dbModal.EmergencyfirepumpComment = Modal.EmergencyfirepumpComment;
            dbModal.BowthrusterandroomComment = Modal.BowthrusterandroomComment;
            dbModal.SparemooringlinesComment = Modal.SparemooringlinesComment;
            dbModal.PaintlockerComment = Modal.PaintlockerComment;
            dbModal.ForecastleSpaceComment = Modal.ForecastleSpaceComment;

            dbModal.BoottopComment = Modal.BoottopComment;
            dbModal.TopsidesComment = Modal.TopsidesComment;
            dbModal.AntifoulingComment = Modal.AntifoulingComment;
            dbModal.DraftandplimsollComment = Modal.DraftandplimsollComment;
            dbModal.FoulingComment = Modal.FoulingComment;
            dbModal.MechanicalComment = Modal.MechanicalComment;
            dbModal.HullComment = Modal.HullComment;
            dbModal.SummaryComment = Modal.SummaryComment;
            dbModal.IsSynced = Modal.IsSynced;
            dbModal.SavedAsDraft = Modal.SavedAsDraft;

            dbModal.SnapBackZoneComment = Modal.SnapBackZoneComment;
            dbModal.ConditionGantryCranesComment = Modal.ConditionGantryCranesComment;
            dbModal.CylindersLockerComment = Modal.CylindersLockerComment;
            dbModal.MedicalLogBookComment = Modal.MedicalLogBookComment;
            dbModal.DrugsNarcoticsComment = Modal.DrugsNarcoticsComment;
            dbModal.DefibrillatorComment = Modal.DefibrillatorComment;
            dbModal.RPWaterHandbook = Modal.RPWaterHandbook;
            dbModal.BioRPWH = Modal.BioRPWH;
            dbModal.PRE = Modal.PRE;
            dbModal.NoiseVibrationFile = Modal.NoiseVibrationFile;
            dbModal.BioMPR = Modal.BioMPR;
            dbModal.AsbestosPlan = Modal.AsbestosPlan;
            dbModal.ShipPublicAddrComment = Modal.ShipPublicAddrComment;
            dbModal.BridgewindowswiperssprayComment = Modal.BridgewindowswiperssprayComment;
            dbModal.BridgewindowswipersComment = Modal.BridgewindowswipersComment;
            dbModal.DaylightSignalsComment = Modal.DaylightSignalsComment;
            dbModal.LiferaftDavitComment = Modal.LiferaftDavitComment;
            dbModal.SnapBackZone5NComment = Modal.SnapBackZone5NComment;
            dbModal.ADPPublicationsComment = Modal.ADPPublicationsComment;

            //RDBJ 10/20/2021
            dbModal.IsGeneralSectionComplete = Modal.IsGeneralSectionComplete == null ? false : true;
            dbModal.IsManningSectionComplete = Modal.IsManningSectionComplete == null ? false : true;
            dbModal.IsShipCertificationSectionComplete = Modal.IsShipCertificationSectionComplete == null ? false : true;
            dbModal.IsRecordKeepingSectionComplete = Modal.IsRecordKeepingSectionComplete == null ? false : true;
            dbModal.IsSafetyEquipmentSectionComplete = Modal.IsSafetyEquipmentSectionComplete == null ? false : true;
            dbModal.IsSecuritySectionComplete = Modal.IsSecuritySectionComplete == null ? false : true;
            dbModal.IsBridgeSectionComplete = Modal.IsBridgeSectionComplete == null ? false : true;
            dbModal.IsMedicalSectionComplete = Modal.IsMedicalSectionComplete == null ? false : true;
            dbModal.IsGalleySectionComplete = Modal.IsGalleySectionComplete == null ? false : true;
            dbModal.IsEngineRoomSectionComplete = Modal.IsEngineRoomSectionComplete == null ? false : true;
            dbModal.IsSuperstructureSectionComplete = Modal.IsSuperstructureSectionComplete == null ? false : true;
            dbModal.IsDeckSectionComplete = Modal.IsDeckSectionComplete == null ? false : true;
            dbModal.IsHoldsAndCoverSectionComplete = Modal.IsHoldsAndCoverSectionComplete == null ? false : true;
            dbModal.IsForeCastleSectionComplete = Modal.IsForeCastleSectionComplete == null ? false : true;
            dbModal.IsHullSectionComplete = Modal.IsHullSectionComplete == null ? false : true;
            dbModal.IsSummarySectionComplete = Modal.IsSummarySectionComplete == null ? false : true;
            dbModal.IsDeficienciesSectionComplete = Modal.IsDeficienciesSectionComplete == null ? false : true;
            dbModal.IsPhotographsSectionComplete = Modal.IsPhotographsSectionComplete == null ? false : true;
            //End RDBJ 10/20/2021
        }

        //RDBJ 09/25/2021 Commented below two methods this are already in CloudSIRHelper and must use from there
        /*
        public bool SIRSynch(Modals.SIRModal Modal)
        {
            bool res = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                Entity.SuperintendedInspectionReport dbModal = new Entity.SuperintendedInspectionReport();
                if (Modal != null && Modal.SuperintendedInspectionReport.UniqueFormID != null)
                {
                    if (Modal.SuperintendedInspectionReport.UniqueFormID != null)
                    {
                        dbModal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == Modal.SuperintendedInspectionReport.UniqueFormID).FirstOrDefault();
                    }
                }

                if (dbModal == null)
                    dbModal = new Entity.SuperintendedInspectionReport();

                SetSIRFormData(ref dbModal, Modal);

                if (dbModal != null && dbModal.UniqueFormID != null)
                {
                    dbModal.IsSynced = true;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbModal.IsSynced = true;
                    dbModal.UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID;
                    dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion;
                    dbContext.SuperintendedInspectionReports.Add(dbModal);
                    dbContext.SaveChanges();
                }

                //GIRSafeManningRequirements_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRSafeManningRequirements);
                //GIRCrewDocuments_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRCrewDocuments);
                //GIRRestandWorkHours_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRRestandWorkHours);
                GIRDeficiencies_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRDeficiencies);
                //GIRPhotos_Save(Convert.ToString(dbModal.UniqueFormID), Modal.GIRPhotographs);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud SubmitSIR : " + ex.Message);
            }
            return res;
        }
        public void SetSIRFormData(ref Entity.SuperintendedInspectionReport dbModal, Modals.SIRModal Modal)
        {
            dbModal.CreatedDate = Modal.SuperintendedInspectionReport.CreatedDate;
            dbModal.UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID; //RDBJ 09/25/2021 Changed FormVersion to UniqueFormID
            dbModal.ShipID = Modal.SuperintendedInspectionReport.ShipID;
            dbModal.ShipName = Modal.SuperintendedInspectionReport.ShipName;
            dbModal.Date = Modal.SuperintendedInspectionReport.Date;
            dbModal.Port = Modal.SuperintendedInspectionReport.Port;
            dbModal.Master = Modal.SuperintendedInspectionReport.Master;
            dbModal.Superintended = Modal.SuperintendedInspectionReport.Superintended;
            dbModal.FormVersion = Modal.SuperintendedInspectionReport.FormVersion;
            dbModal.Section1_1_Condition = Modal.SuperintendedInspectionReport.Section1_1_Condition;
            dbModal.Section1_1_Comment = Modal.SuperintendedInspectionReport.Section1_1_Comment;
            dbModal.Section1_2_Condition = Modal.SuperintendedInspectionReport.Section1_2_Condition;
            dbModal.Section1_2_Comment = Modal.SuperintendedInspectionReport.Section1_2_Comment;
            dbModal.Section1_3_Condition = Modal.SuperintendedInspectionReport.Section1_3_Condition;
            dbModal.Section1_3_Comment = Modal.SuperintendedInspectionReport.Section1_3_Comment;
            dbModal.Section1_4_Condition = Modal.SuperintendedInspectionReport.Section1_4_Condition;
            dbModal.Section1_4_Comment = Modal.SuperintendedInspectionReport.Section1_4_Comment;
            dbModal.Section1_5_Condition = Modal.SuperintendedInspectionReport.Section1_5_Condition;
            dbModal.Section1_5_Comment = Modal.SuperintendedInspectionReport.Section1_5_Comment;
            dbModal.Section1_6_Condition = Modal.SuperintendedInspectionReport.Section1_6_Condition;
            dbModal.Section1_6_Comment = Modal.SuperintendedInspectionReport.Section1_6_Comment;
            dbModal.Section1_7_Condition = Modal.SuperintendedInspectionReport.Section1_7_Condition;
            dbModal.Section1_7_Comment = Modal.SuperintendedInspectionReport.Section1_7_Comment;
            dbModal.Section1_8_Condition = Modal.SuperintendedInspectionReport.Section1_8_Condition;
            dbModal.Section1_8_Comment = Modal.SuperintendedInspectionReport.Section1_8_Comment;
            dbModal.Section1_9_Condition = Modal.SuperintendedInspectionReport.Section1_9_Condition;
            dbModal.Section1_9_Comment = Modal.SuperintendedInspectionReport.Section1_9_Comment;
            dbModal.Section1_10_Condition = Modal.SuperintendedInspectionReport.Section1_10_Condition;
            dbModal.Section1_10_Comment = Modal.SuperintendedInspectionReport.Section1_10_Comment;
            dbModal.Section1_11_Condition = Modal.SuperintendedInspectionReport.Section1_11_Condition;
            dbModal.Section1_11_Comment = Modal.SuperintendedInspectionReport.Section1_11_Comment;

            dbModal.Section2_1_Condition = Modal.SuperintendedInspectionReport.Section2_1_Condition;
            dbModal.Section2_1_Comment = Modal.SuperintendedInspectionReport.Section2_1_Comment;
            dbModal.Section2_2_Condition = Modal.SuperintendedInspectionReport.Section2_2_Condition;
            dbModal.Section2_2_Comment = Modal.SuperintendedInspectionReport.Section2_2_Comment;
            dbModal.Section2_3_Condition = Modal.SuperintendedInspectionReport.Section2_3_Condition;
            dbModal.Section2_3_Comment = Modal.SuperintendedInspectionReport.Section2_3_Comment;
            dbModal.Section2_4_Condition = Modal.SuperintendedInspectionReport.Section2_4_Condition;
            dbModal.Section2_4_Comment = Modal.SuperintendedInspectionReport.Section2_4_Comment;
            dbModal.Section2_5_Condition = Modal.SuperintendedInspectionReport.Section2_5_Condition;
            dbModal.Section2_5_Comment = Modal.SuperintendedInspectionReport.Section2_5_Comment;
            dbModal.Section2_6_Condition = Modal.SuperintendedInspectionReport.Section2_6_Condition;
            dbModal.Section2_6_Comment = Modal.SuperintendedInspectionReport.Section2_6_Comment;
            dbModal.Section2_7_Condition = Modal.SuperintendedInspectionReport.Section2_7_Condition;
            dbModal.Section2_7_Comment = Modal.SuperintendedInspectionReport.Section2_7_Comment;

            dbModal.Section3_1_Condition = Modal.SuperintendedInspectionReport.Section3_1_Condition;
            dbModal.Section3_1_Comment = Modal.SuperintendedInspectionReport.Section3_1_Comment;
            dbModal.Section3_2_Condition = Modal.SuperintendedInspectionReport.Section3_2_Condition;
            dbModal.Section3_2_Comment = Modal.SuperintendedInspectionReport.Section3_2_Comment;
            dbModal.Section3_3_Condition = Modal.SuperintendedInspectionReport.Section3_3_Condition;
            dbModal.Section3_3_Comment = Modal.SuperintendedInspectionReport.Section3_3_Comment;
            dbModal.Section3_4_Condition = Modal.SuperintendedInspectionReport.Section3_4_Condition;
            dbModal.Section3_4_Comment = Modal.SuperintendedInspectionReport.Section3_4_Comment;
            dbModal.Section3_5_Condition = Modal.SuperintendedInspectionReport.Section3_5_Condition;
            dbModal.Section3_5_Comment = Modal.SuperintendedInspectionReport.Section3_5_Comment;

            dbModal.Section4_1_Condition = Modal.SuperintendedInspectionReport.Section4_1_Condition;
            dbModal.Section4_1_Comment = Modal.SuperintendedInspectionReport.Section4_1_Comment;
            dbModal.Section4_2_Condition = Modal.SuperintendedInspectionReport.Section4_2_Condition;
            dbModal.Section4_2_Comment = Modal.SuperintendedInspectionReport.Section4_2_Comment;
            dbModal.Section4_3_Condition = Modal.SuperintendedInspectionReport.Section4_3_Condition;
            dbModal.Section4_3_Comment = Modal.SuperintendedInspectionReport.Section4_3_Comment;

            dbModal.Section5_1_Condition = Modal.SuperintendedInspectionReport.Section5_1_Condition;
            dbModal.Section5_1_Comment = Modal.SuperintendedInspectionReport.Section5_1_Comment;
            dbModal.Section5_6_Condition = Modal.SuperintendedInspectionReport.Section5_6_Condition;
            dbModal.Section5_6_Comment = Modal.SuperintendedInspectionReport.Section5_6_Comment;
            dbModal.Section5_8_Condition = Modal.SuperintendedInspectionReport.Section5_8_Condition;
            dbModal.Section5_8_Comment = Modal.SuperintendedInspectionReport.Section5_8_Comment;
            dbModal.Section5_9_Condition = Modal.SuperintendedInspectionReport.Section5_9_Condition;
            dbModal.Section5_9_Comment = Modal.SuperintendedInspectionReport.Section5_9_Comment;

            dbModal.Section6_1_Condition = Modal.SuperintendedInspectionReport.Section6_1_Condition;
            dbModal.Section6_1_Comment = Modal.SuperintendedInspectionReport.Section6_1_Comment;
            dbModal.Section6_2_Condition = Modal.SuperintendedInspectionReport.Section6_2_Condition;
            dbModal.Section6_2_Comment = Modal.SuperintendedInspectionReport.Section6_2_Comment;
            dbModal.Section6_3_Condition = Modal.SuperintendedInspectionReport.Section6_3_Condition;
            dbModal.Section6_3_Comment = Modal.SuperintendedInspectionReport.Section6_3_Comment;
            dbModal.Section6_4_Condition = Modal.SuperintendedInspectionReport.Section6_4_Condition;
            dbModal.Section6_4_Comment = Modal.SuperintendedInspectionReport.Section6_4_Comment;
            dbModal.Section6_5_Condition = Modal.SuperintendedInspectionReport.Section6_5_Condition;
            dbModal.Section6_5_Comment = Modal.SuperintendedInspectionReport.Section6_5_Comment;
            dbModal.Section6_6_Condition = Modal.SuperintendedInspectionReport.Section6_6_Condition;
            dbModal.Section6_6_Comment = Modal.SuperintendedInspectionReport.Section6_6_Comment;
            dbModal.Section6_7_Condition = Modal.SuperintendedInspectionReport.Section6_7_Condition;
            dbModal.Section6_7_Comment = Modal.SuperintendedInspectionReport.Section6_7_Comment;
            dbModal.Section6_8_Condition = Modal.SuperintendedInspectionReport.Section6_8_Condition;
            dbModal.Section6_8_Comment = Modal.SuperintendedInspectionReport.Section6_8_Comment;

            dbModal.Section7_1_Condition = Modal.SuperintendedInspectionReport.Section7_1_Condition;
            dbModal.Section7_1_Comment = Modal.SuperintendedInspectionReport.Section7_1_Comment;
            dbModal.Section7_2_Condition = Modal.SuperintendedInspectionReport.Section7_2_Condition;
            dbModal.Section7_2_Comment = Modal.SuperintendedInspectionReport.Section7_2_Comment;
            dbModal.Section7_3_Condition = Modal.SuperintendedInspectionReport.Section7_3_Condition;
            dbModal.Section7_3_Comment = Modal.SuperintendedInspectionReport.Section7_3_Comment;
            dbModal.Section7_4_Condition = Modal.SuperintendedInspectionReport.Section7_4_Condition;
            dbModal.Section7_4_Comment = Modal.SuperintendedInspectionReport.Section7_4_Comment;
            dbModal.Section7_5_Condition = Modal.SuperintendedInspectionReport.Section7_5_Condition;
            dbModal.Section7_5_Comment = Modal.SuperintendedInspectionReport.Section7_5_Comment;
            dbModal.Section7_6_Condition = Modal.SuperintendedInspectionReport.Section7_6_Condition;
            dbModal.Section7_6_Comment = Modal.SuperintendedInspectionReport.Section7_6_Comment;

            dbModal.Section8_1_Condition = Modal.SuperintendedInspectionReport.Section8_1_Condition;
            dbModal.Section8_1_Comment = Modal.SuperintendedInspectionReport.Section8_1_Comment;
            dbModal.Section8_2_Condition = Modal.SuperintendedInspectionReport.Section8_2_Condition;
            dbModal.Section8_2_Comment = Modal.SuperintendedInspectionReport.Section8_2_Comment;
            dbModal.Section8_3_Condition = Modal.SuperintendedInspectionReport.Section8_3_Condition;
            dbModal.Section8_3_Comment = Modal.SuperintendedInspectionReport.Section8_3_Comment;
            dbModal.Section8_4_Condition = Modal.SuperintendedInspectionReport.Section8_4_Condition;
            dbModal.Section8_4_Comment = Modal.SuperintendedInspectionReport.Section8_4_Comment;
            dbModal.Section8_5_Condition = Modal.SuperintendedInspectionReport.Section8_5_Condition;
            dbModal.Section8_5_Comment = Modal.SuperintendedInspectionReport.Section8_5_Comment;
            dbModal.Section8_6_Condition = Modal.SuperintendedInspectionReport.Section8_6_Condition;
            dbModal.Section8_6_Comment = Modal.SuperintendedInspectionReport.Section8_6_Comment;
            dbModal.Section8_7_Condition = Modal.SuperintendedInspectionReport.Section8_7_Condition;
            dbModal.Section8_7_Comment = Modal.SuperintendedInspectionReport.Section8_7_Comment;
            dbModal.Section8_8_Condition = Modal.SuperintendedInspectionReport.Section8_8_Condition;
            dbModal.Section8_8_Comment = Modal.SuperintendedInspectionReport.Section8_8_Comment;
            dbModal.Section8_9_Condition = Modal.SuperintendedInspectionReport.Section8_9_Condition;
            dbModal.Section8_9_Comment = Modal.SuperintendedInspectionReport.Section8_9_Comment;
            dbModal.Section8_10_Condition = Modal.SuperintendedInspectionReport.Section8_10_Condition;
            dbModal.Section8_10_Comment = Modal.SuperintendedInspectionReport.Section8_10_Comment;
            dbModal.Section8_11_Condition = Modal.SuperintendedInspectionReport.Section8_11_Condition;
            dbModal.Section8_11_Comment = Modal.SuperintendedInspectionReport.Section8_11_Comment;
            dbModal.Section8_12_Condition = Modal.SuperintendedInspectionReport.Section8_12_Condition;
            dbModal.Section8_12_Comment = Modal.SuperintendedInspectionReport.Section8_12_Comment;
            dbModal.Section8_13_Condition = Modal.SuperintendedInspectionReport.Section8_13_Condition;
            dbModal.Section8_13_Comment = Modal.SuperintendedInspectionReport.Section8_13_Comment;
            dbModal.Section8_14_Condition = Modal.SuperintendedInspectionReport.Section8_14_Condition;
            dbModal.Section8_14_Comment = Modal.SuperintendedInspectionReport.Section8_14_Comment;
            dbModal.Section8_15_Condition = Modal.SuperintendedInspectionReport.Section8_15_Condition;
            dbModal.Section8_15_Comment = Modal.SuperintendedInspectionReport.Section8_15_Comment;
            dbModal.Section8_16_Condition = Modal.SuperintendedInspectionReport.Section8_16_Condition;
            dbModal.Section8_16_Comment = Modal.SuperintendedInspectionReport.Section8_16_Comment;
            dbModal.Section8_17_Condition = Modal.SuperintendedInspectionReport.Section8_17_Condition;
            dbModal.Section8_17_Comment = Modal.SuperintendedInspectionReport.Section8_17_Comment;
            dbModal.Section8_18_Condition = Modal.SuperintendedInspectionReport.Section8_18_Condition;
            dbModal.Section8_18_Comment = Modal.SuperintendedInspectionReport.Section8_18_Comment;
            dbModal.Section8_19_Condition = Modal.SuperintendedInspectionReport.Section8_19_Condition;
            dbModal.Section8_19_Comment = Modal.SuperintendedInspectionReport.Section8_19_Comment;
            dbModal.Section8_20_Condition = Modal.SuperintendedInspectionReport.Section8_20_Condition;
            dbModal.Section8_20_Comment = Modal.SuperintendedInspectionReport.Section8_20_Comment;
            dbModal.Section8_21_Condition = Modal.SuperintendedInspectionReport.Section8_21_Condition;
            dbModal.Section8_21_Comment = Modal.SuperintendedInspectionReport.Section8_21_Comment;
            dbModal.Section8_22_Condition = Modal.SuperintendedInspectionReport.Section8_22_Condition;
            dbModal.Section8_22_Comment = Modal.SuperintendedInspectionReport.Section8_22_Comment;
            dbModal.Section8_23_Condition = Modal.SuperintendedInspectionReport.Section8_23_Condition;
            dbModal.Section8_23_Comment = Modal.SuperintendedInspectionReport.Section8_23_Comment;
            dbModal.Section8_24_Condition = Modal.SuperintendedInspectionReport.Section8_24_Condition;
            dbModal.Section8_24_Comment = Modal.SuperintendedInspectionReport.Section8_24_Comment;
            dbModal.Section8_25_Condition = Modal.SuperintendedInspectionReport.Section8_25_Condition;
            dbModal.Section8_25_Comment = Modal.SuperintendedInspectionReport.Section8_25_Comment;

            dbModal.Section9_1_Condition = Modal.SuperintendedInspectionReport.Section9_1_Condition;
            dbModal.Section9_1_Comment = Modal.SuperintendedInspectionReport.Section9_1_Comment;
            dbModal.Section9_2_Condition = Modal.SuperintendedInspectionReport.Section9_2_Condition;
            dbModal.Section9_2_Comment = Modal.SuperintendedInspectionReport.Section9_2_Comment;
            dbModal.Section9_3_Condition = Modal.SuperintendedInspectionReport.Section9_3_Condition;
            dbModal.Section9_3_Comment = Modal.SuperintendedInspectionReport.Section9_3_Comment;
            dbModal.Section9_4_Condition = Modal.SuperintendedInspectionReport.Section9_4_Condition;
            dbModal.Section9_4_Comment = Modal.SuperintendedInspectionReport.Section9_4_Comment;
            dbModal.Section9_5_Condition = Modal.SuperintendedInspectionReport.Section9_5_Condition;
            dbModal.Section9_5_Comment = Modal.SuperintendedInspectionReport.Section9_5_Comment;
            dbModal.Section9_6_Condition = Modal.SuperintendedInspectionReport.Section9_6_Condition;
            dbModal.Section9_6_Comment = Modal.SuperintendedInspectionReport.Section9_6_Comment;
            dbModal.Section9_7_Condition = Modal.SuperintendedInspectionReport.Section9_7_Condition;
            dbModal.Section9_7_Comment = Modal.SuperintendedInspectionReport.Section9_7_Comment;
            dbModal.Section9_8_Condition = Modal.SuperintendedInspectionReport.Section9_8_Condition;
            dbModal.Section9_8_Comment = Modal.SuperintendedInspectionReport.Section9_8_Comment;
            dbModal.Section9_9_Condition = Modal.SuperintendedInspectionReport.Section9_9_Condition;
            dbModal.Section9_9_Comment = Modal.SuperintendedInspectionReport.Section9_9_Comment;
            dbModal.Section9_10_Condition = Modal.SuperintendedInspectionReport.Section9_10_Condition;
            dbModal.Section9_10_Comment = Modal.SuperintendedInspectionReport.Section9_10_Comment;
            dbModal.Section9_11_Condition = Modal.SuperintendedInspectionReport.Section9_11_Condition;
            dbModal.Section9_11_Comment = Modal.SuperintendedInspectionReport.Section9_11_Comment;
            dbModal.Section9_12_Condition = Modal.SuperintendedInspectionReport.Section9_12_Condition;
            dbModal.Section9_12_Comment = Modal.SuperintendedInspectionReport.Section9_12_Comment;
            dbModal.Section9_13_Condition = Modal.SuperintendedInspectionReport.Section9_13_Condition;
            dbModal.Section9_13_Comment = Modal.SuperintendedInspectionReport.Section9_13_Comment;
            dbModal.Section9_14_Condition = Modal.SuperintendedInspectionReport.Section9_14_Condition;
            dbModal.Section9_14_Comment = Modal.SuperintendedInspectionReport.Section9_14_Comment;
            dbModal.Section9_15_Condition = Modal.SuperintendedInspectionReport.Section9_15_Condition;
            dbModal.Section9_15_Comment = Modal.SuperintendedInspectionReport.Section9_15_Comment;

            dbModal.Section10_1_Condition = Modal.SuperintendedInspectionReport.Section10_1_Condition;
            dbModal.Section10_1_Comment = Modal.SuperintendedInspectionReport.Section10_1_Comment;
            dbModal.Section10_2_Condition = Modal.SuperintendedInspectionReport.Section10_2_Condition;
            dbModal.Section10_2_Comment = Modal.SuperintendedInspectionReport.Section10_2_Comment;
            dbModal.Section10_3_Condition = Modal.SuperintendedInspectionReport.Section10_3_Condition;
            dbModal.Section10_3_Comment = Modal.SuperintendedInspectionReport.Section10_3_Comment;
            dbModal.Section10_4_Condition = Modal.SuperintendedInspectionReport.Section10_4_Condition;
            dbModal.Section10_4_Comment = Modal.SuperintendedInspectionReport.Section10_4_Comment;
            dbModal.Section10_5_Condition = Modal.SuperintendedInspectionReport.Section10_5_Condition;
            dbModal.Section10_5_Comment = Modal.SuperintendedInspectionReport.Section10_5_Comment;
            dbModal.Section10_6_Condition = Modal.SuperintendedInspectionReport.Section10_6_Condition;
            dbModal.Section10_6_Comment = Modal.SuperintendedInspectionReport.Section10_6_Comment;
            dbModal.Section10_7_Condition = Modal.SuperintendedInspectionReport.Section10_7_Condition;
            dbModal.Section10_7_Comment = Modal.SuperintendedInspectionReport.Section10_7_Comment;
            dbModal.Section10_8_Condition = Modal.SuperintendedInspectionReport.Section10_8_Condition;
            dbModal.Section10_8_Comment = Modal.SuperintendedInspectionReport.Section10_8_Comment;
            dbModal.Section10_9_Condition = Modal.SuperintendedInspectionReport.Section10_9_Condition;
            dbModal.Section10_9_Comment = Modal.SuperintendedInspectionReport.Section10_9_Comment;
            dbModal.Section10_10_Condition = Modal.SuperintendedInspectionReport.Section10_10_Condition;
            dbModal.Section10_10_Comment = Modal.SuperintendedInspectionReport.Section10_10_Comment;
            dbModal.Section10_11_Condition = Modal.SuperintendedInspectionReport.Section10_11_Condition;
            dbModal.Section10_11_Comment = Modal.SuperintendedInspectionReport.Section10_11_Comment;
            dbModal.Section10_12_Condition = Modal.SuperintendedInspectionReport.Section10_12_Condition;
            dbModal.Section10_12_Comment = Modal.SuperintendedInspectionReport.Section10_12_Comment;
            dbModal.Section10_13_Condition = Modal.SuperintendedInspectionReport.Section10_13_Condition;
            dbModal.Section10_13_Comment = Modal.SuperintendedInspectionReport.Section10_13_Comment;
            dbModal.Section10_14_Condition = Modal.SuperintendedInspectionReport.Section10_14_Condition;
            dbModal.Section10_14_Comment = Modal.SuperintendedInspectionReport.Section10_14_Comment;
            dbModal.Section10_15_Condition = Modal.SuperintendedInspectionReport.Section10_15_Condition;
            dbModal.Section10_15_Comment = Modal.SuperintendedInspectionReport.Section10_15_Comment;
            dbModal.Section10_16_Condition = Modal.SuperintendedInspectionReport.Section10_16_Condition;
            dbModal.Section10_16_Comment = Modal.SuperintendedInspectionReport.Section10_16_Comment;

            dbModal.Section11_1_Condition = Modal.SuperintendedInspectionReport.Section11_1_Condition;
            dbModal.Section11_1_Comment = Modal.SuperintendedInspectionReport.Section11_1_Comment;
            dbModal.Section11_2_Condition = Modal.SuperintendedInspectionReport.Section11_2_Condition;
            dbModal.Section11_2_Comment = Modal.SuperintendedInspectionReport.Section11_2_Comment;
            dbModal.Section11_3_Condition = Modal.SuperintendedInspectionReport.Section11_3_Condition;
            dbModal.Section11_3_Comment = Modal.SuperintendedInspectionReport.Section11_3_Comment;
            dbModal.Section11_4_Condition = Modal.SuperintendedInspectionReport.Section11_4_Condition;
            dbModal.Section11_4_Comment = Modal.SuperintendedInspectionReport.Section11_4_Comment;
            dbModal.Section11_5_Condition = Modal.SuperintendedInspectionReport.Section11_5_Condition;
            dbModal.Section11_5_Comment = Modal.SuperintendedInspectionReport.Section11_5_Comment;
            dbModal.Section11_6_Condition = Modal.SuperintendedInspectionReport.Section11_6_Condition;
            dbModal.Section11_6_Comment = Modal.SuperintendedInspectionReport.Section11_6_Comment;
            dbModal.Section11_7_Condition = Modal.SuperintendedInspectionReport.Section11_7_Condition;
            dbModal.Section11_7_Comment = Modal.SuperintendedInspectionReport.Section11_7_Comment;
            dbModal.Section11_8_Condition = Modal.SuperintendedInspectionReport.Section11_8_Condition;
            dbModal.Section11_8_Comment = Modal.SuperintendedInspectionReport.Section11_8_Comment;

            dbModal.Section12_1_Condition = Modal.SuperintendedInspectionReport.Section12_1_Condition;
            dbModal.Section12_1_Comment = Modal.SuperintendedInspectionReport.Section12_1_Comment;
            dbModal.Section12_2_Condition = Modal.SuperintendedInspectionReport.Section12_2_Condition;
            dbModal.Section12_2_Comment = Modal.SuperintendedInspectionReport.Section12_2_Comment;
            dbModal.Section12_3_Condition = Modal.SuperintendedInspectionReport.Section12_3_Condition;
            dbModal.Section12_3_Comment = Modal.SuperintendedInspectionReport.Section12_3_Comment;
            dbModal.Section12_4_Condition = Modal.SuperintendedInspectionReport.Section12_4_Condition;
            dbModal.Section12_4_Comment = Modal.SuperintendedInspectionReport.Section12_4_Comment;
            dbModal.Section12_5_Condition = Modal.SuperintendedInspectionReport.Section12_5_Condition;
            dbModal.Section12_5_Comment = Modal.SuperintendedInspectionReport.Section12_5_Comment;
            dbModal.Section12_6_Condition = Modal.SuperintendedInspectionReport.Section12_6_Condition;
            dbModal.Section12_6_Comment = Modal.SuperintendedInspectionReport.Section12_6_Comment;

            dbModal.Section13_1_Condition = Modal.SuperintendedInspectionReport.Section13_1_Condition;
            dbModal.Section13_1_Comment = Modal.SuperintendedInspectionReport.Section13_1_Comment;
            dbModal.Section13_2_Condition = Modal.SuperintendedInspectionReport.Section13_2_Condition;
            dbModal.Section13_2_Comment = Modal.SuperintendedInspectionReport.Section13_2_Comment;
            dbModal.Section13_3_Condition = Modal.SuperintendedInspectionReport.Section13_3_Condition;
            dbModal.Section13_3_Comment = Modal.SuperintendedInspectionReport.Section13_3_Comment;
            dbModal.Section13_4_Condition = Modal.SuperintendedInspectionReport.Section13_4_Condition;
            dbModal.Section13_4_Comment = Modal.SuperintendedInspectionReport.Section13_4_Comment;

            dbModal.Section14_1_Condition = Modal.SuperintendedInspectionReport.Section14_1_Condition;
            dbModal.Section14_1_Comment = Modal.SuperintendedInspectionReport.Section14_1_Comment;
            dbModal.Section14_2_Condition = Modal.SuperintendedInspectionReport.Section14_2_Condition;
            dbModal.Section14_2_Comment = Modal.SuperintendedInspectionReport.Section14_2_Comment;
            dbModal.Section14_3_Condition = Modal.SuperintendedInspectionReport.Section14_3_Condition;
            dbModal.Section14_3_Comment = Modal.SuperintendedInspectionReport.Section14_3_Comment;
            dbModal.Section14_4_Condition = Modal.SuperintendedInspectionReport.Section14_4_Condition;
            dbModal.Section14_4_Comment = Modal.SuperintendedInspectionReport.Section14_4_Comment;
            dbModal.Section14_5_Condition = Modal.SuperintendedInspectionReport.Section14_5_Condition;
            dbModal.Section14_5_Comment = Modal.SuperintendedInspectionReport.Section14_5_Comment;
            dbModal.Section14_6_Condition = Modal.SuperintendedInspectionReport.Section14_6_Condition;
            dbModal.Section14_6_Comment = Modal.SuperintendedInspectionReport.Section14_6_Comment;
            dbModal.Section14_7_Condition = Modal.SuperintendedInspectionReport.Section14_7_Condition;
            dbModal.Section14_7_Comment = Modal.SuperintendedInspectionReport.Section14_7_Comment;
            dbModal.Section14_8_Condition = Modal.SuperintendedInspectionReport.Section14_8_Condition;
            dbModal.Section14_8_Comment = Modal.SuperintendedInspectionReport.Section14_8_Comment;
            dbModal.Section14_9_Condition = Modal.SuperintendedInspectionReport.Section14_9_Condition;
            dbModal.Section14_9_Comment = Modal.SuperintendedInspectionReport.Section14_9_Comment;
            dbModal.Section14_10_Condition = Modal.SuperintendedInspectionReport.Section14_10_Condition;
            dbModal.Section14_10_Comment = Modal.SuperintendedInspectionReport.Section14_10_Comment;
            dbModal.Section14_11_Condition = Modal.SuperintendedInspectionReport.Section14_11_Condition;
            dbModal.Section14_11_Comment = Modal.SuperintendedInspectionReport.Section14_11_Comment;
            dbModal.Section14_12_Condition = Modal.SuperintendedInspectionReport.Section14_12_Condition;
            dbModal.Section14_12_Comment = Modal.SuperintendedInspectionReport.Section14_12_Comment;
            dbModal.Section14_13_Condition = Modal.SuperintendedInspectionReport.Section14_13_Condition;
            dbModal.Section14_13_Comment = Modal.SuperintendedInspectionReport.Section14_13_Comment;
            dbModal.Section14_14_Condition = Modal.SuperintendedInspectionReport.Section14_14_Condition;
            dbModal.Section14_14_Comment = Modal.SuperintendedInspectionReport.Section14_14_Comment;
            dbModal.Section14_15_Condition = Modal.SuperintendedInspectionReport.Section14_15_Condition;
            dbModal.Section14_15_Comment = Modal.SuperintendedInspectionReport.Section14_15_Comment;
            dbModal.Section14_16_Condition = Modal.SuperintendedInspectionReport.Section14_16_Condition;
            dbModal.Section14_16_Comment = Modal.SuperintendedInspectionReport.Section14_16_Comment;
            dbModal.Section14_17_Condition = Modal.SuperintendedInspectionReport.Section14_17_Condition;
            dbModal.Section14_17_Comment = Modal.SuperintendedInspectionReport.Section14_17_Comment;
            dbModal.Section14_18_Condition = Modal.SuperintendedInspectionReport.Section14_18_Condition;
            dbModal.Section14_18_Comment = Modal.SuperintendedInspectionReport.Section14_18_Comment;
            dbModal.Section14_19_Condition = Modal.SuperintendedInspectionReport.Section14_19_Condition;
            dbModal.Section14_19_Comment = Modal.SuperintendedInspectionReport.Section14_19_Comment;
            dbModal.Section14_20_Condition = Modal.SuperintendedInspectionReport.Section14_20_Condition;
            dbModal.Section14_20_Comment = Modal.SuperintendedInspectionReport.Section14_20_Comment;
            dbModal.Section14_21_Condition = Modal.SuperintendedInspectionReport.Section14_21_Condition;
            dbModal.Section14_21_Comment = Modal.SuperintendedInspectionReport.Section14_21_Comment;
            dbModal.Section14_22_Condition = Modal.SuperintendedInspectionReport.Section14_22_Condition;
            dbModal.Section14_22_Comment = Modal.SuperintendedInspectionReport.Section14_22_Comment;
            dbModal.Section14_23_Condition = Modal.SuperintendedInspectionReport.Section14_23_Condition;
            dbModal.Section14_23_Comment = Modal.SuperintendedInspectionReport.Section14_23_Comment;
            dbModal.Section14_24_Condition = Modal.SuperintendedInspectionReport.Section14_24_Condition;
            dbModal.Section14_24_Comment = Modal.SuperintendedInspectionReport.Section14_24_Comment;
            dbModal.Section14_25_Condition = Modal.SuperintendedInspectionReport.Section14_25_Condition;
            dbModal.Section14_25_Comment = Modal.SuperintendedInspectionReport.Section14_25_Comment;

            dbModal.Section15_1_Condition = Modal.SuperintendedInspectionReport.Section15_1_Condition;
            dbModal.Section15_1_Comment = Modal.SuperintendedInspectionReport.Section15_1_Comment;
            dbModal.Section15_2_Condition = Modal.SuperintendedInspectionReport.Section15_2_Condition;
            dbModal.Section15_2_Comment = Modal.SuperintendedInspectionReport.Section15_2_Comment;
            dbModal.Section15_3_Condition = Modal.SuperintendedInspectionReport.Section15_3_Condition;
            dbModal.Section15_3_Comment = Modal.SuperintendedInspectionReport.Section15_3_Comment;
            dbModal.Section15_4_Condition = Modal.SuperintendedInspectionReport.Section15_4_Condition;
            dbModal.Section15_4_Comment = Modal.SuperintendedInspectionReport.Section15_4_Comment;
            dbModal.Section15_5_Condition = Modal.SuperintendedInspectionReport.Section15_5_Condition;
            dbModal.Section15_5_Comment = Modal.SuperintendedInspectionReport.Section15_5_Comment;
            dbModal.Section15_6_Condition = Modal.SuperintendedInspectionReport.Section15_6_Condition;
            dbModal.Section15_6_Comment = Modal.SuperintendedInspectionReport.Section15_6_Comment;
            dbModal.Section15_7_Condition = Modal.SuperintendedInspectionReport.Section15_7_Condition;
            dbModal.Section15_7_Comment = Modal.SuperintendedInspectionReport.Section15_7_Comment;
            dbModal.Section15_8_Condition = Modal.SuperintendedInspectionReport.Section15_8_Condition;
            dbModal.Section15_8_Comment = Modal.SuperintendedInspectionReport.Section15_8_Comment;
            dbModal.Section15_9_Condition = Modal.SuperintendedInspectionReport.Section15_9_Condition;
            dbModal.Section15_9_Comment = Modal.SuperintendedInspectionReport.Section15_9_Comment;
            dbModal.Section15_10_Condition = Modal.SuperintendedInspectionReport.Section15_10_Condition;
            dbModal.Section15_10_Comment = Modal.SuperintendedInspectionReport.Section15_10_Comment;
            dbModal.Section15_11_Condition = Modal.SuperintendedInspectionReport.Section15_11_Condition;
            dbModal.Section15_11_Comment = Modal.SuperintendedInspectionReport.Section15_11_Comment;
            dbModal.Section15_12_Condition = Modal.SuperintendedInspectionReport.Section15_12_Condition;
            dbModal.Section15_12_Comment = Modal.SuperintendedInspectionReport.Section15_12_Comment;
            dbModal.Section15_13_Condition = Modal.SuperintendedInspectionReport.Section15_13_Condition;
            dbModal.Section15_13_Comment = Modal.SuperintendedInspectionReport.Section15_13_Comment;
            dbModal.Section15_14_Condition = Modal.SuperintendedInspectionReport.Section15_14_Condition;
            dbModal.Section15_14_Comment = Modal.SuperintendedInspectionReport.Section15_14_Comment;
            dbModal.Section15_15_Condition = Modal.SuperintendedInspectionReport.Section15_15_Condition;
            dbModal.Section15_15_Comment = Modal.SuperintendedInspectionReport.Section15_15_Comment;

            dbModal.Section16_1_Condition = Modal.SuperintendedInspectionReport.Section16_1_Condition;
            dbModal.Section16_1_Comment = Modal.SuperintendedInspectionReport.Section16_1_Comment;
            dbModal.Section16_2_Condition = Modal.SuperintendedInspectionReport.Section16_2_Condition;
            dbModal.Section16_2_Comment = Modal.SuperintendedInspectionReport.Section16_2_Comment;
            dbModal.Section16_3_Condition = Modal.SuperintendedInspectionReport.Section16_3_Condition;
            dbModal.Section16_3_Comment = Modal.SuperintendedInspectionReport.Section16_3_Comment;
            dbModal.Section16_4_Condition = Modal.SuperintendedInspectionReport.Section16_4_Condition;
            dbModal.Section16_4_Comment = Modal.SuperintendedInspectionReport.Section16_4_Comment;

            dbModal.Section17_1_Condition = Modal.SuperintendedInspectionReport.Section17_1_Condition;
            dbModal.Section17_1_Comment = Modal.SuperintendedInspectionReport.Section17_1_Comment;
            dbModal.Section17_2_Condition = Modal.SuperintendedInspectionReport.Section17_2_Condition;
            dbModal.Section17_2_Comment = Modal.SuperintendedInspectionReport.Section17_2_Comment;
            dbModal.Section17_3_Condition = Modal.SuperintendedInspectionReport.Section17_3_Condition;
            dbModal.Section17_3_Comment = Modal.SuperintendedInspectionReport.Section17_3_Comment;
            dbModal.Section17_4_Condition = Modal.SuperintendedInspectionReport.Section17_4_Condition;
            dbModal.Section17_4_Comment = Modal.SuperintendedInspectionReport.Section17_4_Comment;
            dbModal.Section17_5_Condition = Modal.SuperintendedInspectionReport.Section17_5_Condition;
            dbModal.Section17_5_Comment = Modal.SuperintendedInspectionReport.Section17_5_Comment;
            dbModal.Section17_6_Condition = Modal.SuperintendedInspectionReport.Section17_6_Condition;
            dbModal.Section17_6_Comment = Modal.SuperintendedInspectionReport.Section17_6_Comment;

            dbModal.Section18_1_Condition = Modal.SuperintendedInspectionReport.Section18_1_Condition;
            dbModal.Section18_1_Comment = Modal.SuperintendedInspectionReport.Section18_1_Comment;
            dbModal.Section18_2_Condition = Modal.SuperintendedInspectionReport.Section18_2_Condition;
            dbModal.Section18_2_Comment = Modal.SuperintendedInspectionReport.Section18_2_Comment;
            dbModal.Section18_3_Condition = Modal.SuperintendedInspectionReport.Section18_3_Condition;
            dbModal.Section18_3_Comment = Modal.SuperintendedInspectionReport.Section18_3_Comment;
            dbModal.Section18_4_Condition = Modal.SuperintendedInspectionReport.Section18_4_Condition;
            dbModal.Section18_4_Comment = Modal.SuperintendedInspectionReport.Section18_4_Comment;
            dbModal.Section18_5_Condition = Modal.SuperintendedInspectionReport.Section18_5_Condition;
            dbModal.Section18_5_Comment = Modal.SuperintendedInspectionReport.Section18_5_Comment;
            dbModal.Section18_6_Condition = Modal.SuperintendedInspectionReport.Section18_6_Condition;
            dbModal.Section18_6_Comment = Modal.SuperintendedInspectionReport.Section18_6_Comment;
            dbModal.Section18_7_Condition = Modal.SuperintendedInspectionReport.Section18_7_Condition;
            dbModal.Section18_7_Comment = Modal.SuperintendedInspectionReport.Section18_7_Comment;

            dbModal.IsSynced = Modal.SuperintendedInspectionReport.IsSynced;
            dbModal.CreatedDate = Modal.SuperintendedInspectionReport.CreatedDate;
            dbModal.ModifyDate = Modal.SuperintendedInspectionReport.ModifyDate;
            dbModal.SavedAsDraft = Modal.SuperintendedInspectionReport.SavedAsDraft;
        }
        */

        public void GIRSafeManningRequirements_Save(string UniqueFormID, List<Modals.GlRSafeManningRequirements> GIRSafeManningRequirements)
        {
            try
            {
                Guid UFormID = Guid.Parse(UniqueFormID);
                if (GIRSafeManningRequirements != null && GIRSafeManningRequirements.Count > 0 && UFormID != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    List<GlRSafeManningRequirement> dbGlRSafeManningRequirements = dbContext.GlRSafeManningRequirements.Where(x => x.UniqueFormID == UFormID).ToList();
                    if (dbGlRSafeManningRequirements != null && dbGlRSafeManningRequirements.Count > 0)
                    {
                        foreach (var item in dbGlRSafeManningRequirements)
                        {
                            dbContext.GlRSafeManningRequirements.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }
                    // Insert Into DB
                    foreach (var item in GIRSafeManningRequirements)
                    {
                        Entity.GlRSafeManningRequirement member = new Entity.GlRSafeManningRequirement();
                        member.GIRFormID = 0;
                        member.UniqueFormID = UFormID;
                        member.Ship = item.Ship;
                        member.Rank = item.Rank;
                        member.RequiredbySMD = item.RequiredbySMD;
                        member.OnBoard = item.OnBoard;
                        member.CreatedDate = item.CreatedDate;
                        member.UpdatedDate = item.UpdatedDate;
                        dbContext.GlRSafeManningRequirements.Add(member);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRSafeManningRequirements_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRCrewDocuments_Save(string UniqueFormID, List<Modals.GlRCrewDocuments> GIRCrewDocuments)
        {
            try
            {
                Guid UFormID = Guid.Parse(UniqueFormID);
                if (GIRCrewDocuments != null && GIRCrewDocuments.Count > 0 && UFormID != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    if (GIRCrewDocuments != null && GIRCrewDocuments.Count > 0)
                    {
                        List<GlRCrewDocument> dbGlRCrewDocuments = dbContext.GlRCrewDocuments.Where(x => x.UniqueFormID == UFormID).ToList();
                        if (dbGlRCrewDocuments != null && dbGlRCrewDocuments.Count > 0)
                        {
                            foreach (var item in dbGlRCrewDocuments)
                            {
                                dbContext.GlRCrewDocuments.Remove(item);
                            }
                            dbContext.SaveChanges();
                        }
                        foreach (var item in GIRCrewDocuments)
                        {
                            Entity.GlRCrewDocument member = new Entity.GlRCrewDocument();
                            member.GIRFormID = 0;
                            member.UniqueFormID = UFormID;
                            member.Ship = item.Ship;
                            member.CrewmemberName = item.CrewmemberName;
                            member.CertificationDetail = item.CertificationDetail;
                            member.CreatedDate = item.CreatedDate;
                            member.UpdatedDate = item.UpdatedDate;
                            dbContext.GlRCrewDocuments.Add(member);
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRCrewDocuments_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRRestandWorkHours_Save(string UniqueFormID, List<Modals.GIRRestandWorkHours> GIRRestandWorkHours)
        {
            try
            {
                Guid UFormID = Guid.Parse(UniqueFormID);
                if (GIRRestandWorkHours != null && GIRRestandWorkHours.Count > 0 && UFormID != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    List<GIRRestandWorkHour> dbGIRRestandWorkHours = dbContext.GIRRestandWorkHours.Where(x => x.UniqueFormID == UFormID).ToList();
                    if (dbGIRRestandWorkHours != null && dbGIRRestandWorkHours.Count > 0)
                    {
                        foreach (var item in dbGIRRestandWorkHours)
                        {
                            dbContext.GIRRestandWorkHours.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }
                    foreach (var item in GIRRestandWorkHours)
                    {
                        Entity.GIRRestandWorkHour member = new Entity.GIRRestandWorkHour();
                        member.GIRFormID = 0;
                        member.UniqueFormID = UFormID;
                        member.Ship = item.Ship;
                        member.CrewmemberName = item.CrewmemberName;
                        member.RestAndWorkDetail = item.RestAndWorkDetail;
                        member.CreatedDate = item.CreatedDate;
                        member.UpdatedDate = item.UpdatedDate;
                        dbContext.GIRRestandWorkHours.Add(member);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRRestandWorkHours_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRPhotos_Save(string UniqueFormID, List<Modals.GIRPhotographs> modal
            , bool blnIsNeedToDeleteORInsert = false
            )
        {
            try
            {
                Guid UFormID = Guid.Parse(UniqueFormID);
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                // JSL 01/03/2023 wrapped in if
                if (blnIsNeedToDeleteORInsert)
                {
                    List<GIRPhotograph> GIRPhotographs = dbContext.GIRPhotographs.Where(x => x.UniqueFormID == UFormID).ToList();
                    if (GIRPhotographs != null && GIRPhotographs.Count > 0)
                    {
                        foreach (var item in GIRPhotographs)
                        {
                            dbContext.GIRPhotographs.Remove(item);
                        }
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    if (modal != null && modal.Count > 0)
                    {
                        foreach (var item in modal)
                        {
                            Entity.GIRPhotograph file = new Entity.GIRPhotograph();
                            file.GIRFormID = 0;
                            file.UniqueFormID = UFormID;
                            file.FileName = item.FileName;
                            //file.ImagePath = item.ImagePath;  // JSL 12/03/2022 commented
                            file.ImageCaption = item.ImageCaption;
                            file.Ship = item.Ship;
                            file.CreatedDate = item.CreatedDate;
                            file.UpdatedDate = item.UpdatedDate;

                            // JSL 12/03/2022
                            if (item.ImagePath.StartsWith("data:"))
                            {
                                Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                                dicFileMetaData["UniqueFormID"] = Convert.ToString(UniqueFormID);
                                dicFileMetaData["ReportType"] = "GI";
                                dicFileMetaData["FileName"] = item.FileName;
                                dicFileMetaData["Base64FileData"] = item.ImagePath;

                                file.ImagePath = Utility.ConvertBase64IntoFile(dicFileMetaData, true);
                            }
                            // End JSL 12/03/2022

                            dbContext.GIRPhotographs.Add(file);
                        }
                        dbContext.SaveChanges();
                    }
                }
                // JSL 01/03/2023 wrapped in if
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRPhotos_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRDeficiencies_Save(string UniqueFormID, List<Modals.GIRDeficiencies> GIRDeficiencies
            , ref bool IsNeedToSendNotification // JSL 06/24/2022
            )
        {
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0 && Guid.Parse(UniqueFormID) != Guid.Empty)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    foreach (var item in GIRDeficiencies)
                    {
                        Entity.GIRDeficiency member = new Entity.GIRDeficiency();

                        //member = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == item.UniqueFormID && x.No == item.No && (string.IsNullOrEmpty(x.ReportType) || x.ReportType == item.ReportType)).FirstOrDefault(); //RDBJ 09/21/2021 Commented 
                        member = dbContext.GIRDeficiencies.Where(x => x.DeficienciesUniqueID == item.DeficienciesUniqueID && (string.IsNullOrEmpty(x.ReportType) || x.ReportType == item.ReportType)).FirstOrDefault(); //RDBJ 09/21/2021

                        if (member == null || member.UniqueFormID == Guid.Empty)
                        {
                            member = new Entity.GIRDeficiency();

                            member.No = item.No;
                            member.DateRaised = item.DateRaised;
                            member.Deficiency = item.Deficiency;
                            member.DateClosed = item.DateClosed;
                            member.CreatedDate = item.CreatedDate;
                            member.UpdatedDate = item.UpdatedDate;
                            member.Ship = item.Ship;
                            member.IsClose = item.IsClose;
                            member.ReportType = item.ReportType;
                            member.ItemNo = item.ItemNo;
                            member.Section = item.Section;
                            member.UniqueFormID = item.UniqueFormID;
                            member.isDelete = item.isDelete;
                            member.DeficienciesUniqueID = item.DeficienciesUniqueID;
                            member.Priority = item.Priority == null ? 12 : item.Priority; //RDBJ 11/01/2021
                            member.AssignTo = item.AssignTo; // RDBJ 12/18/2021
                            member.DueDate = item.DueDate;  // RDBJ 03/01/2022

                            dbContext.GIRDeficiencies.Add(member);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            // JSL 09/09/2022
                            member.Ship = item.Ship;
                            member.ReportType = item.ReportType;
                            member.ItemNo = item.ItemNo;
                            member.Section = item.Section;
                            member.UniqueFormID = item.UniqueFormID;
                            // End JSL 09/09/2022

                            member.No = item.No;    // JSL 08/31/2022
                            member.DateRaised = item.DateRaised;
                            member.Deficiency = item.Deficiency;
                            member.DateClosed = item.DateClosed;
                            member.UpdatedDate = item.UpdatedDate;
                            member.IsClose = item.IsClose;
                            member.isDelete = item.isDelete;
                            member.DeficienciesUniqueID = item.DeficienciesUniqueID;
                            member.Priority = item.Priority == null ? 12 : item.Priority; //RDBJ 11/01/2021
                            member.AssignTo = item.AssignTo; // RDBJ 12/18/2021
                            member.DueDate = item.DueDate;  // RDBJ 03/01/2022

                            dbContext.SaveChanges();
                        }

                        // JSL 12/03/2022
                        Dictionary<string, string> dicFileMetaData = new Dictionary<string, string>();
                        dicFileMetaData["UniqueFormID"] = Convert.ToString(UniqueFormID);
                        dicFileMetaData["ReportType"] = item.ReportType;
                        dicFileMetaData["DetailUniqueId"] = Convert.ToString(item.DeficienciesUniqueID);
                        // End JSL 12/03/2022

                        if (item.GIRDeficienciesFile != null && item.GIRDeficienciesFile.Count > 0)
                        {
                            GIRDeficienciesFile_Save(item, item.Ship, item.DeficienciesUniqueID
                                , dicFileMetaData   // JSL 12/03/2022
                                );
                        }

                        if (item.GIRDeficienciesComments != null && item.GIRDeficienciesComments.Count > 0)
                        {
                            GIRDeficienciesComments_Save(item.GIRDeficienciesComments, item.DeficienciesUniqueID
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/24/2022
                                );
                        }

                        if (item.GIRDeficienciesInitialActions != null && item.GIRDeficienciesInitialActions.Count > 0) //RDBJ 09/22/2021 Updateed Modal Name used
                        {
                            GIRDeficienciesInitialActions_Save(item.GIRDeficienciesInitialActions, item.DeficienciesUniqueID //RDBJ 09/22/2021 Updateed Modal Name used
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/24/2022
                                );
                        }

                        if (item.GIRDeficienciesResolution != null && item.GIRDeficienciesResolution.Count > 0) //RDBJ 09/22/2021 Updateed Modal Name used
                        {
                            GIRDeficienciesResolution_Save(item.GIRDeficienciesResolution, item.DeficienciesUniqueID //RDBJ 09/22/2021 Updateed Modal Name used
                                    , dicFileMetaData   // JSL 12/03/2022
                                    , ref IsNeedToSendNotification  // JSL 06/24/2022
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GIRDeficiencies_Save : " + ex.Message);
            }
        }
        public void GIRDeficienciesFile_Save(GIRDeficiencies modal, string Ship, Guid? DeficienciesUniqueID
            , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                List<Entity.GIRDeficienciesFile> dbdeficienciesFile = new List<Entity.GIRDeficienciesFile>();
                dbdeficienciesFile = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == DeficienciesUniqueID).ToList();

                if (dbdeficienciesFile != null && dbdeficienciesFile.Count > 0)
                {
                    foreach (var item in dbdeficienciesFile)
                    {
                        if (DeficienciesUniqueID!=null)
                        {
                            dbContext.GIRDeficienciesFiles.Remove(item);
                            dbContext.SaveChanges();
                        }
                    }
                }

                foreach (var item in modal.GIRDeficienciesFile)
                {
                    Entity.GIRDeficienciesFile file = new Entity.GIRDeficienciesFile();
                    file.DeficienciesFileUniqueID = (Guid)item.DeficienciesFileUniqueID;  // JSL 06/07/2022
                    file.FileName = item.FileName;
                    file.StorePath = item.StorePath;

                    // JSL 12/03/2022
                    if (item.StorePath.StartsWith("data:"))
                    {
                        dicFileMetaData["FileName"] = item.FileName;
                        dicFileMetaData["Base64FileData"] = item.StorePath;

                        file.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                    }
                    // End JSL 12/03/2022

                    file.DeficienciesUniqueID = DeficienciesUniqueID;
                    dbContext.GIRDeficienciesFiles.Add(file);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GIRDeficienciesFile_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRDeficienciesComments_Save(List<Modals.DeficienciesNote> modalDefNotes, Guid? DeficienciesID
            , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
            , ref bool IsNeedToSendNotification // JSL 06/24/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modalDefNotes != null && modalDefNotes.Count > 0)
                {
                    foreach (var itemDefNotes in modalDefNotes)
                    {
                        Entity.GIRDeficienciesNote dbModal = new Entity.GIRDeficienciesNote();
                        dbModal = dbContext.GIRDeficienciesNotes.Where(x => x.NoteUniqueID == itemDefNotes.NoteUniqueID).FirstOrDefault();

                        if (dbModal == null)
                        {
                            Entity.GIRDeficienciesNote defNotes = new Entity.GIRDeficienciesNote();
                            defNotes.DeficienciesID = itemDefNotes.DeficienciesID;
                            defNotes.UserName = itemDefNotes.UserName;
                            defNotes.Comment = itemDefNotes.Comment;
                            defNotes.CreatedDate = itemDefNotes.CreatedDate;
                            defNotes.ModifyDate = itemDefNotes.ModifyDate;
                            defNotes.NoteUniqueID = itemDefNotes.NoteUniqueID;
                            defNotes.DeficienciesUniqueID = itemDefNotes.DeficienciesUniqueID;
                            //defNotes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/14/2021
                            defNotes.isNew = 0; // JSL 06/27/2022 //RDBJ 10/14/2021
                            dbContext.GIRDeficienciesNotes.Add(defNotes);
                            dbContext.SaveChanges();

                            // JSL 06/24/2022
                            IsNeedToSendNotification = true;
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "5";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeComment;
                            dictNotificationData["IsDraft"] = Convert.ToString(true);
                            dictNotificationData["Title"] = AppStatic.GIRFormName;
                            dictNotificationData["DetailsURL"] = "GIRList/DeficienciesDetails?id=" + Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData);
                            // End JSL 06/24/2022
                        }

                        if (itemDefNotes.GIRDeficienciesCommentFile != null && itemDefNotes.GIRDeficienciesCommentFile.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefNotes.GIRDeficienciesCommentFile)
                            {
                                Entity.GIRDeficienciesCommentFile commentFile = new Entity.GIRDeficienciesCommentFile();
                                commentFile = dbContext.GIRDeficienciesCommentFiles.Where(x => x.CommentFileUniqueID == itemCommentFiles.CommentFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.GIRDeficienciesCommentFile defNotesFile = new Entity.GIRDeficienciesCommentFile();
                                    defNotesFile.DeficienciesID = itemCommentFiles.DeficienciesID;
                                    defNotesFile.FileName = itemCommentFiles.FileName;
                                    defNotesFile.StorePath = itemCommentFiles.StorePath;

                                    // JSL 12/03/2022
                                    if (defNotesFile.StorePath.StartsWith("data:"))
                                    {
                                        dicFileMetaData["FileName"] = defNotesFile.FileName;
                                        dicFileMetaData["Base64FileData"] = defNotesFile.StorePath;

                                        // JSL 01/08/2023
                                        dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeComment;
                                        dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentFiles.NoteUniqueID);
                                        // End JSL 01/08/2023

                                        defNotesFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                    }
                                    // End JSL 12/03/2022

                                    defNotesFile.IsUpload = itemCommentFiles.IsUpload;
                                    defNotesFile.NoteUniqueID = itemCommentFiles.NoteUniqueID;
                                    defNotesFile.CommentFileUniqueID = itemCommentFiles.CommentFileUniqueID;
                                    dbContext.GIRDeficienciesCommentFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }

                //CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                //Modals.GIRDeficienciesCommentFile com = new Modals.GIRDeficienciesCommentFile();
                //var dbDefComments = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesID == DeficienciesID).ToList();
                //if (dbDefComments != null && dbDefComments.Count > 0)
                //{
                //    foreach (var item in dbDefComments)
                //    {
                //        if (DeficienciesID > 0)
                //        {
                //            var commentFiles = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteID == item.NoteID).ToList();
                //            foreach (var itemCommentFiles in commentFiles)
                //            {
                //                dbContext.GIRDeficienciesCommentFiles.Remove(itemCommentFiles);
                //                dbContext.SaveChanges();
                //            }

                //            dbContext.GIRDeficienciesNotes.Remove(item);
                //            dbContext.SaveChanges();
                //        }
                //    }
                //}

                //foreach (var item in modalDefNotes)
                //{
                //    Entity.GIRDeficienciesNote defNotes = new Entity.GIRDeficienciesNote();
                //    defNotes.DeficienciesID = item.DeficienciesID;
                //    defNotes.UserName = item.UserName;
                //    defNotes.Comment = item.Comment;
                //    defNotes.CreatedDate = item.CreatedDate;
                //    defNotes.ModifyDate = item.ModifyDate;

                //    dbContext.GIRDeficienciesNotes.Add(defNotes);
                //    dbContext.SaveChanges();

                //    if (item.GIRDeficienciesCommentFile != null && item.GIRDeficienciesCommentFile.Count > 0)
                //    {
                //        foreach (var itemCommentFiles in item.GIRDeficienciesCommentFile)
                //        {
                //            Entity.GIRDeficienciesCommentFile commentFile = new Entity.GIRDeficienciesCommentFile();

                //        }
                //    }

                //}
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GIRDeficienciesComments_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRDeficienciesInitialActions_Save(List<Modals.GIRDeficienciesInitialActions> modalDefNotes, Guid? DeficienciesID   //RDBJ 09/22/2021 Updateed Modal Name used
                , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
                , ref bool IsNeedToSendNotification // JSL 06/24/2022
            ) 
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modalDefNotes != null && modalDefNotes.Count > 0)
                {
                    foreach (var itemDefNotes in modalDefNotes)
                    {
                        Entity.GIRDeficienciesInitialAction dbModal = new Entity.GIRDeficienciesInitialAction();
                        dbModal = dbContext.GIRDeficienciesInitialActions.Where(x => x.IniActUniqueID == itemDefNotes.IniActUniqueID).FirstOrDefault();

                        if (dbModal == null)
                        {
                            Entity.GIRDeficienciesInitialAction defNotes = new Entity.GIRDeficienciesInitialAction();
                            defNotes.DeficienciesID = itemDefNotes.DeficienciesID;
                            defNotes.CreatedDate = itemDefNotes.CreatedDate;
                            defNotes.Description = itemDefNotes.Description;
                            defNotes.IniActUniqueID = itemDefNotes.IniActUniqueID;
                            defNotes.GIRFormID = itemDefNotes.GIRFormID;
                            defNotes.Name = itemDefNotes.Name;
                            defNotes.DeficienciesUniqueID = itemDefNotes.DeficienciesUniqueID;
                            //defNotes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/14/2021
                            defNotes.isNew = 0; // JSL 06/27/2022 //RDBJ 10/14/2021

                            dbContext.GIRDeficienciesInitialActions.Add(defNotes);
                            dbContext.SaveChanges();

                            // JSL 06/24/2022
                            IsNeedToSendNotification = true;
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "5";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeInitialAction;
                            dictNotificationData["IsDraft"] = Convert.ToString(true);
                            dictNotificationData["Title"] = AppStatic.GIRFormName;
                            dictNotificationData["DetailsURL"] = "GIRList/DeficienciesDetails?id=" + Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData);
                            // End JSL 06/24/2022
                        }
                        if (itemDefNotes.GIRDeficienciesInitialActionsFiles != null && itemDefNotes.GIRDeficienciesInitialActionsFiles.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefNotes.GIRDeficienciesInitialActionsFiles)
                            {
                                Entity.GIRDeficienciesInitialActionsFile commentFile = new Entity.GIRDeficienciesInitialActionsFile();
                                commentFile = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActFileUniqueID == itemCommentFiles.IniActFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.GIRDeficienciesInitialActionsFile defNotesFile = new Entity.GIRDeficienciesInitialActionsFile();
                                    defNotesFile.DeficienciesID = itemCommentFiles.DeficienciesID;
                                    defNotesFile.FileName = itemCommentFiles.FileName;
                                    defNotesFile.StorePath = itemCommentFiles.StorePath;

                                    // JSL 12/03/2022
                                    if (defNotesFile.StorePath.StartsWith("data:"))
                                    {
                                        dicFileMetaData["FileName"] = defNotesFile.FileName;
                                        dicFileMetaData["Base64FileData"] = defNotesFile.StorePath;

                                        // JSL 01/08/2023
                                        dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeInitialAction;
                                        dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentFiles.IniActUniqueID);
                                        // End JSL 01/08/2023

                                        defNotesFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                    }
                                    // End JSL 12/03/2022

                                    defNotesFile.IsUpload = itemCommentFiles.IsUpload;
                                    defNotesFile.IniActUniqueID = itemCommentFiles.IniActUniqueID;
                                    defNotesFile.IniActFileUniqueID = itemCommentFiles.IniActFileUniqueID;
                                    dbContext.GIRDeficienciesInitialActionsFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GIRDeficienciesInitialActions_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        public void GIRDeficienciesResolution_Save(List<Modals.GIRDeficienciesResolution> modalDefNotes, Guid? DeficienciesID //RDBJ 09/22/2021 Updateed Modal Name used
             , Dictionary<string, string> dicFileMetaData   // JSL 12/03/2022
             , ref bool IsNeedToSendNotification // JSL 06/24/2022
            )
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                if (modalDefNotes != null && modalDefNotes.Count > 0)
                {
                    foreach (var itemDefNotes in modalDefNotes)
                    {
                        Entity.GIRDeficienciesResolution dbModal = new Entity.GIRDeficienciesResolution();
                        dbModal = dbContext.GIRDeficienciesResolutions.Where(x => x.ResolutionUniqueID == itemDefNotes.ResolutionUniqueID).FirstOrDefault();

                        if (dbModal == null)
                        {
                            Entity.GIRDeficienciesResolution defNotes = new Entity.GIRDeficienciesResolution();
                            defNotes.DeficienciesID = itemDefNotes.DeficienciesID;
                            defNotes.CreatedDate = itemDefNotes.CreatedDate;
                            defNotes.Resolution = itemDefNotes.Resolution;
                            defNotes.ResolutionUniqueID = itemDefNotes.ResolutionUniqueID;
                            defNotes.GIRFormID = itemDefNotes.GIRFormID;
                            defNotes.DeficienciesUniqueID = itemDefNotes.DeficienciesUniqueID;
                            defNotes.Name = itemDefNotes.Name;
                            //defNotes.isNew = 1;   // JSL 06/27/2022 commented this line //RDBJ 10/14/2021
                            defNotes.isNew = 0; // JSL 06/27/2022 //RDBJ 10/14/2021

                            dbContext.GIRDeficienciesResolutions.Add(defNotes);
                            dbContext.SaveChanges();

                            // JSL 06/24/2022
                            IsNeedToSendNotification = true;
                            Dictionary<string, string> dictNotificationData = new Dictionary<string, string>();
                            dictNotificationData["UserGroup"] = "5";
                            dictNotificationData["UniqueDataId"] = Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            //dictNotificationData["DataType"] = AppStatic.NotificationTypeResolution;    // JSL 07/06/2022 commented
                            dictNotificationData["DataType"] = AppStatic.NotificationTypeComment;   // JSL 07/06/2022
                            dictNotificationData["IsDraft"] = Convert.ToString(true);
                            dictNotificationData["Title"] = AppStatic.GIRFormName;
                            dictNotificationData["DetailsURL"] = "GIRList/DeficienciesDetails?id=" + Convert.ToString(itemDefNotes.DeficienciesUniqueID);
                            NotificationsHelper.SaveNotificationsDataForEachUser(dictNotificationData);
                            // End JSL 06/24/2022
                        }
                        if (itemDefNotes.GIRDeficienciesResolutionFiles != null && itemDefNotes.GIRDeficienciesResolutionFiles.Count > 0)
                        {
                            foreach (var itemCommentFiles in itemDefNotes.GIRDeficienciesResolutionFiles)
                            {
                                Entity.GIRDeficienciesResolutionFile commentFile = new Entity.GIRDeficienciesResolutionFile();
                                commentFile = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionFileUniqueID == itemCommentFiles.ResolutionFileUniqueID).FirstOrDefault();
                                if (commentFile == null)
                                {
                                    Entity.GIRDeficienciesResolutionFile defNotesFile = new Entity.GIRDeficienciesResolutionFile();
                                    defNotesFile.DeficienciesID = itemCommentFiles.DeficienciesID;
                                    defNotesFile.FileName = itemCommentFiles.FileName;
                                    defNotesFile.StorePath = itemCommentFiles.StorePath;

                                    // JSL 12/03/2022
                                    if (defNotesFile.StorePath.StartsWith("data:"))
                                    {
                                        dicFileMetaData["FileName"] = defNotesFile.FileName;
                                        dicFileMetaData["Base64FileData"] = defNotesFile.StorePath;

                                        // JSL 01/08/2023
                                        dicFileMetaData["SubDetailType"] = AppStatic.NotificationTypeResolution;
                                        dicFileMetaData["SubDetailUniqueId"] = Convert.ToString(itemCommentFiles.ResolutionUniqueID);
                                        // End JSL 01/08/2023

                                        defNotesFile.StorePath = Utility.ConvertBase64IntoFile(dicFileMetaData);
                                    }
                                    // End JSL 12/03/2022

                                    defNotesFile.IsUpload = itemCommentFiles.IsUpload;
                                    defNotesFile.ResolutionUniqueID = itemCommentFiles.ResolutionUniqueID;
                                    defNotesFile.ResolutionFileUniqueID = itemCommentFiles.ResolutionFileUniqueID;
                                    dbContext.GIRDeficienciesResolutionFiles.Add(defNotesFile);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GIRDeficienciesResolution_Save " + ex.Message + "\n" + ex.InnerException);
            }
        }
        
        public List<GeneralInspectionReport> getUnsynchGIRList(
            string strShipCode  // JSL 11/12/2022
            )
        {
            List<GeneralInspectionReport> unSyncList = new List<GeneralInspectionReport>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var girList = dbContext.GeneralInspectionReports
                    .Where(x => x.IsSynced == false
                    && x.UniqueFormID != null   // RDBJ 12/18/2021
                    ).ToList();

                if (girList != null && girList.Count > 0)
                {
                    // JSL 11/12/2022
                    if (!string.IsNullOrEmpty(strShipCode))
                    {
                        girList = girList.Where(x => x.Ship == strShipCode).ToList();
                    }
                    // End JSL 11/12/2022

                    foreach (var item in girList)
                    {
                        GeneralInspectionReport dbModal = new GeneralInspectionReport();
                        dbModal.GIRDeficiencies = new List<GIRDeficiencies>();
                        dbModal.GIRSafeManningRequirements = new List<GlRSafeManningRequirements>();
                        dbModal.GIRCrewDocuments = new List<GlRCrewDocuments>();
                        dbModal.GIRRestandWorkHours = new List<GIRRestandWorkHours>();
                        dbModal.GIRPhotographs = new List<GIRPhotographs>();

                        GetGIRFormData(item, ref dbModal);

                        // RDBJ 01/05/2022 wrapped in if
                        if (dbModal.isDelete == 0)
                        {
                            var defList = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                            if (defList != null && defList.Count > 0)
                            {
                                foreach (var def in defList)
                                {
                                    GIRDeficiencies girDef = new GIRDeficiencies();

                                    girDef.GIRDeficienciesComments = new List<DeficienciesNote>();
                                    girDef.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>(); //RDBJ 09/23/2021
                                    girDef.GIRDeficienciesResolution = new List<Modals.GIRDeficienciesResolution>(); //RDBJ 09/23/2021

                                    girDef.No = def.No;
                                    girDef.DateRaised = def.DateRaised;
                                    girDef.Deficiency = def.Deficiency;
                                    girDef.DateClosed = def.DateClosed;
                                    girDef.CreatedDate = def.CreatedDate;
                                    girDef.UpdatedDate = def.UpdatedDate;
                                    girDef.DeficienciesUniqueID = def.DeficienciesUniqueID;
                                    girDef.Ship = def.Ship;
                                    girDef.IsClose = def.IsClose;
                                    girDef.ReportType = def.ReportType;
                                    girDef.ItemNo = def.ItemNo;
                                    girDef.Section = def.Section;
                                    girDef.UniqueFormID = def.UniqueFormID;
                                    girDef.isDelete = def.isDelete;
                                    girDef.Priority = def.Priority == null ? 12 : def.Priority; //RDBJ 11/01/2021
                                    girDef.AssignTo = def.AssignTo; // RDBJ 12/18/2021
                                    girDef.DueDate = def.DueDate;   // RDBJ 03/01/2022

                                    // RDBJ 12/23/2021 wrapped in if
                                    if (def.isDelete == 0)
                                    {
                                        var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == girDef.DeficienciesUniqueID).ToList();
                                        if (defFiles != null && defFiles.Count > 0)
                                        {
                                            foreach (var girDefFile in defFiles)
                                            {
                                                Modals.GIRDeficienciesFile defFile = new Modals.GIRDeficienciesFile();
                                                defFile.DeficienciesFileUniqueID = girDefFile.DeficienciesUniqueID; // JSL 06/07/2022
                                                defFile.DeficienciesID = girDefFile.DeficienciesID != null ? girDefFile.DeficienciesID : 0; // RDBJ 01/15/2022 set avoid null error
                                                defFile.DeficienciesUniqueID = girDefFile.DeficienciesUniqueID;
                                                defFile.FileName = girDefFile.FileName;
                                                defFile.StorePath = girDefFile.StorePath;

                                                // JSL 12/04/2022
                                                if (!defFile.StorePath.StartsWith("data:"))
                                                {
                                                    defFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defFile.StorePath);
                                                }
                                                // End JSL 12/04/2022

                                                girDef.GIRDeficienciesFile.Add(defFile);
                                            }
                                        }

                                        var defComments = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                        if (defComments != null && defComments.Count > 0)
                                        {
                                            foreach (var defComment in defComments)
                                            {
                                                DeficienciesNote defNote = new DeficienciesNote();
                                                //defNote.DeficienciesID = defComment.DeficienciesID;
                                                defNote.DeficienciesUniqueID = defComment.DeficienciesUniqueID;
                                                defNote.UserName = defComment.UserName;
                                                defNote.Comment = defComment.Comment;
                                                defNote.CreatedDate = defComment.CreatedDate;
                                                defNote.ModifyDate = defComment.ModifyDate;
                                                defNote.NoteUniqueID = defComment.NoteUniqueID;

                                                var defCommentFiles = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteUniqueID == defNote.NoteUniqueID).ToList();
                                                if (defCommentFiles != null && defCommentFiles.Count > 0)
                                                {
                                                    foreach (var defCommmentFile in defCommentFiles)
                                                    {
                                                        Modals.GIRDeficienciesCommentFile defComFile = new Modals.GIRDeficienciesCommentFile();
                                                        //defComFile.DeficienciesID = defCommmentFile.DeficienciesID;
                                                        defComFile.FileName = defCommmentFile.FileName;
                                                        defComFile.StorePath = defCommmentFile.StorePath;
                                                        defComFile.IsUpload = defCommmentFile.IsUpload;
                                                        defComFile.NoteUniqueID = defCommmentFile.NoteUniqueID;
                                                        defComFile.CommentFileUniqueID = defCommmentFile.CommentFileUniqueID;
                                                        // JSL 12/04/2022
                                                        if (!defComFile.StorePath.StartsWith("data:"))
                                                        {
                                                            defComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defComFile.StorePath);
                                                        }
                                                        // End JSL 12/04/2022

                                                        defNote.GIRDeficienciesCommentFile.Add(defComFile);
                                                    }
                                                }
                                                girDef.GIRDeficienciesComments.Add(defNote);
                                            }
                                        }

                                        var defIntialActions = dbContext.GIRDeficienciesInitialActions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                        if (defIntialActions != null && defIntialActions.Count > 0)
                                        {
                                            foreach (var defInitial in defIntialActions)
                                            {
                                                GIRDeficienciesInitialActions defIntialAction = new GIRDeficienciesInitialActions(); //RDBJ 09/22/2021 Updateed Modal Name used
                                                                                                                                     //defIntialAction.DeficienciesID = defInitial.DeficienciesID;
                                                defIntialAction.DeficienciesUniqueID = defInitial.DeficienciesUniqueID;
                                                defIntialAction.Name = defInitial.Name;
                                                defIntialAction.Description = defInitial.Description;
                                                defIntialAction.CreatedDate = defInitial.CreatedDate;
                                                defIntialAction.IniActUniqueID = defInitial.IniActUniqueID;

                                                var defIntialActionFiles = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActUniqueID == defInitial.IniActUniqueID).ToList();
                                                if (defIntialActionFiles != null && defIntialActionFiles.Count > 0)
                                                {
                                                    foreach (var defIntialActionFile in defIntialActionFiles)
                                                    {
                                                        Modals.GIRDeficienciesInitialActionsFile defComFile = new Modals.GIRDeficienciesInitialActionsFile();
                                                        //defComFile.DeficienciesID = defIntialActionFile.DeficienciesID;
                                                        defComFile.FileName = defIntialActionFile.FileName;
                                                        defComFile.StorePath = defIntialActionFile.StorePath;
                                                        defComFile.IsUpload = defIntialActionFile.IsUpload;
                                                        defComFile.IniActUniqueID = defIntialActionFile.IniActUniqueID;
                                                        defComFile.IniActFileUniqueID = defIntialActionFile.IniActFileUniqueID;

                                                        // JSL 12/04/2022
                                                        if (!defComFile.StorePath.StartsWith("data:"))
                                                        {
                                                            defComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defComFile.StorePath);
                                                        }
                                                        // End JSL 12/04/2022

                                                        defIntialAction.GIRDeficienciesInitialActionsFiles.Add(defComFile);
                                                    }
                                                }
                                                girDef.GIRDeficienciesInitialActions.Add(defIntialAction); //RDBJ 09/22/2021 Updateed Modal Name used
                                            }
                                        }

                                        var defResolutions = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                        if (defResolutions != null && defResolutions.Count > 0)
                                        {
                                            foreach (var defResolution in defResolutions)
                                            {
                                                Modals.GIRDeficienciesResolution defResolutionModal = new Modals.GIRDeficienciesResolution(); //RDBJ 09/22/2021 Updateed Modal Name used
                                                                                                                                              //defResolutionModal.DeficienciesID = defResolution.DeficienciesID;
                                                defResolutionModal.DeficienciesUniqueID = defResolution.DeficienciesUniqueID;
                                                defResolutionModal.Name = defResolution.Name;
                                                defResolutionModal.Resolution = defResolution.Resolution;
                                                defResolutionModal.CreatedDate = defResolution.CreatedDate;
                                                defResolutionModal.ResolutionUniqueID = defResolution.ResolutionUniqueID;

                                                var defResolutionFiles = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == defResolution.ResolutionUniqueID).ToList();
                                                if (defResolutionFiles != null && defResolutionFiles.Count > 0)
                                                {
                                                    foreach (var defResolutionFile in defResolutionFiles)
                                                    {
                                                        Modals.GIRDeficienciesResolutionFile defresFile = new Modals.GIRDeficienciesResolutionFile();
                                                        defresFile.DeficienciesID = defResolutionFile.DeficienciesID;
                                                        defresFile.FileName = defResolutionFile.FileName;
                                                        defresFile.StorePath = defResolutionFile.StorePath;
                                                        defresFile.IsUpload = defResolutionFile.IsUpload;
                                                        defresFile.ResolutionUniqueID = defResolutionFile.ResolutionUniqueID;
                                                        defresFile.ResolutionFileUniqueID = defResolutionFile.ResolutionFileUniqueID;

                                                        // JSL 12/04/2022
                                                        if (!defresFile.StorePath.StartsWith("data:"))
                                                        {
                                                            defresFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defresFile.StorePath);
                                                        }
                                                        // End JSL 12/04/2022

                                                        defResolutionModal.GIRDeficienciesResolutionFiles.Add(defresFile);
                                                    }
                                                }
                                                girDef.GIRDeficienciesResolution.Add(defResolutionModal); //RDBJ 09/22/2021 Updateed Modal Name used
                                            }
                                        }
                                    }

                                    dbModal.GIRDeficiencies.Add(girDef);
                                }
                            }

                            var safeManning = dbContext.GlRSafeManningRequirements.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                            if (safeManning != null && safeManning.Count > 0)
                            {
                                foreach (var safMan in safeManning)
                                {
                                    GlRSafeManningRequirements mSafMan = new GlRSafeManningRequirements();
                                    mSafMan.GIRFormID = 0;
                                    mSafMan.UniqueFormID = safMan.UniqueFormID;
                                    mSafMan.Ship = safMan.Ship;
                                    mSafMan.Rank = safMan.Rank;
                                    mSafMan.RequiredbySMD = safMan.RequiredbySMD;
                                    mSafMan.OnBoard = safMan.OnBoard;
                                    mSafMan.CreatedDate = safMan.CreatedDate;
                                    mSafMan.UpdatedDate = safMan.UpdatedDate;

                                    dbModal.GIRSafeManningRequirements.Add(mSafMan);
                                }
                            }

                            var creDocs = dbContext.GlRCrewDocuments.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                            if (creDocs != null && creDocs.Count > 0)
                            {
                                foreach (var creDoc in creDocs)
                                {
                                    GlRCrewDocuments mCreDoc = new GlRCrewDocuments();
                                    mCreDoc.GIRFormID = 0;
                                    mCreDoc.UniqueFormID = creDoc.UniqueFormID;
                                    mCreDoc.Ship = creDoc.Ship;
                                    mCreDoc.CrewmemberName = creDoc.CrewmemberName;
                                    mCreDoc.CertificationDetail = creDoc.CertificationDetail;
                                    mCreDoc.CreatedDate = creDoc.CreatedDate;
                                    mCreDoc.UpdatedDate = creDoc.UpdatedDate;

                                    dbModal.GIRCrewDocuments.Add(mCreDoc);
                                }
                            }

                            var restWNH = dbContext.GIRRestandWorkHours.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                            if (restWNH != null && restWNH.Count > 0)
                            {
                                foreach (var restItem in restWNH)
                                {
                                    GIRRestandWorkHours workHours = new GIRRestandWorkHours();
                                    workHours.GIRFormID = 0;
                                    workHours.UniqueFormID = restItem.UniqueFormID;
                                    workHours.Ship = restItem.Ship;
                                    workHours.CrewmemberName = restItem.CrewmemberName;
                                    workHours.RestAndWorkDetail = restItem.RestAndWorkDetail;
                                    workHours.CreatedDate = restItem.CreatedDate;
                                    workHours.UpdatedDate = restItem.UpdatedDate;

                                    dbModal.GIRRestandWorkHours.Add(workHours);
                                }
                            }

                            var girPhotos = dbContext.GIRPhotographs.Where(x => x.UniqueFormID == item.UniqueFormID).ToList();
                            if (girPhotos != null && girPhotos.Count > 0)
                            {
                                foreach (var gitPho in girPhotos)
                                {
                                    GIRPhotographs photo = new GIRPhotographs();
                                    photo.GIRFormID = 0;
                                    photo.UniqueFormID = gitPho.UniqueFormID;
                                    photo.FileName = gitPho.FileName;
                                    photo.ImagePath = gitPho.ImagePath;
                                    photo.ImageCaption = gitPho.ImageCaption;
                                    photo.Ship = gitPho.Ship;
                                    photo.CreatedDate = gitPho.CreatedDate;
                                    photo.UpdatedDate = gitPho.UpdatedDate;

                                    // JSL 12/04/2022
                                    if (!photo.ImagePath.StartsWith("data:"))
                                    {
                                        photo.ImagePath = Utility.ConvertIntoBase64EndCodedUploadedFile(photo.ImagePath);
                                    }
                                    // End JSL 12/04/2022

                                    dbModal.GIRPhotographs.Add(photo);
                                }
                            }
                        }

                        unSyncList.Add(dbModal);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getUnsynchGIRList " + ex.Message.ToString() + "\n" + ex.InnerException.ToString());
                unSyncList = null;
            }
            return unSyncList;
        }

        public bool sendSynchGIRListUFID(List<string> IdsStr) // RDBJ 01/19/2022 set List<string>
        {
            bool response = false;
            //string[] FormUID = IdsStr.Split(',');
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                for (int i = 0; i < IdsStr.Count; i++) // RDBJ 01/19/2022 set with List
                {
                    Guid UFID = Guid.Parse(IdsStr[i]);  // RDBJ 01/19/2022 set with List
                    Entity.GeneralInspectionReport girForms = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == UFID).FirstOrDefault();
                    girForms.IsSynced = true;
                }
                dbContext.SaveChanges();
                response = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud sendSynchGIRListUFID " + ex.Message + "\n" + ex.InnerException);
                response = false;
            }
            return response;
        }
        public bool sendSynchSIRListUFID(string IdsStr)
        {
            bool response = false;
            string[] FormUID = IdsStr.Split(',');
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                for (int i = 0; i < FormUID.Length; i++)
                {
                    Guid UFID = Guid.Parse(FormUID[i]);
                    Entity.SuperintendedInspectionReport girForms = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == UFID).FirstOrDefault();
                    girForms.IsSynced = true;
                }
                dbContext.SaveChanges();
                response = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud sendSynchSIRListUFID " + ex.Message + "\n" + ex.InnerException);
                response = false;
            }
            return response;
        }
        public void GetGIRFormData(Entity.GeneralInspectionReport Modal, ref Modals.GeneralInspectionReport dbModal)
        {
            dbModal.UniqueFormID = Modal.UniqueFormID;
            dbModal.FormVersion = Modal.FormVersion;
            dbModal.CreatedDate = Modal.CreatedDate;
            dbModal.UpdatedDate = Modal.UpdatedDate;    // RDBJ 01/15/2022
            dbModal.isDelete = Modal.isDelete; // RDBJ 01/05/2022

            dbModal.ShipID = Modal.ShipID;
            dbModal.ShipName = Modal.ShipName;
            dbModal.Ship = Modal.Ship;
            dbModal.Port = Modal.Port;
            dbModal.Inspector = Modal.Inspector;
            dbModal.Date = Modal.Date;
            dbModal.GeneralPreamble = Modal.GeneralPreamble;
            dbModal.Classsociety = Modal.Classsociety;
            dbModal.YearofBuild = Modal.YearofBuild;
            dbModal.Flag = Modal.Flag;
            dbModal.Classofvessel = Modal.Classofvessel;
            dbModal.Portofregistry = Modal.Portofregistry;
            dbModal.MMSI = Modal.MMSI;
            dbModal.IMOnumber = Modal.IMOnumber;
            dbModal.Callsign = Modal.Callsign;
            dbModal.SummerDWT = Modal.SummerDWT;
            dbModal.Grosstonnage = Modal.Grosstonnage;
            dbModal.Lightweight = Modal.Lightweight;
            dbModal.Nettonnage = Modal.Nettonnage;
            dbModal.Beam = Modal.Beam;
            dbModal.LOA = Modal.LOA;
            dbModal.Summerdraft = Modal.Summerdraft;
            dbModal.LBP = Modal.LBP;
            dbModal.Bowthruster = Modal.Bowthruster;
            dbModal.BHP = Modal.BHP;
            dbModal.Noofholds = Modal.Noofholds;
            dbModal.Nomoveablebulkheads = Modal.Nomoveablebulkheads;
            dbModal.Containers = Modal.Containers;
            dbModal.Cargocapacity = Modal.Cargocapacity;
            dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
            dbModal.Cargohandlingequipment = Modal.Cargohandlingequipment;
            dbModal.Lastvoyageandcargo = Modal.Lastvoyageandcargo;
            dbModal.CurrentPlannedvoyageandcargo = Modal.CurrentPlannedvoyageandcargo;
            dbModal.ShipboardWorkingArrangements = Modal.ShipboardWorkingArrangements;
            dbModal.CertificationIndex = Modal.CertificationIndex;
            dbModal.IsPubsAndDocsSectionComplete = Modal.IsPubsAndDocsSectionComplete;
            dbModal.CarriedOutByTheDOOW = Modal.CarriedOutByTheDOOW;
            dbModal.IsRegs4shipsDVD = Modal.IsRegs4shipsDVD;
            dbModal.Regs4shipsDVD = Modal.Regs4shipsDVD;
            dbModal.IsSOPEPPoints = Modal.IsSOPEPPoints;
            dbModal.SOPEPPoints = Modal.SOPEPPoints;
            dbModal.IsBWMP = Modal.IsBWMP;
            dbModal.BWMP = Modal.BWMP;
            dbModal.IsBWMPSupplement = Modal.IsBWMPSupplement;
            dbModal.BWMPSupplement = Modal.BWMPSupplement;
            dbModal.IsIntactStabilityManual = Modal.IsIntactStabilityManual;
            dbModal.IntactStabilityManual = Modal.IntactStabilityManual;
            dbModal.IsStabilityComputer = Modal.IsStabilityComputer;
            dbModal.StabilityComputer = Modal.StabilityComputer;
            dbModal.IsDateOfLast = Modal.IsDateOfLast;
            dbModal.DateOfLast = Modal.DateOfLast;
            dbModal.IsCargoSecuring = Modal.IsCargoSecuring;
            dbModal.CargoSecuring = Modal.CargoSecuring;
            dbModal.BulkCargo = Modal.BulkCargo;
            dbModal.IsBulkCargo = Modal.IBulkCargo;
            dbModal.IsSMSManual = Modal.IsSMSManual;
            dbModal.SMSManual = Modal.SMSManual;
            dbModal.IsRegisterOf = Modal.IsRegisterOf;
            dbModal.RegisterOf = Modal.RegisterOf;
            dbModal.IsFleetStandingOrder = Modal.IsFleetStandingOrder;
            dbModal.FleetStandingOrder = Modal.FleetStandingOrder;
            dbModal.IsFleetMemoranda = Modal.IsFleetMemoranda;
            dbModal.FleetMemoranda = Modal.FleetMemoranda;
            dbModal.IsShipsPlans = Modal.IsShipsPlans;
            dbModal.ShipsPlans = Modal.ShipsPlans;
            dbModal.IsCollective = Modal.IsCollective;
            dbModal.Collective = Modal.Collective;
            dbModal.IsDraftAndFreeboardNotice = Modal.IsDraftAndFreeboardNotice;
            dbModal.DraftAndFreeboardNotice = Modal.DraftAndFreeboardNotice;
            dbModal.IsPCSOPEP = Modal.IsPCSOPEP;
            dbModal.PCSOPEP = Modal.PCOPEP;
            dbModal.IsNTVRP = Modal.INTVRP;
            dbModal.NTVRP = Modal.NTVRP;
            dbModal.IsVGP = Modal.IVGP;
            dbModal.VGP = Modal.VGP;
            dbModal.PubsComments = Modal.PubsComments;
            dbModal.OfficialLogbookA = Modal.OfficialLogbookA;
            dbModal.OfficialLogbookB = Modal.OfficialLogbookB;
            dbModal.OfficialLogbookC = Modal.OfficialLogbookC;
            dbModal.OfficialLogbookD = Modal.OfficialLogbookD;
            dbModal.OfficialLogbookE = Modal.OfficialLogbookE;
            dbModal.DeckLogbook = Modal.DeckLogbook;
            dbModal.Listofcrew = Modal.Listofcrew;
            dbModal.LastHose = Modal.LastHose;
            dbModal.PassagePlanning = Modal.PassagePlanning;
            dbModal.LoadingComputer = Modal.LoadingComputer;
            dbModal.EngineLogbook = Modal.EngineLogbook;
            dbModal.OilRecordBook = Modal.OilRecordBook;
            dbModal.RiskAssessments = Modal.RiskAssessments;
            dbModal.GMDSSLogbook = Modal.GMDSSLogbook;
            dbModal.DeckLogbook5D = Modal.DeckLogbook5D;
            dbModal.GarbageRecordBook = Modal.GarbageRecordBook;
            dbModal.BallastWaterRecordBook = Modal.BallastWaterRecordBook;
            dbModal.CargoRecordBook = Modal.CargoRecordBook;
            dbModal.EmissionsControlManual = Modal.EmissionsControlManual;
            dbModal.LGR = Modal.LGR;
            dbModal.PEER = Modal.PEER;
            dbModal.RecordKeepingComments = Modal.RecordKeepingComments;
            dbModal.LastPortStateControl = Modal.LastPortStateControl;

            dbModal.LiferaftsComment = Modal.LiferaftsComment;
            dbModal.releasesComment = Modal.releasesComment;
            dbModal.LifeboatComment = Modal.LifeboatComment;
            dbModal.LifeboatdavitComment = Modal.LifeboatdavitComment;
            dbModal.LifeboatequipmentComment = Modal.LifeboatequipmentComment;
            dbModal.RescueboatComment = Modal.RescueboatComment;
            dbModal.RescueboatequipmentComment = Modal.RescueboatequipmentComment;
            dbModal.RescueboatoutboardmotorComment = Modal.RescueboatoutboardmotorComment;
            dbModal.RescueboatdavitComment = Modal.RescueboatdavitComment;
            dbModal.DeckComment = Modal.DeckComment;
            dbModal.PyrotechnicsComment = Modal.PyrotechnicsComment;
            dbModal.EPIRBComment = Modal.EPIRBComment;
            dbModal.SARTsComment = Modal.SARTsComment;
            dbModal.GMDSSComment = Modal.GMDSSComment;
            dbModal.ManoverboardComment = Modal.ManoverboardComment;
            dbModal.LinethrowingapparatusComment = Modal.LinethrowingapparatusComment;
            dbModal.FireextinguishersComment = Modal.FireextinguishersComment;
            dbModal.EmergencygeneratorComment = Modal.EmergencygeneratorComment;
            dbModal.CO2roomComment = Modal.CO2roomComment;
            dbModal.SurvivalComment = Modal.SurvivalComment;
            dbModal.LifejacketComment = Modal.LifejacketComment;
            dbModal.FiremansComment = Modal.FiremansComment;
            dbModal.LifebuoysComment = Modal.LifebuoysComment;
            dbModal.FireboxesComment = Modal.FireboxesComment;
            dbModal.EmergencybellsComment = Modal.EmergencybellsComment;
            dbModal.EmergencylightingComment = Modal.EmergencylightingComment;
            dbModal.FireplanComment = Modal.FireplanComment;
            dbModal.DamageComment = Modal.DamageComment;
            dbModal.EmergencyplansComment = Modal.EmergencyplansComment;
            dbModal.MusterlistComment = Modal.MusterlistComment;
            dbModal.SafetysignsComment = Modal.SafetysignsComment;
            dbModal.EmergencysteeringComment = Modal.EmergencysteeringComment;
            dbModal.StatutoryemergencydrillsComment = Modal.StatutoryemergencydrillsComment;
            dbModal.EEBDComment = Modal.EEBDComment;
            dbModal.OxygenComment = Modal.OxygenComment;
            dbModal.MultigasdetectorComment = Modal.MultigasdetectorComment;
            dbModal.GasdetectorComment = Modal.GasdetectorComment;
            dbModal.SufficientquantityComment = Modal.SufficientquantityComment;
            dbModal.BASetsComment = Modal.BASetsComment;
            dbModal.SafetyComment = Modal.SafetyComment;

            dbModal.GangwayComment = Modal.GangwayComment;
            dbModal.RestrictedComment = Modal.RestrictedComment;
            dbModal.OutsideComment = Modal.OutsideComment;
            dbModal.EntrancedoorsComment = Modal.EntrancedoorsComment;
            dbModal.AccommodationComment = Modal.AccommodationComment;
            dbModal.GMDSSComment5G = Modal.GMDSSComment5G;
            dbModal.VariousComment = Modal.VariousComment;
            dbModal.SSOComment = Modal.SSOComment;
            dbModal.SecuritylogbookComment = Modal.SecuritylogbookComment;
            dbModal.Listoflast10portsComment = Modal.Listoflast10portsComment;
            dbModal.PFSOComment = Modal.PFSOComment;
            dbModal.SecuritylevelComment = Modal.SecuritylevelComment;
            dbModal.DrillsandtrainingComment = Modal.DrillsandtrainingComment;
            dbModal.DOSComment = Modal.DOSComment;
            dbModal.SSASComment = Modal.SSASComment;
            dbModal.VisitorslogbookComment = Modal.VisitorslogbookComment;
            dbModal.KeyregisterComment = Modal.KeyregisterComment;
            dbModal.ShipSecurityComment = Modal.ShipSecurityComment;
            dbModal.SecurityComment = Modal.SecurityComment;

            dbModal.NauticalchartsComment = Modal.NauticalchartsComment;
            dbModal.NoticetomarinersComment = Modal.NoticetomarinersComment;
            dbModal.ListofradiosignalsComment = Modal.ListofradiosignalsComment;
            dbModal.ListoflightsComment = Modal.ListoflightsComment;
            dbModal.SailingdirectionsComment = Modal.SailingdirectionsComment;
            dbModal.TidetablesComment = Modal.TidetablesComment;
            dbModal.NavtexandprinterComment = Modal.NavtexandprinterComment;
            dbModal.RadarsComment = Modal.RadarsComment;
            dbModal.GPSComment = Modal.GPSComment;
            dbModal.AISComment = Modal.AISComment;
            dbModal.VDRComment = Modal.VDRComment;
            dbModal.ECDISComment = Modal.ECDISComment;
            dbModal.EchosounderComment = Modal.EchosounderComment;
            dbModal.ADPbackuplaptopComment = Modal.ADPbackuplaptopComment;
            dbModal.ColourprinterComment = Modal.ColourprinterComment;
            dbModal.VHFDSCtransceiverComment = Modal.VHFDSCtransceiverComment;
            dbModal.radioinstallationComment = Modal.radioinstallationComment;
            dbModal.InmarsatCComment = Modal.InmarsatCComment;
            dbModal.MagneticcompassComment = Modal.MagneticcompassComment;
            dbModal.SparecompassbowlComment = Modal.SparecompassbowlComment;
            dbModal.CompassobservationbookComment = Modal.CompassobservationbookComment;
            dbModal.GyrocompassComment = Modal.GyrocompassComment;
            dbModal.RudderindicatorComment = Modal.RudderindicatorComment;
            dbModal.SpeedlogComment = Modal.SpeedlogComment;
            dbModal.NavigationComment = Modal.NavigationComment;
            dbModal.SignalflagsComment = Modal.SignalflagsComment;
            dbModal.RPMComment = Modal.RPMComment;
            dbModal.BasicmanoeuvringdataComment = Modal.BasicmanoeuvringdataComment;
            dbModal.MasterstandingordersComment = Modal.MasterstandingordersComment;
            dbModal.MasternightordersbookComment = Modal.MasternightordersbookComment;
            dbModal.SextantComment = Modal.SextantComment;
            dbModal.AzimuthmirrorComment = Modal.AzimuthmirrorComment;
            dbModal.BridgepostersComment = Modal.BridgepostersComment;
            dbModal.ReviewofplannedComment = Modal.ReviewofplannedComment;
            dbModal.BridgebellbookComment = Modal.BridgebellbookComment;
            dbModal.BridgenavigationalComment = Modal.BridgenavigationalComment;
            dbModal.SecurityEquipmentComment = Modal.SecurityEquipmentComment;
            dbModal.NavigationPost = Modal.NavigationPost;

            dbModal.GeneralComment = Modal.GeneralComment;
            dbModal.MedicinestorageComment = Modal.MedicinestorageComment;
            dbModal.MedicinechestcertificateComment = Modal.MedicinechestcertificateComment;
            dbModal.InventoryStoresComment = Modal.InventoryStoresComment;
            dbModal.OxygencylindersComment = Modal.OxygencylindersComment;
            dbModal.StretcherComment = Modal.StretcherComment;
            dbModal.SalivaComment = Modal.SalivaComment;
            dbModal.AlcoholComment = Modal.AlcoholComment;
            dbModal.HospitalComment = Modal.HospitalComment;

            dbModal.GeneralGalleyComment = Modal.GeneralGalleyComment;
            dbModal.HygieneComment = Modal.HygieneComment;
            dbModal.FoodstorageComment = Modal.FoodstorageComment;
            dbModal.FoodlabellingComment = Modal.FoodlabellingComment;
            dbModal.GalleyriskassessmentComment = Modal.GalleyriskassessmentComment;
            dbModal.FridgetemperatureComment = Modal.FridgetemperatureComment;
            dbModal.FoodandProvisionsComment = Modal.FoodandProvisionsComment;
            dbModal.GalleyComment = Modal.GalleyComment;

            dbModal.ConditionComment = Modal.ConditionComment;
            dbModal.PaintworkComment = Modal.PaintworkComment;
            dbModal.LightingComment = Modal.LightingComment;
            dbModal.PlatesComment = Modal.PlatesComment;
            dbModal.BilgesComment = Modal.BilgesComment;
            dbModal.PipelinesandvalvesComment = Modal.PipelinesandvalvesComment;
            dbModal.LeakageComment = Modal.LeakageComment;
            dbModal.EquipmentComment = Modal.EquipmentComment;
            dbModal.OilywaterseparatorComment = Modal.OilywaterseparatorComment;
            dbModal.FueloiltransferplanComment = Modal.FueloiltransferplanComment;
            dbModal.SteeringgearComment = Modal.SteeringgearComment;
            dbModal.WorkshopandequipmentComment = Modal.WorkshopandequipmentComment;
            dbModal.SoundingpipesComment = Modal.SoundingpipesComment;
            dbModal.EnginecontrolComment = Modal.EnginecontrolComment;
            dbModal.ChiefEngineernightordersbookComment = Modal.ChiefEngineernightordersbookComment;
            dbModal.ChiefEngineerstandingordersComment = Modal.ChiefEngineerstandingordersComment;
            dbModal.PreUMSComment = Modal.PreUMSComment;
            dbModal.EnginebellbookComment = Modal.EnginebellbookComment;
            dbModal.LockoutComment = Modal.LockoutComment;
            dbModal.EngineRoomComment = Modal.EngineRoomComment;

            dbModal.CleanlinessandhygieneComment = Modal.CleanlinessandhygieneComment;
            dbModal.ConditionComment5M = Modal.ConditionComment5M;
            dbModal.PaintworkComment5M = Modal.PaintworkComment5M;
            dbModal.SignalmastandstaysComment = Modal.SignalmastandstaysComment;
            dbModal.MonkeyislandComment = Modal.MonkeyislandComment;
            dbModal.FireDampersComment = Modal.FireDampersComment;
            dbModal.RailsBulwarksComment = Modal.RailsBulwarksComment;
            dbModal.WatertightdoorsComment = Modal.WatertightdoorsComment;
            dbModal.VentilatorsComment = Modal.VentilatorsComment;
            dbModal.WinchesComment = Modal.WinchesComment;
            dbModal.FairleadsComment = Modal.FairleadsComment;
            dbModal.MooringLinesComment = Modal.MooringLinesComment;
            dbModal.EmergencyShutOffsComment = Modal.EmergencyShutOffsComment;
            dbModal.RadioaerialsComment = Modal.RadioaerialsComment;
            dbModal.SOPEPlockerComment = Modal.SOPEPlockerComment;
            dbModal.ChemicallockerComment = Modal.ChemicallockerComment;
            dbModal.AntislippaintComment = Modal.AntislippaintComment;
            dbModal.SuperstructureComment = Modal.SuperstructureComment;
            dbModal.CabinsComment = Modal.CabinsComment;
            dbModal.OfficesComment = Modal.OfficesComment;
            dbModal.MessroomsComment = Modal.MessroomsComment;
            dbModal.ToiletsComment = Modal.ToiletsComment;
            dbModal.LaundryroomComment = Modal.LaundryroomComment;
            dbModal.ChangingroomComment = Modal.ChangingroomComment;
            dbModal.OtherComment = Modal.OtherComment;
            dbModal.ConditionComment5N = Modal.ConditionComment5N;
            dbModal.SelfclosingfiredoorsComment = Modal.SelfclosingfiredoorsComment;
            dbModal.StairwellsComment = Modal.StairwellsComment;
            dbModal.SuperstructureInternalComment = Modal.SuperstructureInternalComment;

            dbModal.PortablegangwayComment = Modal.PortablegangwayComment;
            dbModal.SafetynetComment = Modal.SafetynetComment;
            dbModal.AccommodationLadderComment = Modal.AccommodationLadderComment;
            dbModal.SafeaccessprovidedComment = Modal.SafeaccessprovidedComment;
            dbModal.PilotladdersComment = Modal.PilotladdersComment;
            dbModal.BoardingEquipmentComment = Modal.BoardingEquipmentComment;
            dbModal.CleanlinessComment = Modal.CleanlinessComment;
            dbModal.PaintworkComment5P = Modal.PaintworkComment5P;
            dbModal.ShipsiderailsComment = Modal.ShipsiderailsComment;
            dbModal.WeathertightdoorsComment = Modal.WeathertightdoorsComment;
            dbModal.FirehydrantsComment = Modal.FirehydrantsComment;
            dbModal.VentilatorsComment5P = Modal.VentilatorsComment5P;
            dbModal.ManholecoversComment = Modal.ManholecoversComment;
            dbModal.MainDeckAreaComment = Modal.MainDeckAreaComment;

            dbModal.ConditionComment5Q = Modal.ConditionComment5Q;
            dbModal.PaintworkComment5Q = Modal.PaintworkComment5Q;
            dbModal.MechanicaldamageComment = Modal.MechanicaldamageComment;
            dbModal.AccessladdersComment = Modal.AccessladdersComment;
            dbModal.ManholecoversComment5Q = Modal.ManholecoversComment5Q;
            dbModal.HoldbilgeComment = Modal.HoldbilgeComment;
            dbModal.AccessdoorsComment = Modal.AccessdoorsComment;
            dbModal.ConditionHatchCoversComment = Modal.ConditionHatchCoversComment;
            dbModal.PaintworkHatchCoversComment = Modal.PaintworkHatchCoversComment;
            dbModal.RubbersealsComment = Modal.RubbersealsComment;
            dbModal.SignsofhatchesComment = Modal.SignsofhatchesComment;
            dbModal.SealingtapeComment = Modal.SealingtapeComment;
            dbModal.ConditionofhydraulicsComment = Modal.ConditionofhydraulicsComment;
            dbModal.PortablebulkheadsComment = Modal.PortablebulkheadsComment;
            dbModal.TweendecksComment = Modal.TweendecksComment;
            dbModal.HatchcoamingComment = Modal.HatchcoamingComment;
            dbModal.ConditionCargoCranesComment = Modal.ConditionCargoCranesComment;
            dbModal.GantrycranealarmComment = Modal.GantrycranealarmComment;
            dbModal.GantryconditionComment = Modal.GantryconditionComment;
            dbModal.CargoHoldsComment = Modal.CargoHoldsComment;

            dbModal.CleanlinessComment5R = Modal.CleanlinessComment5R;
            dbModal.PaintworkComment5R = Modal.PaintworkComment5R;
            dbModal.TriphazardsComment = Modal.TriphazardsComment;
            dbModal.WindlassComment = Modal.WindlassComment;
            dbModal.CablesComment = Modal.CablesComment;
            dbModal.WinchesComment5R = Modal.WinchesComment5R;
            dbModal.FairleadsComment5R = Modal.FairleadsComment5R;
            dbModal.MooringComment = Modal.MooringComment;
            dbModal.HatchToforecastlespaceComment = Modal.HatchToforecastlespaceComment;
            dbModal.VentilatorsComment5R = Modal.VentilatorsComment5R;
            dbModal.BellComment = Modal.BellComment;
            dbModal.ForemastComment = Modal.ForemastComment;
            dbModal.FireComment = Modal.FireComment;
            dbModal.RailsComment = Modal.RailsComment;
            dbModal.AntislippaintComment5R = Modal.AntislippaintComment5R;
            dbModal.ForecastleComment = Modal.ForecastleComment;
            dbModal.CleanlinessComment5S = Modal.CleanlinessComment5S;
            dbModal.PaintworkComment5S = Modal.PaintworkComment5S;
            dbModal.ForepeakComment = Modal.ForepeakComment;
            dbModal.ChainlockerComment = Modal.ChainlockerComment;
            dbModal.LightingComment5S = Modal.LightingComment5S;
            dbModal.AccesssafetychainComment = Modal.AccesssafetychainComment;
            dbModal.EmergencyfirepumpComment = Modal.EmergencyfirepumpComment;
            dbModal.BowthrusterandroomComment = Modal.BowthrusterandroomComment;
            dbModal.SparemooringlinesComment = Modal.SparemooringlinesComment;
            dbModal.PaintlockerComment = Modal.PaintlockerComment;
            dbModal.ForecastleSpaceComment = Modal.ForecastleSpaceComment;

            dbModal.BoottopComment = Modal.BoottopComment;
            dbModal.TopsidesComment = Modal.TopsidesComment;
            dbModal.AntifoulingComment = Modal.AntifoulingComment;
            dbModal.DraftandplimsollComment = Modal.DraftandplimsollComment;
            dbModal.FoulingComment = Modal.FoulingComment;
            dbModal.MechanicalComment = Modal.MechanicalComment;
            dbModal.HullComment = Modal.HullComment;
            dbModal.SummaryComment = Modal.SummaryComment;
            dbModal.IsSynced = Modal.IsSynced;
            dbModal.SavedAsDraft = Modal.SavedAsDraft;

            dbModal.SnapBackZoneComment = Modal.SnapBackZoneComment;
            dbModal.ConditionGantryCranesComment = Modal.ConditionGantryCranesComment;
            dbModal.CylindersLockerComment = Modal.CylindersLockerComment;
            dbModal.MedicalLogBookComment = Modal.MedicalLogBookComment;
            dbModal.DrugsNarcoticsComment = Modal.DrugsNarcoticsComment;
            dbModal.DefibrillatorComment = Modal.DefibrillatorComment;
            dbModal.RPWaterHandbook = Modal.RPWaterHandbook;
            dbModal.BioRPWH = Modal.BioRPWH;
            dbModal.PRE = Modal.PRE;
            dbModal.NoiseVibrationFile = Modal.NoiseVibrationFile;
            dbModal.BioMPR = Modal.BioMPR;
            dbModal.AsbestosPlan = Modal.AsbestosPlan;
            dbModal.ShipPublicAddrComment = Modal.ShipPublicAddrComment;
            dbModal.BridgewindowswiperssprayComment = Modal.BridgewindowswiperssprayComment;
            dbModal.BridgewindowswipersComment = Modal.BridgewindowswipersComment;
            dbModal.DaylightSignalsComment = Modal.DaylightSignalsComment;
            dbModal.LiferaftDavitComment = Modal.LiferaftDavitComment;
            dbModal.SnapBackZone5NComment = Modal.SnapBackZone5NComment;
            dbModal.ADPPublicationsComment = Modal.ADPPublicationsComment;

            //RDBJ 10/20/2021
            dbModal.IsGeneralSectionComplete = Modal.IsGeneralSectionComplete;
            dbModal.IsManningSectionComplete = Modal.IsManningSectionComplete;
            dbModal.IsShipCertificationSectionComplete = Modal.IsShipCertificationSectionComplete;
            dbModal.IsRecordKeepingSectionComplete = Modal.IsRecordKeepingSectionComplete;
            dbModal.IsSafetyEquipmentSectionComplete = Modal.IsSafetyEquipmentSectionComplete;
            dbModal.IsSecuritySectionComplete = Modal.IsSecuritySectionComplete;
            dbModal.IsBridgeSectionComplete = Modal.IsBridgeSectionComplete;
            dbModal.IsMedicalSectionComplete = Modal.IsMedicalSectionComplete;
            dbModal.IsGalleySectionComplete = Modal.IsGalleySectionComplete;
            dbModal.IsEngineRoomSectionComplete = Modal.IsEngineRoomSectionComplete;
            dbModal.IsSuperstructureSectionComplete = Modal.IsSuperstructureSectionComplete;
            dbModal.IsDeckSectionComplete = Modal.IsDeckSectionComplete;
            dbModal.IsHoldsAndCoverSectionComplete = Modal.IsHoldsAndCoverSectionComplete;
            dbModal.IsForeCastleSectionComplete = Modal.IsForeCastleSectionComplete;
            dbModal.IsHullSectionComplete = Modal.IsHullSectionComplete;
            dbModal.IsSummarySectionComplete = Modal.IsSummarySectionComplete;
            dbModal.IsDeficienciesSectionComplete = Modal.IsDeficienciesSectionComplete;
            dbModal.IsPhotographsSectionComplete = Modal.IsPhotographsSectionComplete;
            //End RDBJ 10/20/2021
        }

        //RDBJ 09/22/2021
        public List<GeneralInspectionReport> getSynchGIRList(
             string strShipCode  // JSL 11/12/2022
            )
        {
            List<GeneralInspectionReport> SyncList = new List<GeneralInspectionReport>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                SyncList = dbContext.GeneralInspectionReports
                    .Where(x => x.UniqueFormID != null) // RDBJ 12/17/2021
                    .Select(y => new GeneralInspectionReport()
                {
                    UniqueFormID = y.UniqueFormID,
                    FormVersion = y.FormVersion,
                    Ship = y.Ship,
                    IsSynced = y.IsSynced,
                }).ToList();

                // JSL 11/12/2022
                if (!string.IsNullOrEmpty(strShipCode))
                {
                    SyncList = SyncList.Where(x => x.Ship == strShipCode).ToList();
                }
                // End JSL 11/12/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchGIRList " + ex.Message + "\n" + ex.InnerException);
                SyncList = null;
            }
            return SyncList;
        }
        //End RDBJ 09/22/2021

        //RDBJ 09/22/2021
        public GeneralInspectionReport getSynchGIR(string UniqueFormID)
        {
            GeneralInspectionReport dbModal = new GeneralInspectionReport();
            Guid UFormId = Guid.Parse(UniqueFormID);
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var girData = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == UFormId).FirstOrDefault();
                if (girData != null)
                {
                    dbModal.GIRDeficiencies = new List<GIRDeficiencies>();
                    dbModal.GIRSafeManningRequirements = new List<GlRSafeManningRequirements>();
                    dbModal.GIRCrewDocuments = new List<GlRCrewDocuments>();
                    dbModal.GIRRestandWorkHours = new List<GIRRestandWorkHours>();
                    dbModal.GIRPhotographs = new List<GIRPhotographs>();

                    GetGIRFormData(girData, ref dbModal);

                    // RDBJ 01/05/2022 wrapped in if
                    if (dbModal.isDelete == 0)
                    {
                        var defList = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == girData.UniqueFormID).ToList();
                        if (defList != null && defList.Count > 0)
                        {
                            foreach (var def in defList)
                            {
                                GIRDeficiencies girDef = new GIRDeficiencies();
                                girDef.GIRDeficienciesComments = new List<DeficienciesNote>();
                                girDef.GIRDeficienciesInitialActions = new List<GIRDeficienciesInitialActions>();
                                girDef.GIRDeficienciesResolution = new List<Modals.GIRDeficienciesResolution>();

                                girDef.No = def.No;
                                girDef.DateRaised = def.DateRaised;
                                girDef.Deficiency = def.Deficiency;
                                girDef.DateClosed = def.DateClosed;
                                girDef.CreatedDate = def.CreatedDate;
                                girDef.UpdatedDate = def.UpdatedDate;
                                girDef.Ship = def.Ship;
                                girDef.IsClose = def.IsClose;
                                girDef.ReportType = def.ReportType;
                                girDef.ItemNo = def.ItemNo;
                                girDef.Section = def.Section;
                                girDef.UniqueFormID = def.UniqueFormID;
                                girDef.isDelete = def.isDelete;
                                girDef.DeficienciesUniqueID = def.DeficienciesUniqueID;
                                girDef.Priority = def.Priority == null ? 12 : def.Priority; //RDBJ 11/01/2021
                                girDef.AssignTo = def.AssignTo; // RDBJ 12/18/2021
                                girDef.DueDate = def.DueDate;   // RDBJ 03/01/2022

                                // RDBJ 12/23/2021 wrapped in if
                                if (def.isDelete == 0)
                                {
                                    var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == girDef.DeficienciesUniqueID).ToList();
                                    if (defFiles != null && defFiles.Count > 0)
                                    {
                                        foreach (var girDefFile in defFiles)
                                        {
                                            Modals.GIRDeficienciesFile defFile = new Modals.GIRDeficienciesFile();
                                            defFile.DeficienciesFileUniqueID = girDefFile.DeficienciesUniqueID; // JSL 06/07/2022
                                            defFile.DeficienciesUniqueID = girDefFile.DeficienciesUniqueID;
                                            defFile.DeficienciesID = girDefFile.DeficienciesID != null ? girDefFile.DeficienciesID : 0; // RDBJ 01/15/2022 set avoid null error
                                            defFile.FileName = girDefFile.FileName;
                                            defFile.StorePath = girDefFile.StorePath;

                                            // JSL 12/04/2022
                                            if (!defFile.StorePath.StartsWith("data:"))
                                            {
                                                defFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defFile.StorePath);
                                            }
                                            // End JSL 12/04/2022

                                            girDef.GIRDeficienciesFile.Add(defFile);
                                        }

                                    }

                                    var defComments = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                    if (defComments != null && defComments.Count > 0)
                                    {
                                        foreach (var defComment in defComments)
                                        {
                                            DeficienciesNote defNote = new DeficienciesNote();
                                            defNote.DeficienciesUniqueID = defComment.DeficienciesUniqueID;
                                            defNote.DeficienciesID = defComment.DeficienciesID;
                                            defNote.UserName = defComment.UserName;
                                            defNote.Comment = defComment.Comment;
                                            defNote.CreatedDate = defComment.CreatedDate;
                                            defNote.ModifyDate = defComment.ModifyDate;
                                            defNote.NoteUniqueID = defComment.NoteUniqueID;

                                            var defCommentFiles = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteUniqueID == defNote.NoteUniqueID).ToList();
                                            if (defCommentFiles != null && defCommentFiles.Count > 0)
                                            {
                                                foreach (var defCommmentFile in defCommentFiles)
                                                {
                                                    Modals.GIRDeficienciesCommentFile defComFile = new Modals.GIRDeficienciesCommentFile();
                                                    defComFile.DeficienciesID = defCommmentFile.DeficienciesID;
                                                    defComFile.FileName = defCommmentFile.FileName;
                                                    defComFile.StorePath = defCommmentFile.StorePath;
                                                    defComFile.IsUpload = defCommmentFile.IsUpload;
                                                    defComFile.NoteUniqueID = defCommmentFile.NoteUniqueID;
                                                    defComFile.CommentFileUniqueID = defCommmentFile.CommentFileUniqueID;
                                                    // JSL 12/04/2022
                                                    if (!defComFile.StorePath.StartsWith("data:"))
                                                    {
                                                        defComFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defComFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    defNote.GIRDeficienciesCommentFile.Add(defComFile);
                                                }
                                            }
                                            girDef.GIRDeficienciesComments.Add(defNote);
                                        }
                                    }

                                    var defIntActs = dbContext.GIRDeficienciesInitialActions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                    if (defIntActs != null && defIntActs.Count > 0)
                                    {
                                        foreach (var defIntAct in defIntActs)
                                        {
                                            Modals.GIRDeficienciesInitialActions defIntAction = new Modals.GIRDeficienciesInitialActions();
                                            defIntAction.DeficienciesUniqueID = defIntAct.DeficienciesUniqueID;
                                            defIntAction.DeficienciesID = defIntAct.DeficienciesID;
                                            defIntAction.Name = defIntAct.Name;
                                            defIntAction.Description = defIntAct.Description;
                                            defIntAction.CreatedDate = defIntAct.CreatedDate;
                                            defIntAction.IniActUniqueID = defIntAct.IniActUniqueID;

                                            var defIntActFiles = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActUniqueID == defIntAct.IniActUniqueID).ToList();
                                            if (defIntActFiles != null && defIntActFiles.Count > 0)
                                            {
                                                foreach (var defintActFile in defIntActFiles)
                                                {
                                                    Modals.GIRDeficienciesInitialActionsFile defIntActionFile = new Modals.GIRDeficienciesInitialActionsFile();
                                                    defIntActionFile.DeficienciesID = defintActFile.DeficienciesID;
                                                    defIntActionFile.FileName = defintActFile.FileName;
                                                    defIntActionFile.StorePath = defintActFile.StorePath;
                                                    defIntActionFile.IsUpload = defintActFile.IsUpload;
                                                    defIntActionFile.IniActUniqueID = defintActFile.IniActUniqueID;
                                                    defIntActionFile.IniActFileUniqueID = defintActFile.IniActFileUniqueID;

                                                    // JSL 12/04/2022
                                                    if (!defIntActionFile.StorePath.StartsWith("data:"))
                                                    {
                                                        defIntActionFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defIntActionFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    defIntAction.GIRDeficienciesInitialActionsFiles.Add(defIntActionFile);
                                                }
                                            }
                                            girDef.GIRDeficienciesInitialActions.Add(defIntAction);
                                        }
                                    }

                                    var defResolutions = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesUniqueID == def.DeficienciesUniqueID).ToList();
                                    if (defResolutions != null && defResolutions.Count > 0)
                                    {
                                        foreach (var defResolution in defResolutions)
                                        {
                                            Modals.GIRDeficienciesResolution defRes = new Modals.GIRDeficienciesResolution();
                                            defRes.DeficienciesUniqueID = defResolution.DeficienciesUniqueID;
                                            defRes.DeficienciesID = defResolution.DeficienciesID;
                                            defRes.Name = defResolution.Name;
                                            defRes.Resolution = defResolution.Resolution;
                                            defRes.CreatedDate = defResolution.CreatedDate;
                                            defRes.ResolutionUniqueID = defResolution.ResolutionUniqueID;

                                            var defResolutionFiles = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == defResolution.ResolutionUniqueID).ToList();
                                            if (defResolutionFiles != null && defResolutionFiles.Count > 0)
                                            {
                                                foreach (var defResoFile in defResolutionFiles)
                                                {
                                                    Modals.GIRDeficienciesResolutionFile defResolutionFile = new Modals.GIRDeficienciesResolutionFile();
                                                    defResolutionFile.DeficienciesID = defResoFile.DeficienciesID;
                                                    defResolutionFile.FileName = defResoFile.FileName;
                                                    defResolutionFile.StorePath = defResoFile.StorePath;
                                                    defResolutionFile.IsUpload = defResoFile.IsUpload;
                                                    defResolutionFile.ResolutionUniqueID = defResoFile.ResolutionUniqueID;
                                                    defResolutionFile.ResolutionFileUniqueID = defResoFile.ResolutionFileUniqueID;

                                                    // JSL 12/04/2022
                                                    if (!defResolutionFile.StorePath.StartsWith("data:"))
                                                    {
                                                        defResolutionFile.StorePath = Utility.ConvertIntoBase64EndCodedUploadedFile(defResolutionFile.StorePath);
                                                    }
                                                    // End JSL 12/04/2022

                                                    defRes.GIRDeficienciesResolutionFiles.Add(defResolutionFile);
                                                }
                                            }
                                            girDef.GIRDeficienciesResolution.Add(defRes);
                                        }
                                    }
                                }

                                dbModal.GIRDeficiencies.Add(girDef);
                            }
                        }

                        var safeManning = dbContext.GlRSafeManningRequirements.Where(x => x.UniqueFormID == girData.UniqueFormID).ToList();
                        if (safeManning != null && safeManning.Count > 0)
                        {
                            foreach (var safMan in safeManning)
                            {
                                GlRSafeManningRequirements mSafMan = new GlRSafeManningRequirements();
                                mSafMan.GIRFormID = 0;
                                mSafMan.UniqueFormID = safMan.UniqueFormID;
                                mSafMan.Ship = safMan.Ship;
                                mSafMan.Rank = safMan.Rank;
                                mSafMan.RequiredbySMD = safMan.RequiredbySMD;
                                mSafMan.OnBoard = safMan.OnBoard;
                                mSafMan.CreatedDate = safMan.CreatedDate;
                                mSafMan.UpdatedDate = safMan.UpdatedDate;

                                dbModal.GIRSafeManningRequirements.Add(mSafMan);
                            }
                        }

                        var creDocs = dbContext.GlRCrewDocuments.Where(x => x.UniqueFormID == girData.UniqueFormID).ToList();
                        if (creDocs != null && creDocs.Count > 0)
                        {
                            foreach (var creDoc in creDocs)
                            {
                                GlRCrewDocuments mCreDoc = new GlRCrewDocuments();
                                mCreDoc.GIRFormID = 0;
                                mCreDoc.UniqueFormID = creDoc.UniqueFormID;
                                mCreDoc.Ship = creDoc.Ship;
                                mCreDoc.CrewmemberName = creDoc.CrewmemberName;
                                mCreDoc.CertificationDetail = creDoc.CertificationDetail;
                                mCreDoc.CreatedDate = creDoc.CreatedDate;
                                mCreDoc.UpdatedDate = creDoc.UpdatedDate;

                                dbModal.GIRCrewDocuments.Add(mCreDoc);
                            }
                        }

                        var restWNH = dbContext.GIRRestandWorkHours.Where(x => x.UniqueFormID == girData.UniqueFormID).ToList();
                        if (restWNH != null && restWNH.Count > 0)
                        {
                            foreach (var restItem in restWNH)
                            {
                                GIRRestandWorkHours workHours = new GIRRestandWorkHours();
                                workHours.GIRFormID = 0;
                                workHours.UniqueFormID = restItem.UniqueFormID;
                                workHours.Ship = restItem.Ship;
                                workHours.CrewmemberName = restItem.CrewmemberName;
                                workHours.RestAndWorkDetail = restItem.RestAndWorkDetail;
                                workHours.CreatedDate = restItem.CreatedDate;
                                workHours.UpdatedDate = restItem.UpdatedDate;

                                dbModal.GIRRestandWorkHours.Add(workHours);
                            }
                        }

                        var girPhotos = dbContext.GIRPhotographs.Where(x => x.UniqueFormID == girData.UniqueFormID).ToList();
                        if (girPhotos != null && girPhotos.Count > 0)
                        {
                            foreach (var gitPho in girPhotos)
                            {
                                GIRPhotographs photo = new GIRPhotographs();
                                photo.GIRFormID = 0;
                                photo.UniqueFormID = gitPho.UniqueFormID;
                                photo.FileName = gitPho.FileName;
                                photo.ImagePath = gitPho.ImagePath;
                                photo.ImageCaption = gitPho.ImageCaption;
                                photo.Ship = gitPho.Ship;
                                photo.CreatedDate = gitPho.CreatedDate;
                                photo.UpdatedDate = gitPho.UpdatedDate;

                                // JSL 12/04/2022
                                if (!photo.ImagePath.StartsWith("data:"))
                                {
                                    photo.ImagePath = Utility.ConvertIntoBase64EndCodedUploadedFile(photo.ImagePath);
                                }
                                // End JSL 12/04/2022

                                dbModal.GIRPhotographs.Add(photo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchGIR Full " + UniqueFormID + " \n" + ex.ToString());
                LogHelper.writelog("Cloud getSynchGIR " + ex.Message + "\n" + ex.InnerException);
                dbModal = null;
            }
            return dbModal;
        }
        //End RDBJ 09/22/2021

        #region // RDBJ 12/22/2021 later use
        // RDBJ 12/22/2021
        public List<GIRDeficiencies> GetGIRDeficiencies(string UniqueFormID)
        {
            List<GIRDeficiencies> dbGIRDefModal = new List<GIRDeficiencies>();
            Guid UFormId = Guid.Parse(UniqueFormID);
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var defList = dbContext.GIRDeficiencies.Where(x => x.UniqueFormID == UFormId && x.isDelete == 0).ToList();
                if (defList != null && defList.Count > 0)
                {
                    foreach (var def in defList)
                    {
                        GIRDeficiencies girDef = new GIRDeficiencies();

                        girDef.No = def.No;
                        girDef.DateRaised = def.DateRaised;
                        girDef.Deficiency = def.Deficiency;
                        girDef.DateClosed = def.DateClosed;
                        girDef.CreatedDate = def.CreatedDate;
                        girDef.UpdatedDate = def.UpdatedDate;
                        girDef.Ship = def.Ship;
                        girDef.IsClose = def.IsClose;
                        girDef.ReportType = def.ReportType;
                        girDef.ItemNo = def.ItemNo;
                        girDef.Section = def.Section;
                        girDef.UniqueFormID = def.UniqueFormID;
                        girDef.isDelete = def.isDelete;
                        girDef.DeficienciesUniqueID = def.DeficienciesUniqueID;
                        girDef.Priority = def.Priority == null ? 12 : def.Priority; //RDBJ 11/01/2021
                        girDef.AssignTo = def.AssignTo; // RDBJ 12/18/2021

                        var defFiles = dbContext.GIRDeficienciesFiles.Where(x => x.DeficienciesUniqueID == girDef.DeficienciesUniqueID).ToList();
                        if (defFiles != null && defFiles.Count > 0)
                        {
                            foreach (var girDefFile in defFiles)
                            {
                                Modals.GIRDeficienciesFile defFile = new Modals.GIRDeficienciesFile();
                                defFile.DeficienciesUniqueID = girDefFile.DeficienciesUniqueID;
                                defFile.DeficienciesID = girDefFile.DeficienciesID;
                                defFile.FileName = girDefFile.FileName;
                                defFile.StorePath = girDefFile.StorePath;
                                girDef.GIRDeficienciesFile.Add(defFile);
                            }
                        }

                        dbGIRDefModal.Add(girDef);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GetGIRDeficiencies " + ex.Message + "\n" + ex.InnerException);
                dbGIRDefModal = null;
            }
            return dbGIRDefModal;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        public List<DeficienciesNote> GetGIRDeficienciesComments(string DeficienciesUniqueID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<DeficienciesNote> dbGIRDefCommentsModal = new List<DeficienciesNote>();
            Guid DefUId = Guid.Parse(DeficienciesUniqueID);
            try
            {
                var defComments = dbContext.GIRDeficienciesNotes.Where(x => x.DeficienciesUniqueID == DefUId).ToList();
                if (defComments != null && defComments.Count > 0)
                {
                    foreach (var defComment in defComments)
                    {
                        DeficienciesNote defNote = new DeficienciesNote();
                        defNote.DeficienciesUniqueID = defComment.DeficienciesUniqueID;
                        defNote.DeficienciesID = defComment.DeficienciesID;
                        defNote.UserName = defComment.UserName;
                        defNote.Comment = defComment.Comment;
                        defNote.CreatedDate = defComment.CreatedDate;
                        defNote.ModifyDate = defComment.ModifyDate;
                        defNote.NoteUniqueID = defComment.NoteUniqueID;

                        var defCommentFiles = dbContext.GIRDeficienciesCommentFiles.Where(x => x.NoteUniqueID == defNote.NoteUniqueID).ToList();
                        if (defCommentFiles != null && defCommentFiles.Count > 0)
                        {
                            foreach (var defCommmentFile in defCommentFiles)
                            {
                                Modals.GIRDeficienciesCommentFile defComFile = new Modals.GIRDeficienciesCommentFile();
                                defComFile.DeficienciesID = defCommmentFile.DeficienciesID;
                                defComFile.FileName = defCommmentFile.FileName;
                                defComFile.StorePath = defCommmentFile.StorePath;
                                defComFile.IsUpload = defCommmentFile.IsUpload;
                                defComFile.NoteUniqueID = defCommmentFile.NoteUniqueID;
                                defComFile.CommentFileUniqueID = defCommmentFile.CommentFileUniqueID;

                                defNote.GIRDeficienciesCommentFile.Add(defComFile);
                            }
                        }
                        dbGIRDefCommentsModal.Add(defNote);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GetGIRDeficienciesComments " + ex.Message + "\n" + ex.InnerException);
                dbGIRDefCommentsModal = null;
            }
            return dbGIRDefCommentsModal;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        public List<Modals.GIRDeficienciesInitialActions> GetGIRDeficienciesInitialActions(string DeficienciesUniqueID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Modals.GIRDeficienciesInitialActions> dbGIRDefInitialActionModal = new List<Modals.GIRDeficienciesInitialActions>();
            Guid DefUId = Guid.Parse(DeficienciesUniqueID);
            try
            {
                var defIntActs = dbContext.GIRDeficienciesInitialActions.Where(x => x.DeficienciesUniqueID == DefUId).ToList();
                if (defIntActs != null && defIntActs.Count > 0)
                {
                    foreach (var defIntAct in defIntActs)
                    {
                        Modals.GIRDeficienciesInitialActions defIntAction = new Modals.GIRDeficienciesInitialActions();
                        defIntAction.DeficienciesUniqueID = defIntAct.DeficienciesUniqueID;
                        defIntAction.DeficienciesID = defIntAct.DeficienciesID;
                        defIntAction.Name = defIntAct.Name;
                        defIntAction.Description = defIntAct.Description;
                        defIntAction.CreatedDate = defIntAct.CreatedDate;
                        defIntAction.IniActUniqueID = defIntAct.IniActUniqueID;

                        var defIntActFiles = dbContext.GIRDeficienciesInitialActionsFiles.Where(x => x.IniActUniqueID == defIntAct.IniActUniqueID).ToList();
                        if (defIntActFiles != null && defIntActFiles.Count > 0)
                        {
                            foreach (var defintActFile in defIntActFiles)
                            {
                                Modals.GIRDeficienciesInitialActionsFile defIntActionFile = new Modals.GIRDeficienciesInitialActionsFile();
                                defIntActionFile.DeficienciesID = defintActFile.DeficienciesID;
                                defIntActionFile.FileName = defintActFile.FileName;
                                defIntActionFile.StorePath = defintActFile.StorePath;
                                defIntActionFile.IsUpload = defintActFile.IsUpload;
                                defIntActionFile.IniActUniqueID = defintActFile.IniActUniqueID;
                                defIntActionFile.IniActFileUniqueID = defintActFile.IniActFileUniqueID;

                                defIntAction.GIRDeficienciesInitialActionsFiles.Add(defIntActionFile);
                            }
                        }
                        dbGIRDefInitialActionModal.Add(defIntAction);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GetGIRDeficienciesInitialActions " + ex.Message + "\n" + ex.InnerException);
                dbGIRDefInitialActionModal = null;
            }
            return dbGIRDefInitialActionModal;
        }
        // End RDBJ 12/22/2021

        // RDBJ 12/22/2021
        public List<Modals.GIRDeficienciesResolution> GetGIRDeficienciesResolutions(string DeficienciesUniqueID)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Modals.GIRDeficienciesResolution> dbGIRDefResolutionModal = new List<Modals.GIRDeficienciesResolution>();
            Guid DefUId = Guid.Parse(DeficienciesUniqueID);
            try
            {
                var defResolutions = dbContext.GIRDeficienciesResolutions.Where(x => x.DeficienciesUniqueID == DefUId).ToList();
                if (defResolutions != null && defResolutions.Count > 0)
                {
                    foreach (var defResolution in defResolutions)
                    {
                        Modals.GIRDeficienciesResolution defRes = new Modals.GIRDeficienciesResolution();
                        defRes.DeficienciesUniqueID = defResolution.DeficienciesUniqueID;
                        defRes.DeficienciesID = defResolution.DeficienciesID;
                        defRes.Name = defResolution.Name;
                        defRes.Resolution = defResolution.Resolution;
                        defRes.CreatedDate = defResolution.CreatedDate;
                        defRes.ResolutionUniqueID = defResolution.ResolutionUniqueID;

                        var defResolutionFiles = dbContext.GIRDeficienciesResolutionFiles.Where(x => x.ResolutionUniqueID == defResolution.ResolutionUniqueID).ToList();
                        if (defResolutionFiles != null && defResolutionFiles.Count > 0)
                        {
                            foreach (var defResoFile in defResolutionFiles)
                            {
                                Modals.GIRDeficienciesResolutionFile defResolutionFile = new Modals.GIRDeficienciesResolutionFile();
                                defResolutionFile.DeficienciesID = defResoFile.DeficienciesID;
                                defResolutionFile.FileName = defResoFile.FileName;
                                defResolutionFile.StorePath = defResoFile.StorePath;
                                defResolutionFile.IsUpload = defResoFile.IsUpload;
                                defResolutionFile.ResolutionUniqueID = defResoFile.ResolutionUniqueID;
                                defResolutionFile.ResolutionFileUniqueID = defResoFile.ResolutionFileUniqueID;

                                defRes.GIRDeficienciesResolutionFiles.Add(defResolutionFile);
                            }
                        }
                        dbGIRDefResolutionModal.Add(defRes);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GetGIRDeficienciesResolutions " + ex.Message + "\n" + ex.InnerException);
                dbGIRDefResolutionModal = null;
            }
            return dbGIRDefResolutionModal;
        }
        // End RDBJ 12/22/2021
        #endregion

        // JSL 07/16/2022
        public void SendSignalRNotificationCallForTheOffice(string shipCode = "", bool blnSendNotificationToUserForForm = false)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            var lstUsers = dbContext.UserProfiles.Where(x => x.UserGroup == 1 || x.UserGroup == 5).ToList();
            if (lstUsers != null && lstUsers.Count > 0)
            {
                foreach (var itemUser in lstUsers)
                {
                    NotificationsHelper.SendNotificationsForSignalR(Convert.ToString(itemUser.UserID), itemUser.Email, blnSendNotificationToUserForForm);
                }
            }
        }
        // End JSL 07/16/2022

        // JSL 07/16/2022
        public Dictionary<string, string> PerformPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                case AppStatic.API_METHOD_InsertOrUpdateDeficienciesData:
                    {
                        string strFormType = string.Empty;
                        string strFormUniqueID = string.Empty;
                        string strShipCode = string.Empty;
                        string strDeficiencyData = string.Empty;

                        Guid guidFormUniqueID = Guid.Empty;
                        bool IsNeedToSendNotification = false;
                        List<Modals.GIRDeficiencies> modalDeficiencies = new List<GIRDeficiencies>();
                        try
                        {
                            if (dictMetaData.ContainsKey("FormUniqueID"))
                                strFormUniqueID = dictMetaData["FormUniqueID"];

                            if (dictMetaData.ContainsKey("ShipCode"))
                                strShipCode = dictMetaData["ShipCode"];

                            if (dictMetaData.ContainsKey("DeficienciesData"))
                                strDeficiencyData = dictMetaData["DeficienciesData"];

                            if (!string.IsNullOrEmpty(strDeficiencyData))
                            {
                                Modals.GIRDeficiencies modalDeficiency = new GIRDeficiencies();
                                modalDeficiency = JsonConvert.DeserializeObject<GIRDeficiencies>(strDeficiencyData);
                                if (modalDeficiency != null)
                                {
                                    modalDeficiencies.Add(modalDeficiency);
                                }
                            }

                            GIRDeficiencies_Save(strFormUniqueID, modalDeficiencies
                                , ref IsNeedToSendNotification
                                );

                            if (IsNeedToSendNotification)
                            {
                                SendSignalRNotificationCallForTheOffice();
                            }

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(strFormType + " - " + strFormUniqueID + " : " + AppStatic.API_METHOD_InsertOrUpdateDeficienciesData + " Error : " + ex.Message);
                        }
                        break;
                    }
                // JSL 01/03/2023
                case AppStatic.API_METHOD_DeleteGIRPhotographsData:
                    {
                        string strFormType = AppStatic.GIRForm;
                        string strFormUniqueID = string.Empty;
                        
                        List<Modals.GIRPhotographs> modalPhotographs = new List<GIRPhotographs>();
                        try
                        {
                            if (dictMetaData.ContainsKey("FormUniqueID"))
                                strFormUniqueID = dictMetaData["FormUniqueID"];

                            GIRPhotos_Save(strFormUniqueID, modalPhotographs, true);
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(strFormType + " - " + strFormUniqueID + " : " + AppStatic.API_METHOD_DeleteGIRPhotographsData + " Error : " + ex.ToString());
                        }
                        break;
                    }
                // End JSL 01/03/2023
                // JSL 01/03/2023
                case AppStatic.API_METHOD_InsertOrUpdateGIRPhotographsData:
                    {
                        string strFormType = string.Empty;
                        string strFormUniqueID = string.Empty;
                        string strShipCode = string.Empty;
                        string strGIRPhotographData = string.Empty;

                        Guid guidFormUniqueID = Guid.Empty;
                        List<Modals.GIRPhotographs> modalPhotographs = new List<GIRPhotographs>();
                        try
                        {
                            if (dictMetaData.ContainsKey("FormUniqueID"))
                                strFormUniqueID = dictMetaData["FormUniqueID"];

                            if (dictMetaData.ContainsKey("ShipCode"))
                                strShipCode = dictMetaData["ShipCode"];

                            if (dictMetaData.ContainsKey("PhotographData"))
                                strGIRPhotographData = dictMetaData["PhotographData"];

                            if (!string.IsNullOrEmpty(strGIRPhotographData))
                            {
                                Modals.GIRPhotographs modalPhoto = new GIRPhotographs();
                                modalPhoto = JsonConvert.DeserializeObject<GIRPhotographs>(strGIRPhotographData);

                                if (modalPhoto != null 
                                    && !string.IsNullOrEmpty(modalPhoto.ImagePath)  // JSL 01/25/2023
                                    )
                                {
                                    modalPhotographs.Add(modalPhoto);
                                }
                            }

                            GIRPhotos_Save(strFormUniqueID, modalPhotographs);

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(strFormType + " - " + strFormUniqueID + " : " + AppStatic.API_METHOD_InsertOrUpdateGIRPhotographsData + " Error : " + ex.ToString());
                        }
                        break;
                    }
                // End JSL 01/03/2023
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
        // End JSL 07/16/2022
    }
}
