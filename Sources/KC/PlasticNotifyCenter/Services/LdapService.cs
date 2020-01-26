using System.Linq;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Services
{
    public class LdapService : ILdapService
    {
        #region Dependencies

        private readonly ILogger<LdapService> _logger;

        public LdapService(ILogger<LdapService> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Get users / groups

        /// <summary>
        /// Gets all users found via LDAP
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="userDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        /// <param name="userNameAttr">Name of user name attribute</param>
        /// <param name="userEmailAttr">Name of email address attribute</param>
        public LdapUser[] GetUsers(string ldapStr, string baseDN, string userDN, string filter, string userNameAttr, string userEmailAttr)
        {
            using DirectoryEntry entry = Connect($"{ldapStr}/{userDN},{baseDN}");
            List<LdapUser> users = new List<LdapUser>();

            using DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = filter;

            foreach(SearchResult result in searcher.FindAll())
            {
                using DirectoryEntry userEntry = result?.GetDirectoryEntry();
                if (userEntry == null)
                {
                    continue;
                }

                string userName = userEntry.Properties[userNameAttr]?.Value?.ToString();
                string email = userEntry.Properties[userEmailAttr]?.Value?.ToString();

                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email))
                {
                    continue;
                }

                users.Add(new LdapUser(){
                    UserName = userName,
                    Email = email
                });
            }

            return users.ToArray();
        }

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
        public LdapGroup[] GetGroups(string ldapStr, string baseDN, string groupDN, string filter, string groupNameAttr, string memberAttr, string userNameAttr)
        {
            using DirectoryEntry entry = Connect($"{ldapStr}/{groupDN},{baseDN}");
            List<LdapGroup> groups = new List<LdapGroup>();

            using DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = filter;

            foreach(SearchResult result in searcher.FindAll())
            {
                using DirectoryEntry groupEntry = result?.GetDirectoryEntry();
                if (groupEntry == null)
                {
                    continue;
                }

                string groupName = groupEntry.Properties[groupNameAttr]?.Value?.ToString();
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    continue;
                }

                var group = new LdapGroup(){
                    Name = groupName
                };

                // Get all members
                object[] members = groupEntry.Properties[memberAttr]?.Value as object[];
                if (members == null)
                {
                    continue;
                }
                foreach(object memberObj in members)
                {
                    string userDN = memberObj.ToString();
                    if (string.IsNullOrWhiteSpace(userDN) || !userDN.Contains(baseDN))
                    {
                        continue;
                    }

                    using DirectoryEntry user = Connect($"{ldapStr}/{userDN}");
                    if (user == null)
                    {
                        continue;
                    }

                    string userName = user.Properties[userNameAttr]?.Value?.ToString();
                    if (string.IsNullOrWhiteSpace(userName))
                    {
                        continue;
                    }
                    
                    _logger.LogDebug("{0} is in {1}", userName, groupName);
                    group.Users.Add(userName);
                }

                groups.Add(group);
            }

            return groups.ToArray();
        }

        #endregion
        
        #region Connection

        /// <summary>
        /// Returns a LDAP connection string
        /// </summary>
        /// <param name="useSSL">If true, a secure channel is used</param>
        /// <param name="hostname">Host name of the domain controller</param>
        /// <param name="port">Port to use</param>
        public string GetLdapStr(bool useSSL, string hostname, int port) =>
            $"{(useSSL ? "LDAPS" : "LDAP")}://{hostname}{(port > 0 ? $":{port}" : "")}";

        /// <summary>
        /// Returns a base entry for the provided connection string
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        private DirectoryEntry Connect(string ldapStr)
        {
            _logger.LogDebug("Connect to LDAP: {0}", ldapStr);
            return new DirectoryEntry(ldapStr);
        }

        #endregion

        #region Config tests

        /// <summary>
        /// Trzs to connect to LDAP and returns true, if possible
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        public bool TestConnection(string ldapStr)
        {
            using DirectoryEntry entry = Connect(ldapStr);
            return DirectoryEntry.Exists(entry.Path);
        }

        /// <summary>
        /// Returns the number of users found
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="userDN">Additional user DN</param>
        /// <param name="filter">User filter</param>
        public int GetUserCount(string ldapStr, string baseDN, string userDN, string filter)
        {
            using DirectoryEntry entry = Connect($"{ldapStr}/{userDN},{baseDN}");
            return GetChildCount(entry, filter);
        }

        /// <summary>
        /// Returns the number of groups found
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="groupDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        public int GetGroupCount(string ldapStr, string baseDN, string groupDN, string filter)
        {
            using DirectoryEntry entry = Connect($"{ldapStr}/{groupDN},{baseDN}");
            return GetChildCount(entry, filter);
        }

        /// <summary>
        /// Returns the number of children for an entry
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="filter">Filter to apply</param>
        /// <returns></returns>
        private static int GetChildCount(DirectoryEntry entry, string filter)
        {
            using DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = filter;
            return searcher.FindAll().Count;
        }
        
        /// <summary>
        /// Trys to get the provided attribues on the first found user entry
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="userDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        /// <param name="userNameAttr">Name of user name attribute</param>
        /// <param name="userEmailAttr">Name of email address attribute</param>
        public bool TestUserAttributes(string ldapStr, string baseDN, string userDN, string filter, string userNameAttr, string userEmailAttr)
        {
            using DirectoryEntry entry = Connect($"{ldapStr}/{userDN},{baseDN}");
            return TestAttributes(entry, filter, userNameAttr, userEmailAttr);
        }

        /// <summary>
        /// Trys to get the provided attribues on the first found group entry
        /// </summary>
        /// <param name="ldapStr">LDAP connection string</param>
        /// <param name="baseDN">Base DN</param>
        /// <param name="groupDN">Additional group DN</param>
        /// <param name="filter">Group filter</param>
        /// <param name="groupNameAttr">Name of group name attribute</param>
        /// <param name="memberAttr">Name of group members attribute</param>
        public bool TestGroupAttrbutes(string ldapStr, string baseDN, string groupDN, string filter, string groupNameAttr, string memberAttr)
        {
            using DirectoryEntry entry = Connect($"{ldapStr}/{groupDN},{baseDN}");
            return TestAttributes(entry, filter, groupNameAttr, memberAttr);
        }

        /// <summary>
        /// Trys to find one child entry and checks the provided attributes for values
        /// </summary>
        /// <param name="entry">Root entry</param>
        /// <param name="filter">Filter to apply</param>
        /// <param name="attributes">List of attribute names</param>
        /// <returns></returns>
        private bool TestAttributes(DirectoryEntry entry, string filter, params string[] attributes)
        {
            using DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = filter;

            // Try to find one child
            SearchResult result = searcher.FindOne();
            if (result == null)
            {
                return false;
            }

            // Get the entry
            using DirectoryEntry user = result.GetDirectoryEntry();
            if (user == null)
            {
                return false;
            }

            // Try to get all the attributes
            foreach(string attr in attributes)
            {
                string value = user.Properties[attr].Value.ToString();
                _logger.LogDebug("Testing LDAP Attribute '{0}' value is: {1}", attr, value);
                if (string.IsNullOrWhiteSpace(value))
                {
                    // Value not found
                    return false;
                }
            }

            return true;
        }


        #endregion
    }
}