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
    public class DeficienciesController : ApiController
    {
        [HttpGet]
        public List<Deficiency_GISI_Ships> GetGISIShips()
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Deficiency_GISI_Ships> list = _helper.GetGISIShips();
            return list;
        }
        [HttpGet]
        public List<Deficiency_GISI_Report> GetShipGISIReports(string id, string type)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Deficiency_GISI_Report> list = _helper.GetShipGISIReports(id, type);
            return list;
        }
        [HttpPost]
        public List<BLL.Modals.GIRDataList> GetDeficienciesData([FromBody]Deficiency_GISI_Report value)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<BLL.Modals.GIRDataList> list = _helper.GetDeficienciesData(value);
            return list;
        }

        [HttpGet]
        public List<Deficiency_Audit_Ships> GetAuditShips(string id)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Deficiency_Audit_Ships> list = _helper.GetAuditShips(id);
            return list;
        }
        [HttpGet]
        public List<Deficiency_Ship_Audits> GetShipAudits(string code
            , bool blnIsAddedNewAudit   // JSL 04/20/2022
            )
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Deficiency_Ship_Audits> list = _helper.GetShipAudits(code
                , blnIsAddedNewAudit    // JSL 04/20/2022
                );
            return list;
        }

        // JSL 02/11/2023
        [HttpGet]
        public List<FSTOInspection> GetFSTOAuditDataByShipCode(string code)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<FSTOInspection> list = _helper.GetFSTOAuditDataByShipCode(code);
            return list;
        }
        // End JSL 02/11/2023

        // JSL 02/17/2023
        [HttpGet]
        public Dictionary<string, string> GetFSTOFile(string id)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.GetFSTOFile(id);
            return retDicData;
        }
        // End JSL 02/17/2023

        [HttpGet]
        public bool UpdateAuditDeficiencies(string id, bool isClose)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            bool res = _helper.UpdateAuditDeficiencies(id, isClose);
            return res;
        }
        [HttpPost]
        public bool AddAuditDeficiencyComments([FromBody]Audit_Deficiency_Comments value)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            bool res = _helper.Add_Audit_Deficiency_Comments(value);
            return res;
        }
        [HttpGet]
        public List<Audit_Deficiency_Comments> GetAuditDeficiencyComments(Guid? id)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Audit_Deficiency_Comments> Comments = _helper.GetAuditDeficiencyComments(id);
            return Comments;
        }
        [HttpGet]
        public List<Audit_Deficiency_Comments_Files> GetAuditDeficiencyCommentFiles(long id)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Audit_Deficiency_Comments_Files> CommentFiles = _helper.GetAuditDeficiencyCommentFiles(id);
            return CommentFiles;
        }

        [HttpGet]
        public Dictionary<string, string> GetAuditFile(string id) // RDBJ 01/27/2022 set with dictionary
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            retDicData = _helper.GetAuditFile(id); // RDBJ 01/27/2022 set with dictionary
            return retDicData;
        }

        [HttpGet]
        public Dictionary<string, string> GetFileComment(string id) // RDBJ 01/27/2022 set with dictionary
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>(); // RDBJ 01/27/2022 set with dictionary
            retDicData = _helper.GetFileComment(id); // RDBJ 01/27/2022 set with dictionary
            return retDicData;
        }

        [HttpGet]
        public int DeleteGIRDeficiencies(string UniqueFormID, string ReportType, string defID) //RDBJ 11/02/2021 Added defID
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            int res = _helper.DeleteGIRDeficiencies(UniqueFormID, ReportType, defID); //RDBJ 11/02/2021 Added defID
            return res;
        }

        //RDBJ 11/02/2021
        public List<int> getDeficienciesDeletedNumbers(string ship, string reportType, string UniqueFormID) 
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<int> res = _helper.getDeficienciesDeletedNumbers(ship, reportType, UniqueFormID);
            return res;
        }
        //End RDBJ 11/02/2021

        //RDBJ 11/02/2021
        [HttpGet] //RDBJ 11/13/2021
        public bool UpdateDeficiencyPriority(string DeficienciesUniqueID, int PriorityWeek
            , string DueDate    // RDBJ 02/28/2022
            )
        {
            bool response = false;
            if (!string.IsNullOrEmpty(DeficienciesUniqueID))
            {
                DeficiencyHelper _helper = new DeficiencyHelper();
                response = _helper.UpdateDeficiencyPriority(DeficienciesUniqueID, PriorityWeek
                    , DueDate   // RDBJ 02/28/2022
                    );
            }

            return response;
        }
        //End RDBJ 11/02/2021

        //RDBJ 12/17/2021
        [HttpGet]
        public bool UpdateDeficiencyAssignToUser(string DeficienciesUniqueID, string AssignTo
            , string blnIsIAF   // RDBJ 12/21/2021
            , string blnIsNeedToDelete   // JSL 07/02/2022
            )
        {
            bool response = false;
            if (!string.IsNullOrEmpty(DeficienciesUniqueID))
            {
                DeficiencyHelper _helper = new DeficiencyHelper();
                response = _helper.UpdateDeficiencyAssignToUser(DeficienciesUniqueID, AssignTo
                    , Convert.ToBoolean(blnIsIAF)  // RDBJ 12/21/2021
                    , Convert.ToBoolean(blnIsNeedToDelete)  // JSL 07/02/2022
                    );
            }

            return response;
        }
        //End RDBJ 12/17/2021

        // RDBJ 02/17/2022
        [HttpPost]
        public Dictionary<string, string> CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            DeficiencyHelper _helper = new DeficiencyHelper();
            dicRetMetadata = _helper.CheckCorrectiveActionAddedInDeficiencyByRightClickContextMenu(dicMetadata);
            return dicRetMetadata;
        }
        // End RDBJ 02/17/2022
        
        // RDBJ 02/18/2022
        [HttpPost]
        public Dictionary<string, string> UpdateCorrectiveAction(Dictionary<string, string> dicMetadata)
        {
            Dictionary<string, string> dicRetMetadata = new Dictionary<string, string>();
            DeficiencyHelper _helper = new DeficiencyHelper();
            dicRetMetadata = _helper.UpdateCorrectiveAction(dicMetadata);
            return dicRetMetadata;
        }
        // End RDBJ 02/18/2022

        #region ShipApplication
        [HttpGet]
        public List<Deficiency_GISI_Ships> GetShipDeficincyGrid(string id)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Deficiency_GISI_Ships> list = _helper.GetShipDeficincyGrid(id);
            return list;
        }
        [HttpGet]
        public List<Deficiency_Audit_Ships> GetAuditShipsDeficincyGrid(string id)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            List<Deficiency_Audit_Ships> list = _helper.GetAuditShipsDeficincyGrid(id);
            return list;
        }
        #endregion

        #region Common Functions
        // JSL 05/10/2022
        [HttpPost]
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            DeficiencyHelper _helper = new DeficiencyHelper();
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _helper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End JSL 05/10/2022
        #endregion
    }
}
