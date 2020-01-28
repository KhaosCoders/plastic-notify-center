using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Notifiers;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Data for a SMTP notifier
    /// </summary>
    [Notifier(notifierType: typeof(SmtpNotifier), id: "smtp", name: "SMTP Notifier", icon: @"<i class=""fas fa-at""></i>")]
    public class SmtpNotifierData : BaseNotifierData
    {
        /// <summary>
        /// Gets or sets a hostname of a SMTP server
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets whether SSL should be used to send the SMTP message
        /// </summary>
        public bool EnableSSL { get; set; }

        /// <summary>
        /// Gets or sets a port of a SMTP server
        /// </summary>
        [Required]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a email-address from wich the SMTP message will be send
        /// </summary>
        [Required]
        public string SenderMail { get; set; }

        /// <summary>
        /// Gets or sets an alias shown as sender of the SMTP message
        /// </summary>
        public string SenderAlias { get; set; }

        /// <summary>
        /// Gets or sets a username used to authenticate with the SMTP server
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets a password used to authenticate with the SMTP server
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private SmtpNotifierData()
            : base()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="displayName">Display name of notifier</param>
        public SmtpNotifierData(string displayName)
            : base(displayName)
        { }

        /// <summary>
        /// Creates a new instance and applies the provided configuration values
        /// </summary>
        /// <param name="configuration">SMTP configuration values</param>
        public static SmtpNotifierData CreateFrom(SmtpConfiguration configuration) =>
            CopyPropertyValues(new SmtpNotifierData(configuration.Name), configuration);

        /// <summary>
        /// Copies all values of a SmtpConfiguration to the data model
        /// </summary>
        /// <param name="notifier">data model</param>
        /// <param name="configuration">SMTP configuration</param>
        private static SmtpNotifierData CopyPropertyValues(SmtpNotifierData notifier, SmtpConfiguration configuration)
        {
            notifier.DisplayName = configuration.Name;
            notifier.Host = configuration.Host;
            notifier.EnableSSL = configuration.EnableSSL;
            notifier.Port = configuration.Port;
            notifier.SenderMail = configuration.SenderMail;
            notifier.SenderAlias = configuration.SenderAlias;
            notifier.Username = configuration.Username;
            if (!string.Equals(configuration.Password, "****"))
            {
                notifier.Password = configuration.Password;
            }
            return notifier;
        }

        /// <summary>
        /// Parses a JSON string and applies the configuration values to the model
        /// </summary>
        /// <param name="jsonData">JSON string</param>
        public override async Task ApplyJsonPropertiesAsync(string jsonData)
        {
            SmtpConfiguration configuration = await SmtpConfiguration.ParseJsonAsync(jsonData);
            CopyPropertyValues(this, configuration);
        }
    }
}