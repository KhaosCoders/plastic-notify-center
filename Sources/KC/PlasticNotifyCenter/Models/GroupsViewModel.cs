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
    }
}