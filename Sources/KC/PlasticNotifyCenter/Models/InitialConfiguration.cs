using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// POST data model for setup assistant
    /// </summary>
    public class InitialConfiguration
    {
        /// <summary>
        /// Gets or sets the base URL of this page
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the password for the admin user
        /// </summary>
        public string AdminPw { get; set; }

        /// <summary>
        /// Gets or sets the email address for the admin user
        /// </summary>
        public string AdminEmail { get; set; }

        /// <summary>
        /// Gets or sets the user name for the admin user
        /// </summary>
        public string AdminUsername { get; set; }

        /// <summary>
        /// Gets or sets the configuration for a SMTP notifier
        /// </summary>
        public SmtpConfiguration Smtp { get; set; }

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>Returns true if the configuration is valid</returns>
        public bool Validate() =>
            !string.IsNullOrWhiteSpace(BaseUrl)
            && !string.IsNullOrWhiteSpace(AdminPw)
            && !string.IsNullOrWhiteSpace(AdminEmail)
            && !string.IsNullOrWhiteSpace(AdminUsername)
            && Smtp.ValidateConfig();

        /// <summary>
        /// Parses the Json body of a smtp test to a SmtpMailTest model object
        /// </summary>
        /// <param name="data">Json data</param>
        /// <returns>Parsed model object</returns>
        public static async Task<InitialConfiguration> ParseJsonAsync(string body)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                return await JsonSerializer.DeserializeAsync<InitialConfiguration>(ms);
            }
        }
    }
}