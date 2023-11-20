﻿using System.Net;
using System.Net.Mail;

namespace DynamicStructureObjects
{
    public class EmailSender
    {
        private string from { get; set; }
        private string smtpUsername { get; set; }
        private string smtpPassword { get; set; }
        private string displayName { get; set; }
        private string Host { get; set; }
        private int Port { get; set; }
        public const string defaultHost = "smtp.gmail.com";
        public const int defaultPort = 587;
        private bool IsHtml { get; set; }
        public EmailSender(string fromEmail, string smtpUsername, string smtpPassword, string displayName = null)
            : this (fromEmail, smtpUsername, smtpPassword, defaultHost, defaultPort, displayName)
        { }
        public EmailSender(string fromEmail, string smtpUsername, string smtpPassword, string host, int port, string displayName = null, bool isHtml = true)
        {
            from = fromEmail;
            this.smtpUsername = smtpUsername;
            this.smtpPassword = smtpPassword;
            this.Host = host;
            this.Port = port;
            this.displayName = null;
            this.IsHtml = isHtml;
        }
        public static bool SendEmail(string fromEmail, IEnumerable<string> toEmails, string subject, string body, string smtpUsername, string smtpPassword, List<Attachment> attachments, string host = defaultHost, int port = defaultPort, string displayName = null, bool isHtml = true)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = displayName is null ? new MailAddress(fromEmail) : new MailAddress(fromEmail, displayName);

            foreach (var toEmail in toEmails)
                mailMessage.To.Add(toEmail);

            mailMessage.Subject = subject;
            if (isHtml)
            {
                Console.Write("allo, isHTML lol",isHtml);
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

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendEmail(IEnumerable<string> toEmails, string subject, string body, List<Attachment> attachments, bool isHtml = true)
        {
            return SendEmail(from, toEmails, subject, body, smtpUsername, smtpPassword, attachments, Host, Port, displayName, isHtml);
        }
        public bool SendEmail(string toEmail, string subject, string body, List<Attachment> attachments, bool isHtml = true)
        {
            return SendEmail(new string[] { toEmail }, subject, body, attachments, isHtml);
        }
        public bool SendEmail(string subject, string body, List<Attachment> attachments, bool isHtml = true, params string[] toEmails)
        {
            return SendEmail(toEmails, subject, body, attachments, isHtml);
        }
    }
}
