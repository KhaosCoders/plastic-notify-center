using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Defines when a trigger should send a notification
    /// </summary>
    public class NotificationRule
    {
        /// <summary>
        /// Gets or sets a unique Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a name for the rule
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of creation
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the owning user of the rule
        /// </summary>
        [Required]
        public User Owner { get; set; }

        /// <summary>
        /// Gets or sets whether the rule is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a C# expression, which acts as filter for the rule
        /// </summary>
        public string AdvancedFilter { get; set; }

        /// <summary>
        /// Gets or sets the trigger name, this rule should be invokes for
        /// </summary>
        [Required]
        public string Trigger { get; set; }

        /// <summary>
        /// Gets or sets the title of the notification
        /// </summary>
        public string NotificationTitle { get; set; }

        /// <summary>
        /// Gets or sets the body text of the notification
        /// </summary>
        [Required]
        public string NotificationBody { get; set; }

        /// <summary>
        /// Gets or sets the type of text forming the message body
        /// </summary>
        public MessageBodyType NotificationBodyType { get; set; }

        /// <summary>
        /// Gets or sets whether the global message layout should be applied
        /// </summary>
        public bool UseGlobalMessageTemplate { get; set; }

        /// <summary>
        /// Gets or sets a list of tags, separated by kommas
        /// </summary>
        public string NotificationTags { get; set; }

        /// <summary>
        /// Gets or sets a list of notifiers invoked by this rule
        /// </summary>
        public virtual ICollection<RuleNotifier> Notifiers { get; set; }

        /// <summary>
        /// Gets or sets a list of recipients informed by this rule
        /// </summary>
        public virtual ICollection<NotificationRecipient> Recipients { get; set; }

        /// <summary>
        /// Updates all properties with new values
        /// </summary>
        /// <param name="name">Name of rule</param>
        /// <param name="trigger">Name of trigger type</param>
        /// <param name="filter">Advaced filter</param>
        /// <param name="title">Message title</param>
        /// <param name="message">Message body</param>
        /// <param name="tags">Message tags</param>
        public void UpdateProperties(string name, string trigger, string filter, string title, string message, string tags)
        {
            DisplayName = name;
            Trigger = trigger;
            AdvancedFilter = filter;
            NotificationTitle = title;
            NotificationBody = message;
            NotificationTags = tags;
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private NotificationRule()
        {
            this.Notifiers = new HashSet<RuleNotifier>();
            this.Recipients = new HashSet<NotificationRecipient>();
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="displayName">Name or new rule</param>
        /// <remarks>
        /// Only use as new rule for edit form
        /// </remarks>
        public NotificationRule(string displayName)
            : this()
        {
            DisplayName = displayName;
            NotificationBodyType = MessageBodyType.HTML;
            UseGlobalMessageTemplate = true;
        }

        /// <summary>
        /// Create a new active rule
        /// </summary>
        /// <param name="owner">Owner of new rule</param>
        public NotificationRule(User owner)
            : this()
        {
            Owner = owner;
            CreationTime = DateTime.UtcNow;
            IsActive = true;
        }

        /// <summary>
        /// Assigns a new Id if needed
        /// </summary>
        public void EnsureId() =>
            Id = Id ?? Guid.NewGuid().ToString();
    }
}