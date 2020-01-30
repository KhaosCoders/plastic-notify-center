using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Many-to-many mapping for rules to notifiers
    /// </summary>
    public class RuleNotifier
    {
        /// <summary>
        /// Gets or sets a unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a notification rule
        /// </summary>
        public NotificationRule Rule { get; set; }

        /// <summary>
        /// Gets or sets a notifier
        /// </summary>
        public BaseNotifierData Notifier { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private RuleNotifier()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="notifier">Notifier</param>
        public RuleNotifier(BaseNotifierData notifier)
        {
            Notifier = notifier;
        }
    }
}