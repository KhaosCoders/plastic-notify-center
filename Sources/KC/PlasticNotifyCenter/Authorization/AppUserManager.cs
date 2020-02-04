using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Services;

namespace PlasticNotifyCenter.Authorization
{
    /// <summary>
    /// Adds LDAP login to the Core UserManager
    /// </summary>
    public class AppUserManager : UserManager<User>
    {
        private readonly ILdapService _ldapService;

        public AppUserManager(IUserStore<User> store, 
                            IOptions<IdentityOptions> optionsAccessor, 
                            IPasswordHasher<User> passwordHasher, 
                            IEnumerable<IUserValidator<User>> userValidators,
                            IEnumerable<IPasswordValidator<User>> passwordValidators, 
                            ILookupNormalizer keyNormalizer, 
                            IdentityErrorDescriber errors, 
                            IServiceProvider services, 
                            ILogger<UserManager<User>> logger,
                            ILdapService ldapService) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        { 
            _ldapService = ldapService;
        }

        // Override password check
        public override async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user.Origin == Origins.LDAP)
            {
                // Check LDAP login
                return await CheckLdapPasswordAsync(user, password);
            }
            // default login
            return await base.CheckPasswordAsync(user, password);
        }

        /// <summary>
        /// Checks a user/password combination against LDAP
        /// </summary>
        /// <param name="user">Name of user</param>
        /// <param name="password">Password</param>
        private async Task<bool> CheckLdapPasswordAsync(User user, string password)
        {
            return await Task.Run(() => _ldapService.CheckPassword(user.UserName, password));
        }
    }
}