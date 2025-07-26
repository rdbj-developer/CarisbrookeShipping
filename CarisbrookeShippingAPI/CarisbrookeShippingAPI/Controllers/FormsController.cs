using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize]   // JSL 09/26/2022
    public class FormsController : ApiController
    {
        [HttpGet]
        public string Connect()
        {
            return "API Connected";
        }

        #region GIRForms
        [HttpPost]
        public APIResponse SubmitGIRForm([FromBody] BLL.Modals.GeneralInspectionReport value)
        {
            APIResponse res = new APIResponse();
            try
            {
                GIRHelper _helper = new GIRHelper();
                bool response = _helper.SubmitGIR(value);
                if (response)
                    res.result = AppStatic.SUCCESS;
                else
                    res.result = AppStatic.ERROR;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitGIRForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitGIRForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public List<BLL.Modals.GIRData> GetGIRFormsFilled(string shipCode = "")
        {
            List<BLL.Modals.GIRData> list = new List<BLL.Modals.GIRData>();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetGIRFormsFilled(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRFormsFilled Exception : " + ex.Message);
                LogHelper.writelog("GetGIRFormsFilled Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public List<BLL.Modals.GIRDataList> GetDeficienciesData(string id)
        {
            List<BLL.Modals.GIRDataList> list = new List<BLL.Modals.GIRDataList>();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetDeficienciesData(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesData : " + ex.Message);
                LogHelper.writelog("GetDeficienciesData : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public bool UpdateDeficienciesData(string id, bool isClose)
        {
            try
            {
                GIRHelper _helper = new GIRHelper();
                _helper.UpdateDeficienciesData(id, isClose);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDeficienciesData Exception : " + ex.Message);
                LogHelper.writelog("UpdateDeficienciesData : Inner Exception " + ex.InnerException);
                return false;
            }
            return true;
        }
        [HttpGet]
        public int DeficienciesNumber(string ship, string reportType, string UniqueFormID) //RDBJ 11/02/2021 Added UniqueFormID //RDBJ 09/24/2021 Added reportType
        {
            int nextnumber = 0;
            try
            {
                GIRHelper _helper = new GIRHelper();
                nextnumber = _helper.DeficienciesNumber(ship, reportType, UniqueFormID); //RDBJ 11/02/2021 Added UniqueFormID //RDBJ 09/24/2021 Added reportType //RDBJ 09/18/2021 removed ,Guid.Empty,""
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeficienciesNumber Exception : " + ex.Message);
                LogHelper.writelog("DeficienciesNumber Inner Exception : " + ex.InnerException);
            }
            return nextnumber;
        }
        [HttpGet]
        public List<BLL.Modals.DeficienciesNote> GetDeficienciesNote(Guid id)
        {
            List<BLL.Modals.DeficienciesNote> list = new List<BLL.Modals.DeficienciesNote>();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetDeficienciesNote(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesNote Exception : " + ex.Message);
                LogHelper.writelog("GetDeficienciesNote Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public List<BLL.Modals.GIRDeficienciesFile> GetDeficienciesFiles(int id)
        {
            List<BLL.Modals.GIRDeficienciesFile> list = new List<BLL.Modals.GIRDeficienciesFile>();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetDeficienciesFiles(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesFiles Exception : " + ex.Message);
                LogHelper.writelog("GetDeficienciesFiles Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        [HttpGet]
        public BLL.Modals.GIRDeficiencies GetDeficienciesById(Guid id)
        {
            BLL.Modals.GIRDeficiencies list = new BLL.Modals.GIRDeficiencies();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetDeficienciesById(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesById Exception : " + ex.Message);
                LogHelper.writelog("GetDeficienciesById Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpPost]
        public APIResponse AddDeficienciesNote([FromBody] BLL.Modals.DeficienciesNote value)
        {
            APIResponse res = new APIResponse();
            try
            {
                GIRHelper _helper = new GIRHelper();
                _helper.AddDeficienciesNote(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesNote Exception : " + ex.Message);
                LogHelper.writelog("AddDeficienciesNote Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public Dictionary<string, string> GetFile(int id) // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                GIRHelper _helper = new GIRHelper();
                retDicData = _helper.GetFile(id); // RDBJ 01/27/2022 set with dictionary
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFile Exception : " + ex.Message);
                LogHelper.writelog("GetFile Inner Exception : " + ex.InnerException);
                return retDicData;
            }
            return retDicData;
        }
        [HttpGet]
        public string GetFileComment(int id)
        {
            string str = "";
            try
            {
                GIRHelper _helper = new GIRHelper();
                str = _helper.GetFileComment(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFileComment Exception : " + ex.Message);
                LogHelper.writelog("GetFileComment Inner Exception : " + ex.InnerException);
                return str;
            }
            return str;
        }

        [HttpGet]
        public Dictionary<string, string> GetCommentFile(string id) // RDBJ 01/27/2022 set with dictionary
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                GIRHelper _helper = new GIRHelper();
                retDicData = _helper.GetCommentFile(id); // RDBJ 01/27/2022 set with dictionary
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCommentFile Exception : " + ex.Message);
                LogHelper.writelog("GetCommentFile Inner Exception : " + ex.InnerException);
                return retDicData;
            }
            return retDicData;
        }

        [HttpGet]
        public Dictionary<string, string> GetGIRDeficienciesInitialActionFile(string id) // RDBJ 01/27/2022 set with dictionary //RDBJ 11/10/2021 changed datatype from int to string
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                GIRHelper _helper = new GIRHelper();
                retDicData = _helper.GetGIRDeficienciesInitialActionFile(id); // RDBJ 01/27/2022 set with dictionary
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesInitialActionFile Exception : " + ex.Message);
                LogHelper.writelog("GetGIRDeficienciesInitialActionFile Inner Exception : " + ex.InnerException);
                return retDicData;
            }
            return retDicData;
        }
        [HttpGet]
        public Dictionary<string, string> GetGIRDeficienciesResolutionFile(string id) // RDBJ 01/27/2022 set with dictionary //RDBJ 11/10/2021 changed datatype from int to string
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            try
            {
                GIRHelper _helper = new GIRHelper();
                retDicData = _helper.GetGIRDeficienciesResolutionFile(id); // RDBJ 01/27/2022 set with dictionary
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetGIRDeficienciesResolutionFile Exception : " + ex.Message);
                LogHelper.writelog("GetGIRDeficienciesResolutionFile Inner Exception : " + ex.InnerException);
                return retDicData;
            }
            return retDicData;
        }
        [HttpGet]
        public List<Entity.GeneralInspectionReport> GetAllGIRForms()
        {
            List<Entity.GeneralInspectionReport> list = new List<Entity.GeneralInspectionReport>();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetAllGIRForms();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllGIRForms Exception : " + ex.Message);
                LogHelper.writelog("GetAllGIRForms Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        [HttpGet]
        public BLL.Modals.GeneralInspectionReport GIRFormDetailsView(int id)
        {
            BLL.Modals.GeneralInspectionReport list = new BLL.Modals.GeneralInspectionReport();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GIRFormDetailsView(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GeneralInspectionReport Exception : " + ex.Message);
                LogHelper.writelog("GeneralInspectionReport Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        [HttpGet]
        public BLL.Modals.GeneralInspectionReport GIRFormDetailsViewByGUID(string id)
        {
            BLL.Modals.GeneralInspectionReport list = new BLL.Modals.GeneralInspectionReport();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GIRFormDetailsViewByGUID(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GeneralInspectionReport Exception : " + ex.Message);
                LogHelper.writelog("GeneralInspectionReport Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        [HttpGet]
        public BLL.Modals.GeneralInspectionReport GIRDeficienciesView(int id)
        {
            BLL.Modals.GeneralInspectionReport list = new BLL.Modals.GeneralInspectionReport();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GIRDeficienciesView(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRDeficienciesView Exception : " + ex.Message);
                LogHelper.writelog("GIRDeficienciesView Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpPost]
        public string GIRAutoSave([FromBody] BLL.Modals.GeneralInspectionReport value)
        {
            GIRHelper _helper = new GIRHelper();
            string id = _helper.GIRAutoSave(value);
            return id;
        }
        public string GIRSaveFormDarf([FromBody] BLL.Modals.GeneralInspectionReport value)
        {
            GIRHelper _helper = new GIRHelper();
            string id = _helper.GIRAutoSave(value, true);
            return id;
        }

        public APIResponse AddGIRDeficiencies([FromBody] GIRDeficiencies value)
        {
            APIResponse res = new APIResponse();
            try
            {
                GIRHelper _helper = new GIRHelper();
                res = _helper.AddGIRDeficiencies(value);
                //res.result = AppStatic.SUCCESS;   // RDBJ 02/17/2022 commented this line
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddGIRDeficiencies Exception : " + ex.Message);
                LogHelper.writelog("AddGIRDeficiencies Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public BLL.Modals.GeneralInspectionReport GIRFormGetDeficiency(string id)
        {
            BLL.Modals.GeneralInspectionReport list = new BLL.Modals.GeneralInspectionReport();
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GIRFormGetDeficiency(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRFormGetDeficiency Exception : " + ex.Message);
                LogHelper.writelog("GIRFormGetDeficiency Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        public APIResponse AddDeficienciesInitialActions([FromBody] GIRDeficienciesInitialActions value) //RDBJ 09/22/2021 Used Updated Modal class Name
        {
            APIResponse res = new APIResponse();
            try
            {
                GIRHelper _helper = new GIRHelper();
                res = _helper.AddDeficienciesInitialActions(value);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesInitialActions Exception : " + ex.Message);
                LogHelper.writelog("AddDeficienciesInitialActions Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        public APIResponse AddDeficienciesResolution([FromBody] BLL.Modals.GIRDeficienciesResolution value) //RDBJ 09/22/2021 Used Updated Modal class Name
        {
            APIResponse res = new APIResponse();
            try
            {
                GIRHelper _helper = new GIRHelper();
                _helper.AddDeficienciesResolution(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDeficienciesResolution Exception : " + ex.Message);
                LogHelper.writelog("AddDeficienciesResolution Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public List<GIRDeficienciesInitialActions> GetDeficienciesInitialActions(Guid id) //RDBJ 09/22/2021 Used Updated Modal class Name
        {
            List<GIRDeficienciesInitialActions> list = new List<GIRDeficienciesInitialActions>(); //RDBJ 09/22/2021 Used Updated Modal class Name
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetDeficienciesInitialActions(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesInitialActions Exception : " + ex.Message);
                LogHelper.writelog("GetDeficienciesInitialActions Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public List<BLL.Modals.GIRDeficienciesResolution> GetDeficienciesResolution(Guid id) //RDBJ 09/22/2021 Used Updated Modal class Name
        {
            List<BLL.Modals.GIRDeficienciesResolution> list = new List<BLL.Modals.GIRDeficienciesResolution>(); //RDBJ 09/22/2021 Used Updated Modal class Name
            try
            {
                GIRHelper _helper = new GIRHelper();
                list = _helper.GetDeficienciesResolution(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesResolution Exception : " + ex.Message);
                LogHelper.writelog("GetDeficienciesResolution Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        //RDBJ 10/07/2021
        public bool GIRShipGeneralDescriptionSave(BLL.Modals.CSShipsModal modal)
        {
            bool res = false;
            try
            {
                GIRHelper _helper = new GIRHelper();
                res = _helper.GIRShipGeneralDescriptionSave(modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GIRShipGeneralDescriptionSave Exception : " + ex.Message);
            }
            return res;
        }
        //End RDBJ 10/07/2021

        #endregion

        #region SMRForms
        [HttpPost]
        public APIResponse SubmitSMRForm([FromBody] SMRModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                SMRHelper _helper = new SMRHelper();
                _helper.SubmitSMR(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitSMRForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitSMRForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpPost]
        public List<SMRModal> SMRFormsFilled([FromBody] SMRFormReq value)
        {
            List<SMRModal> SMRList = new List<SMRModal>();
            try
            {
                SMRHelper _helper = new SMRHelper();
                SMRList = _helper.GetSMRFormsFilled(value);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SMRFormsFilled Exception : " + ex.Message);
                LogHelper.writelog("SMRFormsFilled Inner Exception : " + ex.InnerException);
            }
            return SMRList;
        }


        [HttpGet]
        public SMRModal GetSMRFormByID(string id)
        {
            SMRModal Modal = new SMRModal();
            try
            {
                SMRHelper _helper = new SMRHelper();
                Modal = _helper.GetSMRFormsFilledByID(Convert.ToInt64(id));
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormByID Exception : " + ex.Message);
                LogHelper.writelog("GetSMRFormByID Inner Exception : " + ex.InnerException);
            }
            return Modal;
        }
        #endregion

        #region ArrivalReport
        [HttpPost]
        public APIResponse SubmitArrivalReport([FromBody] ArrivalReportModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.SubmitArrivalReport(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitArrivalReport Exception : " + ex.Message);
                LogHelper.writelog("SubmitArrivalReport Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public ArrivalReportModal ArrivalDetailsView(int id)
        {
            ArrivalReportModal list = new ArrivalReportModal();
            try
            {
                FormsHelper _helper = new FormsHelper();
                list = _helper.ArrivalDetailsView(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("ArrivalDetailsView Exception : " + ex.Message);
                LogHelper.writelog("ArrivalDetailsView Inner Exception : " + ex.InnerException);
            }
            return list;
        }


        #endregion

        #region DepartureReport
        [HttpPost]
        public APIResponse SubmitDepartureReport([FromBody] DepartureReportModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.SubmitDepartureReport(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitArrivalReport Exception : " + ex.Message);
                LogHelper.writelog("SubmitArrivalReport Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #endregion

        #region DailyCargoReport
        [HttpPost]
        public APIResponse SubmitDailyCargoReport([FromBody] DailyCargoReportModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.SubmitDailyCargoReport(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitArrivalReport Exception : " + ex.Message);
                LogHelper.writelog("SubmitArrivalReport Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public DailyCargoReportModal DailyCargoFormDetailsView(int id)
        {
            DailyCargoReportModal list = new DailyCargoReportModal();
            try
            {
                FormsHelper _helper = new FormsHelper();
                list = _helper.DailyCargoFormDetailsView(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DailyCargoFormDetailsView Exception : " + ex.Message);
                LogHelper.writelog("DailyCargoFormDetailsView Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        #endregion

        #region DailyPositionReport
        [HttpPost]
        public APIResponse SubmitDailyPositionReport([FromBody] DailyPositionReportModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.SubmitDailyPositionReport(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitArrivalReport Exception : " + ex.Message);
                LogHelper.writelog("SubmitArrivalReport Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #endregion

        #region SIRForms
        [HttpPost]
        public APIResponse SubmitSIRForm([FromBody] SIRModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                SIRHelper _helper = new SIRHelper();
                _helper.SubmitSIR(value);

                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitSIRForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitSIRForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public string SIRAutoSave([FromBody] SIRModal value)
        {
            SIRHelper _helper = new SIRHelper();
            string id = _helper.SIRAutoSave(value);
            return id;
        }
        [HttpGet]
        public List<Entity.SuperintendedInspectionReport> GetAllSIRForms()
        {
            List<Entity.SuperintendedInspectionReport> list = new List<Entity.SuperintendedInspectionReport>();
            try
            {
                SIRHelper _helper = new SIRHelper();
                list = _helper.GetAllSIRForms();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllSIRForms Exception : " + ex.Message);
                LogHelper.writelog("GetAllSIRForms Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public SIRModal SIRFormDetailsView(string id)
        {
            SIRModal list = new SIRModal();
            try
            {
                SIRHelper _helper = new SIRHelper();
                list = _helper.SIRFormDetailsView(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRFormDetailsView Exception : " + ex.Message);
                LogHelper.writelog("SIRFormDetailsView Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public List<BLL.Modals.SIRNote> SIRFormNotes(string id)
        {
            List<BLL.Modals.SIRNote> list = new List<BLL.Modals.SIRNote>();
            try
            {
                SIRHelper _helper = new SIRHelper();
                list = _helper.SIRFormNotes(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRFormNotes Exception : " + ex.Message);
                LogHelper.writelog("SIRFormNotes Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        //public APIResponse AddSIRDeficiencies([FromBody] GIRDeficiencies value)
        //{
        //    APIResponse res = new APIResponse();
        //    try
        //    {
        //        GIRHelper _helper = new GIRHelper();
        //        _helper.AddGIRDeficiencies(value);
        //        res.result = AppStatic.SUCCESS;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.writelog("AddGIRDeficiencies Exception : " + ex.Message);
        //        LogHelper.writelog("AddGIRDeficiencies Inner Exception : " + ex.InnerException);
        //        res.result = AppStatic.ERROR;
        //        res.msg = ex.Message;
        //    }
        //    return res;
        //}


        [HttpGet]
        public BLL.Modals.SIRModal SIRFormGetDeficiency(string id)
        {
            BLL.Modals.SIRModal list = new BLL.Modals.SIRModal();
            try
            {
                SIRHelper _helper = new SIRHelper();
                list = _helper.SIRFormGetDeficiency(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRFormGetDeficiency Exception : " + ex.Message);
                LogHelper.writelog("SIRFormGetDeficiency Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        #endregion

        #region HoldVentilationRecordForm
        [HttpPost]
        public APIResponse SubmitHoldVentilationRecordForm([FromBody] HoldVentilationRecordFormModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                HoldVentilationRecordFormHelper _helper = new HoldVentilationRecordFormHelper();
                _helper.SubmitHoldVentilationRecord(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitHoldVentilationRecordForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitHoldVentilationRecordForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #endregion

        #region RiskAssessmentForm
        [HttpGet]
        public List<RiskAssessmentForm> GetRiskAssessmentFormList(string id)
        {
            RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
            List<RiskAssessmentForm> RAFList = _helper.GetRAFDrafts(id);
            return RAFList;
        }

        [HttpPost]
        public RiskAssessmentFormModal RAFormDetailsView(RAFDetailViewRequestModal value)
        {
            RiskAssessmentFormModal list = new RiskAssessmentFormModal();
            try
            {
                if (value != null)
                {
                    RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                    list = _helper.RAFormDetailsView(value.ShipName, value.id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RAFormDetailsView Exception : " + ex.Message);
                LogHelper.writelog("RAFormDetailsView Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        [HttpPost]
        public APIResponse SubmitRiskAssessmentForm([FromBody] RiskAssessmentFormModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                _helper.SubmitRiskAssessmentFormData(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitRiskAssessmentForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitRiskAssessmentForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpGet]
        public bool CheckRAFNumberExistFromData(string ShipName, string RAFNumber)
        {
            bool res = false;
            try
            {
                RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                res = _helper.CheckRAFNumberExistFromData(ShipName, RAFNumber);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitRiskAssessmentForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitRiskAssessmentForm Inner Exception : " + ex.InnerException);
            }
            return res;
        }

        [HttpGet]
        public List<RiskAssessmentForm> GetAllDocumentRiskassessment(string id)
        {
            RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
            List<RiskAssessmentForm> RAFList = _helper.GetAllDocumentRiskassessment(id);
            return RAFList;
        }

        [HttpPost]
        public bool InsertDocumentsBulkDataInRiskAssessment(List<RiskAssessmentForm> AllDocuments)
        {
            bool res = false;
            try
            {
                RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                res = _helper.InsertDocumentsBulkDataInRiskAssessment(AllDocuments);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssessment : " + ex.Message);
            }
            return res;
        }
        [HttpPost]
        public bool InsertDocumentsBulkDataInRiskAssesmentHazared(List<RiskAssessmentFormHazard> AllDocuments)
        {
            bool res = false;
            try
            {
                RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                res = _helper.InsertDocumentsBulkDataInRiskAssesmentHazared(AllDocuments);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssesmentHazared : " + ex.Message);
            }
            return res;
        }
        [HttpPost]
        public bool InsertDocumentsBulkDataInRiskAssessmentReviewer(List<RiskAssessmentFormReviewer> AllDocuments)
        {
            bool res = false;
            try
            {
                RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                res = _helper.InsertDocumentsBulkDataInRiskAssessmentReviewer(AllDocuments);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssessmentReviewer : " + ex.Message);
            }
            return res;
        }

        [HttpGet]
        public APIResponse GetRiskAssessmentReviewLog(string shipCode)
        {
            APIResponse res = new APIResponse();
            try
            {
                List<RiskAssessmentReviewLog> DocList = new List<RiskAssessmentReviewLog>();
                RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
                DocList = _helper.GetAllRiskAssessmentReviewLog(shipCode);
                res.msg = AppStatic.SUCCESS;
                res.result = JsonConvert.SerializeObject(DocList);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RiskAssessmentReviewLog Exception : " + ex.Message);
                LogHelper.writelog("RiskAssessmentReviewLog Inner Exception : " + ex.InnerException);
            }
            return res;
        }
        [HttpGet]
        public RiskAssessmentDataList GetRiskAssessmentDataToSeupShipApp(string id)
        {
            RiskAssessmentFormHelper _helper = new RiskAssessmentFormHelper();
            RiskAssessmentDataList RAFList = _helper.GetRiskAssessmentDataToSeupShipApp(id);
            return RAFList;
        }
        #endregion

        #region Forms
        [HttpGet]
        public List<FormsModal> GetAllForms()
        {
            List<FormsModal> DocList = new List<FormsModal>();
            try
            {
                FormsHelper _helper = new FormsHelper();
                DocList = _helper.GetAllForms();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllForms Exception : " + ex.Message);
                LogHelper.writelog("GetAllForms Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }
        [HttpGet]
        public List<FormsModal> GetAllFormsForService()
        {
            List<FormsModal> DocList = new List<FormsModal>();
            try
            {
                FormsHelper _helper = new FormsHelper();
                DocList = _helper.GetAllFormsForService();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllFormsForService Exception : " + ex.Message);
                LogHelper.writelog("GetAllFormsForService Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }
        [HttpGet]
        public FormsModal GetFormBYID(string id)
        {
            FormsModal DocModal = new FormsModal();
            try
            {
                FormsHelper _helper = new FormsHelper();
                DocModal = _helper.GetFormByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFormBYID Exception : " + ex.Message);
                LogHelper.writelog("GetFormBYID Inner Exception : " + ex.InnerException);
            }
            return DocModal;
        }
        [HttpPost]
        public APIResponse AddForm([FromBody] FormsModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.AddForm(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddForm Exception : " + ex.Message);
                LogHelper.writelog("AddForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public APIResponse DeleteForm(string id)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.DeleteForm(id);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteForm Exception : " + ex.Message);
                LogHelper.writelog("DeleteForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse UpdateForm([FromBody] FormsModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.UpdateForm(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateForm Exception : " + ex.Message);
                LogHelper.writelog("UpdateForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse UpdateFormFile([FromBody] FormsModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                _helper.UpdateFormFile(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateFormFile Exception : " + ex.Message);
                LogHelper.writelog("UpdateFormFile Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #endregion

        #region Reports
        [HttpPost]
        public List<ArrivalReportModal> GetAllArrivalDataReports(ShipModal Modal)
        {
            List<ArrivalReportModal> res = new List<ArrivalReportModal>();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.GetAllArrivalDataReports(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllArrivalDataReports Exception : " + ex.Message);
                LogHelper.writelog("GetAllArrivalDataReports Exception : " + ex.InnerException);
            }
            return res;
        }
        [HttpPost]
        public List<DepartureReportModal> GetAllDepartureDataReports(ShipModal Modal)
        {
            List<DepartureReportModal> res = new List<DepartureReportModal>();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.GetAllDepartureDataReports(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDepartureDataReports Exception : " + ex.Message);
                LogHelper.writelog("GetAllDepartureDataReports Exception : " + ex.InnerException);
            }
            return res;
        }
        [HttpPost]
        public List<DailyCargoReportModal> GetAllDailyCargoDataReports(ShipModal Modal)
        {
            List<DailyCargoReportModal> res = new List<DailyCargoReportModal>();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.GetAllDailyCargoDataReports(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDailyCargoDataReports Exception : " + ex.Message);
                LogHelper.writelog("GetAllDailyCargoDataReports Exception : " + ex.InnerException);
            }
            return res;
        }
        [HttpPost]
        public List<DailyPositionReportModal> GetAllDailyPositionDataReports(ShipModal Modal)
        {
            List<DailyPositionReportModal> res = new List<DailyPositionReportModal>();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.GetAllDailyPositionDataReports(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDailyPositionDataReports Exception : " + ex.Message);
                LogHelper.writelog("GetAllDailyPositionDataReports Exception : " + ex.InnerException);
            }
            return res;
        }

        #endregion

        #region Feedback Form
        [HttpPost]
        public APIResponse SubmitFeedbackForm([FromBody] FeedbackForm value)
        {
            APIResponse res = new APIResponse();
            try
            {
                FeedbackFormHelper _helper = new FeedbackFormHelper();
                _helper.SubmitFeedbackForm(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitFeedbackForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitFeedbackForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #endregion

        #region SiteInfo
        [HttpPost]
        public List<ShipWisePCModal> GetAllSiteInfoDatas(ShipModal Modal)
        {
            List<ShipWisePCModal> res = new List<ShipWisePCModal>();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.GetAllSiteInfoDatas(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllSiteInfoDatas Exception : " + ex.Message);
                LogHelper.writelog("GetAllSiteInfoDatas Exception : " + ex.InnerException);
            }
            return res;
        }


        [HttpPost]
        public APIResponse GetAllDeletePCRecords(ShipWisePCModal ShipCode)
        {
            //  List<ShipWisePCModal> res = new List<ShipWisePCModal>();
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.DeleteAllPCRecordData(ShipCode);
                //res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteAllPCRecord Exception : " + ex.Message);
                LogHelper.writelog("DeleteAllPCRecord Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse UpdateBlockPCRecords(ShipWisePCModal ShipCode)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.UpdateBlockPCRecords(ShipCode);
                //res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateBlockPCRecords Exception : " + ex.Message);
                LogHelper.writelog("UpdateBlockPCRecords Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse UpdateMainPCRecords(ShipWisePCModal ShipCode)
        {
            APIResponse res = new APIResponse();
            try
            {
                FormsHelper _helper = new FormsHelper();
                res = _helper.UpdateMainPCRecords(ShipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateMainPCRecords Exception : " + ex.Message);
                LogHelper.writelog("UpdateMainPCRecords Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #endregion

        #region Common Functions
        // RDBJ 03/05/2022
        [HttpPost]
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            GIRHelper _helper = new GIRHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End RDBJ 03/05/2022
        #endregion
    }
}
