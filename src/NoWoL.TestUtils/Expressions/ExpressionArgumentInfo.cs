namespace NoWoL.TestingUtilities.Expressions
{
    /// <summary>
    /// Information about an argument
    /// </summary>
    internal class ExpressionArgumentInfo
    {
        /// <summary>
        /// Gets or sets the name of the argument
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the arguments. This is expected to always be the default value of the type.
        /// </summary>
        public object Value { get; set; }
    }
}