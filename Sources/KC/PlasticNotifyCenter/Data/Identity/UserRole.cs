using Microsoft.AspNetCore.Identity;

namespace PlasticNotifyCenter.Data.Identity
{
    /// <summary>
    /// Many-to-many brigde between Users and Roles
    /// </summary>
    public class UserRole : IdentityUserRole<string>
    {
        /// <summary>
        /// Gets or sets a user
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets a role
        /// </summary>
        public virtual Role Role { get; set; }
    }
}