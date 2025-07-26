using DBModificationService.Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DBModificationService.Helpers
{
    public class TableHelper
    {
        public void GetLatestData()
        {
            try
            {
                string connetionString = Utility.GetDBConnStr();
                LogHelper.writelog(connetionString);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("Connection Done");
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter("SELECT * FROM TriggerLogs WHERE IsEmailSent = 0", conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            LogHelper.writelog(dt.Rows.Count + " Records Found ");
                            List<LatestDataModal> dataList = dt.ToListof<LatestDataModal>();
                            foreach (LatestDataModal item in dataList)
                            {
                                if (item.TableName == "SM_DEFECT_REMARKS")
                                {
                                    SM_DEFECT_REMARKS(item);
                                }
                                if (item.TableName == "SM_DEFECT_MAINTENANCE")
                                {
                                    //SM_DEFECT_MAINTENANCE(item);
                                }
                                if (item.TableName == "")
                                {

                                }
                                UpdateStatus(item.ID);
                            }
                        }
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        public void SM_DEFECT_REMARKS(LatestDataModal item)
        {
            try
            {
                string connetionString = Utility.GetDBConnStr();
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("SM_DEFECT_REMARKS");
                        conn.Open();
                        DataTable dt = new DataTable();
                        string Query = @"SELECT [SM_DEFECT_MAINTENANCE].[DEFECTNO], [SM_DEFECT_MAINTENANCE].DEFECTTITLE, 
                                        [SM_DEFECT_MAINTENANCE].CREATED_DATE, SM_DEFECT_REMARKS.REMARK, 
                                        SM_DEFECT_REMARKS.UPDATE_DATE as REMARK_UPDATE_DATE, SM_SHIPS.SHIP_DESCR, SM_USERS.USERNAME 
                                        FROM [CASHIP_WebGUI].[dbo].[SM_DEFECT_MAINTENANCE] 
                                        inner join SM_DEFECT_REMARKS on SM_DEFECT_MAINTENANCE.DEFECTID = SM_DEFECT_REMARKS.DEFECTID 
                                        inner join SM_SHIPS on SM_DEFECT_MAINTENANCE.SITEID = SM_SHIPS.SHIPID 
                                        inner join SM_USERS on SM_DEFECT_REMARKS.UPDATE_BY = SM_USERS.USERID 
                                        where REMID = " + item.RecordID;
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.Fill(dt);
                        List<SM_DEFECT_REMARKS_MODAL> dataList = dt.ToListof<SM_DEFECT_REMARKS_MODAL>();
                        if (dataList != null && dataList.Count > 0)
                        {
                            SM_DEFECT_REMARKS_MODAL first = dataList.FirstOrDefault();
                            SM_DEFECT_REMARKS_EMAIL_MODAL emailModal = new SM_DEFECT_REMARKS_EMAIL_MODAL();
                            emailModal.DEFECTNO = first.DEFECTNO;
                            emailModal.DEFECTTITLE = first.DEFECTTITLE;
                            emailModal.REMARK = first.REMARK;
                            emailModal.UPDATE_DATE = first.REMARK_UPDATE_DATE;
                            emailModal.SHIP_DESCR = first.SHIP_DESCR;
                            emailModal.USERNAME = first.USERNAME;
                            try
                            {
                                string connStrIMOSDB = Utility.GetIMOSDBConnStr();
                                using (var connIMOSDB = new SqlConnection(connStrIMOSDB))
                                {
                                    if (connIMOSDB.IsAvailable())
                                    {
                                        LogHelper.writelog("IMOS Connection Done");
                                        string QueryIMOS = @"SELECT [Email],[CE email] as CEemail,[FleetId] FROM [IMOS].[dbo].[CSShips] where LOWER([Name]) = '" + emailModal.SHIP_DESCR.ToLower() + "'";
                                        DataTable dtIMOS = new DataTable();
                                        SqlDataAdapter sqlAdpIMOS = new SqlDataAdapter(QueryIMOS, connIMOSDB);
                                        sqlAdpIMOS.Fill(dtIMOS);
                                        if (dtIMOS != null && dtIMOS.Rows.Count > 0)
                                        {
                                            LogHelper.writelog(dtIMOS.Rows.Count.ToString());
                                            emailModal.EmailAddress = Utility.ToSTRING(dtIMOS.Rows[0]["Email"]);
                                            emailModal.CCEmailAddress = Utility.ToSTRING(dtIMOS.Rows[0]["CEemail"]);
                                            int FleetId = Utility.ToINT(dtIMOS.Rows[0]["FleetId"]);
                                            emailModal.CCEmail_Manager = GetFleetManagerEmail(FleetId);
                                            connIMOSDB.Close();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.writelog("IMOSDB Error : " + ex.Message);
                            }

                            LogHelper.writelog("DEFECTNO : " + emailModal.DEFECTNO);
                            LogHelper.writelog("emailAddress : " + emailModal.EmailAddress);
                            LogHelper.writelog("CEemailAddress : " + emailModal.CCEmailAddress);
                            LogHelper.writelog("CCEmail_Manager : " + emailModal.CCEmail_Manager);
                            Send_REMARKS_Email(emailModal);
                            LogHelper.writelog(LogHelper.GetEndLine());
                        }
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
        public string GetFleetManagerEmail(int FleetID)
        {
            string Email = string.Empty;
            try
            {
                string connStrIMOSDB = Utility.GetIMOSDBConnStr();
                using (var connIMOSDB = new SqlConnection(connStrIMOSDB))
                {
                    if (connIMOSDB.IsAvailable())
                    {
                        LogHelper.writelog("IMOS Connection For Fleet Done");
                        string QueryIMOS = @"SELECT [Email] FROM [IMOS].[dbo].[Fleets] where [FleetId] = " + FleetID;
                        DataTable dtIMOS = new DataTable();
                        SqlDataAdapter sqlAdpIMOS = new SqlDataAdapter(QueryIMOS, connIMOSDB);
                        sqlAdpIMOS.Fill(dtIMOS);
                        if (dtIMOS != null && dtIMOS.Rows.Count > 0)
                        {
                            LogHelper.writelog(dtIMOS.Rows.Count.ToString());
                            Email = Utility.ToSTRING(dtIMOS.Rows[0]["Email"]);
                            connIMOSDB.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("IMOSDB Error : " + ex.Message);
            }
            return Email;
        }
        public void Send_REMARKS_Email(SM_DEFECT_REMARKS_EMAIL_MODAL emailModal)
        {
            if (!string.IsNullOrEmpty(emailModal.EmailAddress))
            {
                if (string.IsNullOrEmpty(emailModal.CCEmailAddress))
                {
                    emailModal.CCEmailAddress = "ce." + emailModal.EmailAddress;
                }
                string subject = "Defect Updated";
                SMTPServerModal Modal = Utility.GetSMTPSettings();
                Modal.TOEmail = emailModal.EmailAddress;
                Modal.CCEmail = emailModal.CCEmailAddress;
                Modal.CCEmail_Manager = emailModal.CCEmail_Manager;
                string emailBody = EmailHelper.GET_SM_DEFECT_REMARKS_EMAIL_BODY(emailModal);
                EmailHelper.SendMail(subject, emailBody, Modal);
            }
        }

        //        public void SM_DEFECT_MAINTENANCE(LatestDataModal item)
        //        {
        //            try
        //            {
        //                string connetionString = Utility.GetDBConnStr();
        //                using (var conn = new SqlConnection(connetionString))
        //                {
        //                    if (conn.IsAvailable())
        //                    {
        //                        LogHelper.writelog("SM_DEFECT_MAINTENANCE");
        //                        conn.Open();
        //                        DataTable dt = new DataTable();
        //                        string Query = @"SELECT [SM_DEFECT_MAINTENANCE].*, [SM_SHIPS].SHIP_DESCR, [SM_USERS].USERNAME
        //                                          FROM [CASHIP_WebGUI].[dbo].[SM_DEFECT_MAINTENANCE] 
        //                                          inner join SM_SHIPS on SM_DEFECT_MAINTENANCE.SITEID = SM_SHIPS.SITEID 
        //                                          inner join SM_USERS on SM_DEFECT_MAINTENANCE.UPDATE_BY = SM_USERS.USERID  
        //                                          where DEFECTID = " + item.RecordID + " order by UPDATE_DATE ";
        //                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
        //                        sqlAdp.Fill(dt);
        //                        List<SM_DEFECT_MAINTENANCE_MODAL> dataList = dt.ToListof<SM_DEFECT_MAINTENANCE_MODAL>();
        //                        if (dataList != null && dataList.Count > 0)
        //                        {
        //                            SM_DEFECT_MAINTENANCE_MODAL first = dataList.FirstOrDefault();
        //                            SM_DEFECT_MAINTENANCE_Email_MODAL emailModal = new SM_DEFECT_MAINTENANCE_Email_MODAL();
        //                            emailModal.DEFECTNO = first.DEFECTNO;
        //                            emailModal.DEFECTTITLE = first.DEFECTTITLE;
        //                            emailModal.OPEN_DATE = first.OPEN_DATE;
        //                            emailModal.UPDATE_DATE = first.UPDATE_DATE;
        //                            emailModal.USERNAME = first.USERNAME;
        //                            emailModal.SHIP_DESCR = first.SHIP_DESCR;

        //                            string subject = string.Empty;
        //                            if (item.Action == "INSERTED")
        //                            {
        //                                subject = "Record Inserted IN SM_DEFECT_MAINTENANCE TABLE ";
        //                            }
        //                            if (item.Action == "UPDATED")
        //                            {
        //                                subject = "Record Updated IN SM_DEFECT_MAINTENANCE TABLE";
        //                            }
        //                            string emailBody = EmailHelper.GET_SM_DEFECT_MAINTENANCE_EMAIL_BODY(emailModal);
        //                            SMTPServerModal Modal = Utility.GetSMTPSettings();
        //                            EmailHelper.SendMail(subject, emailBody, Modal);
        //                        }
        //                        conn.Close();
        //                    }
        //                    else
        //                    {
        //                        LogHelper.writelog("SQL Connection is not available");
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                LogHelper.writelog(ex.Message);
        //            }
        //        }
        public void UpdateStatus(long ID)
        {
            try
            {
                string connetionString = Utility.GetDBConnStr();
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlCommand cmd = new SqlCommand("UPDATE TriggerLogs SET IsEmailSent = 1 WHERE ID = " + ID, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        LogHelper.writelog("Update Done For ID : " + ID);
                    }
                    else
                    {
                        LogHelper.writelog("SQL Connection is not available");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
        }
    }
}
