namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Stats page view model
    /// </summary>
    public class StatsViewModel
    {
        /// <summary>
        /// Gets or sets a list of stats for trigger events
        /// </summary>
        public TriggerStats[] TriggerStats { get; set; }

        /// <summary>
        /// Gets or sets the number of defined rules
        /// </summary>
        public int RuleCount { get; set; }

        /// <summary>
        /// Gets or sets a list of stats abount send notifications
        /// </summary>
        public NotificationStats[] NotificationStats { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="triggerStats">List of stats for trigger calls</param>
        /// <param name="ruleCount">Number or rules</param>
        /// <param name="notificationStats">List of stats for notifier messages</param>
        public StatsViewModel(TriggerStats[] triggerStats, int ruleCount, NotificationStats[] notificationStats)
        {
            TriggerStats = triggerStats;
            RuleCount = ruleCount;
            NotificationStats = notificationStats;
        }
    }

    /// <summary>
    /// Stats abount a trigger type
    /// </summary>
    public class TriggerStats
    {
        /// <summary>
        /// Gets or sets the name of the trigger type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the invokation count of the trigger type
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// Stats about a notifier
    /// </summary>
    public class NotificationStats
    {
        /// <summary>
        /// Gets or sets the name of the notifier
        /// </summary>
        public string Notifier { get; set; }

        /// <summary>
        /// Gets or sets a HTML code used a icon for the notifier type
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the number of successfull notifications send by the notifier type
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Gets or sets the number of failed notificaation for the notifier type
        /// </summary>
        public int FailedCount { get; set; }
    }
}