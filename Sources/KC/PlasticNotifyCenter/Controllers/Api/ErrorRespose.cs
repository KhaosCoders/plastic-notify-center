using System;

namespace PlasticNotifyCenter.Controllers.Api
{
    /// <summary>
    /// A error/exception response
    /// </summary>
    public class ErrorRespose : StateResposeBase
    {
        public ErrorRespose(Exception ex)
            : base(State.Failed, ex.Message)
        {
        }
    }
}