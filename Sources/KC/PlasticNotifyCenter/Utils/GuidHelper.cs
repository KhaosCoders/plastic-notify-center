using System;

namespace PlasticNotifyCenter.Utils
{
    /// <summary>
    /// Collection of helpers with GUIDs
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// Returns a new GUID as string
        /// </summary>
        public static string NewGuid() =>
            Guid.NewGuid().ToString();

        /// <summary>
        /// Unwraps GUID with {} wrap if needed
        /// </summary>
        /// <param name="guid">GUID</param>
        public static string Unwrap(string guid) =>
            string.IsNullOrWhiteSpace(guid) || !guid.StartsWith('{')
                ? guid
                : guid.Substring(1, guid.Length - 2);
    }
}