using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Services;
using PlasticNotifyCenter.Notifiers;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Manages the notification message history
    /// </summary>
    public interface INotificationHistoryManager
    {
        IEnumerable<NotificationStats> GetNotificationStats();

        /// <summary>
        /// Adds a new entry to the history of notifications
        /// </summary>
        /// <param name="dataType">Type of notifier data</param>
        Task<int> AddHistoryForTypeAsync(Type dataType);

        /// <summary>
        /// Sets the success and failed count for a history entry
        /// </summary>
        /// <param name="id">ID of history entry</param>
        /// <param name="successCount">Numver of successfull messages</param>
        /// <param name="failedCount">Numver of failed messages</param>
        Task SetHistoryCountsAsync(int id, int successCount, int failedCount);

        /// <summary>
        /// Returns a history entry by Id
        /// </summary>
        /// <param name="id">ID of the requested entry</param>
        NotificationHistory GetById(int id);
    }

    public class NotificationHistoryManager : INotificationHistoryManager
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;
        private readonly INotifierDefinitionService _notifierDefinitionService;

        public NotificationHistoryManager(PncDbContext dbContext,
                            INotifierDefinitionService notifierDefinitionService)
        {
            _dbContext = dbContext;
            _notifierDefinitionService = notifierDefinitionService;
        }

        #endregion

        #region Get stats

        public IEnumerable<NotificationStats> GetNotificationStats() =>
            _dbContext.NotificationHistory
                .GroupBy(o => o.NotifierName)
                .Select(g => new NotificationStats()
                {
                    Notifier = g.Key,
                    Icon = _notifierDefinitionService.GetIcon(g.Key),
                    SuccessCount = g.Sum(e => e.SuccessCount),
                    FailedCount = g.Sum(e => e.FailedCount)
                });


        #endregion

        #region Add to history

        /// <summary>
        /// Adds a new entry to the history of notifications
        /// </summary>
        /// <param name="history">History data</param>
        public async Task<int> AddHistoryForTypeAsync(Type dataType)
        {
            // Extract notifier name from data type
            var history = new NotificationHistory(
                    dataType
                        .GetCustomAttributes(false)
                        .Where(a => a is NotifierAttribute)
                        .Cast<NotifierAttribute>()
                        .Select(a => a.Name)
                        .SingleOrDefault());

            // Add new history entry
            await _dbContext.NotificationHistory.AddAsync(history);

            // save
            await _dbContext.SaveChangesAsync();

            return history.Id;
        }

        #endregion

        #region Set counts

        /// <summary>
        /// Sets the success and failed count for a history entry
        /// </summary>
        /// <param name="id">ID of history entry</param>
        /// <param name="successCount">Numver of successfull messages</param>
        /// <param name="failedCount">Numver of failed messages</param>
        public async Task SetHistoryCountsAsync(int id, int successCount, int failedCount)
        {
            var history = GetById(id);
            if (history == null)
            {
                throw new KeyNotFoundException("Notifier history entry not found");
            }

            // update counts
            history.SuccessCount = successCount;
            history.FailedCount = failedCount;

            // save
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Get by Id

        /// <summary>
        /// Returns a history entry by Id
        /// </summary>
        /// <param name="id">ID of the requested entry</param>
        public NotificationHistory GetById(int id) =>
            _dbContext.NotificationHistory.FirstOrDefault(entry => entry.Id == id);

        #endregion
    }
}