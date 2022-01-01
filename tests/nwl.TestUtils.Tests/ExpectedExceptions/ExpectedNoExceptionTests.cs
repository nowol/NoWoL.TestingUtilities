using System;
using nwl.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace nwl.TestingUtilities.Tests.ExpectedExceptions
{
    public class ExpectedNoExceptionTests : ExpectedExceptionBaseTests<ExpectedNoException>
    {
        protected override object GetInvalidParameterValueExpectedValue()
        {
            return GetInvalidParameterValueDefaultValue();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValue()
        {
            Assert.Null(_sut.GetInvalidParameterValue(MethodsHolder.GetStringParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInvalidParameterValueThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ExpectedNoException(), nameof(ExpectedNoException.GetInvalidParameterValue), methodArguments: new object[] { MethodsHolder.GetStringParameterInfo(), null });

            validator.SetupParameter("param", ExpectedExceptionRules.NotNull)
                     .SetupParameter("defaultValue", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsFalseIfExceptionIsNotNull()
        {
            var result = _sut.Evaluate("paramName",
                                       new ArgumentNullException("paramName"),
                                       out var additionalMessage);
            Assert.False(result);
            Assert.Equal(ExpectedNoException.MissingException,
                         additionalMessage);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsTrueIfExceptionIsNull()
        {
            var result = _sut.Evaluate("paramName",
                                       null,
                                       out var additionalMessage);
            Assert.True(result);
            Assert.Null(additionalMessage);
        }
    }
}
