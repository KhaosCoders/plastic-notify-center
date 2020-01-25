using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// A base for notifiers
    /// </summary>
    public abstract class NotifierBase<T> : INotifier where T : BaseNotifierData
    {
        #region Dependencies

        /// <summary>
        /// Gets a logger for the notifier type
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets a database context
        /// </summary>
        protected PncDbContext DbContext { get; }

        public NotifierBase(ILogger _logger, PncDbContext _dbContext)
        {
            this.Logger = _logger;
            this.DbContext = _dbContext;
        }

        #endregion

        /// <summary>
        /// Send a notification message to each recipient
        /// </summary>
        /// <param name="data">Configuration data for the notifier</param>
        /// <param name="message">Message to send</param>
        /// <param name="recipients">List of recipients</param>
        public async Task SendMessageAsync(BaseNotifierData data, Message message, IEnumerable<User> recipients)
        {
            if (!typeof(T).IsInstanceOfType(data))
            {
                Logger.LogError("Internal error. Notifier called with wrong data package");
                return;
            }

            // Cast data
            T notiferData = (T)data;

            Logger.LogDebug("Sending message via notifier {}", notiferData.DisplayName);

            // New notification history entry
            var history = new NotificationHistory(typeof(T).GetCustomAttributes(false)
                                                            .Where(a => a is NotifierAttribute)
                                                            .Cast<NotifierAttribute>()
                                                            .Select(a => a.Name)
                                                            .SingleOrDefault());
            await DbContext.NotificationHistory.AddAsync(history);
            // save it
            await DbContext.SaveChangesAsync();

            // Start sending messages to each recipient
            var tasks = GetRecipientTasks(notiferData, message, recipients);
            if (tasks == null)
            {
                // Count all recipients as failed in the case of a general error
                history.FailedCount = recipients.Count();
                await DbContext.SaveChangesAsync();
                return;
            }

            // wait until all notifications have been send
            await Task.Run(() => tasks.ToList().ForEach(task =>
            {
                try
                {
                    task.Wait();
                    history.SuccessCount++;
                }
                catch (Exception)
                {
                    // Should be logged in the notifier itself\
                    history.FailedCount++;
                }
            }));

            // update the history entry with success stats
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Starts a list of TPL tasks to send a notification message to each recipient
        /// </summary>
        protected abstract IEnumerable<Task> GetRecipientTasks(T data, Message message, IEnumerable<User> recipients);
    }
}