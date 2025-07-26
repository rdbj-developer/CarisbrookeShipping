using CarisbrookeShippingService.BLL.Helpers;
using CarisbrookeShippingService.Helpers;
using System.ServiceProcess;

namespace CarisbrookeShippingService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);


            // RDBJ 12/01/2021 From here to below all for testing

            //Service1 serv = new Service1();
            //serv.StartSync();

            //BLL.Helpers.Utility.CreateAllTable(); // RDBJ 12/30/2021
            //BLL.Helpers.Utility.DatabaseModification(); //RDBJ 10/02/2021
            //BLL.Helpers.Utility.GetUserProfileDataAndSaveInSMTPServerConfigFile(); //RDBJ 11/08/2021
            //BLL.Helpers.Utility.GetMainSyncServiceDataAndSaveInMainSyncServiceFile(); // RDBJ 02/24/2022

            //ShipAppUpdateHelper _ShipAppUpdatehelper = new ShipAppUpdateHelper();
            //_ShipAppUpdatehelper.IsFirstTimeCall = true;
            //_ShipAppUpdatehelper.StartShipAppUpdateSync();

            //DocumentsHelper _DocHelper = new DocumentsHelper();
            //////_DocHelper.IsFirstTimeCall = IsFirstTimeCall;
            //_DocHelper.StartDocSync();

            //GIRFormDataHelper _gHelper = new GIRFormDataHelper();
            //_gHelper.StartGIRSync();
            //_gHelper.StartGIRSyncCloudTOLocal();
            //_gHelper.GETGIRLatestData();


            //SIRFormDataHelper _sirHelper = new SIRFormDataHelper();
            //_sirHelper.StartSIRSync();
            //_sirHelper.StartSIRSyncCloudTOLocal();
            //_sirHelper.GETSIRLatestData();

            //IAFHelper _iafHelper = new IAFHelper();
            //_iafHelper.StartIAFSync();
            //_iafHelper.StartIAFSyncCloudTOLocal();
            //_iafHelper.GETIAFLatestData();
            //_iafHelper.StartSMSReferencesSyncAWSServerToLocal();
            //_iafHelper.StartSSPReferencesSyncAWSServerToLocal();
            //_iafHelper.StartMLCReferencesSyncAWSServerToLocal();

            //ShipAppUpdateHelper _ShipAppUpdatehelper = new ShipAppUpdateHelper();
            //_ShipAppUpdatehelper.StartShipAppUpdateSync();

            // RDBJ 01/01/2022
            //HelpAndSupportHelper _helpAndSupportHelper = new HelpAndSupportHelper();
            //_helpAndSupportHelper.StartHelpAndSupportSyncLocalToAWSServer();
            //_helpAndSupportHelper.StartHelpAndSupportSyncAWSServerToLocal();
            // End RDBJ 01/01/2022
        }
    }
}
