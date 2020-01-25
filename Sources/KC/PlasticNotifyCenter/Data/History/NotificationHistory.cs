using System;
using System.ComponentModel.DataAnnotations;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// A entry in the history of notifications
    /// </summary>
    public class NotificationHistory
    {
        /// <summary>
        /// Gets a unique ID for the entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a timestamp of the notfication
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the notifier
        /// </summary>
        [Required]
        public string NotifierName { get; set; }

        /// <summary>
        /// Gets or sets the number of successfull notifications send
        /// </summary>
        [Required]
        public int SuccessCount { get; set; }

        /// <summary>
        /// Gets or sets the number of failed notifications
        /// </summary>
        [Required]
        public int FailedCount { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private NotificationHistory()
        { }

        /// <summary>
        /// Creates a new instance of NotificationHistory
        /// </summary>
        /// <param name="notifier">Name of the notifier</param>
        public NotificationHistory(string notifier)
        {
            this.NotifierName = notifier;
            this.Timestamp = DateTime.UtcNow;
        }
    }
}