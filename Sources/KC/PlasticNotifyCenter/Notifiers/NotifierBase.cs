using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Data.Managers;

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

        private readonly INotificationHistoryManager _notificationHistoryManager;

        public NotifierBase(ILogger _logger, INotificationHistoryManager notificationHistoryManager)
        {
            Logger = _logger;
            _notificationHistoryManager = notificationHistoryManager;
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
            int historyId = await _notificationHistoryManager.AddHistoryForTypeAsync(typeof(T));

            // Start sending messages to each recipient
            var tasks = GetRecipientTasks(notiferData, message, recipients);

            // Should have tasks
            if (!(tasks?.Any() ?? false))
            {
                // Count all recipients as failed in the case of a general error
                await _notificationHistoryManager.SetHistoryCountsAsync(historyId, 0, recipients.Count());
                return;
            }

            // wait until all notifications have been send
            int successCount = 0;
            int failedCount = 0;
            await Task.Run(() =>
                tasks.AsParallel().ForAll(task =>
                {
                    try
                    {
                        // wait for successfull completion
                        task.Wait();
                        Interlocked.Increment(ref successCount);
                    }
                    catch (Exception)
                    {
                        // Should be logged in the notifier itself\
                        Interlocked.Increment(ref failedCount);
                    }
                }));

            // update the history entry with success stats
            await _notificationHistoryManager.SetHistoryCountsAsync(historyId, successCount, failedCount);
        }

        /// <summary>
        /// Starts a list of TPL tasks to send a notification message to each recipient
        /// </summary>
        protected abstract IEnumerable<Task> GetRecipientTasks(T data, Message message, IEnumerable<User> recipients);
    }
}