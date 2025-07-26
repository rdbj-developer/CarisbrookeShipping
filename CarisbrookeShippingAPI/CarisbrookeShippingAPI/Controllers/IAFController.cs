using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class IAFController : ApiController
    {

        //RDBJ 11/19/2021
        [HttpPost]
        public APIResponse IAFAutoSave([FromBody]BLL.Modals.IAF value)
        {
            APIResponse res = new APIResponse();
            try
            {
                IAFHelper _helper = new IAFHelper();
                _helper.IAFAutoSave(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFAutoSave Exception : " + ex.Message);
                LogHelper.writelog("IAFAutoSave Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        //End RDBJ 11/19/2021

        [HttpPost]
        public APIResponse SubmitIAForm([FromBody]BLL.Modals.IAF value)
        {
            APIResponse res = new APIResponse();
            try
            {
                IAFHelper _helper = new IAFHelper();
                _helper.SubmitIAForm(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitIAForm Exception : " + ex.Message);
                LogHelper.writelog("SubmitIAForm Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public List<Entity.InternalAuditForm> GetAllInternalAuditForm()
        {
            List<Entity.InternalAuditForm> list = new List<Entity.InternalAuditForm>();
            try
            {
                IAFHelper _helper = new IAFHelper();
                list = _helper.GetAllInternalAuditForm();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllInternalAuditForm Exception : " + ex.Message);
                LogHelper.writelog("GetAllInternalAuditForm Inner Exception : " + ex.InnerException);
            }
            return list;
        }
        [HttpGet]
        public Dictionary<string, string> GetNumberForNotes(string ship
             , string UniqueFormID   // RDBJ 01/22/2022
            )
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            try
            {
                IAFHelper _helper = new IAFHelper();
                data = _helper.GetNumberForNotes(ship
                    , UniqueFormID); // RDBJ 01/22/2022
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetNumberForNotes Exception : " + ex.Message);
                LogHelper.writelog("GetNumberForNotes Inner Exception : " + ex.InnerException);
            }
            return data;
        }
        [HttpGet]
        public IAF IAFormDetailsView(Guid? id)
        {
            IAF list = new IAF();
            try
            {
                IAFHelper _helper = new IAFHelper();
                list = _helper.IAFormDetailsView(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IAFormDetailsView Exception : " + ex.Message);
                LogHelper.writelog("IAFormDetailsView Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        [HttpGet]
        public BLL.Modals.AuditNote GetAuditNotesById(Guid id)
        {
            BLL.Modals.AuditNote list = new BLL.Modals.AuditNote();
            try
            {
                IAFHelper _helper = new IAFHelper();
                list = _helper.GetAuditNotesById(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDeficienciesById Exception : " + ex.Message);
                LogHelper.writelog("GetDeficienciesById Inner Exception : " + ex.InnerException);
            }
            return list;
        }

        public APIResponse AddAuditNoteResolutions([FromBody] Audit_Note_Resolutions value)
        {
            APIResponse res = new APIResponse();
            try
            {
                IAFHelper _helper = new IAFHelper();
                _helper.AddAuditNoteResolutions(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddAuditNoteResolutions Exception : " + ex.Message);
                LogHelper.writelog("AddAuditNoteResolutions Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }

        [HttpGet]
        public List<Audit_Note_Resolutions> GetAuditNoteResolutions(Guid id)
        {
            IAFHelper _helper = new IAFHelper();
            List<Audit_Note_Resolutions> Resolutions = _helper.GetAuditNoteResolutions(id);
            return Resolutions;
        }
        [HttpGet]
        public List<Audit_Note_Resolutions_Files> GetAuditNoteResolutionFiles(long id)
        {
            IAFHelper _helper = new IAFHelper();
            List<Audit_Note_Resolutions_Files> ResolutionsFiles = _helper.GetAuditNoteResolutionFiles(id);
            return ResolutionsFiles;
        }

        [HttpGet]
        public Dictionary<string, string> GetFileAuditNoteResolution(string id) // RDBJ 01/27/2022 set with dictionary
        {
            IAFHelper _helper = new IAFHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            retDicData = _helper.GetFileAuditNoteResolution(id); // RDBJ 01/27/2022 set with dictionary
            return retDicData;
        }

        //RDBJ 11/13/2021
        [HttpGet]
        public bool UpdateIAFAuditNotePriority(string NotesUniqueID, int PriorityWeek)
        {
            bool response = false;
            if (!string.IsNullOrEmpty(NotesUniqueID))
            {
                IAFHelper _helper = new IAFHelper();
                response = _helper.UpdateIAFAuditNotePriority(NotesUniqueID, PriorityWeek);
            }

            return response;
        }
        //End RDBJ 11/13/2021

        //RDBJ 11/17/2021
        public APIResponse AddIAFAuditNote([FromBody] AuditNote value)
        {
            APIResponse res = new APIResponse();
            try
            {
                IAFHelper _helper = new IAFHelper();
                _helper.AddIAFAuditNote(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddIAFAuditNote Exception : " + ex.Message);
                LogHelper.writelog("AddIAFAuditNote Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        //End RDBJ 11/17/2021

        //RDBJ 11/24/2021
        [HttpGet]
        public bool UpdateAdditionalAndCloseStatus(string id, bool IsAdditionalAndClosedStatus, bool IsAdditionalAndClosed)
        {
            try
            {
                IAFHelper _helper = new IAFHelper();
                _helper.UpdateAdditionalAndCloseStatus(id, IsAdditionalAndClosedStatus, IsAdditionalAndClosed);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateAdditionalAndCloseStatus Exception : " + ex.Message);
                LogHelper.writelog("UpdateAdditionalAndCloseStatus : Inner Exception " + ex.InnerException);
                return false;
            }
            return true;
        }
        //End RDBJ 11/24/2021

        //RDBJ 11/24/2021
        [HttpGet]
        public bool RemoveAuditsOrAuditNotes(string id
            , bool IsAudit //RDBJ 11/25/2021
            )
        {
            try
            {
                IAFHelper _helper = new IAFHelper();
                _helper.RemoveAuditsOrAuditNotes(id, IsAudit); //RDBJ 11/25/2021 Added IsAudit
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RemoveAuditsOrAuditNotes Exception : " + ex.Message);
                LogHelper.writelog("RemoveAuditsOrAuditNotes : Inner Exception " + ex.InnerException);
                return false;
            }
            return true;
        }
        //End RDBJ 11/24/2021

        //RDBJ 11/29/2021
        [HttpGet]
        public List<SMSReferencesTree> GetSMSReferenceData()
        {
            IAFHelper _helper = new IAFHelper();
            List<SMSReferencesTree> response = _helper.GetSMSReferenceData();
            return response;
        }
        //End RDBJ 11/29/2021

        //RDBJ 11/29/2021
        [HttpGet]
        public List<SSPReferenceTree> GetSSPReferenceData()
        {
            IAFHelper _helper = new IAFHelper();
            List<SSPReferenceTree> response = _helper.GetSSPReferenceData();
            return response;
        }
        //End RDBJ 11/29/2021
        
        //RDBJ 11/29/2021
        [HttpGet]
        public List<MLCRegulationTree> GetMLCRegulationTree()
        {
            IAFHelper _helper = new IAFHelper();
            List<MLCRegulationTree> response = _helper.GetMLCRegulationTree();
            return response;
        }
        //End RDBJ 11/29/2021

        // RDBJ2 02/23/2022
        [HttpPost]
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            IAFHelper _helper = new IAFHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End RDBJ2 02/23/2022
    }
}
