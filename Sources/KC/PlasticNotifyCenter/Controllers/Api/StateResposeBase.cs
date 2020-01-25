namespace PlasticNotifyCenter.Controllers.Api
{
    /// <summary>
    /// Base class for statefull responses
    /// </summary>
    public abstract class StateResposeBase
    {
        /// <summary>
        /// Gets or sets the returned state
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// Gets or sets the message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="state">return state</param>
        public StateResposeBase(State state)
        {
            State = state;
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="state">return state</param>
        /// <param name="message">message text</param>
        public StateResposeBase(State state, string message)
            : this(state)
        {
            Message = message;
        }
    }

    /// <summary>
    /// Response states
    /// </summary>
    public enum State
    {
        Ok = 0,
        Failed = 1
    }
}