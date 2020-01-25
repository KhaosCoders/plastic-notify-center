using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Implementation of the notification queue
    /// </summary>
    public class NotificationQueue : INotificationQueue
    {
        // internal queue
        private readonly ConcurrentQueue<TriggerCall> _calls =
            new ConcurrentQueue<TriggerCall>();

        // singal used as wait-lock for new entries
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// Queues a trigger call
        /// </summary>
        /// <param name="call">Trigger call information</param>
        public void QueueTriggerCall(TriggerCall call)
        {
            if (call == null)
            {
                throw new ArgumentNullException(nameof(call));
            }

            // Enqueue
            _calls.Enqueue(call);
            // Send signal to waiting dequeue
            _signal.Release();
        }

        /// <summary>
        /// Trys to get the next trigger call from the queue.
        /// Will wait for new calls if there are none right now
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TriggerCall> DequeueAsync(CancellationToken cancellationToken)
        {
            // Wait for a entry
            await _signal.WaitAsync(cancellationToken);

            // dequeue the next entry
            _calls.TryDequeue(out var workItem);
            return workItem;
        }
    }
}