using nwl.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace nwl.TestingUtilities.Tests.ExpectedExceptions
{
    public class ExpectedExceptionRulesTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void NoneReturnsExpectedType()
        {
            Assert.IsType<ExpectedNoException>(ExpectedExceptionRules.None);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotNullReturnsExpectedType()
        {
            Assert.IsType<ExpectedNotNullException>(ExpectedExceptionRules.NotNull);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEmptyReturnsExpectedType()
        {
            Assert.IsType<ExpectedNotEmptyException>(ExpectedExceptionRules.NotEmpty);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEmptyOrWhiteSpaceReturnsExpectedType()
        {
            Assert.IsType<ExpectedNotEmptyOrWhiteSpaceException>(ExpectedExceptionRules.NotEmptyOrWhiteSpace);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotValueReturnsExpectedType()
        {
            Assert.IsType<ExpectedExceptionWithInvalidValue<int>>(ExpectedExceptionRules.NotValue(3));
            Assert.IsType<ExpectedExceptionWithInvalidValue<double>>(ExpectedExceptionRules.NotValue(3D));
            Assert.IsType<ExpectedExceptionWithInvalidValue<decimal>>(ExpectedExceptionRules.NotValue(3M));
            Assert.IsType<ExpectedExceptionWithInvalidValue<string>>(ExpectedExceptionRules.NotValue("string value"));
        }
    }
}
