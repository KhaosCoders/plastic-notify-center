using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Data;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Implementation of IRuleConditionEvaluator service
    /// </summary>
    public class RuleConditionEvaluator : IRuleConditionEvaluator
    {
        #region Dependencies

        private readonly ILogger<RuleConditionEvaluator> _logger;

        public RuleConditionEvaluator(ILogger<RuleConditionEvaluator> logger)
        {
            this._logger = logger;
        }

        #endregion

        /// <summary>
        /// Evaluates a C# filter script for each rule and returns a list of the rules who passed the filter
        /// </summary>
        /// <param name="triggerRules">List of rules to evaluate</param>
        /// <param name="environmentVars">List of environment variables from Plastic SCM</param>
        /// <param name="inputLines">INPUT from Plastic SCM</param>
        public async IAsyncEnumerable<NotificationRule> EvalFilterAsync(IEnumerable<NotificationRule> triggerRules, Dictionary<string, string> environmentVars, string[] inputLines)
        {
            foreach (var rule in triggerRules)
            {
                if (string.IsNullOrWhiteSpace(rule.AdvancedFilter))
                {
                    // No filter: Pass
                    yield return rule;
                }
                else
                {
                    // Eval filter
                    if (await EvalFilterAsync(rule.AdvancedFilter, environmentVars, inputLines))
                    {
                        // Passed filter
                        yield return rule;
                    }
                }
            }
        }


        /// <summary>
        /// Trys to evaluate a filter expression to a boolean value
        /// Returns false if the expression can't be evaluated to a boolean value
        /// </summary>
        /// <param name="code">C# filter expression code</param>
        /// <param name="environmentVars">List of environment variables from Plastic SCM</param>
        /// <param name="inputLines">INPUT from Plastic SCM</param>
        private async Task<bool> EvalFilterAsync(string code, Dictionary<string, string> environmentVars, string[] inputLines)
        {
            try
            {
                _logger.LogDebug("Evaluate filter: {filter}", code);

                // evaluate (cancel execution after 20 seconds)
                var state = await CSharpScript.EvaluateAsync(code,
                                    globals: new GlobalScriptVars(environmentVars, inputLines),
                                    cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token,
                                    options: ScriptOptions.Default
                                                .WithImports("System", "System.Text", "System.Linq", "System.Collections.Generic")
                                                .WithReferences(
                                                    typeof(System.Text.ASCIIEncoding).Assembly,
                                                    typeof(System.Linq.Enumerable).Assembly,
                                                    typeof(System.Collections.Generic.Stack<string>).Assembly));

                // result
                _logger.LogDebug("Filter evaluated to: {state}", state);
                if (state is bool result)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while evaluating filter: {filter}", code);
            }
            return false;
        }

        /// <summary>
        /// Global variable context for script execution
        /// </summary>
        public class GlobalScriptVars
        {
            public Dictionary<string, string> EnvVars { get; }
            public string[] Input { get; }

            public GlobalScriptVars(Dictionary<string, string> environmentVars, string[] inputLines)
            {
                EnvVars = environmentVars;
                Input = inputLines;
            }
        }
    }
}