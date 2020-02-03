using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Utils;

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
        /// Gets or sets a unique ID comming from LDAP to identify this user
        /// </summary>
        public string LdapGuid { get; set; }

        /// <summary>
        /// Gets whether a user is locked out right now (deleted / deactivated)
        /// </summary>
        [NotMapped]
        public bool IsDeleted => LockoutEnd > DateTime.Now;

        /// <summary>
        /// Gets or sets a list of roles the user is assigned to
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Sets all properties of a user
        /// </summary>
        /// <param name="name">Name of user</param>
        /// <param name="email">Email address of user</param>
        public void UpdateProperties(string name, string email)
        {
            UserName = name;
            Email = email;
            EmailConfirmed = true;
        }

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

        #region LDAP related

        /// <summary>
        /// Creates a new user based on information from LDAP
        /// </summary>
        /// <param name="ldapUser">LDAP user information</param>
        public static User FromLDAP(LdapUser ldapUser) =>
            new User(ldapUser.UserName)
            {
                Origin = Origins.LDAP,
                LdapGuid = ldapUser.LdapGuid,
                Email = ldapUser.Email,
                EmailConfirmed = true
            };

        /// <summary>
        /// Locks out a user and annonymizes his data
        /// </summary>
        internal void Deactivate()
        {
            // Lockout the user
            LockoutEnabled = true;
            LockoutEnd = DateTime.Now + TimeSpan.FromDays(365 * 200);
            // Annonymize the user
            UserName = GuidHelper.Unwrap(LdapGuid);
            Email = string.Empty;
            PhoneNumber = string.Empty;
            EmailConfirmed = false;
        }

        /// <summary>
        /// Reactivates a user
        /// </summary>
        /// <param name="user">LDAP user information</param>
        internal void Reactivate(LdapUser user)
        {
            // Unlock user
            LockoutEnabled = false;
            LockoutEnd = DateTime.Now;
            // Hydrate user data
            UserName = user.UserName;
            Email = user.Email;
            EmailConfirmed = true;
        }

        #endregion
    }
}