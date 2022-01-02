using System;
using System.Reflection;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    /// <summary>
    /// Base class for the expected exception rules. It provides common code to every rules
    /// </summary>
    public abstract class ExpectedExceptionRuleBase : IExpectedExceptionRule
    {
        internal const string NoExceptionMessage = "An exception was expected but none happened.";

        /// <summary>
        /// Gets the name of the rule
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Evaluates if the state of the exception
        /// </summary>
        /// <param name="paramName">Expected name of the parameter that thrown the exception</param>
        /// <param name="ex">Exception that was thrown</param>
        /// <param name="additionalReason">A way to add additional information of the failure</param>
        /// <returns><c>true</c> if the evaluation was successful; otherwise, <c>false</c>.</returns>
        public virtual bool Evaluate(string paramName, Exception ex, out string additionalReason)
        {
            if (paramName == null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            if (ex == null)
            {
                additionalReason = NoExceptionMessage;
                return false;
            }

            additionalReason = null;

            return true;
        }

        /// <summary>
        /// Generate an invalid value for this rule
        /// </summary>
        /// <param name="parameterInfo">ParameterInfo for the targeted parameter.</param>
        /// <param name="defaultValue">Original value of the parameter.</param>
        /// <returns>An invalid value for the rule.</returns>
        public abstract object GetInvalidParameterValue(ParameterInfo parameterInfo, object defaultValue);
    }
}