using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Mail;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Notifiers
{
    // Interface is used to register the SmtpNotifier as a service
    public interface ISmtpNotifier { }

    /// <summary>
    /// Sends email notfications via SMTP
    /// </summary>
    public class SmtpNotifier : NotifierBase<SmtpNotifierData>, ISmtpNotifier
    {
        #region Dependencies

        // Mail service used to send the notifications
        private readonly IMailService _mailService;

        public SmtpNotifier(ILogger<SmtpNotifier> logger, IMailService mailService, PncDbContext dbContext)
            : base(logger, dbContext)
        {
            this._mailService = mailService;
        }

        #endregion

        #region NotifierBase methods

        /// <summary>
        /// Implementation of abstract method from NotifierBase to send a notification to each recipient
        /// </summary>
        /// <param name="data">SMTP notifier configuration data</param>
        /// <param name="message">Message to send</param>
        /// <param name="recipients">List of recipients</param>
        /// <returns></returns>
        protected override IEnumerable<Task> GetRecipientTasks(SmtpNotifierData data, Message message, IEnumerable<User> recipients)
        {
            // Prepare the config for the SMTP client
            SmtpConfiguration config = SmtpConfiguration.FromData(data);

            // Start a new task for each recipient
            return recipients.Select(recipient => Task.Run(() => SendMessage(config, message, recipient)));
        }

        #endregion

        #region SMTP messages

        /// <summary>
        /// Send an email message via SMTP
        /// </summary>
        /// <param name="config">SMTP configuration for the mail client</param>
        /// <param name="message">Message</param>
        /// <param name="recipient">Recipient of message</param>
        private void SendMessage(SmtpConfiguration config, Message message, User recipient)
        {
            if (string.IsNullOrWhiteSpace(recipient.Email))
            {
                Logger.LogWarning("User {user} has no email", recipient.UserName);
                return;
            }
            Logger.LogDebug("Sending mail to {email}", recipient.Email);

            try
            {
                // Create new client
                SmtpClient client = _mailService.CreateSmtpClient(config);

                // Start a new message
                MailMessage mailMessage = _mailService.CreateMessage(config);

                // Add the receiver
                mailMessage.To.Add(new MailAddress(recipient.Email));
                // Set content
                mailMessage.Subject = message.Title;
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.IsBodyHtml = message.Body.StartsWith("<html", System.StringComparison.InvariantCultureIgnoreCase);
                mailMessage.Body = message.Body;

                // Send the message
                client.Send(mailMessage);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to send mail via SMTP");
                throw e; // Task has to fail, to be counted as failure
            }
        }

        #endregion
    }
}