using System;
using System.ComponentModel.DataAnnotations;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Application settings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets a unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the URL the site is reachable from
        /// </summary>
        [Required]
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets whether users can register themselves
        /// </summary>
        [Required]
        public bool AllowRegistration { get; set; }

        #region LDAP 

        /// <summary>
        /// Gets or sets the timestamp of the last LDAP sync
        /// </summary>
        public DateTime LdapSyncTimestamp {get; set; }

        /// <summary>
        /// Gets or sets the host name of the domain controller
        /// </summary>
        public string LdapDcHost { get; set; }

        /// <summary>
        /// Gets or sets the port used for LDAP querys
        /// </summary>
        public int LdapDcPort { get; set; }

        /// <summary>
        /// Gets or sets whether a secure SLL channel should be used
        /// </summary>
        public bool LdapDcSSL { get; set; }

        /// <summary>
        ///  Gets or set the base dynamic name
        /// </summary>
        public string LdapBaseDN { get; set; }

        /// <summary>
        /// Gets or sets an addition to the BaseDN used to access users
        /// </summary>
        public string LdapUserDN { get; set; }

        /// <summary>
        /// Gets or sets an addition to the BaseDN used to access groups
        /// </summary>
        public string LdapGroupDN { get; set; }

        /// <summary>
        /// Gets or sets a filter expression used to for filter only users
        /// </summary>
        public string LdapUserFilter { get; set; } ="(&(objectCategory=Person)(sAMAccountName=*))";

        /// <summary>
        /// Gets or sets a filter expression used to for filter only groups
        /// </summary>
        public string LdapGroupFilter { get; set; } = "(objectCategory=Group)";

        /// <summary>
        /// Gets or sets the attribute name for user names
        /// </summary>
        public string LdapUserNameAttr { get; set; } = "sAMAccountName";

        /// <summary>
        /// Gets or sets the attribute name for emails
        /// </summary>
        public string LdapUserEmailAttr { get; set; } = "mail";

        /// <summary>
        /// Gets or sets the attribute name for group names
        /// </summary>
        public string LdapGroupNameAttr { get; set; } = "cn";
        
        /// <summary>
        /// Gets or sets the attribute name for group members
        /// </summary>
        public string LdapMember { get; set; } = "member";

        #endregion

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private AppSettings()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="baseUrl">Base URL of the site</param>
        public AppSettings(string baseUrl)
        {
            BaseUrl = baseUrl;
            AllowRegistration = true;
        }
    }
}