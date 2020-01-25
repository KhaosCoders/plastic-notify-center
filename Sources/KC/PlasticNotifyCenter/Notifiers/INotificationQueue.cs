using System.Threading;
using System.Threading.Tasks;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// A queue of notifications to send asynchronously
    /// </summary>
    public interface INotificationQueue
    {
        /// <summary>
        /// Queues a new trigger call for notification rule processing
        /// </summary>
        /// <param name="call">Trigger call information</param>
        void QueueTriggerCall(TriggerCall call);

        /// <summary>
        /// Trys to get the next trigger call information from the queue
        /// Will wait for new calls if there are none right now
        /// </summary>
        /// <param name="cancellationToken">Used to cancle the action</param>
        Task<TriggerCall> DequeueAsync(CancellationToken cancellationToken);
    }
}