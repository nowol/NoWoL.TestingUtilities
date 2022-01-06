using System;
using System.Collections.Generic;
using System.Text;

namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Represents the rules to apply for the different data types
    /// </summary>
    public class ParametersValidationRules : IParametersValidationRules
    {
        /// <summary>
        /// Gets the rules that will be applied for string parameters
        /// </summary>
        public IExpectedExceptionRule[] StringRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for value type parameters
        /// </summary>
        public IExpectedExceptionRule[] ValueTypesRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for collection types parameters (list, array, IEnumerable, dictionary)
        /// </summary>
        public IExpectedExceptionRule[] CollectionTypesRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for interface parameters
        /// </summary>
        public IExpectedExceptionRule[] InterfacesRules { get; set; }

        /// <summary>
        /// Gets the rules that will be applied for other parameters types such as classes
        /// </summary>
        public IExpectedExceptionRule[] OtherTypesRules { get; set; }
    }
}
