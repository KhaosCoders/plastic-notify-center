using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Manages notification rules
    /// </summary>
    public interface INotificationRulesManager
    {
        /// <summary>
        /// Removes a user from all rules where he is a recipient
        /// </summary>
        /// <param name="user">User to remove</param>
        Task DeleteUserRecipientAsync(User user);

        /// <summary>
        /// Removes a role from all rules where it is a recipient
        /// </summary>
        /// <param name="role">Role to remove</param>
        Task DeleteRoleRecipientAsync(Role role);

        /// <summary>
        /// Returns the number or defined rules
        /// </summary>
        int GetRuleCount();

        /// <summary>
        /// Gets a sorted list of all rules
        /// </summary>
        IEnumerable<NotificationRule> Rules { get; }

        /// <summary>
        /// Returns a ordered list of rules owned by a user
        /// </summary>
        /// <param name="owner">Owner of rules</param>
        IEnumerable<NotificationRule> GetRulesOwnedBy(IdentityUser owner);

        /// <summary>
        /// Returns a list of rules triggerd by the provided type of trigger
        /// </summary>
        /// <param name="trigger">Name of trigger type</param>
        IQueryable<NotificationRule> GetRulesOwnedTiggerType(string trigger);

        /// <summary>
        /// Returns a rule by ID. Or null if no rule is found
        /// </summary>
        /// <param name="id">ID of requested rule</param>
        NotificationRule GetRuleById(string id);

        /// <summary>
        /// Returns a list of unassigned notifiers for a rule
        /// </summary>
        /// <param name="rule">Rule notifier are searched for</param>
        IEnumerable<BaseNotifierData> GetUnassignedNotifiers(NotificationRule rule);

        /// <summary>
        /// Returns a list of unassigned users for a rule
        /// </summary>
        /// <param name="rule">Rule users are searched for</param>
        IEnumerable<User> GetUnassignedUsers(NotificationRule rule);

        /// <summary>
        /// Returns a list of unassigned roles for a rule
        /// </summary>
        /// <param name="rule">Rule roles are searched for</param>
        IEnumerable<Role> GetUnassignedRoles(NotificationRule rule);

        /// <summary>
        /// Trys to find a rule by ID. If ID is empty a new rule is created
        /// </summary>
        /// <param name="id">ID or rule</param>
        /// <param name="owner">Owner of new rule</param>
        Task<NotificationRule> GetOrCreateRuleAsync(string id, User owner);

        /// <summary>
        /// Trys to update a rule. Creates a new rule if not ID is supplied
        /// </summary>
        /// <param name="rule">ID of rule to change</param>
        /// <param name="owner">Current user (owner of new rule)</param>
        /// <param name="name">Name of rule</param>
        /// <param name="trigger">Trigger type</param>
        /// <param name="filter">Advanced filter</param>
        /// <param name="title">Message title</param>
        /// <param name="message">Message body</param>
        /// <param name="bodyType">Type of message body</param>
        /// <param name="useTemplate">Whether the global template should be applied</param>
        /// <param name="tags">Message tags</param>
        /// <param name="notifiers">List of assigned notifiers</param>
        /// <param name="recipients">List of assigned recipients</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when rule is not found</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when rule can't be edited by user</exception>
        Task SaveOrCreateRuleAsync(
                string id,
                ClaimsPrincipal owner,
                string name,
                string trigger,
                string filter,
                string title,
                string message,
                MessageBodyType bodyType,
                bool useTemplate,
                string tags,
                string[] notifiers,
                string[] recipients);

        /// <summary>
        /// Deletes a rule
        /// </summary>
        /// <param name="id">ID of the rule to delete</param>
        /// <param name="owner">Current user</param>
        Task DeleteRuleAsync(string id, ClaimsPrincipal owner);

        /// <summary>
        /// Activates a rule
        /// </summary>
        /// <param name="id">ID of the rule to activate</param>
        /// <param name="owner">Current user</param>
        Task ActivateRuleAsync(string id, ClaimsPrincipal owner);

        /// <summary>
        /// Deactivates a rule
        /// </summary>
        /// <param name="id">ID of the rule to deactivate</param>
        /// <param name="owner">Current user</param>
        Task DeactivateRuleAsync(string id, ClaimsPrincipal owner);
    }

    /// <summary>
    /// Implementation of INotificationRulesManager
    /// </summary>
    public class NotificationRulesManager : INotificationRulesManager
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IAuthorizationService _authorizationService;

        public NotificationRulesManager(PncDbContext dbContext,
                                        UserManager<User> userManager,
                                        RoleManager<Role> roleManager,
                                        IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
        }

        #endregion

        #region Delete Recipients

        /// <summary>
        /// Removes a user from all rules where he is a recipient
        /// </summary>
        /// <param name="user">User to remove</param>
        public async Task DeleteUserRecipientAsync(User user)
        {
            // Find all roles where the user is a recipient
            var rules = _dbContext.Rules
                .ToList()
                .Select(rule => new { Rule = rule, Recipient = rule.Recipients.FirstOrDefault(recipient => recipient.User == user) })
                .Where(rule => rule.Recipient != null)
                .ToList();

            // First remove the user from recipient entry (because foreign-key-constraint)
            rules.ForEach(rule => rule.Recipient.User = null);
            await _dbContext.SaveChangesAsync();

            // Then remove the recipient entries
            rules.ForEach(rule => rule.Rule.Recipients.Remove(rule.Recipient));
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a role from all rules where it is a recipient
        /// </summary>
        /// <param name="role">Role to remove</param>
        public async Task DeleteRoleRecipientAsync(Role role)
        {
            // Find all roles where the user is a recipient
            var rules = _dbContext.Rules
                .ToList()
                .Select(rule => new { Rule = rule, Recipient = rule.Recipients.FirstOrDefault(recipient => recipient.Role == role) })
                .Where(rule => rule.Recipient != null)
                .ToList();

            // First remove the role from recipient entry (because foreign-key-constraint)
            rules.ForEach(rule => rule.Recipient.Role = null);
            await _dbContext.SaveChangesAsync();

            // Then remove the recipient entries
            rules.ForEach(rule => rule.Rule.Recipients.Remove(rule.Recipient));
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Rule count

        /// <summary>
        /// Returns the number or defined rules
        /// </summary>
        public int GetRuleCount() =>
            _dbContext.Rules.Count();

        #endregion

        #region Get rules

        /// <summary>
        /// Gets a sorted list of all rules
        /// </summary>
        public IEnumerable<NotificationRule> Rules =>
            _dbContext.Rules
                .Include(rule => rule.Owner)
                .Include(rule => rule.Notifiers).ThenInclude(notifier => notifier.Notifier)
                .OrderBy(rule => rule.DisplayName);

        /// <summary>
        /// Returns a ordered list of rules owned by a user
        /// </summary>
        /// <param name="owner">Owner of rules</param>
        public IEnumerable<NotificationRule> GetRulesOwnedBy(IdentityUser owner) =>
            Rules.Where(rule => rule.Owner == owner);

        /// <summary>
        /// Returns a rule by ID. Or null if no rule is found
        /// </summary>
        /// <param name="id">ID of requested rule</param>
        public NotificationRule GetRuleById(string id) =>
            _dbContext.Rules
                .Include(r => r.Notifiers).ThenInclude(notifier => notifier.Notifier)
                .Include(r => r.Recipients).ThenInclude(n => n.User)
                .Include(r => r.Recipients).ThenInclude(n => n.Role).ThenInclude(r => r.UserRoles).ThenInclude(ur => ur.User)
                .FirstOrDefault(r => r.Id.Equals(id));

        /// <summary>
        /// Returns a list of rules triggerd by the provided type of trigger
        /// </summary>
        /// <param name="trigger">Name of trigger type</param>
        public IQueryable<NotificationRule> GetRulesOwnedTiggerType(string trigger) =>
            _dbContext.Rules
                .Include(r => r.Notifiers).ThenInclude(notifier => notifier.Notifier)
                .Include(r => r.Recipients).ThenInclude(r => r.User)
                .Include(r => r.Recipients).ThenInclude(r => r.Role).ThenInclude(r => r.UserRoles).ThenInclude(ur => ur.User)
                .Where(r => r.Trigger.Equals(trigger) && r.IsActive);

        #endregion

        #region Unassigned notifiers / users / roles (groups)

        /// <summary>
        /// Returns a list of unassigned notifiers for a rule
        /// </summary>
        /// <param name="rule">Rule notifier are searched for</param>
        public IEnumerable<BaseNotifierData> GetUnassignedNotifiers(NotificationRule rule) =>
            _dbContext.Notifiers.ToList().Except(rule.Notifiers.Select(notifier => notifier.Notifier));


        /// <summary>
        /// Returns a list of unassigned users for a rule
        /// </summary>
        /// <param name="rule">Rule users are searched for</param>
        public IEnumerable<User> GetUnassignedUsers(NotificationRule rule) =>
            _userManager.Users.ToList().Except(rule.Recipients.Select(r => r.User).ToArray());


        /// <summary>
        /// Returns a list of unassigned roles for a rule
        /// </summary>
        /// <param name="rule">Rule roles are searched for</param>
        public IEnumerable<Role> GetUnassignedRoles(NotificationRule rule) =>
            _roleManager.Roles.ToList().Except(rule.Recipients.Select(r => r.Role).ToArray());

        #endregion

        #region New rule

        /// <summary>
        /// Trys to find a rule by ID. If ID is empty a new rule is created
        /// </summary>
        /// <param name="id">ID or rule</param>
        /// <param name="owner">Owner of new rule</param>
        public async Task<NotificationRule> GetOrCreateRuleAsync(string id, User owner)
        {
            NotificationRule rule;
            if (string.IsNullOrWhiteSpace(id))
            {
                // New rule
                rule = new NotificationRule(owner);
                rule.EnsureId();
                await _dbContext.Rules.AddAsync(rule);
            }
            else
            {
                // Find the rule
                rule = GetRuleById(id);
            }
            return rule;
        }

        #endregion

        #region Change rule

        /// <summary>
        /// Trys to update a rule. Creates a new rule if not ID is supplied
        /// </summary>
        /// <param name="rule">ID of rule to change</param>
        /// <param name="owner">Current user (owner of new rule)</param>
        /// <param name="name">Name of rule</param>
        /// <param name="trigger">Trigger type</param>
        /// <param name="filter">Advanced filter</param>
        /// <param name="title">Message title</param>
        /// <param name="message">Message body</param>
        /// <param name="bodyType">Type of message body</param>
        /// <param name="useTemplate">Whether the global template should be applied</param>
        /// <param name="tags">Message tags</param>
        /// <param name="notifiers">List of assigned notifiers</param>
        /// <param name="recipients">List of assigned recipients</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when rule is not found</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when rule can't be edited by user</exception>
        public async Task SaveOrCreateRuleAsync(
                string id,
                ClaimsPrincipal owner,
                string name,
                string trigger,
                string filter,
                string title,
                string message,
                MessageBodyType bodyType,
                bool useTemplate,
                string tags,
                string[] notifiers,
                string[] recipients)
        {
            User currentUser = await _userManager.GetUserAsync(owner);
            if (owner == null || currentUser == null)
            {
                throw new InvalidOperationException("Current user unknown");
            }

            NotificationRule rule = await GetOrCreateRuleAsync(id, currentUser);
            if (rule == null)
            {
                throw new KeyNotFoundException("Rule not found");
            }

            // Check authorization
            if (!(await owner.IsCoordinatorAsync(_authorizationService))
                && rule.Owner != currentUser)
            {
                // Not coordinator nor owner
                throw new InvalidOperationException("User is not owner of this rule");
            }

            // Apply changes
            rule.UpdateProperties(name,
                                  trigger,
                                  filter,
                                  title,
                                  message,
                                  bodyType,
                                  useTemplate,
                                  tags);

            // Change notifiers
            _dbContext.RuleNotifiers.RemoveRange(rule.Notifiers);
            rule.Notifiers.Clear();
            foreach (var notifier in _dbContext.Notifiers.Where(n => notifiers.Contains(n.Id)))
            {
                rule.Notifiers.Add(new RuleNotifier(notifier));
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
                        _dbContext.NotificationRecipients.Remove(curRecipient);
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
                        _dbContext.NotificationRecipients.Remove(curRecipient);
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
        }

        #endregion

        #region Delete rule

        /// <summary>
        /// Deletes a rule
        /// </summary>
        /// <param name="id">ID of the rule to delete</param>
        /// <param name="owner">Current user</param>
        public async Task DeleteRuleAsync(string id, ClaimsPrincipal owner)
        {
            // Find the rule
            NotificationRule rule = GetRuleById(id);
            if (rule == null)
            {
                throw new KeyNotFoundException("Rule not found");
            }

            // Check authorization
            if (!(await owner.IsCoordinatorAsync(_authorizationService))
                && rule.Owner != await _userManager.GetUserAsync(owner))
            {
                // Not coordinator nor owner
                throw new InvalidOperationException("User is not owner of this rule");
            }

            // First remove all recipients/notifiers (because of foreign-key-constraint)
            _dbContext.NotificationRecipients.RemoveRange(rule.Recipients);
            rule.Recipients.Clear();

            _dbContext.RuleNotifiers.RemoveRange(rule.Notifiers);
            rule.Notifiers.Clear();

            // Then remove the rule
            _dbContext.Rules.Remove(rule);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Activate / deactivate rule

        /// <summary>
        /// Activates a rule
        /// </summary>
        /// <param name="id">ID of the rule to activate</param>
        /// <param name="owner">Current user</param>
        public async Task ActivateRuleAsync(string id, ClaimsPrincipal owner) =>
            await SetRuleActivAsync(id, owner, true);

        /// <summary>
        /// Deactivates a rule
        /// </summary>
        /// <param name="id">ID of the rule to deactivate</param>
        /// <param name="owner">Current user</param>
        public async Task DeactivateRuleAsync(string id, ClaimsPrincipal owner) =>
            await SetRuleActivAsync(id, owner, false);

        /// <summary>
        /// Sets the active state of a rule
        /// </summary>
        /// <param name="id">ID of the rule to deactivate</param>
        /// <param name="owner">Current user</param>
        /// <param name="isActive">New active state</param>
        private async Task SetRuleActivAsync(string id, ClaimsPrincipal owner, bool isActive)
        {
            // Find the rule
            NotificationRule rule = GetRuleById(id);
            if (rule == null)
            {
                throw new KeyNotFoundException("Rule not found");
            }

            // Check authorization
            if (!(await owner.IsCoordinatorAsync(_authorizationService))
                && rule.Owner != await _userManager.GetUserAsync(owner))
            {
                // Not coordinator nor owner
                throw new InvalidOperationException("User is not owner of this rule");
            }

            // Deactivate rule
            rule.IsActive = isActive;

            // Save
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}