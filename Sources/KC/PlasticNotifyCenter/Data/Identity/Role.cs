using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using PlasticNotifyCenter.Models;

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
        /// Gets or sets a unique ID comming from LDAP to identify this role (group)
        /// </summary>
        public string LdapGuid { get; set; }

        /// <summary>
        /// Gets or sets whether the role is a build-in role
        /// Those can't be deleted
        /// </summary>
        public bool IsBuildIn { get; set; }

        /// <summary>
        /// Gets or sets whether the role (group) was deleted
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Gets or sets a list of users assigned to the role
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Sets all properties of a role (group)
        /// </summary>
        /// <param name="name">Name of role</param>
        public void UpdateProperties(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public Role()
            : base()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name">Name of the role</param>
        public Role(string name)
            : base(name)
        { }

        #region LDAP related

        /// <summary>
        /// Creates a new role based on information from LDAP
        /// </summary>
        /// <param name="ldapGroup">LDAP group information</param>
        public static Role FromLDAP(LdapGroup ldapGroup) =>
            new Role(ldapGroup.Name)
            {
                Origin = Origins.LDAP,
                LdapGuid = ldapGroup.LdapGuid
            };

        /// <summary>
        /// Marks a role (group) as deactivated/deleted
        /// </summary>
        internal void Deactivate()
        {
            IsDeleted = true;
            Name = LdapGuid;
        }

        /// <summary>
        /// Reactivates a role (group)
        /// </summary>
        /// <param name="ldapGroup">LDAP group information</param>
        internal void Reactivate(LdapGroup ldapGroup)
        {
            IsDeleted = false;
            Name = ldapGroup.Name;
        }

        #endregion
    }
}