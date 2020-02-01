using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Mail
{
    /// <summary>
    /// Implementiation of the IMailService provider
    /// </summary>
    public class MailService : IMailService
    {
        #region Dependencies

        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        #endregion

        /// <summary>
        /// Tries to send a test mail via SMTP
        /// </summary>
        /// <param name="data">SMTP configuration to test</param>
        /// <returns>true, is the message was send successfully</returns>
        public bool SendTestMail(SmtpConfiguration data)
        {
            _logger.LogDebug("Testing SMTP configuration: {0}", data);

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
            message.Body = DefaultTemplate.Html
                                .Replace("%PNC_RULESURL%", "")
                                .Replace("%PNC_TITLE%", "Great Success")
                                .Replace("%PNC_BODY%", "<p>Hi there,</p><p>this is your Plastic-Notify-Center =)</p><p>Looks like the SMTP configuration is working. YEAH!</p>")
                                .Replace("%PNC_TAGS%", "Awesome");

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
        public MailMessage CreateMessage(SmtpConfiguration data) =>
            CreateMessage(data.SenderMail, data.SenderAlias);

        /// <summary>
        /// Creates a new MailMessage
        /// </summary>
        /// <param name="senderMail">Sender email address</param>
        /// <param name="senderAlias">Sender name</param>
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
        private SmtpClient CreateSmtpClient(string host, int port = 0, bool enableSSL = false)
        {
            SmtpClient client = port != 0
                ? new SmtpClient(host, port)
                : new SmtpClient(host);

            client.EnableSsl = enableSSL;
            client.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.SendCompleted += new SendCompletedEventHandler(Mail_SendCompleted);

            return client;
        }
    }
}