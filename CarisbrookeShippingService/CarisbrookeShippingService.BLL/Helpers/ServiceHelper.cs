using System;

namespace CarisbrookeShippingService.BLL.Helpers
{
    public class ServiceHelper
    {
        public bool IsFirstTimeCall { get; set; }
        public bool IsInspectorInThisMachine { get; set; }   // JSL 11/12/2022

        public void StartSync()
        {
            try
            {
                if (IsFirstTimeCall)
                {
                    try
                    {
                        Utility.CreateAllTable();
                        LogHelper.writelog("Table creation completed");
                        Utility.DatabaseModification(); //RDBJ 10/02/2021
                        LogHelper.writelog("Database Modification completed"); //RDBJ 10/02/2021

                        //RDBJ 11/08/2021
                        //Utility.GetUserProfileDataAndSaveInSMTPServerConfigFile();
                        //LogHelper.writelog("Added Emails into SMTPServerConfig File"); //RDBJ 10/02/2021
                        //End RDBJ 11/08/2021
                    }
                    catch (Exception)
                    {}
                }

                // JSL 11/12/2022 
                SimpleObject objShipValue = Utility.GetShipValue();
                if (!IsInspectorInThisMachine)
                {
                    LogHelper.writelog("Delete Data Process started.");
                    Utility.DeleteDataFromDatabaseExceptCurrentShip(objShipValue.id);
                    LogHelper.writelog("Delete Data Process completed.");
                }
                // End JSL 11/12/2022

                SMRFormDataHelper _SMRhelper = new SMRFormDataHelper();
                _SMRhelper.StartSMRSync();

                DocumentsHelper _DocHelper = new DocumentsHelper();
                _DocHelper.IsFirstTimeCall = IsFirstTimeCall;
                _DocHelper.StartDocSync();

                GIRFormDataHelper _gHelper = new GIRFormDataHelper();
                _gHelper.StartGIRSync(); //RDBJ 10/02/2021 This is for LocalDB to AWSServer DB
                _gHelper.StartGIRSyncCloudTOLocal(
                    objShipValue.id, IsInspectorInThisMachine   // JSL 11/12/2022
                    ); //RDBJ 10/02/2021 This is for AWSServer DB to LocalDB 
                _gHelper.GETGIRLatestData(
                    objShipValue.id, IsInspectorInThisMachine   // JSL 11/12/2022
                    ); //RDBJ 10/02/2021 This is for Get Data into the Ship

                SIRFormDataHelper _sHelper = new SIRFormDataHelper();
                _sHelper.StartSIRSync(); //RDBJ 10/02/2021 This is for LocalDB to AWSServer DB
                _sHelper.StartSIRSyncCloudTOLocal(
                    objShipValue.id, IsInspectorInThisMachine   // JSL 11/12/2022
                    ); //RDBJ 10/02/2021 This is for AWSServer DB to LocalDB 
                _sHelper.GETSIRLatestData(
                    objShipValue.id, IsInspectorInThisMachine   // JSL 11/12/2022
                    ); //RDBJ 10/02/2021 This is for Get Data into the Ship

                IAFHelper _IAFhelper = new IAFHelper();
                _IAFhelper.StartIAFSync(); //RDBJ 10/02/2021 This is for LocalDB to AWSServer DB
                _IAFhelper.StartIAFSyncCloudTOLocal(
                    objShipValue.id, IsInspectorInThisMachine   // JSL 11/12/2022
                    ); //RDBJ 10/02/2021 This is for AWSServer DB to LocalDB 
                _IAFhelper.GETIAFLatestData(
                    objShipValue.id, IsInspectorInThisMachine   // JSL 11/12/2022
                    ); //RDBJ 10/02/2021 This is for Get Data into the Ship

                // JSL 05/20/2022
                _IAFhelper.StartSMSReferencesSyncAWSServerToLocal();
                _IAFhelper.StartSSPReferencesSyncAWSServerToLocal();
                _IAFhelper.StartMLCReferencesSyncAWSServerToLocal();
                // End JSL 05/20/2022

                AWSUsersHelper _AWSHelper = new AWSUsersHelper();
                _AWSHelper.SyncAwsUsers();

                ARFormDataHelper _ARHelper = new ARFormDataHelper();
                _ARHelper.StartARSync();

                DRFormDataHelper _DRHelper = new DRFormDataHelper();
                _DRHelper.StartDRSync();

                DCRFormDataHelper _DCRHelper = new DCRFormDataHelper();
                _DCRHelper.StartDCRSync();

                DPRFormDataHelper _DPRHelper = new DPRFormDataHelper();
                _DPRHelper.StartDPRSync();

                FormsHelper _FormHelper = new FormsHelper();
                _FormHelper.StartFormsync();

                HVRFormDataHelper _HVRFormDataHelper = new HVRFormDataHelper();
                _HVRFormDataHelper.StartHVRSync();

                RiskAssessmentFormHelper _RiskAssessmentFormDataHelper = new RiskAssessmentFormHelper();
                _RiskAssessmentFormDataHelper.StartRAFSync();
                _RiskAssessmentFormDataHelper.StartRAFSyncCloudTOLocal(objShipValue.id, IsInspectorInThisMachine);   // JSL 11/26/2022
                _RiskAssessmentFormDataHelper.GETRAFLatestData(objShipValue.id, IsInspectorInThisMachine);   // JSL 11/26/2022

                FeedbackFormDataHelper _FeedbackFormDataHelper = new FeedbackFormDataHelper();
                _FeedbackFormDataHelper.StartFeedbackFormSync();

                AssetManagmentEquipmentListHelper _AMEHelper = new AssetManagmentEquipmentListHelper();
                _AMEHelper.StartAMESync();

                CybersecurityRisksAssessmentHelper _CRAHelper = new CybersecurityRisksAssessmentHelper();
                _CRAHelper.StartCRASync();

                // RDBJ 01/02/2021
                HelpAndSupportHelper _helpAndSupportHelper = new HelpAndSupportHelper();
                _helpAndSupportHelper.StartHelpAndSupportSyncLocalToAWSServer();
                _helpAndSupportHelper.StartHelpAndSupportSyncAWSServerToLocal();
                // End RDBJ 01/02/2021

                // RDBJ 02/26/2022
                Utility.GetMainSyncServiceDataAndSaveInMainSyncServiceFile();
                LogHelper.writelog("Main Sync Service Data Saved!");
                // End RDBJ 02/26/2022
            }
            catch (Exception)
            {

            }
        }
    }
}
