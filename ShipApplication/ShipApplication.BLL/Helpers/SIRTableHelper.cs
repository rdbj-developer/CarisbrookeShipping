using ShipApplication.BLL.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ShipApplication.BLL.Helpers
{
    public class SIRTableHelper
    {
        public bool SaveSIRDataInLocalDB(SIRModal modal, bool synced)
        {
            bool res = false;
            string UniqueFormID = string.Empty;
            try
            {
                if (modal.SuperintendedInspectionReport.UniqueFormID != Guid.Empty)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string UpdateQury = GetSIRUpdateQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(UpdateQury, connection);

                        modal.SuperintendedInspectionReport.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime

                        SIRUpdateCMD(modal.SuperintendedInspectionReport, ref command);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        UniqueFormID = modal.SuperintendedInspectionReport.UniqueFormID.ToString();
                    }
                }
                else
                {

                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.SuperintendedInspectionReport);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.SuperintendedInspectionReport); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                            string InsertQury = SIRDataInsertQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            Guid FormGUID = Guid.NewGuid();
                            modal.SuperintendedInspectionReport.UniqueFormID = FormGUID;
                            modal.SuperintendedInspectionReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                            SIRDataInsertCMD(modal.SuperintendedInspectionReport, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            connection.Close();
                        }
                    }
                }
                //RDBJ 10/13/2021
                if (!string.IsNullOrEmpty(UniqueFormID))
                {
                    if (modal.NotesChanged)
                        //SaveSIRNotesDataInLocalDB(modal.SIRNote, modal.SuperintendedInspectionReport.UniqueFormID);   // RDBJ2 04/01/2022 commented this line
                        SIRNotesInsertOrUpdate(modal.SIRNote, modal.SuperintendedInspectionReport.UniqueFormID);    // RDBJ2 04/01/2022

                    if (modal.AdditionalNotesChanged)
                        //SaveSIRAdditionalNotesDataInLocalDB(modal.SIRAdditionalNote, modal.SuperintendedInspectionReport.UniqueFormID);   // RDBJ2 04/01/2022 commented this line
                        SIRAdditionalNotesInsertOrUpdate(modal.SIRAdditionalNote, modal.SuperintendedInspectionReport.UniqueFormID);    // RDBJ2 04/01/2022

                    if (modal.DeficienciesChanged)
                        GIRDeficiencies_Save(modal.SuperintendedInspectionReport.UniqueFormID, modal.GIRDeficiencies);
                }
                //End RDBJ 10/13/2021
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SuperintendedInspectionReport table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
        public string SIRDataInsertQuery()
        {
            // RDBJ 02/15/2022 Added Section9_16_Condition, Section9_16_Comment, Section9_17_Condition, Section9_17_Comment, Section18_8_Condition, Section18_8_Comment, Section18_9_Condition, Section18_9_Comment 
            // RDBJ 01/05/2022 Added isDelete
            string InsertQury = @"INSERT INTO [dbo].[SuperintendedInspectionReport]
                                ([UniqueFormID],[FormVersion],[ShipID],[ShipName],[Date],[Port],[Master],[Superintended],[Section1_1_Condition],[Section1_1_Comment],[Section1_2_Condition]
                                ,[Section1_2_Comment],[Section1_3_Condition],[Section1_3_Comment],[Section1_4_Condition],[Section1_4_Comment],[Section1_5_Condition]
                                ,[Section1_5_Comment],[Section1_6_Condition],[Section1_6_Comment],[Section1_7_Condition],[Section1_7_Comment],[Section1_8_Condition]
                                ,[Section1_8_Comment],[Section1_9_Condition],[Section1_9_Comment],[Section1_10_Condition],[Section1_10_Comment],[Section1_11_Condition]
                                ,[Section1_11_Comment],[Section2_1_Condition],[Section2_1_Comment],[Section2_2_Condition],[Section2_2_Comment],[Section2_3_Condition]
                                ,[Section2_3_Comment],[Section2_4_Condition],[Section2_4_Comment],[Section2_5_Condition],[Section2_5_Comment],[Section2_6_Condition]
                                ,[Section2_6_Comment],[Section2_7_Condition],[Section2_7_Comment],[Section3_1_Condition],[Section3_1_Comment],[Section3_2_Condition]
                                ,[Section3_2_Comment],[Section3_3_Condition],[Section3_3_Comment],[Section3_4_Condition],[Section3_4_Comment],[Section3_5_Condition]
                                ,[Section3_5_Comment],[Section4_1_Condition],[Section4_1_Comment],[Section4_2_Condition],[Section4_2_Comment],[Section4_3_Condition]
                                ,[Section4_3_Comment],[Section5_1_Condition],[Section5_1_Comment],[Section5_6_Condition],[Section5_6_Comment],[Section5_8_Condition]
                                ,[Section5_8_Comment],[Section5_9_Condition],[Section5_9_Comment],[Section6_1_Condition],[Section6_1_Comment],[Section6_2_Condition]
                                ,[Section6_2_Comment],[Section6_3_Condition],[Section6_3_Comment],[Section6_4_Condition],[Section6_4_Comment],[Section6_5_Condition]
                                ,[Section6_5_Comment],[Section6_6_Condition],[Section6_6_Comment],[Section6_7_Condition],[Section6_7_Comment],[Section6_8_Condition]
                                ,[Section6_8_Comment],[Section7_1_Condition],[Section7_1_Comment],[Section7_2_Condition],[Section7_2_Comment],[Section7_3_Condition]
                                ,[Section7_3_Comment],[Section7_4_Condition],[Section7_4_Comment],[Section7_5_Condition],[Section7_5_Comment],[Section7_6_Condition]
                                ,[Section7_6_Comment],[Section8_1_Condition],[Section8_1_Comment],[Section8_2_Condition],[Section8_2_Comment],[Section8_3_Condition]
                                ,[Section8_3_Comment],[Section8_4_Condition],[Section8_4_Comment],[Section8_5_Condition],[Section8_5_Comment],[Section8_6_Condition]
                                ,[Section8_6_Comment],[Section8_7_Condition],[Section8_7_Comment],[Section8_8_Condition],[Section8_8_Comment],[Section8_9_Condition]
                                ,[Section8_9_Comment],[Section8_10_Condition],[Section8_10_Comment],[Section8_11_Condition],[Section8_11_Comment],[Section8_12_Condition]
                                ,[Section8_12_Comment],[Section8_13_Condition],[Section8_13_Comment],[Section8_14_Condition],[Section8_14_Comment],[Section8_15_Condition]
                                ,[Section8_15_Comment],[Section8_16_Condition],[Section8_16_Comment],[Section8_17_Condition],[Section8_17_Comment],[Section8_18_Condition]
                                ,[Section8_18_Comment],[Section8_19_Condition],[Section8_19_Comment],[Section8_20_Condition],[Section8_20_Comment],[Section8_21_Condition]
                                ,[Section8_21_Comment],[Section8_22_Condition],[Section8_22_Comment],[Section8_23_Condition],[Section8_23_Comment],[Section8_24_Condition]
                                ,[Section8_24_Comment],[Section8_25_Condition],[Section8_25_Comment],[Section9_1_Condition],[Section9_1_Comment],[Section9_2_Condition]
                                ,[Section9_2_Comment],[Section9_3_Condition],[Section9_3_Comment],[Section9_4_Condition],[Section9_4_Comment],[Section9_5_Condition]
                                ,[Section9_5_Comment],[Section9_6_Condition],[Section9_6_Comment],[Section9_7_Condition],[Section9_7_Comment],[Section9_8_Condition]
                                ,[Section9_8_Comment],[Section9_9_Condition],[Section9_9_Comment],[Section9_10_Condition],[Section9_10_Comment],[Section9_11_Condition]
                                ,[Section9_11_Comment],[Section9_12_Condition],[Section9_12_Comment],[Section9_13_Condition],[Section9_13_Comment],[Section9_14_Condition]
                                ,[Section9_14_Comment],[Section9_15_Condition],[Section9_15_Comment]
                                ,[Section9_16_Condition],[Section9_16_Comment],[Section9_17_Condition],[Section9_17_Comment]
                                ,[Section10_1_Condition],[Section10_1_Comment],[Section10_2_Condition]
                                ,[Section10_2_Comment],[Section10_3_Condition],[Section10_3_Comment],[Section10_4_Condition],[Section10_4_Comment],[Section10_5_Condition]
                                ,[Section10_5_Comment],[Section10_6_Condition],[Section10_6_Comment],[Section10_7_Condition],[Section10_7_Comment],[Section10_8_Condition]
                                ,[Section10_8_Comment],[Section10_9_Condition],[Section10_9_Comment],[Section10_10_Condition],[Section10_10_Comment],[Section10_11_Condition]
                                ,[Section10_11_Comment],[Section10_12_Condition],[Section10_12_Comment],[Section10_13_Condition],[Section10_13_Comment],[Section10_14_Condition]
                                ,[Section10_14_Comment],[Section10_15_Condition],[Section10_15_Comment],[Section10_16_Condition],[Section10_16_Comment],[Section11_1_Condition]
                                ,[Section11_1_Comment],[Section11_2_Condition],[Section11_2_Comment],[Section11_3_Condition],[Section11_3_Comment],[Section11_4_Condition]
                                ,[Section11_4_Comment],[Section11_5_Condition],[Section11_5_Comment],[Section11_6_Condition],[Section11_6_Comment],[Section11_7_Condition]
                                ,[Section11_7_Comment],[Section11_8_Condition],[Section11_8_Comment],[Section12_1_Condition],[Section12_1_Comment],[Section12_2_Condition]
                                ,[Section12_2_Comment],[Section12_3_Condition],[Section12_3_Comment],[Section12_4_Condition],[Section12_4_Comment],[Section12_5_Condition]
                                ,[Section12_5_Comment],[Section12_6_Condition],[Section12_6_Comment],[Section13_1_Condition],[Section13_1_Comment],[Section13_2_Condition]
                                ,[Section13_2_Comment],[Section13_3_Condition],[Section13_3_Comment],[Section13_4_Condition],[Section13_4_Comment],[Section14_1_Condition]
                                ,[Section14_1_Comment],[Section14_2_Condition],[Section14_2_Comment],[Section14_3_Condition],[Section14_3_Comment],[Section14_4_Condition]
                                ,[Section14_4_Comment],[Section14_5_Condition],[Section14_5_Comment],[Section14_6_Condition],[Section14_6_Comment],[Section14_7_Condition]
                                ,[Section14_7_Comment],[Section14_8_Condition],[Section14_8_Comment],[Section14_9_Condition],[Section14_9_Comment],[Section14_10_Condition]
                                ,[Section14_10_Comment],[Section14_11_Condition],[Section14_11_Comment],[Section14_12_Condition],[Section14_12_Comment],[Section14_13_Condition]
                                ,[Section14_13_Comment],[Section14_14_Condition],[Section14_14_Comment],[Section14_15_Condition],[Section14_15_Comment],[Section14_16_Condition]
                                ,[Section14_16_Comment],[Section14_17_Condition],[Section14_17_Comment],[Section14_18_Condition],[Section14_18_Comment],[Section14_19_Condition]
                                ,[Section14_19_Comment],[Section14_20_Condition],[Section14_20_Comment],[Section14_21_Condition],[Section14_21_Comment],[Section14_22_Condition]
                                ,[Section14_22_Comment],[Section14_23_Condition],[Section14_23_Comment],[Section14_24_Condition],[Section14_24_Comment],[Section14_25_Condition]
                                ,[Section14_25_Comment],[Section15_1_Condition],[Section15_1_Comment],[Section15_2_Condition],[Section15_2_Comment],[Section15_3_Condition],[Section15_3_Comment]
                                ,[Section15_4_Condition],[Section15_4_Comment],[Section15_5_Condition],[Section15_5_Comment],[Section15_6_Condition],[Section15_6_Comment],[Section15_7_Condition]
                                ,[Section15_7_Comment],[Section15_8_Condition],[Section15_8_Comment],[Section15_9_Condition],[Section15_9_Comment],[Section15_10_Condition],[Section15_10_Comment]
                                ,[Section15_11_Condition],[Section15_11_Comment],[Section15_12_Condition],[Section15_12_Comment],[Section15_13_Condition],[Section15_13_Comment],[Section15_14_Condition]
                                ,[Section15_14_Comment],[Section15_15_Condition],[Section15_15_Comment],[Section16_1_Condition],[Section16_1_Comment],[Section16_2_Condition],[Section16_2_Comment]
                                ,[Section16_3_Condition],[Section16_3_Comment],[Section16_4_Condition],[Section16_4_Comment],[Section17_1_Condition],[Section17_1_Comment],[Section17_2_Condition]
                                ,[Section17_2_Comment],[Section17_3_Condition],[Section17_3_Comment],[Section17_4_Condition],[Section17_4_Comment],[Section17_5_Condition],[Section17_5_Comment]
                                ,[Section17_6_Condition],[Section17_6_Comment],[Section18_1_Condition],[Section18_1_Comment],[Section18_2_Condition],[Section18_2_Comment],[Section18_3_Condition]
                                ,[Section18_4_Comment],[Section18_5_Condition],[Section18_5_Comment],[Section18_6_Condition],[Section18_6_Comment],[Section18_7_Condition],[Section18_7_Comment]
                                ,[Section18_8_Condition],[Section18_8_Comment],[Section18_9_Condition],[Section18_9_Comment]                                
                                ,[IsSynced],[CreatedDate],[ModifyDate],[SavedAsDraft], [isDelete])  OUTPUT INSERTED.SIRFormID VALUES
                                (@UniqueFormID,@FormVersion,@ShipID,@ShipName,@Date,@Port,@Master,@Superintended,@Section1_1_Condition,@Section1_1_Comment,@Section1_2_Condition
                                ,@Section1_2_Comment,@Section1_3_Condition,@Section1_3_Comment,@Section1_4_Condition,@Section1_4_Comment,@Section1_5_Condition
                                ,@Section1_5_Comment,@Section1_6_Condition,@Section1_6_Comment,@Section1_7_Condition,@Section1_7_Comment,@Section1_8_Condition
                                ,@Section1_8_Comment,@Section1_9_Condition,@Section1_9_Comment,@Section1_10_Condition,@Section1_10_Comment,@Section1_11_Condition
                                ,@Section1_11_Comment,@Section2_1_Condition,@Section2_1_Comment,@Section2_2_Condition,@Section2_2_Comment,@Section2_3_Condition
                                ,@Section2_3_Comment,@Section2_4_Condition,@Section2_4_Comment,@Section2_5_Condition,@Section2_5_Comment,@Section2_6_Condition
                                ,@Section2_6_Comment,@Section2_7_Condition,@Section2_7_Comment,@Section3_1_Condition,@Section3_1_Comment,@Section3_2_Condition
                                ,@Section3_2_Comment,@Section3_3_Condition,@Section3_3_Comment,@Section3_4_Condition,@Section3_4_Comment,@Section3_5_Condition
                                ,@Section3_5_Comment,@Section4_1_Condition,@Section4_1_Comment,@Section4_2_Condition,@Section4_2_Comment,@Section4_3_Condition
                                ,@Section4_3_Comment,@Section5_1_Condition,@Section5_1_Comment,@Section5_6_Condition,@Section5_6_Comment,@Section5_8_Condition
                                ,@Section5_8_Comment,@Section5_9_Condition,@Section5_9_Comment,@Section6_1_Condition,@Section6_1_Comment,@Section6_2_Condition
                                ,@Section6_2_Comment,@Section6_3_Condition,@Section6_3_Comment,@Section6_4_Condition,@Section6_4_Comment,@Section6_5_Condition
                                ,@Section6_5_Comment,@Section6_6_Condition,@Section6_6_Comment,@Section6_7_Condition,@Section6_7_Comment,@Section6_8_Condition
                                ,@Section6_8_Comment,@Section7_1_Condition,@Section7_1_Comment,@Section7_2_Condition,@Section7_2_Comment,@Section7_3_Condition
                                ,@Section7_3_Comment,@Section7_4_Condition,@Section7_4_Comment,@Section7_5_Condition,@Section7_5_Comment,@Section7_6_Condition
                                ,@Section7_6_Comment,@Section8_1_Condition,@Section8_1_Comment,@Section8_2_Condition,@Section8_2_Comment,@Section8_3_Condition
                                ,@Section8_3_Comment,@Section8_4_Condition,@Section8_4_Comment,@Section8_5_Condition,@Section8_5_Comment,@Section8_6_Condition
                                ,@Section8_6_Comment,@Section8_7_Condition,@Section8_7_Comment,@Section8_8_Condition,@Section8_8_Comment,@Section8_9_Condition
                                ,@Section8_9_Comment,@Section8_10_Condition,@Section8_10_Comment,@Section8_11_Condition,@Section8_11_Comment,@Section8_12_Condition
                                ,@Section8_12_Comment,@Section8_13_Condition,@Section8_13_Comment,@Section8_14_Condition,@Section8_14_Comment,@Section8_15_Condition
                                ,@Section8_15_Comment,@Section8_16_Condition,@Section8_16_Comment,@Section8_17_Condition,@Section8_17_Comment,@Section8_18_Condition
                                ,@Section8_18_Comment,@Section8_19_Condition,@Section8_19_Comment,@Section8_20_Condition,@Section8_20_Comment,@Section8_21_Condition
                                ,@Section8_21_Comment,@Section8_22_Condition,@Section8_22_Comment,@Section8_23_Condition,@Section8_23_Comment,@Section8_24_Condition
                                ,@Section8_24_Comment,@Section8_25_Condition,@Section8_25_Comment,@Section9_1_Condition,@Section9_1_Comment,@Section9_2_Condition
                                ,@Section9_2_Comment,@Section9_3_Condition,@Section9_3_Comment,@Section9_4_Condition,@Section9_4_Comment,@Section9_5_Condition
                                ,@Section9_5_Comment,@Section9_6_Condition,@Section9_6_Comment,@Section9_7_Condition,@Section9_7_Comment,@Section9_8_Condition
                                ,@Section9_8_Comment,@Section9_9_Condition,@Section9_9_Comment,@Section9_10_Condition,@Section9_10_Comment,@Section9_11_Condition
                                ,@Section9_11_Comment,@Section9_12_Condition,@Section9_12_Comment,@Section9_13_Condition,@Section9_13_Comment,@Section9_14_Condition
                                ,@Section9_14_Comment,@Section9_15_Condition,@Section9_15_Comment
                                ,@Section9_16_Condition,@Section9_16_Comment,@Section9_17_Condition,@Section9_17_Comment
                                ,@Section10_1_Condition,@Section10_1_Comment,@Section10_2_Condition
                                ,@Section10_2_Comment,@Section10_3_Condition,@Section10_3_Comment,@Section10_4_Condition,@Section10_4_Comment,@Section10_5_Condition
                                ,@Section10_5_Comment,@Section10_6_Condition,@Section10_6_Comment,@Section10_7_Condition,@Section10_7_Comment,@Section10_8_Condition
                                ,@Section10_8_Comment,@Section10_9_Condition,@Section10_9_Comment,@Section10_10_Condition,@Section10_10_Comment,@Section10_11_Condition
                                ,@Section10_11_Comment,@Section10_12_Condition,@Section10_12_Comment,@Section10_13_Condition,@Section10_13_Comment,@Section10_14_Condition
                                ,@Section10_14_Comment,@Section10_15_Condition,@Section10_15_Comment,@Section10_16_Condition,@Section10_16_Comment,@Section11_1_Condition
                                ,@Section11_1_Comment,@Section11_2_Condition,@Section11_2_Comment,@Section11_3_Condition,@Section11_3_Comment,@Section11_4_Condition
                                ,@Section11_4_Comment,@Section11_5_Condition,@Section11_5_Comment,@Section11_6_Condition,@Section11_6_Comment,@Section11_7_Condition
                                ,@Section11_7_Comment,@Section11_8_Condition,@Section11_8_Comment,@Section12_1_Condition,@Section12_1_Comment,@Section12_2_Condition
                                ,@Section12_2_Comment,@Section12_3_Condition,@Section12_3_Comment,@Section12_4_Condition,@Section12_4_Comment,@Section12_5_Condition
                                ,@Section12_5_Comment,@Section12_6_Condition,@Section12_6_Comment,@Section13_1_Condition,@Section13_1_Comment,@Section13_2_Condition
                                ,@Section13_2_Comment,@Section13_3_Condition,@Section13_3_Comment,@Section13_4_Condition,@Section13_4_Comment,@Section14_1_Condition
                                ,@Section14_1_Comment,@Section14_2_Condition,@Section14_2_Comment,@Section14_3_Condition,@Section14_3_Comment,@Section14_4_Condition
                                ,@Section14_4_Comment,@Section14_5_Condition,@Section14_5_Comment,@Section14_6_Condition,@Section14_6_Comment,@Section14_7_Condition
                                ,@Section14_7_Comment,@Section14_8_Condition,@Section14_8_Comment,@Section14_9_Condition,@Section14_9_Comment,@Section14_10_Condition
                                ,@Section14_10_Comment,@Section14_11_Condition,@Section14_11_Comment,@Section14_12_Condition,@Section14_12_Comment,@Section14_13_Condition
                                ,@Section14_13_Comment,@Section14_14_Condition,@Section14_14_Comment,@Section14_15_Condition,@Section14_15_Comment,@Section14_16_Condition
                                ,@Section14_16_Comment,@Section14_17_Condition,@Section14_17_Comment,@Section14_18_Condition,@Section14_18_Comment,@Section14_19_Condition
                                ,@Section14_19_Comment,@Section14_20_Condition,@Section14_20_Comment,@Section14_21_Condition,@Section14_21_Comment,@Section14_22_Condition
                                ,@Section14_22_Comment,@Section14_23_Condition,@Section14_23_Comment,@Section14_24_Condition,@Section14_24_Comment,@Section14_25_Condition
                                ,@Section14_25_Comment,@Section15_1_Condition,@Section15_1_Comment,@Section15_2_Condition,@Section15_2_Comment,@Section15_3_Condition,@Section15_3_Comment
                                ,@Section15_4_Condition,@Section15_4_Comment,@Section15_5_Condition,@Section15_5_Comment,@Section15_6_Condition,@Section15_6_Comment,@Section15_7_Condition
                                ,@Section15_7_Comment,@Section15_8_Condition,@Section15_8_Comment,@Section15_9_Condition,@Section15_9_Comment,@Section15_10_Condition,@Section15_10_Comment
                                ,@Section15_11_Condition,@Section15_11_Comment,@Section15_12_Condition,@Section15_12_Comment,@Section15_13_Condition,@Section15_13_Comment,@Section15_14_Condition
                                ,@Section15_14_Comment,@Section15_15_Condition,@Section15_15_Comment,@Section16_1_Condition,@Section16_1_Comment,@Section16_2_Condition,@Section16_2_Comment
                                ,@Section16_3_Condition,@Section16_3_Comment,@Section16_4_Condition,@Section16_4_Comment,@Section17_1_Condition,@Section17_1_Comment,@Section17_2_Condition
                                ,@Section17_2_Comment,@Section17_3_Condition,@Section17_3_Comment,@Section17_4_Condition,@Section17_4_Comment,@Section17_5_Condition,@Section17_5_Comment
                                ,@Section17_6_Condition,@Section17_6_Comment,@Section18_1_Condition,@Section18_1_Comment,@Section18_2_Condition,@Section18_2_Comment,@Section18_3_Condition
                                ,@Section18_4_Comment,@Section18_5_Condition,@Section18_5_Comment,@Section18_6_Condition,@Section18_6_Comment,@Section18_7_Condition,@Section18_7_Comment
                                ,@Section18_8_Condition,@Section18_8_Comment,@Section18_9_Condition,@Section18_9_Comment
                                ,@IsSynced,@CreatedDate,@ModifyDate,@SavedAsDraft,@isDelete)";
            return InsertQury;
        }
        public void SIRDataInsertCMD(SuperintendedInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.BigInt).Value = Modal.FormVersion;
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = Modal.isDelete ?? (object)DBNull.Value; // RDBJ 01/05/2022

            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date != null ? Modal.Date : null;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port != null ? Modal.Port : null;
            command.Parameters.Add("@Master", SqlDbType.NVarChar).Value = Modal.Master != null ? Modal.Master : "";
            command.Parameters.Add("@Superintended", SqlDbType.NVarChar).Value = Modal.Superintended != null ? Modal.Superintended : "";
            command.Parameters.Add("@Section1_1_Condition", SqlDbType.NVarChar).Value = Modal.Section1_1_Condition != null ? Modal.Section1_1_Condition : "";
            command.Parameters.Add("@Section1_1_Comment", SqlDbType.NVarChar).Value = Modal.Section1_1_Comment != null ? Modal.Section1_1_Comment : "";
            command.Parameters.Add("@Section1_2_Condition", SqlDbType.NVarChar).Value = Modal.Section1_2_Condition != null ? Modal.Section1_2_Condition : "";
            command.Parameters.Add("@Section1_2_Comment", SqlDbType.NVarChar).Value = Modal.Section1_2_Comment != null ? Modal.Section1_2_Comment : "";
            command.Parameters.Add("@Section1_3_Condition", SqlDbType.NVarChar).Value = Modal.Section1_3_Condition != null ? Modal.Section1_3_Condition : "";
            command.Parameters.Add("@Section1_3_Comment", SqlDbType.NVarChar).Value = Modal.Section1_3_Comment != null ? Modal.Section1_3_Comment : "";
            command.Parameters.Add("@Section1_4_Condition", SqlDbType.NVarChar).Value = Modal.Section1_4_Condition != null ? Modal.Section1_4_Condition : "";
            command.Parameters.Add("@Section1_4_Comment", SqlDbType.NVarChar).Value = Modal.Section1_4_Comment != null ? Modal.Section1_4_Comment : "";
            command.Parameters.Add("@Section1_5_Condition", SqlDbType.NVarChar).Value = Modal.Section1_5_Condition != null ? Modal.Section1_5_Condition : "";
            command.Parameters.Add("@Section1_5_Comment", SqlDbType.NVarChar).Value = Modal.Section1_5_Comment != null ? Modal.Section1_5_Comment : "";
            command.Parameters.Add("@Section1_6_Condition", SqlDbType.NVarChar).Value = Modal.Section1_6_Condition != null ? Modal.Section1_6_Condition : "";
            command.Parameters.Add("@Section1_6_Comment", SqlDbType.NVarChar).Value = Modal.Section1_6_Comment != null ? Modal.Section1_6_Comment : "";
            command.Parameters.Add("@Section1_7_Condition", SqlDbType.NVarChar).Value = Modal.Section1_7_Condition != null ? Modal.Section1_7_Condition : "";
            command.Parameters.Add("@Section1_7_Comment", SqlDbType.NVarChar).Value = Modal.Section1_7_Comment != null ? Modal.Section1_7_Comment : "";
            command.Parameters.Add("@Section1_8_Condition", SqlDbType.NVarChar).Value = Modal.Section1_8_Condition != null ? Modal.Section1_8_Condition : "";
            command.Parameters.Add("@Section1_8_Comment", SqlDbType.NVarChar).Value = Modal.Section1_8_Comment != null ? Modal.Section1_8_Comment : "";
            command.Parameters.Add("@Section1_9_Condition", SqlDbType.NVarChar).Value = Modal.Section1_9_Condition != null ? Modal.Section1_9_Condition : "";
            command.Parameters.Add("@Section1_9_Comment", SqlDbType.NVarChar).Value = Modal.Section1_9_Comment != null ? Modal.Section1_9_Comment : "";
            command.Parameters.Add("@Section1_10_Condition", SqlDbType.NVarChar).Value = Modal.Section1_10_Condition != null ? Modal.Section1_10_Condition : "";
            command.Parameters.Add("@Section1_10_Comment", SqlDbType.NVarChar).Value = Modal.Section1_10_Comment != null ? Modal.Section1_10_Comment : "";
            command.Parameters.Add("@Section1_11_Condition", SqlDbType.NVarChar).Value = Modal.Section1_11_Condition != null ? Modal.Section1_11_Condition : "";
            command.Parameters.Add("@Section1_11_Comment", SqlDbType.NVarChar).Value = Modal.Section1_11_Comment != null ? Modal.Section1_11_Comment : "";
            command.Parameters.Add("@Section2_1_Condition", SqlDbType.NVarChar).Value = Modal.Section2_1_Condition != null ? Modal.Section2_1_Condition : "";
            command.Parameters.Add("@Section2_1_Comment", SqlDbType.NVarChar).Value = Modal.Section2_1_Comment != null ? Modal.Section2_1_Comment : "";
            command.Parameters.Add("@Section2_2_Condition", SqlDbType.NVarChar).Value = Modal.Section2_2_Condition != null ? Modal.Section2_2_Condition : "";
            command.Parameters.Add("@Section2_2_Comment", SqlDbType.NVarChar).Value = Modal.Section2_2_Comment != null ? Modal.Section2_2_Comment : "";
            command.Parameters.Add("@Section2_3_Condition", SqlDbType.NVarChar).Value = Modal.Section2_3_Condition != null ? Modal.Section2_3_Condition : "";
            command.Parameters.Add("@Section2_3_Comment", SqlDbType.NVarChar).Value = Modal.Section2_3_Comment != null ? Modal.Section2_3_Comment : "";
            command.Parameters.Add("@Section2_4_Condition", SqlDbType.NVarChar).Value = Modal.Section2_4_Condition != null ? Modal.Section2_4_Condition : "";
            command.Parameters.Add("@Section2_4_Comment", SqlDbType.NVarChar).Value = Modal.Section2_4_Comment != null ? Modal.Section2_4_Comment : "";
            command.Parameters.Add("@Section2_5_Condition", SqlDbType.NVarChar).Value = Modal.Section2_5_Condition != null ? Modal.Section2_5_Condition : "";
            command.Parameters.Add("@Section2_5_Comment", SqlDbType.NVarChar).Value = Modal.Section2_5_Comment != null ? Modal.Section2_5_Comment : "";
            command.Parameters.Add("@Section2_6_Condition", SqlDbType.NVarChar).Value = Modal.Section2_6_Condition != null ? Modal.Section2_6_Condition : "";
            command.Parameters.Add("@Section2_6_Comment", SqlDbType.NVarChar).Value = Modal.Section2_6_Comment != null ? Modal.Section2_6_Comment : "";
            command.Parameters.Add("@Section2_7_Condition", SqlDbType.NVarChar).Value = Modal.Section2_7_Condition != null ? Modal.Section2_7_Condition : "";
            command.Parameters.Add("@Section2_7_Comment", SqlDbType.NVarChar).Value = Modal.Section2_7_Comment != null ? Modal.Section2_7_Comment : "";
            command.Parameters.Add("@Section3_1_Condition", SqlDbType.NVarChar).Value = Modal.Section3_1_Condition != null ? Modal.Section3_1_Condition : "";
            command.Parameters.Add("@Section3_1_Comment", SqlDbType.NVarChar).Value = Modal.Section3_1_Comment != null ? Modal.Section3_1_Comment : "";
            command.Parameters.Add("@Section3_2_Condition", SqlDbType.NVarChar).Value = Modal.Section3_2_Condition != null ? Modal.Section3_2_Condition : "";
            command.Parameters.Add("@Section3_2_Comment", SqlDbType.NVarChar).Value = Modal.Section3_2_Comment != null ? Modal.Section3_2_Comment : "";
            command.Parameters.Add("@Section3_3_Condition", SqlDbType.NVarChar).Value = Modal.Section3_3_Condition != null ? Modal.Section3_3_Condition : "";
            command.Parameters.Add("@Section3_3_Comment", SqlDbType.NVarChar).Value = Modal.Section3_3_Comment != null ? Modal.Section3_3_Comment : "";
            command.Parameters.Add("@Section3_4_Condition", SqlDbType.NVarChar).Value = Modal.Section3_4_Condition != null ? Modal.Section3_4_Condition : "";
            command.Parameters.Add("@Section3_4_Comment", SqlDbType.NVarChar).Value = Modal.Section3_4_Comment != null ? Modal.Section3_4_Comment : "";
            command.Parameters.Add("@Section3_5_Condition", SqlDbType.NVarChar).Value = Modal.Section3_5_Condition != null ? Modal.Section3_5_Condition : "";
            command.Parameters.Add("@Section3_5_Comment", SqlDbType.NVarChar).Value = Modal.Section3_5_Comment != null ? Modal.Section3_5_Comment : "";
            command.Parameters.Add("@Section4_1_Condition", SqlDbType.NVarChar).Value = Modal.Section4_1_Condition != null ? Modal.Section4_1_Condition : "";
            command.Parameters.Add("@Section4_1_Comment", SqlDbType.NVarChar).Value = Modal.Section4_1_Comment != null ? Modal.Section4_1_Comment : "";
            command.Parameters.Add("@Section4_2_Condition", SqlDbType.NVarChar).Value = Modal.Section4_2_Condition != null ? Modal.Section4_2_Condition : "";
            command.Parameters.Add("@Section4_2_Comment", SqlDbType.NVarChar).Value = Modal.Section4_2_Comment != null ? Modal.Section4_2_Comment : "";
            command.Parameters.Add("@Section4_3_Condition", SqlDbType.NVarChar).Value = Modal.Section4_3_Condition != null ? Modal.Section4_3_Condition : "";
            command.Parameters.Add("@Section4_3_Comment", SqlDbType.NVarChar).Value = Modal.Section4_3_Comment != null ? Modal.Section4_3_Comment : "";
            command.Parameters.Add("@Section5_1_Condition", SqlDbType.NVarChar).Value = Modal.Section5_1_Condition != null ? Modal.Section5_1_Condition : "";
            command.Parameters.Add("@Section5_1_Comment", SqlDbType.NVarChar).Value = Modal.Section5_1_Comment != null ? Modal.Section5_1_Comment : "";
            command.Parameters.Add("@Section5_6_Condition", SqlDbType.NVarChar).Value = Modal.Section5_6_Condition != null ? Modal.Section5_6_Condition : "";
            command.Parameters.Add("@Section5_6_Comment", SqlDbType.NVarChar).Value = Modal.Section5_6_Comment != null ? Modal.Section5_6_Comment : "";
            command.Parameters.Add("@Section5_8_Condition", SqlDbType.NVarChar).Value = Modal.Section5_8_Condition != null ? Modal.Section5_8_Condition : "";
            command.Parameters.Add("@Section5_8_Comment", SqlDbType.NVarChar).Value = Modal.Section5_8_Comment != null ? Modal.Section5_8_Comment : "";
            command.Parameters.Add("@Section5_9_Condition", SqlDbType.NVarChar).Value = Modal.Section5_9_Condition != null ? Modal.Section5_9_Condition : "";
            command.Parameters.Add("@Section5_9_Comment", SqlDbType.NVarChar).Value = Modal.Section5_9_Comment != null ? Modal.Section5_9_Comment : "";
            command.Parameters.Add("@Section6_1_Condition", SqlDbType.NVarChar).Value = Modal.Section6_1_Condition != null ? Modal.Section6_1_Condition : "";
            command.Parameters.Add("@Section6_1_Comment", SqlDbType.NVarChar).Value = Modal.Section6_1_Comment != null ? Modal.Section6_1_Comment : "";
            command.Parameters.Add("@Section6_2_Condition", SqlDbType.NVarChar).Value = Modal.Section6_2_Condition != null ? Modal.Section6_2_Condition : "";
            command.Parameters.Add("@Section6_2_Comment", SqlDbType.NVarChar).Value = Modal.Section6_2_Comment != null ? Modal.Section6_2_Comment : "";
            command.Parameters.Add("@Section6_3_Condition", SqlDbType.NVarChar).Value = Modal.Section6_3_Condition != null ? Modal.Section6_3_Condition : "";
            command.Parameters.Add("@Section6_3_Comment", SqlDbType.NVarChar).Value = Modal.Section6_3_Comment != null ? Modal.Section6_3_Comment : "";
            command.Parameters.Add("@Section6_4_Condition", SqlDbType.NVarChar).Value = Modal.Section6_4_Condition != null ? Modal.Section6_4_Condition : "";
            command.Parameters.Add("@Section6_4_Comment", SqlDbType.NVarChar).Value = Modal.Section6_4_Comment != null ? Modal.Section6_4_Comment : "";
            command.Parameters.Add("@Section6_5_Condition", SqlDbType.NVarChar).Value = Modal.Section6_5_Condition != null ? Modal.Section6_5_Condition : "";
            command.Parameters.Add("@Section6_5_Comment", SqlDbType.NVarChar).Value = Modal.Section6_5_Comment != null ? Modal.Section6_5_Comment : "";
            command.Parameters.Add("@Section6_6_Condition", SqlDbType.NVarChar).Value = Modal.Section6_6_Condition != null ? Modal.Section6_6_Condition : "";
            command.Parameters.Add("@Section6_6_Comment", SqlDbType.NVarChar).Value = Modal.Section6_6_Comment != null ? Modal.Section6_6_Comment : "";
            command.Parameters.Add("@Section6_7_Condition", SqlDbType.NVarChar).Value = Modal.Section6_7_Condition != null ? Modal.Section6_7_Condition : "";
            command.Parameters.Add("@Section6_7_Comment", SqlDbType.NVarChar).Value = Modal.Section6_7_Comment != null ? Modal.Section6_7_Comment : "";
            command.Parameters.Add("@Section6_8_Condition", SqlDbType.NVarChar).Value = Modal.Section6_8_Condition != null ? Modal.Section6_8_Condition : "";
            command.Parameters.Add("@Section6_8_Comment", SqlDbType.NVarChar).Value = Modal.Section6_8_Comment != null ? Modal.Section6_8_Comment : "";
            command.Parameters.Add("@Section7_1_Condition", SqlDbType.NVarChar).Value = Modal.Section7_1_Condition != null ? Modal.Section7_1_Condition : "";
            command.Parameters.Add("@Section7_1_Comment", SqlDbType.NVarChar).Value = Modal.Section7_1_Comment != null ? Modal.Section7_1_Comment : "";
            command.Parameters.Add("@Section7_2_Condition", SqlDbType.NVarChar).Value = Modal.Section7_2_Condition != null ? Modal.Section7_2_Condition : "";
            command.Parameters.Add("@Section7_2_Comment", SqlDbType.NVarChar).Value = Modal.Section7_2_Comment != null ? Modal.Section7_2_Comment : "";
            command.Parameters.Add("@Section7_3_Condition", SqlDbType.NVarChar).Value = Modal.Section7_3_Condition != null ? Modal.Section7_3_Condition : "";
            command.Parameters.Add("@Section7_3_Comment", SqlDbType.NVarChar).Value = Modal.Section7_3_Comment != null ? Modal.Section7_3_Comment : "";
            command.Parameters.Add("@Section7_4_Condition", SqlDbType.NVarChar).Value = Modal.Section7_4_Condition != null ? Modal.Section7_4_Condition : "";
            command.Parameters.Add("@Section7_4_Comment", SqlDbType.NVarChar).Value = Modal.Section7_4_Comment != null ? Modal.Section7_4_Comment : "";
            command.Parameters.Add("@Section7_5_Condition", SqlDbType.NVarChar).Value = Modal.Section7_5_Condition != null ? Modal.Section7_5_Condition : "";
            command.Parameters.Add("@Section7_5_Comment", SqlDbType.NVarChar).Value = Modal.Section7_5_Comment != null ? Modal.Section7_5_Comment : "";
            command.Parameters.Add("@Section7_6_Condition", SqlDbType.NVarChar).Value = Modal.Section7_6_Condition != null ? Modal.Section7_6_Condition : "";
            command.Parameters.Add("@Section7_6_Comment", SqlDbType.NVarChar).Value = Modal.Section7_6_Comment != null ? Modal.Section7_6_Comment : "";
            command.Parameters.Add("@Section8_1_Condition", SqlDbType.NVarChar).Value = Modal.Section8_1_Condition != null ? Modal.Section8_1_Condition : "";
            command.Parameters.Add("@Section8_1_Comment", SqlDbType.NVarChar).Value = Modal.Section8_1_Comment != null ? Modal.Section8_1_Comment : "";
            command.Parameters.Add("@Section8_2_Condition", SqlDbType.NVarChar).Value = Modal.Section8_2_Condition != null ? Modal.Section8_2_Condition : "";
            command.Parameters.Add("@Section8_2_Comment", SqlDbType.NVarChar).Value = Modal.Section8_2_Comment != null ? Modal.Section8_2_Comment : "";
            command.Parameters.Add("@Section8_3_Condition", SqlDbType.NVarChar).Value = Modal.Section8_3_Condition != null ? Modal.Section8_3_Condition : "";
            command.Parameters.Add("@Section8_3_Comment", SqlDbType.NVarChar).Value = Modal.Section8_3_Comment != null ? Modal.Section8_3_Comment : "";
            command.Parameters.Add("@Section8_4_Condition", SqlDbType.NVarChar).Value = Modal.Section8_4_Condition != null ? Modal.Section8_4_Condition : "";
            command.Parameters.Add("@Section8_4_Comment", SqlDbType.NVarChar).Value = Modal.Section8_4_Comment != null ? Modal.Section8_4_Comment : "";
            command.Parameters.Add("@Section8_5_Condition", SqlDbType.NVarChar).Value = Modal.Section8_5_Condition != null ? Modal.Section8_5_Condition : "";
            command.Parameters.Add("@Section8_5_Comment", SqlDbType.NVarChar).Value = Modal.Section8_5_Comment != null ? Modal.Section8_5_Comment : "";
            command.Parameters.Add("@Section8_6_Condition", SqlDbType.NVarChar).Value = Modal.Section8_6_Condition != null ? Modal.Section8_6_Condition : "";
            command.Parameters.Add("@Section8_6_Comment", SqlDbType.NVarChar).Value = Modal.Section8_6_Comment != null ? Modal.Section8_6_Comment : "";
            command.Parameters.Add("@Section8_7_Condition", SqlDbType.NVarChar).Value = Modal.Section8_7_Condition != null ? Modal.Section8_7_Condition : "";
            command.Parameters.Add("@Section8_7_Comment", SqlDbType.NVarChar).Value = Modal.Section8_7_Comment != null ? Modal.Section8_7_Comment : "";
            command.Parameters.Add("@Section8_8_Condition", SqlDbType.NVarChar).Value = Modal.Section8_8_Condition != null ? Modal.Section8_8_Condition : "";
            command.Parameters.Add("@Section8_8_Comment", SqlDbType.NVarChar).Value = Modal.Section8_8_Comment != null ? Modal.Section8_8_Comment : "";
            command.Parameters.Add("@Section8_9_Condition", SqlDbType.NVarChar).Value = Modal.Section8_9_Condition != null ? Modal.Section8_9_Condition : "";
            command.Parameters.Add("@Section8_9_Comment", SqlDbType.NVarChar).Value = Modal.Section8_9_Comment != null ? Modal.Section8_9_Comment : "";
            command.Parameters.Add("@Section8_10_Condition", SqlDbType.NVarChar).Value = Modal.Section8_10_Condition != null ? Modal.Section8_10_Condition : "";
            command.Parameters.Add("@Section8_10_Comment", SqlDbType.NVarChar).Value = Modal.Section8_10_Comment != null ? Modal.Section8_10_Comment : "";
            command.Parameters.Add("@Section8_11_Condition", SqlDbType.NVarChar).Value = Modal.Section8_11_Condition != null ? Modal.Section8_11_Condition : "";
            command.Parameters.Add("@Section8_11_Comment", SqlDbType.NVarChar).Value = Modal.Section8_11_Comment != null ? Modal.Section8_11_Comment : "";
            command.Parameters.Add("@Section8_12_Condition", SqlDbType.NVarChar).Value = Modal.Section8_12_Condition != null ? Modal.Section8_12_Condition : "";
            command.Parameters.Add("@Section8_12_Comment", SqlDbType.NVarChar).Value = Modal.Section8_12_Comment != null ? Modal.Section8_12_Comment : "";
            command.Parameters.Add("@Section8_13_Condition", SqlDbType.NVarChar).Value = Modal.Section8_13_Condition != null ? Modal.Section8_13_Condition : "";
            command.Parameters.Add("@Section8_13_Comment", SqlDbType.NVarChar).Value = Modal.Section8_13_Comment != null ? Modal.Section8_13_Comment : "";
            command.Parameters.Add("@Section8_14_Condition", SqlDbType.NVarChar).Value = Modal.Section8_14_Condition != null ? Modal.Section8_14_Condition : "";
            command.Parameters.Add("@Section8_14_Comment", SqlDbType.NVarChar).Value = Modal.Section8_14_Comment != null ? Modal.Section8_14_Comment : "";
            command.Parameters.Add("@Section8_15_Condition", SqlDbType.NVarChar).Value = Modal.Section8_15_Condition != null ? Modal.Section8_15_Condition : "";
            command.Parameters.Add("@Section8_15_Comment", SqlDbType.NVarChar).Value = Modal.Section8_15_Comment != null ? Modal.Section8_15_Comment : "";
            command.Parameters.Add("@Section8_16_Condition", SqlDbType.NVarChar).Value = Modal.Section8_16_Condition != null ? Modal.Section8_16_Condition : "";
            command.Parameters.Add("@Section8_16_Comment", SqlDbType.NVarChar).Value = Modal.Section8_16_Comment != null ? Modal.Section8_16_Comment : "";
            command.Parameters.Add("@Section8_17_Condition", SqlDbType.NVarChar).Value = Modal.Section8_17_Condition != null ? Modal.Section8_17_Condition : "";
            command.Parameters.Add("@Section8_17_Comment", SqlDbType.NVarChar).Value = Modal.Section8_17_Comment != null ? Modal.Section8_17_Comment : "";
            command.Parameters.Add("@Section8_18_Condition", SqlDbType.NVarChar).Value = Modal.Section8_18_Condition != null ? Modal.Section8_18_Condition : "";
            command.Parameters.Add("@Section8_18_Comment", SqlDbType.NVarChar).Value = Modal.Section8_18_Comment != null ? Modal.Section8_18_Comment : "";
            command.Parameters.Add("@Section8_19_Condition", SqlDbType.NVarChar).Value = Modal.Section8_19_Condition != null ? Modal.Section8_19_Condition : "";
            command.Parameters.Add("@Section8_19_Comment", SqlDbType.NVarChar).Value = Modal.Section8_19_Comment != null ? Modal.Section8_19_Comment : "";
            command.Parameters.Add("@Section8_20_Condition", SqlDbType.NVarChar).Value = Modal.Section8_20_Condition != null ? Modal.Section8_20_Condition : "";
            command.Parameters.Add("@Section8_20_Comment", SqlDbType.NVarChar).Value = Modal.Section8_20_Comment != null ? Modal.Section8_20_Comment : "";
            command.Parameters.Add("@Section8_21_Condition", SqlDbType.NVarChar).Value = Modal.Section8_21_Condition != null ? Modal.Section8_21_Condition : "";
            command.Parameters.Add("@Section8_21_Comment", SqlDbType.NVarChar).Value = Modal.Section8_21_Comment != null ? Modal.Section8_21_Comment : "";
            command.Parameters.Add("@Section8_22_Condition", SqlDbType.NVarChar).Value = Modal.Section8_22_Condition != null ? Modal.Section8_22_Condition : "";
            command.Parameters.Add("@Section8_22_Comment", SqlDbType.NVarChar).Value = Modal.Section8_22_Comment != null ? Modal.Section8_22_Comment : "";
            command.Parameters.Add("@Section8_23_Condition", SqlDbType.NVarChar).Value = Modal.Section8_23_Condition != null ? Modal.Section8_23_Condition : "";
            command.Parameters.Add("@Section8_23_Comment", SqlDbType.NVarChar).Value = Modal.Section8_23_Comment != null ? Modal.Section8_23_Comment : "";
            command.Parameters.Add("@Section8_24_Condition", SqlDbType.NVarChar).Value = Modal.Section8_24_Condition != null ? Modal.Section8_24_Condition : "";
            command.Parameters.Add("@Section8_24_Comment", SqlDbType.NVarChar).Value = Modal.Section8_24_Comment != null ? Modal.Section8_24_Comment : "";
            command.Parameters.Add("@Section8_25_Condition", SqlDbType.NVarChar).Value = Modal.Section8_25_Condition != null ? Modal.Section8_25_Condition : "";
            command.Parameters.Add("@Section8_25_Comment", SqlDbType.NVarChar).Value = Modal.Section8_25_Comment != null ? Modal.Section8_25_Comment : "";
            command.Parameters.Add("@Section9_1_Condition", SqlDbType.NVarChar).Value = Modal.Section9_1_Condition != null ? Modal.Section9_1_Condition : "";
            command.Parameters.Add("@Section9_1_Comment", SqlDbType.NVarChar).Value = Modal.Section9_1_Comment != null ? Modal.Section9_1_Comment : "";
            command.Parameters.Add("@Section9_2_Condition", SqlDbType.NVarChar).Value = Modal.Section9_2_Condition != null ? Modal.Section9_2_Condition : "";
            command.Parameters.Add("@Section9_2_Comment", SqlDbType.NVarChar).Value = Modal.Section9_2_Comment != null ? Modal.Section9_2_Comment : "";
            command.Parameters.Add("@Section9_3_Condition", SqlDbType.NVarChar).Value = Modal.Section9_3_Condition != null ? Modal.Section9_3_Condition : "";
            command.Parameters.Add("@Section9_3_Comment", SqlDbType.NVarChar).Value = Modal.Section9_3_Comment != null ? Modal.Section9_3_Comment : "";
            command.Parameters.Add("@Section9_4_Condition", SqlDbType.NVarChar).Value = Modal.Section9_4_Condition != null ? Modal.Section9_4_Condition : "";
            command.Parameters.Add("@Section9_4_Comment", SqlDbType.NVarChar).Value = Modal.Section9_4_Comment != null ? Modal.Section9_4_Comment : "";
            command.Parameters.Add("@Section9_5_Condition", SqlDbType.NVarChar).Value = Modal.Section9_5_Condition != null ? Modal.Section9_5_Condition : "";
            command.Parameters.Add("@Section9_5_Comment", SqlDbType.NVarChar).Value = Modal.Section9_5_Comment != null ? Modal.Section9_5_Comment : "";
            command.Parameters.Add("@Section9_6_Condition", SqlDbType.NVarChar).Value = Modal.Section9_6_Condition != null ? Modal.Section9_6_Condition : "";
            command.Parameters.Add("@Section9_6_Comment", SqlDbType.NVarChar).Value = Modal.Section9_6_Comment != null ? Modal.Section9_6_Comment : "";
            command.Parameters.Add("@Section9_7_Condition", SqlDbType.NVarChar).Value = Modal.Section9_7_Condition != null ? Modal.Section9_7_Condition : "";
            command.Parameters.Add("@Section9_7_Comment", SqlDbType.NVarChar).Value = Modal.Section9_7_Comment != null ? Modal.Section9_7_Comment : "";
            command.Parameters.Add("@Section9_8_Condition", SqlDbType.NVarChar).Value = Modal.Section9_8_Condition != null ? Modal.Section9_8_Condition : "";
            command.Parameters.Add("@Section9_8_Comment", SqlDbType.NVarChar).Value = Modal.Section9_8_Comment != null ? Modal.Section9_8_Comment : "";
            command.Parameters.Add("@Section9_9_Condition", SqlDbType.NVarChar).Value = Modal.Section9_9_Condition != null ? Modal.Section9_9_Condition : "";
            command.Parameters.Add("@Section9_9_Comment", SqlDbType.NVarChar).Value = Modal.Section9_9_Comment != null ? Modal.Section9_9_Comment : "";
            command.Parameters.Add("@Section9_10_Condition", SqlDbType.NVarChar).Value = Modal.Section9_10_Condition != null ? Modal.Section9_10_Condition : "";
            command.Parameters.Add("@Section9_10_Comment", SqlDbType.NVarChar).Value = Modal.Section9_10_Comment != null ? Modal.Section9_10_Comment : "";
            command.Parameters.Add("@Section9_11_Condition", SqlDbType.NVarChar).Value = Modal.Section9_11_Condition != null ? Modal.Section9_11_Condition : "";
            command.Parameters.Add("@Section9_11_Comment", SqlDbType.NVarChar).Value = Modal.Section9_11_Comment != null ? Modal.Section9_11_Comment : "";
            command.Parameters.Add("@Section9_12_Condition", SqlDbType.NVarChar).Value = Modal.Section9_12_Condition != null ? Modal.Section9_12_Condition : "";
            command.Parameters.Add("@Section9_12_Comment", SqlDbType.NVarChar).Value = Modal.Section9_12_Comment != null ? Modal.Section9_12_Comment : "";
            command.Parameters.Add("@Section9_13_Condition", SqlDbType.NVarChar).Value = Modal.Section9_13_Condition != null ? Modal.Section9_13_Condition : "";
            command.Parameters.Add("@Section9_13_Comment", SqlDbType.NVarChar).Value = Modal.Section9_13_Comment != null ? Modal.Section9_13_Comment : "";
            command.Parameters.Add("@Section9_14_Condition", SqlDbType.NVarChar).Value = Modal.Section9_14_Condition != null ? Modal.Section9_14_Condition : "";
            command.Parameters.Add("@Section9_14_Comment", SqlDbType.NVarChar).Value = Modal.Section9_14_Comment != null ? Modal.Section9_14_Comment : "";
            command.Parameters.Add("@Section9_15_Condition", SqlDbType.NVarChar).Value = Modal.Section9_15_Condition != null ? Modal.Section9_15_Condition : "";
            command.Parameters.Add("@Section9_15_Comment", SqlDbType.NVarChar).Value = Modal.Section9_15_Comment != null ? Modal.Section9_15_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section9_16_Condition", SqlDbType.NVarChar).Value = Modal.Section9_16_Condition != null ? Modal.Section9_16_Condition : "";
            command.Parameters.Add("@Section9_16_Comment", SqlDbType.NVarChar).Value = Modal.Section9_16_Comment != null ? Modal.Section9_16_Comment : "";
            command.Parameters.Add("@Section9_17_Condition", SqlDbType.NVarChar).Value = Modal.Section9_17_Condition != null ? Modal.Section9_17_Condition : "";
            command.Parameters.Add("@Section9_17_Comment", SqlDbType.NVarChar).Value = Modal.Section9_17_Comment != null ? Modal.Section9_17_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@Section10_1_Condition", SqlDbType.NVarChar).Value = Modal.Section10_1_Condition != null ? Modal.Section10_1_Condition : "";
            command.Parameters.Add("@Section10_1_Comment", SqlDbType.NVarChar).Value = Modal.Section10_1_Comment != null ? Modal.Section10_1_Comment : "";
            command.Parameters.Add("@Section10_2_Condition", SqlDbType.NVarChar).Value = Modal.Section10_2_Condition != null ? Modal.Section10_2_Condition : "";
            command.Parameters.Add("@Section10_2_Comment", SqlDbType.NVarChar).Value = Modal.Section10_2_Comment != null ? Modal.Section10_2_Comment : "";
            command.Parameters.Add("@Section10_3_Condition", SqlDbType.NVarChar).Value = Modal.Section10_3_Condition != null ? Modal.Section10_3_Condition : "";
            command.Parameters.Add("@Section10_3_Comment", SqlDbType.NVarChar).Value = Modal.Section10_3_Comment != null ? Modal.Section10_3_Comment : "";
            command.Parameters.Add("@Section10_4_Condition", SqlDbType.NVarChar).Value = Modal.Section10_4_Condition != null ? Modal.Section10_4_Condition : "";
            command.Parameters.Add("@Section10_4_Comment", SqlDbType.NVarChar).Value = Modal.Section10_4_Comment != null ? Modal.Section10_4_Comment : "";
            command.Parameters.Add("@Section10_5_Condition", SqlDbType.NVarChar).Value = Modal.Section10_5_Condition != null ? Modal.Section10_5_Condition : "";
            command.Parameters.Add("@Section10_5_Comment", SqlDbType.NVarChar).Value = Modal.Section10_5_Comment != null ? Modal.Section10_5_Comment : "";
            command.Parameters.Add("@Section10_6_Condition", SqlDbType.NVarChar).Value = Modal.Section10_6_Condition != null ? Modal.Section10_6_Condition : "";
            command.Parameters.Add("@Section10_6_Comment", SqlDbType.NVarChar).Value = Modal.Section10_6_Comment != null ? Modal.Section10_6_Comment : "";
            command.Parameters.Add("@Section10_7_Condition", SqlDbType.NVarChar).Value = Modal.Section10_7_Condition != null ? Modal.Section10_7_Condition : "";
            command.Parameters.Add("@Section10_7_Comment", SqlDbType.NVarChar).Value = Modal.Section10_7_Comment != null ? Modal.Section10_7_Comment : "";
            command.Parameters.Add("@Section10_8_Condition", SqlDbType.NVarChar).Value = Modal.Section10_8_Condition != null ? Modal.Section10_8_Condition : "";
            command.Parameters.Add("@Section10_8_Comment", SqlDbType.NVarChar).Value = Modal.Section10_8_Comment != null ? Modal.Section10_8_Comment : "";
            command.Parameters.Add("@Section10_9_Condition", SqlDbType.NVarChar).Value = Modal.Section10_9_Condition != null ? Modal.Section10_9_Condition : "";
            command.Parameters.Add("@Section10_9_Comment", SqlDbType.NVarChar).Value = Modal.Section10_9_Comment != null ? Modal.Section10_9_Comment : "";
            command.Parameters.Add("@Section10_10_Condition", SqlDbType.NVarChar).Value = Modal.Section10_10_Condition != null ? Modal.Section10_10_Condition : "";
            command.Parameters.Add("@Section10_10_Comment", SqlDbType.NVarChar).Value = Modal.Section10_10_Comment != null ? Modal.Section10_10_Comment : "";
            command.Parameters.Add("@Section10_11_Condition", SqlDbType.NVarChar).Value = Modal.Section10_11_Condition != null ? Modal.Section10_11_Condition : "";
            command.Parameters.Add("@Section10_11_Comment", SqlDbType.NVarChar).Value = Modal.Section10_11_Comment != null ? Modal.Section10_11_Comment : "";
            command.Parameters.Add("@Section10_12_Condition", SqlDbType.NVarChar).Value = Modal.Section10_12_Condition != null ? Modal.Section10_12_Condition : "";
            command.Parameters.Add("@Section10_12_Comment", SqlDbType.NVarChar).Value = Modal.Section10_12_Comment != null ? Modal.Section10_12_Comment : "";
            command.Parameters.Add("@Section10_13_Condition", SqlDbType.NVarChar).Value = Modal.Section10_13_Condition != null ? Modal.Section10_13_Condition : "";
            command.Parameters.Add("@Section10_13_Comment", SqlDbType.NVarChar).Value = Modal.Section10_13_Comment != null ? Modal.Section10_13_Comment : "";
            command.Parameters.Add("@Section10_14_Condition", SqlDbType.NVarChar).Value = Modal.Section10_14_Condition != null ? Modal.Section10_14_Condition : "";
            command.Parameters.Add("@Section10_14_Comment", SqlDbType.NVarChar).Value = Modal.Section10_14_Comment != null ? Modal.Section10_14_Comment : "";
            command.Parameters.Add("@Section10_15_Condition", SqlDbType.NVarChar).Value = Modal.Section10_15_Condition != null ? Modal.Section10_15_Condition : "";
            command.Parameters.Add("@Section10_15_Comment", SqlDbType.NVarChar).Value = Modal.Section10_15_Comment != null ? Modal.Section10_15_Comment : "";
            command.Parameters.Add("@Section10_16_Condition", SqlDbType.NVarChar).Value = Modal.Section10_16_Condition != null ? Modal.Section10_16_Condition : "";
            command.Parameters.Add("@Section10_16_Comment", SqlDbType.NVarChar).Value = Modal.Section10_16_Comment != null ? Modal.Section10_16_Comment : "";
            command.Parameters.Add("@Section11_1_Condition", SqlDbType.NVarChar).Value = Modal.Section11_1_Condition != null ? Modal.Section11_1_Condition : "";
            command.Parameters.Add("@Section11_1_Comment", SqlDbType.NVarChar).Value = Modal.Section11_1_Comment != null ? Modal.Section11_1_Comment : "";
            command.Parameters.Add("@Section11_2_Condition", SqlDbType.NVarChar).Value = Modal.Section11_2_Condition != null ? Modal.Section11_2_Condition : "";
            command.Parameters.Add("@Section11_2_Comment", SqlDbType.NVarChar).Value = Modal.Section11_2_Comment != null ? Modal.Section11_2_Comment : "";
            command.Parameters.Add("@Section11_3_Condition", SqlDbType.NVarChar).Value = Modal.Section11_3_Condition != null ? Modal.Section11_3_Condition : "";
            command.Parameters.Add("@Section11_3_Comment", SqlDbType.NVarChar).Value = Modal.Section11_3_Comment != null ? Modal.Section11_3_Comment : "";
            command.Parameters.Add("@Section11_4_Condition", SqlDbType.NVarChar).Value = Modal.Section11_4_Condition != null ? Modal.Section11_4_Condition : "";
            command.Parameters.Add("@Section11_4_Comment", SqlDbType.NVarChar).Value = Modal.Section11_4_Comment != null ? Modal.Section11_4_Comment : "";
            command.Parameters.Add("@Section11_5_Condition", SqlDbType.NVarChar).Value = Modal.Section11_5_Condition != null ? Modal.Section11_5_Condition : "";
            command.Parameters.Add("@Section11_5_Comment", SqlDbType.NVarChar).Value = Modal.Section11_5_Comment != null ? Modal.Section11_5_Comment : "";
            command.Parameters.Add("@Section11_6_Condition", SqlDbType.NVarChar).Value = Modal.Section11_6_Condition != null ? Modal.Section11_6_Condition : "";
            command.Parameters.Add("@Section11_6_Comment", SqlDbType.NVarChar).Value = Modal.Section11_6_Comment != null ? Modal.Section11_6_Comment : "";
            command.Parameters.Add("@Section11_7_Condition", SqlDbType.NVarChar).Value = Modal.Section11_7_Condition != null ? Modal.Section11_7_Condition : "";
            command.Parameters.Add("@Section11_7_Comment", SqlDbType.NVarChar).Value = Modal.Section11_7_Comment != null ? Modal.Section11_7_Comment : "";
            command.Parameters.Add("@Section11_8_Condition", SqlDbType.NVarChar).Value = Modal.Section11_8_Condition != null ? Modal.Section11_8_Condition : "";
            command.Parameters.Add("@Section11_8_Comment", SqlDbType.NVarChar).Value = Modal.Section11_8_Comment != null ? Modal.Section11_8_Comment : "";
            command.Parameters.Add("@Section12_1_Condition", SqlDbType.NVarChar).Value = Modal.Section12_1_Condition != null ? Modal.Section12_1_Condition : "";
            command.Parameters.Add("@Section12_1_Comment", SqlDbType.NVarChar).Value = Modal.Section12_1_Comment != null ? Modal.Section12_1_Comment : "";
            command.Parameters.Add("@Section12_2_Condition", SqlDbType.NVarChar).Value = Modal.Section12_2_Condition != null ? Modal.Section12_2_Condition : "";
            command.Parameters.Add("@Section12_2_Comment", SqlDbType.NVarChar).Value = Modal.Section12_2_Comment != null ? Modal.Section12_2_Comment : "";
            command.Parameters.Add("@Section12_3_Condition", SqlDbType.NVarChar).Value = Modal.Section12_3_Condition != null ? Modal.Section12_3_Condition : "";
            command.Parameters.Add("@Section12_3_Comment", SqlDbType.NVarChar).Value = Modal.Section12_3_Comment != null ? Modal.Section12_3_Comment : "";
            command.Parameters.Add("@Section12_4_Condition", SqlDbType.NVarChar).Value = Modal.Section12_4_Condition != null ? Modal.Section12_4_Condition : "";
            command.Parameters.Add("@Section12_4_Comment", SqlDbType.NVarChar).Value = Modal.Section12_4_Comment != null ? Modal.Section12_4_Comment : "";
            command.Parameters.Add("@Section12_5_Condition", SqlDbType.NVarChar).Value = Modal.Section12_5_Condition != null ? Modal.Section12_5_Condition : "";
            command.Parameters.Add("@Section12_5_Comment", SqlDbType.NVarChar).Value = Modal.Section12_5_Comment != null ? Modal.Section12_5_Comment : "";
            command.Parameters.Add("@Section12_6_Condition", SqlDbType.NVarChar).Value = Modal.Section12_6_Condition != null ? Modal.Section12_6_Condition : "";
            command.Parameters.Add("@Section12_6_Comment", SqlDbType.NVarChar).Value = Modal.Section12_6_Comment != null ? Modal.Section12_6_Comment : "";
            command.Parameters.Add("@Section13_1_Condition", SqlDbType.NVarChar).Value = Modal.Section13_1_Condition != null ? Modal.Section13_1_Condition : "";
            command.Parameters.Add("@Section13_1_Comment", SqlDbType.NVarChar).Value = Modal.Section13_1_Comment != null ? Modal.Section13_1_Comment : "";
            command.Parameters.Add("@Section13_2_Condition", SqlDbType.NVarChar).Value = Modal.Section13_2_Condition != null ? Modal.Section13_2_Condition : "";
            command.Parameters.Add("@Section13_2_Comment", SqlDbType.NVarChar).Value = Modal.Section13_2_Comment != null ? Modal.Section13_2_Comment : "";
            command.Parameters.Add("@Section13_3_Condition", SqlDbType.NVarChar).Value = Modal.Section13_3_Condition != null ? Modal.Section13_3_Condition : "";
            command.Parameters.Add("@Section13_3_Comment", SqlDbType.NVarChar).Value = Modal.Section13_3_Comment != null ? Modal.Section13_3_Comment : "";
            command.Parameters.Add("@Section13_4_Condition", SqlDbType.NVarChar).Value = Modal.Section13_4_Condition != null ? Modal.Section13_4_Condition : "";
            command.Parameters.Add("@Section13_4_Comment", SqlDbType.NVarChar).Value = Modal.Section13_4_Comment != null ? Modal.Section13_4_Comment : "";
            command.Parameters.Add("@Section14_1_Condition", SqlDbType.NVarChar).Value = Modal.Section14_1_Condition != null ? Modal.Section14_1_Condition : "";
            command.Parameters.Add("@Section14_1_Comment", SqlDbType.NVarChar).Value = Modal.Section14_1_Comment != null ? Modal.Section14_1_Comment : "";
            command.Parameters.Add("@Section14_2_Condition", SqlDbType.NVarChar).Value = Modal.Section14_2_Condition != null ? Modal.Section14_2_Condition : "";
            command.Parameters.Add("@Section14_2_Comment", SqlDbType.NVarChar).Value = Modal.Section14_2_Comment != null ? Modal.Section14_2_Comment : "";
            command.Parameters.Add("@Section14_3_Condition", SqlDbType.NVarChar).Value = Modal.Section14_3_Condition != null ? Modal.Section14_3_Condition : "";
            command.Parameters.Add("@Section14_3_Comment", SqlDbType.NVarChar).Value = Modal.Section14_3_Comment != null ? Modal.Section14_3_Comment : "";
            command.Parameters.Add("@Section14_4_Condition", SqlDbType.NVarChar).Value = Modal.Section14_4_Condition != null ? Modal.Section14_4_Condition : "";
            command.Parameters.Add("@Section14_4_Comment", SqlDbType.NVarChar).Value = Modal.Section14_4_Comment != null ? Modal.Section14_4_Comment : "";
            command.Parameters.Add("@Section14_5_Condition", SqlDbType.NVarChar).Value = Modal.Section14_5_Condition != null ? Modal.Section14_5_Condition : "";
            command.Parameters.Add("@Section14_5_Comment", SqlDbType.NVarChar).Value = Modal.Section14_5_Comment != null ? Modal.Section14_5_Comment : "";
            command.Parameters.Add("@Section14_6_Condition", SqlDbType.NVarChar).Value = Modal.Section14_6_Condition != null ? Modal.Section14_6_Condition : "";
            command.Parameters.Add("@Section14_6_Comment", SqlDbType.NVarChar).Value = Modal.Section14_6_Comment != null ? Modal.Section14_6_Comment : "";
            command.Parameters.Add("@Section14_7_Condition", SqlDbType.NVarChar).Value = Modal.Section14_7_Condition != null ? Modal.Section14_7_Condition : "";
            command.Parameters.Add("@Section14_7_Comment", SqlDbType.NVarChar).Value = Modal.Section14_7_Comment != null ? Modal.Section14_7_Comment : "";
            command.Parameters.Add("@Section14_8_Condition", SqlDbType.NVarChar).Value = Modal.Section14_8_Condition != null ? Modal.Section14_8_Condition : "";
            command.Parameters.Add("@Section14_8_Comment", SqlDbType.NVarChar).Value = Modal.Section14_8_Comment != null ? Modal.Section14_8_Comment : "";
            command.Parameters.Add("@Section14_9_Condition", SqlDbType.NVarChar).Value = Modal.Section14_9_Condition != null ? Modal.Section14_9_Condition : "";
            command.Parameters.Add("@Section14_9_Comment", SqlDbType.NVarChar).Value = Modal.Section14_9_Comment != null ? Modal.Section14_9_Comment : "";
            command.Parameters.Add("@Section14_10_Condition", SqlDbType.NVarChar).Value = Modal.Section14_10_Condition != null ? Modal.Section14_10_Condition : "";
            command.Parameters.Add("@Section14_10_Comment", SqlDbType.NVarChar).Value = Modal.Section14_10_Comment != null ? Modal.Section14_10_Comment : "";
            command.Parameters.Add("@Section14_11_Condition", SqlDbType.NVarChar).Value = Modal.Section14_11_Condition != null ? Modal.Section14_11_Condition : "";
            command.Parameters.Add("@Section14_11_Comment", SqlDbType.NVarChar).Value = Modal.Section14_11_Comment != null ? Modal.Section14_11_Comment : "";
            command.Parameters.Add("@Section14_12_Condition", SqlDbType.NVarChar).Value = Modal.Section14_12_Condition != null ? Modal.Section14_12_Condition : "";
            command.Parameters.Add("@Section14_12_Comment", SqlDbType.NVarChar).Value = Modal.Section14_12_Comment != null ? Modal.Section14_12_Comment : "";
            command.Parameters.Add("@Section14_13_Condition", SqlDbType.NVarChar).Value = Modal.Section14_13_Condition != null ? Modal.Section14_13_Condition : "";
            command.Parameters.Add("@Section14_13_Comment", SqlDbType.NVarChar).Value = Modal.Section14_13_Comment != null ? Modal.Section14_13_Comment : "";
            command.Parameters.Add("@Section14_14_Condition", SqlDbType.NVarChar).Value = Modal.Section14_14_Condition != null ? Modal.Section14_14_Condition : "";
            command.Parameters.Add("@Section14_14_Comment", SqlDbType.NVarChar).Value = Modal.Section14_14_Comment != null ? Modal.Section14_14_Comment : "";
            command.Parameters.Add("@Section14_15_Condition", SqlDbType.NVarChar).Value = Modal.Section14_15_Condition != null ? Modal.Section14_15_Condition : "";
            command.Parameters.Add("@Section14_15_Comment", SqlDbType.NVarChar).Value = Modal.Section14_15_Comment != null ? Modal.Section14_15_Comment : "";
            command.Parameters.Add("@Section14_16_Condition", SqlDbType.NVarChar).Value = Modal.Section14_16_Condition != null ? Modal.Section14_16_Condition : "";
            command.Parameters.Add("@Section14_16_Comment", SqlDbType.NVarChar).Value = Modal.Section14_16_Comment != null ? Modal.Section14_16_Comment : "";
            command.Parameters.Add("@Section14_17_Condition", SqlDbType.NVarChar).Value = Modal.Section14_17_Condition != null ? Modal.Section14_17_Condition : "";
            command.Parameters.Add("@Section14_17_Comment", SqlDbType.NVarChar).Value = Modal.Section14_17_Comment != null ? Modal.Section14_17_Comment : "";
            command.Parameters.Add("@Section14_18_Condition", SqlDbType.NVarChar).Value = Modal.Section14_18_Condition != null ? Modal.Section14_18_Condition : "";
            command.Parameters.Add("@Section14_18_Comment", SqlDbType.NVarChar).Value = Modal.Section14_18_Comment != null ? Modal.Section14_18_Comment : "";
            command.Parameters.Add("@Section14_19_Condition", SqlDbType.NVarChar).Value = Modal.Section14_19_Condition != null ? Modal.Section14_19_Condition : "";
            command.Parameters.Add("@Section14_19_Comment", SqlDbType.NVarChar).Value = Modal.Section14_19_Comment != null ? Modal.Section14_19_Comment : "";
            command.Parameters.Add("@Section14_20_Condition", SqlDbType.NVarChar).Value = Modal.Section14_20_Condition != null ? Modal.Section14_20_Condition : "";
            command.Parameters.Add("@Section14_20_Comment", SqlDbType.NVarChar).Value = Modal.Section14_20_Comment != null ? Modal.Section14_20_Comment : "";
            command.Parameters.Add("@Section14_21_Condition", SqlDbType.NVarChar).Value = Modal.Section14_21_Condition != null ? Modal.Section14_21_Condition : "";
            command.Parameters.Add("@Section14_21_Comment", SqlDbType.NVarChar).Value = Modal.Section14_21_Comment != null ? Modal.Section14_21_Comment : "";
            command.Parameters.Add("@Section14_22_Condition", SqlDbType.NVarChar).Value = Modal.Section14_22_Condition != null ? Modal.Section14_22_Condition : "";
            command.Parameters.Add("@Section14_22_Comment", SqlDbType.NVarChar).Value = Modal.Section14_22_Comment != null ? Modal.Section14_22_Comment : "";
            command.Parameters.Add("@Section14_23_Condition", SqlDbType.NVarChar).Value = Modal.Section14_23_Condition != null ? Modal.Section14_23_Condition : "";
            command.Parameters.Add("@Section14_23_Comment", SqlDbType.NVarChar).Value = Modal.Section14_23_Comment != null ? Modal.Section14_23_Comment : "";
            command.Parameters.Add("@Section14_24_Condition", SqlDbType.NVarChar).Value = Modal.Section14_24_Condition != null ? Modal.Section14_24_Condition : "";
            command.Parameters.Add("@Section14_24_Comment", SqlDbType.NVarChar).Value = Modal.Section14_24_Comment != null ? Modal.Section14_24_Comment : "";
            command.Parameters.Add("@Section14_25_Condition", SqlDbType.NVarChar).Value = Modal.Section14_25_Condition != null ? Modal.Section14_25_Condition : "";
            command.Parameters.Add("@Section14_25_Comment", SqlDbType.NVarChar).Value = Modal.Section14_25_Comment != null ? Modal.Section14_25_Comment : "";
            command.Parameters.Add("@Section15_1_Condition", SqlDbType.NVarChar).Value = Modal.Section15_1_Condition != null ? Modal.Section15_1_Condition : "";
            command.Parameters.Add("@Section15_1_Comment", SqlDbType.NVarChar).Value = Modal.Section15_1_Comment != null ? Modal.Section15_1_Comment : "";
            command.Parameters.Add("@Section15_2_Condition", SqlDbType.NVarChar).Value = Modal.Section15_2_Condition != null ? Modal.Section15_2_Condition : "";
            command.Parameters.Add("@Section15_2_Comment", SqlDbType.NVarChar).Value = Modal.Section15_2_Comment != null ? Modal.Section15_2_Comment : "";
            command.Parameters.Add("@Section15_3_Condition", SqlDbType.NVarChar).Value = Modal.Section15_3_Condition != null ? Modal.Section15_3_Condition : "";
            command.Parameters.Add("@Section15_3_Comment", SqlDbType.NVarChar).Value = Modal.Section15_3_Comment != null ? Modal.Section15_3_Comment : "";
            command.Parameters.Add("@Section15_4_Condition", SqlDbType.NVarChar).Value = Modal.Section15_4_Condition != null ? Modal.Section15_4_Condition : "";
            command.Parameters.Add("@Section15_4_Comment", SqlDbType.NVarChar).Value = Modal.Section15_4_Comment != null ? Modal.Section15_4_Comment : "";
            command.Parameters.Add("@Section15_5_Condition", SqlDbType.NVarChar).Value = Modal.Section15_5_Condition != null ? Modal.Section15_5_Condition : "";
            command.Parameters.Add("@Section15_5_Comment", SqlDbType.NVarChar).Value = Modal.Section15_5_Comment != null ? Modal.Section15_5_Comment : "";
            command.Parameters.Add("@Section15_6_Condition", SqlDbType.NVarChar).Value = Modal.Section15_6_Condition != null ? Modal.Section15_6_Condition : "";
            command.Parameters.Add("@Section15_6_Comment", SqlDbType.NVarChar).Value = Modal.Section15_6_Comment != null ? Modal.Section15_6_Comment : "";
            command.Parameters.Add("@Section15_7_Condition", SqlDbType.NVarChar).Value = Modal.Section15_7_Condition != null ? Modal.Section15_7_Condition : "";
            command.Parameters.Add("@Section15_7_Comment", SqlDbType.NVarChar).Value = Modal.Section15_7_Comment != null ? Modal.Section15_7_Comment : "";
            command.Parameters.Add("@Section15_8_Condition", SqlDbType.NVarChar).Value = Modal.Section15_8_Condition != null ? Modal.Section15_8_Condition : "";
            command.Parameters.Add("@Section15_8_Comment", SqlDbType.NVarChar).Value = Modal.Section15_8_Comment != null ? Modal.Section15_8_Comment : "";
            command.Parameters.Add("@Section15_9_Condition", SqlDbType.NVarChar).Value = Modal.Section15_9_Condition != null ? Modal.Section15_9_Condition : "";
            command.Parameters.Add("@Section15_9_Comment", SqlDbType.NVarChar).Value = Modal.Section15_9_Comment != null ? Modal.Section15_9_Comment : "";
            command.Parameters.Add("@Section15_10_Condition", SqlDbType.NVarChar).Value = Modal.Section15_10_Condition != null ? Modal.Section15_10_Condition : "";
            command.Parameters.Add("@Section15_10_Comment", SqlDbType.NVarChar).Value = Modal.Section15_10_Comment != null ? Modal.Section15_10_Comment : "";
            command.Parameters.Add("@Section15_11_Condition", SqlDbType.NVarChar).Value = Modal.Section15_11_Condition != null ? Modal.Section15_11_Condition : "";
            command.Parameters.Add("@Section15_11_Comment", SqlDbType.NVarChar).Value = Modal.Section15_11_Comment != null ? Modal.Section15_11_Comment : "";
            command.Parameters.Add("@Section15_12_Condition", SqlDbType.NVarChar).Value = Modal.Section15_12_Condition != null ? Modal.Section15_12_Condition : "";
            command.Parameters.Add("@Section15_12_Comment", SqlDbType.NVarChar).Value = Modal.Section15_12_Comment != null ? Modal.Section15_12_Comment : "";
            command.Parameters.Add("@Section15_13_Condition", SqlDbType.NVarChar).Value = Modal.Section15_13_Condition != null ? Modal.Section15_13_Condition : "";
            command.Parameters.Add("@Section15_13_Comment", SqlDbType.NVarChar).Value = Modal.Section15_13_Comment != null ? Modal.Section15_13_Comment : "";
            command.Parameters.Add("@Section15_14_Condition", SqlDbType.NVarChar).Value = Modal.Section15_14_Condition != null ? Modal.Section15_14_Condition : "";
            command.Parameters.Add("@Section15_14_Comment", SqlDbType.NVarChar).Value = Modal.Section15_14_Comment != null ? Modal.Section15_14_Comment : "";
            command.Parameters.Add("@Section15_15_Condition", SqlDbType.NVarChar).Value = Modal.Section15_15_Condition != null ? Modal.Section15_15_Condition : "";
            command.Parameters.Add("@Section15_15_Comment", SqlDbType.NVarChar).Value = Modal.Section15_15_Comment != null ? Modal.Section15_15_Comment : "";
            command.Parameters.Add("@Section16_1_Condition", SqlDbType.NVarChar).Value = Modal.Section16_1_Condition != null ? Modal.Section16_1_Condition : "";
            command.Parameters.Add("@Section16_1_Comment", SqlDbType.NVarChar).Value = Modal.Section16_1_Comment != null ? Modal.Section16_1_Comment : "";
            command.Parameters.Add("@Section16_2_Condition", SqlDbType.NVarChar).Value = Modal.Section16_2_Condition != null ? Modal.Section16_2_Condition : "";
            command.Parameters.Add("@Section16_2_Comment", SqlDbType.NVarChar).Value = Modal.Section16_2_Comment != null ? Modal.Section16_2_Comment : "";
            command.Parameters.Add("@Section16_3_Condition", SqlDbType.NVarChar).Value = Modal.Section16_3_Condition != null ? Modal.Section16_3_Condition : "";
            command.Parameters.Add("@Section16_3_Comment", SqlDbType.NVarChar).Value = Modal.Section16_3_Comment != null ? Modal.Section16_3_Comment : "";
            command.Parameters.Add("@Section16_4_Condition", SqlDbType.NVarChar).Value = Modal.Section16_4_Condition != null ? Modal.Section16_4_Condition : "";
            command.Parameters.Add("@Section16_4_Comment", SqlDbType.NVarChar).Value = Modal.Section16_4_Comment != null ? Modal.Section16_4_Comment : "";
            command.Parameters.Add("@Section17_1_Condition", SqlDbType.NVarChar).Value = Modal.Section17_1_Condition != null ? Modal.Section17_1_Condition : "";
            command.Parameters.Add("@Section17_1_Comment", SqlDbType.NVarChar).Value = Modal.Section17_1_Comment != null ? Modal.Section17_1_Comment : "";
            command.Parameters.Add("@Section17_2_Condition", SqlDbType.NVarChar).Value = Modal.Section17_2_Condition != null ? Modal.Section17_2_Condition : "";
            command.Parameters.Add("@Section17_2_Comment", SqlDbType.NVarChar).Value = Modal.Section17_2_Comment != null ? Modal.Section17_2_Comment : "";
            command.Parameters.Add("@Section17_3_Condition", SqlDbType.NVarChar).Value = Modal.Section17_3_Condition != null ? Modal.Section17_3_Condition : "";
            command.Parameters.Add("@Section17_3_Comment", SqlDbType.NVarChar).Value = Modal.Section17_3_Comment != null ? Modal.Section17_3_Comment : "";
            command.Parameters.Add("@Section17_4_Condition", SqlDbType.NVarChar).Value = Modal.Section17_4_Condition != null ? Modal.Section17_4_Condition : "";
            command.Parameters.Add("@Section17_4_Comment", SqlDbType.NVarChar).Value = Modal.Section17_4_Comment != null ? Modal.Section17_4_Comment : "";
            command.Parameters.Add("@Section17_5_Condition", SqlDbType.NVarChar).Value = Modal.Section17_5_Condition != null ? Modal.Section17_5_Condition : "";
            command.Parameters.Add("@Section17_5_Comment", SqlDbType.NVarChar).Value = Modal.Section17_5_Comment != null ? Modal.Section17_5_Comment : "";
            command.Parameters.Add("@Section17_6_Condition", SqlDbType.NVarChar).Value = Modal.Section17_6_Condition != null ? Modal.Section17_6_Condition : "";
            command.Parameters.Add("@Section17_6_Comment", SqlDbType.NVarChar).Value = Modal.Section17_6_Comment != null ? Modal.Section17_6_Comment : "";
            command.Parameters.Add("@Section18_1_Condition", SqlDbType.NVarChar).Value = Modal.Section18_1_Condition != null ? Modal.Section18_1_Condition : "";
            command.Parameters.Add("@Section18_1_Comment", SqlDbType.NVarChar).Value = Modal.Section18_1_Comment != null ? Modal.Section18_1_Comment : "";
            command.Parameters.Add("@Section18_2_Condition", SqlDbType.NVarChar).Value = Modal.Section18_2_Condition != null ? Modal.Section18_2_Condition : "";
            command.Parameters.Add("@Section18_2_Comment", SqlDbType.NVarChar).Value = Modal.Section18_2_Comment != null ? Modal.Section18_2_Comment : "";
            command.Parameters.Add("@Section18_3_Condition", SqlDbType.NVarChar).Value = Modal.Section18_3_Condition != null ? Modal.Section18_3_Condition : "";
            command.Parameters.Add("@Section18_3_Comment", SqlDbType.NVarChar).Value = Modal.Section18_3_Comment != null ? Modal.Section18_3_Comment : "";
            command.Parameters.Add("@Section18_4_Condition", SqlDbType.NVarChar).Value = Modal.Section18_4_Condition != null ? Modal.Section18_4_Condition : "";
            command.Parameters.Add("@Section18_4_Comment", SqlDbType.NVarChar).Value = Modal.Section18_4_Comment != null ? Modal.Section18_4_Comment : "";
            command.Parameters.Add("@Section18_5_Condition", SqlDbType.NVarChar).Value = Modal.Section18_5_Condition != null ? Modal.Section18_5_Condition : "";
            command.Parameters.Add("@Section18_5_Comment", SqlDbType.NVarChar).Value = Modal.Section18_5_Comment != null ? Modal.Section18_5_Comment : "";
            command.Parameters.Add("@Section18_6_Condition", SqlDbType.NVarChar).Value = Modal.Section18_6_Condition != null ? Modal.Section18_6_Condition : "";
            command.Parameters.Add("@Section18_6_Comment", SqlDbType.NVarChar).Value = Modal.Section18_6_Comment != null ? Modal.Section18_6_Comment : "";
            command.Parameters.Add("@Section18_7_Condition", SqlDbType.NVarChar).Value = Modal.Section18_7_Condition != null ? Modal.Section18_7_Condition : "";
            command.Parameters.Add("@Section18_7_Comment", SqlDbType.NVarChar).Value = Modal.Section18_7_Comment != null ? Modal.Section18_7_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section18_8_Condition", SqlDbType.NVarChar).Value = Modal.Section18_8_Condition != null ? Modal.Section18_8_Condition : "";
            command.Parameters.Add("@Section18_8_Comment", SqlDbType.NVarChar).Value = Modal.Section18_8_Comment != null ? Modal.Section18_8_Comment : "";
            command.Parameters.Add("@Section18_9_Condition", SqlDbType.NVarChar).Value = Modal.Section18_9_Condition != null ? Modal.Section18_9_Condition : "";
            command.Parameters.Add("@Section18_9_Comment", SqlDbType.NVarChar).Value = Modal.Section18_9_Comment != null ? Modal.Section18_9_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Modal.CreatedDate != null ? Modal.CreatedDate : Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@ModifyDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime //Modal.ModifyDate != null ? Modal.ModifyDate : null;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft ?? (object)DBNull.Value;
        }

        public void SIRUpdateCMD(SuperintendedInspectionReport Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@ShipID", SqlDbType.Int).Value = Modal.ShipID ?? (object)DBNull.Value;
            command.Parameters.Add("@ShipName", SqlDbType.NVarChar).Value = Modal.ShipName;
            command.Parameters.Add("@Date", SqlDbType.DateTime).Value = Modal.Date != null ? Modal.Date : null;
            command.Parameters.Add("@Port", SqlDbType.NVarChar).Value = Modal.Port != null ? Modal.Port : null;
            command.Parameters.Add("@Master", SqlDbType.NVarChar).Value = Modal.Master != null ? Modal.Master : "";
            command.Parameters.Add("@Superintended", SqlDbType.NVarChar).Value = Modal.Superintended != null ? Modal.Superintended : "";
            command.Parameters.Add("@Section1_1_Condition", SqlDbType.NVarChar).Value = Modal.Section1_1_Condition != null ? Modal.Section1_1_Condition : "";
            command.Parameters.Add("@Section1_1_Comment", SqlDbType.NVarChar).Value = Modal.Section1_1_Comment != null ? Modal.Section1_1_Comment : "";
            command.Parameters.Add("@Section1_2_Condition", SqlDbType.NVarChar).Value = Modal.Section1_2_Condition != null ? Modal.Section1_2_Condition : "";
            command.Parameters.Add("@Section1_2_Comment", SqlDbType.NVarChar).Value = Modal.Section1_2_Comment != null ? Modal.Section1_2_Comment : "";
            command.Parameters.Add("@Section1_3_Condition", SqlDbType.NVarChar).Value = Modal.Section1_3_Condition != null ? Modal.Section1_3_Condition : "";
            command.Parameters.Add("@Section1_3_Comment", SqlDbType.NVarChar).Value = Modal.Section1_3_Comment != null ? Modal.Section1_3_Comment : "";
            command.Parameters.Add("@Section1_4_Condition", SqlDbType.NVarChar).Value = Modal.Section1_4_Condition != null ? Modal.Section1_4_Condition : "";
            command.Parameters.Add("@Section1_4_Comment", SqlDbType.NVarChar).Value = Modal.Section1_4_Comment != null ? Modal.Section1_4_Comment : "";
            command.Parameters.Add("@Section1_5_Condition", SqlDbType.NVarChar).Value = Modal.Section1_5_Condition != null ? Modal.Section1_5_Condition : "";
            command.Parameters.Add("@Section1_5_Comment", SqlDbType.NVarChar).Value = Modal.Section1_5_Comment != null ? Modal.Section1_5_Comment : "";
            command.Parameters.Add("@Section1_6_Condition", SqlDbType.NVarChar).Value = Modal.Section1_6_Condition != null ? Modal.Section1_6_Condition : "";
            command.Parameters.Add("@Section1_6_Comment", SqlDbType.NVarChar).Value = Modal.Section1_6_Comment != null ? Modal.Section1_6_Comment : "";
            command.Parameters.Add("@Section1_7_Condition", SqlDbType.NVarChar).Value = Modal.Section1_7_Condition != null ? Modal.Section1_7_Condition : "";
            command.Parameters.Add("@Section1_7_Comment", SqlDbType.NVarChar).Value = Modal.Section1_7_Comment != null ? Modal.Section1_7_Comment : "";
            command.Parameters.Add("@Section1_8_Condition", SqlDbType.NVarChar).Value = Modal.Section1_8_Condition != null ? Modal.Section1_8_Condition : "";
            command.Parameters.Add("@Section1_8_Comment", SqlDbType.NVarChar).Value = Modal.Section1_8_Comment != null ? Modal.Section1_8_Comment : "";
            command.Parameters.Add("@Section1_9_Condition", SqlDbType.NVarChar).Value = Modal.Section1_9_Condition != null ? Modal.Section1_9_Condition : "";
            command.Parameters.Add("@Section1_9_Comment", SqlDbType.NVarChar).Value = Modal.Section1_9_Comment != null ? Modal.Section1_9_Comment : "";
            command.Parameters.Add("@Section1_10_Condition", SqlDbType.NVarChar).Value = Modal.Section1_10_Condition != null ? Modal.Section1_10_Condition : "";
            command.Parameters.Add("@Section1_10_Comment", SqlDbType.NVarChar).Value = Modal.Section1_10_Comment != null ? Modal.Section1_10_Comment : "";
            command.Parameters.Add("@Section1_11_Condition", SqlDbType.NVarChar).Value = Modal.Section1_11_Condition != null ? Modal.Section1_11_Condition : "";
            command.Parameters.Add("@Section1_11_Comment", SqlDbType.NVarChar).Value = Modal.Section1_11_Comment != null ? Modal.Section1_11_Comment : "";
            command.Parameters.Add("@Section2_1_Condition", SqlDbType.NVarChar).Value = Modal.Section2_1_Condition != null ? Modal.Section2_1_Condition : "";
            command.Parameters.Add("@Section2_1_Comment", SqlDbType.NVarChar).Value = Modal.Section2_1_Comment != null ? Modal.Section2_1_Comment : "";
            command.Parameters.Add("@Section2_2_Condition", SqlDbType.NVarChar).Value = Modal.Section2_2_Condition != null ? Modal.Section2_2_Condition : "";
            command.Parameters.Add("@Section2_2_Comment", SqlDbType.NVarChar).Value = Modal.Section2_2_Comment != null ? Modal.Section2_2_Comment : "";
            command.Parameters.Add("@Section2_3_Condition", SqlDbType.NVarChar).Value = Modal.Section2_3_Condition != null ? Modal.Section2_3_Condition : "";
            command.Parameters.Add("@Section2_3_Comment", SqlDbType.NVarChar).Value = Modal.Section2_3_Comment != null ? Modal.Section2_3_Comment : "";
            command.Parameters.Add("@Section2_4_Condition", SqlDbType.NVarChar).Value = Modal.Section2_4_Condition != null ? Modal.Section2_4_Condition : "";
            command.Parameters.Add("@Section2_4_Comment", SqlDbType.NVarChar).Value = Modal.Section2_4_Comment != null ? Modal.Section2_4_Comment : "";
            command.Parameters.Add("@Section2_5_Condition", SqlDbType.NVarChar).Value = Modal.Section2_5_Condition != null ? Modal.Section2_5_Condition : "";
            command.Parameters.Add("@Section2_5_Comment", SqlDbType.NVarChar).Value = Modal.Section2_5_Comment != null ? Modal.Section2_5_Comment : "";
            command.Parameters.Add("@Section2_6_Condition", SqlDbType.NVarChar).Value = Modal.Section2_6_Condition != null ? Modal.Section2_6_Condition : "";
            command.Parameters.Add("@Section2_6_Comment", SqlDbType.NVarChar).Value = Modal.Section2_6_Comment != null ? Modal.Section2_6_Comment : "";
            command.Parameters.Add("@Section2_7_Condition", SqlDbType.NVarChar).Value = Modal.Section2_7_Condition != null ? Modal.Section2_7_Condition : "";
            command.Parameters.Add("@Section2_7_Comment", SqlDbType.NVarChar).Value = Modal.Section2_7_Comment != null ? Modal.Section2_7_Comment : "";
            command.Parameters.Add("@Section3_1_Condition", SqlDbType.NVarChar).Value = Modal.Section3_1_Condition != null ? Modal.Section3_1_Condition : "";
            command.Parameters.Add("@Section3_1_Comment", SqlDbType.NVarChar).Value = Modal.Section3_1_Comment != null ? Modal.Section3_1_Comment : "";
            command.Parameters.Add("@Section3_2_Condition", SqlDbType.NVarChar).Value = Modal.Section3_2_Condition != null ? Modal.Section3_2_Condition : "";
            command.Parameters.Add("@Section3_2_Comment", SqlDbType.NVarChar).Value = Modal.Section3_2_Comment != null ? Modal.Section3_2_Comment : "";
            command.Parameters.Add("@Section3_3_Condition", SqlDbType.NVarChar).Value = Modal.Section3_3_Condition != null ? Modal.Section3_3_Condition : "";
            command.Parameters.Add("@Section3_3_Comment", SqlDbType.NVarChar).Value = Modal.Section3_3_Comment != null ? Modal.Section3_3_Comment : "";
            command.Parameters.Add("@Section3_4_Condition", SqlDbType.NVarChar).Value = Modal.Section3_4_Condition != null ? Modal.Section3_4_Condition : "";
            command.Parameters.Add("@Section3_4_Comment", SqlDbType.NVarChar).Value = Modal.Section3_4_Comment != null ? Modal.Section3_4_Comment : "";
            command.Parameters.Add("@Section3_5_Condition", SqlDbType.NVarChar).Value = Modal.Section3_5_Condition != null ? Modal.Section3_5_Condition : "";
            command.Parameters.Add("@Section3_5_Comment", SqlDbType.NVarChar).Value = Modal.Section3_5_Comment != null ? Modal.Section3_5_Comment : "";
            command.Parameters.Add("@Section4_1_Condition", SqlDbType.NVarChar).Value = Modal.Section4_1_Condition != null ? Modal.Section4_1_Condition : "";
            command.Parameters.Add("@Section4_1_Comment", SqlDbType.NVarChar).Value = Modal.Section4_1_Comment != null ? Modal.Section4_1_Comment : "";
            command.Parameters.Add("@Section4_2_Condition", SqlDbType.NVarChar).Value = Modal.Section4_2_Condition != null ? Modal.Section4_2_Condition : "";
            command.Parameters.Add("@Section4_2_Comment", SqlDbType.NVarChar).Value = Modal.Section4_2_Comment != null ? Modal.Section4_2_Comment : "";
            command.Parameters.Add("@Section4_3_Condition", SqlDbType.NVarChar).Value = Modal.Section4_3_Condition != null ? Modal.Section4_3_Condition : "";
            command.Parameters.Add("@Section4_3_Comment", SqlDbType.NVarChar).Value = Modal.Section4_3_Comment != null ? Modal.Section4_3_Comment : "";
            command.Parameters.Add("@Section5_1_Condition", SqlDbType.NVarChar).Value = Modal.Section5_1_Condition != null ? Modal.Section5_1_Condition : "";
            command.Parameters.Add("@Section5_1_Comment", SqlDbType.NVarChar).Value = Modal.Section5_1_Comment != null ? Modal.Section5_1_Comment : "";
            command.Parameters.Add("@Section5_6_Condition", SqlDbType.NVarChar).Value = Modal.Section5_6_Condition != null ? Modal.Section5_6_Condition : "";
            command.Parameters.Add("@Section5_6_Comment", SqlDbType.NVarChar).Value = Modal.Section5_6_Comment != null ? Modal.Section5_6_Comment : "";
            command.Parameters.Add("@Section5_8_Condition", SqlDbType.NVarChar).Value = Modal.Section5_8_Condition != null ? Modal.Section5_8_Condition : "";
            command.Parameters.Add("@Section5_8_Comment", SqlDbType.NVarChar).Value = Modal.Section5_8_Comment != null ? Modal.Section5_8_Comment : "";
            command.Parameters.Add("@Section5_9_Condition", SqlDbType.NVarChar).Value = Modal.Section5_9_Condition != null ? Modal.Section5_9_Condition : "";
            command.Parameters.Add("@Section5_9_Comment", SqlDbType.NVarChar).Value = Modal.Section5_9_Comment != null ? Modal.Section5_9_Comment : "";
            command.Parameters.Add("@Section6_1_Condition", SqlDbType.NVarChar).Value = Modal.Section6_1_Condition != null ? Modal.Section6_1_Condition : "";
            command.Parameters.Add("@Section6_1_Comment", SqlDbType.NVarChar).Value = Modal.Section6_1_Comment != null ? Modal.Section6_1_Comment : "";
            command.Parameters.Add("@Section6_2_Condition", SqlDbType.NVarChar).Value = Modal.Section6_2_Condition != null ? Modal.Section6_2_Condition : "";
            command.Parameters.Add("@Section6_2_Comment", SqlDbType.NVarChar).Value = Modal.Section6_2_Comment != null ? Modal.Section6_2_Comment : "";
            command.Parameters.Add("@Section6_3_Condition", SqlDbType.NVarChar).Value = Modal.Section6_3_Condition != null ? Modal.Section6_3_Condition : "";
            command.Parameters.Add("@Section6_3_Comment", SqlDbType.NVarChar).Value = Modal.Section6_3_Comment != null ? Modal.Section6_3_Comment : "";
            command.Parameters.Add("@Section6_4_Condition", SqlDbType.NVarChar).Value = Modal.Section6_4_Condition != null ? Modal.Section6_4_Condition : "";
            command.Parameters.Add("@Section6_4_Comment", SqlDbType.NVarChar).Value = Modal.Section6_4_Comment != null ? Modal.Section6_4_Comment : "";
            command.Parameters.Add("@Section6_5_Condition", SqlDbType.NVarChar).Value = Modal.Section6_5_Condition != null ? Modal.Section6_5_Condition : "";
            command.Parameters.Add("@Section6_5_Comment", SqlDbType.NVarChar).Value = Modal.Section6_5_Comment != null ? Modal.Section6_5_Comment : "";
            command.Parameters.Add("@Section6_6_Condition", SqlDbType.NVarChar).Value = Modal.Section6_6_Condition != null ? Modal.Section6_6_Condition : "";
            command.Parameters.Add("@Section6_6_Comment", SqlDbType.NVarChar).Value = Modal.Section6_6_Comment != null ? Modal.Section6_6_Comment : "";
            command.Parameters.Add("@Section6_7_Condition", SqlDbType.NVarChar).Value = Modal.Section6_7_Condition != null ? Modal.Section6_7_Condition : "";
            command.Parameters.Add("@Section6_7_Comment", SqlDbType.NVarChar).Value = Modal.Section6_7_Comment != null ? Modal.Section6_7_Comment : "";
            command.Parameters.Add("@Section6_8_Condition", SqlDbType.NVarChar).Value = Modal.Section6_8_Condition != null ? Modal.Section6_8_Condition : "";
            command.Parameters.Add("@Section6_8_Comment", SqlDbType.NVarChar).Value = Modal.Section6_8_Comment != null ? Modal.Section6_8_Comment : "";
            command.Parameters.Add("@Section7_1_Condition", SqlDbType.NVarChar).Value = Modal.Section7_1_Condition != null ? Modal.Section7_1_Condition : "";
            command.Parameters.Add("@Section7_1_Comment", SqlDbType.NVarChar).Value = Modal.Section7_1_Comment != null ? Modal.Section7_1_Comment : "";
            command.Parameters.Add("@Section7_2_Condition", SqlDbType.NVarChar).Value = Modal.Section7_2_Condition != null ? Modal.Section7_2_Condition : "";
            command.Parameters.Add("@Section7_2_Comment", SqlDbType.NVarChar).Value = Modal.Section7_2_Comment != null ? Modal.Section7_2_Comment : "";
            command.Parameters.Add("@Section7_3_Condition", SqlDbType.NVarChar).Value = Modal.Section7_3_Condition != null ? Modal.Section7_3_Condition : "";
            command.Parameters.Add("@Section7_3_Comment", SqlDbType.NVarChar).Value = Modal.Section7_3_Comment != null ? Modal.Section7_3_Comment : "";
            command.Parameters.Add("@Section7_4_Condition", SqlDbType.NVarChar).Value = Modal.Section7_4_Condition != null ? Modal.Section7_4_Condition : "";
            command.Parameters.Add("@Section7_4_Comment", SqlDbType.NVarChar).Value = Modal.Section7_4_Comment != null ? Modal.Section7_4_Comment : "";
            command.Parameters.Add("@Section7_5_Condition", SqlDbType.NVarChar).Value = Modal.Section7_5_Condition != null ? Modal.Section7_5_Condition : "";
            command.Parameters.Add("@Section7_5_Comment", SqlDbType.NVarChar).Value = Modal.Section7_5_Comment != null ? Modal.Section7_5_Comment : "";
            command.Parameters.Add("@Section7_6_Condition", SqlDbType.NVarChar).Value = Modal.Section7_6_Condition != null ? Modal.Section7_6_Condition : "";
            command.Parameters.Add("@Section7_6_Comment", SqlDbType.NVarChar).Value = Modal.Section7_6_Comment != null ? Modal.Section7_6_Comment : "";
            command.Parameters.Add("@Section8_1_Condition", SqlDbType.NVarChar).Value = Modal.Section8_1_Condition != null ? Modal.Section8_1_Condition : "";
            command.Parameters.Add("@Section8_1_Comment", SqlDbType.NVarChar).Value = Modal.Section8_1_Comment != null ? Modal.Section8_1_Comment : "";
            command.Parameters.Add("@Section8_2_Condition", SqlDbType.NVarChar).Value = Modal.Section8_2_Condition != null ? Modal.Section8_2_Condition : "";
            command.Parameters.Add("@Section8_2_Comment", SqlDbType.NVarChar).Value = Modal.Section8_2_Comment != null ? Modal.Section8_2_Comment : "";
            command.Parameters.Add("@Section8_3_Condition", SqlDbType.NVarChar).Value = Modal.Section8_3_Condition != null ? Modal.Section8_3_Condition : "";
            command.Parameters.Add("@Section8_3_Comment", SqlDbType.NVarChar).Value = Modal.Section8_3_Comment != null ? Modal.Section8_3_Comment : "";
            command.Parameters.Add("@Section8_4_Condition", SqlDbType.NVarChar).Value = Modal.Section8_4_Condition != null ? Modal.Section8_4_Condition : "";
            command.Parameters.Add("@Section8_4_Comment", SqlDbType.NVarChar).Value = Modal.Section8_4_Comment != null ? Modal.Section8_4_Comment : "";
            command.Parameters.Add("@Section8_5_Condition", SqlDbType.NVarChar).Value = Modal.Section8_5_Condition != null ? Modal.Section8_5_Condition : "";
            command.Parameters.Add("@Section8_5_Comment", SqlDbType.NVarChar).Value = Modal.Section8_5_Comment != null ? Modal.Section8_5_Comment : "";
            command.Parameters.Add("@Section8_6_Condition", SqlDbType.NVarChar).Value = Modal.Section8_6_Condition != null ? Modal.Section8_6_Condition : "";
            command.Parameters.Add("@Section8_6_Comment", SqlDbType.NVarChar).Value = Modal.Section8_6_Comment != null ? Modal.Section8_6_Comment : "";
            command.Parameters.Add("@Section8_7_Condition", SqlDbType.NVarChar).Value = Modal.Section8_7_Condition != null ? Modal.Section8_7_Condition : "";
            command.Parameters.Add("@Section8_7_Comment", SqlDbType.NVarChar).Value = Modal.Section8_7_Comment != null ? Modal.Section8_7_Comment : "";
            command.Parameters.Add("@Section8_8_Condition", SqlDbType.NVarChar).Value = Modal.Section8_8_Condition != null ? Modal.Section8_8_Condition : "";
            command.Parameters.Add("@Section8_8_Comment", SqlDbType.NVarChar).Value = Modal.Section8_8_Comment != null ? Modal.Section8_8_Comment : "";
            command.Parameters.Add("@Section8_9_Condition", SqlDbType.NVarChar).Value = Modal.Section8_9_Condition != null ? Modal.Section8_9_Condition : "";
            command.Parameters.Add("@Section8_9_Comment", SqlDbType.NVarChar).Value = Modal.Section8_9_Comment != null ? Modal.Section8_9_Comment : "";
            command.Parameters.Add("@Section8_10_Condition", SqlDbType.NVarChar).Value = Modal.Section8_10_Condition != null ? Modal.Section8_10_Condition : "";
            command.Parameters.Add("@Section8_10_Comment", SqlDbType.NVarChar).Value = Modal.Section8_10_Comment != null ? Modal.Section8_10_Comment : "";
            command.Parameters.Add("@Section8_11_Condition", SqlDbType.NVarChar).Value = Modal.Section8_11_Condition != null ? Modal.Section8_11_Condition : "";
            command.Parameters.Add("@Section8_11_Comment", SqlDbType.NVarChar).Value = Modal.Section8_11_Comment != null ? Modal.Section8_11_Comment : "";
            command.Parameters.Add("@Section8_12_Condition", SqlDbType.NVarChar).Value = Modal.Section8_12_Condition != null ? Modal.Section8_12_Condition : "";
            command.Parameters.Add("@Section8_12_Comment", SqlDbType.NVarChar).Value = Modal.Section8_12_Comment != null ? Modal.Section8_12_Comment : "";
            command.Parameters.Add("@Section8_13_Condition", SqlDbType.NVarChar).Value = Modal.Section8_13_Condition != null ? Modal.Section8_13_Condition : "";
            command.Parameters.Add("@Section8_13_Comment", SqlDbType.NVarChar).Value = Modal.Section8_13_Comment != null ? Modal.Section8_13_Comment : "";
            command.Parameters.Add("@Section8_14_Condition", SqlDbType.NVarChar).Value = Modal.Section8_14_Condition != null ? Modal.Section8_14_Condition : "";
            command.Parameters.Add("@Section8_14_Comment", SqlDbType.NVarChar).Value = Modal.Section8_14_Comment != null ? Modal.Section8_14_Comment : "";
            command.Parameters.Add("@Section8_15_Condition", SqlDbType.NVarChar).Value = Modal.Section8_15_Condition != null ? Modal.Section8_15_Condition : "";
            command.Parameters.Add("@Section8_15_Comment", SqlDbType.NVarChar).Value = Modal.Section8_15_Comment != null ? Modal.Section8_15_Comment : "";
            command.Parameters.Add("@Section8_16_Condition", SqlDbType.NVarChar).Value = Modal.Section8_16_Condition != null ? Modal.Section8_16_Condition : "";
            command.Parameters.Add("@Section8_16_Comment", SqlDbType.NVarChar).Value = Modal.Section8_16_Comment != null ? Modal.Section8_16_Comment : "";
            command.Parameters.Add("@Section8_17_Condition", SqlDbType.NVarChar).Value = Modal.Section8_17_Condition != null ? Modal.Section8_17_Condition : "";
            command.Parameters.Add("@Section8_17_Comment", SqlDbType.NVarChar).Value = Modal.Section8_17_Comment != null ? Modal.Section8_17_Comment : "";
            command.Parameters.Add("@Section8_18_Condition", SqlDbType.NVarChar).Value = Modal.Section8_18_Condition != null ? Modal.Section8_18_Condition : "";
            command.Parameters.Add("@Section8_18_Comment", SqlDbType.NVarChar).Value = Modal.Section8_18_Comment != null ? Modal.Section8_18_Comment : "";
            command.Parameters.Add("@Section8_19_Condition", SqlDbType.NVarChar).Value = Modal.Section8_19_Condition != null ? Modal.Section8_19_Condition : "";
            command.Parameters.Add("@Section8_19_Comment", SqlDbType.NVarChar).Value = Modal.Section8_19_Comment != null ? Modal.Section8_19_Comment : "";
            command.Parameters.Add("@Section8_20_Condition", SqlDbType.NVarChar).Value = Modal.Section8_20_Condition != null ? Modal.Section8_20_Condition : "";
            command.Parameters.Add("@Section8_20_Comment", SqlDbType.NVarChar).Value = Modal.Section8_20_Comment != null ? Modal.Section8_20_Comment : "";
            command.Parameters.Add("@Section8_21_Condition", SqlDbType.NVarChar).Value = Modal.Section8_21_Condition != null ? Modal.Section8_21_Condition : "";
            command.Parameters.Add("@Section8_21_Comment", SqlDbType.NVarChar).Value = Modal.Section8_21_Comment != null ? Modal.Section8_21_Comment : "";
            command.Parameters.Add("@Section8_22_Condition", SqlDbType.NVarChar).Value = Modal.Section8_22_Condition != null ? Modal.Section8_22_Condition : "";
            command.Parameters.Add("@Section8_22_Comment", SqlDbType.NVarChar).Value = Modal.Section8_22_Comment != null ? Modal.Section8_22_Comment : "";
            command.Parameters.Add("@Section8_23_Condition", SqlDbType.NVarChar).Value = Modal.Section8_23_Condition != null ? Modal.Section8_23_Condition : "";
            command.Parameters.Add("@Section8_23_Comment", SqlDbType.NVarChar).Value = Modal.Section8_23_Comment != null ? Modal.Section8_23_Comment : "";
            command.Parameters.Add("@Section8_24_Condition", SqlDbType.NVarChar).Value = Modal.Section8_24_Condition != null ? Modal.Section8_24_Condition : "";
            command.Parameters.Add("@Section8_24_Comment", SqlDbType.NVarChar).Value = Modal.Section8_24_Comment != null ? Modal.Section8_24_Comment : "";
            command.Parameters.Add("@Section8_25_Condition", SqlDbType.NVarChar).Value = Modal.Section8_25_Condition != null ? Modal.Section8_25_Condition : "";
            command.Parameters.Add("@Section8_25_Comment", SqlDbType.NVarChar).Value = Modal.Section8_25_Comment != null ? Modal.Section8_25_Comment : "";
            command.Parameters.Add("@Section9_1_Condition", SqlDbType.NVarChar).Value = Modal.Section9_1_Condition != null ? Modal.Section9_1_Condition : "";
            command.Parameters.Add("@Section9_1_Comment", SqlDbType.NVarChar).Value = Modal.Section9_1_Comment != null ? Modal.Section9_1_Comment : "";
            command.Parameters.Add("@Section9_2_Condition", SqlDbType.NVarChar).Value = Modal.Section9_2_Condition != null ? Modal.Section9_2_Condition : "";
            command.Parameters.Add("@Section9_2_Comment", SqlDbType.NVarChar).Value = Modal.Section9_2_Comment != null ? Modal.Section9_2_Comment : "";
            command.Parameters.Add("@Section9_3_Condition", SqlDbType.NVarChar).Value = Modal.Section9_3_Condition != null ? Modal.Section9_3_Condition : "";
            command.Parameters.Add("@Section9_3_Comment", SqlDbType.NVarChar).Value = Modal.Section9_3_Comment != null ? Modal.Section9_3_Comment : "";
            command.Parameters.Add("@Section9_4_Condition", SqlDbType.NVarChar).Value = Modal.Section9_4_Condition != null ? Modal.Section9_4_Condition : "";
            command.Parameters.Add("@Section9_4_Comment", SqlDbType.NVarChar).Value = Modal.Section9_4_Comment != null ? Modal.Section9_4_Comment : "";
            command.Parameters.Add("@Section9_5_Condition", SqlDbType.NVarChar).Value = Modal.Section9_5_Condition != null ? Modal.Section9_5_Condition : "";
            command.Parameters.Add("@Section9_5_Comment", SqlDbType.NVarChar).Value = Modal.Section9_5_Comment != null ? Modal.Section9_5_Comment : "";
            command.Parameters.Add("@Section9_6_Condition", SqlDbType.NVarChar).Value = Modal.Section9_6_Condition != null ? Modal.Section9_6_Condition : "";
            command.Parameters.Add("@Section9_6_Comment", SqlDbType.NVarChar).Value = Modal.Section9_6_Comment != null ? Modal.Section9_6_Comment : "";
            command.Parameters.Add("@Section9_7_Condition", SqlDbType.NVarChar).Value = Modal.Section9_7_Condition != null ? Modal.Section9_7_Condition : "";
            command.Parameters.Add("@Section9_7_Comment", SqlDbType.NVarChar).Value = Modal.Section9_7_Comment != null ? Modal.Section9_7_Comment : "";
            command.Parameters.Add("@Section9_8_Condition", SqlDbType.NVarChar).Value = Modal.Section9_8_Condition != null ? Modal.Section9_8_Condition : "";
            command.Parameters.Add("@Section9_8_Comment", SqlDbType.NVarChar).Value = Modal.Section9_8_Comment != null ? Modal.Section9_8_Comment : "";
            command.Parameters.Add("@Section9_9_Condition", SqlDbType.NVarChar).Value = Modal.Section9_9_Condition != null ? Modal.Section9_9_Condition : "";
            command.Parameters.Add("@Section9_9_Comment", SqlDbType.NVarChar).Value = Modal.Section9_9_Comment != null ? Modal.Section9_9_Comment : "";
            command.Parameters.Add("@Section9_10_Condition", SqlDbType.NVarChar).Value = Modal.Section9_10_Condition != null ? Modal.Section9_10_Condition : "";
            command.Parameters.Add("@Section9_10_Comment", SqlDbType.NVarChar).Value = Modal.Section9_10_Comment != null ? Modal.Section9_10_Comment : "";
            command.Parameters.Add("@Section9_11_Condition", SqlDbType.NVarChar).Value = Modal.Section9_11_Condition != null ? Modal.Section9_11_Condition : "";
            command.Parameters.Add("@Section9_11_Comment", SqlDbType.NVarChar).Value = Modal.Section9_11_Comment != null ? Modal.Section9_11_Comment : "";
            command.Parameters.Add("@Section9_12_Condition", SqlDbType.NVarChar).Value = Modal.Section9_12_Condition != null ? Modal.Section9_12_Condition : "";
            command.Parameters.Add("@Section9_12_Comment", SqlDbType.NVarChar).Value = Modal.Section9_12_Comment != null ? Modal.Section9_12_Comment : "";
            command.Parameters.Add("@Section9_13_Condition", SqlDbType.NVarChar).Value = Modal.Section9_13_Condition != null ? Modal.Section9_13_Condition : "";
            command.Parameters.Add("@Section9_13_Comment", SqlDbType.NVarChar).Value = Modal.Section9_13_Comment != null ? Modal.Section9_13_Comment : "";
            command.Parameters.Add("@Section9_14_Condition", SqlDbType.NVarChar).Value = Modal.Section9_14_Condition != null ? Modal.Section9_14_Condition : "";
            command.Parameters.Add("@Section9_14_Comment", SqlDbType.NVarChar).Value = Modal.Section9_14_Comment != null ? Modal.Section9_14_Comment : "";
            command.Parameters.Add("@Section9_15_Condition", SqlDbType.NVarChar).Value = Modal.Section9_15_Condition != null ? Modal.Section9_15_Condition : "";
            command.Parameters.Add("@Section9_15_Comment", SqlDbType.NVarChar).Value = Modal.Section9_15_Comment != null ? Modal.Section9_15_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section9_16_Condition", SqlDbType.NVarChar).Value = Modal.Section9_16_Condition != null ? Modal.Section9_16_Condition : "";
            command.Parameters.Add("@Section9_16_Comment", SqlDbType.NVarChar).Value = Modal.Section9_16_Comment != null ? Modal.Section9_16_Comment : "";
            command.Parameters.Add("@Section9_17_Condition", SqlDbType.NVarChar).Value = Modal.Section9_17_Condition != null ? Modal.Section9_17_Condition : "";
            command.Parameters.Add("@Section9_17_Comment", SqlDbType.NVarChar).Value = Modal.Section9_17_Comment != null ? Modal.Section9_17_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@Section10_1_Condition", SqlDbType.NVarChar).Value = Modal.Section10_1_Condition != null ? Modal.Section10_1_Condition : "";
            command.Parameters.Add("@Section10_1_Comment", SqlDbType.NVarChar).Value = Modal.Section10_1_Comment != null ? Modal.Section10_1_Comment : "";
            command.Parameters.Add("@Section10_2_Condition", SqlDbType.NVarChar).Value = Modal.Section10_2_Condition != null ? Modal.Section10_2_Condition : "";
            command.Parameters.Add("@Section10_2_Comment", SqlDbType.NVarChar).Value = Modal.Section10_2_Comment != null ? Modal.Section10_2_Comment : "";
            command.Parameters.Add("@Section10_3_Condition", SqlDbType.NVarChar).Value = Modal.Section10_3_Condition != null ? Modal.Section10_3_Condition : "";
            command.Parameters.Add("@Section10_3_Comment", SqlDbType.NVarChar).Value = Modal.Section10_3_Comment != null ? Modal.Section10_3_Comment : "";
            command.Parameters.Add("@Section10_4_Condition", SqlDbType.NVarChar).Value = Modal.Section10_4_Condition != null ? Modal.Section10_4_Condition : "";
            command.Parameters.Add("@Section10_4_Comment", SqlDbType.NVarChar).Value = Modal.Section10_4_Comment != null ? Modal.Section10_4_Comment : "";
            command.Parameters.Add("@Section10_5_Condition", SqlDbType.NVarChar).Value = Modal.Section10_5_Condition != null ? Modal.Section10_5_Condition : "";
            command.Parameters.Add("@Section10_5_Comment", SqlDbType.NVarChar).Value = Modal.Section10_5_Comment != null ? Modal.Section10_5_Comment : "";
            command.Parameters.Add("@Section10_6_Condition", SqlDbType.NVarChar).Value = Modal.Section10_6_Condition != null ? Modal.Section10_6_Condition : "";
            command.Parameters.Add("@Section10_6_Comment", SqlDbType.NVarChar).Value = Modal.Section10_6_Comment != null ? Modal.Section10_6_Comment : "";
            command.Parameters.Add("@Section10_7_Condition", SqlDbType.NVarChar).Value = Modal.Section10_7_Condition != null ? Modal.Section10_7_Condition : "";
            command.Parameters.Add("@Section10_7_Comment", SqlDbType.NVarChar).Value = Modal.Section10_7_Comment != null ? Modal.Section10_7_Comment : "";
            command.Parameters.Add("@Section10_8_Condition", SqlDbType.NVarChar).Value = Modal.Section10_8_Condition != null ? Modal.Section10_8_Condition : "";
            command.Parameters.Add("@Section10_8_Comment", SqlDbType.NVarChar).Value = Modal.Section10_8_Comment != null ? Modal.Section10_8_Comment : "";
            command.Parameters.Add("@Section10_9_Condition", SqlDbType.NVarChar).Value = Modal.Section10_9_Condition != null ? Modal.Section10_9_Condition : "";
            command.Parameters.Add("@Section10_9_Comment", SqlDbType.NVarChar).Value = Modal.Section10_9_Comment != null ? Modal.Section10_9_Comment : "";
            command.Parameters.Add("@Section10_10_Condition", SqlDbType.NVarChar).Value = Modal.Section10_10_Condition != null ? Modal.Section10_10_Condition : "";
            command.Parameters.Add("@Section10_10_Comment", SqlDbType.NVarChar).Value = Modal.Section10_10_Comment != null ? Modal.Section10_10_Comment : "";
            command.Parameters.Add("@Section10_11_Condition", SqlDbType.NVarChar).Value = Modal.Section10_11_Condition != null ? Modal.Section10_11_Condition : "";
            command.Parameters.Add("@Section10_11_Comment", SqlDbType.NVarChar).Value = Modal.Section10_11_Comment != null ? Modal.Section10_11_Comment : "";
            command.Parameters.Add("@Section10_12_Condition", SqlDbType.NVarChar).Value = Modal.Section10_12_Condition != null ? Modal.Section10_12_Condition : "";
            command.Parameters.Add("@Section10_12_Comment", SqlDbType.NVarChar).Value = Modal.Section10_12_Comment != null ? Modal.Section10_12_Comment : "";
            command.Parameters.Add("@Section10_13_Condition", SqlDbType.NVarChar).Value = Modal.Section10_13_Condition != null ? Modal.Section10_13_Condition : "";
            command.Parameters.Add("@Section10_13_Comment", SqlDbType.NVarChar).Value = Modal.Section10_13_Comment != null ? Modal.Section10_13_Comment : "";
            command.Parameters.Add("@Section10_14_Condition", SqlDbType.NVarChar).Value = Modal.Section10_14_Condition != null ? Modal.Section10_14_Condition : "";
            command.Parameters.Add("@Section10_14_Comment", SqlDbType.NVarChar).Value = Modal.Section10_14_Comment != null ? Modal.Section10_14_Comment : "";
            command.Parameters.Add("@Section10_15_Condition", SqlDbType.NVarChar).Value = Modal.Section10_15_Condition != null ? Modal.Section10_15_Condition : "";
            command.Parameters.Add("@Section10_15_Comment", SqlDbType.NVarChar).Value = Modal.Section10_15_Comment != null ? Modal.Section10_15_Comment : "";
            command.Parameters.Add("@Section10_16_Condition", SqlDbType.NVarChar).Value = Modal.Section10_16_Condition != null ? Modal.Section10_16_Condition : "";
            command.Parameters.Add("@Section10_16_Comment", SqlDbType.NVarChar).Value = Modal.Section10_16_Comment != null ? Modal.Section10_16_Comment : "";
            command.Parameters.Add("@Section11_1_Condition", SqlDbType.NVarChar).Value = Modal.Section11_1_Condition != null ? Modal.Section11_1_Condition : "";
            command.Parameters.Add("@Section11_1_Comment", SqlDbType.NVarChar).Value = Modal.Section11_1_Comment != null ? Modal.Section11_1_Comment : "";
            command.Parameters.Add("@Section11_2_Condition", SqlDbType.NVarChar).Value = Modal.Section11_2_Condition != null ? Modal.Section11_2_Condition : "";
            command.Parameters.Add("@Section11_2_Comment", SqlDbType.NVarChar).Value = Modal.Section11_2_Comment != null ? Modal.Section11_2_Comment : "";
            command.Parameters.Add("@Section11_3_Condition", SqlDbType.NVarChar).Value = Modal.Section11_3_Condition != null ? Modal.Section11_3_Condition : "";
            command.Parameters.Add("@Section11_3_Comment", SqlDbType.NVarChar).Value = Modal.Section11_3_Comment != null ? Modal.Section11_3_Comment : "";
            command.Parameters.Add("@Section11_4_Condition", SqlDbType.NVarChar).Value = Modal.Section11_4_Condition != null ? Modal.Section11_4_Condition : "";
            command.Parameters.Add("@Section11_4_Comment", SqlDbType.NVarChar).Value = Modal.Section11_4_Comment != null ? Modal.Section11_4_Comment : "";
            command.Parameters.Add("@Section11_5_Condition", SqlDbType.NVarChar).Value = Modal.Section11_5_Condition != null ? Modal.Section11_5_Condition : "";
            command.Parameters.Add("@Section11_5_Comment", SqlDbType.NVarChar).Value = Modal.Section11_5_Comment != null ? Modal.Section11_5_Comment : "";
            command.Parameters.Add("@Section11_6_Condition", SqlDbType.NVarChar).Value = Modal.Section11_6_Condition != null ? Modal.Section11_6_Condition : "";
            command.Parameters.Add("@Section11_6_Comment", SqlDbType.NVarChar).Value = Modal.Section11_6_Comment != null ? Modal.Section11_6_Comment : "";
            command.Parameters.Add("@Section11_7_Condition", SqlDbType.NVarChar).Value = Modal.Section11_7_Condition != null ? Modal.Section11_7_Condition : "";
            command.Parameters.Add("@Section11_7_Comment", SqlDbType.NVarChar).Value = Modal.Section11_7_Comment != null ? Modal.Section11_7_Comment : "";
            command.Parameters.Add("@Section11_8_Condition", SqlDbType.NVarChar).Value = Modal.Section11_8_Condition != null ? Modal.Section11_8_Condition : "";
            command.Parameters.Add("@Section11_8_Comment", SqlDbType.NVarChar).Value = Modal.Section11_8_Comment != null ? Modal.Section11_8_Comment : "";
            command.Parameters.Add("@Section12_1_Condition", SqlDbType.NVarChar).Value = Modal.Section12_1_Condition != null ? Modal.Section12_1_Condition : "";
            command.Parameters.Add("@Section12_1_Comment", SqlDbType.NVarChar).Value = Modal.Section12_1_Comment != null ? Modal.Section12_1_Comment : "";
            command.Parameters.Add("@Section12_2_Condition", SqlDbType.NVarChar).Value = Modal.Section12_2_Condition != null ? Modal.Section12_2_Condition : "";
            command.Parameters.Add("@Section12_2_Comment", SqlDbType.NVarChar).Value = Modal.Section12_2_Comment != null ? Modal.Section12_2_Comment : "";
            command.Parameters.Add("@Section12_3_Condition", SqlDbType.NVarChar).Value = Modal.Section12_3_Condition != null ? Modal.Section12_3_Condition : "";
            command.Parameters.Add("@Section12_3_Comment", SqlDbType.NVarChar).Value = Modal.Section12_3_Comment != null ? Modal.Section12_3_Comment : "";
            command.Parameters.Add("@Section12_4_Condition", SqlDbType.NVarChar).Value = Modal.Section12_4_Condition != null ? Modal.Section12_4_Condition : "";
            command.Parameters.Add("@Section12_4_Comment", SqlDbType.NVarChar).Value = Modal.Section12_4_Comment != null ? Modal.Section12_4_Comment : "";
            command.Parameters.Add("@Section12_5_Condition", SqlDbType.NVarChar).Value = Modal.Section12_5_Condition != null ? Modal.Section12_5_Condition : "";
            command.Parameters.Add("@Section12_5_Comment", SqlDbType.NVarChar).Value = Modal.Section12_5_Comment != null ? Modal.Section12_5_Comment : "";
            command.Parameters.Add("@Section12_6_Condition", SqlDbType.NVarChar).Value = Modal.Section12_6_Condition != null ? Modal.Section12_6_Condition : "";
            command.Parameters.Add("@Section12_6_Comment", SqlDbType.NVarChar).Value = Modal.Section12_6_Comment != null ? Modal.Section12_6_Comment : "";
            command.Parameters.Add("@Section13_1_Condition", SqlDbType.NVarChar).Value = Modal.Section13_1_Condition != null ? Modal.Section13_1_Condition : "";
            command.Parameters.Add("@Section13_1_Comment", SqlDbType.NVarChar).Value = Modal.Section13_1_Comment != null ? Modal.Section13_1_Comment : "";
            command.Parameters.Add("@Section13_2_Condition", SqlDbType.NVarChar).Value = Modal.Section13_2_Condition != null ? Modal.Section13_2_Condition : "";
            command.Parameters.Add("@Section13_2_Comment", SqlDbType.NVarChar).Value = Modal.Section13_2_Comment != null ? Modal.Section13_2_Comment : "";
            command.Parameters.Add("@Section13_3_Condition", SqlDbType.NVarChar).Value = Modal.Section13_3_Condition != null ? Modal.Section13_3_Condition : "";
            command.Parameters.Add("@Section13_3_Comment", SqlDbType.NVarChar).Value = Modal.Section13_3_Comment != null ? Modal.Section13_3_Comment : "";
            command.Parameters.Add("@Section13_4_Condition", SqlDbType.NVarChar).Value = Modal.Section13_4_Condition != null ? Modal.Section13_4_Condition : "";
            command.Parameters.Add("@Section13_4_Comment", SqlDbType.NVarChar).Value = Modal.Section13_4_Comment != null ? Modal.Section13_4_Comment : "";
            command.Parameters.Add("@Section14_1_Condition", SqlDbType.NVarChar).Value = Modal.Section14_1_Condition != null ? Modal.Section14_1_Condition : "";
            command.Parameters.Add("@Section14_1_Comment", SqlDbType.NVarChar).Value = Modal.Section14_1_Comment != null ? Modal.Section14_1_Comment : "";
            command.Parameters.Add("@Section14_2_Condition", SqlDbType.NVarChar).Value = Modal.Section14_2_Condition != null ? Modal.Section14_2_Condition : "";
            command.Parameters.Add("@Section14_2_Comment", SqlDbType.NVarChar).Value = Modal.Section14_2_Comment != null ? Modal.Section14_2_Comment : "";
            command.Parameters.Add("@Section14_3_Condition", SqlDbType.NVarChar).Value = Modal.Section14_3_Condition != null ? Modal.Section14_3_Condition : "";
            command.Parameters.Add("@Section14_3_Comment", SqlDbType.NVarChar).Value = Modal.Section14_3_Comment != null ? Modal.Section14_3_Comment : "";
            command.Parameters.Add("@Section14_4_Condition", SqlDbType.NVarChar).Value = Modal.Section14_4_Condition != null ? Modal.Section14_4_Condition : "";
            command.Parameters.Add("@Section14_4_Comment", SqlDbType.NVarChar).Value = Modal.Section14_4_Comment != null ? Modal.Section14_4_Comment : "";
            command.Parameters.Add("@Section14_5_Condition", SqlDbType.NVarChar).Value = Modal.Section14_5_Condition != null ? Modal.Section14_5_Condition : "";
            command.Parameters.Add("@Section14_5_Comment", SqlDbType.NVarChar).Value = Modal.Section14_5_Comment != null ? Modal.Section14_5_Comment : "";
            command.Parameters.Add("@Section14_6_Condition", SqlDbType.NVarChar).Value = Modal.Section14_6_Condition != null ? Modal.Section14_6_Condition : "";
            command.Parameters.Add("@Section14_6_Comment", SqlDbType.NVarChar).Value = Modal.Section14_6_Comment != null ? Modal.Section14_6_Comment : "";
            command.Parameters.Add("@Section14_7_Condition", SqlDbType.NVarChar).Value = Modal.Section14_7_Condition != null ? Modal.Section14_7_Condition : "";
            command.Parameters.Add("@Section14_7_Comment", SqlDbType.NVarChar).Value = Modal.Section14_7_Comment != null ? Modal.Section14_7_Comment : "";
            command.Parameters.Add("@Section14_8_Condition", SqlDbType.NVarChar).Value = Modal.Section14_8_Condition != null ? Modal.Section14_8_Condition : "";
            command.Parameters.Add("@Section14_8_Comment", SqlDbType.NVarChar).Value = Modal.Section14_8_Comment != null ? Modal.Section14_8_Comment : "";
            command.Parameters.Add("@Section14_9_Condition", SqlDbType.NVarChar).Value = Modal.Section14_9_Condition != null ? Modal.Section14_9_Condition : "";
            command.Parameters.Add("@Section14_9_Comment", SqlDbType.NVarChar).Value = Modal.Section14_9_Comment != null ? Modal.Section14_9_Comment : "";
            command.Parameters.Add("@Section14_10_Condition", SqlDbType.NVarChar).Value = Modal.Section14_10_Condition != null ? Modal.Section14_10_Condition : "";
            command.Parameters.Add("@Section14_10_Comment", SqlDbType.NVarChar).Value = Modal.Section14_10_Comment != null ? Modal.Section14_10_Comment : "";
            command.Parameters.Add("@Section14_11_Condition", SqlDbType.NVarChar).Value = Modal.Section14_11_Condition != null ? Modal.Section14_11_Condition : "";
            command.Parameters.Add("@Section14_11_Comment", SqlDbType.NVarChar).Value = Modal.Section14_11_Comment != null ? Modal.Section14_11_Comment : "";
            command.Parameters.Add("@Section14_12_Condition", SqlDbType.NVarChar).Value = Modal.Section14_12_Condition != null ? Modal.Section14_12_Condition : "";
            command.Parameters.Add("@Section14_12_Comment", SqlDbType.NVarChar).Value = Modal.Section14_12_Comment != null ? Modal.Section14_12_Comment : "";
            command.Parameters.Add("@Section14_13_Condition", SqlDbType.NVarChar).Value = Modal.Section14_13_Condition != null ? Modal.Section14_13_Condition : "";
            command.Parameters.Add("@Section14_13_Comment", SqlDbType.NVarChar).Value = Modal.Section14_13_Comment != null ? Modal.Section14_13_Comment : "";
            command.Parameters.Add("@Section14_14_Condition", SqlDbType.NVarChar).Value = Modal.Section14_14_Condition != null ? Modal.Section14_14_Condition : "";
            command.Parameters.Add("@Section14_14_Comment", SqlDbType.NVarChar).Value = Modal.Section14_14_Comment != null ? Modal.Section14_14_Comment : "";
            command.Parameters.Add("@Section14_15_Condition", SqlDbType.NVarChar).Value = Modal.Section14_15_Condition != null ? Modal.Section14_15_Condition : "";
            command.Parameters.Add("@Section14_15_Comment", SqlDbType.NVarChar).Value = Modal.Section14_15_Comment != null ? Modal.Section14_15_Comment : "";
            command.Parameters.Add("@Section14_16_Condition", SqlDbType.NVarChar).Value = Modal.Section14_16_Condition != null ? Modal.Section14_16_Condition : "";
            command.Parameters.Add("@Section14_16_Comment", SqlDbType.NVarChar).Value = Modal.Section14_16_Comment != null ? Modal.Section14_16_Comment : "";
            command.Parameters.Add("@Section14_17_Condition", SqlDbType.NVarChar).Value = Modal.Section14_17_Condition != null ? Modal.Section14_17_Condition : "";
            command.Parameters.Add("@Section14_17_Comment", SqlDbType.NVarChar).Value = Modal.Section14_17_Comment != null ? Modal.Section14_17_Comment : "";
            command.Parameters.Add("@Section14_18_Condition", SqlDbType.NVarChar).Value = Modal.Section14_18_Condition != null ? Modal.Section14_18_Condition : "";
            command.Parameters.Add("@Section14_18_Comment", SqlDbType.NVarChar).Value = Modal.Section14_18_Comment != null ? Modal.Section14_18_Comment : "";
            command.Parameters.Add("@Section14_19_Condition", SqlDbType.NVarChar).Value = Modal.Section14_19_Condition != null ? Modal.Section14_19_Condition : "";
            command.Parameters.Add("@Section14_19_Comment", SqlDbType.NVarChar).Value = Modal.Section14_19_Comment != null ? Modal.Section14_19_Comment : "";
            command.Parameters.Add("@Section14_20_Condition", SqlDbType.NVarChar).Value = Modal.Section14_20_Condition != null ? Modal.Section14_20_Condition : "";
            command.Parameters.Add("@Section14_20_Comment", SqlDbType.NVarChar).Value = Modal.Section14_20_Comment != null ? Modal.Section14_20_Comment : "";
            command.Parameters.Add("@Section14_21_Condition", SqlDbType.NVarChar).Value = Modal.Section14_21_Condition != null ? Modal.Section14_21_Condition : "";
            command.Parameters.Add("@Section14_21_Comment", SqlDbType.NVarChar).Value = Modal.Section14_21_Comment != null ? Modal.Section14_21_Comment : "";
            command.Parameters.Add("@Section14_22_Condition", SqlDbType.NVarChar).Value = Modal.Section14_22_Condition != null ? Modal.Section14_22_Condition : "";
            command.Parameters.Add("@Section14_22_Comment", SqlDbType.NVarChar).Value = Modal.Section14_22_Comment != null ? Modal.Section14_22_Comment : "";
            command.Parameters.Add("@Section14_23_Condition", SqlDbType.NVarChar).Value = Modal.Section14_23_Condition != null ? Modal.Section14_23_Condition : "";
            command.Parameters.Add("@Section14_23_Comment", SqlDbType.NVarChar).Value = Modal.Section14_23_Comment != null ? Modal.Section14_23_Comment : "";
            command.Parameters.Add("@Section14_24_Condition", SqlDbType.NVarChar).Value = Modal.Section14_24_Condition != null ? Modal.Section14_24_Condition : "";
            command.Parameters.Add("@Section14_24_Comment", SqlDbType.NVarChar).Value = Modal.Section14_24_Comment != null ? Modal.Section14_24_Comment : "";
            command.Parameters.Add("@Section14_25_Condition", SqlDbType.NVarChar).Value = Modal.Section14_25_Condition != null ? Modal.Section14_25_Condition : "";
            command.Parameters.Add("@Section14_25_Comment", SqlDbType.NVarChar).Value = Modal.Section14_25_Comment != null ? Modal.Section14_25_Comment : "";
            command.Parameters.Add("@Section15_1_Condition", SqlDbType.NVarChar).Value = Modal.Section15_1_Condition != null ? Modal.Section15_1_Condition : "";
            command.Parameters.Add("@Section15_1_Comment", SqlDbType.NVarChar).Value = Modal.Section15_1_Comment != null ? Modal.Section15_1_Comment : "";
            command.Parameters.Add("@Section15_2_Condition", SqlDbType.NVarChar).Value = Modal.Section15_2_Condition != null ? Modal.Section15_2_Condition : "";
            command.Parameters.Add("@Section15_2_Comment", SqlDbType.NVarChar).Value = Modal.Section15_2_Comment != null ? Modal.Section15_2_Comment : "";
            command.Parameters.Add("@Section15_3_Condition", SqlDbType.NVarChar).Value = Modal.Section15_3_Condition != null ? Modal.Section15_3_Condition : "";
            command.Parameters.Add("@Section15_3_Comment", SqlDbType.NVarChar).Value = Modal.Section15_3_Comment != null ? Modal.Section15_3_Comment : "";
            command.Parameters.Add("@Section15_4_Condition", SqlDbType.NVarChar).Value = Modal.Section15_4_Condition != null ? Modal.Section15_4_Condition : "";
            command.Parameters.Add("@Section15_4_Comment", SqlDbType.NVarChar).Value = Modal.Section15_4_Comment != null ? Modal.Section15_4_Comment : "";
            command.Parameters.Add("@Section15_5_Condition", SqlDbType.NVarChar).Value = Modal.Section15_5_Condition != null ? Modal.Section15_5_Condition : "";
            command.Parameters.Add("@Section15_5_Comment", SqlDbType.NVarChar).Value = Modal.Section15_5_Comment != null ? Modal.Section15_5_Comment : "";
            command.Parameters.Add("@Section15_6_Condition", SqlDbType.NVarChar).Value = Modal.Section15_6_Condition != null ? Modal.Section15_6_Condition : "";
            command.Parameters.Add("@Section15_6_Comment", SqlDbType.NVarChar).Value = Modal.Section15_6_Comment != null ? Modal.Section15_6_Comment : "";
            command.Parameters.Add("@Section15_7_Condition", SqlDbType.NVarChar).Value = Modal.Section15_7_Condition != null ? Modal.Section15_7_Condition : "";
            command.Parameters.Add("@Section15_7_Comment", SqlDbType.NVarChar).Value = Modal.Section15_7_Comment != null ? Modal.Section15_7_Comment : "";
            command.Parameters.Add("@Section15_8_Condition", SqlDbType.NVarChar).Value = Modal.Section15_8_Condition != null ? Modal.Section15_8_Condition : "";
            command.Parameters.Add("@Section15_8_Comment", SqlDbType.NVarChar).Value = Modal.Section15_8_Comment != null ? Modal.Section15_8_Comment : "";
            command.Parameters.Add("@Section15_9_Condition", SqlDbType.NVarChar).Value = Modal.Section15_9_Condition != null ? Modal.Section15_9_Condition : "";
            command.Parameters.Add("@Section15_9_Comment", SqlDbType.NVarChar).Value = Modal.Section15_9_Comment != null ? Modal.Section15_9_Comment : "";
            command.Parameters.Add("@Section15_10_Condition", SqlDbType.NVarChar).Value = Modal.Section15_10_Condition != null ? Modal.Section15_10_Condition : "";
            command.Parameters.Add("@Section15_10_Comment", SqlDbType.NVarChar).Value = Modal.Section15_10_Comment != null ? Modal.Section15_10_Comment : "";
            command.Parameters.Add("@Section15_11_Condition", SqlDbType.NVarChar).Value = Modal.Section15_11_Condition != null ? Modal.Section15_11_Condition : "";
            command.Parameters.Add("@Section15_11_Comment", SqlDbType.NVarChar).Value = Modal.Section15_11_Comment != null ? Modal.Section15_11_Comment : "";
            command.Parameters.Add("@Section15_12_Condition", SqlDbType.NVarChar).Value = Modal.Section15_12_Condition != null ? Modal.Section15_12_Condition : "";
            command.Parameters.Add("@Section15_12_Comment", SqlDbType.NVarChar).Value = Modal.Section15_12_Comment != null ? Modal.Section15_12_Comment : "";
            command.Parameters.Add("@Section15_13_Condition", SqlDbType.NVarChar).Value = Modal.Section15_13_Condition != null ? Modal.Section15_13_Condition : "";
            command.Parameters.Add("@Section15_13_Comment", SqlDbType.NVarChar).Value = Modal.Section15_13_Comment != null ? Modal.Section15_13_Comment : "";
            command.Parameters.Add("@Section15_14_Condition", SqlDbType.NVarChar).Value = Modal.Section15_14_Condition != null ? Modal.Section15_14_Condition : "";
            command.Parameters.Add("@Section15_14_Comment", SqlDbType.NVarChar).Value = Modal.Section15_14_Comment != null ? Modal.Section15_14_Comment : "";
            command.Parameters.Add("@Section15_15_Condition", SqlDbType.NVarChar).Value = Modal.Section15_15_Condition != null ? Modal.Section15_15_Condition : "";
            command.Parameters.Add("@Section15_15_Comment", SqlDbType.NVarChar).Value = Modal.Section15_15_Comment != null ? Modal.Section15_15_Comment : "";
            command.Parameters.Add("@Section16_1_Condition", SqlDbType.NVarChar).Value = Modal.Section16_1_Condition != null ? Modal.Section16_1_Condition : "";
            command.Parameters.Add("@Section16_1_Comment", SqlDbType.NVarChar).Value = Modal.Section16_1_Comment != null ? Modal.Section16_1_Comment : "";
            command.Parameters.Add("@Section16_2_Condition", SqlDbType.NVarChar).Value = Modal.Section16_2_Condition != null ? Modal.Section16_2_Condition : "";
            command.Parameters.Add("@Section16_2_Comment", SqlDbType.NVarChar).Value = Modal.Section16_2_Comment != null ? Modal.Section16_2_Comment : "";
            command.Parameters.Add("@Section16_3_Condition", SqlDbType.NVarChar).Value = Modal.Section16_3_Condition != null ? Modal.Section16_3_Condition : "";
            command.Parameters.Add("@Section16_3_Comment", SqlDbType.NVarChar).Value = Modal.Section16_3_Comment != null ? Modal.Section16_3_Comment : "";
            command.Parameters.Add("@Section16_4_Condition", SqlDbType.NVarChar).Value = Modal.Section16_4_Condition != null ? Modal.Section16_4_Condition : "";
            command.Parameters.Add("@Section16_4_Comment", SqlDbType.NVarChar).Value = Modal.Section16_4_Comment != null ? Modal.Section16_4_Comment : "";
            command.Parameters.Add("@Section17_1_Condition", SqlDbType.NVarChar).Value = Modal.Section17_1_Condition != null ? Modal.Section17_1_Condition : "";
            command.Parameters.Add("@Section17_1_Comment", SqlDbType.NVarChar).Value = Modal.Section17_1_Comment != null ? Modal.Section17_1_Comment : "";
            command.Parameters.Add("@Section17_2_Condition", SqlDbType.NVarChar).Value = Modal.Section17_2_Condition != null ? Modal.Section17_2_Condition : "";
            command.Parameters.Add("@Section17_2_Comment", SqlDbType.NVarChar).Value = Modal.Section17_2_Comment != null ? Modal.Section17_2_Comment : "";
            command.Parameters.Add("@Section17_3_Condition", SqlDbType.NVarChar).Value = Modal.Section17_3_Condition != null ? Modal.Section17_3_Condition : "";
            command.Parameters.Add("@Section17_3_Comment", SqlDbType.NVarChar).Value = Modal.Section17_3_Comment != null ? Modal.Section17_3_Comment : "";
            command.Parameters.Add("@Section17_4_Condition", SqlDbType.NVarChar).Value = Modal.Section17_4_Condition != null ? Modal.Section17_4_Condition : "";
            command.Parameters.Add("@Section17_4_Comment", SqlDbType.NVarChar).Value = Modal.Section17_4_Comment != null ? Modal.Section17_4_Comment : "";
            command.Parameters.Add("@Section17_5_Condition", SqlDbType.NVarChar).Value = Modal.Section17_5_Condition != null ? Modal.Section17_5_Condition : "";
            command.Parameters.Add("@Section17_5_Comment", SqlDbType.NVarChar).Value = Modal.Section17_5_Comment != null ? Modal.Section17_5_Comment : "";
            command.Parameters.Add("@Section17_6_Condition", SqlDbType.NVarChar).Value = Modal.Section17_6_Condition != null ? Modal.Section17_6_Condition : "";
            command.Parameters.Add("@Section17_6_Comment", SqlDbType.NVarChar).Value = Modal.Section17_6_Comment != null ? Modal.Section17_6_Comment : "";
            command.Parameters.Add("@Section18_1_Condition", SqlDbType.NVarChar).Value = Modal.Section18_1_Condition != null ? Modal.Section18_1_Condition : "";
            command.Parameters.Add("@Section18_1_Comment", SqlDbType.NVarChar).Value = Modal.Section18_1_Comment != null ? Modal.Section18_1_Comment : "";
            command.Parameters.Add("@Section18_2_Condition", SqlDbType.NVarChar).Value = Modal.Section18_2_Condition != null ? Modal.Section18_2_Condition : "";
            command.Parameters.Add("@Section18_2_Comment", SqlDbType.NVarChar).Value = Modal.Section18_2_Comment != null ? Modal.Section18_2_Comment : "";
            command.Parameters.Add("@Section18_3_Condition", SqlDbType.NVarChar).Value = Modal.Section18_3_Condition != null ? Modal.Section18_3_Condition : "";
            command.Parameters.Add("@Section18_3_Comment", SqlDbType.NVarChar).Value = Modal.Section18_3_Comment != null ? Modal.Section18_3_Comment : "";
            command.Parameters.Add("@Section18_4_Condition", SqlDbType.NVarChar).Value = Modal.Section18_4_Condition != null ? Modal.Section18_4_Condition : "";
            command.Parameters.Add("@Section18_4_Comment", SqlDbType.NVarChar).Value = Modal.Section18_4_Comment != null ? Modal.Section18_4_Comment : "";
            command.Parameters.Add("@Section18_5_Condition", SqlDbType.NVarChar).Value = Modal.Section18_5_Condition != null ? Modal.Section18_5_Condition : "";
            command.Parameters.Add("@Section18_5_Comment", SqlDbType.NVarChar).Value = Modal.Section18_5_Comment != null ? Modal.Section18_5_Comment : "";
            command.Parameters.Add("@Section18_6_Condition", SqlDbType.NVarChar).Value = Modal.Section18_6_Condition != null ? Modal.Section18_6_Condition : "";
            command.Parameters.Add("@Section18_6_Comment", SqlDbType.NVarChar).Value = Modal.Section18_6_Comment != null ? Modal.Section18_6_Comment : "";
            command.Parameters.Add("@Section18_7_Condition", SqlDbType.NVarChar).Value = Modal.Section18_7_Condition != null ? Modal.Section18_7_Condition : "";
            command.Parameters.Add("@Section18_7_Comment", SqlDbType.NVarChar).Value = Modal.Section18_7_Comment != null ? Modal.Section18_7_Comment : "";

            // RDBJ 02/15/2022
            command.Parameters.Add("@Section18_8_Condition", SqlDbType.NVarChar).Value = Modal.Section18_8_Condition != null ? Modal.Section18_8_Condition : "";
            command.Parameters.Add("@Section18_8_Comment", SqlDbType.NVarChar).Value = Modal.Section18_8_Comment != null ? Modal.Section18_8_Comment : "";
            command.Parameters.Add("@Section18_9_Condition", SqlDbType.NVarChar).Value = Modal.Section18_9_Condition != null ? Modal.Section18_9_Condition : "";
            command.Parameters.Add("@Section18_9_Comment", SqlDbType.NVarChar).Value = Modal.Section18_9_Comment != null ? Modal.Section18_9_Comment : "";
            // End RDBJ 02/15/2022

            command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced == null ? DBNull.Value : (object)Modal.IsSynced;
            command.Parameters.Add("@ModifyDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime //Modal.ModifyDate != null ? Modal.ModifyDate : null;
            command.Parameters.Add("@SavedAsDraft", SqlDbType.Bit).Value = Modal.SavedAsDraft ?? (object)DBNull.Value;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@FormVersion", SqlDbType.Decimal).Value = Modal.FormVersion;
        }
        public string GetSIRUpdateQuery()
        {
            // RDBJ 02/15/2022
            string query = @"UPDATE dbo.SuperintendedInspectionReport SET ShipID = @ShipID, FormVersion = @FormVersion,
                           ShipName = @ShipName,Date=@Date,Port=@Port,
                           Master=@Master,Superintended=@Superintended,
                            Section1_1_Condition=@Section1_1_Condition,Section1_1_Comment=@Section1_1_Comment,
                            Section1_2_Condition=@Section1_2_Condition,Section1_2_Comment=@Section1_2_Comment,Section1_3_Condition=@Section1_3_Condition,
                            Section1_3_Comment=@Section1_3_Comment,Section1_4_Condition=@Section1_4_Condition,
                            Section1_4_Comment=@Section1_4_Comment,Section1_5_Condition=@Section1_5_Condition,
                            Section1_5_Comment=@Section1_5_Comment,Section1_6_Condition=@Section1_6_Condition,Section1_6_Comment=@Section1_6_Comment,
                            Section1_7_Condition=@Section1_7_Condition,Section1_7_Comment=@Section1_7_Comment,Section1_8_Condition=@Section1_8_Condition,
                            Section1_8_Comment=@Section1_8_Comment,Section1_9_Condition=@Section1_9_Condition,Section1_9_Comment=@Section1_9_Comment,
                            Section1_10_Condition=@Section1_10_Condition,Section1_10_Comment=@Section1_10_Comment,Section1_11_Condition=@Section1_11_Condition,
                            Section1_11_Comment=@Section1_11_Comment,Section2_1_Condition=@Section2_1_Condition,Section2_1_Comment=@Section2_1_Comment,
                            Section2_2_Condition=@Section2_2_Condition,Section2_2_Comment=@Section2_2_Comment,Section2_3_Condition=@Section2_3_Condition,
                            Section2_3_Comment=@Section2_3_Comment,Section2_4_Condition=@Section2_4_Condition,Section2_4_Comment=@Section2_4_Comment,
                            Section2_5_Condition=@Section2_5_Condition,Section2_5_Comment=@Section2_5_Comment,Section2_6_Condition=@Section2_6_Condition,
                            Section2_6_Comment=@Section2_6_Comment,Section2_7_Condition=@Section2_7_Condition,Section2_7_Comment=@Section2_7_Comment,
                            Section3_1_Condition=@Section3_1_Condition,Section3_1_Comment=@Section3_1_Comment,Section3_2_Condition=@Section3_2_Condition,
                            Section3_2_Comment=@Section3_2_Comment,Section3_3_Condition=@Section3_3_Condition,Section3_3_Comment=@Section3_3_Comment,
                            Section3_4_Condition=@Section3_4_Condition,Section3_4_Comment=@Section3_4_Comment,Section3_5_Condition=@Section3_5_Condition,
                            Section3_5_Comment=@Section3_5_Comment,Section4_1_Condition=@Section4_1_Condition,Section4_1_Comment=@Section4_1_Comment,
                            Section4_2_Condition=@Section4_2_Condition,Section4_2_Comment=@Section4_2_Comment,Section4_3_Condition=@Section4_3_Condition,
                            Section4_3_Comment=@Section4_3_Comment,Section5_1_Condition=@Section5_1_Condition,Section5_1_Comment=@Section5_1_Comment,
                            Section5_6_Condition=@Section5_6_Condition,Section5_6_Comment=@Section5_6_Comment,Section5_8_Condition=@Section5_8_Condition,
                            Section5_8_Comment=@Section5_8_Comment,Section5_9_Condition=@Section5_9_Condition,Section5_9_Comment=@Section5_9_Comment,
                            Section6_1_Condition=@Section6_1_Condition,Section6_1_Comment=@Section6_1_Comment,Section6_2_Condition=@Section6_2_Condition,
                            Section6_2_Comment=@Section6_2_Comment,Section6_3_Condition=@Section6_3_Condition,Section6_3_Comment=@Section6_3_Comment,
                            Section6_4_Condition=@Section6_4_Condition,Section6_4_Comment=@Section6_4_Comment,Section6_5_Condition=@Section6_5_Condition,
                            Section6_5_Comment=@Section6_5_Comment,Section6_6_Condition=@Section6_6_Condition,Section6_6_Comment=@Section6_6_Comment,
                            Section6_7_Condition=@Section6_7_Condition,Section6_7_Comment=@Section6_7_Comment,Section6_8_Condition=@Section6_8_Condition,
                            Section6_8_Comment=@Section6_8_Comment,Section7_1_Condition=@Section7_1_Condition,Section7_1_Comment=@Section7_1_Comment,
                            Section7_2_Condition=@Section7_2_Condition,Section7_2_Comment=@Section7_2_Comment,Section7_3_Condition=@Section7_3_Condition,
                            Section7_3_Comment=@Section7_3_Comment,Section7_4_Condition=@Section7_4_Condition,Section7_4_Comment=@Section7_4_Comment,
                            Section7_5_Condition=@Section7_5_Condition,Section7_5_Comment=@Section7_5_Comment,Section7_6_Condition=@Section7_6_Condition,
                            Section7_6_Comment=@Section7_6_Comment,Section8_1_Condition=@Section8_1_Condition,Section8_1_Comment=@Section8_1_Comment,
                            Section8_2_Condition=@Section8_2_Condition,Section8_2_Comment=@Section8_2_Comment,Section8_3_Condition=@Section8_3_Condition,
                            Section8_3_Comment=@Section8_3_Comment,Section8_4_Condition=@Section8_4_Condition,Section8_4_Comment=@Section8_4_Comment,
                            Section8_5_Condition=@Section8_5_Condition,Section8_5_Comment=@Section8_5_Comment,Section8_6_Condition=@Section8_6_Condition,
                            Section8_6_Comment=@Section8_6_Comment,Section8_7_Condition=@Section8_7_Condition,Section8_7_Comment=@Section8_7_Comment,
                            Section8_8_Condition=@Section8_8_Condition,Section8_8_Comment=@Section8_8_Comment,Section8_9_Condition=@Section8_9_Condition,
                            Section8_9_Comment=@Section8_9_Comment,Section8_10_Condition=@Section8_10_Condition,Section8_10_Comment=@Section8_10_Comment,
                            Section8_11_Condition=@Section8_11_Condition,Section8_11_Comment=@Section8_11_Comment,Section8_12_Condition=@Section8_12_Condition,
                            Section8_12_Comment=@Section8_12_Comment,Section8_13_Condition=@Section8_13_Condition,Section8_13_Comment=@Section8_13_Comment,
                            Section8_14_Condition=@Section8_14_Condition,Section8_14_Comment=@Section8_14_Comment,Section8_15_Condition=@Section8_15_Condition,
                            Section8_15_Comment=@Section8_15_Comment,Section8_16_Condition=@Section8_16_Condition,Section8_16_Comment=@Section8_16_Comment,
                            Section8_17_Condition=@Section8_17_Condition,Section8_17_Comment=@Section8_17_Comment,Section8_18_Condition=@Section8_18_Condition,
                            Section8_18_Comment=@Section8_18_Comment,Section8_19_Condition=@Section8_19_Condition,Section8_19_Comment=@Section8_19_Comment,
                            Section8_20_Condition=@Section8_20_Condition,Section8_20_Comment=@Section8_20_Comment,Section8_21_Condition=@Section8_21_Condition,
                            Section8_21_Comment=@Section8_21_Comment,Section8_22_Condition=@Section8_22_Condition,Section8_22_Comment=@Section8_22_Comment,
                            Section8_23_Condition=@Section8_23_Condition,Section8_23_Comment=@Section8_23_Comment,Section8_24_Condition=@Section8_24_Condition,
                            Section8_24_Comment=@Section8_24_Comment,Section8_25_Condition=@Section8_25_Condition,Section8_25_Comment=@Section8_25_Comment,
                            Section9_1_Condition=@Section9_1_Condition,Section9_1_Comment=@Section9_1_Comment,Section9_2_Condition=@Section9_2_Condition,
                            Section9_2_Comment=@Section9_2_Comment,Section9_3_Condition=@Section9_3_Condition,Section9_3_Comment=@Section9_3_Comment,
                            Section9_4_Condition=@Section9_4_Condition,Section9_4_Comment=@Section9_4_Comment,Section9_5_Condition=@Section9_5_Condition,
                            Section9_5_Comment=@Section9_5_Comment,Section9_6_Condition=@Section9_6_Condition,Section9_6_Comment=@Section9_6_Comment,
                            Section9_7_Condition=@Section9_7_Condition,Section9_7_Comment=@Section9_7_Comment,Section9_8_Condition=@Section9_8_Condition,
                            Section9_8_Comment=@Section9_8_Comment,Section9_9_Condition=@Section9_9_Condition,Section9_9_Comment=@Section9_9_Comment,
                            Section9_10_Condition=@Section9_10_Condition,Section9_10_Comment=@Section9_10_Comment,Section9_11_Condition=@Section9_11_Condition,
                            Section9_11_Comment=@Section9_11_Comment,Section9_12_Condition=@Section9_12_Condition,Section9_12_Comment=@Section9_12_Comment,
                            Section9_13_Condition=@Section9_13_Condition,Section9_13_Comment=@Section9_13_Comment,Section9_14_Condition=@Section9_14_Condition,
                            Section9_14_Comment=@Section9_14_Comment,Section9_15_Condition=@Section9_15_Condition,Section9_15_Comment=@Section9_15_Comment,
                            Section9_16_Condition=@Section9_16_Condition,Section9_16_Comment=@Section9_16_Comment,
                            Section9_17_Condition=@Section9_17_Condition,Section9_17_Comment=@Section9_17_Comment,
                            Section10_1_Condition=@Section10_1_Condition,Section10_1_Comment=@Section10_1_Comment,Section10_2_Condition=@Section10_2_Condition,
                            Section10_2_Comment=@Section10_2_Comment,Section10_3_Condition=@Section10_3_Condition,Section10_3_Comment=@Section10_3_Comment,
                            Section10_4_Condition=@Section10_4_Condition,Section10_4_Comment=@Section10_4_Comment,Section10_5_Condition=@Section10_5_Condition,
                            Section10_5_Comment=@Section10_5_Comment,Section10_6_Condition=@Section10_6_Condition,Section10_6_Comment=@Section10_6_Comment,
                            Section10_7_Condition=@Section10_7_Condition,Section10_7_Comment=@Section10_7_Comment,Section10_8_Condition=@Section10_8_Condition,
                            Section10_8_Comment=@Section10_8_Comment,Section10_9_Condition=@Section10_9_Condition,Section10_9_Comment=@Section10_9_Comment,
                            Section10_10_Condition=@Section10_10_Condition,Section10_10_Comment=@Section10_10_Comment,Section10_11_Condition=@Section10_11_Condition,
                            Section10_11_Comment=@Section10_11_Comment,Section10_12_Condition=@Section10_12_Condition,Section10_12_Comment=@Section10_12_Comment,
                            Section10_13_Condition=@Section10_13_Condition,Section10_13_Comment=@Section10_13_Comment,Section10_14_Condition=@Section10_14_Condition,
                            Section10_14_Comment=@Section10_14_Comment,Section10_15_Condition=@Section10_15_Condition,Section10_15_Comment=@Section10_15_Comment,
                            Section10_16_Condition=@Section10_16_Condition,Section10_16_Comment=@Section10_16_Comment,Section11_1_Condition=@Section11_1_Condition,
                            Section11_1_Comment=@Section11_1_Comment,Section11_2_Condition=@Section11_2_Condition,Section11_2_Comment=@Section11_2_Comment,
                            Section11_3_Condition=@Section11_3_Condition,Section11_3_Comment=@Section11_3_Comment,Section11_4_Condition=@Section11_4_Condition,
                            Section11_4_Comment=@Section11_4_Comment,Section11_5_Condition=@Section11_5_Condition,Section11_5_Comment=@Section11_5_Comment,
                            Section11_6_Condition=@Section11_6_Condition,Section11_6_Comment=@Section11_6_Comment,Section11_7_Condition=@Section11_7_Condition,
                            Section11_7_Comment=@Section11_7_Comment,Section11_8_Condition=@Section11_8_Condition,Section11_8_Comment=@Section11_8_Comment,
                            Section12_1_Condition=@Section12_1_Condition,Section12_1_Comment=@Section12_1_Comment,Section12_2_Condition=@Section12_2_Condition,
                            Section12_2_Comment=@Section12_2_Comment,Section12_3_Condition=@Section12_3_Condition,Section12_3_Comment=@Section12_3_Comment,
                            Section12_4_Condition=@Section12_4_Condition,Section12_4_Comment=@Section12_4_Comment,Section12_5_Condition=@Section12_5_Condition,
                            Section12_5_Comment=@Section12_5_Comment,Section12_6_Condition=@Section12_6_Condition,Section12_6_Comment=@Section12_6_Comment,
                            Section13_1_Condition=@Section13_1_Condition,Section13_1_Comment=@Section13_1_Comment,Section13_2_Condition=@Section13_2_Condition,
                            Section13_2_Comment=@Section13_2_Comment,Section13_3_Condition=@Section13_3_Condition,Section13_3_Comment=@Section13_3_Comment,
                            Section13_4_Condition=@Section13_4_Condition,Section13_4_Comment=@Section13_4_Comment,Section14_1_Condition=@Section14_1_Condition,
                            Section14_1_Comment=@Section14_1_Comment,Section14_2_Condition=@Section14_2_Condition,Section14_2_Comment=@Section14_2_Comment,
                            Section14_3_Condition=@Section14_3_Condition,Section14_3_Comment=@Section14_3_Comment,Section14_4_Condition=@Section14_4_Condition,
                            Section14_4_Comment=@Section14_4_Comment,Section14_5_Condition=@Section14_5_Condition,Section14_5_Comment=@Section14_5_Comment,
                            Section14_6_Condition=@Section14_6_Condition,Section14_6_Comment=@Section14_6_Comment,Section14_7_Condition=@Section14_7_Condition,
                            Section14_7_Comment=@Section14_7_Comment,Section14_8_Condition=@Section14_8_Condition,Section14_8_Comment=@Section14_8_Comment,
                            Section14_9_Condition=@Section14_9_Condition,Section14_9_Comment=@Section14_9_Comment,Section14_10_Condition=@Section14_10_Condition,
                            Section14_10_Comment=@Section14_10_Comment,Section14_11_Condition=@Section14_11_Condition,Section14_11_Comment=@Section14_11_Comment,
                            Section14_12_Condition=@Section14_12_Condition,Section14_12_Comment=@Section14_12_Comment,Section14_13_Condition=@Section14_13_Condition,
                            Section14_13_Comment=@Section14_13_Comment,Section14_14_Condition=@Section14_14_Condition,Section14_14_Comment=@Section14_14_Comment,
                            Section14_15_Condition=@Section14_15_Condition,Section14_15_Comment=@Section14_15_Comment,Section14_16_Condition=@Section14_16_Condition,
                            Section14_16_Comment=@Section14_16_Comment,Section14_17_Condition=@Section14_17_Condition,Section14_17_Comment=@Section14_17_Comment,
                            Section14_18_Condition=@Section14_18_Condition,Section14_18_Comment=@Section14_18_Comment,Section14_19_Condition=@Section14_19_Condition,
                            Section14_19_Comment=@Section14_19_Comment,Section14_20_Condition=@Section14_20_Condition,Section14_20_Comment=@Section14_20_Comment,
                            Section14_21_Condition=@Section14_21_Condition,Section14_21_Comment=@Section14_21_Comment,Section14_22_Condition=@Section14_22_Condition,
                            Section14_22_Comment=@Section14_22_Comment,Section14_23_Condition=@Section14_23_Condition,Section14_23_Comment=@Section14_23_Comment,
                            Section14_24_Condition=@Section14_24_Condition,Section14_24_Comment=@Section14_24_Comment,Section14_25_Condition=@Section14_25_Condition,
                            Section14_25_Comment=@Section14_25_Comment,Section15_1_Condition=@Section15_1_Condition,Section15_1_Comment=@Section15_1_Comment,
                            Section15_2_Condition=@Section15_2_Condition,Section15_2_Comment=@Section15_2_Comment,Section15_3_Condition=@Section15_3_Condition,
                            Section15_3_Comment=@Section15_3_Comment,Section15_4_Condition=@Section15_4_Condition,Section15_4_Comment=@Section15_4_Comment,
                            Section15_5_Condition=@Section15_5_Condition,Section15_5_Comment=@Section15_5_Comment,Section15_6_Condition=@Section15_6_Condition,
                            Section15_6_Comment=@Section15_6_Comment,Section15_7_Condition=@Section15_7_Condition,Section15_7_Comment=@Section15_7_Comment,
                            Section15_8_Condition=@Section15_8_Condition,Section15_8_Comment=@Section15_8_Comment,Section15_9_Condition=@Section15_9_Condition,
                            Section15_9_Comment=@Section15_9_Comment,Section15_10_Condition=@Section15_10_Condition,Section15_10_Comment=@Section15_10_Comment,
                            Section15_11_Condition=@Section15_11_Condition,Section15_11_Comment=@Section15_11_Comment,Section15_12_Condition=@Section15_12_Condition,
                            Section15_12_Comment=@Section15_12_Comment,Section15_13_Condition=@Section15_13_Condition,Section15_13_Comment=@Section15_13_Comment,
                            Section15_14_Condition=@Section15_14_Condition,Section15_14_Comment=@Section15_14_Comment,Section15_15_Condition=@Section15_15_Condition,
                            Section15_15_Comment=@Section15_15_Comment,Section16_1_Condition=@Section16_1_Condition,Section16_1_Comment=@Section16_1_Comment,
                            Section16_2_Condition=@Section16_2_Condition,Section16_2_Comment=@Section16_2_Comment,Section16_3_Condition=@Section16_3_Condition,
                            Section16_3_Comment=@Section16_3_Comment,Section16_4_Condition=@Section16_4_Condition,Section16_4_Comment=@Section16_4_Comment,
                            Section17_1_Condition=@Section17_1_Condition,Section17_1_Comment=@Section17_1_Comment,Section17_2_Condition=@Section17_2_Condition,
                            Section17_2_Comment=@Section17_2_Comment,Section17_3_Condition=@Section17_3_Condition,Section17_3_Comment=@Section17_3_Comment,
                            Section17_4_Condition=@Section17_4_Condition,Section17_4_Comment=@Section17_4_Comment,Section17_5_Condition=@Section17_5_Condition,
                            Section17_5_Comment=@Section17_5_Comment,Section17_6_Condition=@Section17_6_Condition,Section17_6_Comment=@Section17_6_Comment,
                            Section18_1_Condition=@Section18_1_Condition,Section18_1_Comment=@Section18_1_Comment,Section18_2_Condition=@Section18_2_Condition,
                            Section18_2_Comment=@Section18_2_Comment,Section18_3_Condition=@Section18_3_Condition,Section18_4_Comment=@Section18_4_Comment,
                            Section18_5_Condition=@Section18_5_Condition,Section18_5_Comment=@Section18_5_Comment,Section18_6_Condition=@Section18_6_Condition,
                            Section18_6_Comment=@Section18_6_Comment,Section18_7_Condition=@Section18_7_Condition,Section18_7_Comment=@Section18_7_Comment,
                            Section18_8_Condition=@Section18_8_Condition,Section18_8_Comment=@Section18_8_Comment,
                            Section18_9_Condition=@Section18_9_Condition,Section18_9_Comment=@Section18_9_Comment,
                            IsSynced=@IsSynced,ModifyDate=@ModifyDate,SavedAsDraft=@SavedAsDraft WHERE UniqueFormID = @UniqueFormID";
            return query;
        }

        // RDBJ 04/01/2022
        public string CheckRecordsExistOrNot(string tableName, string columnName, Guid UniqueID)
        {
            try
            {
                ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                SqlConnection connection = new SqlConnection(connetionString);
                string res = string.Empty;
                connection.Open();
                DataTable dt1 = new DataTable();
                string isExistRecord = "SELECT " + columnName + " FROM " + tableName + " WHERE " + columnName + " = '" + UniqueID + "'";
                SqlDataAdapter sqlAdp1 = new SqlDataAdapter(isExistRecord, connection);
                sqlAdp1.Fill(dt1);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0][0] == DBNull.Value)
                        res = string.Empty;
                    else
                        res = Convert.ToString(dt1.Rows[0][0]);
                }
                connection.Close();
                return res;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckRecordsExistOrNot :" + ex.Message);
                return string.Empty;
            }
        }
        // End RDBJ 04/01/2022

        // RDBJ2 04/01/2022
        public string SIRNote_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[SIRNotes] ([SIRFormID], [Number], [Note], [UniqueFormID], [NotesUniqueID])
                                VALUES (@SIRFormID, @Number, @Note, @UniqueFormID, @NotesUniqueID)";
            return InsertQuery;
        }
        // End RDBJ2 04/01/2022

        // RDBJ2 04/01/2022
        public string SIRNote_UpdateQuery()
        {
            string UpdateQuery = @"UPDATE [dbo].[SIRNotes] 
                                SET [Number] = @Number, [Note] = @Note
                                WHERE [NotesUniqueID] = @NotesUniqueID";
            return UpdateQuery;
        }
        // End RDBJ2 04/01/2022

        // RDBJ2 04/01/2022
        public void SIRNoteInsertOrUpdate_CMD(SIRNote Modal, ref SqlCommand command
            , bool IsNeedToUpdate = false
            )
        {
            if (!IsNeedToUpdate)
            {
                command.Parameters.Add("@SIRFormID", SqlDbType.BigInt).Value = 0;
                command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            }
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = Modal.Number == null ? DBNull.Value : (object)Modal.Number;
            command.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Modal.Note == null ? DBNull.Value : (object)Modal.Note;
        }
        // End RDBJ2 04/01/2022

        // RDBJ2 04/01/2022
        public bool SIRNotesInsertOrUpdate(List<SIRNote> SIRNote, Guid UniqueFormID)
        {
            bool retBlnRes = false;
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                if (SIRNote != null && SIRNote.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in SIRNote)
                    {
                        string strId = CheckRecordsExistOrNot(AppStatic.SIRNotes, "NotesUniqueID", item.NotesUniqueID);
                        string strQuery = string.Empty;
                        bool blnNeedToUpdate = !string.IsNullOrEmpty(strId);

                        if (blnNeedToUpdate)
                        {
                            strQuery = SIRNote_UpdateQuery();
                        }
                        else
                        {
                            strQuery = SIRNote_InsertQuery();
                        }
                        item.UniqueFormID = UniqueFormID;
                        SqlCommand command = new SqlCommand(strQuery, connection);
                        SIRNoteInsertOrUpdate_CMD(item, ref command, blnNeedToUpdate);
                        connection.Open();

                        if (blnNeedToUpdate)
                            command.ExecuteNonQuery();
                        else
                            command.ExecuteScalar();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRNotesInsertOrUpdate Error : " + ex.Message.ToString());
            }

            return retBlnRes;
        }
        // End RDBJ2 04/01/2022

        // RDBJ2 04/01/2022
        public string SIRAdditionalNote_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO [dbo].[SIRAdditionalNotes] ([SIRFormID], [Number], [Note], [UniqueFormID], [NotesUniqueID])
                                VALUES (@SIRFormID, @Number, @Note, @UniqueFormID, @NotesUniqueID)";
            return InsertQuery;
        }
        // End RDBJ2 04/01/2022

        // RDBJ2 04/01/2022
        public string SIRAdditionalNote_UpdateQuery()
        {
            string UpdateQuery = @"UPDATE [dbo].[SIRAdditionalNotes] 
                                SET [Number] = @Number, [Note] = @Note
                                WHERE [NotesUniqueID] = @NotesUniqueID";
            return UpdateQuery;
        }
        // End RDBJ2 04/01/2022

        // RDBJ2 04/01/2022
        public void SIRAdditionalNoteInsertOrUpdate_CMD(SIRAdditionalNote Modal, ref SqlCommand command
            , bool IsNeedToUpdate = false
            )
        {
            if (!IsNeedToUpdate)
            {
                command.Parameters.Add("@SIRFormID", SqlDbType.BigInt).Value = 0;
                command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            }
            command.Parameters.Add("@NotesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.NotesUniqueID;
            command.Parameters.Add("@Number", SqlDbType.NVarChar).Value = Modal.Number == null ? DBNull.Value : (object)Modal.Number;
            command.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Modal.Note == null ? DBNull.Value : (object)Modal.Note;
        }
        // End RDBJ2 04/01/2022

        // RDBJ2 04/01/2022
        public bool SIRAdditionalNotesInsertOrUpdate(List<SIRAdditionalNote> SIRAdditionalNote, Guid UniqueFormID)
        {
            bool retBlnRes = false;
            ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                if (SIRAdditionalNote != null && SIRAdditionalNote.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in SIRAdditionalNote)
                    {
                        string strId = CheckRecordsExistOrNot(AppStatic.SIRAdditionalNotes, "NotesUniqueID", item.NotesUniqueID);
                        string strQuery = string.Empty;
                        bool blnNeedToUpdate = !string.IsNullOrEmpty(strId);

                        if (blnNeedToUpdate)
                        {
                            strQuery = SIRAdditionalNote_UpdateQuery();
                        }
                        else
                        {
                            strQuery = SIRAdditionalNote_InsertQuery();
                        }
                        item.UniqueFormID = UniqueFormID;
                        SqlCommand command = new SqlCommand(strQuery, connection);
                        SIRAdditionalNoteInsertOrUpdate_CMD(item, ref command, blnNeedToUpdate);

                        connection.Open();

                        if (blnNeedToUpdate)
                            command.ExecuteNonQuery();
                        else
                            command.ExecuteScalar();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SIRAdditionalNotesInsertOrUpdate Error : " + ex.Message.ToString());
            }

            return retBlnRes;
        }
        // End RDBJ2 04/01/2022

        public bool SaveSIRNotesDataInLocalDB(List<SIRNote> SIRNote, Guid UniqueFormID)
        {
            bool res = false;
            try
            {
                if (SIRNote != null && SIRNote.Count > 0 && UniqueFormID !=  Guid.Empty)
                {
                    foreach (var item in SIRNote)
                    {
                        item.SIRFormID = 0; //SIRFormID; //RDBJ 10/13/2021 set with 0
                        item.UniqueFormID = UniqueFormID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.SIRNotes);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.SIRNotes); }
                    if (isTbaleCreated)
                    {
                        
                        bool resp = DeleteRecords(AppStatic.SIRNotes, "UniqueFormID", Convert.ToString(UniqueFormID));
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(SIRNote);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.SIRNotes;
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
        public bool SaveSIRAdditionalNotesDataInLocalDB(List<SIRAdditionalNote> SIRAdditionalNote, Guid UniqueFormID)
        {
            bool res = false;
            try
            {
                if (SIRAdditionalNote != null && SIRAdditionalNote.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    foreach (var item in SIRAdditionalNote)
                    {
                        item.SIRFormID = 0; // SIRFormID; //RDBJ 10/13/2021 set with 0
                        item.UniqueFormID = UniqueFormID;
                    }
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.SIRAdditionalNotes);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.SIRAdditionalNotes); }
                    if (isTbaleCreated)
                    {
                        bool resp = DeleteRecords(AppStatic.SIRAdditionalNotes, "UniqueFormID", Convert.ToString(UniqueFormID));
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            DataTable dt = Utility.ToDataTable(SIRAdditionalNote);
                            using (SqlConnection connection = new SqlConnection(ConnectionString))
                            {
                                SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                                bulkCopy.DestinationTableName = AppStatic.SIRAdditionalNotes;
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
                LogHelper.writelog("Add Local DB in SaveSIRAdditionalNotesDataInLocalDB table Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }

        public string AutoSaveSIRDataInLocalDB(SIRModal Modal)
        {
            string UniqueFormID = string.Empty;
            try
            {
                Modal.SuperintendedInspectionReport.IsSynced = false; //RDBJ 10/13/2021 set global
                if (Modal.SuperintendedInspectionReport.UniqueFormID != Guid.Empty)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        string UpdateQury = GetSIRUpdateQuery();
                        SqlConnection connection = new SqlConnection(connetionString);
                        SqlCommand command = new SqlCommand(UpdateQury, connection);
                        Modal.SuperintendedInspectionReport.ModifyDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                        Modal.SuperintendedInspectionReport.UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID;

                        SIRUpdateCMD(Modal.SuperintendedInspectionReport, ref command);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        UniqueFormID = Modal.SuperintendedInspectionReport.UniqueFormID.ToString();
                    }
                }
                else
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.SuperintendedInspectionReport);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.SuperintendedInspectionReport); }
                    if (isTbaleCreated)
                    {
                       ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                            string InsertQury = SIRDataInsertQuery();
                            SqlConnection connection = new SqlConnection(connetionString);
                            SqlCommand command = new SqlCommand(InsertQury, connection);
                            Guid FormGUID = Guid.NewGuid();
                            Modal.SuperintendedInspectionReport.UniqueFormID = FormGUID;
                            Modal.SuperintendedInspectionReport.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime

                            SIRDataInsertCMD(Modal.SuperintendedInspectionReport, ref command);
                            connection.Open();
                            command.ExecuteScalar();
                            UniqueFormID = FormGUID.ToString();
                            connection.Close();
                        }
                    }
                }
                //RDBJ 10/13/2021
                if (!string.IsNullOrEmpty(UniqueFormID))
                {
                    if (Modal.NotesChanged)
                        //SaveSIRNotesDataInLocalDB(Modal.SIRNote, Modal.SuperintendedInspectionReport.UniqueFormID);   // RDBJ2 04/01/2022 Commented this line
                        SIRNotesInsertOrUpdate(Modal.SIRNote, Modal.SuperintendedInspectionReport.UniqueFormID);    // RDBJ2 04/01/2022

                    if (Modal.AdditionalNotesChanged)
                        //SaveSIRAdditionalNotesDataInLocalDB(Modal.SIRAdditionalNote, Modal.SuperintendedInspectionReport.UniqueFormID);   // RDBJ2 04/01/2022 Commented this line
                        SIRAdditionalNotesInsertOrUpdate(Modal.SIRAdditionalNote, Modal.SuperintendedInspectionReport.UniqueFormID);    // RDBJ2 04/01/2022

                    if (Modal.DeficienciesChanged)
                        GIRDeficiencies_Save(Modal.SuperintendedInspectionReport.UniqueFormID, Modal.GIRDeficiencies);
                }
                //End RDBJ 10/13/2021
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in SuperintendedInspectionReport table Error : " + ex.Message.ToString());
            }
            return UniqueFormID;
        }
        public string GETSIRUpdateQuery()
        {
            string query = @"UPDATE [dbo].[SuperintendedInspectionReport] SET [ShipID] = @ShipID,[ShipName] = @ShipName,[Date] = @Date,[Port] = @Port,[Master] = @Master,[Superintended] = @Superintended,[Section1_1_Condition] = @Section1_1_Condition,[Section1_1_Comment] = @Section1_1_Comment,[Section1_2_Condition] = @Section1_2_Condition,[Section1_2_Comment] = @Section1_2_Comment,[Section1_3_Condition] = @Section1_3_Condition,[Section1_3_Comment] = @Section1_3_Comment,[Section1_4_Condition] = @Section1_4_Condition,[Section1_4_Comment] = @Section1_4_Comment,[Section1_5_Condition] = @Section1_5_Condition,[Section1_5_Comment] = @Section1_5_Comment,[Section1_6_Condition] = @Section1_6_Condition,[Section1_6_Comment] = @Section1_6_Comment,[Section1_7_Condition] = @Section1_7_Condition,[Section1_7_Comment] = @Section1_7_Comment,[Section1_8_Condition] = @Section1_8_Condition,[Section1_8_Comment] = @Section1_8_Comment,[Section1_9_Condition] = @Section1_9_Condition,[Section1_9_Comment] = @Section1_9_Comment,[Section1_10_Condition] = @Section1_10_Condition,[Section1_10_Comment] = @Section1_10_Comment,[Section1_11_Condition] = @Section1_11_Condition,[Section1_11_Comment] = @Section1_11_Comment,[Section2_1_Condition] = @Section2_1_Condition,[Section2_1_Comment] = @Section2_1_Comment,[Section2_2_Condition] = @Section2_2_Condition,[Section2_2_Comment] = @Section2_2_Comment,[Section2_3_Condition] = @Section2_3_Condition,[Section2_3_Comment] = @Section2_3_Comment,[Section2_4_Condition] = @Section2_4_Condition,[Section2_4_Comment] = @Section2_4_Comment,[Section2_5_Condition] = @Section2_5_Condition,[Section2_5_Comment] = @Section2_5_Comment,[Section2_6_Condition] = @Section2_6_Condition,[Section2_6_Comment] = @Section2_6_Comment,[Section2_7_Condition] = @Section2_7_Condition,[Section2_7_Comment] = @Section2_7_Comment,[Section3_1_Condition] = @Section3_1_Condition,[Section3_1_Comment] = @Section3_1_Comment,[Section3_2_Condition] = @Section3_2_Condition,[Section3_2_Comment] = @Section3_2_Comment,[Section3_3_Condition] = @Section3_3_Condition,[Section3_3_Comment] = @Section3_3_Comment,[Section3_4_Condition] = @Section3_4_Condition,[Section3_4_Comment] = @Section3_4_Comment,[Section3_5_Condition] = @Section3_5_Condition,[Section3_5_Comment] = @Section3_5_Comment,[Section4_1_Condition] = @Section4_1_Condition,[Section4_1_Comment] = @Section4_1_Comment,[Section4_2_Condition] = @Section4_2_Condition,[Section4_2_Comment] = @Section4_2_Comment,[Section4_3_Condition] = @Section4_3_Condition,[Section4_3_Comment] = @Section4_3_Comment,[Section5_1_Condition] = @Section5_1_Condition,[Section5_1_Comment] = @Section5_1_Comment,[Section5_6_Condition] = @Section5_6_Condition,[Section5_6_Comment] = @Section5_6_Comment,[Section5_8_Condition] = @Section5_8_Condition,[Section5_8_Comment] = @Section5_8_Comment,[Section5_9_Condition] = @Section5_9_Condition,[Section5_9_Comment] = @Section5_9_Comment,[Section6_1_Condition] = @Section6_1_Condition,[Section6_1_Comment] = @Section6_1_Comment,[Section6_2_Condition] = @Section6_2_Condition,[Section6_2_Comment] = @Section6_2_Comment,[Section6_3_Condition] = @Section6_3_Condition,[Section6_3_Comment] = @Section6_3_Comment,[Section6_4_Condition] = @Section6_4_Condition,[Section6_4_Comment] = @Section6_4_Comment,[Section6_5_Condition] = @Section6_5_Condition,[Section6_5_Comment] = @Section6_5_Comment,[Section6_6_Condition] = @Section6_6_Condition,[Section6_6_Comment] = @Section6_6_Comment,[Section6_7_Condition] = @Section6_7_Condition,[Section6_7_Comment] = @Section6_7_Comment,[Section6_8_Condition] = @Section6_8_Condition,[Section6_8_Comment] = @Section6_8_Comment,[Section7_1_Condition] = @Section7_1_Condition,[Section7_1_Comment] = @Section7_1_Comment,[Section7_2_Condition] = @Section7_2_Condition,[Section7_2_Comment] = @Section7_2_Comment,[Section7_3_Condition] = @Section7_3_Condition,[Section7_3_Comment] = @Section7_3_Comment,[Section7_4_Condition] = @Section7_4_Condition,[Section7_4_Comment] = @Section7_4_Comment,[Section7_5_Condition] = @Section7_5_Condition,[Section7_5_Comment] = @Section7_5_Comment,[Section7_6_Condition] = @Section7_6_Condition,[Section7_6_Comment] = @Section7_6_Comment,[Section8_1_Condition] = @Section8_1_Condition,[Section8_1_Comment] = @Section8_1_Comment,[Section8_2_Condition] = @Section8_2_Condition,[Section8_2_Comment] = @Section8_2_Comment,[Section8_3_Condition] = @Section8_3_Condition,[Section8_3_Comment] = @Section8_3_Comment,[Section8_4_Condition] = @Section8_4_Condition,[Section8_4_Comment] = @Section8_4_Comment,[Section8_5_Condition] = @Section8_5_Condition,[Section8_5_Comment] = @Section8_5_Comment,[Section8_6_Condition] = @Section8_6_Condition,[Section8_6_Comment] = @Section8_6_Comment,[Section8_7_Condition] = @Section8_7_Condition,[Section8_7_Comment] = @Section8_7_Comment,[Section8_8_Condition] = @Section8_8_Condition,[Section8_8_Comment] = @Section8_8_Comment,[Section8_9_Condition] = @Section8_9_Condition,[Section8_9_Comment] = @Section8_9_Comment,[Section8_10_Condition] = @Section8_10_Condition,[Section8_10_Comment] = @Section8_10_Comment,[Section8_11_Condition] = @Section8_11_Condition,[Section8_11_Comment] = @Section8_11_Comment,[Section8_12_Condition] = @Section8_12_Condition,[Section8_12_Comment] = @Section8_12_Comment,[Section8_13_Condition] = @Section8_13_Condition,[Section8_13_Comment] = @Section8_13_Comment,[Section8_14_Condition] = @Section8_14_Condition,[Section8_14_Comment] = @Section8_14_Comment,[Section8_15_Condition] = @Section8_15_Condition,[Section8_15_Comment] = @Section8_15_Comment,[Section8_16_Condition] = @Section8_16_Condition,[Section8_16_Comment] = @Section8_16_Comment,[Section8_17_Condition] = @Section8_17_Condition,[Section8_17_Comment] = @Section8_17_Comment,[Section8_18_Condition] = @Section8_18_Condition,[Section8_18_Comment] = @Section8_18_Comment,[Section8_19_Condition] = @Section8_19_Condition,[Section8_19_Comment] = @Section8_19_Comment,[Section8_20_Condition] = @Section8_20_Condition,[Section8_20_Comment] = @Section8_20_Comment,[Section8_21_Condition] = @Section8_21_Condition,[Section8_21_Comment] = @Section8_21_Comment,[Section8_22_Condition] = @Section8_22_Condition,[Section8_22_Comment] = @Section8_22_Comment,[Section8_23_Condition] = @Section8_23_Condition,[Section8_23_Comment] = @Section8_23_Comment,[Section8_24_Condition] = @Section8_24_Condition,[Section8_24_Comment] = @Section8_24_Comment,[Section8_25_Condition] = @Section8_25_Condition,[Section8_25_Comment] = @Section8_25_Comment,[Section9_1_Condition] = @Section9_1_Condition,[Section9_1_Comment] = @Section9_1_Comment,[Section9_2_Condition] = @Section9_2_Condition,[Section9_2_Comment] = @Section9_2_Comment,[Section9_3_Condition] = @Section9_3_Condition,[Section9_3_Comment] = @Section9_3_Comment,[Section9_4_Condition] = @Section9_4_Condition,[Section9_4_Comment] = @Section9_4_Comment,[Section9_5_Condition] = @Section9_5_Condition,[Section9_5_Comment] = @Section9_5_Comment,[Section9_6_Condition] = @Section9_6_Condition,[Section9_6_Comment] = @Section9_6_Comment,[Section9_7_Condition] = @Section9_7_Condition,[Section9_7_Comment] = @Section9_7_Comment,[Section9_8_Condition] = @Section9_8_Condition,[Section9_8_Comment] = @Section9_8_Comment,[Section9_9_Condition] = @Section9_9_Condition,[Section9_9_Comment] = @Section9_9_Comment,[Section9_10_Condition] = @Section9_10_Condition,[Section9_10_Comment] = @Section9_10_Comment,[Section9_11_Condition] = @Section9_11_Condition,[Section9_11_Comment] = @Section9_11_Comment,[Section9_12_Condition] = @Section9_12_Condition,[Section9_12_Comment] = @Section9_12_Comment,[Section9_13_Condition] = @Section9_13_Condition,[Section9_13_Comment] = @Section9_13_Comment,[Section9_14_Condition] = @Section9_14_Condition,[Section9_14_Comment] = @Section9_14_Comment,[Section9_15_Condition] = @Section9_15_Condition,[Section9_15_Comment] = @Section9_15_Comment,[Section10_1_Condition] = @Section10_1_Condition,[Section10_1_Comment] = @Section10_1_Comment,[Section10_2_Condition] = @Section10_2_Condition,[Section10_2_Comment] = @Section10_2_Comment,[Section10_3_Condition] = @Section10_3_Condition,[Section10_3_Comment] = @Section10_3_Comment,[Section10_4_Condition] = @Section10_4_Condition,[Section10_4_Comment] = @Section10_4_Comment,[Section10_5_Condition] = @Section10_5_Condition,[Section10_5_Comment] = @Section10_5_Comment,[Section10_6_Condition] = @Section10_6_Condition,[Section10_6_Comment] = @Section10_6_Comment,[Section10_7_Condition] = @Section10_7_Condition,[Section10_7_Comment] = @Section10_7_Comment,[Section10_8_Condition] = @Section10_8_Condition,[Section10_8_Comment] = @Section10_8_Comment,[Section10_9_Condition] = @Section10_9_Condition,[Section10_9_Comment] = @Section10_9_Comment,[Section10_10_Condition] = @Section10_10_Condition,[Section10_10_Comment] = @Section10_10_Comment,[Section10_11_Condition] = @Section10_11_Condition,[Section10_11_Comment] = @Section10_11_Comment,[Section10_12_Condition] = @Section10_12_Condition,[Section10_12_Comment] = @Section10_12_Comment,[Section10_13_Condition] = @Section10_13_Condition,[Section10_13_Comment] = @Section10_13_Comment,[Section10_14_Condition] = @Section10_14_Condition,[Section10_14_Comment] = @Section10_14_Comment,[Section10_15_Condition] = @Section10_15_Condition,[Section10_15_Comment] = @Section10_15_Comment,[Section10_16_Condition] = @Section10_16_Condition,[Section10_16_Comment] = @Section10_16_Comment,[Section11_1_Condition] = @Section11_1_Condition,[Section11_1_Comment] = @Section11_1_Comment,[Section11_2_Condition] = @Section11_2_Condition,[Section11_2_Comment] = @Section11_2_Comment,[Section11_3_Condition] = @Section11_3_Condition,[Section11_3_Comment] = @Section11_3_Comment,[Section11_4_Condition] = @Section11_4_Condition,[Section11_4_Comment] = @Section11_4_Comment,[Section11_5_Condition] = @Section11_5_Condition,[Section11_5_Comment] = @Section11_5_Comment,[Section11_6_Condition] = @Section11_6_Condition,[Section11_6_Comment] = @Section11_6_Comment,[Section11_7_Condition] = @Section11_7_Condition,[Section11_7_Comment] = @Section11_7_Comment,[Section11_8_Condition] = @Section11_8_Condition,[Section11_8_Comment] = @Section11_8_Comment,[Section12_1_Condition] = @Section12_1_Condition,[Section12_1_Comment] = @Section12_1_Comment,[Section12_2_Condition] = @Section12_2_Condition,[Section12_2_Comment] = @Section12_2_Comment,[Section12_3_Condition] = @Section12_3_Condition,[Section12_3_Comment] = @Section12_3_Comment,[Section12_4_Condition] = @Section12_4_Condition,[Section12_4_Comment] = @Section12_4_Comment,[Section12_5_Condition] = @Section12_5_Condition,[Section12_5_Comment] = @Section12_5_Comment,[Section12_6_Condition] = @Section12_6_Condition,[Section12_6_Comment] = @Section12_6_Comment,[Section13_1_Condition] = @Section13_1_Condition,[Section13_1_Comment] = @Section13_1_Comment,[Section13_2_Condition] = @Section13_2_Condition,[Section13_2_Comment] = @Section13_2_Comment,[Section13_3_Condition] = @Section13_3_Condition,[Section13_3_Comment] = @Section13_3_Comment,[Section13_4_Condition] = @Section13_4_Condition,[Section13_4_Comment] = @Section13_4_Comment,[Section14_1_Condition] = @Section14_1_Condition,[Section14_1_Comment] = @Section14_1_Comment,[Section14_2_Condition] = @Section14_2_Condition,[Section14_2_Comment] = @Section14_2_Comment,[Section14_3_Condition] = @Section14_3_Condition,[Section14_3_Comment] = @Section14_3_Comment,[Section14_4_Condition] = @Section14_4_Condition,[Section14_4_Comment] = @Section14_4_Comment,[Section14_5_Condition] = @Section14_5_Condition,[Section14_5_Comment] = @Section14_5_Comment,[Section14_6_Condition] = @Section14_6_Condition,[Section14_6_Comment] = @Section14_6_Comment,[Section14_7_Condition] = @Section14_7_Condition,[Section14_7_Comment] = @Section14_7_Comment,[Section14_8_Condition] = @Section14_8_Condition,[Section14_8_Comment] = @Section14_8_Comment,[Section14_9_Condition] = @Section14_9_Condition,[Section14_9_Comment] = @Section14_9_Comment,[Section14_10_Condition] = @Section14_10_Condition,[Section14_10_Comment] = @Section14_10_Comment,[Section14_11_Condition] = @Section14_11_Condition,[Section14_11_Comment] = @Section14_11_Comment,[Section14_12_Condition] = @Section14_12_Condition,[Section14_12_Comment] = @Section14_12_Comment,[Section14_13_Condition] = @Section14_13_Condition,[Section14_13_Comment] = @Section14_13_Comment,[Section14_14_Condition] = @Section14_14_Condition,[Section14_14_Comment] = @Section14_14_Comment,[Section14_15_Condition] = @Section14_15_Condition,[Section14_15_Comment] = @Section14_15_Comment,[Section14_16_Condition] = @Section14_16_Condition,[Section14_16_Comment] = @Section14_16_Comment,[Section14_17_Condition] = @Section14_17_Condition,[Section14_17_Comment] = @Section14_17_Comment,[Section14_18_Condition] = @Section14_18_Condition,[Section14_18_Comment] = @Section14_18_Comment,[Section14_19_Condition] = @Section14_19_Condition,[Section14_19_Comment] = @Section14_19_Comment,[Section14_20_Condition] = @Section14_20_Condition,[Section14_20_Comment] = @Section14_20_Comment,[Section14_21_Condition] = @Section14_21_Condition,[Section14_21_Comment] = @Section14_21_Comment,[Section14_22_Condition] = @Section14_22_Condition,[Section14_22_Comment] = @Section14_22_Comment,[Section14_23_Condition] = @Section14_23_Condition,[Section14_23_Comment] = @Section14_23_Comment,[Section14_24_Condition] = @Section14_24_Condition,[Section14_24_Comment] = @Section14_24_Comment,[Section14_25_Condition] = @Section14_25_Condition,[Section14_25_Comment] = @Section14_25_Comment,[Section15_1_Condition] = @Section15_1_Condition,[Section15_1_Comment] = @Section15_1_Comment,[Section15_2_Condition] = @Section15_2_Condition,[Section15_2_Comment] = @Section15_2_Comment,[Section15_3_Condition] = @Section15_3_Condition,[Section15_3_Comment] = @Section15_3_Comment,[Section15_4_Condition] = @Section15_4_Condition,[Section15_4_Comment] = @Section15_4_Comment,[Section15_5_Condition] = @Section15_5_Condition,[Section15_5_Comment] = @Section15_5_Comment,[Section15_6_Condition] = @Section15_6_Condition,[Section15_6_Comment] = @Section15_6_Comment,[Section15_7_Condition] = @Section15_7_Condition,[Section15_7_Comment] = @Section15_7_Comment,[Section15_8_Condition] = @Section15_8_Condition,[Section15_8_Comment] = @Section15_8_Comment,[Section15_9_Condition] = @Section15_9_Condition,[Section15_9_Comment] = @Section15_9_Comment,[Section15_10_Condition] = @Section15_10_Condition,[Section15_10_Comment] = @Section15_10_Comment,[Section15_11_Condition] = @Section15_11_Condition,[Section15_11_Comment] = @Section15_11_Comment,[Section15_12_Condition] = @Section15_12_Condition,[Section15_12_Comment] = @Section15_12_Comment,[Section15_13_Condition] = @Section15_13_Condition,[Section15_13_Comment] = @Section15_13_Comment,[Section15_14_Condition] = @Section15_14_Condition,[Section15_14_Comment] = @Section15_14_Comment,[Section15_15_Condition] = @Section15_15_Condition,[Section15_15_Comment] = @Section15_15_Comment,[Section16_1_Condition] = @Section16_1_Condition,[Section16_1_Comment] = @Section16_1_Comment,[Section16_2_Condition] = @Section16_2_Condition,[Section16_2_Comment] = @Section16_2_Comment,[Section16_3_Condition] = @Section16_3_Condition,[Section16_3_Comment] = @Section16_3_Comment,[Section16_4_Condition] = @Section16_4_Condition,[Section16_4_Comment] = @Section16_4_Comment,[Section17_1_Condition] = @Section17_1_Condition,[Section17_1_Comment] = @Section17_1_Comment,[Section17_2_Condition] = @Section17_2_Condition,[Section17_2_Comment] = @Section17_2_Comment,[Section17_3_Condition] = @Section17_3_Condition,[Section17_3_Comment] = @Section17_3_Comment,[Section17_4_Condition] = @Section17_4_Condition,[Section17_4_Comment] = @Section17_4_Comment,[Section17_5_Condition] = @Section17_5_Condition,[Section17_5_Comment] = @Section17_5_Comment,[Section17_6_Condition] = @Section17_6_Condition,[Section17_6_Comment] = @Section17_6_Comment,[Section18_1_Condition] = @Section18_1_Condition,[Section18_1_Comment] = @Section18_1_Comment,[Section18_2_Condition] = @Section18_2_Condition,[Section18_2_Comment] = @Section18_2_Comment,[Section18_3_Condition] = @Section18_3_Condition,[Section18_3_Comment] = @Section18_3_Comment,[Section18_4_Condition] = @Section18_4_Condition,[Section18_4_Comment] = @Section18_4_Comment,[Section18_5_Condition] = @Section18_5_Condition,[Section18_5_Comment] = @Section18_5_Comment,[Section18_6_Condition] = @Section18_6_Condition,[Section18_6_Comment] = @Section18_6_Comment,[Section18_7_Condition] = @Section18_7_Condition,[Section18_7_Comment] = @Section18_7_Comment,[IsSynced] = @IsSynced,[CreatedDate] = @CreatedDate,[ModifyDate] = @ModifyDate,[SavedAsDraft] = @SavedAsDraft 
                             WHERE SIRFormID = @SIRFormID";
            return query;
        }

        public bool AddSIRDeficienciesInLocalDB(SIRDeficiencies Modal, bool isInternetAvailable)
        {
            bool res = false;
            try
            {
                if (Modal != null && Modal.SIRFormID.HasValue && Modal.SIRFormID.Value > 0)
                {
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.SIRDeficiencies);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.SIRDeficiencies); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);

                            string InsertQuery = SIRDeficiencies_InsertQuery();
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            connection.Open();
                            try
                            {
                                //If internet not available then get No.
                                if (!isInternetAvailable)
                                {
                                    DataTable dt1 = new DataTable();
                                    string numberQuery = "select MAX(No) from " + AppStatic.SIRDeficiencies + " where [Ship]='" + Modal.Ship + "'";
                                    SqlDataAdapter sqlAdp = new SqlDataAdapter(numberQuery, connection);
                                    sqlAdp.Fill(dt1);
                                    if (dt1 != null && dt1.Rows.Count > 0)
                                    {
                                        if (dt1.Rows[0][0] == DBNull.Value)
                                            Modal.No = 1;
                                        else
                                            Modal.No = Convert.ToInt32(dt1.Rows[0][0]) + 1;
                                    }
                                }
                                //Update Deficiencies
                                if (Modal.Section != "")
                                {
                                    DataTable dt2 = new DataTable();
                                    string getDefQuery = "select DeficienciesID from " + AppStatic.SIRDeficiencies + " where [Section]='" + Modal.Section + "' and (ItemNo='" + Modal.ItemNo + "' or ItemNo is null)  and SIRFormID=" + Modal.SIRFormID + "";
                                    SqlDataAdapter sqlAdp = new SqlDataAdapter(getDefQuery, connection);
                                    sqlAdp.Fill(dt2);
                                    if (dt2 != null && dt2.Rows.Count > 0)
                                    {
                                        string UpdateQury = @"UPDATE dbo.SIRDeficiencies SET Deficiency = @Deficiency WHERE DeficienciesID = @DeficienciesID";
                                        SqlCommand commands = new SqlCommand(UpdateQury, connection);
                                        commands.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency;
                                        commands.Parameters.Add("@DeficienciesID", SqlDbType.Int).Value = Convert.ToInt32(dt2.Rows[0][0]);
                                        commands.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                                        SIRDeficiencies_CMD(Modal, ref command);
                                        object resultObj = command.ExecuteScalar();
                                        long databaseID = 0;
                                        if (resultObj != null)
                                        {
                                            long.TryParse(resultObj.ToString(), out databaseID);
                                        }
                                        if (databaseID > 0)
                                        {
                                            try
                                            {
                                                if (!Modal.IsSynced)
                                                {
                                                    //Update IsSyncValue in GeneralInspectionReport local DB
                                                    var UpdateQury = "UPDATE " + AppStatic.SuperintendedInspectionReport + " SET UpdatedDate=@UpdatedDate,IsSynced=@IsSynced WHERE SIRFormID = " + Modal.SIRFormID;
                                                    command = new SqlCommand(UpdateQury, connection);
                                                    command.Parameters.Add("@UpdatedDate", SqlDbType.Date).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                                    command.Parameters.Add("@IsSynced", SqlDbType.Bit).Value = Modal.IsSynced;
                                                    command.ExecuteNonQuery();
                                                }
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.writelog("Failed to add Deficiencies : " + ex.Message.ToString());
                                res = false;
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in AddSIRDeficiencies table Error : " + ex.Message.ToString());
            }
            return res;
        }
        public string SIRDeficiencies_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.SIRDeficiencies 
                                  (SIRFormID,No,DateRaised,Deficiency,DateClosed,CreatedDate,UpdatedDate,Ship,IsClose,ReportType,FileName,StorePath,SIRNo,ItemNo,Section)
                                  OUTPUT INSERTED.DeficienciesID
                                  VALUES (@SIRFormID,@No,@DateRaised,@Deficiency,@DateClosed,@CreatedDate,@UpdatedDate,@Ship,@IsClose,@ReportType,@FileName,@StorePath,@SIRNo,@ItemNo,@Section)";
            return InsertQuery;
        }
        public void SIRDeficiencies_CMD(SIRDeficiencies Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@SIRFormID", SqlDbType.BigInt).Value = Modal.SIRFormID;
            command.Parameters.Add("@No", SqlDbType.Int).Value = Modal.No;
            command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency == null ? DBNull.Value : (object)Modal.Deficiency;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
            command.Parameters.Add("@IsClose", SqlDbType.Bit).Value = Modal.IsClose == null ? false : (object)Modal.IsClose;
            command.Parameters.Add("@ReportType", SqlDbType.NVarChar).Value = "SI";
            //command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.Ship;
            //command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.Ship
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@SIRNo", SqlDbType.NVarChar).Value = 0;
            command.Parameters.Add("@ItemNo", SqlDbType.NVarChar).Value = Modal.ItemNo == null ? DBNull.Value : (object)Modal.ItemNo;
            command.Parameters.Add("@Section", SqlDbType.NVarChar).Value = Modal.Section == null ? DBNull.Value : (object)Modal.Section;
        }

        public bool DeleteRecords(string tablename, string columnname, string RecID)
        {
            try
            {
                bool isTableExist = LocalDBHelper.CheckTableExist(tablename);
                bool isTableCreated = true;
                if (!isTableExist) { isTableCreated = LocalDBHelper.CreateTable(tablename); }
                if (isTableCreated)
                {
                    bool isColumnGUID = LocalDBHelper.CheckTableColumnExist(tablename, columnname);
                    if (!isColumnGUID)
                        LocalDBHelper.ExecuteQuery("ALTER TABLE " + tablename + " ADD " + columnname + " uniqueidentifier NULL");

                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string connetionString = Utility.GetLocalDBConnStr(dbConnModal);
                        using (SqlConnection conn = new SqlConnection(connetionString))
                        {
                            conn.Open();
                            using (SqlCommand command = new SqlCommand("DELETE FROM " + tablename + " WHERE " + columnname + " = '" + RecID + "'", conn))
                            {
                                command.ExecuteNonQuery();
                            }
                            conn.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeleteRecords Local DB in table Error : " + ex.Message.ToString());
                return false;
            }
        }

        //RDBJ 10/13/2021
        public GIRDeficiencies GIRDefRecordsExist(string tablename, Guid UniqueFormID, long No)
        {
            GIRDeficiencies defDetails = new GIRDeficiencies();
            try
            {
                int res = 0;
                bool isTableExist = LocalDBHelper.CheckTableExist(tablename);
                bool isTableCreated = true;
                if (!isTableExist) { isTableCreated = LocalDBHelper.CreateTable(tablename); }
                if (isTableCreated)
                {
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                    {
                        string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                        SqlConnection connection = new SqlConnection(ConnectionString);
                        connection.Open();
                        DataTable dt1 = new DataTable();
                        string isExistRecord = "select DeficienciesID,DeficienciesUniqueID from " + AppStatic.GIRDeficiencies + " where UniqueFormID = '" + UniqueFormID + "' AND [No] = " + No + "";
                        SqlDataAdapter sqlAdp1 = new SqlDataAdapter(isExistRecord, connection);
                        sqlAdp1.Fill(dt1);
                        connection.Close(); //RDBJ 10/17/2021
                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            if (dt1.Rows[0][0] == DBNull.Value)
                            {
                                //res = 0;
                                return defDetails;
                            }
                            else
                            {
                                res = Convert.ToInt32(dt1.Rows[0][0]);
                                defDetails.DeficienciesID = Convert.ToInt32(dt1.Rows[0][0]);
                                defDetails.DeficienciesUniqueID = new Guid(Convert.ToString(dt1.Rows[0][1]));
                            }
                        }
                    }
                }
                return defDetails;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeficienciesNumber :" + ex.Message);
                return defDetails;
            }
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public void GIRDeficiencies_Save(Guid UniqueFormID, List<GIRDeficiencies> GIRDeficiencies)
        {
            try
            {
                if (GIRDeficiencies != null && GIRDeficiencies.Count > 0 && UniqueFormID != Guid.Empty)
                {
                    GIRDeficiencies defDetails = new GIRDeficiencies();
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficiencies);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficiencies); }
                    ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                    string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                    SqlConnection connection = new SqlConnection(ConnectionString);
                    if (isTbaleCreated)
                    {
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            connection.Open();
                            foreach (var item in GIRDeficiencies)
                            {
                                defDetails = GIRDefRecordsExist(AppStatic.GIRDeficiencies, UniqueFormID, item.No);
                                try
                                {
                                    if (defDetails.DeficienciesID > 0)
                                    {
                                        string UpdateQuery = GIRDeficiencies_UpdateQuery();
                                        SqlCommand command = new SqlCommand(UpdateQuery, connection);
                                        item.UniqueFormID = UniqueFormID;

                                        //RDBJ 10/26/2021 set isClose logic and wrapped in if
                                        if (item.DateClosed != null)
                                            item.IsClose = true;
                                        else
                                            item.IsClose = false;

                                        GIRDeficiencies_UpdateCMD(item, ref command);
                                        command.ExecuteNonQuery();
                                        //GIRDeficienciesFiles_Save(item.GIRDeficienciesFile, defDetails.DeficienciesUniqueID);
                                    }
                                    else
                                    {
                                        item.CreatedDate = item.CreatedDate ?? Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                                        string InsertQuery = GIRDeficiencies_InsertQuery();
                                        SqlCommand command = new SqlCommand(InsertQuery, connection);
                                        item.UniqueFormID = UniqueFormID;
                                        item.DeficienciesUniqueID = Guid.NewGuid();

                                        //RDBJ 10/26/2021 set isClose logic and wrapped in if
                                        if (item.DateClosed != null)
                                            item.IsClose = true;
                                        else
                                            item.IsClose = false;

                                        GIRDeficiencies_CMD(item, ref command);
                                        command.ExecuteScalar();
                                        //GIRDeficienciesFiles_Save(item.GIRDeficienciesFile, item.DeficienciesUniqueID);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.writelog("Failed to add Deficiencies : " + ex.Message.ToString());

                                }
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRDeficiencies_Save table Error : " + ex.Message.ToString());
            }
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public string GIRDeficiencies_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRDeficiencies 
                                  (UniqueFormID,No,DateRaised,Deficiency,DateClosed,CreatedDate,UpdatedDate,Ship,IsClose,ReportType,FileName,StorePath,SIRNo,ItemNo,Section,isDelete,DeficienciesUniqueID)
                                  OUTPUT INSERTED.DeficienciesID
                                  VALUES (@UniqueFormID,@No,@DateRaised,@Deficiency,@DateClosed,@CreatedDate,@UpdatedDate,@Ship,@IsClose,@ReportType,@FileName,@StorePath,@SIRNo,@ItemNo,@Section,@isDelete,@DeficienciesUniqueID)";
            return InsertQuery;
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public void GIRDeficiencies_CMD(GIRDeficiencies Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@isDelete", SqlDbType.Int).Value = 0;
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            command.Parameters.Add("@No", SqlDbType.Int).Value = Modal.No;
            command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency == null ? DBNull.Value : (object)Modal.Deficiency;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Ship", SqlDbType.NVarChar).Value = Modal.Ship == null ? DBNull.Value : (object)Modal.Ship;
            command.Parameters.Add("@IsClose", SqlDbType.Bit).Value = Modal.IsClose == null ? false : (object)Modal.IsClose;
            command.Parameters.Add("@ReportType", SqlDbType.NVarChar).Value = Modal.ReportType;//"GI"
            //command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.Ship;
            //command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.Ship
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = string.Empty;
            command.Parameters.Add("@SIRNo", SqlDbType.NVarChar).Value = 0;
            command.Parameters.Add("@ItemNo", SqlDbType.NVarChar).Value = Modal.ItemNo == null ? DBNull.Value : (object)Modal.ItemNo;
            command.Parameters.Add("@Section", SqlDbType.NVarChar).Value = Modal.Section == null ? DBNull.Value : (object)Modal.Section;
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public string GIRDeficiencies_UpdateQuery()
        {
            //RDBJ 10/26/2021 Added IsClose = @IsClose
            string UpdateQuery = @"UPDATE dbo.GIRDeficiencies SET
                                DateRaised = @DateRaised, Deficiency = @Deficiency, DateClosed = @DateClosed,
                                UpdatedDate = @UpdatedDate, IsClose = @IsClose
                                WHERE No = @No and UniqueFormID = @UniqueFormID";
            return UpdateQuery;
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public void GIRDeficiencies_UpdateCMD(GIRDeficiencies Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@UniqueFormID", SqlDbType.UniqueIdentifier).Value = Modal.UniqueFormID;
            command.Parameters.Add("@No", SqlDbType.Int).Value = Modal.No;
            command.Parameters.Add("@DateRaised", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@Deficiency", SqlDbType.NVarChar).Value = Modal.Deficiency == null ? DBNull.Value : (object)Modal.Deficiency;
            command.Parameters.Add("@DateClosed", SqlDbType.DateTime).Value = Modal.DateClosed == null ? DBNull.Value : (object)Modal.DateClosed;
            command.Parameters.Add("@UpdatedDate", SqlDbType.DateTime).Value = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
            command.Parameters.Add("@IsClose", SqlDbType.Bit).Value = Modal.IsClose; //RDBJ 10/26/2021
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public void GIRDeficienciesFiles_Save(List<GIRDeficienciesFile> DefFiles, Guid defUniqueId)
        {
            try
            {
                if (DefFiles != null && DefFiles.Count > 0)
                {
                    bool res = DeleteRecords(AppStatic.GIRDeficienciesFiles, "DeficienciesUniqueID", Convert.ToString(defUniqueId));
                    bool isTableExist = LocalDBHelper.CheckTableExist(AppStatic.GIRDeficienciesFiles);
                    bool isTbaleCreated = true;
                    if (!isTableExist) { isTbaleCreated = LocalDBHelper.CreateTable(AppStatic.GIRDeficienciesFiles); }
                    if (isTbaleCreated)
                    {
                        ServerConnectModal dbConnModal = LocalDBHelper.ReadDBConfigJson();
                        if (dbConnModal != null && dbConnModal.IsConnection == true && dbConnModal.IsDBCreated == true)
                        {
                            string ConnectionString = Utility.GetLocalDBConnStr(dbConnModal);
                            string InsertQuery = GIRDeficienciesFiles_InsertQuery();
                            SqlConnection connection = new SqlConnection(ConnectionString);
                            connection.Open();
                            foreach (var item in DefFiles)
                            {
                                // RDBJ 03/12/2022 wrapped in if
                                if (Convert.ToBoolean(item.IsUpload))
                                {
                                    item.DeficienciesID = 0; //DefID;
                                    item.DeficienciesUniqueID = defUniqueId;
                                    SqlCommand command = new SqlCommand(InsertQuery, connection);
                                    GIRDeficienciesFiles_CMD(item, ref command);
                                    command.ExecuteScalar();
                                }
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local DB in GIRDeficienciesFiles_Save table Error : " + ex.Message.ToString());
            }
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public string GIRDeficienciesFiles_InsertQuery()
        {
            string InsertQuery = @"INSERT INTO dbo.GIRDeficienciesFiles 
                                  (DeficienciesID,FileName,StorePath,DeficienciesUniqueID)
                                  OUTPUT INSERTED.GIRDeficienciesFileID
                                  VALUES (@DeficienciesID,@FileName,@StorePath,@DeficienciesUniqueID)";
            return InsertQuery;
        }
        //End RDBJ 10/13/2021

        //RDBJ 10/13/2021
        public void GIRDeficienciesFiles_CMD(GIRDeficienciesFile Modal, ref SqlCommand command)
        {
            command.Parameters.Add("@DeficienciesID", SqlDbType.BigInt).Value = Modal.DeficienciesID;
            command.Parameters.Add("@DeficienciesUniqueID", SqlDbType.UniqueIdentifier).Value = Modal.DeficienciesUniqueID;
            command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = Modal.FileName == null ? DBNull.Value : (object)Modal.FileName;
            command.Parameters.Add("@StorePath", SqlDbType.NVarChar).Value = Modal.StorePath == null ? DBNull.Value : (object)Modal.StorePath;
        }
        //End RDBJ 10/13/2021
    }
}
