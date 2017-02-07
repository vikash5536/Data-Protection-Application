using DataProtectionApplication.CommonLibrary.Model;
using DataProtectionApplication.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace DataProtectionApplication.CommonLibrary.SendEmail
{
    /// <summary>
    /// This class is used to manage email features.
    /// </summary>
    public class SendEmail
    {
        public static Logger logger = new Logger(typeof(SendEmail));

        /// <summary>
        /// This method is used to load action arguments.
        /// </summary>
        /// <param name="args"></param>
        public EmailConfiguration LoadEmailConfiguration()
        {
            try
            {
                //// Assigning arguments to Email configuration object
                EmailConfiguration _emailConfig = new EmailConfiguration();
                XmlSerializer serializer = new XmlSerializer(typeof(EmailConfiguration));
                logger.LogInfo(AppDomain.CurrentDomain.BaseDirectory.ToString());
                string fileName = "";

#if DEBUG
                fileName = @"D:\Projects\WPFProject\DataEncyptionApp\28Jan\SpectraLogicBCPA2\SpectraLogicBCPA\bin\Debug\" + "EmailConfiguration.xml";
#else
                fileName = AppDomain.CurrentDomain.BaseDirectory + "..\\" + "EmailConfiguration.xml";
#endif
                if (File.Exists(fileName))
                {
                    StreamReader reader = new StreamReader(fileName);
                    return (EmailConfiguration)serializer.Deserialize(reader);
                }
                else
                {
                    logger.LogInfo(string.Format("Email configuration has not been set."));
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogInfo(string.Format("Exception in LoadEmailArguments , Message : {0}", ex.Message));
                return null;
            }
        }
        /// <summary>
        /// This method is used to send Email notification using email configuration.
        /// </summary>
        /// <param name="emailconfig"></param>
        /// <returns></returns>
        public bool SendMail(EmailConfiguration emailconfig)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailconfig.EmailFrom);
                    mail.To.Add(emailconfig.EmailTo);
                    mail.Subject = emailconfig.Subject;
                    mail.Body = emailconfig.Body;
                    mail.IsBodyHtml = true;
                    // Can set to false, if you are sending pure text.

                    using (SmtpClient smtp = new SmtpClient(emailconfig.SmtpAddress, emailconfig.PortNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailconfig.EmailFrom, emailconfig.Password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                logger.LogError(string.Format("Email sent successfully."));
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Unable to sent an e-mail , Exception in SendMail method, Message: {0}", ex.Message));
            }
            return false;
        }

        /// <summary>
        /// This method is used to update subject and body in email config object.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <param name="emailConfig"></param>
        /// <returns></returns>
        public static EmailConfiguration UpdateEmailConfig(string[] args, HRESULT result, EmailConfiguration emailConfig)
        {

            StringBuilder body = new StringBuilder();
            emailConfig.Subject = string.Format("Scheduled Task : {0} executed with result code : {1}", args[0], result);
            body = body.AppendLine("#====================================================#<br />");
            body = body.AppendLine(string.Format("| Task Name : {0} | <br />", args[0]));
            body = body.AppendLine(string.Format("| Action Type: {0} | <br />", (ActionTypeEnum)Enum.Parse(typeof(ActionTypeEnum), args[1])));
            body = body.AppendLine(string.Format("| Backup Type : {0} | <br />", (BackupRestoreTypeEnum)Enum.Parse(typeof(BackupRestoreTypeEnum), args[2])));
            body = body.AppendLine(string.Format("| Backup Location: {0} | <br />", (BackupRestoreLoactionEnum)Enum.Parse(typeof(BackupRestoreLoactionEnum), args[3])));
            body = body.AppendLine(string.Format("| Source Location : {0} | <br />", args[5]));
            body = body.AppendLine(string.Format("| Source Server Details: {0} | <br />", args[4]));
            body = body.AppendLine(string.Format("| Destination Details: {0} | <br />", args[6]));
            body = body.AppendLine(string.Format("| Destination Bucket Name: {0} | <br />", args[7]));
            body = body.AppendLine("#====================================================#<br />");
            emailConfig.Body = body.ToString();
            return emailConfig;
        }
    }
}
