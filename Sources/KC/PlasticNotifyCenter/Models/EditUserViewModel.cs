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
    }
}