using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Backend.Mail
{
    public class Mailer
    {
        public static void SendTest()
        {
            // trusting my own self sign certificate on my smtp server.
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (
                object s,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors
            ) {
                return true;
            };

            var client = new SmtpClient("smtp1.kristianekman.com", 2525);
            string to = "kristian.ekman.swe@gmail.com";
            string from = "kristian@kristianekman.com";
            string subject = "Testar SMTP client.";
            string body = @"Test Tjurgränd 47.";
            MailMessage message = new MailMessage(from, to, subject, body);
            client.Credentials = new NetworkCredential("kristian", "***");
            client.EnableSsl = true;

            client.Send(message);
        }
    }
}
