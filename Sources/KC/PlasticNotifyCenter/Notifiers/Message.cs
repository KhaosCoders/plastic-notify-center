using System.Linq;
using System.Text;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Notification message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the title of the message
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the main message body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a list of tags attached to the message
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Combines all message fields to a single string
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder((Title?.Length ?? 0) + (Body?.Length ?? 0) + (Tags?.Length ?? 0) * 20 + 10);
            if (!string.IsNullOrWhiteSpace(Title))
            {
                sb.Append(Title).Append(System.Environment.NewLine);
            }
            if (!string.IsNullOrWhiteSpace(Body))
            {
                sb.Append(Body).Append(System.Environment.NewLine);
            }
            if (Tags?.Length > 0)
            {
                sb.Append(string.Join(", ", Tags.Select(tag => $"[{tag}]")));
            }
            return sb.ToString();
        }
    }
}