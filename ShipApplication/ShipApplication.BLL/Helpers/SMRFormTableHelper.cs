using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ShipApplication.BLL.Helpers
{
    public class SMRFormTableHelper
    {
        public bool SaveSMRFormDataInLocalDB(SMRModal Modal, bool synced)
        {
            bool res = false;
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.SMRForm);
                bool isTbaleCreated = true;
                if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.SMRForm); }
                if (isTbaleCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string InsertQury = SMRDataInsertQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(InsertQury, connection);
                        SMRDataInsertCMD(Modal, ref command);
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
                            SaveSMRFormCrewMembersDataInLocalDB(Modal.SMRFormCrewMemberList, databaseID);
                            res = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SMRForm table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string SMRDataInsertQuery()
        {
            string InsertQury = @"INSERT INTO dbo.SMRForm (ShipID,ShipName,ReviewPeriod,Year,DateOfMeeting,
                                Section1,Section2,Section3,Section4,Section5,Section6,
                                Section7,Section7a,Section7b,Section7c,Section7d,Section7e1,Section7e2,Section7e3,Section7f1,Section7f2,Section7g,Section7h,
                                Section8a,Section8b,Section8b1,Section8b2,Section8b3,Section8b4,Section8b5,
                                Section9,Section10,Section11,Section13,IsSynced,CreatedDate,UpdatedDate,
                                Section12a,Section12b,Section12c,Section12d,Section12e,Section12f,Section12g,Section12h,Section12i,Section12j,Section12k)
                                OUTPUT INSERTED.SMRFormID
                                VALUES (@ShipID,@ShipName,@ReviewPeriod,@Year,@DateOfMeeting,
                                @Section1,@Section2,@Section3,@Section4,@Section5,@Section6,
                                @Section7,@Section7a,@Section7b,@Section7c,@Section7d,@Section7e1,@Section7e2,@Section7e3,@Section7f1,@Section7f2,@Section7g,@Section7h,
                                @Section8a,@Section8b,@Section8b1,@Section8b2,@Section8b3,@Section8b4,@Section8b5,
                                @Section9,@Section10,@Section11,@Section13,@IsSynced,@CreatedDate,@UpdatedDate,
                                @Section12a,@Section12b,@Section12c,@Section12d,@Section12e,@Section12f,@Section12g,@Section12h,@Section12i,@Section12j,@Section12k)";
            return InsertQury;
        }
        public void SMRDataInsertCMD(SMRModal Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = 1;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName;
            command.Parameters.Add("@ReviewPeriod", SqlDbType.NVarChar).Value = Modal.ReviewPeriod;
            command.Parameters.Add("@Year", SqlDbType.Int).Value = Modal.Year;
            command.Parameters.Add("@DateOfMeeting", SqlDbType.NVarChar).Value = Utility.DateToString(Modal.DateOfMeeting);
            command.Parameters.Add("@Section1", SqlDbType.NVarChar).Value = Modal.Section1;
            command.Parameters.Add("@Section2", SqlDbType.NVarChar).Value = Modal.Section2;
            command.Parameters.Add("@Section3", SqlDbType.NVarChar).Value = Modal.Section3;
            command.Parameters.Add("@Section4", SqlDbType.NVarChar).Value = Modal.Section4;
            command.Parameters.Add("@Section5", SqlDbType.NVarChar).Value = Modal.Section5;
            command.Parameters.Add("@Section6", SqlDbType.NVarChar).Value = Modal.Section6;
            command.Parameters.Add("@Section7", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@Section7a", SqlDbType.NVarChar).Value = Modal.Section7a;
            command.Parameters.Add("@Section7b", SqlDbType.NVarChar).Value = Modal.Section7b;
            command.Parameters.Add("@Section7c", SqlDbType.NVarChar).Value = Modal.Section7c;
            command.Parameters.Add("@Section7d", SqlDbType.NVarChar).Value = Modal.Section7d;
            command.Parameters.Add("@Section7e1", SqlDbType.NVarChar).Value = Modal.Section7e1;
            command.Parameters.Add("@Section7e2", SqlDbType.NVarChar).Value = Modal.Section7e2;
            command.Parameters.Add("@Section7e3", SqlDbType.NVarChar).Value = Modal.Section7e3;
            command.Parameters.Add("@Section7f1", SqlDbType.NVarChar).Value = Modal.Section7f1;
            command.Parameters.Add("@Section7f2", SqlDbType.NVarChar).Value = Modal.Section7f2;
            command.Parameters.Add("@Section7g", SqlDbType.NVarChar).Value = Modal.Section7g;
            command.Parameters.Add("@Section7h", SqlDbType.NVarChar).Value = Modal.Section7h;
            command.Parameters.Add("@Section8a", SqlDbType.NVarChar).Value = Modal.Section8a;
            command.Parameters.Add("@Section8b", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@Section8b1", SqlDbType.NVarChar).Value = Modal.Section8b1;
            command.Parameters.Add("@Section8b2", SqlDbType.NVarChar).Value = Modal.Section8b2;
            command.Parameters.Add("@Section8b3", SqlDbType.NVarChar).Value = Modal.Section8b3;
            command.Parameters.Add("@Section8b4", SqlDbType.NVarChar).Value = Modal.Section8b4;
            command.Parameters.Add("@Section8b5", SqlDbType.NVarChar).Value = Modal.Section8b5;
            command.Parameters.Add("@Section9", SqlDbType.NVarChar).Value = Modal.Section9;
            command.Parameters.Add("@Section10", SqlDbType.NVarChar).Value = Modal.Section10;
            command.Parameters.Add("@Section11", SqlDbType.NVarChar).Value = Modal.Section11;
            command.Parameters.Add("@Section12a", SqlDbType.NVarChar).Value = Modal.Section12a == null ? string.Empty : Modal.Section12a;
            command.Parameters.Add("@Section12b", SqlDbType.NVarChar).Value = Modal.Section12b == null ? string.Empty : Modal.Section12b;
            command.Parameters.Add("@Section12c", SqlDbType.NVarChar).Value = Modal.Section12c == null ? string.Empty : Modal.Section12c;
            command.Parameters.Add("@Section12d", SqlDbType.NVarChar).Value = Modal.Section12d == null ? string.Empty : Modal.Section12d;
            command.Parameters.Add("@Section12e", SqlDbType.NVarChar).Value = Modal.Section12e == null ? string.Empty : Modal.Section12e;
            command.Parameters.Add("@Section12f", SqlDbType.NVarChar).Value = Modal.Section12f == null ? string.Empty : Modal.Section12f;
            command.Parameters.Add("@Section12g", SqlDbType.NVarChar).Value = Modal.Section12g == null ? string.Empty : Modal.Section12g;
            command.Parameters.Add("@Section12h", SqlDbType.NVarChar).Value = Modal.Section12h == null ? string.Empty : Modal.Section12h;
            command.Parameters.Add("@Section12i", SqlDbType.NVarChar).Value = Modal.Section12i == null ? string.Empty : Modal.Section12i;
            command.Parameters.Add("@Section12j", SqlDbType.NVarChar).Value = Modal.Section12j == null ? string.Empty : Modal.Section12j;
            command.Parameters.Add("@Section12k", SqlDbType.NVarChar).Value = Modal.Section12k == null ? string.Empty : Modal.Section12k;
            command.Parameters.Add("@Section13", SqlDbType.NVarChar).Value = Modal.Section13;
            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Modal.UpdatedDate;
        }
        public bool SaveSMRFormCrewMembersDataInLocalDB(List<SMRFormCrewMemberModal> CrewMembers, long SMRFormID)
        {
            bool res = false;
            try
            {
                if (CrewMembers != null && CrewMembers.Count > 0 && SMRFormID > 0)
                {
                    foreach (var item in CrewMembers)
                    {
                        item.SMRFormID = SMRFormID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.SMRFormCrewMembers);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.SMRFormCrewMembers); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(CrewMembers);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.SMRFormCrewMembers;
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
                LogHelper.writelog("Add Local DB in SMRFormCrewMembersData table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }


        public void SyncSMRFormsLocalData()
        {
            List<SMRModal> SyncList = GetSMRFormsUnsyncedData();
            List<long> SuccessIds = new List<long>();
            foreach (var item in SyncList)
            {
                APIHelper _helper = new APIHelper();
                APIResponse res = _helper.SubmitSMR(item);
                if (res != null && res.result == AppStatic.SUCCESS)
                {
                    SuccessIds.Add(item.SMRFormID);
                }
            }
            if (SuccessIds.Count > 0)
            {
                UpdateLocalSMRFormsStatus(SuccessIds);
            }
        }
        public List<SMRModal> GetSMRFormsUnsyncedData()
        {
            List<SMRModal> SyncList = new List<SMRModal>();
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
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SMRForm + " WHERE IsSynced = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            SyncList = dt.ToListof<SMRModal>();
                            foreach (var item in SyncList)
                            {
                                DataTable dtMembers = new DataTable();
                                sqlAdp = new SqlDataAdapter("SELECT * FROM " + AppStatic.SMRFormCrewMembers + " WHERE SMRFormID = " + item.SMRFormID + "", conn);
                                sqlAdp.Fill(dtMembers);
                                item.SMRFormCrewMemberList = dtMembers.ToListof<SMRFormCrewMemberModal>();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetSMRFormsUnsyncedData " + ex.Message);
            }
            return SyncList;
        }
        public void UpdateLocalSMRFormsStatus(List<long> SuccessIds)
        {
            try
            {
                string IdsStr = string.Join(",", SuccessIds);
                string connetionString = Utility.GetLocalDBConnStr(LocalDBHelper.ReadDBConfigJson());
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        string Query = "UPDATE " + AppStatic.SMRForm + " SET IsSynced = 1 WHERE SMRFormID in (" + IdsStr + ")";
                        SqlCommand cmd = new SqlCommand(Query, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("UpdateLocalSMRFormsStatus : " + ex.Message);
            }
        }
    }
}
