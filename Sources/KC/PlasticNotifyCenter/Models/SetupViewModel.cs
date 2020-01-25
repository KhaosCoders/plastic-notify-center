using PlasticNotifyCenter.Data;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Setup page view model
    /// </summary>
    public class SetupViewModel
    {
        // Only used by RazorPage layout to load the SMTP notifier form
        public SmtpNotifierData SmtpNotifier { get; set; }
    }
}