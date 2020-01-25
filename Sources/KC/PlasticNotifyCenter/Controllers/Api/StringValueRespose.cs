namespace PlasticNotifyCenter.Controllers.Api
{
    /// <summary>
    /// A return message carrying a string
    /// </summary>
    public class StringValueRespose : StateResposeBase
    {
        /// <summary>
        /// Gets or sets the string value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="value">return value</param>
        public StringValueRespose(string value)
            : base(State.Ok)
        {
            Value = value;
        }
    }
}