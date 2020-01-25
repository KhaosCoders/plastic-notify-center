using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Notifiers;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Notifier edit form view model
    /// </summary>
    public class NotifierViewModel
    {
        /// <summary>
        /// Gets or sets a list of all notifiers
        /// </summary>
        public NotifierInfo[] Notifiers { get; set; }

        /// <summary>
        /// Gets or sets the selected notifier data
        /// </summary>
        public BaseNotifierData SelectedNotifier { get; set; }

        /// <summary>
        /// Gets or sets a list of all supported notifier types
        /// </summary>
        public NotifierAttribute[] NotifierTypes { get; set; }
    }

    /// <summary>
    /// Reduced information model of a notifier
    /// </summary>
    public class NotifierInfo
    {
        /// <summary>
        /// Gets or sets a unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the notifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the icon HTML code for the notifier
        /// </summary>
        public string Icon { get; set; }
    }
}