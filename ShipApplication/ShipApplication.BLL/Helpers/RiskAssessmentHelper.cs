using ShipApplication.BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ShipApplication.BLL.Modals
{
    public class RiskAssessmentHelper
    {
        public bool SaveRAFormDataInLocalDB(RiskAssessmentFormModal Modal, bool synced)
        {
            bool res = false;
            bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentForm);
            bool isTbaleCreated = true;
            if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentForm); }
            if (isTbaleCreated)
            {
                try
                {
                    bool isColumnExist = LocalDBHelper.CheckTableColumnExist(AppStatic.RiskAssessmentForm, "IsAmended");
                    if (!isColumnExist)
                    {
                        LocalDBHelper.ExecuteQuery("ALTER TABLE RiskAssessmentForm ADD IsAmended bit");
                    }
                    bool isColumnIsApplicableExist = LocalDBHelper.CheckTableColumnExist(AppStatic.RiskAssessmentForm, "IsApplicable");
                    if (!isColumnIsApplicableExist)
                    {
                        LocalDBHelper.ExecuteQuery("ALTER TABLE RiskAssessmentForm ADD IsApplicable bit");
                    }
                }
                catch (Exception)
                {
                }
                //if (Modal.RiskAssessmentForm.RAFID > 0)   // JSL 11/26/2022 commented
                if (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty) // JSL 11/26/2022
                {
                    //Update Risk Assessment data
                    try
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                            string UpdateQury = string.Empty;
                            string datedata = DateTime.Parse(Modal.RiskAssessmentForm.ReviewerDate.ToString()).ToString("dd/MM/yyyy");
                            DateTime ReviewDate = DateTime.ParseExact(datedata, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            UpdateQury = "UPDATE " + AppStatic.RiskAssessmentForm + " SET Number = @Number, Title = @Title , ReviewerName = @ReviewerName, ReviewerLocation = @ReviewerLocation, ReviewerRank = @ReviewerRank " +
                                ", ReviewerDate = @ReviewerDate,AmendmentDate=@AmendmentDate,IsAmended=@IsAmended, IsApplicable=@IsApplicable, IsSynced=@IsSynced " +
                                ", UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate " +
                                //"WHERE RAFID = " + Modal.RiskAssessmentForm.RAFID;    // JSL 11/26/2022 commented
                                " WHERE RAFUniqueID = '" + Modal.RiskAssessmentForm.RAFUniqueID + "'";   // JSL 11/26/2022

                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(UpdateQury, connection);
                            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = Modal.RiskAssessmentForm.Number ?? (object)DBNull.Value;
                            command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Modal.RiskAssessmentForm.Title ?? (object)DBNull.Value;
                            command.Parameters.Add("@ReviewerName", SqlDbType.NVarChar).Value = Modal.RiskAssessmentForm.ReviewerName ?? (object)DBNull.Value;
                            command.Parameters.Add("@ReviewerDate", SqlDbType.DateTime).Value = ReviewDate;
                            command.Parameters.Add("@ReviewerRank", SqlDbType.NVarChar).Value = Modal.RiskAssessmentForm.ReviewerRank ?? (object)DBNull.Value;
                            command.Parameters.Add("@ReviewerLocation", SqlDbType.NVarChar).Value = Modal.RiskAssessmentForm.ReviewerLocation ?? (object)DBNull.Value;
                            command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = Modal.RiskAssessmentForm.UpdatedBy ?? (object)DBNull.Value;
                            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.RiskAssessmentForm.UpdatedDate ?? (object)DBNull.Value;    // JSL 11/26/2022 set DateTime
                            command.Parameters.Add("@AmendmentDate", SqlDbType.Date).Value = Modal.RiskAssessmentForm.AmendmentDate ?? (object)DBNull.Value;
                            command.Parameters.Add("@IsAmended", SqlDbType.Bit).Value = Modal.RiskAssessmentForm.IsAmended ?? (object)DBNull.Value;
                            command.Parameters.Add("@IsApplicable", SqlDbType.Bit).Value = Modal.RiskAssessmentForm.IsApplicable ?? (object)DBNull.Value;
                            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.RiskAssessmentForm.IsSynced ?? (object)DBNull.Value;
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();

                            string UpdateQury1 = string.Empty;
                            //UpdateQury1 = "Delete from " + AppStatic.RiskAssessmentFormHazard + " WHERE RAFID = " + Modal.RiskAssessmentForm.RAFID; // JSL 11/26/2022 commented
                            UpdateQury1 = "Delete from " + AppStatic.RiskAssessmentFormHazard + " WHERE RAFUniqueID = '" + Modal.RiskAssessmentForm.RAFUniqueID + "'"; // JSL 11/26/2022
                            SqlConnection connection1 = new SqlConnection(connetionString);
                            SqlCommand command1 = new SqlCommand(UpdateQury1, connection1);
                            connection1.Open();
                            command1.ExecuteNonQuery();
                            connection1.Close();

                            string UpdateQury2 = string.Empty;
                            //UpdateQury2 = "Delete from " + AppStatic.RiskAssessmentFormReviewer + " WHERE RAFID = " + Modal.RiskAssessmentForm.RAFID;   // JSL 11/26/2022 commented
                            UpdateQury2 = "Delete from " + AppStatic.RiskAssessmentFormReviewer + " WHERE RAFUniqueID = '" + Modal.RiskAssessmentForm.RAFUniqueID + "'";   // JSL 11/26/2022
                            SqlConnection connection2 = new SqlConnection(connetionString);
                            SqlCommand command2 = new SqlCommand(UpdateQury2, connection2);
                            connection2.Open();
                            command2.ExecuteNonQuery();
                            connection2.Close();

                            //if (Modal.RiskAssessmentForm.RAFID > 0)   // JSL 11/26/2022 commented
                            if (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty) // JSL 11/26/2022
                            {
                                SaveRAFHazardDataInLocalDB(Modal.RiskAssessmentFormHazardList, Modal.RiskAssessmentForm.RAFID
                                    , Modal.RiskAssessmentForm.RAFUniqueID);    // JSL 11/26/2022
                                SaveRAFReviewerDataInLocalDB(Modal.RiskAssessmentFormReviewerList, Modal.RiskAssessmentForm.RAFID
                                    , Modal.RiskAssessmentForm.RAFUniqueID);    // JSL 11/26/2022
                                //  SaveRAFDoumentsDataInLocalDB(Modal.RiskAssessmentForm, Modal.RiskAssessmentForm.RAFID);
                                res = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {
                    try
                    {
                        List<DocumentModal> DocList = new List<DocumentModal>();

                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                            Guid parentID = new Guid();
                            //Document data get
                            using (var conn = new SqlConnection(connetionString))
                            {
                                if (conn.IsAvailable())
                                {
                                    LogHelper.writelog("IsAvailable");
                                    conn.Open();
                                    DataTable dt = new DataTable();
                                    string Query = "SELECT * FROM " + AppStatic.Documents + " WHERE IsDeleted = 0 and Title ='" + Modal.RiskAssessmentForm.Title + "'";
                                    SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                                    sqlAdp.Fill(dt);
                                    if (dt.Rows.Count == 0)
                                    {
                                        DocList = dt.ToListof<DocumentModal>();
                                        if (DocList.Count >= 0)
                                        {
                                            var number = Char.IsLetterOrDigit(Modal.RiskAssessmentForm.Number, 1);
                                            if (number == false)
                                            {
                                                DocumentModal parentId = new DocumentModal();
                                                // Guid parentID = new Guid();
                                                DataTable dt2 = new DataTable();
                                                string Query2 = "SELECT * FROM " + AppStatic.Documents + " WHERE IsDeleted = 0 and substring(Title,6,1) ='" + Modal.RiskAssessmentForm.Title.Substring(0, 1) + "' and Title Like '%[0-9]%'";
                                                SqlDataAdapter sqlAdp1 = new SqlDataAdapter(Query2, conn);
                                                sqlAdp1.Fill(dt2);
                                                if (dt2 != null && dt2.Rows.Count > 0)
                                                {
                                                    foreach (DataRow item in dt2.Rows)
                                                    {
                                                        var data = item[5].ToString().Substring(1, 1);
                                                        if (data != "0")
                                                        {
                                                            DataTable dt3 = new DataTable();
                                                            string Query3 = "SELECT * FROM " + AppStatic.Documents + " WHERE DocID =" + item[0];
                                                            SqlDataAdapter sqlAdp3 = new SqlDataAdapter(Query3, conn);
                                                            sqlAdp3.Fill(dt3);
                                                            if (dt3 != null && dt3.Rows.Count > 0)
                                                            {
                                                                parentID = (Guid)dt3.Rows[0]["DocumentID"];
                                                            }
                                                        }
                                                        else
                                                        {
                                                            DataTable dt5 = new DataTable();
                                                            string Query5 = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " WHERE substring(Title,6,1) ='" + Modal.RiskAssessmentForm.Title.Substring(0, 1) + "' and Title Like '%[0-9]%'";
                                                            SqlDataAdapter sqlAdp5 = new SqlDataAdapter(Query5, conn);
                                                            sqlAdp5.Fill(dt5);
                                                            if (dt5 != null && dt5.Rows.Count > 0)
                                                            {
                                                                parentID = (Guid)dt5.Rows[0]["DocumentID"];
                                                            }
                                                        }
                                                    }
                                                    Modal.RiskAssessmentForm.Type = "";
                                                }
                                            }
                                            else
                                            {
                                                // Guid parentIDs = new Guid();
                                                DataTable dt4 = new DataTable();
                                                string Query4 = "SELECT * FROM " + AppStatic.Documents + " WHERE IsDeleted = 0 and Title = 'Risk Assessments'";
                                                SqlDataAdapter sqlAdp4 = new SqlDataAdapter(Query4, conn);
                                                sqlAdp4.Fill(dt4);
                                                if (dt4 != null && dt4.Rows.Count > 0)
                                                {
                                                    parentID = (Guid)dt4.Rows[0]["DocumentID"];
                                                }
                                                //Modal.RiskAssessmentForm.Title = Modal.RiskAssessmentForm.Number + " - " + Modal.RiskAssessmentForm.Title;
                                                //Modal.RiskAssessmentForm.Type = "FOLDER";
                                            }
                                        }
                                    }
                                    //conn.Close();
                                }
                            }

                            // JSL 11/26/2022
                            Modal.RiskAssessmentForm.RAFUniqueID = Guid.NewGuid();
                            // End JSL 11/26/2022

                            string InsertQury = RADataInsertQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            RAFDataInsertCMD(Modal, ref command, parentID);
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
                                SaveRAFHazardDataInLocalDB(Modal.RiskAssessmentFormHazardList, databaseID
                                    , Modal.RiskAssessmentForm.RAFUniqueID);    // JSL 11/26/2022
                                SaveRAFReviewerDataInLocalDB(Modal.RiskAssessmentFormReviewerList, databaseID
                                    , Modal.RiskAssessmentForm.RAFUniqueID);    // JSL 11/26/2022
                                // SaveRAFDoumentsDataInLocalDB(Modal.RiskAssessmentForm, databaseID);
                                res = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.writelog("Add Local DB in RiskAssessmentForm table Error : " + ex.Message.ToString() + " StackTrace : " + ex.StackTrace);
                        res = false;
                    }
                }
            }
            return res;
        }
        public string RADataInsertQuery()
        {
            // JSL 11/26/2022 added RAFUniqueID
            string InsertQury = @"INSERT INTO [dbo].[RiskAssessmentForm]
                                ([RAFUniqueID],[ShipName],[ShipCode],[Number],[Title],[ReviewerName],[ReviewerDate],[ReviewerRank],[ReviewerLocation],[CreatedBy]
                                ,[CreatedDate],[UpdatedBy],[UpdatedDate],[SavedAsDraft],[IsSynced],[DocumentID],[ParentID],[SectionType],[AmendmentDate],[IsAmended],[IsApplicable])
		                        OUTPUT INSERTED.RAFID
                                VALUES(@RAFUniqueID,@ShipName,@ShipCode,@Number,@Title,@ReviewerName,@ReviewerDate,@ReviewerRank,@ReviewerLocation,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@SavedAsDraft,@IsSynced,@DocumentID, @ParentID, @SectionType,@AmendmentDate,@IsAmended,@IsApplicable)";
            return InsertQury;
        }

        public void RAFDataInsertCMD(RiskAssessmentFormModal ParentModal, ref SqlCommand command, Guid parentId)
        {
            var Modal = ParentModal.RiskAssessmentForm;
            command.Parameters.Add("@RAFUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.RAFUniqueID ?? (object)DBNull.Value;   // JSL 11/26/2022
            command.Parameters.Add("@ShipCode", SqlDbType.NVarChar).Value = Modal.ShipCode ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName ?? (object)DBNull.Value;
            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = Modal.Number ?? (object)DBNull.Value;
            command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Modal.Title ?? (object)DBNull.Value;
            command.Parameters.Add("@ReviewerName", SqlDbType.NVarChar).Value = Modal.ReviewerName ?? (object)DBNull.Value;
            command.Parameters.Add("@ReviewerDate", SqlDbType.Date).Value = Modal.ReviewerDate ?? (object)DBNull.Value;
            command.Parameters.Add("@ReviewerRank", SqlDbType.NVarChar).Value = Modal.ReviewerRank ?? (object)DBNull.Value;
            command.Parameters.Add("@ReviewerLocation", SqlDbType.NVarChar).Value = Modal.ReviewerLocation ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = Modal.CreatedBy ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate ?? (object)DBNull.Value;
            command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar).Value = Modal.UpdatedBy ?? (object)DBNull.Value;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate ?? (object)DBNull.Value;   // JSL 11/26/2022 set dateTime
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft ?? (object)DBNull.Value;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced ?? (object)DBNull.Value;
            command.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
            command.Parameters.Add("@ParentID", SqlDbType.UniqueIdentifier).Value = parentId;
            command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Modal.Type ?? (object)DBNull.Value;
            command.Parameters.Add("@SectionType", SqlDbType.NVarChar).Value = "ISM" ?? (object)DBNull.Value;
            command.Parameters.Add("@AmendmentDate", SqlDbType.Date).Value = Modal.AmendmentDate ?? (object)DBNull.Value;
            command.Parameters.Add("@IsAmended", SqlDbType.Bit).Value = Modal.IsAmended ?? (object)DBNull.Value;
            command.Parameters.Add("@IsApplicable", SqlDbType.Bit).Value = Modal.IsApplicable ?? (object)DBNull.Value;
        }

        public bool SaveRAFHazardDataInLocalDB(List<RiskAssessmentFormHazard> Records, long RAFID
            , Guid? RAFUniqueID // JSL 11/26/2022
            )
        {
            bool res = false;
            try
            {
                if (Records != null && Records.Count > 0 && RAFID > 0)
                {
                    foreach (var item in Records)
                    {
                        item.RAFID = RAFID;
                        item.Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;
                        item.RAFUniqueID = RAFUniqueID; // JSL 11/26/2022
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentFormHazard);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentFormHazard); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(Records);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.RiskAssessmentFormHazard;
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
                LogHelper.writelog("Add Local DB in RiskAssessmentFormHazard table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        public bool SaveRAFReviewerDataInLocalDB(List<RiskAssessmentFormReviewer> Records, long RAFID
            , Guid? RAFUniqueID // JSL 11/26/2022
            )
        {
            bool res = false;
            try
            {
                if (Records != null && Records.Count > 0 && RAFID > 0)
                {
                    foreach (var item in Records)
                    {
                        item.RAFID = RAFID;
                        item.Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;
                        item.RAFUniqueID = RAFUniqueID; // JSL 11/26/2022
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentFormReviewer);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentFormReviewer); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(Records);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.RiskAssessmentFormReviewer;
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
                LogHelper.writelog("Add Local DB in RiskAssessmentFormReviewer table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        public List<RiskAssessmentForm> GET_RiskAssessment_Local_DB(string shipCode)
        {
            List<RiskAssessmentForm> RAForm = new List<RiskAssessmentForm>();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            string selectQuery = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " WHERE ShipName ='" + shipCode + "'";
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                List<RiskAssessmentForm> SyncList = dt.ToListof<RiskAssessmentForm>();
                                foreach (var item in SyncList)
                                {
                                    RiskAssessmentForm Modal = new RiskAssessmentForm();
                                    Modal.ShipCode = item.ShipCode;
                                    Modal.ShipName = item.ShipName;
                                    Modal.Number = item.Number;
                                    Modal.Title = item.Title;
                                    Modal.ReviewerName = item.ReviewerName;
                                    Modal.ReviewerDate = item.ReviewerDate;
                                    Modal.ReviewerRank = item.ReviewerRank;
                                    Modal.ReviewerLocation = item.ReviewerLocation;
                                    Modal.RAFID = item.RAFID;
                                    RAForm.Add(Modal);
                                }
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAForm Error : " + ex.Message);
            }
            return RAForm;
        }

        public RiskAssessmentFormModal RAFormDetailsView_Local_DB(
            //string ShipName   // JSL 06/22/2022 commented this line
            string ShipCode // JSL 06/22/2022
            //, int id) // JSL 11/26/2022 commented
            , string id)   // JSL 11/26/2022
        {
            RiskAssessmentFormModal dbModal = new RiskAssessmentFormModal();
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                {
                    string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                    using (var conn = new SqlConnection(connetionString))
                    {
                        if (conn.IsAvailable())
                        {
                            conn.Open();
                            DataTable dt = new DataTable();
                            //string selectQuery = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " WHERE ShipName ='" + ShipName + "' and RAFID =" + id;  // JSL 06/22/2022 commeted this line
                            //string selectQuery = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " WHERE ShipCode ='" + ShipCode + "' and RAFID =" + id;  // JSL 11/26/2022 commented  // JSL 06/22/2022
                            string selectQuery = "SELECT * FROM " + AppStatic.RiskAssessmentForm + " WHERE ShipCode ='" + ShipCode + "' and RAFUniqueID = '" + id + "'";  // JSL 11/26/2022  
                            SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                            sqlAdp.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                var SyncList = dt.ToListof<RiskAssessmentForm>();
                                ////dbModal = SyncList[0];
                                dbModal.RiskAssessmentForm = SyncList.FirstOrDefault();

                                DataTable dtHazaedList = new DataTable();
                                //sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormHazard + " WHERE RAFID = " + dbModal.RiskAssessmentForm.RAFID, conn);    // JSL 11/26/2022 commented
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormHazard + " WHERE RAFUniqueID = '" + dbModal.RiskAssessmentForm.RAFUniqueID + "'", conn);    // JSL 11/26/2022
                                sqlAdp.Fill(dtHazaedList);
                                dbModal.RiskAssessmentFormHazardList = dtHazaedList.ToListof<RiskAssessmentFormHazard>();

                                DataTable dtReviewerList = new DataTable();
                                //sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormReviewer + " WHERE RAFID = " + dbModal.RiskAssessmentForm.RAFID, conn);  // JSL 11/26/2022 commented
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.RiskAssessmentFormReviewer + " WHERE RAFUniqueID = '" + dbModal.RiskAssessmentForm.RAFUniqueID + "'", conn);  // JSL 11/26/2022 
                                sqlAdp.Fill(dtReviewerList);
                                dbModal.RiskAssessmentFormReviewerList = dtReviewerList.ToListof<RiskAssessmentFormReviewer>();
                            }
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RAFormDetailsView_Local_DB Error : " + ex.Message);
            }
            return dbModal;
        }

        public bool SaveRAFDoumentsDataInLocalDB(RiskAssessmentForm Records, long RAFID)
        {
            bool res = false;

            List<DocumentModal> DocList = new List<DocumentModal>();
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
                        string Query = "SELECT * FROM " + AppStatic.Documents + " WHERE IsDeleted = 0 and Title ='" + Records.Title + "'";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            DocList = dt.ToListof<DocumentModal>();
                            if (DocList.Count >= 0)
                            {
                                var number = Char.IsLetterOrDigit(Records.Number, 1);
                                if (number == false)
                                {
                                    DocumentModal parentId = new DocumentModal();
                                    Guid parentID = new Guid();
                                    DataTable dt2 = new DataTable();
                                    string Query2 = "SELECT * FROM " + AppStatic.Documents + " WHERE IsDeleted = 0 and substring(Title,6,1) ='" + Records.Title.Substring(0, 1) + "' and Title Like '%[0-9]%'";
                                    SqlDataAdapter sqlAdp1 = new SqlDataAdapter(Query2, conn);
                                    sqlAdp1.Fill(dt2);
                                    if (dt2 != null && dt2.Rows.Count > 0)
                                    {
                                        foreach (DataRow item in dt2.Rows)
                                        {
                                            var data = item[5].ToString().Substring(1, 1);
                                            if (data != "0")
                                            {
                                                DataTable dt3 = new DataTable();
                                                string Query3 = "SELECT * FROM " + AppStatic.Documents + " WHERE DocID =" + item[0];
                                                SqlDataAdapter sqlAdp3 = new SqlDataAdapter(Query3, conn);
                                                sqlAdp3.Fill(dt3);
                                                if (dt3 != null && dt3.Rows.Count > 0)
                                                {
                                                    parentID = (Guid)dt3.Rows[0]["DocumentID"];
                                                }
                                            }
                                        }
                                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                                        {
                                            string connetionString1 = Utility.GetLocalDBConnStr(dbConnModal);
                                            string InsertQury = DocumentsDataInsertQuery();
                                            SqlConnection connection = new SqlConnection(connetionString);
                                            SqlCommand command = new SqlCommand(InsertQury, connection);
                                            DocumentsDataInsertCMD(Records, ref command, parentID, RAFID);
                                            connection.Open();
                                            object resultObj = command.ExecuteScalar();

                                        }
                                    }
                                }
                                else
                                {
                                    Guid parentIDs = new Guid();
                                    DataTable dt4 = new DataTable();
                                    string Query4 = "SELECT * FROM " + AppStatic.Documents + " WHERE IsDeleted = false  Title = 'Risk Assessments'";
                                    SqlDataAdapter sqlAdp4 = new SqlDataAdapter(Query4, conn);
                                    sqlAdp4.Fill(dt4);
                                    if (dt4 != null && dt4.Rows.Count > 0)
                                    {
                                        parentIDs = (Guid)dt4.Rows[0]["DocumentID"];
                                    }

                                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                                    {
                                        string connetionString1 = Utility.GetLocalDBConnStr(dbConnModal);
                                        string InsertQury = DocumentsDataInsertQuery();
                                        SqlConnection connection = new SqlConnection(connetionString);
                                        SqlCommand command = new SqlCommand(InsertQury, connection);
                                        DocumentsParentDataInsertCMD(Records, ref command, parentIDs, RAFID);
                                        connection.Open();
                                        object resultObj = command.ExecuteScalar();
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                        conn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in Documents table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        public string DocumentsDataInsertQuery()
        {
            string InsertQury = @"INSERT INTO [dbo].[Documents]
                                ([DocumentID],[ParentID],[Number],[DocNo],[Title],[Type],[Path],[IsDeleted],[DownloadPath],[UploadType]
                                ,[DocumentVersion],[Version],[Location],[CreatedDate],[UpdatedDate],[SectionType],[RAFID])
		                        OUTPUT INSERTED.RAFID
                                VALUES(@DocumentID,@ParentID,@Number,@DocNo,@Title,@Type,@Path,@IsDeleted,@DownloadPath,@UploadType,@DocumentVersion,@Version,@Location,@CreatedDate,@UpdatedDate,@SectionType,@RAFID)";
            return InsertQury;
        }

        public void DocumentsDataInsertCMD(RiskAssessmentForm ParentModal, ref SqlCommand command, Guid parentId, long RAFId)
        {
            var Modal = ParentModal;
            command.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
            command.Parameters.Add("@ParentID", SqlDbType.UniqueIdentifier).Value = parentId;
            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = Modal.Number ?? (object)DBNull.Value;
            command.Parameters.Add("@DocNo", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Modal.Title ?? (object)DBNull.Value;
            command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@Path", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = false;
            command.Parameters.Add("@DownloadPath", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@UploadType", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@DocumentVersion", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@Version", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedDate", SqlDbType.Date).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.Date).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@SectionType", SqlDbType.NVarChar).Value = "ISM" ?? (object)DBNull.Value;
            command.Parameters.Add("@RAFID", SqlDbType.Int).Value = RAFId;
            // command.Parameters.Add("@IsWebPage", SqlDbType.Bit).Value = true;
        }

        public void DocumentsParentDataInsertCMD(RiskAssessmentForm ParentModal, ref SqlCommand command, Guid parentId, long RAFId)
        {
            var Modal = ParentModal;
            command.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
            command.Parameters.Add("@ParentID", SqlDbType.UniqueIdentifier).Value = parentId;
            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@DocNo", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Modal.Number + " - " + Modal.Title ?? (object)DBNull.Value;
            command.Parameters.Add("@Type", SqlDbType.NVarChar).Value = "FOLDER" ?? (object)DBNull.Value;
            command.Parameters.Add("@Path", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@IsDeleted", SqlDbType.Bit).Value = false;
            command.Parameters.Add("@DownloadPath", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@UploadType", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@DocumentVersion", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@Version", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@CreatedDate", SqlDbType.Date).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.Date).Value = null ?? (object)DBNull.Value;
            command.Parameters.Add("@SectionType", SqlDbType.NVarChar).Value = "ISM" ?? (object)DBNull.Value;
            command.Parameters.Add("@RAFID", SqlDbType.Int).Value = RAFId;
            //command.Parameters.Add("@IsWebPage", SqlDbType.Bit).Value = true;
        }

        public List<RiskAssessmentReviewLog> GetAllRiskAssessmentReviewLogFromLocalDB(string shipCode = "")
        {
            List<RiskAssessmentReviewLog> DocList = new List<RiskAssessmentReviewLog>();
            string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
            try
            {
                //string Query = "Select R.Number,R.Title,AmendmentDate,Stage4RiskFactor, ISNULL(R.IsApplicable,1) as IsApplicable from RiskAssessmentForm  R " +
                //                      "Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard Where RAFID = R.RAFID Order By Stage4RiskFactor desc )X " +
                //                      "Outer Apply( SElect * from ( Select '01 - General' as Title, 1 as Orders, 'G' as Category Union Select '02 - Deck Department' as Title , 2 as Orders, 'D' as Category " +
                //                      "Union Select '03 - Engine Department' as Title, 3 as Orders, 'E' as Category Union Select '04 - Catering' as Title, 4 as Orders, 'C' as Category Union " +
                //                      "Select '05 - Subcontractors' as Title, 5 as Orders, 'S' as Category ) as Groups Where Groups.Category = SUBSTRING (LTRIM(R.Number),1,1) )Y " +
                //                      " Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log') Order By Orders,Cast(SUBSTRING (LTRIM(Number),2,LEN(Number)) as int)";

                // JSL 11/26/2022 commented
                /*
                string Query = " SELECT R.*,Stage4RiskFactor,Orders " +
" FROM (SELECT R.ShipName, R.ShipCode, R.Number,R.Title,AmendmentDate,ISNULL(R.IsApplicable,1) as IsApplicable, ReviewerDate,R.RAFID,  " +  // JSL 06/22/2022 added R.ShipName, R.ShipCode,
"             row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt  " +
"       FROM RiskAssessmentForm  R Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log')) R	 " +
" 	 Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard " +
" 		Where RAFID = R.RAFID Order By Stage4RiskFactor desc )X  " +
" 	 Outer Apply(Select * from (  " +
"      Select Title,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Orders, SUBSTRING (RTRIM(LTRIM(Title)),6,1) as Category from Documents where ParentID='0FC3BDB8-0D98-4614-81D3-7918D01BFFF2' ) as Groups  " +
                             " 			Where Groups.Category = SUBSTRING (LTRIM(R.Number),1,1) )Y   " +
" WHERE R.cnt = 1 " +
" Order By Orders,Cast(SUBSTRING (LTRIM(Number),2,LEN(Number)) as int); ";
                */
                // End JSL 11/26/2022 commented

                // JSL 11/26/2022
                string Query = "SELECT R.*,Stage4RiskFactor,Orders" +
                    " FROM (SELECT R.ShipName, R.ShipCode, R.Number,R.Title,AmendmentDate,ISNULL(R.IsApplicable,1) as IsApplicable, ReviewerDate," +    // JSL 01/18/2023  added R.ShipName, R.ShipCode,
                    " R.RAFID, R.RAFUniqueID," +
                    " row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt" +
                    " FROM RiskAssessmentForm  R Where ShipCode=@ShipCode " +
                    " AND R.RAFUniqueID IS NOT NULL " + // JSL 01/18/2023
                    " AND R.Title NOT IN('Review Instructions','Review Log')) R" +
                    " Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard" +
                    //"Where RAFID = R.RAFID" +
                    " WHERE RAFUniqueID = R.RAFUniqueID" +
                    " Order By Stage4RiskFactor desc )X" +
                    " Outer Apply(Select * from (" +
                    //" Select Title,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Orders, SUBSTRING (TRIM(Title),6,1) as Category from Documents where ParentID='0FC3BDB8-0D98-4614-81D3-7918D01BFFF2' ) as Groups" + // JSL 12/15/2022 commented
                    " Select Title,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Orders, SUBSTRING (LTRIM(RTRIM(Title)),6,1) as Category from Documents where ParentID='0FC3BDB8-0D98-4614-81D3-7918D01BFFF2' ) as Groups" +   // JSL 12/15/2022 
                    " Where Groups.Category = SUBSTRING (LTRIM(R.Number),1,1) )Y" +
                    " WHERE R.cnt = 1" +
                    //" Order By Orders,Cast(SUBSTRING (LTRIM(Number),2,LEN(Number)) as int);"; // JSL 01/13/2023 commented
                    " Order By Orders;";    // JSL 01/13/2023
                // End JSL 11/26/2022

                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("IsAvailable");
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", shipCode);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DocList = dt.ToListof<RiskAssessmentReviewLog>();
                            }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string Query = "Select R.Number,R.Title,AmendmentDate,Stage4RiskFactor, ISNULL(R.IsApplicable,1) as IsApplicable,R.ReviewerDate,R.RAFID,R.RAFUniqueID,Orders from RiskAssessmentForm  R " +
                                     //"Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard Where RAFID = R.RAFID Order By Stage4RiskFactor desc )X " +    // JSL 11/26/2022 commented
                                     "Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard Where RAFUniqueID = R.RAFUniqueID Order By Stage4RiskFactor desc )X " +  // JSL 11/26/2022
                                     " Outer Apply(Select * from (Select Title,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Orders, SUBSTRING (RTRIM(LTRIM(Title)),6,1) as Category from Documents where ParentID='0FC3BDB8-0D98-4614-81D3-7918D01BFFF2' ) as Groups  " +
                                     " Where Groups.Category = SUBSTRING (LTRIM(R.Number),1,1)) Y " +
                                     " Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log') Order By Orders,Cast(SUBSTRING (LTRIM(Number),2,LEN(Number)) as int)";
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("IsAvailable");
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", shipCode);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DocList = dt.ToListof<RiskAssessmentReviewLog>();
                            }
                        conn.Close();
                    }
                }
                LogHelper.writelog("GetAllRiskAssessmentReviewLogFromLocalDB " + ex.Message);
            }
            return DocList;
        }

        public bool CheckRAFNumberExistFromLocalDB(string RAFNumber)
        {
            bool result = false;
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
                        string Query = "Select * from RiskAssessmentForm  Where Number=@Number ";
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.SelectCommand.Parameters.AddWithValue("@Number", RAFNumber);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            result = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckRAFNumberExistFromLocalDB " + ex.Message);
            }
            return result;
        }
        public bool DeleteLocalRiskAssessmentData(string ShipCode)
        {
            bool result = false;
            try
            {
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                try
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentFormHazard);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentFormHazard); }
                    else
                    {
                        string UpdateQury1 = string.Empty;
                        UpdateQury1 = "Delete from " + AppStatic.RiskAssessmentFormHazard + " Where RAFID IN( Select RAFID From RiskAssessmentForm Where ShipCode = '" + ShipCode + "')";
                        SqlConnection connection1 = new SqlConnection(connetionString);
                        SqlCommand command1 = new SqlCommand(UpdateQury1, connection1);
                        connection1.Open();
                        command1.ExecuteNonQuery();
                        connection1.Close();
                    }
                }
                catch (Exception e)
                {
                    LogHelper.writelog("Delete RiskAssessmentFormHazard Error : " + e.Message);
                }
                try
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentFormReviewer);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentFormReviewer); }
                    else
                    {
                        string UpdateQury2 = string.Empty;
                        UpdateQury2 = "Delete from " + AppStatic.RiskAssessmentFormReviewer + " Where RAFID IN( Select RAFID From RiskAssessmentForm Where ShipCode = '" + ShipCode + "')";
                        SqlConnection connection2 = new SqlConnection(connetionString);
                        SqlCommand command2 = new SqlCommand(UpdateQury2, connection2);
                        connection2.Open();
                        command2.ExecuteNonQuery();
                        connection2.Close();
                    }
                }
                catch (Exception e)
                {
                    LogHelper.writelog("Delete RiskAssessmentFormReviewer Error : " + e.Message);
                }
                try
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.RiskAssessmentForm);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.RiskAssessmentForm); }
                    else
                    {
                        string UpdateQury3 = string.Empty;
                        UpdateQury3 = "Delete from " + AppStatic.RiskAssessmentForm + " Where ShipCode='" + ShipCode + "'";
                        SqlConnection connection3 = new SqlConnection(connetionString);
                        SqlCommand command3 = new SqlCommand(UpdateQury3, connection3);
                        connection3.Open();
                        command3.ExecuteNonQuery();
                        connection3.Close();
                    }
                }
                catch (Exception e)
                {
                    LogHelper.writelog("Delete RiskAssessmentForm Error : " + e.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteLocalRiskAssessmentData " + ex.Message);
            }
            return result;
        }
    }
}
