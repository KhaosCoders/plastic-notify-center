using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PlasticNotifyCenter.Authorization
{
    /// <summary>
    /// Extends ClaimsPrincipal with authorizaton validation
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Returns true when the user is a administrator
        /// </summary>
        /// <param name="user">ClaimsPrincipal instance</param>
        /// <param name="authorizationService">AuthorizationService to use for authentication</param>
        /// <returns></returns>
        public static async Task<bool> IsAdminAsync(this ClaimsPrincipal user, IAuthorizationService authorizationService) =>
            (await authorizationService.AuthorizeAsync(user, null, RoleRequirements.AdminRoleRequirement)).Succeeded;

    }
}