using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Data.Managers;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Services.Background
{
    public class LdapSyncService : BackgroundService
    {
        #region Dependencies

        private readonly ILogger<LdapSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILdapService _ldapService;

        public LdapSyncService(ILogger<LdapSyncService> logger,
                               IServiceProvider serviceProvider,
                               ILdapService ldapService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _ldapService = ldapService;
        }

        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            _logger.LogDebug("Starting LDAP sync");
            while (!stoppingToken.IsCancellationRequested)
            {
                // create new db context
                using var scope = _serviceProvider.CreateScope();
                IAppSettingsManager appSettingsManager = scope.ServiceProvider.GetRequiredService<IAppSettingsManager>();

                // Get settings and start sync
                LdapSettings ldapConfig = appSettingsManager.LdapConfig;

                if (!string.IsNullOrWhiteSpace(ldapConfig?.LdapDcHost))
                {
                    try
                    {
                        Sync(ldapConfig);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while LDAP sync");
                    }
                }
                // wait 5 minutes. Then sync again
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private void Sync(LdapSettings ldapConfig)
        {
            // Get all users / groups
            LdapUser[] ldapUsers = _ldapService.GetUsers(ldapConfig);
            _logger.LogDebug("{0} users to sync", ldapUsers.Length);
            LdapGroup[] ldapGroups = _ldapService.GetGroups(ldapConfig);
            _logger.LogDebug("{0} groups to sync", ldapGroups.Length);

            // Sync users first
            SyncUsers(ldapUsers);

            // Sync groups then
            SyncGroups(ldapGroups);

            // Sync group members at last
            SyncGroupMembers(ldapGroups);
        }

        /// <summary>
        /// Adds or removes users to and from groups according to LDAP information
        /// </summary>
        /// <param name="ldapGroups">LDAP groups information</param>
        private void SyncGroupMembers(LdapGroup[] ldapGroups)
        {
            // create new db context
            using var scope = _serviceProvider.CreateScope();
            using UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // all known users
            var allUsers = userManager.Users.ToList();

            // For each LDAP group
            ldapGroups.ToList()
                .ForEach(ldapGroup =>
                {
                    // All users in this LDAP group
                    var usersInGroup = allUsers
                        .Where(user => userManager.IsInRoleAsync(user, ldapGroup.Name).Result)
                        .ToArray();

                    // Remove old users
                    usersInGroup
                        // Find users in group, that don't belong to LDAP group
                        .Where(user => !ldapGroup.UserGuids.Contains(user.LdapGuid))
                        .ToList()
                        // Remove each user from the group (role)
                        .ForEach(async user =>
                        {
                            _logger.LogInformation("User {0} is no longer part of group {1}", user.UserName, ldapGroup.Name);
                            if (!(await userManager.RemoveFromRoleAsync(user, ldapGroup.Name)).Succeeded)
                            {
                                _logger.LogWarning("Can't remove user {0} from group {1}", user.UserName, ldapGroup.Name);
                            }
                        });

                    // Add new users to group
                    ldapGroup.UserGuids
                        // Find new users
                        .Where(ldapGuid => !usersInGroup.Any(user => user.LdapGuid == ldapGuid))
                        // Translate LdapGuid to User
                        .Join(allUsers, ldapGuid => ldapGuid, user => user.LdapGuid, (ldapGuid, user) => user)
                        .ToList()
                        // Add each new user to the group (role)
                        .ForEach(async user =>
                        {
                            _logger.LogInformation("User {0} is now part of group {1}", user.UserName, ldapGroup.Name);
                            if (!(await userManager.AddToRoleAsync(user, ldapGroup.Name)).Succeeded)
                            {
                                _logger.LogWarning("Can't add user {0} to group {1}", user.UserName, ldapGroup.Name);
                            }
                        });
                });
        }

        /// <summary>
        /// Creates new roles (groups), deactivates or reactivates them according to LDAP information
        /// </summary>
        /// <param name="ldapGroups">LDAP groups information</param>
        private void SyncGroups(LdapGroup[] ldapGroups)
        {
            // create new db context
            using var scope = _serviceProvider.CreateScope();
            using UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            using RoleManager<Role> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            var knownGroups = roleManager.Roles.Where(role => role.Origin == Origins.LDAP).ToArray();

            // Delete/deactivate old groups
            knownGroups
                .Where(group => !ldapGroups.Any(ldapGroup => ldapGroup.LdapGuid == group.LdapGuid))
                .ToList()
                .ForEach(async group =>
                {
                    _logger.LogInformation("Group {0} is no longer part of the LDAP directory and is therefore deactivated", group.Name);
                    await roleManager.DeactivateRoleAsync(group);
                });

            // Reactivate old groups
            knownGroups
                .Where(group => group.IsDeleted)
                .Select(user => new { Group = user, LdapGroup = ldapGroups.FirstOrDefault(ldapUser => ldapUser.LdapGuid == user.LdapGuid) })
                .Where(pair => pair.LdapGroup != null)
                .ToList()
                .ForEach(async pair =>
                {
                    _logger.LogInformation("Group {0} is again part of the LDAP directory and is therefore no longer deactivated", pair.Group.Name);
                    await roleManager.ReactivateRoleAsync(pair.Group, pair.LdapGroup);
                });

            // Create new groups
            ldapGroups
                .Where(ldapGroup => !knownGroups.Any(group => ldapGroup.LdapGuid == group.LdapGuid))
                .ToList()
                .ForEach(ldapGroup =>
                {
                    _logger.LogInformation("Add new group for LDAP {0} (Guid: {1})", ldapGroup.Name, ldapGroup.LdapGuid);
                    var role = Role.FromLDAP(ldapGroup);
                    var result = roleManager.CreateAsync(role).Result;
                    if (!result.Succeeded)
                    {
                        _logger.LogWarning("Can't create new role {0}", ldapGroup.Name);
                    }
                });
        }

        /// <summary>
        /// Creates new users, locks them out or reactivates them according to LDAP information
        /// </summary>
        /// <param name="ldapUsers">LDAP users information</param>
        private void SyncUsers(LdapUser[] ldapUsers)
        {
            // create new db context
            using var scope = _serviceProvider.CreateScope();
            using UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var knownUsers = userManager.Users.Where(user => user.Origin == Origins.LDAP).ToArray();

            // Lockout old users
            knownUsers
                .Where(user => !ldapUsers.Any(ldapUser => ldapUser.LdapGuid == user.LdapGuid))
                .ToList()
                .ForEach(async user =>
                {
                    _logger.LogInformation("User {0} is no longer part of the LDAP directory and is therefore locked out", user.UserName);
                    await userManager.DeactivateUserAsync(user);
                });

            // Reactivate old users
            knownUsers
                .Where(user => user.IsDeleted)
                .Select(user => new { User = user, LdapUser = ldapUsers.FirstOrDefault(ldapUser => ldapUser.LdapGuid == user.LdapGuid) })
                .Where(pair => pair.LdapUser != null)
                .ToList()
                .ForEach(async pair =>
                {
                    _logger.LogInformation("User {0} is again part of the LDAP directory and is therefore no longer locked out", pair.User.UserName);
                    await userManager.ReactivateUserAsync(pair.User, pair.LdapUser);
                });

            // Create new users
            ldapUsers
                .Where(ldapUser => !knownUsers.Any(user => ldapUser.LdapGuid == user.LdapGuid))
                .ToList()
                .ForEach(async ldapUser =>
                {
                    _logger.LogInformation("Add new user for LDAP {0} (Guid: {1})", ldapUser.UserName, ldapUser.LdapGuid);
                    if (!(await userManager.AddLdapUser(ldapUser)).Succeeded)
                    {
                        _logger.LogWarning("Can't create new user {0} (Email: {1}) or add him to the default 'users' group", ldapUser.UserName, ldapUser.Email);
                    }
                });
        }
    }
}