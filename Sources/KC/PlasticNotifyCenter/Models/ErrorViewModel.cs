namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Error page view model
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the ASP.NET request Id
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Returns true if a RequestId is known
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
