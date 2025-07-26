using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class DocumentsController : ApiController
    {
        [HttpGet]
        public List<DocumentsModal> GetAllDocuments(string sectionType = "")
        {
            List<DocumentsModal> DocList = new List<DocumentsModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                DocList = _helper.GetAllDocuments(sectionType);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocuments Exception : " + ex.Message);
                LogHelper.writelog("GetAllDocuments Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }

        [HttpGet]
        public List<DocumentsModal> GetAllDocumentsForOfficeApp(string sectionType = "")
        {
            List<DocumentsModal> DocList = new List<DocumentsModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                DocList = _helper.GetAllDocumentsForOfficeApp(sectionType);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentsForOfficeApp Exception : " + ex.Message);
                LogHelper.writelog("GetAllDocumentsForOfficeApp Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }

        [HttpGet]
        public List<DocumentsModal> GetAllDocumentsForShip(string sectionType = "", string shipCode = "")
        {
            List<DocumentsModal> DocList = new List<DocumentsModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                DocList = _helper.GetAllDocumentsForShip(sectionType, shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentsForShip Exception : " + ex.Message);
                LogHelper.writelog("GetAllDocumentsForShip Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }

        [HttpGet]
        public List<DocumentsModal> GetAllDocumentsWithRAData(string shipCode)
        {
            List<DocumentsModal> DocList = new List<DocumentsModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                DocList = _helper.GetAllDocumentsWithRAData(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentsWithRAData Exception : " + ex.Message);
                LogHelper.writelog("GetAllDocumentsWithRAData Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }

        [HttpGet]
        public List<DocumentsModal> GetAllDocumentsForService(bool isGetAllRecord = false)
        {
            List<DocumentsModal> DocList = new List<DocumentsModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                DocList = _helper.GetAllDocumentsForService(isGetAllRecord);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllDocumentsForService Exception : " + ex.Message);
                LogHelper.writelog("GetAllDocumentsForService Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }
        [HttpGet]
        public DocumentsModal GetDocumentBYID(string id)
        {
            DocumentsModal DocModal = new DocumentsModal();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                DocModal = _helper.GetDocumentByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentBYID Exception : " + ex.Message);
                LogHelper.writelog("GetDocumentBYID Inner Exception : " + ex.InnerException);
            }
            return DocModal;
        }
        [HttpPost]
        public APIResponse AddDocument([FromBody]DocumentsModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.AddDocument(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("AddDocument Exception : " + ex.Message);
                LogHelper.writelog("AddDocument Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public APIResponse DeleteDocument(string id)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.DeleteDocument(id);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteDocument Exception : " + ex.Message);
                LogHelper.writelog("DeleteDocument Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse UpdateDocument([FromBody]DocumentsModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.UpdateDocument(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDocument Exception : " + ex.Message);
                LogHelper.writelog("UpdateDocument Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpPost]
        public APIResponse UpdateDocumentFile([FromBody]DocumentsModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.UpdateDocumentFile(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateDocumentFile Exception : " + ex.Message);
                LogHelper.writelog("UpdateDocumentFile Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public List<RepositoryModal> GetAllRepositories()
        {
            List<RepositoryModal> DocList = new List<RepositoryModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                DocList = _helper.GetAllRepositories();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllRepositories Exception : " + ex.Message);
                LogHelper.writelog("GetAllRepositories Inner Exception : " + ex.InnerException);
            }
            return DocList;
        }

        #region AssetManagmentEquipmentList
        [HttpPost]
        public APIResponse SubmitAssetManagmentEquipmentList([FromBody]AssetManagmentEquipmentListModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.SubmitAssetManagmentEquipmentListData(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitAssetManagmentEquipmentList Exception : " + ex.Message);
                LogHelper.writelog("SubmitAssetManagmentEquipmentList Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public AssetManagmentEquipmentListModal GetAssetManagmentEquipmentData(string shipCode)
        {
            AssetManagmentEquipmentListModal res = new AssetManagmentEquipmentListModal();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetAssetManagmentEquipmentData(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentData Exception : " + ex.Message);
                LogHelper.writelog("GetAssetManagmentEquipmentData Inner Exception : " + ex.InnerException);
            }
            return res;
        }
        [HttpGet]
        public AssetManagmentEquipmentListModal GetAssetManagmentEquipmentUnSyncData(string shipCode)
        {
            AssetManagmentEquipmentListModal res = new AssetManagmentEquipmentListModal();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetAssetManagmentEquipmentUnSyncData(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentEquipmentUnSyncData Exception : " + ex.Message);
                LogHelper.writelog("GetAssetManagmentEquipmentUnSyncData Inner Exception : " + ex.InnerException);
            }
            return res;
        }
        [HttpGet]
        public APIResponse UpdateAssetManagmentEquipmentSyncStatus(string shipCode, bool isSynced)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.UpdateAssetManagmentEquipmentSyncStatus(shipCode, isSynced);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateAssetManagmentEquipmentSyncStatus Exception : " + ex.Message);
                LogHelper.writelog("UpdateAssetManagmentEquipmentSyncStatus Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public List<string> GetAssetManagmentHardwareId(string shipCode)
        {
            List<string> res = new List<string>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetAssetManagmentHardwareId(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAssetManagmentHardwareId Exception : " + ex.Message);
                LogHelper.writelog("GetAssetManagmentHardwareId Inner Exception : " + ex.InnerException);
            }
            return res;
        }
        #endregion

        #region CybersecurityRisksAssessment  
        [HttpPost]
        public APIResponse SubmitCybersecurityRisksAssessment([FromBody]CybersecurityRisksAssessmentModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.SubmitCybersecurityRisksAssessment(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitCybersecurityRisksAssessment Exception : " + ex.Message);
                LogHelper.writelog("SubmitCybersecurityRisksAssessment Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public CybersecurityRisksAssessmentModal GetCybersecurityRisksAssessmentData(string shipCode) {
            CybersecurityRisksAssessmentModal res = new CybersecurityRisksAssessmentModal();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetCybersecurityRisksAssessmentData(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCybersecurityRisksAssessmentData Exception :" + ex.Message);
                LogHelper.writelog("GetCybersecurityRisksAssessmentData Inner Exception :" + ex.InnerException);
            }
            return res;
        }
        [HttpGet]
        public CybersecurityRisksAssessmentModal GetCybersecurityRisksAssessmentUnSyncData(string shipCode)
        {
            CybersecurityRisksAssessmentModal res = new CybersecurityRisksAssessmentModal();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetCybersecurityRisksAssessmentUnSyncData(shipCode);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCybersecurityRisksAssessmentUnSyncData Exception : " + ex.Message);
                LogHelper.writelog("GetCybersecurityRisksAssessmentUnSyncData Inner Exception : " + ex.InnerException);
            }
            return res;
        }
        [HttpGet]
        public APIResponse UpdateCybersecurityRisksAssessmentSyncStatus(string shipCode, bool isSynced)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.UpdateCybersecurityRisksAssessmentSyncStatus(shipCode, isSynced);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateCybersecurityRisksAssessmentSyncStatus Exception : " + ex.Message);
                LogHelper.writelog("UpdateCybersecurityRisksAssessmentSyncStatus Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        [HttpGet]
        public List<string> GetCyberSecurityRiskList()
        {
            List<string> res = new List<string>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetCyberSecurityRiskList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecurityRiskList Exception :" + ex.Message);
                LogHelper.writelog("GetCyberSecurityRiskList Inner Exception :" + ex.InnerException);
            }
            return res;

        }
        [HttpGet]
        public List<string> GetCyberSecurityVulnerabilitiesList()
        {
            List<string> res = new List<string>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetCyberSecurityVulnerabilitiesList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecurityVulnerabilitiesList Exception :" + ex.Message);
                LogHelper.writelog("GetCyberSecurityVulnerabilitiesList Inner Exception :" + ex.InnerException);
            }
            return res;

        }
        public List<CyberSecuritySettingsModal> GetCyberSecuritySettingsListByType(string type)
        {
            List<CyberSecuritySettingsModal> res = new List<CyberSecuritySettingsModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetCyberSecuritySettingsListByType(type);
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetCyberSecuritySettingsListByType Exception :" + ex.Message);
                LogHelper.writelog("GetCyberSecuritySettingsListByType Inner Exception :" + ex.InnerException);
            }
            return res;
        }
        public List<CyberSecuritySettingsModal> GetAllCyberSecuritySettingsList()
        {
            List<CyberSecuritySettingsModal> res = new List<CyberSecuritySettingsModal>();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                res = _helper.GetAllCyberSecuritySettingsList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllCyberSecuritySettingsList Exception :" + ex.Message);
                LogHelper.writelog("GetAllCyberSecuritySettingsList Inner Exception :" + ex.InnerException);
            }
            return res;
        }
        [HttpPost]
        public APIResponse CopyCybersecurityRisksAssessment([FromBody] CyberSecurityCopyDataModal value)
        {
            APIResponse res = new APIResponse();
            try
            {
                DocumentsHelper _helper = new DocumentsHelper();
                _helper.CopyCybersecurityRisksAssessment(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CopyCybersecurityRisksAssessment Exception : " + ex.Message);
                LogHelper.writelog("CopyCybersecurityRisksAssessment Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return res;
        }
        #endregion
    }
}
