using System;
using System.Reflection;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    /// <summary>
    /// Rule when a value cannot be null, empty or white-space.
    /// </summary>
    public class ExpectedNotEmptyOrWhiteSpaceExceptionRule : ExpectedExceptionRuleBase
    {
        internal const string DefaultInvalidValue = "   \n   ";

        /// <summary>
        /// Gets the name of the rule
        /// </summary>
        public override string Name => nameof(ExpectedNotEmptyOrWhiteSpaceExceptionRule);

        /// <summary>
        /// Evaluates if the state of the exception
        /// </summary>
        /// <param name="paramName">Expected name of the parameter that thrown the exception</param>
        /// <param name="ex">Exception that was thrown</param>
        /// <param name="additionalReason">A way to add additional information of the failure</param>
        /// <returns><c>true</c> if the evaluation was successful; otherwise, <c>false</c>.</returns>
        public override bool Evaluate(string paramName, Exception ex, out string additionalReason)
        {
            if (!base.Evaluate(paramName, ex, out additionalReason))
            {
                return false;
            }
            else if (ex is ArgumentException ane)
            {
                if (!String.Equals(ane.ParamName, paramName, StringComparison.Ordinal))
                {
                    additionalReason = $"An ArgumentException for the parameter '{paramName}' was expected however the exception is for parameter '{ane.ParamName}'";
                    return false;
                }

                additionalReason = null;

                return true; // validate message content in an extensible way? e.g.: message contains "was empty"
            }
            else
            {
                additionalReason = null;
                return false;
            }
        }

        /// <summary>
        /// Generate an invalid value for this rule
        /// </summary>
        /// <param name="parameterInfo">ParameterInfo for the targeted parameter.</param>
        /// <param name="defaultValue">Original value of the parameter.</param>
        /// <returns>An invalid value for the rule.</returns>
        public override object GetInvalidParameterValue(ParameterInfo parameterInfo, object defaultValue)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return DefaultInvalidValue;
        }
    }
}