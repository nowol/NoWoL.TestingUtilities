using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    /// <summary>
    /// Rule that is used when we want to ignore a parameter during the validation process
    /// </summary>
    public class SkipParameterExceptionRule : IExpectedExceptionRule
    {
        /// <summary>
        /// Gets the name of the rule
        /// </summary>
        public virtual string Name => nameof(SkipParameterExceptionRule);

        /// <summary>
        /// Detect if <paramref name="rule"/> is an instance of <see cref="SkipParameterExceptionRule"/>
        /// </summary>
        /// <param name="rule">Rule to analyze</param>
        /// <returns>True if <paramref name="rule"/> is an instance of <see cref="SkipParameterExceptionRule"/> otherwise false</returns>
        public static bool IsSkipParameterExceptionRule(IExpectedExceptionRule rule)
        {
            if (rule == null)
            {
                return false;
            }

            return rule is SkipParameterExceptionRule;
        }

        /// <summary>
        /// Detect if <paramref name="rules"/> contains an instance of <see cref="SkipParameterExceptionRule"/>
        /// </summary>
        /// <param name="rules">Rules to analyze</param>
        /// <returns>True if <paramref name="rules"/> contains an instance of <see cref="SkipParameterExceptionRule"/> otherwise false</returns>
        public static bool IsSkipParameterExceptionRule(IEnumerable<IExpectedExceptionRule> rules)
        {
            if (rules == null)
            {
                return false;
            }

            return rules.Any(r => r is SkipParameterExceptionRule);
        }

        /// <summary>
        /// Evaluates if the state of the exception
        /// </summary>
        /// <param name="paramName">Expected name of the parameter that thrown the exception</param>
        /// <param name="ex">Exception that was thrown</param>
        /// <param name="additionalReason">A way to add additional information of the failure</param>
        /// <returns><c>true</c> if the evaluation was successful; otherwise, <c>false</c>.</returns>
        public virtual bool Evaluate(string paramName, Exception ex, out string additionalReason)
        {
            additionalReason = null;
            return false;
        }

        /// <summary>
        /// Generate an invalid value for this rule
        /// </summary>
        /// <param name="parameterInfo">ParameterInfo for the targeted parameter.</param>
        /// <param name="defaultValue">Original value of the parameter.</param>
        /// <returns>An invalid value for the rule.</returns>
        public virtual object GetInvalidParameterValue(ParameterInfo parameterInfo, object defaultValue)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return defaultValue;
        }
    }
}