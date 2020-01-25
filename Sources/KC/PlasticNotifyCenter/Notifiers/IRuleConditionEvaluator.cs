using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlasticNotifyCenter.Data;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// A service that evaluates C# scripts
    /// </summary>
    public interface IRuleConditionEvaluator
    {
        /// <summary>
        /// Evaluates a C# filter script for each rule and returns a list of the rules who passed the filter
        /// </summary>
        /// <param name="triggerRules">List of rules to evaluate</param>
        /// <param name="environmentVars">List of environment variables from Plastic SCM</param>
        /// <param name="inputLines">INPUT from Plastic SCM</param>
        IAsyncEnumerable<NotificationRule> EvalFilterAsync(IEnumerable<NotificationRule> triggerRules, Dictionary<string, string> environmentVars, string[] inputLines);
    }
}