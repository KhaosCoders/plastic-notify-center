using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Data.Managers;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the /Rules section
    /// </summary>
    public class RulesController : Controller
    {
        #region Dependencies

        private readonly ILogger<RulesController> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly INotificationRulesManager _notificationRulesManager;
        private readonly ITriggerHistoryManager _triggerHistoryManager;

        public RulesController(ILogger<RulesController> logger,
                               PncDbContext dbContect,
                               IAuthorizationService authorizationService,
                               UserManager<User> userManager,
                               RoleManager<Role> roleManager,
                               INotificationRulesManager notificationRulesManager,
                               ITriggerHistoryManager triggerHistoryManager)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationRulesManager = notificationRulesManager;
            _triggerHistoryManager = triggerHistoryManager;
        }

        #endregion

        [HttpGet("/Rules")]
        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<NotificationRule> rules = null;

            // Show different subsets to different roles
            if (await User.IsCoordinatorAsync(_authorizationService))
            {
                // Coordinator (and Admin) may see all rules
                rules = _notificationRulesManager.Rules;
            }
            else
            {
                // Everybody else only sees their own rules
                IdentityUser user = await _userManager.GetUserAsync(User);
                rules = _notificationRulesManager.GetRulesOwnedBy(user);
            }

            return View(new RulesViewModel(rules.ToArray()));
        }

        [HttpGet("/Rules/New")]
        public IActionResult New()
        {
            // Show edit form with new templated rule
            return View("edit", CreateEditViewModel(new NotificationRule("New Rule")));
        }

        [HttpGet("/Rules/Edit/{id}")]
        public async Task<IActionResult> EditAsync([FromRoute] string id)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Find the rule
            NotificationRule rule = _notificationRulesManager.GetRuleById(id);
            if (rule == null)
            {
                return NotFound();
            }

            // Check authorization
            if (!(await User.IsCoordinatorAsync(_authorizationService))
                && rule.Owner != await _userManager.GetUserAsync(User))
            {
                // Not coordinator nor owner
                return Forbid();
            }

            // Show edit form
            return View(CreateEditViewModel(rule));
        }

        /// <summary>
        /// Prepares a view model for the edit form
        /// </summary>
        /// <param name="rule">Rule to edit</param>
        private EditRuleViewModel CreateEditViewModel(NotificationRule rule) =>
            new EditRuleViewModel(
                rule,
                _triggerHistoryManager.GetAllTriggerTypes().ToArray(),
                _notificationRulesManager.GetUnassignedNotifiers(rule).ToArray(),
                _notificationRulesManager.GetUnassignedUsers(rule).ToArray(),
                _notificationRulesManager.GetUnassignedRoles(rule).ToArray());

        [HttpPost("/Rules/Save")]
        public async Task<IActionResult> SaveAsync(
            [FromForm] string id,
            [FromForm] string name,
            [FromForm] string trigger,
            [FromForm] string filter,
            [FromForm] string title,
            [FromForm] string message,
            [FromForm] string tags,
            [FromForm] string[] notifiers,
            [FromForm] string[] recipients)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(trigger)
                || string.IsNullOrWhiteSpace(message))
            {
                return BadRequest();
            }

            try
            {
                // Try save or create rule
                await _notificationRulesManager.SaveOrCreateRuleAsync(
                    id,
                    User,
                    name,
                    trigger,
                    filter,
                    title,
                    message,
                    tags,
                    notifiers,
                    recipients
                );
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }
            catch (InvalidOperationException oex)
            {
                return Forbid(oex.Message);
            }

            // Return to rules view
            return RedirectToAction("Index", "Rules");
        }

        [HttpPost("/Rules/Delete")]
        public async Task<IActionResult> DeleteAsync([FromForm]string id)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            try
            {
                // Try to delete rule
                await _notificationRulesManager.DeleteRuleAsync(id, User);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }
            catch (InvalidOperationException oex)
            {
                Forbid(oex.Message);
            }

            // Return to rules view
            return RedirectToAction("Index", "Rules");
        }

        [HttpGet]
        public async Task<IActionResult> DeactivateAsync(string id)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            try
            {
                // Try deactivate rule
                await _notificationRulesManager.DeactivateRuleAsync(id, User);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }
            catch (InvalidOperationException oex)
            {
                return Forbid(oex.Message);
            }

            // Reload rules view
            return RedirectToAction("Index", "Rules");
        }

        [HttpGet]
        public async Task<IActionResult> ActivateAsync(string id)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            try
            {
                // Try activate rule
                await _notificationRulesManager.ActivateRuleAsync(id, User);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }
            catch (InvalidOperationException oex)
            {
                return Forbid(oex.Message);
            }

            // Reload rules view
            return RedirectToAction("Index", "Rules");
        }
    }
}