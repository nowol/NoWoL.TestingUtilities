namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Represents the rules to apply for the different data types
    /// </summary>
    public interface IArgumentsValidationRules
    {
        /// <summary>
        /// Gets the rules that will be applied for string arguments
        /// </summary>
        IExpectedExceptionRule[] StringRules { get; }

        /// <summary>
        /// Gets the rules that will be applied for value type arguments
        /// </summary>
        IExpectedExceptionRule[] ValueTypesRules { get; }

        /// <summary>
        /// Gets the rules that will be applied for collection types arguments (list, array, IEnumerable, dictionary)
        /// </summary>
        IExpectedExceptionRule[] CollectionTypesRules { get; }

        /// <summary>
        /// Gets the rules that will be applied for interface arguments
        /// </summary>
        IExpectedExceptionRule[] InterfacesRules { get; }

        /// <summary>
        /// Gets the rules that will be applied for other arguments types such as classes
        /// </summary>
        IExpectedExceptionRule[] OtherTypesRules { get; }
    }
}