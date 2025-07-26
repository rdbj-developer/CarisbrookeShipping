using Mvc.Mailer;
using ShipApplication.BLL.Helpers;
using ShipApplication.BLL.Modals;
using ShipApplication.Mailers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace ShipApplication.Controllers
{
    public class MailController : Controller
    {
        // GET: Mail

        private IShipMailler _shipMailer = new ShipMailer();
        public IShipMailler ShipMailer
        {
            get { return _shipMailer; }
            set { _shipMailer = value; }
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SendArrivalReportMail(ArrivalReportModal Modal)
        {
            try
            {
                SettingsHelper _Shelper = new SettingsHelper();

                SMTPServerModal SMTP = _Shelper.GetSMTPSettingsJson();
                if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
                {
                    Modal.CCEmail = SMTP.CCEmail;
                }

                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    APIHelper _helper = new APIHelper();
                    APIResponse resp = _helper.SubmitArrivalReport(Modal);
                    if (resp != null && resp.result == AppStatic.SUCCESS)
                    {
                        Modal.IsSynced = true;
                    }
                    else
                        Modal.IsSynced = false;
                }
                ReportsHelper _rHelper = new ReportsHelper();
                _rHelper.SaveArrivalReportDataInLocalDB(Modal);

                //Modal.ToEmail = "prashantpatel.wa@gmail.com";
                MvcMailMessage mailer = ShipMailer.ArrivalReport(Modal);
                string subject = mailer.Subject;
                string body = mailer.Body;
                SendMail(subject, body, Modal.ToEmail);
                TempData["Result"] = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }
            return RedirectToAction("ArrivalReport", "Forms");
        }
        public ActionResult SendDepartureReportMail(DepartureReportModal Modal)
        {
            try
            {
                SettingsHelper _Shelper = new SettingsHelper();
                SMTPServerModal SMTP = _Shelper.GetSMTPSettingsJson();
                if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
                {
                    Modal.CCEmail = SMTP.CCEmail;
                }

                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    APIHelper _helper = new APIHelper();
                    APIResponse resp = _helper.SubmitDepartureReport(Modal);
                    if (resp != null && resp.result == AppStatic.SUCCESS)
                    {
                        Modal.IsSynced = true;
                    }
                    else
                        Modal.IsSynced = false;
                }

                ReportsHelper _rHelper = new ReportsHelper();
                _rHelper.SaveDepartureReportDataInLocalDB(Modal);

                //Modal.ToEmail = "prashantpatel.wa@gmail.com";
                MvcMailMessage mailer = ShipMailer.DepartureReport(Modal);
                string subject = mailer.Subject;
                string body = mailer.Body;
                SendMail(subject, body, Modal.ToEmail);
                TempData["Result"] = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }
            return RedirectToAction("DepartureReport", "Forms");
        }
        public ActionResult SendDailyCargoReportMail(DailyCargoReportModal Modal)
        {
            try
            {
                SettingsHelper _Shelper = new SettingsHelper();
                SMTPServerModal SMTP = _Shelper.GetSMTPSettingsJson();
                if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
                {
                    Modal.CCEmail = SMTP.CCEmail;
                }

                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    APIHelper _helper = new APIHelper();
                    APIResponse resp = _helper.SubmitDailyCargoReport(Modal);
                    if (resp != null && resp.result == AppStatic.SUCCESS)
                    {
                        Modal.IsSynced = true;
                    }
                    else
                        Modal.IsSynced = false;
                }
                ReportsHelper _rHelper = new ReportsHelper();
                _rHelper.SaveDailyCargoReportDataInLocalDB(Modal);

                //Modal.ToEmail = "prashantpatel.wa@gmail.com";
                MvcMailMessage mailer = ShipMailer.DailyCargoReport(Modal);
                string subject = mailer.Subject;
                string body = mailer.Body;
                SendMail(subject, body, Modal.ToEmail);

                TempData["Result"] = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }

            return RedirectToAction("DailyCargoReport", "Forms");
        }
        public ActionResult SendDailyPositionReportMail(DailyPositionReportModal Modal)
        {
            try
            {
                SettingsHelper _Shelper = new SettingsHelper();
                SMTPServerModal SMTP = _Shelper.GetSMTPSettingsJson();
                if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
                {
                    Modal.CCEmail = SMTP.CCEmail;
                }
                Modal.Latitude = Modal.Latitudedd + "°" + Modal.Latitudemm + Modal.DirectionNS;
                Modal.Longitude = Modal.Longitudeddd + "°" + Modal.Longitudemm + Modal.DirectionEW;

                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    APIHelper _helper = new APIHelper();
                    APIResponse resp = _helper.SubmitDailyPositionReport(Modal);
                    if (resp != null && resp.result == AppStatic.SUCCESS)
                    {
                        Modal.IsSynced = true;
                    }
                    else
                        Modal.IsSynced = false;
                }

                ReportsHelper _rHelper = new ReportsHelper();
                _rHelper.SaveDailyPositionReportDataInLocalDB(Modal);

                MvcMailMessage mailer = ShipMailer.DailyPositionReport(Modal);
                string subject = mailer.Subject;
                string body = mailer.Body;
                SendMail(subject, body, Modal.ToEmail);

                TempData["Result"] = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }

            return RedirectToAction("DailyPositionReport", "Forms");
        }
        public ActionResult SendGIReportMail(GeneralInspectionReport Modal)
        {
            try
            {
                string strShipCode = Modal.ShipName;    // JSL 02/24/2023
                SettingsHelper _Shelper = new SettingsHelper();
                List<string> ccemail = new List<string>();

                SMTPServerModal SMTP = _Shelper.GetSMTPSettingsJson();
                if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
                {
                    ccemail = SMTP.CCEmail;
                }
                
                Modal = (GeneralInspectionReport)TempData["data"];
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    ReportsHelper _rHelper = new ReportsHelper();

                    string ToEmail = "daniel.lewandowski@carisbrooke.co";
                    string subject = "GIR -" + Modal.ShipName + " - " + Modal.Inspector + " - " + Modal.Date;
                    string body = "<h4>New General Inspection Report has been received.</h4>" +
                                  "<br>Ship: " + Convert.ToString(TempData["Name"]) +
                                  "<br>Inspector:" + Modal.Inspector +
                                  "<br>Port:" + Modal.Port +
                                  "<br>Date:" + Utility.DateToString(Modal.Date) +
                                  "<br>No. Of Deficiencies:" + Modal.GIRDeficiencies.Count + 
                                  "<br>Outstanding:" + Modal.GIRDeficiencies.Where(x => x.IsClose == false).Count();

                    //RDBJ 11/10/2021
                    List<string> technicalAndISMGroupEmailsList = _Shelper.GetEmailFromUserProfileTableWhereTechnicalAndISMGroup(strShipCode);
                    if (technicalAndISMGroupEmailsList != null)
                    {
                        foreach (var newEmails in technicalAndISMGroupEmailsList)
                        {
                            if (!ccemail.Contains(newEmails))
                            {
                                ccemail.Add(newEmails);
                            }
                        }
                    }
                    //End RDBJ 11/10/2021

                    SendMail(
                        subject, body, ToEmail,
                        ccemail //RDBJ 11/10/2021
                        );

                    TempData["Result"] = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }

            return RedirectToAction("Index", "Deficiencies");
        }
        public ActionResult SendSIReportMail(SIRModal Modal)
        {
            try
            {
                string strShipCode = Modal.SuperintendedInspectionReport.ShipName;  // JSL 02/24/2023
                SettingsHelper _Shelper = new SettingsHelper();
                List<string> ccemail = new List<string>();

                SMTPServerModal SMTP = _Shelper.GetSMTPSettingsJson();
                if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
                {
                    ccemail = SMTP.CCEmail;
                }

                Modal = (SIRModal)TempData["data"];
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    ReportsHelper _rHelper = new ReportsHelper();

                    string ToEmail = "daniel.lewandowski@carisbrooke.co";
                    string subject = "SIR -" + Modal.SuperintendedInspectionReport.ShipName + " - " + Modal.SuperintendedInspectionReport.Superintended + " - " + Modal.SuperintendedInspectionReport.Date;
                    string body = "<h4>New Superintendent Inspection Report has been received.</h4>" +
                                  "<br>Ship: " + Convert.ToString(TempData["Name"]) +
                                  "<br>Superintendent:" + Modal.SuperintendedInspectionReport.Superintended +
                                  "<br>Port:" + Modal.SuperintendedInspectionReport.Port +
                                  "<br>Date:" + Utility.DateToString(Modal.SuperintendedInspectionReport.Date);

                    //RDBJ 11/10/2021
                    List<string> technicalAndISMGroupEmailsList = _Shelper.GetEmailFromUserProfileTableWhereTechnicalAndISMGroup(strShipCode);
                    if (technicalAndISMGroupEmailsList != null)
                    {
                        foreach (var newEmails in technicalAndISMGroupEmailsList)
                        {
                            if (!ccemail.Contains(newEmails))
                            {
                                ccemail.Add(newEmails);
                            }
                        }
                    }
                    //End RDBJ 11/10/2021

                    SendMail(
                        subject, body, ToEmail,
                        ccemail //RDBJ 11/10/2021
                        );

                    TempData["Result"] = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }

            return RedirectToAction("Index", "Deficiencies");
        }

        //RDBJ 11/08/2021
        public ActionResult SendIAFReportMail(IAF Modal)
        {
            try
            {
                string strShipCode = Modal.InternalAuditForm.ShipName;    // JSL 02/24/2023
                SettingsHelper _Shelper = new SettingsHelper();
                List<string> ccemail = new List<string>();

                SMTPServerModal SMTP = _Shelper.GetSMTPSettingsJson();
                if (SMTP != null && SMTP.CCEmail != null && SMTP.CCEmail.Count > 0)
                {
                    ccemail = SMTP.CCEmail;
                }
                
                Modal = (IAF)TempData["data"];
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    ReportsHelper _rHelper = new ReportsHelper();

                    string ToEmail = "daniel.lewandowski@carisbrooke.co";
                    string subject = "IAF -" + Modal.InternalAuditForm.ShipName + " - " + Modal.InternalAuditForm.Auditor + " - " + Modal.InternalAuditForm.Date;
                    string body = "<h4>New Internal Audit Report has been received.</h4>" +
                                  "<br>Ship: " + Convert.ToString(TempData["Name"]) +
                                  "<br>Auditor:" + Modal.InternalAuditForm.Auditor +
                                  "<br>Port:" + Modal.InternalAuditForm.Location +
                                  "<br>Date:" + Utility.DateToString(Modal.InternalAuditForm.Date);

                    //RDBJ 11/10/2021
                    List<string> technicalAndISMGroupEmailsList = _Shelper.GetEmailFromUserProfileTableWhereTechnicalAndISMGroup(strShipCode);
                    if (technicalAndISMGroupEmailsList != null)
                    {
                        foreach (var newEmails in technicalAndISMGroupEmailsList)
                        {
                            if (!ccemail.Contains(newEmails))
                            {
                                ccemail.Add(newEmails);
                            }
                        }
                    }
                    //End RDBJ 11/10/2021

                    SendMail(
                        subject, body, ToEmail,
                        ccemail //RDBJ 11/10/2021
                        );

                    TempData["Result"] = AppStatic.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }

            return RedirectToAction("Index", "Deficiencies");
        }
        //End RDBJ 11/08/2021

        public ActionResult SendFeedbackMail(FeedbackFormModal Modal)
        {
            try
            {
                bool isInternetAvailable = Utility.CheckInternet();
                if (isInternetAvailable)
                {
                    try
                    {
                        string subject = "Feedback - " + Modal.ShipName + " - " + Modal.Title;
                        string body = Modal.Details;
                        
                        SendMail(
                            subject, body, "it@carisbrooke.co", 
                            null, //RDBJ 11/10/2021 
                            Modal.AttachmentPath, Modal.AttachmentFileName);

                        TempData["Result"] = AppStatic.SUCCESS;
                        Modal.IsMailSent = true;
                    }
                    catch (Exception)
                    {
                        Modal.IsMailSent = false;
                    }
                    APIHelper _helper = new APIHelper();
                    APIResponse resp = _helper.SubmitFeedbackForm(Modal);
                    if (resp != null && resp.result == AppStatic.SUCCESS)
                    {
                        Modal.IsSynced = true;
                    }
                    else
                        Modal.IsSynced = false;
                }
                FeedbackFormHelper _rHelper = new FeedbackFormHelper();
                _rHelper.SaveFeedbackDataInLocalDB(Modal);
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                TempData["Result"] = AppStatic.ERROR;
            }
            return RedirectToAction("FeedbackForm", "Forms");
        }
        public void SendMail(
            string subject, string mailbody, string sendto
            , List<string> TechnicalAndISMUsersEmailForCC = null //RDBJ 11/10/2021
            , string base64AttachmentString = "", string attachmentFileName = ""
            )
        {
            string message = string.Empty;
            try
            {
                SettingsHelper _helper = new SettingsHelper();
                SMTPServerModal Modal = _helper.GetSMTPSettingsJson();
                if (Modal != null)
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(Modal.SMTPFromAddress);
                    mailMessage.Subject = subject;
                    mailMessage.Body = mailbody;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(sendto);

                    //RDBJ 11/10/2021 wrapped in if
                    if (TechnicalAndISMUsersEmailForCC == null)
                    {
                        if (Modal.CCEmail != null && Modal.CCEmail.Count > 0)
                        {
                            foreach (string ccItem in Modal.CCEmail)
                            {
                                mailMessage.CC.Add(ccItem);
                            }
                        }
                    }
                    //RDBJ 11/10/2021 added else for the send email notification for Technical and ISM users when GI-SI-IAF forms submitted
                    else
                    {
                        foreach (string ccItem in TechnicalAndISMUsersEmailForCC)
                        {
                            mailMessage.CC.Add(ccItem);
                        }
                    }

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = Modal.SMPTServerName;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = false;
                    smtpClient.Port = Utility.ToInteger(Modal.SMTPPort);
                    if (Modal.IsAuthenticationRequired == true)
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(Modal.SMTPUserName, Modal.SMTPPassword);
                    }
                    if (!string.IsNullOrWhiteSpace(base64AttachmentString))
                    {
                        var fileParts = base64AttachmentString.Split(',').ToList<string>();
                        //Exclude the header from base64 by taking second element in List.
                        byte[] base64Bytes = Convert.FromBase64String(fileParts[1]);
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(base64Bytes), attachmentFileName));
                    }
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Mail Error : " + ex.Message);
            }
        }
    }
}