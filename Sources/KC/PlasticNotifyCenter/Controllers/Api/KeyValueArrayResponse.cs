using System.Collections.Generic;

namespace PlasticNotifyCenter.Controllers.Api
{
    /// <summary>
    /// An array response with key value pairs
    /// </summary>
    public class KeyValueArrayResponse : SuccessResponse
    {
        /// <summary>
        /// Gets or sets the key values pairs
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> Values { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="pairs">key value pairs</param>
        public KeyValueArrayResponse(Dictionary<string, string> pairs)
        {
            this.Values = pairs;
        }
    }
}