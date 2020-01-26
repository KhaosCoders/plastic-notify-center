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
            while(!stoppingToken.IsCancellationRequested)
            {
                // create new db context
                using var scope = _serviceProvider.CreateScope();
                using PncDbContext dbContext = new PncDbContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<PncDbContext>>());

                // Get settings and start sync
                AppSettings config = dbContext.AppSettings.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(config?.LdapDcHost))
                {
                    try 
                    {
                        Sync(config);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Error while LDAP sync");
                    }
                }
                // wait 5 minutes. Then sync again
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private void Sync(AppSettings config)
        {
            // Get connection string
            string ldapStr = _ldapService.GetLdapStr(config.LdapDcSSL, config.LdapDcHost, config.LdapDcPort);

            // Get all users / groups
            LdapUser[] ldapUsers = _ldapService.GetUsers(ldapStr, config.LdapBaseDN, config.LdapUserDN, config.LdapUserFilter, config.LdapUserNameAttr, config.LdapUserEmailAttr);
            _logger.LogDebug("{0} users to sync", ldapUsers.Length);
            LdapGroup[] ldapGroups = _ldapService.GetGroups(ldapStr, config.LdapBaseDN, config.LdapGroupDN, config.LdapGroupFilter, config.LdapGroupNameAttr, config.LdapMember, config.LdapUserNameAttr);
            _logger.LogDebug("{0} groups to sync", ldapGroups.Length);

            // Sync users first
            SyncUsers(ldapUsers);

            // Sync groups then
            SyncGroups(ldapGroups);
        }

        private void SyncGroups(LdapGroup[] ldapGroups)
        {
            // create new db context
            using var scope = _serviceProvider.CreateScope();
            using PncDbContext dbContext = new PncDbContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<PncDbContext>>());
            using UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            using RoleManager<Role> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            var knownGroups = roleManager.Roles.Where(role => role.Origin == Origins.LDAP).ToList();

            // TODO: Actual sync
        }

        private void SyncUsers(LdapUser[] ldapUsers)
        {
            // create new db context
            using var scope = _serviceProvider.CreateScope();
            using PncDbContext dbContext = new PncDbContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<PncDbContext>>());
            using UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Lockout old users
            var knownUsers = userManager.Users.Where(u => u.Origin == Origins.LDAP).ToList();
            knownUsers.ForEach(user => {
                if (!ldapUsers.Any(lu => lu.UserName == user.UserName))
                {
                    _logger.LogInformation("User {0} is no longer part of the LDAP directory and is therefore locked out", user.UserName);
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.Now + TimeSpan.FromDays(365 * 200);
                    userManager.UpdateAsync(user);
                }
            });

            // Reactivate old users
            knownUsers.Where(u => u.LockoutEnd > DateTime.Now && ldapUsers.Any(lu => lu.UserName == u.UserName))
                .ToList()
                .ForEach(user =>{
                    _logger.LogInformation("User {0} is again part of the LDAP directory and is therefore no longer locked out", user.UserName);
                    user.LockoutEnabled = false;
                    user.LockoutEnd = DateTime.Now;
                    userManager.UpdateAsync(user);
                });

            // Create new users
            ldapUsers.Where(lu => !knownUsers.Any(u => lu.UserName == u.UserName))
                .ToList()
                .ForEach(ldapUser => {
                    _logger.LogInformation("Add new user for LDAP", ldapUser.UserName);
                    var adminUser = new User(ldapUser.UserName)
                    {
                        Email = ldapUser.Email,
                        EmailConfirmed = true,
                        Origin = Origins.LDAP
                    };
                    var result = userManager.CreateAsync(adminUser).Result;
                    if (!result.Succeeded)
                    {
                        _logger.LogWarning("Can't create new user {0} (Email: {1})", ldapUser.UserName, ldapUser.Email);
                    }
                });
        }
    }
}