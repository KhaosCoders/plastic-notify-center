using System.Threading.Tasks;
using PlasticNotifyCenter.Data;
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
        /// <param name="ldapConfig">LDAP configuration</param>
        string GetLdapStr(LdapSettings ldapConfig);

        /// <summary>
        /// Trzs to connect to LDAP and returns true, if possible
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        bool TestConnection(string ldapStr);

        /// <summary>
        /// Returns the number of users found
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        int GetUserCount(LdapSettings ldapConfig);

        /// <summary>
        /// Returns the number of groups found
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        int GetGroupCount(LdapSettings ldapConfig);

        /// <summary>
        /// Trys to get the provided attribues on the first found user entry
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        bool TestUserAttributes(LdapSettings ldapConfig);

        /// <summary>
        /// Trys to get the provided attribues on the first found group entry
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        bool TestGroupAttrbutes(LdapSettings ldapConfig);

        /// <summary>
        /// Gets all users found via LDAP
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        LdapUser[] GetUsers(LdapSettings ldapConfig);

        /// <summary>
        /// Gets all groups found via LDAP
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        LdapGroup[] GetGroups(LdapSettings ldapConfig);

        /// <summary>
        /// Validates a user/password combination via LDAP
        /// </summary>
        /// <param name="user">Name of user</param>
        /// <param name="password">Password</param>
        bool CheckPassword(string user, string password);
    }
}