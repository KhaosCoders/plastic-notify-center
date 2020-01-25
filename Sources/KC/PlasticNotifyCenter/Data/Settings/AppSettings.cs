using System;
using System.ComponentModel.DataAnnotations;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Application settings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets a unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the URL the site is reachable from
        /// </summary>
        [Required]
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets whether users can register themselves
        /// </summary>
        [Required]
        public bool AllowRegistration { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private AppSettings()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="baseUrl">Base URL of the site</param>
        public AppSettings(string baseUrl)
        {
            BaseUrl = baseUrl;
            AllowRegistration = true;
        }
    }
}