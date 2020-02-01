using System.Linq;
using System.Text;
using PlasticNotifyCenter.Data;

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
        /// Gets or sets the type of the message body
        /// </summary>
        public MessageBodyType BodyType { get; set; }

        /// <summary>
        /// Gets or sets a list of tags attached to the message
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets a message with applied template
        /// </summary>
        public string TemplatedMessage { get; set; }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="type">Type of the message body</param>
        public Message(MessageBodyType type)
        {
            BodyType = type;
        }

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

        /// <summary>
        /// Applies a global template to the message
        /// </summary>
        /// <param name="template">Template</param>
        /// <param name="rulesUrl">URL to rules page</param>
        public void ApplyTemplate(string template, string rulesUrl) =>
            TemplatedMessage = template
                .Replace("%PNC_RULESURL%", rulesUrl)
                .Replace("%PNC_TITLE%", Title)
                .Replace("%PNC_BODY%", Body)
                .Replace("%PNC_TAGS%", string.Join(", ", Tags));
    }
}