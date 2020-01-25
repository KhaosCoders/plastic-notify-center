using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Base class for notifier data models
    /// </summary>
    public abstract class BaseNotifierData
    {
        /// <summary>
        /// Gets or sets a unique Identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a name for the notifier
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a version string for the data
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="isEF">set this to true, if your constructor is private and only called by EF, for performance optimization</param>
        public BaseNotifierData(bool isEF)
        {
            if (!isEF)
            {
                Id = Guid.NewGuid().ToString();
                DisplayName = "Notifier";
                Version = "1.0";
            }
        }

        /// <summary>
        /// Parses a JSON string and applies the data to the models properties
        /// </summary>
        /// <param name="jsonData">JSON string with data</param>
        public abstract Task ApplyJsonPropertiesAsync(string jsonData);
    }
}