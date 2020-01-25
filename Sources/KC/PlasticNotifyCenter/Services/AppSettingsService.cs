using System.Linq;
using PlasticNotifyCenter.Data;

namespace PlasticNotifyCenter.Services
{
    /// <summary>
    /// Service used by Razor-Pages to determin application settings
    /// </summary>
    public class AppSettingsService : IAppSettingsService
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;

        public AppSettingsService(PncDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        #endregion

        /// <summary>
        /// Gets whether users can register a new account manually
        /// </summary>
        public bool IsRegisterAllowed => _dbContext?.AppSettings.FirstOrDefault()?.AllowRegistration ?? false;
    }
}