using Microsoft.AspNetCore.Identity;

namespace PlasticNotifyCenter.Data.Identity
{
    /// <summary>
    /// User used be Indentity services
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets where the user was created
        /// </summary>
        public Origins Origin { get; set; } = Origins.Local;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public User() : base()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="username">Username</param>
        public User(string username) : base(username)
        { }
    }
}