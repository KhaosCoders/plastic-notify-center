using System;
using System.Collections.Generic;
using System.Linq;
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
        #region Role lists

        /// <summary>
        /// Returns an ordered list or active roles (groups)
        /// </summary>
        /// <param name="roleManager">RoleManager instance</param>
        public static IEnumerable<Role> GetOrderedRoles(this RoleManager<Role> roleManager) =>
            roleManager.Roles
                    .Where(role => !role.IsDeleted)
                    .ToList()
                    .OrderBy(role => role.Origin == Origins.Local ? 0 : 1)
                    .ThenBy(role => role.Name);

        /// <summary>
        /// Returns an ordered list or all roles (even deleted/deactivates)
        /// </summary>
        /// <param name="roleManager">RoleManager instance</param>
        public static IEnumerable<Role> GetOrderedRolesInclDeactivated(this RoleManager<Role> roleManager) =>
            roleManager.Roles
                    .ToList()
                    .OrderBy(role => role.Origin == Origins.Local ? 0 : 1)
                    .ThenBy(role => role.IsDeleted ? 1 : 0)
                    .ThenBy(role => role.Name);

        #endregion

        #region Deactivate / reactivate roles

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

        #endregion

        #region Add / edit roles

        /// <summary>
        /// Saves an existing role (group) or creates a new one based on supplied ID
        /// </summary>
        /// <param name="roleManager">RoleManager instance</param>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="id">Role-ID (if emtpy or null, a new role is created)</param>
        /// <param name="name">Name of role</param>
        /// <param name="userIDs">List of user-IDs assigned to the role</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when role is not found by ID</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when role can't be created or saved</exception>
        public static async Task SaveRoleAsync(this RoleManager<Role> roleManager, UserManager<User> userManager, string id, string name, string[] userIDs)
        {
            // New role or edited?
            Role role;

            if (string.IsNullOrWhiteSpace(id))
            {
                // New role
                role = new Role(name);
            }
            else
            {
                // Find role by ID
                role = await roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    throw new KeyNotFoundException("User was not found");
                }
            }

            // Set properties
            role.UpdateProperties(name);

            if (string.IsNullOrWhiteSpace(id))
            {
                // Add new role
                if (!(await roleManager.CreateAsync(role)).Succeeded)
                {
                    throw new InvalidOperationException("Can't create this new role (group)");
                }
            }
            else
            {
                // Save role
                if (!(await roleManager.UpdateAsync(role)).Succeeded)
                {
                    throw new InvalidOperationException("Role (group) can't be saved");
                }
            }

            // User assignments
            await userManager.SetRoleUsersAsync(role.Name, userIDs);
        }

        #endregion

        #region Delete roles

        /// <summary>
        /// Deletes a role (group) and all recipient entries for this group
        /// </summary>
        /// <param name="roleManager">UserManager instance</param>
        /// <param name="id">ID of user to delete</param>
        /// <param name="rulesManager">NotificationRulesManager instance</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when role is not found by ID</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when role can't be deleted</exception>
        public static async Task DeleteByIdAsync(this RoleManager<Role> roleManager, string id, INotificationRulesManager rulesManager)
        {
            // Find user by ID
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException("Role was not found");
            }

            if (role.IsBuildIn)
            {
                throw new InvalidOperationException("Build-In groups can't be deleted");
            }

            // Delete rule recipielt entries
            await rulesManager.DeleteRoleRecipientAsync(role);

            // Delete role
            if (!(await roleManager.DeleteAsync(role)).Succeeded)
            {
                throw new InvalidOperationException("Group can't be deleted");
            }
        }

        #endregion
    }
}