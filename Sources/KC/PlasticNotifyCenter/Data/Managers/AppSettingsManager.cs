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
        /// Saves new application settings
        /// </summary>
        /// <param name="baseUrl">Web page base URL</param>
        /// <param name="allowRegistration">Allow user registration</param>
        Task<int> SaveSettingsAsync(string baseUrl, bool allowRegistration);

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

        /// <summary>
        /// Gets the app settings entry (or null, if not jet configured)
        /// </summary>
        public AppSettings AppSettings =>
            _dbContext.AppSettings
                .Include(settings => settings.LdapConfig)
                .FirstOrDefault();

        #region Change settings

        /// <summary>
        /// Saves new application settings
        /// </summary>
        /// <param name="baseUrl">Web page base URL</param>
        /// <param name="allowRegistration">Allow user registration</param>
        public async Task<int> SaveSettingsAsync(string baseUrl, bool allowRegistration)
        {
            // AppSettings has only one record
            var appSettings = _dbContext.AppSettings.First();
            if (appSettings == null)
            {
                // Create first record as part of page setup
                appSettings = new AppSettings(baseUrl);
                _dbContext.AppSettings.Add(appSettings);
            }

            // Update record
            appSettings.BaseUrl = baseUrl;
            appSettings.AllowRegistration = allowRegistration;

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