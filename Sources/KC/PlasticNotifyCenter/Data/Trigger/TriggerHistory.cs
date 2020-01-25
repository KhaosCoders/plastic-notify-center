using System;
using System.ComponentModel.DataAnnotations;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Stores meta-information about a trigger call
    /// </summary>
    public class TriggerHistory
    {
        /// <summary>
        /// Gets a unique id in the history
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the trigger type
        /// </summary>
        [Required]
        public string Trigger { get; set; }

        /// <summary>
        /// Gets the timestamp of the call
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets a input text send by Plastic SCM with the trigger
        /// </summary>
        public string Input { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <remarks>
        /// Used by EF
        /// </remarks>
        private TriggerHistory()
        { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="trigger">Trigger type</param>
        private TriggerHistory(string trigger, string input)
        {
            Trigger = trigger;
            Input = input;
            TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new instance of TriggerHistory for a TriggerCall
        /// </summary>
        /// <param name="call">The call to a trigger that should be preserved in history</param>
        /// <returns>A new TriggerHistory instance</returns>
        public static TriggerHistory From(TriggerCall call) =>
            new TriggerHistory(call.Type, string.Join(Environment.NewLine, call.Input));
    }
}