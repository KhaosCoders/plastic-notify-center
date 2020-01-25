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
    }
}