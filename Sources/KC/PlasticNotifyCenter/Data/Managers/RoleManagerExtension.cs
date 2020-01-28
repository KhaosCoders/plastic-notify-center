using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Extend the RoleManager with more functionallity
    /// </summary>
    public static class RoleManagerExtension
    {
        /// <summary>
        /// Marks a role (group) as deleted
        /// </summary>
        /// <param name="roleManager">RoleManager instance</param>
        /// <param name="role">Role to deactivate</param>
        public static async Task<IdentityResult> DeactivateRoleAsync(this RoleManager<Role> roleManager, Role role)
        {
            role.Deactivate();
            return await roleManager.UpdateAsync(role);
        }

        /// <summary>
        /// Removes the Deleted-Mark from a role (group) again and rehydrates the information from LDAP
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="role"></param>
        /// <param name="ldapGroup"></param>
        public static async Task<IdentityResult> ReactivateRoleAsync(this RoleManager<Role> roleManager, Role role, LdapGroup ldapGroup)
        {
            role.Reactivate(ldapGroup);
            return await roleManager.UpdateAsync(role);
        }
    }
}