using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    // JSL 07/16/2022 added this class
    public class CommonMethodsForSyncing
    {
        // JSL 07/16/2022
        public Dictionary<string, string> PerformPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            bool IsAllowToInsertOrUpdateData = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                case AppStatic.API_METHOD_CheckFormVersion:
                    {
                        string strFormType = string.Empty;
                        string strFormUniqueID = string.Empty;
                        Guid guidFormUniqueID = Guid.Empty;
                        string strFormVersion = string.Empty;
                        double decFormVersion = 0.0;

                        try
                        {
                            if (dictMetaData.ContainsKey("FormType"))
                                strFormType = dictMetaData["FormType"];

                            if (dictMetaData.ContainsKey("FormUniqueID"))
                                strFormUniqueID = dictMetaData["FormUniqueID"];

                            if (dictMetaData.ContainsKey("FormVersion"))
                                strFormVersion = dictMetaData["FormVersion"];

                            if (!string.IsNullOrEmpty(strFormUniqueID))
                            {
                                guidFormUniqueID = Guid.Parse(strFormUniqueID);
                                decFormVersion = Convert.ToDouble(strFormVersion);

                                if (strFormType.ToLower() == AppStatic.GIRForm.ToLower())
                                {
                                    // JSL 09/09/2022
                                    Entity.GeneralInspectionReport dbModal = new Entity.GeneralInspectionReport();
                                    dbModal = dbContext.GeneralInspectionReports.Where(x => x.UniqueFormID == guidFormUniqueID).FirstOrDefault();

                                    if (dbModal == null)
                                    {
                                        IsAllowToInsertOrUpdateData = true;
                                    }
                                    else
                                    {
                                        int IsLocalFormVersionLatest = Decimal.Compare((decimal)decFormVersion, (decimal)dbModal.FormVersion);
                                        if (IsLocalFormVersionLatest == 1)
                                        {
                                            IsAllowToInsertOrUpdateData = true;
                                        }
                                    }
                                    // End JSL 09/09/2022
                                }
                                else if (strFormType.ToLower() == AppStatic.SIRForm.ToLower())
                                {
                                    Entity.SuperintendedInspectionReport dbModal = new Entity.SuperintendedInspectionReport();
                                    dbModal = dbContext.SuperintendedInspectionReports.Where(x => x.UniqueFormID == guidFormUniqueID).FirstOrDefault();

                                    if (dbModal == null)
                                    {
                                        IsAllowToInsertOrUpdateData = true;
                                    }
                                    else
                                    {
                                        int IsLocalFormVersionLatest = Decimal.Compare((decimal)decFormVersion, (decimal)dbModal.FormVersion);
                                        if (IsLocalFormVersionLatest == 1)
                                        {
                                            IsAllowToInsertOrUpdateData = true;
                                        }
                                    }
                                }
                                else if (strFormType.ToLower() == AppStatic.IAFForm.ToLower())
                                {
                                    // JSL 09/09/2022
                                    Entity.InternalAuditForm dbModal = new Entity.InternalAuditForm();
                                    dbModal = dbContext.InternalAuditForms.Where(x => x.UniqueFormID == guidFormUniqueID).FirstOrDefault();

                                    if (dbModal == null)
                                    {
                                        IsAllowToInsertOrUpdateData = true;
                                    }
                                    else
                                    {
                                        int IsLocalFormVersionLatest = Decimal.Compare((decimal)decFormVersion, (decimal)dbModal.FormVersion);
                                        if (IsLocalFormVersionLatest == 1)
                                        {
                                            IsAllowToInsertOrUpdateData = true;
                                        }
                                    }
                                    // End JSL 09/09/2022
                                }
                            }
                            retDictMetaData["IsAllowToInsertOrUpdateData"] = IsAllowToInsertOrUpdateData.ToString().ToLower();
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(strFormType + " - " + strFormUniqueID + " : " + AppStatic.API_METHOD_CheckFormVersion + " Error : " + ex.Message);
                        }
                        break;
                    }
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
