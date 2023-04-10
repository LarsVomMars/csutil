using System.Net.Mail;

namespace csutil
{
    public class Mailer
    {
        private readonly SmtpClient _client;
        private readonly string _sender;
        private readonly string _recipients;

        public Mailer(MailCredentials credentials)
        {
            _client = new SmtpClient
            {
                Host = credentials.Host,
                Port = credentials.Port,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            _sender = credentials.Sender;
            _recipients = string.Join(", ", credentials.Recipients);
        }

        public void Send(string subject, string body, MailPriority priority = MailPriority.Normal)
        {
            _client.Send(new MailMessage(_sender, _recipients)
            {
                Subject = subject,
                Body = body,
                Priority = priority,
            });
        }
    }


    public class MailCredentials
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string[] Recipients { get; set; }
    }
}