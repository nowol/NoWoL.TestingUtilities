using System;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.TestingUtilities.Exceptions
{
    /// <summary>
    /// Parameter formatter for the exception generator
    /// </summary>
    internal static class ExceptionFormatters
    {
        /// <summary>
        /// Return the full name of the specified type
        /// </summary>
        /// <param name="type">The input type</param>
        /// <returns>The full name of the type</returns>
        public static string TypeFullNameFormatter(Type type)
        {
            return type?.FullName ?? String.Empty;
        }

        /// <summary>
        /// Convert the input list to a comma separated string
        /// </summary>
        /// <param name="values">The input values</param>
        /// <returns>A comma separated string</returns>
        public static string JoinCommaFormatter(System.Collections.Generic.IEnumerable<string> values)
        {
            if (values == null)
            {
                return String.Empty;
            }

            return String.Join(", ",
                               values);
        }

        /// <summary>
        /// Add a space before the input value
        /// </summary>
        /// <param name="value">The input value</param>
        /// <returns>The input value with a space before it if it is not empty</returns>
        public static string AddSpaceWhenRequiredFormatter(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return String.Empty;
            }

            return " " + value;
        }
    }
}
