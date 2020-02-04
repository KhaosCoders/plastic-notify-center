using System.Linq;
using System.DirectoryServices;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Data;
using System;
using System.DirectoryServices.AccountManagement;

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
        /// <param name="ldapConfig">LDAP configuration</param>
        public LdapUser[] GetUsers(LdapSettings ldapConfig)
        {
            List<LdapUser> users = new List<LdapUser>();

            // Connect to user dicectory via LDAP
            using DirectoryEntry entry = Connect($"{GetLdapStr(ldapConfig)}/{ldapConfig.LdapUserDN},{ldapConfig.LdapBaseDN}");

            // Apply search filter
            using DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = ldapConfig.LdapUserFilter;

            // Return all found users
            foreach (SearchResult result in searcher.FindAll())
            {
                using DirectoryEntry userEntry = result?.GetDirectoryEntry();
                if (userEntry == null)
                {
                    continue;
                }

                // Obtain attribute values
                string userName = userEntry.Properties[ldapConfig.LdapUserNameAttr]?.Value?.ToString();
                object guidData = userEntry.Properties[ldapConfig.LdapUserGuidAttr]?.Value;
                string email = userEntry.Properties[ldapConfig.LdapUserEmailAttr]?.Value?.ToString();


                // Check values for completeness
                if (guidData == null
                    || !(guidData is byte[])
                    || string.IsNullOrWhiteSpace(userName)
                    || string.IsNullOrWhiteSpace(email))
                {
                    continue;
                }

                string guid = (new Guid((byte[])guidData)).ToString();

                // Add new user to result list
                users.Add(new LdapUser(guid, userName, email));
            }

            return users.ToArray();
        }

        /// <summary>
        /// Gets all groups found via LDAP
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        public LdapGroup[] GetGroups(LdapSettings ldapConfig)
        {
            List<LdapGroup> groups = new List<LdapGroup>();

            // Connect to group directory via LDAP
            string ldapStr = GetLdapStr(ldapConfig);
            using DirectoryEntry entry = Connect($"{ldapStr}/{ldapConfig.LdapGroupDN},{ldapConfig.LdapBaseDN}");

            // Apply search filter
            using DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = ldapConfig.LdapGroupFilter;

            // Return all found groups
            foreach (SearchResult result in searcher.FindAll())
            {
                using DirectoryEntry groupEntry = result?.GetDirectoryEntry();
                if (groupEntry == null)
                {
                    continue;
                }

                // Obtain attributes
                object guidData = groupEntry.Properties[ldapConfig.LdapGroupGuidAttr]?.Value;
                string groupName = groupEntry.Properties[ldapConfig.LdapGroupNameAttr]?.Value?.ToString();

                if (guidData == null
                    || !(guidData is byte[])
                    || string.IsNullOrWhiteSpace(groupName))
                {
                    continue;
                }

                string guid = (new Guid((byte[])guidData)).ToString();
                var group = new LdapGroup(guid, groupName);

                // Get all members
                object[] members = groupEntry.Properties[ldapConfig.LdapMember]?.Value as object[];
                if (members == null)
                {
                    continue;
                }
                // Add all found member to group
                foreach (object memberObj in members)
                {
                    string userDN = memberObj.ToString();
                    if (string.IsNullOrWhiteSpace(userDN) || !userDN.Contains(ldapConfig.LdapBaseDN, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Skip users not part of the LDAP service
                        continue;
                    }

                    // Get user entry
                    using DirectoryEntry user = Connect($"{ldapStr}/{userDN}");
                    if (user == null)
                    {
                        continue;
                    }
                    
                    object userGuidData = user.Properties[ldapConfig.LdapUserGuidAttr]?.Value;
                    string userName = user.Properties[ldapConfig.LdapUserNameAttr]?.Value?.ToString();
                    if (userGuidData == null
                        || !(userGuidData is byte[])
                        || string.IsNullOrWhiteSpace(userName))
                    {
                        // Skip users without GUID or name
                        continue;
                    }

                    string userGuid = (new Guid((byte[])userGuidData)).ToString();

                    _logger.LogDebug("{0} (Guid: {1}) is in group: {2}", userName, userGuid, groupName);
                    group.UserGuids.Add(userGuid);
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
        /// <param name="ldapConfig">LDAP configuration</param>
        public string GetLdapStr(LdapSettings ldapConfig) =>
            $"{(ldapConfig.LdapDcSSL ? "LDAPS" : "LDAP")}://{ldapConfig.LdapDcHost}{(ldapConfig.LdapDcPort > 0 ? $":{ldapConfig.LdapDcPort}" : "")}";

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
        /// <param name="ldapConfig">LDAP configuration</param>
        public int GetUserCount(LdapSettings ldapConfig)
        {
            using DirectoryEntry entry = Connect($"{GetLdapStr(ldapConfig)}/{ldapConfig.LdapUserDN},{ldapConfig.LdapBaseDN}");
            return GetChildCount(entry, ldapConfig.LdapUserFilter);
        }

        /// <summary>
        /// Returns the number of groups found
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        public int GetGroupCount(LdapSettings ldapConfig)
        {
            using DirectoryEntry entry = Connect($"{GetLdapStr(ldapConfig)}/{ldapConfig.LdapGroupDN},{ldapConfig.LdapBaseDN}");
            return GetChildCount(entry, ldapConfig.LdapGroupFilter);
        }

        /// <summary>
        /// Returns the number of children for an entry
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="filter">Filter to apply</param>
        private static int GetChildCount(DirectoryEntry entry, string filter)
        {
            using DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = filter;
            return searcher.FindAll().Count;
        }

        /// <summary>
        /// Trys to get the provided attribues on the first found user entry
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        public bool TestUserAttributes(LdapSettings ldapConfig)
        {
            using DirectoryEntry entry = Connect($"{GetLdapStr(ldapConfig)}/{ldapConfig.LdapUserDN},{ldapConfig.LdapBaseDN}");
            return TestAttributes(entry, ldapConfig.LdapUserFilter,
                                         ldapConfig.LdapUserGuidAttr,
                                         ldapConfig.LdapUserNameAttr,
                                         ldapConfig.LdapUserEmailAttr);
        }

        /// <summary>
        /// Trys to get the provided attribues on the first found group entry
        /// </summary>
        /// <param name="ldapConfig">LDAP configuration</param>
        public bool TestGroupAttrbutes(LdapSettings ldapConfig)
        {
            using DirectoryEntry entry = Connect($"{GetLdapStr(ldapConfig)}/{ldapConfig.LdapGroupDN},{ldapConfig.LdapBaseDN}");
            return TestAttributes(entry, ldapConfig.LdapGroupFilter, ldapConfig.LdapGroupGuidAttr, ldapConfig.LdapGroupNameAttr, ldapConfig.LdapMember);
        }

        /// <summary>
        /// Trys to find one child entry and checks the provided attributes for values
        /// </summary>
        /// <param name="entry">Root entry</param>
        /// <param name="filter">Filter to apply</param>
        /// <param name="attributes">List of attribute names</param>
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
            foreach (string attr in attributes)
            {
                string value = user.Properties[attr]?.Value?.ToString();
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

        #region Password check

        
        /// <summary>
        /// Validates a user/password combination via LDAP
        /// </summary>
        /// <param name="user">Name of user</param>
        /// <param name="password">Password</param>
        public bool CheckPassword(string user, string password)
        {
            using PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            return pc.ValidateCredentials(user, password);
        }

        #endregion
    }
}