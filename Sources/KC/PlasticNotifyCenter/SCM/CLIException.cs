using System;
using System.Runtime.Serialization;

namespace PlasticNotifyCenter.SCM
{
    /// <summary>
    /// A exception while CLI execution
    /// </summary>
    public class CLIException : Exception
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        public CLIException()
        {
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="message">Error message</param>
        public CLIException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Original exception</param>
        public CLIException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="info">Error message</param>
        /// <param name="context">Context</param>
        protected CLIException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}