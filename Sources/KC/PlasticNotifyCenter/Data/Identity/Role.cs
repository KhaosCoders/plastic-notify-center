using Microsoft.AspNetCore.Identity;

namespace PlasticNotifyCenter.Data.Identity
{
    /// <summary>
    /// User-Role used by Identity services
    /// </summary>
    public class Role : IdentityRole
    {
        /// <summary>
        /// Gets or sets where the role was created
        /// </summary>
        public Origins Origin { get; set; } = Origins.Local;

        /// <summary>
        /// Gets or sets whether the role is a build-in role
        /// Those can't be deleted
        /// </summary>
        public bool IsBuildIn { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public Role() : base()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name">Name of the role</param>
        public Role(string name) : base(name)
        { }
    }
}