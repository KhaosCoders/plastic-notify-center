using System.Collections.Generic;
using System.Linq;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Services;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Manages the notification message history
    /// </summary>
    public interface INotificationHistoryManager
    {
        IEnumerable<NotificationStats> GetNotificationStats();
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
    }
}