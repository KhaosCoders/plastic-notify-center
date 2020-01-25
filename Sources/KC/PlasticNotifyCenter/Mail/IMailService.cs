using System.Net.Mail;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Mail
{
    /// <summary>
    /// A service providing eMail related services
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Sends a test mail with the supplies configuration.
        /// Returns true when successfull
        /// </summary>
        /// <param name="data">SMTP configuration to test</param>
        bool SendTestMail(SmtpConfiguration data);

        /// <summary>
        /// Creates a new SMTP client instance
        /// </summary>
        /// <param name="data">Configuration to use for the SMTP client</param>
        SmtpClient CreateSmtpClient(SmtpConfiguration data);

        /// <summary>
        /// Prepares a new eMail message instance
        /// </summary>
        /// <param name="data">Configuration to use for the message</param>
        MailMessage CreateMessage(SmtpConfiguration data);
    }
}