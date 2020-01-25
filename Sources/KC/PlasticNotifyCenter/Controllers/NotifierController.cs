using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Controllers.Api;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the /Notifier section
    /// </summary>
    public class NotifierController : Controller
    {
        #region Dependencies

        private readonly ILogger<NotifierController> _logger;
        private readonly PncDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifierDefinitionService _notifierDefinitionService;

        public NotifierController(ILogger<NotifierController> logger,
                                  PncDbContext dbContect,
                                  IAuthorizationService authorizationService,
                                  INotifierDefinitionService notifierIconService)
        {
            _logger = logger;
            _dbContext = dbContect;
            _authorizationService = authorizationService;
            _notifierDefinitionService = notifierIconService;
        }

        #endregion

        public async Task<IActionResult> IndexAsync(string id)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            NotifierViewModel model = new NotifierViewModel();

            // All supported types of notifiers
            model.NotifierTypes = _notifierDefinitionService.GetAllNotifierTypes();

            // List all created notifiers
            model.Notifiers = _dbContext.Notifiers.ToList()
                                .Select(n => new NotifierInfo()
                                {
                                    Id = n.Id,
                                    Name = n.DisplayName,
                                    Icon = _notifierDefinitionService.GetIcon(n.GetType())
                                })
                                .ToArray();

            if (!string.IsNullOrWhiteSpace(id))
            {
                // Select a specific notifier
                var notifier = _dbContext.Notifiers.FirstOrDefault(n => n.Id.Equals(id));
                if (notifier == null)
                {
                    return NotFound();
                }
                model.SelectedNotifier = notifier;
            }
            else if (model.Notifiers.Length > 0)
            {
                // Select the first notfier (or none)
                model.SelectedNotifier = _dbContext.Notifiers.FirstOrDefault();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(string id)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // Read POST body
            using (var reader = new StreamReader(Request.Body))
            {
                string body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }

                // Find notifier by Id
                BaseNotifierData notifier = _dbContext.Notifiers.FirstOrDefault(n => n.Id.Equals(id));
                if (notifier == null)
                {
                    return Ok(new FailureResponse("Notifier not found"));
                }

                // Apply changes
                await notifier.ApplyJsonPropertiesAsync(body);
            }

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Return success response
            return Ok(new SuccessResponse());
        }

        [HttpPost]
        public async Task<IActionResult> NewAsync([FromForm] string typedId)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // Check parameter
            if (string.IsNullOrWhiteSpace(typedId))
            {
                return BadRequest();
            }

            // Find the type of notifier data
            Type notifierDataType = _notifierDefinitionService.GetNotifierDataType(typedId);
            if (notifierDataType == null)
            {
                return NotFound();
            }

            // Create a new instance of the notifier data type
            BaseNotifierData notifier = Activator.CreateInstance(notifierDataType, new object[] { false }) as BaseNotifierData;
            if (notifier == null || string.IsNullOrWhiteSpace(notifier.Id))
            {
                return NotFound();
            }

            // Get name of notifier type
            string notifierName = _notifierDefinitionService.GetNotifierTypeName(typedId) ?? "Notifier";

            // Prefill display name
            notifier.DisplayName = $"New {notifierName}";

            // Add notifier
            await _dbContext.Notifiers.AddAsync(notifier);

            // save
            await _dbContext.SaveChangesAsync();

            // Redirect to notifiers view (with new notifier selected)
            return new RedirectToActionResult("Index", "Notifier", new { @Id = notifier.Id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Find notifier by Id
            BaseNotifierData notifier = _dbContext.Notifiers.FirstOrDefault(n => n.Id.Equals(id));
            if (notifier == null)
            {
                return NotFound();
            }

            // Remove notifier
            _dbContext.Notifiers.Remove(notifier);

            // Save
            await _dbContext.SaveChangesAsync();

            // Redirect to notifiers view
            return RedirectToAction("Index");
        }
    }
}