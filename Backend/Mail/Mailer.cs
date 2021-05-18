using Backend.Db;
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
            Send("kristian.ekman.swe@gmail.com", "Testar SMTP client.", @"<div><img src='https://backgammon.azurewebsites.net/assets/images/facebook.png'></div><p>Test Tjurgränd 47.</p><a href='https://backgammon.azurewebsites.com'>Testar en html länk.<a/>");
        }

        internal static void Send(string email, string subject, string text)
        {
            // I trust my own self signed certificate on my smtp server.
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (
                object s,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors
            )
            {
                return true;
            };

            using (var client = new SmtpClient("smtp1.kristianekman.com", 2525))
            {
                string from = "backgammon@kristianekman.com";
                var message = new MailMessage(from, email, subject, text)
                {
                    IsBodyHtml = true
                };                

                var pw = Secrets.GetPw();
                client.Credentials = new NetworkCredential("backgammon", pw);
                client.EnableSsl = true;

                client.Send(message);
            }
        }
    }
}
