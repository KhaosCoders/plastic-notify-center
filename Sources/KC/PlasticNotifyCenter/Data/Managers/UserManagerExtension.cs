using System;
using System.Collections.Generic;
using System.Linq;
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
        #region User lists
        
        /// <summary>
        /// Returns an ordered list or active users
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        public static IEnumerable<User> GetOrderedUsers(this UserManager<User> userManager) =>
            userManager.Users
                    .ToList()
                    .Where(user => !user.IsDeleted)
                    .OrderBy(user => user.Origin == Origins.Local ? 0 : 1)
                    .ThenBy(user => user.UserName);

        /// <summary>
        /// Returns an ordered list or all users (even deleted/deactivates)
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        public static IEnumerable<User> GetOrderedUsersInclDeactivated(this UserManager<User> userManager) =>
            userManager.Users
                    .ToList()
                    .OrderBy(user => user.Origin == Origins.Local ? 0 : 1)
                    .ThenBy(user => user.IsDeleted ? 1 : 0)
                    .ThenBy(user => user.UserName);

        #endregion

        #region Deactivate / reactivate users

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

        #endregion

        #region Add LDAP users

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

        #endregion

        #region Add / edit users

        /// <summary>
        /// Saves an existing user or creates a new one based on supplied ID
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="id">User-ID (if emtpy or null, a new user is created)</param>
        /// <param name="userName">Name of user</param>
        /// <param name="password">Password (mandatory for new users)</param>
        /// <param name="email">Email address of user</param>
        /// <exception cref="System.ArgumentException">Thrown when mandatory parameter is missing</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when user is not found by ID</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when user can't be created or saved</exception>
        public static async Task SaveUserAsync(this UserManager<User> userManager, string id, string name, string password, string email)
        {
            // New user or edited?
            User user;

            if (string.IsNullOrWhiteSpace(id))
            {
                // New user
                if (string.IsNullOrWhiteSpace(password))
                {
                    // New users need a password
                    throw new ArgumentException("Password is mandatory for new users", nameof(password));
                }
                user = new User(name);
            }
            else
            {
                // Find user by ID
                user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new KeyNotFoundException("User was not found");
                }
            }

            // Set properties
            user.UpdateProperties(name, email);

            if (string.IsNullOrWhiteSpace(id))
            {
                // Add new user
                if (!(await userManager.CreateAsync(user, password)).Succeeded)
                {
                    throw new InvalidOperationException("Can't create this new user");
                }
                // All users get the User role
                await userManager.AddToRoleAsync(user, Roles.UserRole);
            }
            else
            {
                // Change password?
                if (!string.IsNullOrWhiteSpace(password))
                {
                    // validate password
                    if (userManager.PasswordValidators.Any(v => !v.ValidateAsync(userManager, user, password).Result.Succeeded))
                    {
                        throw new InvalidOperationException("Password is not valid");
                    }
                    // hash password
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);
                }

                // save user changes
                if (!(await userManager.UpdateAsync(user)).Succeeded)
                {
                    throw new InvalidOperationException("User can't be saved");
                }
            }
        }

        #endregion

        #region Delete users

        /// <summary>
        /// Deletes a user and all recipient entries for this user
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="id">ID of user to delete</param>
        /// <param name="rulesManager">NotificationRulesManager instance</param>
        /// <exception cref="System.InvalidOperationException">Thrown when user can't be deleted</exception>
        public static async Task DeleteByIdAsync(this UserManager<User> userManager, string id, INotificationRulesManager rulesManager)
        {
            // Find user by ID
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User was not found");
            }

            // Delete rule recipielt entries
            await rulesManager.DeleteUserRecipientAsync(user);

            // Delete user
            if (!(await userManager.DeleteAsync(user)).Succeeded)
            {
                throw new InvalidOperationException("User can't be deleted");
            }
        }

        #endregion

        #region Set role users

        /// <summary>
        /// Sets all users assigned to a role (removes the ones not mentioned)
        /// </summary>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="roleName">Name of role</param>
        /// <param name="userIDs">List or user-IDs assigned to the role</param>
        public static async Task SetRoleUsersAsync(this UserManager<User> userManager, string roleName, string[] userIDs)
        {
            // Cycle through all users
            foreach (var user in userManager.Users.ToList().Where(user => !user.IsDeleted))
            {
                // Has user the role currently?
                var inRole = await userManager.IsInRoleAsync(user, roleName);
                if (inRole && !userIDs.Contains(user.Id))
                {
                    // User should not have the role anymore
                    await userManager.RemoveFromRoleAsync(user, roleName);
                }
                else if (!inRole && userIDs.Contains(user.Id))
                {
                    // User should have the role now
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        #endregion
    }
}