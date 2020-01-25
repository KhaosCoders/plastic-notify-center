using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Group edit form view model
    /// </summary>
    public class EditGroupViewModel
    {
        /// <summary>
        /// Gets or sets the group (role) which is edited
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Gets or sets whether the group (role) is a new one (has not beed saved jet)
        /// </summary>
        public bool IsNewRole { get; set; }

        /// <summary>
        /// Gets or sets a list of users that are part of the group (role)
        /// </summary>
        public User[] UsersInRole { get; set; }

        /// <summary>
        /// Gets or sets a list of all users that a not part of the group (role)
        /// </summary>
        public User[] UserNotInRole { get; set; }
    }
}