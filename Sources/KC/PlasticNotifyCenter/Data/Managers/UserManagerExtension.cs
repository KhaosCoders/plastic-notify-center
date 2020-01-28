using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Extend the UserManager with more functionallity
    /// </summary>
    public static class UserManagerExtension
    {
        /// <summary>
        /// Lock out a user and remove personal data (deactivate)
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="user">User to deactivate</param>
        public static async Task<IdentityResult> DeactivateUserAsync(this UserManager<User> userManager, User user)
        {
            user.Deactivate();
            return await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Remove the lockout from a user and rehzdrates personal data from LDAP user
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="user">User to activate</param>
        /// <param name="ldapUser">LDAP user information</param>
        public static async Task<IdentityResult> ReactivateUserAsync(this UserManager<User> userManager, User user, LdapUser ldapUser)
        {
            user.Reactivate(ldapUser);
            return await userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Adds a new user based on LDAP information and adds the user to the default 'Users' group (role)
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="ldapUser">LDAP information</param>
        public static async Task<IdentityResult> AddLdapUser(this UserManager<User> userManager, LdapUser ldapUser)
        {
            // Create user instance from LDAP information
            var user = User.FromLDAP(ldapUser);
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            // Every user is in the users role
            return await userManager.AddToRoleAsync(user, Roles.UserRole);
        }
    }
}