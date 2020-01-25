using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace PlasticNotifyCenter.Models
{
    /// <summary>
    /// Reprecents a call to a trigger
    /// </summary>
    public class TriggerCall
    {
        /// <summary>
        /// Gets the type of trigger
        /// </summary>
        /// <value></value>
        public string Type { get; }

        /// <summary>
        /// Gets a list of all provided environment variables
        /// </summary>
        public Dictionary<string, string> EnvironmentVars { get; private set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Gets the input content of the trigger (a file list for most triggers)
        /// </summary>
        public string[] Input { get; private set; }

        /// <summary>
        /// Creates a new instance of TriggerCall
        /// </summary>
        /// <param name="type">The type of trigger</param>
        private TriggerCall(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Parses the Json body of a trigger call to a TriggerCall model object
        /// </summary>
        /// <param name="data">Json data</param>
        /// <param name="type">Trigger type</param>
        /// <returns>Parsed model object</returns>
        public static TriggerCall ParseJson(string data, string type)
        {
            TriggerCall tc = new TriggerCall(type);

            var options = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };

            using (JsonDocument document = JsonDocument.Parse(data, options))
            {
                foreach (JsonProperty property in document.RootElement.EnumerateObject())
                {
                    if (property.Name.Equals("INPUT", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        int length = property.Value.GetArrayLength();
                        tc.Input = new string[length];

                        for (int i = 0; i < length; i++)
                        {
                            tc.Input[i] = property.Value[i].ToString();
                        }
                    }
                    else
                    {
                        tc.EnvironmentVars.Add(property.Name, property.Value.GetString());
                    }
                }
            }

            return tc;
        }
    }
}