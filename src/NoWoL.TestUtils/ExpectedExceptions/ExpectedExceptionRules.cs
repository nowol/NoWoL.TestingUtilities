namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    /// <summary>
    /// Group of common rules
    /// </summary>
    public static class ExpectedExceptionRules
    {
        /// <summary>
        /// Gets a rule when no exception occurs.
        /// </summary>
        public static IExpectedExceptionRule None => new ExpectedNoExceptionRule();
        
        /// <summary>
        /// Gets a rule when a null value is not allowed.
        /// </summary>
        public static IExpectedExceptionRule NotNull => new ExpectedNotNullExceptionRule();
        
        /// <summary>
        /// Gets a rule when a value cannot be null.
        /// </summary>
        public static IExpectedExceptionRule NotEmpty => new ExpectedNotEmptyExceptionRule();
        
        /// <summary>
        /// Gets a rule when a value cannot be null, empty or white-space.
        /// </summary>
        public static IExpectedExceptionRule NotEmptyOrWhiteSpace => new ExpectedNotEmptyOrWhiteSpaceExceptionRule();

        /// <summary>
        /// Get a rule with a specific invalid value.
        /// </summary>
        /// <typeparam name="T">Type of the invalid value.</typeparam>
        /// <param name="invalidValue">The invalid value.</param>
        /// <returns>An instance of <see cref="ExpectedExceptionRuleWithInvalidValue{T}"/></returns>
        public static IExpectedExceptionRule NotValue<T>(T invalidValue) => new ExpectedExceptionRuleWithInvalidValue<T>(invalidValue);
    }
}