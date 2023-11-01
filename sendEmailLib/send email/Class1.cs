using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace sendEmail
{
    public class EmailSender
    {

        private string _from;
        private string _smtpUsername;
        private string _smtpPassword;
        private int _host = 587;
        private Dictionary<string, string> bodiesSubjects = new Dictionary<string, string>();

        public string from
        {
            get { return _from; }
            set { _from = value; }
        }

        public string smtpUsername
        {
            get { return _smtpUsername; }
            set { _smtpUsername = value; }
        }

        public string smtpPassword
        {
            get { return _smtpPassword; }
            set { _smtpPassword = value; }
        }
        public int host
        {
            get { return _host; }
            set { _host = value; }
        }

        public EmailSender(string fromEmail, string smtpUsername, string smtpPassword)
        {
            _from = fromEmail;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
        }
        public static void SendEmail(string fromEmail, List<string> toEmails, string subject, string body, string smtpUsername, string smtpPassword, bool isHtml = false)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);

            foreach (var toEmail in toEmails)
            {
                mailMessage.To.Add(toEmail);
            }

            mailMessage.Subject = subject;

            if (isHtml)
            {
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;
            }
            else
            {
                mailMessage.Body = body;
            }
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void SendEmail(IEnumerable<string> toEmails, string subject, string body, List<Attachment> attachments = null, bool isHtml = false)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_from);

                foreach (var toEmail in toEmails)
                {
                    mailMessage.To.Add(toEmail);
                }

                mailMessage.Subject = subject;
                if (isHtml)
                {
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = body;
                }
                else
                {
                    mailMessage.Body = body;
                }

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;

                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("Email Sent Successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }
        public void SendEmail(string toEmail, string subject, string body)
        {
            SendEmail(new string[] { toEmail }, subject, body);
        }
        public void SendEmail(string subject, string body, params string[] toEmails)
        {
            SendEmail(toEmails, subject, body);
        }
    }
}
