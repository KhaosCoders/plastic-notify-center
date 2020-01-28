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
        /// <remarks>
        /// Used by EF
        /// </remarks>
        protected BaseNotifierData()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="displayName">Display name of notifier</param>
        public BaseNotifierData(string displayName)
        {
            Id = Guid.NewGuid().ToString();
            DisplayName = displayName;
            Version = "1.0";
        }

        /// <summary>
        /// Parses a JSON string and applies the data to the models properties
        /// </summary>
        /// <param name="jsonData">JSON string with data</param>
        public abstract Task ApplyJsonPropertiesAsync(string jsonData);
    }
}