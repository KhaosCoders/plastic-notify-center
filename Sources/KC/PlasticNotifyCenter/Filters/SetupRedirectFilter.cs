using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using PlasticNotifyCenter.Data;

namespace PlasticNotifyCenter.Filters
{
    /// <summary>
    /// A filter that redirects the user to the setup page, if a controller is accessed which is not marked as accessable without compleded app setup
    /// </summary>
    public class SetupRedirectFilter : IAsyncAuthorizationFilter
    {
        #region Dependencies

        private readonly PncDbContext _dbContect;

        public SetupRedirectFilter(PncDbContext dbContect)
        {
            this._dbContect = dbContect;
        }

        #endregion

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            return Task.Run(() =>
            {
                if (context.ActionDescriptor is ControllerActionDescriptor controllerAction)
                {
                    // Not marked and no completed setup => redirect
                    if (!controllerAction.ControllerTypeInfo.CustomAttributes.Any(a => a.AttributeType == typeof(NoSetupAttribute))
                        && !_dbContect.AppSettings.Any())
                    {
                        context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary {
                                { "Controller", "Setup" },
                                { "Action", "Index" }
                            });
                    }
                }
            });
        }
    }
}