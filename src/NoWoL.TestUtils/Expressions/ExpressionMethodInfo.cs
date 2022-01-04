using System.Collections.Generic;
using System.Reflection;

namespace NoWoL.TestingUtilities.Expressions
{
    /// <summary>
    /// Information about a method and its arguments
    /// </summary>
    internal class ExpressionMethodInfo
    {
        /// <summary>
        /// Method analyzed
        /// </summary>
        public MethodBase Method { get; set; }

        /// <summary>
        /// Name of the method that was analyzed
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets the method's arguments with their default value
        /// </summary>
        public List<ExpressionArgumentInfo> ArgumentValues { get; } = new();
    }
}
