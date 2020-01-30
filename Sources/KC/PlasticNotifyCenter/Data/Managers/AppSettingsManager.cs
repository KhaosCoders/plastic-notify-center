using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Manages the configurable app settings
    /// </summary>
    public interface IAppSettingsManager
    {
        /// <summary>
        /// Gets the app settings entry (or null, if not jet configured)
        /// </summary>
        AppSettings AppSettings { get; }

        /// <summary>
        /// Gets the LDAP configurtion (or null, if not jet configured)
        /// </summary>
        LdapSettings LdapConfig { get; }

        /// <summary>
        /// Gets whether users can register a new account manually
        /// </summary>
        bool IsRegisterAllowed { get; }

        /// <summary>
        /// Saves new application settings
        /// </summary>
        /// <param name="baseUrl">Web page base URL</param>
        /// <param name="allowRegistration">Allow user registration</param>
        /// <param name="htmlMessageTemplate">HTML message template text</param>
        Task<int> SaveSettingsAsync(string baseUrl, bool allowRegistration, string htmlMessageTemplate);

        /// <summary>
        /// Saves new LDAP settings
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        Task<int> ChangeLdapConfig(LdapSettings ldapConfig);
    }

    /// <summary>
    /// Implementation of IAppSettingsManager
    /// </summary>
    public class AppSettingsManager : IAppSettingsManager
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;

        public AppSettingsManager(PncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Get app settings

        /// <summary>
        /// Gets the app settings entry (or null, if not jet configured)
        /// </summary>
        public AppSettings AppSettings =>
            _dbContext.AppSettings
                .Include(settings => settings.LdapConfig)
                .FirstOrDefault();

        /// <summary>
        /// Gets the LDAP configurtion (or null, if not jet configured)
        /// </summary>
        public LdapSettings LdapConfig =>
            AppSettings?.LdapConfig;

        /// <summary>
        /// Gets whether users can register a new account manually
        /// </summary>
        public bool IsRegisterAllowed =>
            AppSettings?.AllowRegistration ?? false;

        #endregion

        #region Change settings

        /// <summary>
        /// Saves new application settings
        /// </summary>
        /// <param name="baseUrl">Web page base URL</param>
        /// <param name="allowRegistration">Allow user registration</param>
        /// <param name="htmlMessageTemplate">HTML message template text</param>
        public async Task<int> SaveSettingsAsync(string baseUrl, bool allowRegistration, string htmlMessageTemplate)
        {
            // AppSettings has only one record
            var appSettings = await _dbContext.AppSettings.FirstOrDefaultAsync();
            if (appSettings == null)
            {
                // Create first record as part of page setup
                appSettings = new AppSettings(baseUrl);
                await _dbContext.AppSettings.AddAsync(appSettings);
            }

            // Update record
            appSettings.BaseUrl = baseUrl;
            appSettings.AllowRegistration = allowRegistration;
            appSettings.HtmlMessageTemplate = htmlMessageTemplate;

            // Save
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Saves new LDAP settings
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        public async Task<int> ChangeLdapConfig(LdapSettings ldapConfig)
        {
            // AppSettings has only one record
            var appSettings = _dbContext.AppSettings.First();

            // Update config
            appSettings.LdapConfig = ldapConfig;

            // Save
            return await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}