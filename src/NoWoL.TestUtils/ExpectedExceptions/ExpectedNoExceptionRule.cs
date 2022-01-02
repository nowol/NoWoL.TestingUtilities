using System;
using System.Reflection;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    /// <summary>
    /// Rule that is used when no exception occurs
    /// </summary>
    public class ExpectedNoExceptionRule : IExpectedExceptionRule
    {
        internal const string MissingException = "An exception was thrown when no exception was expected. Details: ";

        /// <summary>
        /// Gets the name of the rule
        /// </summary>
        public virtual string Name => nameof(ExpectedNoExceptionRule);

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
                additionalReason = null;

                return true;
            }

            additionalReason = MissingException + ex.Message;

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