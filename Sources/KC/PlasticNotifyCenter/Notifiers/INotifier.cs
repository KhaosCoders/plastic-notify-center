using System.Collections.Generic;
using System.Threading.Tasks;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Notifier interface used to register notifiers as services
    /// This should ALWAYS be the first interface any service implements
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Send a notification message to each recipient
        /// </summary>
        /// <param name="data">Configuration data for the notifier</param>
        /// <param name="message">Message to send</param>
        /// <param name="recipients">List of recipients</param>
        Task SendMessageAsync(BaseNotifierData data, Message message, IEnumerable<User> recipients);
    }
}