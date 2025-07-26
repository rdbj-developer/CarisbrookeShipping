using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace ShipApplication.BLL.Helpers
{
    public class FormTableHelper
    {
        #region CreateDatabase
        public bool InsertFormsDataInLocalDB(List<FormModal> AllForms)
        {
            bool res = false;
            try
            {
                List<FormModal> Forms = FilterForms(AllForms);
                if (Forms != null && Forms.Count > 0)
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.Forms);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.Forms); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(Forms, new List<string> { "IsFolder" });
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.Forms;
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
                LogHelper.writelog("Insert Forms Data In LocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public List<FormModal> GetFormsCategories(string FilePath, string FolderType)
        {
            XmlDocument doc = new XmlDocument();
            List<FormModal> ModalList = new List<FormModal>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("Form");
            string nodeText = "", ext = "";
            FormModal Modal = null;
            string FormID = "";
            Guid newGuid;
            foreach (XmlNode node in nodesList)
            {
                Modal = new FormModal();
                FormID = node.Attributes["FormId"].InnerText;
                newGuid = Guid.Parse(Convert.ToString(FormID));
                Modal.FormID = newGuid;
                Modal.IsDeleted = false;
                Modal.UploadType = AppStatic.NEW;
                Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.Version = 1.0;
                Modal.DocumentVersion = 1.0;
                foreach (XmlNode chldNode in node.ChildNodes)
                {
                    nodeText = Convert.ToString(chldNode.InnerText).Trim();
                    switch (chldNode.Name)
                    {
                        case "TemplatePath":
                            Modal.TemplatePath = nodeText;
                            break;
                        case "Code":
                            Modal.Code = nodeText;
                            break;
                        case "Title":
                            Modal.Title = nodeText;
                            break;
                        case "Issue":
                            Modal.Issue = Utility.ToInteger(nodeText);
                            break;
                        case "IssueDate":
                            if (nodeText != "0001-01-01T00:00:00")
                                Modal.IssueDate = Utility.ToDateTime(nodeText);
                            break;
                        case "Amendment":
                            Modal.Amendment = Utility.ToInteger(nodeText);
                            break;
                        case "AmendmentDate":
                            if (nodeText != "0001-01-01T00:00:00")
                                Modal.AmendmentDate = Utility.ToDateTime(nodeText);
                            break;
                        case "Department":
                            Modal.Department = nodeText;
                            break;
                        case "Category":
                            Modal.Category = nodeText;
                            break;
                        case "AccessLevel":
                            Modal.AccessLevel = nodeText;
                            break;
                        case "AllowsNetworkAccess":
                            Modal.AllowsNetworkAccess = Utility.ToBoolean(nodeText);
                            break;
                        case "CanBeOpened":
                            Modal.CanBeOpened = Utility.ToBoolean(nodeText);
                            break;
                        case "HasSavedData":
                            Modal.HasSavedData = Utility.ToBoolean(nodeText);
                            break;
                        default:
                            break;
                    }
                }
                ext = System.IO.Path.GetExtension(Modal.TemplatePath);
                Modal.Type = ext.Replace(".", "").ToUpper();
                Modal.FolderType = FolderType;
                ModalList.Add(Modal);
            }
            return ModalList;
        }
        public List<FormModal> GetInfoPathFormCategories(string FilePath, string FolderType)
        {
            XmlDocument doc = new XmlDocument();
            List<FormModal> ModalList = new List<FormModal>();
            doc.Load(FilePath);
            XmlNodeList nodesList = doc.DocumentElement.SelectNodes("InfoPathForm");
            string nodeText = "";
            FormModal Modal = null;
            string FormID = "", ext = "";
            Guid newGuid;
            foreach (XmlNode node in nodesList)
            {
                Modal = new FormModal();
                FormID = node.Attributes["FormId"].InnerText;
                newGuid = Guid.Parse(Convert.ToString(FormID));
                Modal.FormID = newGuid;
                Modal.IsDeleted = false;
                Modal.UploadType = AppStatic.NEW;
                Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                Modal.Version = 1.0;
                Modal.DocumentVersion = 1.0;
                foreach (XmlNode chldNode in node.ChildNodes)
                {
                    nodeText = Convert.ToString(chldNode.InnerText).Trim();
                    switch (chldNode.Name)
                    {
                        case "TemplatePath":
                            Modal.TemplatePath = nodeText;
                            break;
                        case "Code":
                            Modal.Code = nodeText;
                            break;
                        case "Title":
                            Modal.Title = nodeText;
                            break;
                        case "Issue":
                            Modal.Issue = Utility.ToInteger(nodeText);
                            break;
                        case "IssueDate":
                            if (nodeText != "0001-01-01T00:00:00")
                                Modal.IssueDate = Utility.ToDateTime(nodeText);
                            break;
                        case "Amendment":
                            Modal.Amendment = Utility.ToInteger(nodeText);
                            break;
                        case "AmendmentDate":
                            if (nodeText != "0001-01-01T00:00:00")
                                Modal.AmendmentDate = Utility.ToDateTime(nodeText);
                            break;
                        case "Department":
                            Modal.Department = nodeText;
                            break;
                        case "Category":
                            Modal.Category = nodeText;
                            break;
                        case "AccessLevel":
                            Modal.AccessLevel = nodeText;
                            break;
                        case "AllowsNetworkAccess":
                            Modal.AllowsNetworkAccess = Utility.ToBoolean(nodeText);
                            break;
                        case "CanBeOpened":
                            Modal.CanBeOpened = Utility.ToBoolean(nodeText);
                            break;
                        case "HasSavedData":
                            Modal.HasSavedData = Utility.ToBoolean(nodeText);
                            break;
                        case "IsURNBased":
                            Modal.IsURNBased = Utility.ToBoolean(nodeText);
                            break;
                        case "URN":
                            Modal.URN = nodeText;
                            break;
                        default:
                            break;
                    }
                }
                ext = System.IO.Path.GetExtension(Modal.TemplatePath);
                Modal.Type = ext.Replace(".", "").ToUpper();
                Modal.FolderType = FolderType;
                ModalList.Add(Modal);
            }
            return ModalList;
        }
        #endregion

        #region Application Use
        public List<FormModal> GetAllFormsFromLocalDB()
        {
            List<FormModal> DocList = new List<FormModal>();
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
                        string Query = "SELECT * FROM " + AppStatic.Forms + " WHERE IsDeleted = 0";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DocList = dt.ToListof<FormModal>();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllFormsFromLocalDB " + ex.Message);
            }
            return DocList;
        }
        public FormModal GetFormBYID(string id)
        {
            FormModal response = new FormModal();
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {

                        DataTable dt = new DataTable();
                        string Query = "SELECT * FROM " + AppStatic.Forms + " WHERE FormID = @FormID";
                        SqlCommand command = new SqlCommand(Query, conn);
                        conn.Open();
                        command.Parameters.Add("@FormID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(id);
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        da.Fill(dt);
                        conn.Close();
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            DataRow dr = dt.Rows[0];
                            response.TemplatePath = dr["TemplatePath"].ToString();
                            response.Type = dr["Type"].ToString();
                            response.ID = Utility.ToInteger(dr["ID"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetFormBYID Error : " + ex.Message);
            }
            return response;
        }
        #endregion

        public List<FormModal> FilterForms(List<FormModal> Forms)
        {
            try
            {
                List<FormModal> FilteredForms = new List<FormModal>();
                foreach (var item in Forms)
                {
                    FormModal dbDoc = GetFormBYID(Utility.ToString(item.FormID));
                    if (dbDoc != null && dbDoc.ID > 0)
                    {

                    }
                    else
                    {
                        FilteredForms.Add(item);
                    }
                }
                return FilteredForms;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterForms Error : " + ex.Message);
                return null;
            }
        }

        // RDBJ2 04/01/2022
        public Dictionary<string, string> AjaxPostPerformAction(Dictionary<string, string> dictMetaData)
        {
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);

            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;
            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                // RDBJ 04/18/2022
                case AppStatic.API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS:
                    {
                        try
                        {
                            string strUniqueFormID = string.Empty;
                            string strShip = string.Empty;
                            string strFormType = string.Empty;

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                strUniqueFormID = dictMetaData["UniqueFormID"].ToString();

                            if (dictMetaData.ContainsKey("Ship"))
                                strShip = dictMetaData["Ship"].ToString();

                            if (dictMetaData.ContainsKey("FormType"))
                                strFormType = dictMetaData["FormType"].ToString();

                            UpdateDeficienciesShipWhenChangeShipInForms(strUniqueFormID, strFormType, strShip, ref retDictMetaData);

                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_UPDATEDEFICIENCIESSHIPWHENCHANGESHIPINFORMS + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End RDBJ 04/18/2022
                case AppStatic.API_DELETESIRNOTEORSIRADDITIONALNOTE:
                    {
                        try
                        {
                            Guid NotesUniqueID = Guid.Empty;
                            Guid UniqueFormID = Guid.Empty;
                            bool IsSIRAdditionalNote = false;

                            if (dictMetaData.ContainsKey("NotesUniqueID"))
                                NotesUniqueID = Guid.Parse(dictMetaData["NotesUniqueID"].ToString());

                            if (dictMetaData.ContainsKey("UniqueFormID"))
                                UniqueFormID = Guid.Parse(dictMetaData["UniqueFormID"].ToString());

                            if (dictMetaData.ContainsKey("IsSIRAdditionalNote"))
                                IsSIRAdditionalNote = Convert.ToBoolean(dictMetaData["IsSIRAdditionalNote"].ToString());

                            if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                            {
                                string strQuery = string.Empty;
                                string strTableName = string.Empty;
                                
                                if (IsSIRAdditionalNote)
                                    strTableName = AppStatic.SIRAdditionalNotes;
                                else
                                    strTableName = AppStatic.SIRNotes;

                                strQuery = "UPDATE " + strTableName + " SET [IsDeleted] = @IsDeleted WHERE NotesUniqueID = @NotesUniqueID";

                                using (var conn = new SqlConnection(connetionString))
                                {
                                    if (conn.IsAvailable())
                                    {
                                        using (SqlCommand cmd = new SqlCommand(strQuery, conn))
                                        {
                                            cmd.CommandType = CommandType.Text;
                                            cmd.Parameters.Add("@IsDeleted", SqlDbType.Int).Value = 1;
                                            cmd.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = NotesUniqueID;
                                            conn.Open();
                                            int rowsAffected = cmd.ExecuteNonQuery();
                                            conn.Close();
                                        }
                                    }
                                }

                                APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();
                                retDictMetaData["FormVersion"] = _defhelper.UpdateGIRSyncStatus_Local_DB(Convert.ToString(UniqueFormID), AppStatic.SIRForm);

                                retDictMetaData["NotesUniqueID"] = NotesUniqueID.ToString();
                            }
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_DELETESIRNOTEORSIRADDITIONALNOTE + " Error : " + ex.Message);
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
        // End RDBJ2 04/01/2022

        // RDBJ 04/18/2022
        public static void UpdateDeficienciesShipWhenChangeShipInForms(string strUniqueFormID, string strFormType, string strShip
            , ref Dictionary<string, string> retDictMetaData)
        {
            try
            {
                string strTableName = string.Empty;

                if (strFormType.ToUpper() == AppStatic.IAFForm)
                {
                    strTableName = "AuditNotes";
                }
                else if (strFormType.ToUpper() == AppStatic.GIRForm
                    || strFormType.ToUpper() == AppStatic.SIRForm
                    )
                {
                    strTableName = "GIRDeficiencies";
                }

                var strSQLQuery = "UPDATE [" + strTableName + "] SET [Ship] = '" + strShip + "' WHERE [UniqueFormID] = '" + strUniqueFormID + "'";
                CommonDBLayer.RecordDataSet(strSQLQuery, null, CommandType.Text);

                APIDeficienciesHelper _defhelper = new APIDeficienciesHelper();
                retDictMetaData["FormVersion"] = _defhelper.UpdateGIRSyncStatus_Local_DB(strUniqueFormID, strFormType);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(strFormType + " UpdateDeficienciesShipWhenChangeShipInForms Error : " + ex.Message);
            }
        }
        // End RDBJ 04/18/2022
    }
}
