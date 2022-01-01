namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    public static class ExpectedExceptionRules
    {
        public static IExpectedException None => new ExpectedNoException();
        
        public static IExpectedException NotNull => new ExpectedNotNullException();
        
        public static IExpectedException NotEmpty => new ExpectedNotEmptyException();
        
        public static IExpectedException NotEmptyOrWhiteSpace => new ExpectedNotEmptyOrWhiteSpaceException();

        public static IExpectedException NotValue<T>(T invalidValue) => new ExpectedExceptionWithInvalidValue<T>(invalidValue);
    }
}