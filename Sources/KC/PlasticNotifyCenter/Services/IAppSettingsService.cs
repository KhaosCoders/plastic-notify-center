namespace PlasticNotifyCenter.Services
{
    /// <summary>
    /// Retrievs application settings
    /// </summary>
    public interface IAppSettingsService
    {
        /// <summary>
        /// Gets whether users can register a new account manually
        /// </summary>
        bool IsRegisterAllowed { get; }
    }
}