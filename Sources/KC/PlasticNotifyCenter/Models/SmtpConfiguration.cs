using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PlasticNotifyCenter.Data;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Data model for SMTP configuration
    /// </summary>
    public class SmtpConfiguration
    {
        /// <summary>
        /// Gets or sets the name of a SMTP notifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the host name of a SMTP server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets whether SSL should be used or not
        /// </summary>
        public bool EnableSSL { get; set; }

        /// <summary>
        /// Gets or sets the port of a SMTP server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the email address used to send mails from
        /// </summary>
        public string SenderMail { get; set; }

        /// <summary>
        /// Gets or sets an alias name used to send mails from
        /// </summary>
        public string SenderAlias { get; set; }

        /// <summary>
        /// Gets or sets a user name useed to authenticate with the SMTP server
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets a password useed to authenticate with the SMTP server
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a recipiant email address used to test the SMTP configuration
        /// </summary>
        public string ToMail { get; set; }

        /// <summary>
        /// Validates the configuration for testing
        /// </summary>
        /// <returns>Returns true when the configuration is complete</returns>
        public bool ValidateTest() =>
            ValidateConfig()
             && !string.IsNullOrWhiteSpace(ToMail);

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>Returns true when the configuration is complete</returns>
        public bool ValidateConfig() =>
            !string.IsNullOrWhiteSpace(Host)
             && !string.IsNullOrWhiteSpace(SenderMail);

        /// <summary>
        /// Parses the Json body of a smtp test to a SmtpMailTest model object
        /// </summary>
        /// <param name="data">Json data</param>
        /// <returns>Parsed model object</returns>
        public static async Task<SmtpConfiguration> ParseJsonAsync(string body)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                return await JsonSerializer.DeserializeAsync<SmtpConfiguration>(ms);
            }
        }

        /// <summary>
        /// Creates a new instance from a SmtpNotifierData record
        /// </summary>
        /// <param name="data">SmtpNotifierData record</param>
        public static SmtpConfiguration FromData(SmtpNotifierData data)
            => new SmtpConfiguration()
            {
                Name = data.DisplayName,
                Host = data.Host,
                EnableSSL = data.EnableSSL,
                Port = data.Port,
                SenderMail = data.SenderMail,
                SenderAlias = data.SenderAlias,
                Username = data.Username,
                Password = data.Password
            };

        /// <summary>
        /// Transforms the config object to a string (JSON  format)
        /// </summary>
        public override string ToString() =>
            JsonSerializer.Serialize(this, this.GetType());
    }
}