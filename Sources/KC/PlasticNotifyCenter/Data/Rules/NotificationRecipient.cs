using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// A recipient-group or -user of a notification rule
    /// </summary>
    public class NotificationRecipient
    {
        /// <summary>
        /// Gets or sets a unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the recieving user
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the recieving role (group)
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Gets or sets the relevant rule
        /// </summary>
        public NotificationRule NotificationRule { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private NotificationRecipient()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="user">User who recieves the notification</param>
        public NotificationRecipient(User user)
        {
            User = user;
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="role">Group who recieves the notification</param>
        public NotificationRecipient(Role role)
        {
            Role = role;
        }
    }
}