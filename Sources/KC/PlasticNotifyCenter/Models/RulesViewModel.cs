using PlasticNotifyCenter.Data;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Rules page view model
    /// </summary>
    public class RulesViewModel
    {
        /// <summary>
        /// Gets or sets a list of all rules
        /// </summary>
        public NotificationRule[] Rules { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="rules">List of all rules</param>
        public RulesViewModel(NotificationRule[] rules)
        {
            Rules = rules;
        }
    }
}