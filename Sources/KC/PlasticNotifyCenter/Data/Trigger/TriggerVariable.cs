using System;
using System.ComponentModel.DataAnnotations;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Stores information about an environment variable of a trigger call
    /// </summary>
    public class TriggerVariable
    {
        /// <summary>
        /// Gets a unique id in the entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the trigger type
        /// </summary>
        [Required]
        public string Trigger { get; set; }

        /// <summary>
        /// Gets the variable name
        /// </summary>
        [Required]
        public string Variable { get; set; }

        /// <summary>
        /// Gets the variable value
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private TriggerVariable()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="trigger">Name of the trigger</param>
        /// <param name="variable">Name or the variable</param>
        /// <param name="value">Value of the variable</param>
        public TriggerVariable(string trigger, string variable, string value)
        {
            this.Trigger = trigger;
            this.Variable = variable;
            this.Value = value;
        }
    }
}