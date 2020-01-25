using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the /Rules section
    /// </summary>
    public class RulesController : Controller
    {
        #region Dependencies

        private readonly ILogger<RulesController> _logger;
        private readonly PncDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RulesController(ILogger<RulesController> logger,
                               PncDbContext dbContect,
                               IAuthorizationService authorizationService,
                               UserManager<User> userManager,
                               RoleManager<Role> roleManager)
        {
            _logger = logger;
            _dbContext = dbContect;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string id)
        {
            IEnumerable<NotificationRule> rules = null;

            // Show different subsets to different roles
            if ((await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.CoordinatorRoleRequirement)).Succeeded)
            {
                // Coordinator (and Admin) may see all rules
                rules = _dbContext.Rules
                                    .Include(rule => rule.Owner)
                                    .Include(rule => rule.Notifiers);
            }
            else
            {
                // Everybody else only sees their own rules
                IdentityUser user = await _userManager.GetUserAsync(User);
                rules = _dbContext.Rules
                                    .Include(rule => rule.Owner)
                                    .Include(rule => rule.Notifiers)
                                    .Where(rule => rule.Owner == user);
            }

            return View(new RulesViewModel(rules.ToArray()));
        }

        [HttpGet]
        public IActionResult New()
        {
            // Show edit form with new templated rule
            return View("edit", CreateEditViewModel(new NotificationRule("New Rule")));
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(string id)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Find the rule
            NotificationRule rule = _dbContext.Rules
                                    .Include(r => r.Notifiers)
                                    .Include(r => r.Recipients).ThenInclude(n => n.User)
                                    .Include(r => r.Recipients).ThenInclude(n => n.Role)
                                    .FirstOrDefault(r => r.Id.Equals(id));
            if (rule == null)
            {
                return NotFound();
            }

            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.CoordinatorRoleRequirement)).Succeeded
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
        /// <param name="rule"></param>
        /// <returns></returns>
        private EditRuleViewModel CreateEditViewModel(NotificationRule rule) =>
            new EditRuleViewModel(rule, GetAllTriggers(), GetUnusedNotifiers(rule), GetUnassignedUsers(rule), GetUnassignedRoles(rule));

        /// <summary>
        /// Returns all trigger types
        /// </summary>
        private string[] GetAllTriggers() =>
            _dbContext.TriggerHistory.GroupBy(r => r.Trigger).Select(g => g.Key).ToArray();

        /// <summary>
        /// Returns all unused notifiers
        /// </summary>
        /// <param name="rule">Rule</param>
        private BaseNotifierData[] GetUnusedNotifiers(NotificationRule rule) =>
            _dbContext.Notifiers.ToList().Except(rule.Notifiers).ToArray();

        /// <summary>
        /// Returns all unassigned users
        /// </summary>
        /// <param name="rule">Rule</param>
        private User[] GetUnassignedUsers(NotificationRule rule) =>
            _userManager.Users.ToList().Except(rule.Recipients.Select(r => r.User).ToArray()).ToArray();

        /// <summary>
        /// Returns all unassigned Roles
        /// </summary>
        /// <param name="rule">Rule</param>
        private Role[] GetUnassignedRoles(NotificationRule rule) =>
            _roleManager.Roles.ToList().Except(rule.Recipients.Select(r => r.Role).ToArray()).ToArray();

        [HttpPost]
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

            // Get current user
            var currentUser = await _userManager.GetUserAsync(User);

            NotificationRule rule;
            if (string.IsNullOrWhiteSpace(id))
            {
                // New rule
                rule = new NotificationRule(currentUser);
                rule.EnsureId();
                await _dbContext.Rules.AddAsync(rule);
            }
            else
            {
                // Find the rule
                rule = _dbContext.Rules
                                    .Include(r => r.Notifiers)
                                    .Include(r => r.Recipients).ThenInclude(n => n.User)
                                    .Include(r => r.Recipients).ThenInclude(n => n.Role)
                                    .FirstOrDefault(r => r.Id.Equals(id));
            }

            if (rule == null)
            {
                return NotFound();
            }

            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.CoordinatorRoleRequirement)).Succeeded
                && rule.Owner != currentUser)
            {
                // Not coordinator nor owner
                return Forbid();
            }

            // Apply changes
            rule.DisplayName = name;
            rule.Trigger = trigger;
            rule.AdvancedFilter = filter;
            rule.NotificationTitle = title;
            rule.NotificationBody = message;
            rule.NotificationTags = tags;

            // Change notifiers
            rule.Notifiers.Clear();
            foreach (var notifier in _dbContext.Notifiers.Where(n => notifiers.Contains(n.Id)))
            {
                rule.Notifiers.Add(notifier);
            }

            // Change recipients
            List<string> addRecipients = new List<string>(recipients);

            // Remove recipients
            foreach (var curRecipient in rule.Recipients.ToList())
            {
                if (curRecipient.User != null)
                {
                    if (!recipients.Contains("U_" + curRecipient.User.Id))
                    {
                        rule.Recipients.Remove(curRecipient);
                    }
                    else
                    {
                        addRecipients.Remove("U_" + curRecipient.User.Id);
                    }
                }
                else
                {
                    if (!recipients.Contains("G_" + curRecipient.Role.Id))
                    {
                        rule.Recipients.Remove(curRecipient);
                    }
                    else
                    {
                        addRecipients.Remove("G_" + curRecipient.Role.Id);
                    }
                }
            }

            // Add recipients
            foreach (var addRecipient in addRecipients)
            {
                if (addRecipient.StartsWith("U"))
                {
                    var user = await _userManager.FindByIdAsync(addRecipient.Substring(2));
                    if (user != null)
                    {
                        rule.Recipients.Add(new NotificationRecipient(user));
                    }
                }
                else
                {
                    var role = await _roleManager.FindByIdAsync(addRecipient.Substring(2));
                    if (role != null)
                    {
                        rule.Recipients.Add(new NotificationRecipient(role));
                    }
                }
            }

            // Save
            await _dbContext.SaveChangesAsync();

            // Return to rules view
            return RedirectToAction("Index", "Rules");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync([FromForm]string id)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Find the rule
            NotificationRule rule = _dbContext.Rules
                                    .Include(r => r.Notifiers)
                                    .Include(r => r.Recipients).ThenInclude(r => r.User)
                                    .Include(r => r.Recipients).ThenInclude(r => r.Role)
                                    .FirstOrDefault(r => r.Id.Equals(id));
            if (rule == null)
            {
                return NotFound();
            }

            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.CoordinatorRoleRequirement)).Succeeded
                && rule.Owner != await _userManager.GetUserAsync(User))
            {
                // Not coordinator nor owner
                return Forbid();
            }

            // First remove all recipients/notifiers (because of foreign-key-constraint)
            foreach (var recipient in rule.Recipients)
            {
                recipient.User = null;
                recipient.Role = null;
            }
            await _dbContext.SaveChangesAsync();

            rule.Recipients.Clear();
            rule.Notifiers.Clear();
            rule.Owner = null;
            await _dbContext.SaveChangesAsync();

            // Then remove the rule
            _dbContext.Rules.Remove(rule);
            await _dbContext.SaveChangesAsync();

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

            // Find the rule
            NotificationRule rule = _dbContext.Rules.FirstOrDefault(r => r.Id.Equals(id));
            if (rule == null)
            {
                return NotFound();
            }

            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.CoordinatorRoleRequirement)).Succeeded
                && rule.Owner != await _userManager.GetUserAsync(User))
            {
                // Not coordinator nor owner
                return Forbid();
            }

            // Deactivate rule
            rule.IsActive = false;

            // Save
            await _dbContext.SaveChangesAsync();

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

            // Find the rule
            NotificationRule rule = _dbContext.Rules.FirstOrDefault(r => r.Id.Equals(id));
            if (rule == null)
            {
                return NotFound();
            }

            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.CoordinatorRoleRequirement)).Succeeded
                && rule.Owner != await _userManager.GetUserAsync(User))
            {
                // Not coordinator nor owner
                return Forbid();
            }

            // Deactivate rule
            rule.IsActive = true;

            // Save
            await _dbContext.SaveChangesAsync();

            // Reload rules view
            return RedirectToAction("Index", "Rules");
        }
    }
}