using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Notification rule edit form view model
    /// </summary>
    public class EditRuleViewModel
    {
        /// <summary>
        /// Gets or sets the rule that is been edited
        /// </summary>
        public NotificationRule Rule { get; set; }

        /// <summary>
        /// Gets or sets a list of all known Plastic SCM trigger types
        /// </summary>
        public string[] KnownTriggers { get; set; }

        /// <summary>
        /// Gets or sets a list of all notifiers not used by this rule
        /// </summary>
        public BaseNotifierData[] UnusedNotifiers { get; set; }

        /// <summary>
        /// Gets or sets a list of all users not affected by the rule
        /// </summary>
        public User[] OtherUsers { get; set; }

        /// <summary>
        /// Gets or sets a list of all groups (roles) not affected by the rule
        /// </summary>
        public Role[] OtherRoles { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="rule">The rule edited</param>
        /// <param name="triggers">List of known trigger types</param>
        /// <param name="unusedNotifiers">List of unaffected notifiers</param>
        /// <param name="otherUsers">List of unaffected users</param>
        /// <param name="otherRoles">List of unaffected groups</param>
        public EditRuleViewModel(NotificationRule rule, string[] triggers, BaseNotifierData[] unusedNotifiers, User[] otherUsers, Role[] otherRoles)
        {
            Rule = rule;
            KnownTriggers = triggers;
            UnusedNotifiers = unusedNotifiers;
            OtherUsers = otherUsers;
            OtherRoles = otherRoles;
        }
    }
}