using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace PlasticNotifyCenter.Authorization
{
    /// <summary>
    /// Authorizes coordinators
    /// </summary>
    public class CoordinatorsAuthorizationHandler :
        AuthorizationHandler<CoordinatorRoleRequirement>
    {
        protected override Task HandleRequirementAsync(
                                    AuthorizationHandlerContext context,
                                    CoordinatorRoleRequirement requirement)
        {
            // User not logged in => not authorized
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Coordinators and Admins allowed
            if (context.User.IsInRole(Roles.CoordinatorRole) || context.User.IsInRole(Roles.AdminRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}