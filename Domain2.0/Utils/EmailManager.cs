using System;
using System.Collections.Generic;
using System.Web;

using System.Text;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Text.RegularExpressions;

namespace BitPlate.Domain.Utils
{
    //class verantwoordelijk voor zenden van emails
    public class EmailManager
    {
        public void SendEmail(string mailTo, string subject, string email)
        {
            email = email.Replace("<br>", "\r\n");
            SmtpClient smtp = new SmtpClient();
            MailMessage emailMsg = new MailMessage("info@cms.nl", mailTo, subject, email);
            emailMsg.IsBodyHtml = true;
            smtp.Send(emailMsg);

            //tijdelijk loggen van email ipv verzenden omdat er geen mailserver is
            
            //email = email.Replace("<br>", "\r\n");
            //string path = ConfigurationManager.AppSettings["PasswordEmailLogPath"];
            //string filename = "Password Email "+ DateTime.Now.ToString("yyyyMMdd HHmmss") + ".txt";           
            //TextWriter writer = File.CreateText(path + filename);
            //writer.Write(email);
            //writer.Close();
        }
        public static bool SendMail(string mailFrom, string mailTo, string subject, string email, bool isHtml)
        {
            return SendMail(mailFrom, mailTo, subject, email, isHtml, "");
        }
        public static bool SendMail(string mailFrom, string mailTo, string subject, string email, bool isHtml, string fileName)
        {
            return SendMail(mailFrom, mailTo, subject, email, isHtml, new string[] { fileName });
        }
        public static bool SendMail(string mailFrom, string mailTo, string subject, string email, bool isHtml, string[] fileNames, string mailBcc = null)
        {
            try
            {
                if (mailFrom == null || mailFrom == "")
                {
                    mailFrom = getFromAddress();
                }
                if (mailFrom == null || mailFrom == "")
                {
                //    mailFrom = ApplicationSetting.GetSetting(ApplicationSettingsEnum.PasswordEmailFrom);
                }
                if (mailFrom == null || mailFrom == "")
                {
                    throw new Exception("From Address is Required");
                }

                SmtpClient smtp = new SmtpClient();
                MailMessage emailMsg = null;
                if (mailTo.Contains(";"))
                {
                    string[] emailAddresses = mailTo.Split(new char[] {';'});
                    emailMsg = new MailMessage(mailFrom, emailAddresses[0], subject, email);
                    for (int index = 1; index < emailAddresses.Length; index++)
                    {
                        if (emailAddresses[index].Trim() != "")
                        {
                            emailMsg.To.Add(new MailAddress(emailAddresses[index]));
                        }
                    }
                }
                else
                {
                    emailMsg = new MailMessage(mailFrom, mailTo, subject, email);

                }

                /*  bcc   */
                if (mailBcc != null && mailBcc.Contains(";"))
                {
                    string[] emailAddresses = mailBcc.Split(new char[] { ';' });
                    for (int index = 1; index < emailAddresses.Length; index++)
                    {
                        if (emailAddresses[index].Trim() != "")
                        {
                            emailMsg.Bcc.Add(new MailAddress(emailAddresses[index]));
                        }
                    }
                }
                else if (mailBcc != null)
                {
                    emailMsg.Bcc.Add(new MailAddress(mailBcc));

                } 
                /*  end   */

                emailMsg.IsBodyHtml = isHtml;
                foreach (string fileName in fileNames)
                {
                    if (fileName != "")
                    {
                        if (File.Exists(fileName))
                        {
                            emailMsg.Attachments.Add(new Attachment(fileName));
                        }
                    }
                }
                smtp.Send(emailMsg);

                
            }
            catch (Exception Exception)
            {
                logEmail(mailTo, subject, email);
                return false;
            }
            return true;
        }

        private static void logEmail(string to, string subject, string emailMsg)
        {
            try
            {
                //tijdelijk loggen van email ipv verzenden omdat er geen mailserver is
                string path = ConfigurationManager.AppSettings["PasswordEmailLogPath"];
                if (path == null)
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\EmailLogging\\NotSend";
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = "Email " + subject + " " + to + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".htm";
                TextWriter writer = File.CreateText(path + "\\" + filename);
                writer.Write(emailMsg);
                writer.Close();
            }
            catch (Exception ex)
            {
            }
        }

        private static string getFromAddress()
        {
            string from = "";
            CmsSite currentSite = (CmsSite)HttpContext.Current.Session["CurrentSite"];
            string webconfigPath = null;
            if (currentSite != null)
            {
                 webconfigPath = "/bitSites/" + currentSite.Name + "/wwwroot/";
            }
            //Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
            Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(webconfigPath);
            
            MailSettingsSectionGroup mailSettings = configurationFile.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings != null)
            {

                from = mailSettings.Smtp.From;
                
            }
            //string test = configurationFile.ConnectionStrings[0].ConnectionStrings.ToString();
            return from;
        }

        public static bool isValidEmailAddress(string emailaddress)
        {
            return Regex.IsMatch(emailaddress, @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$", RegexOptions.IgnoreCase);
        }
    }
}
