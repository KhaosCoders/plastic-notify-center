using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Background service handling the notification queue requests, evaluating notification rules and sending notifications via notifiers
    /// </summary>
    public class NotifierService : BackgroundService
    {
        #region Dependencies

        private readonly ILogger<NotifierService> _logger;
        private readonly IRuleConditionEvaluator _conditionEval;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationQueue _notificationQueue;

        public NotifierService(ILogger<NotifierService> logger,
                                IRuleConditionEvaluator conditionEval,
                                IServiceProvider serviceProvider,
                                INotificationQueue notificationQueue)
        {
            this._logger = logger;
            this._conditionEval = conditionEval;
            this._serviceProvider = serviceProvider;
            this._notificationQueue = notificationQueue;
        }

        #endregion

        #region Background task

        /// <summary>
        /// The background task started by dotnet core framework
        /// </summary>
        /// <param name="stoppingToken">Token used when stopping the application</param>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (false == stoppingToken.IsCancellationRequested)
            {
                // Get next tigger call information
                var triggerCall = await _notificationQueue.DequeueAsync(stoppingToken);
                try
                {
                    // create new db context
                    using var scope = _serviceProvider.CreateScope();
                    using PncDbContext dbContext = new PncDbContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<PncDbContext>>());

                    // Create entry in history
                    await TriggerHistoryEntryAsync(triggerCall, dbContext);

                    // evaludate notification rules and send messages
                    await EvaluateNotificationRulesAsync(triggerCall, dbContext);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"Error occurred executing {nameof(triggerCall)}.");
                }
            }
        }

        #endregion

        #region Trigger history

        /// <summary>
        /// Saves information about the trigger call in the history database
        /// </summary>
        /// <param name="call">Trigger call information</param>
        /// <param name="dbContext">Database context</param>
        private async Task TriggerHistoryEntryAsync(TriggerCall call, PncDbContext dbContext)
        {
            _logger.LogInformation("Plastic called trigger: {trigger}", call.Type);

            // remove old values
            dbContext.TriggerVariables.RemoveRange(dbContext.TriggerVariables.Where(v => v.Trigger == call.Type));

            // prepare new values
            List<TriggerVariable> variables = new List<TriggerVariable>();
            foreach (var var in call.EnvironmentVars)
            {
                variables.Add(new TriggerVariable(call.Type, var.Key, var.Value));
            }

            // add new values
            await dbContext.TriggerHistory.AddAsync(TriggerHistory.From(call));
            await dbContext.TriggerVariables.AddRangeAsync(variables);

            // save
            await dbContext.SaveChangesAsync();
        }

        #endregion

        #region Notification rules

        /// <summary>
        ///  Finds all notification rules for a trigger call and sends the notifications
        /// </summary>
        /// <param name="call">Trigger call</param>
        /// <returns></returns>
        private async Task EvaluateNotificationRulesAsync(TriggerCall call, PncDbContext dbContext)
        {
            // Get all rules for the rigger
            var triggerRules = await dbContext.Rules
                                            .Include(r => r.Notifiers)
                                            .Include(r => r.Recipients).ThenInclude(r => r.User)
                                            .Include(r => r.Recipients).ThenInclude(r => r.Role)
                                            .Where(r => r.Trigger.Equals(call.Type) && r.IsActive).ToListAsync();

            // Apply filters
            var filteredRules = _conditionEval.EvalFilterAsync(triggerRules, call.EnvironmentVars, call.Input);

            // Send them notifications for each rule
            await foreach (var rule in filteredRules)
            {
                // Send notifications in parallel
                await RunRuleAsync(rule, call);
            }
        }

        /// <summary>
        /// Send the notifications for a rule
        /// </summary>z
        /// <param name="rule">Notification rule</param>
        /// <param name="call">Trigger call information</param>
        private async Task RunRuleAsync(NotificationRule rule, TriggerCall call)
        {
            // Validate notifiers prerequisite
            if (!rule.Notifiers.Any())
            {
                _logger.LogError("This rule has no notifiers: {ruleName}", rule.DisplayName);
                return;
            }

            // Log rule
            using var _ = _logger.BeginScope("Sending notifications for rule: {ruleName}", rule.DisplayName);

            // Prepare the message texts
            var message = await Task.Run(() => PrepareMessage(rule, call));
            if (string.IsNullOrWhiteSpace(message.Body))
            {
                _logger.LogError("The message body is empty for rule: {ruleName}", rule.DisplayName);
                return;
            }
            _logger.LogDebug("The message is: {message}", message.ToString());

            // Resolv groups and get all recipient users
            var recipients = await Task.Run(() => GetRecipientUsers(rule));
            _logger.LogDebug("Recipients are: {recipients}", string.Join(", ", recipients.Select(u => u.UserName)));

            // send message through each notifier
            rule.Notifiers.AsParallel().ForAll(async notifier => await SendMessageAsync(notifier, message, recipients));
        }

        /// <summary>
        /// Sends a message on a specific notifier
        /// </summary>
        /// <param name="notifier">The notifier to use to send the message</param>
        /// <param name="message">The message</param>
        /// <param name="recipients">The recipients of the message</param>
        private async Task SendMessageAsync(BaseNotifierData notifierData, Message message, IEnumerable<User> recipients)
        {
            // Create the correct notifier type
            using var scope = _serviceProvider.CreateScope();
            var notifier = GetNotifier(notifierData, scope.ServiceProvider);
            if (notifier == null)
            {
                _logger.LogError("No notifier found for data class: {class}", notifierData.GetType().FullName);
                return;
            }

            // Send message via notifier
            await notifier.SendMessageAsync(notifierData, message, recipients);
        }

        /// <summary>
        /// Tries to get the appropriate notifier service instance for the given notifier data
        /// </summary>
        /// <param name="notifierData">Notifier configuration data</param>
        private INotifier GetNotifier(BaseNotifierData notifierData, IServiceProvider serviceProvider) =>
            notifierData
                .GetType()
                .GetCustomAttributes(false)
                .Where(attr => attr is NotifierAttribute)
                .Cast<NotifierAttribute>()
                .Select(na => serviceProvider.GetService(na.NotifierType.GetInterfaces().First()))
                .Cast<INotifier>()
                .FirstOrDefault();

        /// <summary>
        /// Prepare the actual text message
        /// </summary>
        /// <param name="rule">Notification rule configuration data</param>
        /// <param name="call">Trigger call information</param>
        private Message PrepareMessage(NotificationRule rule, TriggerCall call) =>
            new Message()
            {
                // Replace placeholers in all fields of the message
                Title = TextHelper.ReplaceVars(rule.NotificationTitle ?? string.Empty, call.EnvironmentVars),
                Body = TextHelper.ReplaceVars(rule.NotificationBody ?? string.Empty, call.EnvironmentVars),
                Tags = TextHelper.ReplaceVars(rule.NotificationTags ?? string.Empty, call.EnvironmentVars)
                                 .Split(',')
                                 .Select(tag => tag.Trim())
                                 .ToArray()
            };

        /// <summary>
        /// Returns a list of all recipient users
        /// </summary>
        /// <param name="rule">Notification rule configuration data</param>
        private IEnumerable<User> GetRecipientUsers(NotificationRule rule)
        {
            // Get the UserManager service instance
            using var scope = _serviceProvider.CreateScope();
            using var userManager = (UserManager<User>)scope.ServiceProvider.GetService(typeof(UserManager<User>));

            // Get a list of all users
            var allUsers = userManager.Users.AsEnumerable();

            // Resolve notication recipients (users/groups) to a list of all affected users
            return rule.Recipients.SelectMany(recipient =>
                recipient.User != null
                    ? new User[] { recipient.User }
                    // Revolve user groups to a list of users in the group
                    : recipient.Role.IsDeleted
                        // If role (group) was deleted
                        ? null
                        // Resolve group
                        : allUsers
                            .Where(u => userManager.IsInRoleAsync(u, recipient.Role.Name).Result)
                            .ToArray())
                // Only active users
                .Where(user => user != null && !user.IsDeleted)
                // return a list of destinct users, even if they are in multiple included user groups
                .Distinct(new UserEqualityComparer())
                .ToArray();
        }

        private class UserEqualityComparer : IEqualityComparer<User>
        {
            public bool Equals([AllowNull] User x, [AllowNull] User y) =>
                x?.Id == y?.Id;

            public int GetHashCode([DisallowNull] User obj) =>
                obj.Id.GetHashCode();
        }

        #endregion
    }
}