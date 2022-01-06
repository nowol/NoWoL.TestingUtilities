using System;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Represents the rules to apply for the different data types
    /// </summary>
    public class ArgumentsValidationRules : IArgumentsValidationRules
    {
        /// <summary>
        /// Gets the rules that will be applied for string arguments
        /// </summary>
        public IExpectedExceptionRule[] StringRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for value type arguments
        /// </summary>
        public IExpectedExceptionRule[] ValueTypesRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for collection types arguments (list, array, IEnumerable, dictionary)
        /// </summary>
        public IExpectedExceptionRule[] CollectionTypesRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for interface arguments
        /// </summary>
        public IExpectedExceptionRule[] InterfacesRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for other arguments types such as classes
        /// </summary>
        public IExpectedExceptionRule[] OtherTypesRules { get; set; }
    }
}
