using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Services
{
    /// <summary>
    /// A service providing access to LDAP resources
    /// </summary>
    public interface ILdapService
    {
        /// <summary>
        /// Returns a LDAP connection string
        /// </summary>
        /// <param name="useSSL">If true, a secure channel is used</param>
        /// <param name="hostname">Host name of the domain controller</param>
        /// <param name="port">Port to use</param>
        string GetLdapStr(bool useSSL, string hostname, int port);

        /// <summary>
        /// Trzs to connect to LDAP and returns true, if possible
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        bool TestConnection(string ldapStr);

        /// <summary>
        /// Returns the number of users found
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="userDN">Additional user DN</param>
        /// <param name="filter">User filter</param>
        int GetUserCount(string ldapStr, string baseDN, string userDN, string filter);
        
        /// <summary>
        /// Returns the number of groups found
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="groupDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        int GetGroupCount(string ldapStr, string baseDN, string groupDN, string filter);

        /// <summary>
        /// Trys to get the provided attribues on the first found user entry
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="userDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        /// <param name="userNameAttr">Name of user name attribute</param>
        /// <param name="userEmailAttr">Name of email address attribute</param>
        bool TestUserAttributes(string ldapStr, string baseDN, string userDN, string filter, string userNameAttr, string userEmailAttr);

        /// <summary>
        /// Trys to get the provided attribues on the first found group entry
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="groupDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        /// <param name="groupNameAttr">Name of group name attribute</param>
        /// <param name="memberAttr">Name of group members attribute</param>
        bool TestGroupAttrbutes(string ldapStr, string baseDN, string groupDN, string filter, string groupNameAttr, string memberAttr);

        /// <summary>
        /// Gets all users found via LDAP
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="userDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        /// <param name="userNameAttr">Name of user name attribute</param>
        /// <param name="userEmailAttr">Name of email address attribute</param>
        LdapUser[] GetUsers(string ldapStr, string baseDN, string userDN, string filter, string userNameAttr, string userEmailAttr);
        
        /// <summary>
        /// Gets all groups found via LDAP
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="groupDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        /// <param name="groupNameAttr">Name of group name attribute</param>
        /// <param name="memberAttr">Name of group members attribute</param>
        /// <param name="userNameAttr">Name of user name attribute</param>
        LdapGroup[] GetGroups(string ldapStr, string baseDN, string groupDN, string filter, string groupNameAttr, string memberAttr, string userNameAttr);
    }
}