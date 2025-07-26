using ShipApplication.BLL.Modals;
using System;
using System.Data;
using System.Data.SqlClient;
namespace ShipApplication.BLL.Helpers
{
    public class FeedbackFormHelper
    {
        public bool SaveFeedbackDataInLocalDB(FeedbackFormModal Modal)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.FeedbackForm);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.FeedbackForm); }
                if (isTbaleCreated)
                {
                    try
                    {
                        bool isColumnExist = LocalDBHelper.CheckTableColumnExist(AppStatic.FeedbackForm, "FeedbackBy");
                        if (!isColumnExist)
                        {
                            LocalDBHelper.ExecuteQuery("ALTER TABLE FeedbackForm ADD FeedbackBy Nvarchar(150)");
                        }
                    }
                    catch (Exception)
                    { 
                    }
                   
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQury = FeedbackFormInsertQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(InsertQury, connection);
                        FeedbackDataInsertCMD(Modal, ref command);
                        connection.Open();
                        object resultObj = command.ExecuteScalar();
                        long databaseID = 0;
                        if (resultObj != null)
                        {
                            long.TryParse(resultObj.ToString(), out databaseID);
                        }
                        connection.Close();
                        if (databaseID > 0)
                        {
                            res = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SaveFeedbackDataInLocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string FeedbackFormInsertQuery()
        {
            string InsertQury = @"INSERT INTO [dbo].[FeedbackForm]([Id],[ShipName],[ShipCode],[Title],[Details],[CreatedDate],[IsMailSent],[IsSynced],[AttachmentPath],[AttachmentFileName],[FeedbackBy])
                                VALUES(@Id,@ShipName,@ShipCode,@Title,@Details,@CreatedDate,@IsMailSent,@IsSynced,@AttachmentPath,@AttachmentFileName,@FeedbackBy)";
            return InsertQury;
        }
        public void FeedbackDataInsertCMD(FeedbackFormModal Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Modal.ShipCode ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName ?? (object)DBNull.Value;
            command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Modal.Title ?? (object)DBNull.Value;
            command.Parameters.Add("@Details", SqlDbType.NVarChar).Value = Modal.Details ?? (object)DBNull.Value;
            command.Parameters.Add("@IsMailSent", SqlDbType.Bit).Value = Modal.IsMailSent ?? (object)DBNull.Value;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate ?? Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@AttachmentPath", SqlDbType.NVarChar).Value = Modal.AttachmentPath ?? (object)DBNull.Value;
            command.Parameters.Add("@AttachmentFileName", SqlDbType.NVarChar).Value = Modal.AttachmentFileName ?? (object)DBNull.Value;
            command.Parameters.Add("@FeedbackBy", SqlDbType.NVarChar).Value = Modal.FeedbackBy ?? (object)DBNull.Value;
        }
    }
}
