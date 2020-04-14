using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InventoryBooks.Helper
{
    public class MailHelper
    {
        public readonly string _connectionString = string.Empty;
        public static string SMTPMailServer = "smtp.gmail.com";
        public static string SMTPServerPort = "587";
        private static string SMTPAuthentication = "1";
        private static string SMTPEnableSSL = "true";
        private static string SMTPUsername = ""; // Email
        private static string SMTPPassword = ""; //password

        public MailHelper()
        {

        }
        public void SendMailNoAttachment(string From, string sendTo, string Subject, string Body, string CC,
                                                string BCC)
        {
            Task.Factory.StartNew(() => SendEMail(From, sendTo, Subject, Body, CC, BCC), TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
        }

        public void SendEMail(string From, string sendTo, string Subject, string Body, string CC, string BCC)
        {
            ArrayList AttachmentFiles;
            AttachmentFiles = null;
            SendEMail(From, sendTo, Subject, Body, AttachmentFiles, CC, BCC);
        }

        public void SendEMail(string From, string sendTo, string Subject, string Body, ArrayList AttachmentFiles,
                                     string CC, string BCC)
        {
            SendEMail(From, sendTo, Subject, Body, AttachmentFiles, CC, BCC, true);
        }
        public void SendMailMultipleAttachments(string From, string sendTo, string Subject, string Body,
                                                       ArrayList AttachmentFiles, string CC, string BCC)
        {
            Task.Factory.StartNew(() => SendEMail(From, sendTo, Subject, Body, AttachmentFiles, CC, BCC), TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
        }
        public void SendEMail(string senderName, string sendTo, string Subject, string Body, ArrayList AttachmentFiles, string CC, string BCC, bool IsHtmlFormat)
        {
            try
            {
                MailMessage myMessage = new MailMessage();
                myMessage.To.Add(sendTo);
                myMessage.From = new MailAddress(SMTPUsername);
                myMessage.Subject = Subject;
                myMessage.Body = Body;
                myMessage.IsBodyHtml = true;

                if (CC.Length != 0)
                    myMessage.CC.Add(CC);

                if (BCC.Length != 0)
                    myMessage.Bcc.Add(BCC);

                if (AttachmentFiles != null)
                {
                    foreach (string x in AttachmentFiles)
                    {
                        if (File.Exists(x)) myMessage.Attachments.Add(new Attachment(x));
                    }
                }
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;
                if (SMTPAuthentication == "1")
                {
                    if (SMTPUsername.Length > 0 && SMTPPassword.Length > 0)
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
                    }
                }
                smtp.UseDefaultCredentials = false;
                smtp.Host = SMTPMailServer.ToString();
                smtp.Port = int.Parse(SMTPServerPort);
                smtp.Send(myMessage);
                smtp.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
