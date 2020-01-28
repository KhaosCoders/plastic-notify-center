using System.Collections.Generic;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// A user found via LDAP
    /// </summary>
    public class LdapUser
    {
        /// <summary>
        /// Gets a unique ID from LDAP
        /// </summary>
        public string LdapGuid { get; set; }

        /// <summary>
        /// Gets the user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets the email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="guid">LDAP GUID</param>
        /// <param name="name">User name</param>
        /// <param name="email">Email address</param>
        public LdapUser(string guid, string name, string email)
        {
            LdapGuid = guid;
            UserName = name;
            Email = email;
        }
    }

    /// <summary>
    /// A group found via LDAP
    /// </summary>
    public class LdapGroup
    {
        /// <summary>
        /// Gets a unique ID from LDAP
        /// </summary>
        public string LdapGuid { get; set; }

        /// <summary>
        /// Gets the groups name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a list of GUIDs assigned to the group
        /// </summary>
        /// <typeparam name="string"></typeparam>
        public IList<string> UserGuids { get; set; }
            = new List<string>();

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="guid">LDAP GUID</param>
        /// <param name="name">Group name</param>
        public LdapGroup(string guid, string name)
        {
            LdapGuid = guid;
            Name = name;
        }
    }
}