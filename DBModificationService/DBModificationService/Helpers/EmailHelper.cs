using DBModificationService.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DBModificationService.Helpers
{
    public class EmailHelper
    {
        public static void SendMail(string subject, string mailbody, SMTPServerModal Modal)
        {
            string message = string.Empty;
            try
            {
                //SMTPServerModal Modal = Utility.GetSMTPSettings();
                if (Modal != null)
                {
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(Modal.SMTPFromAddress);
                    mailMessage.Subject = subject;
                    mailMessage.Body = mailbody;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(Modal.TOEmail);
                    if (!string.IsNullOrEmpty(Modal.CCEmail))
                    {
                        mailMessage.To.Add(Modal.CCEmail);
                    }
                    //if (!string.IsNullOrEmpty(Modal.CCEmail_Manager))
                    //{
                    //    mailMessage.CC.Add(Modal.CCEmail_Manager);
                    //}
                    mailMessage.CC.Add("IT@carisbrooke.co");
                    //mailMessage.CC.Add("prashantpatel.wa@gmail.com");
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = Modal.SMPTServerName;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = false;
                    smtpClient.Port = Utility.ToINT(Modal.SMTPPort);
                    if (Modal.IsAuthenticationRequired == true)
                    {
                        smtpClient.Credentials = new System.Net.NetworkCredential(Modal.SMTPUserName, Modal.SMTPPassword);
                    }
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Mail Error : " + ex.Message);
            }
        }
        public static string GET_SM_DEFECT_REMARKS_EMAIL_BODY(SM_DEFECT_REMARKS_EMAIL_MODAL emailModal)
        {
            string body = string.Empty;
            try
            {
                body = @"Ship : " + Utility.ToSTRING(emailModal.SHIP_DESCR) + "<br/>" +
                    "Defect No : " + Utility.ToSTRING(emailModal.DEFECTNO) + "<br/>" +
                    "Defect Title : " + Utility.ToSTRING(emailModal.DEFECTTITLE) + "<br/>" +
                    "Update Date : " + Utility.ToSTRING(emailModal.UPDATE_DATE) + "<br/>" +
                    "Updated By : " + Utility.ToSTRING(emailModal.USERNAME) + "<br/>" +
                    "Remark : " + Utility.ToSTRING(emailModal.REMARK) + "<br/>";
            }
            catch { }
            return body;
        }
        public static string GET_SM_DEFECT_MAINTENANCE_EMAIL_BODY(SM_DEFECT_MAINTENANCE_Email_MODAL emailModal)
        {
            string body = string.Empty;
            try
            {
                body = @"Defect No : " + emailModal.DEFECTNO + "<br/>" +
                    "Defect Title : " + emailModal.DEFECTTITLE + "<br/>" +
                    "OPEN_DATE : " + emailModal.OPEN_DATE + "<br/>" +
                    "UPDATE_DATE : " + emailModal.UPDATE_DATE + "<br/>" +
                    "Updated By : " + emailModal.USERNAME + "<br/>" +
                    "Ship : " + emailModal.SHIP_DESCR + "<br/>";
            }
            catch { }
            return body;
        }
    }
}
