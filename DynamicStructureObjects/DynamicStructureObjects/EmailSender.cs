using System.Net;
using System.Net.Mail;

namespace DynamicStructureObjects
{
    public class EmailSender
    {

        private string from { get; set; }
        private string smtpUsername { get; set; }
        private string smtpPassword { get; set; }
        private string Host { get; set; }
        private int Port { get; set; }
        private Dictionary<string, string> bodiesSubjects = new Dictionary<string, string>();
        public EmailSender(string fromEmail, string smtpUsername, string smtpPassword, string host, int port)
        {
            from = fromEmail;
            this.smtpUsername = smtpUsername;
            this.smtpPassword = smtpPassword;
            this.Host = host;
            this.Port = port;
        }
        public static bool SendEmail(string fromEmail, List<string> toEmails, string subject, string body, string smtpUsername, string smtpPassword, string host, int port)//"smtp.gmail.com" //587
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);

            foreach (var toEmail in toEmails)
            {
                mailMessage.To.Add(toEmail);
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;

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

        public bool SendEmail(IEnumerable<string> toEmails, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);

            foreach (var toEmail in toEmails)
            {
                mailMessage.To.Add(toEmail);
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = Host;
            smtpClient.Port = Port;
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
