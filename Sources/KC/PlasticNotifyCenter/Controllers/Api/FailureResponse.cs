namespace PlasticNotifyCenter.Controllers.Api
{
    /// <summary>
    /// A failure response
    /// </summary>
    public class FailureResponse : StateResposeBase
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="message">Failure message</param>
        public FailureResponse(string message)
            : base(State.Failed, message)
        {
        }
    }
}