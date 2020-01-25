using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace PlasticNotifyCenter.Authorization
{
    /// <summary>
    /// Authorizes administrators
    /// </summary>
    public class AdministratorsAuthorizationHandler : AuthorizationHandler<AdminRoleRequirement>
    {
        protected override Task HandleRequirementAsync(
                                    AuthorizationHandlerContext context,
                                    AdminRoleRequirement requirement)
        {
            // User not logged in => not authorized
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Admins allowed
            if (context.User.IsInRole(Roles.AdminRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}