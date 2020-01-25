using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Mail
{
    /// <summary>
    /// Implementiation of the IMailService provider
    /// </summary>
    public class MailService : IMailService
    {
        /// <summary>
        /// Tries to send a test mail via SMTP
        /// </summary>
        /// <param name="data">SMTP configuration to test</param>
        /// <returns>true, is the message was send successfully</returns>
        public bool SendTestMail(SmtpConfiguration data)
        {
            // Create new client
            SmtpClient client = CreateSmtpClient(data);

            // Start a new message
            MailMessage message = CreateMessage(data);

            // Add the receiver
            message.To.Add(new MailAddress(data.ToMail));

            // Set content
            message.Subject = "SMTP Test";
            message.SubjectEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            message.Body =
@"<html>
<head><title>SMTP Test</title></head>
<body>
<h1>Success</h1>
<p>
Hi,
</p>
<p>
this is your PlasticNotifyCenter trying to send you an email.
</p>
<p>
Looks like it works =)
</p>
</body>
</html>";

            // Send the message
            client.Send(message);

            return true;
        }

        /// <summary>
        /// Asynchronosly sends a mail message via SmtpClient, but with a timeout
        /// </summary>
        /// <param name="client">SmtpClient</param>
        /// <param name="message">Message</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>true, if the message is send within the timeout</returns>
        private bool SendMailAsync(SmtpClient client, MailMessage message, TimeSpan timeout)
        {
            ManualResetEvent trigger = new ManualResetEvent(false);
            client.SendAsync(message, trigger);
            try
            {
                return trigger.WaitOne((int)timeout.TotalMilliseconds);
            }
            catch (TimeoutException)
            {
                client.SendAsyncCancel();
            }
            return false;
        }

        /// <summary>
        /// Eventhandler for the SmtpClient.SendCompleted event
        /// Used by SendMailAsync to be notified about the success
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mail_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.UserState is ManualResetEvent trigger)
            {
                trigger.Set();
            }
        }

        /// <summary>
        /// Creates a new MailMessage
        /// </summary>
        /// <param name="data">SMTP configuration</param>
        /// <returns></returns>
        public MailMessage CreateMessage(SmtpConfiguration data) =>
            CreateMessage(data.SenderMail, data.SenderAlias);

        /// <summary>
        /// Creates a new MailMessage
        /// </summary>
        /// <param name="senderMail">Sender email address</param>
        /// <param name="senderAlias">Sender name</param>
        /// <returns></returns>
        private MailMessage CreateMessage(string senderMail, string senderAlias = null) =>
            new MailMessage()
            {
                From = !string.IsNullOrWhiteSpace(senderAlias)
                    ? new MailAddress(senderMail, senderAlias, Encoding.UTF8)
                    : new MailAddress(senderMail, "PlasticNotifyCenter", Encoding.UTF8)
            };

        /// <summary>
        /// Setup SmtpClient with credentials or user default credientials (if username is empty)
        /// </summary>
        /// <param name="client">SmtpClient</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        private void SetCredentials(SmtpClient client, string username, string password)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);
            }
            else
            {
                client.UseDefaultCredentials = true;
            }
        }

        /// <summary>
        /// Creates a new SmtpClient instance
        /// </summary>
        /// <param name="data">SMTP configuration</param>
        /// <returns></returns>
        public SmtpClient CreateSmtpClient(SmtpConfiguration data)
        {
            var client = CreateSmtpClient(data.Host, data.Port, data.EnableSSL);

            // set credentials
            SetCredentials(client, data.Username, data.Password);

            return client;
        }

        /// <summary>
        /// Creates a new SmtpClient instance
        /// </summary>
        /// <param name="host">hostname</param>
        /// <param name="port">port</param>
        /// <returns></returns>
        private SmtpClient CreateSmtpClient(string host, int port = 0, bool enableSSL = false)
        {
            SmtpClient client = port != 0
                ? new SmtpClient(host, port) { EnableSsl = enableSSL }
                : new SmtpClient(host) { EnableSsl = enableSSL };
            client.SendCompleted += new SendCompletedEventHandler(Mail_SendCompleted);

            client.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

            return client;
        }
    }
}