using NoWoL.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ExpectedExceptions
{
    public class ExpectedExceptionRulesTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void NoneReturnsExpectedType()
        {
            Assert.IsType<ExpectedNoExceptionRule>(ExpectedExceptionRules.None);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotNullReturnsExpectedType()
        {
            Assert.IsType<ExpectedNotNullExceptionRule>(ExpectedExceptionRules.NotNull);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEmptyReturnsExpectedType()
        {
            Assert.IsType<ExpectedNotEmptyExceptionRule>(ExpectedExceptionRules.NotEmpty);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotEmptyOrWhiteSpaceReturnsExpectedType()
        {
            Assert.IsType<ExpectedNotEmptyOrWhiteSpaceExceptionRule>(ExpectedExceptionRules.NotEmptyOrWhiteSpace);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void NotValueReturnsExpectedType()
        {
            Assert.IsType<ExpectedExceptionRuleWithInvalidValue<int>>(ExpectedExceptionRules.NotValue(3));
            Assert.IsType<ExpectedExceptionRuleWithInvalidValue<double>>(ExpectedExceptionRules.NotValue(3D));
            Assert.IsType<ExpectedExceptionRuleWithInvalidValue<decimal>>(ExpectedExceptionRules.NotValue(3M));
            Assert.IsType<ExpectedExceptionRuleWithInvalidValue<string>>(ExpectedExceptionRules.NotValue("string value"));
        }
    }
}
