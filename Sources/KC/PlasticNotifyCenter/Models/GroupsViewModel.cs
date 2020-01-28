using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Admin groups pages view model
    /// </summary>
    public class GroupsViewModel
    {
        /// <summary>
        /// Gets a list of all groups (roles)
        /// </summary>
        public Role[] Roles { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="roles">List of roles (groups) to display in view</param>
        public GroupsViewModel(Role[] roles)
        {
            Roles = roles;
        }
    }
}