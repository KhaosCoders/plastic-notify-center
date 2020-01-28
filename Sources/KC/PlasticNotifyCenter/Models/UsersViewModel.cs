using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Admin users page view model
    /// </summary>
    public class UsersViewModel
    {
        /// <summary>
        /// Gets or sets a list of all users
        /// </summary>
        public User[] Users { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="users">List of users to display in view</param>
        public UsersViewModel(User[] users)
        {
            Users = users;
        }
    }
}