using System;
using System.Reflection;

namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Provides a way to validate how to handles exceptions thrown by a method's validation
    /// </summary>
    public interface IExpectedExceptionRule
    {
        /// <summary>
        /// Gets the name of the rule
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Evaluates if the state of the exception
        /// </summary>
        /// <param name="paramName">Expected name of the parameter that thrown the exception</param>
        /// <param name="ex">Exception that was thrown</param>
        /// <param name="additionalReason">A way to add additional information of the failure</param>
        /// <returns><c>true</c> if the evaluation was successful; otherwise, <c>false</c>.</returns>
        bool Evaluate(string paramName, Exception ex, out string additionalReason);

        /// <summary>
        /// Generate an invalid value for this rule
        /// </summary>
        /// <param name="parameterInfo">ParameterInfo for the targeted parameter.</param>
        /// <param name="defaultValue">Original value of the parameter.</param>
        /// <returns>An invalid value for the rule.</returns>
        object GetInvalidParameterValue(ParameterInfo parameterInfo, object defaultValue);
    }
}