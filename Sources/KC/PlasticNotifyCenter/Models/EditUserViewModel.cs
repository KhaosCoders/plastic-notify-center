using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// User edit form view model
    /// </summary>
    public class EditUserViewModel
    {
        /// <summary>
        /// Gets or sets the user beeing edited
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets whether the user is new (not saved jet)
        /// </summary>
        public bool IsNewUser { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="user">User to edit</param>
        /// <param name="isNewUser">true if it's a new (unsaved) user</param>
        public EditUserViewModel(User user, bool isNewUser)
        {
            User = user;
            IsNewUser = isNewUser;
        }
    }
}