using System.Collections.Generic;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Helps with replacing placeholders in strings
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// Replaces placeholders in a text string with variable values
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="environmentVars">List of environment variables and their values</param>
        /// <param name="enclosementChar">Character used to enclose the variable values</param>
        /// <returns></returns>
        public static string ReplaceVars(string text, Dictionary<string, string> environmentVars, string enclosementChar = "")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                foreach (string var in environmentVars.Keys)
                {
                    text = text.Replace($"%{var}%", $"{enclosementChar}{environmentVars[var]}{enclosementChar}");
                }
            }
            return text;
        }
    }
}