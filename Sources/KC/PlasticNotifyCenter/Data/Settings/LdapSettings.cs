using System.Text;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Configuration for LDAP access
    /// </summary>
    public class LdapSettings
    {
        /// <summary>
        /// Gets or sets a unique Id
        /// </summary>
        public int Id { get; set; }

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
        public string LdapUserFilter { get; set; } = "(&(objectCategory=Person)(sAMAccountName=*))";

        /// <summary>
        /// Gets or sets a filter expression used to for filter only groups
        /// </summary>
        public string LdapGroupFilter { get; set; } = "(objectCategory=Group)";

        /// <summary>
        /// Gets or sets the attribute name for user GUIDs
        /// </summary>
        public string LdapUserGuidAttr { get; set; } = "objectGUID";

        /// <summary>
        /// Gets or sets the attribute name for user names
        /// </summary>
        public string LdapUserNameAttr { get; set; } = "sAMAccountName";

        /// <summary>
        /// Gets or sets the attribute name for emails
        /// </summary>
        public string LdapUserEmailAttr { get; set; } = "mail";

        /// <summary>
        /// Gets or sets the attribute name for group GUIDs
        /// </summary>
        public string LdapGroupGuidAttr { get; set; } = "objectGUID";

        /// <summary>
        /// Gets or sets the attribute name for group names
        /// </summary>
        public string LdapGroupNameAttr { get; set; } = "cn";

        /// <summary>
        /// Gets or sets the attribute name for group members
        /// </summary>
        public string LdapMember { get; set; } = "member";

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DcHost={0}; ", LdapDcHost);
            sb.AppendFormat("DcPort={0}; ", LdapDcPort);
            sb.AppendFormat("DcSSL={0}; ", LdapDcSSL);
            sb.AppendLine();
            sb.AppendFormat("BaseDN={0}; ", LdapBaseDN);
            sb.AppendFormat("UserDN={0}; ", LdapUserDN);
            sb.AppendFormat("GroupDN={0}; ", LdapGroupDN);
            sb.AppendLine();
            sb.AppendFormat("UserFilter={0}; ", LdapUserFilter);
            sb.AppendLine();
            sb.AppendFormat("GroupFilter={0}; ", LdapGroupFilter);
            sb.AppendLine();
            sb.AppendFormat("UserGuidAttr={0}; ", LdapUserGuidAttr);
            sb.AppendFormat("UserNameAttr={0}; ", LdapUserNameAttr);
            sb.AppendFormat("UserEmailAttr={0}; ", LdapUserEmailAttr);
            sb.AppendLine();
            sb.AppendFormat("GroupGuidAttr={0}; ", LdapGroupGuidAttr);
            sb.AppendFormat("GroupNameAttr={0}; ", LdapGroupNameAttr);
            sb.AppendLine();
            sb.AppendFormat("Member={0}; ", LdapMember);
            return sb.ToString();
        }
    }
}